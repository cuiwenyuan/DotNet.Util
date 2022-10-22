//-----------------------------------------------------------------------
// <copyright file="BaseUserEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2022, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseUserEntity
    /// 用户账号
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
    [Table(CurrentTableName)]
    public partial class BaseUserEntity : BaseEntity
    {
        /// <summary>
        /// 来源
        /// </summary>
        [FieldDescription("来源")]
        [Description("来源")]
        [Column(FieldUserFrom)]
        public string UserFrom { get; set; } = string.Empty;

        /// <summary>
        /// 用户名
        /// </summary>
        [FieldDescription("用户名")]
        [Description("用户名")]
        [Column(FieldUserName)]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 姓名
        /// </summary>
        [FieldDescription("姓名")]
        [Description("姓名")]
        [Column(FieldRealName)]
        public string RealName { get; set; } = string.Empty;

        /// <summary>
        /// 呢称
        /// </summary>
        [FieldDescription("呢称")]
        [Description("呢称")]
        [Column(FieldNickName)]
        public string NickName { get; set; } = string.Empty;

        /// <summary>
        /// 头像
        /// </summary>
        [FieldDescription("头像")]
        [Description("头像")]
        [Column(FieldAvatarUrl)]
        public string AvatarUrl { get; set; } = string.Empty;

        /// <summary>
        /// 编号
        /// </summary>
        [FieldDescription("编号")]
        [Description("编号")]
        [Column(FieldCode)]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 工号
        /// </summary>
        [FieldDescription("工号")]
        [Description("工号")]
        [Column(FieldEmployeeNumber)]
        public string EmployeeNumber { get; set; } = string.Empty;

        /// <summary>
        /// 身份证号码
        /// </summary>
        [FieldDescription("身份证号码")]
        [Description("身份证号码")]
        [Column(FieldIdCard)]
        public string IdCard { get; set; } = string.Empty;

        /// <summary>
        /// 快速查询
        /// </summary>
        [FieldDescription("快速查询")]
        [Description("快速查询")]
        [Column(FieldQuickQuery)]
        public string QuickQuery { get; set; } = string.Empty;

        /// <summary>
        /// 简拼
        /// </summary>
        [FieldDescription("简拼")]
        [Description("简拼")]
        [Column(FieldSimpleSpelling)]
        public string SimpleSpelling { get; set; } = string.Empty;

        /// <summary>
        /// 公司主键
        /// </summary>
        [FieldDescription("公司主键")]
        [Description("公司主键")]
        [Column(FieldCompanyId)]
        public int CompanyId { get; set; } = 0;

        /// <summary>
        /// 公司编码
        /// </summary>
        [FieldDescription("公司编码")]
        [Description("公司编码")]
        [Column(FieldCompanyCode)]
        public string CompanyCode { get; set; } = string.Empty;

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
        /// 分支机构名称
        /// </summary>
        [FieldDescription("分支机构名称")]
        [Description("分支机构名称")]
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
        /// 子部门主键
        /// </summary>
        [FieldDescription("子部门主键")]
        [Description("子部门主键")]
        [Column(FieldSubDepartmentId)]
        public int SubDepartmentId { get; set; } = 0;

        /// <summary>
        /// 子部门名称
        /// </summary>
        [FieldDescription("子部门名称")]
        [Description("子部门名称")]
        [Column(FieldSubDepartmentName)]
        public string SubDepartmentName { get; set; } = string.Empty;

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
        /// 工作行业
        /// </summary>
        [FieldDescription("工作行业")]
        [Description("工作行业")]
        [Column(FieldWorkCategory)]
        public string WorkCategory { get; set; } = string.Empty;

        /// <summary>
        /// 安全级别
        /// </summary>
        [FieldDescription("安全级别")]
        [Description("安全级别")]
        [Column(FieldSecurityLevel)]
        public int SecurityLevel { get; set; } = 0;

        /// <summary>
        /// 职称、职位
        /// </summary>
        [FieldDescription("职称、职位")]
        [Description("职称、职位")]
        [Column(FieldTitle)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 岗位
        /// </summary>
        [FieldDescription("岗位")]
        [Description("岗位")]
        [Column(FieldDuty)]
        public string Duty { get; set; } = string.Empty;

        /// <summary>
        /// 语言
        /// </summary>
        [FieldDescription("语言")]
        [Description("语言")]
        [Column(FieldLang)]
        public string Lang { get; set; } = "CN";

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
        /// 积分
        /// </summary>
        [FieldDescription("积分")]
        [Description("积分")]
        [Column(FieldScore)]
        public int? Score { get; set; } = 0;

        /// <summary>
        /// 粉丝数量
        /// </summary>
        [FieldDescription("粉丝数量")]
        [Description("粉丝数量")]
        [Column(FieldFans)]
        public int? Fans { get; set; } = 0;

        /// <summary>
        /// 家庭住址
        /// </summary>
        [FieldDescription("家庭住址")]
        [Description("家庭住址")]
        [Column(FieldHomeAddress)]
        public string HomeAddress { get; set; } = string.Empty;

        /// <summary>
        /// 个性签名
        /// </summary>
        [FieldDescription("个性签名")]
        [Description("个性签名")]
        [Column(FieldSignature)]
        public string Signature { get; set; } = string.Empty;

        /// <summary>
        /// 系统样式选择
        /// </summary>
        [FieldDescription("系统样式选择")]
        [Description("系统样式选择")]
        [Column(FieldTheme)]
        public string Theme { get; set; } = string.Empty;

        /// <summary>
        /// 是否员工
        /// </summary>
        [FieldDescription("是否员工")]
        [Description("是否员工")]
        [Column(FieldIsStaff)]
        public int IsStaff { get; set; } = 0;

        /// <summary>
        /// 是否显示
        /// </summary>
        [FieldDescription("是否显示")]
        [Description("是否显示")]
        [Column(FieldIsVisible)]
        public int IsVisible { get; set; } = 1;

        /// <summary>
        /// 国家
        /// </summary>
        [FieldDescription("国家")]
        [Description("国家")]
        [Column(FieldCountry)]
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// 州
        /// </summary>
        [FieldDescription("州")]
        [Description("州")]
        [Column(FieldState)]
        public string State { get; set; } = string.Empty;

        /// <summary>
        /// 省份
        /// </summary>
        [FieldDescription("省份")]
        [Description("省份")]
        [Column(FieldProvince)]
        public string Province { get; set; } = string.Empty;

        /// <summary>
        /// 城市
        /// </summary>
        [FieldDescription("城市")]
        [Description("城市")]
        [Column(FieldCity)]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// 区域
        /// </summary>
        [FieldDescription("区域")]
        [Description("区域")]
        [Column(FieldDistrict)]
        public string District { get; set; } = string.Empty;

        /// <summary>
        /// 审核状态
        /// </summary>
        [FieldDescription("审核状态")]
        [Description("审核状态")]
        [Column(FieldAuditStatus)]
        public string AuditStatus { get; set; } = string.Empty;

        /// <summary>
        /// 经理用户编号
        /// </summary>
        [FieldDescription("经理用户编号")]
        [Description("经理用户编号")]
        [Column(FieldManagerUserId)]
        public int ManagerUserId { get; set; } = 0;

        /// <summary>
        /// 是否系统管理员
        /// </summary>
        [FieldDescription("是否系统管理员")]
        [Description("是否系统管理员")]
        [Column(FieldIsAdministrator)]
        public int IsAdministrator { get; set; } = 0;

        /// <summary>
        /// 是否检查余额
        /// </summary>
        [FieldDescription("是否检查余额")]
        [Description("是否检查余额")]
        [Column(FieldIsCheckBalance)]
        public int IsCheckBalance { get; set; } = 0;

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
            if (dr.ContainsColumn(FieldUserFrom))
            {
                UserFrom = BaseUtil.ConvertToString(dr[FieldUserFrom]);
            }
            if (dr.ContainsColumn(FieldUserName))
            {
                UserName = BaseUtil.ConvertToString(dr[FieldUserName]);
            }
            if (dr.ContainsColumn(FieldRealName))
            {
                RealName = BaseUtil.ConvertToString(dr[FieldRealName]);
            }
            if (dr.ContainsColumn(FieldNickName))
            {
                NickName = BaseUtil.ConvertToString(dr[FieldNickName]);
            }
            if (dr.ContainsColumn(FieldAvatarUrl))
            {
                AvatarUrl = BaseUtil.ConvertToString(dr[FieldAvatarUrl]);
            }
            if (dr.ContainsColumn(FieldCode))
            {
                Code = BaseUtil.ConvertToString(dr[FieldCode]);
            }
            if (dr.ContainsColumn(FieldEmployeeNumber))
            {
                EmployeeNumber = BaseUtil.ConvertToString(dr[FieldEmployeeNumber]);
            }
            if (dr.ContainsColumn(FieldIdCard))
            {
                IdCard = BaseUtil.ConvertToString(dr[FieldIdCard]);
            }
            if (dr.ContainsColumn(FieldQuickQuery))
            {
                QuickQuery = BaseUtil.ConvertToString(dr[FieldQuickQuery]);
            }
            if (dr.ContainsColumn(FieldSimpleSpelling))
            {
                SimpleSpelling = BaseUtil.ConvertToString(dr[FieldSimpleSpelling]);
            }
            if (dr.ContainsColumn(FieldCompanyId))
            {
                CompanyId = BaseUtil.ConvertToInt(dr[FieldCompanyId]);
            }
            if (dr.ContainsColumn(FieldCompanyCode))
            {
                CompanyCode = BaseUtil.ConvertToString(dr[FieldCompanyCode]);
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
            if (dr.ContainsColumn(FieldSubDepartmentId))
            {
                SubDepartmentId = BaseUtil.ConvertToInt(dr[FieldSubDepartmentId]);
            }
            if (dr.ContainsColumn(FieldSubDepartmentName))
            {
                SubDepartmentName = BaseUtil.ConvertToString(dr[FieldSubDepartmentName]);
            }
            if (dr.ContainsColumn(FieldWorkgroupId))
            {
                WorkgroupId = BaseUtil.ConvertToInt(dr[FieldWorkgroupId]);
            }
            if (dr.ContainsColumn(FieldWorkgroupName))
            {
                WorkgroupName = BaseUtil.ConvertToString(dr[FieldWorkgroupName]);
            }
            if (dr.ContainsColumn(FieldWorkCategory))
            {
                WorkCategory = BaseUtil.ConvertToString(dr[FieldWorkCategory]);
            }
            if (dr.ContainsColumn(FieldSecurityLevel))
            {
                SecurityLevel = BaseUtil.ConvertToInt(dr[FieldSecurityLevel]);
            }
            if (dr.ContainsColumn(FieldTitle))
            {
                Title = BaseUtil.ConvertToString(dr[FieldTitle]);
            }
            if (dr.ContainsColumn(FieldDuty))
            {
                Duty = BaseUtil.ConvertToString(dr[FieldDuty]);
            }
            if (dr.ContainsColumn(FieldLang))
            {
                Lang = BaseUtil.ConvertToString(dr[FieldLang]);
            }
            if (dr.ContainsColumn(FieldGender))
            {
                Gender = BaseUtil.ConvertToString(dr[FieldGender]);
            }
            if (dr.ContainsColumn(FieldBirthday))
            {
                Birthday = BaseUtil.ConvertToNullableDateTime(dr[FieldBirthday]);
            }
            if (dr.ContainsColumn(FieldScore))
            {
                Score = BaseUtil.ConvertToNullableInt(dr[FieldScore]);
            }
            if (dr.ContainsColumn(FieldFans))
            {
                Fans = BaseUtil.ConvertToNullableInt(dr[FieldFans]);
            }
            if (dr.ContainsColumn(FieldHomeAddress))
            {
                HomeAddress = BaseUtil.ConvertToString(dr[FieldHomeAddress]);
            }
            if (dr.ContainsColumn(FieldSignature))
            {
                Signature = BaseUtil.ConvertToString(dr[FieldSignature]);
            }
            if (dr.ContainsColumn(FieldTheme))
            {
                Theme = BaseUtil.ConvertToString(dr[FieldTheme]);
            }
            if (dr.ContainsColumn(FieldIsStaff))
            {
                IsStaff = BaseUtil.ConvertToInt(dr[FieldIsStaff]);
            }
            if (dr.ContainsColumn(FieldIsVisible))
            {
                IsVisible = BaseUtil.ConvertToInt(dr[FieldIsVisible]);
            }
            if (dr.ContainsColumn(FieldCountry))
            {
                Country = BaseUtil.ConvertToString(dr[FieldCountry]);
            }
            if (dr.ContainsColumn(FieldState))
            {
                State = BaseUtil.ConvertToString(dr[FieldState]);
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
            if (dr.ContainsColumn(FieldAuditStatus))
            {
                AuditStatus = BaseUtil.ConvertToString(dr[FieldAuditStatus]);
            }
            if (dr.ContainsColumn(FieldManagerUserId))
            {
                ManagerUserId = BaseUtil.ConvertToInt(dr[FieldManagerUserId]);
            }
            if (dr.ContainsColumn(FieldIsAdministrator))
            {
                IsAdministrator = BaseUtil.ConvertToInt(dr[FieldIsAdministrator]);
            }
            if (dr.ContainsColumn(FieldIsCheckBalance))
            {
                IsCheckBalance = BaseUtil.ConvertToInt(dr[FieldIsCheckBalance]);
            }
            if (dr.ContainsColumn(FieldDescription))
            {
                Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            }
            return this;
        }

        ///<summary>
        /// 用户账号
        ///</summary>
        [FieldDescription("用户账号")]
        public const string CurrentTableName = "BaseUser";

        ///<summary>
        /// 来源
        ///</summary>
        public const string FieldUserFrom = "UserFrom";

        ///<summary>
        /// 用户名
        ///</summary>
        public const string FieldUserName = "UserName";

        ///<summary>
        /// 姓名
        ///</summary>
        public const string FieldRealName = "RealName";

        ///<summary>
        /// 呢称
        ///</summary>
        public const string FieldNickName = "NickName";

        ///<summary>
        /// 头像
        ///</summary>
        public const string FieldAvatarUrl = "AvatarUrl";

        ///<summary>
        /// 编号
        ///</summary>
        public const string FieldCode = "Code";

        ///<summary>
        /// 工号
        ///</summary>
        public const string FieldEmployeeNumber = "EmployeeNumber";

        ///<summary>
        /// 身份证号码
        ///</summary>
        public const string FieldIdCard = "IdCard";

        ///<summary>
        /// 快速查询
        ///</summary>
        public const string FieldQuickQuery = "QuickQuery";

        ///<summary>
        /// 简拼
        ///</summary>
        public const string FieldSimpleSpelling = "SimpleSpelling";

        ///<summary>
        /// 公司主键
        ///</summary>
        public const string FieldCompanyId = "CompanyId";

        ///<summary>
        /// 公司编码
        ///</summary>
        public const string FieldCompanyCode = "CompanyCode";

        ///<summary>
        /// 公司名称
        ///</summary>
        public const string FieldCompanyName = "CompanyName";

        ///<summary>
        /// 分支机构主键
        ///</summary>
        public const string FieldSubCompanyId = "SubCompanyId";

        ///<summary>
        /// 分支机构名称
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
        /// 子部门主键
        ///</summary>
        public const string FieldSubDepartmentId = "SubDepartmentId";

        ///<summary>
        /// 子部门名称
        ///</summary>
        public const string FieldSubDepartmentName = "SubDepartmentName";

        ///<summary>
        /// 工作组主键
        ///</summary>
        public const string FieldWorkgroupId = "WorkgroupId";

        ///<summary>
        /// 工作组名称
        ///</summary>
        public const string FieldWorkgroupName = "WorkgroupName";

        ///<summary>
        /// 工作行业
        ///</summary>
        public const string FieldWorkCategory = "WorkCategory";

        ///<summary>
        /// 安全级别
        ///</summary>
        public const string FieldSecurityLevel = "SecurityLevel";

        ///<summary>
        /// 职称、职位
        ///</summary>
        public const string FieldTitle = "Title";

        ///<summary>
        /// 岗位
        ///</summary>
        public const string FieldDuty = "Duty";

        ///<summary>
        /// 语言
        ///</summary>
        public const string FieldLang = "Lang";

        ///<summary>
        /// 性别
        ///</summary>
        public const string FieldGender = "Gender";

        ///<summary>
        /// 生日
        ///</summary>
        public const string FieldBirthday = "Birthday";

        ///<summary>
        /// 积分
        ///</summary>
        public const string FieldScore = "Score";

        ///<summary>
        /// 粉丝数量
        ///</summary>
        public const string FieldFans = "Fans";

        ///<summary>
        /// 家庭住址
        ///</summary>
        public const string FieldHomeAddress = "HomeAddress";

        ///<summary>
        /// 个性签名
        ///</summary>
        public const string FieldSignature = "Signature";

        ///<summary>
        /// 系统样式选择
        ///</summary>
        public const string FieldTheme = "Theme";

        ///<summary>
        /// 是否员工
        ///</summary>
        public const string FieldIsStaff = "IsStaff";

        ///<summary>
        /// 是否显示
        ///</summary>
        public const string FieldIsVisible = "IsVisible";

        ///<summary>
        /// 国家
        ///</summary>
        public const string FieldCountry = "Country";

        ///<summary>
        /// 州
        ///</summary>
        public const string FieldState = "State";

        ///<summary>
        /// 省份
        ///</summary>
        public const string FieldProvince = "Province";

        ///<summary>
        /// 城市
        ///</summary>
        public const string FieldCity = "City";

        ///<summary>
        /// 区域
        ///</summary>
        public const string FieldDistrict = "District";

        ///<summary>
        /// 审核状态
        ///</summary>
        public const string FieldAuditStatus = "AuditStatus";

        ///<summary>
        /// 经理用户编号
        ///</summary>
        public const string FieldManagerUserId = "ManagerUserId";

        ///<summary>
        /// 是否系统管理员
        ///</summary>
        public const string FieldIsAdministrator = "IsAdministrator";

        ///<summary>
        /// 是否检查余额
        ///</summary>
        public const string FieldIsCheckBalance = "IsCheckBalance";

        ///<summary>
        /// 描述
        ///</summary>
        public const string FieldDescription = "Description";
    }
}