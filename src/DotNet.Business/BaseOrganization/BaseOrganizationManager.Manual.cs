//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using System.Collections.Generic;
    using System.Linq;
    using Util;

    /// <summary>
    /// BaseOrganizationManager
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
    public partial class BaseOrganizationManager : BaseManager
    {
        #region UniqueAdd
        /// <summary>
        /// 检查唯一值式新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string UniqueAdd(BaseOrganizationEntity entity, out Status status)
        {
            var result = string.Empty;

            //检查是否重复
            var parameters = new List<KeyValuePair<string, object>>();
            if (entity.ParentId > 0)
            {
                //父项不等于空的时候，才检查名称重复
                parameters.Add(new KeyValuePair<string, object>(BaseOrganizationEntity.FieldParentId, entity.ParentId));
                parameters.Add(new KeyValuePair<string, object>(BaseOrganizationEntity.FieldFullName, entity.FullName));
                parameters.Add(new KeyValuePair<string, object>(BaseOrganizationEntity.FieldEnabled, 1));
                parameters.Add(new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0));
            }

            if ((entity.ParentId > 0) && Exists(parameters))
            {
                //名称已重复
                Status = Status.ErrorNameExist;
                StatusCode = Status.ErrorNameExist.ToString();
                StatusMessage = Status.ErrorNameExist.ToDescription();
            }
            else
            {
                parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldCode, entity.Code),
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0)
                };

                if (entity.Code.Length > 0 && Exists(parameters))
                {
                    //编号已重复
                    Status = Status.ErrorCodeExist;
                    StatusCode = Status.ErrorCodeExist.ToString();
                    StatusMessage = Status.ErrorCodeExist.ToDescription();
                }
                else
                {
                    result = AddEntity(entity);
                    if (!string.IsNullOrEmpty(result))
                    {
                        AfterAdd(entity);
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
            }
            status = Status;
            return result;
        }

        #endregion

        #region UniqueUpdate
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int UniqueUpdate(BaseOrganizationEntity entity, out Status status)
        {
            var result = 0;

            // 检查是否已被其他人修改            
            //if (DbUtil.IsModifed(DbHelper, BaseOrganizationEntity.CurrentTableName, entity.Id, entity.UpdateUserId, entity.UpdateTime))
            //{
            //    // 数据已经被修改
            //    status = StatusCode.ErrorChanged.ToString();
            //}

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseOrganizationEntity.FieldParentId, entity.ParentId),
                new KeyValuePair<string, object>(BaseOrganizationEntity.FieldFullName, entity.FullName),
                new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0)
            };

            if (Exists(parameters, entity.Id))
            {
                // 名称已重复
                Status = Status.ErrorNameExist;
                StatusCode = Status.ErrorNameExist.ToString();
                StatusMessage = Status.ErrorNameExist.ToDescription();
            }
            else
            {
                // 检查编号是否重复
                parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldCode, entity.Code),
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0)
                };

                if (entity.Code.Length > 0 && Exists(parameters, entity.Id))
                {
                    // 编号已重复
                    Status = Status.ErrorCodeExist;
                    StatusCode = Status.ErrorCodeExist.ToString();
                    StatusMessage = Status.ErrorCodeExist.ToDescription();
                }
                else
                {
                    if (string.IsNullOrEmpty(entity.QuickQuery))
                    {
                        // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
                        entity.QuickQuery = StringUtil.GetPinyin(entity.FullName).ToLower();
                    }
                    if (string.IsNullOrEmpty(entity.SimpleSpelling))
                    {
                        // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
                        entity.SimpleSpelling = StringUtil.GetSimpleSpelling(entity.FullName).ToLower();
                    }

                    // 获取原始实体信息
                    var entityOld = GetEntity(entity.Id);
                    // 保存修改记录
                    UpdateEntityLog(entity, entityOld);

                    // 1:更新部门的信息
                    result = UpdateEntity(entity);

                    // 2:组织机构修改时，用户表的公司，部门，工作组数据给同步更新。
                    var userManager = new BaseUserManager(DbHelper, UserInfo);
                    switch (entity.CategoryCode)
                    {
                        case "Company":
                            userManager.SetProperty(new KeyValuePair<string, object>(BaseUserEntity.FieldCompanyId, entity.Id), new KeyValuePair<string, object>(BaseUserEntity.FieldCompanyName, entity.FullName));
                            break;
                        case "SubCompany":
                            userManager.SetProperty(new KeyValuePair<string, object>(BaseUserEntity.FieldSubCompanyId, entity.Id), new KeyValuePair<string, object>(BaseUserEntity.FieldSubCompanyName, entity.FullName));
                            break;
                        case "Department":
                            userManager.SetProperty(new KeyValuePair<string, object>(BaseUserEntity.FieldDepartmentId, entity.Id), new KeyValuePair<string, object>(BaseUserEntity.FieldDepartmentName, entity.FullName));
                            break;
                        case "SubDepartment":
                            userManager.SetProperty(new KeyValuePair<string, object>(BaseUserEntity.FieldSubDepartmentId, entity.Id), new KeyValuePair<string, object>(BaseUserEntity.FieldSubDepartmentName, entity.FullName));
                            break;
                        case "Workgroup":
                            userManager.SetProperty(new KeyValuePair<string, object>(BaseUserEntity.FieldWorkgroupId, entity.Id), new KeyValuePair<string, object>(BaseUserEntity.FieldWorkgroupName, entity.FullName));
                            break;

                    }
                    // 03：组织机构修改时，文件夹同步更新
                    // BaseFolderManager folderManager = new BaseFolderManager(this.DbHelper, this.UserInfo);
                    // folderManager.SetProperty(new KeyValuePair<string, object>(BaseFolderEntity.FieldFolderName, entity.FullName), new KeyValuePair<string, object>(BaseFolderEntity.FieldId, entity.Id));
                    if (result == 1)
                    {
                        // AfterUpdate(entity);
                        SetCache(entity.Id.ToString());
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
            }
            status = Status;
            return result;
        }

        #endregion

        #region public void UpdateEntityLog(BaseOrganizationEntity newEntity, BaseOrganizationEntity oldEntity)
        /// <summary>
        /// 保存实体修改记录
        /// </summary>
        /// <param name="newEntity">修改前的实体对象</param>
        /// <param name="oldEntity">修改后的实体对象</param>
        /// <param name="tableName">表名称</param>
        public void UpdateEntityLog(BaseOrganizationEntity newEntity, BaseOrganizationEntity oldEntity, string tableName = null)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                //统一放在一个公共表 Troy.Cui 2016-08-17
                tableName = BaseChangeLogEntity.CurrentTableName;
            }
            var manager = new BaseChangeLogManager(UserInfo, tableName);
            foreach (var property in typeof(BaseOrganizationEntity).GetProperties())
            {
                var oldValue = Convert.ToString(property.GetValue(oldEntity, null));
                var newValue = Convert.ToString(property.GetValue(newEntity, null));
                var fieldDescription = property.GetCustomAttributes(typeof(FieldDescription), false).FirstOrDefault() as FieldDescription;
                //不记录创建人、修改人、没有修改的记录
                if (!fieldDescription.NeedLog || oldValue == newValue)
                {
                    continue;
                }
                var record = new BaseChangeLogEntity
                {
                    TableName = CurrentTableName,
                    TableDescription = FieldExtensions.ToDescription(typeof(BaseOrganizationEntity), "CurrentTableName"),
                    ColumnName = property.Name,
                    ColumnDescription = fieldDescription.Text,
                    RecordKey = oldEntity.Id.ToString(),
                    NewValue = newValue,
                    OldValue = oldValue
                };
                manager.Add(record, true, false);
            }
        }
        #endregion

        #region 高级查询
        /// <summary>
        /// 按条件分页高级查询(带记录状态Enabled和删除状态Deleted)
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="categoryCode">分类编码</param>
        /// <param name="companyId">查看公司主键</param>
        /// <param name="departmentId">查看部门主键</param>
        /// <param name="roleId">角色编号</param>
        /// <param name="roleIdExcluded">排除的角色编号</param>
        /// <param name="moduleId">模块编号</param>
        /// <param name="moduleIdExcluded">排除的模块编号</param>
        /// <param name="parentId">父节点编号</param>
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
        public DataTable GetDataTableByPage(string systemCode, string categoryCode, string companyId, string departmentId, string roleId, string roleIdExcluded, string moduleId, string moduleIdExcluded, string parentId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = "CreateTime", string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldEnabled + " = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldDeleted + " = 0");
            }
            //分类
            if (!string.IsNullOrEmpty(categoryCode))
            {
                sb.Append(" AND " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldCategoryCode + " = N'" + categoryCode + "'");
            }
            if (ValidateUtil.IsInt(companyId))
            {
                //只选择下一级
                //sb.Append(" AND " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldDepartmentId 
                //    + " IN ( SELECT " + BaseOrganizationEntity.FieldId 
                //    + " FROM " + BaseOrganizationEntity.CurrentTableName 
                //    + " WHERE " + BaseOrganizationEntity.FieldId + " = " + departmentId + " OR " + BaseOrganizationEntity.FieldParentId + " = " + departmentId + ")";

                //所有下级的都列出来
                var ids = GetChildrensId(BaseOrganizationEntity.FieldId, departmentId, BaseOrganizationEntity.FieldParentId);
                if (ids != null && ids.Length > 0)
                {
                    sb.Append(" AND (" + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldCompanyId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldId + " IN (" + StringUtil.ArrayToList(ids) + "))");
                }
                sb.Append(" AND (" + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldCompanyId + " = " + companyId + ")");
            }
            if (ValidateUtil.IsInt(departmentId))
            {
                sb.Append(" AND " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldId + " = " + departmentId);
            }

            //是否显示已隐藏记录
            //if (!showInvisible)
            //{
            //    sb.Append(" AND " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldIsVisible + "  = 1 ";
            //}
            //角色
            var tableNameRoleOrganization = UserInfo.SystemCode + "RoleOrganization";
            if (!string.IsNullOrEmpty(systemCode))
            {
                tableNameRoleOrganization = systemCode + "RoleOrganization";
            }
            //指定角色
            if (!string.IsNullOrEmpty(roleId) && ValidateUtil.IsNumeric(roleId))
            {
                sb.Append(" AND ( " + BaseOrganizationEntity.FieldId + " IN ");
                sb.Append(" (SELECT DISTINCT " + BaseRoleOrganizationEntity.FieldOrganizationId);
                sb.Append(" FROM " + tableNameRoleOrganization);
                sb.Append(" WHERE " + BaseRoleOrganizationEntity.FieldRoleId + " = '" + roleId + "'");
                sb.Append(" AND " + BaseRoleOrganizationEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BaseRoleOrganizationEntity.FieldDeleted + " = 0)) ");
            }
            //排除指定角色
            if (!string.IsNullOrEmpty(roleIdExcluded) && ValidateUtil.IsNumeric(roleIdExcluded))
            {
                sb.Append(" AND ( " + BaseOrganizationEntity.FieldId + " NOT IN ");
                sb.Append(" (SELECT DISTINCT " + BaseRoleOrganizationEntity.FieldOrganizationId);
                sb.Append(" FROM " + tableNameRoleOrganization);
                sb.Append(" WHERE " + BaseRoleOrganizationEntity.FieldRoleId + " = '" + roleIdExcluded + "'");
                sb.Append(" AND " + BaseRoleOrganizationEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BaseRoleOrganizationEntity.FieldDeleted + " = 0)) ");
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
                sb.Append(" AND ( " + BaseOrganizationEntity.FieldId + " IN ");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldResourceId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldPermissionId + " = '" + moduleId + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + BaseOrganizationEntity.CurrentTableName + "' ");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0)) ");
            }
            //排除指定菜单模块
            if (!string.IsNullOrEmpty(moduleIdExcluded) && ValidateUtil.IsNumeric(moduleIdExcluded))
            {
                sb.Append(" AND ( " + BaseOrganizationEntity.FieldId + " NOT IN ");
                sb.Append(" (SELECT DISTINCT " + BasePermissionEntity.FieldResourceId);
                sb.Append(" FROM " + tableNamePermission);
                sb.Append(" WHERE " + BasePermissionEntity.FieldPermissionId + " = '" + moduleIdExcluded + "'");
                sb.Append(" AND " + BasePermissionEntity.FieldResourceCategory + " = '" + BaseOrganizationEntity.CurrentTableName + "' ");
                sb.Append(" AND " + BasePermissionEntity.FieldEnabled + " = 1");
                sb.Append(" AND " + BasePermissionEntity.FieldDeleted + " = 0)) ");
            }
            //父级编号
            if (!string.IsNullOrEmpty(parentId) && ValidateUtil.IsNumeric(parentId))
            {
                sb.Append(" AND ( ");
                //本级
                sb.Append(BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldId + "  = " + parentId);
                //下级
                sb.Append(" OR " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldParentId + "  = " + parentId);
                //下下级
                sb.Append(" OR " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldParentId + " IN ");
                sb.Append(" (SELECT " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldId + " FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE " + BaseOrganizationEntity.CurrentTableName + "." + BaseModuleEntity.FieldParentId + "  = " + parentId + ") ");
                //下下下级
                sb.Append(" OR " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldParentId + " IN ");
                sb.Append(" (SELECT " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldId + " FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE " + BaseOrganizationEntity.CurrentTableName + "." + BaseModuleEntity.FieldParentId + " IN ");
                sb.Append(" (SELECT " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldId + " FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE " + BaseOrganizationEntity.CurrentTableName + "." + BaseModuleEntity.FieldParentId + " = " + parentId + ") ");
                sb.Append(" ) ");
                //下下下下级，做个组织机构实际应用应该足够了
                sb.Append(" OR " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldParentId + " IN ");
                sb.Append(" (SELECT " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldId + " FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE " + BaseOrganizationEntity.CurrentTableName + "." + BaseModuleEntity.FieldParentId + " IN ");
                sb.Append(" (SELECT " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldId + " FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE " + BaseOrganizationEntity.CurrentTableName + "." + BaseModuleEntity.FieldParentId + " IN ");
                sb.Append(" (SELECT " + BaseOrganizationEntity.CurrentTableName + "." + BaseOrganizationEntity.FieldId + " FROM " + BaseOrganizationEntity.CurrentTableName + " WHERE " + BaseOrganizationEntity.CurrentTableName + "." + BaseModuleEntity.FieldParentId + " = " + parentId + ") ");
                sb.Append(" ) ");
                sb.Append(" ) ");
                //闭合
                sb.Append(" ) ");
            }
            //创建时间
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseOrganizationEntity.FieldCreateTime + " >= '" + startTime + "'");
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseOrganizationEntity.FieldCreateTime + " <= DATEADD(s,-1,DATEADD(d,1,'" + endTime + "'))");
            }
            //关键词
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseOrganizationEntity.FieldFullName + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseOrganizationEntity.FieldCode + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseOrganizationEntity.FieldShortName + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseOrganizationEntity.FieldStandardName + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseOrganizationEntity.FieldDescription + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseOrganizationEntity.FieldQuickQuery + " LIKE N'%" + searchKey + "%'");
                sb.Append(" OR " + BaseOrganizationEntity.FieldSimpleSpelling + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Put(), null, "*");
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
            var dt = new DataTable(BaseOrganizationEntity.CurrentTableName);
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("ParentId", typeof(int));
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("FullName", typeof(string));
            dt.Columns.Add("CategoryCode", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("Enabled", typeof(int));
            dt.Columns.Add("Deleted", typeof(int));
            dt.Columns.Add("CreateTime", typeof(DateTime));
            dt.Columns.Add("CreateBy", typeof(string));
            dt.Columns.Add("UpdateTime", typeof(DateTime));
            dt.Columns.Add("UpdateBy", typeof(string));
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
                dr = dtOld.Select("ParentId = 0", BaseOrganizationEntity.FieldSortCode);
            }
            else
            {
                dr = dtOld.Select("ParentId = " + parentId + "", BaseOrganizationEntity.FieldSortCode);
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
                row["Deleted"] = int.Parse(dr[i]["Deleted"].ToString());
                if (!string.IsNullOrEmpty(dr[i]["CreateTime"].ToString()))
                {
                    row["CreateTime"] = DateTime.Parse(dr[i]["CreateTime"].ToString());
                }
                row["CreateBy"] = dr[i]["CreateBy"].ToString();
                if (!string.IsNullOrEmpty(dr[i]["UpdateTime"].ToString()))
                {
                    row["UpdateTime"] = DateTime.Parse(dr[i]["UpdateTime"].ToString());
                }
                row["UpdateBy"] = dr[i]["UpdateBy"].ToString();
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

        #region GetOrganizationCode 获得组织编码
        public string GetOrganizationCode(string id)
        {
            var result = string.Empty;
            if (ValidateUtil.IsInt(id))
            {
                var entity = GetEntity(id);
                if (entity != null)
                {
                    result = entity.Code;
                }
            }
            return result;
        }
        #endregion

        #region GetOrganizationName 获得组织名称
        public string GetOrganizationName(string id)
        {
            var result = string.Empty;
            if (ValidateUtil.IsInt(id))
            {
                var entity = GetEntity(id);
                if (entity != null)
                {
                    result = entity.FullName;
                }
            }
            return result;
        }
        #endregion
    }
}