using System.Collections;
using System.Data;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// EnumUtil 测试
    /// </summary>
    public class EnumUtilTests
    {
        private enum SampleEnum
        {
            [EnumDescription("Active status")]
            Active,
            [EnumDescription("Inactive status")]
            Inactive,
            None
        }

        [Fact]
        public void ToDescription_WithAttribute_ReturnsText()
        {
            Assert.Equal("Active status", SampleEnum.Active.ToDescription());
            Assert.Equal("Inactive status", SampleEnum.Inactive.ToDescription());
        }

        [Fact]
        public void ToDescription_WithoutAttribute_ReturnsEnumName()
        {
            Assert.Equal("None", SampleEnum.None.ToDescription());
        }

        [Fact]
        public void GetEnumDescriptions_ReturnsDeclaredOrder()
        {
            var list = EnumUtil.GetEnumDescriptions(typeof(SampleEnum));
            Assert.Equal(3, list.Count);
            Assert.Equal("Active status", (string)list[0]!);
            Assert.Equal("Inactive status", (string)list[1]!);
            // 无特性时回退为字段名
            Assert.Equal("None", (string)list[2]!);
        }

        [Fact]
        public void EnumToDataTable_HasRowsAndColumns()
        {
            var dt = EnumUtil.EnumToDataTable(typeof(SampleEnum));
            Assert.Equal(3, dt.Rows.Count);
            Assert.True(dt.Columns.Contains("key"));
            Assert.True(dt.Columns.Contains("value"));
            Assert.True(dt.Columns.Contains("description"));

            DataRow? activeRow = null;
            foreach (DataRow row in dt.Rows)
            {
                if (row["key"]!.ToString() == "Active")
                {
                    activeRow = row;
                    break;
                }
            }

            Assert.NotNull(activeRow);
            Assert.Equal("Active status", activeRow!["description"].ToString());
            Assert.Equal(0, Convert.ToInt32(activeRow!["value"]));
        }

        [Fact]
        public void EnumToDataTable_CustomColumnNames()
        {
            var dt = EnumUtil.EnumToDataTable(typeof(SampleEnum), "name", "val", "desc");
            Assert.True(dt.Columns.Contains("name"));
            Assert.True(dt.Columns.Contains("val"));
            Assert.True(dt.Columns.Contains("desc"));
            Assert.Equal(3, dt.Rows.Count);
        }
    }
}
