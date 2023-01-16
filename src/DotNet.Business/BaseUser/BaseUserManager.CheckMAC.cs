//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
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
    ///		2016.05.09 版本：2.0 JiRiGaLa	用统一的登录缓存。
    ///		2015.12.21 版本：1.0 JiRiGaLa	进行代码分离、缓存优化、这个可以降低I/O的。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.05.09</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        /// <summary>
        /// 检查用户的 macAddress 绑定是否正常
        ///
        /// 防止重复多读数据？ 
        /// 是否判断正确？
        /// 可以按每个用户缓存？
        /// 若没有就自动化增加？
        /// mac 限制完善？
        /// mac 限制缓存预热？
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="macAddress">硬件地址</param>
        /// <returns>正确</returns>
        public static bool CheckMacAddressByCache(string userId, string macAddress)
        {
            // 默认是不成功的，防止出错误
            var result = false;

            // 检查参数的有效性
            if (!ValidateUtil.IsInt(userId))
            {
                return result;
            }
            if (string.IsNullOrEmpty(macAddress))
            {
                return result;
            }

            // 提高效率，全小写转换
            macAddress = macAddress.ToLower();

            // 这里是处理，多个mac的问题
            var mac = macAddress.Split(';');
            ////Troy.Cui 2016.12.28
            //if (BaseSystemInfo.RedisEnabled)
            //{
            //    using (var redisClient = PooledRedisHelper.GetOpenIdClient())
            //    {
            //        var key = "MAC:" + userId;

            //        // 若是缓存里过期了？
            //        if (!redisClient.ContainsKey(key))
            //        {
            //            // 重新缓存用户的限制数据
            //            if (CachePreheatingMacAddressByUser(redisClient, userId) == 0)
            //            {
            //                // 若没有设置mac限制，需要把限制都自动加上来。
            //                // 没有加到数据的，就是表明是新增加的用户、第一次登录的用户
            //                var parameterManager = new BaseParameterManager();
            //                for (var i = 0; i < mac.Length; i++)
            //                {
            //                    if (!string.IsNullOrEmpty(mac[i]))
            //                    {
            //                        // 把收集过来的mac地址需要保存起来
            //                        var parameterEntity = new BaseParameterEntity();
            //                        parameterEntity.Id = Guid.NewGuid().ToString("N");
            //                        parameterEntity.CategoryCode = "MacAddress";
            //                        parameterEntity.ParameterCode = "Single";
            //                        parameterEntity.ParameterId = userId;
            //                        // 这里之际保存小写、就效率也高，省事了
            //                        parameterEntity.ParameterContent = mac[i].Trim();
            //                        parameterManager.Add(parameterEntity);
            //                    }
            //                }
            //                result = true;
            //            }
            //        }

            //        // 若还是没有？表示是新增的
            //        if (redisClient.ContainsKey(key))
            //        {
            //            // 若已经存在，就需要进行缓存里的判断？
            //            // 这里要提高效率，不能反复打开缓存
            //            for (var i = 0; i < mac.Length; i++)
            //            {
            //                // 这里对数据还不放心，进行优化处理
            //                if (!string.IsNullOrEmpty(mac[i]))
            //                {
            //                    mac[i] = mac[i].Trim();
            //                    result = redisClient.SetContainsItem(key, mac[i]);
            //                    if (result)
            //                    {
            //                        // 这里要提高判断的效率
            //                        break;
            //                    }
            //                }
            //            }
            //            // 若没有验证成功、把当前的 macAddress 保存起来, 方便后台管理的人加上去，这个是软件深入优化的亮点。
            //            if (!result)
            //            {
            //                var parameters = new List<KeyValuePair<string, object>>();
            //                parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldMacAddress, macAddress));
            //                var userLogonManager = new BaseUserLogonManager();
            //                userLogonManager.Update(userId, parameters);
            //            }
            //        }
            //    }
            //}
            //TODO
            result = true;
            return result;
        }


        /// <summary>
        /// 缓存预热,强制重新缓存
        /// </summary>
        /// <returns>影响行数</returns>
        public static int CachePreheatingMacAddress()
        {
            var result = 0;

            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseParameterEntity.FieldCategoryCode, "MacAddress"));
            parameters.Add(new KeyValuePair<string, object>(BaseParameterEntity.FieldEnabled, 1));
            parameters.Add(new KeyValuePair<string, object>(BaseParameterEntity.FieldDeleted, 0));

            // 把所有的数据都缓存起来的代码
            var manager = new BaseParameterManager();

            var dataReader = manager.ExecuteReader(parameters);
            if (dataReader != null && !dataReader.IsClosed)
            {
                while (dataReader.Read())
                {
                    var key = "MAC:" + dataReader[BaseParameterEntity.FieldParameterId];

                    var macAddress = dataReader[BaseParameterEntity.FieldParameterContent].ToString().ToLower();
                    CacheUtil.Set(key, macAddress);
                    result++;
                    if (result % 500 == 0)
                    {
                        Console.WriteLine(result + " : " + macAddress);
                    }
                }

                dataReader.Close();
            }

            return result;
        }

        /// <summary>
        /// 从缓存中去掉MAC限制、防止重复查询数据库？
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        public static bool ResetMacAddressByCache(string userId)
        {
            var result = false;

            if (ValidateUtil.IsInt(userId))
            {
                var key = "MAC:" + userId;
                result = CacheUtil.Remove(key);
            }

            return result;
        }

        /// <summary>
        /// 重设 MacAddress 限制，
        /// 2015-12-21 吉日嘎拉 历史数据不应该被丢失才对
        /// </summary>
        /// <param name="userId">用户、接口主键</param>
        /// <returns>影响行数</returns>
        public int ResetMacAddress(string userId)
        {
            var result = 0;

            // 把缓存里的先清理掉
            ResetMacAddressByCache(userId);

            //TODO 吉日嘎拉 这个操作应该增加个操作日志、谁什么时间，把什么数据删除了？ 把登录日志按操作日志、系统日志来看待？
            var commandText = "UPDATE " + BaseParameterEntity.CurrentTableName
                        + "   SET " + BaseParameterEntity.FieldDeleted + " = 1 "
                        + "     , " + BaseParameterEntity.FieldEnabled + " = 0 "
                        + " WHERE " + BaseParameterEntity.FieldCategoryCode + " =  " + DbHelper.GetParameter(BaseParameterEntity.FieldCategoryCode)
                        + "       AND " + BaseParameterEntity.FieldParameterId + " = " + DbHelper.GetParameter(BaseParameterEntity.FieldParameterId);

            var dbParameters = new List<IDbDataParameter>
            {
                DbHelper.MakeParameter(BaseParameterEntity.FieldCategoryCode, "MacAddress"),
                DbHelper.MakeParameter(BaseParameterEntity.FieldParameterId, userId)
            };
            result = ExecuteNonQuery(commandText, dbParameters.ToArray());

            return result;
        }


        // 2016-05-06 吉日嘎拉 不走缓存的方式实现

        /*

        public bool CheckMACAddress(string userId, string macAddress)
        {
            bool result = false;

            BaseParameterManager parameterManager = new BaseParameterManager(this.DbHelper, this.UserInfo);
            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterId, userId));
            parameters.Add(new KeyValuePair<string, object>(BaseParameterEntity.FieldCategoryCode, "MacAddress"));
            if (parameterManager.Exists(parameters))
            {
                if (this.CheckMacAddress(userId, macAddress))
                {
                    result = true;
                }
                else
                {
                    parameters = new List<KeyValuePair<string, object>>();
                    parameters.Add(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldMACAddress, macAddress));

                    BaseUserLogonManager userLogonManager = new BaseUserLogonManager(this.DbHelper, this.UserInfo);
                    userLogonManager.Update(userId, parameters);

                    return result;
                }
            }
            else
            {
                // 若没有设置mac限制，需要把限制都自动加上来。
                string[] mac = macAddress.Split(';');
                if (mac != null && mac.Length > 0)
                {
                    for (int i = 0; i < mac.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(mac[i]))
                        {
                            // 把收集过来的mac地址需要保存起来
                            BaseParameterEntity parameterEntity = new BaseParameterEntity();
                            parameterEntity.Id = Guid.NewGuid().ToString("N");
                            parameterEntity.CategoryCode = "MacAddress";
                            parameterEntity.ParameterCode = "Single";
                            parameterEntity.ParameterId = userId;
                            // 这里之际保存小写、就效率也高，省事了
                            parameterEntity.ParameterContent = mac[i].Trim().ToLower();
                            parameterManager.Add(parameterEntity);
                        }
                    }
                }
                result = true;
            }
            return result;
        }

        #region private bool CheckMacAddress(string userId, string macAddress) 检查用户的网卡Mac地址
        /// <summary>
        /// 检查用户的网卡Mac地址
        /// </summary>
        /// <param name="macAddress">Mac地址</param>
        /// <returns>符合限制</returns>
        private bool CheckMacAddress(string userId, string macAddress)
        {
            bool result = false;

            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterId, userId));
            parameters.Add(new KeyValuePair<string, object>(BaseParameterEntity.FieldCategoryCode, "MacAddress"));
            parameters.Add(new KeyValuePair<string, object>(BaseParameterEntity.FieldEnabled, 1));

            BaseParameterManager parameterManager = new BaseParameterManager(this.UserInfo);
            var dt = parameterManager.GetDataTable(parameters);
            if (dt != null && dt.Rows.Count > 0)
            {
                string[] mac = macAddress.Split(';');
                if (mac != null)
                {
                    for (int i = 0; i < mac.Length; i++)
                    {
                        string parameterCode = string.Empty;
                        string parameterCotent = string.Empty;
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            // parameterCode = dt.Rows[j][BaseParameterEntity.FieldParameterCode].ToString();
                            parameterCotent = dt.Rows[j][BaseParameterEntity.FieldParameterContent].ToString();
                            // 简单格式化一下
                            result = (mac[i].ToLower()).Equals(parameterCotent.ToLower());
                            if (result)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }
        #endregion

        private bool CheckMacAddress(string userId, string[] macAddress)
        {
            bool result = false;

            for (int i = 0; i < macAddress.Length; i++)
            {
                if (this.CheckMacAddress(userId, macAddress[i]))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
        
        */
    }
}