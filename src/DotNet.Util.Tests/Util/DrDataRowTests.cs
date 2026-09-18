using System.Data;
using DotNet.Model;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// DrDataRow 测试（DataRow 包装读取器）
    /// </summary>
    public class DrDataRowTests
    {
        private static DataRow CreateRow()
        {
            var dt = new DataTable("t");
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Remark", typeof(string));
            dt.Rows.Add(1, "Troy", DBNull.Value);
            return dt.Rows[0];
        }

        [Fact]
        public void Indexer_ByName_ReturnsColumnValue()
        {
            var dr = new DrDataRow(CreateRow());

            Assert.Equal(1, Convert.ToInt32(dr["Id"]));
            Assert.Equal("Troy", dr["Name"].ToString());
        }

        [Fact]
        public void Indexer_ByOrdinal_ReturnsColumnValue()
        {
            var dr = new DrDataRow(CreateRow());

            Assert.Equal(1, Convert.ToInt32(dr[0]));
            Assert.Equal("Troy", dr[1].ToString());
        }

        [Fact]
        public void Indexer_NullColumn_ReturnsDbNull()
        {
            var dr = new DrDataRow(CreateRow());

            Assert.Equal(DBNull.Value, dr["Remark"]);
        }

        [Fact]
        public void ContainsColumn_ExistingColumn_ReturnsTrue()
        {
            var dr = new DrDataRow(CreateRow());

            Assert.True(dr.ContainsColumn("Id"));
            Assert.True(dr.ContainsColumn("Name"));
        }

        [Fact]
        public void ContainsColumn_MissingColumn_ReturnsFalse()
        {
            var dr = new DrDataRow(CreateRow());

            Assert.False(dr.ContainsColumn("NotExists"));
        }

        [Fact]
        public void Indexer_MissingColumn_Throws()
        {
            var dr = new DrDataRow(CreateRow());

            Assert.Throws<ArgumentException>(() => dr["NotExists"]);
        }
    }
}
