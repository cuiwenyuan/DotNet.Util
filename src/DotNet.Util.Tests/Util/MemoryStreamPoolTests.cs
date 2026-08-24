using System.IO;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// PoolUtil.MemoryStreamPool 测试（纯逻辑，仅内存流，不涉及真实文件）
    /// </summary>
    public class MemoryStreamPoolTests
    {
        [Fact]
        public void Defaults_InitialAndMaximumCapacity()
        {
            var pool = new PoolUtil.MemoryStreamPool();

            Assert.Equal(1024, pool.InitialCapacity);
            Assert.Equal(64 * 1024, pool.MaximumCapacity);
        }

        [Fact]
        public void Get_CreatesInstanceWithInitialCapacity()
        {
            var pool = new PoolUtil.MemoryStreamPool { InitialCapacity = 2048 };

            var ms = pool.Get();

            Assert.NotNull(ms);
            Assert.True(ms.Capacity >= 2048);
            Assert.Equal(0, ms.Length);
        }

        [Fact]
        public void Get_OnEmptyPool_ReturnsDistinctInstances()
        {
            var pool = new PoolUtil.MemoryStreamPool();

            var first = pool.Get();
            var second = pool.Get();

            Assert.NotSame(first, second);
        }

        [Fact]
        public void Return_SmallStream_ReturnsTrueAndResets()
        {
            var pool = new PoolUtil.MemoryStreamPool();
            var ms = new MemoryStream();
            ms.Write(new byte[] { 1, 2, 3, 4 }, 0, 4);

            Assert.True(pool.Return(ms));
            Assert.Equal(0, ms.Length);
            Assert.Equal(0, ms.Position);
        }

        [Fact]
        public void Return_CapacityEqualsMaximum_ReturnsTrue()
        {
            // 源码判断条件是 Capacity > MaximumCapacity，等于时应可归还（边界）
            var pool = new PoolUtil.MemoryStreamPool { MaximumCapacity = 256 };
            var ms = new MemoryStream(256);

            Assert.Equal(256, ms.Capacity);
            Assert.True(pool.Return(ms));
            Assert.Equal(0, ms.Length);
        }

        [Fact]
        public void Return_OversizedStream_ReturnsFalseAndKeepsData()
        {
            var pool = new PoolUtil.MemoryStreamPool { MaximumCapacity = 8 };
            var ms = new MemoryStream();
            ms.Write(new byte[100], 0, 100);

            Assert.True(ms.Capacity > 8);
            Assert.False(pool.Return(ms));
            // 超限时提前返回，不会被重置
            Assert.Equal(100, ms.Length);
        }

        [Fact]
        public void Return_Null_ThrowsNullReferenceException()
        {
            var pool = new PoolUtil.MemoryStreamPool();

            // 源码直接读取 ms.Capacity，未做 null 判断
            Assert.Throws<NullReferenceException>(() => pool.Return(null!));
        }

        [Fact]
        public void PoolUtil_MemoryStream_StaticPool_IsAvailable()
        {
            Assert.NotNull(PoolUtil.MemoryStream);

            var ms = PoolUtil.MemoryStream.Get();
            Assert.NotNull(ms);
            Assert.Equal(0, ms.Length);
        }
    }
}
