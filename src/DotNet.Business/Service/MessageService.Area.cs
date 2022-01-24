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
    /// MessageService
    /// 消息服务
    /// 
    /// 修改记录
    /// 
    ///		2015.11.25 版本：2.0 JiRiGaLa 进行缓存优化，ExecuteReader 进行优化。
    ///		2013.11.12 版本：1.0 JiRiGaLa 创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.11.25</date>
    /// </author> 
    /// </summary>
    public partial class MessageService : IMessageService
    {
        /// <summary>
        /// 获取省份
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>省份数组</returns>
        public string[] GetProvince(BaseUserInfo userInfo)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var cacheKey = "AreaOrganizationProvince";
                result = CacheUtil.Cache(cacheKey, () => new BaseOrganizationManager(dbHelper, userInfo).GetProvince(), true);
            });

            return result;
        }

        /// <summary>
        /// 获取城市
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="province">省份</param>
        /// <returns>城市数组</returns>
        public string[] GetCity(BaseUserInfo userInfo, string province)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var cacheKey = "AreaOrganizationCity" + province;
                result = CacheUtil.Cache(cacheKey, () => new BaseOrganizationManager(dbHelper, userInfo).GetCity(province), true);

            });

            return result;
        }

        /// <summary>
        /// 获取县区
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="province">省份</param>
        /// <param name="city">城市</param>
        /// <returns>县区数组</returns>
        public string[] GetDistrict(BaseUserInfo userInfo, string province, string city)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var cacheKey = "AreaOrganizationDistrict" + city;
                result = CacheUtil.Cache(cacheKey, () => new BaseOrganizationManager(dbHelper, userInfo).GetDistrict(province, city), true);
            });

            return result;
        }

        /// <summary>
        /// 获得内部部门（公司的组织机构）
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="province">省</param>
        /// <returns>数据表</returns>
        public string[] GetOrganizationByProvince(BaseUserInfo userInfo, string province)
        {
            string[] result = null;


            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                string cacheKey = "OrganizationByProvince" + province;
                result = CacheUtil.Cache(cacheKey, () =>
                {
                    var manager = new BaseOrganizationManager(dbHelper, userInfo)
                    {
                        SelectFields = BaseOrganizationEntity.FieldId + "," + BaseOrganizationEntity.FieldFullName
                    };
                    DataTable dt = manager.GetOrganizationByProvince(province);
                    List<string> list = new List<string>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(dr[BaseOrganizationEntity.FieldId] + "=" + dr[BaseOrganizationEntity.FieldFullName]);
                    }

                    return list.ToArray();
                }, true);


            });


            return result;
        }

        /// <summary>
        /// 获得内部部门（公司的组织机构）
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="province">省</param>
        /// <param name="city">城市</param>
        /// <returns>数据表</returns>
        public string[] GetOrganizationByCity(BaseUserInfo userInfo, string province, string city)
        {
            string[] result = null;


            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());

            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {

                string cacheKey = "OrganizationByCity" + city;

                result = CacheUtil.Cache(cacheKey, () =>
                {
                    var manager = new BaseOrganizationManager(dbHelper, userInfo)
                    {
                        SelectFields = BaseOrganizationEntity.FieldId + "," + BaseOrganizationEntity.FieldFullName
                    };
                    DataTable dt = manager.GetOrganizationByCity(province, city);
                    List<string> list = new List<string>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(dr[BaseOrganizationEntity.FieldId] + "=" + dr[BaseOrganizationEntity.FieldFullName]);
                    }

                    return list.ToArray();
                }, true);

            });
            return result;
        }

        /// <summary>
        /// 获得公司列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="province">省</param>
        /// <param name="city">城市</param>
        /// <param name="district">县区</param>
        /// <returns>数据表</returns>
        public string[] GetOrganizationByDistrict(BaseUserInfo userInfo, string province, string city, string district)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var cacheKey = "OrganizationByDistrict" + district;
                result = CacheUtil.Cache(cacheKey,
                    () => new BaseOrganizationManager(dbHelper, userInfo).GetOrganizationByDistrict(province, city, district),
                    true);
            });
            return result;
        }

        /// <summary>
        /// 内部组织表
        /// </summary>
        public static DataTable InnerOrganizationDt = null;

        /// <summary>
        /// 最后检查组织机构时间
        /// </summary>
        public static DateTime LaseInnerOrganizationCheck = DateTime.MinValue;

        #region public DataTable GetInnerOrganizationDT(BaseUserInfo userInfo) 获取内部组织机构
        /// <summary>
        /// 获取内部组织机构
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public DataTable GetInnerOrganizationDT(BaseUserInfo userInfo)
        {
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseOrganizationManager(dbHelper, userInfo);
                if (LaseInnerOrganizationCheck == DateTime.MinValue)
                {
                }
                else
                {
                    // 2008.01.23 JiRiGaLa 修正错误
                    var timeSpan = DateTime.Now - LaseInnerOrganizationCheck;
                    if ((timeSpan.Minutes * 60 + timeSpan.Seconds) >= BaseSystemInfo.OnlineCheck * 10)
                    {
                    }
                }
                if (OnlineStateDt == null)
                {
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

                    InnerOrganizationDt = manager.Fill(commandText);
                    InnerOrganizationDt.TableName = BaseOrganizationEntity.TableName;
                    LaseInnerOrganizationCheck = DateTime.Now;
                }
                // BaseLogManager.Instance.Add(result, this.serviceName, MethodBase.GetCurrentMethod());
            });

            return InnerOrganizationDt;
        }
        #endregion

        /// <summary>
        /// 按父节点获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">父节点</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByParent(BaseUserInfo userInfo, string parentId)
        {
            var result = new DataTable(BaseOrganizationEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得组织机构列表
                var manager = new BaseOrganizationManager(dbHelper, userInfo);

                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldParentId, parentId),
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0)
                };

                result = manager.GetDataTable(parameters, BaseOrganizationEntity.FieldSortCode);
                result.DefaultView.Sort = BaseOrganizationEntity.FieldSortCode;
                result.TableName = BaseOrganizationEntity.TableName;
            });

            return result;
        }
    }
}