//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------


using System.Data;

namespace DotNet.Business
{
    using Util;

    /// <summary>
    ///	BaseManager
    /// 通用基类部分
    /// 
    /// 修改记录
    /// 
    ///		2020.08.28 版本：Troy.Cui进行扩展。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2020.08.28</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region 获取最新的N条记录

        /// <summary>
        /// 获取最新的N条记录
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="tableName">表名</param>
        /// <param name="rows">几条记录</param>
        /// <param name="sortField">排序字段</param>
        /// <returns></returns>
        public virtual DataTable GetDataTableLatest(string condition, string tableName = null, int rows = 1, string sortField = BaseUtil.FieldId)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = CurrentTableName;
            }

            var sb = PoolUtil.StringBuilder.Get();

            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.Access:
                case CurrentDbType.SqLite:
                case CurrentDbType.SqlServer:
                    sb.Append("SELECT TOP " + rows + " * FROM " + tableName);
                    if (!string.IsNullOrEmpty(condition))
                    {
                        sb.Append(" WHERE " + condition);
                    }
                    sb.Append(" ORDER BY " + sortField + " DESC");
                    break;
                case CurrentDbType.Oracle:
                    sb.Append("SELECT * FROM " + tableName);
                    if (!string.IsNullOrEmpty(condition))
                    {
                        sb.Append(" WHERE " + condition);
                    }
                    sb.Append(" AND ROWNUM <= " + rows + " ORDER BY " + sortField + " DESC");
                    break;
                case CurrentDbType.MySql:
                    sb.Append("SELECT * FROM " + tableName);
                    if (!string.IsNullOrEmpty(condition))
                    {
                        sb.Append(" WHERE " + condition);
                    }
                    sb.Append(" ORDER BY " + sortField + " DESC LIMIT " + rows + "");
                    break;

            }

            return Fill(sb.Return());
        }
        #endregion
    }
}