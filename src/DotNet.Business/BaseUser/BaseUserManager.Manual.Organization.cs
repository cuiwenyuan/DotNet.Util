//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
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
    /// BaseUserManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///		2012.04.12 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.04.12</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        /// <summary>
        /// 用户是否在某部门
        /// </summary>
        /// <param name="organizationName">部门名称</param>
        /// <returns>存在</returns>
        public bool IsInOrganization(string organizationName)
        {
            return IsInOrganization(UserInfo.Id.ToString(), organizationName);
        }

        /// <summary>
        /// 用户是否在某部门
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="organizationName">部门名称</param>
        /// <returns>存在</returns>
        public bool IsInOrganization(string userId, string organizationName)
        {
            var result = false;
            // 把部门的主键找出来
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseOrganizationEntity.FieldName, organizationName),
                new KeyValuePair<string, object>(BaseOrganizationEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0)
            };
            var organizationManager = new BaseOrganizationManager(UserInfo);
            var organizationId = organizationManager.GetId(parameters);
            if (string.IsNullOrEmpty(organizationId))
            {
                return result;
            }
            // 用户组织机构关联关系
            var organizationIds = GetAllOrganizationIds(userId);
            if (organizationIds == null || organizationIds.Length == 0)
            {
                return result;
            }
            // 用户的部门是否存在这些部门里
            result = StringUtil.Exists(organizationIds, organizationId);
            return result;
        }

        #region public string[] GetOrganizationIds(string userId) 获取用户的所有所在部门主键数组
        /// <summary>
        /// 获取用户的所有所在部门主键数组
        /// 2015-12-02 吉日嘎拉，优化方法。
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns>主键数组</returns>
        public string[] GetAllOrganizationIds(string userId)
        {
            string[] result = null;

            var errorMark = 0;
            /*
            // 被删除的不应该显示出来
            string sql = string.Format(
                             @"SELECT CompanyId AS Id
  FROM BaseUser
                                 WHERE " + BaseUserOrganizationEntity.FieldDeleted + " = 0 AND Enabled =1 AND CompanyId IS NOT NULL  AND (Id = {0})
                                 UNION
                                SELECT SubCompanyId AS Id
  FROM BaseUser
                                 WHERE " + BaseUserOrganizationEntity.FieldDeleted + " = 0 AND " + BaseUserOrganizationEntity.FieldEnabled + " =1 AND CompanyId IS NOT NULL  AND (Id = {0})
                                 UNION
                                SELECT DepartmentId AS Id
  FROM BaseUser
                                 WHERE " + BaseUserOrganizationEntity.FieldDeleted + " = 0 AND " + BaseUserOrganizationEntity.FieldEnabled + " =1  AND DepartmentId IS NOT NULL AND (Id = {0})
                                 UNION
                                SELECT SubDepartmentId AS Id
  FROM BaseUser
                                 WHERE " + BaseUserOrganizationEntity.FieldDeleted + " = 0 AND " + BaseUserOrganizationEntity.FieldEnabled + " =1  AND SubDepartmentId IS NOT NULL AND (Id = {0})
                                 UNION
                                SELECT WorkgroupId AS Id
  FROM BaseUser
                                 WHERE " + BaseUserOrganizationEntity.FieldDeleted + " = 0 AND " + BaseUserOrganizationEntity.FieldEnabled + " =1  AND WorkgroupId IS NOT NULL AND (Id = {0})
                                 UNION

                                SELECT CompanyId AS Id
  FROM BaseUserOrganization
                                 WHERE " + BaseUserOrganizationEntity.FieldDeleted + " = 0 AND " + BaseUserOrganizationEntity.FieldEnabled + " =1  AND CompanyId IS NOT NULL AND (UserId = {0})
                                 UNION
                                SELECT SubCompanyId AS Id
  FROM BaseUserOrganization
                                 WHERE " + BaseUserOrganizationEntity.FieldDeleted + " = 0 AND " + BaseUserOrganizationEntity.FieldEnabled + " =1  AND CompanyId IS NOT NULL AND (UserId = {0})
                                 UNION
                                SELECT DepartmentId AS Id
  FROM BaseUserOrganization
                                 WHERE " + BaseUserOrganizationEntity.FieldDeleted + " = 0 AND " + BaseUserOrganizationEntity.FieldEnabled + " =1  AND DepartmentId IS NOT NULL AND (UserId = {0})
                                 UNION
                                SELECT SubDepartmentId AS Id
  FROM BaseUserOrganization
                                 WHERE " + BaseUserOrganizationEntity.FieldDeleted + " = 0 AND " + BaseUserOrganizationEntity.FieldEnabled + " =1  AND SubDepartmentId IS NOT NULL AND (UserId = {0})
                                 UNION
                                SELECT WorkgroupId AS Id
  FROM BaseUserOrganization
                                 WHERE " + BaseUserOrganizationEntity.FieldDeleted + " = 0 AND " + BaseUserOrganizationEntity.FieldEnabled + " =1  AND WorkgroupId IS NOT NULL AND (UserId = {0}) ", userId);
            */

            var commandText = @"SELECT CompanyId AS Id
  FROM BaseUser 
                                 WHERE " + BaseUserEntity.FieldDeleted + " = 0 AND " + BaseUserEntity.FieldEnabled + " = 1 AND CompanyId IS NOT NULL AND Id = " + DbHelper.GetParameter(BaseUserEntity.FieldId) + @"
                                 UNION
                                SELECT CompanyId AS Id
  FROM BaseUserOrganization
                                 WHERE " + BaseUserOrganizationEntity.FieldDeleted + " = 0 AND " + BaseUserOrganizationEntity.FieldEnabled + " = 1  AND CompanyId IS NOT NULL AND UserId = " + DbHelper.GetParameter(BaseUserOrganizationEntity.FieldUserId);

            /*
            var dt = DbHelper.Fill(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                result = BaseUtil.FieldToArray(dt, BaseUserEntity.FieldId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
            }
            */
            // 2015-12-02 吉日嘎拉 方法优化，采用 ExecuteReader 提高效率，减少内存使用。
            var dbParameters = new List<IDbDataParameter>
            {
                DbHelper.MakeParameter(BaseUserEntity.FieldId, userId),
                DbHelper.MakeParameter(BaseUserOrganizationEntity.FieldUserId, userId)
            };
            var ids = new List<string>();
            try
            {
                errorMark = 1;
                var dataReader = DbHelper.ExecuteReader(commandText, dbParameters.ToArray());
                if (dataReader != null && !dataReader.IsClosed)
                {
                    while (dataReader.Read())
                    {
                        ids.Add(dataReader[BaseOrganizationEntity.FieldId].ToString());
                    }

                    dataReader.Close();
                }

                result = ids.ToArray();
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

        /// <summary>
        /// 根据部门获取数据表
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public DataTable GetDataTableByDepartment(string departmentId)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + SelectFields
                + " FROM " + BaseUserEntity.CurrentTableName);

            sb.Append(" WHERE (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDeleted + " = 0");
            sb.Append(" AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldEnabled + " = 1 ) ");

            if (!string.IsNullOrEmpty(departmentId))
            {
                // 从用户表
                sb.Append(" AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDepartmentId + " = '" + departmentId + "') ");
                // 从兼职表读取用户 
                /*
                sb.Append(" OR " + BaseUserEntity.FieldId + " IN ("
                        + "SELECT " + BaseUserOrganizationEntity.FieldUserId
                        + " FROM " + BaseUserOrganizationEntity.CurrentTableName
                        + " WHERE (" + BaseUserOrganizationEntity.CurrentTableName + "." + BaseUserOrganizationEntity.FieldDeleted + " = 0 ) "
                        + "       AND (" + BaseUserOrganizationEntity.CurrentTableName + "." + BaseUserOrganizationEntity.FieldDepartmentId + " = '" + departmentId + "')) ");
                 */


            }
            sb.Append(" ORDER BY " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldSortCode);
            return DbHelper.Fill(sb.Return());
        }

        /// <summary>
        /// 更新省份城市
        /// </summary>
        /// <param name="all"></param>
        /// <returns></returns>
        public int UpdateProvinceCity(bool all = true)
        {
            var result = 0;
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("UPDATE " + BaseUserEntity.CurrentTableName
                              + " SET " + BaseUserEntity.FieldProvince + " = ( SELECT " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldProvince
                             + " FROM " + BaseOrganizationEntity.CurrentTableName
                            + " WHERE " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldId + " = " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldCompanyId + ")");
            if (!all)
            {
                sb.Append(" AND (" + BaseUserEntity.FieldProvince + " IS NULL)");
            }

            result = ExecuteNonQuery(sb.Return());
            sb = PoolUtil.StringBuilder.Get();
            sb.Append("UPDATE " + BaseUserEntity.CurrentTableName
                              + " SET " + BaseUserEntity.FieldCity + " = ( SELECT " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldCity
                             + " FROM " + BaseOrganizationEntity.CurrentTableName
                            + " WHERE " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldId + " = " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldCompanyId + ")");
            if (!all)
            {
                sb.Append(" AND (" + BaseUserEntity.FieldCity + " IS NULL)");
            }
            result = ExecuteNonQuery(sb.Return());
            sb = PoolUtil.StringBuilder.Get();
            sb.Append("UPDATE " + BaseUserEntity.CurrentTableName
                              + " SET " + BaseUserEntity.FieldDistrict + " = ( SELECT " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldDistrict
                             + " FROM " + BaseOrganizationEntity.CurrentTableName
                            + " WHERE " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldId + " = " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldCompanyId + ")");
            if (!all)
            {
                sb.Append(" AND (" + BaseUserEntity.FieldProvince + " IS NULL OR " + BaseUserEntity.FieldDistrict + " IS NULL)");
            }

            result = ExecuteNonQuery(sb.Return());

            return result;
        }

        #region public List<BaseUserEntity> GetListByDepartment(string departmentId)
        /// <summary>
        /// 按部门获取用户
        /// </summary>
        /// <param name="departmentId">部门主键</param>
        /// <returns>数据表</returns>
        public List<BaseUserEntity> GetListByDepartment(string departmentId)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + BaseUserEntity.CurrentTableName + ".* "
                + " FROM " + BaseUserEntity.CurrentTableName);

            sb.Append(" WHERE (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDeleted + " = 0");
            sb.Append(" AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldEnabled + " = 1 ) ");

            if (!string.IsNullOrEmpty(departmentId))
            {
                // 从用户表
                sb.Append(" AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDepartmentId + " = '" + departmentId + "') ");
                // 从兼职表读取用户 
                /*
                sb.Append(" OR " + BaseUserEntity.FieldId + " IN ("
                        + "SELECT " + BaseUserOrganizationEntity.FieldUserId
                        + " FROM " + BaseUserOrganizationEntity.CurrentTableName
                        + " WHERE (" + BaseUserOrganizationEntity.CurrentTableName + "." + BaseUserOrganizationEntity.FieldDeleted + " = 0 ) "
                        + "       AND (" + BaseUserOrganizationEntity.CurrentTableName + "." + BaseUserOrganizationEntity.FieldDepartmentId + " = '" + departmentId + "')) ");
                */
            }
            sb.Append(" ORDER BY " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldSortCode);

            using (var dr = DbHelper.ExecuteReader(sb.Return()))
            {
                return GetList<BaseUserEntity>(dr);
            }
        }
        #endregion

        #region public List<BaseUserEntity> GetListByCompany(string companyId)
        /// <summary>
        /// 按公司获取用户
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <returns>数据表</returns>
        public List<BaseUserEntity> GetListByCompany(string companyId)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + BaseUserEntity.CurrentTableName + ".* "
                + " FROM " + BaseUserEntity.CurrentTableName);

            sb.Append(" WHERE (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDeleted + " = 0");
            sb.Append(" AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldEnabled + " = 1 ) ");

            if (!string.IsNullOrEmpty(companyId))
            {
                // 从用户表
                sb.Append(" AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldCompanyId + " = '" + companyId + "') ");
                // 从兼职表读取用户 
                /*
                sb.Append(" OR " + BaseUserEntity.FieldId + " IN ("
                        + "SELECT " + BaseUserOrganizationEntity.FieldUserId
                        + " FROM " + BaseUserOrganizationEntity.CurrentTableName
                        + " WHERE (" + BaseUserOrganizationEntity.CurrentTableName + "." + BaseUserOrganizationEntity.FieldDeleted + " = 0 ) "
                        + "       AND (" + BaseUserOrganizationEntity.CurrentTableName + "." + BaseUserOrganizationEntity.FieldCompanyId + " = '" + companyId + "')) ");
                */
            }
            sb.Append(" ORDER BY " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldSortCode);

            using (var dr = DbHelper.ExecuteReader(sb.Return()))
            {
                return GetList<BaseUserEntity>(dr);
            }
        }
        #endregion

        private string GetUserSql(string[] organizationIds, bool idOnly = false)
        {
            var field = idOnly ? BaseUserEntity.FieldId : "*";
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + field
                + " FROM " + BaseUserEntity.CurrentTableName
                // 从用户表里去找
                + " WHERE " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDeleted + " = 0"
                + " AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldEnabled + " = 1"
                + " AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldWorkgroupId + " IN ( " + StringUtil.ArrayToList(organizationIds) + ") "
                + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDepartmentId + " IN (" + StringUtil.ArrayToList(organizationIds) + ") "
                + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldSubCompanyId + " IN (" + StringUtil.ArrayToList(organizationIds) + ") "
                + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldCompanyId + " IN (" + StringUtil.ArrayToList(organizationIds) + ")) "
                // 从用户兼职表里去取用户
                /*
                + " OR " + BaseUserEntity.FieldId + " IN ("
                        + "SELECT " + BaseUserOrganizationEntity.FieldUserId
                        + " FROM " + BaseUserOrganizationEntity.CurrentTableName
                        + " WHERE (" + BaseUserOrganizationEntity.CurrentTableName + "." + BaseUserOrganizationEntity.FieldDeleted + " = 0 ) "
                        + "       AND (" + BaseUserOrganizationEntity.CurrentTableName + "." + BaseUserOrganizationEntity.FieldWorkgroupId + " IN ( " + StringUtil.ArrayToList(organizationIds) + ") "
                        + "             OR " + BaseUserOrganizationEntity.CurrentTableName + "." + BaseUserOrganizationEntity.FieldDepartmentId + " IN (" + StringUtil.ArrayToList(organizationIds) + ") "
                        + "             OR " + BaseUserOrganizationEntity.CurrentTableName + "." + BaseUserOrganizationEntity.FieldSubCompanyId + " IN (" + StringUtil.ArrayToList(organizationIds) + ") "
                        + "             OR " + BaseUserOrganizationEntity.CurrentTableName + "." + BaseUserOrganizationEntity.FieldCompanyId + " IN (" + StringUtil.ArrayToList(organizationIds) + "))) "
                */
                + " ORDER BY " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldSortCode);
            return sb.Return();
        }

        #region public List<BaseUserEntity> GetDataTableByOrganizations(string[] ids) 按工作组、部门、公司获用户列表
        /// <summary>
        /// 按工作组、部门、公司获用户列表
        /// </summary>
        /// <param name="organizationIds">主键数组</param>
        /// <returns>数据表</returns>
        public List<BaseUserEntity> GetListByOrganizations(string[] organizationIds)
        {
            var sql = GetUserSql(organizationIds, false);
            using (var dr = DbHelper.ExecuteReader(sql))
            {
                return GetList<BaseUserEntity>(dr);
            }
        }
        #endregion

        /// <summary>
        /// 根据组织获取数据表
        /// </summary>
        /// <param name="organizationIds"></param>
        /// <returns></returns>
        public DataTable GetDataTableByOrganizations(string[] organizationIds)
        {
            var sql = GetUserSql(organizationIds, false);
            return DbHelper.Fill(sql);
        }

        /// <summary>
        /// 获取用户编号
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="organizationIds"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public string[] GetUserIds(string[] userIds, string[] organizationIds, string[] roleIds)
        {
            string[] companyUsers = null;

            if (organizationIds != null && organizationIds.Length > 0)
            {
                var sql = GetUserSql(organizationIds, true);
                var dt = DbHelper.Fill(sql);
                companyUsers = BaseUtil.FieldToArray(dt, BaseUserEntity.FieldId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
            }

            string[] roleUsers = null;
            if (roleIds != null && roleIds.Length > 0)
            {
                roleUsers = GetUserIds(roleIds);
            }
            // userIds = StringUtil.Concat(userIds, companyUsers, departmentUsers, workgroupUsers, roleUsers);
            userIds = StringUtil.Concat(userIds, companyUsers, roleUsers);
            return userIds;
        }

        #region public DataTable SearchByDepartment(string departmentId, string searchKey)  按部门获取部门用户,包括子部门的用户
        /// <summary>
        /// 按部门获取部门用户,包括子部门的用户
        /// </summary>
        /// <param name="departmentId">部门主键</param>
        /// <param name="searchKey">关键字</param>
        /// <returns>数据表</returns>
        public DataTable SearchByDepartment(string departmentId, string searchKey)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + BaseUserEntity.CurrentTableName + ".* "
                + " FROM " + BaseUserEntity.CurrentTableName);
            sb.Append(" WHERE (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDeleted + " = 0");
            sb.Append(" AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldEnabled + " = 1 ) ");
            if (!string.IsNullOrEmpty(departmentId))
            {
                var organizationManager = new BaseOrganizationManager(DbHelper, UserInfo);
                var organizationIds = organizationManager.GetChildrensId(BaseOrganizationEntity.FieldId, departmentId, BaseOrganizationEntity.FieldParentId);
                if (organizationIds != null && organizationIds.Length > 0)
                {
                    sb.Append(" AND (" + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldCompanyId + " IN (" + StringUtil.ArrayToList(organizationIds) + ")"
                     + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldDepartmentId + " IN (" + StringUtil.ArrayToList(organizationIds) + ")"
                     + " OR " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldWorkgroupId + " IN (" + StringUtil.ArrayToList(organizationIds) + "))");
                }
            }
            var dbParameters = new List<IDbDataParameter>();
            searchKey = searchKey.Trim();
            if (!string.IsNullOrEmpty(searchKey))
            {
                sb.Append(" AND (" + BaseUserEntity.FieldUserName + " LIKE " + DbHelper.GetParameter(BaseUserEntity.FieldUserName));
                sb.Append(" OR " + BaseUserEntity.FieldCode + " LIKE " + DbHelper.GetParameter(BaseUserEntity.FieldCode));
                sb.Append(" OR " + BaseUserEntity.FieldRealName + " LIKE " + DbHelper.GetParameter(BaseUserEntity.FieldRealName));
                sb.Append(" OR " + BaseUserEntity.FieldDepartmentName + " LIKE " + DbHelper.GetParameter(BaseUserEntity.FieldDepartmentName) + ")");
                if (searchKey.IndexOf("%") < 0)
                {
                    searchKey = "%" + searchKey + "%";
                }
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldUserName, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldCode, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldRealName, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldDepartmentName, searchKey));
            }
            sb.Append(" ORDER BY " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldSortCode);
            return DbHelper.Fill(sb.Return(), dbParameters.ToArray());
        }
        #endregion

        /// <summary>
        /// 获取下级的用户清单
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public List<BaseUserEntity> GetChildrenUserList(string organizationId)
        {
            string[] organizationIds = null;
            var organizationManager = new BaseOrganizationManager(DbHelper, UserInfo);
            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.Access:
                case CurrentDbType.SqlServer:
                    var organizationCode = organizationManager.GetCodeById(organizationId);
                    organizationIds = organizationManager.GetChildrensIdByCode(BaseOrganizationEntity.FieldCode, organizationCode);
                    break;
                case CurrentDbType.Oracle:
                    organizationIds = organizationManager.GetChildrensId(BaseOrganizationEntity.FieldId, organizationId, BaseOrganizationEntity.FieldParentId);
                    break;
            }
            return GetListByOrganizations(organizationIds);
        }

        /// <summary>
        /// 获取下级的用户表
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataTable GetChildrenUserDataTable(string organizationId)
        {
            string[] organizationIds = null;
            var organizationManager = new BaseOrganizationManager(DbHelper, UserInfo);
            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.Access:
                case CurrentDbType.SqlServer:
                    var organizationCode = organizationManager.GetCodeById(organizationId);
                    organizationIds = organizationManager.GetChildrensIdByCode(BaseOrganizationEntity.FieldCode, organizationCode);
                    break;
                case CurrentDbType.Oracle:
                    organizationIds = organizationManager.GetChildrensId(BaseOrganizationEntity.FieldId, organizationId, BaseOrganizationEntity.FieldParentId);
                    break;
            }
            return GetDataTableByOrganizations(organizationIds);
        }
    }
}