//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

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
        #region public int static Delete(this IDbHelper dbHelper, string tableName, List<KeyValuePair<string, object>> parameters = null) 删除表数据
        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表名</param>
        /// <param name="parameters">删除条件</param>
        /// <returns>影响行数</returns>
        public static int Delete(this IDbHelper dbHelper, string tableName, List<KeyValuePair<string, object>> parameters = null)
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

        #region public int static Delete(this IDbHelper dbHelper, string tableName, string condition) 删除表数据
        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表名</param>
        /// <param name="condition">删除条件(不含WHERE)</param>
        /// <returns>影响行数</returns>
        public static int Delete(this IDbHelper dbHelper, string tableName, string condition)
        {
            var sb = Pool.StringBuilder.Get();
            sb.Append("DELETE FROM " + tableName);
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" WHERE " + condition);
            }
            return dbHelper.ExecuteNonQuery(sb.Put());
        }
        #endregion

        #region public void BatchDelete(this IDbHelper dbHelper, string tableName, string condition, int batchSize = 100)
        /// <summary>
        /// 批量删除（针对超过几百万行的表进行批次删除）
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表名</param>
        /// <param name="condition">删除条件(不含WHERE)</param>
        /// <param name="batchSize">批次大小</param>
        public static void BatchDelete(this IDbHelper dbHelper, string tableName, string condition, int batchSize = 100)
        {
            var minId = dbHelper.AggregateInt(tableName, BaseUtil.FieldId, condition: condition, function: "MIN");
            if (minId > 0)
            {
                var maxId = minId + batchSize;
                dbHelper.Delete(tableName, condition + " AND " + BaseUtil.FieldId + " > " + minId + " AND " + BaseUtil.FieldId + " <=" + maxId);
                BatchDelete(dbHelper, tableName, condition, batchSize: batchSize);
            }
        }
        #endregion

        #region public static int Truncate(this IDbHelper dbHelper, string tableName) 截断表数据
        /// <summary>
        /// 截断表数据
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表格</param>
        /// <returns>是否成功</returns>
        public static int Truncate(this IDbHelper dbHelper, string tableName)
        {
            var sb = Pool.StringBuilder.Get();
            sb.Append("TRUNCATE TABLE " + tableName);
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