//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseModifyRecordManager
    /// 修改日志管理
    /// 
    /// 修改记录
    /// 
    ///		2016.09.23 版本：1.0 Troy.Cui	新增。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.09.23</date>
    /// </author> 
    /// </summary>
    public partial class BaseModifyRecordManager : BaseManager
    {
        #region 高级查询
        /// <summary>
        /// 按条件分页高级查询
        /// </summary>
        /// <param name="tableCode"></param>
        /// <param name="tableDescription"></param>
        /// <param name="columnCode"></param>
        /// <param name="columnDescription"></param>
        /// <param name="searchKey">搜索关键词</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序规则</param>
        /// <returns></returns>
        public DataTable GetDataTableByPage(string tableCode, string tableDescription, string columnCode, string columnDescription, string searchKey, out int recordCount, int pageIndex = 0, int pageSize = 20, string sortExpression = "CreateOn", string sortDirection = "DESC")
        {
            pageIndex++;
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            
            //表名
            if (!string.IsNullOrEmpty(tableCode))
            {
                sb.Append(" AND " + BaseModifyRecordEntity.TableName + "." + BaseModifyRecordEntity.FieldTableCode + " = N'" + tableCode + "'");
            }
            //表描述
            if (!string.IsNullOrEmpty(tableDescription))
            {
                sb.Append(" AND " + BaseModifyRecordEntity.TableName + "." + BaseModifyRecordEntity.FieldTableDescription + " = N'" + tableDescription + "'");
            }
            //字段名
            if (!string.IsNullOrEmpty(columnCode))
            {
                sb.Append(" AND " + BaseModifyRecordEntity.TableName + "." + BaseModifyRecordEntity.FieldColumnCode + " = N'" + columnCode + "'");
            }
            //字段描述
            if (!string.IsNullOrEmpty(columnDescription))
            {
                sb.Append(" AND " + BaseModifyRecordEntity.TableName + "." + BaseModifyRecordEntity.FieldColumnDescription + " = N'" + columnDescription + "'");
            }
            //关键词
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseModifyRecordEntity.FieldTableCode + " LIKE N'%" + searchKey + "%' OR " + BaseModifyRecordEntity.FieldTableDescription + " LIKE N'%" + searchKey + "%' OR " + BaseModifyRecordEntity.FieldColumnCode + " LIKE N'%" + searchKey + "%' OR " + BaseModifyRecordEntity.FieldColumnDescription + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageIndex, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Put(), null, "*");
        }
        #endregion
    }
}