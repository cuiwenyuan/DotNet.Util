//-----------------------------------------------------------------------
// <copyright file="BaseUserLogonEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseUserLogonEntity
    /// 系统用户登录信息表
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
    public partial class BaseUserLogonEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [FieldDescription("主键")]
        public int Id { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        [FieldDescription("用户编号")]
        public int UserId { get; set; } = 0;

        /// <summary>
        /// 用户密码
        /// </summary>
        [FieldDescription("用户密码")]
        public string UserPassword { get; set; } = string.Empty;

        /// <summary>
        /// 当点登录标识
        /// </summary>
        [FieldDescription("当点登录标识")]
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

        /// <summary>
        /// 密码加盐
        /// </summary>
        [FieldDescription("密码加盐")]
        public string Salt { get; set; } = string.Empty;

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

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExtend(dr);
            if (dr.ContainsColumn(FieldId))
            {
                Id = BaseUtil.ConvertToInt(dr[FieldId]);
            }
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
            if (dr.ContainsColumn(FieldSortCode))
            {
                SortCode = BaseUtil.ConvertToInt(dr[FieldSortCode]);
            }
            if (dr.ContainsColumn(FieldDeleted))
            {
                Deleted = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            }
            if (dr.ContainsColumn(FieldEnabled))
            {
                Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            }
            if (dr.ContainsColumn(FieldCreateTime))
            {
                CreateTime = BaseUtil.ConvertToDateTime(dr[FieldCreateTime]);
            }
            if (dr.ContainsColumn(FieldCreateUserId))
            {
                CreateUserId = BaseUtil.ConvertToInt(dr[FieldCreateUserId]);
            }
            if (dr.ContainsColumn(FieldCreateUserName))
            {
                CreateUserName = BaseUtil.ConvertToString(dr[FieldCreateUserName]);
            }
            if (dr.ContainsColumn(FieldCreateBy))
            {
                CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            }
            if (dr.ContainsColumn(FieldCreateIp))
            {
                CreateIp = BaseUtil.ConvertToString(dr[FieldCreateIp]);
            }
            if (dr.ContainsColumn(FieldUpdateTime))
            {
                UpdateTime = BaseUtil.ConvertToDateTime(dr[FieldUpdateTime]);
            }
            if (dr.ContainsColumn(FieldUpdateUserId))
            {
                UpdateUserId = BaseUtil.ConvertToInt(dr[FieldUpdateUserId]);
            }
            if (dr.ContainsColumn(FieldUpdateUserName))
            {
                UpdateUserName = BaseUtil.ConvertToString(dr[FieldUpdateUserName]);
            }
            if (dr.ContainsColumn(FieldUpdateBy))
            {
                UpdateBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            }
            if (dr.ContainsColumn(FieldUpdateIp))
            {
                UpdateIp = BaseUtil.ConvertToString(dr[FieldUpdateIp]);
            }
            return this;
        }

        ///<summary>
        /// 系统用户登录信息表
        ///</summary>
        [FieldDescription("系统用户登录信息表")]
        public const string CurrentTableName = "BaseUserLogon";

        ///<summary>
        /// 主键
        ///</summary>
        public const string FieldId = "Id";

        ///<summary>
        /// 用户编号
        ///</summary>
        public const string FieldUserId = "UserId";

        ///<summary>
        /// 用户密码
        ///</summary>
        public const string FieldUserPassword = "UserPassword";

        ///<summary>
        /// 当点登录标识
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
        /// 允许同时有多用户登录
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

        ///<summary>
        /// 排序编号
        ///</summary>
        public const string FieldSortCode = "SortCode";

        ///<summary>
        /// 是否删除
        ///</summary>
        public const string FieldDeleted = "Deleted";

        ///<summary>
        /// 是否有效
        ///</summary>
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 创建时间
        ///</summary>
        public const string FieldCreateTime = "CreateTime";

        ///<summary>
        /// 创建人编号
        ///</summary>
        public const string FieldCreateUserId = "CreateUserId";

        ///<summary>
        /// 创建人用户名
        ///</summary>
        public const string FieldCreateUserName = "CreateUserName";

        ///<summary>
        /// 创建人姓名
        ///</summary>
        public const string FieldCreateBy = "CreateBy";

        ///<summary>
        /// 创建IP
        ///</summary>
        public const string FieldCreateIp = "CreateIp";

        ///<summary>
        /// 修改时间
        ///</summary>
        public const string FieldUpdateTime = "UpdateTime";

        ///<summary>
        /// 修改人编号
        ///</summary>
        public const string FieldUpdateUserId = "UpdateUserId";

        ///<summary>
        /// 修改人用户名
        ///</summary>
        public const string FieldUpdateUserName = "UpdateUserName";

        ///<summary>
        /// 修改人姓名
        ///</summary>
        public const string FieldUpdateBy = "UpdateBy";

        ///<summary>
        /// 修改IP
        ///</summary>
        public const string FieldUpdateIp = "UpdateIp";
    }
}
