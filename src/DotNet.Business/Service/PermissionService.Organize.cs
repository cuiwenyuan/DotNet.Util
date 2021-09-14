//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Reflection;

namespace DotNet.Business
{
    using IService;
    using Util;

    /// <summary>
    /// PermissionService
    /// 权限判断服务
    /// 
    /// 修改记录
    /// 
    ///		2012.03.22 版本：1.0 JiRiGaLa 创建权限判断服务。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2012.03.22</date>
    /// </author> 
    /// </summary>
    public partial class PermissionService : IPermissionService
    {
        #region 组织机构权限关联关系相关

        #region public string[] GetOrganizePermissionIds(BaseUserInfo userInfo, string organizeId)
        /// <summary>
        /// 获取组织机构权限主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizeId">组织机构主键</param>
        /// <returns>主键数组</returns>
        public string[] GetOrganizePermissionIds(BaseUserInfo userInfo, string organizeId)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Permission";
                var manager = new BaseOrganizePermissionManager(dbHelper, userInfo, tableName);
                result = manager.GetPermissionIds(organizeId);
            });
            return result;
        }
        #endregion

        #region public string[] GetOrganizeIdsByPermission(BaseUserInfo userInfo, string result)
        /// <summary>
        /// 获取组织机构主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="permissionId">操作权限主键</param>
        /// <returns>主键数组</returns>
        public string[] GetOrganizeIdsByPermission(BaseUserInfo userInfo, string permissionId)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // BaseLogManager.Instance.Add(result, this.serviceName, MethodBase.GetCurrentMethod());
                var tableName = userInfo.SystemCode + "Permission";
                var manager = new BaseOrganizePermissionManager(dbHelper, userInfo, tableName);
                result = manager.GetOrganizeIds(permissionId);
            });
            return result;
        }
        #endregion

        #region public int GrantOrganizePermissions(BaseUserInfo userInfo, string[] ids, string[] grantPermissionIds)
        /// <summary>
        /// 授予组织机构的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizeIds">组织机构主键数组</param>
        /// <param name="grantPermissionIds">授予权限数组</param>
        /// <returns>影响的行数</returns>
        public int GrantOrganizePermissions(BaseUserInfo userInfo, string[] organizeIds, string[] grantPermissionIds)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Permission";
                var manager = new BaseOrganizePermissionManager(dbHelper, userInfo, tableName);
                // 小心异常，检查一下参数的有效性
                if (organizeIds != null && grantPermissionIds != null)
                {
                    result += manager.Grant(userInfo.SystemCode, organizeIds, grantPermissionIds);
                }
            });

            return result;
        }
        #endregion

        #region public string GrantOrganizePermissionById(BaseUserInfo userInfo, string organizeId, string grantPermissionId)
        /// <summary>
        /// 授予组织机构的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizeId">组织机构主键</param>
        /// <param name="grantPermissionId">授予权限数组</param>
        /// <returns>影响的行数</returns>
        public string GrantOrganizePermissionById(BaseUserInfo userInfo, string organizeId, string grantPermissionId)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Permission";
                var manager = new BaseOrganizePermissionManager(dbHelper, userInfo, tableName);
                // 小心异常，检查一下参数的有效性
                if (grantPermissionId != null)
                {
                    result = manager.Grant(userInfo.SystemCode, organizeId, grantPermissionId);
                }
            });

            return result;
        }
        #endregion

        #region public int RevokeOrganizePermissions(BaseUserInfo userInfo, string[] ids, string[] revokePermissionIds)
        /// <summary>
        /// 撤消组织机构的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizeIds">授予权限数组</param>
        /// <param name="revokePermissionIds">撤消权限数组</param>
        /// <returns>影响的行数</returns>
        public int RevokeOrganizePermissions(BaseUserInfo userInfo, string[] organizeIds, string[] revokePermissionIds)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Permission";
                var manager = new BaseOrganizePermissionManager(dbHelper, userInfo, tableName);
                // 小心异常，检查一下参数的有效性
                if (organizeIds != null && revokePermissionIds != null)
                {
                    result += manager.Revoke(userInfo.SystemCode, organizeIds, revokePermissionIds);
                }
            });

            return result;
        }
        #endregion

        #region public int ClearOrganizePermission(BaseUserInfo userInfo, string id)
        /// <summary>
        /// 清除组织机构权限
        /// 
        /// 1.清除组织机构的用户归属。
        /// 2.清除组织机构的模块权限。
        /// 3.清除组织机构的操作权限。
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>数据表</returns>
        public int ClearOrganizePermission(BaseUserInfo userInfo, string id)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Permission";
                var organizePermissionManager = new BaseOrganizePermissionManager(dbHelper, userInfo, tableName);
                result += organizePermissionManager.RevokeAll(id);

                tableName = userInfo.SystemCode + "PermissionScope";
                var organizeScopeManager = new BaseOrganizePermissionScopeManager(dbHelper, userInfo, tableName);
                result += organizeScopeManager.RevokeAll(id);

            });
            return result;
        }
        #endregion

        #region public int RevokeOrganizePermissionById(BaseUserInfo userInfo, string organizeId, string revokePermissionId)
        /// <summary>
        /// 撤消组织机构的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizeId">组织机构主键</param>
        /// <param name="revokePermissionId">撤消权限数组</param>
        /// <returns>影响的行数</returns>
        public int RevokeOrganizePermissionById(BaseUserInfo userInfo, string organizeId, string revokePermissionId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Permission";
                var manager = new BaseOrganizePermissionManager(dbHelper, userInfo, tableName);
                // 小心异常，检查一下参数的有效性
                if (revokePermissionId != null)
                {
                    result += manager.Revoke(userInfo.SystemCode, organizeId, revokePermissionId);
                }
            });
            return result;
        }
        #endregion

        #endregion

        #region 用户组织机构范围权限（省市县区域）关联相关

        /// <summary>
        /// 获取用户的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public PermissionOrganizeScope GetUserOrganizeScope(BaseUserInfo userInfo, string userId, string permissionCode)
        {
            var result = PermissionOrganizeScope.OnlyOwnData;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userScopeManager = new BaseUserScopeManager(dbHelper, userInfo);
                var containChild = false;
                result = userScopeManager.GetUserOrganizeScope(userInfo.SystemCode, userId, out containChild, permissionCode);
            });

            return result;
        }

        /// <summary>
        /// 设置用户某个权限的组织机构范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionOrganizeScope">组织机构范围</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public string SetUserOrganizeScope(BaseUserInfo userInfo, string userId, PermissionOrganizeScope permissionOrganizeScope, string permissionCode)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userOrganizeScopeManager = new BaseUserScopeManager(userInfo);
                result = userOrganizeScopeManager.SetUserOrganizeScope(userInfo.SystemCode, userId, permissionOrganizeScope, permissionCode, false);
            });

            return result;
        }


        #region 角色组织机构范围权限（省市县区域）关联相关

        /// <summary>
        /// 获取角色的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public PermissionOrganizeScope GetRoleOrganizeScope(BaseUserInfo userInfo, string roleId, string permissionCode)
        {
            var result = PermissionOrganizeScope.OnlyOwnData;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var roleOrganizeScopeManager = new BaseRoleScopeManager(dbHelper, userInfo);
                var containChild = false;
                result = roleOrganizeScopeManager.GetRoleOrganizeScope(userInfo.SystemCode, roleId, out containChild, permissionCode);
            });

            return result;
        }

        /// <summary>
        /// 设置角色某个权限的组织机构范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionOrganizeScope">组织机构范围</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public string SetRoleOrganizeScope(BaseUserInfo userInfo, string roleId, PermissionOrganizeScope permissionOrganizeScope, string permissionCode)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var roleOrganizeScopeManager = new BaseRoleScopeManager(dbHelper, userInfo);
                result = roleOrganizeScopeManager.SetRoleOrganizeScope(userInfo.SystemCode, roleId, permissionOrganizeScope, permissionCode, false);
            });
            return result;
        }

        #endregion

        #endregion
    }
}