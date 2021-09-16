//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
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
    /// UserService
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
    public partial class UserService : IUserService
    {
        #region public bool UserIsInOrganize(BaseUserInfo userInfo, string userId, string organizeName)
        /// <summary>
        /// 用户是否在某个组织架构里的判断
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="organizeName">部门名称</param>
        /// <returns>存在</returns>
        public bool UserIsInOrganize(BaseUserInfo userInfo, string userId, string organizeName)
        {
            var result = false;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                result = userManager.IsInOrganize(userId, organizeName);
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
            var dt = new DataTable(BaseUserEntity.TableName);

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
                dt.TableName = BaseUserEntity.TableName;
            });
            return dt;
        }
        #endregion

        #region public DataTable GetUserOrganizeDT(BaseUserInfo userInfo, string userId)
        /// <summary>
        /// 获得用户的组织机构兼职情况
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="userId">用户主键</param>
        /// <returns>数据表</returns>
        public DataTable GetUserOrganizeDT(BaseUserInfo userInfo, string userId)
        {
            var dt = new DataTable(BaseUserOrganizeEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userOrganizeManager = new BaseUserOrganizeManager(dbHelper, userInfo);
                dt = userOrganizeManager.GetUserOrganizeDt(userId);
                dt.TableName = BaseUserOrganizeEntity.TableName;
            });

            return dt;
        }
        #endregion

        #region public string AddUserToOrganize(BaseUserInfo userInfo, BaseUserOrganizeEntity entity, out string statusCode, out string statusMessage)
        /// <summary>
        /// 把用户添加到组织机构
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">用户组织机构关系</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>主键</returns>
        public string AddUserToOrganize(BaseUserInfo userInfo, BaseUserOrganizeEntity entity, out string statusCode, out string statusMessage)
        {
            var returnCode = string.Empty;
            var returnMessage = string.Empty;
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userOrganizeManager = new BaseUserOrganizeManager(dbHelper, userInfo);
                result = userOrganizeManager.Add(entity, out returnCode);
                returnMessage = userOrganizeManager.GetStateMessage(returnCode);
            });
            statusCode = returnCode;
            statusMessage = returnMessage;
            return result;
        }
        #endregion

        #region public int BatchDeleteUserOrganize(BaseUserInfo userInfo, string[] ids)
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int BatchDeleteUserOrganize(BaseUserInfo userInfo, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userOrganizeManager = new BaseUserOrganizeManager(dbHelper, userInfo);
                result = userOrganizeManager.Delete(ids);
            });
            return result;
        }
        #endregion
    }
}