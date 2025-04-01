//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------
using System.Collections.Generic;
using System;

namespace DotNet.Business
{
    using Model;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Util;

    /// <summary>
    /// BaseUserLogonManager
    /// 用户登录管理
    /// 
    /// 修改记录
    /// 
    ///		2022.02.08 版本：1.0 Troy.Cui	新增。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2022.02.08</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserLogonManager : BaseManager
    {
        #region ForceOffline强制下线

        /// <summary>
        /// 强制下线
        /// </summary>
        /// <param name="userIds">用户编号</param>
        /// <returns>更新成功记录数</returns>
        public int ForceOffline(string[] userIds)
        {
            var result = 0;
            if (userIds != null)
            {
                result = Update(BaseUtil.FieldUserId, userIds, new KeyValuePair<string, object>(BaseUserLogonEntity.FieldUserOnline, 0));
                //操作日志
                var entity = new BaseLogEntity
                {
                    Parameters = userIds.ToString(),
                    Description = "强制下线：" + ((result >= 1) ? "成功" : "失败")
                };
                if (UserInfo != null)
                {
                    entity.UserId = UserInfo.Id.ToInt();
                    entity.RealName = UserInfo.RealName;
                }
                new BaseLogManager(UserInfo).Add(entity);
            }

            return result;
        }
        #endregion

        #region SetConcurrentUser设置并发用户

        /// <summary>
        /// 设置并发用户
        /// </summary>
        /// <param name="userIds">用户编号</param>
        /// <returns>更新成功记录数</returns>
        public int SetConcurrentUser(string[] userIds)
        {
            var result = 0;
            if (userIds != null)
            {
                result = Update(BaseUtil.FieldUserId, userIds, new KeyValuePair<string, object>(BaseUserLogonEntity.FieldConcurrentUser, 1));
                //操作日志
                var entity = new BaseLogEntity
                {
                    Parameters = userIds.ToString(),
                    Description = "设置并发用户：" + ((result >= 1) ? "成功" : "失败")
                };
                if (UserInfo != null)
                {
                    entity.UserId = UserInfo.Id.ToInt();
                    entity.RealName = UserInfo.RealName;
                }
                new BaseLogManager(UserInfo).Add(entity);
            }

            return result;
        }
        #endregion

        #region SetConcurrentUser撤销设置并发用户

        /// <summary>
        /// 撤销设置并发用户
        /// </summary>
        /// <param name="userIds">用户编号</param>
        /// <returns>更新成功记录数</returns>
        public int UndoSetConcurrentUser(string[] userIds)
        {
            var result = 0;
            if (userIds != null)
            {
                result = Update(BaseUtil.FieldUserId, userIds, new KeyValuePair<string, object>(BaseUserLogonEntity.FieldConcurrentUser, 0));
                //操作日志
                var entity = new BaseLogEntity
                {
                    Parameters = userIds.ToString(),
                    Description = "撤销设置并发用户：" + ((result >= 1) ? "成功" : "失败")
                };
                if (UserInfo != null)
                {
                    entity.UserId = UserInfo.Id.ToInt();
                    entity.RealName = UserInfo.RealName;
                }
                new BaseLogManager(UserInfo).Add(entity);
            }

            return result;
        }
        #endregion

        #region Lock锁定用户

        /// <summary>
        /// 锁定用户
        /// </summary>
        /// <param name="userIds">用户编号</param>
        /// <param name="minutes">锁定分钟数</param>
        /// <returns>更新成功记录数</returns>
        public int Lock(string[] userIds, int minutes = 365)
        {
            var result = 0;
            if (minutes > 9999)
            {
                minutes = 9999;
            }
            if (userIds != null)
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserLogonEntity.FieldLockStartTime, DateTime.Now),
                    new KeyValuePair<string, object>(BaseUserLogonEntity.FieldLockEndTime, DateTime.Now.AddMinutes(minutes))
                };

                result = Update(BaseUtil.FieldUserId, userIds, parameters);
                //操作日志
                var entity = new BaseLogEntity
                {
                    Parameters = userIds.ToString(),
                    Description = "锁定用户：" + ((result >= 1) ? "成功" : "失败")
                };
                if (UserInfo != null)
                {
                    entity.UserId = UserInfo.Id.ToInt();
                    entity.RealName = UserInfo.RealName;
                }
                new BaseLogManager(UserInfo).Add(entity);
            }

            return result;
        }
        #endregion

        #region Unlock解除锁定用户

        /// <summary>
        /// 解除锁定用户
        /// </summary>
        /// <param name="userIds">用户编号</param>
        /// <returns>更新成功记录数</returns>
        public int Unlock(string[] userIds)
        {
            var result = 0;
            if (userIds != null)
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserLogonEntity.FieldLockStartTime, null),
                    new KeyValuePair<string, object>(BaseUserLogonEntity.FieldLockEndTime, null)
                };

                result = Update(BaseUtil.FieldUserId, userIds, parameters);
                //操作日志
                var entity = new BaseLogEntity
                {
                    Parameters = userIds.ToString(),
                    Description = "解除锁定用户：" + ((result >= 1) ? "成功" : "失败")
                };
                if (UserInfo != null)
                {
                    entity.UserId = UserInfo.Id.ToInt();
                    entity.RealName = UserInfo.RealName;
                }
                new BaseLogManager(UserInfo).Add(entity);
            }

            return result;
        }
        #endregion

        #region 根据UserId获取实体
        /// <summary>
        /// 根据UserId获取实体
        /// </summary>
        /// <param name="userId">主键</param>
        public BaseUserLogonEntity GetEntityByUserId(string userId)
        {
            return ValidateUtil.IsInt(userId) ? GetEntityByUserId(userId.ToInt()) : null;
        }

        /// <summary>
        /// 根据UserId获取实体
        /// </summary>
        /// <param name="userId">主键</param>
        public BaseUserLogonEntity GetEntityByUserId(int userId)
        {
            return BaseEntity.Create<BaseUserLogonEntity>(GetDataTable(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldUserId, userId))); ;
            //var cacheKey = CurrentTableName + ".Entity." + id;
            //var cacheTime = TimeSpan.FromMilliseconds(86400000);
            //return CacheUtil.Cache<BaseUserLogonEntity>(cacheKey, () => BaseEntity.Create<BaseUserLogonEntity>(GetDataTable(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldUserId, userId))), true, false, cacheTime);
        }
        #endregion

        #region public bool ValidateOpenId(string userId, string openId, string systemCode = null)
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

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserLogonEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseUserLogonEntity.FieldEnabled, 1)
            };
            if (!string.IsNullOrWhiteSpace(userId))
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldUserId, userId));
            }

            if (!string.IsNullOrWhiteSpace(openId))
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldOpenId, openId));
            }

            result = DbHelper.Exists(CurrentTableName, parameters);

            return result;
        }

        #endregion

        #region public bool ValidateOpenId(string openId, out string userId)
        /// <summary>
        /// 验证openId是否合法性
        /// </summary>
        /// <param name="openId">key</param>
        /// <param name="userId">用户主键</param>
        /// <returns>合法</returns>
        public bool ValidateOpenId(string openId, out string userId)
        {
            var result = false;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserLogonEntity.FieldOpenId, openId)
            };
            userId = DbHelper.GetProperty(CurrentTableName, parameters, BaseUserLogonEntity.FieldUserId);
            result = !string.IsNullOrEmpty(userId);

            return result;
        }

        #endregion

        #region public string GetUserOpenId(BaseUserInfo userInfo, string cachingSystemCode = null)
        /// <summary>
        /// 获取用户OpenId
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="cachingSystemCode">缓存SystemCode</param>
        /// <returns></returns>
        public string GetUserOpenId(BaseUserInfo userInfo, string cachingSystemCode = null)
        {
            var result = string.Empty;

            using (var dbHelper = DbHelperFactory.Create(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterReadDbConnection))
            {
                // 需要能支持多个业务子系统的登录方法、多密码、多终端登录
                var userLogonTableName = BaseUserLogonEntity.CurrentTableName;

                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserLogonEntity.FieldUserId, userInfo.Id)
                };
                result = dbHelper.GetProperty(userLogonTableName, parameters, BaseUserLogonEntity.FieldOpenId);
                dbHelper.Close();
            }

            return result;
        }

        #endregion

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
                new KeyValuePair<string, object>(BaseUserLogonEntity.FieldOpenId, openId)
            };
            return DbHelper.GetProperty(CurrentTableName, parameters, BaseUserLogonEntity.FieldUserId);
        }
        #endregion

        #region public string[] GetOnlineUserIds(string[] userIds)

        /// <summary>
        /// 获取在线用户，客服
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public string[] GetOnlineUserIds(string[] userIds)
        {
            string[] result = null;
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + BaseUserLogonEntity.FieldUserId + " FROM " + CurrentTableName + " WHERE " + BaseUserLogonEntity.FieldUserOnline + " = 1");
            if (userIds != null && userIds.Length > 0)
            {
                sb.Append(" AND " + BaseUserLogonEntity.FieldUserId + " IN (" + ObjectUtil.ToList(userIds) + ") ");
            }
            var dt = DbHelper.Fill(sb.Return());
            result = BaseUtil.FieldToArray(dt, BaseUserLogonEntity.FieldUserId);

            return result;
        }

        #endregion

        #region public string CreateOpenId()

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
                    DbHelper.MakeParameter(BaseUserLogonEntity.FieldOpenIdTimeoutTime, openIdTimeout)
                };
                result = Guid.NewGuid().ToString("N");
                var sb = PoolUtil.StringBuilder.Get();
                sb.Append("UPDATE " + CurrentTableName + " SET " + BaseUserLogonEntity.FieldOpenId + " = '" + result + "', " + BaseUserLogonEntity.FieldOpenIdTimeoutTime + " = " + DbHelper.GetParameter(BaseUserLogonEntity.FieldOpenIdTimeoutTime) + " WHERE " + BaseUserLogonEntity.FieldOpenId + " = '" + UserInfo.OpenId + "' ");

                if (!(ExecuteNonQuery(sb.Return(), dbParameters.ToArray()) > 0))
                {
                    result = string.Empty;
                }
            }

            return result;
        }

        #endregion

        #region public string GetOnlineCount()
        /// <summary>
        /// 获取在线人数
        /// </summary>
        public string GetOnlineCount()
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) AS UserCount " + " FROM " + CurrentTableName + " WHERE " + BaseUserLogonEntity.FieldUserOnline + " = 1");
            return DbHelper.ExecuteScalar(sb.Return()).ToInt().ToString();
        }

        #endregion

        #region GetLogonCount 获取登录次数
        /// <summary>
        /// 获取登录次数
        /// </summary>
        /// <param name="days">最近多少天</param>
        /// <param name="currentWeek">当周</param>
        /// <param name="currentMonth">当月</param>
        /// <param name="currentQuarter">当季</param>
        /// <param name="currentYear">当年</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public int GetLogonCount(int days = 0, bool currentWeek = false, bool currentMonth = false, bool currentQuarter = false, bool currentYear = false, string startTime = null, string endTime = null)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) AS UserCount FROM " + CurrentTableName + " WHERE " + BaseUserLogonEntity.FieldEnabled + " = 1 AND " + BaseUserLogonEntity.FieldDeleted + " = 0");
            if (days > 0)
            {
                sb.Append(" AND (DATEADD(d, " + days + ", " + BaseUserLogonEntity.FieldLastVisitTime + ") > GETDATE())");
            }
            if (currentWeek)
            {
                sb.Append(" AND DATEDIFF(ww," + BaseUserLogonEntity.FieldLastVisitTime + ",GETDATE()) = 0");
            }
            if (currentMonth)
            {
                sb.Append(" AND DATEDIFF(mm," + BaseUserLogonEntity.FieldLastVisitTime + ",GETDATE()) = 0");
            }
            if (currentQuarter)
            {
                sb.Append(" AND DATEDIFF(qq," + BaseUserLogonEntity.FieldLastVisitTime + ",GETDATE()) = 0");
            }
            if (currentYear)
            {
                sb.Append(" AND DATEDIFF(yy," + BaseUserLogonEntity.FieldLastVisitTime + ",GETDATE()) = 0");
            }
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseUserLogonEntity.FieldLastVisitTime + " >= " + startTime + ")");
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseUserLogonEntity.FieldLastVisitTime + " < " + endTime + ")");
            }
            return DbHelper.ExecuteScalar(sb.Return()).ToInt();
        }

        #endregion

        #region private int ResetVisitInfo(string id) 重置访问情况
        /// <summary>
        /// 重置访问情况
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns>影响行数</returns>
        private int ResetVisitInfo(string userId)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            sqlBuilder.SetNull(BaseUserLogonEntity.FieldFirstVisitTime);
            sqlBuilder.SetNull(BaseUserLogonEntity.FieldPreviousVisitTime);
            sqlBuilder.SetNull(BaseUserLogonEntity.FieldLastVisitTime);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldLogonCount, 0);
            sqlBuilder.SetWhere(BaseUserLogonEntity.FieldUserId, userId);
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

        #region public int Online(string userId, int onLineState = 1) 用户在线

        /// <summary>
        /// 用户在线
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="onLineState">用户在线状态</param>
        /// <param name="onlineOnly">仅已在线用户</param>
        /// <returns>影响行数</returns>
        public int Online(int userId, int onLineState = 1, bool onlineOnly = false)
        {
            var result = 0;
            // 是否更新访问日期信息
            if (!BaseSystemInfo.UpdateVisit)
            {
                return result;
            }

#if (DEBUG)
            var milliStart = Environment.TickCount;
#endif
            var sb = PoolUtil.StringBuilder.Get();
            // 更新在线状态和最后一次访问时间
            sb.Append("UPDATE " + CurrentTableName + " SET " + BaseUserLogonEntity.FieldUserOnline + " = " + onLineState + ", " + BaseUserLogonEntity.FieldLastVisitTime + " = " + DbHelper.GetDbNow());

            //默认给OpenId 8个小时有效期，每次更新在线状态的时候，再刷新一下OpenId的有效期，Troy.Cui 2020-02-29
            DateTime? openIdTimeout = DateTime.Now.AddHours(8);
            sb.Append(", " + BaseUserLogonEntity.FieldOpenIdTimeoutTime + " = '" + openIdTimeout + "'");

            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.Access:
                    sb.Append(" WHERE " + BaseUserLogonEntity.FieldUserId + " = " + userId + "");
                    break;
                default:
                    sb.Append(" WHERE " + BaseUserLogonEntity.FieldUserId + " = " + userId + "");
                    break;
            }
            //只能修改已登录状态的
            if (onlineOnly)
            {
                sb.Append(" AND " + BaseUserLogonEntity.FieldUserOnline + " = 1");
            }
            result += ExecuteNonQuery(sb.Return());

            // 写入调试信息
#if (DEBUG)
            var milliEnd = Environment.TickCount;
            Trace.WriteLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeLongFormat) + " Ticks: " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " " + " BaseUserManager.Online(" + userId + ")");
#endif

            return result;
        }
        #endregion

        // 3分钟不在线，表示用户离开

        #region public int CheckOnline() 检查用户在线状态(服务器专用)
        /// <summary>
        /// 检查用户在线状态(服务器专用)
        /// </summary>
        /// <returns>影响行数</returns>
        public int CheckOnline()
        {
            var result = 0;
            // 是否更新访问日期信息
            if (!BaseSystemInfo.UpdateVisit)
            {
                return result;
            }

#if (DEBUG)
            var milliStart = Environment.TickCount;
#endif
            var sb = PoolUtil.StringBuilder.Get();

            // 最后一次登录时间,用户多了，需要加索引优化
            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.SqlServer:
                    sb.Append("UPDATE " + CurrentTableName + " SET " + BaseUserLogonEntity.FieldUserOnline + " = 0 WHERE " + BaseUserLogonEntity.FieldUserOnline + " > 0 AND (" + BaseUserLogonEntity.FieldLastVisitTime + " IS NULL OR (DATEADD(s, " + BaseSystemInfo.OnlineTimeout + ", " + BaseUserLogonEntity.FieldLastVisitTime + ") < GETDATE()))");
                    result += ExecuteNonQuery(sb.Return());
                    break;
                case CurrentDbType.Oracle:
                    sb.Append("UPDATE " + CurrentTableName + " SET " + BaseUserLogonEntity.FieldUserOnline + " = 0 WHERE " + BaseUserLogonEntity.FieldUserOnline + " > 0 AND (" + BaseUserLogonEntity.FieldLastVisitTime + " IS NULL OR " + BaseUserLogonEntity.FieldLastVisitTime + " < (SYSDATE - " + BaseSystemInfo.OnlineTimeout + " / 24 * 60 * 60 ))");
                    result += ExecuteNonQuery(sb.Return());
                    break;
                case CurrentDbType.MySql:
                    sb.Append("UPDATE " + CurrentTableName + " SET " + BaseUserLogonEntity.FieldUserOnline + " = 0 WHERE (" + BaseUserLogonEntity.FieldLastVisitTime + " IS NULL) OR ((" + BaseUserLogonEntity.FieldUserOnline + " > 0) AND (" + BaseUserLogonEntity.FieldLastVisitTime + " IS NOT NULL) AND (DATE_ADD(" + BaseUserLogonEntity.FieldLastVisitTime + ", Interval " + BaseSystemInfo.OnlineTimeout + " SECOND) < now()))");
                    result += ExecuteNonQuery(sb.Return());
                    break;
                case CurrentDbType.Db2:
                    sb.Append("UPDATE " + CurrentTableName + " SET " + BaseUserLogonEntity.FieldUserOnline + " = 0 WHERE (" + BaseUserLogonEntity.FieldLastVisitTime + " IS NULL) OR ((" + BaseUserLogonEntity.FieldUserOnline + " > 0) AND (" + BaseUserLogonEntity.FieldLastVisitTime + " IS NOT NULL) AND (" + BaseUserLogonEntity.FieldLastVisitTime + " + " + BaseSystemInfo.OnlineTimeout + " SECONDS < " + DbHelper.GetDbNow() + "))");
                    result += ExecuteNonQuery(sb.Return());
                    break;
                case CurrentDbType.Access:
                    sb.Return();
                    break;
            }

            // 写入调试信息
#if (DEBUG)
            var milliEnd = Environment.TickCount;
            Trace.WriteLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeLongFormat) + " Ticks: " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " " + " BaseUserManager.CheckOnline()");
#endif
            return result;
        }
        #endregion

        #region public bool CheckOnlineLimit()
        /// <summary>
        /// 同时在线用户数量限制
        /// </summary>
        /// <returns>是否符合限制</returns>
        public bool CheckOnlineLimit()
        {
            var result = false;

#if (DEBUG)
            var milliStart = Environment.TickCount;
#endif

            CheckOnline();
            var sb = PoolUtil.StringBuilder.Get();
            // 最后一次登录时间
            sb.Append("SELECT COUNT(*) FROM " + CurrentTableName + " WHERE " + BaseUserLogonEntity.FieldUserOnline + " > 0");
            var obj = DbHelper.ExecuteScalar(sb.Return());
            if (obj != null)
            {
                if (BaseSystemInfo.OnlineLimit <= obj.ToInt())
                {
                    result = true;
                }
            }

            // 写入调试信息
#if (DEBUG)
            var milliEnd = Environment.TickCount;
            Trace.WriteLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeLongFormat) + " Ticks: " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " " + " BaseUserManager.CheckOnlineLimit()");
#endif

            return result;
        }
        #endregion

        #region public DataTable GetOnlineStateDT() 获取列表
        /// <summary>
        /// 获取在线状态列表
        /// </summary>	
        /// <returns>数据表</returns>
        public DataTable GetOnlineStateDt()
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldUserId
                              + ", " + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldUserOnline
                              + " FROM " + CurrentTableName
                              + " WHERE " + BaseUserLogonEntity.CurrentTableName + "." + BaseUserLogonEntity.FieldLastVisitTime + " IS NOT NULL ");

            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.SqlServer:
                    sb.Append(" AND (DATEADD (s, " + (BaseSystemInfo.OnlineTimeout + 5) + ", " + BaseUserLogonEntity.FieldLastVisitTime + ") > GETDATE())");
                    break;
            }
            return DbHelper.Fill(sb.Return());
        }
        #endregion

        #region public int ChangeOnline(string id) 登录、重新登录、扮演时的在线状态进行更新
        /// <summary>
        /// 登录、重新登录、扮演时的在线状态进行更新
        /// </summary>
        /// <param name="id">当前用户</param>
        /// <returns>是否在线</returns>
        public int ChangeOnline(int id)
        {
            var result = 0;
            // 是自己在线，然后重新登录为别人时，需要把自己注销掉
            if (UserInfo != null && UserInfo.UserId > 0)
            {
                if (!string.IsNullOrEmpty(UserInfo.OpenId) && !UserInfo.UserId.Equals(id))
                {
                    // 要设置为下线状态，这里要判断游客状态
                    if (SignOut(UserInfo.OpenId))
                    {
                        result += 1;
                    }
                }
            }
            // 用户在线
            result += Online(id);

            return result;
        }
        #endregion

        #region public int Update(BaseUserLogonEntity entity, bool logHistory)
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="logHistory">记录历史</param>
        public int Update(BaseUserLogonEntity entity, bool logHistory)
        {
            var result = 0;
            if (logHistory)
            {
                // 获取原始实体信息
                var entityOld = GetEntity(entity.Id);
                //2016-11-23 欧腾飞加入判断 原始实体信息如果为null 则不去保存修改记录(原始方法 实体可能为null如果去保存修改记录则会报错)
                if (entityOld != null)
                {
                    // 保存修改记录
                    SaveEntityChangeLog(entity, entityOld);
                }
            }
            // 更新数据
            result = UpdateEntity(entity);
            // 重新缓存

            return result;
        }

        #endregion

        #region SaveEntityChangeLog
        /// <summary>
        /// 保存实体修改记录
        /// </summary>
        /// <param name="entityNew">修改后的实体对象</param>
        /// <param name="entityOld">修改前的实体对象</param>
        /// <param name="tableName">表名</param>
        public void SaveEntityChangeLog(BaseUserLogonEntity entityNew, BaseUserLogonEntity entityOld, string tableName = null)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                //统一放在一个公共表 Troy.Cui 2016-08-17
                tableName = BaseChangeLogEntity.CurrentTableName;
            }
            var manager = new BaseChangeLogManager(UserInfo, tableName);
            foreach (var property in typeof(BaseUserLogonEntity).GetProperties())
            {
                var oldValue = Convert.ToString(property.GetValue(entityOld, null));
                var newValue = Convert.ToString(property.GetValue(entityNew, null));
                var fieldDescription = property.GetCustomAttributes(typeof(FieldDescription), false).FirstOrDefault() as FieldDescription;
                //不记录创建人、修改人、没有修改的记录
                if (!fieldDescription.NeedLog || oldValue == newValue)
                {
                    continue;
                }
                var baseChangeLogEntity = new BaseChangeLogEntity
                {
                    TableName = CurrentTableName,
                    TableDescription = CurrentTableDescription,
                    ColumnName = property.Name,
                    ColumnDescription = fieldDescription.Text,
                    NewValue = newValue,
                    OldValue = oldValue,
                    RecordKey = entityOld.Id.ToString(),
                    SortCode = 1 // 不要排序了，加快写入速度
                };
                manager.Add(baseChangeLogEntity, true, false);
            }
        }
        #endregion

        #region 访问时间VisitTime

        #region public void UpdateVisitTimeTask(object param)
        /// <summary>
        /// 更新访问数据
        /// </summary>
        /// <param name="param"></param>
        public void UpdateVisitTimeTask(object param)
        {
            if (param is Tuple<BaseUserLogonEntity, string, bool> tuple)
            {
                var userLogonEntity = tuple.Item1;
                var openId = tuple.Item2;
                var createOpenId = tuple.Item3;
                UpdateVisitTimeTask(userLogonEntity, openId, createOpenId);
            }
        }
        #endregion

        #region public void UpdateVisitTimeTask(BaseUserLogonEntity userLogonEntity, string openId, bool createOpenId = true)
        /// <summary>
        /// 更新访问数据
        /// </summary>
        /// <param name="userLogonEntity"></param>
        /// <param name="openId"></param>
        /// <param name="createOpenId"></param>
        public void UpdateVisitTimeTask(BaseUserLogonEntity userLogonEntity, string openId, bool createOpenId = true)
        {
            var errorMark = 0;

            var sb = PoolUtil.StringBuilder.Get();
            //默认给OpenId 8个小时有效期，每次更新在线状态的时候，再刷新一下OpenId的有效期，Troy.Cui 2020-02-29
            DateTime? openIdTimeout = DateTime.Now.AddHours(8);
            try
            {
                using (var dbHelper = DbHelperFactory.Create(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection))
                {
                    // 是否更新访问日期信息
                    List<IDbDataParameter> dbParameters = null;
                    // 若有一周没登录了，需要重新进行手机验证
                    var mobileNeedValiated = false;
                    if (userLogonEntity.PreviousVisitTime.HasValue || userLogonEntity.FirstVisitTime.HasValue)
                    {
                        var ts = new TimeSpan();
                        if (userLogonEntity.LastVisitTime.HasValue)
                        {
                            ts = DateTime.Now.Subtract((DateTime)userLogonEntity.LastVisitTime);
                            mobileNeedValiated = (ts.TotalDays > 7);
                        }
                        else if (userLogonEntity.FirstVisitTime.HasValue)
                        {
                            ts = DateTime.Now.Subtract((DateTime)userLogonEntity.FirstVisitTime);
                            mobileNeedValiated = (ts.TotalDays > 7);
                        }
                        if (mobileNeedValiated)
                        {
                            sb.Append("UPDATE " + BaseUserContactEntity.CurrentTableName
                                     + " SET " + BaseUserContactEntity.FieldMobileValidated + " = 0"
                                     + " WHERE " + BaseUserContactEntity.FieldUserId + " = " + DbHelper.GetParameter(BaseUserContactEntity.FieldUserId)
                                     + " AND " + BaseUserContactEntity.FieldMobileValidated + " = " + DbHelper.GetParameter(BaseUserContactEntity.FieldMobileValidated));

                            dbParameters = new List<IDbDataParameter>
                            {
                                DbHelper.MakeParameter(BaseUserContactEntity.FieldUserId, userLogonEntity.UserId),
                                DbHelper.MakeParameter(BaseUserContactEntity.FieldMobileValidated, 1)
                            };

                            errorMark = 10;
                            dbHelper.ExecuteNonQuery(sb.Return(), dbParameters.ToArray());
                        }
                    }

                    if (BaseSystemInfo.UpdateVisit)
                    {
                        // 第一次登录时间
                        if (userLogonEntity.FirstVisitTime == null)
                        {
                            sb = PoolUtil.StringBuilder.Get();
                            sb.Append("UPDATE " + CurrentTableName
                                        + " SET " + BaseUserLogonEntity.FieldPasswordErrorCount + " = 0"
                                        + ", " + BaseUserLogonEntity.FieldUserOnline + " = 1"
                                        + ", " + BaseUserLogonEntity.FieldFirstVisitTime + " = " + dbHelper.GetDbNow()
                                        + ", " + BaseUserLogonEntity.FieldLogonCount + " = 1"
                                        + ", " + BaseUserLogonEntity.FieldSystemCode + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldSystemCode)
                                        + ", " + BaseUserLogonEntity.FieldIpAddress + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldIpAddress)
                                        + ", " + BaseUserLogonEntity.FieldIpAddressName + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldIpAddressName)
                                        + ", " + BaseUserLogonEntity.FieldMacAddress + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldMacAddress)
                                        + ", " + BaseUserLogonEntity.FieldComputerName + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldComputerName));

                            dbParameters = new List<IDbDataParameter>
                            {
                                dbHelper.MakeParameter(BaseUserLogonEntity.FieldSystemCode, userLogonEntity.SystemCode),
                                dbHelper.MakeParameter(BaseUserLogonEntity.FieldIpAddress, userLogonEntity.IpAddress),
                                dbHelper.MakeParameter(BaseUserLogonEntity.FieldIpAddressName, userLogonEntity.IpAddressName),
                                dbHelper.MakeParameter(BaseUserLogonEntity.FieldMacAddress, userLogonEntity.MacAddress),
                                dbHelper.MakeParameter(BaseUserLogonEntity.FieldComputerName, userLogonEntity.ComputerName)
                            };

                            if (createOpenId)
                            {
                                sb.Append(", " + BaseUserLogonEntity.FieldOpenId + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldOpenId));
                                sb.Append(", " + BaseUserLogonEntity.FieldOpenIdTimeoutTime + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldOpenIdTimeoutTime));
                                dbParameters.Add(dbHelper.MakeParameter(BaseUserLogonEntity.FieldOpenId, openId));
                                dbParameters.Add(dbHelper.MakeParameter(BaseUserLogonEntity.FieldOpenIdTimeoutTime, openIdTimeout));
                            }

                            sb.Append(" WHERE " + BaseUserLogonEntity.FieldUserId + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldUserId)
                                        + " AND " + BaseUserLogonEntity.FieldFirstVisitTime + " IS NULL");
                            dbParameters.Add(dbHelper.MakeParameter(BaseUserLogonEntity.FieldUserId, userLogonEntity.UserId));

                            errorMark = 20;
                            dbHelper.ExecuteNonQuery(sb.Return(), dbParameters.ToArray());
                        }
                        else
                        {
                            // 最后一次登录时间
                            sb = PoolUtil.StringBuilder.Get();
                            sb.Append("UPDATE " + CurrentTableName
                                        + " SET " + BaseUserLogonEntity.FieldPasswordErrorCount + " = 0"
                                        + ", " + BaseUserLogonEntity.FieldPreviousVisitTime + " = " + BaseUserLogonEntity.FieldLastVisitTime
                                        + ", " + BaseUserLogonEntity.FieldUserOnline + " = 1"
                                        + ", " + BaseUserLogonEntity.FieldLastVisitTime + " = " + dbHelper.GetDbNow()
                                        + ", " + BaseUserLogonEntity.FieldLogonCount + " = " + BaseUserLogonEntity.FieldLogonCount + " + 1 "
                                        + ", " + BaseUserLogonEntity.FieldSystemCode + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldSystemCode)
                                        + ", " + BaseUserLogonEntity.FieldIpAddress + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldIpAddress)
                                        + ", " + BaseUserLogonEntity.FieldIpAddressName + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldIpAddressName)
                                        + ", " + BaseUserLogonEntity.FieldMacAddress + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldMacAddress)
                                        + ", " + BaseUserLogonEntity.FieldComputerName + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldComputerName));

                            dbParameters = new List<IDbDataParameter>
                            {
                                dbHelper.MakeParameter(BaseUserLogonEntity.FieldSystemCode, userLogonEntity.SystemCode),
                                dbHelper.MakeParameter(BaseUserLogonEntity.FieldIpAddress, userLogonEntity.IpAddress),
                                dbHelper.MakeParameter(BaseUserLogonEntity.FieldIpAddressName, userLogonEntity.IpAddressName),
                                dbHelper.MakeParameter(BaseUserLogonEntity.FieldMacAddress, userLogonEntity.MacAddress),
                                dbHelper.MakeParameter(BaseUserLogonEntity.FieldComputerName, userLogonEntity.ComputerName)
                            };

                            if (createOpenId)
                            {
                                sb.Append(", " + BaseUserLogonEntity.FieldOpenId + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldOpenId));
                                sb.Append(", " + BaseUserLogonEntity.FieldOpenIdTimeoutTime + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldOpenIdTimeoutTime));
                                dbParameters.Add(dbHelper.MakeParameter(BaseUserLogonEntity.FieldOpenId, openId));
                                dbParameters.Add(dbHelper.MakeParameter(BaseUserLogonEntity.FieldOpenIdTimeoutTime, openIdTimeout));
                            }

                            sb.Append(" WHERE " + BaseUserLogonEntity.FieldUserId + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldUserId));
                            dbParameters.Add(dbHelper.MakeParameter(BaseUserLogonEntity.FieldUserId, userLogonEntity.UserId));

                            errorMark = 30;
                            dbHelper.ExecuteNonQuery(sb.Return(), dbParameters.ToArray());
                        }
                    }
                    else
                    {
                        sb = PoolUtil.StringBuilder.Get();
                        sb.Append("UPDATE " + CurrentTableName
                                     + " SET  " + BaseUserLogonEntity.FieldPasswordErrorCount + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldPasswordErrorCount)
                                     + ", " + BaseUserLogonEntity.FieldSystemCode + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldSystemCode));
                        dbParameters = new List<IDbDataParameter>
                        {
                            dbHelper.MakeParameter(BaseUserLogonEntity.FieldPasswordErrorCount, 0),
                            dbHelper.MakeParameter(BaseUserLogonEntity.FieldSystemCode, userLogonEntity.SystemCode)
                        };

                        if (createOpenId)
                        {
                            sb.Append(", " + BaseUserLogonEntity.FieldOpenId + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldOpenId));
                            sb.Append(", " + BaseUserLogonEntity.FieldOpenIdTimeoutTime + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldOpenIdTimeoutTime));
                            dbParameters.Add(dbHelper.MakeParameter(BaseUserLogonEntity.FieldOpenId, openId));
                            dbParameters.Add(dbHelper.MakeParameter(BaseUserLogonEntity.FieldOpenIdTimeoutTime, openIdTimeout));
                        }

                        sb.Append(" WHERE " + BaseUserLogonEntity.FieldUserId + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldUserId));
                        // sql += " AND " + BaseUserEntity.FieldOpenId + " IS NULL ";
                        dbParameters.Add(dbHelper.MakeParameter(BaseUserLogonEntity.FieldUserId, userLogonEntity.UserId));

                        errorMark = 40;
                        dbHelper.ExecuteNonQuery(sb.Return(), dbParameters.ToArray());

                    }
                }
            }
            catch (Exception ex)
            {
                var exception = "BaseUserLogonManager.UpdateVisitTimeTask:发生时间:" + DateTime.Now
                    + Environment.NewLine + "errorMark = " + errorMark
                    + Environment.NewLine + "UserInfo:" + UserInfo.Serialize()
                    + Environment.NewLine + "Message:" + ex.Message
                    + Environment.NewLine + "Source:" + ex.Source
                    + Environment.NewLine + "StackTrace:" + ex.StackTrace
                    + Environment.NewLine + "TargetSite:" + ex.TargetSite
                    + Environment.NewLine;

                LogUtil.WriteLog(exception, "Exception");
            }
        }

        #endregion

        #region public string UpdateVisitTime(BaseUserLogonEntity userLogonEntity, bool createOpenId = true)
        /// <summary>
        /// 更新访问数据
        /// </summary>
        /// <param name="userLogonEntity"></param>
        /// <param name="createOpenId"></param>
        /// <returns></returns>
        public string UpdateVisitTime(BaseUserLogonEntity userLogonEntity, bool createOpenId = true)
        {
            var openId = string.IsNullOrEmpty(userLogonEntity?.OpenId) ? Guid.NewGuid().ToString("N") : userLogonEntity?.OpenId;

            //Troy.Cui 2020-02-29 强制每次都自动生成，但对于可并发用户，OpenId过期了才更新一下OpenId
            //Troy.Cui 并发用户需要检测下OpenId过期时间 2020-06-17
            if (createOpenId && userLogonEntity.ConcurrentUser == 1 && userLogonEntity.OpenIdTimeoutTime.HasValue && !string.IsNullOrEmpty(userLogonEntity.OpenId))
            {
                //var timeSpan = DateTime.Now - userLogonEntity.OpenIdTimeoutTime.Value;
                var timeSpan = userLogonEntity.OpenIdTimeoutTime.Value - DateTime.Now;
                if ((timeSpan.TotalSeconds) < 0)
                {
                    openId = userLogonEntity.OpenId;
                }
            }
            // Troy.Cui 非并发用户强制每次生成 2020-06-17
            if (createOpenId && userLogonEntity.ConcurrentUser == 0)
            {
                openId = Guid.NewGuid().ToString("N");
            }

            //UpdateVisitTimeTask(userLogonEntity, openId, createOpenId);
            // 抛出一个线程
            new Thread(UpdateVisitTimeTask).Start(new Tuple<BaseUserLogonEntity, string, bool>(userLogonEntity, openId, createOpenId));

            return openId;
        }
        #endregion

        #region public string UpdateVisitTime(string userId) 更新访问当前访问状态
        /// <summary>
        /// 更新访问当前访问状态
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns>OpenId</returns>
        public string UpdateVisitTime(string userId)
        {
            var entity = GetEntityByUserId(userId);
            return UpdateVisitTime(entity);
        }
        #endregion

        #endregion

        #region 退出登录

        #region public bool SignOut(string openId, string systemCode = "Base", string ipAddress = null, string macAddress = null) 用户退出

        /// <summary>
        /// 用户退出
        /// </summary>
        /// <param name="openId">信令</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="macAddress">MAC地址</param>
        /// <returns>影响行数</returns>
        public bool SignOut(string openId, string systemCode = "Base", string ipAddress = null, string macAddress = null)
        {
            var result = 0;

            // 应该进行一次日志记录
            // 从缓存读取、效率高
            if (!string.IsNullOrWhiteSpace(openId))
            {
                var userEntity = BaseUserManager.GetEntityByOpenIdByCache(openId);
                if (userEntity != null && userEntity.Id > 0)
                {
                    var ipAddressName = string.Empty;
                    if (!string.IsNullOrEmpty(ipAddress))
                    {
                        ipAddressName = IpUtil.GetInstance().FindName(ipAddress);
                    }

                    BaseLogonLogManager.AddLog(systemCode, userEntity, ipAddress, ipAddressName, macAddress, Status.SignOut.ToDescription(), 0, 1);

                    // 是否更新访问日期信息
                    if (!BaseSystemInfo.UpdateVisit)
                    {
                        return result > 0;
                    }
                    // 最后一次登录时间
                    var sb = PoolUtil.StringBuilder.Get();
                    sb.Append("UPDATE " + BaseUserLogonEntity.CurrentTableName + " SET " + BaseUserLogonEntity.FieldPreviousVisitTime + " = " + BaseUserLogonEntity.FieldLastVisitTime);
                    //Troy.Cui 2020-02-29用户退出时也强制OpenId重新生成，和登录时一样强制生成OpenId
                    sb.Append(" ," + BaseUserLogonEntity.FieldOpenId + " = '" + Guid.NewGuid().ToString("N") + "'");
                    sb.Append(" ," + BaseUserLogonEntity.FieldOpenIdTimeoutTime + " = GETDATE()");
                    sb.Append(" ," + BaseUserLogonEntity.FieldUserOnline + " = 0");
                    sb.Append(" ," + BaseUserLogonEntity.FieldLastVisitTime + " = GETDATE()");
                    sb.Append(" WHERE " + BaseUserLogonEntity.FieldUserId + " = " + DbHelper.GetParameter(BaseUserEntity.FieldId));

                    var dbParameters = new List<IDbDataParameter>
                    {
                        DbHelper.MakeParameter(BaseUserEntity.FieldId, userEntity.Id)
                    };
                    result = ExecuteNonQuery(sb.Return(), dbParameters.ToArray());
                }
            }

            return result > 0;
        }
        #endregion

        #endregion
    }
}