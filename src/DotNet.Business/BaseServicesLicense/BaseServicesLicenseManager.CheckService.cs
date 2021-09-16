//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseServicesLicenseManager
    /// 服务管理
    /// 
    /// 修改记录
    /// 
    ///		2015.12.25 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.12.25</date>
    /// </author> 
    /// </summary>
    public partial class BaseServicesLicenseManager : BaseManager
    {
        /// <summary>
        /// 检查一个服务调用是否是允许调用的？
        /// 1：是否要记录日志？
        /// 2：是否需要埋点？检查性能？访问频率等？调用次数？
        /// 3：非合法的调用？是否日志记录？
        /// 4：异常的要进行处理？
        /// </summary>
        /// <param name="appKey">应用唯一标识，用户编号</param>
        /// <param name="appSecret">应用的签名密钥</param>
        /// <param name="callLimit">是否进行限制</param>
        /// <param name="minutes">几分钟内</param>
        /// <param name="limit">限制多少次调用</param>
        /// <param name="systemCode">访问子系统</param>
        /// <param name="permissionCode">判断的权限编号</param>
        /// <returns>验证情况</returns>
        public static BaseResult CheckService(string appKey, string appSecret, bool callLimit = false, int minutes = 10, int limit = 100000, string systemCode = "Base", string permissionCode = null)
        {
            var result = new BaseResult();
            result.Status = false;

            try
            {
                // AppKey： 23286115
                // AppSecret： c8d1f06f599d7370467993c72a34c701
                // permissionCode： "User.Add"

                // 1: 判断参数是否合理？目标服务，总不可以为空，否则怎么区别谁在调用这个服务了？
                if (string.IsNullOrEmpty(appKey))
                {
                    result.Status = false;
                    result.StatusCode = Status.ParameterError.ToString();
                    result.StatusMessage = "appKey " + Status.ParameterError.ToDescription();
                    return result;
                }
                if (string.IsNullOrEmpty(appSecret))
                {
                    result.Status = false;
                    result.StatusCode = Status.ParameterError.ToString();
                    result.StatusMessage = "appSecret " + Status.ParameterError.ToDescription();
                    return result;
                }

                // 2: 是否正确的用户？
                appKey = appKey.Trim();
                var userId = string.Empty;
                var userEntity = BaseUserManager.GetEntityByCodeByCache(appKey);
                if (userEntity == null)
                {
                    result.Status = false;
                    result.StatusCode = Status.AccessDeny.ToString();
                    result.StatusMessage = "appKey:" + appKey + ", GetEntityByCodeByCache " + Status.ParameterError.ToDescription();
                    return result;
                }
                else
                {
                    userId = userEntity.Id;
                }

                // 3: 判断是否在接口角色里, 只有在接口角色里的，才可以进行远程调用，这样也方便把接口随时踢出来。

                var roleCode = "Interface";
                // LogUtil.WriteLog("1: userId:" + userId + "," + "roleCode:" + roleCode);
                if (!BaseUserManager.IsInRoleByCache("Base", userId, roleCode))
                {
                    result.Status = false;
                    result.StatusCode = Status.AccessDeny.ToString();
                    result.StatusMessage = "非接口用户、访问被拒绝 IsInRoleByCache";
                    return result;
                }
                //Troy.Cui 2016.12.28
                //if (BaseSystemInfo.RedisEnabled)
                //{
                //    // 4: 判断调用的频率是否？这里需要高速判断，不能总走数据库？调用的效率要高，不能被远程接口给拖死了、自己的服务都不正常了。
                //    if (callLimit && PooledRedisHelper.CallLimit(userId, minutes, limit))
                //    {
                //        result.Status = false;
                //        result.StatusCode = Status.AccessDeny.ToString();
                //        result.StatusMessage = "访问频率过高、访问被拒绝 CallLimit";
                //        return result;
                //    }
                //}

                appSecret = appSecret.Trim();
                // 5: 判断签名是否有效？是否过期？可以支持多个签名，容易升级、容易兼容、容易有个过度的缓冲期。为了提高安全性，必须要有签名才对。
                if (!CheckServiceByCache(userId, appSecret))
                {
                    result.Status = false;
                    result.StatusCode = Status.AccessDeny.ToString();
                    result.StatusMessage = "不合法签名、访问被拒绝";
                    return result;
                }

                var ipAddress = Utils.GetIp();
                // 6: 判断对方的ip是否合法的？1个服务程序，可以有多个ip。可以把服务当一个用户看待，一个目标用户可能也配置了多个服务，一般是远程连接。
                //int? checkIpAddress = 0;
                var userLogOnEntity = BaseUserLogOnManager.GetEntityByCache(userId);
                if (!BaseUserManager.CheckIpAddressByCache(userId, userLogOnEntity, ipAddress, true))
                {
                    result.Status = false;
                    result.StatusCode = Status.ErrorIpAddress.ToString();
                    result.StatusMessage = Status.ErrorIpAddress.ToDescription();
                    return result;
                }

                // 7: 判断是否有效？判断时间是否对？
                var userLogOnResult = BaseUserManager.CheckUser(userEntity, userLogOnEntity);
                if (userLogOnResult.StatusCode != Status.Ok.ToString())
                {
                    BaseLoginLogManager.AddLog(systemCode, userEntity, ipAddress, string.Empty, string.Empty, userLogOnResult.StatusMessage);
                    result.StatusCode = userLogOnResult.StatusCode;
                    result.StatusMessage = userLogOnResult.StatusMessage;
                    return result;
                }

                // 8: 判断是否有权限？防止被过渡调用，拖死数据库，可以用缓存的方式进行判断，这样不容易被客户端、合作伙伴拖垮。
                if (!string.IsNullOrWhiteSpace(permissionCode))
                {
                    // LogUtil.WriteLog("10: userId:" + userId + "," + "permissionCode:" + permissionCode);
                    if (!BasePermissionManager.IsAuthorizedByCache("Base", userId, permissionCode))
                    {
                        result.Status = false;
                        result.StatusCode = Status.AccessDeny.ToString();
                        result.StatusMessage = "无权限 " + permissionCode;
                        return result;
                    }
                }

                // 9：目前需要判断的，都加上了。
                result.Status = true;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.StatusMessage = ex.Message;
                //LogHttpUtil.WriteException(ex);
            }

            return result;
        }
    }
}