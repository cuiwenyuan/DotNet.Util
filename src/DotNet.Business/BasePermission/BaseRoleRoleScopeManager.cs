//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseRoleScopeManager
    /// 角色权限域
    /// 
    /// 修改记录
    ///
    ///     2011.03.13 版本：2.0 JiRiGaLa 重新整理代码。
    ///     2008.05.24 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2011.03.13</date>
    /// </author>
    /// </summary>
    public partial class BaseRoleScopeManager : BaseManager, IBaseManager
    {
        ////
        ////
        //// 授权范围管理部分
        ////
        ////

        #region public string[] GetRoleIds(string roleId, string permissionId) 获取员工的权限主键数组

        /// <summary>
        /// 获取员工的权限主键数组
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionId">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetRoleIds(string systemCode, string roleId, string permissionId)
        {
            string[] result = null;
            var roleTableName = UserInfo.SystemCode + "Role";
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, roleTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, roleId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, roleTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId)
            };
            var dt = GetDataTable(parameters);
            result = BaseUtil.FieldToArray(dt, BasePermissionScopeEntity.FieldTargetId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
            return result;
        }
        #endregion

        //
        // 授予授权范围的实现部分
        //

        #region private string GrantRole(BasePermissionScopeManager manager, string id, string roleId, string grantRoleId) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="permissionScopeManager">权限域读写器</param>
        /// <param name="roleId">员工主键</param>
        /// <param name="grantRoleId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        private string GrantRole(string systemCode, BasePermissionScopeManager permissionScopeManager, string roleId, string grantRoleId, string permissionCode)
        {
            var roleTableName = systemCode + "Role";

            var result = string.Empty;
            var entity = new BasePermissionScopeEntity
            {
                PermissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode),
                ResourceCategory = roleTableName,
                ResourceId = roleId,
                TargetCategory = roleTableName,
                TargetId = grantRoleId,
                Enabled = 1,
                DeletionStateCode = 0
            };
            return permissionScopeManager.Add(entity, true, false);
        }
        #endregion

        #region public string GrantRole(string roleId, string grantRoleId, string permissionCode) 角色授予权限
        /// <summary>
        /// 角色授予权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleId">员工主键</param>
        /// <param name="grantRoleId"></param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        public string GrantRole(string systemCode, string roleId, string grantRoleId, string permissionCode)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            return GrantRole(systemCode, permissionScopeManager, roleId, grantRoleId, permissionCode);
        }
        #endregion

        /// <summary>
        /// 授权角色
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <param name="grantRoleIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantRoles(string systemCode, string roleId, string[] grantRoleIds, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < grantRoleIds.Length; i++)
            {
                GrantRole(systemCode, permissionScopeManager, roleId, grantRoleIds[i], permissionCode);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 授权角色
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="grantRoleId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantRoles(string systemCode, string[] roleIds, string grantRoleId, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < roleIds.Length; i++)
            {
                GrantRole(systemCode, permissionScopeManager, roleIds[i], grantRoleId, permissionCode);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 授权角色
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="grantRoleIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantRoles(string systemCode, string[] roleIds, string[] grantRoleIds, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < roleIds.Length; i++)
            {
                for (var j = 0; j < grantRoleIds.Length; j++)
                {
                    GrantRole(systemCode, permissionScopeManager, roleIds[i], grantRoleIds[j], permissionCode);
                    result++;
                }
            }

            return result;
        }


        //
        //  撤销授权范围的实现部分
        //

        #region private int RevokeRole(BasePermissionScopeManager manager, string roleId, string revokeRoleId, string permissionCode) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="permissionScopeManager">权限域读写器</param>
        /// <param name="roleId">员工主键</param>
        /// <param name="revokeRoleId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        private int RevokeRole(string systemCode, BasePermissionScopeManager permissionScopeManager, string roleId, string revokeRoleId, string permissionCode)
        {
            var roleTableName = UserInfo.SystemCode + "Role";
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, roleTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, roleId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, roleTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, revokeRoleId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode))
            };
            return permissionScopeManager.Delete(parameters);
        }
        #endregion

        #region public int RevokeRole(string roleId, string revokeRoleId, string permissionCode) 角色撤销授权
        /// <summary>
        /// 角色撤销授权
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleId">员工主键</param>
        /// <param name="revokeRoleId"></param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        public int RevokeRole(string systemCode, string roleId, string revokeRoleId, string permissionCode)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            return RevokeRole(systemCode, permissionScopeManager, roleId, revokeRoleId, permissionCode);
        }
        #endregion

        /// <summary>
        /// 撤回角色
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <param name="revokeRoleIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeRoles(string systemCode, string roleId, string[] revokeRoleIds, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < revokeRoleIds.Length; i++)
            {
                RevokeRole(systemCode, permissionScopeManager, roleId, revokeRoleIds[i], permissionCode);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 撤回角色
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="revokeRoleId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeRoles(string systemCode, string[] roleIds, string revokeRoleId, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < roleIds.Length; i++)
            {
                RevokeRole(systemCode, permissionScopeManager, roleIds[i], revokeRoleId, permissionCode);
                result++;
            }

            return result;
        }
        /// <summary>
        /// 撤回角色
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="revokeRoleIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeRoles(string systemCode, string[] roleIds, string[] revokeRoleIds, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < roleIds.Length; i++)
            {
                for (var j = 0; j < revokeRoleIds.Length; j++)
                {
                    RevokeRole(systemCode, permissionScopeManager, roleIds[i], revokeRoleIds[j], permissionCode);
                    result++;
                }
            }

            return result;
        }
    }
}