//-----------------------------------------------------------------------
// <copyright file="BaseRoleManager.cs" company="DotNet">
//     Copyright (c) 2023, All rights reserved.
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
    /// BaseRoleManager 
    /// 角色结构定义部分
    ///
    /// 修改记录
    ///
    ///     2018-09-07 版本：4.1 Troy.Cui   增加删除缓存功能。
    ///     2008.04.09 版本：1.0 JiRiGaLa 创建主键。
    ///     
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2008.04.09</date>
    /// </author>
    /// </summary>
    public partial class BaseRoleManager : BaseManager //, IBaseRoleManager
    {
        #region public override DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseRoleEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = false, bool showDeleted = false)
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
        public override DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseRoleEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
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

            if (ValidateUtil.IsInt(companyId))
            {
                //sb.Append(" AND " + BaseRoleEntity.FieldCompanyId + " = " + companyId);
            }
            // 只有管理员才能看到所有的
            //if (!(UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled))
            //{
            //sb.Append(" AND (" + BaseRoleEntity.FieldUserCompanyId + " = 0 OR " + BaseRoleEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            //}
            if (ValidateUtil.IsInt(departmentId))
            {
                //sb.Append(" AND " + BaseRoleEntity.FieldDepartmentId + " = " + departmentId);
            }
            if (ValidateUtil.IsInt(userId))
            {
                //sb.Append(" AND " + BaseRoleEntity.FieldUserId + " = " + userId);
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
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseRoleEntity.FieldName + " LIKE N'%" + searchKey + "%' OR " + BaseRoleEntity.FieldDescription + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
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
                //sb.Append("(" + BaseRoleEntity.FieldUserCompanyId + " = 0 OR " + BaseRoleEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            }
            //return GetDataTable(sb.Return(), null, new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0));
            var companyId = string.IsNullOrEmpty(BaseSystemInfo.CustomerCompanyId) ? UserInfo.CompanyId : BaseSystemInfo.CustomerCompanyId;
            var cacheKey = "Dt." + CurrentTableName + "." + companyId + "." + (myCompanyOnly ? "1" : "0");
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            return CacheUtil.Cache<DataTable>(cacheKey, () => GetDataTable(sb.Return(), null, new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0)), true, false, cacheTime);
        }

        #endregion

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
            sb.Append("SELECT * FROM " + CurrentTableName + " WHERE " + BaseRoleEntity.FieldDeleted + " = 0 ");

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
            /*
            foreach (BaseRoleEntity roleEntity in roleEntites)
            {
                // 删除状态
                if (dr.RowState == DataRowState.Deleted)
                {
                    string id = dr[BaseRoleEntity.FieldId, DataRowVersion.Original].ToString();
                    if (id.Length > 0)
                    {
                        result += this.Delete(id);
                    }
                }
                // 被修改过
                if (dr.RowState == DataRowState.Update)
                {
                    string id = dr[BaseRoleEntity.FieldId, DataRowVersion.Original].ToString();
                    if (!string.IsNullOrEmpty(id))
                    {
                        roleEntity.GetFrom(dr);
                        result += this.UpdateEntity(roleEntity);
                    }
                }
                // 添加状态
                if (dr.RowState == DataRowState.Added)
                {
                    roleEntity.GetFrom(dr);
                    result += this.AddEntity(roleEntity).Length > 0 ? 1 : 0;
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
            */

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
            var condition = BaseRoleEntity.FieldDeleted + " = 0 ";

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

            var tableName = BaseUserRoleEntity.CurrentTableName;
            if (!string.IsNullOrWhiteSpace(systemCode))
            {
                tableName = systemCode + "UserRole";
            }

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
                               AND BaseUser." + BaseUserEntity.FieldDeleted + " = 0 ";
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
            commandText = commandText.Replace("BaseUserRole", tableName);
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

        /// <summary>
        /// 获取角色权限范围（组织机构）
        /// 2015-11-28 吉日嘎拉 整理参数化
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>组织机构表</returns>
        public DataTable GetOrganizationDataTable(string systemCode, string roleId)
        {
            var result = new DataTable(BaseOrganizationEntity.CurrentTableName);

            var tableName = BaseRoleOrganizationEntity.CurrentTableName;
            if (!string.IsNullOrWhiteSpace(UserInfo.SystemCode))
            {
                tableName = UserInfo.SystemCode + "RoleOrganization";
            }

            var commandText = @"SELECT BaseOrganization.Id
                                    , BaseOrganization.Code
                                    , BaseOrganization.Name 
                                    , BaseOrganization.Description 
                                    , RoleOrganization.Enabled
                                    , RoleOrganization.CreateTime
                                    , RoleOrganization.CreateBy
                                    , RoleOrganization.UpdateTime
                                    , RoleOrganization.UpdateBy
 FROM BaseOrganization RIGHT OUTER JOIN
                          (SELECT OrganizationId, Enabled, CreateTime, CreateBy, UpdateTime, UpdateBy
 FROM BaseRoleOrganization
                            WHERE RoleId = " + DbHelper.GetParameter(BaseRoleOrganizationEntity.FieldRoleId) +
                                " AND Deleted = " + DbHelper.GetParameter(BaseRoleOrganizationEntity.FieldDeleted) + @") RoleOrganization 
                            ON BaseOrganization.Id = RoleOrganization.OrganizationId
                         WHERE BaseOrganization.Enabled = 1 AND BaseOrganization." + BaseOrganizationEntity.FieldDeleted + @" = 0
                      ORDER BY RoleOrganization.CreateTime DESC ";

            commandText = commandText.Replace("BaseRoleOrganization", tableName);

            var dbParameters = new List<IDbDataParameter>
            {
                DbHelper.MakeParameter(BaseRoleOrganizationEntity.FieldRoleId, roleId),
                DbHelper.MakeParameter(BaseRoleOrganizationEntity.FieldDeleted, 0)
            };

            result = Fill(commandText, dbParameters.ToArray());

            return result;
        }

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
            var tableName = systemCode + "Role";
            //2017.12.20增加默认的HttpRuntime.Cache缓存
            var cacheKey = "List." + systemCode + ".Role";
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
                return new BaseRoleManager(tableName).GetList<BaseRoleEntity>(parametersWhere, BaseRoleEntity.FieldId);
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
            var tableName = systemCode + "Role";
            //2017.12.19增加默认的HttpRuntime.Cache缓存
            var cacheKey = "List." + systemCode + ".Role";
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
                return new BaseRoleManager(tableName).GetList<BaseRoleEntity>(parametersWhere, BaseRoleEntity.FieldId);
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
            var tableName = systemCode + "Role";
            //2017.12.20增加默认的HttpRuntime.Cache缓存
            var cacheKey = "List." + systemCode + ".Role";
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
                return new BaseRoleManager(tableName).GetList<BaseRoleEntity>(parametersWhere, BaseRoleEntity.FieldId);
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

            var tableName = systemCode + "Role";

            //2017.12.20增加默认的HttpRuntime.Cache缓存
            var cacheKey = "List." + systemCode + ".Role";
            //var cacheTime = default(TimeSpan);
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            result = CacheUtil.Cache<List<BaseRoleEntity>>(cacheKey, () =>
            {
                var roleManager = new BaseRoleManager(tableName);
                // 读取目标表中的数据
                var parametersWhere = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldSystemCode,systemCode),
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

        #region public override bool RemoveCache() 删除缓存

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <returns></returns>
        public override bool RemoveCache()
        {
            var result = false;
            var cacheKey = "Dt." + CurrentTableName;
            var cacheKeyListBase = "List.Base.Role";
            var cacheKeyListSystemCode = "List.Base.Role";
            if (UserInfo != null)
            {
                //cacheKey += "." + UserInfo.CompanyId;
                cacheKeyListSystemCode = "List." + UserInfo.SystemCode + ".Role";
            }

            CacheUtil.Remove(cacheKeyListBase);
            CacheUtil.Remove(cacheKeyListSystemCode);
            result = CacheUtil.Remove(cacheKey);
            return result;
        }
        #endregion
    }
}
