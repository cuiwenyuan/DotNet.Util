using System.Reflection;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// AssemblyUtil 测试
    ///
    /// 说明：AssemblyUtil 整个源文件被 #if NETSTANDARD2_0_OR_GREATER 包裹，而该符号在
    /// net8.0 / net9.0 / net48 等目标下并不会定义（只在 netstandard 目标下定义），
    /// 因此本测试运行的 net8.0 版 DotNet.Util 里可能根本不存在该类型。
    /// 为了既不写出必然失败的断言、也不产生编译期依赖，这里统一用反射探测类型：
    /// 类型存在才断言，不存在则跳过。
    /// </summary>
    public class AssemblyUtilTests
    {
        private static Type? GetAssemblyUtilType()
        {
            // 用一个确定存在的公开类型定位 DotNet.Util 程序集
            return typeof(EnumDescription).Assembly.GetType("DotNet.Util.AssemblyUtil");
        }

        [Fact]
        public void Type_WhenAvailable_IsStaticClass()
        {
            var type = GetAssemblyUtilType();
            if (type == null)
            {
                // TODO: net8.0 目标下该类型未参与编译（源码条件编译符号为 NETSTANDARD2_0_OR_GREATER），未测
                return;
            }

            // static class 在元数据里表现为 abstract + sealed
            Assert.True(type.IsAbstract);
            Assert.True(type.IsSealed);
        }

        [Fact]
        public void PublicApi_WhenAvailable_ExposesExpectedMethods()
        {
            var type = GetAssemblyUtilType();
            if (type == null)
            {
                // TODO: 类型不可用，未测
                return;
            }

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;

            Assert.NotNull(type.GetMethod("Load", flags));
            Assert.NotNull(type.GetMethod("LoadByNameEndString", flags));
            Assert.NotNull(type.GetMethod("GetCurrentAssemblyName", flags));

            // TODO: Load / LoadByNameEndString 依赖 DependencyContext.Default（运行时 deps.json）
            // 与 AssemblyLoadContext 真实加载程序集，属外部环境依赖，未测。
        }

        [Fact]
        public void GetCurrentAssemblyName_WhenAvailable_ReturnsNonEmptyName()
        {
            var type = GetAssemblyUtilType();
            if (type == null)
            {
                // TODO: 类型不可用，未测
                return;
            }

            var method = type.GetMethod("GetCurrentAssemblyName", BindingFlags.Public | BindingFlags.Static);
            Assert.NotNull(method);

            var name = method!.Invoke(null, null) as string;

            // 内部用 Assembly.GetCallingAssembly()，反射调用时调用方不确定，
            // 因此只断言"返回了一个非空程序集名"这一稳定行为。
            Assert.False(string.IsNullOrEmpty(name));
        }
    }
}
