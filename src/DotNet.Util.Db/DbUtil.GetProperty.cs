//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DotNet.Util
{
    /// <summary>
    ///	DbUtil
    /// 通用基类
    /// 
    /// 修改记录
    /// 
    ///		2016.02.27 版本：2.1	JiRiGaLa 提高数据库中的读取性能。
    ///		2012.03.20 版本：2.0	JiRiGaLa 整理参数传递方法。
    ///		2012.02.05 版本：1.0	JiRiGaLa 分离程序。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.02.27</date>
    /// </author> 
    /// </summary>
    public partial class DbUtil
    {
        #region public static string GetProperty(this IDbHelper dbHelper, string tableName, List<KeyValuePair<string, object>> parameters, string targetField, int? topLimit = null, string orderBy = null) 读取属性
        /// <summary>
        /// 读取属性
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表名</param>
        /// <param name="parameters">字段名,键值</param>
        /// <param name="targetField">获取字段</param>
        /// <param name="topLimit">TOP多少行</param>
        /// <param name="orderBy">排序</param>
        /// <returns>属性</returns>
        public static string GetProperty(this IDbHelper dbHelper, string tableName, List<KeyValuePair<string, object>> parameters, string targetField, int topLimit = 1, string orderBy = null)
        {
            var result = string.Empty;

            if (string.IsNullOrEmpty(targetField))
            {
                targetField = BaseUtil.FieldId;
            }
            // 这里是需要完善的功能，完善了这个，是一次重大突破           
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + targetField + " FROM " + tableName);
            var whereSql = string.Empty;
            if (topLimit > 0)
            {
                switch (dbHelper.CurrentDbType)
                {
                    case CurrentDbType.Access:
                    case CurrentDbType.SqlServer:
                        sb.Clear();
                        sb.Append("SELECT TOP " + topLimit + " " + targetField + " FROM " + tableName);
                        break;
                    case CurrentDbType.Oracle:
                        whereSql = " ROWNUM < = " + topLimit;
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

            if (!string.IsNullOrEmpty(orderBy))
            {
                sb.Append(" ORDER BY " + orderBy);
            }

            if (topLimit > 0)
            {
                switch (dbHelper.CurrentDbType)
                {
                    case CurrentDbType.MySql:
                        sb.Append(" LIMIT 0, " + topLimit);
                        break;
                }
            }

            var obj = dbHelper.ExecuteScalar(sb.Return(), dbHelper.MakeParameters(parameters));
            if (obj != null && obj != DBNull.Value)
            {
                result = obj.ToString();
            }

            return result;
        }
        #endregion
    }
}