using System.Text;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Plus
{
    /// <summary>
    /// XmlUtil（XXE 加固）测试
    /// </summary>
    public class XmlUtilTests : IDisposable
    {
        private readonly string _tempDir;

        public XmlUtilTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "DotNetUtilXml_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(_tempDir, true);
            }
            catch
            {
                // 忽略清理失败
            }
        }

        private string NewXmlFile(string content)
        {
            var path = Path.Combine(_tempDir, Guid.NewGuid().ToString("N") + ".xml");
            File.WriteAllText(path, content, Encoding.UTF8);
            return path;
        }

        [Fact]
        public void LoadXmlDocSafe_NormalXml()
        {
            var path = NewXmlFile("<root><item>1</item></root>");
            var doc = XmlUtil.LoadXmlDocSafe(path);
            Assert.NotNull(doc);
            Assert.Equal("root", doc!.DocumentElement!.Name);
        }

        [Fact]
        public void LoadXmlDocSafe_XxeDtd_Throws()
        {
            // XXE 攻击样本：DOCTYPE + 外部实体，应被 DtdProcessing.Prohibit 拦截
            var path = NewXmlFile("<!DOCTYPE root [<!ENTITY xxe SYSTEM \"file:///etc/hostname\">]><root><item>&xxe;</item></root>");
            Assert.ThrowsAny<Exception>(() => XmlUtil.LoadXmlDocSafe(path));
        }

        [Fact]
        public void ReadNodes_XPath()
        {
            var path = NewXmlFile("<root><item name=\"a\">1</item><item name=\"b\">2</item></root>");
            // ReadNodes 语义：返回首个 xpath 匹配节点的子节点列表
            var nodes = XmlUtil.ReadNodes(path, "//root");
            Assert.NotNull(nodes);
            Assert.Equal(2, nodes!.Count);
        }

        [Fact]
        public void UpdateNodeInnerText_ThenRead()
        {
            var path = NewXmlFile("<root><item>old</item></root>");
            Assert.True(XmlUtil.UpdateNodeInnerText(path, "//item", "new"));
            var nodes = XmlUtil.ReadNodes(path, "//item");
            Assert.Equal("new", nodes![0]!.InnerText);
        }

        [Fact]
        public void CreateXmlFile_WritesContent()
        {
            var path = Path.Combine(_tempDir, "created.xml");
            Assert.True(XmlUtil.CreateXmlFile(path, "<root>ok</root>", null));
            var doc = XmlUtil.LoadXmlDocSafe(path);
            Assert.Equal("root", doc!.DocumentElement!.Name);
        }
    }
}
