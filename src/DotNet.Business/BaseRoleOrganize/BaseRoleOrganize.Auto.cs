//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseRoleOrganizeManager
    /// 角色-组织机构 关系
    ///
    /// 修改记录
    ///
    ///		2014-04-15 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2014-04-15</date>
    /// </author>
    /// </summary>
    public partial class BaseRoleOrganizeManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseRoleOrganizeManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            CurrentTableName = BaseRoleOrganizeEntity.TableName;
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseRoleOrganizeManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseRoleOrganizeManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseRoleOrganizeManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseRoleOrganizeManager(IDbHelper dbHelper, BaseUserInfo userInfo)
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
        public BaseRoleOrganizeManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseRoleOrganizeEntity entity)
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
        public string Add(BaseRoleOrganizeEntity entity, bool identity, bool returnId)
        {
            Identity = identity;
            ReturnId = returnId;
            return AddEntity(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseRoleOrganizeEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseRoleOrganizeEntity GetEntity(int id)
        {
            return BaseEntity.Create<BaseRoleOrganizeEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseRoleOrganizeEntity.FieldId, id)));
            // return BaseEntity.Create<BaseRoleOrganizeEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseRoleOrganizeEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseRoleOrganizeEntity entity)
        {
            var result = string.Empty;

            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BaseRoleOrganizeEntity.FieldId);

            // 若是非空主键，表明已经指定了主键了
            if (!string.IsNullOrEmpty(entity.Id))
            {
                // 这里已经是指定了主键了，所以不需要返回主键了
                sqlBuilder.SetValue(BaseRoleOrganizeEntity.FieldId, entity.Id);
                result = entity.Id;
            }
            else
            {
                if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                {
                    // 2015-12-23 吉日嘎拉 这里需要兼容一下以前的老的数据结构
                    sqlBuilder.SetFormula(PrimaryKey, "SEQ_" + BaseRoleEntity.TableName.ToUpper() + ".NEXTVAL ");
                }
                //MSSQL数据库是自增字段 Troy.Cui 2016-08-17
                //else
                //{
                //    entity.Id = Guid.NewGuid().ToString("N");
                //    result = entity.Id;
                //    sqlBuilder.SetValue(BaseRoleOrganizeEntity.FieldId, entity.Id);
                //}
            }

            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseRoleOrganizeEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseRoleOrganizeEntity.FieldCreateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseRoleOrganizeEntity.FieldCreateTime);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseRoleOrganizeEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseRoleOrganizeEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseRoleOrganizeEntity.FieldUpdateTime);
            if (DbHelper.CurrentDbType == CurrentDbType.SqlServer && Identity)
            {
                result = sqlBuilder.EndInsert().ToString();
            }
            else
            {
                sqlBuilder.EndInsert();
            }
            if (!string.IsNullOrWhiteSpace(result))
            {
                RemoveCache();
            }
            return result;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        public int UpdateEntity(BaseRoleOrganizeEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseRoleOrganizeEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseRoleOrganizeEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseRoleOrganizeEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BaseRoleOrganizeEntity.FieldId, entity.Id);
            //return sqlBuilder.EndUpdate();
            var result = sqlBuilder.EndUpdate();
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }

        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseRoleOrganizeEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseRoleOrganizeEntity entity)
        {
            sqlBuilder.SetValue(BaseRoleOrganizeEntity.FieldOrganizeId, entity.OrganizeId);
            sqlBuilder.SetValue(BaseRoleOrganizeEntity.FieldRoleId, entity.RoleId);
            sqlBuilder.SetValue(BaseRoleOrganizeEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BaseRoleOrganizeEntity.FieldDescription, entity.Description);
            sqlBuilder.SetValue(BaseRoleOrganizeEntity.FieldDeleted, entity.DeletionStateCode);
            SetEntityExtend(sqlBuilder, entity);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(int id)
        {
            var result = Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(BaseRoleOrganizeEntity.FieldId, id) });
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }        
    }
}
