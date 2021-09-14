//--------------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//--------------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseModuleEntity
    /// 模块（菜单）表
    /// 
    /// 修改记录
    /// 
    /// 2012-05-22 版本：1.0 JiRiGaLa 创建主键。
    /// 
    /// <author>
    ///     <name>JiRiGaLa</name>
    ///     <date>2012-05-22</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseModuleEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [FieldDescription("主键", false)]
        public string Id { get; set; }

        /// <summary>
        /// 父节点主键
        /// </summary>
        [FieldDescription("父节点主键")]
        public string ParentId { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        [FieldDescription("编号")]
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [FieldDescription("名称")]
        public string FullName { get; set; }

        /// <summary>
        /// 菜单分类System\Application
        /// </summary>
        [FieldDescription("分类")]
        public string CategoryCode { get; set; }

        /// <summary>
        /// 图标编号
        /// </summary>
        [FieldDescription("图标编号")]
        public string ImageIndex { get; set; }

        /// <summary>
        /// 选中状态图标编号
        /// </summary>
        [FieldDescription("选中状态图标编号")]
        public string SelectedImageIndex { get; set; }

        /// <summary>
        /// 导航地址(Web网址)[B\S]
        /// </summary>
        [FieldDescription("导航地址")]
        public string NavigateUrl { get; set; }

        /// <summary>
        /// 浏览器
        /// </summary>
        /// public String WebBrowser { get; set; }

        /// <summary>
        /// 图标图片地址[B\S]
        /// </summary>
        [FieldDescription("图标图片地址")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// 目标窗体中打开[B\S]
        /// </summary>
        [FieldDescription("目标窗体")]
        public string Target { get; set; }

        /// <summary>
        /// 窗体名[C\S]
        /// </summary>
        [FieldDescription("窗体名")]
        public string FormName { get; set; }

        /// <summary>
        /// 动态连接库[C\S]
        /// </summary>
        [FieldDescription("动态连接库")]
        public string AssemblyName { get; set; }
        /// <summary>
        /// 默认授权天数、默认多少天有数、0表示无限制
        /// </summary>
        [FieldDescription("默认授权天数")]
        public int? AuthorizedDays { get; set; } = 0;

        /// <summary>
        /// 排序码
        /// </summary>
        [FieldDescription("排序码", false)]
        public int? SortCode { get; set; } = 0;
        /// <summary>
        /// 是否有效
        /// </summary>
        [FieldDescription("是否有效")]
        public int? Enabled { get; set; } = 1;
        /// <summary>
        /// 是否删除
        /// </summary>
        [FieldDescription("是否删除", false)]
        public int? DeletionStateCode { get; set; } = 0;

        private int? _isMenu = 0;
        /// <summary>
        /// 是菜单项
        /// </summary>
        [FieldDescription("是菜单项")]
        public int? IsMenu
        {
            get => _isMenu;
            set => _isMenu = value;
        }
        /// <summary>
        /// 是否公开
        /// </summary>
        [FieldDescription("是否公开")]

        public int? IsPublic { get; set; } = 0;

        private int? _isVisible = 1;
        /// <summary>
        /// 是否可见
        /// </summary>
        [FieldDescription("是否可见")]
        public int? IsVisible
        {
            get => _isVisible;
            set => _isVisible = value;
        }
        /// <summary>
        /// 权限域（需要数据权限）
        /// </summary>
        [FieldDescription("权限域")]
        public int? IsScope { get; set; } = 0;

        private DateTime? _lastCall = null;
        /// <summary>
        /// 最后调用时间
        /// </summary>
        [FieldDescription("最后调用时间", false)]
        public DateTime? LastCall
        {
            get => _lastCall;
            set => _lastCall = value;
        }
        /// <summary>
        /// 展开状态
        /// </summary>
        [FieldDescription("展开状态")]
        public int? Expand { get; set; } = 0;

        private int? _allowEdit = 1;
        /// <summary>
        /// 允许编辑
        /// </summary>
        [FieldDescription("允许编辑", false)]
        public int? AllowEdit
        {
            get => _allowEdit;
            set => _allowEdit = value;
        }
        /// <summary>
        /// 允许删除
        /// </summary>
        [FieldDescription("允许删除", false)]
        public int? AllowDelete { get; set; } = 1;

        /// <summary>
        /// 备注
        /// </summary>
        [FieldDescription("备注")]
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [FieldDescription("创建时间", false)]
        public DateTime? CreateOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人用户编号
        /// </summary>
        [FieldDescription("创建人用户编号", false)]
        public string CreateUserId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [FieldDescription("创建人", false)]
        public string CreateBy { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [FieldDescription("修改时间", false)]
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改人用户编号
        /// </summary>
        [FieldDescription("修改人用户编号", false)]
        public string ModifiedUserId { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        [FieldDescription("修改人", false)]
        public string ModifiedBy { get; set; }

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            Id = BaseUtil.ConvertToString(dr[FieldId]);
            ParentId = BaseUtil.ConvertToString(dr[FieldParentId]);
            Code = BaseUtil.ConvertToString(dr[FieldCode]);
            FullName = BaseUtil.ConvertToString(dr[FieldFullName]);
            CategoryCode = BaseUtil.ConvertToString(dr[FieldCategoryCode]);
            ImageIndex = BaseUtil.ConvertToString(dr[FieldImageIndex]);
            SelectedImageIndex = BaseUtil.ConvertToString(dr[FieldSelectedImageIndex]);
            NavigateUrl = BaseUtil.ConvertToString(dr[FieldNavigateUrl]);
            // WebBrowser = BaseUtil.ConvertToString(dr[BaseModuleEntity.FieldWebBrowser]);
            ImageUrl = BaseUtil.ConvertToString(dr[FieldImageUrl]);
            Target = BaseUtil.ConvertToString(dr[FieldTarget]);
            FormName = BaseUtil.ConvertToString(dr[FieldFormName]);
            AssemblyName = BaseUtil.ConvertToString(dr[FieldAssemblyName]);
            // PermissionScopeTables = BaseUtil.ConvertToString(dr[BaseModuleEntity.FieldPermissionScopeTables]);
            AuthorizedDays = BaseUtil.ConvertToNullableInt(dr[FieldAuthorizedDays]);
            SortCode = BaseUtil.ConvertToInt(dr[FieldSortCode]);
            Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            DeletionStateCode = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            IsMenu = BaseUtil.ConvertToInt(dr[FieldIsMenu]);
            IsPublic = BaseUtil.ConvertToInt(dr[FieldIsPublic]);
            IsScope = BaseUtil.ConvertToInt(dr[FieldIsScope]);
            IsVisible = BaseUtil.ConvertToInt(dr[FieldIsVisible]);
            Expand = BaseUtil.ConvertToInt(dr[FieldExpand]);
            AllowEdit = BaseUtil.ConvertToInt(dr[FieldAllowEdit]);
            AllowDelete = BaseUtil.ConvertToInt(dr[FieldAllowDelete]);
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
        /// 模块（菜单）表
        ///</summary>
        [NonSerialized]
        public const string TableName = "BaseModule";

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
        /// 分类
        ///</summary>
        [NonSerialized]
        public const string FieldCategoryCode = "CategoryCode";

        ///<summary>
        /// 名称
        ///</summary>
        [NonSerialized]
        public const string FieldFullName = "FullName";

        ///<summary>
        /// 图标编号
        ///</summary>
        [NonSerialized]
        public const string FieldImageIndex = "ImageIndex";

        ///<summary>
        /// 选中状态图标编号
        ///</summary>
        [NonSerialized]
        public const string FieldSelectedImageIndex = "SelectedImageIndex";

        ///<summary>
        /// 导航地址(Web网址)[B\S]
        ///</summary>
        [NonSerialized]
        public const string FieldNavigateUrl = "NavigateUrl";

        ///<summary>
        /// 浏览器
        ///</summary>
        /// [NonSerialized]
        /// public const string FieldWebBrowser = "WebBrowser";

        ///<summary>
        /// 图标图片地址[B\S]
        ///</summary>
        [NonSerialized]
        public const string FieldImageUrl = "ImageUrl";

        ///<summary>
        /// 目标窗体中打开[B\S]
        ///</summary>
        [NonSerialized]
        public const string FieldTarget = "Target";

        ///<summary>
        /// 窗体名[C\S]
        ///</summary>
        [NonSerialized]
        public const string FieldFormName = "FormName";

        ///<summary>
        /// 动态连接库[C\S]
        ///</summary>
        [NonSerialized]
        public const string FieldAssemblyName = "AssemblyName";

        /////<summary>
        ///// 需要数据权限过滤的表(英文逗号分割)
        /////</summary>
        //[NonSerialized]
        //public const string FieldPermissionScopeTables = "PermissionScopeTables";

        ///<summary>
        /// 最后呼叫时间
        ///</summary>
        [NonSerialized]
        public const string FieldLastCall = "LastCall";

        /// <summary>
        /// 默认授权天数、默认多少天有数、0表示无限制
        /// </summary>
        [NonSerialized]
        public const string FieldAuthorizedDays = "AuthorizedDays";

        ///<summary>
        /// 排序码
        ///</summary>
        [NonSerialized]
        public const string FieldSortCode = "SortCode";

        ///<summary>
        /// 有效
        ///</summary>
        [NonSerialized]
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 是否删除
        ///</summary>
        [NonSerialized]
        public const string FieldDeleted = "DeletionStateCode";

        ///<summary>
        /// 是菜单项
        ///</summary>
        [NonSerialized]
        public const string FieldIsMenu = "IsMenu";

        ///<summary>
        /// 是否公开
        ///</summary>
        [NonSerialized]
        public const string FieldIsPublic = "IsPublic";

        ///<summary>
        /// 展开状态
        ///</summary>
        [NonSerialized]
        public const string FieldExpand = "Expand";

        ///<summary>
        /// 权限域
        ///</summary>
        [NonSerialized]
        public const string FieldIsScope = "IsScope";

        ///<summary>
        /// 是否可见
        ///</summary>
        [NonSerialized]
        public const string FieldIsVisible = "IsVisible";

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
