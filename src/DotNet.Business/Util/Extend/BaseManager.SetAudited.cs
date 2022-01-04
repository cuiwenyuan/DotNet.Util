﻿//-----------------------------------------------------------------
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
    /// 修改纪录
    ///
    ///     2020.06.11 版本：4 Troy.Cui 增加hasIsCancelled判断
    ///     2018.08.29 版本：3 Troy.Cui 增加操作成功后自动RemoveCache功能
    ///		2015.07.06 版本：Troy.Cui进行扩展。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.07.06</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region public virtual int SetAudited(object id, bool hasIsCancelled = true, bool recordUser = true) 审核标志

        /// <summary>
        /// 审核标志
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="hasIsCancelled">是否有IsCancelled字段</param>
        /// <param name="recordUser">是否记录审核人</param>
        /// <param name="tableVersion">版本默认4为老版本</param>
        /// <returns>影响行数</returns>
        public virtual int SetAudited(object id, bool hasIsCancelled = true, bool recordUser = true, int tableVersion = 4)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldIsAudited, 1));
            if (recordUser && UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldAuditedUserId, UserInfo.Id));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldAuditedUserName, UserInfo.UserName));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldAuditedDate, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            }
            //业务条件
            var whereParameters = new List<KeyValuePair<string, object>>();
            whereParameters.Add(new KeyValuePair<string, object>(PrimaryKey, id));
            //未删除
            whereParameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 1));
            whereParameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldDeletionStateCode : BaseUtil.FieldDeleted, 0));
            if (hasIsCancelled)
            {
                //未取消
                whereParameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldIsCancelled, 0));
            }

            //未审核
            //会报错An item with the same key has already been added.
            //条件参数已修复，特更新这里的逻辑，Troy.Cui 2018.01.06
            whereParameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldIsAudited, 0));
            var result = SetProperty(whereParameters, parameters);
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion

        #region public virtual int SetAudited(object[] ids, bool audit = true, bool recordUser = true) 批量审核标志

        /// <summary>
        /// 批量审核标志
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="hasIsCancelled">是否有IsCancelled字段</param>
        /// <param name="recordUser">是否记录审核人</param>
        /// <param name="tableVersion">版本默认4为老版本</param>
        /// <returns>影响行数</returns>
        public virtual int SetAudited(object[] ids, bool hasIsCancelled = true, bool recordUser = true, int tableVersion = 4)
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
                    result += SetAudited(t, hasIsCancelled, recordUser, tableVersion);
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