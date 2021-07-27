//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2011, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DotNet.Util
{
	using MySql.Data.MySqlClient;

	/// <summary>
	/// MySqlHelper
	/// 有关数据库连接的方法。
	/// 
	/// 修改记录
	///
	///		2008.09.24 版本：1.0 Troy Cui 创建。
	/// 
	/// 版本：1.0
	/// 
	/// <author>
	///		<name>Troy Cui</name>
	///		<date>2008.09.24</date>
	/// </author> 
	/// </summary>
	public class MySqlHelper : BaseDbHelper, IDbHelper
	{
		public MySqlConnection  Connection   = new MySqlConnection();
		public MySqlCommand     Command      = new MySqlCommand();
		public MySqlDataAdapter DataAdapter  = new MySqlDataAdapter();
		public MySqlTransaction Transaction;

		public override DbProviderFactory GetInstance()
		{
			return MySqlClientFactory.Instance;
		}

		/// <summary>
		/// 当前数据库类型
		/// </summary>
		public override CurrentDbType CurrentDbType => CurrentDbType.MySql;

		#region public MySqlHelper() 构造方法
		/// <summary>
		/// 构造方法
		/// </summary>
		public MySqlHelper()
		{
			FileName = "MySqlHelper.txt";   // sql查询句日志
		}
		#endregion

		#region public MySqlHelper(string connectionString) 构造方法
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="connectionString">数据连接</param>
		public MySqlHelper(string connectionString) : this()
		{
			ConnectionString = connectionString;
		}
		#endregion

		#region public string GetDbNow() 获得数据库日期时间
		/// <summary>
		/// 获得数据库日期时间
		/// </summary>
		/// <returns>日期时间</returns>
		public override string GetDbNow()
		{
			return " now() ";
		}
		#endregion

        #region public string GetDbDateTime() 获得数据库日期时间
        /// <summary>
		/// 获得数据库日期时间
		/// </summary>
		/// <returns>日期时间</returns>
		public override string GetDbDateTime()
		{
			var commandText = " SELECT " + GetDbNow();
			Open();
			var dateTime = ExecuteScalar(commandText, null, CommandType.Text).ToString();
			Close();
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
			return new MySqlParameter(targetFiled, targetValue);
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
				dbParameter = MakeInParam(targetFiled, targetValue);
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
			var dbParameters = new List<IDbDataParameter>();
			if (targetFileds != null && targetValues != null)
			{
				for (var i = 0; i < targetFileds.Length; i++)
				{
					if (targetFileds[i] != null && targetValues[i] != null)
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
		
		#region public IDbDataParameter[] MakeParameters(List<KeyValuePair<string, object>> parameters) 获取参数
		/// <summary>
		/// 获取参数
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

		#region public IDbDataParameter MakeOutParam(string paramName, DbType DbType, int Size) 获取输出参数
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

		#region public IDbDataParameter MakeInParam(string paramName, DbType dbType, int size, object value) 获取输入参数
		/// <summary>
		/// 获取输入参数
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
		/// <param name="parameterName">参数名称</param>
		/// <param name="parameterValue">值</param>
		/// <param name="dbType">数据类型</param>
		/// <param name="parameterSize">大小</param>
		/// <param name="parameterDirection">输出方向</param>
		/// <returns>参数集</returns>
		public override IDbDataParameter MakeParameter(string parameterName, object parameterValue, DbType dbType, Int32 parameterSize, ParameterDirection parameterDirection)
		{
			MySqlParameter parameter;

			if (parameterSize > 0)
			{
                //parameter = new MySqlParameter(parameterName, (MySqlDbType)dbType, parameterSize);
                //2018.03.14 修复Oracle时，统一了一下MySql Troy Cui
			    parameter = new MySqlParameter(parameterName, ConvertToMySqlDbType(dbType), parameterSize);
            }
            else
			{
                //parameter = new MySqlParameter(parameterName, (MySqlDbType)dbType);
                //2018.03.14 修复Oracle时，统一了一下MySql Troy Cui
			    parameter = new MySqlParameter(parameterName, ConvertToMySqlDbType(dbType));
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
			return " ?" + parameter;
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
			var returnValue = string.Empty;
			returnValue = " CONCAT(";
			for (var i = 0; i < values.Length; i++)
			{
				returnValue += values[i] + " ,";
			}
			returnValue = returnValue.Substring(0, returnValue.Length - 2);
			returnValue += ")";
			return returnValue;
		}
        #endregion

        #region  private MySql.Data.MySqlClient.SqlDbType ConvertToMySqlDbType(System.Data.DbType dbType) 类型转换
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="dbType">数据类型</param>
        /// <returns>转换结果</returns>
        private MySqlDbType ConvertToMySqlDbType(System.Data.DbType dbType)
	    {
	        var sqlParameter = new MySqlParameter();
	        sqlParameter.DbType = dbType;
	        return sqlParameter.MySqlDbType;
	    }
	    #endregion
    }
}