using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Cache
{
    /// <summary>
    /// CacheUtil 测试（内存缓存路径）。
    /// 说明：BaseSystemInfo.RedisEnabled 默认为 false，故所有方法均走 MemoryUtil，无需真实 Redis。
    /// </summary>
    public class CacheUtilTests
    {
        private static string NewKey() => "cacheutil:" + Guid.NewGuid().ToString("N");

        [Fact]
        public void Set_Get_Contains_Remove_Memory()
        {
            var key = NewKey();
            Assert.True(CacheUtil.Set(key, "hello"));
            Assert.True(CacheUtil.Contains(key));
            Assert.Equal("hello", CacheUtil.Get<string>(key));
            Assert.True(CacheUtil.Remove(key));
            Assert.False(CacheUtil.Contains(key));
        }

        [Fact]
        public void Get_Missing_ReturnsDefault()
        {
            Assert.Null(CacheUtil.Get<string>(NewKey()));
        }

        [Fact]
        public void Get_Generic_StoresTyped()
        {
            var key = NewKey();
            var entity = new DemoEntity { Id = 42, Name = "Troy" };
            CacheUtil.Set(key, entity);
            var actual = CacheUtil.Get<DemoEntity>(key);
            Assert.NotNull(actual);
            Assert.Equal(42, actual!.Id);
            Assert.Equal("Troy", actual.Name);
        }

        [Fact]
        public void Cache_Delegate_CachesResult()
        {
            var key = NewKey();
            var calls = 0;
            Func<int> proc = () => { calls++; return 7; };

            var first = CacheUtil.Cache(key, proc, cacheTime: TimeSpan.FromSeconds(30));
            var second = CacheUtil.Cache(key, proc, cacheTime: TimeSpan.FromSeconds(30));

            Assert.Equal(7, first);
            Assert.Equal(7, second);
            // 第二次从缓存返回，委托只被调用一次
            Assert.Equal(1, calls);
        }

        [Fact]
        public void Cache_RefreshCache_ForcesRecompute()
        {
            var key = NewKey();
            var calls = 0;
            Func<int> proc = () => { calls++; return calls; };

            CacheUtil.Cache(key, proc, refreshCache: false, cacheTime: TimeSpan.FromSeconds(30));
            CacheUtil.Cache(key, proc, refreshCache: true, cacheTime: TimeSpan.FromSeconds(30));

            // 强制刷新会重新执行委托
            Assert.Equal(2, calls);
        }

        [Fact]
        public void RemoveByRegex_RemovesMatchingKeys()
        {
            var prefix = "cacheutil:regex:" + Guid.NewGuid().ToString("N");
            var k1 = prefix + "_a";
            var k2 = prefix + "_b";
            CacheUtil.Set(k1, "1");
            CacheUtil.Set(k2, "2");
            CacheUtil.RemoveByRegex("^" + prefix + "_.*$");
            Assert.False(CacheUtil.Contains(k1));
            Assert.False(CacheUtil.Contains(k2));
        }

        [Fact]
        public void RemoveAllCache_ClearsAll()
        {
            var k1 = NewKey();
            var k2 = NewKey();
            CacheUtil.Set(k1, "a");
            CacheUtil.Set(k2, "b");
            CacheUtil.RemoveAllCache();
            Assert.False(CacheUtil.Contains(k1));
            Assert.False(CacheUtil.Contains(k2));
        }

        private sealed class DemoEntity
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }
    }
}
