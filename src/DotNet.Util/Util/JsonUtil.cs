using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Newtonsoft.Json;

namespace DotNet.Util
{
    /// <summary>
    /// JSON帮助类参考DTcms
    /// </summary>
    public partial class JsonUtil
    {
        /// <summary> 
        /// 对象转JSON 
        /// </summary> 
        /// <param name="obj">对象</param>
        /// <param name="jsonSerializerSettings">序列化设置</param> 
        /// <returns>JSON格式的字符串</returns> 
        public static string ObjectToJson(object obj, object jsonSerializerSettings = null)
        {
            var result = string.Empty;
            try
            {

                if (jsonSerializerSettings == null)
                {
                    result = JsonConvert.SerializeObject(obj);
                }
                else
                {
                    result = JsonConvert.SerializeObject(obj, (JsonSerializerSettings)jsonSerializerSettings);
                }

                result = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(result));
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                //throw new Exception("JsonUtil.ObjectToJson(): " + ex.Message);
            }
            return result;
        }

        /// <summary> 
        /// 数据表转键值对集合
        /// 把DataTable转成 List集合, 存每一行 
        /// 集合中放的是键值对字典,存每一列 
        /// </summary> 
        /// <param name="dt">数据表</param> 
        /// <returns>哈希表数组</returns> 
        public static List<Dictionary<string, object>> DataTableToList(DataTable dt)
        {
            var list
                 = new List<Dictionary<string, object>>();

            foreach (DataRow dr in dt.Rows)
            {
                var dic = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    dic.Add(dc.ColumnName, dr[dc.ColumnName]);
                }
                list.Add(dic);
            }
            return list;
        }

        /// <summary> 
        /// 数据集转键值对数组字典 
        /// </summary> 
        /// <param name="ds">数据集</param> 
        /// <returns>键值对数组字典</returns> 
        public static Dictionary<string, List<Dictionary<string, object>>> DataSetToDic(DataSet ds)
        {
            var result = new Dictionary<string, List<Dictionary<string, object>>>();

            foreach (DataTable dt in ds.Tables)
            {
                result.Add(dt.TableName, DataTableToList(dt));
            }

            return result;
        }

        /// <summary> 
        /// 数据表转JSON 
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="columnNameLowerCase">小写表字段名</param>
        /// <param name="dateTimeFormat">时间日期格式</param>
        /// <returns>JSON字符串</returns> 
        public static string DataTableToJson(DataTable dt, string dateTimeFormat = "MM-dd-yyyy HH:mm:ss", bool columnNameLowerCase = false)
        {
            //return ObjectToJson(DataTableToList(dt));
            var dic = new System.Collections.ArrayList();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var drow = new Dictionary<string, object>();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (columnNameLowerCase)
                        {
                            drow.Add(dc.ColumnName.ToLower(), dr[dc.ColumnName]);
                        }
                        else
                        {
                            drow.Add(dc.ColumnName, dr[dc.ColumnName]);
                        }
                    }
                    dic.Add(drow);
                }
            }
            //Serialize  
            var result = JsonConvert.SerializeObject(dic);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"\\/Date\((\d+)\)\\/", match =>
            {
                var datetime = new DateTime(1970, 1, 1);
                datetime = datetime.AddMilliseconds(long.Parse(match.Groups[1].Value));
                datetime = datetime.ToLocalTime();
                return datetime.ToString(dateTimeFormat);
            });
            return result;
        }

        /// <summary> 
        /// JSON文本转对象,泛型方法 
        /// </summary> 
        /// <typeparam name="T">类型</typeparam> 
        /// <param name="jsonText">JSON文本</param> 
        /// <returns>指定类型的对象</returns> 
        public static T JsonToObject<T>(string jsonText)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonText);
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return default(T);
                //throw new Exception("jsonUtil.JsonToObject(): " + ex.Message);
            }
        }

        /// <summary> 
        /// 将JSON文本转换为数据表数据 
        /// </summary> 
        /// <param name="jsonText">JSON文本</param> 
        /// <returns>数据表字典</returns> 
        public static Dictionary<string, List<Dictionary<string, object>>> TablesDataFromJson(string jsonText)
        {
            return JsonToObject<Dictionary<string, List<Dictionary<string, object>>>>(jsonText);
        }

        /// <summary> 
        /// 将JSON文本转换成数据行 
        /// </summary> 
        /// <param name="jsonText">JSON文本</param> 
        /// <returns>数据行的字典</returns>
        public static Dictionary<string, object> DataRowFromJson(string jsonText)
        {
            return JsonToObject<Dictionary<string, object>>(jsonText);
        }
    }
}