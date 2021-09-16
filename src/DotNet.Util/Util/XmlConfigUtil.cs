using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

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
        /// 构造函数
        /// </summary>
        /// <param name="xmlPath">XML路径，默认为XmlConfig\Config.config</param>
        public XmlConfigUtil(string xmlPath = "XmlConfig\\Config.config")
        {
            _fullName = string.Format(@"{0}\" + xmlPath, AppDomain.CurrentDomain.BaseDirectory); ;

            if (!File.Exists(_fullName))
            {
                var directoryName = Path.GetDirectoryName(_fullName);
                if (!string.IsNullOrWhiteSpace(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                _doc.AppendChild(_doc.CreateXmlDeclaration("1.0", "UTF-8", null));
                _doc.AppendChild(_doc.CreateElement("root"));
                CreateNode(DefaultNodeName);
                _doc.Save(_fullName);
            }

            _doc.Load(_fullName);
        }

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            try
            {
                _doc.Save(_fullName);
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return false;
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
            if (string.IsNullOrEmpty(nodeName))
            {
                nodeName = DefaultNodeName;
            }
            try
            {
                var node = _doc.DocumentElement.SelectSingleNode($"/root/{nodeName}");
                if (node == null)
                {
                    CreateNode(nodeName);
                    node = _doc.DocumentElement.SelectSingleNode($"/root/{nodeName}");
                }
                var item = (XmlElement)_doc.DocumentElement.SelectSingleNode($"/root/{nodeName}/{itemName}[@key='{key}']");
                if (item == null)
                {
                    //自动创建item                    
                    var itemNew = _doc.CreateElement(itemName);
                    itemNew.SetAttribute("key", key);
                    itemNew.SetAttribute("value", defaultValue);
                    node.AppendChild(itemNew);
                    if (_autoSave)
                    {
                        Save();
                    }
                    return defaultValue;
                }
                else
                {
                    return item.GetAttribute("value");
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return string.Empty;
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
            if (string.IsNullOrEmpty(nodeName))
            {
                nodeName = DefaultNodeName;
            }
            try
            {
                var node = _doc.DocumentElement.SelectSingleNode($"/root/{nodeName}");
                if (node == null)
                {
                    CreateNode(nodeName);
                    node = _doc.DocumentElement.SelectSingleNode($"/root/{nodeName}");
                }
                var item = (XmlElement)_doc.DocumentElement.SelectSingleNode($"/root/{nodeName}/{itemName}[@key='{key}']");
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
                    Save();
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return false;
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
            if (string.IsNullOrEmpty(nodeName))
            {
                nodeName = DefaultNodeName;
            }
            try
            {
                var keys = new List<string>();
                var xmlElement = (XmlElement)_doc.DocumentElement.SelectSingleNode($"/root/{nodeName}");
                if (xmlElement != null)
                {
                    foreach (XmlElement node in xmlElement)
                    {
                        keys.Add(node.GetAttribute("key"));
                    }
                }

                return keys;
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return new List<string>();
            }
        }

        /// <summary>
        /// 获取所有的键
        /// </summary>
        /// <param name="nodeName">节点名</param>
        /// <returns></returns>
        public List<string> GetAllValue(string nodeName = DefaultNodeName)
        {
            if (string.IsNullOrEmpty(nodeName))
            {
                nodeName = DefaultNodeName;
            }
            try
            {
                var keys = new List<string>();
                var xmlElement = (XmlElement)_doc.DocumentElement.SelectSingleNode($"/root/{nodeName}");
                if (xmlElement != null)
                {
                    foreach (XmlElement node in xmlElement)
                    {
                        keys.Add(node.GetAttribute("value"));
                    }
                }

                return keys;
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return new List<string>();
            }
        }

        /// <summary>
        /// 获取所有的键/值
        /// </summary>
        /// <param name="nodeName">节点名</param>
        /// <returns></returns>
        public Dictionary<string, string> GetAllKeyValue(string nodeName = DefaultNodeName)
        {
            if (string.IsNullOrEmpty(nodeName))
            {
                nodeName = DefaultNodeName;
            }
            try
            {
                var keyValues = new Dictionary<string, string>();
                var xmlElement = (XmlElement)_doc.DocumentElement.SelectSingleNode($"/root/{nodeName}");
                if (xmlElement != null)
                {
                    foreach (XmlElement node in xmlElement)
                    {
                        keyValues.Add(node.GetAttribute("key"), node.GetAttribute("value"));
                    }
                }


                return keyValues;
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return new Dictionary<string, string>();
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
            if (string.IsNullOrEmpty(nodeName))
            {
                nodeName = DefaultNodeName;
            }
            try
            {
                CreateNode(nodeName);
                var keyValue = _doc.DocumentElement.SelectSingleNode($"/root/{nodeName}");
                var xmlElement = _doc.DocumentElement.SelectSingleNode($"/root/{nodeName}/{itemName}[@key='{key}']");
                if (keyValue != null && xmlElement != null)
                {
                    keyValue.RemoveChild(xmlElement);
                }

                if (_autoSave)
                {
                    Save();
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return false;
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
                var nodes = new List<string>();
                var xmlElement = (XmlElement)_doc.DocumentElement.SelectSingleNode($"/root");
                if (xmlElement != null)
                {
                    foreach (XmlElement node in xmlElement)
                    {
                        nodes.Add(node.Name);
                    }
                }

                return nodes;
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return new List<string>();
            }
        }


        /// <summary>
        /// 检查创建节点
        /// </summary>
        /// <param name="nodeName"></param>
        private void CreateNode(string nodeName = DefaultNodeName)
        {
            if (string.IsNullOrEmpty(nodeName))
            {
                nodeName = DefaultNodeName;
            }
            if (_doc.DocumentElement.SelectSingleNode($"/root/{nodeName}") == null)
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
            try
            {
                var keyValue = _doc.DocumentElement.SelectSingleNode($"/root");
                var xmlElement = _doc.DocumentElement.SelectSingleNode($"/root/{nodeName}");

                if (keyValue != null && xmlElement != null)
                {
                    keyValue.RemoveChild(xmlElement);
                    if (_autoSave)
                    {
                        Save();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return false;
            }
        }
    }
}
