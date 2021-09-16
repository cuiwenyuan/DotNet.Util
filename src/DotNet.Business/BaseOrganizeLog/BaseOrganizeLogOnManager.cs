﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseOrganizeLogOnManager
    /// 系统网点登录信息
    ///
    /// 修改记录
    ///
    ///		2016-04-01 版本：1.1 JiRiGaLa LogOnStatistics 方法进行细化改进。
    ///		2016-03-29 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016-03-29</date>
    /// </author>
    /// </summary>
    public partial class BaseOrganizeLogOnManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 从数据库加载上来？缓存预热的机制
        /// </summary>
        /// <returns></returns>
        public static int CachePreheating()
        {
            var result = 0;
            // 把所有的数据都缓存起来的代码
            var manager = new BaseOrganizeLogOnManager();
            using (var dataReader = manager.ExecuteReader(0, BaseOrganizeLogOnEntity.FieldId))
            {
                while (dataReader.Read())
                {
                    var entity = BaseEntity.Create<BaseOrganizeLogOnEntity>(dataReader, false);
                    SetCache(entity);
                    result++;
                }
                dataReader.Close();
            }

            return result;
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="entity">登录信息</param>
        public static void SetCache(BaseOrganizeLogOnEntity entity)
        {
            var key = string.Empty;

            if (entity != null && !string.IsNullOrWhiteSpace(entity.Id))
            {
                key = "OrganizeLogOn:" + entity.Id;
                CacheUtil.Set(key, entity);
            }
        }

        /// <summary>
        /// 获取缓存的网点登录信息
        /// </summary>
        /// <param name="companyId">网点Id</param>
        public static BaseOrganizeLogOnEntity GetCache(string companyId)
        {
            BaseOrganizeLogOnEntity result = null;
            if (!string.IsNullOrEmpty(companyId))
            {
                result = CacheUtil.Get<BaseOrganizeLogOnEntity>("OrganizeLogOn:" + companyId);
            }
            return result;
        }
    }
}