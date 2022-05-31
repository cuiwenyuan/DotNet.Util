using System;
using System.Text;
using System.Web.UI;
using System.Web;

namespace DotNet.Util
{
    /// <summary>
    /// Web显示消息提示对话框，替代js的写法
    /// </summary>
    public partial class Msg
    {
        #region public static void LayerAlert(string message, bool fail = false, bool close = false) Layer显示消息，不需要确认

        /// <summary>
        /// Layer弹出消息
        /// </summary>
        /// <param name="message">弹出信息</param>
        /// <param name="fail">是否失败提示</param>
        /// <param name="close">关闭父窗口</param>
        /// <param name="closeTime">自动关闭时间（毫秒，默认是3秒即3000）</param>
        public static void LayerAlert(string message, bool fail = false, bool close = false, int closeTime = 3000)
        {
            var page = (Page)System.Web.HttpContext.Current.Handler;
            var jqueryUrl = "../../../js/jquery-1.11.2.min.js";
            if (!page.ClientScript.IsClientScriptBlockRegistered("jquery"))
            {
                page.ClientScript.RegisterClientScriptInclude("jquery", jqueryUrl);
            }
            var layerUrl = "../../../js/layer/layer.js";
            if (!page.ClientScript.IsClientScriptBlockRegistered("layer"))
            {
                page.ClientScript.RegisterClientScriptInclude("layer", layerUrl);
            }
            if (fail)
            {
                message = "<script>layer.ready(function () {layer.msg('" + message + "', {icon: 2, shift: 2} );});</script>";
            }
            else
            {
                if (close)
                {
                    message = "<script>layer.ready(function () {layer.msg('" + message + "', {icon: 1, shift: 2, time: " + closeTime + "}, function(){ parent.layer.close(parent.layer.getFrameIndex(window.name)); });});</script>";
                }
                else
                {
                    message = "<script>layer.ready(function () {layer.msg('" + message + "', {icon: 1, shift: 2, time: " + closeTime + "} );});</script>";
                }
            }
            //page.ClientScript.RegisterStartupScript(page.GetType(), "message", message);
            //Troy.Cui提交后不再等待页面执行完毕再弹出提示 2018-11-27
            //RegisterClientScriptBlock(key, script) 在 form开始处（紧接 ＜form runat="server"＞ 标识之后）发送脚本块
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), "message", message);
        }
        #endregion

        #region public static void LayerAlert(string message,string url) Layer显示消息，不需要确认，并跳转到url

        /// <summary>
        /// Layer弹出消息，并跳转到url
        /// </summary>
        /// <param name="message"></param>
        /// <param name="url"></param>
        /// <param name="fail">是否失败</param>
        /// <param name="closeTime">自动关闭时间（毫秒，默认是3秒即3000）</param>
        public static void LayerAlert(string message, string url, bool fail = false, int closeTime = 3000)
        {
            var page = (Page)System.Web.HttpContext.Current.Handler;
            var jqueryUrl = "../../../js/jquery-1.11.2.min.js";
            if (!page.ClientScript.IsClientScriptBlockRegistered("jquery"))
            {
                page.ClientScript.RegisterClientScriptInclude("jquery", jqueryUrl);
            }
            var layerUrl = "../../../js/layer/layer.js";
            if (!page.ClientScript.IsClientScriptBlockRegistered("layer"))
            {
                page.ClientScript.RegisterClientScriptInclude("layer", layerUrl);
            }
            if (fail)
            {
                //message = "</script><script>layer.ready(function () {layer.msg('" + message + "', {icon: 2, shift: 2}, function(){location.href='" + url + "';} );});</script>";
                //先跳转
                message = "</script><script>layer.ready(function () {parent.layer.msg('" + message + "', {icon: 2, shift: 2, time: " + closeTime + "});location.href='" + url + "';});</script>";
            }
            else
            {
                //message = "<script>layer.ready(function () {layer.msg('" + message + "', {icon: 1, shift: 2}, function(){location.href='" + url + "';} );});</script>";
                //先跳转
                message = "<script>layer.ready(function () {parent.layer.msg('" + message + "', {icon: 1, shift: 2, time: " + closeTime + "});location.href='" + url + "';});</script>";
            }
            //page.ClientScript.RegisterStartupScript(page.GetType(), "message", message);
            //Troy.Cui提交后不再等待页面执行完毕再弹出提示 2018-11-27
            //RegisterClientScriptBlock(key, script) 在 form开始处（紧接 ＜form runat="server"＞ 标识之后）发送脚本块
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), "message", message);
        }
        #endregion

        #region public static void ShowAlert(string message) 显示消息，不需要确认
        /// <summary>
        /// 弹出消息
        /// </summary>
        /// <param name="message"></param>
        public static void ShowAlert(string message)
        {
            var page = (Page)System.Web.HttpContext.Current.Handler;
            message = "<script>alert('" + message + "');</script>";
            page.ClientScript.RegisterStartupScript(page.GetType(), "message", message);

        }
        #endregion

        #region public static void ShowAlert(string message,string url) 显示消息，不需要确认，并跳转到url
        /// <summary>
        /// 弹出消息，并跳转到url
        /// </summary>
        /// <param name="message"></param>
        /// <param name="url"></param>
        public static void ShowAlert(string message, string url)
        {
            var page = (Page)System.Web.HttpContext.Current.Handler;
            message = "<script>alert('" + message + "');location='" + url + "';</script>";
            page.ClientScript.RegisterStartupScript(page.GetType(), "message", message);
        }
        #endregion

        #region public static void ShowAlert(string message, string url, bool isRedirect) 弹出消息，自定义是否跳转到url
        /// <summary>
        /// 弹出消息，自定义是否跳转到url
        /// </summary>
        /// <param name="message"></param>
        /// <param name="url"></param>
        /// <param name="isRedirect"></param>
        public static void ShowAlert(string message, string url, bool isRedirect)
        {
            if (isRedirect)
                ShowAlert(message, url);
            else
                ShowAlert(message);
        }
        #endregion

        #region public static void ShowConfirmAlert(string message, string confirmurl,bool closeWindow=false) 弹出确认消息，跳转到url
        /// <summary>
        /// 弹出确认消息，跳转到url
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="confirmUrl">确认Url</param>
        /// <param name="closeWindow">关闭窗口</param>
        public static void ShowConfirmAlert(string message, string confirmUrl, bool closeWindow = false)
        {
            var page = (Page)System.Web.HttpContext.Current.Handler;
            message = "<script>if( confirm('" + message + "') ) {document.location.href='" + confirmUrl +
                      "'; } else { window.history.back(); }";

            if (closeWindow)
                message += " window.close();";
            message += "</script>";
            page.ClientScript.RegisterStartupScript(page.GetType(), "message", message);
        }
        #endregion

        #region public static void ShowConfirmAlert(string message, string confirmurl, string cancelurl,bool closeWindow=false)弹出确认消息，根据用户选择跳转到不同的url
        /// <summary>
        /// 弹出确认消息，根据用户选择跳转到不同的url
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="confirmurl">确认Url</param>
        /// <param name="cancelUrl">取消Url</param>
        /// <param name="closeWindow">关闭窗口</param>
        public static void ShowConfirmAlert(string message, string confirmurl, string cancelUrl, bool closeWindow = false)
        {
            var page = (Page)System.Web.HttpContext.Current.Handler;
            message = "<script>if( confirm('" + message + "') ) {document.location.href='" + confirmurl +
                      "'; } else { document.location.href='" + cancelUrl + "' }";
            if (closeWindow)
                message += " window.close();";
            message += "</script>";
            page.ClientScript.RegisterStartupScript(page.GetType(), "message", message);
        }
        #endregion

        #region public static void Redirect(string url) url跳转
        /// <summary>
        /// url跳转
        /// </summary>
        /// <param name="url"></param>
        public static void Redirect(string url)
        {
            var page = (Page)System.Web.HttpContext.Current.Handler;
            if (string.IsNullOrEmpty(url))
                page.ClientScript.RegisterStartupScript(page.GetType(), "message", "跳转地址不能为空。");
            else
                page.ClientScript.RegisterStartupScript(page.GetType(), "message", "<script>location='" + url + "';</script>");
        }
        #endregion

        #region public static void Alert(string message) 弹出小窗口
        /// <summary>
        /// 弹出小窗口
        /// </summary>
        /// <param name="message">窗口信息</param>
        public static void Alert(string message)
        {
            // message = StringUtil.DeleteUnVisibleChar(message);
            var js = @"<script>
                    alert('" + message + "');</script>";

            //2008年4月26日15:46:38 Modify by zxy
            //现在使用Page来注册脚本，如果使用 Response.Write来输出脚本，该脚本会出现在页面的顶部，破坏页面的布局。
            var page = System.Web.HttpContext.Current.Handler as Page;
            if (page != null)
            {
                var key = "Alert" + Guid.NewGuid();
                if (!page.ClientScript.IsClientScriptBlockRegistered(key))
                    page.ClientScript.RegisterClientScriptBlock(typeof(Page), key, js);
            }
            else
                HttpContext.Current.Response.Write(js);
        }
        #endregion

        #region public static void AlertAndCloseWindow(string message, bool refreshOpener = false) 弹出一个消息随后关闭当前窗口

        /// <summary>
        /// 弹出一个消息随后关闭当前窗口
        /// </summary>
        /// <param name="message">窗口信息</param>
        /// <param name="refreshOpener">刷新父页面</param>
        public static void AlertAndCloseWindow(string message, bool refreshOpener = false)
        {
            // message = StringUtil.DeleteUnVisibleChar(message);
            var js = string.Empty;
            js = "<script>" + System.Environment.NewLine;
            if (refreshOpener)
            {
                // window.opener != null 这个错误的调用方法
                js += "if(window.opener && !window.opener.closed){" + System.Environment.NewLine;
                js += "window.opener.location.href=window.opener.location.href;" + System.Environment.NewLine;
                js += "}" + System.Environment.NewLine;
            }
            js += "if(window.opener){" + System.Environment.NewLine;
            js += "window.opener=null;" + System.Environment.NewLine;
            js += "window.open('','_self');" + System.Environment.NewLine;
            js += "}" + System.Environment.NewLine;
            js += "alert('" + message + "');" + System.Environment.NewLine;
            js += "window.close();";
            js += "</script>" + System.Environment.NewLine;

            var page = System.Web.HttpContext.Current.Handler as Page;
            if (page != null)
            {
                var key = "CloseWindow" + page.ClientID;
                if (!page.ClientScript.IsClientScriptBlockRegistered(key))
                {
                    page.ClientScript.RegisterClientScriptBlock(typeof(Page), key, js);
                }
            }
            else
            {
                HttpContext.Current.Response.Write(js);
            }
        }
        #endregion

        #region public static void AlertAndRedirect(string message, string toURL) 弹出小窗口,并转到指定的页面
        /// <summary>
        /// 弹出小窗口,并转到指定的页面
        /// </summary>
        /// <param name="message"></param>
        /// <param name="toUrl"></param>
        public static void AlertAndRedirect(string message, string toUrl)
        {
            var js = "<script>alert('{0}');window.location.replace('{1}')</script>";
            HttpContext.Current.Response.Write(string.Format(js, message, toUrl));
        }
        #endregion

        #region public static void JsAlert(string str)
        /// <summary>
        /// JS提示并返回前页
        /// </summary>
        /// <param name="str"></param>
        public static void JsAlert(string str)
        {
            HttpContext.Current.Response.Write("<script>alert('" + str + "');history.go(-1);</script>");
            HttpContext.Current.Response.End();
        }
        #endregion

        #region public static void JsAlert(string str, string url)
        /// <summary>
        /// JS提示并跳转到指定页面
        /// </summary>
        /// <param name="str"></param>
        /// <param name="url"></param>
        public static void JsAlert(string str, string url)
        {
            HttpContext.Current.Response.Write("<script>alert('" + str + "');window.location.href='" + url + "';</script>");
            HttpContext.Current.Response.End();
        }
        #endregion

    }
}
