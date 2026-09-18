using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// StringUtil 命名风格转换方法（StringUtil.CodeStyle.cs）测试
    /// </summary>
    public class StringUtilCodeStyleTests
    {
        #region ConvertToPascal
        [Fact]
        public void ConvertToPascal_UnderscoreDelimiter()
        {
            Assert.Equal("UserName", StringUtil.ConvertToPascal("user_name", "_"));
            Assert.Equal("UserName", StringUtil.ConvertToPascal("USER_NAME", "_"));
        }

        [Fact]
        public void ConvertToPascal_NoDelimiter_CapitalizesFirst()
        {
            Assert.Equal("Username", StringUtil.ConvertToPascal("username", null));
            Assert.Equal("UserName", StringUtil.ConvertToPascal("UserName", null));
        }

        [Fact]
        public void ConvertToPascal_AllCapsNoDelimiter_LowercasesRest()
        {
            Assert.Equal("Username", StringUtil.ConvertToPascal("USERNAME", "_"));
        }

        [Fact]
        public void ConvertToPascal_SingleChar_Uppercase()
        {
            Assert.Equal("A", StringUtil.ConvertToPascal("a", null));
        }

        [Fact]
        public void ConvertToPascal_NullOrWhitespace_ReturnsInput()
        {
            Assert.Equal("", StringUtil.ConvertToPascal("", "_"));
            Assert.Equal("  ", StringUtil.ConvertToPascal("  ", "_"));
        }
        #endregion

        #region ConvertToCamel
        [Fact]
        public void ConvertToCamel_UnderscoreDelimiter()
        {
            Assert.Equal("userName", StringUtil.ConvertToCamel("user_name", "_"));
            Assert.Equal("userName", StringUtil.ConvertToCamel("USER_NAME", "_"));
        }

        [Fact]
        public void ConvertToCamel_NoDelimiter_LowercasesFirst()
        {
            Assert.Equal("userName", StringUtil.ConvertToCamel("UserName", null));
        }

        [Fact]
        public void ConvertToCamel_SingleChar_Lowercase()
        {
            Assert.Equal("a", StringUtil.ConvertToCamel("A", null));
        }
        #endregion

        #region IsAllEnglishLetterUpperCase / LowerCase
        [Fact]
        public void IsAllEnglishLetterUpperCase_Works()
        {
            Assert.True(StringUtil.IsAllEnglishLetterUpperCase("ABC"));
            Assert.False(StringUtil.IsAllEnglishLetterUpperCase("AbC"));
        }

        [Fact]
        public void IsAllEnglishLetterLowerCase_Works()
        {
            Assert.True(StringUtil.IsAllEnglishLetterLowerCase("abc"));
            Assert.False(StringUtil.IsAllEnglishLetterLowerCase("Abc"));
        }
        #endregion

        #region ReplaceFirst
        [Fact]
        public void ReplaceFirst_ReplacesOnlyFirst()
        {
            Assert.Equal("a-b_c", StringUtil.ReplaceFirst("a_b_c", "_", "-"));
        }

        [Fact]
        public void ReplaceFirst_NoDelimiter_ReturnsInput()
        {
            Assert.Equal("abc", StringUtil.ReplaceFirst("abc", "_", "-"));
        }
        #endregion
    }
}
