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
    /// BaseUserLogOnManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///		2015.12.08 版本：1.0 JiRiGaLa	代码进行分离。
    ///     
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.12.08</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserLogOnManager
    {
        /// <summary>
        /// 更新访问数据
        /// </summary>
        /// <param name="param"></param>
        public void UpdateVisitDateTask(object param)
        {
            if (param is Tuple<BaseUserLogOnEntity, string, bool> tuple)
            {
                var userLogOnEntity = tuple.Item1;
                var openId = tuple.Item2;
                var createOpenId = tuple.Item3;
                UpdateVisitDateTask(userLogOnEntity, openId, createOpenId);
            }
        }

        // public async Task 
        /// <summary>
        /// 更新访问数据
        /// </summary>
        /// <param name="userLogOnEntity"></param>
        /// <param name="openId"></param>
        /// <param name="createOpenId"></param>
        public void UpdateVisitDateTask(BaseUserLogOnEntity userLogOnEntity, string openId, bool createOpenId = true)
        {
            var errorMark = 0;

            var sql = string.Empty;
            //默认给OpenId 8个小时有效期，每次更新在线状态的时候，再刷新一下OpenId的有效期，Troy.Cui 2020-02-29
            DateTime? openIdTimeout = DateTime.Now.AddHours(8);
            try
            {
                using (var dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection))
                {
                    // 是否更新访问日期信息
                    List<IDbDataParameter> dbParameters = null;
                    // 若有一周没登录了，需要重新进行手机验证
                    var mobileNeedValiated = false;
                    if (userLogOnEntity.PreviousVisit.HasValue || userLogOnEntity.FirstVisit.HasValue)
                    {
                        var ts = new TimeSpan();
                        if (userLogOnEntity.LastVisit.HasValue)
                        {
                            ts = DateTime.Now.Subtract((DateTime)userLogOnEntity.LastVisit);
                            mobileNeedValiated = (ts.TotalDays > 7);
                        }
                        else if (userLogOnEntity.FirstVisit.HasValue)
                        {
                            ts = DateTime.Now.Subtract((DateTime)userLogOnEntity.FirstVisit);
                            mobileNeedValiated = (ts.TotalDays > 7);
                        }
                        if (mobileNeedValiated)
                        {
                            sql = "UPDATE " + BaseUserContactEntity.TableName
                                     + " SET " + BaseUserContactEntity.FieldMobileValiated + " = 0 "
                                     + " WHERE " + BaseUserContactEntity.FieldId + " = " + DbHelper.GetParameter(BaseUserContactEntity.FieldId)
                                     + " AND " + BaseUserContactEntity.FieldMobileValiated + " = " + DbHelper.GetParameter(BaseUserContactEntity.FieldMobileValiated);

                            dbParameters = new List<IDbDataParameter>
                            {
                                DbHelper.MakeParameter(BaseUserContactEntity.FieldId, userLogOnEntity.Id),
                                DbHelper.MakeParameter(BaseUserContactEntity.FieldMobileValiated, 1)
                            };

                            errorMark = 10;
                            dbHelper.ExecuteNonQuery(sql, dbParameters.ToArray());
                        }
                    }

                    if (BaseSystemInfo.UpdateVisit)
                    {
                        // 第一次登录时间
                        if (userLogOnEntity.FirstVisit == null)
                        {
                            sql = "UPDATE " + CurrentTableName
                                        + " SET " + BaseUserLogOnEntity.FieldPasswordErrorCount + " = 0 "
                                        + ", " + BaseUserLogOnEntity.FieldUserOnLine + " = 1 "
                                        + ", " + BaseUserLogOnEntity.FieldFirstVisit + " = " + dbHelper.GetDbNow()
                                        + ", " + BaseUserLogOnEntity.FieldLogOnCount + " = 1 "
                                        + ", " + BaseUserLogOnEntity.FieldSystemCode + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldSystemCode)
                                        + ", " + BaseUserLogOnEntity.FieldIpAddress + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldIpAddress)
                                        + ", " + BaseUserLogOnEntity.FieldIpAddressName + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldIpAddressName)
                                        + ", " + BaseUserLogOnEntity.FieldMacAddress + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldMacAddress)
                                        + ", " + BaseUserLogOnEntity.FieldComputerName + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldComputerName);

                            dbParameters = new List<IDbDataParameter>
                            {
                                dbHelper.MakeParameter(BaseUserLogOnEntity.FieldSystemCode, userLogOnEntity.SystemCode),
                                dbHelper.MakeParameter(BaseUserLogOnEntity.FieldIpAddress, userLogOnEntity.IpAddress),
                                dbHelper.MakeParameter(BaseUserLogOnEntity.FieldIpAddressName, userLogOnEntity.IpAddressName),
                                dbHelper.MakeParameter(BaseUserLogOnEntity.FieldMacAddress, userLogOnEntity.MacAddress),
                                dbHelper.MakeParameter(BaseUserLogOnEntity.FieldComputerName, userLogOnEntity.ComputerName)
                            };

                            if (createOpenId)
                            {
                                sql += ", " + BaseUserLogOnEntity.FieldOpenId + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldOpenId);
                                sql += ", " + BaseUserLogOnEntity.FieldOpenIdTimeout + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldOpenIdTimeout);
                                dbParameters.Add(dbHelper.MakeParameter(BaseUserLogOnEntity.FieldOpenId, openId));
                                dbParameters.Add(dbHelper.MakeParameter(BaseUserLogOnEntity.FieldOpenIdTimeout, openIdTimeout));
                            }

                            sql += "  WHERE " + BaseUserLogOnEntity.FieldId + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldId)
                                        + "      AND " + BaseUserLogOnEntity.FieldFirstVisit + " IS NULL";
                            dbParameters.Add(dbHelper.MakeParameter(BaseUserLogOnEntity.FieldId, userLogOnEntity.Id));

                            errorMark = 20;
                            dbHelper.ExecuteNonQuery(sql, dbParameters.ToArray());
                        }
                        else
                        {
                            // 最后一次登录时间
                            sql = "UPDATE " + CurrentTableName
                                        + " SET " + BaseUserLogOnEntity.FieldPasswordErrorCount + " = 0 "
                                        + ", " + BaseUserLogOnEntity.FieldPreviousVisit + " = " + BaseUserLogOnEntity.FieldLastVisit
                                        + ", " + BaseUserLogOnEntity.FieldUserOnLine + " = 1 "
                                        + ", " + BaseUserLogOnEntity.FieldLastVisit + " = " + dbHelper.GetDbNow()
                                        + ", " + BaseUserLogOnEntity.FieldLogOnCount + " = " + BaseUserLogOnEntity.FieldLogOnCount + " + 1 "
                                        + ", " + BaseUserLogOnEntity.FieldSystemCode + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldSystemCode)
                                        + ", " + BaseUserLogOnEntity.FieldIpAddress + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldIpAddress)
                                        + ", " + BaseUserLogOnEntity.FieldIpAddressName + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldIpAddressName)
                                        + ", " + BaseUserLogOnEntity.FieldMacAddress + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldMacAddress)
                                        + ", " + BaseUserLogOnEntity.FieldComputerName + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldComputerName);

                            dbParameters = new List<IDbDataParameter>
                            {
                                dbHelper.MakeParameter(BaseUserLogOnEntity.FieldSystemCode, userLogOnEntity.SystemCode),
                                dbHelper.MakeParameter(BaseUserLogOnEntity.FieldIpAddress, userLogOnEntity.IpAddress),
                                dbHelper.MakeParameter(BaseUserLogOnEntity.FieldIpAddressName, userLogOnEntity.IpAddressName),
                                dbHelper.MakeParameter(BaseUserLogOnEntity.FieldMacAddress, userLogOnEntity.MacAddress),
                                dbHelper.MakeParameter(BaseUserLogOnEntity.FieldComputerName, userLogOnEntity.ComputerName)
                            };

                            if (createOpenId)
                            {
                                sql += ", " + BaseUserLogOnEntity.FieldOpenId + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldOpenId);
                                sql += ", " + BaseUserLogOnEntity.FieldOpenIdTimeout + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldOpenIdTimeout);
                                dbParameters.Add(dbHelper.MakeParameter(BaseUserLogOnEntity.FieldOpenId, openId));
                                dbParameters.Add(dbHelper.MakeParameter(BaseUserLogOnEntity.FieldOpenIdTimeout, openIdTimeout));
                            }

                            sql += "  WHERE " + BaseUserLogOnEntity.FieldId + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldId);
                            dbParameters.Add(dbHelper.MakeParameter(BaseUserLogOnEntity.FieldId, userLogOnEntity.Id));

                            errorMark = 30;
                            dbHelper.ExecuteNonQuery(sql, dbParameters.ToArray());
                        }
                    }
                    else
                    {
                        sql = "UPDATE " + CurrentTableName
                                     + " SET  " + BaseUserLogOnEntity.FieldPasswordErrorCount + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldPasswordErrorCount)
                                     + ", " + BaseUserLogOnEntity.FieldSystemCode + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldSystemCode);
                        dbParameters = new List<IDbDataParameter>
                        {
                            dbHelper.MakeParameter(BaseUserLogOnEntity.FieldPasswordErrorCount, 0),
                            dbHelper.MakeParameter(BaseUserLogOnEntity.FieldSystemCode, userLogOnEntity.SystemCode)
                        };

                        if (createOpenId)
                        {
                            sql += ", " + BaseUserLogOnEntity.FieldOpenId + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldOpenId);
                            sql += ", " + BaseUserLogOnEntity.FieldOpenIdTimeout + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldOpenIdTimeout);
                            dbParameters.Add(dbHelper.MakeParameter(BaseUserLogOnEntity.FieldOpenId, openId));
                            dbParameters.Add(dbHelper.MakeParameter(BaseUserLogOnEntity.FieldOpenIdTimeout, openIdTimeout));
                        }

                        sql += " WHERE " + BaseUserLogOnEntity.FieldId + " = " + dbHelper.GetParameter(BaseUserLogOnEntity.FieldId);
                        // sql += " AND " + BaseUserEntity.FieldOpenId + " IS NULL ";
                        dbParameters.Add(dbHelper.MakeParameter(BaseUserLogOnEntity.FieldId, userLogOnEntity.Id));

                        errorMark = 40;
                        dbHelper.ExecuteNonQuery(sql, dbParameters.ToArray());

                    }
                }
            }
            catch (Exception ex)
            {
                var writeMessage = "BaseUserLogOnManager.UpdateVisitDateTask:发生时间:" + DateTime.Now
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

        /// <summary>
        /// 更新访问数据
        /// </summary>
        /// <param name="userLogOnEntity"></param>
        /// <param name="createOpenId"></param>
        /// <returns></returns>
        public string UpdateVisitDate(BaseUserLogOnEntity userLogOnEntity, bool createOpenId = true)
        {
            var openId = string.IsNullOrEmpty(userLogOnEntity?.OpenId)? Guid.NewGuid().ToString("N") : userLogOnEntity?.OpenId;

            //Troy.Cui 2020-02-29 强制每次都自动生成，但对于可并发用户，OpenId过期了才更新一下OpenId
            //Troy.Cui 并发用户需要检测下OpenId过期时间 2020-06-17
            if (createOpenId && userLogOnEntity.MultiUserLogin == 1 && userLogOnEntity.OpenIdTimeout.HasValue && !string.IsNullOrEmpty(userLogOnEntity.OpenId))
            {
                //var timeSpan = DateTime.Now - userLogOnEntity.OpenIdTimeout.Value;
                var timeSpan = userLogOnEntity.OpenIdTimeout.Value - DateTime.Now;
                if ((timeSpan.TotalSeconds) < 0)
                {
                    openId = userLogOnEntity.OpenId;
                }
            }
            // Troy.Cui 非并发用户强制每次生成 2020 - 06 - 17
            if (createOpenId && userLogOnEntity.MultiUserLogin == 0)
            {
                openId = Guid.NewGuid().ToString("N");
            }

            // 抛出一个线程
            UpdateVisitDateTask(userLogOnEntity, openId, createOpenId);
            // new Thread(UpdateVisitDateTask).Start(new Tuple<BaseUserLogOnEntity, bool, string>(userLogOnEntity, result));

            return openId;
        }

        #region public string UpdateVisitDate(string userId) 更新访问当前访问状态
        /// <summary>
        /// 更新访问当前访问状态
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns>OpenId</returns>
        public string UpdateVisitDate(string userId)
        {
            var userLogOnEntity = GetEntity(userId);
            return UpdateVisitDate(userLogOnEntity);
        }
        #endregion
    }
}