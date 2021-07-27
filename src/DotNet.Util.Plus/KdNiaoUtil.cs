using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DotNet.Util
{
    /// <summary>
    /// 阿里短信发送
    /// </summary>
    public static class KdNiaoUtil
    {
        //电商ID
        private const string FieldEBusinessId = "1549187";
        //电商加密私钥，快递鸟提供，注意保管，不要泄漏
        private const string FieldAppKey = "97007c89-3ddd-4efd-adbd-f9ec0025e8dd";
        //请求url
        private const string FieldReqUrl = "http://api.kdniao.com/Ebusiness/EbusinessOrderHandle.aspx";

        /// <summary>
        /// Json方式 查询订单物流轨迹
        /// </summary>
        /// <returns></returns>
        public static string GetOrderTracesByJson(string shipperCode, string logisticCode)
        {
            var requestData = "{'OrderCode':'','ShipperCode':'" + shipperCode + "','LogisticCode':'" + logisticCode + "'}";

            var param = new Dictionary<string, string>
            {
                {"RequestData", HttpUtility.UrlEncode(requestData, Encoding.UTF8)},
                {"EBusinessID", FieldEBusinessId},
                {"RequestType", "1002"}
            };
            var dataSign = Encrypt(requestData, FieldAppKey, "UTF-8");
            param.Add("DataSign", HttpUtility.UrlEncode(dataSign, Encoding.UTF8));
            param.Add("DataType", "2");

            var result = HttpPost(FieldReqUrl, param);

            //根据公司业务处理返回的信息......

            return result;
        }

        /// <summary>
        /// Post方式提交数据，返回网页的源代码
        /// </summary>
        /// <param name="url">发送请求的 URL</param>
        /// <param name="param">请求的参数集合</param>
        /// <returns>远程资源的响应结果</returns>
        private static string HttpPost(string url, Dictionary<string, string> param)
        {
            var result = "";
            var postData = new StringBuilder();
            if (param != null && param.Count > 0)
            {
                foreach (var p in param)
                {
                    if (postData.Length > 0)
                    {
                        postData.Append("&");
                    }
                    postData.Append(p.Key);
                    postData.Append("=");
                    postData.Append(p.Value);
                }
            }
            var byteData = Encoding.GetEncoding("UTF-8").GetBytes(postData.ToString());
            try
            {

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Referer = url;
                request.Accept = "*/*";
                request.Timeout = 30 * 1000;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
                request.Method = "POST";
                request.ContentLength = byteData.Length;
                var stream = request.GetRequestStream();
                stream.Write(byteData, 0, byteData.Length);
                stream.Flush();
                stream.Close();
                var response = (HttpWebResponse)request.GetResponse();
                var backStream = response.GetResponseStream();
                var sr = new StreamReader(backStream, Encoding.GetEncoding("UTF-8"));
                result = sr.ReadToEnd();
                sr.Close();
                backStream.Close();
                response.Close();
                request.Abort();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        ///<summary>
        ///电商Sign签名
        ///</summary>
        ///<param name="content">内容</param>
        ///<param name="keyValue">Appkey</param>
        ///<param name="charset">URL编码 </param>
        ///<returns>DataSign签名</returns>
        private static string Encrypt(string content, string keyValue, string charset)
        {
            if (keyValue != null)
            {
                return Base64(Md5(content + keyValue, charset), charset);
            }
            return Base64(Md5(content, charset), charset);
        }

        ///<summary>
        /// 字符串MD5加密
        ///</summary>
        ///<param name="str">要加密的字符串</param>
        ///<param name="charset">编码方式</param>
        ///<returns>密文</returns>
        private static string Md5(string str, string charset)
        {
            var buffer = System.Text.Encoding.GetEncoding(charset).GetBytes(str);
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider check;
                check = new System.Security.Cryptography.MD5CryptoServiceProvider();
                var somme = check.ComputeHash(buffer);
                var ret = "";
                foreach (var a in somme)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("X");
                    else
                        ret += a.ToString("X");
                }
                return ret.ToLower();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// base64编码
        /// </summary>
        /// <param name="str">内容</param>
        /// <param name="charset">编码方式</param>
        /// <returns></returns>
        private static string Base64(string str, string charset)
        {
            return Convert.ToBase64String(System.Text.Encoding.GetEncoding(charset).GetBytes(str));
        }
    }
}
