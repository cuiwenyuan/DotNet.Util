using DotNet.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.XPath;

namespace DotNet.Util
{
    /// <summary>
    /// var xmlConfigUtil = new XmlConfigUtil();
    /// xmlConfigUtil.GetValue("age");
    /// xmlConfigUtil.GetValue("test1","node1");
    /// xmlConfigUtil.GetValue("test1","node2");
    /// xmlConfigUtil.SetValue("age","23"); // 将 default 节点下的 key = age 的 value 设置为 23
    /// xmlConfigUtil.Delete("age"); // 删除 default 节点下的 key = age 记录
    /// xmlConfigUtil.DeleteNode("node1"); // node1 节点将会被全部删除
    /// </summary>
    public partial class XmlConfigUtil
    {
        /// <summary>
        /// 默认节点名
        /// </summary>
        public const string DefaultNodeName = "default";

        /// <summary>
        /// 默认键值对节点名
        /// </summary>
        public const string DefaultItemName = "item";

        /// <summary>
        /// xml文档
        /// </summary>
        private readonly XmlDocument _doc = new();

        /// <summary>
        /// 配置文件全名
        /// </summary>
        private readonly string _fullName;

        /// <summary>
        /// 自动保存
        /// </summary>
        private readonly bool _autoSave = true;

        /// <summary>
        /// 读写锁，保证线程安全
        /// </summary>
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="xmlPath">XML路径，默认为XmlConfig\Config.config</param>
        public XmlConfigUtil(string xmlPath = "XmlConfig\\Config.config")
        {
            // 增强路径安全验证，防止路径遍历攻击
            var fileName = Path.GetFileName(xmlPath);
            if (!xmlPath.EndsWith(fileName) || ContainsInvalidPathChars(xmlPath))
            {
                throw new ArgumentException("Invalid path: path traversal or invalid characters detected", nameof(xmlPath));
            }

            _fullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, xmlPath);

            // 验证最终路径是否仍然安全
            var fullPath = Path.GetFullPath(_fullName);
            var baseDir = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
            if (!fullPath.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid path: outside of application directory", nameof(xmlPath));
            }

            try
            {
                _lock.EnterWriteLock();

                if (!File.Exists(_fullName))
                {
                    var directoryName = Path.GetDirectoryName(_fullName);
                    if (!string.IsNullOrWhiteSpace(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    // 配置XML设置以防止XXE
                    _doc.XmlResolver = null;
                    _doc.AppendChild(_doc.CreateXmlDeclaration("1.0", "UTF-8", null));
                    _doc.AppendChild(_doc.CreateElement("root"));
                    CreateNode(DefaultNodeName);
                    _doc.Save(_fullName);
                }

                // 配置XML设置以防止XXE
                _doc.XmlResolver = null;
                _doc.Load(_fullName);
            }
            finally
            {
                if (_lock.IsWriteLockHeld)
                    _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            try
            {
                _lock.EnterWriteLock();
                _doc.Save(_fullName);
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return false;
            }
            finally
            {
                if (_lock.IsWriteLockHeld)
                    _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 获取指定键的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="nodeName">节点名</param>
        /// <param name="itemName">键值对节点名</param>
        /// <returns></returns>
        public string GetValue(string key, string defaultValue = null, string nodeName = DefaultNodeName, string itemName = DefaultItemName)
        {
            defaultValue ??= string.Empty;
            if (nodeName.IsNullOrEmpty())
            {
                nodeName = DefaultNodeName;
            }

            // 验证参数安全性
            if (!IsValidXmlName(nodeName) || !IsValidXmlName(itemName) || !IsValidXmlAttribute(key))
            {
                return string.Empty;
            }

            try
            {
                _lock.EnterReadLock();

                var node = SelectSingleNodeSafe($"/root/{EscapeXPathName(nodeName)}");
                if (node == null)
                {
                    // 在读锁中发现不存在，升级为写锁创建节点
                    _lock.ExitReadLock();
                    _lock.EnterWriteLock();

                    // 双重检查
                    node = SelectSingleNodeSafe($"/root/{EscapeXPathName(nodeName)}");
                    if (node == null)
                    {
                        CreateNode(nodeName);
                        node = SelectSingleNodeSafe($"/root/{EscapeXPathName(nodeName)}");
                    }

                    _lock.ExitWriteLock();
                    _lock.EnterReadLock();
                }

                // Use escaped attribute literal without adding extra quotes around it
                var item = SelectSingleNodeSafe($"/root/{EscapeXPathName(nodeName)}/{EscapeXPathName(itemName)}[@key={EscapeXPathAttributeValue(key)}]") as XmlElement;
                if (item == null)
                {
                    // 退出读锁，进入写锁来创建新项目
                    _lock.ExitReadLock();
                    _lock.EnterWriteLock();

                    // 双重检查
                    item = SelectSingleNodeSafe($"/root/{EscapeXPathName(nodeName)}/{EscapeXPathName(itemName)}[@key={EscapeXPathAttributeValue(key)}]") as XmlElement;
                    if (item == null)
                    {
                        //自动创建item                    
                        var itemNew = _doc.CreateElement(itemName);
                        itemNew.SetAttribute("key", key);
                        itemNew.SetAttribute("value", defaultValue);
                        node = SelectSingleNodeSafe($"/root/{EscapeXPathName(nodeName)}") as XmlElement;
                        if (node != null)
                        {
                            node.AppendChild(itemNew);
                            if (_autoSave)
                            {
                                _lock.ExitWriteLock();
                                Save(); // Save方法内部会处理锁
                                _lock.EnterWriteLock(); // 重新获取锁以便返回
                            }
                        }
                    }
                    else
                    {
                        // 其他线程已经创建了该项目，直接返回其值
                        var result = item.GetAttribute("value");
                        _lock.ExitWriteLock();
                        return result;
                    }

                    _lock.ExitWriteLock();
                    return defaultValue;
                }
                else
                {
                    var result = item.GetAttribute("value");
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return string.Empty;
            }
            finally
            {
                if (_lock.IsReadLockHeld)
                    _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// 更改指定键的值 没有则添加
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="nodeName">节点名</param>
        /// <param name="itemName">键值对节点名</param>
        public bool SetValue(string key, string value, string nodeName = DefaultNodeName, string itemName = DefaultItemName)
        {
            value ??= string.Empty;
            if (nodeName.IsNullOrEmpty())
            {
                nodeName = DefaultNodeName;
            }

            // 验证参数安全性
            if (!IsValidXmlName(nodeName) || !IsValidXmlName(itemName) || !IsValidXmlAttribute(key))
            {
                return false;
            }

            try
            {
                _lock.EnterWriteLock();

                var node = SelectSingleNodeSafe($"/root/{EscapeXPathName(nodeName)}") as XmlElement;
                if (node == null)
                {
                    CreateNode(nodeName);
                    node = SelectSingleNodeSafe($"/root/{EscapeXPathName(nodeName)}") as XmlElement;
                }

                // Use escaped attribute literal without adding extra quotes around it
                var item = SelectSingleNodeSafe($"/root/{EscapeXPathName(nodeName)}/{EscapeXPathName(itemName)}[@key={EscapeXPathAttributeValue(key)}]") as XmlElement;
                if (item == null)
                {
                    //自动创建item
                    var itemNew = _doc.CreateElement(itemName);
                    itemNew.SetAttribute("key", key);
                    itemNew.SetAttribute("value", value);
                    node.AppendChild(itemNew);
                }
                else
                {
                    item.SetAttribute("value", value);
                }

                if (_autoSave)
                {
                    _doc.Save(_fullName);
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return false;
            }
            finally
            {
                if (_lock.IsWriteLockHeld)
                    _lock.ExitWriteLock();
            }
            return true;
        }

        /// <summary>
        /// 获取所有的键
        /// </summary>
        /// <param name="nodeName">节点名</param>
        /// <returns></returns>
        public List<string> GetAllKey(string nodeName = DefaultNodeName)
        {
            if (nodeName.IsNullOrEmpty())
            {
                nodeName = DefaultNodeName;
            }

            // 验证参数安全性
            if (!IsValidXmlName(nodeName))
            {
                return new List<string>();
            }

            try
            {
                _lock.EnterReadLock();

                var keys = new List<string>();
                var xmlElement = SelectSingleNodeSafe($"/root/{EscapeXPathName(nodeName)}") as XmlElement;
                if (xmlElement != null)
                {
                    foreach (XmlNode node in xmlElement.ChildNodes)
                    {
                        if (node is XmlElement element)
                        {
                            keys.Add(element.GetAttribute("key"));
                        }
                    }
                }

                return keys;
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return new List<string>();
            }
            finally
            {
                if (_lock.IsReadLockHeld)
                    _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// 获取所有的键
        /// </summary>
        /// <param name="nodeName">节点名</param>
        /// <returns></returns>
        public List<string> GetAllValue(string nodeName = DefaultNodeName)
        {
            if (nodeName.IsNullOrEmpty())
            {
                nodeName = DefaultNodeName;
            }

            // 验证参数安全性
            if (!IsValidXmlName(nodeName))
            {
                return new List<string>();
            }

            try
            {
                _lock.EnterReadLock();

                var values = new List<string>();
                var xmlElement = SelectSingleNodeSafe($"/root/{EscapeXPathName(nodeName)}") as XmlElement;
                if (xmlElement != null)
                {
                    foreach (XmlNode node in xmlElement.ChildNodes)
                    {
                        if (node is XmlElement element)
                        {
                            values.Add(element.GetAttribute("value"));
                        }
                    }
                }

                return values;
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return new List<string>();
            }
            finally
            {
                if (_lock.IsReadLockHeld)
                    _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// 获取所有的键/值
        /// </summary>
        /// <param name="nodeName">节点名</param>
        /// <returns></returns>
        public Dictionary<string, string> GetAllKeyValue(string nodeName = DefaultNodeName)
        {
            if (nodeName.IsNullOrEmpty())
            {
                nodeName = DefaultNodeName;
            }

            // 验证参数安全性
            if (!IsValidXmlName(nodeName))
            {
                return new Dictionary<string, string>();
            }

            try
            {
                _lock.EnterReadLock();

                var keyValues = new Dictionary<string, string>();
                var xmlElement = SelectSingleNodeSafe($"/root/{EscapeXPathName(nodeName)}") as XmlElement;
                if (xmlElement != null)
                {
                    foreach (XmlNode node in xmlElement.ChildNodes)
                    {
                        if (node is XmlElement element)
                        {
                            var key = element.GetAttribute("key");
                            if (!key.IsNullOrEmpty())
                            {
                                var value = element.GetAttribute("value");
                                keyValues[key] = value; // 使用字典的覆盖特性，如果有重复键，取最后一个
                            }
                        }
                    }
                }


                return keyValues;
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return new Dictionary<string, string>();
            }
            finally
            {
                if (_lock.IsReadLockHeld)
                    _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// 删除指定键的项
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="nodeName">节点名</param>
        /// <param name="itemName">键值对节点名</param>
        /// 
        public bool DeleteValue(string key, string nodeName = DefaultNodeName, string itemName = DefaultItemName)
        {
            if (nodeName.IsNullOrEmpty())
            {
                nodeName = DefaultNodeName;
            }

            // 验证参数安全性
            if (!IsValidXmlName(nodeName) || !IsValidXmlName(itemName) || !IsValidXmlAttribute(key))
            {
                return false;
            }

            try
            {
                _lock.EnterWriteLock();

                CreateNode(nodeName);
                var keyValue = SelectSingleNodeSafe($"/root/{EscapeXPathName(nodeName)}") as XmlElement;
                var xmlElement = SelectSingleNodeSafe($"/root/{EscapeXPathName(nodeName)}/{EscapeXPathName(itemName)}[@key={EscapeXPathAttributeValue(key)}]") as XmlElement;
                if (keyValue != null && xmlElement != null)
                {
                    keyValue.RemoveChild(xmlElement);
                }

                if (_autoSave)
                {
                    _doc.Save(_fullName);
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return false;
            }
            finally
            {
                if (_lock.IsWriteLockHeld)
                    _lock.ExitWriteLock();
            }
            return true;
        }


        /// <summary>
        /// 获取所有的节点名称
        /// </summary>
        /// <returns></returns>
        public List<string> GetNodes()
        {
            try
            {
                _lock.EnterReadLock();

                var nodes = new List<string>();
                var xmlElement = SelectSingleNodeSafe($"/root") as XmlElement;
                if (xmlElement != null)
                {
                    foreach (XmlNode node in xmlElement.ChildNodes)
                    {
                        if (node is XmlElement element)
                        {
                            nodes.Add(element.Name);
                        }
                    }
                }

                return nodes;
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return new List<string>();
            }
            finally
            {
                if (_lock.IsReadLockHeld)
                    _lock.ExitReadLock();
            }
        }


        /// <summary>
        /// 检查创建节点
        /// </summary>
        /// <param name="nodeName"></param>
        private void CreateNode(string nodeName = DefaultNodeName)
        {
            if (nodeName.IsNullOrEmpty())
            {
                nodeName = DefaultNodeName;
            }

            // 验证参数安全性
            if (!IsValidXmlName(nodeName))
            {
                return;
            }

            if (SelectSingleNodeSafe($"/root/{EscapeXPathName(nodeName)}") == null)
            {
                _doc.DocumentElement.AppendChild(_doc.CreateElement(nodeName));
            }
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public bool DeleteNode(string nodeName)
        {
            // 验证参数安全性
            if (!IsValidXmlName(nodeName))
            {
                return false;
            }

            try
            {
                _lock.EnterWriteLock();

                var keyValue = SelectSingleNodeSafe($"/root") as XmlElement;
                var xmlElement = SelectSingleNodeSafe($"/root/{EscapeXPathName(nodeName)}") as XmlElement;

                if (keyValue != null && xmlElement != null)
                {
                    keyValue.RemoveChild(xmlElement);
                    if (_autoSave)
                    {
                        _doc.Save(_fullName);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return false;
            }
            finally
            {
                if (_lock.IsWriteLockHeld)
                    _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 安全地选择单个节点，防止XPath注入
        /// </summary>
        /// <param name="xpath">XPath表达式</param>
        /// <returns>匹配的节点</returns>
        private XmlNode SelectSingleNodeSafe(string xpath)
        {
            try
            {
                return _doc.DocumentElement?.SelectSingleNode(xpath);
            }
            catch (XPathException)
            {
                LogUtil.WriteException(new ArgumentException($"Invalid XPath expression: {xpath}"));
                // XPath语法错误，返回null
                return null;
            }
        }

        /// <summary>
        /// 验证XML名称是否有效
        /// </summary>
        /// <param name="name">XML名称</param>
        /// <returns>是否有效</returns>
        private bool IsValidXmlName(string name)
        {
            if (name.IsNullOrEmpty()) return false;

            try
            {
                XmlConvert.VerifyName(name);
                return true;
            }
            catch (XmlException)
            {
                LogUtil.WriteException(new ArgumentException($"Invalid XML name: {name}"));
                return false;
            }
        }

        /// <summary>
        /// 验证XML属性值是否有效（防止XPath注入）
        /// </summary>
        /// <param name="value">属性值</param>
        /// <returns>是否有效</returns>
        private bool IsValidXmlAttribute(string value)
        {
            if (value == null) return true;

            // 检查是否包含可能用于XPath注入的字符
            return !value.Contains("'") && !value.Contains("\"");
        }

        /// <summary>
        /// 转义XPath中的名称
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>转义后的名称</returns>
        private string EscapeXPathName(string name)
        {
            // 简单的验证，确保名称是有效的XML名称
            if (!IsValidXmlName(name))
            {
                throw new ArgumentException($"Invalid XML name: {name}");
            }
            return name;
        }

        /// <summary>
        /// 转义XPath属性值
        /// </summary>
        /// <param name="value">属性值</param>
        /// <returns>转义后的属性值</returns>
        private string EscapeXPathAttributeValue(string value)
        {
            if (value == null) return "''";

            // 如果值中包含单引号，则使用concat函数
            if (value.Contains("'"))
            {
                if (value.Contains("\""))
                {
                    // 如果同时包含单引号和双引号，则需要更复杂的处理
                    // 这里简单返回用单引号包围并转义的内容
                    return "'" + value.Replace("'", "&apos;") + "'";
                }
                return "\"" + value + "\"";
            }
            return "'" + value + "'";
        }

        /// <summary>
        /// 检查路径是否包含非法字符
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>是否包含非法字符</returns>
        private bool ContainsInvalidPathChars(string path)
        {
            char[] invalidChars = { '<', '>', '|', '*', '?' };
            foreach (char c in path)
            {
                if (Array.Exists(invalidChars, x => x == c))
                    return true;
            }
            return false;
        }
    }
}
