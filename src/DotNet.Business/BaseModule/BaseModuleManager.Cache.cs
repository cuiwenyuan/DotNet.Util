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
    /// BaseModuleManager
    /// 模块表缓存
    /// 
    /// 修改记录
    ///
    ///     2020.12.08 版本：1.5 Troy.Cui 使用CacheUtil缓存
    ///		2016.02.26 版本：1.4 JiRiGaLa 独立缓存库。
    ///		2015.12.03 版本：1.3 JiRiGaLa List读写方法改进。
    ///     2015.07.30 版本：1.0 JiRiGaLa 创建。
    ///	
    /// <author>  
    ///		<name>Troy.Cui</name>
    ///		<date>2016.02.26</date>
    /// </author> 
    /// </remarks>
    public partial class BaseModuleManager
    {
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override bool RemoveCache(string key)
        {
            var result = false;

            if (!string.IsNullOrWhiteSpace(key))
            {
                result = CacheUtil.Remove(key);
            }

            return result;
        }

        /// <summary>
        /// 根据Key获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static BaseModuleEntity GetCacheByKey(string key)
        {
            BaseModuleEntity result = null;

            if (!string.IsNullOrWhiteSpace(key))
            {
                result = CacheUtil.Get<BaseModuleEntity>(key);
            }

            return result;
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="entity"></param>
        private static void SetCache(string systemCode, BaseModuleEntity entity)
        {
            if (string.IsNullOrWhiteSpace(systemCode))
            {
                systemCode = "Base";
            }

            if (entity != null && !string.IsNullOrEmpty(entity.Id.ToString()))
            {
                var key = systemCode + ".Module." + entity.Id;
                CacheUtil.Set<BaseModuleEntity>(key, entity);

                key = systemCode + ".Module." + entity.Code;
                CacheUtil.Set<BaseModuleEntity>(key, entity);
            }
        }

        /// <summary>
        /// 设置List缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="list"></param>
        private static void SetListCache(string key, List<BaseModuleEntity> list)
        {
            if (!string.IsNullOrWhiteSpace(key) && list != null)
            {
                CacheUtil.Set<List<BaseModuleEntity>>(key, list);
            }
        }

        /// <summary>
        /// 获取List缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static List<BaseModuleEntity> GetListCache(string key)
        {
            List<BaseModuleEntity> result = null;

            if (!string.IsNullOrWhiteSpace(key))
            {
                result = CacheUtil.Get<List<BaseModuleEntity>>(key);
            }

            return result;
        }
    }
}