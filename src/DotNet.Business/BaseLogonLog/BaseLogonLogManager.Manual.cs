//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

using System.Data;

namespace DotNet.Business
{
    using Model;
    using System;
    using Util;

    /// <summary>
    /// BaseLogonLogManager
    /// 用户登录日志管理
    /// 
    /// 修改记录
    /// 
    ///		2016.09.22 版本：1.0 Troy.Cui	新增。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.09.22</date>
    /// </author> 
    /// </summary>
    public partial class BaseLogonLogManager : BaseManager
    {
        #region 高级查询
        /// <summary>
        /// 按条件分页高级查询
        /// </summary>
        /// <param name="systemCode">子系统</param>
        /// <param name="userId">用户主键</param>
        /// <param name="userName">用户名</param>
        /// <param name="companyName">公司名称</param>
        /// <param name="result">操作结果</param>
        /// <param name="startTime">创建开始时间</param>
        /// <param name="endTime">创建结束时间</param>
        /// <param name="searchKey">搜索关键词</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序规则</param>
        /// <returns></returns>
        public DataTable GetDataTableByPage(string systemCode, string userId, string userName, string companyName, string result, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = "CreateTime", string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseLogonLogEntity.FieldEnabled + " = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseLogonLogEntity.FieldDeleted + " = 0");
            }
            //子系统
            if (!string.IsNullOrEmpty(systemCode))
            {
                sb.Append(" AND " + BaseLogonLogEntity.CurrentTableName + "." + BaseLogonLogEntity.FieldSystemCode + " = N'" + systemCode + "'");
            }
            //用户主键
            if (ValidateUtil.IsInt(userId))
            {
                sb.Append(" AND " + BaseLogonLogEntity.CurrentTableName + "." + BaseLogonLogEntity.FieldUserId + " = N'" + userId + "'");
            }
            //用户名
            if (!string.IsNullOrEmpty(userName))
            {
                sb.Append(" AND " + BaseLogonLogEntity.CurrentTableName + "." + BaseLogonLogEntity.FieldUserName + " = N'" + userName + "'");
            }
            //公司名称
            if (!string.IsNullOrEmpty(companyName))
            {
                sb.Append(" AND " + BaseLogonLogEntity.CurrentTableName + "." + BaseLogonLogEntity.FieldCompanyName + " = N'" + companyName + "'");
            }
            //操作状态
            if (ValidateUtil.IsInt(result))
            {
                sb.Append(" AND " + BaseLogonLogEntity.CurrentTableName + "." + BaseLogonLogEntity.FieldResult + " = " + result);
            }
            //创建日期
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseLogonLogEntity.CurrentTableName + "." + BaseLogonLogEntity.FieldCreateTime + " >= " + dbHelper.ToDbTime(startTime));
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseLogonLogEntity.CurrentTableName + "." + BaseLogonLogEntity.FieldCreateTime + " <= " + dbHelper.ToDbTime(endTime.ToDateTime().Date.AddDays(1).AddMilliseconds(-1)));
            }
            //关键词
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseLogonLogEntity.FieldUserName + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldCompanyName + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldOperationType + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldIpAddress + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldLogonStatus + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldRealName + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Put(), null, "*");
        }
        #endregion

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
        /// <param name="systemCode">子系统编码</param>
        /// <param name="userName">用户名</param>
        /// <param name="companyName">公司名称</param>
        /// <param name="result">结果</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseLogonLogEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true, string systemCode = null, string userName = null, string companyName = null, string result = null)
        {
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseLogonLogEntity.FieldEnabled + " = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseLogonLogEntity.FieldDeleted + " = 0");
            }

            if (ValidateUtil.IsInt(companyId))
            {
                sb.Append(" AND " + BaseLogonLogEntity.FieldCompanyId + " = " + companyId);
            }
            // 只有管理员才能看到所有的
            //if (!(UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled))
            //{
            //sb.Append(" AND (" + BaseLogonLogEntity.FieldUserCompanyId + " = 0 OR " + BaseLogonLogEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            //}
            if (ValidateUtil.IsInt(departmentId))
            {
                //sb.Append(" AND " + BaseLogonLogEntity.FieldDepartmentId + " = " + departmentId);
            }
            if (ValidateUtil.IsInt(userId))
            {
                sb.Append(" AND " + BaseLogonLogEntity.FieldUserId + " = " + userId);
            }
            //创建时间
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseLogonLogEntity.FieldCreateTime + " >= " + dbHelper.ToDbTime(startTime));
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseLogonLogEntity.FieldCreateTime + " <= " + dbHelper.ToDbTime(endTime.ToDateTime().Date.AddDays(1).AddMilliseconds(-1)));
            }
            //子系统编码
            if (!string.IsNullOrEmpty(systemCode))
            {
                sb.Append(" AND " + BaseLogonLogEntity.CurrentTableName + "." + BaseLogonLogEntity.FieldSystemCode + " = N'" + systemCode + "'");
            }
            //用户名
            if (!string.IsNullOrEmpty(userName))
            {
                userName = dbHelper.SqlSafe(userName);
                sb.Append(" AND " + BaseLogonLogEntity.FieldUserName + " = N'" + userName + "'");
            }
            //操作状态
            if (!string.IsNullOrEmpty(result))
            {
                result = dbHelper.SqlSafe(result);
                sb.Append(" AND " + BaseLogonLogEntity.FieldResult + " = N'" + result + "'");
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseLogonLogEntity.FieldUserName + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldCompanyName + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldOperationType + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldIpAddress + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldLogonStatus + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldRealName + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Put(), null, "*");
        }
        #endregion
    }
}