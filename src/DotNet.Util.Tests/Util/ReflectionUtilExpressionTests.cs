using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// ReflectionUtil（Db/Expression 部分）测试
    /// 说明：ReflectionUtil 是 partial 类，本文件覆盖 Db/Expression/ReflectionUtil.cs 中的方法：
    /// IsNullable/IsCollection/IsEnumerable/IsQueryable/IsString/IsAsyncType/CreateInstance/
    /// GetUnderlyingType/LoadTypeByName/GetDefaultValue/IsDictionary/GetPropertyValue/
    /// SetPropertyValue/ToDataTable/ReplaceDataTableColumnType 等。
    /// 类名避开已有的 ReflectionUtilTests。
    /// </summary>
    public class ReflectionUtilExpressionTests
    {
        private class Sample
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }

        #region 类型判断

        [Fact]
        public void IsNullable_Type()
        {
            Assert.True(typeof(int?).IsNullable());
            Assert.False(typeof(int).IsNullable());
            Assert.False(typeof(string).IsNullable());
        }

        [Fact]
        public void IsNullable_Generic()
        {
            int? value = null;
            Assert.True(value.IsNullable());
            int plain = 0;
            Assert.False(plain.IsNullable());
        }

        [Fact]
        public void IsCollection_ListTrue_StringFalse()
        {
            Assert.True(typeof(List<int>).IsCollection());
            Assert.False(typeof(string).IsCollection());
        }

        [Fact]
        public void IsEnumerable_ListTrue_IntFalse()
        {
            Assert.True(typeof(List<int>).IsEnumerable());
            Assert.False(typeof(int).IsEnumerable());
        }

        [Fact]
        public void IsQueryable_QueryableTrue()
        {
            Assert.True(typeof(IQueryable<int>).IsQueryable());
            Assert.False(typeof(List<int>).IsQueryable());
        }

        [Fact]
        public void IsString_StringTrue_IntFalse()
        {
            Assert.True(typeof(string).IsString());
            Assert.False(typeof(int).IsString());
        }

        [Fact]
        public void IsAsyncType_TaskTrue_StringFalse()
        {
            Assert.True(typeof(Task).IsAsyncType());
            Assert.False(typeof(string).IsAsyncType());
        }

        [Fact]
        public void IsDictionary_DictionaryTrue()
        {
            Assert.True(typeof(Dictionary<string, string>).IsDictionary());
            Assert.False(typeof(List<string>).IsDictionary());
        }

        [Fact]
        public void GetUnderlyingType_Nullable_ReturnsUnderlying()
        {
            Assert.Equal(typeof(int), typeof(int?).GetUnderlyingType());
        }

        [Fact]
        public void GetUnderlyingType_Plain_ReturnsItself()
        {
            Assert.Equal(typeof(string), typeof(string).GetUnderlyingType());
        }

        [Fact]
        public void GetUnderlyingType_Task_ReturnsInnerType()
        {
            Assert.Equal(typeof(int), typeof(Task<int>).GetUnderlyingType());
        }

        #endregion

        #region 实例/默认值/加载

        [Fact]
        public void CreateInstance_NoArgs()
        {
            var instance = typeof(Sample).CreateInstance(null);

            Assert.IsType<Sample>(instance);
        }

        [Fact]
        public void CreateInstance_WithArgs()
        {
            var instance = typeof(Sample).CreateInstance(new object[0]);

            Assert.IsType<Sample>(instance);
        }

        [Fact]
        public void LoadTypeByName_ReturnsType()
        {
            var type = ReflectionUtil.LoadTypeByName("System.String");

            Assert.Equal(typeof(string), type);
        }

        [Fact]
        public void LoadTypeByName_Empty_Throws()
        {
            Assert.Throws<Exception>(() => ReflectionUtil.LoadTypeByName(""));
            Assert.Throws<Exception>(() => ReflectionUtil.LoadTypeByName(null));
        }

        [Fact]
        public void GetDefaultValue_ValueType_ZeroInstance()
        {
            Assert.Equal(0, typeof(int).GetDefaultValue());
            Assert.Null(typeof(string).GetDefaultValue());
        }

        #endregion

        #region 属性读写（表达式树）

        [Fact]
        public void GetPropertyValue_ReturnsValue()
        {
            var model = new Sample { Id = 7, Name = "Troy" };

            Assert.Equal(7, (int)ReflectionUtil.GetPropertyValue(model, "Id"));
            Assert.Equal("Troy", (string)ReflectionUtil.GetPropertyValue(model, "Name"));
        }

        [Fact]
        public void GetPropertyValue_Generic()
        {
            var model = new Sample { Name = "Troy" };

            var name = ReflectionUtil.GetPropertyValue<Sample, string>(model, "Name");

            Assert.Equal("Troy", name);
        }

        [Fact]
        public void GetPropertyValue_Missing_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => ReflectionUtil.GetPropertyValue(new Sample(), "NoSuch"));
        }

        [Fact]
        public void SetPropertyValue_Updates()
        {
            var model = new Sample();

            ReflectionUtil.SetPropertyValue(model, "Name", "New");

            Assert.Equal("New", model.Name);
        }

        [Fact]
        public void SetPropertyValue_Missing_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => ReflectionUtil.SetPropertyValue(new Sample(), "NoSuch", 1));
        }

        [Fact]
        public void GetSetPropertyValue_RoundTrip_WithCache()
        {
            // 二次调用走 CacheDictionary 缓存路径
            var model = new Sample();
            ReflectionUtil.SetPropertyValue(model, "Id", 42);
            Assert.Equal(42, (int)ReflectionUtil.GetPropertyValue(model, "Id"));
            ReflectionUtil.SetPropertyValue(model, "Id", 43);
            Assert.Equal(43, (int)ReflectionUtil.GetPropertyValue(model, "Id"));
        }

        #endregion

        #region 委托构建 / ToDataTable / 列类型替换

        [Fact]
        public void BuildObjectGetValuesDelegate_ReturnsPropertyValues()
        {
            var props = typeof(Sample).GetProperties().Where(p => p.CanRead).ToList();
            var func = ReflectionUtil.BuildObjectGetValuesDelegate<Sample>(props);

            var values = func(new Sample { Id = 1, Name = "x" });

            Assert.Equal(1, values[0]);
            Assert.Equal("x", values[1]);
        }

        [Fact]
        public void ToDataTable_BuildsFromEnumerable()
        {
            var list = new List<Sample> { new Sample { Id = 1, Name = "a" }, new Sample { Id = 2, Name = "b" } };

            var dt = list.ToDataTable();

            Assert.NotNull(dt);
            Assert.Equal(2, dt!.Rows.Count);
            Assert.True(dt.Columns.Contains("Id"));
            Assert.True(dt.Columns.Contains("Name"));
            Assert.Equal(1, dt.Rows[0]["Id"]);
            Assert.Equal("b", dt.Rows[1]["Name"]);
        }

        [Fact]
        public void ToDataTable_Empty_ReturnsSchemaOnly()
        {
            var dt = new List<Sample>().ToDataTable();

            Assert.NotNull(dt);
            Assert.Equal(0, dt!.Rows.Count);
            Assert.True(dt.Columns.Contains("Name"));
        }

        [Fact]
        public void ToDataTable_WithPropertyInfos()
        {
            var props = new List<PropertyInfo> { typeof(Sample).GetProperty("Name")! };
            var list = new List<Sample> { new Sample { Name = "only" } };

            var dt = list.ToDataTable(props);

            Assert.True(dt!.Columns.Contains("Name"));
            Assert.False(dt.Columns.Contains("Id"));
            Assert.Equal("only", dt.Rows[0]["Name"]);
        }

        [Fact]
        public void BuildGenerateObjectDelegate_ConstructsObject()
        {
            var ctor = typeof(Sample).GetConstructor(Type.EmptyTypes)!;
            var del = ReflectionUtil.BuildGenerateObjectDelegate(ctor);

            var obj = del.DynamicInvoke();

            Assert.IsType<Sample>(obj);
        }

        [Fact]
        public void ReplaceDataTableColumnType_IntToBytes()
        {
            // 实现把新列类型硬编码为 byte[]，因此 OldType→byte[] 转换才可成功
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Rows.Add(5, "x");

            var bytes = new byte[] { 5 };
            ReflectionUtil.ReplaceDataTableColumnType<int, byte[]>(dt, i => bytes);

            // 原 int 列被替换为 byte[] 列
            Assert.Same(bytes, dt.Rows[0]["Id"]);
            Assert.Equal(typeof(byte[]), dt.Columns["Id"].DataType);
        }

        [Fact]
        public void ReplaceDataTableColumnType_StringToBytes()
        {
            var dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Rows.Add("x");

            ReflectionUtil.ReplaceDataTableColumnType<int, byte[]>(dt, i => new byte[] { 1 });

            // 无匹配列时不变
            Assert.Equal("x", dt.Rows[0]["Name"]);
            Assert.Equal(typeof(string), dt.Columns["Name"].DataType);
        }

        #endregion
    }
}
