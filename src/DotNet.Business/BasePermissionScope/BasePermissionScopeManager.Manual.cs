//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

using System.Data;
using System.Linq;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BasePermissionScopeManager
    /// 资源权限范围
    ///
    ///     2012.08.06 版本：1.0 JiRiGaLa 主键进行了绝对的优化，这是个好东西啊，平时要多用，用得要灵活些。
    ///     
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.08.06</date>
    /// </author>
    /// </summary>
    public partial class BasePermissionScopeManager : BaseManager
    {
        #region public string[] GetPermissionTreeUserIds(string systemCode, string userId, string permissionCode, string permissionName = null)
        /// <summary>
        /// 获取用户权限树
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionName">权限名称</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>用户主键</returns>
        public string[] GetPermissionTreeUserIds(string systemCode, string userId, string permissionCode, string permissionName = null)
        {
            string[] result = null;
            var permissionScopeTableName = GetPermissionScopeTableName(systemCode);
            var permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);
            if (!string.IsNullOrEmpty(permissionId))
            {
                var sb = PoolUtil.StringBuilder.Get();
                sb.Append("(SELECT ResourceId, TargetId FROM " + permissionScopeTableName + " WHERE " + BasePermissionScopeEntity.FieldEnabled + " = 1 AND " + BasePermissionScopeEntity.FieldDeleted + " = 0 AND SystemCode = '" + systemCode + "' AND ResourceCategory = '" + BaseUserEntity.CurrentTableName + "' AND TargetCategory = '" + BaseUserEntity.CurrentTableName + "' AND PermissionId = " + permissionId + ") T");
                var fieldParentId = "ResourceId"; //"ManagerUserId";
                var fieldId = "TargetId"; // "UserId";
                string order = null;
                var idOnly = true;
                var dt = DbHelper.GetChildrens(sb.Return(), fieldId, userId.ToString(), fieldParentId, order, idOnly);
                result = BaseUtil.FieldToArray(dt, BasePermissionScopeEntity.FieldTargetId);
            }
            return result;
        }

        #endregion

        #region public string[] GetUserIds(string systemCode, string organizationId, string permissionCode, string permissionName = null)

        /// <summary>
        /// 获得有某个权限的所有用户主键
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="organizationId">组织机构主键</param>
        /// <param name="permissionCode">操作权限编号</param>
        /// <param name="permissionName">操作权限名称</param>
        /// <returns>用户主键数组</returns>
        public string[] GetUserIds(string systemCode, string organizationId, string permissionCode, string permissionName = null)
        {
            // 若不存在就需要自动能增加一个操作权限项
            var permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);
            return GetUserIdsByPermissionId(systemCode, organizationId, permissionId);
        }

        #endregion

        #region public string[] GetUserIdsByPermissionId(string systemCode, string organizationId, string permissionId)

        /// <summary>
        /// 获取用户编号数组
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="organizationId"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public string[] GetUserIdsByPermissionId(string systemCode, string organizationId, string permissionId)
        {
            DataTable dt = null;
            string[] result = null;
            if (!string.IsNullOrEmpty(permissionId))
            {
                var permissionScopeTableName = GetPermissionScopeTableName(systemCode);
                var sb = PoolUtil.StringBuilder.Get();

                // 1.本人直接就有某个操作权限的。
                sb.Append("SELECT ResourceId FROM " + permissionScopeTableName + " WHERE (ResourceCategory = 'BaseUser') AND (PermissionId = " + permissionId + ") AND TargetCategory='BaseOrganization' AND TargetId = " + organizationId + " AND " + BasePermissionScopeEntity.FieldDeleted + " = 0 AND " + BasePermissionScopeEntity.FieldEnabled + " = 1 AND " + BasePermissionScopeEntity.FieldSystemCode + " = '" + systemCode + "'");
                dt = Fill(sb.Return());
                var userIds = BaseUtil.FieldToArray(dt, BasePermissionEntity.FieldResourceId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();

                // 2.角色本身就有某个操作权限的。
                sb = PoolUtil.StringBuilder.Get();
                sb.Append("SELECT ResourceId FROM " + permissionScopeTableName + " WHERE (ResourceCategory = '" + GetRoleTableName(systemCode) + "') AND (PermissionId = " + permissionId + ") AND TargetCategory='BaseOrganization' AND TargetId = " + organizationId + " AND " + BasePermissionScopeEntity.FieldDeleted + " = 0 AND " + BasePermissionScopeEntity.FieldEnabled + " = 1 AND " + BasePermissionScopeEntity.FieldSystemCode + " = '" + systemCode + "'");
                dt = Fill(sb.Return());
                var roleIds = StringUtil.Concat(result, BaseUtil.FieldToArray(dt, BasePermissionEntity.FieldResourceId)).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();

                // 3.组织机构有某个操作权限。
                // sb.Append("SELECT ResourceId FROM " + permissionScopeTableName + " WHERE ResourceCategory = '" + GetRoleOrganizationTableName(systemCode) + "' AND PermissionId = " + result + " AND " + BaseContactEntity.FieldDeleted + " = 0 AND " + BasePermissionScopeEntity.FieldEnabled + " = 1 AND " + BasePermissionScopeEntity.FieldSystemCode + " = '" + systemCode + "'");
                // result = this.Fill(sb.Return());
                // string[] ids = StringUtil.Concat(result, BaseUtil.FieldToArray(result, BasePermissionEntity.FieldResourceId)).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();

                // 4.获取所有有这个操作权限的用户Id，而且这些用户是有效的。
                var userManager = new BaseUserManager(UserInfo);
                result = userManager.GetUserIds(userIds, null, roleIds);
            }
            return result;
        }

        #endregion
    }
}