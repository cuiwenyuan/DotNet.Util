using DotNet.Util;
using NPOI.XWPF.UserModel;
using Xunit;

namespace DotNet.Util.Tests.Plus
{
    /// <summary>
    /// WordUtil（NPOI Word 模板替换）测试：生成 docx → 占位符替换 → 读回断言
    /// </summary>
    public class WordUtilTests : IDisposable
    {
        private readonly string _tempDir;

        public WordUtilTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "DotNetUtilWord_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            try { Directory.Delete(_tempDir, true); } catch { }
        }

        private string NewDocxPath() => Path.Combine(_tempDir, Guid.NewGuid().ToString("N") + ".docx");

        [Fact]
        public void SaveAndLoad_Document_Roundtrip()
        {
            var doc = new XWPFDocument();
            doc.CreateParagraph().CreateRun().SetText("Hello World");
            var path = NewDocxPath();

            WordUtil.SaveXWPFDocument(path, doc);
            Assert.True(File.Exists(path));

            var loaded = WordUtil.GetXWPFDocument(path);
            Assert.NotNull(loaded);
            Assert.Contains("Hello World", loaded!.Paragraphs[0].Text);
        }

        [Fact]
        public void ReplaceInParagraph_ReplacesPlaceholder()
        {
            var doc = new XWPFDocument();
            doc.CreateParagraph().CreateRun().SetText("Dear {Name}, welcome!");
            var path = NewDocxPath();
            WordUtil.SaveXWPFDocument(path, doc);

            var loaded = WordUtil.GetXWPFDocument(path);
            var replacements = new List<WordUtil.ReplacementBasic>
            {
                new() { Placeholder = "{Name}", Text = "Troy" }
            };
            foreach (var paragraph in loaded!.Paragraphs)
            {
                WordUtil.ReplaceInParagraph(paragraph, replacements);
            }
            WordUtil.SaveXWPFDocument(path, loaded);

            var final = WordUtil.GetXWPFDocument(path);
            var text = final!.Paragraphs[0].Text;
            Assert.Contains("Troy", text);
            Assert.DoesNotContain("{Name}", text);
        }
    }
}
