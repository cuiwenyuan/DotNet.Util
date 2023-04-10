#if NETSTANDARD2_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace DotNet.Util
{
    /// <summary>
    /// ConfigurationExtensions
    /// </summary>
    public static partial class ConfigurationExtensions
    {
        /// <summary>
        /// 获取指定类型的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cfg"></param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static T Get<T>(this IConfiguration cfg, string key) where T : class, new()
        {
            if (cfg == null || key.IsNull())
                return new T();

            var t = cfg.GetSection(key).Get<T>();
            if (t == null)
                return new T();

            return t;
        }
    }
}
#endif