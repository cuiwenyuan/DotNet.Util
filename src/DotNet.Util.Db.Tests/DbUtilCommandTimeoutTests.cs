using System;
using System.Collections.Generic;
using System.Data;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Db
{
    public class DbUtilCommandTimeoutTests
    {
        [Fact]
        public void ScalarUtilitiesForwardExplicitTimeout()
        {
            AssertTimeout(43, helper => helper.Count("Test", commandTimeout: 43));
            AssertTimeout(43, helper => helper.DistinctCount("Test", "Value", commandTimeout: 43));
            AssertTimeout(43, helper => helper.GetProperty("Test", new List<KeyValuePair<string, object>>(), "Value", commandTimeout: 43));
            AssertTimeout(43, helper => helper.AggregateInt("Test", "Value", commandTimeout: 43));
        }

        [Fact]
        public void ReaderAndDataRetrievalUtilitiesForwardExplicitTimeout()
        {
            AssertTimeout(43, helper => helper.GetDataTable("Test", Array.Empty<IDbDataParameter>(), null, commandTimeout: 43));
            AssertTimeout(43, helper => helper.GetDataTable("Test", new List<KeyValuePair<string, object>>(), commandTimeout: 43));
            AssertTimeout(43, helper => helper.ExecuteReader("Test", "Id", new object[] { 1 }, commandTimeout: 43));
        }

        [Fact]
        public void MutationAndLockUtilitiesForwardExplicitTimeout()
        {
            AssertTimeout(43, helper => helper.Delete("Test", "Id = 1", commandTimeout: 43));
            AssertTimeout(43, helper => helper.LockNoWait("Test", 43, new KeyValuePair<string, object>("Id", 1)));
            AssertTimeout(43, helper => helper.LockNoWait("Test", new List<KeyValuePair<string, object>>(), commandTimeout: 43));
        }

        [Fact]
        public void SqlBuilderTerminalOperationsForwardExplicitTimeout()
        {
            AssertTimeout(43, helper =>
            {
                var builder = new SqlBuilder(helper);
                builder.BeginUpdate("Test");
                builder.SetValue("Value", 1);
                builder.SetWhere("Id", 1);
                builder.EndUpdate(43);
            });
            AssertTimeout(43, helper =>
            {
                var builder = new SqlBuilder(helper);
                builder.BeginSelect("Test");
                builder.EndSelect(43);
            });
        }

        private static void AssertTimeout(int expectedTimeout, Action<TestDbHelper> execute)
        {
            var helper = new TestDbHelper();
            var connection = (FakeDbConnection)helper.DbConnection;

            execute(helper);

            Assert.NotNull(connection.LastCommand);
            Assert.Equal(expectedTimeout, connection.LastCommand!.CommandTimeout);
        }
    }
}
