//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System;
using System.Configuration;
using System.Web;
using System.Web.UI;
using DotNet.Model;
using DotNet.Business;
using DotNet.Util;

/// <remarks>
/// BasePage
/// 基础网页类
/// 
/// 修改记录
/// 
/// 版本：4.1 2017.05.09    Troy Cui    完善代码。
/// 版本：2.6 2011.06.19    zgl         修改dbHelper，userCenterDbHelper的属性为protected->private
///                                     增加protected  string  GetSequence(string tableName) 根据表名，取得序列号
///	版本：2.5 2009.11.09    JiRiGaLa    public void Authorized(string permissionItemCode) 函数进行改进。
///	版本：2.4 2008.03.17    JiRiGaLa    登录程序改进为面向服务的登录。
///	版本：2.3 2008.03.07    JiRiGaLa    登录时页面重新导向功能改进。
///	版本：2.2 2007.12.09    JiRiGaLa    获得页面权限的 GetPermission 函数改进。
///	版本：2.1 2007.12.08    JiRiGaLa    单点登录功能完善。
///	版本：2.0 2006.02.02    JiRiGaLa    页面注释都修改好。
///	
/// 版本：4.1
/// <author>  
///		<name>Troy.Cui</name>
///		<date>2017.05.09</date>
/// </author> 
/// </remarks>
public partial class BasePage : System.Web.UI.Page
{
    /// <summary>
    /// 是否数据库报错
    /// </summary>
    protected bool DatabaseError = false;
    /// <summary>
    /// 每页显示多少条记录
    /// </summary>
    protected int PageSize = 30;

    /// <summary>
    /// 图片地址
    /// </summary>
    protected string ImagesUrl = ConfigurationManager.AppSettings["ImagesURL"];

    /// <summary>
    /// 用户信息
    /// </summary>
    private CurrentUserInfo _userInfo = null;
    /// <summary>
    /// 当前操作员信息对象
    /// </summary>
    public CurrentUserInfo UserInfo
    {
        get
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["UserInfo"] != null)
            {
                _userInfo = ((CurrentUserInfo)Session["UserInfo"]);
            }
            if (_userInfo == null)
            {
                // 从 Session 读取 当前操作员信息
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["UserInfo"] == null)
                {
                    _userInfo = new CurrentUserInfo
                    {
                        RealName = Context.Request.ServerVariables["REMOTE_ADDR"],
                        UserName = Context.Request.ServerVariables["REMOTE_ADDR"],
                        IpAddress = Context.Request.ServerVariables["REMOTE_ADDR"],
                        ServiceUserName = BaseSystemInfo.ServiceUserName,
                        ServicePassword = BaseSystemInfo.ServicePassword
                    };
                    // 获得IP 地址
                    // 设置操作员类型，防止出现错误，因为不小心变成系统管理员就不好了
                    // if (userInfo.RoleId.Length == 0)
                    //{
                    //    userInfo.RoleId = DefaultRole.User.ToString();
                    //}
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
        // 控制单点登录部分
        GetParameterOpenId();
        // 统一的错误处理页面部分
        // Error += new EventHandler(BasePage_Error);
        // 这里可以加是否有当前页面的访问权限的？
        //if (!UserInfo.IsAdministrator)
        //{
        var url = HttpContext.Current.Request.Url.AbsolutePath.Remove(0, 1);
        // DotNet.WebForm/Default.aspx
        // string applicationPath = HttpContext.Current.Request.ApplicationPath;
        // ApplicationPath	"/DotNet.WebForm"	string
        // 这个是在调试环境里的优化功能
        if (!string.IsNullOrEmpty(url))
        {
            //#if Debug
            //    url = url.Replace("DotNet.WebForm/", "");
            // #endif

            url = url.ToLower();
            if (!(url == "leftmenu.aspx"
                || url == "leftsubmenu.aspx"
                || url == "main.aspx"
                || url == "commonheader.aspx"
                || url == "foot.aspx"
                || url == "default.aspx"
                || url == "logout.aspx"
                || url == "logouting.aspx"
                || url == "isauthorized.aspx"
                || url == "ismoduleauthorized.aspx"
                || url == "isurlauthorized.aspx"
                || url == "isusersignin.aspx"))
            {
                if (BaseSystemInfo.CheckPasswordStrength)
                {
                    // 这里可以加判断是否登录的？
                    // WebUtil.CheckIsLogon();
                }
                // 这里是判断每个Url的权限
                // UrlAuthorized(url);
            }
        }
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
            // 看看是远程的还是本地的登录方式
            if (ConfigurationManager.AppSettings["SSOVerify"] != null)
            {
                // 通过远程方式进行登录                
                // string url = "http://localhost/GetSignin.ashx?OpenId=" + UserInfo.OpenId;
                var url = ConfigurationManager.AppSettings["SSOVerify"] + "?OpenId=" + UserInfo.OpenId;
                var jsonUserInfo = RequestUtil.GetResponse(url);
                if (!string.IsNullOrEmpty(jsonUserInfo))
                {
                    var userInfoJ = JsonUtil.JsonToObject<BaseUserInfo>(jsonUserInfo);
                    Page.Response.Write(userInfoJ.UserName);
                }
            }
            else
            {
                // 通过本地登录方式进行登录
                // 若没登录或者登录的标识不一致，需要重新登录
                if (!WebUtil.UserIsLogon() || !UserInfo.OpenId.Equals(OpenId))
                {
                    UserInfo = new CurrentUserInfo(WebUtil.LogonByOpenId(OpenId).UserInfo);
                }
            }            
        }
    }
    #endregion

    #region protected string GetReturnUrl() 获取返回页面
    /// <summary>
    /// 获取返回页面
    /// </summary>
    /// <returns></returns>
    protected string GetReturnUrl()
    {
        var result = string.Empty;
        if (Page.Request["ReturnUrl"] != null)
        {
            if (!Page.Request["ReturnUrl"].Contains("Login.aspx") && !Page.Request["ReturnUrl"].Contains("Guide.aspx") && !Page.Request["ReturnUrl"].Contains("Forgot.aspx") && !Page.Request["ReturnUrl"].Contains("Register.aspx") && !Page.Request["ReturnUrl"].Contains("Login.aspx") && !Page.Request["ReturnUrl"].Contains("Logout.aspx") && !Page.Request["ReturnUrl"].Contains("Logouting.aspx") && !Page.Request["ReturnUrl"].Contains("Loading.aspx"))
            {
                result = Page.Request["ReturnUrl"].Replace("Modules", BaseSystemInfo.MainPage + "?ReturnUrl=Modules");
            }
        }
        else
        {
            if (Request.UrlReferrer != null && Request.UrlReferrer.ToString().Contains(WebUtil.CookieDomain))
            {
                //不是本页面时
                if (!Request.UrlReferrer.ToString().Contains("Login.aspx") && !Request.UrlReferrer.ToString().Contains("Guide.aspx") && !Request.UrlReferrer.ToString().Contains("Forgot.aspx") && !Request.UrlReferrer.ToString().Contains("Register.aspx") && !Request.UrlReferrer.ToString().Contains("Login.aspx") && !Request.UrlReferrer.ToString().Contains("Logout.aspx") && !Request.UrlReferrer.ToString().Contains("Logouting.aspx") && !Request.UrlReferrer.ToString().Contains("Loading.aspx"))
                {
                    result = Request.UrlReferrer.ToString().Replace("Modules", BaseSystemInfo.MainPage + "?ReturnUrl=Modules");
                }
            }
        }

        return result;
    }
    #endregion

    #region protected void Page_Load() 页面初次加载
    /// <summary>
    /// 页面初次加载
    /// </summary>
    protected void Page_Load()
    {
        //if (!Page.ClientScript.IsClientScriptBlockRegistered("chost"))
        //{
        //    Page.ClientScript.RegisterClientScriptBlock(GetType(), "chost", "var chost='" + "http://" + Request.Url.Authority + Request.ApplicationPath + "';", true);
        //}
        //var jsurl = "http://" + Request.Url.Authority + Request.ApplicationPath + "/js/jquery-1.11.2.min.js";
        //if (!Page.ClientScript.IsClientScriptBlockRegistered("jquery"))
        //{
        //    Page.ClientScript.RegisterClientScriptInclude("jquery", jsurl);
        //}
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
    /// <param name="url">网址</param>
    public void LogException(Exception exception, string url = null)
    {
        //写入用户数据库
        BaseExceptionManager.LogException(UserCenterDbHelper, UserInfo, exception, url);
        // 写入业务数据库
        //BaseExceptionManager.LogException(dbHelper, UserInfo, exception, url);
        //写文本日志
        LogUtil.WriteException(exception);
    }
    #endregion

    #region public virtual string SortExpression 排序字段
    /// <summary>
    /// 排序字段
    /// </summary>
    public virtual string SortExpression
    {
        get
        {
            if (ViewState["sortExpression"] == null)
            {
                ViewState["sortExpression"] = BaseUtil.FieldId;
            }
            return ViewState["sortExpression"].ToString();
        }
        set => ViewState["sortExpression"] = value;
    }
    #endregion

    #region public virtual string SortDirection 排序顺序
    /// <summary>
    /// 排序顺序
    /// </summary>
    public virtual string SortDirection
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