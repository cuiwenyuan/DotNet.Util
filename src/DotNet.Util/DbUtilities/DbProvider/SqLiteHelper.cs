//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2016 , Hairihan TECH, Ltd.  
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DotNet.Util
{
    using System.Data.SQLite;

    /// <summary>
    /// SQLite 数据库操作
    /// 有关数据库连接的方法。
    /// 
    /// 修改记录
    ///     2013.03.05 版本：1.2 HongMing 更新接口
    ///		2011.07.06 版本：1.1 Pcsky    修改获得参数Sql表达式的BUG，?改成@
    ///		2011.07.06 版本：1.1 Pcsky    修改获得数据库日期时间的表达式，从UTC+0时区改成当前时区:UTC+8。
    ///		2010.10.18 版本：1.0 yanzi    创建。
    /// 
    /// 版本：1.1
    /// 
    /// <author>
    ///		<name>yanzi</name>
    ///		<date>2010.10.18</date>
    /// </author> 
    /// </summary>

    public class SqLiteHelper : BaseDbHelper, IDbHelper
    {
        #region   构造方法

        public SqLiteHelper()
        {
            FileName = "SQLite.txt";   // sql查询句日志
        }

        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <param name="connectionString">数据连接</param>
        public SqLiteHelper(string connectionString)
            : this()
        {
            this.ConnectionString = connectionString;
        }
        #endregion

        #region public override DbProviderFactory GetInstance() 当前数据库实例
        /// <summary>
        /// 当前数据库实例
        /// </summary>
        /// <returns></returns>
        public override DbProviderFactory GetInstance()
        {
            return SQLiteFactory.Instance;
        }
        #endregion

        #region public override CurrentDbType CurrentDbType 当前数据库类型
        /// <summary>
        /// 当前数据库类型
        /// </summary>
        public override CurrentDbType CurrentDbType
        {
            get
            {
                return CurrentDbType.SqLite;
            }
        }
        #endregion

        #region public string GetDbNow() 获得数据库日期时间
        /// <summary>
        /// 获得数据库日期时间
        /// </summary>
        /// <returns>日期时间</returns>
        public override string GetDbNow()
        {
            return "datetime(CURRENT_TIMESTAMP, 'localtime')";
        }
        #endregion

        #region public string GetDBDateTime() 获得数据库日期时间
        /// <summary>
        /// 获得数据库日期时间
        /// </summary>
        /// <returns>日期时间</returns>
        public override string GetDbDateTime()
        {
            string commandText = " SELECT " + this.GetDbNow();
            this.Open();
            string dateTime = this.ExecuteScalar(commandText, null, CommandType.Text).ToString();
            this.Close();
            return dateTime;
        }
        #endregion

        #region public IDbDataParameter MakeInParam(string targetFiled, object targetValue) 获取参数
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="targetFiled">目标字段</param>
        /// <param name="targetValue">值</param>
        /// <returns>参数</returns>
        public IDbDataParameter MakeInParam(string targetFiled, object targetValue)
        {
            return new SQLiteParameter(targetFiled, targetValue);
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
                dbParameter = this.MakeInParam(targetFiled, targetValue);
            }
            return dbParameter;
        }
        #endregion

        #region public IDbDataParameter[] MakeParameters(string[] targetFileds, Object[] targetValues) 获取参数
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="targetFiled">目标字段</param>
        /// <param name="targetValue">值</param>
        /// <returns>参数集</returns>
        public override IDbDataParameter[] MakeParameters(string[] targetFileds, Object[] targetValues)
        {
            // 这里需要用泛型列表，因为有不合法的数组的时候
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            if (targetFileds != null && targetValues != null)
            {
                for (int i = 0; i < targetFileds.Length; i++)
                {
                    if (targetFileds[i] != null && targetValues[i] != null && (!(targetValues[i] is Array)))
                    {
                        dbParameters.Add(this.MakeInParam(targetFileds[i], targetValues[i]));
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
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
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

        #region public IDbDataParameter[] MakeParameters(List<KeyValuePair<string, object>> parameters) 获取参数
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <returns>参数集</returns>
        public override IDbDataParameter[] MakeParameters(List<KeyValuePair<string, object>> parameters)
        {
            // 这里需要用泛型列表，因为有不合法的数组的时候
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
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
        /// <param name="paramName">目标字段</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="size">长度</param>
        /// <returns>参数</returns>
        public IDbDataParameter MakeOutParam(string paramName, DbType dbType, int size)
        {
            return MakeParameter(paramName, null, dbType, size, ParameterDirection.Output);
        }
        #endregion

        #region public IDbDataParameter MakeInParam(string paramName, DbType dbType, int size, object value) 获取输出参数
        /// <summary>
        /// 获取输出参数
        /// </summary>
        /// <param name="paramName">目标字段</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="size">长度</param>
        /// <param name="value">值</param>
        /// <returns>参数</returns>
        public IDbDataParameter MakeInParam(string paramName, DbType dbType, int size, object value)
        {
            return MakeParameter(paramName, value, dbType, size, ParameterDirection.Input);
        }
        #endregion

        #region public IDbDataParameter MakeParameter(string parameterName, object parameterValue, DbType dbType, Int32 parameterSize, ParameterDirection parameterDirection) 获取参数
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="parameterName">目标字段</param>
        /// <param name="parameterValue">值</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="parameterSize">长度</param>
        /// <param name="parameterDirection">参数类型</param>
        /// <returns>参数</returns>
        public override IDbDataParameter MakeParameter(string parameterName, object parameterValue, DbType dbType, Int32 parameterSize, ParameterDirection parameterDirection)
        {
            SQLiteParameter parameter;

            if (parameterSize > 0)
            {
                parameter = new SQLiteParameter(dbType, parameterSize, parameterName);
            }
            else
            {
                parameter = new SQLiteParameter(parameterName, (DbType)dbType);
            }

            parameter.Direction = parameterDirection;
            if (!(parameterDirection == ParameterDirection.Output && parameterValue == null))
            {
                parameter.Value = parameterValue;
            }

            return parameter;
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
            return " @" + parameter;
        }
        #endregion

        #region string PlusSign(params string[] values) 获得Sql字符串相加符号
        /// <summary>
        ///  获得Sql字符串相加符号
        /// </summary>
        /// <param name="values">参数值</param>
        /// <returns>字符加</returns>
        public override string PlusSign(params string[] values)
        {
            string returnValue = string.Empty;
            returnValue = " CONCAT(";
            for (int i = 0; i < values.Length; i++)
            {
                returnValue += values[i] + " ,";
            }
            returnValue = returnValue.Substring(0, returnValue.Length - 2);
            returnValue += ")";
            return returnValue;
        }
        #endregion
    }
}