//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------



namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseAreaEntity
    /// 登录提醒表
    ///
    /// 修改记录
    ///
    ///		2015-01-23 版本：1.0 SongBiao 创建主键。
    ///
    /// <author>
    ///		<name>SongBiao</name>
    ///		<date>2015-01-23</date>
    /// </author>
    /// </summary>
    /// <summary>
    /// BaseUserLogonExtendEntity
    /// 用户登录的扩展表，账号登录方式，登录提醒方式
    /// 
    /// 修改记录
    /// 
    /// 2015-01-23 版本：1.0 JiRiGaLa 创建文件。
    /// 
    /// <author>
    ///     <name>JiRiGaLa</name>
    ///     <date>2015-01-23</date>
    /// </author>
    /// </summary>
    public partial class BaseUserLogonExtendEntity : BaseEntity
    {
        /// <summary>
        /// 主键 用户ID
        /// </summary>
        [FieldDescription("主键", false)]
        public decimal Id { get; set; }

        /// <summary>
        /// 登录邮件提醒
        /// </summary>
        [FieldDescription("登录邮件提醒")]
        public decimal? EmailRemind { get; set; } = null;

        /// <summary>
        /// 二维码登录
        /// </summary>
        [FieldDescription("二维码登录")]
        public decimal? QrCodeLogon { get; set; } = null;

        /// <summary>
        /// 登录吉信提醒
        /// </summary>
        [FieldDescription("登录吉信提醒")]
        public decimal? JixinRemind { get; set; } = null;

        /// <summary>
        /// 登录微信提醒
        /// </summary>
        [FieldDescription("登录微信提醒")]
        public decimal? WechatRemind { get; set; } = null;

        /// <summary>
        /// 动态码登录
        /// </summary>
        [FieldDescription("动态码登录")]
        public decimal? DynamicCodeLogon { get; set; } = null;

        /// <summary>
        /// 登录手机短信提醒
        /// </summary>
        [FieldDescription("登录手机短信提醒")]
        public decimal? MobileRemind { get; set; } = null;

        /// <summary>
        /// 用户名密码方式登录
        /// </summary>
        [FieldDescription("用户名密码方式登录")]
        public decimal? UsernamePasswordLogon { get; set; } = null;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            Id = BaseUtil.ConvertToDecimal(dr[FieldId]);
            EmailRemind = BaseUtil.ConvertToNullableDecimal(dr[FieldEmailRemind]);
            QrCodeLogon = BaseUtil.ConvertToNullableDecimal(dr[FieldQrCodeLogon]);
            JixinRemind = BaseUtil.ConvertToNullableDecimal(dr[FieldJixinRemind]);
            WechatRemind = BaseUtil.ConvertToNullableDecimal(dr[FieldWechatRemind]);
            DynamicCodeLogon = BaseUtil.ConvertToNullableDecimal(dr[FieldDynamicCodeLogon]);
            MobileRemind = BaseUtil.ConvertToNullableDecimal(dr[FieldMobileRemind]);
            UsernamePasswordLogon = BaseUtil.ConvertToNullableDecimal(dr[FieldUsernamePasswordLogon]);
            // 获取扩展属性
            GetFromExtend(dr);
            return this;
        }

        ///<summary>
        /// 用户登录的扩展表，账号登录方式，登录提醒方式
        ///</summary>
        [FieldDescription("用户登录扩展表")]
        public const string TableName = "BASE_USER_LOGON_EXTEND";

        ///<summary>
        /// 主键 用户ID
        ///</summary>
        public const string FieldId = "Id";

        ///<summary>
        /// 登录邮件提醒
        ///</summary>
        public const string FieldEmailRemind = "EMAIL_REMIND";

        ///<summary>
        /// 二维码登录
        ///</summary>
        public const string FieldQrCodeLogon = "QR_CODE_LOGON";

        ///<summary>
        /// 登录吉信提醒
        ///</summary>
        public const string FieldJixinRemind = "JIXIN_REMIND";

        ///<summary>
        /// 登录微信提醒
        ///</summary>
        public const string FieldWechatRemind = "WECHAT_REMIND";

        ///<summary>
        /// 动态码登录
        ///</summary>
        public const string FieldDynamicCodeLogon = "DYNAMIC_CODE_LOGON";

        ///<summary>
        /// 登录手机短信提醒
        ///</summary>
        public const string FieldMobileRemind = "MOBILE_REMIND";

        ///<summary>
        /// 用户名密码方式登录
        ///</summary>
        public const string FieldUsernamePasswordLogon = "USERNAME_PASSWORD_LOGON";
    }
}