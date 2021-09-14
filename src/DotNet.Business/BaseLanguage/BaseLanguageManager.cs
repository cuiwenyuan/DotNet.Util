//-----------------------------------------------------------------------
// <copyright file="BaseLanguageManager.cs" company="DotNet">
//     Copyright (C) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseLanguageManager
    /// 多语言
    /// 
    /// 修改记录
    /// 
    /// 2015-02-25 版本：1.0 JiRiGaLa 创建文件。
    /// 
    /// <author>
    ///     <name>JiRiGaLa</name>
    ///     <date>2015-02-25</date>
    /// </author>
    /// </summary>
    public partial class BaseLanguageManager
    {
        #region public string Add(BaseLanguageEntity entity, out string statusCode) 添加
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <returns>主键</returns>
        public string Add(BaseLanguageEntity entity, out string statusCode)
        {
            var result = string.Empty;
            // 检查名称是否重复
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseLanguageEntity.FieldLanguageCode, entity.LanguageCode),
                new KeyValuePair<string, object>(BaseLanguageEntity.FieldMessageCode, entity.MessageCode),
                new KeyValuePair<string, object>(BaseLanguageEntity.FieldDeleted, 0)
            };
            if (Exists(parameters))
            {
                // 名称是否重复
                statusCode = Status.ErrorNameExist.ToString();
            }
            else
            {
                result = AddObject(entity);
                // 运行成功
                statusCode = Status.OkAdd.ToString();
            }
            return result;
        }
        #endregion

        #region public int Update(BaseLanguageEntity entity, out string statusCode) 更新
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <returns>影响行数</returns>
        public int Update(BaseLanguageEntity entity, out string statusCode)
        {
            var result = 0;
            // 检查是否已被其他人修改

            if (DbUtil.IsModified(DbHelper, CurrentTableName, entity.Id, entity.ModifiedUserId, entity.ModifiedOn))
            {
                // 数据已经被修改
                statusCode = Status.ErrorChanged.ToString();
            }
            else
            {
                // 检查名称是否重复
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseLanguageEntity.FieldLanguageCode, entity.LanguageCode),
                    new KeyValuePair<string, object>(BaseLanguageEntity.FieldMessageCode, entity.MessageCode),
                    new KeyValuePair<string, object>(BaseLanguageEntity.FieldDeleted, 0)
                };
                if (Exists(parameters, entity.Id))
                {
                    // 名称已重复
                    statusCode = Status.ErrorCodeExist.ToString();
                }
                else
                {
                    result = UpdateObject(entity);
                    if (result == 1)
                    {
                        statusCode = Status.OkUpdate.ToString();
                    }
                    else
                    {
                        statusCode = Status.ErrorDeleted.ToString();
                    }
                }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 设置语言
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="messageCode"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public int SetLanguage(string languageCode, string messageCode, string caption)
        {
            var result = 0;

            // 更新的条件部分
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseLanguageEntity.FieldLanguageCode, languageCode),
                new KeyValuePair<string, object>(BaseLanguageEntity.FieldMessageCode, messageCode),
                new KeyValuePair<string, object>(BaseLanguageEntity.FieldDeleted, 0)
            };
            // 更新的内容部分
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseLanguageEntity.FieldCaption, caption)
            };
            result = SetProperty(whereParameters, parameters);

            // 若没能更新就进行新增操作
            if (result == 0)
            {
                var entity = new BaseLanguageEntity
                {
                    LanguageCode = languageCode,
                    MessageCode = messageCode,
                    Caption = caption,
                    DeletionStateCode = 0,
                    ModifiedOn = DateTime.Now,
                    CreateOn = DateTime.Now
                };
                AddObject(entity);
                result++;
            }
            return result;
        }

        #region public int BatchSave(List<BaseLanguageEntity> entities) 批量保存
        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <returns>影响行数</returns>
        public int BatchSave(List<BaseLanguageEntity> entities)
        {
            var result = 0;
            foreach (var entity in entities)
            {
                result += UpdateObject(entity);
            }
            return result;
        }
        #endregion
    }
}
