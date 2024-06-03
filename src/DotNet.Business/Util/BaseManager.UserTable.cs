//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Business
{
    /// <summary>
    ///	BaseManager
    /// 通用基类部分
    /// 
    /// 修改记录
    /// 
    ///		2020.02.06 版本：Troy.Cui进行扩展。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2020.02.06</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region 获取表后缀

        /// <summary>
        /// 获取表后缀
        /// </summary>
        /// <returns>表后缀</returns>
        public virtual string GetTableSuffix()
        {
            var result = string.Empty;
            if (UserInfo != null && !string.IsNullOrEmpty(UserInfo.CompanyId) && !UserInfo.IsAdministrator)
            {
                result = UserInfo.CompanyId.ToString();
            }

            return result;
        }

        #endregion
    }
}