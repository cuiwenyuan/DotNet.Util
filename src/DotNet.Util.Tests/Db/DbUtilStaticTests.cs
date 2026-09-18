using System;
using System.Data;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    /// DbUtil 静态映射方法测试（纯逻辑，不连库）
    /// </summary>
    public class DbUtilStaticTests
    {
        #region GetCommandType

        [Theory]
        [InlineData("Text", CommandType.Text)]
        [InlineData("StoredProcedure", CommandType.StoredProcedure)]
        [InlineData("TableDirect", CommandType.TableDirect)]
        [InlineData("NoSuch", CommandType.Text)]
        [InlineData("", CommandType.Text)]
        [InlineData(null, CommandType.Text)]
        public void GetCommandType_Maps(string input, CommandType expected)
        {
            Assert.Equal(expected, DbUtil.GetCommandType(input));
        }

        #endregion

        #region GetDbHelperClass

        [Theory]
        [InlineData(CurrentDbType.SqlServer, "DotNet.Util.SqlHelper")]
        [InlineData(CurrentDbType.Oracle, "DotNet.Util.OracleHelper")]
        [InlineData(CurrentDbType.MySql, "DotNet.Util.MySqlHelper")]
        [InlineData(CurrentDbType.SQLite, "DotNet.Util.SQLiteHelper")]
        [InlineData(CurrentDbType.PostgreSql, "DotNet.Util.PostgreSqlHelper")]
        public void GetDbHelperClass_Maps(CurrentDbType dbType, string expected)
        {
            Assert.Equal(expected, DbUtil.GetDbHelperClass(dbType));
        }

        #endregion

        #region GetDbHelperDll

        [Theory]
        [InlineData(CurrentDbType.SqlServer, "DotNet.Util.Db")]
        [InlineData(CurrentDbType.Oracle, "DotNet.Util.Db.Oracle")]
        [InlineData(CurrentDbType.MySql, "DotNet.Util.Db.MySql")]
        [InlineData(CurrentDbType.SQLite, "DotNet.Util.Db.SQLite")]
        public void GetDbHelperDll_Maps(CurrentDbType dbType, string expected)
        {
            Assert.Equal(expected, DbUtil.GetDbHelperDll(dbType));
        }

        #endregion

        #region GetDbNow

        [Fact]
        public void GetDbNow_SqlServer_GetDate()
        {
            Assert.Contains("GETDATE", DbUtil.GetDbNow(CurrentDbType.SqlServer));
        }

        [Fact]
        public void GetDbNow_Oracle_SysDate()
        {
            Assert.Contains("SYSDATE", DbUtil.GetDbNow(CurrentDbType.Oracle));
        }

        [Fact]
        public void GetDbNow_MySql_Now()
        {
            Assert.Contains("NOW", DbUtil.GetDbNow(CurrentDbType.MySql));
        }

        [Fact]
        public void GetDbNow_Sqlite_Datetime()
        {
            Assert.Contains("CURRENT_TIMESTAMP", DbUtil.GetDbNow(CurrentDbType.SQLite));
        }

        #endregion

        #region ToDbTime

        [Fact]
        public void ToDbTime_SqlServer_Quoted()
        {
            Assert.Equal("'2026-08-24 10:00:00'", DbUtil.ToDbTime(CurrentDbType.SqlServer, "2026-08-24 10:00:00"));
        }

        [Fact]
        public void ToDbTime_Oracle_ToDate()
        {
            Assert.Equal("TO_DATE('2026-08-24 10:00:00','yyyy-mm-dd hh24:mi:ss')",
                DbUtil.ToDbTime(CurrentDbType.Oracle, "2026-08-24 10:00:00"));
        }

        [Fact]
        public void ToDbTime_InvalidDateTime_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, DbUtil.ToDbTime(CurrentDbType.SqlServer, "not-a-date"));
        }

        #endregion

        #region GetParameter

        [Theory]
        [InlineData(CurrentDbType.SqlServer, "id", "@id")]
        [InlineData(CurrentDbType.Oracle, "id", ":id")]
        [InlineData(CurrentDbType.MySql, "id", "?id")]
        [InlineData(CurrentDbType.SQLite, "id", "@id")]
        public void GetParameter_PrefixesByDbType(CurrentDbType dbType, string input, string expected)
        {
            Assert.Equal(expected, DbUtil.GetParameter(dbType, input));
        }

        #endregion
    }
}
