﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace DotNet.Util
{
    /// <summary>
    /// 数据查询
    /// </summary>
    public static class DbQueryUtil
    {
        /// <summary>
        /// 获取列名
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static string GetColumnName(MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                return "*";
            }
            var columnAttribute = memberInfo.GetCustomAttribute<ColumnAttribute>();

            var columnName = columnAttribute?.Name ?? memberInfo.Name;
            return columnName;
        }
        
    }
}