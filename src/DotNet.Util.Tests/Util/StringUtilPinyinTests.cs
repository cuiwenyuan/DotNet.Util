using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// StringUtil.PinyinHelper 测试（纯逻辑：汉字转拼音/简拼）
    /// </summary>
    public class StringUtilPinyinTests
    {
        [Fact]
        public void GetSimpleSpelling_Chinese_ReturnsInitials()
        {
            Assert.Equal("ZG", StringUtil.GetSimpleSpelling("中国"));
            Assert.Equal("CWY", StringUtil.GetSimpleSpelling("崔文远"));
        }

        [Fact]
        public void GetSimpleSpelling_NonChinese_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, StringUtil.GetSimpleSpelling("abc123"));
        }

        [Fact]
        public void GetSimpleSpelling_EmptyOrNull_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, StringUtil.GetSimpleSpelling(""));
            Assert.Equal(string.Empty, StringUtil.GetSimpleSpelling(null));
        }

        [Fact]
        public void GetPinyin_SingleCharacter_ReturnsFullPinyin()
        {
            Assert.Equal("Zhong", StringUtil.GetPinyin("中"));
            Assert.Equal("Guo", StringUtil.GetPinyin("国"));
        }

        [Fact]
        public void GetPinyin_MultiCharacter_ReturnsConcatenated()
        {
            Assert.Equal("ZhongGuo", StringUtil.GetPinyin("中国"));
        }

        [Fact]
        public void GetPinyin_Empty_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, StringUtil.GetPinyin(""));
            Assert.Equal(string.Empty, StringUtil.GetPinyin(null));
        }

        [Fact]
        public void GetPinyinAll_SingleCharacter()
        {
            // 单字返回首字母（多音字以逗号分隔多个可能）
            Assert.Equal("Z", StringUtil.GetPinyinAll("中"));
        }

        [Fact]
        public void GetPinyinAll_MultiCharacter_AllInitialsJoined()
        {
            Assert.Equal("ZG", StringUtil.GetPinyinAll("中国"));
        }

        [Fact]
        public void GetPinyinAll_EmptyOrNull_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, StringUtil.GetPinyinAll(""));
            Assert.Equal(string.Empty, StringUtil.GetPinyinAll(null));
        }
    }
}
