﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace DotNet.Business
{
    using Util;

    /// <summary>
    /// SQLBuilder
    /// SQL语句生成器（适合简单的添加、删除、更新等语句，可以写出编译时强类型检查的效果）
    /// 
    /// 修改记录
    /// 
    ///     2013.04.01 版本：4.0 JiRiGaLa   改进性能，采用StringBuilder。
    ///     2012.03.17 版本：3.7 zhangyi    修改注释
    ///     2010.06.20 版本：3.1 JiRiGaLa	支持Oracle序列功能改进。
    ///     2010.06.13 版本：3.0 JiRiGaLa	改进为支持静态方法，不用数据库Open、Close的方式，AutoOpenClose开关。
    ///     2008.08.30 版本：2.3 JiRiGaLa	确认 BeginSelect 方法的正确性。
    ///     2008.08.29 版本：2.2 JiRiGaLa	改进 public string SetWhere(string targetFiled, Object[] targetValue) 方法。
    ///     2008.08.29 版本：2.1 JiRiGaLa	修正 BeginSelect、BeginInsert、BeginUpdate、BeginDelete。
    ///     2008.05.07 版本：2.0 JiRiGaLa	改进为多种数据库的支持类型。
    ///     2007.05.20 版本：1.8 JiRiGaLa	改进了OleDbCommand使其可以在多个事件穿插使用。
    ///     2006.02.22 版本：1.7 JiRiGaLa	改进了OleDbCommand使其可以在多个事件穿插使用。
    ///		2006.02.05 版本：1.6 JiRiGaLa	重新调整主键的规范化。
    ///		2006.01.20 版本：1.5 JiRiGaLa   修改主键,货币型的插入。
    ///		2005.12.29 版本：1.4 JiRiGaLa   修改主键,将公式的功能完善,提高效率。
    ///		2005.12.29 版本：1.3 JiRiGaLa   修改主键,将公式的功能完善,提高效率。
    ///		2005.08.08 版本：1.2 JiRiGaLa   修改主键，修改格式。
    ///		2005.12.30 版本：1.1 JiRiGaLa   数据库连接进行优化。
    ///		2005.12.29 版本：1.0 JiRiGaLa   主键创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2013.04.01</date>
    /// </author> 
    /// </summary>
    public partial class SqlBuilder
    {
        /// <summary>
        /// 是否采用自增量的方式
        /// </summary>
        public bool Identity = false;

        /// <summary>
        ///  是否需要返回主键
        /// </summary>
        public bool ReturnId = true;

        /// <summary>
        /// 主键
        /// </summary>
        public string PrimaryKey = "Id";

        private DbOperation _sqlOperation = DbOperation.Update;
        /// <summary>
        /// 语句
        /// </summary>
        public string CommandText = string.Empty;

        private string _selectSql = string.Empty;

        private string _tableName = string.Empty;

        private StringBuilder _insertValue = null;

        private StringBuilder _insertField = null;

        private StringBuilder _updateSql = null;

        private StringBuilder _whereSql = null;

        private CurrentDbType _dbType = CurrentDbType.SqlServer;

        /// <summary>
        /// 获取前几条数据
        /// </summary>
        private int? _topN = null;

        /// <summary>
        /// 排序字段
        /// </summary>
        private string _orderBy = string.Empty;

        private IDbHelper _dbHelper = null;

        /// <summary>
        /// DbParameters
        /// </summary>
        public List<KeyValuePair<string, object>> DbParameters = new List<KeyValuePair<string, object>>();

        /// <summary>
        /// SqlBuilder
        /// </summary>
        /// <param name="currentDbType"></param>
        public SqlBuilder(CurrentDbType currentDbType)
        {
            _dbType = currentDbType;
            DbParameters = new List<KeyValuePair<string, object>>();
        }

        /// <summary>
        /// SqlBuilder
        /// </summary>
        /// <param name="dbHelper"></param>
        public SqlBuilder(IDbHelper dbHelper)
            : this(dbHelper.CurrentDbType)
        {
            _dbHelper = dbHelper;
        }

        /// <summary>
        /// SqlBuilder
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="identity"></param>
        public SqlBuilder(IDbHelper dbHelper, bool identity)
            : this(dbHelper)
        {
            Identity = identity;
        }

        /// <summary>
        /// SqlBuilder
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="identity"></param>
        /// <param name="returnId"></param>
        public SqlBuilder(IDbHelper dbHelper, bool identity, bool returnId)
            : this(dbHelper)
        {
            Identity = identity;
            ReturnId = returnId;
        }

        #region private void Prepare() 获得数据库连接相关
        /// <summary>
        /// 获得数据库连接
        /// </summary>
        private void Prepare()
        {
            _sqlOperation = DbOperation.Update;
            CommandText = string.Empty;
            _tableName = string.Empty;
            _insertValue = Pool.StringBuilder.Get();
            _insertField = Pool.StringBuilder.Get();
            _updateSql = Pool.StringBuilder.Get();
            _whereSql = Pool.StringBuilder.Get();

            // 2016-02-23 吉日嘎拉，提高性能，释放数据库连接，需要时再打开数据库连接，判断是否为空，要区别静态方法与动态调用方法
            /*
            if (DbHelper != null && !DbHelper.MustCloseConnection)
            {
                DbHelper.GetDbCommand().Parameters.Clear();
            }
            */
        }
        #endregion

        #region public void BeginSelect(string tableName) 开始查询
        /// <summary>
        /// 开始查询
        /// </summary>
        /// <param name="tableName">目标表</param>
        public void BeginSelect(string tableName)
        {
            Begin(tableName, DbOperation.Select);
        }
        #endregion

        /// <summary>
        /// 获取前几条数据
        /// </summary>
        /// <param name="topN">几条</param>
        public void SelectTop(int? topN)
        {
            _topN = topN;
        }

        #region public void BeginInsert(string tableName) 开始插入
        /// <summary>
        /// 开始插入
        /// </summary>
        /// <param name="tableName">目标表</param>
        public void BeginInsert(string tableName)
        {
            Begin(tableName, DbOperation.Insert);
        }
        #endregion

        #region public void BeginReplace(string tableName) 开始插入
        /// <summary>
        /// 更新替换
        /// </summary>
        /// <param name="tableName">目标表</param>
        public void BeginReplace(string tableName)
        {
            Begin(tableName, DbOperation.ReplaceInto);
        }
        #endregion

        #region public void BeginInsert(string tableName, bool identity) 开始插入 传入是否自增
        /// <summary>
        /// 开始插入  传入是否自增
        /// </summary>
        /// <param name="tableName">目标表</param>
        /// <param name="identity">自增量方式</param>
        public void BeginInsert(string tableName, bool identity)
        {
            Identity = identity;
            Begin(tableName, DbOperation.Insert);
        }
        #endregion

        #region public void BeginInsert(string tableName, string primaryKey) 开始插入 传入主键
        /// <summary>
        /// 开始插入 传入主键
        /// </summary>
        /// <param name="tableName">目标表</param>
        /// <param name="primaryKey">主键</param>
        public void BeginInsert(string tableName, string primaryKey)
        {
            PrimaryKey = primaryKey;
            Begin(tableName, DbOperation.Insert);
        }
        #endregion

        #region public BeginUpdate(string tableName) 开始更新
        /// <summary>
        /// 开始更新
        /// </summary>
        /// <param name="tableName">目标表</param>
        public void BeginUpdate(string tableName)
        {
            Begin(tableName, DbOperation.Update);
        }
        #endregion

        #region public void BeginDelete(string tableName) 开始删除
        /// <summary>
        /// 开始删除
        /// </summary>
        /// <param name="tableName">目标表</param>
        public void BeginDelete(string tableName)
        {
            Begin(tableName, DbOperation.Delete);
        }
        #endregion

        #region public void BeginInsert(string tableName,string primaryKey, bool identity) 开始插入 传入是否自增
        /// <summary>
        /// 开始插入  传入是否自增
        /// </summary>
        /// <param name="tableName">目标表</param>
        /// <param name="primaryKey">主键</param>
        /// <param name="identity">自增量方式</param>
        public void BeginInsert(string tableName, string primaryKey, bool identity)
        {
            Identity = identity;
            PrimaryKey = primaryKey;
            Begin(tableName, DbOperation.Insert);
        }
        #endregion

        #region private void Begin(string tableName, DbOperation dbOperation) 开始增删改查
        /// <summary>
        /// 开始查询语句
        /// </summary>
        /// <param name="tableName">目标表</param>
        /// <param name="dbOperation">语句操作类别</param>
        private void Begin(string tableName, DbOperation dbOperation)
        {
            // 写入调试信息
#if (DEBUG)
            int milliStart = Environment.TickCount;
            Trace.WriteLine(DateTime.Now.ToString(BaseSystemInfo.TimeFormat) + " :Begin: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
#endif

            Prepare();
            _tableName = tableName;
            _sqlOperation = dbOperation;

            // 写入调试信息
#if (DEBUG)
            int milliEnd = Environment.TickCount;
            Trace.WriteLine(DateTime.Now.ToString(BaseSystemInfo.TimeFormat) + " Ticks: " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " :End: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
#endif
        }
        #endregion


        #region public void SetFormula(string targetFiled, string formula, string relation) 设置公式
        /// <summary>
        /// 设置公式
        /// </summary>
        /// <param name="targetFiled">目标字段</param>
        /// <param name="formula">公式</param>
        public void SetFormula(string targetFiled, string formula)
        {
            var relation = " = ";
            SetFormula(targetFiled, formula, relation);
        }
        /// <summary>
        /// 设置公式
        /// </summary>
        /// <param name="targetFiled">目标字段</param>
        /// <param name="formula">公式</param>
        /// <param name="relation">关系</param>
        public void SetFormula(string targetFiled, string formula, string relation)
        {
            if (_sqlOperation == DbOperation.Insert)
            {
                _insertField.Append(targetFiled + ", ");
                _insertValue.Append(formula + ", ");
            }
            if (_sqlOperation == DbOperation.Update)
            {
                _updateSql.Append(targetFiled + relation + formula + ", ");
            }
        }
        #endregion

        private string GetDbNow()
        {
            var result = string.Empty;
            if (_dbHelper != null)
            {
                result = _dbHelper.GetDbNow();
            }
            else
            {
                result = DbHelper.GetDbNow(_dbType);
            }
            return result;
        }

        #region public void SetDBNow(string targetFiled) 设置为当前时间
        /// <summary>
        /// 设置为当前时间
        /// </summary>
        /// <param name="targetFiled">目标字段</param>
        public void SetDbNow(string targetFiled)
        {
            if (_sqlOperation == DbOperation.Insert)
            {
                _insertField.Append(targetFiled + ", ");
                _insertValue.Append(GetDbNow() + ", ");
            }
            if (_sqlOperation == DbOperation.Update)
            {
                _updateSql.Append(targetFiled + " = " + GetDbNow() + ", ");
            }
        }
        #endregion

        #region public void SetNull(string targetFiled) 设置为Null值
        /// <summary>
        /// 设置为Null值
        /// </summary>
        /// <param name="targetFiled">目标字段</param>
        public void SetNull(string targetFiled)
        {
            SetValue(targetFiled, null);
        }
        #endregion

        #region public void SetValue(string targetFiled, object targetValue, string targetFiledName = null) 设置值
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="targetFiled">目标字段</param>
        /// <param name="targetValue">值</param>
        /// <param name="targetFiledName">字段名</param>
        public void SetValue(string targetFiled, object targetValue, string targetFiledName = null)
        {
            if (targetFiledName == null)
            {
                targetFiledName = targetFiled;
            }
            switch (_sqlOperation)
            {
                case DbOperation.Update:
                    if (targetValue == null)
                    {
                        _updateSql.Append(targetFiled + " = NULL, ");
                    }
                    else
                    {
                        // 判断数据库连接类型
                        _updateSql.Append(targetFiled + " = " + DbHelper.GetParameter(_dbType, targetFiledName) + ", ");
                        AddParameter(targetFiledName, targetValue);
                        //else
                        //{
                        //    this.UpdateSql.Append(targetFiled + " = '', ");
                        //}
                    }
                    break;
                case DbOperation.Insert:
                    // if (DbHelper.CurrentDbType == CurrentDbType.SqlServer)
                    // else if (DbHelper.CurrentDbType == CurrentDbType.Access)
                    // 自增量，不需要赋值
                    // if (this.Identity && targetFiled == this.PrimaryKey)
                    _insertField.Append(targetFiled + ", ");

                    if (targetValue == null)
                    {
                        _insertValue.Append(" NULL, ");
                    }
                    else
                    {
                        _insertValue.Append(DbHelper.GetParameter(_dbType, targetFiledName) + ", ");
                        AddParameter(targetFiledName, targetValue);
                    }
                    break;
            }
        }
        #endregion

        #region private void AddParameter(string targetFiled, object targetValue) 添加参数
        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="targetFiled">目标字段</param>
        /// <param name="targetValue">值</param>
        private void AddParameter(string targetFiled, object targetValue)
        {
            DbParameters.Add(new KeyValuePair<string, object>(targetFiled, targetValue));
        }
        #endregion


        #region public string SetWhere(string sqlWhere) 设置条件
        /// <summary>
        /// 设置条件
        /// </summary>
        /// <param name="sqlWhere">目标字段</param>
        /// <returns>条件语句</returns>
        public void SetWhere(string sqlWhere)
        {
            if (_whereSql == null || _whereSql.Length == 0)
            {
                _whereSql.Put();
                _whereSql = Pool.StringBuilder.Get();
                _whereSql.Append(" WHERE ");
            }
            _whereSql.Append(sqlWhere);
            // return this.WhereSql;
        }
        #endregion

        #region public string SetWhere(string targetFiled, object targetValue, string targetFiledName = null, string relation = " AND ") 设置条件

        /// <summary>
        /// 设置条件
        /// </summary>
        /// <param name="targetFiled">目标字段</param>
        /// <param name="targetValue">值</param>
        /// <param name="targetFiledName"></param>
        /// <param name="relation">条件 AND OR</param>
        /// <returns>条件语句</returns>
        public void SetWhere(string targetFiled, object targetValue, string targetFiledName = null, string relation = " AND ")
        {
            if (string.IsNullOrEmpty(targetFiledName))
            {
                targetFiledName = targetFiled;
            }
            //whereParameters Troy Cui 12.06.2017
            //fix the issue - The variable name '%.*ls' has already been declared. Variable names must be unique within a query batch or stored procedure. 
            //IdWhere就是Id这个字段的WHERE语句中的参数名
            //NameWhere就是Name这个字段的WHERE语句中的参数名，Troy Cui 2019.07.02补充说明
            targetFiledName += "Where";

            if (_whereSql.Length == 0)
            {
                _whereSql = Pool.StringBuilder.Get();
                _whereSql.Append(" WHERE ");
            }
            else
            {
                _whereSql.Append(relation);
            }
            if (targetValue is Array)
            {
                // this.WhereSql.Append(targetFiled + " IN (" + string.Join(",", targetValue) + ")");
                _whereSql.Append(targetFiled + " IN (" + ObjectUtil.ToList((object[])targetValue, "'") + ")");
                return;
            }
            // 这里需要对 null 进行处理
            if ((targetValue == null) || ((targetValue is string) && string.IsNullOrEmpty((string)targetValue)))
            {
                _whereSql.Append(targetFiled + " IS NULL ");
            }
            else
            {
                _whereSql.Append(targetFiled + " = " + DbHelper.GetParameter(_dbType, targetFiledName));
                AddParameter(targetFiledName, targetValue);
            }
            // return this.WhereSql;
        }
        #endregion

        #region public string SetOrderBy(string orderBy) 设置排序顺序
        /// <summary>
        /// 设置排序顺序
        /// </summary>
        /// <param name="orderBy">排序顺序</param>
        /// <returns>排序</returns>
        public string SetOrderBy(string orderBy)
        {
            if (string.IsNullOrEmpty(_orderBy))
            {
                _orderBy = " ORDER BY ";
            }
            _orderBy += orderBy;
            return _orderBy;
        }
        #endregion

        /// <summary>
        /// 数据库中的随机排序功能实现
        /// </summary>
        /// <returns>随机排序函数</returns>
        public string SetOrderByRandom()
        {
            if (string.IsNullOrEmpty(_orderBy))
            {
                _orderBy = " ORDER BY ";
            }
            switch (_dbHelper.CurrentDbType)
            {
                case CurrentDbType.Oracle:
                    _orderBy += "DBMS_RANDOM.VALUE()";
                    break;
                case CurrentDbType.SqlServer:
                case CurrentDbType.Access:
                    _orderBy += "NEWID()";
                    break;
                case CurrentDbType.MySql:
                    _orderBy += "Rand()";
                    break;
            }
            return _orderBy;
        }

        #region public int EndSelect() 结束查询
        /// <summary>
        /// 结束查询
        /// </summary>
        /// <returns>影响行数</returns>
        public DataTable EndSelect()
        {
            var dt = new DataTable(_tableName);
            if (_topN != null)
            {
                switch (_dbHelper.CurrentDbType)
                {
                    case CurrentDbType.Oracle:
                        // 这里还需要把条件进行优化
                        CommandText = "SELECT * FROM " + _tableName + " WHERE ROWNUM <= " + _topN + _orderBy;
                        break;
                    case CurrentDbType.SqlServer:
                    case CurrentDbType.Access:
                        CommandText = "SELECT TOP " + _topN + " * FROM " + _tableName + _whereSql.Put() + _orderBy;
                        break;
                    case CurrentDbType.MySql:
                        CommandText = "SELECT * FROM " + _tableName + _whereSql.Put() + _orderBy + " LIMIT 1 , " + _topN;
                        break;
                }
            }
            else
            {
                CommandText = "SELECT * FROM " + _tableName + _whereSql.Put() + _orderBy;
            }

            // 参数进行规范化
            var dbParameters = new List<IDbDataParameter>();
            foreach (var parameter in DbParameters)
            {
                dbParameters.Add(_dbHelper.MakeParameter(parameter.Key, parameter.Value));
            }
            _dbHelper.Fill(dt, CommandText, dbParameters.ToArray());
            // 清除查询参数
            DbParameters.Clear();
            return dt;
        }
        #endregion

        #region public int EndInsert() 结束插入
        /// <summary>
        /// 结束插入
        /// </summary>
        /// <returns>影响行数</returns>
        public int EndInsert()
        {
            return Execute();
        }
        #endregion

        #region public int EndReplace() 结束插入
        /// <summary>
        /// 结束插入
        /// </summary>
        /// <returns>影响行数</returns>
        public int EndReplace()
        {
            return Execute();
        }
        #endregion

        #region public int EndUpdate() 结束更新
        /// <summary>
        /// 结束更新
        /// </summary>
        /// <returns>影响行数</returns>
        public int EndUpdate()
        {
            return Execute();
        }
        #endregion

        #region public int EndDelete() 结束删除
        /// <summary>
        /// 结束删除
        /// </summary>
        /// <returns>影响行数</returns>
        public int EndDelete()
        {
            return Execute();
        }
        #endregion

        /// <summary>
        /// 准备生成sql语句
        /// </summary>
        public string PrepareCommand()
        {
            if (_sqlOperation == DbOperation.Insert || _sqlOperation == DbOperation.ReplaceInto)
            {
                var sbField = Pool.StringBuilder.Get();
                sbField.Append(_insertField.ToString().Substring(0, _insertField.Length - 2));
                //归还
                _insertField.Put(false);
                var sbValue = Pool.StringBuilder.Get();
                sbValue.Append(_insertValue.ToString().Substring(0, _insertValue.Length - 2));
                //归还
                _insertValue.Put(false);
                if (_sqlOperation == DbOperation.ReplaceInto)
                {
                    CommandText = "REPLACE INTO " + _tableName + "(" + sbField.Put() + ") VALUES(" + sbValue.Put() + ") ";
                }
                else
                {
                    CommandText = "INSERT INTO " + _tableName + "(" + sbField.Put() + ") VALUES(" + sbValue.Put() + ") ";
                }
                // 采用了自增量的方式
                if (Identity)
                {
                    switch (_dbType)
                    {
                        case CurrentDbType.SqlServer:
                            // 需要返回主键
                            if (ReturnId)
                            {
                                CommandText += "; SELECT SCOPE_IDENTITY(); ";
                            }
                            break;
                        case CurrentDbType.Access:
                            // 需要返回主键
                            if (ReturnId)
                            {
                                CommandText += "; SELECT @@identity AS ID FROM " + _tableName + "; ";
                            }
                            break;
                        // Mysql 返回自增主键 胡流东
                        case CurrentDbType.MySql:
                            if (ReturnId)
                            {
                                CommandText += "; SELECT LAST_INSERT_ID(); ";
                            }
                            break;
                    }
                }
            }
            else if (_sqlOperation == DbOperation.Update)
            {
                var sbUpdate = Pool.StringBuilder.Get();
                sbUpdate.Append(_updateSql.ToString().Substring(0, _updateSql.Length - 2));
                _updateSql.Put(false);
                CommandText = "UPDATE " + _tableName + " SET " + sbUpdate.Put() + _whereSql.Put();
            }
            else if (_sqlOperation == DbOperation.Delete)
            {
                CommandText = "DELETE FROM " + _tableName + _whereSql.Put();
            }
            else if (_sqlOperation == DbOperation.Select)
            {
                CommandText = "SELECT * FROM " + _tableName + _whereSql.Put();
            }

            return CommandText;
        }

        #region private int Execute() 执行语句
        /// <summary>
        /// 执行语句
        /// </summary>
        /// <returns>影响行数</returns>
        private int Execute()
        {
            // 处理返回值
            var result = 0;

            // 准备生成sql语句
            PrepareCommand();

            // 参数进行规范化
            var dbParameters = new List<IDbDataParameter>();
            foreach (var parameter in DbParameters)
            {
                dbParameters.Add(_dbHelper.MakeParameter(parameter.Key, parameter.Value));
            }

            if (Identity && _sqlOperation == DbOperation.Insert && (_dbHelper.CurrentDbType == CurrentDbType.SqlServer || _dbHelper.CurrentDbType == CurrentDbType.MySql))
            {
                // 读取返回值
                if (ReturnId)
                {
                    result = int.Parse(_dbHelper.ExecuteScalar(CommandText, dbParameters.ToArray()).ToString());
                }
                else
                {
                    // 执行语句
                    result = _dbHelper.ExecuteNonQuery(CommandText, dbParameters.ToArray());
                }
            }
            else
            {
                // 执行语句
                result = _dbHelper.ExecuteNonQuery(CommandText, dbParameters.ToArray());
            }

            if (!_dbHelper.MustCloseConnection)
            {

            }
            // 清除查询参数
            DbParameters.Clear();

            //写入日志
            //没必要在这里写了，ExecuteNonQuery写日志就行了，Troy.Cui 2018-06-29
            //SqlTrace.WriteLog(this.CommandText, dbParameters.ToArray());

            return result;
        }
        #endregion
    }
}