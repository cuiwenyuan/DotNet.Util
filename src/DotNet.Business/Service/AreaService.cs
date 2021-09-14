//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace DotNet.Business
{
    using IService;
    using Model;
    using Util;

    /// <summary>
    /// AreaService
    /// 区域服务
    /// 
    /// 修改记录
    /// 
    ///		2015.07.17 版本：2.1 JiRiGaLa 刷新缓存功能优化。
    ///		2015.07.03 版本：2.0 JiRiGaLa 修改大头笔增加日志功能。
    ///		2014.03.07 版本：1.0 JiRiGaLa 创建。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.07.17</date>
    /// </author> 
    /// </summary>


    public class AreaService : IAreaService
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
                var manager = new BaseManager(dbHelper, userInfo, BaseAreaEntity.TableName);
                result = manager.Exists(parameters, id);
            });
            return result;
        }
        #endregion

        #region public BaseAreaEntity GetObject(BaseUserInfo userInfo, string id)
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public BaseAreaEntity GetObject(BaseUserInfo userInfo, string id)
        {
            BaseAreaEntity entity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseAreaManager(dbHelper, userInfo);
                entity = manager.GetObject(id);
            });
            return entity;
        }
        #endregion

        #region public int Update(BaseUserInfo userInfo, BaseAreaEntity entity, out string statusCode, out string statusMessage)
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>影响行数</returns>
        public int Update(BaseUserInfo userInfo, BaseAreaEntity entity, out string statusCode, out string statusMessage)
        {
            var result = 0;

            var returnCode = string.Empty;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseAreaManager(dbHelper, userInfo);
                result = manager.Update(entity, out returnCode);
                returnMessage = manager.GetStateMessage(returnCode);
            });
            statusCode = returnCode;
            statusMessage = returnMessage;

            // 处理缓存优化性能

            var cacheKey = string.Empty;
            if (string.IsNullOrEmpty(entity.ParentId))
            {
                cacheKey = "AreaProvince";
            }
            else
            {
                cacheKey = "Area" + entity.ParentId;
            }
            CacheUtil.Remove(cacheKey);

            return result;
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
            var dt = new DataTable(BaseAreaEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseAreaManager(dbHelper, userInfo);
                dt = manager.GetDataTable(BaseAreaEntity.FieldId, ids, BaseAreaEntity.FieldSortCode);
                dt.TableName = BaseAreaEntity.TableName;
            });
            return dt;
        }
        #endregion

        /// <summary>
        /// 刷新列表
        /// 2015-07-17 吉日嘎拉 刷新缓存功能优化
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">父亲节点主键</param>
        /// <returns>数据表</returns>
        public void Refresh(BaseUserInfo userInfo, string id)
        {
            var cacheKey = "Area" + id;
            BaseAreaManager.RemoveCache(cacheKey);

            // 重新获取本身的缓存
            // cacheKey = "AreaList" + id;
            BaseAreaManager.GetCache(cacheKey);

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
            DataTable result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());

            var cacheKey = "Area";
            if (!string.IsNullOrEmpty(parentId))
            {
                cacheKey = "Area" + parentId;
            }

            result = CacheUtil.Cache(cacheKey, () =>
            {
                DataTable dt = null;
                ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
                {
                    // 这里是条件字段
                    var parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BaseAreaEntity.FieldParentId, parentId),
                        new KeyValuePair<string, object>(BaseAreaEntity.FieldEnabled, 1),
                        new KeyValuePair<string, object>(BaseAreaEntity.FieldDeleted, 0)
                    };
                    // 用静态方法获取数据，提高效率，获取列表，指定排序字段
                    dt = DbUtil.GetDataTable(dbHelper, BaseAreaEntity.TableName, parameters, 0, BaseAreaEntity.FieldSortCode, null);
                    // var manager = new BaseAreaManager(dbHelper, result);
                    // result = manager.GetDataTable(parameters, BaseAreaEntity.FieldSortCode);
                    dt.DefaultView.Sort = BaseAreaEntity.FieldSortCode;
                    dt.TableName = BaseAreaEntity.TableName;

                });
                return dt;
            }, true);
            return result;
        }
        #endregion

        #region public DataTable GetAreaRouteMarkEdit(BaseUserInfo userInfo, string parentId)
        /// <summary>
        /// 获取按省路由大头笔信息（输入）
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">父节点</param>
        /// <returns>数据表</returns>
        public DataTable GetAreaRouteMarkEdit(BaseUserInfo userInfo, string parentId)
        {
            DataTable result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var areaProvinceMarkManager = new BaseAreaProvinceMarkManager(dbHelper, userInfo);
                result = areaProvinceMarkManager.GetAreaRouteMarkEdit(parentId);
                result.TableName = BaseAreaProvinceMarkEntity.TableName;
            });

            return result;
        }
        #endregion

        #region public DataTable GetAreaRouteMarkByCache(BaseUserInfo userInfo, string parentId)
        /// <summary>
        /// 获取大头笔信息
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">父节点</param>
        /// <returns>数据表</returns>
        public DataTable GetAreaRouteMarkByCache(BaseUserInfo userInfo, string parentId)
        {
            DataTable result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            var cacheKey = "ProvinceMark";
            if (!string.IsNullOrEmpty(parentId))
            {
                cacheKey = "ProvinceMark" + parentId;
            }

            CacheUtil.Cache(cacheKey, () =>
            {
                DataTable dt = null;
                ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
                {
                    var commandText = @"SELECT PROVINCE, MARK, DESCRIPTION, " + BaseAreaEntity.FieldCreateTime + ", " + BaseAreaEntity.FieldCreateBy + ", " + BaseAreaEntity.FieldUpdateTime + ", " + BaseAreaEntity.FieldUpdateBy + " FROM basearea_provincemark WHERE ENABLED = 1 AND areaid = " + parentId;
                    dt = dbHelper.Fill(commandText);
                    dt.TableName = BaseAreaEntity.TableName;
                });
                return dt;
            }, true);


            return result;
        }
        #endregion

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="searchKey">查询</param>
        /// <returns>数据表</returns>
        public DataTable Search(BaseUserInfo userInfo, string searchKey)
        {
            DataTable result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseAreaManager(dbHelper, userInfo);
                result = manager.Search(searchKey);
            });

            return result;
        }

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
                var manager = new BaseAreaManager(dbHelper, userInfo);
                for (var i = 0; i < ids.Length; i++)
                {
                    // 设置为删除状态
                    result += manager.SetDeleted(ids[i]);
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
                var manager = new BaseAreaManager(dbHelper, userInfo);
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
                var manager = new BaseAreaManager(dbHelper, userInfo);
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
        /// <param name="ids">主键数组</param>
        /// <param name="parentId">父节点主键</param>
        /// <returns>影响行数</returns>
        public int BatchMoveTo(BaseUserInfo userInfo, string[] ids, string parentId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseAreaManager(dbHelper, userInfo);
                for (var i = 0; i < ids.Length; i++)
                {
                    result += manager.MoveTo(ids[i], parentId);
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
                var manager = new BaseAreaManager(dbHelper, userInfo);
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
                var manager = new BaseAreaManager(dbHelper, userInfo);
                result = manager.BatchSetSortCode(ids);
            });
            return result;
        }
        #endregion

        #region public string Add(BaseUserInfo userInfo, BaseAreaEntity entity, out string statusCode, out string statusMessage)
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>主键</returns>
        public string Add(BaseUserInfo userInfo, BaseAreaEntity entity, out string statusCode, out string statusMessage)
        {
            var result = string.Empty;

            var returnCode = string.Empty;
            var returnMessage = string.Empty;
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseAreaManager(dbHelper, userInfo);
                result = manager.Add(entity, out returnCode);
                returnMessage = manager.GetStateMessage(returnCode);
            });
            statusCode = returnCode;
            statusMessage = returnMessage;
            return result;
        }
        #endregion


        /// <summary>
        /// 获取省份
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">父级编号</param>
        /// <returns>省份列表</returns>
        public List<BaseAreaEntity> GetListByParent(BaseUserInfo userInfo, string parentId)
        {
            List<BaseAreaEntity> result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                result = BaseAreaManager.GetListByParentByCache(parentId);
            });

            return result;
        }


        /// <summary>
        /// 获取省份
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>省份列表</returns>
        public List<BaseAreaEntity> GetProvinceList(BaseUserInfo userInfo)
        {
            List<BaseAreaEntity> result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                result = BaseAreaManager.GetProvinceByCache();
            });

            return result;
        }

        /// <summary>
        /// 获取城市
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="provinceId">省份主键</param>
        /// <returns>城市列表</returns>
        public List<BaseAreaEntity> GetCityList(BaseUserInfo userInfo, string provinceId)
        {
            List<BaseAreaEntity> result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                result = BaseAreaManager.GetListByParentByCache(provinceId);
            });

            return result;
        }

        /// <summary>
        /// 获取县区
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="cityId">城市主键</param>
        /// <returns>县区列表</returns>
        public List<BaseAreaEntity> GetDistrictList(BaseUserInfo userInfo, string cityId)
        {
            List<BaseAreaEntity> result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                result = BaseAreaManager.GetListByParentByCache(cityId);
            });

            return result;
        }

        /// <summary>
        /// 获取街道
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="districtId">县区主键</param>
        /// <returns>街道列表</returns>
        public List<BaseAreaEntity> GetStreetList(BaseUserInfo userInfo, string districtId)
        {
            List<BaseAreaEntity> result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                result = BaseAreaManager.GetListByParentByCache(districtId);
            });

            return result;
        }
    }
}