using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// ColumnExpression 测试
    /// </summary>
    public class ColumnExpressionTests
    {
        private class Customer
        {
            [Key]
            [Column("CustomerId")]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            public string? Name { get; set; }

            public int? Age { get; set; }

            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            public decimal Total { get; set; }

            public int? NullableField = null;

            public string PlainField = string.Empty;
        }

        private static PropertyInfo Prop(string name) => typeof(Customer).GetProperty(name)!;

        private static FieldInfo Field(string name) => typeof(Customer).GetField(name)!;

        [Fact]
        public void Ctor_Minimal_SetsCoreProperties()
        {
            var member = Prop(nameof(Customer.Name));
            var column = new ColumnExpression(typeof(string), "a", member, 3);

            Assert.Equal(typeof(string), column.Type);
            Assert.Equal("a", column.TableAlias);
            Assert.Equal(3, column.Index);
            Assert.Same(member, column.MemberInfo);
            Assert.Null(column.ColumnAlias);
            Assert.Null(column.FunctionName);
            Assert.Null(column.Value);
        }

        [Fact]
        public void NodeType_IsDbExpressionTypeColumn()
        {
            var column = new ColumnExpression(typeof(string), "a", Prop(nameof(Customer.Name)), 0);

            Assert.Equal((ExpressionType)DbExpressionType.Column, column.NodeType);
            Assert.Equal("Column", column.NodeTypeName);
            Assert.IsAssignableFrom<DbBaseExpression>(column);
        }

        [Fact]
        public void ColumnName_WithoutColumnAttribute_UsesMemberName()
        {
            var column = new ColumnExpression(typeof(string), "a", Prop(nameof(Customer.Name)), 0);

            Assert.Equal("Name", column.ColumnName);
        }

        [Fact]
        public void ColumnName_WithColumnAttribute_UsesAttributeName()
        {
            var column = new ColumnExpression(typeof(int), "a", Prop(nameof(Customer.Id)), 0);

            Assert.Equal("CustomerId", column.ColumnName);
        }

        [Fact]
        public void ColumnName_NullMemberInfo_ReturnsAsterisk()
        {
            var column = new ColumnExpression(typeof(int), "a", null!, 0);

            Assert.Equal("*", column.ColumnName);
        }

        [Fact]
        public void IsKey_ReflectsKeyAttribute()
        {
            Assert.True(new ColumnExpression(typeof(int), "a", Prop(nameof(Customer.Id)), 0).IsKey);
            Assert.False(new ColumnExpression(typeof(string), "a", Prop(nameof(Customer.Name)), 0).IsKey);
            Assert.False(new ColumnExpression(typeof(string), "a", null!, 0).IsKey);
        }

        [Fact]
        public void IsDatabaseGeneratedIdentity_OnlyTrueForIdentityOption()
        {
            Assert.True(new ColumnExpression(typeof(int), "a", Prop(nameof(Customer.Id)), 0).IsDatabaseGeneratedIdentity);
            // Computed 不算自增
            Assert.False(new ColumnExpression(typeof(decimal), "a", Prop(nameof(Customer.Total)), 0).IsDatabaseGeneratedIdentity);
            Assert.False(new ColumnExpression(typeof(string), "a", Prop(nameof(Customer.Name)), 0).IsDatabaseGeneratedIdentity);
            Assert.False(new ColumnExpression(typeof(string), "a", null!, 0).IsDatabaseGeneratedIdentity);
        }

        [Fact]
        public void IsNullable_ForPropertyInfo()
        {
            Assert.True(new ColumnExpression(typeof(int?), "a", Prop(nameof(Customer.Age)), 0).IsNullable);
            Assert.False(new ColumnExpression(typeof(int), "a", Prop(nameof(Customer.Id)), 0).IsNullable);
            // 引用类型的 C# 可空标注不参与判断，只看 Nullable<T>
            Assert.False(new ColumnExpression(typeof(string), "a", Prop(nameof(Customer.Name)), 0).IsNullable);
        }

        [Fact]
        public void IsNullable_ForFieldInfo()
        {
            Assert.True(new ColumnExpression(typeof(int?), "a", Field(nameof(Customer.NullableField)), 0).IsNullable);
            Assert.False(new ColumnExpression(typeof(string), "a", Field(nameof(Customer.PlainField)), 0).IsNullable);
        }

        [Fact]
        public void IsNullable_NullMemberInfo_ReturnsFalse()
        {
            Assert.False(new ColumnExpression(typeof(int?), "a", null!, 0).IsNullable);
        }

        [Fact]
        public void Ctor_StringLiteralFifthArgument_BindsToColumnAlias()
        {
            // 存在 (…, object value) 与 (…, string columnAlias) 两个重载，
            // 字符串字面量会命中 string 重载（ColumnAlias），Value 保持为 null
            var column = new ColumnExpression(typeof(string), "a", Prop(nameof(Customer.Name)), 0, "n1");

            Assert.Equal("n1", column.ColumnAlias);
            Assert.Null(column.Value);
        }

        [Fact]
        public void Ctor_ObjectTypedFifthArgument_BindsToValue()
        {
            object boxed = "Troy";
            var column = new ColumnExpression(typeof(string), "a", Prop(nameof(Customer.Name)), 0, boxed);

            Assert.Equal("Troy", column.Value);
            Assert.Null(column.ColumnAlias);
        }

        [Fact]
        public void Ctor_ValueOverload_WithNonStringValue()
        {
            var column = new ColumnExpression(typeof(int), "a", Prop(nameof(Customer.Id)), 1, 100);

            Assert.Equal(100, column.Value);
            Assert.Null(column.ColumnAlias);
        }

        [Fact]
        public void Ctor_ColumnAliasAndFunctionName_SetsBoth()
        {
            var column = new ColumnExpression(typeof(string), "a", Prop(nameof(Customer.Name)), 0, "n1", "len");

            Assert.Equal("n1", column.ColumnAlias);
            Assert.Equal("len", column.FunctionName);
            Assert.Null(column.Value);
        }

        [Fact]
        public void Ctor_ValueAndFunctionName_SetsBoth()
        {
            object boxed = 5;
            var column = new ColumnExpression(typeof(int), "a", Prop(nameof(Customer.Id)), 0, boxed, "len");

            Assert.Equal(5, column.Value);
            Assert.Equal("len", column.FunctionName);
            Assert.Null(column.ColumnAlias);
        }

        [Fact]
        public void Ctor_FullOverload_SetsAliasFunctionAndValue()
        {
            var column = new ColumnExpression(typeof(string), "a", Prop(nameof(Customer.Name)), 2, "n1", "len", "Troy");

            Assert.Equal("n1", column.ColumnAlias);
            Assert.Equal("len", column.FunctionName);
            Assert.Equal("Troy", column.Value);
            Assert.Equal(2, column.Index);
        }

        [Fact]
        public void Setters_AreMutable()
        {
            var column = new ColumnExpression(typeof(string), "a", Prop(nameof(Customer.Name)), 0)
            {
                TableAlias = "b",
                ColumnAlias = "n2",
                FunctionName = "upper",
                Value = "x",
                Index = 9
            };

            Assert.Equal("b", column.TableAlias);
            Assert.Equal("n2", column.ColumnAlias);
            Assert.Equal("upper", column.FunctionName);
            Assert.Equal("x", column.Value);
            Assert.Equal(9, column.Index);
        }

        [Fact]
        public void DeepClone_CopiesAllStateToNewInstance()
        {
            var source = new ColumnExpression(typeof(string), "a", Prop(nameof(Customer.Name)), 4, "n1", "len", "Troy");

            var clone = source.DeepClone();

            Assert.NotSame(source, clone);
            Assert.Equal(source.Type, clone.Type);
            Assert.Equal(source.TableAlias, clone.TableAlias);
            Assert.Equal(source.Index, clone.Index);
            Assert.Equal(source.ColumnAlias, clone.ColumnAlias);
            Assert.Equal(source.FunctionName, clone.FunctionName);
            Assert.Equal(source.Value, clone.Value);
            Assert.Same(source.MemberInfo, clone.MemberInfo);
            Assert.Equal(source.ColumnName, clone.ColumnName);
        }

        [Fact]
        public void DeepClone_MutatingCloneDoesNotAffectSource()
        {
            var source = new ColumnExpression(typeof(string), "a", Prop(nameof(Customer.Name)), 0, "n1", "len", "Troy");

            var clone = source.DeepClone();
            clone.TableAlias = "z";
            clone.ColumnAlias = "z1";
            clone.FunctionName = "lower";
            clone.Value = "changed";

            Assert.Equal("a", source.TableAlias);
            Assert.Equal("n1", source.ColumnAlias);
            Assert.Equal("len", source.FunctionName);
            Assert.Equal("Troy", source.Value);
        }
    }
}
