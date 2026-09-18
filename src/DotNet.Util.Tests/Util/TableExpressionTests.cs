using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// TableExpression 测试
    /// </summary>
    public class TableExpressionTests
    {
        [Table("Customer")]
        private class Customer
        {
            [Key]
            [Column("CustomerId")]
            public int Id { get; set; }

            public string? Name { get; set; }

            [NotMapped]
            public string? Temp { get; set; }
        }

        [Table("BaseOrder", Schema = "dbo")]
        private class Order
        {
            public int Id { get; set; }
        }

        private class PlainEntity
        {
            public int Id { get; set; }
            public string? Code { get; set; }
        }

        [Fact]
        public void Type_AndNodeType_AreSet()
        {
            var table = new TableExpression(typeof(Customer));

            Assert.Equal(typeof(Customer), table.Type);
            Assert.Equal((ExpressionType)DbExpressionType.Table, table.NodeType);
            Assert.Equal("Table", table.NodeTypeName);
            Assert.IsAssignableFrom<DbBaseExpression>(table);
        }

        [Fact]
        public void Name_WithTableAttribute_UsesAttributeName()
        {
            Assert.Equal("Customer", new TableExpression(typeof(Customer)).Name);
            Assert.Equal("BaseOrder", new TableExpression(typeof(Order)).Name);
        }

        [Fact]
        public void Name_WithoutTableAttribute_UsesTypeName()
        {
            Assert.Equal(nameof(PlainEntity), new TableExpression(typeof(PlainEntity)).Name);
        }

        [Fact]
        public void Schema_WithoutTableAttribute_ReturnsEmptyString()
        {
            Assert.Equal(string.Empty, new TableExpression(typeof(PlainEntity)).Schema);
        }

        [Fact]
        public void Schema_WithSchemaSpecified_ReturnsSchema()
        {
            Assert.Equal("dbo", new TableExpression(typeof(Order)).Schema);
        }

        [Fact]
        public void Schema_WithTableAttributeButNoSchema_ReturnsNull()
        {
            // 注意：无特性时返回 ""，有特性但未指定 Schema 时返回 null，两条分支返回值不一致
            Assert.Null(new TableExpression(typeof(Customer)).Schema);
        }

        [Fact]
        public void Columns_MapsAllPublicProperties()
        {
            var table = new TableExpression(typeof(PlainEntity));

            Assert.Equal(2, table.Columns.Count);
            Assert.Contains(table.Columns, it => it.ColumnName == "Id");
            Assert.Contains(table.Columns, it => it.ColumnName == "Code");
        }

        [Fact]
        public void Columns_RespectsColumnAttributeName()
        {
            var table = new TableExpression(typeof(Customer));

            var idColumn = table.Columns.Single(it => it.MemberInfo.Name == nameof(Customer.Id));
            Assert.Equal("CustomerId", idColumn.ColumnName);
            Assert.True(idColumn.IsKey);
            Assert.Equal(typeof(int), idColumn.Type);
        }

        [Fact]
        public void Columns_IncludesNotMappedProperties()
        {
            // 实现中排除 NotMapped 的那行代码被注释掉了，因此 NotMapped 列仍会出现
            var table = new TableExpression(typeof(Customer));

            Assert.Equal(3, table.Columns.Count);
            Assert.Contains(table.Columns, it => it.MemberInfo.Name == nameof(Customer.Temp));
        }

        [Fact]
        public void Columns_HaveEmptyTableAliasAndSequentialIndexes()
        {
            var table = new TableExpression(typeof(Customer));

            Assert.All(table.Columns, it => Assert.Equal(string.Empty, it.TableAlias));
            Assert.Equal(new[] { 0, 1, 2 }, table.Columns.Select(it => it.Index).OrderBy(it => it).ToArray());
        }

        [Fact]
        public void Columns_IsLazyAndCached()
        {
            var table = new TableExpression(typeof(Customer));

            var first = table.Columns;
            var second = table.Columns;

            Assert.Same(first, second);
            Assert.Same(first[0], second[0]);
        }

        [Fact]
        public void Columns_ForTypeWithoutPublicProperties_IsEmpty()
        {
            var table = new TableExpression(typeof(object));

            Assert.Empty(table.Columns);
        }
    }
}
