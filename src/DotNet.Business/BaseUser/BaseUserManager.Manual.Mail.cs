//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Business
{
    using Util;

    /// <summary>
    /// BaseUserManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///		2012.04.12 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.04.12</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        /// <summary>
        /// 向管理员发送登录提醒邮件
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="systemCode">系统编码</param>
        public static void SendLoginMailToAdministrator(BaseUserInfo userInfo, string systemCode = "Base")
        {
            //如果是系统管理员登录则发送EMAIL  
            if (userInfo != null && IsAdministrator(userInfo.Id))
            {
                // 登录成功发送
                var userContactEntity = new BaseUserContactManager().GetEntityByUserId(userInfo.Id);
                if (userContactEntity != null)
                {
                    var emailAddress = userContactEntity.Email;
                    if (string.IsNullOrEmpty(emailAddress))
                    {
                        emailAddress = BaseSystemInfo.ErrorReportTo;
                    }
                    if (!string.IsNullOrEmpty(emailAddress))
                    {
                        // 没有邮箱则给管理员发邮件
                        // 使用线程发送邮件
                        var subject = "超级管理员 " + userInfo.CompanyName + " - " + userInfo.UserName + " 登录" + BaseSystemInfo.SoftFullName + " 系统提醒";
                        var body = userInfo.UserName + Environment.NewLine + ":<br/>"
                            + DateTime.Now.ToString(BaseSystemInfo.DateTimeFormat) + "登录了" + BaseSystemInfo.SoftFullName + "；<br/>" + Environment.NewLine
                            + "编号：" + userInfo.Code + "；<br/> " + Environment.NewLine
                            + "登录系统：" + systemCode + "；<br/> " + Environment.NewLine
                            + "登录IP：" + userInfo.IpAddress + "；<br/> " + Environment.NewLine
                            + "MAC地址：" + userInfo.MacAddress + "；<br/>" + Environment.NewLine
                            + "如果不是您自己登录，请马上联系系统管理员并即刻修改密码。";
                        MailUtil.SendByThread(emailAddress, subject, body);
                    }
                }
            }
        }
    }
}