//-----------------------------------------------------------------------
// <copyright file="BaseMessageQueueManager.cs" company="DotNet">
//     Copyright (c) 2024, All rights reserved.
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
        #region 应用启动邮件

        /// <summary>
        /// 应用启动邮件
        /// </summary>
        /// <returns>是否成功</returns>
        public bool ApplicationRestart()
        {
            var result = false;
#if NET452_OR_GREATER
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
