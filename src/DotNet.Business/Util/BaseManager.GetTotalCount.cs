//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using DotNet.Util;
using System.Data;

namespace DotNet.Business
{
    /// <summary>
    ///	BaseManager
    /// 通用基类部分
    /// 
    /// 修改记录
    /// 
    ///		2020.10.21 版本：Troy.Cui进行扩展。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2020.10.21</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region GetTotalCount
        /// <summary>
        /// 获取所有记录总数
        /// </summary>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <returns>总数</returns>
        public virtual int GetTotalCount(string condition = null)
        {
            return DbUtil.Count(DbHelper, CurrentTableName, condition: condition);
        }
        #endregion

        #region GetTotalDistinctCount
        /// <summary>
        /// 获取所有记录唯一值总数
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <returns>总数</returns>
        public virtual int GetTotalDistinctCount(string fieldName, string condition = null)
        {
            return DbUtil.DistinctCount(DbHelper, CurrentTableName, fieldName, condition: condition);
        }
        #endregion

        #region GetActiveTotalCount
        /// <summary>
        /// 获取有效记录总数
        /// </summary>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <returns>总数</returns>
        public virtual int GetActiveTotalCount(string condition = null)
        {
            var sb = Pool.StringBuilder.Get();
            sb.Append((BaseUtil.FieldDeleted) + " = 0 AND " + BaseUtil.FieldEnabled + " = 1");
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" AND " + condition);
            }
            return DbUtil.Count(DbHelper, CurrentTableName, condition: sb.Put());
        }
        #endregion

        #region GetActiveTotalDistinctCount
        /// <summary>
        /// 获取有效唯一值记录总数
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <returns>总数</returns>
        public virtual int GetActiveTotalDistinctCount(string fieldName, string condition = null)
        {
            var sb = Pool.StringBuilder.Get();
            sb.Append((BaseUtil.FieldDeleted) + " = 0 AND " + BaseUtil.FieldEnabled + " = 1");
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" AND " + condition);
            }
            return DbUtil.DistinctCount(DbHelper, CurrentTableName, fieldName, condition: sb.Put());
        }
        #endregion
    }
}