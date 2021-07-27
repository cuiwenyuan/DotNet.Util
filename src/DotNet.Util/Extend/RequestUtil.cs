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
    /// Request������
    /// </summary>
    public partial class RequestUtil
    {
        /// <summary>
        /// �õ���ǰ��������ͷ
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
		/// �õ�����ͷ
		/// </summary>
		/// <returns></returns>
		public static string GetHost()
		{
		    var result = string.Empty;
		    if (HttpContext.Current != null) result = HttpContext.Current.Request.Url.Host;
		    return result;
		}


		/// <summary>
		/// ��ȡ��ǰ�����ԭʼ URL(URL ������Ϣ֮��Ĳ���,������ѯ�ַ���(�������))
		/// </summary>
		/// <returns>ԭʼ URL</returns>
		public static string GetRawUrl()
		{
		    var result = string.Empty;
		    if (HttpContext.Current != null) result = HttpContext.Current.Request.RawUrl;
		    return result;
        }

		/// <summary>
		/// �жϵ�ǰ�����Ƿ�������������
		/// </summary>
		/// <returns>��ǰ�����Ƿ�������������</returns>
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
		/// �ж��Ƿ�����������������
		/// </summary>
		/// <returns>�Ƿ�����������������</returns>
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
		/// ��õ�ǰ����Url��ַ
		/// </summary>
		/// <returns>��ǰ����Url��ַ</returns>
		public static string GetUrl()
		{
			return HttpContext.Current.Request.Url.ToString();
		}
		

		/// <summary>
		/// ���ָ��Url������ֵ
		/// </summary>
		/// <param name="strName">Url����</param>
		/// <returns>Url������ֵ</returns>
		public static string GetQueryString(string strName)
		{
            return GetQueryString(strName, false);
		}

        /// <summary>
        /// ���ָ��Url������ֵ
        /// </summary> 
        /// <param name="strName">Url����</param>
        /// <param name="sqlSafeCheck">�Ƿ����SQL��ȫ���</param>
        /// <returns>Url������ֵ</returns>
        public static string GetQueryString(string strName, bool sqlSafeCheck)
        {
            if (HttpContext.Current.Request.QueryString[strName] == null)
                return "";

            if (sqlSafeCheck && !Utils.IsSafeSqlString(HttpContext.Current.Request.QueryString[strName]))
                return "unsafe string";

            return HttpContext.Current.Request.QueryString[strName];
        }

		/// <summary>
		/// ��õ�ǰҳ�������
		/// </summary>
		/// <returns>��ǰҳ�������</returns>
		public static string GetPageName()
		{
			var urlArr = HttpContext.Current.Request.Url.AbsolutePath.Split('/');
			return urlArr[urlArr.Length - 1].ToLower();
		}

		/// <summary>
		/// ���ر���Url�������ܸ���
		/// </summary>
		/// <returns></returns>
		public static int GetParamCount()
		{
			return HttpContext.Current.Request.Form.Count + HttpContext.Current.Request.QueryString.Count;
		}


		/// <summary>
		/// ���ָ����������ֵ
		/// </summary>
		/// <param name="strName">������</param>
		/// <returns>��������ֵ</returns>
		public static string GetFormString(string strName)
		{
			return GetFormString(strName, false);
		}

        /// <summary>
        /// ���ָ����������ֵ
        /// </summary>
        /// <param name="strName">������</param>
        /// <param name="sqlSafeCheck">�Ƿ����SQL��ȫ���</param>
        /// <returns>��������ֵ</returns>
        public static string GetFormString(string strName, bool sqlSafeCheck)
        {
            if (HttpContext.Current.Request.Form[strName] == null)
                return "";

            if (sqlSafeCheck && !Utils.IsSafeSqlString(HttpContext.Current.Request.Form[strName]))
                return "unsafe string";

            return HttpContext.Current.Request.Form[strName];
        }

		/// <summary>
		/// ���Url���������ֵ, ���ж�Url�����Ƿ�Ϊ���ַ���, ��ΪTrue�򷵻ر�������ֵ
		/// </summary>
		/// <param name="strName">����</param>
		/// <returns>Url���������ֵ</returns>
		public static string GetString(string strName)
		{
            return GetString(strName, false);
		}

        /// <summary>
        /// ���Url���������ֵ, ���ж�Url�����Ƿ�Ϊ���ַ���, ��ΪTrue�򷵻ر�������ֵ
        /// </summary>
        /// <param name="strName">����</param>
        /// <param name="sqlSafeCheck">�Ƿ����SQL��ȫ���</param>
        /// <returns>Url���������ֵ</returns>
        public static string GetString(string strName, bool sqlSafeCheck)
        {
            if ("".Equals(GetQueryString(strName)))
                return GetFormString(strName, sqlSafeCheck);
            else
                return GetQueryString(strName, sqlSafeCheck);
        }

        /// <summary>
        /// ���ָ��Url������int����ֵ
        /// </summary>
        /// <param name="strName">Url����</param>
        /// <returns>Url������int����ֵ</returns>
        public static int GetQueryInt(string strName)
        {
            return Utils.StrToInt(HttpContext.Current.Request.QueryString[strName], 0);
        }


		/// <summary>
		/// ���ָ��Url������int����ֵ
		/// </summary>
		/// <param name="strName">Url����</param>
		/// <param name="defValue">ȱʡֵ</param>
		/// <returns>Url������int����ֵ</returns>
		public static int GetQueryInt(string strName, int defValue)
		{
			return Utils.StrToInt(HttpContext.Current.Request.QueryString[strName], defValue);
		}


		/// <summary>
		/// ���ָ����������int����ֵ
		/// </summary>
		/// <param name="strName">������</param>
		/// <param name="defValue">ȱʡֵ</param>
		/// <returns>��������int����ֵ</returns>
		public static int GetFormInt(string strName, int defValue)
		{
			return Utils.StrToInt(HttpContext.Current.Request.Form[strName], defValue);
		}

		/// <summary>
		/// ���ָ��Url���������int����ֵ, ���ж�Url�����Ƿ�Ϊȱʡֵ, ��ΪTrue�򷵻ر�������ֵ
		/// </summary>
		/// <param name="strName">Url�������</param>
		/// <param name="defValue">ȱʡֵ</param>
		/// <returns>Url���������int����ֵ</returns>
		public static int GetInt(string strName, int defValue)
		{
			if (GetQueryInt(strName, defValue) == defValue)
				return GetFormInt(strName, defValue);
			else
				return GetQueryInt(strName, defValue);
		}

		/// <summary>
		/// ���ָ��Url������float����ֵ
		/// </summary>
		/// <param name="strName">Url����</param>
		/// <param name="defValue">ȱʡֵ</param>
		/// <returns>Url������int����ֵ</returns>
		public static float GetQueryFloat(string strName, float defValue)
		{
			return Utils.StrToFloat(HttpContext.Current.Request.QueryString[strName], defValue);
		}


		/// <summary>
		/// ���ָ����������float����ֵ
		/// </summary>
		/// <param name="strName">������</param>
		/// <param name="defValue">ȱʡֵ</param>
		/// <returns>��������float����ֵ</returns>
		public static float GetFormFloat(string strName, float defValue)
		{
			return Utils.StrToFloat(HttpContext.Current.Request.Form[strName], defValue);
		}
		
		/// <summary>
		/// ���ָ��Url���������float����ֵ, ���ж�Url�����Ƿ�Ϊȱʡֵ, ��ΪTrue�򷵻ر�������ֵ
		/// </summary>
		/// <param name="strName">Url�������</param>
		/// <param name="defValue">ȱʡֵ</param>
		/// <returns>Url���������int����ֵ</returns>
		public static float GetFloat(string strName, float defValue)
		{
			if (Math.Abs(GetQueryFloat(strName, defValue) - defValue) < 0.001)
				return GetFormFloat(strName, defValue);
			else
				return GetQueryFloat(strName, defValue);
		}

        /// <summary>
        /// ���ָ��Url������Decimal����ֵ
        /// </summary>
        /// <param name="strName">Url����</param>
        /// <param name="defValue">ȱʡֵ</param>
        /// <returns>Url������int����ֵ</returns>
        public static decimal GetQueryDecimal(string strName, decimal defValue)
        {
            return Utils.StrToDecimal(HttpContext.Current.Request.QueryString[strName], defValue);
        }


        /// <summary>
        /// ���ָ����������Decimal����ֵ
        /// </summary>
        /// <param name="strName">������</param>
        /// <param name="defValue">ȱʡֵ</param>
        /// <returns>��������float����ֵ</returns>
        public static decimal GetFormDecimal(string strName, decimal defValue)
        {
            return Utils.StrToDecimal(HttpContext.Current.Request.Form[strName], defValue);
        }

        /// <summary>
        /// ���ָ��Url���������Decimal����ֵ, ���ж�Url�����Ƿ�Ϊȱʡֵ, ��ΪTrue�򷵻ر�������ֵ
        /// </summary>
        /// <param name="strName">Url�������</param>
        /// <param name="defValue">ȱʡֵ</param>
        /// <returns>Url���������int����ֵ</returns>
        public static decimal GetDecimal(string strName, decimal defValue)
        {
            if (GetQueryDecimal(strName, defValue) == defValue)
                return GetFormDecimal(strName, defValue);
            else
                return GetQueryDecimal(strName, defValue);
        }

		/// <summary>
		/// ��õ�ǰҳ��ͻ��˵�IP-���Ƽ�ֱ��ʹ�ã���ʹ��Utils.GetIp()
		/// </summary>
		/// <returns>��ǰҳ��ͻ��˵�IP</returns>
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
        /// ͨ���ж��Ƿ�ʹ�ô����ȡ�û�����ʵIP��ַ
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
                    //�����д��� //û�С�.���϶��Ƿ�IPv4��ʽ 
                    if (result.IndexOf(".", StringComparison.OrdinalIgnoreCase) == -1)
                        result = null;
                    else
                    {
                        if (result.IndexOf(",", StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            //�С�,�������ƶ������ȡ��һ������������IP�� 
                            result = result.Replace(" ", "").Replace("'", "");
                            var temporaryIp = result.Split(",;".ToCharArray());
                            foreach (var t in temporaryIp)
                            {
                                if (IsIp(t)
                                    && t.Substring(0, 3) != "10."
                                    && t.Substring(0, 7) != "192.168"
                                    && t.Substring(0, 7) != "172.16.")
                                {
                                    //�ҵ����������ĵ�ַ 
                                    return t;
                                }
                            }
                        }
                        else if (IsIp(result))
                            //������IP��ʽ
                            return result;
                        else
                            //�����е����� ��IP��ȡIP 
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
        /// ͨ�������ж��ַ����Ƿ�ΪIP��ַ
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
        /// ��ȡһ����ҳ
        /// </summary>
        /// <param name="url">��ַ</param>
        /// <returns>�ַ�������ֵ</returns>
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
        /// ��ȡȫ�����������get��post�� �򻯰�
        /// 2016-09-21 �α� ���ӻ�ȡȫ����������ķ���
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
        /// ���� url �ַ����еĲ�����Ϣ
        /// ���get�����
        /// </summary>
        /// <param name="url">����� URL</param>
        /// <param name="baseUrl">��� URL �Ļ�������</param>
        /// <param name="nvc">���������õ��� (������,����ֵ) �ļ���</param>
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
            // ��ʼ����������  
            var reg = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            var mc = reg.Matches(ps);
            foreach (Match m in mc)
            {
                nvc.Add(m.Result("$2").ToLower(), m.Result("$3"));
            }
        }
    }
}
