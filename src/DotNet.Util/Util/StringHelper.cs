//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNet.Util
{
    /// <summary>
    ///	StringUtil
    /// 字符串辅助类
    /// 
    /// 
    /// 修改记录
    /// 
    ///		2016.01.12 版本：1.0	SongBiao
    ///	
    /// <author>
    ///		<name>SongBiao</name>
    ///		<date>2016.01.12</date>
    /// </author> 
    /// </summary>

    public class StringHelper
    {
        private readonly char[] _constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        /// <summary>
        /// 生成随机字符串，默认32位
        /// </summary>
        /// <param name="length">随机数长度</param>
        /// <returns></returns>
        public string GenerateRandom(int length = 32)
        {
            var sb = Pool.StringBuilder.Get();
            var rd = new Random();
            for (int i = 0; i < length; i++)
            {
                sb.Append(_constant[rd.Next(_constant.Length)]);
            }
            return sb.Put();
        }

        /// <summary>
        /// 生成随机字符串，只包含数字
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string GenerateRandomNumber(int length = 6)
        {
            var sb = Pool.StringBuilder.Get();
            var rd = new Random();
            for (int i = 0; i < length; i++)
            {
                sb.Append(_constant[rd.Next(10)]);
            }
            return sb.Put();
        }
    }
}