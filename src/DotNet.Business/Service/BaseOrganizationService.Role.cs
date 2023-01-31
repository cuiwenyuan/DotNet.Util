//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System.Data;
using System.Reflection;

namespace DotNet.Business
{
    using IService;
    using Model;
    using Util;

    /// <summary>
    /// BaseOrganizationService
    /// 用户管理服务
    /// 
    /// 修改记录
    /// 
    ///		2014.04.15 版本：1.0 JiRiGaLa 窗体与数据库连接的分离。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2014.04.15</date>
    /// </author> 
    /// </summary>
    public partial class BaseOrganizationService : IBaseOrganizationService
    {
        #region public DataTable GetDataTableByRole(BaseUserInfo userInfo, string[] roleIds)
        /// <summary>
        /// 按角色获取用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleIds">角色主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByRole(BaseUserInfo userInfo, string[] roleIds)
        {
            var dt = new DataTable(BaseRoleEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                dt = manager.GetDataTableByRole(userInfo.SystemCode, roleIds);
                dt.TableName = BaseOrganizationEntity.CurrentTableName;
                dt.DefaultView.Sort = BaseOrganizationEntity.FieldSortCode;
            });

            return dt;
        }
        #endregion
    }
}