using System;
using System.Collections.Generic;
using System.Linq;
using FreeRedis;

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

        private static RedisClient GetClient()
        {
            var cli = new RedisClient("" + BaseSystemInfo.RedisServer + ":" + BaseSystemInfo.RedisPort + ",user=" + BaseSystemInfo.RedisUserName + ",password=" + BaseSystemInfo.RedisPassword + ",defaultDatabase=" + BaseSystemInfo.RedisInitialDb);
            // Redis命令行日志
            cli.Notice += (s, e) => LogUtil.WriteLog(e.Log, "Cache", null, "Cache");
            return cli;
        }

        /// <summary>
        /// 是否存在指定CacheKey
        /// </summary>
        /// <param name="cacheKey">缓存的key</param>
        /// <returns></returns>
        public static bool Contains(string cacheKey)
        {
            using (var redisClient = RedisUtil.GetClient())
            {
                return redisClient.Exists(cacheKey);
            }
        }

        #region Key/Value读取和存储
        /// <summary>
        /// 添加新缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static bool Add<T>(string key, T t, int timeout)
        {
            using (var redisClient = RedisUtil.GetClient())
            {
                redisClient.Set<T>(key, t, timeout);
                return true;
            }
        }
        /// <summary>
        /// 添加新缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static bool Add<T>(string key, T t, TimeSpan timeSpan)
        {
            using (var redisClient = RedisUtil.GetClient())
            {
                redisClient.Set<T>(key, t, timeSpan);
                return true;
            }
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
            using (var redisClient = RedisUtil.GetClient())
            {
                redisClient.Set<T>(key, t, timeout);
                return true;
            }
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static bool Set<T>(string key, T t, TimeSpan timeSpan)
        {
            using (var redisClient = RedisUtil.GetClient())
            {
                redisClient.Set<T>(key, t, timeSpan);
                return true;
            }
        }


        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                using (var redisClient = RedisUtil.GetClient())
                {
                    return redisClient.Get<T>(key);
                }
            }
            return default(T);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Remove(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                using (var redisClient = RedisUtil.GetClient())
                {
                    redisClient.Del(key);
                    return  true;
                }
            }
            return false;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public static void RemoveAll()
        {
            using (var redisClient = RedisUtil.GetClient())
            {
                redisClient.FlushDb();
            }
        }

        /// <summary>
        /// 删除匹配到的缓存
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static void RemoveByRegex(string pattern)
        {
            if (!string.IsNullOrWhiteSpace(pattern))
            {
                using (var redisClient = RedisUtil.GetClient())
                {
                    var keys = redisClient.Keys(pattern);
                    foreach (var key in keys)
                    {
                        redisClient.Del(key);
                    }
                }
            }
        }

        /// <summary>
        /// 获取所有缓存键
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllKeys()
        {
            using (var redisClient = RedisUtil.GetClient())
            {
                return redisClient.Keys("*");
            }
        }

        #endregion

    }
}
