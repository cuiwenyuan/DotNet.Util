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
    /// ItemDetailsService
    /// 基础主键服务
    /// 
    /// 修改记录
    /// 
    ///		2013.06.05 张祈璟重构
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


    public class ItemDetailsService : IBaseItemDetailsService
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
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, tableName);
                // 若是系统管理员，那就返回全部数据
                if (BaseUserManager.IsAdministrator(userInfo.Id))
                {
                    dt = itemDetailsManager.GetDataTable(
                    new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldDeleted, 0)
                    , BaseItemDetailsEntity.FieldSortCode);
                }
                else
                {
                    // 按数据权限来过滤数据
                    var permissionScopeManager = new BasePermissionScopeManager(dbHelper, userInfo);
                    var ids = permissionScopeManager.GetResourceScopeIds(userInfo.SystemCode, userInfo.Id, tableName, "Resource.ManagePermission");
                    dt = itemDetailsManager.GetDataTable(ids);
                }
                dt.TableName = tableName;
                // 管理时需要把所有的数据显示出来，所以无效的数据也需要显示的
                // , new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldEnabled, 1)
            });
            return dt;
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
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
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
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, tableName);
                dt = itemDetailsManager.GetDataTableByParent(parentId);
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
            // 2013-09-07 吉日嘎拉 目标表，这样来个默认的表名，有助于提高稳定性，可以有一定的容错功能
            var targetTable = "Items" + code;
            var dt = new DataTable(BaseItemDetailsEntity.TableName);
            // 检查有其他目标数据库表存储了数据
            var itemsManager = new BaseItemsManager(dbHelper, userInfo);
            var itemsEntity = BaseEntity.Create<BaseItemsEntity>(itemsManager.GetDataTable(new KeyValuePair<string, object>(BaseItemsEntity.FieldCode, code)));
            if (itemsEntity != null && !string.IsNullOrEmpty(itemsEntity.TargetTable))
            {
                targetTable = itemsEntity.TargetTable;
            }

            var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo);
            itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, targetTable);
            // 这里只要有效的，没被删除的
            var parameters = new List<KeyValuePair<string, object>>
            {
                // 管理的时候无效的也需要被管理
                new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldDeleted, 0)
            };

            // 管理员取得所有数据，没采用数据权限的开关
            if (BaseUserManager.IsAdministrator(userInfo.Id) || !BaseSystemInfo.UsePermissionScope)
            {
                dt = itemDetailsManager.GetDataTable(parameters, BaseItemDetailsEntity.FieldSortCode);
            }
            else
            {
                // 按数据权限来过滤数据
                var permissionScopeManager = new BasePermissionScopeManager(dbHelper, userInfo);
                var ids = permissionScopeManager.GetResourceScopeIds(userInfo.SystemCode, userInfo.Id, itemsEntity.TargetTable, "Resource.ManagePermission");
                dt = itemDetailsManager.GetDataTable(ids);
                // 这里其实未必限制了有效的
                // BaseUtil.SetFilter(result, BaseItemDetailsEntity.FieldDeleted, "0");
                // BaseUtil.SetFilter(result, BaseItemDetailsEntity.FieldEnabled, "1");
            }
            dt.TableName = itemsEntity.TargetTable;
            return dt;
        }

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
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
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

        /// <summary>
        /// GetListByCode
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="userInfo"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<BaseItemDetailsEntity> GetListByCode(IDbHelper dbHelper, BaseUserInfo userInfo, string code)
        {
            // 2013-09-07 吉日嘎拉 目标表，这样来个默认的表名，有助于提高稳定性，可以有一定的容错功能
            var targetTable = "Items" + code;

            // 检查有其他目标数据库表存储了数据
            var itemsManager = new BaseItemsManager(dbHelper, userInfo);
            var itemsEntity = BaseEntity.Create<BaseItemsEntity>(itemsManager.GetDataTable(new KeyValuePair<string, object>(BaseItemsEntity.FieldCode, code)));

            if (itemsEntity != null && !string.IsNullOrEmpty(itemsEntity.TargetTable))
            {
                targetTable = itemsEntity.TargetTable;
            }

            var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo);
            itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, targetTable);
            // 这里只要有效的，没被删除的
            var parameters = new List<KeyValuePair<string, object>>
            {
                // 管理的时候无效的也需要被管理
                new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldDeleted, 0)
            };

            return itemDetailsManager.GetList<BaseItemDetailsEntity>(parameters, BaseItemDetailsEntity.FieldSortCode);
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
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                entityList = GetListByCode(dbHelper, userInfo, code);
            });

            return entityList;
        }

        /// <summary>
        /// GetListByTargetTable
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="targetTableName"></param>
        /// <returns></returns>
        public List<BaseItemDetailsEntity> GetListByTargetTable(BaseUserInfo userInfo, string targetTableName)
        {
            var entityList = new List<BaseItemDetailsEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                entityList = GetListByTargetTable(dbHelper, userInfo, targetTableName);
            });

            return entityList;
        }

        /// <summary>
        /// GetListByTargetTable
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="userInfo"></param>
        /// <param name="targetTableName"></param>
        /// <returns></returns>
        public List<BaseItemDetailsEntity> GetListByTargetTable(IDbHelper dbHelper, BaseUserInfo userInfo, string targetTableName)
        {
            List<BaseItemDetailsEntity> result = null;

            var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo);
            // 检查有其他目标数据库表存储了数据
            var itemsManager = new BaseItemsManager(dbHelper, userInfo);
            var itemsEntity = BaseEntity.Create<BaseItemsEntity>(itemsManager.GetDataTable(new KeyValuePair<string, object>(BaseItemsEntity.FieldTargetTable, targetTableName)));
            if (!string.IsNullOrEmpty(itemsEntity.TargetTable))
            {
                itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, itemsEntity.TargetTable);
            }
            // 这里只要有效的，没被删除的
            var parameters = new List<KeyValuePair<string, object>>
            {
                // 管理的时候无效的也需要被管理
                new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldDeleted, 0)
            };
            result = itemDetailsManager.GetList<BaseItemDetailsEntity>(parameters, BaseItemDetailsEntity.FieldSortCode);

            return result;
        }


        /// <summary>
        /// 按编号获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="code">编号</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByCode(BaseUserInfo userInfo, string code)
        {
            var result = new DataTable(BaseItemDetailsEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                result = GetDataTableByCode(dbHelper, userInfo, code);
            });

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
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
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
            BaseItemDetailsEntity entity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
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
            var entity = new BaseItemDetailsEntity();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, tableName);
                var dt = itemDetailsManager.GetDataTable(new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldItemCode, code), BaseItemDetailsEntity.FieldSortCode);
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    entity = (BaseItemDetailsEntity)entity.GetFrom(dt.Rows[0]);
                }
            });
            return entity;
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
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
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
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
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
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
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
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
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
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
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
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, tableName);
                result = itemDetailsManager.SetDeleted(ids);
            });

            return result;
        }

        #region public int BatchSave(BaseUserInfo userInfo, DataTable result) 批量保存
        /// <summary>
        /// 批量保存
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
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, dt.TableName);
                result = itemDetailsManager.BatchSave(dt);
            });

            return result;
        }
        #endregion

        #region public int Save(BaseUserInfo userInfo, DataTable result) 批量保存
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
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
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
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
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
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(dbHelper, userInfo, tableName);
                result = itemDetailsManager.BatchSetSortCode(ids);
            });

            return result;
        }
    }
}