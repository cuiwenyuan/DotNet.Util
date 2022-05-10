//--------------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
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

            var dr = DbUtil.ExecuteReader(DbHelper, CurrentTableName, null, topLimit, order);
            result = GetList<T>(dr);

            return result;
        }

        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T">泛型实体</typeparam>
        /// <param name="ids">编号数组</param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(string[] ids) where T : BaseEntity, new()
        {
            List<T> result;

            var dr = DbUtil.ExecuteReader(DbHelper, CurrentTableName, "*", BaseUtil.FieldId, ids);
            result = GetList<T>(dr);

            return result;
        }

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

            var dr = DbUtil.ExecuteReader(DbHelper, CurrentTableName, "*", name, values, order);
            result = GetList<T>(dr);

            return result;
        }

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

            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(parameter);

            var dr = DbUtil.ExecuteReader(DbHelper, CurrentTableName, parameters, 0, order);
            result = GetList<T>(dr);

            return result;
        }

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

            var dr = DbUtil.ExecuteReader(DbHelper, CurrentTableName, parameters, 0, order);
            result = GetList<T>(dr);

            return result;
        }

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

            var dr = DbUtil.ExecuteReader(DbHelper, CurrentTableName, parameters, topLimit, order);
            result = GetList<T>(dr);

            return result;
        }

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

            var dr = DbUtil.ExecuteReader(DbHelper, CurrentTableName, parameters, 0, order);
            result = GetList<T>(dr);

            return result;
        }

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

            var dr = DbUtil.ExecuteReader(DbHelper, CurrentTableName, parameters, topLimit, order);
            result = GetList<T>(dr);

            return result;
        }

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

            var dr = DbUtil.ExecuteReader(DbHelper, CurrentTableName, parametersList);
            result = GetList<T>(dr);

                return result;
        }

        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T">泛型实体</typeparam>
        /// <param name="condition">不须传递WHERE</param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(string condition) where T : BaseEntity, new()
        {
            List<T> result;

            var dr = DbUtil.ExecuteReader(DbHelper, CurrentTableName, condition);
            result = GetList<T>(dr);

            return result;
        }

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
            var dr = DbUtil.ExecuteReader2(DbHelper, CurrentTableName, condition, topLimit, order);
            result = GetList<T>(dr);


            return result;
        }

        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T">泛型实体</typeparam>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public List<T> GetList<T>(IDataReader dataReader) where T : BaseEntity, new()
        {
            // 还能继承 IBaseEntity<T>
            var result = new List<T>();
            if (dataReader != null)
            {
                while (dataReader.Read())
                {
                    // T t = new T();
                    // listT.Add(t.GetFrom(dr));
                    // T dynTemp = BaseEntity.Create<T>();
                    // listT.Add((T)dynTemp.GetFrom(dr));
                    result.Add(BaseEntity.Create<T>(dataReader, false));
                }
                dataReader.Close();
            }


            return result;
        }
    }
}
