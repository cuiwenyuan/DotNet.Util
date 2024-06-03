//-----------------------------------------------------------------------
// <copyright file="BaseOperationLogManager.cs" company="DotNet">
//     Copyright (c) 2024, All rights reserved.
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
    /// BaseOperationLogManager
    /// 操作日志管理层
    /// 
    /// 修改记录
    /// 
    ///	2021-11-08 版本：1.0 Troy.Cui 创建文件。
    ///		
    /// <author>
    ///	<name>Troy.Cui</name>
    ///	<date>2021-11-08</date>
    /// </author> 
    /// </summary>
    public partial class BaseOperationLogManager : BaseManager
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
        /// <param name="tableName">表名</param>
        /// <param name="tableDescription">表备注</param>
        /// <param name="operation">操作</param>
        /// <param name="recordKey">记录主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseOperationLogEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true, string tableName = null, string tableDescription = null, string operation = null, string recordKey = null)
        {
            var sb = PoolUtil.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseOperationLogEntity.FieldEnabled + " = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseOperationLogEntity.FieldDeleted + " = 0");
            }

            if (ValidateUtil.IsInt(companyId))
            {
                //sb.Append(" AND " + BaseOperationLogEntity.FieldCompanyId + " = " + companyId);
            }
            // 只有管理员才能看到所有的
            //if (!(UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled))
            //{
            //sb.Append(" AND (" + BaseOperationLogEntity.FieldUserCompanyId + " = 0 OR " + BaseOperationLogEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            //}
            if (ValidateUtil.IsInt(departmentId))
            {
                //sb.Append(" AND " + BaseOperationLogEntity.FieldDepartmentId + " = " + departmentId);
            }
            if (ValidateUtil.IsInt(userId))
            {
                //sb.Append(" AND " + BaseOperationLogEntity.FieldUserId + " = " + userId);
            }
            //创建时间
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseOperationLogEntity.FieldCreateTime + " >= " + dbHelper.ToDbTime(startTime));
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseOperationLogEntity.FieldCreateTime + " <= " + dbHelper.ToDbTime(endTime.ToDateTime().Date.AddDays(1).AddMilliseconds(-1)));
            }
            if (!string.IsNullOrEmpty(operation))
            {
                operation = dbHelper.SqlSafe(operation);
                sb.Append(" AND " + BaseOperationLogEntity.FieldOperation + " = N'" + operation + "'");
            }
            if (!string.IsNullOrEmpty(tableName))
            {
                tableName = dbHelper.SqlSafe(tableName);
                sb.Append(" AND " + BaseOperationLogEntity.FieldTableName + " = N'" + tableName + "'");
            }
            if (!string.IsNullOrEmpty(tableDescription))
            {
                tableDescription = dbHelper.SqlSafe(tableDescription);
                sb.Append(" AND " + BaseOperationLogEntity.FieldTableDescription + " = N'" + tableDescription + "'");
            }
            if (!string.IsNullOrEmpty(recordKey))
            {
                recordKey = dbHelper.SqlSafe(recordKey);
                sb.Append(" AND " + BaseOperationLogEntity.FieldRecordKey + " = N'" + recordKey + "'");
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseOperationLogEntity.FieldOperation + " LIKE N'%" + searchKey + "%' OR " + BaseOperationLogEntity.FieldTableName + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Return());
        }
        #endregion

        #region 快速新增无排序
        /// <summary>
        /// 快速新增无排序
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="tableDescription">表备注</param>
        /// <param name="operation">操作</param>
        /// <param name="recordKey">记录主键</param>
        /// <returns></returns>
        public string QuickAdd(string tableName, string tableDescription, string operation, string recordKey)
        {
            var entity = new BaseOperationLogEntity
            {
                SystemCode = BaseSystemInfo.SystemCode,
                TableName = tableName,
                TableDescription = tableDescription,
                RecordKey = recordKey,
                Operation = operation,
                SortCode = 1
            };
            return Add(entity);
        }
        #endregion
    }
}
