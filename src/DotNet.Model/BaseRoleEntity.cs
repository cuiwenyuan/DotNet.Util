//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseRoleEntity
    /// 系统角色表
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
	public partial class BaseRoleEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [FieldDescription("主键", false)]
        public string Id { get; set; } = null;

        /// <summary>
        /// 组织机构主键
        /// </summary>
        [FieldDescription("组织机构主键")]
        public string OrganizeId { get; set; } = null;

        /// <summary>
        /// 角色编号
        /// </summary>
        [FieldDescription("角色编号")]
        public string Code { get; set; } = null;

        /// <summary>
        /// 角色名称
        /// </summary>
        [FieldDescription("角色名称")]
        [StringLength(200, ErrorMessage = "名称不能超过200个字符")]
        public string RealName { get; set; } = null;
        
        /// <summary>
        /// 分类
        /// </summary>
        [FieldDescription("分类")]
        public string CategoryCode { get; set; } = null;

        /// <summary>
        /// 允许编辑
        /// </summary>
        [FieldDescription("允许编辑")]
        public int? AllowEdit { get; set; } = 1;

        /// <summary>
        /// 允许删除
        /// </summary>
        [FieldDescription("允许删除")]
        public int? AllowDelete { get; set; } = 1;

        /// <summary>
        /// 可显示
        /// </summary>
        [FieldDescription("可显示")]
        public int? IsVisible { get; set; } = 1;

        /// <summary>
        /// 排序码
        /// </summary>
        [FieldDescription("排序码", false)]
        public int? SortCode { get; set; } = 0;

        /// <summary>
        /// 是否删除
        /// </summary>
        [FieldDescription("是否删除", false)]
        public int DeletionStateCode { get; set; } = 0;

        /// <summary>
        /// 是否有效
        /// </summary>
        [FieldDescription("有效标志")]
        public int Enabled { get; set; } = 1;

        /// <summary>
        /// 备注
        /// </summary>
        [FieldDescription("备注")]
        public string Description { get; set; } = null;

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
            OrganizeId = BaseUtil.ConvertToString(dr[FieldOrganizeId]);
            Code = BaseUtil.ConvertToString(dr[FieldCode]);
            RealName = BaseUtil.ConvertToString(dr[FieldRealName]);
            CategoryCode = BaseUtil.ConvertToString(dr[FieldCategoryCode]);
            AllowEdit = BaseUtil.ConvertToInt(dr[FieldAllowEdit]);
            AllowDelete = BaseUtil.ConvertToInt(dr[FieldAllowDelete]);
            IsVisible = BaseUtil.ConvertToInt(dr[FieldIsVisible]);
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
            GetFromExpand(dr);
            return this;
        }

        ///<summary>
        /// 系统角色表
        ///</summary>
        [NonSerialized]
        public const string TableName = "BaseRole";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 组织机构主键
        ///</summary>
        [NonSerialized]
        public const string FieldOrganizeId = "OrganizeId";

        ///<summary>
        /// 角色编号
        ///</summary>
        [NonSerialized]
        public const string FieldCode = "Code";

        ///<summary>
        /// 角色名称
        ///</summary>
        [NonSerialized]
        public const string FieldRealName = "RealName";

        ///<summary>
        /// 角色分类
        ///</summary>
        [NonSerialized]
        public const string FieldCategoryCode = "CategoryCode";

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
        /// 是否显示
        ///</summary>
        [NonSerialized]
        public const string FieldIsVisible = "IsVisible";

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