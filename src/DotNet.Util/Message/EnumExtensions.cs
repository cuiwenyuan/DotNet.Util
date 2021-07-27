//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
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
    ///		2011.10.13 版本：1.0 Troy Cui 创建。
    ///		
    /// <author>
    ///		<name>Troy Cui</name>
    ///		<date>2011.10.13</date>
    /// </author> 
    /// </summary>    
    public static partial class EnumExtensions
    {
        public static string ToDescription(this Enum enumeration) 
        {
            var type = enumeration.GetType();
            var memInfo = type.GetMember(enumeration.ToString());
            if (null != memInfo && memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(EnumDescription), false);
                if (null != attrs && attrs.Length > 0)
                {
                    return ((EnumDescription)attrs[0]).Text;
                }
            }
            return enumeration.ToString(); 
        }
    }
}