//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <remarks>
    /// BaseOrganizeManager
    /// 组织机构管理
    /// 
    /// 修改记录
    /// 
    ///	版本：2.0 2016.05.10  JiRiGaLa 用查询的方式，查找网点数据，快速搜索。
    ///	版本：1.6 2016.01.08  JiRiGaLa 实现只读连接，读写分离。
    ///	版本：1.5 2016.01.04  JiRiGaLa 写入主键的方式进行改进。
    ///	版本：1.4 2015.10.09  JiRiGaLa GetCache方法实现。
    ///	版本：1.3 2015.07.17  JiRiGaLa 缓存清除功能实现（其实是重新获取了就可以了）。
    ///	版本：1.2 2015.06.15  JiRiGaLa 增加强制设置缓存的功能。
    ///	版本：1.1 2015.04.11  JiRiGaLa 取消锁、提高效率、从独立的小库里读取缓存数据、希望能提高性能。
    ///	版本：1.0 2015.01.06  JiRiGaLa 缓存优化。
    ///	
    /// <author>  
    ///		<name>Troy.Cui</name>
    ///		<date>2016.05.10</date>
    /// </author> 
    /// </remarks>
    public partial class BaseOrganizeManager
    {
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool RemoveCache(string key)
        {
            var result = false;

            if (!string.IsNullOrWhiteSpace(key))
            {
                result = CacheUtil.Remove(key);
            }

            return result;
        }

        /// <summary>
        /// 从缓存获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static BaseOrganizeEntity GetCacheByKey(string key)
        {
            BaseOrganizeEntity result = null;

            if (!string.IsNullOrWhiteSpace(key))
            {
                CacheUtil.Get<BaseOrganizeEntity>(key);
            }

            return result;
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="entity">实体</param>
        public static void SetCache(BaseOrganizeEntity entity)
        {
            if (entity != null && entity.Id != null)
            {
                var key = string.Empty;
                // key = "Organize:" + entity.Id;
                key = "O:" + entity.Id;
                CacheUtil.Set<BaseOrganizeEntity>(key, entity);

                // key = "OrganizeByCode:" + entity.Code;
                key = "OBC:" + entity.Code;
                CacheUtil.Set<string>(key, entity.Id);

                //key = "OrganizeByName:" + entity.FullName;
                key = "OBN:" + entity.FullName;
                CacheUtil.Set<string>(key, entity.Id);
            }
        }

        /// <summary>
        /// 从缓存获取
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static BaseOrganizeEntity GetEntityByCodeByCache(string code)
        {
            BaseOrganizeEntity result = null;

            if (!string.IsNullOrEmpty(code))
            {
                // string key = "OrganizeByCode:" + code;
                var key = "OBC:" + code;
                result = CacheUtil.Cache(key, () => new BaseOrganizeManager().GetEntityByCode(code), true);
            }

            return result;
        }

        /// <summary>
        /// 从缓存获取
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static BaseOrganizeEntity GetEntityByNameByCache(string fullName)
        {
            BaseOrganizeEntity result = null;
            if (!string.IsNullOrEmpty(fullName))
            {
                // string key = "OrganizeByName:" + fullName;
                var key = "OBN:" + fullName;
                result = CacheUtil.Cache(key, () => new BaseOrganizeManager().GetEntityByName(fullName), true);
            }

            return result;
        }

        /// <summary>
        /// 获取省份
        /// 2015-11-25 吉日嘎拉 采用缓存方式，效率应该会更高
        /// </summary>
        /// <param name="area">区域</param>
        /// <returns>省份数组</returns>
        public static string[] GetProvinceByCache(string area = null)
        {
            string[] result = null;

            if (!string.IsNullOrWhiteSpace(area))
            {
                var key = "OP:" + area;
                result = CacheUtil.Cache(key, () => new BaseOrganizeManager().GetProvince(area), true);
            }

            return result;
        }

        /// <summary>
        /// 获取城市
        /// 2015-11-25 吉日嘎拉 采用缓存方式，效率应该会更高
        /// </summary>
        /// <param name="province">省份</param>
        /// <returns>城市数组</returns>
        public static string[] GetCityByCache(string province = null)
        {
            string[] result = null;

            if (!string.IsNullOrWhiteSpace(province))
            {
                var key = "OC:" + province;
                result = CacheUtil.Cache(key, () => new BaseOrganizeManager().GetCity(province), true);
            }
            return result;
        }

        /// <summary>
        /// 获取县区
        /// 2015-11-25 吉日嘎拉 采用缓存方式，效率应该会更高
        /// </summary>
        /// <param name="province">省份</param>
        /// <param name="city">城市</param>
        /// <returns>县区数组</returns>
        public static string[] GetDistrictByCache(string province, string city)
        {
            string[] result = null;

            if (string.IsNullOrWhiteSpace(province))
            {
                province = string.Empty;
            }
            if (string.IsNullOrWhiteSpace(city))
            {
                city = string.Empty;
            }
            var key = "OD:" + province + ":" + city;
            result = CacheUtil.Cache(key, () => new BaseOrganizeManager().GetDistrict(province, city), true);

            return result;
        }

        /// <summary>
        /// 获得公司列表
        /// 2015-11-25 吉日嘎拉 进行改进
        /// </summary>
        /// <param name="province">省</param>
        /// <param name="city">城市</param>
        /// <param name="district">县区</param>
        /// <returns>数据表</returns>
        public static string[] GetOrganizeByDistrictByCache(string province, string city, string district)
        {
            string[] result = null;

            var organize = string.Empty;
            if (string.IsNullOrWhiteSpace(province))
            {
                province = string.Empty;
            }
            if (string.IsNullOrWhiteSpace(city))
            {
                city = string.Empty;
            }
            if (string.IsNullOrWhiteSpace(district))
            {
                district = string.Empty;
            }
            var key = "OBD:" + province + ":" + city + ":" + district;
            result = CacheUtil.Cache(key, () => new BaseOrganizeManager().GetOrganizeByDistrict(province, city, district), true);
            return result;
        }

        /// <summary>
        /// 比较
        /// </summary>
        public class Comparint : IEqualityComparer<KeyValuePair<string, string>>
        {
            /// <summary>
            /// 相等
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
            {
                return x.Key == y.Key;
            }

            /// <summary>
            /// 获取HashCode
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(KeyValuePair<string, string> obj)
            {
                return obj.ToString().GetHashCode();
            }
        }

        /// <summary>
        /// 缓存预热
        /// </summary>
        /// <param name="organizeEntity"></param>
        /// <param name="score"></param>
        public static void CachePreheatingSpelling01(BaseOrganizeEntity organizeEntity, double score = 0)
        {
            // 读取到的数据直接强制设置到缓存里
            var id = organizeEntity.Id;
            // 2016-01-06 吉日嘎拉 网点编号不能大小写转换，否则查询就乱套了，不能改变原样
            var code = organizeEntity.Code;
            var fullName = organizeEntity.FullName;
            var simpleSpelling = organizeEntity.SimpleSpelling;
            var enabled = organizeEntity.Enabled.ToString();
            var deletionStateCode = organizeEntity.DeletionStateCode.ToString();
            var organize = id + ";" + code + ";" + fullName + ";" + enabled + ";" + deletionStateCode;

            // 2016-04-11 吉日嘎拉 已经被删除的网点不需要缓存了
            if (organizeEntity.DeletionStateCode.HasValue && organizeEntity.DeletionStateCode.Value == 1)
            {
                // organize += " 已删除";
                return;
            }
            if (organizeEntity.Enabled.HasValue && organizeEntity.Enabled.Value == 0)
            {
                // organize += " 失效"; 
            }
            var key = string.Empty;
            // 01：所有网点查询的缓存数据方法
            if (!string.IsNullOrEmpty(code))
            {
                key = "All:" + code.ToLower();
                CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
            }
            if (!string.IsNullOrEmpty(fullName))
            {
                key = "All:" + fullName.ToLower();
                CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
            }
            if (!string.IsNullOrEmpty(simpleSpelling))
            {
                key = "All:" + simpleSpelling.ToLower();
                CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
            }
        }
        /// <summary>
        /// 缓存预热
        /// </summary>
        /// <param name="organizeEntity"></param>
        /// <param name="score"></param>
        public static void CachePreheatingSpelling02(BaseOrganizeEntity organizeEntity, double score = 0)
        {
            // 读取到的数据直接强制设置到缓存里
            var id = organizeEntity.Id;
            // 2016-01-06 吉日嘎拉 网点编号不能大小写转换，否则查询就乱套了，不能改变原样
            var code = organizeEntity.Code;
            var fullName = organizeEntity.FullName;
            var simpleSpelling = organizeEntity.SimpleSpelling;
            var enabled = organizeEntity.Enabled.ToString();
            var deletionStateCode = organizeEntity.DeletionStateCode.ToString();
            var organize = id + ";" + code + ";" + fullName + ";" + enabled + ";" + deletionStateCode;
            // 2016-04-11 吉日嘎拉 已经被删除的网点不需要缓存了
            if (organizeEntity.DeletionStateCode.HasValue && organizeEntity.DeletionStateCode.Value == 1)
            {
                // organize += " 已删除";
                return;
            }
            if (organizeEntity.Enabled.HasValue && organizeEntity.Enabled.Value == 0)
            {
                // organize += " 失效"; 
            }

            var key = string.Empty;
            // 02：父级主键缓存数据方法
            var parentId = organizeEntity.ParentId;
            if (!string.IsNullOrEmpty(parentId))
            {
                if (!string.IsNullOrEmpty(code))
                {
                    // 2016-01-06 吉日嘎拉 这里需要小写，提高效率，提高有善度
                    key = "ParentId:" + parentId + ":" + code.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(fullName))
                {
                    key = "ParentId:" + parentId + ":" + fullName.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(simpleSpelling))
                {
                    key = "ParentId:" + parentId + ":" + simpleSpelling.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
            }
        }

        /// <summary>
        /// 缓存预热
        /// </summary>
        /// <param name="organizeEntity"></param>
        /// <param name="score"></param>
        public static void CachePreheatingSpelling03(BaseOrganizeEntity organizeEntity, double score = 0)
        {
            // 读取到的数据直接强制设置到缓存里
            var id = organizeEntity.Id;
            // 2016-01-06 吉日嘎拉 网点编号不能大小写转换，否则查询就乱套了，不能改变原样
            var code = organizeEntity.Code;
            var fullName = organizeEntity.FullName;
            var simpleSpelling = organizeEntity.SimpleSpelling;
            var enabled = organizeEntity.Enabled.ToString();
            var deletionStateCode = organizeEntity.DeletionStateCode.ToString();
            var organize = id + ";" + code + ";" + fullName + ";" + enabled + ";" + deletionStateCode;
            // 2016-04-11 吉日嘎拉 已经被删除的网点不需要缓存了
            if (organizeEntity.DeletionStateCode.HasValue && organizeEntity.DeletionStateCode.Value == 1)
            {
                // organize += " 已删除";
                return;
            }
            if (organizeEntity.DeletionStateCode.HasValue && organizeEntity.DeletionStateCode.Value == 1)
            {
                // organize += " 已删除";
            }

            var key = string.Empty;

            // 03: 按一级网点缓存数据方法
            var companyId = organizeEntity.CompanyId;
            if (!string.IsNullOrEmpty(companyId))
            {
                if (!string.IsNullOrEmpty(code))
                {
                    // 2016-01-18 吉日嘎拉 这里需要小写，提高效率，提高有善度
                    key = "CompanyId:" + companyId + ":" + code.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(fullName))
                {
                    key = "CompanyId:" + companyId + ":" + fullName.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(simpleSpelling))
                {
                    key = "CompanyId:" + companyId + ":" + simpleSpelling.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
            }
        }

        /// <summary>
        /// 缓存预热
        /// </summary>
        /// <param name="organizeEntity"></param>
        /// <param name="score"></param>
        public static void CachePreheatingSpelling04(BaseOrganizeEntity organizeEntity, double score = 0)
        {
            // 读取到的数据直接强制设置到缓存里
            var id = organizeEntity.Id;
            // 2016-01-06 吉日嘎拉 网点编号不能大小写转换，否则查询就乱套了，不能改变原样
            var code = organizeEntity.Code;
            var fullName = organizeEntity.FullName;
            var simpleSpelling = organizeEntity.SimpleSpelling;
            var enabled = organizeEntity.Enabled.ToString();
            var deletionStateCode = organizeEntity.DeletionStateCode.ToString();
            var organize = id + ";" + code + ";" + fullName + ";" + enabled + ";" + deletionStateCode;
            // 2016-04-11 吉日嘎拉 已经被删除的网点不需要缓存了
            if (organizeEntity.DeletionStateCode.HasValue && organizeEntity.DeletionStateCode.Value == 1)
            {
                // organize += " 已删除";
                return;
            }
            if (organizeEntity.DeletionStateCode.HasValue && organizeEntity.DeletionStateCode.Value == 1)
            {
                // organize += " 已删除";
            }

            var key = string.Empty;

            // 04：结算中心主键缓存数据方法
            var costCenterId = organizeEntity.CostCenterId;
            if (!string.IsNullOrEmpty(costCenterId))
            {
                if (!string.IsNullOrEmpty(code))
                {
                    // 2016-01-06 吉日嘎拉 这里需要小写，提高效率，提高有善度
                    key = "CostCenterId:" + costCenterId + ":" + code.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(fullName))
                {
                    key = "CostCenterId:" + costCenterId + ":" + fullName.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(simpleSpelling))
                {
                    key = "CostCenterId:" + costCenterId + ":" + simpleSpelling.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
            }

            // 05：按省缓存数据方法
            var provinceId = organizeEntity.ProvinceId;
            if (!string.IsNullOrEmpty(provinceId))
            {
                if (!string.IsNullOrEmpty(code))
                {
                    // 2016-01-06 吉日嘎拉 这里需要小写，提高效率，提高有善度
                    key = "ProvinceId:" + provinceId + ":" + code.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(fullName))
                {
                    key = "ProvinceId:" + provinceId + ":" + fullName.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(simpleSpelling))
                {
                    key = "ProvinceId:" + provinceId + ":" + simpleSpelling.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
            }

            // 06：按市缓存数据方法
            var cityId = organizeEntity.CityId;
            if (!string.IsNullOrEmpty(cityId))
            {
                if (!string.IsNullOrEmpty(code))
                {
                    // 2016-01-06 吉日嘎拉 这里需要小写，提高效率，提高有善度
                    key = "CityId:" + cityId + ":" + code.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(fullName))
                {
                    key = "CityId:" + cityId + ":" + fullName.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(simpleSpelling))
                {
                    key = "CityId:" + cityId + ":" + simpleSpelling.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
            }

            // 07：所有下属递归的方式进行快速缓存检索（START WITH CONNECT BY PRIOR） 包括自己
            var startId = organizeEntity.Id;
            while (!string.IsNullOrEmpty(startId))
            {
                if (!string.IsNullOrEmpty(code))
                {
                    // 2016-01-06 吉日嘎拉 这里需要小写，提高效率，提高有善度
                    key = "StartId:" + startId + ":" + code.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(fullName))
                {
                    key = "StartId:" + startId + ":" + fullName.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(simpleSpelling))
                {
                    key = "StartId:" + startId + ":" + simpleSpelling.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
                // 获取上级的上级，一直进行循环，在缓存里进行计算，提高效率
                startId = GetParentIdByCache(startId);
            }

            // 08：发航空
            var sendAir = organizeEntity.SendAir.ToString();
            if (!string.IsNullOrEmpty(sendAir) && sendAir.Equals("1"))
            {
                if (!string.IsNullOrEmpty(code))
                {
                    // 2016-01-06 吉日嘎拉 这里需要小写，提高效率，提高有善度
                    key = "SendAir:" + code.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(fullName))
                {
                    key = "SendAir:" + fullName.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(simpleSpelling))
                {
                    key = "SendAir:" + simpleSpelling.ToLower();
                    CacheUtil.Set(key, organize, new TimeSpan(15, 0, 0, 0));
                }
            }

            // 输出到屏幕看看运行效果如何？心里有个数
            Console.WriteLine(score + " " + organize);
        }

        private static BaseOrganizeEntity GetOrganizeEntity(IDataReader dataReader)
        {
            var entity = new BaseOrganizeEntity
            {
                Id = dataReader[BaseOrganizeEntity.FieldId].ToString(),
                Code = dataReader[BaseOrganizeEntity.FieldCode].ToString(),
                FullName = dataReader[BaseOrganizeEntity.FieldFullName].ToString(),
                SimpleSpelling = dataReader[BaseOrganizeEntity.FieldSimpleSpelling].ToString().ToLower(),
                CostCenterId = dataReader[BaseOrganizeEntity.FieldCostCenterId].ToString(),
                ProvinceId = dataReader[BaseOrganizeEntity.FieldProvinceId].ToString(),
                CompanyId = dataReader[BaseOrganizeEntity.FieldCompanyId].ToString(),
                CityId = dataReader[BaseOrganizeEntity.FieldCityId].ToString(),
                ParentId = dataReader[BaseOrganizeEntity.FieldParentId].ToString(),
                SendAir = BaseUtil.ConvertToInt(dataReader[BaseOrganizeEntity.FieldSendAir]),
                Enabled = BaseUtil.ConvertToInt(dataReader[BaseOrganizeEntity.FieldEnabled], 1),
                DeletionStateCode = BaseUtil.ConvertToInt(dataReader[BaseOrganizeEntity.FieldDeleted], 0),
                SortCode = BaseUtil.ConvertToInt(dataReader[BaseOrganizeEntity.FieldSortCode])
            };

            return entity;
        }
    }
}