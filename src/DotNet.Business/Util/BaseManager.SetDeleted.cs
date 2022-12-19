//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
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
    ///     2022.01.11 版本：5 Troy.Cui 增加checkAllowDelete可选参数
    ///     2021.11.15 版本：5 Troy.Cui 增加clientIp可选参数
    ///     2020.04.28 版本：4 Troy.Cui 增加TableVersion的功能，兼容老版本的程序
    ///     2018.08.29 版本：3 Troy.Cui 增加操作成功后自动RemoveCache功能
    ///		2012.03.27 版本：2.1 JiRiGaLa 进行整理。
    ///		2012.03.21 版本：2.0 JiRiGaLa 进行整理。
    ///		2012.02.04 版本：1.0 JiRiGaLa 进行提炼，把代码进行分组。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.03.27</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region SetDeleted object id

        /// <summary>
        /// 是否删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="changeEnabled">修改有效状态</param>
        /// <param name="recordUser">记录修改用户</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="checkAllowDelete">检查允许删除</param>
        /// <returns>影响行数</returns>
        public virtual int SetDeleted(object id, bool changeEnabled = true, bool recordUser = true, bool baseOperationLog = true, string clientIp = null, bool checkAllowDelete = false)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldDeleted, 1));
            if (changeEnabled)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 0));
            }
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
                new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUtil.FieldDeleted, 0)
            };
            if (checkAllowDelete)
            {
                whereParameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldAllowDelete, 1));
            }
            var result = Update(whereParameters, parameters);
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

        #region SetDeleted string id

        /// <summary>
        /// 是否删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="changeEnabled">修改有效状态</param>
        /// <param name="recordUser">记录修改用户</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="checkAllowDelete">检查允许删除</param>
        /// <returns>影响行数</returns>
        public virtual int SetDeleted(string id, bool changeEnabled = true, bool recordUser = true, bool baseOperationLog = true, string clientIp = null, bool checkAllowDelete = false)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldDeleted, 1));
            if (changeEnabled)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 0));
            }
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
                new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUtil.FieldDeleted, 0)
            };
            if (checkAllowDelete)
            {
                whereParameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldAllowDelete, 1));
            }
            var result = Update(whereParameters, parameters);
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

        #region SetDeleted object[] ids

        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="changeEnabled">修改有效状态</param>
        /// <param name="recordUser">记录修改用户</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="checkAllowDelete">检查允许删除</param>
        /// <returns>影响行数</returns>
        public virtual int SetDeleted(object[] ids, bool changeEnabled = true, bool recordUser = true, bool baseOperationLog = true, string clientIp = null, bool checkAllowDelete = false)
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
                    result += SetDeleted(t, changeEnabled, recordUser, baseOperationLog, clientIp, checkAllowDelete);
                }
            }
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion

        #region SetDeleted string[] ids

        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="changeEnabled">修改有效状态</param>
        /// <param name="recordUser">记录修改用户</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="checkAllowDelete">检查允许删除</param>
        /// <returns>影响行数</returns>
        public virtual int SetDeleted(string[] ids, bool changeEnabled = true, bool recordUser = true, bool baseOperationLog = true, string clientIp = null, bool checkAllowDelete = false)
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
                    result += SetDeleted(t, changeEnabled, recordUser, baseOperationLog, clientIp, checkAllowDelete);
                }
            }
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion

        #region SetDeleted List<KeyValuePair<string, object>> whereParameters

        /// <summary>
        /// 批量设置删除 Troy Cui 2017.12.01 新增
        /// </summary>
        /// <param name="whereParameters">条件参数</param>
        /// <param name="changeEnabled">修改有效状态</param>
        /// <param name="recordUser">记录修改用户</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="checkAllowDelete">检查允许删除</param>
        /// <returns>影响行数</returns>
        public virtual int SetDeleted(List<KeyValuePair<string, object>> whereParameters, bool changeEnabled = true, bool recordUser = true, bool baseOperationLog = true, string clientIp = null, bool checkAllowDelete = false)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldDeleted, 1));
            if (changeEnabled)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 0));
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
            var result = Update(whereParameters, parameters);
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion
    }
}