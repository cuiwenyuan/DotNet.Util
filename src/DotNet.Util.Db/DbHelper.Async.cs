//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;

namespace DotNet.Util
{
    /// <summary>
    /// DbHelper
    /// 数据库访问层基础类。
    /// 
    /// 修改记录
    ///     
    ///     2023.04.12 版本：3.3 Troy.Cui 创建。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2023.04.12</date>
    /// </author>
    /// </summary>
    public abstract partial class DbHelper : IDbHelper
    {
        #region public virtual IDbConnection OpenAsync() 获取数据库连接的方法
        /// <summary>
        /// 这时主要的获取数据库连接的方法
        /// </summary>
        /// <returns>数据库连接</returns>
        public virtual IDbConnection OpenAsync()
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                BaseConfiguration.GetSetting();
                // 默认打开业务数据库，而不是用户中心的数据库
                // 读取不到，就用用户中心数据库
                if (string.IsNullOrEmpty(BaseSystemInfo.BusinessDbConnection))
                {
                    ConnectionString = BaseSystemInfo.UserCenterDbConnection;
                }
                else
                {
                    ConnectionString = BaseSystemInfo.BusinessDbConnection;
                }
            }
            OpenAsync(ConnectionString);
            return _dbConnection;
        }
        #endregion

        #region public virtual IDbConnection OpenAsync(string connectionString) 获得新的数据库连接
        /// <summary>
        /// 获得新的数据库连接
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <returns>数据库连接</returns>
        public virtual IDbConnection OpenAsync(string connectionString)
        {
            //若是空的话才打开，不可以，每次应该打开新的数据库连接才对，这样才能保证不是一个数据库连接上执行的
            ConnectionString = connectionString;
            _dbConnection = GetInstance().CreateConnection();
            //var dbConnection = _dbConnection;
            if (_dbConnection != null)
            {
                _dbConnection.ConnectionString = ConnectionString;
                try
                {
                    _dbConnection.OpenAsync();
                }
                catch (Exception e)
                {
                    LogUtil.WriteException(e, "open connection error");
                }
                if (_dbConnection.State == ConnectionState.Open)
                {
                    ServerVersion = _dbConnection.ServerVersion;
                }
                return _dbConnection;
            }
            return null;
        }
        #endregion

        #region public virtual IDbConnection GetDbConnectionAsync() 获取数据库连接
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns>数据库连接</returns>
        public virtual IDbConnection GetDbConnectionAsync()
        {
            return _dbConnection;
        }
        #endregion

        #region public virtual IDbConnection GetDbConnectionAsync(string connectionString) 获取数据库连接
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <returns>数据库连接</returns>
        public virtual IDbConnection GetDbConnectionAsync(string connectionString)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                OpenAsync(connectionString);
            }
            return _dbConnection;
        }
        #endregion
    }
}