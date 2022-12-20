//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
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
    ///		<name>Troy.Cui</name>
    ///		<date>2007.05.16</date>
    /// </author> 
    /// </summary>
    public class ResourceManager
    {
        private volatile static ResourceManager _instance = null;
        private static object _locker = new Object();
        /// <summary>
        /// 实例
        /// </summary>
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
        /// <summary>
        /// 语言资源
        /// </summary>
        public SortedList<String, Resources> LanguageResources = new SortedList<String, Resources>();
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="resources"></param>
        /// <param name="filePath"></param>
        public void Serialize(Resources resources, string filePath)
        {
            ResourcesSerializer.Serialize(filePath, resources);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="filePath"></param>
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
        /// <summary>
        /// 获取语言
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 获取语言
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 获取资源
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="language"></param>
        /// <param name="key"></param>
        /// <returns></returns>
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