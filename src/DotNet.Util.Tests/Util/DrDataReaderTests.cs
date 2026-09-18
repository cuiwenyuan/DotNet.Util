using System.Data;
using DotNet.Model;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// DrDataReader 测试（IDataReader 包装器，使用内存 DataTable 构造 IDataReader）
    /// </summary>
    public class DrDataReaderTests
    {
        private static DataTable CreateTable()
        {
            var dt = new DataTable("t");
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Remark", typeof(string));
            dt.Rows.Add(1, "Troy", DBNull.Value);
            dt.Rows.Add(2, "Cui", "ok");
            return dt;
        }

        /// <summary>
        /// 返回已定位到第一行的 IDataReader
        /// </summary>
        private static IDataReader CreateReaderOnFirstRow()
        {
            var reader = CreateTable().CreateDataReader();
            reader.Read();
            return reader;
        }

        [Fact]
        public void Indexer_ByName_ReturnsCurrentRowValue()
        {
            using var reader = CreateReaderOnFirstRow();
            var dr = new DrDataReader(reader);

            Assert.Equal(1, Convert.ToInt32(dr["Id"]));
            Assert.Equal("Troy", dr["Name"].ToString());
        }

        [Fact]
        public void Indexer_ByOrdinal_ReturnsCurrentRowValue()
        {
            using var reader = CreateReaderOnFirstRow();
            var dr = new DrDataReader(reader);

            Assert.Equal(1, Convert.ToInt32(dr[0]));
            Assert.Equal("Troy", dr[1].ToString());
        }

        [Fact]
        public void Indexer_NullColumn_ReturnsDbNull()
        {
            using var reader = CreateReaderOnFirstRow();
            var dr = new DrDataReader(reader);

            Assert.Equal(DBNull.Value, dr["Remark"]);
        }

        [Fact]
        public void Indexer_FollowsReaderPosition()
        {
            using var reader = CreateReaderOnFirstRow();
            var dr = new DrDataReader(reader);
            Assert.Equal("Troy", dr["Name"].ToString());

            reader.Read();

            Assert.Equal("Cui", dr["Name"].ToString());
            Assert.Equal("ok", dr["Remark"].ToString());
        }

        [Fact]
        public void ContainsColumn_IsCaseInsensitive()
        {
            using var reader = CreateReaderOnFirstRow();
            var dr = new DrDataReader(reader);

            Assert.True(dr.ContainsColumn("Name"));
            // 源码使用 OrdinalIgnoreCase 比较（兼容 Oracle 自动大写）
            Assert.True(dr.ContainsColumn("NAME"));
            Assert.True(dr.ContainsColumn("id"));
        }

        [Fact]
        public void ContainsColumn_MissingColumn_ReturnsFalse()
        {
            using var reader = CreateReaderOnFirstRow();
            var dr = new DrDataReader(reader);

            Assert.False(dr.ContainsColumn("NotExists"));
        }
    }
}
