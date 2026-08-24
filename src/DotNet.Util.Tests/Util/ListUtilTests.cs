using System.Data;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// ListUtil 测试
    /// </summary>
    public class ListUtilTests
    {
        private class Sample
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }

        [Fact]
        public void ListToDataTable_MapsPropertiesToColumns()
        {
            var list = new List<Sample>
            {
                new Sample { Id = 1, Name = "Troy" },
                new Sample { Id = 2, Name = "Cui" }
            };

            var dt = ListUtil.ListToDataTable(list);

            Assert.Equal(2, dt.Rows.Count);
            Assert.True(dt.Columns.Contains("Id"));
            Assert.True(dt.Columns.Contains("Name"));
        }

        [Fact]
        public void ListToDataTable_EmptyList_HasColumnsNoRows()
        {
            var dt = ListUtil.ListToDataTable(new List<Sample>());

            Assert.Equal(0, dt.Rows.Count);
            Assert.True(dt.Columns.Contains("Id"));
            Assert.True(dt.Columns.Contains("Name"));
        }

        [Fact]
        public void ListToDataTable_ValuesCopied()
        {
            var list = new List<Sample> { new Sample { Id = 7, Name = "Wangcaisoft" } };
            var dt = ListUtil.ListToDataTable(list);

            Assert.Equal(7, Convert.ToInt32(dt.Rows[0]!["Id"]));
            Assert.Equal("Wangcaisoft", dt.Rows[0]!["Name"].ToString());
        }

        [Fact]
        public void ListToDataTable_NullList_Throws()
        {
            Assert.Throws<NullReferenceException>(() => ListUtil.ListToDataTable<Sample>(null!));
        }
    }
}
