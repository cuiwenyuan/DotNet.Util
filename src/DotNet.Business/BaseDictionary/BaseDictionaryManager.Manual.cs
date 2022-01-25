//-----------------------------------------------------------------------
// <copyright file="BaseDictionaryManager.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Business;
    using Util;

    /// <summary>
    /// BaseDictionaryManager
    /// 字典管理层
    /// 
    /// 修改记录
    /// 
    ///	2021-10-26 版本：1.0 Troy.Cui 创建文件。
    ///		
    /// <author>
    ///	<name>Troy.Cui</name>
    ///	<date>2021-10-26</date>
    /// </author> 
    /// </summary>
    public partial class BaseDictionaryManager : BaseManager, IBaseManager
    {
        #region GetEntityByCode
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        public BaseDictionaryEntity GetEntityByCode(string code)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseDictionaryEntity.FieldCode, code),
                new KeyValuePair<string, object>(BaseDictionaryEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseDictionaryEntity.FieldEnabled, 1)
            };
            var cacheKey = CurrentTableName + ".Entity." + code;
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            return CacheUtil.Cache<BaseDictionaryEntity>(cacheKey, () => BaseEntity.Create<BaseDictionaryEntity>(ExecuteReader(parameters)), true, false, cacheTime);
        }
        #endregion
    }
}
