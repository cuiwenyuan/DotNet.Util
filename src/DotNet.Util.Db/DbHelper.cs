//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace DotNet.Util
{
    /// <summary>
    /// DbHelper
    /// 数据库访问层基础类。
    /// 
    /// 修改记录
    ///     
    ///     2013.02.04 版本：3.3 JiRiGaLa 解决并发问题。
    ///     2011.02.20 版本：3.2 JiRiGaLa 重新排版代码。
    ///     2011.01.29 版本：3.1 JiRiGaLa 实现IDisposable接口。
    ///     2010.06.13 版本：3.0 JiRiGaLa 改进为支持静态方法，不用数据库Open、Close的方式，AutoOpenClose开关。
    ///		2010.03.14 版本：2.0 JiRiGaLa 无法彻底释放、并发时出现异常问题解决。
    ///		2009.11.25 版本：1.0 JiRiGaLa 改进ConnectionString。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.02.20</date>
    /// </author>
    /// </summary>
    public abstract partial class DbHelper : IDbHelper
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public virtual string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// 数据库连接名
        /// </summary>
        public virtual string ConnectionName { get; set; } = string.Empty;

        #region public virtual CurrentDbType CurrentDbType 获取当前数据库类型
        /// <summary>
        /// 获取当前数据库类型
        /// </summary>
        public virtual CurrentDbType CurrentDbType => CurrentDbType.SqlServer;

        #endregion

        #region public virtual DbConnection DbConnection 数据库连接必要条件参数
        /// <summary>
        /// 数据库连接
        /// </summary>
        private DbConnection _dbConnection = null;
        /// <summary>
        /// 数据库连接
        /// </summary>
        public virtual DbConnection DbConnection
        {
            get
            {
                if (_dbConnection != null) return _dbConnection;
                Open();
                return _dbConnection;

            }
            set => _dbConnection = value;
        }

        /// <summary>
        /// 命令
        /// </summary>
        protected DbCommand DbCommand { get; set; } = null;

        // 数据库适配器
        /// <summary>
        /// 数据库适配器
        /// </summary>
        protected DbDataAdapter DbDataAdapter { get; set; } = null;

        /// <summary>
        /// 数据库事务
        /// </summary>
        private DbTransaction _dbTransaction = null;

        /// <summary>
        /// 是否已在事务之中
        /// </summary>
        public virtual bool InTransaction { get; set; } = false;

        /// <summary>
        /// 日志文件名
        /// </summary>
        public virtual string FileName { get; set; } = "DbHelper.log";

        /// <summary>
        /// 数据库版本
        /// </summary>
        public virtual string ServerVersion { get; set; } = string.Empty;

        /// <summary>
        /// 默认打开关闭数据库连接选项（默认为否）
        /// </summary>
        public virtual bool MustCloseConnection { get; set; } = false;

        private DbProviderFactory _dbProviderFactory = null;
        /// <summary>
        /// DbProviderFactory实例
        /// </summary>
        public virtual DbProviderFactory GetInstance()
        {
            if (_dbProviderFactory == null)
            {
                _dbProviderFactory = DbHelperFactory.Create(CurrentDbType, ConnectionString).GetInstance();
            }
            //// 懒汉式，双重校验锁，只在第一次创建实例时加锁，提高访问性能 2022-05-10 Troy.Cui
            //if (_dbProviderFactory != null) return _dbProviderFactory;
            //lock (this)
            //{
            //    if (_dbProviderFactory != null) return _dbProviderFactory;
            //    _dbProviderFactory = DbHelperFactory.Create(CurrentDbType, ConnectionString).GetInstance();
            //}

            return _dbProviderFactory;
        }
        #endregion

        #region public virtual string SqlSafe(string value) 检查参数的安全性
        /// <summary>
        /// 检查参数的安全性
        /// </summary>
        /// <param name="value">参数</param>
        /// <returns>安全的参数</returns>
        public virtual string SqlSafe(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace("'", "''");
            }
            return value;
        }
        #endregion

        #region public virtual string PlusSign() 获得Sql字符串相加符号
        /// <summary>
        ///  获得Sql字符串相加符号
        /// </summary>
        /// <returns>字符加</returns>
        public virtual string PlusSign()
        {
            return " + ";
        }
        #endregion

        #region string virtual PlusSign(params string[] values) 获得Sql字符串相加符号
        /// <summary>
        ///  获得Sql字符串相加符号
        /// </summary>
        /// <param name="values">参数值</param>
        /// <returns>字符加</returns>
        public virtual string PlusSign(params string[] values)
        {
            var result = string.Empty;
            foreach (var t in values)
            {
                result += t + PlusSign();
            }
            if (!string.IsNullOrEmpty(result))
            {
                result = result.Substring(0, result.Length - 3);
            }
            else
            {
                result = PlusSign();
            }
            return result;
        }
        #endregion

        #region public virtual IDbConnection Open() 获取数据库连接的方法
        /// <summary>
        /// 这时主要的获取数据库连接的方法
        /// </summary>
        /// <returns>数据库连接</returns>
        public virtual IDbConnection Open()
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
            Open(ConnectionString);
            return _dbConnection;
        }
        #endregion

        #region public virtual IDbConnection Open(string connectionString) 获得新的数据库连接
        /// <summary>
        /// 获得新的数据库连接
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <returns>数据库连接</returns>
        public virtual IDbConnection Open(string connectionString)
        {
            // 若是空的话才打开，不可以，每次应该打开新的数据库连接才对，这样才能保证不是一个数据库连接上执行的
            ConnectionString = connectionString;
            _dbConnection = GetInstance().CreateConnection();
            if (_dbConnection != null)
            {
                _dbConnection.ConnectionString = ConnectionString;
                try
                {
                    _dbConnection.Open();
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

        #region public virtual IDbConnection GetDbConnection() 获取数据库连接
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns>数据库连接</returns>
        public virtual IDbConnection GetDbConnection()
        {
            return _dbConnection;
        }
        #endregion

        #region public virtual IDbConnection GetDbConnection(string connectionString) 获取数据库连接
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <returns>数据库连接</returns>
        public virtual IDbConnection GetDbConnection(string connectionString)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                Open(connectionString);
            }
            return _dbConnection;
        }
        #endregion

        #region public virtual IDbTransaction GetDbTransaction() 获取数据源上执行的事务
        /// <summary>
        /// 获取数据源上执行的事务
        /// </summary>
        /// <returns>数据源上执行的事务</returns>
        public virtual IDbTransaction GetDbTransaction()
        {
            return _dbTransaction;
        }
        #endregion

        #region public virtual IDbCommand GetDbCommand() 获取数据源上命令
        /// <summary>
        /// 获取数据源上命令
        /// </summary>
        /// <returns>数据源上命令</returns>
        public virtual IDbCommand GetDbCommand()
        {
            return DbConnection.CreateCommand();
        }
        #endregion

        #region public virtual IDbTransaction BeginTransaction() 事务开始
        /// <summary>
        /// 事务开始
        /// </summary>
        /// <returns>事务</returns>
        public virtual IDbTransaction BeginTransaction()
        {
            // 写入调试信息
            if (!InTransaction)
            {
                if (DbConnection.State == ConnectionState.Closed)
                {
                    DbConnection.Open();
                }
                // 这里是不允许自动关闭了
                _dbTransaction = DbConnection.BeginTransaction();
                MustCloseConnection = false;
                InTransaction = true;
                // dbCommand.Transaction = dbTransaction;
            }
            return _dbTransaction;
        }
        #endregion

        #region public virtual void CommitTransaction() 提交事务
        /// <summary>
        /// 提交事务
        /// </summary>
        public virtual void CommitTransaction()
        {
            if (InTransaction)
            {
                // 事务已经完成了，一定要更新标志信息
                InTransaction = false;
                MustCloseConnection = true;
                _dbTransaction.Commit();
                //释放掉 Troy.Cui 2018.07.02
                _dbTransaction.Dispose();
            }
        }
        #endregion

        #region public virtual void RollbackTransaction() 回滚事务
        /// <summary>
        /// 回滚事务
        /// </summary>
        public virtual void RollbackTransaction()
        {
            if (InTransaction)
            {
                InTransaction = false;
                _dbTransaction.Rollback();
                //释放掉 Troy.Cui 2018.07.02
                _dbTransaction.Dispose();
            }
        }
        #endregion

        #region public virtual void Close() 关闭数据库连接
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public virtual void Close()
        {
            if (_dbConnection != null)
            {
                _dbConnection.Close();
                _dbConnection.Dispose();
#if (DEBUG)
                Trace.WriteLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeLongFormat) + " :DbConnection Close: " + DbConnection.Database + " ,ThreadId: " + Thread.CurrentThread.ManagedThreadId);
#endif
            }
            //Troy Cui 2018.01.02启用，解决应用程序池的问题
            Dispose();
        }
        #endregion        

        #region public virtual void Dispose() 内存回收
        /// <summary>
        /// 内存回收
        /// </summary>
        public virtual void Dispose()
        {
            if (DbCommand != null)
            {
#if (DEBUG)
                Trace.WriteLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeLongFormat) + " :DbCommand Dispose: " + DbCommand + " ,ThreadId: " + Thread.CurrentThread.ManagedThreadId);
#endif
                DbCommand.Dispose();
                DbCommand = null;
            }
            if (DbDataAdapter != null)
            {
#if (DEBUG)
                Trace.WriteLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeLongFormat) + " :DbDataAdapter Dispose: " + DbDataAdapter + " ,ThreadId: " + Thread.CurrentThread.ManagedThreadId);
#endif
                DbDataAdapter.Dispose();
                DbDataAdapter = null;
            }
            if (_dbTransaction != null && !InTransaction)
            {
#if (DEBUG)
                Trace.WriteLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeLongFormat) + " :_dbTransaction Dispose: " + _dbTransaction + " ,ThreadId: " + Thread.CurrentThread.ManagedThreadId);
#endif
                _dbTransaction.Dispose();
                _dbTransaction = null;
            }
            // 关闭数据库连接
            if (_dbConnection != null)
            {
#if (DEBUG)
                Trace.WriteLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeLongFormat) + " :_dbConnection Dispose: " + _dbConnection.Database + " State " + _dbConnection.State + " ,ThreadId: " + Thread.CurrentThread.ManagedThreadId);
#endif
                if (_dbConnection.State != ConnectionState.Closed)
                {
                    _dbConnection.Close();                    
                }
                _dbConnection.Dispose();
                _dbConnection = null;
            }            
        }
        #endregion

        #region public virtual void SqlBulkCopyData(DataTable dt) 利用Net SqlBulkCopy 批量导入数据库,速度超快

        /// <summary>
        /// 利用Net SqlBulkCopy 批量导入数据库,速度超快
        /// </summary>
        /// <param name="dt">源内存数据表（先通过SELECT TOP 0获取空白DataTable）</param>
        /// <param name="destinationTableName">目标表名</param>
        /// <param name="bulkCopyTimeout">超时限制，默认为30秒</param>
        /// <param name="batchSize">批大小（默认0，即一次性导入）</param>
        public virtual bool SqlBulkCopyData(DataTable dt, string destinationTableName, int bulkCopyTimeout = 30, int batchSize = 0)
        {
            var result = false;
            // 各自数据集需要自行覆盖实现此处逻辑
            return result;
        }
        #endregion        
    }
}