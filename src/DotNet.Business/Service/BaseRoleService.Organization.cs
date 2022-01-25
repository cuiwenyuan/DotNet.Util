//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Data;
using System.Reflection;

namespace DotNet.Business
{
    using IService;
    using Model;
    using Util;

    /// <summary>
    /// BaseRoleService
    /// 角色管理服务
    /// 
    /// 修改记录
    /// 
    ///		2015.12.10 版本：1.1 JiRiGaLa 代码整理。
    ///		2014.04.15 版本：1.0 JiRiGaLa 分离程序。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2014.04.15</date>
    /// </author> 
    /// </summary>
    public partial class BaseRoleService : IBaseRoleService
    {
        #region public DataTable GetRoleOrganizationDataTable(BaseUserInfo userInfo, string systemCode, string roleId)
        /// <summary>
        /// 获取角色的所有组织机构列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>列表</returns>
        public DataTable GetRoleOrganizationDataTable(BaseUserInfo userInfo, string systemCode, string roleId)
        {
            var result = new DataTable(BaseOrganizationEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var roleManager = new BaseRoleManager(userInfo);
                result = roleManager.GetOrganizationDataTable(systemCode, roleId);
                result.TableName = BaseOrganizationEntity.CurrentTableName;
            });

            return result;
        }
        #endregion

        /// <summary>
        /// 获得角色中的组织机构主键
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>组织机构主键</returns>
        public string[] GetRoleOrganizationIds(BaseUserInfo userInfo, string roleId)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                result = manager.GetIdsInRole(userInfo.SystemCode, roleId);
            });

            return result;
        }

        /// <summary>
        /// 组织机构添加到角色
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="organizationIds">组织机构主键</param>
        /// <returns>影响行数</returns>
        public int AddOrganizationToRole(BaseUserInfo userInfo, string roleId, string[] organizationIds)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                // 小心异常，检查一下参数的有效性
                if (organizationIds != null)
                {
                    result += manager.AddToRole(userInfo.SystemCode, organizationIds, roleId);
                }
            });

            return result;
        }

        /// <summary>
        /// 将组织机构从角色中移除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="organizationIds">组织机构主键</param>
        /// <returns>影响行数</returns>
        public int RemoveOrganizationFromRole(BaseUserInfo userInfo, string roleId, string[] organizationIds)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDbWithTransaction(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                if (organizationIds != null)
                {
                    result += manager.RemoveFormRole(userInfo.SystemCode, organizationIds, roleId);
                }
            });

            return result;
        }

        /// <summary>
        /// 清除角色组织机构关联
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>影响行数</returns>
        public int ClearOrganizationUser(BaseUserInfo userInfo, string roleId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                result = manager.ClearOrganization(userInfo.SystemCode, roleId);
            });

            return result;
        }

        /// <summary>
        /// 设置角色中的组织机构
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="organizationIds">组织机构主键数组</param>
        /// <returns>影响行数</returns>
        public int SetOrganizationToRole(BaseUserInfo userInfo, string roleId, string[] organizationIds)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                result = manager.ClearOrganization(userInfo.SystemCode, roleId);
                // 小心异常，检查一下参数的有效性
                if (organizationIds != null)
                {
                    result += manager.AddToRole(userInfo.SystemCode, organizationIds, roleId);
                }
            });

            return result;
        }
    }
}