//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;
using System.Collections.Generic;

namespace DotNet.Util
{
    /// <summary>
    ///	DbUtil
    /// 通用基类
    /// 
    /// 这个类可是修改了很多次啊，已经比较经典了，随着专业的提升，人也会不断提高，技术也会越来越精湛。
    /// 
    /// 修改记录
    /// 
    ///     2011.08.09 版本：4.9    张广梁   修改 public static bool IsModifed(DataRow dr, string oldUpdateUserId, DateTime? oldUpdateTime)的逻辑
    ///		2010.07.08 版本：4.8	JiRiGaLa 增加 Insert 方法。
    ///		2009.01.15 版本：4.7	JiRiGaLa 将方法修改为 static 静态的，提高运行速度。
    ///		2009.01.05 版本：4.6	JiRiGaLa MySql 获取前几个的错误进行修正。
    ///		2008.06.03 版本：4.5	JiRiGaLa IN SQL语句调试、修改错误。
    ///		2008.05.31 版本：4.4	JiRiGaLa 改进 参数名param去掉了，变量名首字母小写了。
    ///		2008.05.09 版本：4.3	JiRiGaLa 改进 树型结构的数据的获得函数部分。
    ///		2007.11.08 版本：4.2	JiRiGaLa 改进 DataTableToStringList 为 FieldToList。
    ///		2007.11.05 版本：4.1	JiRiGaLa 改进 GetDS、GetDataTable 功能，整体思路又上一个台阶，基类的又一次飞跃。
    ///		2007.11.05 版本：4.0	JiRiGaLa 改进 支持不是“Id”为字段的主键表。
    ///		2007.11.01 版本：3.9	JiRiGaLa 改进 BUOperatorInfo 去掉这个变量，可以提高性能，提高速度，基类的又一次飞跃。
    ///		2007.09.13 版本：3.8	JiRiGaLa 改进 BUBaseUtil.SQLLogicConditional 错误。
    ///		2007.08.14 版本：3.7	JiRiGaLa 改进 WebService 模式下 DataSet 传输数据的速度问题。
    ///		2007.07.20 版本：3.6	JiRiGaLa 改进 DataSet 修改为 DataTable 应该能提高一些速度吧。
    ///		2007.05.20 版本：3.6	JiRiGaLa 改进 GetList() 方法整理，这次又应该是一次升华，质的飞跃很不容易啊，身心都有提高了。
    ///		2007.05.20 版本：3.4	JiRiGaLa 改进 Exists() 方法整理。
    ///		2007.05.13 版本：3.3	JiRiGaLa 改进 GetProperty()，SetProperty()，Delete() 方法整理。
    ///		2007.05.10 版本：3.2	JiRiGaLa 改进 GetList() 方法整理。
    ///		2007.04.10 版本：3.1	JiRiGaLa 添加 GetNextId，GetPreviousId 方法整理。
    ///		2007.03.03 版本：3.0	JiRiGaLa 进行了一次彻底的优化工作，方法的位置及功能整理。
    ///		2007.03.01 版本：2.0	JiRiGaLa 重新调整主键的规范化。
    ///		2006.02.05 版本：1.1	JiRiGaLa 重新调整主键的规范化。
    ///		2005.12.30 版本：1.0	JiRiGaLa 数据库连接方式都进行改进
    ///		2005.09.04 版本：1.0	JiRiGaLa 执行数据库脚本
    ///		2005.08.19 版本：1.0	JiRiGaLa 整理一下编排	
    ///		2005.07.10 版本：1.0	JiRiGaLa 修改了程序，格式以及理念都有些提高，应该是一次大突破
    ///		2004.11.12 版本：1.0	JiRiGaLa 添加了最新的GetParent、GetChildren、GetParentChildren 方法
    ///		2004.07.21 版本：1.0	JiRiGaLa UpdateRecord、Delete、SetProperty、GetProperty、ExecuteNonQuery、GetRecord 方法进行改进。
    ///								还删除一些重复的主键，提取了最优化的方法，有时候写的主键真的是垃圾，可能自己也没有注意时就写出了垃圾。
    ///								GetRepeat、GetDayOfWeek、ExecuteProcedure、GetFromProcedure 方法进行改进，基本上把所有的方法都重新写了一遍。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2009.01.15</date>
    /// </author> 
    /// </summary>
    public partial class DbUtil
    {
        #region public static string SqlSafe(string value) 检查参数的安全性
        /// <summary>
        /// 检查参数的安全性
        /// </summary>
        /// <param name="value">参数</param>
        /// <returns>安全的参数</returns>
        public static string SqlSafe(string value)
        {
            value = value.Replace("'", "''");
            // value = value.Replace("%", "'%");
            return value;
        }
        #endregion

        #region public static string GetWhereString(IDbHelper dbHelper, List<KeyValuePair<string, object>> parameters, string relation) 获得条件语句
        /// <summary>
        /// 获得条件语句
       /// </summary>
       /// <param name="dbHelper"></param>
       /// <param name="parameters"></param>
       /// <param name="relation"></param>
       /// <returns></returns>
        public static string GetWhereString(IDbHelper dbHelper, List<KeyValuePair<string, object>> parameters, string relation)
        {
            var result = string.Empty;
            if (parameters == null)
            {
                return result;
            }
            var subSqlQuery = string.Empty;
            foreach (var parameter in parameters)
            {
                if (!string.IsNullOrEmpty(parameter.Key))
                {
                    //if (values[i] == null || string.IsNullOrEmpty(values[i].ToString()))
                    if (parameter.Value == null)
                    {
                        subSqlQuery = " (" + parameter.Key + " IS NULL) ";
                    }
                    else
                    {
                        if (parameter.Value is Array)
                        {
                            if (((Array)parameter.Value).Length > 0)
                            {
                                subSqlQuery = " (" + parameter.Key + " IN (" + StringUtil.ArrayToList((string[])parameter.Value, "'") + ")) ";
                            }
                            else
                            {
                                subSqlQuery = " (" + parameter.Key + " IS NULL) ";
                            }
                        }
                        else
                        {
                            subSqlQuery = " (" + parameter.Key + " = " + dbHelper.GetParameter(parameter.Key) + ") ";
                            //if ((values[i].ToString().IndexOf('[') >= 0) || (values[i].ToString().IndexOf(']') >= 0))
                            //{
                            //    values[i] = values[i].ToString().Replace("[", "/[");
                            //    values[i] = values[i].ToString().Replace("]", "/]");
                            //    values[i] = SqlSafe(values[i].ToString());
                            //    subSqlQuery = " (" + names[i] + " LIKE '" + values[i] + "' ESCAPE '/') ";
                            //    values[i] = null;
                            //    subSqlQuery = " (" + names[i] + " LIKE ? ESCAPE '/') ";
                            //}
                        }
                        // 这里操作，就会有些重复了，不应该进行处理
                        // values[i] = this.SqlSafe(values[i].ToString());
                    }
                    result += subSqlQuery + relation;
                }
            }
            if (result.Length > 0)
            {
                result = result.Substring(0, result.Length - relation.Length - 1);
            }
            return result;
        }
        #endregion

        #region public static string GetWhereString(IDbHelper dbHelper, string[] names, ref Object[] values, string relation) 获得条件语句
        /// <summary>
        /// 获得条件语句
        /// 20110523 吉日嘎拉，改进空数组 
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="names">字段名</param>
        /// <param name="values">字段值</param>
        /// <param name="relation">逻辑关系</param>
        /// <returns>字符串</returns>
        public static string GetWhereString(IDbHelper dbHelper, ref string[] names, Object[] values, string relation)
        {
            var result = string.Empty;
            var subSqlQuery = string.Empty;
            for (var i = 0; i < names.Length; i++)
            {
                if ((names[i] != null) && (names[i].Length > 0))
                {
                    //if (values[i] == null || string.IsNullOrEmpty(values[i].ToString()))
                    if (values[i] == null)
                    {
                        subSqlQuery = " (" + names[i] + " IS NULL) ";
                        // 这里就不需要参数化了
                        names[i] = null;
                    }
                    else
                    {
                        if (values[i] is Array)
                        {
                            if (((Array)values[i]).Length > 0)
                            {
                                subSqlQuery = " (" + names[i] + " IN (" + StringUtil.ArrayToList((string[])values[i], "'") + ")) ";
                            }
                            else
                            {
                                subSqlQuery = " (" + names[i] + " IS NULL) ";
                            }
                            // 这里就不需要参数化了
                            names[i] = null;
                        }
                        else
                        {
                            subSqlQuery = " (" + names[i] + " = " + dbHelper.GetParameter(names[i]) + ") ";
                            //if ((values[i].ToString().IndexOf('[') >= 0) || (values[i].ToString().IndexOf(']') >= 0))
                            //{
                            //    values[i] = values[i].ToString().Replace("[", "/[");
                            //    values[i] = values[i].ToString().Replace("]", "/]");
                            //    values[i] = SqlSafe(values[i].ToString());
                            //    subSqlQuery = " (" + names[i] + " LIKE '" + values[i] + "' ESCAPE '/') ";
                            //    values[i] = null;
                            //    subSqlQuery = " (" + names[i] + " LIKE ? ESCAPE '/') ";
                            //}
                        }
                        // 这里操作，就会有些重复了，不应该进行处理
                        // values[i] = this.SqlSafe(values[i].ToString());
                    }
                    result += subSqlQuery + relation;
                }
            }
            if (result.Length > 0)
            {
                result = result.Substring(0, result.Length - relation.Length - 1);
            }
            return result;
        }
        #endregion

        #region public static int UpdateRecord(IDbHelper dbHelper, string tableName, string name, string value, string targetField, object targetValue) 更新记录
        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表名</param>
        /// <param name="name">主键</param>
        /// <param name="value">主键值</param>
        /// <param name="targetField">更新字段</param>
        /// <param name="targetValue">更新值</param>
        /// <returns>影响行数</returns>
        public static int UpdateRecord(IDbHelper dbHelper, string tableName, string name, string value, string targetField, object targetValue)
        {
            var result = 0;
            var sqlBuilder = new SqlBuilder(dbHelper);
            sqlBuilder.BeginUpdate(tableName);
            sqlBuilder.SetValue(targetField, targetValue);
            sqlBuilder.SetWhere(name, value);
            result = sqlBuilder.EndUpdate();
            return result;
        }
        #endregion

        #region public static int Insert(IDbHelper dbHelper, string tableName, string[] targetFields, Object[] targetValues) 设置属性
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">目标表名</param>
        /// <param name="targetFields">更新字段</param>
        /// <param name="targetValues">更新值</param>
        /// <returns>影响行数</returns>
        public static int Insert(IDbHelper dbHelper, string tableName, string[] targetFields, Object[] targetValues)
        {
            var sqlBuilder = new SqlBuilder(dbHelper);
            sqlBuilder.BeginInsert(tableName);
            for (var i = 0; i < targetFields.Length; i++)
            {
                sqlBuilder.SetValue(targetFields[i], targetValues[i]);
            }
            return sqlBuilder.EndInsert();
        }
        #endregion
        
        #region public static DataTable GetFromProcedure(IDbHelper dbHelper, string procedureName, string tableName) 通过存储过程获取表数据
        /// <summary>
        /// 通过存储过程获取表数据
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="procedureName">存储过程名</param>
        /// <param name="tableName">填充表</param>
        /// <returns>数据权限</returns>
        public static DataTable GetFromProcedure(IDbHelper dbHelper, string procedureName, string tableName)
        {
            var dt = new DataTable(tableName);
            dbHelper.Fill(dt, procedureName, (IDbDataParameter[])null, CommandType.StoredProcedure);
            return dt;
        }
        #endregion

        #region public static DataTable GetFromProcedure(IDbHelper dbHelper, string procedureName, string tableName, string id) 通过存储过程获取表数据
        /// <summary>
        /// 通过存储过程获取表数据
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="procedureName">存储过程名</param>
        /// <param name="tableName">填充表</param>
        /// <param name="id">主键值</param>
        /// <returns>数据权限</returns>
        public static DataTable GetFromProcedure(IDbHelper dbHelper, string procedureName, string tableName, string id)
        {
            var names = new string[1];
            var values = new Object[1];
            names[0] = BaseUtil.FieldId;
            values[0] = id;
            var dt = new DataTable(tableName);
            dbHelper.Fill(dt, procedureName, dbHelper.MakeParameters(names, values), CommandType.StoredProcedure);
            return dt;
        }
        #endregion

		/// <summary>
		/// 构造个List
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="param"></param>
		/// <returns></returns>
		public static List<T> GetList<T>(T param)
		{
			var result = new List<T> { param };
            return result;
		}
    }
}