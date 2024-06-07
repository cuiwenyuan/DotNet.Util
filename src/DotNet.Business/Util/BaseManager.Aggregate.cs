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
    ///		2022.01.13 版本：Troy.Cui进行扩展。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2022.01.13</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region GetSumInt
        /// <summary>
        /// 获取指定字段的聚合函数值
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <returns>总数</returns>
        public virtual int GetSumInt(string fieldName, string condition = null)
        {
            return DbHelper.AggregateInt(CurrentTableName, fieldName, condition: condition, function: "SUM");
        }
        #endregion

        #region GetSumDecimal
        /// <summary>
        /// 获取指定字段的聚合函数值
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <returns>总数</returns>
        public virtual decimal GetSumDecimal(string fieldName, string condition = null)
        {
            return DbHelper.AggregateDecimal(CurrentTableName, fieldName, condition: condition, function: "SUM");
        }
        #endregion

        #region GetAvgInt
        /// <summary>
        /// 获取指定字段的聚合函数值
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <returns>总数</returns>
        public virtual int GetAvgInt(string fieldName, string condition = null)
        {
            return DbHelper.AggregateInt(CurrentTableName, fieldName, condition: condition, function: "AVG");
        }
        #endregion

        #region GetAvgDecimal
        /// <summary>
        /// 获取指定字段的聚合函数值
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <returns>总数</returns>
        public virtual decimal GetAvgDecimal(string fieldName, string condition = null)
        {
            return DbHelper.AggregateDecimal(CurrentTableName, fieldName, condition: condition, function: "AVG");
        }
        #endregion

        #region GetMinInt
        /// <summary>
        /// 获取指定字段的聚合函数值
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <returns>总数</returns>
        public virtual int GetMinInt(string fieldName, string condition = null)
        {
            return DbHelper.AggregateInt(CurrentTableName, fieldName, condition: condition, function: "MIN");
        }
        #endregion

        #region GetMinDecimal
        /// <summary>
        /// 获取指定字段的聚合函数值
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <returns>总数</returns>
        public virtual decimal GetDecimal(string fieldName, string condition = null)
        {
            return DbHelper.AggregateDecimal(CurrentTableName, fieldName, condition: condition, function: "MIN");
        }
        #endregion

        #region GetMaxInt
        /// <summary>
        /// 获取指定字段的聚合函数值
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <returns>总数</returns>
        public virtual int GetMaxInt(string fieldName, string condition = null)
        {
            return DbHelper.AggregateInt(CurrentTableName, fieldName, condition: condition, function: "MAX");
        }
        #endregion

        #region GetMaxDecimal
        /// <summary>
        /// 获取指定字段的聚合函数值
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <returns>总数</returns>
        public virtual decimal GetMaxDecimal(string fieldName, string condition = null)
        {
            return DbHelper.AggregateDecimal(CurrentTableName, fieldName, condition: condition, function: "MAX");
        }
        #endregion
    }
}