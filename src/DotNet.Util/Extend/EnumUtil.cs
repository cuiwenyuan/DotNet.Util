﻿using System;
using System.Reflection;
using System.Collections;
using System.Data;

namespace DotNet.Util
{
    /// <summary>
    /// 枚举帮助类
    /// </summary>
    public partial class EnumUtil
    {
        /// <summary>
        /// 枚举类型转化为DataTable
        /// var dt = EnumToDataTable(typeof(ProductType), "key", "value");
        /// </summary>
        public static DataTable EnumToDataTable(Type enumType, string nameColumnName = "key", string valueColumnName = "value", string descriptionColumnName = "description")
        {
            var names = Enum.GetNames(enumType);
            var values = Enum.GetValues(enumType);
            var descriptions = GetEnumDescriptions(enumType);

            var dt = new DataTable();
            dt.Columns.Add(valueColumnName, Type.GetType("System.Int32"));
            dt.Columns.Add(nameColumnName, Type.GetType("System.String"));
            dt.Columns.Add(descriptionColumnName, Type.GetType("System.String"));
            dt.Columns[nameColumnName].Unique = true;
            for (var i = 0; i < values.Length; i++)
            {
                var dr = dt.NewRow();
                dr[valueColumnName] = (int)values.GetValue(i);
                dr[nameColumnName] = names[i];
                dr[descriptionColumnName] = descriptions[i];
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// 从枚举类型和它的特性读出并返回一个数组
        /// </summary>
        /// <param name="enumType">Type,该参数的格式为typeof(需要读的枚举类型)</param>
        /// <returns>键值对</returns>
        public static ArrayList GetEnumDescriptions(Type enumType)
        {
            var result = new ArrayList();
            var enumDescription = typeof(EnumDescription);
            var fields = enumType.GetFields();
            var description = string.Empty;
            var value = string.Empty;
            foreach (var field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    value = ((int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null)).ToString();
                    var array = field.GetCustomAttributes(enumDescription, true);
                    if (array.Length > 0)
                    {
                        var temp = (EnumDescription)array[0];
                        description = temp.Text;
                    }
                    else
                    {
                        description = field.Name;
                    }
                    result.Add(description);
                }
            }
            return result;
        }


    }
}
