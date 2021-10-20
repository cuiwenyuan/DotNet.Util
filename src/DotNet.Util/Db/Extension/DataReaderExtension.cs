//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace DotNet.Util
{
    /// <summary>
    /// DataReader扩展
    /// </summary>
    public static class DataReaderExtension
    {
        #region 底层使用DotNet.Business的用法

        /// <summary>
        /// 转List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this IDataReader dataReader) where T : new()
        {
            var entities = new List<T>();
            if ((dataReader == null))
            {
                return entities;
            }
            using (dataReader)
            {
                while (dataReader.Read())
                {
                    entities.Add(dataReader.ToEntity<T>());
                }
            }
            return entities;
        }

        /// <summary>
        /// 转对象(必须实现了GetFrom(dr))
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ToEntity<T>(this IDataReader dr) where T : new()
        {
            dynamic dynTemp = new T();
            return dynTemp.GetFrom(dr);
        }

        #endregion

        #region 反射方式，可任意使用

        /// <summary>
        /// IDataReader转泛型IList(纯反射，无需定义Entity的GetFrom)
        /// </summary>
        /// <typeparam name="T">泛型实体T</typeparam>
        /// <param name="dr">DataReader对象</param>
        /// <returns></returns>
        public static List<T> ToAnyList<T>(this IDataReader dr)
        {
            var ls = new List<T>();
            //获取传入的数据类型
            var t = typeof(T);
            //遍历DataReader对象
            while (dr.Read())
            {
                //使用与指定参数匹配最高的构造函数，来创建指定类型的实例
                var entity = Activator.CreateInstance<T>();
                for (var i = 0; i < dr.FieldCount; i++)
                {
                    //判断字段值是否为空或不存在的值
                    if (!IsNullOrDbNull(dr[i]))
                    {
                        //匹配字段名
                        var pi = t.GetProperty(dr.GetName(i), BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (pi != null)
                        {
                            //绑定实体对象中同名的字段  
                            pi.SetValue(entity, CheckType(dr[i], pi.PropertyType), null);
                        }
                    }
                }
                ls.Add(entity);
            }
            return ls;
        }

        /// <summary>
        /// 对可空类型进行判断转换(*要不然会报错)
        /// </summary>
        /// <param name="value">DataReader字段的值</param>
        /// <param name="conversionType">该字段的类型</param>
        /// <returns></returns>
        private static object CheckType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (value == null)
                    return null;
                var nullableConverter = new System.ComponentModel.NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            return Convert.ChangeType(value, conversionType);
        }

        /// <summary>
        /// 判断指定对象是否是有效值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool IsNullOrDbNull(object obj)
        {
            return (obj == null || (obj is DBNull)) ? true : false;
        }


        /// <summary>
        /// IDataReader(while (dr.Read()){}中使用)循环读取转实体(纯反射，无需定义Entity的GetFrom)
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="dr">IDataReader对象</param>
        /// <returns></returns>
        public static T ToAnyEntity<T>(this IDataReader dr)
        {
            if (dr.Read())
            {
                var t = typeof(T);
                var count = dr.FieldCount;
                var entity = Activator.CreateInstance<T>();
                for (var i = 0; i < count; i++)
                {
                    if (!IsNullOrDbNull(dr[i]))
                    {
                        var pi = t.GetProperty(dr.GetName(i), BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (pi != null)
                        {
                            pi.SetValue(entity, CheckType(dr[i], pi.PropertyType), null);
                        }
                    }
                }
                return entity;
            }
            else
            {
                return default(T);
            }
        }

        #endregion

    }
}
