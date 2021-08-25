﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
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
        public static void WriteLog(string commandText, IDbDataParameter[] dbParameters = null, string statisticsText = null)
        {
            // 系统里应该可以配置是否记录异常现象
            if (!BaseSystemInfo.LogSql)
            {
                return;
            }
            var sb = Pool.StringBuilder.Get();
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
            //将异常信息写入本地文件中
            //高并发日志写法+Winform日志目录bug修复 Troy.Cui 2021-08-13
            LogUtil.WriteLog(sb.Put(), "SqlTrace");

        }

        #endregion
    }
}
