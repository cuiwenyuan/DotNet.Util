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
    /// 角色操作权限域
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

        #region public string[] GetPermissionIds(string roleId, string permissionCode) 获取员工的权限主键数组
        /// <summary>
        /// 获取员工的权限主键数组
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleId">员工主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetPermissionIds(string systemCode, string roleId, string permissionCode)
        {
            string[] result = null;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseRoleEntity.CurrentTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, roleId),
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

        #region private string GrantPermission(BasePermissionScopeManager manager, string id, string roleId, string grantPermissionId) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="permissionScopeManager">权限域读写器</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="grantPermissionId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        private string GrantPermission(string systemCode, BasePermissionScopeManager permissionScopeManager, string roleId, string grantPermissionId, string permissionCode)
        {
            var result = string.Empty;
            var resourcePermissionScopeEntity = new BasePermissionScopeEntity
            {
                PermissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode).ToInt(),
                ResourceCategory = BaseRoleEntity.CurrentTableName,
                ResourceId = roleId.ToInt(),
                TargetCategory = BaseModuleEntity.CurrentTableName,
                TargetId = grantPermissionId.ToInt(),
                Enabled = 1,
                Deleted = 0
            };
            return permissionScopeManager.Add(resourcePermissionScopeEntity);
        }
        #endregion

        #region public string GrantPermission(string roleId, string result) 员工授予权限
        /// <summary>
        /// 员工授予权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleId">员工主键</param>
        /// <param name="grantPermissionId"></param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        public string GrantPermission(string systemCode, string roleId, string grantPermissionId, string permissionCode)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            return GrantPermission(systemCode, permissionScopeManager, roleId, grantPermissionId, permissionCode);
        }
        #endregion

        /// <summary>
        /// 授权权限
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <param name="grantPermissionIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantPermissiones(string systemCode, string roleId, string[] grantPermissionIds, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            for (var i = 0; i < grantPermissionIds.Length; i++)
            {
                GrantPermission(systemCode, permissionScopeManager, roleId, grantPermissionIds[i], permissionCode);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 授权权限
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="grantPermissionId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantPermissiones(string systemCode, string[] roleIds, string grantPermissionId, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            for (var i = 0; i < roleIds.Length; i++)
            {
                GrantPermission(systemCode, permissionScopeManager, roleIds[i], grantPermissionId, permissionCode);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 授权权限
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="grantPermissionIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantPermissions(string systemCode, string[] roleIds, string[] grantPermissionIds, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            for (var i = 0; i < roleIds.Length; i++)
            {
                for (var j = 0; j < grantPermissionIds.Length; j++)
                {
                    GrantPermission(systemCode, permissionScopeManager, roleIds[i], grantPermissionIds[j], permissionCode);
                    result++;
                }
            }

            return result;
        }


        //
        //  撤销授权范围的实现部分
        //

        #region private int RevokePermission(BasePermissionScopeManager manager, string roleId, string permissionCode, string revokePermissionId) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="permissionScopeManager">权限域读写器</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="revokePermissionId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        private int RevokePermission(string systemCode, BasePermissionScopeManager permissionScopeManager, string roleId, string revokePermissionId, string permissionCode)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseRoleEntity.CurrentTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, roleId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseModuleEntity.CurrentTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, revokePermissionId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode))
            };
            return permissionScopeManager.Delete(parameters);
        }
        #endregion

        #region public int RevokePermission(string roleId, string result) 员工撤销授权
        /// <summary>
        /// 员工撤销授权
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="revokePermissionId"></param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        public int RevokePermission(string systemCode, string roleId, string revokePermissionId, string permissionCode)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            return RevokePermission(systemCode, permissionScopeManager, roleId, revokePermissionId, permissionCode);
        }
        #endregion

        /// <summary>
        /// 撤回权限
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <param name="revokePermissionIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokePermissions(string systemCode, string roleId, string[] revokePermissionIds, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            for (var i = 0; i < revokePermissionIds.Length; i++)
            {
                RevokePermission(systemCode, permissionScopeManager, roleId, revokePermissionIds[i], permissionCode);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 撤回权限
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="revokePermissionId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokePermissions(string systemCode, string[] roleIds, string revokePermissionId, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            for (var i = 0; i < roleIds.Length; i++)
            {
                RevokePermission(systemCode, permissionScopeManager, roleIds[i], revokePermissionId, permissionCode);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 撤回权限
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="revokePermissionIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokePermissions(string systemCode, string[] roleIds, string[] revokePermissionIds, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            for (var i = 0; i < roleIds.Length; i++)
            {
                for (var j = 0; j < revokePermissionIds.Length; j++)
                {
                    RevokePermission(systemCode, permissionScopeManager, roleIds[i], revokePermissionIds[j], permissionCode);
                    result++;
                }
            }
            return result;
        }
    }
}