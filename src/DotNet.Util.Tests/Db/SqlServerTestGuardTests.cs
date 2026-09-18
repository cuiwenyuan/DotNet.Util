using System;
using Xunit;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    ///	SqlServerTestGuardTests
    /// 数据库集成测试的「安全护栏」测试（纯逻辑，不连库、不需要环境变量）。
    /// 
    /// 背景：集成测试包含 Truncate / BatchDelete / Delete 等清空整表的操作，
    /// 一旦有人把 DUP_TEST_SQLSERVER 指向业务库，跑一次测试就会清掉真实数据。
    /// SqlServerTestFixture.ValidateConnectionString 负责拦截，本类用用例把该行为锁死，
    /// 防止将来有人放宽校验而无人察觉。
    /// 
    /// 本类不标注 Collection、不注入 fixture —— 无论环境是否配置都应当通过。
    /// 
    /// 修改记录
    /// 
    ///		2026.09.18 版本：1.0	Troy.Cui 建立（数据库测试补齐计划 P1）。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2026.09.18</date>
    /// </author> 
    /// </summary>
    public class SqlServerTestGuardTests
    {
        #region ExtractDatabase

        [Fact]
        public void ExtractDatabase_WithDatabaseKey_ReturnsName()
        {
            var name = SqlServerTestFixture.ExtractDatabase(
                "Server=127.0.0.1,1433;Database=DotNetUtilTest;Trusted_Connection=True;");

            Assert.Equal("DotNetUtilTest", name);
        }

        [Fact]
        public void ExtractDatabase_WithInitialCatalog_ReturnsName()
        {
            var name = SqlServerTestFixture.ExtractDatabase(
                "Server=127.0.0.1,1433;Initial Catalog=DotNetUtilTest;Trusted_Connection=True;");

            Assert.Equal("DotNetUtilTest", name);
        }

        [Theory]
        [InlineData("database=DotNetUtilTest;")]
        [InlineData("DATABASE=DotNetUtilTest;")]
        [InlineData("initial catalog=DotNetUtilTest;")]
        public void ExtractDatabase_KeyIsCaseInsensitive(string connectionString)
        {
            Assert.Equal("DotNetUtilTest", SqlServerTestFixture.ExtractDatabase(connectionString));
        }

        [Fact]
        public void ExtractDatabase_NoDatabase_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, SqlServerTestFixture.ExtractDatabase("Server=127.0.0.1,1433;Trusted_Connection=True;"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ExtractDatabase_NullOrEmpty_ReturnsEmpty(string? connectionString)
        {
            Assert.Equal(string.Empty, SqlServerTestFixture.ExtractDatabase(connectionString!));
        }

        #endregion

        #region ValidateConnectionString（安全护栏）

        [Fact]
        public void ValidateConnectionString_AllowedDatabase_ReturnsName()
        {
            var name = SqlServerTestFixture.ValidateConnectionString(
                $"Server=127.0.0.1,1433;Database={SqlServerTestFixture.AllowedDatabase};Trusted_Connection=True;");

            Assert.Equal(SqlServerTestFixture.AllowedDatabase, name);
        }

        [Fact]
        public void ValidateConnectionString_NameIsCaseInsensitive()
        {
            var name = SqlServerTestFixture.ValidateConnectionString(
                "Server=127.0.0.1,1433;Database=dotnetutiltest;Trusted_Connection=True;");

            Assert.Equal("dotnetutiltest", name);
        }

        [Theory]
        [InlineData("ProductionDb")]
        [InlineData("master")]
        [InlineData("DotNetUtilTest_Prod")]
        public void ValidateConnectionString_DisallowedDatabase_Throws(string database)
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
                SqlServerTestFixture.ValidateConnectionString($"Server=.;Database={database};Trusted_Connection=True;"));

            Assert.Contains("不在白名单内", ex.Message);
            Assert.Contains(database, ex.Message);
        }

        [Fact]
        public void ValidateConnectionString_NoDatabase_Throws()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
                SqlServerTestFixture.ValidateConnectionString("Server=.;Trusted_Connection=True;"));

            Assert.Contains("未指定数据库", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ValidateConnectionString_NullOrEmpty_Throws(string? connectionString)
        {
            Assert.Throws<InvalidOperationException>(() =>
                SqlServerTestFixture.ValidateConnectionString(connectionString!));
        }

        #endregion
    }
}
