//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;
    using System;

    /// <summary>
    /// BaseUserLogonManager
    /// 退出系统管理
    /// 
    /// 修改记录
    /// 
    ///		2016.02.17 版本：2.1 JiRiGaLa 优化退出时代码逻辑，减少执行没必要的数据库连接。
    ///		2015.12.14 版本：2.0 JiRiGaLa 检查代码质量。
    ///		2015.02.02 版本：1.0 JiRiGaLa 分离代码。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.02.17</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserLogonManager : BaseManager
    {
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
                    var sql = "UPDATE " + BaseUserLogonEntity.CurrentTableName + " SET " + BaseUserLogonEntity.FieldPreviousVisitTime + " = " + BaseUserLogonEntity.FieldLastVisitTime;
                    //Troy.Cui 2020-02-29用户退出时也强制OpenId重新生成，和登录时一样强制生成OpenId
                    sql += " , " + BaseUserLogonEntity.FieldOpenId + " = '" + Guid.NewGuid().ToString("N") + "'";
                    sql += ", " + BaseUserLogonEntity.FieldOpenIdTimeoutTime + " = " + DbHelper.GetDbNow();
                    sql += " , " + BaseUserLogonEntity.FieldUserOnline + " = 0 "
                              + " , " + BaseUserLogonEntity.FieldLastVisitTime + " = " + DbHelper.GetDbNow();
                    sql += "  WHERE " + BaseUserLogonEntity.FieldUserId + " = " + DbHelper.GetParameter(BaseUserEntity.FieldId);

                    var dbParameters = new List<IDbDataParameter>
                    {
                        DbHelper.MakeParameter(BaseUserEntity.FieldId, userEntity.Id)
                    };
                    result = ExecuteNonQuery(sql, dbParameters.ToArray());
                }
            }

            return result > 0;
        }
        #endregion
    }
}