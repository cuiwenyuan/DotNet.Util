//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DotNet.Util
{
    /// <summary>
    ///	StringUtil
    /// 字符串辅助类
    /// 
    /// 
    /// 修改记录
    /// 
    ///		2024.01.31 版本：1.0	Troy.Cui
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2024.01.31</date>
    /// </author> 
    /// </summary>

    public static partial class StringUtil
    {
        #region 比较
        /// <summary>
        /// 忽略大小写的字符串相等比较，判断是否与任意一个待比较字符串相等
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="strs">待比较字符串数组</param>
        /// <returns></returns>
        public static bool EqualIgnoreCase(this string value, params string[] strs)
        {
            return NewLife.StringHelper.EqualIgnoreCase(value, strs);
        }

        /// <summary>
        /// 忽略大小写的字符串开始比较，判断是否与任意一个待比较字符串开始
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="strs">待比较字符串数组</param>
        /// <returns></returns>
        public static bool StartsWithIgnoreCase(this string value, params string[] strs)
        {
            return NewLife.StringHelper.StartsWithIgnoreCase(value, strs);
        }

        /// <summary>
        /// 忽略大小写的字符串结束比较，判断是否以任意一个待比较字符串结束
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="strs">待比较字符串数组</param>
        /// <returns></returns>
        public static bool EndsWithIgnoreCase(this string value, params string[] strs)
        {
            return NewLife.StringHelper.EndsWithIgnoreCase(value, strs);
        }

        /// <summary>
        /// 指示指定的字符串是 null 还是 String.Empty 字符串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return NewLife.StringHelper.IsNullOrEmpty(value);
        }

        /// <summary>
        /// 是否空或者空白字符串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return NewLife.StringHelper.IsNullOrWhiteSpace(value);
        }
        #endregion

        #region 拆分
        /// <summary>
        /// 拆分字符串，过滤空格，无效时返回空数组
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="separators">分组分隔符，默认逗号分号</param>
        /// <returns></returns>
        public static string[] Split(this string value, params string[] separators)
        {
            return NewLife.StringHelper.Split(value, separators);
        }

        #endregion

        #region 组合

        /// <summary>
        /// 把一个列表组合成为一个字符串，默认逗号分隔
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="separator">组合分隔符，默认逗号</param>
        /// <returns></returns>
        public static string Join(this IEnumerable value, string separator = ",")
        {
            return NewLife.StringHelper.Join(value, separator);
        }

        /// <summary>
        /// 把一个列表组合成为一个字符串，默认逗号分隔
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">值</param>
        /// <param name="separator">组合分隔符，默认逗号</param>
        /// <param name="func">把对象转为字符串的委托</param>
        /// <returns></returns>
        public static string Join<T>(this IEnumerable<T> value, string separator = ",", Func<T, object> func = null)
        {
            return NewLife.StringHelper.Join(value, separator, func);
        }

        /// <summary>
        /// 追加分隔符字符串，忽略开头，常用于拼接
        /// </summary>
        /// <param name="sb">字符串构造者</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static StringBuilder Separate(this StringBuilder sb, string separator)
        {
            return NewLife.StringHelper.Separate(sb, separator);
        }

        #endregion

        #region 截取

        /// <summary>根据最大长度截取字符串，并允许以指定空白填充末尾</summary>
        /// <param name="str">字符串</param>
        /// <param name="maxLength">截取后字符串的最大允许长度，包含后面填充</param>
        /// <param name="pad">需要填充在后面的字符串，比如几个圆点</param>
        /// <returns></returns>
        public static String Cut(this String str, Int32 maxLength, String pad = null)
        {
            return NewLife.StringHelper.Cut(str, maxLength, pad);
        }

        /// <summary>从当前字符串开头移除另一字符串以及之前的部分</summary>
        /// <param name="str">当前字符串</param>
        /// <param name="starts">另一字符串</param>
        /// <returns></returns>
        public static String CutStart(this String str, params String[] starts)
        {
            return NewLife.StringHelper.CutStart(str, starts);
        }

        /// <summary>从当前字符串结尾移除另一字符串以及之后的部分</summary>
        /// <param name="str">当前字符串</param>
        /// <param name="ends">另一字符串</param>
        /// <returns></returns>
        public static String CutEnd(this String str, params String[] ends)
        {
            return NewLife.StringHelper.CutEnd(str, ends);
        }

        #endregion
    }
}