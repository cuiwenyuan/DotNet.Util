﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Util
{
    /// <summary>
    /// 扩展List，支持遍历中修改元素 - 来自NewLife
    /// </summary>
    public static partial class ListExtension
    {
        /// <summary>线程安全，搜索并返回第一个，支持遍历中修改元素</summary>
        /// <param name="list">实体列表</param>
        /// <param name="match">条件</param>
        /// <returns></returns>
        public static T Find<T>(this IList<T> list, Predicate<T> match)
        {
            if (list is List<T> list2) return list2.Find(match);

            return list.ToArray().FirstOrDefault(e => match(e));
        }

        /// <summary>线程安全，搜索并返回第一个，支持遍历中修改元素</summary>
        /// <param name="list">实体列表</param>
        /// <param name="match">条件</param>
        /// <returns></returns>
        public static IList<T> FindAll<T>(this IList<T> list, Predicate<T> match)
        {
            if (list is List<T> list2) return list2.FindAll(match);

            return list.ToArray().Where(e => match(e)).ToList();
        }
    }
}