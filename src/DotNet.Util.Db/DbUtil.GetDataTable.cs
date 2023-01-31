//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

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
    ///     2021.08.30 版本：2.0	Troy Cui 优化升级。
    ///		2012.02.05 版本：1.0	JiRiGaLa 分离程序。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.02.05</date>
    /// </author> 
    /// </summary>
    public partial class DbUtil
    {
        #region public static DataTable GetDataTable(this IDbHelper dbHelper, string tableName, string name, object[] values, string order = null) 获取数据表 一参 参数为数组
        /// <summary>
        /// 获取数据表 一参 参数为数组
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="name">字段名</param>
        /// <param name="values">IN查询字段值</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        public static DataTable GetDataTable(this IDbHelper dbHelper, string tableName, string name, object[] values, string order = null)
        {
            tableName = tableName.ToTableName();
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT * FROM " + tableName);
            if (values == null || values.Length == 0)
            {
                sb.Append("  WHERE " + name + " IS NULL");
            }
            else
            {
                sb.Append("  WHERE " + name + " IN (" + ObjectUtil.ToList(values, "'") + ")");
            }
            if (!string.IsNullOrEmpty(order))
            {
                sb.Append(" ORDER BY " + order);
            }
            return dbHelper.Fill(sb.Put());
        }
        #endregion

        #region public static DataTable GetDataTable(this IDbHelper dbHelper, string tableName, List<KeyValuePair<string, object>> parameters, int topLimit = 0, string order = null, string sqlLogicConditional = null)

        /// <summary>
        /// GetDataTable
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="parameters">参数</param>
        /// <param name="topLimit">TOP记录数</param>
        /// <param name="order">排序（ORDER BY之后内容）</param>
        /// <param name="sqlLogicConditional">SQL查询的逻辑条件AND/OR</param>
        /// <returns></returns>
        public static DataTable GetDataTable(this IDbHelper dbHelper, string tableName, List<KeyValuePair<string, object>> parameters, int topLimit = 0, string order = null, string sqlLogicConditional = null)
        {
            tableName = tableName.ToTableName();
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT * FROM " + tableName);
            var whereSql = string.Empty;
            if (topLimit != 0)
            {
                switch (dbHelper.CurrentDbType)
                {
                    case CurrentDbType.Access:
                    case CurrentDbType.SqlServer:
                        sb.Clear();
                        sb.Append("SELECT TOP " + topLimit + " * FROM " + tableName);
                        break;
                    case CurrentDbType.Oracle:
                        if (string.IsNullOrEmpty(order))
                        {
                            whereSql = AddWhere(whereSql, " ROWNUM < = " + topLimit);
                        }
                        break;
                }
            }
            if (string.IsNullOrEmpty(sqlLogicConditional))
            {
                sqlLogicConditional = BaseUtil.SqlLogicConditional;
            }
            var subSql = GetWhereString(dbHelper, parameters, sqlLogicConditional);
            if (!string.IsNullOrEmpty(subSql))
            {
                if (whereSql.Length > 0)
                {
                    whereSql = whereSql + sqlLogicConditional + subSql;
                }
                else
                {
                    whereSql = subSql;
                }
            }
            if (whereSql.Length > 0)
            {
                sb.Append(" WHERE " + whereSql);
            }
            if (!string.IsNullOrEmpty(order))
            {
                sb.Append(" ORDER BY " + order);
            }
            if (topLimit != 0)
            {
                switch (dbHelper.CurrentDbType)
                {
                    case CurrentDbType.MySql:
                        sb.Append(" LIMIT 0, " + topLimit);
                        break;
                    case CurrentDbType.Oracle:
                        if (!string.IsNullOrEmpty(order))
                        {
                            sb.Append("SELECT * FROM (" + sb.ToString() + ") WHERE ROWNUM < = " + topLimit);
                        }
                        break;
                }
            }
            var dt = new DataTable(tableName);
            if (parameters != null && parameters.Count > 0)
            {
                dt = dbHelper.Fill(sb.Put(), dbHelper.MakeParameters(parameters));
            }
            else
            {
                dt = dbHelper.Fill(sb.Put());
            }
            return dt;
        }

        #endregion

        #region public static DataTable GetDataTable(this IDbHelper dbHelper, string tableName, IDbDataParameter[] dbParameters, string conditions, int topLimit = 0, string order = null, string selectField = " * ")

        /// <summary>
        /// GetDataTable
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="topLimit">TOP记录数</param>
        /// <param name="order">排序（ORDER BY之后内容）</param>
        /// <param name="selectField">所选字段</param>
        /// <returns></returns>
        public static DataTable GetDataTable(this IDbHelper dbHelper, string tableName, IDbDataParameter[] dbParameters, string conditions, int topLimit = 0, string order = null, string selectField = " * ")
        {
            tableName = tableName.ToTableName();
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT " + selectField + " FROM " + tableName);
            var whereSql = string.Empty;
            if (topLimit != 0)
            {
                switch (dbHelper.CurrentDbType)
                {
                    case CurrentDbType.Access:
                    case CurrentDbType.SqlServer:
                        sb.Clear();
                        sb.Append("SELECT TOP " + topLimit + selectField + " FROM " + tableName);
                        break;
                    case CurrentDbType.Oracle:
                        if (string.IsNullOrEmpty(order))
                        {
                            whereSql = AddWhere(whereSql, " ROWNUM < = " + topLimit);
                        }
                        break;
                }
            }
            // 要传入 conditions
            if (!string.IsNullOrEmpty(conditions))
            {
                conditions = " WHERE " + conditions;
            }
            sb.Append(conditions + whereSql);
            if (!string.IsNullOrEmpty(order))
            {
                sb.Append(" ORDER BY " + order);
            }
            var dt = new DataTable(tableName);
            if (topLimit != 0)
            {
                switch (dbHelper.CurrentDbType)
                {
                    case CurrentDbType.MySql:
                        sb.Append(" LIMIT 0, " + topLimit);
                        break;
                    case CurrentDbType.Oracle:
                        if (!string.IsNullOrEmpty(order))
                        {
                            sb.Append("SELECT * FROM (" + sb.ToString() + ") WHERE ROWNUM < = " + topLimit);
                        }
                        break;
                }
            }
            if (dbParameters != null && dbParameters.Length > 0)
            {
                dt = dbHelper.Fill(sb.Put(), dbParameters);
            }
            else
            {
                dt = dbHelper.Fill(sb.Put());
            }
            return dt;
        }

        #endregion
    }
}