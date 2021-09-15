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
    /// ServicesLicenseService
    /// 参数服务
    /// 
    /// 修改记录
    /// 
    ///		2015.12.26 版本：1.0 JiRiGaLa 创建。
    ///	
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.12.26</date>
    /// </author> 
    /// </summary>


    public class ServicesLicenseService : IServicesLicenseService
    {
        #region public DataTable GetDataTableByUser(BaseUserInfo userInfo, string userId) 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByUser(BaseUserInfo userInfo, string userId)
        {
            var result = new DataTable(BaseServicesLicenseEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseServicesLicenseManager(dbHelper, userInfo);
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseServicesLicenseEntity.FieldUserId, userId),
                    new KeyValuePair<string, object>(BaseServicesLicenseEntity.FieldDeleted, 0),
                    new KeyValuePair<string, object>(BaseServicesLicenseEntity.FieldEnabled, 1)
                };
                result = manager.GetDataTable(parameters);
                result.TableName = BaseServicesLicenseEntity.TableName;
            });

            return result;
        }
        #endregion

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseUserInfo userInfo, BaseServicesLicenseEntity entity)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDbWithTransaction(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseServicesLicenseManager(dbHelper, userInfo);
                result = manager.AddEntity(entity);
            });

            return result;
        }

        #region public BaseServicesLicenseEntity GetEntity(BaseUserInfo userInfo, string id)
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public BaseServicesLicenseEntity GetEntity(BaseUserInfo userInfo, string id)
        {
            BaseServicesLicenseEntity entity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseServicesLicenseManager(dbHelper, userInfo);
                entity = manager.GetEntity(id);
            });

            return entity;
        }
        #endregion

        #region public int Update(BaseUserInfo userInfo, string tableName, BaseParameterEntity entity)
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="entity">实体</param>
        /// <returns>影响行数</returns>
        public int Update(BaseUserInfo userInfo, string tableName, BaseServicesLicenseEntity entity)
        {
            var result = 0;

            var returnCode = string.Empty;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseServicesLicenseManager(dbHelper, userInfo, tableName);
                result = manager.UpdateEntity(entity);
            });

            return result;
        }
        #endregion

        #region public int SetDeleted(BaseUserInfo userInfo, string tableName, string[] ids)
        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int SetDeleted(BaseUserInfo userInfo, string tableName, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseServicesLicenseManager(dbHelper, userInfo, tableName);
                for (var i = 0; i < ids.Length; i++)
                {
                    // 设置为删除状态
                    result += manager.SetDeleted(ids[i], true, true, 4);
                }
            });

            return result;
        }
        #endregion
    }
}