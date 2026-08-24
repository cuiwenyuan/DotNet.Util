using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Cache
{
    /// <summary>
    /// NewLifeUtil 测试（纯属性：默认 null，不触发 Redis 连接）
    /// </summary>
    public class NewLifeUtilTests
    {
        [Fact]
        public void Properties_Default_AreNull()
        {
            // 静态属性未初始化时默认 null（访问不会触发 Redis 连接）
            Assert.Null(NewLifeUtil.MemoryCache);
            Assert.Null(NewLifeUtil.Redis);
            Assert.Null(NewLifeUtil.FullRedis);
        }
    }
}
