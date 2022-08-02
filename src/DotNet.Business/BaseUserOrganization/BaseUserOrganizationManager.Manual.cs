//-----------------------------------------------------------------------
// <copyright file="BaseUserOrganizationManager.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using System;
    using Util;

    /// <summary>
    /// BaseUserOrganizationManager
    /// 用户-组织结构关系管理
    /// 
    /// 修改记录
    /// 
    ///     2015.11.28 版本：1.1 JiRiGaLa	整理代码。
    ///     2010.09.25 版本：1.0 JiRiGaLa	创建。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.11.28</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserOrganizationManager : BaseManager
    {
        #region 高级查询
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
        /// <param name="subCompanyId">子公司编号</param>
        /// <param name="subDepartmentId">子部门编号</param>
        /// <param name="workgroupId">工作组编号</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseUserOrganizationEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true, string subCompanyId = null, string subDepartmentId = null, string workgroupId = null)
        {
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseUserOrganizationEntity.FieldEnabled + " = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseUserOrganizationEntity.FieldDeleted + " = 0");
            }

            if (ValidateUtil.IsInt(companyId))
            {
                sb.Append(" AND " + BaseUserOrganizationEntity.FieldCompanyId + " = " + companyId);
            }
            if (ValidateUtil.IsInt(subCompanyId))
            {
                sb.Append(" AND " + BaseUserOrganizationEntity.FieldSubCompanyId + " = " + subCompanyId);
            }
            // 只有管理员才能看到所有的
            //if (!(UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled))
            //{
            //sb.Append(" AND (" + BaseUserOrganizationEntity.FieldUserCompanyId + " = 0 OR " + BaseUserOrganizationEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            //}
            if (ValidateUtil.IsInt(departmentId))
            {
                sb.Append(" AND " + BaseUserOrganizationEntity.FieldDepartmentId + " = " + departmentId);
            }
            if (ValidateUtil.IsInt(subDepartmentId))
            {
                sb.Append(" AND " + BaseUserOrganizationEntity.FieldSubDepartmentId + " = " + subDepartmentId);
            }
            if (ValidateUtil.IsInt(workgroupId))
            {
                sb.Append(" AND " + BaseUserOrganizationEntity.FieldWorkgroupId + " = " + workgroupId);
            }
            if (ValidateUtil.IsInt(userId))
            {
                sb.Append(" AND " + BaseUserOrganizationEntity.FieldUserId + " = " + userId);
            }
            //创建时间
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseUserOrganizationEntity.FieldCreateTime + " >= '" + startTime + "'");
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseUserOrganizationEntity.FieldCreateTime + " <= DATEADD(s,-1,DATEADD(d,1,'" + endTime + "'))");
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseUserOrganizationEntity.FieldUserId + " LIKE N'%" + searchKey + "%' OR " + BaseUserOrganizationEntity.FieldDescription + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Put(), null, "*");
        }
        #endregion

        #region 添加用户组织机构关系

        /// <summary>
        /// 添加用户组织机构关系
        /// </summary>
        /// <param name="entity">用户组织机构实体</param>
        /// <param name="status">状态</param>
        /// <returns>主键</returns>
        public string Add(BaseUserOrganizationEntity entity, out Status status)
        {
            var result = string.Empty;
            // 判断数据是否重复了
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserOrganizationEntity.FieldUserId, entity.UserId),
                new KeyValuePair<string, object>(BaseUserOrganizationEntity.FieldCompanyId, entity.CompanyId),
                new KeyValuePair<string, object>(BaseUserOrganizationEntity.FieldSubCompanyId, entity.SubCompanyId),
                new KeyValuePair<string, object>(BaseUserOrganizationEntity.FieldDepartmentId, entity.DepartmentId),
                new KeyValuePair<string, object>(BaseUserOrganizationEntity.FieldSubDepartmentId, entity.SubDepartmentId),
                new KeyValuePair<string, object>(BaseUserOrganizationEntity.FieldWorkgroupId, entity.WorkgroupId),
                new KeyValuePair<string, object>(BaseUserOrganizationEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUserOrganizationEntity.FieldDeleted, 0)
            };
            if (Exists(parameters))
            {
                // 用户名已重复
                status = Status.Exist;
            }
            else
            {
                result = AddEntity(entity);
                // 运行成功
                status = Status.OkAdd;
            }
            return result;
        }

        #endregion

        #region 获得用户的组织机构兼职情况

        /// <summary>
        /// 获得用户的组织机构兼职情况
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns>数据表</returns>
        public DataTable GetUserOrganization(string userId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserOrganizationEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseUserOrganizationEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUserOrganizationEntity.FieldUserId, userId)
            };
            return GetDataTable(parameters);
        }

        #endregion

        #region 指定用户是否有指定组织编码的兼任

        /// <summary>
        /// 指定用户是否有指定组织编码的兼任
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="organizationCode">组织编码</param>
        /// <param name="organizationCategoryCode">组织类型</param>
        /// <returns>数据表</returns>
        public bool Exists(string userId, string organizationCode, string organizationCategoryCode = "Company")
        {
            var result = false;
            if (ValidateUtil.IsInt(userId))
            {
                var entity = new BaseOrganizationManager(UserInfo).GetEntityByCode(organizationCode);
                if (entity != null)
                {
                    var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserOrganizationEntity.FieldDeleted, 0),
                    new KeyValuePair<string, object>(BaseUserOrganizationEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseUserOrganizationEntity.FieldUserId, userId)
                };
                    if (organizationCategoryCode.Equals("Company", StringComparison.OrdinalIgnoreCase))
                    {
                        parameters.Add(new KeyValuePair<string, object>(BaseUserOrganizationEntity.FieldCompanyId, entity.Id));
                    }
                    else if (organizationCategoryCode.Equals("SubCompany", StringComparison.OrdinalIgnoreCase))
                    {
                        parameters.Add(new KeyValuePair<string, object>(BaseUserOrganizationEntity.FieldSubCompanyId, entity.Id));
                    }
                    else if (organizationCategoryCode.Equals("Department", StringComparison.OrdinalIgnoreCase))
                    {
                        parameters.Add(new KeyValuePair<string, object>(BaseUserOrganizationEntity.FieldDepartmentId, entity.Id));
                    }
                    else if (organizationCategoryCode.Equals("SubDepartment", StringComparison.OrdinalIgnoreCase))
                    {
                        parameters.Add(new KeyValuePair<string, object>(BaseUserOrganizationEntity.FieldSubDepartmentId, entity.Id));
                    }
                    else if (organizationCategoryCode.Equals("Workgroup", StringComparison.OrdinalIgnoreCase))
                    {
                        parameters.Add(new KeyValuePair<string, object>(BaseUserOrganizationEntity.FieldWorkgroupId, entity.Id));
                    }
                    result = Exists(parameters);
                }
            }
            return result;
        }

        #endregion
    }
}