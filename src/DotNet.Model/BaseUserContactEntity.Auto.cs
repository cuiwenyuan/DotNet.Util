//-----------------------------------------------------------------------
// <copyright file="BaseUserContactEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2023, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseUserContactEntity
    /// 用户联系方式
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
    public partial class BaseUserContactEntity : BaseEntity
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        [FieldDescription("用户编号")]
        [Description("用户编号")]
        [Column(FieldUserId)]
        public int UserId { get; set; } = 0;

        /// <summary>
        /// 显示邮箱
        /// </summary>
        [FieldDescription("显示邮箱")]
        [Description("显示邮箱")]
        [Column(FieldShowEmail)]
        public int ShowEmail { get; set; } = 1;

        /// <summary>
        /// 邮箱
        /// </summary>
        [FieldDescription("邮箱")]
        [Description("邮箱")]
        [Column(FieldEmail)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 邮箱是否验证
        /// </summary>
        [FieldDescription("邮箱是否验证")]
        [Description("邮箱是否验证")]
        [Column(FieldEmailValidated)]
        public int EmailValidated { get; set; } = 0;

        /// <summary>
        /// 显示手机
        /// </summary>
        [FieldDescription("显示手机")]
        [Description("显示手机")]
        [Column(FieldShowMobile)]
        public int? ShowMobile { get; set; } = 1;

        /// <summary>
        /// 手机
        /// </summary>
        [FieldDescription("手机")]
        [Description("手机")]
        [Column(FieldMobile)]
        public string Mobile { get; set; } = string.Empty;

        /// <summary>
        /// 手机是否验证
        /// </summary>
        [FieldDescription("手机是否验证")]
        [Description("手机是否验证")]
        [Column(FieldMobileValidated)]
        public int MobileValidated { get; set; } = 0;

        /// <summary>
        /// 手机验证时间
        /// </summary>
        [FieldDescription("手机验证时间")]
        [Description("手机验证时间")]
        [Column(FieldMobileValidatedTime)]
        public DateTime? MobileValidatedTime { get; set; } = null;

        /// <summary>
        /// 短号
        /// </summary>
        [FieldDescription("短号")]
        [Description("短号")]
        [Column(FieldShortNumber)]
        public string ShortNumber { get; set; } = string.Empty;

        /// <summary>
        /// 电话
        /// </summary>
        [FieldDescription("电话")]
        [Description("电话")]
        [Column(FieldTelephone)]
        public string Telephone { get; set; } = string.Empty;

        /// <summary>
        /// 分机
        /// </summary>
        [FieldDescription("分机")]
        [Description("分机")]
        [Column(FieldExtension)]
        public string Extension { get; set; } = string.Empty;

        /// <summary>
        /// QQ
        /// </summary>
        [FieldDescription("QQ")]
        [Description("QQ")]
        [Column(FieldQq)]
        public string Qq { get; set; } = string.Empty;

        /// <summary>
        /// 旺旺
        /// </summary>
        [FieldDescription("旺旺")]
        [Description("旺旺")]
        [Column(FieldWw)]
        public string Ww { get; set; } = string.Empty;

        /// <summary>
        /// 即时通讯
        /// </summary>
        [FieldDescription("即时通讯")]
        [Description("即时通讯")]
        [Column(FieldIm)]
        public string Im { get; set; } = string.Empty;

        /// <summary>
        /// 微信
        /// </summary>
        [FieldDescription("微信")]
        [Description("微信")]
        [Column(FieldWeChat)]
        public string WeChat { get; set; } = string.Empty;

        /// <summary>
        /// 微信是否验证
        /// </summary>
        [FieldDescription("微信是否验证")]
        [Description("微信是否验证")]
        [Column(FieldWeChatValidated)]
        public int WeChatValidated { get; set; } = 0;

        /// <summary>
        /// 微信OpenId
        /// </summary>
        [FieldDescription("微信OpenId")]
        [Description("微信OpenId")]
        [Column(FieldWeChatOpenId)]
        public string WeChatOpenId { get; set; } = string.Empty;

        /// <summary>
        /// 公司主键
        /// </summary>
        [FieldDescription("公司主键")]
        [Description("公司主键")]
        [Column(FieldCompanyId)]
        public int CompanyId { get; set; } = 0;

        /// <summary>
        /// 公司邮箱
        /// </summary>
        [FieldDescription("公司邮箱")]
        [Description("公司邮箱")]
        [Column(FieldCompanyEmail)]
        public string CompanyEmail { get; set; } = string.Empty;

        /// <summary>
        /// 紧急联系人
        /// </summary>
        [FieldDescription("紧急联系人")]
        [Description("紧急联系人")]
        [Column(FieldEmergencyContact)]
        public string EmergencyContact { get; set; } = string.Empty;

        /// <summary>
        /// 紧急联系手机
        /// </summary>
        [FieldDescription("紧急联系手机")]
        [Description("紧急联系手机")]
        [Column(FieldEmergencyMobile)]
        public string EmergencyMobile { get; set; } = string.Empty;

        /// <summary>
        /// 紧急联系电话
        /// </summary>
        [FieldDescription("紧急联系电话")]
        [Description("紧急联系电话")]
        [Column(FieldEmergencyTelephone)]
        public string EmergencyTelephone { get; set; } = string.Empty;

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
            if (dr.ContainsColumn(FieldShowEmail))
            {
                ShowEmail = BaseUtil.ConvertToInt(dr[FieldShowEmail]);
            }
            if (dr.ContainsColumn(FieldEmail))
            {
                Email = BaseUtil.ConvertToString(dr[FieldEmail]);
            }
            if (dr.ContainsColumn(FieldEmailValidated))
            {
                EmailValidated = BaseUtil.ConvertToInt(dr[FieldEmailValidated]);
            }
            if (dr.ContainsColumn(FieldShowMobile))
            {
                ShowMobile = BaseUtil.ConvertToNullableByteInt(dr[FieldShowMobile]);
            }
            if (dr.ContainsColumn(FieldMobile))
            {
                Mobile = BaseUtil.ConvertToString(dr[FieldMobile]);
            }
            if (dr.ContainsColumn(FieldMobileValidated))
            {
                MobileValidated = BaseUtil.ConvertToInt(dr[FieldMobileValidated]);
            }
            if (dr.ContainsColumn(FieldMobileValidatedTime))
            {
                MobileValidatedTime = BaseUtil.ConvertToNullableDateTime(dr[FieldMobileValidatedTime]);
            }
            if (dr.ContainsColumn(FieldShortNumber))
            {
                ShortNumber = BaseUtil.ConvertToString(dr[FieldShortNumber]);
            }
            if (dr.ContainsColumn(FieldTelephone))
            {
                Telephone = BaseUtil.ConvertToString(dr[FieldTelephone]);
            }
            if (dr.ContainsColumn(FieldExtension))
            {
                Extension = BaseUtil.ConvertToString(dr[FieldExtension]);
            }
            if (dr.ContainsColumn(FieldQq))
            {
                Qq = BaseUtil.ConvertToString(dr[FieldQq]);
            }
            if (dr.ContainsColumn(FieldWw))
            {
                Ww = BaseUtil.ConvertToString(dr[FieldWw]);
            }
            if (dr.ContainsColumn(FieldIm))
            {
                Im = BaseUtil.ConvertToString(dr[FieldIm]);
            }
            if (dr.ContainsColumn(FieldWeChat))
            {
                WeChat = BaseUtil.ConvertToString(dr[FieldWeChat]);
            }
            if (dr.ContainsColumn(FieldWeChatValidated))
            {
                WeChatValidated = BaseUtil.ConvertToInt(dr[FieldWeChatValidated]);
            }
            if (dr.ContainsColumn(FieldWeChatOpenId))
            {
                WeChatOpenId = BaseUtil.ConvertToString(dr[FieldWeChatOpenId]);
            }
            if (dr.ContainsColumn(FieldCompanyId))
            {
                CompanyId = BaseUtil.ConvertToInt(dr[FieldCompanyId]);
            }
            if (dr.ContainsColumn(FieldCompanyEmail))
            {
                CompanyEmail = BaseUtil.ConvertToString(dr[FieldCompanyEmail]);
            }
            if (dr.ContainsColumn(FieldEmergencyContact))
            {
                EmergencyContact = BaseUtil.ConvertToString(dr[FieldEmergencyContact]);
            }
            if (dr.ContainsColumn(FieldEmergencyMobile))
            {
                EmergencyMobile = BaseUtil.ConvertToString(dr[FieldEmergencyMobile]);
            }
            if (dr.ContainsColumn(FieldEmergencyTelephone))
            {
                EmergencyTelephone = BaseUtil.ConvertToString(dr[FieldEmergencyTelephone]);
            }
            return this;
        }

        ///<summary>
        /// 用户联系方式
        ///</summary>
        [FieldDescription("用户联系方式")]
        public const string CurrentTableName = "BaseUserContact";

        ///<summary>
        /// 用户编号
        ///</summary>
        public const string FieldUserId = "UserId";

        ///<summary>
        /// 显示邮箱
        ///</summary>
        public const string FieldShowEmail = "ShowEmail";

        ///<summary>
        /// 邮箱
        ///</summary>
        public const string FieldEmail = "Email";

        ///<summary>
        /// 邮箱是否验证
        ///</summary>
        public const string FieldEmailValidated = "EmailValidated";

        ///<summary>
        /// 显示手机
        ///</summary>
        public const string FieldShowMobile = "ShowMobile";

        ///<summary>
        /// 手机
        ///</summary>
        public const string FieldMobile = "Mobile";

        ///<summary>
        /// 手机是否验证
        ///</summary>
        public const string FieldMobileValidated = "MobileValidated";

        ///<summary>
        /// 手机验证时间
        ///</summary>
        public const string FieldMobileValidatedTime = "MobileValidatedTime";

        ///<summary>
        /// 短号
        ///</summary>
        public const string FieldShortNumber = "ShortNumber";

        ///<summary>
        /// 电话
        ///</summary>
        public const string FieldTelephone = "Telephone";

        ///<summary>
        /// 分机
        ///</summary>
        public const string FieldExtension = "Extension";

        ///<summary>
        /// QQ
        ///</summary>
        public const string FieldQq = "QQ";

        ///<summary>
        /// 旺旺
        ///</summary>
        public const string FieldWw = "WW";

        ///<summary>
        /// 即时通讯
        ///</summary>
        public const string FieldIm = "IM";

        ///<summary>
        /// 微信
        ///</summary>
        public const string FieldWeChat = "WeChat";

        ///<summary>
        /// 微信是否验证
        ///</summary>
        public const string FieldWeChatValidated = "WeChatValidated";

        ///<summary>
        /// 微信OpenId
        ///</summary>
        public const string FieldWeChatOpenId = "WeChatOpenId";

        ///<summary>
        /// 公司主键
        ///</summary>
        public const string FieldCompanyId = "CompanyId";

        ///<summary>
        /// 公司邮箱
        ///</summary>
        public const string FieldCompanyEmail = "CompanyEmail";

        ///<summary>
        /// 紧急联系人
        ///</summary>
        public const string FieldEmergencyContact = "EmergencyContact";

        ///<summary>
        /// 紧急联系手机
        ///</summary>
        public const string FieldEmergencyMobile = "EmergencyMobile";

        ///<summary>
        /// 紧急联系电话
        ///</summary>
        public const string FieldEmergencyTelephone = "EmergencyTelephone";
    }
}
