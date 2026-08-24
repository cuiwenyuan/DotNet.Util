using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// DbOption 测试
    /// </summary>
    public class DbOptionTests
    {
        [Fact]
        public void Ctor_Default_AllStringMembersAreNull()
        {
            var option = new DbOption();

            Assert.Null(option.DbId);
            Assert.Null(option.ProviderName);
            Assert.Null(option.FactoryName);
            Assert.Null(option.ConnectionString);
        }

        [Fact]
        public void Ctor_Default_CurrentDbTypeIsOracle()
        {
            // CurrentDbType 枚举未显式赋值，Oracle == 0 成为默认值
            var option = new DbOption();

            Assert.Equal(CurrentDbType.Oracle, option.CurrentDbType);
            Assert.Equal(0, (int)option.CurrentDbType);
        }

        [Fact]
        public void Properties_AreSettableViaInitializer()
        {
            var option = new DbOption
            {
                DbId = "MainDb",
                CurrentDbType = CurrentDbType.SqlServer,
                ProviderName = "System.Data.SqlClient",
                FactoryName = "System.Data.SqlClient.SqlClientFactory, System.Data.SqlClient",
                ConnectionString = "Server=.;Database=Test;Trusted_Connection=True;"
            };

            Assert.Equal("MainDb", option.DbId);
            Assert.Equal(CurrentDbType.SqlServer, option.CurrentDbType);
            Assert.Equal("System.Data.SqlClient", option.ProviderName);
            Assert.Equal("System.Data.SqlClient.SqlClientFactory, System.Data.SqlClient", option.FactoryName);
            Assert.Equal("Server=.;Database=Test;Trusted_Connection=True;", option.ConnectionString);
        }

        [Fact]
        public void Properties_AreMutableAfterConstruction()
        {
            var option = new DbOption { DbId = "A", CurrentDbType = CurrentDbType.MySql };

            option.DbId = "B";
            option.CurrentDbType = CurrentDbType.PostgreSql;
            option.ConnectionString = "Host=localhost";

            Assert.Equal("B", option.DbId);
            Assert.Equal(CurrentDbType.PostgreSql, option.CurrentDbType);
            Assert.Equal("Host=localhost", option.ConnectionString);
        }

        [Theory]
        [InlineData(CurrentDbType.Oracle)]
        [InlineData(CurrentDbType.SqlServer)]
        [InlineData(CurrentDbType.Access)]
        [InlineData(CurrentDbType.Db2)]
        [InlineData(CurrentDbType.MySql)]
        [InlineData(CurrentDbType.SQLite)]
        [InlineData(CurrentDbType.Ase)]
        [InlineData(CurrentDbType.PostgreSql)]
        public void CurrentDbType_AcceptsAllDefinedDatabaseTypes(CurrentDbType dbType)
        {
            var option = new DbOption { CurrentDbType = dbType };

            Assert.Equal(dbType, option.CurrentDbType);
            Assert.True(Enum.IsDefined(typeof(CurrentDbType), option.CurrentDbType));
        }

        [Fact]
        public void CurrentDbType_UndefinedValue_IsNotValidated()
        {
            // 枚举属性没有做取值校验
            var option = new DbOption { CurrentDbType = (CurrentDbType)999 };

            Assert.False(Enum.IsDefined(typeof(CurrentDbType), option.CurrentDbType));
        }

        [Fact]
        public void Instances_UseReferenceEquality()
        {
            var a = new DbOption { DbId = "X" };
            var b = new DbOption { DbId = "X" };

            Assert.NotSame(a, b);
            Assert.NotEqual(a, b);
        }
    }
}
