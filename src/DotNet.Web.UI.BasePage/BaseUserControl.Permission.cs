//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using DotNet.Model;
using DotNet.Business;
using DotNet.Util;
using System.Web.Caching;

/// <remarks>
/// BasePage
/// 基础网页类
/// 
/// 修改记录
///	版本：2.0 2012.02.25    JiRiGaLa    创建文件。
///	
/// 版本：2.5
/// <author>  
///		<name>Troy.Cui</name>
///		<date>2012.02.25</date>
/// </author> 
/// </remarks>
public partial class BaseUserControl : UserControl
{
    /// <summary>
    /// 访问权限
    /// </summary>
    protected bool PermissionAccess = true;

    /// <summary>
    /// 新增权限
    /// </summary>
    protected bool PermissionAdd = true;

    /// <summary>
    /// 编辑权限
    /// </summary>
    protected bool PermissionEdit = true;

    /// <summary>
    /// 删除权限
    /// </summary>
    protected bool PermissionDelete = true;

    /// <summary>
    /// 查询权限
    /// </summary>
    protected bool PermissionSearch = true;

    /// <summary>
    /// 管理权限
    /// </summary>
    protected bool PermissionAdmin = false;

    /// <summary>
    /// 导出权限
    /// </summary>
    protected bool PermissionExport = true;


    // 操作权限

    #region public bool IsAuthorized(string permissionItemCode, string permissionItemName = null) 是否有相应的权限
    /// <summary>
    /// 是否有相应的权限
    /// </summary>
    /// <param name="permissionItemCode">权限编号</param>
    /// <param name="permissionItemName">权限名称</param>
    /// <returns>是否有权限</returns>
    public bool IsAuthorized(string permissionItemCode, string permissionItemName = null)
    {
        var dotNetService = new DotNetService();
        return dotNetService.PermissionService.IsAuthorized(UserInfo, UserInfo.Id.ToString(), permissionItemCode, permissionItemName);
    }
    /// <summary>
    /// 指定用户是否有相应的权限
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="permissionItemCode"></param>
    /// <param name="permissionItemName"></param>
    /// <returns></returns>
    public bool IsAuthorized(string userId, string permissionItemCode, string permissionItemName = null)
    {
        var dotNetService = new DotNetService();
        return dotNetService.PermissionService.IsAuthorized(UserInfo, userId, permissionItemCode, permissionItemName);
    }
    #endregion

    #region public void Authorized(string permissionItemCode, string accessDenyUrl = null) 是否有相应权限
    /// <summary>
    /// 是否有相应权限
    /// </summary>
    /// <param name="permissionItemCode">权限编号</param>
    /// <param name="accessDenyUrl">访问被阻止的url</param>
    public void Authorized(string permissionItemCode, string accessDenyUrl = null)
    {
        // 若没有相应的权限，那就跳转到没有权限的页面里
        if (!WebUtil.UserIsLogon() || !IsAuthorized(permissionItemCode))
        {
            if (!string.IsNullOrEmpty(accessDenyUrl))
            {
                HttpContext.Current.Response.Redirect(accessDenyUrl);
            }
            else
            {
                HttpContext.Current.Response.Redirect(WebUtil.AccessDenyPage + "?Code=" + permissionItemCode);
            }
        }
    }
    #endregion

    #region public bool UserCanSystemAdmin() 判断用户是否是管理员
    /// <summary>
    /// 判断用户是否是管理员
    /// </summary>
    /// <returns></returns>
    public bool UserCanSystemAdmin()
    {
        // 1.先判断是否已登录
        if (WebUtil.CheckIsLogon())
        {
            // 2.有没有管理员审核的权限？
            if (!IsAuthorized("SystemAdmin"))
            {
                //HttpContext.Current.Response.Redirect(WebUtil.UserLogonPage);
                Response.Write("<script>window.top.location.href='" + WebUtil.UserLogonPage + "'</script>");
                return false;
            }
        }
        else
        {
            return false;
        }
        return true;
    }
    #endregion


    // 模块菜单权限

    #region public bool IsModuleAuthorized(string moduleCode) 是否有相应的模块权限
    /// <summary>
    /// 是否有相应的模块权限
    /// </summary>
    /// <param name="moduleCode">模块编号</param>
    /// <returns>是否有权限</returns>
    public bool IsModuleAuthorized(string moduleCode)
    {
        if (UserInfo.IsAdministrator)
        {
            return true;
        }
        // 这里也可以优化一下，没必要遍历所有的模块列表
        var count = ModuleList().Count(entity => entity.Code.Equals(moduleCode, StringComparison.OrdinalIgnoreCase));
        return count > 0;
    }
    #endregion

    #region public void ModuleAuthorized(string moduleCode, string accessDenyUrl = null) 是否有相应的模块权限
    /// <summary>
    /// 是否有相应的模块权限
    /// </summary>
    /// <param name="moduleCode">模块编号</param>
    /// /// <param name="accessDenyUrl">访问被阻止的url</param>
    public void ModuleAuthorized(string moduleCode, string accessDenyUrl = null)
    {
        // 若没有相应的权限，那就跳转到没有权限的页面里
        if (!WebUtil.UserIsLogon() || !IsModuleAuthorized(moduleCode))
        {
            if (!string.IsNullOrEmpty(accessDenyUrl))
            {
                HttpContext.Current.Response.Redirect(accessDenyUrl);
            }
            else
            {
                HttpContext.Current.Response.Redirect(WebUtil.AccessDenyPage + "?Code=" + moduleCode);
            }
        }
    }
    #endregion


    #region protected List<BaseModuleEntity> ModuleList 获取模块数据表
    /// <summary>
    /// 获取模块数据表
    /// </summary>
    public List<BaseModuleEntity> ModuleList()
    {
        return WebUtil.GetUserPermissionList(UserInfo, UserInfo.Id.ToString());
    }
    #endregion

    // 用户是否在某个角色里（按编号，按名称的）

    #region public bool UserIsInRole(string roleCode)
    /// <summary>
    /// 用户是否在某个角色里
    /// </summary>
    /// <param name="roleCode">角色编号</param>
    /// <returns>是否在某个角色里</returns>
    public bool UserIsInRole(string roleCode)
    {
        var userManager = new BaseUserManager(_userInfo);
        return userManager.IsInRoleByCode(UserInfo.Id.ToString(), roleCode);
    }
    #endregion


    // 用户是否在某个部门（按编号，按名称的，按简称的)

    #region public bool UserIsInOrganization(string organizationName)
    /// <summary>
    /// 用户是否在某个组织架构里的判断
    /// </summary>
    /// <param name="organizationName">角色编号</param>
    /// <returns>是否在某个角色里</returns>
    public bool UserIsInOrganization(string organizationName)
    {
        var userManager = new BaseUserManager(_userInfo);
        return userManager.IsInOrganization(UserInfo.Id.ToString(), organizationName);
    }
    #endregion


    // 获得部门列表(按权限范围)

    #region protected void GetDepartment(DropDownList ddlDepartment, bool insertBlank = true, bool userDepartmentSelected = true)
    /// <summary>
    /// 获取部门列表
    /// </summary>
    /// <param name="ddlDepartment">部门选项</param>
    /// <param name="insertBlank">插入空行</param>
    /// <param name="userDepartmentSelected">默认选中自己的部门</param>
    protected void GetDepartment(DropDownList ddlDepartment, bool insertBlank = true, bool userDepartmentSelected = true)
    {
        var manager = new BaseOrganizationManager(UserCenterDbHelper, UserInfo);
        var dt = manager.GetOrganizationDataTable();
        ddlDepartment.SelectedValue = null;
        if (dt != null && dt.Rows.Count > 0)
        {
            dt.DefaultView.Sort = BaseOrganizationEntity.FieldSortCode;
            ddlDepartment.DataValueField = BaseOrganizationEntity.FieldId;
            ddlDepartment.DataTextField = BaseOrganizationEntity.FieldName;
            ddlDepartment.DataSource = dt;
            ddlDepartment.DataBind();
        }

        if (insertBlank)
        {
            ddlDepartment.Items.Insert(0, new ListItem());
        }
        if (userDepartmentSelected)
        {
            WebUtil.SetDropDownListValue(ddlDepartment, UserInfo.DepartmentId);
        }
    }
    #endregion

    #region protected void GetDepartmentIdsByPermissionScope(bool userDepartment = false, bool insertBlank = false, string permissionItemCode = "Resource.ManagePermission")
    /// <summary>
    /// 按权限范围获取部门数组
    /// </summary>
    /// <param name="permissionItemCode">操作权限项</param>
    /// <param name="insertBlank">插入空行</param>
    /// <param name="userDepartment">若没数据库至少显示用户自己的部门</param>
    protected string[] GetDepartmentIdsByPermissionScope(bool userDepartment = false, bool insertBlank = false, string permissionItemCode = "Resource.ManagePermission")
    {
        var dtDepartment = GetDepartmentByPermissionScope(userDepartment, insertBlank, permissionItemCode);
        return BaseUtil.FieldToArray(dtDepartment, BaseOrganizationEntity.FieldId);
    }
    #endregion

    #region protected void GetDepartmentByPermissionScope(bool userDepartment = false, bool insertBlank = false, string permissionItemCode = "Resource.ManagePermission")
    /// <summary>
    /// 按权限范围获取部门列表
    /// </summary>
    /// <param name="permissionItemCode">操作权限项</param>
    /// <param name="insertBlank">插入空行</param>
    /// <param name="userDepartment">若没数据库至少显示用户自己的部门</param>
    protected DataTable GetDepartmentByPermissionScope(bool userDepartment = false, bool insertBlank = false, string permissionItemCode = "Resource.ManagePermission")
    {
        DataTable dtDepartment = null;
        var manager = new BaseOrganizationManager(UserCenterDbHelper, UserInfo);
        if (UserInfo.IsAdministrator)
        {
            dtDepartment = manager.GetOrganizationDataTable();
        }
        else
        {
            var permissionService = new BasePermissionService();
            dtDepartment = permissionService.GetOrganizationDTByPermission(_userInfo, _userInfo.Id.ToString(), permissionItemCode);

            // BasePermissionScopeManager permissionScopeManager = new BasePermissionScopeManager(dbHelper, userInfo);
            // dtDepartment = permissionScopeManager.GetOrganizationDT(userInfo.Id, permissionItemCode, false);
        }
        // 至少要列出自己的部门的(其实这里还看是否存在了)
        if (userDepartment)
        {
            if (!string.IsNullOrEmpty(UserInfo.DepartmentId))
            {
                if (!BaseUtil.Exists(dtDepartment, BaseOrganizationEntity.FieldId, UserInfo.DepartmentId))
                {
                    dtDepartment.Merge(manager.GetDataTableById(UserInfo.DepartmentId));
                }
            }
        }
        dtDepartment.DefaultView.Sort = BaseOrganizationEntity.FieldSortCode;
        return dtDepartment;
    }
    #endregion

    #region protected void GetDepartmentByPermissionScope(DropDownList ddlDepartment, bool userDepartment = false, bool insertBlank = false, string permissionItemCode = "Resource.ManagePermission")
    /// <summary>
    /// 按权限范围获取部门列表
    /// </summary>
    /// <param name="ddlDepartment">部门选项</param>
    /// <param name="permissionItemCode">操作权限项</param>
    /// <param name="insertBlank">插入空行</param>
    /// <param name="userDepartment">若没数据库至少显示用户自己的部门</param>
    protected void GetDepartmentByPermissionScope(DropDownList ddlDepartment, bool userDepartment = false, bool insertBlank = false, string permissionItemCode = "Resource.ManagePermission")
    {
        ddlDepartment.Items.Clear();
        var dt = GetDepartmentByPermissionScope(userDepartment, insertBlank, permissionItemCode);
        ddlDepartment.SelectedValue = null;
        if (dt != null && dt.Rows.Count > 0)
        {
            ddlDepartment.DataValueField = BaseOrganizationEntity.FieldId;
            ddlDepartment.DataTextField = BaseOrganizationEntity.FieldName;
            ddlDepartment.DataSource = dt.DefaultView;
            ddlDepartment.DataBind();
        }
        // 若是超级管理员或者需要有插入空行
        if (UserInfo.IsAdministrator || insertBlank)
        {
            ddlDepartment.Items.Insert(0, new ListItem());
        }
    }
    #endregion


    // 获得用户列表(按权限范围)

    #region protected void GetUserByPermissionScope(DropDownList ddlUser, string organizationId = null, bool insertBlank = false, string permissionItemCode = "Resource.ManagePermission")
    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <param name="ddlUser">用户选项</param>
    /// <param name="organizationId">部门主键</param>
    /// <param name="insertBlank">插入空行</param>
    /// <param name="permissionItemCode">权限编码</param>
    protected void GetUserByPermissionScope(DropDownList ddlUser, string organizationId = null, bool insertBlank = false, string permissionItemCode = "Resource.ManagePermission")
    {
        ddlUser.Items.Clear();
        var entityList = new List<BaseUserEntity>();
        var manager = new BaseUserManager(UserInfo);
        if (string.IsNullOrEmpty(organizationId))
        {
            if (UserInfo.IsAdministrator)
            {
                entityList = manager.GetList<BaseUserEntity>();
            }
            else
            {
                var permissionService = new BasePermissionService();
                entityList = permissionService.GetUserListByPermission(_userInfo, _userInfo.Id.ToString(), permissionItemCode);
                // 至少要把自己显示出来，否则难控制权限了
                if (entityList.Count == 0)
                {
                    entityList = manager.GetList<BaseUserEntity>(new string[] { UserInfo.Id.ToString() });
                }
            }
        }
        else
        {
            entityList = manager.GetListByOrganizations(new string[] { organizationId });
        }
        ddlUser.SelectedValue = null;
        if (entityList != null && entityList.Count > 0)
        {
            ddlUser.DataValueField = BaseUserEntity.FieldId;
            ddlUser.DataTextField = BaseUserEntity.FieldRealName;
            ddlUser.DataSource = entityList;
            ddlUser.DataBind();
        }

        if (UserInfo.IsAdministrator || insertBlank)
        {
            ddlUser.Items.Insert(0, new ListItem());
        }
    }
    #endregion

    #region protected void GetUser(DropDownList ddlUser, string organizationId = null, bool insertBlank = true, int? securityLevel = null)
    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <param name="ddlUser">用户选项</param>
    /// <param name="organizationId">部门主键</param>
    /// <param name="insertBlank">插入空行</param>
    /// <param name="containSelf">包含自己</param>
    /// <param name="securityLevel">安全级别</param>
    protected void GetUser(DropDownList ddlUser, string organizationId = null, bool insertBlank = true, bool? containSelf = null, int? securityLevel = null)
    {
        ddlUser.Items.Clear();
        ddlUser.SelectedValue = null;
        var dt = GetUser(organizationId, containSelf, securityLevel);
        if (dt != null && dt.Rows.Count > 0)
        {
            ddlUser.DataValueField = BaseUserEntity.FieldId;
            ddlUser.DataTextField = BaseUserEntity.FieldRealName;
            ddlUser.DataSource = dt;
            ddlUser.DataBind();
        }
        if (insertBlank)
        {
            ddlUser.Items.Insert(0, new ListItem());
        }
        if (containSelf.HasValue && containSelf == true)
        {
            ddlUser.SelectedValue = UserInfo.Id.ToString();
        }
    }
    /// <summary>
    /// 获取用户
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="containSelf"></param>
    /// <param name="securityLevel"></param>
    /// <returns></returns>
    protected DataTable GetUser(string organizationId = null, bool? containSelf = null, int? securityLevel = null)
    {
        DataTable dtUser = null;
        var manager = new BaseUserManager(UserInfo);

        var sql = string.Empty;
        sql = " SELECT * "
                + "   FROM " + BaseUserEntity.CurrentTableName
                + "  WHERE (" + BaseUserEntity.FieldDeleted + " = 0 "
                + "       AND " + BaseUserEntity.FieldEnabled + " = 1 "
                + "       AND " + BaseUserEntity.FieldIsVisible + " = 1 ";

        if (!string.IsNullOrEmpty(organizationId))
        {
            sql += " AND " + BaseUserEntity.FieldDepartmentId + " = '" + organizationId + "' ";
        }
        if (securityLevel != null)
        {
            sql += " AND " + BaseUserEntity.FieldSecurityLevel + " < " + securityLevel + " ";
        }
        sql += " ) ";
        if (containSelf.HasValue)
        {
            if (containSelf == true)
            {
                sql += " OR ( " + BaseUserEntity.FieldId + " = '" + UserInfo.Id + "'";
                sql += " AND ( " + BaseUserEntity.FieldCompanyId + " = '" + organizationId + "'";
                sql += " OR  " + BaseUserEntity.FieldSubCompanyId + " = '" + organizationId + "'";
                sql += " OR  " + BaseUserEntity.FieldDepartmentId + " = '" + organizationId + "'";
                sql += " OR  " + BaseUserEntity.FieldWorkgroupId + " = '" + organizationId + "'))";
            }
            else
            {
                sql += " AND ( " + BaseUserEntity.FieldId + " != '" + UserInfo.Id + "')";
            }
        }

        sql += " ORDER BY " + BaseUserEntity.FieldSortCode;

        dtUser = manager.Fill(sql);
        dtUser.TableName = BaseUserEntity.CurrentTableName;
        dtUser.DefaultView.Sort = BaseUserEntity.FieldSortCode;
        return dtUser;
    }
    #endregion
}