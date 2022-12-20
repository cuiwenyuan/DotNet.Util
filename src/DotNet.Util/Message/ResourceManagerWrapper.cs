//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
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
    ///		<name>Troy.Cui</name>
    ///		<date>2007.05.16</date>
    /// </author> 
    /// </summary>
    public class ResourceManagerWrapper
    {
        /// <summary>
        /// 实例
        /// </summary>
        private volatile static ResourceManagerWrapper _instance = null;
        /// <summary>
        /// 锁
        /// </summary>
        private static object _locker = new Object();
        /// <summary>
        /// 实例
        /// </summary>
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
        /// <summary>
        /// 资源管理
        /// </summary>
        public ResourceManagerWrapper()
        {
        }
        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="path"></param>
        public void LoadResources(string path)
        {
            _resourceManager = ResourceManager.Instance;
            _resourceManager.Init(path);
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key)
        {
            if (_resourceManager == null)
            {
                return string.Empty;
            }
            return _resourceManager.Get(BaseSystemInfo.CurrentLanguage, key);
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="language"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string language, string key)
        {
            return _resourceManager.Get(language, key);
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="key"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 获取语言
        /// </summary>
        /// <returns></returns>
        public Hashtable GetLanguages()
        {
            if (_resourceManager == null)
            {
                return null;
            }
            return _resourceManager.GetLanguages();
        }
        /// <summary>
        /// 获取语言
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Hashtable GetLanguages(string path)
        {
            return _resourceManager.GetLanguages(path);
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="language"></param>
        /// <param name="key"></param>
        /// <param name="value">值</param>
        public void Serialize(string path, string language, string key, string value)
        {
            var resources = GetResources(path, language);
            resources.Set(key, value);
            var filePath = path + "\\" + language + ".xml";
            _resourceManager.Serialize(resources, filePath);
        }
        /// <summary>
        /// 获取资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public Resources GetResources(string path, string language)
        {
            var filePath = path + "\\" + language + ".xml";
            return _resourceManager.GetResources(filePath);
        }
        /// <summary>
        /// 获取资源
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public Resources GetResources(string language)
        {
            return _resourceManager.LanguageResources[language];
        }
    }
}