//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;

namespace DotNet.Util
{
    /// <summary>
    ///	DbUtil
    /// 通用基类
    /// 
    /// 修改记录
    /// 2016-05-21增加条件参数
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.05.21</date>
    /// </author> 
    /// </summary>
    public partial class DbUtil
    {
        #region public static DataTable GetDataTableByPage(this IDbHelper dbHelper, int recordCount, int pageNo, int pageSize, string sql, IDbDataParameter[] dbParameters, string sortExpression = null, string sortDirection = null)

        /// <summary>
        /// 分页获取指定数量的数据
        /// </summary>
        /// <param name="dbHelper">数据源</param>
        /// <param name="recordCount">获取多少条</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="sql"></param>
        /// <param name="condition">条件</param>
        /// <param name="dbParameters"></param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序</param>
        /// <returns></returns>
        public static DataTable GetDataTableByPage(this IDbHelper dbHelper, int recordCount, int pageNo, int pageSize, string sql, string condition, IDbDataParameter[] dbParameters, string sortExpression = null, string sortDirection = null)
        {
            if (string.IsNullOrEmpty(sortExpression))
            {
                sortExpression = BaseUtil.FieldCreateTime;
            }
            if (string.IsNullOrEmpty(sortDirection))
            {
                sortDirection = " DESC";
            }
            var sqlCount = recordCount - ((pageNo - 1) * pageSize) > pageSize ? pageSize.ToString() : (recordCount - ((pageNo - 1) * pageSize)).ToString();
            // string sqlStart = (pageNo * pageSize).ToString();
            // string sqlEnd = ((pageNo + 1) * pageSize).ToString();
            var sqlStart = ((pageNo - 1) * pageSize).ToString();
            var sqlEnd = (pageNo * pageSize).ToString();

            var commandText = string.Empty;

            switch (dbHelper.CurrentDbType)
            {
                case CurrentDbType.SqlServer:
                    //Troy Cui 20160521 针对MSSQL自动构造T别名表，如果手动构造过T，就不用了。
                    if (sql.ToUpper().Trim().StartsWith("SELECT") && !sql.ToUpper().Trim().EndsWith(")T"))
                    {
                        sql = "(" + sql + ") T ";
                        // sql = "(" + sql + ") AS T ";
                    }
                    //Troy 20160605 将条件放在内部，解决ROWNUM的筛选不到的bug。
                    if (!string.IsNullOrEmpty(condition))
                    {
                        sql = "(SELECT * FROM " + sql + " WHERE " + condition + ") T";
                    }
                    sqlStart = ((pageNo - 1) * pageSize).ToString();
                    sqlEnd = (pageNo * pageSize).ToString();
                    commandText = "SELECT * FROM ( "
                                  + "SELECT ROW_NUMBER() OVER(ORDER BY " + sortExpression + " " + sortDirection + ") AS ROWNUM, "
                                  //+ sql.Substring(7) + " ) A "
                                  + "* FROM " + sql + " ) A "
                                  + " WHERE ROWNUM > " + sqlStart + " AND ROWNUM <= " + sqlEnd;
                    break;
                case CurrentDbType.Db2:
                    sqlStart = ((pageNo - 1) * pageSize).ToString();
                    sqlEnd = (pageNo * pageSize).ToString();
                    commandText = "SELECT * FROM ( "
                               + "SELECT ROW_NUMBER() OVER(ORDER BY " + sortExpression + " " + sortDirection + ") AS ROWNUM, "
                               //+ sql.Substring(7) + " ) A "
                               + "* FROM " + sql + " ) A "
                               + " WHERE ROWNUM > " + sqlStart + " AND ROWNUM <= " + sqlEnd;

                    break;
                case CurrentDbType.Access:
                    if (sql.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        sql = " (" + sql + ") ";
                    }
                    commandText = string.Format("SELECT * FROM (SELECT TOP {0} * FROM (SELECT TOP {1} * FROM {2} T ORDER BY {3} " + sortDirection + ") T1 ORDER BY {4} DESC ) T2 ORDER BY {5} " + sortDirection
                                    , sqlCount, sqlStart, sql, sortExpression, sortExpression, sortExpression);
                    break;
                case CurrentDbType.Oracle:
                    //commandText = string.Format(@"SELECT T.*, ROWNUM RN FROM ({0} AND ROWNUM <= {1} ORDER BY {2}) T WHERE ROWNUM > {3}", sql, sqlEnd, sortExpression, sqlStart);
                    //Troy Cui 2017.12.14 Oracle分页
                    if (sql.ToUpper().Trim().StartsWith("SELECT") && !sql.ToUpper().Trim().EndsWith(")T"))
                    {
                        //将条件放在内部，解决筛选不到的bug
                        if (!string.IsNullOrEmpty(condition))
                        {
                            sql += " AND " + condition + "";
                        }
                        if (!string.IsNullOrEmpty(sortExpression) && !string.IsNullOrEmpty(sortDirection))
                        {
                            sql += " ORDER BY " + sortExpression + " " + sortDirection + "";
                        }
                        sql = "(" + sql + ") T ";
                        // sql = "(" + sql + ") AS T ";
                    }
                    else
                    {
                        //将条件放在内部，解决筛选不到的bug
                        if (!string.IsNullOrEmpty(condition))
                        {
                            sql += " WHERE " + condition + "";
                        }
                        if (!string.IsNullOrEmpty(sortExpression) && !string.IsNullOrEmpty(sortDirection))
                        {
                            sql += " ORDER BY " + sortExpression + " " + sortDirection + "";
                        }
                        //Troy Cui 2018.03.14 bug修复
                        sql = "(SELECT * FROM " + sql + ") T ";
                    }
                    commandText = string.Format(@"SELECT * FROM (SELECT T.*, ROWNUM RN FROM {0} WHERE ROWNUM <= {2}) T WHERE RN > {1}", sql, sqlStart, sqlEnd);
                    break;
                case CurrentDbType.MySql:
                    if (sql.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        sql = " (" + sql + ") ";
                    }
                    sqlStart = ((pageNo - 1) * pageSize).ToString();
                    sqlEnd = (pageNo * pageSize).ToString();
                    commandText = string.Format("SELECT * FROM {0} ORDER BY {1} {2} LIMIT {3},{4}", sql, sortExpression, sortDirection, sqlStart, sqlEnd);
                    break;
            }
            return dbHelper.Fill(commandText, dbParameters);
        }
        #endregion
    }
}