//--------------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//--------------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace DotNet.Model
{
    using Util;
    /// <summary>
    /// SqlExecute
    /// </summary>
    public class SqlExecute
    {
        /// <summary>
        /// SqlExecute
        /// </summary>
        public SqlExecute()
            : this("", null, CommandType.Text)
        {
        }
        /// <summary>
        /// SqlExecute
        /// </summary>
        /// <param name="commandText"></param>
        public SqlExecute(string commandText)
            : this(commandText, null, CommandType.Text)
        {
        }
        /// <summary>
        /// SqlExecute
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        public SqlExecute(string commandText, CommandType commandType)
            : this(commandText, null, commandType)
        {
        }
        /// <summary>
        /// SqlExecute
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters">参数</param>
        /// <param name="commandType"></param>
        public SqlExecute(string commandText, object parameters, CommandType commandType)
        {
            CommandText = commandText;
            CommandType = commandType;
            if (parameters != null)
            {
                if (parameters is DbParameter[])
                {
                    _parameters = (parameters as DbParameter[]).ToList();
                }
                else
                {
                    foreach (var item in parameters.GetType().GetProperties())
                    {
                        var value = item.GetValue(parameters, null);
                        _parameters.Add(new DbParameter(item.Name, value));
                    }
                }
            }
        }
        /// <summary>
        /// AddParameter
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value"></param>
        /// <param name="parameterDirection"></param>
        /// <returns></returns>
        public SqlExecute AddParameter(string name, object value, ParameterDirection parameterDirection)
        {
            _parameters.Add(new DbParameter(name, value, parameterDirection));
            return this;
        }
        /// <summary>
        /// SetValueAt
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetValueAt(int index, object value)
        {
            _parameters[index].Value = value;
        }
        /// <summary>
        /// GetValueAt
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object GetValueAt(int index)
        {
            return _parameters[index].Value;
        }


        private List<DbParameter> _parameters = new List<DbParameter>();

        /// <summary>
        /// CommandText
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// CommandType
        /// </summary>
        public CommandType CommandType { get; set; }
        /// <summary>
        /// GetParameters
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <returns></returns>
        public IDbDataParameter[] GetParameters(IDbHelper dbHelper)
        {
            if (_parameters.Count == 0) return null;
            var result = new IDbDataParameter[_parameters.Count];
            for (var i = 0; i <= _parameters.Count - 1; i++)
            {
                result[i] = _parameters[i].GetIDbDataParameter(dbHelper);
            }
            return result;
        }
    }
}
