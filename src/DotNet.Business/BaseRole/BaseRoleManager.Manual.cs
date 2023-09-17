//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseRoleManager 
    /// 角色管理
    ///
    /// 修改记录
    /// 
    ///		2016.07.24 版本：1.0 Troy.Cui	新增。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.07.24</date>
    /// </author> 
    /// </summary>
    public partial class BaseRoleManager : BaseManager
    {
        #region UniqueAdd
        /// <summary>
        /// 检查唯一值式新增
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        public string UniqueAdd(BaseRoleEntity entity, out Status status)
        {
            var result = string.Empty;
            //检查是否重复
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseRoleEntity.FieldSystemCode, entity.SystemCode),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldName, entity.Name),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0)
            };

            //检查是否重复
            var parametersCode = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseRoleEntity.FieldSystemCode, entity.SystemCode),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldCode, entity.Code),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0)
            };

            if (!IsUnique(parameters, entity.Id.ToString()))
            {
                //名称已重复
                Status = Status.ErrorNameExist;
                StatusCode = Status.ErrorNameExist.ToString();
                StatusMessage = Status.ErrorNameExist.ToDescription();
            }
            else if (!IsUnique(parametersCode, entity.Id.ToString()))
            {
                //名称已重复
                Status = Status.ErrorCodeExist;
                StatusCode = Status.ErrorCodeExist.ToString();
                StatusMessage = Status.ErrorCodeExist.ToDescription();
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

        #region public int Update(BaseRoleEntity entity, out Status status) 更新
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        /// <returns>影响行数</returns>
        public int UniqueUpdate(BaseRoleEntity entity, out Status status)
        {
            var result = 0;
            // 检查是否已被其他人修改

            if (DbHelper.IsUpdate(CurrentTableName, entity.Id, entity.UpdateUserId.ToString(), entity.UpdateTime))
            {
                // 数据已经被修改
                status = Status.ErrorChanged;
            }
            else
            {
                // 检查名称是否重复
                var parameters = new List<KeyValuePair<string, object>> {
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldSystemCode, entity.SystemCode),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldName, entity.Name),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1)
                };
                if (!string.IsNullOrEmpty(entity.OrganizationId.ToString()))
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldOrganizationId, entity.OrganizationId));
                }

                //检查角色Code是否重复 Troy.Cui 2016-08-17
                var parametersCode = new List<KeyValuePair<string, object>> {
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldSystemCode, entity.SystemCode),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldCode, entity.Code),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1)
                };
                if (!string.IsNullOrEmpty(entity.OrganizationId.ToString()))
                {
                    parametersCode.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldOrganizationId, entity.OrganizationId));
                }

                if (Exists(parameters, entity.Id))
                {
                    // 名称已重复
                    status = Status.ErrorNameExist;
                    StatusCode = Status.ErrorNameExist.ToString();
                    StatusMessage = Status.ErrorNameExist.ToDescription();
                }
                else if (Exists(parametersCode, entity.Id))
                {
                    // 编码已重复
                    status = Status.ErrorCodeExist;
                    StatusCode = Status.ErrorCodeExist.ToString();
                    StatusMessage = Status.ErrorCodeExist.ToDescription();
                }
                else
                {
                    // 获取原始实体信息
                    var entityOld = GetEntity(entity.Id.ToString());
                    // 保存修改记录
                    SaveEntityChangeLog(entity, entityOld);

                    result = UpdateEntity(entity);
                    if (result == 1)
                    {
                        status = Status.OkUpdate;
                        StatusCode = Status.OkUpdate.ToString();
                        StatusMessage = Status.OkUpdate.ToDescription();
                    }
                    else
                    {
                        status = Status.ErrorDeleted;
                        StatusCode = Status.ErrorDeleted.ToString();
                        StatusMessage = Status.ErrorDeleted.ToDescription();
                    }
                }
            }

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
        public void SaveEntityChangeLog(BaseRoleEntity entityNew, BaseRoleEntity entityOld, string tableName = null)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                //统一放在一个公共表 Troy.Cui 2016-08-17
                tableName = BaseChangeLogEntity.CurrentTableName;
            }
            var manager = new BaseChangeLogManager(UserInfo, tableName);
            foreach (var property in typeof(BaseRoleEntity).GetProperties())
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
                    TableDescription = typeof(BaseRoleEntity).FieldDescription("CurrentTableName"),
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
        /// <param name="systemCode">子系统</param>
        /// <param name="categoryCode">分类编码</param>
        /// <param name="userId">指定用户</param>
        /// <param name="userIdExcluded">排除指定用户</param>
        /// <param name="moduleId">指定菜单模块</param>
        /// <param name="moduleIdExcluded">排除指定菜单模块</param>
        /// <param name="showInvisible">显示隐藏</param>
        /// <param name="codePrefix">前缀</param>
        /// <param name="codePrefixExcluded">排除前缀</param>
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
        public DataTable GetDataTableByPage(string systemCode, string categoryCode, string userId, string userIdExcluded, string moduleId, string moduleIdExcluded, bool showInvisible, string codePrefix, string codePrefixExcluded, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = "CreateTime", string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            //角色名
            var tableNameRole = BaseRoleEntity.CurrentTableName;
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableNameRole = systemCode + "Role";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(UserInfo.SystemCode))
                {
                    tableNameRole = UserInfo.SystemCode + "Role";
                }
            }
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");

            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseRoleEntity.FieldEnabled + "  = 1 ");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseRoleEntity.FieldDeleted + "  = 0 ");
            }
            //是否显示已隐藏记录
            if (!showInvisible)
            {
                sb.Append(" AND " + BaseRoleEntity.FieldIsVisible + "  = 1 ");
            }
            //创建时间
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseRoleEntity.FieldCreateTime + " >= " + dbHelper.ToDbTime(startTime));
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseRoleEntity.FieldCreateTime + " <= " + dbHelper.ToDbTime(endTime.ToDateTime().Date.AddDays(1).AddMilliseconds(-1)));
            }

            //角色分类
            if (!string.IsNullOrEmpty(categoryCode))
            {
                sb.Append(" AND " + BaseRoleEntity.FieldCategoryCode + " = N'" + categoryCode + "'");
            }
            //用户角色
            var tableNameUserRole = UserInfo.SystemCode + "UserRole";
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableNameUserRole = systemCode + "UserRole";
            }
            //指定用户
            if (ValidateUtil.IsInt(userId))
            {
                sb.Append(" AND ( " + BaseRoleEntity.FieldId + " IN ");
                sb.Append(" (SELECT DISTINCT " + BaseUserRoleEntity.FieldRoleId);
                sb.Append(" FROM " + tableNameUserRole);
                sb.Append(" WHERE " + BaseUserRoleEntity.FieldUserId + " = " + userId + "");
                sb.Append(" AND " + BaseUserRoleEntity.FieldSystemCode + " = '" + systemCode + "'");
                sb.Append(" AND " + BaseUserRoleEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BaseUserRoleEntity.FieldDeleted + " = 0)) ");
            }
            //排除指定用户
            if (ValidateUtil.IsInt(userIdExcluded))
            {
                sb.Append(" AND ( " + BaseRoleEntity.FieldId + " NOT IN ");
                sb.Append(" (SELECT DISTINCT " + BaseUserRoleEntity.FieldRoleId);
                sb.Append(" FROM " + tableNameUserRole);
                sb.Append(" WHERE " + BaseUserRoleEntity.FieldUserId + " = " + userIdExcluded + "");
                sb.Append(" AND " + BaseUserRoleEntity.FieldSystemCode + " = '" + systemCode + "'");
                sb.Append(" AND " + BaseUserRoleEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BaseUserRoleEntity.FieldDeleted + " = 0)) ");
            }
            //用户菜单模块表
            var tableNamePermission = UserInfo.SystemCode + "Permission";
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableNamePermission = systemCode + "Permission";
            }

            //指定的菜单模块
            if (ValidateUtil.IsInt(moduleId))
            {
                sb.Append(" AND ( " + BaseRoleEntity.FieldId + " IN ");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldResourceId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldPermissionId + " = '" + moduleId + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameRole + "' ");
                sb.Append(" AND " + BasePermissionEntity.FieldSystemCode + " = '" + systemCode + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0)) ");
            }
            //排除指定菜单模块
            if (ValidateUtil.IsInt(moduleIdExcluded))
            {
                sb.Append(" AND ( " + BaseRoleEntity.FieldId + " NOT IN ");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldResourceId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldPermissionId + " = '" + moduleIdExcluded + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameRole + "' ");
                sb.Append(" AND " + BasePermissionEntity.FieldSystemCode + " = '" + systemCode + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0)) ");
            }
            //前缀
            if (!string.IsNullOrEmpty(codePrefix))
            {
                codePrefix = dbHelper.SqlSafe(codePrefix);
                sb.Append(" AND  " + BaseRoleEntity.FieldCode + " LIKE N'" + codePrefix + "%'");
            }
            //排除前缀
            if (!string.IsNullOrEmpty(codePrefixExcluded))
            {
                codePrefixExcluded = dbHelper.SqlSafe(codePrefixExcluded);
                sb.Append(" AND  " + BaseRoleEntity.FieldCode + " NOT LIKE N'" + codePrefixExcluded + "%'");
            }
            //关键词
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseRoleEntity.FieldName + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseRoleEntity.FieldCode + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseRoleEntity.FieldDescription + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            //重新构造viewName
            var sbView = Pool.StringBuilder.Get();
            //指定用户，就读取相应的UserRole授权日期
            if (ValidateUtil.IsInt(userId))
            {
                sbView.Append("SELECT DISTINCT " + tableNameRole + "." + BaseRoleEntity.FieldId);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldOrganizationId);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldCategoryCode);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldCode);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldName);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldAllowEdit);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldAllowDelete);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldIsVisible);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldSortCode);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldDeleted);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldEnabled);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldDescription);
                //授权日期
                sbView.Append("," + tableNameUserRole + "." + BaseUserRoleEntity.FieldCreateTime);
                sbView.Append("," + tableNameUserRole + "." + BaseUserRoleEntity.FieldCreateUserId);
                sbView.Append("," + tableNameUserRole + "." + BaseUserRoleEntity.FieldCreateBy);
                sbView.Append("," + tableNameUserRole + "." + BaseUserRoleEntity.FieldUpdateTime);
                sbView.Append("," + tableNameUserRole + "." + BaseUserRoleEntity.FieldUpdateUserId);
                sbView.Append("," + tableNameUserRole + "." + BaseUserRoleEntity.FieldUpdateBy);
                sbView.Append(" FROM " + tableNameRole + " INNER JOIN " + tableNameUserRole);
                sbView.Append(" ON " + tableNameRole + "." + BaseRoleEntity.FieldId + " = " + tableNameUserRole + "." + BaseUserRoleEntity.FieldRoleId);
                sbView.Append(" AND " + tableNameRole + "." + BaseRoleEntity.FieldSystemCode + " = " + tableNameUserRole + "." + BaseUserRoleEntity.FieldSystemCode);
                sbView.Append(" WHERE (" + tableNameUserRole + "." + BaseUserRoleEntity.FieldUserId + " = " + userId + ")");
            }
            //指定菜单模块，就读取相应的Permission授权日期
            else if (ValidateUtil.IsInt(moduleId))
            {
                sbView.Append("SELECT DISTINCT " + tableNameRole + "." + BaseRoleEntity.FieldId);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldOrganizationId);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldCategoryCode);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldCode);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldName);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldAllowEdit);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldAllowDelete);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldIsVisible);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldSortCode);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldDeleted);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldEnabled);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldDescription);
                //授权日期
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldCreateTime);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldCreateUserId);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldCreateBy);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateTime);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateUserId);
                sbView.Append("," + tableNamePermission + "." + BasePermissionEntity.FieldUpdateBy);
                sbView.Append(" FROM " + tableNameRole + " INNER JOIN " + tableNamePermission);
                sbView.Append(" ON " + tableNameRole + "." + BaseRoleEntity.FieldId + " = " + tableNamePermission + "." + BasePermissionEntity.FieldResourceId);
                sbView.Append(" AND " + tableNameRole + "." + BaseRoleEntity.FieldSystemCode + " = " + tableNamePermission + "." + BasePermissionEntity.FieldSystemCode);
                sbView.Append(" WHERE (" + tableNamePermission + "." + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameRole + "')");
                sbView.Append(" AND (" + tableNamePermission + "." + BasePermissionEntity.FieldPermissionId + " = " + moduleId + ")");
            }
            else
            {
                sbView.Append(tableNameRole);
            }

            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, sbView.Put(), sb.Put());
        }
        #endregion
    }
}
