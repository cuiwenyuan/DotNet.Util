//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Util
{
    /// <summary>
    /// BaseSystemInfo
    /// 这是系统的核心基础信息部分
    /// 
    /// 修改记录
    ///		2017.05.11 版本：4.0 Troy Cui	规范化IP地址及字段名。
    ///		2012.04.14 版本：1.0 JiRiGaLa	主键创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.04.14</date>
    /// </author>
    /// </summary>
    public partial class BaseSystemInfo
    {
        private static BaseUserInfo _userInfo = null;

        /// <summary>
        /// 当前登录系统的用户信息
        /// </summary>
        public static BaseUserInfo UserInfo
        {
            get
            {
                if (_userInfo == null)
                {
                    _userInfo = new BaseUserInfo();
                    //IP地址
                    if (string.IsNullOrEmpty(_userInfo.IpAddress))
                    {
#if NET452_OR_GREATER
                        //按照Web应用获取IP地址
                        _userInfo.IpAddress = Utils.GetIp();
#endif
                    }
                    //Mac地址  add by zgl
                    if (string.IsNullOrEmpty(_userInfo.MacAddress))
                    {
                        // 获取所有的 mac 地址
                        _userInfo.MacAddress = MachineInfo.GetMacAddress(false);
                    }

                    //用户名
                    if (string.IsNullOrEmpty(_userInfo.UserName))
                    {
                        _userInfo.UserName = Environment.MachineName;
                    }
                    //真实姓名
                    if (string.IsNullOrEmpty(_userInfo.RealName))
                    {
                        _userInfo.RealName = Environment.UserName;
                    }
                    _userInfo.ServiceUserName = ServiceUserName;
                    _userInfo.ServicePassword = ServicePassword;
                    //默认一下用户的子系统编码
                    _userInfo.SystemCode = SystemCode;
                }
                return _userInfo;
            }
            set => _userInfo = value;
        }

        /// <summary>
        /// 验证用户是否是授权的用户
        /// 不是任何人都可以调用服务的，将来这里还可以进行扩展的
        /// 例如用IP地址限制等等
        /// 这里应该能抛出异常才可以
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>验证成功</returns>
        public static bool IsAuthorized(BaseUserInfo userInfo)
        {
            var result = true;

            if (userInfo == null)
            {
                LogUtil.WriteLog("服务调用失败，请检查：userInfo", "ServiceFail");
                result = false;
            }
            // 若系统设置的用户名是空的，那就不用判断了
            if (userInfo != null && !ServiceUserName.Equals(userInfo.ServiceUserName))
            {
                LogUtil.WriteLog("服务调用失败，请检查：ServiceUserName", "ServiceFail");
                result = false;
            }
            // 若系统设置的用密码是空的，那就不用判断了
            if (userInfo != null && !ServicePassword.Equals(userInfo.ServicePassword))
            {
                LogUtil.WriteLog("服务调用失败，请检查：ServicePassword", "ServiceFail");
                result = false;
            }
            // 检查参数是否合法，防止注入攻击
            if (userInfo != null && !string.IsNullOrWhiteSpace(userInfo.Id) && !ValidateUtil.IsInt(userInfo.Id))
            {
                LogUtil.WriteLog("服务调用失败，请检查：userInfo.Id", "ServiceFail");
                result = false;
            }
            // 调用服务器的用户名、密码都对了，才可以调用服务程序，否则认为是非授权的操作
            return result;
        }
    }
}