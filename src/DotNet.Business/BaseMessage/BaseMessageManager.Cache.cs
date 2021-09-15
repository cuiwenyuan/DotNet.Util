//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <remarks>
    /// BaseMessageManager
    /// 消息的缓存服务
    /// 
    /// 修改记录
    /// 
    ///	版本：1.0 2015.09.26  JiRiGaLa    消息的缓存服务。
    ///	
    /// <author>  
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.09.26</date>
    /// </author> 
    /// </remarks>
    public partial class BaseMessageManager
    {
        /// <summary>
        /// 缓存预热,强制重新缓存
        /// </summary>
        /// <returns>影响行数</returns>
        public static int CachePreheating()
        {
            var result = 0;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseMessageEntity.FieldCategoryCode, MessageCategory.Receiver.ToString()),
                new KeyValuePair<string, object>(BaseMessageEntity.FieldIsNew, (int)MessageStateCode.New),
                new KeyValuePair<string, object>(BaseMessageEntity.FieldDeleted, 0)
            };

            // 2015-09-27 吉日嘎拉 把所有的未阅读的消息都缓存起来的代码。
            var manager = new BaseMessageManager();
            using (var dataReader = manager.ExecuteReader(parameters, BaseMessageEntity.FieldCreateTime))
            {
                while (dataReader.Read())
                {
                    var entity = BaseEntity.Create<BaseMessageEntity>(dataReader, false);
                    // 2015-09-30 吉日嘎拉 两个月以上的信息，意义不大了，可以考虑不缓存了
                    if (entity != null)
                    {
                        manager.CacheProcessing(entity);
                        result++;
                        // System.Console.WriteLine(result.ToString() + " : " + entity.Contents);
                    }
                }
                dataReader.Close();
            }

            return result;
        }

        /// <summary>
        /// 从缓存中获取实体
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static BaseMessageEntity GetCacheByKey(string key)
        {
            BaseMessageEntity result = null;

            if (!string.IsNullOrWhiteSpace(key))
            {
                result = CacheUtil.Get<BaseMessageEntity>(key);
            }

            return result;
        }

        private static void SetCache(BaseMessageEntity entity)
        {
            if (entity != null && !string.IsNullOrWhiteSpace(entity.Id))
            {
                // string key = string.Empty;
                var key = "m";
                // 默认缓存三个月，三个月不看的消息，意义也不大了，也浪费内存空间了。
                // key = "Message" + entity.Id;
                // 2015-09-27 吉日嘎拉 用简短的Key，这样效率高一些，节约内存
                key += entity.Id;
                CacheUtil.Set(key, entity);
            }
        }

        /// <summary>
        /// 从缓存获取获取实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public static BaseMessageEntity GetEntityByCache(string id)
        {
            BaseMessageEntity result = null;
            var cacheKey = "m";
            if (!string.IsNullOrEmpty(id))
            {
                cacheKey += id;
                result = CacheUtil.Cache(cacheKey, () => new BaseMessageManager().GetEntity(id), true);
            }
            return result;
        }

        /// <summary>
        /// 把未阅读的消息，进行缓存处理
        /// 2015-10-08 吉日嘎拉 优化代码
        /// </summary>
        /// <param name="entity">消息</param>
        /// <param name="expireAt"></param>
        public void CacheProcessing(BaseMessageEntity entity, DateTime? expireAt = null)
        {
            // 需要把消息本身放一份在缓存服务器里
            if (entity != null)
            {

                SetCache(entity);

                if (!string.IsNullOrEmpty(entity.ReceiverId))
                {
                    // 把消息的主键放在有序集合里, 尽量存放小数字，不要太长了
                    CacheUtil.Set(entity.ReceiverId, entity.Id);
                }

                if (!string.IsNullOrEmpty(entity.CreateUserId))
                {
                    // 设置一个最近联络人标注，把最近联系人放在有序集合里(过期时间)
                    // 设置过期时间，防止长时间不能释放
                    CacheUtil.Set("r" + entity.CreateUserId, entity.ReceiverId);
                }

                if (!string.IsNullOrEmpty(entity.ReceiverId))
                {
                    CacheUtil.Set("r" + entity.ReceiverId, entity.CreateUserId);
                }

                // 把多余的数据删除掉，没必要放太多的历史数据，最近联系人列表里
            }
        }
    }
}