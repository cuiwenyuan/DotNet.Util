//-----------------------------------------------------------------------
// <copyright file="BasePermissionScopeManager.cs" company="DotNet">
//     Copyright (c) 2024, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BasePermissionScopeManager
    /// 资源权限范围
    ///
    ///	修改记录
    /// 
    ///     组织
    ///      ↓
    ///     角色 → 组织
    ///      ↓
    ///     用户
    ///     
    /// 
    ///     用户能有某种权限的所有员工      public string[] GetUserIds(int managerUserId, string permissionCode)
    ///                                     public string GetUserIdsSql(int managerUserId, string permissionCode)
    ///     
    ///     用户能有某种权限所有组织机构    public string[] GetOrganizationIds(int managerUserId, string permissionCode)
    ///                                     public string GetOrganizationIdsSql(int managerUserId, string permissionCode)
    ///     
    ///     用户能有某种权限的所有角色      public string[] GetAllRoleIds(int managerUserId, string permissionCode)
    ///                                     public string GetAllRoleIdsSql(int managerUserId, string permissionCode)
    ///     
    ///     2011.10.27 版本：4.3 张广梁 增加获得有效的委托列表的方法GetAuthoriedList。
    ///     2011.09.21 版本：4.2 张广梁 增加 public bool HasAuthorized(string[] names, object[] values,string startDate,string endDate)
    ///		2010.07.06 版本：4.1 JiRiGaLa permissionCode，result 修改为 permissionCode，result。
    ///		2007.03.03 版本：4.0 JiRiGaLa 核心的外部调用程序进行整理。
    ///		2007.03.03 版本：3.0 JiRiGaLa 调整主键的规范化。
    ///		2007.02.15 版本：2.0 JiRiGaLa 调整主键的规范化。
    ///	    2006.02.12 版本：1.2 JiRiGaLa 调整主键的规范化。
    ///     2005.08.14 版本：1.1 JiRiGaLa 添加了批量添加和批量删除。
    ///     2004.11.19 版本：1.0 JiRiGaLa 主键进行了绝对的优化，这是个好东西啊，平时要多用，用得要灵活些。
    ///     
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2007.03.03</date>
    /// </author>
    /// </summary>
    public partial class BasePermissionScopeManager : BaseManager
    {
        #region public override DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BasePermissionScopeEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = false, bool showDeleted = false)
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
        public override DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BasePermissionScopeEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            var sb = PoolUtil.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BasePermissionScopeEntity.FieldEnabled + " = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BasePermissionScopeEntity.FieldDeleted + " = 0");
            }

            if (ValidateUtil.IsInt(companyId))
            {
                //sb.Append(" AND " + BasePermissionScopeEntity.FieldCompanyId + " = " + companyId);
            }
            // 只有管理员才能看到所有的
            //if (!(UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled))
            //{
            //sb.Append(" AND (" + BasePermissionScopeEntity.FieldUserCompanyId + " = 0 OR " + BasePermissionScopeEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            //}
            if (ValidateUtil.IsInt(departmentId))
            {
                //sb.Append(" AND " + BasePermissionScopeEntity.FieldDepartmentId + " = " + departmentId);
            }
            if (ValidateUtil.IsInt(userId))
            {
                //sb.Append(" AND " + BasePermissionScopeEntity.FieldUserId + " = " + userId);
            }
            //创建时间
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BasePermissionScopeEntity.FieldCreateTime + " >= " + dbHelper.ToDbTime(startTime));
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BasePermissionScopeEntity.FieldCreateTime + " <= " + dbHelper.ToDbTime(endTime.ToDateTime().Date.AddDays(1).AddMilliseconds(-1)));
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BasePermissionScopeEntity.FieldPermissionId + " LIKE N'%" + searchKey + "%' OR " + BasePermissionScopeEntity.FieldDescription + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
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
                //sb.Append("(" + BasePermissionScopeEntity.FieldUserCompanyId + " = 0 OR " + BasePermissionScopeEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            }
            //return GetDataTable(sb.Return(), null, new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldDeleted, 0));
            var companyId = string.IsNullOrEmpty(BaseSystemInfo.CustomerCompanyId) ? UserInfo.CompanyId : BaseSystemInfo.CustomerCompanyId;
            var cacheKey = "Dt." + CurrentTableName + "." + companyId + "." + (myCompanyOnly ? "1" : "0");
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            return CacheUtil.Cache<DataTable>(cacheKey, () => GetDataTable(sb.Return(), null, new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldDeleted, 0)), true, false, cacheTime);
        }

        #endregion
        /// <summary>
        /// 是否按编号获得子节点，SQL2005以上或者Oracle数据库按ParentId,Id进行关联 
        /// </summary>
        public static bool UseGetChildrensByCode = false;

        #region public bool PermissionScopeExists(string result, string resourceCategory, string resourceId, string targetCategory, string targetId) 检查是否存在
        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="permissionId">权限主键</param>
        /// <param name="resourceCategory">资源分类</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="targetCategory">目标分类</param>
        /// <param name="targetId">目标主键</param>
        /// <returns>是否存在</returns>
        public bool PermissionScopeExists(string permissionId, string resourceCategory, string resourceId, string targetCategory, string targetId)
        {
            var result = true;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, resourceCategory),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, resourceId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, targetCategory),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, targetId)
            };

            // 检查是否存在
            if (!Exists(parameters))
            {
                result = false;
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 权限范围删除
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="resourceCategory"></param>
        /// <param name="resourceId"></param>
        /// <param name="targetCategory"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public int PermissionScopeDelete(string permissionId, string resourceCategory, string resourceId, string targetCategory, string targetId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, resourceCategory),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, resourceId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, targetCategory),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, targetId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId)
            };
            return Delete(parameters);
        }

        /// <summary>
        /// 添加权限
        /// </summary>
        /// <param name="resourceCategory"></param>
        /// <param name="resourceId"></param>
        /// <param name="targetCategory"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public string AddPermission(string resourceCategory, string resourceId, string targetCategory, string targetId)
        {
            var entity = new BasePermissionScopeEntity
            {
                ResourceCategory = resourceCategory,
                ResourceId = resourceId.ToInt(),
                TargetCategory = targetCategory,
                TargetId = targetId.ToInt(),
                Enabled = 1,
                Deleted = 0
            };
            return AddPermission(entity);
        }

        #region public string AddPermission(BasePermissionScopeEntity entity)
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">对象</param>
        /// <returns>主键</returns>
        public string AddPermission(BasePermissionScopeEntity entity)
        {
            var result = string.Empty;
            // 检查记录是否重复
            if (!PermissionScopeExists(entity.PermissionId.ToString(), entity.ResourceCategory, entity.ResourceId.ToString(), entity.TargetCategory, entity.TargetId.ToString()))
            {
                result = AddEntity(entity);
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 获得用户的权限范围设置
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="managerUserId">用户主键</param>
        /// <param name="permissionCode">权限范围编号</param>
        /// <returns>用户的权限范围</returns>
        public PermissionOrganizationScope GetUserPermissionScope(string systemCode, string managerUserId, string permissionCode)
        {
            var sql = GetOrganizationIdsSql(systemCode, managerUserId, permissionCode);
            var dt = DbHelper.Fill(sql);
            var organizationIds = BaseUtil.FieldToArray(dt, BasePermissionScopeEntity.FieldTargetId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
            return BaseUtil.GetPermissionScope(organizationIds);
        }

        // 权限范围的判断

        //
        // 获得被某个权限管理范围内 组织机构的 Id、SQL、List
        //

        #region public string GetOrganizationIdsSql(int managerUserId, string permissionCode) 按某个权限获取组织机构 Sql
        /// <summary>
        /// 按某个权限获取组织机构 Sql
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="managerUserId">管理用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>Sql</returns>
        public string GetOrganizationIdsSql(string systemCode, string managerUserId, string permissionCode)
        {
            var permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + BasePermissionScopeEntity.FieldTargetId
                     + " FROM " + BasePermissionScopeEntity.CurrentTableName
                     // 有效的，并且不为空的组织机构主键
                     + " WHERE (" + BasePermissionScopeEntity.FieldTargetCategory + " = '" + BaseOrganizationEntity.CurrentTableName + "') "
                     + " AND ( " + BasePermissionScopeEntity.CurrentTableName + "." + BasePermissionScopeEntity.FieldDeleted + " = 0) "
                     + " AND ( " + BasePermissionScopeEntity.CurrentTableName + "." + BasePermissionScopeEntity.FieldEnabled + " = 1) "
                     + " AND ( " + BasePermissionScopeEntity.CurrentTableName + "." + BasePermissionScopeEntity.FieldTargetId + " IS NOT NULL) "
                     // 自己直接由相应权限的组织机构
                     + " AND ((" + BasePermissionScopeEntity.FieldResourceCategory + " = '" + BaseUserEntity.CurrentTableName + "' "
                     + " AND " + BasePermissionScopeEntity.FieldResourceId + " = '" + managerUserId + "')"
                     + " OR (" + BasePermissionScopeEntity.FieldResourceCategory + " = '" + BaseRoleEntity.CurrentTableName + "' "
                     + " AND " + BasePermissionScopeEntity.FieldResourceId + " IN ( "
                     // 获得属于那些角色有相应权限的组织机构
                     + "SELECT " + BaseUserRoleEntity.FieldRoleId
                     + " FROM " + BaseUserRoleEntity.CurrentTableName
                     + " WHERE " + BaseUserRoleEntity.FieldUserId + " = '" + managerUserId + "'"
                     + " AND " + BaseUserRoleEntity.FieldDeleted + " = 0 "
                     + " AND " + BaseUserRoleEntity.FieldEnabled + " = 1 "
                     // 修正不会读取用户默认角色权限域范围BUG
                     + "))) "
                     // 并且是指定的本权限
                     + " AND (" + BasePermissionScopeEntity.FieldPermissionId + " = '" + permissionId + "') ");
            return sb.Return();
        }
        #endregion

        #region public string GetOrganizationIdsSqlByParentId(int managerUserId, string permissionCode) 按某个权限获取组织机构 Sql
        /// <summary>
        /// 按某个权限获取组织机构 Sql (按ParentId树形结构计算)
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="managerUserId">管理用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>Sql</returns>
        public string GetOrganizationIdsSqlByParentId(string systemCode, string managerUserId, string permissionCode)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT Id FROM " + BaseOrganizationEntity.CurrentTableName
                     + " WHERE " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldEnabled + " = 1 "
                     + " AND " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldDeleted + " = 0 "
                     + " START WITH Id IN (" + GetOrganizationIdsSql(systemCode, managerUserId, permissionCode) + ") "
                     + " CONNECT BY PRIOR " + BaseOrganizationEntity.FieldId + " = " + BaseOrganizationEntity.FieldParentId);
            return sb.Return();
        }
        #endregion

        #region public string GetOrganizationIdsSqlByCode(int managerUserId, string permissionCode) 按某个权限获取组织机构 Sql
        /// <summary>
        /// 按某个权限获取组织机构 Sql (按Code编号进行计算)
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="managerUserId">管理用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>Sql</returns>
        public string GetOrganizationIdsSqlByCode(string systemCode, string managerUserId, string permissionCode)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + BaseOrganizationEntity.FieldId + " AS " + BaseUtil.FieldId
                     + " FROM " + BaseOrganizationEntity.CurrentTableName
                     + " , ( SELECT " + DbHelper.PlusSign(BaseOrganizationEntity.FieldCode, "'%'") + " AS " + BaseOrganizationEntity.FieldCode
                     + " FROM " + BaseOrganizationEntity.CurrentTableName
                     + " WHERE " + BaseOrganizationEntity.FieldId + " IN (" + GetOrganizationIdsSql(systemCode, managerUserId, permissionCode) + ")) ManageOrganization "
                     + " WHERE (" + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldEnabled + " = 1 "
                     // 编号相似的所有组织机构获取出来
                     + " AND " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldCode + " LIKE ManageOrganization." + BaseOrganizationEntity.FieldCode + ")");
            return sb.Return();
        }
        #endregion


        #region public string[] GetOrganizationIds(int managerUserId, string permissionCode = "Resource.ManagePermission", bool organizationIdOnly = true) 按某个权限获取组织机构 主键数组
        /// <summary>
        /// 按某个权限获取组织机构 主键数组
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="managerUserId">管理用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="organizationIdOnly">只返回组织机构主键</param>
        /// <returns>主键数组</returns>
        public string[] GetOrganizationIds(string systemCode, string managerUserId, string permissionCode = "Resource.ManagePermission", bool organizationIdOnly = true)
        {
            // 这里应该考虑，当前用户的管理权限是，所在公司？所在部门？所以在工作组等情况
            var sb = PoolUtil.StringBuilder.Get();
            if (UseGetChildrensByCode)
            {
                sb.Append(GetOrganizationIdsSqlByCode(systemCode, managerUserId, permissionCode));
            }
            else
            {
                if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                {
                    sb.Append(GetOrganizationIdsSqlByParentId(systemCode, managerUserId, permissionCode));
                }
                else
                {
                    // edit by zgl 不默认获取子部门
                    // string[] ids = this.GetTreeResourceScopeIds(managerUserId, BaseOrganizationEntity.CurrentTableName, permissionCode, true);
                    var ids = GetTreeResourceScopeIds(systemCode, managerUserId, BaseOrganizationEntity.CurrentTableName, permissionCode, false);
                    if (ids != null && ids.Length > 0 && organizationIdOnly)
                    {
                        TransformPermissionScope(managerUserId, ref ids);
                    }
                    // 这里是否应该整理，自己的公司、部门、工作组的事情？
                    if (organizationIdOnly)
                    {
                        // 这里列出只是有效地，没被删除的组织机构主键
                        if (ids != null && ids.Length > 0)
                        {
                            var organizationManager = new BaseOrganizationManager(DbHelper, UserInfo);
                            var parameters = new List<KeyValuePair<string, object>>
                            {
                                new KeyValuePair<string, object>(BaseOrganizationEntity.FieldId, ids),
                                new KeyValuePair<string, object>(BaseOrganizationEntity.FieldEnabled, 1),
                                new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0)
                            };
                            ids = organizationManager.GetIds(parameters);
                        }
                    }
                    return ids;
                }
            }
            var dt = DbHelper.Fill(sb.Return());
            return BaseUtil.FieldToArray(dt, BaseOrganizationEntity.FieldId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
        }
        #endregion

        #region public DataTable GetOrganizationDT(int managerUserId, string permissionCode = "Resource.ManagePermission", bool childrens = true) 按某个权限获取组织机构 数据表
        /// <summary>
        /// 按某个权限获取组织机构 数据表
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="managerUserId">管理用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="childrens">获取子节点</param>
        /// <returns>数据表</returns>
        public DataTable GetOrganizationDt(string systemCode, string managerUserId, string permissionCode = "Resource.ManagePermission", bool childrens = true)
        {
            var whereQuery = string.Empty;
            var permissionScope = PermissionOrganizationScope.NotAllowed;
            if (UseGetChildrensByCode)
            {
                whereQuery = GetOrganizationIdsSqlByCode(systemCode, managerUserId, permissionCode);
            }
            else
            {
                if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                {
                    whereQuery = GetOrganizationIdsSqlByParentId(systemCode, managerUserId, permissionCode);
                }
                else
                {
                    // edit by zgl on 2011.12.15, 不自动获取子部门
                    var ids = GetTreeResourceScopeIds(systemCode, managerUserId, BaseOrganizationEntity.CurrentTableName, permissionCode, childrens);
                    permissionScope = TransformPermissionScope(managerUserId, ref ids);
                    // 需要进行适当的翻译，所在部门，所在公司，全部啥啥的。
                    whereQuery = StringUtil.ArrayToList(ids);
                }
            }
            if (string.IsNullOrEmpty(whereQuery))
            {
                whereQuery = " NULL ";
            }
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT * FROM " + BaseOrganizationEntity.CurrentTableName
                     + " WHERE " + BaseOrganizationEntity.FieldDeleted + " = 0 "
                     + " AND " + BaseOrganizationEntity.FieldEnabled + " = 1 "
                     + " AND " + BaseOrganizationEntity.FieldIsInnerOrganization + " = 1 ");
            if (permissionScope != PermissionOrganizationScope.AllData)
            {
                sb.Append(" AND " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldId + " IN (" + whereQuery + ") ");
            }
            sb.Append(" ORDER BY " + BaseOrganizationEntity.FieldSortCode);
            return DbHelper.Fill(sb.Return());
        }
        #endregion


        //
        // 获得被某个权限管理范围内 角色的 Id、SQL、List
        // 

        #region public string GetRoleIdsSql(string systemCode, int managerUserId, string permissionCode, bool useBaseRole = false) 按某个权限获取角色 Sql
        /// <summary>
        /// 按某个权限获取角色 Sql
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="managerUserId">管理用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="useBaseRole">基础系统的权限是否采用</param>
        /// <returns>Sql</returns>
        public string GetRoleIdsSql(string systemCode, string managerUserId, string permissionCode, bool useBaseRole = false)
        {
            var permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);
            var roleTableName = GetRoleTableName(systemCode);
            var userRoleTableName = GetUserRoleTableName(systemCode);
            var permissionScopeTableName = GetPermissionScopeTableName(systemCode);
            var sb = PoolUtil.StringBuilder.Get();
            // 被管理的角色 
            sb.Append("SELECT " + permissionScopeTableName + ".TargetId AS " + BaseUtil.FieldId
                      + " FROM " + permissionScopeTableName
                      + " WHERE " + permissionScopeTableName + ".TargetId IS NOT NULL "
                      + " AND " + permissionScopeTableName + ".TargetCategory = '" + roleTableName + "' "
                      + " AND ((" + permissionScopeTableName + ".ResourceCategory = '" + BaseUserEntity.CurrentTableName + "' "
                      + " AND " + permissionScopeTableName + ".ResourceId = '" + managerUserId + "')"
                      // 以及 他所在的角色在管理的角色
                      + " OR (" + permissionScopeTableName + ".ResourceCategory = '" + roleTableName + "'"
                      + " AND " + permissionScopeTableName + ".ResourceId IN ( "
                      + " SELECT RoleId FROM " + userRoleTableName
                      + " WHERE (" + BaseUserRoleEntity.FieldUserId + " = '" + managerUserId + "' "
                      + " AND " + BaseUserRoleEntity.FieldEnabled + " = 1) ");

            if (useBaseRole)
            {
                sb.Append(" UNION SELECT RoleId FROM BaseUserRole WHERE (UserId = '" + managerUserId + "' AND " + BaseUtil.FieldEnabled + " = 1  ) ");
            }

            // 并且是指定的本权限
            sb.Append(")) " + " AND " + BasePermissionScopeEntity.FieldPermissionId + " = '" + permissionId + "')");

            // 被管理部门的列表
            var organizationIds = GetOrganizationIds(systemCode, managerUserId, permissionCode);
            if (organizationIds.Length > 0)
            {
                // 被管理的组织机构包含的角色
                sb.Append("  UNION "
                          + " SELECT " + roleTableName + "." + BaseRoleEntity.FieldId + " AS " + BaseUtil.FieldId
                          + " FROM " + roleTableName
                          + " WHERE " + roleTableName + "." + BaseRoleEntity.FieldEnabled + " = 1 "
                          + " AND " + roleTableName + "." + BaseRoleEntity.FieldDeleted + " = 0 "
                          + " AND " + roleTableName + "." + BaseRoleEntity.FieldOrganizationId + " IN (" + StringUtil.ArrayToList(organizationIds) + ") ");
            }
            return sb.Return();
        }
        #endregion

        #region public string[] GetRoleIds(int managerUserId, string permissionCode) 按某个权限获取角色 主键数组
        /// <summary>
        /// 按某个权限获取角色 主键数组
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="managerUserId">管理用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetRoleIds(string systemCode, string managerUserId, string permissionCode)
        {
            var sql = GetRoleIdsSql(systemCode, managerUserId, permissionCode);
            var dt = DbHelper.Fill(sql);
            var ids = BaseUtil.FieldToArray(dt, BaseUtil.FieldId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
            // 这里列出只是有效地，没被删除的角色主键
            if (ids != null && ids.Length > 0)
            {
                var roleManager = new BaseRoleManager(DbHelper, UserInfo);

                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldId, ids),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0)
                };

                ids = roleManager.GetIds(parameters);
            }
            return ids;
        }
        #endregion

        /// <summary>
        /// 获取清单
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="userId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="useBaseRole"></param>
        /// <returns></returns>
        public List<BaseRoleEntity> GetRoleList(string systemCode, string userId, string permissionCode = "Resource.ManagePermission", bool useBaseRole = false)
        {
            var result = new List<BaseRoleEntity>();
            var dt = GetRoleDt(systemCode, userId, permissionCode, useBaseRole);
            result = BaseEntity.GetList<BaseRoleEntity>(dt);
            return result;
        }

        #region public DataTable GetRoleDT(string systemCode, string userId, string permissionCode, bool useBaseRole = false) 按某个权限获取角色 数据表
        /// <summary>
        /// 按某个权限获取角色 数据表
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">管理用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="useBaseRole">使用基础角色</param>
        /// <returns>数据表</returns>
        public DataTable GetRoleDt(string systemCode, string userId, string permissionCode = "Resource.ManagePermission", bool useBaseRole = false)
        {
            var result = new DataTable(BaseRoleEntity.CurrentTableName);

            // 这里需要判断,是系统权限？
            var isAdmin = false;

            var userEntity = BaseUserManager.GetEntityByCache(userId);

            var userManager = new BaseUserManager(UserInfo);
            // 用户管理员,这里需要判断,是业务权限？
            isAdmin = userManager.IsAdministrator(userEntity);
            /*
                || userManager.IsInRoleByCode(systemCode, userId, "UserAdmin", useBaseRole)
                || userManager.IsInRoleByCode(systemCode, userId, "Admin", useBaseRole);
            */
            var tableName = BaseRoleEntity.CurrentTableName;
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableName = GetRoleTableName(systemCode);
            }
            if (isAdmin)
            {
                var manager = new BaseRoleManager(DbHelper, UserInfo, tableName);
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldIsVisible, 1),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0)
                };
                result = manager.GetDataTable(parameters, BaseModuleEntity.FieldSortCode);
                result.TableName = CurrentTableName;
                return result;
            }
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT * "
                      + " FROM " + tableName
                      + " WHERE " + BaseRoleEntity.FieldCreateUserId + " = '" + userId + "'"
                      + " OR " + tableName + "." + BaseRoleEntity.FieldId + " IN ("
                                + GetRoleIdsSql(systemCode, userId, permissionCode, useBaseRole)
                                + " ) AND (" + BaseRoleEntity.FieldDeleted + " = 0) "
                                + " AND (" + BaseRoleEntity.FieldIsVisible + " = 1) "
                   + " ORDER BY " + BaseRoleEntity.FieldSortCode);

            return DbHelper.Fill(sb.Return());
        }
        #endregion

        //
        // 获得被某个权限管理范围内 用户的 Id、SQL、List
        // 

        #region public string GetUserIdsSql(int managerUserId, string permissionCode) 按某个权限获取员工 Sql
        /// <summary>
        /// 按某个权限获取用户主键 Sql
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="managerUserId">管理用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>Sql</returns>
        public string GetUserIdsSql(string systemCode, string managerUserId, string permissionCode)
        {
            var permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);
            var sb = PoolUtil.StringBuilder.Get();
            // 直接管理的用户
            sb.Append("SELECT BasePermissionScope.TargetId AS " + BaseUtil.FieldId
                     + " FROM BasePermissionScope "
                     + " WHERE (BasePermissionScope.TargetCategory = '" + BaseUserEntity.CurrentTableName + "'"
                     + " AND BasePermissionScope.ResourceId = '" + managerUserId + "'"
                     + " AND BasePermissionScope.ResourceCategory = '" + BaseUserEntity.CurrentTableName + "'"
                     + " AND BasePermissionScope.PermissionId = '" + permissionId + "'"
                     + " AND BasePermissionScope.TargetId IS NOT NULL) ");

            // 被管理部门的列表
            var organizationIds = GetOrganizationIds(systemCode, managerUserId, permissionCode, false);
            if (organizationIds != null && organizationIds.Length > 0)
            {
                // 是否仅仅是自己的还有点儿问题
                if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.OnlyOwnData).ToString()))
                {
                    sb.Append(" UNION SELECT '" + UserInfo.Id + "' AS Id ");
                }
                else
                {
                    // 被管理的组织机构包含的用户，公司、部门、工作组
                    // sql += " UNION "
                    //         + "SELECT " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldUserId + " AS " + BaseUtil.FieldId
                    //         + " FROM " + BaseStaffEntity.CurrentTableName
                    //         + " WHERE (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldCompanyId + " IN (" + organizations + ") "
                    //         + " OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDepartmentId + " IN (" + organizations + ") "
                    //         + " OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldWorkgroupId + " IN (" + organizations + ")) ";

                    sb.Append(" UNION "
                             + "SELECT " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldId + " AS " + BaseUtil.FieldId
                             + " FROM " + BaseUserEntity.CurrentTableName
                             + " WHERE (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDeleted + " = 0 ) "
                             + " AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldEnabled + " = 1 ) "
                             + " AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldCompanyId + " IN (" + StringUtil.ArrayToList(organizationIds) + ") "
                             + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldSubCompanyId + " IN (" + StringUtil.ArrayToList(organizationIds) + ") "
                             + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDepartmentId + " IN (" + StringUtil.ArrayToList(organizationIds) + ") "
                             + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldWorkgroupId + " IN (" + StringUtil.ArrayToList(organizationIds) + ")) ");
                }
            }

            // 被管理角色列表
            var roleIds = GetRoleIds(systemCode, managerUserId, permissionCode);
            if (roleIds.Length > 0)
            {
                // 被管理的角色包含的员工
                sb.Append(" UNION "
                         + "SELECT " + BaseUserRoleEntity.CurrentTableName + "." + BaseUserRoleEntity.FieldUserId + " AS " + BaseUtil.FieldId
                         + " FROM " + BaseUserRoleEntity.CurrentTableName
                         + "  WHERE (" + BaseUserRoleEntity.CurrentTableName + "." + BaseUserRoleEntity.FieldEnabled + " = 1 "
                         + "        AND " + BaseUserRoleEntity.CurrentTableName + "." + BaseUserRoleEntity.FieldDeleted + " = 0 "
                         + "        AND " + BaseUserRoleEntity.CurrentTableName + "." + BaseUserRoleEntity.FieldRoleId + " IN (" + StringUtil.ArrayToList(roleIds) + ")) ");
            }

            return sb.Return();
        }
        #endregion

        #region public string[] GetUserIds(string systemCode, int managerUserId, string permissionCode) 按某个权限获取员工 主键数组
        /// <summary>
        /// 按某个权限获取员工 主键数组
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="managerUserId">管理用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetUserIds(string systemCode, string managerUserId, string permissionCode)
        {
            var ids = GetTreeResourceScopeIds(systemCode, managerUserId, BaseOrganizationEntity.CurrentTableName, permissionCode, true);
            // 是否为仅本人
            if (StringUtil.Exists(ids, ((int)PermissionOrganizationScope.OnlyOwnData).ToString()))
            {
                return new string[] { managerUserId.ToString() };
            }

            var sql = GetUserIdsSql(systemCode, managerUserId, permissionCode);
            var dt = DbHelper.Fill(sql);

            // 这里应该考虑，当前用户的管理权限是，所在公司？所在部门？所以在工作组等情况
            if (ids != null && ids.Length > 0)
            {
                var userEntity = BaseUserManager.GetEntityByCache(managerUserId);
                for (var i = 0; i < ids.Length; i++)
                {
                    if (ids[i].Equals(((int)PermissionOrganizationScope.OnlyOwnData).ToString()))
                    {
                        ids[i] = userEntity.Id.ToString();
                        break;
                    }
                }
            }

            // 这里列出只是有效地，没被删除的角色主键
            if (ids != null && ids.Length > 0)
            {
                var userManager = new BaseUserManager(UserInfo);

                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserEntity.FieldId, ids),
                    new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
                };

                var names = new string[] { BaseUserEntity.FieldId, BaseUserEntity.FieldEnabled, BaseUserEntity.FieldDeleted };
                var values = new Object[] { ids, 1, 0 };
                ids = userManager.GetIds(parameters);
            }

            return ids;
        }
        #endregion

        #region public List<BaseUserEntity> GetUserList(string userId, string permissionCode) 按某个权限获取员工 数据表
        /// <summary>
        /// 按某个权限获取员工 数据表
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">管理用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>数据表</returns>
        public List<BaseUserEntity> GetUserList(string systemCode, string userId, string permissionCode)
        {
            var result = new List<BaseUserEntity>();
            //string[] names = null;
            //object[] values = null;
            // 这里需要判断,是系统权限？
            var isRole = false;
            var userManager = new BaseUserManager(UserInfo);
            // 用户管理员,这里需要判断,是业务权限？
            isRole = userManager.IsInRoleByCode(userId, "UserAdmin") || userManager.IsInRoleByCode(userId, "Admin");
            if (isRole)
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserEntity.FieldIsVisible, 1),
                    new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
                };
                return userManager.GetList<BaseUserEntity>(parameters, BaseModuleEntity.FieldSortCode);
            }
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT * FROM " + BaseUserEntity.CurrentTableName);
            sb.Append(" WHERE " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDeleted + " = 0 "
                     + " AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldIsVisible + " = 1 "
                     + " AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldEnabled + " = 1 "
                     + " AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldId + " IN ("
                     + GetUserIdsSql(systemCode, userId, permissionCode)
                     + " ) "
                     + " ORDER BY " + BaseUserEntity.FieldSortCode);
            using (var dr = userManager.DbHelper.ExecuteReader(sb.Return()))
            {
                result = userManager.GetList<BaseUserEntity>(dr);
            }
            return result;
        }
        #endregion

        #region public DataTable GetUserDataTable(string userId, string permissionCode) 按某个权限获取员工 数据表
        /// <summary>
        /// 按某个权限获取员工 数据表
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">管理用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>数据表</returns>
        public DataTable GetUserDataTable(string systemCode, string userId, string permissionCode)
        {
            //string[] names = null;
            //object[] values = null;
            // 这里需要判断,是系统权限？
            var isRole = false;
            var userManager = new BaseUserManager(UserInfo);
            // 用户管理员,这里需要判断,是业务权限？
            isRole = userManager.IsInRoleByCode(userId, "UserAdmin") || userManager.IsInRoleByCode(userId, "Admin");
            if (isRole)
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserEntity.FieldIsVisible, 1),
                    new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
                };
                return userManager.GetDataTable(parameters, BaseModuleEntity.FieldSortCode);
            }
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT * FROM " + BaseUserEntity.CurrentTableName);
            sb.Append(" WHERE " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDeleted + " = 0 "
                     + " AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldIsVisible + " = 1 "
                     + " AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldEnabled + " = 1 "
                     + " AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldId + " IN ("
                     + GetUserIdsSql(systemCode, userId, permissionCode)
                     + " ) "
                     + " ORDER BY " + BaseUserEntity.FieldSortCode);

            return userManager.Fill(sb.Return());
        }
        #endregion

        #region public string[] GetTargetIds(string userId, string targetCategory, string result)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="targetCategory"></param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>主键数组</returns>
        public string[] GetTargetIds(string userId, string targetCategory, string permissionId)
        {
            var result = new string[0];

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.CurrentTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, targetCategory)
            };

            var dt = GetDataTable(parameters);
            result = BaseUtil.FieldToArray(dt, BasePermissionScopeEntity.FieldTargetId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
            return result;
        }
        #endregion

        //
        // ResourcePermissionScope 权限判断
        //


        /// <summary>
        /// 转换用户的权限范围
        /// </summary>
        /// <param name="userId">目标用户</param>
        /// <param name="resourceIds">权限范围</param>
        /// <param name="userManager"></param>
        /// <returns></returns>
        public PermissionOrganizationScope TransformPermissionScope(string userId, ref string[] resourceIds, BaseUserManager userManager = null)
        {
            var permissionScope = PermissionOrganizationScope.NotAllowed;
            if (resourceIds != null && resourceIds.Length > 0)
            {
                if (userManager == null)
                {
                    userManager = new BaseUserManager(UserInfo);
                }
                var userEntity = userManager.GetEntity(userId);

                for (var i = 0; i < resourceIds.Length; i++)
                {
                    if (resourceIds[i].Equals(((int)PermissionOrganizationScope.AllData).ToString()))
                    {
                        permissionScope = PermissionOrganizationScope.AllData;
                        continue;
                    }
                    if (resourceIds[i].Equals(((int)PermissionOrganizationScope.UserCompany).ToString()))
                    {
                        resourceIds[i] = userEntity.CompanyId.ToString();
                        permissionScope = PermissionOrganizationScope.UserCompany;
                        continue;
                    }
                    if (resourceIds[i].Equals(((int)PermissionOrganizationScope.UserSubCompany).ToString()))
                    {
                        resourceIds[i] = userEntity.SubCompanyId.ToString();
                        permissionScope = PermissionOrganizationScope.UserSubCompany;
                        continue;
                    }
                    if (resourceIds[i].Equals(((int)PermissionOrganizationScope.UserDepartment).ToString()))
                    {
                        resourceIds[i] = userEntity.DepartmentId.ToString();
                        permissionScope = PermissionOrganizationScope.UserDepartment;
                        continue;
                    }
                    if (resourceIds[i].Equals(((int)PermissionOrganizationScope.UserWorkgroup).ToString()))
                    {
                        resourceIds[i] = userEntity.WorkgroupId.ToString();
                        permissionScope = PermissionOrganizationScope.UserWorkgroup;
                        continue;
                    }
                }
            }
            return permissionScope;
        }

        /// <summary>
        /// 获得用户的某个权限范围资源主键数组
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户</param>
        /// <param name="targetCategory">资源分类</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetResourceScopeIds(string systemCode, string userId, string targetCategory, string permissionCode)
        {
            var permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);

            var tableName = GetUserRoleTableName(systemCode);

            CurrentTableName = GetPermissionScopeTableName(systemCode);
            var sb = PoolUtil.StringBuilder.Get();
            // 用户的权限
            sb.Append("SELECT TargetId "
                        + " FROM " + CurrentTableName
                        + "  WHERE (" + CurrentTableName + ".ResourceCategory = '" + BaseUserEntity.CurrentTableName + "') "
                        + "        AND (ResourceId = '" + userId + "') "
                        + "        AND (TargetCategory = '" + targetCategory + "') "
                        + "        AND (PermissionId = '" + permissionId + "') "
                        + "        AND (" + BasePermissionScopeEntity.FieldEnabled + " = 1) "
                        + "        AND (" + BasePermissionScopeEntity.FieldDeleted + " = 0)"

                        + " UNION "

                        // 用户归属的角色的权限                            
                        + "SELECT TargetId "
                        + " FROM " + CurrentTableName
                        + "  WHERE (ResourceCategory  = '" + BaseRoleEntity.CurrentTableName + "') "
                        + "        AND (TargetCategory  = '" + targetCategory + "') "
                        + "        AND (PermissionId = '" + permissionId + "') "
                        + "        AND (" + BasePermissionScopeEntity.FieldDeleted + " = 0)"
                        + "        AND (" + BasePermissionScopeEntity.FieldEnabled + " = 1) "
                        + "        AND ((ResourceId IN ( "
                        + "             SELECT RoleId "
                        + " FROM " + tableName
                        + "              WHERE (UserId  = '" + userId + "') "
                        + "                  AND (" + BasePermissionScopeEntity.FieldEnabled + " = 1) "
                        + "                  AND (" + BasePermissionScopeEntity.FieldDeleted + " = 0) ) ");
            sb.Append(" ) "
            + " ) ");

            var dt = DbHelper.Fill(sb.ToString());
            var resourceIds = BaseUtil.FieldToArray(dt, BasePermissionScopeEntity.FieldTargetId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();

            // 按部门获取权限
            if (BaseSystemInfo.UseOrganizationPermission)
            {
                sb.Clear();
                var userEntity = new BaseUserManager(DbHelper).GetEntity(userId);
                sb.Append("SELECT TargetId "
                           + " FROM " + CurrentTableName
                           + " WHERE (" + CurrentTableName + ".ResourceCategory = '" +
                           BaseOrganizationEntity.CurrentTableName + "') "
                           + " AND (ResourceId = '" + userEntity.CompanyId + "' OR "
                           + " ResourceId = '" + userEntity.DepartmentId + "' OR "
                           + " ResourceId = '" + userEntity.SubCompanyId + "' OR "
                           + " ResourceId = '" + userEntity.WorkgroupId + "') "
                           + " AND (TargetCategory = '" + targetCategory + "') "
                           + " AND (PermissionId = '" + permissionId + "') "
                           + " AND (" + BasePermissionScopeEntity.FieldEnabled + " = 1) "
                           + " AND (" + BasePermissionScopeEntity.FieldDeleted + " = 0)");
                dt = DbHelper.Fill(sb.Return());
                var resourceIdsByOrganization = BaseUtil.FieldToArray(dt, BasePermissionScopeEntity.FieldTargetId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
                resourceIds = StringUtil.Concat(resourceIds, resourceIdsByOrganization);
            }

            if (targetCategory.Equals(BaseOrganizationEntity.CurrentTableName))
            {
                TransformPermissionScope(userId, ref resourceIds);
            }
            return resourceIds;
        }

        /// <summary>
        /// 树型资源的权限范围
        /// 2011-03-17 吉日嘎拉
        /// 这个是嫩牛X的方法，值得收藏，值得分析
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户</param>
        /// <param name="tableName"></param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="childrens">是否含子节点</param>
        /// <returns>主键数组</returns>
        public string[] GetTreeResourceScopeIds(string systemCode, string userId, string tableName, string permissionCode, bool childrens)
        {
            string[] resourceScopeIds = null;
            resourceScopeIds = GetResourceScopeIds(systemCode, userId, tableName, permissionCode);
            if (!childrens)
            {
                return resourceScopeIds;
            }
            var idList = StringUtil.ArrayToList(resourceScopeIds);
            // 若本来就没管理部门啥的，那就没必要进行递归操作了
            if (!string.IsNullOrEmpty(idList))
            {
                var sb = PoolUtil.StringBuilder.Get();
                if (DbHelper.CurrentDbType == CurrentDbType.SqlServer)
                {
                    sb.Append(@" WITH PermissionScopeTree AS (SELECT ID FROM " + tableName + @" WHERE (Id IN (" + idList + @") ) UNION ALL SELECT ResourceTree.Id
                             FROM " + tableName + @" AS ResourceTree INNER JOIN PermissionScopeTree AS A ON A.Id = ResourceTree.ParentId)
                             SELECT Id FROM PermissionScopeTree ");
                }
                else if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                {
                    sb.Append("     SELECT Id "
                             + " FROM " + tableName
                             + " START WITH Id = ParentId "
                             + " CONNECT BY PRIOR Id Id IN (" + idList + ")");
                }

                var dt = DbHelper.Fill(sb.Return());
                var resourceIds = BaseUtil.FieldToArray(dt, "Id").Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
                return StringUtil.Concat(resourceScopeIds, resourceIds);
            }
            return resourceScopeIds;
        }

        #region public bool IsModuleAuthorized(BaseUserInfo userInfo, string moduleCode, string permissionCode) 是否有相应的权限
        /// <summary>
        /// 是否有相应的权限
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="moduleCode">模块编码</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>是否有权限</returns>
        public bool IsModuleAuthorized(BaseUserInfo userInfo, string moduleCode, string permissionCode)
        {
            // 先判断用户类别
            if (BaseUserManager.IsAdministrator(userInfo.Id.ToString()))
            {
                return true;
            }
            return IsModuleAuthorized(UserInfo.SystemCode, UserInfo.Id.ToString(), moduleCode, permissionCode);
        }
        #endregion

        #region public bool IsModuleAuthorized(string userId, string moduleCode, string permissionCode) 是否有相应的权限
        /// <summary>
        /// 是否有相应的权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="moduleCode"></param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>是否有权限</returns>
        public bool IsModuleAuthorized(string systemCode, string userId, string moduleCode, string permissionCode)
        {
            var moduleId = new BaseModuleManager().GetIdByCodeByCache(systemCode, moduleCode);
            var permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);
            // 判断员工权限
            if (CheckUserModulePermission(userId, moduleId, permissionId))
            {
                return true;
            }
            // 判断员工角色权限
            if (CheckRoleModulePermission(userId, moduleId, permissionId))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region private bool CheckUserModulePermission(string userId, string moduleId, string result)
        /// <summary>
        /// 员工是否有模块权限
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="moduleId">模块菜单主键</param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>是否有权限</returns>
        private bool CheckUserModulePermission(string userId, string moduleId, string permissionId)
        {
            return CheckResourcePermissionScope(BaseModuleEntity.CurrentTableName, userId, BaseModuleEntity.CurrentTableName, moduleId, permissionId);
        }
        #endregion

        private bool CheckResourcePermissionScope(string resourceCategory, string resourceId, string targetCategory, string targetId, string permissionId)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) FROM BasePermissionScope "
                             + " WHERE (BasePermissionScope.ResourceCategory = '" + resourceCategory + "')"
                             + " AND (BasePermissionScope." + BasePermissionScopeEntity.FieldEnabled + " = 1) "
                             + " AND (BasePermissionScope." + BasePermissionScopeEntity.FieldDeleted + " = 0 )"
                             + " AND (BasePermissionScope.ResourceId = '" + resourceId + "')"
                             + " AND (BasePermissionScope.TargetCategory = '" + targetCategory + "')"
                             + " AND (BasePermissionScope.TargetId = '" + targetId + "')"
                             + " AND (BasePermissionScope.PermissionId = " + permissionId + "))");
            var result = 0;

            var obj = DbHelper.ExecuteScalar(sb.Return());
            if (obj != null)
            {
                result = obj.ToInt();
            }
            return result > 0;
        }

        #region private bool CheckRoleModulePermission(string userId, string moduleId, string result)
        /// <summary>
        /// 员工角色关系是否有模块权限
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="moduleId">模块菜单主键</param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>有角色权限</returns>
        private bool CheckRoleModulePermission(string userId, string moduleId, string permissionId)
        {
            return CheckRolePermissionScope(userId, BaseModuleEntity.CurrentTableName, moduleId, permissionId);
        }
        #endregion

        private bool CheckRolePermissionScope(string userId, string targetCategory, string targetId, string permissionId)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) "
                            + " FROM BasePermissionScope "
                            + "  WHERE (BasePermissionScope.ResourceCategory = '" + BaseRoleEntity.CurrentTableName + "') "
                            + "        AND (BasePermissionScope." + BasePermissionScopeEntity.FieldEnabled + " = 1 "
                            + "        AND (BasePermissionScope." + BasePermissionScopeEntity.FieldDeleted + " = 0 "
                            + "        AND (BasePermissionScope.ResourceId IN ( "
                                             + "SELECT BaseUserRole.RoleId "
                                             + " FROM BaseUserRole "
                                             + "  WHERE BaseUserRole.UserId = '" + userId + "'"
                                             + "        AND BaseUserRole." + BasePermissionScopeEntity.FieldEnabled + " = 1 "
                                             + " )) "
                            + " AND (BasePermissionScope.TargetCategory = '" + targetCategory + "') "
                            + " AND (BasePermissionScope.TargetId = '" + targetId + "') "
                            + " AND (BasePermissionScope.PermissionId = " + permissionId + ")) ");
            var rowCount = 0;

            var obj = DbHelper.ExecuteScalar(sb.Return());
            if (obj != null)
            {
                rowCount = obj.ToInt();
            }
            return rowCount > 0;
        }

        /// <summary>
        /// 授权资源权限范围
        /// </summary>
        /// <param name="resourceCategory"></param>
        /// <param name="resourceId"></param>
        /// <param name="targetCategory"></param>
        /// <param name="grantTargetIds"></param>
        /// <param name="permissionId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public int GrantResourcePermissionScopeTarget(string resourceCategory, string resourceId, string targetCategory, string[] grantTargetIds, string permissionId, DateTime? startTime = null, DateTime? endTime = null)
        {
            var result = 0;

            var entity = new BasePermissionScopeEntity
            {
                ResourceCategory = resourceCategory,
                ResourceId = resourceId.ToInt(),
                TargetCategory = targetCategory,
                PermissionId = permissionId.ToInt(),
                StartTime = startTime,
                EndTime = endTime,
                Enabled = 1,
                Deleted = 0
            };
            for (var i = 0; i < grantTargetIds.Length; i++)
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, resourceCategory),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, resourceId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, targetCategory),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, grantTargetIds[i]),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldDeleted, 0)
                };

                entity.TargetId = grantTargetIds[i].ToInt();
                if (!Exists(parameters))
                {
                    Add(entity);
                    result++;
                }
            }
            return result;
        }

        /// <summary>
        /// 授权资源权限范围
        /// </summary>
        /// <param name="resourceCategory"></param>
        /// <param name="resourceIds"></param>
        /// <param name="targetCategory"></param>
        /// <param name="grantTargetId"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public int GrantResourcePermissionScopeTarget(string resourceCategory, string[] resourceIds, string targetCategory, string grantTargetId, string permissionId)
        {
            var result = 0;

            List<KeyValuePair<string, object>> parameters = null;
            var entity = new BasePermissionScopeEntity
            {
                ResourceCategory = resourceCategory,
                // entity.ResourceId = resourceId;
                TargetCategory = targetCategory,
                PermissionId = permissionId.ToInt(),
                TargetId = grantTargetId.ToInt(),
                Enabled = 1,
                Deleted = 0
            };
            for (var i = 0; i < resourceIds.Length; i++)
            {
                parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, resourceCategory),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, resourceIds[i]),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, grantTargetId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, targetCategory),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldDeleted, 0)
                };

                entity.ResourceId = resourceIds[i].ToInt();
                if (!Exists(parameters))
                {
                    //entity.Id = Guid.NewGuid().ToString("N");
                    Add(entity, false, false);
                    result++;
                }
            }
            return result;
        }

        /// <summary>
        /// 52.撤消资源的权限范围
        /// </summary>
        /// <param name="resourceCategory">资源分类</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="targetCategory">目标类别</param>
        /// <param name="revokeTargetIds">目标主键数组</param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>影响的行数</returns>
        public int RevokeResourcePermissionScopeTarget(string resourceCategory, string resourceId, string targetCategory, string[] revokeTargetIds, string permissionId)
        {
            var result = 0;
            for (var i = 0; i < revokeTargetIds.Length; i++)
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, resourceCategory),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, resourceId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, targetCategory),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, revokeTargetIds[i]),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldEnabled, 1)
                };
                // 逻辑删除
                // result += this.SetDeleted(parameters);
                // 物理删除
                result += Delete(parameters);
            }
            return result;
        }

        /// <summary>
        /// 52.撤消资源的权限范围
        /// </summary>
        /// <param name="resourceCategory">资源分类</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="targetCategory">目标类别</param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>影响的行数</returns>
        public int RevokeResourcePermissionScopeTarget(string resourceCategory, string resourceId, string targetCategory, string permissionId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, resourceCategory),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, resourceId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, targetCategory),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldEnabled, 1)
            };
            // 逻辑删除
            return SetDeleted(parameters);
            // 物理删除
            // return this.Delete(parameters);
        }

        /// <summary>
        /// 撤回资源权限范围
        /// </summary>
        /// <param name="resourceCategory"></param>
        /// <param name="resourceIds"></param>
        /// <param name="targetCategory"></param>
        /// <param name="revokeTargetId"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public int RevokeResourcePermissionScopeTarget(string resourceCategory, string[] resourceIds, string targetCategory, string revokeTargetId, string permissionId)
        {
            var result = 0;
            var parameters = new List<KeyValuePair<string, object>>();
            for (var i = 0; i < resourceIds.Length; i++)
            {
                parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, resourceCategory),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, resourceIds[i]),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, targetCategory),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, revokeTargetId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldEnabled, 1)
                };
                // 逻辑删除
                result += SetDeleted(parameters);
                // 物理删除
                //result += this.Delete(parameters);
            }
            return result;
        }

        #region public bool HasAuthorized(string[] names, object[] values, string startDate, string endDate) 判断某个时间范围内是否存在
        /// <summary>
        /// 判断某个时间范围内是否存在
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public bool HasAuthorized(List<KeyValuePair<string, object>> parameters, string startDate, string endDate)
        {
            var result = false;
            var manager = new BasePermissionScopeManager(DbHelper, UserInfo);
            result = manager.Exists(parameters);
            /*
            if (result)
            {
                // 再去判断时间
                string minDate = "1900-01-01 00:00:00";
                string maxDate = "3000-12-31 00:00:00";
                // 不用设置的太大
                string srcStartDate = manager.GetProperty(names, values, BasePermissionScopeEntity.FieldStartDate);
                string srcEndDate = manager.GetProperty(names, values, BasePermissionScopeEntity.FieldEndDate);
                if (string.IsNullOrEmpty(srcStartDate))
                {
                    srcStartDate = minDate;
                }
                if (string.IsNullOrEmpty(startDate))
                {
                    startDate = minDate;
                }
                if (string.IsNullOrEmpty(srcEndDate))
                {
                    srcEndDate = maxDate;
                }
                if (string.IsNullOrEmpty(endDate))
                {
                    endDate = maxDate;
                }
                // 满足这样的条件
                // 时间A(Start1-End1)，   时间B(Start2-End2)
                // Start1 <Start2   &&   Start2 <End1
                // Start1 <End2   &&   End2 <End1
                // Start2 <Start1   &&   End1 <End2
                if ((CheckDateScope(srcStartDate, startDate) && CheckDateScope(startDate, srcEndDate)) 
                    || (CheckDateScope(srcStartDate, endDate) && CheckDateScope(endDate, srcEndDate)) 
                    || (CheckDateScope(startDate, srcStartDate) && CheckDateScope(srcEndDate, endDate)))
                {
                    result = true;
                }
                else 
                {
                    result = false;
                }                   
            }
            */
            return result;
        }
        #endregion

        #region  public int CheckDateScope(string smallDate,string bigDate) 检查日期大小
        /// <summary>
        /// 检查日期大小
        /// </summary>
        /// <param name="smallDate">开始日期</param>
        /// <param name="bigDate">结束日期</param>
        /// <returns>0：小于等于 1：大于等于</returns>
        public bool CheckDateScope(string smallDate, string bigDate)
        {
            // 开始日期是无限大
            // 结束日期是无限大
            return DateTime.Parse(smallDate) < DateTime.Parse(bigDate);
        }
        #endregion

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="resourceCategory"></param>
        /// <param name="targetCategory"></param>
        /// <returns></returns>
        public DataTable Search(string resourceId, string resourceCategory, string targetCategory)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT * FROM " + CurrentTableName
                            + " WHERE " + BasePermissionScopeEntity.FieldDeleted + " =0 "
                            + " AND " + BasePermissionScopeEntity.FieldEnabled + " =1 ");
            var dbParameters = new List<IDbDataParameter>();
            if (!string.IsNullOrEmpty(resourceId))
            {
                sb.Append(" AND " + BasePermissionScopeEntity.FieldResourceId + " = '" + resourceId + "'");
            }
            if (!string.IsNullOrEmpty(resourceCategory))
            {
                sb.Append(" AND " + BasePermissionScopeEntity.FieldResourceCategory + " = '" + resourceCategory + "'");
            }
            if (!string.IsNullOrEmpty(targetCategory))
            {
                sb.Append(" AND " + BasePermissionScopeEntity.FieldTargetCategory + " = '" + targetCategory + "'");
            }
            sb.Append(" ORDER BY " + BasePermissionScopeEntity.FieldCreateTime + " DESC ");
            return DbHelper.Fill(sb.Return());

            //for (int i = 0; i < result.Rows.Count; i++)
            //{
            //    if (!string.IsNullOrEmpty(result.Rows[i][BasePermissionScopeEntity.FieldEndDate].ToString()))
            //    {
            //        // 过期的不显示
            //        if (DateTime.Parse(result.Rows[i][BasePermissionScopeEntity.FieldEndDate].ToString()).Date < DateTime.Now.Date)
            //        {
            //            result.Rows.RemoveAt(i);
            //        }
            //    }
            //}
        }

        #region public DataTable GetAuthoriedList(string resourceCategory, string result, string targetCategory, string targetId) 获得有效的委托列表
        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="resourceCategory"></param>
        /// <param name="permissionId"></param>
        /// <param name="targetCategory"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public DataTable GetAuthoriedList(string resourceCategory, string permissionId, string targetCategory, string targetId)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT * FROM " + CurrentTableName
                            + " WHERE " + BasePermissionScopeEntity.FieldDeleted + " = 0 "
                            + " AND " + BasePermissionScopeEntity.FieldEnabled + " = 1 ");
            if (!string.IsNullOrEmpty(resourceCategory))
            {
                sb.Append(" AND " + BasePermissionScopeEntity.FieldResourceCategory + " = '" + resourceCategory + "'");
            }
            if (!string.IsNullOrEmpty(permissionId))
            {
                sb.Append(" AND " + BasePermissionScopeEntity.FieldPermissionId + " = '" + permissionId + "'");
            }
            if (!string.IsNullOrEmpty(targetCategory))
            {
                sb.Append(" AND " + BasePermissionScopeEntity.FieldTargetCategory + " = '" + targetCategory + "'");
            }
            if (!string.IsNullOrEmpty(targetId))
            {
                sb.Append(" AND " + BasePermissionScopeEntity.FieldTargetId + " = '" + targetId + "'");
            }
            // 时间在startDate或者endDate之间为有效
            if (BaseSystemInfo.UserCenterDbType.Equals(CurrentDbType.SqlServer))
            {
                sb.Append(" AND ((SELECT DATEDIFF(day, " + BasePermissionScopeEntity.FieldStartTime + ", GETDATE()))>=0"
                         + " OR (SELECT DATEDIFF(day, " + BasePermissionScopeEntity.FieldStartTime + ", GETDATE())) IS NULL)");
                sb.Append(" AND ((SELECT DATEDIFF(day, " + BasePermissionScopeEntity.FieldEndTime + ", GETDATE()))<=0"
                         + " OR (SELECT DATEDIFF(day, " + BasePermissionScopeEntity.FieldEndTime + ", GETDATE())) IS NULL)");
            }
            // TODO:其他数据库的兼容
            sb.Append(" ORDER BY " + BasePermissionScopeEntity.FieldCreateTime + " DESC ");
            return DbHelper.Fill(sb.Return());
        }
        #endregion
    }
}