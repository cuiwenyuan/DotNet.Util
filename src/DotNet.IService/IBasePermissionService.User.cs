//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;


namespace DotNet.IService
{
    using Model;
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
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 用户关联模块关系相关
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        
        string[] GetPermissionIds(BaseUserInfo userInfo, string systemCode, string userId);

        /// <summary>
        /// 59.获得用户有权限的模块
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="fromCache">从缓存读取</param>
        /// <returns>数据表</returns>
        List<BaseModuleEntity> GetPermissionList(BaseUserInfo userInfo, bool fromCache);

        /// <summary>
        /// 60.获得用户有权限的模块
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="fromCache">从缓存读取</param>
        /// <returns>模块列表</returns>
        List<BaseModuleEntity> GetPermissionListByUser(BaseUserInfo userInfo, string systemCode, string userId, string companyId, bool fromCache);

        //用户权限关联关系相关


        /// <summary>
        /// 40.获取用户权限主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <returns>主键数组</returns>
        string[] GetUserPermissionIds(BaseUserInfo userInfo, string userId);

        /// <summary>
        /// 40.获取用户主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="permissionId">操作权限主键</param>
        /// <returns>主键数组</returns>
        string[] GetUserIdsByPermission(BaseUserInfo userInfo, string permissionId);

        /// <summary>
        /// 41.授予用户的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userIds">用户主键数组</param>
        /// <param name="grantPermissionIds">授予权限数组</param>
        /// <returns>影响的行数</returns>
        int GrantUserPermissions(BaseUserInfo userInfo, string[] userIds, string[] grantPermissionIds);

        /// <summary>
        /// 42.授予用户的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantPermissionId">授予权限数组</param>
        /// <returns>影响的行数</returns>
        string GrantUserPermissionById(BaseUserInfo userInfo, string userId, string grantPermissionId);

        /// <summary>
        /// 43.撤消用户的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userIds">用户主键数组</param>
        /// <param name="revokePermissionIds">撤消权限数组</param>
        /// <returns>影响的行数</returns>
        int RevokeUserPermissions(BaseUserInfo userInfo, string[] userIds, string[] revokePermissionIds);

        /// <summary>
        /// 44.撤消用户的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokePermissionId">撤消权限</param>
        /// <returns>影响的行数</returns>
        int RevokeUserPermissionById(BaseUserInfo userInfo, string userId, string revokePermissionId);

        /// <summary>
        /// 45.获取用户的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        string[] GetUserScopeOrganizationIds(BaseUserInfo userInfo, string userId, string permissionCode);

        /// <summary>
        /// 46.设置用户的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantOrganizationIds">授予的组织主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        int GrantUserOrganizationScopes(BaseUserInfo userInfo, string userId, string[] grantOrganizationIds, string permissionCode);

        /// <summary>
        /// 47.设置用户的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeOrganizationIds">撤消的组织主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        int RevokeUserOrganizationScopes(BaseUserInfo userInfo, string userId, string[] revokeOrganizationIds, string permissionCode);

        /// <summary>
        /// 48.获取用户的某个权限域的用户范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        string[] GetUserScopeUserIds(BaseUserInfo userInfo, string systemCode, string userId, string permissionCode);

        /// <summary>
        /// 49.设置用户的某个权限域的用户范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantUserIds">授予的用户主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        int GrantUserUserScopes(BaseUserInfo userInfo, string userId, string[] grantUserIds, string permissionCode);

        /// <summary>
        /// 50.设置用户的某个权限域的用户范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeUserIds">撤消的用户主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        int RevokeUserUserScopes(BaseUserInfo userInfo, string userId, string[] revokeUserIds, string permissionCode);

        /// <summary>
        /// 51.获取用户的某个权限域的用户范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        string[] GetUserScopeRoleIds(BaseUserInfo userInfo, string systemCode, string userId, string permissionCode);

        /// <summary>
        /// 52.设置用户的某个权限域的用户范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantRoleIds">授予的角色主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        int GrantUserRoleScopes(BaseUserInfo userInfo, string systemCode, string userId, string[] grantRoleIds, string permissionCode);

        /// <summary>
        /// 53.设置用户的某个权限域的用户范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeRoleds">撤消的角色主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        int RevokeUserRoleScopes(BaseUserInfo userInfo, string userId, string[] revokeRoleds, string permissionCode);

        /// <summary>
        /// 54.获取用户授权权限列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        string[] GetUserScopePermissionIds(BaseUserInfo userInfo, string userId, string permissionCode);

        /// <summary>
        /// 55.授予用户的授权权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantPermissionIds">授予的权限主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        int GrantUserPermissionScopes(BaseUserInfo userInfo, string userId, string[] grantPermissionIds, string permissionCode);

        /// <summary>
        /// 56.撤消用户的授权权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokePermissionIds">撤消的权限主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        int RevokeUserPermissionScopes(BaseUserInfo userInfo, string userId, string[] revokePermissionIds, string permissionCode);

        /// <summary>
        /// 57.清除用户操作权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>影响的行数</returns>
        int ClearUserPermission(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 58.清除用户权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        int ClearUserPermissionScope(BaseUserInfo userInfo, string id, string permissionCode);

        /// <summary>
        /// 61.获取用户模块权限范围主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        string[] GetUserScopeModuleIds(BaseUserInfo userInfo, string userId, string permissionCode);

        /// <summary>
        /// 62.授予用户模块的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantModuleId">授予模块主键</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>影响的行数</returns>
        string GrantUserModuleScope(BaseUserInfo userInfo, string userId, string grantModuleId, string permissionCode);

        /// <summary>
        /// 63.授予用户模块的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantModuleIds">授予模块主键数组</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>影响的行数</returns>
        int GrantUserModuleScopes(BaseUserInfo userInfo, string userId, string[] grantModuleIds, string permissionCode);

        /// <summary>
        /// 64.撤消用户模块的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeModuleId">撤消模块主键数组</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>影响的行数</returns>
        int RevokeUserModuleScope(BaseUserInfo userInfo, string userId, string revokeModuleId, string permissionCode);

        /// <summary>
        /// 65.撤消用户模块的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeModuleIds">撤消模块主键数组</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>影响的行数</returns>
        int RevokeUserModuleScopes(BaseUserInfo userInfo, string userId, string[] revokeModuleIds, string permissionCode);

        /// <summary>
        /// 获取用户权限树
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>用户主键</returns>
        string[] GetPermissionTreeUserIds(BaseUserInfo userInfo, string userId, string permissionCode);
    }
}