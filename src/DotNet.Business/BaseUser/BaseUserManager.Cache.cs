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
    /// BaseUserManager
    /// 用户缓存管理
    /// 
    /// 修改记录
    ///
    ///     2020.12.08 版本：1.5 Troy.Cui   使用CacheUtil缓存
    ///	版本：1.3 2016.01.18    JiRiGaLa    读写分离，配置文件优化。
    ///	版本：1.2 2016.01.07    JiRiGaLa    缓存服务器，读写分离。
    ///	版本：1.1 2015.06.15    JiRiGaLa    增加强制刷新缓存的功能。
    ///	版本：1.0 2015.01.06    JiRiGaLa    缓存优化。
    ///	
    /// <author>  
    ///		<name>Troy.Cui</name>
    ///		<date>2016.01.18</date>
    /// </author> 
    /// </remarks>
    public partial class BaseUserManager
    {
        /// <summary>
        /// 获取缓存
        /// 20151007 吉日嘎拉，需要在一个连接上进行大量的操作
        /// </summary>
        /// <param name="key">缓存主键</param>
        /// <returns>用户信息</returns>
        public static BaseUserEntity GetCacheByKey(string key)
        {
            BaseUserEntity result = null;

            if (!string.IsNullOrWhiteSpace(key))
            {
                result = CacheUtil.Get<BaseUserEntity>(key);
            }

            return result;
        }

        /// <summary>
        /// 通过唯一用户名从Reids中获取
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>
        public static BaseUserEntity GetEntityByNickNameByCache(string nickName)
        {
            // 2016-01-25 黄斌 添加, 从缓存中 通过唯一用户名获取
            BaseUserEntity result = null;
            if (string.IsNullOrEmpty(nickName))
            {
                return result;
            }
            var key = "User:ByNickName:" + nickName.ToLower();
            result = CacheUtil.Cache(key, () => new BaseUserManager().GetEntityByNickName(nickName), true);
            return result;
        }

        /// <summary>
        /// 设置缓存
        /// 20151007 吉日嘎拉，需要在一个连接上进行大量的操作
        /// 20160128 吉日嘎拉，一些空调间的判断。
        /// </summary>
        /// <param name="entity">用户实体</param>
        public static void SetCache(BaseUserEntity entity)
        {
            var key = string.Empty;

            if (entity != null && entity.Id > 0)
            {
                key = "User:" + entity.Id;
                CacheUtil.Set<BaseUserEntity>(key, entity);

                if (!string.IsNullOrEmpty(entity.NickName))
                {
                    key = "User:ByNickName:" + entity.NickName.ToLower();
                    CacheUtil.Set<string>(key, entity.Id.ToString());
                }

                if (!string.IsNullOrEmpty(entity.Code))
                {
                    key = "User:ByCode:" + entity.Code;
                    CacheUtil.Set<string>(key, entity.Id.ToString());

                    key = "User:ByCompanyId:ByCode" + entity.CompanyId + ":" + entity.Code;
                    CacheUtil.Set<string>(key, entity.Id.ToString());
                }

                var companyCode = BaseOrganizationManager.GetCodeByCache(entity.CompanyId.ToString());
                if (!string.IsNullOrEmpty(companyCode))
                {
                    key = "User:ByCompanyCode:ByCode" + companyCode + ":" + entity.Code;
                    CacheUtil.Set<string>(key, entity.Id.ToString());
                }

                Console.WriteLine(entity.Id + " : " + entity.RealName);
            }
        }

        /// <summary>
        /// 从缓存获取获取实体
        /// 20151007 吉日嘎拉，需要在一个连接上进行大量的操作
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="refreshCache">刷新缓存</param>
        /// <returns>实体</returns>
        public static BaseUserEntity GetEntityByCache(string id, bool refreshCache = false)
        {
            BaseUserEntity result = null;

            if (!ValidateUtil.IsInt(id))
            {
                return result;
            }

            var key = "User:" + id;
            result = CacheUtil.Cache(key, () => new BaseUserManager().GetEntity(id), true, refreshCache);

            return result;
        }

        /// <summary>
        /// 从缓存中获取用户实体
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public static BaseUserEntity GetEntityByCodeByCache(string userCode)
        {
            BaseUserEntity result = null;

            var key = "User:ByCode:" + userCode;
            result = CacheUtil.Cache(key, () => new BaseUserManager().GetEntityByCode(userCode), true);
            return result;
        }

        /// <summary>
        /// 从缓存中获取用户编号
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public static string GetIdByCodeByCache(string userCode)
        {
            string result = null;

            var userEntity = GetEntityByCodeByCache(userCode);
            if (userEntity != null)
            {
                result = userEntity.Id.ToString();
            }

            return result;
        }

        /// <summary>
        /// 根据公司编号和用户编码获取用户实体
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public static BaseUserEntity GetEntityByCompanyIdByCodeByCache(string companyId, string userCode)
        {
            BaseUserEntity result = null;
            // 检查参数有效性
            if (string.IsNullOrWhiteSpace(companyId) || string.IsNullOrWhiteSpace(userCode))
            {
                return result;
            }
            var key = "User:ByCompanyId:ByCode" + companyId + ":" + userCode;
            result = CacheUtil.Cache(key, () => new BaseUserManager().GetEntityByCompanyIdByCode(companyId, userCode), true);

            return result;
        }

        /// <summary>
        /// 用户是否在公司里
        /// </summary>
        /// <param name="companyCode">公司编号</param>
        /// <param name="userCode">用户编号</param>
        /// <returns></returns>
        public static bool IsInOrganizationByCode(string companyCode, string userCode)
        {
            // 返回值
            var result = false;

            // 检查参数有效性
            if (string.IsNullOrWhiteSpace(companyCode) || string.IsNullOrWhiteSpace(userCode))
            {
                return result;
            }
            var key = "User:ByCompanyCode:ByCode" + companyCode + ":" + userCode;
            result = CacheUtil.Cache(key, () => new BaseUserManager().GetEntityByCompanyCodeByCode(companyCode, userCode) != null, true);

            return result;
        }

        /// <summary>
        /// 通过 openId 获取用户信息
        /// </summary>
        /// <param name="openId">唯一键</param>
        /// <returns>用户实体</returns>
        public static BaseUserEntity GetEntityByOpenIdByCache(string openId)
        {
            BaseUserEntity result = null;
            var userId = string.Empty;

            var key = "OpenId";
            if (!string.IsNullOrWhiteSpace(openId))
            {

                key += openId;

                result = CacheUtil.Cache(key, () =>
                {
                    // 到数据库里查一次
                    userId = new BaseUserLogonManager().GetIdByOpenId(openId);
                    if (!string.IsNullOrWhiteSpace(userId))
                    {
                        return new BaseUserManager().GetEntity(userId);
                    }
                    else
                    {
                        return null;
                    }
                }, true);



            }
            return result;
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="entity">用户实体</param>
        public static void CachePreheatingSpelling(BaseUserEntity entity)
        {
            double score = entity.SortCode;
            CachePreheatingSpelling(entity, score);
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="userEntity"></param>
        /// <param name="score"></param>
        public static void CachePreheatingSpelling(BaseUserEntity userEntity, double score = 0)
        {
            // 读取到的数据直接强制设置到缓存里
            var id = userEntity.Id;
            // 2016-01-06 吉日嘎拉 网点编号不能大小写转换，否则查询就乱套了，不能改变原样
            var code = userEntity.Code;
            var realName = userEntity.RealName;
            //string simpleSpelling = userEntity.SimpleSpelling.ToLower();
            var simpleSpelling = userEntity.SimpleSpelling;
            if (!string.IsNullOrEmpty(simpleSpelling))
            {
                simpleSpelling = simpleSpelling.ToLower();
            }

            var user = id + ";" + code + ";" + realName;

            if (userEntity.Enabled == 0)
            {
                // user += " 失效";
            }


            var key = string.Empty;

            code = code.Replace(" ", "");

            // 01：按网点进行缓存
            var companyId = userEntity.CompanyId;
            if (companyId > 0)
            {
                for (var i = 1; i <= code.Length; i++)
                {
                    // 2016-01-06 吉日嘎拉 这里需要小写，提高效率，提高有善度
                    key = "User:CompanyId:" + companyId + ":" + code.Substring(0, i).ToLower();
                    //redisClient.AddItemToSortedSet(key, user, score);
                    //redisClient.ExpireEntryAt(key, DateTime.Now.AddDays(15));
                }
                for (var i = 1; i <= realName.Length; i++)
                {
                    key = "User:CompanyId:" + companyId + ":" + realName.Substring(0, i).ToLower();
                    //redisClient.AddItemToSortedSet(key, user, score);
                    //redisClient.ExpireEntryAt(key, DateTime.Now.AddDays(15));
                }
                for (var i = 1; i <= simpleSpelling.Length; i++)
                {
                    key = "User:CompanyId:" + companyId + ":" + simpleSpelling.Substring(0, i);
                    //redisClient.AddItemToSortedSet(key, user, score);
                    //redisClient.ExpireEntryAt(key, DateTime.Now.AddDays(15));
                }
            }

            // 02：按用户编号进行缓存
            if (!string.IsNullOrEmpty(code.Trim()))
            {
                for (var i = 6; i <= code.Length; i++)
                {
                    // 2016-01-06 吉日嘎拉 这里需要小写，提高效率，提高有善度
                    key = "User:CodeOrRealName:" + code.Substring(0, i).ToLower();
                    //redisClient.AddItemToSortedSet(key, user, score);
                    //redisClient.ExpireEntryAt(key, DateTime.Now.AddDays(15));
                }
            }
            if (!string.IsNullOrEmpty(realName.Trim()))
            {
                key = "User:CodeOrRealName:" + realName.ToLower();
                //redisClient.AddItemToSortedSet(key, user, score);
                //redisClient.ExpireEntryAt(key, DateTime.Now.AddDays(15));
            }

            // 输出到屏幕看看运行效果如何？心里有个数
            Console.WriteLine(score + " " + user);
        }
    }
}