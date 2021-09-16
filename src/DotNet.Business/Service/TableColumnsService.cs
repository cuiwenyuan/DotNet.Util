//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Reflection;


namespace DotNet.Business
{
    using IService;
    using Model;
    using Util;

    /// <summary>
    /// TableColumnsService
    /// 表字段结构
    /// 
    /// 修改记录
    /// 
    ///		2011.05.16 版本：1.0 JiRiGaLa 创建代码。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.05.16</date>
    /// </author> 
    /// </summary>


    public class TableColumnsService : ITableColumnsService
    {
        /// <summary>
        /// 获取用户的列权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableCode">表名</param>
        /// <param name="permissionCode">操作权限</param>
        /// <returns>有权限的列数组</returns>
        public string[] GetColumns(BaseUserInfo userInfo, string tableCode, string permissionCode = "Column.Access")
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得列表
                var manager = new BaseTableColumnsManager(dbHelper, userInfo);
                result = manager.GetColumns(userInfo.Id, tableCode, permissionCode);
            });
            return result;
        }

        /// <summary>
        /// 按表名获取字段列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableCode">表名</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByTable(BaseUserInfo userInfo, string tableCode)
        {
            var dt = new DataTable(BaseTableColumnsEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得列表
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseTableColumnsEntity.FieldTableCode, tableCode),
                    new KeyValuePair<string, object>(BaseTableColumnsEntity.FieldUseConstraint, 1),
                    new KeyValuePair<string, object>(BaseTableColumnsEntity.FieldDeleted, 0)
                };

                var manager = new BaseTableColumnsManager(dbHelper, userInfo);
                dt = manager.GetDataTable(parameters, BaseTableColumnsEntity.FieldSortCode);
                dt.TableName = BaseTableColumnsEntity.TableName;
            });
            return dt;
        }

        /// <summary>
        /// 获取约束条件（所有的约束）
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源类别</param>
        /// <param name="resourceId">资源主键</param>
        /// <returns>数据表</returns>
        public DataTable GetConstraintDT(BaseUserInfo userInfo, string resourceCategory, string resourceId)
        {
            var dt = new DataTable(BaseTableColumnsEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得列表
                var manager = new BaseTableColumnsManager(dbHelper, userInfo);
                dt = manager.GetConstraintDt(resourceCategory, resourceId);
                dt.TableName = BaseTableColumnsEntity.TableName;
            });
            return dt;
        }

        /// <summary>
        /// 获取当前用户的件约束表达式
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <returns>主键</returns>
        public string GetUserConstraint(BaseUserInfo userInfo, string tableName)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseTableColumnsManager(dbHelper, userInfo);
                result = manager.GetUserConstraint(tableName);
            });
            return result;
        }

        /// <summary>
        /// 设置约束条件
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源类别</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="tableName">表名</param>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="constraint">约束</param>
        /// <param name="enabled">有效</param>
        /// <returns>主键</returns>
        public string SetConstraint(BaseUserInfo userInfo, string resourceCategory, string resourceId, string tableName, string permissionCode, string constraint, bool enabled = true)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseTableColumnsManager(dbHelper, userInfo);
                result = manager.SetConstraint(resourceCategory, resourceId, tableName, permissionCode, constraint, enabled);
            });
            return result;
        }

        /// <summary>
        /// 获取约束条件
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源类别</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="tableName">表名</param>
        /// <returns>约束条件</returns>
        public string GetConstraint(BaseUserInfo userInfo, string resourceCategory, string resourceId, string tableName)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseTableColumnsManager(dbHelper, userInfo);
                result = manager.GetConstraint(resourceCategory, resourceId, tableName);
            });
            return result;
        }

        /// <summary>
        /// 获取约束条件
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源类别</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="tableName">表名</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>约束条件</returns>
        public BasePermissionScopeEntity GetConstraintEntity(BaseUserInfo userInfo, string resourceCategory, string resourceId, string tableName, string permissionCode = "Resource.AccessPermission")
        {
            BasePermissionScopeEntity result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseTableColumnsManager(dbHelper, userInfo);
                result = manager.GetConstraintEntity(resourceCategory, resourceId, tableName, permissionCode);
            });
            return result;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int BatchDeleteConstraint(BaseUserInfo userInfo, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BasePermissionScopeManager(dbHelper, userInfo);
                result = manager.SetDeleted(ids);
            });

            return result;
        }
    }
}