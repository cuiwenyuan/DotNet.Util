using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// DataUtil（NewLife 类型转换包装）测试
    /// </summary>
    public class DataUtilTests
    {
        [Theory]
        [InlineData(null, 0)]
        [InlineData("", 0)]
        [InlineData("123", 123)]
        [InlineData("abc", 0)]
        [InlineData("-5", -5)]
        public void ToInt_ReturnsExpected(object? value, int expected)
        {
            Assert.Equal(expected, value.ToInt());
        }

        [Fact]
        public void ToInt_WithDefault_ReturnsDefaultOnInvalid()
        {
            Assert.Equal(-1, "abc".ToInt(-1));
        }

        [Fact]
        public void ToLong_Works()
        {
            Assert.Equal(9223372036854775807L, "9223372036854775807".ToLong());
            Assert.Equal(0L, "abc".ToLong());
        }

        [Fact]
        public void ToDouble_Works()
        {
            Assert.Equal(3.14, "3.14".ToDouble(), 2);
            Assert.Equal(0.0, "abc".ToDouble());
        }

        [Fact]
        public void ToDecimal_Works()
        {
            Assert.Equal(123.45m, "123.45".ToDecimal());
        }

        [Fact]
        public void ToBoolean_Works()
        {
            Assert.True("true".ToBoolean());
            Assert.True("True".ToBoolean());
            Assert.False("0".ToBoolean());
            Assert.False("abc".ToBoolean());
        }

        [Fact]
        public void ToDateTime_Parses()
        {
            Assert.Equal(new DateTime(2026, 8, 23, 10, 0, 0), "2026-08-23 10:00:00".ToDateTime());
        }

        [Fact]
        public void ToDateTime_Invalid_ReturnsMinValue()
        {
            Assert.Equal(DateTime.MinValue, "not-a-date".ToDateTime());
        }

        [Fact]
        public void ToFullString_Formats()
        {
            var value = new DateTime(2026, 8, 23, 10, 0, 0);
            Assert.Equal("2026-08-23 10:00:00", value.ToFullString());
        }
    }
}
