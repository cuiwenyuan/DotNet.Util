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
    }
}
