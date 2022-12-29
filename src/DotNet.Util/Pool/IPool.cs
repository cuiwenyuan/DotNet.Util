using System;

namespace DotNet.Util
{
    /// <summary>对象池接口</summary>
    /// <remarks>
    /// 文档 https://www.yuque.com/smartstone/nx/object_pool
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public interface IPool<T> where T : class
    {
        /// <summary>对象池大小</summary>
        Int32 Max { get; set; }

        /// <summary>获取</summary>
        /// <returns></returns>
        T Get();

        /// <summary>归还</summary>
        /// <param name="value">值</param>
        Boolean Put(T value);

        /// <summary>清空</summary>
        Int32 Clear();
    }
}