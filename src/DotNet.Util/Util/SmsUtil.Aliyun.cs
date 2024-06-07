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
    /// 阿里云短信发送
    /// </summary>
    public class SmsUtil
    {
        #region 发送短信
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="message"></param>
        /// <param name="mobile">手机号码</param>
        /// <param name="json">参数JSON字符串</param>
        /// <param name="templateCode">模板编码</param>
        /// <param name="serviceUrl">服务网址</param>
        /// <param name="accessKeyId">accessKeyId</param>
        /// <param name="accessKeySecret">accessKeySecret</param>
        /// <param name="signName">签名</param>
        /// <param name="param">参数（建议Dictionary<string, object>）</param>
        /// <returns></returns>
        public static bool Send(out string message, string mobile, string json, string templateCode, string serviceUrl = null, string accessKeyId = null, string accessKeySecret = null, string signName = null, object param = null)
        {
            var result = false;
            if (!ValidateUtil.IsMobile(mobile))
            {
                message = "手机号码有误！";
                return false;
            }
            var xmlConfigUtil = new XmlConfigUtil("XmlConfig\\Sms.config");
            if (serviceUrl.IsNullOrEmpty())
            {
                serviceUrl = xmlConfigUtil.GetValue("ServiceUrl", defaultValue: "http://dysmsapi.aliyuncs.com", nodeName: "AliyunSMS");
            }
            if (accessKeyId.IsNullOrEmpty())
            {
                accessKeyId = xmlConfigUtil.GetValue("AccessKeyId", nodeName: "AliyunSMS");
            }
            if (accessKeySecret.IsNullOrEmpty())
            {
                accessKeySecret = xmlConfigUtil.GetValue("AccessKeySecret", nodeName: "AliyunSMS");
            }
            if (signName.IsNullOrEmpty())
            {
                signName = xmlConfigUtil.GetValue("SignName", nodeName: "AliyunSMS");
            }

            if (json.IsNullOrEmpty() && param != null)
            {
                json = JsonUtil.ObjectToJson(param);
            }

            var timeStamp = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");//GTM时间
            // 声明一个字典
            var dic = new Dictionary<string, string>
            {
                //1.系统参数
                { "SignatureMethod", "HMAC-SHA1" },
                { "SignatureNonce", Guid.NewGuid().ToString() },
                { "AccessKeyId", accessKeyId },
                { "SignatureVersion", "1.0" },
                { "Timestamp", timeStamp },
                { "Format", "Json" },

                //2.业务api参数
                { "Action", "SendSms" },
                { "Version", "2017-05-25" },
                { "RegionId", "cn-hangzhou" },
                { "PhoneNumbers", mobile },
                { "SignName", signName },
                { "TemplateParam", json },
                { "TemplateCode", templateCode },
                { "OutId", "123" }
            };

            //3.去除签名关键字key
            if (dic.ContainsKey("Signature"))
            {
                dic.Remove("Signature");
            }

            //4.参数key排序
            var dicAsc = dic.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value.ToString());
            //5.构造待签名的字符串
            var sb = PoolUtil.StringBuilder.Get();
            foreach (var item in dicAsc)
            {
                if (item.Key == "SignName")
                {
                }
                else
                {
                    sb.Append("&").Append(SpecialUrlEncode(item.Key)).Append("=").Append(SpecialUrlEncode(item.Value));
                }
                if (item.Key == "RegionId")
                {
                    sb.Append("&").Append(SpecialUrlEncode("SignName")).Append("=").Append(SpecialUrlEncode(dic["SignName"]));
                }
            }
            var sortedQueryString = sb.ToString().Substring(1);

            var sbToSign = PoolUtil.StringBuilder.Get();
            sbToSign.Append("GET").Append("&");
            sbToSign.Append(SpecialUrlEncode("/")).Append("&");
            sbToSign.Append(SpecialUrlEncode(sortedQueryString));

            var sign = MySign(accessKeySecret + "&", sbToSign.Return());
            //6.签名最后也要做特殊URL编码
            var signature = SpecialUrlEncode(sign);

            //最终打印出合法GET请求的URL
            var url = string.Format("{0}/?Signature={1}{2}", serviceUrl, signature, sb.Return());
            result = HttpGet(url, out message);            
            return result;
        }
        #endregion

        #region private static bool HttpGet(string url, out string error)

        /// <summary>
        /// 短信接口C#调用方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static bool HttpGet(string url, out string error)
        {
            var result = false;
            error = null;
            if (url == null || url.Trim().ToString() == "")
            {
                return result;
            }
            var targetUrl = url.Trim().ToString();
            try
            {
                var hr = (HttpWebRequest)WebRequest.Create(targetUrl);
                hr.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                hr.Method = "GET";
                hr.Timeout = 30 * 60 * 1000;
                var hs = hr.GetResponse();
                var sr = hs.GetResponseStream();
                var ser = new StreamReader(sr, Encoding.UTF8);
                var returnMessage = ser.ReadToEnd();

                error = MessageHandle(returnMessage, out string messageCode);

                var message = error + Environment.NewLine + returnMessage + Environment.NewLine + url;

                if (messageCode.Equals("OK"))
                {
                    result = true;
                    LogUtil.WriteLog(message, "Sms", "Success");
                }
                else
                {
                    LogUtil.WriteLog(message, "Sms", "Fail");
                }

            }
            catch (Exception ex)
            {
                error = "短信发送失败！" + ex.Message;
                LogUtil.WriteException(ex);
            }
            return result;
        }

        #endregion

        #region private static string SpecialUrlEncode(string value)

        /// <summary>
        /// URL编码
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        private static string SpecialUrlEncode(string value)
        {
            var sb = PoolUtil.StringBuilder.Get();
            for (var i = 0; i < value.Length; i++)
            {
                var t = value[i].ToString();
                var k = HttpUtility.UrlEncode(t, Encoding.UTF8);
                if (t == k)
                {
                    sb.Append(t);
                }
                else
                {
                    sb.Append(k.ToUpper());
                }
            }
            return sb.Return().Replace("+", "%20").Replace("*", "%2A").Replace("%7E", "~");
        }

        #endregion

        #region private static string MySign(string accessSecret, string stringToSign)

        /// <summary>
        /// HMACSHA1签名
        /// </summary>
        /// <param name="accessSecret"></param>
        /// <param name="stringToSign"></param>
        /// <returns></returns>
        private static string MySign(string accessSecret, string stringToSign)
        {
            try
            {
                var hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(accessSecret));
                var dataBuffer = Encoding.UTF8.GetBytes(stringToSign);
                var hashBytes = hmacsha1.ComputeHash(dataBuffer);
                var stringbyte = BitConverter.ToString(hashBytes, 0).Replace("-", string.Empty).ToLower();
                var bytes = StrToToHexByte(stringbyte);
                return Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                throw ex;
            }
        }
        #endregion

        #region private static byte[] StrToToHexByte(string hexString)

        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private static byte[] StrToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            var returnBytes = new byte[hexString.Length / 2];
            for (var i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        #endregion

        #region private static string MessageHandle(string jsonMessage, out string messageCode)
        /// <summary>
        /// 消息处理机制
        /// </summary>
        /// <param name="jsonMessage"></param>
        /// <param name="messageCode"></param>
        /// <returns></returns>
        private static string MessageHandle(string jsonMessage, out string messageCode)
        {
            var message = JsonConvert.DeserializeObject<MessageModel>(jsonMessage);
            messageCode = message.Code;
            var result = "";
            switch (message.Code)
            {
                case "OK":
                    result = "短信发送成功！";
                    break;
                case "isp.RAM_PERMISSION_DENY":
                    result = "RAM权限DENY";
                    break;
                case "isv.OUT_OF_SERVICE":
                    result = "业务停机";
                    break;
                case "isv.PRODUCT_UN_SUBSCRIPT":
                    result = "未开通云通信产品的阿里云客户";
                    break;
                case "isv.PRODUCT_UNSUBSCRIBE":
                    result = "产品未开通";
                    break;
                case "isv.ACCOUNT_NOT_EXISTS":
                    result = "账户不存在";
                    break;
                case "isv.ACCOUNT_ABNORMAL":
                    result = "账户异常    ";
                    break;
                case "isv.SMS_TEMPLATE_ILLEGAL":
                    result = "短信模板不合法";
                    break;
                case "isv.SMS_SIGNATURE_ILLEGAL":
                    result = "短信签名不合法";
                    break;
                case "isv.INVALID_PARAMETERS":
                    result = "参数异常";
                    break;
                case "isv.MOBILE_NUMBER_ILLEGAL":
                    result = "非法手机号";
                    break;
                case "isv.MOBILE_COUNT_OVER_LIMIT":
                    result = "手机号码数量超过限制";
                    break;
                case "isv.TEMPLATE_MISSING_PARAMETERS":
                    result = "模板缺少变量";
                    break;
                case "isv.BUSINESS_LIMIT_CONTROL":
                    result = "业务限流";
                    break;
                case "isv.INVALID_JSON_PARAM":
                    result = "JSON参数不合法，只接受字符串值";
                    break;
                case "isv.PARAM_LENGTH_LIMIT":
                    result = "参数超出长度限制";
                    break;
                case "isv.PARAM_NOT_SUPPORT_URL":
                    result = "不支持URL";
                    break;
                case "isv.AMOUNT_NOT_ENOUGH":
                    result = "账户余额不足";
                    break;
                case "isv.TEMPLATE_PARAMS_ILLEGAL":
                    result = "模板变量里包含非法关键字";
                    break;
            }
            return result;
        }

        #endregion

        #region internal class MessageModel
        /// <summary>
        /// 消息模型
        /// </summary>
        internal class MessageModel
        {
            public string RequestId { get; set; }
            public string BizId { get; set; }
            public string Code { get; set; }
            public string Message { get; set; }
        }
        #endregion
    }
}
