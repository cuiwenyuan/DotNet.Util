//-----------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace DotNet.Util
{
    public static partial class ReflectionUtil
    {
        public static BindingFlags Bf = BindingFlags.DeclaredOnly | BindingFlags.Public |
                                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        public static object InvokeMethod(object obj, string methodName, object[] args)
        {
            object objReturn = null;
            var type = obj.GetType();
            objReturn = type.InvokeMember(methodName, Bf | BindingFlags.InvokeMethod, null, obj, args);
            return objReturn;
        }

        public static void SetField(object obj, string name, object value)
        {
            var fi = obj.GetType().GetField(name, Bf);
            fi.SetValue(obj, value);
        }

        public static object GetField(object obj, string name)
        {
            var fi = obj.GetType().GetField(name, Bf);
            return fi.GetValue(obj);
        }

        /// <summary>
        /// 设置对象属性的值
        /// </summary>
        public static void SetProperty(object obj, string name, object value)
        {
            var propertyInfo = obj.GetType().GetProperty(name, Bf);
            var objValue = ChangeType2(value, propertyInfo.PropertyType);
            propertyInfo.SetValue(obj, objValue, null);
        }

        public static object ChangeType2(object value, Type conversionType)
        {
            if (value is DBNull || value == null||string.IsNullOrWhiteSpace(value.ToString()))
                return null;
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var nullableConverter
                    = new System.ComponentModel.NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }

            return Convert.ChangeType(value, conversionType);
        }

        /// <summary>
        /// 获取对象属性的值
        /// </summary>
        public static object GetProperty(object obj, string name)
        {
            var propertyInfo = obj.GetType().GetProperty(name, Bf);
            return propertyInfo.GetValue(obj, null);
        }

        /// <summary>
        /// 获取对象属性信息（组装成字符串输出）
        /// </summary>
        public static List<string> GetPropertyNames(object obj)
        {
            var nameList = new List<string>();
            var propertyInfos = obj.GetType().GetProperties(Bf);

            foreach (var property in propertyInfos)
            {
                nameList.Add(property.Name);
            }

            return nameList;
        }

        /// <summary>
        /// 获取对象属性信息（组装成字符串输出）
        /// </summary>
        public static Dictionary<string,string> GetPropertyNameTypes(object obj)
        {
            var nameList = new Dictionary<string, string>();
            var propertyInfos = obj.GetType().GetProperties(Bf);

            foreach (var property in propertyInfos)
            {
                nameList.Add(property.Name, property.PropertyType.FullName);
            }

            return nameList;
        }

        public static DataTable CreateTable(object objSource)
        {
            DataTable table = null;
            var objList = objSource as IEnumerable;
            if (objList != null)
                foreach (var obj in objList)
                {
                    if (table == null)
                    {
                        var nameList = GetPropertyNames(obj);
                        table = new DataTable("");
                        DataColumn column;

                        foreach (var name in nameList)
                        {
                            column = new DataColumn();
                            column.DataType = Type.GetType("System.String");
                            column.ColumnName = name;
                            column.Caption = name;
                            table.Columns.Add(column);
                        }
                    }

                    var row = table.NewRow();
                    var propertyInfos = obj.GetType().GetProperties(Bf);
                    foreach (var property in propertyInfos)
                    {
                        row[property.Name] = property.GetValue(obj, null);                    
                    }
                    table.Rows.Add(row);
                }

            return table;
        }
    }
}