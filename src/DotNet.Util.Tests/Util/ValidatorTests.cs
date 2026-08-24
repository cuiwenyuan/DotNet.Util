using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// Validator 测试
    /// </summary>
    public class ValidatorTests
    {
        [Theory]
        [InlineData("0")]
        [InlineData("123")]
        [InlineData("-123")]
        [InlineData("999999999")]
        [InlineData("1234567890")]
        public void IsNumeric_String_ValidNumber_ReturnsTrue(string expression)
        {
            Assert.True(Validator.IsNumeric(expression));
        }

        [Theory]
        [InlineData("")]
        [InlineData("abc")]
        [InlineData("12a")]
        [InlineData("1 2")]
        [InlineData("2147483647000")]
        public void IsNumeric_String_InvalidNumber_ReturnsFalse(string expression)
        {
            Assert.False(Validator.IsNumeric(expression));
        }

        [Fact]
        public void IsNumeric_String_TooLongDigits_ReturnsFalse()
        {
            // 长度 10 但首位不是 1，长度 11 且不是 -1 开头，均返回 false
            Assert.False(Validator.IsNumeric("2222222222"));
            Assert.False(Validator.IsNumeric("22222222222"));
        }

        [Fact]
        public void IsNumeric_String_Null_ReturnsFalse()
        {
            Assert.False(Validator.IsNumeric((string)null!));
        }

        [Fact]
        public void IsNumeric_Object_UsesToString()
        {
            Assert.True(Validator.IsNumeric((object)123));
            Assert.False(Validator.IsNumeric((object)"abc"));
            Assert.False(Validator.IsNumeric((object)null!));
        }

        [Theory]
        [InlineData("1")]
        [InlineData("12.5")]
        [InlineData("0.0")]
        public void IsDouble_ValidValue_ReturnsTrue(string expression)
        {
            Assert.True(Validator.IsDouble(expression));
        }

        [Theory]
        [InlineData("")]
        [InlineData("abc")]
        [InlineData(".5")]
        [InlineData("-1")]
        public void IsDouble_InvalidValue_ReturnsFalse(string expression)
        {
            // 正则 ^([0-9])[0-9]*(\.\w*)?$ 不允许负号与省略整数部分
            Assert.False(Validator.IsDouble(expression));
        }

        [Fact]
        public void IsDouble_Null_ReturnsFalse()
        {
            Assert.False(Validator.IsDouble(null!));
        }

        [Fact]
        public void IsNumericArray_AllNumeric_ReturnsTrue()
        {
            Assert.True(Validator.IsNumericArray(new[] { "1", "2", "-3" }));
        }

        [Fact]
        public void IsNumericArray_ContainsNonNumeric_ReturnsFalse()
        {
            Assert.False(Validator.IsNumericArray(new[] { "1", "a" }));
        }

        [Fact]
        public void IsNumericArray_NullOrEmpty_ReturnsFalse()
        {
            Assert.False(Validator.IsNumericArray(null!));
            Assert.False(Validator.IsNumericArray(Array.Empty<string>()));
        }
    }
}
