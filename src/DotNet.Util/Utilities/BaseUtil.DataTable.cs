//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;
using System.Linq;

namespace DotNet.Util
{
    /// <summary>
    ///	BaseUtil
    /// 通用基类
    /// 
    /// 这个类可是修改了很多次啊，已经比较经典了，随着专业的提升，人也会不断提高，技术也会越来越精湛。
    /// 
    /// 修改记录
    /// 
    ///		2012.04.05 版本：1.0	JiRiGaLa 改进 GetPermissionScope(string[] organizeIds)。
    ///	
    /// <author>
    ///		<name>Troy Cui</name>
    ///		<date>2012.04.05</date>
    /// </author> 
    /// </summary>
    public partial class BaseUtil
    {
        #region public static string GetDateTime(DataRow dr, string name) 获取日期时间
        /// <summary>
        /// 获取日期时间
        /// </summary>
        /// <param name="dr">数据行</param>
        /// <param name="name">字段名</param>
        /// <returns>日期时间</returns>
        public static string GetDateTime(DataRow dr, string name)
        {
            var result = string.Empty;
            if (!dr.IsNull(name))
            {
                var dateTime = DateTime.Parse(dr[name].ToString());
                result = dateTime.ToString(BaseSystemInfo.DateFormat);
            }
            return result;
        }
        #endregion

        #region public const string FieldToList(DataTable dt) 表格字段转换为字符串列表
        /// <summary>
        /// 表格字段转换为字符串列表
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns>字段值字符串</returns>
        public static string FieldToList(DataTable dt)
        {
            return FieldToList(dt, FieldId);
        }
        #endregion

        #region public const string FieldToList(DataTable dt, string name) 表格字段转换为字符串列表
        /// <summary>
        /// 表格字段转换为字符串列表
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="name">列名</param>
        /// <returns>字段值字符串</returns>
        public static string FieldToList(DataTable dt, string name)
        {
            var rowCount = 0;
            var result = "'";
            foreach (DataRow dr in dt.Rows)
            {
                rowCount++;
                result += dr[name] + "', '";
            }
            if (rowCount == 0)
            {
                result = "''";
            }
            else
            {
                result = result.Substring(0, result.Length - 3);
            }
            return result;
        }
        #endregion

        #region public static string[] FieldToArray(DataTable dt, string field) dt某列转换为字符串数组
        /// <summary>
        /// dt某列转换为字符串数组
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="field">列名</param>
        /// <returns>字段值数组</returns>
        public static string[] FieldToArray(DataTable dt, string field)
        {
            return dt.Select().Select(n => n[field].ToString()).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
        }
        #endregion


        #region public static DataTable SetFilter(DataTable dt, string fieldName, string fieldValue, bool equals = false) 对数据表进行过滤
        /// <summary>
        /// 对数据表进行过滤
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="fieldValue">字段值</param>
        /// <param name="equals">相等</param>
        /// <returns>数据权限</returns>
        public static DataTable SetFilter(DataTable dt, string fieldName, string fieldValue, bool equals = false)
        {
            foreach (DataRow dr in dt.Rows)
            {
                // 要求把相等的删除掉
                if (equals)
                {
                    if (string.IsNullOrEmpty(fieldValue))
                    {
                        if (string.IsNullOrEmpty(dr[fieldName].ToString()))
                        {
                            dr.Delete();
                        }
                    }
                    else
                    {
                        if (dr[fieldName].ToString().Equals(fieldValue, StringComparison.OrdinalIgnoreCase))
                        {
                            dr.Delete();
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(fieldValue))
                    {
                        if (!string.IsNullOrEmpty(dr[fieldName].ToString()))
                        {
                            dr.Delete();
                        }
                    }
                    else
                    {
                        if (!dr[fieldName].ToString().Equals(fieldValue, StringComparison.OrdinalIgnoreCase))
                        {
                            dr.Delete();
                        }
                    }
                }
            }
            dt.AcceptChanges();
            return dt;
        }
        #endregion

        #region public static string GetProperty(DataTable dt, string id, string targetField) 读取一个属性
        /// <summary>
        /// 读取一个属性
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="id">主键</param>
        /// <param name="targetField">目标字段</param>
        /// <returns>目标值</returns>
        public static string GetProperty(DataTable dt, string id, string targetField)
        {
            return GetProperty(dt, FieldId, id, targetField);
        }
        public static string GetPropertyDyn(dynamic lstT, string id, string targetField)
        {
            return GetPropertyDyn(lstT, FieldId, id, targetField);
        }
        public static string GetPropertyDyn(dynamic lstT, string fieldName, string fieldValue, string targetField)
        {
            var result = string.Empty;
            foreach (var t in lstT)
            {
                if (ReflectionUtil.GetProperty(t, fieldName).ToString().Equals(fieldValue))
                {
                    result =ReflectionUtil.GetProperty(t, targetField).ToString();
                    break;
                }
            }
            return result;
        }
        #endregion

        #region public static string GetProperty(DataTable dt, string fieldName, string fieldValue, string targetField) 读取一个属性
        /// <summary>
        /// 读取一个属性
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="fieldName">字段</param>
        /// <param name="fieldValue">值</param>
        /// <param name="targetField">目标字段</param>
        /// <returns>目标值</returns>
        public static string GetProperty(DataTable dt, string fieldName, string fieldValue, string targetField)
        {
            var result = string.Empty;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[fieldName].ToString().Trim().Equals(fieldValue))
                {
                    result = dr[targetField].ToString();
                    break;
                }
            }
            return result;
        }
        #endregion

        #region public static int SetProperty(DataTable dt, string id, string targetField, object targetValue) 设置一个属性
        /// <summary>
        /// 设置一个属性
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="id">主键</param>
        /// <param name="targetField">更新字段</param>
        /// <param name="targetValue">目标值</param>
        /// <returns>影响行数</returns>
        public static int SetProperty(DataTable dt, string id, string targetField, object targetValue)
        {
            return SetProperty(dt, FieldId, id, targetField, targetValue);
        }
        public static int SetPropertyDyn(dynamic lstT, string id, string targetField, object targetValue)
        {
            return SetPropertyDyn(lstT, FieldId, id, targetField, targetValue);
        }
        #endregion

        #region public static int SetProperty(DataTable dt, string fieldName, string fieldValue, string targetField, object targetValue) 设置一个属性
        /// <summary>
        /// 设置一个属性
        /// </summary>        
        /// <param name="dt">数据表</param>
        /// <param name="fieldName">字段</param>
        /// <param name="fieldValue">值</param>
        /// <param name="targetField">更新字段</param>
        /// <param name="targetValue">目标值</param>
        /// <returns>影响行数</returns>
        public static int SetProperty(DataTable dt, string fieldName, string fieldValue, string targetField, object targetValue)
        {
            var result = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    if (dr[fieldName].ToString().Equals(fieldValue, StringComparison.OrdinalIgnoreCase))
                    {
                        dr[targetField] = targetValue;
                        result++;
                        // break;
                    }
                }
            }
            return result;
        }
        public static int SetPropertyDyn(dynamic lstT, string fieldName, string fieldValue, string targetField, object targetValue)
        {
            var result = 0;
            for (var i = 0; i < lstT.Count; i++) {
                var t = lstT[i];
                if (ReflectionUtil.GetProperty(t, fieldName).ToString().Equals(fieldValue))
                {
                    ReflectionUtil.SetProperty(t, targetField, targetValue);
                    lstT[i] = t;
                    result++;
                    // break;
                }
            }            
            return result;
        }
        #endregion

        #region public static int Delete(DataTable dt, string id) 删除记录
        /// <summary>
        /// 删除一条记录
        /// </summary>        
        /// <param name="dt">数据表名</param>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public static int Delete(DataTable dt, string id)
        {
            return Delete(dt, FieldId, id);
        }
        #endregion

        #region public static int Delete(DataTable dt, string fieldName, string fieldValue) 删除记录
        /// <summary>
        /// 删除一条记录
        /// </summary>        
        /// <param name="dt">数据表名</param>
        /// <param name="fieldName">字段</param>
        /// <param name="fieldValue">值</param>
        /// <returns>影响行数</returns>
        public static int Delete(DataTable dt, string fieldName, string fieldValue)
        {
            var result = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[fieldName].ToString().Equals(fieldValue, StringComparison.OrdinalIgnoreCase))
                {
                    dr.Delete();
                    result++;
                    break;
                }
            }
            return result;
        }
        #endregion

        #region public static DataRow GetDataRow(DataTable dt, string id) 从数据权限读取一行数据
        /// <summary>
        /// 从数据权限读取一行数据
        /// </summary>        
        /// <param name="dt">数据表</param>
        /// <param name="id">主键</param>
        /// <returns>数据行</returns>
        public static DataRow GetDataRow(DataTable dt, string id)
        {
            return GetDataRow(dt, FieldId, id);
        }
        #endregion

        #region public static DataRow GetDataRow(DataTable dt, string fieldName, string fieldValue) 从数据权限读取一行数据
        /// <summary>
        /// 从数据权限读取一行数据
        /// </summary>        
        /// <param name="dt">数据表</param>
        /// <param name="fieldName">字段</param>
        /// <param name="fieldValue">值</param>
        /// <returns>数据行</returns>
        public static DataRow GetDataRow(DataTable dt, string fieldName, string fieldValue)
        {
            DataRow result = null;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    if (dr[fieldName].ToString().Equals(fieldValue, StringComparison.OrdinalIgnoreCase))
                    {
                        result = dr;
                        break;
                    }
                }
            }
            return result;
        }
        #endregion
    }
}