//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseRoleManager 
    /// 角色管理
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
    public partial class BaseRoleManager : BaseManager
    {
        #region 高级查询

        /// <summary>
        /// 按条件分页查询(带记录状态Enabled和删除状态Deleted)
        /// </summary>
        /// <param name="systemCode">子系统</param>
        /// <param name="categoryCode">分类编码</param>
        /// <param name="userId">指定用户</param>
        /// <param name="userIdExcluded">排除指定用户</param>
        /// <param name="moduleId">指定菜单模块</param>
        /// <param name="moduleIdExcluded">排除指定菜单模块</param>
        /// <param name="showInvisible">显示隐藏</param>
        /// <param name="codePrefix">前缀</param>
        /// <param name="codePrefixExcluded"></param>
        /// <param name="searchKey">查询字段</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="showDisabled">是否显示无效记录</param>
        /// <param name="showDeleted">是否显示已删除记录</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string systemCode, string categoryCode, string userId, string userIdExcluded, string moduleId, string moduleIdExcluded, bool showInvisible, string codePrefix, string codePrefixExcluded, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = "CreateTime", string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            //角色表名
            var tableNameRole = BaseRoleEntity.CurrentTableName;
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableNameRole = systemCode + "Role";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(UserInfo.SystemCode))
                {
                    tableNameRole = UserInfo.SystemCode + "Role";
                }
            }
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseRoleEntity.FieldEnabled + "  = 1 ");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseRoleEntity.FieldDeleted + "  = 0 ");
            }
            //是否显示已隐藏记录
            if (!showInvisible)
            {
                sb.Append(" AND " + BaseRoleEntity.FieldIsVisible + "  = 1 ");
            }
            //角色分类
            if (!string.IsNullOrEmpty(categoryCode))
            {
                sb.Append(" AND " + BaseRoleEntity.FieldCategoryCode + " = N'" + categoryCode + "'");
            }
            //用户角色
            var tableNameUserRole = UserInfo.SystemCode + "UserRole";
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableNameUserRole = systemCode + "UserRole";
            }
            //指定用户
            if (ValidateUtil.IsInt(userId))
            {
                sb.Append(" AND ( " + BaseRoleEntity.FieldId + " IN ");
                sb.Append(" (SELECT DISTINCT " + BaseUserRoleEntity.FieldRoleId);
                sb.Append(" FROM " + tableNameUserRole);
                sb.Append(" WHERE " + BaseUserRoleEntity.FieldUserId + " = " + userId + "");
                sb.Append(" AND " + BaseUserRoleEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BaseUserRoleEntity.FieldDeleted + " = 0)) ");
            }
            //排除指定用户
            if (ValidateUtil.IsInt(userIdExcluded))
            {
                sb.Append(" AND ( " + BaseRoleEntity.FieldId + " NOT IN ");
                sb.Append(" (SELECT DISTINCT " + BaseUserRoleEntity.FieldRoleId);
                sb.Append(" FROM " + tableNameUserRole);
                sb.Append(" WHERE " + BaseUserRoleEntity.FieldUserId + " = " + userIdExcluded + "");
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
                sb.Append(" AND ( " + BaseRoleEntity.FieldId + " IN ");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldResourceId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldPermissionId + " = '" + moduleId + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameRole + "' ");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0)) ");
            }
            //排除指定菜单模块
            if (ValidateUtil.IsInt(moduleIdExcluded))
            {
                sb.Append(" AND ( " + BaseRoleEntity.FieldId + " NOT IN ");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldResourceId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldPermissionId + " = '" + moduleIdExcluded + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameRole + "' ");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0)) ");
            }
            //前缀
            if (!string.IsNullOrEmpty(codePrefix))
            {
                codePrefix = dbHelper.SqlSafe(codePrefix);
                sb.Append(" AND  " + BaseRoleEntity.FieldCode + " LIKE N'" + codePrefix + "%'");
            }
            //排除前缀
            if (!string.IsNullOrEmpty(codePrefixExcluded))
            {
                codePrefixExcluded = dbHelper.SqlSafe(codePrefixExcluded);
                sb.Append(" AND  " + BaseRoleEntity.FieldCode + " NOT LIKE N'" + codePrefixExcluded + "%'");
            }
            //关键词
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseRoleEntity.FieldRealName + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseRoleEntity.FieldCode + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseRoleEntity.FieldDescription + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            //重新构造viewName
            var sbView = Pool.StringBuilder.Get();            
            //指定用户，就读取相应的UserRole授权日期
            if (ValidateUtil.IsInt(userId))
            {
                sbView.Append("SELECT DISTINCT " + tableNameRole + "." + BaseRoleEntity.FieldId);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldOrganizationId);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldCategoryCode);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldCode);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldRealName);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldAllowEdit);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldAllowDelete);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldIsVisible);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldSortCode);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldDeleted);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldEnabled);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldDescription);
                //授权日期
                sbView.Append("," + tableNameUserRole + "." + BaseUserRoleEntity.FieldCreateTime);
                sbView.Append("," + tableNameUserRole + "." + BaseUserRoleEntity.FieldCreateUserId);
                sbView.Append("," + tableNameUserRole + "." + BaseUserRoleEntity.FieldCreateBy);
                sbView.Append("," + tableNameUserRole + "." + BaseUserRoleEntity.FieldUpdateTime);
                sbView.Append("," + tableNameUserRole + "." + BaseUserRoleEntity.FieldUpdateUserId);
                sbView.Append("," + tableNameUserRole + "." + BaseUserRoleEntity.FieldUpdateBy);
                sbView.Append(" FROM " + tableNameRole + " INNER JOIN " + tableNameUserRole);
                sbView.Append(" ON " + tableNameRole + "." + BaseRoleEntity.FieldId + " = " + tableNameUserRole + "." + BaseUserRoleEntity.FieldRoleId);
                sbView.Append(" WHERE (" + tableNameUserRole + "." + BaseUserRoleEntity.FieldUserId + " = " + userId + ")");
            }
            //指定菜单模块，就读取相应的Permission授权日期
            else if (ValidateUtil.IsInt(moduleId))
            {
                sbView.Append("SELECT DISTINCT " + tableNameRole + "." + BaseRoleEntity.FieldId);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldOrganizationId);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldCategoryCode);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldCode);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldRealName);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldAllowEdit);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldAllowDelete);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldIsVisible);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldSortCode);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldDeleted);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldEnabled);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldDescription);
                //授权日期
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldCreateTime);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldCreateUserId);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldCreateBy);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateTime);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateUserId);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateBy);
                sbView.Append(" FROM " + tableNameRole + " INNER JOIN " + tableNamePermission);
                sbView.Append(" ON " + tableNameRole + "." + BaseRoleEntity.FieldId + " = " + tableNamePermission + "." + BasePermissionEntity.FieldResourceId);
                sbView.Append(" WHERE (" + tableNamePermission + "." + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameRole + "')");
                sbView.Append(" AND (" + tableNamePermission + "." + BasePermissionEntity.FieldPermissionId + " = " + moduleId + ")");
            }
            else
            {
                sbView.Append(tableNameRole);
            }

            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, sbView.Put(), sb.Put(), null, "*");
        }
        #endregion
    }
}
