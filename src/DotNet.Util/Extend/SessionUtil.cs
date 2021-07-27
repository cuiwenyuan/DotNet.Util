using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Session;

namespace DotNet.Utilities
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

        #region Set
        /// <summary>
        /// 设置Session值
        /// </summary>
        /// <param name="sessionName">Session名称</param>
        /// <param name="sessionValue">Session名称对应的值</param>
        public static void Set(string sessionName, object sessionValue)
        {
            //默认过期
            HttpContext.Current.Session[sessionName] = sessionValue;
            //if (sessionValue.ToString().Length == 0)
            //{
            //    HttpContext.Current.Session.Remove(sessionName);
            //}
            HttpContext.Session.SetString("Key", Value);
        }
        #endregion

        #region Get
        /// <summary>
        /// 获取Session值
        /// </summary>
        /// <param name="SessionName">Session名称</param>
        /// <param name="SessionValue">Session名称对应的值</param>
        public static string Get(string sessionName)
        {
            if (HttpContext.Current.Session[sessionName] != null)
            {
                return Convert.ToString(HttpContext.Current.Session[sessionName]);
            }
            else
            {
                return sessionex;
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
    }
}
