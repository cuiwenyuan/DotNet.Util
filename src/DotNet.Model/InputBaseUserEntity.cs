//-----------------------------------------------------------------------
// <copyright file="BaseUserEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2022, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// InputBaseUserEntity
    /// 用于后台增加用户
    ///
    /// 修改记录
    ///
    /// 2022-03-14 版本：1.0 Troy.Cui 创建文件。
    ///
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2022-03-14</date>
    /// </author>
    /// </summary>
    public partial class InputBaseUserEntity
    {
        /// <summary>
        /// 编号
        /// </summary>
        [FieldDescription("编号")]
        public int Id { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [FieldDescription("来源")]
        public string UserFrom { get; set; } = "Web";

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
        /// 描述
        /// </summary>
        [FieldDescription("描述")]
        public string Description { get; set; } = string.Empty;

        #region UserLogon

        /// <summary>
        /// 用户密码
        /// </summary>
        [FieldDescription("用户密码")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 确认用户密码
        /// </summary>
        [FieldDescription("确认用户密码")]
        public string ConfirmPassword { get; set; } = string.Empty;

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
        /// 是否并发用户
        /// </summary>
        [FieldDescription("是否并发用户")]
        public int ConcurrentUser { get; set; } = 0;

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

        /// <summary>
        /// 子系统编码
        /// </summary>
        [FieldDescription("子系统编码")]
        public string SystemCode { get; set; } = string.Empty;

        /// <summary>
        /// 电脑名
        /// </summary>
        [FieldDescription("电脑名")]
        public string ComputerName { get; set; } = string.Empty;


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
        public string CompanyMail { get; set; } = null;

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