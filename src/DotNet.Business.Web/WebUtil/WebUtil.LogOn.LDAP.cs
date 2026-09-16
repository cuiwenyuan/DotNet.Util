//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2026, DotNet.
//-----------------------------------------------------------------

using System;
using System.DirectoryServices;
using DotNet.Util;

namespace DotNet.Business
{
    /// <summary>
    /// LDAP登录功能相关部分
    /// </summary>
    public partial class WebUtil
    {
#if NET46_OR_GREATER
        //LDAP域用户登录部分：包括Windows AD域用户登录
        #region public static BaseUserInfo LogonByLDAP(string domain, string lDAP, string userName, string password, string permissionCode, bool persistCookie, bool formsAuthentication, out Status status, out string statusMessage)

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
        /// <param name="status">状态</param>
        /// <param name="statusMessage"></param>
        /// <returns></returns>
        public static BaseUserInfo LogonByLdap(string domain, string lDap, string systemCode, string userName, string password, string openId, string permissionCode, bool persistCookie, bool formsAuthentication, out Status status, out string statusMessage)
        {
            BaseUserInfo baseUserInfo = null;
            // 统一的登录服务
            var taskId = Guid.NewGuid().ToString("N");
            var userInfo = GetUserInfo();
            if (systemCode.IsNullOrEmpty())
            {
                systemCode = BaseSystemInfo.SystemCode;
                if (systemCode.IsNullOrEmpty())
                {
                    systemCode = userInfo.SystemCode;
                }
            }
            if ((userInfo.IpAddress).IsNullOrEmpty())
            {
                userInfo.IpAddress = Utils.GetIp();
            }

            status = Status.UserNotFound;
            statusMessage = Status.UserNotFound.ToDescription();

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
                    var userManager = new BaseUserManager(userInfo);
                    var userLogonResult = userManager.LogonByUserNameOnly(systemCode, userInfo, userName);

                    // 检查身份
                    if (userLogonResult.Status == Status.Ok)
                    {
                        var isAuthorized = true;
                        // 用户是否有哪个相应的权限
                        if (!permissionCode.IsNullOrEmpty())
                        {
                            var permissionManager = new BasePermissionManager(userInfo);
                            isAuthorized = permissionManager.IsAuthorized(systemCode, userLogonResult.UserInfo.Id.ToString(), permissionCode, null);
                        }
                        // 有相应的权限才可以登录
                        if (isAuthorized)
                        {
                            if (persistCookie)
                            {
                                // 相对安全的方式保存登录状态
                                //SaveCookie(userName, password);
                                // 内部单点登录方式 Troy.Cui 2016.12.26
                                SaveCookie(userLogonResult.UserInfo);
                            }
                            else
                            {
                                RemoveUserCookie();
                            }
                            Logon(userLogonResult.UserInfo, formsAuthentication);

                            userLogonResult.Status = Status.Ok;
                            userLogonResult.StatusCode = Status.Ok.ToString();
                            userLogonResult.StatusMessage = Msg.Get("Logon.Success");
                            status = Status.Ok;
                            statusMessage = Msg.Get("Logon.Success");
                            baseUserInfo = userLogonResult.UserInfo;
                        }
                        else
                        {
                            userLogonResult.Status = Status.LogonDeny;
                            userLogonResult.StatusCode = Status.LogonDeny.ToString();
                            userLogonResult.StatusMessage = Msg.Get("Logon.AccessDeniedAdmin");
                            status = Status.LogonDeny;
                            statusMessage = Msg.Get("Logon.AccessDeniedAdmin");
                            baseUserInfo = userLogonResult.UserInfo;
                        }
                    }
                    else
                    {
                        status = Status.LogonDeny;
                        statusMessage = Msg.Get("Logon.AppUserNotFound");
                    }
                }
            }
            catch (Exception e)
            {
                //Logon failure: unknown user name or bad password.
                status = Status.LogonDeny;
                statusMessage = Msg.Format("Logon.DomainServerResponse", e.Message.Replace("\r\n", ""));
            }

            return baseUserInfo;
        }
        #endregion

        #region Windows认证下用户登录，需IIS开启Windows身份验证，关闭匿名身份验证，Web.config启用Windows身份验证

        /// <summary>
        /// LogonWindowsAuthentication
        /// </summary>
        /// <param name="systemCode">子系统</param>
        /// <param name="userName">域用户名</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="persistCookie">是否保存密码</param>
        /// <param name="formsAuthentication">表单验证，是否需要重定位</param>
        /// <param name="status">状态</param>
        /// <param name="statusMessage"></param>
        /// <returns></returns>
        public static BaseUserInfo LogonWindowsAuthentication(string systemCode, string userName, string permissionCode, bool persistCookie, bool formsAuthentication, out Status status, out string statusMessage)
        {
            BaseUserInfo baseUserInfo = null;
            // 统一的登录服务
            var taskId = Guid.NewGuid().ToString("N");
            var userInfo = GetUserInfo();
            if (systemCode.IsNullOrEmpty())
            {
                systemCode = BaseSystemInfo.SystemCode;
                if (systemCode.IsNullOrEmpty())
                {
                    systemCode = userInfo.SystemCode;
                }
            }
            if ((userInfo.IpAddress).IsNullOrEmpty())
            {
                userInfo.IpAddress = Utils.GetIp();
            }

            var userManager = new BaseUserManager(userInfo);
            var userLogonResult = userManager.LogonByUserNameOnly(systemCode, userInfo, userInfo.UserName);
            // 检查身份
            if (userLogonResult.Status == Status.Ok)
            {
                var isAuthorized = true;
                // 用户是否有哪个相应的权限
                if (!permissionCode.IsNullOrEmpty())
                {
                    var permissionManager = new BasePermissionManager(userInfo);
                    isAuthorized = permissionManager.IsAuthorized(systemCode, userLogonResult.UserInfo.UserId.ToString(), permissionCode, null);
                }
                // 有相应的权限才可以登录
                if (isAuthorized)
                {
                    if (persistCookie)
                    {
                        // 相对安全的方式保存登录状态
                        //SaveCookie(userName, password);
                        // 内部单点登录方式 Troy.Cui 2016.12.26
                        SaveCookie(userLogonResult.UserInfo);
                    }
                    else
                    {
                        RemoveUserCookie();
                    }
                    Logon(userLogonResult.UserInfo, formsAuthentication);
                    userLogonResult.Status = Status.Ok;
                    userLogonResult.StatusCode = Status.Ok.ToString();
                    userLogonResult.StatusMessage = Msg.Get("Logon.Success");
                    status = Status.Ok;
                    statusMessage = Msg.Get("Logon.Success");
                    baseUserInfo = userLogonResult.UserInfo;
                }
                else
                {
                    userLogonResult.Status = Status.LogonDeny;
                    userLogonResult.StatusCode = Status.LogonDeny.ToString();
                    userLogonResult.StatusMessage = Msg.Get("Logon.AccessDenied");
                    status = Status.LogonDeny;
                    statusMessage = Msg.Get("Logon.AccessDenied");
                    baseUserInfo = userLogonResult.UserInfo;
                }
            }
            else
            {
                userLogonResult.Status = Status.LogonDeny;
                userLogonResult.StatusCode = Status.LogonDeny.ToString();
                userLogonResult.StatusMessage = Msg.Get("Logon.AccessDenied");
                status = Status.LogonDeny;
                statusMessage = Msg.Get("Logon.AccessDenied");
                baseUserInfo = userLogonResult.UserInfo;
            }
            return baseUserInfo;
        }
        #endregion
#endif
    }
}
