using System.Linq.Expressions;
using System.Reflection;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// ColumnsExpression 测试
    /// </summary>
    /// <remarks>
    /// ColumnsExpression 直接继承 System.Linq.Expressions.Expression，但没有重写 NodeType / Type，
    /// 因此不能访问这两个成员（基类实现会抛异常）。测试只覆盖它实际提供的列集合容器语义。
    /// </remarks>
    public class ColumnsExpressionTests
    {
        private class Customer
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }

        private static ColumnExpression Column(string propertyName, int index)
        {
            var member = typeof(Customer).GetProperty(propertyName)!;
            return new ColumnExpression(typeof(Customer).GetProperty(propertyName)!.PropertyType, "a", member, index);
        }

        [Fact]
        public void Ctor_KeepsSameListReference()
        {
            var list = new List<ColumnExpression> { Column(nameof(Customer.Id), 0) };

            var expression = new ColumnsExpression(list);

            Assert.Same(list, expression.ColumnExpressions);
        }

        [Fact]
        public void Ctor_WithMultipleColumns_PreservesOrderAndContent()
        {
            var list = new List<ColumnExpression>
            {
                Column(nameof(Customer.Id), 0),
                Column(nameof(Customer.Name), 1)
            };

            var expression = new ColumnsExpression(list);

            Assert.Equal(2, expression.ColumnExpressions.Count);
            Assert.Equal("Id", expression.ColumnExpressions[0].ColumnName);
            Assert.Equal("Name", expression.ColumnExpressions[1].ColumnName);
            Assert.Equal(0, expression.ColumnExpressions[0].Index);
            Assert.Equal(1, expression.ColumnExpressions[1].Index);
        }

        [Fact]
        public void Ctor_WithEmptyList_ProducesEmptyCollection()
        {
            var expression = new ColumnsExpression(new List<ColumnExpression>());

            Assert.NotNull(expression.ColumnExpressions);
            Assert.Empty(expression.ColumnExpressions);
        }

        [Fact]
        public void Ctor_WithNull_DoesNotThrowAndKeepsNull()
        {
            // 构造函数未做空校验，null 会被原样保存
            var expression = new ColumnsExpression(null!);

            Assert.Null(expression.ColumnExpressions);
        }

        [Fact]
        public void ColumnExpressions_IsSettable()
        {
            var expression = new ColumnsExpression(new List<ColumnExpression>());
            var replacement = new List<ColumnExpression> { Column(nameof(Customer.Name), 7) };

            expression.ColumnExpressions = replacement;

            Assert.Same(replacement, expression.ColumnExpressions);
            Assert.Single(expression.ColumnExpressions);
        }

        [Fact]
        public void MutatingUnderlyingList_IsVisibleThroughProperty()
        {
            var list = new List<ColumnExpression>();
            var expression = new ColumnsExpression(list);

            list.Add(Column(nameof(Customer.Id), 0));

            Assert.Single(expression.ColumnExpressions);
        }

        [Fact]
        public void IsExpression_ButDoesNotOverrideNodeTypeOrType()
        {
            var expression = new ColumnsExpression(new List<ColumnExpression>());

            Assert.IsAssignableFrom<Expression>(expression);
            // 设计说明：与 DbBaseExpression 不同，这里没有声明 NodeType/Type，
            // 所以它无法作为一个可用的自定义表达式节点参与表达式树遍历。
            Assert.Null(typeof(ColumnsExpression).GetProperty("NodeType",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            Assert.Null(typeof(ColumnsExpression).GetProperty("Type",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            Assert.False(typeof(ColumnsExpression).IsSubclassOf(typeof(DbBaseExpression)));
        }
    }
}
