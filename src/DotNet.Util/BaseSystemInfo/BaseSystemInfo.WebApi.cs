//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
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
    ///		<name>Troy.Cui</name>
    ///		<date>2021.03.17</date>
    /// </author>
    /// </summary>
    public partial class BaseSystemInfo
    {
        /// <summary>
        /// WebApi是否启用性能监控日志（默认不开启）
        /// </summary>
        public static bool WebApiMonitorEnabled = false;

        /// <summary>
        /// 性能监控日志文件夹
        /// </summary>
        public static string WebApiMonitorFolder = "WebApi.Monitor";

        /// <summary>
        /// WebApi是否启用慢响应监控日志（默认开启）
        /// </summary>
        public static bool WebApiSlowMonitorEnabled = true;

        /// <summary>
        /// 慢响应监控日志文件夹
        /// </summary>
        public static string WebApiSlowMonitorFolder = "WebApi.Monitor.Slow";

        /// <summary>
        /// WebApi慢响应毫秒数（默认1000，即1秒钟）
        /// </summary>
        public static int WebApiSlowResponseMilliseconds = 1000;
    }
}