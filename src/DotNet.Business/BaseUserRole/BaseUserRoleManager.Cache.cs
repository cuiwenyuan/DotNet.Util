//-----------------------------------------------------------------------
// <copyright file="BaseUserRoleManager.cs" company="DotNet">
//     Copyright (c) 2024, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserRoleManager
    /// 用户角色管理层
    /// 
    /// 修改记录
    ///
    ///     2021-01-12 版本：5.1 Troy.Cui   增加AddOrUpdate。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2021-01-12</date>
    /// </author>
    /// </summary>
    public partial class BaseUserRoleManager : BaseManager
    {
        #region 删除缓存

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <returns></returns>
        public override bool RemoveCache()
        {
            var result = false;
            var cacheKey = "Dt." + CurrentTableName;
            var cacheKeyListBase = "List.Base.UserRole";
            var cacheKeyListSystemCode = "List.UserBase.Role";
            var cacheKeySystemCode = "Dt.Base.UserRole";
            var cacheKeySystemCodeUserId = "Dt.";
            if (UserInfo != null)
            {
                //cacheKey += "." + UserInfo.CompanyId;
                cacheKeyListSystemCode = "List." + BaseSystemInfo.SystemCode + ".UserRole";
                cacheKeySystemCode = "Dt." + BaseSystemInfo.SystemCode + ".UserRole";
                cacheKeySystemCodeUserId += BaseSystemInfo.SystemCode + "." + UserInfo.Id + ".UserRole";
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
