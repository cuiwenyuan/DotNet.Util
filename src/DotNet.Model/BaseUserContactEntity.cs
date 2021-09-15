//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseUserContactEntity
    /// 系统用户联系方式表
    ///
    /// 修改记录
    ///
    ///		2014-01-13 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2014-01-13</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseUserContactEntity : BaseEntity
    {
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
        /// 手机
        /// </summary>
        [FieldDescription("手机")]
        public string Mobile { get; set; } = string.Empty;

        /// <summary>
        /// 手机验证通过
        /// </summary>
        [FieldDescription("手机验证通过")]
        public int MobileValiated { get; set; } = 0;

        /// <summary>
        /// 手机验证日期
        /// </summary>
        [FieldDescription("手机验证日期")]
        public DateTime? MobileVerificationDate { get; set; } = null;

        /// <summary>
        /// 显示手机号码
        /// </summary>
        [FieldDescription("显示手机号码")]
        public int ShowMobile { get; set; } = 1;

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
        /// 微信识别码
        /// </summary>
        [FieldDescription("微信识别码")]
        public string WeChatOpenId { get; set; } = string.Empty;

        /// <summary>
        /// 微信号码验证通过
        /// </summary>
        [FieldDescription("微信号码验证通过")]
        public int WeChatValiated { get; set; } = 0;

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
        /// 电子邮箱验证通过
        /// </summary>
        [FieldDescription("电子邮箱验证通过")]
        public int EmailValiated { get; set; } = 0;

        /// <summary>
        /// 公司邮件
        /// </summary>
        [FieldDescription("电子邮件")]
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
            Id = BaseUtil.ConvertToString(dr[BaseUserEntity.FieldId]);
            // 2016-03-02 吉日嘎拉 防止程序出错，没有这个字段也可以正常运行
            if (dr.ContainsColumn(BaseUserLogOnEntity.FieldCompanyId))
            {
                CompanyId = BaseUtil.ConvertToString(dr[BaseUserLogOnEntity.FieldCompanyId]);
            }
            Mobile = BaseUtil.ConvertToString(dr[FieldMobile]);
            MobileValiated = BaseUtil.ConvertToInt(dr[FieldMobileValiated]);
            MobileVerificationDate = BaseUtil.ConvertToNullableDateTime(dr[FieldMobileVerificationDate]);
            ShowMobile = BaseUtil.ConvertToInt(dr[FieldShowMobile]);
            Telephone = BaseUtil.ConvertToString(dr[FieldTelephone]);
            Extension = BaseUtil.ConvertToString(dr[FieldExtension]);
            ShortNumber = BaseUtil.ConvertToString(dr[FieldShortNumber]);
            Ww = BaseUtil.ConvertToString(dr[FieldWw]);
            Qq = BaseUtil.ConvertToString(dr[FieldQq]);
            WeChat = BaseUtil.ConvertToString(dr[FieldWeChat]);
            WeChatOpenId = BaseUtil.ConvertToString(dr[FieldWeChatOpenId]);
            WeChatValiated = BaseUtil.ConvertToInt(dr[FieldWeChatValiated]);
            YiXin = BaseUtil.ConvertToString(dr[FieldYiXin]);
            YiXinValiated = BaseUtil.ConvertToInt(dr[FieldYiXinValiated]);
            Email = BaseUtil.ConvertToString(dr[FieldEmail]);
            EmailValiated = BaseUtil.ConvertToInt(dr[FieldEmailValiated]);
            CompanyMail = BaseUtil.ConvertToString(dr[FieldCompanyMail]);
            EmergencyContact = BaseUtil.ConvertToString(dr[FieldEmergencyContact]);
            Yy = BaseUtil.ConvertToString(dr[FieldYy]);
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
        /// 用户联系方式表
        ///</summary>
        [NonSerialized]
        [FieldDescription("用户联系方式表")]
        public const string TableName = "BaseUserContact";

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
        /// 手机
        ///</summary>
        [NonSerialized]
        public const string FieldMobile = "Mobile";

        ///<summary>
        /// 分机号码
        ///</summary>
        [NonSerialized]
        public const string FieldExtension = "Extension";

        /// <summary>
        /// 手机是否验证通过
        /// </summary>
        [NonSerialized]
        public const string FieldMobileValiated = "MobileValiated";

        /// <summary>
        /// 手机验证日期
        /// </summary>
        [NonSerialized]
        public const string FieldMobileVerificationDate = "MobileVerificationDate";

        /// <summary>
        /// 显示手机号码
        /// </summary>
        [NonSerialized]
        public const string FieldShowMobile = "ShowMobile";

        ///<summary>
        /// 电话号码
        ///</summary>
        [NonSerialized]
        public const string FieldTelephone = "Telephone";

        ///<summary>
        /// 短号
        ///</summary>
        [NonSerialized]
        public const string FieldShortNumber = "ShortNumber";

        ///<summary>
        /// 旺旺号码
        ///</summary>
        [NonSerialized]
        public const string FieldWw = "WW";

        ///<summary>
        /// QQ号码
        ///</summary>
        [NonSerialized]
        public const string FieldQq = "QQ";

        /// <summary>
        /// 微信号码
        /// </summary>
        [NonSerialized]
        public const string FieldWeChat = "WeChat";

        /// <summary>
        /// 微信识别码
        /// </summary>
        [NonSerialized]
        public const string FieldWeChatOpenId = "WeChat_OpenId";

        /// <summary>
        /// 微信是否验证通过
        /// </summary>
        [NonSerialized]
        public const string FieldWeChatValiated = "WeChatValiated";

        /// <summary>
        /// YY号码
        /// </summary>
        [NonSerialized]
        public const string FieldYy = "YY";

        /// <summary>
        /// 易信号码
        /// </summary>
        [NonSerialized]
        public const string FieldYiXin = "YiXin";

        /// <summary>
        /// 易信是否验证通过
        /// </summary>
        [NonSerialized]
        public const string FieldYiXinValiated = "YiXinValiated";

        ///<summary>
        /// 公司邮件
        ///</summary>
        [NonSerialized]
        public const string FieldCompanyMail = "CompanyMail";

        ///<summary>
        /// 紧急联系
        ///</summary>
        [NonSerialized]
        public const string FieldEmergencyContact = "EmergencyContact";

        ///<summary>
        /// 电子邮件
        ///</summary>
        [NonSerialized]
        public const string FieldEmail = "Email";

        /// <summary>
        /// 电子邮箱是否验证通过
        /// </summary>
        [NonSerialized]
        public const string FieldEmailValiated = "EmailValiated";

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
