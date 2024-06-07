//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///		2020.10.04 版本：1.0 Troy.Cui	新增用户登录的报表统计。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2020.10.04</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        #region 每日注册统计
        /// <summary>
        /// 每日注册统计GetDailyRegister
        /// </summary>
        /// <param name="days">最近多少天</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns></returns>
        public DataTable DailyUserReport(int days, string startDate, string endDate)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT CONVERT(NVARCHAR(4),B.FiscalYear) + '-' + CONVERT(NVARCHAR(2),B.FiscalMonth) + '-' + CONVERT(NVARCHAR(2),B.FiscalDay) AS TransactionDate");
            sb.Append(" ,(SELECT COUNT(*) FROM " + CurrentTableName + " A WHERE DATEDIFF(d,A.CreateTime,B.TransactionDate) = 0 AND A.Enabled = 1 AND A." + BaseUserEntity.FieldDeleted + " = 0) AS TotalNewUserCount");
            sb.Append(" ,(SELECT COUNT(DISTINCT UserId) FROM " + BaseLogonLogEntity.CurrentTableName + " A WHERE DATEDIFF(d,A.CreateTime,B.TransactionDate) = 0) AS TotalUserLoginCount");
            sb.Append(" ,(SELECT COUNT(*) FROM " + BaseOrganizationEntity.CurrentTableName + " A WHERE DATEDIFF(d,A.CreateTime,B.TransactionDate) = 0 AND A.Enabled = 1 AND A." + BaseOrganizationEntity.FieldDeleted + " = 0 AND ParentId = 0) AS TotalCompanyCount");
            sb.Append(" FROM " + BaseCalendarEntity.CurrentTableName + " B ");
            sb.Append(" WHERE B.TransactionDate <= GETDATE() AND DATEDIFF(d,B.TransactionDate,GETDATE()) < " + days + "");
            if (ValidateUtil.IsDateTime(startDate))
            {
                sb.Append(" AND B.TransactionDate >= '" + startDate + "'");
            }
            if (ValidateUtil.IsDateTime(endDate))
            {
                sb.Append(" AND B.TransactionDate <= '" + endDate + "'");
            }
            sb.Append(" ORDER BY B.TransactionDate ASC");

            return Fill(sb.Return());
        }
        #endregion
    }
}