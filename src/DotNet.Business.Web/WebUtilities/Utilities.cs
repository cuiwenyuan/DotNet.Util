﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
#if NET40_OR_GREATER
using System.Web.UI.WebControls;
#endif
namespace DotNet.Business
{
    using Util;
    using Model;

    public partial class Utilities
    {
        /// <summary>
        /// 上传文件的路径定义
        /// </summary>
        public static string UploadFiles = "UploadFiles";

        /// <summary>
        /// 是否显示提示信息
        /// </summary> 
        public static bool ShowInformation = true;

        /// <summary>
        /// 您确认要保存吗
        /// </summary>
        public static string LangSaveConfirm = " Are your sure to confirm? ";

        /// <summary>
        /// 您确认要删除吗
        /// </summary>
        public static string LangDeleteConfirm = " Are you sure to delete? ";

        /// <summary>
        /// 请仔细核对数据，确认输入的正确吗？
        /// </summary>
        public static string LangConfirm = " Please check and make sure your data is correct? ";

        /// <summary>
        /// 默认页面
        /// </summary>
        public static string DefaultPage = @"~/Default.aspx";

        /// <summary>
        /// 内容未找到页面
        /// </summary>
        public static string NotFoundPage = @"~/Modules/Common/System/NotFound.aspx";

        /// <summary>
        /// 用户登录页面
        /// </summary>
        public static string UserLogOnPage = @"/Login.aspx";
        /// <summary>
        /// 用户注销登录页面
        /// </summary>
        public static string UserLogOutPage = @"/Logouting.aspx";

        /// <summary>
        /// 访问没有权限被拒绝页面
        /// </summary>
        public static string AccessDenyPage = @"~/Modules/Common/System/AccessDeny.aspx";

        /// <summary>
        /// 当前操作员不是系统管理员页面
        /// </summary>
        public static string UserIsNotAdminPage = @"~/Modules/Common/System/AccessDeny.aspx";

        /// <summary>
        /// 选择是简易管理模式，是否部门管理权限管理角色管理等页面很复杂？
        /// </summary>
        protected bool SimpleManagerMode = true;

        /// <summary>
        /// 获取OpenId
        /// </summary>
        /// <returns></returns>
        public static string GetOpenId()
        {
            var openId = string.Empty;
            var userInfo = GetUserInfo();
            if (userInfo != null)
            {
                openId = userInfo.OpenId;
            }
            return openId;
        }

        #region public static BaseUserInfo GetUserInfo() 获取用户信息
        /// <summary>
        /// 获取用户信息（从Session和Cookie中获取）
        /// </summary>
        /// <returns>用户信息</returns>
        public static BaseUserInfo GetUserInfo()
        {
            BaseUserInfo userInfo = null;
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                if (HttpContext.Current.Session[SessionName] != null)
                {
                    userInfo = (BaseUserInfo)HttpContext.Current.Session[SessionName];
                    LogUtil.WriteLog("Get UserInfo from Session", "UserInfo");
                }
                else
                {
                    userInfo = GetUserCookie();
                    // 从 Session 读取 当前操作员信息
                }
            }
            //Troy.Cui 2018-05-12
            if (userInfo == null)
            {
                userInfo = GetUserCookie();
            }

            if (userInfo == null)
            {
                userInfo = new BaseUserInfo();
                //    result.Id = userIP;
                //    result.RealName = userIP;
                //    result.UserName = userIP;
                //    result.IPAddress = userIP;
            }
            else
            {
                userInfo.IpAddress = Utils.GetIp();
            }
            return userInfo;
        }
        #endregion

        //
        // 上传下载文件部分
        //
#if NET40_OR_GREATER
        #region public static string UpLoadFile(string categoryId, string objectId, System.Web.HttpPostedFile httpPostedFile, ref string loadDirectory, bool deleteFile) 上传文件

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="categoryId">分类代码</param>
        /// <param name="objectId">实物代码</param>
        /// <param name="httpPostedFile">被上传的文件</param>
        /// <param name="loadDirectory">上传目录</param>
        /// <param name="deleteFile">是否删除原文件夹</param>
        /// <param name="fileName">文件名</param>
        /// <returns>上传的文件位置</returns>
        public static string UpLoadFile(string categoryId, string objectId, HttpPostedFile httpPostedFile, ref string loadDirectory, bool deleteFile, string fileName = null)
        {
            // 服务器上的绝对路径
            var rootPath = HttpContext.Current.Server.MapPath("~/") + UploadFiles + "\\";
            // 图片重新指定，这里主要是为了起备份的作用，按日期把新的照片备份好就可以了。
            if (loadDirectory.Length == 0)
            {
                // 当前日期
                // string dateTime = DateTime.Now.ToString(BaseSystemInfo.DateFormat).ToString();
                // loadDirectory = categoryId + "\\" + dateTime + "\\" + objectId;
                loadDirectory = categoryId + "\\" + objectId;
            }
            // 需要创建的目录，图片放在这里，为了保存历史纪录，使用了当前日期做为目录
            var makeDirectory = rootPath + loadDirectory;
            if (deleteFile)
            {
                // 删除原文件
                if (Directory.Exists(makeDirectory))
                {
                    // Directory.Delete(makeDirectory, true);
                }
            }
            Directory.CreateDirectory(makeDirectory);
            // 获得文件名
            var postedFileName = string.Empty;
            if (string.IsNullOrEmpty(fileName))
            {
                postedFileName = HttpContext.Current.Server.HtmlEncode(Path.GetFileName(httpPostedFile.FileName));
            }
            else
            {
                postedFileName = fileName;
            }
            // 图片重新指定，虚拟的路径
            // 这里还需要更新学生的最新照片
            var fileUrl = loadDirectory + "\\" + postedFileName;
            // 文件复制到相应的路径下
            var copyToFile = makeDirectory + "\\" + postedFileName;
            httpPostedFile.SaveAs(copyToFile);
            return fileUrl;
        }
        #endregion

        #region public static string UpLoadFile(string categoryId, string objectId, string loadDirectory, bool deleteFile) 上传文件
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="categoryId">分类代码</param>
        /// <param name="objectId">实物代码</param>
        /// <param name="loadDirectory">上传目录</param>
        /// <param name="deleteFile">是否删除原文件夹</param>
        /// <returns>上传的文件位置</returns>
        public static string UpLoadFile(string categoryId, string objectId, string loadDirectory, bool deleteFile)
        {
            return UpLoadFile(categoryId, objectId, HttpContext.Current.Request.Files[0], ref loadDirectory, deleteFile);
        }
        #endregion

        #region public static string UpLoadFiles(string categoryId, string objectId, string upLoadDirectory) 上传文件
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="categoryId">分类代码</param>
        /// <param name="objectId">实物代码</param>
        /// <param name="upLoadDirectory">上传的目录</param>
        /// <returns>上传目录</returns>
        public static string UpLoadFiles(string categoryId, string objectId, string upLoadDirectory)
        {
            // 上传文件的复制文件部分
            var upLoadFilePath = string.Empty;
            for (var i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                if (HttpContext.Current.Request.Files[i].ContentLength > 0)
                {
                    // 获取文件名
                    var fileName = HttpContext.Current.Server.HtmlEncode(Path.GetFileName(HttpContext.Current.Request.Files[i].FileName));
                    upLoadFilePath = UpLoadFile(categoryId, objectId, HttpContext.Current.Request.Files[i], ref upLoadDirectory, false);
                }
            }
            return upLoadFilePath;
        }
        #endregion
#endif
        //
        // 表格选择记录功能部分 GridView
        //
#if NET40_OR_GREATER
        #region public static string[] GetSelecteIds(GridView gv) 获得已选的表格行代码数组
        /// <summary>
        /// 获得已选的表格行代码数组
        /// </summary>
        /// <param name="gv">表格</param>
        /// <returns>代码数组</returns>
        public static string[] GetSelecteIds(GridView gv)
        {
            return GetSelecteIds(gv, true);
        }
        #endregion

        #region public static string[] GetUnSelecteIds(GridView gv) 获得未选的表格行代码数组
        /// <summary>
        /// 获得未选的表格行代码数组
        /// </summary>
        /// <param name="gv">表格</param>
        /// <returns>代码数组</returns>
        public static string[] GetUnSelecteIds(GridView gv)
        {
            return GetSelecteIds(gv, false);
        }
        #endregion

        #region public static string[] GetSelecteIds(GridView gv, bool paramChecked)
        /// <summary>
        /// 获得表格行代码数组
        /// </summary>
        /// <param name="gv">表格</param>
        /// <param name="paramChecked">选中状态</param>
        /// <returns>代码数组</returns>
        public static string[] GetSelecteIds(GridView gv, bool paramChecked)
        {
            return GetSelecteIds(gv, paramChecked, "chkSelected");
        }
        #endregion

        #region public static string[] GetSelecteIds(GridView gv, bool paramChecked, string paramControl) 获取表格行代码数组
        /// <summary>
        /// 获取表格行代码数组
        /// </summary>
        /// <param name="gv">表格</param>
        /// <param name="paramChecked">选中状态</param>
        /// <param name="paramControl">控件名称</param>
        /// <returns></returns>
        public static string[] GetSelecteIds(GridView gv, bool paramChecked, string paramControl)
        {
            return GetSelecteIds(gv, paramChecked, paramControl, string.Empty);
        }
        #endregion

        #region public static string[] GetSelecteIds(GridView gv, bool paramChecked, string paramControl, string key)
        /// <summary>
        /// 获得已选的表格行代码数组
        /// </summary>
        /// <param name="gv">表格</param>
        /// <param name="paramChecked">选中状态</param>
        /// <param name="paramControl">控件名称</param>
        /// <param name="key">关键字</param>
        /// <returns>代码数组</returns>
        public static string[] GetSelecteIds(GridView gv, bool paramChecked, string paramControl, string key)
        {
            var ids = new string[0];
            var idList = string.Empty;
            for (var i = 0; i < gv.Rows.Count; i++)
            {
                // 得到选中的ID
                if (gv.Rows[i].RowType == DataControlRowType.DataRow)
                {
                    var tableCell = gv.Rows[i].Cells[0];
                    var checkBox = (CheckBox)tableCell.FindControl(paramControl);
                    if (checkBox != null)
                    {
                        if (checkBox.Checked == paramChecked)
                        {
                            // 把选中的ID保存到字符串
                            var id = string.Empty;
                            if (string.IsNullOrEmpty(key))
                            {
                                id = gv.DataKeys[gv.Rows[i].RowIndex].Value.ToString();
                            }
                            else
                            {
                                id = gv.DataKeys[gv.Rows[i].RowIndex].Values[key].ToString();
                            }

                            if (id.Length > 0)
                            {
                                idList += id + ",";
                            }
                        }
                    }
                }
            }
            // 切分ID
            if (idList.Length > 1)
            {
                idList = idList.Substring(0, idList.Length - 1);
                ids = idList.Split(',').Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
            }
            return ids;
        }
        #endregion
#endif
        //
        // 表格选择记录功能部分 repeater
        //
#if NET40_OR_GREATER
        #region public static string[] GetSelecteIds(Repeater repeater) 获得已选的表格行代码数组
        /// <summary>
        /// 获得已选的表格行代码数组
        /// </summary>
        /// <param name="repeater">表格</param>
        /// <returns>代码数组</returns>
        public static string[] GetSelecteIds(Repeater repeater)
        {
            return GetSelecteIds(repeater, true);
        }
        #endregion

        #region public static string[] GetUnSelecteIds(Repeater repeater) 获得未选的表格行代码数组
        /// <summary>
        /// 获得未选的表格行代码数组
        /// </summary>
        /// <param name="repeater">表格</param>
        /// <returns>代码数组</returns>
        public static string[] GetUnSelecteIds(Repeater repeater)
        {
            return GetSelecteIds(repeater, false);
        }
        #endregion

        #region public static string[] GetSelecteIds(Repeater repeater, bool isChecked)
        /// <summary>
        /// 获得表格行代码数组
        /// </summary>
        /// <param name="repeater">表格</param>
        /// <param name="isChecked">选中状态</param>
        /// <returns>代码数组</returns>
        public static string[] GetSelecteIds(Repeater repeater, bool isChecked)
        {
            return GetSelecteIds(repeater, isChecked, "chkSelected");
        }
        #endregion

        #region public static string[] GetSelecteIds(Repeater repeater, bool isChecked, string control) 获取表格行代码数组
        /// <summary>
        /// 获取表格行代码数组
        /// </summary>
        /// <param name="repeater">表格</param>
        /// <param name="isChecked">选中状态</param>
        /// <param name="control">控件名称</param>
        /// <returns></returns>
        public static string[] GetSelecteIds(Repeater repeater, bool isChecked, string control)
        {
            return GetSelecteIds(repeater, isChecked, control, string.Empty);
        }
        #endregion

        #region public static string[] GetSelecteIds(Repeater repeater, bool isChecked, string control, string key)
        /// <summary>
        /// 获得已选的表格行代码数组
        /// </summary>
        /// <param name="repeater">表格</param>
        /// <param name="isChecked">选中状态</param>
        /// <param name="control">控件名称</param>
        /// <param name="key">主键</param>
        /// <returns>代码数组</returns>
        public static string[] GetSelecteIds(Repeater repeater, bool isChecked, string control, string key)
        {
            var ids = new string[0];
            var idList = string.Empty;
            var id = string.Empty;
            for (var i = 0; i < repeater.Items.Count; i++)
            {
                // 得到选中的ID
                var checkBox = (CheckBox)repeater.Items[i].FindControl(control);
                if (checkBox.Checked == isChecked)
                {
                    // 把选中的ID保存到字符串
                    if (string.IsNullOrEmpty(key))
                    {
                        id = ((HiddenField)repeater.Items[i].FindControl(key)).Value;
                    }
                    else
                    {
                        id = ((HiddenField)repeater.Items[i].FindControl("hidId")).Value;
                    }
                    if (id.Length > 0)
                    {
                        idList += id + ",";
                    }
                }
            }
            // 切分ID
            if (idList.Length > 1)
            {
                idList = idList.Substring(0, idList.Length - 1);
                ids = idList.Split(',').Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
            }
            return ids;
        }
        #endregion
#endif
        //
        // 表格选择记录功能部分 DataGrid
        //
#if NET40_OR_GREATER
        #region public static string[] GetSelecteIds(DataGrid dataGrid) 获得已选的表格行代码数组
        /// <summary>
        /// 获得已选的表格行代码数组
        /// </summary>
        /// <param name="dataGrid">表格</param>
        /// <returns>代码数组</returns>
        public static string[] GetSelecteIds(DataGrid dataGrid)
        {
            return GetSelecteIds(dataGrid, true);
        }
        #endregion

        #region public static string[] GetUnSelecteIds(DataGrid dataGrid) 获得未选的表格行代码数组
        /// <summary>
        /// 获得未选的表格行代码数组
        /// </summary>
        /// <param name="dataGrid">表格</param>
        /// <returns>代码数组</returns>
        public static string[] GetUnSelecteIds(DataGrid dataGrid)
        {
            return GetSelecteIds(dataGrid, false);
        }
        #endregion

        #region public static string[] GetSelecteIds(DataGrid dataGrid, bool paramChecked)
        /// <summary>
        /// 获得表格行代码数组
        /// </summary>
        /// <param name="dataGrid">表格</param>
        /// <param name="paramChecked">选中状态</param>
        /// <returns>代码数组</returns>
        public static string[] GetSelecteIds(DataGrid dataGrid, bool paramChecked)
        {
            return GetSelecteIds(dataGrid, paramChecked, "chkSelected");
        }
        #endregion

        #region public static string[] GetSelecteIds(DataGrid dataGrid, bool paramChecked, string paramControl)
        /// <summary>
        /// 获得已选的表格行代码数组
        /// </summary>
        /// <param name="dataGrid">表格</param>
        /// <param name="paramChecked">选中状态</param>
        /// <param name="paramControl">控件名称</param>
        /// <returns>代码数组</returns>
        public static string[] GetSelecteIds(DataGrid dataGrid, bool paramChecked, string paramControl)
        {
            var paramIDs = new string[0];
            var ds = string.Empty;
            for (var i = 0; i < dataGrid.Items.Count; i++)
            {
                // 得到选中的ID
                var myTableCell = dataGrid.Items[i].Cells[0];
                var myCheckBox = (CheckBox)myTableCell.FindControl(paramControl);
                if (myCheckBox != null)
                {
                    if (myCheckBox.Checked == paramChecked)
                    {
                        // 把选中的ID保存到字符串
                        var id = dataGrid.DataKeys[dataGrid.Items[i].ItemIndex].ToString();
                        if (id.Length > 0)
                        {
                            ds += id + ",";
                        }
                    }
                }
            }
            // 切分ID
            if (ds.Length > 1)
            {
                ds = ds.Substring(0, ds.Length - 1);
                paramIDs = ds.Split(',').Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
            }
            return paramIDs;
        }
        #endregion
#endif
        //
        // 获取图标地址
        //

        #region public static string GetFileIcon(string fileName) 获取图标地址
        /// <summary>
        /// 获取图标地址
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>图标地址</returns>
        public static string GetFileIcon(string fileName)
        {
            // 这里是默认的图标
            var imageUrl = "Themes/Default/Images/Download.gif";
            // 截取后缀名,GetExtension读出来的后缀带"."的
            var extension = Path.GetExtension(fileName).ToLower().Substring(1);
            // 这里查找是否有指定的图标
            if (File.Exists(HttpContext.Current.Server.MapPath("~/") + "Themes/Default/Images/" + extension + ".png"))
            {
                // 获取图标地址
                imageUrl = "Themes/Default/Images/" + extension + ".png";
            }
            return imageUrl;
        }
        #endregion

        #region public static bool CheckLAN()
        /// <summary>
        /// 当前电脑是否在局域网络里
        /// </summary>
        /// <returns></returns>
        public static bool CheckLan()
        {
            var ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if ((ipAddress.Substring(0, 3) == "127") || (ipAddress.Substring(0, 3) == "192") || (ipAddress.Substring(0, 3) == "10."))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region public static void CloseWindow(bool refreshOpener = null)
        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="refreshOpener">是否刷新父窗体</param>
        public static void CloseWindow(bool refreshOpener = false)
        {
            HttpContext.Current.Response.Write("<script language=\"JavaScript\">" + Environment.NewLine);
            if (refreshOpener)
            {
                // window.opener != null 这个错误的调用方法
                HttpContext.Current.Response.Write("if(window.opener && !window.opener.closed){" + Environment.NewLine);
                HttpContext.Current.Response.Write("window.opener.location.href=window.opener.location.href;" + Environment.NewLine);
                HttpContext.Current.Response.Write("}" + Environment.NewLine);
            }
            HttpContext.Current.Response.Write("window.opener=null;" + Environment.NewLine);
            HttpContext.Current.Response.Write("window.open('','_self');" + Environment.NewLine);
            HttpContext.Current.Response.Write("window.close();");
            HttpContext.Current.Response.Write("</script>" + Environment.NewLine);
        }
        #endregion

        #region public static string GetItemName(string itemsTableName,string itemValue) 获取选项名称
        /// <summary>
        /// 将选项值转换为名称
        /// </summary>
        /// <param name="itemsTableName"></param>
        /// <param name="itemValue"></param>
        /// <returns></returns>
        public static string GetItemName(string itemsTableName, string itemValue)
        {
            if (string.IsNullOrEmpty(itemValue))
                return null;
            return new BaseItemDetailsManager(itemsTableName).GetProperty(new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldItemValue, itemValue), BaseItemDetailsEntity.FieldItemName);
        }
        #endregion
    }
}