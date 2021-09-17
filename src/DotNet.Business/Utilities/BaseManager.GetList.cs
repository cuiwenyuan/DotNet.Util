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
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual List<T> GetListById<T>(string id) where T : BaseEntity, new()
        {
            return GetList<T>(new KeyValuePair<string, object>(BaseUtil.FieldId, id));
        }

        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="topLimit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(int topLimit = 0, string order = null) where T : BaseEntity, new()
        {
            List<T> result;

            using (var dr = DbUtil.ExecuteReader(DbHelper, CurrentTableName, null, topLimit, order))
            {
                result = GetList<T>(dr);
            }

            return result;
        }

        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(string[] ids) where T : BaseEntity, new()
        {
            List<T> result;

            using (var dr = DbUtil.ExecuteReader(DbHelper, CurrentTableName, "*", BaseUtil.FieldId, ids))
            {
                result = GetList<T>(dr);
            }

            return result;
        }

        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(string name, Object[] values, string order = null) where T : BaseEntity, new()
        {
            List<T> result;

            using (var dr = DbUtil.ExecuteReader(DbHelper, CurrentTableName, "*", name, values, order))
            {
                result = GetList<T>(dr);
            }

            return result;
        }

        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(KeyValuePair<string, object> parameter, string order) where T : BaseEntity, new()
        {
            List<T> result;

            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(parameter);
            
            using (var dr = DbUtil.ExecuteReader(DbHelper, CurrentTableName, parameters, 0, order))
            {
                result = GetList<T>(dr);
            }

            return result;
        }

        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2, string order) where T : BaseEntity, new()
        {
            List<T> result;

            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(parameter1);
            parameters.Add(parameter2);
            
            using (var dr = DbUtil.ExecuteReader(DbHelper, CurrentTableName, parameters, 0, order))
            {
                result = GetList<T>(dr);
            }

            return result;
        }

        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        /// <param name="topLimit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(KeyValuePair<string, object> parameter, int topLimit = 0, string order = null) where T : BaseEntity, new()
        {
            List<T> result;

            var parameters = new List<KeyValuePair<string, object>> { parameter };

            using (var dr = DbUtil.ExecuteReader(DbHelper, CurrentTableName, parameters, topLimit, order))
            {
                result = GetList<T>(dr);
            }

            return result;
        }

        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(List<KeyValuePair<string, object>> parameters, string order) where T : BaseEntity, new()
        {
            List<T> result;

            using (var dr = DbUtil.ExecuteReader(DbHelper, CurrentTableName, parameters, 0, order))
            {
                result = GetList<T>(dr);
            }

            return result;
        }

        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="topLimit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(List<KeyValuePair<string, object>> parameters, int topLimit = 0, string order = null) where T : BaseEntity, new()
        {
            List<T> result;

            using (var dr = DbUtil.ExecuteReader(DbHelper, CurrentTableName, parameters, topLimit, order))
            {
                result = GetList<T>(dr);
            }

            return result;
        }

        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(params KeyValuePair<string, object>[] parameters) where T : BaseEntity, new()
        {
            var result = new List<T>();

            var parametersList = new List<KeyValuePair<string, object>>();
            foreach (var t in parameters)
            {
                parametersList.Add(t);
            }
            
            using (var dr = DbUtil.ExecuteReader(DbHelper, CurrentTableName, parametersList))
            {
                result = GetList<T>(dr);
            }

            return result;
        }

        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">不须传递WHERE</param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(string condition) where T : BaseEntity, new()
        {
            List<T> result;

            using (var dr = DbUtil.ExecuteReader(DbHelper, CurrentTableName, condition))
            {
                result = GetList<T>(dr);
            }

            return result;
        }

        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <param name="topLimit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual List<T> GetList2<T>(string condition, int topLimit = 0, string order = null) where T : BaseEntity, new()
        {
            List<T> result;

            using (var dr = DbUtil.ExecuteReader2(DbHelper, CurrentTableName, condition, topLimit, order))
            {
                result = GetList<T>(dr);
            }

            return result;
        }

        /// <summary>
        /// 获取集合对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public List<T> GetList<T>(IDataReader dr) where T : BaseEntity, new()
        {
            // 还能继承 IBaseEntity<T>
            var result = new List<T>();

            // 2016-09-17 吉日嘎拉 这里防止数据库链接没关闭掉、把数据库拖累了。
            // using (dr)
            //{
            while (dr.Read())
            {
                // T t = new T();
                // listT.Add(t.GetFrom(dr));
                // T dynTemp = BaseEntity.Create<T>();
                // listT.Add((T)dynTemp.GetFrom(dr));
                result.Add(BaseEntity.Create<T>(dr, false));
            }
            dr.Close();
            //}

            return result;
        }

        /*
        public List<T> GetList<T>(IDataReader dr) where T : new()
        {
            // 还能继承 IBaseEntity<T>
            List<T> listT = new List<T>();
            while (dr.Read())
            {
                listT.Add(mapEntity(dr));
            }
            dr.Close();
            return listT;
        }
        */
    }
}
