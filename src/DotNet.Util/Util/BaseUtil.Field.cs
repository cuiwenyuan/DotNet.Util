//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Util
{
    /// <summary>
    ///	BaseUtil
    /// 通用基类
    /// 
    /// 这个类可是修改了很多次啊，已经比较经典了，随着专业的提升，人也会不断提高，技术也会越来越精湛。
    /// 
    /// 修改记录
    ///
    ///     2015.07.06 版本：Troy.Cui进行扩展。
    ///		2012.04.05 版本：1.0	JiRiGaLa 改进 GetPermissionScope(string[] organizationIds)。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.04.05</date>
    /// </author> 
    /// </summary>
    public partial class BaseUtil
    {
        #region Field 静态字段
        /// <summary>
        /// 主键字段
        /// </summary>
        public const string FieldId = "Id";

        /// <summary>
        /// 上级字段
        /// </summary>
        public const string FieldParentId = "ParentId";

        /// <summary>
        /// 编号字段
        /// </summary>
        public const string FieldCode = "Code";

        /// <summary>
        /// 名称字段
        /// </summary>
        public const string FieldFullName = "FullName";

        /// <summary>
        /// 类别字段
        /// </summary>
        public const string FieldCategoryCode = "CategoryCode";

        /// <summary>
        /// 有效字段
        /// </summary>
        public const string FieldEnabled = "Enabled";

        /// <summary>
        /// 用户主键
        /// </summary>
        public const string FieldUserId = "UserId";

        /// <summary>
        /// 部门主键
        /// </summary>
        public const string FieldDepartmentId = "DepartmentId";

        /// <summary>
        /// 公司主键
        /// </summary>
        public const string FieldCompanyId = "CompanyId";

        /// <summary>
        /// 是否删除
        /// </summary>
        [Obsolete("V5版本请使用FieldDeleted")]
        public const string FieldDeletionStateCode = "DeletionStateCode";

        /// <summary>
        /// 是否删除
        /// </summary>
        public const string FieldDeleted = "Deleted";

        /// <summary>
        /// 排序码
        /// </summary>
        public const string FieldSortCode = "SortCode";

        /// <summary>
        /// 创建人编号
        /// </summary>
        public const string FieldCreateUserId = "CreateUserId";

        /// <summary>
        /// 创建人用户名
        /// </summary>
        public const string FieldCreateUserName = "CreateUserName";

        /// <summary>
        /// 创建人
        /// </summary>
        public const string FieldCreateBy = "CreateBy";

        /// <summary>
        /// 创建时间
        /// </summary>
        public const string FieldCreateTime = "CreateTime";

        /// <summary>
        /// 创建时间
        /// </summary>
        [Obsolete("V5版本请使用FieldCreateTime")]
        public const string FieldCreateOn = "CreateOn";

        /// <summary>
        /// 最后修改者主键
        /// </summary>
        [Obsolete("V5版本请使用FieldUpdateUserId")]
        public const string FieldModifiedUserId = "ModifiedUserId";

        /// <summary>
        /// 最后修改者用户名
        /// </summary>
        [Obsolete("V5版本请使用FieldUpdateUserName")]
        public const string FieldModifiedUserName = "ModifiedUserName";

        /// <summary>
        /// 最后修改者
        /// </summary>
        [Obsolete("V5版本请使用FieldUpdateBy")]
        public const string FieldModifiedBy = "ModifiedBy";

        /// <summary>
        /// 修改时间
        /// </summary>
        [Obsolete("V5版本请使用FieldUpdateTime")]
        public const string FieldModifiedOn = "ModifiedOn";

        /// <summary>
        /// 最后修改者主键
        /// </summary>
        public const string FieldUpdateUserId = "UpdateUserId";

        /// <summary>
        /// 修改人用户名
        /// </summary>
        public const string FieldUpdateUserName = "UpdateUserName";

        /// <summary>
        /// 修改人
        /// </summary>
        public const string FieldUpdateBy = "UpdateBy";

        /// <summary>
        /// 修改时间
        /// </summary>
        public const string FieldUpdateTime = "UpdateTime";

        /// <summary>
        /// AND查询逻辑
        /// </summary>
        public static string SqlLogicConditional = " AND ";

        /// <summary>
        /// 选择列 Selected 
        /// </summary>
        public static string SelectedColumn = "Selected";
        #endregion

        #region Field 静态字段扩展

        /// <summary>
        /// 创建Ip
        /// </summary>
        public const string FieldCreateIp = "CreateIp";
        /// <summary>
        /// 修改Ip
        /// </summary>
        public const string FieldUpdateIp = "UpdateIp";

        /// <summary>
        /// 修改Ip
        /// </summary>
        [Obsolete("V5版本请使用FieldUpdateIp")]
        public const string FieldModifiedIp = "ModifiedIp";
        /// <summary>
        /// 是否已审核 
        /// </summary>
        public const string FieldIsAudited = "IsAudited";

        /// <summary>
        /// 审核时间
        /// </summary>
        public const string FieldAuditedDate = "AuditedDate";

        /// <summary>
        /// 审核人用户主键
        /// </summary>
        public const string FieldAuditedUserId = "AuditedUserId";

        /// <summary>
        /// 审核人用户名
        /// </summary>
        public const string FieldAuditedUserName = "AuditedUserName";

        /// <summary>
        /// 是否已取消
        /// </summary>
        public const string FieldIsCancelled = "IsCancelled";

        /// <summary>
        /// 取消时间
        /// </summary>
        public const string FieldCancelledDate = "CancelledDate";

        /// <summary>
        /// 取消人用户主键
        /// </summary>
        public const string FieldCancelledUserId = "CancelledUserId";

        /// <summary>
        /// 取消人用户名
        /// </summary>
        public const string FieldCancelledUserName = "CancelledUserName";

        #endregion
    }
}