//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Util;
    using Model;

    /// <summary>
    /// BaseServicesLicenseManager
    /// 参数类
    /// 
    /// 修改记录
    ///		2015.12.25 版本：1.0 JiRiGaLa	创建。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.12.25</date>
    /// </author> 
    /// </summary>
    public partial class BaseServicesLicenseManager : BaseManager
    {
        /// <summary>
        /// 检查用户的 服务访问限制
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="privateKey">私钥</param>
        /// <returns>正确</returns>
        public static bool CheckServiceByCache(string userId, string privateKey)
        {
            // 默认是不成功的，防止出错误
            var result = false;

            // 检查参数的有效性
            if (string.IsNullOrEmpty(userId))
            {
                return result;
            }
            if (string.IsNullOrEmpty(privateKey))
            {
                return result;
            }

            var key = "User:" + userId + ":Service";
            result = CacheUtil.Cache(key, () =>
            {
                //读取数据库
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseServicesLicenseEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseServicesLicenseEntity.FieldDeleted, 0),
                    new KeyValuePair<string, object>(BaseServicesLicenseEntity.FieldUserId, userId),
                    new KeyValuePair<string, object>(BaseServicesLicenseEntity.FieldPrivateKey, privateKey)
                };
                return new BaseServicesLicenseManager().Exists(parameters);
            }, true);
            return result;
        }

        /// <summary>
        /// 缓存预热,强制重新缓存
        /// </summary>
        /// <returns>影响行数</returns>
        public static int CachePreheatingService()
        {
            var result = 0;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseServicesLicenseEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseServicesLicenseEntity.FieldDeleted, 0)
            };

            // 把所有的数据都缓存起来的代码
            var manager = new BaseServicesLicenseManager();

            using (var dataReader = manager.ExecuteReader(parameters))
            {
                while (dataReader.Read())
                {
                    var key = "User:" + dataReader[BaseServicesLicenseEntity.FieldUserId] + ":Service";

                    var privateKey = dataReader[BaseServicesLicenseEntity.FieldPrivateKey].ToString();
                    CacheUtil.Set(key, privateKey);
                    result++;
                    Console.WriteLine(result + " : " + privateKey);
                }
                dataReader.Close();
            }
            return result;
        }

        /// <summary>
        /// 从缓存中去掉
        /// </summary>
        /// <param name="userId">用户主键</param>
        public static bool ResetServiceByCache(string userId)
        {
            var result = false;

            if (!string.IsNullOrEmpty(userId))
            {
                var key = "User:" + userId + ":Service";
                result = CacheUtil.Remove(key);
            }

            return result;
        }

        /// <summary>
        /// 重设 Service 限制，
        /// </summary>
        /// <param name="userId">用户、接口主键</param>
        /// <returns>影响行数</returns>
        public int ResetService(string userId)
        {
            var result = 0;

            // 把缓存里的先清理掉
            ResetServiceByCache(userId);

            // todo 吉日嘎拉 这个操作应该增加个操作日志、谁什么时间，把什么数据删除了？ 把登录日志按操作日志、系统日志来看待？
            var commandText = "UPDATE " + BaseServicesLicenseEntity.TableName
                        + "   SET " + BaseServicesLicenseEntity.FieldDeleted + " = 1 "
                        + "     , " + BaseServicesLicenseEntity.FieldEnabled + " = 0 "
                        + " WHERE " + BaseServicesLicenseEntity.FieldUserId + " = " + DbHelper.GetParameter(BaseServicesLicenseEntity.FieldUserId);

            var dbParameters = new List<IDbDataParameter>();
            dbParameters.Add(DbHelper.MakeParameter(BaseServicesLicenseEntity.FieldUserId, userId));
            result = DbHelper.ExecuteNonQuery(commandText, dbParameters.ToArray());

            return result;
        }
    }
}