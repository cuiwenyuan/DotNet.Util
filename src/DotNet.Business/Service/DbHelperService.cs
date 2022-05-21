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
    using IService;

    /// <summary>
    /// DbHelperService
    /// 执行传入的SQL语句
    /// 
    /// 修改记录
    /// 
    ///		2015.04.30 版本：3.0 JiRiGaLa 加强QL语句安全漏洞。
    ///		2011.05.07 版本：2.3 JiRiGaLa 改进为虚类。
    ///		2007.08.15 版本：2.2 JiRiGaLa 改进运行速度采用 WebService 变量定义 方式处理数据。
    ///		2007.08.14 版本：2.1 JiRiGaLa 改进运行速度采用 Instance 方式处理数据。
    ///		2007.07.10 版本：1.0 JiRiGaLa 数据库访问类。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.04.30</date>
    /// </author> 
    /// </summary>


    public class DbHelperService : IDbHelperService
    {
        /// <summary>
        /// 服务数据库连接
        /// </summary>
        public string ServiceDbConnection = BaseSystemInfo.BusinessDbConnection;
        /// <summary>
        /// 服务数据库类型
        /// </summary>
        public CurrentDbType ServiceDbType = BaseSystemInfo.BusinessDbType;
        /// <summary>
        /// 构造函数
        /// </summary>
        public DbHelperService()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbConnection"></param>
        public DbHelperService(string dbConnection)
        {
            ServiceDbConnection = dbConnection;
        }

        private void InsertDebugInfo(BaseUserInfo userInfo, string commandText)
        {
            // 写入调试信息
#if (DEBUG)
            Console.WriteLine(" User: " + userInfo.RealName + " commandText: " + commandText);
#endif

            // 加强安全验证防止未授权匿名调用
#if (!DEBUG)
            LogonService.UserIsLogon(null, userInfo);
#endif
        }

        private IDbHelper DbHelper
        {
            get
            {
                var dbHelper = DbHelperFactory.Create(ServiceDbType, ServiceDbConnection);
                return dbHelper;
            }
        }
        /// <summary>
        /// 获取数据库时间
        /// </summary>
        /// <returns></returns>
        public string GetDbDateTime()
        {
            return DbHelper.GetDbDateTime();
        }

        #region public int ExecuteNonQuery(BaseUserInfo userInfo, string commandText, CommandType commandType = CommandType.Text)
        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="commandText">语句</param>
        /// <param name="commandType">语句类型</param>
        /// <returns></returns>
        public int ExecuteNonQuery(BaseUserInfo userInfo, string commandText, string commandType = "Text")
        {
            var result = 0;
            // 判断是否已经登录的用户？
            var userManager = new BaseUserManager(userInfo);
            // 判断是否已经登录的用户？
            if (userManager.UserIsLogon(userInfo))
            {
                result = ExecuteNonQuery(userInfo, commandText, null, commandType);
            }
            return result;
        }
        #endregion

        #region public int ExecuteNonQuery(BaseUserInfo userInfo, string commandText, IDbDataParameter[] dbParameters, string commandType = CommandType.Text)
        /// <summary>
        /// 执行sql 带参数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="commandText">语句</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="commandType">语句类型</param>
        /// <returns></returns>
        public int ExecuteNonQuery(BaseUserInfo userInfo, string commandText, List<KeyValuePair<string, object>> dbParameters, string commandType = "Text")
        {
            InsertDebugInfo(userInfo, commandText);

            var result = 0;
            // 判断是否已经登录的用户？
            var userManager = new BaseUserManager(userInfo);
            // 判断是否已经登录的用户？
            if (userManager.UserIsLogon(userInfo))
            {
                result = DbHelper.ExecuteNonQuery(commandText, DbHelper.MakeParameters(dbParameters), DbUtil.GetCommandType(commandType));
            }
            return result;
        }
        #endregion

        #region public object ExecuteScalar(BaseUserInfo userInfo, string commandText, string commandType = CommandType.Text)
        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="commandText">语句</param>
        /// <param name="commandType">语句类型</param>
        /// <returns></returns>
        public object ExecuteScalar(BaseUserInfo userInfo, string commandText, string commandType = "Text")
        {
            object result = null;
            // 判断是否已经登录的用户？
            var userManager = new BaseUserManager(userInfo);
            // 判断是否已经登录的用户？
            if (userManager.UserIsLogon(userInfo))
            {
                result = ExecuteScalar(userInfo, commandText, null, commandType);
            }
            return result;
        }
        #endregion

        #region public object ExecuteScalar(BaseUserInfo userInfo, string commandText, IDbDataParameter[] dbParameters, string commandType = CommandType.Text)
        /// <summary>
        /// 执行sql 带参数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="commandText">语句</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="commandType">语句类型</param>
        /// <returns></returns>
        public object ExecuteScalar(BaseUserInfo userInfo, string commandText, List<KeyValuePair<string, object>> dbParameters, string commandType = "Text")
        {
            InsertDebugInfo(userInfo, commandText);

            object result = null;
            // 判断是否已经登录的用户？
            var userManager = new BaseUserManager(userInfo);
            // 判断是否已经登录的用户？
            if (userManager.UserIsLogon(userInfo))
            {
                result = DbHelper.ExecuteScalar(commandText, DbHelper.MakeParameters(dbParameters), DbUtil.GetCommandType(commandType));
            }
            return result;
        }
        #endregion

        #region public DataTable Fill(BaseUserInfo userInfo, string commandText, CommandType commandType = CommandType.Text)
        /// <summary>
        /// 执行Sql 返回DataTable
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="commandText">语句</param>
        /// <param name="commandType">语句类型</param>
        /// <returns></returns>
        public DataTable Fill(BaseUserInfo userInfo, string commandText, string commandType = "Text")
        {
            DataTable result = null;
            // 判断是否已经登录的用户？
            var userManager = new BaseUserManager(userInfo);
            // 判断是否已经登录的用户？
            if (userManager.UserIsLogon(userInfo))
            {
                result = Fill(userInfo, commandText, null, commandType);
            }
            return result;
        }
        #endregion

        #region public DataTable Fill(BaseUserInfo userInfo, string commandText, IDbDataParameter[] dbParameters, CommandType commandType = CommandType.Text)
        /// <summary>
        /// 执行Sql 返回DataTable
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="commandText">语句</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="commandType">语句类型</param>
        /// <returns></returns>
        public DataTable Fill(BaseUserInfo userInfo, string commandText, List<KeyValuePair<string, object>> dbParameters, string commandType = "Text")
        {
            InsertDebugInfo(userInfo, commandText);

            DataTable result = null;
            // 判断是否已经登录的用户？
            var userManager = new BaseUserManager(userInfo);
            // 判断是否已经登录的用户？
            if (userManager.UserIsLogon(userInfo))
            {
                result = DbHelper.Fill(commandText, DbHelper.MakeParameters(dbParameters), DbUtil.GetCommandType(commandType));
            }
            return result;
        }
        #endregion

        #region IDbHelperService 成员
        /// <summary>
        /// SetBackParamValue
        /// </summary>
        /// <param name="sqlExecute"></param>
        /// <param name="dbparams"></param>
        private void SetBackParamValue(SqlExecute sqlExecute, IDbDataParameter[] dbparams)
        {
            for (var i = 0; i <= dbparams.Length - 1; i++)
            {
                if (dbparams[i].Direction != ParameterDirection.Input)
                {
                    sqlExecute.SetValueAt(i, dbparams[i].Value);
                }
            }
        }
        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="sqlExecute"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(BaseUserInfo userInfo, ref SqlExecute sqlExecute)
        {
            InsertDebugInfo(userInfo, sqlExecute.CommandText);

            var result = 0;
            // 判断是否已经登录的用户？
            var userManager = new BaseUserManager(userInfo);
            // 判断是否已经登录的用户？
            if (userManager.UserIsLogon(userInfo))
            {
                var dbHelper = DbHelper;
                var dbparams = sqlExecute.GetParameters(dbHelper);
                result = dbHelper.ExecuteNonQuery(sqlExecute.CommandText, dbparams, sqlExecute.CommandType);
                SetBackParamValue(sqlExecute, dbparams);
            }
            return result;
        }
        /// <summary>
        /// ExecuteScalar
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="sqlExecute"></param>
        /// <returns></returns>
        public object ExecuteScalar(BaseUserInfo userInfo, ref SqlExecute sqlExecute)
        {
            InsertDebugInfo(userInfo, sqlExecute.CommandText);

            object result = 0;
            // 判断是否已经登录的用户？
            var userManager = new BaseUserManager(userInfo);
            // 判断是否已经登录的用户？
            if (userManager.UserIsLogon(userInfo))
            {
                var dbHelper = DbHelper;
                var dbparams = sqlExecute.GetParameters(dbHelper);
                result = dbHelper.ExecuteScalar(sqlExecute.CommandText, dbparams, sqlExecute.CommandType);
                SetBackParamValue(sqlExecute, dbparams);
            }
            return result;
        }
        /// <summary>
        /// Fill
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="sqlExecute"></param>
        /// <returns></returns>
        public DataTable Fill(BaseUserInfo userInfo, ref SqlExecute sqlExecute)
        {
            InsertDebugInfo(userInfo, sqlExecute.CommandText);

            DataTable result = null;
            // 判断是否已经登录的用户？
            var userManager = new BaseUserManager(userInfo);
            // 判断是否已经登录的用户？
            if (userManager.UserIsLogon(userInfo))
            {
                var dbHelper = DbHelper;
                var dbparams = sqlExecute.GetParameters(dbHelper);
                result = dbHelper.Fill(sqlExecute.CommandText, dbparams, sqlExecute.CommandType);
                SetBackParamValue(sqlExecute, dbparams);
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 获取分页数据（防注入功能的）
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="recordCount">记录条数</param>
        /// <param name="tableName">数据来源表名</param>
        /// <param name="selectField">选择字段</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示多少条</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="dbParameters">查询参数</param>
        /// <param name="orderBy">排序字段</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(BaseUserInfo userInfo, out int recordCount, string tableName, string selectField, int pageNo, int pageSize, string conditions, List<KeyValuePair<string, object>> dbParameters, string orderBy)
        {
            DataTable result = null;
            // 判断是否已经登录的用户？
            var userManager = new BaseUserManager(userInfo);
            recordCount = 0;
            // 判断是否已经登录的用户？
            if (userManager.UserIsLogon(userInfo))
            {
                if (SecretUtil.IsSqlSafe(conditions))
                {
                    recordCount = DbHelper.GetCount(tableName, conditions, DbHelper.MakeParameters(dbParameters));
                    result = DbUtil.GetDataTableByPage(DbHelper, tableName, selectField, pageNo, pageSize, conditions, DbHelper.MakeParameters(dbParameters), orderBy);
                }
                else
                {
                    // 记录注入日志
                    LogUtil.WriteLog("userInfo:" + userInfo.Serialize() + " " + conditions, "SqlSafe");
                }
            }
            return result;
        }
    }
}