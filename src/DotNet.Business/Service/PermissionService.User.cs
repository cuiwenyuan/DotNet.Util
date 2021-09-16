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
    ///		<name>Troy.Cui</name>
    ///		<date>2012.03.22</date>
    /// </author> 
    /// </summary>
    public partial class PermissionService : IPermissionService
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
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseUserPermissionManager(dbHelper, userInfo);
                result = manager.GetPermissionIds(userInfo.SystemCode, userId);
            });

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
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // BaseLogManager.Instance.Add(result, this.serviceName, MethodBase.GetCurrentMethod());
                var manager = new BaseUserPermissionManager(dbHelper, userInfo);
                result = manager.GetUserIds(userInfo.SystemCode, permissionId);
            });

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
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseUserPermissionManager(dbHelper, userInfo);
                // 小心异常，检查一下参数的有效性
                if (userIds != null && grantPermissionIds != null)
                {
                    result += manager.Grant(userInfo.SystemCode, userIds, grantPermissionIds);
                }
            });

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
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseUserPermissionManager(dbHelper, userInfo);
                // 小心异常，检查一下参数的有效性
                if (grantPermissionId != null)
                {
                    result = manager.Grant(userInfo.SystemCode, userId, grantPermissionId);
                }
            });

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
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseUserPermissionManager(dbHelper, userInfo);
                // 小心异常，检查一下参数的有效性
                if (userIds != null && revokePermissionIds != null)
                {
                    result += manager.Revoke(userInfo.SystemCode, userIds, revokePermissionIds);
                }
            });

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
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseUserPermissionManager(dbHelper, userInfo);
                // 小心异常，检查一下参数的有效性
                if (revokePermissionId != null)
                {
                    result += manager.Revoke(userInfo.SystemCode, userId, revokePermissionId);
                }
            });

            return result;
        }
        #endregion

        #region public string[] GetUserScopeOrganizeIds(BaseUserInfo userInfo, string userId, string permissionCode)
        /// <summary>
        /// 获取用户的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetUserScopeOrganizeIds(BaseUserInfo userInfo, string userId, string permissionCode)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BaseUserScopeManager(dbHelper, userInfo, tableName);
                result = manager.GetOrganizeIds(userInfo.SystemCode, userId, permissionCode);
            });

            return result;
        }
        #endregion

        #region public int GrantUserOrganizeScope(BaseUserInfo userInfo, string userId, string[] grantOrganizeIds, string permissionCode)
        /// <summary>
        /// 设置用户的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantOrganizeIds">授予的组织主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int GrantUserOrganizeScopes(BaseUserInfo userInfo, string userId, string[] grantOrganizeIds, string permissionCode)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BaseUserScopeManager(dbHelper, userInfo, tableName);
                // 小心异常，检查一下参数的有效性
                if (grantOrganizeIds == null)
                {
                    result += manager.RevokeOrganize(userInfo.SystemCode, userId, permissionCode);
                }
                else
                {
                    result += manager.GrantOrganizes(userInfo.SystemCode, userId, grantOrganizeIds, permissionCode);
                }
            });
            return result;
        }
        #endregion

        #region public int RevokeUserOrganizeScope(BaseUserInfo userInfo, string userId, string[] revokeOrganizeIds, string permissionCode)
        /// <summary>
        /// 设置用户的某个权限域的组织范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeOrganizeIds">撤消的组织主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int RevokeUserOrganizeScopes(BaseUserInfo userInfo, string userId, string[] revokeOrganizeIds, string permissionCode)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BaseUserScopeManager(dbHelper, userInfo, tableName);
                // 小心异常，检查一下参数的有效性
                if (revokeOrganizeIds != null)
                {
                    result += manager.RevokeOrganizes(userInfo.SystemCode, userId, revokeOrganizeIds, permissionCode);
                }
            });
            return result;
        }
        #endregion

        #region public string[] GetUserScopeUserIds(BaseUserInfo userInfo, string systemCode, string userId, string permissionCode)
        /// <summary>
        /// 获取用户的某个权限域的用户范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetUserScopeUserIds(BaseUserInfo userInfo, string systemCode, string userId, string permissionCode)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = systemCode + "PermissionScope";
                var manager = new BaseUserScopeManager(dbHelper, userInfo, tableName);
                result = manager.GetUserIds(systemCode, userId, permissionCode);
            });

            return result;
        }
        #endregion

        #region public int GrantUserUserScope(BaseUserInfo userInfo, string userId, string[] grantUserIds, string permissionCode)
        /// <summary>
        /// 设置用户的某个权限域的用户范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantUserIds">授予的用户主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int GrantUserUserScopes(BaseUserInfo userInfo, string userId, string[] grantUserIds, string permissionCode)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BaseUserScopeManager(dbHelper, userInfo, tableName);
                // 小心异常，检查一下参数的有效性
                if (grantUserIds != null)
                {
                    result += manager.GrantUsers(userInfo.SystemCode, userId, grantUserIds, permissionCode);
                }
            });

            return result;
        }
        #endregion

        #region public int RevokeUserUserScope(BaseUserInfo userInfo, string userId, string[] revokeUserIds, string permissionCode)
        /// <summary>
        /// 设置用户的某个权限域的用户范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeUserIds">撤消的用户主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int RevokeUserUserScopes(BaseUserInfo userInfo, string userId, string[] revokeUserIds, string permissionCode)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BaseUserScopeManager(dbHelper, userInfo, tableName);
                // 小心异常，检查一下参数的有效性
                if (revokeUserIds != null)
                {
                    result += manager.RevokeUsers(userInfo.SystemCode, userId, revokeUserIds, permissionCode);
                }
            });
            return result;
        }
        #endregion

        #region public string[] GetUserScopeRoleIds(BaseUserInfo userInfo, string systemCode, string userId, string permissionCode)
        /// <summary>
        /// 获取用户的某个权限域的用户范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetUserScopeRoleIds(BaseUserInfo userInfo, string systemCode, string userId, string permissionCode)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = systemCode + "PermissionScope";
                var manager = new BaseUserScopeManager(dbHelper, userInfo, tableName);
                result = manager.GetRoleIds(systemCode, userId, permissionCode);
            });

            return result;
        }
        #endregion

        #region public int GrantUserRoleScope(BaseUserInfo userInfo, string systemCode, string userId, string[] grantRoleIds, string permissionCode)
        /// <summary>
        /// 设置用户的某个权限域的用户范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantRoleIds">授予的用户主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int GrantUserRoleScopes(BaseUserInfo userInfo, string systemCode, string userId, string[] grantRoleIds, string permissionCode)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = systemCode + "PermissionScope";
                var manager = new BaseUserScopeManager(dbHelper, userInfo, tableName);
                // 小心异常，检查一下参数的有效性
                if (grantRoleIds != null)
                {
                    result += manager.GrantRoles(systemCode, userId, grantRoleIds, permissionCode);
                }
            });

            return result;
        }
        #endregion

        #region public int RevokeUserRoleScope(BaseUserInfo userInfo, string userId, string[] revokeRoleIds, string permissionCode)
        /// <summary>
        /// 设置用户的某个权限域的用户范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeRoleIds">撤消的用户主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int RevokeUserRoleScopes(BaseUserInfo userInfo, string userId, string[] revokeRoleIds, string permissionCode)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BaseUserScopeManager(dbHelper, userInfo, tableName);
                // 小心异常，检查一下参数的有效性
                if (revokeRoleIds != null)
                {
                    result += manager.RevokeRoles(userInfo.SystemCode, userId, revokeRoleIds, permissionCode);
                }
            });
            return result;
        }
        #endregion

        #region public string[] GetUserScopePermissionIds(BaseUserInfo userInfo, string userId)
        /// <summary>
        /// 获取用户授权权限列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>主键数组</returns>
        public string[] GetUserScopePermissionIds(BaseUserInfo userInfo, string userId, string permissionCode)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BaseUserScopeManager(dbHelper, userInfo, tableName);
                result = manager.GetPermissionIds(userInfo.SystemCode, userId, permissionCode);
                // BaseLogManager.Instance.Add(result, this.serviceName, MethodBase.GetCurrentMethod());
            });

            return result;
        }
        #endregion

        #region public int GrantUserPermissionScopes(BaseUserInfo userInfo, string userId, string[] grantPermissionIds, string permissionCode)
        /// <summary>
        /// 授予用户的授权权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantPermissionIds">授予的权限主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int GrantUserPermissionScopes(BaseUserInfo userInfo, string userId, string[] grantPermissionIds, string permissionCode)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BaseUserScopeManager(dbHelper, userInfo, tableName);
                // 小心异常，检查一下参数的有效性
                if (grantPermissionIds != null)
                {
                    result += manager.GrantPermissiones(userInfo.SystemCode, userId, grantPermissionIds, permissionCode);
                }
            });

            return result;
        }
        #endregion

        #region public int RevokeUserPermissionScopes(BaseUserInfo userInfo, string userId, string[] revokePermissionIds, string permissionCode)
        /// <summary>
        /// 撤消用户的授权权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokePermissionIds">撤消的权限主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int RevokeUserPermissionScopes(BaseUserInfo userInfo, string userId, string[] revokePermissionIds, string permissionCode)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BaseUserScopeManager(dbHelper, userInfo, tableName);
                // 小心异常，检查一下参数的有效性
                if (revokePermissionIds != null)
                {
                    result += manager.RevokePermissions(userInfo.SystemCode, userId, revokePermissionIds, permissionCode);
                }
            });

            return result;
        }
        #endregion

        #region public int ClearUserPermission(BaseUserInfo userInfo, string id)
        /// <summary>
        /// 清除用户权限
        /// 
        /// 1.清除用户的角色归属。
        /// 2.清除用户的模块权限。
        /// 3.清除用户的操作权限。
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>数据表</returns>
        public int ClearUserPermission(BaseUserInfo userInfo, string id)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var systemCode = userInfo.SystemCode;

                var userManager = new BaseUserManager(dbHelper, userInfo);
                result += userManager.ClearRole(systemCode, id);

                var userPermissionManager = new BaseUserPermissionManager(dbHelper, userInfo);
                result += userPermissionManager.RevokeAll(systemCode, id);

                var userPermissionScopeManager = new BaseUserScopeManager(dbHelper, userInfo);
                result += userPermissionScopeManager.RevokeAll(systemCode, id);
            });

            return result;
        }
        #endregion

        #region public int ClearUserPermissionScope(BaseUserInfo userInfo, string userId, string permissionCode)
        /// <summary>
        /// 清除用户权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">主键</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>数据表</returns>
        public int ClearUserPermissionScope(BaseUserInfo userInfo, string userId, string permissionCode)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BaseUserScopeManager(dbHelper, userInfo, tableName);
                result = manager.ClearUserPermissionScope(userInfo.SystemCode, userId, permissionCode);
            });

            return result;
        }
        #endregion

        #endregion


        #region 用户模块关联关系相关

        #region public string[] GetUserScopeModuleIds(BaseUserInfo userInfo, string userId, string permissionCode)
        /// <summary>
        /// 获取用户模块权限范围主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetUserScopeModuleIds(BaseUserInfo userInfo, string userId, string permissionCode)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BaseUserScopeManager(dbHelper, userInfo, tableName);
                result = manager.GetModuleIds(userInfo.SystemCode, userId, permissionCode);
            });

            return result;
        }
        #endregion

        #region public int GrantUserModuleScopes(BaseUserInfo userInfo, string userId, string[] grantModuleIds, string permissionCode)
        /// <summary>
        /// 授予用户模块的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantModuleIds">授予模块主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int GrantUserModuleScopes(BaseUserInfo userInfo, string userId, string[] grantModuleIds, string permissionCode)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BaseUserScopeManager(dbHelper, userInfo, tableName);
                // 小心异常，检查一下参数的有效性
                if (grantModuleIds != null)
                {
                    result += manager.GrantModules(userInfo.SystemCode, userId, grantModuleIds, permissionCode);
                }
            });
            return result;
        }
        #endregion

        #region public string GrantUserModuleScope(BaseUserInfo userInfo, string userId, string grantModuleId, string permissionCode)
        /// <summary>
        /// 授予用户模块的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantModuleId">授予模块主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public string GrantUserModuleScope(BaseUserInfo userInfo, string userId, string grantModuleId, string permissionCode)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BaseUserScopeManager(dbHelper, userInfo, tableName);
                // 小心异常，检查一下参数的有效性
                if (grantModuleId != null)
                {
                    result = manager.GrantModule(userInfo.SystemCode, userId, grantModuleId, permissionCode);
                }
            });
            return result;
        }
        #endregion

        #region public int RevokeUserModuleScope(BaseUserInfo userInfo, string userId, string revokeModuleId, string permissionCode)
        /// <summary>
        /// 撤消用户模块的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeModuleId">撤消模块主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int RevokeUserModuleScope(BaseUserInfo userInfo, string userId, string revokeModuleId, string permissionCode)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BaseUserScopeManager(dbHelper, userInfo, tableName);
                // 小心异常，检查一下参数的有效性
                if (revokeModuleId != null)
                {
                    result += manager.RevokeModule(userInfo.SystemCode, userId, revokeModuleId, permissionCode);
                }
            });
            return result;
        }
        #endregion

        #region public int RevokeUserModuleScopes(BaseUserInfo userInfo, string userId, string[] revokeModuleIds, string permissionCode)
        /// <summary>
        /// 撤消用户模块的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeModuleIds">撤消模块主键数组</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响的行数</returns>
        public int RevokeUserModuleScopes(BaseUserInfo userInfo, string userId, string[] revokeModuleIds, string permissionCode)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BaseUserScopeManager(dbHelper, userInfo, tableName);
                // 小心异常，检查一下参数的有效性
                if (revokeModuleIds != null)
                {
                    result = manager.RevokeModules(userInfo.SystemCode, userId, revokeModuleIds, permissionCode);
                }
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