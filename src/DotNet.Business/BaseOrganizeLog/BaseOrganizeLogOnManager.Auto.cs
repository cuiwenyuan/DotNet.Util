//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseOrganizeLogOnManager
    /// 系统网点登录信息
    ///
    /// 修改记录
    ///
    ///		2016-03-24 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2016-03-24</date>
    /// </author>
    /// </summary>
    public partial class BaseOrganizeLogOnManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseOrganizeLogOnManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseOrganizeLogOnEntity.TableName;
            }
            // 不是自增量添加
            Identity = false;
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseOrganizeLogOnManager(string tableName)
            : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseOrganizeLogOnManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseOrganizeLogOnManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseOrganizeLogOnManager(IDbHelper dbHelper, BaseUserInfo userInfo)
            : this(dbHelper)
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseOrganizeLogOnManager(BaseUserInfo userInfo, string tableName)
            : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseOrganizeLogOnManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseOrganizeLogOnEntity entity)
        {
            return AddEntity(entity);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式</param>
        /// <param name="returnId">返回主鍵</param>
        /// <returns>主键</returns>
        public string Add(BaseOrganizeLogOnEntity entity, bool identity, bool returnId)
        {
            Identity = identity;
            ReturnId = returnId;
            return AddEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseOrganizeLogOnEntity GetEntity(int? id)
        {
            return BaseEntity.Create<BaseOrganizeLogOnEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseOrganizeLogOnEntity.FieldId, id)));
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseOrganizeLogOnEntity GetEntity(string id)
        {
            return BaseEntity.Create<BaseOrganizeLogOnEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseOrganizeLogOnEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseOrganizeLogOnEntity entity)
        {
            var result = string.Empty;
            if (string.IsNullOrEmpty(entity.Id))
            {
                var manager = new BaseSequenceManager(DbHelper, Identity);
                result = manager.Increment(CurrentTableName);
                entity.Id = result;
            }
            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BaseOrganizeLogOnEntity.FieldId);
            if (!Identity)
            {
                sqlBuilder.SetValue(BaseOrganizeLogOnEntity.FieldId, entity.Id);
            }
            else
            {
                if (!ReturnId && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                {
                    if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                    {
                        sqlBuilder.SetFormula(BaseOrganizeLogOnEntity.FieldId, "SEQ_" + CurrentTableName.ToUpper() + ".NEXTVAL ");
                    }
                    if (DbHelper.CurrentDbType == CurrentDbType.Db2)
                    {
                        sqlBuilder.SetFormula(BaseOrganizeLogOnEntity.FieldId, "NEXT VALUE FOR SEQ_" + CurrentTableName.ToUpper());
                    }
                }
                else
                {
                    if (Identity && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                    {
                        if (entity.Id == null)
                        {
                            if (string.IsNullOrEmpty(result))
                            {
                                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                                result = managerSequence.Increment(CurrentTableName);
                            }
                            entity.Id = result;
                        }
                        sqlBuilder.SetValue(BaseOrganizeLogOnEntity.FieldId, entity.Id);
                    }
                }
            }
            SetEntity(sqlBuilder, entity);
            sqlBuilder.SetDbNow(BaseOrganizeLogOnEntity.FieldUpdateTime);
            if (DbHelper.CurrentDbType == CurrentDbType.SqlServer && Identity)
            {
                result = sqlBuilder.EndInsert().ToString();
            }
            else
            {
                sqlBuilder.EndInsert();
                result = entity.Id;
            }
            return result;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        public int UpdateEntity(BaseOrganizeLogOnEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            sqlBuilder.SetDbNow(BaseOrganizeLogOnEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BaseOrganizeLogOnEntity.FieldId, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseOrganizeLogOnEntity entity)
        {
            sqlBuilder.SetValue(BaseOrganizeLogOnEntity.FieldAgree, entity.Agree);
            sqlBuilder.SetValue(BaseOrganizeLogOnEntity.FieldOppose, entity.Oppose);
            sqlBuilder.SetValue(BaseOrganizeLogOnEntity.FieldFirstVisit, entity.FirstVisit);
            sqlBuilder.SetValue(BaseOrganizeLogOnEntity.FieldLastVisit, entity.LastVisit);
            sqlBuilder.SetValue(BaseOrganizeLogOnEntity.FieldShowCount, entity.ShowCount);
            sqlBuilder.SetValue(BaseOrganizeLogOnEntity.FieldLogOnCount, entity.LogOnCount);
            sqlBuilder.SetValue(BaseOrganizeLogOnEntity.FieldUserOnLine, entity.UserOnLine);
            sqlBuilder.SetValue(BaseOrganizeLogOnEntity.FieldIpAddress, entity.IpAddress);
        }
    }
}
