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
        protected readonly static object obj = new Object();
        private static T Instance = new T();

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
