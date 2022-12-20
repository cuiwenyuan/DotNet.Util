//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    /// BaseSystemInfo
    /// 这是系统的核心基础信息部分
    /// 
    /// 修改记录
    ///		2012.04.14 版本：1.0 JiRiGaLa	主键创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.04.14</date>
    /// </author>
    /// </summary>
    public partial class BaseSystemInfo
    {
        /// <summary>
        /// 用户名强制手机号
        /// </summary>
        public static bool UserNameForceMobileNumber = false;

        /// <summary>
        /// 用户名是否区分大小写
        /// </summary>
        public static bool UserNameMatchCase = true;
    }
}