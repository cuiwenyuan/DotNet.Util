//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Business
{
    using Util;

    /// <summary>
    /// BaseUserRoleManager
    /// 用户-角色 关系
    ///
    /// 修改记录
    ///
    ///     2018-09-07 版本：4.1 Troy.Cui   增加删除缓存功能。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2018-09-07</date>
    /// </author>
    /// </summary>
    public partial class BaseUserRoleManager : BaseManager, IBaseManager
    {
        #region 删除缓存

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <returns></returns>
        public override bool RemoveCache()
        {
            var result = false;
            var cacheKey = "DataTable." + CurrentTableName;
            var cacheKeyListBase = "List.Base.UserRole";
            var cacheKeyListSystemCode = "List.UserBase.Role";
            var cacheKeySystemCode = "DataTable.Base.UserRole";
            var cacheKeySystemCodeUserId = "DataTable.";
            if (UserInfo != null)
            {
                //cacheKey += "." + UserInfo.CompanyId;
                cacheKeyListSystemCode = "List." + UserInfo.SystemCode + ".UserRole";
                cacheKeySystemCode = "DataTable." + UserInfo.SystemCode + ".UserRole";
                cacheKeySystemCodeUserId += UserInfo.SystemCode + "." + UserInfo.Id + ".UserRole";
            }
            CacheUtil.Remove(cacheKeyListBase);
            CacheUtil.Remove(cacheKeyListSystemCode);
            CacheUtil.Remove(cacheKeySystemCode);
            CacheUtil.Remove(cacheKeySystemCodeUserId);
            result = CacheUtil.Remove(cacheKey);
            return result;
        }
        #endregion
    }
}
