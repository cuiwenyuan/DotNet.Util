//-----------------------------------------------------------------------
// <copyright file="organizeScopeEntity.cs" company="DotNet">
//     Copyright (C) 2021, All rights reserved.
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
    /// 2013-12-24 版本：1.0 JiRiGaLa 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2013-12-24</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseOrganizeScopeEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; } = null;

        /// <summary>
        /// 什么类型的
        /// </summary>
        public string ResourceCategory { get; set; } = string.Empty;

        /// <summary>
        /// 什么资源主键
        /// </summary>
        public string ResourceId { get; set; } = string.Empty;

        /// <summary>
        /// 有什么权限（模块菜单）主键
        /// </summary>
        public int? PermissionId { get; set; } = null;

        /// <summary>
        /// 全部数据
        /// </summary>
        public int? AllData { get; set; } = null;

        /// <summary>
        /// 所在省
        /// </summary>
        public int? Province { get; set; } = 0;

        /// <summary>
        /// 所在市
        /// </summary>
        public int? City { get; set; } = 0;

        /// <summary>
        /// 所在区/县
        /// </summary>
        public int? District { get; set; } = 0;

        /// <summary>
        /// 所在街道
        /// </summary>
        public int? Street { get; set; } = 0;

        /// <summary>
        /// 用户所在公司的数据
        /// </summary>
        public int? UserCompany { get; set; } = 0;

        /// <summary>
        /// 用户所在分公司的数据
        /// </summary>
        public int? UserSubCompany { get; set; } = 0;

        /// <summary>
        /// 用户所在部门的数据
        /// </summary>
        public int? UserDepartment { get; set; } = 0;

        /// <summary>
        /// 用户所在子部门的数据
        /// </summary>
        public int? UserSubDepartment { get; set; } = 0;

        /// <summary>
        /// 用户所在工作组的数据
        /// </summary>
        public int? UserWorkgroup { get; set; } = 0;

        /// <summary>
        /// 仅仅用户自己的数据
        /// </summary>
        public int OnlyOwnData { get; set; } = 1;

        /// <summary>
        /// 不允许查看数据
        /// </summary>
        public int? NotAllowed { get; set; } = 0;

        /// <summary>
        /// 按明细设置
        /// </summary>
        public int? ByDetails { get; set; } = 0;

        /// <summary>
        /// 包含子节点的数据
        /// </summary>
        public int ContainChild { get; set; } = 0;

        /// <summary>
        /// 有效标示
        /// </summary>
        public int Enabled { get; set; } = 1;

        /// <summary>
        /// 是否删除
        /// </summary>
        public int DeletionStateCode { get; set; } = 0;

        /// <summary>
        /// 备注
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人用户编号
        /// </summary>
        public string CreateUserId { get; set; } = string.Empty;

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; } = string.Empty;

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改人用户编号
        /// </summary>
        public string ModifiedUserId { get; set; } = string.Empty;

        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifiedBy { get; set; } = string.Empty;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExtend(dr);
            Id = BaseUtil.ConvertToString(dr[FieldId]);
            ResourceCategory = BaseUtil.ConvertToString(dr[FieldResourceCategory]);
            ResourceId = BaseUtil.ConvertToString(dr[FieldResourceId]);
            PermissionId = BaseUtil.ConvertToNullableInt(dr[FieldPermissionId]);
            AllData = BaseUtil.ConvertToNullableInt(dr[FieldAllData]);
            Province = BaseUtil.ConvertToNullableInt(dr[FieldProvince]);
            City = BaseUtil.ConvertToNullableInt(dr[FieldCity]);
            District = BaseUtil.ConvertToNullableInt(dr[FieldDistrict]);
            Street = BaseUtil.ConvertToNullableInt(dr[FieldStreet]);
            UserCompany = BaseUtil.ConvertToNullableInt(dr[FieldUserCompany]);
            UserSubCompany = BaseUtil.ConvertToNullableInt(dr[FieldUserSubCompany]);
            UserDepartment = BaseUtil.ConvertToNullableInt(dr[FieldUserDepartment]);
            UserSubDepartment = BaseUtil.ConvertToNullableInt(dr[FieldUserSubDepartment]);
            UserWorkgroup = BaseUtil.ConvertToNullableInt(dr[FieldUserWorkgroup]);
            OnlyOwnData = BaseUtil.ConvertToInt(dr[FieldOnlyOwnData]);
            NotAllowed = BaseUtil.ConvertToNullableInt(dr[FieldNotAllowed]);
            ByDetails = BaseUtil.ConvertToNullableInt(dr[FieldByDetails]);
            ContainChild = BaseUtil.ConvertToInt(dr[FieldContainChild]);
            Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            DeletionStateCode = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            CreateOn = BaseUtil.ConvertToNullableDateTime(dr[FieldCreateTime]);
            CreateUserId = BaseUtil.ConvertToString(dr[FieldCreateUserId]);
            CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            ModifiedOn = BaseUtil.ConvertToNullableDateTime(dr[FieldUpdateTime]);
            ModifiedUserId = BaseUtil.ConvertToString(dr[FieldUpdateUserId]);
            ModifiedBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            return this;
        }

        ///<summary>
        /// 基于组织机构的权限范围
        ///</summary>
        [NonSerialized]
        public const string TableName = "BaseOrganizeScope";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 什么类型的
        ///</summary>
        [NonSerialized]
        public const string FieldResourceCategory = "ResourceCategory";

        ///<summary>
        /// 什么资源主键
        ///</summary>
        [NonSerialized]
        public const string FieldResourceId = "ResourceId";

        ///<summary>
        /// 有什么权限（模块菜单）主键
        ///</summary>
        [NonSerialized]
        public const string FieldPermissionId = "PermissionId";

        ///<summary>
        /// 全部数据
        ///</summary>
        [NonSerialized]
        public const string FieldAllData = "AllData";

        /// <summary>
        /// 所在省
        /// </summary>
        [NonSerialized]
        public const string FieldProvince = "Province";

        /// <summary>
        /// 所在市
        /// </summary>
        [NonSerialized]
        public const string FieldCity = "City";

        /// <summary>
        /// 所在区/县
        /// </summary>
        [NonSerialized]
        public const string FieldDistrict = "District";

        /// <summary>
        /// 所在街道
        /// </summary>
        [NonSerialized]
        public const string FieldStreet = "Street";

        ///<summary>
        /// 用户所在公司的数据
        ///</summary>
        [NonSerialized]
        public const string FieldUserCompany = "UserCompany";

        ///<summary>
        /// 用户所在分公司的数据
        ///</summary>
        [NonSerialized]
        public const string FieldUserSubCompany = "UserSubCompany";

        ///<summary>
        /// 用户所在部门的数据
        ///</summary>
        [NonSerialized]
        public const string FieldUserDepartment = "UserDepartment";

        ///<summary>
        /// 用户所在子部门的数据
        ///</summary>
        [NonSerialized]
        public const string FieldUserSubDepartment = "UserSubDepartment";

        ///<summary>
        /// 用户所在工作组的数据
        ///</summary>
        [NonSerialized]
        public const string FieldUserWorkgroup = "UserWorkgroup";

        ///<summary>
        /// 仅仅用户自己的数据
        ///</summary>
        [NonSerialized]
        public const string FieldOnlyOwnData = "OnlyOwnData";

        ///<summary>
        /// 不允许查看数据
        ///</summary>
        [NonSerialized]
        public const string FieldNotAllowed = "NotAllowed";

        ///<summary>
        /// 
        ///</summary>
        [NonSerialized]
        public const string FieldByDetails = "ByDetails";

        ///<summary>
        /// 包含子节点的数据
        ///</summary>
        [NonSerialized]
        public const string FieldContainChild = "ContainChild";

        ///<summary>
        /// 有效标示
        ///</summary>
        [NonSerialized]
        public const string FieldEnabled = "Enabled";

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
