//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    /// BaseSystemInfo
    /// 这是系统的核心基础信息部分
    /// 
    /// 修改记录
    ///		2012.04.14 版本：1.0 JiRiGaLa	主键创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.04.14</date>
    /// </author>
    /// </summary>
    public partial class BaseSystemInfo
    {
        /// <summary>
        /// 记录登录日志，默认值为true
        /// </summary>
        public static bool RecordLogonLog = true;

        /// <summary>
        /// 记录服务调用日志，默认值为true
        /// </summary>
        public static bool RecordLog = true;

        /// <summary>
        /// 是否登记异常，默认值为true
        /// </summary>
        public static bool LogException = true;

        /// <summary>
        /// 是否记录数据库操作日志，默认值为false
        /// </summary>
        public static bool LogSql = false;

        /// <summary>
        /// 是否登记到 Windows 系统异常中，默认值为false
        /// </summary>
        public static bool EventLog = false;

        /// <summary>
        /// 日志文件名日期格式，可以按年、按月、按天、按小时、按分钟、按秒生成日志文件，默认值为按小时：yyyy-MM-dd'_'HH
        /// </summary>
        public static string LogFileNamePattern = "yyyy-MM-dd'_'HH";

        /// <summary>
        /// 日志文件最大大小，默认值为100M：100 * 1024 * 1024
        /// </summary>
        public static int LogFileMaxSize = 100 * 1024 * 1024;
    }
}