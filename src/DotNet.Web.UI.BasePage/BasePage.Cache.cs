//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Caching;
using DotNet.Model;
using DotNet.Business;
using DotNet.Util;
using System.Data;


/// <remarks>
/// BasePage
/// 基础网页类
/// 
/// 修改记录
/// 
///	版本：1.0 2016.10.26    Troy.Cui    整理代码。
///	
/// 版本：1.0
/// <author>  
///		<name>Troy.Cui</name>
///		<date>2016.10.26</date>
/// </author> 
/// </remarks>
public partial class BasePage : System.Web.UI.Page
{
    #region protected void ListServerCache() 清除服务器Runtime缓存
    /// <summary>
    /// 清除服务器Runtime缓存
    /// </summary>
    protected DataTable ListServerCache()
    {
        var dt = new DataTable();
        dt.Columns.Add("CacheKey", typeof(string));
        dt.Columns.Add("CacheSize", typeof(long));

        var enumerator = HttpRuntime.Cache.GetEnumerator();
        GC.Collect();
        GC.WaitForFullGCComplete();
        long start = GC.GetTotalMemory(true);
        //循环读取
        while (enumerator.MoveNext())
        {
            var row = dt.NewRow();
            if (enumerator.Key != null)
            {
                row["CacheKey"] = enumerator.Key.ToString();
                GC.Collect();
                GC.WaitForFullGCComplete();
                long end = GC.GetTotalMemory(true);
                long size = end - start;
                start = end;
                row["CacheSize"] = size;
            }

            dt.Rows.Add(row);
        }
        return dt;
    }
    #endregion

    #region protected void ClearServerCache() 清除服务器Runtime缓存
    /// <summary>
    /// 清除服务器Runtime缓存
    /// </summary>
    public void ClearServerCache()
    {
        lock (BaseSystemInfo.UserLock)
        {
            var keys = new List<string>();
            // retrieve application Cache enumerator 
            var enumerator = HttpRuntime.Cache.GetEnumerator();
            //循环读取
            while (enumerator.MoveNext())
            {
                if (enumerator.Key != null) keys.Add(enumerator.Key.ToString());
            }
            //循环删除
            foreach (var t in keys)
            {
                HttpRuntime.Cache.Remove(t);
            }
        }
    }
    #endregion

    #region 清空客户端页面缓存

    /// <summary>
    /// 清空客户端页面缓存
    /// </summary>
    public void ClearClientPageCache()
    {
        HttpContext.Current.Response.Buffer = true;
        HttpContext.Current.Response.Expires = 0;
        HttpContext.Current.Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
        HttpContext.Current.Response.AddHeader("pragma", "no-cache");
        HttpContext.Current.Response.AddHeader("cache-control", "private");
        HttpContext.Current.Response.CacheControl = "no-cache";
    }

    #endregion
}