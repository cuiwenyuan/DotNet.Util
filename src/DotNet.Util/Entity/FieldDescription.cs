//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
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
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, Inherited = true)]
    public class FieldDescription : Attribute
    {
        /// <summary>
        /// 文本
        /// </summary>
        public string Text { get; }
        /// <summary>
        /// 需要日志
        /// </summary>
        public bool NeedLog { get; }
        /// <summary>
        /// 字段描述
        /// </summary>
        /// <param name="text"></param>
        /// <param name="needLog"></param>
        public FieldDescription(string text, bool needLog = true)
        {
            Text = text;
            NeedLog = needLog;
        }
    }
}