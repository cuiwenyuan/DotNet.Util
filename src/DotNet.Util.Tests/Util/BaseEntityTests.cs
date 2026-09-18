using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using DotNet.Model;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// BaseEntity 测试（纯逻辑：字段默认值、Create/GetSingle/GetList/GetBase/IsValid）
    /// 说明：BaseEntity 为抽象类，派生 TestEntity 实现 GetFrom(IDataRow) 以驱动基类逻辑
    /// </summary>
    public class BaseEntityTests
    {
        static BaseEntityTests()
        {
#if NET8_0_OR_GREATER
            // IsValid 内部用 gb2312 编码计算字节长度，net8 下需注册 CodePages 提供程序
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }

        private class TestEntity : BaseEntity
        {
            protected override BaseEntity GetFrom(IDataRow dr)
            {
                GetBase(dr);
                return this;
            }

            [StringLength(5, ErrorMessage = "Name 不能超过 5 个字符")]
            public string Name { get; set; } = string.Empty;
        }

        [Fact]
        public void DefaultFieldValues()
        {
            var entity = new TestEntity();

            Assert.Equal(0, entity.Id);
            Assert.Equal(0, entity.SortCode);
            Assert.Equal(0, entity.Deleted);
            Assert.Equal(1, entity.Enabled);
            Assert.Equal(0, entity.CreateUserId);
            Assert.Equal(string.Empty, entity.CreateBy);
        }

        [Fact]
        public void FieldConstants()
        {
            Assert.Equal("Id", BaseEntity.FieldId);
            Assert.Equal("SortCode", BaseEntity.FieldSortCode);
            Assert.Equal("Deleted", BaseEntity.FieldDeleted);
            Assert.Equal("Enabled", BaseEntity.FieldEnabled);
            Assert.Equal("CreateTime", BaseEntity.FieldCreateTime);
        }

        [Fact]
        public void Create_NoArgs_ReturnsNewInstance()
        {
            var entity = BaseEntity.Create<TestEntity>();

            Assert.NotNull(entity);
            Assert.IsType<TestEntity>(entity);
        }

        [Fact]
        public void Create_WithDataTable_MapsFields()
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("SortCode", typeof(int));
            dt.Columns.Add("Deleted", typeof(int));
            dt.Columns.Add("Enabled", typeof(int));
            dt.Columns.Add("CreateUserId", typeof(int));
            dt.Columns.Add("CreateBy", typeof(string));
            dt.Rows.Add(7, 1, 0, 1, 5, "Troy");

            var entity = BaseEntity.Create<TestEntity>(dt);

            Assert.Equal(7, entity.Id);
            Assert.Equal(1, entity.SortCode);
            Assert.Equal(0, entity.Deleted);
            Assert.Equal(1, entity.Enabled);
            Assert.Equal(5, entity.CreateUserId);
            Assert.Equal("Troy", entity.CreateBy);
        }

        [Fact]
        public void Create_NullDataTable_ReturnsNull()
        {
            Assert.Null(BaseEntity.Create<TestEntity>((DataTable)null));
        }

        [Fact]
        public void Create_EmptyDataTable_ReturnsNull()
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));

            Assert.Null(BaseEntity.Create<TestEntity>(dt));
        }

        [Fact]
        public void GetSingle_EmptyTable_ReturnsNull()
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));

            var entity = new TestEntity().GetSingle(dt);

            Assert.Null(entity);
        }

        [Fact]
        public void GetList_WithRows_ReturnsAll()
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("CreateBy", typeof(string));
            dt.Rows.Add(1, "A");
            dt.Rows.Add(2, "B");

            var list = BaseEntity.GetList<TestEntity>(dt);

            Assert.Equal(2, list.Count);
            Assert.Equal(1, list[0].Id);
            Assert.Equal("B", list[1].CreateBy);
        }

        [Fact]
        public void GetList_NullTable_ReturnsEmptyList()
        {
            var list = BaseEntity.GetList<TestEntity>((DataTable)null);

            Assert.NotNull(list);
            Assert.Empty(list);
        }

        [Fact]
        public void IsValid_ShortValue_ReturnsTrue()
        {
            var entity = new TestEntity { Name = "abc" };

            Assert.True(entity.IsValid(out var message));
            Assert.Equal(string.Empty, message);
        }

        [Fact]
        public void IsValid_TooLongValue_ReturnsFalse()
        {
            var entity = new TestEntity { Name = "abcdefghij" };

            Assert.False(entity.IsValid(out var message));
            Assert.Contains("不能超过", message);
        }
    }
}
