using System;
using System.IO;
using System.Text;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Plus
{
    /// <summary>
    /// QqwryUtil 集成测试（B/C 类硬外部依赖）。
    /// 默认不通过：未设置环境变量 DUP_TEST_QQWRY（qqwry.dat 文件完整路径）时，所有用例以 Assert.Fail 明确提示。
    /// 启用方式（提供纯真 IP 库文件后重跑）：
    ///   set DUP_TEST_QQWRY=D:\data\qqwry.dat
    /// QqwryUtil.GetLocation 内部固定加载 Utils.GetMapPath("/plus/qqwry.dat")，测试会把提供的 dat 复制到该解析路径。
    /// </summary>
    public class QqwryUtilIntegrationTests
    {
        [Fact]
        public void GetLocation_KnownIp_ReturnsCountry()
        {
            var src = Environment.GetEnvironmentVariable("DUP_TEST_QQWRY");
            if (string.IsNullOrWhiteSpace(src))
            {
                Assert.Fail("QqwryUtil 集成测试未启用：请设置环境变量 DUP_TEST_QQWRY=qqwry.dat 的完整路径后重跑。默认不通过。");
            }
            if (!File.Exists(src))
            {
                Assert.Fail($"QqwryUtil 集成测试：指定的 qqwry.dat 不存在：{src}");
            }

            // 纯真库地区名为 GBK，确保 CodePages 编码提供程序已注册（库本身也可能已注册）
            try { Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); } catch { }

            // 复制到 QqwryUtil 实际加载的路径（与 GetLocation 内部调用保持一致）
            var target = Utils.GetMapPath("/plus/qqwry.dat");
            Directory.CreateDirectory(Path.GetDirectoryName(target));
            File.Copy(src, target, overwrite: true);

            var loc = QqwryUtil.GetLocation("8.8.8.8");
            Assert.NotNull(loc);
            Assert.False(string.IsNullOrEmpty(loc.Country), "解析出的国家名不应为空");
        }

        [Fact]
        public void GetLocation_InvalidIp_Throws()
        {
            var src = Environment.GetEnvironmentVariable("DUP_TEST_QQWRY");
            if (string.IsNullOrWhiteSpace(src))
            {
                Assert.Fail("QqwryUtil 集成测试未启用：请设置环境变量 DUP_TEST_QQWRY=qqwry.dat 的完整路径后重跑。默认不通过。");
            }
            if (!File.Exists(src))
            {
                Assert.Fail($"QqwryUtil 集成测试：指定的 qqwry.dat 不存在：{src}");
            }

            try { Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); } catch { }

            var target = Utils.GetMapPath("/plus/qqwry.dat");
            Directory.CreateDirectory(Path.GetDirectoryName(target));
            File.Copy(src, target, overwrite: true);

            Assert.Throws<Exception>(() => QqwryUtil.GetLocation("not-a-valid-ip"));
        }
    }
}
