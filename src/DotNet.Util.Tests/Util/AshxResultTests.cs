using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// AshxResult 测试（纯逻辑，不依赖外部资源）
    /// </summary>
    public class AshxResultTests
    {
        [Fact]
        public void Ctor_Default_StatusIsZero()
        {
            var result = new AshxResult();

            Assert.Equal(0, result.status);
            Assert.Null(result.message);
            Assert.Null(result.data);
        }

        [Fact]
        public void Ctor_WithArgs_SetsAllProperties()
        {
            var result = new AshxResult(1, "ok", "payload");

            Assert.Equal(1, result.status);
            Assert.Equal("ok", result.message);
            Assert.Equal("ok", result.msg);
            Assert.Equal("payload", result.data);
        }

        [Fact]
        public void Success_SetsStatusOneAndMessage()
        {
            var result = new AshxResult();

            var json = result.Success("保存成功", new { id = 1 });

            Assert.Equal(1, result.status);
            Assert.Equal("保存成功", result.message);
            Assert.Equal("保存成功", result.msg);
            Assert.NotNull(result.data);
            // 返回的是 JSON 序列化字符串，应包含 message 内容
            Assert.Contains("保存成功", json);
        }

        [Fact]
        public void Fail_SetsStatusZeroAndMessage()
        {
            var result = new AshxResult();

            var json = result.Fail("保存失败");

            Assert.Equal(0, result.status);
            Assert.Equal("保存失败", result.message);
            Assert.Contains("保存失败", json);
        }

        [Fact]
        public void Ok_Static_ReturnsJsonWithStatusOne()
        {
            var json = AshxResult.Ok("成功");

            Assert.Contains("\"status\":1", json);
            Assert.Contains("成功", json);
        }

        [Fact]
        public void Ng_Static_ReturnsJsonWithStatusZero()
        {
            var json = AshxResult.Ng("失败");

            Assert.Contains("\"status\":0", json);
            Assert.Contains("失败", json);
        }
    }
}
