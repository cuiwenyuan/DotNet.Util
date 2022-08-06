//-----------------------------------------------------------------------
// <copyright file="BaseOrganizationScopeManager.Auto.cs" company="DotNet">
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
    /// BaseOrganizationScopeManager
    /// 组织机构权限范围
    /// 
    /// 修改记录
    /// 
    /// 2021-09-27 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2021-09-27</date>
    /// </author>
    /// </summary>
    public partial class BaseOrganizationScopeManager : BaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseOrganizationScopeManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.Create(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseOrganizationScopeEntity.CurrentTableName;
                //按用户公司分表
                //CurrentTableName = BaseOrganizationScopeEntity.CurrentTableName + GetTableSuffix();
            }
            CurrentTableDescription = typeof(BaseOrganizationScopeEntity).FieldDescription("CurrentTableName");
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseOrganizationScopeManager(string tableName) : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseOrganizationScopeManager(IDbHelper dbHelper) : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseOrganizationScopeManager(BaseUserInfo userInfo) : this()
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseOrganizationScopeEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseOrganizationScopeManager(BaseUserInfo userInfo, string tableName) : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseOrganizationScopeManager(IDbHelper dbHelper, BaseUserInfo userInfo) : this(dbHelper)
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseOrganizationScopeEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseOrganizationScopeManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName) : this(dbHelper, userInfo)
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
        public string Add(BaseOrganizationScopeEntity entity, bool identity = true, bool returnId = true)
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
        public string AddOrUpdate(BaseOrganizationScopeEntity entity, bool identity = true, bool returnId = true)
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
        public int Update(BaseOrganizationScopeEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseOrganizationScopeEntity GetEntity(string id)
        {
            return ValidateUtil.IsInt(id) ? GetEntity(id.ToInt()) : null;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseOrganizationScopeEntity GetEntity(int id)
        {
            return BaseEntity.Create<BaseOrganizationScopeEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id)));
            //var cacheKey = CurrentTableName + ".Entity." + id;
            //var cacheTime = TimeSpan.FromMilliseconds(86400000);
            //return CacheUtil.Cache<BaseOrganizationScopeEntity>(cacheKey, () => BaseEntity.Create<BaseOrganizationScopeEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id))), true, false, cacheTime);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="parameters">参数</param>
        public BaseOrganizationScopeEntity GetEntity(List<KeyValuePair<string, object>> parameters)
        {
            return BaseEntity.Create<BaseOrganizationScopeEntity>(GetDataTable(parameters));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseOrganizationScopeEntity entity)
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
                sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldCreateUserId, UserInfo.UserId);
                sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldCreateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldCreateBy, UserInfo.RealName);
            }
            else
            {
                sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldCreateBy, entity.CreateBy);
                sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldCreateUserName, entity.CreateUserName);
            }
            sqlBuilder.SetDbNow(BaseOrganizationScopeEntity.FieldCreateTime);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldCreateIp, Utils.GetIp());
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUpdateUserId, UserInfo.UserId);
                sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseOrganizationScopeEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUpdateIp, Utils.GetIp());
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
        public int UpdateEntity(BaseOrganizationScopeEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUpdateUserId, UserInfo.UserId);
                sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseOrganizationScopeEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUpdateIp, Utils.GetIp());
            sqlBuilder.SetWhere(PrimaryKey, entity.Id);
            //return sqlBuilder.EndUpdate();
            var result = sqlBuilder.EndUpdate();
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
        private void SetEntity(SqlBuilder sqlBuilder, BaseOrganizationScopeEntity entity)
        {
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldResourceCategory, entity.ResourceCategory);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldResourceId, entity.ResourceId);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldPermissionId, entity.PermissionId);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldAllData, entity.AllData);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldProvince, entity.Province);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldCity, entity.City);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldDistrict, entity.District);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldStreet, entity.Street);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUserCompany, entity.UserCompany);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUserSubCompany, entity.UserSubCompany);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUserDepartment, entity.UserDepartment);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUserSubDepartment, entity.UserSubDepartment);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUserWorkgroup, entity.UserWorkgroup);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldOnlyOwnData, entity.OnlyOwnData);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldNotAllowed, entity.NotAllowed);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldByDetails, entity.ByDetails);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldContainChild, entity.ContainChild);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldDescription, entity.Description);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldDeleted, entity.Deleted);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldEnabled, entity.Enabled);
        }

    }
}
