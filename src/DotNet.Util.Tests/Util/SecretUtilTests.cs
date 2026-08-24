using System.Text;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// SecretUtil 加密/哈希测试
    /// </summary>
    public class SecretUtilTests
    {
        static SecretUtilTests()
        {
#if NET8_0_OR_GREATER
            // .NET Core 需要注册 CodePagesEncodingProvider 才能使用 GBK 等代码页
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }

        [Fact]
        public void Md5_Returns32HexChars_Deterministic()
        {
            var h1 = SecretUtil.Md5("abc");
            var h2 = SecretUtil.Md5("abc");
            Assert.Equal(32, h1.Length);
            Assert.Equal(h1, h2);
            Assert.NotEqual(SecretUtil.Md5("abc"), SecretUtil.Md5("abd"));
        }

        [Fact]
        public void Md5_LengthVariant()
        {
            Assert.Equal(16, SecretUtil.Md5("abc", 16).Length);
            Assert.Equal(32, SecretUtil.Md5("abc", 32).Length);
        }

        [Fact]
        public void Aes_Roundtrip_DefaultKey()
        {
            var encrypted = SecretUtil.AesEncrypt("hello 中文");
            Assert.NotEqual("hello 中文", encrypted);
            Assert.Equal("hello 中文", SecretUtil.AesDecrypt(encrypted));
        }

        [Fact]
        public void Aes_Roundtrip_CustomKey()
        {
            const string key = "custom-key-123";
            var encrypted = SecretUtil.AesEncrypt("secret", key);
            Assert.Equal("secret", SecretUtil.AesDecrypt(encrypted, key));
        }

        [Fact]
        public void Aes_Encrypt_IsRandomized()
        {
            // 每次加密含随机 IV，两次结果不应相同
            Assert.NotEqual(SecretUtil.AesEncrypt("same"), SecretUtil.AesEncrypt("same"));
        }

        [Fact]
        public void Des_Roundtrip_DefaultKey()
        {
            var encrypted = SecretUtil.DesEncrypt("hello");
            Assert.Equal("hello", SecretUtil.DesDecrypt(encrypted));
        }

        [Fact]
        public void Base64_Roundtrip_Utf8()
        {
            const string text = "中文测试 123";
            var encoded = SecretUtil.EncodeBase64("UTF-8", text);
            Assert.Equal(text, SecretUtil.DecodeBase64("UTF-8", encoded));
        }

        [Fact]
        public void Base64_Roundtrip_Gbk()
        {
            const string text = "中文测试";
            var encoded = SecretUtil.EncodeBase64("GBK", text);
            Assert.Equal(text, SecretUtil.DecodeBase64("GBK", encoded));
        }

        [Fact]
        public void SqlSafe_EscapesSingleQuotes()
        {
            // SqlSafe 用 SQL 转义（' → ''）而非删除字符
            Assert.Equal("O''Brien", SecretUtil.SqlSafe("O'Brien"));
        }

        [Fact]
        public void IsSqlSafe_DetectsInjection()
        {
            Assert.False(SecretUtil.IsSqlSafe("1; DROP TABLE UserInfo"));
            Assert.True(SecretUtil.IsSqlSafe("abc123"));
        }
    }
}
