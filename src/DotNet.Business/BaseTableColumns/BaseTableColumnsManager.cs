//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseTableColumnsManager
    /// 表字段定义
    ///
    /// 修改记录
    ///
    ///		2012-02-06 版本：1.1 JiRiGaLa 把字段权限整理完善。
    ///		2010-07-14 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2010-07-14</date>
    /// </author>
    /// </summary>
    public partial class BaseTableColumnsManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 获取用户的列权限
        /// </summary>
        /// <param name="tableCode">表名</param>
        /// <param name="permissionCode">操作权限</param>
        /// <returns>有权限的列数组</returns>
        public string[] GetColumns(string tableCode, string permissionCode = "Column.Access")
        {
            return GetColumns(UserInfo.Id, tableCode, permissionCode);
        }

        /// <summary>
        /// 获取用户的列权限
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="tableCode">表名</param>
        /// <param name="permissionCode">操作权限</param>
        /// <returns>有权限的列数组</returns>
        public string[] GetColumns(string userId, string tableCode, string permissionCode = "Column.Access")
        {
            // Column.Edit
            string[] result = null;
            if (permissionCode.Equals("Column.Deney") || permissionCode.Equals("Column.Edit"))
            {
                // 按数据权限来过滤数据
                var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName); ;
                result = permissionScopeManager.GetResourceScopeIds(UserInfo.SystemCode, userId, tableCode, permissionCode);
            }
            else if (permissionCode.Equals("Column.Access"))
            {
                // 1: 用户有权限的列名
                var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName); ;
                result = permissionScopeManager.GetResourceScopeIds(UserInfo.SystemCode, userId, tableCode, permissionCode);
                // 2: 先获取公开的列名
                var publicIds = GetProperties(new KeyValuePair<string, object>(BaseTableColumnsEntity.FieldTableCode, tableCode), new KeyValuePair<string, object>(BaseTableColumnsEntity.FieldIsPublic, 1), BaseTableColumnsEntity.FieldColumnCode);
                result = StringUtil.Concat(result, publicIds);
            }
            return result;
        }

        /// <summary>
        /// 获取能访问的字段列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tableCode">表名</param>
        /// <returns>数据表</returns>
        public DataTable GetTableColumns(string userId, string tableCode)
        {
            // 当前用户对哪些资源有权限（用户自己的权限 + 相应的角色拥有的权限）
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName); ;
            var ids = permissionScopeManager.GetResourceScopeIds(UserInfo.SystemCode, userId, "TableColumns", "ColumnAccess");

            // 获取有效的，没删除的
            var sql = "SELECT * FROM BaseTableColumns WHERE (" + BaseTableColumnsEntity.FieldDeleted + " = 0 AND Enabled = 1) ";

            // 是否指定了表名
            if (!string.IsNullOrEmpty(tableCode))
            {
                sql += " AND (TableCode = '" + tableCode + "') ";
            }

            // 公开的或者按权限来过滤字段
            sql += " AND (IsPublic = 1 ";
            if (ids != null && ids.Length > 0)
            {
                var idList = StringUtil.ArrayToList(ids);
                sql += " OR Id IN (" + idList + ")";
            }
            sql += ") ORDER BY SortCode ";

            return DbHelper.Fill(sql);
        }

        /// <summary>
        /// 获取约束条件（所有的约束）
        /// </summary>
        /// <param name="resourceCategory">资源类别</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>数据表</returns>
        public DataTable GetConstraintDt(string resourceCategory, string resourceId, string permissionCode = "Resource.AccessPermission")
        {
            var dt = new DataTable(BaseTableColumnsEntity.TableName);

            /*
            -- 这里是都有哪些表？
            SELECT ItemValue, ItemName
 FROM ItemsTablePermissionScope
            WHERE (" + BaseUserEntity.FieldDeleted + " = 0) 
            AND (Enabled = 1)
            ORDER BY ItemsTablePermissionScope.SortCode
             */

            /*
            -- 这里是都有有哪些表达式
            SELECT     Id, TargetId, PermissionConstraint   -- 对什么表有什么表达式？
 FROM         BasePermissionScope
            WHERE (ResourceId = 10000000) 
            AND (ResourceCategory = 'BaseRole')   -- 什么角色？
            AND (TargetId = 'BaseUser') 
            AND (TargetCategory = 'Table') 
            AND (PermissionId = 10000001)  -- 有什么权限？（资源访问权限）
            AND (" + BaseBasePermissionScopeEntity.FieldDeleted + " = 0) 
            AND (Enabled = 1)
             */

            var permissionId = BaseModuleManager.GetIdByCodeByCache(UserInfo.SystemCode, permissionCode);

            var sql = @"SELECT BasePermissionScope.Id
		                                    , ItemsTablePermissionScope.ItemValue AS TableCode
		                                    , ItemsTablePermissionScope.ItemName AS TableName
		                                    , BasePermissionScope.PermissionConstraint
		                                    , ItemsTablePermissionScope.SortCode
    FROM  (
	                                    SELECT ItemValue
		                                     , ItemName
		                                     , SortCode
	    FROM ItemsTablePermissionScope
                                       WHERE (" + BasePermissionEntity.FieldDeleted + @" = 0) 

                                              AND (Enabled = 1)                                              
                                        ) AS ItemsTablePermissionScope LEFT OUTER JOIN
                                        (SELECT Id
			                                    , TargetId
			                                    , PermissionConstraint  
           FROM BasePermissionScope
                                         WHERE (ResourceCategory = '" + resourceCategory + @"') 
			                                    AND (ResourceId = " + resourceId + @") 
			                                    AND (TargetCategory = 'Table') 
			                                    AND (PermissionId = " + permissionId + @") 
			                                    AND (" + BasePermissionScopeEntity.FieldDeleted + @" = 0) 

                                                AND (Enabled = 1)
	                                     ) AS BasePermissionScope 
                                    ON ItemsTablePermissionScope.ItemValue = BasePermissionScope.TargetId
                                    ORDER BY ItemsTablePermissionScope.SortCode ";

            dt = Fill(sql);
            dt.TableName = BaseTableColumnsEntity.TableName;

            return dt;
        }


        /// <summary>
        /// 获取用户的件约束表达式
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>主键</returns>
        public string GetUserConstraint(string tableName, string permissionCode = "Resource.AccessPermission")
        {
            var result = string.Empty;
            // 这里是获取用户的条件表达式
            // 1: 首先用户在哪些角色里是有效的？
            // 2: 这些角色都有哪些哪些条件约束？
            // 3: 组合约束条件？
            // 4：用户本身的约束条件？
            var permissionId = BaseModuleManager.GetIdByCodeByCache(UserInfo.SystemCode, permissionCode);

            var manager = new BaseUserManager(DbHelper, UserInfo);
            var roleIds = manager.GetRoleIds(UserInfo.Id);
            if (roleIds == null || roleIds.Length == 0)
            {
                return result;
            }
            var scopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseRoleEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, roleIds),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, "Table"),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, tableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldDeleted, 0)
            };

            var dtPermissionScope = scopeManager.GetDataTable(parameters);
            var permissionConstraint = string.Empty;
            foreach (DataRow dr in dtPermissionScope.Rows)
            {
                permissionConstraint = dr[BasePermissionScopeEntity.FieldPermissionConstraint].ToString();
                permissionConstraint = permissionConstraint.Trim();
                if (!string.IsNullOrEmpty(permissionConstraint))
                {
                    result += " AND " + permissionConstraint;
                }
            }
            if (!string.IsNullOrEmpty(result))
            {
                result = result.Substring(5);
                // 解析替换约束表达式标准函数
                result = ConstraintUtil.PrepareParameter(UserInfo, result);
            }

            return result;
        }

        /// <summary>
        /// 设置约束条件
        /// </summary>
        /// <param name="resourceCategory">资源类别</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="tableName">表名</param>
        /// <param name="constraint">约束</param>
        /// <param name="enabled">有效</param>
        /// <param name="permissionCode">操作权限项</param>
        /// <returns>主键</returns>
        public string SetConstraint(string resourceCategory, string resourceId, string tableName, string permissionCode, string constraint, bool enabled = true)
        {
            var result = string.Empty;

            var permissionId = string.Empty;
            permissionId = BaseModuleManager.GetIdByCodeByCache(UserInfo.SystemCode, permissionCode);

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, resourceCategory),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, resourceId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, "Table"),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, tableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldDeleted, 0)
            };

            var manager = new BasePermissionScopeManager(DbHelper, UserInfo);
            // 1:先获取是否有这样的主键，若有进行更新操作。
            // 2:若没有进行添加操作。
            result = manager.GetId(parameters);
            if (!string.IsNullOrEmpty(result))
            {
                parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionConstraint, constraint),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldEnabled, enabled ? 1 : 0)
                };
                manager.SetProperty(new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldId, result), parameters);
            }
            else
            {
                var entity = new BasePermissionScopeEntity
                {
                    ResourceCategory = resourceCategory,
                    ResourceId = resourceId,
                    TargetCategory = "Table",
                    TargetId = tableName,
                    PermissionConstraint = constraint,
                    PermissionId = permissionId,
                    DeletionStateCode = 0,
                    Enabled = enabled ? 1 : 0
                };
                result = manager.Add(entity);
            }
            return result;
        }

        /// <summary>
        /// 获取约束条件
        /// </summary>
        /// <param name="resourceCategory">资源类别</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="tableName">表名</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>约束条件</returns>
        public string GetConstraint(string resourceCategory, string resourceId, string tableName, string permissionCode = "Resource.AccessPermission")
        {
            var result = string.Empty;
            var entity = GetConstraintEntity(resourceCategory, resourceId, tableName, permissionCode);
            if (entity != null)
            {
                if (entity.Enabled == 1)
                {
                    result = entity.PermissionConstraint;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取约束实体
        /// </summary>
        /// <param name="resourceCategory"></param>
        /// <param name="resourceId"></param>
        /// <param name="tableName"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public BasePermissionScopeEntity GetConstraintEntity(string resourceCategory, string resourceId, string tableName, string permissionCode = "Resource.AccessPermission")
        {
            BasePermissionScopeEntity entity = null;

            var permissionId = string.Empty;
            permissionId = BaseModuleManager.GetIdByCodeByCache(UserInfo.SystemCode, permissionCode);

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, resourceCategory),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, resourceId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, "Table"),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, tableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldDeleted, 0)
            };

            // 1:先获取是否有这样的主键，若有进行更新操作。
            var manager = new BasePermissionScopeManager(DbHelper, UserInfo);
            var dt = manager.GetDataTable(parameters);
            if (dt != null && dt.Rows.Count > 0)
            {
                entity = BaseEntity.Create<BasePermissionScopeEntity>(dt);
            }
            return entity;
        }
    }
}