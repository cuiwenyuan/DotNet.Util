﻿//-----------------------------------------------------------------------
// <copyright file="BaseLogonLogManager.Auto.cs" company="DotNet">
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
    /// BaseLogonLogManager
    /// 登录日志
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
    public partial class BaseLogonLogManager : BaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseLogonLogManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.Create(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseLogonLogEntity.CurrentTableName;
                //按用户公司分表
                //CurrentTableName = BaseLogonLogEntity.CurrentTableName + GetTableSuffix();
            }
            CurrentTableDescription = typeof(BaseLogonLogEntity).FieldDescription("CurrentTableName");
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseLogonLogManager(string tableName) : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseLogonLogManager(IDbHelper dbHelper) : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseLogonLogManager(BaseUserInfo userInfo) : this()
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseLogonLogEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseLogonLogManager(BaseUserInfo userInfo, string tableName) : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseLogonLogManager(IDbHelper dbHelper, BaseUserInfo userInfo) : this(dbHelper)
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseLogonLogEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseLogonLogManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName) : this(dbHelper, userInfo)
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
        public string Add(BaseLogonLogEntity entity, bool identity = true, bool returnId = true)
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
        public string AddOrUpdate(BaseLogonLogEntity entity, bool identity = true, bool returnId = true)
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
        public int Update(BaseLogonLogEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseLogonLogEntity GetEntity(string id)
        {
            return ValidateUtil.IsInt(id) ? GetEntity(id.ToInt()) : null;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseLogonLogEntity GetEntity(int id)
        {
            return BaseEntity.Create<BaseLogonLogEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id)));
            //var cacheKey = CurrentTableName + ".Entity." + id;
            //var cacheTime = TimeSpan.FromMilliseconds(86400000);
            //return CacheUtil.Cache<BaseLogonLogEntity>(cacheKey, () => BaseEntity.Create<BaseLogonLogEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id))), true, false, cacheTime);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="parameters">参数</param>
        public BaseLogonLogEntity GetEntity(List<KeyValuePair<string, object>> parameters)
        {
            return BaseEntity.Create<BaseLogonLogEntity>(GetDataTable(parameters));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseLogonLogEntity entity)
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
        public int UpdateEntity(BaseLogonLogEntity entity)
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
        private void SetEntity(SqlBuilder sqlBuilder, BaseLogonLogEntity entity)
        {
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldSystemCode, entity.SystemCode);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldSourceType, entity.SourceType);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldUserId, entity.UserId);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldUserName, entity.UserName);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldNickName, entity.NickName);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldRealName, entity.RealName);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldCompanyId, entity.CompanyId);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldCompanyName, entity.CompanyName);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldCompanyCode, entity.CompanyCode);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldProvince, entity.Province);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldCity, entity.City);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldService, entity.Service);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldElapsedTicks, entity.ElapsedTicks);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldTargetApplication, entity.TargetApplication);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldTargetIp, entity.TargetIp);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldResult, entity.Result);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldOperationType, entity.OperationType);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldLogonStatus, entity.LogonStatus);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldLogLevel, entity.LogLevel);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldIpAddress, entity.IpAddress);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldIpAddressName, entity.IpAddressName);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldMacAddress, entity.MacAddress);
        }

    }
}
