﻿#if NETSTANDARD2_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DotNet.Util
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static class CommonExtensions
    {
        #region ==数据转换扩展==

        /// <summary>
        /// 转换成Byte
        /// </summary>
        /// <param name="s">输入字符串</param>
        /// <returns></returns>
        public static byte ToByte(this object s)
        {
            if (s == null || s == DBNull.Value)
                return 0;

            byte.TryParse(s.ToString(), out byte result);
            return result;
        }

        /// <summary>
        /// 转换成Char
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static char ToChar(this object s)
        {
            if (s == null || s == DBNull.Value)
                return default;

            char.TryParse(s.ToString(), out char result);
            return result;
        }

        /// <summary>
        /// 转换成short/Int16
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static short ToShort(this object s)
        {
            if (s == null || s == DBNull.Value)
                return 0;

            short.TryParse(s.ToString(), out short result);
            return result;
        }

        /// <summary>
        /// 转换成Int/Int32
        /// </summary>
        /// <param name="s"></param>
        /// <param name="round">是否四舍五入，默认false</param>
        /// <returns></returns>
        public static int ToInt(this object s, bool round = false)
        {
            if (s == null || s == DBNull.Value)
                return 0;

            if (s.GetType().IsEnum)
            {
                return (int)s;
            }

            if (s is bool b)
                return b ? 1 : 0;

            if (int.TryParse(s.ToString(), out int result))
                return result;

            var f = s.ToFloat();
            return round ? Convert.ToInt32(f) : (int)f;
        }

        /// <summary>
        /// 转换成Long/Int64
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static long ToLong(this object s)
        {
            if (s == null || s == DBNull.Value)
                return 0L;

            long.TryParse(s.ToString(), out long result);
            return result;
        }

        /// <summary>
        /// 转换成Float/Single
        /// </summary>
        /// <param name="s"></param>
        /// <param name="decimals">小数位数</param>
        /// <returns></returns>
        public static float ToFloat(this object s, int? decimals = null)
        {
            if (s == null || s == DBNull.Value)
                return 0f;

            float.TryParse(s.ToString(), out float result);

            if (decimals == null)
                return result;

            return (float)Math.Round(result, decimals.Value);
        }

        /// <summary>
        /// 转换成Double/Single
        /// </summary>
        /// <param name="s"></param>
        /// <param name="digits">小数位数</param>
        /// <returns></returns>
        public static double ToDouble(this object s, int? digits = null)
        {
            if (s == null || s == DBNull.Value)
                return 0d;

            double.TryParse(s.ToString(), out double result);

            if (digits == null)
                return result;

            return Math.Round(result, digits.Value);
        }

        /// <summary>
        /// 转换成Decimal
        /// </summary>
        /// <param name="s"></param>
        /// <param name="decimals">小数位数</param>
        /// <returns></returns>
        public static decimal ToDecimal(this object s, int? decimals = null)
        {
            if (s == null || s == DBNull.Value) return 0m;

            decimal.TryParse(s.ToString(), out decimal result);

            if (decimals == null)
                return result;

            return Math.Round(result, decimals.Value);
        }

        /// <summary>
        /// 转换成DateTime
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object s)
        {
            if (s == null || s == DBNull.Value)
                return DateTime.MinValue;

            DateTime.TryParse(s.ToString(), out DateTime result);
            return result;
        }

        /// <summary>
        /// 转换成Date
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime ToDate(this object s)
        {
            return s.ToDateTime().Date;
        }

        /// <summary>
        /// 转换成Boolean
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool ToBool(this object s)
        {
            if (s == null) return false;
            s = s.ToString().ToLower();
            if (s.Equals(1) || s.Equals("1") || s.Equals("true") || s.Equals("是") || s.Equals("yes"))
                return true;
            if (s.Equals(0) || s.Equals("0") || s.Equals("false") || s.Equals("否") || s.Equals("no"))
                return false;

            Boolean.TryParse(s.ToString(), out bool result);
            return result;
        }

        /// <summary>
        /// 字符串转Guid
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string s)
        {
            if (s.NotNull() && Guid.TryParse(s, out Guid val))
                return val;

            return Guid.Empty;
        }

        /// <summary>
        /// 字符串转Guid
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Guid ToGuid(this object s)
        {
            if (s != null && Guid.TryParse(s.ToString(), out Guid val))
                return val;

            return Guid.Empty;
        }

        /// <summary>
        /// 泛型转换，转换失败会抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T To<T>(this object s)
        {
            return (T)Convert.ChangeType(s, typeof(T));
        }

        #endregion

        #region ==布尔转换==

        /// <summary>
        /// 布尔值转换为字符串1或者0
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string ToIntString(this bool b)
        {
            return b ? "1" : "0";
        }

        /// <summary>
        /// 布尔值转换为整数1或者0
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int ToInt(this bool b)
        {
            return b ? 1 : 0;
        }

        /// <summary>
        /// 布尔值转换为中文
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string ToZhCn(this bool b)
        {
            return b ? "是" : "否";
        }

        #endregion

    }
}
#endif