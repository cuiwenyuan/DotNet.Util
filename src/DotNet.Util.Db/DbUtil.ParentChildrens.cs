//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;
using System.Linq;

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
        //
        // 树型结构的算法
        //

        #region public static DataTable GetParentsByCode(this IDbHelper dbHelper, string tableName, string fieldCode, string code, string order) 获取父节点列表
        /// <summary>
        /// 获取父节点列表
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表明</param>
        /// <param name="fieldCode">编码字段</param>
        /// <param name="code">编码</param>
        /// <param name="order">排序</param>
        /// <param name="idOnly">只需要主键</param>
        /// <returns>数据表</returns>
        public static DataTable GetParentsByCode(this IDbHelper dbHelper, string tableName, string fieldCode, string code, string order, bool idOnly = false)
        {
            var sb = Pool.StringBuilder.Get();
            if (idOnly)
            {
                sb.Append("SELECT " + BaseUtil.FieldId);
            }
            else
            {
                sb.Append("SELECT * ");
            }
            sb.Append(" FROM " + tableName);
            switch (dbHelper.CurrentDbType)
            {
                case CurrentDbType.Access:
                case CurrentDbType.SqlServer:
                    sb.Append(" WHERE (LEFT(" + dbHelper.GetParameter(fieldCode) + ", LEN(" + fieldCode + ")) = " + fieldCode + ") ");
                    break;
                case CurrentDbType.Oracle:
                    sb.Append(" WHERE (SUBSTR(" + dbHelper.GetParameter(fieldCode) + ", 1, LENGTH(" + fieldCode + ")) = " + fieldCode + ") ");
                    break;
            }
            if (!string.IsNullOrEmpty(order))
            {
                sb.Append(" ORDER BY " + order);
            }
            var names = new string[1];
            var values = new Object[1];
            names[0] = fieldCode;
            values[0] = code;
            var dt = new DataTable(tableName);
            dbHelper.Fill(dt, sb.Put(), dbHelper.MakeParameters(names, values));
            return dt;
        }
        #endregion

        #region public static DataTable GetChildrens(this IDbHelper dbHelper, string tableName, string fieldId, string id, string fieldParentId, string order, bool idOnly) 获取子节点列表
        /// <summary>
        /// 获取子节点列表
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表明</param>
        /// <param name="fieldId">主键字段</param>
        /// <param name="id">值</param>
        /// <param name="fieldParentId">父亲节点字段</param>
        /// <param name="order">排序</param>
        /// <param name="idOnly">只需要主键</param>
        /// <returns>数据表</returns>
        public static DataTable GetChildrens(this IDbHelper dbHelper, string tableName, string fieldId, string id, string fieldParentId = null, string order = null, bool idOnly = false)
        {
            var sb = Pool.StringBuilder.Get();
            var dt = new DataTable(tableName);
            if (dbHelper.CurrentDbType == CurrentDbType.Oracle)
            {
                if (idOnly)
                {
                    sb.Append("   SELECT " + fieldId);
                }
                else
                {
                    sb.Append("   SELECT * ");
                }
                sb.Append(" FROM " + tableName
                         + "    START WITH " + fieldParentId + " = " + dbHelper.GetParameter(fieldId)
                         + "  CONNECT BY PRIOR " + fieldId + " = " + fieldParentId);
                if (!string.IsNullOrEmpty(order))
                {
                    sb.Append(" ORDER BY " + order);
                }
                var names = new string[1];
                names[0] = fieldId;
                var values = new Object[1];
                values[0] = id;
                dbHelper.Fill(dt, sb.Put(), dbHelper.MakeParameters(names, values));
            }
            else if (dbHelper.CurrentDbType == CurrentDbType.SqlServer)
            {
                if (idOnly)
                {
                    sb.Append(" WITH Tree AS (SELECT " + fieldId
                             + " FROM " + tableName
                             + "       WHERE " + fieldParentId + " IN ('" + id + "') "
                             + "       UNION ALL "
                             + "      SELECT ResourceTree." + fieldId
                             + " FROM " + tableName + " AS ResourceTree INNER JOIN "
                             + "             Tree AS A ON A." + fieldId + " = ResourceTree." + fieldParentId + ") "
                             + "SELECT " + fieldId
                             + " FROM Tree ");
                }
                else
                {
                    sb.Append(" WITH Tree AS (SELECT * "
                             + " FROM " + tableName
                             + "       WHERE Id IN ('" + id + "') "
                             + "       UNION ALL "
                             + "      SELECT ResourceTree.* "
                             + " FROM " + tableName + " AS ResourceTree INNER JOIN "
                             + "             Tree AS A ON A." + fieldId + " = ResourceTree." + fieldParentId + ") "
                             + "SELECT * "
                             + " FROM Tree ");
                }
                dbHelper.Fill(dt, sb.Put());
            }
            return dt;
        }
        #endregion

        #region public static DataTable GetChildrens(this IDbHelper dbHelper, string tableName, string fieldId, string[] ids, string fieldParentId, string order, bool idOnly) 获取子节点列表
        /// <summary>
        /// 获取子节点列表
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表明</param>
        /// <param name="fieldId">主键字段</param>
        /// <param name="ids">主键数组</param>
        /// <param name="fieldParentId">父亲节点字段</param>
        /// <param name="order">排序</param>
        /// <param name="idOnly">只需要主键</param>
        /// <returns>数据表</returns>
        public static DataTable GetChildrens(this IDbHelper dbHelper, string tableName, string fieldId, string[] ids, string fieldParentId, string order, bool idOnly)
        {
            var sb = Pool.StringBuilder.Get();
            if (idOnly)
            {
                sb.Append("SELECT " + fieldId);
            }
            else
            {
                sb.Append("SELECT * ");
            }
            sb.Append(" FROM " + tableName
                     + " START WITH " + fieldId + " IN (" + string.Join(",", ids) + ")"
                     + " CONNECT BY PRIOR " + fieldId + " = " + fieldParentId);
            if (!string.IsNullOrEmpty(order))
            {
                sb.Append(" ORDER BY " + order);
            }
            return dbHelper.Fill(sb.Put());
        }
        #endregion

        #region public static DataTable GetChildrensByCode(this IDbHelper dbHelper, string tableName, string fieldCode, string code, string order, bool idOnly) 获取子节点列表
        /// <summary>
        /// 获取子节点列表
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表明</param>
        /// <param name="fieldCode">编码字段</param>
        /// <param name="code">编码</param>
        /// <param name="order">排序</param>
        /// <param name="idOnly">只需要主键</param>
        /// <returns>数据表</returns>
        public static DataTable GetChildrensByCode(this IDbHelper dbHelper, string tableName, string fieldCode, string code, string order, bool idOnly = false)
        {
            var sb = Pool.StringBuilder.Get();
            if (idOnly)
            {
                sb.Append("SELECT " + BaseUtil.FieldId);
            }
            else
            {
                sb.Append("SELECT * ");
            }
            sb.Append(" FROM " + tableName);
            switch (dbHelper.CurrentDbType)
            {
                case CurrentDbType.Access:
                case CurrentDbType.SqlServer:
                    sb.Append(" WHERE (LEFT(" + fieldCode + ", LEN('" + code + "')) = '" + code + "') ");
                    break;
                case CurrentDbType.Oracle:
                    sb.Append(" WHERE (SUBSTR(" + fieldCode + ", 1, LENGTH('" + code + "')) = '" + code + "') ");
                    break;
            }
            if (!string.IsNullOrEmpty(order))
            {
                sb.Append(" ORDER BY " + order);
            }
            var dt = new DataTable(tableName);
            dbHelper.Fill(dt, sb.Put());
            return dt;
        }
        #endregion

        #region public static DataTable GetParentChildrensByCode(this IDbHelper dbHelper, string tableName, string fieldCode, string code, string order, bool idOnly) 获取父子节点列表
        /// <summary>
        /// 获取父子节点列表
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表明</param>
        /// <param name="fieldCode">编码字段</param>
        /// <param name="code">编码</param>
        /// <param name="order">排序</param>
        /// <param name="idOnly">只需要主键</param>
        /// <returns>数据表</returns>
        public static DataTable GetParentChildrensByCode(this IDbHelper dbHelper, string tableName, string fieldCode, string code, string order, bool idOnly = false)
        {
            var sb = Pool.StringBuilder.Get();
            if (idOnly)
            {
                sb.Append("SELECT " + BaseUtil.FieldId);
            }
            else
            {
                sb.Append("SELECT * ");
            }
            sb.Append(" FROM " + tableName);
            switch (dbHelper.CurrentDbType)
            {
                case CurrentDbType.Access:
                case CurrentDbType.SqlServer:
                    sb.Append(" WHERE (LEFT(" + fieldCode + ", LEN(" + dbHelper.GetParameter(fieldCode) + ")) = " + dbHelper.GetParameter(fieldCode) + ") ");
                    sb.Append(" OR (LEFT(" + dbHelper.GetParameter(fieldCode) + ", LEN(" + fieldCode + ")) = " + fieldCode + ") ");
                    break;
                case CurrentDbType.Oracle:
                    sb.Append(" WHERE (SUBSTR(" + fieldCode + ", 1, LENGTH(" + dbHelper.GetParameter(fieldCode) + ")) = " + dbHelper.GetParameter(fieldCode) + ") ");
                    sb.Append(" OR (" + fieldCode + " = SUBSTR(" + dbHelper.GetParameter(fieldCode) + ", 1, LENGTH(" + fieldCode + "))) ");
                    break;
            }
            if (!string.IsNullOrEmpty(order))
            {
                sb.Append(" ORDER BY " + order);
            }
            var names = new string[3];
            names[0] = fieldCode;
            names[1] = fieldCode;
            names[2] = fieldCode;
            var values = new Object[3];
            values[0] = code;
            values[1] = code;
            values[2] = code;
            var dt = new DataTable("DotNet");
            dbHelper.Fill(dt, sb.Put(), dbHelper.MakeParameters(names, values));
            return dt;
        }
        #endregion

        
        #region public static string[] GetParentsIdByCode(this IDbHelper dbHelper, string tableName, string fieldCode, string code, string order) 获取父节点列表
        /// <summary>
        /// 获取父节点列表
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表明</param>
        /// <param name="fieldCode">编码字段</param>
        /// <param name="code">编码</param>
        /// <param name="order">排序</param>
        /// <returns>主键数组</returns>
        public static string[] GetParentsIdByCode(this IDbHelper dbHelper, string tableName, string fieldCode, string code, string order)
        {
            return BaseUtil.FieldToArray(GetParentsByCode(dbHelper, tableName, fieldCode, code, order, true), BaseUtil.FieldId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
        }
        #endregion

        #region public static string[] GetChildrensId(this IDbHelper dbHelper, string tableName, string fieldId, string id, string fieldParentId, string order) 获取子节点列表
        /// <summary>
        /// 获取子节点列表
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表明</param>
        /// <param name="fieldId">主键字段</param>
        /// <param name="id">值</param>
        /// <param name="fieldParentId">父亲节点字段</param>
        /// <param name="order">排序</param>
        /// <returns>主键数组</returns>
        public static string[] GetChildrensId(this IDbHelper dbHelper, string tableName, string fieldId, string id, string fieldParentId, string order)
        {
            return BaseUtil.FieldToArray(GetChildrens(dbHelper, tableName, fieldId, id, fieldParentId, order, true), BaseUtil.FieldId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
        }
        #endregion

        #region public static string[] GetChildrensIdByCode(this IDbHelper dbHelper, string tableName, string fieldCode, string code, string order) 获取子节点列表
        /// <summary>
        /// 获取子节点列表
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表明</param>
        /// <param name="fieldCode">编码字段</param>
        /// <param name="code">编码</param>
        /// <param name="order">排序</param>
        /// <returns>主键数组</returns>
        public static string[] GetChildrensIdByCode(this IDbHelper dbHelper, string tableName, string fieldCode, string code, string order)
        {
            return BaseUtil.FieldToArray(GetChildrensByCode(dbHelper, tableName, fieldCode, code, order, true), BaseUtil.FieldId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
        }
        #endregion

        #region public static string[] GetParentChildrensIdByCode(this IDbHelper dbHelper, string tableName, string fieldCode, string code, string order) 获取父子节点列表
        /// <summary>
        /// 获取父子节点列表
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表明</param>
        /// <param name="fieldCode">编码字段</param>
        /// <param name="code">编码</param>
        /// <param name="order">排序</param>
        /// <returns>主键数组</returns>
        public static string[] GetParentChildrensIdByCode(this IDbHelper dbHelper, string tableName, string fieldCode, string code, string order)
        {
            return BaseUtil.FieldToArray(GetParentChildrensByCode(dbHelper, tableName, fieldCode, code, order, true), BaseUtil.FieldId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
        }
        #endregion


        #region public static string GetParentIdByCode(this IDbHelper dbHelper, string tableName, string fieldCode, string code) 获取父节点
        /// <summary>
        /// 获取父节点
        /// </summary>
        /// <param name="dbHelper">数据库帮助接口</param>
        /// <param name="tableName">目标表名</param>
        /// <param name="fieldCode">编码字段</param>
        /// <param name="code">编号</param>
        /// <returns>主键</returns>
        public static string GetParentIdByCode(this IDbHelper dbHelper, string tableName, string fieldCode, string code)
        {
            var parentId = string.Empty;
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT MAX(Id) AS Id "
                      + " FROM " + tableName
                      + "  WHERE (LEFT(" + dbHelper.GetParameter(fieldCode) + ", LEN(" + fieldCode + ")) = " + fieldCode + ") "
                      + "    AND " + fieldCode + " <>  '" + code + " ' ");
            var names = new string[1];
            var values = new Object[1];
            names[0] = fieldCode;
            values[0] = code;
            var returnObject = dbHelper.ExecuteScalar(sb.Put(), dbHelper.MakeParameters(names, values));
            if (returnObject != null)
            {
                parentId = returnObject.ToString();
            }
            return parentId;
        }
        #endregion
    }
}