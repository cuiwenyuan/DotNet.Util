using System.IO.Compression;
using System.Text;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Plus
{
    /// <summary>
    /// ZipUtil 压缩测试：CreateZip → 标准 ZipArchive 解压校验
    /// </summary>
    public class ZipUtilTests : IDisposable
    {
        private readonly string _tempDir;

        public ZipUtilTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "DotNetUtilZip_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            try { Directory.Delete(_tempDir, true); } catch { }
        }

        [Fact]
        public void CreateZip_ProducesValidZip()
        {
            var f1 = Path.Combine(_tempDir, "a.txt");
            var f2 = Path.Combine(_tempDir, "b.txt");
            File.WriteAllText(f1, "content-a", Encoding.UTF8);
            File.WriteAllText(f2, "content-b", Encoding.UTF8);

            var zipPath = Path.Combine(_tempDir, "out.zip");
            ZipUtil.CreateZip(new[] { f1, f2 }, zipPath);

            Assert.True(File.Exists(zipPath));
            using var archive = ZipFile.OpenRead(zipPath);
            Assert.Equal(2, archive.Entries.Count);
            var names = archive.Entries.Select(e => e.Name).ToHashSet();
            Assert.Contains("a.txt", names);
            Assert.Contains("b.txt", names);
        }

        [Fact]
        public void CreateZip_WithSkipFileExtensions()
        {
            var f1 = Path.Combine(_tempDir, "keep.txt");
            var f2 = Path.Combine(_tempDir, "skip.log");
            File.WriteAllText(f1, "keep");
            File.WriteAllText(f2, "skip");

            var zipPath = Path.Combine(_tempDir, "out2.zip");
            ZipUtil.CreateZip(new[] { f1, f2 }, zipPath, skipFileExtensions: new[] { ".log" });

            using var archive = ZipFile.OpenRead(zipPath);
            var names = archive.Entries.Select(e => e.Name).ToHashSet();
            Assert.Contains("keep.txt", names);
            Assert.DoesNotContain("skip.log", names);
        }
    }
}
