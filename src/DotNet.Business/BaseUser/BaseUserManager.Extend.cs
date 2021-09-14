//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

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
    ///		2016.07.24 版本：1.0 Troy.Cui	新增。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.07.24</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        #region 高级查询

        /// <summary>
        /// 按条件分页高级查询(带记录状态Enabled和删除状态DeletionStateCode)
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="organizeId">查看公司主键</param>
        /// <param name="userId">查看用户主键</param>
        /// <param name="roleId"></param>
        /// <param name="roleIdExcluded"></param>
        /// <param name="moduleId"></param>
        /// <param name="moduleIdExcluded"></param>
        /// <param name="showInvisible"></param>
        /// <param name="disabledUserOnly"></param>
        /// <param name="searchKey">查询字段</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="showDisabled">是否显示无效记录</param>
        /// <param name="showDeleted">是否显示已删除记录</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string systemCode, string organizeId, string userId, string roleId, string roleIdExcluded, string moduleId, string moduleIdExcluded, bool showInvisible, bool disabledUserOnly, string searchKey, out int recordCount, int pageIndex = 0, int pageSize = 20, string sortExpression = "CreateOn", string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            //用户表名
            var tableNameUser = BaseUserEntity.TableName;
            //用户登录表名
            var tableNameUserLogOn = BaseUserLogOnEntity.TableName;
            pageIndex++;
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
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

            if (ValidateUtil.IsInt(organizeId) && int.Parse(organizeId) > 0)
            {
                //只选择当前
                //sb.Append(" AND (" + BaseUserEntity.FieldCompanyId + " = " + organizeId + ")";
                //只选择当前和下一级
                //sb.Append(" AND " + BaseUserEntity.FieldDepartmentId 
                //    + " IN ( SELECT " + BaseOrganizeEntity.FieldId 
                //    + " FROM " + BaseOrganizeEntity.TableName 
                //    + " WHERE " + BaseOrganizeEntity.FieldId + " = " + organizeId + " OR " + BaseOrganizeEntity.FieldParentId + " = " + organizeId + ")";

                //所有下级的都列出来
                var organizeManager = new BaseOrganizeManager(UserInfo);
                var ids = organizeManager.GetChildrensId(BaseOrganizeEntity.FieldId, organizeId, BaseOrganizeEntity.FieldParentId);
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
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + BaseUserEntity.TableName + "' ");
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
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + BaseUserEntity.TableName + "' ");
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
                sb.Append(" OR " + BaseUserEntity.FieldCode + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseUserEntity.FieldDescription + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseUserEntity.FieldQuickQuery + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseUserEntity.FieldSimpleSpelling + " LIKE N'%" + searchKey + "%')");
            }

            sb.Replace(" 1 = 1 AND ", "");
            //重新构造viewName
            var viewName = string.Empty;

            viewName = "SELECT DISTINCT " + tableNameUser + "." + BaseUserEntity.FieldId;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldUserFrom;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldUserName;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldRealName;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldNickName;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldCode;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldIdCard;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldQuickQuery;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSimpleSpelling;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldCompanyId;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldCompanyName;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSubCompanyId;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSubCompanyName;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldDepartmentId;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldDepartmentName;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSubDepartmentId;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSubDepartmentName;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldWorkgroupId;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldWorkgroupName;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldWorkCategory;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSecurityLevel;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldTitle;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldDuty;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldLang;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldGender;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldBirthday;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldScore;
            //viewName += "," + tableNameUser + "." + BaseUserEntity.FieldFans;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldHomeAddress;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSignature;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldTheme;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldIsStaff;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldIsVisible;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldProvince;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldCity;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldDistrict;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldAuditStatus;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldEnabled;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldDeleted;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSortCode;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldDescription;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldManagerId;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldIsAdministrator;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldIsCheckBalance;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldManagerAuditStatus;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldManagerAuditDate;
            //用户表
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldCreateTime;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldCreateUserId;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldCreateBy;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldUpdateTime;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldUpdateUserId;
            viewName += "," + tableNameUser + "." + BaseUserEntity.FieldUpdateBy;

            //用户登录表
            viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldAllowStartTime;
            viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldAllowEndTime;
            viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldLockStartDate;
            viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldLockEndDate;
            viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldFirstVisit;
            viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldPreviousVisit;
            viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldLastVisit;
            viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldChangePasswordDate;
            viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldLogOnCount;
            viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldMultiUserLogin;
            viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldUserOnLine;
            //不从用户登录表读取这些字段，从用户表读取即可
            //viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldCreateTime;
            //viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldCreateUserId;
            //viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldCreateBy;
            //viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldUpdateTime;
            //viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldUpdateUserId;
            //viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldUpdateBy;

            viewName += " FROM " + tableNameUser + " INNER JOIN " + tableNameUserLogOn;
            viewName += " ON " + tableNameUser + "." + BaseUserEntity.FieldId + " = " + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldId;

            //指定角色，就读取相应的UserRole授权日期
            if (ValidateUtil.IsInt(roleId))
            {
                viewName = "SELECT DISTINCT " + tableNameUser + "." + BaseUserEntity.FieldId;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldUserFrom;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldUserName;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldRealName;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldNickName;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldCode;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldIdCard;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldQuickQuery;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSimpleSpelling;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldCompanyId;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldCompanyName;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSubCompanyId;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSubCompanyName;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldDepartmentId;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldDepartmentName;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSubDepartmentId;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSubDepartmentName;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldWorkgroupId;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldWorkgroupName;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldWorkCategory;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSecurityLevel;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldTitle;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldDuty;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldLang;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldGender;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldBirthday;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldScore;
                //viewName += "," + tableNameUser + "." + BaseUserEntity.FieldFans;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldHomeAddress;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSignature;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldTheme;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldIsStaff;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldIsVisible;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldProvince;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldCity;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldDistrict;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldAuditStatus;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldEnabled;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldDeleted;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSortCode;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldDescription;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldManagerId;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldIsAdministrator;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldIsCheckBalance;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldManagerAuditStatus;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldManagerAuditDate;
                //用户登录表
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldAllowStartTime;
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldAllowEndTime;
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldLockStartDate;
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldLockEndDate;
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldFirstVisit;
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldPreviousVisit;
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldLastVisit;
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldChangePasswordDate;
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldLogOnCount;
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldMultiUserLogin;
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldUserOnLine;
                //授权日期
                viewName += "," + tableNameUserRole + "." + BaseUserRoleEntity.FieldCreateTime;
                viewName += "," + tableNameUserRole + "." + BaseUserRoleEntity.FieldCreateUserId;
                viewName += "," + tableNameUserRole + "." + BaseUserRoleEntity.FieldCreateBy;
                viewName += "," + tableNameUserRole + "." + BaseUserRoleEntity.FieldUpdateTime;
                viewName += "," + tableNameUserRole + "." + BaseUserRoleEntity.FieldUpdateUserId;
                viewName += "," + tableNameUserRole + "." + BaseUserRoleEntity.FieldUpdateBy;
                viewName += " FROM " + tableNameUser + " INNER JOIN " + tableNameUserRole;
                viewName += " ON " + tableNameUser + "." + BaseUserEntity.FieldId + " = " + tableNameUserRole + "." + BaseUserRoleEntity.FieldUserId;
                viewName += " INNER JOIN " + tableNameUserLogOn;
                viewName += " ON " + tableNameUser + "." + BaseUserEntity.FieldId + " = " + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldId;
                viewName += " WHERE (" + tableNameUserRole + "." + BaseUserRoleEntity.FieldRoleId + " = " + roleId + ")";
            }
            //指定菜单模块，就读取相应的Permission授权日期
            else if (ValidateUtil.IsInt(moduleId))
            {
                viewName = "SELECT DISTINCT " + tableNameUser + "." + BaseUserEntity.FieldId;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldUserFrom;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldUserName;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldRealName;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldNickName;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldCode;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldIdCard;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldQuickQuery;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSimpleSpelling;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldCompanyId;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldCompanyName;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSubCompanyId;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSubCompanyName;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldDepartmentId;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldDepartmentName;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSubDepartmentId;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSubDepartmentName;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldWorkgroupId;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldWorkgroupName;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldWorkCategory;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSecurityLevel;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldTitle;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldDuty;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldLang;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldGender;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldBirthday;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldScore;
                //viewName += "," + tableNameUser + "." + BaseUserEntity.FieldFans;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldHomeAddress;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSignature;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldTheme;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldIsStaff;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldIsVisible;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldProvince;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldCity;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldDistrict;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldAuditStatus;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldEnabled;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldDeleted;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldSortCode;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldDescription;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldManagerId;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldIsAdministrator;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldIsCheckBalance;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldManagerAuditStatus;
                viewName += "," + tableNameUser + "." + BaseUserEntity.FieldManagerAuditDate;
                //用户登录表
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldAllowStartTime;
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldAllowEndTime;
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldLockStartDate;
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldLockEndDate;
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldFirstVisit;
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldPreviousVisit;
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldLastVisit;
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldChangePasswordDate;
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldLogOnCount;
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldMultiUserLogin;
                viewName += "," + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldUserOnLine;
                //授权日期
                viewName += "," + tableNamePermission + "." + BasePermissionEntity.FieldCreateTime;
                viewName += "," + tableNamePermission + "." + BasePermissionEntity.FieldCreateUserId;
                viewName += "," + tableNamePermission + "." + BasePermissionEntity.FieldCreateBy;
                viewName += "," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateTime;
                viewName += "," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateUserId;
                viewName += "," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateBy;
                viewName += " FROM " + tableNameUser + " INNER JOIN " + tableNamePermission;
                viewName += " ON " + tableNameUser + "." + BaseUserEntity.FieldId + " = " + tableNamePermission + "." + BasePermissionEntity.FieldResourceId;
                viewName += " INNER JOIN " + tableNameUserLogOn;
                viewName += " ON " + tableNameUser + "." + BaseUserEntity.FieldId + " = " + tableNameUserLogOn + "." + BaseUserLogOnEntity.FieldId;
                viewName += " WHERE (" + tableNamePermission + "." + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameUser + "')";
                viewName += " AND (" + tableNamePermission + "." + BasePermissionEntity.FieldPermissionId + " = " + moduleId + ")";
            }
            //从视图读取
            if (!string.IsNullOrEmpty(viewName))
            {
                tableNameUser = viewName;
            }

            return GetDataTableByPage(out recordCount, pageIndex, pageSize, sortExpression, sortDirection, tableNameUser, sb.Put(), null, "*");
        }
        #endregion
    }
}