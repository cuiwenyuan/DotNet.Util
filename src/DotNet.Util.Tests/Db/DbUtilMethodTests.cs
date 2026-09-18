using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    /// DbUtil.Method 测试（仅覆盖不连库的安全分支）
    /// 说明：ExecuteNonQuery/ExecuteScalar 等走 DbHelperFactory 反射创建真实 helper 并连库，无法纯单测；
    /// ExecuteCommandWithSplitter 的空输入分支不触发任何 SQL 执行，可安全验证。
    /// </summary>
    public class DbUtilMethodTests
    {
        [Fact]
        public void ExecuteCommandWithSplitter_Null_DoesNotThrow()
        {
            var ex = Record.Exception(() => DbUtil.ExecuteCommandWithSplitter(null));
            Assert.Null(ex);
        }

        [Fact]
        public void ExecuteCommandWithSplitter_Empty_DoesNotThrow()
        {
            var ex = Record.Exception(() => DbUtil.ExecuteCommandWithSplitter(""));
            Assert.Null(ex);
        }

        [Fact]
        public void ExecuteCommandWithSplitter_Whitespace_DoesNotThrow()
        {
            var ex = Record.Exception(() => DbUtil.ExecuteCommandWithSplitter("   "));
            Assert.Null(ex);
        }
    }
}
