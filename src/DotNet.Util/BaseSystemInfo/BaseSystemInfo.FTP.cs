//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    /// BaseSystemInfo
    /// 这是系统的核心基础信息部分
    /// 
    /// 修改记录
    ///		2021.03.17 版本：1.0 Troy.Cui	主键创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2021.03.17</date>
    /// </author>
    /// </summary>
    public partial class BaseSystemInfo
    {
        /// <summary>
        /// FTP服务器
        /// </summary>
        public static string FtpServer = string.Empty;

        /// <summary>
        /// FTP端口号
        /// </summary>
        public static int FtpPort = 1883;

        /// <summary>
        /// FTP是否启用SSL
        /// </summary>
        public static bool FtpSslEnabled = false;

        /// <summary>
        /// FTP用户名
        /// </summary>
        public static string FtpUserName = string.Empty;

        /// <summary>
        /// FTP密码
        /// </summary>
        public static string FtpPassword = string.Empty;
    }
}