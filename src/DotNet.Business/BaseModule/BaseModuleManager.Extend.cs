//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseModuleManager 
    /// 菜单模块管理
    ///
    /// 修改记录
    /// 
    ///		2016.08.18 版本：1.0 Troy.Cui	新增。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.08.18</date>
    /// </author> 
    /// </summary>
    public partial class BaseModuleManager : BaseManager
    {
        #region 高级查询
        /// <summary>
        /// 按条件分页查询(带记录状态Enabled和删除状态DeletionStateCode)
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="categoryCode">分类编码</param>
        /// <param name="userId"></param>
        /// <param name="userIdExcluded"></param>
        /// <param name="roleId"></param>
        /// <param name="roleIdExcluded"></param>
        /// <param name="showInvisible"></param>
        /// <param name="isMenu"></param>
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
        public DataTable GetDataTableByPage(string systemCode, string categoryCode, string userId, string userIdExcluded, string roleId, string roleIdExcluded, bool showInvisible, string isMenu, string parentId, string searchKey, out int recordCount, int pageIndex = 0, int pageSize = 20, string sortExpression = "CreateOn", string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            //菜单模块表名
            var tableNameModule = BaseModuleEntity.TableName;
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableNameModule = systemCode + "Module";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(UserInfo.SystemCode))
                {
                    tableNameModule = UserInfo.SystemCode + "Module";
                }
            }
            pageIndex++;
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");

            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseModuleEntity.FieldEnabled + " = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseModuleEntity.FieldDeleted + " = 0");
            }
            //是否显示已隐藏记录
            if (!showInvisible)
            {
                sb.Append(" AND " + BaseModuleEntity.FieldIsVisible + " = 1");
            }
            //显示是否为菜单
            if (!string.IsNullOrEmpty(isMenu) && ValidateUtil.IsNumeric(isMenu))
            {
                sb.Append(" AND " + BaseModuleEntity.FieldIsMenu + "  = " + isMenu);
            }

            //菜单模块分类
            if (!string.IsNullOrEmpty(categoryCode))
            {
                sb.Append(" AND " + BaseModuleEntity.FieldCategoryCode + " = N'" + categoryCode + "'");
            }
            //用户菜单模块表
            var tableNamePermission = UserInfo.SystemCode + "Permission";
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableNamePermission = systemCode + "Permission";
            }
            //用户角色表
            var tableNameUserRole = UserInfo.SystemCode + "UserRole";
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableNameUserRole = systemCode + "UserRole";
            }
            //用于ResourceCategory的用户角色表
            var tableNameRole = UserInfo.SystemCode + "Role";
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableNameRole = systemCode + "Role";
            }
            //指定用户
            if (ValidateUtil.IsInt(userId))
            {
                //用户权限
                sb.Append(" AND ( " + BasePermissionEntity.FieldId + " IN (SELECT DISTINCT " + BasePermissionEntity.FieldPermissionId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldResourceId + " = " + userId);
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + BaseUserEntity.TableName + "' ");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1 ");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0) ");
                //角色权限,用OR
                sb.Append(" OR " + BasePermissionEntity.FieldId + " IN  ");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldPermissionId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldResourceId + " IN ");
                //用户所拥有的角色
                sb.Append(" (SELECT DISTINCT " + BaseUserRoleEntity.FieldRoleId);
                sb.Append(" FROM " + tableNameUserRole);
                sb.Append(" WHERE " + BaseUserRoleEntity.FieldUserId + " = " + userId);
                sb.Append(" AND " + BaseUserRoleEntity.FieldEnabled + " = 1 ");
                sb.Append(" AND " + BaseUserRoleEntity.FieldDeleted + " = 0) ");
                //角色权限
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameRole + "' ");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1 ");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0) ");
                sb.Append(" ) ");
            }
            //排除指定用户
            if (!string.IsNullOrEmpty(userIdExcluded) && ValidateUtil.IsNumeric(userIdExcluded))
            {
                //用户权限
                sb.Append(" AND ( " + BasePermissionEntity.FieldId + " NOT IN");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldPermissionId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldResourceId + " = " + userIdExcluded);
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + BaseUserEntity.TableName + "' ");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1 ");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0) ");
                //角色权限,用AND
                sb.Append(" AND " + BasePermissionEntity.FieldId + " NOT IN  ");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldPermissionId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldResourceId + " IN ");
                //用户所拥有的角色
                sb.Append(" (SELECT DISTINCT " + BaseUserRoleEntity.FieldRoleId);
                sb.Append(" FROM " + tableNameUserRole);
                sb.Append(" WHERE " + BaseUserRoleEntity.FieldUserId + " = " + userIdExcluded);
                sb.Append(" AND " + BaseUserRoleEntity.FieldEnabled + " = 1 ");
                sb.Append(" AND " + BaseUserRoleEntity.FieldDeleted + " = 0) ");
                //角色权限
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameRole + "' ");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1 ");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0) ");
                sb.Append(" ) ");

            }
            //指定角色
            if (!string.IsNullOrEmpty(roleId) && ValidateUtil.IsNumeric(roleId))
            {
                sb.Append(" AND ( " + BasePermissionEntity.FieldId + " IN ");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldPermissionId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldResourceId + " = " + roleId);
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameRole + "' ");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1 ");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0)) ");
            }
            //排除指定角色
            if (!string.IsNullOrEmpty(roleIdExcluded) && ValidateUtil.IsNumeric(roleIdExcluded))
            {
                sb.Append(" AND ( " + BasePermissionEntity.FieldId + " NOT IN ");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldPermissionId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldResourceId + " = " + roleIdExcluded);
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameRole + "' ");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1 ");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0)) ");
            }
            //父级编号
            if (!string.IsNullOrEmpty(parentId) && ValidateUtil.IsNumeric(parentId))
            {
                sb.Append(" AND ( ");
                //本级
                sb.Append(tableNameModule + "." + BaseModuleEntity.FieldId + "  = " + parentId);
                //下级
                sb.Append(" OR " + tableNameModule + "." + BaseModuleEntity.FieldParentId + "  = " + parentId);
                //下下级
                sb.Append(" OR " + tableNameModule + "." + BaseModuleEntity.FieldParentId + " IN ");
                sb.Append(" (SELECT " + tableNameModule + "." + BaseModuleEntity.FieldId + " FROM " + tableNameModule + " WHERE " + tableNameModule + "." + BaseModuleEntity.FieldParentId + "  = " + parentId + ") ");
                //下下下级，做个菜单模块实际应用应该足够了
                sb.Append(" OR " + tableNameModule + "." + BaseModuleEntity.FieldParentId + " IN ");
                sb.Append(" (SELECT " + tableNameModule + "." + BaseModuleEntity.FieldId + " FROM " + tableNameModule + " WHERE " + tableNameModule + "." + BaseModuleEntity.FieldParentId + " IN ");
                sb.Append(" (SELECT " + tableNameModule + "." + BaseModuleEntity.FieldId + " FROM " + tableNameModule + " WHERE " + tableNameModule + "." + BaseModuleEntity.FieldParentId + " = " + parentId + ") ");
                sb.Append(" ) ");
                //闭合
                sb.Append(" ) ");
            }
            //关键词
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseModuleEntity.FieldFullName + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseModuleEntity.FieldCode + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseModuleEntity.FieldDescription + " LIKE N'%" + searchKey + "%')");
            }

            sb.Replace(" 1 = 1 AND ", "");
            //重新构造viewName
            var viewName = string.Empty;
            //指定用户，就读取相应的Permission授权日期
            if (ValidateUtil.IsInt(userId))
            {
                viewName = "SELECT DISTINCT " + tableNameModule + "." + BaseModuleEntity.FieldId;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldParentId;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldCode;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldFullName;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldCategoryCode;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldImageUrl;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldImageIndex;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldSelectedImageIndex;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldNavigateUrl;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldTarget;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldFormName;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldAssemblyName;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldSortCode;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldEnabled;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldDeleted;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldIsMenu;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldIsPublic;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldExpand;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldIsScope;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldIsVisible;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldLastCall;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldAllowEdit;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldAllowDelete;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldDescription;
                //授权日期
                viewName += "," + tableNamePermission + "." + BasePermissionEntity.FieldCreateTime;
                viewName += "," + tableNamePermission + "." + BasePermissionEntity.FieldCreateUserId;
                viewName += "," + tableNamePermission + "." + BasePermissionEntity.FieldCreateBy;
                viewName += "," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateTime;
                viewName += "," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateUserId;
                viewName += "," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateBy;
                viewName += " FROM " + tableNameModule + " INNER JOIN " + tableNamePermission;
                viewName += " ON " + tableNameModule + "." + BaseModuleEntity.FieldId + " = " + tableNamePermission + "." + BasePermissionEntity.FieldPermissionId;
                //BaseUser
                viewName += " WHERE ((" + tableNamePermission + "." + BasePermissionEntity.FieldResourceCategory + " = '" + BaseUserEntity.TableName + "')";
                viewName += " AND (" + tableNamePermission + "." + BasePermissionEntity.FieldResourceId + " = " + userId + "))";
                //UserRole
                viewName += " OR ((" + tableNamePermission + "." + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameRole + "')";
                viewName += " AND (" + tableNamePermission + "." + BasePermissionEntity.FieldResourceId + " IN ";
                //用户所拥有的角色
                viewName += " (SELECT DISTINCT " + BaseUserRoleEntity.FieldRoleId;
                viewName += " FROM " + tableNameUserRole;
                viewName += " WHERE " + BaseUserRoleEntity.FieldUserId + " = " + userId;
                viewName += " AND " + BaseUserRoleEntity.FieldEnabled + " = 1 ";
                viewName += " AND " + BaseUserRoleEntity.FieldDeleted + " = 0) ";
                viewName += "))";

            }
            //指定菜单模块，就读取相应的Permission授权日期
            else if (!string.IsNullOrEmpty(roleId) && ValidateUtil.IsNumeric(roleId))
            {
                viewName = "SELECT DISTINCT " + tableNameModule + "." + BaseModuleEntity.FieldId;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldParentId;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldCode;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldFullName;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldCategoryCode;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldImageUrl;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldImageIndex;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldSelectedImageIndex;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldNavigateUrl;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldTarget;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldFormName;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldAssemblyName;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldSortCode;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldEnabled;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldDeleted;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldIsMenu;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldIsPublic;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldExpand;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldIsScope;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldIsVisible;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldLastCall;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldAllowEdit;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldAllowDelete;
                viewName += "," + tableNameModule + "." + BaseModuleEntity.FieldDescription;
                //授权日期
                viewName += "," + tableNamePermission + "." + BasePermissionEntity.FieldCreateTime;
                viewName += "," + tableNamePermission + "." + BasePermissionEntity.FieldCreateUserId;
                viewName += "," + tableNamePermission + "." + BasePermissionEntity.FieldCreateBy;
                viewName += "," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateTime;
                viewName += "," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateUserId;
                viewName += "," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateBy;
                viewName += " FROM " + tableNameModule + " INNER JOIN " + tableNamePermission;
                viewName += " ON " + tableNameModule + "." + BaseModuleEntity.FieldId + " = " + tableNamePermission + "." + BasePermissionEntity.FieldPermissionId;
                viewName += " WHERE (" + tableNamePermission + "." + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameRole + "')";
                viewName += " AND (" + tableNamePermission + "." + BasePermissionEntity.FieldResourceId + " = " + roleId + ")";
            }
            //从视图读取
            if (!string.IsNullOrEmpty(viewName))
            {
                tableNameModule = viewName;
            }

            return GetDataTableByPage(out recordCount, pageIndex, pageSize, sortExpression, sortDirection, tableNameModule, sb.Put(), null, "*");
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
            var dt = new DataTable(BaseModuleEntity.TableName);
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("ParentId", typeof(int));
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("FullName", typeof(string));
            dt.Columns.Add("CategoryCode", typeof(string));
            dt.Columns.Add("Enabled", typeof(int));
            dt.Columns.Add("DeletionStateCode", typeof(int));
            dt.Columns.Add("IsMenu", typeof(int));
            dt.Columns.Add("IsPublic", typeof(int));
            dt.Columns.Add("Expand", typeof(int));
            dt.Columns.Add("CreateOn", typeof(DateTime));
            dt.Columns.Add("CreateBy", typeof(string));
            dt.Columns.Add("ModifiedOn", typeof(DateTime));
            dt.Columns.Add("ModifiedBy", typeof(string));
            dt.Columns.Add("LayerId", typeof(int));
            //调用迭代组合成datatable
            //默认从输入的开始
            if (string.IsNullOrEmpty(parentId))
            {
                GetChildren(dtOld, dt, null, 0);
            }
            else if (ValidateUtil.IsNumeric(parentId))
            {
                GetChildren(dtOld, dt, int.Parse(parentId), 0);
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
        private void GetChildren(DataTable dtOld, DataTable dtNew, int? parentId, int layerId)
        {
            layerId++;
            DataRow[] dr = null;
            if (parentId == null)
            {
                dr = dtOld.Select("ParentId IS NULL", BaseModuleEntity.FieldSortCode);
            }
            else
            {
                dr = dtOld.Select("ParentId=" + parentId + "", BaseModuleEntity.FieldSortCode);
            }
            foreach (var t in dr)
            {
                //添加一行数据
                var row = dtNew.NewRow();
                row["Id"] = int.Parse(t["Id"].ToString());
                if (string.IsNullOrEmpty(t["ParentId"].ToString()))
                {
                    row["ParentId"] = 0;
                }
                else
                {
                    row["ParentId"] = int.Parse(t["ParentId"].ToString());
                }
                row["Code"] = t["Code"].ToString();
                row["FullName"] = t["FullName"].ToString();
                row["CategoryCode"] = t["CategoryCode"].ToString();
                row["Enabled"] = int.Parse(t["Enabled"].ToString());
                row["DeletionStateCode"] = int.Parse(t["DeletionStateCode"].ToString());
                row["IsMenu"] = int.Parse(t["IsMenu"].ToString());
                row["IsPublic"] = int.Parse(t["IsPublic"].ToString());
                row["Expand"] = int.Parse(t["Expand"].ToString());
                if (!string.IsNullOrEmpty(t["CreateOn"].ToString()))
                {
                    row["CreateOn"] = DateTime.Parse(t["CreateOn"].ToString());
                }
                row["CreateBy"] = t["CreateBy"].ToString();
                if (!string.IsNullOrEmpty(t["ModifiedOn"].ToString()))
                {
                    row["ModifiedOn"] = DateTime.Parse(t["ModifiedOn"].ToString());
                }
                row["ModifiedBy"] = t["ModifiedBy"].ToString();
                row["LayerId"] = layerId;
                dtNew.Rows.Add(row);
                //循环10级就已经足够了
                if (layerId <= 10)
                {
                    //调用自身迭代
                    GetChildren(dtOld, dtNew, int.Parse(t["Id"].ToString()), layerId);
                }
            }
        }
        #endregion

        #region 树形下拉菜单

        /// <summary>
        /// 菜单模块表
        /// </summary>
        private DataTable _dtModule = null;

        /// <summary>
        ///  菜单模块名称前缀
        /// </summary>
        private string _head = "|";

        /// <summary>
        /// 菜单模块树形表
        /// </summary>
        private DataTable _moduleTable = new DataTable(BaseModuleEntity.TableName);

        #region public DataTable GetModuleTree(DataTable dtModule = null) 绑定下拉筐数据,菜单模块树表
        /// <summary>
        /// 绑定下拉筐数据,组织机构树表
        /// </summary>
        /// <param name="dtModule">组织机构</param>
        /// <returns>组织机构树表</returns>
        public DataTable GetModuleTree(DataTable dtModule = null)
        {
            if (dtModule != null)
            {
                _dtModule = dtModule;
            }
            else
            {
                //2017.12.20增加默认的HttpRuntime.Cache缓存
                var cacheKey = "DataTable.BaseModule";
                //var cacheTime = default(TimeSpan);
                var cacheTime = TimeSpan.FromMilliseconds(86400000);
                _dtModule = CacheUtil.Cache<DataTable>(cacheKey, () =>
                {
                    // 获取所有数据
                    var parameters = new List<KeyValuePair<string, object>>
                    {
                        //菜单
                        //parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldIsMenu, 1));
                        new KeyValuePair<string, object>(BaseModuleEntity.FieldEnabled, 1),
                        new KeyValuePair<string, object>(BaseModuleEntity.FieldDeleted, 0)
                    };
                    //默认读取BaseModule的菜单
                    return GetDataTable(parameters, BaseModuleEntity.FieldSortCode);
                }, true, false, cacheTime);

                //// 获取所有数据
                //List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
                ////parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldIsMenu, 1));
                //parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldEnabled, 1));
                //parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldDeleted, 0));
                ////默认读取BaseModule的菜单
                //_dtModule = this.GetDataTable(parameters, BaseModuleEntity.FieldSortCode);
            }
            // 初始化表
            if (_moduleTable.Columns.Count == 0)
            {
                //建立表的列，不能重复建立
                _moduleTable.Columns.Add(new DataColumn(BaseModuleEntity.FieldId, Type.GetType("System.Int32")));
                _moduleTable.Columns.Add(new DataColumn(BaseModuleEntity.FieldFullName, Type.GetType("System.String")));
            }

            for (var i = 0; i < _dtModule.Rows.Count; i++)
            {
                //null为顶级
                if (string.IsNullOrEmpty(_dtModule.Rows[i][BaseModuleEntity.FieldParentId].ToString()))
                {
                    var dr = _moduleTable.NewRow();
                    dr[BaseModuleEntity.FieldId] = _dtModule.Rows[i][BaseModuleEntity.FieldId];
                    dr[BaseModuleEntity.FieldFullName] = _dtModule.Rows[i][BaseModuleEntity.FieldFullName];
                    _moduleTable.Rows.Add(dr);
                    //查找子级
                    GetModule(_dtModule.Rows[i][BaseModuleEntity.FieldId]);
                }
            }
            return _moduleTable;
        }
        #endregion

        #region public void GetModule(object parentId)
        /// <summary>
        /// 获取子菜单模块
        /// </summary>
        /// <param name="parentId">父节点主键</param>
        private void GetModule(object parentId)
        {
            _head += "--";
            for (var i = 0; i < _dtModule.Rows.Count; i++)
            {
                if (_dtModule.Rows[i][BaseModuleEntity.FieldParentId].Equals(parentId))
                {
                    var dr = _moduleTable.NewRow();
                    dr[BaseModuleEntity.FieldId] = _dtModule.Rows[i][BaseModuleEntity.FieldId];
                    dr[BaseModuleEntity.FieldFullName] = _head + _dtModule.Rows[i][BaseModuleEntity.FieldFullName];
                    _moduleTable.Rows.Add(dr);
                    GetModule(_dtModule.Rows[i][BaseModuleEntity.FieldId]);
                }
            }
            // 子级遍历完成后，退到父级
            _head = _head.Substring(0, _head.Length - 2);
        }
        #endregion

        #endregion

    }
}
