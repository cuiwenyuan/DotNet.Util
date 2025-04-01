//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <remarks>
    /// BaseModuleManager
    /// 模块表缓存
    /// 
    /// 修改记录
    ///
    ///     2020.12.08 版本：1.5 Troy.Cui 使用CacheUtil缓存
    ///		2016.02.26 版本：1.4 JiRiGaLa 独立缓存库。
    ///		2015.12.03 版本：1.3 JiRiGaLa List读写方法改进。
    ///     2015.07.30 版本：1.0 JiRiGaLa 创建。
    ///	
    /// <author>  
    ///		<name>Troy.Cui</name>
    ///		<date>2016.02.26</date>
    /// </author> 
    /// </remarks>
    public partial class BaseModuleManager
    {
        #region 删除缓存

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <returns></returns>
        public override bool RemoveCache()
        {
            var result = false;
            var cacheKey = "Dt." + CurrentTableName;
            var cacheKeyTree = "Dt." + CurrentTableName;
            if (UserInfo != null)
            {
                //cacheKey += "." + UserInfo.CompanyId;
                cacheKeyTree = "Dt." + UserInfo.SystemCode + ".ModuleTree";
            }

            CacheUtil.Remove(cacheKeyTree);
            result = CacheUtil.Remove(cacheKey);
            return result;
        }
        #endregion

        #region public override bool RemoveCache(string key) 删除缓存
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">键</param>
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

        #endregion

        #region public static BaseModuleEntity GetCacheByKey(string key) 根据Key获取缓存
        /// <summary>
        /// 根据Key获取缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static BaseModuleEntity GetCacheByKey(string key)
        {
            BaseModuleEntity result = null;

            if (!string.IsNullOrWhiteSpace(key))
            {
                result = CacheUtil.Get<BaseModuleEntity>(key);
            }

            return result;
        }

        #endregion

        #region private static void SetCache(string systemCode, BaseModuleEntity entity) 设置缓存
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="entity"></param>
        private static void SetCache(string systemCode, BaseModuleEntity entity)
        {
            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = "Base";
            }

            if (entity != null && !string.IsNullOrEmpty(entity.Id.ToString()))
            {
                var key = GetModuleTableName(systemCode) + "." + entity.Id;
                CacheUtil.Set<BaseModuleEntity>(key, entity);

                key = GetModuleTableName(systemCode) + "." + entity.Code;
                CacheUtil.Set<BaseModuleEntity>(key, entity);
            }
        }

        #endregion

        #region private static void SetListCache(string key, List<BaseModuleEntity> list) 设置List缓存
        /// <summary>
        /// 设置List缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="list"></param>
        private static void SetListCache(string key, List<BaseModuleEntity> list)
        {
            if (!string.IsNullOrWhiteSpace(key) && list != null)
            {
                CacheUtil.Set<List<BaseModuleEntity>>(key, list);
            }
        }
        #endregion

        #region private static List<BaseModuleEntity> GetListCache(string key) 获取List缓存

        /// <summary>
        /// 获取List缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        private static List<BaseModuleEntity> GetListCache(string key)
        {
            List<BaseModuleEntity> result = null;

            if (!string.IsNullOrWhiteSpace(key))
            {
                result = CacheUtil.Get<List<BaseModuleEntity>>(key);
            }

            return result;
        }

        #endregion

        /// <summary>
        /// 缓存预热,强制重新缓存
        /// </summary>
        /// <returns></returns>
        public int CachePreheating()
        {
            var result = 0;

            var systemCodes = BaseSystemManager.GetSystemCodes();
            foreach (var entity in systemCodes)
            {
                GetEntitiesByCache(entity.ItemKey, true);
                result += CachePreheating(entity.ItemKey);
            }

            return result;
        }

        /// <summary>
        /// 缓存预热,强制重新缓存
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <returns>影响行数</returns>
        public static int CachePreheating(string systemCode)
        {
            var result = 0;

            // 把所有的组织机构都缓存起来的代码
            var manager = new BaseModuleManager(GetModuleTableName(systemCode));
            var dataReader = manager.ExecuteReader();
            if (dataReader != null && !dataReader.IsClosed)
            {
                while (dataReader.Read())
                {
                    var entity = BaseEntity.Create<BaseModuleEntity>(dataReader, false);
                    if (entity != null)
                    {
                        SetCache(systemCode, entity);
                        result++;
                        System.Console.WriteLine(result + " : " + entity.Code);
                    }
                }

                dataReader.Close();
            }

            return result;
        }
        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public int RefreshCache(string systemCode, string moduleId)
        {
            var result = 0;

            // 2016-02-29 吉日嘎拉 强制刷新缓存
            GetEntityByCache(systemCode, moduleId, true);

            return result;
        }
        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <param name="systemCode"></param>
        /// <returns></returns>
        public int RefreshCache(string systemCode)
        {
            var result = 0;

            var list = new BaseModuleManager().GetEntitiesByCache(systemCode, true);
            foreach (var entity in list)
            {
                // 2016-02-29 吉日嘎拉 强制刷新缓存
                GetEntityByCache(systemCode, entity.Id.ToString(), true);
            }

            return result;
        }
    }
}