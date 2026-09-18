using System;
using System.IO;
using System.Linq;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// 第 8 轮 Code Review 修复的回归测试（R8-9 ~ R8-16）
    /// </summary>
    public class Round8P2BugFixTests
    {
        #region R8-9 CopyDirectory 保留旧行为（默认删源）+ 跨平台路径分隔符
        [Fact]
        public void CopyDirectory_DefaultDeletesSource()
        {
            // R8-9 行为变更被拒：保留既有默认 deleteSourceFile=true（Copy 实为 Move+删源），旧调用依赖此语义
            var baseDir = Path.Combine(Path.GetTempPath(), "r8_copydir_" + Guid.NewGuid().ToString("N"));
            var src = Path.Combine(baseDir, "src");
            var dst = Path.Combine(baseDir, "dst");
            Directory.CreateDirectory(src);
            var srcFile = Path.Combine(src, "a.txt");
            File.WriteAllText(srcFile, "hello");
            var sub = Path.Combine(src, "sub");
            Directory.CreateDirectory(sub);
            File.WriteAllText(Path.Combine(sub, "b.txt"), "world");

            try
            {
                FileUtil.CopyDirectory(src, dst); // 默认 deleteSourceFile=true
                // 目标应存在（含子目录递归）：既有行为是把源文件夹整体复制进目标目录（dst\src\...）
                Assert.True(File.Exists(Path.Combine(dst, "src", "a.txt")));
                Assert.True(File.Exists(Path.Combine(dst, "src", "sub", "b.txt")));
                // 默认删源（保留旧 Move+删源语义）
                Assert.False(File.Exists(srcFile));
            }
            finally
            {
                if (Directory.Exists(baseDir)) Directory.Delete(baseDir, true);
            }
        }

        [Fact]
        public void CopyDirectory_KeepsSourceWhenDeleteFalse()
        {
            // 纯复制：调用方显式传 deleteSourceFile: false
            var baseDir = Path.Combine(Path.GetTempPath(), "r8_copydir2_" + Guid.NewGuid().ToString("N"));
            var src = Path.Combine(baseDir, "src");
            var dst = Path.Combine(baseDir, "dst");
            Directory.CreateDirectory(src);
            var srcFile = Path.Combine(src, "a.txt");
            File.WriteAllText(srcFile, "hello");

            try
            {
                FileUtil.CopyDirectory(src, dst, deleteSourceFile: false);
                Assert.True(File.Exists(Path.Combine(dst, "src", "a.txt")));
                Assert.True(File.Exists(srcFile)); // 显式要求保留源
                Assert.Equal("hello", File.ReadAllText(srcFile));
            }
            finally
            {
                if (Directory.Exists(baseDir)) Directory.Delete(baseDir, true);
            }
        }

        [Fact]
        public void CopyDirectory_ForwardSlashPath_Works()
        {
            // 兼容正斜杠路径（修复前硬编码 "\\" 会出错）
            var baseDir = Path.Combine(Path.GetTempPath(), "r8_copydir3_" + Guid.NewGuid().ToString("N")).Replace('\\', '/');
            var src = baseDir + "/src";
            var dst = baseDir + "/dst";
            Directory.CreateDirectory(src);
            File.WriteAllText(Path.Combine(src.Replace('/', '\\'), "a.txt"), "hi");

            try
            {
                FileUtil.CopyDirectory(src, dst);
                Assert.True(File.Exists(Path.Combine(dst.Replace('/', '\\'), "src", "a.txt")));
            }
            finally
            {
                if (Directory.Exists(baseDir.Replace('/', '\\'))) Directory.Delete(baseDir.Replace('/', '\\'), true);
            }
        }
        #endregion

        #region R8-10 IsLocalIp 172.16.0.0/12 修正
        [Theory]
        [InlineData("192.168.1.1", true)]
        [InlineData("10.0.0.1", true)]
        [InlineData("127.0.0.1", true)]
        [InlineData("172.16.0.1", true)]     // 私网
        [InlineData("172.31.255.255", true)] // 私网上界
        [InlineData("172.15.0.1", false)]    // 公网（修复前误判 true）
        [InlineData("172.32.0.1", false)]    // 公网（修复前误判 true）
        [InlineData("8.8.8.8", false)]
        [InlineData("", false)]
        public void IsLocalIp_ReturnsExpected(string ip, bool expected)
        {
            Assert.Equal(expected, IpUtil.IsLocalIp(ip));
        }
        #endregion

        #region R8-11 GetSearchString 转义 LIKE 通配符
        [Fact]
        public void GetSearchString_EscapesBrackets()
        {
            Assert.Equal("%a[[]b%", StringUtil.GetSearchString("a[b"));  // 字面 [ -> [[]（修复前变成 a_b%）
            Assert.Equal("%a[]]b%", StringUtil.GetSearchString("a]b"));  // 字面 ] -> []]
            Assert.DoesNotContain("_", StringUtil.GetSearchString("a[b"));
        }
        #endregion

        #region R8-12 DeleteUnVisibleChar 空值不 NRE
        [Fact]
        public void DeleteUnVisibleChar_Null_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, StringUtil.DeleteUnVisibleChar(null));
        }
        [Fact]
        public void DeleteUnVisibleChar_KeepsVisible()
        {
            Assert.Equal("abc", StringUtil.DeleteUnVisibleChar("abc"));
        }
        #endregion

        #region R8-13 IsNumeric 注释/实现一致（整数/小数，拒绝纯 "."）
        [Theory]
        [InlineData("123", true)]
        [InlineData("-12", true)]
        [InlineData("12.34", true)]
        [InlineData("abc", false)]
        [InlineData("", false)]
        [InlineData(".", false)]   // 修复前误判 true
        [InlineData("1.2.3", false)]
        public void IsNumeric_ReturnsExpected(string input, bool expected)
        {
            Assert.Equal(expected, Validator.IsNumeric(input));
        }
        #endregion

        #region R8-14 ReflectionUtil 去掉 DeclaredOnly 可取基类成员
        private class R8Base { public int Id { get; set; } }
        private class R8Derived : R8Base { public string Name { get; set; } }
        [Fact]
        public void ReflectionUtil_GetProperty_FindsInheritedMember()
        {
            var obj = new R8Derived { Id = 42, Name = "x" };
            Assert.Equal(42, ReflectionUtil.GetProperty(obj, "Id"));   // 基类属性（修复前取不到 → NRE）
            Assert.Equal("x", ReflectionUtil.GetProperty(obj, "Name"));
        }
        #endregion

        #region R8-15 EncodingUtil.Detect 非 seekable 流不抛异常
        private sealed class NonSeekableStream : Stream
        {
            private readonly byte[] _buffer;
            private int _pos;
            public NonSeekableStream(byte[] data) { _buffer = data; _pos = 0; }
            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();
            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }
            public override void Flush() { }
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override int Read(byte[] buffer, int offset, int count)
            {
                if (_pos >= _buffer.Length) return 0;
                var n = Math.Min(count, _buffer.Length - _pos);
                Array.Copy(_buffer, _pos, buffer, offset, n);
                _pos += n;
                return n;
            }
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }

        [Fact]
        public void EncodingUtil_Detect_NonSeekableStream_DoesNotThrow()
        {
            // 带 UTF-8 BOM 的网络流
            var bom = new byte[] { 0xEF, 0xBB, 0xBF, 0x61, 0x62 };
            var enc = EncodingUtil.Detect(new NonSeekableStream(bom));
            Assert.NotNull(enc);
            Assert.Equal("ab", enc.GetString(bom, 3, 2)); // BOM 之后正确解码
        }
        #endregion

        #region R8-16 TraceabilityUtil.GenerateKey 共享 Random + 均匀洗牌
        [Fact]
        public void GenerateKey_IsPermutationOfAlphabet_AndVaries()
        {
            var k1 = TraceabilityUtil.GenerateKey();
            var k2 = TraceabilityUtil.GenerateKey();
            // 62 位字母表置换（全不重复）
            Assert.Equal(62, k1.Length);
            Assert.Equal(62, k1.Distinct().Count());
            // 两次调用大概率不同（修复前同 tick 种子相同 → 必然相同）
            Assert.NotEqual(k1, k2);
        }
        [Fact]
        public void GenerateKey_IntOverload_Works()
        {
            var k = TraceabilityUtil.GenerateKey(3);
            Assert.Equal(62, k.Length);
            Assert.Equal(62, k.Distinct().Count());
        }
        #endregion
    }
}
