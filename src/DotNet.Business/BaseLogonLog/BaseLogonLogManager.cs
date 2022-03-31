﻿//-----------------------------------------------------------------------
// <copyright file="BaseLogonLogManager.Auto.cs" company="DotNet">
//     Copyright (C) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace DotNet.Business
{
    using Model;
    using System.Collections.Generic;
    using System.Data;
    using Util;

    /// <summary>
    /// BaseLogonLogManager
    /// 系统登录日志表
    /// 
    /// 修改记录
    /// 
    /// 2014-09-05 版本：2.0 JiRiGaLa 获取IP地址城市的功能实现。
    /// 2014-03-18 版本：1.0 JiRiGaLa 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2014-03-18</date>
    /// </author>
    /// </summary>
    public partial class BaseLogonLogManager : BaseManager, IBaseManager
    {
        #region public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseLogonLogEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = false, bool showDeleted = false)
        /// <summary>
        /// 按条件分页查询(带记录状态Enabled和删除状态Deleted)
        /// </summary>
        /// <param name="companyId">查看公司主键</param>
        /// <param name="departmentId">查看部门主键</param>
        /// <param name="userId">查看用户主键</param>
        /// <param name="startTime">创建开始时间</param>
        /// <param name="endTime">创建结束时间</param>
        /// <param name="searchKey">查询关键字</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="showDisabled">是否显示无效记录</param>
        /// <param name="showDeleted">是否显示已删除记录</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseLogonLogEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseLogonLogEntity.FieldEnabled + " = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseLogonLogEntity.FieldDeleted + " = 0");
            }

            if (ValidateUtil.IsInt(companyId))
            {
                //sb.Append(" AND " + BaseLogonLogEntity.FieldCompanyId + " = " + companyId);
            }
            // 只有管理员才能看到所有的
            //if (!(UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled))
            //{
                //sb.Append(" AND (" + BaseLogonLogEntity.FieldUserCompanyId + " = 0 OR " + BaseLogonLogEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            //}
            if (ValidateUtil.IsInt(departmentId))
            {
                //sb.Append(" AND " + BaseLogonLogEntity.FieldDepartmentId + " = " + departmentId);
            }
            if (ValidateUtil.IsInt(userId))
            {
                //sb.Append(" AND " + BaseLogonLogEntity.FieldUserId + " = " + userId);
            }
            //创建时间
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseLogonLogEntity.FieldCreateTime + " >= '" + startTime + "'");
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseLogonLogEntity.FieldCreateTime + " <= DATEADD(s,-1,DATEADD(d,1,'" + endTime + "'))");
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseLogonLogEntity.FieldUserName + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldSystemCode + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Put(), null, "*");
        }
        #endregion

        #region 下拉菜单

        /// <summary>
        /// 下拉菜单
        /// </summary>
        /// <param name="myCompanyOnly">仅本公司</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(bool myCompanyOnly = true)
        {
            var sb = Pool.StringBuilder.Get();
            if (myCompanyOnly)
            {
                //sb.Append("(" + BaseLogonLogEntity.FieldUserCompanyId + " = 0 OR " + BaseLogonLogEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            }
            //return GetDataTable(sb.Put(), null, new KeyValuePair<string, object>(BaseLogonLogEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BaseLogonLogEntity.FieldDeleted, 0));
            var companyId = string.IsNullOrEmpty(BaseSystemInfo.CustomerCompanyId) ? UserInfo.CompanyId : BaseSystemInfo.CustomerCompanyId;
            var cacheKey = "DataTable." + CurrentTableName + "." + companyId + "." + (myCompanyOnly ? "1" : "0");
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            return CacheUtil.Cache<DataTable>(cacheKey, () => GetDataTable(sb.Put(), null, new KeyValuePair<string, object>(BaseLogonLogEntity.FieldEnabled, 1), new KeyValuePair<string, object>(BaseLogonLogEntity.FieldDeleted, 0)), true, false, cacheTime);
        }

        #endregion

        /// <summary>
        /// 获取切割的表名
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <returns>分割后的表名</returns>
        public static string GetSplitTableName(BaseUserInfo userInfo)
        {
            return GetSplitTableName(userInfo.CompanyId.ToString());
        }
        /// <summary>
        /// 获取切割的表名
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static string GetSplitTableName(BaseUserEntity userInfo)
        {
            return GetSplitTableName(userInfo.CompanyId.ToString());
        }

        /// <summary>
        /// 获取切割的表名，这个是决定未来方向的函数，
        /// 不断改进完善，会是我们未来的方向
        /// </summary>
        /// <param name="companyId">公司编号</param>
        /// <returns>分割后的表名</returns>
        public static string GetSplitTableName(string companyId = null)
        {
            // 默认表名字
            var result = BaseLogonLogEntity.CurrentTableName;

            /*

            // 目前主要是Oracle数据库上进行且分、将来可以是所有的数据库上进行切分
            if (BaseSystemInfo.ServerDbType == CurrentDbType.Oracle)
            {
                // result = "BASE_LOGINLOG" + DateTime.Now.ToString("yyyy");
                result = "BASE_LOGINLOG";

                // 这里需要判断网点主键是否为空？
                if (!string.IsNullOrEmpty(companyId))
                {
                    // 对网点主键进行尾部2位截取
                    string db = companyId;
                    if (companyId.Length > 1)
                    {
                        db = companyId.Substring(companyId.Length - 2);
                    }
                    else
                    {
                        // 若不够2位补充前导0，补充为2位长，这里千万不能忘了，否则容易出错误。
                        db = "0" + db;
                    }
                    // 检查你截取到的是否为正确的数字类型？若不是数字类型的，就不能分发到表里去
                    if (ValidateUtil.IsInt(db))
                    {
                        // result = result + "_" + DateTime.Now.ToString("yyyy") + "_" + db;
                        result = result + "_" + db;
                    }
                }
            }
            
            */
            return result;
        }


        #region 选择服务商返回地址  1.百度 2.新浪 3.淘宝
        /// <summary>
        /// 选择服务商返回地址   1.百度 2.新浪 3.淘宝
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="serve">1.百度 2.新浪 3.淘宝</param>
        /// <returns></returns>
        public static string GetIpAddressNameByWebService(string ipAddress, int serve = 2)
        {
            try
            {
                //过滤掉端口号 
                if (ipAddress.IndexOf(':') > 0)
                {
                    ipAddress = ipAddress.Split(':')[0];
                }
                var match =
                    new Regex(@"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))");
                if (!match.IsMatch(ipAddress))
                {
                    return string.Empty;
                }
                var webClient = new System.Net.WebClient();
                NameValueCollection postValues = null;
                // 向服务器发送POST数据
                var url = string.Empty;
                if (serve == 1)
                {
                    url = "http://api.map.baidu.com/location/ip";
                    postValues = new NameValueCollection
                            {
                                {"ak", "MRkBd6jnGOf8O5F58KKrvit5"},
                                {"ip", ipAddress},
                                {"coor", "bd09ll"}
                            };
                }
                else if (serve == 2)
                {
                    //此处是个坑  只支持拼接字符串的请求 不支持form表单得到
                    url = "http://int.dpool.sina.com.cn/iplookup/iplookup.php?format=json&ip=" + ipAddress;
                    postValues = new NameValueCollection
                            {
                                {"format", "json"},
                                {"ip", ipAddress}
                            };
                }
                else
                {
                    url = "http://ip.taobao.com/service/getIpInfo.php";
                    postValues = new NameValueCollection
                            {
                                {"ip", ipAddress}
                            };
                }

                var responseArray = webClient.UploadValues(url, postValues);

                var response = Encoding.UTF8.GetString(responseArray);

                var dataJson = JObject.Parse(response);  //动态解析  正常的解析无法生效
                var address = string.Empty;
                //百度接口
                if (serve == 1)
                {
                    if (dataJson["status"].ToString() == "0")
                    {
                        address = dataJson["content"]["address_detail"]["province"] + "-" + dataJson["content"]["address_detail"]["city"];
                    }
                }
                //新浪接口
                else if (serve == 2)
                {
                    if (dataJson["ret"].ToString() == "1")
                    {
                        address = dataJson["province"] + "-" + dataJson["city"];
                    }
                }
                //淘宝接口
                else
                {
                    if (dataJson["code"].ToString() == "0")
                    {
                        if (!string.IsNullOrEmpty(dataJson["data"]["region"].ToString()))
                            address = dataJson["data"]["region"] + "-" + dataJson["data"]["city"];
                    }
                }
                if (string.IsNullOrEmpty(address))
                {
                    address = "局域网";
                }
                return address;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
        #endregion

        /// <summary>
        /// 获取IP地址的名称
        /// </summary>
        /// <param name="ipHelper"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public int GetIpAddressName(IpUtil ipHelper, string tableName = null)
        {
            var result = 0;

            if (string.IsNullOrEmpty(tableName))
            {
                tableName = BaseLogonLogEntity.CurrentTableName;
                /*
                if (BaseSystemInfo.BusinessDbType == CurrentDbType.Oracle)
                {
                    tableName = BaseLogonLogEntity.CurrentTableName + DateTime.Now.ToString("yyyyMM");
                }
                */
            }

            using (var dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.LogonLogDbType, BaseSystemInfo.LogonLogDbConnection))
            {
                var id = string.Empty;
                var ipaddress = string.Empty;
                var ipAddressName = string.Empty;
                var commandText = "SELECT id, ipAddress FROM " + tableName + " WHERE ipaddressname IS NULL AND ipaddress IS NOT NULL AND ROWNUM < 10000 ORDER BY createon DESC ";
                using (var dataReader = DbHelper.ExecuteReader(commandText))
                {
                    while (dataReader.Read())
                    {
                        id = dataReader["id"].ToString();
                        ipaddress = dataReader["ipaddress"].ToString();
                        ipAddressName = ipHelper.FindName(ipaddress);
                        if (!string.IsNullOrWhiteSpace(ipAddressName))
                        {
                            commandText = "UPDATE " + tableName + " SET ipAddressName = '" + ipAddressName + "' WHERE Id = '" + id + "'";
                            dbHelper.ExecuteNonQuery(commandText);
                            Console.WriteLine(ipaddress + ":" + ipAddressName);
                        }
                        else
                        {
                            ipAddressName = string.Empty;
                        }
                        result++;
                    }
                    dataReader.Close();
                }
            }
            return result;
        }

        private static void AddLogTaskByBaseUserInfo(object param)
        {
            var tuple = param as Tuple<string, BaseUserInfo, string, string, string, string>;
            if (tuple != null)
            {
                var systemCode = tuple.Item1;
                var userInfo = tuple.Item2;
                var ipAddress = tuple.Item3;
                var ipAddressName = tuple.Item4;
                var macAddress = tuple.Item5;
                var loginStatus = tuple.Item6;

                var entity = new BaseLogonLogEntity
                {
                    SystemCode = systemCode,
                    UserId = userInfo.UserId,
                    UserName = userInfo.UserName,
                    NickName = userInfo.NickName,
                    RealName = userInfo.RealName,
                    CompanyId = userInfo.CompanyId.ToInt(),
                    CompanyName = userInfo.CompanyName,
                    CompanyCode = userInfo.CompanyCode,
                    IpAddress = ipAddress,
                    IpAddressName = ipAddressName,
                    MacAddress = macAddress,
                    LogonStatus = loginStatus,
                    LogLevel = LogonStatusToLogLevel(loginStatus),
                    CreateTime = DateTime.Now
                };

                var tableName = GetSplitTableName(userInfo);

                var loginLogManager = new BaseLogonLogManager(tableName);
                try
                {
                    // 2015-07-13 把登录日志无法正常写入的，进行日志记录
                    loginLogManager.Add(entity);
                }
                catch (Exception ex)
                {
                    LogUtil.WriteLog("AddLogTask: ipAddress:" + ipAddress + "macAddress:" + macAddress + "userName:" + userInfo.UserName
                                     + Environment.NewLine + "异常信息:" + ex.Message
                                     + Environment.NewLine + "错误源:" + ex.Source
                                     + Environment.NewLine + "堆栈信息:" + ex.StackTrace, "Log");
                }

            }
        }
        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userInfo"></param>
        /// <param name="ipAddress"></param>
        /// <param name="ipAddressName"></param>
        /// <param name="macAddress"></param>
        /// <param name="loginStatus"></param>
        public static void AddLog(string systemCode, BaseUserInfo userInfo, string ipAddress, string ipAddressName, string macAddress, string loginStatus)
        {
            if (BaseSystemInfo.RecordLogonLog)
            {
                // 吉日嘎拉 抛出一个线程，现在主库的性能有问题，临时屏蔽一下
                new Thread(AddLogTaskByBaseUserInfo).Start(new Tuple<string, BaseUserInfo, string, string, string, string>(systemCode, userInfo, ipAddress, ipAddressName, macAddress, loginStatus));
            }
        }
        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userEntity"></param>
        /// <param name="ipAddress"></param>
        /// <param name="ipAddressName"></param>
        /// <param name="macAddress"></param>
        /// <param name="loginStatus"></param>
        /// <param name="operationType"></param>
        /// <param name="loginResult"></param>
        /// <param name="sourceType"></param>
        /// <param name="targetApplication"></param>
        /// <param name="targetIp"></param>
        /// <returns></returns>
        public static string AddLog(string systemCode, BaseUserEntity userEntity, string ipAddress, string ipAddressName, string macAddress, string loginStatus, int operationType = 1, int loginResult = 1, string sourceType = null, string targetApplication = null, string targetIp = null)
        {
            if (!BaseSystemInfo.RecordLogonLog)
            {
                return string.Empty;
            }

            if (userEntity == null)
            {
                return null;
            }

            var result = string.Empty;

            var entity = new BaseLogonLogEntity
            {
                SystemCode = systemCode,
                UserId = userEntity.Id,
                UserName = userEntity.UserName,
                NickName = userEntity.NickName,
                RealName = userEntity.RealName,
                CompanyId = userEntity.CompanyId,
                CompanyName = userEntity.CompanyName
            };
            if (BaseSystemInfo.OnInternet && userEntity.CompanyId > 0)
            {
                entity.CompanyCode = BaseOrganizationManager.GetCodeByCache(userEntity.CompanyId.ToString());
            }

            entity.Province = userEntity.Province;
            entity.City = userEntity.City;

            entity.TargetApplication = targetApplication;
            entity.TargetIp = targetIp;
            entity.SourceType = sourceType;

            entity.IpAddress = ipAddress;
            entity.IpAddressName = ipAddressName;
            entity.MacAddress = macAddress;
            entity.LogonStatus = loginStatus;

            entity.OperationType = operationType;
            entity.Result = loginResult;

            entity.LogLevel = LogonStatusToLogLevel(loginStatus);
            entity.CreateTime = DateTime.Now;

            var tableName = GetSplitTableName(userEntity);

            var loginLogManager = new BaseLogonLogManager(tableName);
            try
            {
                // 2015-07-13 把登录日志无法正常写入的，进行日志记录
                result = loginLogManager.Add(entity);
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog("AddLogTask: 异常信息:" + ex.Message + "userName:" + userEntity.UserName
                                 + Environment.NewLine + "错误源:" + ex.Source
                                 + Environment.NewLine + "堆栈信息:" + ex.StackTrace, "Log");
            }

            return result;
        }

        /// <summary>
        /// 登录级别（0，正常；1、注意；2，危险；3、攻击）
        /// </summary>
        /// <param name="loginStatus"></param>
        /// <returns></returns>
        private static int LogonStatusToLogLevel(string loginStatus)
        {
            // 
            var result = 1;

            if (!string.IsNullOrEmpty(loginStatus))
            {
                if (loginStatus == "用户登录")
                {
                    result = 0;
                }
                else if (loginStatus == "退出系统")
                {
                    result = 0;
                }
                else if (loginStatus == "密码错误")
                {
                    result = 2;
                }
                else if (loginStatus == "用户没有找到")
                {
                    result = 2;
                }
            }
            return result;
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="loginStatus">动作</param>
        public static void AddLog(BaseUserInfo userInfo, string loginStatus)
        {
            var systemCode = userInfo.SystemCode;
            var ipAddress = userInfo.IpAddress;
            var ipAddressName = IpUtil.GetInstance().FindName(userInfo.IpAddress);
            var macAddress = userInfo.MacAddress;
            AddLog(systemCode, userInfo, ipAddress, ipAddressName, macAddress, loginStatus);
        }

        private static void AddLogTask(object param)
        {
            var tuple = param as Tuple<string, string, string, string, string, string, string, Tuple<string>>;
            if (tuple != null)
            {
                var systemCode = tuple.Item1;
                var userId = tuple.Item2;
                var userName = tuple.Item3;
                var nickName = tuple.Item4;
                var ipAddress = tuple.Item5;
                var ipAddressName = tuple.Item6;
                var macAddress = tuple.Item7;
                var loginStatus = string.Empty;
                if (tuple.Rest != null)
                {
                    loginStatus = tuple.Rest.Item1;
                }

                var entity = new BaseLogonLogEntity
                {
                    SystemCode = systemCode,
                    UserId = userId.ToInt(),
                    UserName = userName,
                    //Troy.Cui 20160927
                    NickName = nickName,
                    IpAddress = ipAddress,
                    IpAddressName = ipAddressName,
                    MacAddress = macAddress,
                    LogonStatus = loginStatus,
                    LogLevel = LogonStatusToLogLevel(loginStatus),
                    CreateTime = DateTime.Now
                };

                var tableName = GetSplitTableName();
                using (var dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection))
                {
                    var loginLogManager = new BaseLogonLogManager(tableName);
                    try
                    {
                        // 2015-07-13 把登录日志无法正常写入的，进行日志记录
                        loginLogManager.Add(entity);
                    }
                    catch (Exception ex)
                    {
                        LogUtil.WriteLog("AddLogTask: 异常信息:" + ex.Message
                                                             + Environment.NewLine + "错误源:" + ex.Source
                                                             + Environment.NewLine + "堆栈信息:" + ex.StackTrace, "Log");
                    }
                }
            }

        }
        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="nickName"></param>
        /// <param name="ipAddress"></param>
        /// <param name="ipAddressName"></param>
        /// <param name="macAddress"></param>
        /// <param name="loginStatus"></param>
        public static void AddLog(string systemCode, string userId, string userName, string nickName, string ipAddress, string ipAddressName, string macAddress, string loginStatus)
        {
            if (BaseSystemInfo.RecordLogonLog)
            {
                //抛出一个线程
                new Thread(AddLogTask).Start(new Tuple<string, string, string, string, string, string, string, Tuple<string>>(systemCode, userId, userName, nickName, ipAddress, ipAddressName, macAddress, new Tuple<string>(loginStatus)));
            }
        }
    }
}
