//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2026, DotNet.
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
    ///		<name>Troy.Cui</name>
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

            if (!salt.IsNullOrEmpty() && (salt.Length == 20))
            {
                result = salt.Substring(6) + result + salt.Substring(6, 10);
                result = SecretUtil.Md5(result, 32).ToUpper();
                result += salt;
                result = SecretUtil.Md5(result, 32).ToUpper();
            }

            return result;
        }

        /// <summary>
        /// 校验用户输入口令与存储哈希是否匹配（R9-1 双路径）。
        /// 新格式（pbkdf2$ 前缀）走 PBKDF2 校验；老格式（MD5，有盐/无盐）走 EncryptUserPassword 兼容校验。
        /// </summary>
        /// <param name="enteredPassword">用户输入的原始口令</param>
        /// <param name="storedHash">存储的口令哈希（UserPassword 字段）</param>
        /// <param name="storedSalt">存储的盐（Salt 字段，老格式使用；新格式盐已内嵌，可传 null/空）</param>
        /// <param name="isLegacyFormat">传出：存储哈希是否为老格式（用于触发惰性升级）</param>
        /// <returns>匹配返回 true</returns>
        public virtual bool VerifyUserPassword(string enteredPassword, string storedHash, string storedSalt, out bool isLegacyFormat)
        {
            isLegacyFormat = false;
            if (storedHash.IsNullOrEmpty())
            {
                // 库里无密码：仅当输入也为空才算匹配
                return enteredPassword.IsNullOrEmpty();
            }
            // 新格式：PBKDF2（盐内嵌）
            if (storedHash.StartsWith(SecretUtil.PasswordHashPrefix, StringComparison.Ordinal))
            {
                return SecretUtil.VerifyPassword(enteredPassword, storedHash);
            }
            // 老格式（MD5，有盐或无盐），保持向后兼容
            isLegacyFormat = true;
            var legacy = EncryptUserPassword(enteredPassword, storedSalt);
            return string.Equals(legacy, storedHash, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 惰性升级老格式口令哈希为 PBKDF2（R9-1）。best-effort：升级失败不阻断登录。
        /// </summary>
        private void UpgradeUserPasswordHash(int userId, string plainPassword)
        {
            try
            {
                var newHash = SecretUtil.HashPassword(plainPassword);
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserLogonEntity.FieldUserPassword, newHash),
                    new KeyValuePair<string, object>(BaseUserLogonEntity.FieldSalt, string.Empty),
                    new KeyValuePair<string, object>(BaseUserLogonEntity.FieldChangePasswordTime, DateTime.Now)
                };
                new BaseUserLogonManager(DbHelper, UserInfo).Update(
                    new KeyValuePair<string, object>(BaseUserLogonEntity.FieldUserId, userId), parameters);
            }
            catch
            {
                // 惰性升级失败不应阻断登录；下次成功登录会再次尝试
            }
        }

        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="userId">被设置的用户主键</param>
        /// <param name="newPassword">新密码(原始，未加密)</param>
        /// <param name="unlock">解除锁定</param>
        /// <param name="autoAdd">数据缺少自动补充登录信息</param>
        /// <param name="changeLog">记录更改</param>
        /// <returns>影响行数</returns>
        public virtual int SetPassword(string userId, string newPassword, bool? unlock = null, bool? autoAdd = null, bool changeLog = true)
        {
            return SetPassword(userId.ToInt(), newPassword, unlock, autoAdd, changeLog);
        }
        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="userId">被设置的用户主键</param>
        /// <param name="newPassword">新密码(原始，未加密)</param>
        /// <param name="unlock">解除锁定</param>
        /// <param name="autoAdd">数据缺少自动补充登录信息</param>
        /// <param name="changeLog">记录更改</param>
        /// <returns>影响行数</returns>
        public virtual int SetPassword(int userId, string newPassword, bool? unlock = null, bool? autoAdd = null, bool changeLog = true)
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
            // 加密密码：R9-1 改用加盐 PBKDF2，盐内嵌于哈希字符串，不再单独写 Salt 列
            // 口令为空时与原 Md5 行为一致（返回空），不抛异常
            if (BaseSystemInfo.ServerEncryptPassword && !newPassword.IsNullOrEmpty())
            {
                encryptPassword = SecretUtil.HashPassword(newPassword);
            }
            // 设置密码字段
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserLogonEntity.FieldUserPassword, encryptPassword),

                // 2016-05-20 吉日嘎拉 把修改的痕迹保留起来
                new KeyValuePair<string, object>(BaseUserLogonEntity.FieldUpdateTime, DateTime.Now)
            };
            if (UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldUpdateUserId, UserInfo.UserId));
                parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldUpdateBy, UserInfo.RealName));
            }

            //需要重新登录才可以，防止正在被人黑中，阻止已经在线上的人
            parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldOpenId, Guid.NewGuid().ToString("N")));
            if (BaseSystemInfo.ServerEncryptPassword)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldSalt, salt));
            }
            parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldChangePasswordTime, DateTime.Now));
            if (unlock.HasValue && unlock.Value)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldEnabled, 1));
                parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldLockStartTime, null));
                parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldLockEndTime, null));
            }
            var userLogonManager = new BaseUserLogonManager(DbHelper, UserInfo);
            result = userLogonManager.Update(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldUserId, userId), parameters);
            if (result == 0 && autoAdd.HasValue && autoAdd.Value)
            {
                var userLogonEntity = new BaseUserLogonEntity
                {
                    UserId = userId,
                    ChangePasswordTime = DateTime.Now,
                    UserPassword = encryptPassword,
                    Salt = salt,
                    Enabled = 1,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                if (UserInfo != null)
                {
                    userLogonEntity.CreateUserId = UserInfo.UserId;
                    userLogonEntity.CreateBy = UserInfo.RealName;
                }
                userLogonManager.AddEntity(userLogonEntity);
                result = 1;
            }

            // 2015-12-09 吉日嘎拉 增加日志功能、谁什么时候设置了谁的密码？
            if (changeLog)
            {
                var baseChangeLogEntity = new BaseChangeLogEntity
                {
                    TableName = BaseUserLogonEntity.CurrentTableName,
                    TableDescription = CurrentTableDescription,
                    ColumnName = BaseUserLogonEntity.FieldUserPassword,
                    ColumnDescription = "用户密码",
                    RecordKey = userId.ToString(),
                    NewValue = "设置密码"
                };

                new BaseChangeLogManager(UserInfo).Add(baseChangeLogEntity, true, false);
            }

            if (result == 1)
            {
                Status = Status.SetPasswordOk;
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
                Status = Status.ErrorDeleted;
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
                    result += SetPassword(t.ToInt(), password, unlock, autoAdd);
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
                    status = StatusCode.PasswordCanNotBeNull.ToString();
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
            parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldUserPassword, encryptPassword));
            // 需要重新登录才可以，防止正在被人黑中，阻止已经在线上的人
            parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldOpenId, Guid.NewGuid().ToString("N")));
            parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldChangePasswordTime, DateTime.Now));
            // 设置密码字段
            result += new BaseUserLogonManager(this.DbHelper, this.UserInfo).Update(userIds, parameters);

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