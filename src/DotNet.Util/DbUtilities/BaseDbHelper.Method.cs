//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;

namespace DotNet.Util
{
    /// <summary>
    /// BaseDbHelper
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
    ///		<name>Troy Cui</name>
    ///		<date>2013.02.04</date>
    /// </author> 
    /// </summary>
    public abstract partial class BaseDbHelper
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
            if (DbConnection == null)
            {
                Open();
                MustCloseConnection = true;
            }
            else if (DbConnection.State == ConnectionState.Closed)
            {
                Open();
                MustCloseConnection = true;
            }
            _dbCommand = DbConnection.CreateCommand();
            _dbCommand.CommandTimeout = DbConnection.ConnectionTimeout;
            _dbCommand.CommandText = commandText;
            if (CurrentDbType == CurrentDbType.Oracle)
            {
                // 针对Oracle，全局替换换行符，避免报错或不执行
                // 仅当前系统的换行符
                _dbCommand.CommandText = commandText.Replace(Environment.NewLine, " ");
                // 各种平台的换行符
                //_dbCommand.CommandText = commandText.Replace("r\n", " ").Replace('\n', ' ').Replace('\r', ' ');
            }
            _dbCommand.CommandType = commandType;
            if (_dbTransaction != null)
            {
                _dbCommand.Transaction = _dbTransaction;
            }
            if (dbParameters != null)
            {
                _dbCommand.Parameters.Clear();
                foreach (var t in dbParameters)
                {
                    if (t != null)
                    {
                        _dbCommand.Parameters.Add(((ICloneable)t).Clone());
                    }
                }
            }

            // 写入日志
            SqlUtil.WriteLog(commandText, dbParameters);

            // 这里要关闭数据库才可以的
            DbDataReader dbDataReader = null;
            if (!MustCloseConnection)
            {
                dbDataReader = _dbCommand.ExecuteReader();
            }
            else
            {
                dbDataReader = _dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }

            stopwatch.Stop();
            //var statisticsText = $"Elapsed time : {stopwatch.Elapsed.Hours:00}:{stopwatch.Elapsed.Minutes:00}:{stopwatch.Elapsed.Seconds:00}.{stopwatch.Elapsed.Milliseconds:000} ({stopwatch.Elapsed.TotalMilliseconds:0}ms)";
            var statisticsText = $"Elapsed time: {stopwatch.Elapsed.TotalMilliseconds}ms";
            SqlUtil.WriteLog(commandText, dbParameters, statisticsText);
            if (stopwatch.Elapsed.TotalMilliseconds >= BaseSystemInfo.SlowQueryMilliseconds)
            {
                var sb = new StringBuilder();
                sb.Append(commandText);
                sb.Append(commandType.ToString());
                if (dbParameters != null)
                {
                    sb.Append(" dbParameters: ");
                    foreach (var parameter in dbParameters)
                    {
                        sb.Append(parameter.ParameterName + "=" + parameter.Value + " ");
                    }
                }
                sb.Append(statisticsText);
                LogUtil.WriteLog(sb.ToString(), "Slow.DbHelper.ExecuteReader");
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
            // 自动打开
            if (DbConnection == null)
            {
                Open();
                MustCloseConnection = true;
            }
            else if (DbConnection.State == ConnectionState.Closed)
            {
                Open();
                MustCloseConnection = true;
            }

            var result = -1;
            try
            {
                _dbCommand = DbConnection.CreateCommand();
                _dbCommand.CommandTimeout = DbConnection.ConnectionTimeout;
                _dbCommand.CommandText = commandText;
                if (CurrentDbType == CurrentDbType.Oracle)
                {
                    // 针对Oracle，全局替换换行符，避免报错或不执行
                    // 仅当前系统的换行符
                    _dbCommand.CommandText = commandText.Replace(Environment.NewLine, " ");
                    // 各种平台的换行符
                    //_dbCommand.CommandText = commandText.Replace("r\n", " ").Replace('\n', ' ').Replace('\r', ' ');
                }
                _dbCommand.CommandType = commandType;
                if (_dbTransaction != null)
                {
                    _dbCommand.Transaction = _dbTransaction;
                }
                if (dbParameters != null)
                {
                    _dbCommand.Parameters.Clear();
                    for (var i = 0; i < dbParameters.Length; i++)
                    {
                        // if (dbParameters[i] != null)
                        //{
                        _dbCommand.Parameters.Add(((ICloneable)dbParameters[i]).Clone());
                        //}
                    }
                }
                //写入日志 
                SqlUtil.WriteLog(commandText, dbParameters);

                result = _dbCommand.ExecuteNonQuery();

                if (CurrentDbType == CurrentDbType.SqlServer)
                {
                    SetBackParamValue(dbParameters);
                }

            }
            catch (Exception e)
            {
                var sb = new StringBuilder();
                sb.Append(commandText);
                sb.Append(commandType.ToString());
                if (dbParameters != null)
                {
                    sb.Append(" dbParameters: ");
                    foreach (var parameter in dbParameters)
                    {
                        sb.Append(parameter.ParameterName + "=" + parameter.Value + " ");
                    }
                }
                LogUtil.WriteException(e, sb.ToString());
            }
            finally
            {
                //自动关闭
                if (MustCloseConnection)
                {
                    Close();
                }
                else
                {
                    _dbCommand.Parameters.Clear();
                }
            }

            stopwatch.Stop();
            var statisticsText = $"Elapsed time: {stopwatch.Elapsed.TotalMilliseconds}ms";
            SqlUtil.WriteLog(commandText, dbParameters, statisticsText);
            if (stopwatch.Elapsed.TotalMilliseconds >= BaseSystemInfo.SlowQueryMilliseconds)
            {
                var sb = new StringBuilder();
                sb.Append(commandText);
                sb.Append(commandType.ToString());
                if (dbParameters != null)
                {
                    sb.Append(" dbParameters: ");
                    foreach (var parameter in dbParameters)
                    {
                        sb.Append(parameter.ParameterName + "=" + parameter.Value + " ");
                    }
                }
                sb.Append(statisticsText);
                LogUtil.WriteLog(sb.ToString(), "Slow.DbHelper.ExecuteNonQuery");
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
                        dbParameters[i].Value = _dbCommand.Parameters[i].Value;
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
            // 自动打开
            if (DbConnection == null)
            {
                MustCloseConnection = true;
                Open();
            }
            else if (DbConnection.State == ConnectionState.Closed)
            {
                MustCloseConnection = true;
                Open();
            }

            object result = null;
            try
            {
                _dbCommand = DbConnection.CreateCommand();
                _dbCommand.CommandTimeout = DbConnection.ConnectionTimeout;
                _dbCommand.CommandText = commandText;
                if (CurrentDbType == CurrentDbType.Oracle)
                {
                    // 针对Oracle，全局替换换行符，避免报错或不执行
                    // 仅当前系统的换行符
                    _dbCommand.CommandText = commandText.Replace(Environment.NewLine, " ");
                    // 各种平台的换行符
                    //_dbCommand.CommandText = commandText.Replace("r\n", " ").Replace('\n', ' ').Replace('\r', ' ');
                }
                _dbCommand.CommandType = commandType;

                if (_dbTransaction != null)
                {
                    _dbCommand.Transaction = _dbTransaction;
                }

                if (dbParameters != null)
                {
                    _dbCommand.Parameters.Clear();
                    for (var i = 0; i < dbParameters.Length; i++)
                    {
                        if (dbParameters[i] != null)
                        {
                            _dbCommand.Parameters.Add(((ICloneable)dbParameters[i]).Clone());
                            // dbCommand.Parameters.Add(dbParameters[i]);
                        }
                    }
                }
                //写入日志 
                SqlUtil.WriteLog(commandText, dbParameters);
                result = _dbCommand.ExecuteScalar();

                // 这里进行输出参数的处理
                if (CurrentDbType == CurrentDbType.SqlServer)
                {
                    SetBackParamValue(dbParameters);
                }
            }
            catch (Exception e)
            {
                var sb = new StringBuilder();
                sb.Append(commandText);
                sb.Append(commandType.ToString());
                if (dbParameters != null)
                {
                    sb.Append(" dbParameters: ");
                    foreach (var parameter in dbParameters)
                    {
                        sb.Append(parameter.ParameterName + "=" + parameter.Value + " ");
                    }
                }
                LogUtil.WriteException(e, sb.ToString());
            }
            finally
            {
                //自动关闭
                if (MustCloseConnection)
                {
                    Close();
                }
                else
                {
                    _dbCommand.Parameters.Clear();
                }
            }
            stopwatch.Stop();
            var statisticsText = $"Elapsed time: {stopwatch.Elapsed.TotalMilliseconds}ms";
            SqlUtil.WriteLog(commandText, dbParameters, statisticsText);
            if (stopwatch.Elapsed.TotalMilliseconds >= BaseSystemInfo.SlowQueryMilliseconds)
            {
                var sb = new StringBuilder();
                sb.Append(commandText);
                sb.Append(commandType.ToString());
                if (dbParameters != null)
                {
                    sb.Append(" dbParameters: ");
                    foreach (var parameter in dbParameters)
                    {
                        sb.Append(parameter.ParameterName + "=" + parameter.Value + " ");
                    }
                }
                sb.Append(statisticsText);
                LogUtil.WriteLog(sb.ToString(), "Slow.DbHelper.ExecuteScalar");
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

            // 自动打开
            if (DbConnection == null)
            {
                Open();
                MustCloseConnection = true;
            }
            else if (DbConnection.State == ConnectionState.Closed)
            {
                Open();
                MustCloseConnection = true;
            }

            try
            {
                using (_dbCommand = DbConnection.CreateCommand())
                {
                    _dbCommand.CommandTimeout = DbConnection.ConnectionTimeout;
                    _dbCommand.CommandText = commandText;
                    if (CurrentDbType == CurrentDbType.Oracle)
                    {
                        // 针对Oracle，全局替换换行符，避免报错或不执行
                        // 仅当前系统的换行符
                        _dbCommand.CommandText = commandText.Replace(Environment.NewLine, " ");
                        // 各种平台的换行符
                        //_dbCommand.CommandText = commandText.Replace("r\n", " ").Replace('\n', ' ').Replace('\r', ' ');
                    }
                    _dbCommand.CommandType = commandType;
                    if (_dbTransaction != null)
                    {
                        _dbCommand.Transaction = _dbTransaction;
                    }

                    _dbDataAdapter = GetInstance().CreateDataAdapter();
                    var dbDataAdapter = _dbDataAdapter;
                    if (dbDataAdapter != null)
                    {
                        dbDataAdapter.SelectCommand = _dbCommand;
                        if ((dbParameters != null) && (dbParameters.Length > 0))
                        {
                            foreach (var t in dbParameters)
                            {
                                if (t != null)
                                {
                                    _dbCommand.Parameters.Add(((ICloneable)t).Clone());
                                }
                            }

                            // dbCommand.Parameters.AddRange(dbParameters);
                        }

                        dbDataAdapter.Fill(dt);
                        SetBackParamValue(dbParameters);
                        dbDataAdapter.SelectCommand.Parameters.Clear();
                    }
                }
            }
            catch (Exception e)
            {
                //Troy.Cui 2020.05.13
                dt = null;
                //记录异常
                var sb = new StringBuilder();
                sb.Append(commandText);
                sb.Append(commandType.ToString());
                if (dbParameters != null)
                {
                    sb.Append(" dbParameters: ");
                    foreach (var parameter in dbParameters)
                    {
                        sb.Append(parameter.ParameterName + "=" + parameter.Value + " ");
                    }
                }
                LogUtil.WriteException(e, sb.ToString());
            }
            finally
            {
                //自动关闭
                if (MustCloseConnection)
                {
                    Close();
                }
                //记录日志，任何时候都写跟踪日志，出错了也要写
                SqlUtil.WriteLog(commandText, dbParameters);
            }

            stopwatch.Stop();
            var statisticsText = $"Elapsed time: {stopwatch.Elapsed.TotalMilliseconds}ms";
            SqlUtil.WriteLog(commandText, dbParameters, statisticsText);
            if (stopwatch.Elapsed.TotalMilliseconds >= BaseSystemInfo.SlowQueryMilliseconds)
            {
                var sb = new StringBuilder();
                sb.Append(commandText);
                sb.Append(commandType.ToString());
                if (dbParameters != null)
                {
                    sb.Append(" dbParameters: ");
                    foreach (var parameter in dbParameters)
                    {
                        sb.Append(parameter.ParameterName + "=" + parameter.Value + " ");
                    }
                }
                sb.Append(statisticsText);
                LogUtil.WriteLog(sb.ToString(), "Slow.DbHelper.Fill");
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

            // 自动打开
            if (DbConnection == null)
            {
                Open();
                MustCloseConnection = true;
            }
            else if (DbConnection.State == ConnectionState.Closed)
            {
                Open();
                MustCloseConnection = true;
            }

            using (_dbCommand = DbConnection.CreateCommand())
            {
                try
                {
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
                    _dbCommand.CommandTimeout = DbConnection.ConnectionTimeout;
                    _dbCommand.CommandText = commandText;
                    if (CurrentDbType == CurrentDbType.Oracle)
                    {
                        // 针对Oracle，全局替换换行符，避免报错或不执行
                        // 仅当前系统的换行符
                        _dbCommand.CommandText = commandText.Replace(Environment.NewLine, " ");
                        // 各种平台的换行符
                        //_dbCommand.CommandText = commandText.Replace("r\n", " ").Replace('\n', ' ').Replace('\r', ' ');
                    }
                    _dbCommand.CommandType = commandType;
                    if (_dbTransaction != null)
                    {
                        _dbCommand.Transaction = _dbTransaction;
                    }

                    if ((dbParameters != null) && (dbParameters.Length > 0))
                    {
                        for (var i = 0; i < dbParameters.Length; i++)
                        {
                            if (dbParameters[i] != null)
                            {
                                _dbCommand.Parameters.Add(((ICloneable)dbParameters[i]).Clone());
                            }
                        }
                        // dbCommand.Parameters.AddRange(dbParameters);
                    }

                    //记录日志
                    SqlUtil.WriteLog(commandText, dbParameters);

                    _dbDataAdapter = GetInstance().CreateDataAdapter();
                    _dbDataAdapter.SelectCommand = _dbCommand;
                    _dbDataAdapter.Fill(dataSet, tableName);

                    SetBackParamValue(dbParameters);
                }
                catch (Exception e)
                {
                    //Troy.Cui 2020.05.13
                    dataSet = null;
                    //记录异常
                    var sb = new StringBuilder();
                    sb.Append(commandText);
                    sb.Append(tableName);
                    sb.Append(commandType.ToString());
                    if (dbParameters != null)
                    {
                        sb.Append(" dbParameters: ");
                        foreach (var parameter in dbParameters)
                        {
                            sb.Append(parameter.ParameterName + "=" + parameter.Value + " ");
                        }
                    }
                    LogUtil.WriteException(e, sb.ToString());
                }
                finally
                {
                    if (MustCloseConnection)
                    {
                        Close();
                    }
                    else
                    {
                        _dbDataAdapter.SelectCommand.Parameters.Clear();
                    }
                }
            }

            stopwatch.Stop();
            var statisticsText = $"Elapsed time: {stopwatch.Elapsed.TotalMilliseconds}ms";
            SqlUtil.WriteLog(commandText, dbParameters, statisticsText);
            if (stopwatch.Elapsed.TotalMilliseconds >= BaseSystemInfo.SlowQueryMilliseconds)
            {
                var sb = new StringBuilder();
                sb.Append(commandText);
                sb.Append(tableName);
                sb.Append(commandType.ToString());
                if (dbParameters != null)
                {
                    sb.Append(" dbParameters: ");
                    foreach (var parameter in dbParameters)
                    {
                        sb.Append(parameter.ParameterName + "=" + parameter.Value + " ");
                    }
                }
                sb.Append(statisticsText);
                LogUtil.WriteLog(sb.ToString(), "Slow.DbHelper.Fill");
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
    }
}