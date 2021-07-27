//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace DotNet.Util
{
    /// <summary>
    /// BUResourceManager
    /// 资源管理器
    /// 
    ///	修改记录
    ///		2007.05.16 版本：1.0 JiRiGaLa	重新调整主键的规范化。
    /// 
    /// <author>
    ///		<name>Troy Cui</name>
    ///		<date>2007.05.16</date>
    /// </author> 
    /// </summary>
    [XmlRoot("resources")]
    public class Resources
    {
        private SortedList<String, String> _indexs = new SortedList<String, String>();

        [XmlElement("language")]
        public string Language = string.Empty;
        [XmlElement("displayName")]
        public string DisplayName = string.Empty;
        [XmlElement("version")]
        public string Version = string.Empty;
        [XmlElement("author")]
        public string Author = string.Empty;
        [XmlElement("description")]
        public string Description = string.Empty;
        [XmlElement("items", typeof(Items))]
        public Items Items;

        public void CreateIndex()
        {
            _indexs.Clear();
            if (Items == null)
            {
                return;
            }
            _indexs = new SortedList<String, String>(Items.items.Length);
            foreach (var t in Items.items)
            {
#if DEBUG
                try
                {
                    _indexs.Add(t.Key, t.Value);
                }
                catch
                {
                    throw (new Exception(t.Key + t.Value));
                }
#else
                    _indexs.Add(t.Key, t.Value);
#endif
            }
        }

        public string Get(string key)
        {
            if (!_indexs.ContainsKey(key))
            {
                return string.Empty;
            }
            return _indexs[key];
        }

        /// <summary>
        /// JiRiGaLa 2007.05.02
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set(string key, string value)
        {
            if (!_indexs.ContainsKey(key))
            {
                return false;
            }
            _indexs[key] = value;
            foreach (var t in Items.items)
            {
                if (t.Key == key)
                {
                    t.Value = value;
                    break;
                }
            }
            return true;
        }
    }

    public class Items
    {
        [XmlElement("item", typeof(Item))]
        public Item[] items;
    }


    public class Item
    {
        [XmlAttribute("key")]
        public string Key = string.Empty;
        [XmlText]
        public string Value = string.Empty;
    }


    internal class ResourcesSerializer
    {
        public static Resources DeSerialize(string filePath)
        {
            var xmlSerializer = new XmlSerializer(typeof(Resources));
            var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open);
            var resources = xmlSerializer.Deserialize(fileStream) as Resources;
            fileStream.Close();
            return resources;
        }

        public static void Serialize(string filePath, Resources resources)
        {
            var xmlSerializer = new XmlSerializer(typeof(Resources));
            var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
            xmlSerializer.Serialize(fileStream, resources);
            fileStream.Close();
        }
    }
}
