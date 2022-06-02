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
        #region SetEnabled object id

        /// <summary>
        /// 设置启用
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="recordUser">记录修改人</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns>影响行数</returns>
        public virtual int SetEnabled(object id, bool recordUser = true, bool baseOperationLog = true, string clientIp = null)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 1));
            if (recordUser && UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserId, UserInfo.UserId));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserName, UserInfo.UserName));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateBy, UserInfo.RealName));
                //宋彪发现这里的错误 文字与格式字符串错误
                //parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedOn: BaseUtil.FieldUpdateTime, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateTime, DateTime.Now));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateIp, !string.IsNullOrEmpty(clientIp) ? clientIp : Utils.GetIp()));
            }
            //业务条件
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(PrimaryKey, id),
                new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 0),
                new KeyValuePair<string, object>(BaseUtil.FieldDeleted, 0)
            };
            var result = SetProperty(whereParameters, parameters);
            if (result > 0)
            {
                RemoveCache();
                //操作日志
                if (baseOperationLog)
                {
                    new BaseOperationLogManager(UserInfo).QuickAdd(CurrentTableName, CurrentTableDescription, "Enable", id.ToString());
                }
            }
            return result;
        }
        #endregion

        #region SetEnabled string id

        /// <summary>
        /// 设置启用
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="recordUser">记录修改人</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns>影响行数</returns>
        public virtual int SetEnabled(string id, bool recordUser = true, bool baseOperationLog = true, string clientIp = null)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 1));
            if (recordUser && UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserId, UserInfo.UserId));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserName, UserInfo.UserName));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateBy, UserInfo.RealName));
                //宋彪发现这里的错误 文字与格式字符串错误
                //parameters.Add(new KeyValuePair<string, object>(version == 4 ? BaseUtil.FieldModifiedOn: BaseUtil.FieldUpdateTime, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateTime, DateTime.Now));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateIp, !string.IsNullOrEmpty(clientIp) ? clientIp : Utils.GetIp()));
            }
            //业务条件
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(PrimaryKey, id),
                new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 0),
                new KeyValuePair<string, object>(BaseUtil.FieldDeleted, 0)
            };
            var result = SetProperty(whereParameters, parameters);
            if (result > 0)
            {
                RemoveCache();
                //操作日志
                if (baseOperationLog)
                {
                    new BaseOperationLogManager(UserInfo).QuickAdd(CurrentTableName, CurrentTableDescription, "Enable", id);
                }
            }
            return result;
        }
        #endregion

        #region SetEnabled object[] ids

        /// <summary>
        /// 批量设置启用
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="enabled">有效</param>
        /// <param name="recordUser">修改人</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns>影响行数</returns>
        public virtual int SetEnabled(object[] ids, bool enabled = true, bool recordUser = true, bool baseOperationLog = true, string clientIp = null)
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
                    result += SetEnabled(t, recordUser, baseOperationLog, clientIp);
                }
            }
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion

        #region SetEnabled string[] ids

        /// <summary>
        /// 批量设置启用
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="recordUser">修改人</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns>影响行数</returns>
        public virtual int SetEnabled(string[] ids, bool recordUser = true, bool baseOperationLog = true, string clientIp = null)
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
                    result += SetEnabled(t, recordUser, baseOperationLog, clientIp);
                }
            }
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion

        #region SetEnabled List<KeyValuePair<string, object>> whereParameters

        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="whereParameters">条件字段，值</param>
        /// <param name="recordUser">记录修改用户</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns>影响行数</returns>
        public virtual int SetEnabled(List<KeyValuePair<string, object>> whereParameters, bool recordUser = true, string clientIp = null)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 1));
            if (recordUser && UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserId, UserInfo.UserId));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserName, UserInfo.UserName));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateBy, UserInfo.RealName));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateTime, DateTime.Now));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateIp, !string.IsNullOrEmpty(clientIp) ? clientIp : Utils.GetIp()));
            }
            var result = SetProperty(whereParameters, parameters);
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion
    }
}