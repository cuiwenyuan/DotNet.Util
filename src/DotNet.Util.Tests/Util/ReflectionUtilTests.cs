using System.Data;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// ReflectionUtil 测试
    /// </summary>
    public class ReflectionUtilTests
    {
        private class Sample
        {
            public int PublicField;
            public string? Name { get; set; } = "init";
            public int Count { get; set; }
            public int Double(int x) => x * 2;
        }

        [Fact]
        public void SetGetField_RoundTrips()
        {
            var obj = new Sample();
            ReflectionUtil.SetField(obj, "PublicField", 42);
            Assert.Equal(42, ReflectionUtil.GetField(obj, "PublicField"));
        }

        [Fact]
        public void SetGetProperty_RoundTrips()
        {
            var obj = new Sample();
            ReflectionUtil.SetProperty(obj, "Name", "Troy");
            Assert.Equal("Troy", ReflectionUtil.GetProperty(obj, "Name"));
        }

        [Fact]
        public void SetProperty_ConvertsValueType()
        {
            var obj = new Sample();
            ReflectionUtil.SetProperty(obj, "Count", 7);
            Assert.Equal(7, ReflectionUtil.GetProperty(obj, "Count"));
        }

        [Fact]
        public void ChangeType2_StringToInt()
        {
            Assert.Equal(123, (int)ReflectionUtil.ChangeType2("123", typeof(int)));
        }

        [Fact]
        public void ChangeType2_StringToBool()
        {
            Assert.Equal(true, (bool)ReflectionUtil.ChangeType2("true", typeof(bool)));
        }

        [Fact]
        public void ChangeType2_NullOrEmpty_ReturnsNull()
        {
            Assert.Null(ReflectionUtil.ChangeType2(null, typeof(int)));
            Assert.Null(ReflectionUtil.ChangeType2("", typeof(int)));
            Assert.Null(ReflectionUtil.ChangeType2(System.DBNull.Value, typeof(int)));
        }

        [Fact]
        public void ChangeType2_NullableUnderlyingType()
        {
            Assert.Equal(5, (int)ReflectionUtil.ChangeType2("5", typeof(int?)));
        }

        [Fact]
        public void GetPropertyNames_ReturnsOnlyProperties()
        {
            var names = ReflectionUtil.GetPropertyNames(new Sample());
            Assert.Contains("Name", names);
            Assert.Contains("Count", names);
            Assert.DoesNotContain("PublicField", names);
        }

        [Fact]
        public void GetPropertyNameTypes_MapsFullName()
        {
            var map = ReflectionUtil.GetPropertyNameTypes(new Sample());
            Assert.True(map.ContainsKey("Name"));
            Assert.Equal("System.String", map["Name"]);
        }

        [Fact]
        public void InvokeMethod_CallsInstanceMethod()
        {
            var obj = new Sample();
            var result = ReflectionUtil.InvokeMethod(obj, "Double", new object[] { 5 });
            Assert.Equal(10, (int)result);
        }

        [Fact]
        public void CreateTable_BuildsFromList()
        {
            var list = new List<Sample> { new Sample { Name = "x", Count = 3 } };
            var dt = ReflectionUtil.CreateTable(list);

            Assert.NotNull(dt);
            Assert.True(dt!.Columns.Contains("Name"));
            Assert.Equal(1, dt.Rows.Count);
            Assert.Equal("x", dt.Rows[0]!["Name"].ToString());
            Assert.Equal(3, Convert.ToInt32(dt.Rows[0]!["Count"]));
        }

        [Fact]
        public void CreateTable_Null_ReturnsNull()
        {
            Assert.Null(ReflectionUtil.CreateTable(null!));
        }
    }
}
