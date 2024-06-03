//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using System.Linq;
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
        #region UniqueAdd
        /// <summary>
        /// 检查唯一值式新增
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        public string UniqueAdd(BaseModuleEntity entity, out Status status)
        {
            var result = string.Empty;
            //检查是否重复
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseModuleEntity.FieldSystemCode, entity.SystemCode),
                new KeyValuePair<string, object>(BaseModuleEntity.FieldParentId, entity.ParentId),
                new KeyValuePair<string, object>(BaseModuleEntity.FieldCode, entity.Code),
                new KeyValuePair<string, object>(BaseModuleEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseModuleEntity.FieldDeleted, 0)
            };

            if (!IsUnique(parameters, entity.Id.ToString()))
            {
                //名称已重复
                Status = Status.ErrorNameExist;
                StatusCode = Status.ErrorNameExist.ToString();
                StatusMessage = Status.ErrorNameExist.ToDescription();
            }
            else
            {
                result = AddEntity(entity);
                if (!string.IsNullOrEmpty(result))
                {
                    Status = Status.OkAdd;
                    StatusCode = Status.OkAdd.ToString();
                    StatusMessage = Status.OkAdd.ToDescription();
                }
                else
                {
                    Status = Status.Error;
                    StatusCode = Status.Error.ToString();
                    StatusMessage = Status.Error.ToDescription();
                }
            }
            status = Status;
            return result;
        }

        #endregion

        #region public int UniqueUpdate(BaseModuleEntity entity, out Status status) 更新
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        /// <returns>返回</returns>
        public int UniqueUpdate(BaseModuleEntity entity, out Status status)
        {
            var result = 0;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseModuleEntity.FieldSystemCode, entity.SystemCode),
                new KeyValuePair<string, object>(BaseModuleEntity.FieldParentId, entity.ParentId),
                new KeyValuePair<string, object>(BaseModuleEntity.FieldCode, entity.Code),
                new KeyValuePair<string, object>(BaseModuleEntity.FieldName, entity.Name),
                new KeyValuePair<string, object>(BaseModuleEntity.FieldDeleted, 0)
            };

            // 检查编号是否重复
            if ((entity.Code.Length > 0) && (Exists(parameters, entity.Id)))
            {
                // 编号已重复
                Status = Status.ErrorCodeExist;
                StatusCode = Status.ErrorCodeExist.ToString();
                StatusMessage = Status.ErrorCodeExist.ToDescription();
            }
            else
            {
                // 获取原始实体信息
                var entityOld = GetEntity(entity.Id);
                if (entityOld != null)
                {
                    // 保存修改记录，无论是否允许
                    SaveEntityChangeLog(entity, entityOld);
                    // 2015-07-14 吉日嘎拉 只有允许修改的，才可以修改，不允许修改的，不让修改，但是把修改记录会保存起来的。
                    if (entityOld.AllowEdit == 1)
                    {
                        result = UpdateEntity(entity);
                        if (result == 1)
                        {
                            Status = Status.OkUpdate;
                            StatusCode = Status.OkUpdate.ToString();
                            StatusMessage = Status.OkUpdate.ToDescription();
                        }
                        else
                        {
                            Status = Status.ErrorDeleted;
                            StatusCode = Status.ErrorDeleted.ToString();
                            StatusMessage = Status.ErrorDeleted.ToDescription();
                        }
                    }
                    else
                    {
                        Status = Status.NotAllowEdit;
                        StatusCode = Status.NotAllowEdit.ToString();
                        StatusMessage = Status.NotAllowEdit.ToDescription();
                    }
                }
            }
            status = Status;
            return result;
        }
        #endregion

        #region SaveEntityChangeLog
        /// <summary>
        /// 保存实体修改记录
        /// </summary>
        /// <param name="entityNew">修改后的实体对象</param>
        /// <param name="entityOld">修改前的实体对象</param>
        /// <param name="tableName">表名称</param>
        public void SaveEntityChangeLog(BaseModuleEntity entityNew, BaseModuleEntity entityOld, string tableName = null)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                //统一放在一个公共表 Troy.Cui 2016-08-17
                tableName = BaseChangeLogEntity.CurrentTableName;
            }
            var manager = new BaseChangeLogManager(UserInfo, tableName);
            foreach (var property in typeof(BaseModuleEntity).GetProperties())
            {
                var oldValue = Convert.ToString(property.GetValue(entityOld, null));
                var newValue = Convert.ToString(property.GetValue(entityNew, null));
                var fieldDescription = property.GetCustomAttributes(typeof(FieldDescription), false).FirstOrDefault() as FieldDescription;
                //不记录创建人、修改人、没有修改的记录
                if (!fieldDescription.NeedLog || oldValue == newValue)
                {
                    continue;
                }
                var entity = new BaseChangeLogEntity
                {
                    TableName = CurrentTableName,
                    TableDescription = typeof(BaseModuleEntity).FieldDescription("CurrentTableName"),
                    ColumnName = property.Name,
                    ColumnDescription = fieldDescription.Text,
                    NewValue = newValue,
                    OldValue = oldValue,
                    RecordKey = entityOld.Id.ToString()
                };
                manager.Add(entity, true, false);
            }
        }
        #endregion

        #region 高级查询
        /// <summary>
        /// 按条件分页查询(带记录状态Enabled和删除状态Deleted)
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="categoryCode">分类编码</param>
        /// <param name="userId">用户编码</param>
        /// <param name="userIdExcluded">排除用户编码</param>
        /// <param name="roleId">角色编码</param>
        /// <param name="roleIdExcluded">排除角色编码</param>
        /// <param name="showInvisible">显示隐藏</param>
        /// <param name="isMenu">是否菜单（1/0）</param>
        /// <param name="parentId">父节点编号</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="searchKey">查询关键字</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="showDisabled">是否显示无效记录</param>
        /// <param name="showDeleted">是否显示已删除记录</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string systemCode, string categoryCode, string userId, string userIdExcluded, string roleId, string roleIdExcluded, bool showInvisible, string isMenu, string parentId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = "CreateTime", string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            //菜单模块表名
            var tableNameModule = BaseModuleEntity.CurrentTableName;
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
            var sb = PoolUtil.StringBuilder.Get().Append(" 1 = 1");

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

            //创建时间
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseModuleEntity.FieldCreateTime + " >= " + dbHelper.ToDbTime(startTime));
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseModuleEntity.FieldCreateTime + " <= " + dbHelper.ToDbTime(endTime.ToDateTime().Date.AddDays(1).AddMilliseconds(-1)));
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
            //用户角色
            var tableNameUserRole = UserInfo.SystemCode + "UserRole";
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableNameUserRole = systemCode + "UserRole";
            }
            //用于ResourceCategory的用户角色
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
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + BaseUserEntity.CurrentTableName + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldSystemCode + " = '" + systemCode + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0)");
                //角色权限,用OR
                sb.Append(" OR " + BasePermissionEntity.FieldId + " IN");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldPermissionId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldResourceId + " IN");
                //用户所拥有的角色
                sb.Append(" (SELECT DISTINCT " + BaseUserRoleEntity.FieldRoleId);
                sb.Append(" FROM " + tableNameUserRole);
                sb.Append(" WHERE " + BaseUserRoleEntity.FieldUserId + " = " + userId);
                sb.Append(" AND " + BaseUserRoleEntity.FieldSystemCode + " = '" + systemCode + "'");
                sb.Append(" AND " + BaseUserRoleEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BaseUserRoleEntity.FieldDeleted + " = 0)");
                //角色权限
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameRole + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldSystemCode + " = '" + systemCode + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0)");
                sb.Append(" )");
            }
            //排除指定用户
            if (ValidateUtil.IsNumeric(userIdExcluded))
            {
                //用户权限
                sb.Append(" AND ( " + BasePermissionEntity.FieldId + " NOT IN");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldPermissionId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldResourceId + " = " + userIdExcluded);
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + BaseUserEntity.CurrentTableName + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldSystemCode + " = '" + systemCode + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0)");
                //角色权限,用AND
                sb.Append(" AND " + BasePermissionEntity.FieldId + " NOT IN");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldPermissionId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldResourceId + " IN");
                //用户所拥有的角色
                sb.Append(" (SELECT DISTINCT " + BaseUserRoleEntity.FieldRoleId);
                sb.Append(" FROM " + tableNameUserRole);
                sb.Append(" WHERE " + BaseUserRoleEntity.FieldUserId + " = " + userIdExcluded);
                sb.Append(" AND " + BaseUserRoleEntity.FieldSystemCode + " = '" + systemCode + "'");
                sb.Append(" AND " + BaseUserRoleEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BaseUserRoleEntity.FieldDeleted + " = 0)");
                //角色权限
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameRole + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldSystemCode + " = '" + systemCode + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0)");
                sb.Append(" )");

            }
            //指定角色
            if (ValidateUtil.IsInt(roleId))
            {
                sb.Append(" AND ( " + BasePermissionEntity.FieldId + " IN");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldPermissionId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldResourceId + " = " + roleId);
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameRole + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldSystemCode + " = '" + systemCode + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0))");
            }
            //排除指定角色
            if (ValidateUtil.IsInt(roleIdExcluded))
            {
                sb.Append(" AND ( " + BasePermissionEntity.FieldId + " NOT IN");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldPermissionId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldResourceId + " = " + roleIdExcluded);
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameRole + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldSystemCode + " = '" + systemCode + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0))");
            }
            //父级编号
            if (ValidateUtil.IsInt(parentId))
            {
                sb.Append(" AND ( ");
                //本级
                sb.Append(BaseModuleEntity.FieldId + "  = " + parentId);
                //下级
                sb.Append(" OR " + BaseModuleEntity.FieldParentId + "  = " + parentId);
                //下下级
                sb.Append(" OR " + BaseModuleEntity.FieldParentId + " IN ");
                sb.Append(" (SELECT " + tableNameModule + "." + BaseModuleEntity.FieldId + " FROM " + tableNameModule + " WHERE " + tableNameModule + "." + BaseModuleEntity.FieldDeleted + "  = 0 AND " + tableNameModule + "." + BaseModuleEntity.FieldEnabled + "  = 1 AND " + tableNameModule + "." + BaseModuleEntity.FieldSystemCode + "  = '" + systemCode + "' AND " + tableNameModule + "." + BaseModuleEntity.FieldParentId + "  = " + parentId + ") ");
                //下下下级，做个菜单模块实际应用应该足够了
                sb.Append(" OR " + BaseModuleEntity.FieldParentId + " IN ");
                sb.Append(" (SELECT " + tableNameModule + "." + BaseModuleEntity.FieldId + " FROM " + tableNameModule + " WHERE " + tableNameModule + "." + BaseModuleEntity.FieldDeleted + "  = 0 AND " + tableNameModule + "." + BaseModuleEntity.FieldEnabled + "  = 1 AND " + tableNameModule + "." + BaseModuleEntity.FieldSystemCode + "  = '" + systemCode + "' AND " + tableNameModule + "." + BaseModuleEntity.FieldParentId + " IN ");
                sb.Append(" (SELECT " + tableNameModule + "." + BaseModuleEntity.FieldId + " FROM " + tableNameModule + " WHERE " + tableNameModule + "." + BaseModuleEntity.FieldDeleted + "  = 0 AND " + tableNameModule + "." + BaseModuleEntity.FieldEnabled + "  = 1 AND " + tableNameModule + "." + BaseModuleEntity.FieldSystemCode + "  = '" + systemCode + "' AND " + tableNameModule + "." + BaseModuleEntity.FieldParentId + " = " + parentId + ") ");
                sb.Append(" ) ");
                //闭合
                sb.Append(" ) ");
            }
            //关键词
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseModuleEntity.FieldName + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseModuleEntity.FieldCode + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseModuleEntity.FieldDescription + " LIKE N'%" + searchKey + "%')");
            }

            sb.Replace(" 1 = 1 AND ", "");
            //重新构造viewName
            var sbView = PoolUtil.StringBuilder.Get();
            //指定用户，就读取相应的Permission授权日期
            if (ValidateUtil.IsInt(userId))
            {
                sbView.Clear();
                sbView.Append("SELECT DISTINCT " + tableNameModule + "." + BaseModuleEntity.FieldId);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldParentId);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldCode);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldName);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldCategoryCode);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldImageUrl);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldImageIndex);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldSelectedImageIndex);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldNavigateUrl);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldTarget);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldFormName);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldAssemblyName);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldSortCode);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldEnabled);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldDeleted);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldIsMenu);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldIsPublic);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldIsExpand);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldIsScope);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldIsVisible);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldLastCall);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldAllowEdit);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldAllowDelete);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldDescription);
                //授权日期
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldCreateTime);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldCreateUserId);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldCreateBy);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateTime);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateUserId);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateBy);
                sbView.Append(" FROM " + tableNameModule + " INNER JOIN " + tableNamePermission);
                sbView.Append(" ON " + tableNameModule + "." + BaseModuleEntity.FieldId + " = " + tableNamePermission + "." + BasePermissionEntity.FieldPermissionId);
                sbView.Append(" AND " + tableNameModule + "." + BaseModuleEntity.FieldSystemCode + " = " + tableNamePermission + "." + BasePermissionEntity.FieldSystemCode);
                //BaseUser
                sbView.Append(" WHERE ((" + tableNamePermission + "." + BasePermissionEntity.FieldResourceCategory + " = '" + BaseUserEntity.CurrentTableName + "')");
                sbView.Append(" AND (" + tableNamePermission + "." + BasePermissionEntity.FieldResourceId + " = " + userId + "))");
                //UserRole
                sbView.Append(" OR ((" + tableNamePermission + "." + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameRole + "')");
                sbView.Append(" AND (" + tableNamePermission + "." + BasePermissionEntity.FieldResourceId + " IN ");
                //用户所拥有的角色
                sbView.Append(" (SELECT DISTINCT " + BaseUserRoleEntity.FieldRoleId);
                sbView.Append(" FROM " + tableNameUserRole);
                sbView.Append(" WHERE " + BaseUserRoleEntity.FieldUserId + " = " + userId);
                sbView.Append(" AND " + BaseUserRoleEntity.FieldSystemCode + " = '" + systemCode + "'");
                sbView.Append(" AND " + BaseUserRoleEntity.FieldEnabled + " = 1");
                sbView.Append(" AND " + BaseUserRoleEntity.FieldDeleted + " = 0)");
                sbView.Append(" ))");

            }
            //指定菜单模块，就读取相应的Permission授权日期
            else if (ValidateUtil.IsInt(roleId))
            {
                sbView.Clear();
                sbView.Append("SELECT DISTINCT " + tableNameModule + "." + BaseModuleEntity.FieldId);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldParentId);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldCode);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldName);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldCategoryCode);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldImageUrl);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldImageIndex);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldSelectedImageIndex);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldNavigateUrl);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldTarget);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldFormName);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldAssemblyName);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldSortCode);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldEnabled);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldDeleted);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldIsMenu);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldIsPublic);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldIsExpand);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldIsScope);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldIsVisible);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldLastCall);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldAllowEdit);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldAllowDelete);
                sbView.Append("," + tableNameModule + "." + BaseModuleEntity.FieldDescription);
                //授权日期
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldCreateTime);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldCreateUserId);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldCreateBy);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateTime);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateUserId);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateBy);
                sbView.Append(" FROM " + tableNameModule + " INNER JOIN " + tableNamePermission);
                sbView.Append(" ON " + tableNameModule + "." + BaseModuleEntity.FieldId + " = " + tableNamePermission + "." + BasePermissionEntity.FieldPermissionId);
                sbView.Append(" AND " + tableNameModule + "." + BaseModuleEntity.FieldSystemCode + " = " + tableNamePermission + "." + BasePermissionEntity.FieldSystemCode);
                sbView.Append(" WHERE " + tableNamePermission + "." + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameRole + "'");
                sbView.Append(" AND " + tableNamePermission + "." + BasePermissionEntity.FieldSystemCode + " = '" + systemCode + "'");
                sbView.Append(" AND " + tableNamePermission + "." + BasePermissionEntity.FieldResourceId + " = " + roleId + "");
            }
            //从视图读取
            if (sbView.Length > 0)
            {
                tableNameModule = sbView.Return();
            }

            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, tableNameModule, sb.Return());
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
            var dt = new DataTable(BaseModuleEntity.CurrentTableName);
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("ParentId", typeof(int));
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("CategoryCode", typeof(string));
            dt.Columns.Add("Enabled", typeof(int));
            dt.Columns.Add("Deleted", typeof(int));
            dt.Columns.Add("IsMenu", typeof(int));
            dt.Columns.Add("IsPublic", typeof(int));
            dt.Columns.Add("IsExpand", typeof(int));
            dt.Columns.Add("SortCode", typeof(int));
            dt.Columns.Add("CreateTime", typeof(DateTime));
            dt.Columns.Add("CreateBy", typeof(string));
            dt.Columns.Add("UpdateTime", typeof(DateTime));
            dt.Columns.Add("UpdateBy", typeof(string));
            dt.Columns.Add("LayerId", typeof(int));
            //调用迭代组合成datatable
            //默认从输入的开始
            if (string.IsNullOrEmpty(parentId))
            {
                GetChildren(dtOld, dt, null, 0);
            }
            else if (ValidateUtil.IsNumeric(parentId))
            {
                GetChildren(dtOld, dt, parentId.ToInt(), 0);
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
                dr = dtOld.Select("ParentId = 0", BaseModuleEntity.FieldSortCode);
            }
            else
            {
                dr = dtOld.Select("ParentId = " + parentId + "", BaseModuleEntity.FieldSortCode);
            }
            foreach (var t in dr)
            {
                //添加一行数据
                var row = dtNew.NewRow();
                row["Id"] = t["Id"].ToInt();
                if (string.IsNullOrEmpty(t["ParentId"].ToString()))
                {
                    row["ParentId"] = 0;
                }
                else
                {
                    row["ParentId"] = t["ParentId"].ToInt();
                }
                row["Code"] = t["Code"].ToString();
                row["Name"] = t["Name"].ToString();
                row["CategoryCode"] = t["CategoryCode"].ToString();
                row["Enabled"] = t["Enabled"].ToInt();
                row["Deleted"] = t["Deleted"].ToInt();
                row["IsMenu"] = t["IsMenu"].ToInt();
                row["IsPublic"] = t["IsPublic"].ToInt();
                row["IsExpand"] = t["IsExpand"].ToInt();
                row["SortCode"] = t["SortCode"].ToInt();
                if (!string.IsNullOrEmpty(t["CreateTime"].ToString()))
                {
                    row["CreateTime"] = DateTime.Parse(t["CreateTime"].ToString());
                }
                row["CreateBy"] = t["CreateBy"].ToString();
                if (!string.IsNullOrEmpty(t["UpdateTime"].ToString()))
                {
                    row["UpdateTime"] = DateTime.Parse(t["UpdateTime"].ToString());
                }
                row["UpdateBy"] = t["UpdateBy"].ToString();
                row["LayerId"] = layerId;
                dtNew.Rows.Add(row);
                //循环10级就已经足够了
                if (layerId <= 10)
                {
                    //调用自身迭代
                    GetChildren(dtOld, dtNew, t["Id"].ToInt(), layerId);
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
        private DataTable _moduleTable = new DataTable(BaseModuleEntity.CurrentTableName);

        #region public DataTable GetModuleTree(DataTable dtModule = null) 绑定下拉框数据,菜单模块树表
        /// <summary>
        /// 绑定下拉框数据,组织机构树表
        /// </summary>
        /// <param name="dt">组织机构表</param>
        /// <returns>组织机构树表</returns>
        public DataTable GetModuleTree(DataTable dt)
        {
            _dtModule = dt;
            // 初始化表
            if (_moduleTable.Columns.Count == 0)
            {
                //建立表的列，不能重复建立
                _moduleTable.Columns.Add(new DataColumn(BaseModuleEntity.FieldId, Type.GetType("System.Int32")));
                _moduleTable.Columns.Add(new DataColumn(BaseModuleEntity.FieldName, Type.GetType("System.String")));
            }

            for (var i = 0; i < _dtModule.Rows.Count; i++)
            {
                //null或者0为顶级
                if (_dtModule.Rows[i][BaseModuleEntity.FieldParentId].ToInt() == 0)
                {
                    var dr = _moduleTable.NewRow();
                    dr[BaseModuleEntity.FieldId] = _dtModule.Rows[i][BaseModuleEntity.FieldId];
                    dr[BaseModuleEntity.FieldName] = _dtModule.Rows[i][BaseModuleEntity.FieldName];
                    _moduleTable.Rows.Add(dr);
                    //递归查找子级
                    GetModule(_dtModule.Rows[i][BaseModuleEntity.FieldId].ToInt());
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
        private void GetModule(int parentId)
        {
            _head += "--";
            for (var i = 0; i < _dtModule.Rows.Count; i++)
            {
                if (_dtModule.Rows[i][BaseModuleEntity.FieldParentId].ToInt() == parentId)
                {
                    var dr = _moduleTable.NewRow();
                    dr[BaseModuleEntity.FieldId] = _dtModule.Rows[i][BaseModuleEntity.FieldId];
                    dr[BaseModuleEntity.FieldName] = _head + _dtModule.Rows[i][BaseModuleEntity.FieldName];
                    _moduleTable.Rows.Add(dr);
                    GetModule(_dtModule.Rows[i][BaseModuleEntity.FieldId].ToInt());
                }
            }
            // 子级遍历完成后，退到父级
            _head = _head.Substring(0, _head.Length - 2);
        }
        #endregion

        #endregion

        #region GetModuleTree
        /// <summary>
        /// 获取菜单模块树型列表
        /// </summary>
        /// <param name="systemCode">子系统</param>
        /// <param name="isMenu">是否菜单(0/1)</param>
        public DataTable GetModuleTree(string systemCode, string isMenu = null)
        {
            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = "Base";
            }
            //读取选定子系统的菜单模块
            var manager = new BaseModuleManager(UserInfo, systemCode + "Module");
            // 获取所有数据
            var parameters = new List<KeyValuePair<string, object>>();
            if (ValidateUtil.IsInt(isMenu))
            {
                parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldIsMenu, isMenu));
            }
            parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldSystemCode, systemCode));
            parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldEnabled, 1));
            parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldDeleted, 0));
            //2017.12.20增加默认的HttpRuntime.Cache缓存
            var cacheKey = "Dt." + systemCode + ".ModuleTree." + isMenu;
            //var cacheTime = default(TimeSpan);
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            return CacheUtil.Cache<DataTable>(cacheKey, () => manager.GetModuleTree(manager.GetDataTable(parameters, BaseModuleEntity.FieldSortCode)), true, false, cacheTime);
            //直接读取数据库
            //return manager.GetModuleTree(manager.GetModuleTree(manager.GetDataTable(parameters, BaseModuleEntity.FieldSortCode)));
        }
        #endregion

    }
}
