//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;

namespace DotNet.Util
{
    public static class DataReaderExtension
    {
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

        public static T ToObject<T>(this IDataReader dr) where T : new()
        {
            dynamic dynTemp = new T();
            return dynTemp.GetFrom(dr);
        }
    }
}
