using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Plus
{
    /// <summary>
    /// CalculateExpression 测试
    /// </summary>
    /// <remarks>
    /// 此前 DotNet.Util 与 DotNet.Util.Plus 两个程序集存在同名同命名空间的 CalculateExpression，
    /// 导致引用双方的测试项目出现 CS0433 歧义。现已删除 Plus 中的重复文件，复用 DotNet.Util 的实现。
    /// </remarks>
    public class CalculateExpressionTests
    {
        [Fact]
        public void Calculate_BasicArithmetic_ReturnsInt()
        {
            // ((1 + 2) * 3 + 6) / 5 = (9 + 6) / 5 = 3
            Assert.Equal(3, CalculateExpression.Calculate("((1 + 2) * 3 + 6) / 5"));
            Assert.Equal(14, CalculateExpression.Calculate("2 + 3 * 4"));
            Assert.Equal(10, CalculateExpression.Calculate("2 * (3 + 2)"));
            Assert.Equal(-5, CalculateExpression.Calculate("3 - 8"));
        }

        [Fact]
        public void Calculate_Fractional_ReturnsDouble()
        {
            Assert.Equal(0.5, (double)CalculateExpression.Calculate("1 / 2"));
            Assert.Equal(2.5, (double)CalculateExpression.Calculate("(1 + 4) / 2"));
        }

        [Fact]
        public void Calculate_InvalidCharacters_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => CalculateExpression.Calculate("1 + a"));
            Assert.Throws<ArgumentException>(() => CalculateExpression.Calculate(""));
        }

        [Fact]
        public void Calculate_DivideByZero_ThrowsDivideByZeroException()
        {
            Assert.Throws<DivideByZeroException>(() => CalculateExpression.Calculate("1 / 0"));
        }
    }
}
