//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Util
{
    /// <summary>
    ///	DbUtil
    /// 通用基类 设置各种属性
    /// 
    /// 修改记录
    /// 
    ///		2022.02.05 版本：1.0	Troy.Cui 分离程序。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2022.02.05</date>
    /// </author> 
    /// </summary>
    public partial class DbUtil
    {
        #region public static int SetProperty(this IDbHelper dbHelper, string tableName, List<KeyValuePair<string, object>> whereParameters, List<KeyValuePair<string, object>> parameters) 设置属性
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表名</param>
        /// <param name="whereParameters">条件字段,条件值</param>
        /// <param name="parameters">更新字段,更新值</param>
        /// <param name="whereSql">条件Sql</param>
        /// <returns>影响行数</returns>
        public static int SetProperty(this IDbHelper dbHelper, string tableName, List<KeyValuePair<string, object>> whereParameters, List<KeyValuePair<string, object>> parameters, string whereSql = null)
        {
            var sqlBuilder = new SqlBuilder(dbHelper);
            sqlBuilder.BeginUpdate(tableName);
            foreach (var parameter in parameters)
            {
                sqlBuilder.SetValue(parameter.Key, parameter.Value);
            }
            // 先设置参数条件
            sqlBuilder.SetWhere(whereParameters);
            // 后设置手写的SQL条件
            if (!string.IsNullOrEmpty(whereSql))
            {
                sqlBuilder.SetWhere(whereSql);
            }
            return sqlBuilder.EndUpdate();
        }
        #endregion
    }
}