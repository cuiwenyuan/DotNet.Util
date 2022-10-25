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
    ///     2015.01.31 版本：2.2    潘齐民   修改分页方法，SqlServer库查询条件从Between strat And end 改成小于等于end 大于start
    ///     2014.08.08 版本：2.1    SongBiao 修改分页方法 多表联合显示查询字段的位置
    ///     2014.01.23 版本：2.o    JiRiGaLa 整理 Oracle 分页功能
    ///     2013.11.03 版本：1.1    HongMing Oracle 获取分页数据 增加MySQL
    ///		2012.02.05 版本：1.0	JiRiGaLa 分离程序。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.02.05</date>
    /// </author> 
    /// </summary>
    public partial class DbUtil
    {
        #region public static IDataReader ExecuteReaderByPage(this IDbHelper dbHelper, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = null, string sortDirection = null, string tableName = null, string condition = null, string selectField = " * ")
        /// <summary>
        /// 使用存储过程获取分页数据
        /// </summary>
        /// <param name="dbHelper">数据源</param>
        /// <param name="recordCount">返回的记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序</param>
        /// <param name="tableName">表名</param>
        /// <param name="condition">查询条件</param>
        /// <param name="selectField">查询字段</param>
        /// <returns></returns>
        public static IDataReader ExecuteReaderByPage(this IDbHelper dbHelper, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = null, string sortDirection = null, string tableName = null, string condition = null, string selectField = " * ")
        {
            IDataReader dataReader = null;
            recordCount = 0;
            if (string.IsNullOrEmpty(selectField))
            {
                selectField = "*";
            }
            if (string.IsNullOrEmpty(condition))
            {
                condition = string.Empty;
            }
            var dbParameters = new List<IDbDataParameter>();
            var dbDataParameter = dbHelper.MakeParameter("RecordCount", recordCount, DbType.Int64, 0, ParameterDirection.Output);
            dbParameters.Add(dbDataParameter);
            dbParameters.Add(dbHelper.MakeParameter("PageNo", pageNo));
            dbParameters.Add(dbHelper.MakeParameter("PageSize", pageSize));
            dbParameters.Add(dbHelper.MakeParameter("SortExpression", sortExpression));
            dbParameters.Add(dbHelper.MakeParameter("SortDirection", sortDirection));
            dbParameters.Add(dbHelper.MakeParameter("TableName", tableName));
            dbParameters.Add(dbHelper.MakeParameter("SelectField", selectField));
            dbParameters.Add(dbHelper.MakeParameter("WhereConditional", condition));
            dataReader = dbHelper.ExecuteReader("GetRecordByPage", dbParameters.ToArray(), CommandType.StoredProcedure);
            recordCount = int.Parse(dbDataParameter.Value.ToString());

            return dataReader;
        }

        /// <summary>
        /// 使用存储过程获取分页数据
        /// </summary>
        /// <param name="dbHelper">数据源</param>
        /// <param name="recordCount">返回的记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序</param>
        /// <param name="tableName">表名</param>
        /// <param name="condition">查询条件</param>
        /// <param name="selectField">查询字段</param>
        /// <returns></returns>
        public static List<TModel> ExecuteReaderByPage<TModel>(this IDbHelper dbHelper, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = null, string sortDirection = null, string tableName = null,
            string condition = null, string selectField = " * ") where TModel : new()
        {
            return
                ExecuteReaderByPage(dbHelper, out recordCount, pageNo, pageSize, sortExpression, sortDirection, tableName,
                    condition, selectField).ToList<TModel>();
        }

        #endregion

        #region public static IDataReader ExecuteReaderByPage(this IDbHelper dbHelper, int recordCount, int pageNo, int pageSize, string sql, IDbDataParameter[] dbParameters, string sortExpression = null, string sortDirection = null)

        /// <summary>
        /// 分页获取指定数量的数据
        /// </summary>
        /// <param name="dbHelper">数据源</param>
        /// <param name="recordCount">获取多少条</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="sql"></param>
        /// <param name="dbParameters"></param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序</param>
        /// <returns></returns>
        public static IDataReader ExecuteReaderByPage(this IDbHelper dbHelper, int recordCount, int pageNo, int pageSize, string sql, IDbDataParameter[] dbParameters, string sortExpression = null, string sortDirection = null)
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
            var sqlStart = ((pageNo - 1) * pageSize).ToString();
            var sqlEnd = (pageNo * pageSize).ToString();

            var commandText = string.Empty;

            switch (dbHelper.CurrentDbType)
            {
                case CurrentDbType.SqlServer:
                case CurrentDbType.Db2:
                    sqlStart = ((pageNo - 1) * pageSize).ToString();
                    sqlEnd = (pageNo * pageSize).ToString();
                    commandText = "SELECT * FROM ( " + "SELECT ROW_NUMBER() OVER (ORDER BY " + sortExpression + ") AS ROWNUM, " + sql.Substring(7) + "  ) A " + " WHERE ROWNUM > " + sqlStart + " AND ROWNUM <= " + sqlEnd;
                    break;
                case CurrentDbType.Access:
                    if (sql.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        sql = " (" + sql + ") ";
                    }
                    commandText = string.Format("SELECT * FROM (SELECT TOP {0} * FROM (SELECT TOP {1} * FROM {2} T ORDER BY {3} " + sortDirection + ") T1 ORDER BY {4} DESC ) T2 ORDER BY {5} " + sortDirection, sqlCount, sqlStart, sql, sortExpression, sortExpression, sortExpression);
                    break;
                case CurrentDbType.Oracle:
                    commandText = string.Format(@"SELECT T.*, ROWNUM RN FROM ({0} AND ROWNUM <= {1} ORDER BY {2}) T WHERE ROWNUM > {3}", sql, sqlEnd, sortExpression, sqlStart);
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

            return dbHelper.ExecuteReader(commandText, dbParameters);
        }

        /// <summary>
        /// 分页获取指定数量的数据
        /// </summary>
        /// <param name="dbHelper">数据源</param>
        /// <param name="recordCount">获取多少条</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="sql"></param>
        /// <param name="dbParameters">参数</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序</param>
        /// <returns></returns>
        public static List<TModel> ExecuteReaderByPage<TModel>(this IDbHelper dbHelper, int recordCount, int pageNo, int pageSize, string sql, IDbDataParameter[] dbParameters, string sortExpression = null, string sortDirection = null) where TModel : new()
        {
            return ExecuteReaderByPage(dbHelper, recordCount, pageNo, pageSize, sql, dbParameters, sortExpression, sortDirection).ToList<TModel>();
        }

        #endregion

        #region public static IDataReader ExecuteReaderByPage(this IDbHelper dbHelper, string tableName, string selectField, int pageNo, int pageSize, string conditions, string orderBy)
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="selectField">选择字段</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示多少条</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="orderBy">排序字段</param>
        /// <returns>数据表</returns>
        public static IDataReader ExecuteReaderByPage(this IDbHelper dbHelper, string tableName, string selectField, int pageNo, int pageSize, string conditions, string orderBy)
        {
            return ExecuteReaderByPage(dbHelper, tableName, selectField, pageNo, pageSize, conditions, null, orderBy);
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="selectField">选择字段</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示多少条</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="orderBy">排序字段</param>
        /// <returns>数据表</returns>
        public static List<TModel> ExecuteReaderByPage<TModel>(this IDbHelper dbHelper, string tableName, string selectField, int pageNo, int pageSize, string conditions, string orderBy) where TModel : new()
        {
            return ExecuteReaderByPage(dbHelper, tableName, selectField, pageNo, pageSize, conditions, orderBy).ToList<TModel>();
        }

        #endregion

        #region public static IDataReader ExecuteReaderByPage(this IDbHelper dbHelper, string tableName, string selectField, int pageNo, int pageSize, string conditions, IDbDataParameter[] dbParameters, string orderBy, string currentIndex = null)
        /// <summary>
        /// 获取分页数据（防注入功能的）
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="selectField">选择字段</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示多少条</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="dbParameters">查询参数</param>
        /// <param name="orderBy">排序字段</param>
        /// <param name="currentIndex">当前索引</param>
        /// <returns>数据表</returns>
        public static IDataReader ExecuteReaderByPage(this IDbHelper dbHelper, string tableName, string selectField, int pageNo, int pageSize, string conditions, IDbDataParameter[] dbParameters, string orderBy, string currentIndex = null)
        {
            var sqlStart = ((pageNo - 1) * pageSize).ToString();
            var sqlEnd = (pageNo * pageSize).ToString();
            if (currentIndex == null)
            {
                currentIndex = string.Empty;
            }
            if (!string.IsNullOrEmpty(conditions))
            {
                conditions = "WHERE " + conditions;
            }
            var sb = Pool.StringBuilder.Get();

            if (dbHelper.CurrentDbType == CurrentDbType.Oracle)
            {
                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    orderBy = " ORDER BY " + orderBy;
                }
                //宋彪修改 2014.8.8
                sb.Append(string.Format("SELECT * FROM (SELECT ROWNUM RN, TT.* FROM ((SELECT " + currentIndex + " " + selectField + " FROM {0} {1} {2} )TT)) ZZ WHERE ZZ.RN <={3} AND ZZ.RN >{4} ", tableName, conditions, orderBy, sqlEnd, sqlStart));
            }
            else if (dbHelper.CurrentDbType == CurrentDbType.SqlServer)
            {
                sb.Append(string.Format("SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {0}) AS RowIndex, " + selectField + " FROM {1} {2}) AS PageTable WHERE RowIndex <={3} AND RowIndex >{4} ", orderBy, tableName, conditions, sqlEnd, sqlStart));
            }
            else if (dbHelper.CurrentDbType == CurrentDbType.MySql || dbHelper.CurrentDbType == CurrentDbType.SqLite)
            {
                sb.Append(string.Format("SELECT {0} FROM {1} {2} ORDER BY {3} LIMIT {4}, {5}", selectField, tableName, conditions, orderBy, sqlStart, pageSize));
            }

            //var dt = new DataTable(tableName);
            if (dbParameters != null && dbParameters.Length > 0)
            {
                return dbHelper.ExecuteReader(sb.Put(), dbParameters);
            }
            else
            {
                return dbHelper.ExecuteReader(sb.Put());
            }
        }

        /// <summary>
        /// 获取分页数据（防注入功能的）
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="selectField">选择字段</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示多少条</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="dbParameters">查询参数</param>
        /// <param name="orderBy">排序字段</param>
        /// <param name="currentIndex">当前索引</param>
        /// <returns>数据表</returns>
        public static List<TModel> ExecuteReaderByPage<TModel>(this IDbHelper dbHelper, string tableName, string selectField, int pageNo, int pageSize, string conditions, IDbDataParameter[] dbParameters, string orderBy, string currentIndex = null) where TModel : new()
        {
            return ExecuteReaderByPage(dbHelper, tableName, selectField, pageNo, pageSize, conditions, dbParameters, orderBy, currentIndex).ToList<TModel>();
        }
        #endregion

        #region public static IDataReader ExecuteReaderByPage(this IDbHelper dbHelper, out int recordCount, string tableName, string selectField, int pageNo, int pageSize, string conditions, List<KeyValuePair<string, object>> dbParameters, string orderBy)
        /// <summary>
        /// 获取分页数据（防注入功能的）
        /// </summary>
        /// <param name="dbHelper">dbHelper</param>
        /// <param name="recordCount">记录条数</param>
        /// <param name="tableName">表名</param>
        /// <param name="selectField">选择字段</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示多少条</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="dbParameters">查询参数</param>
        /// <param name="orderBy">排序字段</param>
        /// <returns>数据表</returns>
        public static IDataReader ExecuteReaderByPage(this IDbHelper dbHelper, out int recordCount, string tableName, string selectField, int pageNo, int pageSize, string conditions, IDbDataParameter[] dbParameters, string orderBy)
        {
            IDataReader result = null;
            recordCount = 0;
            if (dbHelper != null)
            {
                recordCount = GetCount(dbHelper, tableName, conditions, dbParameters);
                result = ExecuteReaderByPage(dbHelper, tableName, selectField, pageNo, pageSize, conditions, dbParameters, orderBy);
            }
            return result;
        }

        /// <summary>
        /// 获取分页数据（防注入功能的） 
        /// </summary>
        /// <param name="recordCount">记录条数</param>
        /// <param name="dbHelper">dbHelper</param>
        /// <param name="tableName">表名</param>
        /// <param name="selectField">选择字段</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示多少条</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="dbParameters">查询参数</param>
        /// <param name="orderBy">排序字段</param>
        /// <returns>数据表</returns>
        public static List<TModel> ExecuteReaderByPage<TModel>(this IDbHelper dbHelper, out int recordCount, string tableName, string selectField, int pageNo, int pageSize, string conditions, IDbDataParameter[] dbParameters, string orderBy) where TModel : new()
        {
            return ExecuteReaderByPage(dbHelper, out recordCount, tableName, selectField, pageNo, pageSize, conditions, dbParameters, orderBy).ToList<TModel>();
        }

        #endregion
    }
}