//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Util
{
    /// <summary>
    ///	BaseUtil
    /// 通用基类
    /// 
    /// 
    /// 修改记录
    /// 
    ///		2021.12.31 版本：5.1   Troy.Cui重构
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2021.12.31</date>
    /// </author> 
    /// </summary>
    public partial class BaseUtil
    {
        #region ConvertTo
        /// <summary>
        /// 转为布尔型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static Boolean ConvertToBoolean(Object targetValue)
        {
            return targetValue != DBNull.Value && (targetValue.ToString().Equals(true.ToString()) || targetValue.ToString().Equals("1"));
        }
        /// <summary>
        /// 转为字符串
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static string ConvertToString(Object targetValue)
        {
            return targetValue != DBNull.Value ? Convert.ToString(targetValue) : null;
        }
        /// <summary>
        /// 转为整型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ConvertToInt(Object targetValue, int defaultValue = 0)
        {
            var returnValue = defaultValue;
            if (targetValue != DBNull.Value)
            {
                if (int.TryParse(targetValue.ToString(), out var result)) returnValue = result;
            }
            return returnValue;
        }
        /// <summary>
        /// 转为可为NULL的整型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static int? ConvertToNullableInt(Object targetValue)
        {
            int? returnValue = null;
            if (targetValue != DBNull.Value)
            {
                if (int.TryParse(targetValue.ToString(), out var result)) returnValue = result;
            }
            return returnValue;
        }
        /// <summary>
        /// 转为字节整型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static Byte ConvertToByteInt(Object targetValue)
        {
            Byte returnValue = 0;
            if (targetValue != DBNull.Value)
            {
                if (Byte.TryParse(targetValue.ToString(), out var result)) returnValue = result;
            }
            return returnValue;
        }
        /// <summary>
        /// 转为可为NULL字节整型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static Byte? ConvertToNullableByteInt(Object targetValue)
        {
            Byte? returnValue = null;
            if (targetValue != DBNull.Value)
            {
                if (Byte.TryParse(targetValue.ToString(), out var result)) returnValue = result;
            }
            return returnValue;
        }
        /// <summary>
        /// 转为32位整型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static Int32 ConvertToInt32(Object targetValue)
        {
            var returnValue = 0;
            if (targetValue != DBNull.Value)
            {
                if (Int32.TryParse(targetValue.ToString(), out var result)) returnValue = result;
            }
            return returnValue;
        }
        /// <summary>
        /// 转为可为NULL的32位整型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static Int32? ConvertToNullableInt32(Object targetValue)
        {
            Int32? returnValue = null;
            if (targetValue != DBNull.Value)
            {
                if (Int32.TryParse(targetValue.ToString(), out var result)) returnValue = result;
            }
            return returnValue;
        }
        /// <summary>
        /// 转为64位整型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static Int64 ConvertToInt64(Object targetValue)
        {
            Int64 returnValue = 0;
            if (targetValue != DBNull.Value)
            {
                if (Int64.TryParse(targetValue.ToString(), out var result)) returnValue = result;
            }
            return returnValue;
        }
        /// <summary>
        /// 转为可为NULL的64位整型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static Int64? ConvertToNullableInt64(Object targetValue)
        {
            Int64? returnValue = null;
            if (targetValue != DBNull.Value)
            {
                if (Int64.TryParse(targetValue.ToString(), out var result)) returnValue = result;
            }
            return returnValue;
        }
        /// <summary>
        /// 转为Long类型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static long ConvertToLong(Object targetValue)
        {
            long returnValue = 0;
            if (targetValue != DBNull.Value)
            {
                if (long.TryParse(targetValue.ToString(), out var result)) returnValue = result;
            }
            return returnValue;
        }
        /// <summary>
        /// 转为可为NULL的Long类型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static long? ConvertToNullableLong(Object targetValue)
        {
            long? returnValue = null;
            if (targetValue != DBNull.Value)
            {
                if (long.TryParse(targetValue.ToString(), out var result)) returnValue = result;
            }
            return returnValue;
        }
        /// <summary>
        /// 转为Double类型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static Double ConvertToDouble(Object targetValue)
        {
            Double returnValue = 0;
            if (targetValue != DBNull.Value)
            {
                if (Double.TryParse(targetValue.ToString(), out var result)) returnValue = result;
            }
            return returnValue;
        }
        /// <summary>
        /// 转为可为NULL的Double类型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static Double? ConvertToNullableDouble(Object targetValue)
        {
            Double? returnValue = null;
            if (targetValue != DBNull.Value)
            {
                if (Double.TryParse(targetValue.ToString(), out var result)) returnValue = result;
            }
            return returnValue;
        }
        /// <summary>
        /// 转为Float类型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static float ConvertToFloat(Object targetValue)
        {
            float returnValue = 0;
            if (targetValue != DBNull.Value)
            {
                if (float.TryParse(targetValue.ToString(), out var result)) returnValue = result;
            }
            return returnValue;
        }
        /// <summary>
        /// 转为可为NULL的Float类型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static float? ConvertToNullableFloat(Object targetValue)
        {
            float? returnValue = null;
            if (targetValue != DBNull.Value)
            {
                if (float.TryParse(targetValue.ToString(), out var result)) returnValue = result;
            }
            return returnValue;
        }
        /// <summary>
        /// 转为Decimal类型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static decimal ConvertToDecimal(Object targetValue)
        {
            decimal returnValue = 0;
            if (targetValue != DBNull.Value)
            {
                if (decimal.TryParse(targetValue.ToString(), out var result)) returnValue = result;
            }
            return returnValue;
        }
        /// <summary>
        /// 转为可为NULL的Decimal类型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static decimal? ConvertToNullableDecimal(Object targetValue)
        {
            decimal? returnValue = null;
            if (targetValue != DBNull.Value)
            {
                if (decimal.TryParse(targetValue.ToString(), out var result)) returnValue = result;
            }
            return returnValue;
        }
        /// <summary>
        /// 转为时间类型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(Object targetValue)
        {
            var returnValue = DateTime.MinValue;

            if (targetValue != DBNull.Value)
            {
                returnValue = Convert.ToDateTime(targetValue.ToString());
            }

            return returnValue;
        }
        /// <summary>
        /// 转为可为NULL的时间类型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static DateTime? ConvertToNullableDateTime(Object targetValue)
        {
            DateTime? returnValue = null;
            if (targetValue != DBNull.Value)
            {
                if (DateTime.TryParse(targetValue.ToString(), out var dt)) returnValue = dt;
            }

            return returnValue;
        }
        /// <summary>
        /// 转为日期类型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static string ConvertToDateToString(Object targetValue)
        {
            var returnValue = string.Empty;
            returnValue = targetValue != DBNull.Value ? DateTime.Parse(targetValue.ToString()).ToString(BaseSystemInfo.DateFormat) : null;
            return returnValue;
        }
        /// <summary>
        /// 转为字节类型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static byte[] ConvertToByte(Object targetValue)
        {
            return targetValue != DBNull.Value ? (byte[])targetValue : null;
        }

        #endregion

        #region 通用类型转换
        /// <summary>
        /// 通用类型转换Convert.ChangeType
        /// </summary>
        /// <param name="value">字段的值</param>
        /// <param name="type">目标字段的类型</param>
        /// <returns></returns>
        public static object ChangeType(object value, Type type)
        {
            //Type nullableType = Nullable.GetUnderlyingType(type);
            //if (nullableType != null)
            //{ }
            if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
            if (value == null) return null;
            if (type == value.GetType()) return value;
            if (type.IsEnum)
            {
                if (value is string)
                {
                    return Enum.Parse(type, value as string);
                }
                else
                {
                    return Enum.ToObject(type, value);
                }
            }
            if (!type.IsInterface && type.IsGenericType)
            {
                var innerType = type.GetGenericArguments()[0];
                var innerValue = ChangeType(value, innerType);
                return Activator.CreateInstance(type, new object[] { innerValue });
            }
            if (value is string && type == typeof(Guid)) return new Guid(value as string);
            if (value is string && type == typeof(Version)) return new Version(value as string);
            if (!(value is IConvertible)) return value;
            return Convert.ChangeType(value, type);
        }

        /// <summary>
        /// 判断指定对象是否是有效值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNullOrDbNull(object obj)
        {
            return (obj == null || (obj is DBNull)) ? true : false;
        }

        #endregion
    }
}