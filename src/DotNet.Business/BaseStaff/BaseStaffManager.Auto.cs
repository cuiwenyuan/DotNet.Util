//-----------------------------------------------------------------------
// <copyright file="BaseStaffManager.Auto.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseStaffManager
    /// 员工
    /// 
    /// 修改记录
    /// 
    /// 2021-09-28 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2021-09-28</date>
    /// </author>
    /// </summary>
    public partial class BaseStaffManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseStaffManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.Create(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseStaffEntity.CurrentTableName;
                //按用户公司分表
                //CurrentTableName = BaseStaffEntity.CurrentTableName + GetTableSuffix();
            }
            CurrentTableDescription = FieldExtensions.ToDescription(typeof(BaseStaffEntity), "CurrentTableName");
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseStaffManager(string tableName) : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseStaffManager(IDbHelper dbHelper) : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseStaffManager(BaseUserInfo userInfo) : this()
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseStaffEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseStaffManager(BaseUserInfo userInfo, string tableName) : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseStaffManager(IDbHelper dbHelper, BaseUserInfo userInfo) : this(dbHelper)
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseStaffEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseStaffManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName) : this(dbHelper, userInfo)
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
        public string Add(BaseStaffEntity entity, bool identity = true, bool returnId = true)
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
        public string AddOrUpdate(BaseStaffEntity entity, bool identity = true, bool returnId = true)
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
        public int Update(BaseStaffEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseStaffEntity GetEntity(string id)
        {
            return ValidateUtil.IsInt(id) ? GetEntity(int.Parse(id)) : null;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseStaffEntity GetEntity(int id)
        {
            return BaseEntity.Create<BaseStaffEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id)));
            //var cacheKey = CurrentTableName + ".Entity." + id;
            //var cacheTime = TimeSpan.FromMilliseconds(86400000);
            //return CacheUtil.Cache<BaseStaffEntity>(cacheKey, () => BaseEntity.Create<BaseStaffEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id))), true, false, cacheTime);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="parameters">参数</param>
        public BaseStaffEntity GetEntity(List<KeyValuePair<string, object>> parameters)
        {
            return BaseEntity.Create<BaseStaffEntity>(GetDataTable(parameters));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseStaffEntity entity)
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
                        sqlBuilder.SetFormula(PrimaryKey, CurrentTableName.ToUpper() + "_SEQ.NEXTVAL ");
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
                sqlBuilder.SetValue(BaseStaffEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseStaffEntity.FieldCreateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseStaffEntity.FieldCreateBy, UserInfo.RealName);
            }
            else
            {
                sqlBuilder.SetValue(BaseStaffEntity.FieldCreateBy, entity.CreateBy);
                sqlBuilder.SetValue(BaseStaffEntity.FieldCreateUserName, entity.CreateUserName);
            }
            sqlBuilder.SetDbNow(BaseStaffEntity.FieldCreateTime);
            sqlBuilder.SetValue(BaseStaffEntity.FieldCreateIp, Utils.GetIp());
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseStaffEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseStaffEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseStaffEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseStaffEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BaseStaffEntity.FieldUpdateIp, Utils.GetIp());
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
        public int UpdateEntity(BaseStaffEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseStaffEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseStaffEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseStaffEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseStaffEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BaseStaffEntity.FieldUpdateIp, Utils.GetIp());
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
        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseStaffEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">Sql语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseStaffEntity entity)
        {
            SetEntityExtend(sqlBuilder, entity);
            sqlBuilder.SetValue(BaseStaffEntity.FieldUserId, entity.UserId);
            sqlBuilder.SetValue(BaseStaffEntity.FieldUserName, entity.UserName);
            sqlBuilder.SetValue(BaseStaffEntity.FieldEmployeeNumber, entity.EmployeeNumber);
            sqlBuilder.SetValue(BaseStaffEntity.FieldRealName, entity.RealName);
            sqlBuilder.SetValue(BaseStaffEntity.FieldChineseName, entity.ChineseName);
            sqlBuilder.SetValue(BaseStaffEntity.FieldEnglishName, entity.EnglishName);
            sqlBuilder.SetValue(BaseStaffEntity.FieldQuickQuery, entity.QuickQuery);
            sqlBuilder.SetValue(BaseStaffEntity.FieldCompanyId, entity.CompanyId);
            sqlBuilder.SetValue(BaseStaffEntity.FieldCompanyName, entity.CompanyName);
            sqlBuilder.SetValue(BaseStaffEntity.FieldSubCompanyId, entity.SubCompanyId);
            sqlBuilder.SetValue(BaseStaffEntity.FieldSubCompanyName, entity.SubCompanyName);
            sqlBuilder.SetValue(BaseStaffEntity.FieldDepartmentId, entity.DepartmentId);
            sqlBuilder.SetValue(BaseStaffEntity.FieldDepartmentName, entity.DepartmentName);
            sqlBuilder.SetValue(BaseStaffEntity.FieldWorkgroupId, entity.WorkgroupId);
            sqlBuilder.SetValue(BaseStaffEntity.FieldWorkgroupName, entity.WorkgroupName);
            sqlBuilder.SetValue(BaseStaffEntity.FieldDutyId, entity.DutyId);
            sqlBuilder.SetValue(BaseStaffEntity.FieldGender, entity.Gender);
            sqlBuilder.SetValue(BaseStaffEntity.FieldBirthday, entity.Birthday);
            sqlBuilder.SetValue(BaseStaffEntity.FieldAge, entity.Age);
            sqlBuilder.SetValue(BaseStaffEntity.FieldHeight, entity.Height);
            sqlBuilder.SetValue(BaseStaffEntity.FieldWeight, entity.Weight);
            sqlBuilder.SetValue(BaseStaffEntity.FieldIdentificationCode, entity.IdentificationCode);
            sqlBuilder.SetValue(BaseStaffEntity.FieldIdCard, entity.IdCard);
            sqlBuilder.SetValue(BaseStaffEntity.FieldNation, entity.Nation);
            sqlBuilder.SetValue(BaseStaffEntity.FieldEducation, entity.Education);
            sqlBuilder.SetValue(BaseStaffEntity.FieldSchool, entity.School);
            sqlBuilder.SetValue(BaseStaffEntity.FieldGraduationDate, entity.GraduationDate);
            sqlBuilder.SetValue(BaseStaffEntity.FieldMajor, entity.Major);
            sqlBuilder.SetValue(BaseStaffEntity.FieldDegree, entity.Degree);
            sqlBuilder.SetValue(BaseStaffEntity.FieldTitleId, entity.TitleId);
            sqlBuilder.SetValue(BaseStaffEntity.FieldTitleDate, entity.TitleDate);
            sqlBuilder.SetValue(BaseStaffEntity.FieldTitleLevel, entity.TitleLevel);
            sqlBuilder.SetValue(BaseStaffEntity.FieldWorkingDate, entity.WorkingDate);
            sqlBuilder.SetValue(BaseStaffEntity.FieldJoinInDate, entity.JoinInDate);
            sqlBuilder.SetValue(BaseStaffEntity.FieldOfficePostCode, entity.OfficePostCode);
            sqlBuilder.SetValue(BaseStaffEntity.FieldOfficeAddress, entity.OfficeAddress);
            sqlBuilder.SetValue(BaseStaffEntity.FieldOfficePhone, entity.OfficePhone);
            sqlBuilder.SetValue(BaseStaffEntity.FieldOfficeFax, entity.OfficeFax);
            sqlBuilder.SetValue(BaseStaffEntity.FieldHomePostCode, entity.HomePostCode);
            sqlBuilder.SetValue(BaseStaffEntity.FieldHomeAddress, entity.HomeAddress);
            sqlBuilder.SetValue(BaseStaffEntity.FieldHomePhone, entity.HomePhone);
            sqlBuilder.SetValue(BaseStaffEntity.FieldHomeFax, entity.HomeFax);
            sqlBuilder.SetValue(BaseStaffEntity.FieldPlateNumber1, entity.PlateNumber1);
            sqlBuilder.SetValue(BaseStaffEntity.FieldPlateNumber2, entity.PlateNumber2);
            sqlBuilder.SetValue(BaseStaffEntity.FieldPlateNumber3, entity.PlateNumber3);
            sqlBuilder.SetValue(BaseStaffEntity.FieldRewardCard, entity.RewardCard);
            sqlBuilder.SetValue(BaseStaffEntity.FieldMedicalCard, entity.MedicalCard);
            sqlBuilder.SetValue(BaseStaffEntity.FieldUnionMember, entity.UnionMember);
            sqlBuilder.SetValue(BaseStaffEntity.FieldEmail, entity.Email);
            sqlBuilder.SetValue(BaseStaffEntity.FieldMobile, entity.Mobile);
            sqlBuilder.SetValue(BaseStaffEntity.FieldQq, entity.Qq);
            sqlBuilder.SetValue(BaseStaffEntity.FieldWeChat, entity.WeChat);
            sqlBuilder.SetValue(BaseStaffEntity.FieldShortNumber, entity.ShortNumber);
            sqlBuilder.SetValue(BaseStaffEntity.FieldTelephone, entity.Telephone);
            sqlBuilder.SetValue(BaseStaffEntity.FieldExtension, entity.Extension);
            sqlBuilder.SetValue(BaseStaffEntity.FieldEmergencyContact, entity.EmergencyContact);
            sqlBuilder.SetValue(BaseStaffEntity.FieldEmergencyMobile, entity.EmergencyMobile);
            sqlBuilder.SetValue(BaseStaffEntity.FieldEmergencyTelephone, entity.EmergencyTelephone);
            sqlBuilder.SetValue(BaseStaffEntity.FieldNativePlace, entity.NativePlace);
            sqlBuilder.SetValue(BaseStaffEntity.FieldBankName, entity.BankName);
            sqlBuilder.SetValue(BaseStaffEntity.FieldBankAccount, entity.BankAccount);
            sqlBuilder.SetValue(BaseStaffEntity.FieldBankUserName, entity.BankUserName);
            sqlBuilder.SetValue(BaseStaffEntity.FieldProvince, entity.Province);
            sqlBuilder.SetValue(BaseStaffEntity.FieldCity, entity.City);
            sqlBuilder.SetValue(BaseStaffEntity.FieldDistrict, entity.District);
            sqlBuilder.SetValue(BaseStaffEntity.FieldCurrentProvince, entity.CurrentProvince);
            sqlBuilder.SetValue(BaseStaffEntity.FieldCurrentCity, entity.CurrentCity);
            sqlBuilder.SetValue(BaseStaffEntity.FieldCurrentDistrict, entity.CurrentDistrict);
            sqlBuilder.SetValue(BaseStaffEntity.FieldParty, entity.Party);
            sqlBuilder.SetValue(BaseStaffEntity.FieldNationality, entity.Nationality);
            sqlBuilder.SetValue(BaseStaffEntity.FieldWorkingProperty, entity.WorkingProperty);
            sqlBuilder.SetValue(BaseStaffEntity.FieldCompetency, entity.Competency);
            sqlBuilder.SetValue(BaseStaffEntity.FieldMarriage, entity.Marriage);
            sqlBuilder.SetValue(BaseStaffEntity.FieldWeddingDate, entity.WeddingDate);
            sqlBuilder.SetValue(BaseStaffEntity.FieldDivorceDate, entity.DivorceDate);
            sqlBuilder.SetValue(BaseStaffEntity.FieldChild1Birthday, entity.Child1Birthday);
            sqlBuilder.SetValue(BaseStaffEntity.FieldChild2Birthday, entity.Child2Birthday);
            sqlBuilder.SetValue(BaseStaffEntity.FieldChild3Birthday, entity.Child3Birthday);
            sqlBuilder.SetValue(BaseStaffEntity.FieldChild4Birthday, entity.Child4Birthday);
            sqlBuilder.SetValue(BaseStaffEntity.FieldChild5Birthday, entity.Child5Birthday);
            sqlBuilder.SetValue(BaseStaffEntity.FieldIsDimission, entity.IsDimission);
            sqlBuilder.SetValue(BaseStaffEntity.FieldDimissionDate, entity.DimissionDate);
            sqlBuilder.SetValue(BaseStaffEntity.FieldDimissionCause, entity.DimissionCause);
            sqlBuilder.SetValue(BaseStaffEntity.FieldDimissionWhereabouts, entity.DimissionWhereabouts);
            sqlBuilder.SetValue(BaseStaffEntity.FieldExt1, entity.Ext1);
            sqlBuilder.SetValue(BaseStaffEntity.FieldExt2, entity.Ext2);
            sqlBuilder.SetValue(BaseStaffEntity.FieldExt3, entity.Ext3);
            sqlBuilder.SetValue(BaseStaffEntity.FieldExt4, entity.Ext4);
            sqlBuilder.SetValue(BaseStaffEntity.FieldExt5, entity.Ext5);
            sqlBuilder.SetValue(BaseStaffEntity.FieldDescription, entity.Description);
            sqlBuilder.SetValue(BaseStaffEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseStaffEntity.FieldDeleted, entity.Deleted);
            sqlBuilder.SetValue(BaseStaffEntity.FieldEnabled, entity.Enabled);
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
