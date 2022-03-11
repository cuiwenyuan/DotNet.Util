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
        /// 记录登录日志
        /// </summary>
        public static bool RecordLogonLog = true;

        /// <summary>
        /// 记录服务调用日志
        /// </summary>
        public static bool RecordLog = false;

        /// <summary>
        /// 是否登记异常
        /// </summary>
        public static bool LogException = true;

        /// <summary>
        /// 是否记录数据库操作日志
        /// </summary>
        public static bool LogSql = false;

        /// <summary>
        /// 是否登记到 Windows 系统异常中
        /// </summary>
        public static bool EventLog = false;
    }
}