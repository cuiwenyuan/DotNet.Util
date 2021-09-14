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
    /// BaseOrganizeManager
    /// 组织机构、部门表
    ///
    /// 修改记录
    ///
    ///		2010-07-15 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2010-07-15</date>
    /// </author>
    /// </summary>
    public partial class BaseOrganizeManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseOrganizeManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseOrganizeEntity.TableName;
            }
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseOrganizeManager(string tableName)
            : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseOrganizeManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseOrganizeManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseOrganizeManager(IDbHelper dbHelper, BaseUserInfo userInfo)
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
        public BaseOrganizeManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
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
        public string Add(BaseOrganizeEntity entity, bool identity, bool returnId)
        {
            Identity = identity;
            ReturnId = returnId;
            return Add(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseOrganizeEntity GetObject(int? id)
        {
            return GetObject(id.ToString());
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseOrganizeEntity GetObject(string id)
        {
            return BaseEntity.Create<BaseOrganizeEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseOrganizeEntity.FieldId, id)));
            // return BaseEntity.Create<BaseOrganizeEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseOrganizeEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddObject(BaseOrganizeEntity entity)
        {
            var result = string.Empty;
            if (entity.SortCode == 0)
            {
                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                result = managerSequence.Increment(CurrentTableName);
                entity.SortCode = int.Parse(result);
            }
            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BaseOrganizeEntity.FieldId);

            if (string.IsNullOrEmpty(entity.Id) && DbHelper.CurrentDbType == CurrentDbType.MySql)
            {
                entity.Id = Guid.NewGuid().ToString("N");
            }
            if (!string.IsNullOrEmpty(entity.Id) || !Identity)
            {
                sqlBuilder.SetValue(BaseOrganizeEntity.FieldId, entity.Id);
            }
            else
            {
                if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                {
                    sqlBuilder.SetFormula(BaseOrganizeEntity.FieldId, "SEQ_" + CurrentTableName.ToUpper() + ".NEXTVAL ");
                }
                if (DbHelper.CurrentDbType == CurrentDbType.Db2)
                {
                    sqlBuilder.SetFormula(BaseOrganizeEntity.FieldId, "NEXT VALUE FOR SEQ_" + CurrentTableName.ToUpper());
                }
            }
            SetObject(sqlBuilder, entity);

            // 创建人信息
            if (!string.IsNullOrEmpty(entity.CreateUserId))
            {
                sqlBuilder.SetValue(BaseOrganizeEntity.FieldCreateUserId, entity.CreateUserId);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseOrganizeEntity.FieldCreateUserId, UserInfo.Id);
                }
            }
            if (!string.IsNullOrEmpty(entity.CreateBy))
            {
                sqlBuilder.SetValue(BaseOrganizeEntity.FieldCreateBy, entity.CreateBy);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseOrganizeEntity.FieldCreateBy, UserInfo.RealName);
                }
            }
            if (entity.CreateOn.HasValue)
            {
                sqlBuilder.SetValue(BaseOrganizeEntity.FieldCreateTime, entity.CreateOn);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseOrganizeEntity.FieldCreateTime);
            }

            // 修改人信息
            if (!string.IsNullOrEmpty(entity.ModifiedUserId))
            {
                sqlBuilder.SetValue(BaseOrganizeEntity.FieldUpdateUserId, entity.ModifiedUserId);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseOrganizeEntity.FieldUpdateUserId, UserInfo.Id);
                }
            }
            if (!string.IsNullOrEmpty(entity.ModifiedBy))
            {
                sqlBuilder.SetValue(BaseOrganizeEntity.FieldUpdateBy, entity.ModifiedBy);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseOrganizeEntity.FieldUpdateBy, UserInfo.RealName);
                }
            }
            if (entity.ModifiedOn.HasValue)
            {
                sqlBuilder.SetValue(BaseOrganizeEntity.FieldUpdateTime, entity.ModifiedOn);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseOrganizeEntity.FieldUpdateTime);
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
        public int UpdateObject(BaseOrganizeEntity entity)
        {
            var result = 0;

            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetObject(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseOrganizeEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseOrganizeEntity.FieldUpdateBy, UserInfo.RealName);
            }
            // 若有修改时间标示，那就按修改时间来，不是按最新的时间来
            if (entity.ModifiedOn.HasValue)
            {
                sqlBuilder.SetValue(BaseOrganizeEntity.FieldUpdateTime, entity.ModifiedOn.Value);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseOrganizeEntity.FieldUpdateTime);
            }
            sqlBuilder.SetWhere(BaseOrganizeEntity.FieldId, entity.Id);
            result = sqlBuilder.EndUpdate();
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }

        partial void SetObjectExpand(SqlBuilder sqlBuilder, BaseOrganizeEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetObject(SqlBuilder sqlBuilder, BaseOrganizeEntity entity)
        {
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldParentId, entity.ParentId);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldParentName, entity.ParentName);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldLayer, entity.Layer);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldCode, entity.Code);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldShortName, entity.ShortName);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldFullName, entity.FullName);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldStandardCode, entity.StandardCode);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldStandardName, entity.StandardName);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldQuickQuery, entity.QuickQuery);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldSimpleSpelling, entity.SimpleSpelling);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldCategoryCode, entity.CategoryCode);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldOuterPhone, entity.OuterPhone);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldInnerPhone, entity.InnerPhone);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldFax, entity.Fax);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldPostalcode, entity.Postalcode);

            sqlBuilder.SetValue(BaseOrganizeEntity.FieldProvinceId, entity.ProvinceId);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldProvince, entity.Province);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldCityId, entity.CityId);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldCity, entity.City);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldDistrictId, entity.DistrictId);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldDistrict, entity.District);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldStreetId, entity.StreetId);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldStreet, entity.Street);

            sqlBuilder.SetValue(BaseOrganizeEntity.FieldAddress, entity.Address);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldWeb, entity.Web);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldIsInnerOrganize, entity.IsInnerOrganize);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldBank, entity.Bank);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldBankAccount, entity.BankAccount);

            sqlBuilder.SetValue(BaseOrganizeEntity.FieldCompanyId, entity.CompanyId);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldCompanyCode, entity.CompanyCode);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldCompanyName, entity.CompanyName);

            sqlBuilder.SetValue(BaseOrganizeEntity.FieldCostCenter, entity.CostCenter);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldCostCenterId, entity.CostCenterId);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldFinancialCenter, entity.FinancialCenter);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldFinancialCenterId, entity.FinancialCenterId);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldManageId, entity.ManageId);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldManageName, entity.ManageName);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldArea, entity.Area);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldJoiningMethods, entity.JoiningMethods);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldIsCheckBalance, entity.IsCheckBalance);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldDeleted, entity.DeletionStateCode);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseOrganizeEntity.FieldDescription, entity.Description);

            SetObjectExpand(sqlBuilder, entity);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(int id)
        {
            var result = Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(BaseOrganizeEntity.FieldId, id) });
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
    }
}
