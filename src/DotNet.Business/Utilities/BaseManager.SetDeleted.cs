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
    ///     2020.04.28 版本：4 Troy.Cui 增加TableVersion的功能，兼容老版本的程序
    ///     2018.08.29 版本：3 Troy.Cui 增加操作成功后自动RemoveCache功能
    ///		2012.03.27 版本：2.1 JiRiGaLa 进行整理。
    ///		2012.03.21 版本：2.0 JiRiGaLa 进行整理。
    ///		2012.02.04 版本：1.0 JiRiGaLa 进行提炼，把代码进行分组。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2012.03.27</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region SetDeleted

        /// <summary>
        /// 是否删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="changeEnabled">修改有效状态</param>
        /// <param name="recordUser">记录修改人</param>
        /// <param name="tableVersion">版本默认4为老版本</param>
        /// <returns>影响行数</returns>
        public virtual int SetDeleted(object id, bool changeEnabled = true, bool recordUser = true, int tableVersion = 4)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldDeletionStateCode : BaseUtil.FieldDeleted, 1));
            if (changeEnabled)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 0));
            }
            if (recordUser && UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedUserId : BaseUtil.FieldUpdateUserId, UserInfo.Id));
                if (tableVersion > 4)
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserName, UserInfo.UserName));
                }
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedBy : BaseUtil.FieldUpdateBy, UserInfo.RealName));
                //宋彪发现这里的错误 文字与格式字符串错误
                //parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedOn: BaseUtil.FieldUpdateTime, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedOn : BaseUtil.FieldUpdateTime, DateTime.Now));
            }
            var result = SetProperty(new KeyValuePair<string, object>(PrimaryKey, id), parameters);
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion

        #region SetDeleted

        /// <summary>
        /// 是否删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="tableVersion">版本默认4为老版本</param>
        /// <param name="changeEnabled">修改有效状态</param>
        /// <param name="recordUser">记录修改人</param>
        /// <returns>影响行数</returns>
        public virtual int SetDeleted(string id, bool changeEnabled = true, bool recordUser = true, int tableVersion = 4)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldDeletionStateCode : BaseUtil.FieldDeleted, 1));
            if (changeEnabled)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 0));
            }
            if (recordUser && UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedUserId : BaseUtil.FieldUpdateUserId, UserInfo.Id));
                if (tableVersion > 4)
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserName, UserInfo.UserName));
                }
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedBy : BaseUtil.FieldUpdateBy, UserInfo.RealName));
                //宋彪发现这里的错误 文字与格式字符串错误
                //parameters.Add(new KeyValuePair<string, object>(version == 4 ? BaseUtil.FieldModifiedOn: BaseUtil.FieldUpdateTime, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedOn : BaseUtil.FieldUpdateTime, DateTime.Now));
            }
            var result = SetProperty(new KeyValuePair<string, object>(PrimaryKey, id), parameters);
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion

        #region SetDeleted

        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="tableVersion">版本默认4为老版本</param>
        /// <param name="enabled">有效</param>
        /// <param name="recordUser">修改人</param>
        /// <returns>影响行数</returns>
        public virtual int SetDeleted(object[] ids, bool enabled = true, bool recordUser = true, int tableVersion = 4)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldDeletionStateCode : BaseUtil.FieldDeleted, 1));
            if (enabled)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 0));
            }
            if (recordUser && UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedUserId : BaseUtil.FieldUpdateUserId, UserInfo.Id));
                if (tableVersion > 4)
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserName, UserInfo.UserName));
                }
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedBy : BaseUtil.FieldUpdateBy, UserInfo.RealName));
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedOn : BaseUtil.FieldUpdateTime, DateTime.Now));
            }
            var result = SetProperty(ids, parameters);
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion

        #region SetDeleted

        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="enabled">有效</param>
        /// <param name="recordUser">修改人</param>
        /// <param name="tableVersion">版本默认4为老版本</param>
        /// <returns>影响行数</returns>
        public virtual int SetDeleted(string[] ids, bool enabled = true, bool recordUser = true, int tableVersion = 4)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldDeletionStateCode : BaseUtil.FieldDeleted, 1));
            if (enabled)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 0));
            }
            if (recordUser && UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedUserId : BaseUtil.FieldUpdateUserId, UserInfo.Id));
                if (tableVersion > 4)
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserName, UserInfo.UserName));
                }
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedBy : BaseUtil.FieldUpdateBy, UserInfo.RealName));
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedOn : BaseUtil.FieldUpdateTime, DateTime.Now));
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedIp : BaseUtil.FieldUpdateIp, Utils.GetIp()));
            }
            var result = SetProperty(ids, parameters);
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion

        #region public virtual int SetDeleted(List<KeyValuePair<string, object>> whereParameters, bool recordUser = false) 批量设置删除

        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="whereParameters">条件字段，值</param>
        /// <param name="recordUser">记录修改用户</param>
        /// <param name="tableVersion">版本默认4为老版本</param>
        /// <returns>影响行数</returns>
        public virtual int SetDeleted(List<KeyValuePair<string, object>> whereParameters, bool recordUser = true, int tableVersion = 4)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldDeletionStateCode : BaseUtil.FieldDeleted, 1));
            if (recordUser && UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedUserId : BaseUtil.FieldUpdateUserId, UserInfo.Id));
                if (tableVersion > 4)
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserName, UserInfo.UserName));
                }
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedBy : BaseUtil.FieldUpdateBy, UserInfo.RealName));
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedOn : BaseUtil.FieldUpdateTime, DateTime.Now));
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedIp : BaseUtil.FieldUpdateIp, Utils.GetIp()));
            }
            var result = SetProperty(whereParameters, parameters);
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion

        #region SetDeleted

        /// <summary>
        /// 批量设置删除 Troy Cui 2017.12.01 新增
        /// </summary>
        /// <param name="whereParameters">条件字段，值</param>
        /// <param name="changeEnabled">有效标识</param>
        /// <param name="changeDeleted">删除标识</param>
        /// <param name="recordUser">记录修改用户</param>
        /// <param name="tableVersion">版本默认4为老版本</param>
        /// <returns>影响行数</returns>
        public virtual int SetDeleted(List<KeyValuePair<string, object>> whereParameters, bool changeEnabled = true, bool changeDeleted = true, bool recordUser = true, int tableVersion = 4)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            if (changeDeleted)
            {
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldDeletionStateCode : BaseUtil.FieldDeleted, 1));
            }
            if (changeEnabled)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 0));
            }
            if (recordUser && UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedUserId : BaseUtil.FieldUpdateUserId, UserInfo.Id));
                if (tableVersion > 4)
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserName, UserInfo.UserName));
                }
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedBy : BaseUtil.FieldUpdateBy, UserInfo.RealName));
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedOn : BaseUtil.FieldUpdateTime, DateTime.Now));
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