//-----------------------------------------------------------------------
// <copyright file="BaseDictionaryItemManager.Auto.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Business;
    using Util;

    /// <summary>
    /// BaseDictionaryItemManager
    /// 字典项
    /// 
    /// 修改记录
    /// 
    /// 2021-10-26 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2021-10-26</date>
    /// </author>
    /// </summary>
    public partial class BaseDictionaryItemManager : BaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseDictionaryItemManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.Create(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseDictionaryItemEntity.CurrentTableName;
                //按用户公司分表
                //CurrentTableName = BaseDictionaryItemEntity.CurrentTableName + GetTableSuffix();
            }
            CurrentTableDescription = typeof(BaseDictionaryItemEntity).FieldDescription("CurrentTableName");
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseDictionaryItemManager(string tableName) : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseDictionaryItemManager(IDbHelper dbHelper) : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseDictionaryItemManager(BaseUserInfo userInfo) : this()
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseDictionaryItemEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseDictionaryItemManager(BaseUserInfo userInfo, string tableName) : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseDictionaryItemManager(IDbHelper dbHelper, BaseUserInfo userInfo) : this(dbHelper)
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseDictionaryItemEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseDictionaryItemManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName) : this(dbHelper, userInfo)
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
        public string Add(BaseDictionaryItemEntity entity, bool identity = true, bool returnId = true)
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
        public string AddOrUpdate(BaseDictionaryItemEntity entity, bool identity = true, bool returnId = true)
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
        public int Update(BaseDictionaryItemEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseDictionaryItemEntity GetEntity(string id)
        {
            return ValidateUtil.IsInt(id) ? GetEntity(id.ToInt()) : null;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseDictionaryItemEntity GetEntity(int id)
        {
            return BaseEntity.Create<BaseDictionaryItemEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id)));
            //var cacheKey = CurrentTableName + ".Entity." + id;
            //var cacheTime = TimeSpan.FromMilliseconds(86400000);
            //return CacheUtil.Cache<BaseDictionaryItemEntity>(cacheKey, () => BaseEntity.Create<BaseDictionaryItemEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id))), true, false, cacheTime);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="parameters">参数</param>
        public BaseDictionaryItemEntity GetEntity(List<KeyValuePair<string, object>> parameters)
        {
            return BaseEntity.Create<BaseDictionaryItemEntity>(GetDataTable(parameters));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseDictionaryItemEntity entity)
        {
            var key = string.Empty;
            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, PrimaryKey);
            SetEntity(sqlBuilder, entity);
            SetEntityCreate(sqlBuilder, entity);
            SetEntityUpdate(sqlBuilder, entity);
            key = AddEntity(sqlBuilder, entity);
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
        public int UpdateEntity(BaseDictionaryItemEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            SetEntityUpdate(sqlBuilder, entity);
            var result = UpdateEntity(sqlBuilder, entity);
            if (result > 0)
            {
                RemoveCache(entity.Id);
            }
            return result;
        }

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">Sql语句生成器</param>
        /// <param name="entity">实体</param>
        public void SetEntity(SqlBuilder sqlBuilder, BaseDictionaryItemEntity entity)
        {
            sqlBuilder.SetValue(BaseDictionaryItemEntity.FieldDictionaryId, entity.DictionaryId);
            sqlBuilder.SetValue(BaseDictionaryItemEntity.FieldParentId, entity.ParentId);
            sqlBuilder.SetValue(BaseDictionaryItemEntity.FieldItemKey, entity.ItemKey);
            sqlBuilder.SetValue(BaseDictionaryItemEntity.FieldItemName, entity.ItemName);
            sqlBuilder.SetValue(BaseDictionaryItemEntity.FieldItemValue, entity.ItemValue);
            sqlBuilder.SetValue(BaseDictionaryItemEntity.FieldLanguage, entity.Language);
            sqlBuilder.SetValue(BaseDictionaryItemEntity.FieldAllowEdit, entity.AllowEdit);
            sqlBuilder.SetValue(BaseDictionaryItemEntity.FieldAllowDelete, entity.AllowDelete);
            sqlBuilder.SetValue(BaseDictionaryItemEntity.FieldDescription, entity.Description);
        }

    }
}
