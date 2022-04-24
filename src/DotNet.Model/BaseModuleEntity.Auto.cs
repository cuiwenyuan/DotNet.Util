//-----------------------------------------------------------------------
// <copyright file="BaseModuleEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2022, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;


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
    public partial class BaseModuleEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [FieldDescription("主键")]
        public int Id { get; set; }

        /// <summary>
        /// 子系统编码
        /// </summary>
        [FieldDescription("子系统编码")]
        public string SystemCode { get; set; } = "Base";

        /// <summary>
        /// 父节点主键
        /// </summary>
        [FieldDescription("父节点主键")]
        public int ParentId { get; set; } = 0;

        /// <summary>
        /// 编号
        /// </summary>
        [FieldDescription("编号")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 名称
        /// </summary>
        [FieldDescription("名称")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 菜单分类
        /// </summary>
        [FieldDescription("菜单分类")]
        public string CategoryCode { get; set; } = "Application";

        /// <summary>
        /// 图标位置
        /// </summary>
        [FieldDescription("图标位置")]
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// 图标编号
        /// </summary>
        [FieldDescription("图标编号")]
        public string ImageIndex { get; set; } = string.Empty;

        /// <summary>
        /// 选中状态图标编号
        /// </summary>
        [FieldDescription("选中状态图标编号")]
        public string SelectedImageIndex { get; set; } = string.Empty;

        /// <summary>
        /// Web网址
        /// </summary>
        [FieldDescription("Web网址")]
        public string NavigateUrl { get; set; } = string.Empty;

        /// <summary>
        /// 目标窗体中打开BS
        /// </summary>
        [FieldDescription("目标窗体中打开BS")]
        public string Target { get; set; } = "fraContent";

        /// <summary>
        ///  窗体名CS
        /// </summary>
        [FieldDescription(" 窗体名CS")]
        public string FormName { get; set; } = string.Empty;

        /// <summary>
        /// 动态连接库CS
        /// </summary>
        [FieldDescription("动态连接库CS")]
        public string AssemblyName { get; set; } = string.Empty;

        /// <summary>
        /// 需要数据权限过滤的表(,符号分割)
        /// </summary>
        [FieldDescription("需要数据权限过滤的表(,符号分割)")]
        public string PermissionScopeTables { get; set; } = string.Empty;

        /// <summary>
        /// 是菜单项
        /// </summary>
        [FieldDescription("是菜单项")]
        public int IsMenu { get; set; } = 1;

        /// <summary>
        /// 是否公开
        /// </summary>
        [FieldDescription("是否公开")]
        public int IsPublic { get; set; } = 1;

        /// <summary>
        /// 是否展开
        /// </summary>
        [FieldDescription("是否展开")]
        public int IsExpand { get; set; } = 1;

        /// <summary>
        /// 权限域
        /// </summary>
        [FieldDescription("权限域")]
        public int IsScope { get; set; } = 0;

        /// <summary>
        /// 是否可见
        /// </summary>
        [FieldDescription("是否可见")]
        public int IsVisible { get; set; } = 1;

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
        /// 最后呼叫时间
        /// </summary>
        [FieldDescription("最后呼叫时间")]
        public DateTime? LastCall { get; set; } = null;

        /// <summary>
        /// 浏览器
        /// </summary>
        [FieldDescription("浏览器")]
        public string WebBrowser { get; set; } = string.Empty;

        /// <summary>
        /// 认证天数
        /// </summary>
        [FieldDescription("认证天数")]
        public int AuthorizedDays { get; set; } = 0;

        /// <summary>
        /// 描述
        /// </summary>
        [FieldDescription("描述")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 排序编号
        /// </summary>
        [FieldDescription("排序编号")]
        public int SortCode { get; set; } = 0;

        /// <summary>
        /// 是否删除
        /// </summary>
        [FieldDescription("是否删除")]
        public int Deleted { get; set; } = 0;

        /// <summary>
        /// 是否有效
        /// </summary>
        [FieldDescription("是否有效")]
        public int Enabled { get; set; } = 1;

        /// <summary>
        /// 创建时间
        /// </summary>
        [FieldDescription("创建时间")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人编号
        /// </summary>
        [FieldDescription("创建人编号")]
        public int CreateUserId { get; set; } = 0;

        /// <summary>
        /// 创建人用户名
        /// </summary>
        [FieldDescription("创建人用户名")]
        public string CreateUserName { get; set; } = string.Empty;

        /// <summary>
        /// 创建人姓名
        /// </summary>
        [FieldDescription("创建人姓名")]
        public string CreateBy { get; set; } = string.Empty;

        /// <summary>
        /// 创建IP
        /// </summary>
        [FieldDescription("创建IP")]
        public string CreateIp { get; set; } = string.Empty;

        /// <summary>
        /// 修改时间
        /// </summary>
        [FieldDescription("修改时间")]
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改人编号
        /// </summary>
        [FieldDescription("修改人编号")]
        public int UpdateUserId { get; set; } = 0;

        /// <summary>
        /// 修改人用户名
        /// </summary>
        [FieldDescription("修改人用户名")]
        public string UpdateUserName { get; set; } = string.Empty;

        /// <summary>
        /// 修改人姓名
        /// </summary>
        [FieldDescription("修改人姓名")]
        public string UpdateBy { get; set; } = string.Empty;

        /// <summary>
        /// 修改IP
        /// </summary>
        [FieldDescription("修改IP")]
        public string UpdateIp { get; set; } = string.Empty;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExtend(dr);
            if (dr.ContainsColumn(FieldId))
            {
                Id = BaseUtil.ConvertToInt(dr[FieldId]);
            }
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
            if (dr.ContainsColumn(FieldSortCode))
            {
                SortCode = BaseUtil.ConvertToInt(dr[FieldSortCode]);
            }
            if (dr.ContainsColumn(FieldDeleted))
            {
                Deleted = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            }
            if (dr.ContainsColumn(FieldEnabled))
            {
                Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            }
            if (dr.ContainsColumn(FieldCreateTime))
            {
                CreateTime = BaseUtil.ConvertToDateTime(dr[FieldCreateTime]);
            }
            if (dr.ContainsColumn(FieldCreateUserId))
            {
                CreateUserId = BaseUtil.ConvertToInt(dr[FieldCreateUserId]);
            }
            if (dr.ContainsColumn(FieldCreateUserName))
            {
                CreateUserName = BaseUtil.ConvertToString(dr[FieldCreateUserName]);
            }
            if (dr.ContainsColumn(FieldCreateBy))
            {
                CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            }
            if (dr.ContainsColumn(FieldCreateIp))
            {
                CreateIp = BaseUtil.ConvertToString(dr[FieldCreateIp]);
            }
            if (dr.ContainsColumn(FieldUpdateTime))
            {
                UpdateTime = BaseUtil.ConvertToDateTime(dr[FieldUpdateTime]);
            }
            if (dr.ContainsColumn(FieldUpdateUserId))
            {
                UpdateUserId = BaseUtil.ConvertToInt(dr[FieldUpdateUserId]);
            }
            if (dr.ContainsColumn(FieldUpdateUserName))
            {
                UpdateUserName = BaseUtil.ConvertToString(dr[FieldUpdateUserName]);
            }
            if (dr.ContainsColumn(FieldUpdateBy))
            {
                UpdateBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            }
            if (dr.ContainsColumn(FieldUpdateIp))
            {
                UpdateIp = BaseUtil.ConvertToString(dr[FieldUpdateIp]);
            }
            return this;
        }

        ///<summary>
        /// 模块菜单操作
        ///</summary>
        [FieldDescription("模块菜单操作")]
        public const string CurrentTableName = "BaseModule";

        ///<summary>
        /// 主键
        ///</summary>
        public const string FieldId = "Id";

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

        ///<summary>
        /// 排序编号
        ///</summary>
        public const string FieldSortCode = "SortCode";

        ///<summary>
        /// 是否删除
        ///</summary>
        public const string FieldDeleted = "Deleted";

        ///<summary>
        /// 是否有效
        ///</summary>
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 创建时间
        ///</summary>
        public const string FieldCreateTime = "CreateTime";

        ///<summary>
        /// 创建人编号
        ///</summary>
        public const string FieldCreateUserId = "CreateUserId";

        ///<summary>
        /// 创建人用户名
        ///</summary>
        public const string FieldCreateUserName = "CreateUserName";

        ///<summary>
        /// 创建人姓名
        ///</summary>
        public const string FieldCreateBy = "CreateBy";

        ///<summary>
        /// 创建IP
        ///</summary>
        public const string FieldCreateIp = "CreateIp";

        ///<summary>
        /// 修改时间
        ///</summary>
        public const string FieldUpdateTime = "UpdateTime";

        ///<summary>
        /// 修改人编号
        ///</summary>
        public const string FieldUpdateUserId = "UpdateUserId";

        ///<summary>
        /// 修改人用户名
        ///</summary>
        public const string FieldUpdateUserName = "UpdateUserName";

        ///<summary>
        /// 修改人姓名
        ///</summary>
        public const string FieldUpdateBy = "UpdateBy";

        ///<summary>
        /// 修改IP
        ///</summary>
        public const string FieldUpdateIp = "UpdateIp";
    }
}
