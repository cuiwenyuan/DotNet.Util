//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;

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
    ///     2020.12.08 版本：1.5 Troy.Cui 使用CacheUtil缓存
    ///		2016.05.17 版本：3.6 JiRiGaLa 实现操作授权码功能、只能消费一次的。
    ///		2016.01.28 版本：3.5 JiRiGaLa 按唯一用户名登录时，可以通过缓存登录的功能实现。
    ///		2015.07.13 版本：3.3 JiRiGaLa 登录日志加上日志、日志无法正常写入也需要系统能正常登录。
    ///		2015.04.27 版本：3.2 JiRiGaLa 支持多系统登录。
    ///		2014.08.05 版本：3.1 JiRiGaLa 登录优化。
    ///		2014.03.25 版本：3.0 JiRiGaLa CheckIsAdministrator 不是所有系统都需要验证是否超级管理员，去掉效率会更高，特别是针对登录接口可以优化了。
    ///		2013.10.20 版本：2.0 JiRiGaLa 集成K8物流系统的登录功能。
    ///		2011.10.17 版本：1.0 JiRiGaLa 主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.01.28</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        /// <summary>
        /// 是否从角色里判断是否超级管理员
        /// </summary>
        public bool CheckIsAdministrator = false;

        /// <summary>
        /// 是否根据OpenId登录
        /// </summary>
        public bool IsLogonByOpenId = false;

        /// <summary>
        /// 当前子系统是否加密了用户密码
        /// </summary>
        public bool SystemEncryptPassword = true;

        /// <summary>
        /// 获取用户的权限
        /// </summary>
        public bool GetUserPermission = true;

        /// <summary>
        /// 登录验证的表对应的表
        /// </summary>
        public string UserLogonTable = "BaseUserLogon";

        /// <summary>
        /// 对应的缓存服务区分
        /// </summary>
        public string CachingSystemCode = "";

        #region GetAuthorizationCode
        /// <summary>
        /// 获取登录操作的验证码
        /// code作为换取access_token的票据，每次用户授权带上的code将不一样，code只能使用一次，5分钟未被使用自动过期。 
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <returns>操作码</returns>
        public static BaseResult GetAuthorizationCode(BaseUserInfo userInfo)
        {
            var result = new BaseResult();

            if (ServiceUtil.VerifySignature(userInfo))
            {
                // 产生一个授权码
                var authorizationCode = Guid.NewGuid().ToString("N");
                var key = "code:" + authorizationCode;
                // 缓存五分钟
                var cacheTime = TimeSpan.FromMilliseconds(300000);
                CacheUtil.Set(key, userInfo.OpenId, cacheTime);

                result.Result = authorizationCode;
                result.Status = true;
                result.StatusCode = Status.Ok.ToString();
                result.StatusMessage = Status.Ok.ToDescription();
                result.CreateSignature(userInfo);
            }

            return result;
        }
        #endregion

        #region VerifyAuthorizationCode
        /// <summary>
        /// 验证授权码
        /// 用掉一次后，一定要消费掉，确保只能用一次。
        /// </summary>
        /// <param name="userInfo">当前用户信息</param>
        /// <param name="code">授权码</param>
        /// <param name="openId">用户唯一识别码</param>
        /// <returns>验证成功</returns>
        public static bool VerifyAuthorizationCode(BaseUserInfo userInfo, string code, out string openId)
        {
            var result = false;
            openId = string.Empty;

            if (userInfo != null && !ServiceUtil.VerifySignature(userInfo))
            {
                return result;
            }

            // 2016-03-03 吉日嘎拉 让缓存早点儿失效
            var key = "code:" + code;
            openId = CacheUtil.Get<string>(key);
            if (!string.IsNullOrEmpty(openId))
            {
                result = true;
                if (userInfo != null && !string.IsNullOrEmpty(userInfo.OpenId))
                {
                    result = userInfo.OpenId.Equals(openId);
                }
            }
            CacheUtil.Remove(key);

            return result;
        }
        #endregion

        #region CallLimit

        /// <summary>
        /// 防止暴力破解
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="ipAddress">ip地址</param>
        /// <returns>登录情况</returns>
        public UserLogonResult CallLimit(string userName, string password, string ipAddress)
        {
            UserLogonResult result = null;

            // 2016-04-13 吉日嘎拉 IP登录限制，防止暴力破解
            if (!string.IsNullOrEmpty(ipAddress))
            {
            }

            if (!string.IsNullOrEmpty(userName))
            {
                ////Troy.Cui 2016.12.28
                //if (BaseSystemInfo.RedisEnabled)
                //{
                //    // 2016-04-13 吉日嘎拉 用户名登录限制，防止暴力破解
                //    if (PooledRedisHelper.CallLimit("u:" + userName, 1, 20))
                //    {
                //        result = new UserLogonResult();
                //        result.StatusCode = Status.UserNameLimit.ToString();
                //        result.StatusMessage = Status.UserNameLimit.ToDescription();
                //        return result;
                //    }
                //}
            }

            if (!string.IsNullOrEmpty(password))
            {
                ////Troy.Cui 2016.12.28
                //if (BaseSystemInfo.RedisEnabled)
                //{
                //    // 2016-04-13 吉日嘎拉 密码登录限制，防止暴力破解
                //    if (PooledRedisHelper.CallLimit("p:" + password, 1, 20))
                //    {
                //        result = new UserLogonResult();
                //        result.StatusCode = Status.PasswordLimit.ToString();
                //        result.StatusMessage = Status.PasswordLimit.ToDescription();
                //        return result;
                //    }
                //}
            }

            return result;
        }
        #endregion

        #region public UserLogonResult LogonByNickName(string nickName, string password, string openId = null, string ipAddress = null, string macAddress = null, bool checkUserPassword = true) 进行登录操作
        /// <summary>
        /// 进行登录操作
        /// </summary>
        /// <param name="nickName">昵称</param>
        /// <param name="password">密码</param>
        /// <param name="openId">单点登录标识</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="macAddress">MAC地址</param>
        /// <param name="computerName"></param>
        /// <param name="checkUserPassword">是否要检查用户密码</param>
        /// <param name="getOpenIdOnly"></param>
        /// <param name="checkMacAddress"></param>
        /// <param name="sourceType"></param>
        /// <param name="targetApplication"></param>
        /// <param name="targetIp"></param>
        /// <returns>用户信息</returns>
        public UserLogonResult LogonByNickName(string nickName, string password, string openId = null, string systemCode = null, string ipAddress = null, string macAddress = null, string computerName = null, bool checkUserPassword = true, bool getOpenIdOnly = false, bool checkMacAddress = true, string sourceType = null, string targetApplication = null, string targetIp = null)
        {
            var result = new UserLogonResult();
            //int errorMark = 0;

            var ipAddressName = string.Empty;

            try
            {
                if (BaseSystemInfo.OnInternet && !string.IsNullOrEmpty(ipAddress))
                {
                    //errorMark = 1;
                    ipAddressName = IpUtil.GetInstance().FindName(ipAddress);
                }

                // 大多人还是看小写比较敏感
                if (!string.IsNullOrWhiteSpace(nickName))
                {
                    //MSSQL不要做任何改变为好，Troy.Cui 2016-08-12
                    //nickName = nickName.ToLower();
                }
                nickName = DbHelper.SqlSafe(nickName);

                if (UserInfo != null)
                {
                    if (string.IsNullOrEmpty(ipAddress))
                    {
                        ipAddress = UserInfo.IpAddress;
                    }
                    // 得到MAC地址，否则会导致后面的MAC地址判断无效 赵秉杰 2012-09-02
                    if (string.IsNullOrEmpty(macAddress))
                    {
                        macAddress = UserInfo.MacAddress;
                    }
                }

                // 04. 默认为用户没有找到状态，查找用户
                // 这是为了达到安全要求，不能提示用户未找到，那容易让别人猜测到帐户
                if (BaseSystemInfo.CheckPasswordStrength)
                {
                    result.Status = Status.ErrorLogon;
                    result.StatusCode = Status.ErrorLogon.ToString();
                }
                else
                {
                    result.Status = Status.UserNotFound;
                    result.StatusCode = Status.UserNotFound.ToString();
                }
                result.StatusMessage = GetStateMessage(result.StatusCode);
                var sb = Pool.StringBuilder.Get();
                var dbParameters = new List<IDbDataParameter>();
                sb.Append("SELECT * "
                          + " FROM " + BaseUserEntity.CurrentTableName
                         + " WHERE " + BaseUserEntity.FieldNickName + " = " + DbHelper.GetParameter(BaseUserEntity.FieldNickName)
                                 + " AND " + BaseUserEntity.FieldDeleted + " = " + DbHelper.GetParameter(BaseUserEntity.FieldDeleted));
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldNickName, nickName));
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldDeleted, 0));
                //errorMark = 2;
                var dt = DbHelper.Fill(sb.Put(), dbParameters.ToArray());
                // 若是有多条数据返回，把设置为无效的数据先过滤掉，防止数据有重复
                if (dt != null && dt.Rows.Count > 1)
                {
                    dt = BaseUtil.SetFilter(dt, BaseUserEntity.FieldEnabled, "1");
                }

                if (dt != null && dt.Rows.Count == 1)
                {
                    // 进行登录校验
                    BaseUserEntity userEntity = null;
                    //errorMark = 3;
                    userEntity = BaseEntity.Create<BaseUserEntity>(dt.Rows[0]);
                    //errorMark = 4;
                    result = LogonByEntity(userEntity, password, openId, systemCode, ipAddress, ipAddressName, macAddress, computerName, checkUserPassword, false, checkMacAddress, sourceType, targetApplication, targetIp);
                }
                else if (dt != null && dt.Rows.Count > 1)
                {
                    // 用户命重复了
                    //errorMark = 5;
                    BaseLogonLogManager.AddLog(systemCode, string.Empty, string.Empty, nickName, ipAddress, ipAddressName, macAddress, Status.UserDuplicate.ToDescription());
                    result.Status = Status.UserDuplicate;
                    result.StatusCode = Status.UserDuplicate.ToString();
                }
                else
                {
                    //errorMark = 6;
                    // 用户没找到
                    BaseLogonLogManager.AddLog(systemCode, string.Empty, string.Empty, nickName, ipAddress, ipAddressName, macAddress, Status.UserNotFound.ToDescription());
                    result.Status = Status.UserNotFound;
                    result.StatusCode = Status.UserNotFound.ToString();
                }
                result.StatusMessage = GetStateMessage(result.StatusCode);
            }
            catch (Exception ex)
            {
                result.Status = Status.SystemCodeError;
                result.StatusCode = Status.SystemCodeError.ToString();
                result.StatusMessage = ex.Message;

                //string writeMessage = "BaseUserManager.LogonByNickName:发生时间:" + DateTime.Now
                //    + System.Environment.NewLine + "errorMark = " + errorMark.ToString()
                //    + System.Environment.NewLine + "Message:" + ex.Message
                //    + System.Environment.NewLine + "Source:" + ex.Source
                //    + System.Environment.NewLine + "StackTrace:" + ex.StackTrace
                //    + System.Environment.NewLine + "TargetSite:" + ex.TargetSite
                //    + System.Environment.NewLine;

                //LogUtil.WriteLog(writeMessage, "Exception");
            }

            return result;
        }
        #endregion

        #region public UserLogonResult LogonByNickNameByCache(string nickName, string password, string openId = null, string ipAddress = null, string macAddress = null, bool checkUserPassword = true) 进行登录操作
        /// <summary>
        /// 唯一用户名登陆登录操作 缓存
        /// </summary>
        /// <param name="nickName">昵称</param>
        /// <param name="password">密码</param>
        /// <param name="openId">单点登录标识</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="macAddress">MAC地址</param>
        /// <param name="computerName"></param>
        /// <param name="checkUserPassword">是否要检查用户密码</param>
        /// <param name="getOpenIdOnly"></param>
        /// <param name="checkMacAddress"></param>
        /// <param name="sourceType"></param>
        /// <returns>用户信息</returns>
        public UserLogonResult LogonByNickNameByCache(string nickName, string password, string openId = null, string systemCode = null, string ipAddress = null, string macAddress = null, string computerName = null, bool checkUserPassword = true, bool getOpenIdOnly = false, bool checkMacAddress = true, string sourceType = null)
        {
            var result = new UserLogonResult();
            var errorMark = 0;

            var ipAddressName = string.Empty;

            try
            {
                if (BaseSystemInfo.OnInternet && !string.IsNullOrEmpty(ipAddress))
                {
                    errorMark = 1;
                    ipAddressName = IpUtil.GetInstance().FindName(ipAddress);
                }

                // 大多人还是看小写比较敏感
                if (!string.IsNullOrWhiteSpace(nickName))
                {
                    //MSSQL不要做任何改变为好，Troy.Cui 2016-08-12
                    //nickName = nickName.ToLower();
                }
                nickName = DbHelper.SqlSafe(nickName);

                if (UserInfo != null)
                {
                    if (string.IsNullOrEmpty(ipAddress))
                    {
                        ipAddress = UserInfo.IpAddress;
                    }
                    // 得到MAC地址，否则会导致后面的MAC地址判断无效 赵秉杰 2012-09-02
                    if (string.IsNullOrEmpty(macAddress))
                    {
                        macAddress = UserInfo.MacAddress;
                    }
                }

                // 04. 默认为用户没有找到状态，查找用户
                // 这是为了达到安全要求，不能提示用户未找到，那容易让别人猜测到帐户
                if (BaseSystemInfo.CheckPasswordStrength)
                {
                    result.Status = Status.ErrorLogon;
                    result.StatusCode = Status.ErrorLogon.ToString();
                }
                else
                {
                    result.Status = Status.UserNotFound;
                    result.StatusCode = Status.UserNotFound.ToString();
                }
                result.StatusMessage = GetStateMessage(result.StatusCode);

                var userEntity = GetEntityByNickNameByCache(nickName);
                if (userEntity != null)
                {
                    // 进行登录校验
                    errorMark = 4;
                    result = LogonByEntity(userEntity, password, openId, systemCode, ipAddress, ipAddressName, macAddress, computerName, checkUserPassword, false, checkMacAddress);
                }
                else
                {
                    errorMark = 6;
                    // 用户没找到
                    BaseLogonLogManager.AddLog(systemCode, string.Empty, string.Empty, nickName, ipAddress, ipAddressName, macAddress, Status.UserNotFound.ToDescription());
                    result.Status = Status.UserNotFound;
                    result.StatusCode = Status.UserNotFound.ToString();
                }

                result.StatusMessage = GetStateMessage(result.StatusCode);
            }
            catch (Exception ex)
            {
                var writeMessage = "BaseUserManager.LogonByNickNameByCache:发生时间:" + DateTime.Now
                    + Environment.NewLine + "errorMark = " + errorMark
                    + Environment.NewLine + "Message:" + ex.Message
                    + Environment.NewLine + "Source:" + ex.Source
                    + Environment.NewLine + "StackTrace:" + ex.StackTrace
                    + Environment.NewLine + "TargetSite:" + ex.TargetSite
                    + Environment.NewLine;

                LogUtil.WriteLog(writeMessage, "Exception");
            }

            return result;
        }
        #endregion

        #region LogonByCompanyByCode
        /// <summary>
        /// 按公司编号、用户编号进行登录
        /// </summary>
        /// <param name="companyCode">公司编号</param>
        /// <param name="userCode">用户编号</param>
        /// <param name="userEntity">用户实体</param>
        /// <returns>登录信息</returns>
        public UserLogonResult LogonByCompanyByCode(string companyCode, string userCode, out BaseUserEntity userEntity)
        {
            var result = new UserLogonResult();

            var errorMark = 0;
            userEntity = null;
            try
            {
                companyCode = SecretUtil.SqlSafe(companyCode);
                userCode = SecretUtil.SqlSafe(userCode);

                // 设备在手里，认为是安全的，不是人人都能有设备在手里
                result.Status = Status.UserNotFound;
                result.StatusCode = Status.UserNotFound.ToString();
                result.StatusMessage = GetStateMessage(result.StatusCode);
                if (string.IsNullOrEmpty(companyCode) || string.IsNullOrEmpty(userCode))
                {
                    return result;
                }
                // 大多人还是看小写比较敏感
                if (!string.IsNullOrWhiteSpace(userCode))
                {
                    // 2015-07-09 吉日嘎拉 这里不能转换成小写
                    // userCode = userCode.ToLower();
                }

                // 2015-11-11 吉日嘎拉 查询数据的同时把数据进行了缓存，提高下一个登录时的效率
                var companyId = string.Empty;

                errorMark = 1;
                companyId = BaseOrganizationManager.GetIdByCodeByCache(companyCode);
                if (string.IsNullOrEmpty(companyId))
                {
                    result.Status = Status.CompanyNotFound;
                    result.StatusCode = Status.CompanyNotFound.ToString();
                    result.StatusMessage = GetStateMessage(result.StatusCode);
                    return result;
                }
                var sb = Pool.StringBuilder.Get();
                var dbParameters = new List<IDbDataParameter>();
                sb.Append("SELECT * FROM " + BaseUserEntity.CurrentTableName
                         + " WHERE " + BaseUserEntity.FieldDeleted + " = " + DbHelper.GetParameter(BaseUserEntity.FieldDeleted)
                         + " AND " + BaseUserEntity.FieldCompanyId + " = " + DbHelper.GetParameter(BaseUserEntity.FieldCompanyId)
                         + " AND " + BaseUserEntity.FieldCode + " = " + DbHelper.GetParameter(BaseUserEntity.FieldCode));

                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldDeleted, 0));
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldCompanyId, companyId));
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldCode, userCode));

                errorMark = 2;
                var dt = DbHelper.Fill(sb.Put(), dbParameters.ToArray());
                // 若是有多条数据返回，把设置为无效的数据先过滤掉，防止数据有重复
                if (dt != null && dt.Rows.Count > 1)
                {
                    dt = BaseUtil.SetFilter(dt, BaseUserEntity.FieldEnabled, "1");
                }
                if (dt != null && dt.Rows.Count > 1)
                {
                    result.Status = Status.UserDuplicate;
                    result.StatusCode = Status.UserDuplicate.ToString();
                    result.StatusMessage = GetStateMessage(result.StatusCode);
                    return result;
                }
                if (dt != null && dt.Rows.Count == 1)
                {
                    errorMark = 3;
                    userEntity = BaseEntity.Create<BaseUserEntity>(dt);
                }
            }
            catch (Exception ex)
            {
                var writeMessage = "BaseUserManager.LogonByCompanyByCode:发生时间:" + DateTime.Now
                    + Environment.NewLine + "errorMark = " + errorMark
                    + Environment.NewLine + "Message:" + ex.Message
                    + Environment.NewLine + "Source:" + ex.Source
                    + Environment.NewLine + "StackTrace:" + ex.StackTrace
                    + Environment.NewLine + "TargetSite:" + ex.TargetSite
                    + Environment.NewLine;

                LogUtil.WriteLog(writeMessage, "Exception");
            }

            return result;
        }
        #endregion

        #region LogonByCompany
        ///  <summary>
        /// 公司名称，用户名，密码登录
        /// 2016-09-18 吉日嘎拉 按网点的id进行登录
        ///  </summary>
        ///  <param name="companyName"></param>
        ///  <param name="userName"></param>
        ///  <param name="password"></param>
        ///  <param name="openId"></param>
        ///  <param name="systemCode">系统编码</param>
        ///  <param name="ipAddress"></param>
        ///  <param name="macAddress"></param>
        ///  <param name="computerName"></param>
        ///  <param name="checkUserPassword"></param>
        ///  <param name="validateUserOnly"></param>
        ///  <param name="checkMacAddress"></param>
        /// <returns></returns>
        public UserLogonResult LogonByCompany(string companyName, string userName, string password, string openId = null, string systemCode = null, string ipAddress = null, string macAddress = null, string computerName = null, bool checkUserPassword = true, bool validateUserOnly = false, bool checkMacAddress = true)
        {
            var ipAddressName = string.Empty;
            if (BaseSystemInfo.OnInternet && !string.IsNullOrEmpty(ipAddress))
            {
                ipAddressName = IpUtil.GetInstance().FindName(ipAddress);
            }

            var result = new UserLogonResult
            {
                Status = Status.Error,
                StatusCode = "Error",
                StatusMessage = "请用唯一用户名登录、若不知道唯一用户名、请向公司的管理员索取。"
            };

            if (string.IsNullOrEmpty(companyName) || string.IsNullOrEmpty(userName))
            {
                return result;
            }
            // 大多人还是看小写比较敏感
            if (!string.IsNullOrWhiteSpace(userName))
            {
                //MSSQL不要做任何改变为好，Troy.Cui 2016-08-12
                //userName = userName.ToLower();
            }
            // 这里先同步数据？然后再登录，这样提高数据同步效果
            companyName = SecretUtil.SqlSafe(companyName);
            userName = SecretUtil.SqlSafe(userName);
            if (UserInfo != null)
            {
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = UserInfo.IpAddress;
                }
                // 得到MAC地址，否则会导致后面的MAC地址判断无效 赵秉杰 2012-09-02
                if (string.IsNullOrEmpty(macAddress))
                {
                    macAddress = UserInfo.MacAddress;
                }
            }

            if (BaseSystemInfo.CheckPasswordStrength)
            {
                result.Status = Status.ErrorLogon;
                result.StatusCode = Status.ErrorLogon.ToString();
            }
            else
            {
                result.Status = Status.UserNotFound;
                result.StatusCode = Status.UserNotFound.ToString();
            }
            result.StatusMessage = GetStateMessage(result.StatusCode);

            var dbParameters = new List<IDbDataParameter>();
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT * FROM " + BaseUserEntity.CurrentTableName + " WHERE " + BaseUserEntity.FieldDeleted + " = " + DbHelper.GetParameter(BaseUserEntity.FieldDeleted));

            dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldDeleted, 0));
            if (!string.IsNullOrEmpty(companyName))
            {
                sb.Append(" AND " + BaseUserEntity.FieldCompanyName + " = " + DbHelper.GetParameter(BaseUserEntity.FieldCompanyName));
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldCompanyName, companyName));
                if (!string.IsNullOrEmpty(userName))
                {
                    sb.Append(" AND " + BaseUserEntity.FieldUserName + " = " + DbHelper.GetParameter(BaseUserEntity.FieldUserName));
                    dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldUserName, userName));
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(userName))
                {
                    sb.Append(" AND " + BaseUserEntity.FieldNickName + " = " + DbHelper.GetParameter(BaseUserEntity.FieldNickName));
                    dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldNickName, userName));
                }
            }

            var dt = DbHelper.Fill(sb.Put(), dbParameters.ToArray());
            // 若是有多条数据返回，把设置为无效的数据先过滤掉，防止数据有重复
            if (dt != null && dt.Rows.Count > 1)
            {
                dt = BaseUtil.SetFilter(dt, BaseUserEntity.FieldEnabled, "1");
            }

            BaseUserEntity userEntity = null;
            if (dt != null && dt.Rows.Count > 1)
            {
                result.Status = Status.UserDuplicate;
                result.StatusCode = Status.UserDuplicate.ToString();
            }
            else if (dt != null && dt.Rows.Count == 1)
            {
                userEntity = BaseEntity.Create<BaseUserEntity>(dt);
                // 用户登录
                result = LogonByEntity(userEntity, password, openId, systemCode, ipAddress, ipAddressName, macAddress, computerName, checkUserPassword, validateUserOnly, checkMacAddress);
            }
            else
            {
                sb = Pool.StringBuilder.Get();
                // 若不能正常登录、看这个人是否有超级管理员的权限？若是超级管理员，可以登录任何一个网点
                sb.Append("SELECT * "
                          + " FROM " + BaseUserEntity.CurrentTableName
                         + " WHERE " + BaseUserEntity.FieldDeleted + " = 0 "
                                 + " AND " + BaseUserEntity.FieldEnabled + " = 1 "
                                 //Troy 20160520一句话判断管理员 start
                                 + " AND " + BaseUserEntity.FieldIsAdministrator + " = 1 "
                                 //+ " AND id IN (SELECT resourceid FROM basepermission WHERE resourcecategory = 'BaseUser' AND permissionid IN (SELECT id FROM basemodule WHERE code = 'LogonAllCompany' AND enabled = 1 AND deletionstatecode = 0)) "
                                 //Troy 20160520一句话判断管理员 end
                                 + " AND (" + BaseUserEntity.FieldUserName + " = " + DbHelper.GetParameter(BaseUserEntity.FieldUserName)
                                            + " OR " + BaseUserEntity.FieldNickName + " = " + DbHelper.GetParameter(BaseUserEntity.FieldNickName) + ")");
                dbParameters = new List<IDbDataParameter>
                {
                    DbHelper.MakeParameter(BaseUserEntity.FieldUserName, userName),
                    DbHelper.MakeParameter(BaseUserEntity.FieldNickName, userName)
                };
                dt = DbHelper.Fill(sb.Put(), dbParameters.ToArray());
                if (dt != null && dt.Rows.Count > 1)
                {
                    result.Status = Status.UserDuplicate;
                    result.StatusCode = Status.UserDuplicate.ToString();
                }
                else if (dt != null && dt.Rows.Count == 1)
                {
                    userEntity = BaseEntity.Create<BaseUserEntity>(dt);
                    //bool logOnAllCompany = true;
                    // var permissionManager = new BasePermissionManager();
                    // logOnAllCompany = permissionManager.IsAuthorized(userEntity.Id, "LogonAllCompany", "登录所有网点权限");
                    //Troy 20160520没有组织机构无所谓啦 start
                    //if (logOnAllCompany)
                    //{
                    //    // 用户登录
                    //    BaseOrganizationEntity organizationEntity = BaseOrganizationManager.GetEntityByNameByCache(companyName);
                    //    if (organizationEntity != null)
                    //    {
                    //        userEntity.CompanyId = organizationEntity.Id.ToString();
                    //        userEntity.CompanyName = organizationEntity.Name;
                    //        result = LogonByEntity(userEntity, password, openId, systemCode, ipAddress, ipAddressName, macAddress, computerName, checkUserPassword, validateUserOnly, checkMacAddress);
                    //    }

                    //}

                    result = LogonByEntity(userEntity, password, openId, systemCode, ipAddress, ipAddressName, macAddress, computerName, checkUserPassword, validateUserOnly, checkMacAddress);
                    //Troy 20160520没有组织机构无所谓啦 end
                }
            }
            result.StatusMessage = GetStateMessage(result.StatusCode);
            return result;
        }
        #endregion

        #region LogonByCompanyByCode
        /// <summary>
        /// 公司编号，用户编号，密码登录
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="userCode"></param>
        /// <param name="password"></param>
        /// <param name="openId"></param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="ipAddress"></param>
        /// <param name="macAddress"></param>
        /// <param name="computerName"></param>
        /// <param name="checkUserPassword"></param>
        /// <param name="validateUserOnly"></param>
        /// <param name="checkMacAddress"></param>
        /// <param name="sourceType"></param>
        /// <param name="targetApplication"></param>
        /// <param name="targetIp"></param>
        /// <returns></returns>
        public UserLogonResult LogonByCompanyByCode(string companyCode, string userCode, string password, string openId = null, string systemCode = null, string ipAddress = null, string macAddress = null, string computerName = null, bool checkUserPassword = true, bool validateUserOnly = false, bool checkMacAddress = false, string sourceType = null, string targetApplication = null, string targetIp = null)
        {
            var result = new UserLogonResult();

            var ipAddressName = string.Empty;
            if (!string.IsNullOrEmpty(ipAddress) && BaseSystemInfo.OnInternet)
            {
                ipAddressName = IpUtil.GetInstance().FindName(ipAddress);
            }
            if (UserInfo != null)
            {
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = UserInfo.IpAddress;
                }
                // 得到MAC地址，否则会导致后面的MAC地址判断无效 赵秉杰 2012-09-02
                if (string.IsNullOrEmpty(macAddress))
                {
                    macAddress = UserInfo.MacAddress;
                }
            }

            // 2015-11-11 吉日嘎拉 是否获取到了用户信息
            BaseUserEntity userEntity = null;

            result = LogonByCompanyByCode(companyCode, userCode, out userEntity);

            if (userEntity != null)
            {
                // 2016-04-27 吉日嘎拉 用这个登录，登录的是按编号登录的，不是按电脑的用户名登录的
                userEntity.NickName = userCode;
                // 用户登录
                result = LogonByEntity(userEntity, password, openId, systemCode, ipAddress, ipAddressName, macAddress, computerName, checkUserPassword, validateUserOnly, checkMacAddress, sourceType, targetApplication, targetIp);
            }
            result.StatusMessage = GetStateMessage(result.StatusCode);

            return result;
        }
        #endregion

        #region LogonByVerificationCode
        /// <summary>
        /// 公司编号，用户编号，验证码登录
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="userCode"></param>
        /// <param name="verificationCode"></param>
        /// <param name="openId"></param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="ipAddress"></param>
        /// <param name="macAddress"></param>
        /// <param name="computerName"></param>
        /// <param name="validateUserOnly"></param>
        /// <param name="checkMacAddress"></param>
        /// <returns></returns>
        public UserLogonResult LogonByVerificationCode(string companyCode, string userCode, string verificationCode, string openId = null, string systemCode = null, string ipAddress = null, string macAddress = null, string computerName = null, bool validateUserOnly = false, bool checkMacAddress = false)
        {
            return LogonByVerificationCode(companyCode, userCode, verificationCode, openId, systemCode, ipAddress, macAddress, computerName, false, validateUserOnly, checkMacAddress);
        }

        /// <summary>
        /// 公司编号，用户编号，验证码登录
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="userCode"></param>
        /// <param name="verificationCode"></param>
        /// <param name="openId"></param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="ipAddress"></param>
        /// <param name="macAddress"></param>
        /// <param name="computerName"></param>
        /// <param name="checkUserPassword"></param>
        /// <param name="validateUserOnly"></param>
        /// <param name="checkMacAddress"></param>
        /// <returns></returns>
        public UserLogonResult LogonByVerificationCode(string companyCode, string userCode, string verificationCode, string openId = null, string systemCode = null, string ipAddress = null, string macAddress = null, string computerName = null, bool checkUserPassword = false, bool validateUserOnly = false, bool checkMacAddress = false)
        {
            var result = new UserLogonResult();

            var ipAddressName = string.Empty;
            if (!string.IsNullOrEmpty(ipAddress) && BaseSystemInfo.OnInternet)
            {
                ipAddressName = IpUtil.GetInstance().FindName(ipAddress);
            }
            if (UserInfo != null)
            {
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = UserInfo.IpAddress;
                }
                // 得到MAC地址，否则会导致后面的MAC地址判断无效 赵秉杰 2012-09-02
                if (string.IsNullOrEmpty(macAddress))
                {
                    macAddress = UserInfo.MacAddress;
                }
            }

            // 2015-11-11 吉日嘎拉 是否获取到了用户信息
            BaseUserEntity userEntity = null;
            result = LogonByCompanyByCode(companyCode, userCode, out userEntity);
            if (userEntity != null)
            {
                // 2015-11-11 吉日嘎拉 进行手机验证
                var mobileValidate = false;
                var mobile = BaseUserContactManager.GetMobileByCache(userEntity.Id);
                //if (ValidateUtil.IsMobile(mobile))
                //{
                //    var mobileService = new MobileService();
                //    mobileValidate = mobileService.ValidateVerificationCode(UserInfo, mobile, verificationCode);
                //}
                if (!mobileValidate)
                {
                    result.Status = Status.VerificationCodeError;
                    result.StatusCode = Status.VerificationCodeError.ToString();
                    result.StatusMessage = GetStateMessage(result.StatusCode);
                    return result;
                }
                // 用户登录
                var password = string.Empty;
                checkUserPassword = false;
                result = LogonByEntity(userEntity, password, openId, systemCode, ipAddress, ipAddressName, macAddress, computerName, checkUserPassword, validateUserOnly, checkMacAddress);
            }
            result.StatusMessage = GetStateMessage(result.StatusCode);

            return result;
        }
        #endregion

        #region LogonByUserNameOnly
        /// <summary>
        /// 近用于LDAP集成登录或其它特殊用途
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public UserLogonResult LogonByUserNameOnly(string systemCode, BaseUserInfo userInfo, string userName)
        {
            var result = new UserLogonResult();
            // 先侦测是否在线
            //userLogonManager.CheckOnline();
            // 然后获取用户密码
            var userManager = new BaseUserManager(userInfo)
            {
                // 是否从角色判断管理员
                CheckIsAdministrator = true
            };
            //根据用户名获取用户信息
            var userEntity = userManager.GetByUserName(userName);


            if (userEntity != null)
            {
                var baseUserLogonManager = new BaseUserLogonManager(userInfo);
                //获取密码
                var userLogonEntity = baseUserLogonManager.GetEntityByUserId(userEntity.Id);
                var password = userLogonEntity.UserPassword;
                //再进行登录，这里密码不能是AD的密码，所以不检验密码
                result = userManager.LogonByUserName(userName, password, systemCode, null, null, null, false, false);
                //可以登录，但不建议，没有登录日志等
                //result = userManager.LogonByOpenId(openId, string.Empty, string.Empty);
            }
            // 登录时会自动记录进行日志记录，所以不需要进行重复日志记录
            //BaseLogManager.Instance.Add(userInfo, this.serviceName, MethodBase.GetCurrentMethod());

            return result;
        }
        #endregion

        #region LogonByUserName
        /// <summary>
        /// 进行登录操作
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="macAddress">MAC地址</param>
        /// <param name="computerName"></param>
        /// <param name="checkUserPassword">是否要检查用户密码</param>
        /// <param name="getOpenIdOnly"></param>
        /// <returns>用户信息</returns>
        public UserLogonResult LogonByUserName(string userName, string password, string systemCode = null, string ipAddress = null, string macAddress = null, string computerName = null, bool checkUserPassword = true, bool getOpenIdOnly = false)
        {
            var result = new UserLogonResult();
            //Troy.Cui 2018-10-06
            if (!string.IsNullOrWhiteSpace(ipAddress))
            {
                ipAddress = Utils.GetIp();
            }

            var ipAddressName = string.Empty;
            if (BaseSystemInfo.OnInternet && !string.IsNullOrEmpty(ipAddress))
            {
                ipAddressName = IpUtil.GetInstance().FindName(ipAddress);
            }

            var realName = string.Empty;
            if (UserInfo != null)
            {
                realName = UserInfo.RealName;
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = UserInfo.IpAddress;
                }
                // 得到MAC地址，否则会导致后面的MAC地址判断无效 赵秉杰 2012-09-02
                if (string.IsNullOrEmpty(macAddress))
                {
                    macAddress = UserInfo.MacAddress;
                }
            }

            // 04. 默认为用户没有找到状态，查找用户
            // 这是为了达到安全要求，不能提示用户未找到，那容易让别人猜测到帐户
            if (BaseSystemInfo.CheckPasswordStrength)
            {
                result.Status = Status.ErrorLogon;
                result.StatusCode = Status.ErrorLogon.ToString();
                result.StatusMessage = Status.ErrorLogon.ToDescription();
            }
            else
            {
                result.Status = Status.UserNotFound;
                result.StatusCode = Status.UserNotFound.ToString();
                result.StatusMessage = Status.UserNotFound.ToDescription();
            }

            // 02. 查询数据库中的用户数据？只查询未被删除的
            // 先按用户名登录
            userName = DbHelper.SqlSafe(userName);
            var where = BaseUserEntity.FieldUserName + " = N'" + userName + "' AND " + BaseUserEntity.FieldDeleted + " = 0 ";
            var dt = GetDataTable(where);

            // 服务器上、本地都需要能登录才可以
            if (dt != null && dt.Rows.Count == 0)
            {
                if (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.SqLite)
                {
                    where = " Id > 0 AND " + BaseUserEntity.FieldUserName + " = '" + userName + "' AND " + BaseUserEntity.FieldDeleted + " = 0 ";
                    dt = GetDataTable(where);
                }

                /*
                if (dt != null && dt.Rows.Count == 0)
                {
                    // 若没数据再按工号登录
                    dt = this.GetDataTable(new KeyValuePair<string, object>(BaseUserEntity.FieldCode, userName)
                        , new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0));
                }

                if (result.Rows.Count == 0)
                {
                    // 若没数据再按邮件登录
                    result = this.GetDataTable(new KeyValuePair<string, object>(BaseUserEntity.FieldEmail, userName)
                        , new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0));
                }
                else if (result.Rows.Count == 0)
                {
                    // 若没数据再按手机号码登录
                    result = this.GetDataTable(new KeyValuePair<string, object>(BaseUserEntity.FieldMobile, userName)
                        , new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0));
                }
                else if (result.Rows.Count == 0)
                {
                    // 若没数据再按手机号码登录
                    result = this.GetDataTable(new KeyValuePair<string, object>(BaseUserEntity.FieldTelephone, userName)
                        , new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0));
                }
                */
            }

            // 若是有多条数据返回，把设置为无效的数据先过滤掉，防止数据有重复
            if (dt != null && dt.Rows.Count > 1)
            {
                dt = BaseUtil.SetFilter(dt, BaseUserEntity.FieldEnabled, "1");
            }

            if (dt != null && dt.Rows.Count == 1)
            {
                // 进行登录校验
                BaseUserEntity userEntity = null;
                userEntity = BaseEntity.Create<BaseUserEntity>(dt.Rows[0]);
                if (userEntity != null)
                {
                    //增加用户名大小写判断 Troy.Cui 2020.07.20
                    if ((!BaseSystemInfo.UserNameMatchCase && userEntity.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)) || (BaseSystemInfo.UserNameMatchCase && userEntity.UserName.Equals(userName, StringComparison.Ordinal)))
                    {
                        result = LogonByEntity(userEntity, password, null, systemCode, ipAddress, ipAddressName, macAddress, computerName, checkUserPassword);
                    }
                }
            }
            else if (dt != null && dt.Rows.Count > 1)
            {
                // 用户命重复了
                BaseLogonLogManager.AddLog(systemCode, string.Empty, userName, string.Empty, ipAddress, ipAddressName, macAddress, Status.UserDuplicate.ToDescription());
                result.Status = Status.UserDuplicate;
                result.StatusCode = Status.UserDuplicate.ToString();
            }
            else
            {
                // 用户没找到
                BaseLogonLogManager.AddLog(systemCode, string.Empty, userName, string.Empty, ipAddress, ipAddressName, macAddress, Status.UserNotFound.ToDescription());
                result.Status = Status.UserNotFound;
                result.StatusCode = Status.UserNotFound.ToString();
            }
            result.StatusMessage = GetStateMessage(result.StatusCode);

            return result;
        }
        #endregion

        #region public UserLogonResult LogonByEmail(string email, string password, string openId = null, string ipAddress = null, string macAddress = null, bool checkUserPassword = true) 进行登录操作
        /// <summary>
        /// 进行登录操作
        /// </summary>
        /// <param name="email">电子邮件</param>
        /// <param name="password">密码</param>
        /// <param name="openId">单点登录标识</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="macAddress">MAC地址</param>
        /// <param name="computerName"></param>
        /// <param name="checkUserPassword">是否要检查用户密码</param>
        /// <param name="validateUserOnly"></param>
        /// <param name="checkMacAddress"></param>
        /// <returns>用户信息</returns>
        public UserLogonResult LogonByEmail(string email, string password, string openId = null, string systemCode = null, string ipAddress = null, string macAddress = null, string computerName = null, bool checkUserPassword = true, bool validateUserOnly = false, bool checkMacAddress = true)
        {
            var ipAddressName = string.Empty;
            if (BaseSystemInfo.OnInternet && !string.IsNullOrEmpty(ipAddress))
            {
                ipAddressName = IpUtil.GetInstance().FindName(ipAddress);
            }

            var result = new UserLogonResult();
            var realname = string.Empty;
            if (UserInfo != null)
            {
                realname = UserInfo.RealName;
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = UserInfo.IpAddress;
                }
                // 得到MAC地址，否则会导致后面的MAC地址判断无效 赵秉杰 2012-09-02
                if (string.IsNullOrEmpty(macAddress))
                {
                    macAddress = UserInfo.MacAddress;
                }
            }

            // 04. 默认为用户没有找到状态，查找用户
            // 这是为了达到安全要求，不能提示用户未找到，那容易让别人猜测到帐户
            if (BaseSystemInfo.CheckPasswordStrength)
            {
                result.Status = Status.ErrorLogon;
                result.StatusCode = Status.ErrorLogon.ToString();
            }
            else
            {
                result.Status = Status.UserNotFound;
                result.StatusCode = Status.UserNotFound.ToString();
            }
            result.StatusMessage = GetStateMessage(result.StatusCode);
            // 02. 查询数据库中的用户数据？只查询未被删除的
            // 先按用户名登录
            var manager = new BaseUserContactManager();
            var parameters = new List<KeyValuePair<string, object>>
            {
                // 验证通过的，才可以登录
                new KeyValuePair<string, object>(BaseUserContactEntity.FieldEmail, email)
            };
            // parameters.Add(new KeyValuePair<string, object>(BaseUserContactEntity.FieldEmailValiated, 1));
            var id = manager.GetId(parameters);

            if (!string.IsNullOrEmpty(id))
            {
                // 05. 判断密码，是否允许登录，是否离职是否正确
                var userEntity = GetEntity(id);
                result = LogonByEntity(userEntity, password, openId, systemCode, ipAddress, ipAddressName, macAddress, computerName, checkUserPassword, validateUserOnly, checkMacAddress);
            }
            return result;
        }
        #endregion

        #region public UserLogonResult LogonByMobile(string mobile, string password, string openId = null, string systemCode = null, string ipAddress = null, string macAddress = null, bool checkUserPassword = true) 进行登录操作
        /// <summary>
        /// 进行登录操作
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="password">密码</param>
        /// <param name="openId">单点登录标识</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="macAddress">MAC地址</param>
        /// <param name="computerName"></param>
        /// <param name="checkUserPassword">是否要检查用户密码</param>
        /// <param name="validateUserOnly"></param>
        /// <param name="checkMacAddress"></param>
        /// <returns>用户信息</returns>
        public UserLogonResult LogonByMobile(string mobile, string password, string openId = null, string systemCode = null, string ipAddress = null, string macAddress = null, string computerName = null, bool checkUserPassword = true, bool validateUserOnly = false, bool checkMacAddress = true)
        {
            var ipAddressName = string.Empty;
            if (BaseSystemInfo.OnInternet && !string.IsNullOrEmpty(ipAddress))
            {
                ipAddressName = IpUtil.GetInstance().FindName(ipAddress);
            }

            var result = new UserLogonResult();
            var realname = string.Empty;
            if (UserInfo != null)
            {
                realname = UserInfo.RealName;
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = UserInfo.IpAddress;
                }
                // 得到MAC地址，否则会导致后面的MAC地址判断无效 赵秉杰 2012-09-02
                if (string.IsNullOrEmpty(macAddress))
                {
                    macAddress = UserInfo.MacAddress;
                }
            }

            // 04. 默认为用户没有找到状态，查找用户
            // 这是为了达到安全要求，不能提示用户未找到，那容易让别人猜测到帐户
            if (BaseSystemInfo.CheckPasswordStrength)
            {
                result.Status = Status.ErrorLogon;
                result.StatusCode = Status.ErrorLogon.ToString();
            }
            else
            {
                result.Status = Status.ErrorLogon;
                result.StatusCode = Status.UserNotFound.ToString();
            }
            result.StatusMessage = GetStateMessage(result.StatusCode);

            // 02. 查询数据库中的用户数据？只查询未被删除的
            // 先按用户名登录
            var manager = new BaseUserContactManager();
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserContactEntity.FieldMobile, mobile)
            };
            // 验证通过的，才可以登录
            // parameters.Add(new KeyValuePair<string, object>(BaseUserContactEntity.FieldMobileValiated, 1));
            var id = manager.GetId(parameters);

            if (!string.IsNullOrEmpty(id))
            {
                // 05. 判断密码，是否允许登录，是否离职是否正确
                var userEntity = GetEntity(id);
                result = LogonByEntity(userEntity, password, openId, systemCode, ipAddress, ipAddressName, macAddress, computerName, checkUserPassword, validateUserOnly, checkMacAddress);
            }
            return result;
        }
        #endregion

        #region LogonByOpenId
        /// <summary>
        /// 根据OpenId登录
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="ipAddress"></param>
        /// <param name="macAddress"></param>
        /// <param name="computerName"></param>
        /// <returns></returns>
        public UserLogonResult LogonByOpenId(string openId, string systemCode, string ipAddress = null, string macAddress = null, string computerName = null)
        {
            var result = new UserLogonResult();

            var ipAddressName = string.Empty;
            if (BaseSystemInfo.OnInternet && !string.IsNullOrEmpty(ipAddress))
            {
                ipAddressName = IpUtil.GetInstance().FindName(ipAddress);
            }

            IsLogonByOpenId = true;

            // 用户没有找到状态
            result.Status = Status.UserNotFound;
            result.StatusCode = Status.UserNotFound.ToString();
            result.StatusMessage = GetStateMessage(result.StatusCode);
            // 检查是否有效的合法的参数
            if (!string.IsNullOrEmpty(openId))
            {
                var parameters = new List<KeyValuePair<string, object>>();
                // parameters.Add(new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1));
                // parameters.Add(new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0));
                if (!string.IsNullOrEmpty(openId))
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldOpenId, openId));
                }
                // 若是单点登录，那就不能判断ip地址，因为不是直接登录，是间接登录
                if (!string.IsNullOrEmpty(ipAddress))
                {
                    // parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldIPAddress, ipAddress));
                }
                if (!string.IsNullOrEmpty(macAddress))
                {
                    // parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldMACAddress, macAddress));
                }
                var dt = new BaseUserLogonManager(DbHelper, UserInfo).GetDataTable(parameters);
                if (dt != null && dt.Rows.Count == 1)
                {
                    var userLogonEntity = new BaseUserLogonEntity();
                    userLogonEntity.GetFrom(dt.Rows[0]);
                    //下面的判断了openid的过期时间，sso登录时没有重新更新openid,直接取数据库中的openid,在此做判断时就有可能过期了，
                    //导致子系统通过openid登录时无法获取用户信息，登录不成功
                    //办法：1、sso登录时创建新的openid  2、此处不做openid过期判断 3，sso登录不创建新openid,但改变一下表中的过期时间
                    //此处判断openid过期时间可能有问题
                    //Troy 20160520 取消OpenId过期时间判断start
                    //if (userLogonEntity.OpenIdTimeoutTime.HasValue && userLogonEntity.OpenIdTimeout > DateTime.Now)
                    //{
                    var userEntity = GetEntity(dt.Rows[0][BaseUserLogonEntity.FieldUserId].ToString());
                    result = LogonByEntity(userEntity, userLogonEntity.UserPassword, openId, systemCode, ipAddress, ipAddressName, macAddress, computerName, false);
                    //}
                    //else
                    //{
                    //    result.StatusCode = Status.Timeout.ToString();
                    //}
                    //Troy 20160520 取消OpenId过期时间判断end
                }
            }
            result.StatusMessage = GetStateMessage(result.StatusCode);

            return result;
        }
        #endregion

        #region public UserLogonResult LogonByEntity(BaseUserEntity userEntity, string password, string openId = null, string systemCode = null, string ipAddress = null, string macAddress = null, bool checkUserPassword = true, bool validateUserOnly = false) 进行登录操作

        /// <summary>
        /// 进行登录操作
        /// 2015-12-06 吉日嘎拉 进行日志更新，方便优化代码
        /// </summary>
        /// <param name="userEntity">用户实体</param>
        /// <param name="password">密码</param>
        /// <param name="openId">单点登录标识</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="ipAddressName"></param>
        /// <param name="macAddress">MAC地址</param>
        /// <param name="computerName"></param>
        /// <param name="checkUserPassword">是否要检查用户密码</param>
        /// <param name="validateUserOnly">只要验证用户就可以了</param>
        /// <param name="checkMacAddress">检查MAC</param>
        /// <param name="sourceType">登录来源</param>
        /// <param name="targetApplication">目标应用</param>
        /// <param name="targetIp">目标应用IP</param>
        /// <returns>用户信息</returns>
        public UserLogonResult LogonByEntity(BaseUserEntity userEntity, string password, string openId = null, string systemCode = null, string ipAddress = null, string ipAddressName = null, string macAddress = null, string computerName = null, bool checkUserPassword = true, bool validateUserOnly = false, bool checkMacAddress = true, string sourceType = null, string targetApplication = null, string targetIp = null)
        {
            var result = new UserLogonResult();

            // 2016-01-22 吉日嘎拉 这里是处理，多个mac的问题，处理外部传递过来的参数不正确，不只是自己的系统，还有外部调用的系统的问题
            if (!string.IsNullOrEmpty(ipAddress))
            {
                var ips = ipAddress.Split(';');
                ips = ips.Where(t => !string.IsNullOrEmpty(t) && !t.Equals("127.0.0.01")).OrderBy(ip => ip).Take(1).ToArray();
                ipAddress = string.Join(";", ips);
            }
            if (!string.IsNullOrEmpty(macAddress))
            {
                // 2016-04-28 pda 的 sn 编码也需要能保存起来 t.Length == 17。
                var mac = macAddress.Split(';');
                mac = mac.Where(t => !string.IsNullOrEmpty(t) && !t.Equals("00-00-00-00-00-00")).OrderBy(ip => ip).Take(2).ToArray();
                macAddress = string.Join(";", mac);
            }

            var errorMark = 0;

            try
            {
                var orginalPassWord = password;
                var parameters = new List<KeyValuePair<string, object>>();

                var userLogonManager = new BaseUserLogonManager(UserInfo, UserLogonTable);
                var userLogonEntity = userLogonManager.GetEntityByUserId(userEntity.Id);
                // 2015-12-24 吉日嘎拉进行代码分离、重复利用这部分代码、需要检查接口安全认证
                result = CheckUser(userEntity, userLogonEntity);
                // 2015-12-26 吉日嘎拉，修改状态判断，成功验证才可以。
                if (!(result.Status == Status.Ok))
                {
                    // 这个是登录失败的
                    BaseLogonLogManager.AddLog(systemCode, userEntity, ipAddress, ipAddressName, macAddress, result.StatusMessage, 1, 0, sourceType, targetApplication, targetIp);
                    return result;
                }

                var commandText = string.Empty;
                // macAddress 地址变化的，会进行手机验证，没有macAddress的，再进行城市验证
                if (string.IsNullOrWhiteSpace(macAddress))
                {
                    if (!string.IsNullOrWhiteSpace(ipAddressName))
                    {
                        if (!string.IsNullOrWhiteSpace(userLogonEntity.IpAddressName))
                        {
                            if (!ipAddressName.Equals("局域网") && !userLogonEntity.IpAddressName.Equals(ipAddressName))
                            {
                                // TODO 开启手机验证功能！, 三天验证一次也可以了
                                errorMark = 10;
                                BaseUserContactEntity userContactEntity = null;
                                // 2015-12-08 吉日嘎拉 提高效率、从缓存获取数据
                                userContactEntity = BaseUserContactManager.GetEntityByCache(userLogonEntity.UserId);

                                var needVerification = false;
                                if (userContactEntity.MobileValidatedTime.HasValue)
                                {
                                    var timeSpan = DateTime.Now - userContactEntity.MobileValidatedTime.Value;
                                    if ((timeSpan.TotalDays) > 7)
                                    {
                                        needVerification = true;
                                    }
                                }
                                else
                                {
                                    needVerification = true;
                                }
                                // 需要进行手机验证
                                if (needVerification)
                                {
                                    commandText = "UPDATE " + BaseUserContactEntity.CurrentTableName
                                                + " SET " + BaseUserContactEntity.FieldMobileValidated + " = " + DbHelper.GetParameter(BaseUserContactEntity.FieldMobileValidated)
                                                + " WHERE " + BaseUserContactEntity.FieldUserId + " = " + DbHelper.GetParameter(BaseUserContactEntity.FieldUserId)
                                                + " AND " + BaseUserContactEntity.FieldMobileValidatedTime + " IS NULL "
                                                + " AND " + BaseUserContactEntity.FieldMobileValidated + " = 1 ";
                                    errorMark = 11;
                                    var dbParameters = new List<IDbDataParameter>
                                    {
                                        DbHelper.MakeParameter(BaseUserContactEntity.FieldMobileValidated, 0),
                                        DbHelper.MakeParameter(BaseUserContactEntity.FieldUserId, userContactEntity.UserId)
                                    };
                                    dbHelper.ExecuteNonQuery(commandText, dbParameters.ToArray());
                                }
                            }
                        }
                        if (!ipAddressName.Equals("局域网"))
                        {
                            userLogonEntity.IpAddressName = ipAddressName;
                        }
                    }
                }


                // 08. 是否检查用户IP地址，是否进行访问限制？管理员不检查IP，不管是否检查，要把最后的登录地址等进行更新才对
                // && !this.IsAdministrator(userEntity.Id

                if (userLogonEntity.CheckIpAddress == 1)
                {
                    // BaseParameterManager parameterManager = new BaseParameterManager(this.DbHelper, this.UserInfo);
                    // 内网不进行限制
                    // if (!ipAddress.StartsWith("192.168."))
                    // {
                    // }
                    // 没有设置IPAddress地址时不检查

                    /*

                    if (!string.IsNullOrEmpty(ipAddress))
                    {
                        parameters = new List<KeyValuePair<string, object>>();
                        parameters.Add(new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterId, userEntity.Id));
                        parameters.Add(new KeyValuePair<string, object>(BaseParameterEntity.FieldCategoryCode, "IPAddress"));
                        // 没有设置IP地址时不检查
                        errorMark = 12;
                        if (parameterManager.Exists(parameters))
                        {
                            if (!this.CheckIPAddress(ipAddress, userEntity.Id))
                            {
                                parameters = new List<KeyValuePair<string, object>>();
                                parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldIPAddress, ipAddress));
                                errorMark = 13;
                                userLogonManager.Update(userEntity.Id, parameters);
                                errorMark = 131;
                                result.StatusCode = Status.ErrorIPAddress.ToString();
                                errorMark = 132;
                                result.StatusMessage = this.GetStateMessage(result.StatusCode);
                                errorMark = 14;
                                BaseLogonLogManager.AddLog(systemCode, userEntity, ipAddress, ipAddressName, macAddress, Status.ErrorIPAddress.ToDescription());
                                return result;
                            }
                        }
                    }

                    */

                    // 没有设置MAC地址时不检查
                    if (checkMacAddress && !string.IsNullOrEmpty(macAddress))
                    {
                        // if (!CheckMACAddress(userLogonEntity.Id, macAddress))
                        if (!CheckMacAddressByCache(userLogonEntity.UserId.ToString(), macAddress))
                        {
                            result.Status = Status.ErrorMacAddress;
                            result.StatusCode = Status.ErrorMacAddress.ToString();
                            result.StatusMessage = GetStateMessage(result.StatusCode);
                            errorMark = 17;
                            BaseLogonLogManager.AddLog(systemCode, userEntity, ipAddress, ipAddressName, macAddress, Status.ErrorMacAddress.ToDescription(), 1, 0, sourceType, targetApplication, targetIp);
                            return result;
                        }
                    }
                }

                // 10. 只允许登录一次，需要检查是否自己重新登录了，或者自己扮演自己了
                if (BaseSystemInfo.CheckOnline)
                {
                    if ((UserInfo != null) && (!UserInfo.Id.Equals(userEntity.Id)))
                    {
                        // 若检查在线，那就检查OpenId是否还是哪个人
                        if (userLogonEntity.ConcurrentUser == 0)
                        {
                            if (userLogonEntity.UserOnline > 0)
                            {
                                // 自己是否登录了2次，在没下线的情况下
                                var isSelf = false;
                                if (!string.IsNullOrEmpty(openId))
                                {
                                    if (!string.IsNullOrEmpty(userLogonEntity.OpenId))
                                    {
                                        if (userLogonEntity.OpenId.Equals(openId))
                                        {
                                            isSelf = true;
                                        }
                                    }
                                }
                                if (!isSelf)
                                {
                                    result.Status = Status.ErrorOnline;
                                    result.StatusCode = Status.ErrorOnline.ToString();
                                    result.StatusMessage = GetStateMessage(result.StatusCode);
                                    errorMark = 19;
                                    BaseLogonLogManager.AddLog(systemCode, userEntity, ipAddress, ipAddressName, macAddress, Status.ErrorOnline.ToDescription(), 1, 0, sourceType, targetApplication, targetIp);
                                    //先允许登录，然后用强制已登录用户退出登录2019/08/10 Troy.Cui
                                    //return result;
                                }
                            }
                        }
                    }
                }

                // 03. 系统是否采用了密码加密策略？
                if (!IsLogonByOpenId)
                {
                    // 2015-11-11 吉日嘎拉 是否检查密码，还有其他方式的登录、例如验证码登录，OpenId登录等
                    if (checkUserPassword)
                    {
                        if (BaseSystemInfo.ServerEncryptPassword && SystemEncryptPassword)
                        {
                            errorMark = 20;
                            password = EncryptUserPassword(password, userLogonEntity.Salt);
                        }

                        // 11. 密码是否正确(null 与空看成是相等的)
                        if (!(string.IsNullOrEmpty(userLogonEntity.UserPassword) && string.IsNullOrEmpty(password)))
                        {
                            var userPasswordOk = true;
                            errorMark = 201;
                            // 用户密码是空的
                            if (string.IsNullOrEmpty(userLogonEntity.UserPassword))
                            {
                                // 但是输入了不为空的密码
                                if (!string.IsNullOrEmpty(password))
                                {
                                    userPasswordOk = false;
                                }
                            }
                            else
                            {
                                // 用户的密码不为空，但是用户是输入了密码
                                if (string.IsNullOrEmpty(password))
                                {
                                    userPasswordOk = false;
                                }
                                else
                                {
                                    errorMark = 202;
                                    // 再判断用户的密码与输入的是否相同
                                    userPasswordOk = userLogonEntity.UserPassword.ToUpper().Equals(password.ToUpper());
                                }
                            }
                            // 用户的密码不相等
                            if (!userPasswordOk)
                            {
                                // 这里更新用户连续输入错误密码次数
                                errorMark = 203;
                                userLogonEntity.PasswordErrorCount = userLogonEntity.PasswordErrorCount + 1;
                                errorMark = 204;
                                if (BaseSystemInfo.PasswordErrorLockLimit > 0 && userLogonEntity.PasswordErrorCount >= BaseSystemInfo.PasswordErrorLockLimit)
                                {
                                    parameters = new List<KeyValuePair<string, object>>();
                                    if (BaseSystemInfo.PasswordErrorLockCycle == 0)
                                    {
                                        // 设置为无效，需要管理员进行审核才可以
                                        parameters.Add(new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 0));
                                        // 待审核状态
                                        parameters.Add(new KeyValuePair<string, object>(BaseUserEntity.FieldAuditStatus, AuditStatus.WaitForAudit.ToString()));
                                        errorMark = 21;
                                        Update(userEntity.Id, parameters);
                                    }
                                    else
                                    {
                                        // 这个是进行锁定帐户设置。
                                        userLogonEntity.LockStartTime = DateTime.Now;
                                        userLogonEntity.LockEndTime = DateTime.Now.AddMinutes(BaseSystemInfo.PasswordErrorLockCycle);
                                        parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldLockStartTime, userLogonEntity.LockStartTime));
                                        parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldLockEndTime, userLogonEntity.LockEndTime));
                                        errorMark = 22;
                                        userLogonManager.Update(userEntity.Id, parameters);
                                    }
                                }
                                else
                                {
                                    parameters = new List<KeyValuePair<string, object>>
                                    {
                                        new KeyValuePair<string, object>(BaseUserLogonEntity.FieldPasswordErrorCount, userLogonEntity.PasswordErrorCount)
                                    };
                                    errorMark = 23;
                                    userLogonManager.Update(userEntity.Id, parameters);
                                }
                                // 密码错误后 1：应该记录日志
                                errorMark = 24;
                                BaseLogonLogManager.AddLog(systemCode, userEntity, ipAddress, ipAddressName, macAddress, Status.PasswordError.ToDescription(), 1, 0, sourceType, targetApplication, targetIp);
                                // TODO: 密码错误后 2：看最近1个小时输入了几次错误了？24小时里。
                                // TODO: 密码错误后 3：若错误密码数量已经超过了指定的限制，那用户就需要被锁定1个小时。
                                // TODO: 密码错误后 4：同时需要处理返回值，是由于密码次数过多导致的被锁定，登录时也应该能读取这个状态比较，时间过期了，也应该进行处理一下状态。
                                // 密码强度检查，若是要有安全要求比较高的，返回的提醒消息要进行特殊处理，不能返回非常明确的提示信息。
                                if (BaseSystemInfo.CheckPasswordStrength)
                                {
                                    result.Status = Status.ErrorLogon;
                                    result.StatusCode = Status.ErrorLogon.ToString();
                                }
                                else
                                {
                                    result.Status = Status.PasswordError;
                                    result.StatusCode = Status.PasswordError.ToString();
                                }
                                result.StatusMessage = GetStateMessage(result.StatusCode);

                                return result;
                            }

                        }

                        userLogonEntity.PasswordErrorCount = 0;
                    }
                }

                //2015-07-31  宋彪添加的密码检测
                //密码检测 强度检测
                //string message;
                //if (IsWeakPassWord(userEntity, orginPassWord, out message))
                //{
                //    //需要修改密码
                //    userLogonEntity.NeedModifyPassword = 1;
                //    result.StatusMessage = message;
                //}

                // 09. 更新IP地址，更新MAC地址，这里是为只执行一次更新优化数据库I/O，若登录成功自然连续输入密码错误就是0了。
                // parameters = new List<KeyValuePair<string, object>>();
                // parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldPasswordErrorCount, 0));
                if (!string.IsNullOrEmpty(ipAddress))
                {
                    userLogonEntity.IpAddress = ipAddress;
                    // parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldIPAddress, ipAddress));
                }
                if (!string.IsNullOrEmpty(macAddress))
                {
                    userLogonEntity.MacAddress = macAddress;
                    // parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldMACAddress, macAddress));
                }
                // userLogonManager.Update(userEntity.Id, parameters);

                // 可以正常登录了
                result.Status = Status.Ok;
                result.StatusCode = Status.Ok.ToString();
                result.StatusMessage = GetStateMessage(result.StatusCode);

                // 13. 登录、重新登录、扮演时的在线状态进行更新
                //userLogonManager.ChangeOnline(userEntity.Id);

                errorMark = 25;
                result.UserInfo = ConvertToUserInfo(userEntity, userLogonEntity, validateUserOnly);
                result.UserInfo.IpAddress = ipAddress;
                result.UserInfo.MacAddress = macAddress;
                // 2015-02-03 宋彪 设置 SystemCode
                result.UserInfo.SystemCode = systemCode;
                if (SystemEncryptPassword)
                {
                    //result.UserInfo.Password = password;
                }

                // 这里是判断用户是否为系统管理员的
                if (!validateUserOnly || CheckIsAdministrator)
                {
                    // result.IsAdministrator = IsAdministrator(userEntity);
                    // 在没登录时才发邮件,重复登录的,看是否有必要发邮件?
                    // 向管理员发送登录提醒邮件
                    // if (BaseUserManager.IsAdministrator(result.UserInfo.Id))
                    // {
                    // SendLoginMailToAdministrator(result.UserInfo, systemCode);
                    // }
                }

                // 14. 记录系统访问日志
                if (result.StatusCode == Status.Ok.ToString() && !string.IsNullOrEmpty(systemCode))
                {
                    userLogonEntity.SystemCode = systemCode;
                    userLogonEntity.ComputerName = computerName;
                    // 登录成功的日志文件
                    errorMark = 31;
                    BaseLogonLogManager.AddLog(systemCode, userEntity, ipAddress, ipAddressName, macAddress, Status.UserLogon.ToDescription(), 1, 1, sourceType, targetApplication, targetIp);

                    if (IsLogonByOpenId)
                    {
                        errorMark = 33;
                        userLogonManager.UpdateVisitTime(userLogonEntity, false);
                    }
                    else
                    {
                        errorMark = 32;
                        result.UserInfo.OpenId = userLogonManager.UpdateVisitTime(userLogonEntity, true);
                    }

                    // 这里统一进行缓存保存就可以了、提高效率、把整个用户的登录信息缓存起来，
                    // 这个里面要考虑多个系统的登录区别问题
                    errorMark = 29;
                }

                // 宋彪 这里增加登录提醒功能 新线程中处理 确保不影响登录主线程 暂时只在SSO上
                // SendLogonRemind(result.UserInfo);
            }
            catch (Exception ex)
            {
                var writeMessage = "BaseUserManager.LogonByEntity:发生时间:" + DateTime.Now
                    + Environment.NewLine + "errorMark = " + errorMark
                    + Environment.NewLine + "Message:" + ex.Message
                    + Environment.NewLine + "Source:" + ex.Source
                    + Environment.NewLine + "StackTrace:" + ex.StackTrace
                    + Environment.NewLine + "TargetSite:" + ex.TargetSite
                    + Environment.NewLine;
                LogUtil.WriteLog(writeMessage, "Exception");
                throw;
            }

            return result;
        }
        #endregion

        #region public bool ValidateUser(string userName, string password) 进行密码验证
        /// <summary>
        /// 进行密码验证
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>是否通过验证</returns>
        public bool ValidateUser(string userName, string password)
        {
            // 先按用户名登录
            var dt = GetDataTable(new KeyValuePair<string, object>(BaseUserEntity.FieldUserName, userName)
                , new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
                , new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1));

            if (dt != null && dt.Rows.Count == 0)
            {
                // 若没数据再按工号登录
                dt = GetDataTable(new KeyValuePair<string, object>(BaseUserEntity.FieldCode, userName)
                    , new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
                    , new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1));

                /*
                if (result.Rows.Count == 0)
                {
                    // 若没数据再按邮件登录
                    result = this.GetDataTable(new KeyValuePair<string, object>(BaseUserEntity.FieldEmail, userName)
                        , new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
                        , new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1));
                }
                else if (result.Rows.Count == 0)
                {
                    // 若没数据再按手机号码登录
                    result = this.GetDataTable(new KeyValuePair<string, object>(BaseUserEntity.FieldMobile, userName)
                        , new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
                        , new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1));
                }
                else if (result.Rows.Count == 0)
                {
                    // 若没数据再按手机号码登录
                    result = this.GetDataTable(new KeyValuePair<string, object>(BaseUserEntity.FieldTelephone, userName)
                        , new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
                        , new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1));
                }
                */
            }
            BaseUserEntity userEntity = null;
            BaseUserLogonEntity userLogonEntity = null;
            var parameters = new List<KeyValuePair<string, object>>();
            if (dt != null && dt.Rows.Count > 1)
            {
                return false;
            }
            else if (dt != null && dt.Rows.Count == 1)
            {
                // 05. 判断密码，是否允许登录，是否离职是否正确
                userEntity = BaseEntity.Create<BaseUserEntity>(dt.Rows[0]);
                if (!string.IsNullOrEmpty(userEntity.AuditStatus)
                    && userEntity.AuditStatus.EndsWith(AuditStatus.WaitForAudit.ToString()))
                {
                    return false;
                }
                var userLogonManager = new BaseUserLogonManager(DbHelper, UserInfo);
                userLogonEntity = userLogonManager.GetEntityByUserId(userEntity.Id);
                if (!string.IsNullOrEmpty(userEntity.AuditStatus)
                    && userEntity.AuditStatus.EndsWith(AuditStatus.WaitForAudit.ToString())
                    && userLogonEntity.PasswordErrorCount == 0)
                {
                    return false;
                }

                // 06. 允许登录时间是否有限制
                if (userLogonEntity.AllowEndTime != null)
                {
                    userLogonEntity.AllowEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, userLogonEntity.AllowEndTime.Value.Hour, userLogonEntity.AllowEndTime.Value.Minute, userLogonEntity.AllowEndTime.Value.Second);
                }
                if (userLogonEntity.AllowStartTime != null)
                {
                    userLogonEntity.AllowStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, userLogonEntity.AllowStartTime.Value.Hour, userLogonEntity.AllowStartTime.Value.Minute, userLogonEntity.AllowStartTime.Value.Second);
                    if (DateTime.Now < userLogonEntity.AllowStartTime)
                    {
                        return false;
                    }
                }
                if (userLogonEntity.AllowEndTime != null)
                {
                    if (DateTime.Now > userLogonEntity.AllowEndTime)
                    {
                        return false;
                    }
                }

                // 07. 锁定日期是否有限制
                if (userLogonEntity.LockStartTime != null)
                {
                    if (DateTime.Now > userLogonEntity.LockStartTime)
                    {
                        if (userLogonEntity.LockEndTime == null || DateTime.Now < userLogonEntity.LockEndTime)
                        {
                            return false;
                        }
                    }
                }
                if (userLogonEntity.LockEndTime != null)
                {
                    if (DateTime.Now < userLogonEntity.LockEndTime)
                    {
                        return false;
                    }
                }

                // 03. 系统是否采用了密码加密策略？
                if (BaseSystemInfo.ServerEncryptPassword)
                {
                    password = EncryptUserPassword(password);
                }

                // 11. 密码是否正确(null 与空看成是相等的)
                if (!(string.IsNullOrEmpty(userLogonEntity.UserPassword) && string.IsNullOrEmpty(password)))
                {
                    var userPasswordOk = true;
                    // 用户密码是空的
                    if (string.IsNullOrEmpty(userLogonEntity.UserPassword))
                    {
                        // 但是输入了不为空的密码
                        if (!string.IsNullOrEmpty(password))
                        {
                            userPasswordOk = false;
                        }
                    }
                    else
                    {
                        // 用户的密码不为空，但是用户是输入了密码
                        if (string.IsNullOrEmpty(password))
                        {
                            userPasswordOk = false;
                        }
                        else
                        {
                            // 再判断用户的密码与输入的是否相同
                            userPasswordOk = userLogonEntity.UserPassword.Equals(password);
                        }
                    }
                    // 用户的密码不相等
                    if (!userPasswordOk)
                    {
                        // 这里更新用户连续输入错误密码次数
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region public bool UserIsLogon(BaseUserInfo userInfo) 判断用户是否已经登录了？
        /// <summary>
        /// 判断用户是否已经登录了？
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>是否已经登录了</returns>
        public bool UserIsLogon(BaseUserInfo userInfo)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserLogonEntity.FieldUserId, userInfo.UserId),
                new KeyValuePair<string, object>(BaseUserLogonEntity.FieldOpenId, userInfo.OpenId),
                new KeyValuePair<string, object>(BaseUserLogonEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseUserLogonEntity.FieldEnabled, 1)
            };
            return new BaseUserLogonManager(userInfo).Exists(parameters);
        }
        #endregion
    }
}