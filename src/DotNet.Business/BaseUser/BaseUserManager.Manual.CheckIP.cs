//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

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
    ///     2020.12.08 版本：1.5 Troy.Cui    使用CacheUtil缓存
    ///		2015.12.24 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.12.24</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        /// <summary>
        /// 检查IP地址
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ipAddress"></param>
        /// <param name="autoAdd"></param>
        /// <returns></returns>
        public static bool CheckIpAddressByCache(string userId, string ipAddress, bool autoAdd = false)
        {
            // 判断用户是否限制ip访问，有的是不限制访问的
            var userLogonManager = new BaseUserLogonManager();
            var userLogonEntity = userLogonManager.GetEntityByUserId(userId);
            return CheckIpAddressByCache(userId, userLogonEntity, ipAddress, autoAdd);
        }

        /// <summary>
        /// 检查用户的 IPAddress 绑定是否正常
        ///
        /// 防止重复多读数据？ 
        /// 是否判断正确？
        /// 可以按每个用户缓存？
        /// 若没有就自动化增加？
        /// IP 限制完善？
        /// IP 限制缓存预热？
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="userLogonEntity">用户登录实体</param>
        /// <param name="ipAddress">ip地址</param>
        /// <param name="autoAdd">没有在列表的IP是否自动增加</param>
        /// <returns>正确</returns>
        public static bool CheckIpAddressByCache(string userId, BaseUserLogonEntity userLogonEntity, string ipAddress, bool autoAdd = false)
        {
            // 默认是不成功的，防止出错误
            var result = false;

            // 检查参数的有效性
            if (!ValidateUtil.IsInt(userId))
            {
                return result;
            }

            if (string.IsNullOrEmpty(ipAddress))
            {
                return result;
            }

            int? checkIpAddress = null;
            if (userLogonEntity != null)
            {
                checkIpAddress = userLogonEntity.CheckIpAddress;
            }

            // 若用户是不限制登录的、那就可以返回真的
            if (!checkIpAddress.HasValue || checkIpAddress.Value == 0)
            {
                return true;
            }

            // 提高效率，全小写转换
            ipAddress = ipAddress.ToLower();
            // 这里是处理，多个IP的问题
            var ip = ipAddress.Split(';');

            var key = "IP:" + userId;
            //TODO
            result = true;
            return result;
        }


        /// <summary>
        /// 缓存预热,强制重新缓存
        /// </summary>
        /// <returns>影响行数</returns>
        public static int CachePreheatingIpAddress()
        {
            var result = 0;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseParameterEntity.FieldCategoryCode, "IPAddress"),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldDeleted, 0)
            };

            // 把所有的数据都缓存起来的代码
            var manager = new BaseParameterManager();

            var dataReader = manager.ExecuteReader(parameters);
            if (dataReader != null && !dataReader.IsClosed)
            {
                while (dataReader.Read())
                {
                    var key = "IP:" + dataReader[BaseParameterEntity.FieldParameterId];
                    var ipAddress = dataReader[BaseParameterEntity.FieldParameterContent].ToString().ToLower();
                    CacheUtil.Set(key, ipAddress);
                    result++;
                    Console.WriteLine(result + " : " + ipAddress);
                }

                dataReader.Close();
            }

            return result;
        }

        /// <summary>
        /// 从缓存中去掉IP限制
        /// 防止重复查询数据库？
        /// </summary>
        /// <param name="userId">用户主键</param>
        public static bool ResetIpAddressByCache(string userId)
        {
            var result = false;

            if (ValidateUtil.IsInt(userId))
            {
                var key = "IP:" + userId;
                result = CacheUtil.Remove(key);
            }

            return result;
        }
        /// <summary>
        /// 根据用户编号删除预热的IP
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int CachePreheatingIpAddressByUser(string userId)
        {
            var result = 0;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterId, userId),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldCategoryCode, "IPAddress"),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldDeleted, 0)
            };

            var key = "IP:" + userId;

            var parameterManager = new BaseParameterManager
            {
                SelectFields = BaseParameterEntity.FieldParameterContent
            };
            var dataReader = parameterManager.ExecuteReader(parameters);
            if (dataReader != null && !dataReader.IsClosed)
            {
                while (dataReader.Read())
                {
                    var ipAddress = dataReader[BaseParameterEntity.FieldParameterContent].ToString().ToLower();
                    CacheUtil.Set(key, ipAddress);
                    result++;
                }

                dataReader.Close();
            }

            return result;
        }

        /// <summary>
        /// 重设 IPAddress 限制，
        /// 2015-12-21 吉日嘎拉 历史数据不应该被丢失才对
        /// </summary>
        /// <param name="userId">用户、接口主键</param>
        /// <returns>影响行数</returns>
        public int ResetIpAddress(string userId)
        {
            var result = 0;

            // 把缓存里的先清理掉
            ResetIpAddressByCache(userId);

            //TODO 吉日嘎拉 这个操作应该增加个操作日志、谁什么时间，把什么数据删除了？ 把登录日志按操作日志、系统日志来看待？
            var commandText = "UPDATE " + BaseParameterEntity.CurrentTableName
                        + "   SET " + BaseParameterEntity.FieldDeleted + " = 1 "
                        + "     , " + BaseParameterEntity.FieldEnabled + " = 0 "
                        + " WHERE " + BaseParameterEntity.FieldCategoryCode + " =  " + DbHelper.GetParameter(BaseParameterEntity.FieldCategoryCode)
                        + "       AND " + BaseParameterEntity.FieldParameterId + " = " + DbHelper.GetParameter(BaseParameterEntity.FieldParameterId);

            var dbParameters = new List<IDbDataParameter>
            {
                DbHelper.MakeParameter(BaseParameterEntity.FieldCategoryCode, "IPAddress"),
                DbHelper.MakeParameter(BaseParameterEntity.FieldParameterId, userId)
            };
            result = ExecuteNonQuery(commandText, dbParameters.ToArray());

            return result;
        }
    }
}