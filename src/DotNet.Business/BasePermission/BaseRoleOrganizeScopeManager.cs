//-----------------------------------------------------------------
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
    /// BaseRoleScopeManager
    /// 角色权限域
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
    public partial class BaseRoleScopeManager : BaseManager, IBaseManager
    {
        ////
        ////
        //// 授权范围管理部分
        ////
        ////

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public List<BaseOrganizationScopeEntity> GetRoleOrganizationScopes(string systemCode, string roleId, string permissionCode = "Resource.AccessPermission")
        {
            List<BaseOrganizationScopeEntity> result = null;
            var permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
            if (!string.IsNullOrEmpty(permissionId))
            {
                var organizeScopeManager = new BaseOrganizationScopeManager(DbHelper, UserInfo);
                var tableName = UserInfo.SystemCode + "Role";
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizationScopeEntity.FieldResourceCategory, tableName),
                    new KeyValuePair<string, object>(BaseOrganizationScopeEntity.FieldResourceId, roleId),
                    new KeyValuePair<string, object>(BaseOrganizationScopeEntity.FieldPermissionId, permissionId)
                };
                result = organizeScopeManager.GetList<BaseOrganizationScopeEntity>(parameters);
            }
            return result;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <param name="containChild"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public PermissionOrganizationScope GetRoleOrganizationScope(string systemCode, string roleId, out bool containChild, string permissionCode = "Resource.AccessPermission")
        {
            containChild = false;
            var permissionScope = PermissionOrganizationScope.OnlyOwnData;

            BaseOrganizationScopeEntity organizeScopeEntity = null;
            var permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
            if (!string.IsNullOrEmpty(permissionId))
            {
                var tableName = BaseOrganizationScopeEntity.TableName;
                if (!string.IsNullOrEmpty(systemCode))
                {
                    tableName = systemCode + "OrganizationScope";
                }
                var organizeScopeManager = new BaseOrganizationScopeManager(DbHelper, UserInfo, tableName);
                var tableRole = systemCode + "Role";
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizationScopeEntity.FieldResourceCategory, tableRole),
                    new KeyValuePair<string, object>(BaseOrganizationScopeEntity.FieldResourceId, roleId),
                    new KeyValuePair<string, object>(BaseOrganizationScopeEntity.FieldPermissionId, permissionId)
                };
                var dt = organizeScopeManager.GetDataTable(parameters);
                if (dt != null && dt.Rows.Count > 0)
                {
                    organizeScopeEntity = BaseEntity.Create<BaseOrganizationScopeEntity>(dt);
                }
            }

            if (organizeScopeEntity != null)
            {
                if (organizeScopeEntity.ContainChild == 1)
                {
                    containChild = true;
                }

                if (organizeScopeEntity.AllData == 1)
                {
                    permissionScope = PermissionOrganizationScope.AllData;
                }
                if (organizeScopeEntity.ByDetails == 1)
                {
                    permissionScope = PermissionOrganizationScope.ByDetails;
                }
                if (organizeScopeEntity.NotAllowed == 1)
                {
                    permissionScope = PermissionOrganizationScope.NotAllowed;
                }
                if (organizeScopeEntity.OnlyOwnData == 1)
                {
                    permissionScope = PermissionOrganizationScope.OnlyOwnData;
                }

                if (organizeScopeEntity.Province == 1)
                {
                    permissionScope = PermissionOrganizationScope.Province;
                }
                if (organizeScopeEntity.City == 1)
                {
                    permissionScope = PermissionOrganizationScope.City;
                }
                if (organizeScopeEntity.District == 1)
                {
                    permissionScope = PermissionOrganizationScope.District;
                }
                if (organizeScopeEntity.Street == 1)
                {
                    permissionScope = PermissionOrganizationScope.Street;
                }
                if (organizeScopeEntity.UserCompany == 1)
                {
                    permissionScope = PermissionOrganizationScope.UserCompany;
                }
                if (organizeScopeEntity.UserSubCompany == 1)
                {
                    permissionScope = PermissionOrganizationScope.UserSubCompany;
                }
                if (organizeScopeEntity.UserDepartment == 1)
                {
                    permissionScope = PermissionOrganizationScope.UserDepartment;
                }
                if (organizeScopeEntity.UserSubDepartment == 1)
                {
                    permissionScope = PermissionOrganizationScope.UserSubDepartment;
                }
                if (organizeScopeEntity.UserWorkgroup == 1)
                {
                    permissionScope = PermissionOrganizationScope.UserWorkgroup;
                }
            }
            return permissionScope;
        }

        /// <summary>
        /// 设置角色组织范围
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <param name="permissionScope"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="containChild"></param>
        /// <returns></returns>
        public string SetRoleOrganizationScope(string systemCode, string roleId, PermissionOrganizationScope permissionScope, string permissionCode = "Resource.AccessPermission", bool containChild = false)
        {
            var result = string.Empty;

            var permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
            if (!string.IsNullOrEmpty(permissionId))
            {
                var tableName = BaseOrganizationScopeEntity.TableName;
                if (!string.IsNullOrEmpty(systemCode))
                {
                    tableName = systemCode + "OrganizationScope";
                }
                var organizeScopeManager = new BaseOrganizationScopeManager(DbHelper, UserInfo, tableName);
                var tableRole = systemCode + "Role";
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizationScopeEntity.FieldResourceCategory, tableRole),
                    new KeyValuePair<string, object>(BaseOrganizationScopeEntity.FieldResourceId, roleId),
                    new KeyValuePair<string, object>(BaseOrganizationScopeEntity.FieldPermissionId, permissionId)
                };
                result = organizeScopeManager.GetId(parameters);

                BaseOrganizationScopeEntity organizeScopeEntity = null;
                if (string.IsNullOrEmpty(result))
                {
                    organizeScopeEntity = new BaseOrganizationScopeEntity
                    {
                        Id = Guid.NewGuid().ToString("N")
                    };
                }
                else
                {
                    organizeScopeEntity = organizeScopeManager.GetEntity(result);
                }

                organizeScopeEntity.AllData = (permissionScope == PermissionOrganizationScope.AllData ? 1 : 0);
                organizeScopeEntity.Province = (permissionScope == PermissionOrganizationScope.Province ? 1 : 0);
                organizeScopeEntity.City = (permissionScope == PermissionOrganizationScope.City ? 1 : 0);
                organizeScopeEntity.District = (permissionScope == PermissionOrganizationScope.District ? 1 : 0);
                organizeScopeEntity.Street = (permissionScope == PermissionOrganizationScope.Street ? 1 : 0);
                organizeScopeEntity.UserCompany = (permissionScope == PermissionOrganizationScope.UserCompany ? 1 : 0);
                organizeScopeEntity.UserSubCompany = (permissionScope == PermissionOrganizationScope.UserSubCompany ? 1 : 0);
                organizeScopeEntity.UserDepartment = (permissionScope == PermissionOrganizationScope.UserDepartment ? 1 : 0);
                organizeScopeEntity.UserSubDepartment = (permissionScope == PermissionOrganizationScope.UserSubDepartment ? 1 : 0);
                organizeScopeEntity.UserWorkgroup = (permissionScope == PermissionOrganizationScope.UserWorkgroup ? 1 : 0);
                organizeScopeEntity.OnlyOwnData = (permissionScope == PermissionOrganizationScope.OnlyOwnData ? 1 : 0);
                organizeScopeEntity.ByDetails = (permissionScope == PermissionOrganizationScope.ByDetails ? 1 : 0);
                organizeScopeEntity.NotAllowed = (permissionScope == PermissionOrganizationScope.NotAllowed ? 1 : 0);
                organizeScopeEntity.Enabled = 1;
                organizeScopeEntity.DeletionStateCode = 0;
                organizeScopeEntity.ContainChild = containChild ? 1 : 0;
                organizeScopeEntity.PermissionId = int.Parse(permissionId);
                organizeScopeEntity.ResourceCategory = tableRole;
                organizeScopeEntity.ResourceId = roleId;

                if (string.IsNullOrEmpty(result))
                {
                    result = organizeScopeManager.Add(organizeScopeEntity, false, false);
                }
                else
                {
                    organizeScopeManager.Update(organizeScopeEntity);
                }
            }

            return result;
        }

        #region public string[] GetOrganizationIds(string roleId, string permissionCode) 获取员工的权限主键数组

        /// <summary>
        /// 获取员工的权限主键数组
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionId">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetOrganizationIds(string systemCode, string roleId, string permissionId)
        {
            string[] result = null;
            if (!string.IsNullOrEmpty(permissionId))
            {
                var roleTableName = systemCode + "Role";
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, roleTableName),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, roleId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseOrganizationEntity.TableName),
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

        //
        // 授予授权范围的实现部分
        //

        #region private string GrantOrganization(BasePermissionScopeManager manager, string id, string roleId, string grantOrganizationId) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="permissionScopeManager">权限域读写器</param>
        /// <param name="roleId">员工主键</param>
        /// <param name="grantOrganizationId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        private string GrantOrganization(string systemCode, BasePermissionScopeManager permissionScopeManager, string roleId, string grantOrganizationId, string permissionCode)
        {
            var result = string.Empty;

            var roleTableName = systemCode + "Role";

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, roleTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, roleId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseOrganizationEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, grantOrganizationId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode))
            };

            // Nick Deng 优化数据权限设置，没有权限和其他任意一种权限互斥
            // 即当没有权限时，该角色对应该数据权限的其他权限都应删除
            // 当该角色拥有对应该数据权限的其他权限时，删除该角色的没有权限的权限
            var resourcePermissionScopeEntity = new BasePermissionScopeEntity();
            var dt = new DataTable();
            if (!Exists(parameters))
            {
                resourcePermissionScopeEntity.PermissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
                resourcePermissionScopeEntity.ResourceCategory = roleTableName;
                resourcePermissionScopeEntity.ResourceId = roleId;
                resourcePermissionScopeEntity.TargetCategory = BaseOrganizationEntity.TableName;
                resourcePermissionScopeEntity.TargetId = grantOrganizationId;
                resourcePermissionScopeEntity.Enabled = 1;
                resourcePermissionScopeEntity.DeletionStateCode = 0;
                result = permissionScopeManager.Add(resourcePermissionScopeEntity, true, false);
                if (grantOrganizationId != ((int)PermissionOrganizationScope.NotAllowed).ToString())
                {
                    parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, roleTableName),
                        new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, roleId),
                        new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseOrganizationEntity.TableName),
                        new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, ((int)PermissionOrganizationScope.NotAllowed).ToString()),
                        new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode))
                    };

                    if (Exists(parameters))
                    {
                        dt = permissionScopeManager.GetDataTable(parameters);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            permissionScopeManager.DeleteObject(dt.Rows[0][BasePermissionScopeEntity.FieldId].ToString());
                        }
                    }
                }
                else
                {
                    parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, roleTableName),
                        new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, roleId),
                        new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseOrganizationEntity.TableName),
                        new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode))
                    };

                    dt = permissionScopeManager.GetDataTable(parameters);
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["TargetId"].ToString() != ((int)PermissionOrganizationScope.NotAllowed).ToString())
                            permissionScopeManager.DeleteObject(dt.Rows[0][BasePermissionScopeEntity.FieldId].ToString());
                    }
                }
            }

            return result;
        }
        #endregion

        #region public string GrantOrganization(string roleId, string result) 员工授予权限
        /// <summary>
        /// 员工授予权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleId">员工主键</param>
        /// <param name="grantOrganizationId">权组织机构限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        public string GrantOrganization(string systemCode, string roleId, string grantOrganizationId, string permissionCode)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            return GrantOrganization(systemCode, permissionScopeManager, roleId, grantOrganizationId, permissionCode);
        }
        #endregion

        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <param name="grantOrganizationIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantOrganizations(string systemCode, string roleId, string[] grantOrganizationIds, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < grantOrganizationIds.Length; i++)
            {
                GrantOrganization(systemCode, permissionScopeManager, roleId, grantOrganizationIds[i], permissionCode);
                result++;
            }

            return result;
        }
        
        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="grantOrganizationId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantOrganizations(string systemCode, string[] roleIds, string grantOrganizationId, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < roleIds.Length; i++)
            {
                GrantOrganization(systemCode, permissionScopeManager, roleIds[i], grantOrganizationId, permissionCode);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="grantOrganizationIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantOrganizations(string systemCode, string[] roleIds, string[] grantOrganizationIds, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < roleIds.Length; i++)
            {
                for (var j = 0; j < grantOrganizationIds.Length; j++)
                {
                    GrantOrganization(systemCode, permissionScopeManager, roleIds[i], grantOrganizationIds[j], permissionCode);
                    result++;
                }
            }
            return result;
        }


        //
        //  撤销授权范围的实现部分
        //

        #region private int RevokeOrganization(BasePermissionScopeManager manager, string roleId, string revokeOrganizationId, string permissionCode) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="permissionScopeManager">权限域读写器</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="revokeOrganizationId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        private int RevokeOrganization(string systemCode, BasePermissionScopeManager permissionScopeManager, string roleId, string revokeOrganizationId, string permissionCode)
        {
            var roleTableName = UserInfo.SystemCode + "Role";
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, roleTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, roleId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseOrganizationEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, revokeOrganizationId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode))
            };

            return permissionScopeManager.Delete(parameters);
        }
        #endregion

        #region public int RevokeOrganization(string roleId, string result) 员工撤销授权
        /// <summary>
        /// 员工撤销授权
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="revokeOrganizationId"></param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        public int RevokeOrganization(string systemCode, string roleId, string revokeOrganizationId, string permissionCode)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            return RevokeOrganization(systemCode, permissionScopeManager, roleId, revokeOrganizationId, permissionCode);
        }
        #endregion

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <param name="revokeOrganizationIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeOrganizations(string systemCode, string roleId, string[] revokeOrganizationIds, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < revokeOrganizationIds.Length; i++)
            {
                RevokeOrganization(systemCode, permissionScopeManager, roleId, revokeOrganizationIds[i], permissionCode);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="revokeOrganizationId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeOrganizations(string systemCode, string[] roleIds, string revokeOrganizationId, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < roleIds.Length; i++)
            {
                RevokeOrganization(systemCode, permissionScopeManager, roleIds[i], revokeOrganizationId, permissionCode);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="revokeOrganizationIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeOrganizations(string systemCode, string[] roleIds, string[] revokeOrganizationIds, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < roleIds.Length; i++)
            {
                for (var j = 0; j < revokeOrganizationIds.Length; j++)
                {
                    RevokeOrganization(systemCode, permissionScopeManager, roleIds[i], revokeOrganizationIds[j], permissionCode);
                    result++;
                }
            }

            return result;
        }
    }
}