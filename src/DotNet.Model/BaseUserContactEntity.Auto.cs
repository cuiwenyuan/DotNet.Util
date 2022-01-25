//-----------------------------------------------------------------------
// <copyright file="BaseUserContactEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseUserContactEntity
    /// 用户联系方式
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
    public partial class BaseUserContactEntity : BaseEntity
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
        /// 显示邮箱
        /// </summary>
        [FieldDescription("显示邮箱")]
        public int ShowEmail { get; set; } = 1;

        /// <summary>
        /// 邮箱
        /// </summary>
        [FieldDescription("邮箱")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 邮箱是否验证
        /// </summary>
        [FieldDescription("邮箱是否验证")]
        public int EmailValidated { get; set; } = 0;

        /// <summary>
        /// 显示手机
        /// </summary>
        [FieldDescription("显示手机")]
        public int? ShowMobile { get; set; } = 1;

        /// <summary>
        /// 手机
        /// </summary>
        [FieldDescription("手机")]
        public string Mobile { get; set; } = string.Empty;

        /// <summary>
        /// 手机是否验证
        /// </summary>
        [FieldDescription("手机是否验证")]
        public int MobileValidated { get; set; } = 0;

        /// <summary>
        /// 手机验证时间
        /// </summary>
        [FieldDescription("手机验证时间")]
        public DateTime? MobileValidatedTime { get; set; } = null;

        /// <summary>
        /// 短号
        /// </summary>
        [FieldDescription("短号")]
        public string ShortNumber { get; set; } = string.Empty;

        /// <summary>
        /// 电话
        /// </summary>
        [FieldDescription("电话")]
        public string Telephone { get; set; } = string.Empty;

        /// <summary>
        /// 分机
        /// </summary>
        [FieldDescription("分机")]
        public string Extension { get; set; } = string.Empty;

        /// <summary>
        /// QQ
        /// </summary>
        [FieldDescription("QQ")]
        public string Qq { get; set; } = string.Empty;

        /// <summary>
        /// 旺旺
        /// </summary>
        [FieldDescription("旺旺")]
        public string Ww { get; set; } = string.Empty;

        /// <summary>
        /// 即时通讯
        /// </summary>
        [FieldDescription("即时通讯")]
        public string Im { get; set; } = string.Empty;

        /// <summary>
        /// 微信
        /// </summary>
        [FieldDescription("微信")]
        public string WeChat { get; set; } = string.Empty;

        /// <summary>
        /// 微信是否验证
        /// </summary>
        [FieldDescription("微信是否验证")]
        public int WeChatValidated { get; set; } = 0;

        /// <summary>
        /// 微信OpenId
        /// </summary>
        [FieldDescription("微信OpenId")]
        public string WeChatOpenId { get; set; } = string.Empty;

        /// <summary>
        /// 公司主键
        /// </summary>
        [FieldDescription("公司主键")]
        public int CompanyId { get; set; } = 0;

        /// <summary>
        /// 公司邮箱
        /// </summary>
        [FieldDescription("公司邮箱")]
        public string CompanyEmail { get; set; } = string.Empty;

        /// <summary>
        /// 紧急联系人
        /// </summary>
        [FieldDescription("紧急联系人")]
        public string EmergencyContact { get; set; } = string.Empty;

        /// <summary>
        /// 紧急联系手机
        /// </summary>
        [FieldDescription("紧急联系手机")]
        public string EmergencyMobile { get; set; } = string.Empty;

        /// <summary>
        /// 紧急联系电话
        /// </summary>
        [FieldDescription("紧急联系电话")]
        public string EmergencyTelephone { get; set; } = string.Empty;

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
        /// 用户联系方式
        ///</summary>
        [FieldDescription("用户联系方式")]
        public const string CurrentTableName = "BaseUserContact";

        ///<summary>
        /// 主键
        ///</summary>
        public const string FieldId = "Id";

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
