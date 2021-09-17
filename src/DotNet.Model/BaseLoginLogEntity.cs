//-----------------------------------------------------------------------
// <copyright file="BaseLoginLogEntity.cs" company="DotNet">
//     Copyright (C) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseLoginLogEntity
    /// 系统登录日志表
    /// 
    /// 修改记录
    /// 
    /// 2016-04-15 版本：1.1 JiRiGaLa 强化字段、按阿里云的安全要求改进。
    /// 2014-03-18 版本：1.0 JiRiGaLa 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2016-04-15</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseLoginLogEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// 子系统编号
        /// 如果是从后台直接登陆数据库，则记数据库名；如果是其他应用通过单点登陆方式登陆，则写明进行登陆的来源应用；否则记该应用本身
        /// </summary>
        public string SystemCode { get; set; } = string.Empty;
        /// <summary>
        /// 发起请求的终端应用类型
        /// web、客户端、iOS、Android、Database
        /// </summary>
        public string SourceType { get; set; } = string.Empty;
        /// <summary>
        /// 用户主键、操作员账号ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;
        /// <summary>
        /// 用户名、操作员账号名
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; } = string.Empty;
        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; } = string.Empty;
        /// <summary>
        /// 公司主键、站点id
        /// 例如：14214
        /// </summary>
        public string CompanyId { get; set; } = null;
        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode { get; set; } = null;
        /// <summary>
        /// 公司名称、站点名称
        /// 例如：xxx数据中心
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;
        /// <summary>
        /// 省、站点所在省份
        /// 例如：浙江
        /// </summary>
        [FieldDescription("省")]
        public string Province { get; set; } = null;
        /// <summary>
        /// 市、站点所在城市
        /// 例如：杭州
        /// </summary>
        [FieldDescription("市")]
        public string City { get; set; } = null;
        /// <summary>
        /// 服务
        /// </summary>
        public string Service { get; set; } = string.Empty;
        /// <summary>
        /// 耗时
        /// </summary>
        public long ElapsedTicks { get; set; } = 0;
        /// <summary>
        /// 登录的目标应用
        /// 如果是从后台直接登陆数据库，则记数据库名；如果有跳转，记录跳转前应用和跳转后应用，用英文分号隔开；否则记为该应用本身;
        /// </summary>
        public string TargetApplication { get; set; } = string.Empty;
        /// <summary>
        /// 登录的目标服务器端IP
        /// 使用英文分割的键对记录目标IP；如果进行了NAT转换或使用了负载均衡，记录公网IP和内网IP；若无NAT转换和负载均衡，则记录公网IP。
        /// 如：publicip=xxx.xxx.xxx.xxx;privateip=192.168.xxx.xxx.xxx
        /// </summary>
        public string TargetIp { get; set; } = string.Empty;
        /// <summary>
        /// 操作结果（Success 1/Fail 0）
        /// </summary>
        public int Result { get; set; } = 0;
        /// <summary>
        /// 操作类型（Login 1/Logout 0）
        /// </summary>
        public int OperationType { get; set; } = 1;
        /// <summary>
        /// 登录状态
        /// </summary>
        public string LoginStatus { get; set; } = string.Empty;
        /// <summary>
        /// 登录级别（0，正常；1、注意；2，危险；3、攻击）
        /// </summary>
        public int LogLevel { get; set; } = 0;
        /// <summary>
        /// IP地址、登陆操作源IP
        /// 使用英文分割的键对记录源IP；如果使用了vpn或NAT转换，记录内网IP、公网IP和vpn分配的IP，若无vpn和NAT转换，则记录公网IP。
        /// 如：privateip=192.168.xxx.xxx.xxx;publicip=xxx.xxx.xxx.xxx;vpnip=192.168.xxx.xxx
        /// </summary>
        public string IpAddress { get; set; } = string.Empty;
        /// <summary>
        /// MAC地址，源MAC地址
        /// 如果是CS架构则记录，BS架构无需记录
        /// </summary>
        public string MacAddress { get; set; } = string.Empty;


        /// <summary>
        /// IP地址名称
        /// </summary>
        public string IpAddressName { get; set; } = "未知";

        /// <summary>
        /// 创建时间、操作时间
        /// yyyy-MM-ddHH:mm:ss格式
        /// </summary>
        public DateTime? CreateOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            Id = BaseUtil.ConvertToString(dr[FieldId]);
            CreateOn = BaseUtil.ConvertToNullableDateTime(dr[FieldCreateTime]);
            SystemCode = BaseUtil.ConvertToString(dr[FieldSystemCode]);
            UserId = BaseUtil.ConvertToString(dr[FieldUserId]);
            UserName = BaseUtil.ConvertToString(dr[FieldUserName]);
            NickName = BaseUtil.ConvertToString(dr[FieldNickName]);
            RealName = BaseUtil.ConvertToString(dr[FieldRealName]);
            CompanyId = BaseUtil.ConvertToString(dr[FieldCompanyId]);
            CompanyCode = BaseUtil.ConvertToString(dr[FieldCompanyCode]);
            CompanyName = BaseUtil.ConvertToString(dr[FieldCompanyName]);
            // Service = BaseUtil.ConvertToString(dr[BaseLoginLogEntity.FieldService]);
            // ElapsedTicks = BaseUtil.ConvertToInt(dr[BaseLoginLogEntity.FieldElapsedTicks]);
            LoginStatus = BaseUtil.ConvertToString(dr[FieldLoginStatus]);
            MacAddress = BaseUtil.ConvertToString(dr[FieldMacAddress]);
            IpAddress = BaseUtil.ConvertToString(dr[FieldIpAddress]);
            IpAddressName = BaseUtil.ConvertToString(dr[FieldIpAddressName]);
            LogLevel = BaseUtil.ConvertToInt(dr[FieldLogLevel]);
            // 获取扩展属性
            GetFromExtend(dr);
            return this;
        }

        ///<summary>
        /// 系统登录日志表
        ///</summary>
        [NonSerialized]
        public const string TableName = "BaseLoginLog";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        /// <summary>
        /// 哪个服务器上运行的？
        /// machine
        /// </summary>
        public const string FieldService = "Service";

        /// <summary>
        /// 耗时
        /// </summary>
        [NonSerialized]
        public const string FieldElapsedTicks = "ElapsedTicks";

        ///<summary>
        /// 系统编号
        ///</summary>
        [NonSerialized]
        public const string FieldSystemCode = "SystemCode";

        ///<summary>
        /// 创建时间
        ///</summary>
        [NonSerialized]
        public const string FieldCreateTime = "CreateOn";

        ///<summary>
        /// 用户主键
        ///</summary>
        [NonSerialized]
        public const string FieldUserId = "UserId";

        ///<summary>
        /// 用户名
        ///</summary>
        [NonSerialized]
        public const string FieldUserName = "UserName";

        ///<summary>
        /// 昵称
        ///</summary>
        [NonSerialized]
        public const string FieldNickName = "NickName";

        ///<summary>
        /// 姓名
        ///</summary>
        [NonSerialized]
        public const string FieldRealName = "RealName";

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
        /// 登录的目标应用
        ///</summary>
        [NonSerialized]
        public const string FieldTargetApplication = "TargetApplication";

        ///<summary>
        /// 登录的目标服务器端IP
        ///</summary>
        [NonSerialized]
        public const string FieldTargetIp = "TargetIp";

        ///<summary>
        /// 发起请求的终端应用类型
        ///</summary>
        [NonSerialized]
        public const string FieldSourceType = "SourceType";
        
        ///<summary>
        /// 操作结果（Success 1/Fail 0）
        ///</summary>
        [NonSerialized]
        public const string FieldResult = "Result";
        
        ///<summary>
        /// 省、站点所在省份
        ///</summary>
        [NonSerialized]
        public const string FieldProvince = "Province";

        ///<summary>
        /// 市、站点所在城市
        ///</summary>
        [NonSerialized]
        public const string FieldCity = "City";

        ///<summary>
        /// 操作类型
        ///</summary>
        [NonSerialized]
        public const string FieldOperationType = "OperationType";

        ///<summary>
        /// 登录状态
        ///</summary>
        [NonSerialized]
        public const string FieldLoginStatus = "LoginStatus";

        ///<summary>
        /// 登录级别（0，正常；1、注意；2，危险；3、攻击）
        ///</summary>
        [NonSerialized]
        public const string FieldLogLevel = "LogLevel";

        ///<summary>
        /// MAC地址
        ///</summary>
        [NonSerialized]
        public const string FieldMacAddress = "MACAddress";

        ///<summary>
        /// IP地址
        ///</summary>
        [NonSerialized]
        public const string FieldIpAddress = "IPAddress";

        ///<summary>
        /// IP地址名称
        ///</summary>
        [NonSerialized]
        public const string FieldIpAddressName = "IPAddressName";
    }
}
