//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Business
{
    /// <summary>
    /// BaseUserManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///		2013.11.11 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2013.11.11</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        /// <summary>
        /// 同步
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int Synchronous(string userId)
        {
            var result = 0;

            result = RefreshCache(userId);

            return result;
        }
    }
}