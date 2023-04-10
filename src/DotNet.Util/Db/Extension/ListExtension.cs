//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace DotNet.Util
{
    /// <summary>
    /// List扩展
    /// </summary>
    public static partial class ListExtension
    {
        /// <summary>
        /// ToDataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> value) where T : class, new()
        {
            var lstProperty = new List<PropertyInfo>();
            var type = typeof(T);
            var dt = new DataTable();
            //type.GetProperties().ForEach(p =>  //ForEach扩展方法，这里使用Array.ForEach(type.GetProperties(),p=>{})也是一样
            Array.ForEach(type.GetProperties(), p =>
            {
                lstProperty.Add(p);
                if (p.PropertyType.IsGenericType)//是否为泛型，泛型获取不到具体的类型
                {
                    dt.Columns.Add(p.Name);
                }
                else
                {
                    dt.Columns.Add(p.Name, p.PropertyType);
                }
            });
            if (value != null)
            {
                foreach (var item in value)
                {
                    //创建一个DataRow实例
                    var row = dt.NewRow();
                    lstProperty.ForEach(p =>
                    {
                        row[p.Name] = p.GetValue(item, null);
                    });
                    dt.Rows.Add(row);
                }
            }
            return dt;
        }


        #region 扩展List，支持遍历中修改元素 - 来自NewLife

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
        #endregion
    }
}
