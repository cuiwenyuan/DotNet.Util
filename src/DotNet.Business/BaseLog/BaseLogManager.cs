//-----------------------------------------------------------------------
// <copyright file="BaseLogManager.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Data;

namespace DotNet.Business
{
    using Util;
    using Model;
    using System.Collections.Generic;

    /// <summary>
    /// BaseLogManager
    /// 系统日志管理层
    /// 
    /// 修改记录
    /// 
    ///		2016.02.14 版本：3.0 JiRiGaLa   代码进行分离、方法进行优化。
    ///     2011.03.24 版本：2.6 JiRiGaLa   讲程序转移到DotNet.BaseManager命名空间中。
    ///     2008.10.21 版本：2.5 JiRiGaLa   日志功能改进。
    ///     2008.04.22 版本：2.4 JiRiGaLa   在新的数据库连接里保存日志，不影响其它程序逻辑的事务处理。
    ///     2007.12.03 版本：2.3 JiRiGaLa   进行规范化整理。
    ///     2007.11.08 版本：2.2 JiRiGaLa   整理多余的主键（OK）。
    ///		2007.07.09 版本：2.1 JiRiGaLa   程序整理，修改 Static 方法，采用 Instance 方法。
    ///		2006.12.02 版本：2.0 JiRiGaLa   程序整理，错误方法修改。
    ///		2004.07.28 版本：1.0 JiRiGaLa   进行了排版、方法规范化、接口继承、索引器。
    ///		2004.11.12 版本：1.0 JiRiGaLa   删除了一些方法。
    ///		2005.09.30 版本：1.0 JiRiGaLa   又进行一次飞跃，把一些思想进行了统一。
    /// 
    /// <author>
    ///	<name>Troy.Cui</name>
    ///	<date>2021-10-04</date>
    /// </author> 
    /// </summary>
    public partial class BaseLogManager : BaseManager
    {
        #region public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseLogEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = false, bool showDeleted = false)
        /// <summary>
        /// 按条件分页查询(带记录状态Enabled和删除状态Deleted)
        /// </summary>
        /// <param name="companyId">查看公司主键</param>
        /// <param name="departmentId">查看部门主键</param>
        /// <param name="userId">查看用户主键</param>
        /// <param name="startTime">创建开始时间</param>
        /// <param name="endTime">创建结束时间</param>
        /// <param name="searchKey">查询关键字</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="showDisabled">是否显示无效记录</param>
        /// <param name="showDeleted">是否显示已删除记录</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseLogEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseLogEntity.FieldEnabled + " = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseLogEntity.FieldDeleted + " = 0");
            }

            if (ValidateUtil.IsInt(companyId))
            {
                //sb.Append(" AND " + BaseLogEntity.FieldCompanyId + " = " + companyId);
            }
            // 只有管理员才能看到所有的
            //if (!(UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled))
            //{
                //sb.Append(" AND (" + BaseLogEntity.FieldUserCompanyId + " = 0 OR " + BaseLogEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            //}
            if (ValidateUtil.IsInt(departmentId))
            {
                //sb.Append(" AND " + BaseLogEntity.FieldDepartmentId + " = " + departmentId);
            }
            if (ValidateUtil.IsInt(userId))
            {
                //sb.Append(" AND " + BaseLogEntity.FieldUserId + " = " + userId);
            }
            //创建时间
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseLogEntity.FieldCreateTime + " >= " + dbHelper.ToDbTime(startTime));
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseLogEntity.FieldCreateTime + " <= " + dbHelper.ToDbTime(endTime.ToDateTime().Date.AddDays(1).AddMilliseconds(-1)));
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseLogEntity.FieldUserName + " LIKE N'%" + searchKey + "%' OR " + BaseLogEntity.FieldDescription + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Put(), null, "*");
        }
        #endregion

        #region 下拉菜单

        /// <summary>
        /// 下拉菜单
        /// </summary>
        /// <param name="myCompanyOnly">仅本公司</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(bool myCompanyOnly = true)
        {
            var sb = Pool.StringBuilder.Get();
            if (myCompanyOnly)
            {
                //sb.Append("(" + BaseLogEntity.FieldUserCompanyId + " = 0 OR " + BaseLogEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            }
            //return GetDataTable(sb.Put(), null, new KeyValuePair<string, object>(BaseLogEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BaseLogEntity.FieldDeleted, 0));
            var companyId = string.IsNullOrEmpty(BaseSystemInfo.CustomerCompanyId) ? UserInfo.CompanyId : BaseSystemInfo.CustomerCompanyId;
            var cacheKey = "DataTable." + CurrentTableName + "." + companyId + "." + (myCompanyOnly ? "1" : "0");
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            return CacheUtil.Cache<DataTable>(cacheKey, () => GetDataTable(sb.Put(), null, new KeyValuePair<string, object>(BaseLogEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BaseLogEntity.FieldDeleted, 0)), true, false, cacheTime);
        }

        #endregion
	
        /// <summary>
        /// 2016-02-14 吉日嘎拉 增加服务器调用耗时统计功能。
        /// </summary>
        /// <param name="serviceInfo">服务调用情况</param>
        public static void AddLog(ServiceInfo serviceInfo)
        {
            if (!BaseSystemInfo.RecordLog)
            {
                return;
            }

            var entity = new BaseLogEntity
            {
                StartTime = serviceInfo.StartTime,
                TaskId = serviceInfo.TaskId,
                ClientIp = serviceInfo.UserInfo.IpAddress,
                ElapsedTicks = serviceInfo.ElapsedTicks,

                UserId = serviceInfo.UserInfo.UserId,
                CompanyId = serviceInfo.UserInfo.CompanyId.ToInt(),
                UserName = serviceInfo.UserInfo.RealName,
                WebUrl = serviceInfo.CurrentMethod.Module.Name.Replace(".dll", "") + "." + serviceInfo.CurrentMethod.Name
            };

            // 远程添加模式
            //LogHttpUtil.AddLog(serviceInfo.UserInfo, entity);

            // 直接写入本地数据库的方法
            var logManager = new BaseLogManager(serviceInfo.UserInfo);
            logManager.Add(entity);
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="processId"></param>
        /// <returns></returns>
        public DataTable GetDataTableByDateByUserIds(string[] userIds, string name, string value, string beginDate, string endDate, string processId = null)
        {
            var sql = GetDataTableSql(userIds, name, value, beginDate, endDate, processId);
            return DbHelper.Fill(sql);
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="processId"></param>
        /// <returns></returns>
        private string GetDataTableSql(string[] userIds, string name, string value, string beginDate, string endDate, string processId = null)
        {
            var sql = "SELECT * FROM " + BaseLogEntity.CurrentTableName + " WHERE 1=1 ";
            if (!string.IsNullOrEmpty(value))
            {
                sql += string.Format(" AND {0} = '{1}' ", name, value);
            }
            if (!string.IsNullOrEmpty(processId))
            {
                // sql += string.Format(" AND {0} = '{1}' ", BaseLogEntity.FieldProcessId, processId);
            }
            if (!string.IsNullOrEmpty(beginDate) && !string.IsNullOrEmpty(endDate))
            {
                beginDate = DateTime.Parse(beginDate).ToShortDateString();
                endDate = DateTime.Parse(endDate).AddDays(1).ToShortDateString();
            }
            // 注意安全问题
            if (userIds != null)
            {
                sql += string.Format(" AND {0} IN ({1}) ", BaseLogEntity.FieldUserId, StringUtil.ArrayToList(userIds));
            }
            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.Access:
                    // Access 中的时间分隔符 是 “#”
                    if (beginDate.Trim().Length > 0)
                    {
                        sql += string.Format(" AND CreateTime >= #{0}#", beginDate);
                    }
                    if (endDate.Trim().Length > 0)
                    {
                        sql += string.Format(" AND CreateTime <= #{0}#", endDate);
                    }
                    break;
                case CurrentDbType.SqlServer:
                    if (beginDate.Trim().Length > 0)
                    {
                        sql += string.Format(" AND CreateTime >= '{0}'", beginDate);
                    }
                    if (endDate.Trim().Length > 0)
                    {
                        sql += string.Format(" AND CreateTime <= '{0}'", endDate);
                    }
                    break;
                case CurrentDbType.Oracle:
                    if (beginDate.Trim().Length > 0)
                    {
                        sql += string.Format(" AND CreateTime >= TO_DATE( '{0}','yyyy-mm-dd hh24-mi-ss') ", beginDate);
                    }
                    if (endDate.Trim().Length > 0)
                    {
                        sql += string.Format(" AND CreateTime <= TO_DATE('{0}','yyyy-mm-dd hh24-mi-ss')", endDate);
                    }
                    break;
            }
            sql += " ORDER BY CreateTime DESC ";
            return sql;
        }

        #region public DataTable GetDataTableByDate(string name, string value, string beginDate, string endDate, string processId=null) 按日期查询
        /// <summary>
        /// 按日期查询
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="processId">日志类型</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByDate(string name, string value, string beginDate, string endDate, string processId = null)
        {
            var sql = GetDataTableSql(null, name, value, beginDate, endDate, processId);
            return DbHelper.Fill(sql);
        }
        #endregion

        #region public DataTable GetDataTableByDate(string createOn, string processId, string createUserId)
        /// <summary>
        /// 按日期查询
        /// </summary>
        /// <param name="createOn">记录日期 yyyy/mm/dd</param>
        /// <param name="processName">模块主键</param>
        /// <param name="createUserId">用户主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByDate(string createOn, string processName, string createUserId)
        {
            var sql = "SELECT * FROM " + BaseLogEntity.CurrentTableName
                    + " WHERE CONVERT(NVARCHAR, " + BaseLogEntity.FieldStartTime + ", 111) = " + dbHelper.GetParameter(BaseLogEntity.FieldStartTime)
                    + " AND " + BaseLogEntity.FieldUserId + " = " + dbHelper.GetParameter(BaseLogEntity.FieldUserId);
            sql += " ORDER BY " + BaseLogEntity.FieldStartTime;
            var names = new string[2];
            names[0] = BaseLogEntity.FieldStartTime;
            names[1] = BaseLogEntity.FieldUserId;
            var values = new Object[2];
            values[0] = createOn;
            values[1] = createUserId;
            var dt = new DataTable(BaseLogEntity.CurrentTableName);
            dbHelper.Fill(dt, sql, DbHelper.MakeParameters(names, values));
            return dt;
        }
        #endregion

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="search"></param>
        /// <param name="enabled"></param>
        /// <param name="onlyOnline"></param>
        /// <returns></returns>
        public DataTable Search(string[] userIds, string search, bool? enabled, bool onlyOnline)
        {
            //TODO 吉日嘎拉，这里需要从2个表读取，2013-04-21
            search = StringUtil.GetSearchString(search);
            var sql = "SELECT " + BaseUserEntity.CurrentTableName + ".* "
                            + " FROM " + BaseUserEntity.CurrentTableName
                            + " WHERE " + BaseUserEntity.FieldDeleted + "= 0 "
                            + " AND " + BaseUserEntity.FieldIsVisible + "= 1 ";

            sql += " AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldUserName + " LIKE '%" + search + "%'"
                        + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldRealName + " LIKE '%" + search + "%'"
                        + " OR " + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldIpAddress + " LIKE '%" + search + "%'"
                        + " OR " + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldMacAddress + " LIKE '%" + search + "%'"
                        + ")";

            sql += " ORDER BY " + BaseUserEntity.FieldSortCode;

            return DbHelper.Fill(sql);
        }
    }
}