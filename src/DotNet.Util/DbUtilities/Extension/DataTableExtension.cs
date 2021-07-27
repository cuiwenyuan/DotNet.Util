//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace DotNet.Util
{
    public static class DataTableExtension
    {
        /// <summary>
        /// DataTable转泛型（dynamic方式）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dt) where T : new()
        {
            var lstT = new List<T>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstT.Add(dr.ToObject<T>());
                }
            }
            return lstT;
        }
        /// <summary>
        /// 转对象（dynamic方式）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ToObject<T>(this DataRow dr) where T : new()
        {
            dynamic dynTemp = new T();
            return dynTemp.GetFrom(dr);
        }

        /// <summary>
        /// DataTable转泛型(纯反射，无需定义Entity的GetFrom)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static List<T> ToList2<T>(this DataTable dt) where T : class, new()
        {
            var t = typeof(T);
            var properties = t.GetProperties();
            var list = new List<T>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var entity = new T();
                    foreach (var pi in properties)
                    {
                        var typeName = pi.Name;
                        if (dt.Columns.Contains(typeName))
                        {
                            if (!pi.CanWrite) continue;
                            var value = dr[typeName];
                            if (value == DBNull.Value) continue;
                            if (pi.PropertyType == typeof(string))
                            {
                                pi.SetValue(entity, value.ToString(), null);
                            }
                            else if (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(int?))
                            {
                                pi.SetValue(entity, int.Parse(value.ToString()), null);
                            }
                            else if (pi.PropertyType == typeof(long) || pi.PropertyType == typeof(long?))
                            {
                                pi.SetValue(entity, long.Parse(value.ToString()), null);
                            }
                            else if (pi.PropertyType == typeof(DateTime) || pi.PropertyType == typeof(DateTime?))
                            {
                                pi.SetValue(entity, DateTime.Parse(value.ToString()), null);
                            }
                            else if (pi.PropertyType == typeof(float))
                            {
                                pi.SetValue(entity, float.Parse(value.ToString()), null);
                            }
                            else if (pi.PropertyType == typeof(double))
                            {
                                pi.SetValue(entity, double.Parse(value.ToString()), null);
                            }
                            else if (pi.PropertyType == typeof(decimal))
                            {
                                pi.SetValue(entity, decimal.Parse(value.ToString()), null);
                            }
                            else
                            {
                                pi.SetValue(entity, value, null);
                            }
                        }
                    }
                    list.Add(entity);
                }
            }
            return list;
        }
    }
}
