using System.Data;
using System.Data.Common;
using DotNet.Model;
using DotNet.Util;
using Xunit;
using DbParameter = DotNet.Model.DbParameter;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// SqlExecute 测试（纯逻辑：构造、参数集合、桩 IDbHelper 生成参数）
    /// </summary>
    public class SqlExecuteTests
    {
        private sealed class StubParameter : IDbDataParameter
        {
            public StubParameter(string name, object value)
            {
                ParameterName = name;
                Value = value;
            }
            public byte Precision { get; set; }
            public byte Scale { get; set; }
            public int Size { get; set; }
            public DbType DbType { get; set; }
            public ParameterDirection Direction { get; set; }
            public bool IsNullable => true;
            public string? SourceColumn { get; set; }
            public DataRowVersion SourceVersion { get; set; }
            public string ParameterName { get; set; }
            public object? Value { get; set; }
        }

        private sealed class StubDbHelper : IDbHelper
        {
            public CurrentDbType CurrentDbType => CurrentDbType.SqlServer;
            public IDbDataParameter MakeParameter(string targetFiled, object targetValue) => new StubParameter(targetFiled, targetValue);
            public void Dispose() { }
            public DbProviderFactory GetInstance() => throw new System.NotImplementedException();
            public string ConnectionName => throw new System.NotImplementedException();
            public bool MustCloseConnection { get; set; }
            public string ConnectionString { get; set; } = null!;
            public string ServerVersion { get; set; } = null!;
            public string GetDbNow() => throw new System.NotImplementedException();
            public string GetDbDateTime() => throw new System.NotImplementedException();
            public bool InTransaction { get; set; }
            public string SqlSafe(string value) => throw new System.NotImplementedException();
            public string PlusSign() => throw new System.NotImplementedException();
            public string PlusSign(params string[] values) => throw new System.NotImplementedException();
            public string GetParameter(string parameter) => throw new System.NotImplementedException();
            public IDbDataParameter MakeParameter(string parameterName, object parameterValue, DbType dbType, int parameterSize, ParameterDirection parameterDirection) => throw new System.NotImplementedException();
            public IDbDataParameter[] MakeParameters(string[] targetFields, object[] targetValues) => throw new System.NotImplementedException();
            public IDbDataParameter[] MakeParameters(Dictionary<string, object> parameters) => throw new System.NotImplementedException();
            public IDbDataParameter[] MakeParameters(List<KeyValuePair<string, object>> parameters) => throw new System.NotImplementedException();
            public IDbConnection GetDbConnection() => throw new System.NotImplementedException();
            public IDbTransaction GetDbTransaction() => throw new System.NotImplementedException();
            public IDbCommand GetDbCommand() => throw new System.NotImplementedException();
            public IDbConnection Open() => throw new System.NotImplementedException();
            public IDbConnection Open(string connectionString) => throw new System.NotImplementedException();
            public IDbTransaction BeginTransaction() => throw new System.NotImplementedException();
            public void CommitTransaction() => throw new System.NotImplementedException();
            public void RollbackTransaction() => throw new System.NotImplementedException();
            public void Close() => throw new System.NotImplementedException();
            public IDataReader ExecuteReader(string commandText, int commandTimeout = 30) => throw new System.NotImplementedException();
            public IDataReader ExecuteReader(string commandText, IDbDataParameter[] dbParameters, int commandTimeout = 30) => throw new System.NotImplementedException();
            public IDataReader ExecuteReader(string commandText, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30) => throw new System.NotImplementedException();
            public int ExecuteNonQuery(string commandText, int commandTimeout = 30) => throw new System.NotImplementedException();
            public int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters, int commandTimeout = 30) => throw new System.NotImplementedException();
            public int ExecuteNonQuery(string commandText, CommandType commandType, int commandTimeout = 30) => throw new System.NotImplementedException();
            public int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30) => throw new System.NotImplementedException();
            public int ExecuteNonQuery(IDbTransaction dbTransaction, string commandText, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30) => throw new System.NotImplementedException();
            public object ExecuteScalar(string commandText, int commandTimeout = 30) => throw new System.NotImplementedException();
            public object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters, int commandTimeout = 30) => throw new System.NotImplementedException();
            public object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30) => throw new System.NotImplementedException();
            public DataTable Fill(string commandText, int commandTimeout = 30) => throw new System.NotImplementedException();
            public DataTable Fill(DataTable dt, string commandText, int commandTimeout = 30) => throw new System.NotImplementedException();
            public DataTable Fill(string commandText, IDbDataParameter[] dbParameters, int commandTimeout = 30) => throw new System.NotImplementedException();
            public DataTable Fill(string commandText, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30) => throw new System.NotImplementedException();
            public DataTable Fill(DataTable dt, string commandText, IDbDataParameter[] dbParameters, int commandTimeout = 30) => throw new System.NotImplementedException();
            public DataTable Fill(DataTable dt, string commandText, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30) => throw new System.NotImplementedException();
            public DataSet Fill(DataSet dataSet, string commandText, string tableName, int commandTimeout = 30) => throw new System.NotImplementedException();
            public DataSet Fill(DataSet dataSet, string commandText, string tableName, IDbDataParameter[] dbParameters, int commandTimeout = 30) => throw new System.NotImplementedException();
            public DataSet Fill(DataSet dataSet, string commandText, string tableName, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30) => throw new System.NotImplementedException();
            public bool SqlBulkCopyData(DataTable dt, string destinationTableName, int bulkCopyTimeout = 1000, int batchSize = 0) => throw new System.NotImplementedException();
        }

        [Fact]
        public void Ctor_Default_EmptyCommandTextTextType()
        {
            var execute = new SqlExecute();

            Assert.Equal("", execute.CommandText);
            Assert.Equal(CommandType.Text, execute.CommandType);
        }

        [Fact]
        public void Ctor_WithCommandText()
        {
            var execute = new SqlExecute("SELECT 1");

            Assert.Equal("SELECT 1", execute.CommandText);
            Assert.Equal(CommandType.Text, execute.CommandType);
        }

        [Fact]
        public void Ctor_WithCommandType()
        {
            var execute = new SqlExecute("sp_Test", CommandType.StoredProcedure);

            Assert.Equal("sp_Test", execute.CommandText);
            Assert.Equal(CommandType.StoredProcedure, execute.CommandType);
        }

        [Fact]
        public void Ctor_WithDbParameterArray()
        {
            var execute = new SqlExecute("SELECT * FROM T WHERE Id=@id",
                new[] { new DbParameter("@id", 1) },
                CommandType.Text);

            Assert.Equal(1, execute.GetParameters(new StubDbHelper()).Length);
            Assert.Equal(1, execute.GetValueAt(0));
        }

        [Fact]
        public void Ctor_WithAnonymousObject_BuildsParameters()
        {
            var execute = new SqlExecute("INSERT", new { Name = "Troy", Age = 30 }, CommandType.Text);

            var parameters = execute.GetParameters(new StubDbHelper());

            Assert.Equal(2, parameters.Length);
        }

        [Fact]
        public void AddParameter_AppendsAndReturnsThis()
        {
            var execute = new SqlExecute("SELECT 1");

            var result = execute.AddParameter("@a", 1, ParameterDirection.Input);

            Assert.Same(execute, result);
            Assert.Equal(1, execute.GetParameters(new StubDbHelper()).Length);
        }

        [Fact]
        public void SetValueAt_UpdatesExistingParameter()
        {
            var execute = new SqlExecute("SELECT 1");
            execute.AddParameter("@a", 1, ParameterDirection.Input);

            execute.SetValueAt(0, 99);

            Assert.Equal(99, execute.GetValueAt(0));
        }

        [Fact]
        public void GetParameters_NoParameters_ReturnsNull()
        {
            var execute = new SqlExecute("SELECT 1");

            Assert.Null(execute.GetParameters(new StubDbHelper()));
        }
    }
}
