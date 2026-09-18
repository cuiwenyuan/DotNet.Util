using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// Validation 校验类测试（纯静态、确定性）
    /// </summary>
    public class ValidationTests
    {
        #region CheckFolderName / CheckFileName
        [Fact]
        public void CheckFolderName_Works()
        {
            Assert.True(Validation.CheckFolderName("MyFolder"));
            Assert.False(Validation.CheckFolderName(""));
            Assert.False(Validation.CheckFolderName("a/b"));
        }

        [Fact]
        public void CheckFileName_Works()
        {
            Assert.True(Validation.CheckFileName("file.txt"));
            Assert.False(Validation.CheckFileName(""));
            Assert.False(Validation.CheckFileName("a/b.txt"));
        }
        #endregion

        #region CheckPasswordStrength
        [Fact]
        public void CheckPasswordStrength_Works()
        {
            Assert.True(Validation.CheckPasswordStrength("abc123"));
            Assert.False(Validation.CheckPasswordStrength("abc"));
            Assert.False(Validation.CheckPasswordStrength("123456"));
            Assert.False(Validation.CheckPasswordStrength(""));
        }
        #endregion

        #region IsBlank / IsNumeric
        [Fact]
        public void IsBlank_Works()
        {
            Assert.True(Validation.IsBlank(""));
            Assert.True(Validation.IsBlank(null));
            Assert.False(Validation.IsBlank("x"));
        }

        [Fact]
        public void IsNumeric_Works()
        {
            Assert.True(Validation.IsNumeric("123"));
            Assert.True(Validation.IsNumeric("-1.5"));
            Assert.False(Validation.IsNumeric("abc"));
            Assert.False(Validation.IsNumeric("1.2.3"));
            Assert.False(Validation.IsNumeric(""));
        }
        #endregion

        #region IsDateTime
        [Fact]
        public void IsDateTime_Works()
        {
            Assert.True(Validation.IsDateTime("2020-01-01"));
            Assert.False(Validation.IsDateTime("2020-13-01"));
            Assert.False(Validation.IsDateTime("abc"));
        }
        #endregion

        #region IsEmail / IsMobile
        [Fact]
        public void IsEmail_Works()
        {
            Assert.True(Validation.IsEmail("a@b.com"));
            Assert.False(Validation.IsEmail("ab.com"));
            Assert.False(Validation.IsEmail("a@b"));
        }

        [Fact]
        public void IsMobile_Works()
        {
            Assert.True(Validation.IsMobile("13800138000"));
            Assert.False(Validation.IsMobile("12345678901"));
            Assert.False(Validation.IsMobile(""));
        }
        #endregion

        #region IsPassword / IsLoginName / IsQq
        [Fact]
        public void IsPassword_Works()
        {
            Assert.True(Validation.IsPassword("abcdef"));
            Assert.False(Validation.IsPassword("12345"));
            Assert.False(Validation.IsPassword("a b"));
        }

        [Fact]
        public void IsLoginName_Works()
        {
            Assert.True(Validation.IsLoginName("troy"));
            Assert.False(Validation.IsLoginName(""));
            Assert.False(Validation.IsLoginName(" "));
        }

        [Fact]
        public void IsQq_Works()
        {
            Assert.True(Validation.IsQq("12345"));
            Assert.False(Validation.IsQq("0"));
            Assert.False(Validation.IsQq("abc"));
            Assert.False(Validation.IsQq("0123"));
        }
        #endregion
    }
}
