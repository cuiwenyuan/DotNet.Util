//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
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
        #region 用户权限关联关系相关

        #region public string[] GetUserPermissionIds(BaseUserInfo userInfo, string userId)
        /// <summary>
        /// 获取用户拥有的操作权限主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <returns>主键数组</returns>
        public string[] GetUserPermissionIds(BaseUserInfo userInfo, string userId)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (ServiceUtil.ProcessFun)((dbHelper) =>
            {
                var manager = new BasePermissionManager((IDbHelper)dbHelper, (BaseUserInfo)userInfo);
                result = manager.GetPermissionIds(userInfo.SystemCode, userId);
            }));

            return result;
        }
        #endregion

        #region public string[] GetUserIdsByPermission(BaseUserInfo userInfo, string result)
        /// <summary>
        /// 获取用户权限主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>主键数组</returns>
        public string[] GetUserIdsByPermission(BaseUserInfo userInfo, string permissionId)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (ServiceUtil.ProcessFun)((dbHelper) =>
            {
                // BaseLogManager.Instance.Add(result, this.serviceName, MethodBase.GetCurrentMethod());
                var manager = new BasePermissionManager((IDbHelper)dbHelper, (BaseUserInfo)userInfo);
                result = manager.GetUserIds((string)userInfo.SystemCode, (string)permissionId);
            }));

            return result;
        }
        #endregion

        #region public int GrantUserPermissions(BaseUserInfo userInfo, string[] userIds, string[] grantPermissionIds)
        /// <summary>
        /// 授予用户操作权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userIds">用户主键数组</param>
        /// <param name="grantPermissionIds">授予权限数组</param>
        /// <returns>影响的行数</returns>
        public int GrantUserPermissions(BaseUserInfo userInfo, string[] userIds, string[] grantPermissionIds)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (ServiceUtil.ProcessFun)((dbHelper) =>
            {
                var manager = new BasePermissionManager((IDbHelper)dbHelper, (BaseUserInfo)userInfo);
                // 小心异常，检查一下参数的有效性
                if (userIds != null && grantPermissionIds != null)
                {
                    result += manager.GrantUser(userInfo.SystemCode, userIds, grantPermissionIds);
                }
            }));

            return result;
        }
        #endregion

        #region public string GrantUserPermissionById(BaseUserInfo userInfo, string userId, string grantPermissionId)
        /// <summary>
        /// 授予用户操作权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantPermissionId">授予权限数组</param>
        /// <returns>影响的行数</returns>
        public string GrantUserPermissionById(BaseUserInfo userInfo, string userId, string grantPermissionId)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (ServiceUtil.ProcessFun)((dbHelper) =>
            {
                var manager = new BasePermissionManager((IDbHelper)dbHelper, (BaseUserInfo)userInfo);
                // 小心异常，检查一下参数的有效性
                if (grantPermissionId != null)
                {
                    result = manager.GrantUser(userInfo.SystemCode, userId, grantPermissionId);
                }
            }));

            return result;
        }
        #endregion

        #region public int RevokeUserPermissions(BaseUserInfo userInfo, string[] userIds, string[] revokePermissionIds)
        /// <summary>
        /// 撤消用户操作权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userIds">用户主键数组</param>
        /// <param name="revokePermissionIds">撤消权限数组</param>
        /// <returns>影响的行数</returns>
        public int RevokeUserPermissions(BaseUserInfo userInfo, string[] userIds, string[] revokePermissionIds)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (ServiceUtil.ProcessFun)((dbHelper) =>
            {
                var manager = new BasePermissionManager((IDbHelper)dbHelper, (BaseUserInfo)userInfo);
                // 小心异常，检查一下参数的有效性
                if (userIds != null && revokePermissionIds != null)
                {
                    result += manager.RevokeUser(userInfo.SystemCode, userIds, revokePermissionIds);
                }
            }));

            return result;
        }
        #endregion

        #region public int RevokeUserPermissionById(BaseUserInfo userInfo, string userId, string revokePermissionId)
        /// <summary>
        /// 撤消用户操作权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokePermissionId">撤消权限数组</param>
        /// <returns>影响的行数</returns>
        public int RevokeUserPermissionById(BaseUserInfo userInfo, string userId, string revokePermissionId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (ServiceUtil.ProcessFun)((dbHelper) =>
            {
                var manager = new BasePermissionManager((IDbHelper)dbHelper, (BaseUserInfo)userInfo);
                // 小心异常，检查一下参数的有效性
                if (revokePermissionId != null)
                {
                    result += manager.RevokeUser(userInfo.SystemCode, userId, revokePermissionId);
                }
            }));

            return result;
        }
        #endregion

        #region public string[] GetUserScopeOrganizationIds(BaseUserInfo userInfo, string userId, string permissionCode)
        /// <summary>
        /// 获取用户的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetUserScopeOrganizationIds(BaseUserInfo userInfo, string userId, string permissionCode)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BasePermissionManager(dbHelper, userInfo, tableName);
                result = manager.GetOrganizationIds(userInfo.SystemCode, userId, permissionCode);
            });

            return result;
        }
        #endregion

        #endregion

        /// <summary>
        /// 获取用户权限树
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>用户主键</returns>
        public string[] GetPermissionTreeUserIds(BaseUserInfo userInfo, string userId, string permissionCode)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var permissionScopeManager = new BasePermissionScopeManager(dbHelper, userInfo);
                result = permissionScopeManager.GetPermissionTreeUserIds(userInfo.SystemCode, userId, permissionCode);
            });
            return result;
        }
    }
}