using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// SmsUtil 纯逻辑部分测试
    /// 注意：Send 的完整链路依赖阿里云短信 HTTP 接口与 XmlConfig 配置，不在此测试；
    /// 只测不触网的手机号校验分支（非法手机号直接返回 false）。
    /// </summary>
    public class SmsUtilTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("abcdefg")]
        [InlineData("1380013800")]   // 少一位
        [InlineData("23800138000")]  // 非 1 开头
        public void Send_InvalidMobile_ReturnsFalseWithoutCallingNetwork(string mobile)
        {
            var result = SmsUtil.Send(out var message, mobile, "{}", "SMS_000");

            Assert.False(result);
            Assert.Equal("手机号码有误！", message);
        }
    }
}
