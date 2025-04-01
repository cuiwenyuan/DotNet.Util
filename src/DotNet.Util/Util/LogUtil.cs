//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace DotNet.Util
{
    /// <summary>
    /// 日志
    /// </summary>
    public class LogUtil
    {
        /// <summary>
        /// 写异常记录
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="extraInfo"></param>
        /// <param name="logFileNamePattern">日志文件名日期格式，可以按年、按月、按天、按小时、按分钟、按秒生成日志文件，默认为BaseSystemInfo.LogFileNamePattern的格式</param>
        public static void WriteException(Exception exception, string extraInfo = null, string logFileNamePattern = "")
        {
            WriteLog(exception, extraInfo, folder: "Exception", logFileNamePattern: logFileNamePattern);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="exception">异常信息</param>
        /// <param name="folder">日志目录（默认根目录下的Log文件夹）</param>
        /// <param name="prefix">日志文件前缀</param>
        /// <param name="suffix">日志文件后缀</param>
        /// <param name="extension">日志文件后缀(默认为log)</param>
        /// <param name="logFileNamePattern">日志文件名日期格式，可以按年、按月、按天、按小时、按分钟、按秒生成日志文件，默认为BaseSystemInfo.LogFileNamePattern的格式</param>
        public static void WriteLog(string exception, string folder = "Log", string prefix = null, string suffix = null, string extension = "log", string logFileNamePattern = "")
        {
            if (string.IsNullOrEmpty(folder))
            {
                folder = "Log";
            }
            if (string.IsNullOrEmpty(extension))
            {
                extension = "log";
            }
            if (string.IsNullOrEmpty(suffix))
            {
                suffix = "log";
            }
            //var logDirectory = string.Format(@"{0}\Log\"+ folder, AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
            //Troy.Cui 2020-02-29 返回各种.NET项目的当前运行路径
            var logDirectory = string.Format(@"{0}\Log\" + folder, AppDomain.CurrentDomain.BaseDirectory);

            //记录错误日志文件的路径
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            var fileName = prefix + DateTime.Now.ToString(BaseSystemInfo.LogFileNamePattern) + "_" + suffix + "_0." + extension;
            if (!string.IsNullOrEmpty(logFileNamePattern))
            {
                fileName = prefix + DateTime.Now.ToString(logFileNamePattern) + "_" + suffix + "_0." + extension;
            }
            FileLogUtil.WriteLog(logDirectory, fileName, exception, extension);
        }

        /// <summary>
        /// 在本地写入错误日志
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="extraInfo">额外信息</param>
        /// <param name="folder">日志目录（默认根目录下的Log文件夹）</param>
        /// <param name="prefix">日志文件前缀</param>
        /// <param name="suffix">日志文件后缀</param>
        /// <param name="extension">日志文件后缀(默认为log)</param>
        /// <param name="logFileNamePattern">日志文件名日期格式，可以按年、按月、按天、按小时、按分钟、按秒生成日志文件，默认为BaseSystemInfo.LogFileNamePattern的格式</param>
        /// 错误信息
        public static void WriteLog(Exception exception, string extraInfo, string folder = "Log", string prefix = null, string suffix = "log", string extension = "log", string logFileNamePattern = "")
        {

            var sb = PoolUtil.StringBuilder.Get();
            sb.Append(extraInfo);
            if (exception != null)
            {
                sb.Append(" Source:" + exception.Source);
                sb.Append(" TargetSite:" + exception.TargetSite.Name);
                sb.Append(" Type:" + exception.GetType());
                sb.Append(" Message:" + exception.Message);
                sb.Append(" StackTrace:" + exception.StackTrace);
            }
            WriteLog(sb.Return(), folder: folder, prefix: prefix, suffix: suffix, logFileNamePattern: logFileNamePattern);
        }
    }
}