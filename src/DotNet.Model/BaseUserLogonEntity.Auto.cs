﻿//-----------------------------------------------------------------------
// <copyright file="BaseUserLogonEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2025, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseUserLogonEntity
    /// 用户登录信息
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
    public partial class BaseUserLogonEntity : BaseEntity
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        [FieldDescription("用户编号")]
        [Description("用户编号")]
        [Column(FieldUserId)]
        public int UserId { get; set; } = 0;

        /// <summary>
        /// 用户密码
        /// </summary>
        [FieldDescription("用户密码")]
        [Description("用户密码")]
        [Column(FieldUserPassword)]
        public string UserPassword { get; set; } = string.Empty;

        /// <summary>
        /// 单点登录标识
        /// </summary>
        [FieldDescription("单点登录标识")]
        [Description("单点登录标识")]
        [Column(FieldOpenId)]
        public string OpenId { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// 允许登录时间开始
        /// </summary>
        [FieldDescription("允许登录时间开始")]
        [Description("允许登录时间开始")]
        [Column(FieldAllowStartTime)]
        public DateTime? AllowStartTime { get; set; } = null;

        /// <summary>
        /// 允许登录时间结束
        /// </summary>
        [FieldDescription("允许登录时间结束")]
        [Description("允许登录时间结束")]
        [Column(FieldAllowEndTime)]
        public DateTime? AllowEndTime { get; set; } = null;

        /// <summary>
        /// 暂停用户开始日期
        /// </summary>
        [FieldDescription("暂停用户开始日期")]
        [Description("暂停用户开始日期")]
        [Column(FieldLockStartTime)]
        public DateTime? LockStartTime { get; set; } = null;

        /// <summary>
        /// 暂停用户结束日期
        /// </summary>
        [FieldDescription("暂停用户结束日期")]
        [Description("暂停用户结束日期")]
        [Column(FieldLockEndTime)]
        public DateTime? LockEndTime { get; set; } = null;

        /// <summary>
        /// 第一次访问时间
        /// </summary>
        [FieldDescription("第一次访问时间")]
        [Description("第一次访问时间")]
        [Column(FieldFirstVisitTime)]
        public DateTime? FirstVisitTime { get; set; } = null;

        /// <summary>
        /// 上一次访问时间
        /// </summary>
        [FieldDescription("上一次访问时间")]
        [Description("上一次访问时间")]
        [Column(FieldPreviousVisitTime)]
        public DateTime? PreviousVisitTime { get; set; } = null;

        /// <summary>
        /// 最后访问时间
        /// </summary>
        [FieldDescription("最后访问时间")]
        [Description("最后访问时间")]
        [Column(FieldLastVisitTime)]
        public DateTime? LastVisitTime { get; set; } = null;

        /// <summary>
        /// 最后修改密码日期
        /// </summary>
        [FieldDescription("最后修改密码日期")]
        [Description("最后修改密码日期")]
        [Column(FieldChangePasswordTime)]
        public DateTime? ChangePasswordTime { get; set; } = null;

        /// <summary>
        /// 登录次数
        /// </summary>
        [FieldDescription("登录次数")]
        [Description("登录次数")]
        [Column(FieldLogonCount)]
        public int LogonCount { get; set; } = 0;

        /// <summary>
        /// 是否并发用户
        /// </summary>
        [FieldDescription("是否并发用户")]
        [Description("是否并发用户")]
        [Column(FieldConcurrentUser)]
        public int ConcurrentUser { get; set; } = 0;

        /// <summary>
        /// 展示次数
        /// </summary>
        [FieldDescription("展示次数")]
        [Description("展示次数")]
        [Column(FieldShowCount)]
        public int ShowCount { get; set; } = 0;

        /// <summary>
        /// 密码连续错误次数
        /// </summary>
        [FieldDescription("密码连续错误次数")]
        [Description("密码连续错误次数")]
        [Column(FieldPasswordErrorCount)]
        public int PasswordErrorCount { get; set; } = 0;

        /// <summary>
        /// 在线状态
        /// </summary>
        [FieldDescription("在线状态")]
        [Description("在线状态")]
        [Column(FieldUserOnline)]
        public int UserOnline { get; set; } = 0;

        /// <summary>
        /// IP访问限制
        /// </summary>
        [FieldDescription("IP访问限制")]
        [Description("IP访问限制")]
        [Column(FieldCheckIpAddress)]
        public int CheckIpAddress { get; set; } = 0;

        /// <summary>
        /// 验证码
        /// </summary>
        [FieldDescription("验证码")]
        [Description("验证码")]
        [Column(FieldVerificationCode)]
        public string VerificationCode { get; set; } = string.Empty;

        /// <summary>
        /// 登录IP地址
        /// </summary>
        [FieldDescription("登录IP地址")]
        [Description("登录IP地址")]
        [Column(FieldIpAddress)]
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// 登录MAC地址
        /// </summary>
        [FieldDescription("登录MAC地址")]
        [Description("登录MAC地址")]
        [Column(FieldMacAddress)]
        public string MacAddress { get; set; } = string.Empty;

        /// <summary>
        /// 密码提示问题
        /// </summary>
        [FieldDescription("密码提示问题")]
        [Description("密码提示问题")]
        [Column(FieldQuestion)]
        public string Question { get; set; } = string.Empty;

        /// <summary>
        /// 密码提示答案
        /// </summary>
        [FieldDescription("密码提示答案")]
        [Description("密码提示答案")]
        [Column(FieldAnswerQuestion)]
        public string AnswerQuestion { get; set; } = string.Empty;

        /// <summary>
        /// 密码加盐
        /// </summary>
        [FieldDescription("密码加盐")]
        [Description("密码加盐")]
        [Column(FieldSalt)]
        public string Salt { get; set; } = string.Empty;

        /// <summary>
        /// OpenId过期时间
        /// </summary>
        [FieldDescription("OpenId过期时间")]
        [Description("OpenId过期时间")]
        [Column(FieldOpenIdTimeoutTime)]
        public DateTime? OpenIdTimeoutTime { get; set; } = null;

        /// <summary>
        /// 子系统编码
        /// </summary>
        [FieldDescription("子系统编码")]
        [Description("子系统编码")]
        [Column(FieldSystemCode)]
        public string SystemCode { get; set; } = string.Empty;

        /// <summary>
        /// IP地址所在位置名称
        /// </summary>
        [FieldDescription("IP地址所在位置名称")]
        [Description("IP地址所在位置名称")]
        [Column(FieldIpAddressName)]
        public string IpAddressName { get; set; } = string.Empty;

        /// <summary>
        /// 密码强度
        /// </summary>
        [FieldDescription("密码强度")]
        [Description("密码强度")]
        [Column(FieldPasswordStrength)]
        public int? PasswordStrength { get; set; } = null;

        /// <summary>
        /// 电脑名
        /// </summary>
        [FieldDescription("电脑名")]
        [Description("电脑名")]
        [Column(FieldComputerName)]
        public string ComputerName { get; set; } = string.Empty;

        /// <summary>
        /// 是否需要修改密码
        /// </summary>
        [FieldDescription("是否需要修改密码")]
        [Description("是否需要修改密码")]
        [Column(FieldNeedModifyPassword)]
        public int NeedModifyPassword { get; set; } = 0;

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
            if (dr.ContainsColumn(FieldUserPassword))
            {
                UserPassword = BaseUtil.ConvertToString(dr[FieldUserPassword]);
            }
            if (dr.ContainsColumn(FieldOpenId))
            {
                OpenId = BaseUtil.ConvertToString(dr[FieldOpenId]);
            }
            if (dr.ContainsColumn(FieldAllowStartTime))
            {
                AllowStartTime = BaseUtil.ConvertToNullableDateTime(dr[FieldAllowStartTime]);
            }
            if (dr.ContainsColumn(FieldAllowEndTime))
            {
                AllowEndTime = BaseUtil.ConvertToNullableDateTime(dr[FieldAllowEndTime]);
            }
            if (dr.ContainsColumn(FieldLockStartTime))
            {
                LockStartTime = BaseUtil.ConvertToNullableDateTime(dr[FieldLockStartTime]);
            }
            if (dr.ContainsColumn(FieldLockEndTime))
            {
                LockEndTime = BaseUtil.ConvertToNullableDateTime(dr[FieldLockEndTime]);
            }
            if (dr.ContainsColumn(FieldFirstVisitTime))
            {
                FirstVisitTime = BaseUtil.ConvertToNullableDateTime(dr[FieldFirstVisitTime]);
            }
            if (dr.ContainsColumn(FieldPreviousVisitTime))
            {
                PreviousVisitTime = BaseUtil.ConvertToNullableDateTime(dr[FieldPreviousVisitTime]);
            }
            if (dr.ContainsColumn(FieldLastVisitTime))
            {
                LastVisitTime = BaseUtil.ConvertToNullableDateTime(dr[FieldLastVisitTime]);
            }
            if (dr.ContainsColumn(FieldChangePasswordTime))
            {
                ChangePasswordTime = BaseUtil.ConvertToNullableDateTime(dr[FieldChangePasswordTime]);
            }
            if (dr.ContainsColumn(FieldLogonCount))
            {
                LogonCount = BaseUtil.ConvertToInt(dr[FieldLogonCount]);
            }
            if (dr.ContainsColumn(FieldConcurrentUser))
            {
                ConcurrentUser = BaseUtil.ConvertToInt(dr[FieldConcurrentUser]);
            }
            if (dr.ContainsColumn(FieldShowCount))
            {
                ShowCount = BaseUtil.ConvertToInt(dr[FieldShowCount]);
            }
            if (dr.ContainsColumn(FieldPasswordErrorCount))
            {
                PasswordErrorCount = BaseUtil.ConvertToInt(dr[FieldPasswordErrorCount]);
            }
            if (dr.ContainsColumn(FieldUserOnline))
            {
                UserOnline = BaseUtil.ConvertToInt(dr[FieldUserOnline]);
            }
            if (dr.ContainsColumn(FieldCheckIpAddress))
            {
                CheckIpAddress = BaseUtil.ConvertToInt(dr[FieldCheckIpAddress]);
            }
            if (dr.ContainsColumn(FieldVerificationCode))
            {
                VerificationCode = BaseUtil.ConvertToString(dr[FieldVerificationCode]);
            }
            if (dr.ContainsColumn(FieldIpAddress))
            {
                IpAddress = BaseUtil.ConvertToString(dr[FieldIpAddress]);
            }
            if (dr.ContainsColumn(FieldMacAddress))
            {
                MacAddress = BaseUtil.ConvertToString(dr[FieldMacAddress]);
            }
            if (dr.ContainsColumn(FieldQuestion))
            {
                Question = BaseUtil.ConvertToString(dr[FieldQuestion]);
            }
            if (dr.ContainsColumn(FieldAnswerQuestion))
            {
                AnswerQuestion = BaseUtil.ConvertToString(dr[FieldAnswerQuestion]);
            }
            if (dr.ContainsColumn(FieldSalt))
            {
                Salt = BaseUtil.ConvertToString(dr[FieldSalt]);
            }
            if (dr.ContainsColumn(FieldOpenIdTimeoutTime))
            {
                OpenIdTimeoutTime = BaseUtil.ConvertToNullableDateTime(dr[FieldOpenIdTimeoutTime]);
            }
            if (dr.ContainsColumn(FieldSystemCode))
            {
                SystemCode = BaseUtil.ConvertToString(dr[FieldSystemCode]);
            }
            if (dr.ContainsColumn(FieldIpAddressName))
            {
                IpAddressName = BaseUtil.ConvertToString(dr[FieldIpAddressName]);
            }
            if (dr.ContainsColumn(FieldPasswordStrength))
            {
                PasswordStrength = BaseUtil.ConvertToNullableByteInt(dr[FieldPasswordStrength]);
            }
            if (dr.ContainsColumn(FieldComputerName))
            {
                ComputerName = BaseUtil.ConvertToString(dr[FieldComputerName]);
            }
            if (dr.ContainsColumn(FieldNeedModifyPassword))
            {
                NeedModifyPassword = BaseUtil.ConvertToInt(dr[FieldNeedModifyPassword]);
            }
            return this;
        }

        ///<summary>
        /// 用户登录信息
        ///</summary>
        [FieldDescription("用户登录信息")]
        public const string CurrentTableName = "BaseUserLogon";

        ///<summary>
        /// 表名
        ///</summary>
        public const string CurrentTableDescription = "用户登录信息";

        ///<summary>
        /// 用户编号
        ///</summary>
        public const string FieldUserId = "UserId";

        ///<summary>
        /// 用户密码
        ///</summary>
        public const string FieldUserPassword = "UserPassword";

        ///<summary>
        /// 单点登录标识
        ///</summary>
        public const string FieldOpenId = "OpenId";

        ///<summary>
        /// 允许登录时间开始
        ///</summary>
        public const string FieldAllowStartTime = "AllowStartTime";

        ///<summary>
        /// 允许登录时间结束
        ///</summary>
        public const string FieldAllowEndTime = "AllowEndTime";

        ///<summary>
        /// 暂停用户开始日期
        ///</summary>
        public const string FieldLockStartTime = "LockStartTime";

        ///<summary>
        /// 暂停用户结束日期
        ///</summary>
        public const string FieldLockEndTime = "LockEndTime";

        ///<summary>
        /// 第一次访问时间
        ///</summary>
        public const string FieldFirstVisitTime = "FirstVisitTime";

        ///<summary>
        /// 上一次访问时间
        ///</summary>
        public const string FieldPreviousVisitTime = "PreviousVisitTime";

        ///<summary>
        /// 最后访问时间
        ///</summary>
        public const string FieldLastVisitTime = "LastVisitTime";

        ///<summary>
        /// 最后修改密码日期
        ///</summary>
        public const string FieldChangePasswordTime = "ChangePasswordTime";

        ///<summary>
        /// 登录次数
        ///</summary>
        public const string FieldLogonCount = "LogonCount";

        ///<summary>
        /// 是否并发用户
        ///</summary>
        public const string FieldConcurrentUser = "ConcurrentUser";

        ///<summary>
        /// 展示次数
        ///</summary>
        public const string FieldShowCount = "ShowCount";

        ///<summary>
        /// 密码连续错误次数
        ///</summary>
        public const string FieldPasswordErrorCount = "PasswordErrorCount";

        ///<summary>
        /// 在线状态
        ///</summary>
        public const string FieldUserOnline = "UserOnline";

        ///<summary>
        /// IP访问限制
        ///</summary>
        public const string FieldCheckIpAddress = "CheckIpAddress";

        ///<summary>
        /// 验证码
        ///</summary>
        public const string FieldVerificationCode = "VerificationCode";

        ///<summary>
        /// 登录IP地址
        ///</summary>
        public const string FieldIpAddress = "IpAddress";

        ///<summary>
        /// 登录MAC地址
        ///</summary>
        public const string FieldMacAddress = "MacAddress";

        ///<summary>
        /// 密码提示问题
        ///</summary>
        public const string FieldQuestion = "Question";

        ///<summary>
        /// 密码提示答案
        ///</summary>
        public const string FieldAnswerQuestion = "AnswerQuestion";

        ///<summary>
        /// 密码加盐
        ///</summary>
        public const string FieldSalt = "Salt";

        ///<summary>
        /// OpenId过期时间
        ///</summary>
        public const string FieldOpenIdTimeoutTime = "OpenIdTimeoutTime";

        ///<summary>
        /// 子系统编码
        ///</summary>
        public const string FieldSystemCode = "SystemCode";

        ///<summary>
        /// IP地址所在位置名称
        ///</summary>
        public const string FieldIpAddressName = "IpAddressName";

        ///<summary>
        /// 密码强度
        ///</summary>
        public const string FieldPasswordStrength = "PasswordStrength";

        ///<summary>
        /// 电脑名
        ///</summary>
        public const string FieldComputerName = "ComputerName";

        ///<summary>
        /// 是否需要修改密码
        ///</summary>
        public const string FieldNeedModifyPassword = "NeedModifyPassword";
    }
}
