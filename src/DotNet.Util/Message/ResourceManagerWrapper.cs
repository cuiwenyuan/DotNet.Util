//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections;

namespace DotNet.Util
{
    /// <summary>
    /// ResourceManagerWrapper
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
    public class ResourceManagerWrapper
    {
        private volatile static ResourceManagerWrapper _instance = null;
        private static object _locker = new Object();

        public static ResourceManagerWrapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new ResourceManagerWrapper();
                        }
                    }
                }
                return _instance;
            }
        }

        private ResourceManager _resourceManager;

        public ResourceManagerWrapper()
        {
        }

        public void LoadResources(string path)
        {
            _resourceManager = ResourceManager.Instance;
            _resourceManager.Init(path);
        }

        public string Get(string key)
        {
            if (_resourceManager == null)
            {
                return string.Empty;
            }
            return _resourceManager.Get(BaseSystemInfo.CurrentLanguage, key);
        }

        public string Get(string language, string key)
        {
            return _resourceManager.Get(language, key);
        }

        public string Get(BaseUserInfo userInfo, string key)
        {
            if ((userInfo == null) || (string.IsNullOrEmpty(userInfo.CurrentLanguage)))
            {
                return _resourceManager.Get(BaseSystemInfo.CurrentLanguage, key);
            }
            else
            {
                return _resourceManager.Get(userInfo.CurrentLanguage, key);
            }
        }

        public Hashtable GetLanguages()
        {
            if (_resourceManager == null)
            {
                return null;
            }
            return _resourceManager.GetLanguages();
        }

        public Hashtable GetLanguages(string path)
        {
            return _resourceManager.GetLanguages(path);
        }

        public void Serialize(string path, string language, string key, string value)
        {
            var resources = GetResources(path, language);
            resources.Set(key, value);
            var filePath = path + "\\" + language + ".xml";
            _resourceManager.Serialize(resources, filePath);
        }

        public Resources GetResources(string path, string language)
        {
            var filePath = path + "\\" + language + ".xml";
            return _resourceManager.GetResources(filePath);
        }

        public Resources GetResources(string language)
        {
            return _resourceManager.LanguageResources[language];
        }
    }
}