using System;
using System.Collections.Generic;
using System.Linq;
using NewLife.Caching;

namespace DotNet.Util
{
    /// <summary>
    /// Redis缓存辅助类
    /// </summary>

    public class RedisUtil
    {
        static RedisUtil()
        {

        }

        // 单例并只有一个连接
        private static FullRedis redisClient { get; } = GetClient();
        // 以下代码每次调用都会有一个实例，千万不要用
        //private static RedisClient redisClient => GetClient();

        private static FullRedis GetClient()
        {
            var sb = PoolUtil.StringBuilder.Get().Append(BaseSystemInfo.RedisServer + ":" + BaseSystemInfo.RedisPort + ",user=" + BaseSystemInfo.RedisUserName + ",password=" + BaseSystemInfo.RedisPassword + ",defaultDatabase=" + BaseSystemInfo.RedisInitialDb);
            var cli = new FullRedis();
            cli.Init(sb.Return());
            return cli;
        }

        #region 是否存在指定CacheKey
        /// <summary>
        /// 是否存在指定CacheKey
        /// </summary>
        /// <param name="cacheKey">缓存的key</param>
        /// <returns></returns>
        public static bool Contains(string cacheKey)
        {
            return redisClient.ContainsKey(cacheKey);
        }
        #endregion

        #region Key/Value读取和存储
        /// <summary>
        /// 添加新缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="t"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static bool Add<T>(string key, T t, int timeout)
        {
            redisClient.Set<T>(key, t, timeout);
            return true;
        }
        /// <summary>
        /// 添加新缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="t"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static bool Add<T>(string key, T t, TimeSpan timeSpan)
        {
            redisClient.Set<T>(key, t, timeSpan);
            return true;
        }

        /// <summary>
        /// 设置缓存 用于修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存建</param>
        /// <param name="t">缓存值</param>
        /// <param name="timeout">过期时间，单位秒,-1：不过期，0：默认过期时间</param>
        /// <returns></returns>
        public static bool Set<T>(string key, T t, int timeout = 0)
        {
            redisClient.Set<T>(key, t, timeout);
            return true;
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="t"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static bool Set<T>(string key, T t, TimeSpan timeSpan)
        {
            redisClient.Set<T>(key, t, timeSpan);
            return true;
        }


        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                return redisClient.Get<T>(key);
            }
            return default(T);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static bool Remove(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                redisClient.Remove(key);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public static void RemoveAll()
        {
            //修复：Clear() 一次 FLUSHDB 清空当前库，避免 Keys 全量枚举阻塞 Redis
            redisClient.Clear();
        }

        /// <summary>
        /// 删除匹配到的缓存
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static void RemoveByRegex(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                return;
            }
            var keys = redisClient.Keys;
            if (keys == null)
            {
                return;
            }
            //修复：先收集匹配的键，再批量 Remove(string[]) 一次删除，避免逐 key 多次往返
            var matchedKeys = new List<string>();
            foreach (var key in keys)
            {
                if (key != null && System.Text.RegularExpressions.Regex.IsMatch(key, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1)))
                {
                    matchedKeys.Add(key);
                }
            }
            if (matchedKeys.Count > 0)
            {
                redisClient.Remove(matchedKeys.ToArray());
            }
        }

        /// <summary>
        /// 获取所有缓存键
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllKeys()
        {
            //修复：Keys 可能为 null，避免 NRE
            return redisClient.Keys?.ToArray() ?? new string[0];
        }

        #endregion

    }
}
