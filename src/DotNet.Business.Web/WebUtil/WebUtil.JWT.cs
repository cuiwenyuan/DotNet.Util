//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace DotNet.Business
{
    using Util;
    using Model;
    using JWT.Algorithms;
    using JWT;
    using JWT.Serializers;
    using JWT.Exceptions;

    public partial class WebUtil
    {
        #region GenerateJwt 生成JWT Web Token
        /// <summary>
        /// 生成JWT Web Token
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="openId">OpenId</param>
        /// <param name="hours">过期小时数（默认24小时）</param>
        /// <returns></returns>
        public static string GenerateJwt(int userId, string openId, int hours = 24)
        {
            var result = string.Empty;

            var exp = (DateTime.Now.AddHours(hours).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            var payload = new Dictionary<string, object>
            {
                { "exp", exp},
                { "userId", userId },
                { "openId", openId }
            };

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            result = encoder.Encode(payload, BaseSystemInfo.JwtSecret);

            return result;
        }
        #endregion

        #region DecodeJwt 解析JWT Web Token
        /// <summary>
        /// 解析JWT Web Token
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="json">解析出的Json</param>
        /// <param name="userId">用户编号</param>
        /// <param name="openId">OpenId</param>
        /// <returns></returns>
        public static bool DecodeJwt(string token, out string json, out int userId, out string openId)
        {
            var result = false;
            json = string.Empty;
            userId = 0;
            openId = string.Empty;
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    IJsonSerializer serializer = new JsonNetSerializer();
                    IDateTimeProvider provider = new UtcDateTimeProvider();
                    IJwtValidator validator = new JwtValidator(serializer, provider);
                    IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                    IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
                    IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

                    json = decoder.Decode(token, BaseSystemInfo.JwtSecret, verify: true);
                    var payload = decoder.DecodeToObject<IDictionary<string, object>>(token, BaseSystemInfo.JwtSecret, verify: true);
                    if (payload.ContainsKey("userId"))
                    {
                        userId = payload["userId"].ToInt();
                    }
                    if (payload.ContainsKey("openId"))
                    {
                        openId = payload["openId"].ToString();
                    }
                    result = true;
                }
                catch (TokenExpiredException)
                {
                    LogUtil.WriteLog("Token has expired");
                }
                catch (SignatureVerificationException)
                {
                    LogUtil.WriteLog("Token has invalid signature");
                }
                catch (Exception)
                {
                    LogUtil.WriteLog("Token is empty");
                }
            }

            return result;
        }
        #endregion
    }
}