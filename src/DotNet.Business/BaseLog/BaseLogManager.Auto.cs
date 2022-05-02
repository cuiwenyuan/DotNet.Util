//-----------------------------------------------------------------------
// <copyright file="BaseLogManager.Auto.cs" company="DotNet">
//     Copyright (c) 2019, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseLogManager
    /// 系统日志
    /// 
    /// 修改记录
    /// 
    /// 2019-07-02 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2019-07-02</date>
    /// </author>
    /// </summary>
    public partial class BaseLogManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseLogManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseLogEntity.CurrentTableName;
                //按用户公司分表
                //CurrentTableName = BaseLogEntity.CurrentTableName + GetTableSuffix();
            }
            CurrentTableDescription = FieldExtensions.ToDescription(typeof(BaseLogEntity), "CurrentTableName");
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseLogManager(string tableName) : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseLogManager(IDbHelper dbHelper) : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseLogManager(BaseUserInfo userInfo) : this()
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseLogEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseLogManager(BaseUserInfo userInfo, string tableName) : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseLogManager(IDbHelper dbHelper, BaseUserInfo userInfo) : this(dbHelper)
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseLogEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseLogManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName) : this(dbHelper, userInfo)
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
        public string Add(BaseLogEntity entity, bool identity = true, bool returnId = true)
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
        public string AddOrUpdate(BaseLogEntity entity, bool identity = true, bool returnId = true)
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
        public int Update(BaseLogEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseLogEntity GetEntity(string id)
        {
            return ValidateUtil.IsInt(id) ? GetEntity(int.Parse(id)) : null;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseLogEntity GetEntity(int id)
        {
            return BaseEntity.Create<BaseLogEntity>(ExecuteReader(new KeyValuePair<string, object>(PrimaryKey, id)));
            //var cacheKey = CurrentTableName + ".Entity." + id;
            //var cacheTime = TimeSpan.FromMilliseconds(86400000);
            //return CacheUtil.Cache<BaseLogEntity>(cacheKey, () => BaseEntity.Create<BaseLogEntity>(ExecuteReader(new KeyValuePair<string, object>(PrimaryKey, id))), true, false, cacheTime);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="parameters">参数</param>
        public BaseLogEntity GetEntity(List<KeyValuePair<string, object>> parameters)
        {
            return BaseEntity.Create<BaseLogEntity>(ExecuteReader(parameters));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseLogEntity entity)
        {
            if (!BaseSystemInfo.RecordLog)
            {
                return "0";
            }

            var key = string.Empty;
            if (entity.SortCode == 0)
            {
                //var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                //key = managerSequence.Increment(CurrentTableName);
                //entity.SortCode = int.Parse(key);
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
                        sqlBuilder.SetFormula(PrimaryKey, CurrentTableName.ToUpper() + "_SEQ.NEXTVAL ");
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
                sqlBuilder.SetValue(BaseLogEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseLogEntity.FieldCreateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseLogEntity.FieldCreateBy, UserInfo.RealName);
            }
            else
            {
                sqlBuilder.SetValue(BaseLogEntity.FieldCreateBy, entity.CreateBy);
                sqlBuilder.SetValue(BaseLogEntity.FieldCreateUserName, entity.CreateUserName);
            }
            sqlBuilder.SetDbNow(BaseLogEntity.FieldCreateTime);
            sqlBuilder.SetValue(BaseLogEntity.FieldCreateIp, Utils.GetIp());
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseLogEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseLogEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseLogEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseLogEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BaseLogEntity.FieldUpdateIp, Utils.GetIp());
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
        public int UpdateEntity(BaseLogEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseLogEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseLogEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseLogEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseLogEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BaseLogEntity.FieldUpdateIp, Utils.GetIp());
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
        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseLogEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">Sql语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseLogEntity entity)
        {
            SetEntityExtend(sqlBuilder, entity);
            sqlBuilder.SetValue(BaseLogEntity.FieldCompanyId, entity.CompanyId);
            sqlBuilder.SetValue(BaseLogEntity.FieldUserId, entity.UserId);
            sqlBuilder.SetValue(BaseLogEntity.FieldUserName, entity.UserName);
            sqlBuilder.SetValue(BaseLogEntity.FieldRealName, entity.RealName);
            sqlBuilder.SetValue(BaseLogEntity.FieldService, entity.Service);
            sqlBuilder.SetValue(BaseLogEntity.FieldTaskId, entity.TaskId);
            sqlBuilder.SetValue(BaseLogEntity.FieldParameters, entity.Parameters);
            sqlBuilder.SetValue(BaseLogEntity.FieldClientIp, entity.ClientIp);
            sqlBuilder.SetValue(BaseLogEntity.FieldServerIp, entity.ServerIp);
            sqlBuilder.SetValue(BaseLogEntity.FieldUrlReferrer, entity.UrlReferrer);
            sqlBuilder.SetValue(BaseLogEntity.FieldWebUrl, entity.WebUrl);
            sqlBuilder.SetValue(BaseLogEntity.FieldElapsedTicks, entity.ElapsedTicks);
            sqlBuilder.SetValue(BaseLogEntity.FieldStartTime, entity.StartTime);
            sqlBuilder.SetValue(BaseLogEntity.FieldDescription, entity.Description);
            sqlBuilder.SetValue(BaseLogEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseLogEntity.FieldDeleted, entity.Deleted);
            sqlBuilder.SetValue(BaseLogEntity.FieldEnabled, entity.Enabled);
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
