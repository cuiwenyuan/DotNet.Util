//-----------------------------------------------------------------------
// <copyright file="BaseUserIdentityAuditEntity.cs" company="DotNet">
//     Copyright (C) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseUserIdentityAuditEntity
    /// 用户身份需要人工审核的：目前有身份证的审核，淘宝每日请求次数要求加入到此表中，后期用户其它身份的人工审核也加入到这里，如银行卡等
    /// 
    /// 修改记录
    /// 
    /// 2015-02-10 版本：1.0 SongBiao 创建文件。
    /// 
    /// <author>
    ///     <name>SongBiao</name>
    ///     <date>2015-02-10</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseUserIdentityAuditEntity : BaseEntity
    {
        /// <summary>
        /// 身份证照片修改时间,被驳回后的重新上传保存时间
        /// </summary>
        [FieldDescription("身份证照片修改时间", false)]
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 手持身份证上传后的地址
        /// </summary>
        [FieldDescription("持身份证上传后的地址", false)]
        public string IdcardPhotoHand { get; set; } = string.Empty;

        /// <summary>
        /// 申请人所属网点名称
        /// </summary>
        [FieldDescription("申请人所属网点名称")]
        public string OrganizationFullname { get; set; } = string.Empty;

        /// <summary>
        /// 身份证照片上传、申请创建时间
        /// </summary>
        [FieldDescription("身份证照片上传、申请创建时间", false)]
        public DateTime CreateOn { get; set; }

        /// <summary>
        /// 身份证审核者审核时间
        /// </summary>
        [FieldDescription("身份证审核者审核时间", false)]
        public DateTime? AuditDate { get; set; } = null;

        /// <summary>
        /// 审核状态：未审核、已通过、已驳回
        /// </summary>
        [FieldDescription("审核状态")]
        public string AuditStatus { get; set; } = string.Empty;

        /// <summary>
        /// 接口每日调用次数
        /// </summary>
        [FieldDescription("接口每日调用次数")]
        public decimal? InterfaceDayLimit { get; set; } = null;

        /// <summary>
        /// 用户唯一名
        /// </summary>
        [FieldDescription("用户唯一名")]
        public string NickName { get; set; } = string.Empty;

        /// <summary>
        /// 用户真实姓名
        /// </summary>
        [FieldDescription("用户真实姓名")]
        public string UserRealName { get; set; } = string.Empty;

        /// <summary>
        /// 身份证号码
        /// </summary>
        [FieldDescription("身份证号码")]
        public string Idcard { get; set; } = string.Empty;

        /// <summary>
        /// 身份证审核者的ID，关联用户表BASEUSER主键
        /// </summary>
        [FieldDescription("身份证审核者的ID")]
        public decimal? AuditUserId { get; set; } = null;

        /// <summary>
        /// 关联用户表BASEUSER主键
        /// </summary>
        [FieldDescription("关联用户表主键")]
        public decimal Id { get; set; }

        /// <summary>
        /// 身份证审核意见
        /// </summary>
        [FieldDescription("身份证审核意见")]
        public string AuditIdea { get; set; } = string.Empty;

        /// <summary>
        /// 审核者真实姓名
        /// </summary>
        [FieldDescription("审核者真实姓名")]
        public string AuditUserRealName { get; set; } = string.Empty;

        /// <summary>
        /// 申请人所属网点ID
        /// </summary>
        [FieldDescription("申请人所属网点ID")]
        public decimal OrganizationId { get; set; }

        /// <summary>
        /// 审核者唯一用户名
        /// </summary>
        [FieldDescription("审核者唯一用户名")]
        public string AuditUserNickName { get; set; } = string.Empty;

        /// <summary>
        /// 身份证正面图片上传后的地址
        /// </summary>
        [FieldDescription("身份证正面图片上传后的地址")]
        public string IdcardPhotoFront { get; set; } = string.Empty;
        
        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExtend(dr);
            ModifiedOn = BaseUtil.ConvertToNullableDateTime(dr[FieldUpdateTime]);
            IdcardPhotoHand = BaseUtil.ConvertToString(dr[FieldIdcardPhotoHand]);
            OrganizationFullname = BaseUtil.ConvertToString(dr[FieldOrganizationFullname]);
            CreateOn = BaseUtil.ConvertToDateTime(dr[FieldCreateTime]);
            AuditDate = BaseUtil.ConvertToNullableDateTime(dr[FieldAuditDate]);
            AuditStatus = BaseUtil.ConvertToString(dr[FieldAuditStatus]);
            InterfaceDayLimit = BaseUtil.ConvertToNullableDecimal(dr[FieldInterfaceDayLimit]);
            NickName = BaseUtil.ConvertToString(dr[FieldNickName]);
            UserRealName = BaseUtil.ConvertToString(dr[FieldUserRealName]);
            Idcard = BaseUtil.ConvertToString(dr[FieldIdcard]);
            AuditUserId = BaseUtil.ConvertToNullableDecimal(dr[FieldAuditUserId]);
            Id = BaseUtil.ConvertToDecimal(dr[FieldId]);
            AuditIdea = BaseUtil.ConvertToString(dr[FieldAuditIdea]);
            AuditUserRealName = BaseUtil.ConvertToString(dr[FieldAuditUserRealName]);
            OrganizationId = BaseUtil.ConvertToDecimal(dr[FieldOrganizationId]);
            AuditUserNickName = BaseUtil.ConvertToString(dr[FieldAuditUserNickName]);
            IdcardPhotoFront = BaseUtil.ConvertToString(dr[FieldIdcardPhotoFront]);
            return this;
        }

        ///<summary>
        /// 用户身份需要人工审核的：目前有身份证的审核，淘宝每日请求次数要求加入到此表中，后期用户其它身份的人工审核也加入到这里，如银行卡等
        ///</summary>
        [NonSerialized]
        [FieldDescription("用户身份审核")]
        public const string TableName = "BASE_USER_IDENTITY_AUDIT";

        ///<summary>
        /// 身份证照片修改时间,被驳回后的重新上传保存时间
        ///</summary>
        [NonSerialized]
        public const string FieldUpdateTime = "MODIFIED_ON";

        ///<summary>
        /// 手持身份证上传后的地址
        ///</summary>
        [NonSerialized]
        public const string FieldIdcardPhotoHand = "IDCARD_PHOTO_HAND";

        ///<summary>
        /// 申请人所属网点名称
        ///</summary>
        [NonSerialized]
        public const string FieldOrganizationFullname = "ORGANIZE_FULLNAME";

        ///<summary>
        /// 身份证照片上传、申请创建时间
        ///</summary>
        [NonSerialized]
        public const string FieldCreateTime = "CREATE_ON";

        ///<summary>
        /// 身份证审核者审核时间
        ///</summary>
        [NonSerialized]
        public const string FieldAuditDate = "AUDIT_DATE";

        ///<summary>
        /// 审核状态：未审核、已通过、已驳回
        ///</summary>
        [NonSerialized]
        public const string FieldAuditStatus = "AUDIT_STATUS";

        ///<summary>
        /// 接口每日调用次数
        ///</summary>
        [NonSerialized]
        public const string FieldInterfaceDayLimit = "INTERFACE_DAY_LIMIT";

        ///<summary>
        /// 用户唯一名
        ///</summary>
        [NonSerialized]
        public const string FieldNickName = "NICK_NAME";

        ///<summary>
        /// 用户真实姓名
        ///</summary>
        [NonSerialized]
        public const string FieldUserRealName = "USER_REAL_NAME";

        ///<summary>
        /// 身份证号码
        ///</summary>
        [NonSerialized]
        public const string FieldIdcard = "IDCARD";

        ///<summary>
        /// 身份证审核者的ID，关联用户表BASEUSER主键
        ///</summary>
        [NonSerialized]
        public const string FieldAuditUserId = "AUDIT_USER_ID";

        ///<summary>
        /// 关联用户表BASEUSER主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 身份证审核意见
        ///</summary>
        [NonSerialized]
        public const string FieldAuditIdea = "AUDIT_IDEA";

        ///<summary>
        /// 审核者真实姓名
        ///</summary>
        [NonSerialized]
        public const string FieldAuditUserRealName = "AUDIT_USER_REAL_NAME";

        ///<summary>
        /// 申请人所属网点ID
        ///</summary>
        [NonSerialized]
        public const string FieldOrganizationId = "ORGANIZE_ID";

        ///<summary>
        /// 审核者唯一用户名
        ///</summary>
        [NonSerialized]
        public const string FieldAuditUserNickName = "AUDIT_USER_NICK_NAME";

        ///<summary>
        /// 身份证正面图片上传后的地址
        ///</summary>
        [NonSerialized]
        public const string FieldIdcardPhotoFront = "IDCARD_PHOTO_FRONT";
    }
}