//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;

namespace DotNet.Util
{
    /// <summary>
    /// MailUtil
    /// 邮件发送
    /// 
    /// 修改记录
    ///     2018.10.15 版本：2.0 Troy.Cui	优化多收件人并增加MailServerPort和MailServerEnableSsl。
    ///		2016.07.22 版本：1.0 songbiao	主键创建。
    ///		
    /// <author>
    ///		<name>songbiao</name>
    ///		<date>2016.07.22</date>
    /// </author>
    /// </summary>

    public partial class MailUtil
    {
        /// <summary>
        /// 同步发送邮件
        /// </summary>
        /// <param name="to">收件人邮箱地址（多个用英文,或;分割）</param>
        /// <param name="subject">主题</param>
        /// <param name="body">内容</param>
        /// <param name="attachmentPaths">附件路径</param>
        /// <param name="encoding">编码</param>
        /// <param name="isBodyHtml">是否Html</param>
        /// <returns>是否成功</returns>
        public static bool Send(string to, string subject, string body, string attachmentPaths = null, string encoding = "UTF-8", bool isBodyHtml = true)
        {
            var result = false;
            if (!string.IsNullOrEmpty(to))
            {
                try
                {
                    if(BaseSystemInfo.MailServerSslEnabled && BaseSystemInfo.MailServerPort == 465)
                    {
#if NET40_OR_GREATER
                        var message = new System.Web.Mail.MailMessage();
                        //接收人邮箱地址
                        message.To = to;

                        if (!string.IsNullOrEmpty(BaseSystemInfo.MailBcc))
                        {
                            message.Bcc = BaseSystemInfo.MailBcc;
                        }

                        if (!string.IsNullOrEmpty(BaseSystemInfo.MailFrom))
                        {
                            message.From = BaseSystemInfo.MailFrom;
                        }

                        //在有附件的情况下添加附件
                        if (!string.IsNullOrEmpty(attachmentPaths))
                        {
                            message.Attachments.Clear();
                            try
                            {
                                string[] attachPath = attachmentPaths.Split(';');
                                System.Web.Mail.MailAttachment attachFile = null;
                                foreach (string path in attachPath)
                                {
                                    //string extName = Path.GetExtension(path).ToLower(); //获取扩展名
                                    //FileStream fs = new FileStream(HostingEnvironment.MapPath("/") + pathFileName, FileMode.Open, FileAccess.Read);
                                    ////将附件添加到mailmessage对象  
                                    //attachment = new Attachment(fs, dictionary[i]);
                                    attachFile = new System.Web.Mail.MailAttachment(path);
                                    message.Attachments.Add(attachFile);
                                }
                            }
                            catch (Exception ex)
                            {
                                LogUtil.WriteException(ex);
                            }
                        }
                        message.BodyEncoding = Encoding.GetEncoding(encoding);
                        message.Body = body;
                        message.Subject = subject;
                        if (isBodyHtml)
                        {
                            message.BodyFormat = System.Web.Mail.MailFormat.Html;
                        }

                        message.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");
                        message.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", BaseSystemInfo.MailUserName);
                        message.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", BaseSystemInfo.MailPassword);
                        message.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserverport", BaseSystemInfo.MailServerPort);//端口 
                        message.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpusessl", BaseSystemInfo.MailServerSslEnabled);

                        System.Web.Mail.SmtpMail.SmtpServer = BaseSystemInfo.MailServer;
                        System.Web.Mail.SmtpMail.Send(message);
#endif
                    }
                    else
                    {
                        var message = new MailMessage();
                        //接收人邮箱地址
                        if (!to.Contains(",") && !to.Contains(";"))
                        {
                            message.To.Add(new MailAddress(to));
                        }
                        else if (!string.IsNullOrWhiteSpace(to))
                        {
                            string[] tos = null;
                            if (to.Contains(","))
                            {
                                tos = to.Split(",".ToCharArray());
                            }
                            else if (to.Contains(";"))
                            {
                                tos = to.Split(";".ToCharArray());

                            }

                            foreach (var t in tos)
                            {
                                message.To.Add(new MailAddress(t));
                            }
                        }

                        if (!string.IsNullOrEmpty(BaseSystemInfo.MailBcc))
                        {
                            message.Bcc.Add(new MailAddress(BaseSystemInfo.MailBcc));
                        }

                        if (!string.IsNullOrEmpty(BaseSystemInfo.MailFrom))
                        {
                            message.From = new MailAddress(BaseSystemInfo.MailFrom);
                        }

                        //在有附件的情况下添加附件
                        if (!string.IsNullOrEmpty(attachmentPaths))
                        {
                            message.Attachments.Clear();
                            try
                            {
                                string[] attachPath = attachmentPaths.Split(';');
                                Attachment attachFile = null;
                                foreach (string path in attachPath)
                                {
                                    //string extName = Path.GetExtension(path).ToLower(); //获取扩展名  
                                    //FileStream fs = new FileStream(HostingEnvironment.MapPath("/") + pathFileName, FileMode.Open, FileAccess.Read);
                                    ////将附件添加到mailmessage对象  
                                    //attachment = new Attachment(fs, dictionary[i]);
                                    attachFile = new Attachment(path);
                                    message.Attachments.Add(attachFile);
                                }
                            }
                            catch (Exception ex)
                            {
                                LogUtil.WriteException(ex);
                            }
                        }
                        message.BodyEncoding = Encoding.GetEncoding(encoding);
                        message.Body = body;
                        message.SubjectEncoding = Encoding.GetEncoding(encoding);
                        message.Subject = subject;
                        message.IsBodyHtml = isBodyHtml;

                        var smtpClient = new SmtpClient(BaseSystemInfo.MailServer, BaseSystemInfo.MailServerPort)
                        {
                            Credentials = new NetworkCredential(BaseSystemInfo.MailUserName, BaseSystemInfo.MailPassword),
                            //SSL设置
                            EnableSsl = BaseSystemInfo.MailServerSslEnabled
                        };

                        smtpClient.Send(message);
                    }
                    
                    result = true;
                }
#if NET40_OR_GREATER
                catch (System.Web.HttpException ex)
                {
                    LogUtil.WriteException(ex);
                }
#endif
                catch (SmtpException ex)
                {
                    LogUtil.WriteException(ex);
                }
                catch (Exception ex)
                {
                    LogUtil.WriteException(ex);
                }
            }

            return result;
        }

        /// <summary>
        /// 异步发送邮件 独立线程
        /// </summary>
        /// <param name="to">邮件接收人</param>
        /// <param name="title">邮件标题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="port">端口号</param>
        /// <returns></returns>
        public static void SendByThread(string to, string title, string body, int port = 25)
        {
            new Thread(new ThreadStart(delegate ()
            {
                try
                {
                    var smtp = new SmtpClient();
                    //邮箱的smtp地址
                    smtp.Host = BaseSystemInfo.MailServer;
                    //端口号
                    smtp.Port = port;
                    //构建发件人的身份凭据类
                    smtp.Credentials = new NetworkCredential(BaseSystemInfo.MailUserName, BaseSystemInfo.MailPassword);
                    //构建消息类
                    var objMailMessage = new MailMessage();
                    //设置优先级
                    objMailMessage.Priority = MailPriority.High;
                    //消息发送人
                    objMailMessage.From = new MailAddress(BaseSystemInfo.MailUserName, "登录提醒", Encoding.UTF8);
                    //收件人
                    objMailMessage.To.Add(to);
                    //标题
                    objMailMessage.Subject = title.Trim();
                    //标题字符编码
                    objMailMessage.SubjectEncoding = Encoding.UTF8;
                    //正文
                    objMailMessage.Body = body.Trim();
                    objMailMessage.IsBodyHtml = true;
                    //内容字符编码
                    objMailMessage.BodyEncoding = Encoding.UTF8;
                    //发送
                    smtp.Send(objMailMessage);
                }
                catch (Exception ex)
                {
                    LogUtil.WriteException(ex);
                }

            })).Start();
        }
    }
}
