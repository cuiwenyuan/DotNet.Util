//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseSystemManager 
    /// 子系统管理
    ///
    /// 修改记录
    ///
    ///		2016.08.09 版本：1.1 JiRiGaLa  CheckSystemCode 函数改进。
    ///		2015.12.10 版本：1.0 JiRiGaLa  创建。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.08.09</date>
    /// </author>
    /// </summary>
    public partial class BaseSystemManager : BaseManager
    {
        #region public static List<BaseDictionaryItemEntity> GetSystemCodes() 获取系统编号清单
        /// <summary>
        /// 获取系统编号清单
        /// </summary>
        /// <returns>系统编号清单</returns>
        public static List<BaseDictionaryItemEntity> GetSystemCodes()
        {
            var result = new List<BaseDictionaryItemEntity>();
            result = new BaseDictionaryItemManager().GetDataTableByDictionaryCode("BaseSystem").ToList<BaseDictionaryItemEntity>();
            return result;
        }

        #endregion

        #region public static List<BaseDictionaryItemEntity> GetSystemCodesByCache() 通过缓存获取系统编号清单
        /// <summary>
        /// 通过缓存获取系统编号清单
        /// </summary>
        /// <returns>系统编号清单</returns>
        public static List<BaseDictionaryItemEntity> GetSystemCodesByCache()
        {
            var cacheKey = "Base.SystemCodes";
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            return CacheUtil.Cache<List<BaseDictionaryItemEntity>>(cacheKey, () => GetSystemCodes(), true, false, cacheTime);
        }

        #endregion

        #region public static bool CheckSystemCode(string systemCode) 当前系统是否有效的系统编号
        /// <summary>
        /// 当前系统是否有效的系统编号？
        /// 2016-08-09 吉日嘎拉
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <returns>有效</returns>
        public static bool CheckSystemCode(string systemCode)
        {
            var result = false;

            if (!string.IsNullOrEmpty(systemCode))
            {
                var systemCodes = GetSystemCodesByCache();
                if (systemCodes != null && systemCodes.Count > 0)
                {
                    result = systemCodes.Exists(e => e.ItemKey.Equals(systemCode, StringComparison.OrdinalIgnoreCase));
                }
            }

            return result;
        }

        #endregion
    }
}