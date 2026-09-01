using System;
using System.Text;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// StringUtil 主体测试（纯字符串逻辑：Like/Concat/Remove/IN 列表/十六进制/脱敏等）
    /// 类名避开已有的 StringUtilTests / StringUtilCodeStyleTests
    /// </summary>
    public class StringUtilCoreTests
    {
        #region GetLike / GetSearchString / GetLikeSearchKey

        [Fact]
        public void GetLike_MultiChar_GeneratesAndConditions()
        {
            var result = StringUtil.GetLike("Name", "ab");

            Assert.Equal("(Name LIKE '%a%' AND Name LIKE '%b%')", result);
        }

        [Fact]
        public void GetSearchString_Default_WrapsWithPercent()
        {
            Assert.Equal("%abc%", StringUtil.GetSearchString("abc"));
        }

        [Fact]
        public void GetSearchString_AllLike_InsertsPercentPerChar()
        {
            Assert.Equal("%a%b%c%", StringUtil.GetSearchString("abc", true));
        }

        [Fact]
        public void GetSearchString_Percent_EscapesToBrackets()
        {
            Assert.Equal("[%]", StringUtil.GetSearchString("%"));
        }

        [Fact]
        public void GetSearchString_Brackets_EscapedForLike()
        {
            // 修正 R8-11：字面 [ ] 转义为 LIKE 通配符形式 [[] / []]，不再误变成下划线，且整体包裹 %
            Assert.Equal("%a[[]b[]]%", StringUtil.GetSearchString("a[b]"));
        }

        [Fact]
        public void GetSearchString_Empty_ReturnsAsIs()
        {
            // 空串/为 null 时直接原样返回（实现中不进入处理分支）
            Assert.Equal("", StringUtil.GetSearchString(""));
            Assert.Null(StringUtil.GetSearchString(null));
        }

        [Fact]
        public void GetLikeSearchKey_EscapesWildcards()
        {
            Assert.Equal("[%]", StringUtil.GetLikeSearchKey("%"));
            Assert.Equal("[_]", StringUtil.GetLikeSearchKey("_"));
            Assert.Equal("a[[]b", StringUtil.GetLikeSearchKey("a[b"));
            Assert.Equal("", StringUtil.GetLikeSearchKey(""));
        }

        #endregion

        #region Exists / Concat / Remove

        [Fact]
        public void Exists_FindsTarget()
        {
            Assert.True(StringUtil.Exists(new[] { "a", "b" }, "b"));
            Assert.False(StringUtil.Exists(new[] { "a", "b" }, "z"));
            Assert.False(StringUtil.Exists(null, "z"));
            Assert.False(StringUtil.Exists(new[] { "a" }, ""));
        }

        [Fact]
        public void Concat_AppendsAndDeduplicates()
        {
            var result = StringUtil.Concat(new[] { "a", "b" }, "c");

            Assert.Equal(new[] { "a", "b", "c" }, result);
        }

        [Fact]
        public void Concat_Params_JoinsArraysDeduplicating()
        {
            var result = StringUtil.Concat(new[] { "a", "b" }, new[] { "b", "c" }, new[] { "a", null, "" });

            Assert.Equal(new[] { "a", "b", "c" }, result);
        }

        [Fact]
        public void Concat_Null_ReturnsEmpty()
        {
            Assert.Empty(StringUtil.Concat(null));
        }

        [Fact]
        public void Remove_RemovesSpecifiedIds()
        {
            var result = StringUtil.Remove(new[] { "a", "b", "c" }, new[] { "b" });

            Assert.Equal(new[] { "a", "c" }, result);
        }

        [Fact]
        public void Remove_SingleId_RemovesIt()
        {
            var result = StringUtil.Remove(new[] { "a", "b" }, "a");

            Assert.Equal(new[] { "b" }, result);
        }

        [Fact]
        public void Remove_EmptyId_ReturnsOriginal()
        {
            var ids = new[] { "a", "b" };
            Assert.Same(ids, StringUtil.Remove(ids, ""));
        }

        [Fact]
        public void Remove_NullIds_ReturnsEmpty()
        {
            Assert.Empty(StringUtil.Remove(null, new[] { "a" }));
        }

        #endregion

        #region StringToInList / ArrayToList / RepeatString / DeleteUnVisibleChar

        [Fact]
        public void StringToInList_ConvertsSeparator()
        {
            Assert.Equal("a','b','c", StringUtil.StringToInList("a,b,c"));
        }

        [Fact]
        public void StringToInList_CustomSeparators()
        {
            Assert.Equal("a;b;c", StringUtil.StringToInList("a-b-c", "-", ";"));
        }

        [Fact]
        public void ArrayToList_Default_NoSeparator()
        {
            // 默认分隔符为 ""：直接逗号拼接
            Assert.Equal("a,b", StringUtil.ArrayToList(new[] { "a", "b" }));
        }

        [Fact]
        public void ArrayToList_WithQuoteSeparator()
        {
            // 传 "'" 作为分隔符时得到引号包裹
            Assert.Equal("'a','b'", StringUtil.ArrayToList(new[] { "a", "b" }, "'"));
        }

        [Fact]
        public void ArrayToList_Empty_ReturnsEmpty()
        {
            Assert.Equal("", StringUtil.ArrayToList(new string[0]));
        }

        [Fact]
        public void RepeatString_Repeats()
        {
            Assert.Equal("ababab", StringUtil.RepeatString("ab", 3));
            Assert.Equal("", StringUtil.RepeatString("ab", 0));
        }

        [Fact]
        public void DeleteUnVisibleChar_RemovesControlChars()
        {
            // \r(13) \n(10) \t(9) 均 < 16 应被删除，空格(32)与可见字符保留
            var result = StringUtil.DeleteUnVisibleChar("a\r\n\tb");

            Assert.Equal("ab", result);
        }

        #endregion

        #region SplitMobile

        [Fact]
        public void SplitMobile_Valid11Digit_Returns()
        {
            var result = StringUtil.SplitMobile("13800138000");

            Assert.Equal(new[] { "13800138000" }, result);
        }

        [Fact]
        public void SplitMobile_MobileOnly_KeepsNonEmptyAll()
        {
            // 实现中 mobileOnly=true 仅过滤空串，长度不足 11 位也会保留
            var result = StringUtil.SplitMobile("13800138000,123,13900139000");

            Assert.Equal(new[] { "13800138000", "123", "13900139000" }, result);
        }

        [Fact]
        public void SplitMobile_MobileOnlyFalse_SameBehavior()
        {
            var result = StringUtil.SplitMobile("13800138000,123", false);

            Assert.Equal(2, result.Length);
        }

        [Fact]
        public void SplitMobile_Distinct_RemovesDuplicates()
        {
            var result = StringUtil.SplitMobile("13800138000,13800138000");

            Assert.Single(result);
        }

        [Fact]
        public void SplitMobile_MixedSeparators()
        {
            // 全角逗号、分号、空格、换行都识别
            var result = StringUtil.SplitMobile("13800138000，13900139000\n13800138000;13700137000");

            Assert.Equal(3, result.Length);
        }

        #endregion

        #region StringToUnicode / CutString

        [Fact]
        public void StringToUnicode_Ascii()
        {
            Assert.Equal("\\u0041", StringUtil.StringToUnicode("A"));
        }

        [Fact]
        public void StringToUnicode_NA_ReturnsEmpty()
        {
            Assert.Equal("", StringUtil.StringToUnicode("N/A"));
            Assert.Equal("", StringUtil.StringToUnicode("n/a"));
        }

        [Fact]
        public void CutString_Ascii_TruncatesAndAddsEllipsis()
        {
            // 实现先追加再判越界，len=2 时实际保留 3 个字符
            Assert.Equal("abc..", StringUtil.CutString("abcdef", 2));
        }

        [Fact]
        public void CutString_Null_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, StringUtil.CutString(null, 5));
        }

        [Fact]
        public void CutString_Short_NoEllipsis()
        {
            Assert.Equal("ab", StringUtil.CutString("ab", 5));
        }

        #endregion

        #region BytesToHexString / HexStringToBytes

        [Fact]
        public void BytesToHexString_Converts()
        {
            Assert.Equal("0AFF", StringUtil.BytesToHexString(new byte[] { 0x0A, 0xFF }));
        }

        [Fact]
        public void HexStringToBytes_Converts()
        {
            Assert.Equal(new byte[] { 0x0A, 0xFF }, StringUtil.HexStringToBytes("0AFF"));
        }

        [Fact]
        public void HexStringToBytes_OddLength_PrependsZero()
        {
            Assert.Equal(new byte[] { 0x0A, 0x0F }, StringUtil.HexStringToBytes("A0F"));
        }

        [Fact]
        public void HexStringToBytes_Empty_ReturnsZeroByte()
        {
            Assert.Equal(new byte[] { 0 }, StringUtil.HexStringToBytes(""));
        }

        [Fact]
        public void HexRoundTrip()
        {
            var input = Encoding.UTF8.GetBytes("DotNet.Util 中文");
            var hex = StringUtil.BytesToHexString(input);
            var back = StringUtil.HexStringToBytes(hex);

            Assert.Equal(input, back);
        }

        #endregion

        #region MergeSpace / CutString(start,end) / 脱敏

        [Fact]
        public void MergeSpace_CollapsesWhitespace()
        {
            Assert.Equal("a b c", StringUtil.MergeSpace("a   b \t c"));
            Assert.Equal("", StringUtil.MergeSpace(""));
        }

        [Fact]
        public void CutString_BetweenMarkers()
        {
            // 截取 start 之后、end 之前（不含括号本身）
            Assert.Equal("bcd", StringUtil.CutString("a[bcd]e", "[", "]"));
        }

        [Fact]
        public void CutString_MissingMarker_ReturnsEmpty()
        {
            Assert.Equal("", StringUtil.CutString("abcdef", "[", "]"));
            Assert.Equal("", StringUtil.CutString("", "[", "]"));
        }

        [Fact]
        public void HideSensitiveInfo_MiddleHidden()
        {
            Assert.Equal("138****5678", StringUtil.HideSensitiveInfo("13812345678", 3, 4));
        }

        [Fact]
        public void HideSensitiveInfo_TooShort_BasedOnLeft()
        {
            Assert.Equal("1****", StringUtil.HideSensitiveInfo("12", 3, 3));
        }

        [Fact]
        public void HideSensitiveInfo_TooShort_BasedOnRight()
        {
            Assert.Equal("****2", StringUtil.HideSensitiveInfo("12", 3, 3, false));
        }

        [Fact]
        public void HideSensitiveInfo_Ratio_Default()
        {
            // 长度 10，ratio 3 → subLength 3，返回 3 + **** + 3
            var result = StringUtil.HideSensitiveInfo("1234567890");

            Assert.Equal("123****890", result);
        }

        [Fact]
        public void HideSensitiveInfo_RatioLeOne_FallsBackToThree()
        {
            var result = StringUtil.HideSensitiveInfo("1234567890", 1);

            Assert.Equal("123****890", result);
        }

        [Fact]
        public void HideSensitiveInfo_Empty_ReturnsEmpty()
        {
            Assert.Equal("", StringUtil.HideSensitiveInfo(""));
            Assert.Equal("", StringUtil.HideSensitiveInfo(null));
        }

        [Fact]
        public void HideEmailDetails_MasksLocalPart()
        {
            // 仅保留前 3 位，隐藏中间 1 位：tes*@example.com
            var result = StringUtil.HideEmailDetails("test@example.com");

            Assert.Equal("tes*@example.com", result);
        }

        [Fact]
        public void HideEmailDetails_NotEmail_FallsBackToGeneric()
        {
            var result = StringUtil.HideEmailDetails("not-an-email");

            Assert.Contains("****", result);
        }

        #endregion
    }
}
