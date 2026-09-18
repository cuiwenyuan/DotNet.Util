using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// XmlConfigUtil 测试
    /// 构造函数强制配置文件位于应用程序基目录内，因此测试在基目录下的 XmlConfigTests 子目录中使用独立文件
    /// </summary>
    public class XmlConfigUtilTests : IDisposable
    {
        private readonly List<string> _createdFiles = new();

        private XmlConfigUtil CreateUtil()
        {
            var relativePath = "XmlConfigTests" + Path.DirectorySeparatorChar + Guid.NewGuid().ToString("N") + ".config";
            _createdFiles.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath));
            return new XmlConfigUtil(relativePath);
        }

        public void Dispose()
        {
            foreach (var file in _createdFiles)
            {
                try
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
                catch (Exception)
                {
                    // 忽略清理失败
                }
            }
        }

        [Fact]
        public void NewInstance_CreatesFileWithDefaultNode()
        {
            var util = CreateUtil();

            Assert.True(File.Exists(_createdFiles[0]));
            Assert.Contains(XmlConfigUtil.DefaultNodeName, util.GetNodes());
        }

        [Fact]
        public void SetValue_GetValue_Roundtrip()
        {
            var util = CreateUtil();

            Assert.True(util.SetValue("age", "23"));
            Assert.Equal("23", util.GetValue("age"));

            Assert.True(util.SetValue("age", "24"));
            Assert.Equal("24", util.GetValue("age"));
        }

        [Fact]
        public void GetValue_MissingKey_ReturnsDefaultValueAndCreatesItem()
        {
            var util = CreateUtil();

            Assert.Equal("fallback", util.GetValue("nokey", "fallback"));
            Assert.Contains("nokey", util.GetAllKey());
        }

        [Fact]
        public void GetAllKeyValue_ReturnsAllItemsOfNode()
        {
            var util = CreateUtil();
            util.SetValue("k1", "v1");
            util.SetValue("k2", "v2");

            var keyValues = util.GetAllKeyValue();

            Assert.Equal("v1", keyValues["k1"]);
            Assert.Equal("v2", keyValues["k2"]);
            Assert.Contains("k1", util.GetAllKey());
            Assert.Contains("v2", util.GetAllValue());
        }

        [Fact]
        public void DeleteValue_RemovesItem()
        {
            var util = CreateUtil();
            util.SetValue("age", "23");
            Assert.Contains("age", util.GetAllKey());

            Assert.True(util.DeleteValue("age"));

            Assert.DoesNotContain("age", util.GetAllKey());
        }

        [Fact]
        public void SetValue_CustomNode_CreatesNode()
        {
            var util = CreateUtil();

            Assert.True(util.SetValue("test1", "value1", "node1"));

            Assert.Contains("node1", util.GetNodes());
            Assert.Equal("value1", util.GetValue("test1", null, "node1"));
        }

        [Fact]
        public void DeleteNode_RemovesWholeNode()
        {
            var util = CreateUtil();
            util.SetValue("test1", "value1", "node1");
            Assert.Contains("node1", util.GetNodes());

            Assert.True(util.DeleteNode("node1"));

            Assert.DoesNotContain("node1", util.GetNodes());
        }

        [Fact]
        public void DeleteNode_InvalidName_ReturnsFalse()
        {
            var util = CreateUtil();

            Assert.False(util.DeleteNode("bad name"));
            Assert.False(util.DeleteNode(string.Empty));
        }

        [Fact]
        public void InvalidKey_WithQuote_IsRejected()
        {
            var util = CreateUtil();

            // IsValidXmlAttribute 拒绝包含单/双引号的键，防止 XPath 注入
            Assert.False(util.SetValue("a'b", "v"));
            Assert.Equal(string.Empty, util.GetValue("a'b"));
            Assert.False(util.DeleteValue("a\"b"));
        }

        [Fact]
        public void Save_ReturnsTrue()
        {
            var util = CreateUtil();
            util.SetValue("age", "23");

            Assert.True(util.Save());
        }

        [Fact]
        public void NewInstance_PathOutsideBaseDirectory_Throws()
        {
            Assert.Throws<ArgumentException>(() => new XmlConfigUtil(".." + Path.DirectorySeparatorChar + "outside.config"));
        }

        [Fact]
        public void NewInstance_PathWithInvalidChars_Throws()
        {
            Assert.Throws<ArgumentException>(() => new XmlConfigUtil("XmlConfigTests" + Path.DirectorySeparatorChar + "bad*name.config"));
        }
    }
}
