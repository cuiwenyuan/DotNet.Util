﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DotNet.Util
{
    /// <summary>
    /// List工具
    /// </summary>
    public static partial class ListUtil
    {
        /// <summary>
        /// ListToDataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(List<T> list) where T : class
        {
            var dt = new DataTable();
            foreach (var info in typeof(T).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            foreach (var item in list)
            {
                var row = dt.NewRow();
                foreach (var info in typeof(T).GetProperties())
                {
                    //TODO: 如果T的属性有List<>类型的，这里不会得到预期的结果，需要进一步处理
                    row[info.Name] = info.GetValue(item, null);
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
