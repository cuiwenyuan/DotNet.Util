//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
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
        /// <summary>
        /// 从缓存中获取角色编号
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
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

        /// <summary>
        /// 用户是否在某个角色里
        /// 包括所在公司的角色也进行判断
        /// </summary>
        /// <param name="userInfo">当前用户</param>
        /// <param name="roleName">角色名称</param>
        /// <returns>是否在角色中</returns>
        public bool IsInRole(BaseUserInfo userInfo, string roleName)
        {
            return IsInRole(userInfo.SystemCode, userInfo.Id, roleName);
        }

        /// <summary>
        /// 用户是否在某个角色中
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="roleName">角色</param>
        /// <returns>存在</returns>
        public bool IsInRole(string userId, string roleName)
        {
            var systemCode = "Base";
            if (UserInfo != null && !string.IsNullOrWhiteSpace(UserInfo.SystemCode))
            {
                systemCode = UserInfo.SystemCode;
            }

            return IsInRole(systemCode, userId, roleName);
        }

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

        /// <summary>
        /// 用户是否在某个角色里
        /// 包括所在公司的角色也进行判断
        /// </summary>
        /// <param name="userInfo">当前用户</param>
        /// <param name="code">编号</param>
        /// <returns>是否在角色里</returns>
        public bool IsInRoleByCode(BaseUserInfo userInfo, string code)
        {
            return IsInRoleByCode(userInfo.SystemCode, userInfo.Id, code);
        }

        /// <summary>
        /// 用户是否在某个角色中
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="code">角色编号</param>
        /// <returns>存在</returns>
        public bool IsInRoleByCode(string userId, string code)
        {
            var systemCode = "Base";
            if (UserInfo != null && !string.IsNullOrWhiteSpace(UserInfo.SystemCode))
            {
                systemCode = UserInfo.SystemCode;
            }

            return IsInRoleByCode(systemCode, userId, code);
        }

        /// <summary>
        /// 用户是否在某个角色中
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="roleCode">角色编号</param>
        /// <param name="useBaseRole">使用基础角色</param>
        /// <returns>存在</returns>
        public bool IsInRoleByCode(string systemCode, string userId, string roleCode, bool useBaseRole = true)
        {
            var result = false;

            if (string.IsNullOrEmpty(systemCode))
            {
                return false;
            }

            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            // 2016-05-24 吉日嘎拉，若是Oracle数据库，用户的Id目前都是数值类型，若不是数字就出错了，不用进行运算了。
            if (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.SqlServer)
            {
                if (!ValidateUtil.IsInt(userId))
                {
                    return false;
                }
            }

            if (string.IsNullOrEmpty(roleCode))
            {
                return false;
            }

            // 2016-01-07 吉日嘎拉 这里用缓存、效率会高
            var roleId = BaseRoleManager.GetIdByCodeByCache(systemCode, roleCode);
            if (string.IsNullOrEmpty(roleId))
            {
                // 2016-01-08 吉日嘎拉 看基础共用的角色里，是否在
                if (useBaseRole && !systemCode.Equals("Base"))
                {
                    roleId = BaseRoleManager.GetIdByCodeByCache("Base", roleCode);
                }
                if (string.IsNullOrEmpty(roleId))
                {
                    // 表明2个系统里都没了，就真没了
                    return false;
                }
            }

            var listUserRole = GetUserRoleEntityList(systemCode);
            result = listUserRole.Exists((t => t.UserId == userId && t.RoleId == roleId));

            return result;
        }
        /// <summary>
        /// 获取用户角色实体列表
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <returns></returns>
        public List<BaseUserRoleEntity> GetUserRoleEntityList(string systemCode)
        {
            var tableName = systemCode + "UserRole";
            //2017.12.19增加默认的HttpRuntime.Cache缓存
            var cacheKey = "List." + systemCode + ".UserRole";
            //var cacheTime = default(TimeSpan);
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            var result = CacheUtil.Cache<List<BaseUserRoleEntity>>(cacheKey, () =>
            {
                var parametersWhere = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserRoleEntity.FieldDeleted, 0),
                    new KeyValuePair<string, object>(BaseUserRoleEntity.FieldEnabled, 1)
                };
                return new BaseUserRoleManager(DbHelper, UserInfo, tableName).GetList<BaseUserRoleEntity>(parametersWhere, BaseUserRoleEntity.FieldId);
            }, true, false, cacheTime);

            return result;
        }

        #region IsHasRoleByCodeContains
        /// <summary>
        /// 用户是否拥有包含指定关键字的角色编码
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="searchKey">关键字</param>
        /// <returns></returns>
        public bool IsHasRoleCodeContains(string userId, string searchKey)
        {
            var systemCode = "Base";
            if (UserInfo != null && !string.IsNullOrWhiteSpace(UserInfo.SystemCode))
            {
                systemCode = UserInfo.SystemCode;
            }

            return IsHasRoleCodeContains(systemCode, userId, searchKey);
        }
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

            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            //用户的Id对Oracle和MSSQL目前都是数值类型，若不是数字就出错了，不用浪费时间了。
            if (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.SqlServer)
            {
                if (!ValidateUtil.IsInt(userId))
                {
                    return false;
                }
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

        #region IsHasRoleCodeStartWith
        /// <summary>
        /// 用户是否拥有以指定关键字开始的角色编码
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="searchKey">关键字</param>
        /// <returns></returns>
        public bool IsHasRoleCodeStartWith(string userId, string searchKey)
        {
            var systemCode = "Base";
            if (UserInfo != null && !string.IsNullOrWhiteSpace(UserInfo.SystemCode))
            {
                systemCode = UserInfo.SystemCode;
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

            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            //用户的Id对Oracle和MSSQL目前都是数值类型，若不是数字就出错了，不用浪费时间了。
            if (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.SqlServer)
            {
                if (!ValidateUtil.IsInt(userId))
                {
                    return false;
                }
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

        /// <summary>
        /// 获取所有用户的角色列表
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <returns>角色数据表</returns>
        public DataTable GetUserRole(string systemCode)
        {
            var result = new DataTable(BaseRoleEntity.TableName);

            var tableUserRoleName = BaseUserRoleEntity.TableName;
            if (!string.IsNullOrWhiteSpace(systemCode))
            {
                tableUserRoleName = systemCode + "UserRole";
            }
            var tableRoleName = BaseRoleEntity.TableName;
            if (!string.IsNullOrWhiteSpace(systemCode))
            {
                tableRoleName = systemCode + "Role";
            }

            var sb = Pool.StringBuilder.Get();
            sb.AppendLine("SELECT BaseRole.Code, BaseRole.RealName, BaseRole.Description, UserRole.Id, UserRole.UserId, UserRole.RoleId, UserRole.Enabled, UserRole.DeletionStateCode, UserRole.CreateOn, UserRole.CreateBy, UserRole.ModifiedOn, UserRole.ModifiedBy");
            sb.AppendLine(" FROM BaseRole INNER JOIN (SELECT Id, UserId, RoleId, Enabled, DeletionStateCode, CreateOn, CreateBy, ModifiedOn, ModifiedBy FROM BaseUserRole WHERE Enabled = 1 AND " + BaseUserRoleEntity.FieldDeleted + " = 0) UserRole ON BaseRole.Id = UserRole.RoleId");
            sb.AppendLine(" WHERE BaseRole.Enabled = 1 AND BaseRole." + BaseRoleEntity.FieldDeleted + " = 0 ORDER BY UserRole.CreateOn DESC");
            //替换表名
            sb = sb.Replace("BaseUserRole", tableUserRoleName);
            sb = sb.Replace("BaseRole", tableRoleName);

            var cacheKey = "DataTable." + systemCode + ".UserRole";
            //var cacheTime = default(TimeSpan);
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            result = CacheUtil.Cache<DataTable>(cacheKey, () => Fill(sb.Put()), true, false, cacheTime);
            return result;
        }
        /// <summary>
        /// 获取用户的角色主键数组
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns>主键数组</returns>
        public string[] GetRoleIds(string userId)
        {
            return GetRoleIds(UserInfo.SystemCode, userId);
        }

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

            var userRoleTable = systemCode + "UserRole";

            // 被删除的角色不应该显示出来
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT " + BaseUserRoleEntity.FieldRoleId);
            sb.Append(" FROM " + userRoleTable);
            sb.Append(" WHERE " + BaseUserRoleEntity.FieldUserId + " = " + DbHelper.GetParameter(BaseUserRoleEntity.FieldUserId));
            sb.Append(" AND " + BaseUserRoleEntity.FieldEnabled + " = " + DbHelper.GetParameter(BaseUserRoleEntity.FieldEnabled));
            sb.Append(" AND " + BaseUserRoleEntity.FieldDeleted + " = " + DbHelper.GetParameter(BaseUserRoleEntity.FieldDeleted));

            var dbParameters = new List<IDbDataParameter>
            {
                DbHelper.MakeParameter(BaseUserRoleEntity.FieldUserId, userId),
                DbHelper.MakeParameter(BaseUserRoleEntity.FieldEnabled, 1),
                DbHelper.MakeParameter(BaseUserRoleEntity.FieldDeleted, 0)
            };

            /*
            if (useBaseRole && !systemCode.Equals("Base", StringComparison.OrdinalIgnoreCase))
            {
                // 是否使用基础角色的权限 
                sql.Append(" UNION SELECT " + BaseUserRoleEntity.FieldRoleId);
                sql.Append(" FROM " + BaseUserRoleEntity.TableName);
                sql.Append(" WHERE ( " + BaseUserRoleEntity.FieldUserId + " = " + DbHelper.GetParameter(BaseUserRoleEntity.TableName + "_USEBASE_" + BaseUserRoleEntity.FieldUserId));
                sql.Append(" AND " + BaseUserRoleEntity.FieldEnabled + " = 1 ");
                sql.Append(" AND " + BaseUserRoleEntity.FieldDeleted + " = 0 ) ");

                dbParameters.Add(DbHelper.MakeParameter(BaseUserRoleEntity.TableName + "_USEBASE_" + BaseUserRoleEntity.FieldUserId, userId));
            }

            if (BaseSystemInfo.UseRoleOrganization && !string.IsNullOrEmpty(companyId))
            {
                string roleOrganizationTableName = systemCode + "RoleOrganization";
                sql += " UNION SELECT " + BaseRoleOrganizationEntity.FieldRoleId
                                  + " FROM " + roleOrganizationTableName
                                + "  WHERE " + BaseRoleOrganizationEntity.FieldOrganizationId + " = " + DbHelper.GetParameter(BaseRoleOrganizationEntity.FieldOrganizationId)
                                  + "        AND " + BaseRoleOrganizationEntity.FieldEnabled + " = 1 "
                                  + "        AND " + BaseRoleOrganizationEntity.FieldDeleted + " = 0 ";

                dbParameters.Add(DbHelper.MakeParameter(BaseRoleOrganizationEntity.FieldOrganizationId, companyId));
            }
            */

            using (var dataReader = DbHelper.ExecuteReader(sb.Put(), dbParameters.ToArray()))
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

        #region public List<BaseRoleEntity> GetRoleList(string systemCode, string userId, string companyId = null)
        /// <summary>
        /// 一个用户的所有的角色列表
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="useBaseRole"></param>
        /// <returns>角色列表</returns>
        public List<BaseRoleEntity> GetRoleList(string systemCode, string userId, string companyId = null, bool useBaseRole = true)
        {
            var result = new List<BaseRoleEntity>();

            var roleIds = GetRoleIds(systemCode, userId, companyId);
            if (roleIds != null && roleIds.Length > 0)
            {
                var entities = BaseRoleManager.GetEntitiesByCache(systemCode);
                result = (entities as List<BaseRoleEntity>).Where(t => roleIds.Contains(t.Id) && t.Enabled == 1 && t.DeletionStateCode == 0).ToList();
            }

            return result;

            /*
            string userRoleTable = systemCode + "UserRole";
            string roleTable = systemCode + "Role";
            // 被删除的角色不应该显示出来
            string sql = @"SELECT * FROM " + roleTable + " WHERE Enabled = 1 AND " + BaseUserEntity.FieldDeleted + " = 0 AND Id "
                + " IN (SELECT RoleId FROM " + userRoleTable
                + " WHERE UserId = " + DbHelper.GetParameter(BaseUserRoleEntity.FieldUserId) + " AND Enabled = 1 AND " + BaseUserRoleEntity.FieldDeleted + " = 0 ";

            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            dbParameters.Add(DbHelper.MakeParameter(BaseUserRoleEntity.FieldUserId, userId));

            if (BaseSystemInfo.UseRoleOrganization && !string.IsNullOrEmpty(companyId))
            {
                string roleOrganizationTableName = systemCode + "RoleOrganization";
                sql += " UNION SELECT " + BaseRoleOrganizationEntity.FieldRoleId
                                  + " FROM " + roleOrganizationTableName
                                + "  WHERE " + BaseRoleOrganizationEntity.FieldOrganizationId + " = " + DbHelper.GetParameter(BaseRoleOrganizationEntity.FieldOrganizationId)
                                  + "        AND " + BaseRoleOrganizationEntity.FieldEnabled + " = 1 "
                                  + "        AND " + BaseRoleOrganizationEntity.FieldDeleted + " = 0 ";

                dbParameters.Add(DbHelper.MakeParameter(BaseRoleOrganizationEntity.FieldOrganizationId, companyId));
            }

            sql += ")";

            using (IDataReader dataReader = DbHelper.ExecuteReader(sql, dbParameters.ToArray()))
            {
                result = BaseEntity.GetList<BaseRoleEntity>(dataReader);
            }
            */
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

            var tableNameUserRole = systemCode + "UserRole";

            var sql = "SELECT " + SelectFields
                     + " FROM " + BaseUserEntity.TableName
                     + "          , (SELECT " + BaseUserRoleEntity.FieldUserId
                          + " FROM " + tableNameUserRole
                          + "        WHERE " + BaseUserRoleEntity.FieldRoleId + " IN (" + string.Join(",", roleIds) + ")"
                          + "               AND " + BaseUserRoleEntity.FieldEnabled + " = 1 "
                          + "               AND " + BaseUserRoleEntity.FieldDeleted + " = 0) B "
                          + " WHERE " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldId + " = B." + BaseUserRoleEntity.FieldUserId
                          + "       AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldEnabled + " = 1 "
                          + "       AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDeleted + "= 0";

            if (!string.IsNullOrWhiteSpace(companyId))
            {
                sql += " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldCompanyId + " = " + DbHelper.GetParameter(BaseUserEntity.FieldCompanyId);
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldCompanyId, companyId));
            }

            using (var dr = DbHelper.ExecuteReader(sql, dbParameters.ToArray()))
            {
                result = GetList<BaseUserEntity>(dr);
            }

            return result;
        }
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
            var tableNameUserRole = systemCode + "UserRole";

            var sql = "SELECT " + SelectFields + " FROM " + BaseUserEntity.TableName
                            + " WHERE " + BaseUserEntity.FieldEnabled + " = 1 "
                            + "       AND " + BaseUserEntity.FieldDeleted + "= 0 "
                            + "       AND ( " + BaseUserEntity.FieldId + " IN "
                            + "           (SELECT  " + BaseUserRoleEntity.FieldUserId
                            + " FROM " + tableNameUserRole
                            + "             WHERE " + BaseUserRoleEntity.FieldRoleId + " IN (" + string.Join(",", roleIds) + ")"
                            + "               AND " + BaseUserRoleEntity.FieldEnabled + " = 1"
                            + "                AND " + BaseUserRoleEntity.FieldDeleted + " = 0)) "
                            + " ORDER BY  " + BaseUserEntity.FieldSortCode;

            return DbHelper.Fill(sql);
        }
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
            var tableName = systemCode + "UserRole";
            var manager = new BaseUserRoleManager(DbHelper, UserInfo, tableName);
            result += manager.Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(BaseUserRoleEntity.FieldRoleId, roleId) });

            return result;
        }
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
            var tableName = systemCode + "UserRole";
            var manager = new BaseUserRoleManager(DbHelper, UserInfo, tableName);
            result += manager.Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(BaseUserRoleEntity.FieldUserId, userId) });

            return result;
        }

        /// <summary>
        /// 获取用户的角色列表
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <returns>角色数据表</returns>
        public DataTable GetUserRoleDataTable(string systemCode, string userId)
        {
            var result = new DataTable(BaseRoleEntity.TableName);

            var tableUserRoleName = BaseUserRoleEntity.TableName;
            if (!string.IsNullOrWhiteSpace(systemCode))
            {
                tableUserRoleName = systemCode + "UserRole";
            }
            var tableRoleName = BaseRoleEntity.TableName;
            if (!string.IsNullOrWhiteSpace(systemCode))
            {
                tableRoleName = systemCode + "Role";
            }

            var commandText = @"SELECT BaseRole.Id
                                    , BaseRole.Code 
                                    , BaseRole.RealName 
                                    , BaseRole.Description
                                    , UserRole.UserId
                                    , UserRole.Enabled
                                    , UserRole.DeletionStateCode
                                    , UserRole.CreateOn
                                    , UserRole.CreateBy
                                    , UserRole.ModifiedOn
                                    , UserRole.ModifiedBy
 FROM BaseRole RIGHT OUTER JOIN
                          (SELECT UserId, RoleId, Enabled, DeletionStateCode, CreateOn, CreateBy, ModifiedOn, ModifiedBy
 FROM BaseUserRole
                            WHERE UserId = " + DbHelper.GetParameter(BaseUserRoleEntity.FieldUserId)
                                  + " AND Enabled = 1 AND " + BaseUserRoleEntity.FieldDeleted + " = 0 " + @") UserRole 
                            ON BaseRole.Id = UserRole.RoleId
                         WHERE BaseRole.Enabled = 1 AND BaseRole." + BaseRoleEntity.FieldDeleted + @" = 0 
                      ORDER BY UserRole.CreateOn DESC ";
            //替换表名
            commandText = commandText.Replace("BaseUserRole", tableUserRoleName);
            commandText = commandText.Replace("BaseRole", tableRoleName);

            var dbParameters =
                new List<IDbDataParameter> { DbHelper.MakeParameter(BaseUserRoleEntity.FieldUserId, userId) };

            //2017.12.20增加默认的HttpRuntime.Cache缓存
            var cacheKey = "DataTable." + systemCode + "." + userId + ".UserRole";
            //var cacheTime = default(TimeSpan);
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            result = CacheUtil.Cache<DataTable>(cacheKey, () => Fill(commandText, dbParameters.ToArray()), true, false, cacheTime);
            return result;
        }

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
            var result = new DataTable(BaseRoleEntity.TableName);

            var commandText = @"SELECT BaseUser.Id
                                    , BaseUser.UserName
                                    , BaseUser.Code
                                    , BaseUser.Companyname
                                    , BaseUser.DepartmentName
                                    , BaseUser.RealName 
                                    , BaseUser.Description 
                                    , UserRole.Enabled
                                    , UserRole.CreateOn
                                    , UserRole.CreateBy
                                    , UserRole.ModifiedOn
                                    , UserRole.ModifiedBy
 FROM BaseUser RIGHT OUTER JOIN
                          (SELECT UserId, Enabled, CreateOn, CreateBy, ModifiedOn, ModifiedBy
 FROM BaseUserRole
                            WHERE RoleId = " + DbHelper.GetParameter(BaseUserRoleEntity.FieldRoleId)
                             + " AND DeletionStateCode = " + DbHelper.GetParameter(BaseUserRoleEntity.FieldDeleted) + @") UserRole 
                            ON BaseUser.Id = UserRole.UserId
                         WHERE BaseUser.CompanyId = " + DbHelper.GetParameter(BaseUserEntity.FieldCompanyId) + @"
                      ORDER BY UserRole.CreateOn";

            var dbParameters = new List<IDbDataParameter>
            {
                DbHelper.MakeParameter(BaseUserRoleEntity.FieldRoleId, roleId),
                DbHelper.MakeParameter(BaseUserRoleEntity.FieldDeleted, 0),
                DbHelper.MakeParameter(BaseUserEntity.FieldCompanyId, companyId)
            };
            result = Fill(commandText, dbParameters.ToArray());

            return result;
        }
        /// <summary>
        /// 是否为管理员
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool IsAdministrator(string userId)
        {
            var result = false;

            using (var dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection))
            {
                var commandText = @"SELECT COUNT(*) "
                                     + " FROM " + BaseUserEntity.TableName
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
                var isAdministrator = dbHelper.ExecuteScalar(commandText, dbParameters.ToArray());
                result = int.Parse(isAdministrator.ToString()) > 0;
            }

            return result;
        }
        /// <summary>
        /// 从角色中删除
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public string[] GetUserIdsInRoleId(string roleId)
        {
            return GetUserIdsInRoleId(UserInfo.SystemCode, roleId);
        }

        /// <summary>
        /// 获取用户主键数组
        /// </summary>
        /// <param name="roleIds">角色主键数组</param>
        /// <returns>用户主键数组</returns>
        public string[] GetUserIds(string[] roleIds)
        {
            return GetUserIds("Base", roleIds);
        }

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

            var tableName = "BaseUserRole";
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableName = systemCode + "UserRole";
            }

            // 需要显示未被删除的用户
            var sql = "SELECT UserId FROM " + tableName + " WHERE RoleId = " + DbHelper.GetParameter(BaseUserRoleEntity.FieldRoleId) + " AND " + BaseUserEntity.FieldDeleted + " = 0 "
                              + " AND ( UserId IN (  SELECT " + BaseUserEntity.FieldId
                                                 + " FROM " + BaseUserEntity.TableName
                                                 + "  WHERE " + BaseUserEntity.FieldEnabled + " = 1 " + BaseUserEntity.FieldDeleted + " = 0 ";

            var dbParameters = new List<IDbDataParameter>
            {
                DbHelper.MakeParameter(BaseUserRoleEntity.FieldRoleId, roleId)
            };

            if (!string.IsNullOrWhiteSpace(companyId))
            {
                sql += " AND " + BaseUserEntity.FieldCompanyId + " = " + DbHelper.GetParameter(BaseUserEntity.FieldCompanyId);
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldCompanyId, companyId));
            }
            sql += " ) )";

            // var dt = DbHelper.Fill(sql);
            // return BaseUtil.FieldToArray(dt, BaseUserRoleEntity.FieldUserId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();

            var userIds = new List<string>();
            using (var dataReader = DbHelper.ExecuteReader(sql, dbParameters.ToArray()))
            {
                while (dataReader.Read())
                {
                    userIds.Add(dataReader["UserId"].ToString());
                }
            }
            result = userIds.ToArray();

            return result;
        }
        #endregion

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
                // 需要显示未被删除的用户
                var tableName = systemCode + "UserRole";
                var commandText = "SELECT UserId FROM " + tableName + " WHERE RoleId IN (" + StringUtil.ArrayToList(roleIds) + ") "
                                + "  AND (UserId IN (SELECT Id FROM " + BaseUserEntity.TableName + " WHERE " + BaseUserEntity.FieldDeleted + " = 0)) AND (" + BaseUserEntity.FieldDeleted + " = 0)";

                var ids = new List<string>();
                using (var dr = DbHelper.ExecuteReader(commandText))
                {
                    while (dr.Read())
                    {
                        ids.Add(dr["UserId"].ToString());
                    }
                }
                // 这里不需要有重复数据、用程序的方式把重复的数据去掉
                // result = BaseUtil.FieldToArray(dt, BaseUserRoleEntity.FieldUserId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
                result = ids.ToArray();
            }

            return result;
        }

        //
        // 加入到角色
        //

        /*
        public string AddToRole(string systemCode, string userId, string roleName, bool enabled = true)
        {
            string result = string.Empty;

            string roleId = BaseRoleManager.GetIdByNameByCache(systemCode, roleName);
            if (!string.IsNullOrEmpty(roleId))
            {
                result = AddToRoleById(systemCode, userId, roleId, enabled);
            }
            
            return result;
        }

        public int AddToRole(string systemCode, string userId, string[] roleIds, bool enabled = true)
        {
            int result = 0;

            for (int i = 0; i < roleIds.Length; i++)
            {
                this.AddToRoleById(systemCode, userId, roleIds[i], enabled);
                result++;
            }

            return result;
        }

        public int AddToRole(string systemCode, string[] userIds, string roleId)
        {
            int result = 0;

            for (int i = 0; i < userIds.Length; i++)
            {
                this.AddToRoleById(systemCode, userIds[i], roleId);
                result++;
            }

            return result;
        }
        */
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

        #region public string AddToRole(string systemCode, string userId, string roleId, bool enabled = true) 为了提高授权的运行速度
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
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(roleId))
            {
                var entity = new BaseUserRoleEntity
                {
                    UserId = userId,
                    RoleId = roleId,
                    Enabled = enabled ? 1 : 0,
                    DeletionStateCode = 0
                };
                // 2016-03-02 吉日嘎拉 增加按公司可以区别数据的功能。
                //if (this.DbHelper.CurrentDbType == CurrentDbType.MySql)
                //{
                //    entity.CompanyId = BaseUserManager.GetCompanyIdByCache(userId);
                //}
                // 2015-12-05 吉日嘎拉 把修改人记录起来，若是新增加的
                if (UserInfo != null)
                {
                    entity.CreateUserId = UserInfo.Id;
                    entity.CreateBy = UserInfo.RealName;
                    entity.CreateOn = DateTime.Now;
                    entity.ModifiedUserId = UserInfo.Id;
                    entity.ModifiedBy = UserInfo.RealName;
                    entity.ModifiedOn = DateTime.Now;
                }
                var tableName = systemCode + "UserRole";
                var manager = new BaseUserRoleManager(DbHelper, UserInfo, tableName);
                result = manager.Add(entity);
            }

            return result;
        }
        #endregion


        //
        //  从角色中移除用户
        //

        #region public int RemoveFormRole(string systemCode, string userId, string roleId) 将用户从角色移除
        /// <summary>
        /// 将用户从角色移除
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>影响行数</returns>
        public int RemoveFormRole(string systemCode, string userId, string roleId)
        {
            var tableName = BaseUserRoleEntity.TableName;
            if (!string.IsNullOrWhiteSpace(systemCode))
            {
                tableName = systemCode + "UserRole";
            }

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldUserId, userId),
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldRoleId, roleId)
            };
            var manager = new BaseUserRoleManager(DbHelper, UserInfo, tableName);
            return manager.Delete(parameters);
        }
        #endregion

        /// <summary>
        /// 从角色中删除
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public int RemoveFormRole(string systemCode, string userId, string[] roleIds)
        {
            var result = 0;
            for (var i = 0; i < roleIds.Length; i++)
            {
                //移除用户角色
                result += RemoveFormRole(systemCode, userId, roleIds[i]);
            }
            return result;
        }

        /// <summary>
        /// 从角色中删除
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public int RemoveFormRole(string systemCode, string[] userIds, string roleId)
        {
            var result = 0;
            for (var i = 0; i < userIds.Length; i++)
            {
                //移除用户角色
                result += RemoveFormRole(systemCode, userIds[i], roleId);
            }
            return result;
        }

        /// <summary>
        /// 从角色中删除
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public int RemoveFormRole(string systemCode, string[] userIds, string[] roleIds)
        {
            var result = 0;

            for (var i = 0; i < userIds.Length; i++)
            {
                for (var j = 0; j < roleIds.Length; j++)
                {
                    result += RemoveFormRole(systemCode, userIds[i], roleIds[j]);
                }
            }

            return result;
        }

        /// <summary>
        /// 轮循计数器
        /// </summary>
        public static int UserIndex = 0;

        /// <summary>
        /// 从某个角色列表中轮循获取一个用户，在线的用户优先。
        /// </summary>
        /// <param name="systemCode">子系统编码</param>
        /// <param name="roleCode">角色编号</param>
        /// <returns>用户主键</returns>
        public string GetRandomUserId(string systemCode, string roleCode)
        {
            var result = string.Empty;

            // 先不管是否在线，总需要能发一个人再说
            var userIds = GetUserIdsByRole(systemCode, roleCode);
            if (userIds != null && userIds.Length > 0)
            {
                var index = UserIndex % userIds.Length;
                result = userIds[index];

                // 接着再判断是否有人在线，若有在线的，发给在线的用户
                var userLogonManager = new BaseUserLogonManager(DbHelper);
                userIds = userLogonManager.GetOnlineUserIds(userIds);
                if (userIds != null && userIds.Length > 0)
                {
                    index = UserIndex % userIds.Length;
                    result = userIds[index];
                }
            }
            UserIndex++;

            return result;
        }
    }
}