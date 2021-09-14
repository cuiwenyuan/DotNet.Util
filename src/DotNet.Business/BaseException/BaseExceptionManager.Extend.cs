﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseExceptionManager
    /// 异常管理
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
    public partial class BaseExceptionManager : BaseManager
    {
        #region public string AddObject(Exception ex, string url) 记录异常情况

        /// <summary>
        /// 记录异常情况
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="url">网址</param>
        /// <returns>主键</returns>
        public string AddObject(Exception ex, string url)
        {
            var entity = new BaseExceptionEntity
            {

                //出错源地址，暂时放Title中
                Title = url,
                //异常消息
                Message = ex.Message,
                //异常源
                Category = ex.Source,
                //异常类型
                ThreadName = ex.GetType().FullName,
                //异常方法
                ProcessName = ex.TargetSite?.Name,
                //异常堆栈
                FormattedMessage = ex.StackTrace
            };
            if (UserInfo != null)
            {
                entity.IpAddress = UserInfo.IpAddress;
                entity.CreateUserId = UserInfo.Id;
                entity.CreateBy = UserInfo.RealName;
            }
            else
            {
                entity.CreateBy = entity.CreateBy;
            }
            
            return AddObject(entity);
        }
        #endregion

        #region public static string LogException(IDbHelper dbHelper, BaseUserInfo userInfo, Exception ex) 记录异常情况

        /// <summary>
        /// 记录异常情况
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户</param>
        /// <param name="ex"></param>
        /// <param name="url"></param>
        /// <returns>主键</returns>
        public static string LogException(IDbHelper dbHelper, BaseUserInfo userInfo, Exception ex, string url = null)
        {
            // 在控制台需要输出错误信息
#if (DEBUG)
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(ex.InnerException);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(string.Empty);
#endif

            var result = string.Empty;
            // 系统里应该可以配置是否记录异常现象
            if (!BaseSystemInfo.LogException)
            {
                return result;
            }
            // Windows系统异常中
            if (BaseSystemInfo.EventLog)
            {
                if (!EventLog.SourceExists(BaseSystemInfo.SoftName))
                {
                    EventLog.CreateEventSource(BaseSystemInfo.SoftName, BaseSystemInfo.SoftFullName);
                }
                var eventLog = new EventLog
                {
                    Source = BaseSystemInfo.SoftName
                };
                eventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
            }

            //判断一下数据库是否打开状态，若数据库都没能打开，还记录啥错误，不是又抛出另一个错误了？
            var exceptionManager = new BaseExceptionManager(dbHelper, userInfo);
            result = exceptionManager.AddObject(ex, url);

            return result;
        }
        #endregion

        #region public DataTable Search(string searchKey) 查询
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="searchKey">查询</param>
        /// <returns>数据表</returns>
        public DataTable Search(string searchKey)
        {
            var sql = "SELECT * "
                    + " FROM " + BaseExceptionEntity.TableName
                    + " WHERE 1 = 1 ";

            var dbParameters = new List<IDbDataParameter>();
            if (!string.IsNullOrEmpty(searchKey))
            {
                sql += string.Format(" AND ({0} LIKE {1}", BaseExceptionEntity.FieldIpAddress, DbHelper.GetParameter(BaseExceptionEntity.FieldIpAddress));
                sql += string.Format(" OR {0} LIKE {1}", BaseExceptionEntity.FieldFormattedMessage, DbHelper.GetParameter(BaseExceptionEntity.FieldFormattedMessage));
                sql += string.Format(" OR {0} LIKE {1}", BaseExceptionEntity.FieldProcessName, DbHelper.GetParameter(BaseExceptionEntity.FieldProcessName));
                sql += string.Format(" OR {0} LIKE {1}", BaseExceptionEntity.FieldMachineName, DbHelper.GetParameter(BaseExceptionEntity.FieldMachineName));
                sql += string.Format(" OR {0} LIKE {1})", BaseExceptionEntity.FieldMessage, DbHelper.GetParameter(BaseExceptionEntity.FieldMessage));
                searchKey = searchKey.Trim();
                if (searchKey.IndexOf("%") < 0)
                {
                    searchKey = string.Format("%{0}%", searchKey);
                }
                dbParameters.Add(DbHelper.MakeParameter(BaseExceptionEntity.FieldIpAddress, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseExceptionEntity.FieldFormattedMessage, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseExceptionEntity.FieldProcessName, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseExceptionEntity.FieldMachineName, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseExceptionEntity.FieldMessage, searchKey));
            }
            var dt = new DataTable(BaseExceptionEntity.TableName);
            DbHelper.Fill(dt, sql, dbParameters.ToArray());
            return dt;
        }
        #endregion

        #region 高级查询
        /// <summary>
        /// 按条件分页高级查询
        /// </summary>
        /// <param name="startTime">创建开始时间</param>
        /// <param name="endTime">创建结束时间</param>
        /// <param name="searchKey">搜索关键词</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序规则</param>
        /// <returns></returns>
        public DataTable GetDataTableByPage(string startTime, string endTime, string searchKey, out int recordCount, int pageIndex = 0, int pageSize = 20, string sortExpression = "CreateOn", string sortDirection = "DESC")
        {
            pageIndex++;
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            
            ////子系统
            //if (!string.IsNullOrEmpty(processId))
            //{
            //    sb.Append(" AND " + BaseExceptionEntity.TableName + "." + BaseExceptionEntity.field + " = N'" + systemCode + "'");
            //}
            ////用户主键
            //if (!string.IsNullOrEmpty(userId))
            //{
            //    sb.Append(" AND " + BaseExceptionEntity.TableName + "." + BaseExceptionEntity.FieldUserId + " = N'" + userId + "'");
            //}
            ////用户名
            //if (!string.IsNullOrEmpty(userName))
            //{
            //    sb.Append(" AND " + BaseExceptionEntity.TableName + "." + BaseExceptionEntity.FieldUserName + " = N'" + userName + "'");
            //}
            //创建日期
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseExceptionEntity.TableName + "." + BaseExceptionEntity.FieldCreateTime + " >= '" + startTime + "'");
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseExceptionEntity.TableName + "." + BaseExceptionEntity.FieldCreateTime + " <= DATEADD(s,-1,DATEADD(d,1,'" + endTime + "'))");
            }
            //关键词
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseExceptionEntity.FieldMessage + " LIKE N'%" + searchKey + "%' OR " + BaseExceptionEntity.FieldId + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageIndex, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Put(), null, "*");
        }
        #endregion

        /// <summary>
        /// 获取总数
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public string GetTotalCount(int days)
        {
            var sql = "SELECT COUNT(*) AS TotalCount "
                            + " FROM " + CurrentTableName
                            + "  WHERE (DATEADD(d, " + days + ", " + BaseExceptionEntity.FieldCreateTime + ") >= " + DbHelper.GetDbNow() + ")";
            return DbHelper.ExecuteScalar(sql).ToString();
        }

        #region 上一个下一个
        /// <summary>
        /// 获取上一个下一个编号
        /// </summary>
        /// <param name="currentId">当前编号</param>
        /// <param name="previousId">上一个编号</param>
        /// <param name="nextId">下一个编号</param>
        /// <returns></returns>
        public bool GetPreviousAndNextId(int currentId, out int previousId, out int nextId)
        {
            previousId = currentId;
            nextId = currentId;
            var result = false;
            var sql = "WITH T1 AS( ";
            sql += "SELECT TOP 1 Id AS PreviousId, " + currentId + " AS CurrentId FROM " + CurrentTableName + " WHERE Id < " + currentId + " ORDER BY Id DESC ";
            sql += ") ";
            sql += ",T2 AS ( ";
            sql += "SELECT TOP 1 Id AS NextId, " + currentId + " AS CurrentId FROM " + CurrentTableName + " WHERE Id > " + currentId + " ORDER BY Id ASC ";
            sql += ") ";
            sql += "SELECT ISNULL(T1.PreviousId," + currentId + ") AS PreviousId,ISNULL(T1.CurrentId,T2.CurrentId) AS CurrentId,ISNULL(T2.NextId," + currentId + ") AS NextId FROM T1 FULL JOIN T2 ON T1.CurrentId = T2.CurrentId ";
            var dt = DbHelper.Fill(sql);
            if (dt != null && dt.Rows.Count == 0)
            {
                previousId = currentId;
                nextId = currentId;
                result = false;
            }
            else if (dt != null && (int.Parse(dt.Rows[0]["PreviousId"].ToString()) != currentId || int.Parse(dt.Rows[0]["NextId"].ToString()) != currentId))
            {
                previousId = int.Parse(dt.Rows[0]["PreviousId"].ToString());
                nextId = int.Parse(dt.Rows[0]["NextId"].ToString());
                result = true;
            }
            return result;
        }
        #endregion
    }
}