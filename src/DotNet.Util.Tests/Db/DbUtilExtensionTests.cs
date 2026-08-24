using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DotNet.Model;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    /// DbUtil IDbHelper 扩展方法测试（桩驱动，验证 SQL 拼接逻辑与返回值路径，不连库）
    /// </summary>
    public class DbUtilExtensionTests
    {
        [Fact]
        public void Count_SqlServer_BuildsSql()
        {
            var stub = new StubDbHelper { ScalarResult = 5 };
            var result = stub.Count("Orders");

            Assert.Equal(5, result);
            Assert.Equal("SELECT COUNT(*) FROM Orders", stub.LastSql);
        }

        [Fact]
        public void Count_WithCondition_AppendsWhere()
        {
            var stub = new StubDbHelper { ScalarResult = 2 };
            stub.Count("Orders", "Enabled = 1");

            Assert.Equal("SELECT COUNT(*) FROM Orders WHERE Enabled = 1", stub.LastSql);
        }

        [Fact]
        public void DistinctCount_BuildsSql()
        {
            var stub = new StubDbHelper { ScalarResult = 3 };
            stub.DistinctCount("Orders", "UserId");

            Assert.Equal("SELECT COUNT(DISTINCT UserId) FROM Orders", stub.LastSql);
        }

        [Fact]
        public void AggregateInt_SqlServer_IsNull()
        {
            var stub = new StubDbHelper { ScalarResult = 10 };
            var result = stub.AggregateInt("Orders", "Amount");

            Assert.Equal(10, result);
            Assert.Equal("SELECT ISNULL(SUM(Amount),0) FROM Orders", stub.LastSql);
        }

        [Fact]
        public void AggregateInt_MySql_IfNull()
        {
            var stub = new StubDbHelper(CurrentDbType.MySql) { ScalarResult = 1 };
            stub.AggregateInt("Orders", "Amount", "UserId = 1");

            Assert.Equal("SELECT IFNULL(SUM(Amount),0) FROM Orders WHERE UserId = 1", stub.LastSql);
        }

        [Fact]
        public void AggregateInt_Oracle_Nvl()
        {
            var stub = new StubDbHelper(CurrentDbType.Oracle) { ScalarResult = 1 };
            stub.AggregateInt("Orders", "Amount");

            Assert.Equal("SELECT NVL(SUM(Amount),0) FROM Orders", stub.LastSql);
        }

        [Fact]
        public void AggregateDecimal_SqlServer()
        {
            var stub = new StubDbHelper { ScalarResult = 12.5m };
            var result = stub.AggregateDecimal("Orders", "Amount", function: "AVG");

            Assert.Equal(12.5m, result);
            Assert.Equal("SELECT ISNULL(AVG(Amount),0) FROM Orders", stub.LastSql);
        }

        [Fact]
        public void AggregateDateTime_Oracle()
        {
            var stub = new StubDbHelper(CurrentDbType.Oracle) { ScalarResult = DateTime.Now };
            stub.AggregateDateTime("Orders", "CreateTime");

            Assert.Equal("SELECT NVL(MIN(CreateTime),0) FROM Orders", stub.LastSql);
        }

        [Fact]
        public void Delete_NoParameters()
        {
            var stub = new StubDbHelper { NonQueryResult = 3 };
            var result = stub.Delete("Orders");

            Assert.Equal(3, result);
            Assert.Equal("DELETE FROM Orders", stub.LastSql);
        }

        [Fact]
        public void Delete_WithParameters()
        {
            var stub = new StubDbHelper { NonQueryResult = 1 };
            stub.Delete("Orders", new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>("Id", 5) });

            Assert.Equal("DELETE FROM Orders WHERE Id = @Id", stub.LastSql);
        }

        [Fact]
        public void Truncate_SqlServer()
        {
            var stub = new StubDbHelper { NonQueryResult = 0 };
            stub.Truncate("Orders");

            Assert.Equal("TRUNCATE TABLE Orders", stub.LastSql);
        }

        [Fact]
        public void TableExists_SqlServer_Sql()
        {
            var stub = new StubDbHelper { ScalarResult = 1 };
            var result = stub.TableExists("Orders");

            Assert.True(result);
            Assert.Contains("sysobjects", stub.LastSql);
        }

        [Fact]
        public void TableExists_Oracle_UpperName()
        {
            var stub = new StubDbHelper(CurrentDbType.Oracle) { ScalarResult = 1 };
            stub.TableExists("orders");

            Assert.Contains("User_tables", stub.LastSql);
            Assert.Contains("'ORDERS'", stub.LastSql);
        }

        [Fact]
        public void TableExists_Unsupported_ReturnsFalse()
        {
            var stub = new StubDbHelper(CurrentDbType.PostgreSql);
            Assert.False(stub.TableExists("Orders"));
        }

        [Fact]
        public void GetProperties_EmptyValues_ReturnsEmpty()
        {
            var stub = new StubDbHelper();
            var result = stub.GetProperties("UserInfo", "Id", new object[0], "Id");

            Assert.Empty(result);
            Assert.Null(stub.LastSql); // 未执行 SQL
        }

        [Fact]
        public void GetProperties_WithValues_BuildsInSql()
        {
            var stub = new StubDbHelper();
            var dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Rows.Add("a");
            dt.Rows.Add("b");
            stub.FillTable = dt;

            var result = stub.GetProperties("UserInfo", "Name", new object[] { "a", "b" }, "Name");

            Assert.Equal(new[] { "a", "b" }, result);
            Assert.Contains("SELECT Name FROM UserInfo", stub.LastSql);
            Assert.Contains("IN (@Value0,@Value1)", stub.LastSql);
        }

        [Fact]
        public void GetProperties_TopLimit_SqlServer_Top()
        {
            var stub = new StubDbHelper();
            var parameters = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>("Enabled", 1) };
            stub.ExecuteReaderResult = null; // 无 reader，返回空
            stub.GetProperties("UserInfo", parameters, 5, "Id");

            // TOP 分支会 Clear 掉前面的 DISTINCT，实际为 SELECT TOP 5
            Assert.StartsWith("SELECT TOP 5 Id FROM UserInfo", stub.LastSql);
        }

        [Fact]
        public void GetProperties_TopLimit_MySql_Limit()
        {
            var stub = new StubDbHelper(CurrentDbType.MySql);
            var parameters = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>("Enabled", 1) };
            stub.ExecuteReaderResult = null;
            stub.GetProperties("UserInfo", parameters, 5, "Id");

            Assert.Contains("LIMIT 0, 5", stub.LastSql);
        }

        [Fact]
        public void GetProperties_TopLimit_Oracle_Rownum()
        {
            var stub = new StubDbHelper(CurrentDbType.Oracle);
            var parameters = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>("Enabled", 1) };
            stub.ExecuteReaderResult = null;
            stub.GetProperties("UserInfo", parameters, 5, "Id");

            Assert.Contains("ROWNUM <= 5", stub.LastSql);
        }

        [Fact]
        public void GetWhereString_SimpleEquals()
        {
            var stub = new StubDbHelper();
            var sql = DbUtil.GetWhereString(stub, new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Id", 1)
            }, " AND ");

            Assert.Equal("Id = @Id", sql);
        }

        [Fact]
        public void GetWhereString_NullValue_IsNull()
        {
            var stub = new StubDbHelper();
            var sql = DbUtil.GetWhereString(stub, new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Name", null)
            }, " AND ");

            Assert.Equal("Name IS NULL", sql);
        }

        [Fact]
        public void GetWhereString_Enumerable_InList()
        {
            var stub = new StubDbHelper();
            var sql = DbUtil.GetWhereString(stub, new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Id", new[] { 1, 2, 3 })
            }, " AND ");

            Assert.Equal("Id IN ('1','2','3')", sql);
        }

        [Fact]
        public void GetWhereString_EmptyEnumerable_IsNull()
        {
            var stub = new StubDbHelper();
            var sql = DbUtil.GetWhereString(stub, new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Id", new int[0])
            }, " AND ");

            Assert.Equal("Id IS NULL", sql);
        }

        [Fact]
        public void GetWhereString_Multiple_JoinedByRelation()
        {
            var stub = new StubDbHelper();
            var sql = DbUtil.GetWhereString(stub, new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Id", 1),
                new KeyValuePair<string, object>("Enabled", 1)
            }, " AND ");

            Assert.Equal("Id = @Id AND Enabled = @Enabled", sql);
        }

        [Fact]
        public void GetWhereString_Null_ReturnsEmpty()
        {
            var stub = new StubDbHelper();
            Assert.Equal(string.Empty, DbUtil.GetWhereString(stub, null, " AND "));
        }

        #region GetDataTable

        [Fact]
        public void GetDataTable_NoValues_IsNull()
        {
            var stub = new StubDbHelper();
            stub.GetDataTable("UserInfo", "Id", new object[0]);

            Assert.Equal("SELECT * FROM UserInfo  WHERE Id IS NULL", stub.LastSql);
        }

        [Fact]
        public void GetDataTable_WithValues_InList()
        {
            var stub = new StubDbHelper();
            stub.GetDataTable("UserInfo", "Id", new object[] { 1, 2 });

            Assert.Equal("SELECT * FROM UserInfo  WHERE Id IN ('1','2')", stub.LastSql);
        }

        [Fact]
        public void GetDataTable_WithValues_SqlSafeEscapes()
        {
            var stub = new StubDbHelper();
            stub.GetDataTable("UserInfo", "Name", new object[] { "O'Brien" });

            // SqlSafe 将单引号转义为双单引号
            Assert.Equal("SELECT * FROM UserInfo  WHERE Name IN ('O''Brien')", stub.LastSql);
        }

        [Fact]
        public void GetDataTable_WithOrder()
        {
            var stub = new StubDbHelper();
            stub.GetDataTable("UserInfo", "Id", new object[] { 1 }, "Id DESC");

            Assert.EndsWith("ORDER BY Id DESC", stub.LastSql);
        }

        #endregion

        #region GetCount / Exists

        [Fact]
        public void GetCount_WithParameters()
        {
            var stub = new StubDbHelper { ScalarResult = 3 };
            var result = stub.GetCount("UserInfo", new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Enabled", 1)
            });

            Assert.Equal(3, result);
            Assert.Contains("SELECT COUNT(*) FROM UserInfo", stub.LastSql);
            Assert.Contains("Enabled = @Enabled", stub.LastSql);
        }

        [Fact]
        public void GetCount_WithCondition()
        {
            var stub = new StubDbHelper { ScalarResult = 1 };
            stub.GetCount("UserInfo", "Enabled = 1");

            Assert.Equal("SELECT  COUNT(*) FROM UserInfo WHERE Enabled = 1", stub.LastSql);
        }

        #endregion

        #region SqlSafe / GetSafeSortDirection

        [Fact]
        public void SqlSafe_EscapesSingleQuote()
        {
            // 仅转义单引号（防 SQL 注入主要手段）
            var result = DbUtil.SqlSafe("O'Brien");

            Assert.Equal("O''Brien", result);
        }

        [Fact]
        public void SqlSafe_KeepsNormalText()
        {
            var result = DbUtil.SqlSafe("Hello World 中文");

            Assert.Equal("Hello World 中文", result);
        }

        [Fact]
        public void SqlSafe_NullOrEmpty_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, DbUtil.SqlSafe(null));
            Assert.Equal(string.Empty, DbUtil.SqlSafe(""));
        }

        [Fact]
        public void GetSafeSortDirection_Asc_ReturnsAsc()
        {
            Assert.Equal("ASC", DbUtil.GetSafeSortDirection("ASC"));
            Assert.Equal("ASC", DbUtil.GetSafeSortDirection("asc"));
        }

        [Fact]
        public void GetSafeSortDirection_Desc_ReturnsDesc()
        {
            Assert.Equal("DESC", DbUtil.GetSafeSortDirection("DESC"));
            Assert.Equal("DESC", DbUtil.GetSafeSortDirection("desc"));
        }

        [Fact]
        public void GetSafeSortDirection_Invalid_FallsBackToDesc()
        {
            Assert.Equal("DESC", DbUtil.GetSafeSortDirection("INJECTION"));
            Assert.Equal("DESC", DbUtil.GetSafeSortDirection(""));
            Assert.Equal("DESC", DbUtil.GetSafeSortDirection(null));
        }

        #endregion

        #region GetSafeSortExpression / ExecuteReaderByPage

        [Fact]
        public void GetSafeSortExpression_Valid_ReturnsAsIs()
        {
            Assert.Equal("CreateTime", DbUtil.GetSafeSortExpression("CreateTime"));
            Assert.Equal("[UserInfo].Id", DbUtil.GetSafeSortExpression("[UserInfo].Id"));
            Assert.Equal("a, b", DbUtil.GetSafeSortExpression("a, b"));
        }

        [Fact]
        public void GetSafeSortExpression_Empty_FallsBack()
        {
            Assert.Equal("CreateTime", DbUtil.GetSafeSortExpression(""));
            Assert.Equal("CreateTime", DbUtil.GetSafeSortExpression(null));
            Assert.Equal("SortCode", DbUtil.GetSafeSortExpression(null, "SortCode"));
        }

        [Fact]
        public void GetSafeSortExpression_Injection_FallsBack()
        {
            Assert.Equal("CreateTime", DbUtil.GetSafeSortExpression("Id; DROP TABLE Users"));
            Assert.Equal("CreateTime", DbUtil.GetSafeSortExpression("Id--comment"));
            Assert.Equal("CreateTime", DbUtil.GetSafeSortExpression("Id/*x*/"));
            Assert.Equal("CreateTime", DbUtil.GetSafeSortExpression("Id'"));
        }

        [Fact]
        public void ExecuteReaderByPage_UsesStoredProcedure()
        {
            var stub = new StubDbHelper { ScalarResult = 0 };

            stub.ExecuteReaderByPage(out var recordCount, 2, 10, "Id", "ASC", "UserInfo", "Enabled = 1", "Id, Name");

            Assert.Equal(0, recordCount);
            Assert.Equal("GetRecordByPage", stub.LastSql);
            Assert.Equal(CommandType.StoredProcedure, stub.LastCommandType);
        }

        #endregion

        #region GetDataTableByPage

        [Fact]
        public void GetDataTableByPage_SqlServer_RowNumber()
        {
            var stub = new StubDbHelper();
            stub.GetDataTableByPage(100, 2, 10, "SELECT * FROM UserInfo", "", null, "Id", "ASC");

            Assert.Contains("ROW_NUMBER() OVER(ORDER BY Id ASC)", stub.LastSql);
            Assert.Contains("ROWNUM > 10 AND ROWNUM <= 20", stub.LastSql);
        }

        [Fact]
        public void GetDataTableByPage_SqlServer_WithCondition()
        {
            var stub = new StubDbHelper();
            stub.GetDataTableByPage(100, 1, 10, "SELECT * FROM UserInfo", "Enabled = 1", null);

            Assert.Contains("WHERE Enabled = 1", stub.LastSql);
        }

        [Fact]
        public void GetDataTableByPage_MySql_Limit()
        {
            var stub = new StubDbHelper(CurrentDbType.MySql);
            stub.GetDataTableByPage(100, 3, 10, "SELECT * FROM UserInfo", "", null, "Id", "DESC");

            Assert.Equal("SELECT * FROM  (SELECT * FROM UserInfo)  ORDER BY Id DESC LIMIT 20,10", stub.LastSql);
        }

        [Fact]
        public void GetDataTableByPage_Sqlite_Limit()
        {
            var stub = new StubDbHelper(CurrentDbType.SQLite);
            stub.GetDataTableByPage(100, 2, 10, "SELECT * FROM UserInfo", "", null);

            Assert.Contains("LIMIT 10,10", stub.LastSql);
        }

        [Fact]
        public void GetDataTableByPage_Oracle_Rownum()
        {
            var stub = new StubDbHelper(CurrentDbType.Oracle);
            stub.GetDataTableByPage(100, 2, 10, "SELECT * FROM UserInfo", "", null, "Id", "ASC");

            Assert.Contains("ROWNUM RN", stub.LastSql);
            Assert.Contains("WHERE RN > 10", stub.LastSql);
            Assert.Contains("ROWNUM <= 20", stub.LastSql);
        }

        [Fact]
        public void GetDataTableByPage_PostgreSql_Offset()
        {
            var stub = new StubDbHelper(CurrentDbType.PostgreSql);
            stub.GetDataTableByPage(100, 2, 10, "SELECT * FROM UserInfo", "", null, "Id", "ASC");

            Assert.Contains("LIMIT 10 OFFSET 10", stub.LastSql);
        }

        [Fact]
        public void GetDataTableByPage_Access_Top()
        {
            var stub = new StubDbHelper(CurrentDbType.Access);
            stub.GetDataTableByPage(100, 1, 10, "SELECT * FROM UserInfo", "", null, "Id", "ASC");

            Assert.Contains("SELECT TOP", stub.LastSql);
        }

        [Fact]
        public void GetDataTableByPage_Db2_RowNumber()
        {
            var stub = new StubDbHelper(CurrentDbType.Db2);
            stub.GetDataTableByPage(100, 1, 10, "SELECT * FROM UserInfo", "", null, "Id", "ASC");

            Assert.Contains("ROW_NUMBER() OVER(ORDER BY Id ASC)", stub.LastSql);
        }

        #endregion

        #region ExecuteReader (表名重载)

        [Fact]
        public void ExecuteReader_NoValues_IsNull()
        {
            var stub = new StubDbHelper();
            stub.ExecuteReader("UserInfo", "Id", new object[0]);

            Assert.Equal("SELECT * FROM UserInfo WHERE Id IS NULL", stub.LastSql);
        }

        [Fact]
        public void ExecuteReader_WithValues_InList()
        {
            var stub = new StubDbHelper();
            stub.ExecuteReader("UserInfo", "Id", new object[] { 1, 2 });

            Assert.Equal("SELECT * FROM UserInfo WHERE Id IN (@value0,@value1)", stub.LastSql);
        }

        [Fact]
        public void ExecuteReader_WithOrder()
        {
            var stub = new StubDbHelper();
            stub.ExecuteReader("UserInfo", "Id", new object[] { 1 }, "Id DESC");

            Assert.EndsWith("ORDER BY Id DESC", stub.LastSql);
        }

        [Fact]
        public void ExecuteReader_WithNullInValues_Skips()
        {
            var stub = new StubDbHelper();
            stub.ExecuteReader("UserInfo", "Id", new object[] { 1, null, 2 });

            // null 值也生成参数（实现未跳过），但至少不崩溃
            Assert.Contains("IN (@value0,@value1,@value2)", stub.LastSql);
        }

        #endregion

        #region ExecuteReader<TModel> 泛型

        [Fact]
        public void ExecuteReader_Generic_ReturnsModels()
        {
            var stub = new StubDbHelper();
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Rows.Add(1, "Troy");
            dt.Rows.Add(2, "Cui");
            stub.ExecuteReaderResult = dt.CreateDataReader();

            var list = stub.ExecuteReader<UserRow>("UserInfo", "Id", new object[] { 1, 2 });

            Assert.Equal(2, list.Count);
            Assert.Equal("Troy", list[0].Name);
            Assert.Equal(2, list[1].Id);
        }

        /// <summary>
        /// ToList&lt;T&gt; 需要 T 实现 GetFrom(IDataReader)（dynamic 绑定）
        /// </summary>
        /// <summary>
        /// ToList&lt;T&gt; 需要 public 类 + public GetFrom(IDataReader)（dynamic 绑定）
        /// </summary>
        public class UserRow
        {
            public int Id { get; set; }
            public string? Name { get; set; }

            public UserRow GetFrom(IDataReader dr)
            {
                Id = dr["Id"].ToInt();
                Name = dr["Name"] as string;
                return this;
            }
        }
        #endregion

        #region GetProperty

        [Fact]
        public void GetProperty_DefaultTop_SqlServer()
        {
            var stub = new StubDbHelper();
            stub.GetProperty("UserInfo", new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Id", 1)
            }, "Name");

            Assert.StartsWith("SELECT TOP 1 Name FROM UserInfo", stub.LastSql);
            Assert.Contains("Id = @Id", stub.LastSql);
        }

        [Fact]
        public void GetProperty_TopLimit_MySql_Limit()
        {
            var stub = new StubDbHelper(CurrentDbType.MySql);
            stub.GetProperty("UserInfo", new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Id", 1)
            }, "Name", 5);

            Assert.Contains("LIMIT 0, 5", stub.LastSql);
        }

        [Fact]
        public void GetProperty_WithOrder()
        {
            var stub = new StubDbHelper();
            stub.GetProperty("UserInfo", new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Id", 1)
            }, "Name", 1, "CreateTime DESC");

            Assert.Contains("ORDER BY CreateTime DESC", stub.LastSql);
        }

        [Fact]
        public void GetProperty_EmptyTargetField_DefaultsToId()
        {
            var stub = new StubDbHelper();
            stub.GetProperty("UserInfo", new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Id", 1)
            }, "");

            Assert.StartsWith("SELECT TOP 1 Id FROM UserInfo", stub.LastSql);
        }

        #endregion

        #region IsUpdate

        [Fact]
        public void IsUpdate_DataRow_NoUpdateFields_ReturnsFalse()
        {
            var dr = CreateUpdateRow(null, null);
            Assert.False(DbUtil.IsUpdate(dr, "user", DateTime.Now.AddDays(-1)));
        }

        [Fact]
        public void IsUpdate_DataRow_NewerTime_ReturnsTrue()
        {
            var dr = CreateUpdateRow("user", DateTime.Now);
            Assert.True(DbUtil.IsUpdate(dr, "user", DateTime.Now.AddDays(-1)));
        }

        [Fact]
        public void IsUpdate_DataRow_DifferentUser_ReturnsTrue()
        {
            var dr = CreateUpdateRow("other", DateTime.Now);
            Assert.True(DbUtil.IsUpdate(dr, "user", DateTime.Now.AddHours(1)));
        }

        [Fact]
        public void IsUpdate_DataRow_SameUserAndOlderTime_ReturnsFalse()
        {
            var dr = CreateUpdateRow("user", DateTime.Now.AddDays(-2));
            Assert.False(DbUtil.IsUpdate(dr, "user", DateTime.Now.AddDays(-1)));
        }

        [Fact]
        public void IsUpdate_DbHelper_BuildsSqlAndUsesFill()
        {
            var stub = new StubDbHelper();
            var dt = new DataTable();
            dt.Columns.Add(BaseUtil.FieldUpdateUserId, typeof(string));
            dt.Columns.Add(BaseUtil.FieldUpdateTime, typeof(DateTime));
            dt.Rows.Add("user", DateTime.Now);
            stub.FillTable = dt;

            var result = stub.IsUpdate("UserInfo", "Id", 5, "user", DateTime.Now.AddDays(-1));

            Assert.True(result);
            Assert.Contains("SELECT Id,CreateUserId,CreateTime,UpdateUserId,UpdateTime FROM UserInfo", stub.LastSql);
        }

        private static DataRow CreateUpdateRow(string? updateUserId, DateTime? updateTime)
        {
            var dt = new DataTable();
            dt.Columns.Add(BaseUtil.FieldUpdateUserId, typeof(string));
            dt.Columns.Add(BaseUtil.FieldUpdateTime, typeof(DateTime));
            var dr = dt.NewRow();
            dr[BaseUtil.FieldUpdateUserId] = (object?)updateUserId ?? DBNull.Value;
            dr[BaseUtil.FieldUpdateTime] = (object?)updateTime ?? DBNull.Value;
            dt.Rows.Add(dr);
            return dr;
        }

        #endregion

        #region Ado (CloseConnection / Reopen)

        [Fact]
        public void CloseConnection_Null_DoesNotThrow()
        {
            IDbConnection? conn = null;
            DbUtil.CloseConnection(conn);
        }

        [Fact]
        public void CloseConnection_Closes()
        {
            var conn = new FakeDbConnection();
            DbUtil.CloseConnection(conn);

            Assert.True(conn.IsClosed);
        }

        [Fact]
        public void Reopen_Null_DoesNotThrow()
        {
            IDbConnection? conn = null;
            DbUtil.Reopen(conn);
        }

        [Fact]
        public void Reopen_ClosesThenOpens()
        {
            var conn = new FakeDbConnection { IsClosed = true };
            DbUtil.Reopen(conn);

            Assert.False(conn.IsClosed);
            Assert.True(conn.OpenCount > 0);
        }

        private sealed class FakeDbConnection : IDbConnection
        {
            public bool IsClosed { get; set; } = true;
            public int OpenCount { get; private set; }
            public string ConnectionString { get; set; } = string.Empty;
            public int ConnectionTimeout => 15;
            public string Database => "test";
            public ConnectionState State => IsClosed ? ConnectionState.Closed : ConnectionState.Open;

            public void Close() => IsClosed = true;
            public void Open() { IsClosed = false; OpenCount++; }
            public IDbTransaction BeginTransaction() => throw new NotImplementedException();
            public IDbTransaction BeginTransaction(IsolationLevel il) => throw new NotImplementedException();
            public void ChangeDatabase(string databaseName) => throw new NotImplementedException();
            public IDbCommand CreateCommand() => throw new NotImplementedException();
            public void Dispose() { }
        }

        #endregion

        #region GetDataTable (List 参数重载)

        [Fact]
        public void GetDataTable_ListParameters_WithTop_SqlServer()
        {
            var stub = new StubDbHelper();
            stub.GetDataTable("UserInfo", new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Id", 1)
            }, 10);

            Assert.StartsWith("SELECT TOP 10 * FROM UserInfo", stub.LastSql);
            Assert.Contains("WHERE Id = @Id", stub.LastSql);
        }

        [Fact]
        public void GetDataTable_ListParameters_MySql_Limit()
        {
            var stub = new StubDbHelper(CurrentDbType.MySql);
            stub.GetDataTable("UserInfo", new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Id", 1)
            }, 10);

            Assert.Contains("LIMIT 0, 10", stub.LastSql);
        }

        [Fact]
        public void GetDataTable_ListParameters_NoParameters_PlainSql()
        {
            var stub = new StubDbHelper();
            stub.GetDataTable("UserInfo", null, 0);

            Assert.Equal("SELECT * FROM UserInfo", stub.LastSql);
        }

        [Fact]
        public void GetDataTable_ListParameters_WithOrder()
        {
            var stub = new StubDbHelper();
            stub.GetDataTable("UserInfo", new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Id", 1)
            }, 0, "CreateTime DESC");

            Assert.EndsWith("ORDER BY CreateTime DESC", stub.LastSql);
        }

        #endregion

        #region GetDataTableByPage (sql 版本)

        [Fact]
        public void GetDataTableByPage_Sql_SqlServer_RowNumber()
        {
            var stub = new StubDbHelper();
            stub.GetDataTableByPage(100, 2, 10, "SELECT * FROM UserInfo", null, "Id", "ASC");

            Assert.Contains("ROW_NUMBER() OVER (ORDER BY Id ASC) AS ROWNUM", stub.LastSql);
            Assert.Contains("ROWNUM > 10 AND ROWNUM <= 20", stub.LastSql);
        }

        [Fact]
        public void GetDataTableByPage_Sql_Db2_RowNumber()
        {
            var stub = new StubDbHelper(CurrentDbType.Db2);
            stub.GetDataTableByPage(100, 1, 10, "SELECT * FROM UserInfo", null, "Id", "DESC");

            Assert.Contains("ROW_NUMBER() OVER (ORDER BY Id DESC)", stub.LastSql);
        }

        [Fact]
        public void GetDataTableByPage_Sql_MySql_Limit()
        {
            var stub = new StubDbHelper(CurrentDbType.MySql);
            stub.GetDataTableByPage(100, 3, 10, "SELECT * FROM UserInfo", null, "Id", "ASC");

            // ToTableName() 将 sql 包裹为 (SELECT ...)
            Assert.Equal("SELECT * FROM (SELECT * FROM UserInfo) ORDER BY Id ASC LIMIT 20,10", stub.LastSql);
        }

        [Fact]
        public void GetDataTableByPage_Sql_PostgreSql_Offset()
        {
            var stub = new StubDbHelper(CurrentDbType.PostgreSql);
            stub.GetDataTableByPage(100, 2, 10, "SELECT * FROM UserInfo", null, "Id", "ASC");

            Assert.Contains("LIMIT 10 OFFSET 10", stub.LastSql);
        }

        [Fact]
        public void GetDataTableByPage_Sql_Access_Top()
        {
            var stub = new StubDbHelper(CurrentDbType.Access);
            stub.GetDataTableByPage(100, 1, 10, "SELECT * FROM UserInfo", null, "Id", "ASC");

            Assert.Contains("SELECT TOP", stub.LastSql);
        }

        [Fact]
        public void GetDataTableByPage_Sql_Oracle_Rownum()
        {
            var stub = new StubDbHelper(CurrentDbType.Oracle);
            stub.GetDataTableByPage(100, 2, 10, "SELECT * FROM UserInfo", null, "Id", "ASC");

            Assert.Contains("ROWNUM RN", stub.LastSql);
            Assert.Contains("ROWNUM <= 20", stub.LastSql);
        }

        #endregion

        #region ParentChildrens

        [Fact]
        public void GetParentsByCode_SqlServer_LeftJoin()
        {
            var stub = new StubDbHelper();
            stub.GetParentsByCode("Category", "Code", "A01", "SortCode");

            Assert.StartsWith("SELECT *  FROM Category", stub.LastSql);
            // GetParameter(fieldCode) 会加 @ 前缀
            Assert.Contains("LEFT(@Code, LEN(Code)) = Code", stub.LastSql);
            Assert.Contains("ORDER BY SortCode", stub.LastSql);
        }

        [Fact]
        public void GetParentsByCode_IdOnly()
        {
            var stub = new StubDbHelper();
            stub.GetParentsByCode("Category", "Code", "A01", null, true);

            Assert.StartsWith("SELECT Id", stub.LastSql);
        }

        [Fact]
        public void GetParentsByCode_Oracle_Substr()
        {
            var stub = new StubDbHelper(CurrentDbType.Oracle);
            stub.GetParentsByCode("Category", "Code", "A01", null);

            Assert.Contains("SUBSTR(@Code, 1, LENGTH(Code)) = Code", stub.LastSql);
        }

        [Fact]
        public void GetChildrens_SqlServer_UsesCte()
        {
            var stub = new StubDbHelper();
            stub.GetChildrens("Category", "Id", "5", "ParentId");

            Assert.Contains("WITH Tree AS", stub.LastSql);
            Assert.Contains("UNION ALL", stub.LastSql);
        }

        [Fact]
        public void GetChildrens_Oracle_UsesStartWith()
        {
            var stub = new StubDbHelper(CurrentDbType.Oracle);
            stub.GetChildrens("Category", "Id", "5", "ParentId");

            Assert.Contains("START WITH ParentId = @Id", stub.LastSql);
            Assert.Contains("CONNECT BY PRIOR Id = ParentId", stub.LastSql);
        }

        [Fact]
        public void GetChildrens_UnsupportedDbType_NoSql()
        {
            var stub = new StubDbHelper(CurrentDbType.MySql);
            stub.GetChildrens("Category", "Id", "5", "ParentId");

            Assert.Null(stub.LastSql);
        }

        #endregion

        #region LockNoWait / SetProperty

        [Fact]
        public void LockNoWait_Params_Delegates()
        {
            var stub = new StubDbHelper();
            stub.LockNoWait("Orders", new KeyValuePair<string, object>("Id", 1));

            Assert.Contains("SELECT Id FROM Orders WHERE Id = @Id", stub.LastSql);
            Assert.Contains("FOR UPDATE NOWAIT", stub.LastSql);
        }

        [Fact]
        public void LockNoWait_ReturnsRowCount()
        {
            var stub = new StubDbHelper();
            var dt = new DataTable();
            dt.Columns.Add(BaseUtil.FieldId, typeof(int));
            dt.Rows.Add(1);
            dt.Rows.Add(2);
            stub.FillTable = dt;

            var result = stub.LockNoWait("Orders", new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Id", 1)
            });

            Assert.Equal(2, result);
        }

        [Fact]
        public void LockNoWait_Exception_ReturnsMinusOne()
        {
            var stub = new StubDbHelper { ThrowOnFill = true };
            var result = stub.LockNoWait("Orders", new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Id", 1)
            });

            Assert.Equal(-1, result);
        }

        [Fact]
        public void SetProperty_BuildsUpdateSql()
        {
            var stub = new StubDbHelper { NonQueryResult = 1 };
            var result = stub.SetProperty("UserInfo",
                new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>("Id", 5) },
                new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>("Name", "Troy") });

            Assert.Equal(1, result);
            Assert.Contains("UPDATE UserInfo", stub.LastSql);
            Assert.Contains("SET Name = @Name", stub.LastSql);
        }

        #endregion

        /// <summary>
        /// 最小 IDbDataParameter 实现：仅用于输出参数 Value 读取
        /// </summary>
        private sealed class FakeDbParameter : IDbDataParameter
        {
            public FakeDbParameter(string name, object value, ParameterDirection direction)
            {
                ParameterName = name;
                Value = value;
                Direction = direction;
            }

            public DbType DbType { get; set; }
            public ParameterDirection Direction { get; set; }
            public bool IsNullable { get; }
            public string ParameterName { get; set; }
            public string SourceColumn { get; set; } = string.Empty;
            public DataRowVersion SourceVersion { get; set; }
            public object Value { get; set; }
            public byte Precision { get; set; }
            public byte Scale { get; set; }
            public int Size { get; set; }
        }

        /// <summary>
        /// 增强桩：记录最近一次 SQL，ExecuteScalar/ExecuteNonQuery 返回可配置值，Fill 返回可配置表
        /// </summary>
        private sealed class StubDbHelper : IDbHelper
        {
            public StubDbHelper(CurrentDbType dbType = CurrentDbType.SqlServer)
            {
                CurrentDbType = dbType;
            }

            public string? LastSql { get; private set; }
            public CommandType? LastCommandType { get; private set; }
            public object ScalarResult { get; set; } = 0;
            public int NonQueryResult { get; set; }
            public DataTable? FillTable { get; set; }
            public IDataReader? ExecuteReaderResult { get; set; }
            public bool ThrowOnFill { get; set; }

            public CurrentDbType CurrentDbType { get; }

            public string GetParameter(string parameter) => "@" + parameter;

            public IDbDataParameter MakeParameter(string targetFiled, object targetValue) => null!;

            public object ExecuteScalar(string commandText, int commandTimeout = 30)
            {
                LastSql = commandText;
                return ScalarResult;
            }

            public object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters, int commandTimeout = 30)
            {
                LastSql = commandText;
                return ScalarResult;
            }

            public object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30)
            {
                LastSql = commandText;
                return ScalarResult;
            }

            public int ExecuteNonQuery(string commandText, int commandTimeout = 30)
            {
                LastSql = commandText;
                return NonQueryResult;
            }

            public int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters, int commandTimeout = 30)
            {
                LastSql = commandText;
                return NonQueryResult;
            }

            public int ExecuteNonQuery(string commandText, CommandType commandType, int commandTimeout = 30)
            {
                LastSql = commandText;
                return NonQueryResult;
            }

            public int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30)
            {
                LastSql = commandText;
                return NonQueryResult;
            }

            public int ExecuteNonQuery(IDbTransaction dbTransaction, string commandText, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30)
            {
                LastSql = commandText;
                return NonQueryResult;
            }

            public IDataReader ExecuteReader(string commandText, int commandTimeout = 30)
            {
                LastSql = commandText;
                return ExecuteReaderResult;
            }

            public IDataReader ExecuteReader(string commandText, IDbDataParameter[] dbParameters, int commandTimeout = 30)
            {
                LastSql = commandText;
                return ExecuteReaderResult;
            }

            public IDataReader ExecuteReader(string commandText, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30)
            {
                LastSql = commandText;
                LastCommandType = commandType;
                return ExecuteReaderResult;
            }

            public DataTable Fill(DataTable dt, string commandText, IDbDataParameter[] dbParameters, int commandTimeout = 30)
            {
                LastSql = commandText;
                if (ThrowOnFill)
                {
                    throw new InvalidOperationException("simulated failure");
                }
                if (FillTable != null)
                {
                    // 模拟真实 Fill：把数据填入传入的 dt（LockNoWait 读取的是传入 dt 的行数）
                    foreach (DataColumn col in FillTable.Columns)
                    {
                        if (!dt.Columns.Contains(col.ColumnName))
                        {
                            dt.Columns.Add(col.ColumnName, col.DataType);
                        }
                    }
                    foreach (DataRow row in FillTable.Rows)
                    {
                        dt.Rows.Add(row.ItemArray);
                    }
                }
                return dt;
            }

            public DataTable Fill(DataTable dt, string commandText, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30)
            {
                LastSql = commandText;
                if (ThrowOnFill)
                {
                    throw new InvalidOperationException("simulated failure");
                }
                return FillTable ?? dt;
            }

            public DataTable Fill(string commandText, IDbDataParameter[] dbParameters, int commandTimeout = 30)
            {
                LastSql = commandText;
                return FillTable ?? new DataTable("DotNet");
            }

            public DataTable Fill(DataTable dt, string commandText, int commandTimeout = 30)
            {
                LastSql = commandText;
                return FillTable ?? dt;
            }

            public DataTable Fill(string commandText, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30)
            {
                LastSql = commandText;
                return FillTable ?? new DataTable("DotNet");
            }

            public DataTable Fill(string commandText, int commandTimeout = 30)
            {
                LastSql = commandText;
                return FillTable ?? new DataTable("DotNet");
            }

            public IDbDataParameter[] MakeParameters(string[] targetFields, object[] targetValues) => new IDbDataParameter[0];
            public IDbDataParameter[] MakeParameters(Dictionary<string, object> parameters) => new IDbDataParameter[0];
            public IDbDataParameter[] MakeParameters(List<KeyValuePair<string, object>> parameters) => new IDbDataParameter[0];

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
            public IDbDataParameter MakeParameter(string parameterName, object parameterValue, DbType dbType, int parameterSize, ParameterDirection parameterDirection)
            {
                // 输出参数需要可读 Value，ExecuteReaderByPage 会读 RecordCount.Value
                return new FakeDbParameter(parameterName, parameterValue, parameterDirection);
            }
            public IDbConnection GetDbConnection() => throw new NotImplementedException();
            public IDbTransaction GetDbTransaction() => throw new NotImplementedException();
            public IDbCommand GetDbCommand() => throw new NotImplementedException();
            public IDbConnection Open() => throw new NotImplementedException();
            public IDbConnection Open(string connectionString) => throw new NotImplementedException();
            public IDbTransaction BeginTransaction() => throw new NotImplementedException();
            public void CommitTransaction() => throw new NotImplementedException();
            public void RollbackTransaction() => throw new NotImplementedException();
            public void Close() => throw new NotImplementedException();
            public DataSet Fill(DataSet dataSet, string commandText, string tableName, int commandTimeout = 30) => throw new NotImplementedException();
            public DataSet Fill(DataSet dataSet, string commandText, string tableName, IDbDataParameter[] dbParameters, int commandTimeout = 30) => throw new NotImplementedException();
            public DataSet Fill(DataSet dataSet, string commandText, string tableName, IDbDataParameter[] dbParameters, CommandType commandType, int commandTimeout = 30) => throw new NotImplementedException();
            public bool SqlBulkCopyData(DataTable dt, string destinationTableName, int bulkCopyTimeout = 1000, int batchSize = 0) => throw new NotImplementedException();
        }
    }
}
