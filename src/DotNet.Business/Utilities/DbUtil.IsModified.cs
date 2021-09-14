﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;

namespace DotNet.Business
{
    using Util;

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
    ///		<name>JiRiGaLa</name>
    ///		<date>2012.02.05</date>
    /// </author> 
    /// </summary>
    public partial class DbUtil
    {
        #region public static bool IsModifed(IDbHelper dbHelper, string tableName, Object id, string oldUpdateUserId, DateTime? oldUpdateTime) 数据是否已经被别人修改了

        /// <summary>
        /// 数据是否已经被别人修改了
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="id">主键</param>
        /// <param name="oldUpdateUserId">最后修改者</param>
        /// <param name="oldUpdateTime">最后修改时间</param>
        /// <param name="tableVersion">表版本</param>
        /// <returns>已被修改</returns>
        public static bool IsModified(IDbHelper dbHelper, string tableName, Object id, string oldUpdateUserId, DateTime? oldUpdateTime, int tableVersion = 4)
        {
            return IsModified(dbHelper, tableName, BaseUtil.FieldId, id, oldUpdateUserId, oldUpdateTime, tableVersion);
        }
        #endregion

        #region public static bool IsModifed(IDbHelper dbHelper, string tableName, string fieldName, Object fieldValue, string oldUpdateUserId, DateTime? oldUpdateTime) 数据是否已经被别人修改了

        /// <summary>
        /// 数据是否已经被别人修改了
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">字段</param>
        /// <param name="fieldValue">值</param>
        /// <param name="oldUpdateUserId">最后修改者</param>
        /// <param name="oldUpdateTime">最后修改时间</param>
        /// <param name="tableVersion">表版本</param>
        /// <returns>已被修改</returns>
        public static bool IsModified(IDbHelper dbHelper, string tableName, string fieldName, Object fieldValue, string oldUpdateUserId, DateTime? oldUpdateTime, int tableVersion = 4)
        {
            var result = false;
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT " + BaseUtil.FieldId + "," + BaseUtil.FieldCreateUserId + "," + (tableVersion == 4 ? BaseUtil.FieldCreateOn : BaseUtil.FieldCreateTime)
                         + "," + (tableVersion == 4 ? BaseUtil.FieldModifiedUserId : BaseUtil.FieldUpdateUserId)
                         + "," + (tableVersion == 4 ? BaseUtil.FieldModifiedOn : BaseUtil.FieldUpdateTime)
                         + " FROM " + tableName
                         + " WHERE " + fieldName + " = " + dbHelper.GetParameter(fieldName));
            var dt = dbHelper.Fill(sb.Put(), new IDbDataParameter[] { dbHelper.MakeParameter(fieldName, fieldValue) });
            result = IsModified(dt, oldUpdateUserId, oldUpdateTime, tableVersion);
            return result;
        }
        #endregion

        #region private static bool IsModifed(DataTable result, string oldUpdateUserId, DateTime? oldUpdateTime) 数据是否已经被别人修改了

        /// <summary>
        /// 数据是否已经被别人修改了
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="oldUpdateUserId">最后修改者</param>
        /// <param name="oldUpdateTime">最后修改时间</param>
        /// <param name="tableVersion">表版本</param>
        /// <returns>已被修改</returns>
        private static bool IsModified(DataTable dt, string oldUpdateUserId, DateTime? oldUpdateTime, int tableVersion = 4)
        {
            var result = false;
            foreach (DataRow dr in dt.Rows)
            {
                result = IsModified(dr, oldUpdateUserId, oldUpdateTime, tableVersion);
            }
            return result;
        }
        #endregion

        #region public static bool IsModifed(DataRow dr, string oldUpdateUserId, DateTime? oldUpdateTime) 数据是否已经被别人修改了

        /// <summary>
        /// 数据是否已经被别人修改了
        /// </summary>
        /// <param name="dr">数据行</param>
        /// <param name="oldUpdateUserId">最后修改者</param>
        /// <param name="oldUpdateTime">最后修改时间</param>
        /// <param name="tableVersion">表版本</param>
        /// <returns>已被修改</returns>
        public static bool IsModified(DataRow dr, string oldUpdateUserId, DateTime? oldUpdateTime, int tableVersion = 4)
        {
            var result = false;
            if ((dr[tableVersion == 4 ? BaseUtil.FieldModifiedUserId : BaseUtil.FieldUpdateUserId] != DBNull.Value) &&
                ((dr[tableVersion == 4 ? BaseUtil.FieldModifiedOn : BaseUtil.FieldUpdateTime] != DBNull.Value)))
            {

                var newModifiedOn =
                    DateTime.Parse(dr[tableVersion == 4 ? BaseUtil.FieldModifiedOn : BaseUtil.FieldUpdateTime].ToString());
                if (!dr[tableVersion == 4 ? BaseUtil.FieldModifiedUserId : BaseUtil.FieldUpdateUserId].ToString()
                    .Equals(oldUpdateUserId) || newModifiedOn != oldUpdateTime)
                {
                    result = true;
                }
            }

            return result;
        }
        #endregion
    }
}