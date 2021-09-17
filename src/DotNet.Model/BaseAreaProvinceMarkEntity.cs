//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseAreaProvinceMarkEntity
    /// 地区表(省、市、县)
    ///
    /// 修改记录
    ///
    ///		2015-06-23 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015-06-23</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseAreaProvinceMarkEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [FieldDescription("主键", false)]
        public string Id { get; set; } = null;
        /// <summary>
        /// 区域主键
        /// </summary>
        [FieldDescription("区域主键")]
        public int AreaId { get; set; } = 0;
        /// <summary>
        /// 区域
        /// </summary>
        [FieldDescription("区域")]
        public string Area { get; set; } = null;
        /// <summary>
        /// 省主键
        /// </summary>
        [FieldDescription("省主键")]
        public int ProvinceId { get; set; } = 0;
        /// <summary>
        /// 省
        /// </summary>
        [FieldDescription("省")]
        public string Province { get; set; } = null;
        /// <summary>
        /// 机打大头笔
        /// </summary>
        [FieldDescription("机打大头笔")]
        public string Mark { get; set; } = null;
        /// <summary>
        /// 备注
        /// </summary>
        [FieldDescription("备注")]
        public string Description { get; set; } = null;
        /// <summary>
        /// 有效
        /// </summary>
        [FieldDescription("是否有效")]
        public int? Enabled { get; set; } = 1;
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
            AreaId = BaseUtil.ConvertToInt(dr[FieldAreaId]);
            Area = BaseUtil.ConvertToString(dr[FieldArea]);
            ProvinceId = BaseUtil.ConvertToInt(dr[FieldProvinceId]);
            Province = BaseUtil.ConvertToString(dr[FieldProvince]);
            Mark = BaseUtil.ConvertToString(dr[FieldMark]);
            Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
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
        /// 机打大头笔表
        ///</summary>
        [NonSerialized]
        [FieldDescription("区域省份表")]
        public const string TableName = "BaseArea_ProvinceMark";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        [FieldDescription("主键")]
        public const string FieldId = "Id";

        ///<summary>
        /// 区域主键
        ///</summary>
        [NonSerialized]
        [FieldDescription("区域键")]
        public const string FieldAreaId = "AreaId";

        ///<summary>
        /// 区域
        ///</summary>
        [NonSerialized]
        [FieldDescription("区域")]
        public const string FieldArea = "Area";

        ///<summary>
        /// 省主键
        ///</summary>
        [NonSerialized]
        [FieldDescription("省主键")]
        public const string FieldProvinceId = "ProvinceId";

        ///<summary>
        /// 省
        ///</summary>
        [NonSerialized]
        [FieldDescription("省")]
        public const string FieldProvince = "Province";

        /// <summary>
        /// 手写大头笔
        /// </summary>
        [NonSerialized]
        [FieldDescription("机打大头笔")]
        public const string FieldMark = "Mark";

        ///<summary>
        /// 备注
        ///</summary>
        [NonSerialized]
        [FieldDescription("备注")]
        public const string FieldDescription = "Description";

        ///<summary>
        /// 有效
        ///</summary>
        [NonSerialized]
        [FieldDescription("是否有效")]
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 创建时间
        ///</summary>
        [NonSerialized]
        [FieldDescription("创建时间")]
        public const string FieldCreateTime = "CreateOn";

        ///<summary>
        /// 创建人用户编号
        ///</summary>
        [NonSerialized]
        [FieldDescription("创建人用户编号")]
        public const string FieldCreateUserId = "CreateUserId";

        ///<summary>
        /// 创建人
        ///</summary>
        [NonSerialized]
        [FieldDescription("创建人")]
        public const string FieldCreateBy = "CreateBy";

        ///<summary>
        /// 修改时间
        ///</summary>
        [NonSerialized]
        [FieldDescription("修改时间")]
        public const string FieldUpdateTime = "ModifiedOn";

        ///<summary>
        /// 修改人用户编号
        ///</summary>
        [NonSerialized]
        [FieldDescription("修改人用户编号")]
        public const string FieldUpdateUserId = "ModifiedUserId";

        ///<summary>
        /// 修改人
        ///</summary>
        [NonSerialized]
        [FieldDescription("修改人")]
        public const string FieldUpdateBy = "ModifiedBy";
    }
}
