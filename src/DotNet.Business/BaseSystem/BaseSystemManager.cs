//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
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
    ///		<name>JiRiGaLa</name>
    ///		<date>2016.08.09</date>
    /// </author>
    /// </summary>
    public partial class BaseSystemManager : BaseManager
    {
        /// <summary>
        /// 获取系统编号清单
        /// </summary>
        /// <returns>系统编号清单</returns>
        public static string[] GetSystemCodes()
        {
            string[] result = null;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldIsPublic, 1),
                new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldDeleted, 0)
            };

            var manager = new BaseItemDetailsManager("ItemsSystem");
            result = manager.GetProperties(parameters, BaseItemDetailsEntity.FieldItemCode);

            return result;
        }

        /// <summary>
        /// 通过缓存获取系统编号清单
        /// </summary>
        /// <returns>系统编号清单</returns>
        public static string[] GetSystemCodesByCache()
        {
            var cacheKey = "Base.SystemCodes";
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            return CacheUtil.Cache<string[]>(cacheKey, () => GetSystemCodes(), true, false, cacheTime);
        }

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
                if (systemCodes != null && systemCodes.Length > 0)
                {
                    result = StringUtil.Exists(systemCodes, systemCode);
                }
            }

            return result;
        }
    }
}