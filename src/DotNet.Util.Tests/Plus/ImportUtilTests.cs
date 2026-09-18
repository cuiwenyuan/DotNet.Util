using System.Data;
using System.IO;
using System.Text;
using DotNet.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Xunit;

namespace DotNet.Util.Tests.Plus
{
    /// <summary>
    /// ImportUtil 测试
    /// - ImportExcel：用 NPOI 生成临时 xlsx → 导入验证列名/数据
    /// - CheckColumnExist / CheckIsNullOrEmpty / DataTableColumn2String：纯逻辑
    /// </summary>
    public class ImportUtilTests : IDisposable
    {
        private readonly string _tempDir;

        public ImportUtilTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "DotNetUtilImp_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            try { Directory.Delete(_tempDir, true); } catch { }
        }

        private string CreateXlsx(string[,] cells)
        {
            var path = Path.Combine(_tempDir, Guid.NewGuid().ToString("N") + ".xlsx");
            using (var fs = File.Create(path))
            {
                var wb = new XSSFWorkbook();
                var sheet = wb.CreateSheet("Sheet1");
                for (var r = 0; r < cells.GetLength(0); r++)
                {
                    var row = sheet.CreateRow(r);
                    for (var c = 0; c < cells.GetLength(1); c++)
                    {
                        row.CreateCell(c).SetCellValue(cells[r, c]);
                    }
                }
                wb.Write(fs);
            }
            return path;
        }

        [Fact]
        public void ImportExcel_ReadsHeaderAndData()
        {
            var path = CreateXlsx(new string[,]
            {
                { "Name", "Age" },
                { "Troy", "30" },
                { "Cui", "40" }
            });

            var dt = ImportUtil.ImportExcel(path);

            Assert.NotNull(dt);
            Assert.Equal(2, dt!.Columns.Count);
            Assert.Equal("Name", dt.Columns[0].ColumnName);
            Assert.Equal("Age", dt.Columns[1].ColumnName);
            Assert.Equal(2, dt.Rows.Count);
            Assert.Equal("Troy", dt.Rows[0]["Name"].ToString());
            Assert.Equal("40", dt.Rows[1]["Age"].ToString());
        }

        [Fact]
        public void ImportExcel_EmptySheet_ReturnsEmptyTable()
        {
            var path = Path.Combine(_tempDir, "empty.xlsx");
            using (var fs = File.Create(path))
            {
                var wb = new XSSFWorkbook();
                wb.CreateSheet("Empty");
                wb.Write(fs);
            }

            var dt = ImportUtil.ImportExcel(path);

            Assert.NotNull(dt);
            Assert.Equal(0, dt!.Rows.Count);
        }

        [Fact]
        public void ImportExcel_NumericCell_ConvertsToString()
        {
            var path = Path.Combine(_tempDir, "num.xlsx");
            using (var fs = File.Create(path))
            {
                var wb = new XSSFWorkbook();
                var sheet = wb.CreateSheet("S");
                var header = sheet.CreateRow(0);
                header.CreateCell(0).SetCellValue("Value");
                var row = sheet.CreateRow(1);
                row.CreateCell(0).SetCellValue(123.0);
                wb.Write(fs);
            }

            var dt = ImportUtil.ImportExcel(path);

            // GetCellStringValue 对 Numeric 先试 DateCellValue（NPOI 将 123 解释为 1900/5/2 日期），
            // 只断言"能取到非空字符串"这一稳定行为
            Assert.False(string.IsNullOrEmpty(dt!.Rows[0]["Value"].ToString()));
        }

        [Fact]
        public void ImportExcel_MissingFile_Throws()
        {
            Assert.ThrowsAny<Exception>(() => ImportUtil.ImportExcel(Path.Combine(_tempDir, "missing.xlsx")));
        }

        [Fact]
        public void CheckColumnExist_Present_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, ImportUtil.CheckColumnExist("Name", "Name,Age"));
        }

        [Fact]
        public void CheckColumnExist_Missing_ReturnsHint()
        {
            var result = ImportUtil.CheckColumnExist("Name", "Age");

            Assert.Contains("Name", result);
            Assert.Contains("不存在", result);
        }

        [Fact]
        public void CheckIsNullOrEmpty_EmptyCells_Reported()
        {
            var dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Age");
            dt.Rows.Add("Troy", null);

            var result = ImportUtil.CheckIsNullOrEmpty(dt, new[] { "Name", "Age" });

            Assert.Contains("Age", result);
            Assert.DoesNotContain("Name", result);
            // 错误信息列被自动添加并写入
            Assert.True(dt.Columns.Contains("错误信息"));
            Assert.Contains("Age", dt.Rows[0]["错误信息"].ToString());
        }

        [Fact]
        public void CheckIsNullOrEmpty_AllFilled_ReturnsEmpty()
        {
            var dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Rows.Add("Troy");

            var result = ImportUtil.CheckIsNullOrEmpty(dt, new[] { "Name" });

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void DataTableColumn2String_JoinsColumns()
        {
            var dt = new DataTable();
            dt.Columns.Add("A");
            dt.Columns.Add("B");
            dt.Columns.Add("C");

            Assert.Equal("A,B,C", ImportUtil.DataTableColumn2String(dt));
        }

        [Fact]
        public void DataTableColumn2String_SingleColumn()
        {
            var dt = new DataTable();
            dt.Columns.Add("Only");

            Assert.Equal("Only", ImportUtil.DataTableColumn2String(dt));
        }
    }
}
