//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.DirectoryServices;
using DotNet.Util;

namespace DotNet.Business
{
    using Model;

    /// <summary>
    /// LDAP登录功能相关部分
    /// </summary>
    public partial class Utilities
    {
        //LDAP域用户登录部分：包括Windows AD域用户登录
        #region public static BaseUserInfo LogOnByLDAP(string domain, string lDAP, string userName, string password, string permissionCode, bool persistCookie, bool formsAuthentication, out string statusCode, out string statusMessage)

        /// <summary>
        /// 验证LDAP用户
        /// </summary>
        /// <param name="domain">域</param>
        /// <param name="lDap">LDAP</param>
        /// <param name="systemCode">子系统</param>
        /// <param name="userName">域用户名</param>
        /// <param name="password">域密码</param>
        /// <param name="openId">OpenId</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="persistCookie">是否保存密码</param>
        /// <param name="formsAuthentication">表单验证，是否需要重定位</param>
        /// <param name="statusCode"></param>
        /// <param name="statusMessage"></param>
        /// <returns></returns>
        public static BaseUserInfo LogOnByLdap(string domain, string lDap, string systemCode, string userName, string password, string openId, string permissionCode, bool persistCookie, bool formsAuthentication, out string statusCode, out string statusMessage)
        {
            // 统一的登录服务
            var taskId = Guid.NewGuid().ToString("N");
            var userInfo = GetUserInfo();
            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = BaseSystemInfo.SystemCode;
                if (string.IsNullOrEmpty(systemCode))
                {
                    systemCode = userInfo.SystemCode;
                }
            }
            if (string.IsNullOrEmpty(userInfo.IpAddress))
            {
                userInfo.IpAddress = Utils.GetIp();
            }

            var dirEntry = new DirectoryEntry();
            dirEntry.Path = lDap;
            dirEntry.Username = domain + "\\" + userName;
            dirEntry.Password = password;
            dirEntry.AuthenticationType = AuthenticationTypes.Secure;

            try
            {
                var dirSearcher = new DirectorySearcher(dirEntry);
                dirSearcher.Filter = String.Format("(&(&(objectClass=user))(samAccountName={0}))", userName);
                var result = dirSearcher.FindOne();
                //如果LDAP用户登录验证通过
                if (result != null)
                {
                    // 统一的登录服务
                    var dotNetService = new DotNetService();
                    var userLogOnResult = dotNetService.LogOnService.LogOnByUserName(taskId, systemCode, GetUserInfo(), userName);
                    // 检查身份
                    if (userLogOnResult.StatusCode.Equals(Status.Ok.ToString()))
                    {
                        var isAuthorized = true;
                        // 用户是否有哪个相应的权限
                        if (!string.IsNullOrEmpty(permissionCode))
                        {
                            isAuthorized = dotNetService.PermissionService.IsAuthorized(userInfo, permissionCode, null);
                        }
                        // 有相应的权限才可以登录
                        if (isAuthorized)
                        {
                            if (persistCookie)
                            {
                                // 相对安全的方式保存登录状态
                                //SaveCookie(userName, password);
                                // 内部单点登录方式 Troy.Cui 2016.12.26
                                SaveCookie(userInfo);
                            }
                            else
                            {
                                RemoveUserCookie();
                            }
                            LogOn(userLogOnResult.UserInfo, formsAuthentication);
                        }
                        else
                        {
                            userLogOnResult.StatusCode = Status.LogOnDeny.ToString();
                            userLogOnResult.StatusMessage = "访问被拒绝、您的账户没有后台管理访问权限。";
                            statusCode = Status.LogOnDeny.ToString();
                            statusMessage = "访问被拒绝、您的账户没有后台管理访问权限。";
                            return userLogOnResult.UserInfo;
                        }
                    }
                    userLogOnResult.StatusCode = Status.Ok.ToString();
                    userLogOnResult.StatusMessage = "登录成功";
                    statusCode = Status.Ok.ToString();
                    statusMessage = "登录成功";
                    return userLogOnResult.UserInfo;
                }
                else
                {
                    statusCode = Status.LogOnDeny.ToString();
                    statusMessage = "应用系统用户不存在，请联系管理员。";
                    return null;
                }
            }
            catch (Exception e)
            {
                //Logon failure: unknown user name or bad password.
                statusCode = Status.LogOnDeny.ToString();
                statusMessage = "域服务器返回信息" + e.Message.Replace("\r\n", "");
                return null;
            }

            
        }
        #endregion

        #region Windows认证下用户登录，需IIS开启Windows身份验证，关闭匿名身份验证，Web.config启用Windows身份验证

        /// <summary>
        /// LogOnWindowsAuthentication
        /// </summary>
        /// <param name="systemCode">子系统</param>
        /// <param name="userName">域用户名</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="persistCookie">是否保存密码</param>
        /// <param name="formsAuthentication">表单验证，是否需要重定位</param>
        /// <param name="statusCode"></param>
        /// <param name="statusMessage"></param>
        /// <returns></returns>
        public static BaseUserInfo LogOnWindowsAuthentication(string systemCode, string userName, string permissionCode, bool persistCookie, bool formsAuthentication, out string statusCode, out string statusMessage)
        {
            // 统一的登录服务
            var taskId = Guid.NewGuid().ToString("N");
            var userInfo = GetUserInfo();
            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = BaseSystemInfo.SystemCode;
                if (string.IsNullOrEmpty(systemCode))
                {
                    systemCode = userInfo.SystemCode;
                }
            }
            if (string.IsNullOrEmpty(userInfo.IpAddress))
            {
                userInfo.IpAddress = Utils.GetIp();
            }

            // 统一的登录服务
            var dotNetService = new DotNetService();
            var userLogOnResult = dotNetService.LogOnService.LogOnByUserName(taskId, systemCode, GetUserInfo(), userName);
            // 检查身份
            if (userLogOnResult.StatusCode.Equals(Status.Ok.ToString()))
            {
                var isAuthorized = true;
                // 用户是否有哪个相应的权限
                if (!string.IsNullOrEmpty(permissionCode))
                {
                    isAuthorized = dotNetService.PermissionService.IsAuthorized(userInfo, permissionCode, null);
                }
                // 有相应的权限才可以登录
                if (isAuthorized)
                {
                    if (persistCookie)
                    {
                        // 相对安全的方式保存登录状态
                        //SaveCookie(userName, password);
                        // 内部单点登录方式 Troy.Cui 2016.12.26
                        SaveCookie(userInfo);
                    }
                    else
                    {
                        RemoveUserCookie();
                    }
                    LogOn(userLogOnResult.UserInfo, formsAuthentication);

                    userLogOnResult.StatusCode = Status.Ok.ToString();
                    userLogOnResult.StatusMessage = "登录成功";
                    statusCode = Status.Ok.ToString();
                    statusMessage = "登录成功";
                    return userLogOnResult.UserInfo;
                }
                else
                {
                    userLogOnResult.StatusCode = Status.LogOnDeny.ToString();
                    userLogOnResult.StatusMessage = "访问被拒绝、您的账户没有访问权限。";
                    statusCode = Status.LogOnDeny.ToString();
                    statusMessage = "访问被拒绝、您的账户没有访问权限。";
                    return userLogOnResult.UserInfo;
                }
            }
            else
            {
                userLogOnResult.StatusCode = Status.LogOnDeny.ToString();
                userLogOnResult.StatusMessage = "访问被拒绝、您的账户没有访问权限。";
                statusCode = Status.LogOnDeny.ToString();
                statusMessage = "访问被拒绝、您的账户没有访问权限。";
                return userLogOnResult.UserInfo;
            }

        }
        #endregion

    }
}