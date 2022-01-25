//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace DotNet.Business
{
    using Model;
    using System;
    using Util;

    /// <summary>
    /// BaseRoleScopeManager
    /// 角色用户权限域
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
    public partial class BaseRoleScopeManager : BaseManager, IBaseManager
    {
        ////
        ////
        //// 授权范围管理部分
        ////
        ////

        #region public string[] GetUserIds(string systemCode, string roleId, string permissionCode) 获取员工的权限主键数组
        /// <summary>
        /// 获取用户的权限主键数组
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionId">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetUserIds(string systemCode, string roleId, string permissionId)
        {
            string[] result = null;
            var roleTableName = systemCode + "Role";
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, roleTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, roleId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseUserEntity.CurrentTableName),
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

        #region private string GrantUser(BasePermissionScopeManager manager, string id, string roleId, string grantUserId) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="permissionScopeManager">权限域读写器</param>
        /// <param name="roleId">员工主键</param>
        /// <param name="grantUserId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        private string GrantUser(string systemCode, BasePermissionScopeManager permissionScopeManager, string roleId, string grantUserId, string permissionCode)
        {
            var result = string.Empty;
            var roleTableName = systemCode + "Role";
            var resourcePermissionScopeEntity = new BasePermissionScopeEntity
            {
                PermissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode).ToInt(),
                ResourceCategory = roleTableName,
                ResourceId = roleId.ToInt(),
                TargetCategory = BaseUserEntity.CurrentTableName,
                TargetId = grantUserId.ToInt(),
                Enabled = 1,
                Deleted = 0
            };
            return permissionScopeManager.Add(resourcePermissionScopeEntity);
        }
        #endregion

        #region public string GrantUser(string roleId, string result) 员工授予权限
        /// <summary>
        /// 员工授予权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="grantUserId">用户主键</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>主键</returns>
        public string GrantUser(string systemCode, string roleId, string grantUserId, string permissionCode)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            return GrantUser(systemCode, permissionScopeManager, roleId, grantUserId, permissionCode);
        }
        #endregion

        /// <summary>
        /// 授权用户
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <param name="grantUserIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantUsers(string systemCode, string roleId, string[] grantUserIds, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < grantUserIds.Length; i++)
            {
                GrantUser(systemCode, permissionScopeManager, roleId, grantUserIds[i], permissionCode);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 授权用户
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="grantUserId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantUsers(string systemCode, string[] roleIds, string grantUserId, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < roleIds.Length; i++)
            {
                GrantUser(systemCode, permissionScopeManager, roleIds[i], grantUserId, permissionCode);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 授权用户
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="grantUserIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantUsers(string systemCode, string[] roleIds, string[] grantUserIds, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < roleIds.Length; i++)
            {
                for (var j = 0; j < grantUserIds.Length; j++)
                {
                    GrantUser(systemCode, permissionScopeManager, roleIds[i], grantUserIds[j], permissionCode);
                    result++;
                }
            }
            return result;
        }


        //
        //  撤销授权范围的实现部分
        //

        #region private int RevokeUser(BasePermissionScopeManager manager, string roleId, string revokeUserId, string permissionCode) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="permissionScopeManager">权限域读写器</param>
        /// <param name="roleId">员工主键</param>
        /// <param name="revokeUserId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        private int RevokeUser(string systemCode, BasePermissionScopeManager permissionScopeManager, string roleId, string revokeUserId, string permissionCode)
        {
            var roleTableName = UserInfo.SystemCode + "Role";
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, roleTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, roleId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseUserEntity.CurrentTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, revokeUserId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode))
            };
            return permissionScopeManager.Delete(parameters);
        }
        #endregion

        #region public int RevokeUser(string roleId, string revokeUserId, string permissionCode) 角色撤销授权
        /// <summary>
        /// 角色撤销授权
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="revokeUserId"></param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        public int RevokeUser(string systemCode, string roleId, string revokeUserId, string permissionCode)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            return RevokeUser(systemCode, permissionScopeManager, roleId, revokeUserId, permissionCode);
        }
        #endregion

        /// <summary>
        /// 撤回用户
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <param name="revokeUserIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeUsers(string systemCode, string roleId, string[] revokeUserIds, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < revokeUserIds.Length; i++)
            {
                result += RevokeUser(systemCode, permissionScopeManager, roleId, revokeUserIds[i], permissionCode);
            }

            return result;
        }

        /// <summary>
        /// 撤回用户
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="revokeUserId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeUsers(string systemCode, string[] roleIds, string revokeUserId, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < roleIds.Length; i++)
            {
                result += RevokeUser(systemCode, permissionScopeManager, roleIds[i], revokeUserId, permissionCode);
            }

            return result;
        }

        /// <summary>
        /// 撤回用户
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="revokeUserIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeUsers(string systemCode, string[] roleIds, string[] revokeUserIds, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < roleIds.Length; i++)
            {
                for (var j = 0; j < revokeUserIds.Length; j++)
                {
                   result+= RevokeUser(systemCode, permissionScopeManager, roleIds[i], revokeUserIds[j], permissionCode);
                }
            }

            return result;
        }
    }
}