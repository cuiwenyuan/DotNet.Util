using System.IO;
using System.Text;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.NewLife
{
    /// <summary>
    /// PoolUtil 对象池扩展方法测试（纯逻辑，不依赖外部服务）。
    /// </summary>
    public class PoolUtilTests
    {
        [Fact]
        public void StringBuilder_Return_WithResult_ReturnsContent()
        {
            var sb = new StringBuilder("hello");
            var str = sb.Return(true);
            Assert.Equal("hello", str);
        }

        [Fact]
        public void StringBuilder_Return_WithoutResult_ReturnsNull()
        {
            var sb = new StringBuilder("hello");
            var str = sb.Return(false);
            Assert.Null(str);
        }

        [Fact]
        public void StringBuilder_Return_Null_DoesNotThrow()
        {
            StringBuilder? sb = null;
            Assert.Null(sb!.Return(true));
        }

        [Fact]
        public void MemoryStream_Return_WithResult_ReturnsBuffer()
        {
            using var ms = new MemoryStream();
            ms.Write(new byte[] { 1, 2, 3 }, 0, 3);
            var buf = ms.Return(true);
            Assert.NotNull(buf);
            Assert.Equal(3, buf!.Length);
        }

        [Fact]
        public void MemoryStream_Return_WithoutResult_ReturnsNull()
        {
            using var ms = new MemoryStream();
            var buf = ms.Return(false);
            Assert.Null(buf);
        }

        [Fact]
        public void MemoryStream_Return_Null_DoesNotThrow()
        {
            MemoryStream? ms = null;
            Assert.Null(ms!.Return(true));
        }

        [Fact]
        public void StringBuilderPool_ReturnsTrue_ForSmallCapacity()
        {
            var pool = new PoolUtil.StringBuilderPool();
            var sb = new StringBuilder("temp");
            Assert.True(pool.Return(sb));
            // 归还后缓冲应被清空
            Assert.Equal(0, sb.Length);
        }

        [Fact]
        public void StringBuilderPool_ReturnsFalse_ForOversizedCapacity()
        {
            var pool = new PoolUtil.StringBuilderPool { MaximumCapacity = 8 };
            var sb = new StringBuilder(new string('x', 100));
            Assert.False(pool.Return(sb));
        }
    }
}
