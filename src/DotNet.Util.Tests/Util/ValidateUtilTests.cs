using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// ValidateUtil 校验测试
    /// </summary>
    public class ValidateUtilTests
    {
        [Theory]
        [InlineData("123", true)]
        [InlineData("-123", false)] // IsInt 仅接受非负整数
        [InlineData("12.5", false)]
        [InlineData("abc", false)]
        [InlineData("", false)]
        public void IsInt_ReturnsExpected(string input, bool expected)
        {
            Assert.Equal(expected, ValidateUtil.IsInt(input));
        }

        [Theory]
        [InlineData("2026-08-23", true)]
        [InlineData("2026-08-23 10:00:00", true)]
        [InlineData("2026-13-45", false)]
        [InlineData("abc", false)]
        public void IsDateTime_ReturnsExpected(string input, bool expected)
        {
            Assert.Equal(expected, ValidateUtil.IsDateTime(input));
        }

        [Fact]
        public void IsEmail_Works()
        {
            Assert.True(ValidateUtil.IsEmail("troy.cui@qq.com"));
            Assert.True(ValidateUtil.IsEmail("user@example.com"));
            Assert.False(ValidateUtil.IsEmail("abc"));
            Assert.False(ValidateUtil.IsEmail("user@.com"));
        }

        [Fact]
        public void IsMobile_Works()
        {
            Assert.True(ValidateUtil.IsMobile("13800138000"));
            Assert.False(ValidateUtil.IsMobile("12345"));
            Assert.False(ValidateUtil.IsMobile("23800138000"));
        }

        [Fact]
        public void IsIdCard_Works()
        {
            // 11010519491231002X 为公开的合法校验码示例
            Assert.True(ValidateUtil.IsIdCard("11010519491231002X"));
            Assert.True(ValidateUtil.IsIdCard("110105491231002"));
            Assert.False(ValidateUtil.IsIdCard("123"));
            Assert.False(ValidateUtil.IsIdCard("123456789012345678"));
        }

        [Fact]
        public void IsNumeric_Works()
        {
            Assert.True(ValidateUtil.IsNumeric("12.34"));
            Assert.True(ValidateUtil.IsNumeric("100"));
            Assert.False(ValidateUtil.IsNumeric("abc"));
        }

        [Fact]
        public void IsChineseCharacters_Works()
        {
            Assert.True(ValidateUtil.IsChineseCharacters("崔文远"));
            Assert.False(ValidateUtil.IsChineseCharacters("abc"));
        }

        [Fact]
        public void IsBoolean_Works()
        {
            // 实现用 bool.ToString() 比较，仅接受 "True"/"False"（大小写敏感）
            Assert.True(ValidateUtil.IsBoolean("True"));
            Assert.True(ValidateUtil.IsBoolean("False"));
            Assert.False(ValidateUtil.IsBoolean("true"));
            Assert.False(ValidateUtil.IsBoolean("yes"));
        }

        [Fact]
        public void IsUserName_Works()
        {
            Assert.True(ValidateUtil.IsUserName("troy_cui"));
            Assert.False(ValidateUtil.IsUserName("ab")); // 小于 3 位
            Assert.False(ValidateUtil.IsUserName("名字带中文"));
        }

        [Theory]
        [InlineData("192.168.1.1", true)]
        [InlineData("0.0.0.0", true)] // 修复：原正则误判为 false
        [InlineData("0.1.2.3", true)] // 修复：首段 0 原正则不支持
        [InlineData("255.255.255.255", true)]
        [InlineData("127.0.0.1", true)]
        [InlineData("256.1.1.1", false)] // 超出 255
        [InlineData("1.2.3", false)] // 不足 4 段
        [InlineData("abc", false)]
        [InlineData("", false)]
        [InlineData("::1", false)] // IPv6 不是 IPv4（严格判定，不再回退 IsIpv6）
        [InlineData("2001:db8::ff00:42:8329", false)]
        public void IsIpv4_ReturnsExpected(string input, bool expected)
        {
            Assert.Equal(expected, ValidateUtil.IsIpv4(input));
        }

        [Fact]
        public void IsIpv4_Null_DoesNotThrow()
        {
            // 修复：原实现 match.IsMatch(null) 会抛 ArgumentNullException
            Assert.False(ValidateUtil.IsIpv4(null));
        }

        [Theory]
        [InlineData("2001:0db8:85a3:0000:0000:8a2e:0370:7334", true)] // 完整 8 段
        [InlineData("::1", true)] // 压缩写法（双冒号）
        [InlineData("abc", false)]
        [InlineData("", false)]
        public void IsIpv6_ReturnsExpected(string input, bool expected)
        {
            Assert.Equal(expected, ValidateUtil.IsIpv6(input));
        }

        [Fact]
        public void IsIpv6_Null_DoesNotThrow()
        {
            // 修复：原实现 ipAddress.Split 会抛 NullReferenceException
            Assert.False(ValidateUtil.IsIpv6(null));
        }
    }
}
