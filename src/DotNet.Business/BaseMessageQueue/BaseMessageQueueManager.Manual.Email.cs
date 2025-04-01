//-----------------------------------------------------------------------
// <copyright file="BaseMessageQueueManager.cs" company="DotNet">
//     Copyright (c) 2025, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Data;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseMessageQueueManager
    /// 消息队列管理层
    /// 
    /// 修改记录
    /// 
    ///	2023-06-20 版本：1.0 Troy.Cui 创建文件。
    ///		
    /// <author>
    ///	<name>Troy.Cui</name>
    ///	<date>2023-06-20</date>
    /// </author> 
    /// </summary>
    public partial class BaseMessageQueueManager : BaseManager
    {
        #region 重新发送邮件

        /// <summary>
        /// 重新发送消息
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="maxFailCount"></param>
        /// <returns>是否成功</returns>
        private bool ResendEmail(BaseMessageQueueEntity entity, int maxFailCount = 5)
        {
            var result = false;
            if (entity.MessageType.Equals("Email", StringComparison.OrdinalIgnoreCase))
            {
                if (entity.FailCount >= maxFailCount)
                {
                    //发送失败超过5次，移动数据到MessageFailed表
                    var entityFailed = new BaseMessageFailedEntity
                    {
                        Source = entity.Source,
                        MessageType = entity.MessageType,
                        Recipient = entity.Recipient,
                        Subject = entity.Subject,
                        Body = entity.Body,
                        FailCount = entity.FailCount,
                        CreateTime = entity.CreateTime,
                        SortCode = 1
                    };
                    //entityFailed.Error = "";
                    if (!string.IsNullOrEmpty(new BaseMessageFailedManager(UserInfo).Add(entityFailed)))
                    {
                        //删除MessageQueue表中的数据
                        Delete(entity.Id);
                        RemoveCache();
                    }
                    result = false;
                }
                else
                {
                    if (MailUtil.Send(entity.Recipient, entity.Subject, entity.Body))
                    {
                        //发送成功，移动数据到MessageSucceed表
                        var entitySucceed = new BaseMessageSucceedEntity
                        {
                            Source = entity.Source,
                            MessageType = entity.MessageType,
                            Recipient = entity.Recipient,
                            Subject = entity.Subject,
                            Body = entity.Body,
                            CreateTime = entity.CreateTime,
                            SortCode = 1
                        };
                        if (!string.IsNullOrEmpty(new BaseMessageSucceedManager(UserInfo).Add(entitySucceed)))
                        {
                            //删除MessageQueue表中的数据
                            Delete(entity.Id);
                            RemoveCache();
                            result = true;
                        }
                    }
                    else
                    {
                        //更新MessageQueue表中的失败次数
                        entity.FailCount = entity.FailCount + 1;
                        UpdateEntity(entity);
                    }
                }
            }
            return result;
        }
        #endregion

        #region 重新发送所有邮件队列
        /// <summary>
        /// 重新发送所有队列
        /// </summary>
        /// <returns>发送成功数量</returns>
        public int ResendEmail(int maxFailCount = 5)
        {
            var result = 0;

            //每次发一封，避免超时，任务不停启动而listEntity并未重新获取
            var cacheKey = "List." + BaseSystemInfo.ApplicationId + "." + CurrentTableName + ".Email";
            //var cacheTime = default(TimeSpan);
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            var messageType = "Email";
            var listEntity = CacheUtil.Cache<List<BaseMessageQueueEntity>>(cacheKey, () => GetList<BaseMessageQueueEntity>(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(BaseMessageQueueEntity.FieldMessageType, messageType), new KeyValuePair<string, object>(BaseMessageQueueEntity.FieldSource, BaseSystemInfo.ApplicationId) }, 1, BaseMessageQueueEntity.FieldId), true, false, cacheTime);

            foreach (var entity in listEntity)
            {
                if (ResendEmail(entity, maxFailCount))
                {
                    result++;
                }
            }

            return result;
        }
        #endregion

    }
}
