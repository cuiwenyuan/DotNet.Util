//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;

namespace DotNet.Business
{
    using Util;

    /// <summary>
    ///	BaseManager
    /// 通用基类部分
    /// 
    /// 总觉得自己写的程序不上档次，这些新技术也玩玩，也许做出来的东西更专业了。
    /// 修改记录
    /// 
    ///		2012.02.04 版本：1.0 JiRiGaLa 进行提炼，把代码进行分组。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.02.04</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        //
        // 记录导航功能
        //

        private string _previousId = string.Empty; // 上一个记录主键。
        private string _nextId = string.Empty; // 下一个记录主键。

        #region public DataTable GetPreviousNextId(bool deletionStateCode, string id) 获得主键列表

        /// <summary>
        /// 获得主键列表中的，上一条，下一条记录的主键
        /// </summary>
        /// <param name="deletionStateCode">是否删除</param>
        /// <param name="id">主键</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        public DataTable GetPreviousNextId(bool deletionStateCode, string id, string order)
        {
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT Id "
                            + " FROM " + CurrentTableName
                            + " WHERE (" + BaseUtil.FieldCreateUserId + " = " + DbHelper.GetParameter(BaseUtil.FieldCreateUserId)
                            + " AND " + (BaseUtil.FieldDeleted) + " = " + (deletionStateCode ? 1 : 0) + ")"
                            + " ORDER BY " + order);
            var names = new string[1];
            var values = new Object[1];
            names[0] = BaseUtil.FieldCreateUserId;
            values[0] = UserInfo.Id;
            var dt = new DataTable(CurrentTableName);
            DbHelper.Fill(dt, sb.Put(), DbHelper.MakeParameters(names, values));
            _nextId = GetNextId(dt, id);
            _previousId = GetPreviousId(dt, id);
            return dt;
        }
        #endregion

        #region public void GetPreviousNextId(DataTable result, string id) 获得主键列表
        /// <summary>
        /// 获取下一条、下一条记录主键
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="id">当前记录主键</param>
        public void GetPreviousNextId(DataTable dt, string id)
        {
            _previousId = GetPreviousId(dt, id);
            _nextId = GetNextId(dt, id);
        }
        #endregion

        #region private string GetPreviousId(string id) 获取上一条记录主键

        /// <summary>
        /// 获取上一条记录主键
        /// </summary>
        /// <param name="id">当前记录主键</param>
        /// <returns>上一条记录主键</returns>
        private string GetPreviousId(string id)
        {
            var result = string.Empty;
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT TOP 1 " + BaseUtil.FieldId
                                      + " FROM " + CurrentTableName
                                      + " WHERE " + BaseUtil.FieldCreateTime + " = (SELECT MAX(" + BaseUtil.FieldCreateTime + ")"
                                      + " FROM " + CurrentTableName
                                      + " WHERE (" + BaseUtil.FieldCreateTime + "< (SELECT " + BaseUtil.FieldCreateTime
                                      + " FROM " + CurrentTableName
                                      + " WHERE " + BaseUtil.FieldId + " = " + DbHelper.GetParameter(BaseUtil.FieldId) + "))");
            sb.Append(" AND (" + BaseUtil.FieldCreateUserId + " = " + DbHelper.GetParameter(BaseUtil.FieldCreateUserId) + " ) AND ( " + BaseUtil.FieldDeleted + " = 0 )) ");
            var names = new string[2];
            var values = new Object[2];
            names[0] = BaseUtil.FieldId;
            values[0] = id;
            names[1] = BaseUtil.FieldCreateUserId;
            values[1] = UserInfo.Id;
            var returnObject = DbHelper.ExecuteScalar(sb.Put(), DbHelper.MakeParameters(names, values));
            if (returnObject != null)
            {
                result = returnObject.ToString();
            }
            return result;
        }
        #endregion

        #region public string GetPreviousId(DataTable result, string id) 获取上一条记录ID
        /// <summary>
        /// 获取上一条记录主键
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="id">当前记录主键</param>
        /// <returns>上一条记录主键</returns>
        public string GetPreviousId(DataTable dt, string id)
        {
            var result = string.Empty;
            foreach (DataRowView dataRowView in dt.DefaultView)
            {
                if (dataRowView[BaseUtil.FieldId].ToString().Equals(id))
                {
                    break;
                }
                result = dataRowView[BaseUtil.FieldId].ToString();
            }

            return result;
        }
        #endregion

        #region private string GetNextId(string id) 获取下一条记录主键

        /// <summary>
        /// 获取下一条记录主键
        /// </summary>
        /// <param name="id">当前记录主键</param>
        /// <returns>下一条记录主键</returns>
        private string GetNextId(string id)
        {
            var result = string.Empty;
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT TOP 1 " + BaseUtil.FieldId
                                      + " FROM " + CurrentTableName
                                      + " WHERE " + BaseUtil.FieldCreateTime + " = (SELECT MIN(" + BaseUtil.FieldCreateTime + ")"
                                      + " FROM " + CurrentTableName
                                      + " WHERE (" + BaseUtil.FieldCreateTime + "> (SELECT " + BaseUtil.FieldCreateTime
                                      + " FROM " + CurrentTableName
                                      + " WHERE " + BaseUtil.FieldId + " = " + DbHelper.GetParameter(BaseUtil.FieldId) + "))");
            sb.Append(" AND (" + BaseUtil.FieldCreateUserId + " = " + DbHelper.GetParameter(BaseUtil.FieldCreateUserId) + ") AND ( " + (BaseUtil.FieldDeleted) + " = 0 )) ");
            var names = new string[2];
            var values = new Object[2];
            names[0] = BaseUtil.FieldId;
            values[0] = id;
            names[1] = BaseUtil.FieldCreateUserId;
            values[1] = UserInfo.Id;
            var returnObject = DbHelper.ExecuteScalar(sb.Put(), DbHelper.MakeParameters(names, values));
            if (returnObject != null)
            {
                result = returnObject.ToString();
            }
            return result;
        }
        #endregion

        #region public string GetNextId(DataTable result, string id) 获取下一条记录主键
        /// <summary>
        /// 获取下一条记录主键
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="id">当前记录主键</param>
        /// <returns>下一条记录主键</returns>
        public string GetNextId(DataTable dt, string id)
        {
            var result = string.Empty;
            var finded = false;
            foreach (DataRowView dataRowView in dt.DefaultView)
            {
                if (finded)
                {
                    result = dataRowView[BaseUtil.FieldId].ToString();
                    break;
                }
                if (dataRowView[BaseUtil.FieldId].ToString().Equals(id))
                {
                    finded = true;
                }
            }
            return result;
        }
        #endregion
    }
}