//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    /// BaseSystemInfo
    /// 这是系统的核心基础信息部分
    /// 
    /// 修改记录
    ///     2018.10.15 版本：4.0 Troy.Cui	增加MailServerPort和MailServerSslEnabled，并分离文件。
    ///     2018.08.15 版本：3.0 Troy.Cui	增加客户网站。
    ///     2016.06.07 版本：2.0 Troy.Cui	增加客户公司名称。
    ///		2012.04.14 版本：1.0 JiRiGaLa	主键创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.04.14</date>
    /// </author>
    /// </summary>
    public partial class BaseSystemInfo
    {
        /// <summary>
        /// 邮件服务器SMTP地址
        /// </summary>
        public static string MailServer = "";

        /// <summary>
        /// SMTP邮件服务器端口号
        /// </summary>
        public static int MailServerPort = 25;

        /// <summary>
        /// SMTP邮件服务器是否启用SSL
        /// </summary>
        public static bool MailServerSslEnabled = false;

        /// <summary>
        /// 用户名
        /// </summary>
        public static string MailUserName = "";

        /// <summary>
        /// 密码
        /// </summary>
        public static string MailPassword = "";

        /// <summary>
        /// 发件人
        /// </summary>
        public static string MailFrom = "";

        /// <summary>
        /// 秘密抄送人
        /// </summary>
        public static string MailBcc = "";

    }
}