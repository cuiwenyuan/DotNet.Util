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
    /// BaseAreaManager
    /// 地区表(省、市、县)
    /// 
    /// 修改记录
    ///
    ///     2020.12.04 版本：1.4 Troy.Cui 统一底层CacheUtil调用
    ///		2015.12.03 版本：1.3 JiRiGaLa List读写方法改进。
    ///	    2015.07.17 版本：1.2 JiRiGaLa 缓存清除功能实现（其实是重新获取了就可以了）。
    ///	    2015.03.15 版本：1.1 JiRiGaLa 缓存的时间不能太长了、不方便变更、10分钟读取一次就可以了。
    ///	    2015.01.06 版本：1.0 JiRiGaLa 选项管理从缓存读取，通过编号显示名称的函数完善。
    ///	
    /// <author>  
    ///		<name>Troy.Cui</name>
    ///		<date>2015.12.03</date>
    /// </author> 
    /// </remarks>
    public partial class BaseAreaManager
    {
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool RemoveCache(string key)
        {
            var result = false;

            if (!string.IsNullOrWhiteSpace(key))
            {
                result = CacheUtil.Remove(key);
            }

            return result;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static BaseAreaEntity GetCache(string key)
        {
            BaseAreaEntity result = null;

            if (!string.IsNullOrWhiteSpace(key))
            {
                result = CacheUtil.Get<BaseAreaEntity>(key);
            }

            return result;
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="entity"></param>
        private static void SetCache(BaseAreaEntity entity)
        {
            if (entity != null && !string.IsNullOrWhiteSpace(entity.Id))
            {
                var key = "Area:" + entity.Id;
                CacheUtil.Set(key, entity);
            }
        }

        /// <summary>
        /// 设置List缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="list"></param>
        private static void SetListCache(string key, List<BaseAreaEntity> list)
        {
            if (!string.IsNullOrWhiteSpace(key) && list != null && list.Count > 0)
            {
                CacheUtil.Set(key, list);
            }
        }

        /// <summary>
        /// 获取List缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static List<BaseAreaEntity> GetListCache(string key)
        {
            List<BaseAreaEntity> result = null;

            if (!string.IsNullOrWhiteSpace(key))
            {
                result = CacheUtil.Get<List<BaseAreaEntity>>(key);
            }

            return result;
        }
    }
}