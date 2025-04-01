//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Util
{
    /// <summary>
    ///	DbUtil
    /// 通用基类
    /// 
    /// 修改记录
    ///
    ///     2021.12.31 版本：1.0	Troy Cui 创建。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.12.31</date>
    /// </author> 
    /// </summary>
    public partial class DbUtil
    {
        #region public static int AggregateInt(this IDbHelper dbHelper, string tableName, string fieldName, string condition = null, string function = "SUM")
        /// <summary>
        /// 获取聚合函数值
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <param name="function">聚合函数AVG,MAX,MIN,SUM</param>
        /// <returns>数据表</returns>
        public static int AggregateInt(this IDbHelper dbHelper, string tableName, string fieldName, string condition = null, string function = "SUM")
        {
            var result = 0;
            var sb = PoolUtil.StringBuilder.Get();
            switch (dbHelper.CurrentDbType)
            {
                case CurrentDbType.SqlServer:
                    sb.Append("SELECT ISNULL(" + function + "(" + fieldName + "),0) FROM " + tableName);
                    break;
                case CurrentDbType.MySql:
                case CurrentDbType.SqLite:
                    sb.Append("SELECT IFNULL(" + function + "(" + fieldName + "),0) FROM " + tableName);
                    break;
                case CurrentDbType.Oracle:
                    sb.Append("SELECT NVL(" + function + "(" + fieldName + "),0) FROM " + tableName);
                    break;
                default:
                    sb.Append("SELECT ISNULL(" + function + "(" + fieldName + "),0) FROM " + tableName);
                    break;

            }

            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" WHERE " + condition);
            }
            var obj = dbHelper.ExecuteScalar(sb.Return());
            if (obj != null && obj != DBNull.Value)
            {
                result = obj.ToInt();
            }
            return result;
        }
        #endregion

        #region public static decimal AggregateDecimal(this IDbHelper dbHelper, string tableName, string fieldName, string condition = null, string function = "SUM")
        /// <summary>
        /// 获取聚合函数值
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <param name="function">聚合函数AVG,MAX,MIN,SUM</param>
        /// <returns>数据表</returns>
        public static decimal AggregateDecimal(this IDbHelper dbHelper, string tableName, string fieldName, string condition = null, string function = "SUM")
        {
            var result = 0M;
            var sb = PoolUtil.StringBuilder.Get();
            switch (dbHelper.CurrentDbType)
            {
                case CurrentDbType.SqlServer:
                    sb.Append("SELECT ISNULL(" + function + "(" + fieldName + "),0) FROM " + tableName);
                    break;
                case CurrentDbType.MySql:
                case CurrentDbType.SqLite:
                    sb.Append("SELECT IFNULL(" + function + "(" + fieldName + "),0) FROM " + tableName);
                    break;
                case CurrentDbType.Oracle:
                    sb.Append("SELECT NVL(" + function + "(" + fieldName + "),0) FROM " + tableName);
                    break;
                default:
                    sb.Append("SELECT ISNULL(" + function + "(" + fieldName + "),0) FROM " + tableName);
                    break;
            }

            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" WHERE " + condition);
            }
            var obj = dbHelper.ExecuteScalar(sb.Return());
            if (obj != null && obj != DBNull.Value)
            {
                result = obj.ToDecimal();
            }
            return result;
        }
        #endregion

        #region public static DateTime AggregateDateTime(this IDbHelper dbHelper, string tableName, string fieldName, string condition = null, string function = "MIN")
        /// <summary>
        /// 获取DateTime聚合函数值
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <param name="function">聚合函数MIN,MAX</param>
        /// <returns>时间</returns>
        public static DateTime AggregateDateTime(this IDbHelper dbHelper, string tableName, string fieldName, string condition = null, string function = "MIN")
        {
            var result = DateTime.Now;
            switch (function)
            {
                case "MIN":
                default:
                    result = DateTime.MinValue;
                    break;
                case "MAX":
                    result = DateTime.MaxValue;
                    break;
            }
            var sb = PoolUtil.StringBuilder.Get();
            switch (dbHelper.CurrentDbType)
            {
                case CurrentDbType.SqlServer:
                    sb.Append("SELECT ISNULL(" + function + "(" + fieldName + "),0) FROM " + tableName);
                    break;
                case CurrentDbType.MySql:
                case CurrentDbType.SqLite:
                    sb.Append("SELECT IFNULL(" + function + "(" + fieldName + "),0) FROM " + tableName);
                    break;
                case CurrentDbType.Oracle:
                    sb.Append("SELECT NVL(" + function + "(" + fieldName + "),0) FROM " + tableName);
                    break;
                default:
                    sb.Append("SELECT ISNULL(" + function + "(" + fieldName + "),0) FROM " + tableName);
                    break;
            }

            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" WHERE " + condition);
            }
            var obj = dbHelper.ExecuteScalar(sb.Return());
            if (obj != null && obj != DBNull.Value)
            {
                result = obj.ToDateTime();
            }
            return result;
        }
        #endregion
    }
}