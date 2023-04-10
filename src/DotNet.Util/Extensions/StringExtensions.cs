#if NETSTANDARD2_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNet.Util
{
    /// <summary>
    /// 类型扩展
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// Windows Platform
        /// </summary>
        public static bool _windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        /// <summary>
        /// ReplacePath
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReplacePath(this string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";
            if (_windows)
                return path.Replace("/", "\\");
            return path.Replace("\\", "/");

        }
        private static DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);

        private static long longTime = 621355968000000000;

        private static int samllTime = 10000000;
        /// <summary>
        /// 获取时间戳 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetTimeStamp(this DateTime dateTime)
        {
            return (dateTime.ToUniversalTime().Ticks - longTime) / samllTime;
        }
        /// <summary>
        /// 时间戳转换成日期
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime GetTimeSpmpToDate(this object timeStamp)
        {
            if (timeStamp == null) return dateStart;
            var dateTime = new DateTime(longTime + Convert.ToInt64(timeStamp) * samllTime, DateTimeKind.Utc).ToLocalTime();
            return dateTime;
        }
        //public static string CreateHtmlParas(this string urlPath, int? userId = null)
        //{
        //    if (string.IsNullOrEmpty(urlPath))
        //        return null;
        //    userId = userId ?? UserContext.Current.UserInfo.User_Id;
        //    return $"{urlPath}{(urlPath.IndexOf("?token") > 0 ? "&" : "?")}uid={userId}&rt_v={DateTime.Now.ToString("HHmmss")}";
        //    //  return urlPath + ((urlPath.IndexOf("?token") > 0 ? "&" : "?") + "uid=" + userId);
        //}

        /// <summary>
        /// IsUrl
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsUrl(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            var Url = @"(http://)?([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
            return Regex.IsMatch(str, Url);

        }
        /// <summary>
        /// 判断是不是正确的手机号码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsPhoneNo(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;
            if (input.Length < 11)
                return false;

            if (new Regex(@"^1[3578][01379]\d{8}$").IsMatch(input)
                || new Regex(@"^1[34578][01256]\d{8}").IsMatch(input)
                || new Regex(@"^(1[012345678]\d{8}|1[345678][0123456789]\d{8})$").IsMatch(input)
                )
                return true;
            return false;
        }

        /// <summary>
        /// GetGuid
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="outId"></param>
        /// <returns></returns>
        public static bool GetGuid(this string guid, out Guid outId)
        {
            var emptyId = Guid.Empty;
            return Guid.TryParse(guid, out outId);
        }

        /// <summary>
        /// IsGuid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static bool IsGuid(this string guid)
        {
            Guid newId;
            return guid.GetGuid(out newId);
        }

        /// <summary>
        /// IsInt
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsInt(this object obj)
        {
            if (obj == null)
                return false;
            var reslut = Int32.TryParse(obj.ToString(), out var _number);
            return reslut;

        }

        /// <summary>
        /// IsDate
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDate(this object str)
        {
            var dateTime = DateTime.Now;
            if (str.IsDate(out dateTime))
            {
                if (dateTime.Year <= 1990)
                    return false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// IsDate
        /// </summary>
        /// <param name="str"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool IsDate(this object str, out DateTime dateTime)
        {
            dateTime = DateTime.Now;
            if (str == null || str.ToString() == "")
            {
                return false;
            }
            return DateTime.TryParse(str.ToString(), out dateTime);
        }

        /// <summary>
        /// 根据传入格式判断是否为小数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="formatString">18,5</param>
        /// <returns></returns>
        public static bool IsNumber(this string str, string formatString)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            var precision = 32;
            var scale = 5;
            try
            {
                var numbers = formatString.Split(',');
                precision = Convert.ToInt32(numbers[0]);
                scale = Convert.ToInt32(numbers[1]);
            }
            catch { };
            return IsNumber(str, precision, scale);
        }
        /**/
        /// <summary>
        /// 判断一个字符串是否为合法数字(指定整数位数和小数位数)
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="precision">整数位数</param>
        /// <param name="scale">小数位数</param>
        /// <returns></returns>
        public static bool IsNumber(this string str, int precision, int scale)
        {
            if ((precision == 0) && (scale == 0))
            {
                return false;
            }
            var pattern = @"(^\d{1," + precision + "}";
            if (scale > 0)
            {
                pattern += @"\.\d{0," + scale + "}$)|" + pattern;
            }
            pattern += "$)";
            return Regex.IsMatch(str, pattern);
        }

        /// <summary>
        /// IsNullOrEmpty
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this object str)
        {
            if (str == null)
                return true;
            return str.ToString() == "";
        }

        /// <summary>
        /// GetInt
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetInt(this object obj)
        {
            if (obj == null)
                return 0;
            var _number = 0;
            var reslut = Int32.TryParse(obj.ToString(), out _number);
            return _number;

        }

        /// <summary>
        /// 获取 object 中的枚举值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static long GetLong(this object obj)
        {
            if (obj == null)
                return 0;

            try
            {
                return Convert.ToInt64(Convert.ToDouble(obj));
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取 object 中的 float
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        public static float GetFloat(this object obj)
        {
            if (System.DBNull.Value.Equals(obj) || null == obj)
                return 0;

            try
            {
                return float.Parse(obj.ToString());
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// GetDouble
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetDouble(this object obj)
        {
            if (System.DBNull.Value.Equals(obj) || null == obj)
                return 0;

            try
            {
                return Convert.ToDouble(obj);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取 object 中的 decimal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static decimal GetDecimal(this object obj)
        {
            if (System.DBNull.Value.Equals(obj) || null == obj)
                return 0;

            try
            {
                return Convert.ToDecimal(obj);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取 object 中的 decimal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static dynamic GetDynamic(this object obj)
        {
            if (System.DBNull.Value.Equals(obj) || null == obj)
                return null;

            try
            {
                var str = obj.ToString();
                if (str.IsNumber(25, 15)) return Convert.ToDecimal(obj);
                else return str;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// GetDateTime
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime? GetDateTime(this object obj)
        {
            if (System.DBNull.Value.Equals(obj) || null == obj)
                return null;
            var result = DateTime.TryParse(obj.ToString(), out var dateTime);
            if (!result)
                return null;
            return dateTime;
        }

        /// <summary>
        /// ParseTo
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ParseTo(this string str, string type)
        {
            switch (type)
            {
                case "System.Boolean":
                    return ToBoolean(str);
                case "System.SByte":
                    return ToSByte(str);
                case "System.Byte":
                    return ToByte(str);
                case "System.UInt16":
                    return ToUInt16(str);
                case "System.Int16":
                    return ToInt16(str);
                case "System.uInt32":
                    return ToUInt32(str);
                case "System.Int32":
                    return str.ToInt32();
                case "System.UInt64":
                    return ToUInt64(str);
                case "System.Int64":
                    return ToInt64(str);
                case "System.Single":
                    return ToSingle(str);
                case "System.Double":
                    return ToDouble(str);
                case "System.Decimal":
                    return ToDecimal(str);
                case "System.DateTime":
                    //return ToDateTime(str);
                case "System.Guid":
                    return ToGuid(str);
            }
            throw new NotSupportedException(string.Format("The string of \"{0}\" can not be parsed to {1}", str, type));
        }

        /// <summary>
        /// ToSByte
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static sbyte? ToSByte(this string value)
        {
            sbyte value2;
            if (sbyte.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }

        /// <summary>
        /// ToByte
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static byte? ToByte(this string value)
        {
            byte value2;
            if (byte.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }

        /// <summary>
        /// ToUInt16
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static ushort? ToUInt16(this string value)
        {
            ushort value2;
            if (ushort.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }
        /// <summary>
        /// ToInt16
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static short? ToInt16(this string value)
        {
            if (short.TryParse(value, out var value2))
            {
                return value2;
            }
            return null;
        }
        /// <summary>
        /// ToUInt32
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static uint? ToUInt32(this string value)
        {
            uint value2;
            if (uint.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }
        /// <summary>
        /// ToUInt64
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static ulong? ToUInt64(this string value)
        {
            ulong value2;
            if (ulong.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }
        /// <summary>
        /// ToInt64
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static long? ToInt64(this string value)
        {
            long value2;
            if (long.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }
        /// <summary>
        /// ToSingle
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static float? ToSingle(this string value)
        {
            float value2;
            if (float.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }
        /// <summary>
        /// ToDouble
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static double? ToDouble(this string value)
        {
            double value2;
            if (double.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }
        /// <summary>
        /// ToDecimal
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static decimal? ToDecimal(this string value)
        {
            decimal value2;
            if (decimal.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }
        /// <summary>
        /// ToBoolean
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool? ToBoolean(this string value)
        {
            bool value2;
            if (bool.TryParse(value, out value2))
            {
                return value2;
            }
            return null;
        }

        /// <summary>
        /// ToGuid
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Guid? ToGuid(this string str)
        {
            Guid value;
            if (Guid.TryParse(str, out value))
            {
                return value;
            }
            return null;
        }


        /// <summary>
        /// ToInt32
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int? ToInt32(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }
            int value;
            if (int.TryParse(input, out value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// 替换空格字符
        /// </summary>
        /// <param name="input"></param>
        /// <param name="replacement">替换为该字符</param>
        /// <returns>替换后的字符串</returns>
        public static string ReplaceWhitespace(this string input, string replacement = "")
        {
            return string.IsNullOrEmpty(input) ? null : Regex.Replace(input, "\\s", replacement, RegexOptions.Compiled);
        }

        private static char[] randomConstant ={
        '0','1','2','3','4','5','6','7','8','9',
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
        'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
      };
        /// <summary>
        /// 生成指定长度的随机数
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomNumber(this int length)
        {
            var sb = new StringBuilder(62);
            var rd = new Random();
            for (var i = 0; i < length; i++)
            {
                sb.Append(randomConstant[rd.Next(62)]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 判断属性是否是静态的
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool IsStatic(this PropertyInfo property) => (property.GetMethod ?? property.SetMethod).IsStatic;

        /// <summary>
        /// 判断指定类型是否实现于该类型
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementType"></param>
        /// <returns></returns>
        public static bool IsImplementType(this Type serviceType, Type implementType)
        {
            //泛型
            if (serviceType.IsGenericType)
            {
                if (serviceType.IsInterface)
                {
                    var interfaces = implementType.GetInterfaces();
                    if (interfaces.Any(m => m.IsGenericType && m.GetGenericTypeDefinition() == serviceType))
                    {
                        return true;
                    }
                }
                else
                {
                    if (implementType.BaseType != null && implementType.BaseType.IsGenericType && implementType.BaseType.GetGenericTypeDefinition() == serviceType)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (serviceType.IsInterface)
                {
                    var interfaces = implementType.GetInterfaces();
                    if (interfaces.Any(m => m == serviceType))
                        return true;
                }
                else
                {
                    if (implementType.BaseType != null && implementType.BaseType == serviceType)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断是否继承自指定的泛型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="generic"></param>
        /// <returns></returns>
        public static bool IsSubclassOfGeneric(this Type type, Type generic)
        {
            while (type != null && type != typeof(object))
            {
                var cur = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (generic == cur)
                {
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }

        ///// <summary>
        ///// 判断是否可空类型
        ///// </summary>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //public static bool IsNullable(this Type type)
        //{
        //    return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        //}

        /// <summary>
        /// 判断是否是String类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsString(this Type type)
        {
            return type == TypeConst.String;
        }

        /// <summary>
        /// 判断是否是Byte类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsByte(this Type type)
        {
            return type == TypeConst.Byte;
        }

        /// <summary>
        /// 判断是否是Char类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsChar(this Type type)
        {
            return type == TypeConst.Char;
        }

        /// <summary>
        /// 判断是否是Short类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsShort(this Type type)
        {
            return type == TypeConst.Short;
        }

        /// <summary>
        /// 判断是否是Int类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsInt(this Type type)
        {
            return type == TypeConst.Int;
        }

        /// <summary>
        /// 判断是否是Long类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsLong(this Type type)
        {
            return type == TypeConst.Long;
        }

        /// <summary>
        /// 判断是否是Float类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsFloat(this Type type)
        {
            return type == TypeConst.Float;
        }

        /// <summary>
        /// 判断是否是Double类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDouble(this Type type)
        {
            return type == TypeConst.Double;
        }

        /// <summary>
        /// 判断是否是Decimal类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDecimal(this Type type)
        {
            return type == TypeConst.Decimal;
        }

        /// <summary>
        /// 判断是否是DateTime类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDateTime(this Type type)
        {
            return type == TypeConst.DateTime;
        }

        /// <summary>
        /// 判断是否是Guid类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsGuid(this Type type)
        {
            return type == TypeConst.Guid;
        }

        /// <summary>
        /// 判断是否是Bool类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsBool(this Type type)
        {
            return type == TypeConst.Bool;
        }
    }

    /// <summary>
    /// 类型常量
    /// </summary>
    public class TypeConst
    {
        /// <summary>
        /// String
        /// </summary>
        public static readonly Type String = typeof(string);

        /// <summary>
        /// Byte
        /// </summary>
        public static readonly Type Byte = typeof(byte);

        /// <summary>
        /// Char
        /// </summary>
        public static readonly Type Char = typeof(char);

        /// <summary>
        /// Short
        /// </summary>
        public static readonly Type Short = typeof(short);

        /// <summary>
        /// Int
        /// </summary>
        public static readonly Type Int = typeof(int);

        /// <summary>
        /// Long
        /// </summary>
        public static readonly Type Long = typeof(long);

        /// <summary>
        /// Float
        /// </summary>
        public static readonly Type Float = typeof(float);

        /// <summary>
        /// Double
        /// </summary>
        public static readonly Type Double = typeof(double);

        /// <summary>
        /// Decimal
        /// </summary>
        public static readonly Type Decimal = typeof(decimal);

        /// <summary>
        /// DateTime
        /// </summary>
        public static readonly Type DateTime = typeof(DateTime);

        /// <summary>
        /// Guid
        /// </summary>
        public static readonly Type Guid = typeof(Guid);

        /// <summary>
        /// Bool
        /// </summary>
        public static readonly Type Bool = typeof(bool);

    }
}
#endif