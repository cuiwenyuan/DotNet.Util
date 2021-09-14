//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseItemsManager
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
    public partial class BaseItemsManager : BaseManager
    {
        #region 高级查询
        /// <summary>
        /// 按条件分页高级查询
        /// </summary>
        /// <param name="systemCode">子系统</param>
        /// <param name="searchKey">搜索关键词</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序规则</param>
        /// <param name="showDisabled"></param>
        /// <param name="showDeleted"></param>
        /// <returns></returns>
        public DataTable GetDataTableByPage(string systemCode, string searchKey, out int recordCount, int pageIndex = 0, int pageSize = 20, string sortExpression = "CreateOn", string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            pageIndex++;
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            //表名
            var tableNameItems = BaseItemsEntity.TableName;
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableNameItems = systemCode + "Items";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(UserInfo.SystemCode))
                {
                    tableNameItems = UserInfo.SystemCode + "Items";
                }
            }
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseItemsEntity.FieldEnabled + "  = 1 ");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseItemsEntity.FieldDeleted + "  = 0 ");
            }
            //关键词
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseItemsEntity.FieldCode + " LIKE N'%" + searchKey + "%' OR " + BaseItemsEntity.FieldFullName + " LIKE N'%" + searchKey + "%' OR " + BaseItemsEntity.FieldTargetTable + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageIndex, pageSize, sortExpression, sortDirection, tableNameItems, sb.Put(), null, "*");
        }
        #endregion
    }
}