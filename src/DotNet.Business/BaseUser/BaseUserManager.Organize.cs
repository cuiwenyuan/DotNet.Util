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
        /// <param name="organizeName">部门名称</param>
        /// <returns>存在</returns>
        public bool IsInOrganize(string organizeName)
        {
            return IsInOrganize(UserInfo.Id, organizeName);
        }

        /// <summary>
        /// 用户是否在某部门
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="organizeName">部门名称</param>
        /// <returns>存在</returns>
        public bool IsInOrganize(string userId, string organizeName)
        {
            var result = false;
            // 把部门的主键找出来
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseOrganizeEntity.FieldFullName, organizeName),
                new KeyValuePair<string, object>(BaseOrganizeEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseOrganizeEntity.FieldDeleted, 0)
            };
            var organizeManager = new BaseOrganizeManager(UserInfo);
            var organizationId = organizeManager.GetId(parameters);
            if (string.IsNullOrEmpty(organizationId))
            {
                return result;
            }
            // 用户组织机构关联关系
            var organizationIds = GetAllOrganizeIds(userId);
            if (organizationIds == null || organizationIds.Length == 0)
            {
                return result;
            }
            // 用户的部门是否存在这些部门里
            result = StringUtil.Exists(organizationIds, organizationId);
            return result;
        }

        #region public string[] GetOrganizeIds(string userId) 获取用户的所有所在部门主键数组
        /// <summary>
        /// 获取用户的所有所在部门主键数组
        /// 2015-12-02 吉日嘎拉，优化方法。
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns>主键数组</returns>
        public string[] GetAllOrganizeIds(string userId)
        {
            string[] result = null;

            var errorMark = 0;
            /*
            // 被删除的不应该显示出来
            string sql = string.Format(
                             @"SELECT CompanyId AS Id
  FROM BaseUser
                                 WHERE " + BaseUserOrganizeEntity.FieldDeleted + " = 0 AND Enabled =1 AND CompanyId IS NOT NULL  AND (Id = {0})
                                 UNION
                                SELECT SubCompanyId AS Id
  FROM BaseUser
                                 WHERE " + BaseUserOrganizeEntity.FieldDeleted + " = 0 AND " + BaseUserOrganizeEntity.FieldEnabled + " =1 AND CompanyId IS NOT NULL  AND (Id = {0})
                                 UNION
                                SELECT DepartmentId AS Id
  FROM BaseUser
                                 WHERE " + BaseUserOrganizeEntity.FieldDeleted + " = 0 AND " + BaseUserOrganizeEntity.FieldEnabled + " =1  AND DepartmentId IS NOT NULL AND (Id = {0})
                                 UNION
                                SELECT SubDepartmentId AS Id
  FROM BaseUser
                                 WHERE " + BaseUserOrganizeEntity.FieldDeleted + " = 0 AND " + BaseUserOrganizeEntity.FieldEnabled + " =1  AND SubDepartmentId IS NOT NULL AND (Id = {0})
                                 UNION
                                SELECT WorkgroupId AS Id
  FROM BaseUser
                                 WHERE " + BaseUserOrganizeEntity.FieldDeleted + " = 0 AND " + BaseUserOrganizeEntity.FieldEnabled + " =1  AND WorkgroupId IS NOT NULL AND (Id = {0})
                                 UNION

                                SELECT CompanyId AS Id
  FROM BaseUserOrganize
                                 WHERE " + BaseUserOrganizeEntity.FieldDeleted + " = 0 AND " + BaseUserOrganizeEntity.FieldEnabled + " =1  AND CompanyId IS NOT NULL AND (UserId = {0})
                                 UNION
                                SELECT SubCompanyId AS Id
  FROM BaseUserOrganize
                                 WHERE " + BaseUserOrganizeEntity.FieldDeleted + " = 0 AND " + BaseUserOrganizeEntity.FieldEnabled + " =1  AND CompanyId IS NOT NULL AND (UserId = {0})
                                 UNION
                                SELECT DepartmentId AS Id
  FROM BaseUserOrganize
                                 WHERE " + BaseUserOrganizeEntity.FieldDeleted + " = 0 AND " + BaseUserOrganizeEntity.FieldEnabled + " =1  AND DepartmentId IS NOT NULL AND (UserId = {0})
                                 UNION
                                SELECT SubDepartmentId AS Id
  FROM BaseUserOrganize
                                 WHERE " + BaseUserOrganizeEntity.FieldDeleted + " = 0 AND " + BaseUserOrganizeEntity.FieldEnabled + " =1  AND SubDepartmentId IS NOT NULL AND (UserId = {0})
                                 UNION
                                SELECT WorkgroupId AS Id
  FROM BaseUserOrganize
                                 WHERE " + BaseUserOrganizeEntity.FieldDeleted + " = 0 AND " + BaseUserOrganizeEntity.FieldEnabled + " =1  AND WorkgroupId IS NOT NULL AND (UserId = {0}) ", userId);
            */

            var commandText = @"SELECT CompanyId AS Id
  FROM BaseUser 
                                 WHERE " + BaseUserEntity.FieldDeleted + " = 0 AND " + BaseUserEntity.FieldEnabled + " = 1 AND CompanyId IS NOT NULL AND Id = " + DbHelper.GetParameter(BaseUserEntity.FieldId) + @"
                                 UNION
                                SELECT CompanyId AS Id
  FROM BaseUserOrganize
                                 WHERE " + BaseUserOrganizeEntity.FieldDeleted + " = 0 AND " + BaseUserOrganizeEntity.FieldEnabled + " = 1  AND CompanyId IS NOT NULL AND UserId = " + DbHelper.GetParameter(BaseUserOrganizeEntity.FieldUserId);

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
                DbHelper.MakeParameter(BaseUserOrganizeEntity.FieldUserId, userId)
            };
            var ids = new List<string>();
            try
            {
                errorMark = 1;
                using (var dataReader = DbHelper.ExecuteReader(commandText, dbParameters.ToArray()))
                {
                    while (dataReader.Read())
                    {
                        ids.Add(dataReader[BaseOrganizeEntity.FieldId].ToString());
                    }
                }
                result = ids.ToArray();
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

        /// <summary>
        /// 根据部门获取数据表
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public DataTable GetDataTableByDepartment(string departmentId)
        {
            var sql = "SELECT " + SelectFields
                + " FROM " + BaseUserEntity.TableName;

            sql += " WHERE (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDeleted + " = 0 ";
            sql += " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldEnabled + " = 1 ) ";

            if (!string.IsNullOrEmpty(departmentId))
            {
                // 从用户表
                sql += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDepartmentId + " = '" + departmentId + "') ";
                // 从兼职表读取用户 
                /*
                sql += " OR " + BaseUserEntity.FieldId + " IN ("
                        + "SELECT " + BaseUserOrganizeEntity.FieldUserId
                        + " FROM " + BaseUserOrganizeEntity.TableName
                        + "  WHERE (" + BaseUserOrganizeEntity.TableName + "." + BaseUserOrganizeEntity.FieldDeleted + " = 0 ) "
                        + "       AND (" + BaseUserOrganizeEntity.TableName + "." + BaseUserOrganizeEntity.FieldDepartmentId + " = '" + departmentId + "')) ";
                 */


            }
            sql += " ORDER BY " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldSortCode;
            return DbHelper.Fill(sql);
        }

        /// <summary>
        /// 更新省份城市
        /// </summary>
        /// <param name="all"></param>
        /// <returns></returns>
        public int UpdateProvinceCity(bool all = true)
        {
            var result = 0;

            var sql = "UPDATE " + BaseUserEntity.TableName
                              + " SET " + BaseUserEntity.FieldProvince + " = ( SELECT " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldProvince
                             + " FROM " + BaseOrganizeEntity.TableName
                            + " WHERE " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldId + " = " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldCompanyId + ")";
            if (!all)
            {
                sql += " AND (" + BaseUserEntity.FieldProvince + " IS NULL)";
            }

            result = DbHelper.ExecuteNonQuery(sql);

            sql = "UPDATE " + BaseUserEntity.TableName
                              + " SET " + BaseUserEntity.FieldCity + " = ( SELECT " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldCity
                             + " FROM " + BaseOrganizeEntity.TableName
                            + " WHERE " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldId + " = " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldCompanyId + ")";
            if (!all)
            {
                sql += " AND (" + BaseUserEntity.FieldCity + " IS NULL)";
            }

            sql = "UPDATE " + BaseUserEntity.TableName
                              + " SET " + BaseUserEntity.FieldDistrict + " = ( SELECT " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldDistrict
                             + " FROM " + BaseOrganizeEntity.TableName
                            + " WHERE " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldId + " = " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldCompanyId + ")";
            if (!all)
            {
                sql += " AND (" + BaseUserEntity.FieldProvince + " IS NULL OR " + BaseUserEntity.FieldDistrict + " IS NULL)";
            }

            result = DbHelper.ExecuteNonQuery(sql);

            return result;
        }

        /// <summary>
        /// 根据公司获取数据表
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public DataTable GetDataTableByCompany(string companyId)
        {
            var sql = "SELECT " + SelectFields
                + " FROM " + BaseUserEntity.TableName;

            sql += " WHERE (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDeleted + " = 0 ";
            sql += " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldEnabled + " = 1 ) ";

            if (!string.IsNullOrEmpty(companyId))
            {
                // 从用户表
                sql += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldCompanyId + " = '" + companyId + "') ";
                // 从兼职表读取用户
                /*
                sql += " OR " + BaseUserEntity.FieldId + " IN ("
                        + "SELECT " + BaseUserOrganizeEntity.FieldUserId
                        + " FROM " + BaseUserOrganizeEntity.TableName
                        + "  WHERE (" + BaseUserOrganizeEntity.TableName + "." + BaseUserOrganizeEntity.FieldDeleted + " = 0 ) "
                        + "       AND (" + BaseUserOrganizeEntity.TableName + "." + BaseUserOrganizeEntity.FieldCompanyId + " = '" + companyId + "')) ";
                */
            }
            sql += " ORDER BY " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldSortCode;
            return DbHelper.Fill(sql);
        }

        #region public List<BaseUserEntity> GetListByDepartment(string departmentId)
        /// <summary>
        /// 按部门获取用户
        /// </summary>
        /// <param name="departmentId">部门主键</param>
        /// <returns>数据表</returns>
        public List<BaseUserEntity> GetListByDepartment(string departmentId)
        {
            var sql = "SELECT " + BaseUserEntity.TableName + ".* "
                + " FROM " + BaseUserEntity.TableName;

            sql += " WHERE (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDeleted + " = 0 ";
            sql += " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldEnabled + " = 1 ) ";

            if (!string.IsNullOrEmpty(departmentId))
            {
                // 从用户表
                sql += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDepartmentId + " = '" + departmentId + "') ";
                // 从兼职表读取用户 
                /*
                sql += " OR " + BaseUserEntity.FieldId + " IN ("
                        + "SELECT " + BaseUserOrganizeEntity.FieldUserId
                        + " FROM " + BaseUserOrganizeEntity.TableName
                        + "  WHERE (" + BaseUserOrganizeEntity.TableName + "." + BaseUserOrganizeEntity.FieldDeleted + " = 0 ) "
                        + "       AND (" + BaseUserOrganizeEntity.TableName + "." + BaseUserOrganizeEntity.FieldDepartmentId + " = '" + departmentId + "')) ";
                */
            }
            sql += " ORDER BY " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldSortCode;

            using (var dr = DbHelper.ExecuteReader(sql))
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
            var sql = "SELECT " + BaseUserEntity.TableName + ".* "
                + " FROM " + BaseUserEntity.TableName;

            sql += " WHERE (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDeleted + " = 0 ";
            sql += " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldEnabled + " = 1 ) ";

            if (!string.IsNullOrEmpty(companyId))
            {
                // 从用户表
                sql += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldCompanyId + " = '" + companyId + "') ";
                // 从兼职表读取用户 
                /*
                sql += " OR " + BaseUserEntity.FieldId + " IN ("
                        + "SELECT " + BaseUserOrganizeEntity.FieldUserId
                        + " FROM " + BaseUserOrganizeEntity.TableName
                        + "  WHERE (" + BaseUserOrganizeEntity.TableName + "." + BaseUserOrganizeEntity.FieldDeleted + " = 0 ) "
                        + "       AND (" + BaseUserOrganizeEntity.TableName + "." + BaseUserOrganizeEntity.FieldCompanyId + " = '" + companyId + "')) ";
                */
            }
            sql += " ORDER BY " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldSortCode;

            using (var dr = DbHelper.ExecuteReader(sql))
            {
                return GetList<BaseUserEntity>(dr);
            }
        }
        #endregion

        private string GetUserSql(string[] organizationIds, bool idOnly = false)
        {
            var field = idOnly ? BaseUserEntity.FieldId : "*";
            var organizeList = string.Join(",", organizationIds);
            var sql = "SELECT " + field
                + " FROM " + BaseUserEntity.TableName
                // 从用户表里去找
                + " WHERE " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDeleted + " = 0 "
                + "       AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldEnabled + " = 1 "
                + "       AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldWorkgroupId + " IN ( " + organizeList + ") "
                + "             OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDepartmentId + " IN (" + organizeList + ") "
                + "             OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldSubCompanyId + " IN (" + organizeList + ") "
                + "             OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldCompanyId + " IN (" + organizeList + ")) "
                // 从用户兼职表里去取用户
                /*
                + " OR " + BaseUserEntity.FieldId + " IN ("
                        + "SELECT " + BaseUserOrganizeEntity.FieldUserId
                        + " FROM " + BaseUserOrganizeEntity.TableName
                        + "  WHERE (" + BaseUserOrganizeEntity.TableName + "." + BaseUserOrganizeEntity.FieldDeleted + " = 0 ) "
                        + "       AND (" + BaseUserOrganizeEntity.TableName + "." + BaseUserOrganizeEntity.FieldWorkgroupId + " IN ( " + organizeList + ") "
                        + "             OR " + BaseUserOrganizeEntity.TableName + "." + BaseUserOrganizeEntity.FieldDepartmentId + " IN (" + organizeList + ") "
                        + "             OR " + BaseUserOrganizeEntity.TableName + "." + BaseUserOrganizeEntity.FieldSubCompanyId + " IN (" + organizeList + ") "
                        + "             OR " + BaseUserOrganizeEntity.TableName + "." + BaseUserOrganizeEntity.FieldCompanyId + " IN (" + organizeList + "))) "
                */
                + " ORDER BY " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldSortCode;
            return sql;
        }

        #region public List<BaseUserEntity> GetDataTableByOrganizes(string[] ids) 按工作组、部门、公司获用户列表
        /// <summary>
        /// 按工作组、部门、公司获用户列表
        /// </summary>
        /// <param name="organizationIds">主键数组</param>
        /// <returns>数据表</returns>
        public List<BaseUserEntity> GetListByOrganizes(string[] organizationIds)
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
        public DataTable GetDataTableByOrganizes(string[] organizationIds)
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
            /*
            // 要注意不能重复发信息，只能发一次。
            // 按公司查找用户
            string[] companyUsers = null;
            // 按部门查找用户
            string[] departmentUsers = null; 
            // 按工作组查找用户
            string[] workgroupUsers = null; 
            if (ids != null && ids.Length > 0)
            {
                // 这里获得的是用户主键，不是员工主键
                companyUsers = this.GetIds(new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
                    , new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1)
                    , new KeyValuePair<string, object>(BaseUserEntity.FieldCompanyId, ids));
                subCompanyUsers = this.GetIds(new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
                    , new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1)
                    , new KeyValuePair<string, object>(BaseUserEntity.FieldSubCompanyId, ids));
                departmentUsers = this.GetIds(new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
                    , new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1)
                    , new KeyValuePair<string, object>(BaseUserEntity.FieldDepartmentId, ids));
                workgroupUsers = this.GetIds(new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
                    , new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1)
                    , new KeyValuePair<string, object>(BaseUserEntity.FieldWorkgroupId, ids));
            }
            */

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
            var sql = "SELECT " + BaseUserEntity.TableName + ".* "
                + " FROM " + BaseUserEntity.TableName;
            sql += " WHERE (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDeleted + " = 0 ";
            sql += " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldEnabled + " = 1 ) ";
            if (!string.IsNullOrEmpty(departmentId))
            {
                /*
                用非递归调用的建议方法
                sql += " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDepartmentId 
                    + " IN ( SELECT " + BaseOrganizeEntity.FieldId 
                    + " FROM " + BaseOrganizeEntity.TableName 
                    + " WHERE " + BaseOrganizeEntity.FieldId + " = " + departmentId + " OR " + BaseOrganizeEntity.FieldParentId + " = " + departmentId + ")";
                */
                var organizeManager = new BaseOrganizeManager(DbHelper, UserInfo);
                var organizationIds = organizeManager.GetChildrensId(BaseOrganizeEntity.FieldId, departmentId, BaseOrganizeEntity.FieldParentId);
                if (organizationIds != null && organizationIds.Length > 0)
                {
                    sql += " AND (" + BaseUserEntity.TableName + "." + BaseUserEntity.FieldCompanyId + " IN (" + StringUtil.ArrayToList(organizationIds) + ")"
                     + " OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDepartmentId + " IN (" + StringUtil.ArrayToList(organizationIds) + ")"
                     + " OR " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldWorkgroupId + " IN (" + StringUtil.ArrayToList(organizationIds) + "))";
                }
            }
            var dbParameters = new List<IDbDataParameter>();
            searchKey = searchKey.Trim();
            if (!string.IsNullOrEmpty(searchKey))
            {
                sql += " AND (" + BaseUserEntity.FieldUserName + " LIKE " + DbHelper.GetParameter(BaseUserEntity.FieldUserName);
                sql += " OR " + BaseUserEntity.FieldCode + " LIKE " + DbHelper.GetParameter(BaseUserEntity.FieldCode);
                sql += " OR " + BaseUserEntity.FieldRealName + " LIKE " + DbHelper.GetParameter(BaseUserEntity.FieldRealName);
                sql += " OR " + BaseUserEntity.FieldDepartmentName + " LIKE " + DbHelper.GetParameter(BaseUserEntity.FieldDepartmentName) + ")";
                if (searchKey.IndexOf("%") < 0)
                {
                    searchKey = "%" + searchKey + "%";
                }
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldUserName, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldCode, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldRealName, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldDepartmentName, searchKey));
            }
            sql += " ORDER BY " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldSortCode;
            return DbHelper.Fill(sql, dbParameters.ToArray());
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
            var organizeManager = new BaseOrganizeManager(DbHelper, UserInfo);
            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.Access:
                case CurrentDbType.SqlServer:
                    var organizeCode = organizeManager.GetCodeById(organizationId);
                    organizationIds = organizeManager.GetChildrensIdByCode(BaseOrganizeEntity.FieldCode, organizeCode);
                    break;
                case CurrentDbType.Oracle:
                    organizationIds = organizeManager.GetChildrensId(BaseOrganizeEntity.FieldId, organizationId, BaseOrganizeEntity.FieldParentId);
                    break;
            }
            return GetListByOrganizes(organizationIds);
        }

        /// <summary>
        /// 获取下级的用户表
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataTable GetChildrenUserDataTable(string organizationId)
        {
            string[] organizationIds = null;
            var organizeManager = new BaseOrganizeManager(DbHelper, UserInfo);
            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.Access:
                case CurrentDbType.SqlServer:
                    var organizeCode = organizeManager.GetCodeById(organizationId);
                    organizationIds = organizeManager.GetChildrensIdByCode(BaseOrganizeEntity.FieldCode, organizeCode);
                    break;
                case CurrentDbType.Oracle:
                    organizationIds = organizeManager.GetChildrensId(BaseOrganizeEntity.FieldId, organizationId, BaseOrganizeEntity.FieldParentId);
                    break;
            }
            return GetDataTableByOrganizes(organizationIds);
        }
    }
}