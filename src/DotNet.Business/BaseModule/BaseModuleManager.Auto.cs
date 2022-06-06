//-----------------------------------------------------------------------
// <copyright file="BaseModuleManager.Auto.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseModuleManager
    /// 模块菜单操作
    /// 
    /// 修改记录
    /// 
    /// 2021-09-26 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2021-09-26</date>
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
                dbHelper = DbHelperFactory.Create(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseModuleEntity.CurrentTableName;
                //按用户公司分表
                //CurrentTableName = BaseModuleEntity.CurrentTableName + GetTableSuffix();
            }
            CurrentTableDescription = FieldExtensions.ToDescription(typeof(BaseModuleEntity), "CurrentTableName");
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseModuleManager(string tableName) : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseModuleManager(IDbHelper dbHelper) : this()
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
            //按用户公司分表
            //CurrentTableName = BaseModuleEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseModuleManager(BaseUserInfo userInfo, string tableName) : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseModuleManager(IDbHelper dbHelper, BaseUserInfo userInfo) : this(dbHelper)
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseModuleEntity.CurrentTableName + GetTableSuffix();
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
        /// 添加, 这里可以人工干预，提高程序的性能
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式，表主键是否采用自增的策略</param>
        /// <param name="returnId">返回主键，不返回程序允许速度会快，主要是为了主细表批量插入数据优化用的</param>
        /// <returns>主键</returns>
        public string Add(BaseModuleEntity entity, bool identity = true, bool returnId = true)
        {
            Identity = identity;
            ReturnId = returnId;
            entity.Id = AddEntity(entity).ToInt();
            return entity.Id.ToString();
        }

        /// <summary>
        /// 添加或更新(主键是否为0)
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式，表主键是否采用自增的策略</param>
        /// <param name="returnId">返回主键，不返回程序允许速度会快，主要是为了主细表批量插入数据优化用的</param>
        /// <returns>主键</returns>
        public string AddOrUpdate(BaseModuleEntity entity, bool identity = true, bool returnId = true)
        {
            Identity = identity;
            ReturnId = returnId;
            if (entity.Id == 0)
            {
                entity.Id = AddEntity(entity).ToInt();
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
            return ValidateUtil.IsInt(id) ? GetEntity(id.ToInt()) : null;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseModuleEntity GetEntity(int id)
        {
            return BaseEntity.Create<BaseModuleEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id)));
            //var cacheKey = CurrentTableName + ".Entity." + id;
            //var cacheTime = TimeSpan.FromMilliseconds(86400000);
            //return CacheUtil.Cache<BaseModuleEntity>(cacheKey, () => BaseEntity.Create<BaseModuleEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id))), true, false, cacheTime);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="parameters">参数</param>
        public BaseModuleEntity GetEntity(List<KeyValuePair<string, object>> parameters)
        {
            return BaseEntity.Create<BaseModuleEntity>(GetDataTable(parameters));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseModuleEntity entity)
        {
            var key = string.Empty;
            if (entity.SortCode == 0)
            {
                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                key = managerSequence.Increment(CurrentTableName);
                entity.SortCode = key.ToInt();
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
                        sqlBuilder.SetFormula(PrimaryKey, CurrentTableName.ToUpper() + "_SEQ.NEXTVAL");
                    }
                    if (DbHelper.CurrentDbType == CurrentDbType.Db2)
                    {
                        sqlBuilder.SetFormula(PrimaryKey, "NEXT VALUE FOR " + CurrentTableName.ToUpper() + "_SEQ");
                    }
                }
                else
                {
                    if (Identity && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                    {
                        var managerSequence = new BaseSequenceManager(DbHelper);
                        entity.Id = managerSequence.Increment(CurrentTableName).ToInt();
                        sqlBuilder.SetValue(PrimaryKey, entity.Id);
                    }
                }
            }
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseModuleEntity.FieldCreateUserId, UserInfo.UserId);
                sqlBuilder.SetValue(BaseModuleEntity.FieldCreateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseModuleEntity.FieldCreateBy, UserInfo.RealName);
            }
            else
            {
                sqlBuilder.SetValue(BaseModuleEntity.FieldCreateBy, entity.CreateBy);
                sqlBuilder.SetValue(BaseModuleEntity.FieldCreateUserName, entity.CreateUserName);
            }
            sqlBuilder.SetDbNow(BaseModuleEntity.FieldCreateTime);
            sqlBuilder.SetValue(BaseModuleEntity.FieldCreateIp, Utils.GetIp());
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseModuleEntity.FieldUpdateUserId, UserInfo.UserId);
                sqlBuilder.SetValue(BaseModuleEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseModuleEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseModuleEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BaseModuleEntity.FieldUpdateIp, Utils.GetIp());
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
        public int UpdateEntity(BaseModuleEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseModuleEntity.FieldUpdateUserId, UserInfo.UserId);
                sqlBuilder.SetValue(BaseModuleEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseModuleEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseModuleEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BaseModuleEntity.FieldUpdateIp, Utils.GetIp());
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
        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseModuleEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">Sql语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseModuleEntity entity)
        {
            SetEntityExtend(sqlBuilder, entity);
            sqlBuilder.SetValue(BaseModuleEntity.FieldSystemCode, entity.SystemCode);
            sqlBuilder.SetValue(BaseModuleEntity.FieldParentId, entity.ParentId);
            sqlBuilder.SetValue(BaseModuleEntity.FieldCode, entity.Code);
            sqlBuilder.SetValue(BaseModuleEntity.FieldName, entity.Name);
            sqlBuilder.SetValue(BaseModuleEntity.FieldCategoryCode, entity.CategoryCode);
            sqlBuilder.SetValue(BaseModuleEntity.FieldImageUrl, entity.ImageUrl);
            sqlBuilder.SetValue(BaseModuleEntity.FieldImageIndex, entity.ImageIndex);
            sqlBuilder.SetValue(BaseModuleEntity.FieldSelectedImageIndex, entity.SelectedImageIndex);
            sqlBuilder.SetValue(BaseModuleEntity.FieldNavigateUrl, entity.NavigateUrl);
            sqlBuilder.SetValue(BaseModuleEntity.FieldTarget, entity.Target);
            sqlBuilder.SetValue(BaseModuleEntity.FieldFormName, entity.FormName);
            sqlBuilder.SetValue(BaseModuleEntity.FieldAssemblyName, entity.AssemblyName);
            sqlBuilder.SetValue(BaseModuleEntity.FieldPermissionScopeTables, entity.PermissionScopeTables);
            sqlBuilder.SetValue(BaseModuleEntity.FieldIsMenu, entity.IsMenu);
            sqlBuilder.SetValue(BaseModuleEntity.FieldIsPublic, entity.IsPublic);
            sqlBuilder.SetValue(BaseModuleEntity.FieldIsExpand, entity.IsExpand);
            sqlBuilder.SetValue(BaseModuleEntity.FieldIsScope, entity.IsScope);
            sqlBuilder.SetValue(BaseModuleEntity.FieldIsVisible, entity.IsVisible);
            sqlBuilder.SetValue(BaseModuleEntity.FieldAllowEdit, entity.AllowEdit);
            sqlBuilder.SetValue(BaseModuleEntity.FieldAllowDelete, entity.AllowDelete);
            sqlBuilder.SetValue(BaseModuleEntity.FieldLastCall, entity.LastCall);
            sqlBuilder.SetValue(BaseModuleEntity.FieldWebBrowser, entity.WebBrowser);
            sqlBuilder.SetValue(BaseModuleEntity.FieldAuthorizedDays, entity.AuthorizedDays);
            sqlBuilder.SetValue(BaseModuleEntity.FieldDescription, entity.Description);
            sqlBuilder.SetValue(BaseModuleEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseModuleEntity.FieldDeleted, entity.Deleted);
            sqlBuilder.SetValue(BaseModuleEntity.FieldEnabled, entity.Enabled);
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
