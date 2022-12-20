//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using DotNet.Util;
using System.Collections.Generic;

namespace DotNet.Business
{
    /// <summary>
    ///	BaseManager
    /// 通用基类部分
    /// 
    /// 总觉得自己写的程序不上档次，这些新技术也玩玩，也许做出来的东西更专业了。
    /// 修改记录
    /// 
    ///		2012.02.04 版本：1.0 JiRiGaLa 进行提炼，把代码进行分组。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.02.04</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Exists(object id)
        {
            var parameters = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(PrimaryKey, id) };
            return DbHelper.Exists(CurrentTableName, parameters);
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual bool Exists(params KeyValuePair<string, object>[] parameters)
        {
            var parametersList = new List<KeyValuePair<string, object>>();
            foreach (var p in parameters)
            {
                parametersList.Add(p);
            }
            return DbHelper.Exists(CurrentTableName, parametersList);
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="parameter">条件</param>
        /// <returns></returns>
        public virtual bool Exists(KeyValuePair<string, object> parameter)
        {
            var parameters = new List<KeyValuePair<string, object>> { parameter };
            return DbHelper.Exists(CurrentTableName, parameters);
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="parameter1">条件1</param>
        /// <param name="parameter2">条件2</param>
        /// <returns></returns>
        public virtual bool Exists(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2)
        {
            var parameters = new List<KeyValuePair<string, object>> { parameter1, parameter2 };
            return DbHelper.Exists(CurrentTableName, parameters);
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="parameter1">条件1</param>
        /// <param name="parameter2">条件2</param>
        /// <param name="id">排除编号</param>
        /// <returns></returns>
        public virtual bool Exists(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2, object id)
        {
            var parameters = new List<KeyValuePair<string, object>> { parameter1, parameter2 };
            return DbHelper.Exists(CurrentTableName, parameters, new KeyValuePair<string, object>(PrimaryKey, id));
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="parameter1">条件1</param>
        /// <param name="parameter2">条件2</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public virtual bool Exists(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2, KeyValuePair<string, object> parameter)
        {
            var parameters = new List<KeyValuePair<string, object>> { parameter1, parameter2 };
            return DbHelper.Exists(CurrentTableName, parameters, parameter);
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="id">排除主键</param>
        /// <returns></returns>
        public virtual bool Exists(KeyValuePair<string, object> parameter, object id)
        {
            var parameters = new List<KeyValuePair<string, object>> { parameter };
            //Troy.Cui 2019-03-02更新
            if (id != null && !string.IsNullOrWhiteSpace(id.ToString()))
            {

                return DbHelper.Exists(CurrentTableName, parameters, new KeyValuePair<string, object>(PrimaryKey, id));
            }
            else
            {
                return DbHelper.Exists(CurrentTableName, parameters);
            }
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <param name="id">排除主键</param>
        /// <returns></returns>
        public virtual bool Exists(List<KeyValuePair<string, object>> parameters, object id = null)
        {
            // 还要判断传来的不能是空格
            if (id != null && !string.IsNullOrWhiteSpace(id.ToString()))
            {
                return DbHelper.Exists(CurrentTableName, parameters, new KeyValuePair<string, object>(PrimaryKey, id));
            }
            else
            {
                return DbHelper.Exists(CurrentTableName, parameters);
            }
        }
    }
}