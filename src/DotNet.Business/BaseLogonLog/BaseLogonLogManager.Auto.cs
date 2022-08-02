//-----------------------------------------------------------------------
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
            if (entity.SortCode == 0)
            {
                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                if (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2)
                {
                    key = managerSequence.Increment($"SC_{CurrentTableName}_SEQ");
                }
                else
                {
                    key = managerSequence.Increment(CurrentTableName);
                }
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
                        sqlBuilder.SetFormula(PrimaryKey, $"{CurrentTableName}_SEQ.NEXTVAL");
                    }
                    if (DbHelper.CurrentDbType == CurrentDbType.Db2)
                    {
                        sqlBuilder.SetFormula(PrimaryKey, $"NEXT VALUE FOR {CurrentTableName}_SEQ");
                    }
                }
                else
                {
                    if (Identity && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                    {
                        var managerSequence = new BaseSequenceManager(DbHelper);
                        entity.Id = managerSequence.Increment($"{CurrentTableName}_SEQ").ToInt();
                        sqlBuilder.SetValue(PrimaryKey, entity.Id);
                    }
                }
            }
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseLogonLogEntity.FieldCreateUserId, UserInfo.UserId);
                sqlBuilder.SetValue(BaseLogonLogEntity.FieldCreateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseLogonLogEntity.FieldCreateBy, UserInfo.RealName);
            }
            else
            {
                sqlBuilder.SetValue(BaseLogonLogEntity.FieldCreateBy, entity.CreateBy);
                sqlBuilder.SetValue(BaseLogonLogEntity.FieldCreateUserName, entity.CreateUserName);
            }
            sqlBuilder.SetDbNow(BaseLogonLogEntity.FieldCreateTime);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldCreateIp, Utils.GetIp());
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseLogonLogEntity.FieldUpdateUserId, UserInfo.UserId);
                sqlBuilder.SetValue(BaseLogonLogEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseLogonLogEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseLogonLogEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldUpdateIp, Utils.GetIp());
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
        public int UpdateEntity(BaseLogonLogEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseLogonLogEntity.FieldUpdateUserId, UserInfo.UserId);
                sqlBuilder.SetValue(BaseLogonLogEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseLogonLogEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseLogonLogEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldUpdateIp, Utils.GetIp());
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
        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseLogonLogEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">Sql语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseLogonLogEntity entity)
        {
            SetEntityExtend(sqlBuilder, entity);
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
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldDeleted, entity.Deleted);
            sqlBuilder.SetValue(BaseLogonLogEntity.FieldEnabled, entity.Enabled);
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
