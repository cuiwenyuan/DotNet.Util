//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------
using System.Data;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using System;
    using System.Linq;
    using System.Reflection;
    using Util;

    /// <summary>
    /// BaseUserLogonManager
    /// 用户登录管理
    /// 
    /// 修改记录
    /// 
    ///		2022.02.08 版本：1.0 Troy.Cui	新增。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2022.02.08</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        #region 高级查询

        /// <summary>
        /// 按条件分页高级查询(带记录状态Enabled和删除状态Deleted)
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="organizationId">查看公司主键</param>
        /// <param name="userId">查看用户主键</param>
        /// <param name="roleId">角色编号</param>
        /// <param name="roleIdExcluded">排除角色编号</param>
        /// <param name="moduleId">模块菜单编号</param>
        /// <param name="moduleIdExcluded">排除模块菜单编号</param>
        /// <param name="showInvisible">是否显示隐藏</param>
        /// <param name="disabledUserOnly">仅显示禁用</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="searchKey">查询关键字</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="showDisabled">是否显示无效记录</param>
        /// <param name="showDeleted">是否显示已删除记录</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string systemCode, string organizationId, string userId, string roleId, string roleIdExcluded, string moduleId, string moduleIdExcluded, bool showInvisible, bool disabledUserOnly, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = "CreateTime", string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            //用户表名
            var tableNameUser = BaseUserEntity.CurrentTableName;
            //用户登录表名
            var tableNameUserLogon = BaseUserLogonEntity.CurrentTableName;

            var sb = PoolUtil.StringBuilder.Get().Append(" 1 = 1");
            //只显示已锁定用户
            if (disabledUserOnly)
            {
                sb.Append(" AND " + BaseUserEntity.FieldEnabled + " = 0");
                //已锁定
                showDisabled = true;
                //未删除
                showDeleted = false;
            }

            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseUserEntity.FieldEnabled + "  = 1 ");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseUserEntity.FieldDeleted + " = 0");
            }

            if (ValidateUtil.IsInt(organizationId) && organizationId.ToInt() > 0)
            {
                //只选择当前
                //sb.Append(" AND (" + BaseUserEntity.FieldCompanyId + " = " + organizationId + ")";
                //只选择当前和下一级
                //sb.Append(" AND " + BaseUserEntity.FieldDepartmentId 
                //    + " IN ( SELECT " + BaseOrganizationEntity.FieldId 
                //    + " FROM " + BaseOrganizationEntity.CurrentTableName 
                //    + " WHERE " + BaseOrganizationEntity.FieldId + " = " + organizationId + " OR " + BaseOrganizationEntity.FieldParentId + " = " + organizationId + ")";

                //所有下级的都列出来
                var organizationManager = new BaseOrganizationManager(UserInfo);
                var ids = organizationManager.GetChildrensId(BaseOrganizationEntity.FieldId, organizationId, BaseOrganizationEntity.FieldParentId);
                if (ids != null && ids.Length > 0)
                {
                    sb.Append(" AND (" + BaseUserEntity.FieldCompanyId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseUserEntity.FieldSubCompanyId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseUserEntity.FieldDepartmentId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseUserEntity.FieldSubDepartmentId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseUserEntity.FieldWorkgroupId + " IN (" + StringUtil.ArrayToList(ids) + "))");
                }

            }
            //if (ValidateUtil.IsInt(departmentId))
            //{
            //    sb.Append(" AND " + BaseUserEntity.FieldDepartmentId + " = " + departmentId;
            //}
            if (ValidateUtil.IsInt(userId))
            {
                // sb.Append(" AND UserId = " + userId);
            }
            //是否显示已隐藏记录
            if (!showInvisible)
            {
                sb.Append(" AND " + BaseUserEntity.FieldIsVisible + "  = 1 ");
            }
            //角色
            var tableNameUserRole = GetUserRoleTableName(UserInfo.SystemCode);
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableNameUserRole = GetUserRoleTableName(systemCode);
            }
            //指定角色
            if (ValidateUtil.IsInt(roleId))
            {
                sb.Append(" AND ( " + BaseUserEntity.FieldId + " IN ");
                sb.Append(" (SELECT DISTINCT " + BaseUserRoleEntity.FieldUserId);
                sb.Append(" FROM " + tableNameUserRole);
                sb.Append(" WHERE " + BaseUserRoleEntity.FieldRoleId + " = '" + roleId + "'");
                sb.Append(" AND " + BaseUserRoleEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BaseUserRoleEntity.FieldDeleted + " = 0)) ");
            }
            //排除指定角色
            if (ValidateUtil.IsInt(roleIdExcluded))
            {
                sb.Append(" AND ( " + BaseUserEntity.FieldId + " NOT IN ");
                sb.Append(" (SELECT DISTINCT " + BaseUserRoleEntity.FieldUserId);
                sb.Append(" FROM " + tableNameUserRole);
                sb.Append(" WHERE " + BaseUserRoleEntity.FieldRoleId + " = '" + roleIdExcluded + "'");
                sb.Append(" AND " + BaseUserRoleEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BaseUserRoleEntity.FieldDeleted + " = 0)) ");
            }
            //用户菜单模块表
            var tableNamePermission = GetPermissionTableName(UserInfo.SystemCode);
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableNamePermission = GetPermissionTableName(systemCode);
            }
            //指定的菜单模块
            if (ValidateUtil.IsInt(moduleId))
            {
                sb.Append(" AND ( " + BaseUserEntity.FieldId + " IN ");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldResourceId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldPermissionId + " = '" + moduleId + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + BaseUserEntity.CurrentTableName + "' ");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0)) ");
            }
            //排除指定菜单模块
            if (ValidateUtil.IsInt(moduleIdExcluded))
            {
                sb.Append(" AND ( " + BaseUserEntity.FieldId + " NOT IN ");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldResourceId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldPermissionId + " = '" + moduleIdExcluded + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + BaseUserEntity.CurrentTableName + "' ");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0)) ");
            }
            //关键词
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseUserEntity.FieldRealName + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseUserEntity.FieldUserName + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseUserEntity.FieldNickName + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseUserEntity.FieldCompanyName + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseUserEntity.FieldSubCompanyName + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseUserEntity.FieldDepartmentName + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseUserEntity.FieldSubDepartmentName + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseUserEntity.FieldWorkgroupName + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseUserEntity.FieldCode + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseUserEntity.FieldDescription + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseUserEntity.FieldQuickQuery + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseUserEntity.FieldSimpleSpelling + " LIKE N'%" + searchKey + "%')");
            }

            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseUserEntity.FieldCreateTime + " >= " + dbHelper.ToDbTime(startTime));
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseUserEntity.FieldCreateTime + " <= " + dbHelper.ToDbTime(endTime.ToDateTime().Date.AddDays(1).AddMilliseconds(-1)));
            }

            sb.Replace(" 1 = 1 AND ", " ");
            //重新构造viewName
            var sbView = PoolUtil.StringBuilder.Get();

            sbView.Append("SELECT DISTINCT " + tableNameUser + "." + BaseUserEntity.FieldId);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldUserFrom);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldUserName);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldRealName);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldNickName);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldCode);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldIdCard);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldQuickQuery);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSimpleSpelling);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldCompanyId);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldCompanyName);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSubCompanyId);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSubCompanyName);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldDepartmentId);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldDepartmentName);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSubDepartmentId);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSubDepartmentName);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldWorkgroupId);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldWorkgroupName);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldWorkCategory);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSecurityLevel);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldTitle);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldDuty);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldLang);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldGender);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldBirthday);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldScore);
            //sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldFans);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldHomeAddress);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSignature);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldTheme);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldIsStaff);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldIsVisible);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldProvince);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldCity);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldDistrict);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldAuditStatus);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldEnabled);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldDeleted);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSortCode);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldDescription);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldIsAdministrator);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldIsCheckBalance);
            //用户表
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldCreateTime);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldCreateUserId);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldCreateBy);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldUpdateTime);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldUpdateUserId);
            sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldUpdateBy);

            //用户登录表
            sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldAllowStartTime);
            sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldAllowEndTime);
            sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldLockStartTime);
            sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldLockEndTime);
            sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldFirstVisitTime);
            sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldPreviousVisitTime);
            sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldLastVisitTime);
            sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldChangePasswordTime);
            sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldLogonCount);
            sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldConcurrentUser);
            sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldUserOnline);
            //不从用户登录表读取这些字段，从用户表读取即可
            //sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldCreateTime);
            //sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldCreateUserId);
            //sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldCreateBy);
            //sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldUpdateTime);
            //sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldUpdateUserId);
            //sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldUpdateBy);

            sbView.Append(" FROM " + tableNameUser + " INNER JOIN " + tableNameUserLogon);
            sbView.Append(" ON " + tableNameUser + "." + BaseUserEntity.FieldId + " = " + tableNameUserLogon + "." + BaseUserLogonEntity.FieldUserId);

            //指定角色，就读取相应的UserRole授权日期
            if (ValidateUtil.IsInt(roleId))
            {
                sbView.Clear();
                sbView.Append("SELECT DISTINCT " + tableNameUser + "." + BaseUserEntity.FieldId);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldUserFrom);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldUserName);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldRealName);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldNickName);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldCode);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldIdCard);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldQuickQuery);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSimpleSpelling);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldCompanyId);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldCompanyName);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSubCompanyId);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSubCompanyName);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldDepartmentId);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldDepartmentName);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSubDepartmentId);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSubDepartmentName);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldWorkgroupId);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldWorkgroupName);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldWorkCategory);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSecurityLevel);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldTitle);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldDuty);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldLang);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldGender);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldBirthday);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldScore);
                //sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldFans);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldHomeAddress);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSignature);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldTheme);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldIsStaff);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldIsVisible);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldProvince);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldCity);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldDistrict);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldAuditStatus);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldEnabled);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldDeleted);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSortCode);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldDescription);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldIsAdministrator);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldIsCheckBalance);
                //用户登录表
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldAllowStartTime);
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldAllowEndTime);
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldLockStartTime);
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldLockEndTime);
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldFirstVisitTime);
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldPreviousVisitTime);
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldLastVisitTime);
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldChangePasswordTime);
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldLogonCount);
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldConcurrentUser);
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldUserOnline);
                //授权日期
                sbView.Append("," + tableNameUserRole + "." + BaseUserRoleEntity.FieldCreateTime);
                sbView.Append("," + tableNameUserRole + "." + BaseUserRoleEntity.FieldCreateUserId);
                sbView.Append("," + tableNameUserRole + "." + BaseUserRoleEntity.FieldCreateBy);
                sbView.Append("," + tableNameUserRole + "." + BaseUserRoleEntity.FieldUpdateTime);
                sbView.Append("," + tableNameUserRole + "." + BaseUserRoleEntity.FieldUpdateUserId);
                sbView.Append("," + tableNameUserRole + "." + BaseUserRoleEntity.FieldUpdateBy);
                sbView.Append(" FROM " + tableNameUser + " INNER JOIN " + tableNameUserRole);
                sbView.Append(" ON " + tableNameUser + "." + BaseUserEntity.FieldId + " = " + tableNameUserRole + "." + BaseUserRoleEntity.FieldUserId);
                sbView.Append(" INNER JOIN " + tableNameUserLogon);
                sbView.Append(" ON " + tableNameUser + "." + BaseUserEntity.FieldId + " = " + tableNameUserLogon + "." + BaseUserLogonEntity.FieldUserId);
                sbView.Append(" WHERE (" + tableNameUserRole + "." + BaseUserRoleEntity.FieldRoleId + " = " + roleId + ")");
            }
            //指定菜单模块，就读取相应的Permission授权日期
            else if (ValidateUtil.IsInt(moduleId))
            {
                sbView.Clear();
                sbView.Append("SELECT DISTINCT " + tableNameUser + "." + BaseUserEntity.FieldId);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldUserFrom);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldUserName);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldRealName);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldNickName);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldCode);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldIdCard);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldQuickQuery);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSimpleSpelling);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldCompanyId);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldCompanyName);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSubCompanyId);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSubCompanyName);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldDepartmentId);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldDepartmentName);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSubDepartmentId);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSubDepartmentName);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldWorkgroupId);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldWorkgroupName);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldWorkCategory);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSecurityLevel);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldTitle);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldDuty);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldLang);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldGender);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldBirthday);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldScore);
                //sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldFans);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldHomeAddress);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSignature);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldTheme);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldIsStaff);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldIsVisible);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldProvince);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldCity);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldDistrict);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldAuditStatus);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldEnabled);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldDeleted);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldSortCode);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldDescription);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldIsAdministrator);
                sbView.Append("," + tableNameUser + "." + BaseUserEntity.FieldIsCheckBalance);
                //用户登录表
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldAllowStartTime);
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldAllowEndTime);
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldLockStartTime);
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldLockEndTime);
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldFirstVisitTime);
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldPreviousVisitTime);
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldLastVisitTime);
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldChangePasswordTime);
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldLogonCount);
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldConcurrentUser);
                sbView.Append("," + tableNameUserLogon + "." + BaseUserLogonEntity.FieldUserOnline);
                //授权日期
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldCreateTime);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldCreateUserId);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldCreateBy);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateTime);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateUserId);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateBy);
                sbView.Append(" FROM " + tableNameUser + " INNER JOIN " + tableNamePermission);
                sbView.Append(" ON " + tableNameUser + "." + BaseUserEntity.FieldId + " = " + tableNamePermission + "." + BasePermissionEntity.FieldResourceId);
                sbView.Append(" INNER JOIN " + tableNameUserLogon);
                sbView.Append(" ON " + tableNameUser + "." + BaseUserEntity.FieldId + " = " + tableNameUserLogon + "." + BaseUserLogonEntity.FieldUserId);
                sbView.Append(" WHERE (" + tableNamePermission + "." + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameUser + "')");
                sbView.Append(" AND (" + tableNamePermission + "." + BasePermissionEntity.FieldPermissionId + " = " + moduleId + ")");
            }
            //从视图读取
            if (sbView.Length > 0)
            {
                tableNameUser = sbView.Return();
            }

            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, tableNameUser, sb.Return());
        }
        #endregion

        #region SetAdministrator设置超级管理员

        /// <summary>
        /// 设置超级管理员
        /// </summary>
        /// <param name="userIds">用户编号</param>
        /// <returns>更新成功记录数</returns>
        public int SetAdministrator(string[] userIds)
        {
            var result = 0;
            if (userIds != null)
            {
                result = Update(BaseUserEntity.FieldId, userIds, new KeyValuePair<string, object>(BaseUserEntity.FieldIsAdministrator, 1));
                //操作日志
                var entity = new BaseLogEntity
                {
                    Parameters = userIds.ToString(),
                    Description = "设置超级管理员：" + ((result >= 1) ? "成功" : "失败")
                };
                if (UserInfo != null)
                {
                    entity.UserId = UserInfo.Id.ToInt();
                    entity.RealName = UserInfo.RealName;
                }
                new BaseLogManager(UserInfo).Add(entity);
            }

            return result;
        }
        #endregion

        #region SetAdministrator撤销设置超级管理员

        /// <summary>
        /// 撤销设置超级管理员
        /// </summary>
        /// <param name="userIds">用户编号</param>
        /// <returns>更新成功记录数</returns>
        public int UndoSetAdministrator(string[] userIds)
        {
            var result = 0;
            if (userIds != null)
            {
                result = Update(BaseUtil.FieldId, userIds, new KeyValuePair<string, object>(BaseUserEntity.FieldIsAdministrator, 0));
                //操作日志
                var entity = new BaseLogEntity
                {
                    Parameters = userIds.ToString(),
                    Description = "撤销设置超级管理员：" + ((result >= 1) ? "成功" : "失败")
                };
                if (UserInfo != null)
                {
                    entity.UserId = UserInfo.Id.ToInt();
                    entity.RealName = UserInfo.RealName;
                }
                new BaseLogManager(UserInfo).Add(entity);
            }

            return result;
        }
        #endregion

        #region GetViewEntity
        /// <summary>
        /// 获取用户实体
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public ViewBaseUserEntity GetViewEntity(string id)
        {
            ViewBaseUserEntity entity = null;
            if (ValidateUtil.IsInt(id))
            {
                var entityUser = GetEntity(id);
                if (entityUser != null)
                {
                    entity = new ViewBaseUserEntity
                    {
                        Id = entityUser.Id,
                        UserFrom = entityUser.UserFrom,
                        UserName = entityUser.UserName,
                        RealName = entityUser.RealName,
                        NickName = entityUser.NickName,
                        AvatarUrl = entityUser.AvatarUrl,
                        Code = entityUser.Code,
                        EmployeeNumber = entityUser.EmployeeNumber,
                        IdCard = entityUser.IdCard,
                        QuickQuery = entityUser.QuickQuery,
                        SimpleSpelling = entityUser.SimpleSpelling,
                        CompanyId = entityUser.CompanyId,
                        CompanyCode = entityUser.CompanyCode,
                        CompanyName = entityUser.CompanyName,
                        SubCompanyId = entityUser.SubCompanyId,
                        SubCompanyName = entityUser.SubCompanyName,
                        DepartmentId = entityUser.DepartmentId,
                        DepartmentName = entityUser.DepartmentName,
                        SubDepartmentId = entityUser.SubDepartmentId,
                        SubDepartmentName = entityUser.SubDepartmentName,
                        WorkgroupId = entityUser.WorkgroupId,
                        WorkgroupName = entityUser.WorkgroupName,
                        WorkCategory = entityUser.WorkCategory,
                        SecurityLevel = entityUser.SecurityLevel,
                        Title = entityUser.Title,
                        Duty = entityUser.Duty,
                        Lang = entityUser.Lang,
                        Gender = entityUser.Gender,
                        Birthday = entityUser.Birthday,
                        Score = entityUser.Score,
                        Fans = entityUser.Fans,
                        HomeAddress = entityUser.HomeAddress,
                        Signature = entityUser.Signature,
                        Theme = entityUser.Theme,
                        IsStaff = entityUser.IsStaff,
                        IsVisible = entityUser.IsVisible,
                        Country = entityUser.Country,
                        State = entityUser.State,
                        Province = entityUser.Province,
                        City = entityUser.City,
                        District = entityUser.District,
                        AuditStatus = entityUser.AuditStatus,
                        ManagerUserId = entityUser.ManagerUserId,
                        IsAdministrator = entityUser.IsAdministrator,
                        IsCheckBalance = entityUser.IsCheckBalance,
                        Description = entityUser.Description,
                        SortCode = entityUser.SortCode,
                        Deleted = entityUser.Deleted,
                        Enabled = entityUser.Enabled,
                        CreateTime = entityUser.CreateTime,
                        CreateUserId = entityUser.CreateUserId,
                        CreateUserName = entityUser.CreateUserName,
                        CreateBy = entityUser.CreateBy,
                        CreateIp = entityUser.CreateIp,
                        UpdateTime = entityUser.UpdateTime,
                        UpdateUserId = entityUser.UpdateUserId,
                        UpdateUserName = entityUser.UpdateUserName,
                        UpdateBy = entityUser.UpdateBy,
                        UpdateIp = entityUser.UpdateIp
                    };

                    var entityUserLogon = new BaseUserLogonManager(UserInfo).GetEntityByUserId(id);
                    if (entityUserLogon != null)
                    {
                        //entity.UserPassword = entityUserLogon.UserPassword;
                        entity.OpenId = entityUserLogon.OpenId;
                        entity.AllowStartTime = entityUserLogon.AllowStartTime;
                        entity.AllowEndTime = entityUserLogon.AllowEndTime;
                        entity.LockStartTime = entityUserLogon.LockStartTime;
                        entity.LockEndTime = entityUserLogon.LockEndTime;
                        entity.FirstVisitTime = entityUserLogon.FirstVisitTime;
                        entity.PreviousVisitTime = entityUserLogon.PreviousVisitTime;
                        entity.LastVisitTime = entityUserLogon.LastVisitTime;
                        entity.ChangePasswordTime = entityUserLogon.ChangePasswordTime;
                        entity.LogonCount = entityUserLogon.LogonCount;
                        entity.ConcurrentUser = entityUserLogon.ConcurrentUser;
                        entity.ShowCount = entityUserLogon.ShowCount;
                        entity.PasswordErrorCount = entityUserLogon.PasswordErrorCount;
                        entity.UserOnline = entityUserLogon.UserOnline;
                        entity.CheckIpAddress = entityUserLogon.CheckIpAddress;
                        entity.VerificationCode = entityUserLogon.VerificationCode;
                        entity.IpAddress = entityUserLogon.IpAddress;
                        entity.MacAddress = entityUserLogon.MacAddress;
                        entity.Question = entityUserLogon.Question;
                        entity.AnswerQuestion = entityUserLogon.AnswerQuestion;
                        //entity.Salt = entityUserLogon.Salt;
                        entity.OpenIdTimeoutTime = entityUserLogon.OpenIdTimeoutTime;
                        entity.SystemCode = entityUserLogon.SystemCode;
                        entity.IpAddressName = entityUserLogon.IpAddressName;
                        entity.PasswordStrength = entityUserLogon.PasswordStrength;
                        entity.ComputerName = entityUserLogon.ComputerName;
                        entity.NeedModifyPassword = entityUserLogon.NeedModifyPassword;
                    }

                    var entityUserContact = new BaseUserContactManager(UserInfo).GetEntityByUserId(id);
                    if (entityUserContact != null)
                    {
                        entity.Mobile = entityUserContact.Mobile;
                        entity.ShortNumber = entityUserContact.ShortNumber;
                        entity.Ww = entityUserContact.Ww;
                        entity.WeChat = entityUserContact.WeChat;
                        entity.Telephone = entityUserContact.Telephone;
                        entity.Extension = entityUserContact.Extension;
                        entity.Qq = entityUserContact.Qq;
                        entity.Email = entityUserContact.Email;
                        entity.CompanyEmail = entityUserContact.CompanyEmail;
                        entity.EmergencyContact = entityUserContact.EmergencyContact;
                    }
                }
            }

            return entity;
        }
        #endregion

        #region 根据角色Code获取用户表
        /// <summary>
        /// 根据角色Code获取用户表
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleCode"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public DataTable GetDataTableByRoleCode(string systemCode, string roleCode, string companyId = null)
        {
            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = "Base";
            }
            var tableNameUserRole = GetUserRoleTableName(systemCode);
            var tableNameRole = GetRoleTableName(systemCode);
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + SelectFields + " FROM " + BaseUserEntity.CurrentTableName
                            + " WHERE " + BaseUserEntity.FieldEnabled + " = 1"
                            + " AND " + BaseUserEntity.FieldDeleted + " = 0"
                            + " AND ( " + BaseUserEntity.FieldId + " IN "
                            + " (SELECT  " + BaseUserRoleEntity.FieldUserId
                            + " FROM " + tableNameUserRole
                            + " WHERE " + BaseUserRoleEntity.FieldRoleId + " IN (SELECT " + BaseRoleEntity.FieldId + " FROM " + tableNameRole + " WHERE " + BaseRoleEntity.FieldCode + " = N'" + roleCode + "')"
                            + " AND " + BaseUserRoleEntity.FieldSystemCode + " = N'" + systemCode + "'"
                            + " AND " + BaseUserRoleEntity.FieldEnabled + " = 1"
                            + " AND " + BaseUserRoleEntity.FieldDeleted + " = 0)) "
                            + " ORDER BY  " + BaseUserEntity.FieldSortCode);

            return DbHelper.Fill(sb.Return());
        }
        #endregion

        #region public string CreateUser(BaseUserInfo userInfo, BaseUserEntity entity, BaseUserContactEntity userContactEntity, out Status status, out string statusMessage)

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="entity">用户实体</param>
        /// <param name="userLogonEntity">用户登录实体</param>
        /// <param name="userContactEntity">用户联系方式</param>
        /// <param name="status">状态</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>主键</returns>
        public string CreateUser(BaseUserInfo userInfo, BaseUserEntity entity, BaseUserLogonEntity userLogonEntity, BaseUserContactEntity userContactEntity, out Status status, out string statusMessage)
        {
            var result = string.Empty;

            // 加强安全验证防止未授权匿名调用
#if (!DEBUG)
            BaseSystemInfo.IsAuthorized(userInfo);
#endif

            var userManager = new BaseUserManager(userInfo);
            result = userManager.AddUser(entity, userLogonEntity);
            status = userManager.Status;
            statusMessage = userManager.GetStateMessage();

            // 20140219 JiRiGaLa 添加成功的用户才增加联系方式
            if (!string.IsNullOrEmpty(result) && status == Status.OkAdd && userContactEntity != null)
            {
                // 添加联系方式
                userContactEntity.UserId = result.ToInt();
                var userContactManager = new BaseUserContactManager(userInfo);
                userContactEntity.CompanyId = entity.CompanyId;
                userContactManager.Add(userContactEntity);
            }

            // 自己不用给自己发提示信息，这个提示信息是为了提高工作效率的，还是需要审核通过的，否则垃圾信息太多了
            if (entity.Enabled == 0 && status == Status.OkAdd)
            {
                // 不是系统管理员添加
                if (!BaseUserManager.IsAdministrator(userInfo.Id))
                {
                    // 给超级管理员群组发信息
                    var roleManager = new BaseRoleManager(userInfo);
                    var roleIds = roleManager.GetIds(new KeyValuePair<string, object>(BaseRoleEntity.FieldCode, "Administrators"));
                    var userIds = userManager.GetIds(new KeyValuePair<string, object>(BaseUserEntity.FieldCode, "Administrator"));
                    // 发送请求审核的信息
                    //var messageEntity = new BaseMessageEntity
                    //{
                    //    FunctionCode = MessageFunction.WaitForAudit.ToString(),

                    //    // Pcsky 2012.05.04 显示申请的用户名
                    //    Contents = userInfo.RealName + "(" + userInfo.IpAddress + ")" + AppMessage.UserServiceApplication + entity.UserName + AppMessage.UserServiceCheck
                    //};
                    //messageEntity.Contents = result.RealName + "(" + result.IPAddress + ")" + AppMessage.UserService_Application + userEntity.RealName + AppMessage.UserService_Check;

                    //var messageManager = new BaseMessageManager(dbHelper, userInfo);
                    //messageManager.BatchSend(userIds, null, roleIds, messageEntity, false);
                }
            }

            return result;
        }
        #endregion

        /// <summary>
        /// 显示用户登录信息
        /// </summary>
        public bool ShowUserLogonInfo = false;

        #region public BaseUserInfo ConvertToUserInfo(BaseUserEntity userEntity, BaseUserLogonEntity userLogonEntity = null, bool validateUserOnly = false)
        /// <summary>
        /// 转换为UserInfo用户信息
        /// </summary>
        /// <param name="userEntity"></param>
        /// <param name="userLogonEntity"></param>
        /// <param name="validateUserOnly"></param>
        /// <returns></returns>
        public BaseUserInfo ConvertToUserInfo(BaseUserEntity userEntity, BaseUserLogonEntity userLogonEntity = null, bool validateUserOnly = false)
        {
            var userInfo = new BaseUserInfo();
            return ConvertToUserInfo(userInfo, userEntity, userLogonEntity, validateUserOnly);
        }
        #endregion

        #region public BaseUserInfo ConvertToUserInfo(BaseUserInfo userInfo, BaseUserEntity userEntity, BaseUserLogonEntity userLogonEntity = null, bool validateUserOnly = false)
        /// <summary>
        /// 转换为UserInfo用户信息
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="userEntity"></param>
        /// <param name="userLogonEntity"></param>
        /// <param name="validateUserOnly"></param>
        /// <returns></returns>
        public BaseUserInfo ConvertToUserInfo(BaseUserInfo userInfo, BaseUserEntity userEntity, BaseUserLogonEntity userLogonEntity = null, bool validateUserOnly = false)
        {
            if (userEntity == null)
            {
                return null;
            }
            userInfo.Id = userEntity.Id.ToString();
            userInfo.UserId = userEntity.Id;
            userInfo.IsAdministrator = userEntity.IsAdministrator == 1;
            userInfo.Code = userEntity.Code;
            userInfo.UserName = userEntity.UserName;
            userInfo.RealName = userEntity.RealName;
            userInfo.NickName = userEntity.NickName;
            if (userLogonEntity != null)
            {
                userInfo.OpenId = userLogonEntity.OpenId;
            }
            userInfo.CompanyId = userEntity.CompanyId.ToString();
            userInfo.CompanyName = userEntity.CompanyName;
            userInfo.CompanyCode = BaseOrganizationManager.GetEntityByCache(userEntity.CompanyId.ToString())?.Code;
            //Troy.Cui 2018.08.04 增加Sub的转换
            userInfo.SubCompanyId = userEntity.SubCompanyId.ToString();
            userInfo.SubCompanyName = userEntity.SubCompanyName;
            userInfo.SubCompanyCode = BaseOrganizationManager.GetEntityByCache(userEntity.SubCompanyId.ToString())?.Code;
            userInfo.DepartmentId = userEntity.DepartmentId.ToString();
            userInfo.DepartmentName = userEntity.DepartmentName;
            userInfo.DepartmentCode = BaseOrganizationManager.GetEntityByCache(userEntity.DepartmentId.ToString())?.Code;
            userInfo.SubDepartmentId = userEntity.SubDepartmentId.ToString();
            userInfo.SubDepartmentName = userEntity.SubDepartmentName;
            userInfo.SubDepartmentCode = BaseOrganizationManager.GetEntityByCache(userEntity.SubDepartmentId.ToString())?.Code;
            userInfo.WorkgroupId = userEntity.WorkgroupId.ToString();
            userInfo.WorkgroupName = userEntity.WorkgroupName;

            //2016-11-23 欧腾飞加入 companyCode 和数字签名
            userInfo.Signature = userEntity.Signature;

            BaseOrganizationEntity organizationEntity = null;

            if (!string.IsNullOrEmpty(userInfo.CompanyId))
            {
                try
                {
                    organizationEntity = BaseOrganizationManager.GetEntityByCache(userInfo.CompanyId.ToString());
                }
                catch (Exception ex)
                {
                    var writeMessage = "BaseOrganizationManager.GetEntityByCache:发生时间:" + DateTime.Now
                        + Environment.NewLine + "CompanyId 无法缓存获取:" + userInfo.CompanyId
                        + Environment.NewLine + "Message:" + ex.Message
                        + Environment.NewLine + "Source:" + ex.Source
                        + Environment.NewLine + "StackTrace:" + ex.StackTrace
                        + Environment.NewLine + "TargetSite:" + ex.TargetSite
                        + Environment.NewLine;

                    LogUtil.WriteLog(writeMessage, "Exception");
                }

                if (organizationEntity == null)
                {
                    var organizationManager = new BaseOrganizationManager();
                    organizationEntity = organizationManager.GetEntity(userInfo.CompanyId);
                    // 2015-12-06 吉日嘎拉 进行记录日志功能改进
                    if (organizationEntity == null)
                    {
                        var writeMessage = "BaseOrganizationManager.GetEntity:发生时间:" + DateTime.Now
                        + Environment.NewLine + "CompanyId 无法缓存获取:" + userInfo.CompanyId
                        + Environment.NewLine + "BaseUserInfo:" + userInfo.Serialize();

                        LogUtil.WriteLog(writeMessage, "Log");
                    }
                }
                if (organizationEntity != null)
                {
                    userInfo.CompanyCode = organizationEntity.Code;
                }
            }
            ////部门数据需要从部门表里读取

            //if (!validateUserOnly && !string.IsNullOrEmpty(userInfo.DepartmentId))
            //{
            //    organizationEntity = BaseOrganizationManager.GetEntityByCache(userInfo.DepartmentId);
            //}
            //else
            //{
            //    if (organizationManager == null)
            //    {
            //        organizationManager = new Business.BaseOrganizationManager();
            //    }
            //    organizationEntity = organizationManager.GetEntity(userInfo.DepartmentId);
            //}
            //if (organizationEntity != null)
            //{
            //    userInfo.DepartmentCode = organizationEntity.Code;
            //}


            return userInfo;
        }

        #endregion

        #region public BaseUserEntity GetEntityByCode(string userCode)
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userCode">用户编号</param>
        /// <returns>用户实体</returns>
        public BaseUserEntity GetEntityByCode(string userCode)
        {
            BaseUserEntity entity = null;
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserEntity.FieldCode, userCode),
                new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
            };
            var dt = GetDataTable(parameters);
            if (dt != null && dt.Rows.Count > 0)
            {
                entity = BaseEntity.Create<BaseUserEntity>(dt);
            }
            return entity;
        }

        #endregion

        #region public BaseUserEntity GetEntityByCompanyIdByCode(string companyId, string userCode)
        /// <summary>
        /// 获取用户实体
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public BaseUserEntity GetEntityByCompanyIdByCode(string companyId, string userCode)
        {
            BaseUserEntity entity = null;
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserEntity.FieldCode, userCode),
                new KeyValuePair<string, object>(BaseUserEntity.FieldCompanyId, companyId),
                new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
            };
            var dt = GetDataTable(parameters);
            if (dt != null && dt.Rows.Count > 0)
            {
                entity = BaseEntity.Create<BaseUserEntity>(dt);
            }
            return entity;
        }

        #endregion

        #region public BaseUserEntity GetEntityByCompanyCodeByCode(string companyCode, string userCode)
        /// <summary>
        /// 根据公司编码和用户编码获取用户实体
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public BaseUserEntity GetEntityByCompanyCodeByCode(string companyCode, string userCode)
        {
            BaseUserEntity result = null;
            var organizationEntity = BaseOrganizationManager.GetEntityByCodeByCache(companyCode);
            if (organizationEntity == null)
            {
                return result;
            }

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserEntity.FieldCode, userCode),
                new KeyValuePair<string, object>(BaseUserEntity.FieldCompanyId, organizationEntity.Id),
                new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
            };
            var dt = GetDataTable(parameters);
            if (dt != null && dt.Rows.Count > 0)
            {
                result = BaseEntity.Create<BaseUserEntity>(dt);
            }
            return result;
        }
        #endregion

        #region public BaseUserEntity GetEntityByUserName(string userName)

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>用户实体</returns>
        public BaseUserEntity GetEntityByUserName(string userName)
        {
            BaseUserEntity entity = null;
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserEntity.FieldUserName, userName),
                new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
            };
            var dt = GetDataTable(parameters);
            if (dt != null && dt.Rows.Count > 0)
            {
                // 用户找到状态
                Status = Status.UserDuplicate;
                StatusCode = Status.UserDuplicate.ToString();
                StatusMessage = GetStateMessage(StatusCode);
                entity = BaseEntity.Create<BaseUserEntity>(dt);
            }
            else
            {
                // 用户没有找到状态
                Status = Status.UserNotFound;
                StatusCode = Status.UserNotFound.ToString();
                StatusMessage = GetStateMessage(StatusCode);
            }
            return entity;
        }
        #endregion

        #region public BaseUserEntity GetEntityByRealName(string realName)

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="realName">姓名</param>
        /// <returns>用户实体</returns>
        public BaseUserEntity GetEntityByRealName(string realName)
        {
            BaseUserEntity entity = null;
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserEntity.FieldRealName, realName),
                new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
            };
            var dt = GetDataTable(parameters);
            if (dt != null && dt.Rows.Count > 0)
            {
                entity = BaseEntity.Create<BaseUserEntity>(dt);
            }
            return entity;
        }

        #endregion

        #region public BaseUserEntity GetEntityByNickName(string nickName)
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="nickName">昵称</param>
        /// <returns>用户实体</returns>
        public BaseUserEntity GetEntityByNickName(string nickName)
        {
            BaseUserEntity entity = null;
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserEntity.FieldNickName, nickName),
                new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
            };
            var dt = GetDataTable(parameters);
            if (dt != null && dt.Rows.Count > 0)
            {
                entity = BaseEntity.Create<BaseUserEntity>(dt);
            }
            else
            {
                // 用户没有找到状态
                Status = Status.UserNotFound;
                StatusCode = Status.UserNotFound.ToString();
                StatusMessage = GetStateMessage(StatusCode);
            }
            return entity;
        }
        #endregion

        #region public BaseUserEntity GetEntityByOpenId(string openId)
        /// <summary>
        /// 根据OpenId获取用户实体
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public BaseUserEntity GetEntityByOpenId(string openId)
        {
            BaseUserEntity userEntity = null;

            // 用户没有找到状态
            Status = Status.UserNotFound;
            StatusCode = Status.UserNotFound.ToString();
            StatusMessage = GetStateMessage(StatusCode);
            // 检查是否有效的合法的参数
            if (!string.IsNullOrEmpty(openId))
            {
                var userLogonManager = new BaseUserLogonManager();
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserLogonEntity.FieldOpenId, openId)
                };
                var entity = userLogonManager.GetEntity(parameters);
                if (entity != null)
                {
                    parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BaseUserEntity.FieldId, entity.UserId),
                        new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0),
                        new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1)
                    };
                    var dt = GetDataTable(parameters);
                    if (dt != null && dt.Rows.Count == 1)
                    {
                        userEntity = BaseEntity.Create<BaseUserEntity>(dt);
                    }
                }
            }

            return userEntity;
        }

        #endregion

        #region public BaseUserEntity GetEntityByEmail(string email)
        /// <summary>
        /// 根据邮箱获取用户实体
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public BaseUserEntity GetEntityByEmail(string email)
        {
            BaseUserEntity userEntity = null;

            // 用户没有找到状态
            Status = Status.UserNotFound;
            StatusCode = Status.UserNotFound.ToString();
            StatusMessage = GetStateMessage(StatusCode);
            // 检查是否有效的合法的参数
            if (!string.IsNullOrEmpty(email) && ValidateUtil.IsEmail(email))
            {
                var userContactManager = new BaseUserContactManager();
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserContactEntity.FieldEmail, email)
                };
                var entity = userContactManager.GetEntity(parameters);
                if (entity != null)
                {
                    parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BaseUserEntity.FieldId, entity.UserId),
                        new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0),
                        new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1)
                    };
                    var dt = GetDataTable(parameters);
                    if (dt != null && dt.Rows.Count == 1)
                    {
                        userEntity = BaseEntity.Create<BaseUserEntity>(dt);
                    }
                }
            }

            return userEntity;
        }
        #endregion

        #region public override string GetIdByCode(string userCode)
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userCode">用户编号</param>
        /// <returns>主键</returns>
        public override string GetIdByCode(string userCode)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserEntity.FieldCode, userCode),
                new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
            };
            return GetId(parameters);
        }
        #endregion

        #region public string GetIdByUserName(string userName)
        /// <summary>
        /// 按用户名获取用户主键
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>主键</returns>
        public string GetIdByUserName(string userName)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserEntity.FieldUserName, userName),
                new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
            };
            return GetId(parameters);
        }
        #endregion

        #region public static string GetRealNameByCache(string id) 通过主键获取姓名
        /// <summary>
        /// 通过主键获取姓名
        /// 这里是进行了内存缓存处理，减少数据库的I/O处理，提高程序的运行性能，
        /// 若有数据修改过，重新启动一下程序就可以了，这些基础数据也不是天天修改来修改去的，
        /// 所以没必要过度担忧，当然有需要时也可以写个刷新缓存的程序
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>显示值</returns>
        public static string GetRealNameByCache(string id)
        {
            var result = string.Empty;

            var entity = GetEntityByCache(id);
            if (entity != null)
            {
                result = entity.RealName;
            }

            return result;
        }
        #endregion

        #region public static string GetUserCodeByCache(string id) 通过主键获取编号
        /// <summary>
        /// 通过主键获取编号
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>显示值</returns>
        public static string GetUserCodeByCache(string id)
        {
            var result = string.Empty;

            var entity = GetEntityByCache(id);
            if (entity != null)
            {
                result = entity.Code;
            }

            return result;
        }
        #endregion

        #region public static string GetDepartmentNameByCache(string id) 通过主键获取部门名称
        /// <summary>
        /// 通过主键获取部门名称
        /// 这里是进行了内存缓存处理，减少数据库的I/O处理，提高程序的运行性能，
        /// 若有数据修改过，重新启动一下程序就可以了，这些基础数据也不是天天修改来修改去的，
        /// 所以没必要过度担忧，当然有需要时也可以写个刷新缓存的程序
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>显示值</returns>
        public static string GetDepartmentNameByCache(string id)
        {
            var result = string.Empty;

            var entity = GetEntityByCache(id);
            if (entity != null)
            {
                result = entity.DepartmentName;
            }

            return result;
        }
        #endregion

        #region public static string GetCompanyIdByCache(string id)
        /// <summary>
        /// 通过主键获取公司主键
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>显示值</returns>
        public static string GetCompanyIdByCache(string id)
        {
            var result = string.Empty;

            var entity = GetEntityByCache(id);
            if (entity != null)
            {
                result = entity.CompanyId.ToString();
            }

            return result;
        }
        #endregion

        #region public static string GetCompanyNameByCache(string id)
        /// <summary>
        /// 通过主键获取公司名称
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>显示值</returns>
        public static string GetCompanyNameByCache(string id)
        {
            var result = string.Empty;

            var entity = GetEntityByCache(id);
            if (entity != null)
            {
                result = entity.CompanyName;
            }

            return result;
        }
        #endregion

        #region public bool IsAdministrator(BaseUserEntity entity)
        /// <summary>
        /// 是否为管理员
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsAdministrator(BaseUserEntity entity)
        {
            // 用户是超级管理员
            //if (entity.Id.Equals("Administrator"))
            //{
            //    return true;
            //}
            //if (!string.IsNullOrEmpty(entity.Code) && entity.Code.Equals("Administrator", StringComparison.Ordinal))
            //{
            //    return true;
            //}
            //if (!string.IsNullOrEmpty(entity.UserName) && entity.UserName.Equals("Administrator", StringComparison.Ordinal))
            //{
            //    return true;
            //}
            //if (!string.IsNullOrEmpty(entity.NickName) && entity.NickName.Equals("Administrator", StringComparison.Ordinal))
            //{
            //    return true;
            //}
            //if (!string.IsNullOrEmpty(entity.RealName) && entity.RealName.Equals("Administrator", StringComparison.Ordinal))
            //{
            //    return true;
            //}

            // 不能获取当前操作元信息时，认为不是管理员
            if (entity != null)
            {
                // 用效率更高的方式进行判断，减少数据的输入输出
                if (IsAdministrator(entity.Id.ToString()))
                {
                    return true;
                }
                /*
                // 用户在超级管理员群里
                List<BaseRoleEntity> roleList = GetRoleList(entity.Id);
                foreach (var roleEntity in roleList)
                {
                    if (roleEntity.Id.ToString().Equals(DefaultRole.Administrators.ToString()))
                    {
                        return true;
                    }
                    if (!string.IsNullOrEmpty(roleEntity.Code) && roleEntity.Code.Equals(DefaultRole.Administrators.ToString()))
                    {
                        return true;
                    }
                    if (!string.IsNullOrEmpty(roleEntity.RealName) && roleEntity.RealName.Equals(DefaultRole.Administrators.ToString()))
                    {
                        return true;
                    }
                }
                */
            }
            return false;
        }

        #endregion

        #region public bool IsAdministratorById(string userId)

        /// <summary>
        /// 指定编号用户是否为管理员
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsAdministratorById(string userId)
        {
            var entity = GetEntity(userId);
            return IsAdministrator(entity);
        }
        #endregion

        #region public string GetUsersName(string userIdList)
        /// <summary>
        /// 获取用户名
        /// </summary>
        /// <param name="userIdList"></param>
        /// <returns></returns>
        public string GetUsersName(string userIdList)
        {
            var userRealNames = string.Empty;
            var ids = userIdList.Split(',').Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
            BaseUserEntity entity = null;
            foreach (var id in ids)
            {
                entity = GetEntity(id);
                if (entity != null && !string.IsNullOrEmpty(entity.RealName))
                {
                    userRealNames += "," + entity.RealName;
                }
            }
            if (!string.IsNullOrEmpty(userRealNames))
            {
                userRealNames = userRealNames.Substring(1);
            }
            return userRealNames;
        }
        #endregion

        #region public List<BaseUserEntity> GetListByManager(string managerUserId)
        /// <summary>
        /// 按上级主管获取下属用户列表
        /// </summary>
        /// <param name="managerUserId">主管主键</param>
        /// <returns>用户列表</returns>
        public List<BaseUserEntity> GetListByManager(string managerUserId)
        {
            var dt = GetChildrens(BaseUserEntity.FieldId, managerUserId, BaseUserEntity.FieldManagerUserId, BaseUserEntity.FieldSortCode);
            return BaseEntity.GetList<BaseUserEntity>(dt);
        }
        #endregion

        #region public string[] GetIdsByManager(string managerUserId)

        /// <summary>
        /// 按上级主管获取下属用户主键数组
        /// </summary>
        /// <param name="managerUserId">主管主键</param>
        /// <returns>用户主键数组</returns>
        public string[] GetIdsByManager(string managerUserId)
        {
            return GetChildrensId(BaseUserEntity.FieldId, managerUserId, BaseUserEntity.FieldManagerUserId);
        }
        #endregion

        #region public BaseUserInfo AccountActivation(string openId)
        /// <summary>
        /// 激活帐户
        /// </summary>
        /// <param name="openId">唯一识别码</param>
        /// <returns>用户实体</returns>
        public BaseUserInfo AccountActivation(string openId)
        {
            // 1.用户是否存在？
            BaseUserInfo userInfo = null;
            // 用户没有找到状态
            Status = Status.UserNotFound;
            StatusCode = Status.UserNotFound.ToString();
            // 检查是否有效的合法的参数
            if (!string.IsNullOrEmpty(openId))
            {
                var manager = new BaseUserManager(DbHelper);
                var parameters = new List<KeyValuePair<string, object>>
                {
                    // parameters.Add(new KeyValuePair<string, object>(BaseUserEntity.FieldOpenId, openId));
                    new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
                };
                var dt = manager.GetDataTable(parameters);
                if (dt != null && dt.Rows.Count == 1)
                {
                    var entity = BaseEntity.Create<BaseUserEntity>(dt);
                    // 3.用户是否被锁定？
                    if (entity.Enabled == 0)
                    {
                        Status = Status.UserLocked;
                        StatusCode = Status.UserLocked.ToString();
                        return userInfo;
                    }
                    if (entity.Enabled == 1)
                    {
                        // 2.用户是否已经被激活？
                        Status = Status.UserIsActivate;
                        StatusCode = Status.UserIsActivate.ToString();
                        return userInfo;
                    }
                    if (entity.Enabled == -1)
                    {
                        // 4.成功激活用户
                        Status = Status.Ok;
                        StatusCode = Status.Ok.ToString();
                        manager.Update(new KeyValuePair<string, object>(BaseUserEntity.FieldId, entity.Id), new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1));
                        return userInfo;
                    }
                }
            }
            return userInfo;
        }
        #endregion

        #region public BaseUserInfo Impersonation(string id) 扮演用户

        /// <summary>
        /// 扮演用户
        /// </summary>
        /// <param name="id">用户主键</param>
        /// <param name="status">状态</param>
        /// <returns>用户类</returns>
        public BaseUserInfo Impersonation(int id, out Status status)
        {
            BaseUserInfo userInfo = null;
            // 获得登录信息
            var entity = new BaseUserLogonManager(DbHelper, UserInfo).GetEntityByUserId(id.ToString());
            // 只允许登录一次，需要检查是否自己重新登录了，或者自己扮演自己了
            if (!UserInfo.Id.Equals(id))
            {
                if (BaseSystemInfo.CheckOnline)
                {
                    if (entity.UserOnline > 0)
                    {
                        status = Status.ErrorOnline;
                        return userInfo;
                    }
                }
            }

            var userEntity = GetEntity(id);
            userInfo = ConvertToUserInfo(userEntity);
            if (userEntity.IsStaff.Equals("1"))
            {
                // 获得员工的信息
                var staffEntity = new BaseStaffEntity();
                var staffManager = new BaseStaffManager(DbHelper, UserInfo);
                var dataTableStaff = staffManager.GetDataTableById(id.ToString());
                staffEntity.GetSingle(dataTableStaff);
                userInfo = staffManager.ConvertToUserInfo(userInfo, staffEntity);
            }
            status = Status.Ok;
            // 登录、重新登录、扮演时的在线状态进行更新
            var userLogonManager = new BaseUserLogonManager(DbHelper, UserInfo);
            userLogonManager.ChangeOnline(id);
            return userInfo;
        }
        #endregion

        #region public int CheckUserStaff()
        /// <summary>
        /// 用户已经被删除的员工的UserId设置为NULL，说白了，是需要整理数据
        /// </summary>
        /// <returns>影响行数</returns>
        public int CheckUserStaff()
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("UPDATE BaseStaff SET UserId = NULL WHERE UserId NOT IN ( SELECT Id FROM " + BaseUserEntity.CurrentTableName + " WHERE " + BaseStaffEntity.FieldDeleted + " = 0 ) ");
            return ExecuteNonQuery(sb.Return());
        }
        #endregion

        #region public string GetCount(string companyId = null)

        /// <summary>
        /// 获取人数
        /// </summary>
        public string GetCount(string companyId = null)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) AS UserCount FROM " + CurrentTableName + " WHERE " + BaseUserEntity.FieldDeleted + " = 0 AND " + BaseUserEntity.FieldEnabled + " = 1");

            if (ValidateUtil.IsInt(companyId))
            {
                sb.Append(" AND " + BaseUserEntity.FieldCompanyId + " = " + companyId);
            }

            return DbHelper.ExecuteScalar(sb.Return()).ToInt().ToString();
        }

        #endregion

        #region GetRegistrationCount

        /// <summary>
        /// 获取注册用户数
        /// </summary>
        /// <param name="days">最近多少天</param>
        /// <param name="currentWeek">当周</param>
        /// <param name="currentMonth">当月</param>
        /// <param name="currentQuarter">当季</param>
        /// <param name="currentYear">当年</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public int GetRegistrationCount(int days = 0, bool currentWeek = false, bool currentMonth = false, bool currentQuarter = false, bool currentYear = false, string startTime = null, string endTime = null)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) AS UserCount FROM " + CurrentTableName + " WHERE " + BaseUserEntity.FieldEnabled + " = 1 AND " + BaseUserEntity.FieldDeleted + " = 0");
            if (days > 0)
            {
                sb.Append(" AND (DATEADD(d, " + days + ", " + BaseUserEntity.FieldCreateTime + ") > " + DbHelper.GetDbNow() + ")");
            }
            if (currentWeek)
            {
                sb.Append(" AND DATEDIFF(ww," + BaseUserEntity.FieldCreateTime + "," + DbHelper.GetDbNow() + ") = 0");
            }
            if (currentMonth)
            {
                sb.Append(" AND DATEDIFF(mm," + BaseUserEntity.FieldCreateTime + "," + DbHelper.GetDbNow() + ") = 0");
            }
            if (currentQuarter)
            {
                sb.Append(" AND DATEDIFF(qq," + BaseUserEntity.FieldCreateTime + "," + DbHelper.GetDbNow() + ") = 0");
            }
            if (currentYear)
            {
                sb.Append(" AND DATEDIFF(yy," + BaseUserEntity.FieldCreateTime + "," + DbHelper.GetDbNow() + ") = 0");
            }
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseUserEntity.FieldCreateTime + " >= " + startTime + ")");
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseUserEntity.FieldCreateTime + " < " + endTime + ")");
            }
            return DbHelper.ExecuteScalar(sb.Return()).ToInt();
        }

        #endregion

        #region public override int BatchSave(DataTable result) 批量保存
        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns>影响行数</returns>
        public override int BatchSave(DataTable dt)
        {
            var result = 0;
            var userEntity = new BaseUserEntity();
            foreach (DataRow dr in dt.Rows)
            {
                // 删除状态
                if (dr.RowState == DataRowState.Deleted)
                {
                    var id = dr[BaseUserEntity.FieldId, DataRowVersion.Original].ToString();
                    if (id.Length > 0)
                    {
                        result += Delete(id);
                    }
                }
                // 被修改过
                if (dr.RowState == DataRowState.Modified)
                {
                    var id = dr[BaseUserEntity.FieldId, DataRowVersion.Original].ToString();
                    if (!string.IsNullOrEmpty(id))
                    {
                        userEntity.GetFrom(dr);
                        result += UpdateEntity(userEntity);
                    }
                }
                // 添加状态
                if (dr.RowState == DataRowState.Added)
                {
                    userEntity.GetFrom(dr);
                    result += AddEntity(userEntity).Length > 0 ? 1 : 0;
                }
                if (dr.RowState == DataRowState.Unchanged)
                {
                    continue;
                }
                if (dr.RowState == DataRowState.Detached)
                {
                    continue;
                }
            }
            return result;
        }
        #endregion

        #region public int GetSortNum(string userId)
        /// <summary>
        /// 取得排名
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetSortNum(string userId)
        {
            var entity = GetEntity(userId);
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) AS UserCount "
                            + " FROM " + CurrentTableName
                            + " INNER JOIN " + BaseStaffEntity.CurrentTableName + " ON " + BaseStaffEntity.CurrentTableName + ".Id = " + CurrentTableName + ".Id"
                            + " WHERE " + CurrentTableName + "." + BaseStaffEntity.FieldDeleted + " = 0 AND " + CurrentTableName + ".Enabled = 1 and " + CurrentTableName + "." + BaseUserEntity.FieldGender + " IS NOT NULL AND " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldCurrentProvince + " IS NOT NULL AND (" + CurrentTableName + "." + BaseUserEntity.FieldScore
                            + " > " + entity.Score + " OR (" + CurrentTableName + "."
                            + BaseUserEntity.FieldSortCode + " < " + entity.SortCode + " AND " + CurrentTableName + "." + BaseUserEntity.FieldScore
                            + " = " + entity.Score + "))");
            return DbHelper.ExecuteScalar(sb.Return()).ToInt() + 1;
        }
        #endregion

        #region public int GetPinYin()
        /// <summary>
        /// 获取拼音
        /// </summary>
        /// <returns></returns>
        public int GetPinYin()
        {
            var result = 0;
            var list = GetList<BaseUserEntity>();
            foreach (var entity in list)
            {
                if (string.IsNullOrEmpty(entity.QuickQuery))
                {
                    // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
                    entity.QuickQuery = StringUtil.GetPinyin(entity.RealName).ToLower();
                }
                if (string.IsNullOrEmpty(entity.SimpleSpelling))
                {
                    // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
                    entity.SimpleSpelling = StringUtil.GetSimpleSpelling(entity.RealName).ToLower();
                }
                result += UpdateEntity(entity);
            }
            return result;
        }
        #endregion

        #region public static string GetNames(List<BaseUserEntity> list)
        /// <summary>
        /// 获取名字
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GetNames(List<BaseUserEntity> list)
        {
            var result = string.Empty;

            foreach (var entity in list)
            {
                result += "," + entity.RealName;
            }
            if (!string.IsNullOrEmpty(result))
            {
                result = result.Substring(1);
            }

            return result;
        }
        #endregion

        #region public static BaseUserEntity SetCache(string id)
        /// <summary>
        /// 重新设置缓存（重新强制设置缓存）可以提供外部调用的
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>用户信息</returns>
        public static BaseUserEntity SetCache(string id)
        {
            BaseUserEntity result = null;

            var manager = new BaseUserManager();
            result = manager.GetEntity(id);

            if (result != null)
            {
                SetCache(result);
            }

            return result;
        }
        #endregion

        #region public static int CachePreheating()
        /// <summary>
        /// 缓存预热,强制重新缓存
        /// </summary>
        /// <returns>影响行数</returns>
        public static int CachePreheating()
        {
            var result = 0;

            // 把所有的数据都缓存起来的代码
            var manager = new BaseUserManager();
            var dataReader = manager.ExecuteReader(0, BaseUserEntity.FieldId);
            if (dataReader != null && !dataReader.IsClosed)
            {
                while (dataReader.Read())
                {
                    var entity = BaseEntity.Create<BaseUserEntity>(dataReader, false);
                    SetCache(entity);
                    result++;
                }

                dataReader.Close();
            }

            return result;
        }
        #endregion
    }
}