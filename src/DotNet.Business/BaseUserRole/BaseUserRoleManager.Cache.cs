//-----------------------------------------------------------------------
// <copyright file="BaseUserRoleManager.cs" company="DotNet">
//     Copyright (c) 2026, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

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
            // 实例级 effSC（与读侧 B1/B3 同源）：UserInfo.SystemCode → BaseSystemInfo.SystemCode → "Base"
            var effectiveSystemCode = UserInfo != null ? UserInfo.SystemCode : null;
            if (string.IsNullOrEmpty(effectiveSystemCode))
            {
                effectiveSystemCode = BaseSystemInfo.SystemCode;
            }
            if (string.IsNullOrEmpty(effectiveSystemCode))
            {
                effectiveSystemCode = "Base";
            }

            // 列表类缓存（跨子系统 + 全局，均清）
            var cacheKeyListBase = "List.Base.UserRole";
            var cacheKeyListSystemCode = "List." + effectiveSystemCode + ".UserRole";
            CacheUtil.Remove(cacheKeyListBase);
            CacheUtil.Remove(cacheKeyListSystemCode);

            // 数据表类缓存：GetDataTable 键为 "Dt.<effSC>.<CurrentTableName>.<companyId>.<flag>"
            // 用正则前缀删除覆盖所有 companyId / myCompanyOnly 组合（Mode A/B 共用同一写法，CurrentTableName 随模式变化）。
            // 同时兼容历史键 "Dt.<effSC>.UserRole" / "Dt.<effSC>.<UserId>.UserRole"（缺/不同表名段）一并清除。
            var prefixPattern = "^Dt\\." + Regex.Escape(effectiveSystemCode) + "\\." + Regex.Escape(CurrentTableName) + "\\.";
            CacheUtil.RemoveByRegex(prefixPattern);
            CacheUtil.RemoveByRegex("^Dt\\." + Regex.Escape(effectiveSystemCode) + "\\.UserRole");
            if (UserInfo != null)
            {
                CacheUtil.RemoveByRegex("^Dt\\." + Regex.Escape(effectiveSystemCode) + "\\." + Regex.Escape(UserInfo.Id.ToString()) + "\\.UserRole");
            }

            // 通用精确键（兼容既有写法：缺表名段的历史键）
            var cacheKeySystemCode = "Dt." + effectiveSystemCode + ".UserRole";
            var cacheKeySystemCodeUserId = "Dt.";
            if (UserInfo != null)
            {
                cacheKeySystemCodeUserId += effectiveSystemCode + "." + UserInfo.Id + ".UserRole";
            }
            CacheUtil.Remove(cacheKeySystemCode);
            CacheUtil.Remove(cacheKeySystemCodeUserId);

            // 表级精确键（无 effSC 段的历史键，兜底）
            var cacheKey = "Dt." + CurrentTableName;
            CacheUtil.Remove(cacheKey);

            result = true;
            return result;
        }
        #endregion

        #region public override bool RemoveCache(int id) 删除缓存（实体 + 数据表缓存对齐）

        /// <summary>
        /// 删除缓存（按主键），同时清除数据表类缓存，确保写（Add/Update/SetEnabled/SetDeleted）后 GetDataTable 缓存失效
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public override bool RemoveCache(int id)
        {
            // 同步清除 GetDataTable 等数据表缓存（与无参 RemoveCache 同一前缀逻辑），覆盖所有走 id 重载的写路径
            RemoveCache();
            // 保留基类实体缓存清除语义（BaseUserRole 实体缓存当前未启用，但保持兼容）
            var cacheKeyEntity = CurrentTableName + ".Entity.";
            if (id == 0)
            {
                CacheUtil.RemoveByRegex("^" + cacheKeyEntity + "+\\d+$");
            }
            else
            {
                CacheUtil.Remove(cacheKeyEntity + id);
            }
            return true;
        }
        #endregion
    }
}
