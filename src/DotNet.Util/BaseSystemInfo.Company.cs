//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
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
    ///		<name>Troy Cui</name>
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
        public static string CustomerCompanyName = "旺财";

        /// <summary>
        /// 客户公司电话
        /// </summary>
        public static string CustomerPhone = "13818699609";

        /// <summary>
        /// 客户公司网站
        /// </summary>
        public static string CustomerCompanyWebsite = "http://www.WangCaiSoft.com";

        /// <summary>
        /// 公司名称
        /// </summary>
        public static string CompanyName = "旺财软件";

        /// <summary>
        /// 公司电话
        /// </summary>
        public static string CompanyPhone = "13818699609";

        /// <summary>
        /// 公司网站
        /// </summary>
        public static string CompanyWebsite = "http://www.WangCaiSoft.com";

        /// <summary>
        /// 发送给谁，用,;符号隔开
        /// </summary>
        public static string ErrorReportTo = "17185490@qq.com";

        /// <summary>
        /// 错误反馈地址
        /// </summary>
        public static string FeedbackUrl = "http://www.wangcaisoft.com/feedback/";

        /// <summary>
        /// 要求客户注册的信息
        /// </summary>
        public static string RegisterException = "请您联系：旺财软件 崔文远 手机：13818699609 电子邮件：17185490@qq.com 购买本软件的授权注册。";
    }
}