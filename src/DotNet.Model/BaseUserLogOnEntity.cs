//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

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
    ///		2015-07-06 版本：2.1 JiRiGaLa 需要修改密码 NeedModifyPassword 的功能实现。
    ///		2014-06-26 版本：2.0 JiRiGaLa 密码盐、OpenId过期时间设置。
    ///		2013-04-21 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015-07-06</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseUserLogonEntity : BaseEntity
    {
        /// <summary>
        /// 用户登录实体
        /// </summary>
        public BaseUserLogonEntity()
        {
            CreateOn = DateTime.Now;
            ModifiedOn = DateTime.Now;
        }
        /// <summary>
        /// 主键
        /// </summary>
        [FieldDescription("主键", false)]
        public string Id { get; set; } = null;

        /// <summary>
        /// 公司主键
        /// </summary>
        [FieldDescription("公司主键")]
        public string CompanyId { get; set; } = null;

        /// <summary>
        /// 用户密码
        /// </summary>
        [FieldDescription("用户密码", false)]
        public string UserPassword { get; set; } = null;

        /// <summary>
        /// 密码盐
        /// </summary>
        [FieldDescription("密码盐", false)]
        public string Salt { get; set; } = string.Empty;

        /// <summary>
        /// 当点登录标示
        /// </summary>
        [FieldDescription("当点登录标示", false)]
        public string OpenId { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// OpenId过期时间
        /// </summary>
        [FieldDescription("OpenId过期时间", false)]
        public DateTime? OpenIdTimeout { get; set; } = null;

        /// <summary>
        /// 系统编号
        /// </summary>
        [FieldDescription("系统编号", false)]
        public string SystemCode { get; set; } = string.Empty;

        /// <summary>
        /// 验证码
        /// </summary>
        [FieldDescription("验证码", false)]
        public string VerificationCode { get; set; } = null;

        /// <summary>
        /// 最后修改密码日期
        /// </summary>
        [FieldDescription("最后修改密码日期", false)]
        public DateTime? ChangePasswordDate { get; set; } = null;

        /// <summary>
        /// 允许登录时间开始
        /// </summary>
        [FieldDescription("允许登录时间开始", false)]
        public DateTime? AllowStartTime { get; set; } = null;

        /// <summary>
        /// 允许登录时间结束
        /// </summary>
        [FieldDescription("允许登录时间结束", false)]
        public DateTime? AllowEndTime { get; set; } = null;

        /// <summary>
        /// 暂停用户开始日期
        /// </summary>
        [FieldDescription("暂停用户开始日期", false)]
        public DateTime? LockStartDate { get; set; } = null;

        /// <summary>
        /// 暂停用户结束日期
        /// </summary>
        [FieldDescription("暂停用户结束日期", false)]
        public DateTime? LockEndDate { get; set; } = null;

        /// <summary>
        /// 第一次访问时间
        /// </summary>
        [FieldDescription("第一次访问时间", false)]
        public DateTime? FirstVisit { get; set; } = null;

        /// <summary>
        /// 上一次访问时间
        /// </summary>
        [FieldDescription("上一次访问时间", false)]
        public DateTime? PreviousVisit { get; set; } = null;

        /// <summary>
        /// 最后访问时间
        /// </summary>
        [FieldDescription("最后访问时间", false)]
        public DateTime? LastVisit { get; set; } = null;

        /// <summary>
        /// 允许有多用户同时登录
        /// </summary>
        [FieldDescription("允许有多用户同时登录")]
        public int? MultiUserLogin { get; set; } = 1;

        /// <summary>
        /// 访问限制
        /// </summary>
        [FieldDescription("访问限制")]
        public int? CheckIpAddress { get; set; } = 0;

        /// <summary>
        /// 登录次数
        /// </summary>
        [FieldDescription("登录次数", false)]
        public int? LogonCount { get; set; } = 0;

        /// <summary>
        /// 展示次数
        /// </summary>
        [FieldDescription("展示次数", false)]
        public int? ShowCount { get; set; } = 0;

        /// <summary>
        /// 密码连续错误次数
        /// </summary>
        [FieldDescription("密码连续错误次数", false)]
        public int? PasswordErrorCount { get; set; } = 0;

        /// <summary>
        /// 在线状态
        /// </summary>
        [FieldDescription("在线状态", false)]
        public int? UserOnline { get; set; } = 0;

        /// <summary>
        /// IP地址
        /// </summary>
        [FieldDescription("IP地址")]
        public string IpAddress { get; set; } = null;

        /// <summary>
        /// IP地址名称
        /// </summary>
        [FieldDescription("IP地址名称")]
        public string IpAddressName { get; set; } = string.Empty;

        /// <summary>
        /// 计算机名称
        /// </summary>
        [FieldDescription("计算机名称")]
        public string ComputerName { get; set; } = null;

        /// <summary>
        /// MAC地址
        /// </summary>
        [FieldDescription("MAC地址")]
        public string MacAddress { get; set; } = null;

        /// <summary>
        /// 密码提示问题
        /// </summary>
        [FieldDescription("密码提示问题")]
        public string Question { get; set; } = null;

        /// <summary>
        /// 密码提示答案
        /// </summary>
        [FieldDescription("密码提示答案")]
        public string AnswerQuestion { get; set; } = null;

        /// <summary>
        /// 密码强度
        /// </summary>
        [FieldDescription("密码强度", false)]
        public decimal? PasswordStrength { get; set; } = -1;

        /// <summary>
        /// 有效
        /// </summary>
        [FieldDescription("需修改密码")]
        public int NeedModifyPassword { get; set; } = 0;

        /// <summary>
        /// 有效
        /// </summary>
        [FieldDescription("是否有效")]
        public int Enabled { get; set; } = 1;

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


        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            Id = BaseUtil.ConvertToString(dr[FieldId]);
            // 2016-03-02 吉日嘎拉 防止程序出错，没有这个字段也可以正常运行
            if (dr.ContainsColumn(FieldCompanyId))
            {
                CompanyId = BaseUtil.ConvertToString(dr[FieldCompanyId]);
            }
            ChangePasswordDate = BaseUtil.ConvertToNullableDateTime(dr[FieldChangePasswordDate]);
            UserPassword = BaseUtil.ConvertToString(dr[FieldUserPassword]);
            Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            OpenId = BaseUtil.ConvertToString(dr[FieldOpenId]);
            OpenIdTimeout = BaseUtil.ConvertToNullableDateTime(dr[FieldOpenIdTimeout]);
            Salt = BaseUtil.ConvertToString(dr[FieldSalt]);
            /*
			CommunicationPassword = BaseUtil.ConvertToString(dr[BaseUserLogonEntity.FieldCommunicationPassword]);
			SignedPassword = BaseUtil.ConvertToString(dr[BaseUserLogonEntity.FieldSignedPassword]);
			PublicKey = BaseUtil.ConvertToString(dr[BaseUserLogonEntity.FieldPublicKey]);
			*/
            AllowStartTime = BaseUtil.ConvertToNullableDateTime(dr[FieldAllowStartTime]);
            AllowEndTime = BaseUtil.ConvertToNullableDateTime(dr[FieldAllowEndTime]);
            SystemCode = BaseUtil.ConvertToString(dr[FieldSystemCode]);
            LockStartDate = BaseUtil.ConvertToNullableDateTime(dr[FieldLockStartDate]);
            LockEndDate = BaseUtil.ConvertToNullableDateTime(dr[FieldLockEndDate]);
            FirstVisit = BaseUtil.ConvertToNullableDateTime(dr[FieldFirstVisit]);
            PreviousVisit = BaseUtil.ConvertToNullableDateTime(dr[FieldPreviousVisit]);
            LastVisit = BaseUtil.ConvertToNullableDateTime(dr[FieldLastVisit]);
            MultiUserLogin = BaseUtil.ConvertToInt(dr[FieldMultiUserLogin]);
            CheckIpAddress = BaseUtil.ConvertToInt(dr[FieldCheckIpAddress]);
            LogonCount = BaseUtil.ConvertToInt(dr[FieldLogonCount]);
            ShowCount = BaseUtil.ConvertToInt(dr[FieldShowCount]);
            PasswordErrorCount = BaseUtil.ConvertToInt(dr[FieldPasswordErrorCount]);
            UserOnline = BaseUtil.ConvertToInt(dr[FieldUserOnline]);
            IpAddress = BaseUtil.ConvertToString(dr[FieldIpAddress]);
            IpAddressName = BaseUtil.ConvertToString(dr[FieldIpAddressName]);
            MacAddress = BaseUtil.ConvertToString(dr[FieldMacAddress]);
            ComputerName = BaseUtil.ConvertToString(dr[FieldComputerName]);
            Question = BaseUtil.ConvertToString(dr[FieldQuestion]);
            AnswerQuestion = BaseUtil.ConvertToString(dr[FieldAnswerQuestion]);
            PasswordStrength = BaseUtil.ConvertToDecimal(dr[FieldPasswordStrength]);
            NeedModifyPassword = BaseUtil.ConvertToInt(dr[FieldNeedModifyPassword]);
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
        /// 系统用户账户表
        ///</summary>
        [NonSerialized]
        [FieldDescription("用户登录信息表")]
        public const string TableName = "BaseUserLogon";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 公司主键
        ///</summary>
        [NonSerialized]
        public const string FieldCompanyId = "CompanyId";

        ///<summary>
        /// 启用状态
        ///</summary>
        [NonSerialized]
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 用户密码
        ///</summary>
        [NonSerialized]
        public const string FieldUserPassword = "UserPassword";

        ///<summary>
        /// 验证码
        ///</summary>
        [NonSerialized]
        public const string FieldVerificationCode = "VerificationCode";

        ///<summary>
        /// 最后修改密码日期
        ///</summary>
        [NonSerialized]
        public const string FieldChangePasswordDate = "ChangePasswordDate";

        ///<summary>
        /// 允许登录时间开始
        ///</summary>
        [NonSerialized]
        public const string FieldAllowStartTime = "AllowStartTime";

        ///<summary>
        /// 允许登录时间结束
        ///</summary>
        [NonSerialized]
        public const string FieldAllowEndTime = "AllowEndTime";

        ///<summary>
        /// 暂停用户开始日期
        ///</summary>
        [NonSerialized]
        public const string FieldLockStartDate = "LockStartDate";

        ///<summary>
        /// 暂停用户结束日期
        ///</summary>
        [NonSerialized]
        public const string FieldLockEndDate = "LockEndDate";

        ///<summary>
        /// 系统编号
        ///</summary>
        [NonSerialized]
        public const string FieldSystemCode = "SystemCode";

        ///<summary>
        /// 第一次访问时间
        ///</summary>
        [NonSerialized]
        public const string FieldFirstVisit = "FirstVisit";

        ///<summary>
        /// 上一次访问时间
        ///</summary>
        [NonSerialized]
        public const string FieldPreviousVisit = "PreviousVisit";

        ///<summary>
        /// 最后访问时间
        ///</summary>
        [NonSerialized]
        public const string FieldLastVisit = "LastVisit";

        ///<summary>
        /// 允许同时有多用户登录
        ///</summary>
        [NonSerialized]
        public const string FieldMultiUserLogin = "MultiUserLogin";

        ///<summary>
        /// 登录次数
        ///</summary>
        [NonSerialized]
        public const string FieldLogonCount = "LogonCount";

        ///<summary>
        /// 展示次数
        ///</summary>
        [NonSerialized]
        public const string FieldShowCount = "ShowCount";

        ///<summary>
        /// 密码连续错误次数
        ///</summary>
        [NonSerialized]
        public const string FieldPasswordErrorCount = "PasswordErrorCount";

        ///<summary>
        /// 在线状态
        ///</summary>
        [NonSerialized]
        public const string FieldUserOnline = "UserOnline";

        ///<summary>
        /// 当点登录标示
        ///</summary>
        [NonSerialized]
        public const string FieldOpenId = "OpenId";

        ///<summary>
        /// OpenId超时时间
        ///</summary>
        [NonSerialized]
        public const string FieldOpenIdTimeout = "OpenIdTimeout";

        ///<summary>
        /// 密码盐
        ///</summary>
        [NonSerialized]
        public const string FieldSalt = "Salt";

        ///<summary>
        /// IP访问限制
        ///</summary>
        [NonSerialized]
        public const string FieldCheckIpAddress = "CheckIPAddress";

        ///<summary>
        /// 登录IP地址
        ///</summary>
        [NonSerialized]
        public const string FieldIpAddress = "IPAddress";

        ///<summary>
        /// IP地址名称
        ///</summary>
        [NonSerialized]
        public const string FieldIpAddressName = "IPAddressName";

        ///<summary>
        /// 登录MAC地址
        ///</summary>
        [NonSerialized]
        public const string FieldMacAddress = "MACAddress";

        /// <summary>
        /// 计算机名称
        /// </summary>
        [NonSerialized]
        public const string FieldComputerName = "ComputerName";

        ///<summary>
        /// 密码提示问题代码
        ///</summary>
        [NonSerialized]
        public const string FieldQuestion = "Question";

        ///<summary>
        /// 密码提示答案
        ///</summary>
        [NonSerialized]
        public const string FieldAnswerQuestion = "AnswerQuestion";

        /*

		///<summary>
		/// 通讯密码
		///</summary>
		[NonSerialized]
		public const string FieldCommunicationPassword = "CommunicationPassword";

		///<summary>
		/// 数字签名密码
		///</summary>
		[NonSerialized]
		public const string FieldSignedPassword = "SignedPassword";

		///<summary>
		/// 公钥
		///</summary>
		[NonSerialized]
		public const string FieldPublicKey = "PublicKey";
		
		*/

        ///<summary>
        /// 需要修改密码
        ///</summary>
        [NonSerialized]
        public const string FieldNeedModifyPassword = "NeedModifyPassword";

        ///<summary>
        /// 密码强度级别
        ///</summary>
        [NonSerialized]
        public const string FieldPasswordStrength = "PasswordStrength";

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
