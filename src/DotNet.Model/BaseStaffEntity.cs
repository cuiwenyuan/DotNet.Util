//--------------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//--------------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseStaffEntity
    /// 员工（职员）表
    /// 
    /// 修改记录
    /// 
    /// 2012-07-07 版本：1.0 JiRiGaLa 创建主键。
    /// 
    /// <author>
    ///     <name>JiRiGaLa</name>
    ///     <date>2012-07-07</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseStaffEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int? Id { get; set; } = null;

        /// <summary>
        /// 用户主键
        /// </summary>
        public int? UserId { get; set; } = null;

        /// <summary>
        /// 用户名(登录用)
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; } = string.Empty;

        /// <summary>
        /// 编号,工号
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; } = string.Empty;

        /// <summary>
        /// 公司主键，数据库中可以设置为int
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// 分支机构主键，数据库中可以设置为int
        /// </summary>
        public string SubCompanyId { get; set; } = string.Empty;

        /// <summary>
        /// 部门主键，数据库中可以设置为int
        /// </summary>
        public string DepartmentId { get; set; } = string.Empty;

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// 工作组主键，数据库中可以设置为int
        /// </summary>
        public string WorkgroupId { get; set; } = string.Empty;

        /// <summary>
        /// 工作名称
        /// </summary>
        public string WorkgroupName { get; set; } = string.Empty;

        /// <summary>
        /// 快速查找，记忆符
        /// </summary>
        public string QuickQuery { get; set; } = string.Empty;

        /// <summary>
        /// 职位主键
        /// </summary>
        public string DutyId { get; set; } = string.Empty;

        /// <summary>
        /// 唯一身份Id
        /// </summary>
        public string IdentificationCode { get; set; } = string.Empty;

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdCard { get; set; } = string.Empty;

        /// <summary>
        /// 银行卡号
        /// </summary>
        public string BankCode { get; set; } = string.Empty;

        /// <summary>
        /// 开户行
        /// </summary>
        public string BankName { get; set; } = null;

        /// <summary>
        /// 开户行姓名
        /// </summary>
        public string BankUserName { get; set; } = null;

        /// <summary>
        /// 奖金卡号
        /// </summary>
        public string RewardCard { get; set; } = string.Empty;

        /// <summary>
        /// 医疗卡号
        /// </summary>
        public string MedicalCard { get; set; } = string.Empty;

        /// <summary>
        /// 工会证号
        /// </summary>
        public string UnionMember { get; set; } = string.Empty;

        /// <summary>
        /// 电子邮件
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; } = string.Empty;

        /// <summary>
        /// 短号
        /// </summary>
        public string ShortNumber { get; set; } = string.Empty;
        /// <summary>
        /// 电话
        /// </summary>

        public string Telephone { get; set; } = string.Empty;
        /// <summary>
        /// QQ号码
        /// </summary>

        public string Qq { get; set; } = string.Empty;
        /// <summary>
        /// 办公电话
        /// </summary>

        public string OfficePhone { get; set; } = string.Empty;
        /// <summary>
        /// 分机号码
        /// </summary>

        public string Extension { get; set; } = string.Empty;
        /// <summary>
        /// 办公邮编
        /// </summary>

        public string OfficePostCode { get; set; } = string.Empty;
        /// <summary>
        /// 办公地址
        /// </summary>

        public string OfficeAddress { get; set; } = string.Empty;
        /// <summary>
        /// 办公传真
        /// </summary>

        public string OfficeFax { get; set; } = string.Empty;

        /// <summary>
        /// 年龄
        /// </summary>
        public string Age { get; set; } = string.Empty;

        /// <summary>
        /// 出生日期
        /// </summary>
        public string Birthday { get; set; } = string.Empty;

        /// <summary>
        /// 身高
        /// </summary>
        public string Height { get; set; } = string.Empty;

        /// <summary>
        /// 体重
        /// </summary>
        public string Weight { get; set; } = string.Empty;

        /// <summary>
        /// 婚姻情况
        /// </summary>
        public string Marriage { get; set; } = string.Empty;

        /// <summary>
        /// 最高学历
        /// </summary>
        public string Education { get; set; } = string.Empty;

        /// <summary>
        /// 毕业院校
        /// </summary>
        public string School { get; set; } = string.Empty;

        /// <summary>
        /// 毕业时间
        /// </summary>
        public string GraduationDate { get; set; } = string.Empty;

        /// <summary>
        /// 专业
        /// </summary>
        public string Major { get; set; } = string.Empty;

        /// <summary>
        /// 最高学位
        /// </summary>
        public string Degree { get; set; } = string.Empty;

        /// <summary>
        /// 职称主键
        /// </summary>
        public string TitleId { get; set; } = string.Empty;

        /// <summary>
        /// 职称评定日期
        /// </summary>
        public string TitleDate { get; set; } = string.Empty;

        /// <summary>
        /// 职称等级
        /// </summary>
        public string TitleLevel { get; set; } = string.Empty;

        /// <summary>
        /// 工作时间
        /// </summary>
        public string WorkingDate { get; set; } = string.Empty;

        /// <summary>
        /// 加入本单位时间
        /// </summary>
        public string JoinInDate { get; set; } = string.Empty;

        /// <summary>
        /// 家庭住址邮编
        /// </summary>
        public string HomePostCode { get; set; } = string.Empty;

        /// <summary>
        /// 家庭住址
        /// </summary>
        public string HomeAddress { get; set; } = string.Empty;

        /// <summary>
        /// 住宅电话
        /// </summary>
        public string HomePhone { get; set; } = string.Empty;

        /// <summary>
        /// 家庭传真
        /// </summary>
        public string HomeFax { get; set; } = string.Empty;

        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarCode { get; set; } = string.Empty;

        /// <summary>
        /// 籍贯省
        /// </summary>
        public string Province { get; set; } = string.Empty;

        /// <summary>
        /// 籍贯市
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// 籍贯区
        /// </summary>
        public string District { get; set; } = string.Empty;

        /// <summary>
        /// 当前省
        /// </summary>
        public string CurrentProvince { get; set; } = string.Empty;

        /// <summary>
        /// 当前市
        /// </summary>
        public string CurrentCity { get; set; } = string.Empty;

        /// <summary>
        /// 当前区
        /// </summary>
        public string CurrentDistrict { get; set; } = string.Empty;

        /// <summary>
        /// 籍贯
        /// </summary>
        public string NativePlace { get; set; } = string.Empty;

        /// <summary>
        /// 政治面貌
        /// </summary>
        public string Party { get; set; } = string.Empty;

        /// <summary>
        /// 国籍
        /// </summary>
        public string Nation { get; set; } = string.Empty;

        /// <summary>
        /// 民族
        /// </summary>
        public string Nationality { get; set; } = string.Empty;

        /// <summary>
        /// 工作性质
        /// </summary>
        public string WorkingProperty { get; set; } = string.Empty;

        /// <summary>
        /// 职业资格
        /// </summary>
        public string Competency { get; set; } = string.Empty;

        /// <summary>
        /// 紧急联系
        /// </summary>
        public string EmergencyContact { get; set; } = string.Empty;

        /// <summary>
        /// 结婚日期
        /// </summary>
        public DateTime? WeddingDay { get; set; } = null;
        /// <summary>
        /// 离婚日期
        /// </summary>

        public DateTime? Divorce { get; set; } = null;

        /// <summary>
        /// 第一个孩子出生时间
        /// </summary>
        public DateTime? ChildBirthday1 { get; set; } = null;

        /// <summary>
        /// 第二个孩子出生时间
        /// </summary>
        public DateTime? ChildBirthday2 { get; set; } = null;

        /// <summary>
        /// 离职
        /// </summary>
        public int? IsDimission { get; set; } = 0;

        /// <summary>
        /// 离职日期
        /// </summary>
        public string DimissionDate { get; set; } = string.Empty;

        /// <summary>
        /// 离职原因
        /// </summary>
        public string DimissionCause { get; set; } = string.Empty;

        /// <summary>
        /// 离职去向
        /// </summary>
        public string DimissionWhither { get; set; } = string.Empty;

        /// <summary>
        /// 有效
        /// </summary>
        public int? Enabled { get; set; } = 1;

        /// <summary>
        /// 是否删除
        /// </summary>
        public int? DeletionStateCode { get; set; } = 0;

        /// <summary>
        /// 排序码
        /// </summary>
        public int? SortCode { get; set; } = 0;

        /// <summary>
        /// 备注
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人用户编号
        /// </summary>
        public string CreateUserId { get; set; } = string.Empty;

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; } = string.Empty;

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改人用户编号
        /// </summary>
        public string ModifiedUserId { get; set; } = string.Empty;

        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifiedBy { get; set; } = string.Empty;

        /// <summary>
        /// 岗位名称
        /// </summary>
        public string Duty { get; set; } = string.Empty;

        /// <summary>
        /// 岗位编号
        /// </summary>
        public string DutyCode { get; set; } = string.Empty;

        /// <summary>
        /// 岗位类型
        /// </summary>
        public string DutyType { get; set; } = string.Empty;

        /// <summary>
        /// 岗位等级
        /// </summary>
        public string DutyLevel { get; set; } = string.Empty;


        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
		protected override BaseEntity GetFrom(IDataRow dr)
        {
            Id = BaseUtil.ConvertToInt(dr[FieldId]);
            UserId = BaseUtil.ConvertToInt(dr[FieldUserId]);
            UserName = BaseUtil.ConvertToString(dr[FieldUserName]);
            RealName = BaseUtil.ConvertToString(dr[FieldRealName]);
            Code = BaseUtil.ConvertToString(dr[FieldCode]);
            Gender = BaseUtil.ConvertToString(dr[FieldGender]);
            SubCompanyId = BaseUtil.ConvertToString(dr[FieldSubCompanyId]);
            CompanyId = BaseUtil.ConvertToString(dr[FieldCompanyId]);
            CompanyName = BaseUtil.ConvertToString(dr[FieldCompanyName]);
            DepartmentId = BaseUtil.ConvertToString(dr[FieldDepartmentId]);
            DepartmentName = BaseUtil.ConvertToString(dr[FieldDepartmentName]);
            WorkgroupId = BaseUtil.ConvertToString(dr[FieldWorkgroupId]);
            WorkgroupName = BaseUtil.ConvertToString(dr[FieldWorkgroupName]);
            QuickQuery = BaseUtil.ConvertToString(dr[FieldQuickQuery]);
            DutyId = BaseUtil.ConvertToString(dr[FieldDutyId]);
            IdentificationCode = BaseUtil.ConvertToString(dr[FieldIdentificationCode]);
            IdCard = BaseUtil.ConvertToString(dr[FieldIdCard]);
            BankCode = BaseUtil.ConvertToString(dr[FieldBankCode]);
            BankName = BaseUtil.ConvertToString(dr[FieldBankName]);
            RewardCard = BaseUtil.ConvertToString(dr[FieldRewardCard]);
            MedicalCard = BaseUtil.ConvertToString(dr[FieldMedicalCard]);
            UnionMember = BaseUtil.ConvertToString(dr[FieldUnionMember]);
            Email = BaseUtil.ConvertToString(dr[FieldEmail]);
            Mobile = BaseUtil.ConvertToString(dr[FieldMobile]);
            ShortNumber = BaseUtil.ConvertToString(dr[FieldShortNumber]);
            Telephone = BaseUtil.ConvertToString(dr[FieldTelephone]);
            Qq = BaseUtil.ConvertToString(dr[FieldQq]);
            OfficePhone = BaseUtil.ConvertToString(dr[FieldOfficePhone]);
            Extension = BaseUtil.ConvertToString(dr[FieldExtension]);
            OfficePostCode = BaseUtil.ConvertToString(dr[FieldOfficePostCode]);
            OfficeAddress = BaseUtil.ConvertToString(dr[FieldOfficeAddress]);
            OfficeFax = BaseUtil.ConvertToString(dr[FieldOfficeFax]);
            Age = BaseUtil.ConvertToString(dr[FieldAge]);
            Birthday = BaseUtil.ConvertToString(dr[FieldBirthday]);
            Height = BaseUtil.ConvertToString(dr[FieldHeight]);
            Weight = BaseUtil.ConvertToString(dr[FieldWeight]);
            Marriage = BaseUtil.ConvertToString(dr[FieldMarriage]);
            Education = BaseUtil.ConvertToString(dr[FieldEducation]);
            School = BaseUtil.ConvertToString(dr[FieldSchool]);
            GraduationDate = BaseUtil.ConvertToString(dr[FieldGraduationDate]);
            Major = BaseUtil.ConvertToString(dr[FieldMajor]);
            Degree = BaseUtil.ConvertToString(dr[FieldDegree]);
            TitleId = BaseUtil.ConvertToString(dr[FieldTitleId]);
            TitleDate = BaseUtil.ConvertToString(dr[FieldTitleDate]);
            TitleLevel = BaseUtil.ConvertToString(dr[FieldTitleLevel]);
            WorkingDate = BaseUtil.ConvertToString(dr[FieldWorkingDate]);
            JoinInDate = BaseUtil.ConvertToString(dr[FieldJoinInDate]);
            HomePostCode = BaseUtil.ConvertToString(dr[FieldHomePostCode]);
            HomeAddress = BaseUtil.ConvertToString(dr[FieldHomeAddress]);
            HomePhone = BaseUtil.ConvertToString(dr[FieldHomePhone]);
            HomeFax = BaseUtil.ConvertToString(dr[FieldHomeFax]);
            CarCode = BaseUtil.ConvertToString(dr[FieldCarCode]);
            Province = BaseUtil.ConvertToString(dr[FieldProvince]);
            City = BaseUtil.ConvertToString(dr[FieldCity]);
            District = BaseUtil.ConvertToString(dr[FieldDistrict]);
            NativePlace = BaseUtil.ConvertToString(dr[FieldNativePlace]);
            CurrentProvince = BaseUtil.ConvertToString(dr[FieldCurrentProvince]);
            CurrentCity = BaseUtil.ConvertToString(dr[FieldCurrentCity]);
            CurrentDistrict = BaseUtil.ConvertToString(dr[FieldCurrentDistrict]);
            Party = BaseUtil.ConvertToString(dr[FieldParty]);
            Nation = BaseUtil.ConvertToString(dr[FieldNation]);
            Nationality = BaseUtil.ConvertToString(dr[FieldNationality]);
            WorkingProperty = BaseUtil.ConvertToString(dr[FieldWorkingProperty]);
            Competency = BaseUtil.ConvertToString(dr[FieldCompetency]);
            EmergencyContact = BaseUtil.ConvertToString(dr[FieldEmergencyContact]);
            WeddingDay = BaseUtil.ConvertToNullableDateTime(dr[FieldWeddingDay]);
            Divorce = BaseUtil.ConvertToNullableDateTime(dr[FieldDivorce]);
            ChildBirthday1 = BaseUtil.ConvertToNullableDateTime(dr[FieldChildBirthday1]);
            ChildBirthday2 = BaseUtil.ConvertToNullableDateTime(dr[FieldChildBirthday2]);
            IsDimission = BaseUtil.ConvertToInt(dr[FieldIsDimission]);
            DimissionDate = BaseUtil.ConvertToString(dr[FieldDimissionDate]);
            DimissionCause = BaseUtil.ConvertToString(dr[FieldDimissionCause]);
            DimissionWhither = BaseUtil.ConvertToString(dr[FieldDimissionWhither]);
            Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            DeletionStateCode = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            SortCode = BaseUtil.ConvertToInt(dr[FieldSortCode]);
            Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            CreateOn = BaseUtil.ConvertToNullableDateTime(dr[FieldCreateTime]);
            CreateUserId = BaseUtil.ConvertToString(dr[FieldCreateUserId]);
            CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            ModifiedOn = BaseUtil.ConvertToNullableDateTime(dr[FieldUpdateTime]);
            ModifiedUserId = BaseUtil.ConvertToString(dr[FieldUpdateUserId]);
            ModifiedBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);

            Duty = BaseUtil.ConvertToString(dr[FieldDuty]);
            DutyCode = BaseUtil.ConvertToString(dr[FieldDutyCode]);
            DutyType = BaseUtil.ConvertToString(dr[FieldDutyType]);
            DutyLevel = BaseUtil.ConvertToString(dr[FieldDutyLevel]);
            // 获取扩展属性
            GetFromExpand(dr);
            return this;
        }

        ///<summary>
        /// 员工（职员）表
        ///</summary>
        [NonSerialized]
        public const string TableName = "BaseStaff";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 用户主键
        ///</summary>
        [NonSerialized]
        public const string FieldUserId = "UserId";

        ///<summary>
        /// 用户名(登录用)
        ///</summary>
        [NonSerialized]
        public const string FieldUserName = "UserName";

        ///<summary>
        /// 姓名
        ///</summary>
        [NonSerialized]
        public const string FieldRealName = "RealName";

        ///<summary>
        /// 编号,工号
        ///</summary>
        [NonSerialized]
        public const string FieldCode = "Code";

        ///<summary>
        /// 性别
        ///</summary>
        [NonSerialized]
        public const string FieldGender = "Gender";

        ///<summary>
        /// 分支机构主键，数据库中可以设置为int
        ///</summary>
        [NonSerialized]
        public const string FieldSubCompanyId = "SubCompanyId";

        ///<summary>
        /// 公司主键，数据库中可以设置为int
        ///</summary>
        [NonSerialized]
        public const string FieldCompanyId = "CompanyId";

        ///<summary>
        /// 公司名称
        ///</summary>
        [NonSerialized]
        public const string FieldCompanyName = "CompanyName";

        ///<summary>
        /// 部门主键，数据库中可以设置为int
        ///</summary>
        [NonSerialized]
        public const string FieldDepartmentId = "DepartmentId";

        ///<summary>
        /// 部门名称
        ///</summary>
        [NonSerialized]
        public const string FieldDepartmentName = "DepartmentName";

        ///<summary>
        /// 工作组主键，数据库中可以设置为int
        ///</summary>
        [NonSerialized]
        public const string FieldWorkgroupId = "WorkgroupId";

        ///<summary>
        /// 工作组名称
        ///</summary>
        [NonSerialized]
        public const string FieldWorkgroupName = "WorkgroupName";

        ///<summary>
        /// 快速查找，记忆符
        ///</summary>
        [NonSerialized]
        public const string FieldQuickQuery = "QuickQuery";

        ///<summary>
        /// 职位代码
        ///</summary>
        [NonSerialized]
        public const string FieldDutyId = "DutyId";

        ///<summary>
        /// 唯一身份Id
        ///</summary>
        [NonSerialized]
        public const string FieldIdentificationCode = "IdentificationCode";

        ///<summary>
        /// 身份证号码
        ///</summary>
        [NonSerialized]
        public const string FieldIdCard = "IDCard";

        ///<summary>
        /// 银行卡号
        ///</summary>
        [NonSerialized]
        public const string FieldBankCode = "BankCode";

        ///<summary>
        /// 开户行
        ///</summary>
        [NonSerialized]
        public const string FieldBankName = "BankName";

        ///<summary>
        /// 开户行姓名
        ///</summary>
        [NonSerialized]
        public const string FieldBankUserName = "BankUserName";

        ///<summary>
        /// 奖金卡号
        ///</summary>
        [NonSerialized]
        public const string FieldRewardCard = "RewardCard";

        ///<summary>
        /// 医疗卡号
        ///</summary>
        [NonSerialized]
        public const string FieldMedicalCard = "MedicalCard";

        ///<summary>
        /// 工会证号
        ///</summary>
        [NonSerialized]
        public const string FieldUnionMember = "UnionMember";

        ///<summary>
        /// 电子邮件
        ///</summary>
        [NonSerialized]
        public const string FieldEmail = "Email";

        ///<summary>
        /// 手机
        ///</summary>
        [NonSerialized]
        public const string FieldMobile = "Mobile";

        ///<summary>
        /// 短号
        ///</summary>
        [NonSerialized]
        public const string FieldShortNumber = "ShortNumber";

        ///<summary>
        /// 电话
        ///</summary>
        [NonSerialized]
        public const string FieldTelephone = "Telephone";

        ///<summary>
        /// QQ号码
        ///</summary>
        [NonSerialized]
        public const string FieldQq = "QQ";

        ///<summary>
        /// 办公电话
        ///</summary>
        [NonSerialized]
        public const string FieldOfficePhone = "OfficePhone";

        ///<summary>
        /// 分机号码
        ///</summary>
        [NonSerialized]
        public const string FieldExtension = "Extension";

        ///<summary>
        /// 办公邮编
        ///</summary>
        [NonSerialized]
        public const string FieldOfficePostCode = "OfficePostCode";

        ///<summary>
        /// 办公地址
        ///</summary>
        [NonSerialized]
        public const string FieldOfficeAddress = "OfficeAddress";

        ///<summary>
        /// 办公传真
        ///</summary>
        [NonSerialized]
        public const string FieldOfficeFax = "OfficeFax";

        ///<summary>
        /// 年龄
        ///</summary>
        [NonSerialized]
        public const string FieldAge = "Age";

        ///<summary>
        /// 出生日期
        ///</summary>
        [NonSerialized]
        public const string FieldBirthday = "Birthday";

        ///<summary>
        /// 身高
        ///</summary>
        [NonSerialized]
        public const string FieldWeight = "Weight";

        ///<summary>
        /// 体重
        ///</summary>
        [NonSerialized]
        public const string FieldHeight = "Height";

        ///<summary>
        /// 婚姻情况
        ///</summary>
        [NonSerialized]
        public const string FieldMarriage = "Marriage";

        ///<summary>
        /// 最高学历
        ///</summary>
        [NonSerialized]
        public const string FieldEducation = "Education";

        ///<summary>
        /// 毕业院校
        ///</summary>
        [NonSerialized]
        public const string FieldSchool = "School";

        ///<summary>
        /// 毕业时间
        ///</summary>
        [NonSerialized]
        public const string FieldGraduationDate = "GraduationDate";

        ///<summary>
        /// 专业
        ///</summary>
        [NonSerialized]
        public const string FieldMajor = "Major";

        ///<summary>
        /// 最高学位
        ///</summary>
        [NonSerialized]
        public const string FieldDegree = "Degree";

        ///<summary>
        /// 职称主键
        ///</summary>
        [NonSerialized]
        public const string FieldTitleId = "TitleId";

        ///<summary>
        /// 职称评定日期
        ///</summary>
        [NonSerialized]
        public const string FieldTitleDate = "TitleDate";

        ///<summary>
        /// 职称等级
        ///</summary>
        [NonSerialized]
        public const string FieldTitleLevel = "TitleLevel";

        ///<summary>
        /// 工作时间
        ///</summary>
        [NonSerialized]
        public const string FieldWorkingDate = "WorkingDate";

        ///<summary>
        /// 加入本单位时间
        ///</summary>
        [NonSerialized]
        public const string FieldJoinInDate = "JoinInDate";

        ///<summary>
        /// 家庭住址邮编
        ///</summary>
        [NonSerialized]
        public const string FieldHomePostCode = "HomePostCode";

        ///<summary>
        /// 家庭住址
        ///</summary>
        [NonSerialized]
        public const string FieldHomeAddress = "HomeAddress";

        ///<summary>
        /// 住宅电话
        ///</summary>
        [NonSerialized]
        public const string FieldHomePhone = "HomePhone";

        ///<summary>
        /// 家庭传真
        ///</summary>
        [NonSerialized]
        public const string FieldHomeFax = "HomeFax";

        ///<summary>
        /// 车牌号
        ///</summary>
        [NonSerialized]
        public const string FieldCarCode = "CarCode";

        ///<summary>
        /// 籍贯省
        ///</summary>
        [NonSerialized]
        public const string FieldProvince = "Province";

        ///<summary>
        /// 籍贯市
        ///</summary>
        [NonSerialized]
        public const string FieldCity = "City";

        ///<summary>
        /// 籍贯区
        ///</summary>
        [NonSerialized]
        public const string FieldDistrict = "District";

        ///<summary>
        /// 籍贯
        ///</summary>
        [NonSerialized]
        public const string FieldNativePlace = "NativePlace";

        ///<summary>
        /// 当前省
        ///</summary>
        [NonSerialized]
        public const string FieldCurrentProvince = "CurrentProvince";

        ///<summary>
        /// 当前市
        ///</summary>
        [NonSerialized]
        public const string FieldCurrentCity = "CurrentCity";

        ///<summary>
        /// 当前区
        ///</summary>
        [NonSerialized]
        public const string FieldCurrentDistrict = "CurrentDistrict";

        ///<summary>
        /// 政治面貌
        ///</summary>
        [NonSerialized]
        public const string FieldParty = "Party";

        ///<summary>
        /// 国籍
        ///</summary>
        [NonSerialized]
        public const string FieldNation = "Nation";

        ///<summary>
        /// 民族
        ///</summary>
        [NonSerialized]
        public const string FieldNationality = "Nationality";

        ///<summary>
        /// 工作性质
        ///</summary>
        [NonSerialized]
        public const string FieldWorkingProperty = "WorkingProperty";

        ///<summary>
        /// 职业资格
        ///</summary>
        [NonSerialized]
        public const string FieldCompetency = "Competency";

        ///<summary>
        /// 紧急联系
        ///</summary>
        [NonSerialized]
        public const string FieldEmergencyContact = "EmergencyContact";

        ///<summary>
        /// 结婚日期
        ///</summary>
        [NonSerialized]
        public const string FieldWeddingDay = "WeddingDay";

        ///<summary>
        /// 离婚日期
        ///</summary>
        [NonSerialized]
        public const string FieldDivorce = "Divorce";

        ///<summary>
        /// 第一个孩子出生时间
        ///</summary>
        [NonSerialized]
        public const string FieldChildBirthday1 = "ChildBirthday1";

        ///<summary>
        /// 第二个孩子出生时间
        ///</summary>
        [NonSerialized]
        public const string FieldChildBirthday2 = "ChildBirthday2";

        ///<summary>
        /// 离职
        ///</summary>
        [NonSerialized]
        public const string FieldIsDimission = "IsDimission";

        ///<summary>
        /// 离职日期
        ///</summary>
        [NonSerialized]
        public const string FieldDimissionDate = "DimissionDate";

        ///<summary>
        /// 离职原因
        ///</summary>
        [NonSerialized]
        public const string FieldDimissionCause = "DimissionCause";

        ///<summary>
        /// 离职去向
        ///</summary>
        [NonSerialized]
        public const string FieldDimissionWhither = "DimissionWhither";

        ///<summary>
        /// 有效
        ///</summary>
        [NonSerialized]
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 是否删除
        ///</summary>
        [NonSerialized]
        public const string FieldDeleted = "DeletionStateCode";

        ///<summary>
        /// 排序码
        ///</summary>
        [NonSerialized]
        public const string FieldSortCode = "SortCode";

        ///<summary>
        /// 备注
        ///</summary>
        [NonSerialized]
        public const string FieldDescription = "Description";

        ///<summary>
        /// 创建时间
        ///</summary>
        [NonSerialized]
        public const string FieldCreateTime = "CreateOn";

        ///<summary>
        /// 创建人用户编号
        ///</summary>
        [NonSerialized]
        public const string FieldCreateUserId = "CreateUserId";

        ///<summary>
        /// 创建人
        ///</summary>
        [NonSerialized]
        public const string FieldCreateBy = "CreateBy";

        ///<summary>
        /// 修改时间
        ///</summary>
        [NonSerialized]
        public const string FieldUpdateTime = "ModifiedOn";

        ///<summary>
        /// 修改人用户编号
        ///</summary>
        [NonSerialized]
        public const string FieldUpdateUserId = "ModifiedUserId";

        ///<summary>
        /// 修改人
        ///</summary>
        [NonSerialized]
        public const string FieldUpdateBy = "ModifiedBy";

        ///<summary>
        /// 岗位名称
        ///</summary>
        [NonSerialized]
        public const string FieldDuty = "Duty";

        ///<summary>
        /// 岗位编号
        ///</summary>
        [NonSerialized]
        public const string FieldDutyCode = "DutyCode";

        ///<summary>
        /// 岗位序列
        ///</summary>
        [NonSerialized]
        public const string FieldDutyType = "DutyType";

        ///<summary>
        /// 岗位等级
        ///</summary>
        [NonSerialized]
        public const string FieldDutyLevel = "DutyLevel";
    }
}
