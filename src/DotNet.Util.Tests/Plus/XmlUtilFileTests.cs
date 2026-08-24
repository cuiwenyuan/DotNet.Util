using System.IO;
using System.Text;
using System.Xml;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Plus
{
    /// <summary>
    /// XmlUtil 文件操作补充测试（AppendChild/Read/Insert/Update/GetTemplate/LoadXmlDoc）
    /// 类名避开已有的 XmlUtilTests（XXE 加固 5 例）
    /// </summary>
    public class XmlUtilFileTests : System.IDisposable
    {
        private readonly string _tempDir;

        public XmlUtilFileTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "DotNetUtilXml2_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            try { Directory.Delete(_tempDir, true); } catch { }
        }

        private string NewXmlFile(string content)
        {
            var path = Path.Combine(_tempDir, Guid.NewGuid().ToString("N") + ".xml");
            File.WriteAllText(path, content, Encoding.UTF8);
            return path;
        }

        [Fact]
        public void LoadXmlDocumentSecure_NullPath_ReturnsNull()
        {
            Assert.Null(XmlUtil.LoadXmlDocumentSecure(null));
            Assert.Null(XmlUtil.LoadXmlDocumentSecure(""));
        }

        [Fact]
        public void LoadXmlDoc_NormalFile()
        {
            var path = NewXmlFile("<root><a>1</a></root>");
            var doc = XmlUtil.LoadXmlDoc(path);

            Assert.NotNull(doc);
            Assert.Equal("root", doc!.DocumentElement!.Name);
        }

        [Fact]
        public void LoadXmlDoc_NullPath_ReturnsNull()
        {
            Assert.Null(XmlUtil.LoadXmlDoc(null));
            Assert.Null(XmlUtil.LoadXmlDoc(""));
        }

        [Fact]
        public void LoadXmlDoc_MissingFile_ReturnsNull()
        {
            // 文件不存在时抛异常被内部捕获，返回 null
            var path = Path.Combine(_tempDir, "nope.xml");
            Assert.Null(XmlUtil.LoadXmlDoc(path));
        }

        [Fact]
        public void AppendChild_AppendsNode()
        {
            var path = NewXmlFile("<root><items></items></root>");
            var node = new XmlDocument().CreateElement("item");
            node.InnerText = "x";

            var ok = XmlUtil.AppendChild(path, "/root/items", node);

            Assert.True(ok);
            var doc = XmlUtil.LoadXmlDocSafe(path);
            Assert.Equal(1, doc!.SelectNodes("/root/items/item")!.Count);
        }

        [Fact]
        public void AppendChild_InvalidPath_ReturnsFalse()
        {
            var ok = XmlUtil.AppendChild(Path.Combine(_tempDir, "missing.xml"), "/root", new XmlDocument().CreateElement("item"));
            Assert.False(ok);
        }

        [Fact]
        public void Read_InnerText()
        {
            var path = NewXmlFile("<root><name>Troy</name></root>");
            var value = XmlUtil.Read(path, "/root/name", "");

            Assert.Equal("Troy", value);
        }

        [Fact]
        public void Read_Attribute()
        {
            var path = NewXmlFile("<root><item key=\"K1\">v</item></root>");
            var value = XmlUtil.Read(path, "/root/item", "key");

            Assert.Equal("K1", value);
        }

        [Fact]
        public void Read_MissingNode_ReturnsEmpty()
        {
            var path = NewXmlFile("<root><a>1</a></root>");
            Assert.Equal(string.Empty, XmlUtil.Read(path, "/root/nope", ""));
        }

        [Fact]
        public void Read_EmptyPath_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, XmlUtil.Read("", "/root", ""));
        }

        [Fact]
        public void Insert_ElementWithInnerText()
        {
            var path = NewXmlFile("<root></root>");
            XmlUtil.Insert(path, "/root", "item", "", "hello");

            var value = XmlUtil.Read(path, "/root/item", "");
            Assert.Equal("hello", value);
        }

        [Fact]
        public void Insert_ElementWithAttribute()
        {
            var path = NewXmlFile("<root></root>");
            XmlUtil.Insert(path, "/root", "item", "code", "C1");

            var value = XmlUtil.Read(path, "/root/item", "code");
            Assert.Equal("C1", value);
        }

        [Fact]
        public void Insert_AttributeOnExistingNode()
        {
            var path = NewXmlFile("<root><item></item></root>");
            XmlUtil.Insert(path, "/root/item", "", "attr", "A1");

            var value = XmlUtil.Read(path, "/root/item", "attr");
            Assert.Equal("A1", value);
        }

        [Fact]
        public void Update_InnerText()
        {
            var path = NewXmlFile("<root><name>old</name></root>");
            XmlUtil.Update(path, "/root/name", "", "new");

            var value = XmlUtil.Read(path, "/root/name", "");
            Assert.Equal("new", value);
        }

        [Fact]
        public void Update_Attribute()
        {
            var path = NewXmlFile("<root><item key=\"old\"></item></root>");
            XmlUtil.Update(path, "/root/item", "key", "newKey");

            var value = XmlUtil.Read(path, "/root/item", "key");
            Assert.Equal("newKey", value);
        }

        [Fact]
        public void GetTemplate_ReadsFileContent()
        {
            var path = NewXmlFile("<root>template</root>");
            var content = XmlUtil.GetTemplate(path);

            Assert.Contains("template", content);
        }

        [Fact]
        public void GetTemplate_EmptyPath_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, XmlUtil.GetTemplate(""));
        }
    }
}
