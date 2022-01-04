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
    /// 修改纪录
    /// 
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
        #region public virtual int UndoSetDeleted(object id, bool changeEnabled = true, bool recordUser = false) 批量设置删除

        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="changeEnabled">更改是否有效</param>
        /// <param name="recordUser">保存删除人</param>
        /// <param name="tableVersion">版本默认4为老版本</param>
        /// <returns>影响行数</returns>
        public virtual int UndoSetDeleted(object id, bool changeEnabled = true, bool recordUser = true, int tableVersion = 4)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldDeletionStateCode : BaseUtil.FieldDeleted, 0));
            if (changeEnabled)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 1));
            }
            if (recordUser && UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedUserId : BaseUtil.FieldUpdateUserId, UserInfo.Id));
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedOn : BaseUtil.FieldUpdateTime, DateTime.Now));
            }
            //业务条件
            var whereParameters = new List<KeyValuePair<string, object>>();
            whereParameters.Add(new KeyValuePair<string, object>(PrimaryKey, id));
            //已删除
            //会报错An item with the same key has already been added.
            //条件参数已修复，特更新这里的逻辑，Troy.Cui 2018.01.06
            if (changeEnabled)
            {
                whereParameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 0));
            }
            whereParameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldDeletionStateCode : BaseUtil.FieldDeleted, 1));
            var result = SetProperty(whereParameters, parameters);
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion

        #region UndoSetDeleted

        /// <summary>
        /// 撤销批量设置删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="changeEnabled">更改是否有效</param>
        /// <param name="recordUser">删除人</param>
        /// <param name="tableVersion">版本默认4为老版本</param>
        /// <returns>影响行数</returns>
        public virtual int UndoSetDeleted(string id, bool changeEnabled = true, bool recordUser = true, int tableVersion = 4)
        {
            var parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldDeletionStateCode : BaseUtil.FieldDeleted, 0));
            if (changeEnabled)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 1));
            }
            if (recordUser && UserInfo != null)
            {
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedUserId : BaseUtil.FieldUpdateUserId, UserInfo.Id));
                parameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldModifiedOn : BaseUtil.FieldUpdateTime, DateTime.Now));
            }
            //业务条件
            var whereParameters = new List<KeyValuePair<string, object>>();
            whereParameters.Add(new KeyValuePair<string, object>(PrimaryKey, id));
            //已删除
            //会报错An item with the same key has already been added.
            //条件参数已修复，特更新这里的逻辑，Troy.Cui 2018.01.06
            if (changeEnabled)
            {
                whereParameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldEnabled, 0));
            }
            whereParameters.Add(new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldDeletionStateCode : BaseUtil.FieldDeleted, 1));
            var result = SetProperty(whereParameters, parameters);
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion

        #region UndoSetDeleted

        /// <summary>
        /// 撤销批量设置删除
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="changeEnabled">更改是否有效</param>
        /// <param name="recordUser">删除人</param>
        /// <param name="tableVersion">版本默认4为老版本</param>
        /// <returns>影响行数</returns>
        public virtual int UndoSetDeleted(object[] ids, bool changeEnabled = true, bool recordUser = true, int tableVersion = 4)
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
                    result += UndoSetDeleted(t, changeEnabled, recordUser, tableVersion);
                }
            }
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        #endregion

        #region UndoSetDeleted

        /// <summary>
        /// 撤销批量设置删除
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="changeEnabled">更改是否有效</param>
        /// <param name="recordUser">删除人</param>
        /// <param name="tableVersion">版本默认4为老版本</param>
        /// <returns>影响行数</returns>
        public virtual int UndoSetDeleted(string[] ids, bool changeEnabled = true, bool recordUser = true, int tableVersion = 4)
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
                    result += UndoSetDeleted(t, changeEnabled, recordUser, tableVersion);
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