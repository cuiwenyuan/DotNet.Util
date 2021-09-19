using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Redis;

namespace DotNet.Util
{
    /// <summary>
    /// Redis缓存辅助类
    /// </summary>

    public class RedisUtil
    {
        //默认缓存过期时间单位秒
        private static int SecondsTimeOut = 30 * 60;
        //数据库
        private static long InitialDb;
        //地址
        private static string Url;
        private static PooledRedisClientManager _instance = null;
        private static readonly object Locker = new Object();

        private static PooledRedisClientManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Locker)
                    {
                        if (_instance == null)
                        {
                            InitialDb = 10;
                            Url = BaseSystemInfo.RedisHosts[0];
                            _instance = new PooledRedisClientManager(InitialDb, new string[] { Url });
                        }
                    }
                }
                return _instance;
            }
        }
        static RedisUtil()
        {
        }

        private static RedisClient GetClient()
        {
            return (RedisClient)Instance.GetClient();
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
                return redisClient.ContainsKey(cacheKey);
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
                if (timeout > 0)
                {
                    SecondsTimeOut = timeout;
                }
                return redisClient.Add<T>(key, t, DateTime.Now.AddHours(SecondsTimeOut));
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
                return redisClient.Add<T>(key, t, timeSpan);
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
                if (timeout > 0)
                {
                    SecondsTimeOut = timeout;
                }
                return redisClient.Set<T>(key, t, DateTime.Now.AddSeconds(SecondsTimeOut));
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
                return redisClient.Set<T>(key, t, timeSpan);
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
                    return redisClient.Remove(key);
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
            var cacheKeys = GetAllKeys();

            using (var redisClient = RedisUtil.GetClient())
            {
                redisClient.RemoveAll(cacheKeys);
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
                    redisClient.RemoveByRegex(pattern);
                }
            }
        }

        /// <summary>
        /// 获取所有缓存键
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllKeys()
        {
            using (var redisClient = RedisUtil.GetClient())
            {
                return redisClient.GetAllKeys();
            }
        }

        #endregion

        #region 链表操作
        /// <summary>
        /// 获取链表数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetList<T>(string listId)
        {
            using (var redisClient = RedisUtil.GetClient())
            {
                var iredisClient = redisClient.As<T>();
                return iredisClient.Lists[listId];
            }
        }

        /// <summary>
        /// IEnumerable数据添加到链表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <param name="values"></param>
        /// <param name="timeout"></param>
        public static void AddList<T>(string listId, IEnumerable<T> values, int timeout = 0)
        {
            using (var redisClient = RedisUtil.GetClient())
            {
                redisClient.Expire(listId, 60);
                var iredisClient = redisClient.As<T>();
                if (timeout >= 0)
                {
                    if (timeout > 0)
                    {
                        SecondsTimeOut = timeout;
                    }
                    redisClient.Expire(listId, SecondsTimeOut);
                }
                var redisList = iredisClient.Lists[listId];
                redisList.AddRange(values);
                iredisClient.Save();
            }
        }

        /// <summary>
        /// 添加单个实体到链表中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <param name="item"></param>
        /// <param name="timeout"></param>
        public static void AddEntityToList<T>(string listId, T item, int timeout = 0)
        {
            using (var redisClient = RedisUtil.GetClient())
            {
                var iredisClient = redisClient.As<T>();
                if (timeout >= 0)
                {
                    if (timeout > 0)
                    {
                        SecondsTimeOut = timeout;
                    }
                    redisClient.Expire(listId, SecondsTimeOut);
                }
                var redisList = iredisClient.Lists[listId];
                redisList.Add(item);
                iredisClient.Save();
            }
        }

        /// <summary>
        /// 在链表中删除单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <param name="t"></param>
        public static void RemoveEntityFromList<T>(string listId, T t)
        {
            using (var redisClient = RedisUtil.GetClient())
            {
                var iredisClient = redisClient.As<T>();
                var redisList = iredisClient.Lists[listId];
                redisList.RemoveValue(t);
                iredisClient.Save();
            }
        }

        /// <summary>
        /// 根据lambada表达式删除符合条件的实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <param name="func"></param>
        public static void RemoveEntityFromList<T>(string listId, Func<T, bool> func)
        {
            var iredisClient = RedisUtil.GetClient().As<T>();
            var redisList = iredisClient.Lists[listId];
            var value = redisList.Where(func).FirstOrDefault();
            redisList.RemoveValue(value);
            iredisClient.Save();
        }
        #endregion


    }
}
