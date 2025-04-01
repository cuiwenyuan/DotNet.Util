//-----------------------------------------------------------------
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
    ///     2025.02.10 版本：5 增加新功能，用于发料状态
    ///     2022.05.25 版本：5 为不影响老程序，新的字段和新的功能改用新的方法
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
        #region public virtual int Issue(object id, bool hasIsCancelled = false, bool recordUser = true)

        /// <summary>
        /// 发料标志
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="hasIsCancelled">是否有IsCancelled字段</param>
        /// <param name="recordUser">是否记录发料人</param>
        /// <returns>影响行数</returns>
        public virtual int Issue(object id, bool hasIsCancelled = false, bool recordUser = true)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldIsIssued, 1)
            };
            if (recordUser && UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldIssueTime, DateTime.Now));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldIssueUserId, UserInfo.Id));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldIssueUserName, UserInfo.UserName));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldIssueBy, UserInfo.RealName));
            }
            //业务条件
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(PrimaryKey, id),
                //未删除
                new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUtil.FieldDeleted, 0)
            };
            if (hasIsCancelled)
            {
                //未取消
                whereParameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldIsCancelled, 0));
            }

            //未发料
            whereParameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldIsIssued, 0));
            var result = Update(whereParameters, parameters);
            if (result > 0)
            {
                RemoveCache(id.ToInt());
            }
            return result;
        }
        #endregion

        #region public virtual int Issue(object[] ids, bool hasIsCancelled = false, bool recordUser = true)

        /// <summary>
        /// 批量发料标志
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="hasIsCancelled">是否有IsCancelled字段</param>
        /// <param name="recordUser">是否记录发料人</param>
        /// <returns>影响行数</returns>
        public virtual int Issue(object[] ids, bool hasIsCancelled = false, bool recordUser = true)
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
                    result += Issue(i, hasIsCancelled, recordUser);
                }
            }
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion

        #region public virtual int UndoIssue(object id, bool hasIsCancelled = false, bool recordUser = true)

        /// <summary>
        /// 撤销发料标志
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="hasIsCancelled">是否有IsCancelled字段</param>
        /// <param name="recordUser">是否记录发料人</param>
        /// <returns>影响行数</returns>
        public virtual int UndoIssue(object id, bool hasIsCancelled = false, bool recordUser = true)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldIsIssued, 0)
            };
            if (recordUser && UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldIssueTime, null));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldIssueUserId, 0));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldIssueUserName, null));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldIssueBy, null));
            }
            //业务条件
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(PrimaryKey, id),
                new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUtil.FieldDeleted, 0)
            };
            if (hasIsCancelled)
            {
                //未取消
                whereParameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldIsCancelled, 0));
            }

            //已发料
            whereParameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldIsIssued, 1));
            var result = Update(whereParameters, parameters);
            if (result > 0)
            {
                RemoveCache(id.ToInt());
            }
            return result;
        }
        #endregion

        #region public virtual int UndoIssue(object[] ids, bool hasIsCancelled = false, bool recordUser = true)

        /// <summary>
        /// 批量撤销发料标志
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="hasIsCancelled"></param>
        /// <param name="recordUser">是否记录发料人</param>
        /// <returns>影响行数</returns>
        public virtual int UndoIssue(object[] ids, bool hasIsCancelled = false, bool recordUser = true)
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
                    result += UndoIssue(i, hasIsCancelled, recordUser);
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