//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Util
{
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
        #region public static string[] GetProperties(this IDbHelper dbHelper, string tableName, string name, Object[] values, string targetField) 获取数据表
        /// <summary>
        /// 获取数据表
        /// 这个方法按道理目标数据不会非常大，所以可以不优化，问题不大
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表名</param>
        /// <param name="name">字段名</param>
        /// <param name="values">字段值</param>
        /// <param name="targetField">目标字段</param>
        /// <returns>数据表</returns>
        public static string[] GetProperties(this IDbHelper dbHelper, string tableName, string name, Object[] values, string targetField)
        {
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT " + targetField + " FROM " + tableName + "  WHERE " + name + " IN (" + string.Join(",", values) + ")");
            var dt = dbHelper.Fill(sb.Put());
            return BaseUtil.FieldToArray(dt, targetField).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
        }
        #endregion

        #region public static string[] GetProperties(this IDbHelper dbHelper, string tableName, List<KeyValuePair<string, object>> parameters, int? topLimit = null, string targetField = null) 获取数据权限
        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="parameters">字段名,字段值</param>
        /// <param name="topLimit">前几个记录</param>
        /// <param name="targetField">目标字段</param>
        /// <returns>数据表</returns>
        public static string[] GetProperties(this IDbHelper dbHelper, string tableName, List<KeyValuePair<string, object>> parameters, int? topLimit = null, string targetField = null)
        {
            var result = new List<string>();

            if (string.IsNullOrEmpty(targetField))
            {
                targetField = BaseUtil.FieldId;
            }
            // 这里是需要完善的功能，完善了这个，是一次重大突破           
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT DISTINCT " + targetField + " FROM " + tableName);
            var whereSql = string.Empty;
            if (topLimit != null && topLimit > 0)
            {
                switch (dbHelper.CurrentDbType)
                {
                    case CurrentDbType.Access:
                    case CurrentDbType.SqlServer:
                        sb.Clear();
                        sb.Append("SELECT TOP " + topLimit + targetField + " FROM " + tableName);
                        break;
                    case CurrentDbType.Oracle:
                        whereSql = " ROWNUM < = " + topLimit;
                        break;
                    case CurrentDbType.MySql:
                        sb.Append(" LIMIT 0, " + topLimit);
                        break;
                }
            }
            var subSql = GetWhereString(dbHelper, parameters, BaseUtil.SqlLogicConditional);
            if (subSql.Length > 0)
            {
                if (whereSql.Length > 0)
                {
                    whereSql = whereSql + BaseUtil.SqlLogicConditional + subSql;
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

            // 20151008 吉日嘎拉 优化为 DataReader 读取数据，大量数据读取时，效率高，节约内存，提高处理效率
            var dataReader = dbHelper.ExecuteReader(sb.Put(), dbHelper.MakeParameters(parameters));
            if (dataReader != null && !dataReader.IsClosed)
            {
                while (dataReader.Read())
                {
                    result.Add(dataReader[targetField].ToString());
                }

                dataReader.Close();
            }

            return result.ToArray();
        }
        #endregion
    }
}