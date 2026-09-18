using System.Text;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// FileUtil 文件读写测试（临时目录）
    /// </summary>
    public class FileUtilTests : IDisposable
    {
        private readonly string _tempFile;

        public FileUtilTests()
        {
            _tempFile = Path.Combine(Path.GetTempPath(), "DotNetUtilTests_" + Guid.NewGuid().ToString("N") + ".tmp");
        }

        public void Dispose()
        {
            FileUtil.DeleteFile(_tempFile);
        }

        [Fact]
        public void SaveFile_GetFile_Roundtrip()
        {
            var data = Encoding.UTF8.GetBytes("hello binary");
            FileUtil.SaveFile(data, _tempFile);
            var read = FileUtil.GetFile(_tempFile);
            Assert.Equal(data, read);
        }

        [Fact]
        public void WriteBinaryFile_ReadBinaryFile_Roundtrip()
        {
            const string text = "中文文本 123";
            FileUtil.WriteBinaryFile(_tempFile, text);
            Assert.Equal(text, FileUtil.ReadBinaryFile(_tempFile));
        }

        [Fact]
        public void DeleteFile_RemovesFile()
        {
            FileUtil.WriteBinaryFile(_tempFile, "x");
            Assert.True(File.Exists(_tempFile));
            Assert.True(FileUtil.DeleteFile(_tempFile));
            Assert.False(File.Exists(_tempFile));
        }

        [Fact]
        public void GetFriendlyFileSize_ReturnsReadable()
        {
            var size = FileUtil.GetFriendlyFileSize(1024 * 1024);
            Assert.False(string.IsNullOrWhiteSpace(size));
            Assert.Contains("M", size);
        }
    }
}
