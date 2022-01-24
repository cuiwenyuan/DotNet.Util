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
    /// ParameterService
    /// 参数服务
    /// 
    /// 修改记录
    /// 
    ///		2015.07.16 版本：2.1 JiRiGaLa 支持多表处理。
    ///		2011.07.15 版本：2.0 JiRiGaLa 获取服务器端配置的功能改进。
    ///		2008.04.30 版本：1.0 JiRiGaLa 创建。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.07.16</date>
    /// </author> 
    /// </summary>


    public class ParameterService : IParameterService
    {
        /// <summary>
        /// 获取服务器上的配置信息
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="key">配置项主键</param>
        /// <returns>配置内容</returns>
        public string GetServiceConfig(BaseUserInfo userInfo, string key)
        {
            return UserConfigUtil.GetValue(key);
        }

        #region public BaseOrganizationEntity GetEntity(BaseUserInfo userInfo, string tableName, string id)
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public BaseParameterEntity GetEntity(BaseUserInfo userInfo, string tableName, string id)
        {
            BaseParameterEntity entity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseParameterManager(dbHelper, userInfo, tableName);
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
        public int Update(BaseUserInfo userInfo, string tableName, BaseParameterEntity entity)
        {
            var result = 0;

            var returnCode = string.Empty;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseParameterManager(dbHelper, userInfo, tableName);
                result = manager.Update(entity);
            });
            return result;
        }
        #endregion

        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="categoryCode">分类编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <param name="parameterCode">参数编号</param>
        /// <returns>参数值</returns>
        public string GetParameter(BaseUserInfo userInfo, string tableName, string categoryCode, string parameterId, string parameterCode)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseParameterManager(dbHelper, userInfo, tableName);
                result = manager.GetParameter(tableName, categoryCode, parameterId, parameterCode);
            });

            return result;
        }

        /// <summary>
        /// 设置参数值
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="categoryCode">分类编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <param name="parameterCode">参数编号</param>
        /// <param name="parameterContent">参数内容</param>
        /// <returns>影响行数</returns>
        public int SetParameter(BaseUserInfo userInfo, string tableName, string categoryCode, string parameterId, string parameterCode, string parameterContent)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseParameterManager(dbHelper, userInfo, tableName);
                result = manager.SetParameter(tableName, categoryCode, parameterId, parameterCode, parameterContent);
            });
            return result;
        }

        /// <summary>
        /// 添加参数设置
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="categoryCode">类别编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <param name="parameterCode">编码</param>
        /// <param name="parameterContent">参数内容</param>
        /// <returns>主键</returns>
        public string AddParameter(BaseUserInfo userInfo, string tableName, string categoryCode, string parameterId,
            string parameterCode, string parameterContent)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseParameterManager(dbHelper, userInfo, tableName);
                result = manager.AddParameter(tableName, categoryCode, parameterId, parameterCode, parameterContent);
            });

            return result;
        }

        /// <summary>
        /// 获取记录
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public DataTable GetSystemParameter(BaseUserInfo userInfo)
        {
            var result = new DataTable(BaseParameterEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseParameterManager(dbHelper, userInfo, "SystemParameter");
                result = manager.GetSystemParameter();
                result.TableName = BaseParameterEntity.TableName;
            });

            return result;
        }

        /// <summary>
        /// 获取参数列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="categoryCode">分类编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByParameter(BaseUserInfo userInfo, string categoryCode, string parameterId)
        {
            var result = new DataTable(BaseParameterEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseParameterManager(dbHelper, userInfo);
                result = manager.GetDataTableByParameter(categoryCode, parameterId);
                result.TableName = BaseParameterEntity.TableName;

                // 2015-12-21 吉日嘎拉，这里重新设置mac缓存，删除掉缓存。
                if (categoryCode.Equals("MacAddress"))
                {
                    BaseUserManager.ResetMacAddressByCache(parameterId);
                }
            });

            return result;
        }

        /// <summary>
        /// 按编号获取参数列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="categoryCode">分类编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <param name="parameterCode">参数编号</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableParameterCode(BaseUserInfo userInfo, string categoryCode, string parameterId, string parameterCode)
        {
            var dt = new DataTable(BaseParameterEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseParameterManager(dbHelper, userInfo);
                dt = manager.GetDataTableParameterCode(categoryCode, parameterId, parameterCode);
                dt.TableName = BaseParameterEntity.TableName;
            });
            return dt;
        }

        #region public bool Exists(BaseUserInfo userInfo, List<KeyValuePair<string, object>> parameters)
        /// <summary>
        /// 用户名是否重复
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parameters">字段名,字段值</param>
        /// <returns>已存在</returns>
        public bool Exists(BaseUserInfo userInfo, List<KeyValuePair<string, object>> parameters)
        {
            var result = false;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseParameterManager(dbHelper, userInfo);
                result = manager.Exists(parameters);
            });

            return result;
        }
        #endregion

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseUserInfo userInfo, string tableName, BaseParameterEntity entity)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDbWithTransaction(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseParameterManager(dbHelper, userInfo, tableName);
                result = manager.Add(entity);
            });

            return result;
        }

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
                var manager = new BaseParameterManager(dbHelper, userInfo, tableName);
                for (var i = 0; i < ids.Length; i++)
                {
                    // 设置为删除状态
                    result += manager.SetDeleted(ids[i], true, true);
                }
            });

            return result;
        }
        #endregion

        /// <summary>
        /// 删除参数
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="categoryCode">分类编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <returns>影响行数</returns>
        public int DeleteByParameter(BaseUserInfo userInfo, string categoryCode, string parameterId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseParameterManager(dbHelper, userInfo);
                result = manager.DeleteByParameter(categoryCode, parameterId);
            });

            return result;
        }

        /// <summary>
        /// 按参数编号删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="categoryCode">分类编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <param name="parameterCode">参数编号</param>
        /// <returns>影响行数</returns>
        public int DeleteByParameterCode(BaseUserInfo userInfo, string categoryCode, string parameterId, string parameterCode)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseParameterManager(dbHelper, userInfo);
                result = manager.DeleteByParameterCode(categoryCode, parameterId, parameterCode);
            });
            return result;
        }

        /// <summary>
        /// 删除参数
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
                var manager = new BaseParameterManager(dbHelper, userInfo);
                result = manager.SetDeleted(id, true, true, 4);
            });

            return result;
        }

        /// <summary>
        /// 批量删除参数
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
                var manager = new BaseParameterManager(dbHelper, userInfo);
                for (var i = 0; i < ids.Length; i++)
                {
                    result += manager.SetDeleted(ids[i], true, true, 4);
                }
            });

            return result;
        }
    }
}