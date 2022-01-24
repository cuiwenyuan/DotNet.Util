//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseAreaManager 
    /// 地区表(省、市、县)
    ///
    /// 修改记录
    ///
    ///     2015-06-09 版本：1.0 PanQiMin  添加获取所有城市包含省份的方法
    ///     2015.04.09 版本：1.0 PanQiMin  添加记录修改日志方法。
    ///		2014.02.11 版本：1.0 JiRiGaLa  表中添加是否可删除，可修改字段。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2014.02.11</date>
    /// </author>
    /// </summary>
    public partial class BaseAreaManager : BaseManager
    {
        /// <summary>
        /// 是否有省份权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissionId"></param>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        public bool HasProvincePermission(string userId, string permissionId, string provinceId)
        {
            var areaIds = GetAreas(userId, permissionId);
            return StringUtil.Exists(areaIds, provinceId);
        }

        /// <summary>
        /// 是否有城市权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissionId"></param>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public bool HasCityPermission(string userId, string permissionId, string cityId)
        {
            var areaIds = GetAreas(userId, permissionId);
            return StringUtil.Exists(areaIds, cityId);
        }

        /// <summary>
        /// 获取区域
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public string[] GetAreas(string userId, string permissionId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldDeleted, 0)
            };

            var tableName = UserInfo.SystemCode + "PermissionScope";
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, tableName);
            return permissionScopeManager.GetProperties(parameters, BasePermissionScopeEntity.FieldTargetId);
        }

        /// <summary>
        /// 按区域分割省、市、县、街道
        /// </summary>
        /// <param name="areaIds">区域主键</param>
        /// <param name="province">省</param>
        /// <param name="city">市</param>
        /// <param name="district">县</param>
        /// <param name="street">街道</param>
        public void SplitArea(string[] areaIds, out string[] province, out string[] city, out string[] district, out string[] street)
        {
            var list = new List<string>();
            province = null;
            for (var i = 0; i < areaIds.Length; i++)
            {
                if (areaIds[i] != null && areaIds[i].EndsWith("0000"))
                {
                    list.Add(areaIds[i]);
                }
            }
            province = list.ToArray();

            city = null;
            // 获取市，330300
            list = new List<string>();
            for (var i = 0; i < areaIds.Length; i++)
            {
                if (areaIds[i] == null)
                {
                    continue;
                }
                if (areaIds[i] != null && !areaIds[i].EndsWith("0000"))
                {
                    continue;
                }
                if (areaIds[i] != null && areaIds[i].Length == 9)
                {
                    continue;
                }
                // 县没能过滤，因为有省直属的县
            }
            city = list.ToArray();

            // 获得县，131024
            district = null;
            list = new List<string>();
            for (var i = 0; i < areaIds.Length; i++)
            {
                if (areaIds[i] != null && !areaIds[i].EndsWith("00"))
                {
                    list.Add(areaIds[i]);
                }
            }
            district = list.ToArray();

            // 获得街道， 110112117
            street = null;
            list = new List<string>();
            for (var i = 0; i < areaIds.Length; i++)
            {
                if (areaIds[i] != null && areaIds[i].Length == 9)
                {
                    list.Add(areaIds[i]);
                }
            }
            street = list.ToArray();
        }

        /// <summary>
        /// 获取用户有权限的区域的管理公司数组
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissionId"></param>
        /// <returns>管理公司数组</returns>
        public string[] GetUserManageCompanyIds(string userId, string permissionId)
        {
            string[] result = null;

            // 用户有权限的省？获取省的管理公司？
            // 用户有权限的市？市的管理公司？
            // 用户有权限的县？县的管理公司？
            // 用户有权限的街道？街道的管理公司？

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldDeleted, 0)
            };

            var tableName = UserInfo.SystemCode + "PermissionScope";
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, tableName);
            var areaIds = permissionScopeManager.GetProperties(parameters, BasePermissionScopeEntity.FieldTargetId);
            if (areaIds != null && areaIds.Length > 0)
            {
                var sql = "   SELECT DISTINCT(" + BaseAreaEntity.FieldManageCompanyId + ") "
                          + " FROM " + CurrentTableName
                          + " WHERE " + BaseAreaEntity.FieldLayer + " < 7 AND " + BaseAreaEntity.FieldManageCompanyId + " IS NOT NULL "
                          + " START WITH " + BaseAreaEntity.FieldId + " IN (" + string.Join(",", areaIds) + ")"
                          + " CONNECT BY PRIOR " + BaseAreaEntity.FieldId + " = " + BaseAreaEntity.FieldParentId;
                var dt = dbHelper.Fill(sql);
                result = BaseUtil.FieldToArray(dt, BaseAreaEntity.FieldManageCompanyId);
            }

            return result;
        }

        /// <summary>
        /// 获取管理网点列表
        /// </summary>
        /// <returns>返回序列化</returns>
        public List<BaseOrganizationEntity> GetUserManageCompanes(string userId, string permissionId)
        {
            List<BaseOrganizationEntity> result = null;
            var manageCompanyIds = GetUserManageCompanyIds(userId, permissionId);
            if (manageCompanyIds != null && manageCompanyIds.Length < 1)
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldId, manageCompanyIds),
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseOrganizationEntity.FieldDeleted, 0)
                };
                var organizeManager = new BaseOrganizationManager(DbHelper, UserInfo);
                result = organizeManager.GetList<BaseOrganizationEntity>(parameters);
            }
            return result;
        }

        /// <summary>
        /// 获取用户的管理网点
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissionId"></param>
        /// <returns>管理网点数组</returns>
        public string[] GetUserCompanyIds(string userId, string permissionId)
        {
            string[] result = null;

            // 用户有权限的省？获取省的网点？
            // 用户有权限的市？市的网点？
            // 用户有权限的县？县的网点？
            // 用户有权限的街道？街道的网点？
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldDeleted, 0)
            };

            var tableName = UserInfo.SystemCode + "PermissionScope";
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, tableName);
            var areaIds = permissionScopeManager.GetProperties(parameters, BasePermissionScopeEntity.FieldTargetId);

            // 按区域分割省、市、县、街道
            string[] province = null;
            string[] city = null;
            string[] district = null;
            string[] street = null;
            SplitArea(areaIds, out province, out city, out district, out street);

            string[] areaCompanyIds = null;
            if (areaIds != null && areaIds.Length > 0)
            {
                var commandText = " SELECT " + BaseOrganizationEntity.FieldId
                                    + " FROM " + BaseOrganizationEntity.TableName
                                    + " WHERE " + BaseOrganizationEntity.FieldEnabled + " = 1 "
                                    + "       AND " + BaseOrganizationEntity.FieldDeleted + " = 0 "
                                    + "       AND (";
                if (province != null && province.Length > 0)
                {
                    commandText += BaseOrganizationEntity.FieldProvinceId + " IN (" + ObjectUtil.ToList(province, "'") + ")";
                }
                if (city != null && city.Length > 0)
                {
                    if (province != null && province.Length > 0)
                    {
                        commandText += "  OR ";
                    }
                    commandText += BaseOrganizationEntity.FieldCityId + " IN (" + ObjectUtil.ToList(city, "'") + ")";
                }
                if (district != null && district.Length > 0)
                {
                    if ((province != null && province.Length > 0) || (city != null && city.Length > 0))
                    {
                        commandText += "  OR ";
                    }
                    commandText += BaseOrganizationEntity.FieldDistrictId + " IN (" + ObjectUtil.ToList(district, "'") + ")";
                }
                if (street != null && street.Length > 0)
                {
                    if ((province != null && province.Length > 0) || (city != null && city.Length > 0) || (district != null && district.Length > 0))
                    {
                        commandText += "  OR ";
                    }
                    commandText += BaseOrganizationEntity.FieldStreetId + " IN (" + ObjectUtil.ToList(areaIds, "'") + ")";
                }
                commandText += ")";

                var organizeManager = new BaseOrganizationManager();
                var dt = organizeManager.Fill(commandText);
                areaCompanyIds = BaseUtil.FieldToArray(dt, BaseOrganizationEntity.FieldId);
            }

            // 用户直接有权限的网点
            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldTargetCategory, BaseOrganizationEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldDeleted, 0)
            };
            var companyIds = permissionScopeManager.GetProperties(parameters, BasePermissionScopeEntity.FieldTargetId);

            result = StringUtil.Concat(companyIds, areaCompanyIds);
            return result;
        }

        /// <summary>
        /// 获取用户能显示的省？查看的范围
        /// 由于底层数据可以看，所以需要能选上层的省才可以
        /// </summary>
        /// <returns>省列表</returns>
        public List<BaseAreaEntity> GetUserProvince(string userId, string permissionId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldDeleted, 0)
            };

            var tableName = UserInfo.SystemCode + "PermissionScope";
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, tableName);
            var areaIds = permissionScopeManager.GetProperties(parameters, BasePermissionScopeEntity.FieldTargetId);
            for (var i = 0; i < areaIds.Length; i++)
            {
                areaIds[i] = areaIds[i].Substring(0, 2) + "0000";
            }

            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseAreaEntity.FieldParentId, null),
                new KeyValuePair<string, object>(BaseAreaEntity.FieldId, areaIds),
                new KeyValuePair<string, object>(BaseAreaEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseAreaEntity.FieldDeleted, 0)
            };
            return GetList<BaseAreaEntity>(parameters, BaseAreaEntity.FieldSortCode);
        }

        /// <summary>
        /// 获取用户能显示的市？查看的范围
        /// 由于底层数据可以市，所以需要能选上层的省才可以
        /// </summary>
        /// <returns>市列表</returns>
        public List<BaseAreaEntity> GetUserCity(string userId, string provinceId, string permissionId)
        {
            var tableName = UserInfo.SystemCode + "PermissionScope";
            provinceId = SecretUtil.SqlSafe(provinceId);
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldDeleted, 0)
            };

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, tableName);
            var areaIds = permissionScopeManager.GetProperties(parameters, BasePermissionScopeEntity.FieldTargetId);
            for (var i = 0; i < areaIds.Length; i++)
            {
                areaIds[i] = areaIds[i].Substring(0, 4) + "00";
            }

            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseAreaEntity.FieldParentId, provinceId),
                new KeyValuePair<string, object>(BaseAreaEntity.FieldId, areaIds),
                new KeyValuePair<string, object>(BaseAreaEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseAreaEntity.FieldDeleted, 0)
            };
            return GetList<BaseAreaEntity>(parameters, BaseAreaEntity.FieldSortCode);
        }

        /// <summary>
        /// 获取用户能显示的县？查看的范围
        /// 由于底层数据可以县，所以需要能选上层的省才可以
        /// </summary>
        /// <returns>县列表</returns>
        public List<BaseAreaEntity> GetUserDistrict(string userId, string cityId, string permissionId)
        {
            var tableName = UserInfo.SystemCode + "PermissionScope";
            cityId = SecretUtil.SqlSafe(cityId);
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldPermissionId, permissionId),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BasePermissionScopeEntity.FieldDeleted, 0)
            };

            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, tableName);
            var areaIds = permissionScopeManager.GetProperties(parameters, BasePermissionScopeEntity.FieldTargetId);

            var where = BaseAreaEntity.FieldId + " IN (" + ObjectUtil.ToList(areaIds) + ") AND ((ParentId = '" + cityId + "' AND Layer = 6) OR (Id = '" + cityId + "' AND Layer = 6)) AND Enabled = 1 AND " + BaseAreaEntity.FieldDeleted + " = 0 ";
            return GetList<BaseAreaEntity>(where);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public string Add(BaseAreaEntity entity, out string statusCode)
        {
            var result = string.Empty;
            // 检查是否重复
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseAreaEntity.FieldParentId, entity.ParentId),
                new KeyValuePair<string, object>(BaseAreaEntity.FieldFullName, entity.FullName),
                new KeyValuePair<string, object>(BaseAreaEntity.FieldDeleted, 0)
            };

            //注意Access 的时候，类型不匹配，会出错故此将 Id 传入
            if (BaseSystemInfo.UserCenterDbType == CurrentDbType.Access)
            {
                if (Exists(parameters, entity.Id))
                {
                    // 名称已重复
                    statusCode = Status.ErrorNameExist.ToString();
                }
                else
                {
                    parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BaseAreaEntity.FieldCode, entity.Code),
                        new KeyValuePair<string, object>(BaseAreaEntity.FieldDeleted, 0)
                    };
                    if (entity.Code.Length > 0 && Exists(parameters))
                    {
                        // 编号已重复
                        statusCode = Status.ErrorCodeExist.ToString();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(entity.QuickQuery))
                        {
                            // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
                            entity.QuickQuery = StringUtil.GetPinyin(entity.FullName).ToLower();
                        }
                        if (string.IsNullOrEmpty(entity.SimpleSpelling))
                        {
                            // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
                            entity.SimpleSpelling = StringUtil.GetSimpleSpelling(entity.FullName).ToLower();
                        }
                        result = AddEntity(entity);
                        // 运行成功
                        statusCode = Status.OkAdd.ToString();
                    }
                }
            }
            else if (Exists(parameters))
            {
                // 名称已重复
                statusCode = Status.ErrorNameExist.ToString();
            }
            else
            {
                parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseAreaEntity.FieldCode, entity.Code),
                    new KeyValuePair<string, object>(BaseAreaEntity.FieldDeleted, 0)
                };
                if (entity.Code.Length > 0 && Exists(parameters))
                {
                    // 编号已重复
                    statusCode = Status.ErrorCodeExist.ToString();
                }
                else
                {
                    result = AddEntity(entity);
                    // 运行成功
                    statusCode = Status.OkAdd.ToString();
                }
            }
            return result;
        }

        /// <summary>
        /// 根据权限获取清单
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="userId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="childrens"></param>
        /// <returns></returns>
        public List<BaseAreaEntity> GetListByPermission(string systemCode, string userId, string permissionCode = "Resource.ManagePermission", bool childrens = true)
        {
            List<BaseAreaEntity> result = null;

            // 先获取有权限的主键
            var tableName = UserInfo.SystemCode + "PermissionScope";
            // string tableName = "Base" + "PermissionScope";
            var manager = new BasePermissionScopeManager(dbHelper, UserInfo, tableName);
            var ids = manager.GetTreeResourceScopeIds(systemCode, userId, BaseAreaEntity.TableName, permissionCode, childrens);
            // 然后再获取地区表，获得所有的列表
            if (ids != null && ids.Length > 0)
            {
                result = GetList<BaseAreaEntity>(ids).OrderBy(t => t.SortCode).ToList();
            }

            return result;
        }

        /// <summary>
        /// 获取有权限的省份列表
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">操作权限编号</param>
        /// <param name="childrens">是否包含子节点</param>
        /// <returns>地区列表</returns>
        public List<BaseAreaEntity> GetProvinceListByPermission(string systemCode, string userId, string permissionCode = "Resource.ManagePermission", bool childrens = true)
        {
            var result = GetListByPermission(systemCode, userId, permissionCode, childrens);
            result = result.Where<BaseAreaEntity>(t => t.Province != null && t.City == null && t.District == null).ToList();
            return result;
        }

        /// <summary>
        /// 获取有权限的城市列表
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">操作权限编号</param>
        /// <param name="childrens">是否包含子节点</param>
        /// <returns>地区列表</returns>
        public List<BaseAreaEntity> GetCityListByPermission(string systemCode, string userId, string permissionCode = "Resource.ManagePermission", bool childrens = true)
        {
            var result = GetListByPermission(systemCode, userId, permissionCode, childrens);
            result = result.Where<BaseAreaEntity>(t => t.City != null && t.District == null).ToList();
            return result;
        }

        /// <summary>
        /// 获取有权限的县区列表
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">操作权限编号</param>
        /// <param name="childrens">是否包含子节点</param>
        /// <returns>地区列表</returns>
        public List<BaseAreaEntity> GetDistrictListByPermission(string systemCode, string userId, string permissionCode = "Resource.ManagePermission", bool childrens = true)
        {
            var result = GetListByPermission(systemCode, userId, permissionCode, childrens);
            result = result.Where<BaseAreaEntity>(t => t.District != null).ToList();
            return result;
        }

        /// <summary>
        /// 获取清单
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public List<BaseAreaEntity> GetListByParent(string parentId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseAreaEntity.FieldParentId, parentId),
                new KeyValuePair<string, object>(BaseAreaEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseAreaEntity.FieldDeleted, 0)
            };
            return GetList<BaseAreaEntity>(parameters, BaseAreaEntity.FieldSortCode);
        }

        /// <summary>
        /// 获取省份
        /// </summary>
        /// <returns></returns>
        public List<BaseAreaEntity> GetProvince()
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                // 省份的下拉框，层级应该是4，执行效率会高
                // parameters.Add(new KeyValuePair<string, object>(BaseAreaEntity.FieldParentId, null));
                new KeyValuePair<string, object>(BaseAreaEntity.FieldLayer, 4),
                new KeyValuePair<string, object>(BaseAreaEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseAreaEntity.FieldDeleted, 0)
            };
            return GetList<BaseAreaEntity>(parameters, BaseAreaEntity.FieldSortCode);
        }

        /// <summary>
        /// 获取城市
        /// </summary>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        public List<BaseAreaEntity> GetCity(string provinceId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseAreaEntity.FieldParentId, provinceId),
                new KeyValuePair<string, object>(BaseAreaEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseAreaEntity.FieldDeleted, 0)
            };
            return GetList<BaseAreaEntity>(parameters, BaseAreaEntity.FieldSortCode);
        }
        /// <summary>
        /// 获取区域
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public List<BaseAreaEntity> GetDistrict(string cityId)
        {
            cityId = SecretUtil.SqlSafe(cityId);
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldParentId, cityId),
                new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldDeleted, 0)
            };
            return GetList<BaseAreaEntity>(parameters, BaseItemDetailsEntity.FieldSortCode);
        }

        /// <summary>
        /// 获取街道
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns></returns>
        public List<BaseAreaEntity> GetStreet(string districtId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseAreaEntity.FieldParentId, districtId),
                new KeyValuePair<string, object>(BaseAreaEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseAreaEntity.FieldDeleted, 0)
            };
            return GetList<BaseAreaEntity>(parameters, BaseAreaEntity.FieldSortCode);
        }

        /// <summary>
        /// 获取所有市
        /// </summary>
        /// <returns>城市数据表</returns>
        public DataTable GetAllCity()
        {
            /*
            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseAreaEntity.FieldLayer, 5));
            parameters.Add(new KeyValuePair<string, object>(BaseAreaEntity.FieldEnabled, 1));
            parameters.Add(new KeyValuePair<string, object>(BaseAreaEntity.FieldDeleted, 0));
            return this.GetDataTable(parameters);
            */

            var commandText = " SELECT * FROM " +
                                 " (SELECT * FROM BASEAREA WHERE PARENTID IN (SELECT ID FROM BASEAREA WHERE PARENTID IS NULL) " +
                                 " UNION " +
                                 " SELECT * FROM BASEAREA WHERE PARENTID IS NULL) " +
                                 " WHERE DELETIONSTATECODE = 0 " +
                                   " AND ENABLED = 1 ";
            return dbHelper.Fill(commandText);
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="parentId">父级主键</param>
        /// <returns>影响行数</returns>
        public int MoveTo(string id, string parentId)
        {
            return SetProperty(id, new KeyValuePair<string, object>(BaseAreaEntity.FieldParentId, parentId));
        }

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public override int BatchSave(DataTable dt)
        {
            var result = 0;
            var entity = new BaseAreaEntity();
            foreach (DataRow dr in dt.Rows)
            {
                // 删除状态
                if (dr.RowState == DataRowState.Deleted)
                {
                    var id = dr[BaseAreaEntity.FieldId, DataRowVersion.Original].ToString();
                    if (id.Length > 0)
                    {
                        result += DeleteObject(id);
                    }
                }
                // 被修改过
                if (dr.RowState == DataRowState.Modified)
                {
                    var id = dr[BaseAreaEntity.FieldId, DataRowVersion.Original].ToString();
                    if (id.Length > 0)
                    {
                        entity.GetFrom(dr);
                        result += UpdateEntity(entity);
                    }
                }
                // 添加状态, 远程接口调用序列化时都会变成添加状态
                if (dr.RowState == DataRowState.Added)
                {
                    entity.GetFrom(dr);
                    if (!string.IsNullOrEmpty(entity.Id))
                    {
                        if (UpdateEntity(entity) > 0)
                        {
                            result++;
                        }
                        else
                        {
                            result += AddEntity(entity).Length > 0 ? 1 : 0;
                        }
                    }
                    else
                    {
                        result += AddEntity(entity).Length > 0 ? 1 : 0;
                    }
                }
                if (dr.RowState == DataRowState.Unchanged)
                {
                    continue;
                }
                if (dr.RowState == DataRowState.Detached)
                {
                    continue;
                }
            }
            return result;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="searchKey">查询</param>
        /// <returns>数据表</returns>
        public DataTable Search(string searchKey)
        {
            var sql = "SELECT * "
                    + " FROM " + CurrentTableName
                    + "  WHERE " + BaseAreaEntity.FieldDeleted + " = 0 ";

            var dbParameters = new List<IDbDataParameter>();

            searchKey = searchKey.Trim();
            if (!string.IsNullOrEmpty(searchKey))
            {
                // 六、这里是进行支持多种数据库的参数化查询
                sql += string.Format(" AND ({0} LIKE {1} ", BaseAreaEntity.FieldFullName, DbHelper.GetParameter(BaseAreaEntity.FieldFullName));
                sql += string.Format(" OR {0} LIKE {1} ", BaseAreaEntity.FieldShortName, DbHelper.GetParameter(BaseAreaEntity.FieldShortName));
                sql += string.Format(" OR {0} LIKE {1} ", BaseAreaEntity.FieldMark, DbHelper.GetParameter(BaseAreaEntity.FieldMark));
                sql += string.Format(" OR {0} LIKE {1}) ", BaseAreaEntity.FieldDescription, DbHelper.GetParameter(BaseAreaEntity.FieldDescription));

                if (searchKey.IndexOf("%") < 0)
                {
                    searchKey = string.Format("%{0}%", searchKey);
                }

                dbParameters.Add(DbHelper.MakeParameter(BaseAreaEntity.FieldFullName, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseAreaEntity.FieldShortName, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseAreaEntity.FieldMark, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseAreaEntity.FieldDescription, searchKey));
            }
            sql += " ORDER BY " + BaseAreaEntity.FieldSortCode + " DESC ";

            return DbHelper.Fill(sql, dbParameters.ToArray());
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="searchKey">查询</param>
        /// <returns>数据表</returns>
        public List<BaseAreaEntity> SearchByList(string searchKey)
        {
            List<BaseAreaEntity> result = null;

            var sql = "SELECT * "
                    + " FROM " + CurrentTableName
                    + "  WHERE " + BaseAreaEntity.FieldDeleted + " = 0 ";

            var dbParameters = new List<IDbDataParameter>();

            searchKey = searchKey.Trim();
            if (!string.IsNullOrEmpty(searchKey))
            {
                // 六、这里是进行支持多种数据库的参数化查询
                sql += string.Format(" AND ({0} LIKE {1} ", BaseAreaEntity.FieldFullName, DbHelper.GetParameter(BaseAreaEntity.FieldFullName));
                sql += string.Format(" OR {0} LIKE {1} ", BaseAreaEntity.FieldShortName, DbHelper.GetParameter(BaseAreaEntity.FieldShortName));
                sql += string.Format(" OR {0} LIKE {1} ", BaseAreaEntity.FieldMark, DbHelper.GetParameter(BaseAreaEntity.FieldMark));
                sql += string.Format(" OR {0} LIKE {1}) ", BaseAreaEntity.FieldDescription, DbHelper.GetParameter(BaseAreaEntity.FieldDescription));

                if (searchKey.IndexOf("%") < 0)
                {
                    searchKey = string.Format("%{0}%", searchKey);
                }

                dbParameters.Add(DbHelper.MakeParameter(BaseAreaEntity.FieldFullName, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseAreaEntity.FieldShortName, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseAreaEntity.FieldMark, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseAreaEntity.FieldDescription, searchKey));
            }
            sql += " ORDER BY " + BaseAreaEntity.FieldSortCode + " DESC ";

            using (var dataReader = DbHelper.ExecuteReader(sql, dbParameters.ToArray()))
            {
                result = GetList<BaseAreaEntity>(dataReader);
            }

            return result;
        }

        /// <summary>
        /// 获取拼音
        /// </summary>
        /// <returns></returns>
        public int GetPinYin()
        {
            var result = 0;
            var list = GetList<BaseAreaEntity>();
            foreach (var entity in list)
            {
                if (string.IsNullOrEmpty(entity.QuickQuery))
                {
                    // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
                    entity.QuickQuery = StringUtil.GetPinyin(entity.FullName).ToLower();
                }
                if (string.IsNullOrEmpty(entity.SimpleSpelling))
                {
                    // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
                    entity.SimpleSpelling = StringUtil.GetSimpleSpelling(entity.FullName).ToLower();
                }
                result += UpdateEntity(entity);
            }
            return result;
        }

        /// <summary>
        /// 根据SQL从数据获取数据,将DataTable中的数据保存到指定文件夹下保存成.csv文件格式
        /// </summary>
        /// <param name="fileName">文件名</param>
        public void ExportToCsv(string fileName)
        {
            var commandText = string.Format(@"SELECT {0} 全称
                                                        ,{1} 省
                                                        ,{2} 市
                                                        ,{3} 县
                                                        ,{4} 大头笔
                                                        ,{5} 延迟天数
                                                        ,{6} 机打大头笔
                                                        ,{7} 允许代收 
                                                        ,{8} 允许到付 
                                                        ,{9} 管理网点 
                                                        ,{10} 最大代收款 
                                                        ,{11} 最大到付款 
                                                        ,{12} 网络订单 
                                                        ,{13} 开通业务 
                                                        ,{14} 超区 
                                                        ,{15} 揽收 
                                                        ,{16} 发件 
                                                        ,{17} 全境派送
                                                        ,{18} 层级 
                    FROM {19} 
                                                   WHERE {20} = 1 
                                                     AND {21} = 0
                                                     AND {22} != 0 
                                                ORDER BY {23}
                                                        ,{24}
                                                        ,{25} "
                , BaseAreaEntity.FieldFullName
                , BaseAreaEntity.FieldProvince
                , BaseAreaEntity.FieldCity
                , BaseAreaEntity.FieldDistrict
                , BaseAreaEntity.FieldMark
                , BaseAreaEntity.FieldDelayDay
                , BaseAreaEntity.FieldPrintMark
                , BaseAreaEntity.FieldAllowGoodsPay
                , BaseAreaEntity.FieldAllowToPay
                , BaseAreaEntity.FieldManageCompany
                , BaseAreaEntity.FieldMaxGoodsPayment
                , BaseAreaEntity.FieldMaxToPayment
                , BaseAreaEntity.FieldNetworkOrders
                , BaseAreaEntity.FieldOpening
                , BaseAreaEntity.FieldOutOfRange
                , BaseAreaEntity.FieldReceive
                , BaseAreaEntity.FieldSend
                , BaseAreaEntity.FieldWhole
                , BaseAreaEntity.FieldLayer
                , BaseAreaEntity.TableName
                , BaseAreaEntity.FieldEnabled
                , BaseAreaEntity.FieldDeleted
                , BaseAreaEntity.FieldLayer
                , BaseAreaEntity.FieldProvince
                , BaseAreaEntity.FieldCity
                , BaseAreaEntity.FieldDistrict);

            var dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            dbHelper.Open();
            using (var dataReader = DbHelper.ExecuteReader(commandText))
            {
                CsvUtil.ExportCsv(dataReader, fileName);
            }
            dbHelper.Close();
        }

        /// <summary>
        /// 从缓存获取获取实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public static BaseAreaEntity GetEntityByCache(string id)
        {
            BaseAreaEntity result = null;
            var cacheKey = "Area:";
            if (!string.IsNullOrEmpty(id))
            {
                cacheKey += id;
            }
            result = CacheUtil.Cache(cacheKey, () => new BaseAreaManager().GetEntity(id), true);
            return result;
        }

        /// <summary>
        /// 从缓存获取名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetNameByByCache(string id)
        {
            var result = string.Empty;

            var entity = GetEntityByCache(id);
            if (entity != null)
            {
                result = entity.FullName;
            }

            return result;
        }

        /// <summary>
        /// 从缓存获取清单
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="refreshCache"></param>
        /// <returns></returns>
        public static List<BaseAreaEntity> GetListByParentByCache(string parentId, bool refreshCache = false)
        {
            List<BaseAreaEntity> result = null;
            var key = "AreaList:";
            if (!string.IsNullOrEmpty(parentId))
            {
                key += parentId;
            }
            result = CacheUtil.Cache(key, () => new BaseAreaManager().GetListByParent(parentId), true, refreshCache);
            return result;
        }

        /// <summary>
        /// 从缓存获取省份
        /// </summary>
        /// <returns></returns>
        public static List<BaseAreaEntity> GetProvinceByCache()
        {
            List<BaseAreaEntity> result = null;

            var key = "AreaProvince:";
            result = CacheUtil.Cache(key, () => new BaseAreaManager().GetProvince(), true);
            return result;
        }

        /// <summary>
        /// 缓存预热,强制重新缓存
        /// </summary>
        /// <returns>影响行数</returns>
        public static int CachePreheating()
        {
            var result = 0;

            // 把所有的组织机构都缓存起来的代码
            var manager = new BaseAreaManager();
            using (var dataReader = manager.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    var entity = BaseEntity.Create<BaseAreaEntity>(dataReader, false);
                    if (entity != null && entity.Layer < 7)
                    {
                        SetCache(entity);
                        result++;
                        Console.WriteLine(result + " : " + entity.FullName);
                        // 把列表缓存起来
                        GetListByParentByCache(entity.Id, true);
                        Console.WriteLine(result + " : " + entity.Id + " " + entity.FullName + " List");
                    }
                }
                dataReader.Close();
            }

            return result;
        }
    }
}