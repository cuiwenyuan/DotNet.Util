//-----------------------------------------------------------------------
// <copyright file="MessageSucceedManager.cs" company="DotNet">
//     Copyright (c) 2019, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseMessageSucceedManager
    /// 成功消息管理层
    /// 
    /// 修改纪录
    /// 
    ///	2016-12-18 版本：1.0 Troy.Cui 创建文件。
    ///		
    /// <author>
    ///	<name>Troy.Cui</name>
    ///	<date>2016-12-18</date>
    /// </author> 
    /// </summary>
    public partial class BaseMessageSucceedManager : BaseManager, IBaseManager
    {
        #region 高级查询

        /// <summary>
        /// 按条件分页查询(带记录状态Enabled和删除状态DeletionStateCode)
        /// </summary>
        /// <param name="companyId">查看公司主键</param>
        /// <param name="departmentId">查看部门主键</param>
        /// <param name="userId">查看用户主键</param>
        /// <param name="startTime">创建开始时间</param>
        /// <param name="endTime">创建结束时间</param>
        /// <param name="messageType">消息类型</param>
        /// <param name="recipient">收件人</param>
        /// <param name="searchKey">查询字段</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="showDisabled">是否显示无效记录</param>
        /// <param name="showDeleted">是否显示已删除记录</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string messageType, string recipient, string searchKey, out int recordCount, int pageIndex = 0, int pageSize = 20, string sortExpression = "CreateOn", string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            pageIndex++;
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND Enabled = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND DeletionStateCode = 0");
            }

            if (ValidateUtil.IsInt(companyId))
            {
                //sb.Append(" AND CompanyId = " + companyId);
            }

            if (UserInfo != null && !string.IsNullOrEmpty(UserInfo.CompanyId))
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
                sb.Append(" AND CreateOn >= '" + startTime + "'");
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND CreateOn <= DATEADD(s,-1,DATEADD(d,1,'" + endTime + "'))");
            }
            if (!string.IsNullOrEmpty(messageType))
            {
                messageType = dbHelper.SqlSafe(messageType);
                sb.Append(" AND MessageType = N'" + messageType + "'");
            }
            if (!string.IsNullOrEmpty(recipient))
            {
                recipient = dbHelper.SqlSafe(recipient);
                sb.Append(" AND Recipient = N'" + recipient + "'");
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (Recipient LIKE N'%" + searchKey + "%' OR Subject LIKE N'%" + searchKey + "%' OR Body LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageIndex, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Put(), null, "*");
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
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) AS TotalCount FROM " + CurrentTableName + " WHERE (DATEADD(d, " + days + ", " + BaseMessageSucceedEntity.FieldCreateTime + ") >= " + DbHelper.GetDbNow() + ")");
            return ExecuteScalar(sb.Put())?.ToString();
        }

        #endregion

    }
}
