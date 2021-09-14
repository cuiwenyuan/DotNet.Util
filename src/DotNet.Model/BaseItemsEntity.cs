//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseItemsEntity
    /// 选项主表（资源）
    ///
    /// 修改记录
    ///
    ///		2010-07-28 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2010-07-28</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseItemsEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int? Id { get; set; } = null;
        /// <summary>
        /// 父节点主键
        /// </summary>
        public int? ParentId { get; set; } = null;
        /// <summary>
        /// 编号
        /// </summary>
        public string Code { get; set; } = null;
        /// <summary>
        /// 名称
        /// </summary>
        public string FullName { get; set; } = null;
        /// <summary>
        /// 目标存储表
        /// </summary>
        public string TargetTable { get; set; } = null;
        /// <summary>
        /// 树型结构
        /// </summary>
        public int? IsTree { get; set; } = 0;
        /// <summary>
        /// 允许编辑
        /// </summary>
        public int? AllowEdit { get; set; } = 1;
        /// <summary>
        /// 允许删除
        /// </summary>
        public int? AllowDelete { get; set; } = 1;
        /// <summary>
        /// 是否删除
        /// </summary>
        public int? DeletionStateCode { get; set; } = 0;
        /// <summary>
        /// 备注
        /// </summary>
        public string Description { get; set; } = null;
        /// <summary>
        /// 是否有效
        /// </summary>
        public int? Enabled { get; set; } = 1;
        /// <summary>
        /// 排序码
        /// </summary>
        public int? SortCode { get; set; } = 0;
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
            ParentId = BaseUtil.ConvertToNullableInt(dr[FieldParentId]);
            Code = BaseUtil.ConvertToString(dr[FieldCode]);
            FullName = BaseUtil.ConvertToString(dr[FieldFullName]);
            TargetTable = BaseUtil.ConvertToString(dr[FieldTargetTable]);
            IsTree = BaseUtil.ConvertToInt(dr[FieldIsTree]);
            AllowEdit = BaseUtil.ConvertToInt(dr[FieldAllowEdit]);
            AllowDelete = BaseUtil.ConvertToInt(dr[FieldAllowDelete]);
            DeletionStateCode = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            SortCode = BaseUtil.ConvertToInt(dr[FieldSortCode]);
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
        /// 选项主表（资源）
        ///</summary>
        [NonSerialized]
        public const string TableName = "BaseItems";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 父节点主键
        ///</summary>
        [NonSerialized]
        public const string FieldParentId = "ParentId";

        ///<summary>
        /// 编号
        ///</summary>
        [NonSerialized]
        public const string FieldCode = "Code";

        ///<summary>
        /// 名称
        ///</summary>
        [NonSerialized]
        public const string FieldFullName = "FullName";

        ///<summary>
        /// 目标存储表
        ///</summary>
        [NonSerialized]
        public const string FieldTargetTable = "TargetTable";

        ///<summary>
        /// 树型结构
        ///</summary>
        [NonSerialized]
        public const string FieldIsTree = "IsTree";

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
        /// 备注
        ///</summary>
        [NonSerialized]
        public const string FieldDescription = "Description";

        ///<summary>
        /// 有效
        ///</summary>
        [NonSerialized]
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 排序码
        ///</summary>
        [NonSerialized]
        public const string FieldSortCode = "SortCode";

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
