//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BasePermissionManager
    /// 角色权限
    /// 
    /// 修改记录
    ///
    ///     2016.02.25 版本：2.2 JiRiGaLa 通过缓存获取权限的方法改进。
    ///     2016.01.06 版本：2.1 JiRiGaLa 分 systemCode 进行代码整顿。
    ///     2013.05.27 版本：2.0 JiRiGaLa BasePermissionEntity.FieldResourceCategory, tableName，写入准确的对象表名
    ///     2010.04.23 版本：1.1 JiRiGaLa Enabled 不允许为NULL的错误解决。
    ///     2008.05.23 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.02.25</date>
    /// </author>
    /// </summary>
    public partial class BasePermissionManager : BaseManager
    {
        #region public static string[] GetPermissionIdsByCache(string systemCode, string[] roleIds)
        /// <summary>
        /// 多个角色，都有啥权限？单个角色都有啥权限的循环获取？
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleIds">角色主键数组</param>
        /// <returns>权限数组</returns>
        public static string[] GetPermissionIdsByCache(string systemCode, string[] roleIds)
        {
            string[] result = null;

            var key = string.Empty;
            var roleId = string.Empty;

            string[] permissionIds = null;

            key = "Permission:" + systemCode + ":Role:" + roleId;
            var hs = new HashSet<string>();
            result = CacheUtil.Cache(key, () =>
            {
                for (var i = 0; i < roleIds.Length; i++)
                {
                    permissionIds = new BasePermissionManager().GetPermissionIds(systemCode, roleIds[i], "Role");
                    foreach (var permissionId in permissionIds)
                    {
                        hs.Add(permissionId);
                    }
                }

                return hs.ToArray();
            }, true);

            return result;
        }

        #endregion

        #region public string[] GetRoleIds(string systemCode, string permissionId) 获取角色主键数组
        /// <summary>
        /// 获取角色主键数组
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="permissionId">操作权限(模块主键)</param>
        /// <returns>主键数组</returns>
        public string[] GetRoleIds(string systemCode, string permissionId)
        {
            string[] result = null;

            CurrentTableName = systemCode + "Permission";
            var parameters = new List<KeyValuePair<string, object>>();
            var resourceCategory = systemCode + "Role";
            parameters.Add(new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, resourceCategory));
            parameters.Add(new KeyValuePair<string, object>(BasePermissionEntity.FieldPermissionId, permissionId));
            parameters.Add(new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 1));
            parameters.Add(new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 0));

            result = GetProperties(parameters, BasePermissionEntity.FieldResourceId);
            return result;
        }
        #endregion

        #region 授予权限的实现部分

        #region public string GrantRole(string systemCode, string roleId, string permissionId, bool chekExists = true) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionId">权限主键</param>
        /// <param name="chekExists"></param>
        /// <returns>主键</returns>
        public string GrantRole(string systemCode, string roleId, string permissionId, bool chekExists = true)
        {
            var result = string.Empty;

            var currentId = string.Empty;

            CurrentTableName = systemCode + "Permission";

            var tableName = systemCode + "Role";

            // 判断是否已经存在这个权限，若已经存在就不重复增加了
            if (chekExists)
            {
                var whereParameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, tableName),
                    new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, roleId),
                    new KeyValuePair<string, object>(BasePermissionEntity.FieldPermissionId, permissionId)
                };
                currentId = GetId(whereParameters);
                if (!string.IsNullOrEmpty(currentId))
                {
                    var parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 1),
                        new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 0),
                    };
                    // 更新状态，设置为有效、并取消删除，权限也不是天天变动的，所以可以更新一下
                    UpdateProperty(currentId, parameters);
                }
            }

            if (string.IsNullOrEmpty(currentId))
            {
                var permissionEntity = new BasePermissionEntity
                {
                    ResourceCategory = tableName,
                    ResourceId = roleId,
                    PermissionId = permissionId
                };
                var permissionManager = new BasePermissionManager(DbHelper, UserInfo, CurrentTableName);
                result = permissionManager.Add(permissionEntity, true, false);
            }

            // 2015-09-21 吉日嘎拉 这里增加变更日志
            tableName = systemCode + "RolePermission";
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginInsert(BaseChangeLogEntity.CurrentTableName);
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldTableName, tableName);
            if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
            {
                sqlBuilder.SetFormula(BaseChangeLogEntity.FieldId, BaseChangeLogEntity.CurrentTableName + "_SEQ.NEXTVAL");
            }
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldRecordKey, roleId);
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldColumnName, "授权");
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldColumnDescription, new BaseModuleManager().GetNameByCache(systemCode, permissionId));
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldOldValue, null);
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldNewValue, permissionId);
            sqlBuilder.EndInsert();

            return result;
        }
        #endregion

        #region public int GrantRole(string systemCode, string roleId, string[] permissionIds) 授权
        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        public int GrantRole(string systemCode, string roleId, string[] permissionIds)
        {
            var result = 0;
            for (var i = 0; i < permissionIds.Length; i++)
            {
                GrantRole(systemCode, roleId, permissionIds[i]);
                result++;
            }
            return result;
        }
        #endregion

        #region public int GrantRole(string systemCode, string[] roleIds, string permissionId) 授权
        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public int GrantRole(string systemCode, string[] roleIds, string permissionId)
        {
            var result = 0;
            for (var i = 0; i < roleIds.Length; i++)
            {
                GrantRole(systemCode, roleIds[i], permissionId);
                result++;
            }
            return result;
        }
        #endregion

        #region public int GrantRole(string systemCode, string[] roleIds, string[] permissionIds) 授权
        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        public int GrantRole(string systemCode, string[] roleIds, string[] permissionIds)
        {
            var result = 0;

            for (var i = 0; i < roleIds.Length; i++)
            {
                for (var j = 0; j < permissionIds.Length; j++)
                {
                    GrantRole(systemCode, roleIds[i], permissionIds[j]);
                    result++;
                }
            }

            return result;
        }
        #endregion

        #endregion

        #region 撤销权限的实现部分

        #region public int RevokeRole(string systemCode, string roleId, string permissionId) 为了提高撤销的运行速度
        /// <summary>
        /// 为了提高撤销的运行速度
        /// </summary>
        /// <param name="systemCode">资源权限读写器</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>影响行数</returns>
        public int RevokeRole(string systemCode, string roleId, string permissionId)
        {
            var result = 0;

            var tableName = systemCode + "Permission";
            var permissionManager = new BasePermissionManager(DbHelper, UserInfo, tableName);

            tableName = systemCode + "Role";
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, tableName),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, roleId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldPermissionId, permissionId)
            };

            // 2015-09-21 吉日嘎拉 这里增加变更日志
            tableName = systemCode + "RolePermission";
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginInsert(BaseChangeLogEntity.CurrentTableName);
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldTableName, tableName);
            if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
            {
                sqlBuilder.SetFormula(BaseChangeLogEntity.FieldId, BaseChangeLogEntity.CurrentTableName + "_SEQ.NEXTVAL");
            }
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldRecordKey, roleId);
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldColumnName, "撤销授权");
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldColumnDescription, new BaseModuleManager().GetNameByCache(systemCode, permissionId));
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldOldValue, "1");
            sqlBuilder.SetValue(BaseChangeLogEntity.FieldNewValue, permissionId);
            sqlBuilder.EndInsert();

            // 伪删除、数据有冗余，但是有历史记录
            result = permissionManager.SetDeleted(parameters);
            // 真删除、执行效率高、但是没有历史记录
            //result = permissionManager.Delete(parameters);

            return result;
        }
        #endregion

        #region public int RevokeRole(string systemCode, string roleId, string[] permissionIds) 撤回
        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        public int RevokeRole(string systemCode, string roleId, string[] permissionIds)
        {
            var result = 0;
            for (var i = 0; i < permissionIds.Length; i++)
            {
                result += RevokeRole(systemCode, roleId, permissionIds[i]);
            }
            return result;
        }
        #endregion

        #region public int RevokeRole(string systemCode, string[] roleIds, string permissionId) 撤回

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public int RevokeRole(string systemCode, string[] roleIds, string permissionId)
        {
            var result = 0;
            for (var i = 0; i < roleIds.Length; i++)
            {
                result += RevokeRole(systemCode, roleIds[i], permissionId);
            }
            return result;
        }

        #endregion

        #region public int RevokeRole(string systemCode, string[] roleIds, string[] permissionIds) 撤回
        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        public int RevokeRole(string systemCode, string[] roleIds, string[] permissionIds)
        {
            var result = 0;
            for (var i = 0; i < roleIds.Length; i++)
            {
                for (var j = 0; j < permissionIds.Length; j++)
                {
                    result += RevokeRole(systemCode, roleIds[i], permissionIds[j]);
                }
            }
            return result;
        }
        #endregion

        #region public int RevokeRoleAll(string systemCode, string roleId) 撤销角色全部权限
        /// <summary>
        /// 撤销角色全部权限
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>影响行数</returns>
        public int RevokeRoleAll(string systemCode, string roleId)
        {
            var tableName = systemCode + "Permission";
            var permissionManager = new BasePermissionManager(DbHelper, UserInfo, tableName);
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, systemCode + "Role"),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, roleId)
            };
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 0),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 1)
            };
            return UpdateProperty(whereParameters, parameters);
        }
        #endregion

        #endregion
    }
}