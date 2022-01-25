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
    /// BaseUserScopeManager
    /// 用户权限域
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

        #region public string[] GetPermissionIds(string userId, string permissionCode) 获取员工的权限主键数组
        /// <summary>
        /// 获取员工的权限主键数组
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">员工主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetPermissionIds(string systemCode, string userId, string permissionCode)
        {
            string[] result = null;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.CurrentTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseModuleEntity.CurrentTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode))
            };

            var dt = GetDataTable(parameters);
            result = BaseUtil.FieldToArray(dt, BasePermissionScopeEntity.FieldTargetId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
            return result;
        }
        #endregion

        //
        // 授予授权范围的实现部分
        //

        #region private string GrantPermission(BasePermissionScopeManager manager, string id, string userId, string grantPermissionId) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="permissionScopeManager">权限域读写器</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantPermissionId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        private string GrantPermission(BasePermissionScopeManager permissionScopeManager, string systemCode, string userId, string grantPermissionId, string permissionCode)
        {
            var result = string.Empty;
            var resourcePermissionScopeEntity = new BasePermissionScopeEntity
            {
                PermissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode).ToInt(),
                ResourceCategory = BaseUserEntity.CurrentTableName,
                ResourceId = userId.ToInt(),
                TargetCategory = BaseModuleEntity.CurrentTableName,
                TargetId = grantPermissionId.ToInt(),
                Enabled = 1,
                Deleted = 0
            };
            return permissionScopeManager.Add(resourcePermissionScopeEntity);
        }
        #endregion

        #region public string GrantPermission(string userId, string result) 员工授予权限
        /// <summary>
        /// 员工授予权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantPermissionId">权限主键</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>主键</returns>
        public string GrantPermission(string systemCode, string userId, string grantPermissionId, string permissionCode)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            return GrantPermission(permissionScopeManager, systemCode, userId, grantPermissionId, permissionCode);
        }
        #endregion

        /// <summary>
        /// 授权权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <param name="grantPermissionIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantPermissiones(string systemCode, string userId, string[] grantPermissionIds, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            for (var i = 0; i < grantPermissionIds.Length; i++)
            {
                GrantPermission(permissionScopeManager, systemCode, userId, grantPermissionIds[i], permissionCode);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 授权权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="grantPermissionId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantPermissiones(string systemCode, string[] userIds, string grantPermissionId, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            for (var i = 0; i < userIds.Length; i++)
            {
                GrantPermission(permissionScopeManager, systemCode, userIds[i], grantPermissionId, permissionCode);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 撤回权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="grantPermissionIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantPermissions(string systemCode, string[] userIds, string[] grantPermissionIds, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            for (var i = 0; i < userIds.Length; i++)
            {
                for (var j = 0; j < grantPermissionIds.Length; j++)
                {
                    GrantPermission(permissionScopeManager, systemCode, userIds[i], grantPermissionIds[j], permissionCode);
                    result++;
                }
            }
            return result;
        }


        //
        //  撤销授权范围的实现部分
        //

        #region private int RevokePermission(BasePermissionScopeManager manager, string userId, string revokePermissionId, string permissionCode) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="permissionScopeManager">权限域读写器</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokePermissionId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        private int RevokePermission(BasePermissionScopeManager permissionScopeManager, string systemCode, string userId, string revokePermissionId, string permissionCode)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.CurrentTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseModuleEntity.CurrentTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, revokePermissionId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode))
            };
            return permissionScopeManager.Delete(parameters);
        }
        #endregion

        #region public int RevokePermission(string userId, string result) 员工撤销授权
        /// <summary>
        /// 员工撤销授权
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokePermissionId">权限主键</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>主键</returns>
        public int RevokePermission(string systemCode, string userId, string revokePermissionId, string permissionCode)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            return RevokePermission(permissionScopeManager, systemCode, userId, revokePermissionId, permissionCode);
        }
        #endregion

        /// <summary>
        /// 撤回权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <param name="revokePermissionIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokePermissions(string systemCode, string userId, string[] revokePermissionIds, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            for (var i = 0; i < revokePermissionIds.Length; i++)
            {
                RevokePermission(permissionScopeManager, systemCode, userId, revokePermissionIds[i], permissionCode);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 撤回权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="revokePermissionId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokePermissions(string systemCode, string[] userIds, string revokePermissionId, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            for (var i = 0; i < userIds.Length; i++)
            {
                RevokePermission(permissionScopeManager, systemCode, userIds[i], revokePermissionId, permissionCode);
                result++;
            }
            return result;
        }
        /// <summary>
        /// 撤回权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="revokePermissionIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokePermissions(string systemCode, string[] userIds, string[] revokePermissionIds, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            for (var i = 0; i < userIds.Length; i++)
            {
                for (var j = 0; j < revokePermissionIds.Length; j++)
                {
                    RevokePermission(permissionScopeManager, systemCode, userIds[i], revokePermissionIds[j], permissionCode);
                    result++;
                }
            }
            return result;
        }
    }
}