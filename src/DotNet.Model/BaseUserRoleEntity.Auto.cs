//-----------------------------------------------------------------------
// <copyright file="BaseUserRoleEntity.Auto.cs" company="DotNet">
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
    /// BaseUserRoleEntity
    /// 用户角色
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
    public partial class BaseUserRoleEntity : BaseEntity
    {
        /// <summary>
        /// 子系统编码
        /// </summary>
        [FieldDescription("子系统编码")]
        [Description("子系统编码")]
        [Column(FieldSystemCode)]
        public string SystemCode { get; set; } = "Base";

        /// <summary>
        /// 用户主键
        /// </summary>
        [FieldDescription("用户主键")]
        [Description("用户主键")]
        [Column(FieldUserId)]
        public int UserId { get; set; }

        /// <summary>
        /// 角色主键
        /// </summary>
        [FieldDescription("角色主键")]
        [Description("角色主键")]
        [Column(FieldRoleId)]
        public int RoleId { get; set; }

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
            if (dr.ContainsColumn(FieldUserId))
            {
                UserId = BaseUtil.ConvertToInt(dr[FieldUserId]);
            }
            if (dr.ContainsColumn(FieldRoleId))
            {
                RoleId = BaseUtil.ConvertToInt(dr[FieldRoleId]);
            }
            if (dr.ContainsColumn(FieldDescription))
            {
                Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            }
            return this;
        }

        ///<summary>
        /// 用户角色
        ///</summary>
        [FieldDescription("用户角色")]
        public const string CurrentTableName = "BaseUserRole";

        ///<summary>
        /// 表名
        ///</summary>
        public const string CurrentTableDescription = "用户角色";

        ///<summary>
        /// 子系统编码
        ///</summary>
        public const string FieldSystemCode = "SystemCode";

        ///<summary>
        /// 用户主键
        ///</summary>
        public const string FieldUserId = "UserId";

        ///<summary>
        /// 角色主键
        ///</summary>
        public const string FieldRoleId = "RoleId";

        ///<summary>
        /// 描述
        ///</summary>
        public const string FieldDescription = "Description";
    }
}
