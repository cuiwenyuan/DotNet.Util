using System.Text;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// SecurityUtil（NewLife 哈希/加密包装）测试
    /// </summary>
    public class SecurityUtilTests
    {
        [Fact]
        public void MD5_String_KnownVector()
        {
            // "abc" 的标准 MD5，NewLife 返回大写十六进制
            Assert.Equal("900150983CD24FB0D6963F7D28E17F72", "abc".MD5());
        }

        [Fact]
        public void MD5_16_KnownVector()
        {
            // NewLife 的 MD5_16 取全串前 16 位
            Assert.Equal("900150983CD24FB0", "abc".MD5_16());
        }

        [Fact]
        public void MD5_Bytes_Returns16Bytes()
        {
            Assert.Equal(16, Encoding.UTF8.GetBytes("abc").MD5().Length);
        }

        [Fact]
        public void SHA1_Keyed_Deterministic20Bytes()
        {
            var data = Encoding.UTF8.GetBytes("abc");
            var key = Encoding.UTF8.GetBytes("secret");
            var h1 = data.SHA1(key);
            var h2 = data.SHA1(key);
            Assert.Equal(20, h1.Length);
            Assert.Equal(h1, h2);
        }

        [Fact]
        public void SHA256_Keyed_Deterministic32Bytes()
        {
            var data = Encoding.UTF8.GetBytes("abc");
            var key = Encoding.UTF8.GetBytes("secret");
            var h1 = data.SHA256(key);
            var h2 = data.SHA256(key);
            Assert.Equal(32, h1.Length);
            Assert.Equal(h1, h2);
        }

        [Fact]
        public void SHA512_Keyed_Deterministic64Bytes()
        {
            var data = Encoding.UTF8.GetBytes("abc");
            var key = Encoding.UTF8.GetBytes("secret");
            Assert.Equal(64, data.SHA512(key).Length);
        }

        [Fact]
        public void Crc_ReturnsUInt32()
        {
            var crc = Encoding.UTF8.GetBytes("abc").Crc();
            Assert.True(crc > 0);
        }

        [Fact]
        public void Crc16_ReturnsUInt16()
        {
            var crc = Encoding.UTF8.GetBytes("abc").Crc16();
            Assert.True(crc > 0);
        }
    }
}
