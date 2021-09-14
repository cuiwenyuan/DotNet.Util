﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseOrganizeManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///		2016.09.20 版本：1.0 Troy.Cui	新增。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.09.20</date>
    /// </author> 
    /// </summary>
    public partial class BaseOrganizeManager : BaseManager
    {
        #region 高级查询
        /// <summary>
        /// 按条件分页高级查询(带记录状态Enabled和删除状态DeletionStateCode)
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="categoryCode"></param>
        /// <param name="companyId">查看公司主键</param>
        /// <param name="departmentId">查看部门主键</param>
        /// <param name="roleId"></param>
        /// <param name="roleIdExcluded"></param>
        /// <param name="moduleId"></param>
        /// <param name="moduleIdExcluded"></param>
        /// <param name="parentId"></param>
        /// <param name="searchKey">查询字段</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="showDisabled">是否显示无效记录</param>
        /// <param name="showDeleted">是否显示已删除记录</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string systemCode, string categoryCode, string companyId, string departmentId, string roleId, string roleIdExcluded, string moduleId, string moduleIdExcluded, string parentId, string searchKey, out int recordCount, int pageIndex = 0, int pageSize = 20, string sortExpression = "CreateOn", string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            pageIndex++;
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldEnabled + " = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldDeleted + " = 0");
            }
            //分类
            if (!string.IsNullOrEmpty(categoryCode))
            {
                sb.Append(" AND " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldCategoryCode + " = N'" + categoryCode + "'");
            }
            if (ValidateUtil.IsInt(companyId))
            {
                //只选择下一级
                //sb.Append(" AND " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldDepartmentId 
                //    + " IN ( SELECT " + BaseOrganizeEntity.FieldId 
                //    + " FROM " + BaseOrganizeEntity.TableName 
                //    + " WHERE " + BaseOrganizeEntity.FieldId + " = " + departmentId + " OR " + BaseOrganizeEntity.FieldParentId + " = " + departmentId + ")";

                //所有下级的都列出来
                var ids = GetChildrensId(BaseOrganizeEntity.FieldId, departmentId, BaseOrganizeEntity.FieldParentId);
                if (ids != null && ids.Length > 0)
                {
                    sb.Append(" AND (" + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldCompanyId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldId + " IN (" + StringUtil.ArrayToList(ids) + "))");
                }
                sb.Append(" AND (" + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldCompanyId + " = " + companyId + ")");
            }
            if (ValidateUtil.IsInt(departmentId))
            {
                sb.Append(" AND " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldId + " = " + departmentId);
            }

            //是否显示已隐藏记录
            //if (!showInvisible)
            //{
            //    sb.Append(" AND " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldIsVisible + "  = 1 ";
            //}
            //角色
            var tableNameRoleOrganize = UserInfo.SystemCode + "RoleOrganize";
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableNameRoleOrganize = systemCode + "RoleOrganize";
            }
            //指定角色
            if (!string.IsNullOrEmpty(roleId) && ValidateUtil.IsNumeric(roleId))
            {
                sb.Append(" AND ( " + BaseOrganizeEntity.FieldId + " IN ");
                sb.Append(" (SELECT DISTINCT " + BaseRoleOrganizeEntity.FieldOrganizeId);
                sb.Append(" FROM " + tableNameRoleOrganize);
                sb.Append(" WHERE " + BaseRoleOrganizeEntity.FieldRoleId + " = '" + roleId + "'");
                sb.Append(" AND " + BaseRoleOrganizeEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BaseRoleOrganizeEntity.FieldDeleted + " = 0)) ");
            }
            //排除指定角色
            if (!string.IsNullOrEmpty(roleIdExcluded) && ValidateUtil.IsNumeric(roleIdExcluded))
            {
                sb.Append(" AND ( " + BaseOrganizeEntity.FieldId + " NOT IN ");
                sb.Append(" (SELECT DISTINCT " + BaseRoleOrganizeEntity.FieldOrganizeId);
                sb.Append(" FROM " + tableNameRoleOrganize);
                sb.Append(" WHERE " + BaseRoleOrganizeEntity.FieldRoleId + " = '" + roleIdExcluded + "'");
                sb.Append(" AND " + BaseRoleOrganizeEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BaseRoleOrganizeEntity.FieldDeleted + " = 0)) ");
            }
            //用户菜单模块表
            var tableNamePermission = UserInfo.SystemCode + "Permission";
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableNamePermission = systemCode + "Permission";
            }
            //指定的菜单模块
            if (!string.IsNullOrEmpty(moduleId) && ValidateUtil.IsNumeric(moduleId))
            {
                sb.Append(" AND ( " + BaseOrganizeEntity.FieldId + " IN ");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldResourceId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldPermissionId + " = '" + moduleId + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + BaseOrganizeEntity.TableName + "' ");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0)) ");
            }
            //排除指定菜单模块
            if (!string.IsNullOrEmpty(moduleIdExcluded) && ValidateUtil.IsNumeric(moduleIdExcluded))
            {
                sb.Append(" AND ( " + BaseOrganizeEntity.FieldId + " NOT IN ");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldResourceId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldPermissionId + " = '" + moduleIdExcluded + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + BaseOrganizeEntity.TableName + "' ");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0)) ");
            }
            //父级编号
            if (!string.IsNullOrEmpty(parentId) && ValidateUtil.IsNumeric(parentId))
            {
                sb.Append(" AND ( ");
                //本级
                sb.Append(BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldId + "  = " + parentId);
                //下级
                sb.Append(" OR " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldParentId + "  = " + parentId);
                //下下级
                sb.Append(" OR " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldParentId + " IN ");
                sb.Append(" (SELECT " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldId + " FROM " + BaseOrganizeEntity.TableName + " WHERE " + BaseOrganizeEntity.TableName + "." + BaseModuleEntity.FieldParentId + "  = " + parentId + ") ");
                //下下下级
                sb.Append(" OR " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldParentId + " IN ");
                sb.Append(" (SELECT " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldId + " FROM " + BaseOrganizeEntity.TableName + " WHERE " + BaseOrganizeEntity.TableName + "." + BaseModuleEntity.FieldParentId + " IN ");
                sb.Append(" (SELECT " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldId + " FROM " + BaseOrganizeEntity.TableName + " WHERE " + BaseOrganizeEntity.TableName + "." + BaseModuleEntity.FieldParentId + " = " + parentId + ") ");
                sb.Append(" ) ");
                //下下下下级，做个组织机构实际应用应该足够了
                sb.Append(" OR " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldParentId + " IN ");
                sb.Append(" (SELECT " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldId + " FROM " + BaseOrganizeEntity.TableName + " WHERE " + BaseOrganizeEntity.TableName + "." + BaseModuleEntity.FieldParentId + " IN ");
                sb.Append(" (SELECT " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldId + " FROM " + BaseOrganizeEntity.TableName + " WHERE " + BaseOrganizeEntity.TableName + "." + BaseModuleEntity.FieldParentId + " IN ");
                sb.Append(" (SELECT " + BaseOrganizeEntity.TableName + "." + BaseOrganizeEntity.FieldId + " FROM " + BaseOrganizeEntity.TableName + " WHERE " + BaseOrganizeEntity.TableName + "." + BaseModuleEntity.FieldParentId + " = " + parentId + ") ");
                sb.Append(" ) ");
                sb.Append(" ) ");
                //闭合
                sb.Append(" ) ");
            }
            //关键词
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseOrganizeEntity.FieldFullName + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseOrganizeEntity.FieldCode + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseOrganizeEntity.FieldShortName + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseOrganizeEntity.FieldStandardName + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseOrganizeEntity.FieldDescription + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseOrganizeEntity.FieldQuickQuery + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseOrganizeEntity.FieldSimpleSpelling + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageIndex, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Put(), null, "*");
        }
        #endregion

        #region 重组TreeTable列表数据排序及转换

        /// <summary>
        /// 重组TreeTable列表数据排序及转换
        /// </summary>
        /// <param name="dtOld">旧数据表</param>
        /// <param name="parentId">父级编号</param>
        /// <returns>新数据表</returns>
        public DataTable GetTreeTable(DataTable dtOld, string parentId = null)
        {
            if (dtOld == null)
            {
                return null;
            }
            //创建一个新的DataTable增加一个深度字段LayerId
            var dt = new DataTable(BaseOrganizeEntity.TableName);
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("ParentId", typeof(int));
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("FullName", typeof(string));
            dt.Columns.Add("CategoryCode", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("Enabled", typeof(int));
            dt.Columns.Add("DeletionStateCode", typeof(int));
            dt.Columns.Add("CreateOn", typeof(DateTime));
            dt.Columns.Add("CreateBy", typeof(string));
            dt.Columns.Add("ModifiedOn", typeof(DateTime));
            dt.Columns.Add("ModifiedBy", typeof(string));
            dt.Columns.Add("LayerId", typeof(int));
            //调用迭代组合成datatable
            //默认从输入的开始
            if (string.IsNullOrEmpty(parentId))
            {
                GetChilds(dtOld, dt, null, 0);
            }
            else if (ValidateUtil.IsNumeric(parentId))
            {
                GetChilds(dtOld, dt, int.Parse(parentId), 0);
            }
            return dt;
        }
        /// <summary>
        /// 从内存中取得所有下级类别列表（自身迭代）
        /// </summary>
        /// <param name="dtOld">旧数据表</param>
        /// <param name="dtNew">新数据表</param>
        /// <param name="parentId">父级编号</param>
        /// <param name="layerId">层级</param>
        private void GetChilds(DataTable dtOld, DataTable dtNew, int? parentId, int layerId)
        {
            layerId++;
            DataRow[] dr = null;
            if (parentId == null)
            {
                dr = dtOld.Select("ParentId IS NULL", BaseOrganizeEntity.FieldSortCode);
            }
            else
            {
                dr = dtOld.Select("ParentId=" + parentId + "", BaseOrganizeEntity.FieldSortCode);
            }
            for (var i = 0; i < dr.Length; i++)
            {
                //添加一行数据
                var row = dtNew.NewRow();
                row["Id"] = int.Parse(dr[i]["Id"].ToString());
                if (string.IsNullOrEmpty(dr[i]["ParentId"].ToString()))
                {
                    row["ParentId"] = 0;
                }
                else
                {
                    row["ParentId"] = int.Parse(dr[i]["ParentId"].ToString());
                }
                row["Code"] = dr[i]["Code"].ToString();
                row["FullName"] = dr[i]["FullName"].ToString();
                row["CategoryCode"] = dr[i]["CategoryCode"].ToString();
                row["Description"] = dr[i]["Description"].ToString();
                row["Enabled"] = int.Parse(dr[i]["Enabled"].ToString());
                row["DeletionStateCode"] = int.Parse(dr[i]["DeletionStateCode"].ToString());
                if (!string.IsNullOrEmpty(dr[i]["CreateOn"].ToString()))
                {
                    row["CreateOn"] = DateTime.Parse(dr[i]["CreateOn"].ToString());
                }
                row["CreateBy"] = dr[i]["CreateBy"].ToString();
                if (!string.IsNullOrEmpty(dr[i]["ModifiedOn"].ToString()))
                {
                    row["ModifiedOn"] = DateTime.Parse(dr[i]["ModifiedOn"].ToString());
                }
                row["ModifiedBy"] = dr[i]["ModifiedBy"].ToString();
                row["LayerId"] = layerId;
                dtNew.Rows.Add(row);
                //循环5级就已经足够了
                if (layerId <= 5)
                {
                    //调用自身迭代
                    GetChilds(dtOld, dtNew, int.Parse(dr[i]["Id"].ToString()), layerId);
                }
            }
        }
        #endregion
    }
}