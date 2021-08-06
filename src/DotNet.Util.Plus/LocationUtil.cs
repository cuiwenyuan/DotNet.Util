using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace DotNet.Util
{
    /// <summary>
    /// 从第三方api获取IP的地理位置信息，参考力软
    /// Troy Cui 2017.12.19
    /// </summary>
    public static partial class LocationUtil
    {
        /// <summary>
        /// 获取IP地址信息
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string GetLocation(string ip)
        {
            var res = "";
            try
            {
                var url = "http://apis.juhe.cn/ip/ip2addr?ip=" + ip + "&dtype=json&key=b39857e36bee7a305d55cdb113a9d725";
                res = HttpUtil.Get(url);
                var resjson = res.ToObject<objex>();
                res = resjson.result.area + " " + resjson.result.location;
            }
            catch
            {
                res = "";
            }
            if (!string.IsNullOrEmpty(res))
            {
                return res;
            }
            try
            {
                var url = "https://sp0.baidu.com/8aQDcjqpAAV3otqbppnN2DJv/api.php?query=" + ip + "&resource_id=6006&ie=utf8&oe=gbk&format=json";
                res = HttpUtil.Get(url, Encoding.GetEncoding("GBK"));
                var resjson = res.ToObject<obj>();
                res = resjson.data[0].location;
            }
            catch
            {
                res = "";
            }



            return res;
        }
        /// <summary>
        /// 百度接口
        /// </summary>
        public class obj
        {
            /// <summary>
            /// dataone List
            /// </summary>
            public List<dataone> data { get; set; }
        }
        /// <summary>
        /// dataone
        /// </summary>
        public class dataone
        {
            /// <summary>
            /// location
            /// </summary>
            public string location { get; set; }
        }
        /// <summary>
        /// 聚合数据
        /// </summary>
        public class objex
        {
            /// <summary>
            /// resultcode
            /// </summary>
            public string resultcode { get; set; }
            /// <summary>
            /// result
            /// </summary>
            public dataoneex result { get; set; }
            /// <summary>
            /// reason
            /// </summary>
            public string reason { get; set; }
            /// <summary>
            /// error_code
            /// </summary>
            public string error_code { get; set; }
        }
        /// <summary>
        /// dataoneex
        /// </summary>
        public class dataoneex
        {
            /// <summary>
            /// area
            /// </summary>
            public string area { get; set; }
            /// <summary>
            /// location
            /// </summary>
            public string location { get; set; }
        }
        #region Newtonsoft.Json
        /// <summary>
        /// 转换为Json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object ToJson(this string json)
        {
            return json == null ? null : JsonUtil.ObjectToJson(json);
        }
        /// <summary>
        /// 转换为Json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            return JsonConvert.SerializeObject(obj, timeConverter);
        }
        /// <summary>
        /// 转换为Json
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="datetimeformats"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, string datetimeformats)
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = datetimeformats };
            return JsonConvert.SerializeObject(obj, timeConverter);
        }
        /// <summary>
        /// 转换为Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Json"></param>
        /// <returns></returns>
        public static T ToObject<T>(this string Json)
        {
            return Json == null ? default(T) : JsonUtil.JsonToObject<T>(Json);
        }
        /// <summary>
        /// 转换为Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this string json)
        {
            return json == null ? null : JsonUtil.JsonToObject<List<T>>(json);
        }
        /// <summary>
        /// 转换为DataTable
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static DataTable ToTable(this string json)
        {
            return json == null ? null : JsonUtil.JsonToObject<DataTable>(json);
        }
        /// <summary>
        /// 转换为JObject
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static JObject ToJObject(this string json)
        {
            return json == null ? JObject.Parse("{}") : JObject.Parse(json.Replace("&nbsp;", ""));
        }
        #endregion
    }
}
