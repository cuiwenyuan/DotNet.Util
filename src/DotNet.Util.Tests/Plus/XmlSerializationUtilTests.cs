using System;
using System.IO;
using System.Xml.Serialization;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Plus
{
    /// <summary>
    /// XmlSerializationUtil（XML 序列化/反序列化）测试。
    /// 使用临时文件并在测试结束后清理，不依赖外部服务。
    /// </summary>
    public class XmlSerializationUtilTests
    {
        // XmlSerializer 要求可公开无参构造的普通类
        public class Person
        {
            public string? Name { get; set; }
            public int Age { get; set; }
        }

        private static string NewTempFile() => Path.GetTempFileName();

        [Fact]
        public void Save_Then_Load_RoundTrips()
        {
            var file = NewTempFile();
            try
            {
                var person = new Person { Name = "Troy", Age = 18 };
                XmlSerializationUtil.Save(person, file);

                Assert.True(File.Exists(file));

                var loaded = XmlSerializationUtil.Load(typeof(Person), file) as Person;
                Assert.NotNull(loaded);
                Assert.Equal("Troy", loaded!.Name);
                Assert.Equal(18, loaded.Age);
            }
            finally
            {
                if (File.Exists(file)) File.Delete(file);
            }
        }

        [Fact]
        public void Load_NonExistentFile_ReturnsNull()
        {
            var file = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".xml");
            Assert.False(File.Exists(file));
            var result = XmlSerializationUtil.Load(typeof(Person), file);
            Assert.Null(result);
        }

        [Fact]
        public void Save_Then_Load_PreservesSpecialCharacters()
        {
            var file = NewTempFile();
            try
            {
                // 包含 XML 特殊字符与中文，验证序列化不丢失内容
                var person = new Person { Name = "Troy & <友> ", Age = 30 };
                XmlSerializationUtil.Save(person, file);

                var loaded = XmlSerializationUtil.Load(typeof(Person), file) as Person;
                Assert.NotNull(loaded);
                Assert.Equal("Troy & <友> ", loaded!.Name);
                Assert.Equal(30, loaded.Age);
            }
            finally
            {
                if (File.Exists(file)) File.Delete(file);
            }
        }
    }
}
