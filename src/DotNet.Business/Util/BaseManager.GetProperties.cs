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
    ///		<name>Troy.Cui</name>
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
            return DbHelper.GetProperties(CurrentTableName, null, 0, BaseUtil.FieldId);
        }
        /// <summary>
        /// 获取编号
        /// </summary>
        /// <param name="topLimit">前多少行</param>
        /// <param name="targetField">目标字段</param>
        /// <returns></returns>
        public virtual string[] GetIds(int topLimit, string targetField)
        {
            return DbHelper.GetProperties(CurrentTableName, null, topLimit, targetField);
        }
        /// <summary>
        /// 获取编号
        /// </summary>
        /// <param name="parameter1">参数1</param>
        /// <param name="parameter2">参数2</param>
        /// <returns></returns>
        public virtual string[] GetIds(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter1,
                parameter2
            };
            return DbHelper.GetProperties(CurrentTableName, parameters, 0, BaseUtil.FieldId);
        }
        /// <summary>
        /// 获取编号
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public virtual string[] GetIds(KeyValuePair<string, object> parameter)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter
            };
            return DbHelper.GetProperties(CurrentTableName, parameters, 0, BaseUtil.FieldId);
        }
        /// <summary>
        /// 获取编号
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public virtual string[] GetIds(string name, Object[] values)
        {
            return DbHelper.GetProperties(CurrentTableName, name, values, BaseUtil.FieldId);
        }
        /// <summary>
        /// 获取编号
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual string[] GetIds(List<KeyValuePair<string, object>> parameters)
        {
            return DbHelper.GetProperties(CurrentTableName, parameters, 0, BaseUtil.FieldId);
        }
        /// <summary>
        /// 获取编号
        /// </summary>
        /// <param name="parameters">参数</param>
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
        /// <param name="targetField">目标字段</param>
        /// <returns></returns>
        public virtual string[] GetProperties(string targetField)
        {
            return DbHelper.GetProperties(CurrentTableName, null, 0, targetField);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="targetField">目标字段</param>
        /// <returns></returns>
        public virtual string[] GetProperties(KeyValuePair<string, object> parameter, string targetField)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter
            };
            return DbHelper.GetProperties(CurrentTableName, parameters, 0, targetField);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="topLimit">前多少行</param>
        /// <param name="targetField">目标字段</param>
        /// <returns></returns>
        public virtual string[] GetProperties(KeyValuePair<string, object> parameter, int topLimit, string targetField)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter
            };
            return DbHelper.GetProperties(CurrentTableName, parameters, topLimit, targetField);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="topLimit">前多少行</param>
        /// <param name="targetField">目标字段</param>
        /// <returns></returns>
        public virtual string[] GetProperties(int topLimit, string targetField)
        {
            return DbHelper.GetProperties(CurrentTableName, null, topLimit, targetField);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="parameter1">参数1</param>
        /// <param name="parameter2">参数2</param>
        /// <param name="targetField">目标字段</param>
        /// <returns></returns>
        public virtual string[] GetProperties(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2, string targetField)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter1,
                parameter2
            };
            return DbHelper.GetProperties(CurrentTableName, parameters, 0, targetField);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="parameter1">参数1</param>
        /// <param name="parameter2">参数2</param>
        /// <param name="topLimit">前多少行</param>
        /// <param name="targetField">目标字段</param>
        /// <returns></returns>
        public virtual string[] GetProperties(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2, int topLimit, string targetField)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter1,
                parameter2
            };
            return DbHelper.GetProperties(CurrentTableName, parameters, topLimit, targetField);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="values">值</param>
        /// <param name="targetField">目标字段</param>
        /// <returns></returns>
        public virtual string[] GetProperties(string name, Object[] values, string targetField)
        {
            return DbHelper.GetProperties(CurrentTableName, name, values, targetField);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <param name="targetField">目标字段</param>
        /// <returns></returns>
        public virtual string[] GetProperties(List<KeyValuePair<string, object>> parameters, string targetField)
        {
            return DbHelper.GetProperties(CurrentTableName, parameters, 0, targetField);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <param name="topLimit">前多少行</param>
        /// <param name="targetField">目标字段</param>
        /// <returns></returns>
        public virtual string[] GetProperties(List<KeyValuePair<string, object>> parameters, int topLimit = 0, string targetField = null)
        {
            return DbHelper.GetProperties(CurrentTableName, parameters, topLimit, targetField);
        }

        #endregion
    }
}