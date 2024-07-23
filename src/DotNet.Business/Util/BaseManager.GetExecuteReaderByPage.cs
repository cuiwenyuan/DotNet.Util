//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Util;

    /// <summary>
    ///	BaseManager
    /// 通用基类部分（分页）
    /// 
    /// 总觉得自己写的程序不上档次，这些新技术也玩玩，也许做出来的东西更专业了。
    /// 修改记录
    /// 
    ///		2012.02.04 版本：1.0 JiRiGaLa 进行提炼，把代码进行分组。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.02.04</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region public virtual IDataReader ExecuteReaderByPage(out int recordCount, int pageNo, int pageSize, string condition, IDbDataParameter[] dbParameters, string order)

        /// <summary>
        /// 获取分页DataTable
        /// </summary>
        /// <param name="recordCount">记录总数</param>
        /// <param name="pageNo">当前页数</param>
        /// <param name="pageSize">每页显示多少条</param>
        /// <param name="condition">条件</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="order">排序字段(不包含ORDER BY)</param>
        /// <returns>数据表</returns>
        public virtual IDataReader ExecuteReaderByPage(out int recordCount, int pageNo, int pageSize, string condition, IDbDataParameter[] dbParameters, string order)
        {
            recordCount = DbHelper.GetCount(CurrentTableName, condition, dbParameters, CurrentIndex);
            return DbHelper.ExecuteReaderByPage(CurrentTableName, SelectFields, pageNo, pageSize, condition, dbParameters, order, CurrentIndex);
        }

        #endregion

        #region public virtual List<TModel> ExecuteReaderByPage<TModel>(out int recordCount, int pageNo, int pageSize, string condition, IDbDataParameter[] dbParameters, string order) where TModel : new()

        /// <summary>
        /// 获取分页DataTable
        /// </summary>
        /// <param name="recordCount">记录总数</param>
        /// <param name="pageNo">当前页数</param>
        /// <param name="pageSize">每页显示多少条</param>
        /// <param name="condition">条件</param>
        /// <param name="dbParameters">数据库参数</param>
        /// <param name="order">排序字段(不包含ORDER BY)</param>
        /// <returns>数据表</returns>
        public virtual List<TModel> ExecuteReaderByPage<TModel>(out int recordCount, int pageNo, int pageSize, string condition, IDbDataParameter[] dbParameters, string order) where TModel : new()
        {
            return ExecuteReaderByPage(out recordCount, pageNo, pageSize, condition, dbParameters, order).ToList<TModel>();
        }

        #endregion

        #region public virtual IDataReader ExecuteReaderByPage(out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = null, string sortDirection = null, string tableName = null, string condition = null, IDbDataParameter[] dbParameters = null, string selectField = null)
        /// <summary>
        /// 分页读取数据
        /// </summary>
        /// <param name="recordCount">条数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序顺序</param>
        /// <param name="tableName">表名</param>
        /// <param name="condition">查询条件</param>
        /// <param name="dbParameters">数据参数</param>
        /// <param name="selectField">选择哪些字段</param>
        /// <returns>数据表</returns>
        public virtual IDataReader ExecuteReaderByPage(out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = null, string sortDirection = null, string tableName = null, string condition = null, IDbDataParameter[] dbParameters = null, string selectField = null)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = CurrentTableName;
            }
            if (tableName.ToUpper().IndexOf("SELECT", StringComparison.Ordinal) >= 0 || DbHelper.CurrentDbType == CurrentDbType.MySql || DbHelper.CurrentDbType == CurrentDbType.Oracle)
            {
                // 统计总条数
                if (string.IsNullOrEmpty(tableName))
                {
                    tableName = CurrentTableName;
                }
                var sb = PoolUtil.StringBuilder.Get();
                if (!string.IsNullOrEmpty(condition))
                {
                    sb.Append(" WHERE " + condition);
                }
                var commandText = tableName;
                if (tableName.ToUpper().IndexOf("SELECT", StringComparison.Ordinal) >= 0)
                {
                    commandText = "(" + tableName + ") T ";
                    // commandText = "(" + tableName + ") AS T ";
                }
                commandText = string.Format("SELECT COUNT(*) AS recordCount FROM {0} {1}", commandText, sb.Return());
                var obj = DbHelper.ExecuteScalar(commandText, dbParameters);
                if (obj != null)
                {
                    recordCount = obj.ToInt();
                }
                else
                {
                    recordCount = 0;
                }
                return DbHelper.ExecuteReaderByPage(recordCount, pageNo, pageSize, tableName, dbParameters, sortExpression, sortDirection);
            }
            // 这个是调用存储过程的方法
            return DbHelper.ExecuteReaderByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, tableName, condition, selectField);
        }

        #endregion

        #region public virtual List<TModel> ExecuteReaderByPage<TModel>(out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = null, string sortDirection = null, string tableName = null, string condition = null, IDbDataParameter[] dbParameters = null, string selectField = null) where TModel : new()

        /// <summary>
        /// 分页读取数据
        /// </summary>
        /// <param name="recordCount">条数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序顺序</param>
        /// <param name="tableName">表名</param>
        /// <param name="condition">查询条件</param>
        /// <param name="dbParameters">数据参数</param>
        /// <param name="selectField">选择哪些字段</param>
        /// <returns>数据表</returns>
        public virtual List<TModel> ExecuteReaderByPage<TModel>(out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = null, string sortDirection = null, string tableName = null, string condition = null, IDbDataParameter[] dbParameters = null, string selectField = null) where TModel : new()
        {
            return ExecuteReaderByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, tableName, condition, dbParameters, selectField).ToList<TModel>();
        }

        #endregion
    }
}