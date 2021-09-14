//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// 登录功能相关部分
    /// </summary>
    public partial class Utilities
    {
        /// <summary>
        /// 端口
        /// </summary>
        public static int Port = 88;
        /// <summary>
        /// 网址
        /// </summary>
        public static string Url = "redis.ztosys.com:6379";
        /// <summary>
        /// 验证OpenId
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static bool ValidateOpenId(string openId)
        {
            var result = false;
            if (!string.IsNullOrWhiteSpace(openId))
            {
                var userId = CacheUtil.Get<string>("openId" + openId);
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    result = true;
                }
            }

            if (!result)
            {
                var url = BaseSystemInfo.UserCenterHost + "UserCenterV42/LogOnService.ashx";
                var webClient = new WebClient();
                var postValues = new NameValueCollection();
                postValues.Add("function", "ValidateOpenId");
                postValues.Add("systemCode", BaseSystemInfo.SystemCode);
                postValues.Add("ipAddress", Utils.GetIp());
                postValues.Add("securityKey", BaseSystemInfo.SecurityKey);
                postValues.Add("openId", openId);
                // 向服务器发送POST数据
                var responseArray = webClient.UploadValues(url, postValues);
                var response = Encoding.UTF8.GetString(responseArray);
                if (!string.IsNullOrEmpty(response))
                {
                    result = response.Equals(true.ToString(), StringComparison.InvariantCultureIgnoreCase);
                }
            }
            return result;
        }
    }
}