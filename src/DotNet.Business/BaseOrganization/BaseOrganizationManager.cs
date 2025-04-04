﻿//-----------------------------------------------------------------------
// <copyright file="BaseOrganizationManager.cs" company="DotNet">
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
    using Util;

    /// <summary>
    /// BaseOrganizationManager（程序OK）
    /// 组织机构
    ///
    /// 修改记录
    /// 
    ///     2015-09-10 版本：3.4 JiRiGaLa   缓存预热,强制重新缓存。
    ///     2015-05-06 版本：3.3 PanQiMin   添加根据城市Id获取外网展示网点的方法。
    ///     2015-04-09 版本：3.3 PanQiMin   添加记录修改日志方法。
    ///     2007.12.02 版本：3.3 JiRiGaLa   增加 SetEntity 方法，优化主键。
    ///     2007.05.31 版本：3.2 JiRiGaLa   OkAdd，OkUpdate，OkDelete 状态进行改进整理。
    ///     2007.05.29 版本：3.1 JiRiGaLa   ErrorDeleted，ErrorChanged 状态进行改进整理。
    ///	    2007.05.29 版本：3.1 JiRiGaLa   BatchSave，ErrorDataRelated，force 进行改进整理。
    ///	    2007.05.29 版本：3.1 JiRiGaLa   StatusCode，StatusMessage 进行改进。
    ///	    2007.05.29 版本：3.1 JiRiGaLa   BatchSave 进行改进。
    ///		2007.04.18 版本：3.0 JiRiGaLa	重新整理主键。
    ///		2007.01.17 版本：2.0 JiRiGaLa	重新整理主键。
    ///		2006.02.06 版本：1.1 JiRiGaLa	重新调整主键的规范化。
    ///		2003.12.29 版本：1.0 JiRiGaLa	最后修改，改进成以后可以扩展到多种数据库的结构形式
    ///		2004.08.16 版本：1.0 JiRiGaLa	更新部分主键
    ///		2004.09.06 版本：1.0 JiRiGaLa	更新一些获得子部门，上级部门等的主键部分
    ///		2004.11.11 版本：1.0 JiRiGaLa	整理主键
    ///		2004.11.12 版本：1.0 JiRiGaLa	有些思想进行了改进。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2007.12.02</date>
    /// </author>
    /// </summary>
    public partial class BaseOrganizationManager : BaseManager //, IBaseOrganizationManager
    {
        #region public override DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseOrganizationEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = false, bool showDeleted = false)
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
        public override DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseOrganizationEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            var sb = PoolUtil.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseOrganizationEntity.FieldEnabled + " = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseOrganizationEntity.FieldDeleted + " = 0");
            }

            if (ValidateUtil.IsInt(companyId))
            {
                //sb.Append(" AND " + BaseOrganizationEntity.FieldCompanyId + " = " + companyId);
            }
            // 只有管理员才能看到所有的
            //if (!(UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled))
            //{
            //sb.Append(" AND (" + BaseOrganizationEntity.FieldUserCompanyId + " = 0 OR " + BaseOrganizationEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            //}
            if (ValidateUtil.IsInt(departmentId))
            {
                //sb.Append(" AND " + BaseOrganizationEntity.FieldDepartmentId + " = " + departmentId);
            }
            if (ValidateUtil.IsInt(userId))
            {
                //sb.Append(" AND " + BaseOrganizationEntity.FieldUserId + " = " + userId);
            }
            //创建时间
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseOrganizationEntity.FieldCreateTime + " >= " + dbHelper.ToDbTime(startTime));
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseOrganizationEntity.FieldCreateTime + " <= " + dbHelper.ToDbTime(endTime.ToDateTime().Date.AddDays(1).AddMilliseconds(-1)));
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseOrganizationEntity.FieldName + " LIKE N'%" + searchKey + "%' OR " + BaseOrganizationEntity.FieldDescription + " LIKE N'%" + searchKey + "%')");
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
                //sb.Append("(" + BaseOrganizationEntity.FieldUserCompanyId + " = 0 OR " + BaseOrganizationEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            }
            //return GetDataTable(sb.Return(), null, new KeyValuePair<string, object>(BaseOrganizationEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0));
            var companyId = string.IsNullOrEmpty(BaseSystemInfo.CustomerCompanyId) ? UserInfo.CompanyId : BaseSystemInfo.CustomerCompanyId;
            var cacheKey = "Dt." + CurrentTableName + "." + companyId + "." + (myCompanyOnly ? "1" : "0");
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            return CacheUtil.Cache<DataTable>(cacheKey, () => GetDataTable(sb.Return(), null, new KeyValuePair<string, object>(BaseOrganizationEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0)), true, false, cacheTime);
        }

        #endregion

        // 当前的锁
        // private static object locker = new Object();
        /// <summary>
        /// 获取名字
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GetNames(List<BaseOrganizationEntity> list)
        {
            var result = string.Empty;

            foreach (var entity in list)
            {
                result += "," + entity.Name;
            }
            if (!string.IsNullOrEmpty(result))
            {
                result = result.Substring(1);
            }

            return result;
        }

        /// <summary>
        /// 按编号获取实体
        /// </summary>
        /// <param name="code">编号</param>
        public BaseOrganizationEntity GetEntityByCode(string code)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseOrganizationEntity.FieldCode, code),
                new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0)
            };
            return BaseEntity.Create<BaseOrganizationEntity>(GetDataTable(parameters));
        }

        /// <summary>
        /// 按名称获取实体
        /// </summary>
        /// <param name="name">名称</param>
        public BaseOrganizationEntity GetEntityByName(string name)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseOrganizationEntity.FieldName, name),
                new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0)
            };
            return BaseEntity.Create<BaseOrganizationEntity>(GetDataTable(parameters));
        }

        /// <summary>
        /// 获取内部组织
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataTable GetInnerOrganization(string organizationId = null)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            if (!string.IsNullOrEmpty(organizationId))
            {
                parameters.Add(new KeyValuePair<string, object>(BaseOrganizationEntity.FieldParentId, organizationId));
            }
            parameters.Add(new KeyValuePair<string, object>(BaseOrganizationEntity.FieldIsInnerOrganization, 1));
            parameters.Add(new KeyValuePair<string, object>(BaseOrganizationEntity.FieldEnabled, 1));
            parameters.Add(new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0));
            return GetDataTable(parameters, BaseOrganizationEntity.FieldSortCode);
        }

        /// <summary>
        /// 获取部门数据表
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataTable GetCompanyDt(string organizationId = null)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            if (!string.IsNullOrEmpty(organizationId))
            {
                parameters.Add(new KeyValuePair<string, object>(BaseOrganizationEntity.FieldParentId, organizationId));
            }
            parameters.Add(new KeyValuePair<string, object>(BaseOrganizationEntity.FieldCategoryCode, "Company"));
            parameters.Add(new KeyValuePair<string, object>(BaseOrganizationEntity.FieldEnabled, 1));
            parameters.Add(new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0));
            return GetDataTable(parameters, BaseOrganizationEntity.FieldSortCode);
        }

        /// <summary>
        /// 获取部门全称
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public DataTable GetNameDepartment(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                var subCompanyNameEntity = GetEntity(dr[BaseOrganizationEntity.FieldParentId].ToString());
                dr[BaseOrganizationEntity.FieldName] = subCompanyNameEntity.Name + "--" + dr[BaseOrganizationEntity.FieldName];
                var companyEntity = GetEntity(subCompanyNameEntity.ParentId.ToString());
                dr[BaseOrganizationEntity.FieldName] = companyEntity.Name + "--" + dr[BaseOrganizationEntity.FieldName];
            }
            return dt;
        }

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public override int BatchSave(DataTable dt)
        {
            var result = 0;
            var entity = new BaseOrganizationEntity();
            foreach (DataRow dr in dt.Rows)
            {
                // 删除状态
                if (dr.RowState == DataRowState.Deleted)
                {
                    var id = dr[BaseOrganizationEntity.FieldId, DataRowVersion.Original].ToString();
                    if (id.Length > 0)
                    {
                        result += DeleteEntity(id);
                    }
                }
                // 被修改过
                if (dr.RowState == DataRowState.Modified)
                {
                    var id = dr[BaseOrganizationEntity.FieldId, DataRowVersion.Original].ToString();
                    if (id.Length > 0)
                    {
                        entity.GetFrom(dr);
                        result += UpdateEntity(entity);
                    }
                }
                // 添加状态
                if (dr.RowState == DataRowState.Added)
                {
                    entity.GetFrom(dr);
                    result += AddEntity(entity).Length > 0 ? 1 : 0;
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
            return result;
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="parentId">父级主键</param>
        /// <returns>影响行数</returns>
        public int MoveTo(string id, string parentId)
        {
            return Update(id, new KeyValuePair<string, object>(BaseOrganizationEntity.FieldParentId, parentId));
        }

        /// <summary>
        /// 获取错误数据表
        /// </summary>
        /// <param name="parentId">上级网点</param>
        /// <returns>错误数据表</returns>
        public DataTable GetErrorDataTable(string parentId)
        {
            DataTable result = null;
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("  SELECT * FROM " + BaseOrganizationEntity.CurrentTableName
                        + " WHERE  (" + BaseOrganizationEntity.FieldProvinceId + " IS NULL OR "
                        + BaseOrganizationEntity.FieldCityId + " IS NULL OR "
                        + BaseOrganizationEntity.FieldDistrictId + " IS NULL ) AND " + BaseOrganizationEntity.FieldEnabled + " = 1"
                        + " AND " + BaseOrganizationEntity.FieldDeleted + " = 0");
            if (!string.IsNullOrWhiteSpace(parentId))
            {
                sb.Append(" START WITH Id = " + parentId + " "
                   + "  CONNECT BY PRIOR " + BaseOrganizationEntity.FieldId + " = " + BaseOrganizationEntity.FieldParentId);
            }
            sb.Append(" ORDER BY " + BaseOrganizationEntity.FieldSortCode);
            result = DbHelper.Fill(sb.Return());
            result.TableName = BaseOrganizationEntity.CurrentTableName;
            return result;
        }

        #region 获取组织树形结构表       
        /// <summary>
        /// 部门缓存表
        /// </summary>
        private DataTable _dtOrganization = null;

        /// <summary>
        ///  部门名称前缀
        /// </summary>
        private string _head = "|";

        /// <summary>
        /// 部门绑定表
        /// </summary>
        private DataTable _organizationTable = new DataTable(BaseOrganizationEntity.CurrentTableName);

        #region public DataTable GetOrganizationTree(DataTable dtOrganization = null) 绑定下拉框数据,组织机构树表
        /// <summary>
        /// 绑定下拉框数据,组织机构树表
        /// </summary>
        /// <param name="dtOrganization">组织机构</param>
        /// <returns>组织机构树表</returns>
        public DataTable GetOrganizationTree(DataTable dtOrganization = null)
        {
            if (dtOrganization != null)
            {
                _dtOrganization = dtOrganization;
            }
            else
            {
                //2017.12.20增加默认的HttpRuntime.Cache缓存
                var cacheKey = "Dt.BaseOrganization";
                //var cacheTime = default(TimeSpan);
                var cacheTime = TimeSpan.FromMilliseconds(86400000);
                _dtOrganization = CacheUtil.Cache<DataTable>(cacheKey, () =>
                {
                    //获取所有数据
                    var parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BaseOrganizationEntity.FieldIsInnerOrganization, 1),
                        new KeyValuePair<string, object>(BaseOrganizationEntity.FieldEnabled, 1),
                        new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0)
                    };
                    return GetDataTable(parameters, BaseOrganizationEntity.FieldSortCode);
                }, true, false, cacheTime);

                // 直接读取数据库
                //List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>
                //{
                //    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldIsInnerOrganization, 1),
                //    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldEnabled, 1),
                //    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0)
                //};
                //_dtOrganization = this.GetDataTable(parameters, BaseOrganizationEntity.FieldSortCode);
            }
            // 初始化部门表
            if (_organizationTable.Columns.Count == 0)
            {
                // 建立表的列，不能重复建立
                _organizationTable.Columns.Add(new DataColumn(BaseOrganizationEntity.FieldId, Type.GetType("System.Int32")));
                _organizationTable.Columns.Add(new DataColumn(BaseOrganizationEntity.FieldName, Type.GetType("System.String")));
            }

            // 查找子部门
            for (var i = 0; i < _dtOrganization.Rows.Count; i++)
            {
                // Null或0
                if (BaseUtil.ConvertToInt(_dtOrganization.Rows[i][BaseOrganizationEntity.FieldParentId]) == 0)
                {
                    var dr = _organizationTable.NewRow();
                    dr[BaseOrganizationEntity.FieldId] = _dtOrganization.Rows[i][BaseOrganizationEntity.FieldId];
                    dr[BaseOrganizationEntity.FieldName] = _dtOrganization.Rows[i][BaseOrganizationEntity.FieldName];
                    _organizationTable.Rows.Add(dr);
                    GetSubOrganization(BaseUtil.ConvertToInt(_dtOrganization.Rows[i][BaseOrganizationEntity.FieldId]));
                }
            }
            return _organizationTable;
        }
        #endregion

        #region public void GetSubOrganization(object parentId)
        /// <summary>
        /// 获取子部门
        /// </summary>
        /// <param name="parentId">父节点主键</param>
        public void GetSubOrganization(int parentId)
        {
            _head += "--";
            for (var i = 0; i < _dtOrganization.Rows.Count; i++)
            {
                if (BaseUtil.ConvertToInt(_dtOrganization.Rows[i][BaseOrganizationEntity.FieldParentId]) == parentId)
                {
                    var dr = _organizationTable.NewRow();
                    dr[BaseOrganizationEntity.FieldId] = _dtOrganization.Rows[i][BaseOrganizationEntity.FieldId];
                    dr[BaseOrganizationEntity.FieldName] = _head + _dtOrganization.Rows[i][BaseOrganizationEntity.FieldName];
                    _organizationTable.Rows.Add(dr);
                    GetSubOrganization(BaseUtil.ConvertToInt(_dtOrganization.Rows[i][BaseOrganizationEntity.FieldId]));
                }
            }
            // 子级遍历完成后，退到父级
            _head = _head.Substring(0, _head.Length - 2);
        }
        #endregion

        #endregion

        /// <summary>
        /// 获取孩子节点属性
        /// </summary>
        /// <param name="parentId">上级主键</param>
        /// <param name="field">选择的字段</param>
        /// <returns>孩子属性数组</returns>
        public string[] GetChildrenProperties(string parentId, string field)
        {
            string[] result = null;
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("  SELECT " + field
                        + " FROM " + BaseOrganizationEntity.CurrentTableName
                        + " WHERE " + BaseOrganizationEntity.FieldEnabled + " = 1"
                        + " AND " + BaseOrganizationEntity.FieldDeleted + " = 0"
                        + " START WITH Id = " + parentId + " "
                        + " CONNECT BY PRIOR " + BaseOrganizationEntity.FieldId + " = " + BaseOrganizationEntity.FieldParentId
                        + " ORDER BY " + BaseOrganizationEntity.FieldSortCode);
            var dt = DbHelper.Fill(sb.Return());
            result = BaseUtil.FieldToArray(dt, field);
            return result;
        }

        /// <summary>
        /// 获取组织机构列表
        /// </summary>
        /// <param name="parentId">上级主键</param>
        /// <param name="childrens">包含树形子节点</param>
        /// <param name="categoryCode">组织分类（Company,SubCompany,Department,SubDepartment,Workgroup）</param>
        /// <returns>数据表</returns>
        public DataTable GetOrganizationDataTable(string parentId = null, bool childrens = false, string categoryCode = "Company")
        {
            DataTable result = null;
            if (ValidateUtil.IsInt(parentId) && childrens)
            {
                var sb = PoolUtil.StringBuilder.Get();
                if (dbHelper.CurrentDbType == CurrentDbType.Oracle)
                {
                    sb.Append("SELECT * FROM " + BaseOrganizationEntity.CurrentTableName
                               + " WHERE " + BaseOrganizationEntity.FieldEnabled + " = 1"
                               + " AND " + BaseOrganizationEntity.FieldDeleted + " = 0"
                               + " AND (" + BaseOrganizationEntity.FieldCategoryCode + "= '" + categoryCode + "' OR " + BaseOrganizationEntity.FieldCategoryCode + "= 'Sub" + categoryCode + "')"
                               + " START WITH Id = " + parentId + " "
                               + " CONNECT BY PRIOR " + BaseOrganizationEntity.FieldId + " = " + BaseOrganizationEntity.FieldParentId
                               + " ORDER BY " + BaseOrganizationEntity.FieldSortCode);
                    result = DbHelper.Fill(sb.Return());
                }
                //此处递归查询需要完善 Troy.Cui 2018.07.21
                else if (dbHelper.CurrentDbType == CurrentDbType.SqlServer)
                {

                }
            }
            else
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldCategoryCode, categoryCode),
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0)
                };
                if (ValidateUtil.IsInt(parentId))
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseOrganizationEntity.FieldParentId, parentId));
                }
                result = GetDataTable(parameters, BaseOrganizationEntity.FieldSortCode);
            }

            if (result != null)
            {
                result.TableName = BaseOrganizationEntity.CurrentTableName;
            }
            return result;
        }

        /// <summary>
        /// 获取拼音
        /// </summary>
        /// <returns></returns>
        public int GetPinYin()
        {
            var result = 0;
            var list = GetList<BaseOrganizationEntity>();
            foreach (var entity in list)
            {
                if (string.IsNullOrEmpty(entity.QuickQuery))
                {
                    // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
                    entity.QuickQuery = StringUtil.GetPinyin(entity.Name).ToLower();
                }
                if (string.IsNullOrEmpty(entity.SimpleSpelling))
                {
                    // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
                    entity.SimpleSpelling = StringUtil.GetSimpleSpelling(entity.Name).ToLower();
                }
                result += UpdateEntity(entity);
            }
            return result;
        }

        #region public DataTable Search(string searchKey, string parentId = null, bool isInnerOrganization = true, bool childrens = false) 搜索组织机构
        /// <summary>
        /// 搜索组织机构
        /// </summary>
        /// <param name="searchKey">查询内容</param>
        /// <param name="parentId">上级组织机构</param>
        /// <param name="isInnerOrganization">内部组织机构</param>
        /// <param name="childrens">包含子结点</param>
        /// <returns>数据表</returns>
        public DataTable Search(string searchKey, string parentId = null, bool? isInnerOrganization = null, bool? childrens = null)
        {
            var sb = PoolUtil.StringBuilder.Get();
            List<IDbDataParameter> dbParameters;
            if (!childrens.HasValue || (childrens.HasValue && childrens.Value == false))
            {
                sb.Append("SELECT * FROM " + CurrentTableName + " WHERE " + BaseOrganizationEntity.FieldDeleted + " =  0 ");
                if (isInnerOrganization.HasValue)
                {
                    var innerOrganization = isInnerOrganization.Value == true ? "1" : "0";
                    sb.Append(string.Format(" AND {0} = {1}", BaseOrganizationEntity.FieldIsInnerOrganization, innerOrganization));
                }
                if (!string.IsNullOrEmpty(parentId))
                {
                    sb.Append(string.Format(" AND {0} = {1}", BaseOrganizationEntity.FieldParentId, parentId));
                }

                dbParameters = new List<IDbDataParameter>();
                searchKey = searchKey.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchKey))
                {
                    sb.Append(string.Format(" AND ({0} LIKE {1}", BaseOrganizationEntity.FieldName, DbHelper.GetParameter(BaseOrganizationEntity.FieldName)));
                    sb.Append(string.Format(" OR {0} LIKE {1}", BaseOrganizationEntity.FieldSimpleSpelling, DbHelper.GetParameter(BaseOrganizationEntity.FieldName)));
                    sb.Append(string.Format(" OR {0} LIKE {1} )", BaseOrganizationEntity.FieldQuickQuery, DbHelper.GetParameter(BaseOrganizationEntity.FieldName)));
                    if (searchKey.IndexOf("%") < 0)
                    {
                        searchKey = string.Format("%{0}%", searchKey);
                    }
                    dbParameters.Add(DbHelper.MakeParameter(BaseOrganizationEntity.FieldName, searchKey));
                }
                sb.Append(" ORDER BY " + BaseOrganizationEntity.FieldSortCode);
                return DbHelper.Fill(sb.Return(), dbParameters.ToArray());
            }
            else
            {
                sb.Append("SELECT * FROM " + BaseOrganizationEntity.CurrentTableName
                         + " WHERE " + BaseOrganizationEntity.FieldEnabled + " = 1"
                         + " AND " + BaseOrganizationEntity.FieldDeleted + " = 0");

                dbParameters = new List<IDbDataParameter>();
                searchKey = searchKey.Trim();
                if (!string.IsNullOrEmpty(searchKey))
                {
                    sb.Append(string.Format(" AND ({0} LIKE {1}", BaseOrganizationEntity.FieldName, DbHelper.GetParameter(BaseOrganizationEntity.FieldName)));
                    sb.Append(string.Format(" OR {0} LIKE {1}", BaseOrganizationEntity.FieldSimpleSpelling, DbHelper.GetParameter(BaseOrganizationEntity.FieldName)));
                    sb.Append(string.Format(" OR {0} LIKE {1} )", BaseOrganizationEntity.FieldQuickQuery, DbHelper.GetParameter(BaseOrganizationEntity.FieldName)));
                    if (searchKey.IndexOf("%") < 0)
                    {
                        searchKey = string.Format("%{0}%", searchKey);
                    }
                    dbParameters.Add(DbHelper.MakeParameter(BaseOrganizationEntity.FieldName, searchKey));
                }

                if (!string.IsNullOrEmpty(parentId))
                {
                    sb.Append("  START WITH Id = " + parentId + " "
                             + "  CONNECT BY PRIOR " + BaseOrganizationEntity.FieldId + " = " + BaseOrganizationEntity.FieldParentId);
                }

                sb.Append(" ORDER BY " + BaseOrganizationEntity.FieldSortCode);
                return DbHelper.Fill(sb.Return(), dbParameters.ToArray());
            }
        }
        #endregion

        /// <summary>
        /// 快速查找上级用
        /// 2016-01-06 吉日嘎拉
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>上级主键</returns>
        public static string GetParentIdByCache(string id)
        {
            var result = string.Empty;

            if (!string.IsNullOrEmpty(id))
            {
                var entity = GetEntityByCache(id);
                if (entity != null)
                {
                    result = entity.ParentId.ToString();
                    // 2016-01-06 吉日嘎拉 这里防止死循环的处理，跳出循环
                    if (id.Equals(result))
                    {
                        result = string.Empty;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// 从缓存获取
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static string GetIdByNameByCache(string name)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(name))
            {
                var entity = GetEntityByNameByCache(name);
                if (entity != null)
                {
                    result = entity.Id.ToString();
                }
            }
            return result;
        }
        /// <summary>
        /// 从缓存获取
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetNameByCodeByCache(string code)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(code))
            {
                var entity = GetEntityByCodeByCache(code);
                if (entity != null)
                {
                    result = entity.Name;
                }
            }
            return result;
        }

        /// <summary>
        /// 从缓存获取
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetIdByCodeByCache(string code)
        {
            var result = string.Empty;

            var entity = GetEntityByCodeByCache(code);
            if (entity != null)
            {
                result = entity.Id.ToString();
            }

            return result;
        }

        #region public static string GetNameByCache(string id) 通过编号获取选项的显示内容
        /// <summary>
        /// 通过编号获取选项的显示内容
        /// 这里是进行了内存缓存处理，减少数据库的I/O处理，提高程序的运行性能，
        /// 若有数据修改过，重新启动一下程序就可以了，这些基础数据也不是天天修改来修改去的，
        /// 所以没必要过度担忧，当然有需要时也可以写个刷新缓存的程序
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>显示值</returns>
        public static string GetNameByCache(string id)
        {
            var result = string.Empty;

            var entity = GetEntityByCache(id);
            if (entity != null)
            {
                result = entity.Name;
            }

            return result;
        }
        #endregion

        /// <summary>
        /// 从缓存获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetCodeByCache(string id)
        {
            var result = string.Empty;

            var entity = GetEntityByCache(id);
            if (entity != null)
            {
                result = entity.Code;
            }

            return result;
        }

        /// <summary>
        /// 重新设置缓存（重新强制设置缓存）可以提供外部调用的
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>用户信息</returns>
        public static BaseOrganizationEntity SetCache(string id)
        {
            BaseOrganizationEntity result = null;

            var manager = new BaseOrganizationManager();
            result = manager.GetEntity(id);

            if (result != null)
            {
                SetCache(result);
            }

            return result;
        }

        /// <summary>
        /// 缓存预热,强制重新缓存
        /// </summary>
        /// <returns>影响行数</returns>
        public static int CachePreheating()
        {
            var result = 0;

            // 把所有的组织机构都缓存起来的代码
            var manager = new BaseOrganizationManager();
            var dataReader = manager.ExecuteReader();
            if (dataReader != null && !dataReader.IsClosed)
            {
                while (dataReader.Read())
                {
                    var entity = BaseEntity.Create<BaseOrganizationEntity>(dataReader, false);
                    if (entity != null)
                    {
                        SetCache(entity);
                        result++;
                        Console.WriteLine(result + " : " + entity.Name);
                    }
                }

                dataReader.Close();
            }

            return result;
        }
        /// <summary>
        /// 从缓存获取
        /// </summary>
        /// <param name="id"></param>
        /// <param name="refreshCache"></param>
        /// <returns></returns>
        public static BaseOrganizationEntity GetEntityByCache(string id, bool refreshCache = false)
        {
            BaseOrganizationEntity result = null;

            if (!string.IsNullOrEmpty(id))
            {
                var cacheKey = "O:";
                cacheKey += id;
                result = CacheUtil.Cache(cacheKey, () => new BaseOrganizationManager().GetEntity(id), true, refreshCache);
            }

            return result;
        }

        #region 删除缓存

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <returns></returns>
        public override bool RemoveCache()
        {
            var result = false;
            //var cacheKey = "Dt." + CurrentTableName;
            //if (UserInfo != null)
            //{
            //    cacheKey += "Dt." + UserInfo.SystemCode + ".ModuleTree";
            //}
            var cacheKey = "Dt.BaseOrganization";
            var cacheKeyTree = "Dt.BaseOrganizationTree";
            CacheUtil.Remove(cacheKeyTree);
            result = CacheUtil.Remove(cacheKey);
            return result;
        }
        #endregion
    }
}