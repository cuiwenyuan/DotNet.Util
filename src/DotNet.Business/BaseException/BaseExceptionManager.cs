//-----------------------------------------------------------------------
// <copyright file="BaseExceptionManager.Auto.cs" company="DotNet">
//     Copyright (c) 2016, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseExceptionManager
    /// 系统异常表
    /// 
    /// 修改纪录
    /// 
    /// 2016-12-21 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2016-12-21</date>
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
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.BusinessDbType, BaseSystemInfo.BusinessDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseExceptionEntity.TableName;
            }
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseExceptionManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseExceptionManager(IDbHelper dbHelper): this()
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
            entity.Id = int.Parse(AddObject(entity));
            return entity.Id.ToString();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseExceptionEntity entity)
        {
            return UpdateObject(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseExceptionEntity GetObject(string id)
        {
            return GetObject(int.Parse(id));
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseExceptionEntity GetObject(int id)
        {
            //return BaseEntity.Create<BaseExceptionEntity>(this.GetDataTable(new KeyValuePair<string, object>(this.PrimaryKey, id)));
            return BaseEntity.Create<BaseExceptionEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseExceptionEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddObject(BaseExceptionEntity entity)
        {
            var key = string.Empty;
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
            SetObject(sqlBuilder, entity);
            if (UserInfo != null) 
            { 
                sqlBuilder.SetValue(BaseExceptionEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseExceptionEntity.FieldCreateBy, UserInfo.RealName);
            } 
            else 
            { 
                sqlBuilder.SetValue(BaseExceptionEntity.FieldCreateBy, entity.CreateBy); 
            } 
            sqlBuilder.SetDbNow(BaseExceptionEntity.FieldCreateTime);
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
                return entity.Id.ToString();
            }
            return key;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        public int UpdateObject(BaseExceptionEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetObject(sqlBuilder, entity);
            sqlBuilder.SetWhere(PrimaryKey, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        // 这个是声明扩展方法
        partial void SetObjectExpand(SqlBuilder sqlBuilder, BaseExceptionEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetObject(SqlBuilder sqlBuilder, BaseExceptionEntity entity)
        {
            SetObjectExpand(sqlBuilder, entity);
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
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(int id)
        {
            return Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(PrimaryKey, id) });
        }
    }
}
