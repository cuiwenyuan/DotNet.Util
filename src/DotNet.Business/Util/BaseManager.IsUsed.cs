//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Business
{
    using Util;

    /// <summary>
    ///	BaseManager
    /// 通用基类部分
    /// 
    /// 修改记录
    /// 
    ///		2020.07.03 版本：Troy.Cui进行扩展。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2020.07.03</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region 是否被用过

        /// <summary>
        /// 是否被用过
        /// </summary>
        /// <param name="id">行id</param>
        /// <param name="targetTableName">要检查的关联表名</param>
        /// <param name="tableFieldName">要检查的关联表字段名</param>
        /// <param name="currentTableField">当前表的关联字段名（默认为Id）</param>
        /// <param name="checkUserCompany">是否检查公司数据</param>
        /// <returns>是否</returns>
        public virtual bool IsUsed(string id, string targetTableName, string tableFieldName, string currentTableField = BaseUtil.FieldId, bool checkUserCompany = false)
        {
            var result = false;
            if (dbHelper.TableExists(targetTableName) && ValidateUtil.IsInt(id))
            {
                var sb = PoolUtil.StringBuilder.Get();
                sb.Append("SELECT COUNT(*) FROM " + CurrentTableName + " A INNER JOIN " + targetTableName + " B ON B." + tableFieldName + " = A." + currentTableField + " WHERE A.Id = " + id + "");
                //未删除
                sb.Append(" AND A." + (BaseUtil.FieldDeleted) + " = 0 AND A." + BaseUtil.FieldEnabled + " = 1");
                sb.Append(" AND B." + (BaseUtil.FieldDeleted) + " = 0 AND B." + BaseUtil.FieldEnabled + " = 1");
                //当前用户所在公司或者系统公用数据
                if (checkUserCompany)
                {
                    sb.Append(" AND (A.UserCompanyId = 0 OR A.UserCompanyId = " + UserInfo.CompanyId + ")");
                    sb.Append(" AND (B.UserCompanyId = 0 OR B.UserCompanyId = " + UserInfo.CompanyId + ")");
                }
                var obj = ExecuteScalar(sb.Return());
                if (obj != null && obj.ToInt() > 0)
                {
                    result = true;
                }
            }
            return result;
        }
        #endregion

        #region 是否被用过（批量）

        /// <summary>
        /// 是否被用过（批量）
        /// </summary>
        /// <param name="ids">行Ids</param>
        /// <param name="targetTableName">要检查的关联表名</param>
        /// <param name="targetTableField">要检查的关联表字段名</param>
        /// <param name="currentTableField">当前表的关联字段名（默认为Id）</param>
        /// <param name="checkUserCompany">是否检查公司数据</param>
        /// <returns>是否</returns>
        public virtual bool IsUsed(string[] ids, string targetTableName, string targetTableField, string currentTableField = BaseUtil.FieldId, bool checkUserCompany = false)
        {
            var result = false;
            foreach (var i in ids)
            {
                result = IsUsed(i, targetTableName, targetTableField, currentTableField: currentTableField, checkUserCompany: checkUserCompany);
                if (result)
                {
                    return result;
                }
            }

            return result;

        }
        #endregion

        #region 是否被用过

        /// <summary>
        /// 是否被用过
        /// </summary>
        /// <param name="id">行id</param>
        /// <param name="targetTableName">要检查的关联表名</param>
        /// <param name="targetTableField1">要检查的关联表字段名1</param>
        /// <param name="targetTableField2">要检查的关联表字段名2</param>
        /// <param name="currentTableField1">当前表的关联字段名1（默认为Id）</param>
        /// <param name="currentTableField2">当前表的关联字段名2（默认为Id）</param>
        /// <param name="checkUserCompany">是否检查公司数据</param>
        /// <returns>是否</returns>
        public virtual bool IsUsed(string id, string targetTableName, string targetTableField1, string targetTableField2, string currentTableField1 = BaseUtil.FieldId, string currentTableField2 = BaseUtil.FieldId, bool checkUserCompany = false)
        {
            var result = false;
            if (dbHelper.TableExists(targetTableName) && ValidateUtil.IsInt(id))
            {
                var sb = PoolUtil.StringBuilder.Get();
                sb.Append("SELECT COUNT(*) FROM " + CurrentTableName + " A INNER JOIN " + targetTableName + " B ON B." + targetTableField1 + " = A." + currentTableField1 + "");
                if (!currentTableField2.IsNullOrEmpty() && !currentTableField2.IsNullOrEmpty())
                {
                    sb.Append(" AND B." + targetTableField2 + " = A." + currentTableField2 + "");
                }
                sb.Append(" WHERE A.Id = " + id.ToInt() + "");
                //未删除
                sb.Append(" AND A." + BaseUtil.FieldDeleted + " = 0 AND A." + BaseUtil.FieldEnabled + " = 1");
                sb.Append(" AND B." + BaseUtil.FieldDeleted + " = 0 AND B." + BaseUtil.FieldEnabled + " = 1");
                //当前用户所在公司或者系统公用数据
                if (checkUserCompany)
                {
                    sb.Append(" AND (A.UserCompanyId = 0 OR A.UserCompanyId = " + UserInfo.CompanyId + ")");
                    sb.Append(" AND (B.UserCompanyId = 0 OR B.UserCompanyId = " + UserInfo.CompanyId + ")");
                }
                var obj = ExecuteScalar(sb.Return());
                if (obj != null && obj.ToInt() > 0)
                {
                    result = true;
                }
            }
            return result;
        }
        #endregion

        #region 是否被用过（批量）

        /// <summary>
        /// 是否被用过（批量）
        /// </summary>
        /// <param name="ids">行Ids</param>
        /// <param name="targetTableName">要检查的关联表名</param>
        /// <param name="targetTableField1">要检查的关联表字段名1</param>
        /// <param name="targetTableField2">要检查的关联表字段名2</param>
        /// <param name="currentTableField1">当前表的关联字段名（默认为Id）</param>
        ///  <param name="currentTableField2">当前表的关联字段名（默认为Id）</param>
        /// <param name="checkUserCompany">是否检查公司数据</param>
        /// <returns>是否</returns>
        public virtual bool IsUsed(string[] ids, string targetTableName, string targetTableField1, string targetTableField2, string currentTableField1 = BaseUtil.FieldId, string currentTableField2 = BaseUtil.FieldId, bool checkUserCompany = false)
        {
            var result = false;
            foreach (var i in ids)
            {
                result = IsUsed(i, targetTableName, targetTableField1, targetTableField2, currentTableField1: currentTableField1, currentTableField2: currentTableField2, checkUserCompany: checkUserCompany);
                if (result)
                {
                    return result;
                }
            }

            return result;

        }
        #endregion
    }
}