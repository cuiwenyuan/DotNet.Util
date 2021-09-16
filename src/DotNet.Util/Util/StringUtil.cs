﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNet.Util
{
    /// <summary>
    ///	StringUtil
    /// 字符串辅助类
    /// 
    /// 
    /// 修改记录
    /// 
    ///		2016.01.12 版本：1.0	SongBiao
    ///	
    /// <author>
    ///		<name>SongBiao</name>
    ///		<date>2016.01.12</date>
    /// </author> 
    /// </summary>

    public static partial class StringUtil
    {
        /// <summary>
        /// 获取子查询条件，这需要处理多个模糊匹配的字符
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="search">模糊查询</param>
        /// <returns>表达式</returns>
        public static string GetLike(string field, string search)
        {
            var result = string.Empty;
            foreach (var t in search)
            {
                result += field + " LIKE '%" + t + "%' AND ";
            }

            if (!string.IsNullOrEmpty(result))
            {
                result = result.Substring(0, result.Length - 5);
            }

            result = "(" + result + ")";
            return result;
        }

        /// <summary>
        /// 获取查询字符串
        /// </summary>
        /// <param name="searchKey">查询字符</param>
        /// <param name="allLike">是否所有的匹配都查询，建议传递"%"字符</param>
        /// <returns>字符串</returns>
        public static string GetSearchString(string searchKey, bool allLike = false)
        {
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = searchKey.Trim();
                searchKey = SecretUtil.SqlSafe(searchKey);
                if (searchKey.Length > 0)
                {
                    searchKey = searchKey.Replace('[', '_');
                    searchKey = searchKey.Replace(']', '_');
                }

                if (searchKey == "%")
                {
                    searchKey = "[%]";
                }

                if ((searchKey.Length > 0) && (searchKey.IndexOf('%') < 0) && (searchKey.IndexOf('_') < 0))
                {
                    if (allLike)
                    {
                        var searchLike = string.Empty;
                        for (var i = 0; i < searchKey.Length; i++)
                        {
                            searchLike += "%" + searchKey[i];
                        }

                        searchKey = searchLike + "%";
                    }
                    else
                    {
                        searchKey = "%" + searchKey + "%";
                    }
                }
            }

            return searchKey;
        }

        /// <summary>
        /// 获取查询字符串
        /// </summary>
        /// <param name="searchKey">查询字符</param>
        /// <returns>字符串</returns>
        public static string GetLikeSearchKey(string searchKey)
        {
            if (!string.IsNullOrEmpty(searchKey))
            {
                //必须[放在%替换前面
                searchKey = searchKey.Replace("[", "[[]");
                searchKey = searchKey.Replace("%", "[%]");
                searchKey = searchKey.Replace("_", "[_]");
                searchKey = searchKey.Replace("^", "[^]");
                searchKey = searchKey.Replace("*", "[*]");
            }

            return searchKey;
        }

        /// <summary>
        /// 判断是否包含的方法
        /// </summary>
        /// <param name="ids">数组</param>
        /// <param name="targetString">目标值</param>
        /// <returns>包含</returns>
        public static bool Exists(string[] ids, string targetString)
        {
            var result = false;

            if (ids != null && !string.IsNullOrEmpty(targetString))
            {
                foreach (var t in ids)
                {
                    if (t.Equals(targetString))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 将字符串加入到字符串数组
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string[] Concat(string[] ids, string id)
        {
            return Concat(ids, new string[] { id });
        }

        /// <summary>
        /// 合并数组
        /// </summary>
        /// <param name="ids">数组</param>
        /// <returns>数组</returns>
        public static string[] Concat(params string[][] ids)
        {
            // 进行合并
            var result = new List<string>();

            if (ids != null)
            {
                foreach (var t in ids)
                {
                    if (t != null)
                    {
                        for (var j = 0; j < t.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(t[j]))
                            {
                                if (!result.Contains(t[j]))
                                {
                                    result.Add(t[j]);
                                }
                            }
                        }
                    }
                }
            }

            //  返回合并结果
            return result.ToArray();
        }

        /// <summary>
        /// 从目标数组中去除某个数组
        /// </summary>
        /// <param name="ids">数组</param>
        /// <param name="removeIds">目标值数组</param>
        /// <returns>数组</returns>
        public static string[] Remove(string[] ids, string[] removeIds)
        {
            // 进行合并
            var keys = new List<string>();
            if (ids != null)
            {
                foreach (var t in ids)
                {
                    if (t != null)
                    {
                        if (!Exists(removeIds, t))
                        {
                            keys.Add(t);
                        }
                    }
                }
            }

            // 返回合并结果
            return keys.ToArray();
        }

        /// <summary>
        /// 从字符串数组中移除指定字符
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string[] Remove(string[] ids, string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                return Remove(ids, new string[] { id });
            }

            return ids;
        }

        /// <summary>
        /// 字符串转IN查询字符串
        /// </summary>
        /// <param name="id"></param>
        /// <param name="separativeSign"></param>
        /// <param name="newSeparativeSign"></param>
        /// <returns></returns>
        public static string StringToInList(string id, string separativeSign = ",", string newSeparativeSign = "','")
        {
            //var ids = id.Split(separativeSign.ToCharArray());
            //return ArrayToList(ids, string.Empty);
            return id.TrimEnd(separativeSign.ToCharArray()).Replace(separativeSign, newSeparativeSign);

        }

        /// <summary>
        /// 数组转字符串
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static string ArrayToList(string[] ids)
        {
            return ArrayToList(ids, string.Empty);
        }

        /// <summary>
        /// 数组转字符串 指定分隔符
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="separativeSign"></param>
        /// <returns></returns>
        public static string ArrayToList(string[] ids, string separativeSign)
        {
            var rowCount = 0;
            var result = string.Empty;
            foreach (var id in ids)
            {
                rowCount++;
                result += separativeSign + id + separativeSign + ",";
            }

            if (rowCount == 0)
            {
                result = "";
            }
            else
            {
                result = result.TrimEnd(',');
            }

            return result;
        }

        /// <summary>
        /// 重复字符串
        /// </summary>
        /// <param name="targetString">目标字符串</param>
        /// <param name="repeatCount">重复次数</param>
        /// <returns>结果字符串</returns>
        public static string RepeatString(string targetString, int repeatCount)
        {
            var result = string.Empty;
            for (var i = 0; i < repeatCount; i++)
            {
                result += targetString;
            }

            return result;
        }

        /// <summary>
        /// 删除不可见字符
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static string DeleteUnVisibleChar(string sourceString)
        {
            var sb = Pool.StringBuilder.Get();
            foreach (var t in sourceString)
            {
                int unicode = t;
                if (unicode >= 16)
                {
                    sb.Append(t.ToString());
                }
            }

            return sb.Put();
        }

        /// <summary>
        /// 手机号码分割
        /// </summary>
        /// <param name="mobiles"></param>
        /// <param name="mobileOnly"></param>
        /// <param name="distinct"></param>
        /// <returns></returns>
        public static string[] SplitMobile(string mobiles, bool mobileOnly = true, bool distinct = true)
        {
            // 把常用的风格符号都用上，全角半角的
            mobiles = Regex.Replace(mobiles.Trim(), "\\s+", " ");
            var mobile = mobiles.Replace("\r\n", "\r").Replace("，", ",").Replace("；", ",").Replace(";", ",")
                .Split(' ');
            var mobileList = new List<string>();
            foreach (var cellPhone in mobile)
            {
                var phones = cellPhone.Trim();
                if (!string.IsNullOrEmpty(phones))
                {
                    // 用回车分割，然后再用,符号分割
                    var phone = phones.Split(',');
                    foreach (var p in phone)
                    {
                        // 手机号码长度对的，才可以发送
                        if (mobileOnly && p.Trim().Length == 11)
                        {
                            mobileList.Add(p.Trim());
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(p.Trim()))
                            {
                                mobileList.Add(p.Trim());
                            }
                        }
                    }
                }
            }

            // 去掉重复，不要空的，有时候需要发重复的短信的，因为有多个包裹时，需要有重复的信息
            if (distinct)
            {
                mobile = mobileList.Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
            }
            else
            {
                mobile = mobileList.Where(t => !string.IsNullOrEmpty(t)).ToArray();
            }

            return mobile;
        }

        /// <summary>  
        /// 字符串转为UniCode码字符串  
        /// 避免生成的json中有特殊字符造成的问题
        /// </summary>  
        /// <param name="target"></param>  
        /// <returns></returns>  
        public static string StringToUnicode(string target)
        {
            target = string.Equals(target, "N/A", StringComparison.OrdinalIgnoreCase) ? "" : target;
            var charbuffers = target.ToCharArray();
            byte[] buffer;
            var sb = Pool.StringBuilder.Get();
            foreach (var t in charbuffers)
            {
                buffer = Encoding.Unicode.GetBytes(t.ToString());
                sb.Append(string.Format("\\u{0:X2}{1:X2}", buffer[1], buffer[0]));
            }

            return sb.Put();
        }

        /// <summary>
        /// 截取固定长度字符
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string CutString(string inputString, int len)
        {
            var ascii = new ASCIIEncoding();
            var tempLen = 0;
            var tempString = "";
            var s = ascii.GetBytes(inputString);
            for (var i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }

                try
                {
                    tempString += inputString.Substring(i, 1);
                }
                catch
                {
                    break;
                }

                if (tempLen > len)
                    break;
            }

            //如果截过则加上半个省略号
            var mybyte = Encoding.Default.GetBytes(inputString);
            if (mybyte.Length > len)
                tempString += "..";
            return tempString;
        }

        /// <summary>
        /// 二进制转字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string BytesToHexString(byte[] input)
        {
            var sb = Pool.StringBuilder.Get();
            foreach (var t in input)
            {
                sb.Append(string.Format("{0:X2}", t));
            }

            return sb.Put();
        }

        /// <summary>
        /// 16进制转二进制
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(string hex)
        {
            if (hex.Length == 0)
            {
                return new byte[] { 0 };
            }

            if (hex.Length % 2 == 1)
            {
                hex = "0" + hex;
            }

            var result = new byte[hex.Length / 2];

            for (var i = 0; i < hex.Length / 2; i++)
            {
                result[i] = byte.Parse(hex.Substring(2 * i, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            return result;
        }

        #region 字符串中多个连续空格转为一个空格
        /// <summary>
        /// 字符串中多个连续空格转为一个空格
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>合并空格后的字符串</returns>
        public static string MergeSpace(string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 0)
            {
                str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(str, " ");
            }
            return str;
        }
        #endregion
    }
}