//-----------------------------------------------------------------------
// <copyright file="BaseDictionaryItemManager.cs" company="DotNet">
//     Copyright (c) 2023, All rights reserved.
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
    using System.Linq;

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
    public partial class BaseDictionaryItemManager : BaseManager
    {
        #region UniqueAdd
        /// <summary>
        /// 检查唯一值式新增
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        public string UniqueAdd(BaseDictionaryItemEntity entity, out Status status)
        {
            var result = string.Empty;
            //检查是否重复
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldDictionaryId, entity.DictionaryId),
                new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldItemKey, entity.ItemKey),
                new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldDeleted, 0)
            };

            if (!IsUnique(parameters, entity.Id.ToString()))
            {
                //名称已重复
                Status = Status.ErrorNameExist;
                StatusCode = Status.ErrorNameExist.ToString();
                StatusMessage = Status.ErrorNameExist.ToDescription();
            }
            else
            {
                result = AddEntity(entity);
                if (!string.IsNullOrEmpty(result))
                {
                    Status = Status.OkAdd;
                    StatusCode = Status.OkAdd.ToString();
                    StatusMessage = Status.OkAdd.ToDescription();
                }
                else
                {
                    Status = Status.Error;
                    StatusCode = Status.Error.ToString();
                    StatusMessage = Status.Error.ToDescription();
                }
            }
            status = Status;
            return result;
        }

        #endregion

        #region UniqueUpdate
        /// <summary>
        /// 检查唯一值式更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        public int UniqueUpdate(BaseDictionaryItemEntity entity, out Status status)
        {
            var result = 0;

            //检查是否重复
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldDictionaryId, entity.DictionaryId),
                new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldItemKey, entity.ItemKey),
                new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldDeleted, 0)
            };

            if (!IsUnique(parameters, entity.Id.ToString()))
            {
                //名称已重复
                Status = Status.ErrorNameExist;
                StatusCode = Status.ErrorNameExist.ToString();
                StatusMessage = Status.ErrorNameExist.ToDescription();
            }
            else
            {
                // 获取原始实体信息
                var entityOld = GetEntity(entity.Id);
                if (entityOld != null)
                {
                    // 保存修改记录，无论是否允许
                    SaveEntityChangeLog(entity, entityOld);

                    if (entityOld.AllowEdit == 1)
                    {
                        result = UpdateEntity(entity);
                        if (result == 1)
                        {
                            Status = Status.OkUpdate;
                            StatusCode = Status.OkUpdate.ToString();
                            StatusMessage = Status.OkUpdate.ToDescription();
                        }
                        else
                        {
                            Status = Status.ErrorDeleted;
                            StatusCode = Status.ErrorDeleted.ToString();
                            StatusMessage = Status.ErrorDeleted.ToDescription();
                        }
                    }
                    else
                    {
                        Status = Status.NotAllowEdit;
                        StatusCode = Status.NotAllowEdit.ToString();
                        StatusMessage = Status.NotAllowEdit.ToDescription();
                    }
                }
            }
            status = Status;
            return result;
        }

        #endregion

        #region SaveEntityChangeLog
        /// <summary>
        /// 保存实体修改记录
        /// </summary>
        /// <param name="entityNew">修改后的实体对象</param>
        /// <param name="entityOld">修改前的实体对象</param>
        /// <param name="tableName">表名称</param>
        public void SaveEntityChangeLog(BaseDictionaryItemEntity entityNew, BaseDictionaryItemEntity entityOld, string tableName = null)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                //统一放在一个公共表 Troy.Cui 2016-08-17
                tableName = BaseChangeLogEntity.CurrentTableName;
            }
            var manager = new BaseChangeLogManager(UserInfo, tableName);
            foreach (var property in typeof(BaseDictionaryItemEntity).GetProperties())
            {
                var oldValue = Convert.ToString(property.GetValue(entityOld, null));
                var newValue = Convert.ToString(property.GetValue(entityNew, null));
                var fieldDescription = property.GetCustomAttributes(typeof(FieldDescription), false).FirstOrDefault() as FieldDescription;
                //不记录创建人、修改人、没有修改的记录
                if (!fieldDescription.NeedLog || oldValue == newValue)
                {
                    continue;
                }
                var entity = new BaseChangeLogEntity
                {
                    TableName = CurrentTableName,
                    TableDescription = typeof(BaseDictionaryItemEntity).FieldDescription("CurrentTableName"),
                    ColumnName = property.Name,
                    ColumnDescription = fieldDescription.Text,
                    NewValue = newValue,
                    OldValue = oldValue,
                    RecordKey = entityOld.Id.ToString()
                };
                manager.Add(entity, true, false);
            }
        }
        #endregion

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
                            result = Update(id, new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldParentId, parentId));
                        }
                    }
                    else if (parentId.ToInt() == 0)
                    {
                        result = Update(id, new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldParentId, parentId));
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
                    entity = CacheUtil.Cache<BaseDictionaryItemEntity>(cacheKey, () => BaseEntity.Create<BaseDictionaryItemEntity>(GetDataTable(parameters)), true, false, cacheTime);
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
