//-----------------------------------------------------------------------
// <copyright file="BaseMessageFailedManager.cs" company="DotNet">
//     Copyright (c) 2024, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseMessageFailedManager
    /// 失败消息管理层
    /// 
    /// 修改记录
    /// 
    ///	2022-12-16 版本：1.0 Troy.Cui 创建文件。
    ///		
    /// <author>
    ///	<name>Troy.Cui</name>
    ///	<date>2022-12-16</date>
    /// </author> 
    /// </summary>
    public partial class BaseMessageFailedManager : BaseManager
    {
        #region 高级查询

        /// <summary>
        /// 按条件分页查询(带记录状态Enabled和删除状态Deleted)
        /// </summary>
        /// <param name="companyId">查看公司主键</param>
        /// <param name="departmentId">查看部门主键</param>
        /// <param name="userId">查看用户主键</param>
        /// <param name="startTime">创建开始时间</param>
        /// <param name="endTime">创建结束时间</param>
        /// <param name="messageType">消息类型</param>
        /// <param name="recipient">收件人</param>
        /// <param name="searchKey">查询关键字</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="showDisabled">是否显示无效记录</param>
        /// <param name="showDeleted">是否显示已删除记录</param>
        /// <param name="source">来源</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = "CreateTime", string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true, string source = null, string messageType = null, string recipient = null)
        {
            var sb = PoolUtil.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND Enabled = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND Deleted = 0");
            }

            if (ValidateUtil.IsInt(companyId))
            {
                //sb.Append(" AND CompanyId = " + companyId);
            }
            if (UserInfo != null && !UserInfo.IsAdministrator)
            {
                sb.Append(" AND (UserCompanyId = 0 OR UserCompanyId = " + UserInfo.CompanyId + ")");
            }
            if (ValidateUtil.IsInt(departmentId))
            {
                //sb.Append(" AND DepartmentId = " + departmentId);
            }
            if (ValidateUtil.IsInt(userId))
            {
                //sb.Append(" AND UserId = " + userId);
            }
            //创建日期
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND CreateTime >= '" + startTime + "'");
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND CreateTime <= DATEADD(s,-1,DATEADD(d,1,'" + endTime + "'))");
            }
            if (!string.IsNullOrEmpty(source))
            {
                source = dbHelper.SqlSafe(source);
                sb.Append(" AND " + BaseMessageSucceedEntity.FieldSource + " = N'" + source + "'");
            }
            if (!string.IsNullOrEmpty(messageType))
            {
                messageType = dbHelper.SqlSafe(messageType);
                sb.Append(" AND " + BaseMessageSucceedEntity.FieldMessageType + " = N'" + messageType + "'");
            }
            if (!string.IsNullOrEmpty(recipient))
            {
                recipient = dbHelper.SqlSafe(recipient);
                sb.Append(" AND " + BaseMessageSucceedEntity.FieldRecipient + " = N'" + recipient + "'");
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseMessageSucceedEntity.FieldSubject + " LIKE N'%" + searchKey + "%' OR " + BaseMessageSucceedEntity.FieldBody + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", " ");
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Return());
        }
        #endregion
        
        #region GetTotalCount
        /// <summary>
        /// 获取总记录数
        /// </summary>
        /// <param name="days">天数</param>
        /// <returns></returns>
        public string GetTotalCount(int days)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) AS TotalCount FROM " + CurrentTableName + " WHERE (DATEADD(d, " + days + ", " + BaseMessageFailedEntity.FieldCreateTime + ") >= " + DbHelper.GetDbNow() + ")");
            return ExecuteScalar(sb.Return())?.ToString();
        }
        #endregion
    }
}
