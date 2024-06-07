//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    /// BaseSystemInfo
    /// 这是系统的核心基础信息部分
    /// 
    /// 修改记录
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
        /// 当前客户公司编号
        /// </summary>
        public static string CustomerCompanyId = "";
        /// <summary>
        /// 当前客户公司名称
        /// </summary>
        public static string CustomerCompanyName = "";

        /// <summary>
        /// 客户公司电话
        /// </summary>
        public static string CustomerPhone = "";

        /// <summary>
        /// 客户公司网站
        /// </summary>
        public static string CustomerCompanyWebsite = "";

        /// <summary>
        /// 公司名称
        /// </summary>
        public static string CompanyName = "";

        /// <summary>
        /// 公司电话
        /// </summary>
        public static string CompanyPhone = "";

        /// <summary>
        /// 公司网站
        /// </summary>
        public static string CompanyWebsite = "";

        /// <summary>
        /// 发送给谁，用,;符号隔开
        /// </summary>
        public static string ErrorReportTo = "";

        /// <summary>
        /// 错误反馈地址
        /// </summary>
        public static string FeedbackUrl = "";

        /// <summary>
        /// 要求客户注册的信息
        /// </summary>
        public static string RegisterException = "请您购买本软件的授权注册。";
    }
}