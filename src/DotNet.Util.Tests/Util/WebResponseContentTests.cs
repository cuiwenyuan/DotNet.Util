using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// WebResponseContent 测试（纯逻辑：OK/Error/Set/ResponseType 枚举）
    /// </summary>
    public class WebResponseContentTests
    {
        [Fact]
        public void Ctor_Default_StatusFalse()
        {
            var content = new WebResponseContent();
            Assert.False(content.Status);
            Assert.Null(content.Message);
        }

        [Fact]
        public void Ctor_WithStatus()
        {
            var content = new WebResponseContent(true);
            Assert.True(content.Status);
        }

        [Fact]
        public void OK_SetsStatusTrue()
        {
            var content = new WebResponseContent().OK();
            Assert.True(content.Status);
        }

        [Fact]
        public void OK_WithMessageAndData()
        {
            var content = new WebResponseContent().OK("成功", new { id = 1 });
            Assert.True(content.Status);
            Assert.Equal("成功", content.Message);
            Assert.NotNull(content.Data);
        }

        [Fact]
        public void OK_WithResponseType()
        {
            var content = new WebResponseContent().OK(WebResponseContent.ResponseType.SaveSuccess);
            Assert.True(content.Status);
            Assert.Equal(((int)WebResponseContent.ResponseType.SaveSuccess).ToString(), content.Code);
        }

        [Fact]
        public void Error_SetsStatusFalse()
        {
            var content = new WebResponseContent().Error("失败");
            Assert.False(content.Status);
            Assert.Equal("失败", content.Message);
        }

        [Fact]
        public void Error_WithResponseType()
        {
            var content = new WebResponseContent().Error(WebResponseContent.ResponseType.ServerError);
            Assert.False(content.Status);
            Assert.Equal("1", content.Code);
        }

        [Fact]
        public void Set_WithMsg_KeepsStatus()
        {
            var content = new WebResponseContent().Set(WebResponseContent.ResponseType.LoginExpiration, "请登录");
            Assert.Equal("302", content.Code);
            Assert.Equal("请登录", content.Message);
        }

        [Fact]
        public void Instance_ReturnsNewInstance()
        {
            var a = WebResponseContent.Instance;
            var b = WebResponseContent.Instance;
            Assert.NotNull(a);
            Assert.NotSame(a, b);
        }

        [Fact]
        public void ResponseType_EnumValues()
        {
            Assert.Equal(1, (int)WebResponseContent.ResponseType.ServerError);
            Assert.Equal(302, (int)WebResponseContent.ResponseType.LoginExpiration);
            Assert.Equal(303, (int)WebResponseContent.ResponseType.ParametersLack);
        }
    }
}
