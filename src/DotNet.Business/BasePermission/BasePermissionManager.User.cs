﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

using System.Data;
using System.Linq;

namespace DotNet.Business
{
    using Model;
    using System;
    using System.Collections.Generic;
    using Util;

    /// <summary>
    /// BasePermissionManager
    /// 资源权限管理，操作权限管理（这里实现了用户操作权限，角色的操作权限）
    /// 
    /// 修改记录
    ///
    ///     2010.09.21 版本：2.0 JiRiGaLa 智能权限判断、后台自动增加权限，增加并发锁PermissionLock。
    ///     2009.09.22 版本：1.1 JiRiGaLa 前台判断的权限，后台都需要记录起来，防止后台缺失前台的判断权限。
    ///     2008.03.28 版本：1.0 JiRiGaLa 创建主键。
    ///     
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2008.03.28</date>
    /// </author>
    /// </summary>
    public partial class BasePermissionManager : BaseManager
    {
        #region CheckPermission 判断用户是否有有相应的权限
        /// <summary>
        /// 判断用户是否有有相应的权限
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>有权限</returns>
        public bool CheckPermission(string systemCode, string userId, string permissionCode)
        {
            if (string.IsNullOrEmpty(systemCode))
            {
                return false;
            }

            if (!ValidateUtil.IsInt(userId))
            {
                return false;
            }

            var permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);
            // 没有找到相应的权限
            if (string.IsNullOrEmpty(permissionId))
            {
                return false;
            }

            CurrentTableName = systemCode + "Permission";
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldSystemCode, systemCode),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, BaseUserEntity.CurrentTableName),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 0),
                //宋彪注：permisssionId先没加上
                new KeyValuePair<string, object>(BasePermissionEntity.FieldPermissionId, permissionId)
            };
            return Exists(parameters);
        }

        #endregion

        #region ResetPermissionByCache 重置权限缓存

        /// <summary>
        /// 重置权限缓存
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户编号</param>
        /// <param name="roleId">角色编号</param>
        /// <returns></returns>
        public static string[] ResetPermissionByCache(string systemCode, string userId, string roleId)
        {
            if (ValidateUtil.IsInt(userId))
            {
                var key = "Permission:" + systemCode + ":User:" + userId;
                CacheUtil.Remove(key);
                return GetPermissionIdsByCache(systemCode, userId);
            }

            else if (ValidateUtil.IsInt(roleId))
            {
                var key = "Permission:" + systemCode + ":Role:" + roleId;
                CacheUtil.Remove(key);
                return GetPermissionIdsByCache(systemCode, new string[] { roleId });
            }
            return null;
        }

        #endregion

        #region GetPermissionIdsByCache 获取权限编号

        /// <summary>
        /// 获取权限编号
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string[] GetPermissionIdsByCache(string systemCode, string userId)
        {
            string[] result = null;

            var key = string.Empty;
            key = "Permission:" + systemCode + ":User:" + userId;

            result = CacheUtil.Cache(key, () => new BasePermissionManager().GetPermissionIds(systemCode, userId), true);

            return result;
        }

        #endregion

        #region public string[] GetPermissionIds(string systemCode, string userId) 获取用户的权限主键数组
        /// <summary>
        /// 获取用户的权限主键数组
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <returns>主键数组</returns>
        public string[] GetPermissionIds(string systemCode, string userId)
        {
            CurrentTableName = systemCode + "Permission";
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldSystemCode, systemCode),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, BaseUserEntity.CurrentTableName),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 0)
            };

            return GetProperties(parameters, BasePermissionEntity.FieldPermissionId);
        }
        #endregion

        #region 授予权限的实现部分

        #region public string GrantUser(string systemCode, string userId, string permissionId, bool chekExists = true) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionId">权限主键</param>
        /// <param name="chekExists">判断是否存在</param>
        /// <returns>主键</returns>
        public string GrantUser(string systemCode, string userId, string permissionId, bool chekExists = true)
        {
            var result = string.Empty;

            if (!ValidateUtil.IsInt(userId) && string.IsNullOrEmpty(permissionId))
            {
                return result;
            }

            CurrentTableName = systemCode + "Permission";

            var currentId = string.Empty;
            // 判断是否已经存在这个权限，若已经存在就不重复增加了
            if (chekExists)
            {
                var whereParameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionEntity.FieldSystemCode, systemCode),
                    new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, BaseUserEntity.CurrentTableName),
                    new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, userId),
                    new KeyValuePair<string, object>(BasePermissionEntity.FieldPermissionId, permissionId)
                };
                currentId = GetId(whereParameters);
                if (!string.IsNullOrEmpty(currentId))
                {
                    var parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 1),
                        new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 0),
                        new KeyValuePair<string, object>(BasePermissionEntity.FieldUpdateUserId, UserInfo.UserId),
                        new KeyValuePair<string, object>(BasePermissionEntity.FieldUpdateBy, UserInfo.RealName),
                        new KeyValuePair<string, object>(BasePermissionEntity.FieldUpdateTime, DateTime.Now)
                    };
                    // 更新状态，设置为有效、并取消删除，权限也不是天天变动的，所以可以更新一下
                    Update(currentId, parameters);

                    result = currentId;
                }
            }

            if (string.IsNullOrEmpty(currentId))
            {
                var permissionEntity = new BasePermissionEntity
                {
                    SystemCode = systemCode,
                    ResourceCategory = BaseUserEntity.CurrentTableName,
                    ResourceId = userId.ToString(),
                    PermissionId = permissionId,
                    Enabled = 1
                };
                // 2015-07-03 吉日嘎拉 若是没有公司相关的信息，就把公司区分出来，每个公司可以看每个公司的数据
                if (permissionEntity.CompanyId > 0)
                {
                    var entity = BaseUserManager.GetEntityByCache(userId);
                    if (entity != null)
                    {
                        permissionEntity.CompanyId = entity.CompanyId;
                        permissionEntity.CompanyName = entity.CompanyName;
                    }
                }

                var permissionManager = new BasePermissionManager(DbHelper, UserInfo, CurrentTableName);
                result = permissionManager.Add(permissionEntity, true, false);
            }

            // 2015-09-21 吉日嘎拉 这里增加变更日志
            var tableName = systemCode + "UserPermission";
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginInsert(BaseChangeLogEntity.CurrentTableName);
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldTableName, tableName);
            if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
            {
                sqlBuilder.SetFormula(BaseChangeLogEntity.FieldId, BaseChangeLogEntity.CurrentTableName + "_SEQ.NEXTVAL");
            }
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldRecordKey, userId);
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldColumnName, "授权");
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldColumnDescription, new BaseModuleManager().GetNameByCache(systemCode, permissionId));
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldOldValue, null);
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldNewValue, permissionId);
            sqlBuilder.EndInsert();

            return result;
        }
        #endregion

        #region public string GrantByPermissionCode(string systemCode, string userId, string permissionCode) 用户授予权限
        /// <summary>
        /// 用户授予权限
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        public string GrantByPermissionCode(string systemCode, string userId, string permissionCode)
        {
            var result = string.Empty;

            var permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);
            if (!string.IsNullOrEmpty(permissionId))
            {
                result = GrantUser(systemCode, userId, permissionId);
            }

            return result;
        }
        #endregion

        #region public int GrantUser(string systemCode, string userId, string[] permissionIds) 授权
        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        public int GrantUser(string systemCode, string userId, string[] permissionIds)
        {
            var result = 0;

            for (var i = 0; i < permissionIds.Length; i++)
            {
                GrantUser(systemCode, userId, permissionIds[i]);
                result++;
            }

            return result;
        }
        #endregion

        #region public int GrantUser(string systemCode, string[] userIds, string permissionId) 授权
        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public int GrantUser(string systemCode, string[] userIds, string permissionId)
        {
            var result = 0;

            for (var i = 0; i < userIds.Length; i++)
            {
                GrantUser(systemCode, userIds[i], permissionId);
                result++;
            }

            return result;
        }
        #endregion

        #region public int GrantUser(string systemCode, string[] userIds, string[] permissionIds) 授权
        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        public int GrantUser(string systemCode, string[] userIds, string[] permissionIds)
        {
            var result = 0;

            for (var i = 0; i < userIds.Length; i++)
            {
                for (var j = 0; j < permissionIds.Length; j++)
                {
                    GrantUser(systemCode, userIds[i], permissionIds[j]);
                    result++;
                }
            }

            return result;
        }
        #endregion

        #endregion

        #region 撤销权限的实现部分

        #region public int Revoke(string systemCode, string userId, string permissionId) 为了提高撤销的运行速度
        /// <summary>
        /// 为了提高撤销的运行速度
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>影响行数</returns>
        public int RevokeUser(string systemCode, string userId, string permissionId)
        {
            var result = 0;

            if (!ValidateUtil.IsInt(userId) && string.IsNullOrEmpty(permissionId))
            {
                return result;
            }

            CurrentTableName = systemCode + "Permission";

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldSystemCode, systemCode),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, BaseUserEntity.CurrentTableName),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldPermissionId, permissionId)
            };
            // 伪删除、数据有冗余，但是有历史记录
            // result = permissionManager.SetDeleted(parameters);
            // 真删除、执行效率高、但是没有历史记录
            result = Delete(parameters);

            // 2015-09-21 吉日嘎拉 这里增加变更日志
            var tableName = systemCode + "UserPermission";
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginInsert(BaseChangeLogEntity.CurrentTableName);
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldTableName, tableName);
            if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
            {
                sqlBuilder.SetFormula(BaseChangeLogEntity.FieldId, BaseChangeLogEntity.CurrentTableName + "_SEQ.NEXTVAL");
            }
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldRecordKey, userId);
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldColumnName, "撤销授权");
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldColumnDescription, new BaseModuleManager().GetNameByCache(systemCode, permissionId));
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldOldValue, "1");
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldNewValue, permissionId);
            sqlBuilder.EndInsert();

            return result;
        }
        #endregion

        #region public int RevokeByPermissionCode(string systemCode, string userId, string permissionCode) 用户授予权限
        /// <summary>
        /// 用户授予权限
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响行数</returns>
        public int RevokeByPermissionCode(string systemCode, string userId, string permissionCode)
        {
            var result = 0;

            var permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);
            if (!string.IsNullOrEmpty(permissionId))
            {
                result = RevokeUser(systemCode, userId, permissionId);
            }

            return result;
        }
        #endregion

        #region public int RevokeUser(string systemCode, string userId, string[] permissionIds) 撤回
        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        public int RevokeUser(string systemCode, string userId, string[] permissionIds)
        {
            var result = 0;

            for (var i = 0; i < permissionIds.Length; i++)
            {
                result += RevokeUser(systemCode, userId, permissionIds[i]);
            }

            return result;
        }
        #endregion

        #region public int RevokeUser(string systemCode, string[] userIds, string permissionId) 撤回

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public int RevokeUser(string systemCode, string[] userIds, string permissionId)
        {
            var result = 0;

            for (var i = 0; i < userIds.Length; i++)
            {
                result += RevokeUser(systemCode, userIds[i], permissionId);
            }

            return result;
        }
        #endregion

        #region public int RevokeUser(string systemCode, string[] userIds, string[] permissionIds) 撤回
        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        public int RevokeUser(string systemCode, string[] userIds, string[] permissionIds)
        {
            var result = 0;

            for (var i = 0; i < userIds.Length; i++)
            {
                for (var j = 0; j < permissionIds.Length; j++)
                {
                    result += RevokeUser(systemCode, userIds[i], permissionIds[j]);
                }
            }

            return result;
        }
        #endregion

        #region public int RevokeUserAll(string systemCode, string userId) 撤回所有
        /// <summary>
        /// 撤回所有
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int RevokeUserAll(string systemCode, string userId)
        {
            var result = 0;

            CurrentTableName = systemCode + "Permission";
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldSystemCode, systemCode),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, BaseUserEntity.CurrentTableName),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, userId)
            };
            result = Delete(parameters);

            return result;
        }
        #endregion

        #endregion

        #region public string[] GetUserIds(string systemCode, string permissionCode) 获得有某个权限的所有用户主键

        /// <summary>
        /// 获得有某个权限的所有用户主键
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="permissionCode">操作权限编号</param>
        /// <returns>用户主键数组</returns>
        public string[] GetUserIds(string systemCode, string permissionCode)
        {
            // 若不存在就需要自动能增加一个操作权限项
            var permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);
            return GetUserIdsByPermissionId(systemCode, permissionId);
        }

        #endregion

        #region public string[] GetUserIdsByPermissionId(string systemCode, string permissionId) 获取用户编号

        /// <summary>
        /// 获取用户编号
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public string[] GetUserIdsByPermissionId(string systemCode, string permissionId)
        {
            DataTable dt = null;
            string[] result = null;
            if (!string.IsNullOrEmpty(permissionId))
            {
                var tableName = systemCode + "Permission";
                var sql = string.Empty;

                // 1.本人直接就有某个操作权限的。
                sql = "SELECT ResourceId FROM " + tableName + " WHERE (ResourceCategory = '" + systemCode + "User') AND (PermissionId = " + permissionId + ") AND (" + BaseModuleEntity.FieldDeleted + " = 0) AND (" + BaseUtil.FieldEnabled + " = 1)";
                dt = Fill(sql);
                var userIds = BaseUtil.FieldToArray(dt, BasePermissionEntity.FieldResourceId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();

                // 2.角色本身就有某个操作权限的。
                sql = "SELECT ResourceId FROM " + tableName + " WHERE (ResourceCategory = '" + systemCode + "Role') AND (PermissionId = " + permissionId + ") AND (" + BaseModuleEntity.FieldDeleted + " = 0) AND (" + BaseUtil.FieldEnabled + " = 1)";
                dt = Fill(sql);
                var roleIds = StringUtil.Concat(result, BaseUtil.FieldToArray(dt, BasePermissionEntity.FieldResourceId)).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();

                // 3.组织机构有某个操作权限。。
                sql = "SELECT ResourceId FROM " + tableName + " WHERE (ResourceCategory = '" + systemCode + "Organization') AND (PermissionId = " + permissionId + ") AND (" + BaseModuleEntity.FieldDeleted + " = 0) AND (" + BaseUtil.FieldEnabled + " = 1)";
                dt = Fill(sql);
                var organizationIds = StringUtil.Concat(result, BaseUtil.FieldToArray(dt, BasePermissionEntity.FieldResourceId)).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();

                // 4.获取所有有这个操作权限的用户Id，而且这些用户是有效的。
                var userManager = new BaseUserManager(DbHelper, UserInfo);
                result = userManager.GetUserIds(userIds, organizationIds, roleIds);
            }
            return result;
        }

        #endregion
    }
}