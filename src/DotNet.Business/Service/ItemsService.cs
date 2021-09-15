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
    /// ItemsService
    /// 基础主键服务
    /// 
    /// 修改记录
    /// 
    ///		2013.06.06 张祈璟重构
    ///		2013.03.10 版本：2.4 JiRiGaLa 连接业务系统表。
    ///		2008.10.23 版本：2.3 JiRiGaLa 表明可以自己定义，可以控制多个表。
    ///		2007.08.15 版本：2.2 JiRiGaLa 改进运行速度采用 WebService 变量定义 方式处理数据。
    ///		2007.08.14 版本：2.1 JiRiGaLa 改进运行速度采用 Instance 方式处理数据。
    ///		2007.05.15 版本：1.0 JiRiGaLa 基础编码管理。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2013.03.10</date>
    /// </author> 
    /// </summary>


    public class ItemsService : IBaseItemsService
    {
        #region public DataTable GetDataTable(BaseUserInfo userInfo) 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(BaseUserInfo userInfo)
        {
            var dt = new DataTable(BaseItemsEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Items";
                var manager = new BaseItemsManager(dbHelper, userInfo, tableName);
                // 若是系统管理员，那就返回全部数据
                if (BaseUserManager.IsAdministrator(userInfo.Id))
                {
                    dt = manager.GetDataTable();
                }
                else
                {
                    // 按数据权限来过滤数据
                    var permissionScopeManager = new BasePermissionScopeManager(dbHelper, userInfo);
                    var ids = permissionScopeManager.GetResourceScopeIds(userInfo.SystemCode, userInfo.Id, BaseItemsEntity.TableName, "Resource.ManagePermission");
                    dt = manager.GetDataTable(ids);
                }
                dt.TableName = tableName;
            });
            return dt;
        }
        #endregion

        #region public List<BaseItemsEntity> GetList(BaseUserInfo userInfo) 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public List<BaseItemsEntity> GetList(BaseUserInfo userInfo)
        {
            List<BaseItemsEntity> result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = BaseItemsEntity.TableName;
                if (userInfo != null && !string.IsNullOrEmpty(userInfo.SystemCode))
                {
                    tableName = userInfo.SystemCode + "Items";
                }
                var itemsManager = new BaseItemsManager(dbHelper, userInfo, tableName);
                result = itemsManager.GetList<BaseItemsEntity>();
            });
            return result;
        }
        #endregion

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public BaseItemsEntity GetEntity(BaseUserInfo userInfo, string id)
        {
            BaseItemsEntity itemsEntity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Items";
                var manager = new BaseItemsManager(dbHelper, userInfo, tableName);
                itemsEntity = manager.GetEntity(id);
            });
            return itemsEntity;
        }

        #region public DataTable GetDataTableByParent(BaseUserInfo userInfo, string parentId) 按父节点获取列表
        /// <summary>
        /// 按父节点获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">父级主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByParent(BaseUserInfo userInfo, string parentId)
        {
            var dt = new DataTable(BaseItemsEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Items";
                var manager = new BaseItemsManager(dbHelper, userInfo, tableName);
                dt = manager.GetDataTableByParent(parentId);
                dt.TableName = tableName;
            });
            return dt;
        }
        #endregion

        #region public string Add(BaseUserInfo userInfo, BaseItemsEntity entity, out string statusCode, out string statusMessage) 添加实体
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状态信息</param>
        /// <returns>数据表</returns>
        public string Add(BaseUserInfo userInfo, BaseItemsEntity entity, out string statusCode, out string statusMessage)
        {
            var result = string.Empty;

            var returnCode = string.Empty;
            var returnMessage = string.Empty;
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Items";
                var manager = new BaseItemsManager(dbHelper, userInfo, tableName);
                // 调用方法，并且返回运行结果
                result = manager.Add(entity, out returnCode);
                returnMessage = manager.GetStateMessage(returnCode);
            });
            statusCode = returnCode;
            statusMessage = returnMessage;
            return result;
        }
        #endregion

        #region public int Update(BaseUserInfo userInfo, BaseItemsEntity entity, out string statusCode, out string statusMessage) 更新实体
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状态信息</param>
        /// <returns>数据表</returns>
        public int Update(BaseUserInfo userInfo, BaseItemsEntity entity, out string statusCode, out string statusMessage)
        {
            var result = 0;

            var returnCode = string.Empty;
            var returnMessage = string.Empty;
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Items";
                var manager = new BaseItemsManager(dbHelper, userInfo, tableName);
                // 更新数据
                result = manager.Update(entity, out returnCode);
                returnMessage = manager.GetStateMessage(returnCode);
            });
            statusCode = returnCode;
            statusMessage = returnMessage;

            return result;
        }
        #endregion

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="statusCode">状态返回码</param>
        /// <param name="statusMessage">状态返回信息</param>
        public void CreateTable(BaseUserInfo userInfo, string tableName, out string statusCode, out string statusMessage)
        {
            var returnCode = string.Empty;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                if (!DbUtil.Exists(dbHelper, tableName))
                {
                    var manager = new BaseItemsManager(dbHelper, userInfo);
                    // 创建表结构
                    manager.CreateTable(tableName, out returnCode);
                    returnMessage = manager.GetStateMessage(returnCode);
                }
            });
            statusCode = returnCode;
            statusMessage = returnMessage;
        }

        #region public int SetDeleted(BaseUserInfo userInfo, string tableName, string[] ids) 批量删除标志
        /// <summary>
        /// 批量删除标志
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">目标表</param>
        /// <param name="ids">主键数组</param>
        public int SetDeleted(BaseUserInfo userInfo, string tableName, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseItemsManager(dbHelper, userInfo, tableName);
                result = manager.SetDeleted(ids);
            });
            return result;
        }
        #endregion

        #region public int Delete(BaseUserInfo userInfo, string tableName, string id) 删除
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">目标表</param>
        /// <param name="id">主键</param>
        /// <returns>影响的行数</returns>
        public int Delete(BaseUserInfo userInfo, string tableName, string id)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseItemsManager(dbHelper, userInfo, tableName);
                result = manager.Delete(id);
            });
            return result;
        }
        #endregion

        #region public int BatchDelete(BaseUserInfo userInfo, string tableName, string[] ids) 批量删除实体
        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">目标表</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int BatchDelete(BaseUserInfo userInfo, string tableName, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseItemsManager(dbHelper, userInfo, tableName);
                result = manager.Delete(ids);
            });
            return result;
        }
        #endregion

        #region public int BatchMoveTo(BaseUserInfo userInfo, string tableName, string[] ids, string targetId) 批量移动数据
        /// <summary>
        /// 批量移动数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">目标表</param>
        /// <param name="ids">编码主键数组</param>
        /// <param name="targetId">目标主键</param>
        /// <returns>影响行数</returns>
        public int BatchMoveTo(BaseUserInfo userInfo, string tableName, string[] ids, string targetId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseItemsManager(dbHelper, userInfo, tableName);
                for (var i = 0; i < ids.Length; i++)
                {
                    result += manager.SetProperty(ids[i], new KeyValuePair<string, object>(BaseItemsEntity.FieldParentId, targetId));
                }
            });
            return result;
        }
        #endregion

        #region public int BatchSave(BaseUserInfo userInfo, DataTable result) 批量保存数据
        /// <summary>
        /// 批量保存数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="dt">数据表</param>
        /// <returns>影响行数</returns>
        public int BatchSave(BaseUserInfo userInfo, DataTable dt)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Items";
                var manager = new BaseItemsManager(dbHelper, userInfo, tableName);
                result = manager.BatchSave(dt);
            });

            return result;
        }
        #endregion
    }
}