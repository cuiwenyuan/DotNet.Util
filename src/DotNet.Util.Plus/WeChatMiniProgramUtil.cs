using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using Newtonsoft.Json;
#if NETSTANDARD2_0_OR_GREATER
using Microsoft.Net.Http;
using Microsoft.Net.Http.Headers;
#endif

namespace DotNet.Util
{
    /// <summary>
    /// 微信小程序工具
    /// </summary>
    public partial class WeChatMiniProgramUtil
    {
#region GetQrCode
        /// <summary>
        /// 获取带参数小程序码并保存到服务器上，返回小程序码图片路径
        /// </summary>
        /// <param name="typeId">1：普通二维码（无参数），2：微信小程序码有数量限制（可把参数放path里面），3：微信小程序码无数量限制（scene传参数，开发后台须配置普通链接二维码规则）。客户端6.5.7版本以上才可识别小程序码。</param>
        /// <param name="path">页面，注意pages前没有/，最大长度128字节，不能为空</param>
        /// <param name="width">默认屏幕尺寸px，最小280，最大1280。（请按照43像素的整数倍设置）</param>
        /// <param name="scene">TypeId为3时才生效！传参数如：id=1，最大32个可见字符，只支持数字，大小写英文以及部分特殊字符：!#$&amp;'()*+,/:;=?@-._~，其它字符请自行编码为合法字符（因不支持%，中文无法使用 urlencode 处理，请使用其他编码方式）</param>
        /// <param name="autoColor">TypeId为2、3时才生效！自动配置线条颜色，如果颜色依然是黑色，则说明不建议配置主色调</param>
        /// <param name="lineColorR">TypeId为2、3时才生效！auto_color 为 false 时生效，使用 rgb 设置颜色，十进制表示</param>
        /// <param name="lineColorG">TypeId为2、3时才生效！auto_color 为 false 时生效，使用 rgb 设置颜色，十进制表示</param>
        /// <param name="lineColorB">TypeId为2、3时才生效！auto_color 为 false 时生效，使用 rgb 设置颜色，十进制表示</param>
        /// <param name="isHyaline">TypeId为2、3时才生效！是否需要透明底色，为 true 时，生成透明底色的小程序码</param>
        /// <returns></returns>
        public static string GetQrCode(int typeId, string path = "pages/default/default", string scene = null, int width = 430, bool autoColor = false, string lineColorR = null, string lineColorG = null, string lineColorB = null, bool isHyaline = false)
        {
            var result = string.Empty;
            var accessToken = GetAccessToken();

            if (string.IsNullOrEmpty(lineColorR))
            {
                lineColorR = "0";
            }
            if (string.IsNullOrEmpty(lineColorG))
            {
                lineColorG = "0";
            }
            if (string.IsNullOrEmpty(lineColorB))
            {
                lineColorB = "0";
            }

            if (!string.IsNullOrEmpty(accessToken))
            {
                //默认1：普通微信二维码
                var postUrl = $"https://api.weixin.qq.com/cgi-bin/wxaapp/createwxaqrcode?access_token={accessToken}";

                switch (typeId)
                {
                    case 1:
                        //普通微信二维码
                        postUrl = $"https://api.weixin.qq.com/cgi-bin/wxaapp/createwxaqrcode?access_token={accessToken}";
                        var data1 = new
                        {
                            access_token = accessToken,
                            path = path,
                            width = width

                        };
                        result = DownloadBufferImage(postUrl, JsonConvert.SerializeObject(data1));
                        break;
                    case 2:
                        //微信小程序码（有数量限制）
                        postUrl = $"https://api.weixin.qq.com/wxa/getwxacode?access_token={accessToken}";
                        var data2 = new
                        {
                            path = path,
                            width = width,
                            auto_color = autoColor,
                            line_color = new { r = lineColorR, g = lineColorG, b = lineColorB },
                            is_hyaline = isHyaline
                        };
                        var json2 = JsonConvert.SerializeObject(data2);
                        result = DownloadBufferImage(postUrl, json2);
                        break;
                    case 3:
                        //微信小程序码（无数量限制）,需要提前设置：开发》配置普通链接二维码规则
                        postUrl = $"https://api.weixin.qq.com/wxa/getwxacodeunlimit?access_token={accessToken}";
                        //注意page = path，很特别！
                        var data3 = new
                        {
                            scene = scene,
                            page = path,
                            width = width,
                            auto_color = autoColor,
                            line_color = new { r = lineColorR, g = lineColorG, b = lineColorB },
                            is_hyaline = isHyaline
                        };
                        var json3 = JsonConvert.SerializeObject(data3);
                        result = DownloadBufferImage(postUrl, JsonConvert.SerializeObject(json3));
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// 根据当前日期 判断Access_Token 是否超期  如果超期返回新的Access_Token，否则返回之前的Access_Token  
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken()
        {

            var token = string.Empty;
            var expirationTime = DateTime.Now;
            // 读取XML文件中的数据，并显示出来 ，注意文件路径

            var appid = "";
            var secret = "";

            var filePath = Utils.GetMapPath("~/xmlconfig/WeChatMiniProgram.config");
            var doc = new XmlDocument();
            doc.Load(filePath);
            appid = doc.SelectSingleNode(@"Root/appid")?.InnerText;
            secret = doc.SelectSingleNode(@"Root/secret")?.InnerText;

            token = doc.SelectSingleNode(@"Root/Access_Token")?.InnerText;
            var expirationTimeString = doc.SelectSingleNode(@"Root/Access_ExpirationTime")?.InnerText;
            if (ValidateUtil.IsDateTime(expirationTimeString))
            {
                expirationTime = Convert.ToDateTime(expirationTimeString);
            }

            if (!string.IsNullOrEmpty(appid) && !string.IsNullOrEmpty(secret) && (string.IsNullOrEmpty(token) || DateTime.Now > expirationTime))
            {
                expirationTime = DateTime.Now;
                var strUrl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + secret;
                var model = new AccessToken();

                var returnContent = HttpRequestUtil.HwGet(strUrl);

                var accessToken = JsonUtil.JsonToObject<AccessToken>(returnContent);
                model.access_token = accessToken.access_token;
                model.expires_in = accessToken.expires_in;

                doc.SelectSingleNode(@"Root/Access_Token").InnerText = model.access_token;
                expirationTime = expirationTime.AddSeconds(int.Parse(model.expires_in));
                doc.SelectSingleNode(@"Root/Access_ExpirationTime").InnerText = expirationTime.ToString("yyyy-MM-dd HH:mm:ss:ffff");
                doc.Save(filePath);
                token = model.access_token;
            }
            return token;
        }

        /// <summary>
        /// 保存接口返回二进制流为文件方法
        /// </summary>
        /// <param name="requestUri">接口地址</param>
        /// <param name="jsonString">json数据对象</param>
        /// <returns></returns>
        public static string DownloadBufferImage(string requestUri, string jsonString)
        {
            var result = string.Empty;

#if NETSTANDARD2_0_OR_GREATER
            try
            {
                HttpContent httpContent = new StringContent(jsonString);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                //注意大量并发的时候不能释放资源的问题
                using (var httpClient = new HttpClient())
                {
                    //停止使用using，使用HttpClientFactory
                    //var httpClient = HttpClientFactory2.GetHttpClient();

                    httpClient.PostAsync(requestUri, httpContent).ContinueWith(
                   (requestTask) =>
                   {
                       var response = requestTask.Result;

                       response.EnsureSuccessStatusCode();

                       var contentType = response.Content.Headers.ContentType;
                       if (string.IsNullOrEmpty(contentType.CharSet))
                       {
                           contentType.CharSet = "utf-8";
                       }
                       LogUtil.WriteLog(response.Content.ReadAsByteArrayAsync().Result.LongLength.ToString(), "QRCode");

                       var data = response.Content.ReadAsByteArrayAsync().Result;

                       var fileName = string.Empty;

                       //会出现113个字节的内容，所以放大十倍
                       if (data.Length > 1130)
                       {
                           fileName = Guid.NewGuid().ToString("D") + ".jpg";
                           var filePath = string.Format(@"{0}\QRCode\" + fileName, AppDomain.CurrentDomain.SetupInformation.ApplicationBase);

                           var folder = Path.GetDirectoryName(filePath);
                           if (!Directory.Exists(folder))
                           {
                               Directory.CreateDirectory(folder);
                           }

                           using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                           {
                               fs.Write(data, 0, data.Length);
                               fs.Flush();
                               fs.Close();
                           }

                           result = "/QRCode/" + fileName;
                       }
                       else
                       {
                           LogUtil.WriteLog(Encoding.Default.GetString(data), "QRCode");
                       }

                   }).Wait(5000);
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
            }
#endif
            return result;
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
#endregion

    }
}
