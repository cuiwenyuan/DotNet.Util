//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
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
    ///		2015.06.16 版本：1.0	JiRiGaLa 分离程序。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.06.16</date>
    /// </author> 
    /// </summary>
    public partial class DbUtil
    {
        #region public static IDataReader ExecuteReader(IDbHelper dbHelper, string tableName, string name, object[] values, string order = null) 获取数据表 一参 参数为数组
        /// <summary>
        /// 获取数据表 一参 参数为数组
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">数据来源表名</param>
        /// <param name="name">字段名</param>
        /// <param name="values">字段值</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        public static IDataReader ExecuteReader(IDbHelper dbHelper, string tableName, string name, object[] values, string order = null)
        {
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT * FROM " + tableName);
            if (values == null || values.Length == 0)
            {
                sb.Append(" WHERE " + name + " IS NULL");
            }
            else
            {
                sb.Append(" WHERE " + name + " IN (" + ObjectUtil.ToList(values, "'") + ")");
            }
            if (!string.IsNullOrEmpty(order))
            {
                sb.Append(" ORDER BY " + order);
            }
            return dbHelper.ExecuteReader(sb.Put());
        }
        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="dbHelper"></param>
        /// <param name="tableName"></param>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static List<TModel> ExecuteReader<TModel>(IDbHelper dbHelper, string tableName, string name,
            object[] values, string order = null) where TModel : new()
        {
            return ExecuteReader(dbHelper, tableName, name, values, order).ToList<TModel>();
        }
        #endregion


        // 读取列表部分 填充IDataReader 常用
        /// <summary>
        /// 添加Where
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="appendWhere"></param>
        /// <returns></returns>
        private static string AddWhere(string condition, string appendWhere)
        {
            if (string.IsNullOrEmpty(condition))
            {
                return appendWhere;
            }
            return condition + BaseUtil.SqlLogicConditional + appendWhere;
        }
        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="tableName"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IDataReader ExecuteReader(IDbHelper dbHelper, string tableName, string condition)
        {
            return ExecuteReader2(dbHelper, tableName, condition);
        }
        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="dbHelper"></param>
        /// <param name="tableName"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static List<TModel> ExecuteReader<TModel>(IDbHelper dbHelper, string tableName, string condition) where TModel : new()
        {
            return ExecuteReader(dbHelper, tableName, condition).ToList<TModel>();
        }
        /// <summary>
        /// ExecuteReader2
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="tableName"></param>
        /// <param name="condition"></param>
        /// <param name="topLimit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static IDataReader ExecuteReader2(IDbHelper dbHelper, string tableName, string condition, int topLimit = 0, string order = null)
        {
            // 这里是需要完善的功能，完善了这个，是一次重大突破 
            var sql = ExecuteReaderQueryString(dbHelper, tableName, "*", condition, topLimit, order);
            return dbHelper.ExecuteReader(sql);
        }
        /// <summary>
        /// ExecuteReader2
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="dbHelper"></param>
        /// <param name="tableName"></param>
        /// <param name="condition"></param>
        /// <param name="topLimit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static List<TModel> ExecuteReader2<TModel>(IDbHelper dbHelper, string tableName, string condition, int topLimit = 0,
            string order = null) where TModel : new()
        {
            return ExecuteReader2(dbHelper, tableName, condition, topLimit, order).ToList<TModel>();
        }
        /// <summary>
        /// ExecuteReaderQueryString
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="tableName"></param>
        /// <param name="selectFields"></param>
        /// <param name="condition"></param>
        /// <param name="topLimit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static string ExecuteReaderQueryString(IDbHelper dbHelper, string tableName, string selectFields, string condition, int topLimit, string order)
        {
            var sb = Pool.StringBuilder.Get();

            // 2015-12-28 吉日嘎拉，简化程序，简化逻辑。
            switch (dbHelper.CurrentDbType)
            {
                case CurrentDbType.Oracle:
                    sb.Append("SELECT " + selectFields);
                    if (tableName.Trim().IndexOf(" ", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        sb.Append(" FROM (" + tableName + ")");
                    }
                    else
                    {
                        sb.Append(" FROM " + tableName);
                    }
                    if (!string.IsNullOrEmpty(condition))
                    {
                        sb.Append(" WHERE " + condition);
                    }
                    if (!string.IsNullOrEmpty(order))
                    {
                        sb.Append(" ORDER BY " + order);
                    }
                    if (topLimit > 0)
                    {
                        sb.Append("SELECT * FROM (" + sb.ToString() + ") WHERE ROWNUM < = " + topLimit);
                    }
                    break;

                case CurrentDbType.Access:
                case CurrentDbType.SqlServer:

                    if (topLimit > 0)
                    {
                        sb.Append("SELECT TOP " + topLimit + " " + selectFields);
                    }
                    else
                    {
                        sb.Append("SELECT " + selectFields);
                    }
                    if (tableName.Trim().IndexOf(" ", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        sb.Append(" FROM (" + tableName + ")");
                    }
                    else
                    {
                        sb.Append(" FROM " + tableName);
                    }
                    if (!string.IsNullOrEmpty(condition))
                    {
                        sb.Append(" WHERE " + condition);
                    }
                    if (!string.IsNullOrEmpty(order))
                    {
                        sb.Append(" ORDER BY " + order);
                    }
                    break;

                case CurrentDbType.MySql:
                case CurrentDbType.SqLite:
                    sb.Append("SELECT " + selectFields);
                    if (tableName.Trim().IndexOf(" ", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        sb.Append(" FROM (" + tableName + ")");
                    }
                    else
                    {
                        sb.Append(" FROM " + tableName);
                    }
                    if (!string.IsNullOrEmpty(condition))
                    {
                        sb.Append(" WHERE " + condition);
                    }
                    if (!string.IsNullOrEmpty(order))
                    {
                        sb.Append(" ORDER BY " + order);
                    }
                    if (topLimit > 0)
                    {
                        sb.Append(" LIMIT 0, " + topLimit);
                    }
                    break;
            }

            return sb.Put();
        }

        #region public static IDataReader ExecuteReader(IDbHelper dbHelper, string tableName, string name, object[] values, string order = null) 获取数据表 一参 参数为数组

        /// <summary>
        /// 获取数据表 一参 参数为数组
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">数据来源表名</param>
        /// <param name="selectField"></param>
        /// <param name="name">字段名</param>
        /// <param name="values">字段值</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        public static IDataReader ExecuteReader(IDbHelper dbHelper, string tableName, string selectField, string name, object[] values, string order = null)
        {
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT " + selectField + " FROM " + tableName);

            if (values == null || values.Length == 0)
            {
                sb.Append(" WHERE " + name + " IS NULL");
            }
            else
            {
                sb.Append(" WHERE " + name + " IN (" + string.Join(",", values) + ")");
            }
            if (!string.IsNullOrEmpty(order))
            {
                sb.Append(" ORDER BY " + order);
            }
            return dbHelper.ExecuteReader(sb.Put());
        }
        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="dbHelper"></param>
        /// <param name="tableName"></param>
        /// <param name="selectField"></param>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static List<TModel> ExecuteReader<TModel>(IDbHelper dbHelper, string tableName, string selectField, string name,
            object[] values, string order = null) where TModel : new()
        {
            return ExecuteReader(dbHelper, tableName, selectField, name, values, order).ToList<TModel>();
        }

        #endregion
        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="tableName"></param>
        /// <param name="parameters"></param>
        /// <param name="topLimit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static IDataReader ExecuteReader(IDbHelper dbHelper, string tableName, List<KeyValuePair<string, object>> parameters, int topLimit = 0, string order = null)
        {
            var sql = ExecuteReaderQueryString(dbHelper, tableName, "*", GetWhereString(dbHelper, parameters, BaseUtil.SqlLogicConditional), topLimit, order);
            if (parameters != null && parameters.Count > 0)
            {
                return dbHelper.ExecuteReader(sql, dbHelper.MakeParameters(parameters));
            }
            else
            {
                return dbHelper.ExecuteReader(sql);
            }
        }
        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="dbHelper"></param>
        /// <param name="tableName"></param>
        /// <param name="parameters"></param>
        /// <param name="topLimit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static List<TModel> ExecuteReader<TModel>(IDbHelper dbHelper, string tableName,
            List<KeyValuePair<string, object>> parameters, int topLimit = 0, string order = null) where TModel : new()
        {
            return ExecuteReader(dbHelper, tableName, parameters, topLimit, order).ToList<TModel>();
        }

        /// <summary>
        /// 参数化查询 
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="parameters">参数</param>
        /// <param name="conditions">条件</param>
        /// <param name="topLimit">前多少条</param>
        /// <param name="order">派讯</param>
        /// <param name="selectField">查询的字段</param>
        /// <returns></returns>
        public static IDataReader ExecuteReader(IDbHelper dbHelper, string tableName, List<KeyValuePair<string, object>> parameters, string conditions, int topLimit = 0, string order = null, string selectField = " * ")
        {
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT " + selectField + " FROM " + tableName);
            var whereSql = string.Empty;
            if (topLimit != 0)
            {
                switch (dbHelper.CurrentDbType)
                {
                    case CurrentDbType.Access:
                    case CurrentDbType.SqlServer:
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
            if (parameters != null && parameters.Count > 0)
            {
                return dbHelper.ExecuteReader(sb.Put(), dbHelper.MakeParameters(parameters));
            }
            else
            {
                return dbHelper.ExecuteReader(sb.Put());
            }
        }
        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="dbHelper"></param>
        /// <param name="tableName"></param>
        /// <param name="parameters"></param>
        /// <param name="conditions"></param>
        /// <param name="topLimit"></param>
        /// <param name="order"></param>
        /// <param name="selectField"></param>
        /// <returns></returns>
        public static List<TModel> ExecuteReader<TModel>(IDbHelper dbHelper, string tableName,
            List<KeyValuePair<string, object>> parameters, string conditions, int topLimit = 0, string order = null,
            string selectField = " * ") where TModel : new()
        {
            return ExecuteReader(dbHelper, tableName, parameters, conditions, topLimit, order, selectField)
                    .ToList<TModel>();
        }
    }
}