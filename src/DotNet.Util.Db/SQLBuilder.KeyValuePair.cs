//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Util
{
    /// <summary>
    /// SQLBuilder
    /// SQL语句生成器（适合简单的添加、删除、更新等语句，可以写出编译时强类型检查的效果）
    /// 
    /// 修改记录
    ///    
    ///		2012.02.07 版本：1.0 JiRiGaLa   主键创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.02.07</date>
    /// </author> 
    /// </summary>
    public partial class SqlBuilder
    {
        #region SetValue public void SetValue(KeyValuePair<string, object> parameter)
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="parameter">目标字段，值</param>
        public void SetValue(KeyValuePair<string, object> parameter)
        {
            SetValue(parameter.Key, parameter.Value, parameter.Key);
        }
        #endregion

        #region public string SetWhere(KeyValuePair<string, object> parameter, string relation = " AND ") 设置条件
        /// <summary>
        /// 设置条件
        /// </summary>
        /// <param name="parameter">参数条件</param>
        /// <param name="relation">条件 AND OR</param>
        /// <returns>条件语句</returns>
        public void SetWhere(KeyValuePair<string, object> parameter, string relation = " AND ")
        {
            SetWhere(parameter.Key, parameter.Value, parameter.Key, relation);
            // string result = this.WhereSql;
            // return result;
        }
        #endregion

        #region public string SetWhere(List<KeyValuePair<string, object>> parameters, string relation = " AND ") 设置条件
        /// <summary>
        /// 设置条件
        /// </summary>
        /// <param name="parameters">参数条件</param>
        /// <param name="relation">条件 AND OR</param>
        /// <returns>条件语句</returns>
        public void SetWhere(List<KeyValuePair<string, object>> parameters, string relation = " AND ")
        {
            foreach (var key in parameters)
            {
                SetWhere(key.Key, key.Value, key.Key, relation);
            }
            // string result = this.WhereSql;
            // return result;
        }
        #endregion
    }
}