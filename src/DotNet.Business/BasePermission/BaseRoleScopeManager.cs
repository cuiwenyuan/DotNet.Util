//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseRoleScopeManager
    /// 角色权限域
    /// 
    /// 修改记录
    ///
    ///     2008.05.24 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2008.05.24</date>
    /// </author>
    /// </summary>
    public partial class BaseRoleScopeManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseRoleScopeManager()
        {
            CurrentTableName = BasePermissionScopeEntity.TableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper"></param>
        public BaseRoleScopeManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo"></param>
        public BaseRoleScopeManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="userInfo"></param>
        public BaseRoleScopeManager(IDbHelper dbHelper, BaseUserInfo userInfo)
            : this(dbHelper)
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="userInfo"></param>
        /// <param name="tableName"></param>
        public BaseRoleScopeManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 清除角色权限范围
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int ClearRolePermissionScope(string systemCode, string roleId, string permissionCode)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseRoleEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, roleId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode))
            };

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo);
            return permissionScopeManager.Delete(parameters);
        }

        /// <summary>
        /// 撤回所有
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public int RevokeAll(string systemCode, string roleId)
        {
            var tableName = systemCode + "PermissionScope";
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseRoleEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, roleId)
            };
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, tableName);
            return permissionScopeManager.Delete(parameters);
        }
    }
}