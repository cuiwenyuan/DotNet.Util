//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Util;

    /// <summary>
    ///	DbUtil
    /// 通用基类
    /// 
    /// 修改记录
    /// 
    ///		2012.02.05 版本：1.0	JiRiGaLa 分离程序。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.02.05</date>
    /// </author> 
    /// </summary>
    public partial class DbUtil
    {
        #region public int static Delete(IDbHelper dbHelper, string tableName, List<KeyValuePair<string, object>> parameters = null) 删除表格数据
        /// <summary>
        /// 删除表格数据
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表名</param>
        /// <param name="parameters">删除条件</param>
        /// <returns>影响行数</returns>
        public static int Delete(IDbHelper dbHelper, string tableName, List<KeyValuePair<string, object>> parameters = null)
        {
            var sb = Pool.StringBuilder.Get();
            sb.Append("DELETE FROM " + tableName);
            var condition = GetWhereString(dbHelper, parameters, BaseUtil.SqlLogicConditional);
            if (condition.Length > 0)
            {
                sb.Append(" WHERE " + condition);
            }
            return dbHelper.ExecuteNonQuery(sb.Put(), dbHelper.MakeParameters(parameters));
        }
        #endregion

        #region public static int Truncate(IDbHelper dbHelper, string tableName) 截断表格数据
        /// <summary>
        /// 截断表格数据
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表格</param>
        /// <returns>是否成功</returns>
        public static int Truncate(IDbHelper dbHelper, string tableName)
        {
            var sb = Pool.StringBuilder.Get();
            sb.Append(" TRUNCATE TABLE " + tableName);
            // DB2 V9.7 以后才支持这个语句
            if (dbHelper.CurrentDbType == CurrentDbType.Db2)
            {
                sb.Append(" ALTER TABLE " + tableName + " ACTIVATE NOT LOGGED INITIALLY WITH EMPTY TABLE ");
            }
            return dbHelper.ExecuteNonQuery(sb.Put());
        }
        #endregion
    }
}