//-----------------------------------------------------------------------
// <copyright file="BaseLogonLogEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2022, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseLogonLogEntity
    /// 登录日志表
    /// 
    /// 修改记录
    /// 
    /// 2021-10-04 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2021-10-04</date>
    /// </author>
    /// </summary>
    public partial class BaseLogonLogEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [FieldDescription("主键")]
        public int Id { get; set; }

        /// <summary>
        /// 子系统编码
        /// </summary>
        [FieldDescription("子系统编码")]
        public string SystemCode { get; set; } = string.Empty;

        /// <summary>
        /// 发起请求的终端应用类型
        /// </summary>
        [FieldDescription("发起请求的终端应用类型")]
        public string SourceType { get; set; } = string.Empty;

        /// <summary>
        /// 用户主键
        /// </summary>
        [FieldDescription("用户主键")]
        public int UserId { get; set; } = 0;

        /// <summary>
        /// 用户名
        /// </summary>
        [FieldDescription("用户名")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 昵称
        /// </summary>
        [FieldDescription("昵称")]
        public string NickName { get; set; } = string.Empty;

        /// <summary>
        /// 真实姓名
        /// </summary>
        [FieldDescription("真实姓名")]
        public string RealName { get; set; } = string.Empty;

        /// <summary>
        /// 公司主键
        /// </summary>
        [FieldDescription("公司主键")]
        public int CompanyId { get; set; } = 0;

        /// <summary>
        /// 公司名称
        /// </summary>
        [FieldDescription("公司名称")]
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// 公司编码
        /// </summary>
        [FieldDescription("公司编码")]
        public string CompanyCode { get; set; } = string.Empty;

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
        /// 服务
        /// </summary>
        [FieldDescription("服务")]
        public string Service { get; set; } = string.Empty;

        /// <summary>
        /// 耗时
        /// </summary>
        [FieldDescription("耗时")]
        public int ElapsedTicks { get; set; } = 0;

        /// <summary>
        /// 登录的目标应用
        /// </summary>
        [FieldDescription("登录的目标应用")]
        public string TargetApplication { get; set; } = string.Empty;

        /// <summary>
        /// 登录的目标服务器端IP
        /// </summary>
        [FieldDescription("登录的目标服务器端IP")]
        public string TargetIp { get; set; } = string.Empty;

        /// <summary>
        /// 操作结果（Success 1/Fail 0）
        /// </summary>
        [FieldDescription("操作结果（Success 1/Fail 0）")]
        public int Result { get; set; } = 0;

        /// <summary>
        /// 操作类型（Login 1/Logout 0）
        /// </summary>
        [FieldDescription("操作类型（Login 1/Logout 0）")]
        public int OperationType { get; set; } = 1;

        /// <summary>
        /// 登录状态
        /// </summary>
        [FieldDescription("登录状态")]
        public string LogonStatus { get; set; } = string.Empty;

        /// <summary>
        /// 登录级别（0，正常；1、注意；2，危险；3、攻击）
        /// </summary>
        [FieldDescription("登录级别（0，正常；1、注意；2，危险；3、攻击）")]
        public int LogLevel { get; set; } = 0;

        /// <summary>
        /// IP地址
        /// </summary>
        [FieldDescription("IP地址")]
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// IP地址位置名称
        /// </summary>
        [FieldDescription("IP地址位置名称")]
        public string IpAddressName { get; set; } = string.Empty;

        /// <summary>
        /// MAC地址
        /// </summary>
        [FieldDescription("MAC地址")]
        public string MacAddress { get; set; } = string.Empty;

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
        /// 登录日志
        ///</summary>
        [FieldDescription("登录日志")]
        public const string CurrentTableName = "BaseLogonLog";

        ///<summary>
        /// 主键
        ///</summary>
        public const string FieldId = "Id";

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
