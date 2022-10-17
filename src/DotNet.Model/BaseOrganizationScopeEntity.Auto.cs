//-----------------------------------------------------------------------
// <copyright file="BaseOrganizationScopeEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2022, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseOrganizationScopeEntity
    /// 组织机构权限范围
    /// 
    /// 修改记录
    /// 
    /// 2021-09-27 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2021-09-27</date>
    /// </author>
    /// </summary>
    public partial class BaseOrganizationScopeEntity : BaseEntity
    {
        /// <summary>
        /// 什么类型的
        /// </summary>
        [FieldDescription("什么类型的")]
        public string ResourceCategory { get; set; } = string.Empty;

        /// <summary>
        /// 什么资源主键
        /// </summary>
        [FieldDescription("什么资源主键")]
        public string ResourceId { get; set; } = string.Empty;

        /// <summary>
        /// 有什么权限（模块菜单）主键
        /// </summary>
        [FieldDescription("有什么权限（模块菜单）主键")]
        public int? PermissionId { get; set; } = null;

        /// <summary>
        /// 全部数据
        /// </summary>
        [FieldDescription("全部数据")]
        public int? AllData { get; set; } = null;

        /// <summary>
        /// 所在的省
        /// </summary>
        [FieldDescription("所在的省")]
        public int? Province { get; set; } = null;

        /// <summary>
        /// 所在的市
        /// </summary>
        [FieldDescription("所在的市")]
        public int? City { get; set; } = null;

        /// <summary>
        /// 所在的县/区
        /// </summary>
        [FieldDescription("所在的县/区")]
        public int? District { get; set; } = null;

        /// <summary>
        /// 街道
        /// </summary>
        [FieldDescription("街道")]
        public int? Street { get; set; } = null;

        /// <summary>
        /// 用户所在公司的数据
        /// </summary>
        [FieldDescription("用户所在公司的数据")]
        public int? UserCompany { get; set; } = null;

        /// <summary>
        /// 用户所在分公司的数据
        /// </summary>
        [FieldDescription("用户所在分公司的数据")]
        public int? UserSubCompany { get; set; } = null;

        /// <summary>
        /// 用户所在部门的数据
        /// </summary>
        [FieldDescription("用户所在部门的数据")]
        public int? UserDepartment { get; set; } = null;

        /// <summary>
        /// 用户所在子部门的数据
        /// </summary>
        [FieldDescription("用户所在子部门的数据")]
        public int? UserSubDepartment { get; set; } = null;

        /// <summary>
        /// 用户所在工作组的数据
        /// </summary>
        [FieldDescription("用户所在工作组的数据")]
        public int? UserWorkgroup { get; set; } = null;

        /// <summary>
        /// 仅仅用户自己的数据
        /// </summary>
        [FieldDescription("仅仅用户自己的数据")]
        public int OnlyOwnData { get; set; } = 1;

        /// <summary>
        /// 不允许查看数据
        /// </summary>
        [FieldDescription("不允许查看数据")]
        public int? NotAllowed { get; set; } = 0;

        /// <summary>
        /// 按详细设置
        /// </summary>
        [FieldDescription("按详细设置")]
        public int? ByDetails { get; set; } = 0;

        /// <summary>
        /// 包含子节点的数据
        /// </summary>
        [FieldDescription("包含子节点的数据")]
        public int ContainChild { get; set; }

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
            if (dr.ContainsColumn(FieldResourceCategory))
            {
                ResourceCategory = BaseUtil.ConvertToString(dr[FieldResourceCategory]);
            }
            if (dr.ContainsColumn(FieldResourceId))
            {
                ResourceId = BaseUtil.ConvertToString(dr[FieldResourceId]);
            }
            if (dr.ContainsColumn(FieldPermissionId))
            {
                PermissionId = BaseUtil.ConvertToNullableInt(dr[FieldPermissionId]);
            }
            if (dr.ContainsColumn(FieldAllData))
            {
                AllData = BaseUtil.ConvertToNullableByteInt(dr[FieldAllData]);
            }
            if (dr.ContainsColumn(FieldProvince))
            {
                Province = BaseUtil.ConvertToNullableByteInt(dr[FieldProvince]);
            }
            if (dr.ContainsColumn(FieldCity))
            {
                City = BaseUtil.ConvertToNullableByteInt(dr[FieldCity]);
            }
            if (dr.ContainsColumn(FieldDistrict))
            {
                District = BaseUtil.ConvertToNullableByteInt(dr[FieldDistrict]);
            }
            if (dr.ContainsColumn(FieldStreet))
            {
                Street = BaseUtil.ConvertToNullableByteInt(dr[FieldStreet]);
            }
            if (dr.ContainsColumn(FieldUserCompany))
            {
                UserCompany = BaseUtil.ConvertToNullableByteInt(dr[FieldUserCompany]);
            }
            if (dr.ContainsColumn(FieldUserSubCompany))
            {
                UserSubCompany = BaseUtil.ConvertToNullableByteInt(dr[FieldUserSubCompany]);
            }
            if (dr.ContainsColumn(FieldUserDepartment))
            {
                UserDepartment = BaseUtil.ConvertToNullableByteInt(dr[FieldUserDepartment]);
            }
            if (dr.ContainsColumn(FieldUserSubDepartment))
            {
                UserSubDepartment = BaseUtil.ConvertToNullableByteInt(dr[FieldUserSubDepartment]);
            }
            if (dr.ContainsColumn(FieldUserWorkgroup))
            {
                UserWorkgroup = BaseUtil.ConvertToNullableByteInt(dr[FieldUserWorkgroup]);
            }
            if (dr.ContainsColumn(FieldOnlyOwnData))
            {
                OnlyOwnData = BaseUtil.ConvertToInt(dr[FieldOnlyOwnData]);
            }
            if (dr.ContainsColumn(FieldNotAllowed))
            {
                NotAllowed = BaseUtil.ConvertToNullableByteInt(dr[FieldNotAllowed]);
            }
            if (dr.ContainsColumn(FieldByDetails))
            {
                ByDetails = BaseUtil.ConvertToNullableByteInt(dr[FieldByDetails]);
            }
            if (dr.ContainsColumn(FieldContainChild))
            {
                ContainChild = BaseUtil.ConvertToInt(dr[FieldContainChild]);
            }
            if (dr.ContainsColumn(FieldDescription))
            {
                Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            }
            return this;
        }

        ///<summary>
        /// 组织机构权限范围
        ///</summary>
        [FieldDescription("组织机构权限范围")]
        public const string CurrentTableName = "BaseOrganizationScope";

        ///<summary>
        /// 什么类型的
        ///</summary>
        public const string FieldResourceCategory = "ResourceCategory";

        ///<summary>
        /// 什么资源主键
        ///</summary>
        public const string FieldResourceId = "ResourceId";

        ///<summary>
        /// 有什么权限（模块菜单）主键
        ///</summary>
        public const string FieldPermissionId = "PermissionId";

        ///<summary>
        /// 全部数据
        ///</summary>
        public const string FieldAllData = "AllData";

        ///<summary>
        /// 所在的省
        ///</summary>
        public const string FieldProvince = "Province";

        ///<summary>
        /// 所在的市
        ///</summary>
        public const string FieldCity = "City";

        ///<summary>
        /// 所在的县/区
        ///</summary>
        public const string FieldDistrict = "District";

        ///<summary>
        /// 街道
        ///</summary>
        public const string FieldStreet = "Street";

        ///<summary>
        /// 用户所在公司的数据
        ///</summary>
        public const string FieldUserCompany = "UserCompany";

        ///<summary>
        /// 用户所在分公司的数据
        ///</summary>
        public const string FieldUserSubCompany = "UserSubCompany";

        ///<summary>
        /// 用户所在部门的数据
        ///</summary>
        public const string FieldUserDepartment = "UserDepartment";

        ///<summary>
        /// 用户所在子部门的数据
        ///</summary>
        public const string FieldUserSubDepartment = "UserSubDepartment";

        ///<summary>
        /// 用户所在工作组的数据
        ///</summary>
        public const string FieldUserWorkgroup = "UserWorkgroup";

        ///<summary>
        /// 仅仅用户自己的数据
        ///</summary>
        public const string FieldOnlyOwnData = "OnlyOwnData";

        ///<summary>
        /// 不允许查看数据
        ///</summary>
        public const string FieldNotAllowed = "NotAllowed";

        ///<summary>
        /// 按详细设置
        ///</summary>
        public const string FieldByDetails = "ByDetails";

        ///<summary>
        /// 包含子节点的数据
        ///</summary>
        public const string FieldContainChild = "ContainChild";

        ///<summary>
        /// 描述
        ///</summary>
        public const string FieldDescription = "Description";
    }
}
