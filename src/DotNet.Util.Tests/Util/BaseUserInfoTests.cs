using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// BaseUserInfo 纯逻辑部分测试（GetUserParameter 占位符替换、CloneData）
    /// 注意：构造函数读取 ConfigurationManager/BaseSystemInfo，此处只验证可实例化与字符串处理
    /// </summary>
    public class BaseUserInfoTests
    {
        [Fact]
        public void Ctor_IsConstructible()
        {
            var info = new BaseUserInfo();

            Assert.NotNull(info);
            // 构造函数内从 BaseSystemInfo 拷贝了服务账号
            Assert.NotNull(info.ServiceUserName);
        }

        [Fact]
        public void GetUserParameter_ReplacesPlaceholders()
        {
            var info = new BaseUserInfo
            {
                Code = "U001",
                UserName = "Troy",
                NickName = "旺财",
                Id = "42",
                UserId = 42,
                OpenId = "openid-1",
                CompanyId = "C001",
                CompanyCode = "CC001"
            };

            var url = info.GetUserParameter("http://x.com?u={UserCode}&n={UserName}&nick={NickName}&id={Id}&uid={UserId}&oid={OpenId}&cid={CompanyId}&cc={CompanyCode}");

            Assert.Contains("u=U001", url);
            Assert.Contains("n=Troy", url);
            Assert.Contains("nick=旺财", url);
            Assert.Contains("id=42", url);
            Assert.Contains("uid=42", url);
            Assert.Contains("oid=openid-1", url);
            Assert.Contains("cid=C001", url);
            Assert.Contains("cc=CC001", url);
        }

        [Fact]
        public void GetUserParameter_ReplacesTicks()
        {
            var info = new BaseUserInfo();
            var url = info.GetUserParameter("http://x.com?t={Ticks}");

            Assert.Matches(@"t=\d{14}", url);
        }

        [Fact]
        public void GetUserParameter_WithAuthorizationCode_NoQuestionMark_AppendsQuestionMark()
        {
            var info = new BaseUserInfo();
            var url = info.GetUserParameter("http://x.com", "ABC123");

            Assert.EndsWith("?code=ABC123", url);
        }

        [Fact]
        public void GetUserParameter_WithAuthorizationCode_HasQuestionMark_AppendsAmpersand()
        {
            var info = new BaseUserInfo();
            var url = info.GetUserParameter("http://x.com?a=1", "ABC123");

            Assert.EndsWith("&code=ABC123", url);
        }

        [Fact]
        public void GetUserParameter_WithCodePlaceholder_ReplacesIt()
        {
            var info = new BaseUserInfo();
            var url = info.GetUserParameter("http://x.com?code={code}", "ABC123");

            Assert.EndsWith("code=ABC123", url);
        }

        [Fact]
        public void GetUserParameter_NullUrl_ReturnsNull()
        {
            var info = new BaseUserInfo();
            Assert.Null(info.GetUserParameter(null));
        }

        [Fact]
        public void CloneData_CopiesSystemCode()
        {
            var source = new BaseUserInfo { SystemCode = "SYS" };
            var target = new BaseUserInfo();

            target.CloneData(source);

            Assert.Equal("SYS", target.SystemCode);
        }

        [Fact]
        public void GetUrl_Relative_AddsWebHostPrefix()
        {
            // WebHost 为只读属性（从配置文件读取），这里只验证相对路径会拼接出 http 前缀
            var info = new BaseUserInfo();

            var url = info.GetUrl("/UserCenter/Index", isUrl: true);

            Assert.StartsWith("http", url);
            Assert.Contains("UserCenter/Index", url);
        }

        [Fact]
        public void GetUrl_AlreadyHttp_Unchanged()
        {
            var info = new BaseUserInfo();
            var url = info.GetUrl("http://elsewhere.com/x", isUrl: true);

            Assert.StartsWith("http://elsewhere.com/x", url);
        }
    }
}
