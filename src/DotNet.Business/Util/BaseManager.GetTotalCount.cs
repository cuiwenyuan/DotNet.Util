//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
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
        /// <param name="condition">查询条件(不包含WHERE和第一个AND)</param>
        /// <param name="days">最近多少天</param>
        /// <param name="currentWeek">当周</param>
        /// <param name="currentMonth">当月</param>
        /// <param name="currentQuarter">当季</param>
        /// <param name="currentYear">当年</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>总数</returns>
        public virtual int GetTotalCount(string condition = null, int days = 0, bool currentWeek = false, bool currentMonth = false, bool currentQuarter = false, bool currentYear = false, string startTime = null, string endTime = null)
        {
            var sb = PoolUtil.StringBuilder.Get().Append(" 1 = 1");

            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" AND " + condition);
            }
            if (days > 0)
            {
                sb.Append(" AND (DATEADD(d, " + days + ", " + BaseUtil.FieldCreateTime + ") > " + DbHelper.GetDbNow() + ")");
            }
            if (currentWeek)
            {
                sb.Append(" AND DATEDIFF(ww," + BaseUtil.FieldCreateTime + "," + DbHelper.GetDbNow() + ") = 0");
            }
            if (currentMonth)
            {
                sb.Append(" AND DATEDIFF(mm," + BaseUtil.FieldCreateTime + "," + DbHelper.GetDbNow() + ") = 0");
            }
            if (currentQuarter)
            {
                sb.Append(" AND DATEDIFF(qq," + BaseUtil.FieldCreateTime + "," + DbHelper.GetDbNow() + ") = 0");
            }
            if (currentYear)
            {
                sb.Append(" AND DATEDIFF(yy," + BaseUtil.FieldCreateTime + "," + DbHelper.GetDbNow() + ") = 0");
            }
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseUtil.FieldCreateTime + " >= " + startTime + ")");
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseUtil.FieldCreateTime + " < " + endTime + ")");
            }
            sb.Replace(" 1 = 1 AND ", " ");
            return DbHelper.Count(CurrentTableName, condition: sb.Return());
        }
        #endregion

        #region GetTotalDistinctCount
        /// <summary>
        /// 获取所有记录唯一值总数
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="condition">查询条件(不包含WHERE和第一个AND)</param>
        /// <param name="days">最近多少天</param>
        /// <param name="currentWeek">当周</param>
        /// <param name="currentMonth">当月</param>
        /// <param name="currentQuarter">当季</param>
        /// <param name="currentYear">当年</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>总数</returns>
        public virtual int GetTotalDistinctCount(string fieldName, string condition = null, int days = 0, bool currentWeek = false, bool currentMonth = false, bool currentQuarter = false, bool currentYear = false, string startTime = null, string endTime = null)
        {
            var sb = PoolUtil.StringBuilder.Get().Append(" 1 = 1");
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" AND " + condition);
            }
            if (days > 0)
            {
                sb.Append(" AND (DATEADD(d, " + days + ", " + BaseUtil.FieldCreateTime + ") > " + DbHelper.GetDbNow() + ")");
            }
            if (currentWeek)
            {
                sb.Append(" AND DATEDIFF(ww," + BaseUtil.FieldCreateTime + "," + DbHelper.GetDbNow() + ") = 0");
            }
            if (currentMonth)
            {
                sb.Append(" AND DATEDIFF(mm," + BaseUtil.FieldCreateTime + "," + DbHelper.GetDbNow() + ") = 0");
            }
            if (currentQuarter)
            {
                sb.Append(" AND DATEDIFF(qq," + BaseUtil.FieldCreateTime + "," + DbHelper.GetDbNow() + ") = 0");
            }
            if (currentYear)
            {
                sb.Append(" AND DATEDIFF(yy," + BaseUtil.FieldCreateTime + "," + DbHelper.GetDbNow() + ") = 0");
            }
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseUtil.FieldCreateTime + " >= " + startTime + ")");
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseUtil.FieldCreateTime + " < " + endTime + ")");
            }
            sb.Replace(" 1 = 1 AND ", " ");
            return DbHelper.DistinctCount(CurrentTableName, fieldName, condition: sb.Return());
        }
        #endregion

        #region GetActiveTotalCount
        /// <summary>
        /// 获取有效记录总数
        /// </summary>
        /// <param name="condition">查询条件(不包含WHERE和第一个AND)</param>
        /// <param name="days">最近多少天</param>
        /// <param name="currentWeek">当周</param>
        /// <param name="currentMonth">当月</param>
        /// <param name="currentQuarter">当季</param>
        /// <param name="currentYear">当年</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>总数</returns>
        public virtual int GetActiveTotalCount(string condition = null, int days = 0, bool currentWeek = false, bool currentMonth = false, bool currentQuarter = false, bool currentYear = false, string startTime = null, string endTime = null)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append(BaseUtil.FieldDeleted + " = 0 AND " + BaseUtil.FieldEnabled + " = 1");
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" AND " + condition);
            }
            return GetTotalCount(condition: sb.Return(), days: days, currentWeek: currentWeek, currentMonth: currentMonth, currentQuarter: currentQuarter, currentYear: currentYear, startTime: startTime, endTime: endTime);
        }
        #endregion

        #region GetActiveTotalDistinctCount
        /// <summary>
        /// 获取有效唯一值记录总数
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="condition">查询条件(不包含WHERE和第一个AND)</param>
        /// <param name="days">最近多少天</param>
        /// <param name="currentWeek">当周</param>
        /// <param name="currentMonth">当月</param>
        /// <param name="currentQuarter">当季</param>
        /// <param name="currentYear">当年</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>总数</returns>
        public virtual int GetActiveTotalDistinctCount(string fieldName, string condition = null, int days = 0, bool currentWeek = false, bool currentMonth = false, bool currentQuarter = false, bool currentYear = false, string startTime = null, string endTime = null)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append(BaseUtil.FieldDeleted + " = 0 AND " + BaseUtil.FieldEnabled + " = 1");
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" AND " + condition);
            }
            return GetTotalDistinctCount(fieldName, condition: sb.Return(), days: days, currentWeek: currentWeek, currentMonth: currentMonth, currentQuarter: currentQuarter, currentYear: currentYear, startTime: startTime, endTime: endTime);
        }
        #endregion
    }
}