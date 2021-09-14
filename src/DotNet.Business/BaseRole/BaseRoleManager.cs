﻿//-----------------------------------------------------------------
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
    /// BaseRoleManager 
    /// 角色表结构定义部分
    ///
    /// 修改记录
    ///
    ///     2018-09-07 版本：4.1 Troy.Cui   增加删除缓存功能。
    ///     2008.04.09 版本：1.0 JiRiGaLa 创建主键。
    ///     
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2008.04.09</date>
    /// </author>
    /// </summary>
    public partial class BaseRoleManager : BaseManager //, IBaseRoleManager
    {
        #region public string Add(BaseRoleEntity entity, out string statusCode) 添加
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <returns>主键</returns>
        public string Add(BaseRoleEntity entity, out string statusCode)
        {
            var result = string.Empty;
            // 检查名称是否重复
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseRoleEntity.FieldRealName, entity.RealName),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0)
            };
            if (!string.IsNullOrEmpty(entity.OrganizeId))
            {
                parameters.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldOrganizeId, entity.OrganizeId));
            }
            //检查角色Code是否重复 Troy.Cui 2016-08-17
            var parametersCode = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseRoleEntity.FieldCode, entity.Code),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0)
            };
            if (!string.IsNullOrEmpty(entity.OrganizeId))
            {
                parametersCode.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldOrganizeId, entity.OrganizeId));
            }

            if (Exists(parameters))
            {
                // 名称已重复
                statusCode = Status.ErrorNameExist.ToString();
            }
            else if (Exists(parametersCode))
            {
                // 编码已重复
                statusCode = Status.ErrorCodeExist.ToString();
            }
            else
            {
                result = AddObject(entity);
                // 运行成功
                statusCode = Status.OkAdd.ToString();
            }
            return result;
        }
        #endregion

        #region public string GetIdByRealName(string realName) 按名称获取主键
        /// <summary>
        /// 按名称获取主键
        /// </summary>
        /// <param name="realName">名称</param>
        /// <returns>主键</returns>
        public string GetIdByRealName(string realName)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseRoleEntity.FieldRealName, realName),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1)
            };
            return DbUtil.GetProperty(DbHelper, CurrentTableName, parameters, BaseUtil.FieldId);
        }
        #endregion

        /// <summary>
        /// 通过主键获取名称
        /// 这里是进行了内存缓存处理，减少数据库的I/O处理，提高程序的运行性能，
        /// 若有数据修改过，重新启动一下程序就可以了，这些基础数据也不是天天修改来修改去的，
        /// 所以没必要过度担忧，当然有需要时也可以写个刷新缓存的程序
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="id">主键</param>
        /// <returns>显示值</returns>
        public static string GetRealNameByCache(string systemCode, string id)
        {
            var result = string.Empty;

            var entity = GetObjectByCache("Base", id);
            if (entity != null)
            {
                result = entity.RealName;
            }

            return result;
        }

        /// <summary>
        /// 通过编号获取主键
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="code">编号</param>
        /// <returns>显示值</returns>
        public static string GetIdByCodeByCache(string systemCode, string code)
        {
            var result = string.Empty;

            var entity = GetObjectByCacheByCode(systemCode, code);
            if (entity != null)
            {
                result = entity.Id;
            }

            return result;
        }

        /// <summary>
        /// 通过名称获取主键
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="realName">名称</param>
        /// <returns>显示值</returns>
        public static string GetIdByNameByCache(string systemCode, string realName)
        {
            var result = string.Empty;

            var entity = GetObjectByCacheByName(systemCode, realName);
            if (entity != null)
            {
                result = entity.RealName;
            }

            return result;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="code">编号</param>
        /// <returns>实体</returns>
        public BaseRoleEntity GetObjectByCode(string code)
        {
            BaseRoleEntity result = null;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseRoleEntity.FieldCode, code),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0)
            };
            result = BaseEntity.Create<BaseRoleEntity>(ExecuteReader(parameters));

            return result;
        }

        /// <summary>
        /// 按名称获取实体
        /// </summary>
        /// <param name="realName">名称</param>
        public BaseRoleEntity GetObjectByName(string realName)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseRoleEntity.FieldRealName, realName),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0)
            };
            return BaseEntity.Create<BaseRoleEntity>(ExecuteReader(parameters));
        }

        /// <summary>
        /// 获取角色列表中的角色名称
        /// </summary>
        /// <param name="list">角色列表</param>
        /// <returns>角色名称</returns>
        public static string GetNames(List<BaseRoleEntity> list)
        {
            var result = string.Empty;

            foreach (var entity in list)
            {
                result += "," + entity.RealName;
            }
            if (!string.IsNullOrEmpty(result))
            {
                result = result.Substring(1);
            }

            return result;
        }

        #region public DataTable GetDataTableByOrganize(string organizeId) 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="organizeId">组织机构主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByOrganize(string organizeId)
        {
            var parametersList = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseRoleEntity.FieldOrganizeId, organizeId),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0)
            };
            return GetDataTable(parametersList, BaseRoleEntity.FieldSortCode);
            /*
            string sql = "SELECT " + BaseRoleEntity.TableName + ".*,"
                            + " (SELECT COUNT(*) FROM " + BaseUserRoleEntity.TableName + " WHERE (Enabled = 1) AND (RoleId = " + BaseRoleEntity.TableName + ".Id)) AS UserCount "
                            + " FROM " + BaseRoleEntity.TableName
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
                new KeyValuePair<string, object>(BaseRoleEntity.FieldRealName, roleName),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0)
            };
            return GetDataTable(parameters, BaseRoleEntity.FieldSortCode);
        }
        #endregion

        #region public DataTable Search(string organizeId, string searchKey,string categoryCode=null) 查询
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="organizeId">组织机构主键</param>
        /// <param name="searchKey">查询字符串</param>
        /// <param name="categoryCode">分类编号</param>
        /// <returns>数据表</returns>
        public DataTable Search(string organizeId, string searchKey, string categoryCode = null)
        {
            string sql = null;
            sql += "SELECT * FROM " + CurrentTableName + " WHERE " + BaseRoleEntity.FieldDeleted + " = 0 ";

            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetSearchString(searchKey);
                sql += string.Format("  AND ({0} LIKE '{1}' OR {2} LIKE '{3}')", BaseRoleEntity.FieldRealName, searchKey, BaseRoleEntity.FieldDescription, searchKey);
            }
            if (!string.IsNullOrEmpty(organizeId))
            {
                sql += string.Format(" AND {0} = '{1}'", BaseRoleEntity.FieldOrganizeId, organizeId);
            }
            if (!string.IsNullOrEmpty(categoryCode))
            {
                sql += string.Format(" AND {0} = '{1}'", BaseRoleEntity.FieldCategoryCode, categoryCode);
            }
            sql += " ORDER BY " + BaseRoleEntity.FieldSortCode;
            return DbHelper.Fill(sql);
        }
        #endregion

        #region public int MoveTo(string id, string targetOrganizeId) 移动
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="targetSystemId">目标主键</param>
        /// <returns>影响行数</returns>
        public int MoveTo(string id, string targetSystemId)
        {
            //return this.SetProperty(id, new KeyValuePair<string, object>(BaseRoleEntity.FieldSystemId, targetSystemId));
            return SetProperty(id, new KeyValuePair<string, object>(BaseRoleEntity.FieldOrganizeId, targetSystemId));
        }
        #endregion

        #region public int BatchMoveTo(string[] ids, string targetOrganizeId) 批量移动
        /// <summary>
        /// 批量移动
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="targetOrganizeId">目标主键</param>
        /// <returns>影响行数</returns>
        public int BatchMoveTo(string[] ids, string targetOrganizeId)
        {
            var result = 0;
            for (var i = 0; i < ids.Length; i++)
            {
                result += MoveTo(ids[i], targetOrganizeId);
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
                if (dr.RowState == DataRowState.Modified)
                {
                    string id = dr[BaseRoleEntity.FieldId, DataRowVersion.Original].ToString();
                    if (!string.IsNullOrEmpty(id))
                    {
                        roleEntity.GetFrom(dr);
                        result += this.UpdateObject(roleEntity);
                    }
                }
                // 添加状态
                if (dr.RowState == DataRowState.Added)
                {
                    roleEntity.GetFrom(dr);
                    result += this.AddObject(roleEntity).Length > 0 ? 1 : 0;
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
                result += UpdateObject(entity);
            }
            return result;
        }
        #endregion

        #region public int ResetSortCode(string organizeId) 重置排序码
        /// <summary>
        /// 重置排序码
        /// </summary>
        /// <param name="organizeId">组织机构主键</param>
        public int ResetSortCode(string organizeId)
        {
            var result = 0;
            var dt = GetDataTable();
            var id = string.Empty;
            var managerSequence = new BaseSequenceManager(DbHelper);
            var sortCode = managerSequence.GetBatchSequence(BaseRoleEntity.TableName, dt.Rows.Count);
            var i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                id = dr[BaseRoleEntity.FieldId].ToString();
                result += SetProperty(id, new KeyValuePair<string, object>(BaseRoleEntity.FieldSortCode, sortCode[i]));
                i++;
            }
            return result;
        }
        #endregion

        #region public DataTable GetDataTableByPage(string userId, string categoryCode, string serviceState, string searchKey, out int recordCount, int pageIndex = 0, int pageSize = 20, string sortExpression = null, string sortDirection = null)
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="categoryCode">分类编码</param>
        /// <param name="searchKey">查询字段</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(BaseUserInfo userInfo, string categoryCode, string searchKey, out int recordCount, int pageIndex = 0, int pageSize = 20, string sortExpression = null, string sortDirection = null)
        {
            var condition = BaseRoleEntity.FieldDeleted + " = 0 ";

            if (!string.IsNullOrEmpty(categoryCode))
            {
                condition += string.Format(" AND {0} = '{1}'", BaseRoleEntity.FieldCategoryCode, categoryCode);
            }

            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = string.Format("'{0}'", StringUtil.GetSearchString(searchKey));
                condition += string.Format(" AND ({0} LIKE {1}", BaseRoleEntity.FieldRealName, searchKey);
                condition += string.Format(" OR {0} LIKE {1}", BaseRoleEntity.FieldCategoryCode, searchKey);
                condition += string.Format(" OR {0} LIKE {1})", BaseRoleEntity.FieldCode, searchKey);
            }
            return GetDataTableByPage(out recordCount, pageIndex, pageSize, sortExpression, sortDirection, CurrentTableName, condition, null, "*");
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
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public DataTable GetUserDataTable(string systemCode, string roleId, string companyId, string userId, string searchKey, out int recordCount, int pageIndex, int pageSize, string orderBy)
        {
            var result = new DataTable(BaseUserEntity.TableName);

            var tableName = BaseUserRoleEntity.TableName;
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
                                    , UserRole.CreateOn
                                    , UserRole.CreateBy
                                    , UserRole.ModifiedOn
                                    , UserRole.ModifiedBy
 FROM BaseUser,
                          (SELECT UserId, Enabled, CreateOn, CreateBy, ModifiedOn, ModifiedBy
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
            // ORDER BY UserRole.CreateOn DESC ";
            commandText = commandText.Replace("BaseUserRole", tableName);
            var dbParameters = new List<IDbDataParameter>
            {
                DbHelper.MakeParameter(BaseUserRoleEntity.FieldRoleId, roleId)
            };

            if (!string.IsNullOrEmpty(companyId))
            {
                commandText += " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldCompanyId + " = " + DbHelper.GetParameter(BaseUserEntity.FieldCompanyId);
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldCompanyId, companyId));
            }
            if (!string.IsNullOrEmpty(userId))
            {
                commandText += " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldId + " = " + DbHelper.GetParameter(BaseUserEntity.FieldId);
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldId, userId));
            }
            commandText = "(" + commandText + ") T ";
            // 2015-12-05 吉日嘎拉 增加参数化功能
            result = DbUtil.GetDataTableByPage(DbHelper, out recordCount, commandText, "*", pageIndex, pageSize, null, dbParameters.ToArray(), orderBy);

            return result;
        }

        /// <summary>
        /// 获取角色权限范围（组织机构）
        /// 2015-11-28 吉日嘎拉 整理参数化
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>组织机构表</returns>
        public DataTable GetOrganizeDataTable(string systemCode, string roleId)
        {
            var result = new DataTable(BaseOrganizeEntity.TableName);

            var tableName = BaseRoleOrganizeEntity.TableName;
            if (!string.IsNullOrWhiteSpace(UserInfo.SystemCode))
            {
                tableName = UserInfo.SystemCode + "RoleOrganize";
            }

            var commandText = @"SELECT BaseOrganize.Id
                                    , BaseOrganize.Code
                                    , BaseOrganize.FullName 
                                    , BaseOrganize.Description 
                                    , RoleOrganize.Enabled
                                    , RoleOrganize.CreateOn
                                    , RoleOrganize.CreateBy
                                    , RoleOrganize.ModifiedOn
                                    , RoleOrganize.ModifiedBy
 FROM BaseOrganize RIGHT OUTER JOIN
                          (SELECT OrganizeId, Enabled, CreateOn, CreateBy, ModifiedOn, ModifiedBy
 FROM BaseRoleOrganize
                            WHERE RoleId = " + DbHelper.GetParameter(BaseRoleOrganizeEntity.FieldRoleId) +
                                " AND DeletionStateCode = " + DbHelper.GetParameter(BaseRoleOrganizeEntity.FieldDeleted) + @") RoleOrganize 
                            ON BaseOrganize.Id = RoleOrganize.OrganizeId
                         WHERE BaseOrganize.Enabled = 1 AND BaseOrganize." + BaseOrganizeEntity.FieldDeleted + @" = 0
                      ORDER BY RoleOrganize.CreateOn DESC ";

            commandText = commandText.Replace("BaseRoleOrganize", tableName);

            var dbParameters = new List<IDbDataParameter>
            {
                DbHelper.MakeParameter(BaseRoleOrganizeEntity.FieldRoleId, roleId),
                DbHelper.MakeParameter(BaseRoleOrganizeEntity.FieldDeleted, 0)
            };

            result = Fill(commandText, dbParameters.ToArray());

            return result;
        }

        /// <summary>
        /// 获取角色权限范围（组织机构）
        /// 2015-12-10 吉日嘎拉 整理参数化
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>组织机构列表</returns>
        public List<BaseOrganizeEntity> GetOrganizeList(string systemCode, string roleId)
        {
            var result = new List<BaseOrganizeEntity>();

            var tableName = BaseRoleOrganizeEntity.TableName;
            if (!string.IsNullOrWhiteSpace(UserInfo.SystemCode))
            {
                tableName = UserInfo.SystemCode + "RoleOrganize";
            }

            var commandText = @"SELECT *
     FROM BaseOrganize 
                                    WHERE BaseOrganize.Enabled = 1 
                                          AND BaseOrganize." + BaseOrganizeEntity.FieldDeleted + @" = 0  Id IN 
                                              (SELECT OrganizeId
                 FROM BaseRoleOrganize
                                                WHERE RoleId = " + DbHelper.GetParameter(BaseRoleOrganizeEntity.FieldRoleId)
                                                  + " AND Enabled = " + DbHelper.GetParameter(BaseRoleOrganizeEntity.FieldEnabled)
                                                  + " AND DeletionStateCode = " + DbHelper.GetParameter(BaseRoleOrganizeEntity.FieldDeleted) + ")";

            commandText = commandText.Replace("BaseRoleOrganize", tableName);

            var dbParameters = new List<IDbDataParameter>
            {
                DbHelper.MakeParameter(BaseRoleOrganizeEntity.FieldRoleId, roleId),
                DbHelper.MakeParameter(BaseRoleOrganizeEntity.FieldEnabled, 1),
                DbHelper.MakeParameter(BaseRoleOrganizeEntity.FieldDeleted, 0)
            };

            result = GetList<BaseOrganizeEntity>(DbHelper.ExecuteReader(commandText, dbParameters.ToArray()));

            return result;
        }

        #region public int Delete(string id) 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(string id)
        {
            var result = 0;

            // 删除角色权限结构定义
            // result = DbUtil.Delete(DbHelper, BaseRoleModuleOperationTable.TableName, BaseRoleModuleOperationTable.FieldRoleId, id);

            // 删除员工角色表结构定义部分
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldRoleId, id)
            };
            result += DbUtil.Delete(DbHelper, BaseUserRoleEntity.TableName, parameters);

            // 删除角色的表结构定义部分
            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseRoleEntity.FieldId, id),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldAllowDelete, 1)
            };
            result += DbUtil.Delete(DbHelper, CurrentTableName, parameters);

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

        /// <summary>
        /// 从缓存获取获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public static BaseRoleEntity GetObjectByCache(string id)
        {
            return GetObjectByCache("Base", id);
        }

        /// <summary>
        /// 从缓存获取获取实体
        /// </summary>
        /// <param name="userInfo">UserInfo</param>
        /// <param name="id">主键</param>
        public static BaseRoleEntity GetObjectByCache(BaseUserInfo userInfo, string id)
        {
            return GetObjectByCache(userInfo.SystemCode, id);
        }

        /// <summary>
        /// 从缓存获取获取实体
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="id"></param>
        /// <param name="refreshCache"></param>
        /// <returns></returns>
        public static BaseRoleEntity GetObjectByCache(string systemCode, string id, bool refreshCache = false)
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
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1)
                };
                return new BaseRoleManager(tableName).GetList<BaseRoleEntity>(parametersWhere, BaseRoleEntity.FieldId);
            }, true, refreshCache, cacheTime);
            result = listRole.Find(t => t.Id == id);
            //直接读取数据库
            //BaseRoleManager manager = new BaseRoleManager(tableName);
            //result = manager.GetObject(id);
            return result;
        }

        /// <summary>
        /// 从缓存获取获取实体
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="code">编号</param>
        /// <returns>权限实体</returns>
        public static BaseRoleEntity GetObjectByCacheByCode(string systemCode, string code)
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
                        new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0),
                        new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1)
                };
                return new BaseRoleManager(tableName).GetList<BaseRoleEntity>(parametersWhere, BaseRoleEntity.FieldId);
            }, true, false, cacheTime);
            result = listRole.Find(t => t.Code == code);
            //直接读取数据库
            //BaseRoleManager manager = new BaseRoleManager(tableName);
            //result = manager.GetObjectByCode(code);

            return result;
        }

        /// <summary>
        /// 从缓存获取获取实体
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="realName">名称</param>
        /// <returns>权限实体</returns>
        public static BaseRoleEntity GetObjectByCacheByName(string systemCode, string realName)
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
                        new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0),
                        new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1)
                };
                return new BaseRoleManager(tableName).GetList<BaseRoleEntity>(parametersWhere, BaseRoleEntity.FieldId);
            }, true, false, cacheTime);
            result = listRole.Find(t => t.RealName == realName);
            //直接读取数据库
            //BaseRoleManager manager = new BaseRoleManager(tableName);
            //result = manager.GetObjectByName(realName);

            return result;
        }

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

        #region 删除缓存

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <returns></returns>
        public override bool RemoveCache()
        {
            var result = false;
            var cacheKey = "DataTable." + CurrentTableName;
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
