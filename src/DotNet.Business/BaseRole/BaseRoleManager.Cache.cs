//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <remarks>
    /// BaseRoleManager
    /// 角色表缓存
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
        /// <summary>
        /// 从缓存中获取
        /// </summary>
        /// <param name="key"></param>
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