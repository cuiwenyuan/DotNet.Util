//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Specialized;

namespace DotNet.Util
{
    /// <summary>
    /// SQLBuilder
    /// SQL语句生成器（适合简单的添加、删除、更新等语句，可以写出编译时强类型检查的效果）
    /// 
    /// 修改记录
    ///    
    ///		2012.02.16 版本：1.0 JiRiGaLa   主键创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.02.16</date>
    /// </author> 
    /// </summary>
    public partial class SqlBuilder
    {
        #region public string SetWhere(NameValueCollection parameters, string relation = " AND ")
        /// <summary>
        /// 设置条件
        /// </summary>
        /// <param name="parameters">条件</param>
        /// <param name="relation">条件AND或OR</param>
        /// <returns>条件语句</returns>
        public void SetWhere(NameValueCollection parameters, string relation = " AND ")
        {
            foreach (var key in parameters.AllKeys)
            {
                SetWhere(key, parameters[key], relation);
            }
        }
        #endregion
    }
}