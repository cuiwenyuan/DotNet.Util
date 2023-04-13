//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
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
    ///     2017.11.05 版本：1.1 Troy Cui 所有方法增加：记录日志
    ///     2016.11.13 版本：1.1 LiuHaiyang 所有使用到自动关闭连接的地方，都加上try{}finally{}，在finally里关闭连接，
    ///                                     防止出现异常时，连接不能自动关闭，导致异常多时连接池占满。
    ///		2013.02.04 版本：1.0 JiRiGaLa 分离改进。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2013.02.04</date>
    /// </author> 
    /// </summary>
    public abstract partial class DbHelper : IDbHelper
    {
        #region public virtual IDataReader ExecuteReader(string commandText) 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <returns>结果集流</returns>
        public virtual IDataReader ExecuteReader(string commandText)
        {
            return ExecuteReader(commandText, (IDbDataParameter[])null, CommandType.Text);
        }
        #endregion

        #region public virtual IDataReader ExecuteReader(string commandText, IDbDataParameter[] dbParameters); 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>结果集流</returns>
        public virtual IDataReader ExecuteReader(string commandText, IDbDataParameter[] dbParameters)
        {
            return ExecuteReader(commandText, dbParameters, CommandType.Text);
        }
        #endregion

        #region public virtual IDataReader ExecuteReader(string commandText, IDbDataParameter[] dbParameters, CommandType commandType) 执行查询

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandType">命令分类</param>
        /// <returns>结果集流</returns>
        public virtual IDataReader ExecuteReader(string commandText, IDbDataParameter[] dbParameters, CommandType commandType)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            //自动打开
            MustCloseConnection = false;
            if (DbConnection == null)
            {
                Open();
            }

            DbDataReader dbDataReader = null;
            if (DbConnection.State == ConnectionState.Closed)
            {
                DbConnection.Open();
            }
            else if (DbConnection.State == ConnectionState.Broken)
            {
                DbConnection.Close();
                DbConnection.Open();
            }
            using (DbCommand = DbConnection.CreateCommand())
            {
                try
                {
#if (DEBUG)
                    Trace.WriteLine(DateTime.Now + " :DbConnection Start: " + DbConnection.Database + " ,ThreadId: " + Thread.CurrentThread.ManagedThreadId);
#endif
                    DbCommand.Connection = DbConnection;
                    DbCommand.CommandTimeout = DbConnection.ConnectionTimeout;
                    DbCommand.CommandText = commandText;
                    if (CurrentDbType == CurrentDbType.Oracle)
                    {
                        // 针对Oracle，全局替换换行符，避免报错或不执行
                        // 仅当前系统的换行符
                        DbCommand.CommandText = commandText.Replace(Environment.NewLine, " ");
                        // 各种平台的换行符
                        //_dbCommand.CommandText = commandText.Replace("r\n", " ").Replace('\n', ' ').Replace('\r', ' ');
                    }

                    DbCommand.CommandType = commandType;
                    if (_dbTransaction != null)
                    {
                        DbCommand.Transaction = _dbTransaction;
                        MustCloseConnection = false;
                    }

                    if (dbParameters != null)
                    {
                        DbCommand.Parameters.Clear();
                        foreach (var t in dbParameters)
                        {
                            if (t != null)
                            {
                                DbCommand.Parameters.Add(((ICloneable)t).Clone());
                            }
                        }
                    }

                    // 写入日志
                    SqlUtil.WriteLog(commandText, commandType.ToString(), dbParameters);
                    
                    if (DbConnection.State != ConnectionState.Open)
                    {
                        DbConnection.Open();
                    }
                    dbDataReader = DbCommand.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (Exception e)
                {
                    var sb = Pool.StringBuilder.Get();
                    sb.Append(commandText);
                    sb.Append(" ");
                    sb.Append(commandType.ToString());
                    if (dbParameters != null)
                    {
                        sb.Append(" dbParameters: ");
                        foreach (var parameter in dbParameters)
                        {
                            sb.Append(parameter.ParameterName + "=" + parameter.Value + " ");
                        }
                    }

                    LogUtil.WriteException(e, sb.Put());
                }
                finally
                {
                    WriteSql(DbCommand);

                    //因为使用了CommandBehavior.CloseConnection，不需要关闭连接
                    //一定不要自动关闭连接，但需要在dataReader.Read()之后手动执行dataReader.Close()
                    //if (MustCloseConnection)
                    //{
                    //    Close();
                    //}
                    if (DbCommand != null)
                    {
                        DbCommand.Parameters.Clear();
                    }
                }
                stopwatch.Stop();
                var statisticsText = $"Elapsed time: {stopwatch.Elapsed.TotalMilliseconds}ms";
                SqlUtil.WriteLog(commandText, commandType.ToString(), dbParameters, statisticsText);
                WriteSql(DbCommand, statisticsText);
                if (stopwatch.Elapsed.TotalMilliseconds >= BaseSystemInfo.SlowQueryMilliseconds)
                {
                    var sb = Pool.StringBuilder.Get();
                    sb.Append(commandText);
                    sb.Append(" ");
                    sb.Append(commandType.ToString());
                    if (dbParameters != null)
                    {
                        sb.Append(" dbParameters: ");
                        foreach (var parameter in dbParameters)
                        {
                            sb.Append(parameter.ParameterName + "=" + parameter.Value + " ");
                        }
                    }
                    else
                    {
                        sb.Append(" ");
                    }
                    sb.Append(statisticsText);
                    LogUtil.WriteLog(sb.Put(), "Slow.DbHelper.ExecuteReader");
                }
            }

            return dbDataReader;
        }
        #endregion

        #region public virtual int ExecuteNonQuery(string commandText) 执行查询, SQL BUILDER 用了这个东西？参数需要保存, 不能丢失.
        /// <summary>
        /// 执行查询, SQL BUILDER 用了这个东西？参数需要保存, 不能丢失.
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <returns>影响行数</returns>
        public virtual int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(_dbTransaction, commandText, (IDbDataParameter[])null, CommandType.Text);
        }
        #endregion

        #region public virtual int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters) 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>影响行数</returns>
        public virtual int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters)
        {
            return ExecuteNonQuery(_dbTransaction, commandText, dbParameters, CommandType.Text);
        }
        #endregion

        #region public virtual int ExecuteNonQuery(string commandText, CommandType commandType) 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="commandType">命令分类</param>
        /// <returns>影响行数</returns>
        public virtual int ExecuteNonQuery(string commandText, CommandType commandType)
        {
            return ExecuteNonQuery(_dbTransaction, commandText, (IDbDataParameter[])null, commandType);
        }
        #endregion

        #region public virtual int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters, CommandType commandType) 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandType">命令分类</param>
        /// <returns>影响行数</returns>
        public virtual int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters, CommandType commandType)
        {
            return ExecuteNonQuery(_dbTransaction, commandText, dbParameters, commandType);
        }
        #endregion

        #region public virtual int ExecuteNonQuery(IDbTransaction transaction, string commandText, IDbDataParameter[] dbParameters, CommandType commandType) 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandType">命令分类</param>
        /// <returns>影响行数</returns>
        public virtual int ExecuteNonQuery(IDbTransaction transaction, string commandText, IDbDataParameter[] dbParameters, CommandType commandType)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            // 自动关闭，以便返还给连接池
            MustCloseConnection = true;
            if (DbConnection == null)
            {
                Open();
            }
            if (DbConnection.State == ConnectionState.Closed)
            {
                DbConnection.Open();
            }
            else if (DbConnection.State == ConnectionState.Broken)
            {
                DbConnection.Close();
                DbConnection.Open();
            }
            var result = -1;
            using (DbCommand = DbConnection.CreateCommand())
            {
                try
                {
#if (DEBUG)
                    Trace.WriteLine(DateTime.Now + " :DbConnection Start: " + DbConnection.Database + " ,ThreadId: " + Thread.CurrentThread.ManagedThreadId);
#endif
                    DbCommand.Connection = DbConnection;
                    DbCommand.CommandTimeout = DbConnection.ConnectionTimeout;
                    DbCommand.CommandText = commandText;
                    if (CurrentDbType == CurrentDbType.Oracle)
                    {
                        // 针对Oracle，全局替换换行符，避免报错或不执行
                        // 仅当前系统的换行符
                        DbCommand.CommandText = commandText.Replace(Environment.NewLine, " ");
                        // 各种平台的换行符
                        //_dbCommand.CommandText = commandText.Replace("r\n", " ").Replace('\n', ' ').Replace('\r', ' ');
                    }

                    DbCommand.CommandType = commandType;
                    if (_dbTransaction != null)
                    {
                        DbCommand.Transaction = _dbTransaction;
                        MustCloseConnection = false;
                    }

                    if (dbParameters != null)
                    {
                        DbCommand.Parameters.Clear();
                        for (var i = 0; i < dbParameters.Length; i++)
                        {
                            // 启用非空判断 2022-05-10 Troy.Cui
                            //if (dbParameters[i] != null)
                            //{
                            DbCommand.Parameters.Add(((ICloneable)dbParameters[i]).Clone());
                            //}
                        }
                    }

                    //写入日志 
                    SqlUtil.WriteLog(commandText, commandType.ToString(), dbParameters);
                    
                    if (DbConnection.State != ConnectionState.Open)
                    {
                        DbConnection.Open();
                    }
                    result = DbCommand.ExecuteNonQuery();

                    if (CurrentDbType == CurrentDbType.SqlServer)
                    {
                        SetBackParamValue(dbParameters);
                    }
                }
                catch (Exception e)
                {
                    var sb = Pool.StringBuilder.Get();
                    sb.Append(commandText);
                    sb.Append(" ");
                    sb.Append(commandType.ToString());
                    if (dbParameters != null)
                    {
                        sb.Append(" dbParameters: ");
                        foreach (var parameter in dbParameters)
                        {
                            sb.Append(parameter.ParameterName + "=" + parameter.Value + " ");
                        }
                    }
                    LogUtil.WriteException(e, sb.Put());
                }
                finally
                {
                    WriteSql(DbCommand);

                    // 必须关闭
                    if (MustCloseConnection)
                    {
                        Close();
                    }
                    else
                    {
                        DbCommand.Parameters.Clear();
                    }
                }

                stopwatch.Stop();
                var statisticsText = $"Elapsed time: {stopwatch.Elapsed.TotalMilliseconds}ms";
                SqlUtil.WriteLog(commandText, commandType.ToString(), dbParameters, statisticsText);
                WriteSql(DbCommand, statisticsText);
                if (stopwatch.Elapsed.TotalMilliseconds >= BaseSystemInfo.SlowQueryMilliseconds)
                {
                    var sb = Pool.StringBuilder.Get();
                    sb.Append(commandText);
                    sb.Append(" ");
                    sb.Append(commandType.ToString());
                    if (dbParameters != null)
                    {
                        sb.Append(" dbParameters: ");
                        foreach (var parameter in dbParameters)
                        {
                            sb.Append(parameter.ParameterName + "=" + parameter.Value + " ");
                        }
                    }
                    else
                    {
                        sb.Append(" ");
                    }
                    sb.Append(statisticsText);
                    LogUtil.WriteLog(sb.Put(), "Slow.DbHelper.ExecuteNonQuery");
                }
            }

            return result;
        }
        #endregion

        #region private void SetBackParamValue(IDbDataParameter[] dbParameters)
        /// <summary>
        /// 设置返回值
        /// </summary>
        /// <param name="dbParameters"></param>
        private void SetBackParamValue(IDbDataParameter[] dbParameters)
        {
            if (dbParameters != null)
            {
                for (var i = 0; i <= dbParameters.Length - 1; i++)
                {
                    if (dbParameters[i].Direction != ParameterDirection.Input)
                    {
                        dbParameters[i].Value = DbCommand.Parameters[i].Value;
                    }
                }
            }
        }
        #endregion

        #region public virtual object ExecuteScalar(string commandText) 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <returns>object</returns>
        public virtual object ExecuteScalar(string commandText)
        {
            return ExecuteScalar(commandText, null, CommandType.Text);
        }
        #endregion

        #region public virtual object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters) 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>Object</returns>
        public virtual object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters)
        {
            return ExecuteScalar(commandText, dbParameters, CommandType.Text);
        }
        #endregion

        #region public virtual object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters, CommandType commandType) 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandType">命令分类</param>
        /// <returns>Object</returns>
        public virtual object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters, CommandType commandType)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            // 自动关闭，以便返还给连接池
            MustCloseConnection = true;
            if (DbConnection == null)
            {
                Open();
            }
            if (DbConnection.State == ConnectionState.Closed)
            {
                DbConnection.Open();
            }
            else if (DbConnection.State == ConnectionState.Broken)
            {
                DbConnection.Close();
                DbConnection.Open();
            }
            object result = null;
            using (DbCommand = DbConnection.CreateCommand())
            {
                try
                {
#if (DEBUG)
                    Trace.WriteLine(DateTime.Now + " :DbConnection Start: " + DbConnection.Database + " ,ThreadId: " + Thread.CurrentThread.ManagedThreadId);
#endif
                    DbCommand.Connection = DbConnection;
                    DbCommand.CommandTimeout = DbConnection.ConnectionTimeout;
                    DbCommand.CommandText = commandText;
                    if (CurrentDbType == CurrentDbType.Oracle)
                    {
                        // 针对Oracle，全局替换换行符，避免报错或不执行
                        // 仅当前系统的换行符
                        DbCommand.CommandText = commandText.Replace(Environment.NewLine, " ");
                        // 各种平台的换行符
                        //_dbCommand.CommandText = commandText.Replace("r\n", " ").Replace('\n', ' ').Replace('\r', ' ');
                    }

                    DbCommand.CommandType = commandType;

                    if (_dbTransaction != null)
                    {
                        DbCommand.Transaction = _dbTransaction;
                        MustCloseConnection = false;
                    }

                    if (dbParameters != null)
                    {
                        DbCommand.Parameters.Clear();
                        for (var i = 0; i < dbParameters.Length; i++)
                        {
                            if (dbParameters[i] != null)
                            {
                                DbCommand.Parameters.Add(((ICloneable)dbParameters[i]).Clone());
                                // dbCommand.Parameters.Add(dbParameters[i]);
                            }
                        }
                    }

                    //写入日志 
                    SqlUtil.WriteLog(commandText, commandType.ToString(), dbParameters);
                    
                    if (DbConnection.State != ConnectionState.Open)
                    {
                        DbConnection.Open();
                    }
                    result = DbCommand.ExecuteScalar();

                    // 这里进行输出参数的处理
                    if (CurrentDbType == CurrentDbType.SqlServer)
                    {
                        SetBackParamValue(dbParameters);
                    }
                }
                catch (Exception e)
                {
                    var sb = Pool.StringBuilder.Get();
                    sb.Append(commandText);
                    sb.Append(" ");
                    sb.Append(commandType.ToString());
                    if (dbParameters != null)
                    {
                        sb.Append(" dbParameters: ");
                        foreach (var parameter in dbParameters)
                        {
                            sb.Append(parameter.ParameterName + "=" + parameter.Value + " ");
                        }
                    }
                    LogUtil.WriteException(e, sb.Put());
                }
                finally
                {
                    WriteSql(DbCommand);

                    // 必须关闭
                    if (MustCloseConnection)
                    {
                        Close();
                    }
                    else
                    {
                        DbCommand.Parameters.Clear();
                    }
                }
                stopwatch.Stop();
                var statisticsText = $"Elapsed time: {stopwatch.Elapsed.TotalMilliseconds}ms";
                SqlUtil.WriteLog(commandText, commandType.ToString(), dbParameters, statisticsText);
                WriteSql(DbCommand, statisticsText);
                if (stopwatch.Elapsed.TotalMilliseconds >= BaseSystemInfo.SlowQueryMilliseconds)
                {
                    var sb = Pool.StringBuilder.Get();
                    sb.Append(commandText);
                    sb.Append(" ");
                    sb.Append(commandType.ToString());
                    if (dbParameters != null)
                    {
                        sb.Append(" dbParameters: ");
                        foreach (var parameter in dbParameters)
                        {
                            sb.Append(parameter.ParameterName + "=" + parameter.Value + " ");
                        }
                    }
                    else
                    {
                        sb.Append(" ");
                    }
                    sb.Append(statisticsText);
                    LogUtil.WriteLog(sb.Put(), "Slow.DbHelper.ExecuteScalar");
                }
            }

            return result;
        }
        #endregion

        #region public virtual DataTable Fill(string commandText) 填充数据表
        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="commandText">查询</param>
        /// <returns>数据表</returns>
        public virtual DataTable Fill(string commandText)
        {
            var dt = new DataTable("DotNet");
            return Fill(dt, commandText, (IDbDataParameter[])null, CommandType.Text);
        }
        #endregion

        #region public virtual DataTable Fill(DataTable dt, string commandText) 填充数据表
        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="dt">目标数据表</param>
        /// <param name="commandText">查询</param>
        /// <returns>数据表</returns>
        public virtual DataTable Fill(DataTable dt, string commandText)
        {
            return Fill(dt, commandText, (IDbDataParameter[])null, CommandType.Text);
        }
        #endregion

        #region public virtual DataTable Fill(string commandText, IDbDataParameter[] dbParameters) 填充数据表
        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>数据表</returns>
        public virtual DataTable Fill(string commandText, IDbDataParameter[] dbParameters)
        {
            var dt = new DataTable("DotNet");
            return Fill(dt, commandText, dbParameters, CommandType.Text);
        }
        #endregion

        #region public virtual DataTable Fill(DataTable dt, string commandText, IDbDataParameter[] dbParameters) 填充数据表
        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="dt">目标数据表</param>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>数据表</returns>
        public virtual DataTable Fill(DataTable dt, string commandText, IDbDataParameter[] dbParameters)
        {
            return Fill(dt, commandText, dbParameters, CommandType.Text);
        }
        #endregion

        #region public virtual DataTable Fill(string commandText, IDbDataParameter[] dbParameters, CommandType commandType) 填充数据表
        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="commandType">命令分类</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>数据表</returns>
        public virtual DataTable Fill(string commandText, IDbDataParameter[] dbParameters, CommandType commandType)
        {
            var dt = new DataTable("DotNet");
            return Fill(dt, commandText, dbParameters, commandType);
        }
        #endregion

        #region public virtual DataTable Fill(DataTable dt, string commandText, IDbDataParameter[] dbParameters, CommandType commandType) 填充数据表
        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="dt">目标数据表</param>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandType">命令分类</param>
        /// <returns>数据表</returns>
        public virtual DataTable Fill(DataTable dt, string commandText, IDbDataParameter[] dbParameters, CommandType commandType)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // 自动关闭，以便返还给连接池
            MustCloseConnection = true;
            if (DbConnection == null)
            {
                Open();
            }
            if (DbConnection.State == ConnectionState.Closed)
            {
                DbConnection.Open();
            }
            else if (DbConnection.State == ConnectionState.Broken)
            {
                DbConnection.Close();
                DbConnection.Open();
            }
            using (DbCommand = DbConnection.CreateCommand())
            {
                try
                {
#if (DEBUG)
                    Trace.WriteLine(DateTime.Now + " :DbConnection Start: " + DbConnection.Database + " ,ThreadId: " + Thread.CurrentThread.ManagedThreadId);
#endif
                    DbCommand.Connection = DbConnection;
                    DbCommand.CommandTimeout = DbConnection.ConnectionTimeout;
                    DbCommand.CommandText = commandText;
                    if (CurrentDbType == CurrentDbType.Oracle)
                    {
                        // 针对Oracle，全局替换换行符，避免报错或不执行
                        // 仅当前系统的换行符
                        DbCommand.CommandText = commandText.Replace(Environment.NewLine, " ");
                        // 各种平台的换行符
                        //_dbCommand.CommandText = commandText.Replace("r\n", " ").Replace('\n', ' ').Replace('\r', ' ');
                    }

                    DbCommand.CommandType = commandType;
                    if (_dbTransaction != null)
                    {
                        DbCommand.Transaction = _dbTransaction;
                        MustCloseConnection = false;
                    }

                    DbDataAdapter = GetInstance().CreateDataAdapter();
                    var dbDataAdapter = DbDataAdapter;
                    if (dbDataAdapter != null)
                    {
                        dbDataAdapter.SelectCommand = DbCommand;
                        DbCommand.Parameters.Clear();
                        if ((dbParameters != null) && (dbParameters.Length > 0))
                        {
                            foreach (var t in dbParameters)
                            {
                                if (t != null)
                                {
                                    DbCommand.Parameters.Add(((ICloneable)t).Clone());
                                }
                            }

                            // dbCommand.Parameters.AddRange(dbParameters);
                        }

                        if (DbConnection.State != ConnectionState.Open)
                        {
                            DbConnection.Open();
                        }

                        dbDataAdapter.Fill(dt);
                        SetBackParamValue(dbParameters);
                        dbDataAdapter.SelectCommand?.Parameters?.Clear();
                    }
                }
                catch (Exception e)
                {
                    //Troy.Cui 2020.05.13
                    dt = null;
                    //记录异常
                    var sb = Pool.StringBuilder.Get();
                    sb.Append(commandText);
                    sb.Append(" ");
                    sb.Append(commandType.ToString());
                    if (dbParameters != null)
                    {
                        sb.Append(" dbParameters: ");
                        foreach (var parameter in dbParameters)
                        {
                            sb.Append(parameter.ParameterName + "=" + parameter.Value + " ");
                        }
                    }
                    LogUtil.WriteException(e, sb.Put());
                }
                finally
                {
                    WriteSql(DbCommand);

                    // 必须关闭
                    if (MustCloseConnection)
                    {
                        Close();
                    }
                    else
                    {
                        DbCommand.Parameters.Clear();
                    }
                    //记录日志，任何时候都写跟踪日志，出错了也要写
                    SqlUtil.WriteLog(commandText, commandType.ToString(), dbParameters);                    
                }
                stopwatch.Stop();
                var statisticsText = $"Elapsed time: {stopwatch.Elapsed.TotalMilliseconds}ms";
                SqlUtil.WriteLog(commandText, commandType.ToString(), dbParameters, statisticsText);
                WriteSql(DbCommand, statisticsText);
                if (stopwatch.Elapsed.TotalMilliseconds >= BaseSystemInfo.SlowQueryMilliseconds)
                {
                    var sb = Pool.StringBuilder.Get();
                    sb.Append(commandText);
                    sb.Append(" ");
                    sb.Append(commandType.ToString());
                    if (dbParameters != null)
                    {
                        sb.Append(" dbParameters: ");
                        foreach (var parameter in dbParameters)
                        {
                            sb.Append(parameter.ParameterName + "=" + parameter.Value + " ");
                        }
                    }
                    else
                    {
                        sb.Append(" ");
                    }
                    sb.Append(statisticsText);
                    LogUtil.WriteLog(sb.Put(), "Slow.DbHelper.Fill");
                }
            }

            return dt;
        }
        #endregion

        #region public virtual DataSet Fill(DataSet dataSet, string commandText, string tableName) 填充数据权限
        /// <summary>
        /// 填充数据权限
        /// </summary>
        /// <param name="dataSet">目标数据权限</param>
        /// <param name="commandText">查询</param>
        /// <param name="tableName">填充表</param>
        /// <returns>数据权限</returns>
        public virtual DataSet Fill(DataSet dataSet, string commandText, string tableName)
        {
            return Fill(dataSet, commandText, tableName, (IDbDataParameter[])null, CommandType.Text);
        }
        #endregion

        #region public virtual DataSet Fill(DataSet dataSet, string commandText, string tableName, IDbDataParameter[] dbParameters) 填充数据权限
        /// <summary>
        /// 填充数据权限
        /// </summary>
        /// <param name="dataSet">数据权限</param>
        /// <param name="commandText">sql查询</param>
        /// <param name="tableName">填充表</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>数据权限</returns>
        public virtual DataSet Fill(DataSet dataSet, string commandText, string tableName, IDbDataParameter[] dbParameters)
        {
            return Fill(dataSet, commandText, tableName, dbParameters, CommandType.Text);
        }
        #endregion

        #region public virtual DataSet Fill(DataSet dataSet, string commandText, string tableName, IDbDataParameter[] dbParameters, CommandType commandType) 填充数据权限
        /// <summary>
        /// 填充数据权限
        /// </summary>
        /// <param name="dataSet">数据权限</param>
        /// <param name="commandType">命令分类</param>
        /// <param name="commandText">sql查询</param>
        /// <param name="tableName">填充表</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>数据权限</returns>
        public virtual DataSet Fill(DataSet dataSet, string commandText, string tableName, IDbDataParameter[] dbParameters, CommandType commandType)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // 自动关闭，以便返还给连接池
            MustCloseConnection = true;
            if (DbConnection == null)
            {
                Open();
            }
            if (DbConnection.State == ConnectionState.Closed)
            {
                DbConnection.Open();
            }
            else if (DbConnection.State == ConnectionState.Broken)
            {
                DbConnection.Close();
                DbConnection.Open();
            }
            using (DbCommand = DbConnection.CreateCommand())
            {
                try
                {
#if (DEBUG)
                    Trace.WriteLine(DateTime.Now + " :DbConnection Start: " + DbConnection.Database + " ,ThreadId: " + Thread.CurrentThread.ManagedThreadId);
#endif
                    //dbCommand.Parameters.Clear();
                    //if ((dbParameters != null) && (dbParameters.Length > 0))
                    //{
                    //    for (int i = 0; i < dbParameters.Length; i++)
                    //    {
                    //        if (dbParameters[i] != null)
                    //        {
                    //            dbDataAdapter.SelectCommand.Parameters.Add(dbParameters[i]);
                    //        }
                    //    }
                    //}
                    DbCommand.Connection = DbConnection;
                    DbCommand.CommandTimeout = DbConnection.ConnectionTimeout;
                    DbCommand.CommandText = commandText;
                    if (CurrentDbType == CurrentDbType.Oracle)
                    {
                        // 针对Oracle，全局替换换行符，避免报错或不执行
                        // 仅当前系统的换行符
                        DbCommand.CommandText = commandText.Replace(Environment.NewLine, " ");
                        // 各种平台的换行符
                        //_dbCommand.CommandText = commandText.Replace("r\n", " ").Replace('\n', ' ').Replace('\r', ' ');
                    }

                    DbCommand.CommandType = commandType;
                    if (_dbTransaction != null)
                    {
                        DbCommand.Transaction = _dbTransaction;
                        MustCloseConnection = false;
                    }
                    DbCommand.Parameters.Clear();
                    if ((dbParameters != null) && (dbParameters.Length > 0))
                    {
                        for (var i = 0; i < dbParameters.Length; i++)
                        {
                            if (dbParameters[i] != null)
                            {
                                DbCommand.Parameters.Add(((ICloneable)dbParameters[i]).Clone());
                            }
                        }
                        // dbCommand.Parameters.AddRange(dbParameters);
                    }

                    //记录日志
                    SqlUtil.WriteLog(commandText, commandType.ToString(), dbParameters);
                    
                    DbDataAdapter = GetInstance().CreateDataAdapter();
                    DbDataAdapter.SelectCommand = DbCommand;
                    if (DbConnection.State != ConnectionState.Open)
                    {
                        DbConnection.Open();
                    }
                    DbDataAdapter.Fill(dataSet, tableName);

                    SetBackParamValue(dbParameters);
                }
                catch (Exception e)
                {
                    //Troy.Cui 2020.05.13
                    dataSet = null;
                    //记录异常
                    var sb = Pool.StringBuilder.Get();
                    sb.Append(commandText);
                    sb.Append(" ");
                    sb.Append(tableName);
                    sb.Append(" ");
                    sb.Append(commandType.ToString());
                    if (dbParameters != null)
                    {
                        sb.Append(" dbParameters: ");
                        foreach (var parameter in dbParameters)
                        {
                            sb.Append(parameter.ParameterName + "=" + parameter.Value + " ");
                        }
                    }

                    LogUtil.WriteException(e, sb.Put());
                }
                finally
                {
                    WriteSql(DbCommand);

                    // 必须关闭
                    if (MustCloseConnection)
                    {
                        Close();
                    }
                    else
                    {
                        DbDataAdapter?.SelectCommand.Parameters.Clear();
                    }
                }

                stopwatch.Stop();
                var statisticsText = $"Elapsed time: {stopwatch.Elapsed.TotalMilliseconds}ms";
                SqlUtil.WriteLog(commandText, commandType.ToString(), dbParameters, statisticsText);
                WriteSql(DbCommand, statisticsText);
                if (stopwatch.Elapsed.TotalMilliseconds >= BaseSystemInfo.SlowQueryMilliseconds)
                {
                    var sb = Pool.StringBuilder.Get();
                    sb.Append(commandText);
                    sb.Append(" ");
                    sb.Append(tableName);
                    sb.Append(" ");
                    sb.Append(commandType.ToString());
                    if (dbParameters != null)
                    {
                        sb.Append(" dbParameters: ");
                        foreach (var parameter in dbParameters)
                        {
                            sb.Append(parameter.ParameterName + "=" + parameter.Value + " ");
                        }
                    }
                    else
                    {
                        sb.Append(" ");
                    }
                    sb.Append(statisticsText);
                    LogUtil.WriteLog(sb.Put(), "Slow.DbHelper.Fill");
                }
            }
            return dataSet;
        }
        /// <summary>
        /// 获取数据库当前时间
        /// </summary>
        /// <returns></returns>
        public abstract string GetDbNow();
        /// <summary>
        /// 获取数据库当前时间
        /// </summary>
        /// <returns></returns>
        public abstract string GetDbDateTime();
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public abstract string GetParameter(string parameter);
        /// <summary>
        /// 组装参数
        /// </summary>
        /// <param name="targetFiled">目标字段</param>
        /// <param name="targetValue">目标值</param>
        /// <returns></returns>
        public abstract IDbDataParameter MakeParameter(string targetFiled, object targetValue);
        /// <summary>
        /// 组装参数
        /// </summary>
        /// <param name="targetFileds"></param>
        /// <param name="targetValues"></param>
        /// <returns></returns>
        public abstract IDbDataParameter[] MakeParameters(string[] targetFileds, object[] targetValues);
        /// <summary>
        /// 组装参数
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public abstract IDbDataParameter[] MakeParameters(Dictionary<string, object> parameters);
        /// <summary>
        /// 组装参数
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public abstract IDbDataParameter[] MakeParameters(List<KeyValuePair<string, object>> parameters);
        /// <summary>
        /// 组装参数
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="parameterValue">参数值</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="parameterSize">参数大小</param>
        /// <param name="parameterDirection">参数方向</param>
        /// <returns></returns>
        public abstract IDbDataParameter MakeParameter(string parameterName, object parameterValue, DbType dbType, int parameterSize, ParameterDirection parameterDirection);
        #endregion

        #region private string GetSql(DbCommand cmd) 获取Sql语句
        /// <summary>
        /// 获取Sql语句
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private string GetSql(DbCommand cmd)
        {
            var max = 4096;
            var sql = string.Empty;
            if (cmd != null)
            {
                try
                {
                    sql = cmd.CommandText;

                    sql = $"[{ConnectionName}] {sql}";

                    var ps = cmd.Parameters;
                    if (ps != null && ps.Count > 0)
                    {
                        var sb = Pool.StringBuilder.Get();
                        sb.Append(sql);
                        sb.Append(" [");
                        for (var i = 0; i < ps.Count; i++)
                        {
                            if (i > 0) sb.Append(", ");
                            var v = ps[i].Value;
                            var sv = "";
                            if (v is Byte[])
                            {
                                var bv = v as Byte[];
                                if (bv.Length > 8)
                                    sv = $"[{bv.Length}]0x{BitConverter.ToString(bv, 0, 8)}...";
                                else
                                    sv = $"[{bv.Length}]0x{BitConverter.ToString(bv)}";
                            }
                            else if (v is String str && str.Length > 64)
                            {
                                //sv = $"[{str.Length}]{str[..64]}...";
                            }
                            else
                            {
                                sv = v is DateTime dt ? dt.ToFullString() : (v + "");
                            }

                            sb.AppendFormat("{0}={1}", ps[i].ParameterName, sv);
                        }
                        sb.Append(']');
                        sql = sb.Put(true);
                    }

                    // 截断超长字符串
                    if (max > 0)
                    {
                        if (sql.Length > max && sql.StartsWithIgnoreCase("Insert"))
                        {
                            //sql = sql[..(max / 2)] + "..." + sql[^(max / 2)..];
                        }
                    }
                    
                }
                catch { return null; }
            }
            return sql;
        }
        #endregion

        #region public static void WriteLog(string commandText, string commandType, IDbDataParameter[] dbParameters = null, string statisticsText = null)

        /// <summary>
        /// 写Sql日志
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="statisticsText">耗时</param>
        private void WriteSql(DbCommand cmd, string statisticsText = null)
        {
            // 系统里应该可以配置是否记录异常现象
            if (!BaseSystemInfo.LogSql)
            {
                return;
            }
            var sb = Pool.StringBuilder.Get();
            sb.Append(GetSql(cmd));
            if (!string.IsNullOrEmpty(statisticsText))
            {
                sb.Append(statisticsText);
            }
            // 将异常信息写入本地文件中
            LogUtil.WriteLog(sb.Put(), "Sql");
        }

        #endregion
    }
}