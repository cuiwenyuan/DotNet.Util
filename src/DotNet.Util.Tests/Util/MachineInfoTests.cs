using DotNet.Util;
using System.Collections.Generic;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// MachineInfo 测试
    /// 仅覆盖 net8.0 下可确定性执行、不依赖 WMI/不可移植成员的静态方法。
    /// </summary>
    public class MachineInfoTests
    {
        [Fact]
        public void GetIpAddressList_ReturnsList()
        {
            List<string> list = MachineInfo.GetIpAddressList();
            Assert.NotNull(list);
            // 若本机有 IPv4 地址，GetIpAddress 应等于列表首项
            if (list.Count > 0)
            {
                Assert.Equal(list[0], MachineInfo.GetIpAddress());
            }
        }

        [Fact]
        public void GetIpAddress_ReturnsString()
        {
            string ip = MachineInfo.GetIpAddress();
            Assert.NotNull(ip);
        }

        [Fact]
        public void GetMacAddress_ReturnsString()
        {
            string mac = MachineInfo.GetMacAddress();
            Assert.NotNull(mac);
        }
    }
}
