//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    /// BaseSystemInfo
    /// 这是系统的核心基础信息部分
    /// 
    /// 修改记录
    ///		2021.03.17 版本：1.0 Troy.Cui	主键创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2021.03.17</date>
    /// </author>
    /// </summary>
    public partial class BaseSystemInfo
    {
        /// <summary>
        /// Cookie名称
        /// </summary>
        public static string CookieName = "DotNet";

        /// <summary>
        /// Cookie域
        /// </summary>
        public static string CookieDomain = string.Empty;

        /// <summary>
        /// Cookie过期天数设置（默认30天）
        /// </summary>
        public static int CookieExpires = 30;
    }
}