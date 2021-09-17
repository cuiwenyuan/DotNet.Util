//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserManager
    /// 用户管理
    /// 
    /// 修改记录
    ///
    ///     2020.12.08 版本：1.5 Troy.Cui 使用CacheUtil缓存
    ///		2016.01.18 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.01.18</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        /// <summary>
        /// 获取最近一个月登录系统的用户，从缓存服务器，登录接口获取，这样僵尸账户就会可以过滤了。
        /// </summary>
        /// <returns>影响行数</returns>
        public int GetActiveUsers()
        {
            var result = 0;

            // 先把用户来源都修改为 空。
            var commandText = "UPDATE " + BaseUserEntity.TableName + " SET " + BaseUserEntity.FieldUserFrom + " = NULL WHERE " + BaseUserEntity.FieldUserFrom + " = 'Base'";
            result = DbHelper.ExecuteNonQuery(commandText);
            // 通过中天登录的，都设置为 "Base";
            var key = string.Empty;
            result = 0;
            var i = 0;
            
                key = "LogOn:UserCount:" + DateTime.Now.ToString("yyyy-MM");
                var list = CacheUtil.Get<List<int>>(key);
                foreach (var id in list)
                {
                    i++;
                    commandText = "UPDATE " + BaseUserEntity.TableName + " SET " + BaseUserEntity.FieldUserFrom + " = 'Base' WHERE " + BaseUserEntity.FieldId + " = " + id;
                    result += DbHelper.ExecuteNonQuery(commandText);
                    Console.WriteLine("Count:" + i + "/" + list.Count + " Id:" + id);
                }

                return result;
        }

        /// <summary>
        /// 登录次数统计功能（通过缓存高速实时统计、统计键值还是有些问题、进行了改进）
        /// 2016-03-28 吉日嘎拉
        /// </summary>
        /// <param name="userInfo">用户</param>
        public static void LogOnStatistics(BaseUserInfo userInfo)
        {
            if (!BaseSystemInfo.LogOnStatistics)
            {
                return;
            }
            var key = string.Empty;
            // 缓存过期设为1年
            var cacheTime = TimeSpan.FromMilliseconds(31536000000);
            // 1: 登录次数统计
            // 1-1：当月的登录人数增加一，
            key = "LogOn:Count:" + DateTime.Now.ToString("yyyy-MM");
            CacheUtil.Set(key, CacheUtil.Get<int>(key) + 1);
            // 1-2：当天的登录人数增加一，
            key = "LogOn:Count:" + DateTime.Now.ToString("yyyy-MM-dd");
            CacheUtil.Set(key, CacheUtil.Get<int>(key) + 1);
            // 1-3：当前小时的登录人数加一。
            key = "LogOn:Count:" + DateTime.Now.ToString("yyyy-MM-dd:HH");
            CacheUtil.Set(key, CacheUtil.Get<int>(key) + 1);

            // 2：不重复的登录统计
            // 2-1：当月的不重复登录人数增加一。
            key = "LogOn:UserCount:" + DateTime.Now.ToString("yyyy-MM");
            CacheUtil.Set(key, CacheUtil.Get<HashSet<string>>(key).Add(userInfo.Id));
            // 2-2：当天的不重复登录人数增加一。
            key = "LogOn:UserCount:" + DateTime.Now.ToString("yyyy-MM-dd");
            CacheUtil.Set(key, CacheUtil.Get<HashSet<string>>(key).Add(userInfo.Id));
            // 2-3：当前小时的不重复登录人数增加一。
            key = "LogOn:UserCount:" + DateTime.Now.ToString("yyyy-MM-dd:HH");
            CacheUtil.Set(key, CacheUtil.Get<HashSet<string>>(key).Add(userInfo.Id));

            // 3：不重复的网点登录统计
            // 3-1：每天有多少个公司在登录系统
            key = "LogOn:CompanyCount:" + DateTime.Now.ToString("yyyy-MM-dd");
            CacheUtil.Set(key, CacheUtil.Get<HashSet<string>>(key).Add(userInfo.CompanyId));
        }
    }
}