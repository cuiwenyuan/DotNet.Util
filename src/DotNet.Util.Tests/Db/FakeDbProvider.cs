using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using DotNet.Util;

namespace DotNet.Util.Tests.Db
{
    internal sealed class TestDbHelper : DbHelper
    {
        public TestDbHelper()
        {
            DbConnection = new FakeDbConnection();
        }

        public override DbProviderFactory GetInstance() => FakeDbProviderFactory.Instance;

        public override string GetDbNow() => "CURRENT_TIMESTAMP";

        public override string GetDbDateTime() => "CURRENT_TIMESTAMP";

        public override string GetParameter(string parameter) => parameter;

        public override IDbDataParameter MakeParameter(string targetFiled, object targetValue)
        {
            return new FakeDbParameter { ParameterName = targetFiled, Value = targetValue };
        }

        public override IDbDataParameter[] MakeParameters(string[] targetFileds, object[] targetValues) => throw new NotSupportedException();

        public override IDbDataParameter[] MakeParameters(Dictionary<string, object> parameters) => throw new NotSupportedException();

        public override IDbDataParameter[] MakeParameters(List<KeyValuePair<string, object>> parameters)
        {
            var result = new IDbDataParameter[parameters.Count];
            for (var i = 0; i < parameters.Count; i++)
            {
                result[i] = MakeParameter(parameters[i].Key, parameters[i].Value);
            }
            return result;
        }

        public override IDbDataParameter MakeParameter(string parameterName, object parameterValue, DbType dbType, int parameterSize, ParameterDirection parameterDirection)
        {
            return new FakeDbParameter
            {
                ParameterName = parameterName,
                Value = parameterValue,
                DbType = dbType,
                Size = parameterSize,
                Direction = parameterDirection
            };
        }
    }

    internal sealed class FakeDbProviderFactory : DbProviderFactory
    {
        public static FakeDbProviderFactory Instance { get; } = new FakeDbProviderFactory();

        public override DbConnection CreateConnection() => new FakeDbConnection();

        public override DbCommand CreateCommand() => new FakeDbCommand();

        public override DbDataAdapter CreateDataAdapter() => new FakeDbDataAdapter();

        public override DbParameter CreateParameter() => new FakeDbParameter();
    }

    internal sealed class FakeDbConnection : DbConnection
    {
        private ConnectionState _state = ConnectionState.Closed;

        public FakeDbCommand? LastCommand { get; private set; }

        [AllowNull]
        public override string ConnectionString { get; set; } = string.Empty;

        public override string Database => "Test";

        public override string DataSource => "Test";

        public override string ServerVersion => "1.0";

        public override ConnectionState State => _state;

        public override void ChangeDatabase(string databaseName)
        {
        }

        public override void Close() => _state = ConnectionState.Closed;

        public override void Open() => _state = ConnectionState.Open;

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotSupportedException();

        protected override DbCommand CreateDbCommand()
        {
            LastCommand = new FakeDbCommand(this);
            return LastCommand;
        }
    }

    internal sealed class FakeDbCommand : DbCommand
    {
        private readonly FakeDbParameterCollection _parameters = new FakeDbParameterCollection();
        private DbConnection? _connection;
        private DbTransaction? _transaction;

        public FakeDbCommand()
        {
        }

        public FakeDbCommand(DbConnection connection)
        {
            _connection = connection;
        }

        [AllowNull]
        public override string CommandText { get; set; } = string.Empty;

        public override int CommandTimeout { get; set; }

        public override CommandType CommandType { get; set; }

        public override UpdateRowSource UpdatedRowSource { get; set; }

        public override bool DesignTimeVisible { get; set; }

        protected override DbConnection? DbConnection
        {
            get => _connection;
            set => _connection = value;
        }

        protected override DbParameterCollection DbParameterCollection => _parameters;

        protected override DbTransaction? DbTransaction
        {
            get => _transaction;
            set => _transaction = value;
        }

        public override void Cancel()
        {
        }

        public override int ExecuteNonQuery() => 1;

        public override object ExecuteScalar() => 1;

        public override void Prepare()
        {
        }

        protected override DbParameter CreateDbParameter() => new FakeDbParameter();

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            var table = new DataTable();
            table.Columns.Add("Value", typeof(int));
            table.Rows.Add(1);
            return table.CreateDataReader();
        }
    }

    internal sealed class FakeDbDataAdapter : DbDataAdapter
    {
    }

    internal sealed class FakeDbParameter : DbParameter, ICloneable
    {
        public override DbType DbType { get; set; }

        public override ParameterDirection Direction { get; set; }

        public override bool IsNullable { get; set; }

        [AllowNull]
        public override string ParameterName { get; set; } = string.Empty;

        [AllowNull]
        public override string SourceColumn { get; set; } = string.Empty;

        public override object? Value { get; set; }

        public override bool SourceColumnNullMapping { get; set; }

        public override int Size { get; set; }

        public override void ResetDbType()
        {
        }

        public object Clone() => MemberwiseClone();
    }

    internal sealed class FakeDbParameterCollection : DbParameterCollection
    {
        private readonly List<DbParameter> _parameters = new List<DbParameter>();

        public override int Count => _parameters.Count;

        public override object SyncRoot => ((ICollection)_parameters).SyncRoot;

        public override int Add(object value)
        {
            _parameters.Add((DbParameter)value);
            return _parameters.Count - 1;
        }

        public override void AddRange(Array values)
        {
            foreach (var value in values)
            {
                Add(value);
            }
        }

        public override void Clear() => _parameters.Clear();

        public override bool Contains(object value) => _parameters.Contains((DbParameter)value);

        public override bool Contains(string value) => IndexOf(value) >= 0;

        public override void CopyTo(Array array, int index) => ((ICollection)_parameters).CopyTo(array, index);

        public override IEnumerator GetEnumerator() => _parameters.GetEnumerator();

        public override int IndexOf(object value) => _parameters.IndexOf((DbParameter)value);

        public override int IndexOf(string parameterName) => _parameters.FindIndex(parameter => parameter.ParameterName == parameterName);

        public override void Insert(int index, object value) => _parameters.Insert(index, (DbParameter)value);

        public override void Remove(object value) => _parameters.Remove((DbParameter)value);

        public override void RemoveAt(int index) => _parameters.RemoveAt(index);

        public override void RemoveAt(string parameterName) => _parameters.RemoveAt(IndexOf(parameterName));

        protected override DbParameter GetParameter(int index) => _parameters[index];

        protected override DbParameter GetParameter(string parameterName) => _parameters[IndexOf(parameterName)];

        protected override void SetParameter(int index, DbParameter value) => _parameters[index] = value;

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var index = IndexOf(parameterName);
            if (index < 0)
            {
                _parameters.Add(value);
            }
            else
            {
                _parameters[index] = value;
            }
        }
    }
}
