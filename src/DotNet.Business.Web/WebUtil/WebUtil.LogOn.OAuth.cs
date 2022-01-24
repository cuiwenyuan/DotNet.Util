//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using DotNet.Util;

namespace DotNet.Business
{
    /// <summary>
    /// LDAP登录功能相关部分
    /// </summary>
    public partial class WebUtil
    {
#if NET40_OR_GREATER
        #region OAuth登录

        /// <summary>
        /// LogonWindowsAuthentication
        /// </summary>
        /// <param name="systemCode">子系统</param>
        /// <param name="oAuthName">OAuth 类型</param>
        /// <param name="oAuthOpenId">OAuth OpenId</param>
        /// <param name="oAuthUnionId">OAuth UnionId</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="persistCookie">是否保存密码</param>
        /// <param name="formsAuthentication">表单验证，是否需要重定位</param>
        /// <param name="statusCode"></param>
        /// <param name="statusMessage"></param>
        /// <returns></returns>
        public static BaseUserInfo OAuthLogin(string systemCode, string oAuthName, string oAuthOpenId, string oAuthUnionId, string permissionCode, bool persistCookie, bool formsAuthentication, out string statusCode, out string statusMessage)
        {
            BaseUserInfo result = null;
            statusCode = Status.Error.ToString();
            statusMessage = "登录失败";
            var entity = new BaseUserOAuthManager(UserInfo).GetEntity(oAuthName, oAuthOpenId, oAuthUnionId);
            if (entity != null)
            {
                var entityUser = new BaseUserManager(UserInfo).GetEntity(entity.UserId);
                if (entityUser != null)
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
                    var userLogonResult = dotNetService.LogonService.LogonByUserName(taskId, systemCode, GetUserInfo(), entityUser.UserName);
                    // 检查身份
                    if (userLogonResult.StatusCode.Equals(Status.Ok.ToString()))
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
                            Logon(userLogonResult.UserInfo, formsAuthentication);

                            userLogonResult.StatusCode = Status.Ok.ToString();
                            userLogonResult.StatusMessage = "登录成功";
                            statusCode = Status.Ok.ToString();
                            statusMessage = "登录成功";
                            result = userLogonResult.UserInfo;
                        }
                        else
                        {
                            userLogonResult.StatusCode = Status.LogonDeny.ToString();
                            userLogonResult.StatusMessage = "访问被拒绝、您的账户没有访问权限。";
                            statusCode = Status.LogonDeny.ToString();
                            statusMessage = "访问被拒绝、您的账户没有访问权限。";
                            result = userLogonResult.UserInfo;
                        }
                    }
                    else
                    {
                        userLogonResult.StatusCode = Status.LogonDeny.ToString();
                        userLogonResult.StatusMessage = "访问被拒绝、您的账户没有访问权限。";
                        statusCode = Status.LogonDeny.ToString();
                        statusMessage = "访问被拒绝、您的账户没有访问权限。";
                        result = userLogonResult.UserInfo;
                    }
                }
            }

            return result;
        }
        #endregion
#endif
    }
}