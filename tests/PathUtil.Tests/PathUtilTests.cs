using System;
using System.IO;
using Xunit;
using DotNet.Util;

namespace PathUtil.Tests
{
    public class PathUtilTests : IDisposable
    {
        private readonly string _tempDir;

        public PathUtilTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "DotNetUtil_Test_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(_tempDir)) Directory.Delete(_tempDir, true);
            }
            catch { }
        }

        [Fact]
        public void CombinePath_ShouldHandleNullAndEmptyParts()
        {
            var basePath = _tempDir;
            var p = basePath.CombinePath(null, "sub", "", "file.txt");
            Assert.Contains("sub", p);
            Assert.EndsWith("file.txt", p.Replace("\\", "/"));
        }

        [Fact]
        public void EnsureDirectory_ShouldCreateDirectory_WhenFilePathProvided()
        {
            var file = Path.Combine(_tempDir, "a", "b", "test.txt");
            var dir = file.EnsureDirectory(isfile: true);
            Assert.True(Directory.Exists(dir));
            Assert.Equal(dir, Path.GetDirectoryName(file).GetFullPath());
        }

        [Fact]
        public void CopyToIfNewer_ShouldCopyOnlyWhenSourceIsNewer()
        {
            var src = Path.Combine(_tempDir, "src.txt");
            var destDir = Path.Combine(_tempDir, "dest");
            Directory.CreateDirectory(destDir);
            var dest = Path.Combine(destDir, "src.txt");

            File.WriteAllText(src, "v1");
            File.WriteAllText(dest, "v0");

            // set dest newer
            File.SetLastWriteTimeUtc(dest, DateTime.UtcNow.AddMinutes(1));
            File.SetLastWriteTimeUtc(src, DateTime.UtcNow);

            var fi = new FileInfo(src);
            var copied = fi.CopyToIfNewer(dest);
            Assert.False(copied);

            // make src newer
            File.SetLastWriteTimeUtc(src, DateTime.UtcNow.AddMinutes(2));
            copied = fi.CopyToIfNewer(dest);
            Assert.True(copied);
            Assert.Equal(File.ReadAllText(src), File.ReadAllText(dest));
        }

        [Fact]
        public void AsFile_ReadWriteBytes_OpenReadOpenWrite()
        {
            var file = Path.Combine(_tempDir, "data.bin").GetFullPath();
            var fi = file.AsFile();
            var data = new byte[] { 1, 2, 3, 4, 5 };
            fi.WriteBytes(data, 0);
            Assert.True(File.Exists(fi.FullName));
            var read = fi.ReadBytes(0, -1);
            Assert.Equal(data.Length, read.Length);

            // test OpenRead/OpenWrite (non-compressed)
            fi.OpenWrite(false, s =>
            {
                s.Write(new byte[] { 9, 9, 9 }, 0, 3);
            });
            // reopen and read tail (simple smoke test)
            fi.OpenRead(false, s =>
            {
                Assert.True(s.CanRead);
            });
        }
    }
}
