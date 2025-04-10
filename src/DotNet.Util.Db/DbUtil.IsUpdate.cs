﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;

namespace DotNet.Util
{
    /// <summary>
    ///	DbUtil
    /// 通用基类
    /// 
    /// 修改记录
    ///
    ///     2021.05.21 版本：4.0 Troy.Cui 增加Version参数，兼容旧版本V4
    ///		2012.02.05 版本：1.0	JiRiGaLa 分离程序。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.02.05</date>
    /// </author> 
    /// </summary>
    public partial class DbUtil
    {
        #region public static bool IsUpdate(this IDbHelper dbHelper, string tableName, Object id, string oldUpdateUserId, DateTime? oldUpdateTime) 数据是否已经被别人修改了

        /// <summary>
        /// 数据是否已经被别人修改了
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="id">主键</param>
        /// <param name="oldUpdateUserId">最后修改者</param>
        /// <param name="oldUpdateTime">修改时间</param>
        /// <returns>已被修改</returns>
        public static bool IsUpdate(this IDbHelper dbHelper, string tableName, Object id, string oldUpdateUserId, DateTime? oldUpdateTime)
        {
            return IsUpdate(dbHelper, tableName, BaseUtil.FieldId, id, oldUpdateUserId, oldUpdateTime);
        }
        #endregion

        #region public static bool IsUpdate(this IDbHelper dbHelper, string tableName, string fieldName, Object fieldValue, string oldUpdateUserId, DateTime? oldUpdateTime) 数据是否已经被别人修改了

        /// <summary>
        /// 数据是否已经被别人修改了
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">字段</param>
        /// <param name="fieldValue">值</param>
        /// <param name="oldUpdateUserId">最后修改者</param>
        /// <param name="oldUpdateTime">修改时间</param>
        /// <returns>已被修改</returns>
        public static bool IsUpdate(this IDbHelper dbHelper, string tableName, string fieldName, Object fieldValue, string oldUpdateUserId, DateTime? oldUpdateTime)
        {
            var result = false;
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT " + BaseUtil.FieldId + "," + BaseUtil.FieldCreateUserId + "," + BaseUtil.FieldCreateTime
                         + "," + BaseUtil.FieldUpdateUserId
                         + "," + BaseUtil.FieldUpdateTime
                         + " FROM " + tableName
                         + " WHERE " + fieldName + " = " + dbHelper.GetParameter(fieldName));
            var dt = dbHelper.Fill(sb.Return(), new IDbDataParameter[] { dbHelper.MakeParameter(fieldName, fieldValue) });
            result = IsUpdate(dt, oldUpdateUserId, oldUpdateTime);
            return result;
        }
        #endregion

        #region private static bool IsUpdate(DataTable result, string oldUpdateUserId, DateTime? oldUpdateTime) 数据是否已经被别人修改了

        /// <summary>
        /// 数据是否已经被别人修改了
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="oldUpdateUserId">最后修改者</param>
        /// <param name="oldUpdateTime">修改时间</param>
        /// <returns>已被修改</returns>
        private static bool IsUpdate(DataTable dt, string oldUpdateUserId, DateTime? oldUpdateTime)
        {
            var result = false;
            foreach (DataRow dr in dt.Rows)
            {
                result = IsUpdate(dr, oldUpdateUserId, oldUpdateTime);
            }
            return result;
        }
        #endregion

        #region public static bool IsUpdate(DataRow dr, string oldUpdateUserId, DateTime? oldUpdateTime) 数据是否已经被别人修改了

        /// <summary>
        /// 数据是否已经被别人修改了
        /// </summary>
        /// <param name="dr">数据行</param>
        /// <param name="oldUpdateUserId">最后修改者</param>
        /// <param name="oldUpdateTime">修改时间</param>
        /// <returns>已被修改</returns>
        public static bool IsUpdate(DataRow dr, string oldUpdateUserId, DateTime? oldUpdateTime)
        {
            var result = false;
            if ((dr[BaseUtil.FieldUpdateUserId] != DBNull.Value) &&
                ((dr[BaseUtil.FieldUpdateTime] != DBNull.Value)))
            {
                var newUpdateTime = dr[BaseUtil.FieldUpdateTime].ToDateTime();
                var newUpdateUserId = dr[BaseUtil.FieldUpdateUserId].ToString();
                var diff = DateTime.Compare(newUpdateTime, oldUpdateTime.ToDateTime());
                if (!newUpdateUserId.Equals(oldUpdateUserId))
                {
                    result = true;
                }
            }

            return result;
        }
        #endregion
    }
}