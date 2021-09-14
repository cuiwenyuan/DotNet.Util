﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserLogOnManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///		2015.12.08 版本：1.1 JiRiGaLa	缓存预热功能实现。
    ///     
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.12.08</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserLogOnManager
    {
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存主键</param>
        /// <returns>用户信息</returns>
        public static BaseUserLogOnEntity GetCacheByKey(string key)
        {
            BaseUserLogOnEntity result = null;

            if (!string.IsNullOrWhiteSpace(key))
            {
                result = CacheUtil.Get<BaseUserLogOnEntity>(key);
            }

            return result;
        }

        /// <summary>
        /// 从缓存中获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static BaseUserLogOnEntity GetObjectByCache(string id)
        {
            BaseUserLogOnEntity result = null;

            if (!string.IsNullOrEmpty(id))
            {
                var key = "UserLogOn:" + id;
                result = CacheUtil.Cache(key, () => GetCacheByKey(key), true);
            }

            return result;
        }

        /// <summary>
        /// 重新设置缓存（重新强制设置缓存）可以提供外部调用的
        /// 20151007 吉日嘎拉，需要在一个连接上进行大量的操作
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>用户信息</returns>
        public static BaseUserLogOnEntity SetCache(string id)
        {
            BaseUserLogOnEntity result = null;

            var manager = new BaseUserLogOnManager();
            result = manager.GetObject(id);
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
        public static void SetCache(BaseUserLogOnEntity entity)
        {
            var key = string.Empty;

            if (entity != null && !string.IsNullOrWhiteSpace(entity.Id))
            {
                key = "UserLogOn:" + entity.Id;
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
            var manager = new BaseUserLogOnManager();
            // 基础用户的登录信息重新缓存起来
            // 2016-04-25 吉日嘎拉 提高性能、读取最少的数据
            var commandText = "SELECT " + BaseUserLogOnEntity.FieldId
                                        + "," + BaseUserLogOnEntity.FieldOpenId
                                + " FROM " + BaseUserLogOnEntity.TableName + " T WHERE T.userpassword IS NOT NULL AND T.openidtimeout IS NOT NULL AND T.enabled = 1 AND T.openidtimeout - sysdate < 0.5";

            using (var dataReader = manager.ExecuteReader(commandText))
            {
                while (dataReader.Read())
                {
                    var id = dataReader[BaseUserLogOnEntity.FieldId].ToString();
                    var openId = dataReader[BaseUserLogOnEntity.FieldOpenId].ToString();
                    //暂停Redis缓存 Troy.Cui 2018.07.24，根本没用到Redis，分离DotNet.Business.Web后，就更没必要了
                    CacheUtil.Set(id, openId);

                    result++;
                    if (result % 500 == 0)
                    {
                        Console.WriteLine(result + " : OpenIdClient User : " + id);
                    }
                }
                dataReader.Close();
                Console.WriteLine(result + " : 完成 ");
            }

            return result;
        }

    }
}