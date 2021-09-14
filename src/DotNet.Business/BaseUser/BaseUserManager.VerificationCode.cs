//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
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
    ///		<name>JiRiGaLa</name>
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

            if (string.IsNullOrEmpty(userId) && UserInfo != null)
            {
                userId = UserInfo.Id;
            }

            var commandText = "UPDATE " + BaseUserLogOnEntity.TableName
                        + "    SET " + BaseUserLogOnEntity.FieldVerificationCode + " = " + DbHelper.GetParameter(BaseUserLogOnEntity.FieldVerificationCode)
                        + "  WHERE " + BaseUserLogOnEntity.FieldId + " = " + DbHelper.GetParameter(BaseUserLogOnEntity.FieldId);

            var dbParameters = new List<IDbDataParameter>();
            dbParameters.Add(DbHelper.MakeParameter(BaseUserLogOnEntity.FieldVerificationCode, verificationCode));
            dbParameters.Add(DbHelper.MakeParameter(BaseUserLogOnEntity.FieldId, userId));
            result = DbHelper.ExecuteNonQuery(commandText, dbParameters.ToArray());

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
                     + " FROM " + BaseUserLogOnEntity.TableName
                     + "  WHERE " + BaseUserLogOnEntity.FieldId + " = " + DbHelper.GetParameter(BaseUserLogOnEntity.FieldId)
                     + "        AND " + BaseUserLogOnEntity.FieldVerificationCode + " = " + DbHelper.GetParameter(BaseUserLogOnEntity.FieldVerificationCode);

            var dbParameters = new List<IDbDataParameter>();
            dbParameters.Add(DbHelper.MakeParameter(BaseUserLogOnEntity.FieldId, userId));
            dbParameters.Add(DbHelper.MakeParameter(BaseUserLogOnEntity.FieldVerificationCode, verificationCode));
            var exist = DbHelper.ExecuteScalar(commandText, dbParameters.ToArray());
            if (exist != null)
            {
                /*
                if (BaseSystemInfo.OnLineLimit <= ))
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