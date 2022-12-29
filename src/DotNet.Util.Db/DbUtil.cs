//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Data;
using System.Reflection;
using System.Text;

namespace DotNet.Util
{
    /// <summary>
    /// DbUtil
    /// 
    /// 修改记录
    /// 
    ///		2022.05.12 版本：5.0 Troy.Cui 将DBHelper简写为DbUtil。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2022.05.12</date>
    /// </author> 
    /// </summary>
    public static partial class DbUtil
    {
        #region 获取实例 Create

        private static readonly ConcurrentDictionary<string, object> DbHelperClasses = new(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 获取指定的数据库连接
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="connectionString">数据库连接串</param>
        /// <returns>数据库访问类</returns>
        public static IDbHelper GetDbHelper(CurrentDbType dbType = CurrentDbType.SqlServer, string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = BaseSystemInfo.UserCenterDbConnection;
            }
            var dbHelperClass = GetDbHelperClass(dbType);
            var dbHelper = (IDbHelper)Assembly.Load("DotNet.Util.Db").CreateInstance(dbHelperClass, true);
            // 千万不要用以下代码，不然会经常数据库访问异常！
            // Dictionary.TryGetValue 在多线程高并发下有可能抛出空异常
            //var obj = DbHelperClasses.GetOrAdd(key: dbHelperClass, valueFactory: _ => (IDbHelper)Assembly.Load("DotNet.Util.Db").CreateInstance(dbHelperClass, true));
            //var dbHelper = (IDbHelper)obj;

            if (dbHelper != null)
            {
                dbHelper.ConnectionString = connectionString;
            }

            return dbHelper;
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

        #region public static string GetDbHelperClass(CurrentDbType dbType)
        /// <summary>
        /// 按数据类型获取数据库访问实现类
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns>数据库访问实现类</returns>
        public static string GetDbHelperClass(CurrentDbType dbType)
        {
            var result = "DotNet.Util.SqlHelper";
            switch (dbType)
            {
                case CurrentDbType.SqlServer:
                    result = "DotNet.Util.SqlHelper";
                    break;
                case CurrentDbType.Oracle:
                    result = "DotNet.Util.OracleHelper";
                    // result = "DotNet.Util.MSOracleHelper";
                    break;
                case CurrentDbType.Access:
                    result = "DotNet.Util.OleDbHelper";
                    break;
                case CurrentDbType.MySql:
                    result = "DotNet.Util.MySqlHelper";
                    break;
                case CurrentDbType.Db2:
                    result = "DotNet.Util.DB2Helper";
                    break;
                case CurrentDbType.SqLite:
                    result = "DotNet.Util.SqLiteHelper";
                    break;
                case CurrentDbType.Ase:
                    result = "DotNet.Util.AseHelper";
                    break;
                case CurrentDbType.PostgreSql:
                    result = "DotNet.Util.PostgreSqlHelper";
                    break;
            }
            return result;
        }

        #endregion

        #region DbConnection & CurrentDbType
        /// <summary>
        /// 数据库连接串，这里是为了简化思路
        /// </summary>
        public static string ConnectionString = BaseSystemInfo.BusinessDbConnection;

        /// <summary>
        /// 数据库类型，这里也是为了简化思路
        /// </summary>
        public static CurrentDbType CurrentDbType = BaseSystemInfo.BusinessDbType;

        #endregion

        #region GetDbNow
        /// <summary>
        /// 获得数据库当前日期
        /// </summary>
        /// <returns>当前日期</returns>
        public static string GetDbNow(CurrentDbType dbType)
        {
            var sb = Pool.StringBuilder.Get();
            if (dbType == CurrentDbType.SqlServer)
            {
                sb.Append(" GETDATE() ");
            }
            else if (dbType == CurrentDbType.Oracle)
            {
                sb.Append(" SYSDATE ");
            }
            else if (dbType == CurrentDbType.MySql)
            {
                sb.Append(" NOW() ");
            }
            else if (dbType == CurrentDbType.SqLite)
            {
                sb.Append(" datetime(CURRENT_TIMESTAMP, 'localtime') ");
            }
            return sb.Put();
        }
        #endregion

        #region public static string ToDbTime(this IDbHelper dbHelper, DateTime dateTime) 获得数据库格式的时间，用于SQL查询语句中的条件中

        /// <summary>
        /// 获得数据库格式的时间，用于SQL查询语句中的条件中
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="dateTime">日期或时间</param>
        /// <returns>字符串</returns>
        public static string ToDbTime(this IDbHelper dbHelper, DateTime dateTime)
        {
            return ToDbTime(dbHelper.CurrentDbType, dateTime.ToString());
        }
        #endregion

        #region public static string ToDbTime(this IDbHelper dbHelper, string dateTime) 获得数据库格式的时间，用于SQL查询语句中的条件中

        /// <summary>
        /// 获得数据库格式的时间，用于SQL查询语句中的条件中
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="dateTime">日期或时间</param>
        /// <returns>字符串</returns>
        public static string ToDbTime(this IDbHelper dbHelper, string dateTime)
        {
            return ToDbTime(dbHelper.CurrentDbType, dateTime);
        }
        #endregion

        #region public static string ToDbTime(CurrentDbType dbType, string dateTime) 获得数据库格式的时间，用于SQL查询语句中的条件中
        /// <summary>
        /// 获得数据库格式的时间，用于SQL查询语句中的条件中
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="dateTime">日期或时间</param>
        /// <returns>当前日期</returns>
        public static string ToDbTime(CurrentDbType dbType, string dateTime)
        {
            var sb = Pool.StringBuilder.Get();
            if (ValidateUtil.IsDateTime(dateTime))
            {
                switch (dbType)
                {
                    case CurrentDbType.SqlServer:
                    case CurrentDbType.Access:
                    case CurrentDbType.Ase:
                    case CurrentDbType.PostgreSql:
                    case CurrentDbType.SqLite:
                        sb.Append("'" + dateTime + "'");
                        break;
                    case CurrentDbType.Db2:
                    case CurrentDbType.Oracle:
                        sb.Append("TO_DATE('" + dateTime + "','yyyy-mm-dd hh24:mi:ss')");
                        break;
                    case CurrentDbType.MySql:
                        sb.Append("'" + dateTime + "'");
                        break;
                }
            }
            return sb.Put();
        }
        #endregion

        #region public static string GetParameter(this IDbHelper dbHelper, string parameter) 获得参数Sql表达式

        /// <summary>
        /// 获得参数Sql表达式
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="parameter">参数名称</param>
        /// <returns>字符串</returns>
        public static string GetParameter(this IDbHelper dbHelper, string parameter)
        {
            return GetParameter(dbHelper.CurrentDbType, parameter);
        }
        #endregion

        #region public static string GetParameter(CurrentDbType dbType, string parameter) 获得参数Sql表达式
        /// <summary>
        /// 获得参数Sql表达式
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="parameter">参数名称</param>
        /// <returns>字符串</returns>
        public static string GetParameter(CurrentDbType dbType, string parameter)
        {
            switch (dbType)
            {
                case CurrentDbType.SqlServer:
                case CurrentDbType.Access:
                case CurrentDbType.Ase:
                case CurrentDbType.PostgreSql:
                case CurrentDbType.SqLite:
                    parameter = "@" + parameter;
                    break;
                case CurrentDbType.Db2:
                case CurrentDbType.Oracle:
                    parameter = ":" + parameter;
                    break;
                case CurrentDbType.MySql:
                    parameter = "?" + parameter;
                    break;
            }
            return parameter;
        }
        #endregion
    }
}