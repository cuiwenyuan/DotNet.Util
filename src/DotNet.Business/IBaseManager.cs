//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using DotNet.Util;

namespace DotNet.Business
{
    using Model;

    /// <summary>
    ///	IBaseManager
    /// 通用接口部分
    /// 
    /// 总觉得自己写的程序不上档次，这些新技术也玩玩，也许做出来的东西更专业了。
    /// 修改记录
    /// 
    ///		2007.11.01 版本：1.9 JiRiGaLa 改进 BUOperatorInfo 去掉这个变量，可以提高性能，提高速度，基类的又一次飞跃。
    ///		2007.05.23 版本：1.8 JiRiGaLa 修改完善了 对象事件触发器，完善了GetDataTable, ref 方法部分。
    ///		2007.05.20 版本：1.7 JiRiGaLa 修改完善了 对象事件触发器，完善了GetDataTable方法部分。
    ///		2007.05.19 版本：1.6 JiRiGaLa 修改完善了 Delete，Exists方法部分，累了休息一下下，争取周六周日两天内完成。
    ///		2007.05.18 版本：1.5 JiRiGaLa 规范了一些接口的标准方法，进行了补充。
    ///		2007.05.17 版本：1.4 JiRiGaLa 重新调整主键的规范化，整体上又上升了一个层次了。
    ///		2006.02.05 版本：1.3 JiRiGaLa 重新调整主键的规范化。
    ///		2005.08.19 版本：1.2 JiRiGaLa 参数进行改进
    ///		2004.07.23 版本：1.1 JiRiGaLa 增加了接口ClearProperty、GetFromDS 的定义。
    ///		2004.07.21 版本：1.0 JiRiGaLa 提炼了最基础的方法部分、觉得这些是很有必要的方法。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2007.05.23</date>
    /// </author> 
    /// </summary>
    public partial interface IBaseManager
    {
        /// <summary>
        /// 当前表名
        /// </summary>
        string CurrentTableName { get; set; }
        /// <summary>
        /// 当前表描述
        /// </summary>
        string CurrentTableDescription { get; set; }

        #region 增删改

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        string AddEntity(object entity);
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int UpdateEntity(object entity);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int DeleteEntity(object id);

        #endregion

        #region 对象事件触发器（编写程序的人员，可以不实现这些方法）

        /// <summary>
        /// 添加前
        /// </summary>
        /// <returns></returns>
        bool AddBefore();
        /// <summary>
        /// 添加后
        /// </summary>
        /// <returns></returns>
        bool AddAfter();
        /// <summary>
        /// 更新前
        /// </summary>
        /// <returns></returns>
        bool UpdateBefore();
        /// <summary>
        /// 更新后
        /// </summary>
        /// <returns></returns>
        bool UpdateAfter();
        /// <summary>
        /// 获取前
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool GetBefore(string id);
        /// <summary>
        /// 获取后
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool GetAfter(string id);
        /// <summary>
        /// 删除前
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteBefore(string id);
        /// <summary>
        /// 删除后
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteAfter(string id);

        #endregion

        #region BatchSave
        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        int BatchSave(DataTable dt);

        #endregion

        #region GetDataTable获取DataTable

        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <returns></returns>
        DataTable GetDataTable(string condition);
        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="topLimit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        DataTable GetDataTable(int topLimit, string order);
        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        DataTable GetDataTable(string name, Object[] values, string order);
        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        DataTable GetDataTable(string[] ids);
        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        DataTable GetDataTable(KeyValuePair<string, object> parameter, string order);
        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="topLimit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        DataTable GetDataTable(KeyValuePair<string, object> parameter, int topLimit, string order);
        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        DataTable GetDataTable(List<KeyValuePair<string, object>> parameters, string order);
        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="topLimit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        DataTable GetDataTable(List<KeyValuePair<string, object>> parameters, int topLimit, string order);

        #endregion

        #region GetList获取List

        /// <summary>
        /// 获取List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        List<T> GetListById<T>(string id) where T : BaseEntity, new();
        /// <summary>
        /// 获取List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <param name="topLimit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        List<T> GetList2<T>(string condition, int topLimit = 0, string order = null) where T : BaseEntity, new();
        /// <summary>
        /// 获取List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="topLimit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        List<T> GetList<T>(int topLimit = 0, string order = null) where T : BaseEntity, new();
        /// <summary>
        /// 获取List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <returns></returns>
        List<T> GetList<T>(string condition) where T : BaseEntity, new();
        /// <summary>
        /// 获取List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        List<T> GetList<T>(string[] ids) where T : BaseEntity, new();
        /// <summary>
        /// 获取List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        List<T> GetList<T>(string name, Object[] values, string order = null) where T : BaseEntity, new();
        /// <summary>
        /// 获取List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        List<T> GetList<T>(KeyValuePair<string, object> parameter, string order) where T : BaseEntity, new();
        /// <summary>
        /// 获取List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        List<T> GetList<T>(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2, string order) where T : BaseEntity, new();
        /// <summary>
        /// 获取List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        /// <param name="topLimit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        List<T> GetList<T>(KeyValuePair<string, object> parameter, int topLimit, string order) where T : BaseEntity, new();
        /// <summary>
        /// 获取List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        List<T> GetList<T>(List<KeyValuePair<string, object>> parameters, string order) where T : BaseEntity, new();
        /// <summary>
        /// 获取List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="topLimit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        List<T> GetList<T>(List<KeyValuePair<string, object>> parameters, int topLimit, string order) where T : BaseEntity, new();
        /// <summary>
        /// 获取List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        List<T> GetList<T>(params KeyValuePair<string, object>[] parameters) where T : BaseEntity, new();

        #endregion

        #region GetProperty读取属性

        /// <summary>
        /// 读取属性
        /// </summary>
        /// <param name="id"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        string GetProperty(object id, string targetField);
        /// <summary>
        /// 读取属性
        /// </summary>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        string GetProperty(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2, string targetField);
        /// <summary>
        /// 读取属性
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="targetField"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        string GetProperty(List<KeyValuePair<string, object>> parameters, string targetField, string orderBy);

        #endregion

        #region GetId获取Id

        /// <summary>
        /// 获取Id
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        string GetId(KeyValuePair<string, object> parameter);
        /// <summary>
        /// 获取Id
        /// </summary>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <returns></returns>
        string GetId(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2);
        /// <summary>
        /// 获取Id
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        string GetId(List<KeyValuePair<string, object>> parameters);
        /// <summary>
        /// 获取Id
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        string GetId(params KeyValuePair<string, object>[] parameters);

        #endregion

        #region GetIds获取Id数组
        /// <summary>
        /// 获取Id数组
        /// </summary>
        /// <returns></returns>
        string[] GetIds();
        /// <summary>
        /// 获取Id数组
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        string[] GetIds(KeyValuePair<string, object> parameter);
        /// <summary>
        /// 获取Id数组
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        string[] GetIds(string name, Object[] values);
        /// <summary>
        /// 获取Id数组
        /// </summary>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <returns></returns>
        string[] GetIds(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2);
        /// <summary>
        /// 获取Id数组
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        string[] GetIds(List<KeyValuePair<string, object>> parameters);
        #endregion

        #region GetProperties获取属性数组

        /// <summary>
        /// 获取属性数组
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        string[] GetProperties(string order);
        /// <summary>
        /// 获取属性数组
        /// </summary>
        /// <param name="topLimit"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        string[] GetProperties(int topLimit, string targetField);
        /// <summary>
        /// 获取属性数组
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        string[] GetProperties(KeyValuePair<string, object> parameter, string targetField);
        /// <summary>
        /// 获取属性数组
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="topLimit"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        string[] GetProperties(KeyValuePair<string, object> parameter, int topLimit, string targetField);
        /// <summary>
        /// 获取属性数组
        /// </summary>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        string[] GetProperties(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2, string targetField);
        /// <summary>
        /// 获取属性数组
        /// </summary>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="topLimit"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        string[] GetProperties(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2, int topLimit, string targetField);
        /// <summary>
        /// 获取属性数组
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        string[] GetProperties(List<KeyValuePair<string, object>> parameters, string targetField);
        /// <summary>
        /// 获取属性数组
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="topLimit"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        string[] GetProperties(List<KeyValuePair<string, object>> parameters, int topLimit, string targetField);

        #endregion

        #region SetProperty设置属性

        /// <summary>
        /// 设置更新属性
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        int SetProperty(KeyValuePair<string, object> parameter);
        /// <summary>
        /// 设置更新属性
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        int SetProperty(object id, KeyValuePair<string, object> parameter);
        /// <summary>
        /// 设置更新属性
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int SetProperty(object id, List<KeyValuePair<string, object>> parameters);
        /// <summary>
        /// 设置更新属性
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        int SetProperty(object[] ids, KeyValuePair<string, object> parameter);
        /// <summary>
        /// 设置更新属性
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int SetProperty(object[] ids, List<KeyValuePair<string, object>> parameters);
        /// <summary>
        /// 设置更新属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        int SetProperty(string name, object[] values, KeyValuePair<string, object> parameter);
        /// <summary>
        /// 设置更新属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int SetProperty(string name, object[] values, List<KeyValuePair<string, object>> parameters);
        /// <summary>
        /// 设置更新属性
        /// </summary>
        /// <param name="whereParameter1"></param>
        /// <param name="whereParameter2"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        int SetProperty(KeyValuePair<string, object> whereParameter1, KeyValuePair<string, object> whereParameter2, KeyValuePair<string, object> parameter);
        /// <summary>
        /// 设置更新属性
        /// </summary>
        /// <param name="whereParameter"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        int SetProperty(KeyValuePair<string, object> whereParameter, KeyValuePair<string, object> parameter);
        /// <summary>
        /// 设置更新属性
        /// </summary>
        /// <param name="whereParameters"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        int SetProperty(List<KeyValuePair<string, object>> whereParameters, KeyValuePair<string, object> parameter);
        /// <summary>
        /// 设置更新属性
        /// </summary>
        /// <param name="whereParameter"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int SetProperty(KeyValuePair<string, object> whereParameter, List<KeyValuePair<string, object>> parameters);
        /// <summary>
        /// 设置更新属性
        /// </summary>
        /// <param name="whereParameters"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int SetProperty(List<KeyValuePair<string, object>> whereParameters, List<KeyValuePair<string, object>> parameters);

        #endregion

        #region Exists是否存在

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Exists(object id);

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Exists(KeyValuePair<string, object> parameter, object id);
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="parameter1"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        bool Exists(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter);
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Exists(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2, object id);
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        bool Exists(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2, KeyValuePair<string, object> parameter);
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Exists(List<KeyValuePair<string, object>> parameters, object id);

        #endregion

        #region Delete删除数据部分

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        int Delete();
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int Delete(object id);
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        int Delete(object[] ids);
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        int Delete(string name, object[] values);
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int Delete(List<KeyValuePair<string, object>> parameters);

        #endregion

        #region Truncate
        /// <summary>
        /// Truncate
        /// </summary>
        /// <returns></returns>
        int Truncate();
        #endregion

        #region ExecuteNonQuery直接执行一些SQL语句的方法

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <returns>影响行数</returns>
        int ExecuteNonQuery(string commandText);

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>影响行数</returns>
        int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters);

        #endregion

        #region ExecuteScalar
        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <returns>object</returns>
        object ExecuteScalar(string commandText);

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>Object</returns>
        object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters);
        #endregion

        #region Fill
        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="commandText">查询</param>
        /// <returns>数据表</returns>
        DataTable Fill(string commandText);

        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>数据表</returns>
        DataTable Fill(string commandText, IDbDataParameter[] dbParameters);

        #endregion

        #region Extend

        /// <summary>
        /// 获取上一个下一个编号
        /// </summary>
        /// <param name="currentId">当前编号</param>
        /// <param name="tableName">表名</param>
        /// <param name="orderTypeId">订单类型编号</param> 
        /// <param name="previousId">上一个编号</param>
        /// <param name="nextId">下一个编号</param>
        /// <returns></returns>
        bool GetPreviousAndNextId(int currentId, string tableName, string orderTypeId, out int previousId, out int nextId);

        /// <summary>
        /// 获取最新的N条记录
        /// </summary>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <param name="tableName">表名</param>
        /// <param name="rows">几条记录</param>
        /// <param name="sortField">排序字段</param>
        /// <returns></returns>
        DataTable GetDataTableLatest(string condition, string tableName = null, int rows = 1, string sortField = BaseUtil.FieldId);

        /// <summary>
        /// 获取当前数据库大小
        /// </summary>
        /// <returns></returns>
        decimal GetDatabaseSize();

        /// <summary>
        /// 查询数据库表空间
        /// </summary>
        /// <param name="searchKey">查询关键字</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns>数据表</returns>
        /// <returns></returns>
        DataTable GetTableSpaceByPage(string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseUtil.FieldId, string sortDirection = "DESC");

        /// <summary>
        /// 获取所有记录总数
        /// </summary>
        /// <param name="condition">查询条件(不需要以AND开头)</param>
        /// <param name="days">最近多少天</param>
        /// <param name="currentWeek">当周</param>
        /// <param name="currentMonth">当月</param>
        /// <param name="currentQuarter">当季</param>
        /// <param name="currentYear">当年</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>总数</returns>
        int GetTotalCount(string condition = null, int days = 0, bool currentWeek = false, bool currentMonth = false, bool currentQuarter = false, bool currentYear = false, string startTime = null, string endTime = null);

        /// <summary>
        /// 获取所有记录（唯一值）总数
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="condition">查询条件(不需要以AND开头)</param>
        /// <param name="days">最近多少天</param>
        /// <param name="currentWeek">当周</param>
        /// <param name="currentMonth">当月</param>
        /// <param name="currentQuarter">当季</param>
        /// <param name="currentYear">当年</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>总数</returns>
        int GetTotalDistinctCount(string fieldName, string condition = null, int days = 0, bool currentWeek = false, bool currentMonth = false, bool currentQuarter = false, bool currentYear = false, string startTime = null, string endTime = null);

        /// <summary>
        /// 获取有效记录总数
        /// </summary>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <param name="days">最近多少天</param>
        /// <param name="currentWeek">当周</param>
        /// <param name="currentMonth">当月</param>
        /// <param name="currentQuarter">当季</param>
        /// <param name="currentYear">当年</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>总数</returns>
        int GetActiveTotalCount(string condition = null, int days = 0, bool currentWeek = false, bool currentMonth = false, bool currentQuarter = false, bool currentYear = false, string startTime = null, string endTime = null);

        /// <summary>
        /// 获取有效记录（唯一值）总数
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <param name="days">最近多少天</param>
        /// <param name="currentWeek">当周</param>
        /// <param name="currentMonth">当月</param>
        /// <param name="currentQuarter">当季</param>
        /// <param name="currentYear">当年</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>总数</returns>
        int GetActiveTotalDistinctCount(string fieldName, string condition = null, int days = 0, bool currentWeek = false, bool currentMonth = false, bool currentQuarter = false, bool currentYear = false, string startTime = null, string endTime = null);

        /// <summary>
        /// 是否唯一的
        /// </summary>
        /// <param name="fieldValue">数据</param>
        /// <param name="excludeId">排除行id</param>
        /// <param name="fieldName">数据字段</param>
        /// <param name="checkUserCompany">是否检查公司数据</param>
        /// <returns>是否</returns>
        bool IsUnique(string fieldValue, string excludeId, string fieldName, bool checkUserCompany = false);

        /// <summary>
        /// 是否唯一的(两个字段)
        /// </summary>
        /// <param name="field1Value">数据1</param>
        /// <param name="field2Value">数据1</param>
        /// <param name="excludeId">排除行id</param>
        /// <param name="field1Name">数据字段1</param>
        /// <param name="field2Name">数据字段2</param>
        /// <param name="checkUserCompany">是否检查公司数据</param>
        /// <returns>是否</returns>
        bool IsUnique(string field1Value, string field2Value, string excludeId, string field1Name, string field2Name, bool checkUserCompany = false);

        /// <summary>
        /// 是否唯一的(三个字段)
        /// </summary>
        /// <param name="field1Value">数据1</param>
        /// <param name="field2Value">数据2</param>
        /// <param name="field3Value">数据3</param>
        /// <param name="excludeId">排除行id</param>
        /// <param name="field1Name">数据字段1</param>
        /// <param name="field2Name">数据字段2</param>
        /// <param name="field3Name">数据字段3</param>
        /// <param name="checkUserCompany">是否检查公司数据</param>
        /// <returns>是否</returns>
        bool IsUnique(string field1Value, string field2Value, string field3Value, string excludeId, string field1Name, string field2Name, string field3Name, bool checkUserCompany = false);

        /// <summary>
        /// 是否被用过
        /// </summary>
        /// <param name="id">行id</param>
        /// <param name="tableName">要检查的关联表名</param>
        /// <param name="tableFieldName">要检查的关联表字段名</param>
        /// <param name="currentTableFieldName">当前表的关联字段名（默认为Id）</param>
        /// <param name="checkUserCompany">是否检查公司数据</param>
        /// <returns>是否</returns>
        bool IsUsed(string id, string tableName, string tableFieldName, string currentTableFieldName = BaseUtil.FieldId, bool checkUserCompany = false);

        /// <summary>
        /// 是否被用过（批量）
        /// </summary>
        /// <param name="ids">行Ids</param>
        /// <param name="tableName">要检查的关联表名</param>
        /// <param name="tableFieldName">要检查的关联表字段名</param>
        /// <param name="currentTableFieldName">当前表的关联字段名（默认为Id）</param>
        /// <param name="checkUserCompany">是否检查公司数据</param>
        /// <returns>是否</returns>
        bool IsUsed(string[] ids, string tableName, string tableFieldName, string currentTableFieldName = BaseUtil.FieldId, bool checkUserCompany = false);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <returns></returns>
        bool RemoveCache();

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        bool RemoveCache(int id);

        /// <summary>
        /// 保存实体修改日志
        /// </summary>
        /// <param name="recordKey">记录主键</param>
        /// <param name="entityNew">修改后的实体对象</param>
        /// <param name="entityOld">修改前的实体对象</param>
        /// <param name="tableName">表名称</param>
        /// <param name="systemCode">子系统编码</param>
        void SaveEntityChangeLog(string recordKey, object entityOld, object entityNew, string tableName = null, string systemCode = null);

        #endregion

        #region public virtual void SetEntityCreate<T>(SqlBuilder sqlBuilder, T t) 设置创建信息
        /// <summary>
        /// 设置创建信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlBuilder"></param>
        /// <param name="t"></param>
        public void SetEntityCreate<T>(SqlBuilder sqlBuilder, T t);
        #endregion

        #region public virtual void SetEntityUpdate<T>(SqlBuilder sqlBuilder, T t) 设置更新信息
        /// <summary>
        /// 设置更新信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlBuilder"></param>
        /// <param name="t"></param>
        public void SetEntityUpdate<T>(SqlBuilder sqlBuilder, T t);
        #endregion
    }
}