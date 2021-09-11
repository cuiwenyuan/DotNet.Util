#if NETSTANDARD2_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNet.Util
{
    /// <summary>
    /// 数组扩展
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// 随机获取数组中的一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static T RandomGet<T>(this T[] arr)
        {
            if (arr == null || !arr.Any())
                return default(T);

            var r = new Random();

            return arr[r.Next(arr.Length)];
        }
    }
}
#endif