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
    public class AliyunSmsUtil
    {
        private const string FieldUrl = "dysmsapi.aliyuncs.com";
        private const string FieldAccessKeyId = "LTAIaYsI2dAApOkD";
        private const string FieldAccessKeySecret = "jfwiIBTK2dRk83Lz3qAasn7JkB08t6";
        private const string FieldSignName = "威格博尔";

        /// <summary>
        /// 发货信息
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="deliveryDate"></param>
        /// <param name="expressName"></param>
        /// <param name="expressNumber"></param>
        /// <param name="templateCode">模版CODE</param>
        /// <param name="message">返回消息</param>
        /// <returns></returns>
        public static bool SendDeliverySms(string mobile, DateTime deliveryDate, string expressName, string expressNumber, string templateCode, out string message)
        {
            var result = false;
            if (string.IsNullOrEmpty(mobile))
            {
                message = "手机号码有误！";
                return false;
            }
            //else if (!IsMobile(mobile))
            //{
            //    message = "手机号码有误！";
            //    return false;
            //}
            var nowDate = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");//GTM时间
            var keyValues = new Dictionary<string, string>();//声明一个字典
            //1.系统参数
            keyValues.Add("SignatureMethod", "HMAC-SHA1");
            keyValues.Add("SignatureNonce", Guid.NewGuid().ToString());
            keyValues.Add("AccessKeyId", FieldAccessKeyId);
            keyValues.Add("SignatureVersion", "1.0");
            keyValues.Add("Timestamp", nowDate);
            keyValues.Add("Format", "Json");//可换成xml

            //2.业务api参数
            keyValues.Add("Action", "SendSms");
            keyValues.Add("Version", "2017-05-25");
            keyValues.Add("RegionId", "cn-hangzhou");
            keyValues.Add("PhoneNumbers", mobile);
            keyValues.Add("SignName", FieldSignName);
            keyValues.Add("TemplateParam", "{\"deliveryDate\":\"" + deliveryDate.ToString(BaseSystemInfo.DateFormat) + "\",\"expressName\":\"" + expressName + "\",\"expressNumber\":\"" + expressNumber + "\"}");
            keyValues.Add("TemplateCode", templateCode);
            keyValues.Add("OutId", "123");

            //3.去除签名关键字key
            if (keyValues.ContainsKey("Signature"))
            {
                keyValues.Remove("Signature");
            }

            //4.参数key排序
            var ascDic = keyValues.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value.ToString());
            //5.构造待签名的字符串
            var sb = Pool.StringBuilder.Get();
            foreach (var item in ascDic)
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
                    sb.Append("&").Append(SpecialUrlEncode("SignName")).Append("=").Append(SpecialUrlEncode(keyValues["SignName"]));
                }
            }
            var sortedQueryString = sb.Put().Substring(1);

            var stringToSign = Pool.StringBuilder.Get();
            stringToSign.Append("GET").Append("&");
            stringToSign.Append(SpecialUrlEncode("/")).Append("&");
            stringToSign.Append(SpecialUrlEncode(sortedQueryString));

            var sign = MySign(FieldAccessKeySecret + "&", stringToSign.Put());
            //6.签名最后也要做特殊URL编码
            var signature = SpecialUrlEncode(sign);

            //最终打印出合法GET请求的URL
            var url = string.Format("http://{0}/?Signature={1}{2}", FieldUrl, signature, sb);
            message = HttpGet(url, out result);
            return result;
        }


        /// <summary>
        /// 短信接口C#调用方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static string HttpGet(string url, out bool result)
        {
            result = false;
            string strRet = null;
            if (url == null || url.Trim().ToString() == "")
            {
                return null;
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
                
                strRet = MessageHandle(returnMessage, out string messageCode);

                var logMessage = strRet + Environment.NewLine + returnMessage + Environment.NewLine + url;

                if (messageCode.Equals("OK"))
                {
                    result = true;
                    LogUtil.WriteLog(logMessage, "Sms", "Success");
                }
                else
                {
                    LogUtil.WriteLog(logMessage, "Sms","Fail");
                }

            }
            catch (Exception ex)
            {
                strRet = "短信发送失败！" + ex.Message;
                LogUtil.WriteException(ex);
            }
            return strRet;
        }

        /// <summary>
        /// 验证手机号码是否合法
        /// </summary>
        /// <param name="mobile">电话号码</param>
        /// <returns></returns>
        public static bool IsMobile(string mobile)
        {
            var result = false;
            if (!string.IsNullOrEmpty(mobile))
            {
                result= System.Text.RegularExpressions.Regex.IsMatch(mobile, @"^1[3|4|5|7|8][0-9]\d{8}$");
            }

            return result;
        }

        /// <summary>
        /// URL编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string SpecialUrlEncode(string value)
        {
            var sb = Pool.StringBuilder.Get();
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
            return sb.Put().Replace("+", "%20").Replace("*", "%2A").Replace("%7E", "~");
        }

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

                throw ex;
            }
        }
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

    }

    internal class MessageModel
    {
        public string RequestId { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
