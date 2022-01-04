﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace DotNet.Util
{
    /// <summary>
    /// DataTable扩展
    /// </summary>
    public static class DataTableExtension
    {
        #region 底层使用DotNet.Business的用法

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
                    lstT.Add(dr.ToEntity<T>());
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
        public static T ToEntity<T>(this DataRow dr) where T : new()
        {
            dynamic dynTemp = new T();
            return dynTemp.GetFrom(dr);
        }

        #endregion

        #region 反射方式，可任意使用

        /// <summary>
        /// DataTable转泛型(纯反射，无需定义Entity的GetFrom)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static List<T> ToAnyList<T>(this DataTable dt) where T : class, new()
        {
            var t = typeof(T);
            var properties = t.GetProperties();
            var ls = new List<T>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    T entity = new T();
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
                            else if (pi.PropertyType == typeof(int))
                            {
                                pi.SetValue(entity, BaseUtil.ConvertToInt(value), null);

                            }
                            else if (pi.PropertyType == typeof(int?))
                            {
                                pi.SetValue(entity, BaseUtil.ConvertToNullableInt(value), null);
                            }
                            else if (pi.PropertyType == typeof(long))
                            {
                                pi.SetValue(entity, BaseUtil.ConvertToLong(value), null);
                            }
                            else if (pi.PropertyType == typeof(long?))
                            {
                                pi.SetValue(entity, BaseUtil.ConvertToNullableLong(value), null);
                            }
                            else if (pi.PropertyType == typeof(DateTime))
                            {
                                pi.SetValue(entity, BaseUtil.ConvertToDateTime(value), null);
                            }
                            else if (pi.PropertyType == typeof(DateTime?))
                            {
                                pi.SetValue(entity, BaseUtil.ConvertToNullableDateTime(value), null);
                            }
                            else if (pi.PropertyType == typeof(float))
                            {
                                pi.SetValue(entity, BaseUtil.ConvertToFloat(value), null);
                            }
                            else if (pi.PropertyType == typeof(float?))
                            {
                                pi.SetValue(entity, BaseUtil.ConvertToNullableFloat(value), null);
                            }
                            else if (pi.PropertyType == typeof(double?))
                            {
                                pi.SetValue(entity, BaseUtil.ConvertToDouble(value), null);
                            }
                            else if (pi.PropertyType == typeof(double?))
                            {
                                pi.SetValue(entity, BaseUtil.ConvertToNullableDouble(value), null);
                            }
                            else if (pi.PropertyType == typeof(decimal))
                            {
                                pi.SetValue(entity, BaseUtil.ConvertToDecimal(value), null);
                            }
                            else if (pi.PropertyType == typeof(decimal?))
                            {
                                pi.SetValue(entity, BaseUtil.ConvertToNullableDecimal(value), null);
                            }
                            else
                            {
                                pi.SetValue(entity, BaseUtil.ChangeType(value, pi.PropertyType), null);
                            }

                        }
                    }
                    ls.Add(entity);
                }
            }
            return ls;
        }
        #endregion
    }
}
