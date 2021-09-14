//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BasePermissionScopeManager
    /// 数据权限表
    ///
    /// 修改记录
    ///
    ///		2010-07-15 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2010-07-15</date>
    /// </author>
    /// </summary>
    public partial class BasePermissionScopeManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BasePermissionScopeManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            CurrentTableName = BasePermissionScopeEntity.TableName;
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BasePermissionScopeManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BasePermissionScopeManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BasePermissionScopeManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BasePermissionScopeManager(BaseUserInfo userInfo, string tableName)
            : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BasePermissionScopeManager(IDbHelper dbHelper, BaseUserInfo userInfo)
            : this(dbHelper)
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BasePermissionScopeManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BasePermissionScopeEntity entity)
        {
            return AddObject(entity);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式</param>
        /// <param name="returnId">返回主鍵</param>
        /// <returns>主键</returns>
        public string Add(BasePermissionScopeEntity entity, bool identity, bool returnId)
        {
            Identity = identity;
            ReturnId = returnId;
            return AddObject(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BasePermissionScopeEntity entity)
        {
            return UpdateObject(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BasePermissionScopeEntity GetObject(int id)
        {
            return BaseEntity.Create<BasePermissionScopeEntity>(ExecuteReader(new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldId, id)));
            // return BaseEntity.Create<BasePermissionScopeEntity>(this.GetDataTable(new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddObject(BasePermissionScopeEntity entity)
        {
            var result = string.Empty;

            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BasePermissionScopeEntity.FieldId);

            // 若是非空主键，表明已经指定了主键了
            if (!string.IsNullOrEmpty(entity.Id))
            {
                // 这里已经是指定了主键了，所以不需要返回主键了
                sqlBuilder.SetValue(BasePermissionScopeEntity.FieldId, entity.Id);
                result = entity.Id;
            }
            else
            {
                if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                {
                    // 2015-12-23 吉日嘎拉 这里需要兼容一下以前的老的数据结构
                    sqlBuilder.SetFormula(BasePermissionScopeEntity.FieldId, "SEQ_" + BasePermissionEntity.TableName.ToUpper() + ".NEXTVAL ");

                    /*
                    if (DbHelper.CurrentDbType == CurrentDbType.DB2)
                    {
                        sqlBuilder.SetFormula(BasePermissionScopeEntity.FieldId, "NEXT VALUE FOR SEQ_" + BaseRoleEntity.TableName.ToUpper());
                    }
                    */
                }
                //MSSQL数据库是自增字段 Troy.Cui 2016-08-17
                //else
                //{
                //    entity.Id = Guid.NewGuid().ToString("N");
                //    result = entity.Id;
                //    sqlBuilder.SetValue(BasePermissionScopeEntity.FieldId, entity.Id);
                //}
            }
            
            SetObject(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BasePermissionScopeEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BasePermissionScopeEntity.FieldCreateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BasePermissionScopeEntity.FieldCreateTime);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BasePermissionScopeEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BasePermissionScopeEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BasePermissionScopeEntity.FieldUpdateTime);
            if (DbHelper.CurrentDbType == CurrentDbType.SqlServer && Identity)
            {
                result = sqlBuilder.EndInsert().ToString();
            }
            else
            {
                sqlBuilder.EndInsert();
            }

            return result;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        public int UpdateObject(BasePermissionScopeEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetObject(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BasePermissionScopeEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BasePermissionScopeEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BasePermissionScopeEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BasePermissionScopeEntity.FieldId, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        partial void SetObjectExpand(SqlBuilder sqlBuilder, BasePermissionScopeEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetObject(SqlBuilder sqlBuilder, BasePermissionScopeEntity entity)
        {
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldResourceCategory, entity.ResourceCategory);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldResourceId, entity.ResourceId);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldTargetCategory, entity.TargetCategory);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldTargetId, entity.TargetId);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldPermissionId, entity.PermissionId);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldContainChild, entity.ContainChild);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldPermissionConstraint, entity.PermissionConstraint);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldStartDate, entity.StartDate);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldEndDate, entity.EndDate);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldDeleted, entity.DeletionStateCode);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldDescription, entity.Description);
            SetObjectExpand(sqlBuilder, entity);
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
