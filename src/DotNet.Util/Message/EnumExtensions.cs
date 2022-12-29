//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace DotNet.Util
{
    /// <summary>
    /// EnumDescription
    /// 枚举状态的说明。
    /// 
    /// 修改记录
    /// 
    ///		2011.10.13 版本：1.0 JiRiGaLa 创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.10.13</date>
    /// </author> 
    /// </summary>    
    public static partial class EnumExtensions
    {
        #region ToDescription 获取枚举描述
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
    }
}