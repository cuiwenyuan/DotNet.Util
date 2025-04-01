//-----------------------------------------------------------------------
// <copyright file="BasePermissionManager.cs" company="DotNet">
//     Copyright (c) 2025, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

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
    public partial class BasePermissionManager : BaseManager
    {
        #region public override DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BasePermissionEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = false, bool showDeleted = false)
        /// <summary>
        /// 按条件分页查询(带记录状态Enabled和删除状态Deleted)
        /// </summary>
        /// <param name="companyId">查看公司主键</param>
        /// <param name="departmentId">查看部门主键</param>
        /// <param name="userId">查看用户主键</param>
        /// <param name="startTime">创建开始时间</param>
        /// <param name="endTime">创建结束时间</param>
        /// <param name="searchKey">查询关键字</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="showDisabled">是否显示无效记录</param>
        /// <param name="showDeleted">是否显示已删除记录</param>
        /// <returns>数据表</returns>
        public override DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BasePermissionEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            var sb = PoolUtil.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0");
            }

            if (ValidateUtil.IsInt(companyId))
            {
                //sb.Append(" AND " + BasePermissionEntity.FieldCompanyId + " = " + companyId);
            }
            // 只有管理员才能看到所有的
            //if (!(UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled))
            //{
            //sb.Append(" AND (" + BasePermissionEntity.FieldUserCompanyId + " = 0 OR " + BasePermissionEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            //}
            if (ValidateUtil.IsInt(departmentId))
            {
                //sb.Append(" AND " + BasePermissionEntity.FieldDepartmentId + " = " + departmentId);
            }
            if (ValidateUtil.IsInt(userId))
            {
                //sb.Append(" AND " + BasePermissionEntity.FieldUserId + " = " + userId);
            }
            //创建时间
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BasePermissionEntity.FieldCreateTime + " >= " + dbHelper.ToDbTime(startTime));
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BasePermissionEntity.FieldCreateTime + " <= " + dbHelper.ToDbTime(endTime.ToDateTime().Date.AddDays(1).AddMilliseconds(-1)));
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BasePermissionEntity.FieldPermissionId + " LIKE N'%" + searchKey + "%' OR " + BasePermissionEntity.FieldDescription + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", " ");
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Return());
        }
        #endregion

        #region 下拉菜单

        /// <summary>
        /// 下拉菜单
        /// </summary>
        /// <param name="myCompanyOnly">仅本公司</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(bool myCompanyOnly = true)
        {
            var sb = PoolUtil.StringBuilder.Get();
            if (myCompanyOnly)
            {
                //sb.Append("(" + BasePermissionEntity.FieldUserCompanyId + " = 0 OR " + BasePermissionEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            }
            //return GetDataTable(sb.Return(), null, new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 0));
            var companyId = string.IsNullOrEmpty(BaseSystemInfo.CustomerCompanyId) ? UserInfo.CompanyId : BaseSystemInfo.CustomerCompanyId;
            var cacheKey = "Dt." + CurrentTableName + "." + companyId + "." + (myCompanyOnly ? "1" : "0");
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            return CacheUtil.Cache<DataTable>(cacheKey, () => GetDataTable(sb.Return(), null, new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 0)), true, false, cacheTime);
        }
        #endregion

        #region public bool PermissionExists(string permissionId, string resourceCategory, string resourceId) 检查是否存在
        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="systemCode">子系统编码</param>
        /// <param name="permissionId">权限主键</param>
        /// <param name="resourceCategory">资源分类</param>
        /// <param name="resourceId">资源主键</param>
        /// <returns>是否存在</returns>      
        public bool PermissionExists(string systemCode, string permissionId, string resourceCategory, string resourceId)
        {
            var result = false;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldSystemCode, systemCode),
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
            if (!PermissionExists(permissionEntity.SystemCode, permissionEntity.PermissionId, permissionEntity.ResourceCategory, permissionEntity.ResourceId))
            {
                result = AddEntity(permissionEntity);
            }
            return result;
        }
        #endregion

        #region public bool IsAuthorized(string systemCode, string userId, string permissionCode, string permissionName = null, bool ignoreAdministrator = false) 是否有相应的权限
        /// <summary>
        /// 是否有相应的权限
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="permissionName">权限名称</param>
        /// <param name="ignoreAdministrator">忽略管理员默认权限</param>
        /// <returns>是否有权限</returns>
        public bool IsAuthorized(string systemCode, string userId, string permissionCode, string permissionName = null, bool ignoreAdministrator = false)
        {
            var result = false;

            if (!ValidateUtil.IsInt(userId))
            {
                return result;
            }

            // 忽略超级管理员，这里判断拒绝权限用的，因为超级管理员也被拒绝了，若所有的权限都有了
            if (!ignoreAdministrator)
            {
                // 先判断用户类别
                if (UserInfo != null && BaseUserManager.IsAdministrator(UserInfo.Id.ToString()))
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

            var permissionEntity = new BaseModuleManager().GetEntityByCacheByCode(systemCode, permissionCode);
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
            if (CheckResourcePermission(systemCode, BaseUserEntity.CurrentTableName, userId, permissionEntity.Id.ToString()))
            {
                return true;
            }

            // 判断用户角色权限
            if (CheckUserRolePermission(systemCode, userId, permissionEntity.Id.ToString()))
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
                    if (CheckResourcePermission(systemCode, BaseOrganizationEntity.CurrentTableName, companyId, permissionEntity.Id.ToString()))
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

        #region private bool CheckUserOrganizationPermission(string systemCode, string userId, string permissionId, string[] organizationIds)
        private bool CheckUserOrganizationPermission(string systemCode, string userId, string permissionId, string[] organizationIds)
        {
            var result = false;

            var errorMark = 0;
            if (!ValidateUtil.IsInt(userId))
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
                if (UserInfo != null && !UserInfo.SystemCode.IsNullOrEmpty() && !UserInfo.SystemCode.IsNullOrWhiteSpace())
                {
                    systemCode = UserInfo.SystemCode;
                    if (!BaseSystemInfo.SystemCode.IsNullOrEmpty() && !BaseSystemInfo.SystemCode.Equals(systemCode))
                    {
                        systemCode = BaseSystemInfo.SystemCode;
                    }
                }
            }

            var permissionTableName = GetPermissionTableName(systemCode);
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) FROM " + permissionTableName
                             + " WHERE " + BasePermissionEntity.FieldResourceCategory + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldResourceCategory)
                             + " AND " + BasePermissionEntity.FieldResourceId + " IN (" + StringUtil.ArrayToList(organizationIds) + ")"
                             + " AND " + BasePermissionEntity.FieldPermissionId + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldPermissionId)
                             + " AND " + BasePermissionEntity.FieldSystemCode + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldSystemCode)
                             + " AND " + BasePermissionEntity.FieldEnabled + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldEnabled)
                             + " AND " + BasePermissionEntity.FieldDeleted + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldDeleted));

            var rowCount = 0;
            var dbParameters = new List<IDbDataParameter>
            {
                DbHelper.MakeParameter(BasePermissionEntity.FieldSystemCode, systemCode),
                DbHelper.MakeParameter(BasePermissionEntity.FieldResourceCategory, BaseOrganizationEntity.CurrentTableName),
                DbHelper.MakeParameter(BasePermissionEntity.FieldPermissionId, permissionId),
                DbHelper.MakeParameter(BasePermissionEntity.FieldEnabled, 1),
                DbHelper.MakeParameter(BasePermissionEntity.FieldDeleted, 0)
            };

            try
            {
                errorMark = 1;
                var obj = DbHelper.ExecuteScalar(sb.Return(), dbParameters.ToArray());

                if (obj != null)
                {
                    rowCount = obj.ToInt();
                }
                result = rowCount > 0;
            }
            catch (Exception ex)
            {
                var exception = "BasePermissionManager.CheckUserOrganizationPermission:发生时间:" + DateTime.Now
                    + Environment.NewLine + "errorMark = " + errorMark
                    + Environment.NewLine + "Message:" + ex.Message
                    + Environment.NewLine + "Source:" + ex.Source
                    + Environment.NewLine + "StackTrace:" + ex.StackTrace
                    + Environment.NewLine + "TargetSite:" + ex.TargetSite
                    + Environment.NewLine;

                LogUtil.WriteLog(exception, "Exception");
            }

            return result;
        }

        #endregion

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
            var permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);
            // 没有找到相应的权限
            if (string.IsNullOrEmpty(permissionId))
            {
                return false;
            }
            return CheckResourcePermission(systemCode, BaseUserEntity.CurrentTableName, userId.ToString(), permissionId);
        }
        #endregion

        #region private bool CheckResourcePermission(string systemCode, string resourceCategory, string resourceId, string permissionId)
        private bool CheckResourcePermission(string systemCode, string resourceCategory, string resourceId, string permissionId)
        {
            var result = false;

            var errorMark = 0;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldSystemCode, systemCode),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, resourceCategory),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, resourceId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldPermissionId, permissionId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 0)
            };

            try
            {
                // 2016-02-27 吉日嘎拉 提高数据库查询效率，不需要全表扫描，提高判断权限的效率
                CurrentTableName = GetPermissionTableName(systemCode);
                var id = GetProperty(parameters, BasePermissionEntity.FieldId);
                result = !string.IsNullOrEmpty(id);
            }
            catch (Exception ex)
            {
                var exception = "BasePermissionManager.CheckResourcePermission:发生时间:" + DateTime.Now
                    + Environment.NewLine + "errorMark = " + errorMark
                    + Environment.NewLine + "Message:" + ex.Message
                    + Environment.NewLine + "Source:" + ex.Source
                    + Environment.NewLine + "StackTrace:" + ex.StackTrace
                    + Environment.NewLine + "TargetSite:" + ex.TargetSite
                    + Environment.NewLine;

                LogUtil.WriteLog(exception, "Exception");
            }

            return result;
        }
        #endregion

        #region private bool CheckUserRolePermission(string systemCode, string userId, string permissionId)
        /// <summary>
        /// 用户角色关系是否有模块权限
        /// 2015-11-29 吉日嘎拉 进行参数化改进。
        /// 2016-02-26 吉日嘎拉 优化索引的顺序。
        /// </summary>
        /// <param name="systemCode">数据库连接</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>有角色权限</returns>
        private bool CheckUserRolePermission(string systemCode, string userId, string permissionId)
        {
            var result = false;

            var errorMark = 0;

            var permissionTableName = GetPermissionTableName(systemCode);
            var userRoleTableName = GetUserRoleTableName(systemCode);
            var roleTableName = GetRoleTableName(systemCode);

            var dbParameters = new List<IDbDataParameter>();
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) FROM " + permissionTableName
                            + " WHERE " + BasePermissionEntity.FieldResourceCategory + " = '" + roleTableName + "'"
                            + " AND " + BasePermissionEntity.FieldResourceId + " IN ( "
                                                + " SELECT " + BaseUserRoleEntity.FieldRoleId
                                                + " FROM " + userRoleTableName
                                                + " WHERE " + BaseUserRoleEntity.FieldUserId + " = " + DbHelper.GetParameter(userRoleTableName + "_" + BaseUserRoleEntity.FieldUserId)
                                                + " AND " + BaseUserRoleEntity.FieldEnabled + " = 1"
                                                + " AND " + BaseUserRoleEntity.FieldDeleted + " = 0");

            dbParameters.Add(DbHelper.MakeParameter(userRoleTableName + "_" + BaseUserRoleEntity.FieldUserId, userId));
            sb.Append(" ) "
                + " AND " + BasePermissionEntity.FieldPermissionId + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldPermissionId)
                + " AND " + BasePermissionEntity.FieldEnabled + " = 1"
                + " AND " + BasePermissionEntity.FieldDeleted + " = 0");
            dbParameters.Add(DbHelper.MakeParameter(BasePermissionEntity.FieldPermissionId, permissionId));

            var rowCount = 0;

            try
            {
                errorMark = 1;
                var obj = DbHelper.ExecuteScalar(sb.Return(), dbParameters.ToArray());
                if (obj != null)
                {
                    rowCount = obj.ToInt();
                }
                result = rowCount > 0;
            }
            catch (Exception ex)
            {
                var exception = "BasePermissionManager.CheckUserRolePermission:发生时间:" + DateTime.Now
                    + Environment.NewLine + "errorMark = " + errorMark
                    + Environment.NewLine + "Message:" + ex.Message
                    + Environment.NewLine + "Source:" + ex.Source
                    + Environment.NewLine + "StackTrace:" + ex.StackTrace
                    + Environment.NewLine + "TargetSite:" + ex.TargetSite
                    + Environment.NewLine;

                LogUtil.WriteLog(exception, "Exception");
            }

            return result;
        }
        #endregion

        #region public List<BaseModuleEntity> GetPermissionListByUser(string systemCode, string userId, string companyId = null, bool fromCache = false)
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

            if (!ValidateUtil.IsInt(userId))
            {
                return result;
            }

            var key = GetModuleTableName(systemCode);
            var moduleTableName = GetModuleTableName(systemCode);

            var isAdministrator = BaseUserManager.IsAdministrator(userId);

            if (isAdministrator)
            {
                result = new BaseModuleManager().GetEntitiesByCache(systemCode);
            }
            else
            {
                string[] permissionIds = null;
                permissionIds = GetPermissionIdsByUser(systemCode, userId, companyId: companyId, containPublic: true);

                // 2016-03-02 吉日嘎拉，少读一次缓存服务器，减少缓存服务器读写压力
                var entities = new BaseModuleManager().GetEntitiesByCache(systemCode);
                // 若是以前赋予的权限，后来有些权限设置为无效了，那就不应该再获取哪些无效的权限才对。
                if (permissionIds != null && permissionIds.Length > 0 && entities != null)
                {
                    // 要特别注意IsPublic的设置，容易造成失控
                    result = entities.Where(t => (t.IsPublic == 1 && t.Enabled == 1 && t.Deleted == 0) || permissionIds.Contains(t.Id.ToString())).ToList();
                }
                else
                {
                    result = entities.Where(t => t.IsPublic == 1 && t.Enabled == 1 && t.Deleted == 0).ToList();
                }
            }

            return result;
        }

        #endregion

        #region public string[] GetPermissionIds(BaseUserInfo userInfo)
        /// <summary>
        /// 获得一个员工的某一模块的权限
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <returns>数据表</returns>
        public string[] GetPermissionIds(BaseUserInfo userInfo)
        {
            return GetPermissionIdsByUser(BaseSystemInfo.SystemCode, userInfo.Id.ToString(), companyId: userInfo.CompanyId);
        }
        #endregion

        #region public string[] GetPermissionIdsByUser(string systemCode, string userId, string companyId = null, bool containPublic = true)
        /// <summary>
        /// 获取用户的权限主键数组
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="containPublic">公开的也包含</param>
        /// <returns>拥有权限数组</returns>
        public string[] GetPermissionIdsByUser(string systemCode, string userId, string companyId = null, bool containPublic = true)
        {
            // 公开的操作权限需要计算
            string[] result = null;

            var errorMark = 0;
            var moduleTableName = GetModuleTableName(systemCode);

            try
            {
                errorMark = 1;

                if (containPublic)
                {
                    // 把公开的部分获取出来（把公开的主键数组从缓存里获取出来，减少数据库的读取次数）
                    var moduleEntities = new BaseModuleManager().GetEntitiesByCache(systemCode);
                    if (moduleEntities != null)
                    {
                        result = moduleEntities.Where((t => t.IsPublic == 1 && t.Enabled == 1 && t.Deleted == 0)).Select(t => t.Id.ToString()).ToArray();
                    }
                }

                var userRoleTableName = GetUserRoleTableName(systemCode);
                var roleTableName = GetRoleTableName(systemCode);
                CurrentTableName = GetPermissionTableName(systemCode);
                var dbParameters = new List<IDbDataParameter>();

                var sb = PoolUtil.StringBuilder.Get();
                // 用户的操作权限
                sb.Append("SELECT " + BasePermissionEntity.FieldPermissionId);
                sb.Append(" FROM " + CurrentTableName);
                sb.Append(" WHERE " + BasePermissionEntity.FieldResourceCategory + " = " + DbHelper.GetParameter(BaseUserEntity.CurrentTableName + "_" + BasePermissionEntity.FieldResourceCategory));
                sb.Append(" AND " + BasePermissionEntity.FieldResourceId + " = " + DbHelper.GetParameter(BaseUserEntity.CurrentTableName + "_" + BaseUserEntity.FieldId));
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0");

                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.CurrentTableName + "_" + BasePermissionEntity.FieldResourceCategory, BaseUserEntity.CurrentTableName));
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.CurrentTableName + "_" + BaseUserEntity.FieldId, userId));

                // 角色的操作权限                            
                sb.Append(" UNION ");

                sb.Append("SELECT " + BasePermissionEntity.FieldPermissionId);
                sb.Append(" FROM " + CurrentTableName);
                sb.Append(" , ( SELECT " + BaseUserRoleEntity.FieldRoleId);
                sb.Append(" FROM " + userRoleTableName);
                sb.Append(" WHERE " + BaseUserRoleEntity.FieldUserId + " = " + DbHelper.GetParameter(BaseUserRoleEntity.CurrentTableName + "_" + BaseUserRoleEntity.FieldUserId));
                sb.Append(" AND " + BaseUserRoleEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BaseUserRoleEntity.FieldDeleted + " = 0");

                dbParameters.Add(DbHelper.MakeParameter(BaseUserRoleEntity.CurrentTableName + "_" + BaseUserRoleEntity.FieldUserId, userId));

                sb.Append(") B ");
                sb.Append(" WHERE " + BasePermissionEntity.FieldResourceCategory + " = " + DbHelper.GetParameter(BaseRoleEntity.CurrentTableName + "_" + BasePermissionEntity.FieldResourceCategory));
                sb.Append(" AND " + CurrentTableName + "." + BasePermissionEntity.FieldResourceId + " = B." + BaseUserRoleEntity.FieldRoleId);
                sb.Append(" AND " + CurrentTableName + "." + BasePermissionEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + CurrentTableName + "." + BasePermissionEntity.FieldDeleted + " = 0");

                dbParameters.Add(DbHelper.MakeParameter(BaseRoleEntity.CurrentTableName + "_" + BasePermissionEntity.FieldResourceCategory, roleTableName));

                var ids = new List<string>();
                errorMark = 3;
                var dataReader = DbHelper.ExecuteReader(sb.Return(), dbParameters.ToArray());
                if (dataReader != null && !dataReader.IsClosed)
                {
                    while (dataReader.Read())
                    {
                        ids.Add(dataReader[BasePermissionEntity.FieldPermissionId].ToString());
                    }

                    dataReader.Close();
                }

                result = StringUtil.Concat(result, ids.ToArray());

                // 按部门(组织机构)获取权限项
                if (BaseSystemInfo.UseOrganizationPermission)
                {
                    if (!string.IsNullOrEmpty(companyId))
                    {
                        sb = PoolUtil.StringBuilder.Get();
                        sb.Append("SELECT " + BasePermissionEntity.FieldPermissionId);
                        sb.Append(" FROM " + CurrentTableName);
                        sb.Append(" WHERE " + BasePermissionEntity.FieldResourceCategory + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldResourceCategory));
                        sb.Append(" AND " + BasePermissionEntity.FieldPermissionId + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldPermissionId));
                        sb.Append(" AND " + BasePermissionEntity.FieldSystemCode + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldSystemCode));
                        sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldEnabled));
                        sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = " + DbHelper.GetParameter(BasePermissionEntity.FieldDeleted));
                        // dt = DbHelper.Fill(sql);
                        // string[] organizationPermission = BaseUtil.FieldToArray(dt, BasePermissionEntity.FieldPermissionId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
                        // 2015-12-02 吉日嘎拉 优化参数，用ExecuteReader，提高效率节约内存。
                        dbParameters = new List<IDbDataParameter>
                        {
                            DbHelper.MakeParameter(BasePermissionEntity.FieldSystemCode, systemCode),
                            DbHelper.MakeParameter(BasePermissionEntity.FieldResourceCategory, BaseOrganizationEntity.CurrentTableName),
                            DbHelper.MakeParameter(BasePermissionEntity.FieldPermissionId, companyId),
                            DbHelper.MakeParameter(BasePermissionEntity.FieldEnabled, 1),
                            DbHelper.MakeParameter(BasePermissionEntity.FieldDeleted, 0)
                        };
                        ids = new List<string>();
                        errorMark = 4;
                        dataReader = DbHelper.ExecuteReader(sb.Return(), dbParameters.ToArray());
                        if (dataReader != null && !dataReader.IsClosed)
                        {
                            while (dataReader.Read())
                            {
                                ids.Add(dataReader[BasePermissionEntity.FieldPermissionId].ToString());
                            }

                            dataReader.Close();
                        }

                        result = StringUtil.Concat(result, ids.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                var exception = "BasePermissionManager.GetPermissionIdsByUser:发生时间:" + DateTime.Now
                    + Environment.NewLine + "errorMark = " + errorMark
                    + Environment.NewLine + "Message:" + ex.Message
                    + Environment.NewLine + "Source:" + ex.Source
                    + Environment.NewLine + "StackTrace:" + ex.StackTrace
                    + Environment.NewLine + "TargetSite:" + ex.TargetSite
                    + Environment.NewLine;

                LogUtil.WriteLog(exception, "Exception");
            }

            return result;
        }

        #endregion

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
                var permissionEntity = new BaseModuleEntity();
                permissionEntity.Code = permissionCode;
                permissionEntity.Name = permissionCode;
                permissionEntity.IsScope = 0;
                permissionEntity.IsPublic = 0;
                permissionEntity.IsMenu = 0;
                permissionEntity.IsVisible = 1;
                permissionEntity.AllowDelete = 1;
                permissionEntity.AllowEdit = 1;
                permissionEntity.Deleted = 0;
                permissionEntity.Enabled = 1;
                // 这里是防止主键重复？
                // permissionEntity.ID = BaseUtil.NewGuid();
                var moduleManager = new BaseModuleManager();
                moduleManager.AddEntity(permissionEntity);
            }
            else
            {
                // 更新最后一次访问日期，设置为当前服务器日期
                var sqlBuilder = new SqlBuilder(DbHelper);
                sqlBuilder.BeginUpdate(CurrentTableName);
                sqlBuilder.SetDbNow(BaseModuleEntity.FieldLastCall);
                sqlBuilder.SetWhere(BaseModuleEntity.FieldId, permissionId);
                sqlBuilder.EndUpdate();
            }
#endif

            permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);
            // 没有找到相应的权限
            if (string.IsNullOrEmpty(permissionId))
            {
                return false;
            }

            var resourceCategory = GetRoleTableName(systemCode);
            return CheckResourcePermission(systemCode, resourceCategory, roleId, permissionId);
        }
        #endregion
    }
}