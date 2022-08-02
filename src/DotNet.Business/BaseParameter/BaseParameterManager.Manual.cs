//-----------------------------------------------------------------------
// <copyright file="BaseParameterManager.cs" company="DotNet">
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
    /// BaseParameterManager
    /// 参数管理层
    /// 
    /// 修改记录
    /// 
    ///	2021-10-07 版本：1.0 Troy.Cui 创建文件。
    ///		
    /// <author>
    ///	<name>Troy.Cui</name>
    ///	<date>2021-10-07</date>
    /// </author> 
    /// </summary>
    public partial class BaseParameterManager : BaseManager
    {

        #region public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseParameterEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = false, bool showDeleted = false)
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
        /// <param name="systemCode">子系统编码</param>
        /// <param name="parameterId">参数编号</param>
        /// <param name="parameterCode">参数编码</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseParameterEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true, string systemCode = null, string parameterId = null, string parameterCode = null)
        {
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseParameterEntity.FieldEnabled + " = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseParameterEntity.FieldDeleted + " = 0");
            }

            if (ValidateUtil.IsInt(companyId))
            {
                //sb.Append(" AND " + BaseParameterEntity.FieldCompanyId + " = " + companyId);
            }
            // 只有管理员才能看到所有的
            //if (!(UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled))
            //{
            //sb.Append(" AND (" + BaseParameterEntity.FieldUserCompanyId + " = 0 OR " + BaseParameterEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            //}
            if (ValidateUtil.IsInt(departmentId))
            {
                //sb.Append(" AND " + BaseParameterEntity.FieldDepartmentId + " = " + departmentId);
            }
            if (ValidateUtil.IsInt(userId))
            {
                //sb.Append(" AND " + BaseParameterEntity.FieldUserId + " = " + userId);
            }
            //创建时间
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseParameterEntity.FieldCreateTime + " >= " + dbHelper.ToDbTime(startTime));
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseParameterEntity.FieldCreateTime + " <= " + dbHelper.ToDbTime(endTime.ToDateTime().Date.AddDays(1).AddMilliseconds(-1)));
            }
            if (!string.IsNullOrEmpty(systemCode))
            {
                systemCode = dbHelper.SqlSafe(systemCode);
                sb.Append(" AND " + BaseParameterEntity.FieldSystemCode + " = N'" + systemCode + "'");
            }
            if (!string.IsNullOrEmpty(parameterId))
            {
                parameterId = dbHelper.SqlSafe(parameterId);
                sb.Append(" AND " + BaseParameterEntity.FieldParameterId + " = N'" + parameterId + "'");
            }
            if (!string.IsNullOrEmpty(parameterCode))
            {
                parameterCode = dbHelper.SqlSafe(parameterCode);
                sb.Append(" AND " + BaseParameterEntity.FieldParameterCode + " = N'" + parameterCode + "'");
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseParameterEntity.FieldParameterCode + " LIKE N'%" + searchKey + "%' OR " + BaseParameterEntity.FieldDescription + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Put(), null, "*");
        }
        #endregion

        #region 获取当前用户的所有用户参数
        /// <summary>
        /// 获取当前用户的所有用户参数
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> GetUserParameterList()
        {
            var result = new List<KeyValuePair<string, string>>();
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseParameterEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldEnabled, 1)
            };
            if (UserInfo != null)
            {
                whereParameters.Add(new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterId, UserInfo.Id));
            }
            var list = GetList<BaseParameterEntity>(whereParameters, order: BaseParameterEntity.FieldParameterCode);
            foreach (var entity in list)
            {
                result.Add(new KeyValuePair<string, string>(entity.ParameterCode, entity.ParameterContent));
            }

            return result;
        }
        #endregion
    }
}
