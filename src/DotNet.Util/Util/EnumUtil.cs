using System;
using System.Reflection;
using System.Collections;
using System.Data;

namespace DotNet.Util
{
    /// <summary>
    /// 枚举帮助类
    /// </summary>
    public static partial class EnumUtil
    {
        #region public static string ToDescription(this Enum enumeration)
        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="enumeration">枚举</param>
        /// <returns></returns>
        public static string ToDescription(this Enum enumeration)
        {
            var type = enumeration.GetType();
            var memInfo = type.GetMember(enumeration.ToString());
            if (null != memInfo && memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(EnumDescription), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((EnumDescription)attrs[0]).Text;
                }
            }
            return enumeration.ToString();
        }
        #endregion

        #region public static DataTable EnumToDataTable(Type enumType, string nameColumnName = "key", string valueColumnName = "value", string descriptionColumnName = "description")
        /// <summary>
        /// 枚举类型转化为DataTable
        /// var dt = EnumToDataTable(typeof(ProductType), "key", "value");
        /// </summary>
        public static DataTable EnumToDataTable(Type enumType, string nameColumnName = "key", string valueColumnName = "value", string descriptionColumnName = "description")
        {
            var descriptions = GetEnumDescriptions(enumType);

            var dt = new DataTable();
            dt.Columns.Add(valueColumnName, Type.GetType("System.Int32"));
            dt.Columns.Add(nameColumnName, Type.GetType("System.String"));
            dt.Columns.Add(descriptionColumnName, Type.GetType("System.String"));
            dt.Columns[nameColumnName].Unique = true;

            //修复：Enum.GetNames/GetValues 按值排序，而 GetEnumDescriptions 按声明顺序返回，
            //对于未按值递增声明的枚举会错位；这里改为按声明顺序遍历字段，与描述一一对应。
            //同时用 Convert.ToInt32 兼容底层类型为 uint/long/ulong 的枚举。
            var fields = enumType.GetFields();
            var descriptionIndex = 0;
            foreach (var field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    var dr = dt.NewRow();
                    dr[valueColumnName] = Convert.ToInt32(enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null));
                    dr[nameColumnName] = field.Name;
                    if (descriptionIndex < descriptions.Count)
                    {
                        dr[descriptionColumnName] = descriptions[descriptionIndex];
                    }
                    dt.Rows.Add(dr);
                    descriptionIndex++;
                }
            }
            return dt;
        }

        #endregion

        #region public static ArrayList GetEnumDescriptions(Type enumType)
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
                    //修复：底层类型不是 int 时 (int) 拆箱会抛 InvalidCastException，改用 Convert.ToInt32
                    value = Convert.ToInt32(enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null)).ToString();
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
        #endregion
    }
}
