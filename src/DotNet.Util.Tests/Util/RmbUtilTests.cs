using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// RmbUtil 人民币大写测试
    /// </summary>
    public class RmbUtilTests
    {
        [Theory]
        [InlineData(0d, "零元整")]
        [InlineData(10d, "壹拾元整")]
        [InlineData(123.45d, "壹佰贰拾叁元肆角伍分")]
        [InlineData(200.55d, "贰佰元零伍角伍分")]
        [InlineData(1008.5d, "壹仟零捌元伍角整")]
        public void Capital_Decimal_ReturnsExpected(double input, string expected)
        {
            Assert.Equal(expected, RmbUtil.Capital((decimal)input));
        }

        [Fact]
        public void Capital_String_Invalid_ReturnsErrorMessage()
        {
            Assert.Equal("非数字形式！", RmbUtil.Capital("abc"));
        }

        [Fact]
        public void Capital_Overflow_ReturnsOverflow()
        {
            // num*100 超过 15 位
            Assert.Equal("溢出", RmbUtil.Capital(12345678901234m));
        }

        [Fact]
        public void Capital_String_Valid()
        {
            Assert.Equal("壹佰贰拾叁元肆角伍分", RmbUtil.Capital("123.45"));
        }
    }
}
