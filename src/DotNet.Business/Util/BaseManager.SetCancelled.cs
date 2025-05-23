﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
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
    ///     2020.06.11 版本：4 Troy.Cui 增加hasIsAudited判断
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
        #region SetCancelled object id

        /// <summary>
        /// 取消标志
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="hasIsAudited">是否有IsAudited字段</param>
        /// <param name="recordUser">是否记录取消人</param>
        /// <returns>影响行数</returns>
        public virtual int SetCancelled(object id, bool hasIsAudited = true, bool recordUser = true)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldIsCancelled, 1)
            };
            if (recordUser && UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldCancelledUserId, UserInfo.Id));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldCancelledUserName, UserInfo.UserName));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldCancelledDate, DateTime.Now));
            }
            //业务条件
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(PrimaryKey, id),
                new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUtil.FieldDeleted, 0)
            };
            if (hasIsAudited)
            {
                //未审核
                whereParameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldIsAudited, 0));
            }            
            //未取消
            whereParameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldIsCancelled, 0));
            var result = Update(whereParameters, parameters);
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion

        #region SetCancelled object[] ids

        /// <summary>
        /// 批量取消标志
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="hasIsAudited">是否有IsAudited字段</param>
        /// <param name="recordUser">是否记录取消人</param>
        /// <returns>影响行数</returns>
        public virtual int SetCancelled(object[] ids, bool hasIsAudited = true, bool recordUser = true)
        {
            // 循环执行
            var result = 0;
            if (ids == null)
            {
                result += 0;
            }
            else
            {
                foreach (var i in ids)
                {
                    result += SetCancelled(i, hasIsAudited, recordUser);
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