using System.Collections.Generic;
using System.Collections.Specialized;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    /// SQLBuilder 扩展构造测试（KeyValuePair / NameValueCollection 重载）
    /// </summary>
    public class SqlBuilderOverloadTests
    {
        [Fact]
        public void SetValue_KeyValuePair_AddsParameter()
        {
            var sb = new SqlBuilder(CurrentDbType.SqlServer);
            sb.BeginInsert("UserInfo");

            sb.SetValue(new KeyValuePair<string, object>("UserName", "Troy"));

            Assert.Single(sb.DbParameters);
            Assert.Equal("UserName", sb.DbParameters[0].Key);
            Assert.Equal("Troy", sb.DbParameters[0].Value);
        }

        [Fact]
        public void SetWhere_KeyValuePair_AddsWhereParameter()
        {
            var sb = new SqlBuilder(CurrentDbType.SqlServer);
            sb.BeginSelect("UserInfo");

            sb.SetWhere(new KeyValuePair<string, object>("Id", 7));

            // 参数名实际为 key 后缀 Where（如 IdWhere）
            Assert.Single(sb.DbParameters);
            Assert.Equal("IdWhere", sb.DbParameters[0].Key);
            Assert.Equal(7, sb.DbParameters[0].Value);
        }

        [Fact]
        public void SetWhere_KeyValuePairList_AddsAll()
        {
            var sb = new SqlBuilder(CurrentDbType.SqlServer);
            sb.BeginSelect("UserInfo");

            sb.SetWhere(new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Id", 1),
                new KeyValuePair<string, object>("Enabled", 1)
            });

            Assert.Equal(2, sb.DbParameters.Count);
        }

        [Fact]
        public void SetWhere_NameValueCollection_AddsAll()
        {
            var sb = new SqlBuilder(CurrentDbType.SqlServer);
            sb.BeginSelect("UserInfo");

            var nvc = new NameValueCollection { { "Id", "1" }, { "Code", "A" } };
            sb.SetWhere(nvc);

            Assert.Equal(2, sb.DbParameters.Count);
        }

        [Fact]
        public void SetValue_KeyValuePair_GeneratesSql()
        {
            var sb = new SqlBuilder(CurrentDbType.SqlServer);
            sb.BeginInsert("UserInfo");
            sb.SetValue(new KeyValuePair<string, object>("UserName", "Troy"));

            var sql = sb.PrepareCommand(out _);

            Assert.Contains("INSERT INTO UserInfo", sql, System.StringComparison.OrdinalIgnoreCase);
            Assert.Contains("UserName", sql);
        }
    }
}
