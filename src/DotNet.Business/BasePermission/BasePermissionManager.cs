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
    /// BasePermissionManager
    /// 资源权限管理，操作权限管理（这里实现了用户操作权限，角色的操作权限）
    /// 
    /// 修改记录
    ///
    ///     2015.07.10 版本：2.1 JiRiGaLa 把删除补上来。
    ///     2010.09.21 版本：2.0 JiRiGaLa 智能权限判断、后台自动增加权限，增加并发锁PermissionLock。
    ///     2009.09.22 版本：1.1 JiRiGaLa 前台判断的权限，后台都需要记录起来，防止后台缺失前台的判断权限。
    ///     2008.03.28 版本：1.0 JiRiGaLa 创建主键。
    ///     
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.07.10</date>
    /// </author>
    /// </summary>
    public partial class BasePermissionManager : BaseManager, IBaseManager
    {
        #region public bool PermissionExists(string permissionId, string resourceCategory, string resourceId) 检查是否存在
        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="permissionId">权限主键</param>
        /// <param name="resourceCategory">资源分类</param>
        /// <param name="resourceId">资源主键</param>
        /// <returns>是否存在</returns>      
        public bool PermissionExists(string permissionId, string resourceCategory, string resourceId)
        {
            var result = false;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, resourceId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, resourceCategory),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldPermissionId, permissionId)
            };
            // 检查是否存在
            result = Exists(parameters);

            return result;
        }
        #endregion

        #region public string AddPermission(BasePermissionEntity permissionEntity) 添加
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="permissionEntity">实体</param>
        /// <returns>主键</returns>
        public string AddPermission(BasePermissionEntity permissionEntity)
        {
            var result = string.Empty;
            // 检查记录是否重复
            if (!PermissionExists(permissionEntity.PermissionId, permissionEntity.ResourceCategory, permissionEntity.ResourceId))
            {
                result = AddEntity(permissionEntity);
            }
            return result;
        }
        #endregion


        //
        // ResourcePermission 权限判断
        // 

        #region public bool IsAuthorized(string systemCode, string userId, string permissionCode, string permissionName = null, bool ignoreAdministrator = false, bool useBaseRole = true) 是否有相应的权限
        /// <summary>
        /// 是否有相应的权限
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="permissionName">权限名称</param>
        /// <param name="ignoreAdministrator">忽略管理员默认权限</param>
        /// <param name="useBaseRole">通用基础角色的权限是否计算</param>
        /// <returns>是否有权限</returns>
        public bool IsAuthorized(string systemCode, string userId, string permissionCode, string permissionName = null, bool ignoreAdministrator = false, bool useBaseRole = true)
        {
            var result = false;

            // 2016-05-24 吉日嘎拉，若是Oracle数据库，用户的Id目前都是数值类型，若不是数字就出错了，不用进行运算了。
            if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
            {
                if (!ValidateUtil.IsInt(userId))
                {
                    return result;
                }
            }

            // 忽略超级管理员，这里判断拒绝权限用的，因为超级管理员也被拒绝了，若所有的权限都有了
            if (!ignoreAdministrator)
            {
                // 先判断用户类别
                if (UserInfo != null && BaseUserManager.IsAdministrator(UserInfo.Id))
                {
                    return true;
                }
            }

            // string permissionId = moduleManager.GetIdByAdd(permissionCode, permissionName);
            // 没有找到相应的权限
            // if (string.IsNullOrEmpty(permissionId))
            //{
            //    return false;
            //}

            var permissionEntity = BaseModuleManager.GetEntityByCacheByCode(systemCode, permissionCode);
            // 没有找到这个权限
            if (permissionEntity == null)
            {
                return false;
            }
            // 若是公开的权限，就不用进行判断了
            if (permissionEntity.IsPublic == 1)
            {
                return true;
            }
            if (permissionEntity.Enabled == 0)
            {
                return false;
            }
            if (!ignoreAdministrator)
            {
                // 这里需要判断,是系统权限？（系统管理员？）
                /*
                BaseUserManager userManager = new BaseUserManager(this.DbHelper, this.UserInfo);
                if (!string.IsNullOrEmpty(permissionEntity.CategoryCode) && permissionEntity.CategoryCode.Equals("System"))
                {
                    // 用户管理员
                    result = userManager.IsInRoleByCode(userId, "System");
                    if (result)
                    {
                        return result;
                    }
                }
                // 这里需要判断,是业务权限？(业务管理员？)
                if (!string.IsNullOrEmpty(permissionEntity.CategoryCode) && permissionEntity.CategoryCode.Equals("Application"))
                {
                    result = userManager.IsInRoleByCode(userId, "Application");
                    if (result)
                    {
                        return result;
                    }
                }
                */
            }

            // 判断用户权限
            if (CheckResourcePermission(systemCode, BaseUserEntity.TableName, userId, permissionEntity.Id))
            {
                return true;
            }

            // 判断用户角色权限
            if (CheckUserRolePermission(systemCode, userId, permissionEntity.Id, useBaseRole))
            {
                return true;
            }

            // 判断用户组织机构权限，这里有开关是为了提高性能用的，
            // 下面的函数接着还可以提高性能，可以进行一次判断就可以了，其实不用执行4次判断，浪费I/O，浪费性能。
            if (BaseSystemInfo.UseOrganizationPermission)
            {
                // 2016-02-26 吉日嘎拉 进行简化权限判断，登录时应该选系统，选公司比较好，登录到哪个公司应该先确定？
                var companyId = BaseUserManager.GetCompanyIdByCache(userId);
                if (!string.IsNullOrEmpty(companyId))
                {
                    if (CheckResourcePermission(systemCode, BaseOrganizationEntity.TableName, companyId, permissionEntity.Id))
                    {
                        return true;
                    }
                }

                // 这里获取用户的所有所在的部门，包括兼职的部门
                /*
                BaseUserManager userManager = new BaseUserManager(this.DbHelper, this.UserInfo);
                string[] organizationIds = userManager.GetAllOrganizationIds(userId);
                if (organizationIds != null
                    && organizationIds.Length > 0
                    && this.CheckUserOrganizationPermission(systemCode, userId, permissionEntity.Id, organizationIds))
                {
                    return true;
                }
                */
            }

            return false;
        }
        #endregion

        private bool CheckUserOrganizationPermission(string systemCode, string userId, string permissionId, string[] organizationIds)
        {
            var result = false;

            var errorMark = 0;
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }
            if (string.IsNullOrEmpty(permissionId))
            {
                return false;
            }
            if (organizationIds == null || organizationIds.Length == 0)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(systemCode))
            {
                if (UserInfo != null && !string.IsNullOrWhiteSpace(UserInfo.SystemCode))
                {
                    systemCode = UserInfo.SystemCode;
                }
            }

            var tableName = systemCode + "Permission";
            var ids = "(" + string.Join(",", organizationIds) + ")";
            var sql = "SELECT COUNT(*) "
                             + " FROM " + tableName
                             + "  WHERE " + BasePermissionEntity.FieldResourceCategory + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldResourceCategory)
                             + "        AND (" + BasePermissionEntity.FieldResourceId + " IN " + ids + ") "
                             + "        AND " + BasePermissionEntity.FieldPermissionId + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldPermissionId)
                             + "        AND " + BasePermissionEntity.FieldEnabled + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldEnabled)
                             + "        AND " + BasePermissionEntity.FieldDeleted + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldDeleted);

            var rowCount = 0;
            var dbParameters = new List<IDbDataParameter>
            {
                DbHelper.MakeParameter(BasePermissionEntity.FieldResourceCategory, BaseOrganizationEntity.TableName),
                DbHelper.MakeParameter(BasePermissionEntity.FieldPermissionId, permissionId),
                DbHelper.MakeParameter(BasePermissionEntity.FieldEnabled, 1),
                DbHelper.MakeParameter(BasePermissionEntity.FieldDeleted, 0)
            };

            try
            {
                errorMark = 1;
                var returnObject = DbHelper.ExecuteScalar(sql, dbParameters.ToArray());

                if (returnObject != null)
                {
                    rowCount = int.Parse(returnObject.ToString());
                }
                result = rowCount > 0;
            }
            catch (Exception ex)
            {
                var writeMessage = "BasePermissionManager.CheckUserOrganizationPermission:发生时间:" + DateTime.Now
                    + Environment.NewLine + "errorMark = " + errorMark
                    + Environment.NewLine + "Message:" + ex.Message
                    + Environment.NewLine + "Source:" + ex.Source
                    + Environment.NewLine + "StackTrace:" + ex.StackTrace
                    + Environment.NewLine + "TargetSite:" + ex.TargetSite
                    + Environment.NewLine;

                LogUtil.WriteLog(writeMessage, "Exception");
            }

            return result;
        }

        #region public bool CheckPermissionByUser(string systemCode, string userId, string permissionCode)
        /// <summary>
        /// 直接看用户本身是否有这个权限（不管角色是否有权限）
        /// </summary>
        /// <param name="systemCode">系统</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限主键</param>
        /// <returns>是否有权限</returns>
        public bool CheckPermissionByUser(string systemCode, string userId, string permissionCode)
        {
            var permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
            // 没有找到相应的权限
            if (string.IsNullOrEmpty(permissionId))
            {
                return false;
            }
            return CheckResourcePermission(systemCode, BaseUserEntity.TableName, userId, permissionId);
        }
        #endregion

        private bool CheckResourcePermission(string systemCode, string resourceCategory, string resourceId, string permissionId)
        {
            var result = false;

            var errorMark = 0;

            /*
            string tableName = systemCode + "Permission";
            string sql = "SELECT COUNT(*) "
                             + " FROM " + tableName
                             + "  WHERE " + BasePermissionEntity.FieldResourceCategory + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldResourceCategory)
                             + "        AND " + BasePermissionEntity.FieldResourceId + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldResourceId)
                             + "        AND " + BasePermissionEntity.FieldPermissionId + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldPermissionId)
                             + "        AND " + BasePermissionEntity.FieldEnabled + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldEnabled)
                             + "        AND " + BasePermissionEntity.FieldDeleted + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldDeleted);

            // 2016-02-26 吉日嘎拉 若是 mysql 等数据库，可以限制只获取一条就可以了，这个语句有优化的潜力，不需要获取所有，只要存在就可以了，判断是否存在就可以了

            int rowCount = 0;
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            dbParameters.Add(DbHelper.MakeParameter(BasePermissionEntity.FieldResourceCategory, resourceCategory));
            dbParameters.Add(DbHelper.MakeParameter(BasePermissionEntity.FieldResourceId, resourceId));
            dbParameters.Add(DbHelper.MakeParameter(BasePermissionEntity.FieldPermissionId, permissionId));
            dbParameters.Add(DbHelper.MakeParameter(BasePermissionEntity.FieldEnabled, 1));
            dbParameters.Add(DbHelper.MakeParameter(BasePermissionEntity.FieldDeleted, 0));
            errorMark = 1;
            */

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, resourceCategory),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, resourceId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldPermissionId, permissionId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 0)
            };

            try
            {
                // 2016-02-27 吉日嘎拉 提高数据库查询效率，不需要全表扫描，提高判断权限的效率
                CurrentTableName = systemCode + "Permission";
                var id = GetProperty(parameters, BasePermissionEntity.FieldId);
                result = !string.IsNullOrEmpty(id);

                //object returnObject = this.DbHelper.ExecuteScalar(sql, dbParameters.ToArray());
                //if (returnObject != null && returnObject != DBNull.Value)
                //{
                //    rowCount = int.Parse(returnObject.ToString());
                //}
                //result = rowCount > 0;
            }
            catch (Exception ex)
            {
                var writeMessage = "BasePermissionManager.CheckResourcePermission:发生时间:" + DateTime.Now
                    + Environment.NewLine + "errorMark = " + errorMark
                    + Environment.NewLine + "Message:" + ex.Message
                    + Environment.NewLine + "Source:" + ex.Source
                    + Environment.NewLine + "StackTrace:" + ex.StackTrace
                    + Environment.NewLine + "TargetSite:" + ex.TargetSite
                    + Environment.NewLine;

                LogUtil.WriteLog(writeMessage, "Exception");
            }

            return result;
        }

        #region private bool CheckUserRolePermission(string systemCode, string userId, string permissionId, bool useBaseRole = true)
        /// <summary>
        /// 用户角色关系是否有模块权限
        /// 2015-11-29 吉日嘎拉 进行参数化改进。
        /// 2016-02-26 吉日嘎拉 优化索引的顺序。
        /// </summary>
        /// <param name="systemCode">数据库连接</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionId">权限主键</param>
        /// <param name="useBaseRole">基础角色是否包含</param>
        /// <returns>有角色权限</returns>
        private bool CheckUserRolePermission(string systemCode, string userId, string permissionId, bool useBaseRole = true)
        {
            var result = false;

            var errorMark = 0;
            var permissionTableName = "BasePermission";
            var userRoleTableName = "BaseUserRole";
            var roleTableName = "BaseRole";

            if (string.IsNullOrWhiteSpace(systemCode))
            {
                if (UserInfo != null && !string.IsNullOrWhiteSpace(UserInfo.SystemCode))
                {
                    systemCode = UserInfo.SystemCode;
                }
            }

            permissionTableName = systemCode + "Permission";
            userRoleTableName = systemCode + "UserRole";
            roleTableName = systemCode + "Role";

            var dbParameters = new List<IDbDataParameter>();

            var sql = "SELECT COUNT(*) "
                            + " FROM " + permissionTableName
                            + "  WHERE " + BasePermissionEntity.FieldResourceCategory + " = '" + roleTableName + "'"
                            + "        AND " + BasePermissionEntity.FieldResourceId + " IN ( "
                                                + " SELECT " + BaseUserRoleEntity.FieldRoleId
                                                + " FROM " + userRoleTableName
                                                + "  WHERE " + BaseUserRoleEntity.FieldUserId + " = " + DbHelper.GetParameter(userRoleTableName + "_" + BaseUserRoleEntity.FieldUserId)
                                                + "        AND " + BaseUserRoleEntity.FieldEnabled + " = 1 "
                                                + "        AND " + BaseUserRoleEntity.FieldDeleted + " = 0 ";

            dbParameters.Add(DbHelper.MakeParameter(userRoleTableName + "_" + BaseUserRoleEntity.FieldUserId, userId));
            if (useBaseRole && !systemCode.Equals("Base", StringComparison.OrdinalIgnoreCase))
            {
                sql += " UNION SELECT " + BaseUserRoleEntity.FieldRoleId
                                + " FROM " + BaseUserRoleEntity.TableName
                                + "  WHERE " + BaseUserRoleEntity.FieldUserId + " = " + DbHelper.GetParameter(BaseUserRoleEntity.TableName + "_" + BaseUserRoleEntity.FieldUserId)
                                + "        AND " + BaseUserRoleEntity.FieldEnabled + " = 1 "
                                + "        AND " + BaseUserRoleEntity.FieldDeleted + " = 0 ";

                dbParameters.Add(DbHelper.MakeParameter(BaseUserRoleEntity.TableName + "_" + BaseUserRoleEntity.FieldUserId, userId));
            }
            sql += " ) "
                + "        AND " + BasePermissionEntity.FieldPermissionId + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldPermissionId)
                + "        AND " + BasePermissionEntity.FieldEnabled + " = 1 "
                + "        AND " + BasePermissionEntity.FieldDeleted + " = 0 ";
            dbParameters.Add(DbHelper.MakeParameter(BasePermissionEntity.FieldPermissionId, permissionId));

            var rowCount = 0;

            LogUtil.WriteLog("sql:" + sql, "Log");
            try
            {
                errorMark = 1;
                var returnObject = DbHelper.ExecuteScalar(sql, dbParameters.ToArray());
                if (returnObject != null)
                {
                    rowCount = int.Parse(returnObject.ToString());
                }
                result = rowCount > 0;
            }
            catch (Exception ex)
            {
                var writeMessage = "BasePermissionManager.CheckUserRolePermission:发生时间:" + DateTime.Now
                    + Environment.NewLine + "errorMark = " + errorMark
                    + Environment.NewLine + "Message:" + ex.Message
                    + Environment.NewLine + "Source:" + ex.Source
                    + Environment.NewLine + "StackTrace:" + ex.StackTrace
                    + Environment.NewLine + "TargetSite:" + ex.TargetSite
                    + Environment.NewLine;

                LogUtil.WriteLog(writeMessage, "Exception");
            }

            return result;
        }
        #endregion

        //
        // 从数据库获取权限
        //
        /// <summary>
        /// 获取清单
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="userId"></param>
        /// <param name="companyId"></param>
        /// <param name="fromCache"></param>
        /// <returns></returns>
        public List<BaseModuleEntity> GetPermissionListByUser(string systemCode, string userId, string companyId = null, bool fromCache = false)
        {
            var result = new List<BaseModuleEntity>();

            // 2016-05-24 吉日嘎拉，若是Oracle数据库，用户的Id目前都是数值类型，若不是数字就出错了，不用进行运算了。
            if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
            {
                if (!ValidateUtil.IsInt(userId))
                {
                    return result;
                }
            }

            var useBaseRole = false;

            var key = "BaseModule";
            var tableName = "BaseModule";
            if (!string.IsNullOrWhiteSpace(systemCode))
            {
                key = systemCode + "Module";
                tableName = systemCode + "Module";

                // 2015-11-19 所有的系统都继承基础角色的权限
                useBaseRole = true;
                // 2015-01-21 吉日嘎拉，实现判断别人的权限，是否超级管理员
                var isAdministrator = false;

                if (UserInfo != null && BaseUserManager.IsAdministrator(UserInfo.Id))
                {
                    if (UserInfo.Id.Equals(userId, StringComparison.CurrentCulture))
                    {
                        isAdministrator = true;
                    }
                    else
                    {
                        isAdministrator = BaseUserManager.IsAdministrator(userId);
                    }
                }
                if (isAdministrator)
                {
                    result = BaseModuleManager.GetEntitiesByCache(systemCode);
                }
                else
                {
                    string[] permissionIds = null;
                    // 2016-02-26 吉日嘎拉进行优化，用缓存与不用缓存感觉区别不是很大。
                    if (fromCache)
                    {
                        // permissionIds = GetPermissionIdsByUserByCache(systemCode, userId, companyId, useBaseRole);
                        permissionIds = GetPermissionIdsByUser(systemCode, userId, companyId, false, useBaseRole);
                    }
                    else
                    {
                        permissionIds = GetPermissionIdsByUser(systemCode, userId, companyId, false, useBaseRole);
                    }

                    // 2016-03-02 吉日嘎拉，少读一次缓存服务器，减少缓存服务器读写压力
                    var entities = BaseModuleManager.GetEntitiesByCache(systemCode);
                    // 若是以前赋予的权限，后来有些权限设置为无效了，那就不应该再获取哪些无效的权限才对。
                    if (permissionIds != null && permissionIds.Length > 0)
                    {   
                        result = (entities as List<BaseModuleEntity>).Where(t => (t.IsPublic == 1 && t.Enabled == 1 && t.DeletionStateCode == 0) || permissionIds.Contains(t.Id)).ToList();
                    }
                    else
                    {
                        result = (entities as List<BaseModuleEntity>).Where(t => t.IsPublic == 1 && t.Enabled == 1 && t.DeletionStateCode == 0).ToList();
                    }
                }
            }

            return result;
        }

        #region public string[] GetPermissionIds(BaseUserInfo userInfo)
        /// <summary>
        /// 获得一个员工的某一模块的权限
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <returns>数据表</returns>
        public string[] GetPermissionIds(BaseUserInfo userInfo)
        {
            return GetPermissionIdsByUser(userInfo.Id, userInfo.CompanyId);
        }
        #endregion

        /// <summary>
        /// 获取用户的权限主键数组
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="containPublic">公开的也包含</param>
        /// <param name="useBaseRole">使用基础角色权限</param>
        /// <returns>拥有权限数组</returns>
        public string[] GetPermissionIdsByUser(string systemCode, string userId, string companyId = null, bool containPublic = true, bool useBaseRole = false)
        {
            // 公开的操作权限需要计算
            string[] result = null;

            var errorMark = 0;
            var tableName = BaseModuleEntity.TableName;
            if (string.IsNullOrWhiteSpace(systemCode))
            {
                systemCode = "Base";
            }
            // 就不需要参合基础的角色了
            if (systemCode.Equals("Base"))
            {
                useBaseRole = false;
            }
            tableName = systemCode + "Module";

            try
            {
                errorMark = 1;

                if (containPublic)
                {
                    // 把公开的部分获取出来（把公开的主键数组从缓存里获取出来，减少数据库的读取次数）
                    var moduleEntities = BaseModuleManager.GetEntitiesByCache(systemCode);
                    if (moduleEntities != null)
                    {
                        result = moduleEntities.Where((t => t.IsPublic == 1 && t.Enabled == 1 && t.DeletionStateCode == 0)).Select(t => t.Id.ToString()).ToArray();
                    }
                }

                tableName = systemCode + "UserRole";
                var roleTableName = systemCode + "Role";
                CurrentTableName = systemCode + "Permission";
                var dbParameters = new List<IDbDataParameter>();

                var sb = Pool.StringBuilder.Get();
                // 用户的操作权限
                sb.Append("SELECT " + BasePermissionEntity.FieldPermissionId);
                sb.Append(" FROM " + CurrentTableName);
                sb.Append(" WHERE (" + BasePermissionEntity.FieldResourceCategory + " = " + DbHelper.GetParameter(BaseUserEntity.TableName + "_" + BasePermissionEntity.FieldResourceCategory));
                sb.Append(" AND " + BasePermissionEntity.FieldResourceId + " = " + DbHelper.GetParameter(BaseUserEntity.TableName + "_" + BaseUserEntity.FieldId));
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1 ");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0)");

                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.TableName + "_" + BasePermissionEntity.FieldResourceCategory, BaseUserEntity.TableName));
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.TableName + "_" + BaseUserEntity.FieldId, userId));

                // 角色的操作权限                            
                sb.Append(" UNION ");

                sb.Append("SELECT " + BasePermissionEntity.FieldPermissionId);
                sb.Append(" FROM " + CurrentTableName);
                sb.Append(" , ( SELECT " + BaseUserRoleEntity.FieldRoleId);
                sb.Append(" FROM " + tableName);
                sb.Append(" WHERE (" + BaseUserRoleEntity.FieldUserId + " = " + DbHelper.GetParameter(BaseUserRoleEntity.TableName + "_" + BaseUserRoleEntity.FieldUserId));
                sb.Append(" AND " + BaseUserRoleEntity.FieldEnabled + " = 1 ");
                sb.Append(" AND " + BaseUserRoleEntity.FieldDeleted + " = 0 ) ");

                dbParameters.Add(DbHelper.MakeParameter(BaseUserRoleEntity.TableName + "_" + BaseUserRoleEntity.FieldUserId, userId));

                // 2015-12-02 吉日嘎拉 简化SQL语句，提高效率
                if (useBaseRole && !systemCode.Equals("Base", StringComparison.OrdinalIgnoreCase))
                {
                    // 是否使用基础角色的权限 
                    sb.Append(" UNION SELECT " + BaseUserRoleEntity.FieldRoleId);
                    sb.Append(" FROM " + BaseUserRoleEntity.TableName);
                    sb.Append(" WHERE ( " + BaseUserRoleEntity.FieldUserId + " = " + DbHelper.GetParameter(BaseUserRoleEntity.TableName + "_USEBASE_" + BaseUserRoleEntity.FieldUserId));
                    sb.Append(" AND " + BaseUserRoleEntity.FieldEnabled + " = 1 ");
                    sb.Append(" AND " + BaseUserRoleEntity.FieldDeleted + " = 0 ) ");

                    dbParameters.Add(DbHelper.MakeParameter(BaseUserRoleEntity.TableName + "_USEBASE_" + BaseUserRoleEntity.FieldUserId, userId));
                }

                /*
                // 角色与部门是否进行关联？
                // 2015-12-02 吉日嘎拉 这里基本上没在用的，心里有个数。
                if (BaseSystemInfo.UseRoleOrganization && !string.IsNullOrEmpty(companyId))
                {
                    string roleOrganizationTableName = systemCode + "RoleOrganization";
                    sql.Append(" UNION SELECT " + BaseRoleOrganizationEntity.FieldRoleId);
                    sql.Append(" FROM " + roleOrganizationTableName);
                    sql.Append(" WHERE ( " + BaseRoleOrganizationEntity.FieldOrganizationId + " = " + DbHelper.GetParameter(BaseRoleOrganizationEntity.FieldOrganizationId));
                    sql.Append(" AND " + BaseRoleOrganizationEntity.FieldEnabled + " = 1 ");
                    sql.Append(" AND " + BaseRoleOrganizationEntity.FieldDeleted + " = 0 )");
                    dbParameters.Add(DbHelper.MakeParameter(BaseRoleOrganizationEntity.FieldOrganizationId, companyId));
                }
                */

                sb.Append(") B ");
                sb.Append("   WHERE " + BasePermissionEntity.FieldResourceCategory + " = " + DbHelper.GetParameter(BaseRoleEntity.TableName + "_" + BasePermissionEntity.FieldResourceCategory));
                sb.Append("        AND " + CurrentTableName + "." + BasePermissionEntity.FieldResourceId + " = B." + BaseUserRoleEntity.FieldRoleId);
                sb.Append("        AND " + CurrentTableName + "." + BasePermissionEntity.FieldEnabled + " = 1 ");
                sb.Append("        AND " + CurrentTableName + "." + BasePermissionEntity.FieldDeleted + " = 0 ");

                dbParameters.Add(DbHelper.MakeParameter(BaseRoleEntity.TableName + "_" + BasePermissionEntity.FieldResourceCategory, roleTableName));

                var ids = new List<string>();
                errorMark = 3;
                using (var dataReader = DbHelper.ExecuteReader(sb.Put(), dbParameters.ToArray()))
                {
                    while (dataReader.Read())
                    {
                        ids.Add(dataReader[BasePermissionEntity.FieldPermissionId].ToString());
                    }
                }

                // string[] userRolePermissionIds = ids.ToArray();
                result = StringUtil.Concat(result, ids.ToArray());

                // 按部门(组织机构)获取权限项
                if (BaseSystemInfo.UseOrganizationPermission)
                {
                    if (!string.IsNullOrEmpty(companyId))
                    {
                        sb.Clear();
                        sb.Append("SELECT " + BasePermissionEntity.FieldPermissionId);
                        sb.Append(" FROM " + CurrentTableName);
                        sb.Append(" WHERE " + BasePermissionEntity.FieldResourceCategory + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldResourceCategory));
                        sb.Append(" AND " + BasePermissionEntity.FieldPermissionId + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldPermissionId));
                        sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldEnabled));
                        sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldDeleted));
                        // dt = DbHelper.Fill(sql);
                        // string[] organizePermission = BaseUtil.FieldToArray(dt, BasePermissionEntity.FieldPermissionId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
                        // 2015-12-02 吉日嘎拉 优化参数，用ExecuteReader，提高效率节约内存。
                        dbParameters = new List<IDbDataParameter>
                        {
                            DbHelper.MakeParameter(BasePermissionEntity.FieldResourceCategory, BaseOrganizationEntity.TableName),
                            DbHelper.MakeParameter(BasePermissionEntity.FieldPermissionId, companyId),
                            DbHelper.MakeParameter(BasePermissionEntity.FieldEnabled, 1),
                            DbHelper.MakeParameter(BasePermissionEntity.FieldDeleted, 0)
                        };
                        ids = new List<string>();
                        errorMark = 4;
                        using (var dataReader = DbHelper.ExecuteReader(sb.Put(), dbParameters.ToArray()))
                        {
                            while (dataReader.Read())
                            {
                                ids.Add(dataReader[BasePermissionEntity.FieldPermissionId].ToString());
                            }
                        }
                        // string[] organizePermission = ids.ToArray();
                        result = StringUtil.Concat(result, ids.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                var writeMessage = "BasePermissionManager.GetPermissionIdsByUser:发生时间:" + DateTime.Now
                    + Environment.NewLine + "errorMark = " + errorMark
                    + Environment.NewLine + "Message:" + ex.Message
                    + Environment.NewLine + "Source:" + ex.Source
                    + Environment.NewLine + "StackTrace:" + ex.StackTrace
                    + Environment.NewLine + "TargetSite:" + ex.TargetSite
                    + Environment.NewLine;

                LogUtil.WriteLog(writeMessage, "Exception");
            }

            return result;
        }

        #region public bool CheckPermissionByRole(string systemCode, string roleId, string permissionCode)
        /// <summary>
        /// 用户角色关系是否有模块权限
        /// 2015-12-15 吉日嘎拉 优化参数化
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>有角色权限</returns>
        public bool CheckPermissionByRole(string systemCode, string roleId, string permissionCode)
        {
            // 判断当前判断的权限是否存在，否则很容易出现前台设置了权限，后台没此项权限
            // 需要自动的能把前台判断过的权限，都记录到后台来
            var permissionId = string.Empty;
#if (DEBUG)
                if (string.IsNullOrEmpty(permissionId))
                {
                    BaseModuleEntity permissionEntity = new BaseModuleEntity();
                    permissionEntity.Code = permissionCode;
                    permissionEntity.FullName = permissionCode;
                    permissionEntity.IsScope = 0;
                    permissionEntity.IsPublic = 0;
                    permissionEntity.IsMenu = 0;
                    permissionEntity.IsVisible = 1;
                    permissionEntity.AllowDelete = 1;
                    permissionEntity.AllowEdit = 1;
                    permissionEntity.DeletionStateCode = 0;
                    permissionEntity.Enabled = 1;
                    // 这里是防止主键重复？
                    // permissionEntity.ID = BaseUtil.NewGuid();
                    BaseModuleManager moduleManager = new BaseModuleManager();
                    moduleManager.AddEntity(permissionEntity);
                }
                else
                {
                    // 更新最后一次访问日期，设置为当前服务器日期
                    SqlBuilder sqlBuilder = new SqlBuilder(DbHelper);
                    sqlBuilder.BeginUpdate(CurrentTableName);
                    sqlBuilder.SetDbNow(BaseModuleEntity.FieldLastCall);
                    sqlBuilder.SetWhere(BaseModuleEntity.FieldId, permissionId);
                    sqlBuilder.EndUpdate();
                }
#endif

            permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
            // 没有找到相应的权限
            if (string.IsNullOrEmpty(permissionId))
            {
                return false;
            }

            var resourceCategory = systemCode + "Role";
            return CheckResourcePermission(systemCode, resourceCategory, roleId, permissionId);
        }
        #endregion
    }
}