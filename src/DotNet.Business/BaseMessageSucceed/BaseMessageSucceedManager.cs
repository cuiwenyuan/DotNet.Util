//-----------------------------------------------------------------------
// <copyright file="BaseMessageSucceedManager.cs" company="DotNet">
//     Copyright (c) 2020, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseMessageSucceedManager
    /// 成功消息管理层
    /// 
    /// 修改记录
    /// 
    ///	2020-03-22 版本：1.0 Troy.Cui 创建文件。
    ///		
    /// <author>
    ///	<name>Troy.Cui</name>
    ///	<date>2020-03-22</date>
    /// </author> 
    /// </summary>
    public partial class BaseMessageSucceedManager : BaseManager, IBaseManager
    {
        #region public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = "CreateTime", string sortDirection = "DESC", bool showDisabled = false, bool showDeleted = false)
        /// <summary>
        /// 按条件分页查询(带记录状态Enabled和删除状态Deleted)
        /// </summary>
        /// <param name="companyId">查看公司主键</param>
        /// <param name="departmentId">查看部门主键</param>
        /// <param name="userId">查看用户主键</param>
        /// <param name="searchKey">查询字段</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="showDisabled">是否显示无效记录</param>
        /// <param name="showDeleted">是否显示已删除记录</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = "CreateTime", string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND Enabled = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND Deleted = 0");
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
                sb.Append(" AND (Recipient LIKE N'%" + searchKey + "%' OR Subject LIKE N'%" + searchKey + "%' OR Body LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Put(), null, "*");
        }
        #endregion

        #region public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = "CreateTime", string sortDirection = "DESC", bool showDisabled = false, bool showDeleted = false)
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
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = "CreateTime", string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND Enabled = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND Deleted = 0");
            }

            if (ValidateUtil.IsInt(companyId))
            {
                //sb.Append(" AND CompanyId = " + companyId);
            }
            sb.Append(" AND (UserCompanyId = 0 OR UserCompanyId = " + UserInfo.CompanyId + ")");
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
                sb.Append(" AND CreateTime >= '" + startTime + "'");
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND CreateTime <= DATEADD(s,-1,DATEADD(d,1,'" + endTime + "'))");
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (Recipient LIKE N'%" + searchKey + "%' OR Subject LIKE N'%" + searchKey + "%' OR Body LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Put(), null, "*");
        }
        #endregion

        #region SetDeleted删除（自己公司的数据）

        /// <summary>
        /// 删除（自己公司的数据）
        /// </summary>
        /// <param name="id">编号</param>
        /// <param name="errorMessage">报错信息</param>
        /// <returns></returns>
        public int SetDeleted(string id, out string errorMessage)
        {
            var result = 0;
            errorMessage = string.Empty;
            var entity = GetEntity(id);
            if (entity != null)
            {
                if (entity.Enabled == 0 || entity.Deleted == 1)
                {
                    errorMessage = @"数据已被删除，无法再次删除";
                }
                else if (entity.UserCompanyId == 0)
                {
                    errorMessage = "系统数据无法删除";
                }
                //检查是否为自己公司的数据
                else if (entity.UserCompanyId.ToString().Equals(UserInfo.CompanyId))
                {
                    return base.SetDeleted(id, true, true);
                }
                else
                {
                    errorMessage = "非本公司数据无法删除";
                }
            }
            return result;
        }

        /// <summary>
        /// 批量删除（自己公司的数据）
        /// </summary>
        /// <param name="ids">编号</param>
        /// <param name="errorMessage">报错信息</param>
        /// <returns></returns>
        public int SetDeleted(string[] ids, out string errorMessage)
        {
            var result = 0;
            errorMessage = string.Empty;
            foreach (var id in ids)
            {
                var entity = GetEntity(id);
                if (entity != null)
                {
                    if (entity.Enabled == 0 || entity.Deleted == 1)
                    {
                        errorMessage = @"数据已被删除，无法再次删除";
                    }
                    else if (entity.UserCompanyId == 0)
                    {
                        errorMessage = @"系统数据无法删除";
                    }
                    //检查是否为自己公司的数据
                    else if (entity.UserCompanyId.ToString().Equals(UserInfo.CompanyId))
                    {
                        result += base.SetDeleted(id, true, true);
                    }
                    else
                    {
                        errorMessage = @"非本公司数据无法删除";
                    }
                }
            }

            return result;
        }
        #endregion

        #region UndoSetDeleted撤销删除（自己公司的数据）

        /// <summary>
        /// 撤销删除（自己公司的数据）
        /// </summary>
        /// <param name="id">编号</param>
        /// <param name="errorMessage">报错信息</param>
        /// <returns></returns>
        public int UndoSetDeleted(string id, out string errorMessage)
        {
            var result = 0;
            errorMessage = string.Empty;
            var entity = GetEntity(id);
            if (entity != null)
            {
                if (entity.Enabled == 1 || entity.Deleted == 0)
                {
                    errorMessage = @"数据未被删除，无法撤销";
                }
                else if (entity.UserCompanyId == 0)
                {
                    errorMessage = @"系统数据无法删除";
                }
                //检查是否为自己公司的数据
                else if (entity.UserCompanyId.ToString().Equals(UserInfo.CompanyId))
                {
                    return base.UndoSetDeleted(id, true, true);
                }
                else
                {
                    errorMessage = @"非本公司数据无法撤销删除";
                }
            }
            return result;
        }

        /// <summary>
        /// 批量撤销删除（自己公司的数据）
        /// </summary>
        /// <param name="ids">编号</param>
        /// <param name="errorMessage">报错信息</param>
        /// <returns></returns>
        public int UndoSetDeleted(string[] ids, out string errorMessage)
        {
            var result = 0;
            errorMessage = string.Empty;
            foreach (var id in ids)
            {
                var entity = GetEntity(id);
                if (entity != null)
                {
                    if (entity.Enabled == 1 || entity.Deleted == 0)
                    {
                        errorMessage = @"数据未被删除，无法撤销";
                    }
                    else if (entity.UserCompanyId == 0)
                    {
                        errorMessage = @"系统数据无法删除";
                    }
                    //检查是否为自己公司的数据
                    else if (entity.UserCompanyId.ToString().Equals(UserInfo.CompanyId))
                    {
                        result += base.SetDeleted(id, true, true);
                    }
                    else
                    {
                        errorMessage = @"非本公司数据无法撤销删除";
                    }
                }
            }
            return result;
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
                sb.Append("(" + BaseMessageSucceedEntity.FieldUserCompanyId + " = 0 OR " + BaseMessageSucceedEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            }
            //return GetDataTable(where, null, new KeyValuePair<string, object>(BaseMessageSucceedEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BaseMessageSucceedEntity.FieldDeleted, 0));
            var companyId = string.IsNullOrEmpty(BaseSystemInfo.CustomerCompanyId) ? UserInfo.CompanyId : BaseSystemInfo.CustomerCompanyId;
            var cacheKey = "DataTable." + CurrentTableName + "." + companyId;
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            return CacheUtil.Cache<DataTable>(cacheKey, () => GetDataTable(sb.Put(), null, new KeyValuePair<string, object>(BaseMessageSucceedEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BaseMessageSucceedEntity.FieldDeleted, 0)), true, false, cacheTime);
        }

        #endregion
    }
}
