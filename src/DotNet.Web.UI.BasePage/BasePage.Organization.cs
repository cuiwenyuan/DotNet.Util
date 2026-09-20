//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2026, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;
using System.Web.UI.WebControls;
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
///	版本：1.0 2012.11.10    JiRiGaLa    整理代码。
///	
/// 版本：4.1
/// <author>  
///		<name>Troy.Cui</name>
///		<date>2017.05.09</date>
/// </author> 
/// </remarks>
public partial class BasePage : System.Web.UI.Page
{
    // 用户是否在某个部门（按编号，按名称的，按简称的)

    #region public bool IsUserInOrganization(string organizationName)
    /// <summary>
    /// 用户是否在某个组织架构里的判断
    /// </summary>
    /// <param name="organizationName">角色编号</param>
    /// <returns>是否在某个角色里</returns>
    public bool IsUserInOrganization(string organizationName)
    {
        var userManager = new BaseUserManager(UserInfo);
        return userManager.IsInOrganization(UserInfo.Id.ToString(), organizationName);
    }
    #endregion

    // 获取所有的部门列表

    #region protected void GetDepartment(DropDownList ddlDepartment, bool insertBlank = true, bool userDepartmentSelected = true)

    /// <summary>
    /// 获取部门列表
    /// </summary>
    /// <param name="dropDownList">下拉控件</param>
    /// <param name="insertBlank">插入空行</param>
    /// <param name="parentId">父级编号</param>
    /// <param name="categoryCode">组织分类（Company,SubCompany,Department,SubDepartment,Workgroup）</param>
    /// <param name="userCompanySelected">默认选中自己的公司</param>
    /// <param name="userSubCompanySelected">默认选中自己的子公司</param>
    /// <param name="userDepartmentSelected">默认选中自己的部门</param>
    /// <param name="userSubDepartmentSelected">默认选中自己的子部门</param>
    /// <param name="userWorkgroupSelected">默认选中自己的工作组</param>
    protected void GetOrganizationDataTable(DropDownList dropDownList, bool insertBlank = true, string parentId = null, string categoryCode = "Department", bool userCompanySelected = false, bool userSubCompanySelected = false, bool userDepartmentSelected = false, bool userSubDepartmentSelected = false, bool userWorkgroupSelected = false)
    {
        //dropDownList.Items.Clear();
        var manager = new BaseOrganizationManager(UserInfo);
        var dt = manager.GetOrganizationDataTable(parentId, false, categoryCode);
        dropDownList.SelectedValue = null;
        if (dt != null && dt.Rows.Count > 0)
        {
            dt.DefaultView.Sort = BaseOrganizationEntity.FieldSortCode;
            dropDownList.DataValueField = BaseOrganizationEntity.FieldId;
            dropDownList.DataTextField = BaseOrganizationEntity.FieldName;
            dropDownList.DataSource = dt;
        }
        dropDownList.DataBind();
        if (insertBlank)
        {
            dropDownList.Items.Insert(0, new ListItem());
        }
        if (userCompanySelected)
        {
            WebUtil.SetDropDownListValue(dropDownList, UserInfo.CompanyId);
        }
        if (userSubCompanySelected)
        {
            WebUtil.SetDropDownListValue(dropDownList, UserInfo.SubCompanyId);
        }
        if (userDepartmentSelected)
        {
            WebUtil.SetDropDownListValue(dropDownList, UserInfo.DepartmentId);
        }
        if (userSubDepartmentSelected)
        {
            WebUtil.SetDropDownListValue(dropDownList, UserInfo.SubDepartmentId);
        }
        if (userWorkgroupSelected)
        {
            WebUtil.SetDropDownListValue(dropDownList, UserInfo.WorkgroupId);
        }
    }
    #endregion


    #region 获取组织的树形选项
    /// <summary>
    /// 获取组织的树形选项
    /// </summary>
    /// <param name="dropDownList">下拉控件</param>
    /// <param name="insertBlank">插入空行</param>
    /// <param name="userCompanySelected">默认选中自己的公司</param>
    /// <param name="userSubCompanySelected">默认选中自己的子公司</param>
    /// <param name="userDepartmentSelected">默认选中自己的部门</param>
    /// <param name="userSubDepartmentSelected">默认选中自己的子部门</param>
    /// <param name="userWorkgroupSelected">默认选中自己的工作组</param>
    protected void GetOrganizationTree(DropDownList dropDownList, bool insertBlank = true, bool userCompanySelected = false, bool userSubCompanySelected = false, bool userDepartmentSelected = false, bool userSubDepartmentSelected = false, bool userWorkgroupSelected = false)
    {
        dropDownList.Items.Clear();
        var manager = new BaseOrganizationManager(UserInfo);
        //2017.12.20增加默认的HttpRuntime.Cache缓存
        var cacheKey = "Dt.BaseOrganizationTree";
        //var cacheTime = default(TimeSpan);
        var cacheTime = TimeSpan.FromMilliseconds(86400000);
        var dt = CacheUtil.Cache<DataTable>(cacheKey, () => manager.GetOrganizationTree(), true, false, cacheTime);
        if (dt != null && dt.Rows.Count > 0)
        {
            dropDownList.DataValueField = BaseOrganizationEntity.FieldId;
            dropDownList.DataTextField = BaseOrganizationEntity.FieldName;
            dropDownList.DataSource = dt;
            dropDownList.DataBind();
        }

        if (insertBlank)
        {
            dropDownList.Items.Insert(0, new ListItem());
        }
        if (userCompanySelected)
        {
            WebUtil.SetDropDownListValue(dropDownList, UserInfo.CompanyId);
        }
        if (userSubCompanySelected)
        {
            WebUtil.SetDropDownListValue(dropDownList, UserInfo.SubCompanyId);
        }
        if (userDepartmentSelected)
        {
            WebUtil.SetDropDownListValue(dropDownList, UserInfo.DepartmentId);
        }
        if (userSubDepartmentSelected)
        {
            WebUtil.SetDropDownListValue(dropDownList, UserInfo.SubDepartmentId);
        }
        if (userWorkgroupSelected)
        {
            WebUtil.SetDropDownListValue(dropDownList, UserInfo.WorkgroupId);
        }
    }
    #endregion

    #region GetOrganizationCode 获得组织编码
    /// <summary>
    /// 获得组织编码
    /// </summary>
    /// <param name="organizationId"></param>
    /// <returns></returns>
    public string GetOrganizationCode(string organizationId)
    {
        return new BaseOrganizationManager(UserInfo).GetOrganizationCode(organizationId);
    }
    #endregion

    #region GetOrganizationName 获得组织名称
    /// <summary>
    /// 获得组织名称
    /// </summary>
    /// <param name="organizationId"></param>
    /// <returns></returns>
    public string GetOrganizationName(string organizationId)
    {
        return new BaseOrganizationManager(UserInfo).GetOrganizationName(organizationId);
    }
    #endregion

    #region 获取组织机构类型
    /// <summary>
    /// 获取组织机构类型
    /// </summary>
    /// <param name="itemValue"></param>
    /// <returns></returns>
    public string GetOrganizationCategory(string itemValue)
    {
        var result = string.Empty;
        if (!itemValue.IsNullOrEmpty())
        {
            var entity = new BaseDictionaryItemManager(UserInfo).GetEntity("BaseOrganizationCategory", itemValue, itemValue);
            if (entity != null)
            {
                result = entity.ItemName;
            }
        }
        return result;
    }
    #endregion
}
