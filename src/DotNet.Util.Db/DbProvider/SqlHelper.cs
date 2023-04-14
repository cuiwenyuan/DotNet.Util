//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;

namespace DotNet.Util
{
    /// <summary>
    /// SqlHelper
    /// 获取参数总汇
    /// 
    /// 修改记录
    /// 
    ///     2012.03.17 版本：1.3 zhangyi  精细化注释
    ///		2008.08.26 版本：1.2 JiRiGaLa 修改Open时的错误反馈。
    ///		2008.06.01 版本：1.1 JiRiGaLa 数据库连接获得方式进行改进，构造函数获得调通。
    ///		2008.05.07 版本：1.0 JiRiGaLa 创建。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2008.08.26</date>
    /// </author> 
    /// </summary>
    public class SqlHelper : DbHelper, IDbHelper
    {
        /// <summary>
        /// GetInstance
        /// </summary>
        /// <returns></returns>
        public override DbProviderFactory GetInstance()
        {
            return SqlClientFactory.Instance;
        }

        #region public override CurrentDbType CurrentDbType 获得当前数据库类型
        /// <summary>
        /// 获得当前数据库类型
        /// </summary>
        public override CurrentDbType CurrentDbType => CurrentDbType.SqlServer;

        #endregion

        #region public SqlHelper() 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public SqlHelper()
        {
            FileName = "SqlHelper.txt"; // sql查询句日志
        }
        #endregion

        #region public SqlHelper(string connectionString) 构造函数,设置数据库连接
        /// <summary>
        /// 构造函数,设置数据库连接
        /// </summary>
        /// <param name="connectionString">数据连接</param>
        public SqlHelper(string connectionString) : this()
        {
            ConnectionString = connectionString;
        }
        #endregion

        #region public string GetDbNow() 获得数据库日期时间 字符串
        /// <summary>
        /// 获得数据库当前日期时间 字符串
        /// </summary>
        /// <returns>日期时间</returns>
        public override string GetDbNow()
        {
            return " GETDATE() ";
        }
        #endregion

        #region public string GetDbDateTime() 获得数据库日期时间 执行SQL后的结果
        /// <summary>
        /// 获得数据库日期时间 执行SQL后的结果
        /// </summary>
        /// <returns>日期时间</returns>
        public override string GetDbDateTime()
        {
            var commandText = "SELECT " + GetDbNow();
            Open();
            var dateTime = ExecuteScalar(commandText, null, CommandType.Text).ToString();
            Close();
            return dateTime;
        }
        #endregion

        #region string PlusSign(params string[] values) 获得Sql字符串相加符号
        /// <summary>
        ///  获得Sql字符串相加符号
        /// </summary>
        /// <param name="values">参数值</param>
        /// <returns>字符加</returns>
        public new string PlusSign(params string[] values)
        {
            var result = string.Empty;
            for (var i = 0; i < values.Length; i++)
            {
                result += values[i] + " + ";
            }
            if (!string.IsNullOrEmpty(result))
            {
                result = result.Substring(0, result.Length - 3);
            }
            else
            {
                result = " + ";
            }
            return result;
        }
        #endregion

        #region public string GetParameter(string parameter) 获得参数Sql表达式
        /// <summary>
        /// 获得参数Sql表达式
        /// </summary>
        /// <param name="parameter">参数名称</param>
        /// <returns>字符串</returns>
        public override string GetParameter(string parameter)
        {
            return "@" + parameter;
        }
        #endregion

        #region public IDbDataParameter MakeParameter(string targetFiled, object targetValue) 获取参数
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="targetFiled">目标字段</param>
        /// <param name="targetValue">值</param>
        /// <returns>参数集</returns>
        public override IDbDataParameter MakeParameter(string targetFiled, object targetValue)
        {
            IDbDataParameter dbParameter = null;
            if (targetFiled != null)
            {
                dbParameter = MakeInParam(targetFiled, targetValue ?? DBNull.Value);
            }
            return dbParameter;
        }
        #endregion

        #region public IDbDataParameter MakeInParam(string targetFiled, object targetValue) 获取参数实际方法
        /// <summary>
        /// 获取参数实际方法
        /// </summary>
        /// <param name="targetFiled">目标字段</param>
        /// <param name="targetValue">值</param>
        /// <returns>参数</returns>
        public IDbDataParameter MakeInParam(string targetFiled, object targetValue)
        {
            return new SqlParameter("@" + targetFiled, targetValue);
        }
        #endregion

        #region public IDbDataParameter[] MakeParameters(string[] targetFileds, Object[] targetValues) 获取参数数组集合
        /// <summary>
        /// 获取参数数组集合
        /// </summary>
        /// <param name="targetFileds">目标字段</param>
        /// <param name="targetValues">值</param>
        /// <returns>参数集</returns>
        public override IDbDataParameter[] MakeParameters(string[] targetFileds, Object[] targetValues)
        {
            // 这里需要用泛型列表，因为有不合法的数组的时候
            var dbParameters = new List<IDbDataParameter>();
            if (targetFileds != null && targetValues != null)
            {
                for (var i = 0; i < targetFileds.Length; i++)
                {
                    if (targetFileds[i] != null && targetValues[i] != null && (!(targetValues[i] is Array)))
                    {
                        dbParameters.Add(MakeInParam(targetFileds[i], targetValues[i]));
                    }
                }
            }
            return dbParameters.ToArray();
        }
        #endregion

        #region public IDbDataParameter[] MakeParameters(Dictionary<string, object> parameters) 获取参数
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <returns>参数集</returns>
        public override IDbDataParameter[] MakeParameters(Dictionary<string, object> parameters)
        {
            // 这里需要用泛型列表，因为有不合法的数组的时候
            var dbParameters = new List<IDbDataParameter>();
            if (parameters != null && parameters.Count > 0)
            {
                foreach (var parameter in parameters)
                {
                    if (parameter.Key != null && parameter.Value != null && (!(parameter.Value is Array)))
                    {
                        dbParameters.Add(MakeParameter(parameter.Key, parameter.Value));
                    }
                }
            }
            return dbParameters.ToArray();
        }
        #endregion

        #region public IDbDataParameter[] MakeParameters(List<KeyValuePair<string, object>> parameters) 获取参数泛型列表
        /// <summary>
        /// 获取参数泛型列表
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <returns>参数集</returns>
        public override IDbDataParameter[] MakeParameters(List<KeyValuePair<string, object>> parameters)
        {
            // 这里需要用泛型列表，因为有不合法的数组的时候
            var dbParameters = new List<IDbDataParameter>();
            if (parameters != null && parameters.Count > 0)
            {
                foreach (var parameter in parameters)
                {
                    if (parameter.Key != null && parameter.Value != null && (!(parameter.Value is Array)))
                    {
                        dbParameters.Add(MakeParameter(parameter.Key, parameter.Value));
                    }
                }
            }
            return dbParameters.ToArray();
        }
        #endregion

        #region public IDbDataParameter MakeOutParam(string paramName, DbType dbType, int size) 获取输出参数
        /// <summary>
        /// 获取输出参数
        /// </summary>
        /// <param name="paramName">参数</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="size">长度</param>
        /// <returns></returns>
        public IDbDataParameter MakeOutParam(string paramName, DbType dbType, int size)
        {
            return MakeParameter(paramName, null, dbType, size, ParameterDirection.Output);
        }
        #endregion

        #region public IDbDataParameter MakeInParam(string paramName, DbType dbType, int Size, object value) 获取输入参数
        /// <summary>
        /// 获取输入参数
        /// </summary>
        /// <param name="paramName">参数</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="size">长度</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public IDbDataParameter MakeInParam(string paramName, DbType dbType, int size, object value)
        {
            return MakeParameter(paramName, value, dbType, size, ParameterDirection.Input);
        }
        #endregion

        #region public IDbDataParameter MakeParameter(string parameterName, object parameterValue, DbType dbType, Int32 parameterSize, ParameterDirection parameterDirection) 获取参数，包含详细参数设置
        /// <summary>
        /// 获取参数，包含详细参数设置
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="parameterValue">值</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="parameterSize">长度</param>
        /// <param name="parameterDirection">参数类型</param>
        /// <returns>参数</returns>
        public override IDbDataParameter MakeParameter(string parameterName, object parameterValue, DbType dbType, Int32 parameterSize, ParameterDirection parameterDirection)
        {
            SqlParameter parameter;

            if (parameterSize > 0)
            {
                parameter = new SqlParameter(parameterName, ConvertToSqlDbType(dbType), parameterSize);
            }
            else
            {
                parameter = new SqlParameter(parameterName, ConvertToSqlDbType(dbType));
            }

            parameter.Direction = parameterDirection;
            if (!(parameterDirection == ParameterDirection.Output && parameterValue == null))
            {
                parameter.Value = parameterValue;
            }

            return parameter;
        }
        #endregion

        #region  private System.Data.SqlDbType ConvertToSqlDbType(System.Data.DbType dbType) 类型转换
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="dbType">数据类型</param>
        /// <returns>转换结果</returns>
        private SqlDbType ConvertToSqlDbType(DbType dbType)
        {
            var sqlParameter = new SqlParameter
            {
                DbType = dbType
            };
            return sqlParameter.SqlDbType;
        }
        #endregion

        #region public void SqlBulkCopyData(DataTable dt) 利用Net SqlBulkCopy 批量导入数据库,速度超快
        /// <summary>
        /// 利用Net SqlBulkCopy 批量导入数据库,速度超快
        /// </summary>
        /// <param name="dt">源内存数据表（先通过SELECT TOP 0获取空白DataTable）</param>
        /// <param name="destinationTableName">目标表名称</param>
        /// <param name="bulkCopyTimeout">超时限制（毫秒）</param>
        /// <param name="batchSize">批大小（默认0，即一次性导入）</param>
        public override bool SqlBulkCopyData(DataTable dt, string destinationTableName, int bulkCopyTimeout = 1000, int batchSize = 0)
        {
            var result = false;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            if (string.IsNullOrEmpty(destinationTableName))
            {
                destinationTableName = dt.TableName;
            }
            // SQL 数据连接
            SqlConnection sqlConnection = null;
            // 打开数据库
            Open();
            // 获取连接
            sqlConnection = (SqlConnection)GetDbConnection();
            using (var tran = sqlConnection.BeginTransaction())
            {
                // 批量保存数据，只能用于Sql
                var sqlBulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, tran);
                sqlBulkCopy.BatchSize = batchSize;
                // 设置目标表名称
                sqlBulkCopy.DestinationTableName = destinationTableName;
                // 设置超时限制
                sqlBulkCopy.BulkCopyTimeout = bulkCopyTimeout;

                foreach (DataColumn dtColumn in dt.Columns)
                {
                    sqlBulkCopy.ColumnMappings.Add(dtColumn.ColumnName, dtColumn.ColumnName);
                }
                try
                {
                    // 写入
                    sqlBulkCopy.WriteToServer(dt);
                    // 提交事务
                    tran.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    sqlBulkCopy.Close();
                    LogUtil.WriteException(ex, "SqlBulkCopyData");
                }
                finally
                {
                    sqlBulkCopy.Close();
                    Close();
                }
            }
            stopwatch.Stop();
            var statisticsText = $"Elapsed time: {stopwatch.Elapsed.TotalMilliseconds}ms";
            //SqlUtil.WriteLog(destinationTableName, "SqlBulkCopy", null, statisticsText);
            WriteSqlLog(destinationTableName, "SqlBulkCopy", null, statisticsText);
            if (stopwatch.Elapsed.TotalMilliseconds >= BaseSystemInfo.SlowQueryMilliseconds)
            {
                WriteSqlLog(destinationTableName, "SqlBulkCopy", null, statisticsText, "Slow.DbHelper.SqlBulkCopy");
            }

            return result;
        }
        #endregion
    }
}