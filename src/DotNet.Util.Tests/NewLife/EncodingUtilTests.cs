using System.Text;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.NewLife
{
    /// <summary>
    /// EncodingUtil 编码探测测试
    /// </summary>
    public class EncodingUtilTests
    {
        [Fact]
        public void DetectBOM_Utf8()
        {
            var bom = Encoding.UTF8.GetPreamble(); // EF BB BF
            var encoding = EncodingUtil.DetectBOM(bom);
            Assert.NotNull(encoding);
            Assert.Equal("utf-8", encoding!.WebName);
        }

        [Fact]
        public void DetectBOM_Utf16LittleEndian()
        {
            var bom = Encoding.Unicode.GetPreamble(); // FF FE
            var encoding = EncodingUtil.DetectBOM(bom);
            Assert.NotNull(encoding);
            Assert.Equal("utf-16", encoding!.WebName);
        }

        [Fact]
        public void DetectBOM_Utf32LittleEndian()
        {
            var bom = Encoding.UTF32.GetPreamble(); // FF FE 00 00
            var encoding = EncodingUtil.DetectBOM(bom);
            Assert.NotNull(encoding);
            Assert.Equal("utf-32", encoding!.WebName);
        }

        [Fact]
        public void Detect_Utf8Bytes_NoBom_ReturnsUtf8()
        {
            var data = Encoding.UTF8.GetBytes("hello world");
            var encoding = EncodingUtil.Detect(data);
            Assert.NotNull(encoding);
            Assert.Equal("utf-8", encoding!.WebName);
        }

        [Fact]
        public void Detect_AsciiBytes_ReturnsAsciiOrUtf8()
        {
            var data = Encoding.ASCII.GetBytes("plain ascii");
            var encoding = EncodingUtil.Detect(data);
            Assert.NotNull(encoding);
            // ASCII 也是合法 UTF-8，探测可能返回 ascii 或 utf-8，两者皆可
            Assert.Contains(encoding!.WebName, new[] { "us-ascii", "utf-8" });
        }
    }
}
