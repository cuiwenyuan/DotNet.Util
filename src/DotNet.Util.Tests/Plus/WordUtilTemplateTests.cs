using System;
using System.IO;
using DotNet.Util;
using NPOI.XWPF.UserModel;
using Xunit;

namespace DotNet.Util.Tests.Plus
{
    /// <summary>
    /// WordUtil.TemplateExport 文件操作测试（临时 docx 读写，不涉及占位符替换业务）
    /// </summary>
    public class WordUtilTemplateTests : IDisposable
    {
        private readonly string _tempDir;

        public WordUtilTemplateTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "DotNetUtilWord_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            try { Directory.Delete(_tempDir, true); } catch { }
        }

        [Fact]
        public void GetXWPFDocument_MissingFile_ReturnsNull()
        {
            var doc = WordUtil.GetXWPFDocument(Path.Combine(_tempDir, "missing.docx"));

            Assert.Null(doc);
        }

        [Fact]
        public void SaveAndLoadXWPFDocument_RoundTrip()
        {
            var path = Path.Combine(_tempDir, "out.docx");
            var doc = new XWPFDocument();
            doc.CreateParagraph().CreateRun().SetText("Hello");

            WordUtil.SaveXWPFDocument(path, doc);

            Assert.True(File.Exists(path));
            var loaded = WordUtil.GetXWPFDocument(path);
            Assert.NotNull(loaded);
        }

        [Fact]
        public void SaveXWPFDocument_CreatesNestedDirectory()
        {
            var path = Path.Combine(_tempDir, "sub", "dir", "out.docx");
            var doc = new XWPFDocument();

            WordUtil.SaveXWPFDocument(path, doc);

            Assert.True(File.Exists(path));
        }
    }
}
