//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using System;

    /// <summary>
    /// BaseUserScopeManager
    /// 用户对角色权限域
    /// 
    /// 修改记录
    ///
    ///     2011.03.13 版本：2.0 JiRiGaLa 重新整理代码。
    ///     2008.05.24 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.03.13</date>
    /// </author>
    /// </summary>
    public partial class BaseUserScopeManager : BaseManager, IBaseManager
    {
        ////
        ////
        //// 授权范围管理部分
        ////
        ////

        #region public string[] GetRoleIds(string systemCode, string userId, string permissionCode) 获取角色的权限主键数组
        /// <summary>
        /// 获取角色的权限主键数组
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>主键数组</returns>
        public string[] GetRoleIds(string systemCode, string userId, string permissionId)
        {
            string[] result = null;

            if (!string.IsNullOrEmpty(permissionId))
            {
                var roleTableName = systemCode + "Role";
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.CurrentTableName),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, roleTableName),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId)
                };

                // 20130605 JiRiGaLa 这个运行效率更高一些
                // this.CurrentTableName = systemCode + "PermissionScope";
                result = GetProperties(parameters, BasePermissionScopeEntity.FieldTargetId);
                // var result = this.GetDataTable(parameters);
                // result = BaseUtil.FieldToArray(result, BasePermissionScopeEntity.FieldTargetId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
            }

            return result;
        }
        #endregion
        //
        // 授予授权范围的实现部分
        //

        #region private string GrantRole(BasePermissionScopeManager manager, string id, string userId, string grantRoleId) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="manager">权限范围管理器</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantRoleId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        private string GrantRole(BasePermissionScopeManager manager, string systemCode, string userId, string grantRoleId, string permissionCode)
        {
            var result = string.Empty;

            var permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);
            if (!string.IsNullOrEmpty(permissionId))
            {
                // 对应哪个角色
                var roleTableName = systemCode + "Role";
                var entity = new BasePermissionScopeEntity
                {
                    PermissionId = permissionId.ToInt(),
                    ResourceCategory = BaseUserEntity.CurrentTableName,
                    ResourceId = userId.ToInt(),
                    TargetCategory = roleTableName,
                    TargetId = grantRoleId.ToInt(),
                    Enabled = 1,
                    Deleted = 0
                };
                result = manager.Add(entity, true, false);
            }

            return result;
        }
        #endregion

        #region public string GrantRole(string userId, string result) 员工授予权限
        /// <summary>
        /// 员工授予权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantRoleId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>主键</returns>
        public string GrantRole(string systemCode, string userId, string grantRoleId, string permissionCode)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            return GrantRole(permissionScopeManager, systemCode, userId, grantRoleId, permissionCode);
        }
        #endregion

        /// <summary>
        /// 授权角色
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <param name="grantRoleIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantRoles(string systemCode, string userId, string[] grantRoleIds, string permissionCode)
        {
            var result = 0;
            var manager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < grantRoleIds.Length; i++)
            {
                GrantRole(manager, systemCode, userId, grantRoleIds[i], permissionCode);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 授权角色
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="grantRoleId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantRoles(string systemCode, string[] userIds, string grantRoleId, string permissionCode)
        {
            var result = 0;
            var manager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < userIds.Length; i++)
            {
                GrantRole(manager, systemCode, userIds[i], grantRoleId, permissionCode);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 授权用户
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="grantRoleIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantRoles(string systemCode, string[] userIds, string[] grantRoleIds, string permissionCode)
        {
            var result = 0;
            var manager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < userIds.Length; i++)
            {
                for (var j = 0; j < grantRoleIds.Length; j++)
                {
                    GrantRole(manager, systemCode, userIds[i], grantRoleIds[j], permissionCode);
                    result++;
                }
            }
            return result;
        }


        //
        //  撤销授权范围的实现部分
        //

        #region private int RevokeRole(BasePermissionScopeManager manager, string userId, string revokeRoleId, string permissionCode) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="manager">权限域读写器</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeRoleId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        private int RevokeRole(BasePermissionScopeManager manager, string systemCode, string userId, string revokeRoleId, string permissionCode)
        {
            var permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);
            var roleTableName = UserInfo.SystemCode + "Role";
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.CurrentTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, roleTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, revokeRoleId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId)
            };
            return manager.Delete(parameters);
        }
        #endregion

        #region public int RevokeRole(string userId, string revokeRoleId, string permissionCode) 员工撤销授权
        /// <summary>
        /// 员工撤销授权
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeRoleId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        public int RevokeRole(string systemCode, string userId, string revokeRoleId, string permissionCode)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            return RevokeRole(permissionScopeManager, systemCode, userId, revokeRoleId, permissionCode);
        }
        #endregion

        /// <summary>
        /// 撤回角色
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <param name="revokeRoleIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeRoles(string systemCode, string userId, string[] revokeRoleIds, string permissionCode)
        {
            var result = 0;
            var manager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < revokeRoleIds.Length; i++)
            {
                RevokeRole(manager, systemCode, userId, revokeRoleIds[i], permissionCode);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 撤回角色
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="revokeRoleId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeRoles(string systemCode, string[] userIds, string revokeRoleId, string permissionCode)
        {
            var result = 0;
            var manager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < userIds.Length; i++)
            {
                RevokeRole(manager, systemCode, userIds[i], revokeRoleId, permissionCode);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 撤回角色
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="revokeRoleIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeRoles(string systemCode, string[] userIds, string[] revokeRoleIds, string permissionCode)
        {
            var result = 0;
            var manager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < userIds.Length; i++)
            {
                for (var j = 0; j < revokeRoleIds.Length; j++)
                {
                    RevokeRole(manager, systemCode, userIds[i], revokeRoleIds[j], permissionCode);
                    result++;
                }
            }
            return result;
        }
    }
}