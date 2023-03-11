//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using System.Threading;
    using Util;

    /// <summary>
    /// BaseUserLogonManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///		2015.12.08 版本：1.0 JiRiGaLa	代码进行分离。
    ///     
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.12.08</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserLogonManager
    {

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

            var sb = Pool.StringBuilder.Get();
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
                                     + " SET " + BaseUserContactEntity.FieldMobileValidated + " = 0 "
                                     + " WHERE " + BaseUserContactEntity.FieldUserId + " = " + DbHelper.GetParameter(BaseUserContactEntity.FieldUserId)
                                     + " AND " + BaseUserContactEntity.FieldMobileValidated + " = " + DbHelper.GetParameter(BaseUserContactEntity.FieldMobileValidated));

                            dbParameters = new List<IDbDataParameter>
                            {
                                DbHelper.MakeParameter(BaseUserContactEntity.FieldUserId, userLogonEntity.UserId),
                                DbHelper.MakeParameter(BaseUserContactEntity.FieldMobileValidated, 1)
                            };

                            errorMark = 10;
                            dbHelper.ExecuteNonQuery(sb.ToString(), dbParameters.ToArray());
                        }
                    }

                    if (BaseSystemInfo.UpdateVisit)
                    {
                        // 第一次登录时间
                        if (userLogonEntity.FirstVisitTime == null)
                        {
                            sb.Clear();
                            sb.Append("UPDATE " + CurrentTableName
                                        + " SET " + BaseUserLogonEntity.FieldPasswordErrorCount + " = 0 "
                                        + ", " + BaseUserLogonEntity.FieldUserOnline + " = 1 "
                                        + ", " + BaseUserLogonEntity.FieldFirstVisitTime + " = " + dbHelper.GetDbNow()
                                        + ", " + BaseUserLogonEntity.FieldLogonCount + " = 1 "
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
                            dbHelper.ExecuteNonQuery(sb.ToString(), dbParameters.ToArray());
                        }
                        else
                        {
                            // 最后一次登录时间
                            sb.Clear();
                            sb.Append("UPDATE " + CurrentTableName
                                        + " SET " + BaseUserLogonEntity.FieldPasswordErrorCount + " = 0 "
                                        + ", " + BaseUserLogonEntity.FieldPreviousVisitTime + " = " + BaseUserLogonEntity.FieldLastVisitTime
                                        + ", " + BaseUserLogonEntity.FieldUserOnline + " = 1 "
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

                            sb.Append("  WHERE " + BaseUserLogonEntity.FieldUserId + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldUserId));
                            dbParameters.Add(dbHelper.MakeParameter(BaseUserLogonEntity.FieldUserId, userLogonEntity.UserId));

                            errorMark = 30;
                            dbHelper.ExecuteNonQuery(sb.ToString(), dbParameters.ToArray());
                        }
                    }
                    else
                    {
                        sb.Clear();
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
                        dbHelper.ExecuteNonQuery(sb.Put(), dbParameters.ToArray());

                    }
                }
            }
            catch (Exception ex)
            {
                var writeMessage = "BaseUserLogonManager.UpdateVisitTimeTask:发生时间:" + DateTime.Now
                    + Environment.NewLine + "errorMark = " + errorMark
                    + Environment.NewLine + "UserInfo:" + UserInfo.Serialize()
                    + Environment.NewLine + "Message:" + ex.Message
                    + Environment.NewLine + "Source:" + ex.Source
                    + Environment.NewLine + "StackTrace:" + ex.StackTrace
                    + Environment.NewLine + "TargetSite:" + ex.TargetSite
                    + Environment.NewLine;

                LogUtil.WriteLog(writeMessage, "Exception");
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
    }
}