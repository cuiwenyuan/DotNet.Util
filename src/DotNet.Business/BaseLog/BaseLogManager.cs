//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;

namespace DotNet.Business
{
    using Util;
    using Model;

    /// <summary>
    /// BaseLogManager
    /// 日志的基类（程序OK）
    /// 
    /// 想在这里实现访问记录、继承以前的比较好的思路。
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
    ///		<name>JiRiGaLa</name>
    ///		<date>2016.02.14</date>
    /// </author> 
    /// </summary>
    public partial class BaseLogManager : BaseManager
    {
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

                UserId = int.Parse(serviceInfo.UserInfo.Id),
                CompanyId = int.Parse(serviceInfo.UserInfo.CompanyId),
                UserName = serviceInfo.UserInfo.RealName,
                WebUrl = serviceInfo.CurrentMethod.Module.Name.Replace(".dll", "") + "." + serviceInfo.CurrentMethod.Name
            };

            // 远程添加模式
            //LogUtil.AddLog(serviceInfo.UserInfo, entity);

            // 直接写入本地数据库的方法
            var logManager = new BaseLogManager(serviceInfo.UserInfo);
            logManager.Add(entity);
        }

        #region public void Add(BaseLogEntity entity)
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="entity">日志对象</param>
        /// <returns>主键</returns>
        public void Add(BaseLogEntity entity)
        {
            //本地添加模式
            AddObject(entity);
        }
        #endregion

        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="urlReferrer"></param>
        /// <param name="adId"></param>
        /// <param name="webUrl"></param>
        public void AddWebLog(string urlReferrer, string adId, string webUrl)
        {
            /*
            string userId = string.Empty;
            if (!UserInfo.Id.Equals(UserInfo.IPAddress))
            {
                userId = UserInfo.Id;
            }
            string userName = string.Empty;
            if (!UserInfo.UserName.Equals(UserInfo.IPAddress))
            {
                userName = UserInfo.UserName;
            }
            this.AddWebLog(urlReferrer, adId, webUrl, UserInfo.IPAddress, userId, userName);
            */
        }

        /// <summary>
        /// 写入网页访问日志
        /// </summary>
        /// <param name="urlReferrer">导入网址</param>
        /// <param name="adId">广告商ID</param>
        /// <param name="webUrl">访问的网址</param>
        /// <param name="ipAddress">网络地址</param>
        /// <param name="userId">用户主键</param>
        /// <param name="userName">用户名</param>
        public void AddWebLog(string urlReferrer, string adId, string webUrl, string ipAddress, string userId, string userName)
        {
            /*
            BaseLogEntity entity = new BaseLogEntity();
            entity.ProcessId = "WebLog";
            entity.UrlReferrer = urlReferrer;
            if (!string.IsNullOrEmpty(adId))
            {
                entity.MethodName = "AD";
                entity.Parameters = adId;
            }
            entity.WebUrl = webUrl;
            entity.IPAddress = ipAddress;
            entity.UserId = userId;
            entity.UserRealName = userName;
            this.AddObject(entity);
            */
        }
        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
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
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="processId"></param>
        /// <returns></returns>
        private string GetDataTableSql(string[] userIds, string name, string value, string beginDate, string endDate, string processId = null)
        {
            var sql = "SELECT * FROM " + BaseLogEntity.TableName + " WHERE 1=1 ";
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
                sql += string.Format(" AND {0} IN ({1}) ", BaseLogEntity.FieldUserId, string.Join(",", userIds));
            }
            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.Access:
                    // Access 中的时间分隔符 是 “#”
                    if (beginDate.Trim().Length > 0)
                    {
                        sql += string.Format(" AND CreateOn >= #{0}#", beginDate);
                    }
                    if (endDate.Trim().Length > 0)
                    {
                        sql += string.Format(" AND CreateOn <= #{0}#", endDate);
                    }
                    break;
                case CurrentDbType.SqlServer:
                    if (beginDate.Trim().Length > 0)
                    {
                        sql += string.Format(" AND CreateOn >= '{0}'", beginDate);
                    }
                    if (endDate.Trim().Length > 0)
                    {
                        sql += string.Format(" AND CreateOn <= '{0}'", endDate);
                    }
                    break;
                case CurrentDbType.Oracle:
                    if (beginDate.Trim().Length > 0)
                    {
                        sql += string.Format(" AND CreateOn >= TO_DATE( '{0}','yyyy-mm-dd hh24-mi-ss') ", beginDate);
                    }
                    if (endDate.Trim().Length > 0)
                    {
                        sql += string.Format(" AND CreateOn <= TO_DATE('{0}','yyyy-mm-dd hh24-mi-ss')", endDate);
                    }
                    break;
            }
            sql += " ORDER BY CreateOn DESC ";
            return sql;
        }

        #region public DataTable GetDataTableByDate(string name, string value, string beginDate, string endDate, string processId=null) 按日期查询
        /// <summary>
        /// 按日期查询
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
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
            var sql = "SELECT * FROM " + BaseLogEntity.TableName
                    + " WHERE CONVERT(NVARCHAR, " + BaseLogEntity.FieldStartTime + ", 111) = " + dbHelper.GetParameter(BaseLogEntity.FieldStartTime)
                    + " AND " + BaseLogEntity.FieldUserId + " = " + dbHelper.GetParameter(BaseLogEntity.FieldUserId);
            sql += " ORDER BY " + BaseLogEntity.FieldStartTime;
            var names = new string[2];
            names[0] = BaseLogEntity.FieldStartTime;
            names[1] = BaseLogEntity.FieldUserId;
            var values = new Object[2];
            values[0] = createOn;
            values[1] = createUserId;
            var dt = new DataTable(BaseLogEntity.TableName);
            dbHelper.Fill(dt, sql, DbHelper.MakeParameters(names, values));
            return dt;
        }
        #endregion

        //public DataTable Search(string[] userIds, string search, bool? enabled, bool OnlyOnLine)
        //{
        //    search = StringUtil.GetSearchString(search);
        //    string sql = "SELECT " + BaseLogEntity.TableName + ".*," + BaseUserEntity.TableName+ ".* " 
        //        + " FROM  " + BaseLogEntity.TableName + " LEFT OUTER JOIN "+
        //        BaseUserEntity.TableName + " ON RTRIM(LTRIM(" + BaseUserEntity.FieldId +"))="+
        //        BaseLogEntity.FieldUserId +
        //        " WHERE 1=1 ";
        //    //string sql = "SELECT * FROM " + BaseLogEntity.TableName + " WHERE 1=1 ";

        //    // 注意安全问题
        //    if (userIds.Length >0) 
        //    {
        //        sql += " AND " + BaseLogEntity.FieldUserId + " IN (" + ObjectUtil.ToList(userIds) + ") ";
        //    }


        //     sql += " AND ("  + BaseLogEntity.TableName +"." + BaseLogEntity.FieldWebUrl + " LIKE '%" + search + "%'"
        //                + " OR " + BaseLogEntity.TableName + "." + BaseLogEntity.FieldUserId + " LIKE '%" + search + "%'"
        //                + " OR " + BaseLogEntity.TableName + "." + BaseLogEntity.FieldUserRealName + " LIKE '%" + search + "%'"
        //                + " OR " + BaseLogEntity.TableName + "." + BaseLogEntity.FieldUrlReferrer + " LIKE '%" + search + "%'"
        //                + " OR " + BaseLogEntity.TableName + "." + BaseLogEntity.FieldIPAddress + " LIKE '%" + search + "%'"
        //                + " OR " + BaseLogEntity.TableName + "." + BaseLogEntity.FieldDescription + " LIKE '%" + search + "%')";

        //    //sql += " ORDER BY " + BaseLogEntity.TableName + "." + BaseUserEntity.FieldSortCode;
        //    return DbHelper.Fill(sql);
        //}

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="search"></param>
        /// <param name="enabled"></param>
        /// <param name="onlyOnLine"></param>
        /// <returns></returns>
        public DataTable Search(string[] userIds, string search, bool? enabled, bool onlyOnLine)
        {
            //TODO 吉日嘎拉，这里需要从2个表读取，2013-04-21
            search = StringUtil.GetSearchString(search);
            var sql = "SELECT " + BaseUserEntity.TableName + ".* "
                            + " FROM " + BaseUserEntity.TableName
                            + " WHERE " + BaseUserEntity.FieldDeleted + "= 0 "
                            + " AND " + BaseUserEntity.FieldIsVisible + "= 1 ";

            sql += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldUserName + " LIKE '%" + search + "%'"
                        + " OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldRealName + " LIKE '%" + search + "%'"
                        + " OR " + BaseUserLogOnEntity.TableName + "." + BaseUserLogOnEntity.FieldIpAddress + " LIKE '%" + search + "%'"
                        + " OR " + BaseUserLogOnEntity.TableName + "." + BaseUserLogOnEntity.FieldMacAddress + " LIKE '%" + search + "%'"
                        + ")";

            sql += " ORDER BY " + BaseUserEntity.FieldSortCode;

            return DbHelper.Fill(sql);
        }

        #region 高级查询
        /// <summary>
        /// 按条件分页查询(带记录状态Enabled和删除状态DeletionStateCode)
        /// </summary>
        /// <param name="companyId">查看公司主键</param>
        /// <param name="departmentId">查看部门主键</param>
        /// <param name="userId">查看用户主键</param>
        /// <param name="startTime">创建开始时间</param>
        /// <param name="endTime">创建结束时间</param>
        /// <param name="searchKey">查询字段</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="showDisabled">是否显示无效记录</param>
        /// <param name="showDeleted">是否显示已删除记录</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageIndex = 0, int pageSize = 20, string sortExpression = "CreateOn", string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            pageIndex++;
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseLogEntity.FieldEnabled + " = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                //sb.Append(" AND " + BaseLogEntity.FieldDeleted + " = 0";
            }

            if (ValidateUtil.IsInt(companyId))
            {
                //sb.Append(" AND " + BaseLogEntity.FieldCompanyId + " = " + companyId;
            }
            // 只有管理员才能看到所有的
            //if (!(UserInfo.IsAdministrator && BaseSystemInfo.EnableAdministrator))
            //{
                //sb.Append(" AND (" + BaseLogEntity.FieldUserCompanyId + " = 0 OR " + BaseLogEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")";
            //}
            if (ValidateUtil.IsInt(departmentId))
            {
                //sb.Append(" AND " + BaseLogEntity.FieldDepartmentId + " = " + departmentId;
            }
            if (ValidateUtil.IsInt(userId))
            {
                //sb.Append(" AND " + BaseLogEntity.FieldUserId + " = " + userId;
            }
            //创建时间
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseLogEntity.FieldCreateTime + " >= '" + startTime + "'");
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseLogEntity.FieldCreateTime + " <= DATEADD(s,-1,DATEADD(d,1,'" + endTime + "'))");
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (UserName LIKE N'%" + searchKey + "%' OR RealName LIKE N'%" + searchKey + "%' OR Service LIKE N'%" + searchKey + "%' OR Parameters LIKE N'%" + searchKey + "%' OR WebUrl LIKE N'%" + searchKey + "%' OR Description LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageIndex, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Put(), null, "*");
        }
        #endregion
    }
}