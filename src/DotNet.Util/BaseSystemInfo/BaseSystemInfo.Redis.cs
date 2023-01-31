//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
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

        /// <summary>
        /// Redis服务器（注意最终组合使用RedisPassword@RedisServer:RedisPort）
        /// </summary>
        public static string RedisServer = "localhost";

        /// <summary>
        /// Redis端口号
        /// </summary>
        public static int RedisPort = 6379;

        /// <summary>
        /// Redis默认数据库
        /// </summary>
        public static long RedisInitialDb = 10;

        /// <summary>
        /// Redis是否启用SSL
        /// </summary>
        public static bool RedisSslEnabled = false;

        /// <summary>
        /// Redis用户名（Redis6才支持）
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