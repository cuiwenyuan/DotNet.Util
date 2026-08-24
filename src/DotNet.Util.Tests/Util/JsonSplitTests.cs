using System.Reflection;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// JsonUtil.JsonSplit 测试
    /// 该类是 JsonUtil 的 internal 嵌套类，且未开放 InternalsVisibleTo，故通过反射调用
    /// </summary>
    public class JsonSplitTests
    {
        private static readonly Type SplitType = typeof(JsonUtil).GetNestedType("JsonSplit", BindingFlags.NonPublic)!;

        private static bool IsJson(string json)
        {
            var method = SplitType.GetMethod("IsJson", BindingFlags.Static | BindingFlags.NonPublic, null,
                new[] { typeof(string) }, null)!;
            return (bool)method.Invoke(null, new object[] { json })!;
        }

        private static List<Dictionary<string, string>> Split(string json)
        {
            var method = SplitType.GetMethod("Split", BindingFlags.Static | BindingFlags.NonPublic, null,
                new[] { typeof(string) }, null)!;
            return (List<Dictionary<string, string>>)method.Invoke(null, new object[] { json })!;
        }

        [Fact]
        public void JsonSplit_TypeExists()
        {
            Assert.NotNull(SplitType);
        }

        [Theory]
        [InlineData("{\"a\":1}")]
        [InlineData("{\"a\":1,\"b\":\"x\"}")]
        [InlineData("{\"a\":{\"b\":1}}")]
        [InlineData("[{\"a\":1},{\"a\":2}]")]
        public void IsJson_ValidJson_ReturnsTrue(string json)
        {
            Assert.True(IsJson(json));
        }

        [Theory]
        [InlineData("")]
        [InlineData("{")]
        [InlineData("not a json")]
        [InlineData("{\"a\":1")]
        [InlineData("\"a\":1}")]
        public void IsJson_InvalidJson_ReturnsFalse(string json)
        {
            Assert.False(IsJson(json));
        }

        [Fact]
        public void Split_SimpleObject_ReturnsOneDictionary()
        {
            var result = Split("{\"a\":1,\"b\":\"x\"}");

            Assert.Single(result);
            Assert.Equal("1", result[0]["a"]);
            Assert.Equal("x", result[0]["b"]);
        }

        [Fact]
        public void Split_KeyIsCaseInsensitive()
        {
            var result = Split("{\"Name\":\"Troy\"}");

            Assert.Single(result);
            Assert.Equal("Troy", result[0]["name"]);
            Assert.Equal("Troy", result[0]["NAME"]);
        }

        [Fact]
        public void Split_NullValue_ReturnsEmptyString()
        {
            var result = Split("{\"a\":null}");

            Assert.Single(result);
            Assert.Equal(string.Empty, result[0]["a"]);
        }

        [Fact]
        public void Split_NestedObject_KeepsRawChildJson()
        {
            var result = Split("{\"a\":{\"b\":1}}");

            Assert.Single(result);
            Assert.Equal("{\"b\":1}", result[0]["a"]);
        }

        [Fact]
        public void Split_Array_ReturnsOneDictionaryPerElement()
        {
            var result = Split("[{\"a\":1},{\"a\":2}]");

            Assert.Equal(2, result.Count);
            Assert.Equal("1", result[0]["a"]);
            Assert.Equal("2", result[1]["a"]);
        }

        [Fact]
        public void Split_EmptyOrNonJson_ReturnsEmptyList()
        {
            Assert.Empty(Split(string.Empty));
            Assert.Empty(Split("not a json"));
        }
    }
}
