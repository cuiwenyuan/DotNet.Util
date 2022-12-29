//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;
using System.Web.UI.WebControls;
using DotNet.Model;
using DotNet.Business;
using DotNet.Util;
using System.Collections.Generic;

/// <remarks>
/// BasePage
/// 基础网页类
/// 
/// 修改记录
/// 
/// 版本：4.1 2017.05.09    Troy Cui    完善代码。
///	版本：1.0 2016.09.24    Troy.Cui    整理代码。
///	
/// 版本：4.1
/// <author>  
///		<name>Troy.Cui</name>
///		<date>2016.09.24</date>
/// </author> 
/// </remarks>
public partial class BasePage : System.Web.UI.Page
{
    #region protected void GetModuleTree(string systemCode, DropDownList ddlDepartment, bool insertBlank = true, bool userDepartmentSelected = true)
    /// <summary>
    /// 获取菜单模块树型列表
    /// </summary>
    /// <param name="systemCode">子系统</param>
    /// <param name="ddlModule">菜单模块选项</param>
    /// <param name="insertBlank">插入空行</param>
    /// <param name="isMenu">是否菜单(0/1)</param>
    protected void GetModuleTree(string systemCode, DropDownList ddlModule, bool insertBlank = true, string isMenu = null)
    {
        ddlModule.Items.Clear();
        if (string.IsNullOrEmpty(systemCode))
        {
            systemCode = "Base";
        }
        //读取选定子系统的菜单模块
        var dt = new BaseModuleManager(UserInfo).GetModuleTree(systemCode, isMenu);
        if (dt != null && dt.Rows.Count > 0)
        {
            ddlModule.DataValueField = BaseModuleEntity.FieldId;
            ddlModule.DataTextField = BaseModuleEntity.FieldName;
            ddlModule.DataSource = dt;
            ddlModule.DataBind();
        }
        if (insertBlank)
        {
            ddlModule.Items.Insert(0, new ListItem());
        }
    }
    #endregion

    #region GetModuleCategory 菜单模块类型
    /// <summary>
    /// 获取菜单模块类型
    /// </summary>
    /// <param name="itemValue"></param>
    /// <returns></returns>
    public string GetModuleCategory(string itemValue)
    {
        var result = string.Empty;
        if (!string.IsNullOrEmpty(itemValue))
        {
            var entity = new BaseDictionaryItemManager(UserInfo).GetEntity("BaseModuleCategory", itemValue, itemValue);
            if (entity != null)
            {
                result = entity.ItemName;
            }
        }
        return result;
    }
    #endregion

}