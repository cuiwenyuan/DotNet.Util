using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// BaseResult 与 JsonResult&lt;T&gt; 测试（纯逻辑 POCO）
    /// </summary>
    public class BaseResultTests
    {
        [Fact]
        public void BaseResult_DefaultFieldValues()
        {
            var result = new BaseResult();

            Assert.False(result.Status);
            Assert.Equal("", result.Result);
            Assert.Equal("UnknownError", result.StatusCode);
            Assert.Equal("未知错误", result.StatusMessage);
            Assert.Equal(0, result.RecordCount);
            Assert.Equal(1, result.PageCount);
        }

        [Fact]
        public void BaseResult_Fields_AreMutable()
        {
            var result = new BaseResult
            {
                Status = true,
                Result = "ok",
                StatusCode = "Success",
                StatusMessage = "成功",
                RecordCount = 100,
                PageCount = 10
            };

            Assert.True(result.Status);
            Assert.Equal("ok", result.Result);
            Assert.Equal("Success", result.StatusCode);
            Assert.Equal("成功", result.StatusMessage);
            Assert.Equal(100, result.RecordCount);
            Assert.Equal(10, result.PageCount);
        }

        [Fact]
        public void JsonResult_DefaultData_IsDefault()
        {
            var result = new JsonResult<int>();

            Assert.False(result.Status);
            Assert.Equal(0, result.Data);
        }

        [Fact]
        public void JsonResult_Generic_DataSettable()
        {
            var result = new JsonResult<string> { Status = true, Data = "payload" };

            Assert.True(result.Status);
            Assert.Equal("payload", result.Data);
        }

        [Fact]
        public void JsonResult_IsBaseResult()
        {
            Assert.IsAssignableFrom<BaseResult>(new JsonResult<object>());
        }
    }
}
