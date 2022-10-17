//-----------------------------------------------------------------------
// <copyright file="BaseRoleEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2022, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseRoleEntity
    /// 系统角色
    ///
    /// 修改记录
    /// 
    /// 2021-09-29 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2021-09-29</date>
    /// </author>
    /// </summary>
    public partial class BaseRoleEntity : BaseEntity
    {
        /// <summary>
        /// 子系统编码
        /// </summary>
        [FieldDescription("子系统编码")]
        public string SystemCode { get; set; } = "Base";

        /// <summary>
        /// 组织机构主键
        /// </summary>
        [FieldDescription("组织机构主键")]
        public int OrganizationId { get; set; } = 0;

        /// <summary>
        /// 角色编号
        /// </summary>
        [FieldDescription("角色编号")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 角色名称
        /// </summary>
        [FieldDescription("角色名称")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 角色分类
        /// </summary>
        [FieldDescription("角色分类")]
        public string CategoryCode { get; set; } = string.Empty;

        /// <summary>
        /// 允许编辑
        /// </summary>
        [FieldDescription("允许编辑")]
        public int AllowEdit { get; set; } = 1;

        /// <summary>
        /// 允许删除
        /// </summary>
        [FieldDescription("允许删除")]
        public int AllowDelete { get; set; } = 1;

        /// <summary>
        /// 是否显示
        /// </summary>
        [FieldDescription("是否显示")]
        public int IsVisible { get; set; } = 1;

        /// <summary>
        /// 描述
        /// </summary>
        [FieldDescription("描述")]
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
            if (dr.ContainsColumn(FieldOrganizationId))
            {
                OrganizationId = BaseUtil.ConvertToInt(dr[FieldOrganizationId]);
            }
            if (dr.ContainsColumn(FieldCode))
            {
                Code = BaseUtil.ConvertToString(dr[FieldCode]);
            }
            if (dr.ContainsColumn(FieldName))
            {
                Name = BaseUtil.ConvertToString(dr[FieldName]);
            }
            if (dr.ContainsColumn(FieldCategoryCode))
            {
                CategoryCode = BaseUtil.ConvertToString(dr[FieldCategoryCode]);
            }
            if (dr.ContainsColumn(FieldAllowEdit))
            {
                AllowEdit = BaseUtil.ConvertToInt(dr[FieldAllowEdit]);
            }
            if (dr.ContainsColumn(FieldAllowDelete))
            {
                AllowDelete = BaseUtil.ConvertToInt(dr[FieldAllowDelete]);
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
        /// 角色
        ///</summary>
        [FieldDescription("角色")]
        public const string CurrentTableName = "BaseRole";

        ///<summary>
        /// 子系统编码
        ///</summary>
        public const string FieldSystemCode = "SystemCode";

        ///<summary>
        /// 组织机构主键
        ///</summary>
        public const string FieldOrganizationId = "OrganizationId";

        ///<summary>
        /// 角色编号
        ///</summary>
        public const string FieldCode = "Code";

        ///<summary>
        /// 角色名称
        ///</summary>
        public const string FieldName = "Name";

        ///<summary>
        /// 角色分类
        ///</summary>
        public const string FieldCategoryCode = "CategoryCode";

        ///<summary>
        /// 允许编辑
        ///</summary>
        public const string FieldAllowEdit = "AllowEdit";

        ///<summary>
        /// 允许删除
        ///</summary>
        public const string FieldAllowDelete = "AllowDelete";

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