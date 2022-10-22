//-----------------------------------------------------------------------
// <copyright file="BaseModuleEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2022, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseModuleEntity
    /// 模块菜单操作
    /// 
    /// 修改记录
    /// 
    /// 2021-09-26 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2021-09-26</date>
    /// </author>
    /// </summary>
    [Table(CurrentTableName)]
    public partial class BaseModuleEntity : BaseEntity
    {
        /// <summary>
        /// 子系统编码
        /// </summary>
        [FieldDescription("子系统编码")]
        [Description("子系统编码")]
        [Column(FieldSystemCode)]
        public string SystemCode { get; set; } = "Base";

        /// <summary>
        /// 父节点主键
        /// </summary>
        [FieldDescription("父节点主键")]
        [Description("父节点主键")]
        [Column(FieldParentId)]
        public int ParentId { get; set; } = 0;

        /// <summary>
        /// 编号
        /// </summary>
        [FieldDescription("编号")]
        [Description("编号")]
        [Column(FieldCode)]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 名称
        /// </summary>
        [FieldDescription("名称")]
        [Description("名称")]
        [Column(FieldName)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 菜单分类
        /// </summary>
        [FieldDescription("菜单分类")]
        [Description("菜单分类")]
        [Column(FieldCategoryCode)]
        public string CategoryCode { get; set; } = "Application";

        /// <summary>
        /// 图标位置
        /// </summary>
        [FieldDescription("图标位置")]
        [Description("图标位置")]
        [Column(FieldImageUrl)]
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// 图标编号
        /// </summary>
        [FieldDescription("图标编号")]
        [Description("图标编号")]
        [Column(FieldImageIndex)]
        public string ImageIndex { get; set; } = string.Empty;

        /// <summary>
        /// 选中状态图标编号
        /// </summary>
        [FieldDescription("选中状态图标编号")]
        [Description("选中状态图标编号")]
        [Column(FieldSelectedImageIndex)]
        public string SelectedImageIndex { get; set; } = string.Empty;

        /// <summary>
        /// Web网址
        /// </summary>
        [FieldDescription("Web网址")]
        [Description("Web网址")]
        [Column(FieldNavigateUrl)]
        public string NavigateUrl { get; set; } = string.Empty;

        /// <summary>
        /// 目标窗体中打开BS
        /// </summary>
        [FieldDescription("目标窗体中打开BS")]
        [Description("目标窗体中打开BS")]
        [Column(FieldTarget)]
        public string Target { get; set; } = "fraContent";

        /// <summary>
        ///  窗体名CS
        /// </summary>
        [FieldDescription(" 窗体名CS")]
        [Description(" 窗体名CS")]
        [Column(FieldFormName)]
        public string FormName { get; set; } = string.Empty;

        /// <summary>
        /// 动态连接库CS
        /// </summary>
        [FieldDescription("动态连接库CS")]
        [Description("动态连接库CS")]
        [Column(FieldAssemblyName)]
        public string AssemblyName { get; set; } = string.Empty;

        /// <summary>
        /// 需要数据权限过滤的表(,符号分割)
        /// </summary>
        [FieldDescription("需要数据权限过滤的表(,符号分割)")]
        [Description("需要数据权限过滤的表(,符号分割)")]
        [Column(FieldPermissionScopeTables)]
        public string PermissionScopeTables { get; set; } = string.Empty;

        /// <summary>
        /// 是菜单项
        /// </summary>
        [FieldDescription("是菜单项")]
        [Description("是菜单项")]
        [Column(FieldIsMenu)]
        public int IsMenu { get; set; } = 1;

        /// <summary>
        /// 是否公开
        /// </summary>
        [FieldDescription("是否公开")]
        [Description("是否公开")]
        [Column(FieldIsPublic)]
        public int IsPublic { get; set; } = 0;

        /// <summary>
        /// 是否展开
        /// </summary>
        [FieldDescription("是否展开")]
        [Description("是否展开")]
        [Column(FieldIsExpand)]
        public int IsExpand { get; set; } = 1;

        /// <summary>
        /// 权限域
        /// </summary>
        [FieldDescription("权限域")]
        [Description("权限域")]
        [Column(FieldIsScope)]
        public int IsScope { get; set; } = 0;

        /// <summary>
        /// 是否可见
        /// </summary>
        [FieldDescription("是否可见")]
        [Description("是否可见")]
        [Column(FieldIsVisible)]
        public int IsVisible { get; set; } = 1;

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
        /// 最后呼叫时间
        /// </summary>
        [FieldDescription("最后呼叫时间")]
        [Description("最后呼叫时间")]
        [Column(FieldLastCall)]
        public DateTime? LastCall { get; set; } = null;

        /// <summary>
        /// 浏览器
        /// </summary>
        [FieldDescription("浏览器")]
        [Description("浏览器")]
        [Column(FieldWebBrowser)]
        public string WebBrowser { get; set; } = string.Empty;

        /// <summary>
        /// 认证天数
        /// </summary>
        [FieldDescription("认证天数")]
        [Description("认证天数")]
        [Column(FieldAuthorizedDays)]
        public int AuthorizedDays { get; set; } = 0;

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
            if (dr.ContainsColumn(FieldParentId))
            {
                ParentId = BaseUtil.ConvertToInt(dr[FieldParentId]);
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
            if (dr.ContainsColumn(FieldImageUrl))
            {
                ImageUrl = BaseUtil.ConvertToString(dr[FieldImageUrl]);
            }
            if (dr.ContainsColumn(FieldImageIndex))
            {
                ImageIndex = BaseUtil.ConvertToString(dr[FieldImageIndex]);
            }
            if (dr.ContainsColumn(FieldSelectedImageIndex))
            {
                SelectedImageIndex = BaseUtil.ConvertToString(dr[FieldSelectedImageIndex]);
            }
            if (dr.ContainsColumn(FieldNavigateUrl))
            {
                NavigateUrl = BaseUtil.ConvertToString(dr[FieldNavigateUrl]);
            }
            if (dr.ContainsColumn(FieldTarget))
            {
                Target = BaseUtil.ConvertToString(dr[FieldTarget]);
            }
            if (dr.ContainsColumn(FieldFormName))
            {
                FormName = BaseUtil.ConvertToString(dr[FieldFormName]);
            }
            if (dr.ContainsColumn(FieldAssemblyName))
            {
                AssemblyName = BaseUtil.ConvertToString(dr[FieldAssemblyName]);
            }
            if (dr.ContainsColumn(FieldPermissionScopeTables))
            {
                PermissionScopeTables = BaseUtil.ConvertToString(dr[FieldPermissionScopeTables]);
            }
            if (dr.ContainsColumn(FieldIsMenu))
            {
                IsMenu = BaseUtil.ConvertToInt(dr[FieldIsMenu]);
            }
            if (dr.ContainsColumn(FieldIsPublic))
            {
                IsPublic = BaseUtil.ConvertToInt(dr[FieldIsPublic]);
            }
            if (dr.ContainsColumn(FieldIsExpand))
            {
                IsExpand = BaseUtil.ConvertToInt(dr[FieldIsExpand]);
            }
            if (dr.ContainsColumn(FieldIsScope))
            {
                IsScope = BaseUtil.ConvertToInt(dr[FieldIsScope]);
            }
            if (dr.ContainsColumn(FieldIsVisible))
            {
                IsVisible = BaseUtil.ConvertToInt(dr[FieldIsVisible]);
            }
            if (dr.ContainsColumn(FieldAllowEdit))
            {
                AllowEdit = BaseUtil.ConvertToInt(dr[FieldAllowEdit]);
            }
            if (dr.ContainsColumn(FieldAllowDelete))
            {
                AllowDelete = BaseUtil.ConvertToInt(dr[FieldAllowDelete]);
            }
            if (dr.ContainsColumn(FieldLastCall))
            {
                LastCall = BaseUtil.ConvertToNullableDateTime(dr[FieldLastCall]);
            }
            if (dr.ContainsColumn(FieldWebBrowser))
            {
                WebBrowser = BaseUtil.ConvertToString(dr[FieldWebBrowser]);
            }
            if (dr.ContainsColumn(FieldAuthorizedDays))
            {
                AuthorizedDays = BaseUtil.ConvertToInt(dr[FieldAuthorizedDays]);
            }
            if (dr.ContainsColumn(FieldDescription))
            {
                Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            }
            return this;
        }

        ///<summary>
        /// 模块菜单操作
        ///</summary>
        [FieldDescription("模块菜单操作")]
        public const string CurrentTableName = "BaseModule";

        ///<summary>
        /// 子系统编码
        ///</summary>
        public const string FieldSystemCode = "SystemCode";

        ///<summary>
        /// 父节点主键
        ///</summary>
        public const string FieldParentId = "ParentId";

        ///<summary>
        /// 编号
        ///</summary>
        public const string FieldCode = "Code";

        ///<summary>
        /// 名称
        ///</summary>
        public const string FieldName = "Name";

        ///<summary>
        /// 菜单分类
        ///</summary>
        public const string FieldCategoryCode = "CategoryCode";

        ///<summary>
        /// 图标位置
        ///</summary>
        public const string FieldImageUrl = "ImageUrl";

        ///<summary>
        /// 图标编号
        ///</summary>
        public const string FieldImageIndex = "ImageIndex";

        ///<summary>
        /// 选中状态图标编号
        ///</summary>
        public const string FieldSelectedImageIndex = "SelectedImageIndex";

        ///<summary>
        /// Web网址
        ///</summary>
        public const string FieldNavigateUrl = "NavigateUrl";

        ///<summary>
        /// 目标窗体中打开BS
        ///</summary>
        public const string FieldTarget = "Target";

        ///<summary>
        ///  窗体名CS
        ///</summary>
        public const string FieldFormName = "FormName";

        ///<summary>
        /// 动态连接库CS
        ///</summary>
        public const string FieldAssemblyName = "AssemblyName";

        ///<summary>
        /// 需要数据权限过滤的表(,符号分割)
        ///</summary>
        public const string FieldPermissionScopeTables = "PermissionScopeTables";

        ///<summary>
        /// 是菜单项
        ///</summary>
        public const string FieldIsMenu = "IsMenu";

        ///<summary>
        /// 是否公开
        ///</summary>
        public const string FieldIsPublic = "IsPublic";

        ///<summary>
        /// 是否展开
        ///</summary>
        public const string FieldIsExpand = "IsExpand";

        ///<summary>
        /// 权限域
        ///</summary>
        public const string FieldIsScope = "IsScope";

        ///<summary>
        /// 是否可见
        ///</summary>
        public const string FieldIsVisible = "IsVisible";

        ///<summary>
        /// 允许编辑
        ///</summary>
        public const string FieldAllowEdit = "AllowEdit";

        ///<summary>
        /// 允许删除
        ///</summary>
        public const string FieldAllowDelete = "AllowDelete";

        ///<summary>
        /// 最后呼叫时间
        ///</summary>
        public const string FieldLastCall = "LastCall";

        ///<summary>
        /// 浏览器
        ///</summary>
        public const string FieldWebBrowser = "WebBrowser";

        ///<summary>
        /// 认证天数
        ///</summary>
        public const string FieldAuthorizedDays = "AuthorizedDays";

        ///<summary>
        /// 描述
        ///</summary>
        public const string FieldDescription = "Description";
    }
}
