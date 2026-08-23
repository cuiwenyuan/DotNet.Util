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
    }
}
