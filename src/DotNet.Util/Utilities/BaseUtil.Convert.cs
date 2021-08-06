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
    /// 这个类可是修改了很多次啊，已经比较经典了，随着专业的提升，人也会不断提高，技术也会越来越精湛。
    /// 
    /// 修改记录
    /// 
    ///		2012-05-07 版本：3.71   Serwif 改进ObjectsToList(CurrentDbType DbType ,object ids) 字段值数组转换为字符串列表时，增加末尾去掉逗号功能CutLastDot(string input)
    ///		2009.09.08 版本：4.4	JiRiGaLa 改进 GetPermissionScope(string[] organizeIds)。
    ///		2008.08.29 版本：4.3	JiRiGaLa 改进 DataTableToString 的 null值处理技术。
    ///		2007.11.08 版本：4.2	JiRiGaLa 改进 DataTableToStringList 为 FieldToList。
    ///		2007.11.05 版本：4.1	JiRiGaLa 改进 GetDS、GetDataTable 功能，整体思路又上一个台阶，基类的又一次飞跃。
    ///		2007.11.05 版本：4.0	JiRiGaLa 改进 支持不是“Id”为字段的主键表。
    ///		2007.11.01 版本：3.9	JiRiGaLa 改进 BUOperatorInfo 去掉这个变量，可以提高性能，提高速度，基类的又一次飞跃。
    ///		2007.09.13 版本：3.8	JiRiGaLa 改进 BUBaseUtil.SQLLogicConditional 错误。
    ///		2007.08.14 版本：3.7	JiRiGaLa 改进 WebService 模式下 DataSet 传输数据的速度问题。
    ///		2007.07.20 版本：3.6	JiRiGaLa 改进 DataSet 修改为 DataTable 应该能提高一些速度吧。
    ///		2007.05.20 版本：3.6	JiRiGaLa 改进 GetList() 方法整理，这次又应该是一次升华，质的飞跃很不容易啊，身心都有提高了。
    ///		2007.05.20 版本：3.4	JiRiGaLa 改进 Exists() 方法整理。
    ///		2007.05.13 版本：3.3	JiRiGaLa 改进 GetProperty()，SetProperty()，Delete() 方法整理。
    ///		2007.05.10 版本：3.2	JiRiGaLa 改进 GetList() 方法整理。
    ///		2007.04.10 版本：3.1	JiRiGaLa 添加 GetNextId，GetPreviousId 方法整理。
    ///		2007.03.03 版本：3.0	JiRiGaLa 进行了一次彻底的优化工作，方法的位置及功能整理。
    ///		2007.03.01 版本：2.0	JiRiGaLa 重新调整主键的规范化。
    ///		2006.02.05 版本：1.1	JiRiGaLa 重新调整主键的规范化。
    ///		2005.12.30 版本：1.0	JiRiGaLa 数据库连接方式都进行改进
    ///		2005.09.04 版本：1.0	JiRiGaLa 执行数据库脚本
    ///		2005.08.19 版本：1.0	整理一下编排	
    ///		2005.07.10 版本：1.0	修改了程序，格式以及理念都有些提高，应该是一次大突破
    ///		2004.11.12 版本：1.0	添加了最新的GetParent、GetChildren、GetParentChildren 方法
    ///		2004.07.21 版本：1.0	UpdateRecord、Delete、SetProperty、GetProperty、ExecuteNonQuery、GetRecord 方法进行改进。
    ///								还删除一些重复的主键，提取了最优化的方法，有时候写的主键真的是垃圾，可能自己也没有注意时就写出了垃圾。
    ///								GetRepeat、GetDayOfWeek、ExecuteProcedure、GetFromProcedure 方法进行改进，基本上把所有的方法都重新写了一遍。
    ///	
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2009.09.08</date>
    /// </author> 
    /// </summary>
    public partial class BaseUtil
    {
        /// <summary>
        /// 整型转为布尔型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static Boolean ConvertIntToBoolean(Object targetValue)
        {
            return targetValue != DBNull.Value && (targetValue.ToString().Equals("1") || targetValue.ToString().Equals(true.ToString()));
        }
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
        public static int ConvertToInt(Object targetValue, int defaultValue  = 0)
        {
            if (targetValue == DBNull.Value)
            {
                return defaultValue;
            }

            var result = 0;

            var resultValue = 0;
            int.TryParse(targetValue.ToString(), out resultValue);
            result = resultValue;

            return result;
        }
        /// <summary>
        /// 转为可为NULL的整型
        /// </summary>
        /// <param name="targetValue"></param>
        /// <returns></returns>
        public static int? ConvertToNullableInt(Object targetValue)
        {
            int? returnValue = null;
            if (targetValue == DBNull.Value)
            {
                return null;
            }

            var result = 0;
            int.TryParse(targetValue.ToString(), out result);
            returnValue = result;

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
            if (targetValue == DBNull.Value)
            {
                return returnValue;
            }

            Byte result = 0;
            Byte.TryParse(targetValue.ToString(), out result);
            returnValue = result;

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
            if (targetValue == DBNull.Value)
            {
                return null;
            }

            Byte result = 0;
            Byte.TryParse(targetValue.ToString(), out result);
            returnValue = result;

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
            if (targetValue == DBNull.Value)
            {
                return returnValue;
            }

            var result = 0;
            Int32.TryParse(targetValue.ToString(), out result);
            returnValue = result;

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
            if (targetValue == DBNull.Value)
            {
                return null;
            }

            var result = 0;
            Int32.TryParse(targetValue.ToString(), out result);
            returnValue = result;

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
            if (targetValue == DBNull.Value)
            {
                return returnValue;
            }

            Int64 result = 0;
            Int64.TryParse(targetValue.ToString(), out result);
            returnValue = result;

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
            if (targetValue == DBNull.Value)
            {
                return null;
            }

            Int64 result = 0;
            Int64.TryParse(targetValue.ToString(), out result);
            returnValue = result;

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
            if (targetValue == DBNull.Value)
            {
                return returnValue;
            }

            long result = 0;
            long.TryParse(targetValue.ToString(), out result);
            returnValue = result;

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
            if (targetValue == DBNull.Value)
            {
                return null;
            }

            long result = 0;
            long.TryParse(targetValue.ToString(), out result);
            returnValue = result;

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
            if (targetValue == DBNull.Value)
            {
                return returnValue;
            }

            Double result = 0;
            Double.TryParse(targetValue.ToString(), out result);
            returnValue = result;

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
            if (targetValue == DBNull.Value)
            {
                return null;
            }

            Double result = 0;
            Double.TryParse(targetValue.ToString(), out result);
            returnValue = result;

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
            if (targetValue == DBNull.Value)
            {
                return returnValue;
            }

            float result = 0;
            float.TryParse(targetValue.ToString(), out result);
            returnValue = result;

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
            if (targetValue == DBNull.Value)
            {
                return null;
            }

            float result = 0;
            float.TryParse(targetValue.ToString(), out result);
            returnValue = result;

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
            if (targetValue == DBNull.Value)
            {
                return returnValue;
            }

            decimal result = 0;
            decimal.TryParse(targetValue.ToString(), out result);
            returnValue = result;

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

            if (targetValue == DBNull.Value)
            {
                return null;
            }

            decimal result = 0;
            decimal.TryParse(targetValue.ToString(), out result);
            returnValue = result;

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
                // returnValue = Convert.ToDateTime(targetValue.ToString()).ToUniversalTime();
                returnValue = Convert.ToDateTime(targetValue.ToString());
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
    }
}