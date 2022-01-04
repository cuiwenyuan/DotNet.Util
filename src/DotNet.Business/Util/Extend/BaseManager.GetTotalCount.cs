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
    /// 修改纪录
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
        #region 获取记录总数
        /// <summary>
        /// 获取所有记录总数
        /// </summary>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <returns>总数</returns>
        public virtual int GetTotalCount(string condition = null)
        {
            var result = 0;
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) AS TotalCount FROM " + CurrentTableName);
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" WHERE " + condition);
            }

            var obj = ExecuteScalar(sb.Put()) ?? 0;
            result = BaseUtil.ConvertToInt(obj);
            return result;
        }

        /// <summary>
        /// 获取有效记录总数
        /// </summary>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <param name="tableVersion">版本默认4为老版本</param>
        /// <returns>总数</returns>
        public virtual int GetActiveTotalCount(string condition = null, int tableVersion = 4)
        {
            var result = 0;
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) AS TotalCount FROM " + CurrentTableName + " WHERE " + (tableVersion == 4 ? BaseUtil.FieldDeletionStateCode : BaseUtil.FieldDeleted) + " = 0 AND Enabled = 1");
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" AND " + condition);
            }
            var obj = ExecuteScalar(sb.Put()) ?? 0;
            result = BaseUtil.ConvertToInt(obj);
            return result;
        }
        #endregion
    }
}