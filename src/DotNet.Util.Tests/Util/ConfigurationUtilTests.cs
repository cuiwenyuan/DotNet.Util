using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// ConfigurationUtil 测试（无配置文件冒烟）
    ///
    /// 说明：net8 下 ConfigurationManager.AppSettings 为只读（KeyValueInternalCollection），
    /// 测试进程无法注入配置值；而 Get*Config 各方法均为 null 判断（无配置时跳过赋值，不抛异常）。
    /// 因此这里验证：无配置环境下 AppSettings 空 key 返回空串、GetConfig 及各子方法可安全调用。
    /// </summary>
    public class ConfigurationUtilTests
    {
        [Fact]
        public void AppSettings_MissingKey_ReturnsEmpty()
        {
            // 使用一个几乎不可能存在的 key
            var value = ConfigurationUtil.AppSettings("___no_such_key___", false);

            Assert.Equal(string.Empty, value);
        }

        [Fact]
        public void AppSettings_MissingKey_Encrypt_ReturnsEmpty()
        {
            // 无值时即使 encrypt=true 也不进入解密分支
            var value = ConfigurationUtil.AppSettings("___no_such_key___", true);

            Assert.Equal(string.Empty, value);
        }

        [Fact]
        public void GetConfig_NoConfig_DoesNotThrow()
        {
            var ex = Record.Exception(() => ConfigurationUtil.GetConfig());
            Assert.Null(ex);
        }

        [Fact]
        public void GetDatabaseConfig_NoConfig_DoesNotThrow()
        {
            var ex = Record.Exception(() => ConfigurationUtil.GetDatabaseConfig());
            Assert.Null(ex);
        }

        [Fact]
        public void GetMailConfig_NoConfig_DoesNotThrow()
        {
            var ex = Record.Exception(() => ConfigurationUtil.GetMailConfig());
            Assert.Null(ex);
        }

        [Fact]
        public void GetWebAppConfig_NoConfig_DoesNotThrow()
        {
            var ex = Record.Exception(() => ConfigurationUtil.GetWebAppConfig());
            Assert.Null(ex);
        }

        [Fact]
        public void GetRedisConfig_NoConfig_DoesNotThrow()
        {
            var ex = Record.Exception(() => ConfigurationUtil.GetRedisConfig());
            Assert.Null(ex);
        }

        [Fact]
        public void GetFtpConfig_NoConfig_DoesNotThrow()
        {
            var ex = Record.Exception(() => ConfigurationUtil.GetFtpConfig());
            Assert.Null(ex);
        }

        [Fact]
        public void GetUserConfig_NoConfig_DoesNotThrow()
        {
            var ex = Record.Exception(() => ConfigurationUtil.GetUserConfig());
            Assert.Null(ex);
        }

        [Fact]
        public void GetCookieConfig_NoConfig_DoesNotThrow()
        {
            var ex = Record.Exception(() => ConfigurationUtil.GetCookieConfig());
            Assert.Null(ex);
        }

        [Fact]
        public void GetMqttConfig_NoConfig_DoesNotThrow()
        {
            var ex = Record.Exception(() => ConfigurationUtil.GetMqttConfig());
            Assert.Null(ex);
        }

        [Fact]
        public void GetWebApiConfig_NoConfig_DoesNotThrow()
        {
            var ex = Record.Exception(() => ConfigurationUtil.GetWebApiConfig());
            Assert.Null(ex);
        }
    }
}
