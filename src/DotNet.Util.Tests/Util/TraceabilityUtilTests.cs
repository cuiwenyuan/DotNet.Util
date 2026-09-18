using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// TraceabilityUtil 测试
    /// </summary>
    public class TraceabilityUtilTests
    {
        private const string DefaultKey = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        [Fact]
        public void GenerateKey_Returns62CharsPermutation()
        {
            var key = TraceabilityUtil.GenerateKey();

            Assert.Equal(62, key.Length);
            // 只是打乱顺序，因此字符集与默认键完全一致且不重复
            Assert.Equal(62, key.Distinct().Count());
            Assert.Equal(DefaultKey.OrderBy(c => c), key.OrderBy(c => c));
        }

        [Fact]
        public void GenerateKey_WithZeroRandom_ReturnsDefaultOrder()
        {
            // random = 0 时不执行任何交换，返回原始顺序
            Assert.Equal(DefaultKey, TraceabilityUtil.GenerateKey(0));
        }

        [Fact]
        public void GenerateKey_WithRandom_Returns62CharsPermutation()
        {
            var key = TraceabilityUtil.GenerateKey(10);

            Assert.Equal(62, key.Length);
            Assert.Equal(62, key.Distinct().Count());
        }

        [Theory]
        [InlineData(0L)]
        [InlineData(1L)]
        [InlineData(61L)]
        [InlineData(62L)]
        [InlineData(12345L)]
        [InlineData(999999999L)]
        public void Mixup_UnMixup_Roundtrip(long id)
        {
            var code = TraceabilityUtil.Mixup(id);

            Assert.False(string.IsNullOrEmpty(code));
            Assert.Equal(id, TraceabilityUtil.UnMixup(code));
        }

        [Fact]
        public void Mixup_SmallId_ReturnsSingleChar()
        {
            Assert.Equal("0", TraceabilityUtil.Mixup(0));
            Assert.Equal("1", TraceabilityUtil.Mixup(1));
        }

        [Fact]
        public void Mixup_InvalidKeyLength_FallsBackToDefaultKey()
        {
            // key 长度不等于 62 时会被替换为默认键
            Assert.Equal(TraceabilityUtil.Mixup(12345), TraceabilityUtil.Mixup(12345, "abc"));
        }

        [Fact]
        public void Mixup_DifferentIds_ProduceDifferentCodes()
        {
            Assert.NotEqual(TraceabilityUtil.Mixup(12345), TraceabilityUtil.Mixup(12346));
        }
    }
}
