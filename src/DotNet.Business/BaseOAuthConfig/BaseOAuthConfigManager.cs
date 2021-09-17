//-----------------------------------------------------------------------
// <copyright file="BaseOAuthConfigManager.cs" company="DotNet">
//     Copyright (c) 2020, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Data;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseOAuthConfigManager
    /// OAuth配置管理层
    /// 
    /// 修改纪录
    /// 
    ///	2020-02-13 版本：1.0 Troy.Cui 创建文件。
    ///		
    /// <author>
    ///	<name>Troy.Cui</name>
    ///	<date>2020-02-13</date>
    /// </author> 
    /// </summary>
    public partial class BaseOAuthConfigManager : BaseManager, IBaseManager
    {
        #region public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string searchKey, out int recordCount, int pageIndex = 0, int pageSize = 20, string sortExpression = "CreateOn", string sortDirection = "DESC", bool showDisabled = false, bool showDeleted = false)
        /// <summary>
        /// 按条件分页查询(带记录状态Enabled和删除状态DeletionStateCode)
        /// </summary>
        /// <param name="companyId">查看公司主键</param>
        /// <param name="departmentId">查看部门主键</param>
        /// <param name="userId">查看用户主键</param>
        /// <param name="searchKey">查询字段</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="showDisabled">是否显示无效记录</param>
        /// <param name="showDeleted">是否显示已删除记录</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string searchKey, out int recordCount, int pageIndex = 0, int pageSize = 20, string sortExpression = "CreateOn", string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            pageIndex++;
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND Enabled = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND DeletionStateCode = 0");
            }

            if (ValidateUtil.IsInt(companyId))
            {
                //sb.Append(" AND CompanyId = " + companyId);
            }
            //sb.Append(" AND (UserCompanyId = 0 OR UserCompanyId = " + UserInfo.CompanyId + ")");
            if (ValidateUtil.IsInt(departmentId))
            {
                //sb.Append(" AND DepartmentId = " + departmentId);
            }
            if (ValidateUtil.IsInt(userId))
            {
                //sb.Append(" AND UserId = " + userId);
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (Name LIKE N'%" + searchKey + "%' OR Description LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageIndex, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Put(), null, "*");
        }
        #endregion

        #region public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageIndex = 0, int pageSize = 20, string sortExpression = "CreateOn", string sortDirection = "DESC", bool showDisabled = false, bool showDeleted = false)
        /// <summary>
        /// 按条件分页查询(带记录状态Enabled和删除状态DeletionStateCode)
        /// </summary>
        /// <param name="companyId">查看公司主键</param>
        /// <param name="departmentId">查看部门主键</param>
        /// <param name="userId">查看用户主键</param>
        /// <param name="startTime">创建开始时间</param>
        /// <param name="endTime">创建结束时间</param>
        /// <param name="searchKey">查询字段</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="showDisabled">是否显示无效记录</param>
        /// <param name="showDeleted">是否显示已删除记录</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageIndex = 0, int pageSize = 20, string sortExpression = "CreateOn", string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            pageIndex++;
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND Enabled = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND DeletionStateCode = 0");
            }

            if (ValidateUtil.IsInt(companyId))
            {
                //sb.Append(" AND CompanyId = " + companyId);
            }
            //sb.Append(" AND (UserCompanyId = 0 OR UserCompanyId = " + UserInfo.CompanyId + ")");
            if (ValidateUtil.IsInt(departmentId))
            {
                //sb.Append(" AND DepartmentId = " + departmentId);
            }
            if (ValidateUtil.IsInt(userId))
            {
                //sb.Append(" AND UserId = " + userId);
            }
            //创建日期
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND CreateOn >= '" + startTime + "'");
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND CreateOn <= DATEADD(s,-1,DATEADD(d,1,'" + endTime + "'))");
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (Name LIKE N'%" + searchKey + "%' OR Description LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageIndex, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Put(), null, "*");
        }
        #endregion

        #region 下拉菜单
        /// <summary>
        /// 下拉菜单
        /// </summary>
        /// <param name="myCompanyOnly"></param>
        /// <returns></returns>
        public DataTable GetDataTable(bool myCompanyOnly = true)
        {
            var sb = Pool.StringBuilder.Get();
            if (myCompanyOnly)
            {
                //sb.Append("(" + BaseOAuthConfigEntity.FieldUserCompanyId + " = 0 OR " + BaseOAuthConfigEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            }
            //return GetDataTable(where, null, new KeyValuePair<string, object>(BaseOAuthConfigEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BaseOAuthConfigEntity.FieldDeleted, 0));
            var companyId = string.IsNullOrEmpty(BaseSystemInfo.CustomerCompanyId) ? UserInfo.CompanyId : BaseSystemInfo.CustomerCompanyId;
            var cacheKey = "DataTable." + CurrentTableName + "." + companyId;
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            return CacheUtil.Cache<DataTable>(cacheKey, () => GetDataTable(sb.Put(), null, new KeyValuePair<string, object>(BaseOAuthConfigEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BaseOAuthConfigEntity.FieldDeleted, 0)), true, false, cacheTime);
        }

        #endregion
    }
}
