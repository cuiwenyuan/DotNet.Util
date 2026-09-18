using System.Collections.Generic;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// StringUtil（NewLife 字符串辅助包装，Split/Join 已改为静态方法）测试
    /// </summary>
    public class StringUtilTests
    {
        [Fact]
        public void Split_ByStringSeparator()
        {
            Assert.Equal(new[] { "a", "b,c" }, StringUtil.Split("a;b,c", ";"));
        }

        [Fact]
        public void Split_NoMatch_ReturnsSingle()
        {
            Assert.Equal(new[] { "abc" }, StringUtil.Split("abc", "|"));
        }

        [Fact]
        public void Split_NoSeparators_ReturnsSingle()
        {
            // 不传分隔符时不拆分（NewLife 实现无默认分隔符）
            Assert.Equal(new[] { "a,b" }, StringUtil.Split("a,b"));
        }

        [Fact]
        public void Join_Generic()
        {
            Assert.Equal("a,b", StringUtil.Join(new List<string> { "a", "b" }, ","));
        }

        [Fact]
        public void Join_DefaultSeparator()
        {
            Assert.Equal("a,b", StringUtil.Join(new[] { "a", "b" }));
        }

        [Fact]
        public void EqualIgnoreCase_Works()
        {
            Assert.True("AbC".EqualIgnoreCase("abc"));
            Assert.True("AbC".EqualIgnoreCase("abc", "def"));
            Assert.False("AbC".EqualIgnoreCase("xyz"));
        }

        [Fact]
        public void StartsWithIgnoreCase_Works()
        {
            Assert.True("HelloWorld".StartsWithIgnoreCase("hello"));
            Assert.False("HelloWorld".StartsWithIgnoreCase("world"));
        }

        [Fact]
        public void EndsWithIgnoreCase_Works()
        {
            Assert.True("HelloWorld".EndsWithIgnoreCase("WORLD"));
        }

        [Fact]
        public void IsNullOrEmpty_Works()
        {
            Assert.True(((string?)null).IsNullOrEmpty());
            Assert.True(string.Empty.IsNullOrEmpty());
            Assert.False("abc".IsNullOrEmpty());
        }

        [Fact]
        public void IsNullOrWhiteSpace_Works()
        {
            Assert.True("   ".IsNullOrWhiteSpace());
            Assert.False("abc".IsNullOrWhiteSpace());
        }
    }
}
