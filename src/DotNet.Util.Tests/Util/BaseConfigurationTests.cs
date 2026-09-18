using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// BaseConfiguration 纯逻辑部分测试
    /// 注意：GetSetting() 依赖 ConfigurationUtil/UserConfigUtil/RegistryUtil 读取外部配置，不在此测试
    /// </summary>
    public class BaseConfigurationTests
    {
        [Fact]
        public void Ctor_SetsBaseSystemInfoSoftName()
        {
            var original = BaseSystemInfo.SoftName;
            try
            {
                _ = new BaseConfiguration("MySoft");
                Assert.Equal("MySoft", BaseSystemInfo.SoftName);
            }
            finally
            {
                BaseSystemInfo.SoftName = original;
            }
        }

        [Theory]
        [InlineData("Configuration", ConfigurationCategory.Configuration)]
        [InlineData("UserConfig", ConfigurationCategory.UserConfig)]
        [InlineData("RegistryKey", ConfigurationCategory.RegistryKey)]
        [InlineData("NoSuchCategory", ConfigurationCategory.Configuration)]
        public void GetConfiguration_MapsStringToEnum(string input, ConfigurationCategory expected)
        {
            Assert.Equal(expected, BaseConfiguration.GetConfiguration(input));
        }
    }
}
