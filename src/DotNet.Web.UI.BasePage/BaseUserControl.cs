//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using DotNet.Model;
using DotNet.Business;
using DotNet.Util;

/// <remarks>
/// BaseUserControl
/// 基础网页类
/// 
/// 修改记录
/// 版本：2.6 2011.06.19    zgl         修改dbHelper，_userCenterDbHelper的属性为protected->private
///                                     增加protected  string  GetSequence(string tableName) 根据表名，取得序列号
///	版本：2.5 2009.11.09    JiRiGaLa    public void Authorized(string permissionCode) 函数进行改进。
///	版本：2.4 2008.03.17    JiRiGaLa    登录程序改进为面向服务的登录。
///	版本：2.3 2008.03.07    JiRiGaLa    登录时页面重新导向功能改进。
///	版本：2.2 2007.12.09    JiRiGaLa    获得页面权限的 GetPermission 函数改进。
///	版本：2.1 2007.12.08    JiRiGaLa    单点登录功能完善。
///	版本：2.0 2006.02.02    JiRiGaLa    页面注释都修改好。
///	
/// 版本：2.5
/// <author>
///		<name>Troy.Cui</name>
///		<date>2009.11.09</date>
/// </author> 
/// </remarks>
public partial class BaseUserControl : UserControl
{
    /// <summary>
    /// 每页显示多少条记录
    /// </summary>
    protected int PageSize = 30;

    /// <summary>
    /// 图片地址
    /// </summary>
    protected string ImagesUrl = ConfigurationManager.AppSettings["ImagesURL"];

    /// <summary>
    /// 用户锁
    /// </summary>
    public static readonly object UserLock = new object();

    /// <summary>
    /// 业务数据库部分
    /// </summary>
    private IDbHelper _dbHelper = null;

    /// <summary>
    /// 业务数据库部分
    /// </summary>
    protected IDbHelper DbHelper
    {
        get
        {
            if (_dbHelper == null)
            {
                // 当前数据库连接对象
                _dbHelper = DbHelperFactory.Create(BaseSystemInfo.BusinessDbType, BaseSystemInfo.BusinessDbConnection);
            }
            return _dbHelper;
        }
    }

    /// <summary>
    /// 工作流数据库部分
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
                // 当前数据库连接对象
                _userCenterDbHelper = DbHelperFactory.Create(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            return _userCenterDbHelper;
        }
    }

    /// <summary>
    /// 工作流数据库部分
    /// </summary>
    private IDbHelper _workFlowDbHelper = null;

    /// <summary>
    /// 工作流数据库部分
    /// </summary>
    protected IDbHelper WorkFlowDbHelper
    {
        get
        {
            if (_workFlowDbHelper == null)
            {
                // 当前数据库连接对象
                _workFlowDbHelper = DbHelperFactory.Create(BaseSystemInfo.WorkFlowDbType, BaseSystemInfo.WorkFlowDbConnection);
            }
            return _workFlowDbHelper;
        }
    }

    /// <summary>
    /// 用户信息
    /// </summary>
    private BaseUserInfo _userInfo = null;
    /// <summary>
    /// 当前操作员信息对象
    /// </summary>
    public BaseUserInfo UserInfo
    {
        get
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["UserInfo"] != null)
            {
                _userInfo = (BaseUserInfo)HttpContext.Current.Session["UserInfo"];
            }

            if (_userInfo == null)
            {
                // 从 Session 读取 当前操作员信息
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["UserInfo"] == null)
                {
                    _userInfo = new BaseUserInfo();
                    // 获得IP 地址
                    _userInfo.RealName = Context.Request.ServerVariables["REMOTE_ADDR"];
                    _userInfo.UserName = Context.Request.ServerVariables["REMOTE_ADDR"];
                    _userInfo.IpAddress = Context.Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            return _userInfo;
        }
        set => _userInfo = value;
    }

    /// <summary>
    /// 单点登录唯一识别标识
    /// </summary>
    public string OpenId = string.Empty;

    /// <summary>
    /// 时间戳，防止网页缓存的功能
    /// </summary>
    public string Ticks = DateTime.Now.Ticks.ToString();

    #region protected override void OnInit(EventArgs e) 所有页面加载时默认运行的方法
    /// <summary>
    /// 所有页面加载时默认运行的方法
    /// </summary>
    /// <param name="e">系统默认参数</param>
    protected override void OnInit(EventArgs e)
    {
        // 当要用到主题皮肤切换时必须加上
        base.OnInit(e);
        GetParameterOpenId();
        // 统一的错误处理页面部分
        // Error += new EventHandler(BasePage_Error);
        Page_Load();
    }
    #endregion

    #region public void GetParameterOpenId() 所有页面基础类的，活得单点登录唯一识别标识的方法
    /// <summary>
    ///所有页面基础类的，活得单点登录唯一识别标识的方法
    /// </summary>
    public void GetParameterOpenId()
    {
        if (Page.Request["OpenId"] != null)
        {
            // 读取参数
            OpenId = Page.Request["OpenId"];
            // 若没登录或者登录的标识不一直，需要重新登录
            if (!WebUtil.UserIsLogon() || !UserInfo.OpenId.Equals(OpenId))
            {
                UserInfo = WebUtil.LogonByOpenId(OpenId).UserInfo;
            }
        }
    }
    #endregion

    #region protected void Page_Load() 页面初次加载
    /// <summary>
    /// 页面初次加载
    /// </summary>
    protected void Page_Load()
    {
        var jsurl = "http://" + Request.Url.Authority + Request.ApplicationPath + "/js/jquery-1.10.2.min.js";
        Page.ClientScript.RegisterClientScriptInclude("jquery", jsurl);
        //jsurl = "http://" + Request.Url.Authority + Request.ApplicationPath + "/js/jfloat.js";
        //Page.ClientScript.RegisterClientScriptInclude("jfloat", jsurl);
        if (!Page.ClientScript.IsClientScriptBlockRegistered("chost"))
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "", "var chost='" + "http://" + Request.Url.Authority + Request.ApplicationPath + "';", true);
    }
    #endregion

    #region protected void GetConfig() 读取一些基本配置信息
    /// <summary>
    /// 读取一些基本配置信息
    /// </summary>
    protected void GetConfig()
    {
    }
    #endregion

    #region public void LogException(Exception exception) 记录异常信息
    /// <summary>
    /// 记录异常信息
    /// </summary>
    /// <param name="exception">异常信息实体</param>
    public void LogException(Exception exception)
    {
        BaseExceptionManager.LogException(UserCenterDbHelper, UserInfo, exception);
    }
    #endregion

    #region public string SortExpression 排序字段
    /// <summary>
    /// 排序字段
    /// </summary>
    public string SortExpression
    {
        get
        {
            if (ViewState["sortExpression"] == null)
            {
                ViewState["sortExpression"] = BaseUtil.FieldCreateTime;
            }
            return ViewState["sortExpression"].ToString();
        }
        set => ViewState["sortExpression"] = value;
    }
    #endregion

    #region public string SortDirection 排序顺序
    /// <summary>
    /// 排序顺序
    /// </summary>
    public string SortDirection
    {
        get
        {
            if (ViewState["sortDirection"] == null)
            {
                ViewState["sortDirection"] = " DESC ";
            }
            return ViewState["sortDirection"].ToString();
        }
        set => ViewState["sortDirection"] = value;
    }
    #endregion
}