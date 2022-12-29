//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;

namespace DotNet.Business
{
    using IService;
    using Model;
    using Util;

    /// <summary>
    /// BasePermissionService
    /// 角色权限判断服务
    /// 
    /// 修改记录
    /// 
    ///		2015.12.17 版本：1.1 JiRiGaLa 增加 GetRolePermissionList 方法。
    ///		2012.03.22 版本：1.0 JiRiGaLa 创建权限判断服务。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.12.17</date>
    /// </author> 
    /// </summary>
    public partial class BasePermissionService : IBasePermissionService
    {
        #region 角色权限关联关系相关

        #region public List<BaseModuleEntity> GetRolePermissionList(BaseUserInfo userInfo, string roleId)
        /// <summary>
        /// 获取角色权限主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>权限列表</returns>
        public List<BaseModuleEntity> GetRolePermissionList(BaseUserInfo userInfo, string roleId)
        {
            List<BaseModuleEntity> result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BasePermissionManager(dbHelper, userInfo);
                result = manager.GetPermissionList(userInfo.SystemCode, roleId, "Role");
            });

            return result;
        }
        #endregion

        #region public string[] GetRolePermissionIds(BaseUserInfo userInfo, string roleId)
        /// <summary>
        /// 获取角色权限主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>主键数组</returns>
        public string[] GetRolePermissionIds(BaseUserInfo userInfo, string roleId)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BasePermissionManager(dbHelper, userInfo);
                result = manager.GetPermissionIds(userInfo.SystemCode, roleId, "Role");
            });

            return result;
        }
        #endregion

        #region public string[] GetRoleIdsByPermission(BaseUserInfo userInfo, string result)
        /// <summary>
        /// 获取角色主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="permissionId">操作权限主键</param>
        /// <returns>主键数组</returns>
        public string[] GetRoleIdsByPermission(BaseUserInfo userInfo, string permissionId)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BasePermissionManager(dbHelper, userInfo);
                result = manager.GetRoleIds(userInfo.SystemCode, permissionId);
                // BaseLogManager.Instance.Add(result, this.serviceName, MethodBase.GetCurrentMethod());
            });

            return result;
        }
        #endregion

        #region public int GrantRolePermissions(BaseUserInfo userInfo, string[] roleIds, string[] grantPermissionIds)
        /// <summary>
        /// 授予角色的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleIds">角色主键数组</param>
        /// <param name="grantPermissionIds">授予权限数组</param>
        /// <returns>影响的行数</returns>
        public int GrantRolePermissions(BaseUserInfo userInfo, string[] roleIds, string[] grantPermissionIds)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Permission";
                var manager = new BasePermissionManager(dbHelper, userInfo, tableName);
                // 小心异常，检查一下参数的有效性
                if (roleIds != null && grantPermissionIds != null)
                {
                    result += manager.GrantRole(userInfo.SystemCode, roleIds, grantPermissionIds);
                }
            });

            return result;
        }
        #endregion

        #region public string GrantRolePermissionById(BaseUserInfo userInfo, string roleId, string grantPermissionId)
        /// <summary>
        /// 授予角色的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="grantPermissionId">授予权限数组</param>
        /// <returns>数据主键</returns>
        public string GrantRolePermissionById(BaseUserInfo userInfo, string roleId, string grantPermissionId)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BasePermissionManager(dbHelper, userInfo);
                // 小心异常，检查一下参数的有效性
                if (grantPermissionId != null)
                {
                    result = manager.GrantRole(userInfo.SystemCode, roleId, grantPermissionId);
                }
            });

            return result;
        }
        #endregion

        #region public int RevokeRolePermissions(BaseUserInfo userInfo, string[] roleIds, string[] revokePermissionIds)
        /// <summary>
        /// 撤消角色的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleIds">角色主键数组</param>
        /// <param name="revokePermissionIds">撤消权限数组</param>
        /// <returns>影响的行数</returns>
        public int RevokeRolePermissions(BaseUserInfo userInfo, string[] roleIds, string[] revokePermissionIds)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Permission";
                var manager = new BasePermissionManager(dbHelper, userInfo, tableName);
                // 小心异常，检查一下参数的有效性
                if (roleIds != null && revokePermissionIds != null)
                {
                    result += manager.RevokeRole(userInfo.SystemCode, roleIds, revokePermissionIds);
                }
            });

            return result;
        }
        #endregion

        #region public int RevokeRolePermissionById(BaseUserInfo userInfo, string roleId, string revokePermissionId)
        /// <summary>
        /// 撤消角色的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="revokePermissionId">撤消权限数组</param>
        /// <returns>影响的行数</returns>
        public int RevokeRolePermissionById(BaseUserInfo userInfo, string roleId, string revokePermissionId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BasePermissionManager(dbHelper, userInfo);
                // 小心异常，检查一下参数的有效性
                if (revokePermissionId != null)
                {
                    result += manager.RevokeRole(userInfo.SystemCode, roleId, revokePermissionId);
                }
            });

            return result;
        }
        #endregion

        #endregion
    }
}