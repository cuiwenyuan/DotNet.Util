//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Globalization;

namespace DotNet.Util
{
    /// <summary>
    /// 日期工具
    /// </summary>
    public partial class DateUtil
    {
        #region public static string GetDayOfWeek(string dayOfWeek) 获得数字星期几
        /// <summary>
        /// 获得数字星期几
        /// </summary>
        /// <param name="dayOfWeek">英文星期</param>
        /// <returns>数字星期几</returns>
        public static string GetDayOfWeek(string dayOfWeek)
        {
            return GetDayOfWeek(dayOfWeek, false);
        }
        #endregion

        #region public static int GetDayOfWeek(string dayOfWeek, bool chinese) 获得星期几
        /// <summary>
        /// 获得星期几
        /// </summary>
        /// <param name="dayOfWeek">英文星期</param>
        /// <param name="chinese">需要中文</param>
        /// <returns>数字星期几</returns>
        public static string GetDayOfWeek(string dayOfWeek, bool chinese)
        {
            var week = "0";
            switch (dayOfWeek)
            {
                case "Sunday":
                    week = "0";
                    break;
                case "Monday":
                    week = "1";
                    break;
                case "Tuesday":
                    week = "2";
                    break;
                case "Wednesday":
                    week = "3";
                    break;
                case "Thursday":
                    week = "4";
                    break;
                case "Friday":
                    week = "5";
                    break;
                case "Saturday":
                    week = "6";
                    break;
                default:
                    break;
            }
            if (chinese)
            {
                switch (week)
                {
                    case "0":
                        week = "星期日";
                        break;
                    case "1":
                        week = "星期一";
                        break;
                    case "2":
                        week = "星期二";
                        break;
                    case "3":
                        week = "星期三";
                        break;
                    case "4":
                        week = "星期四";
                        break;
                    case "5":
                        week = "星期五";
                        break;
                    case "6":
                        week = "星期六";
                        break;
                    default:
                        break;
                }
            }
            return week;
        }
        #endregion


        /*
        //1.1 取当前年月日时分秒 
                currentTime=System.DateTime.Now;
        //1.2 取当前年 
                int 年=currentTime.Year; 
        //1.3 取当前月 
                int 月=currentTime.Month; 
        //1.4 取当前日 
                int 日=currentTime.Day; 
        //1.5 取当前时 
                int 时=currentTime.Hour; 
        //1.6 取当前分 
                int 分=currentTime.Minute; 
        //1.7 取当前秒 
                int 秒=currentTime.Second; 
        //1.8 取当前毫秒 
                int 毫秒=currentTime.Millisecond; 
        //（变量可用中文） 

        //1.9 取中文日期显示——年月日时分 
                         string strY=currentTime.ToString("f"); //不显示秒 

        //1.10 取中文日期显示_年月 
                 string strYM=currentTime.ToString("y"); 

        //1.11 取中文日期显示_月日 
                 string strMD=currentTime.ToString("m"); 

        //1.12 取中文年月日 
                 string strYMD=currentTime.ToString("D"); 

        /1.13 取当前时分，格式为：14：24 
        string strT=currentTime.ToString("t"); 

        //1.14 取当前时间，格式为：2003-09-23T14:46:48 
        string strT=currentTime.ToString("s"); 

        //1.15 取当前时间，格式为：2003-09-23 14:48:30Z 
                                              string strT=currentTime.ToString("u"); 

        //1.16 取当前时间，格式为：2003-09-23 14:48 
        string strT=currentTime.ToString("g"); 

        //1.17 取当前时间，格式为：Tue, 23 Sep 2003 14:52:40 GMT 
                                                     string strT=currentTime.ToString("r"); 

        //1.18获得当前时间 n 天后的日期时间 
            DateTime newDay = DateTime.Now.AddDays(100); 
        */

        #region 返回本年有多少天

        /// <summary>返回本年有多少天</summary>
        /// <param name="iYear">年份</param>
        /// <returns>本年的天数</returns>
        public static int GetDaysOfYear(int iYear)
        {
            return IsRuYear(iYear) ? 366 : 365;
        }

        /// <summary>本年有多少天</summary>
        /// <param name="dt">日期</param>
        /// <returns>本天在当年的天数</returns>
        public static int GetDaysOfYear(DateTime dt)
        {
            return IsRuYear(dt.Year) ? 366 : 365;
        }

        #endregion

        #region 返回本月有多少天
        /// <summary>本月有多少天</summary>
        /// <param name="iYear">年</param>
        /// <param name="month">月</param>
        /// <returns>天数</returns>
        public static int GetDaysOfMonth(int iYear, int month)
        {
            var days = 0;
            switch (month)
            {
                case 1:
                    days = 31;
                    break;
                case 2:
                    days = IsRuYear(iYear) ? 29 : 28;
                    break;
                case 3:
                    days = 31;
                    break;
                case 4:
                    days = 30;
                    break;
                case 5:
                    days = 31;
                    break;
                case 6:
                    days = 30;
                    break;
                case 7:
                    days = 31;
                    break;
                case 8:
                    days = 31;
                    break;
                case 9:
                    days = 30;
                    break;
                case 10:
                    days = 31;
                    break;
                case 11:
                    days = 30;
                    break;
                case 12:
                    days = 31;
                    break;
            }

            return days;
        }


        /// <summary>本月有多少天</summary>
        /// <param name="dt">日期</param>
        /// <returns>天数</returns>
        public static int GetDaysOfMonth(DateTime dt)
        {
            //--------------------------------//
            //--从dt中取得当前的年，月信息  --//
            //--------------------------------//
            var days = 0;
            var year = dt.Year;
            var month = dt.Month;

            //--利用年月信息，得到当前月的天数信息。
            switch (month)
            {
                case 1:
                    days = 31;
                    break;
                case 2:
                    days = IsRuYear(year) ? 29 : 28;
                    break;
                case 3:
                    days = 31;
                    break;
                case 4:
                    days = 30;
                    break;
                case 5:
                    days = 31;
                    break;
                case 6:
                    days = 30;
                    break;
                case 7:
                    days = 31;
                    break;
                case 8:
                    days = 31;
                    break;
                case 9:
                    days = 30;
                    break;
                case 10:
                    days = 31;
                    break;
                case 11:
                    days = 30;
                    break;
                case 12:
                    days = 31;
                    break;
            }
            return days;
        }
        #endregion

        #region 返回当前日期的 （星期名称or星期编号）
        /// <summary>返回当前日期的星期名称</summary>
        /// <param name="dt">日期</param>
        /// <returns>星期名称</returns>
        public static string GetWeekNameOfDay(DateTime dt)
        {
            var week = string.Empty;
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    week = "星期一";
                    break;
                case DayOfWeek.Tuesday:
                    week = "星期二";
                    break;
                case DayOfWeek.Wednesday:
                    week = "星期三";
                    break;
                case DayOfWeek.Thursday:
                    week = "星期四";
                    break;
                case DayOfWeek.Friday:
                    week = "星期五";
                    break;
                case DayOfWeek.Saturday:
                    week = "星期六";
                    break;
                case DayOfWeek.Sunday:
                    week = "星期日";
                    break;
            }
            return week;
        }


        /// <summary>返回当前日期的星期编号</summary>
        /// <param name="dt">日期</param>
        /// <returns>星期数字编号</returns>
        public static int GetWeekNumberOfDay(DateTime dt)
        {
            var week = 0;
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    week = 1;
                    break;
                case DayOfWeek.Tuesday:
                    week = 2;
                    break;
                case DayOfWeek.Wednesday:
                    week = 3;
                    break;
                case DayOfWeek.Thursday:
                    week = 4;
                    break;
                case DayOfWeek.Friday:
                    week = 5;
                    break;
                case DayOfWeek.Saturday:
                    week = 6;
                    break;
                case DayOfWeek.Sunday:
                    week = 7;
                    break;
            }
            return week;
        }
        #endregion

        #region 获取某一年有多少周
        /// <summary>
        /// 获取某一年有多少周
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>该年周数</returns>
        public static int GetWeekAmount(int year)
        {
            var end = new DateTime(year, 12, 31); //该年最后一天
            var gc = new GregorianCalendar();
            return gc.GetWeekOfYear(end, CalendarWeekRule.FirstDay, DayOfWeek.Monday); //该年星期数
        }
        #endregion

        #region 获取某一日期是该年中的第几周
        /// <summary>
        /// 获取某一日期是该年中的第几周
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>该日期在该年中的周数</returns>
        public static int GetWeekOfYear(DateTime dt)
        {
            var gc = new GregorianCalendar();
            return gc.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }
        #endregion

        #region 根据某年的第几周获取这周的起止日期
        /// <summary>
        /// 根据某年的第几周获取这周的起止日期
        /// </summary>
        /// <param name="year"></param>
        /// <param name="weekOrder"></param>
        /// <param name="firstDate"></param>
        /// <param name="lastDate"></param>
        /// <returns></returns>
        public static void WeekRange(int year, int weekOrder, ref DateTime firstDate, ref DateTime lastDate)
        {
            //当年的第一天
            var firstDay = new DateTime(year, 1, 1);

            //当年的第一天是星期几
            var firstOfWeek = Convert.ToInt32(firstDay.DayOfWeek);

            //计算当年第一周的起止日期，可能跨年
            var dayDiff = (-1) * firstOfWeek + 1;
            var dayAdd = 7 - firstOfWeek;

            firstDate = firstDay.AddDays(dayDiff).Date;
            lastDate = firstDay.AddDays(dayAdd).Date;

            //如果不是要求计算第一周
            if (weekOrder != 1)
            {
                var addDays = (weekOrder - 1) * 7;
                firstDate = firstDate.AddDays(addDays);
                lastDate = lastDate.AddDays(addDays);
            }
        }
        #endregion

        #region 返回两个日期之间相差的天数
        /// <summary>
        /// 返回两个日期之间相差的天数
        /// </summary>
        /// <param name="dtfrm">Start参数</param>
        /// <param name="dtto">两个日期参数</param>
        /// <returns>天数</returns>
        public static int DiffDays(DateTime dtfrm, DateTime dtto)
        {
            var tsDiffer = dtto.Date - dtfrm.Date;
            return tsDiffer.Days;
        }
        #endregion

        #region 判断当前年份是否是闰年
        /// <summary>判断当前年份是否是闰年，私有函数</summary>
        /// <param name="iYear">年份</param>
        /// <returns>是闰年：True ，不是闰年：False</returns>
        private static bool IsRuYear(int iYear)
        {
            //形式参数为年份
            //例如：2003
            var n = iYear;
            return (n % 400 == 0) || (n % 4 == 0 && n % 100 != 0);
        }
        #endregion

        #region 将输入的字符串转化为日期。如果字符串的格式非法，则返回当前日期
        /// <summary>
        /// 将输入的字符串转化为日期。如果字符串的格式非法，则返回当前日期。
        /// </summary>
        /// <param name="strInput">输入字符串</param>
        /// <returns>日期对象</returns>
        public static DateTime ToDate(string strInput)
        {
            DateTime oDateTime;

            try
            {
                oDateTime = DateTime.Parse(strInput);
            }
            catch (Exception)
            {
                oDateTime = DateTime.Today;
            }

            return oDateTime;
        }
        #endregion

        #region 将日期对象转化为格式字符串
        /// <summary>
        /// 将日期对象转化为格式字符串
        /// </summary>
        /// <param name="oDateTime">日期对象</param>
        /// <param name="strFormat">
        /// 格式：
        ///		"SHORTDATE"===短日期
        ///		"LONGDATE"==长日期
        ///		其它====自定义格式
        /// </param>
        /// <returns>日期字符串</returns>
        public static string ToString(DateTime oDateTime, string strFormat)
        {
            string strDate;

            try
            {
                switch (strFormat.ToUpper())
                {
                    case "SHORTDATE":
                        strDate = oDateTime.ToShortDateString();
                        break;
                    case "LONGDATE":
                        strDate = oDateTime.ToLongDateString();
                        break;
                    default:
                        strDate = oDateTime.ToString(strFormat);
                        break;
                }
            }
            catch (Exception)
            {
                strDate = oDateTime.ToShortDateString();
            }

            return strDate;
        }
        #endregion

        #region 判断是否为合法日期
        /// <summary>
        /// 判断是否为合法日期，必须大于1800年1月1日
        /// </summary>
        /// <param name="dateTime">输入日期字符串</param>
        /// <returns>True/False</returns>
        public static bool IsDateTime(string dateTime)
        {
            return ValidateUtil.IsDateTime(dateTime);
        }
        #endregion

        #region 某日期是当月的第几周
        /// <summary>
        /// 某日期是当月的第几周
        /// </summary>
        /// <param name="day"></param>
        /// <param name="WeekStart"></param>
        /// <returns></returns>
        public static int WeekOfMonth(DateTime day, int WeekStart = 1)
        {
            //WeekStart
            //1表示 周一至周日 为一周
            //2表示 周日至周六 为一周
            DateTime FirstofMonth;
            FirstofMonth = Convert.ToDateTime(day.Date.Year + "-" + day.Date.Month + "-" + 1);

            int i = (int)FirstofMonth.Date.DayOfWeek;
            if (i == 0)
            {
                i = 7;
            }

            if (WeekStart == 1)
            {
                return (day.Date.Day + i - 2) / 7 + 1;
            }
            if (WeekStart == 2)
            {
                return (day.Date.Day + i - 1) / 7;

            }
            return 0;
            //错误返回值0
        }
        #endregion

        #region 获取 本周、本月、本季度、本年 的开始时间或结束时间
        /// <summary>
        /// 获取结束时间
        /// </summary>
        /// <param name="TimeType">Week、Month、Season、Year</param>
        /// <param name="now"></param>
        /// <returns></returns>
        public static DateTime GetStartTime(DateTime now, string TimeType)
        {
            switch (TimeType)
            {
                case "Week":
                    return now.AddDays(-(int)now.DayOfWeek + 1);
                case "Month":
                    return now.AddDays(-now.Day + 1);
                case "Season":
                    var time = now.AddMonths(0 - ((now.Month - 1) % 3));
                    return time.AddDays(-time.Day + 1);
                case "Year":
                    return now.AddDays(-now.DayOfYear + 1);
                default:
                    return now.AddDays(-(int)now.DayOfWeek + 1);
            }
        }

        /// <summary>
        /// 获取结束时间
        /// </summary>
        /// <param name="TimeType">Week、Month、Season、Year</param>
        /// <param name="now"></param>
        /// <returns></returns>
        public static DateTime GetEndTime(DateTime now, string TimeType)
        {
            switch (TimeType)
            {
                case "Week":
                    return now.AddDays(7 - (int)now.DayOfWeek);
                case "Month":
                    return now.AddMonths(1).AddDays(-now.AddMonths(1).Day + 1).AddDays(-1);
                case "Season":
                    var time = now.AddMonths((3 - ((now.Month - 1) % 3) - 1));
                    return time.AddMonths(1).AddDays(-time.AddMonths(1).Day + 1).AddDays(-1);
                case "Year":
                    var time2 = now.AddYears(1);
                    return time2.AddDays(-time2.DayOfYear);
                default:
                    return now.AddDays(7 - (int)now.DayOfWeek);
            }
        }
        #endregion

        #region GetTimeStamp
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="dt">不要用UTC时间，直接用本地时间</param>
        /// <param name="precision">精度(秒：s,毫秒：ms)</param>
        /// <returns></returns>
        public static long GetTimeStamp(string dt, string precision = "s")
        {
            long currentTicks = DateTime.Now.ToUniversalTime().Ticks;
            if (ValidateUtil.IsDateTime(dt))
            {
                currentTicks = DateTime.Parse(dt).ToUniversalTime().Ticks;
            }
            if (precision.Equals("ms", StringComparison.OrdinalIgnoreCase))
            {
                //毫秒
                return (currentTicks - 621355968000000000) / 10000;
            }
            else if (precision.Equals("s", StringComparison.OrdinalIgnoreCase))
            {
                //秒
                return (currentTicks - 621355968000000000) / 10000000;
            }
            else
            {
                return (currentTicks - 621355968000000000) / 10000000;
            }
        }
        #endregion

        #region GetTimeStamp
        /// <summary>
        /// 获取时间戳(秒)
        /// </summary>
        /// <param name="precision">精度(秒：s,毫秒：ms)</param>
        /// <returns></returns>
        public static long GetTimeStamp(string precision = "s")
        {
            if (precision.Equals("ms", StringComparison.OrdinalIgnoreCase))
            {
                //毫秒
                return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
            }
            else if (precision.Equals("s", StringComparison.OrdinalIgnoreCase))
            {
                //秒
                return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            }
            else
            {
                return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            }
        }
        #endregion

        #region GetLocalTime根据时间戳获取本地时间
        /// <summary>
        /// 根据时间戳获取本地时间
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <param name="precision">精度(秒：s,毫秒：ms)</param>
        /// <returns></returns>
        public static DateTime GetLocalTime(long timestamp, string precision = "s")
        {
            DateTime converted = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            if (precision.Equals("ms", StringComparison.OrdinalIgnoreCase))
            {
                //毫秒
                DateTime newDateTime = converted.AddMilliseconds(timestamp);
                return newDateTime.ToLocalTime();
            }
            else if (precision.Equals("s", StringComparison.OrdinalIgnoreCase))
            {
                //秒
                DateTime newDateTime = converted.AddSeconds(timestamp);
                return newDateTime.ToLocalTime();
            }
            else
            {
                DateTime newDateTime = converted.AddSeconds(timestamp);
                return newDateTime.ToLocalTime();
            }

        }
        #endregion
    }
}