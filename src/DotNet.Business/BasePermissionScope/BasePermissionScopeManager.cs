//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

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
    ///     用户能有某种权限的所有员工      public string[] GetUserIds(string managerUserId, string permissionCode)
    ///                                     public string GetUserIdsSql(string managerUserId, string permissionCode)
    ///     
    ///     用户能有某种权限所有组织机构    public string[] GetOrganizationIds(string managerUserId, string permissionCode)
    ///                                     public string GetOrganizationIdsSql(string managerUserId, string permissionCode)
    ///     
    ///     用户能有某种权限的所有角色      public string[] GetAllRoleIds(string managerUserId, string permissionCode)
    ///                                     public string GetAllRoleIdsSql(string managerUserId, string permissionCode)
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
    public partial class BasePermissionScopeManager : BaseManager, IBaseManager
    {
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
            var resourcePermissionScope = new BasePermissionScopeEntity
            {
                ResourceCategory = resourceCategory,
                ResourceId = resourceId,
                TargetCategory = targetCategory,
                TargetId = targetId,
                Enabled = 1,
                DeletionStateCode = 0
            };
            return AddPermission(resourcePermissionScope);
        }

        #region public string AddPermission(BasePermissionScopeEntity resourcePermissionScope)
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="resourcePermissionScope">对象</param>
        /// <returns>主键</returns>
        public string AddPermission(BasePermissionScopeEntity resourcePermissionScope)
        {
            var result = string.Empty;
            // 检查记录是否重复
            if (!PermissionScopeExists(resourcePermissionScope.PermissionId, resourcePermissionScope.ResourceCategory, resourcePermissionScope.ResourceId, resourcePermissionScope.TargetCategory, resourcePermissionScope.TargetId))
            {
                result = AddEntity(resourcePermissionScope);
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

        #region public string GetOrganizationIdsSql(string managerUserId, string permissionCode) 按某个权限获取组织机构 Sql
        /// <summary>
        /// 按某个权限获取组织机构 Sql
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="managerUserId">管理用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>Sql</returns>
        public string GetOrganizationIdsSql(string systemCode, string managerUserId, string permissionCode)
        {
            var permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);

            var sql = "SELECT " + BasePermissionScopeEntity.FieldTargetId
                     + " FROM " + BasePermissionScopeEntity.TableName
                     // 有效的，并且不为空的组织机构主键
                     + "  WHERE (" + BasePermissionScopeEntity.FieldTargetCategory + " = '" + BaseOrganizationEntity.TableName + "') "
                     + "        AND ( " + BasePermissionScopeEntity.TableName + "." + BasePermissionScopeEntity.FieldDeleted + " = 0) "
                     + "        AND ( " + BasePermissionScopeEntity.TableName + "." + BasePermissionScopeEntity.FieldEnabled + " = 1) "
                     + "        AND ( " + BasePermissionScopeEntity.TableName + "." + BasePermissionScopeEntity.FieldTargetId + " IS NOT NULL) "
                     // 自己直接由相应权限的组织机构
                     + "        AND ((" + BasePermissionScopeEntity.FieldResourceCategory + " = '" + BaseUserEntity.TableName + "' "
                     + "        AND " + BasePermissionScopeEntity.FieldResourceId + " = '" + managerUserId + "')"
                     + " OR (" + BasePermissionScopeEntity.FieldResourceCategory + " = '" + BaseRoleEntity.TableName + "' "
                     + "       AND " + BasePermissionScopeEntity.FieldResourceId + " IN ( "
                     // 获得属于那些角色有相应权限的组织机构
                     + "SELECT " + BaseUserRoleEntity.FieldRoleId
                     + " FROM " + BaseUserRoleEntity.TableName
                     + "  WHERE " + BaseUserRoleEntity.FieldUserId + " = '" + managerUserId + "'"
                     + "        AND " + BaseUserRoleEntity.FieldDeleted + " = 0 "
                     + "        AND " + BaseUserRoleEntity.FieldEnabled + " = 1"
                     // 修正不会读取用户默认角色权限域范围BUG
                     + "))) "
                     // 并且是指定的本权限
                     + " AND (" + BasePermissionScopeEntity.FieldPermissionId + " = '" + permissionId + "') ";
            return sql;
        }
        #endregion

        #region public string GetOrganizationIdsSqlByParentId(string managerUserId, string permissionCode) 按某个权限获取组织机构 Sql
        /// <summary>
        /// 按某个权限获取组织机构 Sql (按ParentId树形结构计算)
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="managerUserId">管理用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>Sql</returns>
        public string GetOrganizationIdsSqlByParentId(string systemCode, string managerUserId, string permissionCode)
        {
            var sql = "SELECT Id "
                     + " FROM " + BaseOrganizationEntity.TableName
                     + "  WHERE " + BaseOrganizationEntity.TableName + "." + BaseOrganizationEntity.FieldEnabled + " = 1 "
                     + "        AND " + BaseOrganizationEntity.TableName + "." + BaseOrganizationEntity.FieldDeleted + " = 0 "
                     + "  START WITH Id IN (" + GetOrganizationIdsSql(systemCode, managerUserId, permissionCode) + ") "
                     + " CONNECT BY PRIOR " + BaseOrganizationEntity.FieldId + " = " + BaseOrganizationEntity.FieldParentId;
            return sql;
        }
        #endregion

        #region public string GetOrganizationIdsSqlByCode(string managerUserId, string permissionCode) 按某个权限获取组织机构 Sql
        /// <summary>
        /// 按某个权限获取组织机构 Sql (按Code编号进行计算)
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="managerUserId">管理用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>Sql</returns>
        public string GetOrganizationIdsSqlByCode(string systemCode, string managerUserId, string permissionCode)
        {
            var sql = "SELECT " + BaseOrganizationEntity.FieldId + " AS " + BaseUtil.FieldId
                     + " FROM " + BaseOrganizationEntity.TableName
                     + "         , ( SELECT " + DbHelper.PlusSign(BaseOrganizationEntity.FieldCode, "'%'") + " AS " + BaseOrganizationEntity.FieldCode
                     + " FROM " + BaseOrganizationEntity.TableName
                     + "     WHERE " + BaseOrganizationEntity.FieldId + " IN (" + GetOrganizationIdsSql(systemCode, managerUserId, permissionCode) + ")) ManageOrganization "
                     + " WHERE (" + BaseOrganizationEntity.TableName + "." + BaseOrganizationEntity.FieldEnabled + " = 1 "
                     // 编号相似的所有组织机构获取出来
                     + "       AND " + BaseOrganizationEntity.TableName + "." + BaseOrganizationEntity.FieldCode + " LIKE ManageOrganization." + BaseOrganizationEntity.FieldCode + ")";
            return sql;
        }
        #endregion


        #region public string[] GetOrganizationIds(string managerUserId, string permissionCode = "Resource.ManagePermission", bool organizationIdOnly = true) 按某个权限获取组织机构 主键数组
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
            var sql = string.Empty;
            if (UseGetChildrensByCode)
            {
                sql = GetOrganizationIdsSqlByCode(systemCode, managerUserId, permissionCode);
            }
            else
            {
                if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                {
                    sql = GetOrganizationIdsSqlByParentId(systemCode, managerUserId, permissionCode);
                }
                else
                {
                    // edit by zgl 不默认获取子部门
                    // string[] ids = this.GetTreeResourceScopeIds(managerUserId, BaseOrganizationEntity.TableName, permissionCode, true);
                    var ids = GetTreeResourceScopeIds(systemCode, managerUserId, BaseOrganizationEntity.TableName, permissionCode, false);
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
                            var organizeManager = new BaseOrganizationManager(DbHelper, UserInfo);
                            var parameters = new List<KeyValuePair<string, object>>
                            {
                                new KeyValuePair<string, object>(BaseOrganizationEntity.FieldId, ids),
                                new KeyValuePair<string, object>(BaseOrganizationEntity.FieldEnabled, 1),
                                new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0)
                            };
                            ids = organizeManager.GetIds(parameters);
                        }
                    }
                    return ids;
                }
            }
            var dt = DbHelper.Fill(sql);
            return BaseUtil.FieldToArray(dt, BaseOrganizationEntity.FieldId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
        }
        #endregion

        #region public DataTable GetOrganizationDT(string managerUserId, string permissionCode = "Resource.ManagePermission", bool childrens = true) 按某个权限获取组织机构 数据表
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
                    var ids = GetTreeResourceScopeIds(systemCode, managerUserId, BaseOrganizationEntity.TableName, permissionCode, childrens);
                    permissionScope = TransformPermissionScope(managerUserId, ref ids);
                    // 需要进行适当的翻译，所在部门，所在公司，全部啥啥的。
                    whereQuery = StringUtil.ArrayToList(ids);
                }
            }
            if (string.IsNullOrEmpty(whereQuery))
            {
                whereQuery = " NULL ";
            }
            var sql = "SELECT * FROM " + BaseOrganizationEntity.TableName
                     + " WHERE " + BaseOrganizationEntity.FieldDeleted + " = 0 "
                     + "   AND " + BaseOrganizationEntity.FieldEnabled + " = 1 "
                     + "   AND " + BaseOrganizationEntity.FieldIsInnerOrganization + " = 1 ";
            if (permissionScope != PermissionOrganizationScope.AllData)
            {
                sql += " AND " + BaseOrganizationEntity.TableName + "." + BaseOrganizationEntity.FieldId + " IN (" + whereQuery + ") ";
            }
            sql += " ORDER BY " + BaseOrganizationEntity.FieldSortCode;
            return DbHelper.Fill(sql);
        }
        #endregion


        //
        // 获得被某个权限管理范围内 角色的 Id、SQL、List
        // 

        #region public string GetRoleIdsSql(string systemCode, string managerUserId, string permissionCode, bool useBaseRole = false) 按某个权限获取角色 Sql
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
            var permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
            var roleTableName = systemCode + "Role";
            var userRoleTableName = systemCode + "UserRole";
            var permissionScopeTableName = systemCode + "PermissionScope";
            var sql = string.Empty;
            // 被管理的角色 
            sql += "SELECT " + permissionScopeTableName + ".TargetId AS " + BaseUtil.FieldId
                      + " FROM " + permissionScopeTableName
                      + "  WHERE " + permissionScopeTableName + ".TargetId IS NOT NULL "
                      + "        AND " + permissionScopeTableName + ".TargetCategory = '" + roleTableName + "' "
                      + "        AND ((" + permissionScopeTableName + ".ResourceCategory = '" + BaseUserEntity.TableName + "' "
                      + "             AND " + permissionScopeTableName + ".ResourceId = '" + managerUserId + "')"
                      // 以及 他所在的角色在管理的角色
                      + "        OR (" + permissionScopeTableName + ".ResourceCategory = '" + roleTableName + "'"
                      + "            AND " + permissionScopeTableName + ".ResourceId IN ( "
                      + " SELECT RoleId "
                                                 + " FROM " + userRoleTableName
                      + "  WHERE (" + BaseUserRoleEntity.FieldUserId + " = '" + managerUserId + "' "
                      + "        AND " + BaseUserRoleEntity.FieldEnabled + " = 1) ";

            if (useBaseRole)
            {
                sql += " UNION SELECT RoleId FROM BaseUserRole WHERE (UserId = '" + managerUserId + "' AND Enabled = 1  ) ";
            }

            // 并且是指定的本权限
            sql += ")) " + " AND " + BasePermissionScopeEntity.FieldPermissionId + " = '" + permissionId + "')";

            // 被管理部门的列表
            var organizationIds = GetOrganizationIds(systemCode, managerUserId, permissionCode);
            if (organizationIds.Length > 0)
            {
                var organizes = string.Join(",", organizationIds);
                if (!string.IsNullOrEmpty(organizes))
                {
                    // 被管理的组织机构包含的角色
                    sql += "  UNION "
                              + " SELECT " + roleTableName + "." + BaseRoleEntity.FieldId + " AS " + BaseUtil.FieldId
                              + " FROM " + roleTableName
                              + "  WHERE " + roleTableName + "." + BaseRoleEntity.FieldEnabled + " = 1 "
                              + "    AND " + roleTableName + "." + BaseRoleEntity.FieldDeleted + " = 0 "
                              + "    AND " + roleTableName + "." + BaseRoleEntity.FieldOrganizationId + " IN (" + organizes + ") ";
                }
            }
            return sql;
        }
        #endregion

        #region public string[] GetRoleIds(string managerUserId, string permissionCode) 按某个权限获取角色 主键数组
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
            var result = new DataTable(BaseRoleEntity.TableName);

            // 这里需要判断,是系统权限？
            var isAdmin = false;

            var userEntity = BaseUserManager.GetEntityByCache(userId);

            var userManager = new BaseUserManager(DbHelper, UserInfo);
            // 用户管理员,这里需要判断,是业务权限？
            isAdmin = userManager.IsAdministrator(userEntity);
            /*
                || userManager.IsInRoleByCode(systemCode, userId, "UserAdmin", useBaseRole)
                || userManager.IsInRoleByCode(systemCode, userId, "Admin", useBaseRole);
            */
            var tableName = BaseRoleEntity.TableName;
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableName = systemCode + "Role";
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

            var sql = "SELECT * "
                      + " FROM " + tableName
                      + " WHERE " + BaseRoleEntity.FieldCreateUserId + " = '" + userId + "'"
                      + "    OR " + tableName + "." + BaseRoleEntity.FieldId + " IN ("
                                + GetRoleIdsSql(systemCode, userId, permissionCode, useBaseRole)
                                + " ) AND (" + BaseRoleEntity.FieldDeleted + " = 0) "
                                + " AND (" + BaseRoleEntity.FieldIsVisible + " = 1) "
                   + " ORDER BY " + BaseRoleEntity.FieldSortCode;

            return DbHelper.Fill(sql);
        }
        #endregion

        //
        // 获得被某个权限管理范围内 用户的 Id、SQL、List
        // 

        #region public string GetUserIdsSql(string managerUserId, string permissionCode) 按某个权限获取员工 Sql
        /// <summary>
        /// 按某个权限获取用户主键 Sql
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="managerUserId">管理用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>Sql</returns>
        public string GetUserIdsSql(string systemCode, string managerUserId, string permissionCode)
        {
            var permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
            // 直接管理的用户
            var sql = "SELECT BasePermissionScope.TargetId AS " + BaseUtil.FieldId
                     + " FROM BasePermissionScope "
                     + "  WHERE (BasePermissionScope.TargetCategory = '" + BaseUserEntity.TableName + "'"
                     + "        AND BasePermissionScope.ResourceId = '" + managerUserId + "'"
                     + "        AND BasePermissionScope.ResourceCategory = '" + BaseUserEntity.TableName + "'"
                     + "        AND BasePermissionScope.PermissionId = '" + permissionId + "'"
                     + "        AND BasePermissionScope.TargetId IS NOT NULL) ";

            // 被管理部门的列表
            var organizationIds = GetOrganizationIds(systemCode, managerUserId, permissionCode, false);
            if (organizationIds != null && organizationIds.Length > 0)
            {
                // 是否仅仅是自己的还有点儿问题
                if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.OnlyOwnData).ToString()))
                {
                    sql += " UNION SELECT '" + UserInfo.Id + "' AS Id ";
                }
                else
                {
                    var organizes = string.Join(",", organizationIds);
                    if (!string.IsNullOrEmpty(organizes))
                    {
                        // 被管理的组织机构包含的用户，公司、部门、工作组
                        // sql += " UNION "
                        //         + "SELECT " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldUserId + " AS " + BaseUtil.FieldId
                        //         + " FROM " + BaseStaffEntity.TableName
                        //         + " WHERE (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldCompanyId + " IN (" + organizes + ") "
                        //         + " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldDepartmentId + " IN (" + organizes + ") "
                        //         + " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldWorkgroupId + " IN (" + organizes + ")) ";

                        sql += " UNION "
                                 + "SELECT " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldId + " AS " + BaseUtil.FieldId
                                 + " FROM " + BaseUserEntity.TableName
                                 + "  WHERE (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDeleted + " = 0 ) "
                                 + "        AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldEnabled + " = 1 ) "
                                 + "        AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldCompanyId + " IN (" + organizes + ") "
                                  + "            OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldSubCompanyId + " IN (" + organizes + ") "
                                 + "            OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDepartmentId + " IN (" + organizes + ") "
                                 + "            OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldWorkgroupId + " IN (" + organizes + ")) ";
                    }
                }
            }

            // 被管理角色列表
            var roleIds = GetRoleIds(systemCode, managerUserId, permissionCode);
            if (roleIds.Length > 0)
            {
                var roles = string.Join(",", roleIds);
                if (!string.IsNullOrEmpty(roles))
                {
                    // 被管理的角色包含的员工
                    sql += " UNION "
                             + "SELECT " + BaseUserRoleEntity.TableName + "." + BaseUserRoleEntity.FieldUserId + " AS " + BaseUtil.FieldId
                             + " FROM " + BaseUserRoleEntity.TableName
                             + "  WHERE (" + BaseUserRoleEntity.TableName + "." + BaseUserRoleEntity.FieldEnabled + " = 1 "
                             + "        AND " + BaseUserRoleEntity.TableName + "." + BaseUserRoleEntity.FieldDeleted + " = 0 "
                             + "        AND " + BaseUserRoleEntity.TableName + "." + BaseUserRoleEntity.FieldRoleId + " IN (" + roles + ")) ";
                }
            }

            return sql;
        }
        #endregion

        #region public string[] GetUserIds(string systemCode, string managerUserId, string permissionCode) 按某个权限获取员工 主键数组
        /// <summary>
        /// 按某个权限获取员工 主键数组
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="managerUserId">管理用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetUserIds(string systemCode, string managerUserId, string permissionCode)
        {
            var ids = GetTreeResourceScopeIds(systemCode, managerUserId, BaseOrganizationEntity.TableName, permissionCode, true);
            // 是否为仅本人
            if (StringUtil.Exists(ids, ((int)PermissionOrganizationScope.OnlyOwnData).ToString()))
            {
                return new string[] { managerUserId };
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
                        ids[i] = userEntity.Id;
                        break;
                    }
                }
            }

            // 这里列出只是有效地，没被删除的角色主键
            if (ids != null && ids.Length > 0)
            {
                var userManager = new BaseUserManager(DbHelper, UserInfo);

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
            var userManager = new BaseUserManager(DbHelper, UserInfo);
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

            var sql = "SELECT * FROM " + BaseUserEntity.TableName;
            sql += " WHERE " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDeleted + " = 0 "
                     + " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldIsVisible + " = 1 "
                     + " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldEnabled + " = 1 "
                     + " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldId + " IN ("
                     + GetUserIdsSql(systemCode, userId, permissionCode)
                     + " ) "
                     + " ORDER BY " + BaseUserEntity.FieldSortCode;
            using (var dr = userManager.DbHelper.ExecuteReader(sql))
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
            var userManager = new BaseUserManager(DbHelper, UserInfo);
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

            var sql = "SELECT * FROM " + BaseUserEntity.TableName;
            sql += " WHERE " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDeleted + " = 0 "
                     + " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldIsVisible + " = 1 "
                     + " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldEnabled + " = 1 "
                     + " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldId + " IN ("
                     + GetUserIdsSql(systemCode, userId, permissionCode)
                     + " ) "
                     + " ORDER BY " + BaseUserEntity.FieldSortCode;

            return userManager.Fill(sql);
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
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.TableName),
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
                    userManager = new BaseUserManager(DbHelper, UserInfo);
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
                        resourceIds[i] = userEntity.CompanyId;
                        permissionScope = PermissionOrganizationScope.UserCompany;
                        continue;
                    }
                    if (resourceIds[i].Equals(((int)PermissionOrganizationScope.UserSubCompany).ToString()))
                    {
                        resourceIds[i] = userEntity.SubCompanyId;
                        permissionScope = PermissionOrganizationScope.UserSubCompany;
                        continue;
                    }
                    if (resourceIds[i].Equals(((int)PermissionOrganizationScope.UserDepartment).ToString()))
                    {
                        resourceIds[i] = userEntity.DepartmentId;
                        permissionScope = PermissionOrganizationScope.UserDepartment;
                        continue;
                    }
                    if (resourceIds[i].Equals(((int)PermissionOrganizationScope.UserWorkgroup).ToString()))
                    {
                        resourceIds[i] = userEntity.WorkgroupId;
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
            var permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);

            var tableName = systemCode + "UserRole";

            CurrentTableName = systemCode + "PermissionScope";

            var sql = string.Empty;
            sql =
                          // 用户的权限
                          "SELECT TargetId "
                        + " FROM " + CurrentTableName
                        + "  WHERE (" + CurrentTableName + ".ResourceCategory = '" + BaseUserEntity.TableName + "') "
                        + "        AND (ResourceId = '" + userId + "') "
                        + "        AND (TargetCategory = '" + targetCategory + "') "
                        + "        AND (PermissionId = '" + permissionId + "') "
                        + "        AND (Enabled = 1) "
                        + "        AND (" + BasePermissionScopeEntity.FieldDeleted + " = 0)"

                        + " UNION "

                        // 用户归属的角色的权限                            
                        + "SELECT TargetId "
                        + " FROM " + CurrentTableName
                        + "  WHERE (ResourceCategory  = '" + BaseRoleEntity.TableName + "') "
                        + "        AND (TargetCategory  = '" + targetCategory + "') "
                        + "        AND (PermissionId = '" + permissionId + "') "
                        + "        AND (" + BasePermissionScopeEntity.FieldDeleted + " = 0)"
                        + "        AND (Enabled = 1) "
                        + "        AND ((ResourceId IN ( "
                        + "             SELECT RoleId "
                        + " FROM " + tableName
                        + "              WHERE (UserId  = '" + userId + "') "
                        + "                  AND (Enabled = 1) "
                        + "                  AND (" + BasePermissionScopeEntity.FieldDeleted + " = 0) ) ";
            sql += " ) "
            + " ) ";

            var dt = DbHelper.Fill(sql);
            var resourceIds = BaseUtil.FieldToArray(dt, BasePermissionScopeEntity.FieldTargetId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();

            // 按部门获取权限
            if (BaseSystemInfo.UseOrganizationPermission)
            {
                sql = string.Empty;
                var userEntity = new BaseUserManager(DbHelper).GetEntity(userId);
                sql = "SELECT TargetId "
                           + " FROM " + CurrentTableName
                           + "  WHERE (" + CurrentTableName + ".ResourceCategory = '" +
                           BaseOrganizationEntity.TableName + "') "
                           + "        AND (ResourceId = '" + userEntity.CompanyId + "' OR "
                           + "              ResourceId = '" + userEntity.DepartmentId + "' OR "
                           + "              ResourceId = '" + userEntity.SubCompanyId + "' OR"
                           + "              ResourceId = '" + userEntity.WorkgroupId + "') "
                           + "        AND (TargetCategory = '" + targetCategory + "') "
                           + "        AND (PermissionId = '" + permissionId + "') "
                           + "        AND (Enabled = 1) "
                           + "        AND (" + BasePermissionScopeEntity.FieldDeleted + " = 0)";
                dt = DbHelper.Fill(sql);
                var resourceIdsByOrganization = BaseUtil.FieldToArray(dt, BasePermissionScopeEntity.FieldTargetId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
                resourceIds = StringUtil.Concat(resourceIds, resourceIdsByOrganization);
            }

            if (targetCategory.Equals(BaseOrganizationEntity.TableName))
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
                var sql = string.Empty;

                if (DbHelper.CurrentDbType == CurrentDbType.SqlServer)
                {
                    sql = @" WITH PermissionScopeTree AS (SELECT ID 
                             FROM " + tableName + @"
                                                            WHERE (Id IN (" + idList + @") )
                                                            UNION ALL
                                                           SELECT ResourceTree.Id
                             FROM " + tableName + @" AS ResourceTree INNER JOIN
                                                                  PermissionScopeTree AS A ON A.Id = ResourceTree.ParentId)
                                                   SELECT Id
                     FROM PermissionScopeTree ";
                }
                else if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                {
                    sql = "     SELECT Id "
                             + " FROM " + tableName
                             + " START WITH Id = ParentId "
                             + " CONNECT BY PRIOR Id Id IN (" + idList + ")";
                }

                var dt = DbHelper.Fill(sql);
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
            if (BaseUserManager.IsAdministrator(userInfo.Id))
            {
                return true;
            }
            return IsModuleAuthorized(UserInfo.SystemCode, UserInfo.Id, moduleCode, permissionCode);
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
            var moduleId = BaseModuleManager.GetIdByCodeByCache(systemCode, moduleCode);
            var permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
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
            return CheckResourcePermissionScope(BaseModuleEntity.TableName, userId, BaseModuleEntity.TableName, moduleId, permissionId);
        }
        #endregion

        private bool CheckResourcePermissionScope(string resourceCategory, string resourceId, string targetCategory, string targetId, string permissionId)
        {
            var sql = "SELECT COUNT(*) "
                             + " FROM BasePermissionScope "
                             + " WHERE (BasePermissionScope.ResourceCategory = '" + resourceCategory + "')"
                             + "       AND (BasePermissionScope.Enabled = 1) "
                             + "       AND (BasePermissionScope." + BasePermissionScopeEntity.FieldDeleted + " = 0 )"
                             + "       AND (BasePermissionScope.ResourceId = '" + resourceId + "')"
                             + "       AND (BasePermissionScope.TargetCategory = '" + targetCategory + "')"
                             + "       AND (BasePermissionScope.TargetId = '" + targetId + "')"
                             + "       AND (BasePermissionScope.PermissionId = " + permissionId + "))";
            var rowCount = 0;

            var returnObject = DbHelper.ExecuteScalar(sql);
            if (returnObject != null)
            {
                rowCount = int.Parse(returnObject.ToString());
            }
            return rowCount > 0;
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
            return CheckRolePermissionScope(userId, BaseModuleEntity.TableName, moduleId, permissionId);
        }
        #endregion

        private bool CheckRolePermissionScope(string userId, string targetCategory, string targetId, string permissionId)
        {
            var sql = "SELECT COUNT(*) "
                            + " FROM BasePermissionScope "
                            + "  WHERE (BasePermissionScope.ResourceCategory = '" + BaseRoleEntity.TableName + "') "
                            + "        AND (BasePermissionScope.Enabled = 1 "
                            + "        AND (BasePermissionScope." + BasePermissionScopeEntity.FieldDeleted + " = 0 "
                            + "        AND (BasePermissionScope.ResourceId IN ( "
                                             + "SELECT BaseUserRole.RoleId "
                                             + " FROM BaseUserRole "
                                             + "  WHERE BaseUserRole.UserId = '" + userId + "'"
                                             + "        AND BaseUserRole.Enabled = 1 "
                                             + " )) "
                            + " AND (BasePermissionScope.TargetCategory = '" + targetCategory + "') "
                            + " AND (BasePermissionScope.TargetId = '" + targetId + "') "
                            + " AND (BasePermissionScope.PermissionId = " + permissionId + ")) ";
            var rowCount = 0;

            var returnObject = DbHelper.ExecuteScalar(sql);
            if (returnObject != null)
            {
                rowCount = int.Parse(returnObject.ToString());
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
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public int GrantResourcePermissionScopeTarget(string resourceCategory, string resourceId, string targetCategory, string[] grantTargetIds, string permissionId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var result = 0;

            var resourcePermissionScope = new BasePermissionScopeEntity
            {
                ResourceCategory = resourceCategory,
                ResourceId = resourceId,
                TargetCategory = targetCategory,
                PermissionId = permissionId,
                StartDate = startDate,
                EndDate = endDate,
                Enabled = 1,
                DeletionStateCode = 0
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

                resourcePermissionScope.TargetId = grantTargetIds[i];
                if (!Exists(parameters))
                {
                    Add(resourcePermissionScope);
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
            var resourcePermissionScope = new BasePermissionScopeEntity
            {
                ResourceCategory = resourceCategory,
                // resourcePermissionScope.ResourceId = resourceId;
                TargetCategory = targetCategory,
                PermissionId = permissionId,
                TargetId = grantTargetId,

                Enabled = 1,
                DeletionStateCode = 0
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

                resourcePermissionScope.ResourceId = resourceIds[i];
                if (!Exists(parameters))
                {
                    resourcePermissionScope.Id = Guid.NewGuid().ToString("N");
                    Add(resourcePermissionScope, false, false);
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
            return SetDeleted(parameters, true, true, 4);
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
                result += SetDeleted(parameters, true, true, 4);
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
            var sql = "SELECT * FROM " + CurrentTableName
                            + " WHERE " + BasePermissionScopeEntity.FieldDeleted + " =0 "
                            + " AND " + BasePermissionScopeEntity.FieldEnabled + " =1 ";
            var dbParameters = new List<IDbDataParameter>();
            if (!string.IsNullOrEmpty(resourceId))
            {
                sql += " AND " + BasePermissionScopeEntity.FieldResourceId + " = '" + resourceId + "'";
            }
            if (!string.IsNullOrEmpty(resourceCategory))
            {
                sql += " AND " + BasePermissionScopeEntity.FieldResourceCategory + " = '" + resourceCategory + "'";
            }
            if (!string.IsNullOrEmpty(targetCategory))
            {
                sql += " AND " + BasePermissionScopeEntity.FieldTargetCategory + " = '" + targetCategory + "'";
            }
            sql += " ORDER BY " + BasePermissionScopeEntity.FieldCreateTime + " DESC ";
            return DbHelper.Fill(sql);

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
            var sql = "SELECT * FROM " + CurrentTableName
                            + " WHERE " + BasePermissionScopeEntity.FieldDeleted + " =0 "
                            + " AND " + BasePermissionScopeEntity.FieldEnabled + " =1 ";
            if (!string.IsNullOrEmpty(resourceCategory))
            {
                sql += " AND " + BasePermissionScopeEntity.FieldResourceCategory + " = '" + resourceCategory + "'";
            }
            if (!string.IsNullOrEmpty(permissionId))
            {
                sql += " AND " + BasePermissionScopeEntity.FieldPermissionId + " = '" + permissionId + "'";
            }
            if (!string.IsNullOrEmpty(targetCategory))
            {
                sql += " AND " + BasePermissionScopeEntity.FieldTargetCategory + " = '" + targetCategory + "'";
            }
            if (!string.IsNullOrEmpty(targetId))
            {
                sql += " AND " + BasePermissionScopeEntity.FieldTargetId + " = '" + targetId + "'";
            }
            // 时间在startDate或者endDate之间为有效
            if (BaseSystemInfo.UserCenterDbType.Equals(CurrentDbType.SqlServer))
            {
                sql += " AND ((SELECT DATEDIFF(day, " + BasePermissionScopeEntity.FieldStartDate + ", GETDATE()))>=0"
                         + " OR (SELECT DATEDIFF(day, " + BasePermissionScopeEntity.FieldStartDate + ", GETDATE())) IS NULL)";
                sql += " AND ((SELECT DATEDIFF(day, " + BasePermissionScopeEntity.FieldEndDate + ", GETDATE()))<=0"
                         + " OR (SELECT DATEDIFF(day, " + BasePermissionScopeEntity.FieldEndDate + ", GETDATE())) IS NULL)";
            }
            // TODO:其他数据库的兼容
            sql += " ORDER BY " + BasePermissionScopeEntity.FieldCreateTime + " DESC ";
            return DbHelper.Fill(sql);
        }
        #endregion
    }
}