//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNet.Business
{
    using IService;
    using Model;
    using Util;

    /// <summary>
    /// PermissionService
    /// 权限判断服务
    /// 
    /// 修改记录
    /// 
    ///		2012.03.22 版本：1.0 JiRiGaLa 创建权限判断服务。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2012.03.22</date>
    /// </author> 
    /// </summary>
    public partial class PermissionService : IPermissionService
    {
        /// <summary>
        /// 获取用户的权限范围列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionId">操作权限主键</param>
        /// <returns>权限范围列表</returns>
        public List<BasePermissionScopeEntity> GetUserPermissionScopeList(BaseUserInfo userInfo, string userId, string permissionId)
        {
            var result = new List<BasePermissionScopeEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.TableName),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldDeleted, 0)
                };

                var tableName = userInfo.SystemCode + "PermissionScope";
                var permissionScopeManager = new BasePermissionScopeManager(dbHelper, userInfo, tableName);
                result = permissionScopeManager.GetList<BasePermissionScopeEntity>(parameters);
            });

            return result;
        }

        /// <summary>
        /// 获取角色的权限范围列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionId">操作权限主键</param>
        /// <returns>权限范围列表</returns>
        public List<BasePermissionScopeEntity> GetRolePermissionScopeList(BaseUserInfo userInfo, string roleId, string permissionId)
        {
            var result = new List<BasePermissionScopeEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var roleTableName = BaseRoleEntity.TableName;
                if (userInfo != null && !string.IsNullOrEmpty(userInfo.SystemCode))
                {
                    roleTableName = userInfo.SystemCode + "Role";
                }

                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, roleTableName),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, roleId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldDeleted, 0)
                };

                var tableName = userInfo.SystemCode + "PermissionScope";
                var permissionScopeManager = new BasePermissionScopeManager(dbHelper, userInfo, tableName);
                result = permissionScopeManager.GetList<BasePermissionScopeEntity>(parameters);
            });

            return result;
        }

        #region 资源权限设定关系相关

        #region public string[] GetResourcePermissionIds(BaseUserInfo userInfo, string resourceCategory, string resourceId)
        /// <summary>
        /// 获取资源权限主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源分类</param>
        /// <param name="resourceId"></param>
        /// <returns>主键数组</returns>
        public string[] GetResourcePermissionIds(BaseUserInfo userInfo, string resourceCategory, string resourceId)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, resourceCategory),
                    new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, resourceId)
                };

                var dt = DbUtil.GetDataTable(dbHelper, BasePermissionEntity.TableName, parameters);
                result = BaseUtil.FieldToArray(dt, BasePermissionEntity.FieldPermissionId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
            });
            return result;
        }
        #endregion

        #region public int GrantResourcePermission(BaseUserInfo userInfo, string resourceCategory, string resourceId, string[] grantPermissionIds)
        /// <summary>
        /// 授予资源的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源分类</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="grantPermissionIds">权限主键</param>
        /// <returns>影响的行数</returns>
        public int GrantResourcePermission(BaseUserInfo userInfo, string resourceCategory, string resourceId, string[] grantPermissionIds)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseRoleScopeManager(dbHelper, userInfo);
                // 小心异常，检查一下参数的有效性
                if (grantPermissionIds != null)
                {
                    var permissionManager = new BasePermissionManager(dbHelper, userInfo);
                    for (var i = 0; i < grantPermissionIds.Length; i++)
                    {
                        var resourcePermissionEntity = new BasePermissionEntity
                        {
                            ResourceCategory = resourceCategory,
                            ResourceId = resourceId,
                            PermissionId = grantPermissionIds[i],
                            Enabled = 1,
                            DeletionStateCode = 0
                        };
                        permissionManager.Add(resourcePermissionEntity);
                        result++;
                    }
                }
            });
            return result;
        }
        #endregion

        #region public int RevokeResourcePermission(BaseUserInfo userInfo, string resourceCategory, string resourceId, string[] revokePermissionIds)
        /// <summary>
        /// 撤消资源的权限
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源分类</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="revokePermissionIds">权限主键</param>
        /// <returns>影响的行数</returns>
        public int RevokeResourcePermission(BaseUserInfo userInfo, string resourceCategory, string resourceId, string[] revokePermissionIds)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // BaseRoleScopeManager manager = new BaseRoleScopeManager(dbHelper, result);
                // 小心异常，检查一下参数的有效性
                if (revokePermissionIds != null)
                {
                    var permissionManager = new BasePermissionManager(dbHelper, userInfo);
                    for (var i = 0; i < revokePermissionIds.Length; i++)
                    {
                        var parameters = new List<KeyValuePair<string, object>>
                        {
                            new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, resourceCategory),
                            new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, resourceId),
                            new KeyValuePair<string, object>(BasePermissionEntity.FieldPermissionId, revokePermissionIds[i])
                        };
                        // result += permissionManager.SetDeleted(parameters);
                        result += permissionManager.Delete(parameters);
                    }
                }
            });
            return result;
        }
        #endregion

        #endregion

        #region 资源权限范围设定关系相关

        #region public string[] GetPermissionScopeTargetIds(BaseUserInfo userInfo, string resourceCategory, string resourceId, string targetCategory, string permissionCode)
        /// <summary>
        /// 获取资源权限范围主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源分类</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="targetCategory">目标类别</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetPermissionScopeTargetIds(BaseUserInfo userInfo, string resourceCategory, string resourceId, string targetCategory, string permissionCode)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var permissionId = BaseModuleManager.GetIdByCodeByCache(userInfo.SystemCode, permissionCode);

                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, resourceCategory),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, resourceId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, targetCategory),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldDeleted, 0)
                };

                var tableName = userInfo.SystemCode + "PermissionScope";
                result = DbUtil.GetProperties(dbHelper, tableName, parameters, 0, BasePermissionScopeEntity.FieldTargetId);
            });
            return result;
        }
        #endregion

        #region public string[] GetPermissionScopeResourceIds(BaseUserInfo userInfo, string resourceCategory, string targetId, string targetResourceCategory, string permissionCode)
        /// <summary>
        /// GetPermissionScopeResourceIds
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="resourceCategory"></param>
        /// <param name="targetId"></param>
        /// <param name="targetResourceCategory"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public string[] GetPermissionScopeResourceIds(BaseUserInfo userInfo, string resourceCategory, string targetId, string targetResourceCategory, string permissionCode)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var permissionId = BaseModuleManager.GetIdByCodeByCache(userInfo.SystemCode, permissionCode);

                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, targetId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, resourceCategory),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, targetResourceCategory),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldDeleted, 0)
                };

                var tableName = userInfo.SystemCode + "PermissionScope";
                result = DbUtil.GetProperties(dbHelper, tableName, parameters, 0, BasePermissionScopeEntity.FieldResourceId);
            });
            return result;
        }
        #endregion

        #region public int GrantPermissionScopeTarget(BaseUserInfo userInfo, string resourceCategory, string resourceId, string targetCategory, string[] grantTargetIds, string result)
        /// <summary>
        /// 授予资源的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源分类</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="targetCategory">目标类别</param>
        /// <param name="grantTargetIds">目标主键数组</param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>影响的行数</returns>
        public int GrantPermissionScopeTargets(BaseUserInfo userInfo, string resourceCategory, string resourceId, string targetCategory, string[] grantTargetIds, string permissionId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var permissionScopeManager = new BasePermissionScopeManager(dbHelper, userInfo, tableName);
                result = permissionScopeManager.GrantResourcePermissionScopeTarget(resourceCategory, resourceId, targetCategory, grantTargetIds, permissionId);
            });
            return result;
        }
        #endregion

        #region public int GrantPermissionScopeTarget(BaseUserInfo userInfo, string resourceCategory, string[] resourceIds, string targetCategory, string grantTargetId, string result)
        /// <summary>
        /// GrantPermissionScopeTarget
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="resourceCategory"></param>
        /// <param name="resourceIds"></param>
        /// <param name="targetCategory"></param>
        /// <param name="grantTargetId"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public int GrantPermissionScopeTarget(BaseUserInfo userInfo, string resourceCategory, string[] resourceIds, string targetCategory, string grantTargetId, string permissionId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BasePermissionScopeManager(dbHelper, userInfo, tableName);
                result = manager.GrantResourcePermissionScopeTarget(resourceCategory, resourceIds, targetCategory, grantTargetId, permissionId);
            });
            return result;
        }
        #endregion

        #region public int RevokePermissionScopeTargets(BaseUserInfo userInfo, string resourceCategory, string resourceId, string targetCategory, string[] revokeTargetIds, string result)
        /// <summary>
        /// 撤消资源的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源分类</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="targetCategory">目标类别</param>
        /// <param name="revokeTargetIds">目标主键数组</param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>影响的行数</returns>
        public int RevokePermissionScopeTargets(BaseUserInfo userInfo, string resourceCategory, string resourceId, string targetCategory, string[] revokeTargetIds, string permissionId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BasePermissionScopeManager(dbHelper, userInfo, tableName);
                result = manager.RevokeResourcePermissionScopeTarget(resourceCategory, resourceId, targetCategory, revokeTargetIds, permissionId);
            });
            return result;
        }
        #endregion

        #region public int RevokePermissionScopeTarget(BaseUserInfo userInfo, string resourceCategory, string[] resourceIds, string targetCategory, string revokeTargetId, string result)
        /// <summary>
        /// 移除权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源分类</param>
        /// <param name="resourceIds">资源主键</param>
        /// <param name="targetCategory">目标分类</param>
        /// <param name="revokeTargetId">目标主键</param>
        /// <param name="permissionId">操作权限项</param>
        /// <returns>影响行数</returns>
        public int RevokePermissionScopeTarget(BaseUserInfo userInfo, string resourceCategory, string[] resourceIds, string targetCategory, string revokeTargetId, string permissionId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BasePermissionScopeManager(dbHelper, userInfo, tableName);
                result = manager.RevokeResourcePermissionScopeTarget(resourceCategory, resourceIds, targetCategory, revokeTargetId, permissionId);
            });
            return result;
        }
        #endregion

        #region public int ClearPermissionScopeTarget(BaseUserInfo userInfo, string resourceCategory, string resourceId, string targetCategory, string result)
        /// <summary>
        /// 撤消资源的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源分类</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="targetCategory">目标类别</param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>影响的行数</returns>
        public int ClearPermissionScopeTarget(BaseUserInfo userInfo, string resourceCategory, string resourceId, string targetCategory, string permissionId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BasePermissionScopeManager(dbHelper, userInfo, tableName);
                result = manager.RevokeResourcePermissionScopeTarget(resourceCategory, resourceId, targetCategory, permissionId);
            });
            return result;
        }
        #endregion

        #endregion

        #region public string[] GetResourceScopeIds(BaseUserInfo userInfo, string userId, string targetCategory, string permissionCode)
        /// <summary>
        /// 获取用户的某个资源的权限范围
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="targetCategory">目标类别</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetResourceScopeIds(BaseUserInfo userInfo, string userId, string targetCategory, string permissionCode)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BasePermissionScopeManager(dbHelper, userInfo, tableName);
                result = manager.GetResourceScopeIds(userInfo.SystemCode, userId, targetCategory, permissionCode);
            });
            return result;
        }
        #endregion

        #region public string[] GetTreeResourceScopeIds(BaseUserInfo userInfo, string userId, string targetCategory, string permissionCode, bool childrens)
        /// <summary>
        /// 60.获取用户的某个资源的权限范围(树型资源)
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="targetCategory">目标类别</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="childrens">是否含子节点</param>
        /// <returns>主键数组</returns>
        public string[] GetTreeResourceScopeIds(BaseUserInfo userInfo, string userId, string targetCategory, string permissionCode, bool childrens)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "PermissionScope";
                var manager = new BasePermissionScopeManager(dbHelper, userInfo, tableName);
                result = manager.GetTreeResourceScopeIds(userInfo.SystemCode, userId, targetCategory, permissionCode, childrens);
            });
            return result;
        }
        #endregion
    }
}