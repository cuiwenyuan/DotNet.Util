//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

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
    ///		<name>Troy Cui</name>
    ///		<date>2021.03.17</date>
    /// </author>
    /// </summary>
    public partial class BaseSystemInfo
    {
        /// <summary>
        /// Redis服务器
        /// </summary>
        public static string RedisServer = string.Empty;

        /// <summary>
        /// Redis端口号
        /// </summary>
        public static int RedisPort = 6379;

        /// <summary>
        /// Redis是否启用SSL
        /// </summary>
        public static bool RedisEnableSsl = false;

        /// <summary>
        /// Redis用户名（暂时无用）
        /// </summary>
        public static string RedisUserName = string.Empty;

        /// <summary>
        /// Redis密码
        /// </summary>
        public static string RedisPassword = string.Empty;

        /// <summary>
        /// 默认Redis Cache缓存时间(6秒)
        /// </summary>
        public static int RedisCacheMillisecond = 6000;
    }
}