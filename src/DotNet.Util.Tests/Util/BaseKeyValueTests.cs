using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// BaseKeyValue 测试（纯逻辑 POCO）
    /// </summary>
    public class BaseKeyValueTests
    {
        [Fact]
        public void Ctor_Default_EmptyStrings()
        {
            var item = new BaseKeyValue();

            Assert.Equal(string.Empty, item.Key);
            Assert.Equal(string.Empty, item.Value);
            Assert.Equal(string.Empty, item.Description);
        }

        [Fact]
        public void Properties_AreSettable()
        {
            var item = new BaseKeyValue
            {
                Key = "cn",
                Value = "中国",
                Description = "国家"
            };

            Assert.Equal("cn", item.Key);
            Assert.Equal("中国", item.Value);
            Assert.Equal("国家", item.Description);
        }

        [Fact]
        public void Instances_UseReferenceEquality()
        {
            var a = new BaseKeyValue { Key = "x" };
            var b = new BaseKeyValue { Key = "x" };

            Assert.NotSame(a, b);
            Assert.NotEqual(a, b);
        }
    }
}
