//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Net;
using System.Text;
#if NET452_OR_GREATER
using System.Web;
using System.Web.Security;
#endif
namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// 登录功能相关部分
    /// </summary>
    public partial class WebUtil
    {
        /// <summary>
        /// 数据库连接串，改进性能只读取一次就可以了
        /// </summary>
        public static readonly string BusinessDbConnection = ConfigurationManager.AppSettings["BusinessDbConnection"];

        /// <summary>
        /// 用户中心数据库连接
        /// </summary>
        public static readonly string UserCenterDbConnection = ConfigurationManager.AppSettings["UserCenterDbConnection"];

        /// <summary>
        /// Cookie 名称
        /// </summary>
        public static string CookieName = ConfigurationManager.AppSettings["CookieName"];
        /// <summary>
        /// Cookie 域名
        /// </summary>
        public static string CookieDomain = ConfigurationManager.AppSettings["CookieDomain"];

        /// <summary>
        /// Cookie 用户名
        /// </summary>
        public static string CookieUserName = "UserName";
        /// <summary>
        /// Cookie 密码
        /// </summary>
        public static string CookiePassword = "Password";

        /// <summary>
        /// Session 名称
        /// </summary>
        public static string SessionName = "UserInfo";


        #region public static List<BaseModuleEntity> GetUserPermissionList(BaseUserInfo userInfo, string userId = null) 获用户拥有的操作权限列表
        /// <summary>
        /// 获用户拥有的操作权限列表
        /// </summary>
        /// <param name="userInfo">当前操作员</param>
        /// <param name="userId">用户主键</param>
        public static List<BaseModuleEntity> GetUserPermissionList(BaseUserInfo userInfo, string userId = null)
        {
            return new BasePermissionManager(userInfo).GetUserPermissionList(userInfo, userId, BaseSystemInfo.SystemCode);
        }
        #endregion

#if NET452_OR_GREATER

        #region public static bool CheckIsLogon(string accessDenyUrl = null) 检查是否已登录
        /// <summary>
        /// 检查是否已登录
        /// </summary>
        /// <param name="accessDenyUrl"></param>
        /// <param name="returnUrl">返回Url</param>
        /// <returns></returns>
        public static bool CheckIsLogon(string returnUrl = null, string accessDenyUrl = null)
        {
            if (!UserIsLogon())
            {
                if (string.IsNullOrEmpty(accessDenyUrl))
                {
                    //获取设置的当点登录页面
                    if (ConfigurationManager.AppSettings["SSO"] != null)
                    {
                        UserLogonPage = ConfigurationManager.AppSettings["SSO"];
                    }

                    if (string.IsNullOrWhiteSpace(returnUrl))
                    {
                        //获取当前页面
                        returnUrl = HttpUtility.UrlEncode(HttpContext.Current.Request.Url.ToString());
                        //returnUrl = Server.UrlEncode(HttpContext.Current.Request.Url.ToString()); 
                    }
                    var js = @"<script language='JavaScript'>top.window.location.replace('{0}');</script>";
                    js = string.Format(js, UserLogonPage + "?ReturnUrl=" + returnUrl);
                    HttpContext.Current.Response.Write(js);
                    //这里需要结束输出了，防止意外发生。
                    HttpContext.Current.Response.End();
                }
                else
                {
                    HttpContext.Current.Response.Redirect(accessDenyUrl);
                }
                return false;
            }
            return true;
        }
        #endregion

        #region 判断用户是否已登录部分

        #region public static bool UserIsLogon() 判断用户是否已登录
        /// <summary>
        /// 判断用户是否已登录
        /// </summary>
        /// <returns>已登录</returns>
        public static bool UserIsLogon()
        {
            // 先判断 Session 里是否有用户，若没有检查Cookie是没错
            if (HttpContext.Current.Session == null || HttpContext.Current.Session[SessionName] == null)
            {
                // 检查是否有Cookie？若密码有错，这里就无法登录成功了
                CheckCookie();
            }
            else
            {
                // 这里还需要检查用户在线过程中是否有设置被修改过，例如密码被修改过
                if (BaseSystemInfo.CheckPasswordStrength)
                {
                    // 密码不对，要退出，修改了密码，不能继续在线上了。
                    // 若是IP地址变了，也需要重新登录
                    // 检查是否有Cookie？，其实自己修改过密码，没必要重新登录的，所以需要检查 UserInfo.OpenId 是否有变过
                    CheckOpenId();
                }
            }
            // 若用户没，就是登录不成功了
            if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionName] != null)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region public static void SetSession(BaseUserInfo userInfo)
        /// <summary>
        /// 保存Session
        /// </summary>
        /// <param name="userInfo">当前用户</param>
        public static void SetSession(BaseUserInfo userInfo)
        {
            // 检查是否有效用户
            if (userInfo != null)
            {
                if (userInfo.UserId > 0)
                {
                    // if (result.RoleId.Length == 0)
                    // {
                    //     result.RoleId = DefaultRole.OnlyOwnData.ToString();
                    // }
                    // 操作员
                    if (HttpContext.Current.Session != null)
                    {
                        // HttpContext.Current.Session[SessionName] = result;
                        HttpContext.Current.Session[SessionName] = new CurrentUserInfo(userInfo);
                    }
                }
            }
            else
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session.Remove(SessionName);
                }
            }
        }
        #endregion

        #region public static BaseUserInfo UserInfo 获取用户信息
        /// <summary>
        /// 获取用户信息
        /// </summary>
        public static BaseUserInfo UserInfo => GetUserInfo();

        #endregion

        #endregion

        #region 判断当前的CheckCookie内容情况

        #region public static BaseUserInfo CheckCookie()
        /// <summary>
        /// 检查当前的Cookie内容
        /// </summary>
        public static BaseUserInfo CheckCookie()
        {
            //这里要考虑，从来没登录过的情况
            var userInfo = GetUserCookie();
            if (userInfo != null && !string.IsNullOrEmpty(userInfo.IpAddress) && !userInfo.IpAddress.Equals(GetUserInfo().IpAddress))
            {
                userInfo = null;
            }
            else
            {
                //这里应该再判断是否密码过期，用户过期等等，不只是用Cookie就可以了
                userInfo = CheckCookie(HttpContext.Current.Request);
                //若是用户的Cookie的IPAddress与当前的IPAddress不一样了，那就是IP地址变过了
            }
            SetSession(userInfo);
            return userInfo;
        }
        #endregion

        #region public static BaseUserInfo CheckOpenId()
        /// <summary>
        /// 检查当前的OpenId内容，
        /// 若有密码等被修改时，会踢出前面的用户，密码不正确了，就会踢出用户
        /// </summary>
        public static BaseUserInfo CheckOpenId()
        {
            var userInfo = GetUserInfo();
            // 已经登录的用户IP地址是否有变化了
            if (!string.IsNullOrEmpty(userInfo.OpenId))
            {
                var userManager = new BaseUserManager(userInfo);
                //userInfo = userManager.LogonByOpenId(userInfo.OpenId, userInfo.IPAddress).UserInfo;
                userInfo = userManager.LogonByOpenId(userInfo.OpenId, UserInfo.SystemCode, userInfo.IpAddress).UserInfo;
                SetSession(userInfo);
            }
            return userInfo;
        }
        #endregion

        #region public static BaseUserInfo CheckCookie(HttpRequest httpRequest)
        /// <summary>
        /// 检查当前的Cookie内容
        /// </summary>
        /// <param name="httpRequest">当前页</param>
        /// <returns>Cookie内容</returns>
        public static BaseUserInfo CheckCookie(HttpRequest httpRequest)
        {
            BaseUserInfo userInfo = null;
            // 取得cookie的保存信息
            var httpCookie = httpRequest.Cookies[CookieName];
            if (httpCookie != null)
            {
                // 读取用户名
                if (!string.IsNullOrEmpty(httpCookie.Values[CookieUserName]))
                {
                    // 2012.11.03 Pcsky修改，解决中文用户名无法自动登录的问题。
                    var username = httpCookie.Values[CookieUserName];
                    username = HttpContext.Current.Server.UrlDecode(username);

                    if (string.IsNullOrEmpty(BaseSystemInfo.UserCenterDbConnection))
                    {
                        // 若没有能连接数据库，就直接从Cookie读取用户，这里应该重新定位一下用户信息那里，判断用户是否有效等等，密码是否修改了等等。
                        userInfo = GetUserCookie();
                    }
                    else
                    {
                        if (BaseSystemInfo.RememberPassword)
                        {
                            //if (BaseSystemInfo.CheckIPAddress)
                            //{
                            //    if (!string.IsNullOrEmpty(httpCookie.Values["IPAddress"]))
                            //    {
                            //        string ipAddress = httpCookie.Values["IPAddress"];
                            //        // 若IP地址变了，也需要重新登录，从数据库获取用户的登录信息
                            //        if (!string.IsNullOrEmpty(result.IPAddress))
                            //        {
                            //            if (!ipAddress.Equals(result.IPAddress))
                            //            {
                            //                result = null;
                            //                return result;
                            //            }
                            //        }
                            //    }
                            //}

                            // 读取密码
                            var password = string.Empty;
                            if (!string.IsNullOrEmpty(httpCookie.Values[CookiePassword]))
                            {
                                password = httpCookie.Values[CookiePassword];
                                password = Decrypt(password);
                            }

                            // 2013-02-20 吉日嘎拉
                            // 进行登录，这里是靠重新登录获取 Cookie，这里其实是判断密码是不是过期了，其实这里openId登录也可以
                            userInfo = Logon(username, password, false);
                        }
                    }
                }
            }
            return userInfo;
        }
        #endregion

        #region public static BaseUserInfo GetUserCookie() 获取用户相应的Cookies信息
        /// <summary>
        /// 获取用户相应的Cookies信息
        /// </summary>
        /// <returns></returns>
        public static BaseUserInfo GetUserCookie()
        {
            BaseUserInfo userInfo = null;
            var httpRequest = HttpContext.Current.Request;
            var httpCookie = httpRequest.Cookies[CookieName];
            if (httpCookie != null)
            {
                userInfo = new BaseUserInfo();

                userInfo.Id = httpCookie.Values["Id"];
                if (ValidateUtil.IsInt(httpCookie.Values["Id"]))
                {
                    userInfo.UserId = Convert.ToInt32(httpCookie.Values["Id"]);
                }
                userInfo.OpenId = httpCookie.Values["OpenId"];

                /*
                 *  2013-02-20 吉日嘎拉
                 *  若有安全要求，以下信息都可以不从 Cookies 里读取，就是读取了，也重新从后台覆盖，若没有安全要求，才可以从 Cookies 读取
                 *  其实也是为了提高系统的效率，这些信息没反复从后台读取
                 */

                userInfo.RealName = HttpUtility.UrlDecode(httpCookie.Values["RealName"]);
                userInfo.UserName = HttpUtility.UrlDecode(httpCookie.Values[CookieUserName]);
                userInfo.Code = httpCookie.Values["Code"];

                userInfo.ServicePassword = httpCookie.Values["ServicePassword"];
                userInfo.ServiceUserName = httpCookie.Values["ServiceUserName"];

                //result.TargetUserId = httpCookie.Values["TargetUserId"];
                //result.StaffId = httpCookie.Values["StaffId"];

                userInfo.CompanyCode = httpCookie.Values["CompanyCode"];
                if (!string.IsNullOrEmpty(httpCookie.Values["CompanyId"]))
                {
                    userInfo.CompanyId = httpCookie.Values["CompanyId"];
                }
                else
                {
                    userInfo.CompanyId = null;
                }
                userInfo.CompanyName = HttpUtility.UrlDecode(httpCookie.Values["CompanyName"]);

                if (!string.IsNullOrEmpty(httpCookie.Values["DepartmentId"]))
                {
                    userInfo.DepartmentId = httpCookie.Values["DepartmentId"];
                }
                else
                {
                    userInfo.DepartmentId = null;
                }
                userInfo.DepartmentCode = httpCookie.Values["DepartmentCode"];
                userInfo.DepartmentName = HttpUtility.UrlDecode(httpCookie.Values["DepartmentName"]);

                //if (!string.IsNullOrEmpty(httpCookie.Values["WorkgroupId"]))
                //{
                //    result.WorkgroupId = httpCookie.Values["WorkgroupId"];
                //}
                //else
                //{
                //    result.WorkgroupId = null;
                //}
                //result.WorkgroupCode = httpCookie.Values["WorkgroupCode"];
                //result.WorkgroupName = HttpUtility.UrlDecode(httpCookie.Values["WorkgroupName"]);

                if (!string.IsNullOrEmpty(httpCookie.Values["IsAdministrator"]))
                {
                    userInfo.IsAdministrator = httpCookie.Values["IsAdministrator"].Equals(true.ToString());
                }
                //if (!string.IsNullOrEmpty(httpCookie.Values["SecurityLevel"]))
                //{
                //    result.SecurityLevel = int.Parse(httpCookie.Values["SecurityLevel"]);
                //}
                //result.IPAddress = httpCookie.Values["IPAddress"];
                //result.CurrentLanguage = httpCookie.Values["CurrentLanguage"];
                //result.Themes = httpCookie.Values["Themes"];

                // 只要出错，应该删除Cookie，重新跳转到登录页面才正确
                if (userInfo.UserId <= 0)
                {
                    RemoveUserCookie();
                    userInfo = null;
                }
                else
                {
                    LogUtil.WriteLog("Get UserInfo from Cookie", "UserInfo");
                }
            }
            return userInfo;
        }
        #endregion

        #region public static void SaveCookie(string userName, string password)
        /// <summary>
        /// 保存Cookie
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        public static void SaveCookie(string userName, string password)
        {
            // userName = Encrypt(userName);
            password = Encrypt(password);
            var httpCookie = new HttpCookie(CookieName);
            if (!string.IsNullOrEmpty(CookieDomain))
            {
                httpCookie.Domain = CookieDomain;
            }
            httpCookie.Values[CookieUserName] = userName;
            if (BaseSystemInfo.RememberPassword)
            {
                httpCookie.Values[CookiePassword] = password;
            }
            // 设置过期时间为30天，若需要关闭掉浏览器就要退出程序，这下面的2行代码注释掉就可以了
            if (BaseSystemInfo.CookieExpires != 0)
            {
                var dateTime = DateTime.Now;
                httpCookie.Expires = dateTime.AddDays(BaseSystemInfo.CookieExpires);
            }
            HttpContext.Current.Response.Cookies.Add(httpCookie);
        }
        #endregion

        #region public static void SaveCookie(BaseUserInfo userInfo, bool allInfo = false)
        /// <summary>
        /// 保存Cookie
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="allInfo">是否保存所有的信息</param>
        public static void SaveCookie(BaseUserInfo userInfo, bool allInfo = false)
        {
            //var password = Encrypt(userInfo.Password);
            var httpCookie = new HttpCookie(CookieName);
            if (!string.IsNullOrEmpty(CookieDomain))
            {
                httpCookie.Domain = CookieDomain;
            }
            httpCookie.Values[CookieUserName] = HttpUtility.UrlEncode(userInfo.UserName);
            //if (BaseSystemInfo.RememberPassword)
            //{
            //    httpCookie.Values[CookiePassword] = password;
            //}
            httpCookie.Values["Id"] = userInfo.Id.ToString();
            httpCookie.Values["UserId"] = userInfo.Id.ToString();
            httpCookie.Values["OpenId"] = userInfo.OpenId;
            httpCookie.Values["Code"] = userInfo.Code;
            httpCookie.Values["UserName"] = HttpUtility.UrlEncode(userInfo.UserName);
            httpCookie.Values["RealName"] = HttpUtility.UrlEncode(userInfo.RealName);
            if (HttpContext.Current.Response.Charset.ToLower().Equals("gb2312"))
            {
                var encoding = Encoding.GetEncoding("gb2312");
                httpCookie.Values["RealName"] = HttpUtility.UrlEncode(userInfo.RealName, encoding);
            }
            httpCookie.Values["IsAdministrator"] = userInfo.IsAdministrator.ToString();
            httpCookie.Values["IPAddress"] = userInfo.IpAddress;

            if (allInfo)
            {
                httpCookie.Values["ServiceUserName"] = userInfo.ServiceUserName;
                httpCookie.Values["ServicePassword"] = userInfo.ServicePassword;

                if (userInfo.CompanyId != null)
                {
                    httpCookie.Values["CompanyId"] = userInfo.CompanyId;
                }
                else
                {
                    httpCookie.Values["CompanyId"] = null;
                }
                httpCookie.Values["CompanyCode"] = userInfo.CompanyCode;
                httpCookie.Values["CompanyName"] = HttpUtility.UrlEncode(userInfo.CompanyName);
                httpCookie.Values["DepartmentCode"] = userInfo.DepartmentCode;
                if (userInfo.DepartmentId != null)
                {
                    httpCookie.Values["DepartmentId"] = userInfo.DepartmentId;
                }
                else
                {
                    httpCookie.Values["DepartmentId"] = null;
                }
                httpCookie.Values["DepartmentName"] = HttpUtility.UrlEncode(userInfo.DepartmentName);

                //if (result.WorkgroupId != null)
                //{
                //    httpCookie.Values["WorkgroupId"] = result.WorkgroupId;
                //}
                //else
                //{
                //    httpCookie.Values["WorkgroupId"] = null;
                //}
                // httpCookie.Values["WorkgroupCode"] = result.WorkgroupCode;
                // httpCookie.Values["WorkgroupName"] = HttpUtility.UrlEncode(result.WorkgroupName);

                // httpCookie.Values["SecurityLevel"] = result.SecurityLevel.ToString();
                // httpCookie.Values["StaffId"] = userInfo.StaffId;
                // httpCookie.Values["TargetUserId"] = result.TargetUserId;
                // httpCookie.Values["CurrentLanguage"] = result.CurrentLanguage;
            }

            // httpCookie.Values["Themes"] = result.Themes;
            // 设置过期时间为30天，若需要关闭掉浏览器就要退出程序，这下面的2行代码注释掉就可以了
            if (BaseSystemInfo.CookieExpires != 0)
            {
                var dateTime = DateTime.Now;
                httpCookie.Expires = dateTime.AddDays(BaseSystemInfo.CookieExpires);
            }
            HttpContext.Current.Response.Cookies.Add(httpCookie);
        }
        #endregion

        #endregion

        #region 用OpenId登录部分
        /// <summary>
        /// 授权码登录
        /// </summary>
        /// <param name="authorizationCode"></param>
        /// <param name="transparent"></param>
        /// <param name="useCaching"></param>
        /// <param name="useDataBase"></param>
        /// <param name="useUserCenterHost"></param>
        /// <returns></returns>
        public static UserLogonResult LogonByAuthorizationCode(string authorizationCode, bool transparent = false, bool useCaching = true, bool useDataBase = true, bool useUserCenterHost = true)
        {
            // 统一的登录服务
            UserLogonResult result = null;
            var openId = string.Empty;

            if (BaseUserManager.VerifyAuthorizationCode(null, authorizationCode, out openId))
            {
                result = LogonByOpenId(openId, transparent, useCaching, useDataBase, useUserCenterHost);
            }

            return result;
        }


        #region public static UserLogonResult LogonByOpenId(string openId, bool transparent = false, bool useCaching = true, bool useDataBase = true, bool useUserCenterHost = true)
        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="openId">当点登录识别码</param>
        /// <param name="transparent">是否使用了代理</param>
        /// <param name="useCaching">使用缓存</param>
        /// <param name="useDataBase">使用数据库</param>
        /// <param name="useUserCenterHost">使用接口</param>
        /// <returns>用户登录信息</returns>
        public static UserLogonResult LogonByOpenId(string openId, bool transparent = false, bool useCaching = true, bool useDataBase = true, bool useUserCenterHost = true)
        {
            // 统一的登录服务
            UserLogonResult userLogonResult = null;

            if (useCaching)
            {
                // 先从缓存活取用户是否在？若在缓存里已经在了，就不需要再登录了，直接登录就可以了。
                var result = GetUserInfoCaching(openId);
                if (result != null)
                {
                    userLogonResult = new UserLogonResult
                    {
                        UserInfo = result,
                        Status = Status.Ok,
                        StatusCode = Status.Ok.ToString()
                    };
                    return userLogonResult;
                }
            }

            if (useDataBase)
            {

            }

            if (useUserCenterHost)
            {
                // DotNetService dotNetService = new DotNetService();
                // result = dotNetService.LogonService.LogonByOpenId(GetUserInfo(), openId);
                var url = BaseSystemInfo.UserCenterHost + "/UserCenterV42/LogonService.ashx";
                var webClient = new WebClient();
                var postValues = new NameValueCollection();
                postValues.Add("function", "LogonByOpenId");
                postValues.Add("userInfo", BaseSystemInfo.UserInfo.Serialize());
                postValues.Add("systemCode", BaseSystemInfo.SystemCode);
                // 若ip地址没有传递过来，就获取BS客户端ip地址
                postValues.Add("ipAddress", Utils.GetIp());
                // BS 登录容易引起混乱，
                // postValues.Add("macAddress", BaseSystemInfo.UserInfo.MACAddress);
                postValues.Add("securityKey", BaseSystemInfo.SecurityKey);
                postValues.Add("openId", openId);
                // 向服务器发送POST数据
                var responseArray = webClient.UploadValues(url, postValues);
                var response = Encoding.UTF8.GetString(responseArray);
                if (!string.IsNullOrEmpty(response))
                {
                    userLogonResult = JsonUtil.JsonToObject<UserLogonResult>(response);
                }
                // 检查身份
                if (userLogonResult != null && userLogonResult.Status == Status.Ok)
                {
                    Logon(userLogonResult.UserInfo, false);
                }
            }

            return userLogonResult;
        }
        #endregion

        #endregion

        #region 用用户名密码登录部分

        #region public static BaseUserInfo Logon(string userName, string password, bool checkUserPassword = true)
        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="checkUserPassword">是否要检查用户密码</param>
        public static BaseUserInfo Logon(string userName, string password, bool checkUserPassword = true)
        {
            var userManager = new BaseUserManager(GetUserInfo());
            return userManager.LogonByUserName(userName, password, BaseSystemInfo.SystemCode, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], string.Empty, null, checkUserPassword).UserInfo;
        }
        #endregion

        #region public static BaseUserInfo Logon(string userName, string password, string openId, string permissionCode, bool persistCookie, bool formsAuthentication, out Status status, out string statusMessage)

        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="openId">单点登录标识openId</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="ipAddress"></param>
        /// <param name="systemCode"></param>
        /// <param name="persistCookie">是否保存密码</param>
        /// <param name="formsAuthentication">表单验证，是否需要重定位</param>
        /// <param name="webApiLogin">是否WebApi登录，解决同一请求的Cookie清除无效问题</param>
        /// <param name="status">状态</param>
        /// <param name="statusMessage">返回状态消息</param>
        /// <returns></returns>
        public static BaseUserInfo Logon(string userName, string password, string openId, string permissionCode, string ipAddress, string systemCode, bool persistCookie, bool formsAuthentication, bool webApiLogin, out Status status, out string statusMessage)
        {
            BaseUserInfo result = null;
            status = Status.UserNotFound;
            statusMessage = Status.UserNotFound.ToDescription();

            // 统一的登录服务
            var taskId = Guid.NewGuid().ToString("N");
            var dotNetService = new DotNetService();
            var userInfo = GetUserInfo();
            if (!string.IsNullOrEmpty(ipAddress))
            {
                userInfo.IpAddress = ipAddress;
            }
            if (!string.IsNullOrEmpty(systemCode))
            {
                userInfo.SystemCode = systemCode;
            }
            if (string.IsNullOrEmpty(userInfo.IpAddress))
            {
                userInfo.IpAddress = Utils.GetIp();
            }
            //2020-06-12 WebApi中登录方法中无法先删除Cookie，因为没有返回给客户端。Troy.Cui
            if (webApiLogin)
            {
                userInfo = new BaseUserInfo
                {
                    IpAddress = Utils.GetIp()
                };
            }
            //2020年2月29日，每次登录都强制重新生成OpenId，Troy.Cui
            var userLogonResult = dotNetService.LogonService.UserLogon(taskId, userInfo, userName, password, openId);
            if (userLogonResult != null)
            {
                status = userLogonResult.Status;
                statusMessage = userLogonResult.StatusMessage;
            }
            // 检查身份
            if (userLogonResult != null && userLogonResult.Status == Status.Ok)
            {
                //LogUtil.WriteLog("Logon Ok");

                var isAuthorized = true;
                // 用户是否有哪个相应的权限
                if (!string.IsNullOrEmpty(permissionCode))
                {
                    isAuthorized = dotNetService.PermissionService.IsAuthorized(userLogonResult.UserInfo, permissionCode, null);
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
                }
                else
                {
                    userLogonResult.Status = Status.LogonDeny;
                    userLogonResult.StatusCode = Status.LogonDeny.ToString();
                    userLogonResult.StatusMessage = "访问被拒绝、您的账户没有后台管理访问权限。";
                }
                result = userLogonResult.UserInfo;
            }

            return result;
        }
        #endregion

        #region public static BaseUserInfo LogonByCompany(string companyName, string userName, string password, string openId, string permissionCode, string ipAddress, string systemCode, bool persistCookie, bool formsAuthentication)
        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="companyName">公司</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="openId">OpenId</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="systemCode"></param>
        /// <param name="persistCookie">是否保存密码</param>
        /// <param name="formsAuthentication">表单验证，是否需要重定位</param>
        /// <returns></returns>
        public static BaseUserInfo LogonByCompany(string companyName, string userName, string password, string openId, string permissionCode, string ipAddress, string systemCode, bool persistCookie, bool formsAuthentication)
        {
            var taskId = Guid.NewGuid().ToString("N");
            // 统一的登录服务
            var userInfo = GetUserInfo();
            if (!string.IsNullOrEmpty(ipAddress))
            {
                userInfo.IpAddress = ipAddress;
            }
            if (!string.IsNullOrEmpty(systemCode))
            {
                userInfo.SystemCode = systemCode;
            }
            if (!string.IsNullOrEmpty(userInfo.IpAddress))
            {
                userInfo.IpAddress = Utils.GetIp();
            }
            var dotNetService = new DotNetService();
            var userLogonResult = dotNetService.LogonService.LogonByCompany(taskId, userInfo, companyName, userName, password, openId);
            // 检查身份
            if (userLogonResult.Status == Status.Ok)
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
                        SaveCookie(userName, password);
                        // 内部单点登录方式
                        //SaveCookie(userLogonResult.UserInfo);
                    }
                    else
                    {
                        RemoveUserCookie();
                    }
                    Logon(userLogonResult.UserInfo, formsAuthentication);
                }
                else
                {
                    userLogonResult.Status = Status.LogonDeny;
                    userLogonResult.StatusCode = Status.LogonDeny.ToString();
                    userLogonResult.StatusMessage = "访问被拒绝、您的账户没有后台管理访问权限。";
                }
            }
            return userLogonResult.UserInfo;
        }
        #endregion

        #region public static void Logon(BaseUserInfo userInfo, bool formsAuthentication = false)
        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="userInfo">登录</param>
        /// <param name="formsAuthentication">Forms认证</param>
        public static void Logon(BaseUserInfo userInfo, bool formsAuthentication = false)
        {
            // 检查身份
            if (userInfo != null && userInfo.UserId > 0)
            {
                SetSession(userInfo);
                if (formsAuthentication)
                {
                    FormsAuthentication.RedirectFromLoginPage(CookieName, false);
                }
            }
            else
            {
                Logout(userInfo);
            }
        }
        #endregion

        #endregion

        #region 安全退出部分

        #region public static void RemoveUserCookie()
        /// <summary>
        /// 清空cookie
        /// </summary>
        public static void RemoveUserCookie()
        {
            // 清空cookie
            var httpCookie = new HttpCookie(CookieName);
            // 设置过期时间，1秒钟后删除cookie就不对了,得时间很长才可以服务器时间与客户时间的差距得考虑好
            httpCookie.Expires = DateTime.Now.AddYears(-50);
            if (!string.IsNullOrEmpty(CookieDomain))
            {
                httpCookie.Domain = CookieDomain;
            }
            httpCookie.Values.Clear();
            HttpContext.Current.Response.Cookies.Add(httpCookie);
            //增加一个删除功能，最新版的Google Chrome内核不支持上述的删除了
            HttpContext.Current.Request.Cookies.Remove(CookieName);
        }
        #endregion

        #region public static void RemoveUserSession()
        /// <summary>
        /// 清空cookie
        /// </summary>
        public static void RemoveUserSession()
        {
            // 用户信息清除
            HttpContext.Current.Session[SessionName] = null;
            // 模块菜单信息清除
            // HttpContext.Current.Session["_DTModule"] = null;
        }
        #endregion

        #region public static void Logout(BaseUserInfo userInfo)
        /// <summary>
        /// 退出登录部分
        /// <param name="userInfo">当前用户</param>
        /// </summary>
        public static void Logout(BaseUserInfo userInfo = null)
        {
            // 退出时，需要把用户的操作权限，模块权限清除
            if (userInfo == null)
            {
                userInfo = GetUserCookie();
            }
            if (userInfo == null)
            {
                userInfo = HttpContext.Current.Session[SessionName] as BaseUserInfo;
            }
            if (userInfo != null)
            {
                var cacheKey = "P" + userInfo.Id;
                CacheUtil.Remove(cacheKey);

                // 这里要考虑读写分离的处理
                // IDbHelper dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterWriteDbConnection);
                // BaseUserLogonManager userLogonManager = new BaseUserLogonManager(dbHelper, userInfo);
                // userLogonManager.SignOut(userInfo.Id);
            }

            // 清除Seesion对象
            RemoveUserSession();
            // HttpContext.Current.Session.Abandon();
            // HttpContext.Current.Session.Clear();

            // 清空cookie
            RemoveUserCookie();
            // Session.Abandon();
            // 在此处放置用户代码以初始化页面
            // FormsAuthentication.SignOut();
            // 重新定位到登录页面
            // HttpContext.Current.Response.Redirect("Default.aspx", true);

            //var url = BaseSystemInfo.WebHost + "/UserCenterV42/LogonService.ashx?";
            //var webClient = new WebClient();
            //var postValues = new NameValueCollection();
            //postValues.Add("function", "SignOut");
            //if (userInfo != null) postValues.Add("openId", userInfo.OpenId);
            //postValues.Add("systemCode", BaseSystemInfo.SystemCode);
            //postValues.Add("ipAddress", Utils.GetIp());
            //postValues.Add("securityKey", BaseSystemInfo.SecurityKey);
            //// 向服务器发送POST数据
            //var responseArray = webClient.UploadValues(url, postValues);

            // string response = Encoding.UTF8.GetString(responseArray);
            // if (!string.IsNullOrEmpty(response))
            // {
            //      result = response.Equals(true.ToString(), StringComparison.InvariantCultureIgnoreCase);
            // }
        }
        #endregion

        #endregion
#endif

        #region 密码相关

        #region public static bool ResetPassword(string email, out Status status, out string statusMessage, out string newPassword) 用户忘记密码，发送密码

        /// <summary>
        /// 用户忘记密码，发送密码
        /// </summary>
        /// <param name="email">邮箱地址</param>
        /// <param name="status">状态</param>
        /// <param name="statusMessage">状态信息</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>成功发送密码</returns>
        public static bool ResetPassword(string email, out Status status, out string statusMessage, out string newPassword)
        {
            var result = false;
            // 1.用户是否找到？默认是未找到用户状态
            status = Status.UserNotFound;
            statusMessage = "未找到对应的用户";
            newPassword = RandomUtil.GetRandom(100000, 999999).ToString();

            var userContactManager = new BaseUserContactManager();
            var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserContactEntity.FieldEmail, email)
                };
            var userContactEntity = BaseEntity.Create<BaseUserContactEntity>(userContactManager.GetDataTable(parameters));
            if (userContactEntity != null && userContactEntity.UserId > 0)
            {
                var userManager = new BaseUserManager();
                // 2.用户是否已被删除？
                parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserEntity.FieldId, userContactEntity.UserId),
                    new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
                };
                var userEntity = BaseEntity.Create<BaseUserEntity>(userManager.GetDataTable(parameters));
                // 是否已找到了此用户
                if (userEntity != null && userEntity.Id > 0)
                {
                    // 3.用户是否有效的？
                    if (userEntity.Enabled == 1)
                    {
                        if (userManager.SetPassword(userEntity.Id, newPassword) > 0)
                        {
                            result = true;
                            status = Status.Ok;
                            statusMessage = "新密码已发送到您的注册邮箱" + email + "，请注意查收。";
                        }
                        else
                        {
                            status = Status.ErrorUpdate;
                            statusMessage = "更新数据库失败，请重试！";
                        }
                    }
                    else
                    {
                        if (userEntity.Enabled == 0)
                        {
                            status = Status.UserLocked;
                            statusMessage = "用户被锁定，不允许重置密码。";
                        }
                    }
                }
            }
            return result;
        }
        #endregion

        #region public static bool SendPassword(string userName, out Status status, out string statusMessage) 用户忘记密码，发送密码
        /// <summary>
        /// 用户忘记密码，发送密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="status">状态</param>
        /// <param name="statusMessage">状态信息</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>成功发送密码</returns>
        public static bool SendPassword(string userName, out Status status, out string statusMessage, out string newPassword)
        {
            var result = false;
            // 1.用户是否找到？默认是未找到用户状态
            status = Status.UserNotFound;
            statusMessage = "用户未找到，请重新输入用户名。";
            newPassword = RandomUtil.GetRandom(100000, 999999).ToString();

            var userManager = new BaseUserManager();
            // 2.用户是否已被删除？
            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseUserEntity.FieldUserName, userName));
            parameters.Add(new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0));
            var userEntity = BaseEntity.Create<BaseUserEntity>(userManager.GetDataTable(parameters));

            // 是否已找到了此用户
            if (userEntity != null && userEntity.Id > 0)
            {
                // 3.用户是否有效的？
                if (userEntity.Enabled == 1)
                {

                    //if (!string.IsNullOrEmpty(userEntity.Email))
                    //{
                    //    // 5.重新产生随机密码？
                    //    // 6.发送邮件给用户？
                    //    // 7.重新设置用户密码？
                    //    result = SendPassword(userEntity);
                    //    status = Status.Ok.ToString();
                    //    statusMessage = "新密码已发送到您的注册邮箱" + userEntity.Email + "。";
                    //}
                    //else
                    //{
                    //    // 4.用户是否有邮件账户？
                    //    status = Status.UserNotEmail.ToString();
                    //    statusMessage = "用户没有电子邮件地址，无法从新设置密码，请您及时联系系统管理员。";
                    //}

                }
                else
                {
                    if (userEntity.Enabled == 0)
                    {
                        status = Status.UserLocked;
                        statusMessage = "用户被锁定，不允许设置密码。";
                    }
                    else
                    {
                        status = Status.UserNotActive;
                        statusMessage = "用户还未被激活，不允许设置密码。";
                    }
                }
            }

            return result;
        }
        #endregion

        #endregion

        #region 字符串加密解密部分

        #region public static string Encrypt(string targetValue) DES数据加密
        /// <summary>
        /// DES数据加密
        /// </summary>
        /// <param name="targetValue">目标字段</param>
        /// <returns>加密</returns>
        public static string Encrypt(string targetValue)
        {
            return Encrypt(targetValue, "DotNet");
        }
        #endregion

        #region private static string Encrypt(string targetValue, string key) DES数据加密
        /// <summary>
        /// DES数据加密
        /// </summary>
        /// <param name="targetValue">目标值</param>
        /// <param name="key">密钥</param>
        /// <returns>加密值</returns>
        private static string Encrypt(string targetValue, string key)
        {
            return SecretUtil.DesEncrypt(targetValue, key);
        }
        #endregion

        #region public static string Decrypt(string targetValue) DES数据解密
        /// <summary>
        /// DES数据解密
        /// </summary>
        /// <param name="targetValue">目标字段</param>
        /// <returns>解密</returns>
        public static string Decrypt(string targetValue)
        {
            return Decrypt(targetValue, "DotNet");
        }
        #endregion

        #region private static string Decrypt(string targetValue, string key) DES数据解密
        /// <summary>
        /// DES数据解密
        /// </summary>
        /// <param name="targetValue"></param>
        /// <param name="key">键</param>
        /// <returns></returns>
        private static string Decrypt(string targetValue, string key)
        {
            return SecretUtil.DesDecrypt(targetValue, key);
        }
        #endregion

        #endregion
    }
}