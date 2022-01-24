//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseRoleManager
    /// 角色表
    ///
    /// 修改记录
    ///
    ///     2018-09-07 版本：4.1 Troy.Cui   增加删除缓存功能。
    ///		2010-07-15 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2010-07-15</date>
    /// </author>
    /// </summary>
    public partial class BaseRoleManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseRoleManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseRoleEntity.TableName;
            }
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseRoleManager(string tableName) : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseRoleManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseRoleManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseRoleManager(IDbHelper dbHelper, BaseUserInfo userInfo)
            : this(dbHelper)
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">指定表名</param>
        public BaseRoleManager(IDbHelper dbHelper, string tableName)
            : this(dbHelper)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseRoleManager(BaseUserInfo userInfo, string tableName)
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
        public BaseRoleManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseRoleEntity entity)
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
        public string Add(BaseRoleEntity entity, bool identity, bool returnId)
        {
            Identity = identity;
            ReturnId = returnId;
            return AddEntity(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseRoleEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseRoleEntity GetEntity(string id)
        {
            return BaseEntity.Create<BaseRoleEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseRoleEntity.FieldId, id)));
            // return BaseEntity.Create<BaseRoleEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseRoleEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// 全局的角色不重复、多子系统之间的角色不重复，就厉害了，以后可以合并到一起也没关系了
        /// 通用基础子系统里的角色，可能在各子系统里也有权限需要设置的情况
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseRoleEntity entity)
        {
            var result = string.Empty;

            if (entity.SortCode == 0)
            {
                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                result = managerSequence.Increment(CurrentTableName);
                entity.SortCode = int.Parse(result);
            }

            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BaseRoleEntity.FieldId);

            // 若是非空主键，表明已经指定了主键了
            if (!string.IsNullOrEmpty(entity.Id))
            {
                // 这里已经是指定了主键了，所以不需要返回主键了
                sqlBuilder.SetValue(BaseRoleEntity.FieldId, entity.Id);
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
                //    sqlBuilder.SetValue(BaseRoleEntity.FieldId, entity.Id);
                //}
            }

            SetEntity(sqlBuilder, entity);

            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseRoleEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseRoleEntity.FieldCreateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseRoleEntity.FieldCreateTime);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseRoleEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseRoleEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseRoleEntity.FieldUpdateTime);

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
        public int UpdateEntity(BaseRoleEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseRoleEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseRoleEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseRoleEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BaseRoleEntity.FieldId, entity.Id);
            //return sqlBuilder.EndUpdate();
            var result = sqlBuilder.EndUpdate();
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }

        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseRoleEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseRoleEntity entity)
        {
            sqlBuilder.SetValue(BaseRoleEntity.FieldOrganizationId, entity.OrganizationId);
            sqlBuilder.SetValue(BaseRoleEntity.FieldCode, entity.Code);
            sqlBuilder.SetValue(BaseRoleEntity.FieldRealName, entity.RealName);
            sqlBuilder.SetValue(BaseRoleEntity.FieldCategoryCode, entity.CategoryCode);
            sqlBuilder.SetValue(BaseRoleEntity.FieldAllowEdit, entity.AllowEdit);
            sqlBuilder.SetValue(BaseRoleEntity.FieldAllowDelete, entity.AllowDelete);
            sqlBuilder.SetValue(BaseRoleEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseRoleEntity.FieldDeleted, entity.DeletionStateCode);
            sqlBuilder.SetValue(BaseRoleEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BaseRoleEntity.FieldIsVisible, entity.IsVisible);
            sqlBuilder.SetValue(BaseRoleEntity.FieldDescription, entity.Description);
            SetEntityExtend(sqlBuilder, entity);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(int id)
        {
            var result = Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(BaseRoleEntity.FieldId, id) });
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        /// <summary>
        /// 添加删除的附加条件
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
		protected override List<KeyValuePair<string, object>> GetDeleteExtParam(List<KeyValuePair<string, object>> parameters)
		{
			var result = base.GetDeleteExtParam(parameters);
			result.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldAllowDelete, 1));
			return result;
		}
    }
}