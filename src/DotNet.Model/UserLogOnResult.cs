﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    ///	UserLogonResult
    /// 登录返回状态
    /// 
    /// 修改记录
    /// 
    ///		2016.02.01 版本：1.0	JiRiGaLa 搬移位置。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.02.01</date>
    /// </author> 
    /// </summary>
    [Serializable]
    public class UserLogonResult
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        public BaseUserInfo UserInfo = null;

        /// <summary>
        /// 状态码
        /// </summary>
        public Status Status = Status.UserNotFound;

        /// <summary>
        /// 状态码
        /// </summary>
        public string StatusCode = Status.UserNotFound.ToString();

        /// <summary>
        /// 状态消息
        /// </summary>
        public string StatusMessage = Status.UserNotFound.ToDescription();

        /// <summary>
        /// 角色
        /// </summary>
        public List<BaseRoleEntity> Roles = new List<BaseRoleEntity>();

        /// <summary>
        /// 模块
        /// </summary>
        public List<BaseModuleEntity> Modules = new List<BaseModuleEntity>();

        /// <summary>
        /// 消息
        /// </summary>
        public string Message = "";
    }
}