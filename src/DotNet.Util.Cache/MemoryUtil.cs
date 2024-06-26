﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
#if NET452_OR_GREATER
using System.Web;
using System.Web.Caching;
#elif NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
using Microsoft.Extensions.Caching.Memory;
#endif
namespace DotNet.Util
{
    /// <summary>
    /// MemoryCache缓存辅助类
    /// </summary>
    public static class MemoryUtil
    {
#if NET452_OR_GREATER
        //HttpRuntime.Cache可用于Web和WinForm
        static readonly Cache Cache = HttpRuntime.Cache;
#elif NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
        static MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());
#endif
        /// <summary>
        /// 是否存在指定CacheKey
        /// </summary>
        /// <param name="cacheKey">缓存的key</param>
        /// <returns></returns>
        public static bool Contains(string cacheKey)
        {
#if NET452_OR_GREATER
            if (!string.IsNullOrEmpty(cacheKey) && Cache[cacheKey] == null)
            {
                return true;
            }
            else
            {
                return false;
            }
#elif NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
            object obj = null;
            if (!string.IsNullOrEmpty(cacheKey) && Cache.TryGetValue(cacheKey, out obj))
            {
                return true;
            }
            else
            {
                return false;
            }
#endif
        }

        /// <summary>
        /// 获取当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="cacheKey">缓存的key</param>
        /// <returns></returns>
        public static object Get(string cacheKey)
        {
#if NET452_OR_GREATER
            return Cache[cacheKey];
#elif NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
            object obj = null;
            if (!string.IsNullOrEmpty(cacheKey) && Cache.TryGetValue(cacheKey, out obj))
            {
                return obj;
            }
            else
            {
                return default(object);
            }
#endif
        }

        /// <summary>
        /// 获取当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="cacheKey">缓存的key</param>
        /// <returns></returns>
        public static T Get<T>(string cacheKey)
        {
            var obj = Get(cacheKey);
            return obj == null ? default(T) : (T)obj;
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="cacheKey">缓存的key</param>
        /// <param name="cacheValue">缓存的对象</param>
        public static void Set(string cacheKey, object cacheValue)
        {
            //向cacheKey对象插入项,使用此方法改写具有相同cacheKey的现有cacheKey项。
#if NET452_OR_GREATER
            Cache.Insert(cacheKey, cacheValue);
#elif NETSTANDARD2_0_OR_GREATER
            Cache.Set(cacheKey, cacheValue);
#endif
        }
#if NET452_OR_GREATER
        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        /// <param name="slidingExpiration">弹性过期</param>
        public static void Set(string cacheKey, object cacheValue, TimeSpan slidingExpiration)
        {
            //注意两种过期策略只能使用其中一种，使用了NoAbsoluteExpiration 参数就得把NoSlidingExpiration参数置为TimeSpan.Zero
            //使用了NoSlidingExpiration参数就得把NoAbsoluteExpiration 参数置为 DateTime.MaxValues
            Cache.Insert(cacheKey, cacheValue, null, DateTime.MaxValue, slidingExpiration, CacheItemPriority.High, CacheItemRemovedCallback);
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="t"></param>
        /// <param name="slidingExpiration">弹性过期</param>
        public static void Set<T>(string cacheKey, T t, TimeSpan slidingExpiration)
        {
            //注意两种过期策略只能使用其中一种，使用了NoAbsoluteExpiration 参数就得把NoSlidingExpiration参数置为TimeSpan.Zero
            //使用了NoSlidingExpiration参数就得把NoAbsoluteExpiration 参数置为 DateTime.MaxValues
            Cache.Insert(cacheKey, t, null, DateTime.MaxValue, slidingExpiration, CacheItemPriority.High, CacheItemRemovedCallback);
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        /// <param name="absoluteExpiration">绝对过期</param>
        public static void Set(string cacheKey, object cacheValue, DateTime absoluteExpiration)
        {
            Cache.Insert(cacheKey, cacheValue, null, absoluteExpiration, TimeSpan.Zero, CacheItemPriority.High, CacheItemRemovedCallback);
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        /// <param name="dependencies"></param>
        /// <param name="absoluteExpiration">绝对过期</param>
        /// <param name="slidingExpiration">弹性过期</param>
        /// <param name="cacheItemPriority"></param>
        /// <param name="onRemoveCallback"></param>
        public static void Set(string cacheKey, object cacheValue, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority cacheItemPriority, CacheItemRemovedCallback onRemoveCallback)
        {
            //将指定项添加到 Cache 对象，该对象具有依赖项、过期和优先级策略以及一个委托（可用于在从 Cache 移除插入项时通知应用程序）。如果 Cache 中已保存了具有相同 key 参数的项，则对此方法的调用将失败。若要使用相同的 key 参数改写现有的 Cache 项，请使用 Insert 方法.
            Cache.Add(cacheKey, cacheValue, null, absoluteExpiration, Cache.NoSlidingExpiration, cacheItemPriority, onRemoveCallback);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        public static bool Remove(string cacheKey)
        {
            return Cache.Remove(cacheKey) != null;
        }
        /// <summary>
        /// 这个不能固定  
        /// </summary>
        public static object UserLock = new object();
        /// <summary>
        /// 缓存失效回调
        /// </summary>
        /// <param name="cacheKey">缓存key值</param>
        /// <param name="cacheValue">缓存的对象</param>
        /// <param name="reasonToRemove">缓存移出原因</param>
        public static void CacheItemRemovedCallback(string cacheKey, object cacheValue, CacheItemRemovedReason reasonToRemove)
        {
            if (BaseSystemInfo.LogCache)
            {
                var sb = PoolUtil.StringBuilder.Get();
                sb.Append(
                    string.Format("Cache Key: {0} invalid at {1} with reason {2}",
                        new object[]
                        {
                            cacheKey, DateTime.Now.ToString(BaseSystemInfo.DateTimeLongFormat),
                            reasonToRemove.ToString()
                        }));
                LogUtil.WriteLog(sb.Return(), "Cache", null, "Cache");
            }
        }
#elif NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        /// <param name="slidingExpiration">弹性过期</param>
        /// <param name="cacheItemPriority">优先级</param>
        public static void Set(string cacheKey, object cacheValue, TimeSpan slidingExpiration, CacheItemPriority cacheItemPriority = CacheItemPriority.High)
        {
            //注意两种过期策略只能使用其中一种，使用了AbsoluteExpiration 参数就得把NoSlidingExpiration参数置为TimeSpan.Zero
            //使用了NoSlidingExpiration参数就得把AbsoluteExpiration 参数置为 DateTime.MaxValues
            Cache.Set(cacheKey, cacheValue, new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.MaxValue,
                SlidingExpiration = slidingExpiration,
                Priority = cacheItemPriority
            }.RegisterPostEvictionCallback(MyCallback));

        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="t"></param>
        /// <param name="slidingExpiration">弹性过期</param>
        /// <param name="cacheItemPriority">优先级</param>
        public static void Set<T>(string cacheKey, T t, TimeSpan slidingExpiration, CacheItemPriority cacheItemPriority = CacheItemPriority.High)
        {
            //注意两种过期策略只能使用其中一种，使用了AbsoluteExpiration 参数就得把NoSlidingExpiration参数置为TimeSpan.Zero
            //使用了NoSlidingExpiration参数就得把AbsoluteExpiration 参数置为 DateTime.MaxValues
            Cache.Set(cacheKey, t, new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.MaxValue,
                SlidingExpiration = slidingExpiration,
                Priority = cacheItemPriority
            }.RegisterPostEvictionCallback(MyCallback));
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        /// <param name="absoluteExpiration">绝对过期</param>
        /// <param name="cacheItemPriority">优先级</param>
        public static void Set(string cacheKey, object cacheValue, DateTime absoluteExpiration, CacheItemPriority cacheItemPriority = CacheItemPriority.High)
        {
            Cache.Set(cacheKey, cacheValue, new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = absoluteExpiration,
                SlidingExpiration = TimeSpan.Zero,
                Priority = cacheItemPriority
            }.RegisterPostEvictionCallback(MyCallback));
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        /// <param name="absoluteExpiration">绝对过期</param>
        /// <param name="slidingExpiration">弹性过期</param>
        /// <param name="cacheItemPriority">优先级</param>
        public static void Set(string cacheKey, object cacheValue, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority cacheItemPriority = CacheItemPriority.High)
        {
            //将指定项添加到 Cache 对象，该对象具有依赖项、过期和优先级策略以及一个委托（可用于在从 Cache 移除插入项时通知应用程序）。如果 Cache 中已保存了具有相同 key 参数的项，则对此方法的调用将失败。若要使用相同的 key 参数改写现有的 Cache 项，请使用 Insert 方法.
            Cache.Set(cacheKey, cacheValue, new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = absoluteExpiration,
                SlidingExpiration = slidingExpiration,
                Priority = cacheItemPriority
            }.RegisterPostEvictionCallback(MyCallback));
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        public static bool Remove(string cacheKey)
        {
            try
            {
                Cache.Remove(cacheKey);
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return false;
            }

        }

        /// <summary>
        /// 缓存失效回调
        /// </summary>
        /// <param name="cacheKey">缓存key值</param>
        /// <param name="cacheValue">缓存的对象</param>
        /// <param name="reason">缓存移出原因</param>
        /// <param name="state"></param>
        private static void MyCallback(object cacheKey, object cacheValue, EvictionReason reason, object state)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append(
            string.Format("Cache Key: {0} invalid at {1} with reason {2}",
                new object[]
                {
                    cacheKey, DateTime.Now.ToString(BaseSystemInfo.DateTimeLongFormat), reason.ToString()
                }));
            LogUtil.WriteLog(sb.Return(), "Cache", null, "Cache");
        }
#endif
        /// <summary>
        /// 删除所有缓存
        /// </summary>
        public static void RemoveAll()
        {
            var keys = new List<string>();
#if NET452_OR_GREATER
            var iDictionaryEnumerator = Cache.GetEnumerator();
            while (iDictionaryEnumerator.MoveNext())
            {
                Cache.Remove(Convert.ToString(iDictionaryEnumerator.Key));
            }
#elif NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var entries = Cache.GetType().GetField("_entries", flags).GetValue(Cache);
            var cacheItems = entries as IDictionary;
            if (cacheItems != null)
            {
                foreach (DictionaryEntry cacheItem in cacheItems)
                {
                    keys.Add(cacheItem.Key.ToString());
                }
            }
#endif
        }
        /// <summary>
        /// 删除匹配到的缓存
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static void RemoveByRegex(string pattern)
        {
            var ils = SearchByRegex(pattern);
            foreach (var s in ils)
            {
                Remove(s);
            }
        }

        /// <summary>
        /// 搜索 匹配到的缓存
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static IList<string> SearchByRegex(string pattern)
        {
            var cacheKeys = GetAllKeys();
            var l = cacheKeys.Where(k => Regex.IsMatch(k, pattern, (RegexOptions.Singleline | (RegexOptions.Compiled | RegexOptions.IgnoreCase)))).ToList();
            return l.AsReadOnly();
        }

        /// <summary>
        /// 获取所有缓存键
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllKeys()
        {
            var keys = new List<string>();
#if NET452_OR_GREATER
            var iDictionaryEnumerator = Cache.GetEnumerator();
            while (iDictionaryEnumerator.MoveNext())
            {
                keys.Add(Convert.ToString(iDictionaryEnumerator.Key));
            }
#elif NETSTANDARD2_0_OR_GREATER
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var entries = Cache.GetType().GetField("_entries", flags).GetValue(Cache);
            var cacheItems = entries as IDictionary;
            if (cacheItems != null)
            {
                foreach (DictionaryEntry cacheItem in cacheItems)
                {
                    keys.Add(cacheItem.Key.ToString());
                }
            }
#endif
            return keys;
        }
    }
}
