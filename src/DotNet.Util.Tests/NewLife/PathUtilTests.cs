using System;
using System.IO;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.NewLife
{
    /// <summary>
    /// PathUtil 路径处理纯逻辑测试（不依赖外部服务）。
    /// </summary>
    public class PathUtilTests
    {
        [Fact]
        public void CombinePath_JoinsSegments()
        {
            var result = "base".CombinePath("a", "b", "c");
            Assert.NotNull(result);
            Assert.Contains("a", result!);
            Assert.Contains("c", result);
            // 路径应以最后一段结尾
            Assert.EndsWith("c", result);
        }

        [Fact]
        public void GetFullPath_ReturnsRootedPathWithFileName()
        {
            var result = "test_file.txt".GetFullPath();
            Assert.NotNull(result);
            Assert.True(Path.IsPathRooted(result!));
            Assert.EndsWith("test_file.txt", result);
        }

        [Fact]
        public void GetBasePath_ReturnsNonNull()
        {
            var result = "sub/file.txt".GetBasePath();
            Assert.NotNull(result);
            Assert.Contains("file.txt", result!);
        }

        [Fact]
        public void GetCurrentPath_ReturnsNonNull()
        {
            var result = "cur/file.txt".GetCurrentPath();
            Assert.NotNull(result);
            Assert.Contains("file.txt", result!);
        }

        [Fact]
        public void AsFile_ReturnsFileInfoWithName()
        {
            var fi = "data/report.xml".AsFile();
            Assert.NotNull(fi);
            Assert.Equal("report.xml", fi!.Name);
        }

        [Fact]
        public void AsDirectory_ReturnsDirectoryInfoWithName()
        {
            var di = "logs/2026".AsDirectory();
            Assert.NotNull(di);
            Assert.Equal("2026", di!.Name);
        }

        [Fact]
        public void EnsureDirectory_CreatesDirectoryAndReturnsPath()
        {
            var dir = Path.Combine(Path.GetTempPath(), "DotNetUtilPath_" + Guid.NewGuid().ToString("N"));
            var target = dir + Path.DirectorySeparatorChar; // 斜杠结尾视为目录
            try
            {
                var result = target.EnsureDirectory();
                Assert.NotNull(result);
                Assert.True(Directory.Exists(dir));
            }
            finally
            {
                if (Directory.Exists(dir)) Directory.Delete(dir, true);
            }
        }
    }
}
