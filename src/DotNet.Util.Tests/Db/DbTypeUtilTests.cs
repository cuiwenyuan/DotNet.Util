using DotNet.Util;

using Xunit;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    /// DbTypeUtil 鏁版嵁搴撶被鍨嬫槧灏?杞崲娴嬭瘯锛堢函閫昏緫锛屼笉杩炲簱锛?    /// </summary>
    public class DbTypeUtilTests
    {
        [Fact]
        public void GetDbType_ExactMatch_ReturnsEnum()
        {
            Assert.Equal(CurrentDbType.SqlServer, DbTypeUtil.GetDbType("SqlServer"));
            Assert.Equal(CurrentDbType.SQLite, DbTypeUtil.GetDbType("SQLite"));
            Assert.Equal(CurrentDbType.PostgreSql, DbTypeUtil.GetDbType("PostgreSql"));
        }

        [Fact]
        public void GetDbType_CaseInsensitive_ReturnsEnum()
        {
            Assert.Equal(CurrentDbType.SqlServer, DbTypeUtil.GetDbType("sqlserver"));
            Assert.Equal(CurrentDbType.MySql, DbTypeUtil.GetDbType("MYSQL"));
            Assert.Equal(CurrentDbType.SQLite, DbTypeUtil.GetDbType("sqlite"));
        }

        [Fact]
        public void GetDbType_EmptyOrNull_ReturnsDefault()
        {
            Assert.Equal(CurrentDbType.SqlServer, DbTypeUtil.GetDbType(""));
            Assert.Equal(CurrentDbType.SqlServer, DbTypeUtil.GetDbType(null!));
        }

        [Fact]
        public void GetDbType_InvalidValue_ReturnsDefault()
        {
            Assert.Equal(CurrentDbType.SqlServer, DbTypeUtil.GetDbType("NotARealDb"));
        }

        [Fact]
        public void GetDbType_CustomDefault_UsedWhenInvalid()
        {
            Assert.Equal(CurrentDbType.Oracle, DbTypeUtil.GetDbType("NotARealDb", CurrentDbType.Oracle));
        }

        [Fact]
        public void GetDbType_CustomDefault_OverriddenWhenValid()
        {
            Assert.Equal(CurrentDbType.MySql, DbTypeUtil.GetDbType("mysql", CurrentDbType.Oracle));
        }
    }
}
