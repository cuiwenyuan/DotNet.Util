using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Util
{
    /// <summary>
    /// 单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonUtil<T> where T : class, new()
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly static object obj = new Object();
        /// <summary>
        /// 
        /// </summary>
        private static T Instance = null;

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        public static T CreateInstance()
        {
            if (Instance == null)
            {
                lock (obj)
                {
                    if (Instance == null)
                    {
                        Instance = new T();
                    }
                }

            }
            return Instance;
        }

    }
}
