//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2026, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
#if NET46_OR_GREATER
using System.Web;
using System.Web.Configuration;
#endif

namespace DotNet.Business
{
    using Util;

    /// <summary>
    /// 登录功能相关部分
    /// 
    /// 修改记录
    /// 
    ///		2015.11.20 版本：1.0 JiRiGaLa 进行改进。
    ///		
    /// </summary>
    public partial class WebUtil
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="cachingSystemCode"></param>
        /// <returns></returns>
        public static BaseUserInfo GetUserInfoCaching(string openId, string cachingSystemCode = null)
        {
            BaseUserInfo result = null;

            var key = string.Empty;
            if (cachingSystemCode.IsNullOrEmpty())
            {
                key = "openId:" + openId;
            }
            else
            {
                key = "openId:" + cachingSystemCode + ":" + openId;
            }
            var userId = CacheUtil.Get<string>(key);
            if (!userId.IsNullOrEmpty())
            {
                key = "userInfo:" + userId;
                result = CacheUtil.Get<BaseUserInfo>(key);
            }
            return result;
        }
    }
}