using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// ObjectUtil 测试
    /// </summary>
    public class ObjectUtilTests
    {
        [Fact]
        public void ToList_DefaultSeparator_JoinsWithComma()
        {
            var result = ObjectUtil.ToList(new object[] { 1, 2, 3 });
            Assert.Equal("1,2,3", result);
        }

        [Fact]
        public void ToList_WithSeparator_WrapsEachValue()
        {
            var result = ObjectUtil.ToList(new object[] { 1, 2, 3 }, "::");
            Assert.Equal("::1::,::2::,::3::", result);
        }

        [Fact]
        public void ToList_SingleValue_WithoutSeparator()
        {
            Assert.Equal("5", ObjectUtil.ToList(new object[] { 5 }));
        }

        [Fact]
        public void ToList_EmptyArray_ReturnsNullLiteral()
        {
            Assert.Equal("NULL", ObjectUtil.ToList(new object[] { }));
        }

        [Fact]
        public void ToList_NullArray_ReturnsNullLiteral()
        {
            Assert.Equal("NULL", ObjectUtil.ToList(null!));
        }
    }
}
