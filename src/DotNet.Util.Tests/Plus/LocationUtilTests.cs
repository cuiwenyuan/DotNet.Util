using System.Data;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Plus
{
    /// <summary>
    /// LocationUtil 中纯逻辑（Newtonsoft.Json 扩展方法）的测试。
    /// 说明：GetLocation 依赖外部 HTTP 接口，本测试不覆盖。
    /// </summary>
    public class LocationUtilTests
    {
        private sealed class Sample
        {
            public string? Name { get; set; }
            public int Age { get; set; }
        }

        [Fact]
        public void ToJson_Object_ReturnsSerializedString()
        {
            var obj = new { Name = "Troy", Age = 18 };
            var json = obj.ToJson();
            Assert.NotNull(json);
            Assert.Contains("Troy", json!);
            Assert.Contains("18", json);
        }

        [Fact]
        public void ToJson_WithDateTimeFormat_AppliesFormat()
        {
            var obj = new { Time = new System.DateTime(2026, 1, 2, 3, 4, 5) };
            var json = obj.ToJson("yyyy-MM-dd");
            Assert.NotNull(json);
            Assert.Contains("2026-01-02", json!);
            // 时间部分不应出现默认的 HH:mm:ss 格式
            Assert.DoesNotContain("03:04:05", json);
        }

        [Fact]
        public void ToJson_StringExtension_ReturnsNullForNull()
        {
            string? nullStr = null;
            Assert.Null(nullStr!.ToJson());
        }

        [Fact]
        public void ToObject_ValidJson_RoundTrips()
        {
            var json = "{\"Name\":\"Troy\",\"Age\":18}";
            var obj = json.ToObject<Sample>();
            Assert.NotNull(obj);
            Assert.Equal("Troy", obj!.Name);
            Assert.Equal(18, obj.Age);
        }

        [Fact]
        public void ToObject_NullString_ReturnsDefault()
        {
            string? nullStr = null;
            Assert.Null(nullStr!.ToObject<Sample>());
        }

        [Fact]
        public void ToList_JsonArray_ReturnsList()
        {
            var json = "[{\"Name\":\"A\"},{\"Name\":\"B\"}]";
            var list = json.ToList<Sample>();
            Assert.NotNull(list);
            Assert.Equal(2, list!.Count);
            Assert.Equal("A", list[0].Name);
            Assert.Equal("B", list[1].Name);
        }

        [Fact]
        public void ToList_NullString_ReturnsNull()
        {
            string? nullStr = null;
            Assert.Null(nullStr!.ToList<Sample>());
        }

        [Fact]
        public void ToTable_JsonArray_ReturnsDataTable()
        {
            var json = "[{\"id\":\"1\",\"name\":\"x\"}]";
            var table = json.ToTable();
            Assert.NotNull(table);
            Assert.Equal(1, table!.Rows.Count);
            Assert.Equal("x", table.Rows[0]["name"]);
        }

        [Fact]
        public void ToTable_NullString_ReturnsNull()
        {
            string? nullStr = null;
            Assert.Null(nullStr!.ToTable());
        }

        [Fact]
        public void ToJObject_ValidJson_ReturnsParsed()
        {
            var json = "{\"a\":1}";
            var jo = json.ToJObject();
            Assert.NotNull(jo);
            Assert.Equal(1, (int)jo!["a"]!);
        }

        [Fact]
        public void ToJObject_NullString_ReturnsEmptyObject()
        {
            string? nullStr = null;
            var jo = nullStr!.ToJObject();
            Assert.NotNull(jo);
            Assert.Empty(jo!);
        }
    }
}
