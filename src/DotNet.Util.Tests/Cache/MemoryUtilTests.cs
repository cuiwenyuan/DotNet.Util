using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Cache
{
    /// <summary>
    /// MemoryUtil（Microsoft.Extensions MemoryCache 封装）测试
    /// </summary>
    public class MemoryUtilTests
    {
        private static string NewKey() => "test:" + Guid.NewGuid().ToString("N");

        [Fact]
        public void Set_Get_Contains_Remove()
        {
            var key = NewKey();
            MemoryUtil.Set(key, "hello");
            Assert.True(MemoryUtil.Contains(key));
            Assert.Equal("hello", MemoryUtil.Get<string>(key));
            Assert.True(MemoryUtil.Remove(key));
            Assert.False(MemoryUtil.Contains(key));
        }

        [Fact]
        public void Get_Missing_ReturnsDefault()
        {
            Assert.Null(MemoryUtil.Get<string>(NewKey()));
        }

        [Fact]
        public void Get_Generic_StoresTyped()
        {
            var key = NewKey();
            var entity = new DemoEntity { Id = 42, Name = "Troy" };
            MemoryUtil.Set(key, entity);
            var actual = MemoryUtil.Get<DemoEntity>(key);
            Assert.NotNull(actual);
            Assert.Equal(42, actual!.Id);
            Assert.Equal("Troy", actual.Name);
        }

        [Fact]
        public void Set_SlidingExpiration_Expires()
        {
            var key = NewKey();
            MemoryUtil.Set(key, "v", TimeSpan.FromMilliseconds(100));
            Assert.Equal("v", MemoryUtil.Get<string>(key));
            Thread.Sleep(500);
            Assert.Null(MemoryUtil.Get<string>(key));
        }

        [Fact]
        public void Set_AbsoluteExpiration_Expires()
        {
            var key = NewKey();
            MemoryUtil.Set(key, "v", DateTime.Now.AddMilliseconds(100));
            Thread.Sleep(500);
            Assert.Null(MemoryUtil.Get<string>(key));
        }

        [Fact]
        public void Set_Overwrite_UpdatesValue()
        {
            var key = NewKey();
            MemoryUtil.Set(key, "v1");
            MemoryUtil.Set(key, "v2");
            Assert.Equal("v2", MemoryUtil.Get<string>(key));
        }

        [Fact]
        public void RemoveAll_ClearsAll()
        {
            var k1 = NewKey();
            var k2 = NewKey();
            MemoryUtil.Set(k1, "a");
            MemoryUtil.Set(k2, "b");
            MemoryUtil.RemoveAll();
            Assert.False(MemoryUtil.Contains(k1));
            Assert.False(MemoryUtil.Contains(k2));
        }

        private sealed class DemoEntity
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }
    }
}
