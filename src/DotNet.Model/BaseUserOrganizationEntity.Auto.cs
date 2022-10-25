//-----------------------------------------------------------------------
// <copyright file="BaseUserOrganizationEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2022, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseUserOrganizationEntity
    /// 用户兼任
    /// 
    /// 修改记录
    /// 
    /// 2022-10-23 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2022-10-23</date>
    /// </author>
    /// </summary>
    [Table(CurrentTableName)]
    public partial class BaseUserOrganizationEntity : BaseEntity
    {
        /// <summary>
        /// 用户账户主键
        /// </summary>
        [FieldDescription("用户账户主键")]
        [Description("用户账户主键")]
        [Column(FieldUserId)]
        public int UserId { get; set; } = 0;

        /// <summary>
        /// 公司主键
        /// </summary>
        [FieldDescription("公司主键")]
        [Description("公司主键")]
        [Column(FieldCompanyId)]
        public int CompanyId { get; set; } = 0;

        /// <summary>
        /// 分支机构主键
        /// </summary>
        [FieldDescription("分支机构主键")]
        [Description("分支机构主键")]
        [Column(FieldSubCompanyId)]
        public int SubCompanyId { get; set; } = 0;

        /// <summary>
        /// 部门主键
        /// </summary>
        [FieldDescription("部门主键")]
        [Description("部门主键")]
        [Column(FieldDepartmentId)]
        public int DepartmentId { get; set; } = 0;

        /// <summary>
        /// 子部门主键
        /// </summary>
        [FieldDescription("子部门主键")]
        [Description("子部门主键")]
        [Column(FieldSubDepartmentId)]
        public int SubDepartmentId { get; set; } = 0;

        /// <summary>
        /// 工作组主键
        /// </summary>
        [FieldDescription("工作组主键")]
        [Description("工作组主键")]
        [Column(FieldWorkgroupId)]
        public int WorkgroupId { get; set; } = 0;

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
            if (dr.ContainsColumn(FieldUserId))
            {
                UserId = BaseUtil.ConvertToInt(dr[FieldUserId]);
            }
            if (dr.ContainsColumn(FieldCompanyId))
            {
                CompanyId = BaseUtil.ConvertToInt(dr[FieldCompanyId]);
            }
            if (dr.ContainsColumn(FieldSubCompanyId))
            {
                SubCompanyId = BaseUtil.ConvertToInt(dr[FieldSubCompanyId]);
            }
            if (dr.ContainsColumn(FieldDepartmentId))
            {
                DepartmentId = BaseUtil.ConvertToInt(dr[FieldDepartmentId]);
            }
            if (dr.ContainsColumn(FieldSubDepartmentId))
            {
                SubDepartmentId = BaseUtil.ConvertToInt(dr[FieldSubDepartmentId]);
            }
            if (dr.ContainsColumn(FieldWorkgroupId))
            {
                WorkgroupId = BaseUtil.ConvertToInt(dr[FieldWorkgroupId]);
            }
            if (dr.ContainsColumn(FieldDescription))
            {
                Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            }
            return this;
        }

        ///<summary>
        /// 用户兼任
        ///</summary>
        [FieldDescription("用户兼任")]
        public const string CurrentTableName = "BaseUserOrganization";

        ///<summary>
        /// 用户账户主键
        ///</summary>
        public const string FieldUserId = "UserId";

        ///<summary>
        /// 公司主键
        ///</summary>
        public const string FieldCompanyId = "CompanyId";

        ///<summary>
        /// 分支机构主键
        ///</summary>
        public const string FieldSubCompanyId = "SubCompanyId";

        ///<summary>
        /// 部门主键
        ///</summary>
        public const string FieldDepartmentId = "DepartmentId";

        ///<summary>
        /// 子部门主键
        ///</summary>
        public const string FieldSubDepartmentId = "SubDepartmentId";

        ///<summary>
        /// 工作组主键
        ///</summary>
        public const string FieldWorkgroupId = "WorkgroupId";

        ///<summary>
        /// 描述
        ///</summary>
        public const string FieldDescription = "Description";
    }
}
