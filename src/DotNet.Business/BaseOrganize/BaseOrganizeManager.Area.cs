//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseOrganizeManager
    /// 组织机构、部门表
    ///
    /// 修改记录
    ///     
    ///		2015.11.25 版本：1.2 JiRiGaLa	GetArea、GetProvince、GetCity、GetDistrict函数ExecuteReader进行优化。
    ///		2015.11.03 版本：1.1 JiRiGaLa	按区域获取组织机构的主键。
    ///		2014.02.20 版本：1.0 JiRiGaLa	有些思想进行了改进。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.11.25</date>
    /// </author>
    /// </summary>
    public partial class BaseOrganizeManager : BaseManager //, IBaseOrganizeManager
    {
        /// <summary>
        /// 按区域主键获取组织机构主键数组
        /// </summary>
        /// <param name="areaIds">区域主键数组</param>
        /// <returns>组织机构主键数组</returns>
        public string[] GetIdsByAreaIds(string[] areaIds)
        {
            string[] result = null;

            if (areaIds != null && areaIds.Length > 0)
            {
                var commandText = " SELECT " + BaseOrganizeEntity.FieldId
                                    + " FROM " + BaseOrganizeEntity.TableName
                                    + "  WHERE " + BaseOrganizeEntity.FieldEnabled + " = 1 AND " + BaseOrganizeEntity.FieldDeleted + " = 0 "
                                    + "        AND (" + BaseOrganizeEntity.FieldProvinceId + " IN (" + StringUtil.ArrayToList(areaIds) + ")"
                                    + "         OR " + BaseOrganizeEntity.FieldCityId + " IN (" + StringUtil.ArrayToList(areaIds) + ")"
                                    + "         OR " + BaseOrganizeEntity.FieldStreetId + " IN (" + StringUtil.ArrayToList(areaIds) + ")"
                                    + "         OR " + BaseOrganizeEntity.FieldDistrictId + " IN (" + StringUtil.ArrayToList(areaIds) + "))";

                var ids = new List<string>();
                using (var dr = DbHelper.ExecuteReader(commandText))
                {
                    while (dr.Read())
                    {
                        ids.Add(dr[BaseOrganizeEntity.FieldId].ToString());
                    }
                }
                result = ids.ToArray();
            }

            return result;
        }

        /// <summary>
        /// 获取区域
        /// </summary>
        /// <returns>区域数组</returns>
        public string[] GetArea()
        {
            string[] result = null;

            var commandText = "SELECT DISTINCT Area FROM BaseOrganize WHERE Enabled = 1 AND Area IS NOT NULL AND " + BaseOrganizeEntity.FieldDeleted + " = 0 ORDER BY Area";
            // DataTable dt = DbHelper.Fill(commandText);
            // return BaseUtil.FieldToArray(dt, "Area");

            var area = new List<string>();
            using (var dataReader = DbHelper.ExecuteReader(commandText))
            {
                while (dataReader.Read())
                {
                    area.Add(dataReader["Area"].ToString());
                }
            }
            result = area.ToArray();

            return result;
        }

        /// <summary>
        /// 获取省份
        /// </summary>
        /// <param name="area">区域</param>
        /// <returns>省份数组</returns>
        public string[] GetProvince(string area = null)
        {
            string[] result = null;

            var commandText = "SELECT DISTINCT Province FROM BaseOrganize WHERE Province IS NOT NULL AND Enabled = 1 AND " + BaseOrganizeEntity.FieldDeleted + " = 0 ORDER BY Province ";
            if (!string.IsNullOrEmpty(area))
            {
                commandText = "SELECT DISTINCT Province FROM BaseOrganize WHERE Area = '" + area + "' AND Province IS NOT NULL AND Enabled = 1 AND " + BaseOrganizeEntity.FieldDeleted + " = 0 ORDER BY Province ";
            }
            // DataTable dt = DbHelper.Fill(commandText);
            // return BaseUtil.FieldToArray(dt, "Province");

            var province = new List<string>();
            using (var dataReader = DbHelper.ExecuteReader(commandText))
            {
                while (dataReader.Read())
                {
                    province.Add(dataReader["Province"].ToString());
                }
            }
            result = province.ToArray();

            return result;
        }

        /// <summary>
        /// 获取城市
        /// </summary>
        /// <param name="province">省份</param>
        /// <returns>城市数组</returns>
        public string[] GetCity(string province)
        {
            string[] result = null;

            var commandText = "SELECT DISTINCT City FROM BaseOrganize WHERE Province = '" + province + "' AND Enabled = 1 AND City IS NOT NULL AND " + BaseOrganizeEntity.FieldDeleted + " = 0 ORDER BY City";
            // DataTable dt = DbHelper.Fill(commandText);
            // return BaseUtil.FieldToArray(dt, "City");

            var city = new List<string>();
            using (var dataReader = DbHelper.ExecuteReader(commandText))
            {
                while (dataReader.Read())
                {
                    city.Add(dataReader["City"].ToString());
                }
            }
            result = city.ToArray();

            return result;
        }

        /// <summary>
        /// 获取县区
        /// </summary>
        /// <param name="province">省份</param>
        /// <param name="city">城市</param>
        /// <returns>县区数组</returns>
        public string[] GetDistrict(string province, string city)
        {
            string[] result = null;

            var commandText = "SELECT DISTINCT District FROM BaseOrganize WHERE Province = '" + province + "' AND City = '" + city + "' AND District IS NOT NULL AND Enabled = 1 AND " + BaseOrganizeEntity.FieldDeleted + " = 0 ORDER BY District";
            // DataTable dt = DbHelper.Fill(commandText);
            // return BaseUtil.FieldToArray(dt, "District");

            var district = new List<string>();
            using (var dataReader = DbHelper.ExecuteReader(commandText))
            {
                while (dataReader.Read())
                {
                    district.Add(dataReader["District"].ToString());
                }
            }
            result = district.ToArray();

            return result;
        }

        /// <summary>
        /// 获得内部部门（公司的组织机构）
        /// </summary>
        /// <param name="province">省</param>
        /// <returns>数据表</returns>
        public DataTable GetOrganizeByProvince(string province)
        {
            var commandText = "SELECT " + SelectFields + " FROM BaseOrganize WHERE Province = '" + province + "' AND (City IS NULL OR City = '') AND (District IS NULL OR District = '') AND Enabled = 1 AND " + BaseOrganizeEntity.FieldDeleted + " = 0";
            return DbHelper.Fill(commandText);
        }

        /// <summary>
        /// 获得内部部门（公司的组织机构）
        /// </summary>
        /// <param name="province">省份</param>
        /// <param name="city">城市</param>
        /// <returns>数据表</returns>
        public DataTable GetOrganizeByCity(string province, string city)
        {
            var commandText = "SELECT " + SelectFields + " FROM BaseOrganize "
                                + " WHERE City = '" + city + "'"
                                + "       AND (District IS NULL OR District = '') AND Enabled = 1 AND " + BaseOrganizeEntity.FieldDeleted + " = 0";
            if (!string.IsNullOrEmpty(province))
            {
                commandText += " AND " + BaseOrganizeEntity.FieldProvince + " ='" + province + "'";
            }
            return DbHelper.Fill(commandText);
        }

        /// <summary>
        /// 获得公司列表
        /// 2015-11-25 吉日嘎拉 进行改进
        /// </summary>
        /// <param name="province">省</param>
        /// <param name="city">城市</param>
        /// <param name="district">县区</param>
        /// <returns>数据表</returns>
        public string[] GetOrganizeByDistrict(string province, string city, string district)
        {
            string[] result = null;

            SelectFields = BaseOrganizeEntity.FieldId
                                + "," + BaseOrganizeEntity.FieldFullName
                                 + "," + BaseOrganizeEntity.FieldProvinceId
                                  + "," + BaseOrganizeEntity.FieldCityId
                                   + "," + BaseOrganizeEntity.FieldDistrictId;

            var commandText = "SELECT " + SelectFields + " FROM BaseOrganize WHERE District = '" + district + "' AND Enabled = 1 AND " + BaseOrganizeEntity.FieldDeleted + " = 0 ";
            if (!string.IsNullOrEmpty(province))
            {
                commandText += " AND " + BaseOrganizeEntity.FieldProvince + " ='" + province + "'";
            }
            if (!string.IsNullOrEmpty(city))
            {
                commandText += " AND " + BaseOrganizeEntity.FieldCity + " ='" + city + "'";
            }

            // DataTable dt = DbHelper.Fill(commandText);
            var list = new List<string>();
            using (var dataReader = DbHelper.ExecuteReader(commandText))
            {
                while (dataReader.Read())
                {
                    if (string.IsNullOrEmpty(dataReader[BaseOrganizeEntity.FieldProvinceId].ToString())
                    || string.IsNullOrEmpty(dataReader[BaseOrganizeEntity.FieldCityId].ToString())
                    || string.IsNullOrEmpty(dataReader[BaseOrganizeEntity.FieldDistrictId].ToString())
                    )
                    {
                        list.Add(dataReader[BaseOrganizeEntity.FieldId] + "=*" + dataReader[BaseOrganizeEntity.FieldFullName]);
                    }
                    else
                    {
                        list.Add(dataReader[BaseOrganizeEntity.FieldId] + "=" + dataReader[BaseOrganizeEntity.FieldFullName]);
                    }
                }
            }
            result = list.ToArray();

            return result;
        }
    }
}