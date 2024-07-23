//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
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

            CurrentTableName = GetPermissionTableName(systemCode);
            var resourceCategory = GetRoleTableName(systemCode);
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldSystemCode, systemCode),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, resourceCategory),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldPermissionId, permissionId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 0)
            };

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

            CurrentTableName = GetPermissionTableName(systemCode);
            var permissionManager = new BasePermissionManager(DbHelper, UserInfo, CurrentTableName);

            var tableName = GetRoleTableName(systemCode);

            // 判断是否已经存在这个权限，若已经存在就不重复增加了
            if (chekExists)
            {
                var whereParameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionEntity.FieldSystemCode, systemCode),
                    new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, tableName),
                    new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, roleId),
                    new KeyValuePair<string, object>(BasePermissionEntity.FieldPermissionId, permissionId)
                };
                result = permissionManager.GetId(whereParameters);
                if (!string.IsNullOrEmpty(result))
                {
                    var parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 1),
                        new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 0),
                    };
                    // 更新状态，设置为有效、并取消删除，权限也不是天天变动的，所以可以更新一下
                    permissionManager.Update(result, parameters);
                }
            }

            if (string.IsNullOrEmpty(result))
            {
                var permissionEntity = new BasePermissionEntity
                {
                    SystemCode = systemCode,
                    ResourceCategory = tableName,
                    ResourceId = roleId,
                    PermissionId = permissionId
                };

                result = permissionManager.Add(permissionEntity);
            }
            #region 记录日志

            var baseChangeLogEntity = new BaseChangeLogEntity
            {
                SystemCode = systemCode,
                TableName = GetPermissionTableName(systemCode),
                TableDescription = CurrentTableDescription,
                RecordKey = result,
                OldValue = null,
                NewValue = "{" + BasePermissionEntity.FieldResourceId + ":" + roleId + "," + BasePermissionEntity.FieldPermissionId + ":" + permissionId + "}",
                SortCode = 1 // 不要排序了，加快写入速度
            };
            new BaseChangeLogManager(UserInfo).Add(baseChangeLogEntity, true, false);

            #endregion

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
            if (!ValidateUtil.IsInt(roleId) && string.IsNullOrEmpty(permissionId))
            {
                return result;
            }

            CurrentTableName = GetPermissionTableName(systemCode);
            var permissionManager = new BasePermissionManager(DbHelper, UserInfo, CurrentTableName);

            var tableName = GetPermissionTableName(systemCode);

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldSystemCode, systemCode),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, GetRoleTableName(systemCode)),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, roleId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldPermissionId, permissionId)
            };
            var id = permissionManager.GetId(parameters);
            if (!id.IsNullOrEmpty())
            {
                // 伪删除、数据有冗余，但是有历史记录
                result = permissionManager.SetDeleted(id);
                // 真删除、执行效率高、但是没有历史记录
                //result = permissionManager.Delete(id);

                #region 记录日志

                var baseChangeLogEntity = new BaseChangeLogEntity
                {
                    SystemCode = systemCode,
                    TableName = GetPermissionTableName(systemCode),
                    TableDescription = CurrentTableDescription,
                    RecordKey = id,
                    ColumnName = BaseChangeLogEntity.FieldDeleted,
                    ColumnDescription = "删除",
                    OldValue = "0",
                    NewValue = "1",
                    SortCode = 1 // 不要排序了，加快写入速度
                };
                new BaseChangeLogManager(UserInfo).Add(baseChangeLogEntity, true, false);

                #endregion
            }
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
            var tableName = GetPermissionTableName(systemCode);
            var permissionManager = new BasePermissionManager(DbHelper, UserInfo, tableName);
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldSystemCode, systemCode),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, GetRoleTableName(systemCode)),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, roleId)
            };
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 0),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 1)
            };
            return Update(whereParameters, parameters);
        }
        #endregion

        #endregion

        #region 复制用户权限到新用户
        /// <summary>
        /// 复制用户权限到新用户
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="referenceUserId">源用户编号</param>
        /// <param name="targetUserId">目标用户编号</param>
        /// <returns></returns>
        public int CopyRolePermission(string systemCode, int referenceUserId, int targetUserId)
        {
            var result = 0;
            var tableName = GetPermissionTableName(systemCode);
            var permissionManager = new BasePermissionManager(DbHelper, UserInfo, tableName);

            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldSystemCode, systemCode),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, GetRoleTableName(systemCode)),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, referenceUserId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 0),
            };
            var ls = permissionManager.GetList<BasePermissionEntity>(whereParameters, order: BasePermissionEntity.FieldCreateTime + " ASC");
            if (ls != null)
            {
                foreach (var item in ls)
                {
                    item.ResourceId = targetUserId.ToString();
                    if (permissionManager.AddOrActive(item).IsNullOrEmpty())
                    {
                        result++;
                    }
                }
                //运行成功
                Status = Status.OkAdd;
                StatusCode = Status.OkAdd.ToString();
                StatusMessage = Status.OkAdd.ToDescription();
            }
            else
            {
                //未找到记录
                Status = Status.NotFound;
                StatusCode = Status.NotFound.ToString();
                StatusMessage = Status.NotFound.ToDescription();
            }

            return result;
        }
        #endregion
    }
}