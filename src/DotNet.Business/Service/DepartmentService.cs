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
    /// DepartmentService
    /// 部门服务
    /// 
    /// 修改记录
    /// 
    ///		2014.12.08 版本：1.0 JiRiGaLa 窗体与数据库连接的分离。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2014.12.08</date>
    /// </author> 
    /// </summary>


    public partial class DepartmentService : IDepartmentService
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
                var manager = new BaseManager(dbHelper, userInfo, BaseDepartmentEntity.TableName);
                result = manager.Exists(parameters, id);
            });
            return result;
        }
        #endregion

        #region public string Add(BaseUserInfo userInfo, BaseDepartmentEntity entity, out string statusCode, out string statusMessage)
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>主键</returns>
        public string Add(BaseUserInfo userInfo, BaseDepartmentEntity entity, out string statusCode, out string statusMessage)
        {
            var result = string.Empty;

            var returnCode = string.Empty;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseDepartmentManager(dbHelper, userInfo);
                result = manager.Add(entity, out returnCode);
                returnMessage = manager.GetStateMessage(returnCode);
            });
            statusCode = returnCode;
            statusMessage = returnMessage;
            return result;
        }
        #endregion

        #region public BaseDepartmentEntity GetEntity(BaseUserInfo userInfo, string id)
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public BaseDepartmentEntity GetEntity(BaseUserInfo userInfo, string id)
        {
            BaseDepartmentEntity result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseDepartmentManager(dbHelper, userInfo);
                result = manager.GetEntity(id);

            });

            return result;
        }
        #endregion

        #region public DataTable GetDataTable(BaseUserInfo userInfo, string companyId)
        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="companyId">公司主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(BaseUserInfo userInfo, string companyId)
        {
            var result = new DataTable(BaseDepartmentEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得组织机构列表
                var commandText = "   SELECT * "
                            + " FROM " + BaseDepartmentEntity.TableName
                            + "    WHERE " + BaseDepartmentEntity.FieldDeleted + " = 0 "
                            + "         AND " + BaseDepartmentEntity.FieldEnabled + " = 1 "
                            + "         AND " + BaseDepartmentEntity.FieldCategoryCode + " = '2' "
                            + "         AND " + BaseDepartmentEntity.FieldCompanyId + " = '" + companyId + "'";
                // + "   START WITH " + BaseDepartmentEntity.FieldCompanyId + " = '" + companyId + "'"
                // + " CONNECT BY PRIOR " + BaseDepartmentEntity.FieldId + " = " + BaseDepartmentEntity.FieldParentId
                // + "   ORDER BY " + BaseDepartmentEntity.FieldSortCode;

                result = dbHelper.Fill(commandText);
                result.TableName = BaseDepartmentEntity.TableName;
            });

            return result;
        }
        #endregion

        #region public DataTable GetDataTable(BaseUserInfo userInfo, List<KeyValuePair<string, object>> parameters)
        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parameters">参数</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(BaseUserInfo userInfo, List<KeyValuePair<string, object>> parameters)
        {
            var dt = new DataTable(BaseDepartmentEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得组织机构列表
                var manager = new BaseDepartmentManager(dbHelper, userInfo);
                dt = manager.GetDataTable(parameters, BaseDepartmentEntity.FieldSortCode);
                dt.DefaultView.Sort = BaseDepartmentEntity.FieldSortCode;
                dt.TableName = BaseDepartmentEntity.TableName;
            });
            return dt;
        }
        #endregion

        #region public DataTable GetDataTableByParent(BaseUserInfo userInfo, string parentId)
        /// <summary>
        /// 按父节点获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">父节点</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByParent(BaseUserInfo userInfo, string parentId)
        {
            var dt = new DataTable(BaseDepartmentEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 这里可以缓存起来，提高效率
                var manager = new BaseDepartmentManager(dbHelper, userInfo);
                // 这里是条件字段
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseDepartmentEntity.FieldParentId, parentId),
                    new KeyValuePair<string, object>(BaseDepartmentEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseDepartmentEntity.FieldDeleted, 0)
                };
                // 获取列表，指定排序字段
                dt = manager.GetDataTable(parameters, BaseDepartmentEntity.FieldSortCode);
                dt.DefaultView.Sort = BaseDepartmentEntity.FieldSortCode;
                dt.TableName = BaseDepartmentEntity.TableName;
            });

            return dt;
        }
        #endregion

        #region public DataTable GetDataTableByIds(BaseUserInfo userInfo, string[] ids)
        /// <summary>
        /// 按主键数组获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">组织机构主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByIds(BaseUserInfo userInfo, string[] ids)
        {
            var dt = new DataTable(BaseDepartmentEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseDepartmentManager(dbHelper, userInfo);
                dt = manager.GetDataTable(BaseDepartmentEntity.FieldId, ids, BaseDepartmentEntity.FieldSortCode);
                dt.TableName = BaseDepartmentEntity.TableName;
            });
            return dt;
        }
        #endregion

        #region public List<BaseDepartmentEntity> GetListByIds(BaseUserInfo userInfo, string[] ids) 获取用户列表
        /// <summary>
        /// 按主键获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>数据表</returns>
        public List<BaseDepartmentEntity> GetListByIds(BaseUserInfo userInfo, string[] ids)
        {
            var result = new List<BaseDepartmentEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseDepartmentManager(dbHelper, userInfo);
                result = userManager.GetList<BaseDepartmentEntity>(BaseDepartmentEntity.FieldId, ids, BaseDepartmentEntity.FieldSortCode);
            });

            return result;
        }
        #endregion

        #region public DataTable Search(BaseUserInfo userInfo, string organizationId, string searchKey)
        /// <summary>
        /// 查询组织机构
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">组织机构</param>
        /// <param name="searchKey">查询</param>
        /// <returns>数据表</returns>
        public DataTable Search(BaseUserInfo userInfo, string organizationId, string searchKey)
        {
            var dt = new DataTable(BaseDepartmentEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得组织机构列表
                var manager = new BaseDepartmentManager(dbHelper, userInfo);
                // dt = manager.Search(string.Empty, searchKey);
                dt.DefaultView.Sort = BaseDepartmentEntity.FieldSortCode;
                dt.TableName = BaseDepartmentEntity.TableName;
            });
            return dt;
        }
        #endregion

        #region public int Update(BaseUserInfo userInfo, BaseDepartmentEntity entity, out string statusCode, out string statusMessage)
        /// <summary>
        /// 更新组织机构
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>影响行数</returns>
        public int Update(BaseUserInfo userInfo, BaseDepartmentEntity entity, out string statusCode, out string statusMessage)
        {
            var result = 0;

            var returnCode = string.Empty;
            var returnMessage = string.Empty;
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseDepartmentManager(dbHelper, userInfo);
                result = manager.Update(entity, out returnCode);
                returnMessage = manager.GetStateMessage(returnCode);
            });
            statusCode = returnCode;
            statusMessage = returnMessage;
            return result;
        }
        #endregion

        #region public int Delete(BaseUserInfo userInfo, string id)
        /// <summary>
        /// 删除数据
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
                var manager = new BaseDepartmentManager(dbHelper, userInfo);
                result = manager.Delete(id);
            });
            return result;
        }
        #endregion

        #region public int BatchDelete(BaseUserInfo userInfo, string[] ids)
        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int BatchDelete(BaseUserInfo userInfo, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseDepartmentManager(dbHelper, userInfo);
                result = manager.Delete(ids);
            });
            return result;
        }
        #endregion

        #region public int SetDeleted(BaseUserInfo userInfo, string[] ids)
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
                var manager = new BaseDepartmentManager(dbHelper, userInfo);
                for (var i = 0; i < ids.Length; i++)
                {
                    // 设置部门为删除状态
                    result += manager.SetDeleted(ids[i]);
                    // 相应的用户也需要处理
                    var userManager = new BaseUserManager(dbHelper, userInfo);
                    var parameters = new List<KeyValuePair<string, object>>();
                    parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BaseUserEntity.FieldDepartmentId, null),
                        new KeyValuePair<string, object>(BaseUserEntity.FieldDepartmentName, null)
                    };
                    userManager.SetProperty(new KeyValuePair<string, object>(BaseUserEntity.FieldDepartmentId, ids[i]), parameters);
                }
            });
            return result;
        }
        #endregion

        #region public int BatchSave(BaseUserInfo userInfo, DataTable result)
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
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseDepartmentManager(dbHelper, userInfo);
                result = manager.BatchSave(dt);
            });
            return result;
        }
        #endregion

        #region public int MoveTo(BaseUserInfo userInfo, string id, string parentId)
        /// <summary>
        /// 移动数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <param name="parentId">父主键</param>
        /// <returns>影响行数</returns>
        public int MoveTo(BaseUserInfo userInfo, string id, string parentId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseDepartmentManager(dbHelper, userInfo);
                result = manager.MoveTo(id, parentId);
            });
            return result;
        }
        #endregion

        #region public int BatchMoveTo(BaseUserInfo userInfo, string[] ids, string parentId)
        /// <summary>
        /// 批量移动数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationIds">主键数组</param>
        /// <param name="parentId">父节点主键</param>
        /// <returns>影响行数</returns>
        public int BatchMoveTo(BaseUserInfo userInfo, string[] organizationIds, string parentId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseDepartmentManager(dbHelper, userInfo);
                for (var i = 0; i < organizationIds.Length; i++)
                {
                    result += manager.MoveTo(organizationIds[i], parentId);
                }
            });
            return result;
        }
        #endregion
    }
}