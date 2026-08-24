using System;
using System.Data;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// BaseUtil（partial 各文件合并）核心 static 方法测试
    /// 聚焦确定性、可移植、无外部依赖的方法。
    /// </summary>
    public class BaseUtilTests
    {
        #region GetPaging
        [Fact]
        public void GetPaging_Defaults()
        {
            var p = BaseUtil.GetPaging();
            Assert.Equal(1, p.PageNo);
            Assert.Equal(20, p.PageSize);
            Assert.Equal("DESC", p.SortDirection);
        }

        [Fact]
        public void GetPaging_InvalidPageNoAndSize_KeepsDefaults()
        {
            var p = BaseUtil.GetPaging(0, 0);
            Assert.Equal(1, p.PageNo);
            Assert.Equal(20, p.PageSize);
        }

        [Fact]
        public void GetPaging_ValidSortDirection()
        {
            var p = BaseUtil.GetPaging(2, 10, "CreateTime", "ASC");
            Assert.Equal(2, p.PageNo);
            Assert.Equal(10, p.PageSize);
            Assert.Equal("CreateTime", p.SortExpression);
            Assert.Equal("ASC", p.SortDirection);
        }

        [Fact]
        public void GetPaging_InvalidSortDirection_StaysDesc()
        {
            var p = BaseUtil.GetPaging(1, 10, "CreateTime", "XYZ");
            Assert.Equal("DESC", p.SortDirection);
        }
        #endregion

        #region GetIntKeys
        [Fact]
        public void GetIntKeys_ParsesValid()
        {
            var keys = BaseUtil.GetIntKeys(new[] { "1", "2" });
            Assert.Equal(new[] { 1, 2 }, keys);
        }

        [Fact]
        public void GetIntKeys_SkipsEmpty()
        {
            var keys = BaseUtil.GetIntKeys(new[] { "1", "", "" });
            Assert.Single(keys);
            Assert.Equal(1, keys[0]);
        }
        #endregion

        #region GetAuditStatus
        [Fact]
        public void GetAuditStatus_Enum()
        {
            Assert.Equal("草稿", BaseUtil.GetAuditStatus(AuditStatus.Draft));
            Assert.Equal("通过", BaseUtil.GetAuditStatus(AuditStatus.AuditPass));
        }

        [Fact]
        public void GetAuditStatus_Int()
        {
            Assert.Equal("草稿", BaseUtil.GetAuditStatus(1));
            Assert.Equal("完成", BaseUtil.GetAuditStatus(7));
        }

        [Fact]
        public void GetAuditStatus_String()
        {
            Assert.Equal("草稿", BaseUtil.GetAuditStatus("Draft"));
            // 未知值回退到 Draft 的描述
            Assert.Equal("草稿", BaseUtil.GetAuditStatus("NotExists"));
        }
        #endregion

        #region IsKeywords
        [Fact]
        public void IsKeywords_KnownFields()
        {
            Assert.True(BaseUtil.IsKeywords("id"));
            Assert.True(BaseUtil.IsKeywords("Id"));
            Assert.True(BaseUtil.IsKeywords("SortCode"));
        }

        [Fact]
        public void IsKeywords_UnknownField()
        {
            Assert.False(BaseUtil.IsKeywords("UserName"));
        }
        #endregion

        #region SetColumnsFilter
        [Fact]
        public void SetColumnsFilter_RemovesNonKeywordNotInList()
        {
            var dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Name");
            dt.Columns.Add("UserName");
            dt.Rows.Add("1", "a", "u1");

            var result = BaseUtil.SetColumnsFilter(dt, new[] { "Id", "UserName" });
            Assert.Contains("Id", result.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
            Assert.DoesNotContain("Name", result.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
            Assert.Contains("UserName", result.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
        }
        #endregion

        #region Exists / IsAuthorized
        [Fact]
        public void Exists_MatchesValue_IgnoreCase()
        {
            var dt = new DataTable();
            dt.Columns.Add("Code");
            dt.Rows.Add("A");
            dt.Rows.Add("B");

            Assert.True(BaseUtil.Exists(dt, "Code", "a"));
            Assert.False(BaseUtil.Exists(dt, "Code", "C"));
            Assert.False(BaseUtil.Exists(null, "Code", "A"));
        }

        [Fact]
        public void IsAuthorized_ChecksCodeColumn()
        {
            var dt = new DataTable();
            dt.Columns.Add("Code");
            dt.Rows.Add("Read");
            dt.Rows.Add("Write");

            Assert.True(BaseUtil.IsAuthorized(dt, "read"));
            Assert.False(BaseUtil.IsAuthorized(dt, "Admin"));
        }
        #endregion

        #region GetPermissionScope
        [Fact]
        public void GetPermissionScope_MapsByIntValue()
        {
            Assert.Equal(PermissionOrganizationScope.OnlyOwnData, BaseUtil.GetPermissionScope(new[] { "0" }));
            Assert.Equal(PermissionOrganizationScope.NotAllowed, BaseUtil.GetPermissionScope(new[] { "-2" }));
        }

        [Fact]
        public void GetPermissionScope_UnknownOrEmpty_ReturnsNotAllowed()
        {
            Assert.Equal(PermissionOrganizationScope.NotAllowed, BaseUtil.GetPermissionScope(new[] { "99" }));
            Assert.Equal(PermissionOrganizationScope.NotAllowed, BaseUtil.GetPermissionScope(Array.Empty<string>()));
        }
        #endregion

        #region Convert methods
        [Fact]
        public void ConvertToBoolean_Works()
        {
            Assert.True(BaseUtil.ConvertToBoolean("true"));
            Assert.True(BaseUtil.ConvertToBoolean("True"));
            Assert.True(BaseUtil.ConvertToBoolean("1"));
            Assert.False(BaseUtil.ConvertToBoolean("0"));
            Assert.False(BaseUtil.ConvertToBoolean("false"));
            Assert.False(BaseUtil.ConvertToBoolean(null));
            Assert.False(BaseUtil.ConvertToBoolean(DBNull.Value));
        }

        [Fact]
        public void ConvertToString_HandlesDbNull()
        {
            Assert.Null(BaseUtil.ConvertToString(DBNull.Value));
            Assert.Equal("x", BaseUtil.ConvertToString("x"));
        }

        [Fact]
        public void ConvertToInt_Works()
        {
            Assert.Equal(5, BaseUtil.ConvertToInt("5"));
            Assert.Equal(0, BaseUtil.ConvertToInt(DBNull.Value));
            Assert.Equal(0, BaseUtil.ConvertToInt("abc"));
            Assert.Equal(7, BaseUtil.ConvertToInt("abc", 7));
        }

        [Fact]
        public void ConvertToNullableInt_Works()
        {
            var v = BaseUtil.ConvertToNullableInt("5");
            Assert.True(v.HasValue);
            Assert.Equal(5, v!.Value);
            Assert.Null(BaseUtil.ConvertToNullableInt(DBNull.Value));
        }

        [Fact]
        public void ConvertToLongAndInt64_Work()
        {
            Assert.Equal(5L, BaseUtil.ConvertToLong("5"));
            Assert.Equal(5L, BaseUtil.ConvertToInt64("5"));
            Assert.Equal(0L, BaseUtil.ConvertToInt64("bad"));
        }

        [Fact]
        public void ConvertToDoubleDecimal_Work()
        {
            Assert.Equal(1.5, BaseUtil.ConvertToDouble("1.5"));
            Assert.Equal(2.25m, BaseUtil.ConvertToDecimal("2.25"));
            Assert.Equal(0m, BaseUtil.ConvertToDecimal(DBNull.Value));
        }

        [Fact]
        public void ConvertToDateTime_Works()
        {
            var dt = BaseUtil.ConvertToDateTime("2020-01-01");
            Assert.Equal(2020, dt.Year);
            Assert.Equal(DateTime.MinValue, BaseUtil.ConvertToDateTime(DBNull.Value));
            Assert.Equal(DateTime.MinValue, BaseUtil.ConvertToDateTime(null));
        }

        [Fact]
        public void ConvertToNullableDateTime_Works()
        {
            var v = BaseUtil.ConvertToNullableDateTime("2020-01-01");
            Assert.True(v.HasValue);
            Assert.Equal(2020, v!.Value.Year);
            Assert.Null(BaseUtil.ConvertToNullableDateTime(DBNull.Value));
        }

        [Fact]
        public void ConvertToByte_Works()
        {
            Assert.Equal((byte)5, BaseUtil.ConvertToByteInt("5"));
            Assert.Equal((byte)0, BaseUtil.ConvertToByteInt("bad"));
        }

        [Fact]
        public void ConvertToDateToString_Works()
        {
            var result = BaseUtil.ConvertToDateToString("2020-01-01");
            Assert.False(string.IsNullOrEmpty(result));
            Assert.Contains("2020", result);
        }

        [Fact]
        public void ChangeType_Works()
        {
            Assert.Equal(123, BaseUtil.ChangeType("123", typeof(int)));
            Assert.Equal(AuditStatus.Draft, BaseUtil.ChangeType("Draft", typeof(AuditStatus)));
            var nullable = BaseUtil.ChangeType("5", typeof(int?));
            Assert.Equal(5, nullable);
        }

        [Fact]
        public void IsNullOrDbNull_Works()
        {
            Assert.True(BaseUtil.IsNullOrDbNull(null));
            Assert.True(BaseUtil.IsNullOrDbNull(DBNull.Value));
            Assert.False(BaseUtil.IsNullOrDbNull("x"));
        }
        #endregion

        #region DataTable helpers
        private static DataTable BuildTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Name");
            dt.Columns.Add("SortCode");
            dt.Rows.Add("1", "Apple", "100");
            dt.Rows.Add("2", "Banana", "200");
            dt.Rows.Add("3", "Cherry", "300");
            return dt;
        }

        [Fact]
        public void FieldToList_FormatsQuotedList()
        {
            var dt = BuildTable();
            Assert.Equal("'1', '2', '3'", BaseUtil.FieldToList(dt));
        }

        [Fact]
        public void FieldToList_Empty_ReturnsEmptyQuotes()
        {
            var dt = new DataTable();
            dt.Columns.Add("Id");
            Assert.Equal("''", BaseUtil.FieldToList(dt));
        }

        [Fact]
        public void FieldToArray_ReturnsDistinct()
        {
            var dt = BuildTable();
            var arr = BaseUtil.FieldToArray(dt, "Id");
            Assert.Equal(new[] { "1", "2", "3" }, arr);
        }

        [Fact]
        public void GetProperty_And_SetProperty_Work()
        {
            var dt = BuildTable();
            Assert.Equal("Apple", BaseUtil.GetProperty(dt, "1", "Name"));

            var count = BaseUtil.SetProperty(dt, "1", "Name", "ApplePie");
            Assert.Equal(1, count);
            Assert.Equal("ApplePie", BaseUtil.GetProperty(dt, "1", "Name"));
        }

        [Fact]
        public void Delete_RemovesRow()
        {
            var dt = BuildTable();
            var count = BaseUtil.Delete(dt, "2");
            Assert.Equal(1, count);
            Assert.Null(BaseUtil.GetDataRow(dt, "2"));
            Assert.NotNull(BaseUtil.GetDataRow(dt, "1"));
        }

        [Fact]
        public void Filter_ByExpression()
        {
            var dt = BuildTable();
            var filtered = BaseUtil.Filter(dt, "Id = '1'");
            Assert.Single(filtered.Rows);
            Assert.Equal("Apple", filtered.Rows[0]["Name"]);
        }

        [Fact]
        public void SetFilter_DeletesMatching()
        {
            var dt = BuildTable();
            BaseUtil.SetFilter(dt, "Name", "Banana", true);
            Assert.Null(BaseUtil.GetDataRow(dt, "2"));
            Assert.NotNull(BaseUtil.GetDataRow(dt, "1"));
        }

        [Fact]
        public void GetDateTime_FormatsDate()
        {
            var dt = new DataTable();
            dt.Columns.Add("CreatedOn", typeof(DateTime));
            dt.Rows.Add(new DateTime(2020, 1, 1));
            var result = BaseUtil.GetDateTime(dt.Rows[0], "CreatedOn");
            Assert.Contains("2020", result);
        }
        #endregion

        #region Sort helpers
        [Fact]
        public void GetNextId_And_PreviousId_Work()
        {
            var dt = BuildTable();
            Assert.Equal("2", BaseUtil.GetNextId(dt, "1"));
            Assert.Equal("3", BaseUtil.GetNextId(dt, "2"));
            Assert.Equal(string.Empty, BaseUtil.GetNextId(dt, "3"));

            Assert.Equal("1", BaseUtil.GetPreviousId(dt, "2"));
            Assert.Equal(string.Empty, BaseUtil.GetPreviousId(dt, "1"));
        }

        [Fact]
        public void Swap_ExchangesSortCode()
        {
            var dt = BuildTable();
            BaseUtil.Swap(dt, "1", "2");
            Assert.Equal("200", BaseUtil.GetProperty(dt, "1", "SortCode"));
            Assert.Equal("100", BaseUtil.GetProperty(dt, "2", "SortCode"));
        }
        #endregion
    }
}
