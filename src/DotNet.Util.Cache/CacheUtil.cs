using System;

namespace DotNet.Util
{
    /// <summary>
    /// 缓存工厂类
    /// </summary>
    public class CacheUtil
    {
        #region 缓存（创建或刷新）
        /// <summary>
        /// 缓存（创建或刷新）
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="cacheKey">缓存的Key</param>
        /// <param name="proc">泛型委托</param>
        /// <param name="isFromCache"></param>
        /// <param name="refreshCache"></param>
        /// <param name="cacheTime"></param>
        /// <returns></returns>
        public static T Cache<T>(string cacheKey, Func<T> proc, bool isFromCache = false, bool refreshCache = false, TimeSpan cacheTime = default(TimeSpan))
        {
            //string cacheType = BaseSystemInfo.CacheType;
            var cacheType = string.Empty;
            if (cacheType.Equals("Redis"))
            {
                if (cacheTime == default(TimeSpan))
                {
                    //设置默认Redis缓存1毫秒
                    cacheTime = TimeSpan.FromMilliseconds(BaseSystemInfo.RedisCacheMillisecond);
                }
                return RedisCache<T>(cacheKey, proc, isFromCache, refreshCache, cacheTime);
            }
            else
            {
                if (cacheTime == default(TimeSpan))
                {
                    //设置默认Memory缓存1毫秒
                    cacheTime = TimeSpan.FromMilliseconds(BaseSystemInfo.MemoryCacheMillisecond);
                }
                return MemoryCache<T>(cacheKey, proc, isFromCache, refreshCache, cacheTime);
            }
        }
        #endregion

        #region 获取缓存Set

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="cacheKey">缓存的Key</param>
        /// <param name="t"></param>
        /// <param name="cacheTime"></param>
        /// <returns></returns>
        public static bool Set<T>(string cacheKey, T t, TimeSpan cacheTime = default(TimeSpan))
        {
            //string cacheType = BaseSystemInfo.CacheType;
            var cacheType = string.Empty;
            if (cacheType.Equals("Redis"))
            {
                if (cacheTime == default(TimeSpan))
                {
                    //设置默认Redis缓存1毫秒
                    cacheTime = TimeSpan.FromMilliseconds(BaseSystemInfo.RedisCacheMillisecond);
                }
                return RedisUtil.Set<T>(cacheKey, t, cacheTime);
            }
            else
            {
                if (cacheTime == default(TimeSpan))
                {
                    //设置默认Redis缓存1毫秒
                    cacheTime = TimeSpan.FromMilliseconds(BaseSystemInfo.MemoryCacheMillisecond);
                }
                MemoryUtil.Set<T>(cacheKey, t, cacheTime);
                return true;
            }
        }

        #endregion

        #region 获取缓存GetCache

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="cacheKey">缓存的Key</param>
        /// <returns></returns>
        public static T Get<T>(string cacheKey)
        {
            //string cacheType = BaseSystemInfo.CacheType;
            var cacheType = string.Empty;
            if (cacheType.Equals("Redis"))
            {
                return RedisUtil.Get<T>(cacheKey);
            }
            else
            {
                return MemoryUtil.Get<T>(cacheKey);
            }
        }

        #endregion

        #region 删除缓存Remove
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="cacheKey">缓存的Key</param>
        /// <returns></returns>
        public static bool Remove(string cacheKey)
        {
            //string cacheType = BaseSystemInfo.CacheType;
            var cacheType = string.Empty;
            if (cacheType.Equals("Redis"))
            {
                return RedisUtil.Remove(cacheKey);
            }
            else
            {
                return MemoryUtil.Remove(cacheKey);
            }
        }

        /// <summary>
        /// 删除全部缓存
        /// </summary>
        /// <returns></returns>
        public static void RemoveAllCache()
        {
            //string cacheType = BaseSystemInfo.CacheType;
            var cacheType = string.Empty;
            if (cacheType.Equals("Redis"))
            {
                RedisUtil.RemoveAll();
            }
            else
            {
                MemoryUtil.RemoveAll();
            }
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="pattern">正则</param>
        /// <returns></returns>
        public static void RemoveByRegex(string pattern)
        {
            //string cacheType = BaseSystemInfo.CacheType;
            var cacheType = string.Empty;
            if (cacheType.Equals("Redis"))
            {
                RedisUtil.RemoveByRegex(pattern);
            }
            else
            {
                MemoryUtil.RemoveByRegex(pattern);
            }
        }
        #endregion

        #region Contains
        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="cacheKey">缓存的Key</param>
        /// <returns></returns>
        public static bool Contains(string cacheKey)
        {
            //string cacheType = BaseSystemInfo.CacheType;
            var cacheType = string.Empty;
            if (cacheType.Equals("Redis"))
            {
                return RedisUtil.Contains(cacheKey);
            }
            else
            {
                return MemoryUtil.Contains(cacheKey);
            }
        }

        #endregion

        #region MemoryCache
        /// <summary>
        /// 内存缓存处理 微软
        /// </summary>
        /// <param name="cacheKey">缓存的Key</param>
        /// <param name="proc">处理函数</param>
        /// <param name="isFromCache">是否从缓存读取</param>
        /// <param name="refreshCache">是否强制刷新</param>
        /// <param name="cacheTime"></param>
        /// <returns></returns>
        private static T MemoryCache<T>(string cacheKey, Func<T> proc, bool isFromCache = false, bool refreshCache = false, TimeSpan cacheTime = default(TimeSpan))
        {
            T result;
            if (isFromCache)
            {
                //缓存
                if (MemoryUtil.Get(cacheKey) != null) //判断是否有缓存
                {
                    //已经缓存
                    if (refreshCache)//是否强制刷新缓存
                    {
                        //强制刷新
                        result = proc();
                        if (result != null)
                        {
                            MemoryUtil.Set(cacheKey, result, cacheTime);
                        }
                    }
                    else
                    {
                        //不强制刷新
                        try
                        {
                            result = (T)MemoryUtil.Get(cacheKey);
                        }
                        catch (Exception ex)
                        {
                            result = proc();
                            LogUtil.WriteException(ex);
                        }
                    }
                }
                else
                {
                    //未缓存
                    result = proc();
                    if (result != null)
                    {
                        MemoryUtil.Set(cacheKey, result, cacheTime);
                    }
                }
            }
            else
            {
                result = proc();
            }
            return result;
        }
        #endregion

        #region RedisCache
        /// <summary>
        /// 缓存处理 Redis
        /// </summary>
        /// <param name="cacheKey">缓存的Key</param>
        /// <param name="proc">处理函数</param>
        /// <param name="isFromCache">是否从缓存读取</param>
        /// <param name="refreshCache">是否强制刷新</param>
        /// <param name="cacheTime"></param>
        /// <returns></returns>
        private static T RedisCache<T>(string cacheKey, Func<T> proc, bool isFromCache = false, bool refreshCache = false, TimeSpan cacheTime = default(TimeSpan))
        {
            if (cacheTime == default(TimeSpan))
            {
                cacheTime = TimeSpan.FromMilliseconds(30 * 1000);//设置默认缓存30秒
            }
            T result;
            if (isFromCache)
            {
                //缓存
                if (RedisUtil.Get<T>(cacheKey) != null) //判断是否有缓存
                {
                    //已经缓存
                    if (refreshCache)//是否强制刷新缓存
                    {
                        //强制刷新
                        result = proc();
                        if (result != null)
                        {
                            RedisUtil.Set(cacheKey, result, cacheTime);
                        }
                    }
                    else
                    {
                        //不强制刷新
                        try
                        {
                            result = (T)RedisUtil.Get<T>(cacheKey);
                        }
                        catch (Exception ex)
                        {
                            result = proc();
                            LogUtil.WriteException(ex);
                        }
                    }
                }
                else
                {
                    //未缓存
                    result = proc();
                    if (result != null)
                    {
                        RedisUtil.Set(cacheKey, result, cacheTime);
                    }
                }
            }
            else
            {
                result = proc();
            }
            return result;
        }
        #endregion

    }
}
