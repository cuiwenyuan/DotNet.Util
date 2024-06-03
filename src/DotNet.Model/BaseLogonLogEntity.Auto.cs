//-----------------------------------------------------------------------
// <copyright file="BaseLogonLogEntity.Auto.cs" company="DotNet">
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
    /// BaseLogonLogEntity
    /// 登录日志
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
    public partial class BaseLogonLogEntity : BaseEntity
    {
        /// <summary>
        /// 子系统编码
        /// </summary>
        [FieldDescription("子系统编码")]
        [Description("子系统编码")]
        [Column(FieldSystemCode)]
        public string SystemCode { get; set; } = string.Empty;

        /// <summary>
        /// 发起请求的终端应用类型
        /// </summary>
        [FieldDescription("发起请求的终端应用类型")]
        [Description("发起请求的终端应用类型")]
        [Column(FieldSourceType)]
        public string SourceType { get; set; } = string.Empty;

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
        /// 昵称
        /// </summary>
        [FieldDescription("昵称")]
        [Description("昵称")]
        [Column(FieldNickName)]
        public string NickName { get; set; } = string.Empty;

        /// <summary>
        /// 真实姓名
        /// </summary>
        [FieldDescription("真实姓名")]
        [Description("真实姓名")]
        [Column(FieldRealName)]
        public string RealName { get; set; } = string.Empty;

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
        /// 公司编码
        /// </summary>
        [FieldDescription("公司编码")]
        [Description("公司编码")]
        [Column(FieldCompanyCode)]
        public string CompanyCode { get; set; } = string.Empty;

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
        /// 服务
        /// </summary>
        [FieldDescription("服务")]
        [Description("服务")]
        [Column(FieldService)]
        public string Service { get; set; } = string.Empty;

        /// <summary>
        /// 耗时
        /// </summary>
        [FieldDescription("耗时")]
        [Description("耗时")]
        [Column(FieldElapsedTicks)]
        public int ElapsedTicks { get; set; } = 0;

        /// <summary>
        /// 登录的目标应用
        /// </summary>
        [FieldDescription("登录的目标应用")]
        [Description("登录的目标应用")]
        [Column(FieldTargetApplication)]
        public string TargetApplication { get; set; } = string.Empty;

        /// <summary>
        /// 登录的目标服务器端IP
        /// </summary>
        [FieldDescription("登录的目标服务器端IP")]
        [Description("登录的目标服务器端IP")]
        [Column(FieldTargetIp)]
        public string TargetIp { get; set; } = string.Empty;

        /// <summary>
        /// 操作结果（Success 1/Fail 0）
        /// </summary>
        [FieldDescription("操作结果（Success 1/Fail 0）")]
        [Description("操作结果（Success 1/Fail 0）")]
        [Column(FieldResult)]
        public int Result { get; set; } = 0;

        /// <summary>
        /// 操作类型（Login 1/Logout 0）
        /// </summary>
        [FieldDescription("操作类型（Login 1/Logout 0）")]
        [Description("操作类型（Login 1/Logout 0）")]
        [Column(FieldOperationType)]
        public int OperationType { get; set; } = 1;

        /// <summary>
        /// 登录状态
        /// </summary>
        [FieldDescription("登录状态")]
        [Description("登录状态")]
        [Column(FieldLogonStatus)]
        public string LogonStatus { get; set; } = string.Empty;

        /// <summary>
        /// 登录级别（0，正常；1、注意；2，危险；3、攻击）
        /// </summary>
        [FieldDescription("登录级别（0，正常；1、注意；2，危险；3、攻击）")]
        [Description("登录级别（0，正常；1、注意；2，危险；3、攻击）")]
        [Column(FieldLogLevel)]
        public int LogLevel { get; set; } = 0;

        /// <summary>
        /// IP地址
        /// </summary>
        [FieldDescription("IP地址")]
        [Description("IP地址")]
        [Column(FieldIpAddress)]
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// IP地址位置名称
        /// </summary>
        [FieldDescription("IP地址位置名称")]
        [Description("IP地址位置名称")]
        [Column(FieldIpAddressName)]
        public string IpAddressName { get; set; } = string.Empty;

        /// <summary>
        /// MAC地址
        /// </summary>
        [FieldDescription("MAC地址")]
        [Description("MAC地址")]
        [Column(FieldMacAddress)]
        public string MacAddress { get; set; } = string.Empty;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExtend(dr);
            GetBase(dr);
            if (dr.ContainsColumn(FieldSystemCode))
            {
                SystemCode = BaseUtil.ConvertToString(dr[FieldSystemCode]);
            }
            if (dr.ContainsColumn(FieldSourceType))
            {
                SourceType = BaseUtil.ConvertToString(dr[FieldSourceType]);
            }
            if (dr.ContainsColumn(FieldUserId))
            {
                UserId = BaseUtil.ConvertToInt(dr[FieldUserId]);
            }
            if (dr.ContainsColumn(FieldUserName))
            {
                UserName = BaseUtil.ConvertToString(dr[FieldUserName]);
            }
            if (dr.ContainsColumn(FieldNickName))
            {
                NickName = BaseUtil.ConvertToString(dr[FieldNickName]);
            }
            if (dr.ContainsColumn(FieldRealName))
            {
                RealName = BaseUtil.ConvertToString(dr[FieldRealName]);
            }
            if (dr.ContainsColumn(FieldCompanyId))
            {
                CompanyId = BaseUtil.ConvertToInt(dr[FieldCompanyId]);
            }
            if (dr.ContainsColumn(FieldCompanyName))
            {
                CompanyName = BaseUtil.ConvertToString(dr[FieldCompanyName]);
            }
            if (dr.ContainsColumn(FieldCompanyCode))
            {
                CompanyCode = BaseUtil.ConvertToString(dr[FieldCompanyCode]);
            }
            if (dr.ContainsColumn(FieldProvince))
            {
                Province = BaseUtil.ConvertToString(dr[FieldProvince]);
            }
            if (dr.ContainsColumn(FieldCity))
            {
                City = BaseUtil.ConvertToString(dr[FieldCity]);
            }
            if (dr.ContainsColumn(FieldService))
            {
                Service = BaseUtil.ConvertToString(dr[FieldService]);
            }
            if (dr.ContainsColumn(FieldElapsedTicks))
            {
                ElapsedTicks = BaseUtil.ConvertToInt(dr[FieldElapsedTicks]);
            }
            if (dr.ContainsColumn(FieldTargetApplication))
            {
                TargetApplication = BaseUtil.ConvertToString(dr[FieldTargetApplication]);
            }
            if (dr.ContainsColumn(FieldTargetIp))
            {
                TargetIp = BaseUtil.ConvertToString(dr[FieldTargetIp]);
            }
            if (dr.ContainsColumn(FieldResult))
            {
                Result = BaseUtil.ConvertToInt(dr[FieldResult]);
            }
            if (dr.ContainsColumn(FieldOperationType))
            {
                OperationType = BaseUtil.ConvertToInt(dr[FieldOperationType]);
            }
            if (dr.ContainsColumn(FieldLogonStatus))
            {
                LogonStatus = BaseUtil.ConvertToString(dr[FieldLogonStatus]);
            }
            if (dr.ContainsColumn(FieldLogLevel))
            {
                LogLevel = BaseUtil.ConvertToInt(dr[FieldLogLevel]);
            }
            if (dr.ContainsColumn(FieldIpAddress))
            {
                IpAddress = BaseUtil.ConvertToString(dr[FieldIpAddress]);
            }
            if (dr.ContainsColumn(FieldIpAddressName))
            {
                IpAddressName = BaseUtil.ConvertToString(dr[FieldIpAddressName]);
            }
            if (dr.ContainsColumn(FieldMacAddress))
            {
                MacAddress = BaseUtil.ConvertToString(dr[FieldMacAddress]);
            }
            return this;
        }

        ///<summary>
        /// 登录日志
        ///</summary>
        [FieldDescription("登录日志")]
        public const string CurrentTableName = "BaseLogonLog";

        ///<summary>
        /// 子系统编码
        ///</summary>
        public const string FieldSystemCode = "SystemCode";

        ///<summary>
        /// 发起请求的终端应用类型
        ///</summary>
        public const string FieldSourceType = "SourceType";

        ///<summary>
        /// 用户主键
        ///</summary>
        public const string FieldUserId = "UserId";

        ///<summary>
        /// 用户名
        ///</summary>
        public const string FieldUserName = "UserName";

        ///<summary>
        /// 昵称
        ///</summary>
        public const string FieldNickName = "NickName";

        ///<summary>
        /// 真实姓名
        ///</summary>
        public const string FieldRealName = "RealName";

        ///<summary>
        /// 公司主键
        ///</summary>
        public const string FieldCompanyId = "CompanyId";

        ///<summary>
        /// 公司名称
        ///</summary>
        public const string FieldCompanyName = "CompanyName";

        ///<summary>
        /// 公司编码
        ///</summary>
        public const string FieldCompanyCode = "CompanyCode";

        ///<summary>
        /// 省份
        ///</summary>
        public const string FieldProvince = "Province";

        ///<summary>
        /// 城市
        ///</summary>
        public const string FieldCity = "City";

        ///<summary>
        /// 服务
        ///</summary>
        public const string FieldService = "Service";

        ///<summary>
        /// 耗时
        ///</summary>
        public const string FieldElapsedTicks = "ElapsedTicks";

        ///<summary>
        /// 登录的目标应用
        ///</summary>
        public const string FieldTargetApplication = "TargetApplication";

        ///<summary>
        /// 登录的目标服务器端IP
        ///</summary>
        public const string FieldTargetIp = "TargetIp";

        ///<summary>
        /// 操作结果（Success 1/Fail 0）
        ///</summary>
        public const string FieldResult = "Result";

        ///<summary>
        /// 操作类型（Login 1/Logout 0）
        ///</summary>
        public const string FieldOperationType = "OperationType";

        ///<summary>
        /// 登录状态
        ///</summary>
        public const string FieldLogonStatus = "LogonStatus";

        ///<summary>
        /// 登录级别（0，正常；1、注意；2，危险；3、攻击）
        ///</summary>
        public const string FieldLogLevel = "LogLevel";

        ///<summary>
        /// IP地址
        ///</summary>
        public const string FieldIpAddress = "IpAddress";

        ///<summary>
        /// IP地址位置名称
        ///</summary>
        public const string FieldIpAddressName = "IpAddressName";

        ///<summary>
        /// MAC地址
        ///</summary>
        public const string FieldMacAddress = "MacAddress";
    }
}
