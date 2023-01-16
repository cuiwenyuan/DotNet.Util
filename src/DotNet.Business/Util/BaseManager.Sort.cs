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
    ///		2022.12.18 版本：Troy.Cui增加。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2022.12.18</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region 置顶排序(SortCode变小)
        /// <summary>
        /// 置顶排序(SortCode变小)
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int SetTop(object id)
        {
            var managerSequence = new BaseSequenceManager(dbHelper);
            // 先重置一下，避免手动添加的数据造成功能无效
            managerSequence.Reset(CurrentTableName);
            var sequence = managerSequence.GetReduction(CurrentTableName);
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldId, id)
            };
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldSortCode, sequence)
            };
            //return dbHelper.Update(CurrentTableName, whereParameters, parameters);
            return Update(whereParameters, parameters);
        }
        #endregion

        #region 置底排序(SortCode变大)
        /// <summary>
        /// 置底排序(SortCode变小)
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int SetBottom(object id)
        {
            var managerSequence = new BaseSequenceManager(dbHelper);
            // 先重置一下，避免手动添加的数据造成功能无效
            managerSequence.Reset(CurrentTableName);
            var sequence = managerSequence.Increment(CurrentTableName);
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldId, id)
            };
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldSortCode, sequence)
            };
            //return dbHelper.Update(CurrentTableName, whereParameters, parameters);
            return Update(whereParameters, parameters);
        }
        #endregion

        #region 交换排序命令
        /// <summary>
        /// 交换排序方法
        /// </summary>
        /// <param name="id">记录主键</param>
        /// <param name="targetId">目标记录主键</param>
        /// <returns>影响行数</returns>
        public int Swap(object id, object targetId)
        {
            var result = 0;
            // 要移动的主键的排序码
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldId, id)
            };
            var sortCode = dbHelper.GetProperty(CurrentTableName, parameters, BaseUtil.FieldSortCode);
            // 目标主键的排序码
            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldId, targetId)
            };
            var targetSortCode = dbHelper.GetProperty(CurrentTableName, parameters, BaseUtil.FieldSortCode);

            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldId, id)
            };
            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldSortCode, targetSortCode)
            };
            // 设置目标主键的排序码
            //result += dbHelper.Update(CurrentTableName, whereParameters, parameters);
            result += Update(whereParameters, parameters);
            whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldId, targetId)
            };
            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldSortCode, sortCode)
            };
            // 设置目标主键的排序码
            //result += dbHelper.Update(CurrentTableName, whereParameters, parameters);
            result += Update(whereParameters, parameters);
            return result;
        }
        #endregion

        #region 获得上一个记录主键
        /// <summary>
        /// 获得上一个记录主键(SortCode变小)
        /// </summary>
        /// <param name="categoryCode">类别主键</param>
        /// <param name="id">当前主键</param>
        /// <param name="whereSubQuery">Where条件</param>
        /// <returns>目标主键</returns>
        public string GetUpId(string categoryCode, string id, string whereSubQuery = null)
        {
            var subQuery = string.Empty;
            if (!string.IsNullOrEmpty(categoryCode))
            {
                subQuery = BaseUtil.FieldCategoryCode + " = '" + categoryCode + "' AND ";
            }
            if (!string.IsNullOrEmpty(whereSubQuery))
            {
                subQuery += whereSubQuery + " AND ";
            }

            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) FROM " + CurrentTableName
                + " WHERE (" + subQuery + BaseUtil.FieldSortCode + " < (SELECT TOP 1 ISNULL(" + BaseUtil.FieldSortCode
                + "," + BaseUtil.FieldId + ") FROM " + CurrentTableName
                + " WHERE (" + subQuery + BaseUtil.FieldId + " = '" + id + "') ORDER BY " + BaseUtil.FieldSortCode + "))");
            if (dbHelper.ExecuteScalar(sb.ToString()).Equals("0"))
            {
                sb.Clear();
                sb.Append("SELECT TOP 1 " + BaseUtil.FieldId
                                           + " FROM " + CurrentTableName
                                           + " WHERE (" + subQuery + BaseUtil.FieldSortCode + " <= (SELECT TOP 1 ISNULL(" + BaseUtil.FieldSortCode
                                           + "," + BaseUtil.FieldId + ") FROM " + CurrentTableName
                                           + " WHERE (" + subQuery + BaseUtil.FieldId + " = '" + id + "') ORDER BY " + BaseUtil.FieldSortCode + ")) ORDER BY " + BaseUtil.FieldSortCode + " DESC, " + BaseUtil.FieldId + " DESC");
            }
            else
            {
                sb.Clear();
                sb.Append("SELECT TOP 1 " + BaseUtil.FieldId
                                           + " FROM " + CurrentTableName
                                           + " WHERE (" + subQuery + BaseUtil.FieldSortCode + " < (SELECT TOP 1 ISNULL(" + BaseUtil.FieldSortCode
                                           + "," + BaseUtil.FieldId + ") FROM " + CurrentTableName
                                           + " WHERE (" + subQuery + BaseUtil.FieldId + " = '" + id + "') ORDER BY " + BaseUtil.FieldSortCode + ")) ORDER BY " + BaseUtil.FieldSortCode + " DESC, " + BaseUtil.FieldId + " DESC");
            }

            return dbHelper.ExecuteScalar(sb.Put())?.ToString();
        }
        #endregion

        #region 获得上一个记录主键
        /// <summary>
        /// 获得上一个记录主键
        /// </summary>
        /// <param name="id">当前主键</param>
        /// <param name="whereSubQuery">Where条件</param>
        /// <returns>目标主键</returns>
        public string GetUpId(string id, string whereSubQuery = null)
        {
            // 有bug
            return GetUpId(CurrentTableName, id);
        }
        #endregion

        #region 上移(SortCode变大)

        /// <summary>
        /// 上移(SortCode变大)
        /// </summary>
        /// <param name="categoryId">类别主键</param>
        /// <param name="id">当前主键</param>
        /// <param name="whereSubQuery"></param>
        /// <returns>目标主键</returns>
        public int SetUp(string categoryId, string id, string whereSubQuery = null)
        {
            var upId = GetUpId(categoryId, id, whereSubQuery);

            var result = 0;
            if (upId.Length == 0)
            {
                return result;
            }

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldId, id)
            };
            var sortCode = dbHelper.GetProperty(CurrentTableName, parameters, BaseUtil.FieldSortCode);

            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldId, upId)
            };
            var upSortCode = dbHelper.GetProperty(CurrentTableName, parameters, BaseUtil.FieldSortCode);

            //把upId的SortCode更新为当前SortCode
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldId, upId)
            };
            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldSortCode, sortCode)
            };
            dbHelper.SetProperty(CurrentTableName, whereParameters, parameters);

            //把当前的SortCode更新为upId的upSortCode
            whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldId, id)
            };
            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldSortCode, upSortCode)
            };
            //result = dbHelper.Update(CurrentTableName, whereParameters, parameters);
            result = Update(whereParameters, parameters);
            return result;
        }
        #endregion

        #region 上移(SortCode变大)

        /// <summary>
        /// 上移(SortCode变大)
        /// </summary>
        /// <param name="id">当前主键</param>
        /// <param name="whereSubQuery"></param>
        /// <returns>目标主键</returns>
        public int SetUp(string id, string whereSubQuery = null)
        {
            return SetUp(string.Empty, id, whereSubQuery);
        }
        #endregion

        #region 获得下一个记录主键

        /// <summary>
        /// 获得下一个记录主键(SortCode变大)
        /// </summary>
        /// <param name="categoryCode">类别主键</param>
        /// <param name="id">当前主键</param>
        /// <param name="whereSubQuery"></param>
        /// <returns>目标主键</returns>
        public string GetDownId(string categoryCode, string id, string whereSubQuery = null)
        {
            var subQuery = string.Empty;
            if (!string.IsNullOrEmpty(categoryCode))
            {
                subQuery = BaseUtil.FieldCategoryCode + " = '" + categoryCode + "' AND ";
            }
            if (!string.IsNullOrEmpty(whereSubQuery))
            {
                subQuery += whereSubQuery + " AND ";
            }

            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) FROM " + CurrentTableName
                + " WHERE (" + subQuery + BaseUtil.FieldSortCode + " > (SELECT TOP 1 ISNULL(" + BaseUtil.FieldSortCode
                + "," + BaseUtil.FieldId + ") FROM " + CurrentTableName
                + " WHERE (" + subQuery + BaseUtil.FieldId + " = '" + id + "') ORDER BY " + BaseUtil.FieldSortCode + " ))");
            if (dbHelper.ExecuteScalar(sb.ToString()).Equals("0"))
            {
                sb.Clear();
                sb.Append("SELECT TOP 1 " + BaseUtil.FieldId
                                           + " FROM " + CurrentTableName
                                           + " WHERE (" + subQuery + BaseUtil.FieldSortCode + " >= (SELECT TOP 1 ISNULL(" + BaseUtil.FieldSortCode
                                           + "," + BaseUtil.FieldId + ") FROM " + CurrentTableName
                                           + " WHERE (" + subQuery + BaseUtil.FieldId + " = '" + id + "') ORDER BY " + BaseUtil.FieldSortCode + " )) ORDER BY " + BaseUtil.FieldSortCode + " ASC, " + BaseUtil.FieldId + " ASC");
            }
            else
            {
                sb.Clear();
                sb.Append("SELECT TOP 1 " + BaseUtil.FieldId
                                           + " FROM " + CurrentTableName
                                           + " WHERE (" + subQuery + BaseUtil.FieldSortCode + " > (SELECT TOP 1 ISNULL(" + BaseUtil.FieldSortCode
                                           + "," + BaseUtil.FieldId + ") FROM " + CurrentTableName
                                           + " WHERE (" + subQuery + BaseUtil.FieldId + " = '" + id + "') ORDER BY " + BaseUtil.FieldSortCode + " )) ORDER BY " + BaseUtil.FieldSortCode + " ASC, " + BaseUtil.FieldId + " ASC");
            }

            return dbHelper.ExecuteScalar(sb.Put())?.ToString();
        }
        #endregion

        #region 获得下一个记录主键
        /// <summary>
        /// 获得下一个记录主键
        /// </summary>
        /// <param name="id">当前主键</param>
        /// <returns>目标主键</returns>
        public string GetDownId(string id)
        {
            return GetDownId(string.Empty, id);
        }
        #endregion

        #region 下移(SortCode变小)

        /// <summary>
        /// 下移(SortCode变小)
        /// </summary>
        /// <param name="categoryId">类别主键</param>
        /// <param name="id">当前主键</param>
        /// <param name="whereSubQuery"></param>
        /// <returns>目标主键</returns>
        public int SetDown(string categoryId, string id, string whereSubQuery = null)
        {
            var result = 0;
            var downId = GetDownId(categoryId, id, whereSubQuery);
            if (downId.Length == 0)
            {
                return result;
            }

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldId, id)
            };
            var sortCode = dbHelper.GetProperty(CurrentTableName, parameters, BaseUtil.FieldSortCode);
            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldId, downId)
            };
            var downSortCode = dbHelper.GetProperty(CurrentTableName, parameters, BaseUtil.FieldSortCode);

            //把downId的SortCode更新为当前SortCode
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldId, downId)
            };
            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldSortCode, sortCode)
            };
            dbHelper.SetProperty(CurrentTableName, whereParameters, parameters);

            //把当前的SortCode更新为downId的downSortCode
            whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldId, id)
            };
            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldSortCode, downSortCode)
            };
            //result = dbHelper.Update(CurrentTableName, whereParameters, parameters);
            result = Update(whereParameters, parameters);
            return result;
        }
        #endregion

        #region 下移记录的方法
        /// <summary>
        /// 下移记录的方法
        /// </summary>
        /// <param name="id">当前主键</param>
        /// <param name="whereSubQuery">Where条件</param>
        /// <returns>目标主键</returns>
        public int SetDown(string id, string whereSubQuery = null)
        {
            return SetDown(string.Empty, id, whereSubQuery);
        }
        #endregion
    }
}