//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;

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
    ///		2012.02.04 版本：1.0 JiRiGaLa 进行提炼，把代码进行分组。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2012.02.04</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region 获取编号
        /// <summary>
        /// 获取编号
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetIds()
        {
            return DbUtil.GetProperties(DbHelper, CurrentTableName, null, 0, BaseUtil.FieldId);
        }
        /// <summary>
        /// 获取编号
        /// </summary>
        /// <param name="topLimit"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        public virtual string[] GetIds(int topLimit, string targetField)
        {
            return DbUtil.GetProperties(DbHelper, CurrentTableName, null, topLimit, targetField);
        }
        /// <summary>
        /// 获取编号
        /// </summary>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <returns></returns>
        public virtual string[] GetIds(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter1,
                parameter2
            };
            return DbUtil.GetProperties(DbHelper, CurrentTableName, parameters, 0, BaseUtil.FieldId);
        }
        /// <summary>
        /// 获取编号
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual string[] GetIds(KeyValuePair<string, object> parameter)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter
            };
            return DbUtil.GetProperties(DbHelper, CurrentTableName, parameters, 0, BaseUtil.FieldId);
        }
        /// <summary>
        /// 获取编号
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual string[] GetIds(string name, Object[] values)
        {
            return DbUtil.GetProperties(DbHelper, CurrentTableName, name, values, BaseUtil.FieldId);
        }
        /// <summary>
        /// 获取编号
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual string[] GetIds(List<KeyValuePair<string, object>> parameters)
        {
            return DbUtil.GetProperties(DbHelper, CurrentTableName, parameters, 0, BaseUtil.FieldId);
        }
        /// <summary>
        /// 获取编号
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual string[] GetIds(params KeyValuePair<string, object>[] parameters)
        {
            var parametersList = new List<KeyValuePair<string, object>>();
            foreach (var parameter in parameters)
            {
                parametersList.Add(parameter);
            }
            return GetIds(parametersList);
        }

        #endregion

        #region 获取属性
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="targetField"></param>
        /// <returns></returns>
        public virtual string[] GetProperties(string targetField)
        {
            return DbUtil.GetProperties(DbHelper, CurrentTableName, null, 0, targetField);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        public virtual string[] GetProperties(KeyValuePair<string, object> parameter, string targetField)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter
            };
            return DbUtil.GetProperties(DbHelper, CurrentTableName, parameters, 0, targetField);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="topLimit"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        public virtual string[] GetProperties(KeyValuePair<string, object> parameter, int topLimit, string targetField)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter
            };
            return DbUtil.GetProperties(DbHelper, CurrentTableName, parameters, topLimit, targetField);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="topLimit"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        public virtual string[] GetProperties(int topLimit, string targetField)
        {
            return DbUtil.GetProperties(DbHelper, CurrentTableName, null, topLimit, targetField);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        public virtual string[] GetProperties(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2, string targetField)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter1,
                parameter2
            };
            return DbUtil.GetProperties(DbHelper, CurrentTableName, parameters, 0, targetField);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="topLimit"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        public virtual string[] GetProperties(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2, int topLimit, string targetField)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter1,
                parameter2
            };
            return DbUtil.GetProperties(DbHelper, CurrentTableName, parameters, topLimit, targetField);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        public virtual string[] GetProperties(string name, Object[] values, string targetField)
        {
            return DbUtil.GetProperties(DbHelper, CurrentTableName, name, values, targetField);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        public virtual string[] GetProperties(List<KeyValuePair<string, object>> parameters, string targetField)
        {
            return DbUtil.GetProperties(DbHelper, CurrentTableName, parameters, 0, targetField);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="topLimit"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        public virtual string[] GetProperties(List<KeyValuePair<string, object>> parameters, int topLimit = 0, string targetField = null)
        {
            return DbUtil.GetProperties(DbHelper, CurrentTableName, parameters, topLimit, targetField);
        }

        #endregion
    }
}