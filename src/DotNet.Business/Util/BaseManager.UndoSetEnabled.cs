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
    ///     2021.11.15 版本：5 Troy.Cui 增加
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.06.13</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region UndoSetEnabled object id

        /// <summary>
        /// 批量设置禁用
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="recordUser">保存禁用人</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns>影响行数</returns>
        public virtual int UndoSetEnabled(object id, bool recordUser = true, bool baseOperationLog = true, string clientIp = null)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 0));
            if (recordUser && UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserId, UserInfo.UserId));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserName, UserInfo.UserName));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateBy, UserInfo.RealName));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateTime, DateTime.Now));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateIp, !string.IsNullOrEmpty(clientIp) ? clientIp : Utils.GetIp()));
            }
            //业务条件
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(PrimaryKey, id),
                new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUtil.FieldDeleted, 0)
            };
            var result = Update(whereParameters, parameters);
            if (result > 0)
            {
                RemoveCache();
                //操作日志
                if (baseOperationLog)
                {
                    new BaseOperationLogManager(UserInfo).QuickAdd(CurrentTableName, CurrentTableDescription, "Disable", id.ToString());
                }
            }
            return result;
        }
        #endregion

        #region UndoSetEnabled string id

        /// <summary>
        /// 撤销批量设置禁用
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="recordUser">禁用人</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns>影响行数</returns>
        public virtual int UndoSetEnabled(string id, bool recordUser = true, bool baseOperationLog = true, string clientIp = null)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 0));
            if (recordUser && UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserId, UserInfo.UserId));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserName, UserInfo.UserName));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateBy, UserInfo.RealName));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateTime, DateTime.Now));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateIp, !string.IsNullOrEmpty(clientIp) ? clientIp : Utils.GetIp()));
            }
            //业务条件
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(PrimaryKey, id),
                new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUtil.FieldDeleted, 0)
            };
            var result = Update(whereParameters, parameters);
            if (result > 0)
            {
                RemoveCache();
                //操作日志
                if (baseOperationLog)
                {
                    new BaseOperationLogManager(UserInfo).QuickAdd(CurrentTableName, CurrentTableDescription, "Disable", id.ToString());
                }
            }
            return result;
        }
        #endregion

        #region UndoSetEnabled object[] ids

        /// <summary>
        /// 撤销批量设置禁用
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="recordUser">禁用人</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns>影响行数</returns>
        public virtual int UndoSetEnabled(object[] ids, bool recordUser = true, bool baseOperationLog = true, string clientIp = null)
        {
            // 循环执行
            var result = 0;
            if (ids == null)
            {
                result += 0;
            }
            else
            {
                foreach (var t in ids)
                {
                    result += UndoSetEnabled(t, recordUser, baseOperationLog, clientIp);
                }
            }
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion

        #region UndoSetEnabled string[] ids

        /// <summary>
        /// 撤销批量设置禁用
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="recordUser">禁用人</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns>影响行数</returns>
        public virtual int UndoSetEnabled(string[] ids, bool recordUser = true, bool baseOperationLog = true, string clientIp = null)
        {
            // 循环执行
            var result = 0;
            if (ids == null)
            {
                result += 0;
            }
            else
            {
                foreach (var t in ids)
                {
                    result += UndoSetEnabled(t, recordUser, baseOperationLog, clientIp);
                }
            }
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion
    }
}