﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    ///	BaseStaffManager
    /// 员工的基类
    /// 
    /// 改进了很多次，基本上很好用了。
    /// 
    /// 修改记录
    /// 
    ///     2011.08.01 版本：1.8 张广梁     修改public DataTable GetAddressPageDT(out int recordCount, string organizationId, string searchKey, int pageSize, int pageIndex) 中的错误
    ///     2009.09.29 版本: 1.7 JiRiGaLa   已删除的进行过滤。
    ///     2008.05.07 版本: 1.6 JiRiGaLa   主键进行整理。
    ///     2007.07.19 版本: 1.5 JiRiGaLa   GetListByDepartment 函数改进。
    ///     2007.07.19 版本: 1.5 JiRiGaLa   增加 GetImpersonationList 方法。
    ///     2007.07.12 版本: 1.4 JiRiGaLa   增加 SetProperty,GetList 方法。
    ///		2007.07.02 版本：1.3 JiRiGaLa   添加 GetList。
    ///     2007.01.06 版本：1.2 JiRiGaLa   添加排序方法(Swap)。    
    ///		2006.12.07 版本：1.1 JiRiGaLa   增加排序码重置方法(ResetSortCode)。
    ///		2006.02.05 版本：1.1 JiRiGaLa   重新调整主键的规范化。
    ///		2004.08.29 版本：1.0 Jirigala   改进了错误提示，变得更友好一些，注意大小写错误取消。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2008.05.07</date>
    /// </author> 
    /// </summary>
    public partial class BaseStaffManager : BaseManager
    {
        /// <summary>
        /// 根据编码获取实体
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public BaseStaffEntity GetEntityByCode(string code)
        {
            BaseStaffEntity entity = null;
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseStaffEntity.FieldCode, code),
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

        #region public BaseUserInfo ConvertToUserInfo(BaseUserInfo userInfo, BaseStaffEntity staffEntity)
        /// <summary>
        /// 员工实体转换为用户实体
        /// </summary>
        /// <param name="userInfo">用户实体</param>
        /// <param name="staffEntity">员工实体</param>
        /// <returns>用户实体</returns>
        public BaseUserInfo ConvertToUserInfo(BaseUserInfo userInfo, BaseStaffEntity staffEntity)
        {
            // result.Id = staffEntity.Id;
            // userInfo.StaffId = staffEntity.Id.ToString();
            userInfo.Code = staffEntity.Code;
            if (string.IsNullOrEmpty(userInfo.UserName))
            {
                userInfo.UserName = staffEntity.UserName;
            }
            if (string.IsNullOrEmpty(userInfo.RealName))
            {
                userInfo.RealName = staffEntity.RealName;
            }
            // 需要修正
            userInfo.CompanyId = staffEntity.CompanyId;
            // result.CompanyCode = staffEntity.CompanyCode;
            // result.CompanyName = staffEntity.CompanyName;
            // result.SubCompanyId = staffEntity.SubCompanyId;
            userInfo.DepartmentId = staffEntity.DepartmentId;
            // result.DepartmentCode = staffEntity.DepartmentCode;
            // result.DepartmentName = staffEntity.DepartmentName;
            // result.WorkgroupId = staffEntity.WorkgroupId;
            // result.WorkgroupCode = staffEntity.WorkgroupCode;
            // result.WorkgroupName = staffEntity.WorkgroupName;
            return userInfo;
        }
        #endregion

        #region public string Add(string DepartmentId, string userName, string code, string fullName, bool isVirtual, bool isDimission, bool enabled, string description)
        /// <summary>
        /// 添加员工
        /// </summary>
        /// <param name="departmentId">部门主键</param>
        /// <param name="userName">用户名</param>
        /// <param name="code">编号</param>
        /// <param name="fullName">名称</param>
        /// <param name="isVirtual">虚拟用户</param>
        /// <param name="isDimission">离职</param>
        /// <param name="enabled">有效</param>
        /// <param name="description">备注</param>
        /// <returns>主键</returns>
        public string Add(string departmentId, string userName, string code, string fullName, bool isVirtual, bool isDimission, bool enabled, string description)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            var managerSequence = new BaseSequenceManager(DbHelper);
            var sequence = managerSequence.Increment(CurrentTableName);
            sqlBuilder.BeginInsert(CurrentTableName);
            sqlBuilder.SetValue(BaseStaffEntity.FieldId, sequence);
            sqlBuilder.SetValue(BaseStaffEntity.FieldCode, code);
            sqlBuilder.SetValue(BaseStaffEntity.FieldUserName, userName);
            sqlBuilder.SetValue(BaseStaffEntity.FieldRealName, fullName);
            sqlBuilder.SetValue(BaseStaffEntity.FieldDepartmentId, departmentId);
            sqlBuilder.SetValue(BaseStaffEntity.FieldSortCode, sequence);
            sqlBuilder.SetValue(BaseStaffEntity.FieldEnabled, enabled ? 1 : 0);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseStaffEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseStaffEntity.FieldCreateTime, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseStaffEntity.FieldCreateTime);
            var result = sqlBuilder.EndInsert() > 0 ? sequence : string.Empty;
            return result;
        }
        #endregion

        #region public string Add(BaseStaffEntity staffEntity, out string statusCode)
        /// <summary>
        /// 添加员工
        /// </summary>
        /// <param name="staffEntity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <returns>主键</returns>
        public string Add(BaseStaffEntity staffEntity, out string statusCode)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(staffEntity.UserName) && Exists(new KeyValuePair<string, object>(BaseStaffEntity.FieldUserName, staffEntity.UserName), new KeyValuePair<string, object>(BaseStaffEntity.FieldDeleted, 0)))
            {
                // 名称已重复
                statusCode = Status.ErrorUserExist.ToString();
            }
            else
            {
                // 检查编号是否重复
                if (!string.IsNullOrEmpty(staffEntity.Code) && Exists(new KeyValuePair<string, object>(BaseStaffEntity.FieldCode, staffEntity.Code), new KeyValuePair<string, object>(BaseStaffEntity.FieldDeleted, 0)))
                {
                    // 编号已重复
                    statusCode = Status.ErrorCodeExist.ToString();
                }
                else
                {
                    result = AddEntity(staffEntity);
                    // 运行成功
                    statusCode = Status.OkAdd.ToString();
                }
            }
            return result;
        }
        #endregion

        #region public int Update(BaseStaffEntity staffEntity, out string statusCode)
        /// <summary>
        /// 更新员工
        /// </summary>
        /// <param name="staffEntity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <returns>影响行数</returns>
        public int Update(BaseStaffEntity staffEntity, out string statusCode)
        {
            var result = 0;
            // 检查编号是否重复
            if (!string.IsNullOrEmpty(staffEntity.Code) && Exists(new KeyValuePair<string, object>(BaseStaffEntity.FieldCode, staffEntity.Code), new KeyValuePair<string, object>(BaseStaffEntity.FieldDeleted, 0), staffEntity.Id))
            {
                // 编号已重复
                statusCode = Status.ErrorCodeExist.ToString();
            }
            else
            {
                // 用户名是空的，不判断是否重复了
                if (!string.IsNullOrEmpty(staffEntity.UserName) && Exists(new KeyValuePair<string, object>(BaseStaffEntity.FieldUserName, staffEntity.UserName), new KeyValuePair<string, object>(BaseStaffEntity.FieldDeleted, 0), staffEntity.Id))
                {
                    // 名称已重复
                    statusCode = Status.ErrorUserExist.ToString();
                }
                else
                {
                    result = UpdateEntity(staffEntity);
                    // 按员工的修改信息，把用户信息进行修改
                    UpdateUser(staffEntity.Id.ToString());
                    if (result > 0)
                    {
                        statusCode = Status.OkUpdate.ToString();
                    }
                    else
                    {
                        statusCode = Status.ErrorDeleted.ToString();
                    }
                }
            }
            return result;
        }
        #endregion

        #region public int UpdateAddress(BaseStaffEntity staffEntity, out string statusCode) 更新
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="staffEntity">实体类</param>
        /// <param name="statusCode"></param>
        /// <returns>影响行数</returns>
        public int UpdateAddress(BaseStaffEntity staffEntity, out string statusCode)
        {
            var result = 0;
            // 检查是否已被其他人修改            
            //if (DbUtil.IsModifed(DbHelper, BaseStaffEntity.TableName, staffEntity.Id, staffEntity.ModifiedUserId, staffEntity.ModifiedOn))
            //{
            //    // 数据已经被修改
            //    statusCode = StatusCode.ErrorChanged.ToString();
            //}
            //else
            //{
            // 进行更新操作
            result = UpdateEntity(staffEntity);
            if (result == 1)
            {
                // 按员工的修改信息，把用户信息进行修改
                UpdateUser(staffEntity.Id.ToString());
                statusCode = Status.OkUpdate.ToString();
            }
            else
            {
                // 数据可能被删除
                statusCode = Status.ErrorDeleted.ToString();
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
            var sql = "SELECT " + BaseStaffEntity.TableName + ".* "
                + " FROM " + BaseStaffEntity.TableName;
            if (!string.IsNullOrEmpty(companyId))
            {
                sql += " WHERE " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldCompanyId + " = '" + companyId + "' ";
            }
            sql += " ORDER BY " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldSortCode;
            return DbHelper.Fill(sql);
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
            var organizeList = string.Join(",", organizationIds);
            var sql = "    SELECT " + BaseStaffEntity.TableName + ".* "
                                + " FROM " + BaseStaffEntity.TableName
                                + " WHERE " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldDeleted + " = 0 "
                                + "        AND (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldWorkgroupId + " IN ( " + organizeList + ") "
                                + "               OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldDepartmentId + " IN (" + organizeList + ") "
                                + "               OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldSubCompanyId + " IN (" + organizeList + ") "
                                + "               OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldCompanyId + " IN (" + organizeList + ")) "
                                + " ORDER BY " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldSortCode;
            return DbHelper.Fill(sql);
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
            var sql = "SELECT " + BaseStaffEntity.TableName + ".* "
                + " ,(SELECT " + BaseOrganizationEntity.FieldCode + " FROM " + BaseOrganizationEntity.TableName + " WHERE Id = " + BaseStaffEntity.TableName + ".CompanyId) AS CompanyCode"
                + " ,(SELECT " + BaseOrganizationEntity.FieldFullName + " FROM " + BaseOrganizationEntity.TableName + " WHERE Id = " + BaseStaffEntity.TableName + ".CompanyId) AS CompanyFullname "
                + " ,(SELECT " + BaseOrganizationEntity.FieldCode + " From " + BaseOrganizationEntity.TableName + " WHERE Id = " + BaseStaffEntity.TableName + ".DepartmentId) AS DepartmentCode"
                + " ,(SELECT " + BaseOrganizationEntity.FieldFullName + " FROM " + BaseOrganizationEntity.TableName + " WHERE Id = " + BaseStaffEntity.TableName + ".DepartmentId) AS DepartmentName "
                + " ,(SELECT " + BaseItemDetailsEntity.FieldItemName + " FROM ItemsDuty WHERE Id = " + BaseStaffEntity.TableName + ".DutyId) AS DutyName "
                + " ,(SELECT " + BaseItemDetailsEntity.FieldItemName + " FROM ItemsTitle WHERE Id = " + BaseStaffEntity.TableName + ".TitleId) AS TitleName "
                + " ,(SELECT " + BaseRoleEntity.FieldRealName + " FROM " + BaseRoleEntity.TableName + " WHERE Id = RoleId) AS RoleName "
                + " FROM " + BaseStaffEntity.TableName + " LEFT OUTER JOIN " + BaseUserEntity.TableName
                + "       ON " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldId + " = " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldId;
            if (!string.IsNullOrEmpty(departmentId))
            {
                sql += " WHERE " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldDepartmentId + " = '" + departmentId + "' ";
            }
            sql += " ORDER BY " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldSortCode;
            return DbHelper.Fill(sql);
        }
        #endregion

        #region public string GetStaffCount(IDbHelper dbHelper, string organizeCode)
        /// <summary>
		/// 获取部门的员工个数
		/// </summary>
		/// <param name="organizeCode">部门编码</param>
		/// <returns>员工个数</returns>
		public string GetStaffCount(string organizeCode)
        {
            var staffCount = string.Empty;
            var names = new string[1];
            var values = new object[1];
            var sql = @"SELECT COUNT(*) AS STAFFCOUNT FROM " + BaseStaffEntity.TableName + " WHERE (ENABLED = 1) AND (ISDIMISSION <> 1) AND (ISSTAFF = 1) AND (DepartmentId IN (SELECT Id FROM " + BaseOrganizationEntity.TableName + " WHERE (LEFT(CODE, LEN(?)) = ?))) ";
            names[0] = BaseStaffEntity.FieldCompanyId;
            values[0] = organizeCode;
            var result = DbHelper.ExecuteScalar(sql, DbHelper.MakeParameters(names, values));
            if (result != null)
            {
                staffCount = result.ToString();
            }
            return staffCount;
        }
        #endregion

        #region public string GetCategoryCount(IDbHelper dbHelper, string categoryId, string organizeCode, string categoryField)
        /// <summary>
        /// 获得某部门某种属性的人数
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="categoryId"></param>
        /// <param name="organizeCode"></param>
        /// <param name="categoryField"></param>
        /// <returns></returns>
        public string GetCategoryCount(IDbHelper dbHelper, string categoryId, string organizeCode, string categoryField)
        {
            var staffCount = string.Empty;
            var names = new string[3];
            var values = new object[3];
            var sql = "SELECT COUNT(*) AS STAFFCOUNT FROM " + BaseStaffEntity.TableName
                            + " WHERE (" + categoryField + " = ?) AND (ENABLED = 1) AND (ISDIMISSION <> 1) AND (ISSTAFF = 1) AND (DepartmentId IN (SELECT Id FROM " + BaseOrganizationEntity.TableName + " WHERE (LEFT(CODE, LEN(?)) = ?))) ";
            names[0] = categoryField;
            names[1] = BaseOrganizationEntity.FieldCode;
            names[2] = organizeCode;
            values[0] = categoryId;
            values[1] = organizeCode;
            values[2] = organizeCode;
            var result = dbHelper.ExecuteScalar(sql, DbHelper.MakeParameters(names, values));
            if (result != null)
            {
                staffCount = result.ToString();
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
            var sql = "SELECT " + BaseStaffEntity.TableName + ".* "
                                + " FROM " + BaseStaffEntity.TableName
                                + " WHERE 1=1 ";

            // 这里要注意系统安全隐患
            if (organizationIds != null)
            {
                // 可以管理的部门
                var organizes = string.Join(",", organizationIds);
                sql += " AND (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldWorkgroupId + " IN (" + organizes + ") ";
                sql += "     OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldDepartmentId + " IN (" + organizes + ") ";
                sql += "     OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldSubCompanyId + " IN (" + organizes + ") ";
                sql += "     OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldCompanyId + " IN (" + organizes + ")) ";
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
                sql += " AND " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldUserName + " LIKE ('" + userName + "') ";
            }

            if (!string.IsNullOrEmpty(enabled))
            {
                sql += " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldEnabled + " = '" + enabled + "'";
            }

            if (!string.IsNullOrEmpty(role))
            {
                // sql += " AND " + BaseUserEntity.FieldRoleId + " = '" + role + "'");
            }
            sql += " ORDER BY " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldSortCode;

            return DbHelper.Fill(sql);
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

                /*"SELECT A.* ,B.Code AS CompanyCode ,B.FullName AS CompanyName , " +
                        " C.Code AS DepartmentCode ,C.FullName AS DepartmentName ,D.ItemName AS DutyName ," +
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
            var sql = "SELECT " + BaseStaffEntity.TableName + ".* "
                            + "," + BaseOrganizationEntity.TableName + "A." + BaseOrganizationEntity.FieldCode + " AS CompanyCode "
                            + "," + BaseOrganizationEntity.TableName + "A." + BaseOrganizationEntity.FieldFullName + " AS CompanyName "
                            + "," + BaseOrganizationEntity.TableName + "B." + BaseOrganizationEntity.FieldCode + " AS DepartmentCode "
                            + "," + BaseOrganizationEntity.TableName + "B." + BaseOrganizationEntity.FieldFullName + " AS DepartmentName "
                            + ",ItemsDuty." + BaseItemDetailsEntity.FieldItemName + " AS DutyName "
                            + "," + "OT.RoleName "
                            + " FROM " + BaseStaffEntity.TableName
                            + "      LEFT OUTER JOIN " + BaseOrganizationEntity.TableName + " " + BaseOrganizationEntity.TableName + "A ON " + BaseOrganizationEntity.TableName + "A." + BaseOrganizationEntity.FieldId + " = " + BaseStaffEntity.FieldCompanyId
                            + "      LEFT OUTER JOIN " + BaseOrganizationEntity.TableName + " " + BaseOrganizationEntity.TableName + "B ON " + BaseOrganizationEntity.TableName + "B." + BaseOrganizationEntity.FieldId + " = " + BaseStaffEntity.FieldDepartmentId
                            + "      LEFT OUTER JOIN ItemsDuty ON ItemsDuty." + BaseItemDetailsEntity.FieldId + " = " + BaseStaffEntity.FieldDutyId
                            + "      ON " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldId + " = OT.Id ";
            if (string.IsNullOrEmpty(organizationId))
            {
                sql += " WHERE ((" + BaseOrganizationEntity.TableName + "A." + BaseOrganizationEntity.FieldIsInnerOrganization + " = 1) OR (" + BaseOrganizationEntity.TableName + "B." + BaseOrganizationEntity.FieldIsInnerOrganization + " =1)) ";
            }
            else
            {
                sql += " WHERE (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldCompanyId + " = '" + organizationId + "'"
                + " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldDepartmentId + " = '" + organizationId + "') ";
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                sql += " AND (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldUserName + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldRealName + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseOrganizationEntity.TableName + "A." + BaseOrganizationEntity.FieldFullName + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseOrganizationEntity.TableName + "B." + BaseOrganizationEntity.FieldFullName + " LIKE '%" + searchKey + "%'";
                sql += " OR " + "ItemsDuty." + BaseItemDetailsEntity.FieldItemName + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldOfficePhone + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldExtension + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldMobile + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldShortNumber + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldQq + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldEmail + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldDescription + " LIKE '%" + searchKey + "%'";
                sql += " OR OT.RoleName LIKE '%" + searchKey + "%')";
            }
            sql += " ORDER BY " + BaseOrganizationEntity.TableName + "B." + BaseOrganizationEntity.FieldSortCode
                          + " ," + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldSortCode;
            return DbHelper.Fill(sql);
        }
        #endregion

        #region public DataTable GetAddressDataTableByPage(string organizationId, string searchKey, out int recordCount, int pageIndex = 0, int pageSize = 100, string sort = null) 获取打印列表
        /// <summary>
        /// 获取列表 HJC
        /// </summary>
        /// <param name="recordCount">记录总数</param>
        /// <param name="organizationId">组织机构主键</param>
        /// <param name="searchKey">查询字符</param>
        /// <param name="pageSize">分页的条数</param>
        /// <param name="sort"></param>
        /// <param name="pageIndex">当前页数</param>
        /// <returns>数据表</returns>
        public DataTable GetAddressDataTableByPage(string organizationId, string searchKey, out int recordCount, int pageIndex = 0, int pageSize = 100, string sort = null)
        {
            // 因为Access中不支持分页，故此操作 假设行 1 //xtzwd
            if (BaseSystemInfo.UserCenterDbType == CurrentDbType.Access)
            {
                recordCount = 1;
                var dt = new DataTable();
                var cmd = "SELECT * from BaseStaff ";

                /*"SELECT A.* ,B.Code AS CompanyCode ,B.FullName AS CompanyName , " +
                        " C.Code AS DepartmentCode ,C.FullName AS DepartmentName ,D.ItemName AS DutyName ," +
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
            var sql = "SELECT " + BaseStaffEntity.TableName + ".* "
                            + " ," + BaseOrganizationEntity.TableName + "A." + BaseOrganizationEntity.FieldCode + " AS CompanyCode "
                            + " ," + BaseOrganizationEntity.TableName + "B." + BaseOrganizationEntity.FieldCode + " AS DepartmentCode "
                            + " ,ItemsDuty." + BaseItemDetailsEntity.FieldItemName + " AS DutyName "
                            + " FROM " + BaseStaffEntity.TableName
                            + "      LEFT OUTER JOIN " + BaseOrganizationEntity.TableName + " " + BaseOrganizationEntity.TableName + "A ON " + BaseOrganizationEntity.TableName + "A." + BaseOrganizationEntity.FieldId + " = " + BaseStaffEntity.FieldCompanyId
                            + "      LEFT OUTER JOIN " + BaseOrganizationEntity.TableName + " " + BaseOrganizationEntity.TableName + "B ON " + BaseOrganizationEntity.TableName + "B." + BaseOrganizationEntity.FieldId + " = " + BaseStaffEntity.FieldDepartmentId
                            + "      LEFT OUTER JOIN ItemsDuty ON ItemsDuty." + BaseItemDetailsEntity.FieldId + " = " + BaseStaffEntity.FieldDutyId;
            if (string.IsNullOrEmpty(organizationId))
            {
                sql += " WHERE ((" + BaseOrganizationEntity.TableName + "A." + BaseOrganizationEntity.FieldIsInnerOrganization + " = 1) OR (" + BaseOrganizationEntity.TableName + "B." + BaseOrganizationEntity.FieldIsInnerOrganization + " =1)) ";
            }
            else
            {
                sql += " WHERE (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldCompanyId + " = '" + organizationId + "'"
                + " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldDepartmentId + " = '" + organizationId + "') ";
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                sql += " AND (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldUserName + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldRealName + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "A." + BaseStaffEntity.FieldCompanyName + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "B." + BaseStaffEntity.FieldDepartmentName + " LIKE '%" + searchKey + "%'";
                sql += " OR " + "ItemsDuty." + BaseItemDetailsEntity.FieldItemName + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldOfficePhone + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldExtension + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldMobile + " LIKE '%" + searchKey + "'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldShortNumber + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldQq + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldEmail + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldDescription + " LIKE '%" + searchKey + "%')";
            }
            var orderBy = string.Empty;
            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.SqlServer:
                    orderBy = BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldSortCode;
                    break;
                case CurrentDbType.Db2:
                    orderBy = BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldSortCode;
                    break;
                default:
                    orderBy = BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldSortCode;
                    break;
            }
            return GetDataTableByPage(out recordCount, pageIndex, pageSize, orderBy, "ASC", sql);
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
            var sql = "SELECT " + BaseStaffEntity.TableName + ".* "
                            + "," + BaseOrganizationEntity.TableName + "." + BaseOrganizationEntity.FieldFullName + " AS DepartmentName "
                            + ",ItemsDuty." + BaseItemDetailsEntity.FieldItemName + " AS DutyName "
                            + " FROM " + BaseStaffEntity.TableName
                            + "      LEFT OUTER JOIN " + BaseOrganizationEntity.TableName + " ON " + BaseOrganizationEntity.TableName + "." + BaseOrganizationEntity.FieldId + " = " + BaseStaffEntity.FieldDepartmentId
                            + "      LEFT OUTER JOIN ItemsDuty ON  ItemsDuty." + BaseItemDetailsEntity.FieldId + " = " + BaseStaffEntity.FieldDutyId
                            + " WHERE (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldDeleted + " = " + (deletionStateCode ? 1 : 0) + ")";
            if (!string.IsNullOrEmpty(organizationId))
            {
                sql += " AND (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldDepartmentId + " = '" + organizationId + "' OR " + BaseStaffEntity.FieldCompanyId + " = '" + organizationId + "')";
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                sql += " AND (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldUserName + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldRealName + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldShortNumber + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldTelephone + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldMobile + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldEmail + " LIKE '%" + searchKey + "%'";
                sql += " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldQq + " LIKE '%" + searchKey + "%'";
                sql += " OR ItemsDuty." + BaseItemDetailsEntity.FieldItemName + " LIKE '%" + searchKey + "%')";
            }
            sql += " ORDER BY " // + BaseOrganizationEntity.TableName + "." + BaseOrganizationEntity.FieldSortCode
                                // + " ," 
                          + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldSortCode;
            return DbHelper.Fill(sql);
        }
        #endregion

        /// <summary>
        /// 获取下属员工
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataTable GetChildrenStaffs(string organizationId)
        {
            var organizeManager = new BaseOrganizationManager(DbHelper, UserInfo);
            string[] organizationIds = null;
            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.Access:
                case CurrentDbType.SqlServer:
                    var organizeCode = GetCodeById(organizationId);
                    organizationIds = organizeManager.GetChildrensIdByCode(BaseOrganizationEntity.FieldCode, organizeCode);
                    break;
                case CurrentDbType.Oracle:
                    organizationIds = organizeManager.GetChildrensId(BaseOrganizationEntity.FieldId, organizationId, BaseOrganizationEntity.FieldParentId);
                    break;
            }
            return GetDataTableByOrganizations(organizationIds);
        }

        /// <summary>
        /// 获取上级下的所有下属
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataTable GetParentChildrenStaffs(string organizationId)
        {
            var organizeManager = new BaseOrganizationManager(DbHelper, UserInfo);
            var organizeCode = organizeManager.GetCodeById(organizationId);
            var organizationIds = organizeManager.GetChildrensIdByCode(BaseOrganizationEntity.FieldCode, organizeCode);
            return GetDataTableByOrganizations(organizationIds);
        }

        #region public DataTable GetDataTable()
        /// <summary>
        /// 获取员工列表
        /// </summary>
        /// <returns>数据表</returns>
        public DataTable GetDataTable()
        {
            var sql = "SELECT " + BaseStaffEntity.TableName + ".* "
                + " , " + BaseUserEntity.TableName + ".UserOnline"
                + " ,(SELECT " + BaseOrganizationEntity.FieldCode + " FROM " + BaseOrganizationEntity.TableName + " WHERE Id = " + BaseStaffEntity.TableName + ".CompanyId) AS CompanyCode"
                + " ,(SELECT " + BaseOrganizationEntity.FieldFullName + " FROM " + BaseOrganizationEntity.TableName + " WHERE Id = " + BaseStaffEntity.TableName + ".CompanyId) AS CompanyFullname "

                + " ,(SELECT " + BaseOrganizationEntity.FieldCode + " From " + BaseOrganizationEntity.TableName + " WHERE Id = " + BaseStaffEntity.TableName + ".DepartmentId) AS DepartmentCode"
                + " ,(SELECT " + BaseOrganizationEntity.FieldFullName + " FROM " + BaseOrganizationEntity.TableName + " WHERE Id = " + BaseStaffEntity.TableName + ".DepartmentId) AS DepartmentName "

                + " ,(SELECT " + BaseOrganizationEntity.FieldCode + " From " + BaseOrganizationEntity.TableName + " WHERE Id = " + BaseStaffEntity.TableName + ".WorkgroupId) AS WorkgroupCode"
                + " ,(SELECT " + BaseOrganizationEntity.FieldFullName + " FROM " + BaseOrganizationEntity.TableName + " WHERE Id = " + BaseStaffEntity.TableName + ".WorkgroupId) AS WorkgroupName "

                + " ,(SELECT " + BaseItemDetailsEntity.FieldItemName + " FROM ItemsDuty WHERE Id = " + BaseStaffEntity.TableName + ".DutyId) AS DutyName "

                + " ,(SELECT " + BaseItemDetailsEntity.FieldItemName + " FROM ItemsTitle WHERE Id = " + BaseStaffEntity.TableName + ".TitleId) AS TitleName "

                + " ,(SELECT " + BaseRoleEntity.FieldRealName + " FROM " + BaseRoleEntity.TableName + " WHERE Id = RoleId) AS RoleName "
                // + " ,(SELECT COUNT(*) FROM " + BaseUserRoleEntity.TableName + " WHERE " + BaseUserRoleEntity.TableName + ".StaffID = " + BaseStaffEntity.TableName + ".Id) AS RoleCount "
                + " FROM (" + BaseStaffEntity.TableName + " LEFT OUTER JOIN " + BaseUserEntity.TableName
                + " ON " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldId + " = " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldId + ") "
                + "  LEFT OUTER JOIN " + BaseOrganizationEntity.TableName + " "
                + " ON " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldDepartmentId + " = " + BaseOrganizationEntity.TableName + "." + BaseOrganizationEntity.FieldId
                + " ORDER BY " + BaseOrganizationEntity.TableName + "." + BaseOrganizationEntity.FieldSortCode
                + " , " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldSortCode;
            return DbHelper.Fill(sql);
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
            var sql = "SELECT A.* "

                            + " ,(SELECT Code FROM " + BaseOrganizationEntity.TableName + " WHERE " + BaseOrganizationEntity.TableName + ".ID = A.CompanyId) AS CompanyCode"
                            + " ,(SELECT FullName FROM " + BaseOrganizationEntity.TableName + " WHERE " + BaseOrganizationEntity.TableName + ".ID = A.CompanyId) AS CompanyName "

                            + " ,(SELECT Code FROM " + BaseOrganizationEntity.TableName + " WHERE " + BaseOrganizationEntity.TableName + ".ID = A.DepartmentId) AS DepartmentCode"
                            + " ,(SELECT FullName FROM " + BaseOrganizationEntity.TableName + " WHERE " + BaseOrganizationEntity.TableName + ".ID = A.DepartmentId) AS DepartmentName "

                            + " ,(SELECT " + BaseOrganizationEntity.FieldCode + " From " + BaseOrganizationEntity.TableName + " WHERE Id = A.WorkgroupId) AS WorkgroupCode"
                            + " ,(SELECT " + BaseOrganizationEntity.FieldFullName + " FROM " + BaseOrganizationEntity.TableName + " WHERE Id = A.WorkgroupId) AS WorkgroupName "

                            + " ,(SELECT ItemName FROM ItemsDuty WHERE ItemsDuty.Id = A.DutyId) AS DutyName "

                            + " ,(SELECT ItemName FROM ItemsTitle WHERE ItemsTitle.Id = A.TitleId) AS TitleName "

                            + " FROM " + BaseStaffEntity.TableName + " A "
                            + " WHERE " + fieldName + " = " + DbHelper.GetParameter(fieldName)
                            + " ORDER BY A.SortCode";
            return DbHelper.Fill(sql, new IDbDataParameter[] { DbHelper.MakeParameter(fieldName, fieldValue) });
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
                        result += DeleteObject(id);
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
            //    BaseUserManager userManager = new BaseUserManager(DbHelper, UserInfo);
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
                var userManager = new BaseUserManager(DbHelper, UserInfo);
                result += userManager.SetDeleted(userId);
            }
            // 将员工的用户设置为空
            result += SetProperty(new KeyValuePair<string, object>(BaseStaffEntity.FieldId, staffId), new KeyValuePair<string, object>(BaseStaffEntity.FieldUserId, null));
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
                var userManager = new BaseUserManager(DbHelper, UserInfo);
                userManager.SetDeleted(userId);
            }
            // 再把员工设置为是否删除
            return SetProperty(new KeyValuePair<string, object>(BaseStaffEntity.FieldId, id), new KeyValuePair<string, object>(BaseStaffEntity.FieldDeleted, 1));
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
                result += SetProperty(id, new KeyValuePair<string, object>(BaseStaffEntity.FieldSortCode, sortCode));
            }
            return result;
        }
        #endregion

        #region public int Delete(string id) 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(string id)
        {
            var result = 0;
            var staffEntity = GetEntity(id);
            if (staffEntity != null)
            {
                var parameters = new List<KeyValuePair<string, object>>();
                // 删除相关的用户数据
                var userManager = new BaseUserManager(DbHelper, UserInfo);
                userManager.DeleteObject(staffEntity.UserId);
                // 删除员工本表
                parameters.Add(new KeyValuePair<string, object>(BaseStaffEntity.FieldId, id));
                result = DbUtil.Delete(DbHelper, BaseStaffEntity.TableName, parameters);
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
            return DbUtil.GetProperty(DbHelper, CurrentTableName, parameters, BaseUtil.FieldId);
        }
        #endregion
    }
}