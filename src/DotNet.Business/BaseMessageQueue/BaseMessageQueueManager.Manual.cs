//-----------------------------------------------------------------------
// <copyright file="MessageQueueManager.cs" company="DotNet">
//     Copyright (c) 2019, All rights reserved.
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
    /// 修改纪录
    /// 
    ///	2016-12-18 版本：1.0 Troy.Cui 创建文件。
    ///		
    /// <author>
    ///	<name>Troy.Cui</name>
    ///	<date>2016-12-18</date>
    /// </author> 
    /// </summary>
    public partial class BaseMessageQueueManager : BaseManager, IBaseManager
    {
        #region 删除缓存

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <returns></returns>
        public override bool RemoveCache()
        {
            var result = false;

            var cacheKey = "DataTable." + BaseSystemInfo.ApplicationId + "." + CurrentTableName;
            var cacheKeyList = "List." + BaseSystemInfo.ApplicationId + "." + CurrentTableName;
            var cacheKeyListEmail = "List." + BaseSystemInfo.ApplicationId + "." + CurrentTableName + ".Email";
            var cacheKeyListSms = "List." + BaseSystemInfo.ApplicationId + "." + CurrentTableName + ".Sms";
            var cacheKeyPrefix = "List." + BaseSystemInfo.ApplicationId + "." + CurrentTableName;

            CacheUtil.Remove(cacheKeyList);
            CacheUtil.Remove(cacheKeyListEmail);
            CacheUtil.Remove(cacheKeyListSms);
            CacheUtil.RemoveByRegex("^" + cacheKeyPrefix + "+\\*?$");
            result = CacheUtil.Remove(cacheKey);
            return result;
        }
        #endregion

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
                        CreateOn = entity.CreateOn,
                        SortCode = 1
                    };
                    //entityFailed.Error = "";
                    if (!string.IsNullOrWhiteSpace(new BaseMessageFailedManager(UserInfo).Add(entityFailed)))
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
                            CreateOn = entity.CreateOn,
                            SortCode = 1
                        };
                        if (!string.IsNullOrWhiteSpace(new BaseMessageSucceedManager(UserInfo).Add(entitySucceed)))
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
                        UpdateObject(entity);
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
#if NET40_OR_GREATER
            var host = System.Web.Hosting.HostingEnvironment.ApplicationID.Replace("/", "");
#elif NETSTANDARD2_0_OR_GREATER
//TO-DO
            var host = string.Empty;
#endif

            //每次发一封，避免超时，任务不停启动而listEntity并未重新获取
            var cacheKey = "List." + host + "." + CurrentTableName + ".Email";
            //var cacheTime = default(TimeSpan);
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            var messageType = "Email";
            var listEntity = CacheUtil.Cache<List<BaseMessageQueueEntity>>(cacheKey, () => GetList<BaseMessageQueueEntity>(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(BaseMessageQueueEntity.FieldMessageType, messageType), new KeyValuePair<string, object>(BaseMessageQueueEntity.FieldSource, host) }, 1, BaseMessageQueueEntity.FieldId), true, false, cacheTime);

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

        #region 重新发送短信

        /// <summary>
        /// 重新发送消息
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="maxFailCount"></param>
        /// <returns>是否成功</returns>
        private bool ResendSms(BaseMessageQueueEntity entity, int maxFailCount = 5)
        {
            var result = false;
            if (entity.MessageType.Equals("sms", StringComparison.OrdinalIgnoreCase))
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
                        CreateOn = entity.CreateOn,
                        SortCode = 1
                    };
                    //entityFailed.Error = "";
                    if (!string.IsNullOrWhiteSpace(new BaseMessageFailedManager(UserInfo).Add(entityFailed)))
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
                            CreateOn = entity.CreateOn,
                            SortCode = 1
                        };
                        if (!string.IsNullOrWhiteSpace(new BaseMessageSucceedManager(UserInfo).Add(entitySucceed)))
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
                        UpdateObject(entity);
                    }
                }

            }
            return result;
        }
        #endregion

        #region 重新发送所有短信队列
        /// <summary>
        /// 重新发送所有队列
        /// </summary>
        /// <returns>发送成功数量</returns>
        public int ResendSms(int maxFailCount = 5)
        {
            var result = 0;

            //每次发一封，避免超时，任务不停启动而listEntity并未重新获取
            var cacheKey = "List." + BaseSystemInfo.ApplicationId + "." + CurrentTableName + ".Sms";
            //var cacheTime = default(TimeSpan);
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            var messageType = "Sms";
            var listEntity = CacheUtil.Cache<List<BaseMessageQueueEntity>>(cacheKey, () => GetList<BaseMessageQueueEntity>(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(BaseMessageQueueEntity.FieldMessageType, messageType), new KeyValuePair<string, object>(BaseMessageQueueEntity.FieldSource, BaseSystemInfo.ApplicationId) }, 1, BaseMessageQueueEntity.FieldId), true, false, cacheTime);

            foreach (var entity in listEntity)
            {
                if (ResendSms(entity, maxFailCount))
                {
                    result++;
                }
            }

            return result;
        }
        #endregion

        #region 高级查询

        /// <summary>
        /// 按条件分页查询(带记录状态Enabled和删除状态DeletionStateCode)
        /// </summary>
        /// <param name="companyId">查看公司主键</param>
        /// <param name="departmentId">查看部门主键</param>
        /// <param name="userId">查看用户主键</param>
        /// <param name="startTime">创建开始时间</param>
        /// <param name="endTime">创建结束时间</param>
        /// <param name="messageType">消息类型</param>
        /// <param name="recipient">收件人</param>
        /// <param name="searchKey">查询字段</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="showDisabled">是否显示无效记录</param>
        /// <param name="showDeleted">是否显示已删除记录</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string messageType, string recipient, string searchKey, out int recordCount, int pageIndex = 0, int pageSize = 20, string sortExpression = "CreateOn", string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            pageIndex++;
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND Enabled = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND DeletionStateCode = 0");
            }

            if (ValidateUtil.IsInt(companyId))
            {
                //sb.Append(" AND CompanyId = " + companyId);
            }
            sb.Append(" AND (UserCompanyId = 0 OR UserCompanyId = " + UserInfo.CompanyId + ")");
            if (ValidateUtil.IsInt(departmentId))
            {
                //sb.Append(" AND DepartmentId = " + departmentId);
            }
            if (ValidateUtil.IsInt(userId))
            {
                //sb.Append(" AND UserId = " + userId);
            }
            //创建日期
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND CreateOn >= '" + startTime + "'");
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND CreateOn <= DATEADD(s,-1,DATEADD(d,1,'" + endTime + "'))");
            }
            if (!string.IsNullOrEmpty(messageType))
            {
                messageType = dbHelper.SqlSafe(messageType);
                sb.Append(" AND MessageType = N'%" + messageType + "%'");
            }
            if (!string.IsNullOrEmpty(recipient))
            {
                recipient = dbHelper.SqlSafe(recipient);
                sb.Append(" AND Recipient = N'%" + recipient + "%'");
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (Recipient LIKE N'%" + searchKey + "%' OR Subject LIKE N'%" + searchKey + "%' OR Body LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageIndex, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Put(), null, "*");
        }
        #endregion

        #region GetTotalCount
        /// <summary>
        /// 获取总记录数
        /// </summary>
        /// <param name="days">天数</param>
        /// <returns></returns>
        public string GetTotalCount(int days)
        {
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) AS TotalCount FROM " + CurrentTableName + " WHERE (DATEADD(d, " + days + ", " + BaseMessageQueueEntity.FieldCreateTime + ") >= " + DbHelper.GetDbNow() + ")");
            return ExecuteScalar(sb.Put())?.ToString();
        }
        #endregion

        #region 应用启动邮件

        /// <summary>
        /// 应用启动邮件
        /// </summary>
        /// <returns>是否成功</returns>
        public bool ApplicationRestart()
        {
            var result = false;
#if NET40_OR_GREATER
            //发送邮件，写入数据库
            var entity = new BaseMessageQueueEntity
            {
                Source = BaseSystemInfo.ApplicationId,
                Recipient = BaseSystemInfo.MailBcc,
                Subject = "Web服务重新启动了 - " + System.Web.Hosting.HostingEnvironment.SiteName + " - " + BaseSystemInfo.SoftFullName,
                Body = "主人：<br>运行于<b>" + System.Web.Hosting.HostingEnvironment.SiteName + "</b>的<b>" + BaseSystemInfo.SoftFullName +
                       "</b>于<b>" + DateTime.Now.AddSeconds(-1) + "</b>重新启动了。" + Environment.NewLine
                       + "<br>ApplicationID：" + BaseSystemInfo.ApplicationId + "<br>ApplicationPhysicalPath：" + System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "<br>ApplicationVirtualPath：" + System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath + "<br><br>" + Environment.NewLine + BaseSystemInfo.SoftFullName + "<br>自动发送" + "<br>" + DateTime.Now,
                SortCode = 1
            };
            if (!string.IsNullOrEmpty(Add(entity)))
            {
                result = true;
            }
#endif
            return result;
        }
        #endregion
    }
}
