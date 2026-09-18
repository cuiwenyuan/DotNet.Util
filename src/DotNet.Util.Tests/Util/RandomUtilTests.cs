using System.Text.RegularExpressions;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// RandomUtil 随机字符串测试（字符集：0-9 + A-H + J-N + P-Z，排除 I/O）
    /// </summary>
    public partial class RandomUtilTests
    {
        private static readonly Regex StringCharset = new("^[0-9A-NP-Z]+$", RegexOptions.Compiled);
        private static readonly Regex NumberCharset = new("^[0-9]+$", RegexOptions.Compiled);

        [Fact]
        public void GetString_Length()
        {
            Assert.Equal(8, RandomUtil.GetString(8).Length);
            Assert.Equal(32, RandomUtil.GetString(32).Length);
        }

        [Fact]
        public void GetString_CharsetOnly()
        {
            var value = RandomUtil.GetString(64);
            Assert.Matches(StringCharset, value);
        }

        [Fact]
        public void GetString_DefaultLength_NonEmpty()
        {
            Assert.False(string.IsNullOrEmpty(RandomUtil.GetString()));
        }

        [Fact]
        public void GetNumber_DigitsOnly()
        {
            var value = RandomUtil.GetNumber(16);
            Assert.Equal(16, value.Length);
            Assert.Matches(NumberCharset, value);
        }

        [Fact]
        public void GetString_Distinctness_Smoke()
        {
            // 极小概率碰撞，仅作冒烟：连续生成 2 次 32 位字符串，几乎不可能相同
            Assert.NotEqual(RandomUtil.GetString(32), RandomUtil.GetString(32));
        }
    }
}
