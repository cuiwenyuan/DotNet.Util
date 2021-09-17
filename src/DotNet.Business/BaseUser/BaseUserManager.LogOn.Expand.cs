//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserManager 
    /// 用户管理 扩展类 
    /// 
    /// 修改记录
    /// 
    ///		2015.01.25 版本：3.1 SongBiao 扩展登录提醒功能。
    /// 
    /// <author>
    ///		<name>SongBiao</name>
    ///		<date>2015.01.25</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        #region public void SendLogOnRemind(BaseUserInfo userInfo) 向登录用户发送登录提醒消息
        /// <summary>
        /// 宋彪 2015-01-22
        /// 向登录用户发送登录提醒消息
        /// 1、邮件提醒；、2手机短信提醒；3、吉信提醒
        /// 为了避免线程阻塞，使用一个新线程处理提醒消息的发送
        /// 所有超管及IT信息中心的人员全部强制提醒
        /// </summary>
        /// <param name="userInfo">用户登录信息</param>
        public void SendLogOnRemind(BaseUserInfo userInfo)
        {
            System.Threading.ThreadPool.QueueUserWorkItem((System.Threading.WaitCallback)delegate
            {
                try
                {
                    //获取提醒实体信息 提醒要求已设置且启用
                    var systemName = userInfo.SystemCode;
                    var manager = new BaseUserLogonExtendManager();
                    var userLogonRemind = manager.GetEntity(userInfo.Id);
                    var userContactEntity = new BaseUserContactManager().GetEntity(userInfo.Id);
                    var webClient = new WebClient();
                    //提醒对象实体和联系信息实体存在则进行下一步
                    if (userLogonRemind != null && userContactEntity != null)
                    {
                        //发送吉信消息提醒 有唯一账号而且设置了在登录时发送吉信登录提醒
                        if (!string.IsNullOrWhiteSpace(userInfo.NickName) && userLogonRemind.JixinRemind == 1)
                        {
                            //吉信接口地址 
                            var url = "http://jixin.wangcaisoft.com:8280/mng/httpservices/msg-sendMessageToUsers.action";
                            var postValues = new NameValueCollection();
                            //为空则无发送者，客户无回复按钮+(v1.1)
                            postValues.Add("sender", string.Empty);
                            //关闭延迟 默认为30秒 +(v1.1)
                            postValues.Add("closeDelay", "30");
                            //显示延迟 默认为0秒 +(v1.1)
                            postValues.Add("showDelay", "0");
                            //接收者，以逗号分隔，包含中文需使用URL编码
                            // ReSharper disable once AssignNullToNotNullAttribute
                            postValues.Add("receivers", System.Web.HttpUtility.UrlEncode(userInfo.NickName, System.Text.Encoding.UTF8));
                            //显示位置,0表示居中，1表示右下角(默认0)
                            postValues.Add("position", "1");
                            //消息标题
                            postValues.Add("title", "系统账号登录提醒");
                            //消息内容
                            var content = "<div style='word-break:keep-all;'><font color='#FF7E00'>" + userInfo.NickName + "</font>，您的账号于<font color='#FF7E00'>" + DateTime.Now.ToString(BaseSystemInfo.DateTimeFormat) + "</font>登录了<font color='#FF7E00'>" + systemName + "</font></div>"
                                + "<div style='word-break:keep-all;margin-top:5px'>登录IP：<font color='#FF7E00'>" + userInfo.IpAddress + "</font></div>"
                                  + "<div style=' word-break:keep-all;margin-top:5px'>IP参考所在地：<font color='#FF7E00'>" + IpUtil.GetInstance().FindName(userInfo.IpAddress) + "</font></div>"
                                + "<div style=' word-break:keep-all;margin-top:5px'>如果不是您自己登录，请马上联系：021-31165566,或即刻<a href='http://security.wangcaisoft.com' target='_blank'>登录安全中心</a>修改密码。</div>";
                            postValues.Add("content", (string)content);
                            postValues.Add("width", "300");
                            postValues.Add("height", "180");
                            // 向服务器发送POST数据
                            webClient.UploadValues(url, postValues);
                        }
                        //用户邮箱存在，邮箱已经认证而且设置了使用登录时发送邮件提醒
                        if (!string.IsNullOrWhiteSpace(userContactEntity.Email) && userContactEntity.EmailValiated == 1 && userLogonRemind.EmailRemind == 1)
                        {
                            var subject = userInfo.CompanyName + " - " + userInfo.NickName + " 登录" + systemName + " 系统提醒";
                            var body = userInfo.UserName + Environment.NewLine + ":<br/>"
                                + DateTime.Now.ToString(BaseSystemInfo.DateTimeFormat) + "登录了" + systemName + "；<br/>" + Environment.NewLine
                                + "编号：" + userInfo.Code + "；<br/> " + Environment.NewLine
                                + "登录系统：" + systemName + "；<br/> " + Environment.NewLine
                                + "登录IP：" + userInfo.IpAddress + "；<br/> " + Environment.NewLine
                                + "MAC地址：" + userInfo.MacAddress + "；<br/>" + Environment.NewLine
                                + "如果不是您自己登录，请马上联系021-88888888，或即刻登录系统修改密码。";
                            var smtp = new SmtpClient();
                            //邮箱的smtp地址
                            smtp.Host = "mail.wangcaisoft.com";//BaseSystemInfo.MailServer;
                            //端口号
                            smtp.Port = 25;
                            //构建发件人的身份凭据类
                            //smtp.Credentials = new NetworkCredential(BaseSystemInfo.MailUserName, BaseSystemInfo.MailPassword);
                            smtp.Credentials = new NetworkCredential("remind", "remind#@!~");
                            //构建消息类
                            var objMailMessage = new MailMessage();
                            //设置优先级
                            objMailMessage.Priority = MailPriority.High;
                            //消息发送人
                            objMailMessage.From = new MailAddress("remind", "登录提醒", System.Text.Encoding.UTF8);
                            //收件人
                            objMailMessage.To.Add(userContactEntity.Email);
                            //标题
                            objMailMessage.Subject = subject;
                            //标题字符编码
                            objMailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                            //正文
                            objMailMessage.Body = body;
                            objMailMessage.IsBodyHtml = true;
                            //内容字符编码
                            objMailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                            //发送
                            smtp.Send(objMailMessage);
                        }
                        //用户手机存在，已验证，而且设置了登录时发送手机短信提醒 需要对网点扣费
                        if (!string.IsNullOrWhiteSpace(userContactEntity.Mobile) && userContactEntity.MobileValiated == 1 && userLogonRemind.MobileRemind == 1)
                        {
                            //根据朱工建议，增加判断登陆地是否发生变化
                            //获取最近两次的登录记录 按时间降序查询
                            var loginLogManager = new BaseLoginLogManager(userInfo);
                            var loginLogEntities = loginLogManager.GetList<BaseLoginLogEntity>(new KeyValuePair<string, object>(BaseLoginLogEntity.FieldUserId, UserInfo.Id), 2, " " + BaseLoginLogEntity.FieldCreateTime + " DESC ");
                            var ipHelper = new IpUtil();
                            var addressA = ipHelper.FindName(loginLogEntities[0].IpAddress);
                            if (string.IsNullOrWhiteSpace((string)addressA))
                            {
                                addressA = ipHelper.FindName(loginLogEntities[0].IpAddress);
                            }
                            var addressB = ipHelper.FindName(loginLogEntities[1].IpAddress);
                            if (string.IsNullOrWhiteSpace((string)addressB))
                            {
                                addressB = ipHelper.FindName(loginLogEntities[1].IpAddress);
                            }
                            if (loginLogEntities[0] != null
                                && loginLogEntities[1] != null
                                && (!string.Equals(loginLogEntities[0].IpAddress, loginLogEntities[1].IpAddress, StringComparison.OrdinalIgnoreCase)
                                || !string.Equals((string)addressA, (string)addressB, StringComparison.OrdinalIgnoreCase)
                                ))
                            {
                                var url = "http://mas.wangcaisoft.com/WebAPIV42/API/Mobile/SendMessageByCompanyCode";
                                var postValues = new NameValueCollection();
                                postValues.Add("companyCode", userInfo.CompanyCode);
                                postValues.Add("mobiles", userContactEntity.Mobile);
                                var message = userInfo.NickName + "，您好！您的账号于" + DateTime.Now.ToString(BaseSystemInfo.DateTimeFormat) + "登录了" + systemName + "，登录IP：" + userInfo.IpAddress + "，如果不是您自己登录，请马上联系021-31165566，或即刻登录安全中心修改密码。";
                                postValues.Add("message", message);
                                postValues.Add("customerName", userInfo.NickName);
                                webClient.UploadValues(url, postValues);
                            }
                        }
                        //微信提醒
                        if (!string.IsNullOrWhiteSpace(userContactEntity.WeChat) && userContactEntity.WeChatValiated == 1 && userLogonRemind.WechatRemind == 1)
                        {
                            var url = "http://weixin.wangcaisoft.com/Template/WeiXinLogin";
                            var postValues = new NameValueCollection();
                            postValues.Add("first", "您已经成功登录系统");
                            postValues.Add("keyword1", userInfo.NickName);
                            postValues.Add("remark", userInfo.NickName + "，您的账号于" + DateTime.Now.ToString(BaseSystemInfo.DateTimeFormat) + "登录了" + systemName);
                            postValues.Add("OpenId", userContactEntity.WeChat);
                            //postValues.Add("url", "http://security.wangcaisoft.com/changepassword"); 详情的链接
                            webClient.UploadValues(url, postValues);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.WriteLog(userInfo.NickName + "登录提醒消息发送异常：" + ex.Message, "Log");
                }
            });
        }
        #endregion

        #region public int GetRateUserPass(string pass) 密码强度级别
        /// <summary>
        /// 密码强度级别 小于3不允许登录
        /// </summary>
        /// <param name="pass">密码</param>
        /// <returns>强度级别</returns>
        public int GetRateUserPass(string pass)
        {
            /*  
             * 返回值值表示口令等级  
             * 0 不合法口令  
             * 1 太短  
             * 2 弱  
             * 3 一般  
             * 4 很好  
             * 5 极佳  
             */
            var i = 0;
            //if(pass==null || pass.length()==0)
            if (string.IsNullOrWhiteSpace(pass))
            {
                return 0;
            }
            var hasLetter = MatcherLength(pass, "[a-zA-Z]", "");
            var hasNumber = MatcherLength(pass, "[0-9]", "");
            var passLen = pass.Length;
            if (passLen >= 6)
            {
                /* 如果仅包含数字或仅包含字母 */
                if ((passLen - hasLetter) == 0 || (passLen - hasNumber) == 0)
                {
                    if (passLen < 8)
                    {
                        i = 2;
                    }
                    else
                    {
                        i = 3;
                    }
                }
                /* 如果口令大于6位且即包含数字又包含字母 */
                else if (hasLetter > 0 && hasNumber > 0)
                {
                    if (passLen >= 10)
                    {
                        i = 5;
                    }
                    else if (passLen >= 8)
                    {
                        i = 4;
                    }
                    else
                    {
                        i = 3;
                    }
                }
                /* 如果既不包含数字又不包含字母 */
                else if (hasLetter == 0 && hasNumber == 0)
                {
                    if (passLen >= 7)
                    {
                        i = 5;
                    }
                    else
                    {
                        i = 4;
                    }
                }
                /* 字母或数字有一方为0 */
                else if (hasNumber == 0 || hasLetter == 0)
                {
                    if ((passLen - hasLetter) == 0 || (passLen - hasNumber) == 0)
                    {
                        i = 2;
                    }
                    /*   
                     * 字母数字任意一种类型小于6且总长度大于等于6  
                     * 则说明此密码是字母或数字加任意其他字符组合而成  
                     */
                    else
                    {
                        if (passLen > 8)
                        {
                            i = 5;
                        }
                        else if (passLen == 8)
                        {
                            i = 4;
                        }
                        else
                        {
                            i = 3;
                        }
                    }
                }
            }
            else
            { //口令小于6位则显示太短  
                if (passLen > 0)
                {
                    i = 1; //口令太短  
                }
                else
                {
                    i = 0;
                }
            }
            return i;
        }
        #endregion

        #region public int MatcherLength(string str, string cp, string s) 查询匹配长度
        /// <summary>
        /// 查询匹配长度
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="cp">规则</param>
        /// <param name="s"></param>
        /// <returns></returns>
        public int MatcherLength(string str, string cp, string s)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return 0;
            }
            var mc = Regex.Matches(str, cp);
            return mc.Count;
        }
        #endregion
    }
}
