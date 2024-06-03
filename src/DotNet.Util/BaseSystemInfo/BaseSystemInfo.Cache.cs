//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System;
using System.Configuration;

namespace DotNet.Util
{
    /// <summary>
    /// BaseSystemInfo
    /// 这是系统的核心基础信息部分
    /// 
    /// 修改记录
    ///		2017.04.14 版本：1.0 Troy.Cui	主键创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2017.04.14</date>
    /// </author>
    /// </summary>
    public partial class BaseSystemInfo
    {
        /// <summary>
        /// 默认Memory Cache缓存时间(1 秒=1000 毫秒、1 分=60000 毫秒、1 时=3600000 毫秒、1 天=86400000 毫秒)
        /// </summary>
        public static int MemoryCacheMillisecond = 1;

        /// <summary>
        /// 是否记录缓存操作日志
        /// </summary>
        public static bool LogCache = false;
    }
}