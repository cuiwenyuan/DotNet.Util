//-----------------------------------------------------------------------
// <copyright file="BaseParameterEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2025, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseParameterEntity
    /// 参数
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
    public partial class BaseParameterEntity : BaseEntity
    {
        /// <summary>
        /// 子系统编码
        /// </summary>
        [FieldDescription("子系统编码")]
        [Description("子系统编码")]
        [Column(FieldSystemCode)]
        public string SystemCode { get; set; } = "Base";

        /// <summary>
        /// 分类编号
        /// </summary>
        [FieldDescription("分类编号")]
        [Description("分类编号")]
        [Column(FieldCategoryCode)]
        public string CategoryCode { get; set; } = string.Empty;

        /// <summary>
        /// 参数主键
        /// </summary>
        [FieldDescription("参数主键")]
        [Description("参数主键")]
        [Column(FieldParameterId)]
        public string ParameterId { get; set; } = string.Empty;

        /// <summary>
        /// 参数编码
        /// </summary>
        [FieldDescription("参数编码")]
        [Description("参数编码")]
        [Column(FieldParameterCode)]
        public string ParameterCode { get; set; } = string.Empty;

        /// <summary>
        /// 参数内容
        /// </summary>
        [FieldDescription("参数内容")]
        [Description("参数内容")]
        [Column(FieldParameterContent)]
        public string ParameterContent { get; set; } = string.Empty;

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
            if (dr.ContainsColumn(FieldSystemCode))
            {
                SystemCode = BaseUtil.ConvertToString(dr[FieldSystemCode]);
            }
            if (dr.ContainsColumn(FieldCategoryCode))
            {
                CategoryCode = BaseUtil.ConvertToString(dr[FieldCategoryCode]);
            }
            if (dr.ContainsColumn(FieldParameterId))
            {
                ParameterId = BaseUtil.ConvertToString(dr[FieldParameterId]);
            }
            if (dr.ContainsColumn(FieldParameterCode))
            {
                ParameterCode = BaseUtil.ConvertToString(dr[FieldParameterCode]);
            }
            if (dr.ContainsColumn(FieldParameterContent))
            {
                ParameterContent = BaseUtil.ConvertToString(dr[FieldParameterContent]);
            }
            if (dr.ContainsColumn(FieldDescription))
            {
                Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            }
            return this;
        }

        ///<summary>
        /// 参数
        ///</summary>
        [FieldDescription("参数")]
        public const string CurrentTableName = "BaseParameter";

        ///<summary>
        /// 表名
        ///</summary>
        public const string CurrentTableDescription = "参数";

        ///<summary>
        /// 子系统编码
        ///</summary>
        public const string FieldSystemCode = "SystemCode";

        ///<summary>
        /// 分类编号
        ///</summary>
        public const string FieldCategoryCode = "CategoryCode";

        ///<summary>
        /// 参数主键
        ///</summary>
        public const string FieldParameterId = "ParameterId";

        ///<summary>
        /// 参数编码
        ///</summary>
        public const string FieldParameterCode = "ParameterCode";

        ///<summary>
        /// 参数内容
        ///</summary>
        public const string FieldParameterContent = "ParameterContent";

        ///<summary>
        /// 描述
        ///</summary>
        public const string FieldDescription = "Description";
    }
}