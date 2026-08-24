using System;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// LogUtil 测试
    ///
    /// 说明：LogUtil.WriteLog / WriteException 固定把日志写到
    /// AppDomain.CurrentDomain.BaseDirectory\Log\ 目录（源码 LogUtil.cs:53），
    /// 测试进程无法重定向 BaseDirectory，写测试会污染测试输出目录且无法做无副作用断言。
    /// 因此这里只做轻量类型/重载可用性冒烟，不触发真实磁盘写入。
    /// </summary>
    public class LogUtilTests
    {
        [Fact]
        public void WriteLog_StringOverload_IsStaticPublic()
        {
            // 仅验证重载存在且签名可调用（不实际写盘，避免污染测试输出目录）
            var method = typeof(LogUtil).GetMethod("WriteLog", new[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) });
            Assert.NotNull(method);
            Assert.True(method.IsStatic);
            Assert.True(method.IsPublic);
        }

        [Fact]
        public void WriteException_Overload_IsStaticPublic()
        {
            var method = typeof(LogUtil).GetMethod("WriteException", new[] { typeof(Exception), typeof(string), typeof(string) });
            Assert.NotNull(method);
            Assert.True(method.IsStatic);
            Assert.True(method.IsPublic);
        }

        // TODO: WriteLog/WriteException 实际写盘依赖 AppDomain.BaseDirectory，无法在单测中
        // 重定向输出目录；如需覆盖可改为注入式日志目录（库改造，非测试问题）。
    }
}
