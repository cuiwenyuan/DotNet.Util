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
    /// OrganizationService
    /// 组织机构服务
    /// 
    /// 修改记录
    /// 
    ///		2014.01.30 版本：3.0 JiRiGaLa 增加判断重复的Exists代码。
    ///		2013.06.06 版本：3.0 张祈璟重构
    ///		2007.08.15 版本：2.2 JiRiGaLa 改进运行速度采用 WebService 变量定义 方式处理数据。
    ///		2007.08.14 版本：2.1 JiRiGaLa 改进运行速度采用 Instance 方式处理数据。
    ///     2007.06.11 版本：1.3 JiRiGaLa 加入调试信息#if (DEBUG)。
    ///     2007.06.04 版本：1.2 JiRiGaLa 加入WebService。
    ///     2007.05.29 版本：1.1 JiRiGaLa 排版，修改，完善。
    ///		2007.05.11 版本：1.0 JiRiGaLa 窗体与数据库连接的分离。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2007.08.15</date>
    /// </author> 
    /// </summary>


    public partial class OrganizationService : IOrganizationService
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
                var manager = new BaseManager(dbHelper, userInfo, BaseOrganizationEntity.TableName);
                result = manager.Exists(parameters, id);
            });
            return result;
        }
        #endregion

        #region public string Add(BaseUserInfo userInfo, BaseOrganizationEntity entity, out string statusCode, out string statusMessage)
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>主键</returns>
        public string Add(BaseUserInfo userInfo, BaseOrganizationEntity entity, out string statusCode, out string statusMessage)
        {
            var result = string.Empty;
            var returnCode = string.Empty;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                result = manager.Add(entity);
                //增加返回状态和信息Troy.Cui 2018-10-03
                returnCode = manager.StatusCode;
                returnMessage = manager.StatusMessage;
                //returnMessage = manager.GetStateMessage(returnCode);
                if (returnCode.Equals(Status.OkAdd.ToString()))
                {
                    entity.Id = result;
                    //Troy.Cui 2018-10-18去掉这里的Folder检查
                    //var folderManager = new BaseFolderManager(dbHelper, userInfo);
                    //folderManager.FolderCheck(entity.Id.ToString(), entity.FullName);
                }
            });
            statusCode = returnCode;
            statusMessage = returnMessage;
            return result;
        }
        #endregion

        #region public string AddByDetail(BaseUserInfo userInfo, string parentId, string code, string fullName, string categoryId, string outerPhone, string innerPhone, string fax, bool enabled, out string statusCode, out string statusMessage)
        /// <summary>
        /// 按详细情况添加实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">父主键</param>
        /// <param name="code">编号</param>
        /// <param name="fullName">全称</param>
        /// <param name="categoryId">分类</param>
        /// <param name="outerPhone">外线</param>
        /// <param name="innerPhone">内线</param>
        /// <param name="fax">传真</param>
        /// <param name="enabled">有效</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>主键</returns>
        public string AddByDetail(BaseUserInfo userInfo, string parentId, string code, string fullName, string categoryId, string outerPhone, string innerPhone, string fax, bool enabled, out string statusCode, out string statusMessage)
        {
            var result = string.Empty;

            var returnCode = string.Empty;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                result = manager.AddByDetail(parentId, code, fullName, categoryId, outerPhone, innerPhone, fax, enabled);
                returnMessage = manager.GetStateMessage(returnCode);
            });
            statusCode = returnCode;
            statusMessage = returnMessage;
            return result;
        }
        #endregion

        #region public BaseOrganizationEntity GetEntity(BaseUserInfo userInfo, string id)
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public BaseOrganizationEntity GetEntity(BaseUserInfo userInfo, string id)
        {
            BaseOrganizationEntity entity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                entity = manager.GetEntity(id);
            });
            return entity;
        }
        #endregion

        #region public BaseOrganizationEntity GetEntityByCode(BaseUserInfo userInfo, string code)
        /// <summary>
        /// 按编号获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="code">编号</param>
        /// <returns>实体</returns>
        public BaseOrganizationEntity GetEntityByCode(BaseUserInfo userInfo, string code)
        {
            BaseOrganizationEntity entity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                entity = manager.GetEntityByCode(code);
            });
            return entity;
        }
        #endregion

        #region public BaseOrganizationEntity GetEntityByName(BaseUserInfo userInfo, string fullName)
        /// <summary>
        /// 按名称获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="fullName">名称</param>
        /// <returns>实体</returns>
        public BaseOrganizationEntity GetEntityByName(BaseUserInfo userInfo, string fullName)
        {
            BaseOrganizationEntity entity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                entity = manager.GetEntityByName(fullName);
            });
            return entity;
        }
        #endregion

        #region public DataTable GetDataTable(BaseUserInfo userInfo)
        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(BaseUserInfo userInfo)
        {
            var dt = new DataTable(BaseOrganizationEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得组织机构列表
                var commandText = string.Empty;
                if (BaseSystemInfo.OrganizationDynamicLoading)
                {
                    commandText = "    SELECT * "
                                + " FROM " + BaseOrganizationEntity.TableName
                                + "     WHERE " + BaseOrganizationEntity.FieldDeleted + " = 0 "
                                + "           AND " + BaseOrganizationEntity.FieldIsInnerOrganization + " = 1 "
                                + "           AND " + BaseOrganizationEntity.FieldEnabled + " = 1 "
                                + "           AND (" + BaseOrganizationEntity.FieldParentId + " IS NULL "
                                + "                OR " + BaseOrganizationEntity.FieldParentId + " IN (SELECT " + BaseOrganizationEntity.FieldId + " FROM " + BaseOrganizationEntity.TableName + " WHERE " + BaseOrganizationEntity.FieldDeleted + " = 0 AND " + BaseOrganizationEntity.FieldIsInnerOrganization + " = 1 AND " + BaseOrganizationEntity.FieldEnabled + " = 1 AND " + BaseOrganizationEntity.FieldParentId + " IS NULL)) "
                                + "  ORDER BY " + BaseOrganizationEntity.FieldSortCode;
                }
                else
                {
                    commandText = "    SELECT * "
                                + " FROM " + BaseOrganizationEntity.TableName
                                + "     WHERE " + BaseOrganizationEntity.FieldDeleted + " = 0 "
                                + "           AND " + BaseOrganizationEntity.FieldIsInnerOrganization + " = 1 "
                                + "           AND " + BaseOrganizationEntity.FieldEnabled + " = 1 "
                                + "  ORDER BY " + BaseOrganizationEntity.FieldSortCode;
                }


                // var manager = new BaseOrganizationManager(dbHelper, result);
                // result = manager.GetDataTable(new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0), BaseOrganizationEntity.FieldSortCode);
                dt = dbHelper.Fill(commandText);
                dt.DefaultView.Sort = BaseOrganizationEntity.FieldSortCode;
                dt.TableName = BaseOrganizationEntity.TableName;
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
            var dt = new DataTable(BaseOrganizationEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                dt = manager.GetDataTable(BaseOrganizationEntity.FieldId, ids, BaseOrganizationEntity.FieldSortCode);
                dt.TableName = BaseOrganizationEntity.TableName;
            });
            return dt;
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
            var dt = new DataTable(BaseOrganizationEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得组织机构列表
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                dt = manager.GetDataTable(parameters, BaseOrganizationEntity.FieldSortCode);
                dt.DefaultView.Sort = BaseOrganizationEntity.FieldSortCode;
                dt.TableName = BaseOrganizationEntity.TableName;
            });
            return dt;
        }
        #endregion

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">父亲节点主键</param>
        /// <returns>数据表</returns>
        public DataTable GetErrorDataTable(BaseUserInfo userInfo, string parentId)
        {
            var dt = new DataTable(BaseOrganizationEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 这里可以缓存起来，提高效率
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                dt = manager.GetErrorDataTable(parentId);
                dt.DefaultView.Sort = BaseOrganizationEntity.FieldSortCode;
                dt.TableName = BaseOrganizationEntity.TableName;
            });
            return dt;
        }

        #region public DataTable GetDataTableByParent(BaseUserInfo userInfo, string parentId)
        /// <summary>
        /// 按父节点获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">父节点</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByParent(BaseUserInfo userInfo, string parentId)
        {
            var dt = new DataTable(BaseOrganizationEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 这里可以缓存起来，提高效率
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                // 这里是条件字段
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldParentId, parentId),
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0)
                };
                // 获取列表，指定排序字段
                dt = manager.GetDataTable(parameters, BaseOrganizationEntity.FieldSortCode);
                dt.DefaultView.Sort = BaseOrganizationEntity.FieldSortCode;
                dt.TableName = BaseOrganizationEntity.TableName;
            });
            return dt;
        }
        #endregion

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="provinceId">省主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByProvinceId(BaseUserInfo userInfo, string provinceId)
        {
            var result = new DataTable(BaseOrganizationEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 这里可以缓存起来，提高效率
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                // 这里是条件字段
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldProvinceId, provinceId),
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0)
                };
                // 获取列表，指定排序字段
                result = manager.GetDataTable(parameters, BaseOrganizationEntity.FieldSortCode);
                result.DefaultView.Sort = BaseOrganizationEntity.FieldSortCode;
                result.TableName = BaseOrganizationEntity.TableName;
            });

            return result;
        }

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="cityId">市主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByCityId(BaseUserInfo userInfo, string cityId)
        {
            var result = new DataTable(BaseOrganizationEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 这里可以缓存起来，提高效率
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                // 这里是条件字段
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldCityId, cityId),
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0)
                };
                // 获取列表，指定排序字段
                result = manager.GetDataTable(parameters, BaseOrganizationEntity.FieldSortCode);
                result.DefaultView.Sort = BaseOrganizationEntity.FieldSortCode;
                result.TableName = BaseOrganizationEntity.TableName;
            });

            return result;
        }

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="districtId">区县主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByDistrictId(BaseUserInfo userInfo, string districtId)
        {
            var result = new DataTable(BaseOrganizationEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 这里可以缓存起来，提高效率
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                // 这里是条件字段
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDistrictId, districtId),
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0)
                };
                // 获取列表，指定排序字段
                result = manager.GetDataTable(parameters, BaseOrganizationEntity.FieldSortCode);
                result.DefaultView.Sort = BaseOrganizationEntity.FieldSortCode;
                result.TableName = BaseOrganizationEntity.TableName;
            });

            return result;
        }

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="streetId">街道主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByStreetId(BaseUserInfo userInfo, string streetId)
        {
            var result = new DataTable(BaseOrganizationEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 这里可以缓存起来，提高效率
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                // 这里是条件字段
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldStreetId, streetId),
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0)
                };
                // 获取列表，指定排序字段
                result = manager.GetDataTable(parameters, BaseOrganizationEntity.FieldSortCode);
                result.DefaultView.Sort = BaseOrganizationEntity.FieldSortCode;
                result.TableName = BaseOrganizationEntity.TableName;
            });

            return result;
        }

        #region public DataTable GetInnerOrganizationDT(BaseUserInfo userInfo, string organizationId)
        /// <summary>
        /// 获取内部组织机构
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">组织机构</param>
        /// <returns>数据表</returns>
        public DataTable GetInnerOrganizationDT(BaseUserInfo userInfo, string organizationId)
        {
            var dt = new DataTable(BaseOrganizationEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得组织机构列表
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                dt = manager.GetInnerOrganization(organizationId);
                dt.DefaultView.Sort = BaseOrganizationEntity.FieldSortCode;
                dt.TableName = BaseOrganizationEntity.TableName;
            });
            return dt;
        }
        #endregion

        #region public DataTable GetCompanyDT(BaseUserInfo userInfo, string organizationId)
        /// <summary>
        /// 获取公司列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">组织机构主键</param>
        /// <returns>数据表</returns>
        public DataTable GetCompanyDT(BaseUserInfo userInfo, string organizationId)
        {
            var dt = new DataTable(BaseOrganizationEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得组织机构列表
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                dt = manager.GetCompanyDt(organizationId);
                dt.DefaultView.Sort = BaseOrganizationEntity.FieldSortCode;
                dt.TableName = BaseOrganizationEntity.TableName;
            });
            return dt;
        }
        #endregion

        #region public DataTable GetDepartmentDT(BaseUserInfo userInfo, string organizationId)
        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">组织机构</param>
        /// <returns>数据表</returns>
        public DataTable GetDepartmentDT(BaseUserInfo userInfo, string organizationId)
        {
            var dt = new DataTable(BaseOrganizationEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得组织机构列表
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                dt = manager.GetOrganizationDataTable(organizationId);
                dt.DefaultView.Sort = BaseOrganizationEntity.FieldSortCode;
                dt.TableName = BaseOrganizationEntity.TableName;
            });
            return dt;
        }
        #endregion

        #region public List<BaseOrganizationEntity> GetListByIds(BaseUserInfo userInfo, string[] ids) 获取用户列表
        /// <summary>
        /// 按主键获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>数据表</returns>
        public List<BaseOrganizationEntity> GetListByIds(BaseUserInfo userInfo, string[] ids)
        {
            var result = new List<BaseOrganizationEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseOrganizationManager(dbHelper, userInfo);
                result = userManager.GetList<BaseOrganizationEntity>(BaseOrganizationEntity.FieldId, ids, BaseOrganizationEntity.FieldSortCode);
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
            var dt = new DataTable(BaseOrganizationEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得组织机构列表
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                dt = manager.Search(searchKey, organizationId);
                dt.DefaultView.Sort = BaseOrganizationEntity.FieldSortCode;
                dt.TableName = BaseOrganizationEntity.TableName;
            });
            return dt;
        }
        #endregion

        #region public int Update(BaseUserInfo userInfo, BaseOrganizationEntity entity, out string statusCode, out string statusMessage)
        /// <summary>
        /// 更新组织机构
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>影响行数</returns>
        public int Update(BaseUserInfo userInfo, BaseOrganizationEntity entity, out string statusCode, out string statusMessage)
        {
            var result = 0;

            var returnCode = string.Empty;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                // 2015-12-19 吉日嘎拉 网络不稳定，数据获取不完整时，异常时，会引起重大隐患
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                // result = manager.Update(entity);
                if (manager.StatusCode.Equals(Status.OkUpdate.ToString()))
                {
                    // var folderManager = new BaseFolderManager(dbHelper, userInfo);
                    // result = folderManager.SetProperty(entity.Id.ToString(), new KeyValuePair<string, object>(BaseFolderEntity.FieldFolderName, entity.FullName));
                }
                returnCode = manager.StatusCode;
                returnMessage = manager.StatusMessage;
            });

            statusCode = returnCode;
            statusMessage = returnMessage;

            return result;
        }
        #endregion

        #region public int Synchronous(BaseUserInfo userInfo, bool all = false)
        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="all">同步所有数据</param>
        /// <returns>影响行数</returns>
        public int Synchronous(BaseUserInfo userInfo, bool all = false)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                // result = manager.Synchronous(all);
            });

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
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                result = manager.Delete(id);
                // 把公司文件夹也删除了
                var folderManager = new BaseFolderManager(dbHelper, userInfo);
                result = folderManager.Delete(id);
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
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                result = manager.Delete(ids);
                // 把公司文件夹也删除了
                var folderManager = new BaseFolderManager(dbHelper, userInfo);
                result = folderManager.Delete(ids);
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
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                for (var i = 0; i < ids.Length; i++)
                {
                    // 设置部门为删除状态
                    result += manager.SetDeleted(ids[i]);
                    // 相应的用户也需要处理
                    var userManager = new BaseUserManager(dbHelper, userInfo);
                    var parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BaseUserEntity.FieldCompanyId, null),
                        new KeyValuePair<string, object>(BaseUserEntity.FieldCompanyName, null)
                    };
                    userManager.SetProperty(new KeyValuePair<string, object>(BaseUserEntity.FieldCompanyId, ids[i]), parameters);
                    parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BaseUserEntity.FieldSubCompanyId, null),
                        new KeyValuePair<string, object>(BaseUserEntity.FieldSubCompanyName, null)
                    };
                    userManager.SetProperty(new KeyValuePair<string, object>(BaseUserEntity.FieldSubCompanyId, ids[i]), parameters);
                    parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BaseUserEntity.FieldDepartmentId, null),
                        new KeyValuePair<string, object>(BaseUserEntity.FieldDepartmentName, null)
                    };
                    userManager.SetProperty(new KeyValuePair<string, object>(BaseUserEntity.FieldDepartmentId, ids[i]), parameters);
                    parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BaseUserEntity.FieldWorkgroupId, null),
                        new KeyValuePair<string, object>(BaseUserEntity.FieldWorkgroupName, null)
                    };
                    userManager.SetProperty(new KeyValuePair<string, object>(BaseUserEntity.FieldWorkgroupId, ids[i]), parameters);
                    // 相应的员工也需要处理
                    var staffManager = new BaseStaffManager(dbHelper, userInfo);
                    staffManager.SetProperty(new KeyValuePair<string, object>(BaseStaffEntity.FieldCompanyId, ids[i]), new KeyValuePair<string, object>(BaseStaffEntity.FieldCompanyId, null));
                    staffManager.SetProperty(new KeyValuePair<string, object>(BaseStaffEntity.FieldSubCompanyId, ids[i]), new KeyValuePair<string, object>(BaseStaffEntity.FieldSubCompanyId, null));
                    staffManager.SetProperty(new KeyValuePair<string, object>(BaseStaffEntity.FieldDepartmentId, ids[i]), new KeyValuePair<string, object>(BaseStaffEntity.FieldDepartmentId, null));
                    staffManager.SetProperty(new KeyValuePair<string, object>(BaseStaffEntity.FieldWorkgroupId, ids[i]), new KeyValuePair<string, object>(BaseStaffEntity.FieldWorkgroupId, null));
                }
                var folderManager = new BaseFolderManager(dbHelper, userInfo);
                folderManager.SetDeleted(ids);
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
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
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
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
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
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                for (var i = 0; i < organizationIds.Length; i++)
                {
                    result += manager.MoveTo(organizationIds[i], parentId);
                }
            });
            return result;
        }
        #endregion

        #region public int BatchSetCode(BaseUserInfo userInfo, string[] ids, string[] codes)
        /// <summary>
        /// 批量重新生成编号
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键</param>
        /// <param name="codes">编号</param>
        /// <returns>影响行数</returns>
        public int BatchSetCode(BaseUserInfo userInfo, string[] ids, string[] codes)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                result = manager.BatchSetCode(ids, codes);
            });
            return result;
        }
        #endregion

        #region public int BatchSetSortCode(BaseUserInfo userInfo, string[] ids)
        /// <summary>
        /// 批量重新生成排序码
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int BatchSetSortCode(BaseUserInfo userInfo, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                result = manager.BatchSetSortCode(ids);
            });
            return result;
        }
        #endregion

        #region public DataTable GetDataTableByPage(BaseUserInfo userInfo, out int recordCount, int pageIndex, int pageSize, string condition, List<KeyValuePair<string, object>> dbParameters, string order = null)
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="condition">条件</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(BaseUserInfo userInfo, out int recordCount, int pageIndex, int pageSize, string condition, List<KeyValuePair<string, object>> dbParameters, string order = null)
        {
            var myRecordCount = 0;
            var dt = new DataTable(BaseOrganizationEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                if (SecretUtil.IsSqlSafe(condition))
                {
                    var organizeManager = new BaseOrganizationManager(dbHelper, userInfo);
                    dt = organizeManager.GetDataTableByPage(out myRecordCount, pageIndex, pageSize, condition, dbHelper.MakeParameters(dbParameters), order);
                    dt.TableName = BaseOrganizationEntity.TableName;
                }
                else
                {
                    // 记录注入日志
                    LogUtil.WriteLog("userInfo:" + userInfo.Serialize() + " " + condition, "SqlSafe");
                }
            });
            recordCount = myRecordCount;
            return dt;
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
            BaseOrganizationManager.CachePreheating();
        }
    }
}