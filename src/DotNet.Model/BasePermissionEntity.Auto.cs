//-----------------------------------------------------------------------
// <copyright file="BasePermissionEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2025, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BasePermissionEntity
    /// 权限
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
    public partial class BasePermissionEntity : BaseEntity
    {
        /// <summary>
        /// 子系统编码
        /// </summary>
        [FieldDescription("子系统编码")]
        [Description("子系统编码")]
        [Column(FieldSystemCode)]
        public string SystemCode { get; set; } = "Base";

        /// <summary>
        /// 资料类别
        /// </summary>
        [FieldDescription("资料类别")]
        [Description("资料类别")]
        [Column(FieldResourceCategory)]
        public string ResourceCategory { get; set; } = string.Empty;

        /// <summary>
        /// 资源主键
        /// </summary>
        [FieldDescription("资源主键")]
        [Description("资源主键")]
        [Column(FieldResourceId)]
        public string ResourceId { get; set; } = string.Empty;

        /// <summary>
        /// 权限（菜单模块）主键
        /// </summary>
        [FieldDescription("权限（菜单模块）主键")]
        [Description("权限（菜单模块）主键")]
        [Column(FieldPermissionId)]
        public string PermissionId { get; set; } = string.Empty;

        /// <summary>
        /// 公司主键
        /// </summary>
        [FieldDescription("公司主键")]
        [Description("公司主键")]
        [Column(FieldCompanyId)]
        public int CompanyId { get; set; } = 0;

        /// <summary>
        /// 公司名称
        /// </summary>
        [FieldDescription("公司名称")]
        [Description("公司名称")]
        [Column(FieldCompanyName)]
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// 权限条件限制
        /// </summary>
        [FieldDescription("权限条件限制")]
        [Description("权限条件限制")]
        [Column(FieldPermissionConstraint)]
        public string PermissionConstraint { get; set; } = string.Empty;

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
                PermissionId = BaseUtil.ConvertToString(dr[FieldPermissionId]);
            }
            if (dr.ContainsColumn(FieldCompanyId))
            {
                CompanyId = BaseUtil.ConvertToInt(dr[FieldCompanyId]);
            }
            if (dr.ContainsColumn(FieldCompanyName))
            {
                CompanyName = BaseUtil.ConvertToString(dr[FieldCompanyName]);
            }
            if (dr.ContainsColumn(FieldPermissionConstraint))
            {
                PermissionConstraint = BaseUtil.ConvertToString(dr[FieldPermissionConstraint]);
            }
            if (dr.ContainsColumn(FieldDescription))
            {
                Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            }
            return this;
        }

        ///<summary>
        /// 权限
        ///</summary>
        [FieldDescription("权限")]
        public const string CurrentTableName = "BasePermission";

        ///<summary>
        /// 表名
        ///</summary>
        public const string CurrentTableDescription = "权限";

        ///<summary>
        /// 子系统编码
        ///</summary>
        public const string FieldSystemCode = "SystemCode";

        ///<summary>
        /// 资料类别
        ///</summary>
        public const string FieldResourceCategory = "ResourceCategory";

        ///<summary>
        /// 资源主键
        ///</summary>
        public const string FieldResourceId = "ResourceId";

        ///<summary>
        /// 权限（菜单模块）主键
        ///</summary>
        public const string FieldPermissionId = "PermissionId";

        ///<summary>
        /// 公司主键
        ///</summary>
        public const string FieldCompanyId = "CompanyId";

        ///<summary>
        /// 公司名称
        ///</summary>
        public const string FieldCompanyName = "CompanyName";

        ///<summary>
        /// 权限条件限制
        ///</summary>
        public const string FieldPermissionConstraint = "PermissionConstraint";

        ///<summary>
        /// 描述
        ///</summary>
        public const string FieldDescription = "Description";
    }
}
