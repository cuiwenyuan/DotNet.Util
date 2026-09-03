using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
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

        [Theory]
        [InlineData(0)]
        [InlineData(8)]
        [InlineData(20)]
        [InlineData(31)]
        [InlineData(64)]
        [InlineData(-1)]
        public void Md5_InvalidLength_Throws(int length)
        {
            // 修复：原实现仅 length==16 生效，其余任意值一律静默返回 32 位，
            // 调用方按期望长度截断/比对会静默出错。改为非法值快速失败。
            Assert.Throws<ArgumentOutOfRangeException>(() => SecretUtil.Md5("abc", length));
        }

        [Fact]
        public void Md5_16Bit_EqualsMiddleOf32Bit()
        {
            // 16 位应取自 32 位结果的第 9~24 个字符（Substring(8,16)）
            var full = SecretUtil.Md5("abc", 32);
            Assert.Equal(full.Substring(8, 16), SecretUtil.Md5("abc", 16));
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
        public void Aes_TamperedCiphertext_FailsIntegrityCheck()
        {
            // R9-10：加密输出已追加 HMAC，篡改任何字节都应导致解密失败（返回空串）
            var encrypted = SecretUtil.AesEncrypt("敏感数据 payload");
            var raw = Convert.FromBase64String(encrypted);
            // 翻转最后一个字节（位于 HMAC 区），破坏完整性
            raw[raw.Length - 1] ^= 0xFF;
            var tampered = Convert.ToBase64String(raw);
            Assert.Equal(string.Empty, SecretUtil.AesDecrypt(tampered));
        }

        [Fact]
        public void Aes_DetectsTamperedIv()
        {
            // R9-10：篡改 IV 同样破坏 HMAC，解密应失败
            var encrypted = SecretUtil.AesEncrypt("secret-content");
            var raw = Convert.FromBase64String(encrypted);
            raw[0] ^= 0xFF; // 破坏 IV 首字节
            var tampered = Convert.ToBase64String(raw);
            Assert.Equal(string.Empty, SecretUtil.AesDecrypt(tampered));
        }

        #region R9-13 RSA 签名须用 UTF-8 处理非 ASCII 数据
        [Fact]
        public void SignData_VerifyData_Roundtrip_NonAscii()
        {
            // RSACryptoServiceProvider/ImportCspBlob 仅 Windows 支持；其余平台跳过
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;
            using var rsa = new RSACryptoServiceProvider(2048);
            var privateBlob = Convert.ToBase64String(rsa.ExportCspBlob(true));
            var publicBlob = Convert.ToBase64String(rsa.ExportCspBlob(false));
            const string data = "中文签名数据 ★ こんにちは";
            var sign = SecretUtil.SignData(data, privateBlob);
            Assert.False(string.IsNullOrEmpty(sign));
            Assert.True(SecretUtil.VerifyData(data, sign, publicBlob));
            // 数据被篡改后验签应失败
            Assert.False(SecretUtil.VerifyData(data + "x", sign, publicBlob));
        }

        [Fact]
        public void SignData_SignsOverUtf8Bytes_NotAsciiFolded()
        {
            // R9-13：独立用底层 RSA 按 UTF-8 字节校验；若 SignData 仍用 ASCII，非 ASCII 会被折叠为 '?'，验签失败
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;
            using var rsa = new RSACryptoServiceProvider(2048);
            var privateBlob = Convert.ToBase64String(rsa.ExportCspBlob(true));
            var publicBlob = Convert.ToBase64String(rsa.ExportCspBlob(false));
            const string data = "中文签名数据 ★";
            var sign = SecretUtil.SignData(data, privateBlob);
            using var verifyRsa = new RSACryptoServiceProvider();
            verifyRsa.ImportCspBlob(Convert.FromBase64String(publicBlob));
            var utf8Bytes = Encoding.UTF8.GetBytes(data);
            using var sha256 = SHA256.Create();
            Assert.True(verifyRsa.VerifyData(utf8Bytes, sha256, Convert.FromBase64String(sign)));
        }
        #endregion

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

        #region R9-14 Base64 编解码不得静默吞异常（须正确失败）
        [Fact]
        public void DecodeBase64_InvalidInput_ThrowsInsteadOfReturningInput()
        {
            // 修复前 catch{} 会静默返回原值，调用方无法察觉解码失败
            Assert.ThrowsAny<Exception>(() => SecretUtil.DecodeBase64("utf-8", "!!!not_valid_base64!!!"));
        }

        [Fact]
        public void EncodeBase64_InvalidEncoding_Throws()
        {
            // 无效代码页不应静默返回原值
            Assert.ThrowsAny<Exception>(() => SecretUtil.EncodeBase64("__no_such_codepage__", "abc"));
        }

        [Fact]
        public void DecodeBase64_InvalidEncoding_Throws()
        {
            var encoded = SecretUtil.EncodeBase64("utf-8", "hello");
            Assert.ThrowsAny<Exception>(() => SecretUtil.DecodeBase64("__no_such_codepage__", encoded));
        }
        #endregion

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

        #region R9-1 PBKDF2 口令哈希（替代无盐/快速 MD5）
        [Fact]
        public void HashPassword_ReturnsPbkdf2Format()
        {
            var hash = SecretUtil.HashPassword("P@ssw0rd");
            Assert.StartsWith(SecretUtil.PasswordHashPrefix, hash);
            // 格式：pbkdf2$iters$saltB64$hashB64 → 4 段
            var parts = hash.Split('$');
            Assert.Equal(4, parts.Length);
            Assert.True(int.Parse(parts[1]) > 0);
            Assert.False(string.IsNullOrEmpty(parts[2]));
            Assert.False(string.IsNullOrEmpty(parts[3]));
        }

        [Fact]
        public void HashPassword_VerifyPassword_Roundtrip()
        {
            var hash = SecretUtil.HashPassword("P@ssw0rd");
            Assert.True(SecretUtil.VerifyPassword("P@ssw0rd", hash));
            Assert.False(SecretUtil.VerifyPassword("wrong", hash));
        }

        [Fact]
        public void HashPassword_IsSalted_Randomized()
        {
            // 相同口令两次哈希结果不同（盐随机），但都能校验通过
            var h1 = SecretUtil.HashPassword("P@ssw0rd");
            var h2 = SecretUtil.HashPassword("P@ssw0rd");
            Assert.NotEqual(h1, h2);
            Assert.True(SecretUtil.VerifyPassword("P@ssw0rd", h1));
            Assert.True(SecretUtil.VerifyPassword("P@ssw0rd", h2));
        }

        [Fact]
        public void VerifyPassword_TamperedHash_ReturnsFalse()
        {
            var hash = SecretUtil.HashPassword("P@ssw0rd");
            // 篡改哈希部分
            var tampered = hash.Substring(0, hash.Length - 2) + (hash[hash.Length - 1] == 'A' ? "BB" : "AA");
            Assert.False(SecretUtil.VerifyPassword("P@ssw0rd", tampered));
        }

        [Fact]
        public void VerifyPassword_NullOrEmpty_ReturnsFalse()
        {
            Assert.False(SecretUtil.VerifyPassword(null, "pbkdf2$100000$abc==$def=="));
            Assert.False(SecretUtil.VerifyPassword("x", null));
            Assert.False(SecretUtil.VerifyPassword("x", "not-a-hash"));
            Assert.False(SecretUtil.VerifyPassword("x", "pbkdf2$notanint$abc==$def=="));
        }

        [Fact]
        public void HashPassword_NullPassword_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => SecretUtil.HashPassword(null));
        }

        [Fact]
        public void HashPassword_CustomIterations_StillVerifies()
        {
            // 迭代次数写入哈希，VerifyPassword 能自适应
            var hash = SecretUtil.HashPassword("P@ssw0rd", 200000);
            Assert.True(SecretUtil.VerifyPassword("P@ssw0rd", hash));
        }
        #endregion
    }
}
