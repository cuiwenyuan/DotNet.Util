using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// RandUtil 测试（纯逻辑，仅内存计算，不依赖外部资源）
    /// </summary>
    public class RandUtilTests
    {
        [Fact]
        public void Next_WithMax_IsWithinRange()
        {
            for (var i = 0; i < 200; i++)
            {
                var value = RandUtil.Next(10);

                Assert.True(value >= 0, $"实际值 {value} 小于 0");
                Assert.True(value < 10, $"实际值 {value} 不小于上界 10");
            }
        }

        [Fact]
        public void Next_WithMaxOne_AlwaysZero()
        {
            // 上界不可取，[0,1) 只能是 0
            for (var i = 0; i < 20; i++)
            {
                Assert.Equal(0, RandUtil.Next(1));
            }
        }

        [Fact]
        public void Next_Default_IsNonNegative()
        {
            Assert.True(RandUtil.Next() >= 0);
        }

        [Fact]
        public void Next_WithMinAndMax_IsWithinRange()
        {
            for (var i = 0; i < 200; i++)
            {
                var value = RandUtil.Next(5, 10);

                Assert.True(value >= 5, $"实际值 {value} 小于下界 5");
                Assert.True(value < 10, $"实际值 {value} 不小于上界 10");
            }
        }

        [Fact]
        public void Next_RangeCrossingZero_IsWithinRange()
        {
            for (var i = 0; i < 200; i++)
            {
                var value = RandUtil.Next(-10, 10);

                Assert.True(value >= -10 && value < 10, $"实际值 {value} 超出 [-10,10) 范围");
            }
        }

        [Fact]
        public void NextBytes_ReturnsRequestedLength()
        {
            Assert.Equal(1, RandUtil.NextBytes(1).Length);
            Assert.Equal(16, RandUtil.NextBytes(16).Length);
            Assert.Equal(32, RandUtil.NextBytes(32).Length);
        }

        [Fact]
        public void NextBytes_TwoCallsProduceDifferentData()
        {
            var first = RandUtil.NextBytes(32);
            var second = RandUtil.NextBytes(32);

            Assert.NotNull(first);
            Assert.NotNull(second);
            // 32 字节完全相同的概率可忽略
            Assert.False(first.SequenceEqual(second));
        }

        [Fact]
        public void NextString_ReturnsRequestedLength()
        {
            Assert.Equal(8, RandUtil.NextString(8).Length);
            Assert.Equal(8, RandUtil.NextString(8, true).Length);
            Assert.Equal(1, RandUtil.NextString(1).Length);
        }

        [Fact]
        public void NextString_ZeroLength_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, RandUtil.NextString(0));
        }

        [Fact]
        public void NextString_ContainsOnlyPrintableAsciiChars()
        {
            var text = RandUtil.NextString(64);

            Assert.All(text, c => Assert.True(c >= 0x20 && c < 0x7F, $"出现非可打印 ASCII 字符 0x{(int)c:X2}"));
        }
    }
}
