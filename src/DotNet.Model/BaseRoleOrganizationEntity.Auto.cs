﻿//-----------------------------------------------------------------------
// <copyright file="BaseRoleOrganizationEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2022, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseRoleOrganizationEntity
    /// 角色组织机构关系表
    /// 
    /// 修改记录
    /// 
    /// 2022-02-07 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2022-02-07</date>
    /// </author>
    /// </summary>
    public partial class BaseRoleOrganizationEntity : BaseEntity
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
        /// 角色主键
        /// </summary>
        [FieldDescription("角色主键")]
        public int RoleId { get; set; } = 0;

        /// <summary>
        /// 组织机构主键
        /// </summary>
        [FieldDescription("组织机构主键")]
        public int OrganizationId { get; set; } = 0;

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
            if (dr.ContainsColumn(FieldRoleId))
            {
                RoleId = BaseUtil.ConvertToInt(dr[FieldRoleId]);
            }
            if (dr.ContainsColumn(FieldOrganizationId))
            {
                OrganizationId = BaseUtil.ConvertToInt(dr[FieldOrganizationId]);
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
        /// 角色组织机构关系表
        ///</summary>
        [FieldDescription("角色组织机构关系表")]
        public const string CurrentTableName = "BaseRoleOrganization";

        ///<summary>
        /// 主键
        ///</summary>
        public const string FieldId = "Id";

        ///<summary>
        /// 子系统编码
        ///</summary>
        public const string FieldSystemCode = "SystemCode";

        ///<summary>
        /// 角色主键
        ///</summary>
        public const string FieldRoleId = "RoleId";

        ///<summary>
        /// 组织机构主键
        ///</summary>
        public const string FieldOrganizationId = "OrganizationId";

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
