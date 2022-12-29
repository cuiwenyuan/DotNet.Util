using System;
using System.Collections;
using System.Collections.Generic;

namespace DotNet.Util
{
    /// <summary>
    /// 集合
    /// </summary>
    public static partial class CollectionUtil
    {
        /// <summary>
        /// IsNullOrEmpty
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this IEnumerable enumerable)
        {
            if (enumerable != null)
                return !enumerable.GetEnumerator().MoveNext();
            return true;
        }

        /// <summary>
        /// IsNotNullAndNotEmpty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsNotNullAndNotEmpty<T>(this List<T> list)
        {
            return list != null && list.Count > 0;
        }

        /// <summary>
        /// 对字符串集合进行拼接
        /// </summary>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string StringJoin(this IEnumerable<string> source, string separator = ",")
        {
            return string.Join(separator, source);
        }
    }
}