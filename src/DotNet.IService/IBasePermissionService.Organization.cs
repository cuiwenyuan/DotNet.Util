//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------



namespace DotNet.IService
{
    using Util;
    
    /// <summary>
    /// IPermissionService
    /// 与权限判断等相关的接口定义
    /// 
    /// 修改记录
    /// 
    ///		2012.03.22 版本：1.0 JiRiGaLa 添加权限。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.03.22</date>
    /// </author> 
    /// </summary>
    public partial interface IBasePermissionService
    {
        //用户组织机构范围权限（省市县区域）关联相关

        /// <summary>
        /// 获取用户的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        PermissionOrganizationScope GetUserOrganizationScope(BaseUserInfo userInfo, string userId, string permissionCode);

        /// <summary>
        /// 设置用户某个权限的组织机构范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionOrganizationScope">组织机构范围</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        string SetUserOrganizationScope(BaseUserInfo userInfo, string userId, PermissionOrganizationScope permissionOrganizationScope, string permissionCode);

        //用户组织机构范围权限（省市县区域）关联相关

        /// <summary>
        /// 获取用户的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        PermissionOrganizationScope GetRoleOrganizationScope(BaseUserInfo userInfo, string roleId, string permissionCode);

        /// <summary>
        /// 设置用户某个权限的组织机构范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionOrganizationScope">组织机构范围</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        string SetRoleOrganizationScope(BaseUserInfo userInfo, string roleId, PermissionOrganizationScope permissionOrganizationScope, string permissionCode);
    }
}