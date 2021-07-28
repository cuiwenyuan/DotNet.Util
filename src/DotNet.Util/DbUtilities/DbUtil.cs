﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;

namespace DotNet.Util
{
    /// <summary>
    /// DbHelper
    /// 有关数据库连接的方法。
    /// 
    /// 修改记录
    /// 
    ///		2011.09.18 版本：2.0 JiRiGaLa 采用默认参数技术,把一些方法进行简化。
    ///		2008.09.03 版本：1.0 Troy Cui 创建。
    /// 
    /// <author>
    ///		<name>Troy Cui</name>
    ///		<date>2011.09.18</date>
    /// </author> 
    /// </summary>
    public partial class DbUtil
    {
        #region public static CurrentDbType GetDbType(string dbType, CurrentDbType defaultDbType = CurrentDbType.SqlServer) 数据库连接的类型判断
        /// <summary>
        /// 数据库连接的类型判断
        /// 2016-02-24 吉日嘎拉 忽略大小写，只要拼写正确就可以了，提高兼容性
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="defaultDbType">默认数据库，防止设置有错误</param>
        /// <returns>数据库类型</returns>
        public static CurrentDbType GetDbType(string dbType, CurrentDbType defaultDbType = CurrentDbType.SqlServer)
        {
            var result = defaultDbType;

            if (!string.IsNullOrEmpty(dbType))
            {
                foreach (CurrentDbType currentDbType in Enum.GetValues(typeof(CurrentDbType)))
                {
                    if (currentDbType.ToString().Equals(dbType, StringComparison.OrdinalIgnoreCase))
                    {
                        result = currentDbType;
                        break;
                    }
                }
            }

            return result;
        }
        #endregion

        #region public static CommandType GetCommandType(string commandType) 数据库连接的类型判断
        /// <summary>
        /// 数据库连接的类型判断
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <returns>命令类型</returns>
        public static CommandType GetCommandType(string commandType)
        {
            var result = CommandType.Text;
            foreach (CommandType currentCommandType in Enum.GetValues(typeof(CommandType)))
            {
                if (currentCommandType.ToString().Equals(commandType))
                {
                    result = currentCommandType;
                    break;
                }
            }
            return result;
        }
        #endregion
    }
}