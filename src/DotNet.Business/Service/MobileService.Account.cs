//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;

namespace DotNet.Business
{
    using IService;
    using Util;

    /// <summary>
    /// MobileService
    /// 消息服务
    /// 
    /// 修改记录
    /// 
    ///		2015.06.10 版本：1.1 JiRiGaLa 调整充值逻辑。
    ///		2014.03.20 版本：1.0 JiRiGaLa 创建。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.06.10</date>
    /// </author> 
    /// </summary>
    public partial class MobileService : IMobileService
    {
        /// <summary>
        /// 手机短信充值
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="applicationCode">应用编号</param>
        /// <param name="accountCode">帐户编号</param>
        /// <param name="accountName">帐户名</param>
        /// <param name="applicationName">应用名称（充值的用户名字）</param>
        /// <param name="messageCount">充值短信个数</param>
        /// <returns>影响行数</returns>
        public int Recharge(BaseUserInfo userInfo, string applicationCode, string applicationName, string accountCode, string accountName, int messageCount)
        {
            var result = 0;

            var connectionString = ConfigurationHelper.AppSettings("OpenMasDbConnection", BaseSystemInfo.EncryptDbConnection);
            if (!string.IsNullOrEmpty(connectionString))
            {
                using (var dbHelper = DbHelperFactory.GetHelper(CurrentDbType.SqlServer, connectionString))
                {
                    // 1: 先进对拆分的帐户行充值操作？
                    var commandText = @"UPDATE SMSUserAccount 
                                          SET RemainingNumber = RemainingNumber + " + dbHelper.GetParameter("MessageCount")
                                        + "       , ModifiedOn = GETDATE() "
                                        + "       , ModifiedUserId = '" + userInfo.Id + "'"
                                        + "       , ModifiedBy = '" + userInfo.RealName + "'"
                                        + " WHERE ApplicationCode = " + dbHelper.GetParameter("ApplicationCode")
                                        + "       AND AccountCode = " + dbHelper.GetParameter("AccountCode");
                    result = dbHelper.ExecuteNonQuery(commandText
                        , new IDbDataParameter[] { dbHelper.MakeParameter("MessageCount", messageCount)
                    , dbHelper.MakeParameter("ApplicationCode", applicationCode)
                    , dbHelper.MakeParameter("AccountCode", accountCode)});
                    if (result == 0)
                    {
                        // 2: 若没有拆分帐户、就给主帐户充值、没拆分的，有效的帐户
                        commandText = @"UPDATE SMSUserAccount 
                                          SET RemainingNumber = RemainingNumber + " + dbHelper.GetParameter("MessageCount")
                                            + "       , ModifiedOn = GETDATE() "
                                            + "       , ModifiedUserId = '" + userInfo.Id + "'"
                                            + "       , ModifiedBy = '" + userInfo.RealName + "'"
                                            + " WHERE SplitAccount = 0 AND Enabled = 1 AND ApplicationCode = " + dbHelper.GetParameter("ApplicationCode")
                                            + "       AND AccountCode = " + dbHelper.GetParameter("AccountCode");
                        result = dbHelper.ExecuteNonQuery(commandText
                            , new IDbDataParameter[] { dbHelper.MakeParameter("MessageCount", messageCount)
                    , dbHelper.MakeParameter("ApplicationCode", applicationCode)
                    , dbHelper.MakeParameter("AccountCode", applicationCode)});
                        if (result == 0 && ValidateUtil.IsInt(applicationCode))
                        {
                            // 2: 若没记录，就进行插入操作。
                            //BaseOrganizeManager organizeManager = new BaseOrganizeManager();
                            //string id = organizeManager.GetPermissionIdByCode(accountCode);
                            //if (!string.IsNullOrEmpty(id))
                            //{
                            //    BaseOrganizeEntity organizeEntity = organizeManager.GetObject(id);
                            //    if (organizeEntity != null)
                            //    {
                            //        string accountName = organizeEntity.FullName;

                            commandText = @"INSERT INTO SMSUserAccount(ApplicationCode, ApplicationName, AccountCode, AccountName, RemainingNumber, CreateOn, CreateUserId, CreateBy) 
                                        VALUES(" + dbHelper.GetParameter("ApplicationCode") + ","
                                                         + dbHelper.GetParameter("ApplicationName") + ","
                                                         + dbHelper.GetParameter("AccountCode") + ","
                                                         + dbHelper.GetParameter("AccountName") + ","
                                                         + dbHelper.GetParameter("MessageCount") + ","
                                                         + "GETDATE(),"
                                                         + "'" + userInfo.Id + "',"
                                                         + "'" + userInfo.RealName + "')";
                            result = dbHelper.ExecuteNonQuery(commandText,
                                new IDbDataParameter[] { dbHelper.MakeParameter("ApplicationCode", applicationCode)
                                                    , dbHelper.MakeParameter("ApplicationName", applicationName)
                                                    , dbHelper.MakeParameter("AccountCode", accountCode)
                                                    , dbHelper.MakeParameter("AccountName", accountName)
                                                    , dbHelper.MakeParameter("MessageCount", messageCount) });
                            //    }
                            //}
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取剩余数量
        /// </summary>
        /// <param name="userEntity"></param>
        /// <param name="applicationCode"></param>
        /// <param name="accountCode"></param>
        /// <returns></returns>
        public int GetRemainingCount(Model.BaseUserEntity userEntity, string applicationCode, string accountCode)
        {
            if (string.IsNullOrEmpty(applicationCode))
            {
                if (userEntity != null)
                {
                    applicationCode = BaseOrganizeManager.GetCodeByCache(userEntity.CompanyId);
                }
            }
            if (string.IsNullOrEmpty(accountCode))
            {
                if (userEntity != null)
                {
                    accountCode = userEntity.Code;
                }
            }

            return GetRemainingCount(applicationCode, accountCode);
        }

        /// <summary>
        /// 获取剩余数量
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="applicationCode"></param>
        /// <param name="accountCode"></param>
        /// <returns></returns>
        public int GetRemainingCount(BaseUserInfo userInfo, string applicationCode, string accountCode)
        {
            if (string.IsNullOrEmpty(applicationCode))
            {
                if (userInfo != null)
                {
                    applicationCode = userInfo.CompanyCode;
                }
            }
            if (string.IsNullOrEmpty(accountCode))
            {
                if (userInfo != null)
                {
                    accountCode = userInfo.Code;
                }
            }

            return GetRemainingCount(applicationCode, accountCode);
        }

        /// <summary>
        /// 获取可发送短信的余额
        /// </summary>
        /// <param name="applicationCode">公司编号、应用编号</param>
        /// <param name="accountCode">用户工号、用户账户</param>
        /// <returns>短信余额</returns>
        public int GetRemainingCount(string applicationCode, string accountCode)
        {
            var result = 0;

            var connectionString = ConfigurationHelper.AppSettings("OpenMasDbConnection", BaseSystemInfo.EncryptDbConnection);
            if (!string.IsNullOrEmpty(connectionString))
            {
                using (var dbHelper = DbHelperFactory.GetHelper(CurrentDbType.SqlServer, connectionString))
                {
                    var commandText = @"SELECT RemainingNumber + Credit
             FROM SMSUserAccount
                                            WHERE ApplicationCode = " + dbHelper.GetParameter("ApplicationCode")
                                              + " AND AccountCode = " + dbHelper.GetParameter("AccountCode");
                    var remainingNumber = dbHelper.ExecuteScalar(commandText
                        , new IDbDataParameter[]
                    {
                        dbHelper.MakeParameter("ApplicationCode", applicationCode)
                        , dbHelper.MakeParameter("AccountCode", accountCode)
                });
                    // 若是空的，那就找整个公司是否购买了短信？不分配短信也可以才对
                    if (remainingNumber == null)
                    {
                        commandText = "SELECT RemainingNumber + Credit "
                                   + " FROM SMSUserAccount "
                                   + "  WHERE SplitAccount = " + dbHelper.GetParameter("SplitAccount")
                                   + "        AND Enabled = " + dbHelper.GetParameter("Enabled")
                                   + "        AND ApplicationCode = " + dbHelper.GetParameter("ApplicationCode")
                                   + "        AND AccountCode = " + dbHelper.GetParameter("AccountCode");
                        remainingNumber = dbHelper.ExecuteScalar(commandText
                            , new IDbDataParameter[]
                        {
                            dbHelper.MakeParameter("SplitAccount", 0)
                            , dbHelper.MakeParameter("Enabled", 1)
                            , dbHelper.MakeParameter("ApplicationCode", applicationCode)
                            , dbHelper.MakeParameter("AccountCode", applicationCode)
                        });
                    }
                    if (remainingNumber != null)
                    {
                        result = int.Parse(remainingNumber.ToString());
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 扣除短信费用(主要账户发？分账户发？不拆分账户发？拆分账户发？)
        /// </summary>
        /// <param name="applicationCode">网点编号</param>
        /// <param name="accountCode">网点编号或者员工编号</param>
        /// <param name="messageCount">消息长度</param>
        /// <returns>影响行数</returns>
        public int Costing(string applicationCode, string accountCode, int messageCount)
        {
            var result = 0;

            // 这里需要帐户余额减掉
            var connectionString = ConfigurationHelper.AppSettings("OpenMasDbConnection", BaseSystemInfo.EncryptDbConnection);
            if (!string.IsNullOrEmpty(connectionString))
            {
                using (var dbHelper = DbHelperFactory.GetHelper(CurrentDbType.SqlServer, connectionString))
                {
                    var commandText = @"UPDATE SMSUserAccount SET RemainingNumber = RemainingNumber - " + messageCount + ", AllCount = AllCount + " + messageCount + " WHERE ApplicationCode = " + dbHelper.GetParameter("ApplicationCode")
                                              + " AND AccountCode = " + dbHelper.GetParameter("AccountCode");
                    result = dbHelper.ExecuteNonQuery(commandText, new[] { dbHelper.MakeParameter("ApplicationCode", applicationCode), dbHelper.MakeParameter("AccountCode", accountCode) });
                    if (result == 0)
                    {
                        result = dbHelper.ExecuteNonQuery(commandText, new[] { dbHelper.MakeParameter("ApplicationCode", applicationCode), dbHelper.MakeParameter("AccountCode", applicationCode) });
                    }
                    // 有没有扣费成功的情况发生、需要写日志
                    if (result == 0)
                    {
                        LogUtil.WriteException(new Exception(string.Format("Costing函数扣费失败 ApplicationCode：{0}  AccountCode：{1}", applicationCode, accountCode)));
                    }
                }
            }

            return result;
        }
    }
}