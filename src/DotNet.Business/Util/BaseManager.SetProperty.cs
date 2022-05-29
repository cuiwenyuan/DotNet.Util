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
        /// 设置属性
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual int SetProperty(KeyValuePair<string, object> parameter)
        {
            var parameters = new List<KeyValuePair<string, object>> { parameter };
            return DbHelper.SetProperty(CurrentTableName, null, parameters);
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="id">条件参数</param>
        /// <param name="parameter">更新参数</param>
        /// <returns></returns>
        public virtual int SetProperty(string id, KeyValuePair<string, object> parameter)
        {
            return SetProperty(new KeyValuePair<string, object>(PrimaryKey, id), parameter);
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="id">条件参数</param>
        /// <param name="parameter">更新参数</param>
        /// <returns></returns>
        public virtual int SetProperty(object id, KeyValuePair<string, object> parameter)
        {
            return SetProperty(new KeyValuePair<string, object>(PrimaryKey, id), parameter);
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="id">条件参数</param>
        /// <param name="parameters">更新参数</param>
        /// <returns></returns>
        public virtual int SetProperty(object id, List<KeyValuePair<string, object>> parameters)
        {
            return SetProperty(new KeyValuePair<string, object>(PrimaryKey, id), parameters);
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="ids">数组条件参数</param>
        /// <param name="parameter">更新参数</param>
        /// <returns></returns>
        public virtual int SetProperty(object[] ids, KeyValuePair<string, object> parameter)
        {
            return SetProperty(PrimaryKey, ids, parameter);
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="ids">数组条件参数</param>
        /// <param name="parameters">更新参数</param>
        /// <returns></returns>
        public virtual int SetProperty(object[] ids, List<KeyValuePair<string, object>> parameters)
        {
            return SetProperty(PrimaryKey, ids, parameters);
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="name">条件参数名</param>
        /// <param name="values">条件参数值</param>
        /// <param name="parameter">更新参数</param>
        /// <returns></returns>
        public virtual int SetProperty(string name, object[] values, KeyValuePair<string, object> parameter)
        {
            var result = 0;
            if (values == null)
            {
                result += SetProperty(new KeyValuePair<string, object>(name, string.Empty), parameter);
            }
            else
            {
                for (var i = 0; i < values.Length; i++)
                {
                    result += SetProperty(new KeyValuePair<string, object>(name, values[i]), parameter);
                }
            }
            return result;
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="name">条件参数名</param>
        /// <param name="values">条件参数值</param>
        /// <param name="parameters">更新参数</param>
        /// <returns></returns>
        public virtual int SetProperty(string name, object[] values, List<KeyValuePair<string, object>> parameters)
        {
            var result = 0;
            if (values == null)
            {
                result += SetProperty(new KeyValuePair<string, object>(name, string.Empty), parameters);
            }
            else
            {
                for (var i = 0; i < values.Length; i++)
                {
                    result += SetProperty(new KeyValuePair<string, object>(name, values[i]), parameters);
                }
            }
            return result;
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="whereParameter1">条件参数1</param>
        /// <param name="whereParameter2">条件参数2</param>
        /// <param name="parameter">更新参数</param>
        /// <returns></returns>
        public virtual int SetProperty(KeyValuePair<string, object> whereParameter1, KeyValuePair<string, object> whereParameter2, KeyValuePair<string, object> parameter)
        {
            var whereParameters = new List<KeyValuePair<string, object>> { whereParameter1, whereParameter2 };
            var parameters = new List<KeyValuePair<string, object>> { parameter };

            return DbHelper.SetProperty(CurrentTableName, whereParameters, parameters);
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="whereParameter">条件参数</param>
        /// <param name="parameter">更新参数</param>
        /// <returns></returns>
        public virtual int SetProperty(KeyValuePair<string, object> whereParameter, KeyValuePair<string, object> parameter)
        {
            var whereParameters = new List<KeyValuePair<string, object>> { whereParameter };
            var parameters = new List<KeyValuePair<string, object>> { parameter };
            return DbHelper.SetProperty(CurrentTableName, whereParameters, parameters);
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="whereParameters">条件参数</param>
        /// <param name="parameter">更新参数</param>
        /// <returns></returns>
        public virtual int SetProperty(List<KeyValuePair<string, object>> whereParameters, KeyValuePair<string, object> parameter)
        {
            var parameters = new List<KeyValuePair<string, object>> { parameter };

            return DbHelper.SetProperty(CurrentTableName, whereParameters, parameters);
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="whereParameter">条件参数</param>
        /// <param name="parameters">更新参数</param>
        /// <returns></returns>
        public virtual int SetProperty(KeyValuePair<string, object> whereParameter, List<KeyValuePair<string, object>> parameters)
        {
            var whereParameters = new List<KeyValuePair<string, object>> { whereParameter };
            return DbHelper.SetProperty(CurrentTableName, whereParameters, parameters);
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="whereParameters">条件参数</param>
        /// <param name="parameters">更新参数</param>
        /// <returns></returns>
        public virtual int SetProperty(List<KeyValuePair<string, object>> whereParameters, List<KeyValuePair<string, object>> parameters)
        {
            return DbHelper.SetProperty(CurrentTableName, whereParameters, parameters);
        }
    }
}