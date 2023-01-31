//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
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
                key = systemCode + ".Role." + entity.Id;
                CacheUtil.Set<BaseRoleEntity>(key, entity);

                key = systemCode + ".Role." + entity.Code;
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