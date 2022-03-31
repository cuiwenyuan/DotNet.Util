//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///		2011.10.17 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.10.17</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        #region public void BeforeAdd(BaseUserEntity entity) 用户添加之前执行的方法
        /// <summary>
        /// 用户添加之前执行的方法
        /// </summary>
        /// <param name="entity">用户实体</param>
        public void BeforeAdd(BaseUserEntity entity)
        {
            // 检测成功，可以添加用户
            StatusCode = Status.OkAdd.ToString();
            if (!string.IsNullOrEmpty(entity.UserName)
                    && Exists(new KeyValuePair<string, object>(BaseUserEntity.FieldUserName, entity.UserName)
                , new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)))
            {
                // 用户名已重复
                StatusCode = Status.ErrorNameExist.ToString();
            }
            else
            {
                // 检查编号是否重复
                if (!string.IsNullOrEmpty(entity.Code)
                    && Exists(new KeyValuePair<string, object>(BaseUserEntity.FieldCode, entity.Code)
                    , new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)))
                {
                    // 编号已重复
                    StatusCode = Status.ErrorCodeExist.ToString();
                }

                if (entity.IsStaff == 1)
                {
                    var parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BaseStaffEntity.FieldUserName, entity.UserName),
                        new KeyValuePair<string, object>(BaseStaffEntity.FieldDeleted, 0)
                    };
                    if (DbUtil.Exists(DbHelper, BaseStaffEntity.CurrentTableName, parameters))
                    {
                        // 编号已重复
                        StatusCode = Status.ErrorNameExist.ToString();
                    }
                    if (!string.IsNullOrEmpty(entity.Code))
                    {
                        parameters = new List<KeyValuePair<string, object>>
                        {
                            new KeyValuePair<string, object>(BaseStaffEntity.FieldEmployeeNumber, entity.Code),
                            new KeyValuePair<string, object>(BaseStaffEntity.FieldDeleted, 0)
                        };
                        if (DbUtil.Exists(DbHelper, BaseStaffEntity.CurrentTableName, parameters))
                        {
                            // 编号已重复
                            StatusCode = Status.ErrorCodeExist.ToString();
                        }
                    }
                }
            }
        }
        #endregion

        #region public string AddUser(BaseUserEntity entity) 添加用户

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="entity">用户实体</param>
        /// <param name="userLogonEntity"></param>
        /// <returns>主键</returns>
        public string AddUser(BaseUserEntity entity, BaseUserLogonEntity userLogonEntity = null)
        {
            var result = string.Empty;

            BeforeAdd(entity);

            if (StatusCode == Status.OkAdd.ToString())
            {
                //添加用户
                result = AddEntity(entity);

                // 用户登录表里，插入一条记录
                if (userLogonEntity == null)
                {
                    userLogonEntity = new BaseUserLogonEntity();
                }
                userLogonEntity.UserId = result.ToInt();
                //userLogonEntity.CompanyId = entity.CompanyId;
                //把一些默认值读取到，系统的默认值，这样增加用户时可以把系统的默认值带入
                userLogonEntity.ConcurrentUser = BaseSystemInfo.CheckOnline ? 0 : 1;
                userLogonEntity.CheckIpAddress = BaseSystemInfo.CheckIpAddress ? 1 : 0;
                //此处设置密码强度级别
                userLogonEntity.PasswordStrength = SecretUtil.GetUserPassWordRate(userLogonEntity.UserPassword);
                //密码盐
                userLogonEntity.Salt = RandomUtil.GetString(20);
                // 若是系统需要用加密的密码，这里需要加密密码。
                if (BaseSystemInfo.ServerEncryptPassword)
                {
                    userLogonEntity.UserPassword = EncryptUserPassword(userLogonEntity.UserPassword, userLogonEntity.Salt);
                    // 安全通讯密码、交易密码也生成好
                    // userLogonEntity.UserPassword = this.EncryptUserPassword(entity.CommunicationPassword);
                }
                //// 2016.05.21 吉日嘎拉 完善创建信息
                //userLogonEntity.CreateTime = DateTime.Now;
                //userLogonEntity.UpdateTime = DateTime.Now;
                //if (UserInfo != null)
                //{
                //    userLogonEntity.CreateUserId = UserInfo.UserId;
                //    userLogonEntity.CreateBy = UserInfo.RealName;
                //}
                new BaseUserLogonManager(DbHelper, UserInfo).Add(userLogonEntity);

                AfterAdd(entity);
            }

            return result;
        }
        #endregion
    }
}