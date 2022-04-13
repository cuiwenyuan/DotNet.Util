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
    /// BaseModuleService
    /// 模块服务
    /// 
    /// 修改记录
    /// 
    ///		2015.07.14 版本：2.0 JiRiGaLa 不允许修改的、不让修改，不允许删除的，不让删除、并且记录修改人。
    ///		2013.06.06 版本：1.2 张祈璟   重构。
    ///		2008.04.03 版本：1.1 JiRiGaLa 整理程序主键。
    ///		2007.05.11 版本：1.0 JiRiGaLa 窗体与数据库连接的分离。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.07.14</date>
    /// </author> 
    /// </summary>


    public class BaseModuleService : IBaseModuleService
    {
        #region public bool Exists(BaseUserInfo userInfo, List<KeyValuePair<string, object>> parameters, string id)
        /// <summary>
        /// 判断字段是否重复
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parameters">字段名,字段值</param>
        /// <param name="id">主键</param>
        /// <returns>已存在</returns>
        public bool Exists(BaseUserInfo userInfo, List<KeyValuePair<string, object>> parameters, string id)
        {
            var result = false;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseManager(dbHelper, userInfo, BaseModuleEntity.CurrentTableName);
                result = manager.Exists(parameters, id);
            });

            return result;
        }
        #endregion

        /// <summary>
        /// 获取模块列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(BaseUserInfo userInfo)
        {
            var result = new DataTable(BaseModuleEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Module";
                var manager = new BaseModuleManager(dbHelper, userInfo, tableName);
                result = manager.GetDataTable(
                    new KeyValuePair<string, object>(BaseModuleEntity.FieldDeleted, 0)
                    //, new KeyValuePair<string, object>(BaseModuleEntity.FieldEnabled, 1)
                    , BaseModuleEntity.FieldSortCode);
                result.DefaultView.Sort = BaseModuleEntity.FieldSortCode;
                result.TableName = BaseModuleEntity.CurrentTableName;
            });

            return result;
        }

        #region public List<BaseModuleEntity> GetList(BaseUserInfo userInfo) 获取角色列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>列表</returns>
        public List<BaseModuleEntity> GetList(BaseUserInfo userInfo)
        {
            var result = new List<BaseModuleEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Module";
                // 获得列表
                var manager = new BaseModuleManager(dbHelper, userInfo, tableName);
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseModuleEntity.FieldDeleted, 0)
                };
                result = manager.GetList<BaseModuleEntity>(parameters, BaseModuleEntity.FieldSortCode);
            });

            return result;
        }
        #endregion

        /// <summary>
        /// 按主键数组获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">角色主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByIds(BaseUserInfo userInfo, string[] ids)
        {
            var result = new DataTable(BaseRoleEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Module";
                var manager = new BaseModuleManager(dbHelper, userInfo, tableName);
                result = manager.GetDataTable(BaseModuleEntity.FieldId, ids, BaseModuleEntity.FieldSortCode);
                result.TableName = BaseModuleEntity.CurrentTableName;
            });

            return result;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public BaseModuleEntity GetEntity(BaseUserInfo userInfo, string id)
        {
            BaseModuleEntity result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Module";
                var manager = new BaseModuleManager(dbHelper, userInfo, tableName);
                result = manager.GetEntity(id);
            });

            return result;
        }

        /// <summary>
        /// 按编号获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="code">编号</param>
        /// <returns>实体</returns>
        public BaseModuleEntity GetEntityByCode(BaseUserInfo userInfo, string code)
        {
            BaseModuleEntity result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Module";
                var manager = new BaseModuleManager(dbHelper, userInfo, tableName);
                result = BaseEntity.Create<BaseModuleEntity>(manager.GetDataTableByCode(code));
            });

            return result;
        }


        /// <summary>
        /// 按窗体名称获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="formName">窗体名称</param>
        /// <returns>实体</returns>
        public BaseModuleEntity GetEntityByFormName(BaseUserInfo userInfo, string formName)
        {
            BaseModuleEntity result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Module";
                var manager = new BaseModuleManager(dbHelper, userInfo, tableName);
                result = BaseEntity.Create<BaseModuleEntity>(manager.GetDataTableByFormName(formName));
            });

            return result;
        }

        /// <summary>
        /// 通过编号获取模块名称
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="code">编号</param>
        /// <returns>数据表</returns>
        public string GetNameByCode(BaseUserInfo userInfo, string code)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Module";
                var manager = new BaseModuleManager(dbHelper, userInfo, tableName);
                result = manager.GetNameByCode(code);
            });

            return result;
        }

        /// <summary>
        /// 添加模块菜单
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        /// <param name="statusMessage">返回状消息</param>
        /// <returns>主键</returns>
        public string Add(BaseUserInfo userInfo, BaseModuleEntity entity, out Status status, out string statusMessage)
        {
            var result = string.Empty;

            Status returnCode = Status.Ok;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Module";
                var manager = new BaseModuleManager(dbHelper, userInfo, tableName);
                // 调用方法，并且返回运行结果
                result = manager.UniqueAdd(entity, out returnCode);
                // 获得状态消息
                returnMessage = manager.GetStateMessage(returnCode);
            });
            status = returnCode;
            statusMessage = returnMessage;

            return result;
        }

        /// <summary>
        /// 更新模块菜单
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        /// <param name="statusMessage">返回状消息</param>
        /// <returns>影响行数</returns>
        public int Update(BaseUserInfo userInfo, BaseModuleEntity entity, out Status status, out string statusMessage)
        {
            var result = 0;

            Status returnCode = Status.Ok;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Module";
                var manager = new BaseModuleManager(dbHelper, userInfo, tableName);
                // 调用方法，并且返回运行结果
                result = manager.UniqueUpdate(entity, out returnCode);
                // 获得状态消息
                returnMessage = manager.GetStateMessage(returnCode);
            });
            status = returnCode;
            statusMessage = returnMessage;

            return result;
        }

        /// <summary>
        /// 按父节点获得列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">父结点主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByParent(BaseUserInfo userInfo, string parentId)
        {
            var result = new DataTable(BaseModuleEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Module";
                var manager = new BaseModuleManager(dbHelper, userInfo, tableName);
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseModuleEntity.FieldDeleted, 0),
                    new KeyValuePair<string, object>(BaseModuleEntity.FieldParentId, parentId)
                };
                result = manager.GetDataTable(parameters, 0, BaseModuleEntity.FieldSortCode);
                // result.DefaultView.Sort = BaseModuleEntity.FieldSortCode;
                result.TableName = BaseModuleEntity.CurrentTableName;
            });

            return result;
        }

        /// <summary>
        /// 获取范围权限项列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>列表</returns>
        public List<BaseModuleEntity> GetScopePermissionList(BaseUserInfo userInfo)
        {
            var result = new List<BaseModuleEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Module";
                var manager = new BaseModuleManager(dbHelper, userInfo, tableName);

                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseModuleEntity.FieldDeleted, 0),
                    new KeyValuePair<string, object>(BaseModuleEntity.FieldIsScope, 1),
                    new KeyValuePair<string, object>(BaseModuleEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseModuleEntity.FieldIsVisible, 1)
                };

                result = manager.GetList<BaseModuleEntity>(parameters, BaseModuleEntity.FieldSortCode);
            });

            return result;
        }

        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(BaseUserInfo userInfo, string id)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Module";
                var manager = new BaseModuleManager(dbHelper, userInfo, tableName);
                result = manager.Delete(id);
            });

            return result;
        }

        /// <summary>
        /// 批量删除模块
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>数据表</returns>
        public int BatchDelete(BaseUserInfo userInfo, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Module";
                var manager = new BaseModuleManager(dbHelper, userInfo, tableName);
                result = manager.Delete(ids);
            });

            return result;
        }

        #region public int SetDeleted(BaseUserInfo userInfo, string[] ids) 批量设置删除
        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int SetDeleted(BaseUserInfo userInfo, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Module";
                var manager = new BaseModuleManager(dbHelper, userInfo, tableName);
                foreach (var id in ids)
                {
                    var entityOld = manager.GetEntity(id);
                    // 2015-07-14 吉日嘎拉 只有允许删除的，才可以删除，不允许删除的，不让删除。
                    if (entityOld.AllowDelete == 1)
                    {
                        result += manager.SetDeleted(id, true, true);
                    }
                }
            });

            return result;
        }
        #endregion

        /// <summary>
        /// 移动模块菜单
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="moduleId">组织机构主键</param>
        /// <param name="parentId">父结点主键</param>
        /// <returns>数据表</returns>
        public int MoveTo(BaseUserInfo userInfo, string moduleId, string parentId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Module";
                var manager = new BaseModuleManager(dbHelper, userInfo, tableName);
                result = manager.SetProperty(moduleId, new KeyValuePair<string, object>(BaseModuleEntity.FieldParentId, parentId));
            });

            return result;
        }

        /// <summary>
        /// 批量移动数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="moduleIds">组织机构主键数组</param>
        /// <param name="parentId">父结点主键</param>
        /// <returns>数据表</returns>
        public int BatchMoveTo(BaseUserInfo userInfo, string[] moduleIds, string parentId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Module";
                var manager = new BaseModuleManager(dbHelper, userInfo, tableName);
                for (var i = 0; i < moduleIds.Length; i++)
                {
                    result += manager.SetProperty(moduleIds[i], new KeyValuePair<string, object>(BaseModuleEntity.FieldParentId, parentId));
                }
            });

            return result;
        }

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
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Module";
                var manager = new BaseModuleManager(dbHelper, userInfo, tableName);
                result = manager.BatchSave(dt);
            });

            return result;
        }

        /// <summary>
        /// 保存排序顺序
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int SetSortCode(BaseUserInfo userInfo, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                if (BaseUserManager.IsAdministrator(userInfo.Id.ToString()))
                {
                    var tableName = userInfo.SystemCode + "Module";
                    var manager = new BaseModuleManager(dbHelper, userInfo, tableName);
                    result = manager.BatchSetSortCode(ids);
                }
            });

            return result;
        }

        /// <summary>
        /// 获取菜单的用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="moduleId">模块主键</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="userId">用户主键</param>
        /// <returns>列表</returns>
        public DataTable GetModuleUserDataTable(BaseUserInfo userInfo, string systemCode, string moduleId, string companyId, string userId)
        {
            var result = new DataTable(BaseRoleEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var moduleManager = new BaseModuleManager(userInfo);
                result = moduleManager.GetModuleUserDataTable(systemCode, moduleId, companyId, userId);
                result.TableName = BaseUserEntity.CurrentTableName;
            });

            return result;
        }

        #region public DataTable GetModuleRoleDataTable(BaseUserInfo userInfo, string systemCode, string moduleId)
        /// <summary>
        /// 获取菜单的所有角色列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="moduleId">模块主键</param>
        /// <returns>列表</returns>
        public DataTable GetModuleRoleDataTable(BaseUserInfo userInfo, string systemCode, string moduleId)
        {
            var result = new DataTable(BaseRoleEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var moduleManager = new BaseModuleManager(userInfo);
                result = moduleManager.GetModuleRoleDataTable(systemCode, moduleId);
                result.TableName = BaseRoleEntity.CurrentTableName;
            });

            return result;
        }
        #endregion

        #region public DataTable GetModuleOrganizationDataTable(BaseUserInfo userInfo, string systemCode, string moduleId)
        /// <summary>
        /// 获取菜单的所有组织机构列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="moduleId">模块主键</param>
        /// <returns>列表</returns>
        public DataTable GetModuleOrganizationDataTable(BaseUserInfo userInfo, string systemCode, string moduleId)
        {
            var result = new DataTable(BaseOrganizationEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var moduleManager = new BaseModuleManager(userInfo);
                result = moduleManager.GetModuleOrganizationDataTable(systemCode, moduleId);
                result.TableName = BaseOrganizationEntity.CurrentTableName;
            });

            return result;
        }
        #endregion

        /// <summary>
        /// 刷新列表
        /// 2015-12-11 吉日嘎拉 刷新缓存功能优化
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public void CachePreheating(BaseUserInfo userInfo)
        {
            new BaseModuleManager().CachePreheating();
        }
    }
}
