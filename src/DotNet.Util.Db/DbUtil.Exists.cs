﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
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
    ///		2012.02.05 版本：1.0	JiRiGaLa 分离程序。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.02.05</date>
    /// </author> 
    /// </summary>
    public partial class DbUtil
    {
        #region 获取记录数
        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表名</param>
        /// <param name="parameters">目标字段,值</param>
        /// <param name="parameter">参数</param>
        /// <returns>行数</returns>
        public static int GetCount(IDbHelper dbHelper, string tableName, List<KeyValuePair<string, object>> parameters, KeyValuePair<string, object> parameter = new KeyValuePair<string, object>())
        {
            var result = 0;
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) FROM " + tableName + " WHERE " + GetWhereString(dbHelper, parameters, BaseUtil.SqlLogicConditional));

            if (!string.IsNullOrEmpty(parameter.Key))
            {
                if (parameter.Value != null)
                {
                    sb.Append(BaseUtil.SqlLogicConditional + "( " + parameter.Key + " <> '" + parameter.Value + "' ) ");
                }
                else
                {
                    sb.Append(BaseUtil.SqlLogicConditional + "( " + parameter.Key + " IS NOT NULL) ");
                }
            }

            object obj = null;
            if (parameters != null)
            {
                obj = dbHelper.ExecuteScalar(sb.Put(), dbHelper.MakeParameters(parameters));
            }
            else
            {
                obj = dbHelper.ExecuteScalar(sb.Put());
            }
            if (obj != null)
            {
                result = Convert.ToInt32(obj.ToString());
            }
            return result;
        }
        #endregion

        #region 获取记录数

        /// <summary>
        /// 获取个数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表名</param>
        /// <param name="condition">条件</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="currentIndex"></param>
        /// <returns>行数</returns>
        public static int GetCount(IDbHelper dbHelper, string tableName, string condition = null, IDbDataParameter[] dbParameters = null, string currentIndex = null)
        {
            var result = 0;
            var sb = Pool.StringBuilder.Get();
            if (currentIndex == null)
            {
                currentIndex = string.Empty;
            }
            sb.Append("SELECT " + currentIndex + " COUNT(*) FROM " + tableName);
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" WHERE " + condition);
            }
            var obj = dbHelper.ExecuteScalar(sb.Put(), dbParameters);
            if (obj != null && obj != DBNull.Value)
            {
                result = Convert.ToInt32(obj.ToString());
            }

            return result;
        }
        #endregion

        #region 表是否存在

        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="dbHelper">dbHelper</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public static bool Exists(IDbHelper dbHelper, string tableName)
        {
            var result = false;
            var sb = Pool.StringBuilder.Get();
            if (dbHelper.CurrentDbType == CurrentDbType.SqlServer)
            {
                sb.Append(string.Format("SELECT COUNT(*) FROM sysobjects WHERE id = object_id(N'[{0}]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1", tableName));
            }
            else if (dbHelper.CurrentDbType == CurrentDbType.Oracle)
            {
                sb.Append(string.Format("SELECT COUNT(*) FROM User_tables WHERE table_name = '{0}'", tableName.ToUpper()));
            }
            else if (dbHelper.CurrentDbType == CurrentDbType.MySql)
            {
                sb.Append(string.Format("SELECT COUNT(*) FROM information_schema.TABLES WHERE table_name = '{0}'", tableName));
            }
            else if (dbHelper.CurrentDbType == CurrentDbType.SqLite)
            {
                sb.Append(string.Format("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name = '{0}'", tableName));
            }
            var obj = dbHelper.ExecuteScalar(sb.Put());
            if (obj != null && obj != DBNull.Value)
            {
                result = Convert.ToInt32(obj.ToString()) > 0;
            }
            return result;
        }
        #endregion

        #region public static bool Exists(IDbHelper dbHelper, string tableName, List<KeyValuePair<string, object>> parameters, KeyValuePair<string, object> parameter = null) 记录是否存在

        /// <summary>
        /// 记录是否存在
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表名</param>
        /// <param name="parameters">参数</param>
        /// <param name="parameter">参数</param>
        /// <returns>存在</returns>
        public static bool Exists(IDbHelper dbHelper, string tableName, List<KeyValuePair<string, object>> parameters, KeyValuePair<string, object> parameter = new KeyValuePair<string, object>())
        {
            return GetCount(dbHelper, tableName, parameters, parameter) > 0;
        }
        #endregion
    }
}