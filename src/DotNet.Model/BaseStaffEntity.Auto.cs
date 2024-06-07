//-----------------------------------------------------------------------
// <copyright file="BaseStaffEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2024, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseStaffEntity
    /// 员工
    /// 
    /// 修改记录
    /// 
    /// 2022-10-23 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2022-10-23</date>
    /// </author>
    /// </summary>
    [Table(CurrentTableName)]
    public partial class BaseStaffEntity : BaseEntity
    {
        /// <summary>
        /// 用户主键
        /// </summary>
        [FieldDescription("用户主键")]
        [Description("用户主键")]
        [Column(FieldUserId)]
        public int UserId { get; set; } = 0;

        /// <summary>
        /// 用户名
        /// </summary>
        [FieldDescription("用户名")]
        [Description("用户名")]
        [Column(FieldUserName)]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 工号
        /// </summary>
        [FieldDescription("工号")]
        [Description("工号")]
        [Column(FieldEmployeeNumber)]
        public string EmployeeNumber { get; set; } = string.Empty;

        /// <summary>
        /// 姓名
        /// </summary>
        [FieldDescription("姓名")]
        [Description("姓名")]
        [Column(FieldRealName)]
        public string RealName { get; set; } = string.Empty;

        /// <summary>
        /// 中文名
        /// </summary>
        [FieldDescription("中文名")]
        [Description("中文名")]
        [Column(FieldChineseName)]
        public string ChineseName { get; set; } = string.Empty;

        /// <summary>
        /// 英文名
        /// </summary>
        [FieldDescription("英文名")]
        [Description("英文名")]
        [Column(FieldEnglishName)]
        public string EnglishName { get; set; } = string.Empty;

        /// <summary>
        /// 快速查找，记忆符
        /// </summary>
        [FieldDescription("快速查找，记忆符")]
        [Description("快速查找，记忆符")]
        [Column(FieldQuickQuery)]
        public string QuickQuery { get; set; } = string.Empty;

        /// <summary>
        /// 公司主键
        /// </summary>
        [FieldDescription("公司主键")]
        [Description("公司主键")]
        [Column(FieldCompanyId)]
        public int CompanyId { get; set; } = 0;

        /// <summary>
        /// 公司名称
        /// </summary>
        [FieldDescription("公司名称")]
        [Description("公司名称")]
        [Column(FieldCompanyName)]
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// 分支机构主键
        /// </summary>
        [FieldDescription("分支机构主键")]
        [Description("分支机构主键")]
        [Column(FieldSubCompanyId)]
        public int SubCompanyId { get; set; } = 0;

        /// <summary>
        /// 子公司名称
        /// </summary>
        [FieldDescription("子公司名称")]
        [Description("子公司名称")]
        [Column(FieldSubCompanyName)]
        public string SubCompanyName { get; set; } = string.Empty;

        /// <summary>
        /// 部门主键
        /// </summary>
        [FieldDescription("部门主键")]
        [Description("部门主键")]
        [Column(FieldDepartmentId)]
        public int DepartmentId { get; set; } = 0;

        /// <summary>
        /// 部门名称
        /// </summary>
        [FieldDescription("部门名称")]
        [Description("部门名称")]
        [Column(FieldDepartmentName)]
        public string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// 工作组主键
        /// </summary>
        [FieldDescription("工作组主键")]
        [Description("工作组主键")]
        [Column(FieldWorkgroupId)]
        public int WorkgroupId { get; set; } = 0;

        /// <summary>
        /// 工作组名称
        /// </summary>
        [FieldDescription("工作组名称")]
        [Description("工作组名称")]
        [Column(FieldWorkgroupName)]
        public string WorkgroupName { get; set; } = string.Empty;

        /// <summary>
        /// 职位主键
        /// </summary>
        [FieldDescription("职位主键")]
        [Description("职位主键")]
        [Column(FieldDutyId)]
        public int DutyId { get; set; } = 0;

        /// <summary>
        /// 性别
        /// </summary>
        [FieldDescription("性别")]
        [Description("性别")]
        [Column(FieldGender)]
        public string Gender { get; set; } = string.Empty;

        /// <summary>
        /// 生日
        /// </summary>
        [FieldDescription("生日")]
        [Description("生日")]
        [Column(FieldBirthday)]
        public DateTime? Birthday { get; set; } = null;

        /// <summary>
        /// 年龄
        /// </summary>
        [FieldDescription("年龄")]
        [Description("年龄")]
        [Column(FieldAge)]
        public int? Age { get; set; } = null;

        /// <summary>
        /// 身高
        /// </summary>
        [FieldDescription("身高")]
        [Description("身高")]
        [Column(FieldHeight)]
        public string Height { get; set; } = string.Empty;

        /// <summary>
        /// 体重
        /// </summary>
        [FieldDescription("体重")]
        [Description("体重")]
        [Column(FieldWeight)]
        public string Weight { get; set; } = string.Empty;

        /// <summary>
        /// 唯一身份Id
        /// </summary>
        [FieldDescription("唯一身份Id")]
        [Description("唯一身份Id")]
        [Column(FieldIdentificationCode)]
        public string IdentificationCode { get; set; } = string.Empty;

        /// <summary>
        /// 身份证号码
        /// </summary>
        [FieldDescription("身份证号码")]
        [Description("身份证号码")]
        [Column(FieldIdCard)]
        public string IdCard { get; set; } = string.Empty;

        /// <summary>
        /// 国籍
        /// </summary>
        [FieldDescription("国籍")]
        [Description("国籍")]
        [Column(FieldNation)]
        public string Nation { get; set; } = string.Empty;

        /// <summary>
        /// 最高学历
        /// </summary>
        [FieldDescription("最高学历")]
        [Description("最高学历")]
        [Column(FieldEducation)]
        public string Education { get; set; } = string.Empty;

        /// <summary>
        /// 毕业院校
        /// </summary>
        [FieldDescription("毕业院校")]
        [Description("毕业院校")]
        [Column(FieldSchool)]
        public string School { get; set; } = string.Empty;

        /// <summary>
        /// 毕业日期
        /// </summary>
        [FieldDescription("毕业日期")]
        [Description("毕业日期")]
        [Column(FieldGraduationDate)]
        public DateTime? GraduationDate { get; set; } = null;

        /// <summary>
        /// 专业
        /// </summary>
        [FieldDescription("专业")]
        [Description("专业")]
        [Column(FieldMajor)]
        public string Major { get; set; } = string.Empty;

        /// <summary>
        /// 最高学位
        /// </summary>
        [FieldDescription("最高学位")]
        [Description("最高学位")]
        [Column(FieldDegree)]
        public string Degree { get; set; } = string.Empty;

        /// <summary>
        /// 职称主键
        /// </summary>
        [FieldDescription("职称主键")]
        [Description("职称主键")]
        [Column(FieldTitleId)]
        public string TitleId { get; set; } = string.Empty;

        /// <summary>
        /// 职称评定日期
        /// </summary>
        [FieldDescription("职称评定日期")]
        [Description("职称评定日期")]
        [Column(FieldTitleDate)]
        public string TitleDate { get; set; } = string.Empty;

        /// <summary>
        /// 职称等级
        /// </summary>
        [FieldDescription("职称等级")]
        [Description("职称等级")]
        [Column(FieldTitleLevel)]
        public string TitleLevel { get; set; } = string.Empty;

        /// <summary>
        /// 工作时间
        /// </summary>
        [FieldDescription("工作时间")]
        [Description("工作时间")]
        [Column(FieldWorkingDate)]
        public DateTime? WorkingDate { get; set; } = null;

        /// <summary>
        /// 加入本单位时间
        /// </summary>
        [FieldDescription("加入本单位时间")]
        [Description("加入本单位时间")]
        [Column(FieldJoinInDate)]
        public DateTime? JoinInDate { get; set; } = null;

        /// <summary>
        /// 办公邮编
        /// </summary>
        [FieldDescription("办公邮编")]
        [Description("办公邮编")]
        [Column(FieldOfficePostCode)]
        public string OfficePostCode { get; set; } = string.Empty;

        /// <summary>
        /// 办公地址
        /// </summary>
        [FieldDescription("办公地址")]
        [Description("办公地址")]
        [Column(FieldOfficeAddress)]
        public string OfficeAddress { get; set; } = string.Empty;

        /// <summary>
        /// 办公电话
        /// </summary>
        [FieldDescription("办公电话")]
        [Description("办公电话")]
        [Column(FieldOfficePhone)]
        public string OfficePhone { get; set; } = string.Empty;

        /// <summary>
        /// 办公传真
        /// </summary>
        [FieldDescription("办公传真")]
        [Description("办公传真")]
        [Column(FieldOfficeFax)]
        public string OfficeFax { get; set; } = string.Empty;

        /// <summary>
        /// 家庭住址邮编
        /// </summary>
        [FieldDescription("家庭住址邮编")]
        [Description("家庭住址邮编")]
        [Column(FieldHomePostCode)]
        public string HomePostCode { get; set; } = string.Empty;

        /// <summary>
        /// 家庭住址
        /// </summary>
        [FieldDescription("家庭住址")]
        [Description("家庭住址")]
        [Column(FieldHomeAddress)]
        public string HomeAddress { get; set; } = string.Empty;

        /// <summary>
        /// 住宅电话
        /// </summary>
        [FieldDescription("住宅电话")]
        [Description("住宅电话")]
        [Column(FieldHomePhone)]
        public string HomePhone { get; set; } = string.Empty;

        /// <summary>
        /// 家庭传真
        /// </summary>
        [FieldDescription("家庭传真")]
        [Description("家庭传真")]
        [Column(FieldHomeFax)]
        public string HomeFax { get; set; } = string.Empty;

        /// <summary>
        /// 第一辆车牌号
        /// </summary>
        [FieldDescription("第一辆车牌号")]
        [Description("第一辆车牌号")]
        [Column(FieldPlateNumber1)]
        public string PlateNumber1 { get; set; } = string.Empty;

        /// <summary>
        /// 第二辆车牌号
        /// </summary>
        [FieldDescription("第二辆车牌号")]
        [Description("第二辆车牌号")]
        [Column(FieldPlateNumber2)]
        public string PlateNumber2 { get; set; } = string.Empty;

        /// <summary>
        /// 第三辆车牌号
        /// </summary>
        [FieldDescription("第三辆车牌号")]
        [Description("第三辆车牌号")]
        [Column(FieldPlateNumber3)]
        public string PlateNumber3 { get; set; } = string.Empty;

        /// <summary>
        /// 奖金卡号
        /// </summary>
        [FieldDescription("奖金卡号")]
        [Description("奖金卡号")]
        [Column(FieldRewardCard)]
        public string RewardCard { get; set; } = string.Empty;

        /// <summary>
        /// 医疗卡号
        /// </summary>
        [FieldDescription("医疗卡号")]
        [Description("医疗卡号")]
        [Column(FieldMedicalCard)]
        public string MedicalCard { get; set; } = string.Empty;

        /// <summary>
        /// 工会证号
        /// </summary>
        [FieldDescription("工会证号")]
        [Description("工会证号")]
        [Column(FieldUnionMember)]
        public string UnionMember { get; set; } = string.Empty;

        /// <summary>
        /// Email
        /// </summary>
        [FieldDescription("Email")]
        [Description("Email")]
        [Column(FieldEmail)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 手机
        /// </summary>
        [FieldDescription("手机")]
        [Description("手机")]
        [Column(FieldMobile)]
        public string Mobile { get; set; } = string.Empty;

        /// <summary>
        /// QQ
        /// </summary>
        [FieldDescription("QQ")]
        [Description("QQ")]
        [Column(FieldQq)]
        public string Qq { get; set; } = string.Empty;

        /// <summary>
        /// 微信
        /// </summary>
        [FieldDescription("微信")]
        [Description("微信")]
        [Column(FieldWeChat)]
        public string WeChat { get; set; } = string.Empty;

        /// <summary>
        /// 短号
        /// </summary>
        [FieldDescription("短号")]
        [Description("短号")]
        [Column(FieldShortNumber)]
        public string ShortNumber { get; set; } = string.Empty;

        /// <summary>
        /// 电话
        /// </summary>
        [FieldDescription("电话")]
        [Description("电话")]
        [Column(FieldTelephone)]
        public string Telephone { get; set; } = string.Empty;

        /// <summary>
        /// 分机
        /// </summary>
        [FieldDescription("分机")]
        [Description("分机")]
        [Column(FieldExtension)]
        public string Extension { get; set; } = string.Empty;

        /// <summary>
        /// 紧急联系
        /// </summary>
        [FieldDescription("紧急联系")]
        [Description("紧急联系")]
        [Column(FieldEmergencyContact)]
        public string EmergencyContact { get; set; } = string.Empty;

        /// <summary>
        /// 紧急联系手机
        /// </summary>
        [FieldDescription("紧急联系手机")]
        [Description("紧急联系手机")]
        [Column(FieldEmergencyMobile)]
        public string EmergencyMobile { get; set; } = string.Empty;

        /// <summary>
        /// 紧急联系电话
        /// </summary>
        [FieldDescription("紧急联系电话")]
        [Description("紧急联系电话")]
        [Column(FieldEmergencyTelephone)]
        public string EmergencyTelephone { get; set; } = string.Empty;

        /// <summary>
        /// 籍贯
        /// </summary>
        [FieldDescription("籍贯")]
        [Description("籍贯")]
        [Column(FieldNativePlace)]
        public string NativePlace { get; set; } = string.Empty;

        /// <summary>
        /// 开户行
        /// </summary>
        [FieldDescription("开户行")]
        [Description("开户行")]
        [Column(FieldBankName)]
        public string BankName { get; set; } = string.Empty;

        /// <summary>
        /// 银行卡号
        /// </summary>
        [FieldDescription("银行卡号")]
        [Description("银行卡号")]
        [Column(FieldBankAccount)]
        public string BankAccount { get; set; } = string.Empty;

        /// <summary>
        /// 开户行姓名
        /// </summary>
        [FieldDescription("开户行姓名")]
        [Description("开户行姓名")]
        [Column(FieldBankUserName)]
        public string BankUserName { get; set; } = string.Empty;

        /// <summary>
        /// 籍贯省
        /// </summary>
        [FieldDescription("籍贯省")]
        [Description("籍贯省")]
        [Column(FieldProvince)]
        public string Province { get; set; } = string.Empty;

        /// <summary>
        /// 籍贯市
        /// </summary>
        [FieldDescription("籍贯市")]
        [Description("籍贯市")]
        [Column(FieldCity)]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// 籍贯区
        /// </summary>
        [FieldDescription("籍贯区")]
        [Description("籍贯区")]
        [Column(FieldDistrict)]
        public string District { get; set; } = string.Empty;

        /// <summary>
        /// 当前省
        /// </summary>
        [FieldDescription("当前省")]
        [Description("当前省")]
        [Column(FieldCurrentProvince)]
        public string CurrentProvince { get; set; } = string.Empty;

        /// <summary>
        /// 当前市
        /// </summary>
        [FieldDescription("当前市")]
        [Description("当前市")]
        [Column(FieldCurrentCity)]
        public string CurrentCity { get; set; } = string.Empty;

        /// <summary>
        /// 当前区
        /// </summary>
        [FieldDescription("当前区")]
        [Description("当前区")]
        [Column(FieldCurrentDistrict)]
        public string CurrentDistrict { get; set; } = string.Empty;

        /// <summary>
        /// 政治面貌
        /// </summary>
        [FieldDescription("政治面貌")]
        [Description("政治面貌")]
        [Column(FieldParty)]
        public string Party { get; set; } = string.Empty;

        /// <summary>
        /// 民族
        /// </summary>
        [FieldDescription("民族")]
        [Description("民族")]
        [Column(FieldNationality)]
        public string Nationality { get; set; } = string.Empty;

        /// <summary>
        /// 工作性质
        /// </summary>
        [FieldDescription("工作性质")]
        [Description("工作性质")]
        [Column(FieldWorkingProperty)]
        public string WorkingProperty { get; set; } = string.Empty;

        /// <summary>
        /// 职业资格
        /// </summary>
        [FieldDescription("职业资格")]
        [Description("职业资格")]
        [Column(FieldCompetency)]
        public string Competency { get; set; } = string.Empty;

        /// <summary>
        /// 婚姻情况
        /// </summary>
        [FieldDescription("婚姻情况")]
        [Description("婚姻情况")]
        [Column(FieldMarriage)]
        public string Marriage { get; set; } = string.Empty;

        /// <summary>
        /// 结婚日期
        /// </summary>
        [FieldDescription("结婚日期")]
        [Description("结婚日期")]
        [Column(FieldWeddingDate)]
        public DateTime? WeddingDate { get; set; } = null;

        /// <summary>
        /// 离婚日期
        /// </summary>
        [FieldDescription("离婚日期")]
        [Description("离婚日期")]
        [Column(FieldDivorceDate)]
        public DateTime? DivorceDate { get; set; } = null;

        /// <summary>
        /// 第一个孩子生日
        /// </summary>
        [FieldDescription("第一个孩子生日")]
        [Description("第一个孩子生日")]
        [Column(FieldChild1Birthday)]
        public DateTime? Child1Birthday { get; set; } = null;

        /// <summary>
        /// 第二个孩子生日
        /// </summary>
        [FieldDescription("第二个孩子生日")]
        [Description("第二个孩子生日")]
        [Column(FieldChild2Birthday)]
        public DateTime? Child2Birthday { get; set; } = null;

        /// <summary>
        /// 第三个孩子生日
        /// </summary>
        [FieldDescription("第三个孩子生日")]
        [Description("第三个孩子生日")]
        [Column(FieldChild3Birthday)]
        public DateTime? Child3Birthday { get; set; } = null;

        /// <summary>
        /// 第四个孩子生日
        /// </summary>
        [FieldDescription("第四个孩子生日")]
        [Description("第四个孩子生日")]
        [Column(FieldChild4Birthday)]
        public DateTime? Child4Birthday { get; set; } = null;

        /// <summary>
        /// 第五个孩子生日
        /// </summary>
        [FieldDescription("第五个孩子生日")]
        [Description("第五个孩子生日")]
        [Column(FieldChild5Birthday)]
        public DateTime? Child5Birthday { get; set; } = null;

        /// <summary>
        /// 是否离职
        /// </summary>
        [FieldDescription("是否离职")]
        [Description("是否离职")]
        [Column(FieldIsDimission)]
        public int IsDimission { get; set; } = 0;

        /// <summary>
        /// 离职日期
        /// </summary>
        [FieldDescription("离职日期")]
        [Description("离职日期")]
        [Column(FieldDimissionDate)]
        public DateTime? DimissionDate { get; set; } = null;

        /// <summary>
        /// 离职原因
        /// </summary>
        [FieldDescription("离职原因")]
        [Description("离职原因")]
        [Column(FieldDimissionCause)]
        public string DimissionCause { get; set; } = string.Empty;

        /// <summary>
        /// 离职去向
        /// </summary>
        [FieldDescription("离职去向")]
        [Description("离职去向")]
        [Column(FieldDimissionWhereabouts)]
        public string DimissionWhereabouts { get; set; } = string.Empty;

        /// <summary>
        /// 扩展信息1
        /// </summary>
        [FieldDescription("扩展信息1")]
        [Description("扩展信息1")]
        [Column(FieldExt1)]
        public string Ext1 { get; set; } = string.Empty;

        /// <summary>
        /// 扩展信息2
        /// </summary>
        [FieldDescription("扩展信息2")]
        [Description("扩展信息2")]
        [Column(FieldExt2)]
        public string Ext2 { get; set; } = string.Empty;

        /// <summary>
        /// 扩展信息3
        /// </summary>
        [FieldDescription("扩展信息3")]
        [Description("扩展信息3")]
        [Column(FieldExt3)]
        public string Ext3 { get; set; } = string.Empty;

        /// <summary>
        /// 扩展信息4
        /// </summary>
        [FieldDescription("扩展信息4")]
        [Description("扩展信息4")]
        [Column(FieldExt4)]
        public string Ext4 { get; set; } = string.Empty;

        /// <summary>
        /// 扩展信息5
        /// </summary>
        [FieldDescription("扩展信息5")]
        [Description("扩展信息5")]
        [Column(FieldExt5)]
        public string Ext5 { get; set; } = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        [FieldDescription("描述")]
        [Description("描述")]
        [Column(FieldDescription)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExtend(dr);
            GetBase(dr);
            if (dr.ContainsColumn(FieldUserId))
            {
                UserId = BaseUtil.ConvertToInt(dr[FieldUserId]);
            }
            if (dr.ContainsColumn(FieldUserName))
            {
                UserName = BaseUtil.ConvertToString(dr[FieldUserName]);
            }
            if (dr.ContainsColumn(FieldEmployeeNumber))
            {
                EmployeeNumber = BaseUtil.ConvertToString(dr[FieldEmployeeNumber]);
            }
            if (dr.ContainsColumn(FieldRealName))
            {
                RealName = BaseUtil.ConvertToString(dr[FieldRealName]);
            }
            if (dr.ContainsColumn(FieldChineseName))
            {
                ChineseName = BaseUtil.ConvertToString(dr[FieldChineseName]);
            }
            if (dr.ContainsColumn(FieldEnglishName))
            {
                EnglishName = BaseUtil.ConvertToString(dr[FieldEnglishName]);
            }
            if (dr.ContainsColumn(FieldQuickQuery))
            {
                QuickQuery = BaseUtil.ConvertToString(dr[FieldQuickQuery]);
            }
            if (dr.ContainsColumn(FieldCompanyId))
            {
                CompanyId = BaseUtil.ConvertToInt(dr[FieldCompanyId]);
            }
            if (dr.ContainsColumn(FieldCompanyName))
            {
                CompanyName = BaseUtil.ConvertToString(dr[FieldCompanyName]);
            }
            if (dr.ContainsColumn(FieldSubCompanyId))
            {
                SubCompanyId = BaseUtil.ConvertToInt(dr[FieldSubCompanyId]);
            }
            if (dr.ContainsColumn(FieldSubCompanyName))
            {
                SubCompanyName = BaseUtil.ConvertToString(dr[FieldSubCompanyName]);
            }
            if (dr.ContainsColumn(FieldDepartmentId))
            {
                DepartmentId = BaseUtil.ConvertToInt(dr[FieldDepartmentId]);
            }
            if (dr.ContainsColumn(FieldDepartmentName))
            {
                DepartmentName = BaseUtil.ConvertToString(dr[FieldDepartmentName]);
            }
            if (dr.ContainsColumn(FieldWorkgroupId))
            {
                WorkgroupId = BaseUtil.ConvertToInt(dr[FieldWorkgroupId]);
            }
            if (dr.ContainsColumn(FieldWorkgroupName))
            {
                WorkgroupName = BaseUtil.ConvertToString(dr[FieldWorkgroupName]);
            }
            if (dr.ContainsColumn(FieldDutyId))
            {
                DutyId = BaseUtil.ConvertToInt(dr[FieldDutyId]);
            }
            if (dr.ContainsColumn(FieldGender))
            {
                Gender = BaseUtil.ConvertToString(dr[FieldGender]);
            }
            if (dr.ContainsColumn(FieldBirthday))
            {
                Birthday = BaseUtil.ConvertToNullableDateTime(dr[FieldBirthday]);
            }
            if (dr.ContainsColumn(FieldAge))
            {
                Age = BaseUtil.ConvertToNullableInt(dr[FieldAge]);
            }
            if (dr.ContainsColumn(FieldHeight))
            {
                Height = BaseUtil.ConvertToString(dr[FieldHeight]);
            }
            if (dr.ContainsColumn(FieldWeight))
            {
                Weight = BaseUtil.ConvertToString(dr[FieldWeight]);
            }
            if (dr.ContainsColumn(FieldIdentificationCode))
            {
                IdentificationCode = BaseUtil.ConvertToString(dr[FieldIdentificationCode]);
            }
            if (dr.ContainsColumn(FieldIdCard))
            {
                IdCard = BaseUtil.ConvertToString(dr[FieldIdCard]);
            }
            if (dr.ContainsColumn(FieldNation))
            {
                Nation = BaseUtil.ConvertToString(dr[FieldNation]);
            }
            if (dr.ContainsColumn(FieldEducation))
            {
                Education = BaseUtil.ConvertToString(dr[FieldEducation]);
            }
            if (dr.ContainsColumn(FieldSchool))
            {
                School = BaseUtil.ConvertToString(dr[FieldSchool]);
            }
            if (dr.ContainsColumn(FieldGraduationDate))
            {
                GraduationDate = BaseUtil.ConvertToNullableDateTime(dr[FieldGraduationDate]);
            }
            if (dr.ContainsColumn(FieldMajor))
            {
                Major = BaseUtil.ConvertToString(dr[FieldMajor]);
            }
            if (dr.ContainsColumn(FieldDegree))
            {
                Degree = BaseUtil.ConvertToString(dr[FieldDegree]);
            }
            if (dr.ContainsColumn(FieldTitleId))
            {
                TitleId = BaseUtil.ConvertToString(dr[FieldTitleId]);
            }
            if (dr.ContainsColumn(FieldTitleDate))
            {
                TitleDate = BaseUtil.ConvertToString(dr[FieldTitleDate]);
            }
            if (dr.ContainsColumn(FieldTitleLevel))
            {
                TitleLevel = BaseUtil.ConvertToString(dr[FieldTitleLevel]);
            }
            if (dr.ContainsColumn(FieldWorkingDate))
            {
                WorkingDate = BaseUtil.ConvertToNullableDateTime(dr[FieldWorkingDate]);
            }
            if (dr.ContainsColumn(FieldJoinInDate))
            {
                JoinInDate = BaseUtil.ConvertToNullableDateTime(dr[FieldJoinInDate]);
            }
            if (dr.ContainsColumn(FieldOfficePostCode))
            {
                OfficePostCode = BaseUtil.ConvertToString(dr[FieldOfficePostCode]);
            }
            if (dr.ContainsColumn(FieldOfficeAddress))
            {
                OfficeAddress = BaseUtil.ConvertToString(dr[FieldOfficeAddress]);
            }
            if (dr.ContainsColumn(FieldOfficePhone))
            {
                OfficePhone = BaseUtil.ConvertToString(dr[FieldOfficePhone]);
            }
            if (dr.ContainsColumn(FieldOfficeFax))
            {
                OfficeFax = BaseUtil.ConvertToString(dr[FieldOfficeFax]);
            }
            if (dr.ContainsColumn(FieldHomePostCode))
            {
                HomePostCode = BaseUtil.ConvertToString(dr[FieldHomePostCode]);
            }
            if (dr.ContainsColumn(FieldHomeAddress))
            {
                HomeAddress = BaseUtil.ConvertToString(dr[FieldHomeAddress]);
            }
            if (dr.ContainsColumn(FieldHomePhone))
            {
                HomePhone = BaseUtil.ConvertToString(dr[FieldHomePhone]);
            }
            if (dr.ContainsColumn(FieldHomeFax))
            {
                HomeFax = BaseUtil.ConvertToString(dr[FieldHomeFax]);
            }
            if (dr.ContainsColumn(FieldPlateNumber1))
            {
                PlateNumber1 = BaseUtil.ConvertToString(dr[FieldPlateNumber1]);
            }
            if (dr.ContainsColumn(FieldPlateNumber2))
            {
                PlateNumber2 = BaseUtil.ConvertToString(dr[FieldPlateNumber2]);
            }
            if (dr.ContainsColumn(FieldPlateNumber3))
            {
                PlateNumber3 = BaseUtil.ConvertToString(dr[FieldPlateNumber3]);
            }
            if (dr.ContainsColumn(FieldRewardCard))
            {
                RewardCard = BaseUtil.ConvertToString(dr[FieldRewardCard]);
            }
            if (dr.ContainsColumn(FieldMedicalCard))
            {
                MedicalCard = BaseUtil.ConvertToString(dr[FieldMedicalCard]);
            }
            if (dr.ContainsColumn(FieldUnionMember))
            {
                UnionMember = BaseUtil.ConvertToString(dr[FieldUnionMember]);
            }
            if (dr.ContainsColumn(FieldEmail))
            {
                Email = BaseUtil.ConvertToString(dr[FieldEmail]);
            }
            if (dr.ContainsColumn(FieldMobile))
            {
                Mobile = BaseUtil.ConvertToString(dr[FieldMobile]);
            }
            if (dr.ContainsColumn(FieldQq))
            {
                Qq = BaseUtil.ConvertToString(dr[FieldQq]);
            }
            if (dr.ContainsColumn(FieldWeChat))
            {
                WeChat = BaseUtil.ConvertToString(dr[FieldWeChat]);
            }
            if (dr.ContainsColumn(FieldShortNumber))
            {
                ShortNumber = BaseUtil.ConvertToString(dr[FieldShortNumber]);
            }
            if (dr.ContainsColumn(FieldTelephone))
            {
                Telephone = BaseUtil.ConvertToString(dr[FieldTelephone]);
            }
            if (dr.ContainsColumn(FieldExtension))
            {
                Extension = BaseUtil.ConvertToString(dr[FieldExtension]);
            }
            if (dr.ContainsColumn(FieldEmergencyContact))
            {
                EmergencyContact = BaseUtil.ConvertToString(dr[FieldEmergencyContact]);
            }
            if (dr.ContainsColumn(FieldEmergencyMobile))
            {
                EmergencyMobile = BaseUtil.ConvertToString(dr[FieldEmergencyMobile]);
            }
            if (dr.ContainsColumn(FieldEmergencyTelephone))
            {
                EmergencyTelephone = BaseUtil.ConvertToString(dr[FieldEmergencyTelephone]);
            }
            if (dr.ContainsColumn(FieldNativePlace))
            {
                NativePlace = BaseUtil.ConvertToString(dr[FieldNativePlace]);
            }
            if (dr.ContainsColumn(FieldBankName))
            {
                BankName = BaseUtil.ConvertToString(dr[FieldBankName]);
            }
            if (dr.ContainsColumn(FieldBankAccount))
            {
                BankAccount = BaseUtil.ConvertToString(dr[FieldBankAccount]);
            }
            if (dr.ContainsColumn(FieldBankUserName))
            {
                BankUserName = BaseUtil.ConvertToString(dr[FieldBankUserName]);
            }
            if (dr.ContainsColumn(FieldProvince))
            {
                Province = BaseUtil.ConvertToString(dr[FieldProvince]);
            }
            if (dr.ContainsColumn(FieldCity))
            {
                City = BaseUtil.ConvertToString(dr[FieldCity]);
            }
            if (dr.ContainsColumn(FieldDistrict))
            {
                District = BaseUtil.ConvertToString(dr[FieldDistrict]);
            }
            if (dr.ContainsColumn(FieldCurrentProvince))
            {
                CurrentProvince = BaseUtil.ConvertToString(dr[FieldCurrentProvince]);
            }
            if (dr.ContainsColumn(FieldCurrentCity))
            {
                CurrentCity = BaseUtil.ConvertToString(dr[FieldCurrentCity]);
            }
            if (dr.ContainsColumn(FieldCurrentDistrict))
            {
                CurrentDistrict = BaseUtil.ConvertToString(dr[FieldCurrentDistrict]);
            }
            if (dr.ContainsColumn(FieldParty))
            {
                Party = BaseUtil.ConvertToString(dr[FieldParty]);
            }
            if (dr.ContainsColumn(FieldNationality))
            {
                Nationality = BaseUtil.ConvertToString(dr[FieldNationality]);
            }
            if (dr.ContainsColumn(FieldWorkingProperty))
            {
                WorkingProperty = BaseUtil.ConvertToString(dr[FieldWorkingProperty]);
            }
            if (dr.ContainsColumn(FieldCompetency))
            {
                Competency = BaseUtil.ConvertToString(dr[FieldCompetency]);
            }
            if (dr.ContainsColumn(FieldMarriage))
            {
                Marriage = BaseUtil.ConvertToString(dr[FieldMarriage]);
            }
            if (dr.ContainsColumn(FieldWeddingDate))
            {
                WeddingDate = BaseUtil.ConvertToNullableDateTime(dr[FieldWeddingDate]);
            }
            if (dr.ContainsColumn(FieldDivorceDate))
            {
                DivorceDate = BaseUtil.ConvertToNullableDateTime(dr[FieldDivorceDate]);
            }
            if (dr.ContainsColumn(FieldChild1Birthday))
            {
                Child1Birthday = BaseUtil.ConvertToNullableDateTime(dr[FieldChild1Birthday]);
            }
            if (dr.ContainsColumn(FieldChild2Birthday))
            {
                Child2Birthday = BaseUtil.ConvertToNullableDateTime(dr[FieldChild2Birthday]);
            }
            if (dr.ContainsColumn(FieldChild3Birthday))
            {
                Child3Birthday = BaseUtil.ConvertToNullableDateTime(dr[FieldChild3Birthday]);
            }
            if (dr.ContainsColumn(FieldChild4Birthday))
            {
                Child4Birthday = BaseUtil.ConvertToNullableDateTime(dr[FieldChild4Birthday]);
            }
            if (dr.ContainsColumn(FieldChild5Birthday))
            {
                Child5Birthday = BaseUtil.ConvertToNullableDateTime(dr[FieldChild5Birthday]);
            }
            if (dr.ContainsColumn(FieldIsDimission))
            {
                IsDimission = BaseUtil.ConvertToInt(dr[FieldIsDimission]);
            }
            if (dr.ContainsColumn(FieldDimissionDate))
            {
                DimissionDate = BaseUtil.ConvertToNullableDateTime(dr[FieldDimissionDate]);
            }
            if (dr.ContainsColumn(FieldDimissionCause))
            {
                DimissionCause = BaseUtil.ConvertToString(dr[FieldDimissionCause]);
            }
            if (dr.ContainsColumn(FieldDimissionWhereabouts))
            {
                DimissionWhereabouts = BaseUtil.ConvertToString(dr[FieldDimissionWhereabouts]);
            }
            if (dr.ContainsColumn(FieldExt1))
            {
                Ext1 = BaseUtil.ConvertToString(dr[FieldExt1]);
            }
            if (dr.ContainsColumn(FieldExt2))
            {
                Ext2 = BaseUtil.ConvertToString(dr[FieldExt2]);
            }
            if (dr.ContainsColumn(FieldExt3))
            {
                Ext3 = BaseUtil.ConvertToString(dr[FieldExt3]);
            }
            if (dr.ContainsColumn(FieldExt4))
            {
                Ext4 = BaseUtil.ConvertToString(dr[FieldExt4]);
            }
            if (dr.ContainsColumn(FieldExt5))
            {
                Ext5 = BaseUtil.ConvertToString(dr[FieldExt5]);
            }
            if (dr.ContainsColumn(FieldDescription))
            {
                Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            }
            return this;
        }

        ///<summary>
        /// 员工
        ///</summary>
        [FieldDescription("员工")]
        public const string CurrentTableName = "BaseStaff";

        ///<summary>
        /// 用户主键
        ///</summary>
        public const string FieldUserId = "UserId";

        ///<summary>
        /// 用户名
        ///</summary>
        public const string FieldUserName = "UserName";

        ///<summary>
        /// 工号
        ///</summary>
        public const string FieldEmployeeNumber = "EmployeeNumber";

        ///<summary>
        /// 姓名
        ///</summary>
        public const string FieldRealName = "RealName";

        ///<summary>
        /// 中文名
        ///</summary>
        public const string FieldChineseName = "ChineseName";

        ///<summary>
        /// 英文名
        ///</summary>
        public const string FieldEnglishName = "EnglishName";

        ///<summary>
        /// 快速查找，记忆符
        ///</summary>
        public const string FieldQuickQuery = "QuickQuery";

        ///<summary>
        /// 公司主键
        ///</summary>
        public const string FieldCompanyId = "CompanyId";

        ///<summary>
        /// 公司名称
        ///</summary>
        public const string FieldCompanyName = "CompanyName";

        ///<summary>
        /// 分支机构主键
        ///</summary>
        public const string FieldSubCompanyId = "SubCompanyId";

        ///<summary>
        /// 子公司名称
        ///</summary>
        public const string FieldSubCompanyName = "SubCompanyName";

        ///<summary>
        /// 部门主键
        ///</summary>
        public const string FieldDepartmentId = "DepartmentId";

        ///<summary>
        /// 部门名称
        ///</summary>
        public const string FieldDepartmentName = "DepartmentName";

        ///<summary>
        /// 工作组主键
        ///</summary>
        public const string FieldWorkgroupId = "WorkgroupId";

        ///<summary>
        /// 工作组名称
        ///</summary>
        public const string FieldWorkgroupName = "WorkgroupName";

        ///<summary>
        /// 职位主键
        ///</summary>
        public const string FieldDutyId = "DutyId";

        ///<summary>
        /// 性别
        ///</summary>
        public const string FieldGender = "Gender";

        ///<summary>
        /// 生日
        ///</summary>
        public const string FieldBirthday = "Birthday";

        ///<summary>
        /// 年龄
        ///</summary>
        public const string FieldAge = "Age";

        ///<summary>
        /// 身高
        ///</summary>
        public const string FieldHeight = "Height";

        ///<summary>
        /// 体重
        ///</summary>
        public const string FieldWeight = "Weight";

        ///<summary>
        /// 唯一身份Id
        ///</summary>
        public const string FieldIdentificationCode = "IdentificationCode";

        ///<summary>
        /// 身份证号码
        ///</summary>
        public const string FieldIdCard = "IdCard";

        ///<summary>
        /// 国籍
        ///</summary>
        public const string FieldNation = "Nation";

        ///<summary>
        /// 最高学历
        ///</summary>
        public const string FieldEducation = "Education";

        ///<summary>
        /// 毕业院校
        ///</summary>
        public const string FieldSchool = "School";

        ///<summary>
        /// 毕业日期
        ///</summary>
        public const string FieldGraduationDate = "GraduationDate";

        ///<summary>
        /// 专业
        ///</summary>
        public const string FieldMajor = "Major";

        ///<summary>
        /// 最高学位
        ///</summary>
        public const string FieldDegree = "Degree";

        ///<summary>
        /// 职称主键
        ///</summary>
        public const string FieldTitleId = "TitleId";

        ///<summary>
        /// 职称评定日期
        ///</summary>
        public const string FieldTitleDate = "TitleDate";

        ///<summary>
        /// 职称等级
        ///</summary>
        public const string FieldTitleLevel = "TitleLevel";

        ///<summary>
        /// 工作时间
        ///</summary>
        public const string FieldWorkingDate = "WorkingDate";

        ///<summary>
        /// 加入本单位时间
        ///</summary>
        public const string FieldJoinInDate = "JoinInDate";

        ///<summary>
        /// 办公邮编
        ///</summary>
        public const string FieldOfficePostCode = "OfficePostCode";

        ///<summary>
        /// 办公地址
        ///</summary>
        public const string FieldOfficeAddress = "OfficeAddress";

        ///<summary>
        /// 办公电话
        ///</summary>
        public const string FieldOfficePhone = "OfficePhone";

        ///<summary>
        /// 办公传真
        ///</summary>
        public const string FieldOfficeFax = "OfficeFax";

        ///<summary>
        /// 家庭住址邮编
        ///</summary>
        public const string FieldHomePostCode = "HomePostCode";

        ///<summary>
        /// 家庭住址
        ///</summary>
        public const string FieldHomeAddress = "HomeAddress";

        ///<summary>
        /// 住宅电话
        ///</summary>
        public const string FieldHomePhone = "HomePhone";

        ///<summary>
        /// 家庭传真
        ///</summary>
        public const string FieldHomeFax = "HomeFax";

        ///<summary>
        /// 第一辆车牌号
        ///</summary>
        public const string FieldPlateNumber1 = "PlateNumber1";

        ///<summary>
        /// 第二辆车牌号
        ///</summary>
        public const string FieldPlateNumber2 = "PlateNumber2";

        ///<summary>
        /// 第三辆车牌号
        ///</summary>
        public const string FieldPlateNumber3 = "PlateNumber3";

        ///<summary>
        /// 奖金卡号
        ///</summary>
        public const string FieldRewardCard = "RewardCard";

        ///<summary>
        /// 医疗卡号
        ///</summary>
        public const string FieldMedicalCard = "MedicalCard";

        ///<summary>
        /// 工会证号
        ///</summary>
        public const string FieldUnionMember = "UnionMember";

        ///<summary>
        /// Email
        ///</summary>
        public const string FieldEmail = "Email";

        ///<summary>
        /// 手机
        ///</summary>
        public const string FieldMobile = "Mobile";

        ///<summary>
        /// QQ
        ///</summary>
        public const string FieldQq = "QQ";

        ///<summary>
        /// 微信
        ///</summary>
        public const string FieldWeChat = "WeChat";

        ///<summary>
        /// 短号
        ///</summary>
        public const string FieldShortNumber = "ShortNumber";

        ///<summary>
        /// 电话
        ///</summary>
        public const string FieldTelephone = "Telephone";

        ///<summary>
        /// 分机
        ///</summary>
        public const string FieldExtension = "Extension";

        ///<summary>
        /// 紧急联系
        ///</summary>
        public const string FieldEmergencyContact = "EmergencyContact";

        ///<summary>
        /// 紧急联系手机
        ///</summary>
        public const string FieldEmergencyMobile = "EmergencyMobile";

        ///<summary>
        /// 紧急联系电话
        ///</summary>
        public const string FieldEmergencyTelephone = "EmergencyTelephone";

        ///<summary>
        /// 籍贯
        ///</summary>
        public const string FieldNativePlace = "NativePlace";

        ///<summary>
        /// 开户行
        ///</summary>
        public const string FieldBankName = "BankName";

        ///<summary>
        /// 银行卡号
        ///</summary>
        public const string FieldBankAccount = "BankAccount";

        ///<summary>
        /// 开户行姓名
        ///</summary>
        public const string FieldBankUserName = "BankUserName";

        ///<summary>
        /// 籍贯省
        ///</summary>
        public const string FieldProvince = "Province";

        ///<summary>
        /// 籍贯市
        ///</summary>
        public const string FieldCity = "City";

        ///<summary>
        /// 籍贯区
        ///</summary>
        public const string FieldDistrict = "District";

        ///<summary>
        /// 当前省
        ///</summary>
        public const string FieldCurrentProvince = "CurrentProvince";

        ///<summary>
        /// 当前市
        ///</summary>
        public const string FieldCurrentCity = "CurrentCity";

        ///<summary>
        /// 当前区
        ///</summary>
        public const string FieldCurrentDistrict = "CurrentDistrict";

        ///<summary>
        /// 政治面貌
        ///</summary>
        public const string FieldParty = "Party";

        ///<summary>
        /// 民族
        ///</summary>
        public const string FieldNationality = "Nationality";

        ///<summary>
        /// 工作性质
        ///</summary>
        public const string FieldWorkingProperty = "WorkingProperty";

        ///<summary>
        /// 职业资格
        ///</summary>
        public const string FieldCompetency = "Competency";

        ///<summary>
        /// 婚姻情况
        ///</summary>
        public const string FieldMarriage = "Marriage";

        ///<summary>
        /// 结婚日期
        ///</summary>
        public const string FieldWeddingDate = "WeddingDate";

        ///<summary>
        /// 离婚日期
        ///</summary>
        public const string FieldDivorceDate = "DivorceDate";

        ///<summary>
        /// 第一个孩子生日
        ///</summary>
        public const string FieldChild1Birthday = "Child1Birthday";

        ///<summary>
        /// 第二个孩子生日
        ///</summary>
        public const string FieldChild2Birthday = "Child2Birthday";

        ///<summary>
        /// 第三个孩子生日
        ///</summary>
        public const string FieldChild3Birthday = "Child3Birthday";

        ///<summary>
        /// 第四个孩子生日
        ///</summary>
        public const string FieldChild4Birthday = "Child4Birthday";

        ///<summary>
        /// 第五个孩子生日
        ///</summary>
        public const string FieldChild5Birthday = "Child5Birthday";

        ///<summary>
        /// 是否离职
        ///</summary>
        public const string FieldIsDimission = "IsDimission";

        ///<summary>
        /// 离职日期
        ///</summary>
        public const string FieldDimissionDate = "DimissionDate";

        ///<summary>
        /// 离职原因
        ///</summary>
        public const string FieldDimissionCause = "DimissionCause";

        ///<summary>
        /// 离职去向
        ///</summary>
        public const string FieldDimissionWhereabouts = "DimissionWhereabouts";

        ///<summary>
        /// 扩展信息1
        ///</summary>
        public const string FieldExt1 = "Ext1";

        ///<summary>
        /// 扩展信息2
        ///</summary>
        public const string FieldExt2 = "Ext2";

        ///<summary>
        /// 扩展信息3
        ///</summary>
        public const string FieldExt3 = "Ext3";

        ///<summary>
        /// 扩展信息4
        ///</summary>
        public const string FieldExt4 = "Ext4";

        ///<summary>
        /// 扩展信息5
        ///</summary>
        public const string FieldExt5 = "Ext5";

        ///<summary>
        /// 描述
        ///</summary>
        public const string FieldDescription = "Description";
    }
}
