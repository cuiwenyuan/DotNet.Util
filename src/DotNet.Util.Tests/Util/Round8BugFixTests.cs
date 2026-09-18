using System;
using System.Globalization;
using System.IO;
using System.Linq;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// 第 8 轮 Code Review 修复的回归测试（R8-1 ~ R8-8）
    /// </summary>
    public class Round8BugFixTests
    {
        #region R8-1 CheckPasswordStrength 空值不再 NRE
        [Fact]
        public void CheckPasswordStrength_NullOrEmpty_ReturnsFalse_NotThrow()
        {
            Assert.False(Validation.CheckPasswordStrength(null));
            Assert.False(Validation.CheckPasswordStrength(string.Empty));
            // 回归：原有 isDigit&&isLetter 逻辑仍正确
            Assert.False(Validation.CheckPasswordStrength("abc"));      // 无数字
            Assert.False(Validation.CheckPasswordStrength("123"));     // 无字母
            Assert.False(Validation.CheckPasswordStrength("ab1"));      // <6 位
            Assert.True(Validation.CheckPasswordStrength("abc123"));   // 数字+字母 >=6
        }
        #endregion

        #region R8-3 SaveFile 裸文件名（无目录）不崩溃
        [Fact]
        public void SaveFile_BareFileName_DoesNotThrow()
        {
            var bytes = new byte[] { 1, 2, 3, 4 };
            var path = "r8_savetest.bin";
            try
            {
                FileUtil.SaveFile(bytes, path);
                Assert.True(File.Exists(path));
                Assert.True(bytes.SequenceEqual(File.ReadAllBytes(path)));
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }
        #endregion

        #region R8-2 Convert 系列跨文化不变（de-DE 下 "1234.56" 不应被当作 123456）
        [Fact]
        public void ConvertToDecimal_IsCultureInvariant()
        {
            var prev = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("de-DE");
                Assert.Equal(1234.56m, BaseUtil.ConvertToDecimal("1234.56")); // 修复前=123456
                Assert.Equal(1234.56m, BaseUtil.ConvertToDecimal(1234.56m));
                Assert.Equal(1234, BaseUtil.ConvertToInt("1234"));
                Assert.Equal(1234L, BaseUtil.ConvertToInt64("1234"));
            }
            finally
            {
                CultureInfo.CurrentCulture = prev;
            }
        }
        #endregion

        #region R8-4 IsDouble 正则修复
        [Theory]
        [InlineData("123.45", true)]
        [InlineData("123", true)]
        [InlineData("123.abc", false)] // 修复前误判 true
        [InlineData("abc", false)]
        [InlineData("", false)]
        public void IsDouble_ReturnsExpected(string input, bool expected)
        {
            Assert.Equal(expected, Validator.IsDouble(input));
        }
        #endregion

        #region R8-5 IsMobile 号段白名单覆盖 14/16/17/19
        [Theory]
        [InlineData("13800138000", true)]
        [InlineData("14712345678", true)]
        [InlineData("17012345678", true)] // 17x
        [InlineData("16612345678", true)] // 16x
        [InlineData("19912345678", true)] // 19x
        [InlineData("12345678901", false)] // 12x 非法
        [InlineData("1380013800", false)]   // 10 位
        public void IsMobile_ReturnsExpected(string input, bool expected)
        {
            Assert.Equal(expected, Validation.IsMobile(input));
        }
        #endregion

        #region R8-6 EnumToDataTable 支持 long/ulong 底层类型
        private enum BigLong : long { A = 3000000000L, B = 1L }
        [Fact]
        public void EnumToDataTable_LongUnderlying_NoOverflow()
        {
            var dt = EnumUtil.EnumToDataTable(typeof(BigLong));
            Assert.Equal(typeof(long), dt.Columns["value"].DataType);
            Assert.Equal(3000000000L, Convert.ToInt64(dt.Rows[0]["value"]));
            Assert.Equal(1L, Convert.ToInt64(dt.Rows[1]["value"]));
        }
        #endregion

        #region R8-7 StringToInList 空值 + 单引号转义（保持 a','b','c 契约）
        [Fact]
        public void StringToInList_Null_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, StringUtil.StringToInList(null));
        }
        [Fact]
        public void StringToInList_EscapesSingleQuote()
        {
            Assert.Equal("O''Brien','Smith", StringUtil.StringToInList("O'Brien,Smith"));
            Assert.Equal("a','b','c", StringUtil.StringToInList("a,b,c"));
        }
        #endregion

        #region R8-8 GetLike 空串返回空 + 转义通配符/单引号
        [Fact]
        public void GetLike_EmptySearch_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, StringUtil.GetLike("Name", ""));
        }
        [Fact]
        public void GetLike_EscapesWildcardsAndQuote()
        {
            var sql = StringUtil.GetLike("Name", "100%");
            Assert.Contains("[%]", sql);   // 百分号被转义为 [%]
            Assert.DoesNotContain("LIKE '%100%%'", sql);
            var sql2 = StringUtil.GetLike("Name", "O'Brien");
            Assert.Contains("''", sql2);    // 单引号转义
        }
        #endregion
    }
}
