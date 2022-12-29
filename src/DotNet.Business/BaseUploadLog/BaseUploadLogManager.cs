//-----------------------------------------------------------------------
// <copyright file="BaseUploadLogManager.cs" company="DotNet">
//     Copyright (c) 2022, All rights reserved.
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
    /// BaseUploadLogManager
    /// 文件上传日志管理层
    /// 
    /// 修改记录
    /// 
    ///	2022-12-16 版本：1.0 Troy.Cui 创建文件。
    ///		
    /// <author>
    ///	<name>Troy.Cui</name>
    ///	<date>2022-12-16</date>
    /// </author> 
    /// </summary>
    public partial class BaseUploadLogManager : BaseManager
    {
        #region public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseUploadLogEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = false, bool showDeleted = false)
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
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseUploadLogEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseUploadLogEntity.FieldEnabled + " = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseUploadLogEntity.FieldDeleted + " = 0");
            }

            if (ValidateUtil.IsInt(companyId))
            {
                //sb.Append(" AND " + BaseUploadLogEntity.FieldCompanyId + " = " + companyId);
            }
            // 只有管理员才能看到所有的
            if (!(UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled))
            {
            sb.Append(" AND (" + BaseUploadLogEntity.FieldUserCompanyId + " = 0 OR " + BaseUploadLogEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            }
            if (ValidateUtil.IsInt(departmentId))
            {
                //sb.Append(" AND " + BaseUploadLogEntity.FieldDepartmentId + " = " + departmentId);
            }
            if (ValidateUtil.IsInt(userId))
            {
                //sb.Append(" AND " + BaseUploadLogEntity.FieldUserId + " = " + userId);
            }
            //创建时间
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseUploadLogEntity.FieldCreateTime + " >= " + dbHelper.ToDbTime(startTime));
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseUploadLogEntity.FieldCreateTime + " <= " + dbHelper.ToDbTime(endTime.ToDateTime().Date.AddDays(1).AddMilliseconds(-1)));
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (FileName LIKE N'%" + searchKey + "%' OR FilePath LIKE N'%" + searchKey + "%')");
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
                    errorMessage = @"数据已被删除，无需再次删除";
                }
                else if (entity.UserCompanyId == 0)
                {
                    errorMessage = "系统数据无权操作";
                }
                //检查是否为自己公司的数据
                else if ((UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled) || entity.UserCompanyId.ToString().Equals(UserInfo.CompanyId))
                {
                    return base.SetDeleted(id, true, true);
                }
                else
                {
                    errorMessage = "非本公司数据无权操作";
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
                        errorMessage = @"数据已被删除，无需再次删除";
                    }
                    else if (entity.UserCompanyId == 0)
                    {
                        errorMessage = @"系统数据无权操作";
                    }
                    //检查是否为自己公司的数据
                    else if ((UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled) || entity.UserCompanyId.ToString().Equals(UserInfo.CompanyId))
                    {
                        result += base.SetDeleted(id, true, true);
                    }
                    else
                    {
                        errorMessage = @"非本公司数据无权操作";
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
                    errorMessage = @"数据未被删除，无需撤销";
                }
                else if (entity.UserCompanyId == 0)
                {
                    errorMessage = @"系统数据无权操作";
                }
                //检查是否为自己公司的数据
                else if ((UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled) || entity.UserCompanyId.ToString().Equals(UserInfo.CompanyId))
                {
                    return base.UndoSetDeleted(id, true, true);
                }
                else
                {
                    errorMessage = @"非本公司数据无权操作";
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
                        errorMessage = @"数据未被删除，无需撤销";
                    }
                    else if (entity.UserCompanyId == 0)
                    {
                        errorMessage = @"系统数据无权操作";
                    }
                    //检查是否为自己公司的数据
                    else if ((UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled) || entity.UserCompanyId.ToString().Equals(UserInfo.CompanyId))
                    {
                        result += base.SetDeleted(id, true, true);
                    }
                    else
                    {
                        errorMessage = @"非本公司数据无权操作";
                    }
                }
            }
            return result;
        }
        #endregion


        #region SetEnabled启用（自己公司的数据）

        /// <summary>
        /// 启用（自己公司的数据）
        /// </summary>
        /// <param name="id">编号</param>
        /// <param name="errorMessage">报错信息</param>
        /// <returns></returns>
        public int SetEnabled(string id, out string errorMessage)
        {
            var result = 0;
            errorMessage = string.Empty;
            var entity = GetEntity(id);
            if (entity != null)
            {
                if (entity.Enabled == 1)
                {
                    errorMessage = @"数据已启用，无需再次启用";
                }
                else if (entity.UserCompanyId == 0)
                {
                    errorMessage = "系统数据无权操作";
                }
                //检查是否为自己公司的数据
                else if ((UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled) || entity.UserCompanyId.ToString().Equals(UserInfo.CompanyId))
                {
                    return base.SetEnabled(id, recordUser: true);
                }
                else
                {
                    errorMessage = "非本公司数据无权操作";
                }
            }
            return result;
        }

        /// <summary>
        /// 批量启用（自己公司的数据）
        /// </summary>
        /// <param name="ids">编号</param>
        /// <param name="errorMessage">报错信息</param>
        /// <returns></returns>
        public int SetEnabled(string[] ids, out string errorMessage)
        {
            var result = 0;
            errorMessage = string.Empty;
            foreach (var id in ids)
            {
                var entity = GetEntity(id);
                if (entity != null)
                {
                    if (entity.Enabled == 1)
                    {
                        errorMessage = @"数据已启用，无需再次启用";
                    }
                    else if (entity.UserCompanyId == 0)
                    {
                        errorMessage = @"系统数据无权操作";
                    }
                    //检查是否为自己公司的数据
                    else if ((UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled) || entity.UserCompanyId.ToString().Equals(UserInfo.CompanyId))
                    {
                        result += base.SetEnabled(id, recordUser: true);
                    }
                    else
                    {
                        errorMessage = @"非本公司数据无权操作";
                    }
                }
            }

            return result;
        }
        #endregion

        #region SetDisabled禁用（自己公司的数据）

        /// <summary>
        /// 禁用（自己公司的数据）
        /// </summary>
        /// <param name="id">编号</param>
        /// <param name="errorMessage">报错信息</param>
        /// <returns></returns>
        public int SetDisabled(string id, out string errorMessage)
        {
            var result = 0;
            errorMessage = string.Empty;
            var entity = GetEntity(id);
            if (entity != null)
            {
                if (entity.Enabled == 0)
                {
                    errorMessage = @"数据已禁用，无需再次禁用";
                }
                else if (entity.UserCompanyId == 0)
                {
                    errorMessage = @"系统数据无权操作";
                }
                //检查是否为自己公司的数据
                else if ((UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled) || entity.UserCompanyId.ToString().Equals(UserInfo.CompanyId))
                {
                    return base.SetDisabled(id, recordUser: true);
                }
                else
                {
                    errorMessage = @"非本公司数据无权操作";
                }
            }
            return result;
        }

        /// <summary>
        /// 批量禁用（自己公司的数据）
        /// </summary>
        /// <param name="ids">编号</param>
        /// <param name="errorMessage">报错信息</param>
        /// <returns></returns>
        public int SetDisabled(string[] ids, out string errorMessage)
        {
            var result = 0;
            errorMessage = string.Empty;
            foreach (var id in ids)
            {
                var entity = GetEntity(id);
                if (entity != null)
                {
                    if (entity.Enabled == 0)
                    {
                        errorMessage = @"数据已禁用，无需再次禁用";
                    }
                    else if (entity.UserCompanyId == 0)
                    {
                        errorMessage = @"系统数据无权操作";
                    }
                    //检查是否为自己公司的数据
                    else if ((UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled) || entity.UserCompanyId.ToString().Equals(UserInfo.CompanyId))
                    {
                        result += base.SetDisabled(id, true);
                    }
                    else
                    {
                        errorMessage = @"非本公司数据无权操作";
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
        /// <param name="myCompanyOnly">仅本公司</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(bool myCompanyOnly = true)
        {
            var sb = Pool.StringBuilder.Get();
            if (myCompanyOnly)
            {
                sb.Append("(" + BaseUploadLogEntity.FieldUserCompanyId + " = 0 OR " + BaseUploadLogEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            }
            //return GetDataTable(sb.Put(), null, new KeyValuePair<string, object>(BaseUploadLogEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BaseUploadLogEntity.FieldDeleted, 0));
            var companyId = string.IsNullOrEmpty(BaseSystemInfo.CustomerCompanyId) ? UserInfo.CompanyId : BaseSystemInfo.CustomerCompanyId;
            var cacheKey = "DataTable." + CurrentTableName + "." + companyId + "." + (myCompanyOnly ? "1" : "0");
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            return CacheUtil.Cache<DataTable>(cacheKey, () => GetDataTable(sb.Put(), null, new KeyValuePair<string, object>(BaseUploadLogEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BaseUploadLogEntity.FieldDeleted, 0)), true, false, cacheTime);
        }

        #endregion
    }
}
