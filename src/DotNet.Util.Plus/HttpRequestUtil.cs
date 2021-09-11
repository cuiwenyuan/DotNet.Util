using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if NET40
using System.Net;
#endif
#if NETSTANDARD2_0_OR_GREATER
using System.Net.Http;
#endif
using System.Text;
using Newtonsoft.Json;

namespace DotNet.Util
{
    /// <summary>
    /// 总结各种请求方式
    /// </summary>
    public static class HttpRequestUtil
    {
#if NET40
#region WebClient

#region WebClient的Get请求
        /// <summary>
        /// WebClient的Get请求
        /// </summary>
        /// <param name="url">请求地址,含拼接数据，请求格式为："http://XXXX?userName=admin&amp;pwd=123456";</param>
        /// <returns></returns>
        public static string WcGet(string url)
        {
            var wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            return wc.DownloadString(url);
        }
#endregion

#region WebClient的Post请求
        /// <summary>
        /// WebClient的Post请求
        /// 表单提交模式[application/x-www-form-urlencoded]
        /// </summary>
        /// <param name="url">请求地址,单纯的地址,没有数据拼接</param>
        /// <param name="data">请求数据,格式为:"userName=admin&amp;pwd=123456"</param>
        /// <returns></returns>
        public static string WcPost(string url, string data)
        {
            var wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            //也可以向表头中添加一些其他东西
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            return wc.UploadString(url, data);
        }
#endregion

#region WebClient的Post请求（JSON）
        /// <summary>
        /// WebClient的Post请求
        /// Json提交模式[application/json]
        /// </summary>
        /// <param name="url">请求地址,单纯的地址,没有数据拼接</param>
        /// <param name="data">请求数据,格式为(Json)对象、或者类对象 eg： new {id="1"}</param>
        /// <returns></returns>
        public static string WcPostJson(string url, object data)
        {
            var wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            //也可以向表头中添加一些其他东西
            wc.Headers.Add("Content-Type", "application/json");
            return wc.UploadString(url, JsonConvert.SerializeObject(data));
        }
#endregion

#endregion

#region HttpWebRequest

#region HttpWebRequest的Get请求
        /// <summary>
        /// HttpWebRequest的Get请求
        /// </summary>
        /// <param name="url">请求地址,含拼接数据，请求格式为："http://XXXX?userName=admin&amp;pwd=123456";</param>
        /// <returns></returns>
        public static string HwGet(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 30 * 1000;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.118 Safari/537.36";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            var result = "";
            using (var res = request.GetResponse() as HttpWebResponse)
            {
                if (res != null && res.StatusCode == HttpStatusCode.OK)
                {
                    var reader = new StreamReader(res.GetResponseStream() ?? throw new InvalidOperationException(), Encoding.UTF8);
                    result = reader.ReadToEnd();
                }
            }
            return result;
        }
#endregion

#region HttpWebRequest的Post请求
        /// <summary>
        /// HttpWebRequest的Post请求
        /// 表单提交模式[application/x-www-form-urlencoded]
        /// </summary>
        /// <param name="url">请求地址,单纯的地址,没有数据拼接</param>
        /// <param name="data">请求数据,格式为:"userName=admin&amp;pwd=123456"</param>
        /// <returns></returns>
        public static string HwPost(string url, string data)
        {
            var result = "";
            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            if (request != null)
            {
                request.Timeout = 30 * 1000; //设置30s的超时
                request.UserAgent =
                    "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.118 Safari/537.36";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "POST";
                var data2 = Encoding.UTF8.GetBytes(data);
                request.ContentLength = data2.Length;
                var postStream = request.GetRequestStream();
                postStream.Write(data2, 0, data2.Length);
                postStream.Close();

                using (var res = request.GetResponse() as HttpWebResponse)
                {
                    if (res != null && res.StatusCode == HttpStatusCode.OK)
                    {
                        var reader = new StreamReader(res.GetResponseStream() ?? throw new InvalidOperationException(), Encoding.UTF8);
                        result = reader.ReadToEnd();
                    }
                }
            }
            return result;
        }
#endregion

#region HttpWebRequest的Post请求（JSON）
        /// <summary>
        /// HttpWebRequest的Post请求
        /// Json提交模式[application/json]
        /// </summary>
        /// <param name="url">请求地址,单纯的地址,没有数据拼接</param>
        /// <param name="data">请求数据,格式为(Json)对象、或者类对象 eg： new {id="1"}</param>
        /// <returns></returns>
        public static string HwPostJson(string url, object data)
        {
            var result = "";
            var postData = JsonConvert.SerializeObject(data);
            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            if (request != null)
            {
                request.Timeout = 30 * 1000; //设置30s的超时
                request.UserAgent =
                    "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.118 Safari/537.36";
                request.ContentType = "application/json";
                request.Method = "POST";
                var data2 = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = data2.Length;
                var postStream = request.GetRequestStream();
                postStream.Write(data2, 0, data2.Length);
                postStream.Close();

                using (var res = request.GetResponse() as HttpWebResponse)
                {
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        var reader = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                        result = reader.ReadToEnd();
                    }
                }
            }
            return result;
        }
#endregion

#endregion
#endif

#if NETSTANDARD2_0_OR_GREATER
#region HttpClient

#region HttpClient的Get请求
        /// <summary>
        /// HttpClient的Get请求
        /// </summary>
        ///<param name="url">请求地址,含拼接数据，请求格式为："http://XXXX?userName=admin&amp;pwd=123456";</param>
        /// <returns></returns>
        public static string HcGet(string url)
        {
            var http = HttpClientFactory2.GetHttpClient();
            var response1 = http.GetAsync(url).Result;
            return response1.Content.ReadAsStringAsync().Result;
        }
#endregion

#region HttpClient的Post请求
        /// <summary>
        /// HttpClient的Post请求
        /// 表单提交模式[application/x-www-form-urlencoded]
        /// </summary>
        /// <param name="url">请求地址,单纯的地址,没有数据拼接</param>
        /// <param name="data">请求数据,格式为:"userName=admin&amp;pwd=123456"</param>
        /// <returns></returns>
        public static string HcPost(string url, string data)
        {
            var http = HttpClientFactory2.GetHttpClient();
            var content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = http.PostAsync(url, content).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
#endregion

#region HttpClient的Post请求（JSON）
        /// <summary>
        /// HttpClient的Post请求
        /// Json提交模式[application/json]
        /// </summary>
        /// <param name="url">请求地址,单纯的地址,没有数据拼接</param>
        /// <param name="data">请求数据,格式为(Json)对象、或者类对象 eg： new {id="1"}</param>
        /// <returns></returns>
        public static string HcPostJson(string url, object data)
        {
            var http = HttpClientFactory2.GetHttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = http.PostAsync(url, content).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
#endregion

#endregion
#endif

    }
#if NETSTANDARD2_0_OR_GREATER
#region HttpClientFactory2
    /// <summary>
    /// 将HttpClient做成单例的，不用Using，全局只有一个
    /// 来解决tcp连接不能释放的问题，但仍有DNS变更不会反应的问题
    /// </summary>
    public class HttpClientFactory2
    {
        private static HttpClient _httpClient = null;

        /// <summary>
        /// 静态的构造函数：只能有一个，且是无参数的
        /// 由CLR保证，只有在程序第一次使用该类之前被调用，而且只能调用一次
        /// 说明： keep-alive关键字可以理解为一个长链接，超时时间也可以在上面进行设置，例如10秒的超时时间，当然并发量太大，这个10秒应该会抛弃很多请求
        /// 发送请求的代码没有了using，即这个HttpClient不会被手动dispose，而是由系统控制它，当然你的程序重启时，这也就被回收了。
        /// </summary>
        static HttpClientFactory2()
        {
            _httpClient = new HttpClient(new HttpClientHandler())
            {
                Timeout = new TimeSpan(0, 0, 10)
            };
            _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
        }

        /// <summary>
        /// 对外开放接口
        /// </summary>
        /// <returns></returns>
        public static HttpClient GetHttpClient()
        {
            return _httpClient;
        }
    }
#endregion
#endif
}
