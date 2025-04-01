//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Util
{
    /// <summary>
    /// BaseSystemInfo
    /// 这是系统的核心基础信息部分
    /// 
    /// 修改记录
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
        /// 系统登录后提醒消息
        /// </summary>
        public static string Notice = string.Empty;

        /// <summary>
        /// 客户端数据已经同步好
        /// </summary>
        public static bool Synchronized = false;

        /// <summary>
        /// 客户端数据同步的时间
        /// </summary>
        public static DateTime? SynchronizedTime = null;

        /// <summary>
        /// 本次登录是否手机验证
        /// </summary>
        public static bool MobileValiated = false;

        /// <summary>
        /// 主視窗
        /// </summary>
        public static string MainForm = "FrmRibbonMain";

        /// <summary>
        /// 登录系统的主窗体 
        /// </summary>
        public static string LogonForm = "FrmLogonByCompany";
        /// <summary>
        /// ConfigFile
        /// </summary>
        public static string ConfigFile = "Intranet";

        /// <summary>
        /// 是否启用即时内部消息
        /// </summary>
        public static bool UseMessage = true;

        /// <summary>
        /// 客户端是否同步数据
        /// </summary>
        public static bool Synchronous = true;

        /// <summary>
        /// 检查余额
        /// </summary>
        public static bool CheckBalance = true;

        /// <summary>
        /// 目前用户公司
        /// </summary>
        public static string CurrentCompany = string.Empty;

        /// <summary>
        /// 目前用户名称
        /// </summary>
        public static string CurrentUserName = string.Empty;

        /// <summary>
        /// 唯一用户名
        /// </summary>
        public static string CurrentNickName = string.Empty;

        /// <summary>
        /// 唯一用户名
        /// </summary>
        public static string[] HistoryUsers = new string[0];

        /// <summary>
        /// 目前使用者密碼
        /// </summary>
        public static string CurrentPassword = string.Empty;

        /// <summary>
        /// 是否存储登录密码，默认记住密码
        /// </summary>
        public static bool RememberPassword = true;

        /// <summary>
        /// 是否自动登录，默认不自动登录
        /// </summary>
        public static bool AutoLogon = false;

        /// <summary>
        /// 客户端加密存储密码
        /// </summary>
        public static bool ClientEncryptPassword = true;


        /// <summary>
        /// 组织机构动态加载，当数据量大时可以设置为启用
        /// </summary>
        public static bool OrganizationDynamicLoading = true;

        /// <summary>
        /// 是否使用多語言
        /// </summary>
        public static bool MultiLanguage = true;

        /// <summary>
        /// 目前使用者選擇的語系
        /// </summary>
        public static string CurrentLanguage = "zh-CN";

        /// <summary>
        /// 目前佈景
        /// </summary>
        public static string Themes = string.Empty;

        /// <summary>
        /// 是否採用客戶端緩存
        /// </summary>
        public static bool ClientCache = false;

        private int _lockWaitMinute = 60;

        /// <summary>
        /// 鎖定等待時間分鐘
        /// </summary>
        public int LockWaitMinute
        {
            get => _lockWaitMinute;
            set => _lockWaitMinute = value;
        }

        /// <summary>
        /// 主應用程式集名稱
        /// </summary>
        public static string MainAssembly = "DotNet.WinForm";

        /// <summary>
        /// 是双击打开功能菜单还是单击打开
        /// </summary>
        public static bool IsDoubleOpenModule = false;

        /// <summary>
        /// 上传文件分块大小 KB为单位
        /// </summary>
        public static int UploadBlockSize = 10;

        /// <summary>
        /// 上传文件存储方式 Database Disk
        /// </summary>
        public static string UploadStorageMode = "Database";

        /// <summary>
        /// 上传文件到服务器上的路径
        /// </summary>
        public static string UploadPath = "";

        /// <summary>
        /// 是否在内部打开IE网页
        /// </summary>
        public static bool OpenNewWebWindow = true;
    }
}