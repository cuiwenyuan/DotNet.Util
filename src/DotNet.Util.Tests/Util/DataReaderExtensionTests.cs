using System.Data;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// DataReaderExtension 测试（全部使用内存 DataTable 生成的 IDataReader，不依赖数据库）
    /// </summary>
    public class DataReaderExtensionTests
    {
        /// <summary>
        /// ToEntity/ToList 内部使用 dynamic 调用 GetFrom，实体与方法必须为 public 才能被绑定
        /// </summary>
        public class GetFromEntity
        {
            public int Id { get; set; }
            public string? Name { get; set; }

            public GetFromEntity GetFrom(IDataReader dr)
            {
                Id = Convert.ToInt32(dr["Id"]);
                Name = dr["Name"] as string;
                return this;
            }
        }

        /// <summary>
        /// 纯反射映射用实体
        /// </summary>
        public class AnyEntity
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }

        private static DataTable CreateTable(bool withNullRow = false)
        {
            var dt = new DataTable("t");
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Rows.Add(1, "Troy");
            dt.Rows.Add(2, "Cui");
            if (withNullRow)
            {
                dt.Rows.Add(3, DBNull.Value);
            }
            return dt;
        }

        [Fact]
        public void ToAnyList_MapsColumnsToProperties()
        {
            using var reader = CreateTable().CreateDataReader();

            var list = reader.ToAnyList<AnyEntity>();

            Assert.Equal(2, list.Count);
            Assert.Equal(1, list[0].Id);
            Assert.Equal("Troy", list[0].Name);
            Assert.Equal(2, list[1].Id);
            Assert.Equal("Cui", list[1].Name);
        }

        [Fact]
        public void ToAnyList_ClosesReader()
        {
            using var reader = CreateTable().CreateDataReader();

            reader.ToAnyList<AnyEntity>();

            Assert.True(reader.IsClosed);
        }

        [Fact]
        public void ToAnyList_DbNullValue_KeepsPropertyDefault()
        {
            using var reader = CreateTable(withNullRow: true).CreateDataReader();

            var list = reader.ToAnyList<AnyEntity>();

            Assert.Equal(3, list.Count);
            Assert.Equal(3, list[2].Id);
            Assert.Null(list[2].Name);
        }

        [Fact]
        public void ToAnyList_ClosedReader_ReturnsEmptyList()
        {
            var reader = CreateTable().CreateDataReader();
            reader.Close();

            Assert.Empty(reader.ToAnyList<AnyEntity>());
        }

        [Fact]
        public void ToAnyEntity_ReadsOnlyCurrentRow_AndDoesNotCloseReader()
        {
            using var reader = CreateTable().CreateDataReader();

            var first = reader.ToAnyEntity<AnyEntity>();
            var second = reader.ToAnyEntity<AnyEntity>();

            Assert.Equal(1, first.Id);
            Assert.Equal("Troy", first.Name);
            Assert.Equal(2, second.Id);
            Assert.Equal("Cui", second.Name);
            Assert.False(reader.IsClosed);
        }

        [Fact]
        public void ToAnyEntity_ClosedReader_ReturnsDefault()
        {
            var reader = CreateTable().CreateDataReader();
            reader.Close();

            Assert.Null(reader.ToAnyEntity<AnyEntity>());
        }

        [Fact]
        public void ToList_UsesGetFromOfEntity()
        {
            using var reader = CreateTable().CreateDataReader();

            var list = reader.ToList<GetFromEntity>();

            Assert.Equal(2, list.Count);
            Assert.Equal(1, list[0].Id);
            Assert.Equal("Cui", list[1].Name);
            Assert.True(reader.IsClosed);
        }

        [Fact]
        public void ReaderExists_ExistingColumn_ReturnsTrue()
        {
            using var reader = CreateTable().CreateDataReader();

            Assert.True(reader.ReaderExists("Id"));
            Assert.True(reader.ReaderExists("Name"));
        }

        [Fact]
        public void ReaderExists_MissingColumn_ReturnsFalse()
        {
            using var reader = CreateTable().CreateDataReader();

            Assert.False(reader.ReaderExists("NotExists"));
            // 实现使用 string.Equals 序数比较，因此大小写不同视为不存在
            Assert.False(reader.ReaderExists("id"));
        }

        [Fact]
        public void ReaderExists_ClosedReader_ReturnsFalse()
        {
            var reader = CreateTable().CreateDataReader();
            reader.Close();

            Assert.False(reader.ReaderExists("Id"));
        }
    }
}
