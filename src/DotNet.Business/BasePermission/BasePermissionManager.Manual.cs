//-----------------------------------------------------------------------
// <copyright file="BasePermissionManager.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BasePermissionManager
    /// 资源权限管理，操作权限管理（这里实现了用户操作权限，角色的操作权限）
    /// 
    /// 修改记录
    ///
    ///     2021.12.06 版本：1.0 Troy.Cui 创建。
    ///     
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2021.12.06</date>
    /// </author>
    /// </summary>
    public partial class BasePermissionManager : BaseManager
    {
        #region public static List<BaseModuleEntity> GetUserPermissionList(BaseUserInfo userInfo, string userId = null) 获用户拥有的操作权限列表
        /// <summary>
        /// 获用户拥有的操作权限列表
        /// </summary>
        /// <param name="userInfo">当前操作员</param>
        /// <param name="userId">用户主键</param>
        /// <param name="systemCode">子系统编码</param>
        public List<BaseModuleEntity> GetUserPermissionList(BaseUserInfo userInfo, string userId = null, string systemCode = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = userInfo.Id.ToString();
            }
            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = BaseSystemInfo.SystemCode;
            }
            var cacheKey = "P" + userId;
            List<BaseModuleEntity> entityList = null;
            // 这里是控制用户并发的，减少框架等重复读取数据库的效率问题
            lock (BaseSystemInfo.UserLock)
            {
                //var cacheTime = default(TimeSpan);
                var cacheTime = TimeSpan.FromMilliseconds(3600000);
                entityList = CacheUtil.Cache(cacheKey, () => GetPermissionListByUser(systemCode, userInfo.Id.ToString(), userInfo.CompanyId, true), true);
            }
            //LogUtil.WriteLog(JsonUtil.ObjectToJson(entityList));
            return entityList;
        }
        #endregion

        #region 新增或激活 AddOrActive
        /// <summary>
        /// 新增或激活
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public string AddOrActive(BasePermissionEntity entity)
        {
            var result = string.Empty;
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, entity.ResourceCategory),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, entity.ResourceId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldPermissionId, entity.PermissionId)
            };
            var entityOld = GetEntity(whereParameters);
            if (entityOld != null)
            {
                result = entityOld.Id.ToString();
                entity.Id = entityOld.Id;
                entity.Enabled = 1;
                entity.Deleted = 0;
                //激活
                UpdateEntity(entity);
            }
            else
            {
                result = AddEntity(entity);
                if (!string.IsNullOrEmpty(result))
                {
                    //运行成功
                    StatusCode = Status.OkAdd.ToString();
                    StatusMessage = Status.OkAdd.ToDescription();
                }
                else
                {
                    //保存失败
                    StatusCode = Status.DbError.ToString();
                    StatusMessage = Status.DbError.ToDescription();
                }
            }

            return result;
        }
        #endregion

        #region 获取角色拥有的权限列表
        /// <summary>
        /// 获取角色拥有的权限列表
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="resourceCategory">资源类别（User,Role）</param>
        /// <returns>列表</returns>
        public List<BaseModuleEntity> GetPermissionList(string systemCode, string roleId, string resourceCategory)
        {
            List<BaseModuleEntity> result = null;

            var permissionIds = GetPermissionIds(systemCode, roleId, resourceCategory);
            if (permissionIds != null && permissionIds.Length > 0)
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseModuleEntity.FieldId, permissionIds),
                    new KeyValuePair<string, object>(BaseModuleEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseModuleEntity.FieldDeleted, 0)
                };

                var tableName = systemCode + "Module";
                var moduleManager = new BaseModuleManager(DbHelper, UserInfo, tableName);
                result = moduleManager.GetList<BaseModuleEntity>(parameters);
            }

            return result;
        }

        #endregion

        #region public string[] GetPermissionIds(string systemCode, string roleId, string resourceCategory) 获取角色的权限主键数组
        /// <summary>
        /// 获取角色的权限主键数组
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="resourceCategory">资源类别（User,Role）</param>
        /// <returns>主键数组</returns>
        public string[] GetPermissionIds(string systemCode, string roleId, string resourceCategory)
        {
            string[] result = null;

            CurrentTableName = systemCode + "Permission";
            resourceCategory = systemCode + resourceCategory;
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, resourceCategory),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, roleId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 0)
            };

            result = GetProperties(parameters, BasePermissionEntity.FieldPermissionId);

            return result;
        }
        #endregion

    }
}