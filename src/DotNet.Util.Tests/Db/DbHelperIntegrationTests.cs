using System;
using System.Data;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    /// DbHelper 集成测试（B/C 类硬外部依赖，覆盖 DbHelper.Method / DbHelper.Async 的数据库访问路径）。
    /// 默认不通过：未设置环境变量 DUP_TEST_SQLSERVER（SQL Server 连接串）时，所有用例以 Assert.Fail 明确提示。
    /// 启用方式（提供测试库连接串后重跑）：
    ///   set DUP_TEST_SQLSERVER=Server=127.0.0.1;Database=Test;User Id=sa;Password=xxx;
    /// 建议使用独立的测试库，避免影响业务数据。
    /// </summary>
    public class DbHelperIntegrationTests
    {
        private static string ConnectionString()
        {
            var cs = Environment.GetEnvironmentVariable("DUP_TEST_SQLSERVER");
            if (string.IsNullOrWhiteSpace(cs))
            {
                Assert.Fail("DbHelper 集成测试未启用：请设置环境变量 DUP_TEST_SQLSERVER=连接串 后重跑。默认不通过。");
            }
            return cs;
        }

        [Fact]
        public void Open_Connects_ToSqlServer()
        {
            var cs = ConnectionString();
            using var db = new SqlHelper { ConnectionString = cs };
            var conn = db.Open();
            Assert.NotNull(conn);
            Assert.Equal(ConnectionState.Open, conn.State);
            db.Close();
        }

        [Fact]
        public void ExecuteScalar_ReturnsValue()
        {
            var cs = ConnectionString();
            using var db = new SqlHelper { ConnectionString = cs };
            var v = db.ExecuteScalar("SELECT 1");
            Assert.Equal(1, Convert.ToInt32(v));
            db.Close();
        }

        [Fact]
        public void Fill_ReturnsDataTable()
        {
            var cs = ConnectionString();
            using var db = new SqlHelper { ConnectionString = cs };
            var dt = db.Fill("SELECT 1 AS X");
            Assert.NotNull(dt);
            Assert.Equal(1, dt.Rows.Count);
            Assert.Equal(1, Convert.ToInt32(dt.Rows[0]["X"]));
            db.Close();
        }

        [Fact]
        public void ExecuteNonQuery_RunsWithoutError()
        {
            var cs = ConnectionString();
            using var db = new SqlHelper { ConnectionString = cs };
            // SELECT 在 SQL Server 上受影响行数为 -1，此处仅验证可执行且不抛异常
            var affected = db.ExecuteNonQuery("SELECT 1");
            Assert.Equal(-1, affected);
            db.Close();
        }
    }
}
