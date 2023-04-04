//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
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
    /// 修改记录
    ///
    ///     2021.11.15 版本：1 Troy.Cui 新增
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2021.11.15</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region SetDisabled object id

        /// <summary>
        /// 设置启用
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="recordUser">记录修改人</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns>影响行数</returns>
        public virtual int SetDisabled(object id, bool recordUser = true, bool baseOperationLog = true, string clientIp = null)
        {
            return UndoSetEnabled(id, recordUser, baseOperationLog, clientIp);
        }
        #endregion

        #region SetDisabled string id

        /// <summary>
        /// 设置启用
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="recordUser">记录修改人</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns>影响行数</returns>
        public virtual int SetDisabled(string id, bool recordUser = true, bool baseOperationLog = true, string clientIp = null)
        {
            return UndoSetEnabled(id, recordUser, baseOperationLog, clientIp);
        }
        #endregion

        #region SetDisabled object[] ids

        /// <summary>
        /// 批量设置启用
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="recordUser">修改人</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns>影响行数</returns>
        public virtual int SetDisabled(object[] ids, bool recordUser = true, bool baseOperationLog = true, string clientIp = null)
        {
            return UndoSetEnabled(ids, recordUser, baseOperationLog, clientIp);
        }
        #endregion

        #region SetDisabled string[] ids

        /// <summary>
        /// 批量设置启用
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="recordUser">修改人</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns>影响行数</returns>
        public virtual int SetDisabled(string[] ids, bool recordUser = true, bool baseOperationLog = true, string clientIp = null)
        {
            return UndoSetEnabled(ids, recordUser, baseOperationLog, clientIp);
        }
        #endregion

        #region SetDisabled List<KeyValuePair<string, object>> whereParameters

        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="whereParameters">条件字段，值</param>
        /// <param name="whereSql">条件Sql</param>
        /// <param name="recordUser">记录修改用户</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns>影响行数</returns>
        public virtual int SetDisabled(List<KeyValuePair<string, object>> whereParameters, string whereSql = null, bool recordUser = true, bool baseOperationLog = true, string clientIp = null)
        {
            return UndoSetEnabled(whereParameters, whereSql: whereSql, recordUser: recordUser, baseOperationLog: baseOperationLog, clientIp: clientIp);
        }
        #endregion
    }
}