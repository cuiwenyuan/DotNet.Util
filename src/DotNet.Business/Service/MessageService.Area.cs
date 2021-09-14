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
    ///		<name>JiRiGaLa</name>
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
                var cacheKey = "AreaOrganizeProvince";
                result = CacheUtil.Cache(cacheKey, () => new BaseOrganizeManager(dbHelper, userInfo).GetProvince(), true);
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
                var cacheKey = "AreaOrganizeCity" + province;
                result = CacheUtil.Cache(cacheKey, () => new BaseOrganizeManager(dbHelper, userInfo).GetCity(province), true);

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
                var cacheKey = "AreaOrganizeDistrict" + city;
                result = CacheUtil.Cache(cacheKey, () => new BaseOrganizeManager(dbHelper, userInfo).GetDistrict(province, city), true);
            });

            return result;
        }

        /// <summary>
        /// 获得内部部门（公司的组织机构）
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="province">省</param>
        /// <returns>数据表</returns>
        public string[] GetOrganizeByProvince(BaseUserInfo userInfo, string province)
        {
            string[] result = null;


            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                string cacheKey = "OrganizeByProvince" + province;
                result = CacheUtil.Cache(cacheKey, () =>
                {
                    var manager = new BaseOrganizeManager(dbHelper, userInfo)
                    {
                        SelectFields = BaseOrganizeEntity.FieldId + "," + BaseOrganizeEntity.FieldFullName
                    };
                    DataTable dt = manager.GetOrganizeByProvince(province);
                    List<string> list = new List<string>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(dr[BaseOrganizeEntity.FieldId] + "=" + dr[BaseOrganizeEntity.FieldFullName]);
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
        public string[] GetOrganizeByCity(BaseUserInfo userInfo, string province, string city)
        {
            string[] result = null;


            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());

            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {

                string cacheKey = "OrganizeByCity" + city;

                result = CacheUtil.Cache(cacheKey, () =>
                {
                    var manager = new BaseOrganizeManager(dbHelper, userInfo)
                    {
                        SelectFields = BaseOrganizeEntity.FieldId + "," + BaseOrganizeEntity.FieldFullName
                    };
                    DataTable dt = manager.GetOrganizeByCity(province, city);
                    List<string> list = new List<string>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(dr[BaseOrganizeEntity.FieldId] + "=" + dr[BaseOrganizeEntity.FieldFullName]);
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
        public string[] GetOrganizeByDistrict(BaseUserInfo userInfo, string province, string city, string district)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var cacheKey = "OrganizeByDistrict" + district;
                result = CacheUtil.Cache(cacheKey,
                    () => new BaseOrganizeManager(dbHelper, userInfo).GetOrganizeByDistrict(province, city, district),
                    true);
            });
            return result;
        }

        /// <summary>
        /// 内部组织表
        /// </summary>
        public static DataTable InnerOrganizeDt = null;

        /// <summary>
        /// 最后检查组织机构时间
        /// </summary>
        public static DateTime LaseInnerOrganizeCheck = DateTime.MinValue;

        #region public DataTable GetInnerOrganizeDT(BaseUserInfo userInfo) 获取内部组织机构
        /// <summary>
        /// 获取内部组织机构
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public DataTable GetInnerOrganizeDT(BaseUserInfo userInfo)
        {
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseOrganizeManager(dbHelper, userInfo);
                if (LaseInnerOrganizeCheck == DateTime.MinValue)
                {
                }
                else
                {
                    // 2008.01.23 JiRiGaLa 修正错误
                    var timeSpan = DateTime.Now - LaseInnerOrganizeCheck;
                    if ((timeSpan.Minutes * 60 + timeSpan.Seconds) >= BaseSystemInfo.OnLineCheck * 10)
                    {
                    }
                }
                if (OnLineStateDt == null)
                {
                    var commandText = string.Empty;

                    if (BaseSystemInfo.OrganizeDynamicLoading)
                    {
                        commandText = "    SELECT * "
                                    + " FROM " + BaseOrganizeEntity.TableName
                                    + "     WHERE " + BaseOrganizeEntity.FieldDeleted + " = 0 "
                                    + "           AND " + BaseOrganizeEntity.FieldIsInnerOrganize + " = 1 "
                                    + "           AND " + BaseOrganizeEntity.FieldEnabled + " = 1 "
                                    + "           AND (" + BaseOrganizeEntity.FieldParentId + " IS NULL "
                                    + "                OR " + BaseOrganizeEntity.FieldParentId + " IN (SELECT " + BaseOrganizeEntity.FieldId + " FROM " + BaseOrganizeEntity.TableName + " WHERE " + BaseOrganizeEntity.FieldDeleted + " = 0 AND " + BaseOrganizeEntity.FieldIsInnerOrganize + " = 1 AND " + BaseOrganizeEntity.FieldEnabled + " = 1 AND " + BaseOrganizeEntity.FieldParentId + " IS NULL)) "
                                    + "  ORDER BY " + BaseOrganizeEntity.FieldSortCode;
                    }
                    else
                    {
                        commandText = "    SELECT * "
                                    + " FROM " + BaseOrganizeEntity.TableName
                                    + "     WHERE " + BaseOrganizeEntity.FieldDeleted + " = 0 "
                                    + "           AND " + BaseOrganizeEntity.FieldIsInnerOrganize + " = 1 "
                                    + "           AND " + BaseOrganizeEntity.FieldEnabled + " = 1 "
                                    + "  ORDER BY " + BaseOrganizeEntity.FieldSortCode;
                    }

                    InnerOrganizeDt = manager.Fill(commandText);
                    InnerOrganizeDt.TableName = BaseOrganizeEntity.TableName;
                    LaseInnerOrganizeCheck = DateTime.Now;
                }
                // BaseLogManager.Instance.Add(result, this.serviceName, MethodBase.GetCurrentMethod());
            });

            return InnerOrganizeDt;
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
            var result = new DataTable(BaseOrganizeEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得组织机构列表
                var manager = new BaseOrganizeManager(dbHelper, userInfo);

                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizeEntity.FieldParentId, parentId),
                    new KeyValuePair<string, object>(BaseOrganizeEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseOrganizeEntity.FieldDeleted, 0)
                };

                result = manager.GetDataTable(parameters, BaseOrganizeEntity.FieldSortCode);
                result.DefaultView.Sort = BaseOrganizeEntity.FieldSortCode;
                result.TableName = BaseOrganizeEntity.TableName;
            });

            return result;
        }
    }
}