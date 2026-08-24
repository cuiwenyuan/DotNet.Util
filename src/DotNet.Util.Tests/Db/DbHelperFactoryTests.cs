using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    /// DbHelperFactory 测试（反射创建实例，不连库）
    /// </summary>
    public class DbHelperFactoryTests
    {
        [Fact]
        public void Create_Default_ReturnsSqlHelper()
        {
            var helper = DbHelperFactory.Create();

            Assert.NotNull(helper);
            Assert.Equal(CurrentDbType.SqlServer, helper.CurrentDbType);
        }

        [Fact]
        public void Create_SqlServer_ReturnsHelper()
        {
            var helper = DbHelperFactory.Create(CurrentDbType.SqlServer, "Server=.;Database=test;");

            Assert.NotNull(helper);
            Assert.Equal("Server=.;Database=test;", helper.ConnectionString);
        }

        [Fact]
        public void Create_WithDbOption()
        {
            var option = new DbOption
            {
                CurrentDbType = CurrentDbType.SqlServer,
                ConnectionString = "Server=x;"
            };

            var helper = DbHelperFactory.Create(option);

            Assert.NotNull(helper);
            Assert.Equal("Server=x;", helper.ConnectionString);
        }
    }
}
