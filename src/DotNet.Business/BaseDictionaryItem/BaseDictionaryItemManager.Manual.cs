//-----------------------------------------------------------------------
// <copyright file="BaseDictionaryItemManager.cs" company="DotNet">
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
    /// BaseDictionaryItemManager
    /// 字典项管理层
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
    public partial class BaseDictionaryItemManager : BaseManager, IBaseManager
    {
        #region 更改父节点 ChangeParentId
        /// <summary>
        /// 更改父节点
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="parentId">父级主键</param>
        /// <returns>影响行数</returns>
        public int ChangeParentId(string id, string parentId)
        {
            var result = 0;
            if (ValidateUtil.IsInt(id) && ValidateUtil.IsInt(parentId))
            {
                var entity = GetEntity(id);
                if (entity != null)
                {
                    if (parentId.ToInt() > 0)
                    {
                        var entityParent = GetEntity(parentId);
                        if (entityParent != null && entityParent.DictionaryId == entity.DictionaryId)
                        {
                            result = UpdateProperty(id, new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldParentId, parentId));
                        }
                    }
                    else if (parentId.ToInt() == 0)
                    {
                        result = UpdateProperty(id, new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldParentId, parentId));
                    }
                }
            }
            return result;
        }
        #endregion

        #region 根据字典编码、主键获取实体 GetEntity
        /// <summary>
        /// 根据字典编码、主键获取实体
        /// </summary>
        /// <param name="dictionaryCode">字典编码</param>
        /// <param name="itemKey">字典项主键</param>
        /// <param name="itemValue">字典项值</param>
        /// <returns></returns>
        public BaseDictionaryItemEntity GetEntity(string dictionaryCode, string itemKey, string itemValue = null)
        {
            BaseDictionaryItemEntity entity = null;
            if (!string.IsNullOrEmpty(dictionaryCode) && !string.IsNullOrEmpty(itemKey))
            {
                var entityBaseDictionary = new BaseDictionaryManager(UserInfo).GetEntityByCode(dictionaryCode);
                if (entityBaseDictionary != null)
                {
                    var parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldDictionaryId, entityBaseDictionary.Id),
                        new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldItemKey, itemKey),
                        new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldDeleted, 0),
                        new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldEnabled, 1)
                    };
                    if (!string.IsNullOrEmpty(itemValue))
                    {
                        parameters.Add(new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldItemValue, itemValue));
                    }
                    var cacheKey = CurrentTableName + ".Entity." + dictionaryCode + "." + itemKey;
                    var cacheTime = TimeSpan.FromMilliseconds(86400000);
                    entity = CacheUtil.Cache<BaseDictionaryItemEntity>(cacheKey, () => BaseEntity.Create<BaseDictionaryItemEntity>(ExecuteReader(parameters)), true, false, cacheTime);
                }
            }
            return entity;
        }
        #endregion

        #region 根据字典编码获取数据表 GetDataTableByDictionaryCode

        /// <summary>
        /// 根据字典编码获取数据表
        /// </summary>
        /// <param name="dictionaryCode">字典编码</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByDictionaryCode(string dictionaryCode)
        {
            var dt = new DataTable();
            if (!string.IsNullOrEmpty(dictionaryCode))
            {
                var entity = new BaseDictionaryManager(UserInfo).GetEntityByCode(dictionaryCode);
                if (entity != null)
                {
                    var cacheKey = "DataTable." + CurrentTableName + "." + dictionaryCode;
                    var cacheTime = TimeSpan.FromMilliseconds(86400000);
                    var parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldDictionaryId, entity.Id),
                        new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldDeleted, 0),
                        new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldEnabled, 1)
                    };
                    dt = CacheUtil.Cache<DataTable>(cacheKey, () => GetDataTable(parameters), true, false, cacheTime);
                }
            }

            return dt;
        }

        #endregion
    }
}
