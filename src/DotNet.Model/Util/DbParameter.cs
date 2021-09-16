//--------------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//--------------------------------------------------------------------

using System;
using System.Data;


namespace DotNet.Model
{
    using Util;
    /// <summary>
    /// DbParameter
    /// </summary>
    public class DbParameter
    {
        /// <summary>
        /// DbParameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public DbParameter(string name, object value)
            : this(name, value, ParameterDirection.Input)
        {
        }
        /// <summary>
        /// DbParameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="parameterDirection"></param>
        public DbParameter(string name, object value, ParameterDirection parameterDirection)
        {
            Name = name;
            Value = value;
            ParameterDirection = parameterDirection;
        }
        /// <summary>
        /// GetIDbDataParameter
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <returns></returns>
        public IDbDataParameter GetIDbDataParameter(IDbHelper dbHelper)
        {
            var result = dbHelper.MakeParameter(Name, Value);
            result.Direction = ParameterDirection;
            return result;
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// ParameterDirection
        /// </summary>
        public ParameterDirection ParameterDirection { get; set; }
    }
}
