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
    ///		2018.07.06 版本：1.0 Troy.Cui	主键创建。
    ///		
    /// <author>
    ///		<name>Troy Cui</name>
    ///		<date>2018.07.06</date>
    /// </author>
    /// </summary>
    public partial class BaseSystemInfo
    {
        /// <summary>
        /// 主页面
        /// </summary>
        public static string MainPage = "MainTabs.aspx";

        /// <summary>
        /// 是否正式环境
        /// </summary>
        public static bool IsProduction = false;

        /// <summary>
        /// 启用SMS短信
        /// </summary>
        public static bool SmsEnabled = false;

        /// <summary>
        /// 是否启用Windows域身份认证自动登录
        /// </summary>
        public static bool IsWindowsAuthentication = false;

        /// <summary>
        /// 上传附件文件后缀
        /// </summary>
        public static string UploadFileExtension = "doc,docx,xls,xlsx,ppt,pptx,txt,csv,pdf,gif,jpg,jpeg,png,bmp,rar,7z,zip,wma,mp4,msg,apk,ipa";
        /// <summary>
        /// 上传视频文件后缀
        /// </summary>
        public static string UploadVideoExtension = "flv,mp4,avi";
        /// <summary>
        /// 上传音频文件后缀
        /// </summary>
        public static string UploadAudioExtension = "mp3";
        /// <summary>
        /// 用户角色前缀（用于用户自行管理用户和角色）
        /// </summary>
        public static string UserRolePrefix = "";
        /// <summary>
        /// 排除用户角色前缀（用于用户自行管理用户和角色）
        /// </summary>
        public static string ExcludedUserRolePrefix = "Common";
        /// <summary>
        /// 根菜单编码
        /// </summary>
        public static string RootMenuCode = "DotNet.Common";
    }
}