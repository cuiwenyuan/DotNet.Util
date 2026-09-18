using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// SystemTime 测试（纯逻辑 POCO 结构体包装类，字段读写）
    /// </summary>
    public class SystemTimeTests
    {
        [Fact]
        public void Ctor_Default_AllFieldsZero()
        {
            var time = new SystemTime();

            Assert.Equal(0, time.vYear);
            Assert.Equal(0, time.vMonth);
            Assert.Equal(0, time.vDayOfWeek);
            Assert.Equal(0, time.vDay);
            Assert.Equal(0, time.vHour);
            Assert.Equal(0, time.vMinute);
            Assert.Equal(0, time.vSecond);
        }

        [Fact]
        public void Fields_AreSettable()
        {
            var time = new SystemTime
            {
                vYear = 2026,
                vMonth = 8,
                vDay = 24,
                vHour = 10,
                vMinute = 30,
                vSecond = 15
            };

            Assert.Equal(2026, time.vYear);
            Assert.Equal(8, time.vMonth);
            Assert.Equal(24, time.vDay);
            Assert.Equal(10, time.vHour);
            Assert.Equal(30, time.vMinute);
            Assert.Equal(15, time.vSecond);
        }
    }

    /// <summary>
    /// SetSystemDateTime 测试（DllImport 声明，仅验证方法存在，不实际调用）
    /// </summary>
    public class SetSystemDateTimeTests
    {
        [Fact]
        public void DllImportMethods_AreDeclared()
        {
            var type = typeof(SetSystemDateTime);

            var getLocalTime = type.GetMethod("GetLocalTime");
            var setLocalTime = type.GetMethod("SetLocalTime");

            Assert.NotNull(getLocalTime);
            Assert.NotNull(setLocalTime);
            Assert.True(getLocalTime.IsStatic);
            Assert.True(setLocalTime.IsStatic);
        }

        [Fact]
        public void GetLocalTime_AcceptsSystemTime()
        {
            var method = typeof(SetSystemDateTime).GetMethod("GetLocalTime");
            var parameters = method.GetParameters();

            Assert.Single(parameters);
            Assert.Equal(typeof(SystemTime), parameters[0].ParameterType);
        }
    }
}
