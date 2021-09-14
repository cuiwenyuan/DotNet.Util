//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <remarks>
    /// BaseDepartmentManager
    /// 部门管理缓存类
    /// 
    /// 修改记录
    /// 
    ///	版本：1.0 2016.01.18  SongBiao 增加承包区缓存预热。
    ///	版本：1.1 2016.01.04  JiRiGaLa 写入主键的方式进行改进。
    ///	版本：1.0 2015.10.23  SongBiao GetCache 方法实现。
    ///	
    /// <author>  
    ///		<name>SongBiao</name>
    ///		<date>2016.01.04</date>
    /// </author> 
    /// </remarks>
    public partial class BaseDepartmentManager
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
                CacheUtil.Remove(key);
            }

            return result;
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static BaseDepartmentEntity GetCacheByKey(string key)
        {
            BaseDepartmentEntity result = null;

            if (!string.IsNullOrWhiteSpace(key))
            {
                CacheUtil.Get<BaseDepartmentEntity>(key);
            }

            return result;
        }

        /// <summary>
        /// 设置缓存
        /// 这里缓存的数据容量控制了一下，只保存Id，会节约内存空间
        /// </summary>
        /// <param name="entity">实体</param>
        public static void SetCache(BaseDepartmentEntity entity)
        {
            if (entity != null && entity.Id != null)
            {
                var key = string.Empty;

                key = "D:" + entity.Id;
                CacheUtil.Set(key, entity);
                key = "DBN:" + entity.CompanyId + ":" + entity.FullName;
                CacheUtil.Set(key, entity.Id.ToString());

                key = "DBC:" + entity.Code;
                CacheUtil.Set(key, entity.Id.ToString());
            }
        }

        /// <summary>
        /// 从缓存获取实体
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static BaseDepartmentEntity GetObjectByCodeByCache(string code)
        {
            BaseDepartmentEntity result = null;

            if (!string.IsNullOrEmpty(code))
            {
                result = CacheUtil.Cache(code, () => new BaseDepartmentManager().GetObjectByCode(code));
            }
            return result;
        }

        /// <summary>
        /// 从缓存获取获取实体
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <param name="fullName">部门名称</param>
        /// <returns>实体</returns>
        public static BaseDepartmentEntity GetObjectByNameByCache(string companyId, string fullName)
        {
            BaseDepartmentEntity result = null;

            if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(fullName))
            {
                var key = "DBN:" + companyId + ":" + fullName;
                result = CacheUtil.Cache(key, () => new BaseDepartmentManager().GetObjectByName(companyId, fullName));
            }

            return result;
        }

        /// <summary>
        /// 按实体缓存承包区 宋彪
        /// </summary>
        /// <param name="score"></param>
        /// <param name="departmentEntity"></param>
        public static void CacheContractAreaPreheatingSpelling(BaseDepartmentEntity departmentEntity, double score = 0)
        {
            // 承包区主管id字段不可为空，对应的是用户的Id
            if (!string.IsNullOrWhiteSpace(departmentEntity.ManagerId))
            {
                var contractArea = departmentEntity.ManagerId + ";" + departmentEntity.Code + ";" + departmentEntity.FullName;
                var key = string.Empty;
                // 01：所有承包区查询的缓存数据方法  编号是按网点code生成 至少输入4个编号才返回查询结果
                for (var i = 4; i <= departmentEntity.Code.Length; i++)
                {
                    key = "ContractArea:" + departmentEntity.CompanyId + ":" + departmentEntity.Code.Substring(0, i).ToLower();
                    CacheUtil.Set(key, contractArea);
                }

                // 02：按承包区名字查询的缓存
                for (var i = 1; i <= departmentEntity.FullName.Length; i++)
                {
                    key = "ContractArea:" + departmentEntity.CompanyId + ":" + departmentEntity.FullName.Substring(0, i).ToLower();
                    CacheUtil.Set(key, contractArea);
                }

            }
        }
    }
}