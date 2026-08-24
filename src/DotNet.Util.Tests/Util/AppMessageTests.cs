using System;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// AppMessage 测试（纯逻辑：格式化、资源读取、枚举描述）
    /// </summary>
    public class AppMessageTests
    {
        [Fact]
        public void Format_Placeholders_AreReplaced()
        {
            var result = AppMessage.Format("Hello {0} and {1}", "A", "B");

            Assert.Equal("Hello A and B", result);
        }

        [Fact]
        public void Format_NoMessages_ReturnsFormatString()
        {
            // string.Format 对无参占位符会抛异常，这里用无占位符验证
            var result = AppMessage.Format("no placeholder");
            Assert.Equal("no placeholder", result);
        }

        [Fact]
        public void GetMessage_ReturnsEmpty()
        {
            // 实现中资源读取被注释掉，固定返回空串
            Assert.Equal(string.Empty, AppMessage.GetMessage("ANY_ID"));
            Assert.Equal(string.Empty, AppMessage.GetMessage("ANY_ID", "a", "b"));
        }

        [Fact]
        public void GetEnumMessage_KnownCode_ReturnsDescription()
        {
            var result = AppMessage.GetEnumMessage(typeof(DayOfWeek), "Monday");

            // Monday 无 Description 特性时返回 ToString()
            Assert.Equal("Monday", result);
        }

        [Fact]
        public void GetEnumMessage_UnknownCode_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, AppMessage.GetEnumMessage(typeof(DayOfWeek), "NoSuchDay"));
        }

        [Fact]
        public void GetLanguageResource_WithFields_SetsFieldsFromEmptyMessages()
        {
            // 实现中 messages 固定为空串，length>0 恒为 false，因此不赋值，返回 0
            var target = new LanguageTarget();
            var result = AppMessage.GetLanguageResource(target);

            Assert.Equal(0, result);
        }

        [Fact]
        public void MessageConstants_HaveExpectedValues()
        {
            // AppMessage.Message.cs 中静态消息字段（纯字段默认值）
            Assert.Equal("提示信息", AppMessage.Msg0000);
            Assert.Equal("发生未知错误。", AppMessage.Msg0001);
            Assert.Equal("新增成功。", AppMessage.Msg0009);
            Assert.Equal("更新成功。", AppMessage.Msg0010);
            Assert.Equal("保存成功。", AppMessage.Msg0011);
            Assert.Equal("档案服务", AppMessage.FileService);
        }

        private class LanguageTarget
        {
            public string Field1 = "v1";
            public string Field2 = "v2";
        }
    }
}
