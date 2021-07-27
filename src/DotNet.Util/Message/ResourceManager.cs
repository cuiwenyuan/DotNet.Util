//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DotNet.Util
{
    /// <summary>
    /// ResourceManager
    /// 资源管理器
    /// 
    ///	修改纪录
    ///		2007.05.16 版本：1.0 JiRiGaLa	重新调整主键的规范化。
    /// 
    /// <author>
    ///		<name>Troy Cui</name>
    ///		<date>2007.05.16</date>
    /// </author> 
    /// </summary>
    public class ResourceManager
    {
        private volatile static ResourceManager _instance = null;
        private static object _locker = new Object();
        public static ResourceManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new ResourceManager();
                        }
                    }
                }
                return _instance;
            }
        }

        private string _folderPath = string.Empty;
        public SortedList<String, Resources> LanguageResources = new SortedList<String, Resources>();

        public void Serialize(Resources resources, string filePath)
        {
            ResourcesSerializer.Serialize(filePath, resources);
        }

        public void Init(string filePath)
        {
            _folderPath = filePath;
            var directoryInfo = new DirectoryInfo(filePath);
            LanguageResources.Clear();
            if (!directoryInfo.Exists)
            {
                return;
            }
            var fileInfo = directoryInfo.GetFiles();
            foreach (var t in fileInfo)
            {
                var resources = ResourcesSerializer.DeSerialize(t.FullName);
                resources.CreateIndex();
                LanguageResources.Add(resources.Language, resources);
            }
        }

        public Hashtable GetLanguages()
        {
            var hashtable = new Hashtable();
            var iEnumerator = LanguageResources.GetEnumerator();
            while (iEnumerator.MoveNext())
            {
                hashtable.Add(iEnumerator.Current.Key, iEnumerator.Current.Value.DisplayName);
            }
            return hashtable;
        }

        public Hashtable GetLanguages(string path)
        {
            var hashtable = new Hashtable();
            var directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists)
            {
                return hashtable;
            }
            var fileInfo = directoryInfo.GetFiles();
            foreach (var t in fileInfo)
            {
                var resources = ResourcesSerializer.DeSerialize(t.FullName);
                hashtable.Add(resources.Language, resources.DisplayName);
            }
            return hashtable;
        }

        public Resources GetResources(string filePath)
        {
            var resources = new Resources();
            if (File.Exists(filePath))
            {
                resources = ResourcesSerializer.DeSerialize(filePath);
                resources.CreateIndex();
            }
            return resources;
        }

        public string Get(string language, string key)
        {
            if (string.IsNullOrEmpty(language))
            {
                language = "zh-CHS";
            }
            if (!LanguageResources.ContainsKey(language))
            {
                return string.Empty;
            }
            return LanguageResources[language].Get(key);
        }
    }
}