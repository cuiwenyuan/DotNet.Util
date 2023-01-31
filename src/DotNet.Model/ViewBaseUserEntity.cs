//-----------------------------------------------------------------------
// <copyright file="BaseUserEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2023, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// ViewBaseUserEntity
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
    public partial class ViewBaseUserEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [FieldDescription("主键")]
        public int Id { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [FieldDescription("来源")]
        public string UserFrom { get; set; } = string.Empty;

        /// <summary>
        /// 用户名
        /// </summary>
        [FieldDescription("用户名")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 姓名
        /// </summary>
        [FieldDescription("姓名")]
        public string RealName { get; set; } = string.Empty;

        /// <summary>
        /// 呢称
        /// </summary>
        [FieldDescription("呢称")]
        public string NickName { get; set; } = string.Empty;

        /// <summary>
        /// 头像
        /// </summary>
        [FieldDescription("头像")]
        public string AvatarUrl { get; set; } = string.Empty;

        /// <summary>
        /// 编号
        /// </summary>
        [FieldDescription("编号")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 工号
        /// </summary>
        [FieldDescription("工号")]
        public string EmployeeNumber { get; set; } = string.Empty;

        /// <summary>
        /// 身份证号码
        /// </summary>
        [FieldDescription("身份证号码")]
        public string IdCard { get; set; } = string.Empty;

        /// <summary>
        /// 快速查询
        /// </summary>
        [FieldDescription("快速查询")]
        public string QuickQuery { get; set; } = string.Empty;

        /// <summary>
        /// 简拼
        /// </summary>
        [FieldDescription("简拼")]
        public string SimpleSpelling { get; set; } = string.Empty;

        /// <summary>
        /// 公司主键
        /// </summary>
        [FieldDescription("公司主键")]
        public int CompanyId { get; set; } = 0;

        /// <summary>
        /// 公司编码
        /// </summary>
        [FieldDescription("公司编码")]
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
        public int SubCompanyId { get; set; } = 0;

        /// <summary>
        /// 分支机构名称
        /// </summary>
        [FieldDescription("分支机构名称")]
        public string SubCompanyName { get; set; } = string.Empty;

        /// <summary>
        /// 部门主键
        /// </summary>
        [FieldDescription("部门主键")]
        public int DepartmentId { get; set; } = 0;

        /// <summary>
        /// 部门名称
        /// </summary>
        [FieldDescription("部门名称")]
        public string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// 子部门主键
        /// </summary>
        [FieldDescription("子部门主键")]
        public int SubDepartmentId { get; set; } = 0;

        /// <summary>
        /// 子部门名称
        /// </summary>
        [FieldDescription("子部门名称")]
        public string SubDepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// 工作组主键
        /// </summary>
        [FieldDescription("工作组主键")]
        public int WorkgroupId { get; set; } = 0;

        /// <summary>
        /// 工作组名称
        /// </summary>
        [FieldDescription("工作组名称")]
        public string WorkgroupName { get; set; } = string.Empty;

        /// <summary>
        /// 工作行业
        /// </summary>
        [FieldDescription("工作行业")]
        public string WorkCategory { get; set; } = string.Empty;

        /// <summary>
        /// 安全级别
        /// </summary>
        [FieldDescription("安全级别")]
        public int SecurityLevel { get; set; } = 0;

        /// <summary>
        /// 职称、职位
        /// </summary>
        [FieldDescription("职称、职位")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 岗位
        /// </summary>
        [FieldDescription("岗位")]
        public string Duty { get; set; } = string.Empty;

        /// <summary>
        /// 语言
        /// </summary>
        [FieldDescription("语言")]
        public string Lang { get; set; } = "CN";

        /// <summary>
        /// 性别
        /// </summary>
        [FieldDescription("性别")]
        public string Gender { get; set; } = string.Empty;

        /// <summary>
        /// 生日
        /// </summary>
        [FieldDescription("生日")]
        public DateTime? Birthday { get; set; } = null;

        /// <summary>
        /// 积分
        /// </summary>
        [FieldDescription("积分")]
        public int? Score { get; set; } = 0;

        /// <summary>
        /// 粉丝数量
        /// </summary>
        [FieldDescription("粉丝数量")]
        public int? Fans { get; set; } = 0;

        /// <summary>
        /// 家庭住址
        /// </summary>
        [FieldDescription("家庭住址")]
        public string HomeAddress { get; set; } = string.Empty;

        /// <summary>
        /// 个性签名
        /// </summary>
        [FieldDescription("个性签名")]
        public string Signature { get; set; } = string.Empty;

        /// <summary>
        /// 系统样式选择
        /// </summary>
        [FieldDescription("系统样式选择")]
        public string Theme { get; set; } = string.Empty;

        /// <summary>
        /// 是否员工
        /// </summary>
        [FieldDescription("是否员工")]
        public int IsStaff { get; set; } = 0;

        /// <summary>
        /// 是否显示
        /// </summary>
        [FieldDescription("是否显示")]
        public int IsVisible { get; set; } = 1;

        /// <summary>
        /// 国家
        /// </summary>
        [FieldDescription("国家")]
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// 州
        /// </summary>
        [FieldDescription("州")]
        public string State { get; set; } = string.Empty;

        /// <summary>
        /// 省份
        /// </summary>
        [FieldDescription("省份")]
        public string Province { get; set; } = string.Empty;

        /// <summary>
        /// 城市
        /// </summary>
        [FieldDescription("城市")]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// 区域
        /// </summary>
        [FieldDescription("区域")]
        public string District { get; set; } = string.Empty;

        /// <summary>
        /// 审核状态
        /// </summary>
        [FieldDescription("审核状态")]
        public string AuditStatus { get; set; } = string.Empty;

        /// <summary>
        /// 经理用户编号
        /// </summary>
        [FieldDescription("经理用户编号")]
        public int ManagerUserId { get; set; } = 0;

        /// <summary>
        /// 是否系统管理员
        /// </summary>
        [FieldDescription("是否系统管理员")]
        public int IsAdministrator { get; set; } = 0;

        /// <summary>
        /// 是否检查余额
        /// </summary>
        [FieldDescription("是否检查余额")]
        public int IsCheckBalance { get; set; } = 0;

        /// <summary>
        /// 描述
        /// </summary>
        [FieldDescription("描述")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 排序编号
        /// </summary>
        [FieldDescription("排序编号")]
        public int SortCode { get; set; } = 0;

        /// <summary>
        /// 是否删除
        /// </summary>
        [FieldDescription("是否删除")]
        public int Deleted { get; set; } = 0;

        /// <summary>
        /// 是否有效
        /// </summary>
        [FieldDescription("是否有效")]
        public int Enabled { get; set; } = 1;

        /// <summary>
        /// 创建时间
        /// </summary>
        [FieldDescription("创建时间")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人编号
        /// </summary>
        [FieldDescription("创建人编号")]
        public int CreateUserId { get; set; } = 0;

        /// <summary>
        /// 创建人用户名
        /// </summary>
        [FieldDescription("创建人用户名")]
        public string CreateUserName { get; set; } = string.Empty;

        /// <summary>
        /// 创建人姓名
        /// </summary>
        [FieldDescription("创建人姓名")]
        public string CreateBy { get; set; } = string.Empty;

        /// <summary>
        /// 创建IP
        /// </summary>
        [FieldDescription("创建IP")]
        public string CreateIp { get; set; } = string.Empty;

        /// <summary>
        /// 修改时间
        /// </summary>
        [FieldDescription("修改时间")]
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改人编号
        /// </summary>
        [FieldDescription("修改人编号")]
        public int UpdateUserId { get; set; } = 0;

        /// <summary>
        /// 修改人用户名
        /// </summary>
        [FieldDescription("修改人用户名")]
        public string UpdateUserName { get; set; } = string.Empty;

        /// <summary>
        /// 修改人姓名
        /// </summary>
        [FieldDescription("修改人姓名")]
        public string UpdateBy { get; set; } = string.Empty;

        /// <summary>
        /// 修改IP
        /// </summary>
        [FieldDescription("修改IP")]
        public string UpdateIp { get; set; } = string.Empty;

        #region UserLogon

        ///// <summary>
        ///// 用户密码
        ///// </summary>
        //[FieldDescription("用户密码")]
        //public string UserPassword { get; set; } = string.Empty;

        /// <summary>
        /// 单点登录标识
        /// </summary>
        [FieldDescription("单点登录标识")]
        public string OpenId { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// 允许登录时间开始
        /// </summary>
        [FieldDescription("允许登录时间开始")]
        public DateTime? AllowStartTime { get; set; } = null;

        /// <summary>
        /// 允许登录时间结束
        /// </summary>
        [FieldDescription("允许登录时间结束")]
        public DateTime? AllowEndTime { get; set; } = null;

        /// <summary>
        /// 暂停用户开始日期
        /// </summary>
        [FieldDescription("暂停用户开始日期")]
        public DateTime? LockStartTime { get; set; } = null;

        /// <summary>
        /// 暂停用户结束日期
        /// </summary>
        [FieldDescription("暂停用户结束日期")]
        public DateTime? LockEndTime { get; set; } = null;

        /// <summary>
        /// 第一次访问时间
        /// </summary>
        [FieldDescription("第一次访问时间")]
        public DateTime? FirstVisitTime { get; set; } = null;

        /// <summary>
        /// 上一次访问时间
        /// </summary>
        [FieldDescription("上一次访问时间")]
        public DateTime? PreviousVisitTime { get; set; } = null;

        /// <summary>
        /// 最后访问时间
        /// </summary>
        [FieldDescription("最后访问时间")]
        public DateTime? LastVisitTime { get; set; } = null;

        /// <summary>
        /// 最后修改密码日期
        /// </summary>
        [FieldDescription("最后修改密码日期")]
        public DateTime? ChangePasswordTime { get; set; } = null;

        /// <summary>
        /// 登录次数
        /// </summary>
        [FieldDescription("登录次数")]
        public int LogonCount { get; set; } = 0;

        /// <summary>
        /// 是否并发用户
        /// </summary>
        [FieldDescription("是否并发用户")]
        public int ConcurrentUser { get; set; } = 0;

        /// <summary>
        /// 展示次数
        /// </summary>
        [FieldDescription("展示次数")]
        public int ShowCount { get; set; } = 0;

        /// <summary>
        /// 密码连续错误次数
        /// </summary>
        [FieldDescription("密码连续错误次数")]
        public int PasswordErrorCount { get; set; } = 0;

        /// <summary>
        /// 在线状态
        /// </summary>
        [FieldDescription("在线状态")]
        public int UserOnline { get; set; } = 0;

        /// <summary>
        /// IP访问限制
        /// </summary>
        [FieldDescription("IP访问限制")]
        public int CheckIpAddress { get; set; } = 0;

        /// <summary>
        /// 验证码
        /// </summary>
        [FieldDescription("验证码")]
        public string VerificationCode { get; set; } = string.Empty;

        /// <summary>
        /// 登录IP地址
        /// </summary>
        [FieldDescription("登录IP地址")]
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// 登录MAC地址
        /// </summary>
        [FieldDescription("登录MAC地址")]
        public string MacAddress { get; set; } = string.Empty;

        /// <summary>
        /// 密码提示问题
        /// </summary>
        [FieldDescription("密码提示问题")]
        public string Question { get; set; } = string.Empty;

        /// <summary>
        /// 密码提示答案
        /// </summary>
        [FieldDescription("密码提示答案")]
        public string AnswerQuestion { get; set; } = string.Empty;

        ///// <summary>
        ///// 密码加盐
        ///// </summary>
        //[FieldDescription("密码加盐")]
        //public string Salt { get; set; } = string.Empty;

        /// <summary>
        /// OpenId过期时间
        /// </summary>
        [FieldDescription("OpenId过期时间")]
        public DateTime? OpenIdTimeoutTime { get; set; } = null;

        /// <summary>
        /// 子系统编码
        /// </summary>
        [FieldDescription("子系统编码")]
        public string SystemCode { get; set; } = string.Empty;

        /// <summary>
        /// IP地址所在位置名称
        /// </summary>
        [FieldDescription("IP地址所在位置名称")]
        public string IpAddressName { get; set; } = string.Empty;

        /// <summary>
        /// 密码强度
        /// </summary>
        [FieldDescription("密码强度")]
        public int? PasswordStrength { get; set; } = null;

        /// <summary>
        /// 电脑名
        /// </summary>
        [FieldDescription("电脑名")]
        public string ComputerName { get; set; } = string.Empty;

        /// <summary>
        /// 是否需要修改密码
        /// </summary>
        [FieldDescription("是否需要修改密码")]
        public int NeedModifyPassword { get; set; } = 0;

        #endregion

        #region UserContact

        /// <summary>
        /// 手机
        /// </summary>
        [FieldDescription("手机")]
        public string Mobile { get; set; } = string.Empty;

        /// <summary>
        /// 短号
        /// </summary>
        [FieldDescription("短号")]
        public string ShortNumber { get; set; } = string.Empty;

        /// <summary>
        /// 旺旺号码
        /// </summary>
        [FieldDescription("旺旺号码")]
        public string Ww { get; set; } = string.Empty;

        /// <summary>
        /// 微信号码
        /// </summary>
        [FieldDescription("微信号码")]
        public string WeChat { get; set; } = string.Empty;

        /// <summary>
        /// 易信号码
        /// </summary>
        [FieldDescription("易信号码")]
        public string YiXin { get; set; } = string.Empty;

        /// <summary>
        /// 易信号码验证通过
        /// </summary>
        [FieldDescription("易信号码验证通过")]
        public int YiXinValiated { get; set; } = 0;

        /// <summary>
        /// 电话号码
        /// </summary>
        [FieldDescription("电话号码")]
        public string Telephone { get; set; } = string.Empty;

        /// <summary>
        /// 分机号码
        /// </summary>
        [FieldDescription("分机号码")]
        public string Extension { get; set; } = string.Empty;

        /// <summary>
        /// QQ号码
        /// </summary>
        [FieldDescription("QQ号码")]
        public string Qq { get; set; } = null;

        /// <summary>
        /// 电子邮件
        /// </summary>
        [FieldDescription("电子邮件")]
        public string Email { get; set; } = null;

        /// <summary>
        /// 公司邮件
        /// </summary>
        [FieldDescription("公司邮件")]
        public string CompanyEmail { get; set; } = null;

        /// <summary>
        /// YY
        /// </summary>
        [FieldDescription("YY")]
        public string Yy { get; set; } = null;

        /// <summary>
        /// 紧急联系
        /// </summary>
        [FieldDescription("紧急联系")]
        public string EmergencyContact { get; set; } = null;

        #endregion
    }
}