//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserLogonManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///		2015.12.08 版本：1.1 JiRiGaLa	缓存预热功能实现。
    ///     
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.12.08</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserLogonManager
    {
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存主键</param>
        /// <returns>用户信息</returns>
        public static BaseUserLogonEntity GetCacheByKey(string key)
        {
            BaseUserLogonEntity result = null;

            if (!string.IsNullOrWhiteSpace(key))
            {
                result = CacheUtil.Get<BaseUserLogonEntity>(key);
            }

            return result;
        }

        /// <summary>
        /// 从缓存中获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static BaseUserLogonEntity GetEntityByCache(string id)
        {
            BaseUserLogonEntity result = null;

            if (!string.IsNullOrEmpty(id))
            {
                var key = "UserLogon:" + id;
                result = CacheUtil.Cache(key, () => GetCacheByKey(key), true);
            }

            return result;
        }

        /// <summary>
        /// 重新设置缓存（重新强制设置缓存）可以提供外部调用的
        /// 20151007 吉日嘎拉，需要在一个连接上进行大量的操作
        /// </summary>
        /// <param name="userId">主键</param>
        /// <returns>用户信息</returns>
        public static BaseUserLogonEntity SetCache(string userId)
        {
            BaseUserLogonEntity result = null;

            var manager = new BaseUserLogonManager();
            result = manager.GetEntityByUserId(userId);
            if (result != null)
            {
                SetCache(result);
            }

            return result;
        }

        /// <summary>
        /// 设置缓存
        /// 20151007 吉日嘎拉，需要在一个连接上进行大量的操作
        /// 20160128 吉日嘎拉，一些空调间的判断。
        /// </summary>
        /// <param name="entity">用户实体</param>
        public static void SetCache(BaseUserLogonEntity entity)
        {
            var key = string.Empty;

            if (entity != null && entity.UserId > 0)
            {
                key = "UserLogon:" + entity.UserId;
                CacheUtil.Set(key, entity);
            }
        }

        /// <summary>
        /// 缓存预热,强制重新缓存
        /// </summary>
        /// <returns>影响行数</returns>
        public static int CachePreheating()
        {
            var result = 0;

            // 把所有的数据都缓存起来的代码
            var manager = new BaseUserLogonManager();
            // 基础用户的登录信息重新缓存起来
            // 2016-04-25 吉日嘎拉 提高性能、读取最少的数据
            var commandText = "SELECT " + BaseUserLogonEntity.FieldUserId + "," + BaseUserLogonEntity.FieldOpenId
                                + " FROM " + BaseUserLogonEntity.CurrentTableName + " T WHERE T." + BaseUserLogonEntity.FieldUserPassword + " IS NOT NULL AND T." + BaseUserLogonEntity.FieldOpenIdTimeoutTime + " IS NOT NULL AND T." + BaseUserLogonEntity.FieldEnabled + " = 1 AND T." + BaseUserLogonEntity.FieldOpenIdTimeoutTime + " - sysdate < 0.5";

            var dataReader = manager.ExecuteReader(commandText);
            if (dataReader != null && !dataReader.IsClosed)
            {
                while (dataReader.Read())
                {
                    var userId = dataReader[BaseUserLogonEntity.FieldUserId].ToString();
                    var openId = dataReader[BaseUserLogonEntity.FieldOpenId].ToString();
                    //暂停Redis缓存 Troy.Cui 2018.07.24，根本没用到Redis，分离DotNet.Business.Web后，就更没必要了
                    CacheUtil.Set(userId, openId);

                    result++;
                    if (result % 500 == 0)
                    {
                        Console.WriteLine(result + " : OpenIdClient User : " + userId);
                    }
                }

                dataReader.Close();
            }

            Console.WriteLine(result + " : 完成 ");

            return result;
        }

    }
}