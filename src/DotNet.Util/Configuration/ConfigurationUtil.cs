//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

using System;
using System.Configuration;

namespace DotNet.Util
{
    /// <summary>
    /// ConfigurationUtil
    /// 连接配置。
    /// 
    /// 修改记录
    /// 
    ///     2021.03.17 版本：4.0 Troy Cui  新增MQTT、FTP、WebApi的相关配置，并分类获取代码
    ///     2017.12.05 版本：3.0 Troy Cui  新增10个外部系统数据库连接的配置
    ///     2016.03.14 版本：2.1 JiRiGaLa  RecordLogonLog、RecordLog 开关的读取完善。 
    ///     2014.01.16 版本：2.0 JiRiGaLa  读取加密连接串的方法。 
    ///     2011.07.05 版本：1.1 zgl 增加  BaseSystemInfo.CheckIPAddress。
    ///		2008.06.08 版本：1.0 JiRiGaLa 将程序从 BaseConfiguration 进行了分离。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.03.14</date>
    /// </author> 
    /// </summary>
    public partial class ConfigurationUtil
    {
        /// <summary>
        /// AppSettting
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="encrypt"></param>
        /// <returns></returns>
        public static string AppSettings(string key, bool encrypt)
        {
            var result = string.Empty;

            if (ConfigurationManager.AppSettings[key] != null)
            {
                result = ConfigurationManager.AppSettings[key];
            }
            if (!string.IsNullOrEmpty(result))
            {
                if (encrypt)
                {
                    result = SecretUtil.DesDecrypt(result);
                }
            }

            return result;
        }

        #region public static void GetConfig()
        /// <summary>
        /// 从配置信息获取配置信息
        /// </summary>
        public static void GetConfig()
        {
            //Web相关配置
            GetWebAppConfig();
            //用户相关配置
            GetUserConfig();
            //Cookie相关配置
            GetCookieConfig();
            //数据库配置信息
            GetDatabaseConfig();
            //Mail邮件配置信息
            GetMailConfig();
            //获取MQTT配置
            GetMqttConfig();
            //获取WebApi配置
            GetWebApiConfig();
            //获取FTP配置
            GetFtpConfig();
            //获取Redis配置
            GetRedisConfig();

            //读取注册码
            if (ConfigurationManager.AppSettings["RegisterKey"] != null)
            {
                BaseSystemInfo.RegisterKey = ConfigurationManager.AppSettings["RegisterKey"];
            }
            //追溯码
            if (ConfigurationManager.AppSettings["TraceabilityCodeEnabled"] != null)
            {
                BaseSystemInfo.TraceabilityCodeEnabled = ConfigurationManager.AppSettings["TraceabilityCodeEnabled"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["TraceabilityKey"] != null)
            {
                BaseSystemInfo.TraceabilityKey = ConfigurationManager.AppSettings["TraceabilityKey"];
            }

            // 客户信息配置
            if (ConfigurationManager.AppSettings["CustomerCompanyId"] != null)
            {
                BaseSystemInfo.CustomerCompanyId = ConfigurationManager.AppSettings["CustomerCompanyId"];
            }
            if (ConfigurationManager.AppSettings["CustomerCompanyName"] != null)
            {
                BaseSystemInfo.CustomerCompanyName = ConfigurationManager.AppSettings["CustomerCompanyName"];
            }
            if (ConfigurationManager.AppSettings["ConfigurationFrom"] != null)
            {
                BaseSystemInfo.ConfigurationFrom = BaseConfiguration.GetConfiguration(ConfigurationManager.AppSettings["ConfigurationFrom"]);
            }

            if (ConfigurationManager.AppSettings["TimeFormat"] != null)
            {
                BaseSystemInfo.TimeFormat = ConfigurationManager.AppSettings["TimeFormat"];
            }
            if (ConfigurationManager.AppSettings["DateFormat"] != null)
            {
                BaseSystemInfo.DateFormat = ConfigurationManager.AppSettings["DateFormat"];
            }
            if (ConfigurationManager.AppSettings["DateTimeFormat"] != null)
            {
                BaseSystemInfo.DateTimeFormat = ConfigurationManager.AppSettings["DateTimeFormat"];
            }
            if (ConfigurationManager.AppSettings["DateTimeLongFormat"] != null)
            {
                BaseSystemInfo.DateTimeLongFormat = ConfigurationManager.AppSettings["DateTimeLongFormat"];
            }

            if (ConfigurationManager.AppSettings["SoftName"] != null)
            {
                BaseSystemInfo.SoftName = ConfigurationManager.AppSettings["SoftName"];
            }
            if (ConfigurationManager.AppSettings["CompanyName"] != null)
            {
                BaseSystemInfo.CompanyName = ConfigurationManager.AppSettings["CompanyName"];
            }
            if (ConfigurationManager.AppSettings["CompanyWebsite"] != null)
            {
                BaseSystemInfo.CompanyWebsite = ConfigurationManager.AppSettings["CompanyWebsite"];
            }
            if (ConfigurationManager.AppSettings["CustomerCompanyWebsite"] != null)
            {
                BaseSystemInfo.CustomerCompanyWebsite = ConfigurationManager.AppSettings["CustomerCompanyWebsite"];
            }
            if (ConfigurationManager.AppSettings["SoftFullName"] != null)
            {
                BaseSystemInfo.SoftFullName = ConfigurationManager.AppSettings["SoftFullName"];
            }

            if (ConfigurationManager.AppSettings["SecurityKey"] != null)
            {
                BaseSystemInfo.SecurityKey = ConfigurationManager.AppSettings["SecurityKey"];
            }
            if (ConfigurationManager.AppSettings["AppKey"] != null)
            {
                BaseSystemInfo.AppKey = ConfigurationManager.AppSettings["AppKey"];
            }
            if (ConfigurationManager.AppSettings["AppSecret"] != null)
            {
                BaseSystemInfo.AppSecret = ConfigurationManager.AppSettings["AppSecret"];
            }
            if (ConfigurationManager.AppSettings["JwtEnabled"] != null)
            {
                BaseSystemInfo.JwtEnabled = ConfigurationManager.AppSettings["JwtEnabled"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["JwtSecret"] != null)
            {
                BaseSystemInfo.JwtSecret = ConfigurationManager.AppSettings["JwtSecret"];
            }
            if (ConfigurationManager.AppSettings["ServiceUserName"] != null)
            {
                BaseSystemInfo.ServiceUserName = ConfigurationManager.AppSettings["ServiceUserName"];
            }
            if (ConfigurationManager.AppSettings["ServicePassword"] != null)
            {
                BaseSystemInfo.ServicePassword = ConfigurationManager.AppSettings["ServicePassword"];
            }
            if (ConfigurationManager.AppSettings["WhiteListEnabled"] != null)
            {
                BaseSystemInfo.WhiteListEnabled = ConfigurationManager.AppSettings["WhiteListEnabled"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["WhiteListEnabled"] != null)
            {
                BaseSystemInfo.WhiteListEnabled = ConfigurationManager.AppSettings["WhiteListEnabled"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["WhiteList"] != null)
            {
                BaseSystemInfo.WhiteList = ConfigurationManager.AppSettings["WhiteList"];
            }
            if (ConfigurationManager.AppSettings["BlackList"] != null)
            {
                BaseSystemInfo.BlackList = ConfigurationManager.AppSettings["BlackList"];
            }
            if (ConfigurationManager.AppSettings["AdministratorEnabled"] != null)
            {
                BaseSystemInfo.AdministratorEnabled = ConfigurationManager.AppSettings["AdministratorEnabled"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["PermissionExportEnabled"] != null)
            {
                BaseSystemInfo.PermissionExportEnabled = ConfigurationManager.AppSettings["PermissionExportEnabled"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            // BaseSystemInfo.CurrentLanguage = ConfigurationManager.AppSettings[BaseConfiguration.CURRENT_LANGUAGE];
            // BaseSystemInfo.Version = ConfigurationManager.AppSettings[BaseConfiguration.VERSION];

            // BaseSystemInfo.UseModulePermission = (ConfigurationManager.AppSettings[BaseConfiguration.USE_MODULE_PERMISSION].ToUpper(), true.ToString().ToUpper(), true);
            // BaseSystemInfo.UsePermissionScope = (ConfigurationManager.AppSettings[BaseConfiguration.USE_PERMISSIONS_COPE].ToUpper(), true.ToString().ToUpper(), true);
            // BaseSystemInfo.UseTablePermission = (ConfigurationManager.AppSettings[BaseConfiguration.USE_TABLE_PERMISSION].ToUpper(), true.ToString().ToUpper(), true);

            // BaseSystemInfo.Service = ConfigurationManager.AppSettings[BaseConfiguration.SERVICE];

            if (ConfigurationManager.AppSettings["RecordLogonLog"] != null)
            {
                BaseSystemInfo.RecordLogonLog = ConfigurationManager.AppSettings["RecordLogonLog"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["RecordLog"] != null)
            {
                BaseSystemInfo.RecordLog = ConfigurationManager.AppSettings["RecordLog"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["LogonStatistics"] != null)
            {
                BaseSystemInfo.LogonStatistics = ConfigurationManager.AppSettings["LogonStatistics"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["ServerEncryptPassword"] != null)
            {
                BaseSystemInfo.ServerEncryptPassword = ConfigurationManager.AppSettings["ServerEncryptPassword"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["ClientEncryptPassword"] != null)
            {
                BaseSystemInfo.ClientEncryptPassword = ConfigurationManager.AppSettings["ClientEncryptPassword"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["UseBaseTable"] != null)
            {
                BaseSystemInfo.UseBaseTable = ConfigurationManager.AppSettings["UseBaseTable"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["CheckIPAddress"] != null)
            {
                BaseSystemInfo.CheckIpAddress = ConfigurationManager.AppSettings["CheckIPAddress"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["LogException"] != null)
            {
                BaseSystemInfo.LogException = ConfigurationManager.AppSettings["LogException"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["LogSql"] != null)
            {
                BaseSystemInfo.LogSql = ConfigurationManager.AppSettings["LogSql"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["LogCache"] != null)
            {
                BaseSystemInfo.LogCache = ConfigurationManager.AppSettings["LogCache"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["EventLog"] != null)
            {
                BaseSystemInfo.EventLog = ConfigurationManager.AppSettings["EventLog"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["LogFileNamePattern"] != null)
            {
                BaseSystemInfo.LogFileNamePattern = ConfigurationManager.AppSettings["LogFileNamePattern"];
            }
            if (ConfigurationManager.AppSettings["LogFileMaxSize"] != null)
            {
                BaseSystemInfo.LogFileMaxSize = ConfigurationManager.AppSettings["LogFileMaxSize"].ToInt();
            }

            if (ConfigurationManager.AppSettings["UseOrganizationPermission"] != null)
            {
                BaseSystemInfo.UseOrganizationPermission = ConfigurationManager.AppSettings["UseOrganizationPermission"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }

            // BaseSystemInfo.AutoLogon = (ConfigurationManager.AppSettings[BaseConfiguration.AUTO_LOGON].ToUpper(), true.ToString().ToUpper(), true);
            // BaseSystemInfo.LogonAssembly = ConfigurationManager.AppSettings[BaseConfiguration.LOGON_ASSEMBLY];
            // BaseSystemInfo.LogonForm = ConfigurationManager.AppSettings[BaseConfiguration.LOGON_FORM];
            // BaseSystemInfo.MainForm = ConfigurationManager.AppSettings[BaseConfiguration.MAIN_FORM];



            if (ConfigurationManager.AppSettings["OpenNewWebWindow"] != null)
            {
                BaseSystemInfo.OpenNewWebWindow = ConfigurationManager.AppSettings["OpenNewWebWindow"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }

            // BaseSystemInfo.LoadAllUser = (ConfigurationManager.AppSettings[BaseConfiguration.LOAD_All_USER].ToUpper(), true.ToString().ToUpper(), true);



        }

        #endregion

        #region 数据库配置信息
        /// <summary>
        /// 数据库配置信息
        /// </summary>
        public static void GetDatabaseConfig()
        {
            // 数据库连接
            if (ConfigurationManager.AppSettings["ServerDbType"] != null)
            {
                BaseSystemInfo.ServerDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["ServerDbType"]);
            }
            //慢查询毫秒数
            if (ConfigurationManager.AppSettings["SlowQueryMilliseconds"] != null)
            {
                if (ValidateUtil.IsInt(ConfigurationManager.AppSettings["SlowQueryMilliseconds"]))
                {
                    BaseSystemInfo.SlowQueryMilliseconds = ConfigurationManager.AppSettings["SlowQueryMilliseconds"].ToInt();
                }
            }
            if (ConfigurationManager.AppSettings["UserCenterDbType"] != null)
            {
                BaseSystemInfo.UserCenterDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["UserCenterDbType"]);
                BaseSystemInfo.LogonLogDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["UserCenterDbType"]);
                BaseSystemInfo.MessageDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["UserCenterDbType"]);
            }
            if (ConfigurationManager.AppSettings["BusinessDbType"] != null)
            {
                BaseSystemInfo.BusinessDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["BusinessDbType"]);
            }
            if (ConfigurationManager.AppSettings["WorkFlowDbType"] != null)
            {
                BaseSystemInfo.WorkFlowDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["WorkFlowDbType"]);
            }
            if (ConfigurationManager.AppSettings["LogonLogDbType"] != null)
            {
                BaseSystemInfo.LogonLogDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["LogonLogDbType"]);
            }
            if (ConfigurationManager.AppSettings["MessageDbType"] != null)
            {
                BaseSystemInfo.MessageDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["MessageDbType"]);
            }

            if (ConfigurationManager.AppSettings["WebAppDbType"] != null)
            {
                BaseSystemInfo.WebAppDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["WebAppDbType"]);
            }
            if (ConfigurationManager.AppSettings["BPMDbType"] != null)
            {
                BaseSystemInfo.BpmDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["BPMDbType"]);
            }
            if (ConfigurationManager.AppSettings["ERPDbType"] != null)
            {
                BaseSystemInfo.ErpDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["ERPDbType"]);
            }
            if (ConfigurationManager.AppSettings["MESDbType"] != null)
            {
                BaseSystemInfo.MesDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["MESDbType"]);
            }
            if (ConfigurationManager.AppSettings["HRMDbType"] != null)
            {
                BaseSystemInfo.HrmDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["HRMDbType"]);
            }
            if (ConfigurationManager.AppSettings["CRMDbType"] != null)
            {
                BaseSystemInfo.CrmDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["CRMDbType"]);
            }
            if (ConfigurationManager.AppSettings["OADbType"] != null)
            {
                BaseSystemInfo.OaDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["OADbType"]);
            }
            if (ConfigurationManager.AppSettings["LabelDbType"] != null)
            {
                BaseSystemInfo.LabelDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["LabelDbType"]);
            }
            if (ConfigurationManager.AppSettings["WebDbType"] != null)
            {
                BaseSystemInfo.WebDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["WebDbType"]);
            }
            if (ConfigurationManager.AppSettings["CMSDbType"] != null)
            {
                BaseSystemInfo.CmsDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["CMSDbType"]);
            }
            if (ConfigurationManager.AppSettings["DTcmsDbType"] != null)
            {
                BaseSystemInfo.DTcmsDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["DTcmsDbType"]);
            }
            if (ConfigurationManager.AppSettings["FlowPortalDbType"] != null)
            {
                BaseSystemInfo.FlowPortalDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["FlowPortalDbType"]);
            }
            if (ConfigurationManager.AppSettings["CustomerPortalDbType"] != null)
            {
                BaseSystemInfo.CustomerPortalDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["CustomerPortalDbType"]);
            }
            if (ConfigurationManager.AppSettings["SupplierPortalDbType"] != null)
            {
                BaseSystemInfo.SupplierPortalDbType = DbTypeUtil.GetDbType(ConfigurationManager.AppSettings["SupplierPortalDbType"]);
            }
            if (ConfigurationManager.AppSettings["EncryptDbConnection"] != null)
            {
                BaseSystemInfo.EncryptDbConnection = ConfigurationManager.AppSettings["EncryptDbConnection"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }

            if (ConfigurationManager.AppSettings["UserCenterDbConnection"] != null)
            {
                BaseSystemInfo.UserCenterDbConnectionString = ConfigurationManager.AppSettings["UserCenterDbConnection"];
                BaseSystemInfo.LogonLogDbConnectionString = ConfigurationManager.AppSettings["UserCenterDbConnection"];
                BaseSystemInfo.MessageDbConnectionString = ConfigurationManager.AppSettings["UserCenterDbConnection"];
            }

            if (ConfigurationManager.AppSettings["BusinessDbConnection"] != null)
            {
                BaseSystemInfo.BusinessDbConnectionString = ConfigurationManager.AppSettings["BusinessDbConnection"];
            }

            if (ConfigurationManager.AppSettings["MessageDbConnection"] != null)
            {
                BaseSystemInfo.MessageDbConnectionString = ConfigurationManager.AppSettings["MessageDbConnection"];
            }
            if (ConfigurationManager.AppSettings["WorkFlowDbConnection"] != null)
            {
                BaseSystemInfo.WorkFlowDbConnectionString = ConfigurationManager.AppSettings["WorkFlowDbConnection"];
            }
            if (ConfigurationManager.AppSettings["LogonLogDbConnection"] != null)
            {
                BaseSystemInfo.LogonLogDbConnectionString = ConfigurationManager.AppSettings["LogonLogDbConnection"];
            }

            if (ConfigurationManager.AppSettings["WebAppDbConnection"] != null)
            {
                BaseSystemInfo.WebAppDbConnectionString = ConfigurationManager.AppSettings["WebAppDbConnection"];
            }
            if (ConfigurationManager.AppSettings["BPMDbConnection"] != null)
            {
                BaseSystemInfo.BpmDbConnectionString = ConfigurationManager.AppSettings["BPMDbConnection"];
            }
            if (ConfigurationManager.AppSettings["ERPDbConnection"] != null)
            {
                BaseSystemInfo.ErpDbConnectionString = ConfigurationManager.AppSettings["ERPDbConnection"];
            }
            if (ConfigurationManager.AppSettings["WMSDbConnection"] != null)
            {
                BaseSystemInfo.WmsDbConnectionString = ConfigurationManager.AppSettings["WMSDbConnection"];
            }
            if (ConfigurationManager.AppSettings["ZBWMSDbConnection"] != null)
            {
                BaseSystemInfo.ZbwmsDbConnectionString = ConfigurationManager.AppSettings["ZBWMSDbConnection"];
            }
            if (ConfigurationManager.AppSettings["SPWMSDbConnection"] != null)
            {
                BaseSystemInfo.SpwmsDbConnectionString = ConfigurationManager.AppSettings["SPWMSDbConnection"];
            }
            if (ConfigurationManager.AppSettings["MESDbConnection"] != null)
            {
                BaseSystemInfo.MesDbConnectionString = ConfigurationManager.AppSettings["MESDbConnection"];
            }
            if (ConfigurationManager.AppSettings["HRMDbConnection"] != null)
            {
                BaseSystemInfo.HrmDbConnectionString = ConfigurationManager.AppSettings["HRMDbConnection"];
            }
            if (ConfigurationManager.AppSettings["CRMDbConnection"] != null)
            {
                BaseSystemInfo.CrmDbConnectionString = ConfigurationManager.AppSettings["CRMDbConnection"];
            }
            if (ConfigurationManager.AppSettings["OADbConnection"] != null)
            {
                BaseSystemInfo.OaDbConnectionString = ConfigurationManager.AppSettings["OADbConnection"];
            }
            if (ConfigurationManager.AppSettings["LabelDbConnection"] != null)
            {
                BaseSystemInfo.LabelDbConnectionString = ConfigurationManager.AppSettings["LabelDbConnection"];
            }
            if (ConfigurationManager.AppSettings["WebDbConnection"] != null)
            {
                BaseSystemInfo.WebDbConnectionString = ConfigurationManager.AppSettings["WebDbConnection"];
            }
            if (ConfigurationManager.AppSettings["CMSDbConnection"] != null)
            {
                BaseSystemInfo.CmsDbConnectionString = ConfigurationManager.AppSettings["CMSDbConnection"];
            }
            if (ConfigurationManager.AppSettings["DTcmsDbConnection"] != null)
            {
                BaseSystemInfo.DTcmsDbConnectionString = ConfigurationManager.AppSettings["DTcmsDbConnection"];
            }
            if (ConfigurationManager.AppSettings["FlowPortalDbConnection"] != null)
            {
                BaseSystemInfo.FlowPortalDbConnectionString = ConfigurationManager.AppSettings["FlowPortalDbConnection"];
            }
            if (ConfigurationManager.AppSettings["DealerPortalDbConnection"] != null)
            {
                BaseSystemInfo.DealerPortalDbConnectionString = ConfigurationManager.AppSettings["DealerPortalDbConnection"];
            }
            if (ConfigurationManager.AppSettings["CustomerPortalDbConnection"] != null)
            {
                BaseSystemInfo.CustomerPortalDbConnectionString = ConfigurationManager.AppSettings["CustomerPortalDbConnection"];
            }
            if (ConfigurationManager.AppSettings["SupplierPortalDbConnection"] != null)
            {
                BaseSystemInfo.SupplierPortalDbConnectionString = ConfigurationManager.AppSettings["SupplierPortalDbConnection"];
            }
            if (ConfigurationManager.AppSettings["ReportDbConnection"] != null)
            {
                BaseSystemInfo.ReportDbConnectionString = ConfigurationManager.AppSettings["ReportDbConnection"];
            }
            if (ConfigurationManager.AppSettings["ScmDbConnection"] != null)
            {
                BaseSystemInfo.ScmDbConnectionString = ConfigurationManager.AppSettings["ScmDbConnection"];
            }
            if (ConfigurationManager.AppSettings["ImsDbConnection"] != null)
            {
                BaseSystemInfo.ImsDbConnectionString = ConfigurationManager.AppSettings["ImsDbConnection"];
            }
            if (ConfigurationManager.AppSettings["OmsDbConnection"] != null)
            {
                BaseSystemInfo.OmsDbConnectionString = ConfigurationManager.AppSettings["OmsDbConnection"];
            }
            if (ConfigurationManager.AppSettings["MemberDbConnection"] != null)
            {
                BaseSystemInfo.MemberDbConnectionString = ConfigurationManager.AppSettings["MemberDbConnection"];
            }
            if (ConfigurationManager.AppSettings["BudgetDbConnection"] != null)
            {
                BaseSystemInfo.BudgetDbConnectionString = ConfigurationManager.AppSettings["BudgetDbConnection"];
            }
            if (ConfigurationManager.AppSettings["ItamsDbConnection"] != null)
            {
                BaseSystemInfo.ItamsDbConnectionString = ConfigurationManager.AppSettings["ItamsDbConnection"];
            }
            if (ConfigurationManager.AppSettings["CardTicketDbConnection"] != null)
            {
                BaseSystemInfo.CardTicketDbConnectionString = ConfigurationManager.AppSettings["CardTicketDbConnection"];
            }
            if (ConfigurationManager.AppSettings["MDMDbConnection"] != null)
            {
                BaseSystemInfo.MdmDbConnectionString = ConfigurationManager.AppSettings["MDMDbConnection"];
            }
            // 对加密的数据库连接进行解密操作
            if (BaseSystemInfo.EncryptDbConnection)
            {
                BaseSystemInfo.UserCenterDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.UserCenterDbConnectionString);
                BaseSystemInfo.BusinessDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.BusinessDbConnectionString);
                BaseSystemInfo.MessageDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.MessageDbConnectionString);
                BaseSystemInfo.WorkFlowDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.WorkFlowDbConnectionString);
                BaseSystemInfo.LogonLogDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.LogonLogDbConnectionString);

                BaseSystemInfo.WebAppDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.WebAppDbConnectionString);
                BaseSystemInfo.BpmDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.BpmDbConnectionString);
                BaseSystemInfo.ErpDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.ErpDbConnectionString);
                BaseSystemInfo.WmsDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.WmsDbConnectionString);
                BaseSystemInfo.CwmsDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.CwmsDbConnectionString);
                BaseSystemInfo.ZbwmsDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.ZbwmsDbConnectionString);
                BaseSystemInfo.SpwmsDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.SpwmsDbConnectionString);
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
                BaseSystemInfo.ReportDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.ReportDbConnectionString);
                BaseSystemInfo.ScmDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.ScmDbConnectionString);
                BaseSystemInfo.CscmDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.CscmDbConnectionString);
                BaseSystemInfo.ImsDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.ImsDbConnectionString);
                BaseSystemInfo.IcsDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.IcsDbConnectionString);
                BaseSystemInfo.OmsDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.OmsDbConnectionString);
                BaseSystemInfo.MemberDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.MemberDbConnectionString);
                BaseSystemInfo.BudgetDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.BudgetDbConnectionString);
                BaseSystemInfo.ItamsDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.ItamsDbConnectionString);
                BaseSystemInfo.CardTicketDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.CardTicketDbConnectionString);
                BaseSystemInfo.MdmDbConnection = SecretUtil.DesDecrypt(BaseSystemInfo.MdmDbConnectionString);
            }
            else
            {
                BaseSystemInfo.UserCenterDbConnection = BaseSystemInfo.UserCenterDbConnectionString;
                BaseSystemInfo.BusinessDbConnection = BaseSystemInfo.BusinessDbConnectionString;
                BaseSystemInfo.MessageDbConnection = BaseSystemInfo.MessageDbConnectionString;
                BaseSystemInfo.WorkFlowDbConnection = BaseSystemInfo.WorkFlowDbConnectionString;
                BaseSystemInfo.LogonLogDbConnection = BaseSystemInfo.LogonLogDbConnectionString;

                BaseSystemInfo.WebAppDbConnection = BaseSystemInfo.WebAppDbConnectionString;
                BaseSystemInfo.BpmDbConnection = BaseSystemInfo.BpmDbConnectionString;
                BaseSystemInfo.ErpDbConnection = BaseSystemInfo.ErpDbConnectionString;
                BaseSystemInfo.WmsDbConnection = BaseSystemInfo.WmsDbConnectionString;
                BaseSystemInfo.CwmsDbConnection = BaseSystemInfo.CwmsDbConnectionString;
                BaseSystemInfo.ZbwmsDbConnection = BaseSystemInfo.ZbwmsDbConnectionString;
                BaseSystemInfo.SpwmsDbConnection = BaseSystemInfo.SpwmsDbConnectionString;
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
                BaseSystemInfo.CscmDbConnection = BaseSystemInfo.CscmDbConnectionString;
                BaseSystemInfo.ImsDbConnection = BaseSystemInfo.ImsDbConnectionString;
                BaseSystemInfo.IcsDbConnection = BaseSystemInfo.IcsDbConnectionString;
                BaseSystemInfo.OmsDbConnection = BaseSystemInfo.OmsDbConnectionString;
                BaseSystemInfo.MemberDbConnection = BaseSystemInfo.MemberDbConnectionString;
                BaseSystemInfo.BudgetDbConnection = BaseSystemInfo.BudgetDbConnectionString;
                BaseSystemInfo.ItamsDbConnection = BaseSystemInfo.ItamsDbConnectionString;
                BaseSystemInfo.CardTicketDbConnection = BaseSystemInfo.CardTicketDbConnectionString;
                BaseSystemInfo.MdmDbConnection = BaseSystemInfo.MdmDbConnectionString;
            }

            BaseSystemInfo.UserCenterReadDbConnection = BaseSystemInfo.UserCenterDbConnection;
            BaseSystemInfo.UserCenterWriteDbConnection = BaseSystemInfo.UserCenterDbConnection;

            // 这里重新给静态数据库连接对象进行赋值
            // DotNet.Util.DbTypeUtil.DbConnection = BaseSystemInfo.BusinessDbConnection;
            // DotNet.Util.DbTypeUtil.DbType = BaseSystemInfo.BusinessDbType;

            // 这里是处理读写分离功能，读取数据与写入数据进行分离的方式
            if (ConfigurationManager.AppSettings["UserCenterReadDbConnection"] != null)
            {
                BaseSystemInfo.UserCenterReadDbConnection = ConfigurationManager.AppSettings["UserCenterReadDbConnection"];
            }
            if (ConfigurationManager.AppSettings["UserCenterWriteDbConnection"] != null)
            {
                BaseSystemInfo.UserCenterWriteDbConnection = ConfigurationManager.AppSettings["UserCenterWriteDbConnection"];
            }

            if (ConfigurationManager.AppSettings["DatabaseTableVersion"] != null)
            {
                if (ValidateUtil.IsInt(ConfigurationManager.AppSettings["DatabaseTableVersion"]))
                {
                    BaseSystemInfo.DatabaseTableVersion = ConfigurationManager.AppSettings["DatabaseTableVersion"].ToInt();
                }
            }
        }
        #endregion

        #region Mail邮件配置信息
        /// <summary>
        /// Mail邮件配置信息
        /// </summary>
        public static void GetMailConfig()
        {
            //SMTP邮件服务器
            if (ConfigurationManager.AppSettings["MailServer"] != null)
            {
                BaseSystemInfo.MailServer = ConfigurationManager.AppSettings["MailServer"];
            }
            if (ConfigurationManager.AppSettings["MailServerPort"] != null)
            {
                if (ValidateUtil.IsInt(ConfigurationManager.AppSettings["MailServerPort"]))
                {
                    BaseSystemInfo.MailServerPort = ConfigurationManager.AppSettings["MailServerPort"].ToInt();
                }
            }
            if (ConfigurationManager.AppSettings["MailServerSslEnabled"] != null)
            {
                BaseSystemInfo.MailServerSslEnabled = ConfigurationManager.AppSettings["MailServerSslEnabled"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["MailUserName"] != null)
            {
                BaseSystemInfo.MailUserName = ConfigurationManager.AppSettings["MailUserName"];
            }
            if (ConfigurationManager.AppSettings["MailPassword"] != null)
            {
                BaseSystemInfo.MailPassword = ConfigurationManager.AppSettings["MailPassword"];
                if (BaseSystemInfo.EncryptDbConnection)
                {
                    BaseSystemInfo.MailPassword = SecretUtil.DesDecrypt(BaseSystemInfo.MailPassword);
                }
            }
            if (ConfigurationManager.AppSettings["MailFrom"] != null)
            {
                BaseSystemInfo.MailFrom = ConfigurationManager.AppSettings["MailFrom"];
            }
            if (ConfigurationManager.AppSettings["MailBCC"] != null)
            {
                BaseSystemInfo.MailBcc = ConfigurationManager.AppSettings["MailBCC"];
            }
        }
        #endregion

        #region MQTT配置信息
        /// <summary>
        /// MQTT配置信息
        /// </summary>
        public static void GetMqttConfig()
        {
            if (ConfigurationManager.AppSettings["Mqtterver"] != null)
            {
                BaseSystemInfo.MqttServer = ConfigurationManager.AppSettings["MqttServer"];
            }

            if (ConfigurationManager.AppSettings["MqttPort"] != null)
            {
                if (ValidateUtil.IsInt(ConfigurationManager.AppSettings["MqttPort"]))
                {
                    BaseSystemInfo.MqttPort = ConfigurationManager.AppSettings["MqttPort"].ToInt();
                }
            }

            if (ConfigurationManager.AppSettings["MqttSslEnabled"] != null)
            {
                BaseSystemInfo.MqttSslEnabled = ConfigurationManager.AppSettings["MqttSslEnabled"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }

            if (ConfigurationManager.AppSettings["MqttUserName"] != null)
            {
                BaseSystemInfo.MqttUserName = ConfigurationManager.AppSettings["MqttUserName"];
            }

            if (ConfigurationManager.AppSettings["MqttPassword"] != null)
            {
                BaseSystemInfo.MqttPassword = ConfigurationManager.AppSettings["MqttPassword"];
            }
        }
        #endregion

        #region WebApi配置信息
        /// <summary>
        /// WebApi配置信息
        /// </summary>
        public static void GetWebApiConfig()
        {
            if (ConfigurationManager.AppSettings["WebApiMonitorEnabled"] != null)
            {
                BaseSystemInfo.WebApiMonitorEnabled = ConfigurationManager.AppSettings["WebApiMonitorEnabled"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }

            if (ConfigurationManager.AppSettings["WebApiMonitorFolder"] != null)
            {
                BaseSystemInfo.WebApiMonitorFolder = ConfigurationManager.AppSettings["WebApiMonitorFolder"];
            }

            if (ConfigurationManager.AppSettings["WebApiSlowMonitorEnabled"] != null)
            {
                BaseSystemInfo.WebApiSlowMonitorEnabled = ConfigurationManager.AppSettings["WebApiSlowMonitorEnabled"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }

            if (ConfigurationManager.AppSettings["WebApiSlowMonitorFolder"] != null)
            {
                BaseSystemInfo.WebApiSlowMonitorFolder = ConfigurationManager.AppSettings["WebApiSlowMonitorFolder"];
            }

            if (ConfigurationManager.AppSettings["WebApiSlowResponseMilliseconds"] != null)
            {
                if (ValidateUtil.IsInt(ConfigurationManager.AppSettings["WebApiSlowResponseMilliseconds"]))
                {
                    BaseSystemInfo.WebApiSlowResponseMilliseconds = ConfigurationManager.AppSettings["WebApiSlowResponseMilliseconds"].ToInt();
                }
            }
        }
        #endregion

        #region User用户配置信息
        /// <summary>
        /// BaseUser用户配置信息
        /// </summary>
        public static void GetUserConfig()
        {
            // 用户名强制手机号
            if (ConfigurationManager.AppSettings["UserNameForceMobileNumber"] != null)
            {
                BaseSystemInfo.UserNameForceMobileNumber = ConfigurationManager.AppSettings["UserNameForceMobileNumber"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            // 用户名强制大小写
            if (ConfigurationManager.AppSettings["UserNameMatchCase"] != null)
            {
                BaseSystemInfo.UserNameMatchCase = ConfigurationManager.AppSettings["UserNameMatchCase"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["CheckOnline"] != null)
            {
                BaseSystemInfo.CheckOnline = ConfigurationManager.AppSettings["CheckOnline"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            //是否允许用户注册
            if (ConfigurationManager.AppSettings["AllowUserRegister"] != null)
            {
                BaseSystemInfo.AllowUserRegister = ConfigurationManager.AppSettings["AllowUserRegister"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }

            if (ConfigurationManager.AppSettings["OnlineTimeout"] != null)
            {
                if (ValidateUtil.IsInt(ConfigurationManager.AppSettings["OnlineTimeout"]))
                {
                    BaseSystemInfo.OnlineTimeout = ConfigurationManager.AppSettings["OnlineTimeout"].ToInt();
                }
            }

            if (ConfigurationManager.AppSettings["CheckPasswordStrength"] != null)
            {
                BaseSystemInfo.CheckPasswordStrength = ConfigurationManager.AppSettings["CheckPasswordStrength"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
        }
        #endregion

        #region Cookie配置信息
        /// <summary>
        /// Cookie配置信息
        /// </summary>
        public static void GetCookieConfig()
        {
            if (ConfigurationManager.AppSettings["CookieName"] != null)
            {
                BaseSystemInfo.CookieName = ConfigurationManager.AppSettings["CookieName"];
            }
            if (ConfigurationManager.AppSettings["CookieDomain"] != null)
            {
                BaseSystemInfo.CookieDomain = ConfigurationManager.AppSettings["CookieDomain"];
            }
            if (ConfigurationManager.AppSettings["CookieExpires"] != null)
            {
                if (ValidateUtil.IsInt(ConfigurationManager.AppSettings["CookieExpires"]))
                {
                    BaseSystemInfo.CookieExpires = ConfigurationManager.AppSettings["CookieExpires"].ToInt();
                }
            }
        }
        #endregion

        #region WebApp基本配置信息
        /// <summary>
        /// WebApp基本配置信息
        /// </summary>
        public static void GetWebAppConfig()
        {
            if (ConfigurationManager.AppSettings["SystemCode"] != null)
            {
                BaseSystemInfo.SystemCode = ConfigurationManager.AppSettings["SystemCode"];
            }

            if (ConfigurationManager.AppSettings["ForceHttps"] != null)
            {
                BaseSystemInfo.ForceHttps = ConfigurationManager.AppSettings["ForceHttps"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["IsProduction"] != null)
            {
                BaseSystemInfo.IsProduction = ConfigurationManager.AppSettings["IsProduction"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }

            if (ConfigurationManager.AppSettings["ApplicationId"] != null)
            {
                BaseSystemInfo.ApplicationId = ConfigurationManager.AppSettings["ApplicationId"];
            }
#if NET452_OR_GREATER
            if (string.IsNullOrEmpty(BaseSystemInfo.ApplicationId))
            {
                if (System.Web.Hosting.HostingEnvironment.ApplicationID != null)
                {
                    BaseSystemInfo.ApplicationId = System.Web.Hosting.HostingEnvironment.ApplicationID.Replace("/", "");
                }
            }
#endif

            if (ConfigurationManager.AppSettings["MainPage"] != null)
            {
                BaseSystemInfo.MainPage = ConfigurationManager.AppSettings["MainPage"];
            }
            if (ConfigurationManager.AppSettings["RootMenuCode"] != null)
            {
                BaseSystemInfo.RootMenuCode = ConfigurationManager.AppSettings["RootMenuCode"];
            }
            if (ConfigurationManager.AppSettings["SmsEnabled"] != null)
            {
                BaseSystemInfo.SmsEnabled = ConfigurationManager.AppSettings["SmsEnabled"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["IsWindowsAuthentication"] != null)
            {
                BaseSystemInfo.IsWindowsAuthentication = ConfigurationManager.AppSettings["IsWindowsAuthentication"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            if (ConfigurationManager.AppSettings["UploadFileExtension"] != null)
            {
                BaseSystemInfo.UploadFileExtension = ConfigurationManager.AppSettings["UploadFileExtension"];
            }
            if (ConfigurationManager.AppSettings["UploadVideoExtension"] != null)
            {
                BaseSystemInfo.UploadVideoExtension = ConfigurationManager.AppSettings["UploadVideoExtension"];
            }
            if (ConfigurationManager.AppSettings["UploadAudioExtension"] != null)
            {
                BaseSystemInfo.UploadAudioExtension = ConfigurationManager.AppSettings["UploadAudioExtension"];
            }
            if (ConfigurationManager.AppSettings["UserRolePrefix"] != null)
            {
                BaseSystemInfo.UserRolePrefix = ConfigurationManager.AppSettings["UserRolePrefix"];
            }
            if (ConfigurationManager.AppSettings["ExcludedUserRolePrefix"] != null)
            {
                BaseSystemInfo.ExcludedUserRolePrefix = ConfigurationManager.AppSettings["ExcludedUserRolePrefix"];
            }

            if (ConfigurationManager.AppSettings["PasswordErrorLockLimit"] != null)
            {
                BaseSystemInfo.PasswordErrorLockLimit = ConfigurationManager.AppSettings["PasswordErrorLockLimit"].ToInt();
            }
            if (ConfigurationManager.AppSettings["PasswordErrorLockCycle"] != null)
            {
                BaseSystemInfo.PasswordErrorLockCycle = ConfigurationManager.AppSettings["PasswordErrorLockCycle"].ToInt();
            }
        }
        #endregion

        #region FTP配置信息
        /// <summary>
        /// FTP配置信息
        /// </summary>
        public static void GetFtpConfig()
        {
            if (ConfigurationManager.AppSettings["Ftperver"] != null)
            {
                BaseSystemInfo.FtpServer = ConfigurationManager.AppSettings["FtpServer"];
            }

            if (ConfigurationManager.AppSettings["FtpPort"] != null)
            {
                if (ValidateUtil.IsInt(ConfigurationManager.AppSettings["FtpPort"]))
                {
                    BaseSystemInfo.FtpPort = ConfigurationManager.AppSettings["FtpPort"].ToInt();
                }
            }

            if (ConfigurationManager.AppSettings["FtpSslEnabled"] != null)
            {
                BaseSystemInfo.FtpSslEnabled = ConfigurationManager.AppSettings["FtpSslEnabled"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }

            if (ConfigurationManager.AppSettings["FtpUserName"] != null)
            {
                BaseSystemInfo.FtpUserName = ConfigurationManager.AppSettings["FtpUserName"];
            }

            if (ConfigurationManager.AppSettings["FtpPassword"] != null)
            {
                BaseSystemInfo.FtpPassword = ConfigurationManager.AppSettings["FtpPassword"];
            }
        }
        #endregion

        #region Redis配置信息
        /// <summary>
        /// Redis配置信息
        /// </summary>
        public static void GetRedisConfig()
        {
            //Redis配置
            if (ConfigurationManager.AppSettings["RedisEnabled"] != null)
            {
                BaseSystemInfo.RedisEnabled = ConfigurationManager.AppSettings["RedisEnabled"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }

            if (ConfigurationManager.AppSettings["RedisServer"] != null)
            {
                BaseSystemInfo.RedisServer = ConfigurationManager.AppSettings["RedisServer"];
            }

            if (ConfigurationManager.AppSettings["RedisPort"] != null)
            {
                if (ValidateUtil.IsInt(ConfigurationManager.AppSettings["RedisPort"]))
                {
                    BaseSystemInfo.RedisPort = ConfigurationManager.AppSettings["RedisPort"].ToInt();
                }
            }

            if (ConfigurationManager.AppSettings["RedisInitialDb"] != null)
            {
                if (ValidateUtil.IsInt(ConfigurationManager.AppSettings["RedisInitialDb"]))
                {
                    BaseSystemInfo.RedisInitialDb = long.Parse(ConfigurationManager.AppSettings["RedisInitialDb"]);
                }
            }

            if (ConfigurationManager.AppSettings["RedisSslEnabled"] != null)
            {
                BaseSystemInfo.RedisSslEnabled = ConfigurationManager.AppSettings["RedisSslEnabled"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);
            }

            if (ConfigurationManager.AppSettings["RedisUserName"] != null)
            {
                BaseSystemInfo.RedisUserName = ConfigurationManager.AppSettings["RedisUserName"];
            }

            if (ConfigurationManager.AppSettings["RedisPassword"] != null)
            {
                BaseSystemInfo.RedisPassword = ConfigurationManager.AppSettings["RedisPassword"];
            }

            if (ConfigurationManager.AppSettings["RedisCacheMillisecond"] != null)
            {
                if (ValidateUtil.IsInt(ConfigurationManager.AppSettings["RedisCacheMillisecond"]))
                {
                    BaseSystemInfo.RedisCacheMillisecond = ConfigurationManager.AppSettings["RedisCacheMillisecond"].ToInt();
                }
            }

            if (ConfigurationManager.AppSettings["MemoryCacheMillisecond"] != null)
            {
                if (ValidateUtil.IsInt(ConfigurationManager.AppSettings["MemoryCacheMillisecond"]))
                {
                    BaseSystemInfo.MemoryCacheMillisecond = ConfigurationManager.AppSettings["MemoryCacheMillisecond"].ToInt();
                }
            }
        }
        #endregion
    }
}