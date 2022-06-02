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
    ///     2021.11.15 版本：5 Troy.Cui 增加clientIp可选参数
    ///     2018.08.29 版本：3 Troy.Cui 增加操作成功后自动RemoveCache功能
    ///		2016.06.13 版本：Troy.Cui进行扩展。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.06.13</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region UndoSetDeleted object id

        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="changeEnabled">修改有效状态</param>
        /// <param name="recordUser">记录修改用户</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="checkAllowDelete">检查允许删除</param>
        /// <returns>影响行数</returns>
        public virtual int UndoSetDeleted(object id, bool changeEnabled = true, bool recordUser = true, bool baseOperationLog = true, string clientIp = null, bool checkAllowDelete = false)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldDeleted, 0));
            if (changeEnabled)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 1));
            }
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
                new KeyValuePair<string, object>(BaseUtil.FieldDeleted, 1),
                new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 0)
            };
            if (checkAllowDelete)
            {
                whereParameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldAllowDelete, 1));
            }
            var result = SetProperty(whereParameters, parameters);
            if (result > 0)
            {
                RemoveCache();
                //操作日志
                if (baseOperationLog)
                {
                    new BaseOperationLogManager(UserInfo).QuickAdd(CurrentTableName, CurrentTableDescription, "Delete", id.ToString());
                }
            }
            return result;
        }
        #endregion

        #region UndoSetDeleted string id

        /// <summary>
        /// 撤销批量设置删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="changeEnabled">修改有效状态</param>
        /// <param name="recordUser">记录修改用户</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns>影响行数</returns>
        public virtual int UndoSetDeleted(string id, bool changeEnabled = true, bool recordUser = true, bool baseOperationLog = true, string clientIp = null)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldDeleted, 0));
            if (changeEnabled)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 1));
            }
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
                new KeyValuePair<string, object>(BaseUtil.FieldDeleted, 1),
                new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 0)
            };
            var result = SetProperty(whereParameters, parameters);
            if (result > 0)
            {
                RemoveCache();
                //操作日志
                if (baseOperationLog)
                {
                    new BaseOperationLogManager(UserInfo).QuickAdd(CurrentTableName, CurrentTableDescription, "Delete", id);
                }
            }
            return result;
        }
        #endregion

        #region UndoSetDeleted object[] ids

        /// <summary>
        /// 撤销批量设置删除
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="changeEnabled">修改有效状态</param>
        /// <param name="recordUser">记录修改用户</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns>影响行数</returns>
        public virtual int UndoSetDeleted(object[] ids, bool changeEnabled = true, bool recordUser = true, bool baseOperationLog = true, string clientIp = null)
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
                    result += UndoSetDeleted(t, changeEnabled, recordUser, baseOperationLog, clientIp);
                }
            }
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion

        #region UndoSetDeleted string[] ids

        /// <summary>
        /// 撤销批量设置删除
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="changeEnabled">修改有效状态</param>
        /// <param name="recordUser">记录修改用户</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="checkAllowDelete">检查允许删除</param>
        /// <returns>影响行数</returns>
        public virtual int UndoSetDeleted(string[] ids, bool changeEnabled = true, bool recordUser = true, bool baseOperationLog = true, string clientIp = null, bool checkAllowDelete = false)
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
                    result += UndoSetDeleted(t, changeEnabled, recordUser, baseOperationLog, clientIp, checkAllowDelete);
                }
            }
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion

        #region UndoSetDeleted List<KeyValuePair<string, object>> whereParameters

        /// <summary>
        /// 撤销批量设置删除 Troy Cui 2017.12.01 新增
        /// </summary>
        /// <param name="whereParameters">条件参数</param>
        /// <param name="changeEnabled">修改有效状态</param>
        /// <param name="recordUser">记录修改用户</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="checkAllowDelete">检查是否允许删除</param>
        /// <returns>影响行数</returns>
        public virtual int UndoSetDeleted(List<KeyValuePair<string, object>> whereParameters, bool changeEnabled = true, bool recordUser = true, bool baseOperationLog = true, string clientIp = null, bool checkAllowDelete = false)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldDeleted, 0));
            if (changeEnabled)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 1));
            }
            if (recordUser && UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserId, UserInfo.UserId));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserName, UserInfo.UserName));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateBy, UserInfo.RealName));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateTime, DateTime.Now));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateIp, !string.IsNullOrEmpty(clientIp) ? clientIp : Utils.GetIp()));
            }
            if (checkAllowDelete)
            {
                whereParameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldAllowDelete, 1));
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