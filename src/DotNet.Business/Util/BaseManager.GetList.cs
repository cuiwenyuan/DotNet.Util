//--------------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//--------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseManager
    /// 
    /// 
    /// 修改记录
    /// 
    ///
    /// <author>
    ///		<name>SongBiao</name>
    ///		<date>2015.12.10</date>
    /// </author>
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region public virtual List<T> GetListById<T>(string id) where T : BaseEntity, new()

        /// <summary>
        /// 根据ID获取集合对象
        /// </summary>
        /// <typeparam name="T">泛型实体</typeparam>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public virtual List<T> GetListById<T>(string id) where T : BaseEntity, new()
        {
            return GetList<T>(new KeyValuePair<string, object>(BaseUtil.FieldId, id));
        }

        #endregion

        #region public virtual List<T> GetList<T>(int topLimit = 0, string order = null) where T : BaseEntity, new()

        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T">泛型实体</typeparam>
        /// <param name="topLimit">记录数</param>
        /// <param name="order">包含字段和方向ASC/DESC</param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(int topLimit = 0, string order = null) where T : BaseEntity, new()
        {
            List<T> result;

            var dr = DbHelper.ExecuteReader(CurrentTableName, null, topLimit, order);
            result = GetList<T>(dr);

            return result;
        }

        #endregion

        #region public virtual List<T> GetList<T>(string[] ids) where T : BaseEntity, new()

        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T">泛型实体</typeparam>
        /// <param name="ids">编号数组</param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(string[] ids) where T : BaseEntity, new()
        {
            List<T> result;

            var dr = DbHelper.ExecuteReader(CurrentTableName, "*", BaseUtil.FieldId, ids);
            result = GetList<T>(dr);

            return result;
        }

        #endregion

        #region public virtual List<T> GetList<T>(string name, Object[] values, string order = null) where T : BaseEntity, new()
        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T">泛型实体</typeparam>
        /// <param name="name">名称</param>
        /// <param name="values">值</param>
        /// <param name="order">包含字段和方向ASC/DESC</param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(string name, Object[] values, string order = null) where T : BaseEntity, new()
        {
            List<T> result;

            var dr = DbHelper.ExecuteReader(CurrentTableName, "*", name, values, order);
            result = GetList<T>(dr);

            return result;
        }

        #endregion

        #region public virtual List<T> GetList<T>(KeyValuePair<string, object> parameter, string order) where T : BaseEntity, new()

        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T">泛型实体</typeparam>
        /// <param name="parameter">查询参数</param>
        /// <param name="order">包含字段和方向ASC/DESC</param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(KeyValuePair<string, object> parameter, string order) where T : BaseEntity, new()
        {
            List<T> result;

            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter
            };

            var dr = DbHelper.ExecuteReader(CurrentTableName, parameters, 0, order);
            result = GetList<T>(dr);

            return result;
        }

        #endregion

        #region public virtual List<T> GetList<T>(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2, string order) where T : BaseEntity, new() 
        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T">泛型实体</typeparam>
        /// <param name="parameter1">查询参数1</param>
        /// <param name="parameter2">查询参数2</param>
        /// <param name="order">包含字段和方向ASC/DESC</param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2, string order) where T : BaseEntity, new()
        {
            List<T> result;

            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter1,
                parameter2
            };

            var dr = DbHelper.ExecuteReader(CurrentTableName, parameters, 0, order);
            result = GetList<T>(dr);

            return result;
        }

        #endregion

        #region public virtual List<T> GetList<T>(KeyValuePair<string, object> parameter, int topLimit = 0, string order = null) where T : BaseEntity, new()

        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T">泛型实体</typeparam>
        /// <param name="parameter">查询参数</param>
        /// <param name="topLimit">记录数</param>
        /// <param name="order">包含字段和方向ASC/DESC</param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(KeyValuePair<string, object> parameter, int topLimit = 0, string order = null) where T : BaseEntity, new()
        {
            List<T> result;

            var parameters = new List<KeyValuePair<string, object>> { parameter };

            var dr = DbHelper.ExecuteReader(CurrentTableName, parameters, topLimit, order);
            result = GetList<T>(dr);

            return result;
        }

        #endregion

        #region public virtual List<T> GetList<T>(List<KeyValuePair<string, object>> parameters, string order) where T : BaseEntity, new()
        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T">泛型实体</typeparam>
        /// <param name="parameters">查询参数</param>
        /// <param name="order">包含字段和方向ASC/DESC</param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(List<KeyValuePair<string, object>> parameters, string order) where T : BaseEntity, new()
        {
            List<T> result;

            var dr = DbHelper.ExecuteReader(CurrentTableName, parameters, 0, order);
            result = GetList<T>(dr);

            return result;
        }

        #endregion

        #region public virtual List<T> GetList<T>(List<KeyValuePair<string, object>> parameters, int topLimit = 0, string order = null) where T : BaseEntity, new()
        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T">泛型实体</typeparam>
        /// <param name="parameters">查询参数</param>
        /// <param name="topLimit">记录数</param>
        /// <param name="order">包含字段和方向ASC/DESC</param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(List<KeyValuePair<string, object>> parameters, int topLimit = 0, string order = null) where T : BaseEntity, new()
        {
            List<T> result;

            var dr = DbHelper.ExecuteReader(CurrentTableName, parameters, topLimit, order);
            result = GetList<T>(dr);

            return result;
        }

        #endregion

        #region public virtual List<T> GetList<T>(params KeyValuePair<string, object>[] parameters) where T : BaseEntity, new()
        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T">泛型实体</typeparam>
        /// <param name="parameters">查询参数</param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(params KeyValuePair<string, object>[] parameters) where T : BaseEntity, new()
        {
            var result = new List<T>();

            var parametersList = new List<KeyValuePair<string, object>>();
            foreach (var p in parameters)
            {
                parametersList.Add(p);
            }

            var dr = DbHelper.ExecuteReader(CurrentTableName, parametersList);
            result = GetList<T>(dr);

            return result;
        }
        #endregion

        #region public virtual List<T> GetList<T>(string condition) where T : BaseEntity, new()
        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T">泛型实体</typeparam>
        /// <param name="condition">不须传递WHERE</param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(string condition) where T : BaseEntity, new()
        {
            List<T> result;

            var dr = DbHelper.ExecuteReader(CurrentTableName, condition);
            result = GetList<T>(dr);

            return result;
        }

        #endregion

        #region public virtual List<T> GetList<T>(string condition) where T : BaseEntity, new()
        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T">泛型实体</typeparam>
        /// <param name="condition">查询条件</param>
        /// <param name="topLimit">记录数</param>
        /// <param name="order">包含字段和方向ASC/DESC</param>
        /// <returns></returns>
        public virtual List<T> GetList2<T>(string condition, int topLimit = 0, string order = null) where T : BaseEntity, new()
        {
            List<T> result;
            var dr = DbHelper.ExecuteReader2(CurrentTableName, condition, topLimit, order);
            result = GetList<T>(dr);

            return result;
        }

        #endregion

        #region public virtual List<T> GetList<T>(IDataReader dataReader) where T : BaseEntity, new()
        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T">泛型实体</typeparam>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(IDataReader dataReader) where T : BaseEntity, new()
        {
            // 还能继承 IBaseEntity<T>
            var result = new List<T>();
            if (dataReader != null && !dataReader.IsClosed)
            {
                while (dataReader.Read())
                {
                    result.Add(BaseEntity.Create<T>(dataReader, false));
                }
                dataReader.Close();
            }

            return result;
        }

        #endregion
    }
}
