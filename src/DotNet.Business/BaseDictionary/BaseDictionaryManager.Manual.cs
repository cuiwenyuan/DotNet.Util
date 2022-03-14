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
                result = UpdateEntity(entity);
                Status = Status.OkUpdate;
                StatusCode = Status.OkUpdate.ToString();
                StatusMessage = Status.OkUpdate.ToDescription();
            }
            status = Status;
            return result;
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
