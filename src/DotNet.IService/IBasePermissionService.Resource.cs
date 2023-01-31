//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;


namespace DotNet.IService
{
    using Model;
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
        /// <summary>
        /// 获取用户的权限范围列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionId">操作权限主键</param>
        /// <returns>权限范围列表</returns>
        List<BasePermissionScopeEntity> GetUserPermissionScopeList(BaseUserInfo userInfo, string userId, string permissionId);

        /// <summary>
        /// 获取角色的权限范围列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionId">操作权限主键</param>
        /// <returns>权限范围列表</returns>
        List<BasePermissionScopeEntity> GetRolePermissionScopeList(BaseUserInfo userInfo, string roleId, string permissionId);

        //资源权限设定关系相关

        /// <summary>
        /// 获取资源权限主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源分类</param>
        /// <param name="resourceId">资源主键</param>
        /// <returns>主键数组</returns>
        string[] GetResourcePermissionIds(BaseUserInfo userInfo, string resourceCategory, string resourceId);

        /// <summary>
        /// 撤消资源的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源分类</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="revokePermissionIds">权限主键</param>
        /// <returns>影响的行数</returns>
        int RevokeResourcePermission(BaseUserInfo userInfo, string resourceCategory, string resourceId, string[] revokePermissionIds);


        //资源权限范围设定关系相关


        /// <summary>
        /// 获取资源权限范围主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源分类</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="targetCategory">目标类别</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        string[] GetPermissionScopeTargetIds(BaseUserInfo userInfo, string resourceCategory, string resourceId, string targetCategory, string permissionCode);

        /// <summary>
        /// 获取数据权限目标主键
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源类别</param>
        /// <param name="targetResourceId">资源主键</param>
        /// <param name="targetResourceCategory">目标资源</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        string[] GetPermissionScopeResourceIds(BaseUserInfo userInfo, string resourceCategory, string targetResourceId, string targetResourceCategory, string permissionCode);

        /// <summary>
        /// 授予资源的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源分类</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="targetCategory">目标类别</param>
        /// <param name="grantTargetIds">目标主键数组</param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>影响的行数</returns>
        int GrantPermissionScopeTargets(BaseUserInfo userInfo, string resourceCategory, string resourceId, string targetCategory, string[] grantTargetIds, string permissionId);

        /// <summary>
        /// 授予数据权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源类别</param>
        /// <param name="resourceIds">资源主键数组</param>
        /// <param name="targetCategory">目标资源类别</param>
        /// <param name="grantTargetId">目标资源主键</param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>影响行数</returns>
        int GrantPermissionScopeTarget(BaseUserInfo userInfo, string resourceCategory, string[] resourceIds, string targetCategory, string grantTargetId, string permissionId);

        /// <summary>
        /// 撤消资源的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源分类</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="targetCategory">目标类别</param>
        /// <param name="revokeTargetIds">目标主键数组</param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>影响的行数</returns>
        int RevokePermissionScopeTargets(BaseUserInfo userInfo, string resourceCategory, string resourceId, string targetCategory, string[] revokeTargetIds, string permissionId);

        /// <summary>
        /// 撤销数据权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源类别</param>
        /// <param name="resourceIds">资源主键数组</param>
        /// <param name="targetCategory">目标分类</param>
        /// <param name="revokeTargetId">目标主键</param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>影响行数</returns>
        int RevokePermissionScopeTarget(BaseUserInfo userInfo, string resourceCategory, string[] resourceIds, string targetCategory, string revokeTargetId, string permissionId);

        /// <summary>
        /// 清除数据权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源类别</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="targetCategory">目标类别</param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>影响行数</returns>
        int ClearPermissionScopeTarget(BaseUserInfo userInfo, string resourceCategory, string resourceId, string targetCategory, string permissionId);

        /// <summary>
        /// 获取用户的某个资源的权限范围(列表资源)
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="targetCategory">目标类别</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        string[] GetResourceScopeIds(BaseUserInfo userInfo, string userId, string targetCategory, string permissionCode);

        /// <summary>
        /// 获取用户的某个资源的权限范围(树型资源)
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="targetCategory">目标类别</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="childrens">是否含子节点</param>
        /// <returns>主键数组</returns>
        string[] GetTreeResourceScopeIds(BaseUserInfo userInfo, string userId, string targetCategory, string permissionCode, bool childrens);
    }
}