//-----------------------------------------------------------------------
// <copyright file="BaseUserManager.cs" company="DotNet">
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
    using System.Linq;
    using Util;

    /// <summary>
    /// BaseUserManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///     2011.10.17 版本：4.5 JiRiGaLa   拆分代码，按核心业务逻辑进行划分，简化代码。
    ///     2011.10.05 版本：4.4 张广梁     增加 public DataTable SearchByDepartment(string departmentId,string searchKey) ，获得部门和子部门的人员
    ///     2011.09.22 版本：4.3 张广梁     完善public DataTable GetAuthorizeDT(string permissionCode, string userId = null) 增加有效期的验证
    ///     2011.07.21 版本：4.2 zgl        修正检查IP和MAC的业务逻辑，如果没有设置IP或MAC时不执行检查
    ///     2011.07.05 版本：4.1 zgl        增加几个检查Ip的方法。
    ///     2011.07.04 版本：4.0 JiRiGaLa	用户名、密码的登录程序改进。
    ///     2011.06.29 版本：3.9 JiRiGaLa	每次登录时是否产生了一个新的OpenId。
    ///     2011.06.14 版本：3.8 JiRiGaLa	用户登录时间限制、锁定日期限制。
    ///     2011.02.12 版本：3.7 JiRiGaLa	按备注也可以查询。
    ///     2009.09.11 版本：3.6 JiRiGaLa	用户的审核状态功能改进。
    ///     2008.05.13 版本：3.6 JiRiGaLa	登录时数据获取进行了优化配置。
    ///     2008.03.18 版本：3.4 JiRiGaLa	登录、重新登录、扮演时的在线状态进行更新。
    ///     2007.10.02 版本：3.3 JiRiGaLa	登录限制改进。
    ///     2007.10.01 版本：3.2 JiRiGaLa	参数传递方式改进 IDbHelper dbHelper, BaseUserInfo userInfo。
    ///     2007.06.11 版本：3.1 JiRiGaLa	设置密码，修改密码进行大修改。
    ///     2006.12.15 版本：3.0 JiRiGaLa	程序排版重新整理一次。
    ///     2006.12.11 版本：2.2 JiRiGaLa	登录部分写入日志。
    ///     2006.12.02 版本：2.1 JiRiGaLa	登录部分的主键改进。
    ///     2006.11.23 版本：2.0 JiRiGaLa	结构优化整理。
    ///		2006.02.02 版本：1.0 JiRiGaLa	书写格式进行整理。
    ///		2005.01.23 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.10.17</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        #region public override DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseUserEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = false, bool showDeleted = false)
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
        public override DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseUserEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            var sb = PoolUtil.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseUserEntity.FieldEnabled + " = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseUserEntity.FieldDeleted + " = 0");
            }

            if (ValidateUtil.IsInt(companyId))
            {
                //sb.Append(" AND " + BaseUserEntity.FieldCompanyId + " = " + companyId);
            }
            // 只有管理员才能看到所有的
            //if (!(UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled))
            //{
            //sb.Append(" AND (" + BaseUserEntity.FieldUserCompanyId + " = 0 OR " + BaseUserEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            //}
            if (ValidateUtil.IsInt(departmentId))
            {
                //sb.Append(" AND " + BaseUserEntity.FieldDepartmentId + " = " + departmentId);
            }
            if (ValidateUtil.IsInt(userId))
            {
                //sb.Append(" AND " + BaseUserEntity.FieldUserId + " = " + userId);
            }
            //创建时间
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseUserEntity.FieldCreateTime + " >= " + dbHelper.ToDbTime(startTime));
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseUserEntity.FieldCreateTime + " <= " + dbHelper.ToDbTime(endTime.ToDateTime().Date.AddDays(1).AddMilliseconds(-1)));
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseUserEntity.FieldUserName + " LIKE N'%" + searchKey + "%' OR " + BaseUserEntity.FieldDescription + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", " ");
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Return());
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
            var sb = PoolUtil.StringBuilder.Get();
            if (myCompanyOnly)
            {
                //sb.Append("(" + BaseUserEntity.FieldUserCompanyId + " = 0 OR " + BaseUserEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            }
            //return GetDataTable(sb.Return(), null, new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0));
            var companyId = string.IsNullOrEmpty(BaseSystemInfo.CustomerCompanyId) ? UserInfo.CompanyId : BaseSystemInfo.CustomerCompanyId;
            var cacheKey = "Dt." + CurrentTableName + "." + companyId + "." + (myCompanyOnly ? "1" : "0");
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            return CacheUtil.Cache<DataTable>(cacheKey, () => GetDataTable(sb.Return(), null, new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)), true, false, cacheTime);
        }

        #endregion        
    }
}