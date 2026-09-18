using System.Text;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// PoolUtil.StringBuilderPool 测试（纯逻辑，不依赖外部资源）
    /// </summary>
    public class StringBuilderPoolTests
    {
        [Fact]
        public void Defaults_InitialAndMaximumCapacity()
        {
            var pool = new PoolUtil.StringBuilderPool();

            Assert.Equal(100, pool.InitialCapacity);
            Assert.Equal(4 * 1024, pool.MaximumCapacity);
        }

        [Fact]
        public void Get_CreatesInstanceWithInitialCapacity()
        {
            var pool = new PoolUtil.StringBuilderPool { InitialCapacity = 128 };

            var sb = pool.Get();

            Assert.NotNull(sb);
            Assert.True(sb.Capacity >= 128);
            Assert.Equal(0, sb.Length);
        }

        [Fact]
        public void Get_OnEmptyPool_ReturnsDistinctInstances()
        {
            var pool = new PoolUtil.StringBuilderPool();

            var first = pool.Get();
            var second = pool.Get();

            Assert.NotSame(first, second);
        }

        [Fact]
        public void Return_SmallBuilder_ReturnsTrueAndClears()
        {
            var pool = new PoolUtil.StringBuilderPool();
            var sb = new StringBuilder("hello world");

            Assert.True(pool.Return(sb));
            Assert.Equal(0, sb.Length);
        }

        [Fact]
        public void Return_CapacityEqualsMaximum_ReturnsTrue()
        {
            // 源码判断条件是 Capacity > MaximumCapacity，等于时应可归还（边界）
            var pool = new PoolUtil.StringBuilderPool { MaximumCapacity = 64 };
            var sb = new StringBuilder(64);
            sb.Append("x");

            Assert.Equal(64, sb.Capacity);
            Assert.True(pool.Return(sb));
            Assert.Equal(0, sb.Length);
        }

        [Fact]
        public void Return_OversizedBuilder_ReturnsFalseAndKeepsContent()
        {
            var pool = new PoolUtil.StringBuilderPool { MaximumCapacity = 8 };
            var sb = new StringBuilder(new string('x', 100));

            Assert.False(pool.Return(sb));
            // 超限时提前返回，不会被清空
            Assert.Equal(100, sb.Length);
        }

        [Fact]
        public void Return_Null_ThrowsNullReferenceException()
        {
            var pool = new PoolUtil.StringBuilderPool();

            // 源码直接读取 sb.Capacity，未做 null 判断
            Assert.Throws<NullReferenceException>(() => pool.Return(null!));
        }

        [Fact]
        public void PoolUtil_StringBuilder_StaticPool_IsAvailable()
        {
            Assert.NotNull(PoolUtil.StringBuilder);

            var sb = PoolUtil.StringBuilder.Get();
            Assert.NotNull(sb);
            Assert.Equal(0, sb.Length);
        }
    }
}
