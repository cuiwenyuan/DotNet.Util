using System.Data;
using System.Data.Common;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    /// SqlBuilder SQL 生成测试
    /// - PrepareCommand 构建 SQL 文本（不连库）
    /// - EndSelect 走执行路径，用最小 IDbHelper 桩捕获 SQL（不真正连库）
    /// </summary>
    public class SqlBuilderTests
    {
        [Fact]
        public void BeginSelect_GeneratesSelect()
        {
            var sb = new SqlBuilder(CurrentDbType.SqlServer);
            sb.BeginSelect("UserInfo");
            var sql = sb.PrepareCommand(out _);
            Assert.Contains("SELECT * FROM UserInfo", sql, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void BeginSelect_SelectTop_SqlServer()
        {
            var stub = new StubDbHelper();
            var sb = new SqlBuilder(stub);
            sb.BeginSelect("UserInfo");
            sb.SelectTop(10);
            sb.EndSelect();
            Assert.Contains("SELECT TOP 10 * FROM UserInfo", stub.LastSql, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void BeginSelect_SelectTop_Sqlite()
        {
            var stub = new StubDbHelper(CurrentDbType.SQLite);
            var sb = new SqlBuilder(stub);
            sb.BeginSelect("UserInfo");
            sb.SelectTop(10);
            sb.EndSelect();
            Assert.Contains("SELECT * FROM UserInfo", stub.LastSql, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("LIMIT", stub.LastSql, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void BeginInsert_GeneratesInsert()
        {
            var sb = new SqlBuilder(CurrentDbType.SqlServer);
            sb.BeginInsert("UserInfo");
            sb.SetValue("UserName", "Troy");
            var sql = sb.PrepareCommand(out _);
            Assert.Contains("INSERT INTO UserInfo", sql, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("UserName", sql, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("VALUES", sql, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void BeginUpdate_GeneratesUpdate()
        {
            var sb = new SqlBuilder(CurrentDbType.SqlServer);
            sb.BeginUpdate("UserInfo");
            sb.SetValue("CompanyName", "Wangcaisoft");
            sb.SetWhere("Id = 1");
            var sql = sb.PrepareCommand(out _);
            Assert.Contains("UPDATE UserInfo", sql, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("SET", sql, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("CompanyName", sql, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void BeginDelete_GeneratesDelete()
        {
            var sb = new SqlBuilder(CurrentDbType.SqlServer);
            sb.BeginDelete("UserInfo");
            sb.SetWhere("Id = 1");
            var sql = sb.PrepareCommand(out _);
            Assert.Contains("DELETE FROM UserInfo", sql, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("WHERE Id = 1", sql, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SetWhere_Multiple_JoinedByAnd()
        {
            var sb = new SqlBuilder(CurrentDbType.SqlServer);
            sb.BeginSelect("UserInfo");
            sb.SetWhere("Id = 1");
            sb.SetWhere("Enabled = 1");
            var sql = sb.PrepareCommand(out _);
            var whereIndex = sql.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase);
            Assert.True(whereIndex >= 0);
            Assert.Contains("AND", sql.Substring(whereIndex), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SetValue_AddsParameter()
        {
            var sb = new SqlBuilder(CurrentDbType.SqlServer);
            sb.BeginInsert("UserInfo");
            sb.SetValue("UserName", "Troy");
            Assert.Single(sb.DbParameters);
            Assert.Equal("UserName", sb.DbParameters[0].Key);
            Assert.Equal("Troy", sb.DbParameters[0].Value);
        }

        [Fact]
        public void PrepareCommand_NoSetValue_DoesNotThrow()
        {
            var sb = new SqlBuilder(CurrentDbType.SqlServer);
            sb.BeginInsert("UserInfo");
            var sql = sb.PrepareCommand(out _);
            Assert.Contains("INSERT INTO UserInfo", sql, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 最小 IDbHelper 桩：仅实现 EndSelect 路径用到的成员，其余抛 NotImplementedException
        /// </summary>
        private sealed class StubDbHelper : IDbHelper
        {
            public StubDbHelper(CurrentDbType dbType = CurrentDbType.SqlServer)
            {
                CurrentDbType = dbType;
            }

            public string? LastSql { get; private set; }

            public CurrentDbType CurrentDbType { get; }

            public IDbDataParameter MakeParameter(string targetFiled, object targetValue) => null!;

            public DataTable Fill(DataTable dt, string commandText, IDbDataParameter[] dbParameters, int commandTimeout = 30)
            {
                LastSql = commandText;
                return dt;
            }

            public void Dispose() { }

            public DbProviderFactory GetInstance() => throw new NotImplementedException();
            public string ConnectionName => throw new NotImplementedException();
            public bool MustCloseConnection { get; set; }
            public string ConnectionString { get; set; } = null!;
            public string ServerVersion { get; set; } = null!;
            public string GetDbNow() => throw new NotImplementedException();
            public string GetDbDateTime() => throw new NotImplementedException();
            public bool InTransaction { get; set; }
            public string SqlSafe(string value) => throw new NotImplementedException();
            public string PlusSign() => throw new NotImplementedException();
            public string PlusSign(params string[] values) => throw new NotImplementedException();
            public string GetParameter(string parameter) => throw new NotImplementedException();
            public IDbDataParameter MakeParameter(string parameterName, object parameterValue, DbType dbType, int parameterSize, ParameterDirection parameterDirection) => throw new NotImplementedException();
            public IDbDataParameter[] MakeParameters(string[] targetFields, object[] targetValues) => throw new NotImplementedException();
            public IDbDataParameter[] MakeParameters(Dictionary<string, object> parameters) => throw new NotImplementedException();
            public IDbDataParameter[] MakeParameters(List<KeyValuePair<string, object>> parameters) => throw new NotImplementedException();
            public IDbConnection GetDbConnection() => throw new NotImplementedException();
            public IDbTransaction GetDbTransaction() => throw new NotImplementedException();
            public IDbCommand GetDbCommand() => throw new NotImplementedException();
            public IDbConnection Open() => throw new NotImplementedException();
            public IDbConnection Open(string connectionString) => throw new NotImplementedException();
            public IDbTransaction BeginTransaction() => throw new NotImplementedException();
            public void CommitTransaction() => throw new NotImplementedException();
            public void RollbackTransaction() => throw new NotImplementedException();
            public void Close() => throw new NotImplementedException();
            public IDataReader ExecuteReader(string commandText, int commandTimeout = 30) => throw new NotImplementedException();
            public IDataReader ExecuteReader(string commandText, IDbDataParameter[] dbParameters, int commandTimeout = 30) => throw new NotImplementedException();
            public IDataReader ExecuteReader(string commandText, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30) => throw new NotImplementedException();
            public int ExecuteNonQuery(string commandText, int commandTimeout = 30) => throw new NotImplementedException();
            public int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters, int commandTimeout = 30) => throw new NotImplementedException();
            public int ExecuteNonQuery(string commandText, CommandType commandType, int commandTimeout = 30) => throw new NotImplementedException();
            public int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30) => throw new NotImplementedException();
            public int ExecuteNonQuery(IDbTransaction dbTransaction, string commandText, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30) => throw new NotImplementedException();
            public object ExecuteScalar(string commandText, int commandTimeout = 30) => throw new NotImplementedException();
            public object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters, int commandTimeout = 30) => throw new NotImplementedException();
            public object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30) => throw new NotImplementedException();
            public DataTable Fill(string commandText, int commandTimeout = 30) => throw new NotImplementedException();
            public DataTable Fill(DataTable dt, string commandText, int commandTimeout = 30) => throw new NotImplementedException();
            public DataTable Fill(string commandText, IDbDataParameter[] dbParameters, int commandTimeout = 30) => throw new NotImplementedException();
            public DataTable Fill(string commandText, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30) => throw new NotImplementedException();
            public DataTable Fill(DataTable dt, string commandText, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30) => throw new NotImplementedException();
            public DataSet Fill(DataSet dataSet, string commandText, string tableName, int commandTimeout = 30) => throw new NotImplementedException();
            public DataSet Fill(DataSet dataSet, string commandText, string tableName, IDbDataParameter[] dbParameters, int commandTimeout = 30) => throw new NotImplementedException();
            public DataSet Fill(DataSet dataSet, string commandText, string tableName, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30) => throw new NotImplementedException();
            public bool SqlBulkCopyData(DataTable dt, string destinationTableName, int bulkCopyTimeout = 1000, int batchSize = 0) => throw new NotImplementedException();
        }
    }
}
