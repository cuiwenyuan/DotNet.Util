//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using DotNet.Util;

namespace DotNet.Business
{
    /// <summary>
    ///	BaseManager
    /// 通用基类部分
    /// 
    /// 修改记录
    /// 
    ///		2018.01.09 版本：Troy.Cui进行扩展。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2018.01.09</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region 获取上一个下一个

        /// <summary>
        /// 获取上一个下一个编号
        /// </summary>
        /// <param name="currentId">当前编号</param>
        /// <param name="tableName">表名</param>
        /// <param name="orderTypeId">订单类型编号</param> 
        /// <param name="previousId">上一个编号</param>
        /// <param name="nextId">下一个编号</param>
        /// <returns></returns>
        public virtual bool GetPreviousAndNextId(int currentId, string tableName, string orderTypeId, out int previousId, out int nextId)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = CurrentTableName;
            }
            previousId = currentId;
            nextId = currentId;
            var result = false;
            var sb = Pool.StringBuilder.Get();
            sb.Append("WITH T1 AS( ");
            sb.Append("SELECT TOP 1 Id AS PreviousId, " + currentId + " AS CurrentId FROM " + tableName + " WHERE 1 = 1 ");
            if (!string.IsNullOrEmpty(orderTypeId))
            {
                sb.Append(" AND OrderTypeId = " + orderTypeId + "");
            }
            
            sb.Append(" AND Id < " + currentId + " ORDER BY Id DESC ");

            sb.Append(") ");
            sb.Append(",T2 AS ( ");
            sb.Append("SELECT TOP 1 Id AS NextId, " + currentId + " AS CurrentId FROM " + tableName + " WHERE 1 = 1 ");
            if (!string.IsNullOrEmpty(orderTypeId))
            {
                sb.Append(" AND OrderTypeId = " + orderTypeId + "");
            }
            sb.Append(" AND Id > " + currentId + " ORDER BY Id ASC ");
            sb.Append(") ");
            sb.Append("SELECT ISNULL(T1.PreviousId," + currentId + ") AS PreviousId,ISNULL(T1.CurrentId,T2.CurrentId) AS CurrentId,ISNULL(T2.NextId," + currentId + ") AS NextId FROM T1 FULL JOIN T2 ON T1.CurrentId = T2.CurrentId ");
            var dt = DbHelper.Fill(sb.Put());
            if (dt != null && dt.Rows.Count == 0)
            {
                previousId = currentId;
                nextId = currentId;
                result = false;
            }
            else if (dt != null && (int.Parse(dt.Rows[0]["PreviousId"].ToString()) != currentId || int.Parse(dt.Rows[0]["NextId"].ToString()) != currentId))
            {
                previousId = int.Parse(dt.Rows[0]["PreviousId"].ToString());
                nextId = int.Parse(dt.Rows[0]["NextId"].ToString());
                result = true;
            }
            return result;
        }
        #endregion
    }
}