using System.Data;
using DotNet.Util;

using Xunit;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    /// ListExtension.ToDataTable 娴嬭瘯锛堢函鍐呭瓨杞崲锛屼笉杩炲簱锛?    /// </summary>
    public class ListExtensionTests
    {
        private sealed class Person
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public bool Enabled { get; set; }
        }

        [Fact]
        public void ToDataTable_WithItems_CreatesColumnsAndRows()
        {
            var list = new List<Person>
            {
                new Person { Id = 1, Name = "Troy", Enabled = true },
                new Person { Id = 2, Name = "Cui", Enabled = false }
            };

            var dt = list.ToDataTable();

            Assert.Equal(2, dt.Rows.Count);
            Assert.Contains("Id", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
            Assert.Contains("Name", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
            Assert.Contains("Enabled", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
            Assert.Equal(1, dt.Rows[0]["Id"]);
            Assert.Equal("Cui", dt.Rows[1]["Name"]);
        }

        [Fact]
        public void ToDataTable_EmptyList_HasColumnsButNoRows()
        {
            var list = new List<Person>();

            var dt = list.ToDataTable();

            Assert.Equal(0, dt.Rows.Count);
            Assert.Equal(3, dt.Columns.Count);
        }

        [Fact]
        public void ToDataTable_Null_StillReturnsTableWithColumns()
        {
            List<Person>? list = null;

            var dt = list.ToDataTable();

            Assert.NotNull(dt);
            Assert.Equal(0, dt.Rows.Count);
            Assert.Equal(3, dt.Columns.Count);
        }
    }
}
