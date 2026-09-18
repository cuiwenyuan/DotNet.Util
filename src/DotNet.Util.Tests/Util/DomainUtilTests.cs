using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// DomainUtil 纯逻辑分支测试（不触 LDAP）
    /// 说明：空/无反斜杠用户名的域查询会返回 null（LDAP 连接仅在非空输入时建立）
    /// </summary>
    public class DomainUtilTests
    {
        [Fact]
        public void GetDomainUserInfo_Empty_ReturnsNull()
        {
            Assert.Null(DomainUtil.GetDomainUserInfo(""));
        }

        [Fact]
        public void GetDomainUserInfo_Null_ReturnsNull()
        {
            Assert.Null(DomainUtil.GetDomainUserInfo(null));
        }

        [Fact]
        public void GetDomainUserInfo_NoBackslash_ThrowsOrReturnsNull()
        {
            // 无 \ 分隔符时 userArr[1] 越界 → 内部 catch 返回 null
            Assert.Null(DomainUtil.GetDomainUserInfo("justname"));
        }
    }
}
