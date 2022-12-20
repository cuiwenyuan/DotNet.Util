//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
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
    /// BaseOrganizationManager
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
    public partial class BaseOrganizationManager
    {
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override bool RemoveCache(string key)
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
        public static BaseOrganizationEntity GetCacheByKey(string key)
        {
            BaseOrganizationEntity result = null;

            if (!string.IsNullOrWhiteSpace(key))
            {
                CacheUtil.Get<BaseOrganizationEntity>(key);
            }

            return result;
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="entity">实体</param>
        public static void SetCache(BaseOrganizationEntity entity)
        {
            if (entity != null)
            {
                var key = string.Empty;
                // key = "Organization:" + entity.Id;
                key = "O:" + entity.Id;
                CacheUtil.Set<BaseOrganizationEntity>(key, entity);

                // key = "OrganizationByCode:" + entity.Code;
                key = "OBC:" + entity.Code;
                CacheUtil.Set<string>(key, entity.Id.ToString());

                //key = "OrganizationByName:" + entity.Name;
                key = "OBN:" + entity.Name;
                CacheUtil.Set<string>(key, entity.Id.ToString());
            }
        }

        /// <summary>
        /// 从缓存获取
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static BaseOrganizationEntity GetEntityByCodeByCache(string code)
        {
            BaseOrganizationEntity result = null;

            if (!string.IsNullOrEmpty(code))
            {
                // string key = "OrganizationByCode:" + code;
                var key = "OBC:" + code;
                result = CacheUtil.Cache(key, () => new BaseOrganizationManager().GetEntityByCode(code), true);
            }

            return result;
        }

        /// <summary>
        /// 从缓存获取
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static BaseOrganizationEntity GetEntityByNameByCache(string name)
        {
            BaseOrganizationEntity result = null;
            if (!string.IsNullOrEmpty(name))
            {
                // string key = "OrganizationByName:" + name;
                var key = "OBN:" + name;
                result = CacheUtil.Cache(key, () => new BaseOrganizationManager().GetEntityByName(name), true);
            }

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
        /// <param name="organizationEntity"></param>
        /// <param name="score"></param>
        public static void CachePreheatingSpelling01(BaseOrganizationEntity organizationEntity, double score = 0)
        {
            // 读取到的数据直接强制设置到缓存里
            var id = organizationEntity.Id;
            // 2016-01-06 吉日嘎拉 网点编号不能大小写转换，否则查询就乱套了，不能改变原样
            var code = organizationEntity.Code;
            var name = organizationEntity.Name;
            var simpleSpelling = organizationEntity.SimpleSpelling;
            var enabled = organizationEntity.Enabled.ToString();
            var deletionStateCode = organizationEntity.Deleted.ToString();
            var organization = id + ";" + code + ";" + name + ";" + enabled + ";" + deletionStateCode;

            // 2016-04-11 吉日嘎拉 已经被删除的网点不需要缓存了
            if (organizationEntity.Deleted == 1)
            {
                // organization += " 已删除";
                return;
            }

            var key = string.Empty;
            // 01：所有网点查询的缓存数据方法
            if (!string.IsNullOrEmpty(code))
            {
                key = "All:" + code.ToLower();
                CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
            }
            if (!string.IsNullOrEmpty(name))
            {
                key = "All:" + name.ToLower();
                CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
            }
            if (!string.IsNullOrEmpty(simpleSpelling))
            {
                key = "All:" + simpleSpelling.ToLower();
                CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
            }
        }
        /// <summary>
        /// 缓存预热
        /// </summary>
        /// <param name="organizationEntity"></param>
        /// <param name="score"></param>
        public static void CachePreheatingSpelling02(BaseOrganizationEntity organizationEntity, double score = 0)
        {
            // 读取到的数据直接强制设置到缓存里
            var id = organizationEntity.Id;
            // 2016-01-06 吉日嘎拉 网点编号不能大小写转换，否则查询就乱套了，不能改变原样
            var code = organizationEntity.Code;
            var name = organizationEntity.Name;
            var simpleSpelling = organizationEntity.SimpleSpelling;
            var enabled = organizationEntity.Enabled.ToString();
            var deletionStateCode = organizationEntity.Deleted.ToString();
            var organization = id + ";" + code + ";" + name + ";" + enabled + ";" + deletionStateCode;
            // 2016-04-11 吉日嘎拉 已经被删除的网点不需要缓存了
            if (organizationEntity.Deleted == 1)
            {
                // organization += " 已删除";
                return;
            }

            var key = string.Empty;
            // 02：父级主键缓存数据方法
            var parentId = organizationEntity.ParentId;
            if (!string.IsNullOrEmpty(parentId.ToString()))
            {
                if (!string.IsNullOrEmpty(code))
                {
                    // 2016-01-06 吉日嘎拉 这里需要小写，提高效率，提高有善度
                    key = "ParentId:" + parentId + ":" + code.ToLower();
                    CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(name))
                {
                    key = "ParentId:" + parentId + ":" + name.ToLower();
                    CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(simpleSpelling))
                {
                    key = "ParentId:" + parentId + ":" + simpleSpelling.ToLower();
                    CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
                }
            }
        }

        /// <summary>
        /// 缓存预热
        /// </summary>
        /// <param name="organizationEntity"></param>
        /// <param name="score"></param>
        public static void CachePreheatingSpelling03(BaseOrganizationEntity organizationEntity, double score = 0)
        {
            // 读取到的数据直接强制设置到缓存里
            var id = organizationEntity.Id;
            // 2016-01-06 吉日嘎拉 网点编号不能大小写转换，否则查询就乱套了，不能改变原样
            var code = organizationEntity.Code;
            var name = organizationEntity.Name;
            var simpleSpelling = organizationEntity.SimpleSpelling;
            var enabled = organizationEntity.Enabled.ToString();
            var deletionStateCode = organizationEntity.Deleted.ToString();
            var organization = id + ";" + code + ";" + name + ";" + enabled + ";" + deletionStateCode;
            // 2016-04-11 吉日嘎拉 已经被删除的网点不需要缓存了
            if (organizationEntity.Deleted == 1)
            {
                // organization += " 已删除";
                return;
            }

            var key = string.Empty;

            // 03: 按一级网点缓存数据方法
            if (organizationEntity.CompanyId > 0)
            {
                if (!string.IsNullOrEmpty(code))
                {
                    // 2016-01-18 吉日嘎拉 这里需要小写，提高效率，提高有善度
                    key = "CompanyId:" + organizationEntity.CompanyId + ":" + code.ToLower();
                    CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(name))
                {
                    key = "CompanyId:" + organizationEntity.CompanyId + ":" + name.ToLower();
                    CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(simpleSpelling))
                {
                    key = "CompanyId:" + organizationEntity.CompanyId + ":" + simpleSpelling.ToLower();
                    CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
                }
            }
        }

        /// <summary>
        /// 缓存预热
        /// </summary>
        /// <param name="organizationEntity"></param>
        /// <param name="score"></param>
        public static void CachePreheatingSpelling04(BaseOrganizationEntity organizationEntity, double score = 0)
        {
            // 读取到的数据直接强制设置到缓存里
            var id = organizationEntity.Id;
            // 2016-01-06 吉日嘎拉 网点编号不能大小写转换，否则查询就乱套了，不能改变原样
            var code = organizationEntity.Code;
            var name = organizationEntity.Name;
            var simpleSpelling = organizationEntity.SimpleSpelling;
            var enabled = organizationEntity.Enabled.ToString();
            var deletionStateCode = organizationEntity.Deleted.ToString();
            var organization = id + ";" + code + ";" + name + ";" + enabled + ";" + deletionStateCode;
            // 2016-04-11 吉日嘎拉 已经被删除的网点不需要缓存了
            if (organizationEntity.Deleted == 1)
            {
                // organization += " 已删除";
                return;
            }

            var key = string.Empty;

            // 04：结算中心主键缓存数据方法
            var costCenterId = organizationEntity.CostCenterId;
            if (!string.IsNullOrEmpty(costCenterId))
            {
                if (!string.IsNullOrEmpty(code))
                {
                    // 2016-01-06 吉日嘎拉 这里需要小写，提高效率，提高有善度
                    key = "CostCenterId:" + costCenterId + ":" + code.ToLower();
                    CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(name))
                {
                    key = "CostCenterId:" + costCenterId + ":" + name.ToLower();
                    CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(simpleSpelling))
                {
                    key = "CostCenterId:" + costCenterId + ":" + simpleSpelling.ToLower();
                    CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
                }
            }

            // 05：按省缓存数据方法
            var provinceId = organizationEntity.ProvinceId;
            if (!string.IsNullOrEmpty(provinceId?.ToString()))
            {
                if (!string.IsNullOrEmpty(code))
                {
                    // 2016-01-06 吉日嘎拉 这里需要小写，提高效率，提高有善度
                    key = "ProvinceId:" + provinceId + ":" + code.ToLower();
                    CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(name))
                {
                    key = "ProvinceId:" + provinceId + ":" + name.ToLower();
                    CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(simpleSpelling))
                {
                    key = "ProvinceId:" + provinceId + ":" + simpleSpelling.ToLower();
                    CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
                }
            }

            // 06：按市缓存数据方法
            var cityId = organizationEntity.CityId;
            if (!string.IsNullOrEmpty(cityId?.ToString()))
            {
                if (!string.IsNullOrEmpty(code))
                {
                    // 2016-01-06 吉日嘎拉 这里需要小写，提高效率，提高有善度
                    key = "CityId:" + cityId + ":" + code.ToLower();
                    CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(name))
                {
                    key = "CityId:" + cityId + ":" + name.ToLower();
                    CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(simpleSpelling))
                {
                    key = "CityId:" + cityId + ":" + simpleSpelling.ToLower();
                    CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
                }
            }

            // 07：所有下属递归的方式进行快速缓存检索（START WITH CONNECT BY PRIOR） 包括自己
            var startId = organizationEntity.Id.ToString();
            while (!string.IsNullOrEmpty(startId))
            {
                if (!string.IsNullOrEmpty(code))
                {
                    // 2016-01-06 吉日嘎拉 这里需要小写，提高效率，提高有善度
                    key = "StartId:" + startId + ":" + code.ToLower();
                    CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(name))
                {
                    key = "StartId:" + startId + ":" + name.ToLower();
                    CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
                }
                if (!string.IsNullOrEmpty(simpleSpelling))
                {
                    key = "StartId:" + startId + ":" + simpleSpelling.ToLower();
                    CacheUtil.Set(key, organization, new TimeSpan(15, 0, 0, 0));
                }
                // 获取上级的上级，一直进行循环，在缓存里进行计算，提高效率
                startId = GetParentIdByCache(startId);
            }

            // 输出到屏幕看看运行效果如何？心里有个数
            Console.WriteLine(score + " " + organization);
        }

        private static BaseOrganizationEntity GetOrganizationEntity(IDataReader dataReader)
        {
            var entity = new BaseOrganizationEntity
            {
                Id = BaseUtil.ConvertToInt(dataReader[BaseOrganizationEntity.FieldId].ToString()),
                Code = dataReader[BaseOrganizationEntity.FieldCode].ToString(),
                Name = dataReader[BaseOrganizationEntity.FieldName].ToString(),
                SimpleSpelling = dataReader[BaseOrganizationEntity.FieldSimpleSpelling].ToString().ToLower(),
                CostCenterId = dataReader[BaseOrganizationEntity.FieldCostCenterId].ToString(),
                ProvinceId = BaseUtil.ConvertToInt(dataReader[BaseOrganizationEntity.FieldProvinceId].ToString()),
                CompanyId = BaseUtil.ConvertToInt(dataReader[BaseOrganizationEntity.FieldCompanyId].ToString()),
                CityId = BaseUtil.ConvertToInt(dataReader[BaseOrganizationEntity.FieldCityId].ToString()),
                ParentId = BaseUtil.ConvertToInt(dataReader[BaseOrganizationEntity.FieldParentId].ToString()),
                Enabled = BaseUtil.ConvertToInt(dataReader[BaseOrganizationEntity.FieldEnabled], 1),
                Deleted = BaseUtil.ConvertToInt(dataReader[BaseOrganizationEntity.FieldDeleted], 0),
                SortCode = BaseUtil.ConvertToInt(dataReader[BaseOrganizationEntity.FieldSortCode])
            };

            return entity;
        }
    }
}