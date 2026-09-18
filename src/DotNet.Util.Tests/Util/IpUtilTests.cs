using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// IpUtil 测试（仅覆盖确定性、无外部依赖的静态方法 IsLocalIp）
    /// FindIp/FindName 依赖 17monipdb.dat 二进制库，不在此测试。
    /// </summary>
    public class IpUtilTests
    {
        [Theory]
        [InlineData("192.168.1.1")]
        [InlineData("172.16.0.1")]
        [InlineData("10.0.0.1")]
        [InlineData("127.0.0.1")]
        public void IsLocalIp_PrivateAndLoopback_ReturnsTrue(string ip)
        {
            Assert.True(IpUtil.IsLocalIp(ip));
        }

        [Theory]
        [InlineData("8.8.8.8")]
        [InlineData("1.1.1.1")]
        public void IsLocalIp_Public_ReturnsFalse(string ip)
        {
            Assert.False(IpUtil.IsLocalIp(ip));
        }

        [Fact]
        public void IsLocalIp_NullOrEmpty_ReturnsFalse()
        {
            Assert.False(IpUtil.IsLocalIp(null));
            Assert.False(IpUtil.IsLocalIp(""));
        }
    }
}
