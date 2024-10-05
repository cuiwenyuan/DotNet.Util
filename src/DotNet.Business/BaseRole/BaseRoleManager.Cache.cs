//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <remarks>
    /// BaseRoleManager
    /// 角色缓存
    /// 
    /// 修改记录
    /// 
    ///     2015.12.10 版本：1.0 JiRiGaLa  创建。
    ///	
    /// <author>  
    ///		<name>Troy.Cui</name>
    ///		<date>2015.12.10</date>
    /// </author> 
    /// </remarks>
    public partial class BaseRoleManager
    {
        #region public override bool RemoveCache() 删除缓存

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <returns></returns>
        public override bool RemoveCache()
        {
            var result = false;
            var cacheKey = "Dt." + CurrentTableName;
            var cacheKeyListBase = "List.Base.Role";
            var cacheKeyListSystemCode = "List.Base.Role";
            if (UserInfo != null)
            {
                //cacheKey += "." + UserInfo.CompanyId;
                cacheKeyListSystemCode = "List." + BaseSystemInfo.SystemCode + ".Role";
            }

            CacheUtil.Remove(cacheKeyListBase);
            CacheUtil.Remove(cacheKeyListSystemCode);
            result = CacheUtil.Remove(cacheKey);
            return result;
        }
        #endregion

        #region public static BaseRoleEntity GetCacheByKey(string key) 从缓存中获取
        /// <summary>
        /// 从缓存中获取
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static BaseRoleEntity GetCacheByKey(string key)
        {
            BaseRoleEntity result = null;

            if (!string.IsNullOrWhiteSpace(key))
            {
                result = CacheUtil.Get<BaseRoleEntity>(key);
            }

            return result;
        }

        #endregion

        #region private static void SetCache(string systemCode, BaseRoleEntity entity) 设置缓存
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="entity"></param>
        private static void SetCache(string systemCode, BaseRoleEntity entity)
        {
            if (string.IsNullOrWhiteSpace(systemCode))
            {
                systemCode = "Base";
            }

            if (entity != null && !string.IsNullOrEmpty(entity.Id.ToString()))
            {
                var key = string.Empty;
                key = GetRoleTableName(systemCode) + "." + entity.Id;
                CacheUtil.Set<BaseRoleEntity>(key, entity);

                key = GetRoleTableName(systemCode) + "." + entity.Code;
                CacheUtil.Set<BaseRoleEntity>(key, entity);
            }
        }

        #endregion

        #region public static void ClearCache() 清除缓存
        /// <summary>
        /// 清除缓存
        /// </summary>
        public static void ClearCache()
        {
        }
        #endregion

        /// <summary>
        /// 缓存预热,强制重新缓存
        /// </summary>
        /// <returns></returns>
        public static int CachePreheating()
        {
            var result = 0;

            var systemCodes = BaseSystemManager.GetSystemCodes();
            foreach (var entity in systemCodes)
            {
                GetEntitiesByCache(entity.ItemKey, true);
                result += CachePreheating(entity.ItemKey);
            }

            return result;
        }

        /// <summary>
        /// 缓存预热,强制重新缓存
        /// 2016-02-26 每个角色的权限也进行缓存起来
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <returns>影响行数</returns>
        public static int CachePreheating(string systemCode)
        {
            var result = 0;

            // 把所有的组织机构都缓存起来的代码
            var roleTableName = GetRoleTableName(systemCode);
            var manager = new BaseRoleManager
            {
                CurrentTableName = roleTableName
            };
            var dataReader = manager.ExecuteReader();
            if (dataReader != null && !dataReader.IsClosed)
            {
                while (dataReader.Read())
                {
                    var entity = BaseEntity.Create<BaseRoleEntity>(dataReader, false);
                    if (entity != null)
                    {
                        // 设置角色本身的缓存
                        SetCache(systemCode, entity);
                        // 重置权限缓存数据
                        BasePermissionManager.ResetPermissionByCache(systemCode, null, entity.Id.ToString());
                        result++;
                        System.Console.WriteLine(result + " : " + entity.Name);
                    }
                }

                dataReader.Close();
            }

            return result;
        }
        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static int RefreshCache(string systemCode, string roleId)
        {
            var result = 0;

            // 2016-02-29 吉日嘎拉 强制刷新缓存
            var roleEntity = GetEntityByCache(systemCode, roleId, true);
            if (roleEntity != null)
            {
                var systemCodes = BaseSystemManager.GetSystemCodes();
                foreach (var entity in systemCodes)
                {
                    GetEntitiesByCache(entity.ItemKey, true);
                    result += CachePreheating(entity.ItemKey);
                }
            }

            return result;
        }
        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <param name="systemCode"></param>
        /// <returns></returns>
        public static int RefreshCache(string systemCode)
        {
            var result = 0;

            var list = GetEntitiesByCache(systemCode, true);
            foreach (var entity in list)
            {
                // 2016-02-29 吉日嘎拉 强制刷新缓存
                var roleEntity = GetEntityByCache(systemCode, entity.Id.ToString(), true);
                if (roleEntity != null)
                {
                    BasePermissionManager.ResetPermissionByCache(systemCode, null, entity.Id.ToString());
                }
            }

            return result;
        }
    }
}