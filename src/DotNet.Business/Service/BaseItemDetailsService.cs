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
    /// BaseItemDetailsService
    /// 基础主键服务
    /// 
    /// 修改记录
    /// 
    ///		2013.06.05 张祈璟重构
    ///		2008.10.23 版本：2.3 JiRiGaLa 表明可以自己定义，可以控制多个表。
    ///		2007.08.15 版本：2.2 JiRiGaLa 改进运行速度采用 WebService 变量定义 方式处理数据。
    ///		2007.08.14 版本：2.1 JiRiGaLa 改进运行速度采用 Instance 方式处理数据。
    ///		2007.05.15 版本：1.0 JiRiGaLa 基础编码管理。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2007.08.15</date>
    /// </author> 
    /// </summary>


    public class BaseItemDetailsService : IBaseItemDetailsService
    {
        #region public DataTable GetDataTable(BaseUserInfo userInfo, string tableName) 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">目标表</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(BaseUserInfo userInfo, string tableName)
        {
            var dt = new DataTable(tableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, tableName);
                // 若是系统管理员，那就返回全部数据
                dt = itemDetailsManager.GetDataTable(0, BaseItemDetailsEntity.FieldSortCode);
                dt.TableName = tableName;
                // 管理时需要把所有的数据显示出来，所以无效的数据也需要显示的
                // , new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldEnabled, 1)
                // 管理时需要把被删除的也需要都显示出来，还能恢复数据才可以
                // new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldDeleted, 0)
            });
            return dt;
        }
        #endregion

        #region public DataTable GetDataTableByPermission(BaseUserInfo userInfo, string tableName, string permissionCode = "Resource.ManagePermission") 按操作权限获取列表
        /// <summary>
        /// 按操作权限获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">目标表</param>
        /// <param name="permissionCode">操作权限</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPermission(BaseUserInfo userInfo, string tableName, string permissionCode = "Resource.ManagePermission")
        {
            DataTable result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, tableName);

                // 管理员取得所有数据
                if (BaseUserManager.IsAdministrator(userInfo.Id))
                {
                    result = itemDetailsManager.GetDataTable(
                        new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldDeleted, 0)
                        , BaseItemDetailsEntity.FieldSortCode);
                }
                else
                {
                    // 管理时需要把所有的数据显示出来，所以无效的数据也需要显示的
                    // , new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldEnabled, 1)

                    // 按数据权限来过滤数据
                    var permissionScopeManager = new BasePermissionScopeManager(dbHelper, userInfo);
                    var ids = permissionScopeManager.GetResourceScopeIds(userInfo.SystemCode, userInfo.Id, tableName, permissionCode);
                    result = itemDetailsManager.GetDataTable(ids);
                }
                result.TableName = tableName;
            });
            return result;
        }
        #endregion

        #region public List<BaseItemDetailsEntity> GetListByPermission(BaseUserInfo userInfo, string tableName, string permissionCode = "Resource.ManagePermission") 按操作权限获取列表
        /// <summary>
        /// 按操作权限获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">目标表</param>
        /// <param name="permissionCode">操作权限</param>
        /// <returns>数据表</returns>
        public List<BaseItemDetailsEntity> GetListByPermission(BaseUserInfo userInfo, string tableName, string permissionCode = "Resource.ManagePermission")
        {
            List<BaseItemDetailsEntity> result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, tableName);

                // 管理员取得所有数据
                if (BaseUserManager.IsAdministrator(userInfo.Id))
                {
                    result = itemDetailsManager.GetList<BaseItemDetailsEntity>(
                        new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldDeleted, 0)
                        , BaseItemDetailsEntity.FieldSortCode);
                }
                else
                {
                    // 管理时需要把所有的数据显示出来，所以无效的数据也需要显示的
                    // , new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldEnabled, 1)

                    // 按数据权限来过滤数据
                    var permissionScopeManager = new BasePermissionScopeManager(dbHelper, userInfo);
                    var ids = permissionScopeManager.GetResourceScopeIds(userInfo.SystemCode, userInfo.Id, tableName, permissionCode);
                    result = itemDetailsManager.GetList<BaseItemDetailsEntity>(ids);
                }
            });
            return result;
        }
        #endregion

        #region public List<BaseItemDetailsEntity> GetList(BaseUserInfo userInfo, string tableName) 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">目标表</param>
        /// <returns>列表</returns>
        public List<BaseItemDetailsEntity> GetList(BaseUserInfo userInfo, string tableName)
        {
            var entityList = new List<BaseItemDetailsEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, tableName);
                entityList = itemDetailsManager.GetList<BaseItemDetailsEntity>(
                    new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldDeleted, 0)
                    , BaseItemDetailsEntity.FieldSortCode);
                // 管理时需要把所有的数据显示出来，所以无效的数据也需要显示的
                // , new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldEnabled, 1)
            });
            return entityList;
        }
        #endregion

        #region public DataTable GetDataTableByParent(BaseUserInfo userInfo, string tableName, string parentId) 获取子列表
        /// <summary>
        /// 获取子列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">目标表</param>
        /// <param name="parentId">父级主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByParent(BaseUserInfo userInfo, string tableName, string parentId)
        {
            var dt = new DataTable(tableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, tableName);
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldParentId, parentId),
                    new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldDeleted, 0)
                };
                dt = itemDetailsManager.GetDataTable(parameters, 0, BaseItemDetailsEntity.FieldSortCode);
                // result = itemDetailsManager.GetDataTableByParent(parentId);
                dt.TableName = tableName;
            });

            return dt;
        }
        #endregion

        /// <summary>
        /// GetDataTableByCode
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="userInfo"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public DataTable GetDataTableByCode(IDbHelper dbHelper, BaseUserInfo userInfo, string code)
        {
            var result = new DataTable(BaseItemDetailsEntity.TableName);
            // 检查有其他目标数据库表存储了数据
            var itemsManager = new BaseItemsManager(dbHelper, userInfo);
            var itemsEntity = BaseEntity.Create<BaseItemsEntity>(itemsManager.GetDataTable(new KeyValuePair<string, object>(BaseItemsEntity.FieldCode, code)));
            if (!string.IsNullOrEmpty(itemsEntity.TargetTable))
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, itemsEntity.TargetTable);
                // 这里只要有效的，没被删除的
                var parameters = new List<KeyValuePair<string, object>>
                {
                    // 管理的时候无效的也需要被管理
                    new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldDeleted, 0)
                };

                result = itemDetailsManager.GetDataTable(parameters, BaseItemDetailsEntity.FieldSortCode);
                result.TableName = itemsEntity.TargetTable;
            }
            return result;
        }
        /// <summary>
        /// GetListByCode
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="userInfo"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<BaseItemDetailsEntity> GetListByCode(IDbHelper dbHelper, BaseUserInfo userInfo, string code)
        {
            List<BaseItemDetailsEntity> result = null;
            // 检查有其他目标数据库表存储了数据
            var itemsManager = new BaseItemsManager(dbHelper, userInfo);
            var itemsEntity = BaseEntity.Create<BaseItemsEntity>(itemsManager.GetDataTable(new KeyValuePair<string, object>(BaseItemsEntity.FieldCode, code)));
            if (itemsEntity != null && !string.IsNullOrEmpty(itemsEntity.TargetTable))
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, itemsEntity.TargetTable);
                // 这里只要有效的，没被删除的
                var parameters = new List<KeyValuePair<string, object>>
                {
                    // 管理的时候无效的也需要被管理
                    new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldDeleted, 0)
                };
                result = itemDetailsManager.GetList<BaseItemDetailsEntity>(parameters, BaseItemDetailsEntity.FieldSortCode);
            }
            return result;
        }
        /// <summary>
        /// GetListByCode
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<BaseItemDetailsEntity> GetListByCode(BaseUserInfo userInfo, string code)
        {
            var entityList = new List<BaseItemDetailsEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                entityList = GetListByCode(dbHelper, userInfo, code);
            });

            return entityList;
        }

        /// <summary>
        /// 按编号获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="code">编号</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByCode(BaseUserInfo userInfo, string code)
        {
            var dt = new DataTable(BaseItemDetailsEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                dt = GetDataTableByCode(dbHelper, userInfo, code);
            });

            return dt;
        }
        /// <summary>
        /// 根据目标表获取列表
        /// </summary>
        /// <param name="userInfo">用户名</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public List<BaseItemDetailsEntity> GetListByTargetTable(BaseUserInfo userInfo, string tableName)
        {
            var result = new List<BaseItemDetailsEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                result = GetListByTargetTable(dbHelper, userInfo, tableName);
            });

            return result;
        }
        /// <summary>
        /// 根据目标表获取列表
        /// </summary>
        /// <param name="dbHelper">数据库帮助接口</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public List<BaseItemDetailsEntity> GetListByTargetTable(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
        {
            List<BaseItemDetailsEntity> result = null;
            // 检查有其他目标数据库表存储了数据
            var itemsManager = new BaseItemsManager(dbHelper, userInfo);
            var itemsEntity = BaseEntity.Create<BaseItemsEntity>(itemsManager.GetDataTable(new KeyValuePair<string, object>(BaseItemsEntity.FieldTargetTable, tableName)));
            if (!string.IsNullOrEmpty(itemsEntity.TargetTable))
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, itemsEntity.TargetTable);
                // 这里只要有效的，没被删除的
                var parameters = new List<KeyValuePair<string, object>>
                {
                    // 管理的时候无效的也需要被管理
                    new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldDeleted, 0)
                };
                result = itemDetailsManager.GetList<BaseItemDetailsEntity>(parameters, BaseItemDetailsEntity.FieldSortCode);
            }
            return result;
        }


        /// <summary>
        /// 批量获取选项数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="codes">编号数组</param>
        /// <returns>数据权限合</returns>
        public DataSet GetDataSetByCodes(BaseUserInfo userInfo, string[] codes)
        {
            var dataSet = new DataSet();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                for (var i = 0; i < codes.Length; i++)
                {
                    var dt = GetDataTableByCode(dbHelper, userInfo, codes[i]);
                    dt.TableName = codes[i];
                    dataSet.Tables.Add(dt);
                }
            });
            return dataSet;
        }

        #region public BaseItemDetailsEntity GetEntity(BaseUserInfo userInfo, string tableName, string id) 获取實體
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="id">主键</param>
        /// <returns>数据表</returns>
        public BaseItemDetailsEntity GetEntity(BaseUserInfo userInfo, string tableName, string id)
        {
            var dt = new DataTable(BaseItemDetailsEntity.TableName);
            BaseItemDetailsEntity entity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, tableName);
                entity = itemDetailsManager.GetEntity(id);
            });
            return entity;
        }
        #endregion

        #region public BaseItemDetailsEntity GetEntityByCode(BaseUserInfo userInfo, string tableName, string code) 获取實體
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="code">编码</param>
        /// <returns>数据表</returns>
        public BaseItemDetailsEntity GetEntityByCode(BaseUserInfo userInfo, string tableName, string code)
        {
            var itemDetailsEntity = new BaseItemDetailsEntity();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, tableName);
                var dt = itemDetailsManager.GetDataTable(new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldItemCode, code), BaseItemDetailsEntity.FieldSortCode);
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    itemDetailsEntity = (BaseItemDetailsEntity)itemDetailsEntity.GetFrom(dt.Rows[0]);
                }
            });
            return itemDetailsEntity;
        }
        #endregion


        #region public string Add(BaseUserInfo userInfo, string tableName, BaseItemDetailsEntity entity, out string statusCode, out string statusMessage) 添加编码
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状态信息</param>
        /// <returns>数据表</returns>
        public string Add(BaseUserInfo userInfo, string tableName, BaseItemDetailsEntity entity, out string statusCode, out string statusMessage)
        {
            var result = string.Empty;

            var returnCode = string.Empty;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, tableName);
                // 调用方法，并且返回运行结果
                result = itemDetailsManager.Add(entity, out returnCode);
                returnMessage = itemDetailsManager.GetStateMessage(returnCode);
            });
            statusCode = returnCode;
            statusMessage = returnMessage;
            return result;
        }
        #endregion

        #region public int Update(BaseUserInfo userInfo, string tableName, BaseItemDetailsEntity entity, out string statusCode, out string statusMessage) 更新编码
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状态信息</param>
        /// <returns>数据表</returns>
        public int Update(BaseUserInfo userInfo, string tableName, BaseItemDetailsEntity entity, out string statusCode, out string statusMessage)
        {
            var result = 0;

            var returnCode = string.Empty;
            var returnMessage = string.Empty;
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, tableName);
                // 编辑数据
                result = itemDetailsManager.Update(entity, out returnCode);
                returnMessage = itemDetailsManager.GetStateMessage(returnCode);
            });
            statusCode = returnCode;
            statusMessage = returnMessage;
            return result;
        }
        #endregion

        #region public int Delete(BaseUserInfo userInfo, string tableName, string id) 删除實體
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
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, tableName);
                result = itemDetailsManager.Delete(id);
            });
            return result;
        }
        #endregion

        #region public int BatchMoveTo(BaseUserInfo userInfo, string tableName, string[] ids, string targetId) 批量移动
        /// <summary>
        /// 批量移动
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
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, tableName);
                for (var i = 0; i < ids.Length; i++)
                {
                    result += itemDetailsManager.SetProperty(ids[i], new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldParentId, targetId));
                }
            });
            return result;
        }
        #endregion

        #region public int BatchDelete(BaseUserInfo userInfo, string tableName, string[] ids) 批量删除编码
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">目标表</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int BatchDelete(BaseUserInfo userInfo, string tableName, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, tableName);
                result = itemDetailsManager.Delete(ids);
            });
            return result;
        }
        #endregion

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
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, tableName);
                result = itemDetailsManager.SetDeleted(ids);
            });
            return result;
        }

        #region public int BatchSave(BaseUserInfo userInfo, DataTable dt) 批量保存更新
        /// <summary>
        /// 批量保存更新
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="dt">数据表</param>
        /// <returns>影响行数</returns>
        public int BatchSave(BaseUserInfo userInfo, DataTable dt)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, dt.TableName);
                result = itemDetailsManager.BatchSave(dt);
            });
            return result;
        }
        #endregion

        #region public int Save(BaseUserInfo userInfo, DataTable dt) 批量保存
        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="dt">数据表</param>
        /// <returns>影响行数</returns>
        public int Save(BaseUserInfo userInfo, DataTable dt)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, dt.TableName);
                result = itemDetailsManager.Save(dt);
            });
            return result;
        }
        #endregion


        /// <summary>
        /// 移动数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="id">主键</param>
        /// <param name="parentId">父主键</param>
        /// <returns>影响行数</returns>
        public int MoveTo(BaseUserInfo userInfo, string tableName, string id, string parentId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, tableName);
                result = itemDetailsManager.MoveTo(id, parentId);
            });

            return result;
        }

        /// <summary>
        /// 批量重新生成排序码
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int BatchSetSortCode(BaseUserInfo userInfo, string tableName, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, tableName);
                result = itemDetailsManager.BatchSetSortCode(ids);
            });
            return result;
        }
    }
}