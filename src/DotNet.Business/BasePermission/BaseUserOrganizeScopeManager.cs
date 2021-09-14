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
    /// BaseUserScopeManager
    /// 用户组织机构权限域
    /// 
    /// 修改记录
    ///
    ///     2011.03.13 版本：2.0 JiRiGaLa 重新整理代码。
    ///     2008.05.24 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2011.03.13</date>
    /// </author>
    /// </summary>
    public partial class BaseUserScopeManager : BaseManager, IBaseManager
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
        /// <param name="organizeParentId">组织机构上级主键</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>数据表</returns>
        public DataTable GetUserOrganizeScopes(string systemCode, string organizeParentId, string userId, string permissionCode = "Resource.AccessPermission")
        {
            DataTable result = null;
            var permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
            if (!string.IsNullOrEmpty(permissionId))
            {
                var tableName = UserInfo.SystemCode + "PermissionScope";
                var sql = @"SELECT  BaseOrganize.Id AS OrganizeId
                                          , BaseOrganize.FullName
                                          , BaseOrganize.ContainChildNodes
                                          , BasePermissionScope.TargetId
                                          , BasePermissionScope.ContainChild
     FROM BaseOrganize
                          LEFT OUTER JOIN BasePermissionScope ON BaseOrganize.Id = BasePermissionScope.TargetId
                                          AND BasePermissionScope.TargetCategory = 'BaseOrganize'
                                          AND BasePermissionScope.ResourceCategory = 'BaseUser'
                                          AND BasePermissionScope.ResourceId = '" + userId + @"'
                                          AND BasePermissionScope.PermissionId = " + permissionId;
                if (!string.IsNullOrEmpty(organizeParentId))
                {
                    sql += "  WHERE BaseOrganize.ParentId = " + organizeParentId;
                }
                else
                {
                    sql += "  WHERE BaseOrganize.ParentId IS NULL ";
                }
                sql = sql.Replace("BasePermissionScope", tableName);
                result = DbHelper.Fill(sql);
            }
            return result;
        }

        /*
        public List<BaseOrganizeScopeEntity> GetUserOrganizeScopes(string userId, string permissionCode = "Resource.AccessPermission")
        {
            List<BaseOrganizeScopeEntity> result = null;
            string result = this.GetPermissionIdByCode(permissionCode);
            if (!string.IsNullOrEmpty(result))
            {
                BaseOrganizeScopeManager organizeScopeManager = new BaseOrganizeScopeManager(this.DbHelper, this.UserInfo);
                List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
                parameters.Add(new KeyValuePair<string, object>(BaseOrganizeScopeEntity.FieldResourceCategory, BaseUserEntity.TableName));
                parameters.Add(new KeyValuePair<string, object>(BaseOrganizeScopeEntity.FieldResourceId, userId));
                parameters.Add(new KeyValuePair<string, object>(BaseOrganizeScopeEntity.FieldPermissionId, result));
                result = organizeScopeManager.GetList<BaseOrganizeScopeEntity>(parameters);
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
        public PermissionOrganizeScope GetUserOrganizeScope(string systemCode, string userId, out bool containChild, string permissionCode = "Resource.AccessPermission")
        {
            containChild = false;
            var permissionScope = PermissionOrganizeScope.UserCompany;

            BaseOrganizeScopeEntity organizeScopeEntity = null;
            var permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
            if (!string.IsNullOrEmpty(permissionId))
            {
                var tableName =  BaseOrganizeScopeEntity.TableName;
                if (!string.IsNullOrEmpty(systemCode))
                {
                    tableName = systemCode + "OrganizeScope";
                }

                var organizeScopeManager = new BaseOrganizeScopeManager(DbHelper, UserInfo, tableName);
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizeScopeEntity.FieldResourceCategory, BaseUserEntity.TableName),
                    new KeyValuePair<string, object>(BaseOrganizeScopeEntity.FieldResourceId, userId),
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
        /// 设置组织范围
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <param name="permissionScope"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="containChild"></param>
        /// <returns></returns>
        public string SetUserOrganizeScope(string systemCode, string userId, PermissionOrganizeScope permissionScope, string permissionCode = "Resource.AccessPermission", bool containChild = false)
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
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizeScopeEntity.FieldResourceCategory, BaseUserEntity.TableName),
                    new KeyValuePair<string, object>(BaseOrganizeScopeEntity.FieldResourceId, userId),
                    new KeyValuePair<string, object>(BaseOrganizeScopeEntity.FieldPermissionId, permissionId)
                };
                result = organizeScopeManager.GetId(parameters);
                BaseOrganizeScopeEntity organizeScopeEntity = null;
                if (string.IsNullOrEmpty(result))
                {
                    organizeScopeEntity = new BaseOrganizeScopeEntity();
                }
                else
                {
                    organizeScopeEntity = organizeScopeManager.GetObject(result);
                }
                organizeScopeEntity.AllData = (permissionScope == PermissionOrganizeScope.AllData ? 1 : 0);
                organizeScopeEntity.Province = (permissionScope == PermissionOrganizeScope.Province ? 1 : 0);
                organizeScopeEntity.City = (permissionScope == PermissionOrganizeScope.City ? 1 : 0);
                organizeScopeEntity.District = (permissionScope == PermissionOrganizeScope.District ? 1 : 0);
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
                organizeScopeEntity.ResourceCategory = BaseUserEntity.TableName;
                organizeScopeEntity.ResourceId = userId;
                if (string.IsNullOrEmpty(result))
                {
                    result = organizeScopeManager.Add(organizeScopeEntity);
                }
                else
                {
                    organizeScopeManager.Update(organizeScopeEntity);
                }
            }
            return result;
        }


        ////
        ////
        //// 授权范围管理部分
        ////
        ////

        #region public string[] GetOrganizeIds(string userId, string permissionCode) 获取用户的权限主键数组
        /// <summary>
        /// 获取用户的权限主键数组
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionId">权限编号</param>
        /// <returns>主键数组</returns>
        public string[] GetOrganizeIds(string systemCode, string userId, string permissionId)
        {
            string[] result = null;

            if (!string.IsNullOrEmpty(permissionId))
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.TableName),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
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

        #region public string GrantOrganize(string userId, string grantOrganizeId, string permissionCode = "Resource.AccessPermission", bool containChild = false) 给用户授予组织机构的某个范围权限
        /// <summary>
        /// 给用户授予组织机构的某个范围权限
        /// 哪个用户对哪个部门有什么权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantOrganizeId">权组织机构限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="containChild">包含子节点，递归</param>
        /// <returns>主键</returns>
        public string GrantOrganize(string systemCode, string userId, string grantOrganizeId, string permissionCode = "Resource.AccessPermission", bool containChild = false)
        {
            return GrantOrganize(ScopeManager, systemCode, userId, grantOrganizeId, permissionCode, containChild);
        }
        #endregion

        #region private string GrantOrganize(BasePermissionScopeManager manager, string userId, string grantOrganizeId, string permissionCode = "Resource.AccessPermission", bool containChild = false) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="manager">权限域读写器</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="grantOrganizeId">权组织机构限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="containChild"></param>
        /// <returns>主键</returns>
        private string GrantOrganize(BasePermissionScopeManager manager, string systemCode, string userId, string grantOrganizeId, string permissionCode = "Resource.AccessPermission", bool containChild = false)
        {
            var result = string.Empty;
            var permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
            if (!string.IsNullOrEmpty(permissionId))
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.TableName),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseOrganizeEntity.TableName),
                    new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, grantOrganizeId),
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
                        PermissionId = permissionId,
                        ResourceCategory = BaseUserEntity.TableName,
                        ResourceId = userId,
                        TargetCategory = BaseOrganizeEntity.TableName,
                        TargetId = grantOrganizeId,
                        ContainChild = containChild ? 1 : 0,
                        Enabled = 1,
                        DeletionStateCode = 0
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
        /// <param name="grantOrganizeIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="containChild"></param>
        /// <returns></returns>
        public int GrantOrganizes(string systemCode, string userId, string[] grantOrganizeIds, string permissionCode, bool containChild = false)
        {
            var result = 0;
            for (var i = 0; i < grantOrganizeIds.Length; i++)
            {
                GrantOrganize(ScopeManager, systemCode, userId, grantOrganizeIds[i], permissionCode, containChild);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 授权组织
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="grantOrganizeId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="containChild"></param>
        /// <returns></returns>
        public int GrantOrganizes(string systemCode, string[] userIds, string grantOrganizeId, string permissionCode, bool containChild = false)
        {
            var result = 0;
            for (var i = 0; i < userIds.Length; i++)
            {
                GrantOrganize(ScopeManager, systemCode, userIds[i], grantOrganizeId, permissionCode, containChild);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 授权组织
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="grantOrganizeIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="containChild"></param>
        /// <returns></returns>
        public int GrantOrganizes(string systemCode, string[] userIds, string[] grantOrganizeIds, string permissionCode, bool containChild = false)
        {
            var result = 0;
            for (var i = 0; i < userIds.Length; i++)
            {
                for (var j = 0; j < grantOrganizeIds.Length; j++)
                {
                    GrantOrganize(ScopeManager, systemCode, userIds[i], grantOrganizeIds[j], permissionCode, containChild);
                    result++;
                }
            }
            return result;
        }


        //
        //  撤销授权范围的实现部分
        //

        #region public int RevokeOrganize(string userId, string revokeOrganizeId, string permissionCode) 用户撤销授权
        /// <summary>
        /// 用户撤销授权
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeOrganizeId"></param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        public int RevokeOrganize(string systemCode, string userId, string revokeOrganizeId, string permissionCode = "Resource.AccessPermission")
        {
            return RevokeOrganize(ScopeManager, systemCode, userId, revokeOrganizeId, permissionCode);
        }
        #endregion

        #region private int RevokeOrganize(BasePermissionScopeManager manager, string userId, string revokeOrganizeId, string permissionCode) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="manager">权限域读写器</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="revokeOrganizeId">权限主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        private int RevokeOrganize(BasePermissionScopeManager manager, string systemCode, string userId, string revokeOrganizeId, string permissionCode)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseOrganizeEntity.TableName)
            };
            if (!string.IsNullOrEmpty(revokeOrganizeId))
            {
                parameters.Add(new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetId, revokeOrganizeId));
            }
            parameters.Add(new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode)));
            return manager.Delete(parameters);
        }
        #endregion

        #region public int RevokeOrganize(string userId, string permissionCode) 用户撤销授权
        /// <summary>
        /// 用户撤销授权
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>主键</returns>
        public int RevokeOrganize(string systemCode, string userId, string permissionCode)
        {
            return RevokeOrganize(ScopeManager, systemCode, userId, permissionCode, null);
        }
        #endregion

        
        /// <summary>
        /// 撤回组织
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <param name="revokeOrganizeIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeOrganizes(string systemCode, string userId, string[] revokeOrganizeIds, string permissionCode)
        {
            var result = 0;
            for (var i = 0; i < revokeOrganizeIds.Length; i++)
            {
                RevokeOrganize(ScopeManager, systemCode, userId, revokeOrganizeIds[i], permissionCode);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 撤回组织
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="revokeOrganizeId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeOrganizes(string systemCode, string[] userIds, string revokeOrganizeId, string permissionCode)
        {
            var result = 0;
            for (var i = 0; i < userIds.Length; i++)
            {
                RevokeOrganize(ScopeManager, systemCode, userIds[i], revokeOrganizeId, permissionCode);
                result++;
            }
            return result;
        }
        /// <summary>
        /// 撤回组织
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="revokeOrganizeIds"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public int RevokeOrganizes(string systemCode, string[] userIds, string[] revokeOrganizeIds, string permissionCode)
        {
            var result = 0;
            for (var i = 0; i < userIds.Length; i++)
            {
                for (var j = 0; j < revokeOrganizeIds.Length; j++)
                {
                    RevokeOrganize(ScopeManager, systemCode, userIds[i], revokeOrganizeIds[j], permissionCode);
                    result++;
                }
            }
            return result;
        }
    }
}