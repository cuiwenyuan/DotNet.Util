using System;
using Xunit;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    ///	SqlServerTestFixtureIntegrationTests
    /// SqlServerTestFixture 自身的集成测试（P1 基建验收）。
    /// 
    /// 验证：环境变量门控后的连接串已通过白名单、测试表能幂等创建、
    /// 清表与种子数据可用。后续 P2~P5 的用例都依赖这套基建。
    /// 
    /// 默认不通过：未设置环境变量 DUP_TEST_SQLSERVER 时，fixture 构造即抛明确异常。
    /// 启用方式：
    ///   set DUP_TEST_SQLSERVER=Server=127.0.0.1,1433;Database=DotNetUtilTest;Trusted_Connection=True;TrustServerCertificate=True;
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
    [Collection(SqlServerTestCollection.Name)]
    public class SqlServerTestFixtureIntegrationTests : IClassFixture<SqlServerTestFixture>
    {
        private readonly SqlServerTestFixture _fixture;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="fixture">数据库测试基础设施</param>
        public SqlServerTestFixtureIntegrationTests(SqlServerTestFixture fixture)
        {
            _fixture = fixture;
        }

        #region 连接与白名单

        [Fact]
        public void ConnectionString_PassesWhitelist()
        {
            Assert.Equal(SqlServerTestFixture.AllowedDatabase, _fixture.Database);
            Assert.False(string.IsNullOrWhiteSpace(_fixture.ConnectionString));
        }

        #endregion

        #region 表结构

        [Fact]
        public void EnsureSchema_CreatesUserTable()
        {
            Assert.True(_fixture.UserTableExists(), $"测试表 {SqlServerTestFixture.TableUser} 应已创建");
        }

        [Fact]
        public void EnsureSchema_IsIdempotent()
        {
            // 重复调用不应抛异常、也不应丢表
            _fixture.EnsureSchema();
            _fixture.EnsureSchema();

            Assert.True(_fixture.UserTableExists());
        }

        #endregion

        #region 数据生命周期

        [Fact]
        public void ResetData_ClearsBothTables()
        {
            // 先清一次，避免受其它用例残留数据影响（xUnit 不保证用例顺序）
            _fixture.ResetData();

            _fixture.SeedUsers(3);
            Assert.Equal(3, _fixture.UserCount());

            _fixture.ResetData();

            Assert.Equal(0, _fixture.UserCount());
            Assert.Equal(0, _fixture.OrderCount());
        }

        [Fact]
        public void SeedUsers_InsertsExpectedRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            Assert.Equal(5, _fixture.UserCount());
        }

        [Fact]
        public void SeedUsers_FieldsAreCorrect()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(2);

            var dt = _fixture.Fill($"SELECT [Code], [Name], [Age], [Score], [Enabled], [Deleted], [SortCode], [CreateUserId], [UpdateUserId] FROM [dbo].[{SqlServerTestFixture.TableUser}] ORDER BY [Id]");

            Assert.Equal(2, dt.Rows.Count);

            var row = dt.Rows[0];
            Assert.Equal("U0001", Convert.ToString(row["Code"]));
            Assert.Equal("User1", Convert.ToString(row["Name"]));
            Assert.Equal(21, Convert.ToInt32(row["Age"]));
            Assert.Equal(81m, Convert.ToDecimal(row["Score"]));
            Assert.True(Convert.ToBoolean(row["Enabled"]));
            Assert.False(Convert.ToBoolean(row["Deleted"]));
            Assert.Equal(1, Convert.ToInt32(row["SortCode"]));
            Assert.Equal("seed-creator", Convert.ToString(row["CreateUserId"]));
            Assert.Equal("seed-updater", Convert.ToString(row["UpdateUserId"]));
        }

        [Fact]
        public void SeedOrders_InsertsExpectedRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(1);

            var userId = Convert.ToInt32(_fixture.ExecuteScalar($"SELECT TOP 1 [Id] FROM [dbo].[{SqlServerTestFixture.TableUser}]"));
            _fixture.SeedOrders(userId, 3);

            Assert.Equal(3, _fixture.OrderCount());
            Assert.Equal(1, _fixture.UserCount());
        }

        #endregion

        #region SQL 执行辅助

        [Fact]
        public void ExecuteScalar_ReturnsValue()
        {
            Assert.Equal(1, Convert.ToInt32(_fixture.ExecuteScalar("SELECT 1")));
        }

        [Fact]
        public void Fill_ReturnsDataTable()
        {
            var dt = _fixture.Fill("SELECT 1 AS X");

            Assert.NotNull(dt);
            Assert.Equal(1, dt.Rows.Count);
            Assert.Equal(1, Convert.ToInt32(dt.Rows[0]["X"]));
        }

        #endregion
    }
}
