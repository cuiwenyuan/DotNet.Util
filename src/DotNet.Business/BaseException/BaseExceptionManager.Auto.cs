//-----------------------------------------------------------------------
// <copyright file="BaseExceptionManager.Auto.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseExceptionManager
    /// 系统异常
    /// 
    /// 修改记录
    /// 
    /// 2021-09-28 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2021-09-28</date>
    /// </author>
    /// </summary>
    public partial class BaseExceptionManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseExceptionManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseExceptionEntity.CurrentTableName;
                //按用户公司分表
                //CurrentTableName = BaseExceptionEntity.CurrentTableName + GetTableSuffix();
            }
            CurrentTableDescription = FieldExtensions.ToDescription(typeof(BaseExceptionEntity), "CurrentTableName");
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseExceptionManager(string tableName) : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseExceptionManager(IDbHelper dbHelper) : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseExceptionManager(BaseUserInfo userInfo) : this()
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseExceptionEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseExceptionManager(BaseUserInfo userInfo, string tableName) : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseExceptionManager(IDbHelper dbHelper, BaseUserInfo userInfo) : this(dbHelper)
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseExceptionEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseExceptionManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName) : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加, 这里可以人工干预，提高程序的性能
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式，表主键是否采用自增的策略</param>
        /// <param name="returnId">返回主键，不返回程序允许速度会快，主要是为了主细表批量插入数据优化用的</param>
        /// <returns>主键</returns>
        public string Add(BaseExceptionEntity entity, bool identity = true, bool returnId = true)
        {
            Identity = identity;
            ReturnId = returnId;
            entity.Id = int.Parse(AddEntity(entity));
            return entity.Id.ToString();
        }

        /// <summary>
        /// 添加或更新(主键是否为0)
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式，表主键是否采用自增的策略</param>
        /// <param name="returnId">返回主键，不返回程序允许速度会快，主要是为了主细表批量插入数据优化用的</param>
        /// <returns>主键</returns>
        public string AddOrUpdate(BaseExceptionEntity entity, bool identity = true, bool returnId = true)
        {
            Identity = identity;
            ReturnId = returnId;
            if (entity.Id == 0)
            {
                entity.Id = int.Parse(AddEntity(entity));
                return entity.Id.ToString();
            }
            else
            {
                return UpdateEntity(entity) > 0 ? entity.Id.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseExceptionEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseExceptionEntity GetEntity(string id)
        {
            return ValidateUtil.IsInt(id) ? GetEntity(int.Parse(id)) : null;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseExceptionEntity GetEntity(int id)
        {
            return BaseEntity.Create<BaseExceptionEntity>(ExecuteReader(new KeyValuePair<string, object>(PrimaryKey, id)));
            //var cacheKey = CurrentTableName + ".Entity." + id;
            //var cacheTime = TimeSpan.FromMilliseconds(86400000);
            //return CacheUtil.Cache<BaseExceptionEntity>(cacheKey, () => BaseEntity.Create<BaseExceptionEntity>(ExecuteReader(new KeyValuePair<string, object>(PrimaryKey, id))), true, false, cacheTime);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="parameters">参数</param>
        public BaseExceptionEntity GetEntity(List<KeyValuePair<string, object>> parameters)
        {
            return BaseEntity.Create<BaseExceptionEntity>(ExecuteReader(parameters));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseExceptionEntity entity)
        {
            var key = string.Empty;
            if (entity.SortCode == 0)
            {
                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                key = managerSequence.Increment(CurrentTableName);
                entity.SortCode = int.Parse(key);
            }
            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, PrimaryKey);
            if (!Identity)
            {
                // 这里已经是指定了主键了，所以不需要返回主键了
                sqlBuilder.ReturnId = false;
                sqlBuilder.SetValue(PrimaryKey, entity.Id);
            }
            else
            {
                if (!ReturnId && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                {
                    if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                    {
                        sqlBuilder.SetFormula(PrimaryKey, "SEQ_" + CurrentTableName.ToUpper() + ".NEXTVAL ");
                    }
                    if (DbHelper.CurrentDbType == CurrentDbType.Db2)
                    {
                        sqlBuilder.SetFormula(PrimaryKey, "NEXT VALUE FOR SEQ_" + CurrentTableName.ToUpper());
                    }
                }
                else
                {
                    if (Identity && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                    {
                        var managerSequence = new BaseSequenceManager(DbHelper);
                        entity.Id = int.Parse(managerSequence.Increment(CurrentTableName));
                        sqlBuilder.SetValue(PrimaryKey, entity.Id);
                    }
                }
            }
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseExceptionEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseExceptionEntity.FieldCreateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseExceptionEntity.FieldCreateBy, UserInfo.RealName);
            }
            else
            {
                sqlBuilder.SetValue(BaseExceptionEntity.FieldCreateBy, entity.CreateBy);
                sqlBuilder.SetValue(BaseExceptionEntity.FieldCreateUserName, entity.CreateUserName);
            }
            sqlBuilder.SetDbNow(BaseExceptionEntity.FieldCreateTime);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldCreateIp, Utils.GetIp());
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseExceptionEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseExceptionEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseExceptionEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseExceptionEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldUpdateIp, Utils.GetIp());
            if (Identity && (DbHelper.CurrentDbType == CurrentDbType.SqlServer || DbHelper.CurrentDbType == CurrentDbType.Access))
            {
                key = sqlBuilder.EndInsert().ToString();
            }
            else
            {
                sqlBuilder.EndInsert();
            }
            if (Identity && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
            {
                //return entity.Id.ToString();
                key = entity.Id.ToString();
            }
            if (!string.IsNullOrWhiteSpace(key))
            {
                RemoveCache();
            }
            return key;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        public int UpdateEntity(BaseExceptionEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseExceptionEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseExceptionEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseExceptionEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseExceptionEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldUpdateIp, Utils.GetIp());
            sqlBuilder.SetWhere(PrimaryKey, entity.Id);
            //return sqlBuilder.EndUpdate();
            var result = sqlBuilder.EndUpdate();
            if (result > 0)
            {
                RemoveCache(entity.Id);
            }
            return result;
        }

        // 这个是声明扩展方法
        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseExceptionEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">Sql语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseExceptionEntity entity)
        {
            SetEntityExtend(sqlBuilder, entity);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldSystemCode, entity.SystemCode);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldEventId, entity.EventId);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldCategory, entity.Category);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldPriority, entity.Priority);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldSeverity, entity.Severity);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldTitle, entity.Title);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldTimestamp, entity.Timestamp);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldMachineName, entity.MachineName);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldIpAddress, entity.IpAddress);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldAppDomainName, entity.AppDomainName);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldProcessId, entity.ProcessId);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldProcessName, entity.ProcessName);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldThreadName, entity.ThreadName);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldWin32ThreadId, entity.Win32ThreadId);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldMessage, entity.Message);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldFormattedMessage, entity.FormattedMessage);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldDeleted, entity.Deleted);
            sqlBuilder.SetValue(BaseExceptionEntity.FieldEnabled, entity.Enabled);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(int id)
        {
            var result = Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(PrimaryKey, id) });
            if (result > 0)
            {
                RemoveCache(id);
            }
            return result;
        }
    }
}
