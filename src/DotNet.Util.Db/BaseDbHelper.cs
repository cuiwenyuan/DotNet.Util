//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;

namespace DotNet.Util
{
    /// <summary>
    /// BaseDbHelper
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
    public abstract partial class BaseDbHelper : IDbHelper
    {
        #region public virtual CurrentDbType CurrentDbType 获取当前数据库类型
        /// <summary>
        /// 获取当前数据库类型
        /// </summary>
        public virtual CurrentDbType CurrentDbType => CurrentDbType.SqlServer;

        #endregion

        #region private DbConnection DbConnection 数据库连接必要条件参数
        // 数据库连接
        private DbConnection _dbConnection = null;
        /// <summary>
        /// 数据库连接
        /// </summary>
        public DbConnection DbConnection
        {
            get
            {
                if (_dbConnection == null)
                {
                    //若没打开，就变成自动打开关闭的
                    Open();
                    MustCloseConnection = true;
                }
                return _dbConnection;
            }
            set => _dbConnection = value;
        }

        // 命令
        /// <summary>
        /// 命令
        /// </summary>
        public DbCommand DbCommand { get; set; } = null;

        // 数据库适配器
        /// <summary>
        /// 数据库适配器
        /// </summary>
        public DbDataAdapter DbDataAdapter { get; set; } = null;

        // 数据库连接
        /// <summary>
        /// 数据库连接
        /// </summary>
        public string ConnectionString { get; set; } = "Data Source=localhost;Initial Catalog=UserCenterV4;Integrated Security=SSPI;";

        private DbTransaction _dbTransaction = null;

        /// <summary>
        /// 是否已在事务之中
        /// </summary>
        public bool InTransaction { get; set; } = false;

        /// <summary>
        /// 日志文件名
        /// </summary>
        public string FileName = "BaseDbHelper.txt";    // sql查询句日志

        /// <summary>
        /// 默认打开关闭数据库选项（默认为否）
        /// </summary>
        public bool MustCloseConnection { get; set; } = false;

        private DbProviderFactory _dbProviderFactory = null;
        /// <summary>
        /// DbProviderFactory实例
        /// </summary>
        public virtual DbProviderFactory GetInstance()
        {
            //if (_dbProviderFactory == null)
            //{
            //    _dbProviderFactory = DbHelperFactory.Create().GetInstance();
            //}
            // 懒汉式，双重校验锁，只在第一次创建实例时加锁，提高访问性能 2022-05-10 Troy.Cui
            if (_dbProviderFactory != null) return _dbProviderFactory;
            lock (this)
            {
                if (_dbProviderFactory != null) return _dbProviderFactory;

                _dbProviderFactory = DbHelperFactory.Create(CurrentDbType, ConnectionString).GetInstance();
            }

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

        #region string PlusSign(params string[] values) 获得Sql字符串相加符号
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
            // 这里是获取一个连接的详细方法
            if (string.IsNullOrEmpty(ConnectionString))
            {
                // 默认打开业务数据库，而不是用户中心的数据库
                if (string.IsNullOrEmpty(BaseSystemInfo.BusinessDbConnection))
                {
                    BaseConfiguration.GetSetting();
                }
                //读取不到，就用用户中心数据库
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
            // 这里数据库连接打开的时候，就判断注册属性的有效性
            if (!SecretUtil.CheckRegister())
            {
                // 若没有进行注册，让程序无法打开数据库比较好。
                connectionString = string.Empty;

                // 抛出异常信息显示给客户
                throw new Exception(BaseSystemInfo.RegisterException);
            }

            //若是空的话才打开，不可以，每次应该打开新的数据库连接才对，这样才能保证不是一个数据库连接上执行的
            ConnectionString = connectionString;
            _dbConnection = GetInstance().CreateConnection();
            var dbConnection = _dbConnection;
            if (dbConnection != null)
            {
                dbConnection.ConnectionString = ConnectionString;
                dbConnection.Open();
                //MustCloseConnection = false;
                //修改为必须关闭 Troy.Cui 2018.07.02
                MustCloseConnection = true;
                return dbConnection;
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

        #region public IDbTransaction BeginTransaction() 事务开始
        /// <summary>
        /// 事务开始
        /// </summary>
        /// <returns>事务</returns>
        public IDbTransaction BeginTransaction()
        {
            // 写入调试信息
            if (!InTransaction)
            {
                if (DbConnection.State == ConnectionState.Closed)
                {
                    DbConnection.Open();
                }
                else if (DbConnection.State == ConnectionState.Broken)
                {
                    DbConnection.Close();
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

        #region public void CommitTransaction() 提交事务
        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTransaction()
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

        #region public void RollbackTransaction() 回滚事务
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTransaction()
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

        #region public void Close() 关闭数据库连接
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void Close()
        {
            if (_dbConnection != null)
            {
                _dbConnection.Close();
                //Troy Cui 2018.01.02启用，解决应用程序池的问题
                _dbConnection.Dispose();
            }
            //Troy Cui 2018.01.02启用，解决应用程序池的问题
            Dispose();
        }
        #endregion

        #region public virtual void WriteLog(string commandText, string fileName = null) 写入sql查询句日志

        /// <summary>
        /// 写入sql查询句日志
        /// </summary>
        /// <param name="commandText"></param>
        public virtual void WriteLog(string commandText)
        {
            var fileName = DateTime.Now.ToString(BaseSystemInfo.DateFormat) + "_" + DateTime.Now.Hour + "_" + FileName + "_0.log";
            WriteLog(commandText, fileName);
        }

        /// <summary>
        /// 写入sql查询句日志
        /// </summary>
        /// <param name="commandText">异常</param>
        /// <param name="fileName">文件名</param>
        private void WriteLog(string commandText, string fileName)
        {
            // 系统里应该可以配置是否记录异常现象
            if (!BaseSystemInfo.LogSql)
            {
                return;
            }
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = DateTime.Now.ToString(BaseSystemInfo.DateFormat) + " _ " + FileName;
            }
            var logDirectory = BaseSystemInfo.StartupPath + @"\\Log\\Query";
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            FileLogUtil.WriteLog(logDirectory, fileName, commandText, "log");

        }
        #endregion

        #region public void Dispose() 内存回收
        /// <summary>
        /// 内存回收
        /// </summary>
        public void Dispose()
        {
            if (DbCommand != null)
            {
                DbCommand.Dispose();
            }
            if (DbDataAdapter != null)
            {
                DbDataAdapter.Dispose();
            }
            if (_dbTransaction != null)
            {
                _dbTransaction.Dispose();
            }
            // 关闭数据库连接
            if (_dbConnection != null)
            {
                if (_dbConnection.State != ConnectionState.Closed)
                {
                    _dbConnection.Close();
                    _dbConnection.Dispose();
                }
            }
            _dbConnection = null;
        }
        #endregion
    }
}