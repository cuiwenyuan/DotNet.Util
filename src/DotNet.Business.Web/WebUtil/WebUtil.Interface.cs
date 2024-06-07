//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
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
        #region public static string BuildUrl(string url, string paramText, string paramValue)
        /// <summary>
        /// url里有key的值，就替换为value,没有的话就追加.构造Url的参数 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paramText"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public static string BuildUrl(string url, string paramText, string paramValue)
        {
            var reg = new Regex($"{paramText}=[^&]*", RegexOptions.IgnoreCase);
            var reg1 = new Regex("[&]{2,}", RegexOptions.IgnoreCase);
            var _url = reg.Replace(url, "");
            //_url = reg1.Replace(_url, "");
            if (_url.IndexOf("?", StringComparison.Ordinal) == -1)
            {
                _url += $"?{paramText}={paramValue}";//?
            }
            else
            {
                _url += $"&{paramText}={paramValue}";//&
            }
            _url = reg1.Replace(_url, "&");
            _url = _url.Replace("?&", "?");
            return _url;
        }
        #endregion

#if NET452_OR_GREATER

        #region public static bool SetDropDownListValue(DropDownList dropDownList, string selectedValue)
        /// <summary>
        /// 设置下拉框的被选中值
        /// </summary>
        /// <param name="dropDownList">下拉框</param>
        /// <param name="selectedValue">被选中的值</param>
        /// <returns>是否找到</returns>
        public static bool SetDropDownListValue(DropDownList dropDownList, string selectedValue)
        {
            var result = false;
            if (dropDownList.SelectedItem != null)
            {
                dropDownList.SelectedItem.Selected = false;
            }
            // 按值找
            var listItem = dropDownList.Items.FindByValue(selectedValue);
            if (listItem == null)
            {
                // 按显示的文本找
                for (var i = 0; i < dropDownList.Items.Count; i++)
                {
                    if (dropDownList.Items[i].Text.Equals(selectedValue))
                    {
                        dropDownList.Items[i].Selected = true;
                        result = true;
                        break;
                    }
                }
                // 还是没找到
                if (dropDownList.SelectedItem == null)
                {
                    listItem = new ListItem(selectedValue, selectedValue);
                    dropDownList.Items.Insert(0, listItem);
                }
            }
            else
            {
                // 设置为被选中状态
                listItem.Selected = true;
            }
            return result;
        }
        #endregion

        #region public static void SetDropDownList(DropDownList dropDownList, DataTable result, string fieldValue = "Id", string fieldText = "Name", string sortCode = "SortCode", bool addEmptyItem = true)
        /// <summary>
        /// 绑定下拉列表
        /// </summary>
        /// <param name="dropDownList">下拉列表</param>
        /// <param name="dt">表名</param>
        /// <param name="fieldValue">邦定的值字段</param>
        /// <param name="fieldText">邦定的名称字段</param>
        /// <param name="sortCode">排序</param>
        /// <param name="addEmptyItem">是否增加空行</param>
        public static void SetDropDownList(DropDownList dropDownList, DataTable dt, string fieldValue = "Id", string fieldText = "Name", string sortCode = "SortCode", bool addEmptyItem = true)
        {
            dropDownList.Items.Clear();
            dropDownList.DataValueField = fieldValue;
            dropDownList.DataTextField = fieldText;
            if (dt != null && dt.Rows.Count > 0)
            {
                dt.DefaultView.Sort = sortCode;
                dropDownList.DataSource = dt.DefaultView;
                dropDownList.DataBind();
            }

            if (addEmptyItem)
            {
                dropDownList.Items.Insert(0, new ListItem());
            }
        }
        #endregion

#endif
        #region public void CheckTreeParentId(DataTable result, string fieldId, string fieldParentId)
        /// <summary>
        /// 查找 ParentId 字段的值是否在 Id字段 里
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="fieldId">主键字段</param>
        /// <param name="fieldParentId">父节点字段</param>
        public void CheckTreeParentId(DataTable dt, string fieldId, string fieldParentId)
        {
            for (var i = dt.Rows.Count - 1; i >= 0; i--)
            {
                var dr = dt.Rows[i];
                // if (result.Columns[fieldId].GetType() == typeof(int))
                if (dr[fieldParentId].ToString().Length > 0)
                {
                    if (dt.Select(fieldId + " = " + dr[fieldParentId] + "").Length == 0)
                    {
                        dr[fieldParentId] = DBNull.Value;
                    }
                }
            }
        }
        #endregion
    }
}