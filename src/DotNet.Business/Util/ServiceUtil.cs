//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Reflection;

namespace DotNet.Business
{
    using Util;

    /// <summary>
    /// ServiceUtil
    /// 使用独创的委托加匿名函数对传统的模板模式作出了新的诠释
    /// 
    /// 修改记录
    /// 
    ///		2016.05.10 版本：1.0	JiRiGaLa 数字签名信息、用户信息签名校验搬迁到这里来。
    /// 
    /// <author>
    ///		<name>张祈璟</name>
    ///		<date>2016.05.10</date>
    /// </author> 
    /// </summary>
    public class ServiceUtil
    {
        /// <summary>
        /// 对登录的用户进行数字签名
        /// </summary>
        /// <param name="userInfo">登录信息</param>
        /// <returns>进行过数字签名的用户登录信息</returns>
        public static BaseUserInfo CreateSignature(BaseUserInfo userInfo)
        {
            if (userInfo != null)
            {
                if (string.IsNullOrEmpty(userInfo.Code))
                {
                    userInfo.Code = string.Empty;
                }
                if (string.IsNullOrEmpty(userInfo.CompanyCode))
                {
                    userInfo.CompanyCode = string.Empty;
                }
                if (string.IsNullOrEmpty(userInfo.CompanyId))
                {
                    userInfo.CompanyId = string.Empty;
                }
                if (string.IsNullOrEmpty(userInfo.CompanyName))
                {
                    userInfo.CompanyName = string.Empty;
                }
                if (string.IsNullOrEmpty(userInfo.DepartmentCode))
                {
                    userInfo.DepartmentCode = string.Empty;
                }
                if (string.IsNullOrEmpty(userInfo.DepartmentId))
                {
                    userInfo.DepartmentId = string.Empty;
                }
                if (string.IsNullOrEmpty(userInfo.DepartmentName))
                {
                    userInfo.DepartmentName = string.Empty;
                }
                if (string.IsNullOrEmpty(userInfo.NickName))
                {
                    userInfo.NickName = string.Empty;
                }
                if (string.IsNullOrEmpty(userInfo.OpenId))
                {
                    userInfo.OpenId = string.Empty;
                }
                if (string.IsNullOrEmpty(userInfo.RealName))
                {
                    userInfo.RealName = string.Empty;
                }
                if (string.IsNullOrEmpty(userInfo.UserName))
                {
                    userInfo.UserName = string.Empty;
                }
                // 需要签名的内容部分
                var dataToSign = userInfo.Code + "_"
                    + userInfo.CompanyCode + "_"
                    + userInfo.CompanyId + "_"
                    + userInfo.CompanyName + "_"
                    + userInfo.DepartmentCode + "_"
                    + userInfo.DepartmentId + "_"
                    + userInfo.DepartmentName + "_"
                    + userInfo.Id + "_"
                    + userInfo.IdentityAuthentication + "_"
                    + userInfo.IsAdministrator + "_"
                    + userInfo.NickName + "_"
                    + userInfo.OpenId + "_"
                    + userInfo.RealName + "_"
                    + userInfo.UserName;

                // 进行签名
                userInfo.Signature = SecretUtil.Md5(dataToSign);
            }

            return userInfo;
        }

        /// <summary>
        /// 对登录的用户进行数字签名
        /// </summary>
        /// <param name="userInfo">登录信息</param>
        /// <returns>进行过数字签名的用户登录信息</returns>
        public static bool VerifySignature(BaseUserInfo userInfo)
        {
            var result = false;

            if (userInfo != null && !string.IsNullOrEmpty(userInfo.Signature))
            {
                if (string.IsNullOrEmpty(userInfo.Code))
                {
                    userInfo.Code = string.Empty;
                }
                if (string.IsNullOrEmpty(userInfo.CompanyCode))
                {
                    userInfo.CompanyCode = string.Empty;
                }
                if (string.IsNullOrEmpty(userInfo.CompanyId))
                {
                    userInfo.CompanyId = string.Empty;
                }
                if (string.IsNullOrEmpty(userInfo.CompanyName))
                {
                    userInfo.CompanyName = string.Empty;
                }
                if (string.IsNullOrEmpty(userInfo.DepartmentCode))
                {
                    userInfo.DepartmentCode = string.Empty;
                }
                if (string.IsNullOrEmpty(userInfo.DepartmentId))
                {
                    userInfo.DepartmentId = string.Empty;
                }
                if (string.IsNullOrEmpty(userInfo.DepartmentName))
                {
                    userInfo.DepartmentName = string.Empty;
                }
                if (string.IsNullOrEmpty(userInfo.NickName))
                {
                    userInfo.NickName = string.Empty;
                }
                if (string.IsNullOrEmpty(userInfo.OpenId))
                {
                    userInfo.OpenId = string.Empty;
                }
                if (string.IsNullOrEmpty(userInfo.RealName))
                {
                    userInfo.RealName = string.Empty;
                }
                if (string.IsNullOrEmpty(userInfo.UserName))
                {
                    userInfo.UserName = string.Empty;
                }
                // 需要签名的内容部分
                var dataToSign = userInfo.Code + "_"
                    + userInfo.CompanyCode + "_"
                    + userInfo.CompanyId + "_"
                    + userInfo.CompanyName + "_"
                    + userInfo.DepartmentCode + "_"
                    + userInfo.DepartmentId + "_"
                    + userInfo.DepartmentName + "_"
                    + userInfo.Id + "_"
                    + userInfo.IdentityAuthentication + "_"
                    + userInfo.IsAdministrator + "_"
                    + userInfo.NickName + "_"
                    + userInfo.OpenId + "_"
                    + userInfo.RealName + "_"
                    + userInfo.UserName;

                result = userInfo.Signature == SecretUtil.Md5(dataToSign);
            }

            return result;
        }

        /// <summary>
        /// ProcessFun
        /// </summary>
        /// <param name="dbHelper"></param>
		public delegate void ProcessFun(IDbHelper dbHelper);

        /// <summary>
        /// ProcessFunWithLock
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="getOnline"></param>
        /// <returns></returns>
        public delegate bool ProcessFunWithLock(IDbHelper dbHelper, bool getOnline);
        /// <summary>
        /// ProcessUserCenterReadDb
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="parameter">参数</param>
        /// <param name="fun"></param>
        public static void ProcessUserCenterReadDb(BaseUserInfo userInfo, ServiceInfo parameter, ProcessFun fun)
        {
            if (BaseSystemInfo.IsAuthorized(userInfo))
            {
                ProcessDb(parameter, fun, DbType.UserCenterRead);
            }
        }
        /// <summary>
        /// ProcessUserCenterWriteDb
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="parameter">参数</param>
        /// <param name="fun"></param>
        public static void ProcessUserCenterWriteDb(BaseUserInfo userInfo, ServiceInfo parameter, ProcessFun fun)
        {
            if (BaseSystemInfo.IsAuthorized(userInfo))
            {
                ProcessDb(parameter, fun, DbType.UserCenterWrite);
            }
        }
        /// <summary>
        /// ProcessUserCenterDb
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="parameter">参数</param>
        /// <param name="fun"></param>
        public static void ProcessUserCenterDb(BaseUserInfo userInfo, ServiceInfo parameter, ProcessFun fun)
        {
            if (BaseSystemInfo.IsAuthorized(userInfo))
            {
                ProcessDb(parameter, fun, DbType.UserCenter);
            }
        }
        /// <summary>
        /// ProcessUserCenterWriteDbWithLock
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="parameter">参数</param>
        /// <param name="locker"></param>
        /// <param name="fun"></param>
        public static void ProcessUserCenterWriteDbWithLock(BaseUserInfo userInfo, ServiceInfo parameter, object locker, ProcessFunWithLock fun)
        {
            if (BaseSystemInfo.IsAuthorized(userInfo))
            {
                var milliStart = Begin(parameter.UserInfo, parameter.CurrentMethod);

                var getOnline = false;

                lock (locker)
                {
                    using (var dbHelper = DbHelperFactory.Create(GetDbType(DbType.UserCenterWrite)))
                    {
                        try
                        {
                            dbHelper.Open(GetDbConnection(DbType.UserCenterWrite));
                            getOnline = fun(dbHelper, getOnline);
                            AddLog(parameter);
                        }
                        catch (Exception ex)
                        {
                            BaseExceptionManager.LogException(dbHelper, parameter.UserInfo, ex);
                            throw;
                        }
                        finally
                        {
                            dbHelper.Close();
                        }
                    }
                }
                End(parameter.UserInfo, milliStart, parameter.CurrentMethod, getOnline);
            }
        }
        /// <summary>
        /// ProcessUserCenterWriteDbWithLock
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="parameter">参数</param>
        /// <param name="locker"></param>
        /// <param name="fun"></param>
        public static void ProcessUserCenterWriteDbWithLock(BaseUserInfo userInfo, ServiceInfo parameter, object locker, ProcessFun fun)
        {
            if (BaseSystemInfo.IsAuthorized(userInfo))
            {
                var milliStart = Begin(parameter.UserInfo, parameter.CurrentMethod);
                lock (locker)
                {
                    ProcessDbHelp(parameter, fun, DbType.UserCenterWrite, false);
                }
                End(parameter.UserInfo, milliStart, parameter.CurrentMethod);
            }
        }
        /// <summary>
        /// ProcessUserCenterWriteDbWithTransaction
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="parameter">参数</param>
        /// <param name="fun"></param>
        public static void ProcessUserCenterWriteDbWithTransaction(BaseUserInfo userInfo, ServiceInfo parameter, ProcessFun fun)
        {
            if (BaseSystemInfo.IsAuthorized(userInfo))
            {
                ProcessDb(parameter, fun, DbType.UserCenterWrite, true);
            }
        }
        /// <summary>
        /// ProcessBusinessDb
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="parameter">参数</param>
        /// <param name="fun"></param>
        public static void ProcessBusinessDb(BaseUserInfo userInfo, ServiceInfo parameter, ProcessFun fun)
        {
            if (BaseSystemInfo.IsAuthorized(userInfo))
            {
                ProcessDb(parameter, fun, DbType.Business);
            }
        }
        /// <summary>
        /// ProcessLogonLogDb
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="parameter">参数</param>
        /// <param name="fun"></param>
        public static void ProcessLogonLogDb(BaseUserInfo userInfo, ServiceInfo parameter, ProcessFun fun)
        {
            if (BaseSystemInfo.IsAuthorized(userInfo))
            {
                ProcessDb(parameter, fun, DbType.LogonLog);
            }
        }
        /// <summary>
        /// ProcessMessageDb
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="parameter">参数</param>
        /// <param name="fun"></param>
        public static void ProcessMessageDb(BaseUserInfo userInfo, ServiceInfo parameter, ProcessFun fun)
        {
            if (BaseSystemInfo.IsAuthorized(userInfo))
            {
                ProcessDb(parameter, fun, DbType.Message);
            }
        }

        /// <summary>
        /// ProcessWorkFlowDb
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="parameter">参数</param>
        /// <param name="fun"></param>
        public static void ProcessWorkFlowDb(BaseUserInfo userInfo, ServiceInfo parameter, ProcessFun fun)
        {
            if (BaseSystemInfo.IsAuthorized(userInfo))
            {
                ProcessDb(parameter, fun, DbType.WorkFlow);
            }
        }

        /// <summary>
        /// ProcessWorkFlowDbWithTransaction
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="parameter">参数</param>
        /// <param name="fun"></param>
        public static void ProcessWorkFlowDbWithTransaction(BaseUserInfo userInfo, ServiceInfo parameter, ProcessFun fun)
        {
            if (BaseSystemInfo.IsAuthorized(userInfo))
            {
                ProcessDb(parameter, fun, DbType.WorkFlow, true);
            }
        }

        /// <summary>
        /// 使用简单的工厂方法，可以做成多态的类
        /// </summary>
        private enum DbType
        {
            UserCenterRead = 1,
            UserCenterWrite = 2,
            Business = 3,
            Message = 4,
            WorkFlow = 5,
            UserCenter = 6,
            LogonLog = 7
        }
        /// <summary>
        /// 获取数据库类型
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
		private static CurrentDbType GetDbType(DbType dbType)
        {
            switch (dbType)
            {
                default:
                case DbType.UserCenterRead:
                case DbType.UserCenterWrite:
                case DbType.UserCenter:
                    return BaseSystemInfo.UserCenterDbType;
                case DbType.Business:
                    return BaseSystemInfo.BusinessDbType;
                case DbType.Message:
                    return BaseSystemInfo.MessageDbType;
                case DbType.WorkFlow:
                    return BaseSystemInfo.WorkFlowDbType;
                case DbType.LogonLog:
                    return BaseSystemInfo.LogonLogDbType;
            }
        }

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
		private static string GetDbConnection(DbType dbType)
        {
            switch (dbType)
            {
                default:
                case DbType.UserCenterRead:
                    return BaseSystemInfo.UserCenterReadDbConnection;
                case DbType.UserCenterWrite:
                    return BaseSystemInfo.UserCenterWriteDbConnection;
                case DbType.UserCenter:
                    return BaseSystemInfo.UserCenterDbConnection;
                case DbType.Business:
                    return BaseSystemInfo.BusinessDbConnection;
                case DbType.Message:
                    return BaseSystemInfo.MessageDbConnection;
                case DbType.WorkFlow:
                    return BaseSystemInfo.WorkFlowDbConnection;
                case DbType.LogonLog:
                    return BaseSystemInfo.LogonLogDbConnection;
            }
        }
        /// <summary>
        /// ProcessDb
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="fun"></param>
        /// <param name="dbType"></param>
        /// <param name="inTransaction"></param>
        private static void ProcessDb(ServiceInfo parameter, ProcessFun fun, DbType dbType, bool inTransaction = false)
        {
            var milliStart = Begin(parameter.UserInfo, parameter.CurrentMethod);

            ProcessDbHelp(parameter, fun, dbType, inTransaction);

            End(parameter.UserInfo, milliStart, parameter.CurrentMethod);
        }
        /// <summary>
        /// ProcessDb
        /// </summary>
        /// <param name="serviceInfo"></param>
        /// <param name="processFun"></param>
        /// <param name="dbType"></param>
        /// <param name="inTransaction"></param>
        private static void ProcessDbHelp(ServiceInfo serviceInfo, ProcessFun processFun, DbType dbType, bool inTransaction)
        {
            // 2016-02-14 吉日嘎拉 增加耗时记录功能
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            using (var dbHelper = DbHelperFactory.Create(GetDbType(dbType), GetDbConnection(dbType)))
            {
                try
                {
                    // dbHelper.Open(GetDbConnection(dbType));
                    if (inTransaction)
                    {
                        // dbHelper.BeginTransaction();
                    }
                    processFun(dbHelper);
                    stopwatch.Stop();
                    serviceInfo.ElapsedTicks = stopwatch.ElapsedTicks;
                    AddLog(serviceInfo);
                    if (inTransaction)
                    {
                        // dbHelper.CommitTransaction();
                    }
                }
                catch (Exception ex)
                {
                    if (inTransaction)
                    {
                        // dbHelper.RollbackTransaction();
                    }
                    BaseExceptionManager.LogException(dbHelper, serviceInfo.UserInfo, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// 2016-02-14 吉日嘎拉 增加服务器调用耗时统计功能。
        /// </summary>
        /// <param name="serviceInfo"></param>
        private static void AddLog(ServiceInfo serviceInfo)
        {
            if (serviceInfo.RecordLog)
            {
                // 若用户信息没有，就获取现在的用户信息
                if (serviceInfo.UserInfo == null)
                {
                    serviceInfo.UserInfo = BaseSystemInfo.UserInfo;
                }

                // 本地直接写入数据库
                BaseLogManager.AddLog(serviceInfo);
            }
        }
        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="currentMethod"></param>
        /// <returns></returns>
		private static int Begin(BaseUserInfo userInfo, MethodBase currentMethod)
        {
            var milliStart = 0;

            // 写入调试信息
#if (DEBUG)
                milliStart = BaseUtil.StartDebug(userInfo, currentMethod);
#endif

            milliStart = BaseUtil.StartDebug(userInfo, currentMethod);

            return milliStart;
        }
        /// <summary>
        /// 结束
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="milliStart"></param>
        /// <param name="currentMethod"></param>
        private static void End(BaseUserInfo userInfo, int milliStart, MethodBase currentMethod)
        {
            // 写入调试信息
#if (DEBUG)
				BaseUtil.EndDebug(userInfo, currentMethod, milliStart);
#endif
        }
        /// <summary>
        /// 结束
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="milliStart"></param>
        /// <param name="currentMethod"></param>
        /// <param name="getOnline"></param>
        private static void End(BaseUserInfo userInfo, int milliStart, MethodBase currentMethod, bool getOnline)
        {
            // 写入调试信息
#if (DEBUG)
			if(getOnline)
			{
				BaseUtil.EndDebug(userInfo, currentMethod, milliStart);
			}
#endif
        }
    }
}
