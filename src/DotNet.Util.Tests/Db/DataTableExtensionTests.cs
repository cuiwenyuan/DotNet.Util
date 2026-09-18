using System.Data;
using DotNet.Util;

using Xunit;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    /// DataTableExtension 绾弽灏勮浆鎹㈡祴璇曪紙涓嶈繛搴擄級
    /// 娉細ToList/ToEntity 渚濊禆 dynamic.GetFrom(涓氬姟鍩虹被)锛屾湰鎵规浠呮祴 ToAny* 鍙嶅皠璺緞銆?    /// </summary>
    public class DataTableExtensionTests
    {
        private sealed class Person
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }

        private static DataTable BuildPersonTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Rows.Add(1, "Troy");
            dt.Rows.Add(2, "Cui");
            return dt;
        }

        [Fact]
        public void ToAnyList_MapsRowsToEntities()
        {
            var dt = BuildPersonTable();

            var list = dt.ToAnyList<Person>();

            Assert.Equal(2, list.Count);
            Assert.Equal(1, list[0].Id);
            Assert.Equal("Troy", list[0].Name);
            Assert.Equal(2, list[1].Id);
            Assert.Equal("Cui", list[1].Name);
        }

        [Fact]
        public void ToAnyEntity_DataTable_ReturnsFirstRow()
        {
            var dt = BuildPersonTable();

            var entity = dt.ToAnyEntity<Person>();

            Assert.NotNull(entity);
            Assert.Equal(1, entity!.Id);
            Assert.Equal("Troy", entity.Name);
        }

        [Fact]
        public void ToAnyEntity_DataRow_ReturnsMappedEntity()
        {
            var dt = BuildPersonTable();

            var entity = dt.Rows[1].ToAnyEntity<Person>();

            Assert.NotNull(entity);
            Assert.Equal(2, entity!.Id);
            Assert.Equal("Cui", entity.Name);
        }

        [Fact]
        public void ToAnyEntity_DbNull_LeavesDefault()
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Rows.Add(1, DBNull.Value);

            var entity = dt.ToAnyEntity<Person>();

            Assert.NotNull(entity);
            Assert.Equal(1, entity!.Id);
            Assert.Null(entity.Name);
        }

        [Fact]
        public void ToAnyEntity_EmptyDataTable_ReturnsNull()
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));

            var entity = dt.ToAnyEntity<Person>();

            Assert.Null(entity);
        }

        [Fact]
        public void ToAnyList_EmptyDataTable_ReturnsEmptyList()
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));

            var list = dt.ToAnyList<Person>();

            Assert.NotNull(list);
            Assert.Empty(list);
        }
    }
}
