using System.Data;
using System.Data.Common;
using DotNet.Model;
using DotNet.Util;
using Xunit;
using DbParameter = DotNet.Model.DbParameter;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// DbParameter 测试（构造、属性、GetIDbDataParameter 桩验证）
    /// </summary>
    public class DbParameterTests
    {
        private sealed class StubDbHelper : IDbHelper
        {
            public IDbDataParameter? LastParameter { get; private set; }

            public CurrentDbType CurrentDbType => CurrentDbType.SqlServer;

            public IDbDataParameter MakeParameter(string targetFiled, object targetValue)
            {
                LastParameter = new StubParameter(targetFiled, targetValue);
                return LastParameter;
            }

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

        [Fact]
        public void Ctor_TwoArgs_DefaultsToInputDirection()
        {
            var parameter = new DbParameter("@name", "Troy");

            Assert.Equal("@name", parameter.Name);
            Assert.Equal("Troy", parameter.Value);
            Assert.Equal(ParameterDirection.Input, parameter.ParameterDirection);
        }

        [Fact]
        public void Ctor_ThreeArgs_SetsDirection()
        {
            var parameter = new DbParameter("@id", 42, ParameterDirection.Output);

            Assert.Equal("@id", parameter.Name);
            Assert.Equal(42, parameter.Value);
            Assert.Equal(ParameterDirection.Output, parameter.ParameterDirection);
        }

        [Fact]
        public void Properties_AreMutable()
        {
            var parameter = new DbParameter("a", 1);

            parameter.Name = "b";
            parameter.Value = 2;
            parameter.ParameterDirection = ParameterDirection.ReturnValue;

            Assert.Equal("b", parameter.Name);
            Assert.Equal(2, parameter.Value);
            Assert.Equal(ParameterDirection.ReturnValue, parameter.ParameterDirection);
        }

        [Fact]
        public void GetIDbDataParameter_UsesHelperAndSetsDirection()
        {
            var helper = new StubDbHelper();
            var parameter = new DbParameter("@code", "X", ParameterDirection.InputOutput);

            var result = parameter.GetIDbDataParameter(helper);

            Assert.Same(helper.LastParameter, result);
            Assert.Equal("X", result.Value);
            Assert.Equal(ParameterDirection.InputOutput, result.Direction);
        }
    }
}
