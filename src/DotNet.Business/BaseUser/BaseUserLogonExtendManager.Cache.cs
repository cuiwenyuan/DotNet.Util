//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Globalization;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserLogonExtendManager
    /// 用户登录提醒 缓存
    /// 
    /// 修改记录
    /// 
    /// 2015-01-23 版本：1.0 SongBiao 创建文件。
    /// 
    /// <author>
    ///     <name>SongBiao</name>
    ///     <date>2015-01-23</date>
    /// </author>
    /// </summary>
    public partial class BaseUserLogonExtendManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 缓存键前缀
        /// </summary>
        private static string _cacheKeyPrefix = "BaseUserLogonRemind";

        /// <summary>
        /// 根据key值获取缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static BaseUserLogonExtendEntity GetCache(string key)
        {
            BaseUserLogonExtendEntity result = null;
            if (!string.IsNullOrWhiteSpace(key))
            {
                result = CacheUtil.Get<BaseUserLogonExtendEntity>(key);
            }
            return result;
        }

        /// <summary>
        /// 将对象设置到缓存中
        /// </summary>
        /// <param name="entity"></param>
        public static void SetCache(BaseUserLogonExtendEntity entity)
        {
            var key = string.Empty;
            if (entity != null && !string.IsNullOrWhiteSpace(entity.Id.ToString()))
            {
                key = _cacheKeyPrefix + entity.Id;
                CacheUtil.Set<BaseUserLogonExtendEntity>(key, entity);
            }
        }

        /// <summary>
        /// 根据主键从缓存中获取获取实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public static BaseUserLogonExtendEntity GetEntityByCache(string id)
        {
            BaseUserLogonExtendEntity result = null;
            var cacheKey = _cacheKeyPrefix;
            if (!string.IsNullOrEmpty(id))
            {
                cacheKey = cacheKey + id;
            }

            result = CacheUtil.Cache(cacheKey, () => new BaseUserLogonExtendManager().GetEntity(id), true);

            return result;
        }
    }
}
