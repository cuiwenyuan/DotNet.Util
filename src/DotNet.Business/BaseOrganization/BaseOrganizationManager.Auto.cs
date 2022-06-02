//-----------------------------------------------------------------------
// <copyright file="BaseOrganizationManager.Auto.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseOrganizationManager
    /// 组织机构
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
    public partial class BaseOrganizationManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseOrganizationManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.Create(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseOrganizationEntity.CurrentTableName;
                //按用户公司分表
                //CurrentTableName = BaseOrganizationEntity.CurrentTableName + GetTableSuffix();
            }
            CurrentTableDescription = FieldExtensions.ToDescription(typeof(BaseOrganizationEntity), "CurrentTableName");
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseOrganizationManager(string tableName) : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseOrganizationManager(IDbHelper dbHelper) : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseOrganizationManager(BaseUserInfo userInfo) : this()
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseOrganizationEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseOrganizationManager(BaseUserInfo userInfo, string tableName) : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseOrganizationManager(IDbHelper dbHelper, BaseUserInfo userInfo) : this(dbHelper)
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseOrganizationEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseOrganizationManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName) : this(dbHelper, userInfo)
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
        public string Add(BaseOrganizationEntity entity, bool identity = true, bool returnId = true)
        {
            Identity = identity;
            ReturnId = returnId;
            entity.Id = int.Parse(AddEntity(entity));
            return entity.Id.ToString();
        }

        /// <summary>
        /// 添加或更新(主键是否为0)
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式，表主键是否采用自增的策略</param>
        /// <param name="returnId">返回主键，不返回程序允许速度会快，主要是为了主细表批量插入数据优化用的</param>
        /// <returns>主键</returns>
        public string AddOrUpdate(BaseOrganizationEntity entity, bool identity = true, bool returnId = true)
        {
            Identity = identity;
            ReturnId = returnId;
            if (entity.Id == 0)
            {
                entity.Id = int.Parse(AddEntity(entity));
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
        public int Update(BaseOrganizationEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseOrganizationEntity GetEntity(string id)
        {
            return ValidateUtil.IsInt(id) ? GetEntity(int.Parse(id)) : null;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseOrganizationEntity GetEntity(int id)
        {
            //return BaseEntity.Create<BaseOrganizationEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id)));
            var cacheKey = CurrentTableName + ".Entity." + id;
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            return CacheUtil.Cache<BaseOrganizationEntity>(cacheKey, () => BaseEntity.Create<BaseOrganizationEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id))), true, false, cacheTime);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="parameters">参数</param>
        public BaseOrganizationEntity GetEntity(List<KeyValuePair<string, object>> parameters)
        {
            return BaseEntity.Create<BaseOrganizationEntity>(GetDataTable(parameters));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseOrganizationEntity entity)
        {
            var key = string.Empty;
            if (entity.SortCode == 0)
            {
                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                key = managerSequence.Increment(CurrentTableName);
                entity.SortCode = int.Parse(key);
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
                        entity.Id = int.Parse(managerSequence.Increment(CurrentTableName));
                        sqlBuilder.SetValue(PrimaryKey, entity.Id);
                    }
                }
            }
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldCreateUserId, UserInfo.UserId);
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldCreateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldCreateBy, UserInfo.RealName);
            }
            else
            {
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldCreateBy, entity.CreateBy);
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldCreateUserName, entity.CreateUserName);
            }
            sqlBuilder.SetDbNow(BaseOrganizationEntity.FieldCreateTime);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCreateIp, Utils.GetIp());
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldUpdateUserId, UserInfo.UserId);
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseOrganizationEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldUpdateIp, Utils.GetIp());
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
        public int UpdateEntity(BaseOrganizationEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldUpdateUserId, UserInfo.UserId);
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseOrganizationEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldUpdateIp, Utils.GetIp());
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
        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseOrganizationEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">Sql语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseOrganizationEntity entity)
        {
            SetEntityExtend(sqlBuilder, entity);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldParentId, entity.ParentId);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldParentName, entity.ParentName);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCode, entity.Code);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldName, entity.Name);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldShortName, entity.ShortName);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldStandardName, entity.StandardName);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldStandardCode, entity.StandardCode);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldQuickQuery, entity.QuickQuery);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldSimpleSpelling, entity.SimpleSpelling);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCategoryCode, entity.CategoryCode);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldOuterPhone, entity.OuterPhone);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldInnerPhone, entity.InnerPhone);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldFax, entity.Fax);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldPostalCode, entity.PostalCode);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldProvince, entity.Province);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCity, entity.City);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldDistrict, entity.District);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCompanyId, entity.CompanyId);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCompanyCode, entity.CompanyCode);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCompanyName, entity.CompanyName);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldArea, entity.Area);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCostCenter, entity.CostCenter);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldFinancialCenter, entity.FinancialCenter);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldAddress, entity.Address);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldWeb, entity.Web);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldBank, entity.Bank);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldBankAccount, entity.BankAccount);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldLayer, entity.Layer);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldLongitude, entity.Longitude);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldLatitude, entity.Latitude);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldContainChildNodes, entity.ContainChildNodes);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldIsInnerOrganization, entity.IsInnerOrganization);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldProvinceId, entity.ProvinceId);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCityId, entity.CityId);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldDistrictId, entity.DistrictId);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldStreetId, entity.StreetId);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldStreet, entity.Street);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCostCenterId, entity.CostCenterId);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldFinancialCenterId, entity.FinancialCenterId);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldLeader, entity.Leader);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldLeaderMobile, entity.LeaderMobile);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldManager, entity.Manager);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldManagerMobile, entity.ManagerMobile);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldEmergencyCall, entity.EmergencyCall);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldBusinessPhone, entity.BusinessPhone);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldDescription, entity.Description);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldDeleted, entity.Deleted);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldEnabled, entity.Enabled);
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
