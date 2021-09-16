//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseUserEntity
    /// 系统用户表
    ///
    /// 修改记录
    ///
    ///		2010-08-07 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2010-08-07</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseUserEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [FieldDescription("主键", false)]
        public string Id { get; set; } = null;

        /// <summary>
        /// 用户来源
        /// </summary>
        [FieldDescription("用户来源")]
        public string UserFrom { get; set; } = string.Empty;

        /// <summary>
        /// 用户密码
        /// </summary>
        [FieldDescription("用户密码")]
        public string UserPassword { get; set; } = null;

        /// <summary>
        /// 编号
        /// </summary>
        [FieldDescription("编号")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 登录名
        /// </summary>
        [FieldDescription("登录名")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 姓名
        /// </summary>
        [FieldDescription("姓名")]
        public string RealName { get; set; } = string.Empty;

        /// <summary>
        /// 唯一用户名
        /// </summary>
        [FieldDescription("唯一用户名")]
        public string NickName { get; set; } = string.Empty;

        /// <summary>
        /// 快速查询，全拼
        /// </summary>
        [FieldDescription("快速查询，全拼")]
        public string QuickQuery { get; set; } = string.Empty;

        /// <summary>
        /// 快速查询，简拼
        /// </summary>
        [FieldDescription("快速查询，简拼")]
        public string SimpleSpelling { get; set; } = string.Empty;

        /// <summary>
        /// 安全级别
        /// </summary>
        [FieldDescription("安全级别")]
        public int? SecurityLevel { get; set; } = null;

        /// <summary>
        /// 工作行业
        /// </summary>
        [FieldDescription("工作行业")]
        public string WorkCategory { get; set; } = string.Empty;

        /// <summary>
        /// 公司主键
        /// </summary>
        [FieldDescription("公司主键")]
        public string CompanyId { get; set; } = null;

        /// <summary>
        /// 公司编号
        /// </summary>
        [FieldDescription("公司编号")]
        public string CompanyCode { get; set; } = string.Empty;

        /// <summary>
        /// 公司名称
        /// </summary>
        [FieldDescription("公司名称")]
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// 分支机构主键
        /// </summary>
        [FieldDescription("分支机构主键")]
        public string SubCompanyId { get; set; } = null;

        /// <summary>
        /// 分支机构名称
        /// </summary>
        [FieldDescription("分支机构名称")]
        public string SubCompanyName { get; set; } = string.Empty;

        /// <summary>
        /// 部门主键
        /// </summary>
        [FieldDescription("部门主键")]
        public string DepartmentId { get; set; } = null;

        /// <summary>
        /// 部门名称
        /// </summary>
        [FieldDescription("部门名称")]
        public string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// 子部门主键
        /// </summary>
        [FieldDescription("子部门主键")]
        public string SubDepartmentId { get; set; } = null;

        /// <summary>
        /// 子部门名称
        /// </summary>
        [FieldDescription("子部门名称")]
        public string SubDepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// 工作组代码
        /// </summary>
        [FieldDescription("工作组代码")]
        public string WorkgroupId { get; set; } = null;

        /// <summary>
        /// 工作组名称
        /// </summary>
        [FieldDescription("工作组名称")]
        public string WorkgroupName { get; set; } = string.Empty;

        /// <summary>
        /// 性别
        /// </summary>
        [FieldDescription("性别")]
        public string Gender { get; set; } = string.Empty;

        /// <summary>
        /// 出生日期
        /// </summary>
        [FieldDescription("出生日期")]
        public string Birthday { get; set; } = string.Empty;

        /// <summary>
        /// 身份证号码
        /// </summary>
        [FieldDescription("身份证号码")]
        public string IdCard { get; set; } = string.Empty;

        /// <summary>
        /// 岗位
        /// </summary>
        [FieldDescription("岗位")]
        public string Duty { get; set; } = null;

        /// <summary>
        /// 职称
        /// </summary>
        [FieldDescription("职称")]
        public string Title { get; set; } = null;

        /// <summary>
        /// 省
        /// </summary>
        [FieldDescription("省")]
        public string Province { get; set; } = null;

        /// <summary>
        /// 市
        /// </summary>
        [FieldDescription("市")]
        public string City { get; set; } = null;

        /// <summary>
        /// 县
        /// </summary>
        [FieldDescription("县")]
        public string District { get; set; } = null;

        /// <summary>
        /// 家庭住址（单位）
        /// </summary>
        [FieldDescription("家庭住址（单位）")]
        public string HomeAddress { get; set; } = null;

        /// <summary>
        /// 用户积分
        /// </summary>
        [FieldDescription("用户积分")]
        public int? Score { get; set; } = 0;

        /// <summary>
        /// 系统语言选择
        /// </summary>
        [FieldDescription("系统语言选择")]
        public string Lang { get; set; } = null;

        /// <summary>
        /// 系统样式选择
        /// </summary>
        [FieldDescription("系统样式选择")]
        public string Theme { get; set; } = null;

        /// <summary>
        /// 是否员工
        /// </summary>
        [FieldDescription("是否员工")]
        public int? IsStaff { get; set; } = 0;

        /// <summary>
        /// 审核状态
        /// </summary>
        [FieldDescription("审核状态")]
        public string AuditStatus { get; set; } = null;

        /// <summary>
        /// 上级主管审核主键
        /// </summary>
        [FieldDescription("上级主管审核主键")]
        public string ManagerId { get; set; } = null;

        /// <summary>
        /// 上级主管审核状态
        /// </summary>
        [FieldDescription("上级主管审核状态")]
        public string ManagerAuditStatus { get; set; } = null;

        /// <summary>
        /// 上级主管审核日期
        /// </summary>
        [FieldDescription("上级主管审核日期")]
        public DateTime? ManagerAuditDate { get; set; } = null;

        /// <summary>
        /// 是否显示
        /// </summary>
        [FieldDescription("是否显示")]
        public int? IsVisible { get; set; } = 1;

        /// <summary>
        /// 是否删除
        /// </summary>
        [FieldDescription("是否删除")]
        public int? DeletionStateCode { get; set; } = 0;

        /// <summary>
        /// 有效
        /// </summary>
        [FieldDescription("是否有效")]
        public int? Enabled { get; set; } = 1;

        /// <summary>
        /// 管理员
        /// </summary>
        [FieldDescription("管理员")]
        public bool IsAdministrator { get; set; } = false;

        /// <summary>
        /// 排序码
        /// </summary>
        [FieldDescription("排序码", false)]
        public int? SortCode { get; set; } = 0;

        /// <summary>
        /// 备注
        /// </summary>
        [FieldDescription("备注")]
        public string Description { get; set; } = null;

        /// <summary>
        /// 个性签名
        /// </summary>
        [FieldDescription("个性签名")]
        public string Signature { get; set; } = null;

        /// <summary>
        /// 创建时间
        /// </summary>
        [FieldDescription("创建时间", false)]
        public DateTime? CreateOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人用户编号
        /// </summary>
        [FieldDescription("创建人用户编号", false)]
        public string CreateUserId { get; set; } = null;

        /// <summary>
        /// 创建人
        /// </summary>
        [FieldDescription("创建人", false)]
        public string CreateBy { get; set; } = null;

        /// <summary>
        /// 修改时间
        /// </summary>
        [FieldDescription("修改时间", false)]
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改人用户编号
        /// </summary>
        [FieldDescription("修改人用户编号", false)]
        public string ModifiedUserId { get; set; } = null;

        /// <summary>
        /// 修改人
        /// </summary>
        [FieldDescription("修改人", false)]
        public string ModifiedBy { get; set; } = null;

        ///<summary>
        /// 扫描检测余额
        ///</summary>
        [FieldDescription("扫描检测余额")]
        public int? IsCheckBalance { get; set; } = 0;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            Id = BaseUtil.ConvertToString(dr[FieldId]);
            UserFrom = BaseUtil.ConvertToString(dr[FieldUserFrom]);
            Code = BaseUtil.ConvertToString(dr[FieldCode]);
            UserName = BaseUtil.ConvertToString(dr[FieldUserName]);
            NickName = BaseUtil.ConvertToString(dr[FieldNickName]);
            RealName = BaseUtil.ConvertToString(dr[FieldRealName]);
            QuickQuery = BaseUtil.ConvertToString(dr[FieldQuickQuery]);
            SimpleSpelling = BaseUtil.ConvertToString(dr[FieldSimpleSpelling]);
            SecurityLevel = BaseUtil.ConvertToInt(dr[FieldSecurityLevel]);
            WorkCategory = BaseUtil.ConvertToString(dr[FieldWorkCategory]);
            CompanyId = BaseUtil.ConvertToString(dr[FieldCompanyId]);
            // 2015-12-29 吉日嘎拉 防止程序出错，没有这个字段也可以正常运行
            if (dr.ContainsColumn(FieldCompanyCode))
            {
                CompanyCode = BaseUtil.ConvertToString(dr[FieldCompanyCode]);
            }
            CompanyName = BaseUtil.ConvertToString(dr[FieldCompanyName]);
            SubCompanyId = BaseUtil.ConvertToString(dr[FieldSubCompanyId]);
            SubCompanyName = BaseUtil.ConvertToString(dr[FieldSubCompanyName]);
            DepartmentId = BaseUtil.ConvertToString(dr[FieldDepartmentId]);
            DepartmentName = BaseUtil.ConvertToString(dr[FieldDepartmentName]);
            SubDepartmentId = BaseUtil.ConvertToString(dr[FieldSubDepartmentId]);
            SubDepartmentName = BaseUtil.ConvertToString(dr[FieldSubDepartmentName]);
            WorkgroupId = BaseUtil.ConvertToString(dr[FieldWorkgroupId]);
            WorkgroupName = BaseUtil.ConvertToString(dr[FieldWorkgroupName]);
            IdCard = BaseUtil.ConvertToString(dr[FieldIdCard]);
            Gender = BaseUtil.ConvertToString(dr[FieldGender]);
            Birthday = BaseUtil.ConvertToString(dr[FieldBirthday]);
            Duty = BaseUtil.ConvertToString(dr[FieldDuty]);
            Title = BaseUtil.ConvertToString(dr[FieldTitle]);
            Province = BaseUtil.ConvertToString(dr[FieldProvince]);
            City = BaseUtil.ConvertToString(dr[FieldCity]);
            District = BaseUtil.ConvertToString(dr[FieldDistrict]);
            HomeAddress = BaseUtil.ConvertToString(dr[FieldHomeAddress]);
            Score = BaseUtil.ConvertToInt(dr[FieldScore]);
            IsAdministrator = BaseUtil.ConvertToBoolean(dr[FieldIsAdministrator]);
            Lang = BaseUtil.ConvertToString(dr[FieldLang]);
            Theme = BaseUtil.ConvertToString(dr[FieldTheme]);
            Signature = BaseUtil.ConvertToString(dr[FieldSignature]);
            IsStaff = BaseUtil.ConvertToInt(dr[FieldIsStaff]);
            IsCheckBalance = BaseUtil.ConvertToInt(dr[FieldIsCheckBalance]);
            AuditStatus = BaseUtil.ConvertToString(dr[FieldAuditStatus]);
            ManagerId = BaseUtil.ConvertToString(dr[FieldManagerId]);
            ManagerAuditStatus = BaseUtil.ConvertToString(dr[FieldManagerAuditStatus]);
            ManagerAuditDate = BaseUtil.ConvertToNullableDateTime(dr[FieldManagerAuditDate]);
            IsVisible = BaseUtil.ConvertToInt(dr[FieldIsVisible]);
            DeletionStateCode = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            SortCode = BaseUtil.ConvertToInt(dr[FieldSortCode]);
            Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            CreateOn = BaseUtil.ConvertToNullableDateTime(dr[FieldCreateTime]);
            CreateUserId = BaseUtil.ConvertToString(dr[FieldCreateUserId]);
            CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            ModifiedOn = BaseUtil.ConvertToNullableDateTime(dr[FieldUpdateTime]);
            ModifiedUserId = BaseUtil.ConvertToString(dr[FieldUpdateUserId]);
            ModifiedBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            // 获取扩展属性
            GetFromExtend(dr);
            return this;
        }

        ///<summary>
        /// 系统用户表
        ///</summary>
        [NonSerialized]
        [FieldDescription("系统用户表")]
        public const string TableName = "BaseUser";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 用户来源
        ///</summary>
        [NonSerialized]
        public const string FieldUserFrom = "UserFrom";

        /// <summary>
        /// 主管主键
        /// </summary>
        [NonSerialized]
        public const string FieldManagerId = "ManagerId";

        /// <summary>
        /// 上级主管审核状态
        /// </summary>
        [NonSerialized]
        public const string FieldManagerAuditStatus = "ManagerAuditStatus";

        /// <summary>
        /// 上级主管审核日期
        /// </summary>
        [NonSerialized]
        public const string FieldManagerAuditDate = "ManagerAuditDate";

        ///<summary>
        /// 编号
        ///</summary>
        [NonSerialized]
        public const string FieldCode = "Code";

        ///<summary>
        /// 登录名
        ///</summary>
        [NonSerialized]
        public const string FieldUserName = "UserName";

        ///<summary>
        /// 姓名
        ///</summary>
        [NonSerialized]
        public const string FieldRealName = "RealName";

        ///<summary>
        /// 呢称
        ///</summary>
        [NonSerialized]
        public const string FieldNickName = "Nickname";

        ///<summary>
        /// 身份证号码
        ///</summary>
        [NonSerialized]
        public const string FieldIdCard = "IDCard";

        ///<summary>
        /// 快速查询，全拼
        ///</summary>
        [NonSerialized]
        public const string FieldQuickQuery = "QuickQuery";

        ///<summary>
        /// 快速查询，简拼
        ///</summary>
        [NonSerialized]
        public const string FieldSimpleSpelling = "SimpleSpelling";

        ///<summary>
        /// 个性签名
        ///</summary>
        [NonSerialized]
        public const string FieldSignature = "Signature";

        ///<summary>
        /// 安全级别
        ///</summary>
        [NonSerialized]
        public const string FieldSecurityLevel = "SecurityLevel";

        ///<summary>
        /// 工作行业
        ///</summary>
        [NonSerialized]
        public const string FieldWorkCategory = "WorkCategory";

        ///<summary>
        /// 公司主键
        ///</summary>
        [NonSerialized]
        public const string FieldCompanyId = "CompanyId";

        ///<summary>
        /// 公司编号
        ///</summary>
        [NonSerialized]
        public const string FieldCompanyCode = "CompanyCode";

        ///<summary>
        /// 公司名称
        ///</summary>
        [NonSerialized]
        public const string FieldCompanyName = "CompanyName";

        ///<summary>
        /// 分支机构主键
        ///</summary>
        [NonSerialized]
        public const string FieldSubCompanyId = "SubCompanyId";

        ///<summary>
        /// 分支机构名称
        ///</summary>
        [NonSerialized]
        public const string FieldSubCompanyName = "SubCompanyName";

        ///<summary>
        /// 部门主键
        ///</summary>
        [NonSerialized]
        public const string FieldDepartmentId = "DepartmentId";

        ///<summary>
        /// 部门名称
        ///</summary>
        [NonSerialized]
        public const string FieldDepartmentName = "DepartmentName";

        ///<summary>
        /// 子部门主键
        ///</summary>
        [NonSerialized]
        public const string FieldSubDepartmentId = "SubDepartmentId";

        ///<summary>
        /// 子部门名称
        ///</summary>
        [NonSerialized]
        public const string FieldSubDepartmentName = "SubDepartmentName";

        ///<summary>
        /// 工作组主键
        ///</summary>
        [NonSerialized]
        public const string FieldWorkgroupId = "WorkgroupId";

        ///<summary>
        /// 工作组名称
        ///</summary>
        [NonSerialized]
        public const string FieldWorkgroupName = "WorkgroupName";

        ///<summary>
        /// 性别
        ///</summary>
        [NonSerialized]
        public const string FieldGender = "Gender";

        ///<summary>
        /// 出生日期
        ///</summary>
        [NonSerialized]
        public const string FieldBirthday = "Birthday";

        ///<summary>
        /// 岗位
        ///</summary>
        [NonSerialized]
        public const string FieldDuty = "Duty";

        ///<summary>
        /// 职称
        ///</summary>
        [NonSerialized]
        public const string FieldTitle = "Title";

        ///<summary>
        /// 省
        ///</summary>
        [NonSerialized]
        public const string FieldProvince = "Province";

        ///<summary>
        /// 市
        ///</summary>
        [NonSerialized]
        public const string FieldCity = "City";

        ///<summary>
        /// 县
        ///</summary>
        [NonSerialized]
        public const string FieldDistrict = "District";

        ///<summary>
        /// 家庭住址
        ///</summary>
        [NonSerialized]
        public const string FieldHomeAddress = "HomeAddress";

        ///<summary>
        /// 用户积分
        ///</summary>
        [NonSerialized]
        public const string FieldScore = "Score";

        ///<summary>
        /// 系统语言选择
        ///</summary>
        [NonSerialized]
        public const string FieldLang = "Lang";

        ///<summary>
        /// 系统样式选择
        ///</summary>
        [NonSerialized]
        public const string FieldTheme = "Theme";

        ///<summary>
        /// 是否员工
        ///</summary>
        [NonSerialized]
        public const string FieldIsStaff = "IsStaff";

        ///<summary>
        /// 扫描检测余额
        ///</summary>
        [NonSerialized]
        public const string FieldIsCheckBalance = "IsCheckBalance";

        ///<summary>
        /// 审核状态
        ///</summary>
        [NonSerialized]
        public const string FieldAuditStatus = "AuditStatus";

        ///<summary>
        /// 是否显示
        ///</summary>
        [NonSerialized]
        public const string FieldIsVisible = "IsVisible";

        ///<summary>
        /// 是否删除
        ///</summary>
        [NonSerialized]
        public const string FieldDeleted = "DeletionStateCode";

        ///<summary>
        /// 有效
        ///</summary>
        [NonSerialized]
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 是否超级管理员
        ///</summary>
        [NonSerialized]
        public const string FieldIsAdministrator = "IsAdministrator";

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
    }
}