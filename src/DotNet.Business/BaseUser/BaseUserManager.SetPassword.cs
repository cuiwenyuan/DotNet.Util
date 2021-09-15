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
    ///		2015.12.09 版本：2.1 JiRiGaLa	增加日志记录。
    ///		2014.02.08 版本：2.0 JiRiGaLa	密码加密方式改进。
    ///		2011.10.17 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.12.09</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        /// <summary>
        /// 用户密码加密处理功能
        /// 
        /// 用户的密码到底如何加密，数据库中如何存储用户的密码？
        /// 若是明文方式存储，在管理上会有很多漏洞，虽然调试时不方便，当时加密的密码相对是安全的，
        /// 而且最好是密码是不可逆的，这样安全性更高一些，各种不同的系统，这里适当的处理一下就饿可以了。
        /// </summary>
        /// <param name="password">用户密码</param>
        /// <param name="salt">密码盐</param>
        /// <returns>处理后的密码</returns>
        public virtual string EncryptUserPassword(string password, string salt = null)
        {
            var result = SecretUtil.Md5(password, 32).ToUpper();

            if (!string.IsNullOrEmpty(salt) && (salt.Length == 20))
            {
                result = salt.Substring(6) + result + salt.Substring(6, 10);
                result = SecretUtil.Md5(result, 32).ToUpper();
                result += salt;
                result = SecretUtil.Md5(result, 32).ToUpper();
            }

            return result;
        }

        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="userId">被设置的用户主键</param>
        /// <param name="newPassword">新密码(原始，未加密)</param>
        /// <param name="unlock">解除锁定</param>
        /// <param name="autoAdd">数据缺少自动补充登录信息</param>
        /// <param name="modifyRecord">记录更改</param>
        /// <returns>影响行数</returns>
        public virtual int SetPassword(string userId, string newPassword, bool? unlock = null, bool? autoAdd = null, bool modifyRecord = true)
        {
            var result = 0;

            // 密码强度检查
            /*
            if (BaseSystemInfo.CheckPasswordStrength)
            {
                if (password.Length == 0)
                {
                    this.StatusCode = StatusCode.PasswordCanNotBeNull.ToString();
                    return result;
                }
            }
            */
            var encryptPassword = newPassword;
            var salt = string.Empty;
            // 加密密码
            if (BaseSystemInfo.ServerEncryptPassword)
            {
                salt = RandomUtil.GetString(20);
                encryptPassword = EncryptUserPassword(newPassword, salt);
            }
            // 设置密码字段
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldUserPassword, encryptPassword),

                // 2016-05-20 吉日嘎拉 把修改的痕迹保留起来
                new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldUpdateTime, DateTime.Now)
            };
            if (UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldUpdateUserId, UserInfo.Id));
                parameters.Add(new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldUpdateBy, UserInfo.RealName));
            }

            //需要重新登录才可以，防止正在被人黑中，阻止已经在线上的人
            parameters.Add(new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldOpenId, Guid.NewGuid().ToString("N")));
            if (BaseSystemInfo.ServerEncryptPassword)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldSalt, salt));
            }
            parameters.Add(new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldChangePasswordDate, DateTime.Now));
            if (unlock.HasValue && unlock.Value == true)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldEnabled, 1));
                parameters.Add(new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldLockStartDate, null));
                parameters.Add(new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldLockEndDate, null));
            }
            var userLogOnManager = new BaseUserLogOnManager(DbHelper, UserInfo);
            result = userLogOnManager.SetProperty(new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldId, userId), parameters);
            if (result == 0 && autoAdd.HasValue && autoAdd.Value == true)
            {
                var userLogOnEntity = new BaseUserLogOnEntity
                {
                    Id = userId,
                    ChangePasswordDate = DateTime.Now,
                    UserPassword = encryptPassword,
                    Salt = salt,
                    Enabled = 1,
                    CreateOn = DateTime.Now,
                    ModifiedOn = DateTime.Now
                };
                if (UserInfo != null)
                {
                    userLogOnEntity.CreateUserId = UserInfo.Id;
                    userLogOnEntity.CreateBy = UserInfo.RealName;
                }
                userLogOnManager.AddEntity(userLogOnEntity);
                result = 1;
            }

            // 2015-12-09 吉日嘎拉 增加日志功能、谁什么时候设置了谁的密码？
            if (modifyRecord)
            {
                var record = new BaseModifyRecordEntity
                {
                    TableCode = BaseUserLogOnEntity.TableName.ToUpper(),
                    TableDescription = "用户登录信息表",
                    ColumnCode = BaseUserLogOnEntity.FieldUserPassword,
                    ColumnDescription = "用户密码",
                    RecordKey = userId,
                    NewValue = "设置密码"
                };
                // record.OldValue = "";
                if (UserInfo != null)
                {
                    record.IpAddress = UserInfo.IpAddress;
                    record.CreateUserId = UserInfo.Id;
                    record.CreateOn = DateTime.Now;
                }
                var modifyRecordManager = new BaseModifyRecordManager(UserInfo);
                modifyRecordManager.Add(record, true, false);
            }

            if (result == 1)
            {
                StatusCode = Status.SetPasswordOk.ToString();
                // 调用扩展
                if (BaseSystemInfo.OnInternet && BaseSystemInfo.ServerEncryptPassword)
                {
                    // AfterSetPassword(userId, salt, password);
                }
            }
            else
            {
                // 数据可能被删除
                StatusCode = Status.ErrorDeleted.ToString();
            }

            return result;
        }

        /// <summary>
        /// 批量设置密码
        /// </summary>
        /// <param name="userIds">被设置的员工主键</param>
        /// <param name="password">新密码</param>
        /// <param name="unlock"></param>
        /// <param name="autoAdd"></param>
        /// <returns>影响行数</returns>
        public virtual int BatchSetPassword(string[] userIds, string password, bool? unlock = null, bool? autoAdd = null)
        {
            var result = 0;

            if (userIds != null)
            {
                foreach (var t in userIds)
                {
                    result += SetPassword(t, password, unlock, autoAdd);
                }
            }

            return result;

            /*
            
            int result = 0;
            // 密码强度检查
            
            if (BaseSystemInfo.CheckPasswordStrength)
            {
                if (password.Length == 0)
                {
                    statusCode = StatusCode.PasswordCanNotBeNull.ToString();
                    return result;
                }
            }

            /*

            string encryptPassword = password;
            // 加密密码
            if (BaseSystemInfo.ServerEncryptPassword)
            {
                encryptPassword = this.EncryptUserPassword(password);
            }

            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldUserPassword, encryptPassword));
            // 需要重新登录才可以，防止正在被人黑中，阻止已经在线上的人
            parameters.Add(new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldOpenId, Guid.NewGuid().ToString("N")));
            parameters.Add(new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldChangePasswordDate, DateTime.Now));
            // 设置密码字段
            result += new BaseUserLogOnManager(this.DbHelper, this.UserInfo).SetProperty(userIds, parameters);

            if (result > 0)
            {
                this.StatusCode = Status.SetPasswordOK.ToString();
                // 调用扩展
                AfterBatchSetPassword(userIds, password);
            }
            else
            {
                // 数据可能被删除
                this.StatusCode = Status.ErrorDeleted.ToString();
            }

            return result;

            */
        }
    }
}