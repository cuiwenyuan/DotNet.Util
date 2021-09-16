//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;

    /// <summary>
    /// BaseUserScopeManager
    /// 用户对用户的权限域
    /// 
    /// 修改记录
    ///
    ///     2011.03.13 版本：2.0 JiRiGaLa 重新整理代码。
    ///     2008.05.24 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.03.13</date>
    /// </author>
    /// </summary>
    public partial class BaseUserScopeManager : BaseManager, IBaseManager
    {
        ////
        ////
        //// 授权范围管理部分
        ////
        ////

        #region public string[] GetUserIds(string systemCode, string userId, string permissionCode) 获取员工的权限主键数组
        /// <summary>
        /// 获取员工的权限主键数组
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetUserIds(string systemCode, string userId, string permissionCode)
        {
            string[] result = null;

            var permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
            if (!string.IsNullOrEmpty(permissionId))
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.TableName),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseUserEntity.TableName),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId)
                };

                // 20130605 JiRiGaLa 这个运行效率更高一些
                result = GetProperties(parameters, BasePermissionScopeEntity.FieldTargetId);
                // var result = this.GetDataTable(parameters);
                // result = BaseUtil.FieldToArray(result, BasePermissionScopeEntity.FieldTargetId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
            }
            return result;
        }
        #endregion
        /// <summary>
        /// 获取员工的权限主键数组
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionId">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetUserIds(string userId, string permissionId)
        {
            string[] result = null;

             if (!string.IsNullOrEmpty(permissionId))
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.TableName),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseUserEntity.TableName),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId)
                };

                // 20130605 JiRiGaLa 这个运行效率更高一些
                result = GetProperties(parameters, BasePermissionScopeEntity.FieldTargetId);
                // var result = this.GetDataTable(parameters);
                // result = BaseUtil.FieldToArray(result, BasePermissionScopeEntity.FieldTargetId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
            }
            return result;
        }

        //
        // 授予授权范围的实现部分
        //

        #region private string GrantUser(BasePermissionScopeManager manager, string userId, string grantUserId, string permissionCode) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="permissionScopeManager">权限域读写器</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantUserId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        private string GrantUser(BasePermissionScopeManager permissionScopeManager, string systemCode, string userId, string grantUserId, string permissionCode)
        {
            var result = string.Empty;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, grantUserId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode))
            };

            if (!Exists(parameters))
            {
                var resourcePermissionScopeEntity = new BasePermissionScopeEntity
                {
                    PermissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode),
                    ResourceCategory = BaseUserEntity.TableName,
                    ResourceId = userId,
                    TargetCategory = BaseUserEntity.TableName,
                    TargetId = grantUserId,
                    Enabled = 1,
                    DeletionStateCode = 0
                };
                return permissionScopeManager.Add(resourcePermissionScopeEntity);
            }

            return result;
        }
        #endregion

        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="permissionScopeManager">权限域读写器</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantUserId">权限主键</param>
        /// <param name="permissionId">权限编号</param>
        /// <returns>主键</returns>
        private string GrantUser(BasePermissionScopeManager permissionScopeManager, string userId, string grantUserId, string permissionId)
        {
            var result = string.Empty;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, grantUserId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId)
            };

            if (!Exists(parameters))
            {
                var resourcePermissionScopeEntity = new BasePermissionScopeEntity
                {
                    PermissionId = permissionId,
                    ResourceCategory = BaseUserEntity.TableName,
                    ResourceId = userId,
                    TargetCategory = BaseUserEntity.TableName,
                    TargetId = grantUserId,
                    Enabled = 1,
                    DeletionStateCode = 0,
                    Id = Guid.NewGuid().ToString("N")
                };
                return permissionScopeManager.Add(resourcePermissionScopeEntity, false, false);
            }

            return result;
        }


        #region public string GrantUser(string userId, string grantUserId, string permissionCode) 用户授予权限
        /// <summary>
        /// 用户授予权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantUserId">权组织机构限主键</param>
        /// <param name="permissionCode">权限主键</param>
        /// <returns>主键</returns>
        public string GrantUser(string systemCode, string userId, string grantUserId, string permissionCode)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            return GrantUser(permissionScopeManager, systemCode, userId, grantUserId, permissionCode);
        }
        #endregion

        /// <summary>
        /// 授权用户
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <param name="grantUserIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantUsers(string systemCode, string userId, string[] grantUserIds, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < grantUserIds.Length; i++)
            {
                GrantUser(permissionScopeManager, systemCode, userId, grantUserIds[i], permissionCode);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 用户授予权限 传ID
        /// 在某个范围权限菜单上，用户对那些用户有权限
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="grantUserId">权组织机构限主键</param>
        /// <param name="permissionId"></param>
        /// <returns>主键</returns>
        public string GrantUser(string userId, string grantUserId, string permissionId)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            return GrantUser(permissionScopeManager, userId, grantUserId, permissionId);
        }

        /// <summary>
        /// 授权用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="grantUserIds"></param>
        /// <param name="permissionid"></param>
        /// <returns></returns>
        public int GrantUsers(string userId, string[] grantUserIds, string permissionid)
        {
            var result = 0;
            for (var i = 0; i < grantUserIds.Length; i++)
            {
                GrantUser(userId, grantUserIds[i], permissionid);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 授权用户
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="grantUserId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantUsers(string systemCode, string[] userIds, string grantUserId, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < userIds.Length; i++)
            {
                GrantUser(permissionScopeManager, systemCode, userIds[i], grantUserId, permissionCode);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 授权用户
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="grantUserIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantUsers(string systemCode, string[] userIds, string[] grantUserIds, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < userIds.Length; i++)
            {
                for (var j = 0; j < grantUserIds.Length; j++)
                {
                    GrantUser(permissionScopeManager, systemCode, userIds[i], grantUserIds[j], permissionCode);
                    result++;
                }
            }
            return result;
        }


        //
        //  撤销授权范围的实现部分
        //

        #region private int RevokeUser(BasePermissionScopeManager manager, string userId, string revokeUserId, string permissionCode) 为了提高授权的运行速度

        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="permissionScopeManager">权限域读写器</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeUserId">权限主键</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>主键</returns>
        private int RevokeUser(BasePermissionScopeManager permissionScopeManager, string systemCode, string userId, string revokeUserId, string permissionCode)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, revokeUserId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode))
            };
            return permissionScopeManager.Delete(parameters);
        }

        /// <summary>
        /// 员工撤销授权  全部传ID 
        /// </summary>
        /// <param name="permissionScopeManager">权限域读写器</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeUserId">权限主键</param>
        /// <param name="permissionId"></param>
        /// <returns>主键</returns>
        private int RevokeUser(BasePermissionScopeManager permissionScopeManager, string userId, string revokeUserId, string permissionId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, revokeUserId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId)
            };
            return permissionScopeManager.Delete(parameters);
        }

        #endregion

        #region public int RevokeUser(string userId, string revokeUserId, string permissionCode) 员工撤销授权

        /// <summary>
        /// 员工撤销授权
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeUserId"></param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        public int RevokeUser(string systemCode, string userId, string revokeUserId, string permissionCode)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            return RevokeUser(permissionScopeManager, systemCode, userId, revokeUserId, permissionCode);
        }
        #endregion

        /// <summary>
        /// 撤回用户
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <param name="revokeUserIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeUsers(string systemCode, string userId, string[] revokeUserIds, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < revokeUserIds.Length; i++)
            {
                RevokeUser(permissionScopeManager, systemCode, userId, revokeUserIds[i], permissionCode);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 撤回用户
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="revokeUserId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeUsers(string systemCode, string[] userIds, string revokeUserId, string permissionCode)
        {
            var result = 0;
            for (var i = 0; i < userIds.Length; i++)
            {
                RevokeUser( systemCode, userIds[i], revokeUserId, permissionCode);
                result++;
            }
            return result;
        }


        /// <summary>
        /// 撤销用户的对用户的数据权限 基于某个菜单  传ID 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="revokeUserIds"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public int RevokeUsers(string userId, string[] revokeUserIds, string permissionId)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < revokeUserIds.Length; i++)
            {
                result += RevokeUser(permissionScopeManager,userId, revokeUserIds[i], permissionId);
            }
            return result;
        }


        /// <summary>
        /// 撤销用户的对用户的数据权限，基于某个菜单  传ID 
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="revokeUserId"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>

        public int RevokeUsers(string[] userIds, string revokeUserId, string permissionId)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < userIds.Length; i++)
            {
                RevokeUser(permissionScopeManager, userIds[i], revokeUserId, permissionId);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 撤回用户
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="revokeUserIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeUsers(string systemCode, string[] userIds, string[] revokeUserIds, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < userIds.Length; i++)
            {
                for (var j = 0; j < revokeUserIds.Length; j++)
                {
                    RevokeUser(permissionScopeManager, systemCode, userIds[i], revokeUserIds[j], permissionCode);
                    result++;
                }
            }
            return result;
        }
    }
}