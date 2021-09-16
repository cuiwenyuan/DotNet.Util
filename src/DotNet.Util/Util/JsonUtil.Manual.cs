using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace DotNet.Util
{
    /// <summary>
    /// JSON帮助类扩展参考DTcms
    /// </summary>
    public partial class JsonUtil
    {
        #region json 处理

        /// <summary>
        /// 拼接json的字符串;
        /// 比如："{\"ret\":\"err\",\"stadname\":\"未知\"}"
        /// </summary>
        /// <param name="jsonDict"></param>
        /// <returns></returns>
        public static string GetJsonStr(Dictionary<string, string> jsonDict)
        {
            var sb = Pool.StringBuilder.Get();
            sb.Append("{");
            var i = 0;
            foreach (var jd in jsonDict)
            {
                if (i != (jsonDict.Count - 1))
                {
                    sb.Append("\"" + jd.Key + "\":\"" + jd.Value + "\",");
                }
                else
                {
                    sb.Append("\"" + jd.Key + "\":\"" + jd.Value + "\"");
                }

                i++;
            }

            sb.Append("}");
            return sb.Put();
        }

        /// <summary>
        /// json转化为Dictionay集合
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static Dictionary<string, object> JsonToDictionary(string jsonData)
        {
            try
            {
                //将指定的 JSON 字符串转换为 Dictionary<string, object> 类型的对象
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        #endregion

        #region cyqdata扩展
        /// <summary>
        /// 检测是否Json格式的字符串
        /// </summary>
        /// <param name="json">要检测的字符串</param>
        public static bool IsJson(string json)
        {
            return JsonSplit.IsJson(json);
        }
        /// <summary>
        /// 检测是否Json格式的字符串
        /// </summary>
        /// <param name="json">要检测的字符串</param>
        /// <param name="errIndex">错误的字符索引</param>
        /// <returns></returns>
        public static bool IsJson(string json, out int errIndex)
        {
            return JsonSplit.IsJson(json, out errIndex);
        }

        /// <summary>
        /// 获取Json字符串的值
        /// </summary>
        /// <param name="json">Json字符串</param>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public static string GetJosnValue(string json, string key)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(json))
            {
                var jsonDic = Split(json);
                if (jsonDic.ContainsKey(key))
                {
                    result = jsonDic[key];
                }
                else
                {
                    #region 字符串截取
                    key = "\"" + key.Trim('"') + "\"";
                    var index = json.IndexOf(key, StringComparison.OrdinalIgnoreCase) + key.Length + 1;
                    if (index > key.Length + 1)
                    {
                        var end = 0;
                        for (var i = index; i < json.Length; i++)
                        {
                            switch (json[i])
                            {
                                case '{':
                                    end = json.IndexOf('}', i) + 1;
                                    goto endfor;
                                case '[':
                                    end = json.IndexOf(']', i) + 1;
                                    goto endfor;
                                case '"':
                                    end = json.IndexOf('"', i) + 1;
                                    goto endfor;
                                case '\'':
                                    end = json.IndexOf('\'', i) + 1;
                                    goto endfor;
                                case ' ':
                                    continue;
                                default:
                                    end = json.IndexOf(',', i);
                                    if (end == -1)
                                    {
                                        end = json.IndexOf('}', index);
                                    }
                                    goto endfor;

                            }
                        }
                    endfor:
                        if (end > index)
                        {
                            //index = json.IndexOf('"', index + key.Length + 1) + 1;
                            result = json.Substring(index, end - index);
                            //过滤引号或空格
                            result = result.Trim(new char[] { '"', ' ', '\'' });
                        }
                    }
                    #endregion
                }
                jsonDic = null;
            }
            return result;
        }

        /// <summary>
        /// 将Json分隔成键值对。
        /// </summary>
        public static Dictionary<string, string> Split(string json)
        {
            var result = JsonSplit.Split(json);
            if (result != null && result.Count > 0)
            {
                return result[0];
            }
            return null;
        }
        /// <summary>
        /// 将Json 数组分隔成多个键值对。
        /// </summary>
        public static List<Dictionary<string, string>> SplitArray(string jsonArray)
        {
            if (string.IsNullOrEmpty(jsonArray))
            {
                return null;
            }
            jsonArray = jsonArray.Trim();
            return JsonSplit.Split(jsonArray);
        }

        #endregion 

        #region cyqdata扩展：Json转Xml
        /// <summary>
        /// 将一个Json转成Xml
        /// </summary>
        /// <param name="json">Json字符串</param>
        public static string ToXml(string json)
        {
            return ToXml(json, true);
        }
        /// <summary>
        /// 将一个Json转成Xml
        /// </summary>
        /// <param name="json">Json字符串</param>
        /// <param name="isWithAttr">是否转成属性，默认true</param>
        /// <returns></returns>
        public static string ToXml(string json, bool isWithAttr)
        {
            var sb = Pool.StringBuilder.Get();
            sb.Append("<?xml version=\"1.0\"  standalone=\"yes\"?>");
            var dicList = JsonSplit.Split(json);
            if (dicList != null && dicList.Count > 0)
            {
                var addRoot = dicList.Count > 1 || dicList[0].Count > 1;
                if (addRoot)
                {
                    sb.Append("<root>");//</root>";
                }

                sb.Append(GetXmlList(dicList, isWithAttr));

                if (addRoot)
                {
                    sb.Append("</root>");//</root>";
                }

            }
            return sb.Put();
        }

        private static string GetXmlList(List<Dictionary<string, string>> dicList, bool isWithAttr)
        {
            if (dicList == null || dicList.Count == 0)
            {
                return string.Empty;
            }
            var sb = Pool.StringBuilder.Get();
            for (var i = 0; i < dicList.Count; i++)
            {
                sb.Append(GetXml(dicList[i], isWithAttr));
            }
            return sb.Put();
        }

        private static string GetXml(Dictionary<string, string> dic, bool isWithAttr)
        {
            var sb = Pool.StringBuilder.Get();
            var isJson = false;
            foreach (var item in dic)
            {
                isJson = IsJson(item.Value);
                if (!isJson)
                {
                    sb.AppendFormat("<{0}>{1}</{0}>", item.Key, FormatCdata(item.Value));
                }
                else
                {
                    var jsonList = JsonSplit.Split(item.Value);
                    if (jsonList != null && jsonList.Count > 0)
                    {
                        if (!isWithAttr)
                        {
                            sb.AppendFormat("<{0}>", item.Key);
                        }
                        for (var j = 0; j < jsonList.Count; j++)
                        {
                            if (isWithAttr)
                            {
                                sb.Append(GetXmlElement(item.Key, jsonList[j]));
                            }
                            else
                            {
                                sb.Append(GetXml(jsonList[j], isWithAttr));
                            }
                        }
                        if (!isWithAttr)
                        {
                            sb.AppendFormat("</{0}>", item.Key);
                        }
                    }
                    else // 空Json {}
                    {
                        sb.AppendFormat("<{0}></{0}>", item.Key);
                    }
                }
            }
            return sb.Put();
        }

        private static string GetXmlElement(string parentName, Dictionary<string, string> dic)
        {
            var sb = Pool.StringBuilder.Get();
            var jsonDic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            sb.Append("<" + parentName);
            foreach (var kv in dic)
            {
                if (kv.Value.IndexOf('"') > -1 || IsJson(kv.Value)) // 属性不能带双引号，所以转到元素处理。
                {
                    jsonDic.Add(kv.Key, kv.Value);
                }
            }
            //InnerText 节点存在=》（如果有元素节点，则当属性处理；若无，则当InnerText）
            var useForInnerText = dic.ContainsKey(parentName) && jsonDic.Count == 0;
            foreach (var kv in dic)
            {
                if (!jsonDic.ContainsKey(kv.Key) && (kv.Key != parentName || !useForInnerText))
                {
                    sb.AppendFormat(" {0}=\"{1}\"", kv.Key, kv.Value);//先忽略同名属性，内部InnerText节点，
                }
            }
            sb.Append(">");
            if (useForInnerText)
            {
                sb.Append(FormatCdata(dic[parentName]));//InnerText。
            }
            else if (jsonDic.Count > 0)
            {
                sb.Append(GetXml(jsonDic, true));//数组，当元素处理。
            }
            sb.Append("</" + parentName + ">");

            return sb.Put();
        }
        private static string FormatCdata(string text)
        {
            if (text.LastIndexOfAny(new char[] { '<', '>', '&' }) > -1 && !text.StartsWith("<![CDATA["))
            {
                text = "<![CDATA[" + text.Trim() + "]]>";
            }
            return text;
        }
        #endregion

        #region Json to Datatable
        /// <summary>
        /// 将json转换为DataTable
        /// </summary>
        /// <param name="json">得到的json</param>
        /// <returns></returns>
        public static DataTable JsonToDataTable(string json)
        {
            //转换json格式
            json = json.Replace(",\"", "*\"").Replace("\":", "\"#");
            //取出表名   
            var rg = new Regex(@"(?<={)[^:]+(?=:\[)", RegexOptions.IgnoreCase);
            var strName = rg.Match(json).Value;
            DataTable tb = null;
            //去除表名   
            json = json.Substring(json.IndexOf("[", StringComparison.OrdinalIgnoreCase) + 1);
            json = json.Substring(0, json.IndexOf("]", StringComparison.OrdinalIgnoreCase));

            //获取数据   
            rg = new Regex(@"(?<={)[^}]+(?=})");
            var mc = rg.Matches(json);
            for (var i = 0; i < mc.Count; i++)
            {
                var strRow = mc[i].Value;
                var strRows = strRow.Split('*');

                //创建表   
                if (tb == null)
                {
                    tb = new DataTable();
                    tb.TableName = strName;
                    foreach (var str in strRows)
                    {
                        var dc = new DataColumn();
                        var strCell = str.Split('#');

                        if (strCell[0].Substring(0, 1) == "\"")
                        {
                            var a = strCell[0].Length;
                            dc.ColumnName = strCell[0].Substring(1, a - 2);
                        }
                        else
                        {
                            dc.ColumnName = strCell[0];
                        }
                        tb.Columns.Add(dc);
                    }
                    tb.AcceptChanges();
                }

                //增加内容   
                var dr = tb.NewRow();
                for (var r = 0; r < strRows.Length; r++)
                {
                    dr[r] = strRows[r].Split('#')[1].Trim().Replace("，", ",").Replace("：", ":").Replace("\"", "");
                }
                tb.Rows.Add(dr);
                tb.AcceptChanges();
            }

            return tb;
        }
        #endregion
    }
}