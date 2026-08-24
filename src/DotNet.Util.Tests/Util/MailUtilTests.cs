using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// MailUtil 纯逻辑分支测试（不触 SMTP）
    /// 说明：Send 的空收件人分支直接返回 false，不连邮件服务器
    /// </summary>
    public class MailUtilTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Send_EmptyTo_ReturnsFalse(string to)
        {
            var result = MailUtil.Send(to, "subject", "body");

            Assert.False(result);
        }
    }
}
