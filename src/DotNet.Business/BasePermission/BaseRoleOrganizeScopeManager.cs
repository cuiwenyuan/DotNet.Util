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
        public List<BaseOrganizeScopeEntity> GetRoleOrganizeScopes(string systemCode, string roleId, string permissionCode = "Resource.AccessPermission")
        {
            List<BaseOrganizeScopeEntity> result = null;
            var permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
            if (!string.IsNullOrEmpty(permissionId))
            {
                var organizeScopeManager = new BaseOrganizeScopeManager(DbHelper, UserInfo);
                var tableName = UserInfo.SystemCode + "Role";
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizeScopeEntity.FieldResourceCategory, tableName),
                    new KeyValuePair<string, object>(BaseOrganizeScopeEntity.FieldResourceId, roleId),
                    new KeyValuePair<string, object>(BaseOrganizeScopeEntity.FieldPermissionId, permissionId)
                };
                result = organizeScopeManager.GetList<BaseOrganizeScopeEntity>(parameters);
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
        public PermissionOrganizeScope GetRoleOrganizeScope(string systemCode, string roleId, out bool containChild, string permissionCode = "Resource.AccessPermission")
        {
            containChild = false;
            var permissionScope = PermissionOrganizeScope.OnlyOwnData;

            BaseOrganizeScopeEntity organizeScopeEntity = null;
            var permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
            if (!string.IsNullOrEmpty(permissionId))
            {
                var tableName = BaseOrganizeScopeEntity.TableName;
                if (!string.IsNullOrEmpty(systemCode))
                {
                    tableName = systemCode + "OrganizeScope";
                }
                var organizeScopeManager = new BaseOrganizeScopeManager(DbHelper, UserInfo, tableName);
                var tableRole = systemCode + "Role";
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizeScopeEntity.FieldResourceCategory, tableRole),
                    new KeyValuePair<string, object>(BaseOrganizeScopeEntity.FieldResourceId, roleId),
                    new KeyValuePair<string, object>(BaseOrganizeScopeEntity.FieldPermissionId, permissionId)
                };
                var dt = organizeScopeManager.GetDataTable(parameters);
                if (dt != null && dt.Rows.Count > 0)
                {
                    organizeScopeEntity = BaseEntity.Create<BaseOrganizeScopeEntity>(dt);
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
                    permissionScope = PermissionOrganizeScope.AllData;
                }
                if (organizeScopeEntity.ByDetails == 1)
                {
                    permissionScope = PermissionOrganizeScope.ByDetails;
                }
                if (organizeScopeEntity.NotAllowed == 1)
                {
                    permissionScope = PermissionOrganizeScope.NotAllowed;
                }
                if (organizeScopeEntity.OnlyOwnData == 1)
                {
                    permissionScope = PermissionOrganizeScope.OnlyOwnData;
                }

                if (organizeScopeEntity.Province == 1)
                {
                    permissionScope = PermissionOrganizeScope.Province;
                }
                if (organizeScopeEntity.City == 1)
                {
                    permissionScope = PermissionOrganizeScope.City;
                }
                if (organizeScopeEntity.District == 1)
                {
                    permissionScope = PermissionOrganizeScope.District;
                }
                if (organizeScopeEntity.Street == 1)
                {
                    permissionScope = PermissionOrganizeScope.Street;
                }
                if (organizeScopeEntity.UserCompany == 1)
                {
                    permissionScope = PermissionOrganizeScope.UserCompany;
                }
                if (organizeScopeEntity.UserSubCompany == 1)
                {
                    permissionScope = PermissionOrganizeScope.UserSubCompany;
                }
                if (organizeScopeEntity.UserDepartment == 1)
                {
                    permissionScope = PermissionOrganizeScope.UserDepartment;
                }
                if (organizeScopeEntity.UserSubDepartment == 1)
                {
                    permissionScope = PermissionOrganizeScope.UserSubDepartment;
                }
                if (organizeScopeEntity.UserWorkgroup == 1)
                {
                    permissionScope = PermissionOrganizeScope.UserWorkgroup;
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
        public string SetRoleOrganizeScope(string systemCode, string roleId, PermissionOrganizeScope permissionScope, string permissionCode = "Resource.AccessPermission", bool containChild = false)
        {
            var result = string.Empty;

            var permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
            if (!string.IsNullOrEmpty(permissionId))
            {
                var tableName = BaseOrganizeScopeEntity.TableName;
                if (!string.IsNullOrEmpty(systemCode))
                {
                    tableName = systemCode + "OrganizeScope";
                }
                var organizeScopeManager = new BaseOrganizeScopeManager(DbHelper, UserInfo, tableName);
                var tableRole = systemCode + "Role";
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizeScopeEntity.FieldResourceCategory, tableRole),
                    new KeyValuePair<string, object>(BaseOrganizeScopeEntity.FieldResourceId, roleId),
                    new KeyValuePair<string, object>(BaseOrganizeScopeEntity.FieldPermissionId, permissionId)
                };
                result = organizeScopeManager.GetId(parameters);

                BaseOrganizeScopeEntity organizeScopeEntity = null;
                if (string.IsNullOrEmpty(result))
                {
                    organizeScopeEntity = new BaseOrganizeScopeEntity
                    {
                        Id = Guid.NewGuid().ToString("N")
                    };
                }
                else
                {
                    organizeScopeEntity = organizeScopeManager.GetEntity(result);
                }

                organizeScopeEntity.AllData = (permissionScope == PermissionOrganizeScope.AllData ? 1 : 0);
                organizeScopeEntity.Province = (permissionScope == PermissionOrganizeScope.Province ? 1 : 0);
                organizeScopeEntity.City = (permissionScope == PermissionOrganizeScope.City ? 1 : 0);
                organizeScopeEntity.District = (permissionScope == PermissionOrganizeScope.District ? 1 : 0);
                organizeScopeEntity.Street = (permissionScope == PermissionOrganizeScope.Street ? 1 : 0);
                organizeScopeEntity.UserCompany = (permissionScope == PermissionOrganizeScope.UserCompany ? 1 : 0);
                organizeScopeEntity.UserSubCompany = (permissionScope == PermissionOrganizeScope.UserSubCompany ? 1 : 0);
                organizeScopeEntity.UserDepartment = (permissionScope == PermissionOrganizeScope.UserDepartment ? 1 : 0);
                organizeScopeEntity.UserSubDepartment = (permissionScope == PermissionOrganizeScope.UserSubDepartment ? 1 : 0);
                organizeScopeEntity.UserWorkgroup = (permissionScope == PermissionOrganizeScope.UserWorkgroup ? 1 : 0);
                organizeScopeEntity.OnlyOwnData = (permissionScope == PermissionOrganizeScope.OnlyOwnData ? 1 : 0);
                organizeScopeEntity.ByDetails = (permissionScope == PermissionOrganizeScope.ByDetails ? 1 : 0);
                organizeScopeEntity.NotAllowed = (permissionScope == PermissionOrganizeScope.NotAllowed ? 1 : 0);
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

        #region public string[] GetOrganizeIds(string roleId, string permissionCode) 获取员工的权限主键数组

        /// <summary>
        /// 获取员工的权限主键数组
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="permissionId">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetOrganizeIds(string systemCode, string roleId, string permissionId)
        {
            string[] result = null;
            if (!string.IsNullOrEmpty(permissionId))
            {
                var roleTableName = systemCode + "Role";
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, roleTableName),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, roleId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseOrganizeEntity.TableName),
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

        #region private string GrantOrganize(BasePermissionScopeManager manager, string id, string roleId, string grantOrganizeId) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="permissionScopeManager">权限域读写器</param>
        /// <param name="roleId">员工主键</param>
        /// <param name="grantOrganizeId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        private string GrantOrganize(string systemCode, BasePermissionScopeManager permissionScopeManager, string roleId, string grantOrganizeId, string permissionCode)
        {
            var result = string.Empty;

            var roleTableName = systemCode + "Role";

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, roleTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, roleId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseOrganizeEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, grantOrganizeId),
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
                resourcePermissionScopeEntity.TargetCategory = BaseOrganizeEntity.TableName;
                resourcePermissionScopeEntity.TargetId = grantOrganizeId;
                resourcePermissionScopeEntity.Enabled = 1;
                resourcePermissionScopeEntity.DeletionStateCode = 0;
                result = permissionScopeManager.Add(resourcePermissionScopeEntity, true, false);
                if (grantOrganizeId != ((int)PermissionOrganizeScope.NotAllowed).ToString())
                {
                    parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, roleTableName),
                        new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, roleId),
                        new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseOrganizeEntity.TableName),
                        new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, ((int)PermissionOrganizeScope.NotAllowed).ToString()),
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
                        new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseOrganizeEntity.TableName),
                        new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode))
                    };

                    dt = permissionScopeManager.GetDataTable(parameters);
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["TargetId"].ToString() != ((int)PermissionOrganizeScope.NotAllowed).ToString())
                            permissionScopeManager.DeleteObject(dt.Rows[0][BasePermissionScopeEntity.FieldId].ToString());
                    }
                }
            }

            return result;
        }
        #endregion

        #region public string GrantOrganize(string roleId, string result) 员工授予权限
        /// <summary>
        /// 员工授予权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleId">员工主键</param>
        /// <param name="grantOrganizeId">权组织机构限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        public string GrantOrganize(string systemCode, string roleId, string grantOrganizeId, string permissionCode)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            return GrantOrganize(systemCode, permissionScopeManager, roleId, grantOrganizeId, permissionCode);
        }
        #endregion

        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <param name="grantOrganizeIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantOrganizes(string systemCode, string roleId, string[] grantOrganizeIds, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < grantOrganizeIds.Length; i++)
            {
                GrantOrganize(systemCode, permissionScopeManager, roleId, grantOrganizeIds[i], permissionCode);
                result++;
            }

            return result;
        }
        
        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="grantOrganizeId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantOrganizes(string systemCode, string[] roleIds, string grantOrganizeId, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < roleIds.Length; i++)
            {
                GrantOrganize(systemCode, permissionScopeManager, roleIds[i], grantOrganizeId, permissionCode);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="grantOrganizeIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int GrantOrganizes(string systemCode, string[] roleIds, string[] grantOrganizeIds, string permissionCode)
        {
            var result = 0;
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < roleIds.Length; i++)
            {
                for (var j = 0; j < grantOrganizeIds.Length; j++)
                {
                    GrantOrganize(systemCode, permissionScopeManager, roleIds[i], grantOrganizeIds[j], permissionCode);
                    result++;
                }
            }
            return result;
        }


        //
        //  撤销授权范围的实现部分
        //

        #region private int RevokeOrganize(BasePermissionScopeManager manager, string roleId, string revokeOrganizeId, string permissionCode) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="permissionScopeManager">权限域读写器</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="revokeOrganizeId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        private int RevokeOrganize(string systemCode, BasePermissionScopeManager permissionScopeManager, string roleId, string revokeOrganizeId, string permissionCode)
        {
            var roleTableName = UserInfo.SystemCode + "Role";
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, roleTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, roleId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseOrganizeEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, revokeOrganizeId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode))
            };

            return permissionScopeManager.Delete(parameters);
        }
        #endregion

        #region public int RevokeOrganize(string roleId, string result) 员工撤销授权
        /// <summary>
        /// 员工撤销授权
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="revokeOrganizeId"></param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        public int RevokeOrganize(string systemCode, string roleId, string revokeOrganizeId, string permissionCode)
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            return RevokeOrganize(systemCode, permissionScopeManager, roleId, revokeOrganizeId, permissionCode);
        }
        #endregion

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <param name="revokeOrganizeIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeOrganizes(string systemCode, string roleId, string[] revokeOrganizeIds, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < revokeOrganizeIds.Length; i++)
            {
                RevokeOrganize(systemCode, permissionScopeManager, roleId, revokeOrganizeIds[i], permissionCode);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="revokeOrganizeId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeOrganizes(string systemCode, string[] roleIds, string revokeOrganizeId, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < roleIds.Length; i++)
            {
                RevokeOrganize(systemCode, permissionScopeManager, roleIds[i], revokeOrganizeId, permissionCode);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <param name="revokeOrganizeIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeOrganizes(string systemCode, string[] roleIds, string[] revokeOrganizeIds, string permissionCode)
        {
            var result = 0;

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < roleIds.Length; i++)
            {
                for (var j = 0; j < revokeOrganizeIds.Length; j++)
                {
                    RevokeOrganize(systemCode, permissionScopeManager, roleIds[i], revokeOrganizeIds[j], permissionCode);
                    result++;
                }
            }

            return result;
        }
    }
}