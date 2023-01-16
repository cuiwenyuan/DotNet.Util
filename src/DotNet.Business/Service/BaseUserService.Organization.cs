//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
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
    /// BaseUserService
    /// 用户管理服务
    /// 
    /// 修改记录
    /// 
    ///		2012.03.27 版本：1.0 JiRiGaLa 窗体与数据库连接的分离。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.03.27</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserService : IBaseUserService
    {
        #region public bool UserIsInOrganization(BaseUserInfo userInfo, string userId, string organizationName)
        /// <summary>
        /// 用户是否在某个组织架构里的判断
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="organizationName">部门名称</param>
        /// <returns>存在</returns>
        public bool UserIsInOrganization(BaseUserInfo userInfo, string userId, string organizationName)
        {
            var result = false;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                result = userManager.IsInOrganization(userId, organizationName);
            });
            return result;
        }
        #endregion

        #region public List<BaseUserEntity> GetListByDepartment(BaseUserInfo userInfo, string departmentId, bool containChildren) 按部门获取部门用户
        /// <summary>
        /// 按部门获取用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="departmentId">部门主键</param>
        /// <param name="containChildren">含子部门</param>
        /// <returns>数据表</returns>
        public List<BaseUserEntity> GetListByDepartment(BaseUserInfo userInfo, string departmentId, bool containChildren)
        {
            var entityList = new List<BaseUserEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                if (string.IsNullOrEmpty(departmentId))
                {
                    entityList = userManager.GetList<BaseUserEntity>(new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
                        , 200, BaseUserEntity.FieldSortCode);
                }
                else
                {
                    if (containChildren)
                    {
                        entityList = userManager.GetChildrenUserList(departmentId);
                    }
                    else
                    {
                        entityList = userManager.GetListByDepartment(departmentId);
                    }
                }
            });
            return entityList;
        }
        #endregion

        #region public DataTable GetDataTableByDepartment(BaseUserInfo userInfo, string departmentId, bool containChildren) 按部门获取部门用户
        /// <summary>
        /// 按部门获取用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="departmentId">部门主键</param>
        /// <param name="containChildren">含子部门</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByDepartment(BaseUserInfo userInfo, string departmentId, bool containChildren)
        {
            var dt = new DataTable(BaseUserEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                if (string.IsNullOrEmpty(departmentId))
                {
                    dt = userManager.GetDataTable(new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
                    , 200, BaseUserEntity.FieldSortCode);
                }
                else
                {
                    if (containChildren)
                    {
                        dt = userManager.GetChildrenUserDataTable(departmentId);
                    }
                    else
                    {
                        dt = userManager.GetDataTableByDepartment(departmentId);
                    }
                }
                dt.TableName = BaseUserEntity.CurrentTableName;
            });
            return dt;
        }
        #endregion

        #region public DataTable GetUserOrganizationDT(BaseUserInfo userInfo, string userId)
        /// <summary>
        /// 获得用户的组织机构兼职情况
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="userId">用户主键</param>
        /// <returns>数据表</returns>
        public DataTable GetUserOrganizationDT(BaseUserInfo userInfo, string userId)
        {
            var dt = new DataTable(BaseUserOrganizationEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userOrganizationManager = new BaseUserOrganizationManager(dbHelper, userInfo);
                dt = userOrganizationManager.GetUserOrganization(userId);
                dt.TableName = BaseUserOrganizationEntity.CurrentTableName;
            });

            return dt;
        }
        #endregion

        #region public string AddUserToOrganization(BaseUserInfo userInfo, BaseUserOrganizationEntity entity, out Status status, out string statusMessage)
        /// <summary>
        /// 把用户添加到组织机构
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">用户组织机构关系</param>
        /// <param name="status">状态</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>主键</returns>
        public string AddUserToOrganization(BaseUserInfo userInfo, BaseUserOrganizationEntity entity, out Status status, out string statusMessage)
        {
            var returnCode = Status.Ok;
            var returnMessage = string.Empty;
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userOrganizationManager = new BaseUserOrganizationManager(dbHelper, userInfo);
                result = userOrganizationManager.Add(entity, out returnCode);
                returnMessage = userOrganizationManager.GetStateMessage(returnCode);
            });
            status = returnCode;
            statusMessage = returnMessage;
            return result;
        }
        #endregion

        #region public int BatchDeleteUserOrganization(BaseUserInfo userInfo, string[] ids)
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int BatchDeleteUserOrganization(BaseUserInfo userInfo, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userOrganizationManager = new BaseUserOrganizationManager(dbHelper, userInfo);
                result = userOrganizationManager.Delete(ids);
            });
            return result;
        }
        #endregion
    }
}