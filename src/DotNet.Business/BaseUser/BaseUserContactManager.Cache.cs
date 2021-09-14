//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <remarks>
    /// BaseUserContactContact
    /// 用户联系方式管理
    /// 
    /// 修改记录
    /// 
    ///	版本：1.1 2015.06.05    JiRiGaLa    强制重新设置缓存的功能。
    ///	版本：1.0 2015.01.06    JiRiGaLa    选项管理从缓存读取，通过编号显示名称的函数完善。
    ///	
    /// <author>  
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.06.05</date>
    /// </author> 
    /// </remarks>
    public partial class BaseUserContactManager
    {
        /// <summary>
        /// 从缓存中获取实体
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static BaseUserContactEntity GetCacheByKey(string key)
        {
            BaseUserContactEntity result = null;

            if (!string.IsNullOrWhiteSpace(key))
            {
                result = CacheUtil.Get<BaseUserContactEntity>(key);
            }
            return result;
        }

        private static void SetCache(BaseUserContactEntity entity)
        {
            var key = string.Empty;
            if (entity != null && !string.IsNullOrWhiteSpace(entity.Id))
            {
                key = "UserContact:" + entity.Id;
                CacheUtil.Set(key, entity);
            }
        }

        /// <summary>
        /// 从缓存获取获取实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public static BaseUserContactEntity GetObjectByCache(string id)
        {
            BaseUserContactEntity result = null;
            
            if (!string.IsNullOrEmpty(id))
            {
                var key = "UserContact:" + id;
                result = CacheUtil.Cache(key, () => new BaseUserContactManager().GetObject(id), true);
            }

            return result;
        }
    }
}