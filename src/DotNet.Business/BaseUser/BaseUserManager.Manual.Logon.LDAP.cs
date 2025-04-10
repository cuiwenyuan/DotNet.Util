﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserManager 
    /// 用户管理 扩展类 
    /// 
    /// 修改记录
    /// 
    ///		2016.07.04 版本：3.1 Troy.Cui 扩展LDAP登录功能。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.07.04</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        /// <summary>
        /// 根据OpenId获取用户实体
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public BaseUserEntity GetByOpenId(string openId)
        {
            BaseUserEntity entity = null;
            // 用户没有找到状态
            Status = Status.UserNotFound;
            StatusCode = Status.UserNotFound.ToString();
            // 检查是否有效的合法的参数
            if (!string.IsNullOrEmpty(openId))
            {
                var userLogonManager = new BaseUserLogonManager();
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserLogonEntity.FieldOpenId, openId)
                };
                var id = userLogonManager.GetId(parameters);
                if (!string.IsNullOrEmpty(id))
                {
                    parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BaseUserEntity.FieldId, id),
                        new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0),
                        new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1)
                    };
                    var dt = GetDataTable(parameters);
                    if (dt != null && dt.Rows.Count == 1)
                    {
                        entity = BaseEntity.Create<BaseUserEntity>(dt);
                    }
                }
            }
            return entity;
        }
        /// <summary>
        /// 根据用户名获取用户实体
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public BaseUserEntity GetByUserName(string userName)
        {
            BaseUserEntity entity = null;
            // 用户没有找到状态
            Status = Status.UserNotFound;
            StatusCode = Status.UserNotFound.ToString();
            // 检查是否有效的合法的参数
            if (!string.IsNullOrEmpty(userName))
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserEntity.FieldUserName, userName),
                    new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0),
                    new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1)
                };
                var dt = GetDataTable(parameters);
                if (dt != null && dt.Rows.Count == 1)
                {
                    entity = BaseEntity.Create<BaseUserEntity>(dt);
                }
            }
            return entity;
        }

        /// <summary>
        /// 根据邮件获取用户
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public BaseUserEntity GetByEmail(string email)
        {
            BaseUserEntity entity = null;
            // 用户没有找到状态
            Status = Status.UserNotFound;
            StatusCode = Status.UserNotFound.ToString();
            // 检查是否有效的合法的参数
            if (!string.IsNullOrEmpty(email) && ValidateUtil.IsEmail(email))
            {
                var userContactManager = new BaseUserContactManager();
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserContactEntity.FieldEmail, email)
                };
                var id = userContactManager.GetId(parameters);
                if (!string.IsNullOrEmpty(id))
                {
                    parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BaseUserEntity.FieldId, id),
                        new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0),
                        new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1)
                    };
                    var dt = GetDataTable(parameters);
                    if (dt != null && dt.Rows.Count == 1)
                    {
                        entity = BaseEntity.Create<BaseUserEntity>(dt);
                    }
                }
            }
            return entity;
        }
    }
}
