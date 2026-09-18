using System.Collections;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// CollectionUtil 测试（纯逻辑，不依赖外部资源）
    /// </summary>
    public class CollectionUtilTests
    {
        #region IsNullOrEmpty

        [Fact]
        public void IsNullOrEmpty_Null_ReturnsTrue()
        {
            Assert.True(CollectionUtil.IsNullOrEmpty(null!));
        }

        [Fact]
        public void IsNullOrEmpty_EmptyCollections_ReturnsTrue()
        {
            Assert.True(CollectionUtil.IsNullOrEmpty(new int[0]));
            Assert.True(CollectionUtil.IsNullOrEmpty(new List<string>()));
            // 字符串也是 IEnumerable，空串没有元素
            Assert.True(CollectionUtil.IsNullOrEmpty(string.Empty));
        }

        [Fact]
        public void IsNullOrEmpty_NonEmptyCollections_ReturnsFalse()
        {
            Assert.False(CollectionUtil.IsNullOrEmpty(new[] { 1 }));
            Assert.False(CollectionUtil.IsNullOrEmpty(new List<string> { "a" }));
            Assert.False(CollectionUtil.IsNullOrEmpty("a"));
        }

        [Fact]
        public void IsNullOrEmpty_Hashtable_WorksOnNonGenericEnumerable()
        {
            IEnumerable empty = new Hashtable();
            var one = new Hashtable { { "k", "v" } };

            Assert.True(CollectionUtil.IsNullOrEmpty(empty));
            Assert.False(CollectionUtil.IsNullOrEmpty(one));
        }

        #endregion

        #region IsNotNullAndNotEmpty

        [Fact]
        public void IsNotNullAndNotEmpty_Null_ReturnsFalse()
        {
            Assert.False(CollectionUtil.IsNotNullAndNotEmpty<int>(null!));
        }

        [Fact]
        public void IsNotNullAndNotEmpty_Empty_ReturnsFalse()
        {
            Assert.False(CollectionUtil.IsNotNullAndNotEmpty(new List<int>()));
        }

        [Fact]
        public void IsNotNullAndNotEmpty_NonEmpty_ReturnsTrue()
        {
            Assert.True(CollectionUtil.IsNotNullAndNotEmpty(new List<int> { 0 }));
        }

        #endregion

        #region StringJoin

        [Fact]
        public void StringJoin_DefaultSeparator_UsesComma()
        {
            Assert.Equal("a,b,c", CollectionUtil.StringJoin(new[] { "a", "b", "c" }));
        }

        [Fact]
        public void StringJoin_CustomSeparator()
        {
            Assert.Equal("a-b", CollectionUtil.StringJoin(new List<string> { "a", "b" }, "-"));
        }

        [Fact]
        public void StringJoin_SingleOrEmpty_EdgeCases()
        {
            Assert.Equal("a", CollectionUtil.StringJoin(new[] { "a" }));
            Assert.Equal(string.Empty, CollectionUtil.StringJoin(new string[0]));
        }

        [Fact]
        public void StringJoin_Null_Throws()
        {
            // 内部直接调用 string.Join，null 集合会抛 ArgumentNullException
            Assert.Throws<ArgumentNullException>(() => CollectionUtil.StringJoin(null!));
        }

        #endregion
    }
}
