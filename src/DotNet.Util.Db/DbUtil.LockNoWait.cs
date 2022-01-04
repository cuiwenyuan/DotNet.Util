//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;

namespace DotNet.Util
{
    /// <summary>
    ///	DbUtil
    /// 通用基类
    /// 
    /// 修改记录
    /// 
    ///		2012.03.21 版本：2.0	JiRiGaLa 优化代码。
    ///		2012.02.05 版本：1.0	JiRiGaLa 分离程序。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.03.21</date>
    /// </author> 
    /// </summary>
    public partial class DbUtil
    {
        //
        // 锁定表记录
        //
        /// <summary>
        /// 锁定表
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="tableName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int LockNoWait(IDbHelper dbHelper, string tableName, params KeyValuePair<string, object>[] parameters)
        {
            var parametersList = new List<KeyValuePair<string, object>>();
            for (var i = 0; i < parameters.Length; i++)
            {
                parametersList.Add(parameters[i]);
            }
            return LockNoWait(dbHelper, tableName, parametersList);
        }


        #region public static int LockNoWait(IDbHelper dbHelper, string tableName, List<KeyValuePair<string, object>> parameters) 记录是否存在
        /// <summary>
        /// 锁定表记录
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表名</param>
        /// <param name="parameters">参数</param>
        /// <returns>锁定的行数</returns>
        public static int LockNoWait(IDbHelper dbHelper, string tableName, List<KeyValuePair<string, object>> parameters)
        {
            var result = 0;
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT " + BaseUtil.FieldId + " FROM " + tableName + " WHERE " + GetWhereString(dbHelper, parameters, BaseUtil.SqlLogicConditional));
            sb.Append(" FOR UPDATE NOWAIT ");
            try
            {
                var dt = new DataTable("ForUpdateNoWait");
                dbHelper.Fill(dt, sb.Put(), dbHelper.MakeParameters(parameters));
                result = dt.Rows.Count;
            }
            catch
            {
                result = -1;
            }
            return result;
        }
        #endregion
    }
}