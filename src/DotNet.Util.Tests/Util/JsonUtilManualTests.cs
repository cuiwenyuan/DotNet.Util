using System.Collections.Generic;
using System.Data;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// JsonUtil.Manual 测试（GetJsonStr/JsonToDictionary/GetJosnValue/Split/ToXml/JsonToDataTable）
    /// 类名避开已有的 JsonUtilTests
    /// </summary>
    public class JsonUtilManualTests
    {
        #region GetJsonStr / JsonToDictionary

        [Fact]
        public void GetJsonStr_BuildsJson()
        {
            var dict = new Dictionary<string, string>
            {
                { "ret", "err" },
                { "stadname", "未知" }
            };

            var json = JsonUtil.GetJsonStr(dict);

            Assert.Equal("{\"ret\":\"err\",\"stadname\":\"未知\"}", json);
        }

        [Fact]
        public void GetJsonStr_EmptyDict_ReturnsBraces()
        {
            Assert.Equal("{}", JsonUtil.GetJsonStr(new Dictionary<string, string>()));
        }

        [Fact]
        public void JsonToDictionary_RoundTrip()
        {
            var dict = JsonUtil.JsonToDictionary("{\"a\":1,\"b\":\"x\"}");

            Assert.NotNull(dict);
            Assert.Equal(2, dict!.Count);
            Assert.Equal(1L, dict["a"]);
            Assert.Equal("x", dict["b"]);
        }

        [Fact]
        public void JsonToDictionary_Invalid_Throws()
        {
            Assert.Throws<System.Exception>(() => JsonUtil.JsonToDictionary("not-json"));
        }

        #endregion

        #region IsJson / GetJosnValue / Split / SplitArray

        [Fact]
        public void IsJson_Valid()
        {
            Assert.True(JsonUtil.IsJson("{\"a\":1}"));
            Assert.False(JsonUtil.IsJson("abc"));
            Assert.False(JsonUtil.IsJson(null));
        }

        [Fact]
        public void GetJosnValue_ExistingKey()
        {
            var value = JsonUtil.GetJosnValue("{\"name\":\"Troy\"}", "name");

            Assert.Equal("Troy", value);
        }

        [Fact]
        public void GetJosnValue_MissingKey_ReturnsEmpty()
        {
            var value = JsonUtil.GetJosnValue("{\"name\":\"Troy\"}", "nope");

            Assert.Equal(string.Empty, value);
        }

        [Fact]
        public void GetJosnValue_EmptyJson_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, JsonUtil.GetJosnValue("", "a"));
            Assert.Equal(string.Empty, JsonUtil.GetJosnValue(null, "a"));
        }

        [Fact]
        public void Split_ObjectToDictionary()
        {
            var dict = JsonUtil.Split("{\"a\":\"1\",\"b\":\"2\"}");

            Assert.NotNull(dict);
            Assert.Equal("1", dict!["a"]);
            Assert.Equal("2", dict["b"]);
        }

        [Fact]
        public void SplitArray_MultipleObjects()
        {
            var list = JsonUtil.SplitArray("[{\"a\":\"1\"},{\"a\":\"2\"}]");

            Assert.NotNull(list);
            Assert.Equal(2, list!.Count);
            Assert.Equal("2", list[1]["a"]);
        }

        [Fact]
        public void SplitArray_Empty_ReturnsNull()
        {
            Assert.Null(JsonUtil.SplitArray(""));
            Assert.Null(JsonUtil.SplitArray(null));
        }

        #endregion

        #region ToXml

        [Fact]
        public void ToXml_SimpleObject()
        {
            var xml = JsonUtil.ToXml("{\"name\":\"Troy\"}");

            Assert.StartsWith("<?xml", xml);
            Assert.Contains("<name>Troy</name>", xml);
        }

        [Fact]
        public void ToXml_MultipleKeys_AddsRoot()
        {
            var xml = JsonUtil.ToXml("{\"a\":\"1\",\"b\":\"2\"}");

            Assert.Contains("<root>", xml);
            Assert.Contains("</root>", xml);
        }

        [Fact]
        public void ToXml_EmptyJson_ReturnsHeaderOnly()
        {
            var xml = JsonUtil.ToXml("{}");

            Assert.StartsWith("<?xml", xml);
        }

        #endregion

        #region JsonToDataTable

        [Fact]
        public void JsonToDataTable_SingleRow()
        {
            var dt = JsonUtil.JsonToDataTable("{\"table\":[{\"name\":\"Troy\",\"age\":30}]}");

            Assert.NotNull(dt);
            // 实现中表名提取依赖正则，替换分隔符后取不到，实际为空
            Assert.Equal(1, dt!.Rows.Count);
            Assert.True(dt.Columns.Contains("name"));
            Assert.Equal("Troy", dt.Rows[0]["name"].ToString());
        }

        [Fact]
        public void JsonToDataTable_MultipleRows()
        {
            var dt = JsonUtil.JsonToDataTable("{\"t\":[{\"id\":\"1\"},{\"id\":\"2\"}]}");

            Assert.NotNull(dt);
            Assert.Equal(2, dt!.Rows.Count);
        }

        #endregion

        #region R9-5 / R9-6 回归

        [Fact]
        public void Split_PreservesCaseVariantKeys()
        {
            // 修复 R9-5：原 OrdinalIgnoreCase 会把 "Id"/"id" 判重而静默丢数据
            var dict = JsonUtil.Split("{\"Id\":\"1\",\"id\":\"2\"}");

            Assert.NotNull(dict);
            Assert.True(dict!.ContainsKey("Id"));
            Assert.True(dict.ContainsKey("id"));
            Assert.Equal("1", dict["Id"]);
            Assert.Equal("2", dict["id"]);
        }

        [Fact]
        public void GetJosnValue_CaseSensitiveLookup()
        {
            // 修复 R9-5：按 "id" 取应得 2，而非被 "Id" 覆盖
            var value = JsonUtil.GetJosnValue("{\"Id\":\"1\",\"id\":\"2\"}", "id");

            Assert.Equal("2", value);
        }

        [Fact]
        public void GetJsonStr_EscapesSpecialChars()
        {
            // 修复 R9-6：键值含 " \ 换行时必须产出合法 JSON（可反序列化回去）
            var dict = new Dictionary<string, string>
            {
                { "k", "a\"b\\c" },
                { "note", "line1\nline2" }
            };
            var json = JsonUtil.GetJsonStr(dict);

            var back = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Assert.NotNull(back);
            Assert.Equal("a\"b\\c", back!["k"]);
            Assert.Equal("line1\nline2", back["note"]);
        }

        [Fact]
        public void GetJsonStr_NullDict_ReturnsEmptyObject()
        {
            // 修复 R9-6：null 入参不再 NRE，返回 "{}"
            Assert.Equal("{}", JsonUtil.GetJsonStr(null));
        }

        #endregion
    }
}
