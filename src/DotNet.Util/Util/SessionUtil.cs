using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if NET40_OR_GREATER
using System.Web;
#elif NETSTANDARD2_0_OR_GREATER
using Microsoft.AspNetCore.Session;
#endif
namespace DotNet.Util
{
    /// <summary>
    /// Session操作类
    /// </summary>
    public partial class SessionUtil
    {
        ////session 设置
        //services.AddSession(options =>
        //{
        //    // 设置 Session 过期时间
        //    options.IdleTimeout = TimeSpan.FromDays(90);
        //    options.CookieHttpOnly = true;
        //});

#if NET40_OR_GREATER
        #region Set
        /// <summary>
        /// 设置Session值
        /// </summary>
        /// <param name="sessionName">Session名称</param>
        /// <param name="sessionValue">Session名称对应的值</param>
        /// <param name="timeout"></param>
        public static void Set(string sessionName, object sessionValue, int timeout = 20)
        {
            // 默认过期
            HttpContext.Current.Session.Timeout = timeout;
            HttpContext.Current.Session[sessionName] = sessionValue;
            //if (sessionValue.ToString().Length == 0)
            //{
            //    HttpContext.Current.Session.Remove(sessionName);
            //}
        }
        #endregion

        #region Get
        /// <summary>
        /// 获取Session值
        /// </summary>
        /// <param name="sessionName">Session名称</param>
        public static string Get(string sessionName)
        {
            if (HttpContext.Current.Session[sessionName] != null)
            {
                return Convert.ToString(HttpContext.Current.Session[sessionName]);
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Clear
        /// <summary>
        /// 清除Session
        /// </summary>
        public static void Clear(string sessionName)
        {
            HttpContext.Current.Session.Remove(sessionName);
        }
        #endregion

        #region ClearAll
        /// <summary>
        /// 清除所有Session
        /// </summary>
        public static void ClearAll()
        {
            HttpContext.Current.Session.Abandon();
            HttpContext.Current.Session.Clear();
        }
        #endregion
#endif
    }
}
