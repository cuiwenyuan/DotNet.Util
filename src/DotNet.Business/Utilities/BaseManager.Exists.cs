//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

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
    ///		<name>JiRiGaLa</name>
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
            return DbUtil.Exists(DbHelper, CurrentTableName, parameters);
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual bool Exists(params KeyValuePair<string, object>[] parameters)
        {
            var parametersList = new List<KeyValuePair<string, object>>();
            foreach (var t in parameters)
            {
                parametersList.Add(t);
            }
            return DbUtil.Exists(DbHelper, CurrentTableName, parametersList);
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual bool Exists(KeyValuePair<string, object> parameter)
        {
            var parameters = new List<KeyValuePair<string, object>> { parameter };
            return DbUtil.Exists(DbHelper, CurrentTableName, parameters);
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <returns></returns>
        public virtual bool Exists(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2)
        {
            var parameters = new List<KeyValuePair<string, object>> { parameter1, parameter2 };
            return DbUtil.Exists(DbHelper, CurrentTableName, parameters);
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Exists(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2, object id)
        {
            var parameters = new List<KeyValuePair<string, object>> { parameter1, parameter2 };
            return DbUtil.Exists(DbHelper, CurrentTableName, parameters, new KeyValuePair<string, object>(PrimaryKey, id));
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual bool Exists(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2, KeyValuePair<string, object> parameter)
        {
            var parameters = new List<KeyValuePair<string, object>> { parameter1, parameter2 };
            return DbUtil.Exists(DbHelper, CurrentTableName, parameters, parameter);
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual bool Exists(KeyValuePair<string, object> parameter, object id)
        {
            var parameters = new List<KeyValuePair<string, object>> { parameter };
            //Troy.Cui 2019-03-02更新
            if (id != null && !string.IsNullOrWhiteSpace(id.ToString()))
            {

                return DbUtil.Exists(DbHelper, CurrentTableName, parameters, new KeyValuePair<string, object>(PrimaryKey, id));
            }
            else
            {
                return DbUtil.Exists(DbHelper, CurrentTableName, parameters);
            }
        }

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual bool Exists(List<KeyValuePair<string, object>> parameters, object id = null)
        {
            // 还要判断传来的不能是空格
            if (id != null && !string.IsNullOrWhiteSpace(id.ToString()))
            {
                return DbUtil.Exists(DbHelper, CurrentTableName, parameters, new KeyValuePair<string, object>(PrimaryKey, id));
            }
            else
            {
                return DbUtil.Exists(DbHelper, CurrentTableName, parameters);
            }
        }
    }
}