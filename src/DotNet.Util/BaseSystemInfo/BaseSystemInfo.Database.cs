//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2026, DotNet.
//-----------------------------------------------------------------

using System.Configuration;

namespace DotNet.Util
{
    /// <summary>
    /// BaseSystemInfo
    /// 这是系统的核心基础信息部分
    /// 
    /// 修改记录
    ///
    ///     2024.06.28 版本：1.9 Troy Cui	增加常用外部系统的数据库连接:CWMS,CSCM
    ///     2020.08.16 版本：1.9 Troy Cui	增加常用外部系统的数据库连接:OMS
    ///     2018.10.31 版本：1.9 Troy Cui	增加常用外部系统的数据库连接:DealerPortal
    ///     2018.08.09 版本：1.8 Troy Cui	增加常用外部系统的数据库连接:SCM,IMS,Member
    ///     2018.08.08 版本：1.7 Troy Cui	增加常用外部系统的数据库连接:Report
    ///     2018.08.01 版本：1.6 Troy Cui	增加常用外部系统的数据库连接:CustomerPortal,SupplierPortal
    ///     2018.03.11 版本：1.5 Troy Cui	增加常用外部系统的数据库连接:DTcms,FlowPortal
    ///     2017.12.05 版本：1.4 Troy Cui	重新布局并增加常用外部系统的数据库连接:BPM,ERP,MES,HRM,CRM,OA,Web,CMS
    ///     2017.02.05 版本：1.3 Troy Cui	增加WebApp数据库连接
    ///     2016.12.05 版本：1.2 Troy Cui	增加Label数据库连接
    ///		2015.02.03 版本：1.1 JiRiGaLa	登录日志很庞大时，需要有专门的登录日志服务器，因为大家会查自己的登录日志是否安全。
    ///		2012.04.14 版本：1.0 JiRiGaLa	主键创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.02.03</date>
    /// </author>
    /// </summary>
    public partial class BaseSystemInfo
    {
        /// <summary>
        /// 慢查询毫秒数
        /// </summary>
        public static int SlowQueryMilliseconds = 3000;

        /// <summary>
        /// 用户数据库类别
        /// </summary>
        public static CurrentDbType ServerDbType = CurrentDbType.SqlServer;

        /// <summary>
        /// 是否加密数据库连接
        /// </summary>
        public static bool EncryptDbConnection = false;

        #region UserCenter
        /// <summary>
        /// 用户数据库类别
        /// </summary>
        public static CurrentDbType UserCenterDbType = CurrentDbType.SqlServer;
        private static string _userCenterDbConnection = string.Empty;
        /// <summary>
        /// 数据库连接(不进行读写分离)
        /// </summary>
        public static string UserCenterDbConnection
        {
            get
            {
                if (_userCenterDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["UserCenterDbConnection"].IsNullOrEmpty())
                    {
                        _userCenterDbConnection = ConfigurationManager.AppSettings["UserCenterDbConnection"];
                    }
                    if (_userCenterDbConnection.IsNullOrEmpty())
                    {
                        _userCenterDbConnection = "Data Source=localhost;Initial Catalog=UserCenterV" + BaseSystemInfo.DatabaseTableVersion + ";Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _userCenterDbConnection;
            }
            set
            {
                // 写入的数据库连接
                _userCenterWriteDbConnection = value;
                // 读取的数据库连接
                _userCenterReadDbConnection = value;
                // 默认的数据库连接
                _userCenterDbConnection = value;
            }
        }

        private static string _userCenterWriteDbConnection = string.Empty;
        /// <summary>
        /// 数据库连接(读取的数据库连接、主要目标是为了读写分离)
        /// </summary>
        public static string UserCenterWriteDbConnection
        {
            get
            {
                if (_userCenterWriteDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["UserCenterDbConnection"].IsNullOrEmpty())
                    {
                        _userCenterWriteDbConnection = ConfigurationManager.AppSettings["UserCenterDbConnection"];
                    }
                    if (_userCenterWriteDbConnection.IsNullOrEmpty())
                    {
                        _userCenterWriteDbConnection = "Data Source=localhost;Initial Catalog=UserCenterV" + BaseSystemInfo.DatabaseTableVersion + ";Integrated Security=SSPI;";
                    }
                }
                return _userCenterWriteDbConnection;
            }
            set => _userCenterWriteDbConnection = value;
        }

        private static string _userCenterReadDbConnection = string.Empty;
        /// <summary>
        /// 数据库连接(读取的数据库连接、主要目标是为了读写分离)
        /// </summary>
        public static string UserCenterReadDbConnection
        {
            get
            {
                if (_userCenterReadDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["UserCenterDbConnection"].IsNullOrEmpty())
                    {
                        _userCenterReadDbConnection = ConfigurationManager.AppSettings["UserCenterDbConnection"];
                    }
                    if (_userCenterReadDbConnection.IsNullOrEmpty())
                    {
                        _userCenterReadDbConnection = "Data Source=localhost;Initial Catalog=UserCenterV" + BaseSystemInfo.DatabaseTableVersion + ";Integrated Security=SSPI;";
                    }
                }
                return _userCenterReadDbConnection;
            }
            set => _userCenterReadDbConnection = value;
        }

        /// <summary>
        /// 数据库连接的字符串
        /// </summary>
        public static string UserCenterDbConnectionString = string.Empty;

        #endregion

        #region  Message
        /// <summary>
        /// 消息数据库类别
        /// </summary>
        public static CurrentDbType MessageDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// 消息数据库
        /// </summary>
        private static string _messageDbConnection = string.Empty;
        /// <summary>
        /// 消息数据库连接
        /// </summary>
        public static string MessageDbConnection
        {
            get
            {
                if (_messageDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["MessageDbConnection"].IsNullOrEmpty())
                    {
                        _messageDbConnection = ConfigurationManager.AppSettings["MessageDbConnection"];
                    }
                    if (_messageDbConnection.IsNullOrEmpty())
                    {
                        _messageDbConnection = "Data Source=localhost;Initial Catalog=Business_Message;Integrated Security=SSPI;";
                    }
                }
                // 默认的消息数据库连接
                return _messageDbConnection;
            }
            // 默认的消息数据库连接
            set => _messageDbConnection = value;
        }

        /// <summary>
        /// 消息数据库连接的字符串
        /// </summary>
        public static string MessageDbConnectionString = string.Empty;

        #endregion

        #region Business
        /// <summary>
        /// 业务数据库类别
        /// </summary>
        public static CurrentDbType BusinessDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// 业务数据库
        /// </summary>
        private static string _businessDbConnection = string.Empty;
        /// <summary>
        /// 业务数据库连接
        /// </summary>
        public static string BusinessDbConnection
        {
            get
            {
                if (_businessDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["BusinessDbConnection"].IsNullOrEmpty())
                    {
                        _businessDbConnection = ConfigurationManager.AppSettings["BusinessDbConnection"];
                    }
                    if (_businessDbConnection.IsNullOrEmpty())
                    {
                        _businessDbConnection = "Data Source=localhost;Initial Catalog=Business_Project;Integrated Security=SSPI;";
                    }
                }
                // 默认的业务数据库连接
                return _businessDbConnection;
            }
            // 默认的业务数据库连接
            set => _businessDbConnection = value;
        }
        /// <summary>
        /// 业务数据库（连接串，可能是加密的）
        /// </summary>
        public static string BusinessDbConnectionString = string.Empty;

        #endregion

        #region WorkFlow
        /// <summary>
        /// 工作流数据库类别
        /// </summary>
        public static CurrentDbType WorkFlowDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// 工作流数据库
        /// </summary>
        // public static string WorkFlowDbConnection = "Data Source=localhost;Initial Catalog=WorkFlowV" + BaseSystemInfo.DatabaseTableVersion + ";Integrated Security=SSPI;";
        private static string _workflowDbConnection = string.Empty;
        /// <summary>
        /// 工作流数据库连接
        /// </summary>
        public static string WorkFlowDbConnection
        {
            get
            {
                if (_workflowDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["WorkFlowDbConnection"].IsNullOrEmpty())
                    {
                        _workflowDbConnection = ConfigurationManager.AppSettings["WorkFlowDbConnection"];
                    }
                    if (_workflowDbConnection.IsNullOrEmpty())
                    {
                        _workflowDbConnection = "Data Source=localhost;Initial Catalog=WorkFlowV" + BaseSystemInfo.DatabaseTableVersion + ";Integrated Security=SSPI;";
                    }
                }
                // 默认的工作流数据库连接
                return _workflowDbConnection;
            }
            // 默认的工作流数据库连接
            set => _workflowDbConnection = value;
        }
        /// <summary>
        /// 工作流数据库（连接串，可能是加密的）
        /// </summary>
        public static string WorkFlowDbConnectionString = string.Empty;

        #endregion

        #region LogonLog
        /// <summary>
        /// 登录日志数据库类别
        /// </summary>
        public static CurrentDbType LogonLogDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// 登录日志数据库
        /// </summary>
        private static string _loginLogDbConnection = string.Empty;

        /// <summary>
        /// 登录日志数据库
        /// </summary>
        public static string LogonLogDbConnection
        {
            get
            {
                if (_loginLogDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["LogonLogDbConnection"].IsNullOrEmpty())
                    {
                        _loginLogDbConnection = ConfigurationManager.AppSettings["LogonLogDbConnection"];
                    }
                    if (_loginLogDbConnection.IsNullOrEmpty())
                    {
                        _loginLogDbConnection = "Data Source=localhost;Initial Catalog=UserCenterV" + BaseSystemInfo.DatabaseTableVersion + ";Integrated Security=SSPI;";
                    }
                }
                // 默认的登录日志数据库连接
                return _loginLogDbConnection;
            }
            // 登录日志数据库连接
            set => _loginLogDbConnection = value;
        }

        /// <summary>
        /// 登录日志据库（连接串，可能是加密的）
        /// </summary>
        public static string LogonLogDbConnectionString = string.Empty;

        #endregion

        #region WebApp
        /// <summary>
        /// WebApp数据库类别
        /// </summary>
        public static CurrentDbType WebAppDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// WebApp数据库
        /// </summary>
        private static string _webAppDbConnection = string.Empty;
        /// <summary>
        /// WebApp数据库连接
        /// </summary>
        public static string WebAppDbConnection
        {
            get
            {
                if (_webAppDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["WebAppDbConnection"].IsNullOrEmpty())
                    {
                        _webAppDbConnection = ConfigurationManager.AppSettings["WebAppDbConnection"];
                    }
                    if (_webAppDbConnection.IsNullOrEmpty())
                    {
                        _webAppDbConnection = "Data Source=localhost;Initial Catalog=Business_WebApp;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _webAppDbConnection;
            }
            // 默认的数据库连接
            set => _webAppDbConnection = value;
        }
        /// <summary>
        /// WebApp数据库（连接串，可能是加密的）
        /// </summary>
        public static string WebAppDbConnectionString = string.Empty;
        #endregion

        #region BPM
        /// <summary>
        /// BPM数据库类别
        /// </summary>
        public static CurrentDbType BpmDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// BPM数据库
        /// </summary>
        private static string _bpmDbConnection = string.Empty;
        /// <summary>
        /// BPM数据库连接
        /// </summary>
        public static string BpmDbConnection
        {
            get
            {
                if (_bpmDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["BPMDbConnection"].IsNullOrEmpty())
                    {
                        _bpmDbConnection = ConfigurationManager.AppSettings["BPMDbConnection"];
                    }
                    if (_bpmDbConnection.IsNullOrEmpty())
                    {
                        _bpmDbConnection = "Data Source=localhost;Initial Catalog=Business_BPM;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _bpmDbConnection;
            }
            // 默认的数据库连接
            set => _bpmDbConnection = value;
        }
        /// <summary>
        /// BPM数据库（连接串，可能是加密的）
        /// </summary>
        public static string BpmDbConnectionString = string.Empty;
        #endregion

        #region ERP
        /// <summary>
        /// ERP数据库类别
        /// </summary>
        public static CurrentDbType ErpDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// ERP数据库
        /// </summary>
        private static string _erpDbConnection = string.Empty;
        /// <summary>
        /// ERP数据库连接
        /// </summary>
        public static string ErpDbConnection
        {
            get
            {
                if (_erpDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["ERPDbConnection"].IsNullOrEmpty())
                    {
                        _erpDbConnection = ConfigurationManager.AppSettings["ERPDbConnection"];
                    }
                    if (_erpDbConnection.IsNullOrEmpty())
                    {
                        _erpDbConnection = "Data Source=localhost;Initial Catalog=Business_ERP;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _erpDbConnection;
            }
            // 默认的数据库连接
            set => _erpDbConnection = value;
        }
        /// <summary>
        /// ERP数据库（连接串，可能是加密的）
        /// </summary>
        public static string ErpDbConnectionString = string.Empty;
        #endregion

        #region WMS
        /// <summary>
        /// WMS数据库类别
        /// </summary>
        public static CurrentDbType WmsDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// WMS数据库
        /// </summary>
        private static string _wmsDbConnection = string.Empty;
        /// <summary>
        /// WMS数据库连接
        /// </summary>
        public static string WmsDbConnection
        {
            get
            {
                if (_wmsDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["WMSDbConnection"].IsNullOrEmpty())
                    {
                        _wmsDbConnection = ConfigurationManager.AppSettings["WMSDbConnection"];
                    }
                    if (_wmsDbConnection.IsNullOrEmpty())
                    {
                        _wmsDbConnection = "Data Source=localhost;Initial Catalog=Business_WMS;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _wmsDbConnection;
            }
            // 默认的数据库连接
            set => _wmsDbConnection = value;
        }
        /// <summary>
        /// WMS数据库（连接串，可能是加密的）
        /// </summary>
        public static string WmsDbConnectionString = string.Empty;
        #endregion

        #region CWMS
        /// <summary>
        /// CWMS数据库类别
        /// </summary>
        public static CurrentDbType CwmsDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// CWMS数据库
        /// </summary>
        private static string _cwmsDbConnection = string.Empty;
        /// <summary>
        /// CWMS数据库连接
        /// </summary>
        public static string CwmsDbConnection
        {
            get
            {
                if (_cwmsDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["CWMSDbConnection"].IsNullOrEmpty())
                    {
                        _cwmsDbConnection = ConfigurationManager.AppSettings["CWMSDbConnection"];
                    }
                    if (_cwmsDbConnection.IsNullOrEmpty())
                    {
                        _cwmsDbConnection = "Data Source=localhost;Initial Catalog=Business_CWMS;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _cwmsDbConnection;
            }
            // 默认的数据库连接
            set => _cwmsDbConnection = value;
        }
        /// <summary>
        /// CWMS数据库（连接串，可能是加密的）
        /// </summary>
        public static string CwmsDbConnectionString = string.Empty;
        #endregion

        #region ZBWMS
        /// <summary>
        /// ZBWMS数据库类别
        /// </summary>
        public static CurrentDbType ZbwmsDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// ZBWMS数据库
        /// </summary>
        private static string _zbwmsDbConnection = string.Empty;
        /// <summary>
        /// ZBWMS数据库连接
        /// </summary>
        public static string ZbwmsDbConnection
        {
            get
            {
                if (_zbwmsDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["ZBWMSDbConnection"].IsNullOrEmpty())
                    {
                        _zbwmsDbConnection = ConfigurationManager.AppSettings["ZBWMSDbConnection"];
                    }
                    if (_zbwmsDbConnection.IsNullOrEmpty())
                    {
                        _zbwmsDbConnection = "Data Source=localhost;Initial Catalog=Business_ZBWMS;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _zbwmsDbConnection;
            }
            // 默认的数据库连接
            set => _zbwmsDbConnection = value;
        }
        /// <summary>
        /// ZBWMS数据库（连接串，可能是加密的）
        /// </summary>
        public static string ZbwmsDbConnectionString = string.Empty;
        #endregion

        #region SPWMS
        /// <summary>
        /// SPWMS数据库类别
        /// </summary>
        public static CurrentDbType SpwmsDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// SPWMS数据库
        /// </summary>
        private static string _spwmsDbConnection = string.Empty;
        /// <summary>
        /// SPWMS数据库连接
        /// </summary>
        public static string SpwmsDbConnection
        {
            get
            {
                if (_spwmsDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["SPWMSDbConnection"].IsNullOrEmpty())
                    {
                        _spwmsDbConnection = ConfigurationManager.AppSettings["SPWMSDbConnection"];
                    }
                    if (_spwmsDbConnection.IsNullOrEmpty())
                    {
                        _spwmsDbConnection = "Data Source=localhost;Initial Catalog=Business_SPWMS;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _spwmsDbConnection;
            }
            // 默认的数据库连接
            set => _spwmsDbConnection = value;
        }
        /// <summary>
        /// SPWMS数据库（连接串，可能是加密的）
        /// </summary>
        public static string SpwmsDbConnectionString = string.Empty;
        #endregion

        #region MES
        /// <summary>
        /// MES数据库类别
        /// </summary>
        public static CurrentDbType MesDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// MES数据库
        /// </summary>
        private static string _mesDbConnection = string.Empty;
        /// <summary>
        /// MES数据库连接
        /// </summary>
        public static string MesDbConnection
        {
            get
            {
                if (_mesDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["MESDbConnection"].IsNullOrEmpty())
                    {
                        _mesDbConnection = ConfigurationManager.AppSettings["MESDbConnection"];
                    }
                    if (_mesDbConnection.IsNullOrEmpty())
                    {
                        _mesDbConnection = "Data Source=localhost;Initial Catalog=Business_MES;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _mesDbConnection;
            }
            // 默认的数据库连接
            set => _mesDbConnection = value;
        }
        /// <summary>
        /// MES数据库（连接串，可能是加密的）
        /// </summary>
        public static string MesDbConnectionString = string.Empty;
        #endregion

        #region HRM
        /// <summary>
        /// HRM数据库类别
        /// </summary>
        public static CurrentDbType HrmDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// HRM数据库
        /// </summary>
        private static string _hrmDbConnection = string.Empty;
        /// <summary>
        /// HRM数据库连接
        /// </summary>
        public static string HrmDbConnection
        {
            get
            {
                if (_hrmDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["HRMDbConnection"].IsNullOrEmpty())
                    {
                        _hrmDbConnection = ConfigurationManager.AppSettings["HRMDbConnection"];
                    }
                    if (_hrmDbConnection.IsNullOrEmpty())
                    {
                        _hrmDbConnection = "Data Source=localhost;Initial Catalog=Business_HRM;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _hrmDbConnection;
            }
            // 默认的数据库连接
            set => _hrmDbConnection = value;
        }
        /// <summary>
        /// HRM数据库（连接串，可能是加密的）
        /// </summary>
        public static string HrmDbConnectionString = string.Empty;
        #endregion

        #region CRM
        /// <summary>
        /// CRM数据库类别
        /// </summary>
        public static CurrentDbType CrmDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// CRM数据库
        /// </summary>
        private static string _crmDbConnection = string.Empty;
        /// <summary>
        /// CRM数据库连接
        /// </summary>
        public static string CrmDbConnection
        {
            get
            {
                if (_crmDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["CRMDbConnection"].IsNullOrEmpty())
                    {
                        _crmDbConnection = ConfigurationManager.AppSettings["CRMDbConnection"];
                    }
                    if (_crmDbConnection.IsNullOrEmpty())
                    {
                        _crmDbConnection = "Data Source=localhost;Initial Catalog=Business_CRM;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _crmDbConnection;
            }
            // 默认的数据库连接
            set => _crmDbConnection = value;
        }
        /// <summary>
        /// CRM数据库（连接串，可能是加密的）
        /// </summary>
        public static string CrmDbConnectionString = string.Empty;
        #endregion

        #region OA
        /// <summary>
        /// OA数据库类别
        /// </summary>
        public static CurrentDbType OaDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// OA数据库
        /// </summary>
        private static string _oaDbConnection = string.Empty;
        /// <summary>
        /// OA数据库连接
        /// </summary>
        public static string OaDbConnection
        {
            get
            {
                if (_oaDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["OADbConnection"].IsNullOrEmpty())
                    {
                        _oaDbConnection = ConfigurationManager.AppSettings["OADbConnection"];
                    }
                    if (_oaDbConnection.IsNullOrEmpty())
                    {
                        _oaDbConnection = "Data Source=localhost;Initial Catalog=Business_OA;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _oaDbConnection;
            }
            // 默认的数据库连接
            set => _oaDbConnection = value;
        }
        /// <summary>
        /// OA数据库（连接串，可能是加密的）
        /// </summary>
        public static string OaDbConnectionString = string.Empty;
        #endregion

        #region Label
        /// <summary>
        /// 标签打印数据库类别
        /// </summary>
        public static CurrentDbType LabelDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// 标签数据库
        /// </summary>
        private static string _labelDbConnection = string.Empty;
        /// <summary>
        /// 业务数据库连接
        /// </summary>
        public static string LabelDbConnection
        {
            get
            {
                if (_labelDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["LabelDbConnection"].IsNullOrEmpty())
                    {
                        _labelDbConnection = ConfigurationManager.AppSettings["LabelDbConnection"];
                    }
                    if (_labelDbConnection.IsNullOrEmpty())
                    {
                        _labelDbConnection = "Data Source=localhost;Initial Catalog=Business_Label;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _labelDbConnection;
            }
            // 默认的数据库连接
            set => _labelDbConnection = value;
        }
        /// <summary>
        /// 业务数据库（连接串，可能是加密的）
        /// </summary>
        public static string LabelDbConnectionString = string.Empty;

        #endregion

        #region Web
        /// <summary>
        /// Web数据库类别
        /// </summary>
        public static CurrentDbType WebDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// Web数据库
        /// </summary>
        private static string _webDbConnection = string.Empty;
        /// <summary>
        /// Web数据库连接
        /// </summary>
        public static string WebDbConnection
        {
            get
            {
                if (_webDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["WebDbConnection"].IsNullOrEmpty())
                    {
                        _webDbConnection = ConfigurationManager.AppSettings["WebDbConnection"];
                    }
                    if (_webDbConnection.IsNullOrEmpty())
                    {
                        _webDbConnection = "Data Source=localhost;Initial Catalog=Business_Web;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _webDbConnection;
            }
            // 默认的数据库连接
            set => _webDbConnection = value;
        }
        /// <summary>
        /// Web数据库（连接串，可能是加密的）
        /// </summary>
        public static string WebDbConnectionString = string.Empty;
        #endregion

        #region CMS
        /// <summary>
        /// CMS数据库类别
        /// </summary>
        public static CurrentDbType CmsDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// CMS数据库
        /// </summary>
        private static string _cmsDbConnection = string.Empty;
        /// <summary>
        /// CMS数据库连接
        /// </summary>
        public static string CmsDbConnection
        {
            get
            {
                if (_cmsDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["CMSDbConnection"].IsNullOrEmpty())
                    {
                        _cmsDbConnection = ConfigurationManager.AppSettings["CMSDbConnection"];
                    }
                    if (_cmsDbConnection.IsNullOrEmpty())
                    {
                        _cmsDbConnection = "Data Source=localhost;Initial Catalog=Business_CMS;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _cmsDbConnection;
            }
            // 默认的数据库连接
            set => _cmsDbConnection = value;
        }
        /// <summary>
        /// CMS数据库（连接串，可能是加密的）
        /// </summary>
        public static string CmsDbConnectionString = string.Empty;
        #endregion

        #region DTcms
        /// <summary>
        /// DTcms数据库类别
        /// </summary>
        public static CurrentDbType DTcmsDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// DTcms数据库
        /// </summary>
        private static string _dTcmsDbConnection = string.Empty;
        /// <summary>
        /// DTcms数据库连接
        /// </summary>
        public static string DTcmsDbConnection
        {
            get
            {
                if (_dTcmsDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["DTcmsDbConnection"].IsNullOrEmpty())
                    {
                        _dTcmsDbConnection = ConfigurationManager.AppSettings["DTcmsDbConnection"];
                    }
                    if (_dTcmsDbConnection.IsNullOrEmpty())
                    {
                        _dTcmsDbConnection = "Data Source=localhost;Initial Catalog=DTcms;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _dTcmsDbConnection;
            }
            // 默认的数据库连接
            set => _dTcmsDbConnection = value;
        }
        /// <summary>
        /// DTcms数据库（连接串，可能是加密的）
        /// </summary>
        public static string DTcmsDbConnectionString = string.Empty;
        #endregion

        #region FlowPortal
        /// <summary>
        /// FlowPortal数据库类别
        /// </summary>
        public static CurrentDbType FlowPortalDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// FlowPortal数据库
        /// </summary>
        private static string _flowPortalDbConnection = string.Empty;
        /// <summary>
        /// FlowPortal数据库连接
        /// </summary>
        public static string FlowPortalDbConnection
        {
            get
            {
                if (_flowPortalDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["FlowPortalDbConnection"].IsNullOrEmpty())
                    {
                        _flowPortalDbConnection = ConfigurationManager.AppSettings["FlowPortalDbConnection"];
                    }
                    if (_flowPortalDbConnection.IsNullOrEmpty())
                    {
                        _flowPortalDbConnection = "Data Source=localhost;Initial Catalog=Business_FlowPortal;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _flowPortalDbConnection;
            }
            // 默认的数据库连接
            set => _flowPortalDbConnection = value;
        }
        /// <summary>
        /// FlowPortal数据库（连接串，可能是加密的）
        /// </summary>
        public static string FlowPortalDbConnectionString = string.Empty;
        #endregion

        #region DealerPortal
        /// <summary>
        /// DealerPortal数据库类别
        /// </summary>
        public static CurrentDbType DealerPortalDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// DealerPortal数据库
        /// </summary>
        private static string _dealerPortalDbConnection = string.Empty;
        /// <summary>
        /// DealerPortal数据库连接
        /// </summary>
        public static string DealerPortalDbConnection
        {
            get
            {
                if (_dealerPortalDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["DealerPortalDbConnection"].IsNullOrEmpty())
                    {
                        _dealerPortalDbConnection = ConfigurationManager.AppSettings["DealerPortalDbConnection"];
                    }
                    if (_dealerPortalDbConnection.IsNullOrEmpty())
                    {
                        _dealerPortalDbConnection = "Data Source=localhost;Initial Catalog=Business_DealerPortal;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _dealerPortalDbConnection;
            }
            // 默认的数据库连接
            set => _dealerPortalDbConnection = value;
        }
        /// <summary>
        /// DealerPortal数据库（连接串，可能是加密的）
        /// </summary>
        public static string DealerPortalDbConnectionString = string.Empty;
        #endregion

        #region CustomerPortal
        /// <summary>
        /// CustomerPortal数据库类别
        /// </summary>
        public static CurrentDbType CustomerPortalDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// CustomerPortal数据库
        /// </summary>
        private static string _customerPortalDbConnection = string.Empty;
        /// <summary>
        /// CustomerPortal数据库连接
        /// </summary>
        public static string CustomerPortalDbConnection
        {
            get
            {
                if (_customerPortalDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["CustomerPortalDbConnection"].IsNullOrEmpty())
                    {
                        _customerPortalDbConnection = ConfigurationManager.AppSettings["CustomerPortalDbConnection"];
                    }
                    if (_customerPortalDbConnection.IsNullOrEmpty())
                    {
                        _customerPortalDbConnection = "Data Source=localhost;Initial Catalog=Business_CustomerPortal;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _customerPortalDbConnection;
            }
            // 默认的数据库连接
            set => _customerPortalDbConnection = value;
        }
        /// <summary>
        /// CustomerPortal数据库（连接串，可能是加密的）
        /// </summary>
        public static string CustomerPortalDbConnectionString = string.Empty;
        #endregion

        #region SupplierPortal
        /// <summary>
        /// SupplierPortal数据库类别
        /// </summary>
        public static CurrentDbType SupplierPortalDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// SupplierPortal数据库
        /// </summary>
        private static string _supplierPortalDbConnection = string.Empty;
        /// <summary>
        /// SupplierPortal数据库连接
        /// </summary>
        public static string SupplierPortalDbConnection
        {
            get
            {
                if (_supplierPortalDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["SupplierPortalDbConnection"].IsNullOrEmpty())
                    {
                        _supplierPortalDbConnection = ConfigurationManager.AppSettings["SupplierPortalDbConnection"];
                    }
                    if (_supplierPortalDbConnection.IsNullOrEmpty())
                    {
                        _supplierPortalDbConnection = "Data Source=localhost;Initial Catalog=Business_SupplierPortal;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _supplierPortalDbConnection;
            }
            // 默认的数据库连接
            set => _supplierPortalDbConnection = value;
        }
        /// <summary>
        /// SupplierPortal数据库（连接串，可能是加密的）
        /// </summary>
        public static string SupplierPortalDbConnectionString = string.Empty;
        #endregion

        #region Report
        /// <summary>
        /// Report数据库类别
        /// </summary>
        public static CurrentDbType ReportDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// Report数据库
        /// </summary>
        private static string _reportDbConnection = string.Empty;
        /// <summary>
        /// Report数据库连接
        /// </summary>
        public static string ReportDbConnection
        {
            get
            {
                if (_reportDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["ReportDbConnection"].IsNullOrEmpty())
                    {
                        _reportDbConnection = ConfigurationManager.AppSettings["ReportDbConnection"];
                    }
                    if (_reportDbConnection.IsNullOrEmpty())
                    {
                        _reportDbConnection = "Data Source=localhost;Initial Catalog=Business_Report;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _reportDbConnection;
            }
            // 默认的数据库连接
            set => _reportDbConnection = value;
        }
        /// <summary>
        /// Report数据库（连接串，可能是加密的）
        /// </summary>
        public static string ReportDbConnectionString = string.Empty;
        #endregion

        #region SCM
        /// <summary>
        /// SCM数据库类别
        /// </summary>
        public static CurrentDbType ScmDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// SCM数据库
        /// </summary>
        private static string _scmDbConnection = string.Empty;
        /// <summary>
        /// SCM数据库连接
        /// </summary>
        public static string ScmDbConnection
        {
            get
            {
                if (_scmDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["ScmDbConnection"].IsNullOrEmpty())
                    {
                        _scmDbConnection = ConfigurationManager.AppSettings["ScmDbConnection"];
                    }
                    if (_scmDbConnection.IsNullOrEmpty())
                    {
                        _scmDbConnection = "Data Source=localhost;Initial Catalog=Business_SCM;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _scmDbConnection;
            }
            // 默认的数据库连接
            set => _scmDbConnection = value;
        }
        /// <summary>
        /// SCM数据库（连接串，可能是加密的）
        /// </summary>
        public static string ScmDbConnectionString = string.Empty;
        #endregion

        #region CSCM
        /// <summary>
        /// CSCM数据库类别
        /// </summary>
        public static CurrentDbType CscmDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// CSCM数据库
        /// </summary>
        private static string _cscmDbConnection = string.Empty;
        /// <summary>
        /// CSCM数据库连接
        /// </summary>
        public static string CscmDbConnection
        {
            get
            {
                if (_cscmDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["CscmDbConnection"].IsNullOrEmpty())
                    {
                        _cscmDbConnection = ConfigurationManager.AppSettings["CscmDbConnection"];
                    }
                    if (_cscmDbConnection.IsNullOrEmpty())
                    {
                        _cscmDbConnection = "Data Source=localhost;Initial Catalog=Business_CSCM;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _cscmDbConnection;
            }
            // 默认的数据库连接
            set => _cscmDbConnection = value;
        }
        /// <summary>
        /// CSCM数据库（连接串，可能是加密的）
        /// </summary>
        public static string CscmDbConnectionString = string.Empty;
        #endregion

        #region IMS
        /// <summary>
        /// IMS数据库类别
        /// </summary>
        public static CurrentDbType ImsDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// IMS数据库
        /// </summary>
        private static string _imsDbConnection = string.Empty;
        /// <summary>
        /// IMS数据库连接
        /// </summary>
        public static string ImsDbConnection
        {
            get
            {
                if (_imsDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["ImsDbConnection"].IsNullOrEmpty())
                    {
                        _imsDbConnection = ConfigurationManager.AppSettings["ImsDbConnection"];
                    }
                    if (_imsDbConnection.IsNullOrEmpty())
                    {
                        _imsDbConnection = "Data Source=localhost;Initial Catalog=Business_IMS;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _imsDbConnection;
            }
            // 默认的数据库连接
            set => _imsDbConnection = value;
        }
        /// <summary>
        /// Ims数据库（连接串，可能是加密的）
        /// </summary>
        public static string ImsDbConnectionString = string.Empty;
        #endregion

        #region ICS
        /// <summary>
        /// ICS数据库类别
        /// </summary>
        public static CurrentDbType IcsDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// ICS数据库
        /// </summary>
        private static string _icsDbConnection = string.Empty;
        /// <summary>
        /// ICS数据库连接
        /// </summary>
        public static string IcsDbConnection
        {
            get
            {
                if (_icsDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["IcsDbConnection"].IsNullOrEmpty())
                    {
                        _icsDbConnection = ConfigurationManager.AppSettings["IcsDbConnection"];
                    }
                    if (_icsDbConnection.IsNullOrEmpty())
                    {
                        _icsDbConnection = "Data Source=localhost;Initial Catalog=Business_ICS;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _icsDbConnection;
            }
            // 默认的数据库连接
            set => _icsDbConnection = value;
        }
        /// <summary>
        /// Ics数据库（连接串，可能是加密的）
        /// </summary>
        public static string IcsDbConnectionString = string.Empty;
        #endregion

        #region OMS
        /// <summary>
        /// OMS数据库类别
        /// </summary>
        public static CurrentDbType OmsDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// OMS数据库
        /// </summary>
        private static string _omsDbConnection = string.Empty;
        /// <summary>
        /// OMS数据库连接
        /// </summary>
        public static string OmsDbConnection
        {
            get
            {
                if (_omsDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["OMSDbConnection"].IsNullOrEmpty())
                    {
                        _omsDbConnection = ConfigurationManager.AppSettings["OMSDbConnection"];
                    }
                    if (_omsDbConnection.IsNullOrEmpty())
                    {
                        _omsDbConnection = "Data Source=localhost;Initial Catalog=Business_OMS;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _omsDbConnection;
            }
            // 默认的数据库连接
            set => _omsDbConnection = value;
        }
        /// <summary>
        /// Oms数据库（连接串，可能是加密的）
        /// </summary>
        public static string OmsDbConnectionString = string.Empty;
        #endregion

        #region Member
        /// <summary>
        /// Member数据库类别
        /// </summary>
        public static CurrentDbType MemberDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// Member数据库
        /// </summary>
        private static string _memberDbConnection = string.Empty;
        /// <summary>
        /// Member数据库连接
        /// </summary>
        public static string MemberDbConnection
        {
            get
            {
                if (_memberDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["MemberDbConnection"].IsNullOrEmpty())
                    {
                        _memberDbConnection = ConfigurationManager.AppSettings["MemberDbConnection"];
                    }
                    if (_memberDbConnection.IsNullOrEmpty())
                    {
                        _memberDbConnection = "Data Source=localhost;Initial Catalog=Business_Member;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _memberDbConnection;
            }
            // 默认的数据库连接
            set => _memberDbConnection = value;
        }
        /// <summary>
        /// Member数据库（连接串，可能是加密的）
        /// </summary>
        public static string MemberDbConnectionString = string.Empty;
        #endregion

        #region Budget
        /// <summary>
        /// Budget数据库类别
        /// </summary>
        public static CurrentDbType BudgetDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// Budget数据库
        /// </summary>
        private static string _budgetDbConnection = string.Empty;
        /// <summary>
        /// Budget数据库连接
        /// </summary>
        public static string BudgetDbConnection
        {
            get
            {
                if (_budgetDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["BudgetDbConnection"].IsNullOrEmpty())
                    {
                        _budgetDbConnection = ConfigurationManager.AppSettings["BudgetDbConnection"];
                    }
                    if (_budgetDbConnection.IsNullOrEmpty())
                    {
                        _budgetDbConnection = "Data Source=localhost;Initial Catalog=Business_Budget;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _budgetDbConnection;
            }
            // 默认的数据库连接
            set => _budgetDbConnection = value;
        }
        /// <summary>
        /// Budget数据库（连接串，可能是加密的）
        /// </summary>
        public static string BudgetDbConnectionString = string.Empty;
        #endregion

        #region ITAMS
        /// <summary>
        /// ITAMS数据库类别
        /// </summary>
        public static CurrentDbType ItamsDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// ITAMS数据库
        /// </summary>
        private static string _itamsDbConnection = string.Empty;
        /// <summary>
        /// ITAMS数据库连接
        /// </summary>
        public static string ItamsDbConnection
        {
            get
            {
                if (_itamsDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["ItamsDbConnection"].IsNullOrEmpty())
                    {
                        _itamsDbConnection = ConfigurationManager.AppSettings["ItamsDbConnection"];
                    }
                    if (_itamsDbConnection.IsNullOrEmpty())
                    {
                        _itamsDbConnection = "Data Source=localhost;Initial Catalog=Business_ITAMS;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _itamsDbConnection;
            }
            // 默认的数据库连接
            set => _itamsDbConnection = value;
        }
        /// <summary>
        /// ITAMS数据库（连接串，可能是加密的）
        /// </summary>
        public static string ItamsDbConnectionString = string.Empty;
        #endregion

        #region CardTicket
        /// <summary>
        /// CardTicket数据库类别
        /// </summary>
        public static CurrentDbType CardTicketDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// CardTicket数据库
        /// </summary>
        private static string _cardTicketDbConnection = string.Empty;
        /// <summary>
        /// CardTicket数据库连接
        /// </summary>
        public static string CardTicketDbConnection
        {
            get
            {
                if (_cardTicketDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["CardTicketDbConnection"].IsNullOrEmpty())
                    {
                        _cardTicketDbConnection = ConfigurationManager.AppSettings["CardTicketDbConnection"];
                    }
                    if (_cardTicketDbConnection.IsNullOrEmpty())
                    {
                        _cardTicketDbConnection = "Data Source=localhost;Initial Catalog=Business_CardTicket;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _cardTicketDbConnection;
            }
            // 默认的数据库连接
            set => _cardTicketDbConnection = value;
        }
        /// <summary>
        /// CardTicket数据库（连接串，可能是加密的）
        /// </summary>
        public static string CardTicketDbConnectionString = string.Empty;
        #endregion

        #region MDM
        /// <summary>
        /// MDM数据库类别
        /// </summary>
        public static CurrentDbType MdmDbType = CurrentDbType.SqlServer;
        /// <summary>
        /// MDM数据库
        /// </summary>
        private static string _mdmDbConnection = string.Empty;
        /// <summary>
        /// MDM数据库连接
        /// </summary>
        public static string MdmDbConnection
        {
            get
            {
                if (_mdmDbConnection.IsNullOrEmpty())
                {
                    if (!ConfigurationManager.AppSettings["MDMDbConnection"].IsNullOrEmpty())
                    {
                        _mdmDbConnection = ConfigurationManager.AppSettings["MDMDbConnection"];
                    }
                    if (_mdmDbConnection.IsNullOrEmpty())
                    {
                        _mdmDbConnection = "Data Source=localhost;Initial Catalog=Business_MDM;Integrated Security=SSPI;";
                    }
                }
                // 默认的数据库连接
                return _mdmDbConnection;
            }
            // 默认的数据库连接
            set => _mdmDbConnection = value;
        }
        /// <summary>
        /// MDM数据库（连接串，可能是加密的）
        /// </summary>
        public static string MdmDbConnectionString = string.Empty;
        #endregion

        #region 杂项

        /// <summary>
        /// 数据库表版本(默认为5版本)
        /// </summary>
        public static int DatabaseTableVersion = 5;

        #endregion
    }
}