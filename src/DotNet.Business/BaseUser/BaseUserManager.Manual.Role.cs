﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserManager
    /// 用户管理-用户角色关系管理
    /// 
    /// 修改记录
    ///
    ///     2020.12.08 版本：1.5 Troy.Cui    使用CacheUtil缓存
    ///		2016.03.02 版本：2.1 JiRiGaLa	方法简化、能把去掉的方法全部去掉、这样调用的来源就好控制了。
    ///		2016.02.26 版本：2.0 JiRiGaLa	用户角色关系进行缓存优化。
    ///		2015.12.06 版本：1.1 JiRiGaLa	改进参数化、未绑定变量硬解析。
    ///		2015.11.17 版本：1.1 JiRiGaLa	谁操作的？哪个系统的？哪个用户是否在哪个角色里？进行改进。
    ///		2012.04.12 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.03.02</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        #region GetRoleIdsByCache(string systemCode, string userId, string companyId = null)
        /// <summary>
        /// 从缓存中获取角色编号
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户编号</param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static string[] GetRoleIdsByCache(string systemCode, string userId, string companyId = null)
        {
            // 返回值
            string[] result = null;

            if (!string.IsNullOrEmpty(userId))
            {
                //2017.12.20增加默认的HttpRuntime.Cache缓存
                var cacheKey = "Array" + systemCode + userId + companyId + "RoleIds";
                //var cacheTime = default(TimeSpan);
                var cacheTime = TimeSpan.FromMilliseconds(86400000);
                result = CacheUtil.Cache<string[]>(cacheKey, () =>
                {
                    //进行数据库查询
                    var userManager = new BaseUserManager();
                    return userManager.GetRoleIds(systemCode, userId, companyId);
                }, true, false, cacheTime);

                //// 进行数据库查询
                //BaseUserManager userManager = new BaseUserManager();
                //result = userManager.GetRoleIds(systemCode, userId, companyId);
            }

            return result;
        }
        #endregion

        #region IsInRoleByCache(string systemCode, string userId, string roleCode, string companyId = null) 用户是否在角色里
        /// <summary>
        /// 用户是否在角色里
        /// 2015-12-24 吉日嘎拉 提供高速缓存使用的方法
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="roleCode">角色编号</param>
        /// <param name="companyId">公司主键</param>
        /// <returns>在角色中</returns>
        public static bool IsInRoleByCache(string systemCode, string userId, string roleCode, string companyId = null)
        {
            // 返回值
            var result = false;

            if (!string.IsNullOrEmpty(userId))
            {
                // 从缓存里快速得到角色对应的主键盘
                var roleId = BaseRoleManager.GetIdByCodeByCache(systemCode, roleCode);
                if (!string.IsNullOrEmpty(roleId))
                {
                    var roleIds = GetRoleIdsByCache(systemCode, userId, companyId);
                    if (roleIds != null && roleIds.Length > 0)
                    {
                        result = (Array.IndexOf(roleIds, roleId) >= 0);
                    }
                }
            }

            return result;
        }
        #endregion

        #region IsInRole(string userId, string roleName) 用户是否在某个角色中

        /// <summary>
        /// 用户是否在某个角色中
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="roleName">角色</param>
        /// <returns>存在</returns>
        public bool IsInRole(string userId, string roleName)
        {
            var systemCode = "Base";
            if (UserInfo != null && !UserInfo.SystemCode.IsNullOrEmpty() && !UserInfo.SystemCode.IsNullOrWhiteSpace())
            {
                systemCode = UserInfo.SystemCode;
                if (!BaseSystemInfo.SystemCode.IsNullOrEmpty() && !BaseSystemInfo.SystemCode.Equals(systemCode))
                {
                    systemCode = BaseSystemInfo.SystemCode;
                }
            }

            return IsInRole(systemCode, userId, roleName);
        }

        #endregion

        #region IsInRole(string systemCode, string userId, string roleName)
        /// <summary>
        /// 2015-11-17 吉日嘎拉 改进判断函数，方便别人调用，弱化当前操作者、可以灵活控制哪个子系统的数据
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="roleName">角色名称</param>
        /// <returns>是否在角色中</returns>
        public bool IsInRole(string systemCode, string userId, string roleName)
        {
            var result = false;

            // 用户参数不合法
            if (string.IsNullOrEmpty(userId))
            {
                return result;
            }
            // 角色名称不合法
            if (string.IsNullOrEmpty(roleName))
            {
                return result;
            }
            // 传入的系统编号不合法，自动认为是基础系统
            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = "Base";
            }

            var roleId = BaseRoleManager.GetIdByNameByCache(systemCode, roleName);

            // 无法获取角色主键
            if (string.IsNullOrEmpty(roleId))
            {
                return false;
            }

            // 获取用户的所有角色主键列表
            var roleIds = GetRoleIds(systemCode, userId);
            result = StringUtil.Exists(roleIds, roleId);

            return result;
        }

        #endregion

        #region IsInRoleByCode(BaseUserInfo userInfo, string code) 用户是否在某个角色里
        /// <summary>
        /// 用户是否在某个角色里
        /// 包括所在公司的角色也进行判断
        /// </summary>
        /// <param name="userInfo">当前用户</param>
        /// <param name="code">编号</param>
        /// <returns>是否在角色里</returns>
        public bool IsInRoleByCode(BaseUserInfo userInfo, string code)
        {
            return IsInRoleByCode(userInfo.SystemCode, userInfo.Id.ToString(), code);
        }
        #endregion

        #region IsInRoleByCode(string userId, string code) 用户是否在某个角色中
        /// <summary>
        /// 用户是否在某个角色中
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="code">角色编号</param>
        /// <returns>存在</returns>
        public bool IsInRoleByCode(string userId, string code)
        {
            var systemCode = "Base";
            if (UserInfo != null && !UserInfo.SystemCode.IsNullOrEmpty() && !UserInfo.SystemCode.IsNullOrWhiteSpace())
            {
                systemCode = UserInfo.SystemCode;
                if (!BaseSystemInfo.SystemCode.IsNullOrEmpty() && !BaseSystemInfo.SystemCode.Equals(systemCode))
                {
                    systemCode = BaseSystemInfo.SystemCode;
                }
            }

            return IsInRoleByCode(systemCode, userId, code);
        }
        #endregion

        #region IsInRoleByCode(string systemCode, string userId, string roleCode) 用户是否在某个角色中
        /// <summary>
        /// 用户是否在某个角色中
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="roleCode">角色编号</param>
        /// <returns>存在</returns>
        public bool IsInRoleByCode(string systemCode, string userId, string roleCode)
        {
            var result = false;

            if (string.IsNullOrEmpty(systemCode))
            {
                return false;
            }

            if (!ValidateUtil.IsInt(userId))
            {
                return false;
            }

            if (string.IsNullOrEmpty(roleCode))
            {
                return false;
            }

            // 2016-01-07 吉日嘎拉 这里用缓存、效率会高
            var roleId = BaseRoleManager.GetIdByCodeByCache(systemCode, roleCode);
            if (string.IsNullOrEmpty(roleId))
            {
                return false;
            }

            var ls = GetUserRoleEntityList(systemCode);
            result = ls.Exists((t => t.UserId == userId.ToInt() && t.RoleId == roleId.ToInt()));

            return result;
        }
        #endregion

        #region GetUserRoleEntityList(string systemCode) 获取用户角色实体列表（有缓存）
        /// <summary>
        /// 获取用户角色实体列表
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <returns></returns>
        public List<BaseUserRoleEntity> GetUserRoleEntityList(string systemCode)
        {
            var userRoleTableName = GetUserRoleTableName(systemCode);
            //2017.12.19增加默认的HttpRuntime.Cache缓存
            var cacheKey = "List." + systemCode + "." + userRoleTableName;
            //var cacheTime = default(TimeSpan);
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            var result = CacheUtil.Cache<List<BaseUserRoleEntity>>(cacheKey, () =>
            {
                var parametersWhere = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserRoleEntity.FieldSystemCode, systemCode),
                    new KeyValuePair<string, object>(BaseUserRoleEntity.FieldDeleted, 0),
                    new KeyValuePair<string, object>(BaseUserRoleEntity.FieldEnabled, 1)
                };
                return new BaseUserRoleManager(DbHelper, UserInfo, userRoleTableName).GetList<BaseUserRoleEntity>(parametersWhere, BaseUserRoleEntity.FieldId);
            }, true, false, cacheTime);

            return result;
        }
        #endregion

        #region public bool IsHasRoleCodeContains(string userId, string searchKey) 用户是否拥有包含指定关键字的角色编码
        /// <summary>
        /// 用户是否拥有包含指定关键字的角色编码
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="searchKey">关键字</param>
        /// <param name="systemCode">子系统</param>
        /// <returns></returns>
        public bool IsHasRoleCodeContains(string userId, string searchKey)
        {
            var systemCode = "Base";
            if (UserInfo != null && !UserInfo.SystemCode.IsNullOrEmpty() && !UserInfo.SystemCode.IsNullOrWhiteSpace())
            {
                systemCode = UserInfo.SystemCode;
                if (!BaseSystemInfo.SystemCode.IsNullOrEmpty() && !BaseSystemInfo.SystemCode.Equals(systemCode))
                {
                    systemCode = BaseSystemInfo.SystemCode;
                }
            }

            return IsHasRoleCodeContains(systemCode, userId, searchKey);
        }

        #region public bool IsHasRoleCodeContains(string systemCode, string userId, string searchKey)

        /// <summary>
        /// 用户是否拥有包含指定关键字的角色编码
        /// </summary>
        /// <param name="systemCode">子系统编码</param>
        /// <param name="userId">用户编号</param>
        /// <param name="searchKey">关键字</param>
        /// <returns></returns>
        public bool IsHasRoleCodeContains(string systemCode, string userId, string searchKey)
        {
            var result = false;

            if (string.IsNullOrEmpty(systemCode))
            {
                return false;
            }

            if (!ValidateUtil.IsInt(userId))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(searchKey))
            {
                return false;
            }

            var dt = GetUserRole(systemCode);
            var strWhere = "UserId = " + userId + " AND Code LIKE '%" + searchKey + "%'";
            try
            {
                var drs = dt.Select(strWhere);
                //foreach (var dr in drs)
                //{
                //    result = true;
                //    break;
                //}
                if (drs.Length > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
            }

            return result;
        }
        #endregion

        #endregion

        #region public bool IsHasRoleCodeStartWith(string userId, string searchKey) 用户是否拥有以指定关键字开始的角色编码
        /// <summary>
        /// 用户是否拥有以指定关键字开始的角色编码
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="searchKey">关键字</param>
        /// <returns></returns>
        public bool IsHasRoleCodeStartWith(string userId, string searchKey)
        {
            var systemCode = "Base";
            if (UserInfo != null && !UserInfo.SystemCode.IsNullOrEmpty() && !UserInfo.SystemCode.IsNullOrWhiteSpace())
            {
                systemCode = UserInfo.SystemCode;
                if (!BaseSystemInfo.SystemCode.IsNullOrEmpty() && !BaseSystemInfo.SystemCode.Equals(systemCode))
                {
                    systemCode = BaseSystemInfo.SystemCode;
                }
            }

            return IsHasRoleCodeStartWith(systemCode, userId, searchKey);
        }
        /// <summary>
        /// 用户是否拥有以指定关键字开始的角色编码
        /// </summary>
        /// <param name="systemCode">子系统编码</param>
        /// <param name="userId">用户编号</param>
        /// <param name="searchKey">关键字</param>
        /// <returns></returns>
        public bool IsHasRoleCodeStartWith(string systemCode, string userId, string searchKey)
        {
            var result = false;

            if (string.IsNullOrEmpty(systemCode))
            {
                return false;
            }

            if (!ValidateUtil.IsInt(userId))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(searchKey))
            {
                return false;
            }

            var dt = GetUserRole(systemCode);
            var strWhere = "UserId = " + userId + " AND Code LIKE '" + searchKey + "%'";
            try
            {
                var drs = dt.Select(strWhere);
                //foreach (var dr in drs)
                //{
                //    result = true;
                //    break;
                //}
                if (drs.Length > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
            }

            return result;
        }
        #endregion

        #region public DataTable GetUserRole(string systemCode) 获取所有用户的角色列表

        /// <summary>
        /// 获取所有用户的角色列表
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <returns>角色数据表</returns>
        public DataTable GetUserRole(string systemCode)
        {
            var result = new DataTable(BaseRoleEntity.CurrentTableName);

            var userRoleTableName = BaseUserRoleEntity.CurrentTableName;
            if (!string.IsNullOrWhiteSpace(systemCode))
            {
                userRoleTableName = GetUserRoleTableName(systemCode);
            }
            var roleTableName = BaseRoleEntity.CurrentTableName;
            if (!string.IsNullOrWhiteSpace(systemCode))
            {
                roleTableName = GetRoleTableName(systemCode);
            }

            var sb = PoolUtil.StringBuilder.Get();
            sb.AppendLine("SELECT BaseRole.Code, BaseRole.Name, BaseRole.Description, UserRole.Id, UserRole.UserId, UserRole.RoleId, UserRole.Enabled, UserRole.Deleted, UserRole.CreateTime, UserRole.CreateBy, UserRole.UpdateTime, UserRole.UpdateBy");
            sb.AppendLine(" FROM BaseRole INNER JOIN (SELECT Id, UserId, RoleId, Enabled, Deleted, CreateTime, CreateBy, UpdateTime, UpdateBy FROM BaseUserRole WHERE " + BaseUtil.FieldEnabled + " = 1 AND " + BaseUserRoleEntity.FieldDeleted + " = 0) UserRole ON BaseRole.Id = UserRole.RoleId");
            sb.AppendLine(" WHERE BaseRole." + BaseUtil.FieldEnabled + " = 1 AND BaseRole." + BaseRoleEntity.FieldDeleted + " = 0 ORDER BY UserRole.CreateTime DESC");
            //替换表名
            sb = sb.Replace("BaseUserRole", userRoleTableName);
            sb = sb.Replace("BaseRole", roleTableName);

            var cacheKey = "Dt." + GetUserRoleTableName(systemCode);
            //var cacheTime = default(TimeSpan);
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            result = CacheUtil.Cache<DataTable>(cacheKey, () => Fill(sb.Return()), true, false, cacheTime);
            return result;
        }
        #endregion

        #region public string[] GetRoleIds(string userId) 获取用户的角色主键数组

        /// <summary>
        /// 获取用户的角色主键数组
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns>主键数组</returns>
        public string[] GetRoleIds(string userId)
        {
            return GetRoleIds(UserInfo.SystemCode, userId);
        }

        #endregion

        #region public string[] GetRoleIds(string systemCode, string userId, string companyId = null) 获取用户的角色主键数组
        /// <summary>
        /// 获取用户的角色主键数组
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="companyId">公司主键</param>
        /// <returns>角色主键数组</returns>
        public string[] GetRoleIds(string systemCode, string userId, string companyId = null)
        {
            var result = new List<string>();

            var userRoleTable = GetUserRoleTableName(systemCode);

            // 被删除的角色不应该显示出来
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + BaseUserRoleEntity.FieldRoleId);
            sb.Append(" FROM " + userRoleTable);
            sb.Append(" WHERE " + BaseUserRoleEntity.FieldUserId + " = " + DbHelper.GetParameter(BaseUserRoleEntity.FieldUserId));
            sb.Append(" AND " + BaseUserRoleEntity.FieldSystemCode + " = " + DbHelper.GetParameter(systemCode));
            sb.Append(" AND " + BaseUserRoleEntity.FieldEnabled + " = " + DbHelper.GetParameter(BaseUserRoleEntity.FieldEnabled));
            sb.Append(" AND " + BaseUserRoleEntity.FieldDeleted + " = " + DbHelper.GetParameter(BaseUserRoleEntity.FieldDeleted));

            var dbParameters = new List<IDbDataParameter>
            {
                DbHelper.MakeParameter(BaseUserRoleEntity.FieldUserId, userId),
                DbHelper.MakeParameter(BaseUserRoleEntity.FieldEnabled, 1),
                DbHelper.MakeParameter(BaseUserRoleEntity.FieldDeleted, 0)
            };

            var dataReader = DbHelper.ExecuteReader(sb.Return(), dbParameters.ToArray());
            if (dataReader != null && !dataReader.IsClosed)
            {
                while (dataReader.Read())
                {
                    result.Add(dataReader[BaseUserRoleEntity.FieldRoleId].ToString());
                }

                dataReader.Close();
            }

            return result.ToArray();
            // return BaseUtil.FieldToArray(result, BaseUserRoleEntity.FieldRoleId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
        }
        #endregion

        #region public List<BaseRoleEntity> GetRoleList(string systemCode, string userId, string companyId = null) 一个用户的所有的角色列表
        /// <summary>
        /// 一个用户的所有的角色列表
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="companyId">公司主键</param>
        /// <returns>角色列表</returns>
        public List<BaseRoleEntity> GetRoleList(string systemCode, string userId, string companyId = null)
        {
            var result = new List<BaseRoleEntity>();

            var roleIds = GetRoleIds(systemCode, userId, companyId);
            if (roleIds != null && roleIds.Length > 0)
            {
                var entities = BaseRoleManager.GetEntitiesByCache(systemCode);
                result = (entities as List<BaseRoleEntity>).Where(t => roleIds.Contains(t.Id.ToString()) && t.SystemCode == systemCode && t.Enabled == 1 && t.Deleted == 0).ToList();
            }

            return result;
        }
        #endregion

        #region public List<BaseUserEntity> GetListByRole(string systemCode, string roleCode, string companyId = null) 按角色编号获得用户列表
        /// <summary>
        /// 按角色编号获得用户列表
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleCode">角色编号</param>
        /// <param name="companyId"></param>
        /// <returns>主键数组</returns>
        public List<BaseUserEntity> GetListByRole(string systemCode, string roleCode, string companyId = null)
        {
            List<BaseUserEntity> result = null;

            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = "Base";
            }
            var roleId = BaseRoleManager.GetIdByCodeByCache(systemCode, roleCode);
            if (!string.IsNullOrEmpty(roleId))
            {
                result = GetListByRole(systemCode, new string[] { roleId }, companyId);
            }

            return result;
        }
        #endregion

        #region public List<BaseUserEntity> GetListByRole(string systemCode, string[] roleIds, string companyId = null) 根据角色获取清单

        /// <summary>
        /// 根据角色获取清单
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleIds"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public List<BaseUserEntity> GetListByRole(string systemCode, string[] roleIds, string companyId = null)
        {
            var result = new List<BaseUserEntity>();
            var dbParameters = new List<IDbDataParameter>();

            var userRoleTableName = GetUserRoleTableName(systemCode);
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + SelectFields
                     + " FROM " + BaseUserEntity.CurrentTableName
                     + " , (SELECT " + BaseUserRoleEntity.FieldUserId
                          + " FROM " + userRoleTableName
                          + " WHERE " + BaseUserRoleEntity.FieldRoleId + " IN (" + StringUtil.ArrayToList(roleIds) + ")"
                          + " AND " + BaseUserRoleEntity.FieldSystemCode + " = '" + systemCode + "'"
                          + " AND " + BaseUserRoleEntity.FieldEnabled + " = 1"
                          + " AND " + BaseUserRoleEntity.FieldDeleted + " = 0) B "
                          + " WHERE " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldId + " = B." + BaseUserRoleEntity.FieldUserId
                          + " AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldEnabled + " = 1"
                          + " AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDeleted + " = 0");

            if (!string.IsNullOrWhiteSpace(companyId))
            {
                sb.Append(" AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldCompanyId + " = " + DbHelper.GetParameter(BaseUserEntity.FieldCompanyId));
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldCompanyId, companyId));
            }

            using (var dr = DbHelper.ExecuteReader(sb.Return(), dbParameters.ToArray()))
            {
                result = GetList<BaseUserEntity>(dr);
            }

            return result;
        }

        #endregion

        #region public DataTable GetDataTableByRole(string systemCode, string[] roleIds, string companyId = null) 根据角色获取数据表

        /// <summary>
        /// 根据角色获取数据表
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleIds"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public DataTable GetDataTableByRole(string systemCode, string[] roleIds, string companyId = null)
        {
            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = "Base";
            }
            var userRoleTableName = GetUserRoleTableName(systemCode);
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + SelectFields + " FROM " + BaseUserEntity.CurrentTableName
                            + " WHERE " + BaseUserEntity.FieldEnabled + " = 1"
                            + " AND " + BaseUserEntity.FieldDeleted + " = 0"
                            + " AND ( " + BaseUserEntity.FieldId + " IN "
                            + " (SELECT  " + BaseUserRoleEntity.FieldUserId
                            + " FROM " + userRoleTableName
                            + " WHERE " + BaseUserRoleEntity.FieldRoleId + " IN (" + StringUtil.ArrayToList(roleIds) + ")"
                            + " AND " + BaseUserRoleEntity.FieldSystemCode + " = '" + systemCode + "'"
                            + " AND " + BaseUserRoleEntity.FieldEnabled + " = 1"
                            + " AND " + BaseUserRoleEntity.FieldDeleted + " = 0)) "
                            + " ORDER BY  " + BaseUserEntity.FieldSortCode);

            return DbHelper.Fill(sb.Return());
        }
        #endregion

        #region 清空

        #region public int ClearUser(string systemCode, string roleId) 清空用户
        /// <summary>
        /// 清空用户
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public int ClearUser(string systemCode, string roleId)
        {
            var result = 0;

            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = "Base";
            }
            var userRoleTableName = GetUserRoleTableName(systemCode);
            var manager = new BaseUserRoleManager(DbHelper, UserInfo, userRoleTableName);
            var parameters = new List<KeyValuePair<string, object>> {
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldSystemCode, systemCode),
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldRoleId, roleId),
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldDeleted, 0)
            };
            result += manager.Delete(parameters);

            return result;
        }

        #endregion

        #region public int ClearRole(string systemCode, string userId) 清空角色

        /// <summary>
        /// 清空角色
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int ClearRole(string systemCode, string userId)
        {
            var result = 0;

            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = "Base";
            }
            var userRoleTableName = GetUserRoleTableName(systemCode);
            var manager = new BaseUserRoleManager(DbHelper, UserInfo, userRoleTableName);
            var parameters = new List<KeyValuePair<string, object>> {
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldUserId, userId),
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldDeleted, 0)
            };
            result += manager.Delete(parameters);

            return result;
        }
        #endregion

        #endregion

        #region public DataTable GetUserRoleDataTable(string systemCode, string userId)

        /// <summary>
        /// 获取用户的角色列表
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <returns>角色数据表</returns>
        public DataTable GetUserRoleDataTable(string systemCode, string userId)
        {
            var result = new DataTable(BaseRoleEntity.CurrentTableName);

            var userRoleTableName = BaseUserRoleEntity.CurrentTableName;
            if (!string.IsNullOrWhiteSpace(systemCode))
            {
                userRoleTableName = GetUserRoleTableName(systemCode);
            }
            var roleTableName = BaseRoleEntity.CurrentTableName;
            if (!string.IsNullOrWhiteSpace(systemCode))
            {
                roleTableName = GetRoleTableName(systemCode);
            }

            var commandText = @"SELECT BaseRole.Id
                                    , BaseRole.Code 
                                    , BaseRole.Name 
                                    , BaseRole.Description
                                    , UserRole.UserId
                                    , UserRole.Enabled
                                    , UserRole.Deleted
                                    , UserRole.CreateTime
                                    , UserRole.CreateBy
                                    , UserRole.UpdateTime
                                    , UserRole.UpdateBy
 FROM BaseRole RIGHT OUTER JOIN
                          (SELECT UserId, RoleId, Enabled, Deleted, CreateTime, CreateBy, UpdateTime, UpdateBy FROM BaseUserRole
                            WHERE UserId = " + DbHelper.GetParameter(BaseUserRoleEntity.FieldUserId)
                                  + " AND " + BaseUtil.FieldEnabled + " = 1 AND SystemCode = '" + systemCode + "' AND " + BaseUserRoleEntity.FieldDeleted + " = 0" + @") UserRole 
                            ON BaseRole.Id = UserRole.RoleId WHERE BaseRole." + BaseRoleEntity.FieldEnabled + " = 1 AND BaseRole." + BaseRoleEntity.FieldDeleted + @" = 0 
                      ORDER BY UserRole." + BaseRoleEntity.FieldCreateTime + " DESC ";
            //替换表名
            commandText = commandText.Replace("BaseUserRole", userRoleTableName);
            commandText = commandText.Replace("BaseRole", roleTableName);

            var dbParameters =
                new List<IDbDataParameter> { DbHelper.MakeParameter(BaseUserRoleEntity.FieldUserId, userId) };

            //2017.12.20增加默认的HttpRuntime.Cache缓存
            var cacheKey = "Dt." + systemCode + "." + userId + ".UserRole";
            //var cacheTime = default(TimeSpan);
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            result = CacheUtil.Cache<DataTable>(cacheKey, () => Fill(commandText, dbParameters.ToArray()), true, false, cacheTime);
            return result;
        }

        #endregion

        #region public DataTable GetDataTableByCompanyByRole(string systemCode, string companyId, string roleId) 获取某个单位某个角色里的成员

        /// <summary>
        /// 获取某个单位某个角色里的成员
        /// 2015-12-05 吉日嘎拉 增加参数化优化。
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>用户数据表</returns>
        public DataTable GetDataTableByCompanyByRole(string systemCode, string companyId, string roleId)
        {
            var result = new DataTable(BaseRoleEntity.CurrentTableName);

            var commandText = @"SELECT A." + BaseUserEntity.FieldId + ", A." + BaseUserEntity.FieldId + ", A." + BaseUserEntity.FieldCode + ", A." + BaseUserEntity.FieldCompanyName + ", A." + BaseUserEntity.FieldDepartmentName + ", A." + BaseUserEntity.FieldRealName + ", A." + BaseUserEntity.FieldDescription + ", A." + BaseUserEntity.FieldEnabled + ", A." + BaseUserEntity.FieldCreateTime + ", A." + BaseUserEntity.FieldCreateBy + ", A." + BaseUserEntity.FieldUpdateTime + ", A." + BaseUserEntity.FieldUpdateBy + " FROM " + BaseUserEntity.CurrentTableName + @" A RIGHT OUTER JOIN (SELECT UserId, Enabled, CreateTime, CreateBy, UpdateTime, UpdateBy FROM BaseUserRole WHERE RoleId = " + DbHelper.GetParameter(BaseUserRoleEntity.FieldRoleId) + " AND " + BaseUtil.FieldDeleted + " = " + DbHelper.GetParameter(BaseUserRoleEntity.FieldDeleted) + @") UserRole ON A.Id = UserRole.UserId  WHERE A." + BaseUserEntity.FieldCompanyId + " = " + DbHelper.GetParameter(BaseUserEntity.FieldCompanyId) + " ORDER BY UserRole." + BaseUserRoleEntity.FieldUpdateTime;
            var dbParameters = new List<IDbDataParameter>
            {
                DbHelper.MakeParameter(BaseUserRoleEntity.FieldRoleId, roleId),
                DbHelper.MakeParameter(BaseUserRoleEntity.FieldDeleted, 0),
                DbHelper.MakeParameter(BaseUserEntity.FieldCompanyId, companyId)
            };
            result = Fill(commandText, dbParameters.ToArray());

            return result;
        }
        #endregion

        #region public static bool IsAdministrator(int userId) 是否为管理员
        /// <summary>
        /// 是否为管理员
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        public static bool IsAdministrator(int userId)
        {
            return IsAdministrator(userId.ToString());
        }
        /// <summary>
        /// 是否为管理员
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        public static bool IsAdministrator(string userId)
        {
            var result = false;

            using (var dbHelper = DbHelperFactory.Create(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection))
            {
                var commandText = @"SELECT COUNT(*) FROM " + BaseUserEntity.CurrentTableName
                                    + " WHERE Id = " + dbHelper.GetParameter(BaseUserEntity.FieldId)
                                              + " AND " + BaseUserEntity.FieldEnabled + " = " + dbHelper.GetParameter(BaseUserEntity.FieldEnabled)
                                              + " AND " + BaseUserEntity.FieldDeleted + " = " + dbHelper.GetParameter(BaseUserEntity.FieldDeleted)
                                              + " AND IsAdministrator = 1";
                var dbParameters = new List<IDbDataParameter>
                {
                    dbHelper.MakeParameter(BaseUserEntity.FieldId, userId),
                    dbHelper.MakeParameter(BaseUserEntity.FieldEnabled, 1),
                    dbHelper.MakeParameter(BaseUserEntity.FieldDeleted, 0)
                };
                var obj = dbHelper.ExecuteScalar(commandText, dbParameters.ToArray());

                if (obj != null && obj.ToInt() > 0)
                {
                    result = true;
                }
            }

            return result;
        }
        #endregion

        #region public string[] GetUserIds(string[] roleIds) 获取用户主键数组

        /// <summary>
        /// 获取用户主键数组
        /// </summary>
        /// <param name="roleIds">角色主键数组</param>
        /// <returns>用户主键数组</returns>
        public string[] GetUserIds(string[] roleIds)
        {
            return GetUserIds("Base", roleIds);
        }

        #endregion

        #region public string[] GetUserIdsByRole(string systemCode, string roleCode, string companyId = null) 按角色编号获得用户主键数组
        /// <summary>
        /// 按角色编号获得用户主键数组
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleCode">角色编号</param>
        /// <param name="companyId">公司主键</param>
        /// <returns>主键数组</returns>
        public string[] GetUserIdsByRole(string systemCode, string roleCode, string companyId = null)
        {
            string[] result = null;
            var roleId = BaseRoleManager.GetIdByCodeByCache(systemCode, roleCode);
            if (!string.IsNullOrEmpty(roleId))
            {
                result = GetUserIdsInRoleId(systemCode, roleId, companyId);
            }

            return result;
        }
        #endregion

        #region public string[] GetUserIdsInRoleId(string systemCode, string roleId, string companyId = null) 按角色主键获得用户主键数组
        /// <summary>
        /// 按角色主键获得用户主键数组
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="companyId">公司主键</param>
        /// <returns>主键数组</returns>
        public string[] GetUserIdsInRoleId(string systemCode, string roleId, string companyId = null)
        {
            string[] result = null;

            var userRoleTableName = GetUserRoleTableName(systemCode);
            var sb = PoolUtil.StringBuilder.Get();
            // 需要显示未被删除的用户
            sb.Append("SELECT UserId FROM " + userRoleTableName + " WHERE RoleId = " + DbHelper.GetParameter(BaseUserRoleEntity.FieldRoleId) + " AND " + BaseUserEntity.FieldDeleted + " = 0"
                              + " AND ( UserId IN (  SELECT " + BaseUserEntity.FieldId
                                                 + " FROM " + BaseUserEntity.CurrentTableName
                                                 + " WHERE " + BaseUserEntity.FieldEnabled + " = 1" + BaseUserEntity.FieldDeleted + " = 0");

            var dbParameters = new List<IDbDataParameter>
            {
                DbHelper.MakeParameter(BaseUserRoleEntity.FieldRoleId, roleId)
            };

            if (!string.IsNullOrWhiteSpace(companyId))
            {
                sb.Append(" AND " + BaseUserEntity.FieldCompanyId + " = " + DbHelper.GetParameter(BaseUserEntity.FieldCompanyId));
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldCompanyId, companyId));
            }
            sb.Append(" ) )");

            // var dt = DbHelper.Fill(sql);
            // return BaseUtil.FieldToArray(dt, BaseUserRoleEntity.FieldUserId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();

            var userIds = new List<string>();
            var dataReader = DbHelper.ExecuteReader(sb.Return(), dbParameters.ToArray());
            if (dataReader != null && !dataReader.IsClosed)
            {
                while (dataReader.Read())
                {
                    userIds.Add(dataReader["UserId"].ToString());
                }

                dataReader.Close();
            }

            result = userIds.ToArray();

            return result;
        }
        #endregion

        #region GetUserIds(string systemCode, string[] roleIds) 获取用户主键数组

        /// <summary>
        /// 获取用户主键数组
        /// 2015-11-03 吉日嘎拉 优化程序、用ExecuteReader提高性能
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleIds">角色主键数组</param>
        /// <returns>用户主键数组</returns>
        public string[] GetUserIds(string systemCode, string[] roleIds)
        {
            string[] result = null;

            if (roleIds != null && roleIds.Length > 0)
            {
                var userRoleTableName = GetUserRoleTableName(systemCode);
                var commandText = "SELECT DISTINCT " + BaseUserRoleEntity.FieldUserId + " FROM " + userRoleTableName + " WHERE " + BaseUserRoleEntity.FieldRoleId + " IN (" + StringUtil.ArrayToList(roleIds) + ") "
                                + "  AND (" + BaseUserRoleEntity.FieldUserId + " IN (SELECT " + BaseUserEntity.FieldId + " FROM " + BaseUserEntity.CurrentTableName + " WHERE " + BaseUserEntity.FieldDeleted + " = 0)) AND (" + BaseUserRoleEntity.FieldDeleted + " = 0)";

                var ids = new List<string>();
                var dataReader = DbHelper.ExecuteReader(commandText);
                if (dataReader != null && !dataReader.IsClosed)
                {
                    while (dataReader.Read())
                    {
                        ids.Add(dataReader["UserId"].ToString());
                    }

                    dataReader.Close();
                }

                result = ids.ToArray();
            }

            return result;
        }

        #endregion

        #region 添加角色

        #region AddToRole(string systemCode, string[] userIds, string[] roleIds, bool enabled = true) 加入到角色
        /// <summary>
        /// 加入到角色
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="roleIds"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public int AddToRole(string systemCode, string[] userIds, string[] roleIds, bool enabled = true)
        {
            var result = 0;
            for (var i = 0; i < userIds.Length; i++)
            {
                for (var j = 0; j < roleIds.Length; j++)
                {
                    AddToRoleById(systemCode, userIds[i], roleIds[j], enabled);
                    result++;
                }
            }
            return result;
        }
        #endregion

        #region AddToRole(string systemCode, string userId, string roleId, bool enabled = true) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="enabled">有效状态</param>
        /// <returns>主键</returns>
        public string AddToRoleById(string systemCode, string userId, string roleId, bool enabled = true)
        {
            var result = string.Empty;

            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = "Base";
            }
            if (ValidateUtil.IsInt(userId) && ValidateUtil.IsInt(roleId))
            {
                var entity = new BaseUserRoleEntity
                {
                    SystemCode = systemCode,
                    UserId = userId.ToInt(),
                    RoleId = roleId.ToInt(),
                    Enabled = enabled ? 1 : 0
                };
                var userRoleTableName = GetUserRoleTableName(systemCode);
                //新增或激活
                result = new BaseUserRoleManager(DbHelper, UserInfo, userRoleTableName).AddOrActive(entity);
            }

            return result;
        }
        #endregion

        #endregion

        #region 移除角色

        #region RemoveFromRole(string systemCode, string userId, string roleId) 将用户从角色移除
        /// <summary>
        /// 将用户从角色移除
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>影响行数</returns>
        public int RemoveFromRole(string systemCode, string userId, string roleId)
        {
            var userRoleTableName = GetUserRoleTableName(systemCode);

            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldSystemCode, systemCode),
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldUserId, userId),
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldRoleId, roleId)
            };
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldEnabled, 0),
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldDeleted, 1)
            };
            // 更新删除状态
            return new BaseUserRoleManager(DbHelper, UserInfo, userRoleTableName).Update(whereParameters, parameters);
        }
        #endregion

        #region RemoveFromRole(string systemCode, string userId, string[] roleIds) 将用户从角色移除

        /// <summary>
        /// 从角色中删除
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public int RemoveFromRole(string systemCode, string userId, string[] roleIds)
        {
            var result = 0;
            for (var i = 0; i < roleIds.Length; i++)
            {
                result += RemoveFromRole(systemCode, userId, roleIds[i]);
            }
            return result;
        }

        #endregion

        #region RemoveFromRole(string systemCode, string[] userIds, string roleId) 将用户从角色移除

        /// <summary>
        /// 从角色中删除
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public int RemoveFromRole(string systemCode, string[] userIds, string roleId)
        {
            var result = 0;
            for (var i = 0; i < userIds.Length; i++)
            {
                result += RemoveFromRole(systemCode, userIds[i], roleId);
            }
            return result;
        }
        #endregion

        #region RemoveFromRole(string systemCode, string[] userIds, string[] roleIds) 将用户从角色移除
        /// <summary>
        /// 从角色中删除
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public int RemoveFromRole(string systemCode, string[] userIds, string[] roleIds)
        {
            var result = 0;

            for (var i = 0; i < userIds.Length; i++)
            {
                for (var j = 0; j < roleIds.Length; j++)
                {
                    result += RemoveFromRole(systemCode, userIds[i], roleIds[j]);
                }
            }

            return result;
        }

        #endregion

        #endregion

        #region 权限

        #region 复制用户权限
        /// <summary>
        /// 复制用户权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="referenceUserIds">参考源用户编号</param>
        /// <param name="targetUserIds">目标用户编号</param>
        /// <returns></returns>
        public int CopyPermission(string systemCode, string[] referenceUserIds, string[] targetUserIds)
        {
            var result = 0;

            for (var i = 0; i < referenceUserIds.Length; i++)
            {
                for (var j = 0; j < targetUserIds.Length; j++)
                {
                    result += CopyPermission(systemCode, referenceUserIds[i].ToInt(), targetUserIds[j].ToInt());
                }
            }

            return result;
        }
        #endregion

        #region 复制用户权限
        /// <summary>
        /// 复制用户权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="referenceUserId">参考源用户编号</param>
        /// <param name="targetUserId">目标用户编号</param>
        /// <returns></returns>
        public int CopyPermission(string systemCode, int referenceUserId, int targetUserId)
        {
            var result = 0;
            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = "Base";
            }
            var userRoleTableName = GetUserRoleTableName(systemCode);
            var manager = new BaseUserRoleManager(DbHelper, UserInfo, userRoleTableName);
            result += manager.CopyRole(systemCode, referenceUserId, targetUserId);
            var permissionTableName = GetPermissionTableName(systemCode);
            var permissionManager = new BasePermissionManager(DbHelper, UserInfo, permissionTableName);
            result += permissionManager.CopyRolePermission(systemCode, referenceUserId, targetUserId);
            result += permissionManager.CopyUserPermission(systemCode, referenceUserId, targetUserId);

            return result;
        }
        #endregion

        #endregion
    }
}