//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseOrganizationManager
    /// 组织机构、部门表
    ///
    /// 修改记录
    ///
    ///		2010-07-15 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2010-07-15</date>
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
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseOrganizationEntity.TableName;
            }
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseOrganizationManager(string tableName)
            : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseOrganizationManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseOrganizationManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseOrganizationManager(IDbHelper dbHelper, BaseUserInfo userInfo)
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
        public BaseOrganizationManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式</param>
        /// <param name="returnId">返回主鍵</param>
        /// <returns>主键</returns>
        public string Add(BaseOrganizationEntity entity, bool identity, bool returnId)
        {
            Identity = identity;
            ReturnId = returnId;
            return Add(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseOrganizationEntity GetEntity(int? id)
        {
            return GetEntity(id.ToString());
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseOrganizationEntity GetEntity(string id)
        {
            return BaseEntity.Create<BaseOrganizationEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseOrganizationEntity.FieldId, id)));
            // return BaseEntity.Create<BaseOrganizationEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseOrganizationEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseOrganizationEntity entity)
        {
            var result = string.Empty;
            if (entity.SortCode == 0)
            {
                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                result = managerSequence.Increment(CurrentTableName);
                entity.SortCode = int.Parse(result);
            }
            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BaseOrganizationEntity.FieldId);

            if (string.IsNullOrEmpty(entity.Id) && DbHelper.CurrentDbType == CurrentDbType.MySql)
            {
                entity.Id = Guid.NewGuid().ToString("N");
            }
            if (!string.IsNullOrEmpty(entity.Id) || !Identity)
            {
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldId, entity.Id);
            }
            else
            {
                if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                {
                    sqlBuilder.SetFormula(BaseOrganizationEntity.FieldId, "SEQ_" + CurrentTableName.ToUpper() + ".NEXTVAL ");
                }
                if (DbHelper.CurrentDbType == CurrentDbType.Db2)
                {
                    sqlBuilder.SetFormula(BaseOrganizationEntity.FieldId, "NEXT VALUE FOR SEQ_" + CurrentTableName.ToUpper());
                }
            }
            SetEntity(sqlBuilder, entity);

            // 创建人信息
            if (!string.IsNullOrEmpty(entity.CreateUserId))
            {
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldCreateUserId, entity.CreateUserId);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseOrganizationEntity.FieldCreateUserId, UserInfo.Id);
                }
            }
            if (!string.IsNullOrEmpty(entity.CreateBy))
            {
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldCreateBy, entity.CreateBy);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseOrganizationEntity.FieldCreateBy, UserInfo.RealName);
                }
            }
            if (entity.CreateOn.HasValue)
            {
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldCreateTime, entity.CreateOn);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseOrganizationEntity.FieldCreateTime);
            }

            // 修改人信息
            if (!string.IsNullOrEmpty(entity.ModifiedUserId))
            {
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldUpdateUserId, entity.ModifiedUserId);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseOrganizationEntity.FieldUpdateUserId, UserInfo.Id);
                }
            }
            if (!string.IsNullOrEmpty(entity.ModifiedBy))
            {
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldUpdateBy, entity.ModifiedBy);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseOrganizationEntity.FieldUpdateBy, UserInfo.RealName);
                }
            }
            if (entity.ModifiedOn.HasValue)
            {
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldUpdateTime, entity.ModifiedOn);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseOrganizationEntity.FieldUpdateTime);
            }

            if (Identity && (DbHelper.CurrentDbType == CurrentDbType.SqlServer || DbHelper.CurrentDbType == CurrentDbType.MySql))
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
        public int UpdateEntity(BaseOrganizationEntity entity)
        {
            var result = 0;

            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldUpdateBy, UserInfo.RealName);
            }
            // 若有修改时间标示，那就按修改时间来，不是按最新的时间来
            if (entity.ModifiedOn.HasValue)
            {
                sqlBuilder.SetValue(BaseOrganizationEntity.FieldUpdateTime, entity.ModifiedOn.Value);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseOrganizationEntity.FieldUpdateTime);
            }
            sqlBuilder.SetWhere(BaseOrganizationEntity.FieldId, entity.Id);
            result = sqlBuilder.EndUpdate();
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }

        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseOrganizationEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseOrganizationEntity entity)
        {
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldParentId, entity.ParentId);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldParentName, entity.ParentName);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldLayer, entity.Layer);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCode, entity.Code);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldShortName, entity.ShortName);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldFullName, entity.FullName);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldStandardCode, entity.StandardCode);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldStandardName, entity.StandardName);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldQuickQuery, entity.QuickQuery);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldSimpleSpelling, entity.SimpleSpelling);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCategoryCode, entity.CategoryCode);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldOuterPhone, entity.OuterPhone);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldInnerPhone, entity.InnerPhone);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldFax, entity.Fax);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldPostalcode, entity.Postalcode);

            sqlBuilder.SetValue(BaseOrganizationEntity.FieldProvinceId, entity.ProvinceId);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldProvince, entity.Province);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCityId, entity.CityId);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCity, entity.City);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldDistrictId, entity.DistrictId);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldDistrict, entity.District);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldStreetId, entity.StreetId);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldStreet, entity.Street);

            sqlBuilder.SetValue(BaseOrganizationEntity.FieldAddress, entity.Address);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldWeb, entity.Web);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldIsInnerOrganization, entity.IsInnerOrganization);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldBank, entity.Bank);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldBankAccount, entity.BankAccount);

            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCompanyId, entity.CompanyId);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCompanyCode, entity.CompanyCode);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCompanyName, entity.CompanyName);

            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCostCenter, entity.CostCenter);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldCostCenterId, entity.CostCenterId);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldFinancialCenter, entity.FinancialCenter);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldFinancialCenterId, entity.FinancialCenterId);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldManageId, entity.ManageId);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldManageName, entity.ManageName);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldArea, entity.Area);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldJoiningMethods, entity.JoiningMethods);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldIsCheckBalance, entity.IsCheckBalance);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldDeleted, entity.DeletionStateCode);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseOrganizationEntity.FieldDescription, entity.Description);

            SetEntityExtend(sqlBuilder, entity);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(int id)
        {
            var result = Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(BaseOrganizationEntity.FieldId, id) });
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
    }
}
