//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using System;
    using Util;

    /// <summary>
    /// BaseUserScopeManager
    /// 用户组织机构权限域
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
    public partial class BaseUserScopeManager : BaseManager
    {
        BasePermissionScopeManager ScopeManager
        {
            get
            {
                var tableName = UserInfo.SystemCode + "PermissionScope";
                return new BasePermissionScopeManager(DbHelper, UserInfo, tableName);
            }
        }

        /// <summary>
        /// 组织机构上级节点
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="organizationParentId">组织机构上级主键</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>数据表</returns>
        public DataTable GetUserOrganizationScopes(string systemCode, string organizationParentId, string userId, string permissionCode = "Resource.AccessPermission")
        {
            DataTable result = null;
            var permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);
            if (!string.IsNullOrEmpty(permissionId))
            {
                var tableName = UserInfo.SystemCode + "PermissionScope";
                var sql = @"SELECT  BaseOrganization.Id AS OrganizationId
                                          , BaseOrganization.Name
                                          , BaseOrganization.ContainChildNodes
                                          , BasePermissionScope.TargetId
                                          , BasePermissionScope.ContainChild
     FROM BaseOrganization
                          LEFT OUTER JOIN BasePermissionScope ON BaseOrganization.Id = BasePermissionScope.TargetId
                                          AND BasePermissionScope.TargetCategory = 'BaseOrganization'
                                          AND BasePermissionScope.ResourceCategory = 'BaseUser'
                                          AND BasePermissionScope.ResourceId = '" + userId + @"'
                                          AND BasePermissionScope.PermissionId = " + permissionId;
                if (!string.IsNullOrEmpty(organizationParentId))
                {
                    sql += "  WHERE BaseOrganization.ParentId = " + organizationParentId;
                }
                else
                {
                    sql += "  WHERE BaseOrganization.ParentId = 0 ";
                }
                sql = sql.Replace("BasePermissionScope", tableName);
                result = DbHelper.Fill(sql);
            }
            return result;
        }

        /*
        public List<BaseOrganizationScopeEntity> GetUserOrganizationScopes(string userId, string permissionCode = "Resource.AccessPermission")
        {
            List<BaseOrganizationScopeEntity> result = null;
            string result = this.GetPermissionIdByCode(permissionCode);
            if (!string.IsNullOrEmpty(result))
            {
                BaseOrganizationScopeManager organizationScopeManager = new BaseOrganizationScopeManager(this.DbHelper, this.UserInfo);
                List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
                parameters.Add(new KeyValuePair<string, object>(BaseOrganizationScopeEntity.FieldResourceCategory, BaseUserEntity.CurrentTableName));
                parameters.Add(new KeyValuePair<string, object>(BaseOrganizationScopeEntity.FieldResourceId, userId));
                parameters.Add(new KeyValuePair<string, object>(BaseOrganizationScopeEntity.FieldPermissionId, result));
                result = organizationScopeManager.GetList<BaseOrganizationScopeEntity>(parameters);
            }
            return result;
        }
        */

        /// <summary>
        /// 获取用户组织范围
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <param name="containChild"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public PermissionOrganizationScope GetUserOrganizationScope(string systemCode, string userId, out bool containChild, string permissionCode = "Resource.AccessPermission")
        {
            containChild = false;
            var permissionScope = PermissionOrganizationScope.UserCompany;

            BaseOrganizationScopeEntity organizationScopeEntity = null;
            var permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);
            if (!string.IsNullOrEmpty(permissionId))
            {
                var tableName =  BaseOrganizationScopeEntity.CurrentTableName;
                if (!string.IsNullOrEmpty(systemCode))
                {
                    tableName = systemCode + "OrganizationScope";
                }

                var organizationScopeManager = new BaseOrganizationScopeManager(DbHelper, UserInfo, tableName);
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizationScopeEntity.FieldResourceCategory, BaseUserEntity.CurrentTableName),
                    new KeyValuePair<string, object>(BaseOrganizationScopeEntity.FieldResourceId, userId),
                    new KeyValuePair<string, object>(BaseOrganizationScopeEntity.FieldPermissionId, permissionId)
                };
                var dt = organizationScopeManager.GetDataTable(parameters);
                if (dt != null && dt.Rows.Count > 0)
                {
                    organizationScopeEntity = BaseEntity.Create<BaseOrganizationScopeEntity>(dt);
                }
            }

            if (organizationScopeEntity != null)
            {
                if (organizationScopeEntity.ContainChild == 1)
                {
                    containChild = true;
                }
                if (organizationScopeEntity.AllData == 1)
                {
                    permissionScope = PermissionOrganizationScope.AllData;
                }
                if (organizationScopeEntity.Province == 1)
                {
                    permissionScope = PermissionOrganizationScope.Province;
                }
                if (organizationScopeEntity.City == 1)
                {
                    permissionScope = PermissionOrganizationScope.City;
                }
                if (organizationScopeEntity.District == 1)
                {
                    permissionScope = PermissionOrganizationScope.District;
                }
                if (organizationScopeEntity.ByDetails == 1)
                {
                    permissionScope = PermissionOrganizationScope.ByDetails;
                }
                if (organizationScopeEntity.NotAllowed == 1)
                {
                    permissionScope = PermissionOrganizationScope.NotAllowed;
                }
                if (organizationScopeEntity.OnlyOwnData == 1)
                {
                    permissionScope = PermissionOrganizationScope.OnlyOwnData;
                }
                if (organizationScopeEntity.UserCompany == 1)
                {
                    permissionScope = PermissionOrganizationScope.UserCompany;
                }
                if (organizationScopeEntity.UserSubCompany == 1)
                {
                    permissionScope = PermissionOrganizationScope.UserSubCompany;
                }
                if (organizationScopeEntity.UserDepartment == 1)
                {
                    permissionScope = PermissionOrganizationScope.UserDepartment;
                }
                if (organizationScopeEntity.UserSubDepartment == 1)
                {
                    permissionScope = PermissionOrganizationScope.UserSubDepartment;
                }
                if (organizationScopeEntity.UserWorkgroup == 1)
                {
                    permissionScope = PermissionOrganizationScope.UserWorkgroup;
                }
            }
            return permissionScope;
        }

        /// <summary>
        /// 设置组织范围
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <param name="permissionScope"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="containChild"></param>
        /// <returns></returns>
        public string SetUserOrganizationScope(string systemCode, string userId, PermissionOrganizationScope permissionScope, string permissionCode = "Resource.AccessPermission", bool containChild = false)
        {
            var result = string.Empty;

            var permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);
            if (!string.IsNullOrEmpty(permissionId))
            {
                var tableName = BaseOrganizationScopeEntity.CurrentTableName;
                if (!string.IsNullOrEmpty(systemCode))
                {
                    tableName = systemCode + "OrganizationScope";
                }

                var organizationScopeManager = new BaseOrganizationScopeManager(DbHelper, UserInfo, tableName);
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizationScopeEntity.FieldResourceCategory, BaseUserEntity.CurrentTableName),
                    new KeyValuePair<string, object>(BaseOrganizationScopeEntity.FieldResourceId, userId),
                    new KeyValuePair<string, object>(BaseOrganizationScopeEntity.FieldPermissionId, permissionId)
                };
                result = organizationScopeManager.GetId(parameters);
                BaseOrganizationScopeEntity organizationScopeEntity = null;
                if (string.IsNullOrEmpty(result))
                {
                    organizationScopeEntity = new BaseOrganizationScopeEntity();
                }
                else
                {
                    organizationScopeEntity = organizationScopeManager.GetEntity(result);
                }
                organizationScopeEntity.AllData = (permissionScope == PermissionOrganizationScope.AllData ? 1 : 0);
                organizationScopeEntity.Province = (permissionScope == PermissionOrganizationScope.Province ? 1 : 0);
                organizationScopeEntity.City = (permissionScope == PermissionOrganizationScope.City ? 1 : 0);
                organizationScopeEntity.District = (permissionScope == PermissionOrganizationScope.District ? 1 : 0);
                organizationScopeEntity.UserCompany = (permissionScope == PermissionOrganizationScope.UserCompany ? 1 : 0);
                organizationScopeEntity.UserSubCompany = (permissionScope == PermissionOrganizationScope.UserSubCompany ? 1 : 0);
                organizationScopeEntity.UserDepartment = (permissionScope == PermissionOrganizationScope.UserDepartment ? 1 : 0);
                organizationScopeEntity.UserSubDepartment = (permissionScope == PermissionOrganizationScope.UserSubDepartment ? 1 : 0);
                organizationScopeEntity.UserWorkgroup = (permissionScope == PermissionOrganizationScope.UserWorkgroup ? 1 : 0);
                organizationScopeEntity.OnlyOwnData = (permissionScope == PermissionOrganizationScope.OnlyOwnData ? 1 : 0);
                organizationScopeEntity.ByDetails = (permissionScope == PermissionOrganizationScope.ByDetails ? 1 : 0);
                organizationScopeEntity.NotAllowed = (permissionScope == PermissionOrganizationScope.NotAllowed ? 1 : 0);
                organizationScopeEntity.Enabled = 1;
                organizationScopeEntity.Deleted = 0;
                organizationScopeEntity.ContainChild = containChild ? 1 : 0;
                organizationScopeEntity.PermissionId = int.Parse(permissionId);
                organizationScopeEntity.ResourceCategory = BaseUserEntity.CurrentTableName;
                organizationScopeEntity.ResourceId = userId.ToString();
                if (string.IsNullOrEmpty(result))
                {
                    result = organizationScopeManager.Add(organizationScopeEntity);
                }
                else
                {
                    organizationScopeManager.Update(organizationScopeEntity);
                }
            }
            return result;
        }


        ////
        ////
        //// 授权范围管理部分
        ////
        ////

        #region public string[] GetOrganizationIds(string userId, string permissionCode) 获取用户的权限主键数组
        /// <summary>
        /// 获取用户的权限主键数组
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionId">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetOrganizationIds(string systemCode, string userId, string permissionId)
        {
            string[] result = null;

            if (!string.IsNullOrEmpty(permissionId))
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.CurrentTableName),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseOrganizationEntity.CurrentTableName),
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

        #region public string GrantOrganization(string userId, string grantOrganizationId, string permissionCode = "Resource.AccessPermission", bool containChild = false) 给用户授予组织机构的某个范围权限
        /// <summary>
        /// 给用户授予组织机构的某个范围权限
        /// 哪个用户对哪个部门有什么权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantOrganizationId">权组织机构限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="containChild">包含子节点，递归</param>
        /// <returns>主键</returns>
        public string GrantOrganization(string systemCode, string userId, string grantOrganizationId, string permissionCode = "Resource.AccessPermission", bool containChild = false)
        {
            return GrantOrganization(ScopeManager, systemCode, userId, grantOrganizationId, permissionCode, containChild);
        }
        #endregion

        #region private string GrantOrganization(BasePermissionScopeManager manager, string userId, string grantOrganizationId, string permissionCode = "Resource.AccessPermission", bool containChild = false) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="manager">权限域读写器</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantOrganizationId">权组织机构限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="containChild"></param>
        /// <returns>主键</returns>
        private string GrantOrganization(BasePermissionScopeManager manager, string systemCode, string userId, string grantOrganizationId, string permissionCode = "Resource.AccessPermission", bool containChild = false)
        {
            var result = string.Empty;
            var permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);
            if (!string.IsNullOrEmpty(permissionId))
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.CurrentTableName),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseOrganizationEntity.CurrentTableName),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, grantOrganizationId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId)
                };
                // Nick Deng 优化数据权限设置，没有权限和其他任意一种权限互斥
                // 即当没有权限时，该用户对应该数据权限的其他权限都应删除
                // 当该用户拥有对应该数据权限的其他权限时，删除该用户的没有权限的权限
                result = manager.GetId(parameters);
                if (!string.IsNullOrEmpty(result))
                {
                    manager.SetProperty(new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldId, result), new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldContainChild, containChild ? 1 : 0));
                }
                else
                {
                    var entity = new BasePermissionScopeEntity
                    {
                        PermissionId = permissionId.ToInt(),
                        ResourceCategory = BaseUserEntity.CurrentTableName,
                        ResourceId = userId.ToInt(),
                        TargetCategory = BaseOrganizationEntity.CurrentTableName,
                        TargetId = grantOrganizationId.ToInt(),
                        ContainChild = containChild ? 1 : 0,
                        Enabled = 1,
                        Deleted = 0
                    };
                    result = manager.Add(entity);
                }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 授权组织
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <param name="grantOrganizationIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="containChild"></param>
        /// <returns></returns>
        public int GrantOrganizations(string systemCode, string userId, string[] grantOrganizationIds, string permissionCode, bool containChild = false)
        {
            var result = 0;
            for (var i = 0; i < grantOrganizationIds.Length; i++)
            {
                GrantOrganization(ScopeManager, systemCode, userId, grantOrganizationIds[i], permissionCode, containChild);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 授权组织
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="grantOrganizationId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="containChild"></param>
        /// <returns></returns>
        public int GrantOrganizations(string systemCode, string[] userIds, string grantOrganizationId, string permissionCode, bool containChild = false)
        {
            var result = 0;
            for (var i = 0; i < userIds.Length; i++)
            {
                GrantOrganization(ScopeManager, systemCode, userIds[i], grantOrganizationId, permissionCode, containChild);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 授权组织
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="grantOrganizationIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="containChild"></param>
        /// <returns></returns>
        public int GrantOrganizations(string systemCode, string[] userIds, string[] grantOrganizationIds, string permissionCode, bool containChild = false)
        {
            var result = 0;
            for (var i = 0; i < userIds.Length; i++)
            {
                for (var j = 0; j < grantOrganizationIds.Length; j++)
                {
                    GrantOrganization(ScopeManager, systemCode, userIds[i], grantOrganizationIds[j], permissionCode, containChild);
                    result++;
                }
            }
            return result;
        }


        //
        //  撤销授权范围的实现部分
        //

        #region public int RevokeOrganization(string userId, string revokeOrganizationId, string permissionCode) 用户撤销授权
        /// <summary>
        /// 用户撤销授权
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeOrganizationId"></param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        public int RevokeOrganization(string systemCode, string userId, string revokeOrganizationId, string permissionCode = "Resource.AccessPermission")
        {
            return RevokeOrganization(ScopeManager, systemCode, userId, revokeOrganizationId, permissionCode);
        }
        #endregion

        #region private int RevokeOrganization(BasePermissionScopeManager manager, string userId, string revokeOrganizationId, string permissionCode) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="manager">权限域读写器</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeOrganizationId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        private int RevokeOrganization(BasePermissionScopeManager manager, string systemCode, string userId, string revokeOrganizationId, string permissionCode)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.CurrentTableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseOrganizationEntity.CurrentTableName)
            };
            if (!string.IsNullOrEmpty(revokeOrganizationId))
            {
                parameters.Add(new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, revokeOrganizationId));
            }
            parameters.Add(new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode)));
            return manager.Delete(parameters);
        }
        #endregion

        #region public int RevokeOrganization(string userId, string permissionCode) 用户撤销授权
        /// <summary>
        /// 用户撤销授权
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        public int RevokeOrganization(string systemCode, string userId, string permissionCode)
        {
            return RevokeOrganization(ScopeManager, systemCode, userId, permissionCode, null);
        }
        #endregion

        
        /// <summary>
        /// 撤回组织
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <param name="revokeOrganizationIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeOrganizations(string systemCode, string userId, string[] revokeOrganizationIds, string permissionCode)
        {
            var result = 0;
            for (var i = 0; i < revokeOrganizationIds.Length; i++)
            {
                RevokeOrganization(ScopeManager, systemCode, userId, revokeOrganizationIds[i], permissionCode);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 撤回组织
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="revokeOrganizationId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeOrganizations(string systemCode, string[] userIds, string revokeOrganizationId, string permissionCode)
        {
            var result = 0;
            for (var i = 0; i < userIds.Length; i++)
            {
                RevokeOrganization(ScopeManager, systemCode, userIds[i], revokeOrganizationId, permissionCode);
                result++;
            }
            return result;
        }
        /// <summary>
        /// 撤回组织
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="revokeOrganizationIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeOrganizations(string systemCode, string[] userIds, string[] revokeOrganizationIds, string permissionCode)
        {
            var result = 0;
            for (var i = 0; i < userIds.Length; i++)
            {
                for (var j = 0; j < revokeOrganizationIds.Length; j++)
                {
                    RevokeOrganization(ScopeManager, systemCode, userIds[i], revokeOrganizationIds[j], permissionCode);
                    result++;
                }
            }
            return result;
        }
    }
}