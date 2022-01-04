//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Util;

    /// <summary>
    ///	BaseManager
    /// 通用基类部分
    /// 
    /// 总觉得自己写的程序不上档次，这些新技术也玩玩，也许做出来的东西更专业了。
    /// 修改记录
    /// 
    ///     2018.05.27 版本：2.0 Troy.Cui 新增2个GetDataTable，同时传递where和参数。
    ///		2012.02.04 版本：1.0 JiRiGaLa 进行提炼，把代码进行分组。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.02.04</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        //
        // 获取一些列表的常用方法
        //
        /// <summary>
        /// GetDataTable
        /// </summary>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <param name="parameters">参数</param>
        /// <param name="order">排序(不包含ORDER BY)</param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(string condition, List<KeyValuePair<string, object>> parameters, string order)
        {
            var subSql = DbUtil.GetWhereString(dbHelper, parameters, BaseUtil.SqlLogicConditional);
            if (!string.IsNullOrEmpty(subSql))
            {
                if (!string.IsNullOrWhiteSpace(condition))
                {
                    condition = condition + BaseUtil.SqlLogicConditional + subSql;
                }
                else
                {
                    condition = subSql;
                }
            }

            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT * FROM " + CurrentTableName);
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" WHERE " + condition);
            }
            if (!string.IsNullOrWhiteSpace(order))
            {
                sb.Append(" ORDER BY " + order);
            }

            return DbHelper.Fill(sb.Put(), dbHelper.MakeParameters(parameters));
        }
        /// <summary>
        /// GetDataTable
        /// </summary>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <param name="order">排序(不包含ORDER BY)</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(string condition, string order, params KeyValuePair<string, object>[] parameters)
        {
            var parametersList = new List<KeyValuePair<string, object>>();
            foreach (var t in parameters)
            {
                parametersList.Add(t);
            }

            return GetDataTable(condition, parametersList, order);

        }
        /// <summary>
        /// GetDataTableByWhere
        /// </summary>
        /// <param name="condition">查询条件(不包含WHERE)</param>
        /// <returns></returns>
        public virtual DataTable GetDataTableByWhere(string condition = null)
        {
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT * FROM " + CurrentTableName);
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" WHERE " + condition);
            }
            return DbHelper.Fill(sb.Put());
        }
        /// <summary>
        /// GetDataTableById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual DataTable GetDataTableById(string id)
        {
            return GetDataTable(new KeyValuePair<string, object>(BaseUtil.FieldId, id));
        }

        #region public virtual DataTable GetDataTableByCategory(string categoryId) 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="categoryCode">类别主键</param>
        /// <returns>数据表</returns>
        public virtual DataTable GetDataTableByCategory(string categoryCode)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldCategoryCode, categoryCode)
            };
            return DbUtil.GetDataTable(DbHelper, CurrentTableName, parameters);
        }
        #endregion

        #region public virtual DataTable GetDataTableByParent(string parentId) 按父亲节点获取数据
        /// <summary>
        /// 按父亲节点获取数据
        /// </summary>
        /// <param name="parentId">父节点主键</param>
        /// <returns>数据表</returns>
        public virtual DataTable GetDataTableByParent(string parentId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldParentId, parentId)
            };
            return DbUtil.GetDataTable(DbHelper, CurrentTableName, parameters, 0, BaseUtil.FieldSortCode);
        }
        #endregion

        //
        // 读取多条记录
        //
        /// <summary>
        /// GetDataTable
        /// </summary>
        /// <param name="topLimit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(int topLimit = 0, string order = null)
        {
            return DbUtil.GetDataTable(DbHelper, CurrentTableName, null, topLimit, order);
        }
        /// <summary>
        /// GetDataTable
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(string condition)
        {
            return GetDataTableByWhere(condition);
        }
        /// <summary>
        /// GetDataTable
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(string[] ids)
        {
            return DbUtil.GetDataTable(DbHelper, CurrentTableName, BaseUtil.FieldId, ids);
        }
        /// <summary>
        /// GetDataTable
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(string name, Object[] values, string order = null)
        {
            return DbUtil.GetDataTable(DbHelper, CurrentTableName, name, values, order);
        }
        /// <summary>
        /// GetDataTable
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(KeyValuePair<string, object> parameter, string order)
        {
            var parameters = new List<KeyValuePair<string, object>> { parameter };
            return DbUtil.GetDataTable(DbHelper, CurrentTableName, parameters, 0, order);
        }
        /// <summary>
        /// GetDataTable
        /// </summary>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2, string order)
        {
            var parameters = new List<KeyValuePair<string, object>> { parameter1, parameter2 };
            return DbUtil.GetDataTable(DbHelper, CurrentTableName, parameters, 0, order);
        }
        /// <summary>
        /// GetDataTable
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="topLimit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(KeyValuePair<string, object> parameter, int topLimit = 0, string order = null)
        {
            var parameters = new List<KeyValuePair<string, object>> { parameter };
            return DbUtil.GetDataTable(DbHelper, CurrentTableName, parameters, topLimit, order);
        }
        /// <summary>
        /// GetDataTable
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(params KeyValuePair<string, object>[] parameters)
        {
            var parametersList = new List<KeyValuePair<string, object>>();
            foreach (var t in parameters)
            {
                parametersList.Add(t);
            }
            return DbUtil.GetDataTable(DbHelper, CurrentTableName, parametersList);
        }
        /// <summary>
        /// GetDataTable
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="topLimit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(List<KeyValuePair<string, object>> parameters, int topLimit = 0, string order = null)
        {
            return DbUtil.GetDataTable(DbHelper, CurrentTableName, parameters, topLimit, order);
        }
        /// <summary>
        /// GetDataTable
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(List<KeyValuePair<string, object>> parameters, string order)
        {
            return DbUtil.GetDataTable(DbHelper, CurrentTableName, parameters, 0, order);
        }
    }
}