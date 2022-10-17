//-----------------------------------------------------------------------
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
    /// 角色组织机构
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
            return this;
        }

        ///<summary>
        /// 角色组织机构
        ///</summary>
        [FieldDescription("角色组织机构")]
        public const string CurrentTableName = "BaseRoleOrganization";

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
    }
}
