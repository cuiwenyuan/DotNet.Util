//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
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
///	版本：1.0 2012.11.10    JiRiGaLa    整理代码。
///	
/// 版本：1.0
/// <author>  
///		<name>Troy.Cui</name>
///		<date>2017.05.09</date>
/// </author> 
/// </remarks>
public partial class BasePage : System.Web.UI.Page
{
    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <param name="organizationId">组织机构主键</param>
    /// <param name="containSelf">是否包含自己</param>
    /// <param name="securityLevel">安全级别</param>
    /// <returns>数据表</returns>
    protected DataTable GetUser(string organizationId = null, bool? containSelf = null, int? securityLevel = null)
    {
        DataTable dtUser = null;
        var manager = new BaseUserManager(UserInfo);

        var sb = PoolUtil.StringBuilder.Get();
        sb.Append(" SELECT * FROM " + BaseUserEntity.CurrentTableName);
        sb.Append(" WHERE (" + BaseUserEntity.FieldDeleted + " = 0 AND " + BaseUserEntity.FieldEnabled + " = 1 AND " + BaseUserEntity.FieldIsVisible + " = 1 ");

        if (!string.IsNullOrEmpty(organizationId))
        {
            sb.Append(" AND " + BaseUserEntity.FieldDepartmentId + " = '" + organizationId + "' ");
        }
        if (securityLevel != null)
        {
            sb.Append(" AND " + BaseUserEntity.FieldSecurityLevel + " < " + securityLevel + " ");
        }
        sb.Append(" )");
        if (containSelf.HasValue)
        {
            if (containSelf == true)
            {
                sb.Append(" OR ( " + BaseUserEntity.FieldId + " = '" + UserInfo.Id + "'");
                sb.Append(" AND ( " + BaseUserEntity.FieldCompanyId + " = '" + organizationId + "'");
                sb.Append(" OR  " + BaseUserEntity.FieldSubCompanyId + " = '" + organizationId + "'");
                sb.Append(" OR  " + BaseUserEntity.FieldDepartmentId + " = '" + organizationId + "'");
                sb.Append(" OR  " + BaseUserEntity.FieldWorkgroupId + " = '" + organizationId + "'))");
            }
            else
            {
                sb.Append(" AND ( " + BaseUserEntity.FieldId + " != '" + UserInfo.Id + "')");
            }
        }

        sb.Append(" ORDER BY " + BaseUserEntity.FieldSortCode);

        dtUser = manager.Fill(sb.Return());
        dtUser.TableName = BaseUserEntity.CurrentTableName;
        dtUser.DefaultView.Sort = BaseUserEntity.FieldSortCode;
        return dtUser;
    }

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
        //ddlUser.Items.Clear();
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
    /// 获取用户列表
    /// 获得用户列表(按权限范围)
    /// </summary>
    /// <param name="ddlUser">用户选项</param>
    /// <param name="organizationId">部门主键</param>
    /// <param name="insertBlank">插入空行</param>
    /// <param name="permissionCode">权限编码</param>
    protected void GetUserByPermissionScope(DropDownList ddlUser, string organizationId = null, bool insertBlank = false, string permissionCode = "Resource.ManagePermission")
    {
        //ddlUser.Items.Clear();
        var entityList = new List<BaseUserEntity>();
        var manager = new BaseUserManager(UserInfo);
        if (!string.IsNullOrEmpty(organizationId))
        {
            entityList = manager.GetListByOrganizations(new string[] { organizationId });
        }
        else
        {
            if (UserInfo.IsAdministrator)
            {
                entityList = manager.GetList<BaseUserEntity>();
            }
            else
            {
                entityList = new BasePermissionScopeManager(UserInfo).GetUserList(UserInfo.SystemCode, UserInfo.Id.ToString(), permissionCode);
                // 至少要把自己显示出来，否则难控制权限了
                if (entityList.Count == 0)
                {
                    entityList = manager.GetList<BaseUserEntity>(new string[] { UserInfo.Id.ToString() });
                }
            }
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

    /// <summary>
    /// 有某个操作权限的所有用户列表
    /// 反向通知有权限的用户
    /// </summary>
    /// <param name="permissionCode">操作权限编号</param>
    /// <param name="permissionName">操作权限名称</param>
    /// <returns>用户主键数组</returns>
    public string[] GetPermissionUserIds(string permissionCode, string permissionName = null)
    {
        var manager = new BasePermissionManager(UserInfo);
        return manager.GetUserIds(permissionCode, permissionName);
    }

    /// <summary>
    /// 对某个部门有某种管理权限的所有用户列表
    /// 反向通知有权限的用户
    /// </summary>
    /// <param name="organizationId">组织机构主键</param>
    /// <param name="permissionCode">操作权限编号</param>
    /// <param name="permissionName">操作权限名称</param>
    /// <returns>用户主键数组</returns>
    public string[] GetPermissionScopeUserIds(string organizationId, string permissionCode, string permissionName = null)
    {
        var manager = new BasePermissionScopeManager(UserInfo);
        return manager.GetUserIds(organizationId, permissionCode, permissionName);
    }

    #region GetRoleCategory 角色类型
    /// <summary>
    /// 获取角色类型
    /// </summary>
    /// <param name="itemValue"></param>
    /// <returns></returns>
    public string GetRoleCategory(string itemValue)
    {
        var result = string.Empty;
        if (!string.IsNullOrEmpty(itemValue))
        {
            var entity = new BaseDictionaryItemManager(UserInfo).GetEntity("BaseRoleCategory", itemValue, itemValue);
            if (entity != null)
            {
                result = entity.ItemName;
            }
        }
        return result;
    }
    #endregion
}