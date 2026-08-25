using System;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Configuration
{
    /// <summary>
    /// RegistryUtil 集成测试（B/C 类硬外部依赖）。
    /// 注册表读写逻辑仅在 net48（NET46_OR_GREATER）编译分支中实现；net8.0 下 GetValue/SetValue 为 no-op，
    /// 因此 net8.0 直接 Skip，真实校验需在 net48 TFM 下运行。
    /// 默认不通过（net48）：未设置环境变量 DUP_TEST_REGISTRY=1 时以 Assert.Fail 明确提示。
    /// 启用前提：写入 HKLM\Software\DotNet 需要管理员权限（RegistryUtil 固定使用 LocalMachine，不支持 HKCU）。
    ///   set DUP_TEST_REGISTRY=1
    /// </summary>
    public class RegistryUtilIntegrationTests
    {
#if NET46_OR_GREATER
        [Fact]
        public void SetGet_Value_RoundTrips()
        {
            var enabled = Environment.GetEnvironmentVariable("DUP_TEST_REGISTRY");
            if (string.IsNullOrWhiteSpace(enabled))
            {
                Assert.Fail("RegistryUtil 集成测试未启用：请设置环境变量 DUP_TEST_REGISTRY=1 后重跑（需管理员权限写入 HKLM）。默认不通过。");
            }

            var key = "DUP_TestKey_" + Guid.NewGuid().ToString("N");
            try
            {
                RegistryUtil.SetValue(key, "hello");
                Assert.Equal("hello", RegistryUtil.GetValue(key));
                Assert.True(RegistryUtil.Exists(key));
            }
            finally
            {
                // 清理：删除临时写入的值（需管理员权限）
                try
                {
                    using var rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(RegistryUtil.SubKey, true);
                    rk?.DeleteValue(key, false);
                }
                catch { /* 清理失败不影响断言结果 */ }
            }
        }
#else
        [Fact(Skip = "RegistryUtil 注册表代码仅在 net48 (NET46_OR_GREATER) 编译；net8.0 下为 no-op，集成测试需在 net48 TFM 运行。")]
        public void SetGet_Value_RoundTrips()
        {
        }
#endif
    }
}
