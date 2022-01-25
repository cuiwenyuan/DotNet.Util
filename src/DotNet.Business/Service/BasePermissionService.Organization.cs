//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Reflection;

namespace DotNet.Business
{
    using IService;
    using Util;

    /// <summary>
    /// BasePermissionService
    /// 权限判断服务
    /// 
    /// 修改记录
    /// 
    ///		2012.03.22 版本：1.0 JiRiGaLa 创建权限判断服务。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.03.22</date>
    /// </author> 
    /// </summary>
    public partial class BasePermissionService : IBasePermissionService
    {

        #region 用户组织机构范围权限（省市县区域）关联相关

        /// <summary>
        /// 获取用户的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public PermissionOrganizationScope GetUserOrganizationScope(BaseUserInfo userInfo, string userId, string permissionCode)
        {
            var result = PermissionOrganizationScope.OnlyOwnData;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userScopeManager = new BaseUserScopeManager(dbHelper, userInfo);
                var containChild = false;
                result = userScopeManager.GetUserOrganizationScope(userInfo.SystemCode, userId, out containChild, permissionCode);
            });

            return result;
        }

        /// <summary>
        /// 设置用户某个权限的组织机构范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionOrganizationScope">组织机构范围</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public string SetUserOrganizationScope(BaseUserInfo userInfo, string userId, PermissionOrganizationScope permissionOrganizationScope, string permissionCode)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userOrganizationScopeManager = new BaseUserScopeManager(userInfo);
                result = userOrganizationScopeManager.SetUserOrganizationScope(userInfo.SystemCode, userId, permissionOrganizationScope, permissionCode, false);
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
        public PermissionOrganizationScope GetRoleOrganizationScope(BaseUserInfo userInfo, string roleId, string permissionCode)
        {
            var result = PermissionOrganizationScope.OnlyOwnData;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var roleOrganizationScopeManager = new BaseRoleScopeManager(dbHelper, userInfo);
                var containChild = false;
                result = roleOrganizationScopeManager.GetRoleOrganizationScope(userInfo.SystemCode, roleId, out containChild, permissionCode);
            });

            return result;
        }

        /// <summary>
        /// 设置角色某个权限的组织机构范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionOrganizationScope">组织机构范围</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public string SetRoleOrganizationScope(BaseUserInfo userInfo, string roleId, PermissionOrganizationScope permissionOrganizationScope, string permissionCode)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var roleOrganizationScopeManager = new BaseRoleScopeManager(dbHelper, userInfo);
                result = roleOrganizationScopeManager.SetRoleOrganizationScope(userInfo.SystemCode, roleId, permissionOrganizationScope, permissionCode, false);
            });
            return result;
        }

        #endregion

        #endregion
    }
}