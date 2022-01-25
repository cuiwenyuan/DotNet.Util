//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseLogonLogManager
    /// 登录管理
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
    public partial class BaseLogonLogManager
    {
        #region 每日登录统计
        /// <summary>
        /// 每日登录统计GetDailyRegister
        /// </summary>
        /// <param name="days">最近多少天</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns></returns>
        public DataTable GetDailyLogin(int days, string startDate, string endDate)
        {
            var sql = string.Empty;
            sql += "SELECT CONVERT(NVARCHAR(4),B." + BaseCalendarEntity.FieldFiscalYear + ") + '-' + CONVERT(NVARCHAR(2),B." + BaseCalendarEntity.FieldFiscalMonth + ") + '-' + CONVERT(NVARCHAR(2),B." + BaseCalendarEntity.FieldFiscalDay + ") AS TransactionDate";
            sql += " ,(SELECT COUNT(*) FROM " + CurrentTableName + " A WHERE A." + BaseLogonLogEntity.FieldCreateTime + " = B." + BaseCalendarEntity.FieldTransactionDate + " AND A." + BaseLogonLogEntity.FieldEnabled + " = 1 AND A." + BaseLogonLogEntity.FieldDeleted + " = 0) AS TotalCount";
            sql += " ,(SELECT COUNT(*) FROM " + CurrentTableName + " A WHERE A." + BaseLogonLogEntity.FieldCreateTime + " = B." + BaseCalendarEntity.FieldTransactionDate + " AND A." + BaseLogonLogEntity.FieldEnabled + " = 1 AND A." + BaseLogonLogEntity.FieldDeleted + " = 0 AND A." + BaseLogonLogEntity.FieldResult + " = 1) AS SuccessCount";
            sql += " ,(SELECT COUNT(*) FROM " + CurrentTableName + " A WHERE A." + BaseLogonLogEntity.FieldCreateTime + " = B." + BaseCalendarEntity.FieldTransactionDate + " AND A." + BaseLogonLogEntity.FieldEnabled + " = 1 AND A." + BaseLogonLogEntity.FieldDeleted + " = 0 AND A." + BaseLogonLogEntity.FieldResult + " = 0) AS FailCount";
            sql += " FROM " + BaseCalendarEntity.CurrentTableName + " B WHERE 1 = 1";
            sql += " AND B." + BaseCalendarEntity.FieldTransactionDate + " <= GETDATE() AND DATEDIFF(d,B." + BaseCalendarEntity.FieldTransactionDate + ",GETDATE()) < " + days + "";
            if (ValidateUtil.IsDateTime(startDate))
            {
                sql += " AND B." + BaseCalendarEntity.FieldTransactionDate + " >= '" + startDate + "'";
            }
            if (ValidateUtil.IsDateTime(endDate))
            {
                sql += " AND B." + BaseCalendarEntity.FieldTransactionDate + " <= '" + endDate + "'";
            }
            sql += " ORDER BY B." + BaseCalendarEntity.FieldTransactionDate + " ASC";

            return Fill(sql);
        }
        #endregion
    }
}