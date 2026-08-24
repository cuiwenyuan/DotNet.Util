using System.Data;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Plus
{
    /// <summary>
    /// ExcelUtil 测试
    /// </summary>
    /// <remarks>
    /// 此前 ExcelUtil 整个类体被 #if NET46_OR_GREATER 包裹，.NET Core / 5+ 下编译为空类无法测试。
    /// 已将纯 NPOI 方法拆分至 ExcelUtil.Npoi.cs（跨框架可用），本测试覆盖 DataTable 与 Excel 的往返转换。
    /// </remarks>
    public class ExcelUtilTests
    {
        [Fact]
        public void DataTableToExcel_ExcelToDataTable_Roundtrip()
        {
            var dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Age", typeof(int));
            dt.Rows.Add("Alice", 30);
            dt.Rows.Add("Bob", 25);

            var file = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
                "ExcelUtil_Roundtrip_" + System.Guid.NewGuid().ToString("N") + ".xlsx");
            try
            {
                ExcelUtil.DataTableToExcel(dt, file);
                Assert.True(System.IO.File.Exists(file));

                var back = ExcelUtil.ExcelToDataTable(file);
                Assert.NotNull(back);
                Assert.Equal(2, back.Rows.Count);
                Assert.Equal("Name", back.Columns[0].ColumnName);
                Assert.Equal("Alice", back.Rows[0][0]?.ToString());
                Assert.Equal("30", back.Rows[0][1]?.ToString());
            }
            finally
            {
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }
            }
        }
    }
}
