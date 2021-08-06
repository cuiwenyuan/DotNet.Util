//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;

namespace DotNet.Util
{
    /// <summary>
    /// DataReader扩展
    /// </summary>
    public static class DataReaderExtension
    {
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
                    entities.Add(dataReader.ToObject<T>());
                }
            }
            return entities;
        }

        /// <summary>
        /// 转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ToObject<T>(this IDataReader dr) where T : new()
        {
            dynamic dynTemp = new T();
            return dynTemp.GetFrom(dr);
        }
    }
}
