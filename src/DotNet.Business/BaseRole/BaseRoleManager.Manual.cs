//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Business
{
    using Model;
    using System.Reflection;
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
        /// <param name="tableName">表名</param>
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
                var baseChangeLogEntity = new BaseChangeLogEntity
                {
                    TableName = CurrentTableName,
                    TableDescription = CurrentTableDescription,
                    ColumnName = property.Name,
                    ColumnDescription = fieldDescription.Text,
                    NewValue = newValue,
                    OldValue = oldValue,
                    RecordKey = entityOld.Id.ToString(),
                    SortCode = 1 // 不要排序了，加快写入速度
                };
                manager.Add(baseChangeLogEntity, true, false);
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
            var tableNameRole = GetRoleTableName(systemCode);
            var sb = PoolUtil.StringBuilder.Get().Append(" 1 = 1");

            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseRoleEntity.FieldEnabled + " = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseRoleEntity.FieldDeleted + " = 0");
            }
            //是否显示已隐藏记录
            if (!showInvisible)
            {
                sb.Append(" AND " + BaseRoleEntity.FieldIsVisible + " = 1");
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
            //子系统
            sb.Append(" AND " + BaseRoleEntity.FieldSystemCode + " = '" + systemCode + "'");
            //用户角色
            var tableNameUserRole = GetUserRoleTableName(systemCode);
            //指定用户
            if (ValidateUtil.IsInt(userId))
            {
                sb.Append(" AND ( " + BaseRoleEntity.FieldId + " IN");
                sb.Append(" (SELECT DISTINCT " + BaseUserRoleEntity.FieldRoleId);
                sb.Append(" FROM " + tableNameUserRole);
                sb.Append(" WHERE " + BaseUserRoleEntity.FieldUserId + " = " + userId + "");
                sb.Append(" AND " + BaseUserRoleEntity.FieldSystemCode + " = '" + systemCode + "'");
                sb.Append(" AND " + BaseUserRoleEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BaseUserRoleEntity.FieldDeleted + " = 0))");
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
            var tableNamePermission = GetPermissionTableName(systemCode);

            //指定的菜单模块
            if (ValidateUtil.IsInt(moduleId))
            {
                sb.Append(" AND ( " + BaseRoleEntity.FieldId + " IN");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldResourceId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldPermissionId + " = '" + moduleId + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameRole + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldSystemCode + " = '" + systemCode + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0))");
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
            sb.Replace(" 1 = 1 AND ", " ");
            //重新构造viewName
            var sbView = PoolUtil.StringBuilder.Get();
            //指定用户，就读取相应的UserRole授权日期
            if (ValidateUtil.IsInt(userId))
            {
                sbView.Append("SELECT DISTINCT " + tableNameRole + "." + BaseRoleEntity.FieldId);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldSystemCode);
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
                sbView.Append(" WHERE " + tableNameUserRole + "." + BaseUserRoleEntity.FieldUserId + " = " + userId + "");
            }
            //指定菜单模块，就读取相应的Permission授权日期
            else if (ValidateUtil.IsInt(moduleId))
            {
                sbView.Append("SELECT DISTINCT " + tableNameRole + "." + BaseRoleEntity.FieldId);
                sbView.Append("," + tableNameRole + "." + BaseRoleEntity.FieldSystemCode);
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
                sbView.Append(" WHERE " + tableNamePermission + "." + BasePermissionEntity.FieldResourceCategory + " = '" + tableNameRole + "'");
                sbView.Append(" AND " + tableNamePermission + "." + BasePermissionEntity.FieldPermissionId + " = " + moduleId + "");
            }
            else
            {
                sbView.Append(tableNameRole);
            }

            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, sbView.Return(), sb.Return());
        }
        #endregion

        #region public DataTable GetApplicationRole(BaseUserInfo userInfo)
        /// <summary>
        /// 获取应用角色
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public DataTable GetApplicationRole(BaseUserInfo userInfo, string systemCode = null)
        {
            var roleTableName = GetRoleTableName(systemCode);
            // 获得角色列表
            var manager = new BaseRoleManager(userInfo, roleTableName);
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseRoleEntity.FieldCategoryCode, "ApplicationRole"),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldIsVisible, 1)
            };
            return manager.GetDataTable(parameters, BaseRoleEntity.FieldSortCode);
        }
        #endregion

        #region protected override List<KeyValuePair<string, object>> GetDeleteExtParam(List<KeyValuePair<string, object>> parameters)

        /// <summary>
        /// 添加删除的附加条件
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        protected override List<KeyValuePair<string, object>> GetDeleteExtParam(List<KeyValuePair<string, object>> parameters)
        {
            var result = base.GetDeleteExtParam(parameters);
            result.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldAllowDelete, 1));
            return result;
        }

        #endregion

        #region public string Add(BaseRoleEntity entity, out Status status) 添加
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        /// <returns>主键</returns>
        public string Add(BaseRoleEntity entity, out Status status)
        {
            var result = string.Empty;
            // 检查名称是否重复
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseRoleEntity.FieldSystemCode, entity.SystemCode),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldName, entity.Name),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0)
            };
            //检查角色Code是否重复 Troy.Cui 2016-08-17
            var parametersCode = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseRoleEntity.FieldSystemCode, entity.SystemCode),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldCode, entity.Code),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0)
            };
            if (!string.IsNullOrEmpty(entity.OrganizationId.ToString()))
            {
                parameters.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldOrganizationId, entity.OrganizationId));
                parametersCode.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldOrganizationId, entity.OrganizationId));
            }

            if (Exists(parameters))
            {
                // 名称已重复
                status = Status.ErrorNameExist;
            }
            else if (Exists(parametersCode))
            {
                // 编码已重复
                status = Status.ErrorCodeExist;
            }
            else
            {
                result = AddEntity(entity);
                // 运行成功
                status = Status.OkAdd;
            }
            return result;
        }
        #endregion

        #region public string GetIdByCode(string systemCode,string code) 根据子系统、编码获取主键
        /// <summary>
        /// 根据子系统、编码获取主键
        /// </summary>
        /// <param name="systemCode">子系统编码</param>
        /// <param name="code">编码</param>
        /// <returns>主键</returns>
        public string GetIdByCode(string systemCode, string code)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseRoleEntity.FieldSystemCode, systemCode),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldCode, code),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1)
            };
            return DbHelper.GetProperty(CurrentTableName, parameters, BaseUtil.FieldId);
        }
        #endregion

        #region public static string GetNameByCache(string id) 通过主键获取名称

        /// <summary>
        /// 通过主键获取名称
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

        #region public static string GetIdByCodeByCache(string systemCode, string code) 通过编号获取主键
        /// <summary>
        /// 通过编号获取主键
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="code">编号</param>
        /// <returns>显示值</returns>
        public static string GetIdByCodeByCache(string systemCode, string code)
        {
            var result = string.Empty;

            var entity = GetEntityByCacheByCode(systemCode, code);
            if (entity != null)
            {
                result = entity.Id.ToString();
            }

            return result;
        }

        #endregion

        #region public static string GetIdByNameByCache(string systemCode, string name) 通过名称获取主键
        /// <summary>
        /// 通过名称获取主键
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="name">名称</param>
        /// <returns>显示值</returns>
        public static string GetIdByNameByCache(string systemCode, string name)
        {
            var result = string.Empty;

            var entity = GetEntityByCacheByName(systemCode, name);
            if (entity != null)
            {
                result = entity.Name;
            }

            return result;
        }

        #endregion

        #region public DataTable GetDataTableByOrganization(string organizationId) 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="organizationId">组织机构主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByOrganization(string organizationId)
        {
            var parametersList = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseRoleEntity.FieldOrganizationId, organizationId),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0)
            };
            return GetDataTable(parametersList, BaseRoleEntity.FieldSortCode);
            /*
            string sql = "SELECT " + BaseRoleEntity.CurrentTableName + ".*,"
                            + " (SELECT COUNT(*) FROM " + BaseUserRoleEntity.CurrentTableName + " WHERE (Enabled = 1) AND (RoleId = " + BaseRoleEntity.CurrentTableName + ".Id)) AS UserCount "
                            + " FROM " + BaseRoleEntity.CurrentTableName
                            + " WHERE " + BaseRoleEntity.FieldSystemId + " = " + "'" + systemId + "'"
                            + " ORDER BY " + BaseRoleEntity.FieldSortCode;
            return DbHelper.Fill(sql);
            */
        }
        #endregion

        #region public DataTable GetDataTableByName(string roleName) 按角色名称获取列表
        /// <summary>
        /// 按角色名称获取列表
        /// </summary>
        /// <param name="roleName">名称</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByName(string roleName)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseRoleEntity.FieldName, roleName),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0)
            };
            return GetDataTable(parameters, BaseRoleEntity.FieldSortCode);
        }
        #endregion

        #region public DataTable Search(string organizationId, string searchKey,string categoryCode=null) 查询
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="organizationId">组织机构主键</param>
        /// <param name="searchKey">查询字符串</param>
        /// <param name="categoryCode">分类编号</param>
        /// <returns>数据表</returns>
        public DataTable Search(string organizationId, string searchKey, string categoryCode = null)
        {
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT * FROM " + CurrentTableName + " WHERE " + BaseRoleEntity.FieldDeleted + " = 0");

            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetSearchString(searchKey);
                sb.Append(string.Format("  AND ({0} LIKE '{1}' OR {2} LIKE '{3}')", BaseRoleEntity.FieldName, searchKey, BaseRoleEntity.FieldDescription, searchKey));
            }
            if (!string.IsNullOrEmpty(organizationId))
            {
                sb.Append(string.Format(" AND {0} = '{1}'", BaseRoleEntity.FieldOrganizationId, organizationId));
            }
            if (!string.IsNullOrEmpty(categoryCode))
            {
                sb.Append(string.Format(" AND {0} = '{1}'", BaseRoleEntity.FieldCategoryCode, categoryCode));
            }
            sb.Append(" ORDER BY " + BaseRoleEntity.FieldSortCode);
            return DbHelper.Fill(sb.Return());
        }
        #endregion

        #region public int MoveTo(string id, string targetOrganizationId) 移动
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="targetSystemId">目标主键</param>
        /// <returns>影响行数</returns>
        public int MoveTo(string id, string targetSystemId)
        {
            //return Update(id, new KeyValuePair<string, object>(BaseRoleEntity.FieldSystemId, targetSystemId));
            return Update(id, new KeyValuePair<string, object>(BaseRoleEntity.FieldOrganizationId, targetSystemId));
        }
        #endregion

        #region public int BatchMoveTo(string[] ids, string targetOrganizationId) 批量移动
        /// <summary>
        /// 批量移动
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="targetOrganizationId">目标主键</param>
        /// <returns>影响行数</returns>
        public int BatchMoveTo(string[] ids, string targetOrganizationId)
        {
            var result = 0;
            for (var i = 0; i < ids.Length; i++)
            {
                result += MoveTo(ids[i], targetOrganizationId);
            }
            return result;
        }
        #endregion

        #region public int BatchSave(List<BaseRoleEntity> entities) 批量保存
        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <returns>影响行数</returns>
        public int BatchSave(List<BaseRoleEntity> entities)
        {
            var result = 0;
            foreach (var entity in entities)
            {
                result += UpdateEntity(entity);
            }
            return result;
        }
        #endregion

        #region public int ResetSortCode(string organizationId) 重置排序码
        /// <summary>
        /// 重置排序码
        /// </summary>
        /// <param name="organizationId">组织机构主键</param>
        public int ResetSortCode(string organizationId)
        {
            var result = 0;
            var dt = GetDataTable();
            var id = string.Empty;
            var managerSequence = new BaseSequenceManager(DbHelper);
            var sortCode = managerSequence.GetBatchSequence(BaseRoleEntity.CurrentTableName, dt.Rows.Count);
            var i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                id = dr[BaseRoleEntity.FieldId].ToString();
                result += Update(id, new KeyValuePair<string, object>(BaseRoleEntity.FieldSortCode, sortCode[i]));
                i++;
            }
            return result;
        }
        #endregion

        #region public DataTable GetDataTableByPage(string userId, string categoryCode, string serviceState, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = null, string sortDirection = null)
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="categoryCode">分类编码</param>
        /// <param name="searchKey">查询关键字</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(BaseUserInfo userInfo, string categoryCode, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = null, string sortDirection = null)
        {
            var condition = BaseRoleEntity.FieldDeleted + " = 0";

            if (!string.IsNullOrEmpty(categoryCode))
            {
                condition += string.Format(" AND {0} = '{1}'", BaseRoleEntity.FieldCategoryCode, categoryCode);
            }

            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = string.Format("'{0}'", StringUtil.GetSearchString(searchKey));
                condition += string.Format(" AND ({0} LIKE {1}", BaseRoleEntity.FieldName, searchKey);
                condition += string.Format(" OR {0} LIKE {1}", BaseRoleEntity.FieldCategoryCode, searchKey);
                condition += string.Format(" OR {0} LIKE {1})", BaseRoleEntity.FieldCode, searchKey);
            }
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, CurrentTableName, condition);
        }
        #endregion

        #region public DataTable GetUserDataTable(string systemCode, string roleId, string companyId, string userId, string searchKey, out int recordCount, int pageNo, int pageSize, string orderBy)
        /// <summary>
        /// 获取用户数据表
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <param name="companyId"></param>
        /// <param name="userId"></param>
        /// <param name="searchKey"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy">排序字段</param>
        /// <returns></returns>
        public DataTable GetUserDataTable(string systemCode, string roleId, string companyId, string userId, string searchKey, out int recordCount, int pageNo, int pageSize, string orderBy)
        {
            var result = new DataTable(BaseUserEntity.CurrentTableName);

            var userRoleTableName = GetUserRoleTableName(systemCode);

            var commandText = @"SELECT BaseUser.Id
                                    , BaseUser.Code
                                    , BaseUser.UserName
                                    , BaseUser.CompanyName
                                    , BaseUser.DepartmentName
                                    , BaseUser.RealName 
                                    , BaseUser.Description 
                                    , UserRole.Enabled
                                    , UserRole.CreateTime
                                    , UserRole.CreateBy
                                    , UserRole.UpdateTime
                                    , UserRole.UpdateBy
 FROM BaseUser,
                          (SELECT UserId, Enabled, CreateTime, CreateBy, UpdateTime, UpdateBy
 FROM BaseUserRole
                            WHERE RoleId = " + DbHelper.GetParameter(BaseUserRoleEntity.FieldRoleId) + @" AND " + BaseUserEntity.FieldDeleted + @" = 0) UserRole 
                         WHERE BaseUser.Id = UserRole.UserId 
                               AND BaseUser." + BaseUserEntity.FieldDeleted + " = 0";
            if (!string.IsNullOrEmpty(searchKey))
            {
                // 2016-02-25 吉日嘎拉 增加搜索功能、方便管理
                if (searchKey.IndexOf("%") < 0)
                {
                    searchKey = string.Format("%{0}%", searchKey);
                }
                commandText += " AND (" + BaseUserEntity.FieldCode + " LIKE '%" + searchKey + "%'"
                         + " OR " + BaseUserEntity.FieldUserName + " LIKE '%" + searchKey + "%'"
                         + " OR " + BaseUserEntity.FieldDepartmentName + " LIKE '%" + searchKey + "%'"
                         + " OR " + BaseUserEntity.FieldRealName + " LIKE '%" + searchKey + "%')";
            }
            // ORDER BY UserRole.CreateTime DESC ";
            commandText = commandText.Replace("BaseUserRole", userRoleTableName);
            var dbParameters = new List<IDbDataParameter>
            {
                DbHelper.MakeParameter(BaseUserRoleEntity.FieldRoleId, roleId)
            };

            if (!string.IsNullOrEmpty(companyId))
            {
                commandText += " AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldCompanyId + " = " + DbHelper.GetParameter(BaseUserEntity.FieldCompanyId);
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldCompanyId, companyId));
            }
            if (!string.IsNullOrEmpty(userId))
            {
                commandText += " AND " + BaseUserEntity.CurrentTableName + "." + BaseUserEntity.FieldId + " = " + DbHelper.GetParameter(BaseUserEntity.FieldId);
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldId, userId));
            }
            commandText = "(" + commandText + ") T ";
            // 2015-12-05 吉日嘎拉 增加参数化功能
            result = DbHelper.GetDataTableByPage(out recordCount, commandText, "*", pageNo, pageSize, null, dbParameters.ToArray(), orderBy);

            return result;
        }

        #endregion

        #region public override int Delete(string id) 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public override int Delete(string id)
        {
            var result = 0;

            // 删除角色权限结构定义
            // result = DbUtil.Delete(DbHelper, BaseRoleModuleOperationTable.TableName, BaseRoleModuleOperationTable.FieldRoleId, id);

            // 删除员工角色结构定义部分
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldRoleId, id)
            };
            result += DbHelper.Delete(BaseUserRoleEntity.CurrentTableName, parameters);

            // 删除角色的表结构定义部分
            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseRoleEntity.FieldId, id),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldAllowDelete, 1)
            };
            result += DbHelper.Delete(CurrentTableName, parameters);

            return result;
        }
        #endregion

        #region public int BatchDelete(string id) 批量删除
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int BatchDelete(string[] ids)
        {
            var result = 0;

            for (var i = 0; i < ids.Length; i++)
            {
                result += Delete(ids[i]);
            }

            return result;
        }
        #endregion

        #region public static BaseRoleEntity GetEntityByCache(string id)

        /// <summary>
        /// 从缓存获取获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public static BaseRoleEntity GetEntityByCache(string id)
        {
            return GetEntityByCache("Base", id);
        }
        #endregion

        #region public static BaseRoleEntity GetEntityByCache(BaseUserInfo userInfo, string id)
        /// <summary>
        /// 从缓存获取获取实体
        /// </summary>
        /// <param name="userInfo">UserInfo</param>
        /// <param name="id">主键</param>
        public static BaseRoleEntity GetEntityByCache(BaseUserInfo userInfo, string id)
        {
            return GetEntityByCache(userInfo.SystemCode, id);
        }

        #endregion

        #region public static BaseRoleEntity GetEntityByCache(string systemCode, string id, bool refreshCache = false)
        /// <summary>
        /// 从缓存获取获取实体
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="id"></param>
        /// <param name="refreshCache"></param>
        /// <returns></returns>
        public static BaseRoleEntity GetEntityByCache(string systemCode, string id, bool refreshCache = false)
        {
            BaseRoleEntity result = null;

            if (string.IsNullOrWhiteSpace(systemCode))
            {
                systemCode = "Base";
            }
            // 动态读取表中的数据
            var roleTableName = GetRoleTableName(systemCode);
            //2017.12.20增加默认的HttpRuntime.Cache缓存
            var cacheKey = "List." + systemCode + "." + roleTableName;
            //var cacheTime = default(TimeSpan);
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            var listRole = CacheUtil.Cache<List<BaseRoleEntity>>(cacheKey, () =>
            {
                var parametersWhere = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldSystemCode,systemCode),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1)
                };
                return new BaseRoleManager(roleTableName).GetList<BaseRoleEntity>(parametersWhere, BaseRoleEntity.FieldId);
            }, true, refreshCache, cacheTime);
            result = listRole.Find(t => t.Id.Equals(id));
            //直接读取数据库
            //BaseRoleManager manager = new BaseRoleManager(tableName);
            //result = manager.GetEntity(id);
            return result;
        }
        #endregion

        #region public static BaseRoleEntity GetEntityByCacheByCode(string systemCode, string code)
        /// <summary>
        /// 从缓存获取获取实体
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="code">编号</param>
        /// <returns>权限实体</returns>
        public static BaseRoleEntity GetEntityByCacheByCode(string systemCode, string code)
        {
            BaseRoleEntity result = null;

            if (string.IsNullOrWhiteSpace(systemCode))
            {
                systemCode = "Base";
            }

            // 动态读取表中的数据
            var roleTableName = GetRoleTableName(systemCode);
            //2017.12.19增加默认的HttpRuntime.Cache缓存
            var cacheKey = "List." + systemCode + "." + roleTableName;
            //var cacheTime = default(TimeSpan);
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            var listRole = CacheUtil.Cache<List<BaseRoleEntity>>(cacheKey, () =>
            {
                var parametersWhere = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldSystemCode,systemCode),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1)
                };
                return new BaseRoleManager(roleTableName).GetList<BaseRoleEntity>(parametersWhere, BaseRoleEntity.FieldId);
            }, true, false, cacheTime);
            result = listRole.Find(t => t.Code == code);
            //直接读取数据库
            //BaseRoleManager manager = new BaseRoleManager(tableName);
            //result = manager.GetEntityByCode(code);

            return result;
        }

        #endregion

        #region public static BaseRoleEntity GetEntityByCacheByName(string systemCode, string name)
        /// <summary>
        /// 从缓存获取获取实体
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="name">名称</param>
        /// <returns>权限实体</returns>
        public static BaseRoleEntity GetEntityByCacheByName(string systemCode, string name)
        {
            BaseRoleEntity result = null;

            if (string.IsNullOrWhiteSpace(systemCode))
            {
                systemCode = "Base";
            }

            // 动态读取表中的数据
            var roleTableName = GetRoleTableName(systemCode);
            //2017.12.20增加默认的HttpRuntime.Cache缓存
            var cacheKey = "List." + systemCode + "." + roleTableName;
            //var cacheTime = default(TimeSpan);
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            var listRole = CacheUtil.Cache<List<BaseRoleEntity>>(cacheKey, () =>
            {
                var parametersWhere = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldSystemCode,systemCode),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1)
                };
                return new BaseRoleManager(roleTableName).GetList<BaseRoleEntity>(parametersWhere, BaseRoleEntity.FieldId);
            }, true, false, cacheTime);
            result = listRole.Find(t => t.Name == name);
            //直接读取数据库
            //BaseRoleManager manager = new BaseRoleManager(tableName);
            //result = manager.GetEntityByName(name);

            return result;
        }

        #endregion

        #region public static List<BaseRoleEntity> GetEntitiesByCache(string systemCode = "Base") 获取模块菜单表，从缓存读取

        /// <summary>
        /// 获取模块菜单表，从缓存读取
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="refreshCache">是否刷新缓存</param>
        /// <returns>角色列表</returns>
        public static List<BaseRoleEntity> GetEntitiesByCache(string systemCode = "Base", bool refreshCache = false)
        {
            var result = new List<BaseRoleEntity>();

            var roleTableName = GetRoleTableName(systemCode);

            //2017.12.20增加默认的HttpRuntime.Cache缓存
            var cacheKey = "List." + systemCode + "." + roleTableName;
            //var cacheTime = default(TimeSpan);
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            result = CacheUtil.Cache<List<BaseRoleEntity>>(cacheKey, () =>
            {
                var roleManager = new BaseRoleManager(roleTableName);
                // 读取目标表中的数据
                var parametersWhere = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldSystemCode, systemCode),
                    // 有效的菜单
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1),
                    // 没被删除的菜单
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0)
                };

                // parameters.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldIsVisible, 1));
                return roleManager.GetList<BaseRoleEntity>(parametersWhere, BaseRoleEntity.FieldSortCode);
            }, true, refreshCache, cacheTime);

            return result;
        }
        #endregion

    }
}
