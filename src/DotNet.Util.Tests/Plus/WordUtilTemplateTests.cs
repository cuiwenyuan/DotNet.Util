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

        #region 占位符替换（纯内存 NPOI 操作）

        [Fact]
        public void ReplaceInParagraph_TextPlaceholder_Replaced()
        {
            var doc = new XWPFDocument();
            var paragraph = doc.CreateParagraph();
            paragraph.CreateRun().SetText("Hello {Name}!");
            var replacements = new List<WordUtil.ReplacementBasic>
            {
                new WordUtil.ReplacementBasic { Placeholder = "{Name}", Text = "Troy", Type = WordUtil.PlaceholderTypeEnum.Text }
            };

            WordUtil.ReplaceInParagraph(paragraph, replacements);

            Assert.DoesNotContain("{Name}", paragraph.Text);
            Assert.Contains("Troy", paragraph.Text);
        }

        [Fact]
        public void ReplaceInParagraph_NoPlaceholder_NoChange()
        {
            var doc = new XWPFDocument();
            var paragraph = doc.CreateParagraph();
            paragraph.CreateRun().SetText("plain text");

            WordUtil.ReplaceInParagraph(paragraph, new List<WordUtil.ReplacementBasic>
            {
                new WordUtil.ReplacementBasic { Placeholder = "{X}", Text = "y", Type = WordUtil.PlaceholderTypeEnum.Text }
            });

            Assert.Equal("plain text", paragraph.Text);
        }

        [Fact]
        public void ReplaceInParagraph_NullOrEmpty_NoThrow()
        {
            WordUtil.ReplaceInParagraph(null, null);
            var doc = new XWPFDocument();
            var empty = doc.CreateParagraph();
            WordUtil.ReplaceInParagraph(empty, null);
        }

        [Fact]
        public void ReplaceInWord_ReplacesParagraphAndTable()
        {
            var doc = new XWPFDocument();
            doc.CreateParagraph().CreateRun().SetText("Name: {Name}");
            var table = doc.CreateTable(1, 2);
            table.GetRow(0).GetCell(0).SetText("{Name}");

            WordUtil.ReplaceInWord(doc,
                new List<WordUtil.ReplacementBasic>
                {
                    new WordUtil.ReplacementBasic { Placeholder = "{Name}", Text = "Troy", Type = WordUtil.PlaceholderTypeEnum.Text }
                },
                new List<WordUtil.ReplacementGrid>());

            var texts = new List<string>();
            var paragraphs = doc.GetParagraphsEnumerator();
            while (paragraphs.MoveNext())
            {
                texts.Add(paragraphs.Current.Text);
            }
            var all = string.Join("|", texts);

            Assert.Contains("Troy", all);
            Assert.DoesNotContain("{Name}", all);
        }

        [Fact]
        public void ReplaceInTable_GridPlaceholder_CopiesTemplateRows()
        {
            var doc = new XWPFDocument();
            // 3 行 2 列：占位符行 + 模板行
            var table = doc.CreateTable(3, 2);
            table.GetRow(0).GetCell(0).SetText("{Items}");
            table.GetRow(1).GetCell(0).SetText("{ColA}");
            table.GetRow(1).GetCell(1).SetText("{ColB}");
            table.GetRow(2).GetCell(0).SetText("tail");

            var grid = new WordUtil.ReplacementGrid
            {
                Placeholder = "{Items}",
                Rows = new List<WordUtil.ReplacementRow>
                {
                    new WordUtil.ReplacementRow
                    {
                        Cells = new List<WordUtil.ReplacementBasic>
                        {
                            new WordUtil.ReplacementBasic { Placeholder = "{ColA}", Text = "A1", Type = WordUtil.PlaceholderTypeEnum.Text },
                            new WordUtil.ReplacementBasic { Placeholder = "{ColB}", Text = "B1", Type = WordUtil.PlaceholderTypeEnum.Text }
                        }
                    },
                    new WordUtil.ReplacementRow
                    {
                        Cells = new List<WordUtil.ReplacementBasic>
                        {
                            new WordUtil.ReplacementBasic { Placeholder = "{ColA}", Text = "A2", Type = WordUtil.PlaceholderTypeEnum.Text },
                            new WordUtil.ReplacementBasic { Placeholder = "{ColB}", Text = "B2", Type = WordUtil.PlaceholderTypeEnum.Text }
                        }
                    }
                }
            };

            WordUtil.ReplaceInTable(table, new List<WordUtil.ReplacementBasic>(), new List<WordUtil.ReplacementGrid> { grid });

            // 占位符行 + 模板行被移除，新增 2 行数据 → 原 3 行 -2 +2 = 3 行
            Assert.True(table.NumberOfRows >= 2);
            var text = string.Join("|", table.Rows.Select(r => r.GetCell(0).GetText()));
            Assert.DoesNotContain("{Items}", text);
        }

        [Fact]
        public void ReplaceInTable_NullTable_NoThrow()
        {
            WordUtil.ReplaceInTable(null, null, null);
        }

        [Fact]
        public void CopyRow_AddsRow()
        {
            var doc = new XWPFDocument();
            var table = doc.CreateTable(1, 2);
            table.GetRow(0).GetCell(0).SetText("src");

            var copy = WordUtil.CopyRow(table.GetRow(0), table, 1);

            Assert.NotNull(copy);
            Assert.Equal(2, table.NumberOfRows);
        }

        #endregion
    }
}
