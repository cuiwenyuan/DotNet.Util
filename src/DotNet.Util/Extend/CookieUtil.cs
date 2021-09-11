#if NET40
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DotNet.Util
{
    /// <summary>
    /// Cookie操作类
    /// </summary>
    public partial class CookieUtil
    {

        #region Set
        /// <summary>
        /// 设置Cookie值
        /// </summary>
        /// <param name="cookieName">Cookie名称</param>
        /// <param name="cookieValue">Cookie名称对应的值</param>
        /// <param name="domainName">域名</param>
        /// <param name="expireDays">失效期日(天)</param>
        /// <param name="httpOnly">客户端能否操作Cookie</param>
        public static void Set(string cookieName, string cookieValue, string domainName, int expireDays = 1, bool httpOnly = false)
        {
            // 先尝试获取是否已经存在指定的Cookie
            var cookie = HttpContext.Current.Request.Cookies[cookieName];

            // 没有的话就新创建
            if (cookie == null)
            {
                cookie = new HttpCookie(cookieName);
            }
            else
            {
                //强制使用SSL传输
                //cookie.Secure = true;

                // 存在的话就设定新值
                cookie.Value = cookieValue;
                // 设定失效时间
                if (expireDays != 0)
                {
                    cookie.Expires = DateTime.Now.AddDays(expireDays);
                }
                // 指定客户端脚本是否可以访问[默认为false]
                cookie.HttpOnly = httpOnly;
                // 指定统一的Path，比便能通存通取
                cookie.Path = "/";
                // 设置跨域,这样在其它二级域名下就都可以访问到了，千万注意一定要.domain.com的形式，.不能少
                cookie.Domain = domainName;
                // 通过覆盖的方法
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        
        }

        #endregion

        #region Get
        /// <summary>
        /// 获取Cookie值
        /// </summary>
        /// <param name="cookieName">Cookie名称</param>
        public static string Get(string cookieName)
        {
            var cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                return cookie.Value;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Clear
        /// <summary> 
        /// 清除Cookie，使用覆盖并加有效期的方式
        /// </summary> 
        /// <param name="cookieName">Cookie名称</param>
        public static void Clear(string cookieName)
        {
            // 先尝试获取是否已经存在指定的Cookie
            var cookie = HttpContext.Current.Request.Cookies[cookieName];

            // 如果存在就删除
            if (cookie != null)
            {
            // 这个不会删除客户端的Cookie的！
            //HttpContext.Current.Response.Cookies.Remove(cookieName);
            // 使用覆盖的方法来删除
            // 先设定Cookie值
            cookie.Value = null;
            // 设定过期时间（50年前），以防万一
            cookie.Expires = DateTime.Now.AddYears(-50);
            HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        #endregion

        #region ClearAll
        /// <summary>
        /// 清除所有Cookie
        /// </summary>
        public static void ClearAll()
        {
            // 清除所有Cookie,并不能清除客户端的Cookie
            //HttpContext.Current.Response.Cookies.Clear();
            // 使用遍历并覆盖的方式
            HttpCookie cookie;
            string cookieName;
            var limit = HttpContext.Current.Request.Cookies.Count;
            for (var i = 0; i < limit; i++)
            {
                cookieName = HttpContext.Current.Request.Cookies[i].Name;
                cookie = new HttpCookie(cookieName);
                cookie.Value = null;
                cookie.Expires = DateTime.Now.AddYears(-50);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        #endregion

        #region GetAll
        /// <summary>
        /// 获得当前网站下的所有Cookies
        /// </summary>
        public static void GetAll()
        {
            HttpCookie cookie;
            string subKeyName;
            string subKeyValue;
            var limit = HttpContext.Current.Request.Cookies.Count;
            for (var i = 0; i < limit; i++)
            {
                cookie = HttpContext.Current.Request.Cookies[i];
                if (cookie.HasKeys)
                {
                    for (var j = 0; j < cookie.Values.Count - 1; j++)
                    {
                        subKeyName = HttpContext.Current.Server.HtmlEncode(cookie.Values.AllKeys[j]);
                        subKeyValue = HttpContext.Current.Server.HtmlEncode(cookie.Values[j]);
                        HttpContext.Current.Response.Write("subKeyName = " + subKeyName + ", subKeyValue = " + subKeyValue + "<br />");
                    }
                }
                else
                    HttpContext.Current.Response.Write(cookie.Name + " " + cookie.Value + "<br />");
            }


        }
        #endregion

    }
}
#endif