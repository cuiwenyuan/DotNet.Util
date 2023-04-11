//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;

namespace DotNet.Util
{
    /// <summary>
    ///	DbUtil
    /// 通用基类 设置各种属性
    /// 
    /// 修改记录
    /// 
    ///		2023.04.11 版本：1.0	Troy.Cui 创建。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2023.04.11</date>
    /// </author> 
    /// </summary>
    public partial class DbUtil
    {
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        /// <param name="dbConnection">数据库连接</param>
        public static void CloseConnection(this IDbConnection dbConnection)
        {
            if (dbConnection == null) return;
            try
            {
                dbConnection.Close();
            }
            catch (Exception e)
            {
                LogUtil.WriteException(e, "close connection error");
            }
        }

        /// <summary>
        /// 重新打开数据库连接
        /// </summary>
        /// <param name="dbConnection">数据库连接</param>
        public static void Reopen(this IDbConnection dbConnection)
        {
            if (dbConnection == null) return;
            try
            {
                dbConnection.Close();
                dbConnection.Open();
            }
            catch (Exception e)
            {
                LogUtil.WriteException(e, "close connection error");
            }
        }

        /// <summary>
        /// 数据库连接是否打开
        /// </summary>
        /// <param name="dbConnection">数据库连接</param>
        /// <returns></returns>
        public static bool IsOpen(this IDbConnection dbConnection)
        {
            if (dbConnection == null) return false;
            return dbConnection.State == ConnectionState.Open;
        }

        /// <summary>
        /// 数据库连接是否关闭
        /// </summary>
        /// <param name="dbConnection">数据库连接</param>
        /// <returns></returns>
        public static bool IsClose(this IDbConnection dbConnection)
        {
            if (dbConnection == null) return true;
            return dbConnection.State == ConnectionState.Closed;
        }

    }
}