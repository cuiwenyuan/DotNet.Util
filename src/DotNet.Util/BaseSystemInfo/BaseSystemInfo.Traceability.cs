//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    /// BaseSystemInfo
    /// 这是系统的核心基础信息部分
    /// 
    /// 修改记录
    ///		2018.10.18 版本：1.0 Troy.Cui	针对追溯码。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2018.10.18</date>
    /// </author>
    /// </summary>
    public partial class BaseSystemInfo
    {
        /// <summary>
        /// 是否启用追溯码
        /// </summary>
        public static bool TraceabilityCodeEnabled = false;

        /// <summary>
        /// 用于生成和反转追溯码的钥匙,随机的62位字符串，包含0-9a-zA-Z
        /// </summary>
        public static string TraceabilityKey = "MljrLJGf30idtqnTVU56ORQWwzxHYk1NpbA7gPahs2KFv9m4yoCXBDIEuc8SeZ";
    }
}