//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseItemDetailsManager
    /// 字典选项管理
    /// 
    /// 修改记录
    /// 
    ///		2016.10.28 版本：1.0 Troy.Cui	新增。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.10.28</date>
    /// </author> 
    /// </summary>
    public partial class BaseItemDetailsManager : BaseManager
    {
        #region 高级查询

        /// <summary>
        /// 按条件分页高级查询
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="searchKey">搜索关键词</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序规则</param>
        /// <param name="showDisabled">显示禁止</param>
        /// <param name="showDeleted">显示删除</param>
        /// <returns></returns>
        public DataTable GetDataTableByPage(string tableName, string searchKey, out int recordCount, int pageIndex = 0, int pageSize = 20, string sortExpression = "CreateOn", string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            pageIndex++;
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = "BaseItemDetails";
            }
            
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseItemDetailsEntity.FieldEnabled + "  = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseItemDetailsEntity.FieldDeleted + "  = 0");
            }
            //关键词
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseItemDetailsEntity.FieldItemCode + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseItemDetailsEntity.FieldItemName + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseItemDetailsEntity.FieldItemValue + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageIndex, pageSize, sortExpression, sortDirection, tableName, sb.Put(), null, "*");
        }
        #endregion
    }
}