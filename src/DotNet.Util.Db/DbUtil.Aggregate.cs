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
    ///     2021.12.31 版本：1.0	Troy Cui 创建。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.12.31</date>
    /// </author> 
    /// </summary>
    public partial class DbUtil
    {
        #region Aggregate
        /// <summary>
        /// 获取聚合函数值
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">数据来源表名</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <param name="function">聚合函数AVG,MAX,MIN,SUM</param>
        /// <returns>数据表</returns>
        public static int Aggregate(IDbHelper dbHelper, string tableName, string fieldName, string condition = null, string function = "SUM")
        {
            var result = 0;
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT " + function + "(" + fieldName + ") FROM " + tableName);
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" WHERE " + condition);
            }
            var obj = dbHelper.ExecuteScalar(sb.Put());
            if (obj != null && obj != DBNull.Value)
            {
                result = Convert.ToInt32(obj.ToString());
            }
            return result;
        }
        #endregion
    }
}