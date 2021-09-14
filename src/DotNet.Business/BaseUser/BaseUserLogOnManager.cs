//-----------------------------------------------------------------
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
    /// BaseUserLogOnManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///		2015.12.08 版本：1.1 JiRiGaLa	缓存预热功能实现。
    ///     2014.03.25 版本：1.0 宋彪       增加登记用户IP，MACAddress 
    ///		2013.04.21 版本：1.1 JiRiGaLa	主键整理。
    ///     
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.12.08</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserLogOnManager
    {
        /// <summary>
        /// 是否合法的用户
        /// 若有用户的Id，这个可以走索引，效率会很高，若没有Id会是全表扫描了。
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="openId">Key</param>
        /// <param name="systemCode">独立子系统</param>
        /// <returns>合法</returns>
        public bool ValidateOpenId(string userId, string openId, string systemCode = null)
        {
            var result = false;

            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = "Base";
            }

            // 这个是独立业务系统
            //if (systemCode.Equals("PDA"))
            //{
            //    CurrentTableName = "PDAUserLogOn";
            //}

            var parameters = new List<KeyValuePair<string, object>>();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldId, userId));
            }

            if (!string.IsNullOrWhiteSpace(openId))
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldOpenId, openId));
            }

            result = DbUtil.Exists(DbHelper, CurrentTableName, parameters);

            return result;
        }

        /// <summary>
        /// 验证openId是否合法性
        /// </summary>
        /// <param name="openId">key</param>
        /// <param name="userId">用户主键</param>
        /// <returns>合法</returns>
        public bool ValidateOpenId(string openId, out string userId)
        {
            var result = false;

            userId = string.Empty;
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldOpenId, openId)
            };
            userId = DbUtil.GetProperty(DbHelper, CurrentTableName, parameters, BaseUserLogOnEntity.FieldId);
            result = !string.IsNullOrEmpty(userId);

            return result;
        }

        /// <summary>
        /// 获取用户OpenId
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="cachingSystemCode">缓存SystemCode</param>
        /// <returns></returns>
        public string GetUserOpenId(BaseUserInfo userInfo, string cachingSystemCode = null)
        {
            var result = string.Empty;

            using (var dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterReadDbConnection))
            {
                // 需要能支持多个业务子系统的登录方法、多密码、多终端登录
                var userLogOnEntityTableName = "BaseUserLogOn";
                if (!string.IsNullOrEmpty(cachingSystemCode))
                {
                    userLogOnEntityTableName = cachingSystemCode + "UserLogOn";
                }

                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldId, userInfo.Id)
                };
                result = DbUtil.GetProperty(dbHelper, userLogOnEntityTableName, parameters, BaseUserLogOnEntity.FieldOpenId);
                dbHelper.Close();
            }

            return result;
        }

        #region public string GetIdByOpenId(string openId) 获取主键
        /// <summary>
        /// 获取主键
        /// </summary>
        /// <param name="openId">编号</param>
        /// <returns>主键</returns>
        public string GetIdByOpenId(string openId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldOpenId, openId)
            };
            return DbUtil.GetProperty(DbHelper, CurrentTableName, parameters, BaseUserLogOnEntity.FieldId);
        }
        #endregion

        /// <summary>
        /// 获取在线用户，客服
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public string[] GetOnLineUserIds(string[] userIds)
        {
            string[] result = null;

            var sql = "SELECT " + BaseUserLogOnEntity.FieldId + " FROM " + CurrentTableName + " WHERE " + BaseUserLogOnEntity.FieldUserOnLine + " = 1 ";
            if (userIds != null && userIds.Length > 0)
            {
                sql += " AND " + BaseUserLogOnEntity.FieldId + " IN (" + ObjectUtil.ToList(userIds) + ") ";
            }
            var dt = DbHelper.Fill(sql);
            result = BaseUtil.FieldToArray(dt, BaseUserLogOnEntity.FieldId);

            return result;
        }

        /// <summary>
        /// 创建OpenId
        /// </summary>
        /// <returns></returns>
        public string CreateOpenId()
        {
            var result = string.Empty;

            if (UserInfo != null)
            {
                DateTime? openIdTimeout = DateTime.Now.AddHours(8);
                var dbParameters = new List<IDbDataParameter>
                {
                    DbHelper.MakeParameter(BaseUserLogOnEntity.FieldOpenIdTimeout, openIdTimeout)
                };
                result = Guid.NewGuid().ToString("N");
                var sql = "UPDATE " + CurrentTableName + " SET " + BaseUserLogOnEntity.FieldOpenId + " = '" + result + "', " + BaseUserLogOnEntity.FieldOpenIdTimeout + " = " + DbHelper.GetParameter(BaseUserLogOnEntity.FieldOpenIdTimeout) + " WHERE " + BaseUserLogOnEntity.FieldOpenId + " = '" + UserInfo.OpenId + "' ";

                if (!(DbHelper.ExecuteNonQuery(sql, dbParameters.ToArray()) > 0))
                {
                    result = string.Empty;
                }
            }

            return result;
        }

        /// <summary>
        /// 获取在线人数
        /// </summary>
        public string GetOnLineCount()
        {
            var sql = "SELECT COUNT(*) AS UserCount " + " FROM " + CurrentTableName + "  WHERE UserOnLine = 1";
            return DbHelper.ExecuteScalar(sql).ToString();
        }

        /// <summary>
        /// 获取登录次数
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public string GetLogOnCount(int days)
        {
            var sql = "SELECT COUNT(*) AS UserCount FROM " + CurrentTableName + " WHERE Enabled = 1 AND (DATEADD(d, " + days + ", " + BaseUserLogOnEntity.FieldLastVisit + ") > " + DbHelper.GetDbNow() + ")";
            return DbHelper.ExecuteScalar(sql).ToString();
        }

        #region private int ResetVisitInfo(string id) 重置访问情况
        /// <summary>
        /// 重置访问情况
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        private int ResetVisitInfo(string id)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            sqlBuilder.SetNull(BaseUserLogOnEntity.FieldFirstVisit);
            sqlBuilder.SetNull(BaseUserLogOnEntity.FieldPreviousVisit);
            sqlBuilder.SetNull(BaseUserLogOnEntity.FieldLastVisit);
            sqlBuilder.SetValue(BaseUserLogOnEntity.FieldLogOnCount, 0);
            sqlBuilder.SetWhere(BaseUserLogOnEntity.FieldId, id);
            return sqlBuilder.EndUpdate();
        }
        #endregion

        #region public int ResetVisitInfo(string[] ids) 重置
        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int ResetVisitInfo(string[] ids)
        {
            var result = 0;
            for (var i = 0; i < ids.Length; i++)
            {
                if (ids[i].Length > 0)
                {
                    result += ResetVisitInfo(ids[i]);
                }
            }
            return result;
        }
        #endregion

        #region public int ResetVisitInfo() 重置访问情况
        /// <summary>
        /// 重置访问情况
        /// </summary>
        /// <returns>影响行数</returns>
        public int ResetVisitInfo()
        {
            var result = 0;
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            sqlBuilder.SetNull(BaseUserLogOnEntity.FieldFirstVisit);
            sqlBuilder.SetNull(BaseUserLogOnEntity.FieldPreviousVisit);
            sqlBuilder.SetNull(BaseUserLogOnEntity.FieldLastVisit);
            sqlBuilder.SetValue(BaseUserLogOnEntity.FieldLogOnCount, 0);
            result = sqlBuilder.EndUpdate();
            return result;
        }
        #endregion

        #region public int OnLine(string userId, int onLineState = 1) 用户在线

        /// <summary>
        /// 用户在线
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="onLineState">用户在线状态</param>
        /// <param name="onlineOnly">仅已在线用户</param>
        /// <returns>影响行数</returns>
        public int OnLine(string userId, int onLineState = 1, bool onlineOnly = false)
        {
            var result = 0;
            // 是否更新访问日期信息
            if (!BaseSystemInfo.UpdateVisit)
            {
                return result;
            }

#if (DEBUG)
            int milliStart = Environment.TickCount;
#endif

            // 更新在线状态和最后一次访问时间
            var sql = "UPDATE " + CurrentTableName + " SET " + BaseUserLogOnEntity.FieldUserOnLine + " = " + onLineState + ", " + BaseUserLogOnEntity.FieldLastVisit + " = " + DbHelper.GetDbNow();

            //默认给OpenId 8个小时有效期，每次更新在线状态的时候，再刷新一下OpenId的有效期，Troy.Cui 2020-02-29
            DateTime? openIdTimeout = DateTime.Now.AddHours(8);
            sql += ", " + BaseUserLogOnEntity.FieldOpenIdTimeout + " = '" + openIdTimeout + "'";

            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.Access:
                    sql += " WHERE " + BaseUserLogOnEntity.FieldId + " = " + userId + "";
                    break;
                default:
                    sql += " WHERE " + BaseUserLogOnEntity.FieldId + " = " + userId + "";
                    break;
            }
            //只能修改已登录状态的
            if (onlineOnly)
            {
                sql += " AND " + BaseUserLogOnEntity.FieldUserOnLine + " = 1";
            }
            result += DbHelper.ExecuteNonQuery(sql);

            // 写入调试信息
#if (DEBUG)
            int milliEnd = Environment.TickCount;
            Trace.WriteLine(DateTime.Now.ToString(BaseSystemInfo.TimeFormat) + " Ticks: " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " " + " BaseUserManager.OnLine(" + userId + ")");
#endif

            return result;
        }
        #endregion


        // 3分钟不在线，表示用户离开

        #region public int CheckOnLine() 检查用户在线状态(服务器专用)
        /// <summary>
        /// 检查用户在线状态(服务器专用)
        /// </summary>
        /// <returns>影响行数</returns>
        public int CheckOnLine()
        {
            var result = 0;
            // 是否更新访问日期信息
            if (!BaseSystemInfo.UpdateVisit)
            {
                return result;
            }

#if (DEBUG)
            int milliStart = Environment.TickCount;
#endif

            var sql = string.Empty;

            // 最后一次登录时间,用户多了，需要加索引优化
            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.SqlServer:
                    sql = "UPDATE " + CurrentTableName + " SET " + BaseUserLogOnEntity.FieldUserOnLine + " = 0 WHERE  " + BaseUserLogOnEntity.FieldUserOnLine + " > 0 AND (" + BaseUserLogOnEntity.FieldLastVisit + " IS NULL OR (DATEADD(s, " + BaseSystemInfo.OnLineTimeout + ", " + BaseUserLogOnEntity.FieldLastVisit + ") < " + DbHelper.GetDbNow() + "))";
                    result += DbHelper.ExecuteNonQuery(sql);
                    break;
                case CurrentDbType.Oracle:
                    sql = "UPDATE " + CurrentTableName + " SET " + BaseUserLogOnEntity.FieldUserOnLine + " = 0 WHERE  " + BaseUserLogOnEntity.FieldUserOnLine + " > 0 AND (" + BaseUserLogOnEntity.FieldLastVisit + " IS NULL OR " + BaseUserLogOnEntity.FieldLastVisit + " < (SYSDATE - " + BaseSystemInfo.OnLineTimeout + " / 24 * 60 * 60 ))";
                    result += DbHelper.ExecuteNonQuery(sql);
                    break;
                case CurrentDbType.MySql:
                    sql = "UPDATE " + CurrentTableName + " SET " + BaseUserLogOnEntity.FieldUserOnLine + " = 0 WHERE (" + BaseUserLogOnEntity.FieldLastVisit + " IS NULL) OR ((" + BaseUserLogOnEntity.FieldUserOnLine + " > 0) AND (" + BaseUserLogOnEntity.FieldLastVisit + " IS NOT NULL) AND (DATE_ADD(" + BaseUserLogOnEntity.FieldLastVisit + ", Interval " + BaseSystemInfo.OnLineTimeout + " SECOND) < " + DbHelper.GetDbNow() + "))";
                    result += DbHelper.ExecuteNonQuery(sql);
                    break;
                case CurrentDbType.Db2:
                    sql = "UPDATE " + CurrentTableName + " SET " + BaseUserLogOnEntity.FieldUserOnLine + " = 0 WHERE (" + BaseUserLogOnEntity.FieldLastVisit + " IS NULL) OR ((" + BaseUserLogOnEntity.FieldUserOnLine + " > 0) AND (" + BaseUserLogOnEntity.FieldLastVisit + " IS NOT NULL) AND (" + BaseUserLogOnEntity.FieldLastVisit + " + " + BaseSystemInfo.OnLineTimeout + " SECONDS < " + DbHelper.GetDbNow() + "))";
                    result += DbHelper.ExecuteNonQuery(sql);
                    break;
                case CurrentDbType.Access:
                    break;
            }

            // 写入调试信息
#if (DEBUG)
            int milliEnd = Environment.TickCount;
            Trace.WriteLine(DateTime.Now.ToString(BaseSystemInfo.TimeFormat) + " Ticks: " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " " + " BaseUserManager.CheckOnLine()");
#endif

            return result;
        }
        #endregion

        #region public bool CheckOnLineLimit()
        /// <summary>
        /// 同时在线用户数量限制
        /// </summary>
        /// <returns>是否符合限制</returns>
        public bool CheckOnLineLimit()
        {
            var result = false;

#if (DEBUG)
            int milliStart = Environment.TickCount;
#endif

            CheckOnLine();

            var sql = string.Empty;
            // 最后一次登录时间
            sql = "SELECT COUNT(*) FROM " + CurrentTableName + " WHERE " + BaseUserLogOnEntity.FieldUserOnLine + " > 0 ";
            var userOnLine = DbHelper.ExecuteScalar(sql);
            if (userOnLine != null)
            {
                if (BaseSystemInfo.OnLineLimit <= int.Parse(userOnLine.ToString()))
                {
                    result = true;
                }
            }

            // 写入调试信息
#if (DEBUG)
            int milliEnd = Environment.TickCount;
            Trace.WriteLine(DateTime.Now.ToString(BaseSystemInfo.TimeFormat) + " Ticks: " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " " + " BaseUserManager.CheckOnLineLimit()");
#endif

            return result;
        }
        #endregion

        #region public DataTable GetOnLineStateDT() 获取列表
        /// <summary>
        /// 获取在线状态列表
        /// </summary>	
        /// <returns>数据表</returns>
        public DataTable GetOnLineStateDt()
        {
            var sql = "SELECT " + BaseUserLogOnEntity.TableName + "." + BaseUserLogOnEntity.FieldId
                              + ", " + BaseUserLogOnEntity.TableName + "." + BaseUserLogOnEntity.FieldUserOnLine
                              + " FROM " + CurrentTableName
                              + " WHERE " + BaseUserLogOnEntity.TableName + "." + BaseUserLogOnEntity.FieldLastVisit + " IS NOT NULL ";

            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.SqlServer:
                    sql += " AND (DATEADD (s, " + (BaseSystemInfo.OnLineTimeout + 5) + ", " + BaseUserLogOnEntity.FieldLastVisit + ") > " + DbHelper.GetDbNow() + ")";
                    break;
            }
            return DbHelper.Fill(sql);
        }
        #endregion


        #region public int ChangeOnLine(string id) 登录、重新登录、扮演时的在线状态进行更新
        /// <summary>
        /// 登录、重新登录、扮演时的在线状态进行更新
        /// </summary>
        /// <param name="id">当前用户</param>
        /// <returns>是否在线</returns>
        public int ChangeOnLine(string id)
        {
            var result = 0;
            // 是自己在线，然后重新登录为别人时，需要把自己注销掉
            if (UserInfo != null && !string.IsNullOrEmpty(UserInfo.Id))
            {
                if (!string.IsNullOrEmpty(UserInfo.OpenId) && !UserInfo.Id.Equals(id))
                {
                    // 要设置为下线状态，这里要判断游客状态
                    if (SignOut(UserInfo.OpenId))
                    {
                        result += 1;
                    }
                }
            }
            // 用户在线
            result += OnLine(id);

            return result;
        }
        #endregion
    }
}