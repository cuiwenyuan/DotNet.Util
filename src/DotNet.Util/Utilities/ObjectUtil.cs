//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNet.Util
{
    /// <summary>
    ///	ObjectUtil
    /// Object辅助类
    /// 
    /// 
    /// 修改记录
    /// 
    ///		2021.04.15 版本：1.0 Troy.Cui
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2021.04.15</date>
    /// </author> 
    /// </summary>

    public static partial class ObjectUtil
    {
        #region public static string ToList(Object[] ids, string separator = null) 字段值数组转换为字符串列表
        /// <summary>
        /// 字段值数组转换为字符串列表
        /// </summary>
        /// <param name="ids">字段值</param>
        /// <param name="separator">分隔符</param>
        /// <returns>字段值字符串</returns>
        public static string ToList(Object[] ids, string separator = null)
        {
            var result = string.Empty;
            var stringList = string.Empty;

            if (ids != null)
            {
                foreach (var t in ids)
                {
                    if (!string.IsNullOrEmpty(separator))
                    {
                        stringList += separator + t + separator + ",";
                    }
                    else
                    {
                        // stringList += ids[i] + "', '";
                        stringList += t + ",";
                    }
                }
            }

            if (!string.IsNullOrEmpty(stringList))
            {
                result = stringList.Substring(0, stringList.Length - 1);
            }
            if (string.IsNullOrEmpty(result))
            {
                result = "NULL";
            }

            return result;
        }
        #endregion
    }
}