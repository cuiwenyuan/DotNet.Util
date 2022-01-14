//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseOrganizePermissionScopeManager
    /// 组织机构权限
    /// 
    /// 修改记录
    ///
    ///     2012.03.22 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.03.22</date>
    /// </author>
    /// </summary>
    public partial class BaseOrganizePermissionScopeManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseOrganizePermissionScopeManager()
        {
            CurrentTableName = BasePermissionScopeEntity.TableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper"></param>
        public BaseOrganizePermissionScopeManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo"></param>
        public BaseOrganizePermissionScopeManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="userInfo"></param>
        public BaseOrganizePermissionScopeManager(IDbHelper dbHelper, BaseUserInfo userInfo)
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
        public BaseOrganizePermissionScopeManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public string GetIdByCode(string permissionCode)
        {
            var tableName = UserInfo.SystemCode + "Module";
            var moduleManager = new BaseModuleManager(dbHelper, UserInfo, tableName);
            return moduleManager.GetIdByCode(permissionCode);
            //BasePermissionManager moduleManager = new BasePermissionManager(DbHelper);
            //// 这里应该是若不存在就自动加一个操作权限
            //return moduleManager.GetIdByAdd(permissionCode);
        }

        /// <summary>
        /// 清空组织权限范围
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int ClearOrganizePermissionScope(string organizationId, string permissionCode)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseOrganizeEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, organizationId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, GetIdByCode(permissionCode))
            };

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            return permissionScopeManager.Delete(parameters);
        }

        /// <summary>
        /// 撤回所有
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public int RevokeAll(string organizationId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseOrganizeEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, organizationId)
            };
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            return permissionScopeManager.Delete(parameters);
        }

        ////
        ////
        //// 授权范围管理部分
        ////
        ////

        #region public string[] GetModuleIds(string organizationId, string permissionCode) 获取员工的权限主键数组
        /// <summary>
        /// 获取员工的权限主键数组
        /// </summary>
        /// <param name="organizationId">员工主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetModuleIds(string organizationId, string permissionCode)
        {
            string[] result = null;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseOrganizeEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, organizationId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseModuleEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, GetIdByCode(permissionCode))
            };

            var dt = GetDataTable(parameters);
            result = BaseUtil.FieldToArray(dt, BasePermissionScopeEntity.FieldTargetId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
            return result;
        }
        #endregion

        //
        // 授予授权范围的实现部分
        //

        #region private string GrantModule(BasePermissionScopeManager manager, string id, string organizationId, string grantModuleId) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="permissionScopeManager">权限域读写器</param>
        /// <param name="organizationId">员工主键</param>
        /// <param name="grantModuleId">权限主键</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>主键</returns>
        private string GrantModule(BasePermissionScopeManager permissionScopeManager, string organizationId, string grantModuleId, string permissionCode)
        {
            var result = string.Empty;
            var resourcePermissionScopeEntity = new BasePermissionScopeEntity
            {
                PermissionId = GetIdByCode(permissionCode),
                ResourceCategory = BaseOrganizeEntity.TableName,
                ResourceId = organizationId,
                TargetCategory = BaseModuleEntity.TableName,
                TargetId = grantModuleId,
                Enabled = 1,
                DeletionStateCode = 0
            };
            return permissionScopeManager.Add(resourcePermissionScopeEntity);
        }
        #endregion

        #region public string GrantModule(string organizationId, string result) 员工授予权限
        /// <summary>
        /// 员工授予权限
        /// </summary>
        /// <param name="organizationId">员工主键</param>
        /// <param name="grantModuleId">权限主键</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>主键</returns>
        public string GrantModule(string organizationId, string grantModuleId, string permissionCode)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            return GrantModule(permissionScopeManager, organizationId, grantModuleId, permissionCode);
        }
        #endregion

        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="grantModuleIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantModules(string organizationId, string[] grantModuleIds, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < grantModuleIds.Length; i++)
            {
                GrantModule(permissionScopeManager, organizationId, grantModuleIds[i], permissionCode);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="organizationIds"></param>
        /// <param name="grantModuleId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantModules(string[] organizationIds, string grantModuleId, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < organizationIds.Length; i++)
            {
                GrantModule(permissionScopeManager, organizationIds[i], grantModuleId, permissionCode);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="organizationIds"></param>
        /// <param name="grantModuleIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantModules(string[] organizationIds, string[] grantModuleIds, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < organizationIds.Length; i++)
            {
                for (var j = 0; j < grantModuleIds.Length; j++)
                {
                    GrantModule(permissionScopeManager, organizationIds[i], grantModuleIds[j], permissionCode);
                    result++;
                }
            }
            return result;
        }


        //
        //  撤销授权范围的实现部分
        //

        #region private int RevokeModule(BasePermissionScopeManager manager, string organizationId, string revokeModuleId, string permissionCode) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="permissionScopeManager">权限域读写器</param>
        /// <param name="organizationId">员工主键</param>
        /// <param name="revokeModuleId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        private int RevokeModule(BasePermissionScopeManager permissionScopeManager, string organizationId, string revokeModuleId, string permissionCode)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseOrganizeEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, organizationId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseModuleEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, revokeModuleId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, GetIdByCode(permissionCode))
            };
            return permissionScopeManager.Delete(parameters);
        }
        #endregion

        #region public int RevokeModule(string organizationId, string result) 员工撤销授权
        /// <summary>
        /// 员工撤销授权
        /// </summary>
        /// <param name="organizationId">员工主键</param>
        /// <param name="revokeModuleId">权限主键</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>主键</returns>
        public int RevokeModule(string organizationId, string revokeModuleId, string permissionCode)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            return RevokeModule(permissionScopeManager, organizationId, revokeModuleId, permissionCode);
        }
        #endregion

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="revokeModuleIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeModules(string organizationId, string[] revokeModuleIds, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < revokeModuleIds.Length; i++)
            {
                RevokeModule(permissionScopeManager, organizationId, revokeModuleIds[i], permissionCode);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="organizationIds"></param>
        /// <param name="revokeModuleId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeModules(string[] organizationIds, string revokeModuleId, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < organizationIds.Length; i++)
            {
                RevokeModule(permissionScopeManager, organizationIds[i], revokeModuleId, permissionCode);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="organizationIds"></param>
        /// <param name="revokeModuleIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeModules(string[] organizationIds, string[] revokeModuleIds, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < organizationIds.Length; i++)
            {
                for (var j = 0; j < revokeModuleIds.Length; j++)
                {
                    RevokeModule(permissionScopeManager, organizationIds[i], revokeModuleIds[j], permissionCode);
                    result++;
                }
            }
            return result;
        }
    }
}