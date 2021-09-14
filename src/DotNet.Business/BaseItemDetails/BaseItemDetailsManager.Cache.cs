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
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.12.10</date>
    /// </author> 
    /// </remarks>
    public partial class BaseItemDetailsManager
    {
        /// <summary>
        /// 从缓存获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static BaseItemDetailsEntity GetCache(string key)
        {
            BaseItemDetailsEntity result = null;

            if (!string.IsNullOrWhiteSpace(key))
            {
                result = CacheUtil.Get<BaseItemDetailsEntity>(key);
            }

            return result;
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="entity"></param>
        private static void SetCache(string tableName, BaseItemDetailsEntity entity)
        {
            if (entity != null)
            {
                var key = "ItemDetails:" + tableName + ":" + entity.Id;
                CacheUtil.Set(key, entity);
            }
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="list"></param>
        private static void SetListCache(string key, List<BaseItemDetailsEntity> list)
        {
            if (!string.IsNullOrWhiteSpace(key) && list != null)
            {
                CacheUtil.Set(key, list);
            }
        }

        /// <summary>
        /// 从缓存获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<BaseItemDetailsEntity> GetListCache(string key)
        {
            List<BaseItemDetailsEntity> result = null;

            if (!string.IsNullOrWhiteSpace(key))
            {
                result = CacheUtil.Get<List<BaseItemDetailsEntity>>(key);
            }

            return result;
        }
    }
}