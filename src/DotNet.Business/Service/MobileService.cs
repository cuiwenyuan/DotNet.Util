//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

using System.Text;

namespace DotNet.Business
{
    using IService;
    using Model;
    using Util;

    /// <summary>
    /// MobileService
    /// 消息服务
    /// 
    /// 修改记录
    /// 
    ///		2013.12.25 版本：2.0 JiRiGaLa 改进注入漏洞，增强安全性。
    ///		2013.11.22 版本：1.0 JiRiGaLa 创建。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2013.12.25</date>
    /// </author> 
    /// </summary>


    public partial class MobileService : IMobileService
    {
        /// <summary>
        /// 手机号码是否存在？
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="mobile">手机号码</param>
        /// <returns>存在</returns>
        public bool Exists(BaseUserInfo userInfo, string mobile)
        {
            var result = false;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserContactEntity.FieldMobile, mobile)
                };
                var manager = new BaseUserContactManager(dbHelper, userInfo, BaseUserContactEntity.TableName);
                result = manager.Exists(parameters, userInfo.Id);
            });
            return result;
        }


        /// <summary>
        /// 短信是否发送成功？
        /// 以八小时内是否有成功发送的短信为准
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="mobile">手机号码</param>
        /// <returns>是否成功到达</returns>
        public bool SentSuccessfully(BaseUserInfo userInfo, string mobile)
        {
            var result = false;

            var connectionString = ConfigurationUtil.AppSettings("OpenMasDbConnection", BaseSystemInfo.EncryptDbConnection);
            if (!string.IsNullOrEmpty(connectionString))
            {
                using (var dbHelper = DbHelperFactory.GetHelper(CurrentDbType.SqlServer, connectionString))
                {
                    /*
                    string commandText = @"SELECT COUNT(*)
             FROM SMSSentLog
                                            WHERE GatawayStatus = 'DELIVRD' 
                                                  AND DATEDIFF(Hour, CreateTime, GETDATE()) < 8
                                                  AND DestinationAddress = " + dbHelper.GetParameter("DestinationAddress");
                     */
                    var commandText = @"SELECT COUNT(*)
         FROM SMSSentLog
                                        WHERE DATEDIFF(Hour, CreateTime, GETDATE()) < 8
                                              AND DestinationAddress = " + dbHelper.GetParameter("DestinationAddress");
                    var remainingNumber = dbHelper.ExecuteScalar(commandText, new IDbDataParameter[] { dbHelper.MakeParameter("DestinationAddress", mobile) });
                    if (remainingNumber != null)
                    {
                        result = int.Parse(remainingNumber.ToString()) > 0;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取发送验证码的数量
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public int GetSendVerificationCodeCount(string mobile)
        {
            var result = 0;
            var connectionString = ConfigurationUtil.AppSettings("OpenMasDbConnection", BaseSystemInfo.EncryptDbConnection);
            if (!string.IsNullOrEmpty(connectionString))
            {
                using (var dbHelper = DbHelperFactory.GetHelper(CurrentDbType.SqlServer, connectionString))
                {
                    var commandText = @"SELECT COUNT(*) 
         FROM SmsSentLog
                                        WHERE UserId = 'VerificationCode' 
                                              AND DestinationAddress = " + dbHelper.GetParameter("Mobile")
                                              + " AND CreateTime > DATEADD(day, -1, GETDATE()) AND CreateTime < GETDATE()";

                    result = int.Parse(dbHelper.ExecuteScalar(commandText, new IDbDataParameter[] { dbHelper.MakeParameter("Mobile", mobile) }).ToString());
                }
            }
            return result;
        }

        #region public bool GetVerificationCode(BaseUserInfo userInfo, string mobile, string system = "核心系统", string channel = "3") 发送手机验证码
        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="mobile">手机号码</param>
        /// <param name="system">系统</param>
        /// <param name="channel">渠道</param>
        /// <returns>验证码</returns>
        /// <returns>发送是否正常</returns>
        public bool GetVerificationCode(BaseUserInfo userInfo, string mobile, string system = "核心系统", string channel = "3")
        {
            // 应用编号
            ApplicationCode = "WangCaiSoft";
            // 短信发送账户编号
            AccountCode = "WangCaiSoft";
            // 应用密码
            Password = "WangCaiSoft";

            var result = false;
            if (string.IsNullOrEmpty(system))
            {
                system = "核心系统";
            }
            //TODO 需要增加一天只能收取几次验证码的限制，8个小时内最多只能发送3次验证码
            var sendVerificationCodeCount = GetSendVerificationCodeCount(mobile);
            if (sendVerificationCodeCount < 6)
            {
                // 产生随机验证码、数字的、六位长度
                var code = new Random().Next(100000, 999999);
                // 发送的手机短信
                var message = "您在" + system + "手机验证码为：" + code;
                var returnMsg = string.Empty;
                result = SendMobile(userInfo, "Base", "VerificationCode", mobile, message, code.ToString(), true, false, channel, out returnMsg) > 0;
                if (result && userInfo != null)
                {
                    var userLogOnManager = new BaseUserLogOnManager(userInfo);
                    userLogOnManager.SetProperty(userInfo.Id, new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldVerificationCode, code));
                }
            }
            return result;
        }
        #endregion

        #region public bool VerificationMobileCode(BaseUserInfo userInfo, string mobile, string code) 验证手机验证码
        /// <summary>
        /// 验证手机验证码
        /// 2015-11-10 吉日嘎拉 手机验证码确认的代码进行优化
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="mobile">手机号码</param>
        /// <param name="code">验证码</param>
        /// <returns>成功验证</returns>
        public bool ValidateVerificationCode(BaseUserInfo userInfo, string mobile, string code)
        {
            var result = false;

            if (string.IsNullOrEmpty(mobile))
            {
                return false;
            }
            if (string.IsNullOrEmpty(code))
            {
                return false;
            }

            var connectionString = ConfigurationUtil.AppSettings("OpenMasDbConnection", BaseSystemInfo.EncryptDbConnection);
            if (!string.IsNullOrEmpty(connectionString))
            {
                using (var dbHelper = DbHelperFactory.GetHelper(CurrentDbType.SqlServer, connectionString))
                {
                    var commandText = @"SELECT COUNT(*)
             FROM SMSSentLog
                                            WHERE DATEDIFF(Hour, CreateTime, GETDATE()) < 8
                                                  AND MessageCode = " + dbHelper.GetParameter("MessageCode")
                                              + " AND DestinationAddress = " + dbHelper.GetParameter("DestinationAddress");
                    var remainingNumber = dbHelper.ExecuteScalar(commandText
                        , new IDbDataParameter[] { dbHelper.MakeParameter("MessageCode", code)
                            , dbHelper.MakeParameter("DestinationAddress", mobile)});
                    if (remainingNumber != null)
                    {
                        result = int.Parse(remainingNumber.ToString()) > 0;
                    }
                }

                // 手机验证码通过审核了
                if (result && userInfo != null)
                {
                    BaseUserContactEntity userContactEntity = BaseUserContactManager.GetObjectByCache(userInfo.Id);

                    if (userContactEntity != null)
                    {
                        // 2016-02-13 吉日嘎拉 这里还需要进行缓存更新操作
                        userContactEntity.MobileValiated = 1;
                        userContactEntity.MobileVerificationDate = DateTime.Now;
                        new BaseUserContactManager().Update(userContactEntity);
                    }
                }
            }

            return result;
        }
        #endregion

        #region public int GetSendUserPasswordCount(string mobile) 获取发送密码次数
        /// <summary>
        /// 获取发送密码次数
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <returns>发送次数</returns>
        public int GetSendUserPasswordCount(string mobile)
        {
            var result = 0;
            var connectionString = ConfigurationUtil.AppSettings("OpenMasDbConnection", BaseSystemInfo.EncryptDbConnection);
            if (!string.IsNullOrEmpty(connectionString))
            {
                using (var dbHelper = DbHelperFactory.GetHelper(CurrentDbType.SqlServer, connectionString))
                {
                    var commandText = @"SELECT COUNT(*) 
         FROM SmsSentLog
                                        WHERE UserId = 'UserPassword' 
                                              AND DestinationAddress = " + dbHelper.GetParameter("Mobile")
                                              + " AND CreateTime > DATEADD(day, -1, GETDATE()) AND CreateTime < GETDATE()";

                    result = int.Parse(dbHelper.ExecuteScalar(commandText, new IDbDataParameter[] { dbHelper.MakeParameter("Mobile", mobile) }).ToString());
                }
            }
            return result;
        }
        #endregion

        #region public bool SendUserPassword(BaseUserInfo userInfo, string mobile, string userPassword) 发送手机验证码
        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="mobile">手机号码</param>
        /// <param name="userPassword">用户密码</param>
        /// <param name="channel">渠道</param>
        /// <returns>发送是否正常</returns>
        public bool SendUserPassword(BaseUserInfo userInfo, string mobile, string userPassword, string channel = "3")
        {
            // 应用编号
            ApplicationCode = "WangCaiSoft";
            // 短信发送账户编号
            AccountCode = "WangCaiSoft";
            // 应用密码
            Password = "WangCaiSoft";

            var result = false;
            var system = "核心系统";
            //TODO 需要增加一天只能收取几次验证码的限制，8个小时内最多只能发送3次验证码
            if (!string.IsNullOrEmpty(userPassword))
            {
                // 产生随机验证码、数字的、六位长度
                // 发送的手机短信
                var message = "您在" + system + "用户密码为：" + userPassword;
                var returnMsg = string.Empty;
                result = SendMobile(userInfo, "Base", "UserPassword", mobile, message, userPassword, true, false, channel, out returnMsg) > 0;
            }
            return result;
        }
        #endregion

        #region public int SendMobileMessage(BaseUserInfo userInfo, string mobiles, string message, bool hotline, bool confidentialInformation) 发送手机消息
        /// <summary>
        /// 发送手机消息
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="mobiles">手机号码</param>
        /// <param name="message">内容</param>
        /// <param name="hotline">客服热线</param>
        /// <param name="confidentialInformation">机密信息</param>
        /// <param name="channel">渠道</param>
        /// <returns>主键</returns>
        public int SendMobileMessage(BaseUserInfo userInfo, string mobiles, string message, bool hotline, bool confidentialInformation, string channel = "3")
        {
            var result = 0;
            if (string.IsNullOrEmpty(message))
            {
                return result;
            }
            var mobile = StringUtil.SplitMobile(mobiles, true, false);
            var returnMsg = string.Empty;
            foreach (var cellPhone in mobile)
            {
                if (SendMobile(userInfo, "Base", userInfo.Id, cellPhone, message, string.Empty, hotline, confidentialInformation, channel, out returnMsg) > 0)
                {
                    result++;
                }
            }
            return result;
        }
        #endregion

        // 应用编号
        // public string ApplicationCode = "WangCaiSoft";
        // 短信发送账户编号
        // public string AccountCode = "WangCaiSoft";
        // 应用密码
        // public string Password = "WangCaiSoft";

        /// <summary>
        /// 应用编号
        /// </summary>
        public string ApplicationCode = "";
        /// <summary>
        /// 短信发送账户编号
        /// </summary>
        public string AccountCode = "";
        /// <summary>
        /// 应用密码
        /// </summary>
        public string Password = "";

        /// <summary>
        /// 发送到手机
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <param name="cellPhone"></param>
        /// <param name="message"></param>
        /// <param name="messageCode"></param>
        /// <param name="hotline"></param>
        /// <param name="confidentialInformation"></param>
        /// <param name="channel"></param>
        /// <param name="returnMsg"></param>
        /// <returns></returns>
        public int SendMobile(BaseUserInfo userInfo, string systemCode, string userId, string cellPhone, string message, string messageCode, bool hotline, bool confidentialInformation, string channel, out string returnMsg)
        {
            returnMsg = string.Empty;

            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = "Base";
            }
            if (userInfo != null && userId != "VerificationCode" && userId != "UserPassword")
            {
                if (string.IsNullOrEmpty(userId))
                {
                    userId = userInfo.Id;
                }
                if (userInfo != null && !string.IsNullOrEmpty(userId) && !(userId.Equals("VerificationCode") || userId.Equals("UserPassword")))
                {
                    if (string.IsNullOrEmpty(ApplicationCode))
                    {
                        ApplicationCode = userInfo.CompanyCode;
                    }
                    if (string.IsNullOrEmpty(AccountCode))
                    {
                        AccountCode = userInfo.Code;
                    }
                    if (string.IsNullOrEmpty(Password))
                    {
                        Password = userInfo.Password;
                    }
                }
            }

            /*
            string url = @"http://192.168.0.130:8800/Send.ashx";
            WebClient webClient = new WebClient();
            // webClient.UseDefaultCredentials = true;
            NameValueCollection postValues = new NameValueCollection();
            postValues.Add("V", "02");
            postValues.Add("E", "8");
            postValues.Add("C", this.AccountCode);
            postValues.Add("P", this.Password);
            postValues.Add("A", this.ApplicationCode);
            postValues.Add("U", userId);
            postValues.Add("H", hotline.ToString());
            postValues.Add("T", cellPhone);
            postValues.Add("Channel", channel);
            postValues.Add("MC", messageCode);
            postValues.Add("CI", confidentialInformation.ToString());
            postValues.Add("M", message);
            // 向服务器发送POST数据
            byte[] responseArray = webClient.UploadValues(url, postValues);
            string response = Encoding.UTF8.GetString(responseArray);
            int result = 0;
            if (!string.IsNullOrEmpty(response))
            {
                if (ValidateUtil.IsInt(response))
                {
                    result = int.Parse(response);
                }
            }
            return result;
            */

            var sb = Pool.StringBuilder.Get();
            // request.Append("http://192.168.0.130:8800/Send.ashx?");
            sb.Append("http://mas.zto.cn:8800/Send.ashx?");
            // request.Append("http://122.225.117.230:8800/Send.ashx?");
            sb.Append("V=").Append("02");
            sb.Append("&E=").Append("8");
            sb.Append("&C=").Append(AccountCode);
            sb.Append("&P=").Append(Password);
            sb.Append("&A=").Append(ApplicationCode);
            sb.Append("&U=").Append(userId);
            sb.Append("&H=").Append(hotline.ToString());
            sb.Append("&T=").Append(cellPhone);
            sb.Append("&Channel=").Append(channel);
            sb.Append("&systemCode=").Append(systemCode);
            sb.Append("&MC=").Append(messageCode);
            sb.Append("&CI=").Append(confidentialInformation.ToString());
            sb.Append("&M=");
            sb.Append(System.Web.HttpUtility.UrlEncode(message, Encoding.UTF8));
            return SendMobile(sb.Put(), out returnMsg);
        }

        private int SendMobile(string url, out string returnMsg)
        {
            var result = 0;
            returnMsg = string.Empty;
            var webResponse = RequestUtil.GetResponse(url);
            if (!string.IsNullOrEmpty(webResponse))
            {
                if (ValidateUtil.IsInt(webResponse))
                {
                    result = int.Parse(webResponse);
                }
            }
            /*
            bool flag = false;
            returnMsg = "未知错误";
            switch (webResponse)
            {
                case "1":
                case "Success":
                    flag = true;
                    returnMsg = "提交短信成功";
                    return flag;
                case "0":
                    returnMsg = "失败";
                    return flag;
                case "-1":
                    returnMsg = "用户名或者密码不正确";
                    return flag;
                case "2":
                    returnMsg = "余额不够";
                    return flag;
                case "3":
                    returnMsg = "黑词审核中";
                    return flag;
                case "4":
                    returnMsg = "出现异常，人工处理中";
                    return flag;
                case "5":
                    returnMsg = "提交频率太快";
                    return flag;
                case "6":
                    returnMsg = "有效号码为空";
                    return flag;
                case "7":
                    returnMsg = "短信内容为空";
                    return flag;
                case "8":
                    returnMsg = "一级黑词";
                    return flag;
                case "9":
                    returnMsg = "没有url提交权限";
                    return flag;
                case "10":
                    returnMsg = "发送号码过多";
                    return flag;
                case "11":
                    returnMsg = "产品ID异常";
                    return flag;
            }
            */
            return result;
        }

        /// <summary>
        /// 获取发送日志
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public DataTable GetSentLog(BaseUserInfo userInfo, string beginDate, string endDate)
        {
            var connectionString = string.Empty;
            DataTable result = null;
            connectionString = ConfigurationUtil.AppSettings("OpenMasDbConnection", BaseSystemInfo.EncryptDbConnection);
            if (!string.IsNullOrEmpty(connectionString))
            {
                using (var dbHelper = DbHelperFactory.GetHelper(CurrentDbType.SqlServer, connectionString))
                {
                    var commandText = "SELECT * FROM SmsSentLog WHERE (UserId = " + dbHelper.GetParameter("UserId");
                    if (!string.IsNullOrEmpty(userInfo.Code))
                    {
                        commandText += " OR UserId = " + dbHelper.GetParameter("UserCode") + ")";
                    }
                    else
                    {
                        commandText += ") ";
                    }
                    var dbParameters = new List<IDbDataParameter>
                    {
                        dbHelper.MakeParameter("UserId", userInfo.Id)
                    };
                    if (!string.IsNullOrEmpty(userInfo.Code))
                    {
                        dbParameters.Add(dbHelper.MakeParameter("UserCode", userInfo.Code));
                    }
                    if (beginDate.Trim().Length > 0)
                    {
                        commandText += " AND CreateTime >= " + dbHelper.GetParameter("BeginDate");

                        var dtBeginDate = DateTime.Parse(beginDate);
                        dtBeginDate = new DateTime(dtBeginDate.Year, dtBeginDate.Month, dtBeginDate.Day);
                        dbParameters.Add(dbHelper.MakeParameter("BeginDate", dtBeginDate));
                    }
                    if (endDate.Trim().Length > 0)
                    {
                        commandText += " AND CreateTime <= " + dbHelper.GetParameter("EndDate");

                        var dtEndDate = DateTime.Parse(endDate);
                        dtEndDate = new DateTime(dtEndDate.Year, dtEndDate.Month, dtEndDate.Day);
                        // 这里可以考虑加一天
                        dtEndDate = dtEndDate.AddDays(1);
                        dbParameters.Add(dbHelper.MakeParameter("EndDate", dtEndDate));
                    }
                    commandText += " ORDER BY CreateTime";
                    result = dbHelper.Fill(commandText, dbParameters.ToArray());

                    var isAdministrator = BaseUserManager.IsAdministrator(userInfo.Id);

                    for (var i = 0; i < result.Rows.Count; i++)
                    {
                        if (result.Rows[i]["GatawayStatus"] != DBNull.Value)
                        {
                            if (result.Rows[i]["GatawayStatus"].ToString().Equals("DELIVRD"))
                            {
                                result.Rows[i]["GatawayStatus"] = "成功";
                                if (!isAdministrator)
                                {
                                    // 若不是超级管理员，都屏蔽信息
                                    if (result.Rows[i]["DestinationAddress"] != DBNull.Value)
                                    {
                                        var destinationAddress = result.Rows[i]["DestinationAddress"].ToString();
                                        if (destinationAddress.Length > 10)
                                        {
                                            destinationAddress = destinationAddress.Replace(destinationAddress.Substring(3, 4), "****");
                                            result.Rows[i]["DestinationAddress"] = destinationAddress;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取分页数据（防注入功能的）
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="recordCount">记录条数</param>
        /// <param name="tableName">数据来源表名</param>
        /// <param name="selectField">选择字段</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示多少条</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="dbParameters">查询参数</param>
        /// <param name="orderBy">排序字段</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(BaseUserInfo userInfo, out int recordCount, string tableName, string selectField, int pageIndex, int pageSize, string conditions, List<KeyValuePair<string, object>> dbParameters, string orderBy)
        {
            DataTable result = null;

            recordCount = 0;
            var connectionString = ConfigurationUtil.AppSettings("OpenMasDbConnection", BaseSystemInfo.EncryptDbConnection);
            if (!string.IsNullOrEmpty(connectionString))
            {
                if (SecretUtil.IsSqlSafe(conditions))
                {
                    // 2016-02-24 吉日嘎拉 查询历史支持各种数据库访问方式
                    var openMasDbType = string.Empty;
                    openMasDbType = ConfigurationUtil.AppSettings("OpenMasDbType", BaseSystemInfo.EncryptDbConnection);
                    var dbType = CurrentDbType.SqlServer;
                    if (!string.IsNullOrEmpty(openMasDbType))
                    {
                        dbType = DbHelper.GetDbType(openMasDbType, CurrentDbType.SqlServer);
                    }

                    using (var dbHelper = DbHelperFactory.GetHelper(dbType, connectionString))
                    {
                        recordCount = DbUtil.GetCount(dbHelper, tableName, conditions, dbHelper.MakeParameters(dbParameters));
                        result = DbUtil.GetDataTableByPage(dbHelper, tableName, selectField, pageIndex, pageSize, conditions, dbHelper.MakeParameters(dbParameters), orderBy);
                    }
                }
                else
                {
                    // 记录注入日志
                    LogUtil.WriteLog("userInfo:" + userInfo.Serialize() + " " + conditions, "SqlSafe");
                }
            }

            return result;
        }
        /// <summary>
        /// GetDataTableByPage
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="recordCount"></param>
        /// <param name="tableName"></param>
        /// <param name="selectField"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="conditions"></param>
        /// <param name="dbParameters"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public DataTable GetDataTableByPage(BaseUserInfo userInfo, out int recordCount, string tableName, string selectField, int pageIndex, int pageSize, string conditions, IDbDataParameter[] dbParameters, string orderBy)
        {
            DataTable result = null;

            recordCount = 0;
            var connectionString = ConfigurationUtil.AppSettings("OpenMasDbConnection", BaseSystemInfo.EncryptDbConnection);
            if (!string.IsNullOrEmpty(connectionString))
            {
                if (SecretUtil.IsSqlSafe(conditions))
                {
                    using (var dbHelper = DbHelperFactory.GetHelper(CurrentDbType.SqlServer, connectionString))
                    {
                        recordCount = DbUtil.GetCount(dbHelper, tableName, conditions, dbParameters);
                        result = DbUtil.GetDataTableByPage(dbHelper, tableName, selectField, pageIndex, pageSize, conditions, dbParameters, orderBy);
                    }
                }
                else
                {
                    // 记录注入日志
                    LogUtil.WriteLog("userInfo:" + userInfo.Serialize() + " " + conditions, "SqlSafe");
                }
            }

            return result;
        }

        /// <summary>
        /// 忘记密码按手机号码获取
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="userName">用户名</param>
        /// <param name="mobile">手机号码</param>
        /// <returns>成功</returns>
        public bool GetPasswordByMobile(BaseUserInfo userInfo, string userName, string mobile)
        {
            var result = false;
            if (!string.IsNullOrEmpty(mobile))
            {
                var manager = new BaseUserContactManager();
                var parameters = new List<KeyValuePair<string, object>>();
                if (!string.IsNullOrEmpty(mobile))
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUserContactEntity.FieldMobile, mobile));
                }
                // 手机号码重复不发验证码，防止把别人的密码给修改了
                var dt = manager.GetDataTable(parameters);
                var id = string.Empty;
                if (dt != null && dt.Rows.Count == 1)
                {
                    id = dt.Rows[0][BaseUserContactEntity.FieldId].ToString();
                }
                BaseUserManager userManager = null;
                if (!string.IsNullOrEmpty(id))
                {
                    userManager = new BaseUserManager();
                    var userNameOk = true;
                    var userEntity = userManager.GetObject(id);
                    if (!string.IsNullOrEmpty(userName))
                    {
                        if (!string.IsNullOrEmpty(userEntity.UserName) && !userEntity.UserName.Equals(userName, StringComparison.Ordinal))
                        {
                            userNameOk = false;
                            userInfo = null;
                        }
                    }
                    // 只有有效的用户，才能获取密码，被删除的，无效的，不可以获取密码
                    if (userEntity.Enabled == 0 || userEntity.DeletionStateCode == 1)
                    {
                        userNameOk = false;
                        userInfo = null;
                    }
                    if (userNameOk)
                    {
                        userInfo = userManager.ConvertToUserInfo(userEntity);
                    }
                    else
                    {
                        userInfo = null;
                    }
                }
                if (!string.IsNullOrEmpty(id) && userInfo != null)
                {
                    var userPassword = string.Empty;
                    if (BaseSystemInfo.CheckPasswordStrength)
                    {
                        userPassword = RandomUtil.GetString(8).ToLower();
                    }
                    else
                    {
                        userPassword = RandomUtil.GetString(8).ToLower();
                        // Random random = new System.Random();
                        // userPassword = random.Next(100000, 999999).ToString();
                    }
                    // 看是否有合理的请求参数
                    if (!string.IsNullOrEmpty(userPassword))
                    {
                        // 看是否一天超过了3次了
                        var sendUserPasswordCount = GetSendUserPasswordCount(mobile);
                        if (sendUserPasswordCount < 4)
                        {
                            // 应用编号
                            if (SendUserPassword(userInfo, mobile, userPassword))
                            {
                                userManager = new BaseUserManager(userInfo);
                                // 按手机号码获取的，可以自动解锁，防止密码连续输入错误，然后手机号码获取密码后，是被锁定状态，提高工作效率
                                userManager.SetPassword(userInfo.Id, userPassword, true);
                                userManager.GetStateMessage();
                                if (userManager.StatusCode == Status.SetPasswordOk.ToString())
                                {
                                    result = true;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}