//-----------------------------------------------------------------------
// <copyright file="BaseStaffManager.cs" company="DotNet">
//     Copyright (c) 2025, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Business;
    using Util;

    /// <summary>
    /// BaseStaffManager
    /// 员工管理层
    /// 
    /// 修改记录
    /// 
    ///	2024-07-16 版本：1.0 Troy.Cui 创建文件。
    ///		
    /// <author>
    ///	<name>Troy.Cui</name>
    ///	<date>2024-07-16</date>
    /// </author> 
    /// </summary>
    public partial class BaseStaffManager : BaseManager
    {
        #region 根据工号获取实体
        /// <summary>
        /// 根据工号获取实体
        /// </summary>
        /// <param name="employeeNumber">工号</param>
        /// <returns></returns>
        public BaseStaffEntity GetEntityByEmployeeNumber(string employeeNumber)
        {
            BaseStaffEntity entity = null;
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseStaffEntity.FieldEmployeeNumber, employeeNumber),
                new KeyValuePair<string, object>(BaseStaffEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseStaffEntity.FieldDeleted, 0)
            };
            var dt = GetDataTable(parameters);
            if (dt != null && dt.Rows.Count > 0)
            {
                entity = BaseEntity.Create<BaseStaffEntity>(dt);
            }
            return entity;
        }
        #endregion

        #region 根据真实姓名获取实体
        /// <summary>
        /// 根据真实姓名获取实体
        /// </summary>
        /// <param name="realName"></param>
        /// <returns></returns>
        public BaseStaffEntity GetEntityByRealName(string realName)
        {
            BaseStaffEntity entity = null;
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseStaffEntity.FieldRealName, realName),
                new KeyValuePair<string, object>(BaseStaffEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseStaffEntity.FieldDeleted, 0)
            };
            var dt = GetDataTable(parameters);
            if (dt != null && dt.Rows.Count > 0)
            {
                entity = BaseEntity.Create<BaseStaffEntity>(dt);
            }
            return entity;
        }
        #endregion

        #region public BaseUserInfo ConvertToUserInfo(BaseUserInfo userInfo, BaseStaffEntity staffEntity)
        /// <summary>
        /// 员工实体转换为用户实体
        /// </summary>
        /// <param name="userInfo">用户实体</param>
        /// <param name="staffEntity">员工实体</param>
        /// <returns>用户实体</returns>
        public BaseUserInfo ConvertToUserInfo(BaseUserInfo userInfo, BaseStaffEntity staffEntity)
        {
            userInfo.EmployeeNumber = staffEntity.EmployeeNumber;
            if (string.IsNullOrEmpty(userInfo.UserName))
            {
                userInfo.UserName = staffEntity.UserName;
            }
            if (string.IsNullOrEmpty(userInfo.RealName))
            {
                userInfo.RealName = staffEntity.RealName;
            }
            // 需要修正
            userInfo.CompanyId = staffEntity.CompanyId.ToString();
            // userInfo.CompanyCode = staffEntity.CompanyCode;
            // userInfo.CompanyName = staffEntity.CompanyName;
            userInfo.SubCompanyId = staffEntity.SubCompanyId.ToString();
            userInfo.DepartmentId = staffEntity.DepartmentId.ToString();
            // userInfo.DepartmentCode = staffEntity.DepartmentCode;
            // userInfo.DepartmentName = staffEntity.DepartmentName;
            userInfo.WorkgroupId = staffEntity.WorkgroupId.ToString();
            // userInfo.WorkgroupCode = staffEntity.WorkgroupCode;
            // userInfo.WorkgroupName = staffEntity.WorkgroupName;
            return userInfo;
        }
        #endregion

        #region public string Add(string DepartmentId, string userName, string code, string realName, bool isVirtual, bool isDimission, bool enabled, string description)
        /// <summary>
        /// 添加员工
        /// </summary>
        /// <param name="departmentId">部门主键</param>
        /// <param name="userName">用户名</param>
        /// <param name="employeeNumber">工号</param>
        /// <param name="realName">名称</param>
        /// <param name="isVirtual">虚拟用户</param>
        /// <param name="isDimission">离职</param>
        /// <param name="enabled">有效</param>
        /// <param name="description">备注</param>
        /// <returns>主键</returns>
        public string Add(string departmentId, string userName, string employeeNumber, string realName, bool isVirtual, bool isDimission, bool enabled, string description)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            var managerSequence = new BaseSequenceManager(DbHelper);
            var sequence = managerSequence.Increment(CurrentTableName);
            sqlBuilder.BeginInsert(CurrentTableName);
            sqlBuilder.SetValue(BaseStaffEntity.FieldId, sequence);
            sqlBuilder.SetValue(BaseStaffEntity.FieldEmployeeNumber, employeeNumber);
            sqlBuilder.SetValue(BaseStaffEntity.FieldUserName, userName);
            sqlBuilder.SetValue(BaseStaffEntity.FieldRealName, realName);
            sqlBuilder.SetValue(BaseStaffEntity.FieldDepartmentId, departmentId);
            sqlBuilder.SetValue(BaseStaffEntity.FieldSortCode, sequence);
            sqlBuilder.SetValue(BaseStaffEntity.FieldEnabled, enabled ? 1 : 0);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseStaffEntity.FieldCreateUserId, UserInfo.UserId);
                sqlBuilder.SetValue(BaseStaffEntity.FieldCreateTime, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseStaffEntity.FieldCreateTime);
            var result = sqlBuilder.EndInsert() > 0 ? sequence : string.Empty;
            return result;
        }
        #endregion

        #region public string Add(BaseStaffEntity staffEntity, out Status status)
        /// <summary>
        /// 添加员工
        /// </summary>
        /// <param name="staffEntity">实体</param>
        /// <param name="status">状态</param>
        /// <returns>主键</returns>
        public string UniqueAdd(BaseStaffEntity staffEntity, out Status status)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(staffEntity.UserName) && Exists(new KeyValuePair<string, object>(BaseStaffEntity.FieldUserName, staffEntity.UserName), new KeyValuePair<string, object>(BaseStaffEntity.FieldDeleted, 0)))
            {
                // 名称已重复
                status = Status.ErrorUserExist;
            }
            else
            {
                // 检查编号是否重复
                if (!string.IsNullOrEmpty(staffEntity.EmployeeNumber) && Exists(new KeyValuePair<string, object>(BaseStaffEntity.FieldEmployeeNumber, staffEntity.EmployeeNumber), new KeyValuePair<string, object>(BaseStaffEntity.FieldDeleted, 0)))
                {
                    // 编号已重复
                    status = Status.ErrorCodeExist;
                }
                else
                {
                    result = AddEntity(staffEntity);
                    // 运行成功
                    status = Status.OkAdd;
                }
            }
            return result;
        }
        #endregion

        #region public int Update(BaseStaffEntity staffEntity, out Status status)
        /// <summary>
        /// 更新员工
        /// </summary>
        /// <param name="staffEntity">实体</param>
        /// <param name="status">状态</param>
        /// <returns>影响行数</returns>
        public int UniqueUpdate(BaseStaffEntity staffEntity, out Status status)
        {
            var result = 0;
            // 检查编号是否重复
            if (!string.IsNullOrEmpty(staffEntity.EmployeeNumber) && Exists(new KeyValuePair<string, object>(BaseStaffEntity.FieldEmployeeNumber, staffEntity.EmployeeNumber), new KeyValuePair<string, object>(BaseStaffEntity.FieldDeleted, 0), staffEntity.Id))
            {
                // 编号已重复
                status = Status.ErrorCodeExist;
            }
            else
            {
                // 用户名是空的，不判断是否重复了
                if (!string.IsNullOrEmpty(staffEntity.UserName) && Exists(new KeyValuePair<string, object>(BaseStaffEntity.FieldUserName, staffEntity.UserName), new KeyValuePair<string, object>(BaseStaffEntity.FieldDeleted, 0), staffEntity.Id))
                {
                    // 名称已重复
                    status = Status.ErrorUserExist;
                }
                else
                {
                    result = UpdateEntity(staffEntity);
                    // 按员工的修改信息，把用户信息进行修改
                    UpdateUser(staffEntity.Id.ToString());
                    if (result > 0)
                    {
                        status = Status.OkUpdate;
                    }
                    else
                    {
                        status = Status.ErrorDeleted;
                    }
                }
            }
            return result;
        }
        #endregion

        #region public int UpdateAddress(BaseStaffEntity staffEntity, out Status status) 更新
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="staffEntity">实体类</param>
        /// <param name="status">状态</param>
        /// <returns>影响行数</returns>
        public int UpdateAddress(BaseStaffEntity staffEntity, out Status status)
        {
            var result = 0;
            // 检查是否已被其他人修改            
            //if (DbUtil.IsModifed(DbHelper, BaseStaffEntity.CurrentTableName, staffEntity.Id, staffEntity.UpdateUserId, staffEntity.UpdateTime))
            //{
            //    // 数据已经被修改
            //    status = StatusCode.ErrorChanged.ToString();
            //}
            //else
            //{
            // 进行更新操作
            result = UpdateEntity(staffEntity);
            if (result == 1)
            {
                // 按员工的修改信息，把用户信息进行修改
                UpdateUser(staffEntity.Id.ToString());
                status = Status.OkUpdate;
            }
            else
            {
                // 数据可能被删除
                status = Status.ErrorDeleted;
            }
            //}
            return result;
        }
        #endregion

        #region public DataTable GetDataTableByCompany(string companyId)
        /// <summary>
        /// 按公司获取部门员工
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByCompany(string companyId)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + BaseStaffEntity.CurrentTableName + ".* "
                + " FROM " + BaseStaffEntity.CurrentTableName);
            if (!string.IsNullOrEmpty(companyId))
            {
                sb.Append(" WHERE " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldCompanyId + " = '" + companyId + "' ");
            }
            sb.Append(" ORDER BY " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldSortCode);
            return DbHelper.Fill(sb.Return());
        }
        #endregion

        #region public DataTable GetDataTableByOrganizations(string[] ids) 按工作组、部门、公司获取员工列表
        /// <summary>
        /// 按工作组、部门、公司获取员工列表
        /// </summary>
        /// <param name="organizationIds">主键数组</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByOrganizations(string[] organizationIds)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append(" SELECT " + BaseStaffEntity.CurrentTableName + ".* "
                                + " FROM " + BaseStaffEntity.CurrentTableName
                                + " WHERE " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDeleted + " = 0"
                                + " AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldWorkgroupId + " IN ( " + StringUtil.ArrayToList(organizationIds) + ") "
                                + " OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDepartmentId + " IN (" + StringUtil.ArrayToList(organizationIds) + ") "
                                + " OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldSubCompanyId + " IN (" + StringUtil.ArrayToList(organizationIds) + ") "
                                + " OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldCompanyId + " IN (" + StringUtil.ArrayToList(organizationIds) + ")) "
                                + " ORDER BY " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldSortCode);
            return DbHelper.Fill(sb.Return());
        }
        #endregion

        #region public DataTable GetDataTableByOrganization(string organizationId)
        /// <summary>
        /// 获取部门员工
        /// </summary>
        /// <param name="organizationId">组织机构主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByOrganization(string organizationId)
        {
            var organizationIds = new string[] { organizationId };
            return GetDataTableByOrganizations(organizationIds);
        }
        #endregion

        #region public DataTable GetDataTableByDepartment(string departmentId)
        /// <summary>
        /// 按部门获取部门员工
        /// </summary>
        /// <param name="departmentId">部门主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByDepartment(string departmentId)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + BaseStaffEntity.CurrentTableName + ".* "
                + " ,(SELECT " + BaseOrganizationEntity.FieldCode + " FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE Id = " + BaseStaffEntity.CurrentTableName + ".CompanyId) AS CompanyCode"
                + " ,(SELECT " + BaseOrganizationEntity.FieldName + " FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE Id = " + BaseStaffEntity.CurrentTableName + ".CompanyId) AS CompanyName "
                + " ,(SELECT " + BaseOrganizationEntity.FieldCode + " From " + BaseOrganizationEntity.CurrentTableName + " WHERE Id = " + BaseStaffEntity.CurrentTableName + ".DepartmentId) AS DepartmentCode"
                + " ,(SELECT " + BaseOrganizationEntity.FieldName + " FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE Id = " + BaseStaffEntity.CurrentTableName + ".DepartmentId) AS DepartmentName "
                //+ " ,(SELECT " + BaseItemDetailsEntity.FieldItemName + " FROM ItemsDuty WHERE Id = " + BaseStaffEntity.CurrentTableName + ".DutyId) AS DutyName "
                //+ " ,(SELECT " + BaseItemDetailsEntity.FieldItemName + " FROM ItemsTitle WHERE Id = " + BaseStaffEntity.CurrentTableName + ".TitleId) AS TitleName "
                + " ,(SELECT " + BaseRoleEntity.FieldName + " FROM " + BaseRoleEntity.CurrentTableName + " WHERE Id = RoleId) AS RoleName "
                + " FROM " + BaseStaffEntity.CurrentTableName + " LEFT OUTER JOIN " + BaseUserEntity.CurrentTableName
                + "       ON " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldId + " = " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldId);
            if (!string.IsNullOrEmpty(departmentId))
            {
                sb.Append(" WHERE " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDepartmentId + " = '" + departmentId + "' ");
            }
            sb.Append(" ORDER BY " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldSortCode);
            return DbHelper.Fill(sb.Return());
        }
        #endregion

        #region public string GetStaffCount(IDbHelper dbHelper, string organizationCode)
        /// <summary>
		/// 获取部门的员工个数
		/// </summary>
		/// <param name="organizationCode">部门编码</param>
		/// <returns>员工个数</returns>
		public string GetStaffCount(string organizationCode)
        {
            var staffCount = string.Empty;
            var names = new string[1];
            var values = new object[1];
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append(@"SELECT COUNT(*) AS STAFFCOUNT FROM " + BaseStaffEntity.CurrentTableName + " WHERE " + BaseStaffEntity.FieldEnabled + " = 1 AND " + BaseStaffEntity.FieldIsDimission + " <> 1 AND " + BaseStaffEntity.FieldDeleted + " = 0 AND (" + BaseStaffEntity.FieldDepartmentId + " IN (SELECT Id FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE (LEFT(CODE, LEN(?)) = ?))) ");
            names[0] = BaseStaffEntity.FieldCompanyId;
            values[0] = organizationCode;
            var obj = DbHelper.ExecuteScalar(sb.Return(), DbHelper.MakeParameters(names, values));
            if (obj != null)
            {
                staffCount = obj.ToString();
            }
            return staffCount;
        }
        #endregion

        #region public string GetCategoryCount(IDbHelper dbHelper, string categoryId, string organizationCode, string categoryField)
        /// <summary>
        /// 获得某部门某种属性的人数
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="categoryId"></param>
        /// <param name="organizationCode"></param>
        /// <param name="categoryField"></param>
        /// <returns></returns>
        public string GetCategoryCount(IDbHelper dbHelper, string categoryId, string organizationCode, string categoryField)
        {
            var staffCount = string.Empty;
            var names = new string[3];
            var values = new object[3];
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) AS STAFFCOUNT FROM " + BaseStaffEntity.CurrentTableName
                            + " WHERE (" + categoryField + " = ?) AND (ENABLED = 1) AND (ISDIMISSION <> 1) AND (ISSTAFF = 1) AND (DepartmentId IN (SELECT Id FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE (LEFT(CODE, LEN(?)) = ?))) ");
            names[0] = categoryField;
            names[1] = BaseOrganizationEntity.FieldCode;
            names[2] = organizationCode;
            values[0] = categoryId;
            values[1] = organizationCode;
            values[2] = organizationCode;
            var obj = dbHelper.ExecuteScalar(sb.Return(), DbHelper.MakeParameters(names, values));
            if (obj != null)
            {
                staffCount = obj.ToString();
            }
            return staffCount;
        }
        #endregion

        #region public DataTable SearchByOrganizationIds(string[] ids, string userName, string enabled, string role)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="organizationIds"></param>
        /// <param name="userName"></param>
        /// <param name="enabled"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public DataTable SearchByOrganizationIds(string[] organizationIds, string userName, string enabled, string role)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + BaseStaffEntity.CurrentTableName + ".* "
                                + " FROM " + BaseStaffEntity.CurrentTableName
                                + " WHERE 1 = 1");

            // 这里要注意系统安全隐患
            if (organizationIds != null)
            {
                // 可以管理的部门
                sb.Append(" AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldWorkgroupId + " IN (" + StringUtil.ArrayToList(organizationIds) + ") ");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDepartmentId + " IN (" + StringUtil.ArrayToList(organizationIds) + ") ");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldSubCompanyId + " IN (" + StringUtil.ArrayToList(organizationIds) + ") ");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldCompanyId + " IN (" + StringUtil.ArrayToList(organizationIds) + ")) ");
            }

            if (!string.IsNullOrEmpty(userName))
            {
                if (userName.IndexOf('%') < 0)
                {
                    userName = "%" + userName + "%";
                }
                userName = DbHelper.SqlSafe(userName);
            }
            if (!string.IsNullOrEmpty(userName))
            {
                userName = userName.ToLower();
                sb.Append(" AND " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldUserName + " LIKE ('" + userName + "') ");
            }

            if (!string.IsNullOrEmpty(enabled))
            {
                sb.Append(" AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldEnabled + " = '" + enabled + "'");
            }

            if (!string.IsNullOrEmpty(role))
            {
                // sql += " AND " + BaseUserEntity.FieldRoleId + " = '" + role + "'");
            }
            sb.Append(" ORDER BY " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldSortCode);
            sb.Replace(" 1 = 1 AND ", " ");
            return DbHelper.Fill(sb.Return());
        }
        #endregion

        #region public DataTable Search(string userName, string enabled, string role) 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="role">类型</param>
        /// <param name="enabled">有效</param>
        /// <returns>数据权限</returns>
        public DataTable Search(string userName, string enabled, string role)
        {
            return SearchByOrganizationIds(null, userName, enabled, role);
        }
        #endregion

        #region public DataTable GetAddressDataTable(string organizationId = null, string searchKey = null) 获取打印列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="organizationId">组织机构主键</param>
        /// <param name="searchKey">关键字</param>
        /// <returns>数据表</returns>
        public DataTable GetAddressDataTable(string organizationId = null, string searchKey = null)
        {
            // 因为Access中不支持分页，故此操作
            if (BaseSystemInfo.UserCenterDbType == CurrentDbType.Access)
            {
                var dt = new DataTable();
                var cmd = "SELECT * from BaseStaff ";

                /*"SELECT A.* ,B.Code AS CompanyCode ,B.Name AS CompanyName , " +
                        " C.Code AS DepartmentCode ,C.Name AS DepartmentName ,D.ItemName AS DutyName ," +
                        " F.RealName as RoleName  " +
                        " FROM (((((BaseStaff A LEFT OUTER JOIN BaseOrganization B ON B.Id = A.CompanyId)" +
                        " LEFT OUTER JOIN BaseOrganization C ON C.Id = A.DepartmentId)" +
                        " LEFT OUTER JOIN ItemsDuty D ON D.Id = CInt(iif(IsNull( A.DutyId ), 0, A.DutyId)))" +
                        " LEFT JOIN BaseUser E ON A.Id = E.Id )" +
                        " left join BaseRole F on F.Id = E.RoleId)"; */
                // dbHelper.Fill(DT, Cmd,"List"); DT.Tables["List"];

                return dbHelper.Fill(dt, cmd);
            }

            searchKey = StringUtil.GetSearchString(searchKey);
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + BaseStaffEntity.CurrentTableName + ".* "
                            + "," + BaseOrganizationEntity.CurrentTableName + "A." + BaseOrganizationEntity.FieldCode + " AS CompanyCode "
                            + "," + BaseOrganizationEntity.CurrentTableName + "A." + BaseOrganizationEntity.FieldName + " AS CompanyName "
                            + "," + BaseOrganizationEntity.CurrentTableName + "B." + BaseOrganizationEntity.FieldCode + " AS DepartmentCode "
                            + "," + BaseOrganizationEntity.CurrentTableName + "B." + BaseOrganizationEntity.FieldName + " AS DepartmentName "
                            //+ ",ItemsDuty." + BaseItemDetailsEntity.FieldItemName + " AS DutyName "
                            + "," + "OT.RoleName "
                            + " FROM " + BaseStaffEntity.CurrentTableName
                            + "      LEFT OUTER JOIN " + BaseOrganizationEntity.CurrentTableName + " " + BaseOrganizationEntity.CurrentTableName + "A ON " + BaseOrganizationEntity.CurrentTableName + "A." + BaseOrganizationEntity.FieldId + " = " + BaseStaffEntity.FieldCompanyId
                            + "      LEFT OUTER JOIN " + BaseOrganizationEntity.CurrentTableName + " " + BaseOrganizationEntity.CurrentTableName + "B ON " + BaseOrganizationEntity.CurrentTableName + "B." + BaseOrganizationEntity.FieldId + " = " + BaseStaffEntity.FieldDepartmentId
                            //+ "      LEFT OUTER JOIN ItemsDuty ON ItemsDuty." + BaseItemDetailsEntity.FieldId + " = " + BaseStaffEntity.FieldDutyId
                            + "      ON " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldId + " = OT.Id ");
            if (string.IsNullOrEmpty(organizationId))
            {
                sb.Append(" WHERE ((" + BaseOrganizationEntity.CurrentTableName + "A." + BaseOrganizationEntity.FieldIsInnerOrganization + " = 1) OR (" + BaseOrganizationEntity.CurrentTableName + "B." + BaseOrganizationEntity.FieldIsInnerOrganization + " =1)) ");
            }
            else
            {
                sb.Append(" WHERE (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldCompanyId + " = '" + organizationId + "'"
                + " OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDepartmentId + " = '" + organizationId + "') ");
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                sb.Append(" AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldUserName + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldRealName + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseOrganizationEntity.CurrentTableName + "A." + BaseOrganizationEntity.FieldName + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseOrganizationEntity.CurrentTableName + "B." + BaseOrganizationEntity.FieldName + " LIKE '%" + searchKey + "%'");
                //sb.Append(" OR " + "ItemsDuty." + BaseItemDetailsEntity.FieldItemName + " LIKE '%" + searchKey + "%'";
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldOfficePhone + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldExtension + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldMobile + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldShortNumber + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldQq + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldEmail + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDescription + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR OT.RoleName LIKE '%" + searchKey + "%')");
            }
            sb.Append(" ORDER BY " + BaseOrganizationEntity.CurrentTableName + "B." + BaseOrganizationEntity.FieldSortCode
                          + " ," + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldSortCode);
            return DbHelper.Fill(sb.Return());
        }
        #endregion

        #region public DataTable GetAddressDataTableByPage(string organizationId, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 100, string sort = null) 获取打印列表
        /// <summary>
        /// 获取列表 HJC
        /// </summary>
        /// <param name="recordCount">记录总数</param>
        /// <param name="organizationId">组织机构主键</param>
        /// <param name="searchKey">查询字符</param>
        /// <param name="pageSize">分页的条数</param>
        /// <param name="sort"></param>
        /// <param name="pageNo">当前页数</param>
        /// <returns>数据表</returns>
        public DataTable GetAddressDataTableByPage(string organizationId, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 100, string sort = null)
        {
            // 因为Access中不支持分页，故此操作 假设行 1 //xtzwd
            if (BaseSystemInfo.UserCenterDbType == CurrentDbType.Access)
            {
                recordCount = 1;
                var dt = new DataTable();
                var cmd = "SELECT * from BaseStaff ";

                /*"SELECT A.* ,B.Code AS CompanyCode ,B.Name AS CompanyName , " +
                        " C.Code AS DepartmentCode ,C.Name AS DepartmentName ,D.ItemName AS DutyName ," +
                        " F.RealName as RoleName  " +
                        " FROM (((((BaseStaff A LEFT OUTER JOIN BaseOrganization B ON B.Id = A.CompanyId)" +
                        " LEFT OUTER JOIN BaseOrganization C ON C.Id = A.DepartmentId)" +
                        " LEFT OUTER JOIN ItemsDuty D ON D.Id = CInt(iif(IsNull( A.DutyId ), 0, A.DutyId)))" +
                        " LEFT JOIN BaseUser E ON A.Id = E.Id )" + 
                         //此处多级查询溢出，有待优化
                        " left join BaseRole F on F.Id = E.RoleId)"; */
                // dbHelper.Fill(DT, Cmd,"List"); DT.Tables["List"];

                return dbHelper.Fill(dt, cmd);
            }

            searchKey = StringUtil.GetSearchString(searchKey);
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + BaseStaffEntity.CurrentTableName + ".* "
                            + " ," + BaseOrganizationEntity.CurrentTableName + "A." + BaseOrganizationEntity.FieldCode + " AS CompanyCode "
                            + " ," + BaseOrganizationEntity.CurrentTableName + "B." + BaseOrganizationEntity.FieldCode + " AS DepartmentCode "
                            //+ " ,ItemsDuty." + BaseItemDetailsEntity.FieldItemName + " AS DutyName "
                            + " FROM " + BaseStaffEntity.CurrentTableName
                            + "      LEFT OUTER JOIN " + BaseOrganizationEntity.CurrentTableName + " " + BaseOrganizationEntity.CurrentTableName + "A ON " + BaseOrganizationEntity.CurrentTableName + "A." + BaseOrganizationEntity.FieldId + " = " + BaseStaffEntity.FieldCompanyId
                            + "      LEFT OUTER JOIN " + BaseOrganizationEntity.CurrentTableName + " " + BaseOrganizationEntity.CurrentTableName + "B ON " + BaseOrganizationEntity.CurrentTableName + "B." + BaseOrganizationEntity.FieldId + " = " + BaseStaffEntity.FieldDepartmentId);
            //+ "      LEFT OUTER JOIN ItemsDuty ON ItemsDuty." + BaseItemDetailsEntity.FieldId + " = " + BaseStaffEntity.FieldDutyId;
            if (string.IsNullOrEmpty(organizationId))
            {
                sb.Append(" WHERE ((" + BaseOrganizationEntity.CurrentTableName + "A." + BaseOrganizationEntity.FieldIsInnerOrganization + " = 1) OR (" + BaseOrganizationEntity.CurrentTableName + "B." + BaseOrganizationEntity.FieldIsInnerOrganization + " =1)) ");
            }
            else
            {
                sb.Append(" WHERE (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldCompanyId + " = '" + organizationId + "'"
                + " OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDepartmentId + " = '" + organizationId + "') ");
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                sb.Append(" AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldUserName + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldRealName + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "A." + BaseStaffEntity.FieldCompanyName + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "B." + BaseStaffEntity.FieldDepartmentName + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldOfficePhone + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldExtension + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldMobile + " LIKE '%" + searchKey + "'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldShortNumber + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldQq + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldEmail + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDescription + " LIKE '%" + searchKey + "%')");
            }
            var orderBy = string.Empty;
            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.SqlServer:
                    orderBy = BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldSortCode;
                    break;
                case CurrentDbType.Db2:
                    orderBy = BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldSortCode;
                    break;
                default:
                    orderBy = BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldSortCode;
                    break;
            }
            return GetDataTableByPage(out recordCount, pageNo, pageSize, orderBy, "ASC", sb.Return());
        }
        #endregion

        #region public DataTable Search(string organizationId, string searchKey, bool deletionStateCode) 查询
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="organizationId">组织机构主键</param>
        /// <param name="searchKey">查询字符串</param>
        /// <param name="deletionStateCode">是否删除</param>
        /// <returns>数据表</returns>
        public DataTable Search(string organizationId, string searchKey, bool deletionStateCode)
        {
            searchKey = StringUtil.GetSearchString(searchKey);
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + BaseStaffEntity.CurrentTableName + ".* "
                            + "," + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldName + " AS DepartmentName "
                            //+ ",ItemsDuty." + BaseItemDetailsEntity.FieldItemName + " AS DutyName "
                            + " FROM " + BaseStaffEntity.CurrentTableName
                            + " LEFT OUTER JOIN " + BaseOrganizationEntity.CurrentTableName + " ON " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldId + " = " + BaseStaffEntity.FieldDepartmentId
                            //+ " LEFT OUTER JOIN ItemsDuty ON  ItemsDuty." + BaseItemDetailsEntity.FieldId + " = " + BaseStaffEntity.FieldDutyId
                            + " WHERE (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDeleted + " = " + (deletionStateCode ? 1 : 0) + ")");
            if (!string.IsNullOrEmpty(organizationId))
            {
                sb.Append(" AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDepartmentId + " = '" + organizationId + "' OR " + BaseStaffEntity.FieldCompanyId + " = '" + organizationId + "')");
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                sb.Append(" AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldUserName + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldRealName + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldShortNumber + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldTelephone + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldMobile + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldEmail + " LIKE '%" + searchKey + "%'");
                sb.Append(" OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldQq + " LIKE '%" + searchKey + "%')");
            }
            sb.Append(" ORDER BY " // + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldSortCode
                                   // + " ," 
                          + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldSortCode);
            return DbHelper.Fill(sb.Return());
        }
        #endregion

        #region public DataTable GetChildrenStaffs(string organizationId) 获取下属员工
        /// <summary>
        /// 获取下属员工
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataTable GetChildrenStaffs(string organizationId)
        {
            var organizationManager = new BaseOrganizationManager(DbHelper, UserInfo);
            string[] organizationIds = null;
            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.Access:
                case CurrentDbType.SqlServer:
                    var organizationCode = GetCodeById(organizationId);
                    organizationIds = organizationManager.GetChildrensIdByCode(BaseOrganizationEntity.FieldCode, organizationCode);
                    break;
                case CurrentDbType.Oracle:
                    organizationIds = organizationManager.GetChildrensId(BaseOrganizationEntity.FieldId, organizationId, BaseOrganizationEntity.FieldParentId);
                    break;
            }
            return GetDataTableByOrganizations(organizationIds);
        }

        #endregion

        #region public DataTable GetParentChildrenStaffs(string organizationId) 获取上级下的所有下属
        /// <summary>
        /// 获取上级下的所有下属
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataTable GetParentChildrenStaffs(string organizationId)
        {
            var organizationManager = new BaseOrganizationManager(DbHelper, UserInfo);
            var organizationCode = organizationManager.GetCodeById(organizationId);
            var organizationIds = organizationManager.GetChildrensIdByCode(BaseOrganizationEntity.FieldCode, organizationCode);
            return GetDataTableByOrganizations(organizationIds);
        }
        #endregion

        #region public DataTable GetDataTable() 获取员工列表
        /// <summary>
        /// 获取员工列表
        /// </summary>
        /// <returns>数据表</returns>
        public DataTable GetDataTable()
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + BaseStaffEntity.CurrentTableName + ".* "
                + " , " + BaseUserEntity.CurrentTableName + ".UserOnline"
                + " ,(SELECT " + BaseOrganizationEntity.FieldCode + " FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE Id = " + BaseStaffEntity.CurrentTableName + ".CompanyId) AS CompanyCode"
                + " ,(SELECT " + BaseOrganizationEntity.FieldName + " FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE Id = " + BaseStaffEntity.CurrentTableName + ".CompanyId) AS CompanyName "

                + " ,(SELECT " + BaseOrganizationEntity.FieldCode + " FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE Id = " + BaseStaffEntity.CurrentTableName + ".DepartmentId) AS DepartmentCode"
                + " ,(SELECT " + BaseOrganizationEntity.FieldName + " FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE Id = " + BaseStaffEntity.CurrentTableName + ".DepartmentId) AS DepartmentName "

                + " ,(SELECT " + BaseOrganizationEntity.FieldCode + " FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE Id = " + BaseStaffEntity.CurrentTableName + ".WorkgroupId) AS WorkgroupCode"
                + " ,(SELECT " + BaseOrganizationEntity.FieldName + " FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE Id = " + BaseStaffEntity.CurrentTableName + ".WorkgroupId) AS WorkgroupName "

                //+ " ,(SELECT " + BaseItemDetailsEntity.FieldItemName + " FROM ItemsDuty WHERE Id = " + BaseStaffEntity.CurrentTableName + ".DutyId) AS DutyName "

                //+ " ,(SELECT " + BaseItemDetailsEntity.FieldItemName + " FROM ItemsTitle WHERE Id = " + BaseStaffEntity.CurrentTableName + ".TitleId) AS TitleName "

                + " ,(SELECT " + BaseRoleEntity.FieldName + " FROM " + BaseRoleEntity.CurrentTableName + " WHERE Id = RoleId) AS RoleName "
                // + " ,(SELECT COUNT(*) FROM " + BaseUserRoleEntity.CurrentTableName + " WHERE " + BaseUserRoleEntity.CurrentTableName + ".StaffID = " + BaseStaffEntity.CurrentTableName + ".Id) AS RoleCount "
                + " FROM (" + BaseStaffEntity.CurrentTableName + " LEFT OUTER JOIN " + BaseUserEntity.CurrentTableName
                + " ON " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldId + " = " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldId + ") "
                + "  LEFT OUTER JOIN " + BaseOrganizationEntity.CurrentTableName + " "
                + " ON " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDepartmentId + " = " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldId
                + " ORDER BY " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldSortCode
                + " , " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldSortCode);
            return DbHelper.Fill(sb.Return());
        }
        #endregion

        #region public DataTable GetDataTable(string fieldName, string fieldValue) 获取打印列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="fieldName">字段</param>
        /// <param name="fieldValue">内容</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(string fieldName, string fieldValue)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT A.* "
                    + " ,(SELECT Code FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE " + BaseOrganizationEntity.CurrentTableName + ".ID = A.CompanyId) AS CompanyCode"
                    + " ,(SELECT Name FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE " + BaseOrganizationEntity.CurrentTableName + ".ID = A.CompanyId) AS CompanyName "

                    + " ,(SELECT Code FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE " + BaseOrganizationEntity.CurrentTableName + ".ID = A.DepartmentId) AS DepartmentCode"
                    + " ,(SELECT Name FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE " + BaseOrganizationEntity.CurrentTableName + ".ID = A.DepartmentId) AS DepartmentName "

                    + " ,(SELECT " + BaseOrganizationEntity.FieldCode + " From " + BaseOrganizationEntity.CurrentTableName + " WHERE Id = A.WorkgroupId) AS WorkgroupCode"
                    + " ,(SELECT " + BaseOrganizationEntity.FieldName + " FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE Id = A.WorkgroupId) AS WorkgroupName "

                    + " ,(SELECT ItemName FROM ItemsDuty WHERE ItemsDuty.Id = A.DutyId) AS DutyName "

                    + " ,(SELECT ItemName FROM ItemsTitle WHERE ItemsTitle.Id = A.TitleId) AS TitleName "

                    + " FROM " + BaseStaffEntity.CurrentTableName + " A "
                    + " WHERE " + fieldName + " = " + DbHelper.GetParameter(fieldName)
                    + " ORDER BY A.SortCode");
            return DbHelper.Fill(sb.Return(), new IDbDataParameter[] { DbHelper.MakeParameter(fieldName, fieldValue) });
        }
        #endregion

        #region public int BatchSave(DataTable result) 批量进行保存
        /// <summary>
        /// 批量进行保存
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns>影响行数</returns>
        public override int BatchSave(DataTable dt)
        {
            var result = 0;
            foreach (DataRow dr in dt.Rows)
            {
                // 删除状态
                if (dr.RowState == DataRowState.Deleted)
                {
                    var id = dr[BaseStaffEntity.FieldId, DataRowVersion.Original].ToString();
                    if (id.Length > 0)
                    {
                        result += DeleteEntity(id);
                    }
                }
                // 被修改过
                if (dr.RowState == DataRowState.Modified)
                {
                    var id = dr[BaseStaffEntity.FieldId, DataRowVersion.Original].ToString();
                    if (id.Length > 0)
                    {
                        var staffEntity = BaseEntity.Create<BaseStaffEntity>(dr);
                        result += UpdateEntity(staffEntity);
                    }
                }
                // 添加状态
                if (dr.RowState == DataRowState.Added)
                {
                    var staffEntity = BaseEntity.Create<BaseStaffEntity>(dr);
                    result += AddEntity(staffEntity).Length > 0 ? 1 : 0;
                }
                if (dr.RowState == DataRowState.Unchanged)
                {
                    continue;
                }
                if (dr.RowState == DataRowState.Detached)
                {
                    continue;
                }
            }
            Status = Status.Ok;
            StatusCode = Status.Ok.ToString();
            return result;
        }
        #endregion

        #region public int UpdateUser(string staffId) 按员工的修改信息，把用户信息进行修改
        /// <summary>
        /// 按员工的修改信息，把用户信息进行修改
        /// </summary>
        /// <param name="staffId">员工主键</param>
        /// <returns>影响行数</returns>
        public int UpdateUser(string staffId)
        {
            //var result = this.GetDataTable(BaseStaffEntity.FieldId, staffId);
            //BaseStaffEntity staffEntity = new BaseStaffEntity(result);
            //if (staffEntity.UserId > 0)
            //{
            //    // 员工信息改变时，用户信息也跟着改变。
            //    BaseUserManager userManager = new BaseUserManager(UserInfo);
            //    BaseUserEntity userEntity = userManager.GetEntity(staffEntity.UserId);
            //    // userEntity.Company = staffEntity.CompanyName;
            //    // userEntity.Department = staffEntity.DepartmentName;
            //    // userEntity.Workgroup = staffEntity.WorkgroupName;

            //    userEntity.UserName = staffEntity.UserName;
            //    userEntity.RealName = staffEntity.RealName;
            //    userEntity.Code = staffEntity.Code;

            //    userEntity.Email = staffEntity.Email;
            //    userEntity.Enabled = staffEntity.Enabled;
            //    // userEntity.Duty = staffEntity.DutyName;
            //    // userEntity.Title = staffEntity.TitleName;
            //    userEntity.Gender = staffEntity.Gender;
            //    userEntity.Birthday = staffEntity.Birthday;
            //    userEntity.Mobile = staffEntity.Mobile;
            //}
            return 0;
        }
        #endregion

        #region public int DeleteUser(string staffId) 删除员工关联的用户
        /// <summary>
        /// 删除员工关联的用户
        /// </summary>
        /// <param name="staffId">员工主键</param>
        /// <returns>影响行数</returns>
        public int DeleteUser(string staffId)
        {
            var result = 0;
            var userId = GetEntity(staffId).UserId.ToString();
            if (!string.IsNullOrEmpty(userId))
            {
                // 删除用户
                var userManager = new BaseUserManager(UserInfo);
                result += userManager.SetDeleted(userId);
            }
            // 将员工的用户设置为空
            result += Update(new KeyValuePair<string, object>(BaseStaffEntity.FieldId, staffId), new KeyValuePair<string, object>(BaseStaffEntity.FieldUserId, 0));
            return result;
        }
        #endregion

        #region public override int SetDeleted(string id) 设置删除
        /// <summary>
        /// 设置删除
        /// 删除员工时，需要把用户也给删除掉
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int SetDeleted(string id)
        {
            // 先把用户设置为是否删除
            var userId = GetProperty(id, BaseStaffEntity.FieldUserId);
            if (!string.IsNullOrEmpty(userId))
            {
                var userManager = new BaseUserManager(UserInfo);
                userManager.SetDeleted(userId);
            }
            // 再把员工设置为是否删除
            return Update(new KeyValuePair<string, object>(BaseStaffEntity.FieldId, id), new KeyValuePair<string, object>(BaseStaffEntity.FieldDeleted, 1));
        }
        #endregion

        #region public override int ResetSortCode() 重置排序码
        /// <summary>
        /// 重置排序码
        /// </summary>
        public override int ResetSortCode()
        {
            var result = 0;
            var dt = GetDataTable();
            var id = string.Empty;
            var sortCode = string.Empty;
            foreach (DataRow dr in dt.Rows)
            {
                id = dr[BaseStaffEntity.FieldId].ToString();
                var managerSequence = new BaseSequenceManager(DbHelper);
                sortCode = managerSequence.Increment(CurrentTableName);
                result += Update(id, new KeyValuePair<string, object>(BaseStaffEntity.FieldSortCode, sortCode));
            }
            return result;
        }
        #endregion

        #region public override int Delete(string id) 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public override int Delete(string id)
        {
            var result = 0;
            var staffEntity = GetEntity(id);
            if (staffEntity != null)
            {
                var parameters = new List<KeyValuePair<string, object>>();
                // 删除相关的用户数据
                var userManager = new BaseUserManager(UserInfo);
                userManager.DeleteEntity(staffEntity.UserId);
                // 删除员工本表
                parameters.Add(new KeyValuePair<string, object>(BaseStaffEntity.FieldId, id));
                result = DbHelper.Delete(BaseStaffEntity.CurrentTableName, parameters);
            }
            return result;
        }
        #endregion

        #region public int BatchDelete(string[] ids)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int BatchDelete(string[] ids)
        {
            var result = 0;
            for (var i = 0; i < ids.Length; i++)
            {
                result += Delete(ids[i]);
            }
            return result;
        }
        #endregion

        #region public string GetIdByUserId(string userId) 通过用户Id获取员工主键
        /// <summary>
        /// 通过用户Id获取员工主键
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>员工主键</returns>
        public string GetIdByUserId(string userId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseStaffEntity.FieldUserId, userId),
                new KeyValuePair<string, object>(BaseStaffEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseStaffEntity.FieldEnabled, 1)
            };
            return DbHelper.GetProperty(CurrentTableName, parameters, BaseUtil.FieldId);
        }
        #endregion

        #region public DataTable GetDataTableByPage(string searchKey, string companyId, string departmentId, string roleId, out int recordCount, int pageNo = 1, int pageSize = 20, string order = null) 分页查询
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="searchKey">查询关键字</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="departmentId">部门主键</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string searchKey, string companyId, string departmentId, string roleId, out int recordCount, int pageNo = 1, int pageSize = 20, string order = null)
        {
            var condition = BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDeleted + " = 0 AND " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldEnabled + " = 1";

            if (!string.IsNullOrEmpty(companyId))
            {
                condition += " AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldCompanyId + " = " + companyId + ")";
            }
            if (!string.IsNullOrEmpty(departmentId))
            {
                condition += " AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDepartmentId + " = " + departmentId + ")";
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = "'" + StringUtil.GetSearchString(searchKey) + "'";
                condition += " AND (" + BaseStaffEntity.FieldRealName + " LIKE '%" + searchKey + "'";
                condition += " OR " + BaseStaffEntity.FieldUserName + " LIKE '%" + searchKey + "'";
                condition += " OR " + BaseStaffEntity.FieldQuickQuery + " LIKE '%" + searchKey + "')";
            }
            recordCount = DbHelper.GetCount(CurrentTableName, condition);

            return DbHelper.GetDataTableByPage(CurrentTableName, SelectFields, pageNo, pageSize, condition, order);
        }
        #endregion
    }
}
