using System;
using System.IO;
using System.Text;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Plus
{
    /// <summary>
    /// BaiduOcrUtil 纯文件逻辑测试（GetFileBase64，不触网）
    /// </summary>
    public class BaiduOcrUtilTests : IDisposable
    {
        private readonly string _tempDir;

        public BaiduOcrUtilTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "DotNetUtilBaidu_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            try { Directory.Delete(_tempDir, true); } catch { }
        }

        [Fact]
        public void GetFileBase64_ReturnsBase64()
        {
            var path = Path.Combine(_tempDir, "pic.bin");
            File.WriteAllBytes(path, new byte[] { 1, 2, 3, 4 });

            var result = BaiduOcrUtil.GetFileBase64(path);

            Assert.Equal(Convert.ToBase64String(new byte[] { 1, 2, 3, 4 }), result);
        }

        [Fact]
        public void GetFileBase64_TextFile_RoundTrips()
        {
            var path = Path.Combine(_tempDir, "text.txt");
            File.WriteAllText(path, "Hello 中文", Encoding.UTF8);

            var base64 = BaiduOcrUtil.GetFileBase64(path);
            var bytes = Convert.FromBase64String(base64);

            // File.WriteAllText 默认带 BOM，去掉后比对内容
            var text = Encoding.UTF8.GetString(bytes).Trim('\uFEFF');
            Assert.Equal("Hello 中文", text);
        }

        [Fact]
        public void GetFileBase64_MissingFile_Throws()
        {
            Assert.ThrowsAny<Exception>(() => BaiduOcrUtil.GetFileBase64(Path.Combine(_tempDir, "nope.png")));
        }
    }
}
