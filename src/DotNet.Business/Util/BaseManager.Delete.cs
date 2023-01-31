//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
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
    /// 
    ///		2012.03.20 版本：2.0 JiRiGaLa 代码进行简化。
    ///		2012.02.04 版本：1.0 JiRiGaLa 进行提炼，把代码进行分组。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.02.04</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region 删除

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public virtual int Delete()
        {
            return MyDelete(null);
            //return DbUtil.Delete(DbHelper, this.CurrentTableName);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual int Delete(object id)
        {
            var parameters = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(PrimaryKey, id) };
            return MyDelete(parameters);
            //return DbUtil.Delete(DbHelper, this.CurrentTableName, parameters);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual int Delete(object[] ids)
        {
            var result = 0;
            foreach (var t in ids)
            {
                var parameters = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(PrimaryKey, t) };
                result += MyDelete(parameters);
                //result += DbUtil.Delete(DbHelper, this.CurrentTableName, parameters);
            }
            return result;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public virtual int Delete(string name, object[] values)
        {
            var result = 0;
            foreach (var t in values)
            {
                var parameters = new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(name, t) };
                result += MyDelete(parameters);
                //result += DbUtil.Delete(DbHelper, this.CurrentTableName, parameters);
            }
            return result;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual int Delete(params KeyValuePair<string, object>[] parameters)
        {
            var parametersList = new List<KeyValuePair<string, object>>();
            foreach (var p in parameters)
            {
                parametersList.Add(p);
            }
            return MyDelete(parametersList);
            //return DbUtil.Delete(DbHelper, this.CurrentTableName, parametersList);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual int Delete(List<KeyValuePair<string, object>> parameters)
        {
            return MyDelete(parameters);
            //return DbUtil.Delete(DbHelper, this.CurrentTableName, parameters);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
		private int MyDelete(List<KeyValuePair<string, object>> parameters)
        {
            parameters = GetDeleteExtParam(parameters);
            return DbHelper.Delete(CurrentTableName, parameters);
        }

        #endregion

        #region public virtual int Truncate() Truncate
        /// <summary>
        /// Truncate
        /// </summary>
        /// <returns></returns>
        public virtual int Truncate()
        {
            return DbHelper.Truncate(CurrentTableName);
        }

        #endregion

        /// <summary>
        /// 添加删除的附加条件
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        protected virtual List<KeyValuePair<string, object>> GetDeleteExtParam(List<KeyValuePair<string, object>> parameters)
        {
            return parameters ?? new List<KeyValuePair<string, object>>();
        }
    }
}