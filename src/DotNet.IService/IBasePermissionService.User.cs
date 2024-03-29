﻿//-----------------------------------------------------------------
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
        /// 获取用户权限树
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>用户主键</returns>
        string[] GetPermissionTreeUserIds(BaseUserInfo userInfo, string userId, string permissionCode);
    }
}