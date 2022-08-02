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
    /// 角色权限域
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
    public partial class BaseRoleScopeManager : BaseManager
    {
        ////
        ////
        //// 授权范围管理部分
        ////
        ////

        #region public string[] GetModuleIds(string roleId, string permissionCode) 获取员工的权限主键数组
        /// <summary>
        /// 获取员工的权限主键数组
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleId">员工主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetModuleIds(string systemCode, string roleId, string permissionCode)
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

        #region private string GrantModule(BasePermissionScopeManager manager, string id, string roleId, string grantModuleId) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="permissionScopeManager">权限域读写器</param>
        /// <param name="roleId">员工主键</param>
        /// <param name="grantModuleId">权限主键</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>主键</returns>
        private string GrantModule(string systemCode, BasePermissionScopeManager permissionScopeManager, string roleId, string grantModuleId, string permissionCode)
        {
            var result = string.Empty;
            var resourcePermissionScopeEntity = new BasePermissionScopeEntity
            {
                PermissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode).ToInt(),
                ResourceCategory = BaseRoleEntity.CurrentTableName,
                ResourceId = roleId.ToInt(),
                TargetCategory = BaseModuleEntity.CurrentTableName,
                TargetId = grantModuleId.ToInt(),
                Enabled = 1,
                Deleted = 0
            };
            return permissionScopeManager.Add(resourcePermissionScopeEntity);
        }
        #endregion

        #region public string GrantModule(string roleId, string result) 员工授予权限
        /// <summary>
        /// 员工授予权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleId">员工主键</param>
        /// <param name="grantModuleId">模块主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        public string GrantModule(string systemCode, string roleId, string grantModuleId, string permissionCode)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            return GrantModule(systemCode, permissionScopeManager, roleId, grantModuleId, permissionCode);
        }
        #endregion

        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <param name="grantModuleIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantModules(string systemCode, string roleId, string[] grantModuleIds, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < grantModuleIds.Length; i++)
            {
                GrantModule(systemCode, permissionScopeManager, roleId, grantModuleIds[i], permissionCode);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="grantModuleId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantModules(string systemCode, string[] roleIds, string grantModuleId, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < roleIds.Length; i++)
            {
                GrantModule(systemCode, permissionScopeManager, roleIds[i], grantModuleId, permissionCode);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="grantModuleIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantModules(string systemCode, string[] roleIds, string[] grantModuleIds, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < roleIds.Length; i++)
            {
                for (var j = 0; j < grantModuleIds.Length; j++)
                {
                    GrantModule(systemCode, permissionScopeManager, roleIds[i], grantModuleIds[j], permissionCode);
                    result++;
                }
            }
            return result;
        }


        //
        //  撤销授权范围的实现部分
        //

        #region private int RevokeModule(BasePermissionScopeManager manager, string roleId, string revokeModuleId, string permissionCode) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="permissionScopeManager">权限域读写器</param>
        /// <param name="roleId">员工主键</param>
        /// <param name="revokeModuleId">权限主键</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>主键</returns>
        private int RevokeModule(string systemCode, BasePermissionScopeManager permissionScopeManager, string roleId, string revokeModuleId, string permissionCode)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseRoleEntity.CurrentTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, roleId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseModuleEntity.CurrentTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, revokeModuleId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode))
            };
            return permissionScopeManager.Delete(parameters);
        }
        #endregion

        #region public int RevokeModule(string roleId, string revokeModuleId, string permissionCode) 员工撤销授权
        /// <summary>
        /// 员工撤销授权
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleId">员工主键</param>
        /// <param name="revokeModuleId"></param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        public int RevokeModule(string systemCode, string roleId, string revokeModuleId, string permissionCode)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            return RevokeModule(systemCode, permissionScopeManager, roleId, revokeModuleId, permissionCode);
        }
        #endregion

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <param name="revokeModuleIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeModules(string systemCode, string roleId, string[] revokeModuleIds, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < revokeModuleIds.Length; i++)
            {
                RevokeModule(systemCode, permissionScopeManager, roleId, revokeModuleIds[i], permissionCode);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="revokeModuleId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeModules(string systemCode, string[] roleIds, string revokeModuleId, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < roleIds.Length; i++)
            {
                RevokeModule(systemCode, permissionScopeManager, roleIds[i], revokeModuleId, permissionCode);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="revokeModuleIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeModules(string systemCode, string[] roleIds, string[] revokeModuleIds, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < roleIds.Length; i++)
            {
                for (var j = 0; j < revokeModuleIds.Length; j++)
                {
                    RevokeModule(systemCode, permissionScopeManager, roleIds[i], revokeModuleIds[j], permissionCode);
                    result++;
                }
            }

            return result;
        }
    }
}