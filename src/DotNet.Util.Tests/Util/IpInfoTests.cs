using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// IpInfo 测试（纯逻辑 POCO）
    /// </summary>
    public class IpInfoTests
    {
        [Fact]
        public void Ctor_Default_AllNull()
        {
            var info = new IpInfo();

            Assert.Null(info.Province);
            Assert.Null(info.City);
            Assert.Null(info.Ip);
        }

        [Fact]
        public void Properties_AreSettable()
        {
            var info = new IpInfo
            {
                Province = "浙江",
                City = "杭州",
                Ip = "1.2.3.4"
            };

            Assert.Equal("浙江", info.Province);
            Assert.Equal("杭州", info.City);
            Assert.Equal("1.2.3.4", info.Ip);
        }
    }

    /// <summary>
    /// IpUtil 纯逻辑部分测试（不加载 17monipdb.dat 数据库文件）
    /// </summary>
    public class IpUtilPureTests
    {
        [Theory]
        [InlineData("192.168.1.1", true)]
        [InlineData("10.0.0.1", true)]
        [InlineData("172.16.0.1", true)]
        [InlineData("127.0.0.1", true)]
        [InlineData("8.8.8.8", false)]
        [InlineData("114.114.114.114", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsLocalIp_VariousInputs(string ip, bool expected)
        {
            Assert.Equal(expected, IpUtil.IsLocalIp(ip));
        }

        [Fact]
        public void IsLocalIp_WhiteListHit_ReturnsTrue()
        {
            var original = BaseSystemInfo.WhiteList;
            try
            {
                BaseSystemInfo.WhiteList = "1.2.3.4,5.6.7.8";
                Assert.True(IpUtil.IsLocalIp("5.6.7.8"));
                Assert.False(IpUtil.IsLocalIp("9.9.9.9"));
            }
            finally
            {
                BaseSystemInfo.WhiteList = original;
            }
        }
    }
}
