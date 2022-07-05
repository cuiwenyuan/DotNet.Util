//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///		2014.03.21 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2014.03.21</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        #region public DataTable GetDataTable() 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns>数据表</returns>
        public DataTable GetDataTable()
        {
            var sql = "SELECT " + BaseUserEntity.CurrentTableName + ".* "
                            + " FROM " + BaseUserEntity.CurrentTableName
                            + " WHERE " + BaseUserEntity.FieldDeleted + "= 0 "
                            + " AND " + BaseUserEntity.FieldIsVisible + "= 1 "
                //+ "       AND " + BaseUserEntity.FieldEnabled + "= 1 "  //如果Enabled = 1，只显示有效用户
                            + " ORDER BY " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldSortCode;
            return DbHelper.Fill(sql);
        }
        #endregion

        private string GetSearchConditional(string systemCode, string permissionCode, string condition, string[] roleIds, bool? enabled, string auditStates, string companyId = null, string departmentId = null, bool onlyOnline = false)
        {
            var sb = BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDeleted + " = 0 "
                            + " AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldIsVisible + " = 1 ";

            if (enabled.HasValue)
            {
                if (enabled == true)
                {
                    sb += " AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldEnabled + " = 1 ";
                }
                else
                {
                    sb += " AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldEnabled + " = 0 ";
                }
            }
            if (onlyOnline)
            {
                sb += " AND " + BaseUserEntity.CurrentTableName + ".Id IN (SELECT Id FROM " + BaseUserLogonEntity.CurrentTableName + " WHERE UserOnline = 1) ";
            }
            if (!string.IsNullOrEmpty(condition))
            {
                // 传递过来的表达式，还是搜索值？
                if (condition.IndexOf("AND", StringComparison.OrdinalIgnoreCase) < 0 && condition.IndexOf("=", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    condition = StringUtil.GetSearchString(condition);
                    sb += " AND ("
                                + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldUserName + " LIKE '%" + condition + "%'"
                                + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldSimpleSpelling + " LIKE '%" + condition + "%'"
                                + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldCode + " LIKE '%" + condition + "%'"
                                + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldRealName + " LIKE '%" + condition + "%'"
                                + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldQuickQuery + " LIKE '%" + condition + "%'"
                                + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldCompanyName + " LIKE '%" + condition + "%'"
                                + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDepartmentName + " LIKE '%" + condition + "%'"
                        // + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDescription + " LIKE '%" + search + "%'"
                                + ")";
                }
                else
                {
                    sb += " AND (" + condition + ")";
                }
            }
            if (!string.IsNullOrEmpty(departmentId))
            {
                /*
                BaseOrganizationManager organizationManager = new BaseOrganizationManager(this.DbHelper, this.UserInfo);
                string[] ids = organizationManager.GetChildrensId(BaseOrganizationEntity.FieldId, departmentId, BaseOrganizationEntity.FieldParentId);
                if (ids != null && ids.Length > 0)
                {
                    condition += " AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldCompanyId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDepartmentId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldWorkgroupId + " IN (" + StringUtil.ArrayToList(ids) + "))";
                }
                */
                sb += " AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDepartmentId + " = '" + departmentId + "')";
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                sb += " AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldCompanyId + " = '" + companyId + "')";
            }
            if (!string.IsNullOrEmpty(auditStates))
            {
                sb += " AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldAuditStatus + " = '" + auditStates + "'";
                // 待审核
                if (auditStates.Equals(AuditStatus.WaitForAudit.ToString()))
                {
                    sb += " OR " + BaseUserEntity.CurrentTableName + ".Id IN ( SELECT Id FROM " + BaseUserLogonEntity.CurrentTableName + " WHERE LockEndTime > " + dbHelper.GetDbNow() + ") ";
                }
                sb += ")";
            }
            if (enabled != null)
            {
                sb += " AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldEnabled + " = " + ((bool)enabled ? 1 : 0) + ")";
            }
            if ((roleIds != null) && (roleIds.Length > 0))
            {
                var roles = StringUtil.ArrayToList(roleIds, "'");
                sb += " AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldId + " IN (" + "SELECT " + BaseUserRoleEntity.FieldUserId + " FROM " + BaseUserRoleEntity.CurrentTableName + " WHERE " + BaseUserRoleEntity.FieldRoleId + " IN (" + roles + ")" + "))";
            }

            // 是否过滤用户， 获得组织机构列表， 这里需要一个按用户过滤得功能
            if (!string.IsNullOrEmpty(permissionCode)
                && (!IsAdministrator(UserInfo.Id.ToString())) 
                && (BaseSystemInfo.UsePermissionScope))
            {
                // string permissionCode = "Resource.ManagePermission";
                var permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);
                if (!string.IsNullOrEmpty(permissionId))
                {
                    // 从小到大的顺序进行显示，防止错误发生
                    var userPermissionScopeManager = new BaseUserScopeManager(DbHelper, UserInfo);
                    var organizationIds = userPermissionScopeManager.GetOrganizationIds(UserInfo.SystemCode, UserInfo.Id.ToString(), permissionId);

                    // 没有任何数据权限
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.NotAllowed).ToString()))
                    {
                        sb += " AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldId + " = NULL ) ";
                    }
                    // 按详细设定的数据
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.ByDetails).ToString()))
                    {
                        var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
                        var userIds = permissionScopeManager.GetUserIds(UserInfo.SystemCode, UserInfo.Id.ToString(), permissionCode);
                        sb += " AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldId + " IN (" + StringUtil.ArrayToList(userIds) + ")) ";
                    }
                    // 自己的数据，仅本人
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.OnlyOwnData).ToString()))
                    {
                        sb += " AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldId + " = " + UserInfo.Id + ") ";
                    }
                    // 用户所在工作组数据
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.UserWorkgroup).ToString()))
                    {
                        // condition += " AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldWorkgroupId + " = " + this.UserInfo.WorkgroupId + ") ";
                    }
                    // 用户所在部门数据
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.UserDepartment).ToString()))
                    {
                        sb += " AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDepartmentId + " = " + UserInfo.DepartmentId + ") ";
                    }
                    // 用户所在分支机构数据
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.UserSubCompany).ToString()))
                    {
                        // condition += " AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldSubCompanyId + " = '" + this.UserInfo.SubCompanyId + "') ";
                    }
                    // 用户所在公司数据
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.UserCompany).ToString()))
                    {
                        sb += " AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldCompanyId + " = '" + UserInfo.CompanyId + "') ";
                    }
                    // 全部数据，这里就不用设置过滤条件了
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.AllData).ToString()))
                    {
                    }
                }
            }

            return sb;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="search"></param>
        /// <param name="roleIds"></param>
        /// <param name="enabled"></param>
        /// <param name="auditStates"></param>
        /// <param name="departmentId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public DataTable Search(string systemCode, string permissionCode, string search, string[] roleIds, bool? enabled, string auditStates, string departmentId, string companyId = null)
        {
            var sql = "SELECT " + BaseUserEntity.CurrentTableName + ".* ";
            if (ShowUserLogonInfo)
            {
                sql += "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldFirstVisitTime
                + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldPreviousVisitTime
                + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldLastVisitTime
                + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldIpAddress
                + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldMacAddress
                + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldLogonCount
                + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldUserOnline
                + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldCheckIpAddress
                + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldConcurrentUser
                + " FROM " + BaseUserEntity.CurrentTableName + " LEFT OUTER JOIN " + BaseUserLogonEntity.CurrentTableName + " ON " + BaseUserEntity.CurrentTableName + ".Id = " + BaseUserLogonEntity.CurrentTableName + ".Id ";
            }
            else
            {
                sql += "," + BaseUserContactEntity.CurrentTableName + "." + BaseUserContactEntity.FieldEmail
                + "," + BaseUserContactEntity.CurrentTableName + "." + BaseUserContactEntity.FieldMobile
                + "," + BaseUserContactEntity.CurrentTableName + "." + BaseUserContactEntity.FieldQq
                + "," + BaseUserContactEntity.CurrentTableName + "." + BaseUserContactEntity.FieldTelephone
                + " FROM " + BaseUserEntity.CurrentTableName + " LEFT OUTER JOIN BaseUserContact ON " + BaseUserEntity.CurrentTableName + ".Id = BaseUserContact.Id ";
            }
            var condition = GetSearchConditional(systemCode, permissionCode, search, roleIds, enabled, auditStates, companyId, departmentId);
            sql += " WHERE " + condition;
            sql += " ORDER BY " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldSortCode;
            return DbHelper.Fill(sql);
        }

        /// <summary>
        /// 分页查询日志
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="condition"></param>
        /// <param name="roleIds"></param>
        /// <param name="enabled"></param>
        /// <param name="auditStates"></param>
        /// <param name="companyId"></param>
        /// <param name="departmentId"></param>
        /// <param name="showRole"></param>
        /// <param name="userAllInformation"></param>
        /// <param name="onlyOnline"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public DataTable SearchByPage(string systemCode, string permissionCode, string condition, string[] roleIds, bool? enabled, string auditStates, string companyId, string departmentId, bool showRole, bool userAllInformation, bool onlyOnline, out int recordCount, int pageNo = 1, int pageSize = 20, string order = null)
        {
            condition = GetSearchConditional(systemCode, permissionCode, condition, roleIds, enabled, auditStates, companyId, departmentId, onlyOnline);

            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.SqlServer:
                case CurrentDbType.Access:
                    CurrentTableName = BaseUserEntity.CurrentTableName + " LEFT OUTER JOIN BaseUserContact ON " + BaseUserEntity.CurrentTableName + ".Id = BaseUserContact.Id";
                    if (ShowUserLogonInfo)
                    {
                        CurrentTableName += " LEFT OUTER JOIN " + BaseUserLogonEntity.CurrentTableName + " ON " + BaseUserEntity.CurrentTableName + ".Id = " + BaseUserLogonEntity.CurrentTableName + ".Id";
                    }
                    SelectFields = BaseUserEntity.CurrentTableName + ".* "
                                                + "," + BaseUserContactEntity.CurrentTableName + "." + BaseUserContactEntity.FieldEmail
                                                + "," + BaseUserContactEntity.CurrentTableName + "." + BaseUserContactEntity.FieldMobile
                                                + "," + BaseUserContactEntity.CurrentTableName + "." + BaseUserContactEntity.FieldTelephone
                                                + "," + BaseUserContactEntity.CurrentTableName + "." + BaseUserContactEntity.FieldQq;

                    if (ShowUserLogonInfo)
                    {
                        SelectFields += "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldFirstVisitTime
                                                + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldPreviousVisitTime
                                                + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldLastVisitTime
                                                + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldIpAddress
                                                + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldMacAddress
                                                + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldLogonCount
                                                + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldUserOnline
                                                + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldCheckIpAddress
                                                + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldConcurrentUser;
                    }
                    break;
                case CurrentDbType.Oracle:
                    SelectFields = "*";
                    CurrentTableName = @"(SELECT " + BaseUserEntity.CurrentTableName + ".*";
                    if (ShowUserLogonInfo)
                    {
                        CurrentTableName += " ," + BaseUserLogonEntity.CurrentTableName + ".FirstVisit, " + BaseUserLogonEntity.CurrentTableName + ".PreviousVisit, " + BaseUserLogonEntity.CurrentTableName + ".LastVisit, " + BaseUserLogonEntity.CurrentTableName + ".IPAddress, " + BaseUserLogonEntity.CurrentTableName + ".MACAddress, " + BaseUserLogonEntity.CurrentTableName + ".LogonCount, " + BaseUserLogonEntity.CurrentTableName + ".UserOnline, " + BaseUserLogonEntity.CurrentTableName + ".CheckIPAddress, " + BaseUserLogonEntity.CurrentTableName + ".ConcurrentUser ";
                    }
                    CurrentTableName += @"       , BaseUserContact.Email
                                          , BaseUserContact.Mobile
                                          , BaseUserContact.Telephone
                                          , BaseUserContact.QQ
    FROM " + BaseUserEntity.CurrentTableName + " LEFT OUTER JOIN BaseUserContact ON " + BaseUserEntity.CurrentTableName + ".Id = BaseUserContact.Id ";

                    if (ShowUserLogonInfo)
                    {
                        CurrentTableName += " LEFT OUTER JOIN " + BaseUserLogonEntity.CurrentTableName + " ON " + BaseUserEntity.CurrentTableName + ".Id = " + BaseUserLogonEntity.CurrentTableName + ".Id ";
                    }
                    if (enabled == null)
                    {
                        CurrentTableName += " WHERE " + BaseUserEntity.CurrentTableName + ".Id > 0 AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDeleted + " = 0  AND " + BaseUserEntity.CurrentTableName + ".IsVisible = 1 ORDER BY " + BaseUserEntity.CurrentTableName + ".SortCode) " + BaseUserEntity.CurrentTableName;
                    }
                    else
                    {
                        var enabledState = enabled == true ? "1" : "0";
                        CurrentTableName += " WHERE " + BaseUserEntity.CurrentTableName + ".Id > 0 AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDeleted + " = 0  AND " + BaseUserEntity.CurrentTableName + ".IsVisible = 1 AND (" + BaseUserEntity.CurrentTableName + ".Enabled = " + enabledState + ") ORDER BY " + BaseUserEntity.CurrentTableName + ".SortCode) " + BaseUserEntity.CurrentTableName;
                    }
                    break;
            }

            return GetDataTableByPage(out recordCount, pageNo, pageSize, condition, null, order);
        }

        /// <summary>
        /// 分页查询日志
        /// </summary>
        /// <param name="recordCount"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="condition"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public DataTable SearchLogByPage(out int recordCount, int pageNo, int pageSize, string permissionCode, string condition, string order = null)
        {
            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.SqlServer:
                case CurrentDbType.Access:
                    CurrentTableName = BaseUserEntity.CurrentTableName + " LEFT OUTER JOIN " + BaseUserLogonEntity.CurrentTableName + " ON " + BaseUserEntity.CurrentTableName + ".Id = " + BaseUserLogonEntity.CurrentTableName + ".Id";
                    SelectFields = BaseUserEntity.CurrentTableName + ".* ";

                    SelectFields += "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldFirstVisitTime
                                            + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldPreviousVisitTime
                                            + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldLastVisitTime
                                            + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldIpAddress
                                            + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldMacAddress
                                            + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldLogonCount
                                            + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldUserOnline
                                            + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldCheckIpAddress
                                            + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldConcurrentUser;
                    break;
                case CurrentDbType.Oracle:
                    SelectFields = "*";
                    CurrentTableName = @"(SELECT " + BaseUserEntity.CurrentTableName + ".*";
                    CurrentTableName += " ," + BaseUserLogonEntity.CurrentTableName + ".FirstVisit, " + BaseUserLogonEntity.CurrentTableName + ".PreviousVisit, " + BaseUserLogonEntity.CurrentTableName + ".LastVisit, " + BaseUserLogonEntity.CurrentTableName + ".IPAddress, " + BaseUserLogonEntity.CurrentTableName + ".MACAddress, " + BaseUserLogonEntity.CurrentTableName + ".LogonCount, " + BaseUserLogonEntity.CurrentTableName + ".UserOnline, " + BaseUserLogonEntity.CurrentTableName + ".CheckIPAddress, " + BaseUserLogonEntity.CurrentTableName + ".ConcurrentUser ";
                    CurrentTableName += @" FROM " + BaseUserEntity.CurrentTableName + " LEFT OUTER JOIN " + BaseUserLogonEntity.CurrentTableName + " ON " + BaseUserEntity.CurrentTableName + ".Id = " + BaseUserLogonEntity.CurrentTableName + ".Id ";
                    CurrentTableName += " WHERE " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDeleted + " = 0 AND " + BaseUserEntity.CurrentTableName + ".IsVisible = 1 AND (" + BaseUserEntity.CurrentTableName + ".Enabled = 1) ORDER BY " + BaseUserEntity.CurrentTableName + ".SortCode) " + BaseUserEntity.CurrentTableName;
                    break;
            }
            return GetDataTableByPage(out recordCount, pageNo, pageSize, condition, null, order);
        }

        #region public DataTable GetDataTableByPage(string searchKey, string companyId, string departmentId, string roleId, out int recordCount, int pageNo = 1, int pageSize = 20, string order = null) 分页查询
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="searchKey">查询关键字</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="departmentId">部门主键</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string searchKey, string companyId, string departmentId, string roleId, out int recordCount, int pageNo = 1, int pageSize = 20, string order = null)
        {
            var condition = BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDeleted + " = 0 "
                 + " AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldEnabled + " = 1 "
                 + " AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldIsVisible + " = 1 "
                 + " AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldId + " > 0 ";

            if (!string.IsNullOrEmpty(companyId))
            {
                condition += " AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldCompanyId + " = '" + companyId + "')";
            }
            if (!string.IsNullOrEmpty(departmentId))
            {
                /*
                用非递归调用的建议方法
                sql += " AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDepartmentId 
                    + " IN ( SELECT " + BaseOrganizationEntity.FieldId 
                    + " FROM " + BaseOrganizationEntity.CurrentTableName 
                    + " WHERE " + BaseOrganizationEntity.FieldId + " = " + departmentId + " OR " + BaseOrganizationEntity.FieldParentId + " = " + departmentId + ")";
                */

                /*
                BaseOrganizationManager organizationManager = new BaseOrganizationManager(this.UserInfo);
                string[] ids = organizationManager.GetChildrensId(BaseOrganizationEntity.FieldId, departmentId, BaseOrganizationEntity.FieldParentId);
                if (ids != null && ids.Length > 0)
                {
                    condition += " AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldCompanyId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldSubCompanyId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDepartmentId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldWorkgroupId + " IN (" + StringUtil.ArrayToList(ids) + "))";
                }
                */
                condition += " AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDepartmentId + " = " + departmentId + ")";
            }
            if (!string.IsNullOrEmpty(roleId))
            {
                var tableNameUserRole = UserInfo.SystemCode + "UserRole";
                condition += " AND ( " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldId + " IN "
                            + "           (SELECT " + BaseUserRoleEntity.FieldUserId
                            + " FROM " + tableNameUserRole
                            + "             WHERE " + BaseUserRoleEntity.FieldRoleId + " = " + roleId + ""
                            + "               AND " + BaseUserRoleEntity.FieldEnabled + " = 1"
                            + "                AND " + BaseUserRoleEntity.FieldDeleted + " = 0)) ";
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = "'" + StringUtil.GetSearchString(searchKey) + "'";
                condition += " AND (" + BaseUserEntity.FieldRealName + " LIKE " + searchKey;
                condition += " OR " + BaseUserEntity.FieldUserName + " LIKE " + searchKey;
                condition += " OR " + BaseUserEntity.FieldQuickQuery + " LIKE " + searchKey;
                condition += " OR " + BaseUserEntity.FieldSimpleSpelling + " LIKE " + searchKey + ")";
            }
            recordCount = DbHelper.GetCount(CurrentTableName, condition);
            CurrentTableName = "BaseUser";
            if (ShowUserLogonInfo)
            {
                CurrentTableName = BaseUserEntity.CurrentTableName + " LEFT OUTER JOIN " + BaseUserLogonEntity.CurrentTableName + " ON " + BaseUserEntity.CurrentTableName + ".Id = " + BaseUserLogonEntity.CurrentTableName + ".Id ";
            }
            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.SqlServer:
                case CurrentDbType.Access:
                    SelectFields = BaseUserEntity.CurrentTableName + ".* ";
                    if (ShowUserLogonInfo)
                    {
                        SelectFields += "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldFirstVisitTime
                                    + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldPreviousVisitTime
                                    + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldLastVisitTime
                                    + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldIpAddress
                                    + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldMacAddress
                                    + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldLogonCount
                                    + "," + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldUserOnline;
                    }
                    break;
                case CurrentDbType.Oracle:
                case CurrentDbType.MySql:
                case CurrentDbType.Db2:
                    break;
            }
            return DbHelper.GetDataTableByPage(CurrentTableName, SelectFields, pageNo, pageSize, condition, order);
        }
        #endregion
    }
}