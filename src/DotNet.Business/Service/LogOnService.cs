//-----------------------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;

namespace DotNet.Business
{
    using IService;
    using Model;
    using Util;

    /// <summary>
    /// LogonService
    /// 
    /// 修改记录
    /// 
    ///		2016.02.14 版本：2.0 JiRiGaLa 增加访问日志记录功能。
    ///		2015.12.09 版本：1.1 JiRiGaLa 增加修改密码设置密码的日志记录。
    ///		2013.06.06 版本：1.0 张祈璟   重构。
    ///		2009.04.15 版本：1.0 JiRiGaLa 添加接口定义。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.02.14</date>
    /// </author> 
    /// </summary>


    public partial class LogonService : IBaseUserLogonService
    {
        /// <summary>
        /// 获取系统版本号
        /// <param name="taskId">任务标识</param>
        /// </summary>
        /// <returns>版本号</returns>
        public string GetServerVersion(string taskId)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var result = fileVersionInfo.FileVersion;
            return result;
        }

        /// <summary>
        /// 获取服务器时间
        /// <param name="taskId">任务标识</param>
        /// </summary>
        /// <returns>当前时间</returns>
        public DateTime GetServerDateTime(string taskId)
        {
            return DateTime.Now;
        }

        /// <summary>
        /// 获取数据库服务器时间
        /// <param name="taskId">任务标识</param>
        /// </summary>
        /// <returns>数据库时间</returns>
        public DateTime GetDbDateTime(string taskId)
        {
            var result = DateTime.Now;

            using (var dbHelper = DbHelperFactory.Create(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection))
            {
                result = DateTime.Parse(dbHelper.GetDbDateTime());
            }

            return result;
        }

        /// <summary>
        /// 获取远端IP
        /// </summary>
        /// <returns></returns>
        public string GetRemoteIp()
        {
            var result = string.Empty;

            // 提供方法执行的上下文环境
            //var context = OperationContext.Current;
            //if (context != null)
            //{
            //    // 获取传进的消息属性
            //    var properties = context.IncomingMessageProperties;
            //    // 获取消息发送的远程终结点IP和端口
            //    var endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            //    result = endpoint?.Address;
            //}
            if (result == "::1")
            {
                result = "127.0.0.1";
            }

            return result;
        }

        #region public UserLogonResult UserLogon(string taskId, BaseUserInfo userInfo, string userName, string password, string openId)
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="openId">单点登录标识</param>
        /// <returns>登录实体类</returns>
        public UserLogonResult UserLogon(string taskId, BaseUserInfo userInfo, string userName, string password, string openId)
        {
            var result = new UserLogonResult();

            var parameter = ServiceInfo.Create(taskId, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(userInfo) { CheckIsAdministrator = true };
                result = userManager.LogonByUserName(userName, password, userInfo.SystemCode, openId);
                //2016-02-16 吉日嘎拉 记录用户日志用
                parameter.UserInfo = result.UserInfo;
            });

            return result;
        }
        #endregion

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="companyName">单位名称</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="openId">单点登录标识</param>
        /// <returns>登录实体类</returns>
        public UserLogonResult LogonByCompany(string taskId, BaseUserInfo userInfo, string companyName, string userName, string password, string openId)
        {
            var result = new UserLogonResult();

            var parameter = ServiceInfo.Create(taskId, userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                // 先侦测是否在线
                // userLogonManager.CheckOnline();
                // 再进行登录
                var userManager = new BaseUserManager(userInfo)
                {
                    CheckIsAdministrator = true
                };
                result = userManager.LogonByCompany(companyName, userName, password, openId, userInfo.SystemCode, GetRemoteIp());
                // 张祈璟20130619添加
                //if (returnUserInfo != null)
                //{
                //    returnUserInfo.CloneData(userInfo);
                //    result.UserInfo = returnUserInfo;
                //}
                // 登录时会自动记录进行日志记录，所以不需要进行重复日志记录
                // BaseLogManager.Instance.Add(result, this.serviceName, MethodBase.GetCurrentMethod());
            });

            return result;
        }

        #region public UserLogonResult LogonByNickName(string taskId, BaseUserInfo userInfo, string nickName, string password, string openId)
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="nickName">昵称</param>
        /// <param name="password">密码</param>
        /// <param name="openId">单点登录标识</param>
        /// <returns>登录实体类</returns>
        public UserLogonResult LogonByNickName(string taskId, BaseUserInfo userInfo, string nickName, string password, string openId)
        {
            var result = new UserLogonResult();

            var parameter = ServiceInfo.Create(taskId, userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                // 先侦测是否在线
                // userLogonManager.CheckOnline();
                // 再进行登录
                var userManager = new BaseUserManager(userInfo)
                {
                    CheckIsAdministrator = true
                };
                result = userManager.LogonByNickName(nickName, password, openId, userInfo.SystemCode);
            });

            return result;
        }
        #endregion

        #region public UserLogonResult LogonByOpenId(string taskId, BaseUserInfo userInfo, string openId)
        /// <summary>
        /// 按唯一识别码登录
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="openId">唯一识别码</param>
        /// <returns>用户实体</returns>
        public UserLogonResult LogonByOpenId(string taskId, BaseUserInfo userInfo, string openId)
        {
            var result = new UserLogonResult();

            var parameter = ServiceInfo.Create(taskId, userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                // 先侦测是否在线
                var userLogonManager = new BaseUserLogonManager();
                userLogonManager.CheckOnline();
                // 若是单点登录，那就不能判断ip地址，因为不是直接登录，是间接登录
                var userManager = new BaseUserManager(userInfo);
                result = userManager.LogonByOpenId(openId, userInfo.SystemCode, string.Empty, string.Empty);
            });

            return result;
        }
        #endregion


        /// <summary>
        /// 获取新的OpenId
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <returns>OpenId</returns>
        public string CreateOpenId(string taskId, BaseUserInfo userInfo)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(taskId, userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userLogonManager = new BaseUserLogonManager(userInfo);
                result = userLogonManager.CreateOpenId();
            });

            return result;
        }

        #region public int ServerCheckOnline(string taskId)
        /// <summary>
        /// 服务器端检查在线状态
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <returns>离线人数</returns>
        public int ServerCheckOnline(string taskId)
        {
            var result = 0;

            using (var dbHelper = DbHelperFactory.Create(BaseSystemInfo.UserCenterDbType))
            {
                try
                {
                    dbHelper.Open(BaseSystemInfo.UserCenterWriteDbConnection);
                    var userLogonManager = new BaseUserLogonManager(dbHelper);
                    result = userLogonManager.CheckOnline();
                }
                catch (Exception ex)
                {
                    LogUtil.WriteException(ex);
                    throw;
                }
                finally
                {
                    dbHelper.Close();
                }
            }

            return result;
        }
        #endregion

        #region public void Online(string taskId, BaseUserInfo userInfo, int onLineState = 1)
        /// <summary>
        /// 用户现在
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="onLineState">用户在线状态</param>
        public void Online(string taskId, BaseUserInfo userInfo, int onLineState = 1)
        {
            var parameter = ServiceInfo.Create(taskId, userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userLogonManager = new BaseUserLogonManager();
                userLogonManager.Online(userInfo.UserId, onLineState);
            });
        }
        #endregion

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public BaseUserLogonEntity GetEntity(string taskId, BaseUserInfo userInfo, string id)
        {
            BaseUserLogonEntity result = null;

            var parameter = ServiceInfo.Create(taskId, userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                // 判断是否已经登录的用户？
                if (userManager.UserIsLogon(userInfo))
                {
                    var userLogonManager = new BaseUserLogonManager();
                    result = userLogonManager.GetEntity(id);
                }
            });

            return result;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <returns>影响行数</returns>
        public int Update(string taskId, BaseUserInfo userInfo, BaseUserLogonEntity entity)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(taskId, userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                // 调用方法，并且返回运行结果
                var userLogonManager = new BaseUserLogonManager();
                result = userLogonManager.Update(entity, true);
            });

            return result;
        }

        #region public BaseUserInfo AccountActivation(string taskId, BaseUserInfo userInfo, string openId)
        /// <summary>
        /// 激活用户
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="openId">唯一识别码</param>
        /// <returns>用户实体</returns>
        public BaseUserInfo AccountActivation(string taskId, BaseUserInfo userInfo, string openId)
        {
            BaseUserInfo result = null;

            var parameter = ServiceInfo.Create(taskId, userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userLogonManager = new BaseUserLogonManager(dbHelper, userInfo);
                // 先侦测是否在线
                userLogonManager.CheckOnline();
                // 再进行登录
                var userManager = new BaseUserManager(dbHelper, userInfo);
                result = userManager.AccountActivation(openId);
            });

            return result;
        }
        #endregion

        #region public int SetPassword(string taskId, BaseUserInfo userInfo, string[] userIds, string password)
        /// <summary>
        /// 设置用户密码
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="userIds">被设置的员工主键</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>影响行数</returns>
        public int SetPassword(string taskId, BaseUserInfo userInfo, string[] userIds, string newPassword)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(taskId, userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                result = userManager.BatchSetPassword(userIds, newPassword, true, true);
            });

            return result;
        }
        #endregion

        #region public UserLogonResult ChangePassword(string taskId, BaseUserInfo userInfo, string oldPassword, string newPassword)
        /// <summary>
        /// 用户修改密码
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="oldPassword">原始密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>影响行数</returns>
        public UserLogonResult ChangePassword(string taskId, BaseUserInfo userInfo, string oldPassword, string newPassword)
        {
            UserLogonResult result = null;

            var parameter = ServiceInfo.Create(taskId, userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                // 事务开始
                // dbHelper.BeginTransaction();
                var userManager = new BaseUserManager(dbHelper, userInfo);
                result = new UserLogonResult
                {
                    UserInfo = userManager.ChangePassword(userInfo.Id.ToString(), oldPassword, newPassword),

                    // 获取登录后信息
                    // result.Message = BaseParameterManager.GetParameterByCache("BaseNotice", "System", "Logon", "Message");
                    // 获得状态消息
                    Status = userManager.Status,
                    StatusCode = userManager.StatusCode,
                    StatusMessage = userManager.GetStateMessage()
                };
                // 事务提交
                // dbHelper.CommitTransaction();
            });

            return result;
        }
        #endregion

        #region public static bool UserIsLogon(string taskId, BaseUserInfo userInfo)
        /// <summary>
        /// 用户是否已经登录了系统？
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <returns>是否登录</returns>
        public static bool UserIsLogon(string taskId, BaseUserInfo userInfo)
        {
            // 加强安全验证防止未授权匿名调用
            if (!BaseSystemInfo.IsAuthorized(userInfo))
            {
                throw new Exception(AppMessage.Msg0800);
            }
            // 这里表示是没登录过的用户
            // if (string.IsNullOrEmpty(result.OpenId))
            // {
            //    throw new Exception(AppMessage.MSG0900);            
            // }
            // 确认用户是否登录了？是否进行了匿名的破坏工作
            /*
            IDbHelper dbHelper = DbHelperFactory.Create(BaseSystemInfo.UserCenterDbConnection);
            var userManager = new BaseUserManager(dbHelper, result);
            if (!userManager.UserIsLogon(result))
            {
                throw new Exception(AppMessage.MSG0900);            
            }
            */
            return true;
        }
        #endregion

        #region public bool LockUser(string taskId, BaseUserInfo userInfo, string userName)
        /// <summary>
        /// 锁定用户
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="userName">用户名</param>
        /// <returns>是否成功锁定</returns>
        public bool LockUser(string taskId, BaseUserInfo userInfo, string userName)
        {
            var result = false;

            var parameter = ServiceInfo.Create(taskId, userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                // BaseLogManager.Instance.Add(result, this.serviceName, AppMessage.LogonService_LockUser, MethodBase.GetCurrentMethod());
                var userManager = new BaseUserManager(userInfo);
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserEntity.FieldUserName, userName),
                    new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
                };
                var userEntity = BaseEntity.Create<BaseUserEntity>(userManager.GetDataTable(parameters));
                // 判断是否为空的
                if (userEntity != null && userEntity.Id > 0)
                {
                    // 被锁定15分钟，不允许15分钟内登录，这时间是按服务器的时间来的。
                    var userLogonManager = new BaseUserLogonManager();
                    var userLogonEntity = userLogonManager.GetEntityByUserId(userEntity.Id);
                    userLogonEntity.LockStartTime = DateTime.Now;
                    userLogonEntity.LockEndTime = DateTime.Now.AddMinutes(BaseSystemInfo.PasswordErrorLockCycle);
                    result = userLogonManager.UpdateEntity(userLogonEntity) > 0;
                }
            });

            return result;
        }
        #endregion

        #region public DataTable GetUserDT(string taskId, BaseUserInfo userInfo)
        /// <summary>
        /// 获得用户列表
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public DataTable GetUserDT(string taskId, BaseUserInfo userInfo)
        {
            var result = new DataTable(BaseUserEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(taskId, userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 检查用户在线状态(服务器专用)
                var userLogonManager = new BaseUserLogonManager();
                userLogonManager.CheckOnline();
                var userManager = new BaseUserManager(dbHelper, userInfo);
                // 获取允许登录列表
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
                };
                result = userManager.GetDataTable(parameters, BaseUserEntity.FieldSortCode);
                result.TableName = BaseUserEntity.CurrentTableName;
            });

            return result;
        }
        #endregion

        #region public bool ValidateVerificationCode(string taskId, BaseUserInfo userInfo, string code)
        /// <summary>
        /// 判断证码是否正确
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="code">验证码</param>
        /// <returns>正确</returns>
        public bool ValidateVerificationCode(string taskId, BaseUserInfo userInfo, string code)
        {
            var result = false;

            var userLogonManager = new BaseUserLogonManager();
            var verificationCode = userLogonManager.GetProperty(userInfo.Id, BaseUserLogonEntity.FieldVerificationCode);
            if (!string.IsNullOrEmpty(verificationCode))
            {
                result = verificationCode.Equals(code);
            }

            return result;
        }
        #endregion

        /// <summary>
        /// 忘记密码按电子邮件获取
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="userName">用户名</param>
        /// <param name="email">电子邮件</param>
        /// <returns>成功</returns>
        public bool GetPasswordByEmail(string taskId, BaseUserInfo userInfo, string userName, string email)
        {
            var result = false;

            var manager = new BaseUserContactManager();
            var parameters = new List<KeyValuePair<string, object>>();
            if (!string.IsNullOrEmpty(email))
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUserContactEntity.FieldEmail, email));
            }
            var id = manager.GetId(parameters);
            if (!string.IsNullOrEmpty(id))
            {
                var userManager = new BaseUserManager();
                var userNameOk = true;
                var userEntity = userManager.GetEntity(id);
                if (!string.IsNullOrEmpty(userName))
                {
                    if (!string.IsNullOrEmpty(userEntity.UserName) && !userEntity.UserName.Equals(userName, StringComparison.Ordinal))
                    {
                        userNameOk = false;
                        userInfo = null;
                    }
                }
                if (userNameOk)
                {
                    userInfo = userManager.ConvertToUserInfo(userEntity);
                }
            }
            if (!string.IsNullOrEmpty(id))
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

                // 邮件内容       
                var smtpClient = new SmtpClient(BaseSystemInfo.MailServer)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(BaseSystemInfo.MailUserName, BaseSystemInfo.MailPassword),
                    // 指定如何处理待发的邮件
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                var mailTitle = BaseSystemInfo.SoftFullName + "忘记密码";

                var mailBody = "您的新密码为：" + userPassword + " " + Environment.NewLine
                    + "<br/> " + Environment.NewLine + BaseSystemInfo.SoftFullName + "访问地址： http://www.wangcaisoft.com/";
                // 读取模板文件
                var file = BaseSystemInfo.StartupPath + "\\Forgot.Mail.txt";
                if (System.IO.File.Exists(file))
                {
                    mailBody = System.IO.File.ReadAllText(file, Encoding.UTF8);
                    mailBody = mailBody.Replace("{Realname}", userInfo.RealName);
                    mailBody = mailBody.Replace("{UserPassword}", userPassword);
                }
                // 发送邮件
                var mailMessage = new MailMessage(BaseSystemInfo.MailUserName, email, mailTitle, mailBody)
                {
                    BodyEncoding = Encoding.Default,
                    IsBodyHtml = true
                };
                smtpClient.Send(mailMessage);

                var userManager = new BaseUserManager(userInfo);
                userManager.SetPassword(userInfo.UserId, userPassword);
                userManager.GetStateMessage();
                if (userManager.StatusCode == Status.SetPasswordOk.ToString())
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        #region public DataTable GetDataTableByPage(string taskId, BaseUserInfo userInfo, out int recordCount, int pageNo, int pageSize, string condition, List<KeyValuePair<string, object>> dbParameters, string order = null)
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="condition">条件</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string taskId, BaseUserInfo userInfo, out int recordCount, int pageNo, int pageSize, string condition, List<KeyValuePair<string, object>> dbParameters, string order = null)
        {
            var result = new DataTable(BaseLogonLogEntity.CurrentTableName);
            var myRecordCount = 0;

            var parameter = ServiceInfo.Create(taskId, userInfo, MethodBase.GetCurrentMethod());
            // 这里需要连接到登录日志数据库服务器
            ServiceUtil.ProcessLogonLogDb(userInfo, parameter, (dbHelper) =>
            {
                if (SecretUtil.IsSqlSafe(condition))
                {
                    var loginLogManager = new BaseLogonLogManager(dbHelper, userInfo);
                    result = loginLogManager.GetDataTableByPage(out myRecordCount, pageNo, pageSize, condition, dbHelper.MakeParameters(dbParameters), order);
                    result.TableName = BaseLogonLogEntity.CurrentTableName;
                }
                else
                {
                    // 记录注入日志
                    LogUtil.WriteLog("userInfo:" + userInfo.Serialize() + " " + condition, "SqlSafe");
                }
            });
            recordCount = myRecordCount;

            return result;
        }
        #endregion

        #region public void SignOut(string taskId, BaseUserInfo userInfo)
        /// <summary>
        /// 用户离线(退出)
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        public void SignOut(string taskId, BaseUserInfo userInfo)
        {
            var parameter = ServiceInfo.Create(taskId, userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                // 2015-12-14 吉日嘎拉 用户的登录日志不用重复写日志
                var userLogonManager = new BaseUserLogonManager();
                userLogonManager.SignOut(userInfo.OpenId);
            });
        }
        #endregion
    }
}