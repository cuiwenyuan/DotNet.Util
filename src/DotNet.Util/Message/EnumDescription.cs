//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Util
{
    /// <summary>
    /// EnumDescription
    /// 枚举状态的说明。
    /// 
    /// 修改记录
    /// 
    ///		2012.02.04 版本：1.1 JiRiGaLa 重新排版。
    ///		2011.10.13 版本：1.0 JiRiGaLa 创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.02.04</date>
    /// </author> 
    /// </summary>    
    public class EnumDescription : Attribute
    {
        private string _text;
        /// <summary>
        /// Text
        /// </summary>
        public string Text => _text;
        /// <summary>
        /// EnumDescription
        /// </summary>
        /// <param name="text"></param>
        public EnumDescription(string text)
        {
            _text = text;
        }
    }
}