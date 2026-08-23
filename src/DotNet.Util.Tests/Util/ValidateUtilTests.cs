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
    }
}
