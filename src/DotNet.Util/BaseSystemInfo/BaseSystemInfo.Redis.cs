//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Configuration;

namespace DotNet.Util
{
    /// <summary>
    /// BaseSystemInfo
    /// 这是系统的核心基础信息部分
    /// 
    /// 修改记录
    ///		2021.03.17 版本：1.0 Troy.Cui	主键创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2021.03.17</date>
    /// </author>
    /// </summary>
    public partial class BaseSystemInfo
    {
        /// <summary>
        /// 启用Redis缓存
        /// </summary>
        public static bool RedisEnabled = false;

        private static string[] _redisHosts = null;
        /// <summary>
        /// RedisHosts：ztredis6482(*)134&amp;^%xswed@redis-Read.wangcaisoft.com:6482
        /// </summary>
        public static string[] RedisHosts
        {
            get
            {
                if (_redisHosts == null)
                {
                    if (ConfigurationManager.AppSettings["RedisHosts"] != null)
                    {
                        _redisHosts = ConfigurationManager.AppSettings["RedisHosts"].Split(',');
                    }
                    if (_redisHosts == null)
                    {
                        _redisHosts = new string[] { "ztredis6482(*)134&^%xswed@redis-Read.wangcaisoft.com:6482" };
                    }
                }
                return _redisHosts;
            }
            set => _redisHosts = value;
        }

        private static string[] _redisReadOnlyHosts = null;
        /// <summary>
        /// RedisReadOnlyHosts:ztredis6488(*)134&amp;^%xswed@redis.wangcaisoft.com:6488
        /// </summary>
        public static string[] RedisReadOnlyHosts
        {
            get
            {
                if (_redisReadOnlyHosts == null)
                {
                    if (ConfigurationManager.AppSettings["RedisReadOnlyHosts"] != null)
                    {
                        _redisReadOnlyHosts = ConfigurationManager.AppSettings["RedisReadOnlyHosts"].Split(',');
                    }
                    if (_redisReadOnlyHosts == null)
                    {
                        _redisReadOnlyHosts = new string[] { "ztredis6488(*)134&^%xswed@redis.wangcaisoft.com:6488" };
                    }
                }
                return _redisReadOnlyHosts;
            }
            set => _redisReadOnlyHosts = value;
        }

        private static string[] _redisOpenIdHosts = null;
        /// <summary>
        /// RedisOpenIdHosts
        /// </summary>
        public static string[] RedisOpenIdHosts
        {
            get
            {
                if (_redisOpenIdHosts == null)
                {
                    if (ConfigurationManager.AppSettings["RedisOpenIdHosts"] != null)
                    {
                        _redisOpenIdHosts = ConfigurationManager.AppSettings["RedisOpenIdHosts"].Split(',');
                    }
                    if (_redisOpenIdHosts == null)
                    {
                        _redisOpenIdHosts = new string[] { "ShZtoRds053##@192.168.1.141:7000" };
                    }
                }
                return _redisOpenIdHosts;
            }
            set => _redisOpenIdHosts = value;
        }

        private static string[] _redisOpenIdReadOnlyHosts = null;
        /// <summary>
        /// RedisOpenIdReadOnlyHosts
        /// </summary>
        public static string[] RedisOpenIdReadOnlyHosts
        {
            get
            {
                if (_redisOpenIdReadOnlyHosts == null)
                {
                    if (ConfigurationManager.AppSettings["RedisOpenIdReadOnlyHosts"] != null)
                    {
                        _redisOpenIdReadOnlyHosts = ConfigurationManager.AppSettings["RedisOpenIdReadOnlyHosts"].Split(',');
                    }
                    if (_redisOpenIdReadOnlyHosts == null)
                    {
                        _redisOpenIdReadOnlyHosts = new string[] { "ShZtoRds053##@192.168.1.142:7000" };
                    }
                }
                return _redisOpenIdReadOnlyHosts;
            }
            set => _redisOpenIdReadOnlyHosts = value;
        }

        /// <summary>
        /// Redis服务器（注意最终组合使用RedisPassword@RedisServer:RedisPort）
        /// </summary>
        public static string RedisServer = string.Empty;

        /// <summary>
        /// Redis端口号
        /// </summary>
        public static int RedisPort = 6379;

        /// <summary>
        /// Redis初始Db
        /// </summary>
        public static long RedisInitialDb = 10;

        /// <summary>
        /// Redis是否启用SSL
        /// </summary>
        public static bool RedisSslEnabled = false;

        /// <summary>
        /// Redis用户名（暂时无用）
        /// </summary>
        public static string RedisUserName = string.Empty;

        /// <summary>
        /// Redis密码
        /// </summary>
        public static string RedisPassword = string.Empty;

        /// <summary>
        /// 默认Redis Cache缓存时间(1 秒=1000 毫秒、1 分=60000 毫秒、1 时=3600000 毫秒、1 天=86400000 毫秒)
        /// </summary>
        public static int RedisCacheMillisecond = 1;
    }
}