//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
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
        #region 获取子查询条件，这需要处理多个模糊匹配的字符
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
        #endregion

        #region 获取查询字符串

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

        #endregion

        #region 获取查询字符串

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

        #endregion

        #region 判断是否包含的方法

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
                foreach (var i in ids)
                {
                    if (i.Equals(targetString))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        #endregion

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
                foreach (var i in ids)
                {
                    if (i != null)
                    {
                        for (var j = 0; j < i.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(i[j]))
                            {
                                if (!result.Contains(i[j]))
                                {
                                    result.Add(i[j]);
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
                foreach (var i in ids)
                {
                    if (i != null)
                    {
                        if (!Exists(removeIds, i))
                        {
                            keys.Add(i);
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

        #region 字符串转IN查询字符串

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

        #endregion

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
            var sb = PoolUtil.StringBuilder.Get();
            foreach (var t in sourceString)
            {
                int unicode = t;
                if (unicode >= 16)
                {
                    sb.Append(t.ToString());
                }
            }

            return sb.Return();
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
            var sb = PoolUtil.StringBuilder.Get();
            foreach (var t in charbuffers)
            {
                buffer = Encoding.Unicode.GetBytes(t.ToString());
                sb.Append(string.Format("\\u{0:X2}{1:X2}", buffer[1], buffer[0]));
            }

            return sb.Return();
        }

        #region 截取固定长度字符

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

        #endregion

        /// <summary>
        /// 二进制转字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string BytesToHexString(byte[] input)
        {
            var sb = PoolUtil.StringBuilder.Get();
            foreach (var t in input)
            {
                sb.Append(string.Format("{0:X2}", t));
            }

            return sb.Return();
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

        #region
        /// <summary>
        /// 从字符串中截取指定两个字符之间的字符
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="start">开始字符(串)</param>
        /// <param name="end">结束字符(串)</param>
        /// <returns></returns>
        public static string CutString(string source, string start, string end)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(source) && source.Contains(start) && source.Contains(end))
            {
                int startIndex, endIndex;
                try
                {
                    startIndex = source.IndexOf(start);
                    if (startIndex == -1) return result;
                    var tmpstr = source.Substring(startIndex + start.Length);
                    endIndex = tmpstr.IndexOf(end);
                    if (endIndex == -1) return result;
                    result = tmpstr.Remove(endIndex);
                }
                catch (Exception ex)
                {
                    LogUtil.WriteException(ex);
                }
            }

            return result;
        }
        #endregion

        #region 敏感信息处理


        /// <summary>
        /// 隐藏敏感信息
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="left">左边保留的字符数</param>
        /// <param name="right">右边保留的字符数</param>
        /// <param name="basedOnLeft">当长度异常时，是否显示左边 </param>
        /// <returns></returns>
        public static string HideSensitiveInfo(this string info, int left, int right, bool basedOnLeft = true)
        {
            if (string.IsNullOrEmpty(info))
            {
                return "";
            }
            var sb = PoolUtil.StringBuilder.Get();
            var hiddenCharCount = info.Length - left - right;
            if (hiddenCharCount > 0)
            {
                string prefix = info.Substring(0, left), suffix = info.Substring(info.Length - right);
                sb.Append(prefix);
                for (var i = 0; i < hiddenCharCount; i++)
                {
                    sb.Append("*");
                }
                sb.Append(suffix);
            }
            else
            {
                if (basedOnLeft)
                {
                    if (info.Length > left && left > 0)
                    {
                        sb.Append(info.Substring(0, left) + "****");
                    }
                    else
                    {
                        sb.Append(info.Substring(0, 1) + "****");
                    }
                }
                else
                {
                    if (info.Length > right && right > 0)
                    {
                        sb.Append("****" + info.Substring(info.Length - right));
                    }
                    else
                    {
                        sb.Append("****" + info.Substring(info.Length - 1));
                    }
                }
            }
            return sb.Return();
        }

        /// <summary>
        /// 隐藏敏感信息
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="ratio">信息总长与左子串（或右子串）的比例</param>
        /// <param name="basedOnLeft">当长度异常时，是否显示左边，默认true，默认显示左边</param>
        /// <code>true</code>显示左边，<code>false</code>显示右边
        /// <returns></returns>
        public static string HideSensitiveInfo(this string info, int ratio = 3, bool basedOnLeft = true)
        {
            if (string.IsNullOrEmpty(info))
            {
                return "";
            }
            if (ratio <= 1)
            {
                ratio = 3;
            }
            var subLength = info.Length / ratio;
            if (subLength > 0 && info.Length > (subLength * 2))
            {
                string prefix = info.Substring(0, subLength), suffix = info.Substring(info.Length - subLength);
                return prefix + "****" + suffix;
            }
            else
            {
                if (basedOnLeft)
                {
                    var prefix = subLength > 0 ? info.Substring(0, subLength) : info.Substring(0, 1);
                    return prefix + "****";
                }
                else
                {
                    var suffix = subLength > 0 ? info.Substring(info.Length - subLength) : info.Substring(info.Length - 1);
                    return "****" + suffix;
                }
            }
        }

        /// <summary>
        /// 隐藏邮件详情
        /// </summary>
        /// <param name="email">邮件地址</param>
        /// <param name="left">邮件头保留字符个数，默认值设置为3</param>
        /// <returns></returns>
        public static string HideEmailDetails(this string email, int left = 3)
        {
            if (string.IsNullOrEmpty(email))
            {
                return "";
            }
            if (Regex.IsMatch(email, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))//如果是邮件地址
            {
                var suffixLen = email.Length - email.LastIndexOf('@');
                return HideSensitiveInfo(email, left, suffixLen, false);
            }
            else
            {
                return HideSensitiveInfo(email);
            }
        }

        #endregion
    }
}