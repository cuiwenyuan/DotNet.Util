//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BasePermissionManager
    /// 操作权限表
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
    public partial class BasePermissionManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BasePermissionManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            CurrentTableName = BasePermissionEntity.TableName;
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BasePermissionManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BasePermissionManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BasePermissionManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BasePermissionManager(IDbHelper dbHelper, BaseUserInfo userInfo)
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
        public BasePermissionManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BasePermissionEntity entity)
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
        public string Add(BasePermissionEntity entity, bool identity, bool returnId)
        {
            Identity = identity;
            ReturnId = returnId;
            return AddObject(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BasePermissionEntity entity)
        {
            return UpdateObject(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BasePermissionEntity GetObject(int id)
        {
            return BaseEntity.Create<BasePermissionEntity>(ExecuteReader(new KeyValuePair<string, object>(BasePermissionEntity.FieldId, id)));
            // return BaseEntity.Create<BasePermissionEntity>(this.GetDataTable(new KeyValuePair<string, object>(BasePermissionEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddObject(BasePermissionEntity entity)
        {
            var result = string.Empty;

            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BasePermissionEntity.FieldId);

            // 若是非空主键，表明已经指定了主键了
            if (!string.IsNullOrEmpty(entity.Id))
            {
                // 这里已经是指定了主键了，所以不需要返回主键了
                sqlBuilder.SetValue(BasePermissionEntity.FieldId, entity.Id);
                result = entity.Id;
            }
            else
            {
                if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                {
                    // 2015-12-23 吉日嘎拉 这里需要兼容一下以前的老的数据结构
                    sqlBuilder.SetFormula(BasePermissionEntity.FieldId, "SEQ_" + BasePermissionEntity.TableName.ToUpper() + ".NEXTVAL ");

                    /*
                    if (DbHelper.CurrentDbType == CurrentDbType.DB2)
                    {
                        sqlBuilder.SetFormula(BasePermissionEntity.FieldId, "NEXT VALUE FOR SEQ_" + BasePermissionEntity.TableName.ToUpper());
                    }
                    */
                }
                //MSSQL数据库是自增字段 Troy.Cui 2016-08-17
                //else
                //{
                //    entity.Id = Guid.NewGuid().ToString("N");
                //    result = entity.Id;
                //    sqlBuilder.SetValue(BasePermissionEntity.FieldId, entity.Id);
                //}
            }

            SetObject(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BasePermissionEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BasePermissionEntity.FieldCreateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BasePermissionEntity.FieldCreateTime);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BasePermissionEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BasePermissionEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BasePermissionEntity.FieldUpdateTime);
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
        public int UpdateObject(BasePermissionEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetObject(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BasePermissionEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BasePermissionEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BasePermissionEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BasePermissionEntity.FieldId, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        partial void SetObjectExpand(SqlBuilder sqlBuilder, BasePermissionEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetObject(SqlBuilder sqlBuilder, BasePermissionEntity entity)
        {
            sqlBuilder.SetValue(BasePermissionEntity.FieldResourceId, entity.ResourceId);
            sqlBuilder.SetValue(BasePermissionEntity.FieldResourceCategory, entity.ResourceCategory);
            sqlBuilder.SetValue(BasePermissionEntity.FieldPermissionId, entity.PermissionId);
            sqlBuilder.SetValue(BasePermissionEntity.FieldPermissionConstraint, entity.PermissionConstraint);
            sqlBuilder.SetValue(BasePermissionEntity.FieldCompanyId, entity.CompanyId);
            sqlBuilder.SetValue(BasePermissionEntity.FieldCompanyName, entity.CompanyName);
            sqlBuilder.SetValue(BasePermissionEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BasePermissionEntity.FieldDeleted, entity.DeletionStateCode);
            sqlBuilder.SetValue(BasePermissionEntity.FieldDescription, entity.Description);
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
