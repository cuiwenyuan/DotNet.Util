using NewLife.Data;
using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;

namespace DotNet.Util
{
    /// <summary>安全算法 - 来自NewLife</summary>
    /// <remarks>
    /// 文档 https://newlifex.com/core/utility 采用静态架构，允许外部重载工具类的各种实现System.DefaultConvert。 所有类型转换均支持默认值，默认值为该default(T)，在转换失败时返回默认值。
    /// </remarks>
    public static class DataUtil
    {
        /// <summary>
        /// 转为整数，转换失败时返回默认值。支持字符串、全角、字节数组（小端）、时间（Unix秒不转UTC）
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
        /// <returns>转换后的整数</returns>
        public static int ToInt(this object? value, int defaultValue = 0)
        {
            return NewLife.Utility.ToInt(value, defaultValue);
        }

        /// <summary>
        /// 转为长整数，转换失败时返回默认值。支持字符串、全角、字节数组（小端）、时间（Unix毫秒不转UTC）
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
        /// <returns>转换后的长整数</returns>
        public static long ToLong(this object? value, long defaultValue = 0L)
        {
            return NewLife.Utility.ToLong(value, defaultValue);
        }

        /// <summary>
        /// 转为浮点数，转换失败时返回默认值。支持字符串、全角、字节数组（小端）
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
        /// <returns>转换后的浮点数</returns>
        public static double ToDouble(this object? value, double defaultValue = 0.0)
        {
            return NewLife.Utility.ToDouble(value, defaultValue);
        }

        /// <summary>
        /// 转为高精度浮点数，转换失败时返回默认值。支持字符串、全角、字节数组（小端）
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
        /// <returns>转换后的高精度浮点数</returns>
        public static decimal ToDecimal(this object? value, decimal defaultValue = 0m)
        {
            return NewLife.Utility.ToDecimal(value, defaultValue);
        }

        /// <summary>
        /// 转为布尔型，转换失败时返回默认值。支持大小写True/False、0和非零
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
        /// <returns>转换后的布尔值</returns>
        public static bool ToBoolean(this object? value, bool defaultValue = false)
        {
            return NewLife.Utility.ToBoolean(value, defaultValue);
        }

        /// <summary>
        /// 转为时间日期，转换失败时返回最小时间。支持字符串、整数（Unix秒不考虑UTC转本地）
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <returns>转换后的时间日期</returns>
        public static DateTime ToDateTime(this object? value)
        {
            return NewLife.Utility.ToDateTime(value, DateTime.MinValue);
        }

        /// <summary>
        /// 转为时间日期，转换失败时返回默认值。支持字符串、整数（Unix秒不考虑UTC转本地）
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
        /// <returns>转换后的时间日期</returns>
        public static DateTime ToDateTime(this object? value, DateTime defaultValue)
        {
            return NewLife.Utility.ToDateTime(value, defaultValue);
        }

        /// <summary>
        /// 转为时间日期，转换失败时返回最小时间。支持字符串、整数（Unix秒）
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <returns>转换后的时间日期</returns>
        public static DateTimeOffset ToDateTimeOffset(this object? value)
        {
            return NewLife.Utility.ToDateTimeOffset(value, DateTimeOffset.MinValue);
        }

        /// <summary>
        /// 转为时间日期，转换失败时返回默认值
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
        /// <returns>转换后的时间日期</returns>
        public static DateTimeOffset ToDateTimeOffset(this object? value, DateTimeOffset defaultValue)
        {
            return NewLife.Utility.ToDateTimeOffset(value, defaultValue);
        }

        /// <summary>
        /// 去掉时间日期秒后面部分，可指定毫秒ms、分m、小时h
        /// </summary>
        /// <param name="value">时间日期</param>
        /// <param name="format">格式字符串，默认s格式化到秒，ms格式化到毫秒</param>
        /// <returns>格式化后的时间日期</returns>
        public static DateTime Trim(this DateTime value, string format = "s")
        {
            return NewLife.Utility.Trim(value, format);
        }

        /// <summary>
        /// 去掉时间日期秒后面部分，可指定毫秒
        /// </summary>
        /// <param name="value">时间日期</param>
        /// <param name="format">格式字符串，默认s格式化到秒，ms格式化到毫秒</param>
        /// <returns>格式化后的时间日期</returns>
        public static DateTimeOffset Trim(this DateTimeOffset value, string format = "s")
        {
            return new DateTimeOffset(NewLife.Utility.Trim(value.DateTime, format), value.Offset);
        }

        /// <summary>
        /// 时间日期转为yyyy-MM-dd HH:mm:ss完整字符串，对UTC时间加后缀
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <returns>转换后的字符串</returns>
        public static string ToFullString(this DateTime value)
        {
            return NewLife.Utility.ToFullString(value, useMillisecond: false);
        }

        /// <summary>
        /// 时间日期转为yyyy-MM-dd HH:mm:ss完整字符串，支持指定最小时间的字符串
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="emptyValue">字符串空值时（DateTime.MinValue）显示的字符串，null表示原样显示最小时间，String.Empty表示不显示</param>
        /// <returns>转换后的字符串</returns>
        public static string ToFullString(this DateTime value, string? emptyValue = null)
        {
            return NewLife.Utility.ToFullString(value, useMillisecond: false, emptyValue);
        }

        /// <summary>
        /// 时间日期转为yyyy-MM-dd HH:mm:ss.fff完整字符串，支持指定最小时间的字符串
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="useMillisecond">是否使用毫秒</param>
        /// <param name="emptyValue">字符串空值时（DateTime.MinValue）显示的字符串，null表示原样显示最小时间，String.Empty表示不显示</param>
        /// <returns>转换后的字符串</returns>
        public static string ToFullString(this DateTime value, bool useMillisecond, string? emptyValue = null)
        {
            return NewLife.Utility.ToFullString(value, useMillisecond, emptyValue);
        }

        /// <summary>
        /// 时间日期转为yyyy-MM-dd HH:mm:ss +08:00完整字符串，支持指定最小时间的字符串
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="emptyValue">字符串空值时（DateTimeOffset.MinValue）显示的字符串，null表示原样显示最小时间，String.Empty表示不显示</param>
        /// <returns>转换后的字符串</returns>
        public static string ToFullString(this DateTimeOffset value, string? emptyValue = null)
        {
            return NewLife.Utility.ToFullString(value, useMillisecond: false, emptyValue);
        }

        /// <summary>
        /// 时间日期转为yyyy-MM-dd HH:mm:ss.fff +08:00完整字符串，支持指定最小时间的字符串
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="useMillisecond">是否使用毫秒</param>
        /// <param name="emptyValue">字符串空值时（DateTimeOffset.MinValue）显示的字符串，null表示原样显示最小时间，String.Empty表示不显示</param>
        /// <returns>转换后的字符串</returns>
        public static string ToFullString(this DateTimeOffset value, bool useMillisecond, string? emptyValue = null)
        {
            return NewLife.Utility.ToFullString(value, useMillisecond, emptyValue);
        }

        /// <summary>
        /// 时间日期转为指定格式字符串
        /// </summary>
        /// <param name="value">待转换对象</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="emptyValue">字符串空值时显示的字符串，null表示原样显示最小时间，String.Empty表示不显示</param>
        /// <returns>格式化后的字符串</returns>
        public static string ToString(this DateTime value, string format, string emptyValue)
        {
            return NewLife.Utility.ToString(value, format, emptyValue);
        }

        /// <summary>
        /// 字节单位字符串
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="format">格式化字符串</param>
        /// <returns>格式化后的字符串</returns>
        public static string ToGMK(this ulong value, string format = null)
        {
            return NewLife.Utility.ToGMK(value, format);
        }

        /// <summary>
        /// 获取内部真实异常
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns>内部真实异常</returns>
        public static Exception GetTrue(this Exception ex)
        {
            return NewLife.Utility.GetTrue(ex);
        }

        /// <summary>
        /// 获取异常消息
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns>异常消息字符串</returns>
        public static string GetMessage(this Exception ex)
        {
            return NewLife.Utility.GetMessage(ex);
        }
    }
}