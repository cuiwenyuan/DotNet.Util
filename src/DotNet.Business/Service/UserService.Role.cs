//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace DotNet.Business
{
    using IService;
    using Model;
    using Util;

    /// <summary>
    /// UserService
    /// 用户管理服务
    /// 
    /// 修改记录
    /// 
    ///		2012.03.27 版本：1.0 JiRiGaLa 窗体与数据库连接的分离。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2012.03.27</date>
    /// </author> 
    /// </summary>
    public partial class UserService : IUserService
    {
        #region public bool UserIsInRole(BaseUserInfo userInfo, string userId, string roleCode)
        /// <summary>
        /// 用户是否在某个角色里的判断
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="roleCode">角色编号</param>
        /// <returns>存在</returns>
        public bool UserIsInRole(BaseUserInfo userInfo, string userId, string roleCode)
        {
            var result = false;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                result = userManager.IsInRoleByCode(userId, roleCode);
            });

            return result;
        }
        #endregion

        #region public int ApplyForJointRole(BaseUserInfo userInfo, string userId, string[] addRoleIds)
        /// <summary>
        /// 添加用户到某些角色里
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="addRoleIds">加入角色</param>
        /// <returns>影响的行数</returns>
        public int ApplyForJointRole(BaseUserInfo userInfo, string userId, string[] addRoleIds)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                // 小心异常，检查一下参数的有效性
                if (addRoleIds != null)
                {
                    result += userManager.AddToRole(userInfo.SystemCode, new string[] { userId }, addRoleIds, false);
                }
            });

            return result;
        }
        #endregion

        #region public int AddUserToRole(BaseUserInfo userInfo, string userId, string[] roleIds)
        /// <summary>
        /// 添加用户到某些角色里
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="roleIds">加入角色</param>
        /// <returns>影响的行数</returns>
        public int AddUserToRole(BaseUserInfo userInfo, string userId, string[] roleIds)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                // 小心异常，检查一下参数的有效性
                if (roleIds != null)
                {
                    result += userManager.AddToRole(userInfo.SystemCode, new string[] { userId }, roleIds);
                }
            });

            return result;
        }
        #endregion

        #region public int RemoveUserFromRole(BaseUserInfo userInfo, string userId, string[] roleIds)
        /// <summary>
        /// 将用户从某些角色中移除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="roleIds">角色主键数组</param>
        /// <returns>影响行数</returns>
        public int RemoveUserFromRole(BaseUserInfo userInfo, string userId, string[] roleIds)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                if (!string.IsNullOrWhiteSpace(userId) && roleIds != null && roleIds.Length > 0)
                {
                    result += userManager.RemoveFormRole(userInfo.SystemCode, new string[] { userId }, roleIds);
                }
            });

            return result;
        }
        #endregion

        /// <summary>
        /// 批量移出角色（按角色编号）
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleCode">角色编号</param>
        /// <returns>影响的行数</returns>
        public int RemoveUserFromRoleByCode(BaseUserInfo userInfo, string userId, string systemCode, string roleCode)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                if (string.IsNullOrWhiteSpace(systemCode))
                {
                    systemCode = "Base";
                }
                if (!string.IsNullOrWhiteSpace(roleCode))
                {
                    var roleId = string.Empty;
                    var tableName = systemCode + "Role";
                    var roleManager = new BaseRoleManager(userInfo, tableName);
                    roleId = roleManager.GetIdByCode(roleCode);
                    if (!string.IsNullOrWhiteSpace(roleId))
                    {
                        var userManager = new BaseUserManager(dbHelper, userInfo);
                        result += userManager.RemoveFormRole(systemCode, userId, roleId);
                    }
                }
            });
            return result;
        }

        #region public int ClearUserRole(BaseUserInfo userInfo, string userId)
        /// <summary>
        /// 清除用户归属的角色
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <returns>影响行数</returns>
        public int ClearUserRole(BaseUserInfo userInfo, string userId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                result = userManager.ClearRole(userInfo.SystemCode, userId);
            });

            return result;
        }
        #endregion

        #region public string[] GetUserRoleIds(BaseUserInfo userInfo, string userId)
        /// <summary>
        /// 获取用户的角色主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <returns>主键数组</returns>
        public string[] GetUserRoleIds(BaseUserInfo userInfo, string userId)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                result = userManager.GetRoleIds(userInfo.SystemCode, userId);
                // result = userRoleManager.GetRoleIds(userId);
            });

            return result;
        }
        #endregion

        #region public List<BaseRoleEntity> GetUserRoleList(BaseUserInfo userInfo, string systemCode, string userId)
        /// <summary>
        /// 获取用户的所有角色列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <returns>列表</returns>
        public List<BaseRoleEntity> GetUserRoleList(BaseUserInfo userInfo, string systemCode, string userId)
        {
            var result = new List<BaseRoleEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(userInfo);
                result = userManager.GetRoleList(systemCode, userId);
            });

            return result;
        }
        #endregion

        #region public DataTable GetUserRoleDataTable(BaseUserInfo userInfo, string systemCode, string userId)
        /// <summary>
        /// 获取用户的所有角色列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <returns>列表</returns>
        public DataTable GetUserRoleDataTable(BaseUserInfo userInfo, string systemCode, string userId)
        {
            var result = new DataTable(BaseRoleEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(userInfo);
                result = userManager.GetUserRoleDataTable(systemCode, userId);
                result.TableName = BaseRoleEntity.TableName;
            });

            return result;
        }
        #endregion

        #region public int SetUserRoles(BaseUserInfo userInfo, string userId, string[] roleIds)
        /// <summary>
        /// 批量设置用户的角色
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="roleIds">角色数组</param>
        /// <returns>影响的行数</returns>
        public int SetUserRoles(BaseUserInfo userInfo, string userId, string[] roleIds)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                // 小心异常，检查一下参数的有效性
                if (roleIds != null)
                {
                    userManager.ClearRole(userInfo.SystemCode, userId);
                    result += userManager.AddToRole(userInfo.SystemCode, new string[] { userId }, roleIds);
                }
            });

            return result;
        }
        #endregion

        #region public DataTable GetDataTableByRole(BaseUserInfo userInfo, string[] roleIds)
        /// <summary>
        /// 按角色获取用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleIds">角色主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByRole(BaseUserInfo userInfo, string[] roleIds)
        {
            var dt = new DataTable(BaseRoleEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                dt = userManager.GetDataTableByRole(userInfo.SystemCode, roleIds);
                dt.TableName = BaseUserEntity.TableName;
                dt.DefaultView.Sort = BaseUserEntity.FieldSortCode;
            });

            return dt;
        }
        #endregion

        #region public DataTable GetDataTableByCompanyByRole(BaseUserInfo userInfo, string systemCode, string companyId, string roleId)
        /// <summary>
        /// 按公司按角色获取用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByCompanyByRole(BaseUserInfo userInfo, string systemCode, string companyId, string roleId)
        {
            var result = new DataTable(BaseUserEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                result = userManager.GetDataTableByCompanyByRole(systemCode, companyId, roleId);
                result.TableName = BaseUserEntity.TableName;
                // dt.DefaultView.Sort = BaseUserEntity.FieldSortCode;
            });

            return result;
        }
        #endregion

        #region public List<BaseUserEntity> GetListByRole(BaseUserInfo userInfo, string[] roleIds)
        /// <summary>
        /// 按角色获取用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleIds">角色主键数组</param>
        /// <returns>数据权限</returns>
        public List<BaseUserEntity> GetListByRole(BaseUserInfo userInfo, string[] roleIds)
        {
            var entityList = new List<BaseUserEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                entityList = userManager.GetListByRole(userInfo.SystemCode, roleIds);
            });

            return entityList;
        }
        #endregion

        #region public DataTable GetRoleDataTable(BaseUserInfo userInfo)
        /// <summary>
        /// 获取用户的角色列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public DataTable GetRoleDataTable(BaseUserInfo userInfo)
        {
            var dt = new DataTable(BaseRoleEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0)
                };
                var roleManager = new BaseRoleManager(dbHelper, userInfo);
                // 获取有效的，未必删除的数据，按排序码排序
                dt = roleManager.GetDataTable(parameters, BaseRoleEntity.FieldSortCode);
                // 不是超级管理员，不能添加超级管理员
                if (!BaseUserManager.IsAdministrator(userInfo.Id))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr[BaseRoleEntity.FieldCode].ToString().Equals(DefaultRole.Administrators.ToString()))
                        {
                            dr.Delete();
                        }
                    }
                    dt.AcceptChanges();
                }
                dt.TableName = BaseUserEntity.TableName;
                dt.DefaultView.Sort = BaseUserEntity.FieldSortCode;
            });
            return dt;
        }
        #endregion

        // 当前的锁
        private static object _locker = new Object();

        /// <summary>
        /// 从某个角色列表中轮循获取一个用户，在线的用户优先。
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="roleCode">角色编号</param>
        /// <param name="userId">当前用户主键</param>
        /// <param name="cacheHours">缓存小时数</param>
        /// <returns>用户主键</returns>
        public string GetRandomUserId(IDbHelper dbHelper, string roleCode, string userId, int cacheHours = 4)
        {
            string result = null;
            var cacheTime = TimeSpan.FromMilliseconds(3600000 * cacheHours);
            // 检查缓存里，是否有这个用户的对应客服的主键？若没有进行分配
            if (!string.IsNullOrEmpty(userId))
            {
                result = CacheUtil.Cache(userId, () => new BaseUserManager(dbHelper).GetRandomUserId("Base", roleCode), true, false, cacheTime);
            }
            return result;
        }
    }
}