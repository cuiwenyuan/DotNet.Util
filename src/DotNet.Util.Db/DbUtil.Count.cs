﻿//-----------------------------------------------------------------
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
        #region public static int Count(this IDbHelper dbHelper, string tableName, string condition = null) 获取记录数
        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <returns>数据表</returns>
        public static int Count(this IDbHelper dbHelper, string tableName, string condition = null)
        {
            var result = 0;
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) FROM " + tableName);
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

        #region public static int DistinctCount(this IDbHelper dbHelper, string tableName, string fieldName, string condition = null) 获取唯一值记录数
        /// <summary>
        /// 获取唯一值记录数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <returns>数据表</returns>
        public static int DistinctCount(this IDbHelper dbHelper, string tableName, string fieldName, string condition = null)
        {
            var result = 0;
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT COUNT(DISTINCT " + fieldName + ") FROM " + tableName);
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
    }
}