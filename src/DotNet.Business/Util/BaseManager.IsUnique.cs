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
    ///     2020.07.09 版本：Troy.Cui调整顺序，增加2个字段的唯一性判断。
    ///		2020.07.03 版本：Troy.Cui进行扩展。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2020.07.03</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region public virtual bool IsUnique(string fieldValue, string excludeId, string fieldName, bool checkUserCompany = false) 是否唯一的

        /// <summary>
        /// 是否唯一的
        /// </summary>
        /// <param name="fieldValue">数据</param>
        /// <param name="excludeId">排除行id</param>
        /// <param name="fieldName">数据字段</param>
        /// <param name="checkUserCompany">是否检查公司数据</param>
        /// <returns>是否</returns>
        public virtual bool IsUnique(string fieldValue, string excludeId, string fieldName, bool checkUserCompany = false)
        {
            var result = false;
            //安全过滤一下
            fieldValue = dbHelper.SqlSafe(fieldValue);
            if (!string.IsNullOrEmpty(fieldName))
            {
                fieldName = dbHelper.SqlSafe(fieldName);
            }
            else
            {
                fieldName = "Name";
            }

            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) FROM " + CurrentTableName + " WHERE " + fieldName + " = N'" + fieldValue + "'");
            //未删除
            sb.Append(" AND " + (BaseUtil.FieldDeleted) + " = 0 AND " + BaseUtil.FieldEnabled + " = 1");
            //当前用户所在公司或者系统公用数据
            if (checkUserCompany)
            {
                sb.Append(" AND (" + BaseUtil.FieldUserCompanyId + " = 0 OR " + BaseUtil.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            }
            if (ValidateUtil.IsInt(excludeId))
            {
                sb.Append(" AND Id <> " + excludeId);
            }
            //需要显示未被删除的记录
            var obj = ExecuteScalar(sb.Return());
            if (obj != null && obj.ToInt() == 0)
            {
                result = true;
            }
            return result;
        }

        #endregion

        #region public virtual bool IsUnique(string field1Value, string field2Value, string excludeId, string field1Name, string field2Name, bool checkUserCompany = false) 是否唯一的
        /// <summary>
        /// 是否唯一的(两个字段)
        /// </summary>
        /// <param name="field1Value">数据1</param>
        /// <param name="field2Value">数据1</param>
        /// <param name="excludeId">排除行id</param>
        /// <param name="field1Name">数据字段1</param>
        /// <param name="field2Name">数据字段2</param>
        /// <param name="checkUserCompany">是否检查公司数据</param>
        /// <returns>是否</returns>
        public virtual bool IsUnique(string field1Value, string field2Value, string excludeId, string field1Name, string field2Name, bool checkUserCompany = false)
        {
            var result = false;
            //安全过滤一下
            field1Value = dbHelper.SqlSafe(field1Value);
            field2Value = dbHelper.SqlSafe(field2Value);
            if (!string.IsNullOrEmpty(field1Name))
            {
                field1Name = dbHelper.SqlSafe(field1Name);
            }
            else
            {
                field1Name = "Name";
            }
            if (!string.IsNullOrEmpty(field2Name))
            {
                field2Name = dbHelper.SqlSafe(field2Name);
            }
            else
            {
                field2Name = "Name";
            }

            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) FROM " + CurrentTableName + " WHERE " + field1Name + " = N'" + field1Value + "' AND " + field2Name + " = N'" + field2Value + "'");
            //未删除
            sb.Append(" AND " + (BaseUtil.FieldDeleted) + " = 0 AND " + BaseUtil.FieldEnabled + " = 1");
            //当前用户所在公司或者系统公用数据
            if (checkUserCompany)
            {
                sb.Append(" AND (" + BaseUtil.FieldUserCompanyId + " = 0 OR " + BaseUtil.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            }
            if (ValidateUtil.IsInt(excludeId))
            {
                sb.Append(" AND Id <> " + excludeId);
            }
            //需要显示未被删除的记录
            var obj = ExecuteScalar(sb.Return());
            if (obj != null && obj.ToInt() == 0)
            {
                result = true;
            }
            return result;
        }
        #endregion

        #region public virtual bool IsUnique(string field1Value, string field2Value, string field3Value, string excludeId, string field1Name, string field2Name, string field3Name, bool checkUserCompany = false) 是否唯一的

        /// <summary>
        /// 是否唯一的(三个字段)
        /// </summary>
        /// <param name="field1Value">数据1</param>
        /// <param name="field2Value">数据2</param>
        /// <param name="field3Value">数据3</param>
        /// <param name="excludeId">排除行id</param>
        /// <param name="field1Name">数据字段1</param>
        /// <param name="field2Name">数据字段2</param>
        /// <param name="field3Name">数据字段3</param>
        /// <param name="checkUserCompany">是否检查公司数据</param>
        /// <returns>是否</returns>
        public virtual bool IsUnique(string field1Value, string field2Value, string field3Value, string excludeId, string field1Name, string field2Name, string field3Name, bool checkUserCompany = false)
        {
            var result = false;
            //安全过滤一下
            field1Value = dbHelper.SqlSafe(field1Value);
            field2Value = dbHelper.SqlSafe(field2Value);
            if (!string.IsNullOrEmpty(field1Name))
            {
                field1Name = dbHelper.SqlSafe(field1Name);
            }
            else
            {
                field1Name = "Name";
            }
            if (!string.IsNullOrEmpty(field2Name))
            {
                field2Name = dbHelper.SqlSafe(field2Name);
            }
            else
            {
                field2Name = "Name";
            }
            if (!string.IsNullOrEmpty(field3Name))
            {
                field3Name = dbHelper.SqlSafe(field3Name);
            }
            else
            {
                field3Name = "Name";
            }

            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) FROM " + CurrentTableName + " WHERE " + field1Name + " = N'" + field1Value + "' AND " + field2Name + " = N'" + field2Value + "' AND " + field3Name + " = N'" + field3Value + "'");
            //未删除
            sb.Append(" AND " + (BaseUtil.FieldDeleted) + " = 0 AND " + BaseUtil.FieldEnabled + " = 1");
            //当前用户所在公司或者系统公用数据
            if (checkUserCompany)
            {
                sb.Append(" AND (" + BaseUtil.FieldUserCompanyId + " = 0 OR " + BaseUtil.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            }
            if (ValidateUtil.IsInt(excludeId))
            {
                sb.Append(" AND Id <> " + excludeId);
            }
            //需要显示未被删除的记录
            var obj = ExecuteScalar(sb.Return());
            if (obj != null && obj.ToInt() == 0)
            {
                result = true;
            }
            return result;
        }
        #endregion

        #region public virtual bool IsUnique(List<KeyValuePair<string, object>> whereParameters, string excludeId = null, bool checkUserCompany = false) 是否唯一的
        /// <summary>
        /// 是否唯一的(条件参数)
        /// </summary>
        /// <param name="whereParameters">条件参数</param>
        /// <param name="excludeId">排除行id</param>
        /// <param name="checkUserCompany">是否检查公司数据</param>
        /// <returns>是否</returns>
        public virtual bool IsUnique(List<KeyValuePair<string, object>> whereParameters, string excludeId = null, bool checkUserCompany = false)
        {
            var result = false;

            if (checkUserCompany)
            {
                whereParameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUserCompanyId, UserInfo.CompanyId));
            }

            //result = !Exists(whereParameters, excludeId);

            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) FROM " + CurrentTableName + " WHERE " + dbHelper.GetWhereString(whereParameters, BaseUtil.SqlLogicConditional));

            if (!string.IsNullOrEmpty(excludeId))
            {
                if (ValidateUtil.IsInt(excludeId))
                {
                    sb.Append(" " + BaseUtil.SqlLogicConditional + "(" + BaseUtil.FieldId + " <> " + excludeId + ") ");
                }
                else
                {
                    sb.Append(" " + BaseUtil.SqlLogicConditional + "(" + BaseUtil.FieldId + " <> '" + excludeId + "') ");
                }

            }

            object obj;
            if (whereParameters != null)
            {
                obj = dbHelper.ExecuteScalar(sb.Return(), dbHelper.MakeParameters(whereParameters));
            }
            else
            {
                obj = dbHelper.ExecuteScalar(sb.Return());
            }
            if (obj != null)
            {
                result = obj.ToInt() == 0;
            }

            return result;
        }
        #endregion
    }
}