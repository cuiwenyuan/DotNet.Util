//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2026, DotNet.
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
    }
}
