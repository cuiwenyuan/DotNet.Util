//-----------------------------------------------------------------------
// <copyright file="BaseDictionaryItemEntity.Auto.cs" company="DotNet">
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
    /// BaseDictionaryItemEntity
    /// 字典项
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
    public partial class BaseDictionaryItemEntity : BaseEntity
    {
        /// <summary>
        /// 字典编号
        /// </summary>
        [FieldDescription("字典编号")]
        [Description("字典编号")]
        [Column(FieldDictionaryId)]
        public int DictionaryId { get; set; }

        /// <summary>
        /// 父节点主键
        /// </summary>
        [FieldDescription("父节点主键")]
        [Description("父节点主键")]
        [Column(FieldParentId)]
        public int ParentId { get; set; } = 0;

        /// <summary>
        /// 键
        /// </summary>
        [FieldDescription("键")]
        [Description("键")]
        [Column(FieldItemKey)]
        public string ItemKey { get; set; } = string.Empty;

        /// <summary>
        /// 名称
        /// </summary>
        [FieldDescription("名称")]
        [Description("名称")]
        [Column(FieldItemName)]
        public string ItemName { get; set; } = string.Empty;

        /// <summary>
        /// 值
        /// </summary>
        [FieldDescription("值")]
        [Description("值")]
        [Column(FieldItemValue)]
        public string ItemValue { get; set; } = string.Empty;

        /// <summary>
        /// 语言(i18n)
        /// </summary>
        [FieldDescription("语言(i18n)")]
        [Description("语言(i18n)")]
        [Column(FieldLanguage)]
        public string Language { get; set; } = "global";

        /// <summary>
        /// 允许编辑
        /// </summary>
        [FieldDescription("允许编辑")]
        [Description("允许编辑")]
        [Column(FieldAllowEdit)]
        public int AllowEdit { get; set; } = 1;

        /// <summary>
        /// 允许删除
        /// </summary>
        [FieldDescription("允许删除")]
        [Description("允许删除")]
        [Column(FieldAllowDelete)]
        public int AllowDelete { get; set; } = 1;

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
            if (dr.ContainsColumn(FieldDictionaryId))
            {
                DictionaryId = BaseUtil.ConvertToInt(dr[FieldDictionaryId]);
            }
            if (dr.ContainsColumn(FieldParentId))
            {
                ParentId = BaseUtil.ConvertToInt(dr[FieldParentId]);
            }
            if (dr.ContainsColumn(FieldItemKey))
            {
                ItemKey = BaseUtil.ConvertToString(dr[FieldItemKey]);
            }
            if (dr.ContainsColumn(FieldItemName))
            {
                ItemName = BaseUtil.ConvertToString(dr[FieldItemName]);
            }
            if (dr.ContainsColumn(FieldItemValue))
            {
                ItemValue = BaseUtil.ConvertToString(dr[FieldItemValue]);
            }
            if (dr.ContainsColumn(FieldLanguage))
            {
                Language = BaseUtil.ConvertToString(dr[FieldLanguage]);
            }
            if (dr.ContainsColumn(FieldAllowEdit))
            {
                AllowEdit = BaseUtil.ConvertToInt(dr[FieldAllowEdit]);
            }
            if (dr.ContainsColumn(FieldAllowDelete))
            {
                AllowDelete = BaseUtil.ConvertToInt(dr[FieldAllowDelete]);
            }
            if (dr.ContainsColumn(FieldDescription))
            {
                Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            }
            return this;
        }

        ///<summary>
        /// 字典项
        ///</summary>
        [FieldDescription("字典项")]
        public const string CurrentTableName = "BaseDictionaryItem";

        ///<summary>
        /// 字典编号
        ///</summary>
        public const string FieldDictionaryId = "DictionaryId";

        ///<summary>
        /// 父节点主键
        ///</summary>
        public const string FieldParentId = "ParentId";

        ///<summary>
        /// 键
        ///</summary>
        public const string FieldItemKey = "ItemKey";

        ///<summary>
        /// 名称
        ///</summary>
        public const string FieldItemName = "ItemName";

        ///<summary>
        /// 值
        ///</summary>
        public const string FieldItemValue = "ItemValue";

        ///<summary>
        /// 语言(i18n)
        ///</summary>
        public const string FieldLanguage = "Language";

        ///<summary>
        /// 允许编辑
        ///</summary>
        public const string FieldAllowEdit = "AllowEdit";

        ///<summary>
        /// 允许删除
        ///</summary>
        public const string FieldAllowDelete = "AllowDelete";

        ///<summary>
        /// 描述
        ///</summary>
        public const string FieldDescription = "Description";
    }
}
