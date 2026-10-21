//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2026, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;

namespace DotNet.Util
{
    /// <summary>
    /// DbUtil
    /// 静态类的方法。
    /// 
    /// 修改记录
    /// 
    ///		2022.05.12 版本：1.0 Troy.Cui 创建,分离方法。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2022.05.12</date>
    /// </author> 
    /// </summary>
    public partial class DbUtil
    {
        #region MakeParameter
        /// <summary>
        /// 制作参数
        /// </summary>
        /// <param name="targetFiled"></param>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static IDbDataParameter MakeParameter(string targetFiled, object targetValue)
        {
            var dbHelper = DbHelperFactory.Create(CurrentDbType, ConnectionString);
            return dbHelper.MakeParameter(targetFiled, targetValue);
        }
        #endregion        

        #region public static int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text, int commandTimeout = 30) 执行查询返回受影响的行数
        /// <summary>
        /// 执行查询返回受影响的行数
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandType">命令分类</param>
        /// <param name="commandTimeout">等待命令执行的秒数。默认值为30。</param>
        /// <returns>影响行数</returns>
        public static int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text, int commandTimeout = 30)
        {
            return ExecuteNonQuery(ConnectionString, commandText, dbParameters, commandType, commandTimeout);
        }
        #endregion

        #region public static int ExecuteNonQuery(string connectionString, string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text, int commandTimeout = 30)
        /// <summary>
        /// 符合多数据库连接的查询方式
        /// </summary>
        /// <param name="connectionString">数据库连接串</param>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandType">命令分类</param>
        /// <param name="commandTimeout">等待命令执行的秒数。默认值为30。</param>
        /// <returns>影响行数</returns>
        public static int ExecuteNonQuery(string connectionString, string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text, int commandTimeout = 30)
        {
            var result = 0;
            var dbHelper = DbHelperFactory.Create(CurrentDbType, connectionString);
            result = dbHelper.ExecuteNonQuery(commandText, dbParameters, commandType, commandTimeout);
            dbHelper.Close();
            return result;
        }
        #endregion

        #region public static object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text, int commandTimeout = 30) 执行查询返回受首行首列
        /// <summary>
        /// 执行查询返回受首行首列
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandType">命令分类</param>
        /// <param name="commandTimeout">等待命令执行的秒数。默认值为30。</param>
        /// <returns>object</returns>
        public static object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text, int commandTimeout = 30)
        {
            return ExecuteScalar(ConnectionString, commandText, dbParameters, commandType, commandTimeout);
        }
        #endregion

        #region public static object ExecuteScalar(string connectionString, string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text, int commandTimeout = 30) 执行查询返回受首行首列
        /// <summary>
        /// 执行查询返回受首行首列
        /// </summary>
        /// <param name="connectionString">数据库连接</param>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandType">命令分类</param>
        /// <param name="commandTimeout">等待命令执行的秒数。默认值为30。</param>
        /// <returns>object</returns>
        public static object ExecuteScalar(string connectionString, string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text, int commandTimeout = 30)
        {
            object result = null;
            var dbHelper = DbHelperFactory.Create(CurrentDbType, connectionString);
            result = dbHelper.ExecuteScalar(commandText, dbParameters, commandType, commandTimeout);
            dbHelper.Close();
            return result;
        }
        #endregion

        #region public static IDataReader ExecuteReader(string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text, int commandTimeout = 30) 执行查询返回DataReader
        /// <summary>
        /// 执行查询返回DataReader
        /// </summary>
        /// <param name="commandType">命令分类</param>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandTimeout">等待命令执行的秒数。默认值为30。</param>
        /// <returns>结果集流</returns>
        public static IDataReader ExecuteReader(string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text, int commandTimeout = 30)
        {
            return ExecuteReader(ConnectionString, commandText, dbParameters, commandType, commandTimeout);
        }
        #endregion

        #region public static IDataReader ExecuteReader(string connectionString, string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text, int commandTimeout = 30) 执行查询返回DataReader
        /// <summary>
        /// 执行查询返回DataReader
        /// </summary>
        /// <param name="connectionString">数据库连接</param>
        /// <param name="commandType">命令分类</param>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandTimeout">等待命令执行的秒数。默认值为30。</param>
        /// <returns>结果集流</returns>
        public static IDataReader ExecuteReader(string connectionString, string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text, int commandTimeout = 30)
        {
            var dbHelper = DbHelperFactory.Create(CurrentDbType, connectionString);
            return dbHelper.ExecuteReader(commandText, dbParameters, commandType, commandTimeout);
        }
        #endregion

        #region public static DataTable Fill(string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text, int commandTimeout = 30) 填充数据表
        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandType">命令分类</param>
        /// <param name="commandTimeout">等待命令执行的秒数。默认值为30。</param>
        /// <returns>数据表</returns>
        public static DataTable Fill(string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text, int commandTimeout = 30)
        {
            return Fill(ConnectionString, commandText, dbParameters, commandType, commandTimeout);
        }
        #endregion

        #region public static DataTable Fill(string connectionString, string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text, int commandTimeout = 30) 填充数据表
        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="connectionString">数据库连接</param>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandType">命令分类</param>
        /// <param name="commandTimeout">等待命令执行的秒数。默认值为30。</param>
        /// <returns>数据表</returns>
        public static DataTable Fill(string connectionString, string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text, int commandTimeout = 30)
        {
            var dt = new DataTable("DotNet");
            var dbHelper = DbHelperFactory.Create(CurrentDbType, connectionString);
            dbHelper.Fill(dt, commandText, dbParameters, commandType, commandTimeout);
            dbHelper.Close();
            return dt;
        }
        #endregion

        #region public static void ExecuteCommandWithSplitter(string commandText, string splitter, int commandTimeout = 30) 批量执行脚本的方法
        /// <summary>
        /// 运行含有GO命令的多条SQL命令
        /// </summary>
        /// <param name="commandText">SQL命令字符串</param>
        /// <param name="splitter">分割字符串</param>
        /// <param name="commandTimeout">等待命令执行的秒数。默认值为30。</param>
        public static void ExecuteCommandWithSplitter(string commandText, string splitter, int commandTimeout = 30)
        {
            var startPos = 0;
            if (!string.IsNullOrWhiteSpace(commandText))
            {
                do
                {
                    var lastPos = commandText.IndexOf(splitter, startPos, StringComparison.Ordinal);
                    var length = (lastPos > startPos ? lastPos : commandText.Length) - startPos;
                    var query = commandText.Substring(startPos, length);

                    if (query.Trim().Length > 0)
                    {
                        ExecuteNonQuery(query, null, CommandType.Text, commandTimeout);
                    }

                    if (lastPos == -1)
                    {
                        break;
                    }
                    else
                    {
                        startPos = lastPos + splitter.Length;
                    }
                } while (startPos < commandText.Length);
            }
        }

        /// <summary>
        /// 运行含有GO命令的多条SQL命令
        /// </summary>
        /// <param name="commandText">SQL命令字符串</param>
        /// <param name="commandTimeout">等待命令执行的秒数。默认值为30。</param>
        public static void ExecuteCommandWithSplitter(string commandText, int commandTimeout = 30)
        {
            ExecuteCommandWithSplitter(commandText, "\r\nGO\r\n", commandTimeout);
        }
        #endregion ExecuteCommandWithSplitter方法结束
    }
}
