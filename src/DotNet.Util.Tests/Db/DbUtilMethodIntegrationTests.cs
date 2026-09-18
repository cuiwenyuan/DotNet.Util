using System;
using System.Data;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    ///	DbUtilMethodIntegrationTests
    /// DbUtil 核心执行方法（DbUtil.Method.cs）的集成测试 —— 数据库测试补齐计划 P2。
    /// 
    /// 覆盖：
    /// 1) ExecuteNonQuery —— 显式连接串重载、静态连接串重载、参数化、影响行数；
    /// 2) ExecuteScalar   —— 首行首列、参数化、空结果返回 null；
    /// 3) ExecuteReader   —— 流式读取、参数化（用完必须 Dispose，底层为 CommandBehavior.CloseConnection）；
    /// 4) Fill            —— 填充 DataTable、参数化、空结果仍保留列；
    /// 5) MakeParameter   —— 参数名规范化、null 转 DBNull；
    /// 6) ExecuteCommandWithSplitter —— 默认 GO 分隔、自定义分隔符、空脚本不执行。
    /// 
    /// 默认不通过：未设置环境变量 DUP_TEST_SQLSERVER 时 fixture 构造即抛明确异常。
    /// 所有用例均标注 SqlServerTestCollection 串行执行，避免 DbUtil 静态字段互相污染。
    /// 
    /// 修改记录
    /// 
    ///		2026.09.18 版本：1.0	Troy.Cui 建立（数据库测试补齐计划 P2）。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2026.09.18</date>
    /// </author> 
    /// </summary>
    [Collection(SqlServerTestCollection.Name)]
    public class DbUtilMethodIntegrationTests : IClassFixture<SqlServerTestFixture>
    {
        private readonly SqlServerTestFixture _fixture;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="fixture">数据库测试基础设施</param>
        public DbUtilMethodIntegrationTests(SqlServerTestFixture fixture)
        {
            _fixture = fixture;
        }

        #region 私有辅助

        private string User => SqlServerTestFixture.TableUser;

        private string InsertSql =>
            $"INSERT INTO [dbo].[{User}] ([Code], [Name], [Age], [Enabled], [Deleted], [SortCode]) " +
            "VALUES ('P2001', N'P2User', 30, 1, 0, 99);";

        private string InsertWithParamsSql =>
            $"INSERT INTO [dbo].[{User}] ([Code], [Name], [Age], [Enabled], [Deleted], [SortCode]) " +
            "VALUES (@Code, @Name, @Age, @Enabled, @Deleted, @SortCode);";

        private IDbDataParameter[] BuildInsertParameters(string code, string name)
        {
            return new[]
            {
                _fixture.CreateParameter("Code", code),
                _fixture.CreateParameter("Name", name),
                _fixture.CreateParameter("Age", 33),
                _fixture.CreateParameter("Enabled", true),
                _fixture.CreateParameter("Deleted", false),
                _fixture.CreateParameter("SortCode", 77)
            };
        }

        #endregion

        #region ExecuteNonQuery

        [Fact]
        public void ExecuteNonQuery_WithConnectionString_Insert_ReturnsOne()
        {
            _fixture.ResetData();

            var affected = DbUtil.ExecuteNonQuery(_fixture.ConnectionString, InsertSql);

            Assert.Equal(1, affected);
            Assert.Equal(1, _fixture.UserCount());
        }

        [Fact]
        public void ExecuteNonQuery_WithConnectionString_Update_ReturnsAffectedRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);

            var affected = DbUtil.ExecuteNonQuery(_fixture.ConnectionString,
                $"UPDATE [dbo].[{User}] SET [Name] = N'Updated';");

            Assert.Equal(3, affected);
        }

        [Fact]
        public void ExecuteNonQuery_WithConnectionString_Delete_ReturnsAffectedRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(4);

            var affected = DbUtil.ExecuteNonQuery(_fixture.ConnectionString,
                $"DELETE FROM [dbo].[{User}] WHERE [SortCode] <= 2;");

            Assert.Equal(2, affected);
            Assert.Equal(2, _fixture.UserCount());
        }

        [Fact]
        public void ExecuteNonQuery_NoMatchingRows_ReturnsZero()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(2);

            var affected = DbUtil.ExecuteNonQuery(_fixture.ConnectionString,
                $"UPDATE [dbo].[{User}] SET [Name] = N'Nobody' WHERE 1 = 0;");

            Assert.Equal(0, affected);
        }

        [Fact]
        public void ExecuteNonQuery_WithParameters_InsertsExpectedValues()
        {
            _fixture.ResetData();

            var affected = DbUtil.ExecuteNonQuery(_fixture.ConnectionString,
                InsertWithParamsSql, BuildInsertParameters("P2002", "参数化用户"));

            Assert.Equal(1, affected);

            var name = Convert.ToString(_fixture.ExecuteScalar(
                $"SELECT [Name] FROM [dbo].[{User}] WHERE [Code] = 'P2002'"));
            var age = Convert.ToInt32(_fixture.ExecuteScalar(
                $"SELECT [Age] FROM [dbo].[{User}] WHERE [Code] = 'P2002'"));
            var sortCode = Convert.ToInt32(_fixture.ExecuteScalar(
                $"SELECT [SortCode] FROM [dbo].[{User}] WHERE [Code] = 'P2002'"));

            Assert.Equal("参数化用户", name);
            Assert.Equal(33, age);
            Assert.Equal(77, sortCode);
        }

        [Fact]
        public void ExecuteNonQuery_StaticConnection_Insert_ReturnsOne()
        {
            _fixture.ResetData();

            using (_fixture.UseStaticConnection())
            {
                var affected = DbUtil.ExecuteNonQuery(InsertSql);

                Assert.Equal(1, affected);
            }

            Assert.Equal(1, _fixture.UserCount());
        }

        #endregion

        #region ExecuteScalar

        [Fact]
        public void ExecuteScalar_WithConnectionString_ReturnsFirstRowFirstColumn()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            var result = DbUtil.ExecuteScalar(_fixture.ConnectionString,
                $"SELECT COUNT(*) FROM [dbo].[{User}]");

            Assert.Equal(5, Convert.ToInt32(result));
        }

        [Fact]
        public void ExecuteScalar_WithParameters_ReturnsValue()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);

            var result = DbUtil.ExecuteScalar(_fixture.ConnectionString,
                $"SELECT [Name] FROM [dbo].[{User}] WHERE [Code] = @Code",
                new[] { _fixture.CreateParameter("Code", "U0002") });

            Assert.Equal("User2", Convert.ToString(result));
        }

        [Fact]
        public void ExecuteScalar_NoRows_ReturnsNull()
        {
            _fixture.ResetData();

            var result = DbUtil.ExecuteScalar(_fixture.ConnectionString,
                $"SELECT [Name] FROM [dbo].[{User}] WHERE 1 = 0");

            Assert.Null(result);
        }

        [Fact]
        public void ExecuteScalar_StaticConnection_ReturnsValue()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(2);

            using (_fixture.UseStaticConnection())
            {
                var result = DbUtil.ExecuteScalar($"SELECT COUNT(*) FROM [dbo].[{User}]");

                Assert.Equal(2, Convert.ToInt32(result));
            }
        }

        #endregion

        #region ExecuteReader

        [Fact]
        public void ExecuteReader_WithConnectionString_ReturnsRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(4);

            var count = 0;
            // 底层为 CommandBehavior.CloseConnection，必须释放 reader 才能归还连接
            using (var reader = DbUtil.ExecuteReader(_fixture.ConnectionString,
                $"SELECT [Code], [Name] FROM [dbo].[{User}] ORDER BY [Id]"))
            {
                while (reader.Read())
                {
                    Assert.False(string.IsNullOrEmpty(Convert.ToString(reader["Code"])));
                    count++;
                }
            }

            Assert.Equal(4, count);
        }

        [Fact]
        public void ExecuteReader_WithParameters_ReturnsRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);

            var names = new System.Collections.Generic.List<string>();
            using (var reader = DbUtil.ExecuteReader(_fixture.ConnectionString,
                $"SELECT [Name] FROM [dbo].[{User}] WHERE [Age] >= @Age ORDER BY [Id]",
                new[] { _fixture.CreateParameter("Age", 23) }))
            {
                while (reader.Read())
                {
                    names.Add(Convert.ToString(reader["Name"]));
                }
            }

            // 种子数据 Age = 20 + i，i 从 1 开始：U0001=21, U0002=22, U0003=23
            Assert.Single(names);
            Assert.Equal("User3", names[0]);
        }

        [Fact]
        public void ExecuteReader_StaticConnection_ReturnsRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(2);

            using (_fixture.UseStaticConnection())
            {
                var count = 0;
                using (var reader = DbUtil.ExecuteReader($"SELECT [Id] FROM [dbo].[{User}]"))
                {
                    while (reader.Read())
                    {
                        count++;
                    }
                }

                Assert.Equal(2, count);
            }
        }

        #endregion

        #region Fill

        [Fact]
        public void Fill_WithConnectionString_ReturnsDataTable()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);

            var dt = DbUtil.Fill(_fixture.ConnectionString,
                $"SELECT [Code], [Name], [Age] FROM [dbo].[{User}] ORDER BY [Id]");

            Assert.NotNull(dt);
            Assert.Equal(3, dt.Rows.Count);
            Assert.Equal("U0001", Convert.ToString(dt.Rows[0]["Code"]));
            Assert.Equal(21, Convert.ToInt32(dt.Rows[0]["Age"]));
        }

        [Fact]
        public void Fill_WithParameters_ReturnsDataTable()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);

            var dt = DbUtil.Fill(_fixture.ConnectionString,
                $"SELECT [Code], [Name] FROM [dbo].[{User}] WHERE [Code] = @Code",
                new[] { _fixture.CreateParameter("Code", "U0003") });

            Assert.Single(dt.Rows);
            Assert.Equal("User3", Convert.ToString(dt.Rows[0]["Name"]));
        }

        [Fact]
        public void Fill_NoRows_ReturnsEmptyTableWithColumns()
        {
            _fixture.ResetData();

            var dt = DbUtil.Fill(_fixture.ConnectionString,
                $"SELECT [Id], [Code] FROM [dbo].[{User}] WHERE 1 = 0");

            Assert.NotNull(dt);
            Assert.Empty(dt.Rows);
            Assert.Equal(2, dt.Columns.Count);
            // DbUtil.Fill 内部固定以 DotNet 为表名
            Assert.Equal("DotNet", dt.TableName);
        }

        [Fact]
        public void Fill_StaticConnection_ReturnsDataTable()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(2);

            using (_fixture.UseStaticConnection())
            {
                var dt = DbUtil.Fill($"SELECT [Code] FROM [dbo].[{User}] ORDER BY [Id]");

                Assert.Equal(2, dt.Rows.Count);
            }
        }

        #endregion

        #region MakeParameter

        [Fact]
        public void MakeParameter_ReturnsParameterWithNameAndValue()
        {
            using (_fixture.UseStaticConnection())
            {
                var p = DbUtil.MakeParameter("Code", "U9999");

                Assert.NotNull(p);
                Assert.Equal("@Code", p.ParameterName);
                Assert.Equal("U9999", Convert.ToString(p.Value));
                Assert.Equal(ParameterDirection.Input, p.Direction);
            }
        }

        [Fact]
        public void MakeParameter_NullValue_ConvertsToDbNull()
        {
            using (_fixture.UseStaticConnection())
            {
                var p = DbUtil.MakeParameter("Code", null);

                Assert.NotNull(p);
                Assert.Equal("@Code", p.ParameterName);
                Assert.Same(DBNull.Value, p.Value);
            }
        }

        #endregion

        #region ExecuteCommandWithSplitter

        [Fact]
        public void ExecuteCommandWithSplitter_DefaultGoSplitter_RunsAllBatches()
        {
            _fixture.ResetData();

            var script = InsertSql.Replace("'P2001', N'P2User'", "'S0001', N'ScriptUser1'").Replace(";", "")
                        + "\r\nGO\r\n"
                        + InsertSql.Replace("'P2001', N'P2User'", "'S0002', N'ScriptUser2'").Replace(";", "");

            using (_fixture.UseStaticConnection())
            {
                DbUtil.ExecuteCommandWithSplitter(script);
            }

            Assert.Equal(2, _fixture.UserCount());
        }

        [Fact]
        public void ExecuteCommandWithSplitter_CustomSplitter_RunsAllBatches()
        {
            _fixture.ResetData();

            var script = InsertSql.Replace("'P2001', N'P2User'", "'C0001', N'CustomUser1'").Replace(";", "")
                        + "@@@"
                        + InsertSql.Replace("'P2001', N'P2User'", "'C0002', N'CustomUser2'").Replace(";", "")
                        + "@@@"
                        + InsertSql.Replace("'P2001', N'P2User'", "'C0003', N'CustomUser3'").Replace(";", "");

            using (_fixture.UseStaticConnection())
            {
                DbUtil.ExecuteCommandWithSplitter(script, "@@@");
            }

            Assert.Equal(3, _fixture.UserCount());
        }

        [Fact]
        public void ExecuteCommandWithSplitter_BlankScript_ExecutesNothing()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(1);

            using (_fixture.UseStaticConnection())
            {
                DbUtil.ExecuteCommandWithSplitter("   ");
                DbUtil.ExecuteCommandWithSplitter(string.Empty);
            }

            Assert.Equal(1, _fixture.UserCount());
        }

        #endregion
    }
}
