using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace DotNet.Util
{
    /// <summary>
    /// Request操作类
    /// </summary>
    public partial class RequestUtil
    {
        /// <summary>
        /// 得到当前完整主机头
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentFullHost()
		{
		    var result = string.Empty;
		    if (Microsoft.AspNetCore.Http.HttpContext.Current != null)
		    {
		        if (!request.Url.IsDefaultPort)
		        {
		            result = string.Format("{0}:{1}", request.Url.Host, request.Url.Port);
		        }
		        else
		        {
		            result = request.Url.Host;
                }
		    }
		    return result;
		}

		/// <summary>
		/// 得到主机头
		/// </summary>
		/// <returns></returns>
		public static string GetHost()
		{
		    var result = string.Empty;
		    if (HttpContext.Current != null) result = HttpContext.Current.Request.Url.Host;
		    return result;
		}


		/// <summary>
		/// 获取当前请求的原始 URL(URL 中域信息之后的部分,包括查询字符串(如果存在))
		/// </summary>
		/// <returns>原始 URL</returns>
		public static string GetRawUrl()
		{
		    var result = string.Empty;
		    if (HttpContext.Current != null) result = HttpContext.Current.Request.RawUrl;
		    return result;
        }

		/// <summary>
		/// 判断当前访问是否来自浏览器软件
		/// </summary>
		/// <returns>当前访问是否来自浏览器软件</returns>
		public static bool IsBrowserGet()
		{
			string[] browserName = {"ie", "opera", "netscape", "mozilla", "konqueror", "firefox"};
			var curBrowser = HttpContext.Current.Request.Browser.Type.ToLower();
			foreach (var t in browserName)
			{
			    if (curBrowser.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0)
			        return true;
			}
			return false;
		}

		/// <summary>
		/// 判断是否来自搜索引擎链接
		/// </summary>
		/// <returns>是否来自搜索引擎链接</returns>
		public static bool IsSearchEnginesGet()
		{
            if (HttpContext.Current.Request.UrlReferrer == null)
                return false;

            string[] searchEngine = {"google", "yahoo", "msn", "baidu", "sogou", "sohu", "sina", "163", "lycos", "tom", "yisou", "iask", "soso", "gougou", "zhongsou"};
			var tmpReferrer = HttpContext.Current.Request.UrlReferrer.ToString().ToLower();
			for (var i = 0; i < searchEngine.Length; i++)
			{
				if (tmpReferrer.IndexOf(searchEngine[i], StringComparison.OrdinalIgnoreCase) >= 0)
					return true;
			}
			return false;
		}

		/// <summary>
		/// 获得当前完整Url地址
		/// </summary>
		/// <returns>当前完整Url地址</returns>
		public static string GetUrl()
		{
			return HttpContext.Current.Request.Url.ToString();
		}
		

		/// <summary>
		/// 获得指定Url参数的值
		/// </summary>
		/// <param name="strName">Url参数</param>
		/// <returns>Url参数的值</returns>
		public static string GetQueryString(string strName)
		{
            return GetQueryString(strName, false);
		}

        /// <summary>
        /// 获得指定Url参数的值
        /// </summary> 
        /// <param name="strName">Url参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>Url参数的值</returns>
        public static string GetQueryString(string strName, bool sqlSafeCheck)
        {
            if (HttpContext.Current.Request.QueryString[strName] == null)
                return "";

            if (sqlSafeCheck && !Utils.IsSafeSqlString(HttpContext.Current.Request.QueryString[strName]))
                return "unsafe string";

            return HttpContext.Current.Request.QueryString[strName];
        }

		/// <summary>
		/// 获得当前页面的名称
		/// </summary>
		/// <returns>当前页面的名称</returns>
		public static string GetPageName()
		{
			var urlArr = HttpContext.Current.Request.Url.AbsolutePath.Split('/');
			return urlArr[urlArr.Length - 1].ToLower();
		}

		/// <summary>
		/// 返回表单或Url参数的总个数
		/// </summary>
		/// <returns></returns>
		public static int GetParamCount()
		{
			return HttpContext.Current.Request.Form.Count + HttpContext.Current.Request.QueryString.Count;
		}


		/// <summary>
		/// 获得指定表单参数的值
		/// </summary>
		/// <param name="strName">表单参数</param>
		/// <returns>表单参数的值</returns>
		public static string GetFormString(string strName)
		{
			return GetFormString(strName, false);
		}

        /// <summary>
        /// 获得指定表单参数的值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>表单参数的值</returns>
        public static string GetFormString(string strName, bool sqlSafeCheck)
        {
            if (HttpContext.Current.Request.Form[strName] == null)
                return "";

            if (sqlSafeCheck && !Utils.IsSafeSqlString(HttpContext.Current.Request.Form[strName]))
                return "unsafe string";

            return HttpContext.Current.Request.Form[strName];
        }

		/// <summary>
		/// 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
		/// </summary>
		/// <param name="strName">参数</param>
		/// <returns>Url或表单参数的值</returns>
		public static string GetString(string strName)
		{
            return GetString(strName, false);
		}

        /// <summary>
        /// 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>Url或表单参数的值</returns>
        public static string GetString(string strName, bool sqlSafeCheck)
        {
            if ("".Equals(GetQueryString(strName)))
                return GetFormString(strName, sqlSafeCheck);
            else
                return GetQueryString(strName, sqlSafeCheck);
        }

        /// <summary>
        /// 获得指定Url参数的int类型值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <returns>Url参数的int类型值</returns>
        public static int GetQueryInt(string strName)
        {
            return Utils.StrToInt(HttpContext.Current.Request.QueryString[strName], 0);
        }


		/// <summary>
		/// 获得指定Url参数的int类型值
		/// </summary>
		/// <param name="strName">Url参数</param>
		/// <param name="defValue">缺省值</param>
		/// <returns>Url参数的int类型值</returns>
		public static int GetQueryInt(string strName, int defValue)
		{
			return Utils.StrToInt(HttpContext.Current.Request.QueryString[strName], defValue);
		}


		/// <summary>
		/// 获得指定表单参数的int类型值
		/// </summary>
		/// <param name="strName">表单参数</param>
		/// <param name="defValue">缺省值</param>
		/// <returns>表单参数的int类型值</returns>
		public static int GetFormInt(string strName, int defValue)
		{
			return Utils.StrToInt(HttpContext.Current.Request.Form[strName], defValue);
		}

		/// <summary>
		/// 获得指定Url或表单参数的int类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
		/// </summary>
		/// <param name="strName">Url或表单参数</param>
		/// <param name="defValue">缺省值</param>
		/// <returns>Url或表单参数的int类型值</returns>
		public static int GetInt(string strName, int defValue)
		{
			if (GetQueryInt(strName, defValue) == defValue)
				return GetFormInt(strName, defValue);
			else
				return GetQueryInt(strName, defValue);
		}

		/// <summary>
		/// 获得指定Url参数的float类型值
		/// </summary>
		/// <param name="strName">Url参数</param>
		/// <param name="defValue">缺省值</param>
		/// <returns>Url参数的int类型值</returns>
		public static float GetQueryFloat(string strName, float defValue)
		{
			return Utils.StrToFloat(HttpContext.Current.Request.QueryString[strName], defValue);
		}


		/// <summary>
		/// 获得指定表单参数的float类型值
		/// </summary>
		/// <param name="strName">表单参数</param>
		/// <param name="defValue">缺省值</param>
		/// <returns>表单参数的float类型值</returns>
		public static float GetFormFloat(string strName, float defValue)
		{
			return Utils.StrToFloat(HttpContext.Current.Request.Form[strName], defValue);
		}
		
		/// <summary>
		/// 获得指定Url或表单参数的float类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
		/// </summary>
		/// <param name="strName">Url或表单参数</param>
		/// <param name="defValue">缺省值</param>
		/// <returns>Url或表单参数的int类型值</returns>
		public static float GetFloat(string strName, float defValue)
		{
			if (Math.Abs(GetQueryFloat(strName, defValue) - defValue) < 0.001)
				return GetFormFloat(strName, defValue);
			else
				return GetQueryFloat(strName, defValue);
		}

        /// <summary>
        /// 获得指定Url参数的Decimal类型值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url参数的int类型值</returns>
        public static decimal GetQueryDecimal(string strName, decimal defValue)
        {
            return Utils.StrToDecimal(HttpContext.Current.Request.QueryString[strName], defValue);
        }


        /// <summary>
        /// 获得指定表单参数的Decimal类型值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>表单参数的float类型值</returns>
        public static decimal GetFormDecimal(string strName, decimal defValue)
        {
            return Utils.StrToDecimal(HttpContext.Current.Request.Form[strName], defValue);
        }

        /// <summary>
        /// 获得指定Url或表单参数的Decimal类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">Url或表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url或表单参数的int类型值</returns>
        public static decimal GetDecimal(string strName, decimal defValue)
        {
            if (GetQueryDecimal(strName, defValue) == defValue)
                return GetFormDecimal(strName, defValue);
            else
                return GetQueryDecimal(strName, defValue);
        }

		/// <summary>
		/// 获得当前页面客户端的IP-不推荐直接使用，请使用Utils.GetIp()
		/// </summary>
		/// <returns>当前页面客户端的IP</returns>
		public static string GetIp()
		{
		    var result = string.Empty;

            if (HttpContext.Current != null)
		    {
		        result = HttpContext.Current.Request.Headers["X-Real-IP"];
		        if (string.IsNullOrEmpty(result))
		        {
		            result = HttpContext.Current.Request.Headers["X-Forwarded-For"];
		        }
		        if (string.IsNullOrEmpty(result))
		        {
		            result = HttpContext.Current.Features.Get<IHttpConnectionFeature>().RemoteIpAddress.ToString();
		        }
            }
		    if (string.IsNullOrEmpty(result) || !Utils.IsIp(result))
		    {
		        return "127.0.0.1";
		    }
			return result;
		}

        /// <summary>
        /// 通过判断是否使用代理获取用户的真实IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetRealIp()
        {
            var result = string.Empty;
            if (HttpContext.Current != null)
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Request.Headers["X-Forwarded-For"]))
                {
                    result = HttpContext.Current.Request.Headers["X-Forwarded-For"];
                }
                else
                {
                    result = HttpContext.Current.Request.Headers["X-Real-IP"];
                }
                if (!string.IsNullOrEmpty(result))
                {
                    //可能有代理 //没有“.”肯定是非IPv4格式 
                    if (result.IndexOf(".", StringComparison.OrdinalIgnoreCase) == -1)
                        result = null;
                    else
                    {
                        if (result.IndexOf(",", StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            //有“,”，估计多个代理。取第一个不是内网的IP。 
                            result = result.Replace(" ", "").Replace("'", "");
                            var temporaryIp = result.Split(",;".ToCharArray());
                            foreach (var t in temporaryIp)
                            {
                                if (IsIp(t)
                                    && t.Substring(0, 3) != "10."
                                    && t.Substring(0, 7) != "192.168"
                                    && t.Substring(0, 7) != "172.16.")
                                {
                                    //找到不是内网的地址 
                                    return t;
                                }
                            }
                        }
                        else if (IsIp(result))
                            //代理即是IP格式
                            return result;
                        else
                            //代理中的内容 非IP，取IP 
                            result = null;
                    }

                }

                var ipAddress = (
                    HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null &&
                    HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != string.Empty
                )
                    ? HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]
                    : HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                if (string.IsNullOrEmpty(result))
                {
                    result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (string.IsNullOrEmpty(result) || result.Equals("::1"))
                {
                    result = HttpContext.Current.Request.UserHostAddress;
                }
            }
            return result;
        }

        /// <summary>
        /// 通过正则判断字符串是否为IP地址
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool IsIp(string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length < 7 || str.Length > 15) return false;

            var regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";

            var regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(str);
        }

        #region public static string GetResponse(string url)
        /// <summary>
        /// 获取一个网页
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns>字符串返回值</returns>
        public static string GetResponse(string url)
        {
            string result = null;
            // string url = "http://www.MengGuRen.com";
            WebResponse webResponse = null;
            StreamReader streamReader = null;
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "GET";
                webResponse = httpWebRequest.GetResponse();
                streamReader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8);
                result = streamReader.ReadToEnd();
            }
            catch
            {
                // handle error
            }
            finally
            {
                if (streamReader != null)
                {
                    streamReader.Close();
                }
                if (webResponse != null)
                {
                    webResponse.Close();
                }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 获取全部请求参数，get和post的 简化版
        /// 2016-09-21 宋彪 增加获取全部请求参数的方法
        /// </summary>
        public static string GetRequestParameters()
        {
            var query = HttpContext.Current.Request.Url.Query;
            NameValueCollection nvc;
            string baseUrl;
            ParseUrl(query, out baseUrl, out nvc);
            var list = new List<string>() { };
            foreach (var key in nvc.AllKeys)
            {
                list.Add(key + "=" + nvc[key]);
            }
            var form = HttpContext.Current.Request.Form;
            foreach (var key in form.Keys)
            {
                list.Add(key + "=" + form[key]);
            }
            var result = HttpContext.Current.Request.Url.AbsoluteUri + "?" + string.Join("&", list);
            return result;
        }

        /// <summary>
        /// 分析 url 字符串中的参数信息
        /// 针对get请求的
        /// </summary>
        /// <param name="url">输入的 URL</param>
        /// <param name="baseUrl">输出 URL 的基础部分</param>
        /// <param name="nvc">输出分析后得到的 (参数名,参数值) 的集合</param>
        public static void ParseUrl(string url, out string baseUrl, out NameValueCollection nvc)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }
            nvc = new NameValueCollection();
            baseUrl = "";
            if (url == "")
            {
                return;
            }
            var questionMarkIndex = url.IndexOf('?');
            if (questionMarkIndex == -1)
            {
                baseUrl = url;
                return;
            }
            baseUrl = url.Substring(0, questionMarkIndex);
            if (questionMarkIndex == url.Length - 1)
            {
                return;
            }
            var ps = url.Substring(questionMarkIndex + 1);
            // 开始分析参数对  
            var reg = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            var mc = reg.Matches(ps);
            foreach (Match m in mc)
            {
                nvc.Add(m.Result("$2").ToLower(), m.Result("$3"));
            }
        }
    }
}
