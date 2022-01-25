//-----------------------------------------------------------------------
// <copyright file="BaseOrganizationScopeEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// organizeScopeEntity
    /// 基于组织机构的权限范围
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
        /// 主键
        /// </summary>
        [FieldDescription("主键")]
        public int Id { get; set; }

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
        /// 基于组织机构的权限范围
        ///</summary>
        [FieldDescription("基于组织机构的权限范围")]
        public const string CurrentTableName = "BaseOrganizationScope";

        ///<summary>
        /// 主键
        ///</summary>
        public const string FieldId = "Id";

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
