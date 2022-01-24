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
            var sql = "SELECT " + BaseUserEntity.TableName + ".* "
                            + " FROM " + BaseUserEntity.TableName
                            + " WHERE " + BaseUserEntity.FieldDeleted + "= 0 "
                            + " AND " + BaseUserEntity.FieldIsVisible + "= 1 "
                //+ "       AND " + BaseUserEntity.FieldEnabled + "= 1 "  //如果Enabled = 1，只显示有效用户
                            + " ORDER BY " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldSortCode;
            return DbHelper.Fill(sql);
        }
        #endregion

        private string GetSearchConditional(string systemCode, string permissionCode, string condition, string[] roleIds, bool? enabled, string auditStates, string companyId = null, string departmentId = null, bool onlyOnline = false)
        {
            var sb = BaseUserEntity.TableName + "." + BaseUserEntity.FieldDeleted + " = 0 "
                            + " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldIsVisible + " = 1 ";

            if (enabled.HasValue)
            {
                if (enabled == true)
                {
                    sb += " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldEnabled + " = 1 ";
                }
                else
                {
                    sb += " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldEnabled + " = 0 ";
                }
            }
            if (onlyOnline)
            {
                sb += " AND " + BaseUserEntity.TableName + ".Id IN (SELECT Id FROM " + BaseUserLogonEntity.TableName + " WHERE UserOnline = 1) ";
            }
            if (!string.IsNullOrEmpty(condition))
            {
                // 传递过来的表达式，还是搜索值？
                if (condition.IndexOf("AND", StringComparison.OrdinalIgnoreCase) < 0 && condition.IndexOf("=", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    condition = StringUtil.GetSearchString(condition);
                    sb += " AND ("
                                + BaseUserEntity.TableName + "." + BaseUserEntity.FieldUserName + " LIKE '%" + condition + "%'"
                                + " OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldSimpleSpelling + " LIKE '%" + condition + "%'"
                                + " OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldCode + " LIKE '%" + condition + "%'"
                                + " OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldRealName + " LIKE '%" + condition + "%'"
                                + " OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldQuickQuery + " LIKE '%" + condition + "%'"
                                + " OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldCompanyName + " LIKE '%" + condition + "%'"
                                + " OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDepartmentName + " LIKE '%" + condition + "%'"
                        // + " OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDescription + " LIKE '%" + search + "%'"
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
                BaseOrganizationManager organizeManager = new BaseOrganizationManager(this.DbHelper, this.UserInfo);
                string[] ids = organizeManager.GetChildrensId(BaseOrganizationEntity.FieldId, departmentId, BaseOrganizationEntity.FieldParentId);
                if (ids != null && ids.Length > 0)
                {
                    condition += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldCompanyId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDepartmentId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldWorkgroupId + " IN (" + StringUtil.ArrayToList(ids) + "))";
                }
                */
                sb += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDepartmentId + " = '" + departmentId + "')";
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                sb += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldCompanyId + " = '" + companyId + "')";
            }
            if (!string.IsNullOrEmpty(auditStates))
            {
                sb += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldAuditStatus + " = '" + auditStates + "'";
                // 待审核
                if (auditStates.Equals(AuditStatus.WaitForAudit.ToString()))
                {
                    sb += " OR " + BaseUserEntity.TableName + ".Id IN ( SELECT Id FROM " + BaseUserLogonEntity.TableName + " WHERE LockEndDate > " + dbHelper.GetDbNow() + ") ";
                }
                sb += ")";
            }
            if (enabled != null)
            {
                sb += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldEnabled + " = " + ((bool)enabled ? 1 : 0) + ")";
            }
            if ((roleIds != null) && (roleIds.Length > 0))
            {
                var roles = StringUtil.ArrayToList(roleIds, "'");
                sb += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldId + " IN (" + "SELECT " + BaseUserRoleEntity.FieldUserId + " FROM " + BaseUserRoleEntity.TableName + " WHERE " + BaseUserRoleEntity.FieldRoleId + " IN (" + roles + ")" + "))";
            }

            // 是否过滤用户， 获得组织机构列表， 这里需要一个按用户过滤得功能
            if (!string.IsNullOrEmpty(permissionCode)
                && (!IsAdministrator(UserInfo.Id)) 
                && (BaseSystemInfo.UsePermissionScope))
            {
                // string permissionCode = "Resource.ManagePermission";
                var permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
                if (!string.IsNullOrEmpty(permissionId))
                {
                    // 从小到大的顺序进行显示，防止错误发生
                    var userPermissionScopeManager = new BaseUserScopeManager(DbHelper, UserInfo);
                    var organizationIds = userPermissionScopeManager.GetOrganizationIds(UserInfo.SystemCode, UserInfo.Id, permissionId);

                    // 没有任何数据权限
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.NotAllowed).ToString()))
                    {
                        sb += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldId + " = NULL ) ";
                    }
                    // 按详细设定的数据
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.ByDetails).ToString()))
                    {
                        var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);;
                        var userIds = permissionScopeManager.GetUserIds(UserInfo.SystemCode, UserInfo.Id, permissionCode);
                        sb += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldId + " IN (" + string.Join(",", userIds) + ")) ";
                    }
                    // 自己的数据，仅本人
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.OnlyOwnData).ToString()))
                    {
                        sb += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldId + " = " + UserInfo.Id + ") ";
                    }
                    // 用户所在工作组数据
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.UserWorkgroup).ToString()))
                    {
                        // condition += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldWorkgroupId + " = " + this.UserInfo.WorkgroupId + ") ";
                    }
                    // 用户所在部门数据
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.UserDepartment).ToString()))
                    {
                        sb += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDepartmentId + " = " + UserInfo.DepartmentId + ") ";
                    }
                    // 用户所在分支机构数据
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.UserSubCompany).ToString()))
                    {
                        // condition += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldSubCompanyId + " = '" + this.UserInfo.SubCompanyId + "') ";
                    }
                    // 用户所在公司数据
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.UserCompany).ToString()))
                    {
                        sb += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldCompanyId + " = '" + UserInfo.CompanyId + "') ";
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
            var sql = "SELECT " + BaseUserEntity.TableName + ".* ";
            if (ShowUserLogonInfo)
            {
                sql += "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldFirstVisit
                + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldPreviousVisit
                + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldLastVisit
                + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldIpAddress
                + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldMacAddress
                + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldLogonCount
                + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldUserOnline
                + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldCheckIpAddress
                + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldMultiUserLogin
                + " FROM " + BaseUserEntity.TableName + " LEFT OUTER JOIN " + BaseUserLogonEntity.TableName + " ON " + BaseUserEntity.TableName + ".Id = " + BaseUserLogonEntity.TableName + ".Id ";
            }
            else
            {
                sql += "," + BaseUserContactEntity.TableName + "." + BaseUserContactEntity.FieldEmail
                + "," + BaseUserContactEntity.TableName + "." + BaseUserContactEntity.FieldMobile
                + "," + BaseUserContactEntity.TableName + "." + BaseUserContactEntity.FieldQq
                + "," + BaseUserContactEntity.TableName + "." + BaseUserContactEntity.FieldTelephone
                + " FROM " + BaseUserEntity.TableName + " LEFT OUTER JOIN BaseUserContact ON " + BaseUserEntity.TableName + ".Id = BaseUserContact.Id ";
            }
            var condition = GetSearchConditional(systemCode, permissionCode, search, roleIds, enabled, auditStates, companyId, departmentId);
            sql += " WHERE " + condition;
            sql += " ORDER BY " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldSortCode;
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
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public DataTable SearchByPage(string systemCode, string permissionCode, string condition, string[] roleIds, bool? enabled, string auditStates, string companyId, string departmentId, bool showRole, bool userAllInformation, bool onlyOnline, out int recordCount, int pageIndex = 0, int pageSize = 20, string order = null)
        {
            condition = GetSearchConditional(systemCode, permissionCode, condition, roleIds, enabled, auditStates, companyId, departmentId, onlyOnline);

            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.SqlServer:
                case CurrentDbType.Access:
                    CurrentTableName = BaseUserEntity.TableName + " LEFT OUTER JOIN BaseUserContact ON " + BaseUserEntity.TableName + ".Id = BaseUserContact.Id";
                    if (ShowUserLogonInfo)
                    {
                        CurrentTableName += " LEFT OUTER JOIN " + BaseUserLogonEntity.TableName + " ON " + BaseUserEntity.TableName + ".Id = " + BaseUserLogonEntity.TableName + ".Id";
                    }
                    SelectFields = BaseUserEntity.TableName + ".* "
                                                + "," + BaseUserContactEntity.TableName + "." + BaseUserContactEntity.FieldEmail
                                                + "," + BaseUserContactEntity.TableName + "." + BaseUserContactEntity.FieldMobile
                                                + "," + BaseUserContactEntity.TableName + "." + BaseUserContactEntity.FieldTelephone
                                                + "," + BaseUserContactEntity.TableName + "." + BaseUserContactEntity.FieldQq;

                    if (ShowUserLogonInfo)
                    {
                        SelectFields += "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldFirstVisit
                                                + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldPreviousVisit
                                                + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldLastVisit
                                                + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldIpAddress
                                                + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldMacAddress
                                                + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldLogonCount
                                                + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldUserOnline
                                                + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldCheckIpAddress
                                                + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldMultiUserLogin;
                    }
                    break;
                case CurrentDbType.Oracle:
                    SelectFields = "*";
                    CurrentTableName = @"(SELECT " + BaseUserEntity.TableName + ".*";
                    if (ShowUserLogonInfo)
                    {
                        CurrentTableName += " ," + BaseUserLogonEntity.TableName + ".FirstVisit, " + BaseUserLogonEntity.TableName + ".PreviousVisit, " + BaseUserLogonEntity.TableName + ".LastVisit, " + BaseUserLogonEntity.TableName + ".IPAddress, " + BaseUserLogonEntity.TableName + ".MACAddress, " + BaseUserLogonEntity.TableName + ".LogonCount, " + BaseUserLogonEntity.TableName + ".UserOnline, " + BaseUserLogonEntity.TableName + ".CheckIPAddress, " + BaseUserLogonEntity.TableName + ".MultiUserLogin ";
                    }
                    CurrentTableName += @"       , BaseUserContact.Email
                                          , BaseUserContact.Mobile
                                          , BaseUserContact.Telephone
                                          , BaseUserContact.QQ
    FROM " + BaseUserEntity.TableName + " LEFT OUTER JOIN BaseUserContact ON " + BaseUserEntity.TableName + ".Id = BaseUserContact.Id ";

                    if (ShowUserLogonInfo)
                    {
                        CurrentTableName += " LEFT OUTER JOIN " + BaseUserLogonEntity.TableName + " ON " + BaseUserEntity.TableName + ".Id = " + BaseUserLogonEntity.TableName + ".Id ";
                    }
                    if (enabled == null)
                    {
                        CurrentTableName += " WHERE " + BaseUserEntity.TableName + ".Id > 0 AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDeleted + " = 0  AND " + BaseUserEntity.TableName + ".IsVisible = 1 ORDER BY " + BaseUserEntity.TableName + ".SortCode) " + BaseUserEntity.TableName;
                    }
                    else
                    {
                        var enabledState = enabled == true ? "1" : "0";
                        CurrentTableName += " WHERE " + BaseUserEntity.TableName + ".Id > 0 AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDeleted + " = 0  AND " + BaseUserEntity.TableName + ".IsVisible = 1 AND (" + BaseUserEntity.TableName + ".Enabled = " + enabledState + ") ORDER BY " + BaseUserEntity.TableName + ".SortCode) " + BaseUserEntity.TableName;
                    }
                    break;
            }

            return GetDataTableByPage(out recordCount, pageIndex, pageSize, condition, null, order);
        }

        /// <summary>
        /// 分页查询日志
        /// </summary>
        /// <param name="recordCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="condition"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public DataTable SearchLogByPage(out int recordCount, int pageIndex, int pageSize, string permissionCode, string condition, string order = null)
        {
            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.SqlServer:
                case CurrentDbType.Access:
                    CurrentTableName = BaseUserEntity.TableName + " LEFT OUTER JOIN " + BaseUserLogonEntity.TableName + " ON " + BaseUserEntity.TableName + ".Id = " + BaseUserLogonEntity.TableName + ".Id";
                    SelectFields = BaseUserEntity.TableName + ".* ";

                    SelectFields += "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldFirstVisit
                                            + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldPreviousVisit
                                            + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldLastVisit
                                            + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldIpAddress
                                            + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldMacAddress
                                            + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldLogonCount
                                            + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldUserOnline
                                            + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldCheckIpAddress
                                            + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldMultiUserLogin;
                    break;
                case CurrentDbType.Oracle:
                    SelectFields = "*";
                    CurrentTableName = @"(SELECT " + BaseUserEntity.TableName + ".*";
                    CurrentTableName += " ," + BaseUserLogonEntity.TableName + ".FirstVisit, " + BaseUserLogonEntity.TableName + ".PreviousVisit, " + BaseUserLogonEntity.TableName + ".LastVisit, " + BaseUserLogonEntity.TableName + ".IPAddress, " + BaseUserLogonEntity.TableName + ".MACAddress, " + BaseUserLogonEntity.TableName + ".LogonCount, " + BaseUserLogonEntity.TableName + ".UserOnline, " + BaseUserLogonEntity.TableName + ".CheckIPAddress, " + BaseUserLogonEntity.TableName + ".MultiUserLogin ";
                    CurrentTableName += @" FROM " + BaseUserEntity.TableName + " LEFT OUTER JOIN " + BaseUserLogonEntity.TableName + " ON " + BaseUserEntity.TableName + ".Id = " + BaseUserLogonEntity.TableName + ".Id ";
                    CurrentTableName += " WHERE " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDeleted + " = 0 AND " + BaseUserEntity.TableName + ".IsVisible = 1 AND (" + BaseUserEntity.TableName + ".Enabled = 1) ORDER BY " + BaseUserEntity.TableName + ".SortCode) " + BaseUserEntity.TableName;
                    break;
            }
            return GetDataTableByPage(out recordCount, pageIndex, pageSize, condition, null, order);
        }

        #region public DataTable GetDataTableByPage(string searchKey, string companyId, string departmentId, string roleId, out int recordCount, int pageIndex = 0, int pageSize = 20, string order = null) 分页查询
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="searchKey">查询字段</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="departmentId">部门主键</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string searchKey, string companyId, string departmentId, string roleId, out int recordCount, int pageIndex = 0, int pageSize = 20, string order = null)
        {
            var condition = BaseUserEntity.TableName + "." + BaseUserEntity.FieldDeleted + " = 0 "
                 + " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldEnabled + " = 1 "
                 + " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldIsVisible + " = 1 "
                 + " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldId + " > 0 ";

            if (!string.IsNullOrEmpty(companyId))
            {
                condition += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldCompanyId + " = '" + companyId + "')";
            }
            if (!string.IsNullOrEmpty(departmentId))
            {
                /*
                用非递归调用的建议方法
                sql += " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDepartmentId 
                    + " IN ( SELECT " + BaseOrganizationEntity.FieldId 
                    + " FROM " + BaseOrganizationEntity.TableName 
                    + " WHERE " + BaseOrganizationEntity.FieldId + " = " + departmentId + " OR " + BaseOrganizationEntity.FieldParentId + " = " + departmentId + ")";
                */

                /*
                BaseOrganizationManager organizeManager = new BaseOrganizationManager(this.UserInfo);
                string[] ids = organizeManager.GetChildrensId(BaseOrganizationEntity.FieldId, departmentId, BaseOrganizationEntity.FieldParentId);
                if (ids != null && ids.Length > 0)
                {
                    condition += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldCompanyId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldSubCompanyId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDepartmentId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldWorkgroupId + " IN (" + StringUtil.ArrayToList(ids) + "))";
                }
                */
                condition += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDepartmentId + " = " + departmentId + ")";
            }
            if (!string.IsNullOrEmpty(roleId))
            {
                var tableNameUserRole = UserInfo.SystemCode + "UserRole";
                condition += " AND ( " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldId + " IN "
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
            recordCount = DbUtil.GetCount(DbHelper, CurrentTableName, condition);
            CurrentTableName = "BaseUser";
            if (ShowUserLogonInfo)
            {
                CurrentTableName = BaseUserEntity.TableName + " LEFT OUTER JOIN " + BaseUserLogonEntity.TableName + " ON " + BaseUserEntity.TableName + ".Id = " + BaseUserLogonEntity.TableName + ".Id ";
            }
            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.SqlServer:
                case CurrentDbType.Access:
                    SelectFields = BaseUserEntity.TableName + ".* ";
                    if (ShowUserLogonInfo)
                    {
                        SelectFields += "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldFirstVisit
                                    + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldPreviousVisit
                                    + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldLastVisit
                                    + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldIpAddress
                                    + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldMacAddress
                                    + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldLogonCount
                                    + "," + BaseUserLogonEntity.TableName + "." + BaseUserLogonEntity.FieldUserOnline;
                    }
                    break;
                case CurrentDbType.Oracle:
                case CurrentDbType.MySql:
                case CurrentDbType.Db2:
                    break;
            }
            return DbUtil.GetDataTableByPage(DbHelper, CurrentTableName, SelectFields, pageIndex, pageSize, condition, order);
        }
        #endregion
    }
}