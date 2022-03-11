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
    ///		2015.06.16 版本：1.0 JiRiGaLa 进行提炼，把代码进行分组。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.06.16</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region 获取一些列表的常用方法

        /// <summary>
        /// ExecuteReaderByWhere
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="parameters">条件参数</param>
        /// <param name="topLimit">TOP记录数</param>
        /// <param name="order">排序信息</param>
        /// <returns></returns>
        public virtual IDataReader ExecuteReaderByWhere(string condition = null, List<KeyValuePair<string, object>> parameters = null, int topLimit = 0, string order = null)
        {
            return DbUtil.ExecuteReader(DbHelper, CurrentTableName, parameters, condition, topLimit, order, "*");
        }

        /// <summary>
        /// ExecuteReaderById
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public virtual IDataReader ExecuteReaderById(string id)
        {
            return ExecuteReader(new KeyValuePair<string, object>(BaseUtil.FieldId, id));
        }

        #region public virtual IDataReader ExecuteReaderByCategory(string categoryId) 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="categoryCode">类别主键</param>
        /// <returns>数据表</returns>
        public virtual IDataReader ExecuteReaderByCategory(string categoryCode)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldCategoryCode, categoryCode)
            };
            return DbUtil.ExecuteReader(DbHelper, CurrentTableName, parameters);
        }
        #endregion

        #region public virtual IDataReader ExecuteReaderByParent(string parentId) 按父亲节点获取数据
        /// <summary>
        /// 按父亲节点获取数据
        /// </summary>
        /// <param name="parentId">父节点主键</param>
        /// <returns>数据表</returns>
        public virtual IDataReader ExecuteReaderByParent(string parentId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldParentId, parentId)
            };
            return DbUtil.ExecuteReader(DbHelper, CurrentTableName, parameters, 0, BaseUtil.FieldSortCode);
        }
        #endregion

        #endregion

        #region 读取多条记录
        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <param name="topLimit">TOP记录数</param>
        /// <param name="order">排序信息</param>
        /// <returns></returns>
        public virtual IDataReader ExecuteReader(int topLimit = 0, string order = null)
        {
            return DbUtil.ExecuteReader(DbHelper, CurrentTableName, null, string.Empty, topLimit, order, SelectFields);
        }

        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <param name="commandText">SQL命令字符串</param>
        /// <returns></returns>
        public virtual IDataReader ExecuteReader(string commandText)
        {
            return DbHelper.ExecuteReader(commandText);
        }

        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <param name="ids">编号</param>
        /// <returns></returns>
        public virtual IDataReader ExecuteReader(string[] ids)
        {
            return DbUtil.ExecuteReader(DbHelper, CurrentTableName, BaseUtil.FieldId, ids);
        }

        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="values">字段值</param>
        /// <param name="order">排序信息</param>
        /// <returns></returns>
        public virtual IDataReader ExecuteReader(string name, Object[] values, string order = null)
        {
            return DbUtil.ExecuteReader(DbHelper, CurrentTableName, name, values, order);
        }

        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <param name="parameter">条件参数</param>
        /// <param name="order">排序信息</param>
        /// <returns></returns>
        public virtual IDataReader ExecuteReader(KeyValuePair<string, object> parameter, string order)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter
            };
            return DbUtil.ExecuteReader(DbHelper, CurrentTableName, parameters, 0, order);
        }

        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <param name="parameter1">条件参数1</param>
        /// <param name="parameter2">条件参数2</param>
        /// <param name="order">排序信息</param>
        /// <returns></returns>
        public virtual IDataReader ExecuteReader(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2, string order)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter1,
                parameter2
            };
            return DbUtil.ExecuteReader(DbHelper, CurrentTableName, parameters, 0, order);
        }

        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <param name="parameter">条件参数</param>
        /// <param name="topLimit">TOP记录数</param>
        /// <param name="order">排序信息</param>
        /// <returns></returns>
        public virtual IDataReader ExecuteReader(KeyValuePair<string, object> parameter, int topLimit = 0, string order = null)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter
            };
            return DbUtil.ExecuteReader(DbHelper, CurrentTableName, parameters, topLimit: topLimit, order: order);
        }

        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <param name="parameters">条件参数</param>
        /// <returns></returns>
        public virtual IDataReader ExecuteReader(params KeyValuePair<string, object>[] parameters)
        {
            var parametersList = new List<KeyValuePair<string, object>>();
            for (var i = 0; i < parameters.Length; i++)
            {
                parametersList.Add(parameters[i]);
            }
            return DbUtil.ExecuteReader(DbHelper, CurrentTableName, parametersList);
        }

        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <param name="parameters">条件参数</param>
        /// <param name="topLimit">TOP记录数</param>
        /// <param name="order">排序信息</param>
        /// <returns></returns>
        public virtual IDataReader ExecuteReader(List<KeyValuePair<string, object>> parameters, int topLimit = 0, string order = null)
        {
            return DbUtil.ExecuteReader(DbHelper, CurrentTableName, parameters, topLimit, order);
        }

        #endregion
    }
}