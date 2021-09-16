﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;

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
    ///		2016.05.20 版本：2.2 JiRiGaLa	修改日期等进行完善。
    ///		2015.12.09 版本：2.1 JiRiGaLa	修改用户密码，多一个userId参数，增加修改日志。
    ///		2015.01.19 版本：2.0 JiRiGaLa	用户修改密码的日志。
    ///		2011.10.17 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.05.20</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="userId">用户主键、方便外部系统调用，若能传递参数过来</param>
        /// <param name="oldPassword">原密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>影响行数</returns>
        public virtual BaseUserInfo ChangePassword(string userId, string oldPassword, string newPassword)
        {
            #if (DEBUG)
                int milliStart = Environment.TickCount;
            #endif

            var encryptOldPassword = oldPassword;
            var encryptNewPassword = newPassword;

            BaseUserInfo userInfo = null;
            // 密码强度检查
            if (BaseSystemInfo.CheckPasswordStrength)
            {
                if (string.IsNullOrEmpty(newPassword))
                {
                    StatusCode = Status.PasswordCanNotBeNull.ToString();
                    return userInfo;
                }
            }
            // 判断输入原始密码是否正确
            var entity = new BaseUserLogOnManager(DbHelper, UserInfo).GetEntity(UserInfo.Id);
            if (entity.UserPassword == null)
            {
                entity.UserPassword = string.Empty;
            }

            // 加密密码
            if (BaseSystemInfo.ServerEncryptPassword)
            {
                encryptOldPassword = EncryptUserPassword(oldPassword, entity.Salt);
            }

            // 密码错误
            if (!entity.UserPassword.Equals(encryptOldPassword, StringComparison.CurrentCultureIgnoreCase))
            {
                StatusCode = Status.OldPasswordError.ToString();
                return userInfo;
            }
            // 对比是否最近2次用过这个密码
            if (BaseSystemInfo.CheckPasswordStrength)
            {
                /*
                int i = 0;
                BaseParameterManager manager = new BaseParameterManager(this.DbHelper, this.UserInfo);
                var dt = manager.GetDataTableParameterCode("User", this.UserInfo.Id, "Password");
                foreach (DataRow dr in dt.Rows)
                {
                    string parameter = dr[BaseParameterEntity.FieldParameterContent].ToString();
                    if (parameter.Equals(newPassword))
                    {
                        this.StatusCode = Status.PasswordCanNotBeRepeat.ToString();
                        return userInfo;
                    }
                    i++;
                    {
                        // 判断连续2个密码就是可以了
                        if (i > 2)
                        {
                            break;
                        }
                    }
                }
                */
            }
            
            // 更改密码，同时修改密码的修改日期，这里需要兼容多数据库
            var salt = string.Empty;
            if (BaseSystemInfo.ServerEncryptPassword)
            {
                salt = RandomUtil.GetString(20);
                encryptNewPassword = EncryptUserPassword(newPassword, salt);
            }
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(BaseUserLogOnEntity.TableName);
            if (BaseSystemInfo.ServerEncryptPassword)
            {
                sqlBuilder.SetValue(BaseUserLogOnEntity.FieldSalt, salt);
            }
            // 宋彪：此处增加更新密码强度级别
            sqlBuilder.SetValue(BaseUserLogOnEntity.FieldPasswordStrength, SecretUtil.GetUserPassWordRate(newPassword));
            sqlBuilder.SetValue(BaseUserLogOnEntity.FieldUserPassword, encryptNewPassword);
            // 2015-08-04 吉日嘎拉 修改了密码后,把需要修改密码字段设置为 0
            sqlBuilder.SetValue(BaseUserLogOnEntity.FieldNeedModifyPassword, 0);
            sqlBuilder.SetDbNow(BaseUserLogOnEntity.FieldChangePasswordDate);
            sqlBuilder.SetDbNow(BaseUserLogOnEntity.FieldUpdateTime);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseUserLogOnEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseUserLogOnEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetWhere(BaseUserLogOnEntity.FieldId, userId);
            var result = sqlBuilder.EndUpdate();

            if (result == 1)
            {
                // 2015-12-09 吉日嘎拉 确认已经记录了修改密码日志
                // BaseLoginLogManager.AddLog(this.UserInfo, Status.ChangePassword.ToDescription()); 

                // 2015-12-09 吉日嘎拉 增加日志功能、谁什么时候设置了谁的密码？
                var record = new BaseModifyRecordEntity();
                record.TableCode = BaseUserLogOnEntity.TableName.ToUpper();
                record.TableDescription = "用户登录信息表";
                record.ColumnCode = BaseUserLogOnEntity.FieldUserPassword;
                record.ColumnDescription = "用户密码";
                record.RecordKey = userId;
                record.NewValue = "修改密码";
                // record.OldValue = "";
                if (UserInfo != null)
                {
                    record.IpAddress = UserInfo.IpAddress;
                    record.CreateUserId = UserInfo.Id;
                    record.CreateOn = DateTime.Now;
                }
                var modifyRecordManager = new BaseModifyRecordManager(UserInfo);
                modifyRecordManager.Add(record, true, false);

                /*
                // 若是强类型密码检查，那就保存密码修改历史，防止最近2-3次的密码相同的功能实现。
                if (BaseSystemInfo.CheckPasswordStrength)
                {
                    BaseParameterManager parameterManager = new BaseParameterManager(this.DbHelper, this.UserInfo);
                    BaseParameterEntity parameterEntity = new BaseParameterEntity();
                    parameterEntity.CategoryCode = "User";
                    parameterEntity.ParameterId = this.UserInfo.Id;
                    parameterEntity.ParameterCode = "Password";
                    parameterEntity.ParameterContent = newPassword;
                    parameterEntity.DeletionStateCode = 0;
                    parameterEntity.Enabled = true;
                    parameterEntity.Worked = true;
                    parameterManager.AddEntity(parameterEntity);
                }
                */
                
                userInfo = LogOnByOpenId(UserInfo.OpenId, UserInfo.SystemCode).UserInfo;
                // 同步处理其他系统的密码修改动作
                if (BaseSystemInfo.ServerEncryptPassword)
                {
                    // AfterChangePassword(this.UserInfo.Id, salt, oldPassword, newPassword);
                }
                // 修改密码成功，写入状态
                StatusCode = Status.ChangePasswordOk.ToString();
            }
            else
            {
                // 数据可能被删除
                StatusCode = Status.ErrorDeleted.ToString();
            }

            return userInfo;
        }
    }
}