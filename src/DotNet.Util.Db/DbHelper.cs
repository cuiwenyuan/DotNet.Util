//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
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
    ///		2008.09.03 版本：1.0 JiRiGaLa 创建。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.09.18</date>
    /// </author> 
    /// </summary>
    public partial class DbHelper
    {
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

        #region GetDbHelperClass
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

        #region DbConnection & DbType
        /// <summary>
        /// 数据库连接串，这里是为了简化思路
        /// </summary>
        public static string DbConnection = BaseSystemInfo.BusinessDbConnection;

        /// <summary>
        /// 数据库类型，这里也是为了简化思路
        /// </summary>
        public static CurrentDbType DbType = BaseSystemInfo.BusinessDbType;

        #endregion
    }
}