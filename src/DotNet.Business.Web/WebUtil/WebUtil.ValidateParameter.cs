//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

#if NET452_OR_GREATER
using System.Web;
#endif

namespace DotNet.Business
{
    using Util;

    /// <summary>
    /// 
    /// ValidateParameter
    /// 参数验证，用于调接口的参数检查
    /// 要申请AppKey和AppSecret：在单点系统创建用户，用户code为AppKey，同时在BASE_SERVICES_LICENSE里添加一条记录设置AppSecret，用户同时要加到接口Interface角色里
    /// 
    /// 修改记录
    ///
    ///		2016-09-02 版本：1.0 SongBiao 参考 JiRiGaLa 手机接口验证方法 实现参数验证功能。
    ///
    /// <author>
    ///		<name>SongBiao</name>
    ///		<date>2016-09-02</date>
    /// </author>
    /// </summary>
    public partial class WebUtil
    {
#if NET452_OR_GREATER
        /// <summary>
        /// 验证参数
        /// </summary>
        /// <param name="context">上下文对象</param>
        /// <param name="checkLocalIp">是否检测ip</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns></returns>
        public BaseResult ValidateParameter(HttpContext context, bool checkLocalIp = true, string permissionCode = null)
        {
            var result = new BaseResult();

            BaseUserInfo userInfo = null;

            result = ValidateParameter(context, ref userInfo, checkLocalIp, permissionCode);

            return result;
        }

        /// <summary>
        /// 参数检查
        /// </summary>
        /// <param name="context">当前请求上下文</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="checkLocalIp">检查内网</param>
        /// <param name="permissionCode">权限菜单的Code</param>
        /// <returns></returns>
        public static BaseResult ValidateParameter(HttpContext context, ref BaseUserInfo userInfo, bool checkLocalIp = true, string permissionCode = null)
        {
            var result = new BaseResult();

            // 1：是否是有效的系统编号？
            if (context.Request["systemCode"] == null)
            {
                result.Status = false;
                result.StatusCode = Status.ParameterError.ToString();
                result.StatusMessage = "systemCode " + Status.ParameterError.ToDescription();
                return result;
            }

            var systemCode = context.Request["systemCode"];
            if (!BaseSystemManager.CheckSystemCode(systemCode))
            {
                result.Status = false;
                result.StatusCode = Status.ParameterError.ToString();
                result.StatusMessage = "systemCode 不是有效的系统编号";
                return result;
            }

            var ipAddress = Utils.GetIp();
            // 2016-08-22 已经正常登录的用户，也可以调用验证码函数，这样接口程序更灵活一些，更方便被调用
            if (context.Request["userInfo"] != null)
            {
                // 从当前使用的用户确定当前使用者
                userInfo = BaseUserInfo.Deserialize(context.Request["userInfo"]);
                if (userInfo == null)
                {
                    result.Status = false;
                    result.StatusCode = Status.Error.ToString();
                    result.StatusMessage = "userInfo " + Status.Error.ToDescription();
                    return result;
                }

                // 防止伪造、判断用户的有效性
                if (!ServiceUtil.VerifySignature(userInfo))
                {
                    // LogUtil.WriteLine("userInfo.Signature:" + userInfo.Signature + " GetSignature(userInfo):" + ServiceUtil.GetSignature(userInfo));
                    result.Status = false;
                    result.StatusCode = Status.SignatureError.ToString();
                    result.StatusMessage = "userInfo " + Status.SignatureError.ToDescription();
                    return result;
                }

                // 这里需要是已经登录的用户，不是已经被踢掉的用户
                if (!ValidateOpenId(userInfo.Id, userInfo.OpenId))
                {
                    result.Status = false;
                    result.StatusCode = Status.ParameterError.ToString();
                    result.StatusMessage = "OpenId " + Status.ParameterError.ToDescription();
                    return result;
                }
            }
            else
            {
                // 2016-08-09 吉日嘎拉，必须保证是服务器调用的，外部不允许直接调用，方式被短信轰炸，有安全漏洞。
                // 检查是否为内部ip地址发送出去的手机短信  
                // 2016-08-09 宋彪 不仅仅针对短信发送，用户中心接口也要考虑
                if (checkLocalIp)
                {
                    if (!IpUtil.IsLocalIp(ipAddress))
                    {
                        // 不是内网发出的, 也不是信任的ip列表里的，直接给拒绝发送出去
                        result.Status = false;
                        result.StatusCode = Status.ErrorIpAddress.ToString();
                        result.StatusMessage = ipAddress + " " + Status.ErrorIpAddress.ToDescription();
                        return result;
                    }
                }

                // 应用唯一标识
                string appKey;
                if (context.Request["appKey"] == null)
                {
                    result.Status = false;
                    result.StatusCode = Status.ParameterError.ToString();
                    result.StatusMessage = "appKey " + Status.ParameterError.ToDescription();
                    return result;
                }
                else
                {
                    appKey = context.Request["appKey"];
                }

                // 应用的签名密钥
                string appSecret;
                if (context.Request["appSecret"] == null)
                {
                    result.Status = false;
                    result.StatusCode = Status.ParameterError.ToString();
                    result.StatusMessage = "appSecret " + Status.ParameterError.ToDescription();
                    return result;
                }
                else
                {
                    appSecret = context.Request["appSecret"];
                }

                // 检查服务的有效性，是否调用限制到了？是否有相应的权限
                //result = BaseServicesLicenseManager.CheckService(appKey, appSecret, false, 0, 0, systemCode, permissionCode);
                //if (!result.Status)
                //{
                //    return result;
                //}

                // 从接口确定当前调用者
                var userEntity = BaseUserManager.GetEntityByCodeByCache(appKey);
                if (userEntity != null)
                {
                    userInfo = new BaseUserInfo
                    {
                        Id = userEntity.Id.ToString(),
                        UserId = userEntity.Id,
                        Code = userEntity.Code,
                        UserName = userEntity.UserName,
                        NickName = userEntity.NickName,
                        RealName = userEntity.RealName,
                        CompanyId = userEntity.CompanyId.ToString(),
                        CompanyCode = userEntity.CompanyCode,
                        CompanyName = userEntity.CompanyName,
                        IpAddress = ipAddress
                    };
                }
            }

            result.Status = true;
            return result;
        }
#endif
    }
}
