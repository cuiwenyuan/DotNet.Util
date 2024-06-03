//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Model
{
    /// <summary>
    /// FieldDescription
    /// 字段说明。
    /// 
    /// 修改记录
    /// 
    ///		2014.12.01 版本：1.0 JiRiGaLa 创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2014.12.01</date>
    /// </author> 
    /// </summary>    
    public static partial class FieldExtensions
    {
        /// <summary>
        /// 获取描述
        /// </summary>
        /// <param name="enumeration"></param>
        /// <returns></returns>
        public static string ToDescription(this string enumeration)
        {
            var type = enumeration.GetType();
            var memInfo = type.GetMember(enumeration);
            if (memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(FieldDescription), false);
                if (attrs.Length > 0)
                {
                    return ((FieldDescription)attrs[0]).Text;
                }
            }
            return enumeration;
        }

        #region 获取指定类别的自定义属性
        /// <summary>
        /// 获取指定类别的自定义属性
        /// </summary>
        /// <param name="entityType">实体类类别</param>
        /// <param name="fieldName">实体类属性名称</param>
        /// <param name="attributeType">自定义类别</param>
        /// <returns></returns>
        public static object GetCustomAttribute(this Type entityType, string fieldName, Type attributeType)
        {
            var fieldInfo = entityType.GetField(fieldName);

            if (fieldInfo != null)
            {
                var attribute = fieldInfo.GetCustomAttributes(attributeType, false);
                if (attribute.Length > 0)
                {
                    return attribute[0];
                }
            }
            else
            {
                var propertyInfo = entityType.GetProperty(fieldName);
                if (propertyInfo != null)
                {
                    var attribute = propertyInfo.GetCustomAttributes(attributeType, false);
                    if (attribute.Length > 0)
                    {
                        return attribute[0];
                    }
                }
            }
            return null;
        }
        #endregion

        #region 获取字段备注
        /// <summary>
        /// 获取字段备注
        /// </summary>
        /// <param name="entityType">实体类类别</param>
        /// <param name="fieldName">实体类属性名称</param>
        /// <returns></returns>
        [Obsolete("Please use FieldDescription from 2022-07-21", true)]
        public static string ToDescription(this Type entityType, string fieldName)
        {
            var obj = (FieldDescription)GetCustomAttribute(entityType, fieldName, typeof(FieldDescription));
            return obj != null ? obj.Text : string.Empty;
        }
        #endregion

        #region 获取字段备注
        /// <summary>
        /// 获取字段备注
        /// </summary>
        /// <param name="entityType">实体类类别</param>
        /// <param name="fieldName">实体类属性名称</param>
        /// <returns></returns>
        public static string FieldDescription(this Type entityType, string fieldName)
        {
            var obj = (FieldDescription)GetCustomAttribute(entityType, fieldName, typeof(FieldDescription));
            return obj != null ? obj.Text : string.Empty;
        }
        #endregion
    }
}