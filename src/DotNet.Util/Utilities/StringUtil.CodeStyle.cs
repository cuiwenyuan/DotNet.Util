//-----------------------------------------------------------------
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
    ///		2018.03.15 版本：1.0	Troy.Cui
    ///	
    /// <author>
    ///		<name>Troy Cui</name>
    ///		<date>2018.03.15</date>
    /// </author> 
    /// </summary>

    public static partial class StringUtil
    {
        #region 代码风格化

        /// <summary>
        /// 转换为Pascal风格-每一个单词的首字母大写
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="fieldDelimiter">分隔符</param>
        /// <returns></returns>
        public static string ConvertToPascal(string fieldName, string fieldDelimiter)
        {
            var result = string.Empty;
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                result = fieldName;
            }
            else
            {
                if (fieldName.Length == 1)
                {
                    result = fieldName.ToUpper();
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(fieldDelimiter))
                    {
                        result = fieldName.Substring(0, 1).ToUpper() + fieldName.Substring(1);
                    }
                    else
                    {
                        if (fieldName.Contains(fieldDelimiter))
                        {
                            //全部小写
                            var array = fieldName.ToLower().Split(fieldDelimiter.ToCharArray());
                            foreach (var t in array)
                            {
                                //万一多个分隔符连续，会有空值产生
                                if (!string.IsNullOrWhiteSpace(t))
                                {
                                    //首字母大写
                                    result += t.Substring(0, 1).ToUpper() + t.Substring(1);
                                }
                            }
                        }
                        else if (IsAllEnglishLetterUpperCase(fieldName))
                        {
                            //如果字段中的英文字符全部都是大写的，特别针对Oracle数据库的字段
                            result = fieldName.Substring(0, 1).ToUpper() + fieldName.Substring(1).ToLower();
                        }
                        else
                        {
                            result = fieldName.Substring(0, 1).ToUpper() + fieldName.Substring(1);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 转换为Camel风格-第一个单词小写，其后每个单词首字母大写
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="fieldDelimiter">分隔符</param>
        /// <returns></returns>
        public static string ConvertToCamel(string fieldName, string fieldDelimiter)
        {
            //先Pascal
            var result = ConvertToPascal(fieldName, fieldDelimiter);
            //然后首字母小写
            if (result.Length == 1)
            {
                result = result.ToLower();
            }
            else
            {
                result = result.Substring(0, 1).ToLower() + result.Substring(1);
            }

            return result;
        }

        /// <summary>
        /// 判断给定字符串中是否所有的英文字母都是大写的
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool IsAllEnglishLetterUpperCase(string fieldName)
        {
            var result = true;
            foreach (var t in fieldName)
            {
                //先判断是否是英文字母
                if (Regex.IsMatch(t.ToString(), "[a-zA-Z]+"))
                {
                    //再判断是否是小写
                    if (Regex.IsMatch(t.ToString(), "[a-z]+"))
                    {
                        result = false;
                        //立即跳出循环
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 判断给定字符串中是否所有的英文字母都是小写的
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool IsAllEnglishLetterLowerCase(string fieldName)
        {
            var result = true;
            foreach (var t in fieldName)
            {
                //先判断是否是英文字母
                if (Regex.IsMatch(t.ToString(), "[a-zA-Z]+"))
                {
                    //再判断是否是大写
                    if (Regex.IsMatch(t.ToString(), "[A-Z]+"))
                    {
                        result = false;
                        //立即跳出循环
                        break;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// 替换制定字符串中第一个指定字符为替代字符
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldDelimiter"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceFirst(string fieldName, string fieldDelimiter, string replacement)
        {
            var result = fieldName;
            if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(fieldDelimiter))
            {
                var reg = new Regex(fieldDelimiter);
                if (reg.IsMatch(fieldName))
                {
                    result = reg.Replace(fieldName, replacement, 1);
                }
            }
            return result;
        }

        #endregion
    }
}