//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
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
    ///		2012.03.22 版本：1.0 JiRiGaLa 创建权限判断服务。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.03.22</date>
    /// </author> 
    /// </summary>
    public partial class PermissionService : IPermissionService
    {
        #region 用户权限范围判断相关(需要实现对外调用)

        #region public DataTable GetOrganizeDTByPermission(BaseUserInfo userInfo, string userId, string permissionCode, bool childrens = true)
        /// <summary>
        /// 按某个权限域获取组织列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">数据权限编号</param>
        /// <param name="childrens">获取子节点</param>
        /// <returns>数据表</returns>
        public DataTable GetOrganizeDTByPermission(BaseUserInfo userInfo, string userId, string permissionCode = "Resource.ManagePermission", bool childrens = true)
        {
            var dt = new DataTable(BaseOrganizeEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 若权限是空的，直接返回所有数据
                if (string.IsNullOrEmpty(permissionCode))
                {
                    var organizeManager = new BaseOrganizeManager(dbHelper, userInfo);
                    dt = organizeManager.GetDataTable();
                    dt.DefaultView.Sort = BaseOrganizeEntity.FieldSortCode;
                }
                else
                {
                    // 获得组织机构列表
                    var permissionScopeManager = new BasePermissionScopeManager(dbHelper, userInfo);
                    dt = permissionScopeManager.GetOrganizeDt(userInfo.SystemCode, userInfo.Id, permissionCode, childrens);
                    dt.DefaultView.Sort = BaseOrganizeEntity.FieldSortCode;
                }
                dt.TableName = BaseOrganizeEntity.TableName;
            });
            return dt;
        }
        #endregion

        #region public string[] GetOrganizeIdsByPermissionScope(BaseUserInfo userInfo, string userId, string permissionCode)
        /// <summary>
        /// 按某个数据权限获取组织主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">数据权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetOrganizeIdsByPermissionScope(BaseUserInfo userInfo, string userId, string permissionCode)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 若权限是空的，直接返回所有数据
                if (!string.IsNullOrEmpty(permissionCode))
                {
                    // 获得组织机构列表
                    var permissionScopeManager = new BasePermissionScopeManager(dbHelper, userInfo);
                    result = permissionScopeManager.GetOrganizeIds(userId, permissionCode);
                }
            });
            return result;
        }
        #endregion

        /// <summary>
        /// 按某个权限域获取角色列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">数据权限编号</param>
        /// <returns>数据表</returns>
        public List<BaseRoleEntity> GetRoleListByPermission(BaseUserInfo userInfo, string systemCode, string userId, string permissionCode)
        {
            var result = new List<BaseRoleEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得角色列表
                var permissionScopeManager = new BasePermissionScopeManager(dbHelper, userInfo);
                result = permissionScopeManager.GetRoleList(systemCode, userId, permissionCode, true);
            });

            return result;
        }


        /// <summary>
        /// 按某个权限域获取角色列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">数据权限编号</param>
        /// <returns>数据表</returns>
        public DataTable GetRoleDTByPermission(BaseUserInfo userInfo, string systemCode, string userId, string permissionCode)
        {
            var result = new DataTable(BaseRoleEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得角色列表
                var permissionScopeManager = new BasePermissionScopeManager(dbHelper, userInfo);
                result = permissionScopeManager.GetRoleDt(systemCode, userInfo.Id, permissionCode, true);
                result.TableName = BaseRoleEntity.TableName;
            });

            return result;
        }

        #region public string[] GetRoleIdsByPermissionScope(BaseUserInfo userInfo, string userId, string permissionCode)
        /// <summary>
        /// 按某个数据权限获取角色主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">数据权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetRoleIdsByPermissionScope(BaseUserInfo userInfo, string userId, string permissionCode)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 若权限是空的，直接返回所有数据
                if (!string.IsNullOrEmpty(permissionCode))
                {
                    // 获得角色主键数组
                    var manager = new BasePermissionScopeManager(dbHelper, userInfo);
                    result = manager.GetRoleIds(userInfo.SystemCode, userId, permissionCode);
                }
            });

            return result;
        }
        #endregion

        #region public List<BaseUserEntity> GetUserListByPermission(BaseUserInfo userInfo, string userId, string permissionCode)
        /// <summary>
        /// 按某个权限域获取用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">数据权限编号</param>
        /// <returns>数据表</returns>
        public List<BaseUserEntity> GetUserListByPermission(BaseUserInfo userInfo, string userId, string permissionCode)
        {
            var entityList = new List<BaseUserEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得组织机构列表
                var manager = new BasePermissionScopeManager(dbHelper, userInfo);
                // 若权限是空的，直接返回所有数据
                if (!string.IsNullOrEmpty(permissionCode))
                {
                    entityList = manager.GetUserList(userInfo.SystemCode, userInfo.Id, permissionCode);
                }
            });
            return entityList;
        }
        #endregion

        /// <summary>
        /// GetUserDTByPermission
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="userId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public DataTable GetUserDTByPermission(BaseUserInfo userInfo, string userId, string permissionCode)
        {
            var dt = new DataTable(BaseUserEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得组织机构列表
                var manager = new BasePermissionScopeManager(dbHelper, userInfo);
                // 若权限是空的，直接返回所有数据
                if (!string.IsNullOrEmpty(permissionCode))
                {
                    dt = manager.GetUserDataTable(userInfo.SystemCode, userInfo.Id, permissionCode);
                    dt.TableName = BaseUserEntity.TableName;
                }
            });
            return dt;
        }

        #region public string[] GetUserIdsByPermissionScope(BaseUserInfo userInfo, string userId, string permissionCode)
        /// <summary>
        /// 按某个数据权限获取用户主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">数据权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetUserIdsByPermissionScope(BaseUserInfo userInfo, string userId, string permissionCode)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 若权限是空的，直接返回所有数据
                if (!string.IsNullOrEmpty(permissionCode))
                {
                    // 获得用户主键数组
                    var manager = new BasePermissionScopeManager(dbHelper, userInfo);
                    result = manager.GetUserIds(userInfo.SystemCode, userId, permissionCode);
                }
            });

            return result;
        }
        #endregion

        #region public DataTable GetModuleDTByPermission(BaseUserInfo userInfo, string userId, string permissionCode)
        /// <summary>
        /// 按某个权限域获取模块列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限域编号</param>
        /// <returns>数据表</returns>
        public DataTable GetModuleDTByPermission(BaseUserInfo userInfo, string userId, string permissionCode)
        {
            var dt = new DataTable(BaseModuleEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseModuleManager(dbHelper, userInfo);
                dt = manager.GetDataTableByPermission(userInfo.SystemCode, userId, permissionCode);
                dt.TableName = BaseModuleEntity.TableName;
            });
            return dt;
        }
        #endregion

        #region public DataTable GetPermissionDTByPermission(BaseUserInfo userInfo, string userId, string permissionCode)
        /// <summary>
        /// 用户的所有可授权范围(有授权权限的权限列表)
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限域编号</param>
        /// <returns>数据表</returns>
        public DataTable GetPermissionDTByPermission(BaseUserInfo userInfo, string userId, string permissionCode)
        {
            var dt = new DataTable(BaseModuleEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var permissionId = BaseModuleManager.GetIdByCodeByCache(userInfo.SystemCode, permissionCode);
                // 数据库里没有设置可授权的权限项，系统自动增加一个权限配置项
                if (string.IsNullOrEmpty(permissionId) && permissionCode.Equals("Resource.ManagePermission"))
                {
                    var permissionEntity = new BaseModuleEntity
                    {
                        Code = "Resource.ManagePermission",
                        FullName = "资源管理范围权限（系统默认）",
                        IsScope = 1,
                        Enabled = 1,
                        AllowDelete = 0
                    };
                    permissionEntity.AllowDelete = 0;
                    new BaseModuleManager(userInfo).AddEntity(permissionEntity);
                }
                dt = new BaseModuleManager().GetDataTableByUser(userInfo.SystemCode, userId, permissionCode);
                dt.TableName = BaseModuleEntity.TableName;
            });
            return dt;
        }
        #endregion

        /// <summary>
        /// GetStaffDataTableByPermissionScope
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="userId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public DataTable GetStaffDataTableByPermissionScope(BaseUserInfo userInfo, string userId, string permissionCode)
        {
            var dt = new DataTable(BaseUserEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得组织机构列表
                var manager = new BasePermissionScopeManager(dbHelper, userInfo);
                // 若权限是空的，直接返回所有数据
                if (!string.IsNullOrEmpty(permissionCode))
                {
                    // 被管理部门的列表
                    var organizeIds = manager.GetOrganizeIds(userInfo.SystemCode, userId, permissionCode, false);
                    var staffManager = new BaseStaffManager(dbHelper, userInfo);
                    dt = staffManager.GetDataTableByOrganizes(organizeIds);
                    dt.TableName = BaseStaffEntity.TableName;
                }
            });
            return dt;
        }

        #endregion
    }
}