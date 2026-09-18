using System.Xml;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// UserConfigUtil XML 解析测试（构造内存 XmlDocument，不读写文件）
    /// </summary>
    public class UserConfigUtilTests
    {
        private static XmlDocument CreateConfig()
        {
            var doc = new XmlDocument();
            doc.LoadXml(@"<configuration>
  <appSettings>
    <add key=""ServerDbType"" value=""SqlServer"" />
    <add key=""WebHost"" value=""http://localhost/"" Options=""a;b;c"" />
    <add key=""EmptyValue"" value="""" />
  </appSettings>
</configuration>");
            return doc;
        }

        [Fact]
        public void GetValue_FindsKey()
        {
            var value = UserConfigUtil.GetValue(CreateConfig(), "//appSettings/add", "ServerDbType");

            Assert.Equal("SqlServer", value);
        }

        [Fact]
        public void GetValue_CaseInsensitive()
        {
            var value = UserConfigUtil.GetValue(CreateConfig(), "//appSettings/add", "serverdbtype");

            Assert.Equal("SqlServer", value);
        }

        [Fact]
        public void GetValue_MissingKey_ReturnsEmpty()
        {
            var value = UserConfigUtil.GetValue(CreateConfig(), "//appSettings/add", "NoSuchKey");

            Assert.Equal(string.Empty, value);
        }

        [Fact]
        public void GetValue_EmptyValue_ReturnsEmpty()
        {
            var value = UserConfigUtil.GetValue(CreateConfig(), "//appSettings/add", "EmptyValue");

            Assert.Equal(string.Empty, value);
        }

        [Fact]
        public void GetValue_MissingKeyAttribute_DoesNotThrow()
        {
            // 节点无 key 属性时不应抛 NullReferenceException
            var doc = new XmlDocument();
            doc.LoadXml(@"<configuration><appSettings><add value=""x"" /></appSettings></configuration>");

            var value = UserConfigUtil.GetValue(doc, "//appSettings/add", "key");

            Assert.Equal(string.Empty, value);
        }

        [Fact]
        public void GetValue_DefaultSelectPath_Works()
        {
            var value = UserConfigUtil.GetValue(CreateConfig(), "WebHost");

            Assert.Equal("http://localhost/", value);
        }

        [Fact]
        public void GetOption_FindsKey()
        {
            var options = UserConfigUtil.GetOption(CreateConfig(), "//appSettings/add", "WebHost");

            Assert.Equal("a;b;c", options);
        }

        [Fact]
        public void GetOption_MissingKey_ReturnsEmpty()
        {
            var options = UserConfigUtil.GetOption(CreateConfig(), "//appSettings/add", "NoSuch");

            Assert.Equal(string.Empty, options);
        }

        [Fact]
        public void GetOption_NoOptionsAttribute_ReturnsEmpty()
        {
            var options = UserConfigUtil.GetOption(CreateConfig(), "//appSettings/add", "ServerDbType");

            Assert.Equal(string.Empty, options);
        }
    }
}
