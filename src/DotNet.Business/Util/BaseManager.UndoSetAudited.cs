//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

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
        #region UndoSetAudited object id

        /// <summary>
        /// 撤销审核标志
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="hasIsCancelled">是否有IsCancelled字段</param>
        /// <param name="recordUser">是否记录审核人</param>
        /// <returns>影响行数</returns>
        public virtual int UndoSetAudited(object id, bool hasIsCancelled = true, bool recordUser = true)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldIsAudited, 0)
            };
            if (recordUser && UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldAuditedUserId, null));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldAuditedUserName, null));
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldAuditedDate, null));
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

            //已审核
            whereParameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldIsAudited, 1));
            var result = Update(whereParameters, parameters);
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion

        #region UndoSetAudited object[] ids

        /// <summary>
        /// 批量撤销审核标志
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="hasIsCancelled"></param>
        /// <param name="recordUser">是否记录审核人</param>
        /// <returns>影响行数</returns>
        public virtual int UndoSetAudited(object[] ids, bool hasIsCancelled = true, bool recordUser = true)
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
                    result += UndoSetAudited(t, hasIsCancelled, recordUser);
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