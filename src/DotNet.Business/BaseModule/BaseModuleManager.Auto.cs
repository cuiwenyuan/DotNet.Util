//--------------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//--------------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseModuleManager
    /// 模块（菜单）表
    /// 
    /// 修改记录
    /// 
    /// 2018-09-03 版本：2.0 Troy.Cui 增加RemoveCache删除缓存
    /// 2012-05-22 版本：1.0 JiRiGaLa 创建主键。
    /// 
    /// <author>
    ///     <name>JiRiGaLa</name>
    ///     <date>2012-05-22</date>
    /// </author>
    /// </summary>
    public partial class BaseModuleManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseModuleManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseModuleEntity.TableName;
            }
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseModuleManager(string tableName)
            : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseModuleManager(IDbHelper dbHelper): this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseModuleManager(BaseUserInfo userInfo) : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseModuleManager(IDbHelper dbHelper, BaseUserInfo userInfo) : this(dbHelper)
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseModuleManager(BaseUserInfo userInfo, string tableName)
            : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">指定表名</param>
        public BaseModuleManager(IDbHelper dbHelper, string tableName)
            : this(dbHelper)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseModuleManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName) : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseModuleEntity entity)
        {
            return AddEntity(entity);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式</param>
        /// <param name="returnId">返回主键</param>
        /// <returns>主键</returns>
        public string Add(BaseModuleEntity entity, bool identity, bool returnId)
        {
            Identity = identity;
            ReturnId = returnId;
            return AddEntity(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseModuleEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseModuleEntity GetEntity(string id)
        {
            return BaseEntity.Create<BaseModuleEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseModuleEntity.FieldId, id)));
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseModuleEntity GetEntity(int id)
        {
            return GetEntity(id.ToString());
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseModuleEntity entity)
        {
            var result = string.Empty;

            if (entity.SortCode == 0)
            {
                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                result = managerSequence.Increment(CurrentTableName);
                entity.SortCode = int.Parse(result);
            }

            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BaseModuleEntity.FieldId);

            // 若是非空主键，表明已经指定了主键了
            if (!string.IsNullOrEmpty(entity.Id))
            {
                // 这里已经是指定了主键了，所以不需要返回主键了
                sqlBuilder.SetValue(BaseModuleEntity.FieldId, entity.Id);
                result = entity.Id;
            }
            else
            {
                if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                {
                    // 2015-12-23 吉日嘎拉 这里需要兼容一下以前的老的数据结构
                    sqlBuilder.SetFormula(PrimaryKey, "SEQ_" + BaseModuleEntity.TableName.ToUpper() + ".NEXTVAL ");
                }
                //MSSQL数据库是自增字段 Troy.Cui 2016-08-17
                //else
                //{
                //    entity.Id = Guid.NewGuid().ToString("N");
                //    result = entity.Id;
                //    sqlBuilder.SetValue(BaseModuleEntity.FieldId, entity.Id);
                //}
            }

            SetEntity(sqlBuilder, entity);
            if (UserInfo != null) 
            { 
                sqlBuilder.SetValue(BaseModuleEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseModuleEntity.FieldCreateBy, UserInfo.RealName);
            } 
            sqlBuilder.SetDbNow(BaseModuleEntity.FieldCreateTime);
            if (UserInfo != null) 
            { 
                sqlBuilder.SetValue(BaseModuleEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseModuleEntity.FieldUpdateBy, UserInfo.RealName);
            } 
            sqlBuilder.SetDbNow(BaseModuleEntity.FieldUpdateTime);
            if (Identity && (DbHelper.CurrentDbType == CurrentDbType.SqlServer || DbHelper.CurrentDbType == CurrentDbType.Access))
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
        public int UpdateEntity(BaseModuleEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null) 
            { 
                sqlBuilder.SetValue(BaseModuleEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseModuleEntity.FieldUpdateBy, UserInfo.RealName);
            } 
            sqlBuilder.SetDbNow(BaseModuleEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BaseModuleEntity.FieldId, entity.Id);
            //return sqlBuilder.EndUpdate();
            var result = sqlBuilder.EndUpdate();
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }

        // 这个是声明扩展方法
        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseModuleEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseModuleEntity entity)
        {
            SetEntityExtend(sqlBuilder, entity);
            sqlBuilder.SetValue(BaseModuleEntity.FieldParentId, entity.ParentId);
            sqlBuilder.SetValue(BaseModuleEntity.FieldCode, entity.Code);
            sqlBuilder.SetValue(BaseModuleEntity.FieldFullName, entity.FullName);
            sqlBuilder.SetValue(BaseModuleEntity.FieldCategoryCode, entity.CategoryCode);
            sqlBuilder.SetValue(BaseModuleEntity.FieldImageIndex, entity.ImageIndex);
            sqlBuilder.SetValue(BaseModuleEntity.FieldSelectedImageIndex, entity.SelectedImageIndex);
            sqlBuilder.SetValue(BaseModuleEntity.FieldNavigateUrl, entity.NavigateUrl);
            sqlBuilder.SetValue(BaseModuleEntity.FieldImageUrl, entity.ImageUrl);
            sqlBuilder.SetValue(BaseModuleEntity.FieldTarget, entity.Target);
            sqlBuilder.SetValue(BaseModuleEntity.FieldFormName, entity.FormName);
            sqlBuilder.SetValue(BaseModuleEntity.FieldAssemblyName, entity.AssemblyName);
            // sqlBuilder.SetValue(BaseModuleEntity.FieldPermissionScopeTables, entity.PermissionScopeTables);            
            sqlBuilder.SetValue(BaseModuleEntity.FieldIsMenu, entity.IsMenu);
            sqlBuilder.SetValue(BaseModuleEntity.FieldIsPublic, entity.IsPublic);
            sqlBuilder.SetValue(BaseModuleEntity.FieldIsScope, entity.IsScope);
            sqlBuilder.SetValue(BaseModuleEntity.FieldIsVisible, entity.IsVisible);
            sqlBuilder.SetValue(BaseModuleEntity.FieldExpand, entity.Expand);
            sqlBuilder.SetValue(BaseModuleEntity.FieldAllowEdit, entity.AllowEdit);
            sqlBuilder.SetValue(BaseModuleEntity.FieldAllowDelete, entity.AllowDelete);
            sqlBuilder.SetValue(BaseModuleEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseModuleEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BaseModuleEntity.FieldDeleted, entity.DeletionStateCode);
            sqlBuilder.SetValue(BaseModuleEntity.FieldDescription, entity.Description);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(int id)
        {
            var result = Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(BaseModuleEntity.FieldId, id) });
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
			result.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldAllowDelete, 1));
			return result;
		}
    }
}
