//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseLogManager
    /// 日志管理
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
    public partial class BaseLogManager : BaseManager
    {
        #region 高级查询

        /// <summary>
        /// 按条件分页高级查询
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="processName"></param>
        /// <param name="methodId"></param>
        /// <param name="methodName"></param>
        /// <param name="userRealName"></param>
        /// <param name="searchKey">搜索关键词</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序规则</param>
        /// <returns></returns>
        public DataTable GetDataTableByPage(string processId, string processName, string methodId, string methodName, string userRealName, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = "CreateTime", string sortDirection = "DESC")
        {
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            
            ////子系统
            //if (!string.IsNullOrEmpty(processId))
            //{
            //    sb.Append(" AND " + BaseLogEntity.CurrentTableName + "." + BaseLogEntity.field + " = N'" + systemCode + "'");
            //}
            ////用户主键
            //if (!string.IsNullOrEmpty(userId))
            //{
            //    sb.Append(" AND " + BaseLogEntity.CurrentTableName + "." + BaseLogEntity.FieldUserId + " = N'" + userId + "'");
            //}
            ////用户名
            //if (!string.IsNullOrEmpty(userName))
            //{
            //    sb.Append(" AND " + BaseLogEntity.CurrentTableName + "." + BaseLogEntity.FieldUserName + " = N'" + userName + "'");
            //}

            //关键词
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseLogEntity.FieldRealName + " LIKE N'%" + searchKey + "%' OR " + BaseLogEntity.FieldService + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Put(), null, "*");
        }
        #endregion
    }
}