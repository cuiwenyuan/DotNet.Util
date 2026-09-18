using System.Data;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// JsonUtil（Newtonsoft 封装）测试
    /// </summary>
    public class JsonUtilTests
    {
        [Fact]
        public void ObjectToJson_JsonToObject_Roundtrip()
        {
            var entity = new DemoEntity { Id = 1, Name = "Troy" };
            var json = JsonUtil.ObjectToJson(entity);
            Assert.Contains("\"Id\"", json);
            Assert.Contains("\"Troy\"", json);

            var back = JsonUtil.JsonToObject<DemoEntity>(json);
            Assert.NotNull(back);
            Assert.Equal(1, back!.Id);
            Assert.Equal("Troy", back.Name);
        }

        [Fact]
        public void DataTableToJson_ContainsColumns()
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Rows.Add(1, "Alice");

            var json = JsonUtil.DataTableToJson(dt);
            Assert.Contains("Alice", json);
        }

        [Fact]
        public void JsonToObject_InvalidJson_ReturnsDefault()
        {
            // 实现内吞异常并返回 default（不抛出）
            Assert.Null(JsonUtil.JsonToObject<DemoEntity>("not-json"));
        }

        [Fact]
        public void ObjectToJson_Null_ReturnsNullLiteral()
        {
            Assert.Equal("null", JsonUtil.ObjectToJson(null));
        }

        private sealed class DemoEntity
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }
    }
}
