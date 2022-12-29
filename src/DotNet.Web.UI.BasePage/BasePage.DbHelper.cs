//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

using DotNet.Util;

/// <remarks>
/// BasePage
/// 基础网页类
/// 
/// 修改记录
/// 版本：4.2 2021.08.28    Troy Cui    完善其它所支持的子系统数据库。
/// 版本：4.1 2017.05.09    Troy Cui    完善代码。
///	版本：1.0 2012.11.10    JiRiGaLa    整理代码。
///	
/// 版本：4.2
/// <author>  
///		<name>Troy.Cui</name>
///		<date>2017.05.09</date>
/// </author> 
/// </remarks>
public partial class BasePage : System.Web.UI.Page
{
    #region 默认

    /// <summary>
    /// 默认数据库部分（业务）
    /// </summary>
    private IDbHelper _dbHelper = null;

    /// <summary>
    /// 默认数据库部分（业务）
    /// </summary>
    protected IDbHelper DbHelper
    {
        get
        {
            if (_dbHelper == null)
            {
                // 当前数据库连接对象，默认为业务数据库
                _dbHelper = DbHelperFactory.Create(BaseSystemInfo.BusinessDbType, BaseSystemInfo.BusinessDbConnection);
            }
            return _dbHelper;
        }
    }

    #endregion

    #region UserCenter

    /// <summary>
    /// 用户中心数据库部分
    /// </summary>
    private IDbHelper _userCenterDbHelper = null;

    /// <summary>
    /// 用户中心数据库部分
    /// </summary>
    protected IDbHelper UserCenterDbHelper
    {
        get
        {
            if (_userCenterDbHelper == null)
            {
                // 获取数据库连接对象
                _userCenterDbHelper = DbHelperFactory.Create(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            return _userCenterDbHelper;
        }
    }

    #endregion

    #region Message

    /// <summary>
    /// 消息数据库部分
    /// </summary>
    private IDbHelper _messageDbHelper = null;

    /// <summary>
    /// 消息数据库部分
    /// </summary>
    protected IDbHelper MessageDbHelper
    {
        get
        {
            if (_messageDbHelper == null)
            {
                // 获取数据库连接对象
                _messageDbHelper = DbHelperFactory.Create(BaseSystemInfo.MessageDbType, BaseSystemInfo.MessageDbConnection);
            }
            return _messageDbHelper;
        }
    }

    #endregion

    #region Business

    /// <summary>
    /// 业务系统数据库部分
    /// </summary>
    private IDbHelper _businessDbHelper = null;

    /// <summary>
    /// 业务系统数据库部分
    /// </summary>
    protected IDbHelper BusinessDbHelper
    {
        get
        {
            if (_businessDbHelper == null)
            {
                // 获取数据库连接对象
                _businessDbHelper = DbHelperFactory.Create(BaseSystemInfo.BusinessDbType, BaseSystemInfo.BusinessDbConnection);
            }
            return _businessDbHelper;
        }
    }

    #endregion

    #region WorkFlow

    /// <summary>
    /// WorkFlow数据库部分
    /// </summary>
    private IDbHelper _workFlowDbHelper = null;

    /// <summary>
    /// WorkFlow数据库部分
    /// </summary>
    protected IDbHelper WorkFlowDbHelper
    {
        get
        {
            if (_workFlowDbHelper == null)
            {
                // 获取数据库连接对象
                _workFlowDbHelper = DbHelperFactory.Create(BaseSystemInfo.WorkFlowDbType, BaseSystemInfo.WorkFlowDbConnection);
            }
            return _workFlowDbHelper;
        }
    }

    #endregion

    #region WebApp

    /// <summary>
    /// WebApp数据库部分
    /// </summary>
    private IDbHelper _webAppDbHelper = null;

    /// <summary>
    /// WebApp数据库部分
    /// </summary>
    protected IDbHelper WebAppDbHelper
    {
        get
        {
            if (_webAppDbHelper == null)
            {
                // 获取数据库连接对象
                _webAppDbHelper = DbHelperFactory.Create(BaseSystemInfo.WebAppDbType, BaseSystemInfo.WebAppDbConnection);
            }
            return _webAppDbHelper;
        }
    }

    #endregion

    #region BPM

    /// <summary>
    /// BPM数据库部分
    /// </summary>
    private IDbHelper _bpmDbHelper = null;

    /// <summary>
    /// BPM数据库部分
    /// </summary>
    protected IDbHelper BpmDbHelper
    {
        get
        {
            if (_bpmDbHelper == null)
            {
                // 获取数据库连接对象
                _bpmDbHelper = DbHelperFactory.Create(BaseSystemInfo.BpmDbType, BaseSystemInfo.BpmDbConnection);
            }
            return _bpmDbHelper;
        }
    }

    #endregion

    #region ERP

    /// <summary>
    /// ERP数据库部分
    /// </summary>
    private IDbHelper _erpDbHelper = null;

    /// <summary>
    /// ERP数据库部分
    /// </summary>
    protected IDbHelper ErpDbHelper
    {
        get
        {
            if (_erpDbHelper == null)
            {
                // 获取数据库连接对象
                _erpDbHelper = DbHelperFactory.Create(BaseSystemInfo.ErpDbType, BaseSystemInfo.ErpDbConnection);
            }
            return _erpDbHelper;
        }
    }

    #endregion

    #region WMS

    /// <summary>
    /// WMS数据库部分
    /// </summary>
    private IDbHelper _wmsDbHelper = null;

    /// <summary>
    /// WMS数据库部分
    /// </summary>
    protected IDbHelper WmsDbHelper
    {
        get
        {
            if (_wmsDbHelper == null)
            {
                // 获取数据库连接对象
                _wmsDbHelper = DbHelperFactory.Create(BaseSystemInfo.WmsDbType, BaseSystemInfo.WmsDbConnection);
            }
            return _wmsDbHelper;
        }
    }

    #endregion

    #region ZBWMS

    /// <summary>
    /// WMS数据库部分
    /// </summary>
    private IDbHelper _zbwmsDbHelper = null;

    /// <summary>
    /// WMS数据库部分
    /// </summary>
    protected IDbHelper ZbwWmsDbHelper
    {
        get
        {
            if (_zbwmsDbHelper == null)
            {
                // 获取数据库连接对象
                _zbwmsDbHelper = DbHelperFactory.Create(BaseSystemInfo.ZbwmsDbType, BaseSystemInfo.ZbwmsDbConnection);
            }
            return _zbwmsDbHelper;
        }
    }

    #endregion

    #region MES

    /// <summary>
    /// MES数据库部分
    /// </summary>
    private IDbHelper _mesDbHelper = null;

    /// <summary>
    /// MES数据库部分
    /// </summary>
    protected IDbHelper MesDbHelper
    {
        get
        {
            if (_mesDbHelper == null)
            {
                // 获取数据库连接对象
                _mesDbHelper = DbHelperFactory.Create(BaseSystemInfo.MesDbType, BaseSystemInfo.MesDbConnection);
            }
            return _mesDbHelper;
        }
    }

    #endregion

    #region HRM

    /// <summary>
    /// HRM数据库部分
    /// </summary>
    private IDbHelper _hrmDbHelper = null;

    /// <summary>
    /// HRM数据库部分
    /// </summary>
    protected IDbHelper HrmDbHelper
    {
        get
        {
            if (_hrmDbHelper == null)
            {
                // 获取数据库连接对象
                _hrmDbHelper = DbHelperFactory.Create(BaseSystemInfo.HrmDbType, BaseSystemInfo.HrmDbConnection);
            }
            return _hrmDbHelper;
        }
    }

    #endregion

    #region CRM

    /// <summary>
    /// CRM数据库部分
    /// </summary>
    private IDbHelper _crmDbHelper = null;

    /// <summary>
    /// CRM数据库部分
    /// </summary>
    protected IDbHelper CrmDbHelper
    {
        get
        {
            if (_crmDbHelper == null)
            {
                // 获取数据库连接对象
                _crmDbHelper = DbHelperFactory.Create(BaseSystemInfo.CrmDbType, BaseSystemInfo.CrmDbConnection);
            }
            return _crmDbHelper;
        }
    }

    #endregion

    #region OA

    /// <summary>
    /// OA数据库部分
    /// </summary>
    private IDbHelper _oaDbHelper = null;

    /// <summary>
    /// OA数据库部分
    /// </summary>
    protected IDbHelper OaDbHelper
    {
        get
        {
            if (_oaDbHelper == null)
            {
                // 获取数据库连接对象
                _oaDbHelper = DbHelperFactory.Create(BaseSystemInfo.OaDbType, BaseSystemInfo.OaDbConnection);
            }
            return _oaDbHelper;
        }
    }

    #endregion

    #region Label

    /// <summary>
    /// Label数据库部分
    /// </summary>
    private IDbHelper _labelDbHelper = null;

    /// <summary>
    /// Label数据库部分
    /// </summary>
    protected IDbHelper LabelDbHelper
    {
        get
        {
            if (_labelDbHelper == null)
            {
                // 获取数据库连接对象
                _labelDbHelper = DbHelperFactory.Create(BaseSystemInfo.LabelDbType, BaseSystemInfo.LabelDbConnection);
            }
            return _labelDbHelper;
        }
    }

    #endregion

    #region WebSite

    /// <summary>
    /// WebSite数据库部分
    /// </summary>
    private IDbHelper _webSiteDbHelper = null;

    /// <summary>
    /// WebSite数据库部分
    /// </summary>
    protected IDbHelper WebSiteDbHelper
    {
        get
        {
            if (_webSiteDbHelper == null)
            {
                // 获取数据库连接对象
                _webSiteDbHelper = DbHelperFactory.Create(BaseSystemInfo.WebDbType, BaseSystemInfo.WebDbConnection);
            }
            return _webSiteDbHelper;
        }
    }

    #endregion

    #region CMS

    /// <summary>
    /// CMS数据库部分
    /// </summary>
    private IDbHelper _cmsDbHelper = null;

    /// <summary>
    /// CMS数据库部分
    /// </summary>
    protected IDbHelper CmsDbHelper
    {
        get
        {
            if (_cmsDbHelper == null)
            {
                // 获取数据库连接对象
                _cmsDbHelper = DbHelperFactory.Create(BaseSystemInfo.CmsDbType, BaseSystemInfo.CmsDbConnection);
            }
            return _cmsDbHelper;
        }
    }

    #endregion

    #region FlowPortal

    /// <summary>
    /// FlowPortal数据库部分
    /// </summary>
    private IDbHelper _flowPortalDbHelper = null;

    /// <summary>
    /// FlowPortal数据库部分
    /// </summary>
    protected IDbHelper FlowPortalDbHelper
    {
        get
        {
            if (_flowPortalDbHelper == null)
            {
                // 获取数据库连接对象
                _flowPortalDbHelper = DbHelperFactory.Create(BaseSystemInfo.FlowPortalDbType, BaseSystemInfo.FlowPortalDbConnection);
            }
            return _flowPortalDbHelper;
        }
    }

    #endregion

    #region DealerPortal

    /// <summary>
    /// DealerPortal数据库部分
    /// </summary>
    private IDbHelper _dealerPortalDbHelper = null;

    /// <summary>
    /// DealerPortal数据库部分
    /// </summary>
    protected IDbHelper DealerPortalDbHelper
    {
        get
        {
            if (_dealerPortalDbHelper == null)
            {
                // 获取数据库连接对象
                _dealerPortalDbHelper = DbHelperFactory.Create(BaseSystemInfo.DealerPortalDbType, BaseSystemInfo.DealerPortalDbConnection);
            }
            return _dealerPortalDbHelper;
        }
    }

    #endregion

    #region CustomerPortal

    /// <summary>
    /// CustomerPortal数据库部分
    /// </summary>
    private IDbHelper _customerPortalDbHelper = null;

    /// <summary>
    /// CustomerPortal数据库部分
    /// </summary>
    protected IDbHelper CustomerPortalDbHelper
    {
        get
        {
            if (_customerPortalDbHelper == null)
            {
                // 获取数据库连接对象
                _customerPortalDbHelper = DbHelperFactory.Create(BaseSystemInfo.CustomerPortalDbType, BaseSystemInfo.CustomerPortalDbConnection);
            }
            return _customerPortalDbHelper;
        }
    }

    #endregion

    #region SupplierPortal

    /// <summary>
    /// SupplierPortal数据库部分
    /// </summary>
    private IDbHelper _supplierPortalDbHelper = null;

    /// <summary>
    /// SupplierPortal数据库部分
    /// </summary>
    protected IDbHelper SupplierPortalDbHelper
    {
        get
        {
            if (_supplierPortalDbHelper == null)
            {
                // 获取数据库连接对象
                _supplierPortalDbHelper = DbHelperFactory.Create(BaseSystemInfo.SupplierPortalDbType, BaseSystemInfo.SupplierPortalDbConnection);
            }
            return _supplierPortalDbHelper;
        }
    }

    #endregion

    #region Report

    /// <summary>
    /// Report数据库部分
    /// </summary>
    private IDbHelper _reportDbHelper = null;

    /// <summary>
    /// Report数据库部分
    /// </summary>
    protected IDbHelper ReportDbHelper
    {
        get
        {
            if (_reportDbHelper == null)
            {
                // 获取数据库连接对象
                _reportDbHelper = DbHelperFactory.Create(BaseSystemInfo.ReportDbType, BaseSystemInfo.ReportDbConnection);
            }
            return _reportDbHelper;
        }
    }

    #endregion

    #region SCM

    /// <summary>
    /// SCM数据库部分
    /// </summary>
    private IDbHelper _scmDbHelper = null;

    /// <summary>
    /// SCM数据库部分
    /// </summary>
    protected IDbHelper ScmDbHelper
    {
        get
        {
            if (_scmDbHelper == null)
            {
                // 获取数据库连接对象
                _scmDbHelper = DbHelperFactory.Create(BaseSystemInfo.ScmDbType, BaseSystemInfo.ScmDbConnection);
            }
            return _scmDbHelper;
        }
    }

    #endregion

    #region IMS

    /// <summary>
    /// IMS数据库部分
    /// </summary>
    private IDbHelper _imsDbHelper = null;

    /// <summary>
    /// IMS数据库部分
    /// </summary>
    protected IDbHelper ImsDbHelper
    {
        get
        {
            if (_imsDbHelper == null)
            {
                // 获取数据库连接对象
                _imsDbHelper = DbHelperFactory.Create(BaseSystemInfo.ImsDbType, BaseSystemInfo.ImsDbConnection);
            }
            return _imsDbHelper;
        }
    }

    #endregion

    #region OMS

    /// <summary>
    /// OMS数据库部分
    /// </summary>
    private IDbHelper _omsDbHelper = null;

    /// <summary>
    /// OMS数据库部分
    /// </summary>
    protected IDbHelper OmsDbHelper
    {
        get
        {
            if (_omsDbHelper == null)
            {
                // 获取数据库连接对象
                _omsDbHelper = DbHelperFactory.Create(BaseSystemInfo.OmsDbType, BaseSystemInfo.OmsDbConnection);
            }
            return _omsDbHelper;
        }
    }

    #endregion

    #region Member

    /// <summary>
    /// Member数据库部分
    /// </summary>
    private IDbHelper _memberDbHelper = null;

    /// <summary>
    /// Member数据库部分
    /// </summary>
    protected IDbHelper MemberDbHelper
    {
        get
        {
            if (_memberDbHelper == null)
            {
                // 获取数据库连接对象
                _memberDbHelper = DbHelperFactory.Create(BaseSystemInfo.MemberDbType, BaseSystemInfo.MemberDbConnection);
            }
            return _memberDbHelper;
        }
    }

    #endregion

    #region Budget

    /// <summary>
    /// Budget数据库部分
    /// </summary>
    private IDbHelper _budgetDbHelper = null;

    /// <summary>
    /// Budget数据库部分
    /// </summary>
    protected IDbHelper BudgetDbHelper
    {
        get
        {
            if (_budgetDbHelper == null)
            {
                // 获取数据库连接对象
                _budgetDbHelper = DbHelperFactory.Create(BaseSystemInfo.BudgetDbType, BaseSystemInfo.BudgetDbConnection);
            }
            return _budgetDbHelper;
        }
    }

    #endregion

    #region ITAMS

    /// <summary>
    /// ITAMS数据库部分
    /// </summary>
    private IDbHelper _itamsDbHelper = null;

    /// <summary>
    /// ITAMS数据库部分
    /// </summary>
    protected IDbHelper ItamsDbHelper
    {
        get
        {
            if (_itamsDbHelper == null)
            {
                // 获取数据库连接对象
                _itamsDbHelper = DbHelperFactory.Create(BaseSystemInfo.ItamsDbType, BaseSystemInfo.ItamsDbConnection);
            }
            return _itamsDbHelper;
        }
    }

    #endregion

    #region CardTicket

    /// <summary>
    /// Budget数据库部分
    /// </summary>
    private IDbHelper _cardTicketDbHelper = null;

    /// <summary>
    /// Budget数据库部分
    /// </summary>
    protected IDbHelper CardTicketDbHelper
    {
        get
        {
            if (_cardTicketDbHelper == null)
            {
                // 获取数据库连接对象
                _cardTicketDbHelper = DbHelperFactory.Create(BaseSystemInfo.CardTicketDbType, BaseSystemInfo.CardTicketDbConnection);
            }
            return _cardTicketDbHelper;
        }
    }

    #endregion

}