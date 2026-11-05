using System;
using System.Data;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Db
{
    public class PagedDataTableCommandTimeoutTests
    {
        [Fact]
        public void AllGetDataTableByPageOverloadsUseDefaultTimeout()
        {
            AssertTimeout(30, helper => DbUtil.GetDataTableByPage(helper, 100, 1, 10, "SELECT * FROM Test", null, Array.Empty<IDbDataParameter>()));
            AssertTimeout(30, helper =>
            {
                var recordCount = 0;
                DbUtil.GetDataTableByPage(helper, out recordCount);
            });
            AssertTimeout(30, helper => DbUtil.GetDataTableByPage(helper, 100, 1, 10, "SELECT * FROM Test", Array.Empty<IDbDataParameter>()));
            AssertTimeout(30, helper => DbUtil.GetDataTableByPage(helper, "Test", "*", 1, 10, null, "Id"));
            AssertTimeout(30, helper => DbUtil.GetDataTableByPage(helper, "Test", "*", 1, 10, null, Array.Empty<IDbDataParameter>(), "Id"));
            AssertTimeout(30, helper =>
            {
                var recordCount = 0;
                DbUtil.GetDataTableByPage(helper, out recordCount, "Test", "*", 1, 10, null, Array.Empty<IDbDataParameter>(), "Id", showRecordCount: false);
            });
        }

        [Fact]
        public void AllGetDataTableByPageOverloadsForwardExplicitTimeout()
        {
            const int commandTimeout = 47;

            AssertTimeout(commandTimeout, helper => DbUtil.GetDataTableByPage(helper, 100, 1, 10, "SELECT * FROM Test", null, Array.Empty<IDbDataParameter>(), commandTimeout: commandTimeout));
            AssertTimeout(commandTimeout, helper =>
            {
                var recordCount = 0;
                DbUtil.GetDataTableByPage(helper, out recordCount, commandTimeout: commandTimeout);
            });
            AssertTimeout(commandTimeout, helper => DbUtil.GetDataTableByPage(helper, 100, 1, 10, "SELECT * FROM Test", Array.Empty<IDbDataParameter>(), commandTimeout: commandTimeout));
            AssertTimeout(commandTimeout, helper => DbUtil.GetDataTableByPage(helper, "Test", "*", 1, 10, null, "Id", commandTimeout: commandTimeout));
            AssertTimeout(commandTimeout, helper => DbUtil.GetDataTableByPage(helper, "Test", "*", 1, 10, null, Array.Empty<IDbDataParameter>(), "Id", commandTimeout: commandTimeout));
            AssertTimeout(commandTimeout, helper =>
            {
                var recordCount = 0;
                DbUtil.GetDataTableByPage(helper, out recordCount, "Test", "*", 1, 10, null, Array.Empty<IDbDataParameter>(), "Id", showRecordCount: false, commandTimeout: commandTimeout);
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
