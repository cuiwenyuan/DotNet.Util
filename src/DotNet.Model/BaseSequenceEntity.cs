//--------------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//--------------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseSequenceEntity
    /// 序列产生器表
    /// 
    /// 修改记录
    /// 
    /// 2012-04-23 版本：1.0 JiRiGaLa 创建主键。
    /// 
    /// <author>
    ///     <name>JiRiGaLa</name>
    ///     <date>2012-04-23</date>
    /// </author>
    /// </summary>
    [Serializable]
	public partial class BaseSequenceEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 序列号前缀
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 序列号分隔符
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// 升序序列
        /// </summary>
        public int? Sequence { get; set; }

        /// <summary>
        /// 倒序序列
        /// </summary>
        public int? Reduction { get; set; }

        /// <summary>
        /// 步骤
        /// </summary>
        public int? Step { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public int? IsVisible { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateOn { get; set; }

        /// <summary>
        /// 创建人用户编号
        /// </summary>
        public string CreateUserId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// 修改人用户编号
        /// </summary>
        public string ModifiedUserId { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
		protected override BaseEntity GetFrom(IDataRow dr)
        {
            Id = BaseUtil.ConvertToString(dr[FieldId]);
            FullName = BaseUtil.ConvertToString(dr[FieldFullName]);
            Prefix = BaseUtil.ConvertToString(dr[FieldPrefix]);
            Delimiter = BaseUtil.ConvertToString(dr[FieldDelimiter]);
            Sequence = BaseUtil.ConvertToInt(dr[FieldSequence]);
            Reduction = BaseUtil.ConvertToInt(dr[FieldReduction]);
            Step = BaseUtil.ConvertToInt(dr[FieldStep]);
            IsVisible = BaseUtil.ConvertToInt(dr[FieldIsVisible]);
            Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            CreateOn = BaseUtil.ConvertToNullableDateTime(dr[FieldCreateTime]);
            CreateUserId = BaseUtil.ConvertToString(dr[FieldCreateUserId]);
            CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            ModifiedOn = BaseUtil.ConvertToNullableDateTime(dr[FieldUpdateTime]);
            ModifiedUserId = BaseUtil.ConvertToString(dr[FieldUpdateUserId]);
            ModifiedBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            // 获取扩展属性
            GetFromExpand(dr);
            return this;
        }

        ///<summary>
        /// 序列产生器表
        ///</summary>
        [NonSerialized]
        public const string TableName = "BaseSequence";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 名称
        ///</summary>
        [NonSerialized]
        public const string FieldFullName = "FullName";

        ///<summary>
        /// 序列号前缀
        ///</summary>
        [NonSerialized]
        public const string FieldPrefix = "Prefix";

        ///<summary>
        /// 序列号分隔符
        ///</summary>
        [NonSerialized]
        public const string FieldDelimiter = "Delimiter";

        ///<summary>
        /// 升序序列
        ///</summary>
        [NonSerialized]
        public const string FieldSequence = "Sequence";

        ///<summary>
        /// 倒序序列
        ///</summary>
        [NonSerialized]
        public const string FieldReduction = "Reduction";

        ///<summary>
        /// 步骤
        ///</summary>
        [NonSerialized]
        public const string FieldStep = "Step";

        ///<summary>
        /// 是否显示
        ///</summary>
        [NonSerialized]
        public const string FieldIsVisible = "IsVisible";

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
