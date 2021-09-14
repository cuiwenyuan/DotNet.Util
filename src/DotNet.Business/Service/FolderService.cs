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
    /// FolderService
    /// 文件夹服务
    /// 
    /// 修改记录
    /// 
    ///		2013.06.05 张祈璟重构
    ///		2008.05.04 版本：1.1 JiRiGaLa 添加方法。
    ///		2008.04.30 版本：1.0 JiRiGaLa 创建。
    ///	
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2008.05.04</date>
    /// </author> 
    /// </summary>


    public class FolderService : IFolderService
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(BaseUserInfo userInfo)
        {
            var dt = new DataTable(BaseModuleEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var folderManager = new BaseFolderManager(dbHelper, userInfo);
                // 检查相应的系统必备文件夹
                folderManager.FolderCheck();
                if (BaseUserManager.IsAdministrator(userInfo.Id))
                {
                    dt = folderManager.GetDataTable(new KeyValuePair<string, object>(BaseFolderEntity.FieldDeleted, 0), BaseFolderEntity.FieldSortCode);
                }
                else
                {
                    // 数据权限部分，部门的权限部分。
                    var permissionScopeManager = new BasePermissionScopeManager(dbHelper, userInfo);
                    var ids = permissionScopeManager.GetOrganizeIds(userInfo.Id, "File.Admin");
                    // 获取安全等级，比自己小的。
                    var commandText = string.Format(@"SELECT * 
                               FROM BaseFolder 
                                                              WHERE (" + BaseFolderEntity.FieldDeleted + @" = 0 
                                                                    AND Enabled = 1 
                                                                    AND (IsPublic = 1 
                                                                         OR Id = 'UserSpace' 
                                                                         OR Id = 'CompanyFile' 
                                                                         OR Id = '{0}' 
                                                                         OR Id = '{1}' 
                                                                         OR Id = '{2}' 
                                                                         OR CreateUserId = '{3}')) ", userInfo.Id, userInfo.DepartmentId, userInfo.CompanyId, userInfo.Id);
                    if (ids != null && ids.Length > 0)
                    {
                        commandText += " OR ID IN (" + string.Join(",", ids) + ") ";
                    }
                    dt = folderManager.Fill(commandText);
                }
                dt.DefaultView.Sort = BaseFolderEntity.FieldSortCode;
                dt.TableName = BaseFolderEntity.TableName;
            });
            return dt;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public BaseFolderEntity GetObject(BaseUserInfo userInfo, string id)
        {
            BaseFolderEntity folderEntity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var folderManager = new BaseFolderManager(dbHelper, userInfo);
                folderEntity = folderManager.GetObject(id);
            });
            return folderEntity;
        }
        /// <summary>
        /// GetDataTableByParent
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetDataTableByParent(BaseUserInfo userInfo, string id)
        {
            var dt = new DataTable(BaseOrganizeEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得文件夹列表
                var folderManager = new BaseFolderManager(dbHelper, userInfo);
                dt = folderManager.GetDataTableByParent(id);
                dt.TableName = BaseFolderEntity.TableName;
            });

            return dt;
        }
        /// <summary>
        /// 根据文件夹名新增
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="parentId"></param>
        /// <param name="folderName"></param>
        /// <param name="enabled"></param>
        /// <param name="statusCode"></param>
        /// <param name="statusMessage"></param>
        /// <returns></returns>
        public string AddByFolderName(BaseUserInfo userInfo, string parentId, string folderName, bool enabled, out string statusCode, out string statusMessage)
        {
            var result = string.Empty;

            var returnCode = string.Empty;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var folderEntity = new BaseFolderEntity();
                var folderManager = new BaseFolderManager(dbHelper, userInfo);
                folderEntity.ParentId = parentId;
                folderEntity.FolderName = folderName;
                folderEntity.Enabled = enabled ? 1 : 0;
                result = folderManager.Add(folderEntity, out returnCode);
                returnMessage = folderManager.GetStateMessage(returnCode);
            });
            statusCode = returnCode;
            statusMessage = returnMessage;
            return result;
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="entity"></param>
        /// <param name="statusCode"></param>
        /// <param name="statusMessage"></param>
        /// <returns></returns>
        public string Add(BaseUserInfo userInfo, BaseFolderEntity entity, out string statusCode, out string statusMessage)
        {
            var result = string.Empty;

            var returnCode = string.Empty;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var folderManager = new BaseFolderManager(dbHelper, userInfo);
                result = folderManager.Add(entity, out returnCode);
                returnMessage = folderManager.GetStateMessage(returnCode);
            });
            statusCode = returnCode;
            statusMessage = returnMessage;
            return result;
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="entity"></param>
        /// <param name="statusCode"></param>
        /// <param name="statusMessage"></param>
        /// <returns></returns>
        public int Update(BaseUserInfo userInfo, BaseFolderEntity entity, out string statusCode, out string statusMessage)
        {
            var result = 0;

            var returnCode = string.Empty;
            var returnMessage = string.Empty;
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var folderManager = new BaseFolderManager(dbHelper, userInfo);
                result = folderManager.Update(entity, out returnCode);
                returnMessage = folderManager.GetStateMessage(returnCode);
            });
            statusCode = returnCode;
            statusMessage = returnMessage;
            return result;
        }

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <param name="newName">新名称</param>
        /// <param name="enabled">有效</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>影响行数</returns>
        public int Rename(BaseUserInfo userInfo, string id, string newName, bool enabled, out string statusCode, out string statusMessage)
        {
            var result = 0;

            var returnCode = string.Empty;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var folderEntity = new BaseFolderEntity();
                var folderManager = new BaseFolderManager(dbHelper, userInfo);
                var dt = folderManager.GetDataTableById(id);
                folderEntity.GetSingle(dt);
                folderEntity.FolderName = newName;
                folderEntity.Enabled = enabled ? 1 : 0;
                result = folderManager.Update(folderEntity, out returnCode);
                returnMessage = folderManager.GetStateMessage(returnCode);
            });
            statusCode = returnCode;
            statusMessage = returnMessage;
            return result;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="searchKey">查询</param>
        /// <returns>数据表</returns>
        public DataTable Search(BaseUserInfo userInfo, string searchKey)
        {
            var dt = new DataTable(BaseOrganizeEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var folderManager = new BaseFolderManager(dbHelper, userInfo);
                dt = folderManager.Search(searchKey);
                dt.TableName = BaseFolderEntity.TableName;
            });
            return dt;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(BaseUserInfo userInfo, string id)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var folderManager = new BaseFolderManager(dbHelper, userInfo);
                result = folderManager.Delete(id);
            });
            return result;
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int BatchDelete(BaseUserInfo userInfo, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var folderManager = new BaseFolderManager(dbHelper, userInfo);
                result = folderManager.Delete(ids);
            });
            return result;
        }
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="folderId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public int MoveTo(BaseUserInfo userInfo, string folderId, string parentId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var folderManager = new BaseFolderManager(dbHelper, userInfo);
                result = folderManager.MoveTo(folderId, parentId);
            });
            return result;
        }
        /// <summary>
        /// 批量移动
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="folderIds"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public int BatchMoveTo(BaseUserInfo userInfo, string[] folderIds, string parentId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var folderManager = new BaseFolderManager(dbHelper, userInfo);
                for (var i = 0; i < folderIds.Length; i++)
                {
                    result += folderManager.MoveTo(folderIds[i], parentId);
                }
            });
            return result;
        }
        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int BatchSave(BaseUserInfo userInfo, DataTable dt)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var folderManager = new BaseFolderManager(dbHelper, userInfo);
                result = folderManager.BatchSave(dt);
            });
            return result;
        }
        /// <summary>
        /// 获取编号
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <returns></returns>
        public string GetId(BaseUserInfo userInfo, KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterDb(userInfo, parameter, (dbHelper) =>
            {
                var folderManager = new BaseFolderManager(dbHelper, userInfo);
                result = folderManager.GetId(parameter1, parameter2);
            });
            return result;
        }
    }
}