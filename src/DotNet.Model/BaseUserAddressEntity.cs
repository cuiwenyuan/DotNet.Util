//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseUserAddressEntity
    /// 用户送货地址表
    ///
    /// 修改记录
    ///
    ///		2010-07-15 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2010-07-15</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseUserAddressEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; } = null;

        /// <summary>
        /// 用户主键
        /// </summary>
        public string UserId { get; set; } = null;

        /// <summary>
        /// 联系人（收货人）
        /// </summary>
        public string RealName { get; set; } = null;

        /// <summary>
        /// 省主键
        /// </summary>
        public string ProvinceId { get; set; } = null;

        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; } = null;

        /// <summary>
        /// 市主键
        /// </summary>
        public string CityId { get; set; } = null;

        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; } = null;

        /// <summary>
        /// 区/县主键
        /// </summary>
        public string AreaId { get; set; } = null;

        /// <summary>
        /// 区/县
        /// </summary>
        public string Area { get; set; } = null;

        /// <summary>
        /// 街道地址
        /// </summary>
        public string Address { get; set; } = null;

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string PostCode { get; set; } = null;

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; } = null;

        /// <summary>
        /// 传真
        /// </summary>
        public string Fax { get; set; } = null;

        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; } = null;

        /// <summary>
        /// 电子邮件
        /// </summary>
        public string Email { get; set; } = null;

        /// <summary>
        /// 送货方式
        /// </summary>
        public string DeliverCategory { get; set; } = null;

        /// <summary>
        /// 排序码
        /// </summary>
        public int? SortCode { get; set; } = 0;

        /// <summary>
        /// 是否删除
        /// </summary>
        public int? DeletionStateCode { get; set; } = 0;

        /// <summary>
        /// 有效
        /// </summary>
        public int? Enabled { get; set; } = 1;

        /// <summary>
        /// 备注
        /// </summary>
        public string Description { get; set; } = null;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人用户编号
        /// </summary>
        public string CreateUserId { get; set; } = null;

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; } = null;

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改人用户编号
        /// </summary>
        public string ModifiedUserId { get; set; } = null;

        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifiedBy { get; set; } = null;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            Id = BaseUtil.ConvertToString(dr[FieldId]);
            UserId = BaseUtil.ConvertToString(dr[FieldUserId]);
            RealName = BaseUtil.ConvertToString(dr[FieldRealName]);
            ProvinceId = BaseUtil.ConvertToString(dr[FieldProvinceId]);
            Province = BaseUtil.ConvertToString(dr[FieldProvince]);
            CityId = BaseUtil.ConvertToString(dr[FieldCityId]);
            City = BaseUtil.ConvertToString(dr[FieldCity]);
            AreaId = BaseUtil.ConvertToString(dr[FieldAreaId]);
            Area = BaseUtil.ConvertToString(dr[FieldArea]);
            Address = BaseUtil.ConvertToString(dr[FieldAddress]);
            PostCode = BaseUtil.ConvertToString(dr[FieldPostCode]);
            Phone = BaseUtil.ConvertToString(dr[FieldPhone]);
            Fax = BaseUtil.ConvertToString(dr[FieldFax]);
            Mobile = BaseUtil.ConvertToString(dr[FieldMobile]);
            Email = BaseUtil.ConvertToString(dr[FieldEmail]);
            DeliverCategory = BaseUtil.ConvertToString(dr[FieldDeliverCategory]);
            SortCode = BaseUtil.ConvertToInt(dr[FieldSortCode]);
            DeletionStateCode = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            Description = BaseUtil.ConvertToString(dr[FieldDescription]);
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
        /// 用户送货地址表
        ///</summary>
        [NonSerialized]
        public const string TableName = "BaseUserAddress";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 用户主键
        ///</summary>
        [NonSerialized]
        public const string FieldUserId = "UserId";

        ///<summary>
        /// 联系人（收货人）
        ///</summary>
        [NonSerialized]
        public const string FieldRealName = "RealName";

        ///<summary>
        /// 省主键
        ///</summary>
        [NonSerialized]
        public const string FieldProvinceId = "ProvinceId";

        ///<summary>
        /// 省
        ///</summary>
        [NonSerialized]
        public const string FieldProvince = "Province";

        ///<summary>
        /// 市主键
        ///</summary>
        [NonSerialized]
        public const string FieldCityId = "CityId";

        ///<summary>
        /// 市
        ///</summary>
        [NonSerialized]
        public const string FieldCity = "City";

        ///<summary>
        /// 区/县主键
        ///</summary>
        [NonSerialized]
        public const string FieldAreaId = "AreaId";

        ///<summary>
        /// 区/县
        ///</summary>
        [NonSerialized]
        public const string FieldArea = "Area";

        ///<summary>
        /// 街道地址
        ///</summary>
        [NonSerialized]
        public const string FieldAddress = "Address";

        ///<summary>
        /// 邮政编码
        ///</summary>
        [NonSerialized]
        public const string FieldPostCode = "PostCode";

        ///<summary>
        /// 联系电话
        ///</summary>
        [NonSerialized]
        public const string FieldPhone = "Phone";

        ///<summary>
        /// 传真
        ///</summary>
        [NonSerialized]
        public const string FieldFax = "Fax";

        ///<summary>
        /// 手机
        ///</summary>
        [NonSerialized]
        public const string FieldMobile = "Mobile";

        ///<summary>
        /// 电子邮件
        ///</summary>
        [NonSerialized]
        public const string FieldEmail = "Email";

        ///<summary>
        /// 送货方式
        ///</summary>
        [NonSerialized]
        public const string FieldDeliverCategory = "DeliverCategory";

        ///<summary>
        /// 排序码
        ///</summary>
        [NonSerialized]
        public const string FieldSortCode = "SortCode";

        ///<summary>
        /// 是否删除
        ///</summary>
        [NonSerialized]
        public const string FieldDeleted = "DeletionStateCode";

        ///<summary>
        /// 是否有效
        ///</summary>
        [NonSerialized]
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 备注
        ///</summary>
        [NonSerialized]
        public const string FieldDescription = "Description";

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
