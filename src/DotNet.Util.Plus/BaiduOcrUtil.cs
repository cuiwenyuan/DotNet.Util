using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace DotNet.Util
{
    public class BaiduOcrUtil
    {

        // 调用getAccessToken()获取的 access_token建议根据expires_in 时间 设置缓存
        // 百度云中开通对应服务应用的 API Key 建议开通应用的时候多选服务
        private static string clientId = "W7RZgLSjpG9oMnW2zn79Rddq";
        // 百度云中开通对应服务应用的 Secret Key
        private static string clientSecret = "1XNVhalVWrKmj2384bgDw1Y9j1FE0zTa";

        //public static string GetAccessToken()
        //{
        //    var authHost = "https://aip.baidubce.com/oauth/2.0/token";
        //    var client = new HttpClient();
        //    var paraList = new List<KeyValuePair<string, string>>
        //    {
        //        new KeyValuePair<string, string>("grant_type", "client_credentials"),
        //        new KeyValuePair<string, string>("client_id", clientId),
        //        new KeyValuePair<string, string>("client_secret", clientSecret)
        //    };

        //    var response = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
        //    var result = response.Content.ReadAsStringAsync().Result;
        //    Console.WriteLine(result);
        //    return result;
        //}

        /// <summary>
        /// 根据当前日期 判断Access_Token 是否超期  如果超期返回新的Access_Token，否则返回之前的Access_Token  
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken()
        {

            var token = string.Empty;
            var expirationTime = DateTime.Now;
            // 读取XML文件中的数据，并显示出来 ，注意文件路径

            var apiKey = "";
            var secretKey = "";

            var filePath = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), @"xmlconfig\BaiduAI.config");
            var doc = new XmlDocument();
            try
            {
                doc.Load(filePath);
                apiKey = doc.SelectSingleNode(@"Root/apiKey")?.InnerText;
                secretKey = doc.SelectSingleNode(@"Root/secretKey")?.InnerText;

                token = doc.SelectSingleNode(@"Root/Access_Token")?.InnerText;
                var expirationTimeString = doc.SelectSingleNode(@"Root/Access_ExpirationTime")?.InnerText;
                if (ValidateUtil.IsDateTime(expirationTimeString))
                {
                    expirationTime = expirationTimeString.ToDateTime();
                }

                if (!string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(secretKey) && (string.IsNullOrEmpty(token) || DateTime.Now > expirationTime))
                {
                    expirationTime = DateTime.Now;

                    var authHost = "https://aip.baidubce.com/oauth/2.0/token";
                    var client = new HttpClient();
                    var paraList = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("grant_type", "client_credentials"),
                        new KeyValuePair<string, string>("client_id", clientId),
                        new KeyValuePair<string, string>("client_secret", clientSecret)
                    };

                    var response = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
                    var returnContent = response.Content.ReadAsStringAsync().Result;

                    var accessToken = JsonUtil.JsonToObject<AccessToken>(returnContent);

                    var model = new AccessToken();
                    model.access_token = accessToken.access_token;
                    model.expires_in = accessToken.expires_in;

                    doc.SelectSingleNode(@"Root/Access_Token").InnerText = model.access_token;
                    expirationTime = expirationTime.AddSeconds(int.Parse(model.expires_in));
                    doc.SelectSingleNode(@"Root/Access_ExpirationTime").InnerText = expirationTime.ToString("yyyy-MM-dd HH:mm:ss:ffff");
                    doc.Save(filePath);
                    token = model.access_token;

                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
            }
            return token;
        }

        /// <summary>
        /// 通用文字识别
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        /// <returns></returns>
        public static string GeneralBasic(string imagePath)
        {
            var token = GetAccessToken();
            var host = "https://aip.baidubce.com/rest/2.0/ocr/v1/general_basic?access_token=" + token;
            var encoding = Encoding.Default;
            var request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            // 图片的base64编码
            var base64 = GetFileBase64(imagePath);
            var str = "image=" + HttpUtility.UrlEncode(base64);
            var buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            var result = reader.ReadToEnd();
            Console.WriteLine("通用文字识别:");
            Console.WriteLine(result);
            return result;
        }

        /// <summary>
        /// 通用文字识别（高精度版）
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        /// <returns></returns>
        public static string AccurateBasic(string imagePath)
        {
            var token = GetAccessToken();
            var host = "https://aip.baidubce.com/rest/2.0/ocr/v1/accurate_basic?access_token=" + token;
            var encoding = Encoding.Default;
            var request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;
            // 图片的base64编码
            var base64 = GetFileBase64(imagePath);
            var str = "image=" + HttpUtility.UrlEncode(base64);
            var buffer = encoding.GetBytes(str);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            var result = reader.ReadToEnd();
            Console.WriteLine("通用文字识别（高精度版）:");
            Console.WriteLine(result);
            return result;
        }
        /// <summary>
        /// 获取文件Base64
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static string GetFileBase64(string filePath)
        {
            var fs = new FileStream(filePath, FileMode.Open);
            var arr = new byte[fs.Length];
            fs.Read(arr, 0, (int)fs.Length);
            var baser64 = Convert.ToBase64String(arr);
            fs.Close();
            return baser64;
        }

        /// <summary>  
        ///Access_token 的摘要说明  
        /// </summary>  
        public class AccessToken
        {
            /// <summary>  
            /// 获取到的凭证   
            /// </summary>  
            public string access_token { get; set; }

            /// <summary>  
            /// 凭证有效时间，单位：秒  
            /// </summary>  
            public string expires_in { get; set; }
        }
    }
}
