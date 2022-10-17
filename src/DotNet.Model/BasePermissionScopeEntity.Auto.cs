//-----------------------------------------------------------------------
// <copyright file="BasePermissionScopeEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2022, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BasePermissionScopeEntity
    /// 数据权限
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
    public partial class BasePermissionScopeEntity : BaseEntity
    {
        /// <summary>
        /// 子系统编码
        /// </summary>
        [FieldDescription("子系统编码")]
        public string SystemCode { get; set; } = "Base";

        /// <summary>
        /// 什么类型的
        /// </summary>
        [FieldDescription("什么类型的")]
        public string ResourceCategory { get; set; } = string.Empty;

        /// <summary>
        /// 什么资源主键
        /// </summary>
        [FieldDescription("什么资源主键")]
        public int ResourceId { get; set; }

        /// <summary>
        /// 对什么类型的
        /// </summary>
        [FieldDescription("对什么类型的")]
        public string TargetCategory { get; set; } = string.Empty;

        /// <summary>
        /// 对什么资源主键
        /// </summary>
        [FieldDescription("对什么资源主键")]
        public int TargetId { get; set; }

        /// <summary>
        /// 有什么权限（模块菜单）主键
        /// </summary>
        [FieldDescription("有什么权限（模块菜单）主键")]
        public int PermissionId { get; set; }

        /// <summary>
        /// 包含子节点
        /// </summary>
        [FieldDescription("包含子节点")]
        public int ContainChild { get; set; } = 0;

        /// <summary>
        /// 有什么权限约束表达式
        /// </summary>
        [FieldDescription("有什么权限约束表达式")]
        public string PermissionConstraint { get; set; } = string.Empty;

        /// <summary>
        /// 开始生效时间
        /// </summary>
        [FieldDescription("开始生效时间")]
        public DateTime? StartTime { get; set; } = null;

        /// <summary>
        /// 结束生效时间
        /// </summary>
        [FieldDescription("结束生效时间")]
        public DateTime? EndTime { get; set; } = null;

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
            if (dr.ContainsColumn(FieldResourceCategory))
            {
                ResourceCategory = BaseUtil.ConvertToString(dr[FieldResourceCategory]);
            }
            if (dr.ContainsColumn(FieldResourceId))
            {
                ResourceId = BaseUtil.ConvertToInt(dr[FieldResourceId]);
            }
            if (dr.ContainsColumn(FieldTargetCategory))
            {
                TargetCategory = BaseUtil.ConvertToString(dr[FieldTargetCategory]);
            }
            if (dr.ContainsColumn(FieldTargetId))
            {
                TargetId = BaseUtil.ConvertToInt(dr[FieldTargetId]);
            }
            if (dr.ContainsColumn(FieldPermissionId))
            {
                PermissionId = BaseUtil.ConvertToInt(dr[FieldPermissionId]);
            }
            if (dr.ContainsColumn(FieldContainChild))
            {
                ContainChild = BaseUtil.ConvertToInt(dr[FieldContainChild]);
            }
            if (dr.ContainsColumn(FieldPermissionConstraint))
            {
                PermissionConstraint = BaseUtil.ConvertToString(dr[FieldPermissionConstraint]);
            }
            if (dr.ContainsColumn(FieldStartTime))
            {
                StartTime = BaseUtil.ConvertToNullableDateTime(dr[FieldStartTime]);
            }
            if (dr.ContainsColumn(FieldEndTime))
            {
                EndTime = BaseUtil.ConvertToNullableDateTime(dr[FieldEndTime]);
            }
            if (dr.ContainsColumn(FieldDescription))
            {
                Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            }
            return this;
        }

        ///<summary>
        /// 数据权限
        ///</summary>
        [FieldDescription("数据权限")]
        public const string CurrentTableName = "BasePermissionScope";

        ///<summary>
        /// 子系统编码
        ///</summary>
        public const string FieldSystemCode = "SystemCode";

        ///<summary>
        /// 什么类型的
        ///</summary>
        public const string FieldResourceCategory = "ResourceCategory";

        ///<summary>
        /// 什么资源主键
        ///</summary>
        public const string FieldResourceId = "ResourceId";

        ///<summary>
        /// 对什么类型的
        ///</summary>
        public const string FieldTargetCategory = "TargetCategory";

        ///<summary>
        /// 对什么资源主键
        ///</summary>
        public const string FieldTargetId = "TargetId";

        ///<summary>
        /// 有什么权限（模块菜单）主键
        ///</summary>
        public const string FieldPermissionId = "PermissionId";

        ///<summary>
        /// 包含子节点
        ///</summary>
        public const string FieldContainChild = "ContainChild";

        ///<summary>
        /// 有什么权限约束表达式
        ///</summary>
        public const string FieldPermissionConstraint = "PermissionConstraint";

        ///<summary>
        /// 开始生效时间
        ///</summary>
        public const string FieldStartTime = "StartTime";

        ///<summary>
        /// 结束生效时间
        ///</summary>
        public const string FieldEndTime = "EndTime";

        ///<summary>
        /// 描述
        ///</summary>
        public const string FieldDescription = "Description";
    }
}
