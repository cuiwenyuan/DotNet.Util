using System.Linq.Expressions;
using System.Reflection;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// DbBaseExpression 测试
    /// </summary>
    /// <remarks>
    /// DbBaseExpression 的构造函数是 protected，只能通过派生类构造，这里用测试专用派生类驱动。
    /// </remarks>
    public class DbBaseExpressionTests
    {
        /// <summary>
        /// 测试用派生表达式
        /// </summary>
        private sealed class FakeDbExpression : DbBaseExpression
        {
            public FakeDbExpression(DbExpressionType dbExpressionType, Type type)
                : base((ExpressionType)dbExpressionType, type)
            {
            }
        }

        /// <summary>
        /// 用原生 ExpressionType 构造，用于验证 NodeTypeName 的越界行为
        /// </summary>
        private sealed class RawDbExpression : DbBaseExpression
        {
            public RawDbExpression(ExpressionType expressionType, Type type)
                : base(expressionType, type)
            {
            }
        }

        [Fact]
        public void Ctor_SetsNodeTypeAndType()
        {
            var expression = new FakeDbExpression(DbExpressionType.Where, typeof(string));

            Assert.Equal((ExpressionType)DbExpressionType.Where, expression.NodeType);
            Assert.Equal(typeof(string), expression.Type);
        }

        [Fact]
        public void IsSystemLinqExpression()
        {
            var expression = new FakeDbExpression(DbExpressionType.Query, typeof(int));

            Assert.IsAssignableFrom<Expression>(expression);
        }

        [Theory]
        [InlineData(DbExpressionType.Query, "Query")]
        [InlineData(DbExpressionType.Select, "Select")]
        [InlineData(DbExpressionType.Column, "Column")]
        [InlineData(DbExpressionType.Table, "Table")]
        [InlineData(DbExpressionType.Join, "Join")]
        [InlineData(DbExpressionType.Where, "Where")]
        [InlineData(DbExpressionType.WhereCondition, "WhereCondition")]
        [InlineData(DbExpressionType.WhereTrueCondition, "WhereTrueCondition")]
        [InlineData(DbExpressionType.FunctionWhereCondition, "FunctionWhereCondition")]
        [InlineData(DbExpressionType.OrderBy, "OrderBy")]
        [InlineData(DbExpressionType.GroupBy, "GroupBy")]
        public void NodeTypeName_ReturnsDbExpressionTypeName(DbExpressionType dbExpressionType, string expected)
        {
            var expression = new FakeDbExpression(dbExpressionType, typeof(object));

            Assert.Equal(expected, expression.NodeTypeName);
        }

        [Fact]
        public void NodeType_KeepsRawNumericValueOfDbExpressionType()
        {
            var expression = new FakeDbExpression(DbExpressionType.Query, typeof(object));

            // DbExpressionType 从 1000 起，刻意避开 ExpressionType 已用的取值区间
            Assert.Equal(1000, (int)expression.NodeType);
            Assert.False(Enum.IsDefined(typeof(ExpressionType), expression.NodeType));
        }

        [Fact]
        public void NodeTypeName_ForUndefinedDbExpressionType_ReturnsNumericString()
        {
            // ExpressionType.Add == 0，不属于 DbExpressionType 的任何取值，
            // 因此 ToString() 退化为数字字符串
            var expression = new RawDbExpression(ExpressionType.Add, typeof(int));

            Assert.Equal("0", expression.NodeTypeName);
        }

        [Fact]
        public void Ctor_AcceptsNullType()
        {
            // 未做参数校验，null 会被原样保存
            var expression = new FakeDbExpression(DbExpressionType.Select, null!);

            Assert.Null(expression.Type);
        }

        [Fact]
        public void NodeTypeAndType_AreReadOnly()
        {
            var nodeType = typeof(DbBaseExpression).GetProperty(nameof(DbBaseExpression.NodeType),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            var type = typeof(DbBaseExpression).GetProperty(nameof(DbBaseExpression.Type),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;

            Assert.NotNull(nodeType);
            Assert.NotNull(type);
            Assert.Null(nodeType.SetMethod);
            Assert.Null(type.SetMethod);
            Assert.True(nodeType.GetMethod!.IsVirtual);
            Assert.True(type.GetMethod!.IsVirtual);
        }

        [Fact]
        public void DerivedExpressions_ReuseNodeTypeNameLogic()
        {
            var column = new ColumnExpression(typeof(int), "a", typeof(string).GetProperty(nameof(string.Length))!, 0);
            var table = new TableExpression(typeof(object));

            Assert.Equal("Column", column.NodeTypeName);
            Assert.Equal("Table", table.NodeTypeName);
            Assert.IsAssignableFrom<DbBaseExpression>(column);
            Assert.IsAssignableFrom<DbBaseExpression>(table);
        }
    }
}
