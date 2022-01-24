//-----------------------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2010, DotNet.
//-----------------------------------------------------------------------------

using System.Reflection;

namespace DotNet.Business
{
    using IService;
    using Model;
    using Util;

    /// <summary>
    /// LogonService
    /// 
    /// 修改纪录
    /// 
    /// 	2014.02.13 崔文远增加(LDAP专用)
    ///		2013.06.06 张祈璟重构
    ///		2009.04.15 版本：1.0 JiRiGaLa 添加接口定义。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2009.04.15</date>
    /// </author> 
    /// </summary>
    public partial class LogonService : ILogonService
    {

        #region public BaseUserInfo LogonByUserName(string taskId, string systemCode, BaseUserInfo userInfo, string userName)

        /// <summary>
        /// 按用户名登录(LDAP专用)
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="systemCode">子系统编码</param>
        /// <param name="userInfo">用户</param>
        /// <param name="userName">用户名</param>
        /// <returns>用户实体</returns>
        public UserLogonResult LogonByUserName(string taskId, string systemCode, BaseUserInfo userInfo, string userName)
        {
            var result = new UserLogonResult();

            var parameter = ServiceInfo.Create(taskId, userInfo, MethodBase.GetCurrentMethod());

            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
             {
                // 先侦测是否在线
                //userLogonManager.CheckOnline();
                // 然后获取用户密码
                var userManager = new BaseUserManager(userInfo);
                // 是否从角色判断管理员
                userManager.CheckIsAdministrator = true;
                //根据用户名获取用户信息
                var userEntity = userManager.GetByUserName(userName);


                 if (userEntity != null)
                 {
                     var baseUserLogonManager = new BaseUserLogonManager(userInfo);
                    //获取密码
                    var userLogonEntity = baseUserLogonManager.GetEntity(userEntity.Id);
                     var password = userLogonEntity.UserPassword;
                    //再进行登录，这里密码不能是AD的密码，所以不检验密码
                    result = userManager.LogonByUserName(userName, password, systemCode, null, null, null, false, false);
                    //可以登录，但不建议，没有登录日志等
                    //result = userManager.LogonByOpenId(openId, string.Empty, string.Empty);
                }
                // 登录时会自动记录进行日志记录，所以不需要进行重复日志记录
                //BaseLogManager.Instance.Add(userInfo, this.serviceName, MethodBase.GetCurrentMethod());
            });
            return result;
        }
        #endregion

    }
}