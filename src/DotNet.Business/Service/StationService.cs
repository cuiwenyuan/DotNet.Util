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
    /// StationService
    /// 站点管理服务
    /// 
    /// 修改记录
    /// 
    ///		2015.02.24 版本：1.0 JiRiGaLa 窗体与数据库连接的分离。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.02.24</date>
    /// </author> 
    /// </summary>


    public partial class StationService : IStationService
    {
        #region public DataTable GetDataTable(BaseUserInfo userInfo) 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(BaseUserInfo userInfo)
        {
            var dt = new DataTable(BaseRoleEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Role";
                // 获得角色列表
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                var parameters = new List<KeyValuePair<string, object>>();
                // parameters.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0));
                // parameters.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1)); //如果1 只显示有效用户
                // parameters.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldIsVisible, 1));
                // dt = manager.GetDataTable(parameters, BaseRoleEntity.FieldSortCode);
                dt = manager.GetDataTable(0, BaseRoleEntity.FieldSortCode);
                dt.TableName = BaseRoleEntity.TableName;
            });
            return dt;
        }
        #endregion
    }
}