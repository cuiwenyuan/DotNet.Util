//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------



namespace DotNet.IService
{
    using Util;

    /// <summary>
    /// IBasePermissionService
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
        //角色权限关联关系相关

        /// <summary>
        /// 20.获取角色拥有的操作权限主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>主键数组</returns>
        string[] GetRolePermissionIds(BaseUserInfo userInfo, string roleId);

        /// <summary>
        /// 20.获取角色主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="permissionId">操作权限主键</param>
        /// <returns>主键数组</returns>
        string[] GetRoleIdsByPermission(BaseUserInfo userInfo, string permissionId);

        /// <summary>
        /// 21.授予角色的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleIds">角色主键数组</param>
        /// <param name="grantPermissionIds">授予权限数组</param>
        /// <returns>影响的行数</returns>
        int GrantRolePermissions(BaseUserInfo userInfo, string[] roleIds, string[] grantPermissionIds);

        /// <summary>
        /// 23.授予角色的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="grantPermissionId">授予权限</param>
        /// <returns>影响的行数</returns>
        string GrantRolePermissionById(BaseUserInfo userInfo, string roleId, string grantPermissionId);

        /// <summary>
        /// 24.撤消角色的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleIds">角色主键数组</param>
        /// <param name="revokePermissionIds">撤消权限数组</param>
        /// <returns>影响的行数</returns>
        int RevokeRolePermissions(BaseUserInfo userInfo, string[] roleIds, string[] revokePermissionIds);

        /// <summary>
        /// 26.撤消角色的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="revokePermissionId">撤消权限数组</param>
        /// <returns>影响的行数</returns>
        int RevokeRolePermissionById(BaseUserInfo userInfo, string roleId, string revokePermissionId);
    }
}