using System;
using System.Collections.Generic;
using System.Text;

namespace DotNet.Util
{
    public partial class JsonUtil
    {
        /// <summary>
        /// 分隔Json字符串为字典集合。
        /// </summary>
        internal class JsonSplit
        {
            internal static bool IsJson(string json)
            {
                int errIndex;
                return IsJson(json, out errIndex);
            }
            internal static bool IsJson(string json, out int errIndex)
            {
                errIndex = 0;
                if (!string.IsNullOrEmpty(json) && json.Length > 1 &&
                    ((json[0] == '{' && json[json.Length - 1] == '}') || (json[0] == '[' && json[json.Length - 1] == ']')))
                {
                    var cs = new CharState();
                    char c;
                    for (var i = 0; i < json.Length; i++)
                    {
                        c = json[i];
                        if (SetCharState(c, ref cs) && cs.ChildrenStart)//设置关键符号状态。
                        {
                            var item = json.Substring(i);
                            int err;
                            var length = GetValueLength(item, true, out err);
                            cs.ChildrenStart = false;
                            if (err > 0)
                            {
                                errIndex = i + err;
                                return false;
                            }
                            i = i + length - 1;
                        }
                        if (cs.IsError)
                        {
                            errIndex = i;
                            return false;
                        }
                    }

                    return !cs.ArrayStart && !cs.JsonStart;
                }
                return false;
            }
            internal static List<Dictionary<string, string>> Split(string json)
            {
                var result = new List<Dictionary<string, string>>();

                if (!string.IsNullOrEmpty(json))
                {
                    var dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    var key = string.Empty;
                    var value = PoolUtil.StringBuilder.Get();
                    var cs = new CharState();
                    try
                    {
                        #region 核心逻辑
                        char c;
                        for (var i = 0; i < json.Length; i++)
                        {
                            c = json[i];
                            if (!SetCharState(c, ref cs))//设置关键符号状态。
                            {
                                if (cs.JsonStart)//Json进行中。。。
                                {
                                    if (cs.KeyStart > 0)
                                    {
                                        key += c;
                                    }
                                    else if (cs.ValueStart > 0)
                                    {
                                        value.Append(c);
                                        //value += c;
                                    }
                                }
                                else if (!cs.ArrayStart)//json结束，又不是数组，则退出。
                                {
                                    break;
                                }
                            }
                            else if (cs.ChildrenStart)//正常字符，值状态下。
                            {
                                var item = json.Substring(i);
                                int temp;
                                var length = GetValueLength(item, false, out temp);
                                //value = item.Substring(0, length);
                                value.Length = 0;
                                value.Append(item.Substring(0, length));
                                cs.ChildrenStart = false;
                                cs.ValueStart = 0;
                                //cs.state = 0;
                                cs.SetDicValue = true;
                                i = i + length - 1;
                            }
                            if (cs.SetDicValue)//设置键值对。
                            {
                                if (!string.IsNullOrEmpty(key) && !dic.ContainsKey(key))
                                {
                                    //if (value != string.Empty)
                                    //{
                                    var isNull = json[i - 5] == ':' && json[i] != '"' && value.Length == 4 && value.ToString() == "null";
                                    if (isNull)
                                    {
                                        value.Length = 0;
                                    }
                                    dic.Add(key, value.ToString());

                                    //}
                                }
                                cs.SetDicValue = false;
                                key = string.Empty;
                                value.Length = 0;
                            }

                            if (!cs.JsonStart && dic.Count > 0)
                            {
                                result.Add(dic);
                                if (cs.ArrayStart)//处理数组。
                                {
                                    dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                                }
                            }
                        }
                        #endregion
                    }
                    catch
                    {

                    }
                    finally
                    {
                        key = null;
                        value.Length = 0;
                        value.Capacity = 16;
                        value = null;
                    }
                }
                return result;
            }
            /// <summary>
            /// 获取值的长度（当Json值嵌套以"{"或"["开头时）
            /// </summary>
            private static int GetValueLength(string json, bool breakOnErr, out int errIndex)
            {
                errIndex = 0;
                var len = 0;
                if (!string.IsNullOrEmpty(json))
                {
                    var cs = new CharState();
                    char c;
                    for (var i = 0; i < json.Length; i++)
                    {
                        c = json[i];
                        if (!SetCharState(c, ref cs))//设置关键符号状态。
                        {
                            if (!cs.JsonStart && !cs.ArrayStart)//json结束，又不是数组，则退出。
                            {
                                break;
                            }
                        }
                        else if (cs.ChildrenStart)//正常字符，值状态下。
                        {
                            var length = GetValueLength(json.Substring(i), breakOnErr, out errIndex);//递归子值，返回一个长度。。。
                            cs.ChildrenStart = false;
                            cs.ValueStart = 0;
                            //cs.state = 0;
                            i = i + length - 1;
                        }
                        if (breakOnErr && cs.IsError)
                        {
                            errIndex = i;
                            return i;
                        }
                        if (!cs.JsonStart && !cs.ArrayStart)//记录当前结束位置。
                        {
                            len = i + 1;//长度比索引+1
                            break;
                        }
                    }
                }
                return len;
            }
            /// <summary>
            /// 字符状态
            /// </summary>
            private class CharState
            {
                internal bool JsonStart = false;//以 "{"开始了...
                internal bool SetDicValue = false;// 可以设置字典值了。
                internal bool EscapeChar = false;//以"\"转义符号开始了
                /// <summary>
                /// 数组开始【仅第一开头才算】，值嵌套的以【childrenStart】来标识。
                /// </summary>
                internal bool ArrayStart = false;//以"[" 符号开始了
                internal bool ChildrenStart = false;//子级嵌套开始了。
                /// <summary>
                /// 【0 取名称中】；【1 取值中】
                /// </summary>
                internal int State = -1;

                /// <summary>
                /// 【-1 未初始化】【0 未开始】【1 无引号开始】【2 单引号开始】【3 双引号开始】
                /// </summary>
                internal int KeyStart = -1;
                /// <summary>
                /// 【-1 未初始化】【0 未开始】【1 无引号开始】【2 单引号开始】【3 双引号开始】
                /// </summary>
                internal int ValueStart = -1;
                internal bool IsError = false;//是否语法错误。

                internal void CheckIsError(char c)//只当成一级处理（因为GetLength会递归到每一个子项处理）
                {
                    switch (c)
                    {
                        case '{'://[{ "[{A}]":[{"[{B}]":3,"m":"C"}]}]
                            IsError = JsonStart && State == 0;//重复开始错误 同时不是值处理。
                            break;
                        case '}':
                            IsError = !JsonStart || (KeyStart > -1 && State == 0);//重复结束错误 或者 提前结束。
                            break;
                        case '[':
                            IsError = ArrayStart && State == 0;//重复开始错误
                            break;
                        case ']':
                            IsError = !ArrayStart;//重复开始错误
                            break;
                        case '"':
                        case '\'':
                            IsError = !JsonStart;//未开始Json
                            break;
                        case ':':
                            IsError = !JsonStart || (JsonStart && KeyStart < 2 && ValueStart < 2 && State == 1);//未开始Json 同时 只能处理在取值之前。
                            break;
                        case ',':
                            IsError = (!JsonStart && !ArrayStart) || (JsonStart && KeyStart < 2 && ValueStart < 2 && State == 0);//未开始Json 同时 只能处理在取值之后。
                            break;
                        default: //值开头。。
                            IsError = !JsonStart || (KeyStart == 0 && ValueStart == 0 && State == 0);//
                            break;
                    }
                    //if (isError)
                    //{

                    //}
                }
            }
            /// <summary>
            /// 设置字符状态(返回true则为关键词，返回false则当为普通字符处理）
            /// </summary>
            private static bool SetCharState(char c, ref CharState cs)
            {
                switch (c)
                {
                    case '{'://[{ "[{A}]":[{"[{B}]":3,"m":"C"}]}]
                        #region 大括号
                        if (cs.KeyStart <= 0 && cs.ValueStart <= 0)
                        {
                            cs.CheckIsError(c);
                            if (cs.JsonStart && cs.State == 1)
                            {
                                cs.ValueStart = 0;
                                cs.ChildrenStart = true;
                            }
                            else
                            {
                                cs.State = 0;
                            }
                            cs.JsonStart = true;//开始。
                            return true;
                        }
                        #endregion
                        break;
                    case '}':
                        #region 大括号结束
                        if (cs.KeyStart <= 0 && cs.ValueStart < 2)
                        {
                            cs.CheckIsError(c);
                            if (cs.JsonStart)
                            {
                                cs.JsonStart = false;//正常结束。
                                cs.ValueStart = 0;
                                cs.SetDicValue = true;
                            }
                            return true;
                        }
                        // cs.isError = !cs.jsonStart && cs.state == 0;
                        #endregion
                        break;
                    case '[':
                        #region 中括号开始
                        if (!cs.JsonStart)
                        {
                            cs.CheckIsError(c);
                            cs.ArrayStart = true;
                            return true;
                        }
                        else if (cs.JsonStart && cs.State == 1 && cs.ValueStart < 2)
                        {
                            cs.CheckIsError(c);
                            //cs.valueStart = 1;
                            cs.ChildrenStart = true;
                            return true;
                        }
                        #endregion
                        break;
                    case ']':
                        #region 中括号结束
                        if (!cs.JsonStart && cs.KeyStart <= 0 && cs.ValueStart <= 0)
                        {
                            cs.CheckIsError(c);
                            if (cs.ArrayStart)// && !cs.childrenStart
                            {
                                cs.ArrayStart = false;
                            }
                            return true;
                        }
                        #endregion
                        break;
                    case '"':
                    case '\'':
                        cs.CheckIsError(c);
                        #region 引号
                        if (cs.JsonStart)
                        {
                            if (cs.State == 0)//key阶段
                            {
                                cs.KeyStart = (cs.KeyStart <= 0 ? (c == '"' ? 3 : 2) : 0);
                                return true;
                            }
                            else if (cs.State == 1)//值阶段
                            {
                                if (cs.ValueStart <= 0)
                                {
                                    cs.ValueStart = (c == '"' ? 3 : 2);
                                    return true;
                                }
                                else if ((cs.ValueStart == 2 && c == '\'') || (cs.ValueStart == 3 && c == '"'))
                                {
                                    if (!cs.EscapeChar)
                                    {
                                        cs.ValueStart = 0;
                                        return true;
                                    }
                                    else
                                    {
                                        cs.EscapeChar = false;
                                    }
                                }

                            }
                        }
                        #endregion
                        break;
                    case ':':
                        cs.CheckIsError(c);
                        #region 冒号
                        if (cs.JsonStart && cs.KeyStart < 2 && cs.ValueStart < 2 && cs.State == 0)
                        {
                            cs.KeyStart = 0;
                            cs.State = 1;
                            return true;
                        }
                        // cs.isError = !cs.jsonStart || (cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 1);
                        #endregion
                        break;
                    case ',':
                        cs.CheckIsError(c);
                        #region 逗号
                        if (cs.JsonStart && cs.KeyStart < 2 && cs.ValueStart < 2 && cs.State == 1)
                        {
                            cs.State = 0;
                            cs.ValueStart = 0;
                            cs.SetDicValue = true;
                            return true;
                        }
                        else if (cs.ArrayStart && !cs.JsonStart)
                        {
                            return true;
                        }
                        #endregion
                        break;
                    case ' ':
                    case '\r':
                    case '\n':
                        if (cs.JsonStart && cs.KeyStart <= 0 && cs.ValueStart <= 0)
                        {
                            return true;//跳过空格。
                        }
                        break;
                    default: //值开头。。
                        cs.CheckIsError(c);
                        if (c == '\\') //转义符号
                        {
                            if (cs.EscapeChar)
                            {
                                cs.EscapeChar = false;
                            }
                            else
                            {
                                cs.EscapeChar = true;
                                return true;
                            }
                        }
                        else
                        {
                            cs.EscapeChar = false;
                        }
                        if (cs.JsonStart)
                        {
                            if (cs.KeyStart <= 0 && cs.State <= 0)
                            {
                                cs.KeyStart = 1;//无引号的
                            }
                            else if (cs.ValueStart <= 0 && cs.State == 1)
                            {
                                cs.ValueStart = 1;//无引号的
                            }
                        }
                        break;
                }
                return false;
            }


        }
    }
}
