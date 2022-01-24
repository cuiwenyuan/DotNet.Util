//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------
#if NET40_OR_GREATER
using System.Web;
#endif

namespace DotNet.Business
{
    using Util;

    /// <summary>
    /// 登录功能相关部分
    /// </summary>
    public partial class WebUtil
    {
#if NET40_OR_GREATER
        #region public static void Logout(bool useSso, BaseUserInfo userInfo)
        /// <summary>
        /// <param name="useSso">是否使用SSO</param>
        /// 退出登录部分
        /// <param name="userInfo">当前用户</param>
        /// </summary>
        public static void Logout(bool useSso, BaseUserInfo userInfo = null)
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
                //Troy.Cui 2018-09-20，更新数据库中的UserOnline
                //Troy.Cui 2019-04-13, 更新OpenId
                new BaseUserLogonManager(userInfo).SignOut(userInfo.OpenId, userInfo.SystemCode, Utils.GetIp());
                //这里是缓存的Permission
                var cacheKey = "P" + userInfo.Id;
                CacheUtil.Remove(cacheKey);
                //在线用户
                cacheKey = "OnlineUserName." + userInfo.UserName;
                CacheUtil.Remove(cacheKey);
                //组织机构
                cacheKey = "DataTable.BaseOrganizationTree";
                CacheUtil.Remove(cacheKey);
                //菜单
                cacheKey = "DataTable." + BaseSystemInfo.SystemCode + ".ModuleTree";
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
            if (useSso)
            {
                //var url = BaseSystemInfo.WebHost + "/UserCenterV42/LogonService.ashx?";
                //var webClient = new WebClient();
                //var postValues = new NameValueCollection();
                //postValues.Add("function", "SignOut");
                //postValues.Add("openId", userInfo.OpenId);
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
        }
        #endregion
#endif
    }
}