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
        /// <summary>
        /// 更新访问数据
        /// </summary>
        /// <param name="param"></param>
        public void UpdateVisitDateTask(object param)
        {
            if (param is Tuple<BaseUserLogonEntity, string, bool> tuple)
            {
                var userLogonEntity = tuple.Item1;
                var openId = tuple.Item2;
                var createOpenId = tuple.Item3;
                UpdateVisitDateTask(userLogonEntity, openId, createOpenId);
            }
        }

        // public async Task 
        /// <summary>
        /// 更新访问数据
        /// </summary>
        /// <param name="userLogonEntity"></param>
        /// <param name="openId"></param>
        /// <param name="createOpenId"></param>
        public void UpdateVisitDateTask(BaseUserLogonEntity userLogonEntity, string openId, bool createOpenId = true)
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
                    if (userLogonEntity.PreviousVisit.HasValue || userLogonEntity.FirstVisit.HasValue)
                    {
                        var ts = new TimeSpan();
                        if (userLogonEntity.LastVisit.HasValue)
                        {
                            ts = DateTime.Now.Subtract((DateTime)userLogonEntity.LastVisit);
                            mobileNeedValiated = (ts.TotalDays > 7);
                        }
                        else if (userLogonEntity.FirstVisit.HasValue)
                        {
                            ts = DateTime.Now.Subtract((DateTime)userLogonEntity.FirstVisit);
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
                                DbHelper.MakeParameter(BaseUserContactEntity.FieldId, userLogonEntity.Id),
                                DbHelper.MakeParameter(BaseUserContactEntity.FieldMobileValiated, 1)
                            };

                            errorMark = 10;
                            dbHelper.ExecuteNonQuery(sql, dbParameters.ToArray());
                        }
                    }

                    if (BaseSystemInfo.UpdateVisit)
                    {
                        // 第一次登录时间
                        if (userLogonEntity.FirstVisit == null)
                        {
                            sql = "UPDATE " + CurrentTableName
                                        + " SET " + BaseUserLogonEntity.FieldPasswordErrorCount + " = 0 "
                                        + ", " + BaseUserLogonEntity.FieldUserOnline + " = 1 "
                                        + ", " + BaseUserLogonEntity.FieldFirstVisit + " = " + dbHelper.GetDbNow()
                                        + ", " + BaseUserLogonEntity.FieldLogonCount + " = 1 "
                                        + ", " + BaseUserLogonEntity.FieldSystemCode + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldSystemCode)
                                        + ", " + BaseUserLogonEntity.FieldIpAddress + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldIpAddress)
                                        + ", " + BaseUserLogonEntity.FieldIpAddressName + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldIpAddressName)
                                        + ", " + BaseUserLogonEntity.FieldMacAddress + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldMacAddress)
                                        + ", " + BaseUserLogonEntity.FieldComputerName + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldComputerName);

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
                                sql += ", " + BaseUserLogonEntity.FieldOpenId + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldOpenId);
                                sql += ", " + BaseUserLogonEntity.FieldOpenIdTimeout + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldOpenIdTimeout);
                                dbParameters.Add(dbHelper.MakeParameter(BaseUserLogonEntity.FieldOpenId, openId));
                                dbParameters.Add(dbHelper.MakeParameter(BaseUserLogonEntity.FieldOpenIdTimeout, openIdTimeout));
                            }

                            sql += "  WHERE " + BaseUserLogonEntity.FieldId + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldId)
                                        + "      AND " + BaseUserLogonEntity.FieldFirstVisit + " IS NULL";
                            dbParameters.Add(dbHelper.MakeParameter(BaseUserLogonEntity.FieldId, userLogonEntity.Id));

                            errorMark = 20;
                            dbHelper.ExecuteNonQuery(sql, dbParameters.ToArray());
                        }
                        else
                        {
                            // 最后一次登录时间
                            sql = "UPDATE " + CurrentTableName
                                        + " SET " + BaseUserLogonEntity.FieldPasswordErrorCount + " = 0 "
                                        + ", " + BaseUserLogonEntity.FieldPreviousVisit + " = " + BaseUserLogonEntity.FieldLastVisit
                                        + ", " + BaseUserLogonEntity.FieldUserOnline + " = 1 "
                                        + ", " + BaseUserLogonEntity.FieldLastVisit + " = " + dbHelper.GetDbNow()
                                        + ", " + BaseUserLogonEntity.FieldLogonCount + " = " + BaseUserLogonEntity.FieldLogonCount + " + 1 "
                                        + ", " + BaseUserLogonEntity.FieldSystemCode + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldSystemCode)
                                        + ", " + BaseUserLogonEntity.FieldIpAddress + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldIpAddress)
                                        + ", " + BaseUserLogonEntity.FieldIpAddressName + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldIpAddressName)
                                        + ", " + BaseUserLogonEntity.FieldMacAddress + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldMacAddress)
                                        + ", " + BaseUserLogonEntity.FieldComputerName + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldComputerName);

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
                                sql += ", " + BaseUserLogonEntity.FieldOpenId + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldOpenId);
                                sql += ", " + BaseUserLogonEntity.FieldOpenIdTimeout + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldOpenIdTimeout);
                                dbParameters.Add(dbHelper.MakeParameter(BaseUserLogonEntity.FieldOpenId, openId));
                                dbParameters.Add(dbHelper.MakeParameter(BaseUserLogonEntity.FieldOpenIdTimeout, openIdTimeout));
                            }

                            sql += "  WHERE " + BaseUserLogonEntity.FieldId + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldId);
                            dbParameters.Add(dbHelper.MakeParameter(BaseUserLogonEntity.FieldId, userLogonEntity.Id));

                            errorMark = 30;
                            dbHelper.ExecuteNonQuery(sql, dbParameters.ToArray());
                        }
                    }
                    else
                    {
                        sql = "UPDATE " + CurrentTableName
                                     + " SET  " + BaseUserLogonEntity.FieldPasswordErrorCount + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldPasswordErrorCount)
                                     + ", " + BaseUserLogonEntity.FieldSystemCode + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldSystemCode);
                        dbParameters = new List<IDbDataParameter>
                        {
                            dbHelper.MakeParameter(BaseUserLogonEntity.FieldPasswordErrorCount, 0),
                            dbHelper.MakeParameter(BaseUserLogonEntity.FieldSystemCode, userLogonEntity.SystemCode)
                        };

                        if (createOpenId)
                        {
                            sql += ", " + BaseUserLogonEntity.FieldOpenId + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldOpenId);
                            sql += ", " + BaseUserLogonEntity.FieldOpenIdTimeout + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldOpenIdTimeout);
                            dbParameters.Add(dbHelper.MakeParameter(BaseUserLogonEntity.FieldOpenId, openId));
                            dbParameters.Add(dbHelper.MakeParameter(BaseUserLogonEntity.FieldOpenIdTimeout, openIdTimeout));
                        }

                        sql += " WHERE " + BaseUserLogonEntity.FieldId + " = " + dbHelper.GetParameter(BaseUserLogonEntity.FieldId);
                        // sql += " AND " + BaseUserEntity.FieldOpenId + " IS NULL ";
                        dbParameters.Add(dbHelper.MakeParameter(BaseUserLogonEntity.FieldId, userLogonEntity.Id));

                        errorMark = 40;
                        dbHelper.ExecuteNonQuery(sql, dbParameters.ToArray());

                    }
                }
            }
            catch (Exception ex)
            {
                var writeMessage = "BaseUserLogonManager.UpdateVisitDateTask:发生时间:" + DateTime.Now
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
        /// <param name="userLogonEntity"></param>
        /// <param name="createOpenId"></param>
        /// <returns></returns>
        public string UpdateVisitDate(BaseUserLogonEntity userLogonEntity, bool createOpenId = true)
        {
            var openId = string.IsNullOrEmpty(userLogonEntity?.OpenId)? Guid.NewGuid().ToString("N") : userLogonEntity?.OpenId;

            //Troy.Cui 2020-02-29 强制每次都自动生成，但对于可并发用户，OpenId过期了才更新一下OpenId
            //Troy.Cui 并发用户需要检测下OpenId过期时间 2020-06-17
            if (createOpenId && userLogonEntity.MultiUserLogin == 1 && userLogonEntity.OpenIdTimeout.HasValue && !string.IsNullOrEmpty(userLogonEntity.OpenId))
            {
                //var timeSpan = DateTime.Now - userLogonEntity.OpenIdTimeout.Value;
                var timeSpan = userLogonEntity.OpenIdTimeout.Value - DateTime.Now;
                if ((timeSpan.TotalSeconds) < 0)
                {
                    openId = userLogonEntity.OpenId;
                }
            }
            // Troy.Cui 非并发用户强制每次生成 2020 - 06 - 17
            if (createOpenId && userLogonEntity.MultiUserLogin == 0)
            {
                openId = Guid.NewGuid().ToString("N");
            }

            // 抛出一个线程
            UpdateVisitDateTask(userLogonEntity, openId, createOpenId);
            // new Thread(UpdateVisitDateTask).Start(new Tuple<BaseUserLogonEntity, bool, string>(userLogonEntity, result));

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
            var userLogonEntity = GetEntity(userId);
            return UpdateVisitDate(userLogonEntity);
        }
        #endregion
    }
}