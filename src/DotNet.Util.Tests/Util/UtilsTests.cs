using System.Linq;
using System.Text;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// Utils 纯逻辑测试
    ///
    /// 重要说明：Utils.cs 大量方法（GetStringLength/InArray/Md5/SHA256 等）被
    /// #if NET46_OR_GREATER 包裹，net8.0 下不参与编译。本测试只覆盖 net8.0
    /// 编译产物中真实存在的方法：GbkEncoding / DistinctStringArray / EncodeHtml /
    /// SplitString / PadStringArray / GetIp。
    /// </summary>
    public class UtilsTests
    {
        static UtilsTests()
        {
#if NET8_0_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }

        [Fact]
        public void GbkEncoding_IsUsable()
        {
            // 中文在 GBK 下占 2 字节
            Assert.Equal(4, Utils.GbkEncoding.GetBytes("中国").Length);
        }

        [Fact]
        public void DistinctStringArray_RemovesDuplicates()
        {
            var result = Utils.DistinctStringArray(new[] { "a", "b", "a", "c" });

            Assert.Equal(3, result.Length);
            Assert.Contains("a", result);
            Assert.Contains("b", result);
            Assert.Contains("c", result);
        }

        [Fact]
        public void DistinctStringArray_WithMaxLength_TruncatesKeys()
        {
            var result = Utils.DistinctStringArray(new[] { "abc", "abd" }, 2);

            // 两个元素截断后 key 都为 "ab"，去重后只剩 1 个
            Assert.Single(result);
        }

        [Fact]
        public void DistinctStringArray_NullArray_Throws()
        {
            Assert.Throws<System.NullReferenceException>(() => Utils.DistinctStringArray(null));
        }

        [Fact]
        public void EncodeHtml_ReplacesSpecialChars()
        {
            Assert.Equal("a&defb", Utils.EncodeHtml("a,b"));
            Assert.Equal("a&dotb", Utils.EncodeHtml("a'b"));
            Assert.Equal("a&decb", Utils.EncodeHtml("a;b"));
            Assert.Equal("", Utils.EncodeHtml(""));
        }

        [Fact]
        public void SplitString_Basic()
        {
            var result = Utils.SplitString("a,b,c", ",");

            Assert.Equal(new[] { "a", "b", "c" }, result);
        }

        [Fact]
        public void SplitString_NoSeparator_ReturnsWhole()
        {
            var result = Utils.SplitString("abc", ",");

            Assert.Single(result);
            Assert.Equal("abc", result[0]);
        }

        [Fact]
        public void SplitString_Empty_ReturnsEmptyArray()
        {
            var result = Utils.SplitString("", ",");

            Assert.Empty(result);
        }

        [Fact]
        public void SplitString_WithCount_PadsEmpty()
        {
            var result = Utils.SplitString("a,b", ",", 4);

            Assert.Equal(4, result.Length);
            Assert.Equal("a", result[0]);
            Assert.Equal("b", result[1]);
            Assert.Equal("", result[2]);
            Assert.Equal("", result[3]);
        }

        [Fact]
        public void SplitString_IgnoreRepeat_Distinct()
        {
            var result = Utils.SplitString("a,b,a", ",", true);

            Assert.Equal(2, result.Length);
            Assert.Contains("a", result);
            Assert.Contains("b", result);
        }

        [Fact]
        public void SplitString_MinMaxLength_PadsAndTruncates()
        {
            var result = Utils.SplitString("a,bb,ccc", ",", false, 2, 2);

            // "a" 被 minLength=2 过滤，其余截断为 2 位
            Assert.Equal(2, result.Length);
            Assert.Contains("bb", result);
            Assert.Contains("cc", result);
        }

        [Fact]
        public void PadStringArray_FiltersAndTruncates()
        {
            var result = Utils.PadStringArray(new[] { "a", "bbbb", "ccc" }, 2, 3);

            // "a" 被过滤，"bbbb" 截断为 "bbb"
            Assert.Equal(2, result.Length);
            Assert.Contains("bbb", result);
            Assert.Contains("ccc", result);
        }

        [Fact]
        public void PadStringArray_MinGreaterThanMax_Swaps()
        {
            // min=3, max=1 → 交换为 min=1, max=3，abcd 截断为 abc
            var result = Utils.PadStringArray(new[] { "abcd" }, 3, 1);

            Assert.Single(result);
            Assert.Equal("abc", result[0]);
        }

        [Fact]
        public void GetIp_ReturnsNonEmptyOrEmptyWithoutThrowing()
        {
            // 纯环境依赖（MachineInfo），只验证不抛异常且返回 string
            var ip = Utils.GetIp();

            Assert.NotNull(ip);
        }
    }
}
