﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseTableColumnsEntity
    /// 表字段结构定义说明
    ///
    /// 修改记录
    ///
    ///		2010-07-15 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2010-07-15</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseTableColumnsEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int? Id { get; set; } = 0;

        /// <summary>
        /// 表
        /// </summary>
        public string TableCode { get; set; } = null;

        /// <summary>
        /// 表
        /// </summary>
        public string ColumnCode { get; set; } = null;

        /// <summary>
        /// 表名
        /// </summary>
        public string ColumnName { get; set; } = null;

        /// <summary>
        /// 是否公开
        /// </summary>
        public int? IsPublic { get; set; } = 0;

        /// <summary>
        /// 有效
        /// </summary>
        public int? Enabled { get; set; } = 1;

        /// <summary>
        /// 允许编辑
        /// </summary>
        public int? AllowEdit { get; set; } = 0;

        /// <summary>
        /// 允许删除
        /// </summary>
        public int? AllowDelete { get; set; } = 0;

        /// <summary>
        /// 是否删除
        /// </summary>
        public int? DeletionStateCode { get; set; } = 0;

        /// <summary>
        /// 排序码
        /// </summary>
        public int? SortCode { get; set; } = 0;

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
            Id = BaseUtil.ConvertToInt(dr[FieldId]);
            TableCode = BaseUtil.ConvertToString(dr[FieldTableCode]);
            ColumnCode = BaseUtil.ConvertToString(dr[FieldColumnCode]);
            ColumnName = BaseUtil.ConvertToString(dr[FieldColumnName]);
            IsPublic = BaseUtil.ConvertToInt(dr[FieldIsPublic]);
            Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            AllowEdit = BaseUtil.ConvertToInt(dr[FieldAllowEdit]);
            AllowDelete = BaseUtil.ConvertToInt(dr[FieldAllowDelete]);
            DeletionStateCode = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            SortCode = BaseUtil.ConvertToInt(dr[FieldSortCode]);
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
        /// 表字段结构定义说明
        ///</summary>
        [NonSerialized]
        public const string TableName = "BaseTableColumns";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 表
        ///</summary>
        [NonSerialized]
        public const string FieldTableCode = "TableCode";

        ///<summary>
        /// 字段编号
        ///</summary>
        [NonSerialized]
        public const string FieldColumnCode = "ColumnCode";

        ///<summary>
        /// 字段名
        ///</summary>
        [NonSerialized]
        public const string FieldColumnName = "ColumnName";

        /// <summary>
        /// 约束条件
        /// </summary>
        [NonSerialized]
        public const string FieldUseConstraint = "UseConstraint";

        ///<summary>
        /// 是否公开
        ///</summary>
        [NonSerialized]
        public const string FieldIsPublic = "IsPublic";

        ///<summary>
        /// 数据类型
        ///</summary>
        [NonSerialized]
        public const string FieldDataType = "DataType";

        ///<summary>
        /// 数据字典来源
        ///</summary>
        [NonSerialized]
        public const string FieldTargetTable = "TargetTable";

        ///<summary>
        /// 有效
        ///</summary>
        [NonSerialized]
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 允许编辑
        ///</summary>
        [NonSerialized]
        public const string FieldAllowEdit = "AllowEdit";

        ///<summary>
        /// 允许删除
        ///</summary>
        [NonSerialized]
        public const string FieldAllowDelete = "AllowDelete";

        ///<summary>
        /// 是否删除
        ///</summary>
        [NonSerialized]
        public const string FieldDeleted = "DeletionStateCode";

        ///<summary>
        /// 排序码
        ///</summary>
        [NonSerialized]
        public const string FieldSortCode = "SortCode";

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