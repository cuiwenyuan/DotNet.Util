using System;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Cache
{
    /// <summary>
    /// RedisUtil 集成测试（B/C 类硬外部依赖）。
    /// 默认不通过：未设置环境变量 DUP_TEST_REDIS 时，所有用例以 Assert.Fail 明确提示。
    /// 启用方式（提供 Redis 连接信息后重跑）：
    ///   set DUP_TEST_REDIS=127.0.0.1:6379
    ///   set DUP_TEST_REDIS=password@127.0.0.1:6379
    ///   set DUP_TEST_REDIS=user:password@127.0.0.1:6379
    /// 注意：RemoveAll / RemoveByRegex 会清空或批量删除真实库，默认不纳入集成测试，避免误伤共享 Redis。
    /// </summary>
    public class RedisUtilIntegrationTests
    {
        private static (string Host, int Port, string User, string Password) ParseRedis(string raw)
        {
            var user = string.Empty;
            var password = string.Empty;
            var hostport = raw;
            var at = raw.LastIndexOf('@');
            if (at >= 0)
            {
                var creds = raw.Substring(0, at);
                hostport = raw.Substring(at + 1);
                var c = creds.Split(':');
                if (c.Length == 2) { user = c[0]; password = c[1]; }
                else { password = creds; }
            }
            var hp = hostport.Split(':');
            if (hp.Length != 2)
            {
                throw new FormatException("DUP_TEST_REDIS 格式应为 host:port 或 [user:]password@host:port");
            }
            return (hp[0], int.Parse(hp[1]), user, password);
        }

        private static void ConfigureFromEnv()
        {
            var raw = Environment.GetEnvironmentVariable("DUP_TEST_REDIS");
            if (string.IsNullOrWhiteSpace(raw))
            {
                Assert.Fail("RedisUtil 集成测试未启用：请设置环境变量 DUP_TEST_REDIS=host:port（可选 [user:]password@host:port）后重跑。默认不通过。");
            }
            var (host, port, user, password) = ParseRedis(raw);
            // RedisUtil 的 FullRedis 单例在首次访问静态成员时根据 BaseSystemInfo 初始化，必须在调用前赋值
            BaseSystemInfo.RedisServer = host;
            BaseSystemInfo.RedisPort = port;
            BaseSystemInfo.RedisUserName = user;
            BaseSystemInfo.RedisPassword = password;
            BaseSystemInfo.RedisInitialDb = 0;
        }

        [Fact]
        public void SetGet_String_RoundTrips()
        {
            ConfigureFromEnv();
            var key = "dup_itest_" + Guid.NewGuid().ToString("N");
            RedisUtil.Set(key, "hello", 60);
            Assert.Equal("hello", RedisUtil.Get<string>(key));
            Assert.True(RedisUtil.Contains(key));
            RedisUtil.Remove(key);
            Assert.False(RedisUtil.Contains(key));
        }

        [Fact]
        public void Add_Generic_RoundTrips()
        {
            ConfigureFromEnv();
            var key = "dup_itest_" + Guid.NewGuid().ToString("N");
            RedisUtil.Add(key, 123, 60);
            Assert.Equal(123, RedisUtil.Get<int>(key));
            RedisUtil.Remove(key);
        }

        [Fact]
        public void GetAllKeys_ReturnsArray()
        {
            ConfigureFromEnv();
            var keys = RedisUtil.GetAllKeys();
            Assert.NotNull(keys);
            // 仅验证非破坏性调用可返回集合，不校验内容
        }
    }
}
