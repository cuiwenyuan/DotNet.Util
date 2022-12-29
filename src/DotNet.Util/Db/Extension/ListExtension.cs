//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace DotNet.Util
{
    /// <summary>
    /// List扩展
    /// </summary>
    public static class ListExtension
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
    }
}
