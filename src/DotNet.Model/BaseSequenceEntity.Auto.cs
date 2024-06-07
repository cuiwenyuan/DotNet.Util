//-----------------------------------------------------------------------
// <copyright file="BaseSequenceEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2024, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseSequenceEntity
    /// 序列
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
    public partial class BaseSequenceEntity : BaseEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        [FieldDescription("名称")]
        [Description("名称")]
        [Column(FieldName)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 前缀
        /// </summary>
        [FieldDescription("前缀")]
        [Description("前缀")]
        [Column(FieldPrefix)]
        public string Prefix { get; set; } = string.Empty;

        /// <summary>
        /// 分隔符
        /// </summary>
        [FieldDescription("分隔符")]
        [Description("分隔符")]
        [Column(FieldDelimiter)]
        public string Delimiter { get; set; } = string.Empty;

        /// <summary>
        /// 升序序列
        /// </summary>
        [FieldDescription("升序序列")]
        [Description("升序序列")]
        [Column(FieldSequence)]
        public int Sequence { get; set; } = 10000000;

        /// <summary>
        /// 倒序序列
        /// </summary>
        [FieldDescription("倒序序列")]
        [Description("倒序序列")]
        [Column(FieldReduction)]
        public int Reduction { get; set; } = 9999999;

        /// <summary>
        /// 步长
        /// </summary>
        [FieldDescription("步长")]
        [Description("步长")]
        [Column(FieldStep)]
        public int Step { get; set; } = 1;

        /// <summary>
        /// 是否显示
        /// </summary>
        [FieldDescription("是否显示")]
        [Description("是否显示")]
        [Column(FieldIsVisible)]
        public int IsVisible { get; set; } = 1;

        /// <summary>
        /// 描述
        /// </summary>
        [FieldDescription("描述")]
        [Description("描述")]
        [Column(FieldDescription)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExtend(dr);
            GetBase(dr);
            if (dr.ContainsColumn(FieldName))
            {
                Name = BaseUtil.ConvertToString(dr[FieldName]);
            }
            if (dr.ContainsColumn(FieldPrefix))
            {
                Prefix = BaseUtil.ConvertToString(dr[FieldPrefix]);
            }
            if (dr.ContainsColumn(FieldDelimiter))
            {
                Delimiter = BaseUtil.ConvertToString(dr[FieldDelimiter]);
            }
            if (dr.ContainsColumn(FieldSequence))
            {
                Sequence = BaseUtil.ConvertToInt(dr[FieldSequence]);
            }
            if (dr.ContainsColumn(FieldReduction))
            {
                Reduction = BaseUtil.ConvertToInt(dr[FieldReduction]);
            }
            if (dr.ContainsColumn(FieldStep))
            {
                Step = BaseUtil.ConvertToInt(dr[FieldStep]);
            }
            if (dr.ContainsColumn(FieldIsVisible))
            {
                IsVisible = BaseUtil.ConvertToInt(dr[FieldIsVisible]);
            }
            if (dr.ContainsColumn(FieldDescription))
            {
                Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            }
            return this;
        }

        ///<summary>
        /// 序列
        ///</summary>
        [FieldDescription("序列")]
        public const string CurrentTableName = "BaseSequence";

        ///<summary>
        /// 名称
        ///</summary>
        public const string FieldName = "Name";

        ///<summary>
        /// 前缀
        ///</summary>
        public const string FieldPrefix = "Prefix";

        ///<summary>
        /// 分隔符
        ///</summary>
        public const string FieldDelimiter = "Delimiter";

        ///<summary>
        /// 升序序列
        ///</summary>
        public const string FieldSequence = "Sequence";

        ///<summary>
        /// 倒序序列
        ///</summary>
        public const string FieldReduction = "Reduction";

        ///<summary>
        /// 步长
        ///</summary>
        public const string FieldStep = "Step";

        ///<summary>
        /// 是否显示
        ///</summary>
        public const string FieldIsVisible = "IsVisible";

        ///<summary>
        /// 描述
        ///</summary>
        public const string FieldDescription = "Description";
    }
}
