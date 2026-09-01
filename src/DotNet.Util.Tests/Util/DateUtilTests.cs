using System;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// DateUtil 日期工具测试
    /// </summary>
    public class DateUtilTests
    {
        [Theory]
        [InlineData(2024, 2, 29)] // 闰年
        [InlineData(2026, 2, 28)]
        [InlineData(2026, 4, 30)]
        [InlineData(2026, 12, 31)]
        public void GetDaysOfMonth_ReturnsExpected(int year, int month, int expected)
        {
            Assert.Equal(expected, DateUtil.GetDaysOfMonth(year, month));
        }

        [Theory]
        [InlineData(2024, 366)]
        [InlineData(2026, 365)]
        public void GetDaysOfYear_ReturnsExpected(int year, int expected)
        {
            Assert.Equal(expected, DateUtil.GetDaysOfYear(year));
        }

        [Fact]
        public void GetDaysOfMonth_InvalidMonth_Throws()
        {
            // 修复：原实现 switch 无 default，非法月份（0/13/负数）静默返回 0
            Assert.Throws<ArgumentOutOfRangeException>(() => DateUtil.GetDaysOfMonth(2026, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => DateUtil.GetDaysOfMonth(2026, 13));
            Assert.Throws<ArgumentOutOfRangeException>(() => DateUtil.GetDaysOfMonth(2026, -1));
        }

        [Fact]
        public void DiffDays_Positive()
        {
            Assert.Equal(9, DateUtil.DiffDays(new DateTime(2026, 1, 1), new DateTime(2026, 1, 10)));
        }

        [Fact]
        public void DiffDays_Negative()
        {
            Assert.Equal(-9, DateUtil.DiffDays(new DateTime(2026, 1, 10), new DateTime(2026, 1, 1)));
        }

        [Fact]
        public void ToDate_ValidString()
        {
            Assert.Equal(new DateTime(2026, 8, 23), DateUtil.ToDate("2026-08-23"));
        }

        [Fact]
        public void GetDayOfWeek_ReturnsNumber()
        {
            // 2026-08-23 是星期日，数字星期为 0 或 7（视实现）
            var day = DateUtil.GetDayOfWeek("星期日");
            Assert.True(int.TryParse(day, out _));
        }

        [Fact]
        public void GetWeekNameOfDay_ReturnsNonEmpty()
        {
            Assert.False(string.IsNullOrEmpty(DateUtil.GetWeekNameOfDay(new DateTime(2026, 8, 23))));
        }

        #region GetStartTimeOfDay / GetEndTimeOfDay(新增：归零 / 补满)

        // 2026-02-15 是星期日
        private static readonly DateTime Probe = new DateTime(2026, 2, 15, 10, 30, 45);

        [Fact]
        public void GetStartTimeOfDay_ReturnsMidnight()
        {
            // 旧方法沿用入参时刻 10:30:45；新方法应归零到 00:00:00
            Assert.Equal(new DateTime(2026, 2, 1, 0, 0, 0), DateUtil.GetStartTimeOfDay(Probe, "Month"));
        }

        [Fact]
        public void GetEndTimeOfDay_ReturnsLastSecondOfDay()
        {
            // 旧方法沿用入参时刻 10:30:45；新方法应补满到 23:59:59
            Assert.Equal(new DateTime(2026, 2, 28, 23, 59, 59), DateUtil.GetEndTimeOfDay(Probe, "Month"));
        }

        [Theory]
        [InlineData("Week", "2026-02-09", "2026-02-15")] // 周日属于本周，本周为 周一~周日
        [InlineData("Month", "2026-02-01", "2026-02-28")]
        [InlineData("Season", "2026-01-01", "2026-03-31")]
        [InlineData("Year", "2026-01-01", "2026-12-31")]
        public void StartEndOfDay_CoverWholePeriod(string timeType, string expectedStart, string expectedEnd)
        {
            var start = DateUtil.GetStartTimeOfDay(Probe, timeType);
            var end = DateUtil.GetEndTimeOfDay(Probe, timeType);

            Assert.Equal(DateTime.Parse(expectedStart), start);
            Assert.Equal(DateTime.Parse(expectedEnd).AddDays(1).AddSeconds(-1), end);
            Assert.Equal(TimeSpan.Zero, start.TimeOfDay);          // 起始必须归零
            Assert.Equal(new TimeSpan(23, 59, 59), end.TimeOfDay); // 结束必须补满
            Assert.True(end > start);
        }

        [Fact]
        public void GetEndTimeOfDay_LeapYear_FebruaryHas29Days()
        {
            var leap = new DateTime(2024, 2, 15, 8, 0, 0);
            Assert.Equal(new DateTime(2024, 2, 29, 23, 59, 59), DateUtil.GetEndTimeOfDay(leap, "Month"));
        }

        [Fact]
        public void LegacyMethods_KeepTimeOfDay_ForBackwardCompatibility()
        {
            // 固化旧方法行为：不归零、不补满，沿用入参时刻（已标 [Obsolete]，但行为不得改变）
#pragma warning disable CS0618
            Assert.Equal(new DateTime(2026, 2, 1, 10, 30, 45), DateUtil.GetStartTime(Probe, "Month"));
            Assert.Equal(new DateTime(2026, 2, 28, 10, 30, 45), DateUtil.GetEndTime(Probe, "Month"));
#pragma warning restore CS0618
        }

        #region WeekRange 周区间

        [Theory]
        [InlineData(2023)] // 1/1 是周日（原实现会漏掉 1/1）
        [InlineData(2017)] // 1/1 是周日
        [InlineData(2024)] // 1/1 是周一
        [InlineData(2026)] // 1/1 是周四
        [InlineData(2022)] // 1/1 是周六
        [InlineData(2021)] // 1/1 是周五
        [InlineData(2025)] // 1/1 是周三
        public void WeekRange_FirstWeekCoversJan1(int year)
        {
            var first = default(DateTime);
            var last = default(DateTime);
            DateUtil.WeekRange(year, 1, ref first, ref last);

            var jan1 = new DateTime(year, 1, 1);

            // 第 1 周必须覆盖 1/1：原实现 dayDiff=(-1)*firstOfWeek+1 在 1/1 为周日时算得 +1，
            // 导致第 1 周从 1/2 开始，1/1 不属于任何一周。
            Assert.True(first <= jan1 && jan1 <= last,
                $"{year}-01-01 未被第 1 周覆盖：{first:yyyy-MM-dd} ~ {last:yyyy-MM-dd}");
            // 一周必须正好 7 天
            Assert.Equal(6, (last - first).Days);
        }

        [Fact]
        public void WeekRange_Jan1IsSunday_ReturnsPreviousMondayToJan1()
        {
            // 2023-01-01 是星期日：第 1 周应为 2022-12-26(周一) ~ 2023-01-01(周日)
            var first = default(DateTime);
            var last = default(DateTime);
            DateUtil.WeekRange(2023, 1, ref first, ref last);

            Assert.Equal(new DateTime(2022, 12, 26), first);
            Assert.Equal(new DateTime(2023, 1, 1), last);
        }

        [Fact]
        public void WeekRange_FirstWeek_ConsistentWithGetWeekOfYear()
        {
            // GetWeekOfYear 按「周一为一周首日」计算，1/1 恒返回 1；
            // WeekRange 的第 1 周区间必须能覆盖 1/1，否则两个 API 自相矛盾。
            var first = default(DateTime);
            var last = default(DateTime);
            DateUtil.WeekRange(2023, 1, ref first, ref last);

            var jan1 = new DateTime(2023, 1, 1);
            Assert.Equal(1, DateUtil.GetWeekOfYear(jan1));
            Assert.True(jan1 >= first && jan1 <= last);
        }

        [Fact]
        public void WeekRange_WeekOrder2_ShiftsBy7Days()
        {
            var first1 = default(DateTime);
            var last1 = default(DateTime);
            DateUtil.WeekRange(2026, 1, ref first1, ref last1);

            var first2 = default(DateTime);
            var last2 = default(DateTime);
            DateUtil.WeekRange(2026, 2, ref first2, ref last2);

            Assert.Equal(first1.AddDays(7), first2);
            Assert.Equal(last1.AddDays(7), last2);
        }

        #endregion

        #endregion
    }
}
