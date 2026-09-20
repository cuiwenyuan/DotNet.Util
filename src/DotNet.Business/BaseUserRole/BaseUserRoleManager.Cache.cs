//-----------------------------------------------------------------------
// <copyright file="BaseUserRoleManager.cs" company="DotNet">
//     Copyright (c) 2026, All rights reserved.
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
                // B3 修复：清缓存键改用“实例级 SystemCode”（当前用户所属子系统），而非全局 BaseSystemInfo.SystemCode，
                // 避免多子系统 Mode B 下只清全局子系统缓存、漏清其它子系统的 UserRole 缓存。
                // 单子系统部署（UserInfo.SystemCode 为空或与全局一致）时行为不变。
                var effectiveSystemCode = UserInfo.SystemCode;
                if (string.IsNullOrEmpty(effectiveSystemCode))
                {
                    effectiveSystemCode = BaseSystemInfo.SystemCode;
                }
                cacheKeyListSystemCode = "List." + effectiveSystemCode + ".UserRole";
                cacheKeySystemCode = "Dt." + effectiveSystemCode + ".UserRole";
                cacheKeySystemCodeUserId += effectiveSystemCode + "." + UserInfo.Id + ".UserRole";
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
