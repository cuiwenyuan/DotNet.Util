//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
#if NET452_OR_GREATER
using System.Web.UI.WebControls;
#endif
namespace DotNet.Business
{
    using Util;
    using Model;

    public partial class WebUtil
    {
#if NET452_OR_GREATER
        #region 绑定字典到下拉列表
        /// <summary>
        /// 绑定下拉列表
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="dropDownList">下拉列表</param>
        /// <param name="dictionaryCode">字典编码</param>
        /// <param name="addEmptyItem">是否增加空行</param>
        public static void BindDictionary(BaseUserInfo userInfo, DropDownList dropDownList, string dictionaryCode, bool addEmptyItem = true)
        {
            dropDownList.Items.Clear();
            var dt = new BaseDictionaryItemManager(userInfo).GetDataTableByDictionaryCode(dictionaryCode);
            if (dt != null && dt.Rows.Count > 0)
            {
                dropDownList.DataValueField = BaseDictionaryItemEntity.FieldItemValue;
                dropDownList.DataTextField = BaseDictionaryItemEntity.FieldItemName;
                dt.DefaultView.Sort = BaseDictionaryItemEntity.FieldSortCode;
                dropDownList.DataSource = dt.DefaultView;
                dropDownList.DataBind();
            }
            if (addEmptyItem)
            {
                dropDownList.Items.Insert(0, new ListItem());
            }
        }
        #endregion

        #region 绑定字典到复选框列表

        /// <summary>
        /// 绑定字典到复选框列表
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="checkBoxList">复选框列表</param>
        /// <param name="dictionaryCode">字典编码</param>
        /// <param name="checkAll">选中全部</param>
        /// <param name="itemStyle">样式</param>
        public static void BindDictionary(BaseUserInfo userInfo, CheckBoxList checkBoxList, string dictionaryCode, bool checkAll = false, bool itemStyle = false)
        {
            checkBoxList.Items.Clear();
            var dt = new BaseDictionaryItemManager(userInfo).GetDataTableByDictionaryCode(dictionaryCode);
            if (dt != null && dt.Rows.Count > 0)
            {
                checkBoxList.DataValueField = BaseDictionaryItemEntity.FieldItemValue;
                checkBoxList.DataTextField = BaseDictionaryItemEntity.FieldItemName;
                dt.DefaultView.Sort = BaseDictionaryItemEntity.FieldSortCode;
                checkBoxList.DataSource = dt.DefaultView;
                checkBoxList.DataBind();
                if (checkAll)
                {
                    for (var i = 0; i < checkBoxList.Items.Count; i++)
                    {
                        checkBoxList.Items[i].Selected = true;
                        if (itemStyle)
                        {
                            checkBoxList.Items[i].Attributes.Add("style", "padding-right:10px");
                        }
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// 绑定组织机构类别
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="dropDownList">下拉列表</param>
        /// <param name="addEmptyItem">是否增加空行</param>
        public static void BindBaseOrganizationCategory(BaseUserInfo userInfo, DropDownList dropDownList, bool addEmptyItem = true)
        {
            WebUtil.BindDictionary(userInfo, dropDownList, "BaseOrganizationCategory");
        }

        /// <summary>
        /// 绑定角色类别
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="dropDownList">下拉列表</param>
        /// <param name="addEmptyItem">是否增加空行</param>
        public static void BindBaseRoleCategory(BaseUserInfo userInfo, DropDownList dropDownList, bool addEmptyItem = true)
        {
            WebUtil.BindDictionary(userInfo, dropDownList, "BaseRoleCategory");
        }

        /// <summary>
        /// 绑定模块类别
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="dropDownList">下拉列表</param>
        /// <param name="addEmptyItem">是否增加空行</param>
        public static void BindBaseModuleCategory(BaseUserInfo userInfo, DropDownList dropDownList, bool addEmptyItem = true)
        {
            WebUtil.BindDictionary(userInfo, dropDownList, "BaseModuleCategory");
        }

        /// <summary>
        /// 绑定操作类别
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="checkBoxList">复选框列表</param>
        /// <param name="checkAll">选中全部</param>
        /// <param name="itemStyle">样式</param>
        public static void BindBaseActionCategory(BaseUserInfo userInfo, CheckBoxList checkBoxList, bool checkAll = false, bool itemStyle = false)
        {
            WebUtil.BindDictionary(userInfo, checkBoxList, "BaseActionCategory");
        }

        /// <summary>
        /// 绑定子系统
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="dropDownList">下拉列表</param>
        /// <param name="addEmptyItem">是否增加空行</param>
        public static void BindBaseSystem(BaseUserInfo userInfo, DropDownList dropDownList, bool addEmptyItem = true)
        {
            WebUtil.BindDictionary(userInfo, dropDownList, "BaseSystemCode");
        }
#endif
    }
}