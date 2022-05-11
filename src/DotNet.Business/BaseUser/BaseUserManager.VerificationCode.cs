﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using DotNet.Util;
    using Model;

    /// <summary>
    /// BaseUserManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///		2015.11.29 版本：1.1 JiRiGaLa	整理参数化代码。
    ///		2013.08.17 版本：1.0 JiRiGaLa	用户登录后才设置验证码、获取验证码等。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.11.29</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        /// <summary>
        /// 设置验证码
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="verificationCode">验证码</param>
        /// <returns>影响行数</returns>
        public int SetVerificationCode(string userId, string verificationCode)
        {
            var result = 0;

            if (!ValidateUtil.IsInt(userId) && UserInfo != null)
            {
                userId = UserInfo.Id.ToString();
            }

            var commandText = "UPDATE " + BaseUserLogonEntity.CurrentTableName
                        + "    SET " + BaseUserLogonEntity.FieldVerificationCode + " = " + DbHelper.GetParameter(BaseUserLogonEntity.FieldVerificationCode)
                        + "  WHERE " + BaseUserLogonEntity.FieldUserId + " = " + DbHelper.GetParameter(BaseUserLogonEntity.FieldUserId);

            var dbParameters = new List<IDbDataParameter>();
            dbParameters.Add(DbHelper.MakeParameter(BaseUserLogonEntity.FieldVerificationCode, verificationCode));
            dbParameters.Add(DbHelper.MakeParameter(BaseUserLogonEntity.FieldUserId, userId));
            result = ExecuteNonQuery(commandText, dbParameters.ToArray());

            return result;
        }

        /// <summary>
        /// 验证，验证码是否正确
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="verificationCode">验证码</param>
        /// <returns>验证正确</returns>
        public bool Verify(string userId, string verificationCode)
        {
            var result = false;

            // 最后一次登录时间
            var commandText = " SELECT COUNT(*)"
                     + " FROM " + BaseUserLogonEntity.CurrentTableName
                     + "  WHERE " + BaseUserLogonEntity.FieldUserId + " = " + DbHelper.GetParameter(BaseUserLogonEntity.FieldUserId)
                     + "        AND " + BaseUserLogonEntity.FieldVerificationCode + " = " + DbHelper.GetParameter(BaseUserLogonEntity.FieldVerificationCode);

            var dbParameters = new List<IDbDataParameter>();
            dbParameters.Add(DbHelper.MakeParameter(BaseUserLogonEntity.FieldUserId, userId));
            dbParameters.Add(DbHelper.MakeParameter(BaseUserLogonEntity.FieldVerificationCode, verificationCode));
            var exist = DbHelper.ExecuteScalar(commandText, dbParameters.ToArray());
            if (exist != null)
            {
                /*
                if (BaseSystemInfo.OnlineLimit <= ))
                {
                    result = true;
                }
                */
                result = int.Parse(exist.ToString()) > 0;
            }

            return result;
        }
    }
}