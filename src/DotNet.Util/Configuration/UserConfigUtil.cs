//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace DotNet.Util
{
    /// <summary>
    /// UserConfigUtil
    /// 访问用户配置文件的类
    /// 
    /// 修改记录
    ///     2021.03.17 版本：4.0 Troy Cui  新增MQTT、FTP、WebApi的相关配置，并分类获取代码
    ///     2015.07.31 版本：1.5 lhy      增加保存多个历史登录用户的记录功能。
    ///     2011.07.06 版本：1.4 zgl      增加对 CheckIPAddress 的操作
    ///		2008.06.08 版本：1.3 JiRiGaLa 命名修改为 ConfigHelper。
    ///		2008.04.22 版本：1.2 JiRiGaLa 从指定的文件读取配置项。
    ///		2007.07.31 版本：1.1 JiRiGaLa 规范化 FielName 变量。
    ///		2007.04.14 版本：1.0 JiRiGaLa 专门读取注册表的类，主键书写格式改进。
    ///		
    ///	版本：1.2
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2008.04.22</date>
    /// </author> 
    /// </summary>
    public partial class UserConfigUtil
    {
        /// <summary>
        /// LogonTo
        /// </summary>
        public static string LogonTo = "Config";
        /// <summary>
        /// 配置文件名
        /// </summary>
        public static string FileName => LogonTo + ".xml";
        /// <summary>
        /// 选择路径
        /// </summary>
        public static string SelectPath = "//appSettings/add";
        /// <summary>
        /// 配置文件名
        /// </summary>
        public static string ConfigFileName
        {
            get
            {
                var fileName = FileName;
                if (!string.IsNullOrEmpty(BaseSystemInfo.StartupPath))
                {
                    fileName = BaseSystemInfo.StartupPath + "\\" + FileName;
                }
                return fileName;
                // return Application.StartupPath + "\\" + FielName;
            }
        }

        #region public static Dictionary<String, String> GetLogonTo() 获取配置文件选项
        /// <summary>
        /// 获取配置文件选项
        /// </summary>
        /// <returns>配置文件设置</returns>
        public static Dictionary<String, String> GetLogonTo()
        {
            var result = new Dictionary<String, String>();
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(ConfigFileName);
            var xmlNodeList = xmlDocument.SelectNodes(SelectPath);
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                if (xmlNode.Attributes["key"].Value.ToUpper().Equals("LogonTo".ToUpper()))
                {
                    result.Add(xmlNode.Attributes["value"].Value, xmlNode.Attributes["dispaly"].Value);
                }
            }
            return result;
        }
        #endregion      

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Exists(string key)
        {
            return !string.IsNullOrEmpty(GetValue(key));
        }
        /// <summary>
        /// 获取选项值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string[] GetOptions(string key)
        {
            var option = string.Empty;
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(ConfigFileName);
            option = GetOption(xmlDocument, SelectPath, key);
            return option.Split(',').Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
        }

        #region public static string GetOption(XmlDocument xmlDocument, string selectPath, string key) 设置配置项
        /// <summary>
        /// 读取配置项
        /// </summary>
        /// <param name="xmlDocument">配置文件</param>
        /// <param name="selectPath">查询条件</param>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public static string GetOption(XmlDocument xmlDocument, string selectPath, string key)
        {
            var result = string.Empty;
            var xmlNodeList = xmlDocument.SelectNodes(selectPath);
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                if (xmlNode.Attributes["key"].Value.ToUpper().Equals(key.ToUpper()))
                {
                    if (xmlNode.Attributes["Options"] != null)
                    {
                        result = xmlNode.Attributes["Options"].Value;
                        break;
                    }
                }
            }
            return result;
        }
        #endregion

        #region public static string GetValue(string key) 读取配置项

        /// <summary>
        /// 读取配置项
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="encrypt">是否加密</param>
        /// <returns>值</returns>
        public static string GetValue(string key, bool encrypt = false)
        {
            var result = string.Empty;
            result = GetValue(_xmlDocument, SelectPath, key);
            if (!string.IsNullOrEmpty(result) && encrypt)
            {
                result = SecretUtil.DesDecrypt(result);
            }
            return result;
        }
        #endregion

        #region public static string GetValue(string fileName, string key) 读取配置项
        /// <summary>
        /// 读取配置项
        /// </summary>
        /// <param name="fileName">配置文件</param>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public static string GetValue(string fileName, string key)
        {
            return GetValue(fileName, SelectPath, key);
        }
        #endregion

        #region public static string GetValue(string fileName, string selectPath, string key) 设置配置项
        /// <summary>
        /// 读取配置项
        /// </summary>
        /// <param name="fileName">配置文件</param>
        /// <param name="selectPath">查询条件</param>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public static string GetValue(string fileName, string selectPath, string key)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);
            return GetValue(xmlDocument, selectPath, key);
        }
        #endregion

        #region public static string GetValue(XmlDocument xmlDocument, string key) 读取配置项
        /// <summary>
        /// 读取配置项
        /// </summary>
        /// <param name="xmlDocument">配置文件</param>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public static string GetValue(XmlDocument xmlDocument, string key)
        {
            return GetValue(xmlDocument, SelectPath, key);
        }
        #endregion

        #region public static string GetValue(XmlDocument xmlDocument, string selectPath, string key) 设置配置项
        /// <summary>
        /// 读取配置项
        /// </summary>
        /// <param name="xmlDocument">配置文件</param>
        /// <param name="selectPath">查询条件</param>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public static string GetValue(XmlDocument xmlDocument, string selectPath, string key)
        {
            var result = string.Empty;
            var xmlNodeList = xmlDocument.SelectNodes(selectPath);
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                if (xmlNode.Attributes["key"].Value.ToUpper().Equals(key.ToUpper()))
                {
                    result = xmlNode.Attributes["value"].Value;
                    break;
                }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <returns></returns>
        public static bool Exists()
        {
            return File.Exists(ConfigFileName);
        }

        #region public static void GetConfig() 读取配置文件
        /// <summary>
        /// 读取配置文件
        /// </summary>
        public static void GetConfig()
        {
            if (Exists())
            {
                var fileName = ConfigFileName;
                if (!string.IsNullOrEmpty(BaseSystemInfo.StartupPath))
                {
                    fileName = BaseSystemInfo.StartupPath + "\\" + ConfigFileName;
                }
                GetConfig(fileName);
            }
        }
        #endregion

        private static XmlDocument _xmlDocument = new XmlDocument();

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="stream"></param>
        public static void GetConfig(Stream stream)
        {
            _xmlDocument.Load(stream);
            GetConfig(_xmlDocument);
        }

        /// <summary>
        /// 从指定的文件读取配置项
        /// </summary>
        /// <param name="fileName">配置文件</param>
        public static void GetConfig(string fileName)
        {
            _xmlDocument.Load(fileName);
            GetConfig(_xmlDocument);
        }

        #region public static void GetConfig() 从指定的文件读取配置项
        /// <summary>
        /// 从指定的文件读取配置项
        /// </summary>
        public static void GetConfig(XmlDocument document)
        {
            _xmlDocument = document;

            #region 获取Redis配置
            if (Exists("RedisServer"))
            {
                BaseSystemInfo.RedisServer = GetValue(_xmlDocument, "RedisServer");
            }
            if (Exists("RedisPort"))
            {
                if (ValidateUtil.IsInt(GetValue(_xmlDocument, "RedisPort")))
                {
                    BaseSystemInfo.RedisPort = int.Parse(GetValue(_xmlDocument, "RedisPort"));
                }
            }
            if (Exists("RedisInitialDb"))
            {
                if (ValidateUtil.IsInt(GetValue(_xmlDocument, "RedisInitialDb")))
                {
                    BaseSystemInfo.RedisInitialDb = int.Parse(GetValue(_xmlDocument, "RedisInitialDb"));
                }
            }
            if (Exists("RedisSslEnabled"))
            {
                BaseSystemInfo.RedisSslEnabled = GetValue(_xmlDocument, "RedisSslEnabled").Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (Exists("RedisUserName"))
            {
                BaseSystemInfo.RedisUserName = GetValue(_xmlDocument, "RedisUserName");
            }
            if (Exists("RedisPassword"))
            {
                BaseSystemInfo.RedisPassword = GetValue(_xmlDocument, "RedisPassword");
            }
            if (Exists("RedisCacheMillisecond"))
            {
                if (ValidateUtil.IsInt(GetValue(_xmlDocument, "RedisCacheMillisecond")))
                {
                    BaseSystemInfo.RedisCacheMillisecond = int.Parse(GetValue(_xmlDocument, "RedisCacheMillisecond"));
                }
            }
            if (Exists("MemoryCacheMillisecond"))
            {
                if (ValidateUtil.IsInt(GetValue(_xmlDocument, "MemoryCacheMillisecond")))
                {
                    BaseSystemInfo.MemoryCacheMillisecond = int.Parse(GetValue(_xmlDocument, "MemoryCacheMillisecond"));
                }
            }
            #endregion

            #region 获取FTP配置
            if (Exists("FtpServer"))
            {
                BaseSystemInfo.FtpServer = GetValue(_xmlDocument, "FtpServer");
            }
            if (Exists("FtpPort"))
            {
                if (ValidateUtil.IsInt(GetValue(_xmlDocument, "FtpPort")))
                {
                    BaseSystemInfo.FtpPort = int.Parse(GetValue(_xmlDocument, "FtpPort"));
                }
            }
            if (Exists("FtpSslEnabled"))
            {
                BaseSystemInfo.FtpSslEnabled = GetValue(_xmlDocument, "FtpSslEnabled").Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (Exists("FtpUserName"))
            {
                BaseSystemInfo.FtpUserName = GetValue(_xmlDocument, "FtpUserName");
            }
            if (Exists("FtpPassword"))
            {
                BaseSystemInfo.FtpPassword = GetValue(_xmlDocument, "FtpPassword");
            }
            #endregion

            #region 获取MQTT配置
            if (Exists("MqttServer"))
            {
                BaseSystemInfo.MqttServer = GetValue(_xmlDocument, "MqttServer");
            }
            if (Exists("MqttPort"))
            {
                if (ValidateUtil.IsInt(GetValue(_xmlDocument, "MqttPort")))
                {
                    BaseSystemInfo.MqttPort = int.Parse(GetValue(_xmlDocument, "MqttPort"));
                }
            }
            if (Exists("MqttSslEnabled"))
            {
                BaseSystemInfo.MqttSslEnabled = GetValue(_xmlDocument, "MqttSslEnabled").Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (Exists("MqttUserName"))
            {
                BaseSystemInfo.MqttUserName = GetValue(_xmlDocument, "MqttUserName");
            }
            if (Exists("MqttPassword"))
            {
                BaseSystemInfo.MqttPassword = GetValue(_xmlDocument, "MqttPassword");
            }
            #endregion

            #region 获取WebApi配置
            if (Exists("WebApiMonitorEnabled"))
            {
                BaseSystemInfo.WebApiMonitorEnabled = GetValue(_xmlDocument, "WebApiMonitorEnabled").Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (Exists("WebApiSlowMonitorEnabled"))
            {
                BaseSystemInfo.WebApiSlowMonitorEnabled = GetValue(_xmlDocument, "WebApiSlowMonitorEnabled").Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (Exists("WebApiSlowResponseMilliseconds"))
            {
                if (ValidateUtil.IsInt(GetValue(_xmlDocument, "WebApiSlowResponseMilliseconds")))
                {
                    BaseSystemInfo.WebApiSlowResponseMilliseconds = int.Parse(GetValue(_xmlDocument, "WebApiSlowResponseMilliseconds"));
                }
            }
            #endregion

            #region 获取Cookie配置
            // 客户信息配置
            if (Exists("CookieName"))
            {
                BaseSystemInfo.CookieName = GetValue(_xmlDocument, "CookieName");
            }
            if (Exists("CookieDomain"))
            {
                BaseSystemInfo.CookieDomain = GetValue(_xmlDocument, "CookieDomain");
            }
            if (Exists("CookieExpires"))
            {
                if (ValidateUtil.IsInt(GetValue(_xmlDocument, "CookieExpires")))
                {
                    BaseSystemInfo.CookieExpires = int.Parse(GetValue(_xmlDocument, "CookieExpires"));
                }
            }
            #endregion

            if (Exists("ConfigFile"))
            {
                BaseSystemInfo.ConfigFile = GetValue(_xmlDocument, "ConfigFile");
            }
            if (Exists("Host"))
            {
                BaseSystemInfo.Host = GetValue(_xmlDocument, "Host");
            }
            if (Exists("Port"))
            {
                int.TryParse(GetValue(_xmlDocument, "Port"), out BaseSystemInfo.Port);
            }
            if (Exists("MobileHost"))
            {
                if (!string.IsNullOrWhiteSpace(GetValue(_xmlDocument, "MobileHost")))
                {
                    BaseSystemInfo.MobileHost = GetValue(_xmlDocument, "MobileHost");
                }
            }
            // 客户信息配置
            if (Exists("NeedRegister"))
            {
                BaseSystemInfo.NeedRegister = (string.Compare(GetValue(_xmlDocument, "NeedRegister"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            // 客户信息配置
            if (Exists("CurrentCompany"))
            {
                BaseSystemInfo.CurrentCompany = GetValue(_xmlDocument, "CurrentCompany");
            }
            if (Exists("CurrentUserName"))
            {
                BaseSystemInfo.CurrentUserName = GetValue(_xmlDocument, "CurrentUserName");
            }
            if (Exists("CurrentNickName"))
            {
                BaseSystemInfo.CurrentNickName = GetValue(_xmlDocument, "CurrentNickName");
            }
            if (Exists("CurrentPassword"))
            {
                BaseSystemInfo.CurrentPassword = GetValue(_xmlDocument, "CurrentPassword");
            }
            //历史登录用户记录
            if (Exists("HistoryUsers"))
            {
                var strUserList = GetValue(_xmlDocument, "HistoryUsers");
                var userInfoArray = strUserList.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                BaseSystemInfo.HistoryUsers = userInfoArray;
            }
            if (Exists("MultiLanguage"))
            {
                BaseSystemInfo.MultiLanguage = (string.Compare(GetValue(_xmlDocument, "MultiLanguage"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("CurrentLanguage"))
            {
                BaseSystemInfo.CurrentLanguage = GetValue(_xmlDocument, "CurrentLanguage");
            }
            if (Exists("RememberPassword"))
            {
                BaseSystemInfo.RememberPassword = (string.Compare(GetValue(_xmlDocument, "RememberPassword"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("OnInternet"))
            {
                BaseSystemInfo.OnInternet = (string.Compare(GetValue(_xmlDocument, "OnInternet"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("AutoLogon"))
            {
                BaseSystemInfo.AutoLogon = (string.Compare(GetValue(_xmlDocument, "AutoLogon"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("ClientEncryptPassword"))
            {
                BaseSystemInfo.ClientEncryptPassword = (string.Compare(GetValue(_xmlDocument, "ClientEncryptPassword"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("ServerEncryptPassword"))
            {
                BaseSystemInfo.ServerEncryptPassword = (string.Compare(GetValue(_xmlDocument, "ServerEncryptPassword"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }

            if (Exists("OpenNewWebWindow"))
            {
                BaseSystemInfo.OpenNewWebWindow = (string.Compare(GetValue(_xmlDocument, "OpenNewWebWindow"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }

            // add by zgl
            if (Exists("CheckIPAddress"))
            {
                BaseSystemInfo.CheckIpAddress = (string.Compare(GetValue(_xmlDocument, "CheckIPAddress"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("LogException"))
            {
                BaseSystemInfo.LogException = (string.Compare(GetValue(_xmlDocument, "LogException"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("LogSql"))
            {
                BaseSystemInfo.LogSql = (string.Compare(GetValue(_xmlDocument, "LogSql"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("LogCache"))
            {
                BaseSystemInfo.LogCache = (string.Compare(GetValue(_xmlDocument, "LogCache"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("EventLog"))
            {
                BaseSystemInfo.EventLog = (string.Compare(GetValue(_xmlDocument, "EventLog"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("CheckOnline"))
            {
                BaseSystemInfo.CheckOnline = (string.Compare(GetValue(_xmlDocument, "CheckOnline"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("UseMessage"))
            {
                BaseSystemInfo.UseMessage = (string.Compare(GetValue(_xmlDocument, "UseMessage"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("Synchronous"))
            {
                BaseSystemInfo.Synchronous = (string.Compare(GetValue(_xmlDocument, "Synchronous"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("CheckBalance"))
            {
                BaseSystemInfo.CheckBalance = (string.Compare(GetValue(_xmlDocument, "CheckBalance"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("ForceHttps"))
            {
                BaseSystemInfo.ForceHttps = (string.Compare(GetValue(_xmlDocument, "ForceHttps"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("AllowUserRegister"))
            {
                BaseSystemInfo.AllowUserRegister = (string.Compare(GetValue(_xmlDocument, "AllowUserRegister"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            // 用户名强制手机号
            if (Exists("UserNameForceMobileNumber"))
            {
                BaseSystemInfo.UserNameForceMobileNumber = GetValue(_xmlDocument, "UserNameForceMobileNumber").Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            // 用户名强制大小写
            if (Exists("UserNameMatchCase"))
            {
                BaseSystemInfo.UserNameMatchCase = GetValue(_xmlDocument, "UserNameMatchCase").Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (Exists("RecordLog"))
            {
                BaseSystemInfo.RecordLog = (string.Compare(GetValue(_xmlDocument, "RecordLog"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("CustomerCompanyId"))
            {
                BaseSystemInfo.CustomerCompanyId = GetValue(_xmlDocument, "CustomerCompanyId");
            }
            if (Exists("CustomerCompanyName"))
            {
                BaseSystemInfo.CustomerCompanyName = GetValue(_xmlDocument, "CustomerCompanyName");
            }
            if (Exists("CompanyName"))
            {
                BaseSystemInfo.CompanyName = GetValue(_xmlDocument, "CompanyName");
            }
            if (Exists("CustomerCompanyWebsite"))
            {
                BaseSystemInfo.CustomerCompanyWebsite = GetValue(_xmlDocument, "CustomerCompanyWebsite");
            }
            if (Exists("CompanyWebsite"))
            {
                BaseSystemInfo.CompanyWebsite = GetValue(_xmlDocument, "CompanyWebsite");
            }
            if (Exists("ConfigurationFrom"))
            {
                BaseSystemInfo.ConfigurationFrom = BaseConfiguration.GetConfiguration(GetValue(_xmlDocument, "ConfigurationFrom"));
            }
            if (Exists("TimeFormat"))
            {
                BaseSystemInfo.TimeFormat = GetValue(_xmlDocument, "TimeFormat");
            }
            if (Exists("DateFormat"))
            {
                BaseSystemInfo.DateFormat = GetValue(_xmlDocument, "DateFormat");
            }
            if (Exists("DateTimeFormat"))
            {
                BaseSystemInfo.DateTimeFormat = GetValue(_xmlDocument, "DateTimeFormat");
            }
            if (Exists("DateTimeLongFormat"))
            {
                BaseSystemInfo.DateTimeLongFormat = GetValue(_xmlDocument, "DateTimeLongFormat");
            }
            if (Exists("SoftName"))
            {
                BaseSystemInfo.SoftName = GetValue(_xmlDocument, "SoftName");
            }
            if (Exists("SoftFullName"))
            {
                BaseSystemInfo.SoftFullName = GetValue(_xmlDocument, "SoftFullName");
            }

            if (Exists("SystemCode"))
            {
                BaseSystemInfo.SystemCode = GetValue(_xmlDocument, "SystemCode");
            }

            if (Exists("OnlineTimeout"))
            {
                if (ValidateUtil.IsInt(GetValue(_xmlDocument, "OnlineTimeout")))
                {
                    BaseSystemInfo.OnlineTimeout = int.Parse(GetValue(_xmlDocument, "OnlineTimeout"));
                }

            }
            if (Exists("Version"))
            {
                BaseSystemInfo.Version = GetValue(_xmlDocument, "Version");
            }

            if (Exists("UseOrganizationPermission"))
            {
                BaseSystemInfo.UseOrganizationPermission = (string.Compare(GetValue(_xmlDocument, "UseOrganizationPermission"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("UseUserPermission"))
            {
                BaseSystemInfo.UseUserPermission = (string.Compare(GetValue(_xmlDocument, "UseUserPermission"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("UseTableColumnPermission"))
            {
                BaseSystemInfo.UseTableColumnPermission = (string.Compare(GetValue(_xmlDocument, "UseTableColumnPermission"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("UseTableScopePermission"))
            {
                BaseSystemInfo.UseTableScopePermission = (string.Compare(GetValue(_xmlDocument, "UseTableScopePermission"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("UsePermissionScope"))
            {
                BaseSystemInfo.UsePermissionScope = (string.Compare(GetValue(_xmlDocument, "UsePermissionScope"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("UseAuthorizationScope"))
            {
                BaseSystemInfo.UseAuthorizationScope = (string.Compare(GetValue(_xmlDocument, "UseAuthorizationScope"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("HandwrittenSignature"))
            {
                BaseSystemInfo.HandwrittenSignature = (string.Compare(GetValue(_xmlDocument, "HandwrittenSignature"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }

            //if (Exists("LoadAllUser"))
            //{
            //    BaseSystemInfo.LoadAllUser = (string.Compare(GetValue(xmlDocument, "LoadAllUser"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            //}

            if (Exists("Service"))
            {
                BaseSystemInfo.Service = GetValue(_xmlDocument, "Service");
            }
            if (Exists("LogonForm"))
            {
                BaseSystemInfo.LogonForm = GetValue(_xmlDocument, "LogonForm");
            }
            if (Exists("MainForm"))
            {
                BaseSystemInfo.MainForm = GetValue(_xmlDocument, "MainForm");
            }
            if (Exists("OnlineLimit"))
            {
                int.TryParse(GetValue(_xmlDocument, "OnlineLimit"), out BaseSystemInfo.OnlineLimit);
            }
            if (Exists("SlowQueryMilliseconds"))
            {
                int.TryParse(GetValue(_xmlDocument, "SlowQueryMilliseconds"), out BaseSystemInfo.SlowQueryMilliseconds);
            }
            if (Exists("UserCenterDbType"))
            {
                BaseSystemInfo.UserCenterDbType = DbTypeUtil.GetDbType(GetValue(_xmlDocument, "UserCenterDbType"));
                BaseSystemInfo.MessageDbType = BaseSystemInfo.UserCenterDbType;
                BaseSystemInfo.BusinessDbType = BaseSystemInfo.UserCenterDbType;
                BaseSystemInfo.WorkFlowDbType = BaseSystemInfo.UserCenterDbType;
                BaseSystemInfo.LogonLogDbType = BaseSystemInfo.UserCenterDbType;
            }
            // 打开数据库连接
            if (Exists("MessageDbType"))
            {
                BaseSystemInfo.MessageDbType = DbTypeUtil.GetDbType(GetValue(_xmlDocument, "MessageDbType"));
            }
            if (Exists("WorkFlowDbType"))
            {
                BaseSystemInfo.WorkFlowDbType = DbTypeUtil.GetDbType(GetValue(_xmlDocument, "WorkFlowDbType"));
            }
            if (Exists("BusinessDbType"))
            {
                BaseSystemInfo.BusinessDbType = DbTypeUtil.GetDbType(GetValue(_xmlDocument, "BusinessDbType"));
            }
            if (Exists("LogonLogDbType"))
            {
                BaseSystemInfo.LogonLogDbType = DbTypeUtil.GetDbType(GetValue(_xmlDocument, "LogonLogDbType"));
            }
            if (Exists("WebAppDbType"))
            {
                BaseSystemInfo.WebAppDbType = DbTypeUtil.GetDbType(GetValue(_xmlDocument, "WebAppDbType"));
            }
            if (Exists("BPMDbType"))
            {
                BaseSystemInfo.BpmDbType = DbTypeUtil.GetDbType(GetValue(_xmlDocument, "BPMDbType"));
            }
            if (Exists("ERPDbType"))
            {
                BaseSystemInfo.ErpDbType = DbTypeUtil.GetDbType(GetValue(_xmlDocument, "ERPDbType"));
            }
            if (Exists("MESDbType"))
            {
                BaseSystemInfo.MesDbType = DbTypeUtil.GetDbType(GetValue(_xmlDocument, "MESDbType"));
            }
            if (Exists("HRMDbType"))
            {
                BaseSystemInfo.HrmDbType = DbTypeUtil.GetDbType(GetValue(_xmlDocument, "HRMDbType"));
            }
            if (Exists("CRMDbType"))
            {
                BaseSystemInfo.CrmDbType = DbTypeUtil.GetDbType(GetValue(_xmlDocument, "CRMDbType"));
            }
            if (Exists("OADbType"))
            {
                BaseSystemInfo.OaDbType = DbTypeUtil.GetDbType(GetValue(_xmlDocument, "OADbType"));
            }
            if (Exists("LabelDbType"))
            {
                BaseSystemInfo.LabelDbType = DbTypeUtil.GetDbType(GetValue(_xmlDocument, "LabelDbType"));
            }
            if (Exists("WebDbType"))
            {
                BaseSystemInfo.WebDbType = DbTypeUtil.GetDbType(GetValue(_xmlDocument, "WebDbType"));
            }
            if (Exists("CmsDbType"))
            {
                BaseSystemInfo.CmsDbType = DbTypeUtil.GetDbType(GetValue(_xmlDocument, "CmsDbType"));
            }
            if (Exists("DTcmsDbType"))
            {
                BaseSystemInfo.DTcmsDbType = DbTypeUtil.GetDbType(GetValue(_xmlDocument, "DTcmsDbType"));
            }
            if (Exists("FlowPortalDbType"))
            {
                BaseSystemInfo.FlowPortalDbType = DbTypeUtil.GetDbType(GetValue(_xmlDocument, "FlowPortalDbType"));
            }
            if (Exists("CustomerPortalDbType"))
            {
                BaseSystemInfo.CustomerPortalDbType = DbTypeUtil.GetDbType(GetValue(_xmlDocument, "CustomerPortalDbType"));
            }
            if (Exists("SupplierPortalDbType"))
            {
                BaseSystemInfo.SupplierPortalDbType = DbTypeUtil.GetDbType(GetValue(_xmlDocument, "SupplierPortalDbType"));
            }
            if (Exists("UserCenterDbConnection"))
            {
                BaseSystemInfo.UserCenterDbConnectionString = GetValue(_xmlDocument, "UserCenterDbConnection");
                BaseSystemInfo.UserCenterReadDbConnection = BaseSystemInfo.UserCenterDbConnectionString;
                BaseSystemInfo.UserCenterWriteDbConnection = BaseSystemInfo.UserCenterDbConnectionString;
                BaseSystemInfo.MessageDbConnectionString = BaseSystemInfo.UserCenterDbConnectionString;
                BaseSystemInfo.LogonLogDbConnectionString = BaseSystemInfo.UserCenterDbConnectionString;
                BaseSystemInfo.BusinessDbConnectionString = BaseSystemInfo.UserCenterDbConnectionString;
                // BaseSystemInfo.WorkFlowDbConnectionString = BaseSystemInfo.UserCenterDbConnectionString;
            }

            if (Exists("OrganizationDynamicLoading"))
            {
                BaseSystemInfo.OrganizationDynamicLoading = (string.Compare(GetValue(_xmlDocument, "OrganizationDynamicLoading"), "TRUE", true, CultureInfo.CurrentCulture) == 0);
            }
            if (Exists("MessageDbConnection"))
            {
                BaseSystemInfo.MessageDbConnectionString = GetValue(_xmlDocument, "MessageDbConnection");
            }
            if (Exists("LogonLogDbConnection"))
            {
                BaseSystemInfo.LogonLogDbConnectionString = GetValue(_xmlDocument, "LogonLogDbConnection");
            }
            if (Exists("WorkFlowDbConnection"))
            {
                BaseSystemInfo.WorkFlowDbConnectionString = GetValue(_xmlDocument, "WorkFlowDbConnection");
            }
            if (Exists("BusinessDbConnection"))
            {
                BaseSystemInfo.BusinessDbConnectionString = GetValue(_xmlDocument, "BusinessDbConnection");
            }
            if (Exists("WebAppDbConnection"))
            {
                BaseSystemInfo.WebAppDbConnectionString = GetValue(_xmlDocument, "WebAppDbConnection");
            }
            if (Exists("BPMDbConnection"))
            {
                BaseSystemInfo.BpmDbConnectionString = GetValue(_xmlDocument, "BPMDbConnection");
            }
            if (Exists("ERPDbConnection"))
            {
                BaseSystemInfo.ErpDbConnectionString = GetValue(_xmlDocument, "ERPDbConnection");
            }
            if (Exists("WMSDbConnection"))
            {
                BaseSystemInfo.WmsDbConnectionString = GetValue(_xmlDocument, "WMSDbConnection");
            }
            if (Exists("MESDbConnection"))
            {
                BaseSystemInfo.MesDbConnectionString = GetValue(_xmlDocument, "MESDbConnection");
            }
            if (Exists("HRMDbConnection"))
            {
                BaseSystemInfo.HrmDbConnectionString = GetValue(_xmlDocument, "HRMDbConnection");
            }
            if (Exists("CRMDbConnection"))
            {
                BaseSystemInfo.CrmDbConnectionString = GetValue(_xmlDocument, "CRMDbConnection");
            }
            if (Exists("OADbConnection"))
            {
                BaseSystemInfo.OaDbConnectionString = GetValue(_xmlDocument, "OADbConnection");
            }
            if (Exists("LabelDbConnection"))
            {
                BaseSystemInfo.LabelDbConnectionString = GetValue(_xmlDocument, "LabelDbConnection");
            }
            if (Exists("WebDbConnection"))
            {
                BaseSystemInfo.WebDbConnectionString = GetValue(_xmlDocument, "WebDbConnection");
            }
            if (Exists("CMSDbConnection"))
            {
                BaseSystemInfo.CmsDbConnectionString = GetValue(_xmlDocument, "CMSDbConnection");
            }
            if (Exists("DTcmsDbConnection"))
            {
                BaseSystemInfo.DTcmsDbConnectionString = GetValue(_xmlDocument, "DTcmsDbConnection");
            }
            if (Exists("FlowPortalDbConnection"))
            {
                BaseSystemInfo.FlowPortalDbConnectionString = GetValue(_xmlDocument, "FlowPortalDbConnection");
            }
            if (Exists("DealerPortalDbConnection"))
            {
                BaseSystemInfo.DealerPortalDbConnectionString = GetValue(_xmlDocument, "DealerPortalDbConnection");
            }
            if (Exists("CustomerPortalDbConnection"))
            {
                BaseSystemInfo.CustomerPortalDbConnectionString = GetValue(_xmlDocument, "CustomerPortalDbConnection");
            }
            if (Exists("SupplierPortalDbConnection"))
            {
                BaseSystemInfo.SupplierPortalDbConnectionString = GetValue(_xmlDocument, "SupplierPortalDbConnection");
            }
            if (Exists("ReportDbConnection"))
            {
                BaseSystemInfo.ReportDbConnectionString = GetValue(_xmlDocument, "ReportDbConnection");
            }
            if (Exists("ScmDbConnection"))
            {
                BaseSystemInfo.ScmDbConnectionString = GetValue(_xmlDocument, "ScmDbConnection");
            }
            if (Exists("ImsDbConnection"))
            {
                BaseSystemInfo.ImsDbConnectionString = GetValue(_xmlDocument, "ImsDbConnection");
            }
            if (Exists("OmsDbConnection"))
            {
                BaseSystemInfo.OmsDbConnectionString = GetValue(_xmlDocument, "OmsDbConnection");
            }
            if (Exists("MemberDbConnection"))
            {
                BaseSystemInfo.MemberDbConnectionString = GetValue(_xmlDocument, "MemberDbConnection");
            }
            BaseSystemInfo.UserCenterDbConnection = BaseSystemInfo.UserCenterDbConnectionString;
            BaseSystemInfo.LogonLogDbConnection = BaseSystemInfo.LogonLogDbConnectionString;
            BaseSystemInfo.MessageDbConnection = BaseSystemInfo.MessageDbConnectionString;
            BaseSystemInfo.BusinessDbConnection = BaseSystemInfo.BusinessDbConnectionString;
            BaseSystemInfo.WorkFlowDbConnection = BaseSystemInfo.WorkFlowDbConnectionString;
            BaseSystemInfo.WebAppDbConnection = BaseSystemInfo.WebAppDbConnectionString;
            BaseSystemInfo.BpmDbConnection = BaseSystemInfo.BpmDbConnectionString;
            BaseSystemInfo.ErpDbConnection = BaseSystemInfo.ErpDbConnectionString;
            BaseSystemInfo.WmsDbConnection = BaseSystemInfo.WmsDbConnectionString;
            BaseSystemInfo.MesDbConnection = BaseSystemInfo.MesDbConnectionString;
            BaseSystemInfo.HrmDbConnection = BaseSystemInfo.HrmDbConnectionString;
            BaseSystemInfo.CrmDbConnection = BaseSystemInfo.CrmDbConnectionString;
            BaseSystemInfo.OaDbConnection = BaseSystemInfo.OaDbConnectionString;
            BaseSystemInfo.LabelDbConnection = BaseSystemInfo.LabelDbConnectionString;
            BaseSystemInfo.WebDbConnection = BaseSystemInfo.WebDbConnectionString;
            BaseSystemInfo.CmsDbConnection = BaseSystemInfo.CmsDbConnectionString;
            BaseSystemInfo.DTcmsDbConnection = BaseSystemInfo.DTcmsDbConnectionString;
            BaseSystemInfo.FlowPortalDbConnection = BaseSystemInfo.FlowPortalDbConnectionString;
            BaseSystemInfo.DealerPortalDbConnection = BaseSystemInfo.DealerPortalDbConnectionString;
            BaseSystemInfo.CustomerPortalDbConnection = BaseSystemInfo.CustomerPortalDbConnectionString;
            BaseSystemInfo.SupplierPortalDbConnection = BaseSystemInfo.SupplierPortalDbConnectionString;
            BaseSystemInfo.ReportDbConnection = BaseSystemInfo.ReportDbConnectionString;
            BaseSystemInfo.ScmDbConnection = BaseSystemInfo.ScmDbConnectionString;
            BaseSystemInfo.ImsDbConnection = BaseSystemInfo.ImsDbConnectionString;
            BaseSystemInfo.OmsDbConnection = BaseSystemInfo.OmsDbConnectionString;
            BaseSystemInfo.MemberDbConnection = BaseSystemInfo.MemberDbConnectionString;

            if (Exists("EncryptDbConnection"))
            {
                BaseSystemInfo.EncryptDbConnection = (string.Compare(GetValue(_xmlDocument, "EncryptDbConnection"), "TRUE", true, CultureInfo.CurrentCulture) == 0);

                if (BaseSystemInfo.EncryptDbConnection)
                {
                    BaseSystemInfo.UserCenterDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.UserCenterDbConnectionString);
                    BaseSystemInfo.LogonLogDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.LogonLogDbConnectionString);
                    BaseSystemInfo.MessageDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.MessageDbConnectionString);
                    BaseSystemInfo.BusinessDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.BusinessDbConnectionString);
                    BaseSystemInfo.WorkFlowDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.WorkFlowDbConnectionString);
                    BaseSystemInfo.WebAppDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.WebAppDbConnectionString);
                    BaseSystemInfo.BpmDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.BpmDbConnectionString);
                    BaseSystemInfo.ErpDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.ErpDbConnectionString);
                    BaseSystemInfo.WmsDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.WmsDbConnectionString);
                    BaseSystemInfo.MesDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.MesDbConnectionString);
                    BaseSystemInfo.HrmDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.HrmDbConnectionString);
                    BaseSystemInfo.CrmDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.CrmDbConnectionString);
                    BaseSystemInfo.OaDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.OaDbConnectionString);
                    BaseSystemInfo.LabelDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.LabelDbConnectionString);
                    BaseSystemInfo.WebDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.WebDbConnectionString);
                    BaseSystemInfo.CmsDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.CmsDbConnectionString);
                    BaseSystemInfo.DTcmsDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.DTcmsDbConnectionString);
                    BaseSystemInfo.FlowPortalDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.FlowPortalDbConnectionString);
                    BaseSystemInfo.DealerPortalDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.DealerPortalDbConnectionString);
                    BaseSystemInfo.CustomerPortalDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.CustomerPortalDbConnectionString);
                    BaseSystemInfo.SupplierPortalDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.SupplierPortalDbConnectionString);
                }
            }

            // 若是本地模式运行，然后还缺少数据库配置？
            if (BaseSystemInfo.Service.Equals("DotNet.Business"))
            {
                if (string.IsNullOrEmpty(BaseSystemInfo.UserCenterDbConnection))
                {
                    BaseSystemInfo.UserCenterDbConnection = "Data Source=localhost;Initial Catalog=UserCenterV" + BaseSystemInfo.DatabaseTableVersion + ";Integrated Security=SSPI;";
                }
                if (string.IsNullOrEmpty(BaseSystemInfo.LogonLogDbConnection))
                {
                    BaseSystemInfo.LogonLogDbConnection = "Data Source=localhost;Initial Catalog=UserCenterV" + BaseSystemInfo.DatabaseTableVersion + ";Integrated Security=SSPI;";
                }
                if (string.IsNullOrEmpty(BaseSystemInfo.MessageDbConnection))
                {
                    BaseSystemInfo.MessageDbConnection = BaseSystemInfo.UserCenterDbConnection;
                }
                if (string.IsNullOrEmpty(BaseSystemInfo.WorkFlowDbConnection))
                {
                    // BaseSystemInfo.WorkFlowDbConnection = "Data Source=localhost;Initial Catalog=WorkFlowV" + BaseSystemInfo.DatabaseTableVersion + ";Integrated Security=SSPI;";
                }
                if (string.IsNullOrEmpty(BaseSystemInfo.BusinessDbConnection))
                {
                    // BaseSystemInfo.BusinessDbConnection = "Data Source=localhost;Initial Catalog=ProjectV" + BaseSystemInfo.DatabaseTableVersion + ";Integrated Security=SSPI;";
                }
                if (string.IsNullOrEmpty(BaseSystemInfo.WebAppDbConnection))
                {
                    // BaseSystemInfo.WebAppDbConnection = "Data Source=localhost;Initial Catalog=ProjectV" + BaseSystemInfo.DatabaseTableVersion + ";Integrated Security=SSPI;";
                }
                if (string.IsNullOrEmpty(BaseSystemInfo.BpmDbConnection))
                {
                    // BaseSystemInfo.BpmDbConnection = "Data Source=localhost;Initial Catalog=BPMDB;Integrated Security=SSPI;";
                }
                if (string.IsNullOrEmpty(BaseSystemInfo.ErpDbConnection))
                {
                    // BaseSystemInfo.ErpDbConnection = "Data Source=localhost;Initial Catalog=ERPDB;Integrated Security=SSPI;";
                }
                if (string.IsNullOrEmpty(BaseSystemInfo.MesDbConnection))
                {
                    // BaseSystemInfo.ErpDbConnection = "Data Source=localhost;Initial Catalog=MESDB;Integrated Security=SSPI;";
                }
                if (string.IsNullOrEmpty(BaseSystemInfo.HrmDbConnection))
                {
                    // BaseSystemInfo.ErpDbConnection = "Data Source=localhost;Initial Catalog=HRMDB;Integrated Security=SSPI;";
                }
                if (string.IsNullOrEmpty(BaseSystemInfo.CrmDbConnection))
                {
                    // BaseSystemInfo.ErpDbConnection = "Data Source=localhost;Initial Catalog=CRMDB;Integrated Security=SSPI;";
                }
                if (string.IsNullOrEmpty(BaseSystemInfo.OaDbConnection))
                {
                    // BaseSystemInfo.ErpDbConnection = "Data Source=localhost;Initial Catalog=OADB;Integrated Security=SSPI;";
                }
                if (string.IsNullOrEmpty(BaseSystemInfo.LabelDbConnection))
                {
                    // BaseSystemInfo.ErpDbConnection = "Data Source=localhost;Initial Catalog=LabelDB;Integrated Security=SSPI;";
                }
                if (string.IsNullOrEmpty(BaseSystemInfo.WebDbConnection))
                {
                    // BaseSystemInfo.ErpDbConnection = "Data Source=localhost;Initial Catalog=WebDB;Integrated Security=SSPI;";
                }
                if (string.IsNullOrEmpty(BaseSystemInfo.CmsDbConnection))
                {
                    // BaseSystemInfo.CmsDbConnection = "Data Source=localhost;Initial Catalog=CMSDB;Integrated Security=SSPI;";
                }

            }

            if (Exists("DatabaseTableVersion"))
            {
                if (ValidateUtil.IsInt(GetValue(_xmlDocument, "DatabaseTableVersion")))
                {
                    BaseSystemInfo.DatabaseTableVersion = int.Parse(GetValue(_xmlDocument, "DatabaseTableVersion"));
                }
            }

            if (Exists("SecurityKey"))
            {
                BaseSystemInfo.SecurityKey = GetValue(_xmlDocument, "SecurityKey");
            }
            if (Exists("AppKey"))
            {
                BaseSystemInfo.AppKey = GetValue(_xmlDocument, "AppKey");
            }
            if (Exists("AppSecret"))
            {
                BaseSystemInfo.AppSecret = GetValue(_xmlDocument, "AppSecret");
            }
            if (Exists("JwtEnabled"))
            {
                BaseSystemInfo.JwtEnabled = GetValue(_xmlDocument, "JwtEnabled").Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (Exists("JwtSecret"))
            {
                BaseSystemInfo.JwtSecret = GetValue(_xmlDocument, "JwtSecret");
            }
            if (Exists("ServiceUserName"))
            {
                BaseSystemInfo.ServiceUserName = GetValue(_xmlDocument, "ServiceUserName");
            }
            if (Exists("ServicePassword"))
            {
                BaseSystemInfo.ServicePassword = GetValue(_xmlDocument, "ServicePassword");
            }
            if (Exists("WhiteListEnabled"))
            {
                BaseSystemInfo.WhiteListEnabled = GetValue(_xmlDocument, "WhiteListEnabled").Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (Exists("WhiteList"))
            {
                BaseSystemInfo.WhiteList = GetValue(_xmlDocument, "WhiteList");
            }
            if (Exists("BlackList"))
            {
                BaseSystemInfo.BlackList = GetValue(_xmlDocument, "BlackList");
            }
            if (Exists("AdministratorEnabled"))
            {
                BaseSystemInfo.AdministratorEnabled = GetValue(_xmlDocument, "AdministratorEnabled").Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (Exists("RegisterKey"))
            {
                BaseSystemInfo.RegisterKey = GetValue(_xmlDocument, "RegisterKey");
            }

            // 错误报告相关
            if (Exists("ErrorReportTo"))
            {
                BaseSystemInfo.ErrorReportTo = GetValue(_xmlDocument, "ErrorReportTo");
            }
            //追溯码
            if (Exists("TraceabilityKey"))
            {
                BaseSystemInfo.TraceabilityKey = GetValue(_xmlDocument, "TraceabilityKey");
            }
            if (Exists("TraceabilityCodeEnabled"))
            {
                BaseSystemInfo.TraceabilityCodeEnabled = GetValue(_xmlDocument, "TraceabilityCodeEnabled").Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            //SMTP邮件服务器
            if (Exists("MailServer"))
            {
                BaseSystemInfo.MailServer = GetValue(_xmlDocument, "MailServer");
            }
            if (Exists("MailServerPort"))
            {
                if (ValidateUtil.IsInt(GetValue(_xmlDocument, "MailServerPort")))
                {
                    BaseSystemInfo.MailServerPort = Convert.ToInt32(GetValue(_xmlDocument, "MailServerPort"));
                }
            }
            if (Exists("MailServerSslEnabled"))
            {
                BaseSystemInfo.MailServerSslEnabled = GetValue(_xmlDocument, "MailServerSslEnabled").Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (Exists("MailUserName"))
            {
                BaseSystemInfo.MailUserName = GetValue(_xmlDocument, "MailUserName");
            }
            if (Exists("MailPassword"))
            {
                BaseSystemInfo.MailPassword = GetValue(_xmlDocument, "MailPassword");
                // BaseSystemInfo.MailPassword = SecretUtil.Encrypt(BaseSystemInfo.MailPassword);
                // BaseSystemInfo.MailPassword = SecretUtil.Decrypt(BaseSystemInfo.MailPassword);
            }
            if (Exists("MailFrom"))
            {
                BaseSystemInfo.MailFrom = GetValue(_xmlDocument, "MailFrom");
            }
            if (Exists("MailBCC"))
            {
                BaseSystemInfo.MailBcc = GetValue(_xmlDocument, "MailBCC");
            }
            if (Exists("UploadBlockSize"))
            {
                BaseSystemInfo.UploadBlockSize = Convert.ToInt32(GetValue(_xmlDocument, "UploadBlockSize"));
            }
            if (Exists("UploadStorageMode"))
            {
                BaseSystemInfo.UploadStorageMode = GetValue(_xmlDocument, "UploadStorageMode");
            }
            if (Exists("UploadPath"))
            {
                BaseSystemInfo.UploadPath = GetValue(_xmlDocument, "UploadPath");
            }

            // 这里重新给静态数据库连接对象进行赋值
            // DotNet.Util.DbTypeUtil.DbConnection = BaseSystemInfo.BusinessDbConnection;
            // DotNet.Util.DbTypeUtil.DbType = BaseSystemInfo.BusinessDbType;

            // 这里是处理读写分离功能，读取数据与写入数据进行分离的方式
            if (Exists("UserCenterReadDbConnection"))
            {
                BaseSystemInfo.UserCenterReadDbConnection = GetValue(_xmlDocument, "UserCenterReadDbConnection");
            }
            if (Exists("UserCenterWriteDbConnection"))
            {
                BaseSystemInfo.UserCenterWriteDbConnection = GetValue(_xmlDocument, "UserCenterWriteDbConnection");
            }

            if (Exists("ApplicationId"))
            {
                BaseSystemInfo.ApplicationId = GetValue(_xmlDocument, "ApplicationId");
            }
            if (Exists("MainPage"))
            {
                BaseSystemInfo.MainPage = GetValue(_xmlDocument, "MainPage");
            }
            if (Exists("IsProduction"))
            {
                BaseSystemInfo.IsProduction = GetValue(_xmlDocument, "IsProduction").Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (Exists("SmsEnabled"))
            {
                BaseSystemInfo.SmsEnabled = GetValue(_xmlDocument, "SmsEnabled").Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (Exists("IsWindowsAuthentication"))
            {
                BaseSystemInfo.IsWindowsAuthentication = GetValue(_xmlDocument, "IsWindowsAuthentication").Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (Exists("UploadFileExtension"))
            {
                BaseSystemInfo.UploadFileExtension = GetValue(_xmlDocument, "UploadFileExtension");
            }
            if (Exists("UploadVideoExtension"))
            {
                BaseSystemInfo.UploadVideoExtension = GetValue(_xmlDocument, "UploadVideoExtension");
            }
            if (Exists("UploadAudioExtension"))
            {
                BaseSystemInfo.UploadAudioExtension = GetValue(_xmlDocument, "UploadAudioExtension");
            }
            if (Exists("UserRolePrefix"))
            {
                BaseSystemInfo.UserRolePrefix = GetValue(_xmlDocument, "UserRolePrefix");
            }
            if (Exists("ExcludedUserRolePrefix"))
            {
                BaseSystemInfo.ExcludedUserRolePrefix = GetValue(_xmlDocument, "ExcludedUserRolePrefix");
            }
            if (Exists("RootMenuCode"))
            {
                BaseSystemInfo.RootMenuCode = GetValue(_xmlDocument, "RootMenuCode");
            }
        }
        #endregion

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyValue"></param>
        /// <param name="checkExists"></param>
        public static void SetValue(string key, string keyValue, bool checkExists = false)
        {
            if (File.Exists(ConfigFileName))
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(ConfigFileName);
                SetValue(xmlDocument, key, keyValue, checkExists);
                xmlDocument.Save(ConfigFileName);
            }
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <param name="key"></param>
        /// <param name="keyValue"></param>
        /// <param name="checkExists"></param>
        /// <returns></returns>
        public static bool SetValue(XmlDocument xmlDocument, string key, string keyValue, bool checkExists = false)
        {
            return SetValue(xmlDocument, SelectPath, key, keyValue, checkExists);
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <param name="selectPath"></param>
        /// <param name="key"></param>
        /// <param name="keyValue"></param>
        /// <param name="checkExists"></param>
        /// <returns></returns>
        public static bool SetValue(XmlDocument xmlDocument, string selectPath, string key, string keyValue, bool checkExists = false)
        {
            var result = false;
            var exists = false;
            var xmlNodeList = xmlDocument.SelectNodes(selectPath);
            if (xmlNodeList != null)
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    if (xmlNode.Attributes != null && xmlNode.Attributes["key"].Value.ToUpper().Equals(key.ToUpper()))
                    {
                        xmlNode.Attributes["value"].Value = keyValue;
                        result = true;
                        exists = true;
                        break;
                    }
                }

            if (checkExists && !exists)
            {
                var xmlElement = xmlDocument.CreateElement("add");
                // xmlElement.Value = keyValue;
                xmlElement.SetAttribute("key", key);
                xmlElement.SetAttribute("value", keyValue);
                var selectPathRoot = "//appSettings";
                var xmlNode = xmlDocument.SelectSingleNode(selectPathRoot);
                xmlNode?.AppendChild(xmlElement);
            }
            return result;
        }

        #region public static void SaveConfig() 保存配置文件
        /// <summary>
        /// 保存配置文件
        /// </summary>
        public static void SaveConfig()
        {
            if (File.Exists(ConfigFileName))
            {
                SaveConfig(ConfigFileName);
            }
        }
        #endregion

        #region public static void SaveConfig(string fileName) 保存到指定的文件
        /// <summary>
        /// 保存到指定的文件
        /// </summary>
        /// <param name="fileName">配置文件</param>
        public static void SaveConfig(string fileName)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);

            #region 写入Redis配置
            SetValue(xmlDocument, "RedisEnabled", BaseSystemInfo.RedisEnabled.ToString());
            SetValue(xmlDocument, "RedisServer", BaseSystemInfo.RedisServer);
            SetValue(xmlDocument, "RedisPort", BaseSystemInfo.RedisPort.ToString());
            SetValue(xmlDocument, "RedisInitialDb", BaseSystemInfo.RedisInitialDb.ToString());
            SetValue(xmlDocument, "RedisSslEnabled", BaseSystemInfo.RedisSslEnabled.ToString());
            SetValue(xmlDocument, "RedisUserName", BaseSystemInfo.RedisUserName);
            SetValue(xmlDocument, "RedisPassword", BaseSystemInfo.RedisPassword);
            SetValue(xmlDocument, "RedisCacheMillisecond", BaseSystemInfo.RedisCacheMillisecond.ToString());
            SetValue(xmlDocument, "MemoryCacheMillisecond", BaseSystemInfo.MemoryCacheMillisecond.ToString());
            #endregion

            #region 写入FTP配置
            SetValue(xmlDocument, "FtpServer", BaseSystemInfo.FtpServer);
            SetValue(xmlDocument, "FtpPort", BaseSystemInfo.FtpPort.ToString());
            SetValue(xmlDocument, "FtpSslEnabled", BaseSystemInfo.FtpSslEnabled.ToString());
            SetValue(xmlDocument, "FtpUserName", BaseSystemInfo.FtpUserName);
            SetValue(xmlDocument, "FtpPassword", BaseSystemInfo.FtpPassword);
            #endregion

            #region 写入MQTT配置
            SetValue(xmlDocument, "MqttServer", BaseSystemInfo.MqttServer);
            SetValue(xmlDocument, "MqttPort", BaseSystemInfo.MqttPort.ToString());
            SetValue(xmlDocument, "MqttSslEnabled", BaseSystemInfo.MqttSslEnabled.ToString());
            SetValue(xmlDocument, "MqttUserName", BaseSystemInfo.MqttUserName);
            SetValue(xmlDocument, "MqttPassword", BaseSystemInfo.MqttPassword);
            #endregion

            #region 写入WebApi配置
            SetValue(xmlDocument, "WebApiMonitorEnabled", BaseSystemInfo.WebApiMonitorEnabled.ToString());
            SetValue(xmlDocument, "WebApiSlowMonitorEnabled", BaseSystemInfo.WebApiSlowMonitorEnabled.ToString());
            SetValue(xmlDocument, "WebApiSlowResponseMilliseconds", BaseSystemInfo.WebApiSlowResponseMilliseconds.ToString());
            #endregion

            #region 写入Cookie配置
            SetValue(xmlDocument, "CookieName", BaseSystemInfo.CookieName);
            SetValue(xmlDocument, "CookieDomain", BaseSystemInfo.CookieDomain);
            SetValue(xmlDocument, "CookieExpires", BaseSystemInfo.CookieExpires.ToString());
            #endregion

            SetValue(xmlDocument, "ConfigFile", BaseSystemInfo.ConfigFile);

            SetValue(xmlDocument, "CurrentNickName", BaseSystemInfo.CurrentNickName, true);
            SetValue(xmlDocument, "CurrentCompany", BaseSystemInfo.CurrentCompany);
            SetValue(xmlDocument, "CurrentUserName", BaseSystemInfo.CurrentUserName);
            SetValue(xmlDocument, "CurrentPassword", BaseSystemInfo.CurrentPassword);

            // 保存历史登录用户信息。
            SetValue(xmlDocument, "HistoryUsers", GetHistoryUsersSaveValue(), true);

            SetValue(xmlDocument, "MultiLanguage", BaseSystemInfo.MultiLanguage.ToString());
            SetValue(xmlDocument, "CurrentLanguage", BaseSystemInfo.CurrentLanguage);
            SetValue(xmlDocument, "OnInternet", BaseSystemInfo.OnInternet.ToString());
            SetValue(xmlDocument, "RememberPassword", BaseSystemInfo.RememberPassword.ToString());

            SetValue(xmlDocument, "ClientEncryptPassword", BaseSystemInfo.ClientEncryptPassword.ToString());
            SetValue(xmlDocument, "CheckIPAddress", BaseSystemInfo.CheckIpAddress.ToString());
            SetValue(xmlDocument, "LogException", BaseSystemInfo.LogException.ToString());
            SetValue(xmlDocument, "LogSql", BaseSystemInfo.LogSql.ToString());
            SetValue(xmlDocument, "LogCache", BaseSystemInfo.LogCache.ToString());
            SetValue(xmlDocument, "EventLog", BaseSystemInfo.EventLog.ToString());
            SetValue(xmlDocument, "UploadBlockSize", BaseSystemInfo.UploadBlockSize.ToString());

            SetValue(xmlDocument, "ServerEncryptPassword", BaseSystemInfo.ServerEncryptPassword.ToString());
            SetValue(xmlDocument, "PasswordMiniLength", BaseSystemInfo.PasswordMiniLength.ToString());
            SetValue(xmlDocument, "NumericCharacters", BaseSystemInfo.NumericCharacters.ToString());
            SetValue(xmlDocument, "PasswordChangeCycle", BaseSystemInfo.PasswordChangeCycle.ToString());
            SetValue(xmlDocument, "CheckOnline", BaseSystemInfo.CheckOnline.ToString());
            SetValue(xmlDocument, "AccountMinimumLength", BaseSystemInfo.AccountMinimumLength.ToString());
            SetValue(xmlDocument, "PasswordErrorLockLimit", BaseSystemInfo.PasswordErrorLockLimit.ToString());
            SetValue(xmlDocument, "PasswordErrorLockCycle", BaseSystemInfo.PasswordErrorLockCycle.ToString());
            SetValue(xmlDocument, "OpenNewWebWindow", BaseSystemInfo.OpenNewWebWindow.ToString());

            SetValue(xmlDocument, "UseMessage", BaseSystemInfo.UseMessage.ToString(), true);
            SetValue(xmlDocument, "Synchronous", BaseSystemInfo.Synchronous.ToString(), true);
            SetValue(xmlDocument, "CheckBalance", BaseSystemInfo.CheckBalance.ToString(), true);
            SetValue(xmlDocument, "AutoLogon", BaseSystemInfo.AutoLogon.ToString());
            SetValue(xmlDocument, "ForceHttps", BaseSystemInfo.ForceHttps.ToString());
            SetValue(xmlDocument, "AllowUserRegister", BaseSystemInfo.AllowUserRegister.ToString());
            SetValue(xmlDocument, "RecordLog", BaseSystemInfo.RecordLog.ToString());

            // 客户信息配置
            SetValue(xmlDocument, "CustomerCompanyId", BaseSystemInfo.CustomerCompanyId);
            SetValue(xmlDocument, "CustomerCompanyName", BaseSystemInfo.CustomerCompanyName);
            SetValue(xmlDocument, "CompanyName", BaseSystemInfo.CompanyName);
            SetValue(xmlDocument, "CompanyWebsite", BaseSystemInfo.CompanyWebsite);
            SetValue(xmlDocument, "CustomerCompanyWebsite", BaseSystemInfo.CustomerCompanyWebsite);
            SetValue(xmlDocument, "SoftName", BaseSystemInfo.SoftName);
            SetValue(xmlDocument, "SoftFullName", BaseSystemInfo.SoftFullName);
            SetValue(xmlDocument, "Version", BaseSystemInfo.Version);

            SetValue(xmlDocument, "ConfigurationFrom", BaseSystemInfo.ConfigurationFrom.ToString());
            SetValue(xmlDocument, "TimeFormat", BaseSystemInfo.TimeFormat);
            SetValue(xmlDocument, "DateFormat", BaseSystemInfo.DateFormat);
            SetValue(xmlDocument, "DateTimeFormat", BaseSystemInfo.DateTimeFormat);
            SetValue(xmlDocument, "DateTimeLongFormat", BaseSystemInfo.DateTimeLongFormat);
            SetValue(xmlDocument, "Host", BaseSystemInfo.Host);
            SetValue(xmlDocument, "Port", BaseSystemInfo.Port.ToString());
            SetValue(xmlDocument, "MobileHost", BaseSystemInfo.MobileHost);

            SetValue(xmlDocument, "UseUserPermission", BaseSystemInfo.UseUserPermission.ToString());
            SetValue(xmlDocument, "UseAuthorizationScope", BaseSystemInfo.UseAuthorizationScope.ToString());
            SetValue(xmlDocument, "UsePermissionScope", BaseSystemInfo.UsePermissionScope.ToString());
            SetValue(xmlDocument, "UseOrganizationPermission", BaseSystemInfo.UseOrganizationPermission.ToString());
            SetValue(xmlDocument, "UseTableColumnPermission", BaseSystemInfo.UseTableColumnPermission.ToString());
            SetValue(xmlDocument, "UseTableScopePermission", BaseSystemInfo.UseTableScopePermission.ToString());
            // SetValue(xmlDocument, "LoadAllUser", BaseSystemInfo.LoadAllUser.ToString());

            SetValue(xmlDocument, "Service", BaseSystemInfo.Service);

            SetValue(xmlDocument, "LogonForm", BaseSystemInfo.LogonForm);
            SetValue(xmlDocument, "MainForm", BaseSystemInfo.MainForm);

            SetValue(xmlDocument, "OnlineLimit", BaseSystemInfo.OnlineLimit.ToString());
            SetValue(xmlDocument, "DbType", BaseSystemInfo.BusinessDbType.ToString());

            // 保存数据库配置
            SetValue(xmlDocument, "SlowQueryMilliseconds", BaseSystemInfo.SlowQueryMilliseconds.ToString());
            SetValue(xmlDocument, "UserCenterDbType", BaseSystemInfo.UserCenterDbType.ToString());
            SetValue(xmlDocument, "MessageDbType", BaseSystemInfo.MessageDbType.ToString());
            SetValue(xmlDocument, "BusinessDbType", BaseSystemInfo.BusinessDbType.ToString());
            SetValue(xmlDocument, "WorkFlowDbType", BaseSystemInfo.WorkFlowDbType.ToString());
            SetValue(xmlDocument, "WebAppDbType", BaseSystemInfo.WebAppDbType.ToString());
            SetValue(xmlDocument, "BpmDbType", BaseSystemInfo.BpmDbType.ToString());
            SetValue(xmlDocument, "ErpDbType", BaseSystemInfo.ErpDbType.ToString());
            SetValue(xmlDocument, "MESDbType", BaseSystemInfo.MesDbType.ToString());
            SetValue(xmlDocument, "HRMDbType", BaseSystemInfo.HrmDbType.ToString());
            SetValue(xmlDocument, "CRMDbType", BaseSystemInfo.CrmDbType.ToString());
            SetValue(xmlDocument, "OADbType", BaseSystemInfo.OaDbType.ToString());
            SetValue(xmlDocument, "LabelDbType", BaseSystemInfo.LabelDbType.ToString());
            SetValue(xmlDocument, "WebDbType", BaseSystemInfo.WebDbType.ToString());
            SetValue(xmlDocument, "CMSDbType", BaseSystemInfo.CmsDbType.ToString());
            SetValue(xmlDocument, "DTcmsDbType", BaseSystemInfo.DTcmsDbType.ToString());
            SetValue(xmlDocument, "FlowPortalDbType", BaseSystemInfo.FlowPortalDbType.ToString());
            SetValue(xmlDocument, "CustomerPortalDbType", BaseSystemInfo.CustomerPortalDbType.ToString());
            SetValue(xmlDocument, "SupplierPortalDbType", BaseSystemInfo.SupplierPortalDbType.ToString());

            SetValue(xmlDocument, "EncryptDbConnection", BaseSystemInfo.EncryptDbConnection.ToString());
            SetValue(xmlDocument, "UserCenterDbConnection", BaseSystemInfo.UserCenterDbConnectionString);
            SetValue(xmlDocument, "MessageDbConnection", BaseSystemInfo.MessageDbConnectionString);
            SetValue(xmlDocument, "BusinessDbConnection", BaseSystemInfo.BusinessDbConnectionString);
            SetValue(xmlDocument, "WorkFlowDbConnection", BaseSystemInfo.WorkFlowDbConnectionString);
            SetValue(xmlDocument, "WebAppDbConnection", BaseSystemInfo.WebAppDbConnectionString);
            SetValue(xmlDocument, "BpmDbConnection", BaseSystemInfo.BpmDbConnectionString);
            SetValue(xmlDocument, "ErpDbConnection", BaseSystemInfo.ErpDbConnectionString);
            SetValue(xmlDocument, "WmsDbConnection", BaseSystemInfo.WmsDbConnectionString);
            SetValue(xmlDocument, "MesDbConnection", BaseSystemInfo.MesDbConnectionString);
            SetValue(xmlDocument, "HrmDbConnection", BaseSystemInfo.HrmDbConnectionString);
            SetValue(xmlDocument, "CrmDbConnection", BaseSystemInfo.CrmDbConnectionString);
            SetValue(xmlDocument, "OaDbConnection", BaseSystemInfo.OaDbConnectionString);
            SetValue(xmlDocument, "LabelDbConnection", BaseSystemInfo.LabelDbConnectionString);
            SetValue(xmlDocument, "WebDbConnection", BaseSystemInfo.WebDbConnectionString);
            SetValue(xmlDocument, "CmsDbConnection", BaseSystemInfo.CmsDbConnectionString);
            SetValue(xmlDocument, "DTcmsDbConnection", BaseSystemInfo.DTcmsDbConnectionString);
            SetValue(xmlDocument, "FlowPortalDbConnection", BaseSystemInfo.FlowPortalDbConnectionString);
            SetValue(xmlDocument, "CustomerPortalDbConnection", BaseSystemInfo.CustomerPortalDbConnectionString);
            SetValue(xmlDocument, "SupplierPortalDbConnection", BaseSystemInfo.SupplierPortalDbConnectionString);
            SetValue(xmlDocument, "ReportDbConnection", BaseSystemInfo.ReportDbConnectionString);
            SetValue(xmlDocument, "ScmDbConnection", BaseSystemInfo.ScmDbConnectionString);
            SetValue(xmlDocument, "ImsDbConnection", BaseSystemInfo.ImsDbConnectionString);
            SetValue(xmlDocument, "OmsDbConnection", BaseSystemInfo.OmsDbConnectionString);
            SetValue(xmlDocument, "MemberDbConnection", BaseSystemInfo.MemberDbConnectionString);
            SetValue(xmlDocument, "DatabaseTableVersion", BaseSystemInfo.DatabaseTableVersion.ToString());

            SetValue(xmlDocument, "RegisterKey", BaseSystemInfo.RegisterKey);

            SetValue(xmlDocument, "ApplicationId", BaseSystemInfo.ApplicationId);
            SetValue(xmlDocument, "MainPage", BaseSystemInfo.MainPage);
            SetValue(xmlDocument, "IsProduction", BaseSystemInfo.IsProduction.ToString());
            SetValue(xmlDocument, "SmsEnabled", BaseSystemInfo.SmsEnabled.ToString());
            SetValue(xmlDocument, "IsWindowsAuthentication", BaseSystemInfo.IsWindowsAuthentication.ToString());
            SetValue(xmlDocument, "UploadFileExtension", BaseSystemInfo.UploadFileExtension);
            SetValue(xmlDocument, "UploadVideoExtension", BaseSystemInfo.UploadVideoExtension);
            SetValue(xmlDocument, "UploadAudioExtension", BaseSystemInfo.UploadAudioExtension);
            SetValue(xmlDocument, "UserRolePrefix", BaseSystemInfo.UserRolePrefix);
            SetValue(xmlDocument, "ExcludedUserRolePrefix", BaseSystemInfo.ExcludedUserRolePrefix);
            SetValue(xmlDocument, "RootMenuCode", BaseSystemInfo.RootMenuCode);

            try
            {
                xmlDocument.Save(fileName);
            }
            catch (UnauthorizedAccessException uae)
            {
                //如果报没有权限异常
                throw new Exception("当前操作系统用户没有权限写入文件 " + fileName, uae);
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
            }
        }

        /// <summary>
        /// 将历史登录用户信息数组转换成保存到配置文件中的字符串。
        /// </summary>
        /// <returns></returns>
        private static string GetHistoryUsersSaveValue()
        {
            var result = "";
            foreach (var t in BaseSystemInfo.HistoryUsers)
            {
                result = result == "" ? t : $"{result};{t}";
            }
            return result;
        }
        #endregion
    }
}