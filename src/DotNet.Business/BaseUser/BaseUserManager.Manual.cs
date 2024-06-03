//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------
using System.Data;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using System;
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
                sb.Append(" AND " + BaseUserEntity.FieldEnabled + "  = 0 ");
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
                sb.Append(" AND " + BaseUserEntity.FieldDeleted + "  = 0 ");
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
            var tableNameUserRole = UserInfo.SystemCode + "UserRole";
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableNameUserRole = systemCode + "UserRole";
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
            var tableNamePermission = UserInfo.SystemCode + "Permission";
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableNamePermission = systemCode + "Permission";
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

            sb.Replace(" 1 = 1 AND ", "");
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
            var tableNameUserRole = systemCode + "UserRole";
            var tableNameRole = systemCode + "Role";
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + SelectFields + " FROM " + BaseUserEntity.CurrentTableName
                            + " WHERE " + BaseUserEntity.FieldEnabled + " = 1 "
                            + " AND " + BaseUserEntity.FieldDeleted + "= 0 "
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
        /// <param name="dbHelper">数据库连接</param>
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
    }
}