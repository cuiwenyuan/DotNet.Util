//-----------------------------------------------------------------------
// <copyright file="BaseDictionaryItemManager.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
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
    /// BaseDictionaryItemManager
    /// 字典项管理层
    /// 
    /// 修改记录
    /// 
    ///	2021-10-26 版本：1.0 Troy.Cui 创建文件。
    ///		
    /// <author>
    ///	<name>Troy.Cui</name>
    ///	<date>2021-10-26</date>
    /// </author> 
    /// </summary>
    public partial class BaseDictionaryItemManager : BaseManager, IBaseManager
    {
        #region public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseDictionaryItemEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = false, bool showDeleted = false)
        /// <summary>
        /// 按条件分页查询(带记录状态Enabled和删除状态Deleted)
        /// </summary>
        /// <param name="companyId">查看公司主键</param>
        /// <param name="departmentId">查看部门主键</param>
        /// <param name="userId">查看用户主键</param>
        /// <param name="startTime">创建开始时间</param>
        /// <param name="endTime">创建结束时间</param>
        /// <param name="searchKey">查询字段</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="showDisabled">是否显示无效记录</param>
        /// <param name="showDeleted">是否显示已删除记录</param>
        /// <param name="dictionaryId">字典编号</param>
        /// <param name="dictionaryCode">字典编码</param>
        /// <param name="language">语言(i18n)</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseDictionaryItemEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true, string dictionaryId = null, string dictionaryCode = null, string language = "")
        {
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseDictionaryItemEntity.FieldEnabled + " = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseDictionaryItemEntity.FieldDeleted + " = 0");
            }

            if (ValidateUtil.IsInt(companyId))
            {
                //sb.Append(" AND " + BaseDictionaryItemEntity.FieldCompanyId + " = " + companyId);
            }
            // 只有管理员才能看到所有的
            //if (!(UserInfo.IsAdministrator && BaseSystemInfo.EnableAdministrator))
            //{
            //sb.Append(" AND (" + BaseDictionaryItemEntity.FieldUserCompanyId + " = 0 OR " + BaseDictionaryItemEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            //}
            if (ValidateUtil.IsInt(departmentId))
            {
                //sb.Append(" AND " + BaseDictionaryItemEntity.FieldDepartmentId + " = " + departmentId);
            }
            if (ValidateUtil.IsInt(userId))
            {
                //sb.Append(" AND " + BaseDictionaryItemEntity.FieldUserId + " = " + userId);
            }
            //创建时间
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseDictionaryItemEntity.FieldCreateTime + " >= '" + startTime + "'");
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseDictionaryItemEntity.FieldCreateTime + " <= DATEADD(s,-1,DATEADD(d,1,'" + endTime + "'))");
            }
            if (ValidateUtil.IsInt(dictionaryId))
            {
                sb.Append(" AND " + BaseDictionaryItemEntity.FieldDictionaryId + " = " + dictionaryId.ToInt());
            }
            if (!string.IsNullOrEmpty(dictionaryCode))
            {
                dictionaryCode = dbHelper.SqlSafe(dictionaryCode);
                sb.Append(" AND " + BaseDictionaryItemEntity.FieldDictionaryId + " IN (SELECT " + BaseDictionaryEntity.FieldId + " FROM " + BaseDictionaryEntity.CurrentTableName + " WHERE " + BaseDictionaryEntity.FieldEnabled + " = 1 AND " + BaseDictionaryEntity.FieldDeleted + " = 0 AND " + BaseDictionaryEntity.FieldCode + " = N'" + dictionaryCode + "')");
            }
            if (!string.IsNullOrEmpty(language))
            {
                language = dbHelper.SqlSafe(language);
                sb.Append(" AND " + BaseDictionaryItemEntity.FieldLanguage + " = N'" + language + "'");
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseDictionaryItemEntity.FieldItemKey + " LIKE N'%" + searchKey + "%' OR " + BaseDictionaryItemEntity.FieldItemName + " LIKE N'%" + searchKey + "%' OR " + BaseDictionaryItemEntity.FieldItemValue + " LIKE N'%" + searchKey + "%' OR " + BaseDictionaryItemEntity.FieldDescription + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Put(), null, "*");
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
            var sb = Pool.StringBuilder.Get();
            if (myCompanyOnly)
            {
                //sb.Append("(" + BaseDictionaryItemEntity.FieldUserCompanyId + " = 0 OR " + BaseDictionaryItemEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            }
            //return GetDataTable(sb.Put(), null, new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldDeleted, 0));
            var companyId = string.IsNullOrEmpty(BaseSystemInfo.CustomerCompanyId) ? UserInfo.CompanyId : BaseSystemInfo.CustomerCompanyId;
            var cacheKey = "DataTable." + CurrentTableName + "." + companyId + "." + (myCompanyOnly ? "1" : "0");
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            return CacheUtil.Cache<DataTable>(cacheKey, () => GetDataTable(sb.Put(), null, new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BaseDictionaryItemEntity.FieldDeleted, 0)), true, false, cacheTime);
        }

        #endregion
    }
}
