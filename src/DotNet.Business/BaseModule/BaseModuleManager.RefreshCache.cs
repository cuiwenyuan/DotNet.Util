﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseModuleManager
    /// 模块菜单权限管理
    /// 
    /// 修改纪录
    /// 
    ///		2016.03.01 版本：1.0 JiRiGaLa	分离代码。
    /// 
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2016.03.01</date>
    /// </author> 
    /// </summary>
    public partial class BaseModuleManager : BaseManager
    {
        /// <summary>
        /// 缓存预热,强制重新缓存
        /// </summary>
        /// <returns></returns>
        public static int CachePreheating()
        {
            var result = 0;

            var systemCodes = BaseSystemManager.GetSystemCodes();
            for (var i = 0; i < systemCodes.Length; i++)
            {
                GetEntitiesByCache(systemCodes[i], true);
                result += CachePreheating(systemCodes[i]);
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
            var manager = new BaseModuleManager
            {
                CurrentTableName = systemCode + "Module"
            };
            using (var dataReader = manager.ExecuteReader())
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
        public static int RefreshCache(string systemCode, string moduleId)
        {
            var result = 0;

            // 2016-02-29 吉日嘎拉 强制刷新缓存
            GetObjectByCache(systemCode, moduleId, true);

            return result;
        }
        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <param name="systemCode"></param>
        /// <returns></returns>
        public static int RefreshCache(string systemCode)
        {
            var result = 0;

            var list = GetEntitiesByCache(systemCode, true);
            foreach (var entity in list)
            {
                // 2016-02-29 吉日嘎拉 强制刷新缓存
                GetObjectByCache(systemCode, entity.Id, true);
            }

            return result;
        }
    }
}