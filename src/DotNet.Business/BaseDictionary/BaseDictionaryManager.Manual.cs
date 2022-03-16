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
    using System.Linq;

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
        #region UniqueAdd
        /// <summary>
        /// 检查唯一值式新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string UniqueAdd(BaseDictionaryEntity entity, out Status status)
        {
            var result = string.Empty;
            //检查是否重复
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseDictionaryEntity.FieldTenantId, entity.TenantId),
                new KeyValuePair<string, object>(BaseDictionaryEntity.FieldCode, entity.Code),
                new KeyValuePair<string, object>(BaseDictionaryEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseDictionaryEntity.FieldDeleted, 0)
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
        /// <param name="entity"></param>
        /// <returns></returns>
        public int UniqueUpdate(BaseDictionaryEntity entity, out Status status)
        {
            var result = 0;

            //检查是否重复
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseDictionaryEntity.FieldTenantId, entity.TenantId),
                new KeyValuePair<string, object>(BaseDictionaryEntity.FieldCode, entity.Code),
                new KeyValuePair<string, object>(BaseDictionaryEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseDictionaryEntity.FieldDeleted, 0)
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
                        Status = Status.AccessDeny;
                        StatusCode = Status.AccessDeny.ToString();
                        StatusMessage = Status.AccessDeny.ToDescription();
                    }
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
            }
            status = Status;
            return result;
        }

        #endregion

        #region SaveEntityChangeLog
        /// <summary>
        /// 保存实体修改记录
        /// </summary>
        /// <param name="newEntity">修改前的实体对象</param>
        /// <param name="oldEntity">修改后的实体对象</param>
        /// <param name="tableName">表名称</param>
        public void SaveEntityChangeLog(BaseDictionaryEntity newEntity, BaseDictionaryEntity oldEntity, string tableName = null)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                //统一放在一个公共表 Troy.Cui 2016-08-17
                tableName = BaseChangeLogEntity.CurrentTableName;
            }
            var manager = new BaseChangeLogManager(UserInfo, tableName);
            foreach (var property in typeof(BaseDictionaryEntity).GetProperties())
            {
                var oldValue = Convert.ToString(property.GetValue(oldEntity, null));
                var newValue = Convert.ToString(property.GetValue(newEntity, null));
                var fieldDescription = property.GetCustomAttributes(typeof(FieldDescription), false).FirstOrDefault() as FieldDescription;
                //不记录创建人、修改人、没有修改的记录
                if (!fieldDescription.NeedLog || oldValue == newValue)
                {
                    continue;
                }
                var record = new BaseChangeLogEntity
                {
                    TableName = CurrentTableName,
                    TableDescription = FieldExtensions.ToDescription(typeof(BaseDictionaryEntity), "CurrentTableName"),
                    ColumnName = property.Name,
                    ColumnDescription = fieldDescription.Text,
                    NewValue = newValue,
                    OldValue = oldValue,
                    RecordKey = oldEntity.Id.ToString()
                };
                manager.Add(record, true, false);
            }
        }
        #endregion

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
