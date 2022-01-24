//-----------------------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;


namespace DotNet.IService
{
    using Model;
    using Util;

    /// <summary>
    /// ILogonService
    /// 
    /// 修改记录
    /// 
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.07.04</date>
    /// </author> 
    /// </summary>
    public partial interface ILogonService
    {
        /// <summary>
        /// 按用户名登录(LDAP专用)
        /// </summary>
        /// <param name="taskId">任务唯一标识</param>
        /// <param name="systemCode">子系统编码</param>
        /// <param name="userInfo">用户</param>
        /// <param name="userName">用户名</param>
        /// <returns>用户实体</returns>
        UserLogonResult LogonByUserName(string taskId, string systemCode, BaseUserInfo userInfo, string userName);

    }
}