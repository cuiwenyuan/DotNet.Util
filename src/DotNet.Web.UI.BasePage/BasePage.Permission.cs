//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
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
    #region 常用操作权限项定义

    /// <summary>
    /// 访问权限
    /// </summary>
    protected bool PermissionAccess = true;

    /// <summary>
    /// 新增权限
    /// </summary>
    protected bool PermissionAdd = false;

    /// <summary>
    /// 新建权限
    /// </summary>
    protected bool PermissionNew = false;

    /// <summary>
    /// 创建权限
    /// </summary>
    protected bool PermissionCreate = false;

    /// <summary>
    /// 编辑权限
    /// </summary>
    protected bool PermissionEdit = false;

    /// <summary>
    /// 更新权限
    /// </summary>
    protected bool PermissionUpdate = false;

    /// <summary>
    /// 删除权限
    /// </summary>
    protected bool PermissionDelete = false;

    /// <summary>
    /// 移除权限
    /// </summary>
    protected bool PermissionRemove = false;

    /// <summary>
    /// 撤销删除权限
    /// </summary>
    protected bool PermissionUndoDelete = false;

    /// <summary>
    /// 撤销移除权限
    /// </summary>
    protected bool PermissionUndoRemove = false;

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

    /// <summary>
    /// 导入权限
    /// </summary>
    protected bool PermissionImport = false;

    /// <summary>
    /// 打印权限
    /// </summary>
    protected bool PermissionPrint = true;

    /// <summary>
    /// 启用权限
    /// </summary>
    protected bool PermissionEnable = false;

    /// <summary>
    /// 禁用权限
    /// </summary>
    protected bool PermissionDisable = false;

    /// <summary>
    /// 上传权限
    /// </summary>
    protected bool PermissionUpload = false;

    /// <summary>
    /// 下载权限
    /// </summary>
    protected bool PermissionDownload = false;

    /// <summary>
    /// 锁定权限
    /// </summary>
    protected bool PermissionLock = false;

    /// <summary>
    /// 解锁权限
    /// </summary>
    protected bool PermissionUnlock = false;

    /// <summary>
    /// 显示权限
    /// </summary>
    protected bool PermissionShow = false;

    /// <summary>
    /// 查看权限
    /// </summary>
    protected bool PermissionView = false;

    /// <summary>
    /// 显示权限
    /// </summary>
    protected bool PermissionDisplay = false;

    /// <summary>
    /// 列表权限
    /// </summary>
    protected bool PermissionList = true;

    /// <summary>
    /// 审核权限
    /// </summary>
    protected bool PermissionAudit = false;

    /// <summary>
    /// 撤销审核权限
    /// </summary>
    protected bool PermissionUndoAudit = false;

    /// <summary>
    /// 取消权限
    /// </summary>
    protected bool PermissionCancel = false;

    /// <summary>
    /// 撤销取消权限
    /// </summary>
    protected bool PermissionUndoCancel = false;

    /// <summary>
    /// 调整权限
    /// </summary>
    protected bool PermissionAdjust = false;

    /// <summary>
    /// 转移权限
    /// </summary>
    protected bool PermissionTransfer = false;

    /// <summary>
    /// 分配权限
    /// </summary>
    protected bool PermissionAssign = false;

    /// <summary>
    /// 读取权限
    /// </summary>
    protected bool PermissionRead = false;

    /// <summary>
    /// 写入权限
    /// </summary>
    protected bool PermissionWrite = false;

    /// <summary>
    /// 分享权限
    /// </summary>
    protected bool PermissionShare = false;

    /// <summary>
    /// 转发权限
    /// </summary>
    protected bool PermissionForward = false;

    /// <summary>
    /// 发送权限
    /// </summary>
    protected bool PermissionSend = false;

    /// <summary>
    /// 激活权限
    /// </summary>
    protected bool PermissionActivate = false;

    /// <summary>
    /// 安装权限
    /// </summary>
    protected bool PermissionInstall = false;

    /// <summary>
    /// 卸载权限
    /// </summary>
    protected bool PermissionUninstall = false;

    /// <summary>
    /// 复制
    /// </summary>
    protected bool PermissionCopy = false;

    #endregion

    #region 操作权限
    /// <summary>
    /// 获取操作的权限
    /// </summary>
    /// <param name="module">模块</param>
    public void GetActionPermission(string module)
    {
        PermissionAccess = IsAuthorized(module + ".Access");
        PermissionAdd = IsAuthorized(module + ".Add");
        PermissionNew = IsAuthorized(module + ".New");
        PermissionCreate = IsAuthorized(module + ".Create");
        PermissionEdit = IsAuthorized(module + ".Edit");
        PermissionUpdate = IsAuthorized(module + ".Update");
        PermissionAdmin = IsAuthorized(module + ".Admin");
        PermissionList = IsAuthorized(module + ".List");
        PermissionShow = IsAuthorized(module + ".Show");
        PermissionView = IsAuthorized(module + ".View");
        PermissionShow = IsAuthorized(module + ".Display");
        PermissionAudit = IsAuthorized(module + ".Audit");
        PermissionUndoAudit = IsAuthorized(module + ".UndoAudit");
        PermissionDelete = IsAuthorized(module + ".Delete");
        PermissionRemove = IsAuthorized(module + ".Remove");
        PermissionUndoDelete = IsAuthorized(module + ".UndoDelete");
        PermissionUndoRemove = IsAuthorized(module + ".UndoRemove");
        PermissionCancel = IsAuthorized(module + ".Cancel");
        PermissionUndoCancel = IsAuthorized(module + ".UndoCancel");
        PermissionLock = IsAuthorized(module + ".Lock");
        PermissionUnlock = IsAuthorized(module + ".Unlock");
        PermissionEnable = IsAuthorized(module + ".Enable");
        PermissionDisable = IsAuthorized(module + ".Disable");
        PermissionSearch = IsAuthorized(module + ".Search");
        PermissionUpload = IsAuthorized(module + ".Upload");
        PermissionDownload = IsAuthorized(module + ".Download");
        PermissionExport = IsAuthorized(module + ".Export");
        PermissionImport = IsAuthorized(module + ".Import");
        PermissionPrint = IsAuthorized(module + ".Print");
        PermissionTransfer = IsAuthorized(module + ".Transfer");
        PermissionAssign = IsAuthorized(module + ".Assign");
        PermissionAdjust = IsAuthorized(module + ".Adjust");
        PermissionRead = IsAuthorized(module + ".Read");
        PermissionWrite = IsAuthorized(module + ".Write");
        PermissionShare = IsAuthorized(module + ".Share");
        PermissionForward = IsAuthorized(module + ".Forward");
        PermissionSend = IsAuthorized(module + ".Send");
        PermissionActivate = IsAuthorized(module + ".Activate");
        PermissionInstall = IsAuthorized(module + ".Install");
        PermissionUninstall = IsAuthorized(module + ".Uninstall");
        PermissionCopy = IsAuthorized(module + ".Copy");
    }

    #endregion

    #region 用户是否拥有某个角色（按编码）
    /// <summary>
    /// 用户是否拥有某个角色（按编码）
    /// </summary>
    /// <param name="roleCode">角色编码</param>
    /// <returns>是否拥有指定角色</returns>
    public bool IsUserInRole(string roleCode)
    {
        var result = false;
        if (UserInfo != null)
        {
            result = new BaseUserManager(UserInfo).IsInRoleByCode(UserInfo.Id, roleCode);
        }
        return result;
    }
    #endregion

    #region 用户是否拥有包含指定关键字的角色编码
    /// <summary>
    /// 用户是否拥有包含指定关键字的角色编码
    /// </summary>
    /// <param name="searchKey">关键字</param>
    /// <returns>是否拥有指定关键字的角色</returns>
    public bool IsUserHasRoleContains(string searchKey)
    {
        var userManager = new BaseUserManager(UserInfo);
        return userManager.IsHasRoleCodeContains(UserInfo.Id.ToString(), searchKey);
    }
    #endregion

    #region 用户是否拥有以指定关键字开始的角色编码
    /// <summary>
    /// 用户是否拥有以指定关键字开始的角色编码
    /// </summary>
    /// <param name="searchKey">关键字</param>
    /// <returns>是否拥有指定关键字的角色</returns>
    public bool IsUserHasRoleStartWith(string searchKey)
    {
        var userManager = new BaseUserManager(UserInfo);
        return userManager.IsHasRoleCodeStartWith(UserInfo.Id.ToString(), searchKey);
    }
    #endregion

    // 用户操作权限常用判断函数

    #region public void Authorized(string permissionItemCode, string accessDenyUrl = null) 是否有相应权限，同时若没权限会重新定位到某个页面
    /// <summary>
    /// 是否有相应权限，同时若没权限会重新定位到某个页面
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
                HttpContext.Current.Response.Redirect(WebUtil.AccessDenyPage + "?PermissionItemCode=" + permissionItemCode);
            }
        }
    }
    #endregion

    #region public bool IsAuthorized(string permissionItemCode, string permissionItemName = null) 是否有相应的权限

    /// <summary>
    /// 是否有相应的权限
    /// </summary>
    /// <param name="permissionCode">权限编码</param>
    /// <param name="userId">用户编号</param>
    /// <returns>是否有权限</returns>
    public bool IsAuthorized(string permissionCode, string userId = null)
    {
        if (UserInfo != null && UserInfo.IsAdministrator)
        {
            return true;
        }
        if (UserInfo != null && string.IsNullOrEmpty(userId))
        {
            userId = UserInfo.Id.ToString();
        }
        return WebUtil.GetUserPermissionList(UserInfo, userId)?.Count(entity => !string.IsNullOrEmpty(entity.Code) && entity.Code.Equals(permissionCode, StringComparison.OrdinalIgnoreCase)) > 0;
    }
    #endregion

    #region public void UrlAuthorized(string url, string accessDenyUrl = null) 是否有相应的模块权限，同时若没权限会重新定位到某个页面
    /// <summary>
    /// 是否有相应的模块权限，同时若没权限会重新定位到某个页面
    /// </summary>
    /// <param name="url">模块地址</param>
    /// <param name="accessDenyUrl">访问被阻止的url</param>
    public void UrlAuthorized(string url, string accessDenyUrl = null)
    {
        // 若没有相应的权限，那就跳转到没有权限的页面里
        if (!WebUtil.UserIsLogon() || !IsUrlAuthorized(url))
        {
            if (!string.IsNullOrEmpty(accessDenyUrl))
            {
                HttpContext.Current.Response.Redirect(accessDenyUrl);
            }
            else
            {
                HttpContext.Current.Response.Redirect(WebUtil.AccessDenyPage + "?URL=" + url);
            }
        }
    }
    #endregion

    #region public bool IsUrlAuthorized(string moduleUrl, string userId = null) 是否有相应的模块权限

    /// <summary>
    /// 是否有相应的模块权限
    /// </summary>
    /// <param name="moduleUrl"></param>
    /// <param name="userId"></param>
    /// <returns>是否有权限</returns>
    public bool IsUrlAuthorized(string moduleUrl, string userId = null)
    {
        if (UserInfo.IsAdministrator)
        {
            return true;
        }
        if (string.IsNullOrEmpty(userId))
        {
            userId = UserInfo.Id.ToString();
        }

        return WebUtil.GetUserPermissionList(UserInfo, userId)?.Count(entity => !string.IsNullOrEmpty(entity.NavigateUrl) && (entity.NavigateUrl.Equals(moduleUrl, StringComparison.OrdinalIgnoreCase) || moduleUrl.StartsWith(entity.NavigateUrl))) > 0;
    }
    #endregion

    #region public void ModuleAuthorized(string moduleCode, string accessDenyUrl = null) 是否有相应的模块权限，同时若没权限会重新定位到某个页面
    /// <summary>
    /// 是否有相应的模块权限，同时若没权限会重新定位到某个页面
    /// </summary>
    /// <param name="moduleCode">模块编号</param>
    /// <param name="accessDenyUrl">访问被阻止的url</param>
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
                HttpContext.Current.Response.Redirect(WebUtil.AccessDenyPage + "?ModuleCode=" + moduleCode);
            }
        }
    }
    #endregion

    #region public bool IsModuleAuthorized(string moduleCode, string userId = null) 是否有相应的模块权限
    /// <summary>
    /// 是否有相应的模块权限
    /// </summary>
    /// <param name="moduleCode">模块编号</param>
    /// <param name="userId">判断谁的权限</param>
    /// <returns>是否有权限</returns>
    public bool IsModuleAuthorized(string moduleCode, string userId = null)
    {
        // 默认就当是判断本用户的权限吧，否则这个程序太复杂了
        if (UserInfo.IsAdministrator)
        {
            return true;
        }
        if (string.IsNullOrEmpty(userId))
        {
            userId = UserInfo.Id.ToString();
        }
        return WebUtil.GetUserPermissionList(UserInfo, userId)?.Count(entity => !string.IsNullOrEmpty(entity.Code) && entity.Code.Equals(moduleCode, StringComparison.OrdinalIgnoreCase)) > 0;
    }
    #endregion

    #region protected void ClearPermissionCache() 清除服务器用户权限缓存
    /// <summary>
    /// 清除服务器用户权限缓存
    /// </summary>
    protected void ClearPermissionCache()
    {
        lock (BaseSystemInfo.UserLock)
        {
            // 清除模块菜单权限
            var cacheKey = string.Empty;
            // 获取所有用户的主键数组
            var userManager = new BaseUserManager();
            var userIds = userManager.GetIds();
            foreach (var userId in userIds)
            {
                //清除Menu菜单缓存
                cacheKey = "M" + userId;
                if (HttpRuntime.Cache[cacheKey] != null)
                {
                    HttpRuntime.Cache.Remove(cacheKey);
                }
                // 清除Permission操作权限
                cacheKey = "P" + userId;
                if (HttpRuntime.Cache[cacheKey] != null)
                {
                    HttpRuntime.Cache.Remove(cacheKey);
                }
            }
            if (HttpRuntime.Cache[BaseModuleEntity.CurrentTableName] != null)
            {
                HttpRuntime.Cache.Remove(BaseModuleEntity.CurrentTableName);
            }
            if (HttpRuntime.Cache[BaseOrganizationEntity.CurrentTableName] != null)
            {
                HttpRuntime.Cache.Remove(BaseOrganizationEntity.CurrentTableName);
            }
            if (HttpRuntime.Cache[BaseRoleEntity.CurrentTableName] != null)
            {
                HttpRuntime.Cache.Remove(BaseRoleEntity.CurrentTableName);
            }
            if (HttpRuntime.Cache[BaseUserEntity.CurrentTableName] != null)
            {
                HttpRuntime.Cache.Remove(BaseUserEntity.CurrentTableName);
            }
            if (HttpRuntime.Cache[BaseStaffEntity.CurrentTableName] != null)
            {
                HttpRuntime.Cache.Remove(BaseStaffEntity.CurrentTableName);
            }
        }
    }
    #endregion


}