//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2011 , Hairihan TECH, Ltd. 
//-----------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace DotNet.Util
{
	using Sybase.Data.AseClient;

	/// <summary>
	/// AseHelper
	/// 有关数据库连接的方法。
	/// 
	/// 修改纪录
	///
	///		2013.01.15 版本：1.0 kings 创建。
	/// 
	/// 版本：1.0
	/// 
	/// <author>
    ///		<name>kings</name>
    ///		<date>2013.01.15</date>
	/// </author> 
	/// </summary>
	public class AseHelper : DbHelper, IDbHelper
	{
		public AseConnection  connection   = new AseConnection();
		public AseCommand     command      = new AseCommand();
		public AseDataAdapter dataAdapter  = new AseDataAdapter();
		public AseTransaction transaction;

		public override DbProviderFactory GetInstance()
		{
            return DbProviderFactories.GetFactory("Sybase.Data.AseClient");
		}

		/// <summary>
		/// 当前数据库类型
		/// </summary>
		public override CurrentDbType CurrentDbType
		{
			get
			{
				return CurrentDbType.Ase;
			}
		}

		#region public AseHelper() 构造方法
		/// <summary>
		/// 构造方法
		/// </summary>
		public AseHelper()
		{
			FileName = "AseHelper.txt";   // sql查询句日志
		}
		#endregion

		#region public AseHelper(string connectionString) 构造方法
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="connectionString">数据连接</param>
		public AseHelper(string connectionString) : this()
		{
			this.ConnectionString = connectionString;
		}
		#endregion

		#region public string GetDbNow() 获得数据库日期时间
		/// <summary>
		/// 获得数据库日期时间
		/// </summary>
		/// <returns>日期时间</returns>
		public string GetDbNow()
		{
            return "getdate()";
		}
		#endregion

        #region public string GetDbDateTime() 获得数据库日期时间
        /// <summary>
		/// 获得数据库日期时间
		/// </summary>
		/// <returns>日期时间</returns>
        public string GetDbDateTime()
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
			return new AseParameter(targetFiled, targetValue);
		}
		#endregion

		#region public IDbDataParameter MakeParameter(string targetFiled, object targetValue) 获取参数
		/// <summary>
		/// 获取参数
		/// </summary>
		/// <param name="targetFiled">目标字段</param>
		/// <param name="targetValue">值</param>
		/// <returns>参数集</returns>
		public IDbDataParameter MakeParameter(string targetFiled, object targetValue)
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
		public IDbDataParameter[] MakeParameters(string[] targetFileds, Object[] targetValues)
		{
			List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
			if (targetFileds != null && targetValues != null)
			{
				for (int i = 0; i < targetFileds.Length; i++)
				{
					if (targetFileds[i] != null && targetValues[i] != null)
					{
						dbParameters.Add(this.MakeInParam(targetFileds[i], targetValues[i]));
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
		public IDbDataParameter[] MakeParameters(List<KeyValuePair<string, object>> parameters)
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

		#region public IDbDataParameter MakeOutParam(string paramName, DbType DbType, int Size) 获取输出参数
		/// <summary>
		/// 获取输出参数
		/// </summary>
		/// <param name="paramName">目标字段</param>
		/// <param name="DbType">数据类型</param>
		/// <param name="Size">长度</param>
		/// <returns>参数</returns>
		public IDbDataParameter MakeOutParam(string paramName, DbType DbType, int Size)
		{
			return MakeParameter(paramName, null, DbType, Size, ParameterDirection.Output);
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
		public IDbDataParameter MakeParameter(string parameterName, object parameterValue, DbType dbType, Int32 parameterSize, ParameterDirection parameterDirection)
		{
			AseParameter parameter;

			if (parameterSize > 0)
			{
				parameter = new AseParameter(parameterName, (AseDbType)dbType, parameterSize);
			}
			else
			{
				parameter = new AseParameter(parameterName, (AseDbType)dbType);
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
		public string GetParameter(string parameter)
		{
            return " :" + parameter + " ";
		}
		#endregion

        ////#region string PlusSign(params string[] values) 获得Sql字符串相加符号
        ///// <summary>
        /////  获得Sql字符串相加符号
        ///// </summary>
        ///// <param name="values">参数值</param>
        ///// <returns>字符加</returns>
        //public string PlusSign(params string[] values)
        //{
        //    string returnValue = string.Empty;
        //    returnValue = " CONCAT(";
        //    for (int i = 0; i < values.Length; i++)
        //    {
        //        returnValue += values[i] + " ,";
        //    }
        //    returnValue = returnValue.Substring(0, returnValue.Length - 2);
        //    returnValue += ")";
        //    return returnValue;
        //}
        //#endregion
	}
}