using System;
using System.Data;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Db
{
    public class DbHelperCommandTimeoutTests
    {
        [Fact]
        public void AllCommandTimeoutOverloadsUseDefaultOfThirty()
        {
            AssertTimeout(30, helper => helper.ExecuteReader("SELECT 1"));
            AssertTimeout(30, helper => helper.ExecuteReader("SELECT 1", Array.Empty<IDbDataParameter>()));
            AssertTimeout(30, helper => helper.ExecuteReader("SELECT 1", Array.Empty<IDbDataParameter>(), CommandType.Text));

            AssertTimeout(30, helper => helper.ExecuteNonQuery("UPDATE Test SET Value = 1"));
            AssertTimeout(30, helper => helper.ExecuteNonQuery("UPDATE Test SET Value = 1", Array.Empty<IDbDataParameter>()));
            AssertTimeout(30, helper => helper.ExecuteNonQuery("UPDATE Test SET Value = 1", CommandType.Text));
            AssertTimeout(30, helper => helper.ExecuteNonQuery("UPDATE Test SET Value = 1", Array.Empty<IDbDataParameter>(), CommandType.Text));
            AssertTimeout(30, helper => helper.ExecuteNonQuery(null, "UPDATE Test SET Value = 1", Array.Empty<IDbDataParameter>(), CommandType.Text));

            AssertTimeout(30, helper => helper.ExecuteScalar("SELECT 1"));
            AssertTimeout(30, helper => helper.ExecuteScalar("SELECT 1", Array.Empty<IDbDataParameter>()));
            AssertTimeout(30, helper => helper.ExecuteScalar("SELECT 1", Array.Empty<IDbDataParameter>(), CommandType.Text));

            AssertTimeout(30, helper => helper.Fill("SELECT 1"));
            AssertTimeout(30, helper => helper.Fill(new DataTable(), "SELECT 1"));
            AssertTimeout(30, helper => helper.Fill("SELECT 1", Array.Empty<IDbDataParameter>()));
            AssertTimeout(30, helper => helper.Fill(new DataTable(), "SELECT 1", Array.Empty<IDbDataParameter>()));
            AssertTimeout(30, helper => helper.Fill("SELECT 1", Array.Empty<IDbDataParameter>(), CommandType.Text));
            AssertTimeout(30, helper => helper.Fill(new DataTable(), "SELECT 1", Array.Empty<IDbDataParameter>(), CommandType.Text));

            AssertTimeout(30, helper => helper.Fill(new DataSet(), "SELECT 1", "Test"));
            AssertTimeout(30, helper => helper.Fill(new DataSet(), "SELECT 1", "Test", Array.Empty<IDbDataParameter>()));
            AssertTimeout(30, helper => helper.Fill(new DataSet(), "SELECT 1", "Test", Array.Empty<IDbDataParameter>(), CommandType.Text));
        }

        [Fact]
        public void AllCommandTimeoutOverloadsForwardExplicitValue()
        {
            const int commandTimeout = 47;

            AssertTimeout(commandTimeout, helper => helper.ExecuteReader("SELECT 1", commandTimeout));
            AssertTimeout(commandTimeout, helper => helper.ExecuteReader("SELECT 1", Array.Empty<IDbDataParameter>(), commandTimeout));
            AssertTimeout(commandTimeout, helper => helper.ExecuteReader("SELECT 1", Array.Empty<IDbDataParameter>(), CommandType.Text, commandTimeout));

            AssertTimeout(commandTimeout, helper => helper.ExecuteNonQuery("UPDATE Test SET Value = 1", commandTimeout));
            AssertTimeout(commandTimeout, helper => helper.ExecuteNonQuery("UPDATE Test SET Value = 1", Array.Empty<IDbDataParameter>(), commandTimeout));
            AssertTimeout(commandTimeout, helper => helper.ExecuteNonQuery("UPDATE Test SET Value = 1", CommandType.Text, commandTimeout));
            AssertTimeout(commandTimeout, helper => helper.ExecuteNonQuery("UPDATE Test SET Value = 1", Array.Empty<IDbDataParameter>(), CommandType.Text, commandTimeout));
            AssertTimeout(commandTimeout, helper => helper.ExecuteNonQuery(null, "UPDATE Test SET Value = 1", Array.Empty<IDbDataParameter>(), CommandType.Text, commandTimeout));

            AssertTimeout(commandTimeout, helper => helper.ExecuteScalar("SELECT 1", commandTimeout));
            AssertTimeout(commandTimeout, helper => helper.ExecuteScalar("SELECT 1", Array.Empty<IDbDataParameter>(), commandTimeout));
            AssertTimeout(commandTimeout, helper => helper.ExecuteScalar("SELECT 1", Array.Empty<IDbDataParameter>(), CommandType.Text, commandTimeout));

            AssertTimeout(commandTimeout, helper => helper.Fill("SELECT 1", commandTimeout));
            AssertTimeout(commandTimeout, helper => helper.Fill(new DataTable(), "SELECT 1", commandTimeout));
            AssertTimeout(commandTimeout, helper => helper.Fill("SELECT 1", Array.Empty<IDbDataParameter>(), commandTimeout));
            AssertTimeout(commandTimeout, helper => helper.Fill(new DataTable(), "SELECT 1", Array.Empty<IDbDataParameter>(), commandTimeout));
            AssertTimeout(commandTimeout, helper => helper.Fill("SELECT 1", Array.Empty<IDbDataParameter>(), CommandType.Text, commandTimeout));
            AssertTimeout(commandTimeout, helper => helper.Fill(new DataTable(), "SELECT 1", Array.Empty<IDbDataParameter>(), CommandType.Text, commandTimeout));

            AssertTimeout(commandTimeout, helper => helper.Fill(new DataSet(), "SELECT 1", "Test", commandTimeout));
            AssertTimeout(commandTimeout, helper => helper.Fill(new DataSet(), "SELECT 1", "Test", Array.Empty<IDbDataParameter>(), commandTimeout));
            AssertTimeout(commandTimeout, helper => helper.Fill(new DataSet(), "SELECT 1", "Test", Array.Empty<IDbDataParameter>(), CommandType.Text, commandTimeout));
        }

        private static void AssertTimeout(int expectedTimeout, Action<DbHelper> execute)
        {
            var helper = new TestDbHelper();
            var connection = (FakeDbConnection)helper.DbConnection;

            execute(helper);

            Assert.NotNull(connection.LastCommand);
            Assert.Equal(expectedTimeout, connection.LastCommand!.CommandTimeout);
        }
    }
}
