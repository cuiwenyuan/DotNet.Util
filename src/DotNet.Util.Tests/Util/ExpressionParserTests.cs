using System.Reflection;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// ExpressionParser 测试
    /// </summary>
    /// <remarks>
    /// ExpressionParser 是 DotNet.Util 程序集内的 internal sealed 类型（ExpressionEvaluator.cs），
    /// 没有配置 InternalsVisibleTo，因此这里通过反射直接驱动其 public 构造函数与 Parse() 方法，
    /// 以便脱离 CalculateExpression.Calculate 的正则白名单，单独验证递归下降解析器本身的行为。
    /// </remarks>
    public class ExpressionParserTests
    {
        private static readonly Type ParserType =
            typeof(CalculateExpression).Assembly.GetType("DotNet.Util.ExpressionParser", throwOnError: true)!;

        private static readonly MethodInfo ParseMethod =
            ParserType.GetMethod("Parse", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;

        /// <summary>
        /// 反射创建解析器并求值，异常按原始类型抛出（剥离 TargetInvocationException 包装）
        /// </summary>
        private static double Parse(string? text)
        {
            var parser = Activator.CreateInstance(ParserType, new object?[] { text })!;
            try
            {
                return (double)ParseMethod.Invoke(parser, null)!;
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw ex.InnerException;
            }
        }

        [Fact]
        public void Type_IsInternalSealed()
        {
            Assert.False(ParserType.IsPublic);
            Assert.True(ParserType.IsSealed);
            Assert.NotNull(ParserType.GetConstructor(new[] { typeof(string) }));
        }

        [Fact]
        public void Parse_SingleNumber_ReturnsValue()
        {
            Assert.Equal(7d, Parse("7"));
            Assert.Equal(0d, Parse("0"));
            Assert.Equal(1.5d, Parse("1.5"));
        }

        [Fact]
        public void Parse_Additive_LeftAssociative()
        {
            Assert.Equal(6d, Parse("1+2+3"));
            Assert.Equal(5d, Parse("10-2-3"));
            Assert.Equal(4d, Parse("10-2-3-1"));
        }

        [Fact]
        public void Parse_Multiplicative_LeftAssociative()
        {
            Assert.Equal(24d, Parse("2*3*4"));
            Assert.Equal(1d, Parse("8/4/2"));
        }

        [Fact]
        public void Parse_OperatorPrecedence_MultiplyBeforeAdd()
        {
            Assert.Equal(14d, Parse("2+3*4"));
            Assert.Equal(10d, Parse("2*3+4"));
            // 除法优先于减法
            Assert.Equal(8d, Parse("10-4/2"));
        }

        [Fact]
        public void Parse_Parentheses_OverridePrecedence()
        {
            Assert.Equal(20d, Parse("(2+3)*4"));
            Assert.Equal(3d, Parse("((1 + 2) * 3 + 6) / 5"));
            Assert.Equal(1d, Parse("((((1))))"));
        }

        [Fact]
        public void Parse_UnarySign_Supported()
        {
            Assert.Equal(-3d, Parse("-3"));
            Assert.Equal(5d, Parse("+5"));
            // 连续一元运算符会递归折叠
            Assert.Equal(3d, Parse("--3"));
            Assert.Equal(-3d, Parse("---3"));
            Assert.Equal(-2d, Parse("-3+1"));
            // 二元减号后紧跟一元减号
            Assert.Equal(5d, Parse("2--3"));
            Assert.Equal(-12d, Parse("-(3*4)"));
        }

        [Fact]
        public void Parse_Division_IsFloatingPoint()
        {
            Assert.Equal(0.5d, Parse("1/2"));
            Assert.Equal(2.5d, Parse("(1+4)/2"));
        }

        [Fact]
        public void Parse_SpacesAreIgnored()
        {
            Assert.Equal(7d, Parse("   7   "));
            Assert.Equal(14d, Parse(" 2  +  3  *  4 "));
        }

        [Fact]
        public void Parse_EmptyOrNullText_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => Parse(string.Empty));
            Assert.Contains("表达式不能为空", ex.Message);

            // 构造函数把 null 归一化为 string.Empty，因此同样抛“表达式不能为空”
            Assert.Throws<ArgumentException>(() => Parse(null));
        }

        [Fact]
        public void Parse_TrailingGarbage_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Parse("1)"));
            Assert.Throws<ArgumentException>(() => Parse("1 2"));
        }

        [Fact]
        public void Parse_MissingOperand_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Parse("1+"));
            Assert.Throws<ArgumentException>(() => Parse("*3"));
        }

        [Fact]
        public void Parse_UnbalancedParenthesis_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => Parse("(1+2"));
            Assert.Contains("括号不匹配", ex.Message);
        }

        [Fact]
        public void Parse_MalformedNumber_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => Parse("1.2.3"));
            Assert.Contains("非法数字", ex.Message);
        }

        [Fact]
        public void Parse_DivideByZero_ThrowsDivideByZeroException()
        {
            Assert.Throws<DivideByZeroException>(() => Parse("1/0"));
            Assert.Throws<DivideByZeroException>(() => Parse("5/(3-3)"));
        }

        [Fact]
        public void Parse_UsesInvariantCulture_ForDecimalPoint()
        {
            // 小数点固定为 '.'，不受当前区域设置影响
            Assert.Equal(3.25d, Parse("1.25+2"));
        }

        [Fact]
        public void Parse_SameInstanceTwice_SecondCallThrows()
        {
            // 解析器持有 _pos 状态且不重置，属于一次性对象
            var parser = Activator.CreateInstance(ParserType, new object?[] { "1+1" })!;
            var first = (double)ParseMethod.Invoke(parser, null)!;
            Assert.Equal(2d, first);

            var ex = Assert.Throws<TargetInvocationException>(() => ParseMethod.Invoke(parser, null));
            Assert.IsType<ArgumentException>(ex.InnerException);
        }
    }
}
