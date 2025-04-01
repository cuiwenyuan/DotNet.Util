//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
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
        public const string FieldName = "Name";

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

        /// <summary>
        /// 创建Ip
        /// </summary>
        public const string FieldCreateIp = "CreateIp";
        /// <summary>
        /// 修改Ip
        /// </summary>
        public const string FieldUpdateIp = "UpdateIp";

        #region IsAudited
        /// <summary>
        /// 是否已审核 
        /// </summary>
        public const string FieldIsAudited = "IsAudited";

        #region 老版本

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

        #endregion

        #region 新版本

        /// <summary>
        /// 审核时间
        /// </summary>
        public const string FieldAuditTime = "AuditTime";

        /// <summary>
        /// 审核人用户主键
        /// </summary>
        public const string FieldAuditUserId = "AuditUserId";

        /// <summary>
        /// 审核人用户名
        /// </summary>
        public const string FieldAuditUserName = "AuditUserName";

        /// <summary>
        /// 审核人
        /// </summary>
        public const string FieldAuditBy = "AuditBy";

        #endregion

        #endregion

        #region IsCancelled
        /// <summary>
        /// 是否已取消
        /// </summary>
        public const string FieldIsCancelled = "IsCancelled";

        #region 老版本

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

        #region 新版本

        /// <summary>
        /// 取消时间
        /// </summary>
        public const string FieldCancelTime = "CancelTime";

        /// <summary>
        /// 取消人用户主键
        /// </summary>
        public const string FieldCancelUserId = "CancelUserId";

        /// <summary>
        /// 取消人用户名
        /// </summary>
        public const string FieldCancelUserName = "CancelUserName";

        /// <summary>
        /// 取消人
        /// </summary>
        public const string FieldCancelBy = "CancelBy";

        #endregion

        #endregion

        #region IsApproved
        /// <summary>
        /// 是否已批准
        /// </summary>
        public const string FieldIsApproved = "IsApproved";

        /// <summary>
        /// 批准时间
        /// </summary>
        public const string FieldApprovedTime = "ApprovedTime";

        #endregion

        #region IsRejected

        /// <summary>
        /// 是否已拒绝
        /// </summary>
        public const string FieldIsRejected = "IsRejected";

        /// <summary>
        /// 拒绝时间
        /// </summary>
        public const string FieldRejectedTime = "RejectedTime";

        #endregion

        #region IsClosed
        /// <summary>
        /// 是否已关闭
        /// </summary>
        public const string FieldIsClosed = "IsClosed";

        /// <summary>
        /// 关闭时间
        /// </summary>
        public const string FieldClosedTime = "ClosedTime";

        #endregion

        #region IsConfirmed
        /// <summary>
        /// 是否已确认
        /// </summary>
        public const string FieldIsConfirmed = "IsConfirmed";

        /// <summary>
        /// 确认时间
        /// </summary>
        public const string FieldConfirmedTime = "ConfirmedTime";
        #endregion

        #region IsReleased
        /// <summary>
        /// 是否已下达
        /// </summary>
        public const string FieldIsReleased = "IsReleased";

        /// <summary>
        /// 下达时间
        /// </summary>
        public const string FieldReleasedTime = "ReleasedTime";
        #endregion

        #region IsCompleted
        /// <summary>
        /// 是否已完成
        /// </summary>
        public const string FieldIsCompleted = "IsCompleted";

        /// <summary>
        /// 完成时间
        /// </summary>
        public const string FieldCompletedTime = "CompletedTime";
        #endregion

        #region IsFinished
        /// <summary>
        /// 是否已完成
        /// </summary>
        public const string FieldIsFinished = "IsFinished";

        /// <summary>
        /// 完成时间
        /// </summary>
        public const string FieldFinishedTime = "FinishedTime";
        #endregion

        #region IsAssigned
        /// <summary>
        /// 是否已分配
        /// </summary>
        public const string FieldIsAssigned = "IsAssigned";

        /// <summary>
        /// 分配时间
        /// </summary>
        public const string FieldAssignedTime = "AssignedTime";

        #endregion

        #region IsScraped
        /// <summary>
        /// 是否已作废
        /// </summary>
        public const string FieldIsScraped = "IsScraped";

        /// <summary>
        /// 作废时间
        /// </summary>
        public const string FieldScrapedTime = "ScrapedTime";

        #endregion

        /// <summary>
        /// 允许删除
        /// </summary>
        public const string FieldAllowDelete = "AllowDelete";

        /// <summary>
        /// 用户公司主键
        /// </summary>
        public const string FieldUserCompanyId = "UserCompanyId";

        /// <summary>
        /// 用户子公司主键
        /// </summary>
        public const string FieldUserSubCompanyId = "UserSubCompanyId";

        /// <summary>
        /// 用户部门主键
        /// </summary>
        public const string FieldUserDepartmentId = "UserDepartmentId";

        /// <summary>
        /// 用户子部门主键
        /// </summary>
        public const string FieldUserSubDepartmentId = "UserSubDepartmentId";

        /// <summary>
        /// 用户工作组主键
        /// </summary>
        public const string FieldUserWorkgroupId = "UserWorkgroupId";

        #endregion
    }
}