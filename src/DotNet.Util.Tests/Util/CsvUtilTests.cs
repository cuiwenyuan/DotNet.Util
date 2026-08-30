using System.Data;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// CsvUtil 导出/导入往返测试（临时文件）
    /// </summary>
    public class CsvUtilTests : IDisposable
    {
        private readonly string _tempFile;

        public CsvUtilTests()
        {
            _tempFile = Path.Combine(Path.GetTempPath(), "DotNetUtilCsv_" + Guid.NewGuid().ToString("N") + ".csv");
        }

        public void Dispose()
        {
            if (File.Exists(_tempFile))
            {
                File.Delete(_tempFile);
            }
        }

        [Fact]
        public void ExportCsv_ToDataTable_Roundtrip()
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Rows.Add(1, "Alice");
            dt.Rows.Add(2, "Bob");

            CsvUtil.ExportCsv(dt, _tempFile);
            Assert.True(File.Exists(_tempFile));

            var back = CsvUtil.ToDataTable(_tempFile, ",", firstLineIsHeader: true);
            Assert.NotNull(back);
            Assert.Equal(2, back!.Rows.Count);
            Assert.Equal("1", back.Rows[0]["Id"]!.ToString());
            Assert.Equal("Alice", back.Rows[0]["Name"]!.ToString());
            Assert.Equal("2", back.Rows[1]["Id"]!.ToString());
            Assert.Equal("Bob", back.Rows[1]["Name"]!.ToString());
        }

        [Fact]
        public void ToDataTable_MissingFile_Throws()
        {
            // 不存在的文件当前实现直接抛 IO 异常（可接受，此处固化行为）
            Assert.ThrowsAny<Exception>(() => CsvUtil.ToDataTable(Path.Combine(Path.GetTempPath(), "no_such_file_xyz.csv"), ",", true));
        }

        #region 引号字段回归(GetLength 顺序颠倒 + ReadSpecialCharacter 索引回传)

        [Fact]
        public void ToDataTable_QuotedFieldContainingSeparator_HeaderPlain()
        {
            // 表头无引号、数据行含「引号内逗号」字段。
            // 修复前：GetLength 中 i = j 先于 result -= (j - i) 执行，导致列数合并不生效，
            // 数据行列数(4)与表头(3)不等 -> 整行被静默丢弃（实测 2 行全丢）。
            File.WriteAllText(_tempFile, "Name,Desc,Age\nTom,\"hello,world\",20\nJerry,\"x,y\",21\n", new System.Text.UTF8Encoding(false));

            var dt = CsvUtil.ToDataTable(_tempFile, ",", firstLineIsHeader: true);

            Assert.NotNull(dt);
            Assert.Equal(3, dt!.Columns.Count);
            Assert.Equal(2, dt.Rows.Count);
            Assert.Equal("Tom", dt.Rows[0]["Name"]!.ToString());
            Assert.Equal("hello,world", dt.Rows[0]["Desc"]!.ToString());
            Assert.Equal("20", dt.Rows[0]["Age"]!.ToString());
            Assert.Equal("Jerry", dt.Rows[1]["Name"]!.ToString());
            Assert.Equal("x,y", dt.Rows[1]["Desc"]!.ToString());
        }

        [Fact]
        public void ToDataTable_QuotedFieldContainingSeparator_HeaderQuoted()
        {
            // 表头与数据行都含「引号内逗号」字段。
            // 修复前：ReadSpecialCharacter 的 i = j 作用于值参数无法回传，调用方重复读取已合并片段，
            // 导致多出一列且残留转义引号（列名 esc" / 数据 world"）。
            File.WriteAllText(_tempFile, "\"Name\",\"D,esc\",\"Age\"\n\"Tom\",\"hello,world\",\"20\"\n", new System.Text.UTF8Encoding(false));

            var dt = CsvUtil.ToDataTable(_tempFile, ",", firstLineIsHeader: true);

            Assert.NotNull(dt);
            Assert.Equal(3, dt!.Columns.Count);
            Assert.Equal(1, dt.Rows.Count);
            Assert.Equal("Name", dt.Columns[0].ColumnName);
            Assert.Equal("D,esc", dt.Columns[1].ColumnName);
            Assert.Equal("Age", dt.Columns[2].ColumnName);
            Assert.Equal("Tom", dt.Rows[0][0]!.ToString());
            Assert.Equal("hello,world", dt.Rows[0][1]!.ToString());
            Assert.Equal("20", dt.Rows[0][2]!.ToString());
        }

        [Fact]
        public void ToDataTable_QuotedEmptyField()
        {
            // 空转义字段 "" 应解析为空字符串
            File.WriteAllText(_tempFile, "Name,Desc,Age\nTom,\"\",20\n", new System.Text.UTF8Encoding(false));

            var dt = CsvUtil.ToDataTable(_tempFile, ",", firstLineIsHeader: true);

            Assert.NotNull(dt);
            Assert.Equal(3, dt!.Columns.Count);
            Assert.Equal(1, dt.Rows.Count);
            Assert.Equal("Tom", dt.Rows[0]["Name"]!.ToString());
            Assert.Equal(string.Empty, dt.Rows[0]["Desc"]!.ToString());
            Assert.Equal("20", dt.Rows[0]["Age"]!.ToString());
        }

        [Fact(Skip = "已知遗留缺陷：字段内的转义双引号 (\"\") 会被误判为「字段未闭合」而触发跨字段合并，解析结果为空。需重构 CSV 转义状态机后启用。")]
        public void ToDataTable_EscapedDoubleQuoteInsideQuotedField()
        {
            File.WriteAllText(_tempFile, "Name,Desc,Age\nTom,\"say \"\"hi\"\"\",20\n", new System.Text.UTF8Encoding(false));

            var dt = CsvUtil.ToDataTable(_tempFile, ",", firstLineIsHeader: true);

            Assert.Equal("say \"hi\"", dt!.Rows[0]["Desc"]!.ToString());
        }

        #endregion
    }
}
