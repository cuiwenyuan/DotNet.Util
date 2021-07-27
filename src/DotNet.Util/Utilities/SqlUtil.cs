//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

namespace DotNet.Util
{
    /// <summary>
    ///	SQLTrace
    /// 记录SQL执行 Global 中设置 BaseSystemInfo.LogSql=true 可以跟踪记录
    /// 
    /// 
    /// 修改记录
    /// 
    ///		2016.01.12 版本：1.0	SongBiao
    ///	
    /// <author>
    ///		<name>SongBiao</name>
    ///		<date>2016.01.12</date>
    /// </author> 
    /// </summary>
    public class SqlUtil
    {
        private static string _fileName = "Sql.log";

        #region public static void WriteLog(string commandText, IDbDataParameter[] dbParameters = null, string fileName = null)
        /// <summary>
        /// 写入sql查询句日志
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="statisticsText">耗时</param>
        /// <param name="fileName">文件名</param>
        public static void WriteLog(string commandText, IDbDataParameter[] dbParameters = null, string statisticsText = null, string fileName = null)
        {
            // 系统里应该可以配置是否记录异常现象
            if (!BaseSystemInfo.LogSql)
            {
                return;
            }
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = DateTime.Now.ToString(BaseSystemInfo.DateFormat) + "_" + DateTime.Now.Hour + "_" + _fileName;
            }
            var sb = new StringBuilder();
            //sb.AppendLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeFormat));
            //InvariantCulture输出全球地区文化统一的日期格式
            //sb.AppendLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeLongFormat));
            sb.Append(commandText);
            if (dbParameters != null)
            {
                sb.Append(" dbParameters: ");
                foreach (var parameter in dbParameters)
                {
                    sb.Append(parameter.ParameterName + "=" + parameter.Value + " ");
                }
            }
            if (!string.IsNullOrEmpty(statisticsText))
            {
                sb.Append(statisticsText);
            }
            // 将异常信息写入本地文件中
            var logDirectory = BaseSystemInfo.StartupPath + @"\Log\SqlTrace";
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            //高并发日志写法 Troy.Cui 2017-07-25
            FileLogUtil.WriteLog(logDirectory, fileName, sb.ToString());

        }

        #endregion
    }
}
