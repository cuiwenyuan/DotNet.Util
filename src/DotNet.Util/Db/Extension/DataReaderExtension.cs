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
            if (dataReader != null && !dataReader.IsClosed)
            {
                while (dataReader.Read())
                {
                    entities.Add(dataReader.ToEntity<T>());
                }

                dataReader.Close();
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
        /// <param name="dataReader">DataReader对象</param>
        /// <returns></returns>
        public static List<T> ToAnyList<T>(this IDataReader dataReader) where T : class, new()
        {
            var ls = new List<T>();
            //获取传入的数据类型
            var t = typeof(T);
            if (dataReader != null && !dataReader.IsClosed)
            {
                //遍历DataReader对象
                while (dataReader.Read())
                {
                    //使用与指定参数匹配最高的构造函数，来创建指定类型的实例
                    var entity = Activator.CreateInstance<T>();
                    for (var i = 0; i < dataReader.FieldCount; i++)
                    {
                        //判断字段值是否为空或不存在的值
                        if (!BaseUtil.IsNullOrDbNull(dataReader[i]))
                        {
                            //匹配字段名
                            var pi = t.GetProperty(dataReader.GetName(i), BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                            if (pi != null)
                            {
                                //绑定实体对象中同名的字段  
                                pi.SetValue(entity, BaseUtil.ChangeType(dataReader[i], pi.PropertyType), null);
                            }
                        }
                    }
                    ls.Add(entity);
                }
                dataReader.Close();
            }
            return ls;
        }

        /// <summary>
        /// IDataReader(while (dataReader.Read()){}中使用)循环读取转实体(纯反射，无需定义Entity的GetFrom)
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="dataReader">IDataReader对象</param>
        /// <returns></returns>
        public static T ToAnyEntity<T>(this IDataReader dataReader)
        {
            if (dataReader != null && !dataReader.IsClosed)
            {
                var entity = Activator.CreateInstance<T>();
                while (dataReader.Read())
                {
                    var t = typeof(T);
                    var count = dataReader.FieldCount;
                    for (var i = 0; i < count; i++)
                    {
                        if (!BaseUtil.IsNullOrDbNull(dataReader[i]))
                        {
                            var pi = t.GetProperty(dataReader.GetName(i), BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                            if (pi != null)
                            {
                                pi.SetValue(entity, BaseUtil.ChangeType(dataReader[i], pi.PropertyType), null);
                            }
                        }
                    }
                }
                dataReader.Close();
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
