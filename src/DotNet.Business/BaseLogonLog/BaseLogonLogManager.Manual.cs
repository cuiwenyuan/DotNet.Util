//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------
using System;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using System.Threading;
    using Util;

    /// <summary>
    /// BaseLogonLogManager
    /// 用户登录日志管理
    /// 
    /// 修改记录
    /// 
    ///		2016.09.22 版本：1.0 Troy.Cui	新增。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.09.22</date>
    /// </author> 
    /// </summary>
    public partial class BaseLogonLogManager : BaseManager
    {
        #region 高级查询
        /// <summary>
        /// 按条件分页高级查询
        /// </summary>
        /// <param name="systemCode">子系统</param>
        /// <param name="userId">用户主键</param>
        /// <param name="userName">用户名</param>
        /// <param name="companyName">公司名称</param>
        /// <param name="result">操作结果</param>
        /// <param name="startTime">创建开始时间</param>
        /// <param name="endTime">创建结束时间</param>
        /// <param name="searchKey">搜索关键词</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序规则</param>
        /// <param name="showDisabled">显示已禁用</param>
        /// <param name="showDeleted">显示已删除</param>
        /// <returns></returns>
        public DataTable GetDataTableByPage(string systemCode, string userId, string userName, string companyName, string result, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseUtil.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            var sb = PoolUtil.StringBuilder.Get().Append(" 1 = 1");
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
            //子系统
            if (!string.IsNullOrEmpty(systemCode))
            {
                sb.Append(" AND " + BaseLogonLogEntity.CurrentTableName + "." + BaseLogonLogEntity.FieldSystemCode + " = N'" + systemCode + "'");
            }
            //用户主键
            if (ValidateUtil.IsInt(userId))
            {
                sb.Append(" AND " + BaseLogonLogEntity.CurrentTableName + "." + BaseLogonLogEntity.FieldUserId + " = N'" + userId + "'");
            }
            //用户名
            if (!string.IsNullOrEmpty(userName))
            {
                sb.Append(" AND " + BaseLogonLogEntity.CurrentTableName + "." + BaseLogonLogEntity.FieldUserName + " = N'" + userName + "'");
            }
            //公司名称
            if (!string.IsNullOrEmpty(companyName))
            {
                sb.Append(" AND " + BaseLogonLogEntity.CurrentTableName + "." + BaseLogonLogEntity.FieldCompanyName + " = N'" + companyName + "'");
            }
            //操作状态
            if (ValidateUtil.IsInt(result))
            {
                sb.Append(" AND " + BaseLogonLogEntity.CurrentTableName + "." + BaseLogonLogEntity.FieldResult + " = " + result);
            }
            //创建日期
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseLogonLogEntity.CurrentTableName + "." + BaseLogonLogEntity.FieldCreateTime + " >= " + dbHelper.ToDbTime(startTime));
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseLogonLogEntity.CurrentTableName + "." + BaseLogonLogEntity.FieldCreateTime + " <= " + dbHelper.ToDbTime(endTime.ToDateTime().Date.AddDays(1).AddMilliseconds(-1)));
            }
            //关键词
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseLogonLogEntity.FieldUserName + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldCompanyName + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldOperationType + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldIpAddress + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldLogonStatus + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldRealName + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", " ");
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Return());
        }
        #endregion

        #region 高级查询
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
        /// <param name="systemCode">子系统编码</param>
        /// <param name="userName">用户名</param>
        /// <param name="companyName">公司名称</param>
        /// <param name="result">结果</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseLogonLogEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true, string systemCode = null, string userName = null, string companyName = null, string result = null)
        {
            var sb = PoolUtil.StringBuilder.Get().Append(" 1 = 1");
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
                sb.Append(" AND " + BaseLogonLogEntity.FieldCompanyId + " = " + companyId);
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
                sb.Append(" AND " + BaseLogonLogEntity.FieldUserId + " = " + userId);
            }
            //创建时间
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseLogonLogEntity.FieldCreateTime + " >= " + dbHelper.ToDbTime(startTime));
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseLogonLogEntity.FieldCreateTime + " <= " + dbHelper.ToDbTime(endTime.ToDateTime().Date.AddDays(1).AddMilliseconds(-1)));
            }
            //子系统编码
            if (!string.IsNullOrEmpty(systemCode))
            {
                sb.Append(" AND " + BaseLogonLogEntity.CurrentTableName + "." + BaseLogonLogEntity.FieldSystemCode + " = N'" + systemCode + "'");
            }
            //用户名
            if (!string.IsNullOrEmpty(userName))
            {
                userName = dbHelper.SqlSafe(userName);
                sb.Append(" AND " + BaseLogonLogEntity.FieldUserName + " = N'" + userName + "'");
            }
            //操作状态
            if (!string.IsNullOrEmpty(result))
            {
                result = dbHelper.SqlSafe(result);
                sb.Append(" AND " + BaseLogonLogEntity.FieldResult + " = N'" + result + "'");
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseLogonLogEntity.FieldUserName + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldCompanyName + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldOperationType + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldIpAddress + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldLogonStatus + " LIKE N'%" + searchKey + "%' OR " + BaseLogonLogEntity.FieldRealName + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", " ");
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Return());
        }
        #endregion

        #region public static string GetSplitTableName(BaseUserInfo userInfo)

        /// <summary>
        /// 获取分表名
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <returns>分割后的表名</returns>
        public static string GetSplitTableName(BaseUserInfo userInfo)
        {
            return GetSplitTableName(userInfo.CompanyId.ToString());
        }

        #endregion

        #region public static string GetSplitTableName(BaseUserEntity userInfo)

        /// <summary>
        /// 获取分表名
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <returns></returns>
        public static string GetSplitTableName(BaseUserEntity userInfo)
        {
            return GetSplitTableName(userInfo.CompanyId.ToString());
        }

        #endregion

        #region public static string GetSplitTableName(string companyId = null)

        /// <summary>
        /// 获取分表名，这个是决定未来方向的函数，
        /// 不断改进完善，会是我们未来的方向
        /// </summary>
        /// <param name="companyId">公司编号</param>
        /// <returns>分割后的表名</returns>
        public static string GetSplitTableName(string companyId = null)
        {
            // 默认表名字
            var result = BaseLogonLogEntity.CurrentTableName;

            return result;
        }

        #endregion

        #region private static void AddLogTaskByBaseUserInfo(object param)
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
                    CreateTime = DateTime.Now,
                    CreateBy = userInfo.RealName,
                    SortCode = 1
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

        #endregion

        #region public static void AddLog(string systemCode, BaseUserInfo userInfo, string ipAddress, string ipAddressName, string macAddress, string loginStatus)

        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="ipAddress"></param>
        /// <param name="ipAddressName"></param>
        /// <param name="macAddress"></param>
        /// <param name="loginStatus"></param>
        public static void AddLog(string systemCode, BaseUserInfo userInfo, string ipAddress, string ipAddressName, string macAddress, string loginStatus)
        {
            if (BaseSystemInfo.RecordLogonLog)
            {
                // 抛出一个线程，现在主库的性能有问题，临时屏蔽一下
                new Thread(AddLogTaskByBaseUserInfo).Start(new Tuple<string, BaseUserInfo, string, string, string, string>(systemCode, userInfo, ipAddress, ipAddressName, macAddress, loginStatus));
            }
        }

        #endregion

        #region public static string AddLog(string systemCode, BaseUserEntity userEntity, string ipAddress, string ipAddressName, string macAddress, string loginStatus, int operationType = 1, int loginResult = 1, string sourceType = null, string targetApplication = null, string targetIp = null)

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
            entity.CreateBy = userEntity.RealName;
            entity.SortCode = 1;

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

        #endregion

        #region private static int LogonStatusToLogLevel(string loginStatus)

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

        #endregion

        #region public static void AddLog(BaseUserInfo userInfo, string loginStatus)

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

        #endregion

        #region private static void AddLogTask(object param)

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
                    CreateTime = DateTime.Now,
                    CreateBy = userName,
                    SortCode = 1
                };

                var tableName = GetSplitTableName();
                using (var dbHelper = DbHelperFactory.Create(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection))
                {
                    var manager = new BaseLogonLogManager(tableName);
                    try
                    {
                        // 2015-07-13 把登录日志无法正常写入的，进行日志记录
                        manager.Add(entity);
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

        #endregion

        #region public static void AddLog(string systemCode, string userId, string userName, string nickName, string ipAddress, string ipAddressName, string macAddress, string loginStatus)

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
                // 抛出一个线程
                new Thread(AddLogTask).Start(new Tuple<string, string, string, string, string, string, string, Tuple<string>>(systemCode, userId, userName, nickName, ipAddress, ipAddressName, macAddress, new Tuple<string>(loginStatus)));
            }
        }

        #endregion
    }
}