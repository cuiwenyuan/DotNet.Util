//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace DotNet.Business
{
    using IService;
    using Model;
    using Util;

    /// <summary>
    /// PermissionService
    /// 权限判断服务
    /// 
    /// 修改记录
    /// 
    ///		2011.09.26 版本：3.2 JiRiGaLa 整理一些范围函数名。
    ///		2011.06.04 版本：3.1 JiRiGaLa 整理日志功能。
    ///		2009.09.25 版本：3.0 JiRiGaLa Resource.ManagePermission 自动判断增加。
    ///		2008.12.12 版本：2.0 JiRiGaLa 进行了彻底的改进。
    ///		2008.05.30 版本：1.0 JiRiGaLa 创建权限判断服务。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2011.06.04</date>
    /// </author> 
    /// </summary>


    public partial class PermissionService : IPermissionService
    {
        #region public void ClearCache(string systemCode) 清除缓存
        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="systemCode">子系统编号</param>
        public void ClearCache(string systemCode)
        {
            lock (BaseSystemInfo.UserLock)
            {
                var tableName = systemCode + "Module";
                CacheUtil.Remove(tableName);
            }
        }
        #endregion

        #region 用户权限判断相关(需要实现对外调用)

        #region public bool IsInRole(BaseUserInfo userInfo, string userId, string roleName)
        /// <summary>
        /// 用户是否在指定的角色里
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="roleName">角色名称</param>
        /// <returns>在角色里</returns>
        public bool IsInRole(BaseUserInfo userInfo, string userId, string roleName)
        {
            var result = false;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 先获得角色主键
                var tableName = userInfo.SystemCode + "Role";
                var roleManager = new BaseRoleManager(dbHelper, userInfo, tableName);
                var roleCode = roleManager.GetProperty(new KeyValuePair<string, object>(BaseRoleEntity.FieldRealName, roleName), BaseRoleEntity.FieldCode);
                // 判断用户的默认角色
                if (!string.IsNullOrEmpty(roleCode))
                {
                    var userManager = new BaseUserManager(dbHelper, userInfo);
                    result = userManager.IsInRoleByCode(userId, roleCode);
                }
            });
            return result;
        }
        #endregion

        #region public bool IsAuthorized(BaseUserInfo userInfo, string permissionCode, string permissionName = null)
        /// <summary>
        /// 当前用户是否有相应的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="permissionName">权限名称</param>
        /// <returns>是否有权限</returns>
        public bool IsAuthorized(BaseUserInfo userInfo, string permissionCode, string permissionName = null)
        {
            return IsAuthorized(userInfo, userInfo.Id, permissionCode, permissionName);
        }
        #endregion

        #region public bool IsAuthorized(BaseUserInfo userInfo, string userId, string permissionCode, string permissionName = null)
        /// <summary>
        /// 某个用户是否有相应的操作权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="permissionName">权限名称</param>
        /// <returns>是否有权限</returns>
        public bool IsAuthorized(BaseUserInfo userInfo, string userId, string permissionCode, string permissionName = null)
        {
            var result = false;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                if (string.IsNullOrEmpty(userId))
                {
                    userId = userInfo.Id;
                }
#if (!DEBUG)
                // 是超级管理员,就不用继续判断权限了
                // var userManager = new BaseUserManager(result);
                // result = userManager.IsAdministrator(userId);
#endif
                if (!result)
                {
                    var permissionManager = new BasePermissionManager(userInfo);
                    result = permissionManager.IsAuthorized(userInfo.SystemCode, userId, permissionCode, permissionName);
                    // BaseLogManager.Instance.Add(result, this.serviceName, AppMessage.PermissionService_IsAuthorized, MethodBase.GetCurrentMethod());
                }
            });

            return result;
        }
        #endregion

        #region public bool CheckPermissionByRole(BaseUserInfo userInfo, string roleId, string permissionCode)
        /// <summary>
        /// 某个角色是否有相应的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>是否有权限</returns>
        public bool CheckPermissionByRole(BaseUserInfo userInfo, string roleId, string permissionCode)
        {
            var result = false;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 是超级管理员,就不用继续判断权限了
                result = roleId.Equals("Administrators");
                if (!result)
                {
                    var manager = new BasePermissionManager(dbHelper, userInfo);
                    result = manager.CheckPermissionByRole(userInfo.SystemCode, roleId, permissionCode);
                }
            });

            return result;
        }
        #endregion

        #region public bool IsAdministrator(BaseUserInfo userInfo)
        /// <summary>
        /// 当前用户是否超级管理员
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <returns>是超级管理员</returns>
        public bool IsAdministrator(BaseUserInfo userInfo)
        {
            return IsAdministratorByUser(userInfo, userInfo.Id);
        }
        #endregion

        #region public bool IsAdministratorByUser(BaseUserInfo userInfo, string userId)
        /// <summary>
        /// 某个用户是否超级管理员
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="userId"></param>
        /// <returns>是超级管理员</returns>
        public bool IsAdministratorByUser(BaseUserInfo userInfo, string userId)
        {
            var result = false;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                result = BaseUserManager.IsAdministrator(userId);
            });

            return result;
        }
        #endregion

        #region public string[] GetPermissionIds(BaseUserInfo userInfo, string systemCode, string userId, bool fromCache)
        /// <summary>
        /// 获得某个用户的所有权限列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="fromCache">是否从缓存读取</param>
        /// <returns>主键数组</returns>
        public string[] GetPermissionIds(BaseUserInfo userInfo, string systemCode, string userId, bool fromCache)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var permissionManager = new BasePermissionManager(dbHelper, userInfo);
                result = permissionManager.GetPermissionIdsByUser(systemCode, userId, string.Empty, true, fromCache);
            });

            return result;
        }
        #endregion

        #region public List<BaseModuleEntity> GetPermissionList(BaseUserInfo userInfo)
        /// <summary>
        /// 获得当前用户的所有权限列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="fromCache">是否从缓存读取</param>
        /// <returns>数据表</returns>
        public List<BaseModuleEntity> GetPermissionList(BaseUserInfo userInfo, bool fromCache)
        {
            return GetPermissionListByUser(userInfo, userInfo.SystemCode, userInfo.Id, userInfo.CompanyId, fromCache);
        }
        #endregion

        #region public List<BaseModuleEntity> GetPermissionList(BaseUserInfo userInfo, string userId)
        /// <summary>
        /// 获得当前用户的所有权限列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <returns>数据表</returns>
        public List<BaseModuleEntity> GetPermissionList(BaseUserInfo userInfo, string userId)
        {
            return GetPermissionListByUser(userInfo, userInfo.SystemCode, userId, userInfo.CompanyId, false);
        }
        #endregion

        /// <summary>
        /// 根据用户获取权限清单
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="userId"></param>
        /// <param name="fromCache"></param>
        /// <returns></returns>
        public List<BaseModuleEntity> GetPermissionListByUser(BaseUserInfo userInfo, string userId, bool fromCache)
        {
            return GetPermissionListByUser(userInfo, userInfo.SystemCode, userId, string.Empty, fromCache);
        }

        #region public List<BaseModuleEntity> GetPermissionListByUser(BaseUserInfo userInfo, string systemCode, string userId, string companyId, bool fromCache)
        /// <summary>
        /// 获得某个用户的所有权限列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="fromCache">是否从缓存读取</param>
        /// <returns>数据表</returns>
        public List<BaseModuleEntity> GetPermissionListByUser(BaseUserInfo userInfo, string systemCode, string userId, string companyId, bool fromCache)
        {
            var result = new List<BaseModuleEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var permissionManager = new BasePermissionManager(dbHelper, userInfo);
                result = permissionManager.GetPermissionListByUser(systemCode, userId, companyId, fromCache);
            });

            return result;
        }
        #endregion

        #region public bool IsModuleAuthorized(BaseUserInfo userInfo, string moduleCode)
        /// <summary>
        /// 当前用户是否对某个模块有相应的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="moduleCode">模块编号</param>
        /// <returns>是否有权限</returns>
        public bool IsModuleAuthorized(BaseUserInfo userInfo, string moduleCode)
        {
            return IsModuleAuthorizedByUser(userInfo, userInfo.Id, moduleCode);
        }
        #endregion

        #region public bool IsModuleAuthorizedByUser(BaseUserInfo userInfo, string userId, string moduleCode)
        /// <summary>
        /// 某个用户是否对某个模块有相应的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="moduleCode">模块编号</param>
        /// <returns>是否有权限</returns>
        public bool IsModuleAuthorizedByUser(BaseUserInfo userInfo, string userId, string moduleCode)
        {
            var result = false;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 是否超级管理员
                // 是超级管理员,就不用继续判断权限了
                var userEntity = BaseUserManager.GetObjectByCache(userId);
                if (userEntity != null && !string.IsNullOrEmpty(userEntity.Id))
                {
                    result = BaseUserManager.IsAdministrator(userId);
                    if (!result)
                    {
                        var tableName = userInfo.SystemCode + "Module";
                        var moduleManager = new BaseModuleManager(dbHelper, userInfo, tableName);
                        List<BaseModuleEntity> entityList = null;
                        // moduleManager.GetListByUser(userId);
                        // 这里需要改进，只读到第一个就可以返回了，没必要全部列表都计算一边
                        var count = entityList.Count(entity => !string.IsNullOrEmpty(entity.Code)
                             && (entity.Code.Equals(moduleCode, StringComparison.OrdinalIgnoreCase) || moduleCode.StartsWith(entity.Code)));
                        // int count = result.Count(entity => !string.IsNullOrEmpty(entity.Code)
                        //     && (entity.Code.Equals(moduleCode, StringComparison.OrdinalIgnoreCase)));
                        result = count > 0;
                    }
                }
            });

            return result;
        }
        #endregion

        #region public bool IsUrlAuthorizedByUser(BaseUserInfo userInfo, string userId, string moduleUrl);
        /// <summary>
        /// 某个用户是否对某个模块Url有访问权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="moduleUrl">模块Url</param>
        /// <returns>是否有权限</returns>
        public bool IsUrlAuthorizedByUser(BaseUserInfo userInfo, string userId, string moduleUrl)
        {
            var result = false;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 是否超级管理员
                // 是超级管理员,就不用继续判断权限了
                var userEntity = BaseUserManager.GetObjectByCache(userId);
                if (userEntity != null && !string.IsNullOrEmpty(userEntity.Id))
                {
                    result = BaseUserManager.IsAdministrator(userId);
                    if (!result)
                    {
                        var tableName = userInfo.SystemCode + "Module";
                        var moduleManager = new BaseModuleManager(dbHelper, userInfo, tableName);
                        List<BaseModuleEntity> entityList = null;
                        // moduleManager.GetList(userId);
                        // 这里需要改进，只读到第一个就可以返回了，没必要全部列表都计算一边
                        var count = entityList.Count(entity => !string.IsNullOrEmpty(entity.NavigateUrl)
                            && (entity.NavigateUrl.Equals(moduleUrl, StringComparison.OrdinalIgnoreCase) || moduleUrl.StartsWith(entity.NavigateUrl)));
                        result = count > 0;
                    }
                }
            });

            return result;
        }
        #endregion

        #region public bool IsModuleAuthorizedByUser(BaseUserInfo userInfo, string userId, string moduleCode, string permissionCode)
        /// <summary>
        /// 某个用户是否对某个模块有相应的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="moduleCode">模块编号</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>是否有权限</returns>
        public bool IsModuleAuthorizedByUser(BaseUserInfo userInfo, string userId, string moduleCode, string permissionCode)
        {
            var result = false;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 是超级管理员,就不用继续判断权限了
                result = BaseUserManager.IsAdministrator(userId);
                if (!result)
                {
                    var permissionScopeManager = new BasePermissionScopeManager(dbHelper);
                    result = permissionScopeManager.IsModuleAuthorized(userInfo.SystemCode, userId, moduleCode, permissionCode);
                }
            });
            return result;
        }
        #endregion

        #region public PermissionOrganizeScope GetUserPermissionScope(BaseUserInfo userInfo, string userId, string permissionCode)
        /// <summary>
        /// 获得用户的数据权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">数据权限编号</param>
        /// <returns>数据权限范围</returns>
        public PermissionOrganizeScope GetUserPermissionScope(BaseUserInfo userInfo, string userId, string permissionCode)
        {
            var result = PermissionOrganizeScope.NotAllowed;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 是超级管理员,就不用继续判断权限了
                var permissionScopeManager = new BasePermissionScopeManager(dbHelper, userInfo);
                result = permissionScopeManager.GetUserPermissionScope(userInfo.SystemCode, userId, permissionCode);
            });
            return result;
        }
        #endregion


        /// <summary>
        /// 获取权限审核
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionId">权限主键</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <returns>数据表</returns>
        public DataTable PermissionMonitor(BaseUserInfo userInfo, DateTime startDate, DateTime endDate, string companyId, string userId, string permissionId, out int recordCount, int pageIndex = 0, int pageSize = 20)
        {
            DataTable result = null;
            recordCount = 0;
            var myRecordCount = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var condition = string.Empty;
                var dbParameters = new List<KeyValuePair<string, object>>();

                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " AND ";
                }
                condition += BasePermissionEntity.FieldCreateTime + " >= " + DbHelper.GetParameter(BaseSystemInfo.UserCenterDbType, "startDate");
                dbParameters.Add(new KeyValuePair<string, object>("startDate", startDate));

                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " AND ";
                }
                condition += BasePermissionEntity.FieldCreateTime + " <= " + DbHelper.GetParameter(BaseSystemInfo.UserCenterDbType, "endDate");
                dbParameters.Add(new KeyValuePair<string, object>("endDate", endDate));

                var tableName = BasePermissionEntity.TableName;
                if (userInfo != null)
                {
                    tableName = userInfo.SystemCode + "Permission";
                }

                myRecordCount = DbUtil.GetCount(dbHelper, tableName, condition, dbHelper.MakeParameters(dbParameters));
                result = DbUtil.GetDataTableByPage(dbHelper, tableName, "*", pageIndex, pageSize, condition, dbHelper.MakeParameters(dbParameters), BasePermissionEntity.FieldCreateTime + " DESC");

                if (!result.Columns.Contains("ResourceCategoryName"))
                {
                    result.Columns.Add("ResourceCategoryName".ToUpper());
                }
                if (!result.Columns.Contains("PermissionName"))
                {
                    result.Columns.Add("PermissionName".ToUpper());
                }
                if (!result.Columns.Contains("PermissionCode"))
                {
                    result.Columns.Add("PermissionCode".ToUpper());
                }
                if (!result.Columns.Contains("ResourceName"))
                {
                    result.Columns.Add("ResourceName".ToUpper());
                }
                if (!result.Columns.Contains("CompanyName"))
                {
                    result.Columns.Add("CompanyName".ToUpper());
                }

                foreach (DataRow dr in result.Rows)
                {
                    var id = dr["PermissionId"].ToString();
                    var moduleEntity = BaseModuleManager.GetObjectByCache(userInfo, id);
                    if (moduleEntity != null)
                    {
                        dr["PermissionName"] = moduleEntity.FullName;
                        dr["PermissionCode"] = moduleEntity.Code;
                    }
                    if (dr["ResourceCategory"].ToString().Equals(BaseUserEntity.TableName))
                    {
                        id = dr["ResourceId"].ToString();
                        var userEntity = BaseUserManager.GetObjectByCache(id);
                        if (userEntity != null)
                        {
                            dr["ResourceName"] = userEntity.RealName;
                            dr["CompanyName"] = userEntity.CompanyName;
                            dr["ResourceCategoryName"] = "用户";
                        }
                    }
                    else if (dr["ResourceCategory"].ToString().Equals(BaseOrganizeEntity.TableName))
                    {
                        id = dr["ResourceId"].ToString();
                        var organizeEntity = BaseOrganizeManager.GetObjectByCache(id);
                        if (organizeEntity != null)
                        {
                            dr["ResourceName"] = organizeEntity.FullName;
                            dr["ResourceCategoryName"] = "网点";
                        }
                    }
                    else if (dr["ResourceCategory"].ToString().Equals(BaseRoleEntity.TableName))
                    {
                        id = dr["ResourceId"].ToString();
                        var roleEntity = BaseRoleManager.GetObjectByCache(userInfo, id);
                        if (roleEntity != null)
                        {
                            dr["ResourceName"] = roleEntity.RealName;
                            dr["ResourceCategoryName"] = "角色";
                        }
                    }
                }

            });

            recordCount = myRecordCount;
            return result;
        }

        #endregion
    }
}